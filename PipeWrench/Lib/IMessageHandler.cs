using System;
using PipeWrench.Lib.Tunnels;

namespace PipeWrench.Lib
{
    public delegate void ServiceDispatch(byte[] data);

    public delegate void RecvThreadDeathNotification();

    public interface IMessageHandler
    {
        event RecvThreadDeathNotification RecvThreadDeathNotification;

        void ReceiveFromTunnel(byte[] data);

        void ReceiveFromServiceBinding(byte[] data);

        void DispatchToServiceBinding();

        void DispatchToTunnel();

        void RecieveThreadDeathFromTunnel(Tunnel tunnel);

        void MessageReceiveThread();
    }
}
