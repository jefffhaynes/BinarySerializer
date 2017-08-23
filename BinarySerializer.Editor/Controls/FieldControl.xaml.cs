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

            Size size = RenderSize;
            Point ofs = new Point(size.Width, size.Height/2);

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
