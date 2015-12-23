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

        private readonly object _subTypesLock = new object();

        public List<TypeNode> Children { get; private set; }

        public ConstructorInfo Constructor { get; private set; }

        public Func<object> CompiledConstructor { get; private set; }

        public string[] ConstructorParameterNames { get; private set; }

        private IDictionary<Type, ObjectTypeNode> _subTypes;

        public ObjectTypeNode(TypeNode parent, Type type) : base(parent, type)
        {
        }

        public ObjectTypeNode(TypeNode parent, Type parentType, MemberInfo memberInfo)
            : base(parent, parentType, memberInfo)
        {
        }

        public IDictionary<Type, object> SubTypeKeys { get; private set; }

        private void Construct()
        {
            if (_subTypes != null)
                return;
       
            Children = IsIgnored ? new List<TypeNode>() : GenerateChildrenImpl(Type).ToList();

            _subTypes = new Dictionary<Type, ObjectTypeNode>();

            if (!Type.IsAbstract)
            {
                var constructors = Type.GetConstructors(ConstructorBindingFlags);

                var serializableChildren = Children.Where(child => !child.IsIgnored);

                var validConstructors =
                    constructors.Where(
                        constructor => constructor.GetParameters().Count() <= serializableChildren.Count());

                var constructorParameterMap = validConstructors.Select(constructor => new
                {
                    Constructor = constructor,
                    ParameterMap = constructor.GetParameters()
                        .Join(serializableChildren,
                            parameter => new {Name = parameter.Name.ToLower(), Type = parameter.ParameterType},
                            child => new {Name = child.Name.ToLower(), child.Type},
                            (parameter, child) => parameter.Name)
                });

                var bestConstructor =
                    constructorParameterMap.OrderByDescending(constructor => constructor.ParameterMap.Count())
                        .FirstOrDefault();

                if (bestConstructor == null)
                    return;

                Constructor = bestConstructor.Constructor;

                ConstructorParameterNames = bestConstructor.ParameterMap.ToArray();

                if (ConstructorParameterNames.Length == 0)
                    CompiledConstructor = CreateCompiledConstructor(Constructor);
            }

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
                GenerateSubtype(subType);
        }

        public override ValueNode CreateSerializerOverride(ValueNode parent)
        {
            return new ObjectValueNode(parent, Name, this);
        }

        public ObjectTypeNode GetSubType(Type type)
        {
            lock (_subTypesLock)
            {
                Construct();

                if (type == Type)
                    return this;

                /* If this is a type we've never seen before let's update our reference types. */
                GenerateSubtype(type);

                var subType = _subTypes[type];
                subType.Construct();
                return subType;
            }
        }

        private void GenerateSubtype(Type type)
        {
            lock (_subTypesLock)
            {
                if (!_subTypes.ContainsKey(type))
                {
                    var parent = (TypeNode)Parent;

                    _subTypes.Add(type,
                        typeof(IBinarySerializable).IsAssignableFrom(type)
                            ? new CustomTypeNode((TypeNode)Parent, parent.Type, MemberInfo)
                            : new ObjectTypeNode((TypeNode)Parent, parent.Type, MemberInfo));
                }
            }
        }

        private IEnumerable<TypeNode> GenerateChildrenImpl(Type parentType)
        {
            IEnumerable<MemberInfo> properties = parentType.GetProperties(MemberBindingFlags);
            IEnumerable<MemberInfo> fields = parentType.GetFields(MemberBindingFlags);
            var all = properties.Union(fields);

            var children =
                all.Select(memberInfo => GenerateChild(parentType, memberInfo)).OrderBy(child => child.Order).ToList();

            var serializableChildren = children.Where(child => !child.IsIgnored).ToList();

            if (serializableChildren.Count > 1)
            {
                var unorderedChild = serializableChildren.FirstOrDefault(child => child.Order == null);

                if (unorderedChild != null)
                    throw new InvalidOperationException(
                        $"'{unorderedChild.Name}' does not have a FieldOrder attribute.  " +
                        "All serializable fields or properties in a class with more than one member must specify a FieldOrder attribute.");

                var orderGroups = serializableChildren.GroupBy(child => child.Order);

                if (orderGroups.Count() != serializableChildren.Count)
                    throw new InvalidOperationException("All fields must have a unique order number.");
            }

            if (parentType.BaseType != null)
            {
                var baseChildren = GenerateChildrenImpl(parentType.BaseType);
                return baseChildren.Concat(children);
            }

            return children;
        }
    }
}