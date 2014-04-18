using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using System.Net.Sockets;
using System.Text;
using System.IO;

namespace WorkerRole
{
    public class WorkerRole : RoleEntryPoint
    {
        #region old code

        //static Dictionary<string, Socket> clientSockets = new Dictionary<string, Socket>();
        //static Dictionary<string, Socket> serverSockets = new Dictionary<string, Socket>();
        //static List<TcpClient> tcpClients = new List<TcpClient>();
        //static string roleName = "WorkerRole";

        //static string internalEndpointKey = "InternalEndpoint";

        #endregion

        //This variable determines the number of 
        //SocketAsyncEventArg objects put in the pool of objects for receive/send.
        //The value of this variable also affects the Semaphore.
        //This app uses a Semaphore to ensure that the max # of connections
        //value does not get exceeded.
        //Max # of connections to a socket can be limited by the Windows Operating System
        //also.
        public const Int32 maxNumberOfConnections = 3000;

        //You would want a buffer size larger than 25 probably, unless you know the
        //data will almost always be less than 25. It is just 25 in our test app.
        public const Int32 testBufferSize = 25;

        //This is the maximum number of asynchronous accept operations that can be 
        //posted simultaneously. This determines the size of the pool of 
        //SocketAsyncEventArgs objects that do accept operations. Note that this
        //is NOT the same as the maximum # of connections.
        public const Int32 maxSimultaneousAcceptOps = 10;

        //The size of the queue of incoming connections for the listen socket.
        public const Int32 backlog = 100;

        //For the BufferManager
        public const Int32 opsToPreAlloc = 2;    // 1 for receive, 1 for send

        //allows excess SAEA objects in pool.
        public const Int32 excessSaeaObjectsInPool = 1;

        //This number must be the same as the value on the client.
        //Tells what size the message prefix will be. Don't change this unless
        //you change the code, because 4 is the length of 32 bit integer, which
        //is what we are using as prefix.
        public const Int32 receivePrefixLength = 4;
        public const Int32 sendPrefixLength = 4;

        public static Int32 mainTransMissionId = 10000;
        public static Int32 mainSessionId = 1000000000;
        public override void Run()
        {

            // Get endpoint for the listener.
            //var endpoint = RoleEnvironment.CurrentRoleInstance.InstanceEndpoints["InternalEndpoint"].IPEndpoint;


            ////This object holds a lot of settings that we pass from Main method
            ////to the SocketListener. In a real app, you might want to read
            ////these settings from a database or windows registry settings that
            ////you would create.
            //SocketListenerSettings theSocketListenerSettings = new SocketListenerSettings(
            //    maxNumberOfConnections,
            //    excessSaeaObjectsInPool,
            //    backlog,
            //    maxSimultaneousAcceptOps,
            //    receivePrefixLength,
            //    testBufferSize,
            //    sendPrefixLength,
            //    opsToPreAlloc,
            //    endpoint
            //    );

            ////instantiate the SocketListener.
            //SocketListener socketListener = new SocketListener(theSocketListenerSettings);

            //while (true)
            //{
            //    Thread.Sleep(10);
            //}

            var endpoint = RoleEnvironment.CurrentRoleInstance.InstanceEndpoints["InputEndpoint"].IPEndpoint;
            TcpListener listener = new TcpListener(endpoint);
            listener.Start();

            while (true)
            {
                var client = listener.AcceptTcpClient();
                while (true)
                {
                    client.Client.Send(Encoding.UTF8.GetBytes("hello"));
                    Thread.Sleep(2000);
                }
            }

        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections 
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

            return base.OnStart();
        }

        #region old code

        //static void StartServerListener()
        //{
        //    Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //    socket.Bind(RoleEnvironment.CurrentRoleInstance.InstanceEndpoints["InternalEndpoint"].IPEndpoint);
        //    socket.Listen(10);

        //    Thread thread = new Thread(WaitForServerConnect);
        //    thread.IsBackground = true;
        //    thread.Start(socket);
        //}

        //static void WaitForServerConnect(object o)
        //{
        //    Socket socket = o as Socket;

        //    while (true)
        //    {
        //        socket.BeginAccept(ServerAcceptComplete, socket);
        //    }
        //}

