using System;
using Marazt.ConfigTransformation.Helpers;
using Marazt.ConfigTransformation.Transformation;
using Microsoft.VisualStudio.Shell.Interop;

namespace Marazt.ConfigTransformation.Commands
{
    /// <summary>
    /// Compare context menu command
    /// </summary>
    internal class CompareFilesCommand : AContextMenuCommand
    {
        #region Properties

        /// <summary>
        /// Gets or sets the command set unique identifier.
        /// </summary>
        /// <value>
        /// The command set unique identifier.
        /// </value>
        public override Guid CmdSetGuid
        {
            get { return GuidList.guidConfigTransformationCmdSet; }
        }

        /// <summary>
        /// Gets or sets the command identifier.
        /// </summary>
        /// <value>
        /// The command identifier.
        /// </value>
        public override int CmdID
        {
            get { return (int)PkgCmdIDList.cmdidCtxMenuComparisonItem; }
        }


        /// <summary>
        /// The difference service
        /// </summary>
        private readonly IVsDifferenceService diffService;

        #endregion Properties


        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CompareFilesCommand"/> class.
        /// </summary>
        public CompareFilesCommand()
        {
            this.diffService = DteHelper.GetServiceIstanceOfInterface<SVsDifferenceService, IVsDifferenceService>();
        }

        #endregion Constructors


        #region Events


        /// <summary>
        /// Commands the callback.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public override void CommandCallback(object sender, EventArgs e)
        {
            string fileName;
            var result = SolutionHelper.IsCorrectItemForTransformationOperationsSelected(out fileName);
            if (!result)
            {
                return;
            }


            var files = TransformationProvider.TransformToTemporaryFile(fileName,
                DteHelper.GetPropertyValue<string>(AppConstants.ConfigTransformation, AppConstants.General, OptionsPage.TransfomationFileNameRegexpPropertyName),
                   DteHelper.GetPropertyValue<int>(AppConstants.ConfigTransformation, AppConstants.General, OptionsPage.SourceFileRegexpMatchIndexPropertyName));

            if (files == null)
            {
                return;
            }

            this.CompareFiles(files.Item1, files.Item2);
        }

        #endregion Events

        #region Methods


        /// <summary>
        /// Compares the files.
        /// </summary>
        /// <param name="leftFileName">Name of the left file.</param>
        /// <param name="rightFileName">Name of the right file.</param>
        private void CompareFiles(string leftFileName, string rightFileName)
        {
            if (this.diffService == null)
            {
                return;
            }

            diffService.OpenComparisonWindow(leftFileName, rightFileName);
        }

        #endregion Methods


    }
}
