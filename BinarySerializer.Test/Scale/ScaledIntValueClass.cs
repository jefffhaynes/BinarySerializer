namespace BinarySerialization.Test.Scale
{
    public class ScaledIntValueClass
    {
        [FieldScale(2)]
        public int Value { get; set; }
    }
}