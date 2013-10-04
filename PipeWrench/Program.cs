using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PipeWrench.Lib.MessageHandlers;
using PipeWrench.Lib.ServiceBindings;

namespace PipeWrench
{
    class Program
    {
        static void Main(string[] args)
        {
            new Test();
        }
    }

    class Test
    {
        public Test()
        {
            var serviceBinding = new LocalServiceBinding();

            serviceBinding.MessageReceived += MessageReceived;
            serviceBinding.MessageSendFailure += MessageSendFail;
            serviceBinding.TunnelCreationFailure += TunnelCreationFail;
            serviceBinding.TunnelCreationSuccess += TunnelCreationSucceed;

            serviceBinding.CreateTunnel("Awesome", new KeyValuePair<string, int>("127.0.0.1", 1025));

            while (true)
            {
                
            }
        }

        private void TunnelCreationSucceed()
        {
            Console.WriteLine("Successfully created tunnel.");
        }

        public void MessageReceived(KeyValuePair<string, int> remoteBinding, byte[] data)
        {
            Console.WriteLine(string.Format("Received {0} byte(s) from remote {1}:{2}", data.Count(), remoteBinding.Key, remoteBinding.Value));
        }

        public void MessageSendFail(int errorCode, string message)
        {
            Console.WriteLine(string.Format("Failed to send message. Error was Code: {0} Reason: {1}", errorCode, message));
        }

        public void TunnelCreationFail(int errorCode, string message)
        {
            Console.WriteLine(string.Format("Failed to create a tunnel. Error was Code: {0} Reason: {1}", errorCode, message));
        }
    }
}
