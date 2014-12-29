using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace BinarySerialization
{
    internal abstract class CollectionNode : ContainerNode
    {
        protected readonly Lazy<Type> LazyChildType;

        protected CollectionNode(Node parent, Type type) : base(parent, type)
        {
            LazyChildType = new Lazy<Type>(() => GetChildType(Type));
        }

        protected CollectionNode(Node parent, MemberInfo memberInfo) : base(parent, memberInfo)
        {
            LazyChildType = new Lazy<Type>(() => GetChildType(Type));
        }

        protected override long CountNodeOverride()
        {
            return ChildCount;
        }

        protected override object GetLastItemValueOverride()
        {
            var lastItem = Children.Last();
            return lastItem.GetChild(ItemSerializeUntilAttribute.ItemValuePath);
        }

        public override void SerializeOverride(Stream stream)
        {
            foreach (var child in Children)
                child.Serialize(stream);
        }

        public override void DeserializeOverride(StreamLimiter stream)
        {
            if (FieldLengthBinding != null)
                stream = new StreamLimiter(stream, (long)FieldLengthBinding.Value);

            ClearChildren();

            var count = FieldCountBinding != null ? FieldCountBinding.Value : ulong.MaxValue;
            for (ulong i = 0; i < count; i++)
            {
                if (ShouldTerminate(stream))
                    break;

                // TODO why am I generating the same type information over and over?
                var child = GenerateChild(LazyChildType.Value);

                child.Bind();

                var childStream = child.ItemLengthBinding != null
                    ? new StreamLimiter(stream, (long) child.ItemLengthBinding.Value)
                    : stream;

                child.Deserialize(childStream);

                /* Check item termination case */
                if (ItemSerializeUntilBinding != null)
                {
                    var terminationValue = ItemSerializeUntilBinding.Value;
                    var terminationChild = child.GetChild(ItemSerializeUntilAttribute.ItemValuePath);

                    if (terminationChild.Value.Equals(terminationValue))
                    {
                        if (!ItemSerializeUntilAttribute.ExcludeLastItem)
                        {
                            AddChild(child);
                        }
                        break;
                    }
                }

                AddChild(child);
            }
        }

        protected abstract Type GetChildType(Type collectionType);
    }
}
