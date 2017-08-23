using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace BinarySerializer.Editor.Controls
{
    public sealed partial class FieldControl : UserControl
    {
        public static readonly DependencyProperty AnchorPointProperty = DependencyProperty.Register(
            "AnchorPoint", typeof(Point), typeof(FieldControl), new PropertyMetadata(default(Point)));

        public Point AnchorPoint
        {
            get => (Point) GetValue(AnchorPointProperty);
            set => SetValue(AnchorPointProperty, value);
        }

        public FieldControl()
        {
            InitializeComponent();

            LayoutUpdated += OnLayoutUpdated;
        }

        private void OnLayoutUpdated(object sender, object o)
        {
            AnchorPoint = TransformToVisual(Window.Current.Content).TransformPoint(new Point(ActualWidth, ActualHeight / 2));
        }
    }
}
