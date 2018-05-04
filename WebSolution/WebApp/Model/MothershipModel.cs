using RandomNameGeneratorLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;
using FanOutClassLibrary.Messages;

namespace WebApp.Model
{
    public class MothershipModel : BaseDeviceModel
    {
        public readonly List<ClientModel> Clients = new List<ClientModel>();
        private static readonly PersonNameGenerator s_personNameGenerator = new PersonNameGenerator();

        public MothershipModel(WebSocket webSocket, string name) : base(webSocket, name)
        {

        }

        public ClientModel TryCreateClient(WebSocket socket)
        {
            ClientModel newClient;

            lock (Clients)
            {
                string newName = GetNewClientName();

                newClient = new ClientModel(socket, newName, this);
                Clients.Add(newClient);
            }

            newClient.StartConnection();

            // Let the mothership know the client has connected
            Send(new MothershipClientConnectedMessage()
            {
                ClientName = newClient.Name
            });

            return newClient;
        }

        public void RemoveClient(ClientModel client)
        {
            lock (Clients)
            {
                Clients.Remove(client);
            }

            Send(new MothershipClientDisconnectedMessage()
            {
                ClientName = client.Name
            });
        }

        private string GetNewClientName()
        {
            while (true)
            {
                string newName = s_personNameGenerator.GenerateRandomFirstName();

                // Skip names that are already in use
                if (!Clients.Any(i => i.Name == newName))
                {
                    return newName;
                }
            }
        }

        public override async void StartConnection()
        {
            Send(new MothershipNameAssignedMessage()
            {
                Name = Name
            });
        }

        public DateTime LastTimeMessageReceived { get; private set; } = DateTime.UtcNow;

        protected override void OnMessageReceived(BaseMessage message)
        {
            // If mothership is requesting to send a card
            if (message is MothershipSendCardMessage)
            {
                LastTimeMessageReceived = DateTime.UtcNow;
                FanOutCard(message as MothershipSendCardMessage);
            }
        }

        private void FanOutCard(MothershipSendCardMessage message)
        {
            lock (Clients)
            {
                foreach (var c in Clients)
                {
                    FanOutCardToClient(c, message);
                }
            }
        }

        private async void FanOutCardToClient(ClientModel client, MothershipSendCardMessage message)
        {
            try
            {
                // Send it to the client
                await client.SendCardAsync(message);

                try
                {
                    // And then let the mothership know it was received
                    Send(new MothershipClientReceivedCardMessage()
                    {
                        CardIdentifier = message.CardIdentifier,
                        ClientName = client.Name
                    });
                }
                catch { }
            }
            catch (Exception ex)
            {
                // Let mothership know it failed to send
                try
                {
                    Send(new MothershipFailedToSendCardToClientMessage()
                    {
                        ClientName = client.Name,
                        CardIdentifier = message.CardIdentifier,
                        ErrorText = ex.Message
                    });
                }
                catch { }
            }
        }

        public void CloseAndRemove()
        {
            CloseSocket();
        }

        protected override void OnSocketClosed()
        {
            AllMothershipsModel.RemoveMothership(this);
        }
    }
}
