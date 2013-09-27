namespace PipeWrench.Lib.Domain
{
    public class TunnelInfo
    {
        public string Name { get; private set; }
        public int Id { get; private set; }

        private static int _counter;

        public TunnelInfo(string name)
        {
            Name = name;
            Id = _counter;
            _counter++;
        }
    }
}
