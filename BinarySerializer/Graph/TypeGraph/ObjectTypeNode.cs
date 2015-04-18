using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BinarySerialization.Graph.ValueGraph;

namespace BinarySerialization.Graph.TypeGraph
{
    internal class ObjectTypeNode : ContainerTypeNode
    {
        private const BindingFlags MemberBindingFlags =
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly;

        private const BindingFlags ConstructorBindingFlags = BindingFlags.Instance | BindingFlags.Public;

        public ObjectTypeNode(TypeNode parent, Type type) : base(parent, type)
        {
            Construct();
        }

        public ObjectTypeNode(TypeNode parent, Type parentType, MemberInfo memberInfo)
            : base(parent, parentType, memberInfo)
        {
            Construct();
        }

        private IDictionary<Type, SubTypeInfo> _subTypes;
        private readonly object _subTypesLock = new object();

        public IDictionary<Type, object> SubTypeKeys { get; private set; }

        private void Construct()
        {
            if (IgnoreAttribute != null)
                return;

            _subTypes = new Dictionary<Type, SubTypeInfo> {{Type, CreateSubTypeInfo(Type)}};

            /* Add subtypes, if any */
            if (SubtypeAttributes == null || SubtypeAttributes.Count <= 0)
                return;

            /* Get subtype keys */
            if(SubtypeBinding.BindingMode == BindingMode.TwoWay)
                SubTypeKeys = SubtypeAttributes.ToDictionary(attribute => attribute.Subtype, attribute => attribute.Value);

            /* Generate subtype children */
            IEnumerable<Type> subTypes = SubtypeAttributes.Select(attribute => attribute.Subtype);

            foreach (Type subType in subTypes)
                GenerateChildren(subType);
        }

        public override ValueNode CreateSerializerOverride(ValueNode parent)
        {
            return new ObjectValueNode(parent, Name, this);
        }

        private void GenerateChildren(Type type)
        {
            lock (_subTypesLock)
            {
                if (!_subTypes.ContainsKey(type))
                {
                    _subTypes.Add(type, CreateSubTypeInfo(type));
                }
            }
        }

        public SubTypeInfo GetSubType(Type type)
        {
            lock (_subTypesLock)
            {
                /* This is a type we've never seen before so let's update our reference type. */
                if (!_subTypes.ContainsKey(type))
                    _subTypes.Add(type, CreateSubTypeInfo(type));

                return _subTypes[type];
            }
        }

        private SubTypeInfo CreateSubTypeInfo(Type type)
        {
            var parameterlessConstructor = type.GetConstructor(new Type[0]);
            var constructors = type.GetConstructors(ConstructorBindingFlags);
            var children = GenerateChildrenImpl(type);
            return new SubTypeInfo(parameterlessConstructor, constructors, children);
        }

        private IEnumerable<TypeNode> GenerateChildrenImpl(Type parentType)
        {
            IEnumerable<MemberInfo> properties = parentType.GetProperties(MemberBindingFlags);
            IEnumerable<MemberInfo> fields = parentType.GetFields(MemberBindingFlags);
            IEnumerable<MemberInfo> all = properties.Union(fields);

            List<TypeNode> children =
                all.Select(memberInfo => GenerateChild(parentType, memberInfo)).OrderBy(child => child.Order).ToList();

            List<TypeNode> serializableChildren = children.Where(child => child.IgnoreAttribute == null).ToList();

            if (serializableChildren.Count > 1)
            {
                TypeNode unorderedChild = serializableChildren.FirstOrDefault(child => child.Order == null);

                if (unorderedChild != null)
                    throw new InvalidOperationException(
                        string.Format(
                            "'{0}' does not have a FieldOrder attribute.  " +
                            "All serializable fields or properties in a class with more than one member must specify a FieldOrder attribute.",
                            unorderedChild.Name));

                IEnumerable<IGrouping<int?, TypeNode>> orderGroups = serializableChildren.GroupBy(child => child.Order);

                if (orderGroups.Count() != serializableChildren.Count)
                    throw new InvalidOperationException("All fields must have a unique order number.");
            }

            if (parentType.BaseType != null)
            {
                IEnumerable<TypeNode> baseChildren = GenerateChildrenImpl(parentType.BaseType);
                return baseChildren.Concat(children);
            }

            return children;
        }
    }
}