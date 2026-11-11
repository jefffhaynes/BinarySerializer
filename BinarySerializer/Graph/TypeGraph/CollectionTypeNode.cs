using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace BinarySerialization.Graph.TypeGraph
{
    internal abstract class CollectionTypeNode : ContainerTypeNode
    {
        private Lazy<TypeNode> _lazyChild;

        protected CollectionTypeNode(TypeNode parent, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type type) : base(parent, type)
        {
            Construct();
        }

        protected CollectionTypeNode(TypeNode parent, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type parentType,
            MemberInfo memberInfo) : base(parent, parentType,
            memberInfo)
        {
            Construct();
        }

        public Func<object> CompiledConstructor { get; private set; }

        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
        public Type ChildType { get; set; }

        public TypeNode Child => _lazyChild.Value;

        public Func<object> CompiledChildConstructor { get; protected set; }

        public TypeNode TerminationChild { get; private set; }

        public object TerminationValue { get; private set; }

        [return: DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
        protected abstract Type GetChildType();

        [UnconditionalSuppressMessage("Trimming", "IL2072",
            Justification =
                "Termination values are user constants provided via attributes; users of trimmed or AOT-compiled applications must root their serialized types (see README, AOT support).")]
        private void Construct()
        {
            CompiledConstructor = TypeExtensions.CreateListConstructor(Type) ?? CreateCompiledConstructor();

            ChildType = GetChildType();
            CompiledChildConstructor = CreateCompiledConstructor(ChildType);

            TerminationChild = GetTerminationChild(out var terminationValue);
            TerminationValue = terminationValue;

            _lazyChild = new Lazy<TypeNode>(() => GenerateChild(ChildType));
        }

        [UnconditionalSuppressMessage("Trimming", "IL2072",
            Justification =
                "Termination values are user constants provided via attributes; users of trimmed or AOT-compiled applications must root their serialized types (see README, AOT support).")]
        private TypeNode GetTerminationChild(out object terminationValue)
        {
            if (SerializeUntilAttribute == null)
            {
                terminationValue = null;
                return null;
            }

            terminationValue = SerializeUntilAttribute.ConstValue;

            TypeNode terminationChild = null;
            if (terminationValue != null)
            {
                terminationChild = GenerateChild(terminationValue.GetType());
            }

            return terminationChild;
        }
    }
}
