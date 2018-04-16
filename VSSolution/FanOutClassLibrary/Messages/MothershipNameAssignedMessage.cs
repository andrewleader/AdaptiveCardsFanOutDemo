using System;
using System.Collections.Generic;
using System.Text;

namespace FanOutClassLibrary.Messages
{
    public class MothershipNameAssignedMessage : BaseMessage
    {
        public string Name { get; set; }
    }
}
