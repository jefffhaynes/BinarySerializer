using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace BinarySerializer.Editor.ViewModels
{
    public class ClassViewModel : FieldViewModel
    {
        public ClassViewModel(string type, IEnumerable<FieldViewModel> fields) : this(null, type, fields)
        {
        }

        public ClassViewModel(string name, string type, IEnumerable<FieldViewModel> fields) : this(name, type, fields, new ClassViewModel[]{})
        {
        }

        public ClassViewModel(string type, IEnumerable<FieldViewModel> fields, IEnumerable<ClassViewModel> subTypes) : this(null, type, fields, subTypes)
        {
        }

        public ClassViewModel(string name, string type, IEnumerable<FieldViewModel> fields, IEnumerable<ClassViewModel> subTypes) : base(name, type)
        {
            Fields = new ObservableCollection<FieldViewModel>(fields);
            SubTypes = new ObservableCollection<ClassViewModel>(subTypes);
        }

        public ObservableCollection<ClassViewModel> SubTypes { get; }

        public ObservableCollection<FieldViewModel> Fields { get; }
    }
}
