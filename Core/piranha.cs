using System;
using System.Net.Sockets;
using BSFv67_Sharp.Core.Byte;

namespace BSFv67_Sharp.Core
{
    public class piranha
    {
        public bytestrm stream;
        public ushort id;
        public ushort version;
        public TcpClient conn;

        public piranha(ushort id, ushort version, TcpClient conn)
        {
            this.stream = new bytestrm();
            this.id = id;
            this.version = version;
            this.conn = conn;
        }

        public void send()
        {
            if (id < 20000) return;

            int bodyLen = stream.buf.Count;

            byte[] header = new byte[7];
            header[0] = (byte)((id >> 8) & 0xFF);
            header[1] = (byte)(id & 0xFF);
            header[2] = (byte)((bodyLen >> 16) & 0xFF);
            header[3] = (byte)((bodyLen >> 8) & 0xFF);
            header[4] = (byte)(bodyLen & 0xFF);
            header[5] = (byte)((version >> 8) & 0xFF);
            header[6] = (byte)(version & 0xFF);

            var netStream = conn.GetStream();
            netStream.Write(header, 0, 7);
            if (bodyLen > 0)
            {
                netStream.Write(stream.buf.ToArray(), 0, bodyLen);
            }
            netStream.Flush();

            logger.packet_out(id, logger.packet_name(id), bodyLen);
        }
    }
}
