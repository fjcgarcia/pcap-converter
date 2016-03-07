using System;

/**
 * This class creats an input and output stream out of a ByteArray and manages the process
 * of reading and writing data from this input/output stream in network byte order.
 *
 * @author  Francisco J. Garcia
 * @version 1.0 07/01/09
 */

namespace PCapConverter
{
    public class ByteArrayStream : System.IO.MemoryStream
    {
        protected long _len;

        public ByteArrayStream(byte[] array)
            : base(array)
        {
            //Console.WriteLine("Array size:: {0}", array.Length);
            _len = this.Length;
        }

        public bool EOF
        {
            get
            {
                return (this.Position >= _len);
            }
        }

        public byte ReadUByte()
        {
            byte val = 0;
            try
            {
                if (this.Position >= this.Length)
                    throw new IndexOutOfRangeException("ByteArrayStream()::ReadUByte()");
                val = (byte)this.ReadByte();
                return val;

            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public ushort ReadU16Short()
        {
            ushort val = 0;
            try
            {
                if ((this.Position + 1) >= this.Length)
                    throw new IndexOutOfRangeException("ByteArrayStream()::ReadU16Short()");
                byte b1 = (byte)this.ReadByte();
                byte b0 = (byte)this.ReadByte();
                val = (ushort)((b1 << 8) | b0);
                return val;
            }
            catch (Exception e)
            {
                throw e;
            }
        }



        public uint ReadU24Int()
        {
            uint val = 0;
            try
            {
                if ((this.Position + 2) >= this.Length)
                    throw new IndexOutOfRangeException("ByteArrayStream()::ReadU24Int()");
                byte b2 = (byte)this.ReadByte();
                byte b1 = (byte)this.ReadByte();
                byte b0 = (byte)this.ReadByte();
                val = (uint)((b2 << 16) | (b1 << 8) | b0);
                return val;
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        public uint ReadU32Int()
        {
            uint val = 0;
            try
            {
                if ((this.Position + 3) >= this.Length)
                    throw new IndexOutOfRangeException("ByteArrayStream()::ReadU32Int()");
                byte b3 = (byte)this.ReadByte();
                byte b2 = (byte)this.ReadByte();
                byte b1 = (byte)this.ReadByte();
                byte b0 = (byte)this.ReadByte();
                val = (uint)((b3 << 24) | (b2 << 16) | (b1 << 8) | b0);
                return val;
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        // Big- endian notation
        public void WriteU16Short(ushort val)
        {
            try
            {
                if ((this.Position + 1) >= this.Length)
                    throw new IndexOutOfRangeException("ByteArrayStream()::WriteU16Short()");
                byte b1 = (byte)((val >> 8) & 0xff);
                byte b0 = (byte)(val & 0xff);
                this.WriteByte(b0);
                this.WriteByte(b1);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        // Big- endian notation
        public void WriteU32Int(uint val)
        {
            try
            {
                if ((this.Position + 3) >= this.Length)
                    throw new IndexOutOfRangeException("ByteArrayStream()::WriteU32Int()");
                byte b3 = (byte)((val >> 24) & 0xff);
                byte b2 = (byte)((val >> 16) & 0xff);
                byte b1 = (byte)((val >> 8) & 0xff);
                byte b0 = (byte)(val & 0xff);
                this.WriteByte(b0);
                this.WriteByte(b1);
                this.WriteByte(b2);
                this.WriteByte(b3);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        // Big- endian notation
        public void WriteU64Int(long val)
        {
            try
            {
                if ((this.Position + 5) >= this.Length)
                    throw new IndexOutOfRangeException("ByteArrayStream()::WriteU32Int()");
                byte b7 = (byte)((val >> 56) & 0xff);
                byte b6 = (byte)((val >> 48) & 0xff);
                byte b5 = (byte)((val >> 40) & 0xff);
                byte b4 = (byte)((val >> 32) & 0xff);
                byte b3 = (byte)((val >> 24) & 0xff);
                byte b2 = (byte)((val >> 16) & 0xff);
                byte b1 = (byte)((val >> 8) & 0xff);
                byte b0 = (byte)(val & 0xff);
                this.WriteByte(b0);
                this.WriteByte(b1);
                this.WriteByte(b2);
                this.WriteByte(b3);
                this.WriteByte(b4);
                this.WriteByte(b5);
                this.WriteByte(b6);
                this.WriteByte(b7);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
