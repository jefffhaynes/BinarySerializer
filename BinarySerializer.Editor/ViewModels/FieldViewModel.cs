using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.Foundation;

namespace BinarySerializer.Editor.ViewModels
{
    public class FieldViewModel : ViewModelBase
    {
        private Point _anchorPoint;

        private string _name;
        private string _type;

        public FieldViewModel(string type)
        {
            Type = type;
        }

        public FieldViewModel(string name, string type) : this(type)
        {
            Name = name;
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

        ////public string TypeName => Type.Name;
    }
}