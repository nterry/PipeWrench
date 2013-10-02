using System.Collections.Generic;

namespace PipeWrench.Lib.Tunnels
{
    public class TunnelManager
    {
        private static IMessageHandler _messageHandler;
        private static List<Tunnel> _tunnels;

        public TunnelManager(IMessageHandler messageHandler)
        {
            _messageHandler = messageHandler;
            _tunnels = new List<Tunnel>();
        }

        public static Tunnel CreateTunnel(string friendlyName, string remoteIp, int remotePort)
        {
            var tunnel = new Tunnel(_messageHandler, friendlyName).Run(remoteIp, remotePort);
            _tunnels.Add(tunnel);
            return tunnel;
        }

        public static Tunnel GetTunnelById(int id)
        {
            return _tunnels.Find(x => x.GetId() == id);
        }

        public static Tunnel GetTunnelByRemoteBinding(string remoteIp, int remotePort)
        {
            return GetTunnelByRemoteBinding(new KeyValuePair<string, int>(remoteIp, remotePort));
        }

        public static Tunnel GetTunnelByRemoteBinding(KeyValuePair<string, int> remoteBinding)
        {
            return _tunnels.Find(x => x.RemoteBinding.Equals(remoteBinding));
        }
    }
}
