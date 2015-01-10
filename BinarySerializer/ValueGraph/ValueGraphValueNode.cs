using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BinarySerialization.ValueGraph
{
    public class ValueGraphValueNode : ValueGraphNode
    {
        public ValueGraphValueNode()
        {
        }

        public ValueGraphValueNode(object value)
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
