using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace erpc
{
    public class TCPTransport: FramedTransport
    {
        private string _host;
        private int _port;
        private bool _isServer;
        TcpListener _server = null;
        TcpClient _client = null;
        public TCPTransport(string host, int port, bool isServer)
        {
            this._host = host;
            this._port = port;
            this._isServer = isServer;
            if (this._isServer)
            {
                Thread newThread = new Thread(this._serve);
                newThread.Start();
            }
            else
            {
                this._client = new TcpClient(this._host, this._port);
            }
        }

        public void _serve()
        {
            try
            {
                IPAddress localAddr = IPAddress.Parse("127.0.0.1");
                // TcpListener server = new TcpListener(port);
                _server = new TcpListener(localAddr, this._port);
                // Start listening for client requests.
                _server.Start();

                while (true)
                {
                    Console.Write("Waiting for a connection... ");

                    // Perform a blocking call to accept requests.
                    // You could also user server.AcceptSocket() here.
                    this._client = _server.AcceptTcpClient();
                    Console.WriteLine("Connected!");
                }
            }
            catch(SocketException e)
            {
              Console.WriteLine("SocketException: {0}", e);
            }
            finally
            {
               // Stop listening for new clients.
               _server.Stop();
            }
        }

        public virtual void close()
        {
            if(this._client != null)
            {
                this._client.Close();
                this._client = null;
            }

            if (this._server != null)
            {
                this._server.Stop();
                this._server = null;
            }
        }

        public override erpc_status _base_send(byte[] data, int length)
        {
            if(length > data.Length)
            {
                return erpc_status.kErpcStatus_BufferOverrun;
            }

            if (this._isServer)
            {
                while (this._server == null){ }
            }
            while (this._client == null) { }
            NetworkStream stream = this._client.GetStream();
            stream.Write(data, 0, length);

            return erpc_status.kErpcStatus_Success;
        }

        public override erpc_status _base_receive(byte[] data, int length)
        {
            if (length > data.Length)
            {
                return erpc_status.kErpcStatus_BufferOverrun;
            }

            if (this._isServer)
            {
                while (this._server == null) { }
            }

            while (this._client == null) { }

            NetworkStream stream = this._client.GetStream();
            int numBytesToRead = length;
            int numBytesRead = 0;
            do
            {
                int n = stream.Read(data, numBytesRead, numBytesToRead);
                numBytesRead += n;
                numBytesToRead -= n;
            } while (numBytesToRead > 0);

           return erpc_status.kErpcStatus_Success;
        }
    }

}
