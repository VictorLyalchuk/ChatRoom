using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
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

namespace ChatRoom
{
    public partial class Chat : Window
    {
        ObservableCollection<MessageInfo> messages = new ObservableCollection<MessageInfo>();
        NetworkStream stream = null;
        StreamReader stREAD = null;
        StreamWriter stWRITE = null;

        IPEndPoint serverEndPoint;
        TcpClient tcpClient;
        public Chat(string name)
        {
            InitializeComponent();
            Nickname.Content = name;
            tcpClient = new TcpClient();
            string serverAddress = ConfigurationManager.AppSettings["ServerAddress"]!;
            short serverPort = short.Parse(ConfigurationManager.AppSettings["ServerPort"]!);
            serverEndPoint = new IPEndPoint(IPAddress.Parse(serverAddress), serverPort);
            DataContext = messages;
        }
        private async void Listen()
        {
            try
            {
                while (true)
                {
                    string? message = await stREAD.ReadLineAsync();
                    messages.Add(new MessageInfo(message));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void Send_Button_Click(object sender, RoutedEventArgs e)
        {
            if (MessageTextBox.Text != "")
            {
                string message = MessageTextBox.Text;
                stWRITE.WriteLine(message);
                stWRITE.Flush();
                MessageTextBox.Text = "";
            }
            else MessageBox.Show("Please, enter your messages");
        }

        private void Connection_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                tcpClient.Connect(serverEndPoint);
                stream = tcpClient.GetStream();
                stREAD = new StreamReader(stream);
                stWRITE = new StreamWriter(stream);
                Listen();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void Exit_Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Diconnection_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                stWRITE.WriteLine("exit");
                stWRITE.Flush();
                stream.Close();
                tcpClient.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
    class MessageInfo
    {
        public string Message { get; set; }
        public DateTime Time { get; set; }
        public MessageInfo(string? message)
        {
            Message = message ?? "";
            Time = DateTime.Now;
        }
        public override string ToString()
        {
            return $"{Message} : {Time}";
        }
    }
}
