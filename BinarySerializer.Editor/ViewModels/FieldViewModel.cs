namespace BinarySerializer.Editor.ViewModels
{
    public class FieldViewModel : ViewModelBase
    {
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
