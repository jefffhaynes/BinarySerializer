﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Reflection;
using BinarySerialization.Graph.TypeGraph;

namespace BinarySerialization.Graph.ValueGraph
{
    internal abstract class ValueNode : Node
    {
        private const char PathSeparator = '.';

        protected ValueNode(Node parent, string name, TypeNode typeNode) : base(parent)
        {
            Name = name;
            TypeNode = typeNode;
            Children = new List<ValueNode>();
            Bindings = new List<Func<object>>();
        }

        public TypeNode TypeNode { get; set; }

        public List<ValueNode> Children { get; set; }

        public List<Func<object>> Bindings { get; set; }

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
                        throw new InvalidOperationException("Binding targets must not be null.");

                    var objectTypeNode = (ObjectTypeNode)typeNode;

                    return objectTypeNode.SubTypeKeys[valueType];
                });
            }

            if (typeNode.ItemSerializeUntilBinding != null && typeNode.ItemSerializeUntilBinding.BindingMode == BindingMode.TwoWay)
            {
                typeNode.ItemSerializeUntilBinding.Bind(this, GetLastItemValueOverride);
            }

            if (typeNode.FieldValueAttribute != null)
            {
                typeNode.FieldValueBinding.Bind(this, () => typeNode.FieldValueAttribute.ComputeFinalInternal());
            }

            foreach (ValueNode child in Children)
                child.Bind();
        }

        protected IEnumerable<ValueNode> GetSerializableChildren()
        {
            return Children.Where(child => !child.TypeNode.IsIgnored);
        }

        public void Serialize(BoundedStream stream, EventShuttle eventShuttle)
        {
            try
            {
                var serializeWhenBindings = TypeNode.SerializeWhenBindings;
                
                if (serializeWhenBindings != null &&
                    !serializeWhenBindings.Any(binding => binding.IsSatisfiedBy(binding.GetBoundValue(this))))
                    return;

                Binding fieldOffsetBinding = TypeNode.FieldOffsetBinding;

                long? maxLength = TypeNode.FieldLengthBinding != null && TypeNode.FieldLengthBinding.IsConst
                    ? (long?) Convert.ToInt64(TypeNode.FieldLengthBinding.ConstValue)
                    : null;

                if (fieldOffsetBinding != null)
                {
                    using (new StreamResetter(stream))
                    {
                        stream.Position = Convert.ToInt64(fieldOffsetBinding.GetValue(this));

                        SerializeInternal(stream, eventShuttle, maxLength);
                    }
                }
                else
                {
                    SerializeInternal(stream, eventShuttle, maxLength);
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

        private void SerializeInternal(BoundedStream stream, EventShuttle eventShuttle, long? maxLength)
        {
            if (maxLength != null)
                stream = new BoundedStream(stream, maxLength.Value);

            // if I need to store serialized data for later...
            if (TypeNode.FieldValueAttribute != null)
            {
                var context = CreateSerializationContext();
                TypeNode.FieldValueAttribute.ResetInternal(context);
                var tap = new FieldValueAdapterStream(TypeNode.FieldValueAttribute);
                stream = new TapStream(stream, tap);
            } 

            SerializeOverride(stream, eventShuttle);

            if(TypeNode.FieldValueAttribute != null)
                stream.Flush();
        }

        // this is internal only because of the weird custom subtype case.  If I can figure out a better
        // way to handle that case, this can be protected.
        internal abstract void SerializeOverride(BoundedStream stream, EventShuttle eventShuttle);

        public void Deserialize(BoundedStream stream, EventShuttle eventShuttle)
        {
            try
            {
                var serializeWhenBindings = TypeNode.SerializeWhenBindings;
                if (serializeWhenBindings != null &&
                    !serializeWhenBindings.Any(binding => binding.IsSatisfiedBy(binding.GetValue(this))))
                    return;
                
                Binding fieldOffsetBinding = TypeNode.FieldOffsetBinding;

                long? maxLength = TypeNode.FieldLengthBinding != null
                    ? (long?)Convert.ToInt64(TypeNode.FieldLengthBinding.GetValue(this))
                    : null;

                if (fieldOffsetBinding != null)
                {
                    using (new StreamResetter(stream))
                    {
                        stream.Position = Convert.ToInt64(fieldOffsetBinding.GetValue(this));

                        DeserializeInternal(stream, maxLength, eventShuttle);
                    }
                }
                else
                {
                    DeserializeInternal(stream, maxLength, eventShuttle);
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
                string message = $"Error deserializing '{reference}'.  See inner exception for detail.";
                throw new InvalidOperationException(message, e);
            }
        }

        private void DeserializeInternal(BoundedStream stream, long? maxLength, EventShuttle eventShuttle)
        {
            if (maxLength != null)
                stream = new BoundedStream(stream, maxLength.Value);

            if (EndOfStream(stream))
            {
#if PORTABLE328
                if (TypeNode.Type.IsPrimitive)
#else
                   if(TypeNode.Type.GetTypeInfo().IsPrimitive)
#endif
                    throw new EndOfStreamException();
            }
            else DeserializeOverride(stream, eventShuttle);
        }

        internal abstract void DeserializeOverride(BoundedStream stream, EventShuttle eventShuttle);

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
            var parent = Parent as ValueNode;
            return new BinarySerializationContext(Value, parent?.Value, parent?.TypeNode.Type, parent?.CreateSerializationContext());
        }

        protected virtual long MeasureOverride()
        {
            var nullStream = new NullStream();
            var boundedStream = new BoundedStream(nullStream);
            Serialize(boundedStream, null);
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
            if (stream.IsAtLimit)
                return true;

            return stream.CanSeek && stream.Position >= stream.Length;
        }
    }
}