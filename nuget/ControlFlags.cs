using System;
using System.Collections.Generic;
using System.Text;

namespace System.IO.Ports
{
    [Flags]
    internal enum ControlFlags : uint
    {
        CIGNORE = (1 << 0), // Ignore these control flags.  
        CSIZE = (CS5 | CS6 | CS7 | CS8),    // Number of bits per byte =(mask).  
        CS5 = 0,            // 5 bits per byte.  
        CS6 = (1 << 8),     // 6 bits per byte.  
        CS7 = (1 << 9),     // 7 bits per byte.  
        CS8 = (CS6 | CS7),  // 8 bits per byte.  
        CSTOPB = (1 << 10), // Two stop bits instead of one.  
        CREAD = (1 << 11),  // Enable receiver.  
        PARENB = (1 << 12), // Parity enable.  
        PARODD = (1 << 13), // Odd parity instead of even.  
        HUPCL = (1 << 14),  // Hang up on last close.  
        CLOCAL = (1 << 15), // Ignore modem status lines.  
        CRTSCTS = (1 << 16),    // RTS/CTS flow control.  
        CDTRCTS = (1 << 17),    // DTR/CTS flow control.  
        MDMBUF = (1 << 20), // DTR/DCD flow control.  
        CHWFLOW = (MDMBUF | CRTSCTS | CDTRCTS), // All types of flow control.  
    }
}
