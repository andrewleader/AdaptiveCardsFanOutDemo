using FanOutDeviceClassLibrary.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace FanOutDeviceClientClassLibrary.ViewModels
{
    public class MainViewModel
    {
        public static readonly MainViewModel Current = new MainViewModel();

        public ObservableCollection<CrossPlatformCardViewModel> Cards { get; private set; } = new ObservableCollection<CrossPlatformCardViewModel>();

        private MainViewModel() { }

        public void AddCard(CrossPlatformCardViewModel card)
        {
            Cards.Add(card);
            // TODO: Clean old cards
        }
    }
}
