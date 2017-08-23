using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace BinarySerializer.Editor.Controls
{
    public sealed partial class ClassControl : UserControl
    {
        public ClassControl()
        {
            InitializeComponent();

            LayoutUpdated += OnLayoutUpdated;
        }

        public static readonly DependencyProperty AnchorPointProperty = DependencyProperty.Register(
            "AnchorPoint", typeof(Point), typeof(ClassControl), new PropertyMetadata(default(Point)));

        public Point AnchorPoint
        {
            get => (Point)GetValue(AnchorPointProperty);
            set => SetValue(AnchorPointProperty, value);
        }

        private void OnLayoutUpdated(object sender, object o)
        {
            AnchorPoint = TransformToVisual(Window.Current.Content).TransformPoint(new Point(ActualWidth, ActualHeight / 2));
        }
    }
}
