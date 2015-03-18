using System;
using BinarySerialization.Graph.TypeGraph;

namespace BinarySerializer.Editor.ViewModels
{
    public abstract class ObjectViewModel : ViewModelBase
    {
        protected ObjectViewModel(Type type)
        {
            Type = type;
        }

        public Type Type { get; set; }
    }
}
