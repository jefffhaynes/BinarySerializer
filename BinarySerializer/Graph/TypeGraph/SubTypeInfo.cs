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

            var serializableChildren = Children.Where(child => child.IgnoreAttribute == null);

            var validConstructors =
                constructors.Where(constructor => constructor.GetParameters().Count() <= serializableChildren.Count());

            var constructorParameterMap = validConstructors.Select(constructor => new
            {
                Constructor = constructor,
                ParameterMap = constructor.GetParameters()
                    .Join(serializableChildren, parameter => new { Name = parameter.Name.ToLower(), Type = parameter.ParameterType },
                        child => new {Name = child.Name.ToLower(), child.Type},
                        (parameter, child) => parameter.Name)
            });

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
