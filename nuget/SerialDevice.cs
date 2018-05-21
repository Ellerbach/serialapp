using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace System.IO.Ports
{
    public class SerialDevice
    {
        public const int READING_BUFFER_SIZE = 1024;

        private readonly CancellationTokenSource cts = new CancellationTokenSource();
        private CancellationToken CancellationToken => cts.Token;

        private int? fd;
        private readonly IntPtr readingBuffer = Marshal.AllocHGlobal(READING_BUFFER_SIZE);

        protected readonly string portName;

        protected readonly BaudRate baudRate;

        public event Action<object, byte[]> DataReceived;
        public SerialDevice(string portName, BaudRate baudRate)
        {
            this.portName = portName;
            this.baudRate = baudRate;
        }

        public void Open()
        {
            // open serial port
            int fd = Libc.open(portName, Libc.OpenFlags.O_RDWR | Libc.OpenFlags.O_NONBLOCK);

            if (fd == -1)
            {
                throw new Exception($"failed to open port ({portName})");
            }

            // set baud rate
            byte[] termiosData = new byte[256];

            Libc.tcgetattr(fd, termiosData);
            Libc.cfsetspeed(termiosData, baudRate);
            Libc.tcsetattr(fd, 0, termiosData);
            // start reading
            Task.Run((Action)StartReading, CancellationToken);
            this.fd = fd;
        }

        private void StartReading()
        {
            if (!fd.HasValue)
            {
                throw new Exception();
            }

            while (true)
            {
                CancellationToken.ThrowIfCancellationRequested();

                int res = Libc.read(fd.Value, readingBuffer, READING_BUFFER_SIZE);

                if (res != -1)
                {
                    byte[] buf = new byte[res];
                    Marshal.Copy(readingBuffer, buf, 0, res);

                    OnDataReceived(buf);
                }

                Thread.Sleep(50);
            }
        }

        protected virtual void OnDataReceived(byte[] data)
        {
            DataReceived?.Invoke(this, data);
        }

        public bool IsOpened => fd.HasValue;

        public void Close()
        {
            if (!fd.HasValue)
            {
                throw new Exception();
            }

            cts.Cancel();
            Libc.close(fd.Value);
            Marshal.FreeHGlobal(readingBuffer);
        }

        public void Write(byte[] buf)
        {
            if (!fd.HasValue)
            {
                throw new Exception();
            }

            IntPtr ptr = Marshal.AllocHGlobal(buf.Length);
            Marshal.Copy(buf, 0, ptr, buf.Length);
            Libc.write(fd.Value, ptr, buf.Length);
            Marshal.FreeHGlobal(ptr);
        }

        public static string[] GetPortNames()
        {
            int p = (int)Environment.OSVersion.Platform;
            List<string> serial_ports = new List<string>();

            // Are we on Unix?
            if (p == 4 || p == 128 || p == 6)
            {
                string[] ttys = System.IO.Directory.GetFiles("/dev/", "tty*");
                foreach (string dev in ttys)
                {
                    //Arduino MEGAs show up as ttyACM due to their different USB<->RS232 chips
                    if (dev.StartsWith("/dev/ttyS") || dev.StartsWith("/dev/ttyUSB") || dev.StartsWith("/dev/ttyACM") || dev.StartsWith("/dev/ttyAMA"))
                    {
                        serial_ports.Add(dev);
                        //Console.WriteLine("Serial list: {0}", dev);
                    }
                }
            }
            
            return serial_ports.ToArray();
        }

        public void Dispose()
        {
            if (IsOpened)
            {
                Close();
            }
        }
    }
}