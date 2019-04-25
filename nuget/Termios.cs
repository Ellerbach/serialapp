using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace System.IO.Ports
{
    internal struct Termios
    {
        public uint InputFlag;
        public uint OutputFlag;
        public uint ControlFlag;
        public uint LocalFalg;
        public byte ControlCharacter00;
        public byte ControlCharacter01;
        public byte ControlCharacter02;
        public byte ControlCharacter03;
        public byte ControlCharacter04;
        public byte ControlCharacter05;
        public byte ControlCharacter06;
        public byte ControlCharacter07;
        public byte ControlCharacter08;
        public byte ControlCharacter09;
        public byte ControlCharacter10;
        public byte ControlCharacter11;
        public byte ControlCharacter12;
        public byte ControlCharacter13;
        public byte ControlCharacter14;
        public byte ControlCharacter15;
        public byte ControlCharacter16;
        public byte ControlCharacter17;
        public byte ControlCharacter18;
        public byte ControlCharacter19;
    }

    internal class TermiosHelpers
    {
        public static byte[] getBytes(Termios str)
        {
            int size = Marshal.SizeOf(str);
            byte[] arr = new byte[size];

            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(str, ptr, true);
            Marshal.Copy(ptr, arr, 0, size);
            Marshal.FreeHGlobal(ptr);
            return arr;
        }

        public static Termios fromBytes(byte[] arr)
        {
            Termios str = new Termios();
            //Debug.WriteLine(Marshal.SizeOf(str));
            int size = Marshal.SizeOf(str);
            IntPtr ptr = Marshal.AllocHGlobal(size);

            Marshal.Copy(arr, 0, ptr, size);

            str = (Termios)Marshal.PtrToStructure(ptr, str.GetType());
            Marshal.FreeHGlobal(ptr);

            return str;
        }
    }
}
