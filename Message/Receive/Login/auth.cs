using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using BSFv67_Sharp.Core.Byte;

namespace BSFv67_Sharp.Message.Receive.Login
{
    public class auth
    {
        public bytestrm stream;
        public TcpClient conn;
        public int highId;
        public int lowId;
        public string token = "";
        public int major;
        public int build;
        public int content;

        public auth(byte[] payload, TcpClient client)
        {
            stream = new bytestrm(payload);
            conn = client;
        }

        public void decode()
        {
            highId = stream.read_int();
            lowId = stream.read_int();
            token = stream.read_string();
            major = stream.read_vint();
            build = stream.read_vint();
            content = stream.read_vint();
        }

        public async Task process()
        {
            await Task.Delay(1000);

            var ok = new BSFv67_Sharp.Message.Transmit.Login.authok(conn);
            ok.send();

            var home = new BSFv67_Sharp.Message.Transmit.Home.owndata(conn);
            home.send();
        }
    }
}
