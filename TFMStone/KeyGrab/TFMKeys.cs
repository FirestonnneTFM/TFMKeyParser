using System;
using System.Text;
using System.IO;

using TFMStone.ABC;

namespace TFMStone.KeyGrab
{
    internal static class TFMKeys
    {
        public static byte[] HashKey;
        public static int LoginKey;
        public static ushort HandshakeNumber;
        public static string HandshakeString;

        public static void GetKeys()
        {
            SWFExtractor swf = new SWFExtractor("sentinel.swf");
            SearchBlock(swf.Block);
        }

        private static void SearchBlock(ABCBlock block)
        {
            ABCClass tfmMain = block.Classes[0];
            tfmMain.DoStaticInit();
            LoginKey = FindLoginConst(block, tfmMain);
            HandshakeNumber = FindHandshakeConst(block, tfmMain, block.Methods[tfmMain.ConstructorMethod]);
            ABCMethod mark0 = block.Methods[tfmMain.ConstructorMethod];
            ABCMethod mark1 = null;
            ABCMethod mark2 = null;
            ABCMethod mark3 = null;
            ABCMethod mark4 = null;
            ABCMethod handshake = null;
            foreach (ABCMethod m in block.GetMethods(tfmMain.Traits))
            {
                if (m.ParamCount == 1 && block.RN(m.ParamType[0]).Equals("flash.events.Event"))
                {
                    if (QuickMatch(m.Body.Code, new byte[] { 0xd0, 0x30, 0x57, 0x2a, 0xd6, 0x30, 0x65, 0x01, 0x20 }))
                    {
                        if (mark1 != null)
                            throw new Exception("Mark1 not unique");
                        mark1 = m;
                    }
                    if (IsHandshake(block, m))
                    {
                        if (handshake != null)
                            throw new Exception("Handshake method not unique");
                        handshake = m;
                    }
                }
                if (m.ParamCount == 0 && block.RN(m.ReturnType).Equals(".void"))
                {
                    if (block.RN(m.Name).Equals(".Initialisation"))
                    {
                        mark3 = m;
                    }
                    if (m.Body.Code.Length > 100 && m.Body.Code.Length < 200 && m.Body.Code[0] == 0xD0 && m.Body.Code[1] == 0x30)
                    {
                        if (m.Body.Code[2] == 0x5D)
                        {
                            if (mark2 != null)
                                throw new Exception("Mark2 not unique");
                            mark2 = m;
                        }
                        if (IsMark4(block, m))
                        {
                            if (mark4 != null)
                                throw new Exception("Mark4 not unique");
                            mark4 = m;
                        }
                    }
                }
            }
            if (mark0 == null)
                throw new Exception("Mark0 not found");
            if (mark1 == null)
                throw new Exception("Mark1 not found");
            if (mark2 == null)
                throw new Exception("Mark2 not found");
            if (mark3 == null)
                throw new Exception("Mark3 not found");
            if (mark4 == null)
                throw new Exception("Mark4 not found");
            if (handshake == null)
                throw new Exception("HandshakeMark not found");
            using (KeyGrabStream emit = new KeyGrabStream())
            {
                emit.WriteMark0(block, mark0.Body.Code);
                emit.WriteMark1(block, mark1.Body.Code);
                emit.WriteMark2(block, mark2.Body.Code);
                emit.WriteMark3(block, mark3.Body.Code);
                emit.WriteMark4(block, mark4.Body.Code);
                HashKey = emit.ToArray();
            }
            FindHandshakeString(block, handshake);
        }

        private static bool IsMark4(ABCBlock block, ABCMethod m)
        {
            try
            {
                using (ABCReader reader = new ABCReader(new MemoryStream(m.Body.Code)))
                {
                    reader.ReadUInt16();
                    if (reader.ReadByte() != 0x60)
                        return false;
                    reader.ReadU30();
                    if (reader.ReadUInt16() != 0x66D0)
                        return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static bool IsHandshake(ABCBlock block, ABCMethod m)
        {
            try
            {
                using (ABCExecuter exec = new ABCExecuter(new MemoryStream(m.Body.Code)))
                {
                    if (exec.BE16() != 0xd030)
                        return false;
                    if (exec.ExecStep() != ABCOp.pushnull)
                        return false;
                    if (exec.ReadByte() != 0x80)
                        return false;
                    if (block.RN(exec.ReadU30()).Equals("flash.text.Font"))
                        return true;
                }
            }
            catch
            {
            }
            return false;
        }

        private static bool QuickMatch(byte[] parent, byte[] child)
        {
            if (child.Length > parent.Length)
                return false;
            for (int i = 0; i < child.Length; i++)
            {
                if (parent[i] != child[i])
                    return false;
            }
            return true;
        }

        private static int FindLoginConst(ABCBlock b, ABCClass c)
        {
            foreach (ABCMethod m in b.GetMethods(c.Traits))
            {
                if (m.ParamCount == 0 && b.RN(m.ReturnType).Equals(".int"))
                    return LoginConstExec(b, m.Body.Code);
            }
            return 0;
        }

        private static int LoginConstExec(ABCBlock block, byte[] aob)
        {
            int value = 0;
            using (ABCReader reader = new ABCReader(new MemoryStream(aob)))
            {
                Assert(reader.BE32() == 0xD030D066);
                reader.ReadU30();
                Assert(reader.ReadByte() == 0x73);
                byte sent;
                while ((sent = reader.ReadByte()) == 0xD5)
                {
                    Assert(reader.ReadByte() == 0xD1);
                    byte op = reader.ReadByte();
                    if (op == 0x48)
                        break;
                    Assert(op == 0x60);
                    int a = block.GetLexInt(reader);
                    op = reader.ReadByte();
                    if (op == 0xAA)
                    {
                        value ^= a;
                        continue;
                    }
                    Assert(op == 0x60);
                    int b = block.GetLexInt(reader);
                    Assert(reader.ReadUInt16() == 0xAAA5);
                    value ^= (a << b);
                }
            }
            return value;
        }

        private static void FindHandshakeString(ABCBlock block, ABCMethod m)
        {
            StringBuilder sb = new StringBuilder();
            using (ABCExecuter exec = new ABCExecuter(new MemoryStream(m.Body.Code)))
            {
                exec.ExecUntil(ABCOp.inclocal_i, 1);
                exec.ExecUntil(ABCOp.setlocal, 1);
                exec.ExecUntil(ABCOp.getproperty, 3);
                Assert(exec.ReadByte() == 0x60);
                sb.Append(block.GetLexStr(exec));
                do
                {
                    Assert(exec.ReadByte() == 0x60);
                    sb.Append(block.GetLexStr(exec));
                } while (exec.ExecStep() == ABCOp.add);
            }
            HandshakeString = sb.ToString();
        }

        private static ushort FindHandshakeConst(ABCBlock block, ABCClass c, ABCMethod m)
        {
            using (ABCExecuter exec = new ABCExecuter(new MemoryStream(m.Body.Code)))
            {
                exec.ExecUntil(ABCOp.constructsuper, 1);
                exec.ExecUntil(ABCOp.setproperty, 1);
                Assert(exec.ReadByte() == 0x60);
                if (!block.GetLexBool(exec))
                    throw new Exception("The handshake const is burried deeper");
                return (ushort)block.ConstantPool.Doubles[c.Traits[0].Id];
                /*
                int jmp;
                Assert(exec.ExecStep(out jmp) == ABCOp.iffalse);
                if (! b)
                    exec.JumpRel(jmp);

                */
            }
        }

        public static void Assert(bool b)
        {
            if (!b)
                throw new Exception("Assert failed in TFMKeys");
        }
    }
}
