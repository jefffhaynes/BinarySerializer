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
            LazyChildType = new Lazy<Type>(() => GetChildType(Type));
        }

        protected override long CountNodeOverride()
        {
            return Children.Count;
        }

        public override void Serialize(Stream stream)
        {
            foreach (var child in Children)
                child.Serialize(stream);
        }

        public override void Deserialize(StreamLimiter stream)
        {
            if (FieldLengthBinding != null)
                stream = new StreamLimiter(stream, (long)FieldLengthBinding.Value);

            Unbind();
            Children.Clear();
            Bind();

            var count = FieldCountBinding != null ? FieldCountBinding.Value : ulong.MaxValue;
            for (ulong i = 0; i < count; i++)
            {
                if (ShouldTerminate(stream))
                    break;

                var child = GenerateChild(LazyChildType.Value);
                child.Bind();
                Children.Add(child);

                var childStream = child.ItemLengthBinding != null
                    ? new StreamLimiter(stream, (long) child.ItemLengthBinding.Value)
                    : stream;

                child.Deserialize(childStream);
            }
        }

        protected abstract Type GetChildType(Type collectionType);
    }
}
