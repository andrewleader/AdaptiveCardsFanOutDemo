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
        public string DisplayName { get; private set; }

        private bool _isSending;
        public bool IsSending
        {
            get { return _isSending; }
            set { SetProperty(ref _isSending, value); NotifyPropertyChanged(nameof(IsReceived)); }
        }

        public bool IsReceived
        {
            get { return !IsSending; }
        }

        public ClientViewModel(string name)
        {
            DisplayName = name;
        }

        public void HandleReceivedCard(Guid cardIdentifier)
        {
            if (m_currentCardSentToClient == cardIdentifier)
            {
                m_currentCardSentToClient = Guid.Empty;
                IsSending = false;
            }
        }

        private Guid m_currentCardSentToClient;

        public void HandleCardSentToClient(Guid cardIdentifier)
        {
            m_currentCardSentToClient = cardIdentifier;
            IsSending = true;
        }
    }
}
