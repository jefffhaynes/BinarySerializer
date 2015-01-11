using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BinarySerialization.Graph.TypeGraph;

namespace BinarySerialization.Graph.ValueGraph
{
    internal class ValueValueNode : ValueNode
    {
        protected static readonly Dictionary<Type, Func<object, object>> TypeConverters =
    new Dictionary<Type, Func<object, object>>
            {
                {typeof (char), o => Convert.ToChar(o)},
                {typeof (byte), o => Convert.ToByte(o)},
                {typeof (sbyte), o => Convert.ToSByte(o)},
                {typeof (bool), o => Convert.ToBoolean(o)},
                {typeof (Int16), o => Convert.ToInt16(o)},
                {typeof (Int32), o => Convert.ToInt32(o)},
                {typeof (Int64), o => Convert.ToInt64(o)},
                {typeof (UInt16), o => Convert.ToUInt16(o)},
                {typeof (UInt32), o => Convert.ToUInt32(o)},
                {typeof (UInt64), o => Convert.ToUInt64(o)},
                {typeof (Single), o => Convert.ToSingle(o)},
                {typeof (Double), o => Convert.ToDouble(o)},
                {typeof (string), Convert.ToString}
            };

        public ValueValueNode(Node parent, TypeNode typeNode)
            : base(parent, typeNode)
        {
        }

        public ValueValueNode(Node parent, TypeNode typeNode, object value)
            : base(parent, typeNode)
        {
            Value = value;
        }

        public object Value { get; set; }

        protected override void SerializeOverride(Stream stream)
        {
            throw new NotImplementedException();
        }

        public object BoundValue
        {
            get
            {
                object value;

                if (TargetBindings.Any())
                {
                    value = TargetBindings[0]();

                    if (TargetBindings.Count != 1)
                    {
                        var targetValues = TargetBindings.Select(binding => binding()).ToArray();

                        if (targetValues.Any(v => !value.Equals(v)))
                            throw new BindingException(
                                "Multiple bindings to a single source must have equivalent target values.");
                    }
                }
                else value = Value;

                return ConvertToFieldType(value);
            }
        }

        public object ConvertToFieldType(object value)
        {
            if (value == null)
                return null;

            var valueType = value.GetType();
            var nodeType = TypeNode.Type;

            if (valueType == nodeType)
                return value;

            /* Special handling for strings */
            if (valueType == typeof(string) && nodeType.IsPrimitive)
            {
                if (string.IsNullOrWhiteSpace(value.ToString()))
                    value = 0;
            }

            Func<object, object> converter;
            if (TypeConverters.TryGetValue(nodeType, out converter))
                return converter(value);

            if (nodeType.IsEnum && value.GetType().IsPrimitive)
                return Enum.ToObject(nodeType, value);

            return value;
        }
    }
}
