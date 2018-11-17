namespace BinarySerialization.Test.Issues.Issue29
{
    public class LoadCarrierData
    {
        #region constructor

        /// <summary>
        ///     Initializes a new instance of the <see cref="LoadCarrierData" /> class.
        /// </summary>
        public LoadCarrierData()
            : this(LoadCarrierType.Unknown, null)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="LoadCarrierData" /> class.
        /// </summary>
        /// <param name="carrierType">
        ///     The carrier Type.
        /// </param>
        /// <param name="data">
        ///     The data.
        /// </param>
        public LoadCarrierData(LoadCarrierType carrierType, BaseCarrierData data)
        {
            CarrierType = carrierType;
            Data = data;
        }

        #endregion

        #region public properties  

        /// <summary>
        ///     Gets or sets type of the load carrier
        /// </summary>
        [FieldOrder(0)]
        [SerializeAs(SerializedType = SerializedType.UInt2)]
        public LoadCarrierType CarrierType { get; private set; }

        /// <summary>
        ///     Gets or Sets the data
        /// </summary>
        [FieldOrder(1)]
        [Subtype("CarrierType", LoadCarrierType.Unknown, typeof (BaseCarrierData))]
        public BaseCarrierData Data { get; private set; }

        #endregion
    }
}