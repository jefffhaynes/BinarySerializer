using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BinarySerialization.Graph.TypeGraph;

namespace BinarySerialization.Graph.ValueGraph
{
    public abstract class ValueNode : Node<ValueNode>
    {
        public static readonly object UnsetValue = new object();

        private bool _visited;

        protected ValueNode(ValueNode parent, string name, TypeNode typeNode) : base(parent)
        {
            Name = name;
            TypeNode = typeNode;
            Children = new List<ValueNode>();
            Bindings = new List<Func<object>>();
        }

        public TypeNode TypeNode { get; set; }

        public List<Func<object>> Bindings { get; set; }

        public abstract object Value { get; set; }

        public virtual object BoundValue => Value;

        public virtual bool Visited
        {
            get => _visited;

            set
            {
                foreach (var child in Children)
                {
                    child.Visited = value;
                }

                _visited = value;
            }
        }

        private bool ShouldSerialize
        {
            get
            {
                if (TypeNode.SerializeWhenBindings != null &&
                    !TypeNode.SerializeWhenBindings.Any(binding => binding.IsSatisfiedBy(binding.GetValue(this))))
                {
                    return false;
                }

                if (TypeNode.SerializeWhenNotBindings != null &&
                    TypeNode.SerializeWhenNotBindings.All(binding => binding.IsSatisfiedBy(binding.GetValue(this))))
                {
                    return false;
                }

                return true;
            }
        }

        public virtual void Bind()
        {
            var typeNode = TypeNode;

            typeNode.FieldLengthBindings?.Bind(this, () => MeasureOverride());
            typeNode.ItemLengthBindings?.Bind(this, MeasureItemsOverride);
            typeNode.FieldCountBindings?.Bind(this, () => CountOverride());

            typeNode.SubtypeBindings?.Bind(this, () => SubtypeBindingCallback(typeNode));

            if (typeNode.SubtypeFactoryBinding != null && typeNode.SubtypeFactoryBinding.BindingMode !=
                BindingMode.OneWay)
            {
                typeNode.SubtypeFactoryBinding.Bind(this, () => SubtypeBindingCallback(typeNode));
            }

            var parent = typeNode.Parent;
            parent.ItemSubtypeBindings?.Bind(Parent, () => ItemSubtypeBindingCallback(typeNode));

            if (parent.ItemSubtypeFactoryBinding != null &&
                parent.ItemSubtypeFactoryBinding.BindingMode != BindingMode.OneWay)
            {
                parent.ItemSubtypeFactoryBinding.Bind(Parent, () => ItemSubtypeBindingCallback(typeNode));
            }

            if (typeNode.ItemSerializeUntilBinding != null &&
                typeNode.ItemSerializeUntilBinding.BindingMode != BindingMode.OneWay)
            {
                typeNode.ItemSerializeUntilBinding.Bind(this, GetLastItemValueOverride);
            }

            if (typeNode.FieldValueBindings != null)
            {
                // for each field value binding, create an anonymous function to get the final value from the corresponding attribute.
                for (var index = 0; index < typeNode.FieldValueBindings.Count; index++)
                {
                    var fieldValueBinding = typeNode.FieldValueBindings[index];

                    var attributeIndex = index;
                    fieldValueBinding.Bind(this, () =>
                    {
                        if (!Visited)
                        {
                            throw new InvalidOperationException(
                                "Reverse binding not allowed on FieldValue attributes.  Consider swapping source and target.");
                        }

                        return TypeNode.FieldValueAttributes[attributeIndex].ComputeFinalInternal();
                    });
                }
            }

            // recurse to children
            foreach (var child in Children)
            {
                child.Bind();
            }
        }

        internal void Serialize(BoundedStream stream, EventShuttle eventShuttle, bool align = true)
        {
            try
            {
                if (TypeNode.SerializeWhenBindings != null &&
                    !TypeNode.SerializeWhenBindings.Any(binding => binding.IsSatisfiedBy(binding.GetBoundValue(this))))
                {
                    return;
                }

                if (TypeNode.SerializeWhenNotBindings != null &&
                    TypeNode.SerializeWhenNotBindings.All(
                        binding => binding.IsSatisfiedBy(binding.GetBoundValue(this))))
                {
                    return;
                }

                if (align)
                {
                    var leftAlignment = GetLeftFieldAlignment();
                    if (leftAlignment != null)
                    {
                        Align(stream, leftAlignment, true);
                    }
                }

                var offset = GetFieldOffset();

                if (offset != null)
                {
                    using (new StreamResetter(stream))
                    {
                        stream.Position = offset.Value;
                        SerializeInternal(stream, GetConstFieldLength, eventShuttle);
                    }
                }
                else
                {
                    SerializeInternal(stream, GetConstFieldLength, eventShuttle);
                }

                if (align)
                {
                    var rightAlignment = GetRightFieldAlignment();
                    if (rightAlignment != null)
                    {
                        Align(stream, rightAlignment, true);
                    }
                }
            }
            catch (IOException)
            {
                // since this isn't really a serialization exception, no sense in hiding it
                throw;
            }
            catch (TimeoutException)
            {
                // since this isn't really a serialization exception, no sense in hiding it
                throw;
            }
            catch (Exception e)
            {
                var reference = Name == null
                    ? $"type '{TypeNode.Type}'"
                    : $"member '{Name}'";
                var message = $"Error serializing {reference}.  See inner exception for detail.";
                throw new InvalidOperationException(message, e);
            }
            finally
            {
                Visited = true;
            }
        }

