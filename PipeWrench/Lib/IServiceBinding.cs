namespace PipeWrench.Lib
{
    public delegate void ServiceBindingMessageNotification(byte[] data);

    interface IServiceBinding
    {
        event ServiceDispatch ServiceDispatch;

        void SendMessage(byte[] dataToSend);
        void ServiceDispatchedEvent(byte[] data);
    }
}
