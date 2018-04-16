using System;
using System.Collections.Generic;
using System.Text;

namespace FanOutClassLibrary.Messages
{
    public class MothershipClientConnectedMessage : BaseMessage
    {
        public string ClientName { get; set; }
    }
}
