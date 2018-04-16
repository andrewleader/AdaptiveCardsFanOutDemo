using System;
using System.Collections.Generic;
using System.Text;

namespace FanOutClassLibrary.Messages
{
    public class ClientNameAssignedMessage : BaseMessage
    {
        public string ClientName { get; set; }
    }
}
