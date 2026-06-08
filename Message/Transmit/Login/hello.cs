using System;
using System.Net.Sockets;
using BSFv67_Sharp.Core;
using BSFv67_Sharp.Core.Byte;

namespace BSFv67_Sharp.Message.Transmit.Login
{
    public class hello
    {
        public piranha msg;

        public hello(TcpClient conn)
        {
            msg = new piranha(20100, 0, conn);
        }

        public void encode()
        {
            var w = new wtr(msg.stream);
            w.@int(24);
            for (int i = 0; i < 24; i++)
            {
                w.@byte(1);
            }
        }

        public void send()
        {
            encode();
            msg.send();
        }
    }
}
