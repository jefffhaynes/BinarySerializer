using System;
using System.IO;
using System.Linq;
using BinarySerialization.Graph.TypeGraph;

namespace BinarySerialization.Graph.ValueGraph
{
    internal class ArrayValueNode : CollectionValueNode
    {
        public ArrayValueNode(Node parent, string name, TypeNode typeNode) : base(parent, name, typeNode)
        {
        }

        public override object Value
        {
            get
            {
                var typeNode = (ArrayTypeNode)TypeNode;
                var array = Array.CreateInstance(typeNode.ChildType, Children.Count);
                var childValues = Children.Cast<ValueNode>().Select(child => child.Value).ToArray();
                Array.Copy(childValues, array, childValues.Length);
                return array;
            }

            set
            {
                if(Children.Any())
                    throw new InvalidOperationException("Value already set.");

                if (value == null)
                    return;

                var typeNode = (ArrayTypeNode)TypeNode;

                var array = (Array)value;

                if (typeNode.FieldCountBinding != null && typeNode.FieldCountBinding.IsConst)
                {
                    /* Pad out const-sized array */
                    var count = (int)typeNode.FieldCountBinding.GetValue(this);
                    var valueArray = Array.CreateInstance(typeNode.ChildType, count);
                    Array.Copy(array, valueArray, array.Length);
                    array = valueArray;
                }

                var children = array.Cast<object>().Select(childValue =>
                {
                    var child = typeNode.Child.CreateSerializer(this);
                    child.Value = childValue;
                    return child;
                });

                Children.AddRange(children);
            }
        }
    }
}