        internal void Deserialize(BoundedStream stream, EventShuttle eventShuttle)
        {
            try
            {
                if (!ShouldSerialize)
                {
                    return;
                }

                AlignLeft(stream);

                var offset = GetFieldOffset();

                if (offset != null)
                {
                    using (new StreamResetter(stream))
                    {
                        stream.Position = offset.Value;
                        DeserializeInternal(stream, GetFieldLength, eventShuttle);
                    }
                }
                else
                {
                    DeserializeInternal(stream, GetFieldLength, eventShuttle);
                }

                AlignRight(stream);
            }
            catch (IOException)
            {
                // since this isn't really a serialization exception, no sense in hiding it
                throw;
            }
            catch (TimeoutException)
            {
                // since this isn't really a serialization exception, no sense in hiding it
                throw;
            }
            catch (Exception e)
            {
                var reference = Name == null
                    ? $"type '{TypeNode.Type}'"
                    : $"member '{Name}'";
                var message = $"Error deserializing '{reference}'.  See inner exception for detail.";
                throw new InvalidOperationException(message, e);
            }
            finally
            {
                Visited = true;
            }
        }

        internal async Task DeserializeAsync(BoundedStream stream, EventShuttle eventShuttle,
            CancellationToken cancellationToken)
        {
            try
            {
                if (!ShouldSerialize)
                {
                    return;
                }

                AlignLeft(stream);

                var offset = GetFieldOffset();

                if (offset != null)
                {
                    using (new StreamResetter(stream))
                    {
                        stream.Position = offset.Value;
                        await DeserializeInternalAsync(stream, GetFieldLength, eventShuttle, cancellationToken)
                            .ConfigureAwait(false);
                    }
                }
                else
                {
                    await DeserializeInternalAsync(stream, GetFieldLength, eventShuttle, cancellationToken)
                        .ConfigureAwait(false);
                }

                AlignRight(stream);
            }
            catch (IOException)
            {
                throw;
            }
            catch (Exception e)
            {
                var reference = Name == null
                    ? $"type '{TypeNode.Type}'"
                    : $"member '{Name}'";
                var message = $"Error deserializing '{reference}'.  See inner exception for detail.";
                throw new InvalidOperationException(message, e);
            }
            finally
            {
                Visited = true;
            }
        }

        internal LazyBinarySerializationContext CreateLazySerializationContext()
        {
            var lazyContext = new Lazy<BinarySerializationContext>(CreateSerializationContext);
            return new LazyBinarySerializationContext(lazyContext);
        }

        // this is internal only because of the weird custom subtype case.  If I can figure out a better
        // way to handle that case, this can be protected.
        internal abstract void SerializeOverride(BoundedStream stream, EventShuttle eventShuttle);

        internal abstract void DeserializeOverride(BoundedStream stream, EventShuttle eventShuttle);

        internal abstract Task DeserializeOverrideAsync(BoundedStream stream, EventShuttle eventShuttle,
            CancellationToken cancellationToken);

        protected IEnumerable<ValueNode> GetSerializableChildren()
        {
            return Children.Where(child => !child.TypeNode.IsIgnored);
        }

        protected long? GetFieldLength()
        {
            var length = GetNumericValue(TypeNode.FieldLengthBindings);
            if (length != null)
            {
                return length;
            }

            var parent = Parent;
            if (parent?.TypeNode.ItemLengthBindings != null)
            {
                var parentItemLength = parent.TypeNode.ItemLengthBindings.GetValue(parent);
                if (parentItemLength.GetType().GetTypeInfo().IsPrimitive)
                {
                    return Convert.ToInt64(parentItemLength);
                }
            }

            return null;
        }

