using ChatClient.Commands;
using ChatClient.MVVM.Model;
using System.Collections.ObjectModel;
using System.Windows;

namespace ChatClient.MVVM.ViewModel
{
    internal class MainViewModel
    {
        private readonly Client _client;
        public RelayCommand ConnectToServerCommand { get; set; }
        public string? ClientUsername { get; set; }
        public ObservableCollection<string> Usernames { get; set; }

        public MainViewModel()
        {
            Usernames = new ObservableCollection<string>();
            _client = new Client();
            _client.usernamesInfoSent += UsernamesInfoSent;
            ConnectToServerCommand = new RelayCommand(ConnectToServer, CanConnectToServer);
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

        private void UsernamesInfoSent(string[] usernames)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                foreach (string username in usernames)
                {
                    Usernames.Add(username);
                }
            });
        }
    }
}
