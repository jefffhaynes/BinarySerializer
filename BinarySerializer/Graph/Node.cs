using System.Collections.Generic;
using System.Linq;

namespace BinarySerialization.Graph
{
    public abstract class Node<TNode> where TNode : Node<TNode>
    {
        private const char PathSeparator = '.';

        protected Node(TNode parent)
        {
            Parent = parent;
        }

        public TNode Parent { get; }

        public string Name { get; set; }
        
        public List<TNode> Children { get; set; }

        public TNode GetChild(string path)
        {
            var memberNames = path.Split(PathSeparator);

            if (memberNames.Length == 0)
            {
                throw new BindingException("Path cannot be empty.");
            }

            var child = (TNode)this;
            foreach (var name in memberNames)
            {
                child = child.Children.SingleOrDefault(c => c.Name == name);

                if (child == null)
                {
                    throw new BindingException($"No field found at '{path}'.");
                }
            }

            return child;
        }

        public override string ToString()
        {
            return Name ?? base.ToString();
        }
    }
}