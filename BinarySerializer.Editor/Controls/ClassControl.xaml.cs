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
            Size size = RenderSize;
            Point ofs = new Point(size.Width, size.Height / 2);

            var classControl = GetClassControl();

            if (classControl == null)
            {
                return;
            }

            // TODO change to canvas
            AnchorPoint = TransformToVisual(classControl).TransformPoint(ofs);
        }

        private ClassControl _classControl;

        private ClassControl GetClassControl()
        {
            return _classControl ?? (_classControl = VisualHelper.GetAncestor<ClassControl>(this));
        }
    }
}
