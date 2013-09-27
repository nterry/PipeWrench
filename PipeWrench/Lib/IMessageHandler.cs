﻿using System;
using PipeWrench.Lib.Tunnels;

namespace PipeWrench.Lib
{
    public delegate void ServiceDispatch(byte[] data);

    public interface IMessageHandler
    {
        event ServiceBindingMessageNotification ServiceBindingReceiveMessage;

        void ReceiveFromTunnel(byte[] data);

        void ReceiveFromServiceBinding(byte[] data);

        void DispatchToServiceBinding();

        void DispatchToTunnel();

        void RecieveThreadDeathFromTunnel(Tunnel tunnel);
    }
}
