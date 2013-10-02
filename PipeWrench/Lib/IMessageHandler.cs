using PipeWrench.Lib.Tunnels;

namespace PipeWrench.Lib
{
    public delegate void ServiceDispatch(Tunnel tunnel, byte[] data);
    public delegate void RecvThreadDeathNotification();

    public interface IMessageHandler
    {
        event RecvThreadDeathNotification RecvThreadDeathNotification;
        event MessageRecievedFromTunnel MessageRecievedFromTunnel;

        void ReceiveFromTunnel(byte[] data);

        void ReceiveFromServiceBinding(Tunnel tunnel, byte[] data);

        void DispatchToServiceBinding();

        void DispatchToTunnel();

        void RecieveThreadDeathFromTunnel(Tunnel tunnel);

        void MessageReceiveThread();
    }
}
