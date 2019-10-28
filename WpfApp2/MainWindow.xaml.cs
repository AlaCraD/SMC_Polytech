using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Server;
using System.Threading;
using System.IO.MemoryMappedFiles;
using System.Diagnostics;
using System.IO.Pipes;
using System.IO;
using System.Timers;

namespace WpfApp2
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public delegate void Handler(string message);
        Handler _Handler;
        

        public void RegisterHandler(Handler handler)
        {
            _Handler = handler;
        }

        public MainWindow()
        {
            // Получить поток, выполняющий данный метод.
            InitializeComponent();
            //_Handler = ShowText;
            
            Process server = new Process();

            server.StartInfo.FileName = "Server.exe";
            using (AnonymousPipeServerStream pipeMainWindow =
                new AnonymousPipeServerStream(PipeDirection.In, HandleInheritability.Inheritable))
            {
                Console.WriteLine("[SERVER] Current TransmissionMode: {0}.",
                pipeMainWindow.TransmissionMode);

                // Pass the client process a handle to the server.
                server.StartInfo.Arguments =
                    pipeMainWindow.GetClientHandleAsString();
                server.StartInfo.UseShellExecute = false;
                server.Start();

                pipeMainWindow.DisposeLocalCopyOfClientHandle();

                ReceivePipe(pipeMainWindow);
            }
            //void ShowText(string mess, object obj)
            //{
            //    if (obj.GetType != mess)
            //        id.Text = mess;
            //    Thread.Sleep(10);
            //}
            string ReceivePipe(PipeStream pipeStream)
            {
                string temp;
                using (StreamReader sr = new StreamReader(pipeStream))
                {
                    while ((temp = sr.ReadLine()) != null)
                    {
                        temp += temp;
                        Console.WriteLine("[MainWindow] Echo: " + temp);
                    }
                }
                return temp;
            }

            

            

        }

        private void IpEndPoint_ipAddr_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}