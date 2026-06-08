using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using BSFv67_Sharp.Core;

namespace BSFv67_Sharp.Gate
{
    public static class msg
    {
        public static async Task dispatch(ushort id, byte[] payload, TcpClient client)
        {
            switch (id)
            {
                case 10100:
                    {
                        var m = new BSFv67_Sharp.Message.Receive.Login.hello(payload, client);
                        m.decode();
                        await m.process();
                        break;
                    }
                case 10101:
                    {
                        var m = new BSFv67_Sharp.Message.Receive.Login.auth(payload, client);
                        m.decode();
                        await m.process();
                        break;
                    }
                default:
                    logger.unknown(id);
                    break;
            }
        }
    }
}
