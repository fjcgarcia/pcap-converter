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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/**
 * @author  Francisco J. Garcia
 * @version 1.0 07/01/09
 * 
 */

namespace PCapConverter
{
    // http://www.radiotap.org/  For full details of this header

    public class RadioTapHdr
    {
        byte _version;  //version (currently 0)
        byte _pad;      //pad
        ushort _len;    //length of entire message including header the TLV payload
        uint _flags;    //data link fields present 

        public const int HdrSize = 8;

        public byte Version
        {
            get { return _version; }
            set { _version = value; }
        }

        public byte Pad
        {
            get { return _pad; }
            set { _pad = value; }
        }

        public ushort Length
        {
            get { return _len; }
            set { _len = value; }
        }

        public uint Flags
        {
            get { return _flags; }
            set { _flags = value; }
        }

        public void toOutputStream(ByteArrayStream aStream)
        {
            aStream.WriteByte(0);
            aStream.WriteByte(0);
            aStream.WriteU16Short(8);
            aStream.WriteU32Int(0);
        }
    }

}
