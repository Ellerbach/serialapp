using System;
using System.Collections.Generic;
using System.Text;

namespace System.IO.Ports
{
    /// <summary>
    /// Standard handshake
    /// </summary>
    public enum Handshake
    {
        None,
        XOnXOff,
        RequestToSend,
        RequestToSendXOnXOff
    };
}
