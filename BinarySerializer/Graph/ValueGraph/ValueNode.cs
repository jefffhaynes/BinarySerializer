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
    internal abstract class ValueNode : Node<ValueNode>
    {
        public static readonly object UnsetValue = new object();

        private bool _visited;

        protected ValueNode(ValueNode parent, string name, TypeNode typeNode) : base(parent)
        {
            Name = name;
            TypeNode = typeNode;
            Children = new List<ValueNode>();
            Bindings = new List<Func<object>>();

            _fieldValueAttributeTaps =
                typeNode.FieldValueAttributes?.ToDictionary(attribute => attribute, attribute => default(FieldValueAdapterStream));

            _fieldValueAttributeFinalValue = new Dictionary<FieldValueAttributeBase, object>();
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

        private readonly Dictionary<FieldValueAttributeBase, FieldValueAdapterStream> _fieldValueAttributeTaps;
        private readonly Dictionary<FieldValueAttributeBase, object> _fieldValueAttributeFinalValue;

        private bool ShouldSerialize(Func<Binding, object> bindingValueSelector)
        {
            return TypeNode.SerializeWhenBindings == null || 
                   TypeNode.SerializeWhenBindings.Any(binding => binding.IsSatisfiedBy(bindingValueSelector(binding)));
        }

        public bool ShouldSerialize()
        {
            return ShouldSerialize(binding => binding.GetBoundValue(this));
        }

        public bool ShouldDeserialize()
        {
            return ShouldSerialize(binding => binding.GetValue(this));
        }

        public virtual void Bind()
        {
            var typeNode = TypeNode;

            typeNode.FieldLengthBindings?.Bind(this, () => MeasureOverride().ByteCount);
            typeNode.FieldBitLengthBindings?.Bind(this, () => MeasureOverride().TotalBitCount);
            typeNode.ItemLengthBindings?.Bind(this, () => MeasureItemsOverride().Select(item => item.ByteCount));
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

                    if (fieldValueBinding.BindingMode == BindingMode.OneWay)
                    {
                        continue;
                    }

                    var fieldValueAttribute = typeNode.FieldValueAttributes[index];
                    
                    fieldValueBinding.Bind(this, () =>
                    {
                        if (!Visited)
                        {
                            throw new InvalidOperationException(
                                "Reverse binding not allowed on FieldValue attributes.  Consider swapping source and target.");
                        }

                        var source = fieldValueBinding.GetSource(this);
                        if (!source._fieldValueAttributeFinalValue.TryGetValue(fieldValueAttribute, out var finalValue))
                        {
                            var tap = source._fieldValueAttributeTaps[fieldValueAttribute];

                            finalValue = fieldValueAttribute.GetFinalValueInternal(tap.State);
                            source._fieldValueAttributeFinalValue.Add(fieldValueAttribute, finalValue);
                        }

                        return finalValue;
                    });
                }
            }

            // recurse to children
            foreach (var child in Children)
            {
                child.Bind();
            }
        }

        public virtual void BindChecks()
        {
            var typeNode = TypeNode;

            if (typeNode.FieldValueBindings != null)
            {
                // for each field value binding, create an anonymous function to get the final value from the corresponding attribute.
                for (var index = 0; index < typeNode.FieldValueBindings.Count; index++)
                {
                    var fieldValueBinding = typeNode.FieldValueBindings[index];

                    if (fieldValueBinding.BindingMode == BindingMode.OneWayToSource)
                    {
                        continue;
                    }

                    var fieldValueAttribute = typeNode.FieldValueAttributes[index];

                    fieldValueBinding.Bind(this, () =>
                    {
                        if (!Visited)
                        {
                            throw new InvalidOperationException(
                                "Reverse binding not allowed on FieldValue attributes.  Consider swapping source and target.");
                        }

                        var source = fieldValueBinding.GetSource(this);
                        if (!source._fieldValueAttributeFinalValue.TryGetValue(fieldValueAttribute, out var finalValue))
                        {
                            var tap = source._fieldValueAttributeTaps[fieldValueAttribute];

                            finalValue = fieldValueAttribute.GetFinalValueInternal(tap.State);
                            source._fieldValueAttributeFinalValue.Add(fieldValueAttribute, finalValue);
                        }

                        return finalValue;
                    });
                }
            }

            foreach (var valueNode in Children)
            {
                valueNode.BindChecks();
            }
        }

        internal void Serialize(BoundedStream stream, EventShuttle eventShuttle, bool align = true,
            bool measuring = false)
        {
            try
            {
                if (align)
                {
                    AlignLeft(stream, true);
                }

                var offset = GetFieldOffset();

                if (offset != null)
                {
                    using (new StreamResetter(stream))
                    {
                        stream.Position = offset.Value;
                        SerializeInternal(stream, GetConstFieldLength, eventShuttle, measuring);
                    }
                }
                else
                {
                    SerializeInternal(stream, GetConstFieldLength, eventShuttle, measuring);
                }

                if (align)
                {
                    AlignRight(stream, true);
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
                ThrowSerializationException(e);
            }
            finally
            {
                Visited = true;
            }
        }
        
        internal async Task SerializeAsync(BoundedStream stream, EventShuttle eventShuttle, bool align, CancellationToken cancellationToken)
        {
            try
            {
                if (align)
                {
                    AlignLeft(stream, true);
                }

                var offset = GetFieldOffset();

                if (offset != null)
                {
                    using (new StreamResetter(stream))
                    {
                        stream.Position = offset.Value;
                        await SerializeInternalAsync(stream, GetConstFieldLength, eventShuttle, cancellationToken)
                            .ConfigureAwait(false);
                    }
                }
                else
                {
                    await SerializeInternalAsync(stream, GetConstFieldLength, eventShuttle, cancellationToken)
                        .ConfigureAwait(false);
                }

                if (align)
                {
                    AlignRight(stream, true);
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
                ThrowSerializationException(e);
            }
            finally
            {
                Visited = true;
            }
        }

        private void ThrowSerializationException(Exception e)
        {
            var reference = Name == null
                ? $"type '{TypeNode.Type}'"
                : $"member '{Name}'";
            var message = $"Error serializing {reference}.  See inner exception for detail.";
            throw new InvalidOperationException(message, e);
        }

        internal void Deserialize(BoundedStream stream, SerializationOptions options, EventShuttle eventShuttle)
        {
            try
            {
                AlignLeft(stream);

                var offset = GetFieldOffset();

                if (offset != null)
                {
                    using (new StreamResetter(stream))
                    {
                        stream.Position = offset.Value;
                        DeserializeInternal(stream, GetFieldLength, options, eventShuttle);
                    }
                }
                else
                {
                    DeserializeInternal(stream, GetFieldLength, options, eventShuttle);
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
                ThrowDeserializationException(e);
            }
            finally
            {
                Visited = true;
            }
            
            BindChecks();
        }

        internal async Task DeserializeAsync(BoundedStream stream, SerializationOptions options, EventShuttle eventShuttle, 
            CancellationToken cancellationToken)
        {
            try
            {
                AlignLeft(stream);

                var offset = GetFieldOffset();

                if (offset != null)
                {
                    using (new StreamResetter(stream))
                    {
                        stream.Position = offset.Value;
                        await DeserializeInternalAsync(stream, GetFieldLength, options, eventShuttle, cancellationToken)
                            .ConfigureAwait(false);
                    }
                }
                else
                {
                    await DeserializeInternalAsync(stream, GetFieldLength, options, eventShuttle, cancellationToken)
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
                ThrowDeserializationException(e);
            }
            finally
            {
                Visited = true;
            }
            
            BindChecks();
        }

        private void ThrowDeserializationException(Exception e)
        {
            var reference = Name == null
                ? $"type '{TypeNode.Type}'"
                : $"member '{Name}'";
            var message = $"Error deserializing '{reference}'.  See inner exception for detail.";
            throw new InvalidOperationException(message, e);
        }

        internal LazyBinarySerializationContext CreateLazySerializationContext()
        {
            var lazyContext = new Lazy<BinarySerializationContext>(CreateSerializationContext);
            return new LazyBinarySerializationContext(lazyContext);
        }

        // this is internal only because of the weird custom subtype case.  If I can figure out a better
        // way to handle that case, this can be protected.
        internal abstract void SerializeOverride(BoundedStream stream, EventShuttle eventShuttle);
        
        internal abstract Task SerializeOverrideAsync(BoundedStream stream, EventShuttle eventShuttle,
            CancellationToken cancellationToken);

        internal abstract void DeserializeOverride(BoundedStream stream, SerializationOptions options, EventShuttle eventShuttle);

        internal abstract Task DeserializeOverrideAsync(BoundedStream stream, SerializationOptions options, 
            EventShuttle eventShuttle, CancellationToken cancellationToken);

        protected IEnumerable<ValueNode> GetSerializableChildren()
        {
            return Children.Where(child => !child.TypeNode.IsIgnored);
        }

        protected FieldLength GetFieldLength()
        {
            var length = GetNumericValue(TypeNode.FieldLengthBindings);
            if (length != null)
            {
                return length.Value;
            }

            var bitLength = GetNumericValue(TypeNode.FieldBitLengthBindings);
            var bitOrder = ((BitOrder?)GetNumericValue(TypeNode.FieldBitOrderBindings) ?? 
                           (BitOrder?)GetConstNumericValue(TypeNode.FieldBitOrderBindings)) ??
                           BitOrder.LsbFirst;


            if (bitLength != null)
            {
                return FieldLength.FromBitCount((int) bitLength, bitOrder);
            }

            var parent = Parent;
            var parentItemLengthBindings = parent?.TypeNode.ItemLengthBindings;

            if (parentItemLengthBindings == null)
            {
                return null;
            }

            var parentItemLength = parentItemLengthBindings.GetValue(parent);
            if (parentItemLength.GetType().GetTypeInfo().IsPrimitive)
            {
                return Convert.ToInt64(parentItemLength);
            }

            return null;
        }

        protected FieldLength GetConstFieldLength()
        {
            var length = GetConstNumericValue(TypeNode.FieldLengthBindings);

            if (length != null)
            {
                return length;
            }

            var bitLength = GetConstNumericValue(TypeNode.FieldBitLengthBindings);
            var bitOrder = ((BitOrder?)GetNumericValue(TypeNode.FieldBitOrderBindings) ??
                           (BitOrder?)GetConstNumericValue(TypeNode.FieldBitOrderBindings)) ??
                           BitOrder.LsbFirst;

            return bitLength != null ? FieldLength.FromBitCount((int) bitLength, bitOrder) : Parent?.GetConstFieldItemLength();
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

        protected FieldLength GetConstFieldItemLength()
        {
            return GetConstNumericValue(TypeNode.ItemLengthBindings);
        }

        protected long? GetFieldOffset()
        {
            return GetNumericValue(TypeNode.FieldOffsetBindings);
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
                if (value is Encoding encodingValue)
                {
                    encoding = encodingValue;
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


        protected virtual byte GetFieldPaddingValue()
        {
            if (TypeNode.PaddingValue != null)
            {
                return TypeNode.PaddingValue.Value;
            }

            return Parent?.GetFieldPaddingValue() ?? default;
        }

        protected virtual FieldLength MeasureOverride()
        {
            var nullStream = new NullStream();
            var boundedStream = new BoundedStream(nullStream, Name);
            Serialize(boundedStream, null, false, true);
            return boundedStream.RelativePosition;
        }

        protected virtual IEnumerable<FieldLength> MeasureItemsOverride()
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
            return stream.IsAtLimit || stream.AvailableForReading == FieldLength.Zero;
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

        private void SerializeInternal(BoundedStream stream, Func<FieldLength> maxLengthDelegate, EventShuttle eventShuttle, bool measuring)
        {
            stream = PrepareStream(stream, maxLengthDelegate, measuring);

            SerializeOverride(stream, eventShuttle);

            WritePadding(stream);

            FlushStream(stream);
        }

        private async Task SerializeInternalAsync(BoundedStream stream, Func<FieldLength> maxLengthDelegate, EventShuttle eventShuttle,
            CancellationToken cancellationToken)
        {
            stream = PrepareStream(stream, maxLengthDelegate, false);

            await SerializeOverrideAsync(stream, eventShuttle, cancellationToken).ConfigureAwait(false);

            await WritePaddingAsync(stream, cancellationToken).ConfigureAwait(false);

            await FlushStreamAsync(stream, cancellationToken).ConfigureAwait(false);
        }

        private BoundedStream PrepareStream(BoundedStream stream, Func<FieldLength> maxLengthDelegate, bool measuring)
        {
            stream = new BoundedStream(stream, Name, maxLengthDelegate);

            if (!measuring && TypeNode.FieldValueAttributes != null && TypeNode.FieldValueAttributes.Count > 0)
            {
                var context = CreateLazySerializationContext();

                // Setup tap for value attributes if we need to siphon serialized data for later
                for (var index = 0; index < TypeNode.FieldValueAttributes.Count; index++)
                {
                    var fieldValueAttribute = TypeNode.FieldValueAttributes[index];
                    var fieldValueBinding = TypeNode.FieldValueBindings[index];

                    var source = fieldValueBinding.GetSource(this);

                    // Check if this tap has never been created, or if it has been created by this node, in
                    // which case we need to recreate it and reset processing of the source value node.  This
                    // would happen in instances where the tap was previously used inside of a measure override.
                    if (!source._fieldValueAttributeTaps.TryGetValue(fieldValueAttribute, out var tap))
                    {
                        var state = fieldValueAttribute.GetInitialStateInternal(context);
                        tap = new FieldValueAdapterStream(fieldValueAttribute, state);
                        source._fieldValueAttributeTaps[fieldValueAttribute] = tap;
                    }

                    stream = new TapStream(stream, tap, Name);
                }
            }

            return stream;
        }

        private void FlushStream(BoundedStream stream)
        {
            if (TypeNode.FieldValueAttributes != null && TypeNode.FieldValueAttributes.Any())
            {
                stream.Flush();
            }
        }

        private async Task FlushStreamAsync(BoundedStream stream, CancellationToken cancellationToken)
        {
            if (TypeNode.FieldValueAttributes != null && TypeNode.FieldValueAttributes.Any())
            {
                await stream.FlushAsync(cancellationToken).ConfigureAwait(false);
            }
        }

        private void AlignRight(BoundedStream stream, bool pad = false)
        {
            var rightAlignment = GetRightFieldAlignment();
            if (rightAlignment != null)
            {
                Align(stream, rightAlignment, pad);
            }
        }

        private void AlignLeft(BoundedStream stream, bool pad = false)
        {
            var leftAlignment = GetLeftFieldAlignment();
            if (leftAlignment != null)
            {
                Align(stream, leftAlignment, pad);
            }
        }

        private void Align(BoundedStream stream, FieldLength alignment, bool pad = false)
        {
            if (alignment == null)
            {
                throw new ArgumentNullException(nameof(alignment));
            }

            var position = stream.RelativePosition;
            var delta = (alignment - position % alignment) % alignment;

            if (delta == FieldLength.Zero)
            {
                return;
            }

            if (pad)
            {
                var padding = new byte[delta.TotalByteCount];
                stream.Write(padding, delta);
            }
            else
            {
                for (var i = 0; i < (int) delta.ByteCount; i++)
                {
                    if (stream.ReadByte() < 0)
                    {
                        break;
                    }
                }
            }
        }

        private void DeserializeInternal(BoundedStream stream, Func<FieldLength> maxLengthDelegate, SerializationOptions options, EventShuttle eventShuttle)
        {
            stream = PrepareStream(stream, maxLengthDelegate, false);

            DeserializeOverride(stream, options, eventShuttle);

            SkipPadding(stream);

            // needed for tap
            FlushStream(stream);
        }

        private async Task DeserializeInternalAsync(BoundedStream stream, Func<FieldLength> maxLengthDelegate, SerializationOptions options,
            EventShuttle eventShuttle, CancellationToken cancellationToken)
        {
            stream = PrepareStream(stream, maxLengthDelegate, false);

            await DeserializeOverrideAsync(stream, options, eventShuttle, cancellationToken).ConfigureAwait(false);

            await SkipPaddingAsync(stream, cancellationToken).ConfigureAwait(false);

            // needed for tap
            await FlushStreamAsync(stream, cancellationToken).ConfigureAwait(false);
        }

        private void WritePadding(BoundedStream stream)
        {
            ProcessPadding(stream, (s, bytes, length) => s.Write(bytes, length));
        }

        private Task WritePaddingAsync(BoundedStream stream, CancellationToken cancellationToken)
        {
            return ProcessPaddingAsync(stream, async (s, bytes, length) =>
                await s.WriteAsync(bytes, length, cancellationToken).ConfigureAwait(false));
        }

        private void SkipPadding(BoundedStream stream)
        {
            ProcessPadding(stream, (s, bytes, length) => 
                _ = s.Read(bytes, 0, (int)length.ByteCount));
        }

        private Task SkipPaddingAsync(BoundedStream stream, CancellationToken cancellationToken)
        {
            return ProcessPaddingAsync(stream,
                async (s, bytes, length) =>
                    _ = await s.ReadAsync(bytes, 0, (int)length.ByteCount, cancellationToken).ConfigureAwait(false));
        }

        private void ProcessPadding(BoundedStream stream, Action<BoundedStream, byte[], FieldLength> streamOperation)
        {
            var length = GetConstFieldLength();

            if (length == null)
            {
                return;
            }

            if (length > stream.RelativePosition)
            {
                var padLength = length - stream.RelativePosition;
                var pad = new byte[(int) padLength.TotalByteCount];
                streamOperation(stream, pad, padLength);
            }
        }

        private async Task ProcessPaddingAsync(BoundedStream stream, Func<BoundedStream, byte[], FieldLength, Task> streamOperationAsync)
        {
            var length = GetConstFieldLength();

            if (length == null)
            {
                return;
            }

            if (length > stream.RelativePosition)
            {
                var padLength = length - stream.RelativePosition;
                var pad = new byte[(int) padLength.TotalByteCount];
                await streamOperationAsync(stream, pad, padLength).ConfigureAwait(false);
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