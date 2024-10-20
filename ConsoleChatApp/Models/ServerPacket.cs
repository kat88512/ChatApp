using Common.Codes;
using Common.Packets;

namespace Server.Models
{
    internal class ServerPacket : Packet
    {
        private ServerPacket(ServerCode code, string content)
            : base((byte)code, content) { }

        public static ServerPacket NewChatMessage(string sender, string message)
        {
            var content = $"{sender} : {message}";
            return new ServerPacket(ServerCode.NewChatMessage, content);
        }

        //Not yet used
        public static ServerPacket ServerAnnouncement(string message)
        {
            return new ServerPacket(ServerCode.ServerAnnouncement, message);
        }

        public static ServerPacket UserConnected(User user)
        {
            return new ServerPacket(ServerCode.UserConnected, user.Username);
        }

        //Not yet used
        public static ServerPacket UserDisconnected(User user)
        {
            return new ServerPacket(ServerCode.UserDisconnected, user.Username);
        }
    }
}
