using System;
using System.Linq;
using System.Reflection;

namespace BinarySerialization
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
                var array = Array.CreateInstance(LazyChildType.Value, Children.Count);
                var childValues = Children.Select(child => child.Value).ToArray();
                Array.Copy(childValues, array, childValues.Length);
                return array;
            }

            set
            {
                var array = (Array)value;

                if (FieldCountEvaluator.IsConst)
                {
                    /* Pad out const-sized array */
                    var count = (int)FieldCountEvaluator.Value;
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

                Children.AddRange(children);
            }
        }

        protected override Type GetChildType(Type collectionType)
        {
            return collectionType.GetElementType();
        }
    }
}
