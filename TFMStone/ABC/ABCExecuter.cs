using System;
using System.IO;

namespace TFMStone.ABC
{
    internal class ABCExecuter : ABCReader
    {
        public ABCExecuter(Stream stream)
            : base(stream)
        {
            OPArgs.Init();
        }

        public ABCOp ExecStep()
        {
            int immediate;
            return this.ExecStep(out immediate);
        }

        public ABCOp ExecStep(out int param)
        {
            ABCOp op = (ABCOp)this.ReadByte();
            string argPattern = OPArgs.Args[op];
            if (argPattern == null)
                throw new Exception("Missing arg pattern for " + op.ToString());
            param = 0;
            foreach (char c in argPattern)
            {
                switch (c)
                {
                    case 'M':
                        param = (int)this.ReadU30();
                        break;
                    case 'B':
                        param = this.ReadByte();
                        break;
                    case 'H':
                        param = this.ReadS32();
                        break;
                    case 'S':
                        param = this.ReadS24();
                        break;
                    default:
                        throw new Exception("Unknown pattern " + c);
                }
            }
            return op;
        }

        public void ExecUntil(ABCOp expected, int num)
        {
            int count = 0;
            while (count < num)
            {
                if (this.ExecStep() == expected)
                    count++;
            }
        }

        public void JumpRel(int where)
        {
            this.BaseStream.Position += where;
        }
    }
}
