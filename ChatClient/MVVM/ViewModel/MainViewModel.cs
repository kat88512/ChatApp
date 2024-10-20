using System.Collections.ObjectModel;
using System.Windows;
using ChatClient.Commands;
using ChatClient.MVVM.Model;

namespace ChatClient.MVVM.ViewModel
{
    internal class MainViewModel
    {
        private readonly Client _client;
        public ObservableCollection<string> Usernames { get; set; } = [];
        public ObservableCollection<string> MessagesHistory { get; set; } = [];

        public string? ClientUsername { get; set; }
        public string? ClientMessage { get; set; }

        public RelayCommand ConnectToServerCommand { get; set; }
        public RelayCommand SendMessageCommand { get; set; }

        public MainViewModel()
        {
            _client = new Client();

            _client.UsernamesInfoReceived += AddUsers;
            _client.MessageReceived += AddToMessageHistory;

            ConnectToServerCommand = new RelayCommand(ConnectToServer, CanConnectToServer);
            SendMessageCommand = new RelayCommand(SendMessage, CanSendMessage);
        }

        private void ConnectToServer(object? obj)
        {
            _client.Connect(ClientUsername);
        }

        private bool CanConnectToServer(object? obj)
        {
            if (string.IsNullOrWhiteSpace(ClientUsername))
            {
                return false;
            }

            return true;
        }

        private void SendMessage(object? obj)
        {
            _client.SendMessage(ClientMessage);
        }

        private bool CanSendMessage(object? obj)
        {
            if (string.IsNullOrWhiteSpace(ClientMessage) || !_client.ConnectionSuccessful)
            {
                return false;
            }

            return true;
        }

        private void AddUsers(string[] usernames)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                foreach (string username in usernames)
                {
                    Usernames.Add(username);
                }
            });
        }

        private void AddToMessageHistory(string message)
        {
            Application.Current.Dispatcher.Invoke(() => MessagesHistory.Add(message));
        }
    }
}
