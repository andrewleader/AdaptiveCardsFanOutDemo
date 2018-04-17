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
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace MothershipApp.ViewModels
{
    public class ClientViewModel : BindableBase
    {
        private static readonly Brush FailedBrush = new SolidColorBrush(Colors.Red);
        private static readonly Brush SuccessBrush = new SolidColorBrush(Colors.Green);
        private static readonly Brush PendingBrush = new SolidColorBrush(Color.FromArgb(255, 242, 186, 0));

        public string DisplayName { get; private set; }

        private Brush m_backgroundBrush = SuccessBrush;
        public Brush BackgroundBrush
        {
            get { return m_backgroundBrush; }
            set { SetProperty(ref m_backgroundBrush, value); }
        }

        private string m_status = "Connected";
        public string Status
        {
            get { return m_status; }
            set { SetProperty(ref m_status, value); }
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
                Status = "Received!";
                BackgroundBrush = SuccessBrush;
            }
        }

        public void HandleSendFailed()
        {
            Status = "FAILED";
            BackgroundBrush = FailedBrush;
        }

        private Guid m_currentCardSentToClient;

        public void HandleCardSentToClient(Guid cardIdentifier)
        {
            if (m_currentCardSentToClient == cardIdentifier)
            {
                m_currentCardSentToClient = cardIdentifier;
                Status = "Sent...";
                BackgroundBrush = PendingBrush;
            }
        }

        public void HandleCardSendingToClient(Guid cardIdentifier)
        {
            m_currentCardSentToClient = cardIdentifier;
            Status = "Sending...";
            BackgroundBrush = PendingBrush;
        }
    }
}
