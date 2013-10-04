using System.Collections.Generic;
using PipeWrench.Lib.Tunnels;

namespace PipeWrench.Lib
{
    public delegate void ServiceBindingMessageNotification(byte[] data);
    
    //The following delegates are for clients
    public delegate void MessageReceived(KeyValuePair<string, int> remoteBinding, byte[] data);
    public delegate void MessageSendFailure(int errorCode, string additionalInformation);
    public delegate void TunnelCreationFailure(int errorCode, string additionalInformation);
    public delegate void TunnelCreationSuccess();

    public interface IServiceBinding
    {
        IMessageHandler MessageHandler { get; set; }

        void SendMessage(KeyValuePair<string, int> remoteBinding, byte[] dataToSend);

        void CreateTunnel(string friendlyName, KeyValuePair<string, int> remoteBinding);

        void MessageRecieved(KeyValuePair<string, int> remoteBinding, byte[] data);

        void ServiceDispatchFail(int errorCode, string additionalInformation);

        void TunnelCreationFail(int errorCode, string additionalInformation);

        void TunnelCreationSucceed(Tunnel tunnel);
    }
}

