using System;
using System.IO;
using System.Linq;
using System.Text;
using BinarySerialization.Graph.TypeGraph;

namespace BinarySerialization.Graph.ValueGraph
{
    internal abstract class ValueNode : Node
    {
        protected ValueNode(Node parent, string name, TypeNode typeNode) : base(parent)
        {
            Name = name;
            TypeNode = typeNode;
        }

        public TypeNode TypeNode { get; set; }

        public Encoding Encoding
        {
            get { return TypeNode.Encoding; }
        }

        public Endianness Endianness
        {
            get { return TypeNode.Endianness; }
        }

        public abstract object Value { get; set; }

        public void Bind()
        {
            if (TypeNode.FieldLengthBinding != null)
            {
                TypeNode.FieldLengthBinding.Bind<ValueNode>(this, () => MeasureOverride());
            }

            if (TypeNode.FieldCountBinding != null)
            {
                TypeNode.FieldCountBinding.Bind<ValueNode>(this, () => CountOverride());
            }

            if (TypeNode.SubtypeBinding != null)
            {
                TypeNode.SubtypeBinding.Bind<ValueNode>(this, () =>
                {
                    Type valueType = GetValueTypeOverride();
                    if(valueType == null)
                        return null;

                    var typeNode = (ObjectTypeNode) TypeNode;

                    return typeNode.SubTypeKeys[valueType];
                });
            }

            foreach (ValueNode child in Children.Cast<ValueNode>())
                child.Bind();
        }


        public virtual void Serialize(Stream stream)
        {
            try
            {
                Binding fieldOffsetBinding = TypeNode.FieldOffsetBinding;

                using (new StreamResetter(stream, fieldOffsetBinding != null))
                {
                    if (fieldOffsetBinding != null)
                        stream.Position = Convert.ToInt64(fieldOffsetBinding.GetValue(this));

                    SerializeOverride(stream);
                }
            }
            catch (IOException)
            {
                throw;
            }
            catch (Exception e)
            {
                string reference = Name == null
                    ? string.Format("graphType '{0}'", TypeNode.Type)
                    : string.Format("member '{0}'", Name);
                string message = string.Format("Error serializing {0}.  See inner exception for detail.", reference);
                throw new InvalidOperationException(message, e);
            }
        }

        protected abstract void SerializeOverride(Stream stream);

        public virtual void Deserialize(StreamLimiter stream)
        {
            try
            {
                if (TypeNode.FieldLengthBinding != null)
                    stream = new StreamLimiter(stream, Convert.ToInt64(TypeNode.FieldLengthBinding.GetValue(this)));

                Binding fieldOffsetBinding = TypeNode.FieldOffsetBinding;

                using (new StreamResetter(stream, fieldOffsetBinding != null))
                {
                    if (fieldOffsetBinding != null)
                        stream.Position = Convert.ToInt64(fieldOffsetBinding.GetValue(this));

                    DeserializeOverride(stream);
                }
            }
            catch (EndOfStreamException e)
            {
                string reference = Name == null
                    ? string.Format("type '{0}'", TypeNode.Type)
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

        public abstract void DeserializeOverride(StreamLimiter stream);


        protected virtual long MeasureOverride()
        {
            var nullStream = new NullStream();
            var streamKeeper = new StreamKeeper(nullStream);
            Serialize(streamKeeper);
            return streamKeeper.RelativePosition;
        }

        protected virtual long CountOverride()
        {
            throw new NotSupportedException("Can't count this field.");
        }

        protected virtual Type GetValueTypeOverride()
        {
            throw new NotSupportedException("Can't set subtypes on this field.");
        }

        protected static bool ShouldTerminate(StreamLimiter stream)
        {
            if (stream.IsAtLimit)
                return true;

            return stream.CanSeek && stream.Position >= stream.Length;
        }
    }
}