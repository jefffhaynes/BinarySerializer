using System;
using System.Linq;
using System.Reflection;
using BinarySerialization.Graph.ValueGraph;

namespace BinarySerialization.Graph.TypeGraph
{
    internal sealed class ListTypeNode : CollectionTypeNode
    {
        public ListTypeNode(TypeNode parent, Type type) : base(parent, type)
        {
            ChildType = GetChildType(Type);
            Child = GenerateChild(ChildType);
        }

        public ListTypeNode(TypeNode parent, MemberInfo memberInfo) : base(parent, memberInfo)
        {
            ChildType = GetChildType(Type);
            Child = GenerateChild(ChildType);
        }

        //public override object Value
        //{
        //    get
        //    {
        //        /* Handle primitive case */
        //        if (ChildType.IsPrimitive)
        //            return CollectionValue;

        //        /* Handle object case */
        //        var list = (IList)Activator.CreateInstance(Type);

        //        foreach (var child in Children)
        //            list.Add(child.Value);

        //        return list;
        //    }

        //    set
        //    {
        //        /* Handle primitive case */
        //        if (ChildType.IsPrimitive)
        //        {
        //            CollectionValue = value;
        //            return;
        //        }

        //        /* Handle object case */
        //        ClearChildren();

        //        if (value == null)
        //            return;

        //        var list = (IList) value;

        //        if (FieldCountBinding != null && FieldCountBinding.IsConst)
        //        {
        //            /* Pad out const-sized list */
        //            var count = (int)FieldCountBinding.Value;
        //            if (list.Count < count)
        //            {
        //                var extraChildren = Enumerable.Repeat(CreateChildValue(),
        //                    count - ChildCount);

        //                foreach (var extraChild in extraChildren)
        //                    list.Add(extraChild);
        //            }
        //        }

        //        var children = list.Cast<object>().Select(item =>
        //        {
        //            var child = GenerateChild(ChildType);
        //            child.Value = item;
        //            return child;
        //        });

        //        AddChildren(children);
        //    }
        //}

        private object CreateChildValue()
        {
            return ChildType == typeof(string)
                ? default(string)
                : Activator.CreateInstance(ChildType);
        }

        private Type GetChildType(Type collectionType)
        {
            if (collectionType.GetGenericArguments().Length > 1)
            {
                throw new NotSupportedException("Multiple generic arguments not supported");
            }

            return collectionType.GetGenericArguments().Single();
        }

        //protected override object CreatePrimitiveCollection(int size)
        //{
        //    var array = Array.CreateInstance(ChildType, size);
        //    return Activator.CreateInstance(Type, array);
        //}

        //protected override int GetCollectionCount(object collection)
        //{
        //    throw new NotImplementedException();
        //}

        //protected override int GetCollectionCount()
        //{
        //    var list = (IList) CollectionValue;
        //    return list.Count;
        //}

        //protected override void SetPrimitiveValue(object item, int index)
        //{
        //    var list = (IList)CollectionValue;
        //    list[index] = item;
        //}

        //protected override object GetPrimitiveValue(int index)
        //{
        //    var list = (IList)CollectionValue;
        //    return list[index];
        //}

        public override ValueNode CreateSerializerOverride(ValueNode parent)
        {
            if(ChildType.IsPrimitive || ChildType == typeof(string))
                return new PrimitiveListValueNode(parent, Name, this);
            return new ListValueNode(parent, Name, this);
        }
    }
}
