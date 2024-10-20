using System.Net.Sockets;
using Common.Codes;
using Common.Packets;

namespace ChatClient
{
    internal class Client
    {
        private readonly TcpClient _client;
        private readonly string _hostname;
        private readonly int _port;

        public Client(string hostname, int port)
        {
            _client = new TcpClient();
            _hostname = hostname;
            _port = port;
        }

        public void Connect(string? username)
        {
            if (username is null)
            {
                return;
            }

            if (!_client.Connected)
            {
                _client.Connect(_hostname, _port);
                var stream = _client.GetStream();

                var request = ClientPacket.Connect(username);
                PacketWriter.TryWritePacket(stream, request);
            }
        }

        public void TryReadMessage(out string? message)
        {
            message = null;

            PacketReader.TryReadPacket(_client.GetStream(), out Packet? packet);
            if (packet is not null)
            {
                switch ((ServerCode)packet.Code)
                {
                    case ServerCode.NewChatMessage:
                    {
                        message = packet.Content;
                        return;
                    }

                    case ServerCode.ServerAnnouncement:
                    {
                        message = packet.Content;
                        return;
                    }

                    case ServerCode.UserConnected:
                    {
                        message = packet.Content;
                        return;
                    }

                    default:
                    {
                        break;
                    }
                }
            }
        }

        public void SendMessage(string message)
        {
            PacketWriter.TryWritePacket(_client.GetStream(), ClientPacket.SendChatMessage(message));
        }
    }
}
