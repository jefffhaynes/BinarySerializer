using System;
using System.Collections.Generic;
using System.Reflection;
using BinarySerialization.Graph.ValueGraph;

namespace BinarySerialization.Graph.TypeGraph
{
    internal class ValueTypeNode : TypeNode
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

        public ValueTypeNode(TypeNode parent, Type type) : base(parent, type)
        {
            LazyWriter = new Lazy<EndianAwareBinaryWriter>(() => new EndianAwareBinaryWriter());
        }

        public ValueTypeNode(TypeNode parent, Type parentType, MemberInfo memberInfo) : base(parent, parentType, memberInfo)
        {
            LazyWriter = new Lazy<EndianAwareBinaryWriter>(() => new EndianAwareBinaryWriter());
        }

        public override ValueNode CreateSerializerOverride(ValueNode parent)
        {
            return new ValueValueNode(parent, Name, this);
        }

        public Lazy<EndianAwareBinaryWriter> LazyWriter { get; private set; }

        protected override Func<object, object> GetValueGetter(Type parentType, PropertyInfo propertyInfo)
        {
            return MagicMethods.MagicMethod(parentType, propertyInfo.GetGetMethod());
        }
    }
}