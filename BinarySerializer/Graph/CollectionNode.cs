using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace BinarySerialization.Graph
{
    internal abstract class CollectionNode : ContainerNode
    {
        protected readonly Lazy<Type> LazyChildType;
        private readonly object _terminationValue;

        protected CollectionNode(Node parent, Type type) : base(parent, type)
        {
            LazyChildType = new Lazy<Type>(() => GetChildType(Type));
            _terminationValue = GetTerminationValue();
        }

        protected CollectionNode(Node parent, MemberInfo memberInfo) : base(parent, memberInfo)
        {
            LazyChildType = new Lazy<Type>(() => GetChildType(Type));
            _terminationValue = GetTerminationValue();
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

            /* Handle termination case */
            if (_terminationValue != null)
            {
                Node terminationChild = GenerateChild(_terminationValue.GetType());
                terminationChild.Value = _terminationValue;
                terminationChild.Serialize(stream);
            }
        }

        public override void DeserializeOverride(StreamLimiter stream)
        {
            if (FieldLengthBinding != null)
                stream = new StreamLimiter(stream, (long)FieldLengthBinding.Value);

            ClearChildren();

            /* Prep termination case */
            Node terminationChild = null;
            if (_terminationValue != null)
                terminationChild = GenerateChild(_terminationValue.GetType());

            var count = FieldCountBinding != null ? FieldCountBinding.Value : ulong.MaxValue;
            for (ulong i = 0; i < count; i++)
            {
                if (ShouldTerminate(stream))
                    break;

                /* Check termination case */
                if (terminationChild != null)
                {
                    using (var streamResetter = new StreamResetter(stream))
                    {
                        terminationChild.Deserialize(stream);

                        if (terminationChild.Value.Equals(_terminationValue))
                        {
                            streamResetter.CancelReset();
                            break;
                        }
                    }
                }

                // TODO why am I generating the same type information over and over?
                var child = GenerateChild(LazyChildType.Value);

                var childStream = child.ItemLengthBinding != null
                    ? new StreamLimiter(stream, (long) child.ItemLengthBinding.Value)
                    : stream;

                child.Deserialize(childStream);

                /* Check item termination case */
                if (ItemSerializeUntilBinding != null)
                {
                    var itemTerminationValue = ItemSerializeUntilBinding.Value;
                    var itemTerminationChild = child.GetChild(ItemSerializeUntilAttribute.ItemValuePath);

                    if (itemTerminationChild.Value.Equals(itemTerminationValue))
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

            Bind();
        }

        private object GetTerminationValue()
        {
            if (SerializeUntilAttribute == null) 
                return null;

            return SerializeUntilAttribute.ConstValue ?? (byte)0;
        }

        protected abstract Type GetChildType(Type collectionType);
    }
}
