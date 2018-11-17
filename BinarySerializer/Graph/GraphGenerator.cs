using System;
using System.Collections.Concurrent;
using BinarySerialization.Graph.TypeGraph;

namespace BinarySerialization.Graph
{
    public class GraphGenerator
    {
        private readonly ConcurrentDictionary<Type, RootTypeNode> _graphCache =
            new ConcurrentDictionary<Type, RootTypeNode>();

        public RootTypeNode GenerateGraph(Type valueType)
        {
            return _graphCache.GetOrAdd(valueType, type => new RootTypeNode(type));
        }
    }
}
