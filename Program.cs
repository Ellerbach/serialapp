using System;

namespace serialapp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello Serial port!");
            SerialDevice mySer = new SerialDevice("/dev/ttyS0", BaudRate.B1152000);
            mySer.DataReceived += MySer_DataReceived;
            while (!Console.KeyAvailable)
                ;
        }

        private static void MySer_DataReceived(object arg1, byte[] arg2)
        {
            Console.WriteLine($"Received: {System.Text.Encoding.UTF8.GetString(arg2)}");
        }
    }
}
