using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using PipeWrench.Lib.MessageHandlers;
using PipeWrench.Lib.Tunnels;

namespace PipeWrench.Lib.ServiceBindings
{
    public class LocalServiceBinding : IServiceBinding
    {
        public IMessageHandler MessageHandler { get; set; }

        private event ServiceDispatch ServiceDispatch;
        private event TunnelCreateDispatch TunnelCreateDispatch;

        //The following events are for clients
        public event MessageReceived MessageReceived;
        public event MessageSendFailure MessageSendFailure;
        public event TunnelCreationFailure TunnelCreationFailure;
        public event TunnelCreationSuccess TunnelCreationSuccess;

        private readonly ConcurrentBag<Tunnel> _linkedTunnels;

        public LocalServiceBinding()
        {
            MessageHandler = DefaultMessageHandler.GetExistingOrNew();
            ServiceDispatch += MessageHandler.ReceiveMessageFromServiceBinding;
            TunnelCreateDispatch += MessageHandler.ReceiveTunnelCreationRequestFromServiceBinding;
            MessageHandler.MessageRecieved += MessageRecieved;
            _linkedTunnels = new ConcurrentBag<Tunnel>();
        }

        public void SendMessage(KeyValuePair<string, int> remoteBinding, byte[] dataToSend)
        {
            ServiceDispatch(this, remoteBinding, dataToSend);
        }

        public void CreateTunnel(string friendlyName, KeyValuePair<string, int> remoteBinding)
        {
            TunnelCreateDispatch(this, friendlyName, remoteBinding);
        }

        public void MessageRecieved(KeyValuePair<string, int> remoteBinding, byte[] data)
        {
            foreach (var linkedTunnel in _linkedTunnels.Where(linkedTunnel => linkedTunnel.RemoteBinding.Equals(remoteBinding)))
            {
                MessageReceived(remoteBinding, data);
            }
        }

        public void ServiceDispatchFail(int errorCode, string additionalInformation)
        {
            MessageSendFailure(errorCode, additionalInformation);
        }

        //TODO: Message Handler will call these directly in ReceiveTunnelCreationRequestFromServiceBinding method
        public void TunnelCreationFail(int errorCode, string additionalInformation)
        {
            TunnelCreationFailure(errorCode, additionalInformation);
        }

        public void TunnelCreationSucceed(Tunnel tunnel)
        {
            _linkedTunnels.Add(tunnel);
            TunnelCreationSuccess();
        }
    }
}
