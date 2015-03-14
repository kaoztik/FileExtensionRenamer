using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace FileExtensionRenamer.Converter
{
    /// <summary>
    /// The converter base.
    /// </summary>
    public abstract class ConverterBase : MarkupExtension, IValueConverter
    {
        #region Fields

        /// <summary>
        /// Field for the _converter;
        /// </summary>
        private object _converter;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The convert method.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="targetType">
        /// The targettype.
        /// </param>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        /// <param name="culture">
        /// The culture.
        /// </param>
        /// <returns>
        /// The converted value.
        /// </returns>
        public abstract object Convert(object value, Type targetType, object parameter, CultureInfo culture);

        /// <summary>
        /// The convert back method.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="targetType">
        /// The targettype.
        /// </param>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        /// <param name="culture">
        /// The culture.
        /// </param>
        /// <returns>
        /// The converted value.
        /// </returns>
        public abstract object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture);

        /// <summary>
        /// The provide value method.
        /// </summary>
        /// <param name="serviceProvider">
        /// The service provider.
        /// </param>
        /// <returns>
        /// A converter.
        /// </returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this._converter ?? (this._converter = Activator.CreateInstance(this.GetType()));
        }

        #endregion
    }
}