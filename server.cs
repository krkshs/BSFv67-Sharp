//srv
// ___ ___ _ _ 
//|_ -|  _| | |
//|___|_|  \_/ 
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using BSFv67_Sharp.Core;
using BSFv67_Sharp.Gate;

namespace BSFv67_Sharp
{
    public class server
    {
        public static async Task Main(string[] args)
        {
            var listener = new TcpListener(IPAddress.Parse(cfg.ip), cfg.port);
            
            //reuse_address = true
            listener.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            
            try
            {
                listener.Start();
            }
            catch (Exception ex)
            {
                logger.server_info($"Failed to start listener: {ex.Message}");
                return;
            }

            logger.banner();
            logger.server_info($"Listening on {cfg.port}");

            while (true)
            {
                try
                {
                    var client = await listener.AcceptTcpClientAsync();
                    _ = Task.Run(() => client_loop(client));
                }
                catch (Exception ex)
                {
                    logger.server_info($"Accept failed: {ex.Message}");
                }
            }
        }

        private static async Task client_loop(TcpClient client)
        {
            using (client)
            {
                string ip = "client";
                try
                {
                    if (client.Client.RemoteEndPoint is IPEndPoint ep)
                    {
                        ip = ep.Address.ToString();
                    }
                }
                catch {}

                logger.set_addr_str(ip);
                logger.connect();

                var netStream = client.GetStream();

                while (true)
                {
                    byte[] header;
                    try
                    {
                        header = await ReadExactlyAsync(netStream, 7);
                    }
                    catch (EndOfStreamException)
                    {
                        logger.disconnect();
                        break;
                    }
                    catch (Exception ex)
                    {
                        logger.client_err($"Read failed  {ex.Message}");
                        break;
                    }

                    ushort msgId = (ushort)((header[0] << 8) | header[1]);
                    int msgLen = (header[2] << 16) | (header[3] << 8) | header[4];
                    ushort msgVer = (ushort)((header[5] << 8) | header[6]);

                    byte[] payload = new byte[msgLen];
                    if (msgLen > 0)
                    {
                        try
                        {
                            payload = await ReadExactlyAsync(netStream, msgLen);
                        }
                        catch (EndOfStreamException)
                        {
                            logger.disconnect();
                            break;
                        }
                        catch (Exception ex)
                        {
                            logger.client_err($"Payload read failed  {ex.Message}");
                            break;
                        }
                    }

                    logger.packet_in(msgId, logger.packet_name(msgId), msgLen);

                    try
                    {
                        await msg.dispatch(msgId, payload, client);
                    }
                    catch (Exception ex)
                    {
                        logger.client_err($"Dispatch failed on {msgId}  {ex.Message}");
                    }
                }
            }
        }

        private static async Task<byte[]> ReadExactlyAsync(NetworkStream stream, int count)
        {
            byte[] buffer = new byte[count];
            int read = 0;
            while (read < count)
            {
                int r = await stream.ReadAsync(buffer, read, count - read);
                if (r <= 0) throw new EndOfStreamException();
                read += r;
            }
            return buffer;
        }
    }
}
