using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;

namespace GraphGen
{
    internal class ListNode : ContainerNode
    {
        private readonly Type _childType;

        public ListNode(Node parent, Type type) : base(parent, type)
        {
            throw new InvalidOperationException("Must specify field count attribute.");
        }

        public ListNode(Node parent, MemberInfo memberInfo) : base(parent, memberInfo)
        {
            if(FieldCountEvaluator == null)
                throw new InvalidOperationException("Must specify field count attribute.");

            var source = FieldCountEvaluator.Source;
            if (source != null)
            {
                source.Bindings.Add(new Binding(() => Children.Count));
            }

            _childType = GetChildType(Type);
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
                
                /* Pad out list to count specification */
                var count = (int)FieldCountEvaluator.Value;
                if (list.Count < count)
                {
                    var extraChildren = Enumerable.Repeat(Activator.CreateInstance(_childType), count - Children.Count);

                    foreach (var extraChild in extraChildren)
                        list.Add(extraChild);
                }

                var children = list.Cast<object>().Select(item =>
                {
                    var child = GenerateChild(_childType);
                    child.Value = item;
                    return child;
                });

                Children.AddRange(children);
            }
        }

        public override void Serialize(Stream stream)
        {
            foreach(var child in Children)
                child.Serialize(stream);
        }

        public override void Deserialize(Stream stream)
        {
            // TODO StreamLimiter
            Children.Clear();

            var count = FieldCountEvaluator.Value;
            for (ulong i = 0; i < count; i++)
            {
                var node = GenerateChild(_childType);
                Children.Add(node);
            }

            foreach (var child in Children)
                child.Deserialize(stream);
        }

        private static Type GetChildType(Type collectionType)
        {
            if (collectionType.GetGenericArguments().Length > 1)
            {
                throw new NotSupportedException("Multiple generic arguments not supported");
            }

            return collectionType.GetGenericArguments().Single();
        }
    }
}
