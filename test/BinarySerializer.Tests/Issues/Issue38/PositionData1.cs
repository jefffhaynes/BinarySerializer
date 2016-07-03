namespace BinarySerialization.Test.Issues.Issue38
{
    /// <summary>
    /// the position data structure
    /// </summary>
    public class PositionData1
    {
        #region public properties  

        /// <summary>
        /// Gets or sets the adress of position
        /// </summary>
        [FieldOrder(0)]
        [SerializeAs(SerializedType = SerializedType.UInt4)]
        public uint Address { get; set; }

        #endregion

    }
}
