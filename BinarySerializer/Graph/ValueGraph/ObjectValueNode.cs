using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BinarySerialization.Graph.TypeGraph;

namespace BinarySerialization.Graph.ValueGraph
{
    internal class ObjectValueNode : ValueNode
    {
        public ObjectValueNode(Node parent, string name, TypeNode typeNode)
            : base(parent, name, typeNode)
        {
        }

        private Type _valueType;

        public override object Value
        {
            get
            {
                if (_valueType == null)
                    return null;

                var value = Activator.CreateInstance(_valueType);

                var serializableChildren = GetSerializableChildren();

                foreach (var child in serializableChildren)
                    child.TypeNode.ValueSetter(value, child.Value);

                return value;
            }

            set
            {
                if (Children.Any())
                    throw new InvalidOperationException("Value already set.");

                if (value == null)
                    return;

                var typeNode = (ObjectTypeNode) TypeNode;

                var typeChildren = typeNode.TypeChildren[value.GetType()];

                Children = new List<Node>(typeChildren.Select(child => child.CreateSerializer(this)));

                foreach (var child in Children.Cast<ValueNode>())
                    child.Value = child.TypeNode.ValueGetter(value);

                _valueType = value.GetType();
            }
        }

        private IEnumerable<ValueNode> GetSerializableChildren()
        {
            return Children.Cast<ValueNode>().Where(child => child.TypeNode.IgnoreAttribute == null);
        }


        protected override void SerializeOverride(Stream stream)
        {
            var serializableChildren = GetSerializableChildren();

            foreach (var child in serializableChildren)
            {
                child.Serialize(stream);
            }
        }

        public override void DeserializeOverride(StreamLimiter stream)
        {
            if (TypeNode.SubtypeBinding == null)
            {
                _valueType = TypeNode.Type;
            }
            else
            {
                var subTypeValue = TypeNode.SubtypeBinding.GetValue(this);

                var matchingAttribute =
                    TypeNode.SubtypeAttributes.SingleOrDefault(
                        attribute =>
                            subTypeValue.Equals(Convert.ChangeType(attribute.Value, subTypeValue.GetType(), null)));

                _valueType = matchingAttribute == null ? null : matchingAttribute.Subtype;
            }

            if (_valueType == null)
                return;

            var typeNode = (ObjectTypeNode)TypeNode;

            Children = new List<Node>(typeNode.TypeChildren[_valueType].Select(child => child.CreateSerializer(this)));

            foreach (var child in Children.Cast<ValueNode>())
            {
                if (ShouldTerminate(stream))
                    break;

                child.Deserialize(stream);
            }
        }

        protected override Type GetValueTypeOverride()
        {
            return _valueType;
        }
    }
}
