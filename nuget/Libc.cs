using System;
using System.Runtime.InteropServices;

namespace System.IO.Ports
{
    public static class Libc
    {
        [Flags]
        public enum OpenFlags
        {
            O_RDONLY = 0,
            O_WRONLY = 1,
            O_RDWR = 2,

            O_NONBLOCK = 4,
        }

        [DllImport("libc")]
        public static extern int getpid();

        [DllImport("libc")]
        public static extern int tcgetattr(int fd, [Out] byte[] termios_data);

        [DllImport("libc")]
        public static extern int open(string pathname, OpenFlags flags);

        [DllImport("libc")]
        public static extern int close(int fd);

        [DllImport("libc")]
        public static extern int read(int fd, IntPtr buf, int count);

        [DllImport("libc")]
        public static extern int write(int fd, IntPtr buf, int count);

        [DllImport("libc")]
        public static extern int tcsetattr(int fd, int optional_actions, byte[] termios_data);

        [DllImport("libc")]
        public static extern int cfsetspeed(byte[] termios_data, BaudRate speed);
    }
}