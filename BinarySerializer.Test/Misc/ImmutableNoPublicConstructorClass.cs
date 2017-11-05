namespace BinarySerialization.Test.Misc
{
    public class ImmutableNoPublicConstructorClass
    {
        private ImmutableNoPublicConstructorClass()
        {
        }
        
        // ReSharper disable once UnassignedGetOnlyAutoProperty
        public byte Value { get; }
    }
}