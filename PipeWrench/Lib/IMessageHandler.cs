using System.Collections.Generic;
using PipeWrench.Lib.Tunnels;

namespace PipeWrench.Lib
{
    public delegate void ServiceDispatch(IServiceBinding sender, KeyValuePair<string,int> remoteBinding, byte[] data);
    public delegate void TunnelCreateDispatch(IServiceBinding sender, string friendlyName, KeyValuePair<string, int> remoteBinding);
    public delegate void RecvThreadDeathNotification();

    public interface IMessageHandler
    {
        event RecvThreadDeathNotification RecvThreadDeathNotification;
        event MessageReceived MessageRecieved;

        void ReceiveMessageFromServiceBinding(IServiceBinding sender, KeyValuePair<string, int> remoteBinding, byte[] data);

        void ReceiveTunnelCreationRequestFromServiceBinding(IServiceBinding sender, string friendlyName, KeyValuePair<string, int> remoteBinding);

        void DispatchToServiceBinding();

        void DispatchToTunnel();

        void RecieveThreadDeathFromTunnel(Tunnel tunnel);

        void MessageReceiveThread();
    }
}
