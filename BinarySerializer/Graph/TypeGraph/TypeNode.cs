using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using BinarySerialization.Graph.ValueGraph;

namespace BinarySerialization.Graph.TypeGraph
{
    internal abstract class TypeNode : Node
    {
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

        private readonly int? _order;
        private readonly SerializedType? _serializedType;

        protected TypeNode(TypeNode parent)
            : base(parent)
        {
        }

        protected TypeNode(TypeNode parent, Type type)
            : this(parent)
        {
            Type = type;
            NullableUnderlyingType = Nullable.GetUnderlyingType(Type);
        }

        protected TypeNode(TypeNode parent, Type parentType, MemberInfo memberInfo)
            : this(parent)
        {
            if (memberInfo == null)
                return;

            Name = memberInfo.Name;

            var propertyInfo = memberInfo as PropertyInfo;
            var fieldInfo = memberInfo as FieldInfo;

            if (propertyInfo != null)
            {
                Type = propertyInfo.PropertyType;

                ValueGetter = MagicMethods.MagicFunc(parentType, propertyInfo.GetGetMethod());

                var setMethod = propertyInfo.GetSetMethod();

                if(setMethod != null)
                    ValueSetter = MagicMethods.MagicAction(parentType, setMethod);
            }
            else if (fieldInfo != null)
            {
                Type = fieldInfo.FieldType;

                ValueGetter = fieldInfo.GetValue;
                ValueSetter = fieldInfo.SetValue;
            }
            else throw new NotSupportedException(String.Format("{0} not supported", memberInfo.GetType().Name));

            NullableUnderlyingType = Nullable.GetUnderlyingType(Type);

            var attributes = memberInfo.GetCustomAttributes(true);

            IgnoreAttribute = attributes.OfType<IgnoreAttribute>().SingleOrDefault();

            /* Don't go any further if we're ignoring this. */
            if (IgnoreAttribute != null)
                return;

            var fieldOrderAttribute = attributes.OfType<FieldOrderAttribute>().SingleOrDefault();
            if (fieldOrderAttribute != null)
                _order = fieldOrderAttribute.Order;

            var serializeAsAttribute = attributes.OfType<SerializeAsAttribute>().SingleOrDefault();
            if (serializeAsAttribute != null)
            {
                _serializedType = serializeAsAttribute.SerializedType;
                Endianness = serializeAsAttribute.Endianness;

                if (!String.IsNullOrEmpty(serializeAsAttribute.Encoding))
                    Encoding = Encoding.GetEncoding(serializeAsAttribute.Encoding);
            }

            FieldLengthAttribute = attributes.OfType<FieldLengthAttribute>().SingleOrDefault();
            if (FieldLengthAttribute != null)
            {
                FieldLengthBinding = new Binding(FieldLengthAttribute, GetBindingLevel(FieldLengthAttribute.Binding));
            }

            FieldCountAttribute = attributes.OfType<FieldCountAttribute>().SingleOrDefault();
            if (FieldCountAttribute != null)
            {
                FieldCountBinding = new Binding(FieldCountAttribute, FindAncestorLevel(FieldCountAttribute.Binding));
            }

            FieldOffsetAttribute = attributes.OfType<FieldOffsetAttribute>().SingleOrDefault();
            if (FieldOffsetAttribute != null)
            {
                FieldOffsetBinding = new Binding(FieldOffsetAttribute, GetBindingLevel(FieldOffsetAttribute.Binding));
            }

            var serializeWhenAttributes = attributes.OfType<SerializeWhenAttribute>().ToArray();
            SerializeWhenAttributes = new ReadOnlyCollection<SerializeWhenAttribute>(serializeWhenAttributes);

            if (SerializeWhenAttributes.Count > 0)
            {
                SerializeWhenBindings = new ReadOnlyCollection<ConditionalBinding>(
                    serializeWhenAttributes.Select(
                        attribute => new ConditionalBinding(attribute, GetBindingLevel(attribute.Binding))).ToList());
            }

            var subtypeAttributes = attributes.OfType<SubtypeAttribute>().ToArray();
            SubtypeAttributes = new ReadOnlyCollection<SubtypeAttribute>(subtypeAttributes);

            if (SubtypeAttributes.Count > 0)
            {
                var bindingGroups =
                    SubtypeAttributes.GroupBy(subtypeAttribute => subtypeAttribute.Binding);

                if (bindingGroups.Count() > 1)
                    throw new BindingException("Subtypes must all use the same binding configuration.");

                var firstBinding = SubtypeAttributes[0];
                SubtypeBinding = new Binding(firstBinding, GetBindingLevel(firstBinding.Binding));

                var valueGroups = SubtypeAttributes.GroupBy(attribute => attribute.Value);
                if (valueGroups.Count() < SubtypeAttributes.Count)
                    throw new InvalidOperationException("Subtype values must be unique.");

                if (SubtypeBinding.BindingMode == BindingMode.TwoWay)
                {
                    var subTypeGroups = SubtypeAttributes.GroupBy(attribute => attribute.Subtype);
                    if (subTypeGroups.Count() < SubtypeAttributes.Count)
                        throw new InvalidOperationException(
                            "Subtypes must be unique for two-way subtype bindings.  Set BindingMode to OneWay to disable updates to the binding source during serialization.");
                }

                var invalidSubtype =
                    SubtypeAttributes.FirstOrDefault(attribute => !Type.IsAssignableFrom(attribute.Subtype));

                if (invalidSubtype != null)
                    throw new InvalidOperationException(String.Format("{0} is not a subtype of {1}",
                        invalidSubtype.Subtype, Type));
            }


            SerializeUntilAttribute = attributes.OfType<SerializeUntilAttribute>().SingleOrDefault();
            if (SerializeUntilAttribute != null)
            {
                SerializeUntilBinding = new Binding(SerializeUntilAttribute,
                    GetBindingLevel(SerializeUntilAttribute.Binding));
            }

            ItemLengthAttribute = attributes.OfType<ItemLengthAttribute>().SingleOrDefault();
            if (ItemLengthAttribute != null)
            {
                ItemLengthBinding = new Binding(ItemLengthAttribute, GetBindingLevel(ItemLengthAttribute.Binding));
            }

            ItemSerializeUntilAttribute = attributes.OfType<ItemSerializeUntilAttribute>().SingleOrDefault();

            if (ItemSerializeUntilAttribute != null)
            {
                ItemSerializeUntilBinding = new Binding(ItemSerializeUntilAttribute,
                    GetBindingLevel(ItemSerializeUntilAttribute.Binding));
            }
        }

