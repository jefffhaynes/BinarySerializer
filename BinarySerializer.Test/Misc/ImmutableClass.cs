// ReSharper disable UnusedParameter.Local

namespace BinarySerialization.Test.Misc;

public class ImmutableClass
{
    public ImmutableClass(string value, string value2)
    {
        throw new NotSupportedException();
    }

    public ImmutableClass(int valuE, int value2)
    {
        Value = valuE;
        Value2 = value2;
    }

    public ImmutableClass(int value)
    {
        throw new NotSupportedException();
    }

    public ImmutableClass(string whyDoWeEvenHaveThisConstructor)
    {
        throw new NotSupportedException();
    }

    public ImmutableClass(int value, int value2, int value3)
    {
        throw new NotSupportedException();
    }

    [FieldOrder(0)]
    public int Value { get; }

    [FieldOrder(1)]
    public int Value2 { get; }

    [FieldOrder(2)]
    public int MutableValue { get; set; }
}
