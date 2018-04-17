using FanOutClassLibrary.Messages;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        protected async void Send(BaseMessage message)
        {
            try
            {
                await SendAsync(message);
            }
            catch { }
        }

        private Task _prevSendTask;
        protected Task SendAsync(BaseMessage message)
        {
            Task answer;
            lock (this)
            {
                answer = SendAsyncHelper(message);
                _prevSendTask = answer;
            }
            return answer;
        }

        private async Task SendAsyncHelper(BaseMessage message)
        {
            // Make sure we don't send interspersed
            Task taskToWaitFor = _prevSendTask;

            if (taskToWaitFor != null)
            {
                try
                {
                    await taskToWaitFor;
                }
                catch { }
            }

            try
            {
                var bytes = Encoding.UTF8.GetBytes(message.ToJson());
                byte[][] chunks = BreakIntoChunks(bytes);

                foreach (var chunk in chunks)
                {
                    await m_webSocket.SendAsync(new ArraySegment<byte>(chunk, 0, chunk.Length), WebSocketMessageType.Text, chunk == chunks.Last(), CancellationToken.None);
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
            int chunkSize = 4000;
            for (int i = 0; i < originalBytes.Length; i += chunkSize)
            {
                answer.Add(originalBytes.Skip(i).Take(chunkSize).ToArray());
            }
            return answer.ToArray();
        }

        public async Task RunReceiveLoopAsync()
        {
            var buffer = new byte[Startup.RECEIVE_BUFFER_SIZE];

            while (m_webSocket != null)
            {
                string text = "";

                try
                {
                    while (true)
                    {
                        var result = await m_webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                        if (result.CloseStatus.HasValue)
                        {
                            CloseSocket();
                            return;
                        }

                        text += Encoding.UTF8.GetString(buffer, 0, result.Count);

                        if (result.EndOfMessage)
                        {
                            break;
                        }
                    }

                    var message = BaseMessage.FromJson(text);

                    OnMessageReceived(message);
                }
                catch (WebSocketException)
                {
                    CloseSocket();
                }
                catch (Exception ex)
                {
                    Trace.TraceError("InvalidMessage. Text: " + text);
                    Send(new InvalidMessageReceivedMessage()
                    {
                        Error = ex.Message,
                        TextLength = text.Length
                    });
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
