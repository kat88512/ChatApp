namespace Common.Packets
{
    public class Packet
    {
        public byte Code { get; private set; }
        public string Content { get; private set; }

        public Packet(byte code, string content)
        {
            Code = code;
            Content = content;
        }
    }
}
