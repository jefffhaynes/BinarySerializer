namespace BinarySerialization
{
    /// <summary>
    ///     The order for allocations of bits in Bit Fields.
    ///     NOTE: This does not re-order bits.  Just determines
    ///     if they should be retrieved starting from LSB, or MSB
    ///     of a byte
    /// </summary>
    public enum BitOrder
    {
        /// <summary>
        ///     Bit field will start indexed from LSB
        /// </summary>
        LsbFirst = 0,

        /// <summary>
        ///     Bit field will start indexed from MSB
        /// </summary>
        MsbFirst = 1,
    }
}
