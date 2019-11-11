using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

namespace SerialSenderr
{
    class Program
    {
        static void Main(string[] args)
        {
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
                port.BaudRate = 115200;
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
                Console.ReadKey();
                return;
            }
            byte [] masss = { 11, 00, 00, 10, 00, 00, 00, 00, 00, 21};
            while (n != "exit")
            {
                Console.WriteLine("Введите текст:");
                n = Console.ReadLine();
                //string [] str = n.Split('#');
                //Console.WriteLine(str[0]);
                //Console.WriteLine(str[1]);
                //Console.WriteLine(str[str.Length-1]);
                //byte[] asciiBytes = Encoding.ASCII.GetBytes(n);
                

                port.Write(masss, 0 , masss.Length);
                
            }
            port.Close();
        } 
    }
}
