﻿using System;
using BinarySerialization.Graph.TypeGraph;

namespace BinarySerialization.Graph.ValueGraph
{
    internal class UnknownValueNode : ValueNode
    {
        private object _cachedValue;

        public UnknownValueNode(Node parent, string name, TypeNode typeNode) : base(parent, name, typeNode)
        {
        }

        public override object Value
        {
            get { return _cachedValue; }

            set
            {
                if (value == null)
                    return;

                var valueType = value.GetType();

                if(valueType == typeof(object))
                    throw new InvalidOperationException("Unable to serialize object.");

                /* Create graph as if parent were creating it */
                var unknownTypeGraph = new RootTypeNode((TypeNode)TypeNode.Parent, valueType);
                var unknownSerializer = (ContextValueNode)unknownTypeGraph.CreateSerializer((ValueNode)Parent);
                unknownSerializer.EndiannessCallback = () => Endianness;
                unknownSerializer.EncodingCallback = () => Encoding;
                unknownSerializer.Value = value;
                Children.Add(unknownSerializer.Child);

                _cachedValue = value;
            }
        }

        //public override Node Parent
        //{
        //    get
        //    {
        //        var parent = base.Parent;
        //        return parent.Parent;
        //    }
        //}

        internal override void SerializeOverride(BoundedStream stream, EventShuttle eventShuttle)
        {
            foreach(var child in Children)
                child.Serialize(stream, eventShuttle);
        }

        internal override void DeserializeOverride(BoundedStream stream, EventShuttle eventShuttle)
        {
            throw new InvalidOperationException("Deserializing object fields not supported.");
        }
    }
}
