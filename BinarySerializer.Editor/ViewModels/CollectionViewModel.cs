using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace BinarySerializer.Editor.ViewModels
{
    public class CollectionViewModel : CollectionViewModelBase
    {
        public CollectionViewModel(string type, IEnumerable<ClassViewModel> subTypes) : this(null, type, subTypes)
        {
        }

        public CollectionViewModel(string name, string type, IEnumerable<ClassViewModel> subTypes) : base(name, type)
        {
            SubTypes = new ObservableCollection<ClassViewModel>(subTypes);
        }

        public ObservableCollection<ClassViewModel> SubTypes { get; }
    }
}
