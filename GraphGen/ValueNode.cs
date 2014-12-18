using System.IO;
using System.Reflection;

namespace GraphGen
{
    public class ValueNode : Node
    {
        public ValueNode(Node parent, MemberInfo memberInfo) : base(parent, memberInfo)
        {
        }

        public override void Serialize(Stream stream)
        {
            throw new System.NotImplementedException();
        }

        public override void Deserialize(Stream stream)
        {
            throw new System.NotImplementedException();
        }
    }
}
