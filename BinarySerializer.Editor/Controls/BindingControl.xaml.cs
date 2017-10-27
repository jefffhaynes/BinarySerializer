using System;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace BinarySerializer.Editor.Controls
{
    public sealed partial class BindingControl : UserControl
    {
        private const double ConnectorOffset = 12;

        public static readonly DependencyProperty TargetProperty = DependencyProperty.Register(
            "Target", typeof(Point), typeof(BindingControl),
            new PropertyMetadata(default(Point), TargetPropertyChangedCallback));

        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(
            "Source", typeof(Point), typeof(BindingControl),
            new PropertyMetadata(default(Point), SourcePropertyChangedCallback));

        public static readonly DependencyProperty TransformedTargetProperty = DependencyProperty.Register(
            "TransformedTarget", typeof(Point), typeof(BindingControl), new PropertyMetadata(default(Point)));

        public static readonly DependencyProperty TransformedSourceProperty = DependencyProperty.Register(
            "TransformedSource", typeof(Point), typeof(BindingControl), new PropertyMetadata(default(Point)));

        public static readonly DependencyProperty TargetCornerProperty = DependencyProperty.Register(
            "TargetCorner", typeof(Point), typeof(BindingControl), new PropertyMetadata(default(Point)));

        public static readonly DependencyProperty SourceCornerProperty = DependencyProperty.Register(
            "SourceCorner", typeof(Point), typeof(BindingControl), new PropertyMetadata(default(Point)));

        private ItemsControl _itemsControl;


        private Page _page;

        public BindingControl()
        {
            InitializeComponent();
        }

        public Point TargetCorner
        {
            get => (Point) GetValue(TargetCornerProperty);
            set => SetValue(TargetCornerProperty, value);
        }

        public Point SourceCorner
        {
            get => (Point) GetValue(SourceCornerProperty);
            set => SetValue(SourceCornerProperty, value);
        }

        public Point Target
        {
            get => (Point) GetValue(TargetProperty);
            set => SetValue(TargetProperty, value);
        }

        public Point Source
        {
            get => (Point) GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }

        public Point TransformedTarget
        {
            get => (Point) GetValue(TransformedTargetProperty);
            set => SetValue(TransformedTargetProperty, value);
        }

        public Point TransformedSource
        {
            get => (Point) GetValue(TransformedSourceProperty);
            set => SetValue(TransformedSourceProperty, value);
        }

        private static void SourcePropertyChangedCallback(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var control = (BindingControl) dependencyObject;
            control.UpdatePoints();
        }

        private static void TargetPropertyChangedCallback(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var control = (BindingControl) dependencyObject;
            control.UpdatePoints();
        }

        private void UpdatePoints()
        {
            var page = GetPage();
            var itemsControl = GetItemsControl();

            var transform = page.TransformToVisual(itemsControl);
            TransformedTarget = transform.TransformPoint(Target);
            TransformedSource = transform.TransformPoint(Source);

            var connectorX = Math.Max(TransformedTarget.X, TransformedSource.X) + ConnectorOffset;

            TargetCorner = new Point(connectorX, TransformedTarget.Y);
            SourceCorner = new Point(connectorX, TransformedSource.Y);
        }

        private ItemsControl GetItemsControl()
        {
            return _itemsControl ?? (_itemsControl = this.FindVisualParent<ItemsControl>());
        }

        private Page GetPage()
        {
            return _page ?? (_page = this.FindVisualParent<Page>());
        }
    }
}