using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BinarySerialization.Graph.TypeGraph;

namespace BinarySerialization.Graph.ValueGraph
{
    internal abstract class ValueNode : Node
    {
        private const char PathSeparator = '.';

        protected static readonly Dictionary<Type, Func<object, object>> TypeConverters =
            new Dictionary<Type, Func<object, object>>
            {
                {typeof (char), o => Convert.ToChar(o)},
                {typeof (byte), o => Convert.ToByte(o)},
                {typeof (sbyte), o => Convert.ToSByte(o)},
                {typeof (bool), o => Convert.ToBoolean(o)},
                {typeof (short), o => Convert.ToInt16(o)},
                {typeof (int), o => Convert.ToInt32(o)},
                {typeof (long), o => Convert.ToInt64(o)},
                {typeof (ushort), o => Convert.ToUInt16(o)},
                {typeof (uint), o => Convert.ToUInt32(o)},
                {typeof (ulong), o => Convert.ToUInt64(o)},
                {typeof (float), o => Convert.ToSingle(o)},
                {typeof (double), o => Convert.ToDouble(o)},
                {typeof (string), Convert.ToString}
            };

        protected ValueNode(Node parent, string name, TypeNode typeNode) : base(parent)
        {
            Name = name;
            TypeNode = typeNode;
            Children = new List<ValueNode>();
        }

        public TypeNode TypeNode { get; set; }

        public List<ValueNode> Children { get; set; }

        public virtual Encoding Encoding
        {
            get
            {
                if (TypeNode.Encoding != null)
                    return TypeNode.Encoding;

                var parent = (ValueNode)Parent;
                return parent.Encoding;
            }
        }

        public virtual Endianness Endianness
        {
            get
            {
                if (TypeNode.Endianness != null && TypeNode.Endianness != Endianness.Inherit)
                    return TypeNode.Endianness.Value;

                var parent = (ValueNode)Parent;
                return parent.Endianness;
            }
        }

        public abstract object Value { get; set; }

        public virtual object BoundValue => Value;

        public virtual void Bind()
        {
            var typeNode = TypeNode;

            if (typeNode.FieldLengthBinding != null && typeNode.FieldLengthBinding.BindingMode == BindingMode.TwoWay)
            {
                typeNode.FieldLengthBinding.Bind(this, () => MeasureOverride());
            }

            if (typeNode.ItemLengthBinding != null && typeNode.ItemLengthBinding.BindingMode == BindingMode.TwoWay)
            {
                typeNode.ItemLengthBinding.Bind(this, MeasureItemsOverride);
            }

            if (typeNode.FieldCountBinding != null && typeNode.FieldCountBinding.BindingMode == BindingMode.TwoWay)
            {
                typeNode.FieldCountBinding.Bind(this, () => CountOverride());
            }

            if (typeNode.SubtypeBinding != null && typeNode.SubtypeBinding.BindingMode == BindingMode.TwoWay)
            {
                typeNode.SubtypeBinding.Bind(this, () =>
                {
                    Type valueType = GetValueTypeOverride();
                    if (valueType == null)
                        return null;

                    var objectTypeNode = (ObjectTypeNode)typeNode;

                    return objectTypeNode.SubTypeKeys[valueType];
                });
            }

            if (typeNode.ItemSerializeUntilBinding != null && typeNode.ItemSerializeUntilBinding.BindingMode == BindingMode.TwoWay)
            {
                typeNode.ItemSerializeUntilBinding.Bind(this, GetLastItemValueOverride);
            }

            foreach (ValueNode child in Children)
                child.Bind();
        }

        protected IEnumerable<ValueNode> GetSerializableChildren()
        {
            return Children.Where(child => !child.TypeNode.IsIgnored);
        }

        public void Serialize(StreamLimiter stream, EventShuttle eventShuttle)
        {
            try
            {
                var serializeWhenBindings = TypeNode.SerializeWhenBindings;
                if (serializeWhenBindings != null &&
                    !serializeWhenBindings.Any(binding => binding.ConditionalValue.Equals(binding.GetBoundValue(this))))
                    return;

                if (TypeNode.FieldLengthBinding != null && TypeNode.FieldLengthBinding.IsConst)
                    stream = new StreamLimiter(stream, Convert.ToInt64(TypeNode.FieldLengthBinding.ConstValue));

                Binding fieldOffsetBinding = TypeNode.FieldOffsetBinding;

                using (new StreamResetter(stream, fieldOffsetBinding != null))
                {
                    if (fieldOffsetBinding != null)
                        stream.Position = Convert.ToInt64(fieldOffsetBinding.GetValue(this));

                    SerializeOverride(stream, eventShuttle);
                }
            }
            catch (IOException)
            {
                throw;
            }
            catch (Exception e)
            {
                string reference = Name == null
                    ? $"type '{TypeNode.Type}'"
                    : $"member '{Name}'";
                string message = $"Error serializing {reference}.  See inner exception for detail.";
                throw new InvalidOperationException(message, e);
            }
        }

