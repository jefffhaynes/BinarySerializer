using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BinarySerializer.Editor.ViewModels;

namespace BinarySerializer.Editor
{
    public static class AssemblyLoader
    {
        public static IEnumerable<ObjectViewModel> Load(string assemblyPath)
        {
            var assembly = Assembly.LoadFile(assemblyPath);
            return assembly.DefinedTypes.Where(type => type.IsClass).Select(type => new ClassViewModel(type));
        }
    }
}