        //static void ServerAcceptComplete(IAsyncResult iar)
        //{
        //    Socket socket = iar.AsyncState as Socket;
        //    Socket tSocket = socket.EndAccept(iar);
        //    string targetEndpoint = tSocket.RemoteEndPoint.ToString();
        //    if (!serverSockets.Keys.Contains(targetEndpoint))
        //    {
        //        Thread thread = new Thread(WaitForServerMessage);
        //        thread.IsBackground = true;
        //        thread.Start(tSocket);
        //        serverSockets.Add(targetEndpoint, tSocket);
        //    }
        //}

        //static void WaitForServerMessage(object o)
        //{
        //    Socket client = o as Socket;
        //    while (true)
        //    {
        //        try
        //        {
        //            byte[] buffer = new byte[1024 * 1024];
        //            int n = client.Receive(buffer);
        //            string message = Encoding.UTF8.GetString(buffer, 0, n);
        //            try
        //            {
        //                byte[] buffer2 = Encoding.UTF8.GetBytes(message);
        //                foreach (var item in clientSockets)
        //                {
        //                    item.Value.Send(buffer2);
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                ShowMsg("发错信息到实例出错：" + ex.Message);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            ShowMsg(ex.Message);
        //            break;
        //        }
        //    }
        //}

        //static void SendMessageToInstances(string msg)
        //{
        //    byte[] encodeString = Encoding.UTF8.GetBytes(msg);
        //    foreach (var instance in RoleEnvironment.Roles[roleName].Instances)
        //    {
        //        var endpoint = instance.InstanceEndpoints[internalEndpointKey].IPEndpoint;
        //        if (endpoint.ToString() != RoleEnvironment.CurrentRoleInstance.InstanceEndpoints[internalEndpointKey].IPEndpoint.ToString())
        //        {
        //            if (!serverSockets.Keys.Contains(endpoint.ToString()))
        //            {
        //                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //                socket.Connect(endpoint);
        //                serverSockets.Add(endpoint.ToString(), socket);

        //                Thread thread = new Thread(WaitForServerMessage);
        //                thread.IsBackground = true;
        //                thread.Start(socket);
        //            }
        //            serverSockets[endpoint.ToString()].Send(encodeString);
        //        }
        //    }
        //}

        //private static void StartClientListener()
        //{
        //    Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //    var ipEnpoint = RoleEnvironment.CurrentRoleInstance.InstanceEndpoints["InputEndpoint"].IPEndpoint;
        //    socket.Bind(ipEnpoint);
        //    socket.Listen(10);

        //    Thread thread = new Thread(WaitForClientConnect);
        //    thread.IsBackground = true;
        //    thread.Start(socket);
        //}

        //// private Socket client;
        //static void WaitForClientConnect(object o)
        //{
        //    Socket socket = o as Socket;
        //    while (true)
        //    {
        //        socket.BeginAccept(ClientAcceptComplete, socket);
        //    }
        //}

        //static void ClientAcceptComplete(IAsyncResult iar)
        //{
        //    Socket socket = iar.AsyncState as Socket;
        //    //通信用socket
        //    try
        //    {
        //        Socket tSocket = socket.EndAccept(iar);
        //        string point = tSocket.RemoteEndPoint.ToString();
        //        ShowMsg(point + "连接成功！");
        //        clientSockets.Add(point, tSocket);
        //        Thread th = new Thread(ReceiveClientMessage);
        //        th.IsBackground = true;
        //        th.Start(tSocket);
        //    }
        //    catch (Exception ex)
        //    {
        //        ShowMsg(ex.Message);
        //    }
        //}

        //static void ReceiveClientMessage(object o)
        //{
        //    Socket client = o as Socket;
        //    while (true)
        //    {
        //        try
        //        {
        //            byte[] buffer = new byte[1024 * 1024];
        //            int n = client.Receive(buffer);
        //            string words = Encoding.UTF8.GetString(buffer, 0, n);
        //            var message = "接收到：" + client.RemoteEndPoint.ToString() + "的消息:" + words;
        //            ShowMsg(message);
        //            try
        //            {
        //                byte[] buffer2 = Encoding.UTF8.GetBytes("来自服务器的信息：" + message);
        //                foreach (var item in clientSockets)
        //                {
        //                    item.Value.Send(buffer2);
        //                }
        //                SendMessageToInstances(message);
        //            }
        //            catch (Exception ex)
        //            {
        //                ShowMsg("发错信息到客户端出错：" + ex.Message);
        //            }
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
        //    Trace.Write(msg);
        //}

        #endregion

    }
}
