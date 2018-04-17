using FanOutClassLibrary.Messages;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebApp.Model
{
    public abstract class BaseDeviceModel
    {
        private WebSocket m_webSocket;

        public BaseDeviceModel(WebSocket webSocket, string name)
        {
            m_webSocket = webSocket;
            Name = name;
        }

        public string Name { get; private set; }

        public abstract void StartConnection();

        protected void Send(BaseMessage message)
        {
            var dontWait = SendAsync(message);
        }

        protected async Task SendAsync(BaseMessage message)
        {
            try
            {
                var bytes = Encoding.UTF8.GetBytes(message.ToJson());

                await m_webSocket.SendAsync(new ArraySegment<byte>(bytes, 0, bytes.Length), WebSocketMessageType.Text, true, CancellationToken.None);
            }
            catch (WebSocketException)
            {
                CloseSocket();
            }
            catch { }
        }

        public async Task RunReceiveLoopAsync()
        {
            var buffer = new byte[Startup.RECEIVE_BUFFER_SIZE];

            while (m_webSocket != null)
            {
                try
                {
                    var result = await m_webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                    if (result.CloseStatus.HasValue)
                    {
                        CloseSocket();
                        return;
                    }

                    string text = Encoding.UTF8.GetString(buffer, 0, result.Count);

                    var message = BaseMessage.FromJson(text);

                    OnMessageReceived(message);
                }
                catch (WebSocketException)
                {
                    CloseSocket();
                }
                catch (Exception)
                {

                }
            }
        }

        protected virtual void OnMessageReceived(BaseMessage message)
        {
            // Parents should implement
        }

        private void CloseSocket()
        {
            try
            {
                try
                {
                    var closeTask = m_webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                }
                catch { }
                m_webSocket = null;
                OnSocketClosed();
            }
            catch { }
        }

        protected abstract void OnSocketClosed();
    }
}
