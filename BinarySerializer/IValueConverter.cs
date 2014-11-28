namespace BinarySerialization
{
    /// <summary>
    /// Provides a way to apply custom logic to a binding.
    /// </summary>
    /// <seealso cref="BinarySerializer"/>
    public interface IValueConverter
    {
        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value to be converted.</param>
        /// <param name="parameter">The converter parameter.</param>
        /// <param name="ctx">The current serialization context.</param>
        /// <returns></returns>
        object Convert(object value, object parameter, BinarySerializationContext ctx);

        object ConvertBack(object value, object parameter, BinarySerializationContext ctx);
    }
}
