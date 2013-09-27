namespace PipeWrench.Lib.Domain
{
    public class SimpleMutex
    {
        private bool Held { get; set; }

        public SimpleMutex()
        {
            Held = false;
        }

        public bool IsHeld()
        {
            return Held;
        }

        public void Hold()
        {
            Held = true;
        }

        public void Release()
        {
            Held = false;
        }
    }
}
