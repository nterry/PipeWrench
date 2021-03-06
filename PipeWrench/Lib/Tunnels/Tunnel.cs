﻿using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using PipeWrench.Lib.Domain;
using PipeWrench.Lib.Util;
using SockLibNG.Sockets;
using log4net;
using Buffer = SockLibNG.Buffers.Buffer;

namespace PipeWrench.Lib.Tunnels
{
    public delegate void ThreadDeathNotification(Tunnel tunnel);

    public delegate void TunnelMessageNotification(byte[] data);

    public class Tunnel
    {
        public Thread SendThread { get; private set; }
        public string FriendlyName { get; private set; }
        public KeyValuePair<string, int> RemoteBinding { get; set; } 
        
        private IMessageHandler MessageHandler { get; set; }

        private readonly Buffer _sendBuffer = Buffer.New();
        private readonly Socket _socket;
        private readonly ConcurrentQueue<Message> _messageQueue;
        private readonly SimpleMutex _hbMutex;
        private readonly int _id; 

        private const int HeartbeatTimeout = 60000;
        
        private static readonly ILog Logger = LogManager.GetLogger(typeof(Tunnel));
        private static int _tunnelCounter;
        

        public Tunnel(IMessageHandler messageHandler, string friendlyName="Cactus Fantastico", int port=14804)
        {
            MessageHandler = messageHandler;

            FriendlyName = friendlyName;
            _socket = SockLib.UdpConnect(port);
            _messageQueue = new ConcurrentQueue<Message>();
            _hbMutex = new SimpleMutex();
            _id = _tunnelCounter++;
        }

        public Tunnel Run(string remoteIp, int remotePort)
        {
            RemoteBinding = new KeyValuePair<string, int>(remoteIp, remotePort);
            SendThread = new Thread(() => TunnelSendThread(remoteIp, remotePort)).Run();
            return this;
        }

        public void EnqueueMessage(Message message)
        {
            _messageQueue.Enqueue(message);
        }

        public int GetId()
        {
            return _id;
        }

        private void TunnelSendThread(string remoteIp, int remotePort)
        {
            while (true)
            {
                var message = DequeueMessage();
                if (message == null)
                {
                    new Thread(() => SendHeartbeat(remoteIp, remotePort)).Run();
                    continue;
                }
                    
                Buffer.ClearBuffer(_sendBuffer);
                Buffer.Add(_sendBuffer, message);
                Buffer.FinalizeBuffer(_sendBuffer);
                SockLib.SendMessage(_socket, remoteIp, remotePort, _sendBuffer);
            }
        }

        private byte[] DequeueMessage()
        {
            Message message;
            _messageQueue.TryDequeue(out message);
            if (message != null)
            {
                Logger.Info(string.Format("TunnelId={0} TunnelName={1} Message=\"Message available to send. Preparing and sending message with id {2}\"", GetId(), FriendlyName, message.Id));
                return message.Data;
            }
            return null;
        }

        private void SendHeartbeat(string remoteIp, int remotePort)
        {
            lock(_hbMutex)
            {
                if (_hbMutex.IsHeld())
                {
                    //Logger.Info(string.Format("TunnelId={0} TunnelName={1} Message=\"Heartbeat has already been sent to {2}:{3} in the last {4} seconds. Aborting.\"", GetId(), FriendlyName, remoteIp, remotePort, HeartbeatTimeout / 1000));
                    return;
                }
                Logger.Info(string.Format("TunnelId={0} TunnelName={1} Message=\"Locking available mutex to send heartbeat to remote client {2}:{3}\"", GetId(), FriendlyName, remoteIp, remotePort));
                _hbMutex.Hold();
            }   

            var tmpBuffer = Buffer.New();
            Buffer.ClearBuffer(tmpBuffer);
            Buffer.FinalizeBuffer(tmpBuffer);
            SockLib.SendMessage(_socket, remoteIp, remotePort, tmpBuffer);
            Thread.Sleep(HeartbeatTimeout);
            
            lock (_hbMutex)
            {
                Logger.Info(string.Format("TunnelId={0} TunnelName={1} Message=\"Releasing mutex for heartbeat\"", GetId(), FriendlyName));
                _hbMutex.Release();
            }
        }
    }
}
