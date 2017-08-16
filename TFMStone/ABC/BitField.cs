using System;
using System.Text;

namespace TFMStone.ABC
{
    internal class BitField
    {
        public byte Value;

        public BitField(byte value)
        {
            this.Value = value;
        }

        public bool this[int index]
        {
            get
            {
                if (index < 0 || index >= 8)
                    throw new IndexOutOfRangeException();
                return (this.Value >> index & 1) != 0;
            }
            set
            {
                if (index < 0 || index >= 8)
                    throw new IndexOutOfRangeException();
                if (value != this[index])
                    this.Value ^= (byte)(1 << index);
            }
        }

        public static implicit operator BitField(byte b)
        {
            return new BitField(b);
        }

        public static implicit operator byte(BitField obj)
        {
            return obj.Value;
        }

        public override string ToString()
        {
            return this.ToString(false);
        }

        public string ToString(bool asFlags)
        {
            if (asFlags)
            {
                StringBuilder sb = new StringBuilder(8);
                for (int i = 7; i >= 0; i--)
                {
                    if (this[i])
                        sb.Append('1');
                    else
                        sb.Append('0');
                }
                return sb.ToString();
            }
            else
            {
                return this.Value.ToString();
            }
        }
    }
}
