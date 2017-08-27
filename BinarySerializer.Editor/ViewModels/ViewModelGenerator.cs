using System;
using System.Collections.Generic;
using System.Linq;
using BinarySerialization.Graph.TypeGraph;

namespace BinarySerializer.Editor.ViewModels
{
    public class ViewModelGenerator
    {
        private Dictionary<TypeNode, FieldViewModel> _map = new Dictionary<TypeNode, FieldViewModel>();

        public FieldViewModel Generate(TypeNode typeNode)
        {
            FieldViewModel fieldViewModel = null;

            if (typeNode is RootTypeNode rootTypeNode)
            {
                fieldViewModel = Generate(rootTypeNode.Child);
            }
            else if (typeNode is ValueTypeNode valueTypeNode)
            {
                fieldViewModel = new FieldViewModel(valueTypeNode.Name, valueTypeNode.Type.ToString());
            }
            else if (typeNode is ObjectTypeNode objectTypeNode)
            {
                var fields = objectTypeNode.Children.Select(Generate);

                var subTypes = objectTypeNode.SubTypeKeys == null
                    ? Enumerable.Empty<ClassViewModel>()
                    : objectTypeNode.SubTypeKeys.Keys
                        .Select(key => objectTypeNode.GetSubTypeNode(key))
                        .Select(Generate).Cast<ClassViewModel>();

                fieldViewModel = new ClassViewModel(objectTypeNode.Name, objectTypeNode.Type.ToString(), fields, subTypes);
            }
            else if (typeNode is CollectionTypeNode collectionTypeNode)
            {
                var subType = Generate(collectionTypeNode.Child) as ClassViewModel;

                fieldViewModel = new CollectionViewModel(collectionTypeNode.Name, collectionTypeNode.Type.ToString(),
                    new[] {subType});
            }

            if (typeNode.FieldLengthBindings != null)
            {
                var bindingViewModels = typeNode.FieldLengthBindings.Where(binding => !binding.IsConst).Select(binding =>
                {
                    var sourceNode = binding.GetSource(typeNode);

                    FieldViewModel sourceViewModel;
                    if (_map.TryGetValue(sourceNode, out sourceViewModel))
                    {
                        return new BindingViewModel(BindingKind.Length, sourceViewModel, fieldViewModel);
                    }

                    throw new InvalidOperationException();
                });

                foreach (var bindingViewModel in bindingViewModels)
                {
                    fieldViewModel.Bindings.Add(bindingViewModel);
                }
            }

            _map.Add(typeNode, fieldViewModel);

            return fieldViewModel;
        }
    }
}
