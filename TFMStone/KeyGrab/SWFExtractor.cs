using System;
using System.IO;

using TFMStone.ABC;

namespace TFMStone.KeyGrab
{
    internal class SWFExtractor
    {
        public readonly ABCBlock Block;

        public SWFExtractor(string fname)
        {
            try
            {
                byte[] pattern = new byte[] { 0x01, 0x00, 0x00, 0x00, 0x66, 0x72, 0x61, 0x6D, 0x65, 0x31, 0x00 };

                using (FileStream fs = new FileStream(fname, FileMode.Open, FileAccess.Read))
                using (BinaryReader reader = new BinaryReader(fs))
                {
                    int pindex = 0;
                    while (pindex < pattern.Length)
                    {
                        byte b = reader.ReadByte();
                        if (b == pattern[pindex])
                            pindex++;
                        else
                            pindex = 0;
                    }
                    using (ABCReader abcreader = new ABCReader(fs))
                    {
                        this.Block = new ABCBlock(abcreader);
                    }
                }
            }
            catch (EndOfStreamException)
            {
                throw new Exception("Wrong swf");
            }
        }
    }
}
