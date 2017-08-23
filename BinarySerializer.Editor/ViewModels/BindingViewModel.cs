using System;
using System.ComponentModel;
using Windows.Foundation;

namespace BinarySerializer.Editor.ViewModels
{
    public class BindingViewModel : ViewModelBase
    {
        private const double ConnectorOffset = 12;

        public BindingViewModel(FieldViewModel source, FieldViewModel target)
        {
            Source = source;
            Target = target;

            Source.PropertyChanged += FieldOnPropertyChanged;
            Target.PropertyChanged += FieldOnPropertyChanged;
        }

        public FieldViewModel Source { get; }
        public FieldViewModel Target { get; }

        public Point SourceCornerPoint => new Point(ConnectorX, Source.AnchorPoint.Y);
        public Point TargetCornerPoint => new Point(ConnectorX, Target.AnchorPoint.Y);

        private double ConnectorX => Math.Max(Target.AnchorPoint.X, Source.AnchorPoint.X) + ConnectorOffset;

        private void FieldOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName != nameof(FieldViewModel.AnchorPoint))
            {
                return;
            }

            OnPropertyChanged(nameof(SourceCornerPoint));
            OnPropertyChanged(nameof(TargetCornerPoint));
        }
    }
}