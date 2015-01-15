using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BinarySerialization.Graph.ValueGraph;

namespace BinarySerialization.Graph.TypeGraph
{
    internal class ObjectTypeNode : ContainerTypeNode
    {
        private const BindingFlags MemberBindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly;

        public ObjectTypeNode(TypeNode parent, Type type) : base(parent, type)
        {
            Construct();
        }

        public ObjectTypeNode(TypeNode parent, MemberInfo memberInfo) : base(parent, memberInfo)
        {
            Construct();
        }

        public IDictionary<Type, List<TypeNode>> TypeChildren { get; private set; }

        public IDictionary<Type, object> SubTypeKeys { get; private set; }

        private void Construct()
        {
            var baseChildren = GenerateChildrenImpl(Type).ToList();

            TypeChildren = new Dictionary<Type, List<TypeNode>> {{Type, baseChildren}};

            /* Add subtypes, if any */
            if (SubtypeAttributes == null || SubtypeAttributes.Count <= 0)
                return;

            /* Get subtype keys */
            SubTypeKeys = SubtypeAttributes.ToDictionary(attribute => attribute.Subtype, attribute => attribute.Value);

            /* Generate subtype children */
            var subTypes = SubtypeAttributes.Select(attribute => attribute.Subtype);

            foreach (var subType in subTypes)
                TypeChildren.Add(subType, GenerateChildrenImpl(subType).ToList());
        }

        public override ValueNode CreateSerializerOverride(ValueNode parent)
        {
            return new ObjectValueNode(parent, Name, this);
        }

        private IEnumerable<TypeNode> GenerateChildrenImpl(Type type)
        { 
            IEnumerable<MemberInfo> properties = type.GetProperties(MemberBindingFlags);
            IEnumerable<MemberInfo> fields = type.GetFields(MemberBindingFlags);
            IEnumerable<MemberInfo> all = properties.Union(fields);

            var children = all.Select(GenerateChild).OrderBy(child => child.Order).ToList();

            var serializableChildren = children.Where(child => child.IgnoreAttribute == null).ToList();

            if (serializableChildren.Count > 1)
            {
                var unorderedChild = serializableChildren.FirstOrDefault(child => child.Order == null);

                if (unorderedChild != null)
                    throw new InvalidOperationException(
                        string.Format(
                            "'{0}' does not have a FieldOrder attribute.  " + 
                            "All serializable fields or properties in a class with more than one member must specify a FieldOrder attribute.",
                            unorderedChild.Name));

                var orderGroups = serializableChildren.GroupBy(child => child.Order);

                if (orderGroups.Count() != serializableChildren.Count)
                    throw new InvalidOperationException("All fields must have a unique order number.");
            }

            if (type.BaseType != null)
            {
                var baseChildren = GenerateChildrenImpl(type.BaseType);
                return baseChildren.Concat(children);
            }

            return children;
        }
    }
}