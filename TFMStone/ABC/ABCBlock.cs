using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace TFMStone.ABC
{
    internal class ABCBlock
    {
        public ushort MinorVersion;
        public ushort MajorVersion;
        public ABCContantPool ConstantPool;
        public ABCMethod[] Methods;
        public ABCClass[] Classes;
        private int[] NameToClass;
        public ABCFakeClass[] FakeClasses;

        public int NumMultinames { get { return this.ConstantPool.Multinames.Length; } }

        public ABCBlock(ABCReader reader)
        {
            this.MinorVersion = reader.ReadUInt16();
            this.MajorVersion = reader.ReadUInt16();
            this.ConstantPool = new ABCContantPool(reader);
            uint n = reader.ReadU30();
            this.Methods = new ABCMethod[n];
            for (int i = 0; i < n; i++)
                this.Methods[i] = new ABCMethod(reader);
            ReadMetadata(reader);
            n = reader.ReadU30();
            this.Classes = new ABCClass[n];
            this.NameToClass = new int[this.NumMultinames];
            for (int i = 0; i < n; i++)
            {
                this.Classes[i] = new ABCClass(reader, this);
                this.NameToClass[this.Classes[i].Name] = i;
            }
            for (int i = 0; i < n; i++)
                this.Classes[i].ReadStaticInfo(reader);
            n = reader.ReadU30();
            this.FakeClasses = new ABCFakeClass[n];
            for (int i = 0; i < n; i++)
                this.FakeClasses[i] = new ABCFakeClass(reader);
            n = reader.ReadU30();
            for (int i = 0; i < n; i++)
            {
                ABCMethodBody body = new ABCMethodBody(reader);
                this.Methods[body.MethodIndex].Body = body;
            }
        }

        // stands for `resolve name` since I am sick of having to type it
        public string RN(uint name)
        {
            return this.ConstantPool.GetMultinameString(name);
        }

        public IEnumerable<ABCMethod> GetMethods(ABCClassTrait[] traits)
        {
            foreach (ABCClassTrait t in traits)
            {
                if (t.Kind == TraitKind.Method)
                {
                    this.Methods[t.Id].Name = t.Name;
                    yield return this.Methods[t.Id];
                }
            }
        }

        public static string GetPrintString(byte[] buf)
        {
            StringBuilder sb = new StringBuilder(buf.Length);
            foreach (byte b in buf)
            {
                if (b >= 0x20)
                    sb.Append((char)b);
                else
                    sb.Append('.');
            }
            return sb.ToString();
        }

        public static void ReadMetadata(ABCReader reader)
        {
            uint n = reader.ReadU30();
            for (int i = 0; i < n; i++)
            {
                reader.ReadU30();
                uint itemCount = reader.ReadU30();
                for (int j = 0; j < itemCount; j++)
                {
                    reader.ReadU30();
                    reader.ReadU30();
                }
            }
        }

        public ABCClass ClassByName(uint name)
        {
            return this.Classes[this.NameToClass[name]];
        }

        public int GetLexInt(ABCReader reader)
        {
            ABCClass c = this.ClassByName(reader.ReadU30());
            c.DoStaticInit();
            Assert(reader.ReadByte() == 0x46);
            uint name = reader.ReadU30();
            Assert(reader.ReadU30() == 0);
            ABCMethod m = c.StaticMethodByName(name);
            Assert(m != null);
            int a, b;
            using (ABCReader mreader = new ABCReader(new MemoryStream(m.Body.Code)))
            {
                Assert(mreader.BE16() == 0xD030);
                a = this.ReadPush(mreader);
                b = this.ReadPush(mreader);
                Assert(mreader.ReadByte() == 0xA0);
                Assert(mreader.ReadByte() == 0x48);
            }
            return a + b;
        }

        public string GetLexStr(ABCReader reader)
        {
            ABCClass c = this.ClassByName(reader.ReadU30());
            c.DoStaticInit();
            Assert(reader.ReadByte() == 0x66);
            ABCClassTrait t = c.StaticTraitByName(reader.ReadU30());
            if (t == null)
                return string.Empty;
            else
                return ABCBlock.GetPrintString(this.ConstantPool.Strings[t.Id]);
        }

        public bool GetLexBool(ABCReader reader)
        {
            ABCClass c = this.ClassByName(reader.ReadU30());
            c.DoStaticInit();
            Assert(reader.ReadByte() == 0x66);
            ABCClassTrait t = c.StaticTraitByName(reader.ReadU30());
            return t.OptValue != 0;
        }

        private static void Assert(bool b)
        {
            if (!b)
                throw new Exception("Assert failed in Block");
        }

        private int ReadPush(ABCReader reader)
        {
            byte op = reader.ReadByte();
            if (op == 0x25)
                return reader.ReadS32();
            else if (op == 0x2D)
                return this.ConstantPool.S32Ints[reader.ReadU30()];
            else if (op == 0x24)
                return reader.ReadByte();
            throw new Exception("Assert failed in Block.ReadPush");
        }
    }
}
