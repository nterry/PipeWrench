using System.Collections.Generic;
using PipeWrench.Lib.Tunnels;

namespace PipeWrench.Lib
{
    public delegate void ServiceDispatch(IServiceBinding sender, KeyValuePair<string,int> remoteBinding, byte[] data);
    public delegate void RecvThreadDeathNotification();

    public interface IMessageHandler
    {
        event RecvThreadDeathNotification RecvThreadDeathNotification;
        event MessageRecievedFromTunnel MessageRecievedFromTunnel;

        void ReceiveFromTunnel(byte[] data);

        void ReceiveFromServiceBinding(IServiceBinding sender, KeyValuePair<string, int> remoteBinding, byte[] data);

        void DispatchToServiceBinding();

        void DispatchToTunnel();

        void RecieveThreadDeathFromTunnel(Tunnel tunnel);

        void MessageReceiveThread();
    }
}
