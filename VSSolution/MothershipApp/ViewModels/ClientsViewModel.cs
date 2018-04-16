using FanOutClassLibrary.Messages;
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

        public void HandleClientConnected(MothershipClientConnectedMessage msg)
        {
            Clients.Add(new ClientViewModel(msg.ClientName));
        }

        public void HandleClientDisconnected(MothershipClientDisconnectedMessage msg)
        {
            var c = FindClient(msg.ClientName);
            if (c != null)
            {
                Clients.Remove(c);
            }
        }

        public void HandleClientReceivedCard(MothershipClientReceivedCardMessage msg)
        {
            FindClient(msg.ClientName)?.HandleReceivedCard(msg.CardIdentifier);
        }

        public void HandleSentCardToClients(Guid cardIdentifier)
        {
            foreach (var c in Clients)
            {
                c.HandleCardSentToClient(cardIdentifier);
            }
        }

        private ClientViewModel FindClient(string clientName)
        {
            return Clients.FirstOrDefault(i => i.DisplayName == clientName);
        }
    }
}
