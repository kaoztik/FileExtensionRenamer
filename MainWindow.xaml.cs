using System.Linq;
using System.Windows;

using FileExtensionRenamer.ViewModel;

using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace FileExtensionRenamer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow" /> class.
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();
            this.DataContext = new MainViewModel();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Handles the OnClick event of the PickExtension control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void PickExtension_OnClick(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.CheckFileExists = true;
            dialog.Multiselect = false;
            bool? result = dialog.ShowDialog();

            if (result.HasValue && result.Value)
            {
                var vm = this.DataContext as MainViewModel;
                if (vm != null)
                {
                    string extension = dialog.FileName.Split('.').LastOrDefault();
                    vm.FileExtension = "." + extension;
                }
            }
        }

        /// <summary>
        /// Handles the OnClick event of the PickFolder control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void PickFolder_OnClick(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            dialog.Multiselect = false;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                var vm = this.DataContext as MainViewModel;
                if (vm != null)
                {
                    vm.RootFolder = dialog.FileName;
                }
            }
        }

        #endregion
    }
}