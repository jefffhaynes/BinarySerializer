using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BinarySerialization.Graph.TypeGraph
{
    internal class SubTypeInfo
    {
        public SubTypeInfo(ConstructorInfo parameterlessConstructor, 
            IEnumerable<ConstructorInfo> constructors,
            IEnumerable<TypeNode> children)
        {
            ParameterlessConstructor = parameterlessConstructor;
            Constructors = constructors.ToArray();
            Children = children.ToArray();
        }

        public ConstructorInfo ParameterlessConstructor { get; private set; }

        public ConstructorInfo[] Constructors { get; private set; }

        public TypeNode[] Children { get; private set; }
    }
}
