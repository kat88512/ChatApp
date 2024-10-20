using Common.Codes;
using Common.Packets;

namespace ChatClient
{
    internal class ClientPacket : Packet
    {
        private ClientPacket(ClientCode code, string content)
            : base((byte)code, content) { }

        public static ClientPacket Connect(string username)
        {
            return new ClientPacket(ClientCode.Connect, username);
        }

        public static ClientPacket SendChatMessage(string message)
        {
            return new ClientPacket(ClientCode.SendChatMessage, message);
        }
    }
}
