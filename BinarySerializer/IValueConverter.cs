namespace BinarySerialization
{
    /// <summary>
    ///     Provides a way to apply custom logic to a binding.
    /// </summary>
    /// <seealso cref="BinarySerializer" />
    public interface IValueConverter
    {
        /// <summary>
        ///     Converts a value from source to target.
        /// </summary>
        /// <param name="value">The value to be converted.</param>
        /// <param name="parameter">An optional converter parameter.</param>
        /// <param name="context">The current serialization context.</param>
        /// <returns></returns>
        object Convert(object value, object parameter, BinarySerializationContext context);

        /// <summary>
        ///     Converts a value from target to source.
        /// </summary>
        /// <param name="value">The value to be converted back.</param>
        /// <param name="parameter">An optional converter parameter.</param>
        /// <param name="context">The current serialization context.</param>
        /// <returns></returns>
        object ConvertBack(object value, object parameter, BinarySerializationContext context);
    }
}