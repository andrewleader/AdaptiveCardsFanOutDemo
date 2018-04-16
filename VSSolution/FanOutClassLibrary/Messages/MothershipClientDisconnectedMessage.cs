using System;
using System.Collections.Generic;
using System.Text;

namespace FanOutClassLibrary.Messages
{
    public class MothershipClientDisconnectedMessage : BaseMessage
    {
        public string ClientName { get; set; }
    }
}
