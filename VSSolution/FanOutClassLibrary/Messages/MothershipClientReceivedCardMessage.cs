using System;
using System.Collections.Generic;
using System.Text;

namespace FanOutClassLibrary.Messages
{
    public class MothershipClientReceivedCardMessage : BaseMessage
    {
        public string ClientName { get; set; }

        public Guid CardIdentifier { get; set; }
    }
}
