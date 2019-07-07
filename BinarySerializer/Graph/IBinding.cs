using System;
using BinarySerialization.Graph.ValueGraph;

namespace BinarySerialization.Graph
{
    internal interface IBinding
    {
        bool IsConst { get; }
        object ConstValue { get; }
        object GetValue(ValueNode target);
        void Bind(ValueNode target, Func<object> callback);
    }
}