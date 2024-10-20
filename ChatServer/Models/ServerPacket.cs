using Common.Codes;
using Common.Packets;

namespace ChatServer.Models
{
    internal class ServerPacket : Packet
    {
        private ServerPacket(ServerCode code, string content)
            : base((byte)code, content) { }

        public static ServerPacket NewChatMessage(string sender, string message)
        {
            var time = DateTime.Now.ToShortTimeString();
            var content = $"[{time}]{sender}: {message}";
            return new ServerPacket(ServerCode.NewChatMessage, content);
        }

        //Not yet used
        public static ServerPacket ServerAnnouncement(string message)
        {
            return new ServerPacket(ServerCode.ServerAnnouncement, message);
        }

        public static ServerPacket UserConnected(User user)
        {
            var content = $"New user connected: '{user.Username}'";
            return new ServerPacket(ServerCode.UserConnected, content);
        }

        //Not yet used
        public static ServerPacket UserDisconnected(User user)
        {
            var content = $"User: '{user.Username}' disconnected";
            return new ServerPacket(ServerCode.UserDisconnected, content);
        }
    }
}
