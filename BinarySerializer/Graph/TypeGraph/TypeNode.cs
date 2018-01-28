using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using BinarySerialization.Graph.ValueGraph;

namespace BinarySerialization.Graph.TypeGraph
{
    public abstract class TypeNode : Node<TypeNode>
    {
        public static readonly Dictionary<Type, SerializedType> DefaultSerializedTypes =
            new Dictionary<Type, SerializedType>
            {
                {typeof(bool), SerializedType.Int1},
                {typeof(sbyte), SerializedType.Int1},
                {typeof(byte), SerializedType.UInt1},
                {typeof(char), SerializedType.UInt2},
                {typeof(short), SerializedType.Int2},
                {typeof(ushort), SerializedType.UInt2},
                {typeof(int), SerializedType.Int4},
                {typeof(uint), SerializedType.UInt4},
                {typeof(long), SerializedType.Int8},
                {typeof(ulong), SerializedType.UInt8},
                {typeof(float), SerializedType.Float4},
                {typeof(double), SerializedType.Float8},
                {typeof(string), SerializedType.TerminatedString},
                {typeof(byte[]), SerializedType.ByteArray}
            };

        public static readonly Dictionary<SerializedType, object> SerializedTypeDefault =
            new Dictionary<SerializedType, object>
            {
                {SerializedType.Default, null},
                {SerializedType.Int1, default(sbyte)},
                {SerializedType.UInt1, default(byte)},
                {SerializedType.Int2, default(short)},
                {SerializedType.UInt2, default(ushort)},
                {SerializedType.Int4, default(int)},
                {SerializedType.UInt4, default(uint)},
                {SerializedType.Int8, default(long)},
                {SerializedType.UInt8, default(ulong)},
                {SerializedType.Float4, default(float)},
                {SerializedType.Float8, default(double)},
                {SerializedType.TerminatedString, default(string)},
                {SerializedType.SizedString, default(string)},
                {SerializedType.LengthPrefixedString, default(string)},
                {SerializedType.ByteArray, default(byte[])}
            };

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

