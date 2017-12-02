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

        private bool ShouldSerialize(Func<Binding, object> bindingValueSelector)
        {
            if (TypeNode.SerializeWhenBindings != null &&
                !TypeNode.SerializeWhenBindings.Any(binding => binding.IsSatisfiedBy(bindingValueSelector(binding))))
            {
                return false;
            }

            if (TypeNode.SerializeWhenNotBindings != null &&
                TypeNode.SerializeWhenNotBindings.All(binding => binding.IsSatisfiedBy(bindingValueSelector(binding))))
            {
                return false;
            }

            return true;
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
                    var fieldValueAttribute = typeNode.FieldValueAttributes[index];
                    
                    fieldValueBinding.Bind(this, () =>
                    {
                        if (!Visited)
                        {
                            throw new InvalidOperationException(
                                "Reverse binding not allowed on FieldValue attributes.  Consider swapping source and target.");
                        }

                        return fieldValueAttribute.ComputeFinalInternal();
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
                if (!ShouldSerialize(binding => binding.GetBoundValue(this)))
                {
                    return;
                }

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
                        SerializeInternal(stream, GetConstFieldLength, eventShuttle);
                    }
                }
                else
                {
                    SerializeInternal(stream, GetConstFieldLength, eventShuttle);
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
                if (!ShouldSerialize(binding => binding.GetBoundValue(this)))
                {
                    return;
                }

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

        internal void Deserialize(BoundedStream stream, EventShuttle eventShuttle)
        {
            try
            {
                if (!ShouldSerialize(binding => binding.GetValue(this)))
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
                ThrowDeserializationException(e);
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
                if (!ShouldSerialize(binding => binding.GetValue(this)))
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
                ThrowDeserializationException(e);
            }
            finally
            {
                Visited = true;
            }
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
            stream = PrepareStream(stream, maxLengthDelegate);

            SerializeOverride(stream, eventShuttle);

            WritePadding(stream);

            FlushStream(stream);
        }

        private async Task SerializeInternalAsync(BoundedStream stream, Func<long?> maxLengthDelegate, EventShuttle eventShuttle,
            CancellationToken cancellationToken)
        {
            stream = PrepareStream(stream, maxLengthDelegate);

            await SerializeOverrideAsync(stream, eventShuttle, cancellationToken).ConfigureAwait(false);

            await WritePaddingAsync(stream, cancellationToken).ConfigureAwait(false);

            await FlushStreamAsync(stream, cancellationToken).ConfigureAwait(false);
        }

        private BoundedStream PrepareStream(BoundedStream stream, Func<long?> maxLengthDelegate)
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

            await DeserializeOverrideAsync(stream, eventShuttle, cancellationToken).ConfigureAwait(false);

            await SkipPaddingAsync(stream, cancellationToken).ConfigureAwait(false);
        }

        private void WritePadding(BoundedStream stream)
        {
            ProcessPadding(stream, (s, bytes, length) => s.Write(bytes, 0, length));
        }

        private Task WritePaddingAsync(BoundedStream stream, CancellationToken cancellationToken)
        {
            return ProcessPaddingAsync(stream, async (s, bytes, length) =>
                await s.WriteAsync(bytes, 0, length, cancellationToken).ConfigureAwait(false));
        }

        private void SkipPadding(BoundedStream stream)
        {
            ProcessPadding(stream, (s, bytes, length) => s.Read(bytes, 0, length));
        }

        private Task SkipPaddingAsync(BoundedStream stream, CancellationToken cancellationToken)
        {
            return ProcessPaddingAsync(stream,
                async (s, bytes, length) =>
                    await s.ReadAsync(bytes, 0, length, cancellationToken).ConfigureAwait(false));
        }

        private void ProcessPadding(BoundedStream stream, Action<Stream, byte[], int> streamOperation)
        {
            var length = GetConstFieldLength();

            if (length == null)
            {
                return;
            }

            if (length > stream.RelativePosition)
            {
                var padLength = length - stream.RelativePosition;
                var pad = new byte[(int) padLength];
                streamOperation(stream, pad, pad.Length);
            }
        }

        private async Task ProcessPaddingAsync(BoundedStream stream, Func<Stream, byte[], int, Task> streamOperationAsync)
        {
            var length = GetConstFieldLength();

            if (length == null)
            {
                return;
            }

            if (length > stream.RelativePosition)
            {
                var padLength = length - stream.RelativePosition;
                var pad = new byte[(int)padLength];
                await streamOperationAsync(stream, pad, pad.Length).ConfigureAwait(false);
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