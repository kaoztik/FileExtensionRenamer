using System;
using System.Windows.Input;

namespace FileExtensionRenamer.ViewModel
{
    /// <summary>
    /// The delegate command.
    /// </summary>
    public class DelegateCommand : ICommand
    {
        #region Fields

        /// <summary>
        /// The canExecute.
        /// </summary>
        private readonly Predicate<object> _canExecute;

        /// <summary>
        /// The execute.
        /// </summary>
        private readonly Action<object> _execute;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateCommand" /> class.
        /// </summary>
        /// <param name="execute">
        /// The execute.
        /// </param>
        /// <param name="canExecute">
        /// The can execute.
        /// </param>
        public DelegateCommand(Action<object> execute, Predicate<object> canExecute)
        {
            this._execute = execute;
            this._canExecute = canExecute;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateCommand" /> class.
        /// </summary>
        /// <param name="execute">
        /// The execute.
        /// </param>
        public DelegateCommand(Action<object> execute)
            : this(execute, null)
        {
        }

        #endregion

        #region Public Events

        /// <summary>
        /// The can execute changed.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The can execute.
        /// </summary>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        /// <returns>
        /// The <see cref="bool" />.
        /// </returns>
        public bool CanExecute(object parameter)
        {
            if (this._canExecute == null)
            {
                return true;
            }

            return this._canExecute(parameter);
        }

        /// <summary>
        /// The execute.
        /// </summary>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        public void Execute(object parameter)
        {
            this._execute(parameter);
        }

        /// <summary>
        /// The raise can execute changed.
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            if (this.CanExecuteChanged != null)
            {
                this.CanExecuteChanged(this, EventArgs.Empty);
            }
        }

        #endregion
    }
}