using System;
using System.Collections;

/**
 * A simple class to handle the properties and manage the usage of an unsigned 
 * char buffer.
 * 
 * @auther  Francisco J. Garcia
 * @version 1.0 07/01/09
 */

namespace PCapConverter
{
    public class ByteArray : Printable
    {
        protected byte[] buff;

        public ByteArray()
        {
            buff = null;
        }

        public ByteArray(int len)
        {
            buff = new byte[len];
        }

        public ByteArray(byte[] pdu)
        {
            Data = pdu;
        }

        public ByteArray(byte[] pdu, int len)
        {
            this.buff = new byte[len];
            System.Buffer.BlockCopy(pdu, 0, this.buff, 0, len);
        }

        public byte[] Data
        {
            get
            {
                return this.buff;
            }
            set
            {
                this.buff = new byte[value.Length];
                System.Buffer.BlockCopy(value, 0, this.buff, 0, value.Length);
            }
        }

        public static byte[] ToByteArray(BitArray bits)
        {
            int numBytes = bits.Count / 8;
            if (bits.Count % 8 != 0) numBytes++;

            byte[] bytes = new byte[numBytes];
            int byteIndex = 0, bitIndex = 0;

            for (int i = 0; i < bits.Count; i++)
            {
                if (bits[i])
                    bytes[byteIndex] |= (byte)(1 << (7 - bitIndex));

                bitIndex++;
                if (bitIndex == 8)
                {
                    bitIndex = 0;
                    byteIndex++;
                }
            }

            return bytes;
        }

        public override string toStringTab(int tabs)
        {
            string str = "";

            str += this.toHexString(buff, buff.Length, 16, tabs);

            return str;
        }
    }
}
