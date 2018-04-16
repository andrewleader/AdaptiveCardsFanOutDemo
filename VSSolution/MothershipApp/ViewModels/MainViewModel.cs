using FanOutClassLibrary;
using FanOutUwpClassLibrary.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;

namespace MothershipApp.ViewModels
{
    public class MainViewModel : BindableBase
    {
        public ObservableCollection<CardViewModel> UpNextCards { get; private set; } = new ObservableCollection<CardViewModel>();

        private CardViewModel _currentCard;
        public CardViewModel CurrentCard
        {
            get { return _currentCard; }
            set { SetProperty(ref _currentCard, value); }
        }

        public ObservableCollection<CardViewModel> GalleryCards { get; private set; } = new ObservableCollection<CardViewModel>();

        public ClientsViewModel Clients { get; private set; } = ClientsViewModel.Current;

        public static async Task<MainViewModel> CreateAsync()
        {
            CardViewModel.HOST_CONFIG_JSON = await FileIO.ReadTextAsync(await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///HostConfigs/HostConfig.json")));

            var mainViewModel = new MainViewModel();

            var cardsFolder = await Package.Current.InstalledLocation.GetFolderAsync("Cards");
            foreach (var file in await cardsFolder.GetFilesAsync())
            {
                if (file.FileType.ToLower().Equals(".json"))
                {
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
            await Clients.SendCardToAllClientsAsync(card);
        }

        private void MoveToNextCard()
        {
            var card = UpNextCards.First().Clone();
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

            return notInUpNextQueue[new Random().Next(0, notInUpNextQueue.Length)].Clone();
        }

        public double CardWidth => App.CardWidth;
    }
}
