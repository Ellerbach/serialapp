using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace System.IO.Ports
{
    public class SerialDevice : IDisposable
    {

        private const int MaxDataBits = 8;
        private const int MinDataBits = 5;
        private const bool DefaultDtrEnable = false;
        private const bool DefaultRtsEnable = false;
        private const int DefaultDataBits = 8;
        private const Parity DefaultParity = Parity.None;
        private const StopBits DefaultStopBits = StopBits.One;
        private const Handshake DefaultHandshake = Handshake.None;
        public const int DefaultReadBuffer = 4096;

        private readonly CancellationTokenSource cts = new CancellationTokenSource();
        private CancellationToken _CancellationToken => cts.Token;
        private int? _FileDescriptor;
        private readonly IntPtr _ReadingBuffer = Marshal.AllocHGlobal(DefaultReadBuffer);
        protected readonly string _PortName;
        protected BaudRate _BaudRate;
        protected int _DataBits = DefaultDataBits;
        protected Parity _Parity = DefaultParity;
        protected StopBits _StopBits = DefaultStopBits;
        protected Handshake _Handshake = DefaultHandshake;

        public event Action<object, byte[]> DataReceived;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="portName">Name of the serial port</param>
        /// <param name="baudRate">Baud rate</param>
        /// <param name="parity">PArity</param>
        /// <param name="dataBits">Data bits</param>
        /// <param name="stopBits">Stop bits</param>
        public SerialDevice(string portName, BaudRate baudRate, Parity parity, int dataBits, StopBits stopBits) : this(portName, baudRate, parity, dataBits, stopBits, DefaultHandshake)
        { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="portName">Name of the serial port</param>
        /// <param name="baudRate">Baud rate</param>
        /// <param name="parity">PArity</param>
        /// <param name="dataBits">Data bits</param>
        public SerialDevice(string portName, BaudRate baudRate, Parity parity, int dataBits) : this(portName, baudRate, parity, dataBits, DefaultStopBits, DefaultHandshake)
        { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="portName">Name of the serial port</param>
        /// <param name="baudRate">Baud rate</param>
        /// <param name="parity">PArity</param>
        public SerialDevice(string portName, BaudRate baudRate, Parity parity) : this(portName, baudRate, parity, DefaultDataBits, DefaultStopBits, DefaultHandshake)
        { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="portName">Name of the serial port</param>
        /// <param name="baudRate">Baud rate</param>
        public SerialDevice(string portName, BaudRate baudRate) : this(portName, baudRate, DefaultParity, DefaultDataBits, DefaultStopBits, DefaultHandshake)
        { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="portName">Name of the serial port</param>
        /// <param name="baudRate">Baud rate</param>
        /// <param name="parity">PArity</param>
        /// <param name="dataBits">Data bits</param>
        /// <param name="stopBits">Stop bits</param>
        /// <param name="handshake">Handshake</param>
        public SerialDevice(string portName, BaudRate baudRate, Parity parity, int dataBits, StopBits stopBits, Handshake handshake)
        {
            _PortName = portName;
            _BaudRate = baudRate;
            _Parity = parity;
            _DataBits = dataBits;
            _StopBits = stopBits;
            _Handshake = handshake;
        }

        /// <summary>
        /// Baud rate
        /// All Baud rate may not be supported depending on your hardware
        /// </summary>
        public BaudRate BaudRate
        {
            get { return _BaudRate; }
            set
            {
                _BaudRate = value;
                if (IsOpen)
                {
                    SetBaudRate(_BaudRate);
                }
            }
        }

        /// <summary>
        /// Data bits from 5 to 8
        /// </summary>
        public int DataBits
        {
            get { return _DataBits; }
            set
            {
                if ((value < MinDataBits) || value > MaxDataBits) throw new ArgumentException($"{nameof(DataBits)} can only be between {MinDataBits} and {MaxDataBits}");
                _DataBits = value;
                if (IsOpen)
                {
                    SetDataBits(_DataBits);
                }
            }
        }

        /// <summary>
        /// Parity
        /// </summary>
        public Parity Parity
        {
            get { return _Parity; }
            set
            {
                _Parity = value;
                if (IsOpen)
                {
                    SetParity(_Parity);
                }
            }
        }

        /// <summary>
        /// Stop bits
        /// </summary>
        public StopBits StopBits
        {
            get { return _StopBits;  }
            set
            {
                _StopBits = value;
                if (IsOpen)
                {
                    SetStopBits(_StopBits);
                }
            }
        }

        /// <summary>
        /// Handshake
        /// </summary>
        public Handshake Handshake
        {
            get { return _Handshake;  }
            set
            {
                _Handshake = value;
                if (IsOpen)
                {
                    SetHandshake(_Handshake);
                }
            }
        }

        /// <summary>
        /// Is the port open
        /// </summary>
        public bool IsOpen => _FileDescriptor.HasValue;

        /// <summary>
        /// Open the serial port
        /// </summary>
        public void Open()
        {
            if (IsOpen) throw new IOException($"Port already open");

            // open serial port
            int fd = Libc.open(_PortName, Libc.OpenFlags.O_RDWR | Libc.OpenFlags.O_NONBLOCK);
            if (fd == -1) throw new Exception($"failed to open port ({_PortName})");

            _FileDescriptor = fd;
            // Initilise the serial port
            Termios termios = GetTermios();
            // Make sure we are ok to read
            termios.ControlFlag |= (uint)ControlFlags.CREAD;
            termios.ControlFlag |= (uint)ControlFlags.CLOCAL;
            //Clean the output flags
            termios.OutputFlag &= ~(uint)OutputFlags.OPOST;
            // Clear what can be on the input flags
            termios.InputFlag &= ~(uint)(InputFlags.IGNBRK | InputFlags.BRKINT | InputFlags.ICRNL |
                        InputFlags.PARMRK | InputFlags.INLCR | InputFlags.ISTRIP | InputFlags.IXON);
            // Set ICANON off
            termios.LocalFalg &= ~(uint)LocalFlags.ICANON;
            // Set ECHO off
            termios.LocalFalg &= ~(uint)LocalFlags.ECHO;
            termios.LocalFalg &= ~(uint)LocalFlags.ECHOE;
            termios.LocalFalg &= ~(uint)LocalFlags.ECHOCTL;
            termios.LocalFalg &= ~(uint)LocalFlags.IEXTEN;
            termios.LocalFalg &= ~(uint)LocalFlags.ISIG;
            SetTermios(termios);
            SetBaudRate(_BaudRate);
            SetDataBits(_DataBits);
            SetParity(_Parity);
            SetStopBits(_StopBits);
            // start reading
            Task.Run((Action)StartReading, _CancellationToken);

        }

        private Termios GetTermios()
        {
            byte[] termiosData = new byte[Marshal.SizeOf(typeof(Termios))];
            Libc.tcgetattr(_FileDescriptor.Value, termiosData);
            return TermiosHelpers.fromBytes(termiosData);
        }

        private void SetTermios(Termios terminos)
        {
            byte[] termiosData = TermiosHelpers.getBytes(terminos);            
            Libc.tcsetattr(_FileDescriptor.Value, 0, termiosData);
        }

        private void SetBaudRate(BaudRate baudRate)
        {
            byte[] termiosData = new byte[Marshal.SizeOf(typeof(Termios))];
            Libc.tcgetattr(_FileDescriptor.Value, termiosData);
            Libc.cfsetspeed(termiosData, baudRate);
        }

        private void SetParity(Parity parity)
        {
            Termios termios = GetTermios();
            switch (parity)
            {
                default:
                case Parity.None:
                    termios.ControlFlag &= ~(uint)ControlFlags.PARENB;
                    termios.ControlFlag &= ~(uint)ControlFlags.PARODD;
                    //termios.InputFlag &= ~(uint)InputFlags.INPCK; 
                    //termios.InputFlag &= ~(uint)InputFlags.PARMRK; 
                    break;
                case Parity.Odd:
                    termios.ControlFlag |= (uint)ControlFlags.PARENB;
                    termios.ControlFlag |= (uint)ControlFlags.PARODD;
                    //termios.InputFlag |= (uint)InputFlags.INPCK;
                    //termios.InputFlag &= ~(uint)InputFlags.PARMRK;
                    break;
                case Parity.Even:
                    termios.ControlFlag |= (uint)ControlFlags.PARENB;
                    termios.ControlFlag &= ~(uint)ControlFlags.PARODD;
                    //termios.InputFlag |= (uint)InputFlags.INPCK;
                    //termios.InputFlag &= ~(uint)InputFlags.PARMRK;
                    break;
                case Parity.Mark:
                    termios.ControlFlag |= (uint)ControlFlags.PARENB;
                    termios.ControlFlag |= (uint)ControlFlags.PARODD;
                    termios.ControlFlag |= (uint)ControlFlags.CMSPAR;
                    //termios.InputFlag |= (uint)InputFlags.INPCK;
                    //termios.InputFlag |= (uint)InputFlags.PARMRK;
                    break;
                case Parity.Space:
                    termios.ControlFlag |= (uint)ControlFlags.PARENB;
                    termios.ControlFlag &= ~(uint)ControlFlags.PARODD;
                    termios.ControlFlag |= (uint)ControlFlags.CMSPAR;
                    //termios.InputFlag |= (uint)InputFlags.INPCK;
                    //termios.InputFlag |= (uint)InputFlags.PARMRK;
                    break;
            }
            SetTermios(termios);
        }

        private void SetDataBits(int dataBits)
        {
            Termios termios = GetTermios();
            termios.ControlFlag &= ~(uint)ControlFlags.CSIZE;
            switch (dataBits)
            {
                case 5:
                    termios.ControlFlag |= (uint)ControlFlags.CS5;
                    break;
                case 6:
                    termios.ControlFlag |= (uint)ControlFlags.CS6;
                    break;
                case 7:
                    termios.ControlFlag |= (uint)ControlFlags.CS7;
                    break;
                default:
                case 8:
                    termios.ControlFlag |= (uint)ControlFlags.CS8;
                    break;
            }
            SetTermios(termios);
        }

        private void SetStopBits(StopBits stopBits)
        {
            Termios termios = GetTermios();
            switch (stopBits)
            {
                
                case StopBits.One:
                    termios.ControlFlag &= ~(uint)ControlFlags.CSTOPB;
                    break;
                case StopBits.Two:
                    termios.ControlFlag |= (uint)ControlFlags.CSTOPB;
                    break;
                default:
                case StopBits.None:
                case StopBits.OnePointFive:
                    throw new ArgumentException($"StopBits only support One or Two");
                    break;
            }
            SetTermios(termios);
        }

        private void SetHandshake(Handshake handshake)
        {
            Termios termios = GetTermios();
            switch (handshake)
            {
                case Handshake.None:
                    termios.ControlFlag &= ~(uint)(ControlFlags.CRTSCTS | ControlFlags.CDTRCTS);
                    termios.InputFlag &= ~(uint)(InputFlags.IXON | InputFlags.IXOFF);
                    break;
                case Handshake.XOnXOff:
                    termios.ControlFlag &= ~(uint)(ControlFlags.CRTSCTS | ControlFlags.CDTRCTS);
                    termios.InputFlag |= (uint)(InputFlags.IXON | InputFlags.IXOFF);
                    break;
                case Handshake.RequestToSend:
                    termios.ControlFlag |= (uint)ControlFlags.CRTSCTS;
                    termios.ControlFlag &= (uint)ControlFlags.CDTRCTS;
                    termios.InputFlag &= ~(uint)(InputFlags.IXON | InputFlags.IXOFF);
                    break;
                case Handshake.RequestToSendXOnXOff:
                    termios.ControlFlag |= (uint)ControlFlags.CRTSCTS;
                    termios.ControlFlag &= (uint)ControlFlags.CDTRCTS;
                    termios.InputFlag |= (uint)(InputFlags.IXON | InputFlags.IXOFF);
                    break;
                default:
                    break;
            }
            SetTermios(termios);
        }

        private void StartReading()
        {
            if (!_FileDescriptor.HasValue) throw new Exception($"${nameof(SerialDevice)} not open");
            

            while (true)
            {
                _CancellationToken.ThrowIfCancellationRequested();

                int res = Libc.read(_FileDescriptor.Value, _ReadingBuffer, DefaultReadBuffer);

                if (res != -1)
                {
                    byte[] buf = new byte[res];
                    Marshal.Copy(_ReadingBuffer, buf, 0, res);

                    OnDataReceived(buf);
                }

                Thread.Sleep(50);
            }
        }

        protected virtual void OnDataReceived(byte[] data)
        {
            DataReceived?.Invoke(this, data);
        }

        /// <summary>
        /// Close the port
        /// </summary>
        public void Close()
        {
            if (!_FileDescriptor.HasValue)
            {
                throw new Exception();
            }

            cts.Cancel();
            Libc.close(_FileDescriptor.Value);
            Marshal.FreeHGlobal(_ReadingBuffer);
            _FileDescriptor = null;
        }

        /// <summary>
        /// Write a buffer of bytes
        /// </summary>
        /// <param name="buf"></param>
        public void Write(byte[] buf)
        {
            if (!_FileDescriptor.HasValue)
            {
                throw new Exception();
            }

            IntPtr ptr = Marshal.AllocHGlobal(buf.Length);
            Marshal.Copy(buf, 0, ptr, buf.Length);
            Libc.write(_FileDescriptor.Value, ptr, buf.Length);
            Marshal.FreeHGlobal(ptr);
        }

        /// <summary>
        /// Get all possible serial ports
        /// </summary>
        /// <returns></returns>
        public static string[] GetPortNames()
        {
            List<string> serial_ports = new List<string>();

            // Are we on Unix?
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                string[] ttys = System.IO.Directory.GetFiles("/dev/", "tty*");
                foreach (string dev in ttys)
                {
                    //Arduino MEGAs show up as ttyACM due to their different USB<->RS232 chips
                    if (dev.StartsWith("/dev/ttyS")
                        || dev.StartsWith("/dev/ttyUSB")
                        || dev.StartsWith("/dev/ttyACM")
                        || dev.StartsWith("/dev/ttyAMA")
                        || dev.StartsWith("/dev/ttyPS")
                        || dev.StartsWith("/dev/serial"))
                    {
                        serial_ports.Add(dev);
                        //Console.WriteLine("Serial list: {0}", dev);
                    }
                }
                //newer Pi with bluetooth map serial
                ttys = System.IO.Directory.GetFiles("/dev/", "serial*");
                foreach (string dev in ttys)
                {
                    serial_ports.Add(dev);
                }
            }
            else if(RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                var ttys = System.IO.Directory.GetFiles("/dev/", "tty.*");
                foreach (string dev in ttys)
                {
                    serial_ports.Add(dev);
                }
            }

            return serial_ports.ToArray();
        }

        /// <summary>
        /// Dispose the port
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            if (IsOpen)
            {
                Close();
            }
        }
    }
}