using System;

namespace BinarySerializer.Editor.ViewModels
{
    public class FieldViewModel : ViewModelBase
    {
        public FieldViewModel(string type)
        {
            Type = type;
        }

        public FieldViewModel(string name, string type) : this(type)
        {
            Name = name;
        }

        private string _name;
        private string _type;

        public string Name
        {
            get => _name;

            set
            {
                if (value == _name) return;
                _name = value;
                OnPropertyChanged();
            }
        }

        public string Type
        {
            get => _type;
            set
            {
                if (value == _type) return;
                _type = value;
                OnPropertyChanged();
            }
        }

        ////public string TypeName => Type.Name;
    }
}
