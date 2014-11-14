namespace BinarySerializer.Test.Primitives
{
    public class PrimitiveContainer<T>
    {
        public PrimitiveContainer()
        {
        }

        public PrimitiveContainer(T value)
        {
            Value = value;
        }

        public T Value { get; set; }
    }
}