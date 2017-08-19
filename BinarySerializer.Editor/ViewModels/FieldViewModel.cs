namespace BinarySerializer.Editor.ViewModels
{
    public class FieldViewModel : ViewModelBase
    {
        public FieldViewModel(string name)
        {
            Name = name;
        }

        private string _name;

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
    }
}
