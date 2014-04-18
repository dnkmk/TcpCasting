using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace Client
{
    public class TcpClientConnectedEventArgs
    {
        private System.Net.Sockets.TcpClient tcpClient;

        public TcpClientConnectedEventArgs(TcpClient tcpClient)
        {
            // TODO: Complete member initialization
            this.tcpClient = tcpClient;
        }
    }
}
