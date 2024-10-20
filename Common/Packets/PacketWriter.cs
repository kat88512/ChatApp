using System.Net.Sockets;
using System.Text;

namespace Common.Packets
{
    public static class PacketWriter
    {
        private static readonly Encoding _encoding = Encoding.UTF8;

        public static bool TryWritePacket(TcpClient client, Packet packet)
        {
            try
            {
                var stream = client.GetStream();
                var contentAsBytes = _encoding.GetBytes(packet.Content);

                stream.Write([packet.Code], 0, 1);
                stream.Write(contentAsBytes, 0, contentAsBytes.Length);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }
}
