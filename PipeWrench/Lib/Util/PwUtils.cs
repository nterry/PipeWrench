using System;

namespace PipeWrench.Lib.Util
{
    public class PwUtils
    {
        public static int SecondsSinceEpoch()
        {
            var t = DateTime.UtcNow - new DateTime(1970, 1, 1);
            return (int)t.TotalSeconds;
        }
    }
}
