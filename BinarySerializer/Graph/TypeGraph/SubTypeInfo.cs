using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BinarySerialization.Graph.TypeGraph
{
    internal class SubTypeInfo
    {
        public SubTypeInfo(IEnumerable<ConstructorInfo> constructors,
            IEnumerable<TypeNode> children)
        {
            Children = children.ToArray();

            var validConstructors =
                constructors.Where(constructor => constructor.GetParameters().Count() <= Children.Count());

            // TODO CHECK TYPES
            var constructorParameterMap = validConstructors.Select(constructor => new { Constructor = constructor, ParameterMap = constructor.GetParameters()
                .Join(Children, parameter => parameter.Name.ToLower(), child => child.Name.ToLower(),
                    (parameter, child) => parameter.Name)});

            var bestConstructor = constructorParameterMap.OrderByDescending(constructor => constructor.ParameterMap.Count()).FirstOrDefault();

            if (bestConstructor == null)
                return;

            Constructor = bestConstructor.Constructor;

            ConstructorParameterNames = bestConstructor.ParameterMap.ToArray();
        }

        public ConstructorInfo Constructor { get; private set; }

        public string[] ConstructorParameterNames { get; private set; }

        public TypeNode[] Children { get; private set; }
    }
}
