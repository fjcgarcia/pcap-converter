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
 * 
      0                   1                   2                   3
    0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1
   +---------------------------------------------------------------+
 0 |                    Block Type = 0x00000003                    |
   +---------------------------------------------------------------+
 4 |                      Block Total Length                       |
   +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
 8 |                          Packet Len                           |
   +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
12 /                                                               /
   /                          Packet Data                          /
   /             variable length, aligned to 32 bits               /
   /                                                               /
   +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
   |                      Block Total Length                       |
   +---------------------------------------------------------------+
*/

namespace PCapConverter
{
    public class SimplePacketBlock
    {
        uint _blockType;         // should be 0x00000003
        uint _fwdTotalLen;      // forward tolat length
        uint _packetLen;
        uint _bkwdTotalLen;     // backward total length

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

        public uint PacketLen
        {
            get { return _packetLen; }
            set { _packetLen = value; }
        }

        public uint BkwdTotalLen
        {
            get { return _bkwdTotalLen; }
            set { _bkwdTotalLen = value; }
        }

        public void toOutputStream(ByteArrayStream aStream)
        {
            aStream.WriteU32Int(BlockType);
            aStream.WriteU32Int(FwdTotalLen);
            aStream.WriteU32Int(PacketLen);
            //aStream.WriteU32Int(BkwdTotalLen);
        }

    }
}
