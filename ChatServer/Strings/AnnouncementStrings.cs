namespace ChatServer.Strings
{
    internal static class AnnouncementStrings
    {
        public static string UserConnected(string username)
        {
            return $"--- New user connected: '{username}' ---";
        }

        public static string UserDisconnected(string username)
        {
            return $"--- User: '{username}' disconnected ---";
        }
    }
}
