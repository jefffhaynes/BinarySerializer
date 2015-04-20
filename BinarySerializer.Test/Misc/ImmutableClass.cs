using System;

namespace BinarySerialization.Test.Misc
{
    public class ImmutableClass
    {
        public ImmutableClass(int value, int value2)
        {
            Value = value;
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
