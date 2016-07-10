namespace BinarySerialization.Test.Issues.Issue25
{
    public class LoadCarrierData
    {
        #region public properties  

        /// <summary>
        ///     Gets or sets type of the load carrier
        /// </summary>
        [FieldOrder(0)]
        [SerializeAs(SerializedType = SerializedType.UInt4)]
        public LoadCarrierType CarrierType { get; set; }

        /// <summary>
        ///     Gets or sets the BinData 1.
        /// </summary>
        [FieldOrder(1)]
        [SerializeWhen("CarrierType", LoadCarrierType.Bin1)]
        public string Bin1 { get; set; }

        /// <summary>
        ///     Gets or sets the BinData 2.
        /// </summary>
        [FieldOrder(2)]
        [SerializeWhen("CarrierType", LoadCarrierType.Bin2)]
        public string Bin2 { get; set; }

        /// <summary>
        ///     Gets or sets the pallet data.
        /// </summary>
        [FieldOrder(3)]
        [SerializeWhen("CarrierType", LoadCarrierType.Palette)]
        public string PalletData { get; set; }

        /// <summary>
        ///     Gets or sets the pallet data.
        /// </summary>
        [FieldOrder(4)]
        [SerializeWhen("CarrierType", LoadCarrierType.Pipe)]
        public string PipeData { get; set; }

        /// <summary>
        ///     Gets or Sets the data
        /// </summary>
        [Ignore]
        public object Data { get; set; }

        #endregion
    }
}