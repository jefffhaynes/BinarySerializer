using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace BinarySerialization.Graph
{
    internal abstract class CollectionNode : ContainerNode
    {
        protected Type ChildType { get; set; }

        /// <summary>
        /// For primitive collections.
        /// </summary>
        protected object CollectionValue { get; set; }

        private readonly object _terminationValue;

        protected CollectionNode(Node parent, Type type) : base(parent, type)
        {
            _terminationValue = GetTerminationValue();
        }

        protected CollectionNode(Node parent, MemberInfo memberInfo) : base(parent, memberInfo)
        {
            _terminationValue = GetTerminationValue();
        }

        protected override long CountNodeOverride()
        {
            return ChildType.IsPrimitive ? GetCollectionCount() : ChildCount;
        }

        protected override object GetLastItemValueOverride()
        {
            var lastItem = Children.Last();
            return lastItem.GetChild(ItemSerializeUntilAttribute.ItemValuePath);
        }

        public override void SerializeOverride(Stream stream)
        {
            if (ChildType.IsPrimitive)
            {
                SerializePrimitiveCollection(stream);
            }
            else
            {
                SerializeObjectCollection(stream);
            }
        }

        private void SerializePrimitiveCollection(Stream stream)
        {
            var dummyChild = (ValueNode)GenerateChild(ChildType);

            int count = GetCollectionCount();

            var writer = new EndianAwareBinaryWriter(stream, Endianness);
            var childSerializedType = dummyChild.GetSerializedType();

            for (int i = 0; i < count; i++)
            {
                var value = GetPrimitiveValue(i);
                dummyChild.Serialize(writer, value, childSerializedType);
            }
        }

        private void SerializeObjectCollection(Stream stream)
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

            var count = FieldCountBinding != null ? (int)FieldCountBinding.Value : int.MaxValue;

            if (ChildType.IsPrimitive)
            {
                DeserializePrimitiveCollection(stream, count);
            }
            else
            {
                DeserializeObjectCollection(stream, count);
            }
        }

        private void DeserializePrimitiveCollection(StreamLimiter stream, int count)
        {
            /* Create temporary list */
            Type collectionType = typeof(List<>).MakeGenericType(ChildType);
            var collection = (IList)Activator.CreateInstance(collectionType);

            /* Create single child to do all the work */
            var dummyChild = (ValueNode)GenerateChild(ChildType);

            var reader = new EndianAwareBinaryReader(stream, Endianness);
            var childSerializedType = dummyChild.GetSerializedType();

            int itemCount = 0;
            for (int i = 0; i < count; i++)
            {
                if (ShouldTerminate(stream))
                    break;

                var value = dummyChild.Deserialize(reader, childSerializedType);
                collection.Add(value);

                itemCount++;
            }

            /* Create final collection */
            CollectionValue = CreatePrimitiveCollection(itemCount);

            /* Copy temp list into final collection */
            for (int i = 0; i < itemCount; i++)
            {
                SetPrimitiveValue(collection[i], i);
            }
        }

        private void DeserializeObjectCollection(StreamLimiter stream, int count)
        {
            ClearChildren();

            /* Prep termination case */
            Node terminationChild = null;
            if (_terminationValue != null)
                terminationChild = GenerateChild(_terminationValue.GetType());


            for (int i = 0; i < count; i++)
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

                // TODO why am I generating the same graphType information over and over?
                var child = GenerateChild(ChildType);

                var childStream = child.ItemLengthBinding != null
                    ? new StreamLimiter(stream, (long)child.ItemLengthBinding.Value)
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

        protected abstract object CreatePrimitiveCollection(int size);

        protected abstract int GetCollectionCount();

        protected abstract void SetPrimitiveValue(object item, int index);

        protected abstract object GetPrimitiveValue(int index);
    }
}
