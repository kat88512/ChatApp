using System.Net.Sockets;
using ChatClient;

try
{
    var client = new Client("127.0.0.1", 7891);

    Console.WriteLine("Enter your username.");
    var username = Console.ReadLine();

    client.Connect(username);

    var printMessages = new Thread(() => ReceiveMessages(client));
    printMessages.Start();

    SendMessages(client);
}
catch (SocketException e)
{
    Console.WriteLine("SocketException: {0}", e);
}

static void ReceiveMessages(Client client)
{
    while (true)
    {
        client.TryReadMessage(out string? message);

        if (message is not null)
        {
            Console.WriteLine(message);
        }
    }
}

static void SendMessages(Client client)
{
    while (true)
    {
        string? message = Console.ReadLine();
        if (message is not null)
        {
            client.SendMessage(message);
        }
    }
}
