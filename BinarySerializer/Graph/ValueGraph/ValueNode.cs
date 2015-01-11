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

        public Encoding Encoding { get { return TypeNode.Encoding; } }

        public Endianness Endianness { get { return TypeNode.Endianness; } }

        public abstract object Value { get; set; }

        public void Bind()
        {
            if (TypeNode.FieldLengthBinding != null)
            {
                TypeNode.FieldLengthBinding.Bind(this, () => MeasureOverride());
            }

            foreach(var child in Children.Cast<ValueNode>())
                child.Bind();
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
                    ? string.Format("graphType '{0}'", TypeNode.Type)
                    : string.Format("member '{0}'", Name);
                string message = string.Format("Error serializing {0}.  See inner exception for detail.", reference);
                throw new InvalidOperationException(message, e);
            }
        }

        protected abstract void SerializeOverride(Stream stream);

        public virtual void Deserialize(Stream stream)
        {
            try
            {
                DeserializeOverride(stream);
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

        public abstract void DeserializeOverride(Stream stream);


        protected virtual long MeasureOverride()
        {
            var nullStream = new NullStream();
            var streamKeeper = new StreamKeeper(nullStream);
            Serialize(streamKeeper);
            return streamKeeper.RelativePosition;
        }
    }
}
