using Common.Codes;
using Common.Packets;

namespace ChatClient.MVVM.Model
{
    internal class ClientPacket : Packet
    {
        private ClientPacket(ClientCode code, string content)
            : base((byte)code, content) { }

        public static ClientPacket ConnectionRequest(string username)
        {
            return new ClientPacket(ClientCode.ConnectionRequest, username);
        }

        public static ClientPacket SendChatMessageRequest(string message)
        {
            return new ClientPacket(ClientCode.SendChatMessageRequest, message);
        }
    }
}
