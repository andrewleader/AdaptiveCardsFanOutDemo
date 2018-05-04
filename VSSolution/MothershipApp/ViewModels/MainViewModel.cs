using FanOutClassLibrary;
using FanOutClassLibrary.Messages;
using FanOutDeviceClassLibrary;
using FanOutUwpClassLibrary;
using FanOutUwpClassLibrary.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;

namespace MothershipApp.ViewModels
{
    public class MainViewModel : BindableBase
    {
        public class ChangingCardsArgs
        {
            public CardViewModel RemovedCard { get; set; }
            public CardViewModel NewCurrentCard { get; set; }
            public CardViewModel NewQueuedCard { get; set; }
        }

        public MainViewModel(DeviceSocketConnection deviceSocketConnection)
        {
            m_deviceSocketConnection = deviceSocketConnection;
            deviceSocketConnection.OnMessageReceived += DeviceSocketConnection_OnMessageReceived;
            deviceSocketConnection.OnSocketClosed += DeviceSocketConnection_OnSocketClosed;
            deviceSocketConnection.RunReceiveLoop();
        }

        private void DeviceSocketConnection_OnSocketClosed(object sender, EventArgs e)
        {
            var dontWait = SimpleDispatcher.RunAsync(async delegate
            {
                await new MessageDialog("Connection closed. Please re-open the app.").ShowAsync();
                Application.Current.Exit();
            });
        }

        public void ShowUserSelectedCard(CardViewModel card)
        {
            // Clone to new instance
            card = card.Clone() as CardViewModel;

            if (QueuedAndCurrentCards.Count < 2)
            {
                QueuedAndCurrentCards.Insert(0, card);
            }
            else
            {
                QueuedAndCurrentCards[0] = card;
            }

            SendNextCardImmediately();
        }

        private void DeviceSocketConnection_OnMessageReceived(object sender, BaseMessage e)
        {
            var dontWait = SimpleDispatcher.RunAsync(delegate
            {
                OnMessageReceived(e);
            });
        }

        private void OnMessageReceived(BaseMessage message)
        {
            if (message is MothershipNameAssignedMessage)
            {
                Name = (message as MothershipNameAssignedMessage).Name;
            }

            else if (message is MothershipClientConnectedMessage)
            {
                Clients.HandleClientConnected(message as MothershipClientConnectedMessage);
            }

            else if (message is MothershipClientDisconnectedMessage)
            {
                Clients.HandleClientDisconnected(message as MothershipClientDisconnectedMessage);
            }

            else if (message is MothershipClientReceivedCardMessage)
            {
                Clients.HandleClientReceivedCard(message as MothershipClientReceivedCardMessage);
            }

            else if (message is MothershipFailedToSendCardToClientMessage)
            {
                Debugger.Break();
                Clients.HandleSendFailed((message as MothershipFailedToSendCardToClientMessage).CardIdentifier);
            }

            else if (message is InvalidMessageReceivedMessage)
            {
                Debug.WriteLine("InvalidMessage: " + (message as InvalidMessageReceivedMessage).Error + " TextLength: " + (message as InvalidMessageReceivedMessage).TextLength);
                Clients.HandleSendFailed(m_lastSentCardIdentifier);
            }
        }

        private string m_name = "Connecting...";
        public string Name
        {
            get { return m_name; }
            set { SetProperty(ref m_name, value); }
        }

        public ObservableCollection<CardViewModel> QueuedAndCurrentCards { get; private set; } = new ObservableCollection<CardViewModel>();

        public ObservableCollection<CardViewModel> GalleryCards { get; private set; } = new ObservableCollection<CardViewModel>();

        public ClientsViewModel Clients { get; private set; } = ClientsViewModel.Current;

        private DeviceSocketConnection m_deviceSocketConnection;

