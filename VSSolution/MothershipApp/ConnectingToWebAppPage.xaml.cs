using MothershipApp.ViewModels;
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

namespace MothershipApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ConnectingToWebAppPage : Page
    {
        public ConnectingToWebAppPage()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Create the main view model
            try
            {
                App.MainViewModel = await MainViewModel.CreateAsync(this);
                Frame.Navigate(typeof(MainPage));
            }
            catch (Exception ex)
            {
                await new MessageDialog(ex.ToString()).ShowAsync();
                Application.Current.Exit();
            }
        }

        public void WriteLog(string text)
        {
            TextBlockStatus.Text = text;
        }
    }
}
