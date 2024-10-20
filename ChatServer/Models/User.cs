using System.Net.Sockets;

namespace ChatServer.Models
{
    internal class User
    {
        public Guid Id { get; private init; } = Guid.NewGuid();
        public string Username { get; private set; }
        public TcpClient ClientSocket { get; private set; }

        public User(string username, TcpClient clientSocket)
        {
            Username = username;
            ClientSocket = clientSocket;
        }
    }
}
