using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        //static Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        static byte[] buffer = new byte[1024 * 1024];
        static void Main(string[] args)
        {
            // Get endpoint for the listener.
            var endpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9000);
            TcpClient client = new TcpClient();
            client.Connect(endpoint);

            NetworkStream networkStream = client.GetStream();
            networkStream.BeginRead(buffer,
                  0, 1024 * 1024,
                  HandleDatagramReceived,
                  client);

            while (true)
            {
                var message = Console.ReadLine();
                if (!string.IsNullOrEmpty(message))
                {
                    client.Client.Send(Encoding.UTF8.GetBytes(client.Client.LocalEndPoint.ToString() + ":" + message));

                }
                Thread.Sleep(1000);
            }

        }

        private static void HandleDatagramReceived(IAsyncResult ar)
        {
            TcpClient client = (TcpClient)ar.AsyncState;
            NetworkStream networkStream = client.GetStream();

            int numberOfReadBytes = 0;
            try
            {
                numberOfReadBytes = networkStream.EndRead(ar);
            }
            catch
            {
                numberOfReadBytes = 0;
            }

            // received byte and trigger event notification
            byte[] receivedBytes = new byte[numberOfReadBytes];
            buffer = new byte[1024 * 1024];
            Buffer.BlockCopy(buffer, 0, receivedBytes, 0, numberOfReadBytes);

            Console.WriteLine(Encoding.UTF8.GetString(receivedBytes));

            // continue listening for tcp datagram packets
            networkStream.BeginRead(buffer, 0, buffer.Length, HandleDatagramReceived, client);
        }


        //接收服务器的消息
        //static void ReceiveMsg()
        //{
        //    while (true)
        //    {
        //        try
        //        {
        //            byte[] buffer = new byte[1024 * 1024];
        //            int n = client.Receive(buffer);
        //            string s = Encoding.UTF8.GetString(buffer, 0, n);
        //            ShowMsg(client.RemoteEndPoint.ToString() + ":" + s);
        //        }
        //        catch (Exception ex)
        //        {
        //            ShowMsg(ex.Message);
        //            break;
        //        }
        //    }

        //}

        //static void ShowMsg(string msg)
        //{
        //    Console.WriteLine(msg);
        //}
    }
}
