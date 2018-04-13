using FanOutUwpClassLibrary.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UwpClientApp.ViewModels;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Core;
using Windows.System.RemoteSystems;
using Windows.UI.Xaml;

namespace UwpClientApp.AppServices
{
    public static class AppServiceClient
    {
        private static AppServiceConnection _connection;
        private static string _remoteSystemId;
        private static RemoteSystemWatcher _remoteSystemWatcher;

        public static async Task DiscoverAsync()
        {
            var accessStatus = await RemoteSystem.RequestAccessAsync();
            if (accessStatus != RemoteSystemAccessStatus.Allowed)
            {
                throw new Exception("RemoteSystem access not allowed: " + accessStatus);
            }

            List<IRemoteSystemFilter> filters = new List<IRemoteSystemFilter>()
            {
                // Only allow nearby proximal connections
                //new RemoteSystemDiscoveryTypeFilter(RemoteSystemDiscoveryType.SpatiallyProximal),

                //// Only look for desktops (since Mothership will only be running on desktop)
                //new RemoteSystemKindFilter(new string[] { RemoteSystemKinds.Desktop })
            };

            _remoteSystemWatcher = RemoteSystem.CreateWatcher(filters);
            _remoteSystemWatcher.RemoteSystemAdded += _remoteSystemWatcher_RemoteSystemAdded;
            _remoteSystemWatcher.RemoteSystemRemoved += _remoteSystemWatcher_RemoteSystemRemoved;
            _remoteSystemWatcher.Start();
        }

        private static void _remoteSystemWatcher_RemoteSystemRemoved(RemoteSystemWatcher sender, RemoteSystemRemovedEventArgs args)
        {
            if (_connection != null && args.RemoteSystemId == _remoteSystemId)
            {
                _connection.Dispose();
                _connection = null;
                _remoteSystemId = null;
            }
        }

        private static async void _remoteSystemWatcher_RemoteSystemAdded(RemoteSystemWatcher sender, RemoteSystemAddedEventArgs args)
        {
            if (_connection == null)
            {
                try
                {
                    //if (await args.RemoteSystem.GetCapabilitySupportedAsync(KnownRemoteSystemCapabilities.AppService))
                    if (args.RemoteSystem.DisplayName == "DESKTOP-OJFOA42")
                    {
                        var connection = new AppServiceConnection()
                        {
                            AppServiceName = "com.microsoft.adaptiveCardsMothership",
                            PackageFamilyName = "Microsoft.AdaptiveCardsMothership_88tf7eadxdb5m"
                        };
                        _connection = connection;

                        connection.RequestReceived += Connection_RequestReceived;

                        var status = await connection.OpenRemoteAsync(new RemoteSystemConnectionRequest(args.RemoteSystem));
                        if (status != AppServiceConnectionStatus.Success)
                        {
                            _connection = null;
                            throw new Exception(status.ToString());
                        }
                    }
                }
                catch
                {
                }
            }
        }

        private static void Connection_RequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            var dontWait = CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, delegate
            {
                if (args.Request.Message.ContainsKey("CardJson"))
                {
                    string cardJson = args.Request.Message["CardJson"] as string;

                    MainViewModel.Current.AddCard(new CardViewModel()
                    {
                        CardJson = cardJson
                    });
                }

                else
                {
                    var dontWaitAgain = new Windows.UI.Popups.MessageDialog(args.Request.Message["Error"] as string).ShowAsync();
                }
            });
        }
    }
}
