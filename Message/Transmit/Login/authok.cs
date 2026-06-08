using System;
using System.Net.Sockets;
using BSFv67_Sharp.Core;
using BSFv67_Sharp.Core.Byte;

namespace BSFv67_Sharp.Message.Transmit.Login
{
    public class authok
    {
        public piranha msg;

        public authok(TcpClient conn)
        {
            msg = new piranha(20104, 1, conn);
        }

        public void encode()
        {
            var w = new wtr(msg.stream);
            long ts = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            string tsStr = ts.ToString();
            w.@long(0, 1);
            w.@long(0, 1);
            w.str("psinatoken");
            w.str(null);
            w.str(null);
            w.@int(67);
            w.@int(264);
            w.@int(1);
            w.str("prod");
            w.@int(0);
            w.@int(0);
            w.@int(1);
            w.str("");
            w.str(tsStr);
            w.str(tsStr);
        }

        public void send()
        {
            encode();
            msg.send();
        }
    }
}
