using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

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
