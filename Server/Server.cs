// SocketServer.cs
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO.Pipes;
using System.Linq;
using System.IO;
using System.Data;
using WebRobot_ComputerFutures;

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
            if (args.Length > 0)
            {
                using (PipeStream pipeServer =
                    new AnonymousPipeClientStream(PipeDirection.Out, args[0]))
                {
                    Console.WriteLine("[CLIENT] Current TransmissionMode: {0}.", pipeServer.TransmissionMode);
                    // Устанавливаем для сокета локальную конечную точку

                    IPHostEntry ipHost = Dns.GetHostEntry("localhost");
                    IPAddress ipAddr = ipHost.AddressList[0];
                    IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, 11000);

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
                            SendToPipe(pipeServer, data);
                            Notify?.Invoke(new ServerEventArgs(($"ПОЛУЧЕНО: {data}"),data));
                            // Отправляем ответ клиенту\
                            string reply = "Спасибо за запрос в " + data.Length.ToString()
                                    + " символов";
                            byte[] msg = Encoding.UTF8.GetBytes(reply);
                            handler.Send(msg);

                            if (data.IndexOf("<TheEnd>") > -1)
                            {
                                Console.WriteLine("Сервер завершил соединение с клиентом.");
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

            void SendToPipe(PipeStream pipeStream, string message)
            {
                using (StreamWriter sw = new StreamWriter(pipeStream))
                {
                    sw.AutoFlush = true;
                    // Send a 'sync message' and wait for client to receive it.
                    pipeStream.WaitForPipeDrain();
                    // Send the console input to the client process.
                    Console.WriteLine($"[SERVER] Sended text: {message}");
                    sw.WriteLine(message);
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
            DataTable tbl = clsDB.Get_DataTable(sSQL);
            //</ find record >

            if (tbl.Rows.Count == 0)
            {
                //< add >
                string sql_Add = "INSERT INTO ServerTable1 ([URL],[Title],[dtScan]) VALUES('" + sURL + "','" + sTitle + "',SYSDATETIME())";
                clsDB.Execute_SQL(sql_Add);
                //</ add >
            }

            else
            {
                //< update >
                string ID = tbl.Rows[0]["IdDetail"].ToString();
                string sql_Update = "UPDATE ServerTable1 SET [dtScan] = SYSDATETIME() WHERE IDDetail = " + ID;
                clsDB.Execute_SQL(sql_Update);
                //</ update >
            }
            //--------</ db_Update_Add_Record() >--------
        }


    }
}