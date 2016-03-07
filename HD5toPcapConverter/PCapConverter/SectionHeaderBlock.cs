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
 * 
 *  https://www.winpcap.org/ntar/draft/PCAP-DumpFileFormat.html#anchor2
 *    0                   1                   2                   3
 *    0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1
 *    +---------------------------------------------------------------+
 *  0 |                   Block Type = 0x0A0D0D0A                     |
 *    +---------------------------------------------------------------+
 *  4 |                      Block Total Length                       |
 *    +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
 *  8 |                      Byte-Order Magic                         |
 *    +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
 * 12 |          Major Version        |         Minor Version         |
 *    +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
 * 16 |                                                               |
 *    |                          Section Length                       |
 *    |                                                               |
 *    +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
 * 24 /                                                               /
 *    /                      Options (variable)                       /
 *    /                                                               /
 *    +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
 *    |                      Block Total Length                       |
 *    +---------------------------------------------------------------+
 * 
 */

namespace PCapConverter
{
    public class SectionHeaderBlock
    {
        uint _blockType;        // should be 0x0AODOD0A
        uint _fwdTotalLen;      // forward tolat length
        uint _magicNumber;      // magic number 
        ushort _versionMajor;   // major version number (currently 1)
        ushort _versionMinor;   // minor version number (currently 0)
        uint _bkwdTotalLen;     // backward total length

        //64-bit value specifying the length in bytes of the following section, excluding 
        //the Section Header Block itself. Section Length equal -1 (0xFFFFFFFFFFFFFFFF) means 
        //that the size of the section is not specified the only way to skip the section is to parse 
        //the blocks that it contains.
        long _sectionLen;
        bool _isSwapped;

        public const uint Btype = 168627466;
        // if big endian, do nothing on C# reader
        public const uint BigEndianMagic = 2712847316; // value sean is 0xA1B2C3D4, bytes read 0xD4C3B2A1
        // if little endian, swap bytes -AND NIBBLES!!!
        public const uint LittleEndianMagic = 439041101; // value sean is 0x1A2B3C4D, bytes read 0x4D3C2B1A

        // minimum size of block withouth options
        public const int MinBlockSize = 28; // bytes

        public uint BlockType
        {
            get { return _blockType; }
            set { _blockType = value; }
        }

        public uint FwdTotalLen
        {
            get { return _fwdTotalLen; }
            set { _fwdTotalLen = value; }
        }

        public uint MagicNumber
        {
            get { return _magicNumber; }
            set { _magicNumber = value; }
        }

        public bool IsByteSwapped
        {
            get { return _isSwapped; }
            set { _isSwapped = value; }
        }

        public ushort VersionMajor
        {
            get { return _versionMajor; }
            set { _versionMajor = value; }
        }

        public ushort VersionMinor
        {
            get { return _versionMinor; }
            set { _versionMinor = value; }
        }

        public uint BkwdTotalLen
        {
            get { return _bkwdTotalLen; }
            set { _bkwdTotalLen = value; }
        }

        public long SectionLen
        {
            get { return _sectionLen; }
            set { _sectionLen = value; }
        }

        public void toOutputStream(ByteArrayStream aStream)
        {
            aStream.WriteU32Int(BlockType);
            aStream.WriteU32Int(FwdTotalLen);
            aStream.WriteU32Int(MagicNumber);
            aStream.WriteU16Short(VersionMajor);
            aStream.WriteU16Short(VersionMinor);
            aStream.WriteU64Int(SectionLen);
            aStream.WriteU32Int(BkwdTotalLen);
        }
    }
}
