namespace BinarySerializer.Editor.ViewModels
{
    public class BindingViewModel : ViewModelBase
    {
        public BindingViewModel(BindingKind kind, FieldViewModel source, FieldViewModel target)
        {
            Kind = kind;
            Source = source;
            Target = target;
        }

        public BindingKind Kind { get; }
        public FieldViewModel Source { get; }
        public FieldViewModel Target { get; }
    }
}