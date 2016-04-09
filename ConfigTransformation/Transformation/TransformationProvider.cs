using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Xml;
using Marazt.Commons.Xml;
using Marazt.ConfigTransformation.Logging;

namespace Marazt.ConfigTransformation.Transformation
{
    /// <summary>
    /// Transformation class
    /// </summary>
    public static class TransformationProvider
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
        /// The nest transformation files
        /// </summary>
        public const bool NestTransformationFiles = false;

        /// <summary>
        /// The write attributes on a separate line
        /// </summary>
        public const bool WriteAttributesOnASeparateLine = false;

        /// <summary>
        /// The configuration extension
        /// </summary>
        private const string ConfigExtension = ".config";

        /// <summary>
        /// The backup extension
        /// </summary>
        private const string BackupExtension = ".bak";

        /// <summary>
        /// The transformation temporary file name
        /// </summary>
        private const string TransformationTempFileName = "ConfigTransformationTempFile.tmp" + ConfigExtension;

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
        public static bool IsProjectSupported(string projectExtension)
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
        public static bool CheckTransformationFileAndGetSourceFileFromIt(string transformationFileName, string transformationFilePattern,
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
        public static bool IsTransformationFile(string fileName, string transformationFilePattern, int tranformationFileSourceMatchIndex)
        {
            string sourceFileName;
            return CheckTransformationFileAndGetSourceFileFromIt(fileName, transformationFilePattern,
                tranformationFileSourceMatchIndex, out sourceFileName);
        }

        /// <summary>
        /// Gets the backup file name of file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>Name of the backup source file</returns>
        private static string GetBackupFileNameOfFile(string fileName)
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            return GetTemporaryFileFullName(fileName + BackupExtension);
        }

        /// <summary>
        /// Creates the backup file of file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="backupFileName">Name of the backup file.</param>
        private static void CreateBackupFileOfFile(string fileName, string backupFileName)
        {
            File.Copy(fileName, backupFileName, true);

            //var attributes = File.GetAttributes(backupFileName);

            //if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
            //{
            //    // Make the file RW
            //    attributes = RemoveAttribute(attributes, FileAttributes.ReadOnly);
            //    File.SetAttributes(backupFileName, attributes);

            //} 

        }


        ///// <summary>
        ///// Removes the attribute.
        ///// </summary>
        ///// <param name="attributes">The attributes.</param>
        ///// <param name="attributesToRemove">The attributes to remove.</param>
        ///// <returns>Attributes of the file</returns>
        //private static FileAttributes RemoveAttribute(FileAttributes attributes, FileAttributes attributesToRemove)
        //{
        //    return attributes & ~attributesToRemove;
        //}

        /// <summary>
        /// Determines whether [is file read only] [the specified file name].
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>[true] if the file is readonly, otherwise [false]</returns>
        private static bool IsFileReadOnly(string fileName)
        {
            var info = new FileInfo(fileName);
            return info.IsReadOnly;
        }

        /// <summary>
        /// Deletes the backup file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        private static void DeleteFile(string fileName)
        {
            File.Delete(fileName);
        }

