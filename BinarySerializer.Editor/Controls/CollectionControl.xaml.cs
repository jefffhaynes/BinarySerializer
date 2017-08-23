using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace BinarySerializer.Editor.Controls
{
    public sealed partial class CollectionControl : UserControl
    {
        public CollectionControl()
        {
            InitializeComponent();

            LayoutUpdated += OnLayoutUpdated;
        }

        public static readonly DependencyProperty AnchorPointProperty = DependencyProperty.Register(
            "AnchorPoint", typeof(Point), typeof(CollectionControl), new PropertyMetadata(default(Point)));

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
