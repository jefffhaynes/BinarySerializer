namespace BinarySerialization.Graph
{
    internal abstract class Node
    {
        protected Node(Node parent)
        {
            Parent = parent;
        }

        public Node Parent { get; }

        public string Name { get; set; }

        public override string ToString()
        {
            if (Name != null)
            {
                return Name;
            }

            return base.ToString();
        }
    }
}