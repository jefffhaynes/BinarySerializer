using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BinarySerialization.ValueGraph
{
    public abstract class ValueGraphNode
    {
        protected ValueGraphNode()
        {
        }

        protected ValueGraphNode(IEnumerable<ValueGraphNode> children)
        {
            Children = new List<ValueGraphNode>(children);
        }

        public string Name { get; set; }

        public Type Type { get; set; }

        public List<ValueGraphNode> Children { get; set; }

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
