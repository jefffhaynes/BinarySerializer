namespace BinarySerialization.Test.Issues.Issue24;

public class LoadCarrierData
{
    #region constructor

    #endregion

    #region public properties  

    /// <summary>
    ///     Gets or sets type of the load carrier
    /// </summary>
    [FieldOrder(0)]
    [SerializeAs(SerializedType = SerializedType.UInt4)]
    public LoadCarrierType CarrierType { get; set; }

    /// <summary>
    ///     Gets or Sets the data
    /// </summary>
    [FieldOrder(1)]
    [Subtype("CarrierType", LoadCarrierType.Bin1, typeof(Bin1Data))]
    //[Subtype("CarrierType", LoadCarrierType.Bin2, typeof(Bin2Data))]
    //[Subtype("CarrierType", LoadCarrierType.Palette, typeof(PalData))]
    //[Subtype("CarrierType", LoadCarrierType.Pipe, typeof(PipeData))]
    public object Data { get; set; }

    #endregion
}
