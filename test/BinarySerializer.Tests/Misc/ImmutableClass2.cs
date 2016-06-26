using System;

namespace BinarySerialization.Test.Misc
{
    public class ImmutableClass2 : IEquatable<ImmutableClass2>
    {
        public static readonly ImmutableClass2 Broadcast = new ImmutableClass2(0xFFFF);
        public static readonly ImmutableClass2 CoordinatorAddress = new ImmutableClass2(0);

        public ImmutableClass2()
        {
        }

        public ImmutableClass2(ulong value)
        {
            Value = value;
        }

        public ImmutableClass2(uint high, uint low)
        {
            High = high;
            Low = low;
        }

        public ulong Value
        {
            get { return ((ulong)High << 32) + Low; }

            set
            {
                High = (uint)((value & 0xFFFFFFFF00000000UL) >> 32);
                Low = (uint)(value & 0x00000000FFFFFFFFUL);
            }
        }

        [Ignore]
        public uint High { get; set; }

        [Ignore]
        public uint Low { get; set; }

        public bool Equals(ImmutableClass2 other)
        {
            return Value.Equals(other.Value);
        }

        public override string ToString()
        {
            return Value.ToString("X16");
        }
    }
}
