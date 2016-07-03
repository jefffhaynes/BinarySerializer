namespace BinarySerialization
{
    /// <summary>
    /// Used to represent the underlying byte-ordering.
    /// </summary>
    public enum Endianness
    {
        /// <summary>
        /// Indicates that endianness should be inherited from the parent object in the object graph.
        /// </summary>
        Inherit,

        /// <summary>
        /// Indicates that little-endian byte ordering should be observed.
        /// </summary>
        Little,

        /// <summary>
        /// Indicates that big-endian byte ordering should be observed.
        /// </summary>
        Big,
    }
}
