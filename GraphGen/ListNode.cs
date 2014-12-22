using System;
using System.Collections;
using System.Linq;
using System.Reflection;

namespace GraphGen
{
    internal class ListNode : CollectionNode
    {
        public ListNode(Node parent, Type type) : base(parent, type)
        {
        }

        public ListNode(Node parent, MemberInfo memberInfo) : base(parent, memberInfo)
        {
        }

        public override object Value
        {
            get
            {
                var list = (IList)Activator.CreateInstance(Type);

                foreach (var child in Children)
                    list.Add(child.Value);

                return list;
            }

            set
            {
                var list = (IList) value;

                if (FieldCountEvaluator.IsConst)
                {
                    /* Pad out const-sized list */
                    var count = (int)FieldCountEvaluator.Value;
                    if (list.Count < count)
                    {
                        var extraChildren = Enumerable.Repeat(Activator.CreateInstance(LazyChildType.Value),
                            count - Children.Count);

                        foreach (var extraChild in extraChildren)
                            list.Add(extraChild);
                    }
                }

                var children = list.Cast<object>().Select(item =>
                {
                    var child = GenerateChild(LazyChildType.Value);
                    child.Value = item;
                    return child;
                });

                Children.AddRange(children);
            }
        }

        protected override Type GetChildType(Type collectionType)
        {
            if (collectionType.GetGenericArguments().Length > 1)
            {
                throw new NotSupportedException("Multiple generic arguments not supported");
            }

            return collectionType.GetGenericArguments().Single();
        }
    }
}
