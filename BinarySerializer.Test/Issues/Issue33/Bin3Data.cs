namespace BinarySerialization.Test.Issues.Issue33;

public class Bin3Data
{
    /// <summary>
    ///     Gets or sets type of the bin
    /// </summary>
    [FieldOrder(0)]
    [SerializeAs(SerializedType = SerializedType.UInt1)]
    public byte BinType { get; set; }

    /// <summary>
    ///     Gets or sets ident
    /// </summary>
    [FieldOrder(1)]
    [FieldLength(31)]
#pragma warning disable 618
    [SerializeAs(SerializedType = SerializedType.NullTerminatedString)]
#pragma warning restore 618
    public string Ident { get; set; }

    /// <summary>
    ///     Gets or sets Occupancy
    /// </summary>
    [FieldOrder(2)]
    [SerializeAs(SerializedType = SerializedType.UInt1)]
    public BinOccupancy Occupancy { get; set; }

    /// <summary>
    ///     Gets or sets Occupancy
    /// </summary>
    [FieldOrder(3)]
    [FieldLength(6)]
#pragma warning disable 618
    [SerializeAs(SerializedType = SerializedType.NullTerminatedString)]
#pragma warning restore 618
    public BinOccupancy OccupancyString { get; set; }
}
