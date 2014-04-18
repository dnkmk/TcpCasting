using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Client
{
    public class TcpClientDisconnectedEventArgs : EventArgs
    {
        private System.Net.Sockets.TcpClient tcpClient;

        public TcpClientDisconnectedEventArgs(System.Net.Sockets.TcpClient tcpClient)
        {
            // TODO: Complete member initialization
            this.tcpClient = tcpClient;
        }
    }
}
