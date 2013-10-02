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

        public event ServiceDispatch ServiceDispatch;

        //The following events are for clients
        public event MessageReceived MessageReceived;
        public event MessageSendFailure MessageSendFailure;

        private readonly ConcurrentBag<Tunnel> _linkedTunnels;

        public LocalServiceBinding()
        {
            MessageHandler = DefaultMessageHandler.GetExistingOrNew();
            ServiceDispatch += MessageHandler.ReceiveFromServiceBinding;
            MessageHandler.MessageRecievedFromTunnel += MessageRecieved;
            _linkedTunnels = new ConcurrentBag<Tunnel>();
        }

        public void SendMessage(KeyValuePair<string, int> remoteBinding, byte[] dataToSend)
        {
            ServiceDispatch(this, remoteBinding, dataToSend);
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
    }
}
