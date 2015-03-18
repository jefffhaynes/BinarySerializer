using System.Collections.ObjectModel;
using BinarySerialization.Graph.TypeGraph;

namespace BinarySerializer.Editor.ViewModels
{
    public class ViewModel : ViewModelBase
    {
        public ViewModel()
        {
            Objects = new ObservableCollection<ObjectViewModel>();
        }

        private ObservableCollection<ObjectViewModel> _objects;

        public ObservableCollection<ObjectViewModel> Objects
        {
            get { return _objects; }
            set
            {
                if (_objects != value)
                {
                    _objects = value;
                    OnPropertyChanged();
                }
            }
        }

        private ObjectViewModel _selectedObject;

        public ObjectViewModel SelectedObject
        {
            get { return _selectedObject; }
            set
            {
                if (_selectedObject != value)
                {
                    _selectedObject = value;
                    OnPropertyChanged();
                    OnPropertyChanged("SelectedTypeNode");
                }
            }
        }

        public TypeNode SelectedTypeNode
        {
            get { return SelectedObject == null ? null : new RootTypeNode(SelectedObject.Type); }
        }
    }
}