        protected TypeNode(TypeNode parent, Type parentType, MemberInfo memberInfo, Type subType = null)
            : this(parent)
        {
            if (memberInfo == null)
            {
                Type = subType;
                return;
            }

            MemberInfo = memberInfo;

            Name = memberInfo.Name;

            var propertyInfo = memberInfo as PropertyInfo;
            var fieldInfo = memberInfo as FieldInfo;
            
            if (propertyInfo != null)
            {
                Type = subType ?? propertyInfo.PropertyType;

                var indexParameters = propertyInfo.GetIndexParameters();

                // ignore custom indexers
                if (indexParameters.Length == 0)
                {
                    var getMethod = propertyInfo.GetGetMethod();
                    ValueGetter = MagicMethods.MagicFunc(parentType, getMethod);
                    
                    var setMethod = propertyInfo.GetSetMethod();

                    if (setMethod != null)
                    {
                        ValueSetter = MagicMethods.MagicAction(parentType, setMethod);
                    }
                }
            }
            else if (fieldInfo != null)
            {
                Type = subType ?? fieldInfo.FieldType;

                ValueGetter = fieldInfo.GetValue;
                ValueSetter = fieldInfo.SetValue;
            }
            else
            {
                throw new NotSupportedException($"{memberInfo.GetType().Name} not supported");
            }

            NullableUnderlyingType = Nullable.GetUnderlyingType(Type);

            var attributes = memberInfo.GetCustomAttributes(true).ToList();

            IsIgnored = attributes.OfType<IgnoreAttribute>().Any();

            /* Don't go any further if we're ignoring this. */
            if (IsIgnored)
            {
                return;
            }

            var fieldOrderAttribute = attributes.OfType<FieldOrderAttribute>().SingleOrDefault();
            if (fieldOrderAttribute != null)
            {
                Order = fieldOrderAttribute.Order;
            }

            var serializeAsAttribute = attributes.OfType<SerializeAsAttribute>().SingleOrDefault();
            if (serializeAsAttribute != null)
            {
                _serializedType = serializeAsAttribute.SerializedType;

#pragma warning disable 618
                if (_serializedType == SerializedType.NullTerminatedString)
                {
                    _serializedType = SerializedType.TerminatedString;
                }
#pragma warning restore 618

                if (_serializedType.Value == SerializedType.TerminatedString)
                {
                    AreStringsTerminated = true;
                    StringTerminator = serializeAsAttribute.StringTerminator;
                }
            }

            IsNullable = NullableUnderlyingType != null;
            if (!IsNullable)
            {
                var serializedType = GetSerializedType();
                IsNullable = serializedType == SerializedType.Default ||
                             serializedType == SerializedType.ByteArray ||
                             serializedType == SerializedType.TerminatedString ||
                             serializedType == SerializedType.SizedString;
            }

            // setup bindings
            FieldLengthBindings = GetBindings<FieldLengthAttribute>(attributes);
            FieldCountBindings = GetBindings<FieldCountAttribute>(attributes);
            FieldOffsetBindings = GetBindings<FieldOffsetAttribute>(attributes);
            FieldScaleBindings = GetBindings<FieldScaleAttribute>(attributes);
            FieldEndiannessBindings = GetBindings<FieldEndiannessAttribute>(attributes);
            FieldEncodingBindings = GetBindings<FieldEncodingAttribute>(attributes);

            var fieldAlignmentAttributes = attributes.OfType<FieldAlignmentAttribute>()
                .ToLookup(attribute => attribute.Mode);
            var leftAlignmentAttributes =
                fieldAlignmentAttributes[FieldAlignmentMode.LeftAndRight].Concat(
                    fieldAlignmentAttributes[FieldAlignmentMode.LeftOnly]);

            var rightAlignmentAttributes =
                fieldAlignmentAttributes[FieldAlignmentMode.LeftAndRight].Concat(
                    fieldAlignmentAttributes[FieldAlignmentMode.RightOnly]);

            LeftFieldAlignmentBindings =
                GetBindings<FieldAlignmentAttribute>(leftAlignmentAttributes.Cast<object>().ToArray());
            RightFieldAlignmentBindings =
                GetBindings<FieldAlignmentAttribute>(rightAlignmentAttributes.Cast<object>().ToArray());

            var fieldValueAttributes = attributes.OfType<FieldValueAttributeBase>().ToArray();
            FieldValueAttributes = new ReadOnlyCollection<FieldValueAttributeBase>(fieldValueAttributes);

            if (FieldValueAttributes.Count > 0)
            {
                FieldValueBindings = GetBindings<FieldValueAttributeBase>(attributes);
            }

            var serializeWhenAttributes = attributes.OfType<SerializeWhenAttribute>().ToArray();
            SerializeWhenAttributes = new ReadOnlyCollection<SerializeWhenAttribute>(serializeWhenAttributes);

            if (SerializeWhenAttributes.Count > 0)
            {
                SerializeWhenBindings = new ReadOnlyCollection<ConditionalBinding>(
                    serializeWhenAttributes.Select(
                        attribute => new ConditionalBinding(attribute, GetBindingLevel(attribute.Binding))).ToList());
            }

            var serializeWhenNotAttributes = attributes.OfType<SerializeWhenNotAttribute>().ToArray();
            SerializeWhenNotAttributes = new ReadOnlyCollection<SerializeWhenNotAttribute>(serializeWhenNotAttributes);

            if (SerializeWhenNotAttributes.Count > 0)
            {
                SerializeWhenNotBindings = new ReadOnlyCollection<ConditionalBinding>(
                    serializeWhenNotAttributes.Select(
                        attribute => new ConditionalBinding(attribute, GetBindingLevel(attribute.Binding))).ToList());
            }

            // don't inherit subtypes if this is itself a subtype
            if (subType == null)
            {
                var subtypeAttributes = attributes.OfType<SubtypeAttribute>().Cast<SubtypeBaseAttribute>().ToArray();

                SubtypeAttributes = new ReadOnlyCollection<SubtypeBaseAttribute>(subtypeAttributes);
                SubtypeBindings = GetBindings(subtypeAttributes, Type);
                
                SubtypeDefaultAttribute = attributes.OfType<SubtypeDefaultAttribute>().SingleOrDefault();

                if (SubtypeDefaultAttribute != null)
                {
                    if (!Type.IsAssignableFrom(SubtypeDefaultAttribute.Subtype))
                    {
                        throw new InvalidOperationException(
                            $"{SubtypeDefaultAttribute.Subtype} is not a subtype of {Type}");
                    }
                }

                var subtypeFactoryAttribute = attributes.OfType<SubtypeFactoryAttribute>().SingleOrDefault();
                if (subtypeFactoryAttribute != null)
                {
                    SubtypeFactoryBinding = GetBinding(subtypeFactoryAttribute);
                    SubtypeFactory =
                        (ISubtypeFactory) subtypeFactoryAttribute.FactoryType.GetConstructor(new Type[0]).Invoke(null);
                }
            }

            var itemSubtypeAttributes = attributes.OfType<ItemSubtypeAttribute>().Cast<SubtypeBaseAttribute>()
                .ToArray();
            ItemSubtypeAttributes = new ReadOnlyCollection<SubtypeBaseAttribute>(itemSubtypeAttributes);

            if (itemSubtypeAttributes.Length > 0)
            {
                Type itemBaseType;

                if (Type.IsArray)
                {
                    itemBaseType = Type.GetElementType();
                }
                else if (typeof(IList).IsAssignableFrom(Type))
                {
                    var genericArguments = Type.GetGenericArguments();
                    if (genericArguments.Length > 1)
                    {
                        throw new InvalidOperationException("Multiple generic arguments not supported.");
                    }

                    itemBaseType = genericArguments[0];
                }
                else
                {
                    throw new InvalidOperationException("ItemSubtype can only be used with collections.");
                }

                ItemSubtypeBindings = GetBindings(itemSubtypeAttributes, itemBaseType);
            }

            var itemSubtypeFactoryAttribute = attributes.OfType<ItemSubtypeFactoryAttribute>().SingleOrDefault();
            if (itemSubtypeFactoryAttribute != null)
            {
                ItemSubtypeFactoryBinding = GetBinding(itemSubtypeFactoryAttribute);
                ItemSubtypeFactory =
                    (ISubtypeFactory) itemSubtypeFactoryAttribute.FactoryType.GetConstructor(new Type[0]).Invoke(null);
            }

            ItemSubtypeDefaultAttribute = attributes.OfType<ItemSubtypeDefaultAttribute>().SingleOrDefault();

            SerializeUntilAttribute = attributes.OfType<SerializeUntilAttribute>().SingleOrDefault();
            if (SerializeUntilAttribute != null)
            {
                SerializeUntilBinding = GetBinding(SerializeUntilAttribute);
            }

            ItemLengthBindings = GetBindings<ItemLengthAttribute>(attributes);

            ItemSerializeUntilAttribute = attributes.OfType<ItemSerializeUntilAttribute>().SingleOrDefault();

            if (ItemSerializeUntilAttribute != null)
            {
                ItemSerializeUntilBinding = GetBinding(ItemSerializeUntilAttribute);
            }
        }


