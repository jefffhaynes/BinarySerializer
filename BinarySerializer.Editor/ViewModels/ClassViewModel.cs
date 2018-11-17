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

        public override void Bind(IDictionary<TypeNode, FieldViewModel> map)
        {
            base.Bind(map);

            foreach (var fieldViewModel in Fields)
            {
                fieldViewModel.Bind(map);
            }
        }

        // TODO this seems weird...seems like there should be a better way
        public override ObservableCollection<BindingViewModel> Bindings =>
            new ObservableCollection<BindingViewModel>(Fields.Where(field => field.GetType() != typeof(ClassViewModel))
                .SelectMany(field => field.Bindings));
    }
}