        public static async Task<MainViewModel> CreateAsync(ConnectingToWebAppPage connectingPage)
        {
            connectingPage.WriteLog("Loading host config...");
            CardViewModel.HOST_CONFIG_JSON = await FileIO.ReadTextAsync(await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///HostConfigs/HostConfig.json")));

            DeviceSocketConnection deviceSocketConnection;
            try
            {
                connectingPage.WriteLog("Connecting to socket...");
                deviceSocketConnection = await DeviceSocketConnection.CreateAsync(WebUrls.MOTHERSHIP_SOCKET_URL);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to connect to the web server: " + WebUrls.MOTHERSHIP_SOCKET_URL + "\n\n" + ex.ToString());
            }

            connectingPage.WriteLog("Socket connected!");

            var mainViewModel = new MainViewModel(deviceSocketConnection);

            connectingPage.WriteLog("Getting cards folder...");
            var cardsFolder = await Package.Current.InstalledLocation.GetFolderAsync("Cards");
            connectingPage.WriteLog("Enumerating cards...");
            foreach (var file in await cardsFolder.GetFilesAsync())
            {
                if (file.FileType.ToLower().Equals(".json"))
                {
                    connectingPage.WriteLog("Loading card payload...");
                    mainViewModel.GalleryCards.Add(new CardViewModel()
                    {
                        Name = file.DisplayName,
                        CardJson = await FileIO.ReadTextAsync(file)
                    });
                }
            }

            mainViewModel.QueuedAndCurrentCards.Add(mainViewModel.FindNewCardFromGallery());
            mainViewModel.QueuedAndCurrentCards.Add(mainViewModel.FindNewCardFromGallery());

            // Start the loop
            connectingPage.WriteLog("Starting the card loop...");
            mainViewModel.CycleLoop();

            return mainViewModel;
        }

        private CancellationTokenSource m_cycleDelayCancellationTokenSource;

        private async void CycleLoop()
        {
            while (true)
            {
                m_cycleDelayCancellationTokenSource = new CancellationTokenSource();

                try
                {
                    await Task.Delay(5000, m_cycleDelayCancellationTokenSource.Token);
                }
                catch (OperationCanceledException) { }

                // Add a new queued card if needed
                if (QueuedAndCurrentCards.Count <= 2)
                {
                    QueuedAndCurrentCards.Insert(0, FindNewCardFromGallery());
                }

                // If we have enough cards
                if (QueuedAndCurrentCards.Count >= 2)
                {
                    // Remove the current
                    QueuedAndCurrentCards.Remove(QueuedAndCurrentCards.Last());

                    // Grab the to-send one
                    var toSend = QueuedAndCurrentCards.Last();

                    // Send the new current
                    await SendCardAsync(toSend);
                }
            }
        }

        private void SendNextCardImmediately()
        {
            // Wait a little to let the UI update so that the send animation occurs
            m_cycleDelayCancellationTokenSource?.CancelAfter(200);
        }

        private Guid m_lastSentCardIdentifier;
        private async Task SendCardAsync(CardViewModel card)
        {
            try
            {
                m_lastSentCardIdentifier = card.CardIdentifier;
                Clients.HandleSendingCardToClients(card.CardIdentifier);

                await m_deviceSocketConnection.SendAsync(new MothershipSendCardMessage()
                {
                    CardIdentifier = card.CardIdentifier,
                    CardJson = card.CardJson
                });

                Clients.HandleSentCardToClients(card.CardIdentifier);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Send failed: " + ex.Message);
                System.Diagnostics.Debugger.Break();
                Clients.HandleSendFailed(card.CardIdentifier);
            }
        }

        private CardViewModel FindNewCardFromGallery()
        {
            var notInUpNextQueue = GalleryCards.Where(gc => !QueuedAndCurrentCards.Any(existing => existing.Equals(gc))).ToArray();

            return notInUpNextQueue[new Random().Next(0, notInUpNextQueue.Length)].Clone() as CardViewModel;
        }

        public double CardWidth => App.CardWidth;
    }
}
