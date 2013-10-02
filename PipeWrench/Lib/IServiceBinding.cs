using System.Collections.Generic;
using PipeWrench.Lib.Tunnels;

namespace PipeWrench.Lib
{
    public delegate void ServiceBindingMessageNotification(byte[] data);
    public delegate void MessageRecievedFromTunnel(KeyValuePair<string, int> remoteBinding, byte[] data);
    
    //The following delegates are for clients
    public delegate void MessageReceived(KeyValuePair<string, int> remoteBinding, byte[] data);
    public delegate void MessageSendFailure(int errorCode, string additionalInformation);

    public interface IServiceBinding
    {
        IMessageHandler MessageHandler { get; set; }

        event ServiceDispatch ServiceDispatch;

        void SendMessage(KeyValuePair<string, int> remoteBinding, byte[] dataToSend);

        void MessageRecieved(KeyValuePair<string, int> remoteBinding, byte[] data);

        void ServiceDispatchFail(int errorCode, string additionalInformation);
    }
}

