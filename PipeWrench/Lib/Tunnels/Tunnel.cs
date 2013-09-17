using System.Net.Sockets;
using System.Threading;
using SockLibNG.Sockets;
using Buffer = SockLibNG.Buffers.Buffer;

namespace PipeWrench.Lib.Tunnels
{
    public delegate void ThreadDeathNotification();

    public delegate void TunnelMessageNotification(byte[] data);

    public class Tunnel
    {
        private readonly Buffer _recvBuffer = Buffer.New();
        private readonly Buffer _sendBuffer = Buffer.New();
        private readonly Socket _socket;

        public Tunnel(int port=14804)
        {
            _socket = SockLib.UdpConnect(port);
        }

        public Tunnel Run(string remoteIp, int remotePort)
        {
            var recvThread = new Thread(TunnelReceiveThread);
            var sendThread = new Thread(() => TunnelSendThread(remoteIp, remotePort));
            return this;
        }

        private void TunnelReceiveThread()
        {
            while (SockLib.BytesAvailable(_socket) != 0)
            {
                var bytesReceived = SockLib.ReceiveMessage(_socket, _recvBuffer);
                if (bytesReceived > 0)
                {
                    
                }
            }
        }

        private void TunnelSendThread(string remoteIp, int remotePort)
        {
            //need to loop and check for messages from message handler and send heartbeat if not.
            Buffer.ClearBuffer(_sendBuffer);
            Buffer.FinalizeBuffer(_sendBuffer);
            SockLib.SendMessage(_socket, remoteIp, remotePort, _sendBuffer);
            Thread.Sleep(60000);
        }
    }
}
