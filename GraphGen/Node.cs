using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BinarySerialization;

namespace GraphGen
{
    public abstract class Node
    {
        protected Node(Node parent)
        {
            Parent = parent;
        }

        protected Node(Node parent, MemberInfo memberInfo) : this(parent)
        {
            var propertyInfo = memberInfo as PropertyInfo;
            var fieldInfo = memberInfo as FieldInfo;

            if (propertyInfo != null)
            {
                Type = propertyInfo.PropertyType;
                ValueGetter = declaringValue => propertyInfo.GetValue(declaringValue, null);
                ValueSetter = propertyInfo.SetValue;
            }
            else if (fieldInfo != null)
            {
                Type = fieldInfo.FieldType;
                ValueGetter = fieldInfo.GetValue;
                ValueSetter = fieldInfo.SetValue;
            }
            else throw new NotSupportedException(string.Format("{0} not supported", memberInfo.GetType().Name));
        }

        public Node Parent { get; set; }

        public Type Type { get; protected set; }

        public Action<object, object> ValueSetter { get; private set; }

        public Func<object, object> ValueGetter { get; private set; } 

        public virtual object Value { get; set; }

        public SerializeAsAttribute SerializeAsAttribute { get; set; }
        public IgnoreAttribute IgnoreAttribute { get; set; }
        public FieldOffsetAttribute FieldOffsetAttribute { get; set; }
        public FieldLengthAttribute FieldLengthAttribute { get; set; }
        public FieldCountAttribute FieldCountAttribute { get; set; }
        public SerializeWhenAttribute[] SerializeWhenAttributes { get; set; }
        public SerializeUntilAttribute SerializeUntilAttribute { get; set; }
        public ItemLengthAttribute ItemLengthAttribute { get; set; }
        public ItemSerializeUntilAttribute ItemSerializeUntilAttribute { get; set; }
        public SubtypeAttribute[] SubtypeAttributes { get; set; }

        public abstract void Serialize(Stream stream);

        public abstract void Deserialize(Stream stream);

    }
}
