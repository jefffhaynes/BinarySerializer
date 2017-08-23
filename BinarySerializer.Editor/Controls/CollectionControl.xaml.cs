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

            Size size = RenderSize;
            Point ofs = new Point(size.Width, size.Height / 2);

            var canvas = GetClassControl();

            // TODO change to canvas
            AnchorPoint = TransformToVisual(canvas).TransformPoint(ofs);
        }

        private ClassControl _classControl;

        private ClassControl GetClassControl()
        {
            return _classControl ?? (_classControl = VisualHelper.GetAncestor<ClassControl>(this));
        }
    }
}
