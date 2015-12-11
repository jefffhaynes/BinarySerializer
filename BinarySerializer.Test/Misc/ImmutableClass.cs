using System;
using System.Reflection.Emit;

namespace BinarySerialization.Test.Misc
{
    public class ImmutableClass
    {
        public ImmutableClass(string value, string value2)
        {
            throw new NotImplementedException();
        }

        public ImmutableClass(int valuE, int value2)
        {
            Value = valuE;
            Value2 = value2;
        }

        public ImmutableClass(int value)
        {
            throw new NotImplementedException();
        }

        public ImmutableClass(string whyDoWeEvenHaveThisConstructor)
        {
            throw new NotImplementedException();
        }

        public ImmutableClass(int value, int value2, int value3)
        {
            throw new NotImplementedException();
        }

        [FieldOrder(0)]
        public int Value { get; private set; }

        [FieldOrder(1)]
        public int Value2 { get; private set; }

        [FieldOrder(2)]
        public int MutableValue { get; set; }
    }
}
