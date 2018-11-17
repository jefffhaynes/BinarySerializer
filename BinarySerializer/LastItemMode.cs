namespace BinarySerialization
{
    /// <summary>
    ///     Used in conjunction with the ItemSerializeUntilAttribute to specify handling of the last item in a collection.
    /// </summary>
    public enum LastItemMode
    {
        /// <summary>
        ///     Include the last item of a collection in the collection.
        /// </summary>
        Include,

        /// <summary>
        ///     Discard the last item of a collection.
        /// </summary>
        Discard,

        /// <summary>
        ///     Defer processing of the underlying data (don't advance stream).
        /// </summary>
        Defer
    }
}