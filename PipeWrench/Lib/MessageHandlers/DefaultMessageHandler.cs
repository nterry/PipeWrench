using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using PipeWrench.Lib.Domain;
using PipeWrench.Lib.Tunnels;
using System.Net.Sockets;
using PipeWrench.Lib.Util;
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
        public event MessageReceived MessageRecieved;

        public static DefaultMessageHandler GetExistingOrNew()
        {
            return _singletonMessageHandlerRef ?? new DefaultMessageHandler();
        }

        private DefaultMessageHandler()
        {
            _recvSocket = CreateSocket();
            Logger.Debug("Successfully created local recieve socket.");
            _recvBuffer = Buffer.New();
            _recvThread = new Thread(MessageReceiveThread).Run();
            _singletonMessageHandlerRef = this;

        }

        public void ReceiveMessageFromServiceBinding(IServiceBinding sender, KeyValuePair<string, int> remoteBinding, byte[] data)
        {
            var tunnelToUse = TunnelManager.GetTunnelByRemoteBinding(remoteBinding);
            if (tunnelToUse == null)
                sender.ServiceDispatchFail(1, string.Format("An active tunnel does not exist for remote client {0}:{1}", remoteBinding.Key, remoteBinding.Value));
            else
                tunnelToUse.EnqueueMessage(new Message(data, new Random(PwUtils.SecondsSinceEpoch()).NextLong(Int64.MaxValue)));
        }

        public void ReceiveTunnelCreationRequestFromServiceBinding(IServiceBinding sender, string friendlyName , KeyValuePair<string, int> remoteBinding)
        {
            var tunnel = TunnelManager.GetTunnelByRemoteBinding(remoteBinding);
            if (tunnel == null)
            {
                try
                {
                    var t = TunnelManager.CreateTunnel(friendlyName, remoteBinding);
                    sender.TunnelCreationSucceed(t);
                }
                catch (Exception ex)
                {
                    Logger.ErrorFormat("Failed to create tunnel. Error was: {0}", ex.Message);
                    sender.TunnelCreationFail(2, ex.Message);
                }  
            }
            else
            {
                var message = string.Format("Tunnel with remote binding {0}:{1} already exists.", remoteBinding.Key, remoteBinding.Value);
                Logger.Error(message);
                sender.TunnelCreationFail(2, message);
            }
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
            var exitFlag = false;
            while (exitFlag == false)
            {
                if (SockLib.BytesAvailable(_recvSocket) > 0)
                {
                    if (SockLib.ReceiveMessage(_recvSocket, _recvBuffer) > 0)
                    {
                        //TODO: Apparently, you cannot get ip and/or port address of a udp socket... need to write values to payload...
                        //var remoteIp = SockLib.GetRemoteIpAddress(_recvSocket);
                        //var remotePort = SockLib.GetRemotePort(_recvSocket);
                        Buffer.FinalizeBuffer(_recvBuffer);
                        MessageRecieved(new KeyValuePair<string, int>("127.0.0.1", 1025), Buffer.GetBuffer(_recvBuffer));
                    }
                }
            }
            //RecvThreadDeathNotification();
        }

        private static Socket CreateSocket()
        {
            const int minPort = 1025;
            const int maxPort = 65535;
            var currentPort = minPort;
            do
            {
                try
                {
                    Logger.DebugFormat("Attempting to bind to local port {0}", currentPort);
                    return SockLib.UdpConnect(currentPort);
                }
                catch (Exception)
                {
                    Logger.DebugFormat("Failed to bind to port {0}", currentPort);
                    currentPort++;
                }   
            } while (currentPort <= maxPort);
            const string message = "Failed to bind to any local ports. Check your firewall.";
            Logger.Error(message);
            throw new IOException(message);
        }
    }
}