        public MemberInfo MemberInfo { get; }
        public Type Type { get; }
        public Type NullableUnderlyingType { get; }

        public Type BaseSerializedType => NullableUnderlyingType ?? Type;

        public Action<object, object> ValueSetter { get; }
        public Func<object, object> ValueGetter { get; }

        public BindingCollection FieldLengthBindings { get; }
        public BindingCollection ItemLengthBindings { get; }
        public BindingCollection FieldCountBindings { get; }
        public BindingCollection FieldOffsetBindings { get; }
        public BindingCollection FieldScaleBindings { get; }
        public BindingCollection LeftFieldAlignmentBindings { get; }
        public BindingCollection RightFieldAlignmentBindings { get; }
        public BindingCollection FieldEndiannessBindings { get; }
        public BindingCollection FieldEncodingBindings { get; }
        public BindingCollection FieldValueBindings { get; }

        public Binding SerializeUntilBinding { get; }
        public Binding ItemSerializeUntilBinding { get; }
        public BindingCollection SubtypeBindings { get; }
        public BindingCollection ItemSubtypeBindings { get; }
        public Binding SubtypeFactoryBinding { get; }
        public Binding ItemSubtypeFactoryBinding { get; }

        public ReadOnlyCollection<ConditionalBinding> SerializeWhenBindings { get; }
        public ReadOnlyCollection<ConditionalBinding> SerializeWhenNotBindings { get; }
        public ReadOnlyCollection<FieldValueAttributeBase> FieldValueAttributes { get; }
        public ReadOnlyCollection<SubtypeBaseAttribute> SubtypeAttributes { get; }
        public SubtypeDefaultAttribute SubtypeDefaultAttribute { get; }
        public ISubtypeFactory SubtypeFactory { get; }
        public ISubtypeFactory ItemSubtypeFactory { get; }
        public ReadOnlyCollection<SubtypeBaseAttribute> ItemSubtypeAttributes { get; }
        public ItemSubtypeDefaultAttribute ItemSubtypeDefaultAttribute { get; }
        public ReadOnlyCollection<SerializeWhenAttribute> SerializeWhenAttributes { get; }
        public ReadOnlyCollection<SerializeWhenNotAttribute> SerializeWhenNotAttributes { get; }
        public SerializeUntilAttribute SerializeUntilAttribute { get; }
        public ItemSerializeUntilAttribute ItemSerializeUntilAttribute { get; }

