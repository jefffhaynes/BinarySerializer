using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using BinarySerialization.Graph.TypeGraph;

namespace BinarySerializer.Editor.ViewModels
{
    public class CollectionViewModel : CollectionViewModelBase
    {
        public CollectionViewModel(TypeNode typeNode, IEnumerable<ClassViewModel> subTypes) : base(typeNode)
        {
            SubTypes = new ObservableCollection<ClassViewModel>(subTypes);
        }

        public ObservableCollection<ClassViewModel> SubTypes { get; }

        public override IEnumerable<BindingViewModel> AllBindings
        {
            get
            {
                var subTypeBindings = SubTypes.SelectMany(subType => subType.AllBindings);
                return Bindings.Concat(subTypeBindings);
            }
        }

        public override void Bind(IDictionary<TypeNode, FieldViewModel> map)
        {
            base.Bind(map);

            foreach (var classViewModel in SubTypes)
            {
                classViewModel.Bind(map);
            }
        }
    }
}
