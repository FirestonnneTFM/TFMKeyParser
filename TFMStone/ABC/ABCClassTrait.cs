using System;

namespace TFMStone.ABC
{
    internal enum TraitKind : byte { Slot = 0, Method, Getter, Setter, Class, Function, Constant }

    internal class ABCClassTrait
    {
        public uint Name;
        public uint Id;
        public uint VarType;
        public int OptValue;
        public TraitKind Kind;

        public ABCClassTrait(ABCReader reader)
        {
            this.Name = reader.ReadU30();
            byte k = reader.ReadByte();
            this.Kind = (TraitKind)(k & 0xf);
            this.OptValue = 0;
            switch (this.Kind)
            {
                case TraitKind.Slot:
                case TraitKind.Constant:
                    reader.ReadU30();
                    this.VarType = reader.ReadU30();
                    this.Id = reader.ReadU30();
                    if (this.Id != 0)
                        reader.ReadByte();
                    break;
                case TraitKind.Class:
                case TraitKind.Function:
                case TraitKind.Method:
                case TraitKind.Getter:
                case TraitKind.Setter:
                    reader.ReadU30();
                    this.Id = reader.ReadU30();
                    this.VarType = 0;
                    break;
                default:
                    throw new Exception("Class trait kind = " + this.Kind.ToString());
            }
            if ((k >> 6 & 1) != 0)
                ABCBlock.ReadMetadata(reader);
        }
    }
}
