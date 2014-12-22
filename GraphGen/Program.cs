using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphGen
{
    class Program
    {
        static void Main(string[] args)
        {
            var graph = new TestObject {Name = "bob"};//, Details = new SubtypeChildClassB()};

            var node = new ObjectNode(graph.GetType());

            node.Value = graph;

            var stream = new MemoryStream();
            node.Serialize(stream);
            stream.Position = 0;

            var node2 = new ObjectNode(graph.GetType());
            node2.Deserialize(stream);

            var value = node2.Value;
        }
    }
}
