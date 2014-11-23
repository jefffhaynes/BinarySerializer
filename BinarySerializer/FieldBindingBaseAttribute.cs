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
		    Binding = new MemberBinding();
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="FieldBindingBaseAttribute"/> 
        /// with a path to the binding source.
        /// </summary>
		protected FieldBindingBaseAttribute(string path)
		{
            Binding = new MemberBinding(path);
		}

        public MemberBinding Binding { get; set; }

        public string Path
        {
            get { return Binding.Path; }
            set { Binding.Path = value; }
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

        public RelativeSourceMode Mode
        {
            get { return Binding.Mode; }
            set { Binding.Mode = value; }
        }

        public Type ConverterType
        {
            get { return Binding.ConverterType; }
            set { Binding.ConverterType = value; }
        }

        internal bool IsConst
        {
            get { return IsConstSupported && string.IsNullOrEmpty(Path); }
        }

        internal abstract bool IsConstSupported { get; }
	}
}

