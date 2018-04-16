using FanOutClassLibrary;
using FanOutClassLibrary.Messages;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FanOutDeviceClassLibrary
{
    public class DeviceSocketConnection
    {
        public event EventHandler<BaseMessage> OnMessageReceived;
        public event EventHandler OnSocketClosed;

        private static ClientWebSocket m_webSocket;

        private DeviceSocketConnection(ClientWebSocket webSocket)
        {
            m_webSocket = webSocket;
        }

        public static async Task<DeviceSocketConnection> CreateAsync(string url)
        {
            ClientWebSocket webSocket = new ClientWebSocket();
            await webSocket.ConnectAsync(new Uri(url), CancellationToken.None);

            return new DeviceSocketConnection(webSocket);
        }

        public async void Send(BaseMessage message)
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

        public async void RunReceiveLoop()
        {
            var buffer = new byte[WebUrls.RECEIVE_BUFFER_SIZE];

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

                    OnMessageReceived?.Invoke(this, message);
                }
                catch (WebSocketException ex)
                {
                    CloseSocket();
                }
                catch (Exception)
                {

                }
            }
        }

        public void CloseSocket()
        {
            try
            {
                try
                {
                    var closeTask = m_webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                }
                catch { }
                m_webSocket = null;
                OnSocketClosed?.Invoke(this, null);
            }
            catch { }
        }

        public static async Task CloseAnySocketAsync()
        {
            if (m_webSocket != null)
            {
                try
                {
                    await m_webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                }
                catch { }
                m_webSocket = null;
            }
        }
    }
}
