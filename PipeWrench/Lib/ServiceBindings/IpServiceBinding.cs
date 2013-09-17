using System;

namespace PipeWrench.Lib.ServiceBindings
{
    class IpServiceBinding : IServiceBinding
    {
        public event ServiceDispatch ServiceDispatch;

        public IpServiceBinding()
        {
            ServiceDispatch += ServiceDispatchedEvent;
        }

        public void SendMessage(byte[] dataToSend)
        {
            throw new NotImplementedException();
        }

        public void ServiceDispatchedEvent(byte[] data)
        {
            throw new NotImplementedException();
        }
    }
}
