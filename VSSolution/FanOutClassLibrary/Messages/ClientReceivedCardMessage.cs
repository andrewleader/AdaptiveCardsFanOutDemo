using System;
using System.Collections.Generic;
using System.Text;

namespace FanOutClassLibrary.Messages
{
    public class ClientReceivedCardMessage : BaseMessage
    {
        public Guid CardIdentifier { get; set; }
    }
}
