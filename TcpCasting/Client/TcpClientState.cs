using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Client
{
    public class TcpClientState
    {
        private System.Net.Sockets.TcpClient tcpClient;
        private byte[] buffer;

        public TcpClientState(System.Net.Sockets.TcpClient tcpClient, byte[] buffer)
        {
            // TODO: Complete member initialization
            this.tcpClient = tcpClient;
            this.buffer = buffer;
        }

        public System.Net.Sockets.TcpClient TcpClient { get; set; }

        public System.Net.Sockets.NetworkStream NetworkStream { get; set; }

        public byte[] Buffer { get; set; }
    }
}
