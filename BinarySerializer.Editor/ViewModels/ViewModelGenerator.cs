using System;
using System.Linq;
using BinarySerialization.Graph.TypeGraph;

namespace BinarySerializer.Editor.ViewModels
{
    public class ViewModelGenerator
    {
        public FieldViewModel Generate(TypeNode typeNode)
        {
            if (typeNode is RootTypeNode rootTypeNode)
            {
                return Generate(rootTypeNode.Child);
            }

            if (typeNode is ValueTypeNode valueTypeNode)
            {
                return new FieldViewModel(valueTypeNode.Name, valueTypeNode.Type.ToString());
            }

            if (typeNode is ObjectTypeNode objectTypeNode)
            {
                var fields = objectTypeNode.Children.Select(Generate);

                var subTypes = objectTypeNode.SubTypeKeys == null
                    ? Enumerable.Empty<ClassViewModel>()
                    : objectTypeNode.SubTypeKeys.Keys
                        .Select(key => objectTypeNode.GetSubTypeNode(key))
                        .Select(Generate).Cast<ClassViewModel>();

                return new ClassViewModel(objectTypeNode.Name, objectTypeNode.Type.ToString(), fields, subTypes);
            }

            if (typeNode is CollectionTypeNode collectionTypeNode)
            {
                var subType = Generate(collectionTypeNode.Child) as ClassViewModel;

                return new CollectionViewModel(collectionTypeNode.Name, collectionTypeNode.Type.ToString(),
                    new[] {subType});
            }

            throw new NotSupportedException();
        }
    }
}
