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

        public ObjectTypeNode(TypeNode parent, Type type) : base(parent, type)
        {
            Construct();
        }

        public ObjectTypeNode(TypeNode parent, MemberInfo memberInfo) : base(parent, memberInfo)
        {
            Construct();
        }

        private IDictionary<Type, List<TypeNode>> _typeChildren;
        private readonly object _typeChildrenLock = new object();

        public IDictionary<Type, object> SubTypeKeys { get; private set; }

        private void Construct()
        {
            if (IgnoreAttribute != null)
                return;

            List<TypeNode> baseChildren = GenerateChildrenImpl(Type).ToList();

            _typeChildren = new Dictionary<Type, List<TypeNode>> {{Type, baseChildren}};

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
            lock (_typeChildrenLock)
            {
                if (!_typeChildren.ContainsKey(type))
                    _typeChildren.Add(type, GenerateChildrenImpl(type).ToList());
            }
        }

        public IEnumerable<TypeNode> GetTypeChildren(Type type)
        {
            lock (_typeChildrenLock)
            {
                /* If this is a type we've never seen before so let's update our reference type. */
                if (!_typeChildren.ContainsKey(type))
                    _typeChildren.Add(type, GenerateChildrenImpl(type).ToList());

                return _typeChildren[type];
            }
        }

        private IEnumerable<TypeNode> GenerateChildrenImpl(Type type)
        {
            IEnumerable<MemberInfo> properties = type.GetProperties(MemberBindingFlags);
            IEnumerable<MemberInfo> fields = type.GetFields(MemberBindingFlags);
            IEnumerable<MemberInfo> all = properties.Union(fields);

            List<TypeNode> children = all.Select(GenerateChild).OrderBy(child => child.Order).ToList();

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

            if (type.BaseType != null)
            {
                IEnumerable<TypeNode> baseChildren = GenerateChildrenImpl(type.BaseType);
                return baseChildren.Concat(children);
            }

            return children;
        }
    }
}