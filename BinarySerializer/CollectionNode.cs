using System;
using System.IO;
using System.Reflection;

namespace BinarySerialization
{
    internal abstract class CollectionNode : ContainerNode
    {
        protected readonly Lazy<Type> LazyChildType;

        protected CollectionNode(Node parent, Type type) : base(parent, type)
        {
        }

        protected CollectionNode(Node parent, MemberInfo memberInfo) : base(parent, memberInfo)
        {
            if (FieldCountEvaluator != null)
            {
                var source = FieldCountEvaluator.Source;
                if (source != null)
                {
                    source.Bindings.Add(new Binding(() => Children.Count));
                }
            }

            LazyChildType = new Lazy<Type>(() => GetChildType(Type));
        }

        public override void Serialize(Stream stream)
        {
            foreach (var child in Children)
                child.Serialize(stream);
        }

        public override void Deserialize(StreamLimiter stream)
        {
            if (FieldLengthEvaluator != null)
                stream = new StreamLimiter(stream, (long)FieldLengthEvaluator.Value);

            Children.Clear();

            var count = FieldCountEvaluator != null ? FieldCountEvaluator.Value : ulong.MaxValue;
            for (ulong i = 0; i < count; i++)
            {
                if (ShouldTerminate(stream))
                    break;

                var child = GenerateChild(LazyChildType.Value);
                Children.Add(child);
                child.Deserialize(stream);
            }
        }


        protected abstract Type GetChildType(Type collectionType);
    }
}
