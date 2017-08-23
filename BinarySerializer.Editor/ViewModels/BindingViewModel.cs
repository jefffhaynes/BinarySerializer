namespace BinarySerializer.Editor.ViewModels
{
    public class BindingViewModel : ViewModelBase
    {
        public BindingViewModel(FieldViewModel source, FieldViewModel target)
        {
            Source = source;
            Target = target;
        }

        public FieldViewModel Source { get; }
        public FieldViewModel Target { get; }
    }
}
