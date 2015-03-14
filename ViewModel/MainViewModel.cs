using System;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace FileExtensionRenamer.ViewModel
{
    /// <summary>
    /// The Main ViewModel.
    /// </summary>
    public class MainViewModel : BaseViewModel
    {
        #region Fields

        private DelegateCommand _commandReplace;

        private DelegateCommand _commandScan;

        private ObservableCollection<string> _correctedFiles;

        private string _fileExtension;

        private bool _isBusy;

        private int _progressValue;

        private string _replaceExtension;

        private string _rootFolder;

        private bool _scanned;

        private bool _shouldRemove;

        private ObservableCollection<string> _untouchedFiles;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MainViewModel" /> class.
        /// </summary>
        public MainViewModel()
        {
            this.UntouchedFiles = new ObservableCollection<string>();
            this.CorrectedFiles = new ObservableCollection<string>();
            this.ProgressValue = 0;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the command replace.
        /// </summary>
        /// <value>
        /// The command replace.
        /// </value>
        public DelegateCommand CommandReplace
        {
            get
            {
                return this._commandReplace
                       ?? (this._commandReplace =
                           new DelegateCommand(this.CommandReplaceExecute, this.CommandReplaceCanExecute));
            }
        }

        /// <summary>
        /// Gets the command scan.
        /// </summary>
        /// <value>
        /// The command scan.
        /// </value>
        public DelegateCommand CommandScan
        {
            get
            {
                return this._commandScan
                       ?? (this._commandScan = new DelegateCommand(this.CommandScanExecute, this.CommandScanCanExecute));
            }
        }

        /// <summary>
        /// Gets or sets the corrected files.
        /// </summary>
        /// <value>
        /// The corrected files.
        /// </value>
        public ObservableCollection<string> CorrectedFiles
        {
            get
            {
                return this._correctedFiles;
            }
            set
            {
                if (value != this._correctedFiles)
                {
                    this._correctedFiles = value;
                    this.RaisePropertyChanged(() => this.CorrectedFiles);
                }
            }
        }

        /// <summary>
        /// Gets or sets the old file extension.
        /// </summary>
        /// <value>
        /// The file extension.
        /// </value>
        public string FileExtension
        {
            get
            {
                return this._fileExtension;
            }
            set
            {
                if (value != this._fileExtension)
                {
                    this._fileExtension = value;
                    this.RaisePropertyChanged(() => this.FileExtension);
                    this._scanned = false;
                    this.CommandReplace.RaiseCanExecuteChanged();
                    this.CommandScan.RaiseCanExecuteChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is busy.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is busy; otherwise, <c>false</c>.
        /// </value>
        public bool IsBusy
        {
            get
            {
                return this._isBusy;
            }
            set
            {
                if (value != this._isBusy)
                {
                    this._isBusy = value;
                    this.RaisePropertyChanged(() => this.IsBusy);
                }
            }
        }

        /// <summary>
        /// Gets or sets the progress value.
        /// </summary>
        /// <value>
        /// The progress value.
        /// </value>
        public int ProgressValue
        {
            get
            {
                return this._progressValue;
            }
            set
            {
                if (value != this._progressValue)
                {
                    this._progressValue = value;
                    this.RaisePropertyChanged(() => this.ProgressValue);
                }
            }
        }

        /// <summary>
        /// Gets or sets the new replace extension.
        /// </summary>
        /// <value>
        /// The replace extension.
        /// </value>
        public string ReplaceExtension
        {
            get
            {
                return this._replaceExtension;
            }
            set
            {
                if (value != this._replaceExtension)
                {
                    this._replaceExtension = value;
                    this.RaisePropertyChanged(() => this.ReplaceExtension);
                }
            }
        }

        /// <summary>
        /// Gets or sets the root folder.
        /// </summary>
        /// <value>
        /// The root folder.
        /// </value>
        public string RootFolder
        {
            get
            {
                return this._rootFolder;
            }
            set
            {
                if (value != this._rootFolder)
                {
                    this._rootFolder = value;
                    this.RaisePropertyChanged(() => this.RootFolder);
                    this._scanned = false;
                    this.CommandReplace.RaiseCanExecuteChanged();
                    this.CommandScan.RaiseCanExecuteChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the files should be removed from the Filesystem.
        /// </summary>
        /// <value>
        /// <c>true</c> if the files should be removed from the Filesystem; otherwise, a copy with the new filename will be created
        /// <c>false</c>.
        /// </value>
        public bool ShouldRemove
        {
            get
            {
                return this._shouldRemove;
            }
            set
            {
                if (value != this._shouldRemove)
                {
                    this._shouldRemove = value;
                    this.RaisePropertyChanged(() => this.ShouldRemove);
                }
            }
        }

        /// <summary>
        /// Gets or sets the untouched files.
        /// </summary>
        /// <value>
        /// The untouched files.
        /// </value>
        public ObservableCollection<string> UntouchedFiles
        {
            get
            {
                return this._untouchedFiles;
            }
            set
            {
                if (value != this._untouchedFiles)
                {
                    this._untouchedFiles = value;
                    this.RaisePropertyChanged(() => this.UntouchedFiles);
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Commands the replace can execute.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns></returns>
        private bool CommandReplaceCanExecute(object obj)
        {
            return this.UntouchedFiles.Any() && this._scanned;
        }

        /// <summary>
        /// Commands the replace execute.
        /// </summary>
        /// <param name="obj">The object.</param>
        private async void CommandReplaceExecute(object obj)
        {
            //TODO implement cancellationToken
            if (this.IsBusy)
            {
                return;
            }

            this.IsBusy = true;
            CancellationToken token = CancellationToken.None;
            try
            {
                var result = new BlockingCollection<string>();

                //Producer
                Task task;

                this.FileExtension = this.FileExtension.StartsWith(".") ? this.FileExtension : "." + this.FileExtension;

                if (string.IsNullOrEmpty(this.ReplaceExtension))
                {
                    // just remove
                    task = RenamerCore.RemoveExtensions(
                        this.ShouldRemove,
                        this.RootFolder,
                        this.FileExtension,
                        this.UntouchedFiles,
                        result,
                        token,
                        new Progress<int>(progress => this.ProgressValue = progress));
                }
                else
                {
                    this.ReplaceExtension = this.ReplaceExtension.StartsWith(".")
                                                ? this.ReplaceExtension
                                                : "." + this.ReplaceExtension;
                    //Replace
                    task = RenamerCore.ReplaceExtensions(
                        this.ShouldRemove,
                        this.RootFolder,
                        this.FileExtension,
                        this.ReplaceExtension,
                        this.UntouchedFiles,
                        result,
                        token,
                        new Progress<int>(progress => this.ProgressValue = progress));
                }

                //Consumer
                SynchronizationContext synchContext = SynchronizationContext.Current;
                await Task.Run(
                    () =>
                        {
                            Action<string> addAction = i => this.CorrectedFiles.Add(i);
                            foreach (string item in result.GetConsumingEnumerable(token))
                            {
                                if (synchContext != null)
                                {
                                    synchContext.Post(action => addAction(item), null);
                                }
                                else
                                {
                                    addAction(item);
                                }
                            }
                        });

                await task;
            }
            catch (Exception e)
            {
                MessageBox.Show("Error while replacing" + Environment.NewLine + e.Message, "Error", MessageBoxButton.OK);
            }

            this._scanned = false;
            this.IsBusy = false;
            this.CommandReplace.RaiseCanExecuteChanged();
        }

        /// <summary>
        /// Commands the scan can execute.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns></returns>
        private bool CommandScanCanExecute(object obj)
        {
            return !string.IsNullOrEmpty(this.FileExtension) && !string.IsNullOrEmpty(this.RootFolder);
        }

        /// <summary>
        /// Commands the scan execute.
        /// </summary>
        /// <param name="obj">The object.</param>
        private async void CommandScanExecute(object obj)
        {
            //TODO implement cancellationToken
            if (this.IsBusy)
            {
                return;
            }

            this.IsBusy = true;
            this.CorrectedFiles.Clear();
            CancellationToken token = CancellationToken.None;
            try
            {
                this.UntouchedFiles.Clear();
                var result = new BlockingCollection<string>();

                //Producer
                Task task = RenamerCore.ScanDirectory(
                    this.RootFolder,
                    this.FileExtension,
                    result,
                    token,
                    new Progress<int>(progress => this.ProgressValue = progress));

                //Consumer
                SynchronizationContext synchContext = SynchronizationContext.Current;
                await Task.Run(
                    () =>
                        {
                            Action<string> addAction = i => this.UntouchedFiles.Add(i);
                            foreach (string item in result.GetConsumingEnumerable(token))
                            {
                                if (synchContext != null)
                                {
                                    synchContext.Post(action => addAction(item), null);
                                }
                                else
                                {
                                    addAction(item);
                                }
                            }
                        });

                await task;
            }
            catch (Exception e)
            {
                MessageBox.Show("Error while Scanning" + Environment.NewLine + e.Message, "Error", MessageBoxButton.OK);
            }

            this._scanned = true;
            this.IsBusy = false;
            this.CommandReplace.RaiseCanExecuteChanged();
        }

        #endregion
    }
}