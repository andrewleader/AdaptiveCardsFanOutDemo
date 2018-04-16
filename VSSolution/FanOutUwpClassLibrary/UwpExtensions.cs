using FanOutDeviceClassLibrary;
using FanOutDeviceClassLibrary.ViewModels;
using FanOutUwpClassLibrary.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace FanOutUwpClassLibrary
{
    public static class UwpExtensions
    {
        public static void Initialize()
        {
            CrossPlatformCardViewModel.CreateInstanceFunction = delegate { return new CardViewModel(); };
            SimpleDispatcher.RunAsyncImplementation = RunAsync;
        }

        public static async Task RunAsync(Action action)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, delegate
            {
                action();
            });
        }
    }
}
