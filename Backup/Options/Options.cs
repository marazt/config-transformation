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
        public static string TransfomationFileNameRegexp => DTEHelper.GetPropertyValue<string>(AppConstants.ConfigTransformation,
            AppConstants.General,
            OptionsPage.TransfomationFileNameRegexpPropertyName);

        /// <summary>
        /// Gets the index of the source file regexp match.
        /// </summary>
        /// <value>
        /// The index of the source file regexp match.
        /// </value>
        public static int SourceFileRegexpMatchIndex => DTEHelper.GetPropertyValue<int>(AppConstants.ConfigTransformation, AppConstants.General,
            OptionsPage.SourceFileRegexpMatchIndexPropertyName);


        /// <summary>
        /// Gets a value indicating whether [nest transformation files].
        /// </summary>
        /// <value>
        /// <c>true</c> if [nest transformation files]; otherwise, <c>false</c>.
        /// </value>
        public static bool NestTransformationFiles => DTEHelper.GetPropertyValue<bool>(AppConstants.ConfigTransformation, AppConstants.General,
            OptionsPage.NestTransformationFilesPropertyName);

        /// <summary>
        /// Gets a value indicating whether [write attributes on a separate line].
        /// </summary>
        /// <value>
        /// <c>true</c> if [write attributes on a separate line]; otherwise, <c>false</c>.
        /// </value>
        public static bool WriteAttributesOnASeparateLine => DTEHelper.GetPropertyValue<bool>(AppConstants.ConfigTransformation, AppConstants.General, OptionsPage.WriteAttributesOnASeparateLinePropertyName);

        #endregion Properties
    }
}
