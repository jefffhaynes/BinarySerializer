using System;
using BinarySerialization;
using BinarySerializer.Editor.ViewModels;
using IValueConverter = Windows.UI.Xaml.Data.IValueConverter;

namespace BinarySerializer.Editor.Converters
{
    public class TypeDescriptionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var fieldViewModel = (FieldViewModel) value;

            var serializedType = fieldViewModel.SerializedType;

            switch (serializedType)
            {
                case SerializedType.Default:
                    return fieldViewModel.Type;
                case SerializedType.Int1:
                case SerializedType.UInt1:
                case SerializedType.Int2:
                case SerializedType.UInt2:
                case SerializedType.Int4:
                case SerializedType.UInt4:
                case SerializedType.Int8:
                case SerializedType.UInt8:
                case SerializedType.Float4:
                case SerializedType.Float8:
                    return serializedType;
                case SerializedType.ByteArray:
                    return "Byte array";
                case SerializedType.SizedString:
                    return "String";
                case SerializedType.NullTerminatedString:
                    return "Null-terminated string";
                case SerializedType.LengthPrefixedString:
                    return "Length-prefixed string";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
