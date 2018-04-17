using FanOutClassLibrary;
using FanOutClassLibrary.Messages;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
            webSocket.Options.SetBuffer(WebUrls.RECEIVE_BUFFER_SIZE, WebUrls.RECEIVE_BUFFER_SIZE);
            await webSocket.ConnectAsync(new Uri(url), CancellationToken.None);

            return new DeviceSocketConnection(webSocket);
        }

        public async Task SendAsync(BaseMessage message)
        {
            try
            {
                string json = message.ToJson();
                var bytes = Encoding.UTF8.GetBytes(json);

                if (bytes.Length > WebUrls.RECEIVE_BUFFER_SIZE)
                {
                    throw new Exception("Message too large");
                }

                byte[][] chunks = BreakIntoChunks(bytes);
                foreach (var chunk in chunks)
                {
                    await m_webSocket.SendAsync(new ArraySegment<byte>(chunk), WebSocketMessageType.Text, chunk == chunks.Last(), CancellationToken.None);
                }
            }
            catch (WebSocketException)
            {
                CloseSocket();
            }
        }

        private static byte[][] BreakIntoChunks(byte[] originalBytes)
        {
            List<byte[]> answer = new List<byte[]>();
            int chunkSize = 4 * 1024;
            for (int i = 0; i < originalBytes.Length; i += chunkSize)
            {
                answer.Add(originalBytes.Skip(i).Take(chunkSize).ToArray());
            }
            return answer.ToArray();
        }

        public async void RunReceiveLoop()
        {
            var buffer = new byte[WebUrls.RECEIVE_BUFFER_SIZE];

            while (m_webSocket != null)
            {
                try
                {
                    var result = await m_webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    Debug.WriteLine("Received message");

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
                catch (Exception ex)
                {
                    Debug.WriteLine("Receive failed: " + ex.Message);
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
