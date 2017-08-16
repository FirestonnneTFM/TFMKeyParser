using System;

namespace TFMStone.ABC
{
    internal class ABCMethodBody
    {
        public uint MethodIndex;
        public byte[] Code;
        public ABCClassTrait[] Traits;

        public ABCMethodBody(ABCReader reader)
        {
            this.MethodIndex = reader.ReadU30();
            reader.ReadU30();
            reader.ReadU30();
            reader.ReadU30();
            reader.ReadU30();
            uint n = reader.ReadU30();
            this.Code = new byte[n];
            for (int i = 0; i < n; i++)
                this.Code[i] = reader.ReadByte();
            n = reader.ReadByte();
            for (int i = 0; i < n; i++)
                for (int j = 0; j < 5; j++)
                    reader.ReadU30();
            n = reader.ReadByte();
            this.Traits = new ABCClassTrait[n];
            for (int i = 0; i < n; i++)
                this.Traits[i] = new ABCClassTrait(reader);
        }
    }
}
