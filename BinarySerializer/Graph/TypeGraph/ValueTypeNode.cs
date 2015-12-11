﻿using System;
using System.Reflection;
using BinarySerialization.Graph.ValueGraph;

namespace BinarySerialization.Graph.TypeGraph
{
    internal class ValueTypeNode : TypeNode
    {
        public ValueTypeNode(TypeNode parent, Type type) : base(parent, type)
        {
        }

        public ValueTypeNode(TypeNode parent, Type parentType, MemberInfo memberInfo) : base(parent, parentType, memberInfo)
        {
        }

        public override ValueNode CreateSerializerOverride(ValueNode parent)
        {
            return new ValueValueNode(parent, Name, this);
        }
    }
}