using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using PipeWrench.Lib.Domain;
using SockLibNG.Sockets;
using log4net;
using Buffer = SockLibNG.Buffers.Buffer;

namespace PipeWrench.Lib.Tunnels
{
    public delegate void ThreadDeathNotification(Tunnel tunnel);

    public delegate void TunnelMessageNotification(byte[] data);

    public class Tunnel
    {
        public event TunnelMessageNotification MessageReceivedNotification;
        public event ThreadDeathNotification ThreadDeathNotification;

        public Thread RecieveThread { get; private set; }
        public Thread SendThread { get; private set; }
        public string FriendlyName { get; private set; }
        private IMessageHandler MessageHandler { get; set; }

        private readonly Buffer _recvBuffer = Buffer.New();
        private readonly Buffer _sendBuffer = Buffer.New();
        private readonly Socket _socket;
        private readonly Queue<Message> _messageQueue;
        private readonly SimpleMutex _hbMutex;
        
        private static readonly ILog Logger = LogManager.GetLogger(typeof(Tunnel));
        

        public Tunnel(IMessageHandler messageHandler, string friendlyName="Cactus Fantastico", int port=14804)
        {
            MessageHandler = messageHandler;
            MessageReceivedNotification += MessageHandler.ReceiveFromTunnel;
            ThreadDeathNotification += MessageHandler.RecieveThreadDeathFromTunnel;

            FriendlyName = friendlyName;
            _socket = SockLib.UdpConnect(port);
            _messageQueue = new Queue<Message>();
            _hbMutex = new SimpleMutex();
        }

        public Tunnel Run(string remoteIp, int remotePort)
        {
            RecieveThread = new Thread(TunnelReceiveThread);
            SendThread = new Thread(() => TunnelSendThread(remoteIp, remotePort));
            return this;
        }

        public void EnqueueMessage(Message message)
        {
            _messageQueue.Enqueue(message);
        }

        //TODO: Dont want to BIND to a port for every tunnel to listen for responses. This needs to be extracted to another entity 
        //TODO: so all remotes communicate with one port to me and I parse the IP header to route to the appropriate tunnel or serviceBinding
        private void TunnelReceiveThread()
        {
            while (SockLib.BytesAvailable(_socket) != 0)
            {
                var bytesReceived = SockLib.ReceiveMessage(_socket, _recvBuffer);
                if (bytesReceived > 0)
                {
                    MessageReceivedNotification(Buffer.GetBuffer(_recvBuffer));
                }
            }
            ThreadDeathNotification(this);
        }

        private void TunnelSendThread(string remoteIp, int remotePort)
        {
            while (true)
            {
                var message = DequeueMessage();
                if (message == null)
                    new Thread(() => SendHeartbeat(remoteIp, remotePort));
                Buffer.ClearBuffer(_sendBuffer);
                Buffer.Add(_sendBuffer, message);
                Buffer.FinalizeBuffer(_sendBuffer);
                SockLib.SendMessage(_socket, remoteIp, remotePort, _sendBuffer);
            }
        }

        private byte[] DequeueMessage()
        {
            return IsMessageQueued() ? _messageQueue.Dequeue().Data : null;
        }

        private bool IsMessageQueued()
        {
            return _messageQueue.Count > 0;
        }

        private void SendHeartbeat(string remoteIp, int remotePort)
        {
            lock(_hbMutex)
            {
                if (_hbMutex.IsHeld())
                {
                    Logger.Info(string.Format("Heartbeat has already been sent to {0}:{1} in the last minute. Aborting.", remoteIp, remotePort));
                    return;
                }
                Logger.Info(string.Format("Locking available mutex to send heartbeat to remote client {0}:{1}", remoteIp, remotePort));
                _hbMutex.Hold();
            }

            var tmpBuffer = Buffer.New();
            Buffer.ClearBuffer(tmpBuffer);
            Buffer.FinalizeBuffer(tmpBuffer);
            SockLib.SendMessage(_socket, remoteIp, remotePort, tmpBuffer);
            Thread.Sleep(60000);
            _hbMutex.Release();
        }
    }
}
