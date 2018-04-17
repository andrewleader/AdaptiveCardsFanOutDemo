using System;
using System.Collections.Generic;
using System.Text;

namespace FanOutClassLibrary.Messages
{
    public class MothershipFailedToSendCardToClientMessage : BaseMessage
    {
        public string ClientName { get; set; }

        public string ErrorText { get; set; }
    }
}
