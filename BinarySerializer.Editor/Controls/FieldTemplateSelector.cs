using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using BinarySerializer.Editor.ViewModels;

namespace BinarySerializer.Editor.Controls
{
    public class FieldTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ValueTemplate { get; set; }

        public DataTemplate CollectionTemplate { get; set; }

        public DataTemplate ClassTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            if (item is CollectionViewModel)
            {
                return CollectionTemplate;
            }

            if (item is ObjectViewModel)
            {
                return ClassTemplate;
            }

            if (item is FieldViewModel)
            {
                return ValueTemplate;
            }

            return base.SelectTemplateCore(item);
        }
    }
}
