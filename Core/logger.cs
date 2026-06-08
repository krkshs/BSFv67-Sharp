using System;
using System.Threading;

namespace BSFv67_Sharp.Core
{
    public static class logger
    {
        private static readonly AsyncLocal<string> addr = new AsyncLocal<string>();

        private static int term_width()
        {
            try
            {
                int w = Console.WindowWidth;
                return w > 20 ? w : 80;
            }
            catch
            {
                return 80;
            }
        }

        private static void center(int visible)
        {
            int w = term_width();
            if (w > visible)
            {
                Console.Write(new string(' ', (w - visible) / 2));
            }
        }

        public static void set_addr_str(string ip)
        {
            addr.Value = ip;
        }

        public static string get_addr()
        {
            return addr.Value ?? "client";
        }

        public static void banner()
        {
            string[] lines = new string[]
            {
                "        ____ _____ ______      __________",
                "       / __ ) ___// ____/   __/ ___/__  /",
                "      / __  \\__ \\/ /_  | | / / __ \\  / / ",
                "     / /_/ /__/ / __/  | |/ / /_/ / / /  ",
                "    /_____/____/_/     |___/\\____/ /_/   ",
                "    Brawl Stars V67 | github.com/FMZNkdv"
            };
            string[] colors = new string[]
            {
                "\x1b[38;2;0;255;255m",
                "\x1b[38;2;0;180;255m",
                "\x1b[38;2;80;100;255m",
                "\x1b[38;2;160;60;255m",
                "\x1b[38;2;220;40;220m",
                "\x1b[38;2;255;180;220m"
            };
            int lineLen = 46;
            Console.WriteLine();
            for (int i = 0; i < lines.Length; i++)
            {
                center(lineLen);
                Console.WriteLine($"{colors[i]}\x1b[1m{lines[i]}\x1b[0m");
            }
        }

        public static void server_info(string msg)
        {
            center(10 + 2 + msg.Length);
            Console.WriteLine($"\x1b[36m\x1b[1m[ SERVER ]\x1b[0m  {msg}");
        }

        public static void connect()
        {
            string a = get_addr();
            center(2 + a.Length + 4 + 13);
            Console.WriteLine($"\x1b[32m\x1b[1m[ {a} ]\x1b[0m\x1b[32m  ●  Connected\x1b[0m");
        }

        public static void disconnect()
        {
            string a = get_addr();
            center(2 + a.Length + 4 + 16);
            Console.WriteLine($"\x1b[33m\x1b[1m[ {a} ]\x1b[0m\x1b[33m  ○  Disconnected\x1b[0m");
        }

        public static void packet_in(ushort id, string name, int len)
        {
            string a = get_addr();
            if (name == "Unknown")
            {
                string msg = $"Not Found ({id})";
                center(2 + a.Length + 4 + 5 + msg.Length);
                Console.WriteLine($"\x1b[34m\x1b[1m[ {a} ]\x1b[0m\x1b[34m  ←  \x1b[0m\x1b[33m{msg}\x1b[0m");
            }
            else
            {
                center(2 + a.Length + 4 + 5 + name.Length);
                Console.WriteLine($"\x1b[34m\x1b[1m[ {a} ]\x1b[0m\x1b[34m  ←  \x1b[0m{name}");
            }
        }

        public static void packet_out(ushort id, string name, int len)
        {
            string a = get_addr();
            if (name == "Unknown")
            {
                string msg = $"Not Found ({id})";
                center(2 + a.Length + 4 + 5 + msg.Length);
                Console.WriteLine($"\x1b[35m\x1b[1m[ {a} ]\x1b[0m\x1b[35m  →  \x1b[0m\x1b[33m{msg}\x1b[0m");
            }
            else
            {
                center(2 + a.Length + 4 + 5 + name.Length);
                Console.WriteLine($"\x1b[35m\x1b[1m[ {a} ]\x1b[0m\x1b[35m  →  \x1b[0m{name}");
            }
        }

        public static void unknown(ushort id)
        {
            string a = get_addr();
            string msg = $"Not Found ({id})";
            center(2 + a.Length + 4 + 5 + msg.Length);
            Console.WriteLine($"\x1b[33m\x1b[1m[ {a} ]\x1b[0m\x1b[33m  ?  {msg}\x1b[0m");
        }

        public static void client_err(string msg)
        {
            string a = get_addr();
            center(2 + a.Length + 4 + 5 + msg.Length);
            Console.WriteLine($"\x1b[31m\x1b[1m[ {a} ]  ✗  {msg}\x1b[0m");
        }

        public static string packet_name(ushort id)
        {
            return id switch
            {
                10100 => "Hello",
                10101 => "Auth",
                20100 => "Hello",
                20104 => "AuthOk",
                24101 => "HomeData",
                _ => "Unknown"
            };
        }
    }
}
