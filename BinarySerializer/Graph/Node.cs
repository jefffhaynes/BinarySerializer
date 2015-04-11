using System;
using System.Collections.Generic;
using System.Linq;

namespace BinarySerialization.Graph
{
    public abstract class Node
    {
        private const char PathSeparator = '.';

        protected Node(Node parent)
        {
            Parent = parent;
            TargetBindings = new List<Func<object>>();
        }

        public Node Parent { get; private set; }

        public List<Node> Children { get; set; }

        public string Name { get; set; }

        public List<Func<object>> TargetBindings { get; set; }

        public Node GetChild(string path)
        {
            string[] memberNames = path.Split(PathSeparator);

            if (!memberNames.Any())
                throw new BindingException("Path cannot be empty.");

            Node child = this;
            foreach (string name in memberNames)
            {
                child = child.Children.SingleOrDefault(c => c.Name == name);

                if (child == null)
                    throw new BindingException(string.Format("No field found at '{0}'.", path));
            }

            return child;
        }


        public override string ToString()
        {
            if (Name != null)
                return Name;

            return base.ToString();
        }
    }
}
