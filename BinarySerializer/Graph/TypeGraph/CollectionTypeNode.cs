using System;
using System.Reflection;

namespace BinarySerialization.Graph.TypeGraph
{
    internal abstract class CollectionTypeNode : ContainerTypeNode
    {
        private readonly Lazy<TypeNode> _lazyChild; 

        protected CollectionTypeNode(TypeNode parent, Type type) : base(parent, type)
        {
            object terminationValue;
            TerminationChild = GetTerminationChild(out terminationValue);
            TerminationValue = terminationValue;
            _lazyChild = new Lazy<TypeNode>(() => GenerateChild(ChildType));
        }

        protected CollectionTypeNode(TypeNode parent, Type parentType, MemberInfo memberInfo) : base(parent, parentType, memberInfo)
        {
            object terminationValue;
            TerminationChild = GetTerminationChild(out terminationValue);
            TerminationValue = terminationValue;
            _lazyChild = new Lazy<TypeNode>(() => GenerateChild(ChildType));
        }

        public Type ChildType { get; set; }

        public TypeNode Child
        {
            get { return _lazyChild.Value; }
        }

        public TypeNode TerminationChild { get; private set; }

        public object TerminationValue { get; private set; }

        private TypeNode GetTerminationChild(out object terminationValue)
        {
            if (SerializeUntilAttribute == null)
            {
                terminationValue = null;
                return null;
            }

            terminationValue = SerializeUntilAttribute == null ? null : SerializeUntilAttribute.ConstValue ?? (byte)0;

            TypeNode terminationChild = null;
            if (terminationValue != null)
                terminationChild = GenerateChild(terminationValue.GetType());

            return terminationChild;
        }
    }
}
