using FanOutClassLibrary;
using FanOutClassLibrary.Messages;
using FanOutDeviceClassLibrary;
using FanOutDeviceClassLibrary.ViewModels;
using FanOutDeviceClientClassLibrary.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FanOutDeviceClientClassLibrary
{
    public class ClientConnection
    {
        public static readonly ClientConnection Current = new ClientConnection();

        private DeviceSocketConnection m_deviceSocketConnection;

        public Action<string> OnConnectionClosed;
        private Action m_onNameAssignedAction;

        public async Task ConnectAsync(string mothershipName, Action onNameAssignedAction)
        {
            m_onNameAssignedAction = onNameAssignedAction;
            m_deviceSocketConnection = await DeviceSocketConnection.CreateAsync(WebUrls.ClientSocketUrl(mothershipName));
            m_deviceSocketConnection.OnMessageReceived += M_deviceSocketConnection_OnMessageReceived;
            m_deviceSocketConnection.OnSocketClosed += M_deviceSocketConnection_OnSocketClosed;
            m_deviceSocketConnection.RunReceiveLoop();
        }

        private void M_deviceSocketConnection_OnSocketClosed(object sender, EventArgs e)
        {
            m_deviceSocketConnection.OnSocketClosed -= M_deviceSocketConnection_OnSocketClosed;
            m_deviceSocketConnection.OnMessageReceived -= M_deviceSocketConnection_OnMessageReceived;
            m_deviceSocketConnection = null;

            var dontWait = SimpleDispatcher.RunAsync(delegate
            {
                try
                {
                    OnConnectionClosed?.Invoke("Socket closed");
                }
                catch { }
            });
        }

        private void M_deviceSocketConnection_OnMessageReceived(object sender, BaseMessage e)
        {
            var dontWait = SimpleDispatcher.RunAsync(delegate
            {
                try
                {
                    if (e is MothershipSendCardMessage)
                    {
                        var cardMessage = e as MothershipSendCardMessage;
                        var card = CrossPlatformCardViewModel.CreateInstanceFunction();
                        card.CardIdentifier = cardMessage.CardIdentifier;
                        card.CardJson = cardMessage.CardJson;

                        MainViewModel.Current.AddCard(card);
                    }

                    else if (e is ClientNameAssignedMessage)
                    {
                        MainViewModel.Current.Name = (e as ClientNameAssignedMessage).ClientName;
                        m_onNameAssignedAction?.Invoke();
                    }

                    else if (e is MothershipDisconnectedMessage)
                    {
                        OnConnectionClosed?.Invoke("Mothership has been disconnected");
                    }
                }
                catch { }
            });
        }
    }
}
