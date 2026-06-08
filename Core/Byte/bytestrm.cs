using System;
using System.Collections.Generic;
using System.Text;

namespace BSFv67_Sharp.Core.Byte
{
    public class bytestrm
    {
        public List<byte> buf;
        public int offset;
        public int bit_offset;

        public bytestrm()
        {
            buf = new List<byte>();
            offset = 0;
            bit_offset = 0;
        }

        public bytestrm(byte[] data)
        {
            buf = new List<byte>(data);
            offset = 0;
            bit_offset = 0;
        }

        public int read_int()
        {
            bit_offset = 0;
            int val = (buf[offset] << 24) | (buf[offset + 1] << 16) | (buf[offset + 2] << 8) | buf[offset + 3];
            offset += 4;
            return val;
        }

        public short read_short()
        {
            bit_offset = 0;
            short val = (short)((buf[offset] << 8) | buf[offset + 1]);
            offset += 2;
            return val;
        }

        public string read_string()
        {
            int length = read_int();
            if (length <= 0 || length >= 90000)
                return "";
            byte[] temp = new byte[length];
            buf.CopyTo(offset, temp, 0, length);
            string str = Encoding.UTF8.GetString(temp);
            offset += length;
            return str;
        }

        public int read_vint()
        {
            bit_offset = 0;
            uint result = 0;
            int shift = 0;

            uint b = buf[offset];
            offset += 1;

            uint a1 = (b & 0x40) >> 6;
            uint a2 = (b & 0x80) >> 7;
            uint s = (b << 1) & 0x7E;
            b = s | (a2 << 7) | a1;

            result |= (b & 0x7F) << shift;
            shift += 7;

            while ((b & 0x80) != 0)
            {
                if (shift > 28) break;
                b = buf[offset];
                offset += 1;
                result |= (b & 0x7F) << shift;
                shift += 7;
            }

            int r = (int)result;
            return (r >> 1) ^ (-(r & 1));
        }

        public bool read_boolean()
        {
            return read_vint() >= 1;
        }

        public int[] read_logic_long()
        {
            return new int[] { read_vint(), read_vint() };
        }

        public int[] read_long()
        {
            return new int[] { read_int(), read_int() };
        }

        public int[] read_data_ref()
        {
            int a = read_vint();
            return new int[] { a, a == 0 ? 0 : read_vint() };
        }

        public void write_byte(byte value)
        {
            bit_offset = 0;
            buf.Add(value);
            offset += 1;
        }

        public void write_short(short value)
        {
            bit_offset = 0;
            buf.Add((byte)((value >> 8) & 0xFF));
            buf.Add((byte)(value & 0xFF));
            offset += 2;
        }

        public void write_int(int value)
        {
            bit_offset = 0;
            buf.Add((byte)((value >> 24) & 0xFF));
            buf.Add((byte)((value >> 16) & 0xFF));
            buf.Add((byte)((value >> 8) & 0xFF));
            buf.Add((byte)(value & 0xFF));
            offset += 4;
        }

        public void write_vint(int value)
        {
            bit_offset = 0;
            uint v = (uint)value;

            int flippedSigned = value ^ (value >> 31);
            uint flipped = (uint)flippedSigned;

            uint temp = (v >> 25) & 0x40;
            temp |= v & 0x3F;
            v >>= 6;
            flipped >>= 6;

            if (flipped == 0)
            {
                write_byte((byte)temp);
                return;
            }

            write_byte((byte)(temp | 0x80));
            flipped >>= 7;

            uint r = (flipped != 0) ? 0x80u : 0u;
            write_byte((byte)((v & 0x7F) | r));
            v >>= 7;

            while (flipped != 0)
            {
                flipped >>= 7;
                r = (flipped != 0) ? 0x80u : 0u;
                write_byte((byte)((v & 0x7F) | r));
                v >>= 7;
            }
        }

        public void write_boolean(bool value)
        {
            if (bit_offset == 0)
            {
                buf.Add(0);
                offset += 1;
            }
            if (value)
            {
                buf[offset - 1] |= (byte)(1 << bit_offset);
            }
            bit_offset = (bit_offset + 1) & 7;
        }

        public void write_string(string? value)
        {
            if (value != null)
            {
                byte[] bytes = Encoding.UTF8.GetBytes(value);
                if (bytes.Length > 90000)
                {
                    write_int(-1);
                    return;
                }
                write_int(bytes.Length);
                buf.AddRange(bytes);
                offset += bytes.Length;
            }
            else
            {
                write_int(-1);
            }
        }

        public void write_string_vint(string? value)
        {
            if (value != null)
            {
                byte[] bytes = Encoding.UTF8.GetBytes(value);
                write_vint(bytes.Length);
                buf.AddRange(bytes);
                offset += bytes.Length;
            }
            else
            {
                write_vint(0);
            }
        }

        public void write_long(int v1, int v2)
        {
            write_int(v1);
            write_int(v2);
        }

        public void write_long_long(long value)
        {
            write_int((int)(value >> 32));
            write_int((int)value);
        }

        public void write_logic_long(int v1, int v2)
        {
            write_vint(v1);
            write_vint(v2);
        }

        public void write_data_ref(int v1, int v2)
        {
            if (v1 < 1)
            {
                write_vint(0);
            }
            else
            {
                write_vint(v1);
                write_vint(v2);
            }
        }

        public void write_bytes(byte[]? data)
        {
            if (data != null)
            {
                write_int(data.Length);
                buf.AddRange(data);
                offset += data.Length;
            }
            else
            {
                write_int(-1);
            }
        }

        public void write_hex(string hex)
        {
            if (hex.Length % 2 != 0)
                throw new ArgumentException("OddHexLength");
            for (int i = 0; i < hex.Length; i += 2)
            {
                byte b = Convert.ToByte(hex.Substring(i, 2), 16);
                buf.Add(b);
                offset += 1;
            }
        }
    }

    public class wtr
    {
        private readonly bytestrm s;

        public wtr(bytestrm stream)
        {
            s = stream;
        }

        public void @int(int v) => s.write_int(v);
        public void @short(short v) => s.write_short(v);
        public void @byte(byte v) => s.write_byte(v);
        public void vint(int v) => s.write_vint(v);
        public void str(string? v) => s.write_string(v);
        public void strVInt(string? v) => s.write_string_vint(v);
        public void @long(int a, int b) => s.write_long(a, b);
        public void logicLong(int a, int b) => s.write_logic_long(a, b);
        public void boolean(bool v) => s.write_boolean(v);
        public void hex(string v) => s.write_hex(v);
        public void bytes(byte[]? v) => s.write_bytes(v);
        public void dataRef(int a, int b) => s.write_data_ref(a, b);
    }
}
