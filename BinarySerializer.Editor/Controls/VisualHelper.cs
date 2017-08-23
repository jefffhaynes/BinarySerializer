using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace BinarySerializer.Editor.Controls
{
    public static class VisualHelper
    {
        public static TAncestor GetAncestor<TAncestor>(DependencyObject decendant) where TAncestor : DependencyObject
        {
            DependencyObject parent = decendant;
            while ((parent = VisualTreeHelper.GetParent(parent)) != null)
            {
                if (parent is TAncestor ancestor)
                {
                    return ancestor;
                }
            }

            return null;
        }
    }
}
