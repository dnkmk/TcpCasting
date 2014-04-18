using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace SimpleTcpClient
{
    class Program
    {

        static TcpClient client = new TcpClient();
        static void Main(string[] args)
        {
            
            BeginConnectedToServer();

            while (true)
            {
                string message = Console.ReadLine();
                try
                {
                    client.Client.Send(Encoding.UTF8.GetBytes(message));
                }
                catch (Exception ex)
                {
                }
            }

        }

        static void BeginConnectedToServer()
        {
            Console.WriteLine("start to connect....");
            client.BeginConnect(IPAddress.Parse("127.0.0.1"), 9000, EndConnectToServer, client);
        }

        static void EndConnectToServer(IAsyncResult iar)
        {

            TcpClient client = iar.AsyncState as TcpClient;
            try
            {
                client.EndConnect(iar);
                Console.WriteLine("connected to server.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("fail to connect to server. Retry in 5 seconds.");
                Thread.Sleep(5000);
                BeginConnectedToServer();
            }

            Thread thread = new Thread(ListenForMessage);
            thread.IsBackground = true;
            thread.Start(client);
        }

        private static void ListenForMessage(object obj)
        {
            TcpClient client = obj as TcpClient;
            Socket socket = client.Client;
            while (true)
            {
                byte[] buffer = new byte[client.ReceiveBufferSize];
                int count = socket.Receive(buffer);
                string line = "server: " + Encoding.UTF8.GetString(buffer, 0, count);
                Console.WriteLine(line);
            }
        }
    }
}
