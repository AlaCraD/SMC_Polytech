// SocketServer.cs
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO.Pipes;
using System.Linq;
using System.IO.Ports;
using System.Data;
using ClassDB;

namespace Server
{
    public class ServerEventArgs
    {
        // Сообщение
        protected string Message { get; }
        // Сумма, на которую изменился счет
        protected string Sum { get; }

        public ServerEventArgs(string mes, string sum)
        {
            Message = mes;
            Sum = sum;
        }
    }

    public class Server
    {
        public delegate void ServerHandler(ServerEventArgs e);
        public static event ServerHandler Notify;

        public static void Main(string[] args)
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

            
            //if (args.Length > 0)
            {
                // Устанавливаем для сокета локальную конечную точку

                IPHostEntry ipHost = Dns.GetHostEntry("localhost");
                IPAddress ipAddr = ipHost.AddressList[0];
                IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, 11000);

                //ClsDB.Execute_SQL($"SHOW TABLE FROM dbo LIKE {args[0]}");
                // Создаем сокет Tcp/Ip
                Socket sListener = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                // Назначаем сокет локальной конечной точке и слушаем входящие сокеты
                try
                {
                    sListener.Bind(ipEndPoint);
                    sListener.Listen(10);

                    // Начинаем слушать соединения
                    while (true)
                    {

                        Console.WriteLine("Ожидаем соединение через порт {0}", ipEndPoint);

                        // Программа приостанавливается, ожидая входящее соединение
                        Socket handler = sListener.Accept();
                        string data = null;

                        // Мы дождались клиента, пытающегося с нами соединиться

                        byte[] bytes = new byte[1024];
                        int bytesRec = handler.Receive(bytes);

                        data += Encoding.UTF8.GetString(bytes, 0, bytesRec);
                        // Показываем данные на консоли
                        Console.WriteLine("Полученный текст: " + data + "\n");
                        string[] datasplit = data.Split(new char[] { ' ' });

                        Console.WriteLine($"INSERT INTO {args[0]} ({datasplit[0]}) VALUES ({datasplit[1]})");
                        ClsDB.Execute_SQL($"INSERT INTO {args[0]} ({datasplit[0]}) VALUES ({datasplit[1]})");

                        port.Write(bytes, 0, bytes.Length);

                        Notify?.Invoke(new ServerEventArgs(($"ПОЛУЧЕНО: {data}"),data));
                        // Отправляем ответ клиенту\
                        string reply = "Спасибо за запрос в " + data.Length.ToString()
                                + " символов";
                        byte[] msg = Encoding.UTF8.GetBytes(reply);
                        handler.Send(msg);

                        if (data.IndexOf("<TheEnd>") > -1)
                        {
                            Console.WriteLine("Сервер завершил соединение с клиентом.");
                            port.Close();
                            break;
                        }

                        handler.Shutdown(SocketShutdown.Both);
                        handler.Close();
                        Thread.Sleep(50);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                finally
                {
                    Console.ReadLine();
                }
            }
        }

        private void Db_Update_Add_Record(string sURL, string sTitle)
        {
            //--------< db_Update_Add_Record() >--------

            //*Update or add Record

            //< correct>
            sURL = sURL.Replace("'", "''");
            sTitle = sTitle.Replace("'", "''");
            //</ correct>

            //< find record >
            string sSQL = "SELECT TOP 1 * FROM ServerTable1 WHERE [URL] Like '" + sURL + "'";
            DataTable tbl = ClsDB.Get_DataTable(sSQL);
            //</ find record >

            if (tbl.Rows.Count == 0)
            {
                //< add >
                string sql_Add = "INSERT INTO ServerTable1 ([URL],[Title],[dtScan]) VALUES('" + sURL + "','" + sTitle + "',SYSDATETIME())";
                ClsDB.Execute_SQL(sql_Add);
                //</ add >
            }

            else
            {
                //< update >
                string ID = tbl.Rows[0]["IdDetail"].ToString();
                string sql_Update = "UPDATE ServerTable1 SET [dtScan] = SYSDATETIME() WHERE IDDetail = " + ID;
                ClsDB.Execute_SQL(sql_Update);
                //</ update >
            }
            //--------</ db_Update_Add_Record() >--------
        }


    }
}