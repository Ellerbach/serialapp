using System;
using System.Collections.Generic;
using System.Text;

namespace System.IO.Ports
{
    [Flags]
    internal enum InputFlags : uint
    {
        IGNBRK = (1 << 0),      // Ignore break condition
        BRKINT = (1 << 1),      // Signal interrupt on break
        IGNPAR = (1 << 2),      // Ignore characters with parity errors
        PARMRK = (1 << 3),      // Mark parity and framing errors
        INPCK = (1 << 4),       // Enable input parity check
        ISTRIP = (1 << 5),      // Strip 8th bit off characters
        INLCR = (1 << 6),       // Map NL to CR on input
        IGNCR = (1 << 7),       // Ignore CR
        ICRNL = (1 << 8),       // Map CR to NL on input
        IXON = (1 << 9),        // Enable start/stop output control
        IXOFF = (1 << 10),      // Enable start/stop input control
        IXANY = (1 << 11),      // Any character will restart after stop
        IMAXBEL = (1 << 13),    // Ring bell when input queue is full
        IUCLC = (1 << 14)	    // Translate upper case input to lower case
    }
}
