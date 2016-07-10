namespace BinarySerialization.Test.Misc
{
    public class ImmutableNoPublicConstructorClass
    {
        private ImmutableNoPublicConstructorClass()
        {
        }

        // ReSharper disable UnusedAutoPropertyAccessor.Local
        public byte Value { get; private set; }
        // ReSharper restore UnusedAutoPropertyAccessor.Local
    }
}