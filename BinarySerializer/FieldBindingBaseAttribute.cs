using System;

namespace BinarySerialization
{
    /// <summary>
    /// The base class used for all attributes that support member binding.
    /// <seealso cref="FieldCountAttribute"/>
    /// <seealso cref="FieldLengthAttribute"/>
    /// <seealso cref="FieldOffsetAttribute"/>
    /// <seealso cref="SerializeWhenAttribute"/>
    /// </summary>
	public abstract class FieldBindingBaseAttribute : Attribute, IBindableFieldAttribute
	{
        /// <summary>
        /// Initializes a new instance of the <see cref="FieldBindingBaseAttribute"/>.
        /// </summary>
		protected FieldBindingBaseAttribute()
		{
		    Binding = new BindingInfo();
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="FieldBindingBaseAttribute"/> 
        /// with a path to the binding source.
        /// </summary>
		protected FieldBindingBaseAttribute(string path)
		{
            Binding = new BindingInfo(path);
		}

        public BindingInfo Binding { get; set; }

        public string Path
        {
            get { return Binding.Path; }
            set { Binding.Path = value; }
        }

        public BindingMode BindingMode
        {
            get { return Binding.BindingMode; }
            set { Binding.BindingMode = value; }
        }

        public int AncestorLevel
        {
            get { return Binding.AncestorLevel; }
            set { Binding.AncestorLevel = value; }
        }

        public Type AncestorType
        {
            get { return Binding.AncestorType; }
            set { Binding.AncestorType = value; }
        }

        public RelativeSourceMode RelativeSourceMode
        {
            get { return Binding.RelativeSourceMode; }
            set { Binding.RelativeSourceMode = value; }
        }

        public Type ConverterType
        {
            get { return Binding.ConverterType; }
            set { Binding.ConverterType = value; }
        }

        public object ConverterParameter
        {
            get { return Binding.ConverterParameter; }
            set { Binding.ConverterParameter = value; }
        }
	}
}

