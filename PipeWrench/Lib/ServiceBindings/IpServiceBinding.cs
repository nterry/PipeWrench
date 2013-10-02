using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using PipeWrench.Lib.Tunnels;
using SockLibNG.Sockets;
using Buffer = SockLibNG.Buffers.Buffer;

namespace PipeWrench.Lib.ServiceBindings
{
    class IpServiceBinding : IServiceBinding
    {
        public IMessageHandler MessageHandler { get; set; }

        public event ServiceDispatch ServiceDispatch;

        private readonly ConcurrentDictionary<Tunnel, Socket> _tunnelClientBindMap;
        private readonly Buffer _sendBuffer = Buffer.New();
        private readonly Buffer _recvBuffer = Buffer.New();

        public IpServiceBinding(IMessageHandler messageHandler)
        {
            MessageHandler = messageHandler;
            ServiceDispatch += MessageHandler.ReceiveFromServiceBinding;
            MessageHandler.MessageRecievedFromTunnel += MessageRecieved;
            _tunnelClientBindMap = new ConcurrentDictionary<Tunnel, Socket>();
        }

        public void SendMessage(Tunnel tunnel, byte[] dataToSend)
        {
            throw new NotImplementedException();
        }

        //TODO: Need to add method to handle new client connect including adding socket and method to create tunnel and "bind" socket to it in the bindmap

        //TODO: Need to add recieve method for client(s) and handle sending and cleanup of closed socket(s) and/or tunnels in bindmap

        public void MessageRecieved(KeyValuePair<string, int> remoteBinding, byte[] data)
        {
            foreach (var binding in _tunnelClientBindMap.Select((x => x)).Where(x => x.Key.RemoteBinding.Equals(remoteBinding)))
            {
                PrepareBuffer(_sendBuffer, data);
                SockLib.SendMessage(binding.Value, _sendBuffer);
            }
        }

        private static void PrepareBuffer(Buffer buffer, byte[] data)
        {
            Buffer.ClearBuffer(buffer);
            Buffer.Add(buffer, data);
            Buffer.FinalizeBuffer(buffer);
        }
    }
}
