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
        public readonly string DisplayName;

        private bool _isSending;
        public bool IsSending
        {
            get { return _isSending; }
            set { SetProperty(ref _isSending, value); }
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
            }
        }

        private Guid m_currentCardSentToClient;

        public void HandleCardSentToClient(Guid cardIdentifier)
        {
            m_currentCardSentToClient = cardIdentifier;
        }
    }
}
