namespace PipeWrench.Lib
{
    public delegate void ServiceBindingMessageNotification(byte[] data);

    interface IServiceBinding
    {
        IMessageHandler MessageHandler { get; set; }
        event ServiceDispatch ServiceDispatch;

        void SendMessage(byte[] dataToSend);
    }
}
