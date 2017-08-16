using System;
using System.IO;
using System.Text;

namespace TFMStone.ABC
{
    internal enum NamespaceKind : byte 
    {
        Public = 0x08,
        Package = 0x16,
        PackageInternal = 0x17,
        Protected = 0x18,
        Explicit = 0x19,
        StaticProtected = 0x1A,
        Private = 0x05
    }

    internal struct NamespaceInfo
    {
        public NamespaceKind Kind;
        public uint Name;
        public NamespaceInfo(NamespaceKind kind, uint name)
        {
            this.Kind = kind;
            this.Name = name;
        }
    }

    internal enum MultinameKind : byte
    {
        QName = 0x07,
        QNameA = 0x0D,
        RTQName = 0x0F,
        RTQNameA = 0x10,
        RTQNameL = 0x11,
        RTQNameLA = 0x12,
        Multiname = 0x09,
        MultinameA = 0x0E,
        MultinameL = 0x1B,
        MultinameLA = 0x1C,
        TypeName = 0x1d
    }

    internal struct MultinameInfo
    {
        public MultinameKind Kind;
        public uint NS;
        public uint Name;
        public MultinameInfo(MultinameKind kind, uint ns, uint name)
        {
            this.Kind = kind;
            this.NS = ns;
            this.Name = name;
        }
    }

    internal class ABCContantPool
    {
        public int[] S32Ints;
        public uint[] U32Ints;
        public double[] Doubles;
        public byte[][] Strings;
        public NamespaceInfo[] Namespaces;
        public uint[][] NamespaceSets;
        public MultinameInfo[] Multinames;

        public ABCContantPool (ABCReader reader)
        {
            uint n = reader.ReadU30();
            this.S32Ints = new int[n];
            for (int i = 1; i < n; i++)
                this.S32Ints[i] = reader.ReadS32();
            n = reader.ReadU30();
            this.U32Ints = new uint[n];
            for (int i = 1; i < n; i++)
                this.U32Ints[i] = reader.ReadU32();
            n = reader.ReadU30();
            this.Doubles = new double[n];
            for (int i = 1; i < n; i++)
                this.Doubles[i] = reader.ReadDouble();
            n = reader.ReadU30();
            this.Strings = new byte[n][];
            this.Strings[0] = new byte[0];
            for (int i = 1; i < n; i++)
                this.Strings[i] = reader.ReadStr();
            n = reader.ReadU30();
            this.Namespaces = new NamespaceInfo[n];
            for (int i = 1; i < n; i++)
            {
                byte a = reader.ReadByte();
                uint b = reader.ReadU30();
                this.Namespaces[i] = new NamespaceInfo((NamespaceKind)a, b);
            }
            n = reader.ReadU30();
            this.NamespaceSets = new uint[n][];
            for (int i = 1; i < n; i++)
            {
                uint len = reader.ReadU30();
                this.NamespaceSets[i] = new uint[len];
                for (int j = 0; j < len; j++)
                    this.NamespaceSets[i][j] = reader.ReadU30();
            }
            n = reader.ReadU30();
            this.Multinames = new MultinameInfo[n];
            for (int i = 1; i < n; i++)
            {
                MultinameKind k = (MultinameKind)reader.ReadByte();
                uint ns = 0;
                uint name = 0;
                switch (k)
                {
                    case MultinameKind.QName:
                    case MultinameKind.QNameA:
                        ns = reader.ReadU30();
                        name = reader.ReadU30();
                        break;
                    case MultinameKind.RTQName:
                    case MultinameKind.RTQNameA:
                        name = reader.ReadU30();
                        break;
                    case MultinameKind.RTQNameL:
                    case MultinameKind.RTQNameLA:
                        break;
                    case MultinameKind.Multiname:
                    case MultinameKind.MultinameA:
                        name = reader.ReadU30();
                        ns = reader.ReadU30();
                        break;
                    case MultinameKind.MultinameL:
                    case MultinameKind.MultinameLA:
                        ns = reader.ReadU30();
                        break;
                    case MultinameKind.TypeName:
                        {
                            name = reader.ReadU30();
                            uint t = reader.ReadU30();
                            while (t-- > 0)
                                reader.ReadU30();
                            break;
                        }
                    default:
                        throw new Exception("MultinameKind = " + k.ToString());
                }
                this.Multinames[i] = new MultinameInfo(k, ns, name);
            }
        }

        public string GetMultinameString(uint name)
        {
            MultinameInfo info = this.Multinames[name];
            if (info.Kind == MultinameKind.TypeName)
                return "<TYPENAME>";
            string ns;
            if (info.NS == 0)
                ns = "*";
            else
                ns = ABCBlock.GetPrintString(this.Strings[this.Namespaces[info.NS].Name]);
            return string.Format("{0}.{1}", ns, ABCBlock.GetPrintString(this.Strings[info.Name]));
        }
    }
}
