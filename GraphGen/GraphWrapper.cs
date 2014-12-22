namespace BinarySerialization
{
    public class GraphWrapper<TGraph>
    {
        public GraphWrapper()
        {
        }

        public GraphWrapper(TGraph graph)
        {
            Graph = graph;
        }

        public TGraph Graph { get; set; }
    }

    public class GraphWrapper : GraphWrapper<object>
    {
    }
}
