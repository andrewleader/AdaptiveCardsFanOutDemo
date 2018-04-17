using FanOutDeviceClientClassLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace UwpClientApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ConnectingPage : Page
    {
        public ConnectingPage()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            string mothershipName = e.Parameter as string;

            try
            {
                ClientConnection.Current.OnConnectionClosed = delegate
                {
                    Frame.Navigate(typeof(ConnectToMothershipsPage));
                };

                await ClientConnection.Current.ConnectAsync(mothershipName, delegate
                {
                    Frame.Navigate(typeof(MainPage));
                });
            }
            catch (Exception ex)
            {
                await new MessageDialog(ex.ToString()).ShowAsync();

                Frame.Navigate(typeof(ConnectToMothershipsPage));
            }
        }
    }
}
