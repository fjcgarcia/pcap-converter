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
 *   0                   1                   2                   3
   0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1
   +---------------------------------------------------------------+
 0 |                    Block Type = 0x00000006                    |
   +---------------------------------------------------------------+
 4 |                      Block Total Length                       |
   +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
 8 |                         Interface ID                          |
   +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
12 |                        Timestamp (High)                       |
   +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
16 |                        Timestamp (Low)                        |
   +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
20 |                         Captured Len                          |
   +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
24 |                          Packet Len                           |
   +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
28 /                                                               /
   /                          Packet Data                          /
   /               variable length, aligned to 32 bits             /
   /                                                               /
   +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
   /                                                               /
   /                      Options (variable)                       /
   /                                                               /
   +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
   |                      Block Total Length                       |
   +---------------------------------------------------------------+

 */
namespace PCapConverter
{
    public class EnhancedPacketBlock
    {
        uint _blockType;         // should be 0x00000006
        uint _fwdTotalLen;      // forward tolat length
        uint _interfaceID;
        uint _tstHigh;
        uint _tstLow;
        uint _capLen;
        uint _packetLen;
        uint _bkwdTotalLen;     // backward total length

        // minimum size of block withouth options and variable packet data
        public const int MinBlockSize = 32; // bytes

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

        public uint InterfaceID
        {
            get { return _interfaceID; }
            set { _interfaceID = value; }
        }

        public uint TimeStampHigh
        {
            get { return _tstHigh; }
            set { _tstHigh = value; }
        }

        public uint TimeStampLow
        {
            get { return _tstLow; }
            set { _tstLow = value; }
        }

        public uint CapturedLen
        {
            get { return _capLen; }
            set { _capLen = value; }
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
            aStream.WriteU32Int(InterfaceID);
            aStream.WriteU32Int(TimeStampHigh);
            aStream.WriteU32Int(TimeStampLow);
            aStream.WriteU32Int(CapturedLen);
            aStream.WriteU32Int(PacketLen);
            //aStream.WriteU32Int(BkwdTotalLen);
        }
    }
}
