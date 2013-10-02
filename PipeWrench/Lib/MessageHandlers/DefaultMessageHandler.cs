using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using PipeWrench.Lib.Domain;
using PipeWrench.Lib.ServiceBindings;
using PipeWrench.Lib.Tunnels;
using System.Net.Sockets;
using SockLibNG.Sockets;
using log4net;
using Buffer = SockLibNG.Buffers.Buffer;

namespace PipeWrench.Lib.MessageHandlers
{
    public class DefaultMessageHandler : IMessageHandler
    {
        private static DefaultMessageHandler _singletonMessageHandlerRef;

        private readonly Socket _recvSocket;
        private readonly Buffer _recvBuffer;
        private readonly Thread _recvThread;

        private static readonly ILog Logger = LogManager.GetLogger(typeof(DefaultMessageHandler));

        public event RecvThreadDeathNotification RecvThreadDeathNotification;
        public event MessageRecievedFromTunnel MessageRecievedFromTunnel;

        public static DefaultMessageHandler GetExistingOrNew()
        {
            return _singletonMessageHandlerRef ?? new DefaultMessageHandler();
        }

        private DefaultMessageHandler()
        {
            _recvSocket = SockLib.UdpConnect(14804);
            _recvBuffer = Buffer.New();
            _recvThread = new Thread(MessageReceiveThread).Run();
            _singletonMessageHandlerRef = this;
        }

        public void ReceiveFromTunnel(byte[] data)
        {
            throw new NotImplementedException();
        }

        public void ReceiveFromServiceBinding(IServiceBinding sender, KeyValuePair<string, int> remoteBinding, byte[] data)
        {
            var tunnelToUse = TunnelManager.GetTunnelByRemoteBinding(remoteBinding);
            if (tunnelToUse == null)
                sender.ServiceDispatchFail(1, string.Format("An active tunnel does not exist for remote client {0}:{1}", remoteBinding.Key, remoteBinding.Value));
            else
                tunnelToUse.EnqueueMessage(new Message(data));
        }

        public void DispatchToServiceBinding()
        {
            throw new NotImplementedException();
        }

        public void DispatchToTunnel()
        {
            throw new NotImplementedException();
        }

        public void RecieveThreadDeathFromTunnel(Tunnel tunnel)
        {
            throw new NotImplementedException();
        }

        public void MessageReceiveThread()
        {
            while (SockLib.BytesAvailable(_recvSocket) != 0)
            {
                //TODO: Recieve message and route to appropriate handler
                if (SockLib.ReceiveMessage(_recvSocket, _recvBuffer) > 0)
                {
                    var remoteIp = SockLib.GetRemoteIpAddress(_recvSocket);
                    var remotePort = SockLib.GetRemotePort(_recvSocket);
                }

            }
            RecvThreadDeathNotification();
        }
    }
}
