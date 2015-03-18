using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using BinarySerializer.Editor.ViewModels;
using Microsoft.Win32;

namespace BinarySerializer.Editor
{
    public partial class MainWindow : Window
    {
        private readonly ViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();
            _viewModel = new ViewModel();
            DataContext = _viewModel;
        }

        private void AddAssmeblyButton_OnClick(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();

            if (!dialog.ShowDialog().Value) 
                return;

            var objectViewModels = dialog.FileNames.SelectMany(AssemblyLoader.Load);

            foreach (var objectViewModel in objectViewModels)
                _viewModel.Objects.Add(objectViewModel);
        }
    }
}
