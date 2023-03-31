using System.Data;
using System.Net;
using System.Net.Sockets;
using System.Text;

try
{
    ChatServer chatserver = new ChatServer();
    chatserver.Start();
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}

class ChatServer
{
    const short port = 4040;
    const string address = "127.0.0.1";
    TcpListener? listener = null;
    public ChatServer()
    {
        listener = new TcpListener(IPAddress.Parse(address), port);
    }
    public void Start()
    {
        listener.Start();
        Console.WriteLine("Waiting for connection...");
        TcpClient client = listener.AcceptTcpClient();
        Console.WriteLine("Connected");
        NetworkStream stream = client.GetStream();
        StreamReader stREAD = new StreamReader(stream);
        StreamWriter stWRITE = new StreamWriter(stream);
        while (true)
        {
            string message = stREAD.ReadLine();
            if (message == "exit")
            {
                listener.Stop();
                break;
            }
            Console.WriteLine($"{message} at {DateTime.Now.ToShortTimeString()} from {client.Client.LocalEndPoint}");
            stWRITE.WriteLine(message);
            stWRITE.Flush();
        }
    }
}

