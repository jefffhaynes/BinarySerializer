using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using BinarySerialization.Graph.TypeGraph;

namespace BinarySerializer.Editor.ViewModels
{
    public class ClassViewModel : FieldViewModel
    {
        public ClassViewModel(TypeNode typeNode, IEnumerable<FieldViewModel> fields, IEnumerable<ClassViewModel> subTypes) : base(typeNode)
        {
            Fields = new ObservableCollection<FieldViewModel>(fields);
            SubTypes = new ObservableCollection<ClassViewModel>(subTypes);
        }

        public ObservableCollection<ClassViewModel> SubTypes { get; }

        public ObservableCollection<FieldViewModel> Fields { get; }


        public override IEnumerable<BindingViewModel> AllBindings
        {
            get
            {
                var subTypeBindings = SubTypes.SelectMany(subType => subType.AllBindings);
                var fieldBindings = Fields.SelectMany(c => c.AllBindings);
                return Bindings.Concat(subTypeBindings.Concat(fieldBindings));
            }
        }

        public override void Bind(IDictionary<TypeNode, FieldViewModel> map)
        {
            base.Bind(map);

            foreach (var fieldViewModel in Fields)
            {
                fieldViewModel.Bind(map);
            }
        }
    }
}
