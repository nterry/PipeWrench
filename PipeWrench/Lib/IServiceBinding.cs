using System.Collections.Generic;
using PipeWrench.Lib.Tunnels;

namespace PipeWrench.Lib
{
    public delegate void ServiceBindingMessageNotification(byte[] data);
    public delegate void MessageRecievedFromTunnel(KeyValuePair<string, int> remoteBinding, byte[] data);

    public interface IServiceBinding
    {
        IMessageHandler MessageHandler { get; set; }
        event ServiceDispatch ServiceDispatch;

        void SendMessage(Tunnel tunnel, byte[] dataToSend);

        void MessageRecieved(KeyValuePair<string, int> remoteBinding, byte[] data);
    }
}

