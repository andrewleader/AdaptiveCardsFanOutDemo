using System;
using System.Collections.Generic;
using System.Text;

namespace FanOutClassLibrary.Messages
{
    public class MothershipSendCardMessage : BaseMessage
    {
        public Guid CardIdentifier { get; set; }

        public string CardJson { get; set; }
    }
}
