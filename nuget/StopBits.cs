using System;
using System.Collections.Generic;
using System.Text;

namespace System.IO.Ports
{
    /// <summary>
    /// Stop bits
    /// note that only One and Two are implemented
    /// </summary>
    public enum StopBits
    {
        None = 0,
        One = 1,
        Two = 2,
        OnePointFive = 3
    };
}
