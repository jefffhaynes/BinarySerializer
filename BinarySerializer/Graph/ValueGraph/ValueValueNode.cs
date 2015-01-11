using System;
using System.IO;

namespace BinarySerialization.Graph.ValueGraph
{
    internal class ValueValueNode : ValueNode
    {
        public ValueValueNode(Node parent) : base(parent)
        {
        }

        public ValueValueNode(Node parent, object value) : base(parent)
        {
            Value = value;
        }

        public object Value { get; set; }

        protected override void SerializeOverride(Stream stream)
        {
            throw new NotImplementedException();
        }

        //public object GetBoundValue()
        //{
            
        //}
    }
}
