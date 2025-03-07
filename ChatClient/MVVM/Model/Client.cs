﻿using System.Net.Sockets;
using Common.Codes;
using Common.Packets;

namespace ChatClient.MVVM.Model
{
    internal class Client
    {
        private readonly TcpClient _client;
        private readonly string _hostname;
        private readonly int _port;

        public event Action<string[]>? UsernamesInfoReceived;
        public event Action<string>? MessageReceived;
        public event Action<string>? UserConnected;
        public bool ConnectionSuccessful { get; private set; }

        public Client()
        {
            _client = new TcpClient();
            _hostname = "127.0.0.1";
            _port = 7891;

            ConnectionSuccessful = false;
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

                var request = ClientPacket.ConnectionRequest(username);
                PacketWriter.TryWritePacket(_client, request);
                WaitForUsernamesInfo();

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
                            MessageReceived?.Invoke(packet.Content);
                            break;
                        }

                        case ServerCode.UserConnected:
                        {
                            UserConnected?.Invoke(packet.Content);
                            break;
                        }

                        case ServerCode.ServerAnnouncement:
                        {
                            MessageReceived?.Invoke(packet.Content);
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

        public void WaitForUsernamesInfo()
        {
            while (!ConnectionSuccessful)
            {
                PacketReader.TryReadPacket(_client, out Packet? packet);
                if (packet is not null && (ServerCode)packet.Code == ServerCode.UsernamesInfo)
                {
                    var usernames = packet.Content.Split(',');
                    UsernamesInfoReceived?.Invoke(usernames);
                    ConnectionSuccessful = true;
                }
            }
        }

        public void SendMessage(string? message)
        {
            if (message is null)
            {
                return;
            }

            PacketWriter.TryWritePacket(_client, ClientPacket.SendChatMessageRequest(message));
        }
    }
}
