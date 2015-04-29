using System;
using System.Collections;
using System.Linq;
using BinarySerialization.Graph.TypeGraph;

namespace BinarySerialization.Graph.ValueGraph
{
    internal class ListValueNode : CollectionValueNode
    {
        private object _cachedValue;

        public ListValueNode(Node parent, string name, TypeNode typeNode) : base(parent, name, typeNode)
        {
        }

        public override object Value
        {
            get
            {
                /* For creating serialization contexts quickly */
                if (_cachedValue != null)
                    return _cachedValue;

                var typeNode = (ListTypeNode)TypeNode;

                var list = (IList) typeNode.Constructor.Invoke(null);

                foreach (var child in Children)
                    list.Add(child.Value);

                return list;
            }

            set
            {
                if (Children.Count > 0)
                    throw new InvalidOperationException("Value already set.");

                if (value == null)
                    return;

                var list = (IList)value;

                var typeNode = (ListTypeNode)TypeNode;

                if (typeNode.FieldCountBinding != null && typeNode.FieldCountBinding.IsConst)
                {
                    /* Pad out const-sized list */
                    var count = Convert.ToInt32(typeNode.FieldCountBinding.GetValue(this));
                    while (list.Count < count)
                    {
                        var item = typeNode.ChildType == typeof (string)
                            ? string.Empty
                            : typeNode.ChildConstructor.Invoke(null);

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
    }
}
