using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Globalization;

using PCapConverter;
using HD5toPcapConverter.FileReaders;

namespace HD5toPcapConverter
{
    class Program
    {
        static bool read = true;
        static BinaryWriter bOut;

        static ByteArrayStream asOut;

        static DateTime time;

        static void Main(string[] args)
        {
            Hdf5FileReader df5;
            string inFile = args[0];

            string outFile = inFile.Replace(".hd5", ".pcapng");

            bool le = BitConverter.IsLittleEndian;

            //DateTime time = new DateTime(2010, 9, 4);
            time = DateTime.Now;
            Console.WriteLine("Time:: " + time.ToUniversalTime().ToLongTimeString());

            // create byte array for data marshalling
            ByteArray buff = new ByteArray(512);
            asOut = new ByteArrayStream(buff.Data);

            // create PCAPNG file
            bOut = new BinaryWriter(File.OpenWrite(outFile));

            // create Section Header Block
            SectionHeaderBlock hblock = new SectionHeaderBlock();
            hblock.BlockType = SectionHeaderBlock.Btype;
            hblock.FwdTotalLen = SectionHeaderBlock.MinBlockSize; // total size of this block
            hblock.MagicNumber = SectionHeaderBlock.LittleEndianMagic;
            hblock.VersionMajor = (ushort)1;
            hblock.VersionMinor = (ushort)0;
            hblock.SectionLen = (long)-1;
            hblock.BkwdTotalLen = SectionHeaderBlock.MinBlockSize; // total size of this block
            hblock.toOutputStream(asOut);

            // create Interface Description Block
            InterfaceDescriptionBlock iblock = new InterfaceDescriptionBlock();
            iblock.BlockType = 1;
            iblock.FwdTotalLen = InterfaceDescriptionBlock.MinBlockSize;  // total size of this block
            iblock.LinkType = 127;
            iblock.SnapLen = 65535;
            iblock.BkwdTotalLen = InterfaceDescriptionBlock.MinBlockSize;  // total size of this block
            iblock.toOutputStream(asOut);

            bOut.Write(asOut.ToArray(), 0, (int)asOut.Position);

            df5 = new Hdf5FileReader();
            df5.StatusEvent += new Hdf5FileReader.Hdf5FileReaderStatusEvent(df5_StatusEvent);
            df5.DataEvent += new Hdf5FileReader.Hdf5FileReaderReaderData(df5_DataEvent);

            df5.FileName = inFile;
            df5.Start();
            while (read)
            {
            }

            // close ouput file
            bOut.Close();
        }

        static void df5_StatusEvent(object sender, Hdf5FileReader.Hdf5FileReaderEnum mode)
        {
            if (mode == Hdf5FileReader.Hdf5FileReaderEnum.EndData)
            {
                Console.WriteLine("Finished Reading HDF5 File.");
                read = false;
            }
        }

        static void df5_DataEvent(object sender, List<byte[]> data)
        {
            if (data != null)
            {
                process_data(data);
            }
        }

        /*
        static void process_data(List<byte[]> data)
        {
            // Create Enhanced Packet Block
            SimplePacketBlock sblock = new SimplePacketBlock();
            sblock.BlockType = 3;
            ByteArray buff = new ByteArray(512);
            foreach (var item in data)
            {
                if (item != null)
                {
                    asOut = new ByteArrayStream(buff.Data);
                    byte[] val = item;
                    int pad = val.Length % 16;
                    uint tot = (uint)(val.Length + 16 +pad);
                    sblock.FwdTotalLen = tot;
                    sblock.PacketLen = (uint)val.Length;
                    sblock.BkwdTotalLen = tot;
                    sblock.toOutputStream(asOut);
                    long totBytes = asOut.Position;
                    bOut.Write(asOut.ToArray(), 0, (int)totBytes);
                    bOut.Write(val, 0, val.Length);
                    for (int i = 0; i < pad; i++)
                        bOut.Write((byte)0);
                    bOut.Write((uint)tot);
                }
            }
        }*/

        
        static void process_data(List<byte[]> data)
        {
            // Create Enhanced Packet Block
            EnhancedPacketBlock eblock = new EnhancedPacketBlock();
            eblock.BlockType = 6;
            eblock.InterfaceID = 0;
            eblock.TimeStampHigh = 0;
            eblock.TimeStampLow = 0;
            ByteArray buff = new ByteArray(512);
            foreach (var item in data)
            {
                if (item != null)
                {
                    asOut = new ByteArrayStream(buff.Data);
                    RadioTapHdr rtap = new RadioTapHdr();
                    byte[] val = item;
                    int pad = val.Length % 16;
                    uint tot = (uint)(val.Length + EnhancedPacketBlock.MinBlockSize + 8 + pad);
                    eblock.FwdTotalLen = tot;
                    eblock.PacketLen = (uint)(val.Length +8);
                    eblock.CapturedLen = (uint)(val.Length +8);
                    eblock.BkwdTotalLen = tot;
                    eblock.toOutputStream(asOut);
                    rtap.toOutputStream(asOut);
                    long totBytes = asOut.Position;
                    bOut.Write(asOut.ToArray(), 0, (int)totBytes);
                    bOut.Write(val, 0, val.Length);
                    for (int i = 0; i < pad; i++)
                        bOut.Write((byte)0);
                    bOut.Write((uint)tot);
                }
            }
        }
    }
}
