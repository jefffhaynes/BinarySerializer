using System;
using System.Linq;
using System.Reflection;

namespace BinarySerialization.TypeGraph
{
    internal sealed class ArrayNode : CollectionNode
    {
        public ArrayNode(Node parent, Type type) : base(parent, type)
        {
            ChildType = Type.GetElementType();
        }

        public ArrayNode(Node parent, MemberInfo memberInfo) : base(parent, memberInfo)
        {
            ChildType = Type.GetElementType();
        }

        public override object Value
        {
            get
            {
                /* Handle primitive case */
                if (ChildType.IsPrimitive)
                    return CollectionValue;

                /* Handle object case */
                var array = Array.CreateInstance(ChildType, ChildCount);
                var childValues = Children.Select(child => child.Value).ToArray();
                Array.Copy(childValues, array, childValues.Length);
                return array;
            }

            set
            {
                /* Handle primitive case */
                if (ChildType.IsPrimitive)
                {
                    CollectionValue = value;
                    return;
                }

                /* Handle object case */
                ClearChildren();

                if (value == null)
                    return;

                var array = (Array) value;

                if (FieldCountBinding != null && FieldCountBinding.IsConst)
                {
                    /* Pad out const-sized array */
                    var count = (int) FieldCountBinding.Value;
                    var valueArray = Array.CreateInstance(ChildType, count);
                    Array.Copy(array, valueArray, array.Length);
                    array = valueArray;
                }

                var children = array.Cast<object>().Select(childValue =>
                {
                    var child = GenerateChild(ChildType);
                    child.Value = childValue;
                    return child;
                });

                AddChildren(children);
            }
        }

        protected override object CreatePrimitiveCollection(int size)
        {
            return Array.CreateInstance(ChildType, size);
        }

        protected override void SetPrimitiveValue(object item, int index)
        {
            var array = (Array)CollectionValue;
            array.SetValue(item, index);
        }

        protected override int GetCollectionCount()
        {
            var array = (Array)CollectionValue;
            return array.Length;
        }

        protected override object GetPrimitiveValue(int index)
        {
            var array = (Array)CollectionValue;
            return array.GetValue(index);
        }
    }
}
