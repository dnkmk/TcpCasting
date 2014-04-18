using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Threading;

namespace SimpleTcp
{
    class Program
    {

        static List<TcpClient> clients = new List<TcpClient>();
        static byte[] buffer = new byte[1024];
        static void Main(string[] args)
        {
            IPEndPoint endpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9000);
            TcpListener listener = new TcpListener(endpoint);
            listener.Start();
            Console.WriteLine("listener started");


            Thread thread = new Thread(StartListen);
            thread.IsBackground = true;
            thread.Start(listener);


            while (true)
            {
                var message = Console.ReadLine();
                SendMessageToClients(message);
            }

        }

        static void StartListen(object o)
        {
            TcpListener listener = o as TcpListener;
            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();
                clients.Add(client);
                Console.WriteLine(client.Client.RemoteEndPoint.ToString() + " is connected.");
                client.Client.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, ReceiveCallback, client);
            }
        }

        static void SendMessageToClients(string message)
        {
            foreach (var client in clients)
            {
                client.Client.Send(Encoding.UTF8.GetBytes(message));
            }
        }

        //static void AcceptTcpClientCompleted(IAsyncResult iar)
        //{
        //    TcpListener listener = iar.AsyncState as TcpListener;
        //    TcpClient client = listener.EndAcceptTcpClient(iar);
        //    clients.Add(client);
        //    Console.WriteLine(client.Client.RemoteEndPoint.ToString() + " is connected.");
        //}

        static void ReceiveCallback(IAsyncResult iar)
        {
            TcpClient client = iar.AsyncState as TcpClient;
            int count= client.Client.EndReceive(iar);
            iar.AsyncWaitHandle.Close();
            Console.WriteLine("收到消息：{0}", Encoding.ASCII.GetString(buffer,0,count));

            //清空数据，重新开始异步接收
            buffer = new byte[buffer.Length];
            client.Client.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), client);
        }

    }
}
