namespace BinarySerialization
{
    public enum BindingMode
    {
        /// <summary>
        /// Update the source during serialization and the target during deserialization.
        /// </summary>
        TwoWay,

        /// <summary>
        /// Only update the target during deserialization.
        /// </summary>
        OneWay
    }
}
