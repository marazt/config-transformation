using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Marazt.Commons.Xml;


namespace Marazt.ConfigTransformation
{
    public class TransformationProvider
    {
        #region Constants

        public const string SourceConfigFilePattern = ".+\\.config";
        public const string TranformationFilePattern = "(.+)\\.(.+)\\.config";
        public const string ConfigExtension = ".config";

        List<string> SupportedProjectExtensions = new List<string>
        {
            ".csproj",
            ".vbproj",
            ".fsproj"
        };

        #endregion Constants

        #region Methods


        public bool IsProjectSupported(string projectExtension)
        {
            return this.SupportedProjectExtensions.Contains(Path.GetExtension(projectExtension).ToLower());
        }

        public bool CheckAndGetSourceFileFromTransformationFile(string transformationFileName, out string sourceFileName)
        {
            var match = Regex.Match(transformationFileName, TranformationFilePattern, RegexOptions.IgnoreCase);
            if (match.Success)
            {
                sourceFileName = match.Groups[1].Value + ConfigExtension;
                return true;
            }

            sourceFileName = null;
            return false;
        }

        public bool IsTransformationFile(string fileName)
        {
            string sourceFileName;
            return this.CheckAndGetSourceFileFromTransformationFile(fileName, out sourceFileName);
        }

        public string GetBackupFileNameOfFile(string fileName)
        {
            return Path.Combine(Path.GetDirectoryName(fileName), Path.GetFileName(fileName) + ".bak");
        }

        public bool CreateBackupFileOfFile(string fileName, string  backupFileName)
        {
            try
            {
                File.Copy(fileName, backupFileName, true);
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                backupFileName = null;
                return false;
            }
        }

        public void Transform(string transofrmationFileName)
        {
            string sourceFileName;

            var result = this.CheckAndGetSourceFileFromTransformationFile(transofrmationFileName, out sourceFileName);

            if (!result)
            {
                Logger.LogInfo("Invalid tranformation file name");
                return;
            }


            var backupFileName = this.GetBackupFileNameOfFile(sourceFileName);

            if (!File.Exists(backupFileName))
            {
                if (!this.CreateBackupFileOfFile(sourceFileName, backupFileName))
                {
                    return;
                }
            }

            try
            {
                TransformationManager.Transform(backupFileName, transofrmationFileName, sourceFileName);
                Logger.LogInfo(string.Format("Transformation of file '{0}' done", transofrmationFileName));
            }
            catch (Exception ex)
            {
                Logger.LogError(string.Format("Error while trasforming file '{0}' done", transofrmationFileName));
                Logger.LogError(ex);
            }
        }



        #endregion Methods
    }
}
