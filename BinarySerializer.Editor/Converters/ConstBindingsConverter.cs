using System;
using System.Collections.Generic;
using System.Linq;
using BinarySerialization;
using BinarySerializer.Editor.ViewModels;
using IValueConverter = Windows.UI.Xaml.Data.IValueConverter;

namespace BinarySerializer.Editor.Converters
{
    public class ConstBindingsConverter : IValueConverter
    {
        private static readonly IDictionary<BindingKind, string> BindingTerm = new Dictionary<BindingKind, string>
        {
            {BindingKind.Length, "byte"},
            {BindingKind.Count, "item"}
        };

        private static readonly IDictionary<BindingKind, string> BindingTermPlural = new Dictionary<BindingKind, string>
        {
            {BindingKind.Length, "bytes"},
            {BindingKind.Count, "items"}
        };

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var constBindings = (IList<ConstBindingViewModel>) value;

            if (constBindings.Count == 0)
            {
                return null;
            }

            var descriptions = constBindings.Select(binding =>
            {
                if (binding.Kind == BindingKind.Endianness)
                {
                    var endianness = (Endianness) binding.Value;
                    return endianness == Endianness.Big ? "Big Endian" : "Little Endian";
                }

                var term = binding.Value == 1 ? BindingTerm[binding.Kind] : BindingTermPlural[binding.Kind];
                return $"{binding.Value} {term}";
            });

            return $"[{string.Join(", ", descriptions)}]";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