        protected long? GetConstFieldLength()
        {
            return GetConstNumericValue(TypeNode.FieldLengthBindings) ??
                   Parent?.GetConstFieldItemLength();
        }

        protected long? GetLeftFieldAlignment()
        {
            // Field alignment cannot be determined from graph
            // so always go to a const or bound value
            var value = TypeNode.LeftFieldAlignmentBindings?.GetBoundValue(this);
            if (value == null)
            {
                return null;
            }

            return Convert.ToInt64(value);
        }

        protected long? GetRightFieldAlignment()
        {
            // Field alignment cannot be determined from graph
            // so always go to a const or bound value
            var value = TypeNode.RightFieldAlignmentBindings?.GetBoundValue(this);
            if (value == null)
            {
                return null;
            }

            return Convert.ToInt64(value);
        }

        protected long? GetFieldCount()
        {
            return GetNumericValue(TypeNode.FieldCountBindings);
        }

        protected long? GetConstFieldCount()
        {
            return GetConstNumericValue(TypeNode.FieldCountBindings);
        }

        protected long? GetFieldItemLength()
        {
            return GetNumericValue(TypeNode.ItemLengthBindings);
        }

        protected long? GetConstFieldItemLength()
        {
            return GetConstNumericValue(TypeNode.ItemLengthBindings);
        }

        protected long? GetFieldOffset()
        {
            return GetNumericValue(TypeNode.FieldOffsetBindings);
        }

        protected long? GetConstFieldOffset()
        {
            return GetConstNumericValue(TypeNode.FieldOffsetBindings);
        }

        protected virtual Endianness GetFieldEndianness()
        {
            var endianness = Endianness.Inherit;

            var value = TypeNode.FieldEndiannessBindings?.GetBoundValue(this);

            if (value != null)
            {
                if (value is Endianness)
                {
                    endianness = (Endianness) Enum.ToObject(typeof(Endianness), value);
                }
                else
                {
                    throw new InvalidOperationException(
                        "FieldEndianness converters must return a valid Endianness.");
                }
            }

            if (endianness == Endianness.Inherit && Parent != null)
            {
                var parent = Parent;
                endianness = parent.GetFieldEndianness();
            }

            return endianness;
        }

        protected virtual Encoding GetFieldEncoding()
        {
            Encoding encoding = null;

            var value = TypeNode.FieldEncodingBindings?.GetBoundValue(this);

            if (value != null)
            {
                if (value is Encoding)
                {
                    encoding = value as Encoding;
                }
                else
                {
                    throw new InvalidOperationException("FieldEncoding converters must return a valid Encoding.");
                }
            }

            if (encoding == null && Parent != null)
            {
                var parent = Parent;
                encoding = parent.GetFieldEncoding();
            }

            return encoding;
        }

