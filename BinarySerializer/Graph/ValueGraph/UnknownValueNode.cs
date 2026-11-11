using System;
using System.Diagnostics.CodeAnalysis;
using BinarySerialization.Graph.TypeGraph;

namespace BinarySerialization.Graph.ValueGraph
{
    internal class UnknownValueNode : ObjectValueNode
    {
        private object _cachedValue;

        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
        private Type _valueType;

        public UnknownValueNode(ValueNode parent, string name, TypeNode typeNode) : base(parent, name, typeNode)
        {
        }

        public override object Value
        {
            get
            {
                /* For creating serialization contexts quickly */
                if (_cachedValue != null)
                {
                    return _cachedValue;
                }

                return GetValue(child => child.Value);
            }

            [UnconditionalSuppressMessage("Trimming", "IL2067",
                Justification =
                    "Runtime type discovery over the user object graph; users of trimmed or AOT-compiled applications must root their serialized types (see README, AOT support).")]
            [UnconditionalSuppressMessage("Trimming", "IL2072",
                Justification =
                    "Runtime type discovery over the user object graph; users of trimmed or AOT-compiled applications must root their serialized types (see README, AOT support).")]
            [UnconditionalSuppressMessage("Trimming", "IL2074",
                Justification =
                    "Runtime type discovery over the user object graph; users of trimmed or AOT-compiled applications must root their serialized types (see README, AOT support).")]
            set
            {
                if (value == null)
                {
                    return;
                }

                _valueType = value.GetType();

                if (_valueType == typeof(object))
                {
                    throw new InvalidOperationException("Unable to serialize object.");
                }

                /* Create graph as if parent were creating it */
                var unknownTypeGraph = new RootTypeNode(TypeNode.Parent, _valueType);
                var unknownSerializer = (RootValueNode) unknownTypeGraph.CreateSerializer(Parent);
                unknownSerializer.EndiannessCallback = GetFieldEndianness;
                unknownSerializer.EncodingCallback = GetFieldEncoding;
                unknownSerializer.Value = value;
                Children.Add(unknownSerializer.Child);

                _cachedValue = value;
            }
        }

        protected override Type GetValueTypeOverride()
        {
            return _valueType;
        }
    }
}