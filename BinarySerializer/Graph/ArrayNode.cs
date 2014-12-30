using System;
using System.Linq;
using System.Reflection;

namespace BinarySerialization.Graph
{
    internal class ArrayNode : CollectionNode
    {
        public ArrayNode(Node parent, Type type) : base(parent, type)
        {
        }

        public ArrayNode(Node parent, MemberInfo memberInfo) : base(parent, memberInfo)
        {
        }

        public override object Value
        {
            get
            {
                var array = Array.CreateInstance(LazyChildType.Value, ChildCount);
                var childValues = Children.Select(child => child.Value).ToArray();
                Array.Copy(childValues, array, childValues.Length);
                return array;
            }

            set
            {
                ClearChildren();

                if (value == null)
                    return;

                var array = (Array)value;

                if (FieldCountBinding != null && FieldCountBinding.IsConst)
                {
                    /* Pad out const-sized array */
                    var count = (int)FieldCountBinding.Value;
                    var valueArray = Array.CreateInstance(LazyChildType.Value, count);
                    Array.Copy(array, valueArray, array.Length);
                    array = valueArray;
                }

                var children = array.Cast<object>().Select(childValue =>
                {
                    var child = GenerateChild(LazyChildType.Value);
                    child.Value = childValue;
                    return child;
                });

                AddChildren(children);
            }
        }

        protected override Type GetChildType(Type collectionType)
        {
            return collectionType.GetElementType();
        }
    }
}
