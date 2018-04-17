using System;
using System.Collections.Generic;
using System.Text;

namespace FanOutClassLibrary.Messages
{
    public class InvalidMessageReceivedMessage : BaseMessage
    {
        public string Error { get; set; }
        public int TextLength { get; set; }
    }
}
