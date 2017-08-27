using System;
using System.Collections.Generic;
using System.Linq;
using BinarySerialization.Graph;
using BinarySerialization.Graph.TypeGraph;

namespace BinarySerializer.Editor.ViewModels
{
    public class ViewModelGenerator
    {
        private readonly Dictionary<TypeNode, FieldViewModel> _map = new Dictionary<TypeNode, FieldViewModel>();

        public FieldViewModel Generate(TypeNode typeNode)
        {
            _map.Clear();
            return Generate(typeNode, true);
        }

        private FieldViewModel Generate(TypeNode typeNode, bool generateBindings)
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
                        .Select(child => Generate(child, false)).Cast<ClassViewModel>();

                fieldViewModel = new ClassViewModel(objectTypeNode.Name, objectTypeNode.Type.ToString(), fields, subTypes);
            }
            else if (typeNode is CollectionTypeNode collectionTypeNode)
            {
                var subType = Generate(collectionTypeNode.Child) as ClassViewModel;

                fieldViewModel = new CollectionViewModel(collectionTypeNode.Name, collectionTypeNode.Type.ToString(),
                    new[] {subType});
            }

            if (generateBindings)
            {
                var bindingCollections = new Dictionary<BindingKind, BindingCollection>
                {
                    {BindingKind.Length, typeNode.FieldLengthBindings},
                    {BindingKind.Count, typeNode.FieldCountBindings},
                    {BindingKind.Subtype, typeNode.SubtypeBindings},
                    {BindingKind.Value, typeNode.FieldValueBindings}
                };

                foreach (var bindingCollection in bindingCollections)
                {
                    var kind = bindingCollection.Key;
                    var collection = bindingCollection.Value;

                    GenerateBindings(typeNode, collection, kind, fieldViewModel);
                }
            }

            _map.Add(typeNode, fieldViewModel);

            return fieldViewModel;
        }

        private void GenerateBindings(TypeNode typeNode, BindingCollection bindings, BindingKind kind, FieldViewModel fieldViewModel)
        {
            if (bindings == null)
            {
                return;
            }

            var bindingViewModels = bindings.Where(binding => !binding.IsConst).Select(
                binding =>
                {
                    var sourceNode = binding.GetSource(typeNode);

                    FieldViewModel sourceViewModel;
                    if (_map.TryGetValue(sourceNode, out sourceViewModel))
                    {
                        return new BindingViewModel(kind, sourceViewModel, fieldViewModel);
                    }

                    return null;
                });

            foreach (var bindingViewModel in bindingViewModels)
            {
                fieldViewModel.Bindings.Add(bindingViewModel);
            }
        }
    }
}
