using System.Collections;
using System.Linq;
using BinarySerialization.Graph.TypeGraph;

namespace BinarySerialization.Graph.ValueGraph
{
    internal class ListValueNode : CollectionValueNode
    {
        private object _cachedValue;

        public ListValueNode(ValueNode parent, string name, TypeNode typeNode) : base(parent, name, typeNode)
        {
        }

        public override object Value
        {
            get
            {
                /* For creating serialization contexts quickly */
                if (_cachedValue != null)
                {
                    return _cachedValue;
                }

                var typeNode = (ListTypeNode) TypeNode;

                var list = (IList) typeNode.CompiledConstructor();

                foreach (var child in Children)
                {
                    list.Add(child.Value);
                }

                return list;
            }

            set
            {
                if (value == null)
                {
                    return;
                }

                var list = (IList) value;

                var typeNode = (ListTypeNode) TypeNode;

                var count = GetConstFieldCount();

                if (count != null)
                {
                    /* Pad out const-sized list */
                    while (list.Count < count)
                    {
                        var item = typeNode.ChildType == typeof(string)
                            ? string.Empty
                            : typeNode.CompiledChildConstructor();

                        list.Add(item);
                    }
                }

                var children = list.Cast<object>().Select(childValue =>
                {
                    var child = typeNode.Child.CreateSerializer(this);
                    child.Value = childValue;
                    return child;
                });

                Children.AddRange(children);

                /* For creating serialization contexts quickly */
                _cachedValue = value;
            }
        }

        public override object BoundValue
        {
            get
            {
                var typeNode = (ListTypeNode) TypeNode;

                var list = (IList) typeNode.CompiledConstructor();

                foreach (var child in Children)
                {
                    list.Add(child.BoundValue);
                }

                return list;
            }
        }
    }
}