namespace BinarySerialization.Test.Misc
{
    public class ImmutableClass
    {
        public ImmutableClass(int value)
        {
            Value = value;
        }

        public int Value { get; private set; }
    }
}
