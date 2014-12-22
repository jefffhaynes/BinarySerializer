using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace GraphGen
{
    internal abstract class CollectionNode : ContainerNode
    {
        protected readonly Lazy<Type> LazyChildType;

        protected CollectionNode(Node parent, Type type) : base(parent, type)
        {
            throw new InvalidOperationException("Must specify field count attribute.");
        }

        protected CollectionNode(Node parent, MemberInfo memberInfo) : base(parent, memberInfo)
        {
            if (FieldCountEvaluator == null)
                throw new InvalidOperationException("Must specify field count attribute.");

            var source = FieldCountEvaluator.Source;
            if (source != null)
            {
                source.Bindings.Add(new Binding(() => Children.Count));
            }

            LazyChildType = new Lazy<Type>(() => GetChildType(Type));
        }

        public override void Serialize(Stream stream)
        {
            foreach (var child in Children)
                child.Serialize(stream);
        }

        public override void Deserialize(Stream stream)
        {
            // TODO StreamLimiter
            Children.Clear();

            var count = FieldCountEvaluator.Value;
            for (ulong i = 0; i < count; i++)
            {
                var node = GenerateChild(LazyChildType.Value);
                Children.Add(node);
            }

            foreach (var child in Children)
                child.Deserialize(stream);
        }


        protected abstract Type GetChildType(Type collectionType);
    }
}
