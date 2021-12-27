﻿namespace BinarySerialization.Graph;

internal class GraphGenerator
{
    private readonly ConcurrentDictionary<Type, RootTypeNode> _graphCache =
        new ConcurrentDictionary<Type, RootTypeNode>();

    public RootTypeNode GenerateGraph(Type valueType)
    {
        return _graphCache.GetOrAdd(valueType, type => new RootTypeNode(type));
    }
}
