namespace BinarySerialization.Test.Issues.Issue27;

public class LoadCarrierData
{
    #region public properties  

    [FieldOrder(0)]
    [SerializeAs(SerializedType = SerializedType.UInt4)]
    public uint Prop1 { get; set; }

    [FieldOrder(1)]
    [SerializeAs(SerializedType = SerializedType.UInt1)]
    public byte Prop2 { get; set; }

    [FieldOrder(2)]
    [SerializeAs(SerializedType = SerializedType.UInt1)]
    public byte Prop3 { get; set; }

    [FieldOrder(3)]
    [SerializeAs(SerializedType = SerializedType.UInt4)]
    public uint Prop4 { get; set; }

    #endregion
}
