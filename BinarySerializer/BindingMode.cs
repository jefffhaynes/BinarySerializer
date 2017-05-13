namespace BinarySerialization
{
    /// <summary>
    ///     The serialization binding mode.
    /// </summary>
    public enum BindingMode
    {
        /// <summary>
        ///     Update the source during serialization and the target during deserialization.
        /// </summary>
        TwoWay,

        /// <summary>
        ///     Only update the target during deserialization.
        /// </summary>
        OneWay,

        /// <summary>
        ///     Only update the source during serialization
        /// </summary>
        OneWayToSource
    }
}