using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.Foundation;
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

        public ObservableCollection<BindingViewModel> Bindings { get; } = new ObservableCollection<BindingViewModel>();

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
                {BindingKind.Subtype, TypeNode.SubtypeBindings},
                {BindingKind.Value, TypeNode.FieldValueBindings}
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

            foreach (var bindingViewModel in bindingViewModels)
            {
                Bindings.Add(bindingViewModel);
            }
        }

        ////public string TypeName => Type.Name;
    }
}