using System;
using System.Collections.Generic;
using System.Text;

namespace System.IO.Ports
{
    [Flags]
    internal enum LocalFlags : uint
    {
        ECHOKE = (1 << 0),  // Visual erase for KILL.  
        ECHOE = (1 << 1),  // Visual erase for ERASE.  
        ECHOK = (1 << 2),  // Echo NL after KILL.  
        ECHO = (1 << 3),  // Enable echo.  
        ECHONL = (1 << 4),  // Echo NL even if ECHO is off.  
        ECHOPRT = (1 << 5),  // Hardcopy visual erase.  
        ECHOCTL = (1 << 6),  // Echo control characters as ^X.  
        ISIG = (1 << 7),  // Enable signals.  
        ICANON = (1 << 8),  // Do erase and kill processing.  
        ALTWERASE = (1 << 9),  // Alternate WERASE algorithm.  
        IEXTEN = (1 << 10),  // Enable DISCARD and LNEXT.  
        EXTPROC = (1 << 11),  // External processing.  
        TOSTOP = (1 << 22),  // Send SIGTTOU for background output.  
        FLUSHO = (1 << 23),  // Output being flushed =(state).  
        XCASE = (1 << 24),  // Canonical upper/lower case.  
        NOKERNINFO = (1 << 25),  // Disable VSTATUS.  
        PENDIN = (1 << 29),  // Retype pending input =(state).  
        NOFLSH = ((uint)1 << 31),  // Disable flush after interrupt.  
    }
}
