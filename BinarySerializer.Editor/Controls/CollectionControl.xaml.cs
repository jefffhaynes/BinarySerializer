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
            var page = GetPage();
            AnchorPoint = TransformToVisual(page).TransformPoint(new Point(ActualWidth, ActualHeight / 2));
        }

        private Page _page;

        private Page GetPage()
        {
            return _page ?? (_page = this.FindVisualParent<Page>());
        }
    }
}
