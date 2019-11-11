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
using System.IO.Ports;
using System.Diagnostics;
using System.IO.Pipes;
using System.IO;
using System.Timers;
using System.ComponentModel;

namespace WpfApp2
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public delegate void Handler(string message);
        Handler _Handler;
        public class MyText
        {
            public string someText { get; set; }
        }
        public class People : INotifyPropertyChanged // Наследуемся от нужного интерфеса
        {
            // Ваши поля 
            private string name, name2;

            public event PropertyChangedEventHandler PropertyChanged; // Событие, которое нужно вызывать при изменении

            // Для удобства обернем событие в метод с единственным параметром - имя изменяемого свойства
            public void RaisePropertyChanged(string propertyName)
            {
                // Если кто-то на него подписан, то вызывем его
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }

            // А тут будут свойства, в которые мы обернем поля
            public string Name
            {
                get { return name; }
                set
                {
                    // Устанавливаем новое значение
                    name = value;
                    // Сообщаем всем, кто подписан на событие PropertyChanged, что поле изменилось Name
                    RaisePropertyChanged("Name");
                }
            }

            public string Name2
            {
                get { return name2; }
                set
                {
                    // Устанавливаем новое значение
                    name2 = value;
                    // Сообщаем всем, кто подписан на событие PropertyChanged, что поле изменилось Name2
                    RaisePropertyChanged("Name2");
                }
            }
        }
        public MyText myText { get; set; }
        public void RegisterHandler(Handler handler)
        {
            _Handler = handler;
        }

        public MainWindow()
        {
            
            // Получить поток, выполняющий данный метод.
            InitializeComponent();
            //_Handler = ShowText;
            myText = new MyText
            {
                someText = "blyat"
            };
            People P = new People() { Name = "Ololosha", Name2 = "Trololosha" };
            DataContext = P;
            Process server = new Process();
            server.StartInfo.Arguments = "ServerTable1";
            server.StartInfo.UseShellExecute = false;
            server.StartInfo.FileName = "Server.exe";

            
            

            //Server.Server.Notify += Button_Click();
            
            server.Start();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
                myText.someText = "sosat";
        }

        public 
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
}