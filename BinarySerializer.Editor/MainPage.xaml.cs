using System.Diagnostics;
using System.IO;
using System.Runtime.Loader;
using Windows.UI.Xaml.Controls;
using BinarySerializer.Editor.ViewModels;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

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

        private void TextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var mscorlib = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);

            var syntaxTree = CSharpSyntaxTree.ParseText(CodeTextBox.Text);
            var compilation = CSharpCompilation.Create("compilation", new[] {syntaxTree}, new[] {mscorlib})
                .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            var stream = new MemoryStream();

            var result = compilation.Emit(stream);

            if (!result.Success)
            {
                foreach (var resultDiagnostic in result.Diagnostics)
                {
                    Debug.WriteLine(resultDiagnostic.ToString());
                }
            }

            AssemblyLoadContext.Default.LoadFromStream(stream);
        }
    }
}
