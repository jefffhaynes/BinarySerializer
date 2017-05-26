using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using BinarySerialization.Graph.ValueGraph;

namespace BinarySerialization.Graph.TypeGraph
{
    internal class ObjectTypeNode : ContainerTypeNode
    {
        private const BindingFlags MemberBindingFlags =
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly;

        private readonly object _initializationLock = new object();

        private readonly Lazy<IDictionary<Type, ObjectTypeNode>> _subTypesLazy =
            new Lazy<IDictionary<Type, ObjectTypeNode>>(() => new Dictionary<Type, ObjectTypeNode>());

        private bool _isConstructed;

        public ObjectTypeNode(TypeNode parent, Type type) : base(parent, type)
        {
        }

        public ObjectTypeNode(TypeNode parent, Type parentType, MemberInfo memberInfo) :
            this(parent, parentType, memberInfo, null)
        {
        }

        public ObjectTypeNode(TypeNode parent, Type parentType, MemberInfo memberInfo, Type subType)
            : base(parent, parentType, memberInfo, subType)
        {
        }

        public List<TypeNode> Children { get; private set; }

        public ConstructorInfo Constructor { get; private set; }

        public Func<object> CompiledConstructor { get; private set; }

        public string[] ConstructorParameterNames { get; private set; }

        public IDictionary<Type, object> SubTypeKeys { get; private set; }

        public ObjectTypeNode GetSubTypeNode(Type type)
        {
            // make sure we're constructed first
            Construct();

            // trivial case, nothing to do
            if (type == Type)
            {
                return this;
            }

            // we need to check to see if we know about this type either because it was a listed subtype
            // or because we've already encountered it previously.
            lock (_initializationLock)
            {
                // In case this is a type we've never seen before let's add it.
                GenerateSubtype(type);

                // get matching subtype and make sure it's constructed
                var subType = _subTypesLazy.Value[type];
                subType.Construct();
                return subType;
            }
        }

        public override ValueNode CreateSerializerOverride(ValueNode parent)
        {
            return new ObjectValueNode(parent, Name, this);
        }

        private void Construct()
        {
            if (_isConstructed)
            {
                return;
            }

            lock (_initializationLock)
            {
                if (_isConstructed)
                {
                    return;
                }

                ConstructSubtypes();

                Children = IsIgnored ? new List<TypeNode>() : GenerateChildren(Type).ToList();

                InitializeConstructors();

                _isConstructed = true;
            }
        }

        private void GenerateSubtype(Type type)
        {
            if (_subTypesLazy.Value.ContainsKey(type))
            {
                return;
            }

            ObjectTypeNode typeNode;

            var parent = (TypeNode) Parent;
            if (MemberInfo != null)
            {
                // check for custom subtype
                typeNode = typeof(IBinarySerializable).IsAssignableFrom(type)
                    ? new CustomTypeNode(parent, parent.Type, MemberInfo, type)
                    : new ObjectTypeNode(parent, parent.Type, MemberInfo, type);
            }
            else // handle collection case
            {
                // check for custom subtype
                typeNode = typeof(IBinarySerializable).IsAssignableFrom(type)
                    ? new CustomTypeNode(parent, type)
                    : new ObjectTypeNode(parent, type);
            }

            _subTypesLazy.Value.Add(type, typeNode);
        }

        /// <summary>
        ///     Construct any specified subtypes.
        /// </summary>
        private void ConstructSubtypes()
        {
            var parent = (TypeNode) Parent;

            if (SubtypeAttributes != null && SubtypeAttributes.Count > 0)
            {
                ConstructSubtypes(SubtypeBinding, SubtypeAttributes);
            }
            else if (parent.ItemSubtypeAttributes != null && parent.ItemSubtypeAttributes.Count > 0)
            {
                ConstructSubtypes(parent.ItemSubtypeBinding, parent.ItemSubtypeAttributes);
            }
        }

        private void ConstructSubtypes(Binding binding, ReadOnlyCollection<SubtypeBaseAttribute> attributes)
        {
            // Get subtype keys 
            if (binding.BindingMode != BindingMode.OneWay)
            {
                SubTypeKeys = attributes.ToDictionary(attribute => attribute.Subtype,
                    attribute => attribute.Value);
            }

            // Generate subtype children 
            var subTypes = attributes.Where(attribute => attribute.BindingMode != BindingMode.OneWay)
                .Select(attribute => attribute.Subtype);

            foreach (var subType in subTypes)
            {
                GenerateSubtype(subType);
            }
        }

        /// <summary>
        ///     Initialize any constructors for this type, including parameterized constructors that could be used during
        ///     deserialization.
        /// </summary>
        private void InitializeConstructors()
        {
            // if abstract we will never be constructed, nothing to do
            if (Type.GetTypeInfo().IsAbstract)
            {
                return;
            }

            // we're going to start finding valid constructors.  start by getting all constructors.
            var constructors = Type.GetConstructors(ConstructorBindingFlags);

            var serializableChildren = Children.Where(child => !child.IsIgnored);

            // don't include constructors that we'll never be able to use because they require more parameters
            // than for which we have field definitions.
            var validConstructors =
                constructors.Where(
                    constructor => constructor.GetParameters().Length <= serializableChildren.Count());

            // build a map of all constructors, filling in nulls for parameters without
            // corresponding fields, matched on name and type
            var constructorParameterMap = validConstructors.ToDictionary(constructor => constructor,
                constructor =>
                    constructor.GetParameters()
                        .GroupJoin(serializableChildren,
                            parameter => new {Name = parameter.Name.ToLower(), Type = parameter.ParameterType},
                            child => new {Name = child.Name.ToLower(), child.Type},
                            (parameter, children) => new {parameter, children})
                        .SelectMany(result => result.children.DefaultIfEmpty())
            );

            // eliminate any constructors that aren't complete in terms of required parameters
            var completeConstructors =
                constructorParameterMap.Where(constructorPair => constructorPair.Value.All(child => child != null))
                    .ToList();

            // see if there are any constructors left that can be used at all
            if (!completeConstructors.Any())
            {
                return;
            }

            // choose best match in terms of greatest number of valid parameters
            var bestConstructor =
                completeConstructors.OrderByDescending(constructorPair => constructorPair.Value.Count())
                    .First();

            // get best constructor
            Constructor = bestConstructor.Key;

            // get corresponding parameter names
            ConstructorParameterNames = bestConstructor.Value.Select(child => child.Name).ToArray();

            // if this is a parameterless constructor, go ahead and compile it.  
            // we can't compile constructors with parameters since we don't have them yet.
            if (ConstructorParameterNames.Length == 0)
            {
                CompiledConstructor = CreateCompiledConstructor(Constructor);
            }
        }

        private IEnumerable<TypeNode> GenerateChildren(Type parentType)
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
                {
                    throw new InvalidOperationException(
                        $"'{unorderedChild.Name}' does not have a FieldOrder attribute.  " +
                        "All serializable fields or properties in a class with more than one member must specify a FieldOrder attribute.");
                }

                var orderGroups = serializableChildren.GroupBy(child => child.Order);

                if (orderGroups.Count() != serializableChildren.Count)
                {
                    throw new InvalidOperationException("All fields must have a unique order number.");
                }
            }

            if (parentType.GetTypeInfo().BaseType != null)
            {
                var baseChildren = GenerateChildren(parentType.GetTypeInfo().BaseType);
                return baseChildren.Concat(children);
            }

            return children;
        }
    }
}