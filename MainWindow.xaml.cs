﻿using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;

using FileExtensionRenamer.ViewModel;

using Microsoft.Win32;

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
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.CheckFileExists = true;
            dialog.Multiselect = false;
            bool? result = dialog.ShowDialog();

            if (result.HasValue && result.Value)
            {
                var vm = this.DataContext as MainViewModel;
                if (vm != null)
                {
                    vm.FileExtension = "." + dialog.SafeFileName.Split('.').LastOrDefault();
                    vm.RootFolder = Path.GetDirectoryName(dialog.FileName);
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
            var dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var vm = this.DataContext as MainViewModel;
                if (vm != null)
                {
                    vm.RootFolder = dialog.SelectedPath;
                }
            }
        }

        #endregion
    }
}