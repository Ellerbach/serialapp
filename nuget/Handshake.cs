using System;
using System.Collections.Generic;
using System.Text;

namespace System.IO.Ports
{
    public enum Handshake
    {
        None,
        XOnXOff,
        RequestToSend,
        RequestToSendXOnXOff
    };
}
