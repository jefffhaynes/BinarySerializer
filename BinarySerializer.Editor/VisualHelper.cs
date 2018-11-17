using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace BinarySerializer.Editor
{
    public static class VisualHelper
    {
        public static TElement FindVisualParent<TElement>(this UIElement element) where TElement : UIElement
        {
            UIElement parent = element;

            while (parent != null)
            {
                if (parent is TElement e)
                {
                    return e;
                }

                parent = VisualTreeHelper.GetParent(parent) as UIElement;
            }

            return null;
        }
    }
}
