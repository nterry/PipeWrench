using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using PipeWrench.Lib.MessageHandlers;

namespace PipeWrench.Lib.Tunnels
{
    public class TunnelManager
    {
        private static readonly IMessageHandler MessageHandler;
        private static readonly ConcurrentBag<Tunnel> Tunnels;

        static TunnelManager()
        {
            MessageHandler = DefaultMessageHandler.GetExistingOrNew();
            Tunnels = new ConcurrentBag<Tunnel>();
        }

        public static Tunnel CreateTunnel(string friendlyName, string remoteIp, int remotePort)
        {
            var tunnel = new Tunnel(MessageHandler, friendlyName).Run(remoteIp, remotePort);
            Tunnels.Add(tunnel);
            return tunnel;
        }

        public static Tunnel CreateTunnel(string friendlyName, KeyValuePair<string, int> remoteBinding)
        {
            var tunnel = CreateTunnel(friendlyName, remoteBinding.Key, remoteBinding.Value);
            Tunnels.Add(tunnel);
            return tunnel;
        }

        public static Tunnel GetTunnelById(int id)
        {
            return Tunnels.FirstOrDefault(x => x.GetId() == id);
        }

        public static Tunnel GetTunnelByRemoteBinding(string remoteIp, int remotePort)
        {
            return GetTunnelByRemoteBinding(new KeyValuePair<string, int>(remoteIp, remotePort));
        }

        public static Tunnel GetTunnelByRemoteBinding(KeyValuePair<string, int> remoteBinding)
        {
            return Tunnels.FirstOrDefault(x => x.RemoteBinding.Equals(remoteBinding));
        }
    }
}
