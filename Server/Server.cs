// SocketServer.cs
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO.Pipes;
using System.Linq;
using System.IO;

namespace Server
{
    public class ServerEventArgs
    {
        // Сообщение
        public string Message { get; }
        // Сумма, на которую изменился счет
        public string Sum { get; }

        public ServerEventArgs(string mes, string sum)
        {
            Message = mes;
            Sum = sum;
        }
    }

    public class Server
    {
        public delegate void ServerHandler(object sender, ServerEventArgs e);
        public event ServerHandler Notify;

        public void Main(string[] args)
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
                            Notify?.Invoke(this, new ServerEventArgs(($"ПОЛУЧЕНО: {data}"),data));
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
        
        public 
    }
}