namespace BinarySerialization
{
    /// <summary>
    ///     Used to specify the field alignment mode for the FieldAlignment attribute.
    /// </summary>
    public enum FieldAlignmentMode
    {
        /// <summary>
        ///     Align both left and right sides of the field.
        /// </summary>
        LeftAndRight,

        /// <summary>
        ///     Align only left side of the field.
        /// </summary>
        LeftOnly,

        /// <summary>
        ///     Align only right side of the field.
        /// </summary>
        RightOnly
    }
}