using System.Net;
using System.Net.Sockets;
using ChatServer.Strings;
using Common.Codes;
using Common.Packets;

namespace ChatServer.Models
{
    internal class Server
    {
        private List<User> Users { get; set; } = new List<User>();
        public bool Running { get; private set; }

        private readonly TcpListener _listener;
        private readonly string _chatName;
        private readonly int _port;

        public Server(int port, string chatName)
        {
            _port = port;
            _chatName = chatName;
            _listener = new TcpListener(IPAddress.Any, _port);

            Running = false;
        }

        public void Run()
        {
            Console.WriteLine(
                "Starting the  \"{0}\"TCP Chat Server on port {1}.",
                _chatName,
                _port
            );

            _listener.Start();
            Running = true;

            while (Running)
            {
                if (_listener.Pending())
                {
                    User? newUser = null;

                    var client = _listener.AcceptTcpClient();
                    PacketReader.TryReadPacket(client, out Packet? packet);

                    if (packet is not null)
                    {
                        HandleConnectionRequest(packet, client, out newUser);
                    }

                    if (newUser is not null)
                    {
                        string[] usernames;

                        lock (Users)
                        {
                            usernames = Users.Select(u => u.Username).ToArray();
                        }

                        BroadcastPacket(
                            ServerPacket.UsernamesInfo(usernames),
                            [newUser.ClientSocket]
                        );

                        BroadcastPacket(ServerPacket.UserConnected(newUser), GetUsersAsClients());
                        BroadcastPacket(
                            ServerPacket.ServerAnnouncement(
                                AnnouncementStrings.UserConnected(newUser.Username)
                            ),
                            GetUsersAsClients()
                        );

                        var thread = new Thread(() => ListenForMessages(newUser));
                        thread.Start();
                    }
                    else
                    {
                        client.Close();
                    }
                }
            }
        }

        public void Shutdown() { }

        public void HandleConnectionRequest(Packet packet, TcpClient client, out User? newUser)
        {
            newUser = null;

            if ((ClientCode)packet.Code == ClientCode.ConnectionRequest)
            {
                var username = packet.Content;
                if (string.IsNullOrEmpty(username))
                {
                    return;
                }

                lock (Users)
                {
                    var usernameTaken = Users.Any(u => u.Username == packet.Content);
                    if (!usernameTaken)
                    {
                        newUser = new User(username, client);
                        Users.Add(newUser);
                    }
                }
            }
        }

        public void HandleNewMessageRequest(Packet packet, User user)
        {
            if ((ClientCode)packet.Code == ClientCode.SendChatMessageRequest)
            {
                var message = packet.Content;
                if (string.IsNullOrEmpty(message) || message.Length > 250)
                {
                    return;
                }

                BroadcastPacket(
                    ServerPacket.NewChatMessage(user.Username, message),
                    GetUsersAsClients()
                );
            }
        }

        public void ListenForMessages(User user)
        {
            while (PacketReader.TryReadPacket(user.ClientSocket, out Packet? packet))
            {
                if (packet is not null)
                {
                    HandleNewMessageRequest(packet, user);
                }
            }

            lock (Users)
            {
                Users.Remove(user);
            }

            user.ClientSocket.Close();
        }

        public void BroadcastPacket(ServerPacket packet, IEnumerable<TcpClient> recipents)
        {
            foreach (var recipent in recipents)
            {
                lock (recipent)
                {
                    PacketWriter.TryWritePacket(recipent, packet);
                }
            }
        }

        public IEnumerable<TcpClient> GetUsersAsClients()
        {
            lock (Users)
            {
                return Users.Select(u => u.ClientSocket);
            }
        }
    }
}