        protected abstract void SerializeOverride(StreamLimiter stream, EventShuttle eventShuttle);

        public void Deserialize(StreamLimiter stream, EventShuttle eventShuttle)
        {
            try
            {
                var serializeWhenBindings = TypeNode.SerializeWhenBindings;
                if (serializeWhenBindings != null &&
                    !serializeWhenBindings.Any(binding => binding.ConditionalValue.Equals(binding.GetValue(this))))
                    return;

                if (TypeNode.FieldLengthBinding != null)
                    stream = new StreamLimiter(stream, Convert.ToInt64(TypeNode.FieldLengthBinding.GetValue(this)));

                Binding fieldOffsetBinding = TypeNode.FieldOffsetBinding;

                using (new StreamResetter(stream, fieldOffsetBinding != null))
                {
                    if (fieldOffsetBinding != null)
                        stream.Position = Convert.ToInt64(fieldOffsetBinding.GetValue(this));

                    DeserializeOverride(stream, eventShuttle);
                }
            }
            catch (EndOfStreamException e)
            {
                string reference = Name == null
                    ? $"type '{TypeNode.Type}'"
                    : $"member '{Name}'";
                string message = $"Error deserializing '{reference}'.  See inner exception for detail.";
                throw new InvalidOperationException(message, e);
            }
            catch (IOException)
            {
                throw;
            }
            catch (Exception e)
            {
                string reference = Name == null
                    ? $"type '{TypeNode.Type}'"
                    : $"member '{Name}'";
                string message = $"Error deserializing '{reference}'.  See inner exception for detail.";
                throw new InvalidOperationException(message, e);
            }
        }

        public abstract void DeserializeOverride(StreamLimiter stream, EventShuttle eventShuttle);

        public ValueNode GetChild(string path)
        {
            string[] memberNames = path.Split(PathSeparator);

            if (memberNames.Length == 0)
                throw new BindingException("Path cannot be empty.");

            ValueNode child = this;
            foreach (string name in memberNames)
            {
                child = child.Children.SingleOrDefault(c => c.Name == name);

                if (child == null)
                    throw new BindingException($"No field found at '{path}'.");
            }

            return child;
        }

        public virtual BinarySerializationContext CreateSerializationContext()
        {
            if (Parent == null)
                return null;

            var parent = (ValueNode) Parent;
            return new BinarySerializationContext(parent.Value, parent.TypeNode.Type, parent.CreateSerializationContext());
        }

        protected static object ConvertToType(object value, Type targetType)
        {
            if (value == null)
                return null;

            Type valueType = value.GetType();

            if (valueType == targetType)
                return value;

            /* Special handling for strings */
            if (valueType == typeof(string) && targetType.IsPrimitive)
            {
                if (string.IsNullOrWhiteSpace(value.ToString()))
                    value = 0;
            }

            Func<object, object> converter;
            if (TypeConverters.TryGetValue(targetType, out converter))
                return converter(value);

            if (targetType.IsEnum && value.GetType().IsPrimitive)
                return Enum.ToObject(targetType, value);

            return value;
        }

        protected virtual long MeasureOverride()
        {
            var nullStream = new NullStream();
            var streamLimiter = new StreamLimiter(nullStream);
            Serialize(streamLimiter, null);
            return streamLimiter.RelativePosition;
        }

        protected virtual IEnumerable<long> MeasureItemsOverride()
        {
            throw new InvalidOperationException("Not a collection field.");
        }

        protected virtual long CountOverride()
        {
            throw new InvalidOperationException("Not a collection field.");
        }

        protected virtual Type GetValueTypeOverride()
        {
            throw new InvalidOperationException("Can't set subtypes on this field.");
        }

        protected virtual object GetLastItemValueOverride()
        {
            throw new InvalidOperationException("Not a collection field.");
        }

        protected static bool ShouldTerminate(StreamLimiter stream)
        {
            if (stream.IsAtLimit)
                return true;

            return stream.CanSeek && stream.Position >= stream.Length;
        }

        protected static IEnumerable<int> GetInfiniteSequence(int value)
        {
            while (true)
                yield return value;
            // ReSharper disable FunctionNeverReturns
        }
        // ReSharper restore FunctionNeverReturns
    }
}