        public bool IsIgnored { get; }

        public int? Order { get; }

        public bool AreStringsTerminated { get; }

        public byte StringTerminator { get; }

        public bool IsNullable { get; }

        public SerializedType GetSerializedType(Type referenceType = null)
        {
            if (referenceType == null)
            {
                referenceType = BaseSerializedType;
            }

            SerializedType serializedType;
            if (_serializedType != null && _serializedType.Value != SerializedType.Default)
            {
                serializedType = _serializedType.Value;
            }
            else if (!DefaultSerializedTypes.TryGetValue(referenceType, out serializedType))
            {
                return SerializedType.Default;
            }

            // handle special cases within null terminated strings
            if (serializedType == SerializedType.TerminatedString)
            {
                // If null terminated string is specified but field length is present, override
                if (FieldLengthBindings != null)
                {
                    serializedType = SerializedType.SizedString;
                }

                // If null terminated string is specified but item field length is present, override
                var parent = Parent;
                if (parent.ItemLengthBindings != null)
                {
                    serializedType = SerializedType.SizedString;
                }
            }

            return serializedType;
        }

        public static object GetDefaultValue(SerializedType serializedType)
        {
            return SerializedTypeDefault.TryGetValue(serializedType, out var value) ? value : null;
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
                    ? $"type '{Type}'"
                    : $"member '{Name}'";
                var message = $"Error serializing {reference}.  See inner exception for detail.";
                throw new InvalidOperationException(message, e);
            }
        }

        internal abstract ValueNode CreateSerializerOverride(ValueNode parent);

        public int GetBindingLevel(BindingInfo binding)
        {
            var level = 0;

            switch (binding.RelativeSourceMode)
            {
                case RelativeSourceMode.Self:
                    level = 1;
                    break;
                case RelativeSourceMode.FindAncestor:
                case RelativeSourceMode.SerializationContext:
                    level = FindAncestorLevel(binding);
                    break;
            }

            return level;
        }

