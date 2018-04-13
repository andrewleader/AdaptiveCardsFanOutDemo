using FanOutUwpClassLibrary.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.Foundation.Collections;

namespace MothershipApp.AppServices
{
    public class AppServiceHandler
    {
        public static List<AppServiceHandler> Handlers { get; private set; } = new List<AppServiceHandler>();

        private AppServiceConnection _connection;
        private BackgroundTaskDeferral _deferral;

        private AppServiceHandler(IBackgroundTaskInstance taskInstance, AppServiceConnection connection, BackgroundTaskDeferral deferral)
        {
            taskInstance.Canceled += TaskInstance_Canceled;

            connection.RequestReceived += Connection_RequestReceived;

            _connection = connection;
            _deferral = deferral;
        }

        private void TaskInstance_Canceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            Handlers.Remove(this);
        }

        private void Connection_RequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {

        }

        public static async void Handle(IBackgroundTaskInstance taskInstance)
        {
            try
            {
                var triggerDetails = taskInstance.TriggerDetails as AppServiceTriggerDetails;
                if (triggerDetails == null)
                {
                    return;
                }

                var deferral = taskInstance.GetDeferral();

                if (App.MainViewModel == null)
                {
                    await triggerDetails.AppServiceConnection.SendMessageAsync(new ValueSet()
                    {
                        { "Error", "Mothership app isn't launched." }
                    });

                    triggerDetails.AppServiceConnection.Dispose();
                    deferral.Complete();
                    return;
                }

                Handlers.Add(new AppServiceHandler(taskInstance, triggerDetails.AppServiceConnection, deferral));
            }
            catch { }
        }

        public static Task SendCardToAllClientsAsync(CardViewModel card)
        {
            var clients = Handlers.ToList();

            List<Task> results = new List<Task>();

            foreach (var c in clients)
            {
                results.Add(c.SendCardToClientAsync(card));
            }

            // Wait at most 2 seconds
            return Task.Run(delegate { Task.WaitAll(results.ToArray(), 2000); });
        }

        private async Task SendCardToClientAsync(CardViewModel card)
        {
            try
            {
                await _connection.SendMessageAsync(new ValueSet()
                {
                    { "CardJson", card.CardJson }
                });
            }
            catch { }
        }
    }
}