        /// <summary>
        /// Transforms the specified transofrmation file name.
        /// </summary>
        /// <param name="transformationFileName">Name of the transofrmation file.</param>
        /// <param name="transformationFilePattern">The transformation file pattern.</param>
        /// <param name="transformationFileSourceMatchIndex">Index of the tranformation file source match.</param>
        public static void Transform(string transformationFileName, string transformationFilePattern, int transformationFileSourceMatchIndex)
        {
            string sourceFileName;

            var result = CheckTransformationFileAndGetSourceFileFromIt(transformationFileName, transformationFilePattern,
                transformationFileSourceMatchIndex, out sourceFileName);

            if (!result)
            {
                Logger.LogInfo(Resources.InvalidTransformationFileName);
                return;
            }

            if (IsFileReadOnly(sourceFileName))
            {
                Logger.LogInfo(string.Format(Resources.FileIsReadOnly, sourceFileName));
                MessageBox.Show(string.Format(Resources.FileIsReadOnlyChange, sourceFileName), Resources.ApplicationCaption, MessageBoxButton.OK, MessageBoxImage.Asterisk);
                return;
            }

            var backupFileName = GetBackupFileNameOfFile(sourceFileName);

            try
            {
                DeleteFile(backupFileName);
                Logger.LogInfo(string.Format(Resources.DeletionOfTemporaryFileDone, backupFileName));

                CreateBackupFileOfFile(sourceFileName, backupFileName);
                Logger.LogInfo(string.Format(Resources.CopyOfSourceFileDone, sourceFileName, backupFileName));

                result = TransformationManager.Transform(backupFileName, transformationFileName, sourceFileName, new TransformationLogger());
                Logger.LogInfo(result
                    ? string.Format(Resources.TransformationOfFileSuccessfullyDone, backupFileName,
                        transformationFileName, sourceFileName)
                    : string.Format(Resources.TransformationProcessError, backupFileName, transformationFileName,
                        sourceFileName));

                FormatFile(sourceFileName);
                DeleteFile(backupFileName);
                Logger.LogInfo(string.Format(Resources.DeletionOfTemporaryFileDone, backupFileName));
            }
            catch (Exception ex)
            {
                Logger.LogError(string.Format(Resources.ErrorWhileTransformingFile, backupFileName, transformationFileName, sourceFileName));
                Logger.LogError(ex);
            }

        }

        /// <summary>
        /// Transforms to temporary file.
        /// </summary>
        /// <param name="transformationFileName">Name of the transofrmation file.</param>
        /// <param name="transformationFilePattern">The transformation file pattern.</param>
        /// <param name="transformationFileSourceMatchIndex">Index of the tranformation file source match.</param>
        /// <returns>Name of the source and transformed file if there was no error, otherwise null</returns>
        public static Tuple<string, string> TransformToTemporaryFile(string transformationFileName, string transformationFilePattern, int transformationFileSourceMatchIndex)
        {
            string sourceFileName;

            var result = CheckTransformationFileAndGetSourceFileFromIt(transformationFileName, transformationFilePattern,
                transformationFileSourceMatchIndex, out sourceFileName);

            if (!result)
            {
                Logger.LogInfo(Resources.InvalidTransformationFileName);
                return null;
            }

            var tempTargetFileName = GetTemporaryFileFullName(TransformationTempFileName);

            try
            {
                DeleteFile(tempTargetFileName);
                Logger.LogInfo(string.Format(Resources.DeletionOfTemporaryFileDone, tempTargetFileName));


                result = TransformationManager.Transform(sourceFileName, transformationFileName, tempTargetFileName, new TransformationLogger());

                if (result)
                {
                    Logger.LogInfo(string.Format(Resources.TransformationOfFileSuccessfullyDone, sourceFileName,
                        transformationFileName, tempTargetFileName));

                    FormatFile(tempTargetFileName);

                    return new Tuple<string, string>(sourceFileName, tempTargetFileName);
                }

                //else error
                Logger.LogInfo(string.Format(Resources.TransformationProcessError, sourceFileName, transformationFileName,
                    tempTargetFileName));

                return null;

            }
            catch (Exception ex)
            {
                Logger.LogError(string.Format(Resources.ErrorWhileTransformingFile, sourceFileName, transformationFileName, tempTargetFileName));
                Logger.LogError(ex);
            }

            return null;
        }

        /// <summary>
        /// Formats the file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        private static void FormatFile(string fileName)
        {
            // because now there is only one indent property, run it only if it is true...
            var format = Options.Options.WriteAttributesOnASeparateLine;
            if (format)
            {
                Logger.LogInfo(string.Format(Resources.FormattingFile, fileName));

                var settings = new XmlWriterSettings
                {
                    Indent = true,
                    //IndentChars = "\t",
                    NewLineOnAttributes = Options.Options.WriteAttributesOnASeparateLine,
                    ConformanceLevel = ConformanceLevel.Auto
                };
                
                var document = new XmlDocument();
                document.Load(fileName);

                using (var writer = XmlWriter.Create(fileName, settings))
                {
                    document.WriteContentTo(writer);
                }
            }
        }

        /// <summary>
        /// Gets the full name of the temporary file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>Full name of the temporary file</returns>
        private static string GetTemporaryFileFullName(string fileName)
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            return Path.Combine(Path.GetTempPath(), Path.GetFileName(fileName));
        }

        #endregion Methods


    }
}
