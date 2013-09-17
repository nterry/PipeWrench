using System;
using PipeWrench.Lib.Tunnels;

namespace PipeWrench.Lib.MessageHandlers
{
    public class DefaultMessageHandler : IMessageHandler
    {
        public event TunnelMessageNotification MessageReceivedNotification;
        public event ServiceBindingMessageNotification ServiceBindingReceiveMessage;

        public DefaultMessageHandler()
        {
            MessageReceivedNotification += ReceiveFromTunnel;
            ServiceBindingReceiveMessage += ReceiveFromServiceBinding;
        }

        public void ReceiveFromTunnel(byte[] data)
        {
            throw new NotImplementedException();
        }

        public void ReceiveFromServiceBinding(byte[] data)
        {
            throw new NotImplementedException();
        }

        public void DispatchToServiceBinding()
        {
            throw new NotImplementedException();
        }

        public void DispatchToTunnel()
        {
            throw new NotImplementedException();
        }
    }
}