        public static bool IsValueType(Type type)
        {
            return type.GetTypeInfo().IsPrimitive || type == typeof(string) || type == typeof(byte[]);
        }

        protected Func<object> CreateCompiledConstructor()
        {
            return CreateCompiledConstructor(Type);
        }

        protected static Func<object> CreateCompiledConstructor(Type type)
        {
            if (type == typeof(string))
            {
                return () => string.Empty;
            }

            var constructor = type.GetConstructor(new Type[0]);
            return CreateCompiledConstructor(constructor);
        }

        protected static Func<object> CreateCompiledConstructor(ConstructorInfo constructor)
        {
            return constructor == null ? null : Expression.Lambda<Func<object>>(Expression.New(constructor)).Compile();
        }

        private Binding GetBinding(FieldBindingBaseAttribute attribute)
        {
            return new Binding(attribute, GetBindingLevel(attribute.Binding));
        }

        private BindingCollection GetBindings<TAttribute>(IEnumerable<object> attributes)
            where TAttribute : FieldBindingBaseAttribute
        {
            var typeAttributes = attributes.OfType<TAttribute>().ToList();

            if (!typeAttributes.Any())
            {
                return null;
            }

            var bindings =
                typeAttributes.Select(
                    attribute =>
                        new Binding(attribute, GetBindingLevel(attribute.Binding)));

            return new BindingCollection(bindings);
        }

        private BindingCollection GetBindings(SubtypeBaseAttribute[] attributes, Type checkType)
        {
            if (attributes.Length == 0)
            {
                return null;
            }

            var bindingGroups =
                attributes.GroupBy(subtypeAttribute => subtypeAttribute.Binding);

            if (bindingGroups.Count() > 1)
            {
                throw new BindingException("Subtypes must all specify the same binding configuration.");
            }

            var bindings =
                attributes.Select(
                    attribute =>
                        new Binding(attribute, GetBindingLevel(attribute.Binding)));

            var toSourceAttributes = attributes.Where(attribute => attribute.BindingMode != BindingMode.OneWayToSource)
                .ToList();
            var valueGroups = toSourceAttributes.GroupBy(attribute => attribute.Value);

            if (valueGroups.Count() < toSourceAttributes.Count)
            {
                throw new InvalidOperationException("Subtype values must be unique.");
            }

            var toTargetAttributes = attributes.Where(attribute => attribute.BindingMode != BindingMode.OneWay)
                .ToList();

            var subTypeGroups = toTargetAttributes.GroupBy(attribute => attribute.Subtype);
            var subTypeGroupCount = subTypeGroups.Count();
            if (subTypeGroupCount < toTargetAttributes.Count)
            {
                throw new InvalidOperationException(
                    "Subtypes must be unique for two-way subtype bindings.  Set BindingMode to OneWay to disable updates to the binding source during serialization.");
            }

            var invalidSubtype =
                attributes.FirstOrDefault(attribute => !checkType.IsAssignableFrom(attribute.Subtype));

            if (invalidSubtype != null)
            {
                throw new InvalidOperationException($"{invalidSubtype.Subtype} is not a subtype of {checkType}");
            }

            return new BindingCollection(bindings);
        }

        private int FindAncestorLevel(BindingInfo binding)
        {
            var level = 1;
            var parent = Parent;
            while (parent != null)
            {
                if (binding != null && binding.RelativeSourceMode == RelativeSourceMode.FindAncestor)
                {
                    if (binding.AncestorLevel == level)
                    {
                        return level;
                    }

                    if (binding.AncestorType != null && parent.Type != null &&
                        binding.AncestorType.IsAssignableFrom(parent.Type))
                    {
                        return level;
                    }
                }

                parent = parent.Parent;
                level++;
            }

            if (binding != null && binding.RelativeSourceMode == RelativeSourceMode.SerializationContext)
            {
                return level;
            }

            throw new BindingException("No ancestor found.");
        }
    }
}