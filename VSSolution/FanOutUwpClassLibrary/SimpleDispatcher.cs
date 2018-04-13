using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.UI.Core;

namespace FanOutUwpClassLibrary
{
    public static class SimpleDispatcher
    {
        public static async Task RunAsync(DispatchedHandler callback)
        {
            try
            {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, callback);
            }
            catch { }
        }
    }
}
