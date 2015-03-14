using System;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace FileExtensionRenamer
{
    /// <summary>
    /// Handles all the Fileoperations
    /// </summary>
    public static class RenamerCore
    {
        #region Public Methods and Operators

        /// <summary>
        /// Removes the file extensions.
        /// </summary>
        /// <param name="shouldRemove">if set to <c>true</c> the files will be moved, otherwise copied.</param>
        /// <param name="pathToRootDirectoy">The path to root directoy.</param>
        /// <param name="oldExtension">The old file extension.</param>
        /// <param name="relativeFileList">The relative file list.</param>
        /// <param name="resultQueue">The result queue.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="progress">The progress.</param>
        /// <returns>A task</returns>
        public static Task RemoveExtensions(
            bool shouldRemove,
            string pathToRootDirectoy,
            string oldExtension,
            ObservableCollection<string> relativeFileList,
            BlockingCollection<string> resultQueue,
            CancellationToken cancellationToken,
            IProgress<int> progress)
        {
            return Task.Run(
                () =>
                    {
                        var progressStep = (int)Math.Ceiling((decimal)100 / relativeFileList.Count());
                        var progressEvery = (int)Math.Ceiling(relativeFileList.Count() / (decimal)100);
                        int progressValue = 0;
                        Action<string, string> fileAction;
                        if (shouldRemove)
                        {
                            fileAction = File.Move;
                        }
                        else
                        {
                            fileAction = File.Copy;
                        }

                        for (int i = 0; i < relativeFileList.Count(); i++)
                        {
                            string oldFileName = pathToRootDirectoy + relativeFileList[i];
                            string newFileName = oldFileName.Remove(oldFileName.Length - oldExtension.Length + 1);
                            try
                            {
                                fileAction(oldFileName, newFileName);
                                resultQueue.Add(newFileName.Replace(pathToRootDirectoy, string.Empty), cancellationToken);
                            }
                            catch (Exception e)
                            {
                                //MessageBox.Show(e.Message);
                                resultQueue.Add("Error");
                            }
                            cancellationToken.ThrowIfCancellationRequested();
                            if (i % progressEvery == 0)
                            {
                                progress.Report(progressValue += progressStep);
                            }
                        }

                        progress.Report(100);
                        resultQueue.CompleteAdding();
                    }, cancellationToken);
        }

        /// <summary>
        /// Replaces the file extensions.
        /// </summary>
        /// <param name="shouldRemove">if set to <c>true</c> the files will be moved, otherwise copied.</param>
        /// <param name="pathToRootDirectoy">The path to root directoy.</param>
        /// <param name="oldExtension">The old file extension.</param>
        /// <param name="replaceString">The new replace string.</param>
        /// <param name="relativeFileList">The relative file list.</param>
        /// <param name="resultQueue">The result queue.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="progress">The progress.</param>
        /// <returns>A task.</returns>
        public static Task ReplaceExtensions(
            bool shouldRemove,
            string pathToRootDirectoy,
            string oldExtension,
            string replaceString,
            ObservableCollection<string> relativeFileList,
            BlockingCollection<string> resultQueue,
            CancellationToken cancellationToken,
            IProgress<int> progress)
        {
            return Task.Run(
                () =>
                    {
                        var progressStep = (int)Math.Ceiling((decimal)100 / relativeFileList.Count());
                        var progressEvery = (int)Math.Ceiling(relativeFileList.Count() / (decimal)100);
                        int progressValue = 0;
                        Action<string, string> fileAction;
                        if (shouldRemove)
                        {
                            fileAction = File.Move;
                        }
                        else
                        {
                            fileAction = File.Copy;
                        }

                        for (int i = 0; i < relativeFileList.Count(); i++)
                        {
                            string oldFileName = pathToRootDirectoy + relativeFileList[i];
                            string newFileName = oldFileName.Remove(oldFileName.Length - oldExtension.Length)
                                                 + replaceString;
                            try
                            {
                                fileAction(oldFileName, newFileName);
                                resultQueue.Add(newFileName.Replace(pathToRootDirectoy, string.Empty));
                            }
                            catch (Exception e)
                            {
                                //MessageBox.Show(e.Message);
                                resultQueue.Add("Error");
                            }
                            cancellationToken.ThrowIfCancellationRequested();
                            if (i % progressEvery == 0)
                            {
                                progress.Report(progressValue += progressStep);
                            }
                        }

                        progress.Report(100);
                        resultQueue.CompleteAdding();
                    }, cancellationToken);
        }

        /// <summary>
        /// Scans the directory.
        /// </summary>
        /// <param name="pathToRootDirectory">The path to root directory.</param>
        /// <param name="extensionToRemove">The filter file extension.</param>
        /// <param name="resultQueue">The result queue.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="progress">The progress.</param>
        /// <returns></returns>
        public static Task ScanDirectory(
            string pathToRootDirectory,
            string extensionToRemove,
            BlockingCollection<string> resultQueue,
            CancellationToken cancellationToken,
            IProgress<int> progress)
        {
            return Task.Run(
                () =>
                    {
                        string pattern = "*" + extensionToRemove;
                        String[] allFiles;
                        try
                        {
                            //TODO Exclude Folder+Files where access is not allowed and bring up a warning message that not everything could be included.
                            allFiles = Directory.GetFiles(pathToRootDirectory, pattern, SearchOption.AllDirectories);
                        }
                        catch (Exception e)
                        {
                            MessageBox.Show(e.Message, "Error", MessageBoxButton.OK);
                            return;
                        }

                        var progressStep = (int)Math.Ceiling((decimal)100 / allFiles.Length);
                        var progressEvery = (int)Math.Ceiling(allFiles.Length / (decimal)100);
                        int progressValue = 0;

                        for (int i = 0; i < allFiles.Length; i++)
                        {
                            // TODO scan all files in advance if there might be a conflict.
                            resultQueue.Add(allFiles[i].Replace(pathToRootDirectory, string.Empty));
                            cancellationToken.ThrowIfCancellationRequested();
                            if (i % progressEvery == 0)
                            {
                                progress.Report(progressValue += progressStep);
                            }
                        }

                        progress.Report(100);
                        resultQueue.CompleteAdding();
                    });
        }

        #endregion
    }
}