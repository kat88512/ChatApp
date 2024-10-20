using System.Net.Sockets;
using System.Text;

namespace Common.Packets
{
    public static class PacketWriter
    {
        private static readonly Encoding _encoding = Encoding.Unicode;

        public static bool TryWritePacket(TcpClient client, Packet packet)
        {
            try
            {
                var stream = client.GetStream();
                var contentAsBytes = _encoding.GetBytes(packet.Content);
                var contentLength = BitConverter.GetBytes(contentAsBytes.Length);

                stream.Write([packet.Code], 0, 1);
                stream.Write(contentLength, 0, 4);
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
