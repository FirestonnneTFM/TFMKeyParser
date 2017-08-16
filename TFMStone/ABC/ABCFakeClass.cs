using System;

namespace TFMStone.ABC
{
    internal class ABCFakeClass
    {
        public uint EntryPoint;
        public ABCClassTrait[] Traits;

        public ABCFakeClass(ABCReader reader)
        {
            this.EntryPoint = reader.ReadU30();
            uint n = reader.ReadU30();
            this.Traits = new ABCClassTrait[n];
            for (int i = 0; i < n; i++)
                this.Traits[i] = new ABCClassTrait(reader);
        }
    }
}
