using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

/**
 * @author  Francisco J. Garcia
 * @version 1.0 07/01/09
 * 
 * 
 *      0                   1                   2                   3
 *      0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1
 *      +---------------------------------------------------------------+
 *    0 |                    Block Type = 0x00000001                    |
 *      +---------------------------------------------------------------+
 *    4 |                      Block Total Length                       |
 *      +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
 *    8 |           LinkType            |           Reserved            |
 *      +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
 *   12 |                            SnapLen                            |
 *      +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
 *   16 /                                                               /
 *      /                      Options (variable)                       /
 *      /                                                               /
 *      +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
 *      |                      Block Total Length                       |
 *      +---------------------------------------------------------------+
 *      
 * 
 */

namespace PCapConverter
{
    public class InterfaceDescriptionBlock
    {
        uint _blockType;         // should be 0x00000001
        uint _fwdTotalLen;      // forward tolat length
        ushort _linkType;       // Link Type
        ushort _reserved;       // Reserved
        uint _bkwdTotalLen;     // backward total length

        uint _snapLen;          // max number of bytes dumped from each packet

        // minimum size of block withouth options
        public const int MinBlockSize = 20; // bytes

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

        public ushort LinkType
        {
            get { return _linkType; }
            set { _linkType = value; }
        }

        public ushort Reserved
        {
            get { return _reserved; }
            set { _reserved = value; }
        }

        public uint BkwdTotalLen
        {
            get { return _bkwdTotalLen; }
            set { _bkwdTotalLen = value; }
        }

        public uint SnapLen
        {
            get { return _snapLen; }
            set { _snapLen = value; }
        }

        public void toOutputStream(ByteArrayStream aStream)
        {
            aStream.WriteU32Int(BlockType);
            aStream.WriteU32Int(FwdTotalLen);
            aStream.WriteU16Short(LinkType);
            aStream.WriteU16Short(Reserved);
            aStream.WriteU32Int(SnapLen);
            aStream.WriteU32Int(BkwdTotalLen);
        }
    }
}
