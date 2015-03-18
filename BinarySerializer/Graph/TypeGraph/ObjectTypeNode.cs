using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BinarySerialization.Graph.ValueGraph;

namespace BinarySerialization.Graph.TypeGraph
{
    public class ObjectTypeNode : ContainerTypeNode
    {
        private const BindingFlags MemberBindingFlags =
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly;

        private readonly object _typeChildrenLock = new object();

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

        public IDictionary<object, List<TypeNode>> KeysAndChildren
        {
            get
            {
                var keysAndChildren = SubtypeAttributes.Join(TypeChildren, kvp1 => kvp1.Subtype, kvp2 => kvp2.Key,
                    (kvp1, kvp2) => new {key = kvp1.Value, value = kvp2.Value});

                return keysAndChildren.ToDictionary(kvp => kvp.key, kvp => kvp.value);
            }
        }

        private void Construct()
        {
            if (IgnoreAttribute != null)
                return;

            var baseChildren = GenerateChildrenImpl(Type).ToList();

            TypeChildren = new Dictionary<Type, List<TypeNode>> {{Type, baseChildren}};

            /* Add subtypes, if any */
            if (SubtypeAttributes == null || SubtypeAttributes.Count <= 0)
                return;

            /* Get subtype keys */
            if (SubtypeBinding.BindingMode == BindingMode.TwoWay)
                SubTypeKeys = SubtypeAttributes.ToDictionary(attribute => attribute.Subtype,
                    attribute => attribute.Value);

            /* Generate subtype children */
            var subTypes = SubtypeAttributes.Select(attribute => attribute.Subtype);

            foreach (var subType in subTypes)
                GenerateChildren(subType);
        }

        internal override ValueNode CreateSerializerOverride(ValueNode parent)
        {
            return new ObjectValueNode(parent, Name, this);
        }

        private void GenerateChildren(Type type)
        {
            lock (_typeChildrenLock)
            {
                if (!TypeChildren.ContainsKey(type))
                    TypeChildren.Add(type, GenerateChildrenImpl(type).ToList());
            }
        }

        public IEnumerable<TypeNode> GetTypeChildren(Type type)
        {
            lock (_typeChildrenLock)
            {
                /* If this is a type we've never seen before let's update our reference type. */
                if (!TypeChildren.ContainsKey(type))
                    TypeChildren.Add(type, GenerateChildrenImpl(type).ToList());

                return TypeChildren[type];
            }
        }

        private IEnumerable<TypeNode> GenerateChildrenImpl(Type type)
        {
            IEnumerable<MemberInfo> properties = type.GetProperties(MemberBindingFlags);
            IEnumerable<MemberInfo> fields = type.GetFields(MemberBindingFlags);
            var all = properties.Union(fields);

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