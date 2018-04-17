using FanOutDeviceClassLibrary.ViewModels;
using FanOutDeviceClientClassLibrary.ViewModels;
using FanOutUwpClassLibrary.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UwpClientApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            DataContext = MainViewModel.Current;

            MainViewModel.Current.OnCardReceived = OnCardReceived;
        }

        private void OnCardReceived(CrossPlatformCardViewModel card)
        {
            try
            {
                EventHandler<object> handler = null;
                handler = delegate
                {
                    try
                    {
                        ListViewCards.ScrollIntoView(card);
                        (card as CardViewModel).CardFrameworkElement.LayoutUpdated -= handler;
                    }
                    catch { }
                };
                (card as CardViewModel).CardFrameworkElement.LayoutUpdated += handler;
            }
            catch { }
        }
    }
}
