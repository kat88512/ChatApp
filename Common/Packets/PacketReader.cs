using System.Net.Sockets;
using System.Text;

namespace Common.Packets
{
    public static class PacketReader
    {
        private static readonly Encoding _encoding = Encoding.UTF8;
        private static readonly int _bufferSize = 2 * 1024;

        public static bool TryReadPacket(NetworkStream stream, out Packet? packet)
        {
            packet = null;

            try
            {
                int bytesRead;
                byte[] buffer = new byte[_bufferSize];

                var code = stream.ReadByte();
                bytesRead = stream.Read(buffer, 0, buffer.Length);

                if (bytesRead > 0)
                {
                    var content = _encoding.GetString(buffer, 0, bytesRead);
                    packet = new Packet((byte)code, content);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (IOException)
            {
                return false;
            }
        }
    }
}
