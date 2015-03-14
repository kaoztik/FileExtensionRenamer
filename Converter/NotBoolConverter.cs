using System;
using System.Globalization;

namespace FileExtensionRenamer.Converter
{
    /// <summary>
    /// A Not-Bool converter
    /// </summary>
    public class NotBoolConverter : ConverterBase
    {
        #region Public Methods and Operators

        /// <summary>
        /// The convert method.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">The targettype.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="culture">The culture.</param>
        /// <returns>
        /// The converted value.
        /// </returns>
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !((bool)value);
        }

        /// <summary>
        /// Convertds the back.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="culture">The culture.</param>
        /// <returns></returns>
        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !((bool)value);
        }

        #endregion
    }
}