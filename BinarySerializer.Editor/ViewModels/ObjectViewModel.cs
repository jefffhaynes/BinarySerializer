using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace BinarySerializer.Editor.ViewModels
{
    public class ObjectViewModel : FieldViewModel
    {
        public ObjectViewModel(string name, IEnumerable<FieldViewModel> fields) : base(name)
        {
            Fields = new ObservableCollection<FieldViewModel>(fields);
        }

        public ObservableCollection<FieldViewModel> Fields { get; }
    }
}
