namespace BinarySerialization.Test.Misc
{
    public class PrivateSetterClass
    {
        public PrivateSetterClass()
        {
            Value = 33;
        }

        public int Value { get; private set; }
    }
}