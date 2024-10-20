using System.Net.Sockets;
using System.Text;

namespace Common.Packets
{
    public static class PacketReader
    {
        private static readonly Encoding _encoding = Encoding.Unicode;

        public static bool TryReadPacket(TcpClient client, out Packet? packet)
        {
            packet = null;

            try
            {
                var stream = client.GetStream();

                var code = ReadCode(stream);

                TryReadContent(stream, out string? content);

                if (content is not null)
                {
                    packet = new Packet(code, content);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static byte ReadCode(NetworkStream stream)
        {
            return (byte)stream.ReadByte();
        }

        private static int ReadContentLength(NetworkStream stream)
        {
            byte[] buffer = new byte[4];
            stream.ReadExactly(buffer, 0, 4);
            var contentLength = BitConverter.ToInt32(buffer);

            return contentLength;
        }

        private static bool TryReadContent(NetworkStream stream, out string? content)
        {
            content = null;
            var contentLength = ReadContentLength(stream);

            int bytesRead;
            byte[] buffer = new byte[contentLength];

            bytesRead = stream.Read(buffer, 0, buffer.Length);

            if (bytesRead > 0)
            {
                content = _encoding.GetString(buffer, 0, bytesRead);
                return true;
            }

            return false;
        }
    }
}
