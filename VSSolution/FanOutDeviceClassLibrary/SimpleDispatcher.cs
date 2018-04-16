using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FanOutDeviceClassLibrary
{
    public static class SimpleDispatcher
    {
        public static Func<Action, Task> RunAsyncImplementation { get; set; }

        public static async Task RunAsync(Action action)
        {
            try
            {
                await RunAsyncImplementation(action);
            }
            catch { }
        }
    }
}
