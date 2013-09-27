using System;

namespace PipeWrench.Lib.ServiceBindings
{
    class IpServiceBinding : IServiceBinding
    {
        public IMessageHandler MessageHandler { get; set; }

        public event ServiceDispatch ServiceDispatch;

        public IpServiceBinding(IMessageHandler messageHandler)
        {
            MessageHandler = messageHandler;
            ServiceDispatch += MessageHandler.ReceiveFromServiceBinding;
        }

        public void SendMessage(byte[] dataToSend)
        {
            throw new NotImplementedException();
        }
    }
}
