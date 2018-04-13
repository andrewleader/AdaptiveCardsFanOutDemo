using FanOutUwpClassLibrary.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.Foundation.Collections;

namespace MothershipApp.ViewModels
{
    public class ClientsViewModel
    {
        public static readonly ClientsViewModel Current = new ClientsViewModel();

        public ObservableCollection<ClientViewModel> Clients { get; private set; } = new ObservableCollection<ClientViewModel>();

        public async void HandleAppServiceConnection(IBackgroundTaskInstance taskInstance)
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

                Clients.Add(new ClientViewModel(taskInstance, triggerDetails.AppServiceConnection, deferral));
            }
            catch { }
        }

        public Task SendCardToAllClientsAsync(CardViewModel card)
        {
            var clients = Clients.ToList();

            List<Task> results = new List<Task>();

            foreach (var c in clients)
            {
                results.Add(c.SendCardToClientAsync(card));
            }

            // Wait at most 2 seconds
            return Task.Run(delegate { Task.WaitAll(results.ToArray(), 2000); });
        }
    }
}
