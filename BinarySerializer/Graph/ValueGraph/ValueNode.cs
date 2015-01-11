using System;
using System.IO;

namespace BinarySerialization.Graph.ValueGraph
{
    internal abstract class ValueNode : Node
    {
        protected ValueNode(Node parent) : base(parent)
        {
        }

        public Type Type { get; set; }

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
                    ? string.Format("graphType '{0}'", Type)
                    : string.Format("member '{0}'", Name);
                string message = string.Format("Error serializing {0}.  See inner exception for detail.", reference);
                throw new InvalidOperationException(message, e);
            }
        }

        protected abstract void SerializeOverride(Stream stream);

    }
}
