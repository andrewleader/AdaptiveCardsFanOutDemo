using FanOutClassLibrary.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace WebApp.Model
{
    public class ClientModel : BaseDeviceModel
    {
        public readonly MothershipModel Mothership;

        public ClientModel(WebSocket webSocket, string name, MothershipModel mothersip) : base(webSocket, name)
        {
            Mothership = mothersip;
        }

        public Task SendCardAsync(MothershipSendCardMessage message)
        {
            return SendAsync(message);
        }

        public void NotifyMothershipDisconnected()
        {
            Send(new MothershipDisconnectedMessage());
        }

        protected override void OnSocketClosed()
        {
            Mothership.RemoveClient(this);
        }

        public override void StartConnection()
        {
            Send(new ClientNameAssignedMessage()
            {
                ClientName = Name
            });
        }
    }
}
