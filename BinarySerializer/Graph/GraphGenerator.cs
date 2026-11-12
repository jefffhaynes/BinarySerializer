using System;
using System.Diagnostics.CodeAnalysis;
using System.Collections.Concurrent;
using BinarySerialization.Graph.TypeGraph;

namespace BinarySerialization.Graph
{
    internal class GraphGenerator
    {
        private readonly ConcurrentDictionary<Type, RootTypeNode> _graphCache =
            new ConcurrentDictionary<Type, RootTypeNode>();

        public RootTypeNode GenerateGraph([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type valueType)
        {
            if (_graphCache.TryGetValue(valueType, out var cached))
            {
                return cached;
            }

            var graph = new RootTypeNode(valueType);
            return _graphCache.GetOrAdd(valueType, graph);
        }
    }
}
