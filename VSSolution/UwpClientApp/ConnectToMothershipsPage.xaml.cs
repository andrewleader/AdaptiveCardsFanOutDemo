using FanOutDeviceClientClassLibrary.ViewModels;
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
    public sealed partial class ConnectToMothershipsPage : Page
    {
        public ConnectToMothershipsPage()
        {
            this.InitializeComponent();

            DataContext = new ConnectToMothershipsViewModel();
        }

        public ConnectToMothershipsViewModel ViewModel => DataContext as ConnectToMothershipsViewModel;

        private void ListViewMotherships_ItemClick(object sender, ItemClickEventArgs e)
        {
            string mothershipName = e.ClickedItem as string;

            Frame.Navigate(typeof(ConnectingPage), mothershipName);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            Refresh();
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            Refresh();
        }

        private async void Refresh()
        {
            ListViewMotherships.Visibility = Visibility.Collapsed;
            RefreshingContent.Visibility = Visibility.Visible;

            try
            {
                await ViewModel.RefreshMothershipsAsync();
            }
            catch (Exception ex)
            {
                var dontWait = new MessageDialog(ex.ToString()).ShowAsync();
            }

            ListViewMotherships.ItemsSource = ViewModel.MothershipNames;
            ListViewMotherships.Visibility = Visibility.Visible;
            RefreshingContent.Visibility = Visibility.Collapsed;
        }
    }
}
