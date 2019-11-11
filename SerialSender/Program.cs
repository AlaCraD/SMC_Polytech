using System;
using System.IO.Ports;

namespace SerialSender
{
    class Program
    {
        
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            SerialPort port;
            // получаем список доступных портов 
            string[] ports = SerialPort.GetPortNames();

            Console.WriteLine("Выберите порт:");

            // выводим список портов
            for (int i = 0; i < ports.Length; i++)
            {
                Console.WriteLine("[" + i.ToString() + "] " + ports[i].ToString());
            }

            port = new SerialPort();

            // читаем номер из консоли
            string n = Console.ReadLine();
            int num = int.Parse(n);
            try
            {
                // настройки порта
                port.PortName = ports[num];
                port.BaudRate = 256000;
                port.DataBits = 8;
                port.Parity = System.IO.Ports.Parity.None;
                port.StopBits = System.IO.Ports.StopBits.One;
                port.ReadTimeout = 1000;
                port.WriteTimeout = 1000;
                port.Open();
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR: невозможно открыть порт:" + e.ToString());
                return;
            }
        }
    }
}