        public Type Type { get; private set; }

        public Type NullableUnderlyingType { get; private set; }

        public Type BaseSerializedType { get { return NullableUnderlyingType ?? Type; } }

        public Action<object, object> ValueSetter { get; private set; }

        public Func<object, object> ValueGetter { get; private set; }

        public Binding FieldLengthBinding { get; private set; }
        public Binding ItemLengthBinding { get; private set; }
        public Binding FieldCountBinding { get; private set; }
        public Binding FieldOffsetBinding { get; private set; }
        public Binding SerializeUntilBinding { get; private set; }
        public Binding ItemSerializeUntilBinding { get; private set; }
        public Binding SubtypeBinding { get; private set; }
        public ReadOnlyCollection<ConditionalBinding> SerializeWhenBindings { get; private set; }
        public IgnoreAttribute IgnoreAttribute { get; private set; }
        public FieldLengthAttribute FieldLengthAttribute { get; private set; }
        public FieldCountAttribute FieldCountAttribute { get; private set; }
        public FieldOffsetAttribute FieldOffsetAttribute { get; private set; }
        public ItemLengthAttribute ItemLengthAttribute { get; private set; }
        public ReadOnlyCollection<SubtypeAttribute> SubtypeAttributes { get; private set; }
        public ReadOnlyCollection<SerializeWhenAttribute> SerializeWhenAttributes { get; private set; }
        public SerializeUntilAttribute SerializeUntilAttribute { get; private set; }
        public ItemSerializeUntilAttribute ItemSerializeUntilAttribute { get; private set; }
        public Endianness? Endianness { get; private set; }
        public Encoding Encoding { get; private set; }

        public int? Order
        {
            get { return _order; }
        }

        public SerializedType GetSerializedType(Type referenceType = null)
        {
            if (referenceType == null)
                referenceType = BaseSerializedType;

            if (_serializedType != null && _serializedType.Value != SerializedType.Default)
                return _serializedType.Value;

            SerializedType serializedType;
            if (DefaultSerializedTypes.TryGetValue(referenceType, out serializedType))
            {
                /* Special cases */
                if (serializedType == SerializedType.NullTerminatedString && FieldLengthAttribute != null)
                    serializedType = SerializedType.SizedString;

                var parent = (TypeNode) Parent;
                if (serializedType == SerializedType.NullTerminatedString && parent.ItemLengthAttribute != null)
                    serializedType = SerializedType.SizedString;

                return serializedType;
            }

            return SerializedType.Default;
        }

        public ValueNode CreateSerializer(ValueNode parent)
        {
            try
            {
                return CreateSerializerOverride(parent);
            }
            catch (Exception e)
            {
                var reference = Name == null
                    ? String.Format("type '{0}'", Type)
                    : String.Format("member '{0}'", Name);
                var message = String.Format("Error serializing {0}.  See inner exception for detail.", reference);
                throw new InvalidOperationException(message, e);
            }
        }

        public abstract ValueNode CreateSerializerOverride(ValueNode parent);

        public int GetBindingLevel(BindingInfo binding)
        {
            var level = 0;

            switch (binding.RelativeSourceMode)
            {
                case RelativeSourceMode.Self:
                    level = 1;
                    break;
                case RelativeSourceMode.FindAncestor:
                    level = FindAncestorLevel(binding);
                    break;
                case RelativeSourceMode.PreviousData:
                    throw new NotImplementedException();
                case RelativeSourceMode.SerializationContext:
                    level = FindAncestorLevel(null);
                    break;
            }

            return level;
        }

        public static bool IsValueType(Type type)
        {
            return type.IsPrimitive || type == typeof(string) || type == typeof(byte[]);
            //return type.IsPrimitive || type == typeof(string);
        }

        protected Func<object> CreateCompiledConstructor()
        {
            return CreateCompiledConstructor(Type);
        }

        protected static Func<object> CreateCompiledConstructor(Type type)
        {
            if (type == typeof (string))
                return () => String.Empty;

            var constructor = type.GetConstructor(new Type[0]);
            return CreateCompiledConstructor(constructor);
        }

        protected static Func<object> CreateCompiledConstructor(ConstructorInfo constructor)
        {
            if (constructor == null)
                return null;

            return Expression.Lambda<Func<object>>(Expression.New(constructor)).Compile();
        }

        private int FindAncestorLevel(BindingInfo binding)
        {
            var level = 1;
            var parent = (TypeNode) Parent;
            while (parent != null)
            {
                if (binding != null)
                {
                    if (binding.AncestorLevel == level || parent.Type == binding.AncestorType)
                    {
                        return level;
                    }
                }

                parent = (TypeNode) parent.Parent;
                level++;
            }

            return level;
        }
    }
}