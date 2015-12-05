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
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MainViewModel" /> class.
        /// </summary>
        public MainViewModel()
        {
            UntouchedFiles = new ObservableCollection<string>();
            CorrectedFiles = new ObservableCollection<string>();
            ProgressValue = 0;
        }

        #endregion

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

        #region Public Properties

        /// <summary>
        /// Gets the command replace.
        /// </summary>
        /// <value>
        /// The command replace.
        /// </value>
        public DelegateCommand CommandReplace
            => _commandReplace ??
               (_commandReplace = new DelegateCommand(CommandReplaceExecute, CommandReplaceCanExecute));

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
                return _commandScan ?? (_commandScan = new DelegateCommand(CommandScanExecute, CommandScanCanExecute));
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
            get { return _correctedFiles; }
            set
            {
                if (value != _correctedFiles)
                {
                    _correctedFiles = value;
                    RaisePropertyChanged(() => CorrectedFiles);
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
            get { return _fileExtension; }
            set
            {
                if (value != _fileExtension)
                {
                    _fileExtension = value;
                    RaisePropertyChanged(() => FileExtension);
                    _scanned = false;
                    CommandReplace.RaiseCanExecuteChanged();
                    CommandScan.RaiseCanExecuteChanged();
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
            get { return _isBusy; }
            set
            {
                if (value != _isBusy)
                {
                    _isBusy = value;
                    RaisePropertyChanged(() => IsBusy);
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
            get { return _progressValue; }
            set
            {
                if (value != _progressValue)
                {
                    _progressValue = value;
                    RaisePropertyChanged(() => ProgressValue);
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
            get { return _replaceExtension; }
            set
            {
                if (value != _replaceExtension)
                {
                    _replaceExtension = value;
                    RaisePropertyChanged(() => ReplaceExtension);
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
            get { return _rootFolder; }
            set
            {
                if (value != _rootFolder)
                {
                    _rootFolder = value;
                    RaisePropertyChanged(() => RootFolder);
                    _scanned = false;
                    CommandReplace.RaiseCanExecuteChanged();
                    CommandScan.RaiseCanExecuteChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the files should be removed from the Filesystem.
        /// </summary>
        /// <value>
        /// <c>true</c> if the files should be removed from the Filesystem; otherwise, a copy with the new filename will be
        /// created
        /// <c>false</c>.
        /// </value>
        public bool ShouldRemove
        {
            get { return _shouldRemove; }
            set
            {
                if (value != _shouldRemove)
                {
                    _shouldRemove = value;
                    RaisePropertyChanged(() => ShouldRemove);
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
            get { return _untouchedFiles; }
            set
            {
                if (value != _untouchedFiles)
                {
                    _untouchedFiles = value;
                    RaisePropertyChanged(() => UntouchedFiles);
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
            return UntouchedFiles.Any() && _scanned;
        }

        /// <summary>
        /// Commands the replace execute.
        /// </summary>
        /// <param name="obj">The object.</param>
        private async void CommandReplaceExecute(object obj)
        {
            //TODO implement cancellationToken
            if (IsBusy)
            {
                return;
            }

            IsBusy = true;
            var token = CancellationToken.None;
            try
            {
                var result = new BlockingCollection<string>();

                //Producer
                Task task;

                FileExtension = FileExtension.StartsWith(".") ? FileExtension : "." + FileExtension;

                if (string.IsNullOrEmpty(ReplaceExtension))
                {
                    // just remove
                    task = RenamerCore.RemoveExtensions(
                        ShouldRemove,
                        RootFolder,
                        FileExtension,
                        UntouchedFiles,
                        result,
                        token,
                        new Progress<int>(progress => ProgressValue = progress));
                }
                else
                {
                    ReplaceExtension = ReplaceExtension.StartsWith(".")
                        ? ReplaceExtension
                        : "." + ReplaceExtension;
                    //Replace
                    task = RenamerCore.ReplaceExtensions(
                        ShouldRemove,
                        RootFolder,
                        FileExtension,
                        ReplaceExtension,
                        UntouchedFiles,
                        result,
                        token,
                        new Progress<int>(progress => ProgressValue = progress));
                }

                //Consumer
                var synchContext = SynchronizationContext.Current;
                await Task.Run(
                    () =>
                    {
                        Action<string> addAction = i => CorrectedFiles.Add(i);
                        foreach (var item in result.GetConsumingEnumerable(token))
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

            _scanned = false;
            IsBusy = false;
            CommandReplace.RaiseCanExecuteChanged();
        }

        /// <summary>
        /// Commands the scan can execute.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns></returns>
        private bool CommandScanCanExecute(object obj)
        {
            return !string.IsNullOrEmpty(FileExtension) && !string.IsNullOrEmpty(RootFolder);
        }

        /// <summary>
        /// Commands the scan execute.
        /// </summary>
        /// <param name="obj">The object.</param>
        private async void CommandScanExecute(object obj)
        {
            //TODO implement cancellationToken
            if (IsBusy)
            {
                return;
            }

            IsBusy = true;
            CorrectedFiles.Clear();
            var token = CancellationToken.None;
            try
            {
                UntouchedFiles.Clear();
                var result = new BlockingCollection<string>();

                //Producer
                var task = RenamerCore.ScanDirectory(
                    RootFolder,
                    FileExtension,
                    result,
                    token,
                    new Progress<int>(progress => ProgressValue = progress));

                //Consumer
                var synchContext = SynchronizationContext.Current;
                await Task.Run(
                    () =>
                    {
                        Action<string> addAction = i => UntouchedFiles.Add(i);
                        foreach (var item in result.GetConsumingEnumerable(token))
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

            _scanned = true;
            IsBusy = false;
            CommandReplace.RaiseCanExecuteChanged();
        }

        #endregion
    }
}