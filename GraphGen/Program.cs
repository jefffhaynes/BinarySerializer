using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphGen
{
    class Program
    {
        static void Main(string[] args)
        {
            var graph = new TestObject {Name = "bob", Details = new SubtypeChildClassB()};

            var node = new ObjectNode(graph.GetType());

            node.Value = graph;
        }
    }
}
