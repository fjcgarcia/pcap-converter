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
