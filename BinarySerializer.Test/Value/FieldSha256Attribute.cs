namespace BinarySerialization.Test.Value;

public class FieldSha256Attribute : FieldValueAttributeBase
{
    public FieldSha256Attribute(string valuePath) : base(valuePath)
    {
    }

    protected override object GetInitialState(BinarySerializationContext context)
    {
        return IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
    }

    protected override object GetUpdatedState(object state, byte[] buffer, int offset, int count)
    {
        var sha = (IncrementalHash)state;
        sha.AppendData(buffer, offset, count);
        return sha;
    }

    protected override object GetFinalValue(object state)
    {
        var sha = (IncrementalHash)state;
        return sha.GetHashAndReset();
    }
}
