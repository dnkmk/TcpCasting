using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace Client
{
    public class TcpDatagramReceivedEventArgs<T> : EventArgs
    {
        private System.Net.Sockets.TcpClient sender;
        private string p;
        private byte[] datagram;

        public TcpDatagramReceivedEventArgs(TcpClient sender, string p)
        {
            // TODO: Complete member initialization
            this.sender = sender;
            this.p = p;
        }

        public TcpDatagramReceivedEventArgs(TcpClient sender, byte[] datagram)
        {
            // TODO: Complete member initialization
            this.sender = sender;
            this.datagram = datagram;
        }
    }
}
