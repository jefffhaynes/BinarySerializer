using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace BinarySerialization.Graph
{
    internal abstract class Node
    {
        private const char PathSeparator = '.';

        private static readonly Encoding DefaultEncoding = Encoding.UTF8;

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

        private readonly Encoding _encoding;
        private readonly Endianness? _endianness;

        private readonly IntegerBinding _fieldCountBinding;
        private readonly IntegerBinding _fieldLengthBinding;
        private readonly IntegerBinding _fieldOffsetBinding;
        private readonly bool _ignore;
        private readonly IntegerBinding _itemLengthBinding;
        //private readonly ObjectBinding _serializeUntilBinding;
        private readonly ObjectBinding _itemSerializeUntilBinding;
        private readonly Lazy<List<Binding>> _lazyBindings;
        private readonly Lazy<List<Node>> _lazyChildren;
        private readonly int? _order;
        private readonly SerializedType? _serializedType;
        private readonly ObjectBinding _subtypeBinding;
        private readonly Type _type;
        private readonly ConditionalBinding[] _whenBindings;

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
                ValueSetter = (obj, value) => propertyInfo.SetValue(obj, value, null);
            }
            else if (fieldInfo != null)
            {
                _type = fieldInfo.FieldType;
                ValueGetter = fieldInfo.GetValue;
                ValueSetter = fieldInfo.SetValue;
            }
            else throw new NotSupportedException(string.Format("{0} not supported", memberInfo.GetType().Name));

            object[] attributes = memberInfo.GetCustomAttributes(true);

            IgnoreAttribute ignoreAttribute = attributes.OfType<IgnoreAttribute>().SingleOrDefault();
            _ignore = ignoreAttribute != null;

            if (_ignore)
                return;

            FieldOrderAttribute fieldOrderAttribute = attributes.OfType<FieldOrderAttribute>().SingleOrDefault();
            if (fieldOrderAttribute != null)
                _order = fieldOrderAttribute.Order;

            SerializeAsAttribute serializeAsAttribute = attributes.OfType<SerializeAsAttribute>().SingleOrDefault();
                if (serializeAsAttribute != null)
                {
                    _serializedType = serializeAsAttribute.SerializedType;
                    _endianness = serializeAsAttribute.Endianness;

                    if (!string.IsNullOrEmpty(serializeAsAttribute.Encoding))
                        _encoding = Encoding.GetEncoding(serializeAsAttribute.Encoding);
                }
          

            FieldLengthAttribute fieldLengthAttribute = attributes.OfType<FieldLengthAttribute>().SingleOrDefault();
            if (fieldLengthAttribute != null)
                _fieldLengthBinding = new IntegerBinding(this, fieldLengthAttribute, () => MeasureNodeOverride());

            FieldCountAttribute fieldCountAttribute = attributes.OfType<FieldCountAttribute>().SingleOrDefault();
            if (fieldCountAttribute != null)
                _fieldCountBinding = new IntegerBinding(this, fieldCountAttribute, () => CountNodeOverride());

            FieldOffsetAttribute fieldOffsetAttribute = attributes.OfType<FieldOffsetAttribute>().SingleOrDefault();
            if (fieldOffsetAttribute != null)
                _fieldOffsetBinding = new IntegerBinding(this, fieldOffsetAttribute);

            SerializeWhenAttribute[] serializeWhenAttributes = attributes.OfType<SerializeWhenAttribute>().ToArray();
            if (serializeWhenAttributes.Length > 0)
            {
                _whenBindings =
                    serializeWhenAttributes.Select(
                        attribute => new ConditionalBinding(this, attribute, null)).ToArray();
            }

            SubtypeAttributes = attributes.OfType<SubtypeAttribute>().ToArray();
            if (SubtypeAttributes.Length != 0)
            {
                List<ObjectBinding> subtypeBindings =
                    SubtypeAttributes.Select(subtypeAttribute => new ObjectBinding(this, SubtypeAttributes[0], () =>
                    {
                        Type valueType = GetValueTypeOverride();

                        if (valueType == null)
                            return null;

                        List<SubtypeAttribute> matchingSubtypes =
                            SubtypeAttributes.Where(attribute => attribute.Subtype == valueType).ToList();

                        if (!matchingSubtypes.Any())
                        {
                            /* Try to fall back on base types */
                            matchingSubtypes =
                                SubtypeAttributes.Where(attribute => attribute.Subtype.IsAssignableFrom(valueType))
                                    .ToList();

                            if (!matchingSubtypes.Any())
                                return null;
                        }

                        if (matchingSubtypes.Count() > 1)
                            throw new BindingException("Subtypes must have unique types.");

                        return matchingSubtypes.Single().Value;
                    })).ToList();


                IEnumerable<IGrouping<BindingInfo, SubtypeAttribute>> bindingGroups =
                    SubtypeAttributes.GroupBy(subtypeAttribute => subtypeAttribute.Binding);

                if (bindingGroups.Count() > 1)
                    throw new BindingException("Subtypes must all use the same binding configuration.");

                _subtypeBinding = subtypeBindings.First();
            }

            SerializeUntilAttribute = attributes.OfType<SerializeUntilAttribute>().SingleOrDefault();
            //if (SerializeUntilAttribute != null)
            //{
            //    _serializeUntilBinding = new ObjectBinding(this, SerializeUntilAttribute,
            //        () => { throw new NotSupportedException("Binding for this attribute not currently supported."); });
            //}

            ItemLengthAttribute = attributes.OfType<ItemLengthAttribute>().SingleOrDefault();

            ItemSerializeUntilAttribute = attributes.OfType<ItemSerializeUntilAttribute>().SingleOrDefault();

            if (ItemSerializeUntilAttribute != null)
            {
                _itemSerializeUntilBinding = new ObjectBinding(this, ItemSerializeUntilAttribute,
                    GetLastItemValueOverride);
            }
        }

        protected Node Parent { get; private set; }

        public string Name { get; private set; }

        public virtual Type Type
        {
            get
            {
                Type underlyingType = Nullable.GetUnderlyingType(_type);
                return underlyingType ?? _type;
            }
        }

        public Action<object, object> ValueSetter { get; private set; }

        public Func<object, object> ValueGetter { get; private set; }

        public virtual object Value { get; set; }

        public virtual object BoundValue
        {
            get { return Value; }
        }

        public virtual IEnumerable<Node> Children
        {
            get { return _lazyChildren.Value; }
        }

        public List<Binding> Bindings
        {
            get { return _lazyBindings.Value; }
        }

        protected int ChildCount
        {
            get { return _lazyChildren.Value.Count; }
        }

        public ItemLengthAttribute ItemLengthAttribute { get; private set; }

        public IntegerBinding FieldLengthBinding
        {
            get { return _fieldLengthBinding; }
        }

        public IntegerBinding FieldCountBinding
        {
            get { return _fieldCountBinding; }
        }

        public IntegerBinding FieldOffsetBinding
        {
            get { return _fieldOffsetBinding; }
        }

        public IntegerBinding ItemLengthBinding
        {
            get { return _itemLengthBinding; }
        }

        public ObjectBinding SubtypeBinding
        {
            get { return _subtypeBinding; }
        }

        public ObjectBinding ItemSerializeUntilBinding
        {
            get { return _itemSerializeUntilBinding; }
        }

        public SerializeUntilAttribute SerializeUntilAttribute { get; private set; }

        public ItemSerializeUntilAttribute ItemSerializeUntilAttribute { get; private set; }

        public SubtypeAttribute[] SubtypeAttributes { get; private set; }

        public virtual Endianness Endianness
        {
            set { throw new NotSupportedException(); }

            get
            {
                if (_endianness != null && _endianness.Value != Endianness.Inherit)
                    return _endianness.Value;

                return Parent.Endianness;
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

        public int? Order
        {
            get { return _order; }
        }

        public bool Ignore
        {
            get { return _ignore; }
        }

        public bool ShouldSerialize
        {
            get
            {
                if (_ignore)
                    return false;

                return _whenBindings == null || _whenBindings.Any(binding => binding.Value);
            }
        }

        /// <summary>
        ///     Occurrs after a member has been serialized.
        /// </summary>
        public event EventHandler<MemberSerializedEventArgs> MemberSerialized;

        /// <summary>
        ///     Occurrs after a member has been deserialized.
        /// </summary>
        public event EventHandler<MemberSerializedEventArgs> MemberDeserialized;

        /// <summary>
        ///     Occurrs before a member has been serialized.
        /// </summary>
        public event EventHandler<MemberSerializingEventArgs> MemberSerializing;

        /// <summary>
        ///     Occurrs before a member has been deserialized.
        /// </summary>
        public event EventHandler<MemberSerializingEventArgs> MemberDeserializing;

        protected virtual long MeasureNodeOverride()
        {
            var nullStream = new NullStream();
            var streamKeeper = new StreamKeeper(nullStream);
            Serialize(streamKeeper);
            return streamKeeper.RelativePosition;
        }

        protected virtual long CountNodeOverride()
        {
            throw new NotSupportedException();
        }

        protected virtual Type GetValueTypeOverride()
        {
            throw new NotSupportedException();
        }

        protected virtual object GetLastItemValueOverride()
        {
            throw new NotSupportedException();
        }

        protected void AddChild(Node child)
        {
            _lazyChildren.Value.Add(child);
            AddEvents(child);
        }

        protected void AddChildren(IEnumerable<Node> children)
        {
            foreach (Node child in children)
                AddChild(child);
        }

        protected void ClearChildren()
        {
            foreach (Node child in Children)
            {
                child.Unbind();
                RemoveEvents(child);
            }

            _lazyChildren.Value.Clear();
        }

        public void Bind()
        {
            if (FieldLengthBinding != null)
                FieldLengthBinding.Bind();

            if (FieldCountBinding != null)
                FieldCountBinding.Bind();

            if (FieldOffsetBinding != null)
                FieldOffsetBinding.Bind();

            if (ItemLengthBinding != null)
                ItemLengthBinding.Bind();

            if (SubtypeBinding != null)
                SubtypeBinding.Bind();

            if (_whenBindings != null)
            {
                foreach (ConditionalBinding conditionalBinding in _whenBindings)
                    conditionalBinding.Bind();
            }

            foreach (Node child in Children)
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

            if (_whenBindings != null)
            {
                foreach (ConditionalBinding conditionalBinding in _whenBindings)
                    conditionalBinding.Unbind();
            }
        }

        public SerializedType GetSerializedType(Type referenceType = null)
        {
            if (referenceType == null)
                referenceType = Type;

            if (_serializedType != null && _serializedType.Value != SerializedType.Default)
                return _serializedType.Value;

            SerializedType serializedType;
            if (DefaultSerializedTypes.TryGetValue(referenceType, out serializedType))
            {
                /* Special cases */
                if (serializedType == SerializedType.NullTerminatedString && FieldLengthBinding != null)
                    serializedType = SerializedType.SizedString;

                if (serializedType == SerializedType.NullTerminatedString && Parent.ItemLengthAttribute != null)
                    serializedType = SerializedType.SizedString;

                return serializedType;
            }

            return SerializedType.Default;
        }

        public virtual void Serialize(Stream stream)
        {
            try
            {
                SerializeOverride(stream);
            }
            catch (IOException)
            {
                throw;
            }
            catch (Exception e)
            {
                string reference = Name == null
                    ? string.Format("graphType '{0}'", _type)
                    : string.Format("member '{0}'", Name);
                string message = string.Format("Error serializing {0}.  See inner exception for detail.", reference);
                throw new InvalidOperationException(message, e);
            }
        }

        public virtual void Deserialize(StreamLimiter stream)
        {
            try
            {
                DeserializeOverride(stream);
            }
            catch (EndOfStreamException e)
            {
                string reference = Name == null
                    ? string.Format("graphType '{0}'", _type)
                    : string.Format("member '{0}'", Name);
                string message = string.Format("Error deserializing '{0}'.  See inner exception for detail.", reference);
                throw new InvalidOperationException(message, e);
            }
            catch (IOException)
            {
                throw;
            }
            catch (Exception e)
            {
                string message = string.Format("Error deserializing {0}.", Name);
                throw new InvalidOperationException(message, e);
            }
        }

        public abstract void SerializeOverride(Stream stream);

        public abstract void DeserializeOverride(StreamLimiter stream);

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

            if (source == null)
                throw new BindingException(string.Format("No ancestor found."));

            return source.GetChild(binding.Path);
        }

        public Node GetChild(string path)
        {
            string[] memberNames = path.Split(PathSeparator);

            if (!memberNames.Any())
                throw new BindingException("Path cannot be empty.");

            Node sourceChild = this;
            foreach (string name in memberNames)
            {
                sourceChild = sourceChild.Children.SingleOrDefault(c => c.Name == name);

                if (sourceChild == null)
                    throw new BindingException(string.Format("No field found at '{0}'.", path));
            }

            return sourceChild;
        }

        private Node FindAncestor(BindingInfo binding)
        {
            int level = 1;
            Node parent = Parent;
            while (parent != null)
            {
                if (binding.AncestorLevel == level || parent.Type == binding.AncestorType)
                {
                    return parent;
                }

                parent = parent.Parent;
                level++;
            }

            return null;
        }

        public virtual BinarySerializationContext CreateSerializationContext()
        {
            if (Parent == null)
                return null;

            return new BinarySerializationContext(Parent.Value, Parent.Type, Parent.CreateSerializationContext());
        }

        protected void AddEvents(Node child)
        {
            RemoveEvents(child);
            child.MemberSerializing += OnMemberSerializing;
            child.MemberSerialized += OnMemberSerialized;
            child.MemberDeserializing += OnMemberDeserializing;
            child.MemberDeserialized += OnMemberDeserialized;
        }

        private void RemoveEvents(Node child)
        {
            child.MemberSerializing -= OnMemberSerializing;
            child.MemberSerialized -= OnMemberSerialized;
            child.MemberDeserializing -= OnMemberDeserializing;
            child.MemberDeserialized -= OnMemberDeserialized;
        }

        protected void OnMemberSerialized(object sender, MemberSerializedEventArgs e)
        {
            EventHandler<MemberSerializedEventArgs> handler = MemberSerialized;
            if (handler != null)
                handler(sender, e);
        }

        protected void OnMemberDeserialized(object sender, MemberSerializedEventArgs e)
        {
            EventHandler<MemberSerializedEventArgs> handler = MemberDeserialized;
            if (handler != null)
                handler(sender, e);
        }

        protected void OnMemberSerializing(object sender, MemberSerializingEventArgs e)
        {
            EventHandler<MemberSerializingEventArgs> handler = MemberSerializing;
            if (handler != null)
                handler(sender, e);
        }

        protected void OnMemberDeserializing(object sender, MemberSerializingEventArgs e)
        {
            EventHandler<MemberSerializingEventArgs> handler = MemberDeserializing;
            if (handler != null)
                handler(sender, e);
        }

        public override string ToString()
        {
            if (Name != null)
                return Name;

            return base.ToString();
        }
    }
}