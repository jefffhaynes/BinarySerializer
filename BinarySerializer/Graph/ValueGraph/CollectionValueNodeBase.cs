using System.Threading;
using System.Threading.Tasks;
using BinarySerialization.Graph.TypeGraph;

namespace BinarySerialization.Graph.ValueGraph
{
    internal abstract class CollectionValueNodeBase : ValueNode
    {
        protected CollectionValueNodeBase(ValueNode parent, string name, TypeNode typeNode) : base(parent, name, typeNode)
        {
        }

        protected ValueNode CreateChildSerializer()
        {
            var typeNode = (CollectionTypeNode) TypeNode;
            return typeNode.Child.CreateSerializer(this);
        }

        protected object GetTerminationValue()
        {
            var typeNode = (CollectionTypeNode) TypeNode;
            return typeNode.TerminationValue;
        }

        protected static bool IsTerminated(BoundedStream stream, ValueNode terminationChild, object terminationValue, SerializationOptions options,
            EventShuttle eventShuttle)
        {
            if (terminationChild != null)
            {
                using (var streamResetter = new StreamResetter(stream))
                {
                    terminationChild.Deserialize(stream, options, eventShuttle);

                    if (terminationChild.Value.Equals(terminationValue))
                    {
                        streamResetter.CancelReset();
                        return true;
                    }
                }
            }
            return false;
        }

        protected void SerializeTermination(BoundedStream stream, EventShuttle eventShuttle)
        {
            var typeNode = (CollectionTypeNode) TypeNode;

            if (typeNode.TerminationChild != null)
            {
                var terminationChild = typeNode.TerminationChild.CreateSerializer(this);
                terminationChild.Value = typeNode.TerminationValue;
                terminationChild.Serialize(stream, eventShuttle);
            }
        }

        protected async Task SerializeTerminationAsync(BoundedStream stream, EventShuttle eventShuttle, CancellationToken cancellationToken)
        {
            var typeNode = (CollectionTypeNode)TypeNode;

            if (typeNode.TerminationChild != null)
            {
                var terminationChild = typeNode.TerminationChild.CreateSerializer(this);
                terminationChild.Value = typeNode.TerminationValue;
                await terminationChild.SerializeAsync(stream, eventShuttle, true, cancellationToken)
                    .ConfigureAwait(false);
            }
        }

        protected ValueNode GetTerminationChild()
        {
            var typeNode = (CollectionTypeNode) TypeNode;
            var terminationChild = typeNode.TerminationChild?.CreateSerializer(this);
            return terminationChild;
        }
    }
}