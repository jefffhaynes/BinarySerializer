using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace BinarySerialization
{
    internal abstract class Node
    {
        private const char PathSeparator = '.';

        private static readonly Encoding DefaultEncoding = Encoding.UTF8;
        private const Endianness DefaultEndianness = Endianness.Little;

        public static readonly Dictionary<Type, SerializedType> DefaultSerializedTypes =
        new Dictionary<Type, SerializedType>
                {
                    {typeof (bool), SerializedType.Int1},
                    {typeof (sbyte), SerializedType.Int1},
                    {typeof (byte), SerializedType.UInt1},
                    {typeof (char), SerializedType.UInt2},
                    {typeof (short), SerializedType.Int2},
                    {typeof (ushort), SerializedType.UInt2},
                    {typeof (int), SerializedType.Int4},
                    {typeof (uint), SerializedType.UInt4},
                    {typeof (long), SerializedType.Int8},
                    {typeof (ulong), SerializedType.UInt8},
                    {typeof (float), SerializedType.Float4},
                    {typeof (double), SerializedType.Float8},
                    {typeof (string), SerializedType.NullTerminatedString},
                    {typeof (byte[]), SerializedType.ByteArray}
                };

        private readonly Type _type;

        private readonly Lazy<List<Node>> _lazyChildren; 
        private readonly Lazy<List<Binding>> _lazyBindings;


        private readonly SerializedType? _serializedType;
        private readonly Endianness? _endianness;
        private readonly Encoding _encoding;

        private readonly int? _order;

        private readonly bool _ignore;

        private readonly IntegerBinding _fieldLengthBinding;
        private readonly IntegerBinding _fieldCountBinding;
        private readonly IntegerBinding _fieldOffsetBinding;
        private readonly IntegerBinding _itemLengthBinding;
        private readonly ObjectBinding _subtypeBinding;
        private readonly ObjectBinding _itemSerializeUntilBinding;
        private readonly ConditionalAttributeEvaluator _whenEvaluator;
            


        protected Node(Node parent)
        {
            Parent = parent;

            _lazyChildren = new Lazy<List<Node>>();
            _lazyBindings = new Lazy<List<Binding>>();

            if (Parent is CollectionNode && Parent.ItemLengthAttribute != null)
            {
                _itemLengthBinding = new IntegerBinding(Parent, Parent.ItemLengthAttribute,
                    () => MeasureNodeOverride());
            }


            Bind();
        }

        protected Node(Node parent, Type type) : this(parent)
        {
            _type = type;
        }

        protected Node(Node parent, MemberInfo memberInfo) : this(parent)
        {
            if (memberInfo == null)
                return;

            Name = memberInfo.Name;

            var propertyInfo = memberInfo as PropertyInfo;
            var fieldInfo = memberInfo as FieldInfo;

            if (propertyInfo != null)
            {
                _type = propertyInfo.PropertyType;
                ValueGetter = declaringValue => propertyInfo.GetValue(declaringValue, null);
                ValueSetter = propertyInfo.SetValue;
            }
            else if (fieldInfo != null)
            {
                _type = fieldInfo.FieldType;
                ValueGetter = fieldInfo.GetValue;
                ValueSetter = fieldInfo.SetValue;
            }
            else throw new NotSupportedException(string.Format("{0} not supported", memberInfo.GetType().Name));

            var attributes = memberInfo.GetCustomAttributes(true);

            var serializeAsAttribute = attributes.OfType<SerializeAsAttribute>().SingleOrDefault();
            if (serializeAsAttribute != null)
            {
                _serializedType = serializeAsAttribute.SerializedType;
                _endianness = serializeAsAttribute.Endianness;

                if(!string.IsNullOrEmpty(serializeAsAttribute.Encoding))
                _encoding = Encoding.GetEncoding(serializeAsAttribute.Encoding);

                _order = serializeAsAttribute.Order;
            }

            var ignoreAttribute = attributes.OfType<IgnoreAttribute>().SingleOrDefault();
            _ignore = ignoreAttribute != null;
            
            var fieldLengthAttribute = attributes.OfType<FieldLengthAttribute>().SingleOrDefault();
            if (fieldLengthAttribute != null)
                _fieldLengthBinding = new IntegerBinding(this, fieldLengthAttribute, () => MeasureNodeOverride());

            var fieldCountAttribute = attributes.OfType<FieldCountAttribute>().SingleOrDefault();
            if (fieldCountAttribute != null)
                _fieldCountBinding = new IntegerBinding(this, fieldCountAttribute, () => CountNodeOverride());

            var fieldOffsetAttribute = attributes.OfType<FieldOffsetAttribute>().SingleOrDefault();
            if (fieldOffsetAttribute != null)
                _fieldOffsetBinding = new IntegerBinding(this, fieldOffsetAttribute);

            var serializeWhenAttributes = attributes.OfType<SerializeWhenAttribute>().ToArray();
            if (serializeWhenAttributes.Length > 0)
                _whenEvaluator = new ConditionalAttributeEvaluator(this, serializeWhenAttributes);

            SubtypeAttributes = attributes.OfType<SubtypeAttribute>().ToArray();
            if (SubtypeAttributes.Length != 0)
            {
                var subtypeBindings =
                    SubtypeAttributes.Select(subtypeAttribute => new ObjectBinding(this, SubtypeAttributes[0], () =>
                    {
                        var valueType = GetValueTypeOverride();

                        if (valueType == null)
                            return null;

                        var matchingSubtypes =
                            SubtypeAttributes.Where(attribute => attribute.Subtype == valueType).ToList();

                        if (!matchingSubtypes.Any())
                        {
                            /* Try to fall back on base types */
                            matchingSubtypes =
                                SubtypeAttributes.Where(attribute => attribute.Subtype.IsAssignableFrom(valueType)).ToList();

                            if (!matchingSubtypes.Any())
                                throw new BindingException("No matching subtype.");
                        }

                        if(matchingSubtypes.Count() > 1)
                            throw new BindingException("Subtypes must have unique types.");

                        return matchingSubtypes.Single().Value;
                    })).ToList();

                var subtypeBindingSourceGroups = subtypeBindings.GroupBy(subtypeBinding => subtypeBinding.Source);

                if(subtypeBindingSourceGroups.Count() > 1)
                    throw new BindingException("Subtypes must all bind to a single source.");

                var subtypePathGroups = SubtypeAttributes.GroupBy(subtypeAttribute => subtypeAttribute.Path);

                if(subtypePathGroups.Count() > 1)
                    throw new BindingException("Subtypes must all bind to the same path.");

                _subtypeBinding = subtypeBindings.First();
            }

            //node.SerializeUntilAttribute = attributes.OfType<SerializeUntilAttribute>().SingleOrDefault();
            ItemLengthAttribute = attributes.OfType<ItemLengthAttribute>().SingleOrDefault();

            ItemSerializeUntilAttribute = attributes.OfType<ItemSerializeUntilAttribute>().SingleOrDefault();

            if (ItemSerializeUntilAttribute != null)
            {
                _itemSerializeUntilBinding = new ObjectBinding(this, ItemSerializeUntilAttribute, GetLastItemValueOverride);
            }
        }

        protected virtual long MeasureNodeOverride()
        {
            var nullStream = new NullStream();
            var streamKeeper = new StreamKeeper(nullStream);
            Serialize(streamKeeper);
            return streamKeeper.RelativePosition;
        }

        protected virtual long CountNodeOverride()
        {
            throw new NotImplementedException();
        }

        protected virtual Type GetValueTypeOverride()
        {
            throw new NotImplementedException();
        }

        protected virtual object GetLastItemValueOverride()
        {
            throw new NotImplementedException();
        }

        protected Node Parent { get; private set; }

        public string Name { get; private set; }

        public Type Type
        {
            get
            {
                var underlyingType = Nullable.GetUnderlyingType(_type);
                return underlyingType ?? _type;
            }
        }

        public Action<object, object> ValueSetter { get; private set; }

        public Func<object, object> ValueGetter { get; private set; }

        public virtual object Value { get; set; }

        public virtual object BoundValue { get { return Value; } }

        public List<Node> Children { get { return _lazyChildren.Value; } }

        public List<Binding> Bindings { get { return _lazyBindings.Value; } }

        public void Bind()
        {
            if(FieldLengthBinding != null)
                FieldLengthBinding.Bind();

            if(FieldCountBinding != null)
                FieldCountBinding.Bind();

            if(FieldOffsetBinding != null)
                FieldOffsetBinding.Bind();
            
            if(ItemLengthBinding != null)
                ItemLengthBinding.Bind();
            
            if(SubtypeBinding != null)
                SubtypeBinding.Bind();

            foreach(var child in Children)
                child.Bind();
        }

        public void Unbind()
        {
            if (FieldLengthBinding != null)
                FieldLengthBinding.Unbind();

            if (FieldCountBinding != null)
                FieldCountBinding.Unbind();

            if (FieldOffsetBinding != null)
                FieldOffsetBinding.Unbind();

            if (ItemLengthBinding != null)
                ItemLengthBinding.Unbind();

            if (SubtypeBinding != null)
                SubtypeBinding.Unbind();
        }

        public ItemLengthAttribute ItemLengthAttribute { get; private set; }

        public IntegerBinding FieldLengthBinding { get { return _fieldLengthBinding; } }

        public IntegerBinding FieldCountBinding { get { return _fieldCountBinding; } }

        public IntegerBinding FieldOffsetBinding { get { return _fieldOffsetBinding; } }

        public IntegerBinding ItemLengthBinding { get { return _itemLengthBinding; } }

        public ObjectBinding SubtypeBinding { get { return _subtypeBinding; } }

        public ObjectBinding ItemSerializeUntilBinding { get { return _itemSerializeUntilBinding; } }

        public SerializeUntilAttribute SerializeUntilAttribute { get; set; }
        public ItemSerializeUntilAttribute ItemSerializeUntilAttribute { get; set; }
        public SubtypeAttribute[] SubtypeAttributes { get; set; }

        public SerializedType SerializedType
        {
            get
            {
                if (_serializedType != null && _serializedType.Value != SerializedType.Default)
                    return _serializedType.Value;

                SerializedType serializedType;
                if (DefaultSerializedTypes.TryGetValue(Type, out serializedType))
                {
                    /* Special cases */
                    if(serializedType == SerializedType.NullTerminatedString && FieldLengthBinding != null)
                        serializedType = SerializedType.SizedString;

                    if(serializedType == SerializedType.NullTerminatedString && Parent.ItemLengthAttribute != null)
                        serializedType = SerializedType.SizedString;

                    return serializedType;
                }

                return SerializedType.Default;
            }
        }

        public Endianness Endianness
        {
            get
            {
                if (_endianness != null && _endianness.Value != Endianness.Inherit)
                    return _endianness.Value;

                return Parent != null ? Parent.Endianness : DefaultEndianness;
            }
        }

        public Encoding Encoding
        {
            get
            {
                if (_encoding != null)
                    return _encoding;

                return Parent != null ? Parent.Encoding : DefaultEncoding;
            }
        }

        public int Order
        {
            get { return _order ?? 0; }
        }

        public bool ShouldSerialize
        {
            get
            {
                if (_ignore)
                    return false;

                return _whenEvaluator == null || _whenEvaluator.Value;
            }
        }

        public abstract void Serialize(Stream stream);

        public abstract void Deserialize(StreamLimiter stream);

        public Node GetBindingSource(BindingInfo binding)
        {
            Node source = null;

            switch (binding.Mode)
            {
                case RelativeSourceMode.Self:
                    source = Parent;
                    break;
                case RelativeSourceMode.FindAncestor:
                    source = FindAncestor(binding);
                    break;
                case RelativeSourceMode.PreviousData:
                    throw new NotImplementedException();
            }

            if(source == null)
                throw new BindingException(string.Format("No ancestor found."));

            /* Get various members along path */
            string[] memberNames = binding.Path.Split(PathSeparator);

            if (!memberNames.Any())
                throw new BindingException("Path cannot be empty.");

            return source.GetChild(binding.Path);
        }

        public Node GetChild(string path)
        {
            string[] memberNames = path.Split(PathSeparator);

            if (!memberNames.Any())
                throw new BindingException("Path cannot be empty.");

            Node sourceChild = this;
            foreach (var name in memberNames)
            {
                sourceChild = sourceChild.Children.SingleOrDefault(c => c.Name == name);

                if (sourceChild == null)
                    throw new BindingException(string.Format("No member found at '{0}'.", path));
            }

            return sourceChild;
        }

        private Node FindAncestor(BindingInfo binding)
        {
            int level = 1;
            var parent = Parent;
            while (parent != null)
            {
                if (binding.AncestorLevel == level || parent._type == binding.AncestorType)
                {
                    return Parent;
                }

                parent = parent.Parent;
                level++;
            }

            return null;
        }

        public override string ToString()
        {
            if (Name != null)
                return Name;

            return base.ToString();
        }
    }
}
