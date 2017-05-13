namespace BinarySerialization
{
    /// <summary>
    ///     Describes the location of the binding source member relative to the position of
    ///     the binding target member.
    /// </summary>
    public enum RelativeSourceMode
    {
        /// <summary>
        ///     Refers to the parent object on which you are setting the binding and allows you
        ///     to bind one member of that object to another member on the same object.
        /// </summary>
        Self,

        /// <summary>
        ///     Refers to the ancestor in the parent chain of the data-bound member.
        ///     You can use this to bind to an ancestor of a specific type or its subclasses.
        ///     This is the mode you use if you want to specify <see cref="FieldBindingBaseAttribute.AncestorType" />
        ///     and/or <see cref="FieldBindingBaseAttribute.AncestorLevel" />.
        /// </summary>
        FindAncestor,

        /// <summary>
        ///     Refers to the object context specified during serialization or deserialization.
        /// </summary>
        SerializationContext
    }
}