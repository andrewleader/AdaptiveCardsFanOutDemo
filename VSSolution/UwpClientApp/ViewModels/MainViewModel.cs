using FanOutUwpClassLibrary.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UwpClientApp.ViewModels
{
    public class MainViewModel
    {
        public static readonly MainViewModel Current = new MainViewModel();

        public ObservableCollection<CardViewModel> Cards { get; private set; } = new ObservableCollection<CardViewModel>();

        private MainViewModel() { }

        public void AddCard(CardViewModel card)
        {
            Cards.Add(card);
            // TODO: Clean old cards
        }
    }
}
