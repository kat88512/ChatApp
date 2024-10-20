using System.Net.Sockets;
using System.Text;

namespace Common.Packets
{
    public static class PacketWriter
    {
        private static readonly Encoding _encoding = Encoding.UTF8;

        public static bool TryWritePacket(NetworkStream stream, Packet packet)
        {
            try
            {
                var contentAsBytes = _encoding.GetBytes(packet.Content);

                stream.Write([packet.Code], 0, 1);
                stream.Write(contentAsBytes, 0, contentAsBytes.Length);
            }
            catch (IOException)
            {
                return false;
            }

            return true;
        }
    }
}
