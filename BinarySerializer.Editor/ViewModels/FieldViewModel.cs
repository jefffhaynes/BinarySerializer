using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.Foundation;
using BinarySerialization;
using BinarySerialization.Graph;
using BinarySerialization.Graph.TypeGraph;

namespace BinarySerializer.Editor.ViewModels
{
    public class FieldViewModel : ViewModelBase
    {
        private Point _anchorPoint;

        private string _name;
        private string _type;

        protected TypeNode TypeNode;

        public FieldViewModel(TypeNode typeNode)
        {
            Name = typeNode.Name;
            Type = typeNode.Type.Name;
            TypeNode = typeNode;
            SerializedType = typeNode.GetSerializedType();
        }

        public string Name
        {
            get => _name;

            set
            {
                if (value == _name)
                {
                    return;
                }
                _name = value;
                OnPropertyChanged();
            }
        }

        public string Type
        {
            get => _type;
            set
            {
                if (value == _type)
                {
                    return;
                }
                _type = value;
                OnPropertyChanged();
            }
        }
        
        public SerializedType SerializedType { get; set; }

        public ObservableCollection<BindingViewModel> Bindings { get; } = new ObservableCollection<BindingViewModel>();

        public ObservableCollection<ConstBindingViewModel> ConstBindings { get; } = new ObservableCollection<ConstBindingViewModel>();

        public virtual IEnumerable<BindingViewModel> AllBindings => Bindings;

        public Point AnchorPoint
        {
            get => _anchorPoint;
            set
            {
                if (value.Equals(_anchorPoint))
                {
                    return;
                }
                _anchorPoint = value;
                OnPropertyChanged();
            }
        }

        public virtual void Bind(IDictionary<TypeNode, FieldViewModel> map)
        {
            var bindingCollections = new Dictionary<BindingKind, BindingCollection>
            {
                {BindingKind.Length, TypeNode.FieldLengthBindings},
                {BindingKind.Count, TypeNode.FieldCountBindings},
                {BindingKind.LeftAlignment, TypeNode.LeftFieldAlignmentBindings },
                {BindingKind.RightAlignment, TypeNode.RightFieldAlignmentBindings },
                {BindingKind.Scale, TypeNode.FieldScaleBindings },
                {BindingKind.Endianness, TypeNode.FieldEndiannessBindings },
                {BindingKind.Encoding, TypeNode.FieldEncodingBindings },
                {BindingKind.Value, TypeNode.FieldValueBindings},
                {BindingKind.Offset, TypeNode.FieldOffsetBindings },
                {BindingKind.Subtype, TypeNode.SubtypeBindings},
                {BindingKind.ItemLength, TypeNode.ItemLengthBindings },
                {BindingKind.ItemSubtype, TypeNode.ItemSubtypeBindings }
            };

            foreach (var bindingCollection in bindingCollections)
            {
                var kind = bindingCollection.Key;
                var collection = bindingCollection.Value;

                GenerateBindings(TypeNode, collection, kind, map);
            }
        }

        private void GenerateBindings(TypeNode typeNode, BindingCollection bindings, BindingKind kind, IDictionary<TypeNode, FieldViewModel> map)
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
                    if (map.TryGetValue(sourceNode, out sourceViewModel))
                    {
                        return new BindingViewModel(kind, sourceViewModel, this);
                    }

                    return null;
                });

            foreach (var bindingViewModel in bindingViewModels.Where(binding => binding != null))
            {
                bindingViewModel.Source.Bindings.Add(bindingViewModel);
            }

            var constBindings = bindings.Where(binding => binding.IsConst)
                .Select(binding => new ConstBindingViewModel(kind, Convert.ToInt32(binding.ConstValue)));

            foreach (var constBindingViewModel in constBindings)
            {
                ConstBindings.Add(constBindingViewModel);
            }
        }
    }
}