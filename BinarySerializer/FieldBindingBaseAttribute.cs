namespace BinarySerialization;

/// <summary>
///     The base class used for all attributes that support member binding.
///     <seealso cref="FieldCountAttribute" />
///     <seealso cref="FieldLengthAttribute" />
///     <seealso cref="FieldOffsetAttribute" />
///     <seealso cref="SerializeWhenAttribute" />
/// </summary>
public abstract class FieldBindingBaseAttribute : Attribute, IBindableFieldAttribute
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="FieldBindingBaseAttribute" />.
    /// </summary>
    protected FieldBindingBaseAttribute()
    {
        Binding = new BindingInfo();
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="FieldBindingBaseAttribute" />
    ///     with a path to the binding source.
    /// </summary>
    protected FieldBindingBaseAttribute(string path)
    {
        Binding = new BindingInfo(path);
    }

    internal BindingInfo Binding { get; set; }

    internal virtual bool SupportsDeferredBinding => false;

    /// <summary>
    ///     Gets or sets the path to the binding source property.
    /// </summary>
    public string Path
    {
        get => Binding.Path;
        set => Binding.Path = value;
    }

    /// <summary>
    ///     Gets or sets a value that indicates the direction of the data flow in the binding.
    /// </summary>
    public BindingMode BindingMode
    {
        get => Binding.BindingMode;
        set => Binding.BindingMode = value;
    }

    /// <summary>
    ///     Gets or sets the level of ancestor to look for, in FindAncestor mode.
    ///     Use 1 to indicate the one nearest to the binding target element.
    /// </summary>
    public int AncestorLevel
    {
        get => Binding.AncestorLevel;
        set => Binding.AncestorLevel = value;
    }

    /// <summary>
    ///     Gets or sets the type of ancestor to look for.
    /// </summary>
    public Type AncestorType
    {
        get => Binding.AncestorType;
        set => Binding.AncestorType = value;
    }

    /// <summary>
    ///     Gets or sets a RelativeSourceMode value that describes the location of the binding source relative to the position
    ///     of the binding target.
    /// </summary>
    public RelativeSourceMode RelativeSourceMode
    {
        get => Binding.RelativeSourceMode;
        set => Binding.RelativeSourceMode = value;
    }

    /// <summary>
    ///     Gets or sets the type of converter to use.
    /// </summary>
    public Type ConverterType
    {
        get => Binding.ConverterType;
        set => Binding.ConverterType = value;
    }

    /// <summary>
    ///     Gets or sets the parameter to pass to the Converter.
    /// </summary>
    public object ConverterParameter
    {
        get => Binding.ConverterParameter;
        set => Binding.ConverterParameter = value;
    }
}
