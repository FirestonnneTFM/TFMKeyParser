using System;
using System.IO;
using System.Collections.Generic;

namespace TFMStone.ABC
{
    internal class ABCClass
    {
        private const int CACHE_THRESHOLD = 100;

        private ABCBlock Parent;
        public uint Name;
        public uint SuperName;
        public BitField Flags;
        public uint ConstructorMethod;
        public ABCClassTrait[] Traits;
        public uint StaticInitalizer;
        public ABCClassTrait[] StaticTraits;
        private int[] NameToStaticTrait = null;
        private int[] NameToTrait = null;

        public ABCClass(ABCReader reader, ABCBlock parent)
        {
            this.Parent = parent;
            this.Name = reader.ReadU30();
            this.SuperName = reader.ReadU30();
            this.Flags = reader.ReadByte();
            if (this.Flags[3])
                reader.ReadU30();
            uint n = reader.ReadU30();
            while (n-- > 0)
                reader.ReadU30();
            this.ConstructorMethod = reader.ReadU30();
            n = reader.ReadU30();
            this.Traits = new ABCClassTrait[n];
            if (n > CACHE_THRESHOLD)
            {
                this.NameToTrait = new int[this.Parent.NumMultinames];
                for (int i = 0; i < n; i++)
                {
                    this.Traits[i] = new ABCClassTrait(reader);
                    this.NameToTrait[this.Traits[i].Name] = i;
                }
            }
            else
            {
                for (int i = 0; i < n; i++)
                    this.Traits[i] = new ABCClassTrait(reader);
            }
        }

        public void ReadStaticInfo(ABCReader reader)
        {
            this.StaticInitalizer = reader.ReadU30();
            uint n = reader.ReadU30();
            this.StaticTraits = new ABCClassTrait[n];
            if (n > CACHE_THRESHOLD)
            {
                this.NameToStaticTrait = new int[this.Parent.NumMultinames];
                for (int i = 0; i < n; i++)
                {
                    this.StaticTraits[i] = new ABCClassTrait(reader);
                    this.NameToStaticTrait[this.StaticTraits[i].Name] = i;
                }

            }
            else
            {
                for (int i = 0; i < n; i++)
                    this.StaticTraits[i] = new ABCClassTrait(reader);
            }
        }

        public ABCMethod StaticMethodByName(uint name)
        {
            return this.Parent.Methods[this.StaticTraitByName(name).Id];
        }

        public ABCMethod MethodByName(uint name)
        {
            return this.Parent.Methods[this.TraitByName(name).Id];
        }

        public ABCClassTrait TraitByName(uint name)
        {
            if (this.NameToTrait != null)
                return this.Traits[this.NameToTrait[name]];
            foreach (ABCClassTrait t in this.Traits)
            {
                if (t.Name == name)
                    return t;
            }
            return null;
        }

        public ABCClassTrait StaticTraitByName(uint name)
        {
            if (this.NameToStaticTrait != null)
                return this.StaticTraits[this.NameToStaticTrait[name]];
            foreach (ABCClassTrait t in this.StaticTraits)
            {
                if (t.Name == name)
                    return t;
            }
            return null;
        }

        private bool HasStaticInit = false;

        public void DoStaticInit()
        {
            if (this.HasStaticInit)
                return;
            this.HasStaticInit = true;
            using (ABCExecuter exec = new ABCExecuter(new MemoryStream(this.Parent.Methods[this.StaticInitalizer].Body.Code)))
            {
                bool ret = true;
                // if only there was a way to tell the maxstack of a function
                // oh wait there is, and we are ignoring it
                Stack<int> mini = new Stack<int>(10);
                while (ret)
                {
                    int param;
                    ABCOp op = exec.ExecStep(out param);
                    switch (op)
                    {
                        case ABCOp.returnvoid:
                            ret = false;
                            break;
                        case ABCOp.pushint:
                            mini.Push(this.Parent.ConstantPool.S32Ints[param]);
                            break;
                        case ABCOp.pushbyte:
                        case ABCOp.pushshort:
                            mini.Push(param);
                            break;
                        case ABCOp.pushfalse:
                        case ABCOp.pushstring:
                        case ABCOp.getproperty:
                            mini.Push(0);
                            break;
                        case ABCOp.pushtrue:
                            mini.Push(1);
                            break;
                        case ABCOp.not:
                            if (mini.Pop() != 0)
                                mini.Push(0);
                            else
                                mini.Push(1);
                            break;
                        case ABCOp.equals:
                            if (mini.Pop() == mini.Pop())
                                mini.Push(1);
                            else
                                mini.Push(0);
                            break;
                        case ABCOp.dup:
                            mini.Push(mini.Peek());
                            break;
                        case ABCOp.pop:
                            mini.Pop();
                            break;
                        case ABCOp.add:
                            mini.Push(mini.Pop() + mini.Pop());
                            break;
                        case ABCOp.multiply:
                            mini.Push(mini.Pop() * mini.Pop());
                            break;
                        case ABCOp.divide:
                            int p = mini.Pop();
                            int q = mini.Pop();
                            if (p == 0 || q == 0)
                                mini.Push(0);
                            else
                                mini.Push(p / q);
                            break;
                        case ABCOp.setproperty:
                        case ABCOp.initproperty:
                            this.StaticTraitByName((uint)param).OptValue = mini.Pop();
                            break;
                        case ABCOp.jump:
                            exec.JumpRel(param);
                            break;
                        case ABCOp.iffalse:
                            if (mini.Pop() == 0)
                                exec.JumpRel(param);
                            break;
                        case ABCOp.getlocal0:
                        case ABCOp.pushscope:
                        case ABCOp.findproperty:
                        case ABCOp.findpropstrict:
                        case ABCOp.getlex:
                        case ABCOp.callproperty:
                            // do nothing
                            break;
                        default:
                            throw new Exception("StaticInit: not handling op " + op.ToString());
                    }
                }
            }
        }
    }
}