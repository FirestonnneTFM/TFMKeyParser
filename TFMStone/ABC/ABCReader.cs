using System;
using System.IO;

namespace TFMStone.ABC
{
    internal class ABCReader : BinaryReader
    {
        public ABCReader(Stream stream)
            : base(stream)
        {
        }

        public uint ReadU32()
        {
            uint i;
            uint ret = 0;
            int bytePos = 0;
            int byteCount = 0;
            bool nextByte;
            do
            {
                i = this.ReadByte();
                nextByte = (i >> 7) == 1;
                i &= 0x7f;
                ret += (i << bytePos);
                byteCount++;
                bytePos += 7;
            } while (nextByte && byteCount < 5);
            return ret;
        }

        public uint ReadU30()
        {
            return this.ReadU32() & 0x3FFFFFFF;
        }

        public int ReadS32()
        {
            int i;
            int ret = 0;
            int bytePos = 0;
            int byteCount = 0;
            bool nextByte;
            do
            {
                i = this.ReadByte();
                nextByte = (i >> 7) == 1;
                i &= 0x7f;
                ret += (i << bytePos);
                byteCount++;
                bytePos += 7;
                if (bytePos == 35)
                {
                    if ((ret >> 31) == 1)
                    {
                        ret = -(ret & 0x7fffffff);
                    }
                    break;
                }
            } while (nextByte && byteCount < 5);
            return ret;
        }

        public int ReadS24()
        {
            return this.ReadByte() | (this.ReadByte() >> 8) | (this.ReadByte() >> 16);
        }

        public byte[] ReadStr()
        {
            return this.ReadBytes((int)this.ReadU30());
        }

        public uint BE32()
        {
            return (uint)(this.ReadByte() << 24) | (uint)(this.ReadByte() << 16) | (uint)(this.ReadByte() << 8) | this.ReadByte(); 
        }

        public ushort BE16()
        {
            return (ushort)((this.ReadByte() << 8) | this.ReadByte());
        }
    }
}
