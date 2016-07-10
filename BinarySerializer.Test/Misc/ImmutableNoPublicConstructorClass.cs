namespace BinarySerialization.Test.Misc
{
    public class ImmutableNoPublicConstructorClass
    {
        private ImmutableNoPublicConstructorClass()
        {
        }

        public byte Value { get; private set; }
    }
}