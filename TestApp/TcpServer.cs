using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Diagnostics;
using System.Net;

namespace TestApp
{
    class TcpServer : IDisposable
    {
        TcpListener listener;
        public event EventHandler<ConnectedEventArgs> Connect;
        public event EventHandler Disconnect;
        public event EventHandler<DataReceivedEventArgs> DataReceived;
        
        public TcpServer(int port)
        {
            listener = new TcpListener(IPAddress.Loopback, port);
            listener.Start();
            ThreadPool.QueueUserWorkItem(Listen);
        }

        private void Listen(object state)
        {
            try
            {
                while (true)
                {
                    using (TcpClient client = listener.AcceptTcpClient())
                    {
                        AcceptClient(client);
                    }
                }
            }
            catch (SocketException)
            {
                // most likely caused by dispose, exit the thread
            }
        }

        private void AcceptClient(TcpClient client)
        {
            using (NetworkStream inStream = client.GetStream())
            {
                OnConnect(inStream);
                while (client.Connected)
                {
                    int available = client.Available;
                    if (available > 0)
                    {
                        byte[] buffer = new byte[available];
                        int read = inStream.Read(buffer, 0, available);
                        Debug.Assert(read == available);
                        OnDataReceived(buffer);
                    }
                    else
                    {
                        Thread.Sleep(50);
                    }
                }
            }
            OnDisconnect();
        }

        private void OnConnect(NetworkStream stream)
        {
            var connect = Connect;
            if (connect != null)
            {
                connect(this, new ConnectedEventArgs() { Stream = stream });
            }
        }

        private void OnDisconnect()
        {
            var disconnect = Disconnect;
            if (disconnect != null)
            {
                disconnect(this, EventArgs.Empty);
            }
        }

        private void OnDataReceived(byte[] buffer)
        {
            var execute = DataReceived;
            if (execute != null)
            {
                execute(this, new DataReceivedEventArgs() { Buffer = buffer });
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            listener.Stop();
        }

        #endregion
    }

    public class DataReceivedEventArgs : EventArgs
    {
        public byte[] Buffer { get; set; }
    }

    public class ConnectedEventArgs : EventArgs
    {
        public NetworkStream Stream { get; set; }
    }
}
