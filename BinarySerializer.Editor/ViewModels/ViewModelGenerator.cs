using System.Collections.Generic;
using System.Linq;
using BinarySerialization.Graph.TypeGraph;

namespace BinarySerializer.Editor.ViewModels
{
    public class ViewModelGenerator
    {
        public FieldViewModel Generate(TypeNode typeNode)
        {
            var map = new Dictionary<TypeNode, FieldViewModel>();
            var fieldViewModel = Generate(typeNode, map);
            fieldViewModel.Bind(map);
            return fieldViewModel;
        }

        private FieldViewModel Generate(TypeNode typeNode, IDictionary<TypeNode, FieldViewModel> map)
        {
            FieldViewModel fieldViewModel = null;

            if (typeNode is RootTypeNode rootTypeNode)
            {
                fieldViewModel = Generate(rootTypeNode.Child, map);
            }
            else if (typeNode is ValueTypeNode valueTypeNode)
            {
                fieldViewModel = new FieldViewModel(valueTypeNode);
            }
            else if (typeNode is ObjectTypeNode objectTypeNode)
            {
                var fields = objectTypeNode.Children.Select(node => Generate(node, map));

                var subTypes = objectTypeNode.SubTypeKeys == null
                    ? Enumerable.Empty<ClassViewModel>()
                    : objectTypeNode.SubTypeKeys.Keys
                        .Select(key => objectTypeNode.GetSubTypeNode(key))
                        .Select(node => Generate(node, map)).Cast<ClassViewModel>();

                fieldViewModel = new ClassViewModel(objectTypeNode, fields, subTypes);
            }
            else if (typeNode is CollectionTypeNode collectionTypeNode)
            {
                var subType = Generate(collectionTypeNode.Child, map) as ClassViewModel;

                fieldViewModel = new CollectionViewModel(collectionTypeNode, new[] {subType});
            }
            
            map.Add(typeNode, fieldViewModel);

            return fieldViewModel;
        }
    }
}
