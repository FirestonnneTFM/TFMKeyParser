using System;

namespace TFMStone.ABC
{
    internal class ABCMethod
    {
        public uint ReturnType;
        public uint[] ParamType;
        public int ParamCount { get { return this.ParamType.Length; } }
        public uint Name;
        public BitField Flags;
        public ABCMethodBody Body = null;

        public ABCMethod(ABCReader reader)
        {
            this.ParamType = new uint[reader.ReadU30()];
            this.ReturnType = reader.ReadU30();
            for (int i = 0; i < this.ParamType.Length; i++)
                this.ParamType[i] = reader.ReadU30();
            this.Name = reader.ReadU30();
            this.Flags = reader.ReadByte();
            if (this.Flags[3])
            {
                uint n = reader.ReadU30();
                for (int i = 0; i < n; i++)
                {
                    reader.ReadU30();
                    reader.ReadByte();
                }
            }
            if (this.Flags[7])
            {
                for (int i = 0; i < this.ParamType.Length; i++)
                    reader.ReadU30();
            }
        }
    }
}
