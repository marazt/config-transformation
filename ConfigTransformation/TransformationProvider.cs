using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Marazt.Commons.Xml;


namespace Marazt.ConfigTransformation
{
    /// <summary>
    /// Transformation class
    /// </summary>
    public class TransformationProvider
    {
        #region Constants

        /// <summary>
        /// The source configuration file pattern
        /// </summary>
        public const string SourceConfigFilePattern = ".+\\.config";

        /// <summary>
        /// The tranformation file pattern
        /// </summary>
        public const string TranformationFilePattern = "(.+)\\.(.+)\\.config";

        /// <summary>
        /// The tranformation file source match index
        /// </summary>
        public const int TranformationFileSourceMatchIndex = 1;

        /// <summary>
        /// The configuration extension
        /// </summary>
        private const string ConfigExtension = ".config";

        /// <summary>
        /// The backup extension
        /// </summary>
        private const string BackupExtension = ".bak";

        /// <summary>
        /// The supported project extensions
        /// </summary>
        private static readonly List<string> SupportedProjectExtensions = new List<string>
        {
            ".csproj",
            ".vbproj",
            ".fsproj",
            ""//website
        };

        #endregion Constants

        #region Methods


        /// <summary>
        /// Determines whether [is project supported] [the specified project extension].
        /// </summary>
        /// <param name="projectExtension">The project extension.</param>
        /// <returns>[True] if project is supported, otherwise [False]</returns>
        public bool IsProjectSupported(string projectExtension)
        {
            if (string.IsNullOrEmpty(projectExtension))
            {
                return false;
            }
            return SupportedProjectExtensions.Contains(Path.GetExtension(projectExtension).ToLower());
        }


        /// <summary>
        /// Checks the transformation file and get source file from it.
        /// </summary>
        /// <param name="transformationFileName">Name of the transformation file.</param>
        /// <param name="transformationFilePattern">The transformation file pattern.</param>
        /// <param name="tranformationFileSourceMatchIndex">Index of the tranformation file source match.</param>
        /// <param name="sourceFileName">Name of the source file.</param>
        /// <returns>
        /// [True] if transformation file match pattern, otherwise [False]
        /// </returns>
        private bool CheckTransformationFileAndGetSourceFileFromIt(string transformationFileName, string transformationFilePattern,
            int tranformationFileSourceMatchIndex, out string sourceFileName)
        {
            sourceFileName = null;
            if (string.IsNullOrEmpty(transformationFileName))
            {
                return false;
            }
            var match = Regex.Match(Path.GetFileName(transformationFileName), transformationFilePattern, RegexOptions.IgnoreCase);
            if (match.Success && tranformationFileSourceMatchIndex >= 0 && tranformationFileSourceMatchIndex < match.Groups.Count)
            {
                // ReSharper disable once AssignNullToNotNullAttribute
                sourceFileName = Path.Combine(Path.GetDirectoryName(transformationFileName),
                    match.Groups[tranformationFileSourceMatchIndex].Value + ConfigExtension);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Determines whether [is transformation file] [the specified file name].
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="transformationFilePattern">The transformation file pattern.</param>
        /// <param name="tranformationFileSourceMatchIndex">Index of the tranformation file source match.</param>
        /// <returns>
        /// [True] if transformation file match pattern, otherwise [False]
        /// </returns>
        public bool IsTransformationFile(string fileName, string transformationFilePattern, int tranformationFileSourceMatchIndex)
        {
            string sourceFileName;
            return this.CheckTransformationFileAndGetSourceFileFromIt(fileName, transformationFilePattern,
                tranformationFileSourceMatchIndex, out sourceFileName);
        }

        /// <summary>
        /// Gets the backup file name of file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>Name of the backup source file</returns>
        private string GetBackupFileNameOfFile(string fileName)
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            return Path.Combine(Path.GetDirectoryName(fileName), Path.GetFileName(fileName) + BackupExtension);
        }

        /// <summary>
        /// Creates the backup file of file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="backupFileName">Name of the backup file.</param>
        private void CreateBackupFileOfFile(string fileName, string backupFileName)
        {
            File.Copy(fileName, backupFileName, true);

        }

        /// <summary>
        /// Deletes the backup file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        private void DeleteBackupFile(string fileName)
        {
            File.Delete(fileName);
        }

        /// <summary>
        /// Transforms the specified transofrmation file name.
        /// </summary>
        /// <param name="transofrmationFileName">Name of the transofrmation file.</param>
        /// <param name="transformationFilePattern">The transformation file pattern.</param>
        /// <param name="tranformationFileSourceMatchIndex">Index of the tranformation file source match.</param>
        public void Transform(string transofrmationFileName, string transformationFilePattern, int tranformationFileSourceMatchIndex)
        {
            string sourceFileName;

            var result = this.CheckTransformationFileAndGetSourceFileFromIt(transofrmationFileName, transformationFilePattern,
                tranformationFileSourceMatchIndex, out sourceFileName);

            if (!result)
            {
                Logger.LogInfo(Resources.InvalidTransformationFileName);
                return;
            }

            var backupFileName = this.GetBackupFileNameOfFile(sourceFileName);

            try
            {
                this.DeleteBackupFile(backupFileName);
                Logger.LogInfo(string.Format(Resources.DeletionOfBackupFileDone, backupFileName));

                this.CreateBackupFileOfFile(sourceFileName, backupFileName);
                Logger.LogInfo(string.Format(Resources.CopyOfSourceFileDone, sourceFileName, backupFileName));

                result = TransformationManager.Transform(backupFileName, transofrmationFileName, sourceFileName, new TransformationLogger());
                Logger.LogInfo(result
                    ? string.Format(Resources.TransformationOfFileSuccessfullyDone, backupFileName,
                        transofrmationFileName, sourceFileName)
                    : string.Format(Resources.TransformationProcessError, backupFileName, transofrmationFileName,
                        sourceFileName));

                this.DeleteBackupFile(backupFileName);
                Logger.LogInfo(string.Format(Resources.DeletionOfBackupFileDone, backupFileName));
            }
            catch (Exception ex)
            {
                Logger.LogError(string.Format(Resources.ErrorWhileTransformingFile, backupFileName, transofrmationFileName));
                Logger.LogError(ex);
            }


        }


        #endregion Methods


    }
}