        protected virtual long MeasureOverride()
        {
            var nullStream = new NullStream();
            var boundedStream = new BoundedStream(nullStream);
            Serialize(boundedStream, null, false);
            return boundedStream.RelativePosition;
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

        protected static bool EndOfStream(BoundedStream stream)
        {
            return stream.IsAtLimit || stream.AvailableForReading == 0;
        }

        private object SubtypeBindingCallback(TypeNode typeNode)
        {
            var valueType = GetValueTypeOverride();
            if (valueType == null)
            {
                throw new InvalidOperationException("Binding targets must not be null.");
            }

            var objectTypeNode = (ObjectTypeNode) typeNode;

            object value;

            // first try explicitly specified subtypes
            if (typeNode.SubtypeBindings != null)
            {
                if (objectTypeNode.SubTypeKeys.TryGetValue(valueType, out value))
                {
                    return value;
                }
            }

            // next try factory
            if (typeNode.SubtypeFactory != null)
            {
                if (typeNode.SubtypeFactory.TryGetKey(valueType, out value))
                {
                    return value;
                }
            }

            // allow default subtypes in order to support round-trip
            if (typeNode.SubtypeDefaultAttribute != null)
            {
                if (valueType == typeNode.SubtypeDefaultAttribute.Subtype)
                {
                    return UnsetValue;
                }
            }

            throw new InvalidOperationException($"No subtype specified for ${valueType}");
        }

        private object ItemSubtypeBindingCallback(TypeNode typeNode)
        {
            var valueType = GetValueTypeOverride();
            if (valueType == null)
            {
                throw new InvalidOperationException("Binding targets must not be null.");
            }

            var parent = typeNode.Parent;
            var objectTypeNode = (ObjectTypeNode) typeNode;

            object value;

            // first try explicitly specified subtypes
            if (parent.ItemSubtypeBindings != null)
            {
                if (objectTypeNode.SubTypeKeys.TryGetValue(valueType, out value))
                {
                    return value;
                }
            }

            // next try factory
            if (parent.ItemSubtypeFactory != null)
            {
                if (parent.ItemSubtypeFactory.TryGetKey(valueType, out value))
                {
                    return value;
                }
            }

            // allow default subtypes in order to support round-trip
            if (parent.ItemSubtypeDefaultAttribute != null)
            {
                if (valueType == parent.ItemSubtypeDefaultAttribute.Subtype)
                {
                    return UnsetValue;
                }
            }

            throw new InvalidOperationException($"No subtype specified for ${valueType}");
        }

        private void SerializeInternal(BoundedStream stream, Func<long?> maxLengthDelegate, EventShuttle eventShuttle)
        {
            stream = new BoundedStream(stream, maxLengthDelegate);

            if (TypeNode.FieldValueAttributes != null && TypeNode.FieldValueAttributes.Count > 0)
            {
                var context = CreateLazySerializationContext();

                // Setup tap for value attributes if we need to siphon serialized data for later
                foreach (var fieldValueAttribute in TypeNode.FieldValueAttributes)
                {
                    fieldValueAttribute.ResetInternal(context);
                    var tap = new FieldValueAdapterStream(fieldValueAttribute);
                    stream = new TapStream(stream, tap);
                }
            }

            SerializeOverride(stream, eventShuttle);

            /* Check if we need to pad out object */
            var length = GetConstFieldLength();

            if (length != null)
            {
                if (length > stream.RelativePosition)
                {
                    var padLength = length - stream.RelativePosition;
                    var pad = new byte[(int) padLength];
                    stream.Write(pad, 0, pad.Length);
                }
            }

            if (TypeNode.FieldValueAttributes != null && TypeNode.FieldValueAttributes.Any())
            {
                stream.Flush();
            }
        }

        private void AlignRight(BoundedStream stream)
        {
            var rightAlignment = GetRightFieldAlignment();
            if (rightAlignment != null)
            {
                Align(stream, rightAlignment);
            }
        }

        private void AlignLeft(BoundedStream stream)
        {
            var leftAlignment = GetLeftFieldAlignment();
            if (leftAlignment != null)
            {
                Align(stream, leftAlignment);
            }
        }

        private void Align(BoundedStream stream, long? alignment, bool pad = false)
        {
            if (alignment == null)
            {
                throw new ArgumentNullException(nameof(alignment));
            }

            var position = stream.RelativePosition;
            var delta = (alignment.Value - position % alignment.Value) % alignment.Value;

            if (delta == 0)
            {
                return;
            }

            if (pad)
            {
                var padding = new byte[delta];
                stream.Write(padding, 0, padding.Length);
            }
            else
            {
                for (var i = 0; i < delta; i++)
                {
                    if (stream.ReadByte() < 0)
                    {
                        break;
                    }
                }
            }
        }

        private void DeserializeInternal(BoundedStream stream, Func<long?> maxLengthDelegate, EventShuttle eventShuttle)
        {
            stream = new BoundedStream(stream, maxLengthDelegate);

            DeserializeOverride(stream, eventShuttle);

            SkipPadding(stream);
        }

        private async Task DeserializeInternalAsync(BoundedStream stream, Func<long?> maxLengthDelegate,
            EventShuttle eventShuttle, CancellationToken cancellationToken)
        {
            stream = new BoundedStream(stream, maxLengthDelegate);

            await DeserializeOverrideAsync(stream, eventShuttle, cancellationToken);

            SkipPadding(stream);
        }

        private void SkipPadding(BoundedStream stream)
        {
            var length = GetConstFieldLength();

            if (length != null)
            {
                if (length > stream.RelativePosition)
                {
                    var padLength = length - stream.RelativePosition;
                    var pad = new byte[(int) padLength];
                    stream.Read(pad, 0, pad.Length);
                }
            }
        }

        private long? GetNumericValue(IBinding binding)
        {
            var value = binding?.GetValue(this);
            if (value == null)
            {
                return null;
            }

            return Convert.ToInt64(value);
        }

        private long? GetConstNumericValue(IBinding binding)
        {
            if (binding == null)
            {
                return null;
            }

            if (!binding.IsConst)
            {
                return null;
            }

            return Convert.ToInt64(binding.ConstValue);
        }

        private BinarySerializationContext CreateSerializationContext()
        {
            var parent = Parent;
            return new BinarySerializationContext(Value, parent?.Value, parent?.TypeNode.Type,
                parent?.CreateSerializationContext(), TypeNode.MemberInfo);
        }
    }
}