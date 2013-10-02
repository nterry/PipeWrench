using System.Threading;

namespace PipeWrench.Lib
{
    static class ExtensionMethods
    {
        public static Thread Run(this Thread thread)
        {
            thread.Start();
            return thread;
        }

    }
}
