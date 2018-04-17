using FanOutClassLibrary;
using FanOutClassLibrary.Messages;
using FanOutDeviceClassLibrary;
using FanOutUwpClassLibrary;
using FanOutUwpClassLibrary.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;

namespace MothershipApp.ViewModels
{
    public class MainViewModel : BindableBase
    {
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
                System.Diagnostics.Debugger.Break();
                Clients.HandleSendFailed((message as MothershipFailedToSendCardToClientMessage).CardIdentifier);
            }
        }

        private string m_name = "Connecting...";
        public string Name
        {
            get { return m_name; }
            set { SetProperty(ref m_name, value); }
        }

        public ObservableCollection<CardViewModel> UpNextCards { get; private set; } = new ObservableCollection<CardViewModel>();

        private CardViewModel _currentCard;
        public CardViewModel CurrentCard
        {
            get { return _currentCard; }
            set { SetProperty(ref _currentCard, value); }
        }

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

            while (mainViewModel.AddUpNextCardIfNeeded()) { }

            mainViewModel.MoveToNextCard();

            // Start the loop
            connectingPage.WriteLog("Starting the card loop...");
            mainViewModel.CycleLoop();

            return mainViewModel;
        }

        private async void CycleLoop()
        {
            while (true)
            {
                await Task.Delay(5000);

                // Send current card
                if (CurrentCard != null)
                {
                    var cardToSend = CurrentCard;
                    CurrentCard = null;
                    await SendCardAsync(cardToSend);

                    // And then wait a bit more so we don't instantly switch
                    await Task.Delay(1000);
                }

                MoveToNextCard();
            }
        }

        private async Task SendCardAsync(CardViewModel card)
        {
            try
            {
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
                System.Diagnostics.Debugger.Break();
                Clients.HandleSendFailed();
            }
        }

        private void MoveToNextCard()
        {
            var card = UpNextCards.First().Clone() as CardViewModel;
            UpNextCards.RemoveAt(0);

            CurrentCard = card;

            AddUpNextCardIfNeeded();
        }

        private bool AddUpNextCardIfNeeded()
        {
            if (UpNextCards.Count >= 5)
            {
                return false;
            }

            UpNextCards.Add(FindNewCardFromGallery());
            return true;
        }

        private CardViewModel FindNewCardFromGallery()
        {
            var notInUpNextQueue = GalleryCards.Where(gc => !UpNextCards.Any(unc => unc.Equals(gc)) && (CurrentCard == null || !CurrentCard.Equals(gc))).ToArray();

            return notInUpNextQueue[new Random().Next(0, notInUpNextQueue.Length)].Clone() as CardViewModel;
        }

        public double CardWidth => App.CardWidth;
    }
}
