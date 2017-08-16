using System;
using System.IO;

using TFMStone.ABC;

namespace TFMStone.KeyGrab
{
    internal class KeyGrabStream : MemoryStream
    {
        public KeyGrabStream()
            : base(20)
        {
        }

        public void WriteMark0(ABCBlock block, byte[] aob)
        {
            using (ABCExecuter exec = new ABCExecuter(new MemoryStream(aob)))
            {
                exec.ExecUntil(ABCOp.iftrue, 1);
                exec.ExecUntil(ABCOp.getlocal, 1);
                this.EmitAll(block, exec);
            }
        }

        public void WriteMark1(ABCBlock block, byte[] aob)
        {
            using (ABCExecuter exec = new ABCExecuter(new MemoryStream(aob)))
            {
                exec.ExecUntil(ABCOp.pushnull, 3);
                exec.ExecUntil(ABCOp.getlocal3, 1);
                this.EmitAll(block, exec);
            }
        }

        public void WriteMark2(ABCBlock block, byte[] aob)
        {
            using (ABCExecuter exec = new ABCExecuter(new MemoryStream(aob)))
            {
                exec.ExecUntil(ABCOp.getlocal1, 1);
                this.EmitAll(block, exec);
            }
        }

        public void WriteMark3(ABCBlock block, byte[] aob)
        {
            using (ABCExecuter exec = new ABCExecuter(new MemoryStream(aob)))
            {
                exec.ExecUntil(ABCOp.getlocal1, 1);
                this.EmitAll(block, exec);
            }
        }

        public void WriteMark4(ABCBlock block, byte[] aob)
        {
            using (ABCExecuter exec = new ABCExecuter(new MemoryStream(aob)))
            {
                exec.ExecUntil(ABCOp.getlocal3, 1);
                this.EmitAll(block, exec);
            }
        }

        private void EmitAll(ABCBlock block, ABCExecuter exec)
        {
            for (int i = 0; i < 4; i++)
            {
                Assert(exec.ReadByte() == 0x60);
                this.WriteByte((byte)block.GetLexInt(exec));
            }
        }

        private static void Assert(bool b)
        {
            if (!b)
                throw new Exception("Assert failed in KeyGrab");
        }
    }
}
