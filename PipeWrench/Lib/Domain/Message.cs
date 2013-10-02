namespace PipeWrench.Lib.Domain
{
    public class Message
    {
        public byte[] Data { get; private set; }

        public Message(byte[] data)
        {
            Data = data;
        }
    }
}
