using FanOutClassLibrary;
using FanOutUwpClassLibrary.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.Foundation.Collections;

namespace MothershipApp.ViewModels
{
    public class ClientViewModel : BindableBase
    {
        private static int s_clientIdentifier = 1;

        public string DisplayName { get; private set; } = (s_clientIdentifier++).ToString();

        private AppServiceResponseStatus _responseStatus = AppServiceResponseStatus.Unknown;
        public AppServiceResponseStatus ResponseStatus
        {
            get { return _responseStatus; }
            set { SetProperty(ref _responseStatus, value); }
        }

        private bool _isSending;
        public bool IsSending
        {
            get { return _isSending; }
            set { SetProperty(ref _isSending, value); }
        }

        private AppServiceConnection _connection;
        private BackgroundTaskDeferral _deferral;

        public ClientViewModel(IBackgroundTaskInstance taskInstance, AppServiceConnection connection, BackgroundTaskDeferral deferral)
        {
            taskInstance.Canceled += Connection_Canceled;

            _connection = connection;
            _deferral = deferral;
        }

        private void Connection_Canceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            ClientsViewModel.Current.Clients.Remove(this);
        }

        public async Task SendCardToClientAsync(CardViewModel card)
        {
            try
            {
                IsSending = true;
                var response = await _connection.SendMessageAsync(new ValueSet()
                {
                    { "CardJson", card.CardJson }
                });
                IsSending = false;

                ResponseStatus = response.Status;
            }
            catch { }
        }
    }
}
