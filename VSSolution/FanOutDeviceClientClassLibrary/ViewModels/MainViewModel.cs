using FanOutClassLibrary;
using FanOutDeviceClassLibrary.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace FanOutDeviceClientClassLibrary.ViewModels
{
    public class MainViewModel : BindableBase
    {
        public Action<CrossPlatformCardViewModel> OnCardReceived;

        public static readonly MainViewModel Current = new MainViewModel();

        public ObservableCollection<CrossPlatformCardViewModel> Cards { get; private set; } = new ObservableCollection<CrossPlatformCardViewModel>();

        private string m_name;
        public string Name
        {
            get { return m_name; }
            set { SetProperty(ref m_name, value); }
        }

        private MainViewModel() { }

        public void AddCard(CrossPlatformCardViewModel card)
        {
            // Only display at most 15
            while (Cards.Count > 14)
            {
                Cards.RemoveAt(0);
            }

            Cards.Add(card);

            OnCardReceived?.Invoke(card);
        }
    }
}
