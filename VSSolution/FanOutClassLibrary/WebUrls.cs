using System;
using System.Collections.Generic;
using System.Text;

namespace FanOutClassLibrary
{
    public class WebUrls
    {
        //public const string BASE_URL = "http://localhost:60969/";
        public const string BASE_URL = "https://cardfanout.azurewebsites.net/";

        public static readonly string BASE_WS_URL = BASE_URL.Replace("http://", "ws://").Replace("https://", "wss://");

        public static readonly string MOTHERSHIP_SOCKET_URL = BASE_WS_URL + "wsMothership";

        public static string ClientSocketUrl(string mothershipName)
        {
            return BASE_WS_URL + "wsClient/" + mothershipName;
        }

        /// <summary>
        /// 10 KB
        /// </summary>
        public const int RECEIVE_BUFFER_SIZE = 10 * 1024;
    }
}
