using Marazt.ConfigTransformation.Helpers;

namespace Marazt.ConfigTransformation.Options
{
    /// <summary>
    /// Class with options properties
    /// </summary>
    internal static class Options
    {
        #region Properties

        /// <summary>
        /// Gets the transfomation file name regexp.
        /// </summary>
        /// <value>
        /// The transfomation file name regexp.
        /// </value>
        public static string TransfomationFileNameRegexp
        {
            get
            {
                return DTEHelper.GetPropertyValue<string>(AppConstants.ConfigTransformation,
                    AppConstants.General,
                    OptionsPage.TransfomationFileNameRegexpPropertyName);
            }
        }

        /// <summary>
        /// Gets the index of the source file regexp match.
        /// </summary>
        /// <value>
        /// The index of the source file regexp match.
        /// </value>
        public static int SourceFileRegexpMatchIndex
        {
            get
            {
                return DTEHelper.GetPropertyValue<int>(AppConstants.ConfigTransformation, AppConstants.General,
                    OptionsPage.SourceFileRegexpMatchIndexPropertyName);
            }
        }


        /// <summary>
        /// Gets a value indicating whether [nest transformation files].
        /// </summary>
        /// <value>
        /// <c>true</c> if [nest transformation files]; otherwise, <c>false</c>.
        /// </value>
        public static bool NestTransformationFiles
        {
            get
            {
                return DTEHelper.GetPropertyValue<bool>(AppConstants.ConfigTransformation, AppConstants.General,
                   OptionsPage.NestTransformationFilesPropertyName);
            }
        }

        #endregion Properties
    }
}
