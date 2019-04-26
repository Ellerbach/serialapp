using System;
using System.Collections.Generic;
using System.Text;

namespace System.IO.Ports

{
    /// <summary>
    /// Standard parity. Please note that there is no control
    /// done on the combination of parity and number of bits.
    /// Some are incompatible
    /// </summary>
    public enum Parity
    {
        None = 0,
        Odd = 1,
        Even = 2,
        Mark = 3,
        Space = 4
    };
}