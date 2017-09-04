using System;

namespace serialapp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello Serial port!");
            var ports = SerialDevice.GetPortNames();
            bool isTTY = false;
            foreach (var prt in ports)
            {
                Console.WriteLine($"Serial name: {prt}");
                if (prt.Contains("ttyS0"))
                {
                    isTTY = true;
                }
            }
            if (!isTTY)
            {
                Console.WriteLine("No ttyS0 serial port!");
                return;
            }
            Console.WriteLine("Yes, we have the embedded serial port available, opening it");
            SerialDevice mySer = new SerialDevice("/dev/ttyS0", BaudRate.B1152000);
            mySer.DataReceived += MySer_DataReceived;
            mySer.Open();
            while (!Console.KeyAvailable)
                ;
            mySer.Close();
        }

        private static void MySer_DataReceived(object arg1, byte[] arg2)
        {
            Console.WriteLine($"Received: {System.Text.Encoding.UTF8.GetString(arg2)}");
        }
    }
}
