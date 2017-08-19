using Windows.UI.Xaml.Controls;
using BinarySerializer.Editor.ViewModels;

namespace BinarySerializer.Editor
{
    public sealed partial class MainPage : Page
    {
        private static readonly ViewModel ViewModel = new ViewModel();

        public MainPage()
        {
            InitializeComponent();
            DataContext = ViewModel;
        }
    }
}
