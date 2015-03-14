using System;
using System.ComponentModel;
using System.Linq.Expressions;

namespace FileExtensionRenamer.ViewModel
{
    /// <summary>
    /// The base view model.
    /// </summary>
    public class BaseViewModel : INotifyPropertyChanged
    {
        #region Public Events

        /// <summary>
        /// The property changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Methods

        /// <summary>
        /// The raise property changed.
        /// </summary>
        /// <param name="propName">The prop name.</param>
        protected void RaisePropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }

        /// <summary>
        /// The raise property changed.
        /// </summary>
        /// <remarks>
        /// this allows calls like RaisepropertyChanged(() => Property)
        /// </remarks>
        /// <typeparam name="TResult">The type of the Property.</typeparam>
        /// <param name="propertyExpression">The property expression.</param>
        protected void RaisePropertyChanged<TResult>(Expression<Func<TResult>> propertyExpression)
        {
            this.RaisePropertyChanged(((MemberExpression)propertyExpression.Body).Member.Name);
        }

        #endregion
    }
}