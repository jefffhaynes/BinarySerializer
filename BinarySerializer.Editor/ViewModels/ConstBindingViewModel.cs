namespace BinarySerializer.Editor.ViewModels
{
    public class ConstBindingViewModel : ViewModelBase
    {
        public ConstBindingViewModel(BindingKind kind, int value)
        {
            Kind = kind;
            Value = value;
        }

        public BindingKind Kind { get; }
        public int Value { get; }
    }
}
