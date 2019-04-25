using System;
using System.Collections.Generic;
using System.Text;

namespace System.IO.Ports
{
    [Flags]
    internal enum OutputFlags : uint
    {
        OPOST = (1 << 0),   // Perform output processing.
        ONLCR = (1 << 1),   // Map NL to CR-NL on output.
        ONOEOT = (1 << 3),  // Discard EOT (^D) on output.
        OCRNL = (1 << 4),   // Map CR to NL.
        ONOCR = (1 << 5),   // Discard CR's when on column 0.
        ONLRET = (1 << 6),  // Move to column 0 on NL.
        NLDLY = (3 << 8),   // NL delay.
        NL0 = (0 << 8),     // NL type 0.
        NL1 = (1 << 8),     // NL type 1.
        TABDLY = (3 << 10 | 1 << 2),    // TAB delay.
        TAB0 = (0 << 10),   // TAB delay type 0.
        TAB1 = (1 << 10),   // TAB delay type 1.
        TAB2 = (2 << 10),   // TAB delay type 2.
        TAB3 = (1 << 2),    // Expand tabs to spaces.
        CRDLY = (3 << 12),  // CR delay.
        CR0 = (0 << 12),    // CR delay type 0.
        CR1 = (1 << 12),    // CR delay type 1.
        CR2 = (2 << 12),    // CR delay type 2.
        CR3 = (3 << 12),    // CR delay type 3.
        FFDLY = (1 << 14),  // FF delay.
        FF0 = (0 << 14),    // FF delay type 0.
        FF1 = (1 << 14),    // FF delay type 1.
        BSDLY = (1 << 15),  // BS delay.
        BS0 = (0 << 15),    // BS delay type 0.
        BS1 = (1 << 15),    // BS delay type 1.
        VTDLY = (1 << 16),  // VT delay.
        VT0 = (0 << 16),    // VT delay type 0.
        VT1 = (1 << 16),    // VT delay type 1.
        OLCUC = (1 << 17),  // Translate lower case output to upper case
        OFILL = (1 << 18),  // Send fill characters for delays.
        OFDEL = (1 << 19),  // Fill is DEL.
    }
}
