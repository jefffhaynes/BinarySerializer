using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace BinarySerializer.Editor.ViewModels
{
    public class CollectionViewModel : CollectionViewModelBase
    {
        public CollectionViewModel(string name, IEnumerable<ObjectViewModel> subTypes) : base(name)
        {
            SubTypes = new ObservableCollection<ObjectViewModel>(subTypes);
        }

        public ObservableCollection<ObjectViewModel> SubTypes { get; }
    }
}
