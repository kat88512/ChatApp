using System.Net.Sockets;
using Common.Codes;
using Common.Packets;

namespace ChatClient.MVVM.Model
{
    internal class Client
    {
        private readonly TcpClient _client;
        private readonly string _hostname;
        private readonly int _port;

        public event Action<string[]> usernamesInfoSent;

        public Client()
        {
            _client = new TcpClient();
            _hostname = "127.0.0.1";
            _port = 7891;
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

                var request = ClientPacket.Connect(username);
                PacketWriter.TryWritePacket(_client, request);

                var readMessages = new Thread(ReadMessages);
                readMessages.Start();
            }
        }

        public void ReadMessages()
        {
            while (_client.Connected)
            {
                PacketReader.TryReadPacket(_client, out Packet? packet);
                if (packet is not null)
                {
                    switch ((ServerCode)packet.Code)
                    {
                        case ServerCode.NewChatMessage:
                        {
                            break;
                        }

                        case ServerCode.UsernamesInfo:
                        {
                            var usernames = packet.Content.Split(',');
                            usernamesInfoSent.Invoke(usernames);
                            break;
                        }

                        default:
                        {
                            break;
                        }
                    }
                }
            }
        }

        public void SendMessage(string message)
        {
            PacketWriter.TryWritePacket(_client, ClientPacket.SendChatMessage(message));
        }
    }
}
