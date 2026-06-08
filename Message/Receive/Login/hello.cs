using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using BSFv67_Sharp.Core.Byte;

namespace BSFv67_Sharp.Message.Receive.Login
{
    public class hello
    {
        public bytestrm stream;
        public TcpClient conn;

        public hello(byte[] payload, TcpClient client)
        {
            stream = new bytestrm(payload);
            conn = client;
        }

        public void decode()
        {
        }

        public async Task process()
        {
            var reply = new BSFv67_Sharp.Message.Transmit.Login.hello(conn);
            reply.send();
            await Task.CompletedTask;
        }
    }
}
