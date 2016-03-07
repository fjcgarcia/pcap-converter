/*
Copyright (c) 2016, Keysight Technologies

All rights reserved.
Redistribution and use in source and binary forms, with or without modification,
are permitted provided that the following conditions are met:

1. Redistributions of source code must retain the above copyright notice,
this list of conditions and the following disclaimer.

2. Redistributions in binary form must reproduce the above copyright notice,
this list of conditions and the following disclaimer in the documentation 
and/or other materials provided with the distribution.
THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/


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
