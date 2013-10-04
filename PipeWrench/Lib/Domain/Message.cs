namespace PipeWrench.Lib.Domain
{
    public class Message
    {
        public long Id { get; private set; }
        public byte[] Data { get; private set; }

        public Message(byte[] data, long id)
        {
            Data = data;
            Id = id;
        }
    }
}
