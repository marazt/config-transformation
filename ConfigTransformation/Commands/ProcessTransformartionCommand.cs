using System;
using Marazt.ConfigTransformation.Helpers;
using Marazt.ConfigTransformation.Transformation;

namespace Marazt.ConfigTransformation.Commands
{
    /// <summary>
    /// Transformation process command
    /// </summary>
    class ProcessTransformartionCommand : AContextMenuCommand
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
            get { return (int)PkgCmdIDList.cmdidCtxMenuTransformItem; }
        }

        #endregion Properties

        #region Events

        /// <summary>
        /// Commands the callback.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        public override void CommandCallback(object sender, EventArgs e)
        {
            string fileName;
            var result = SolutionHelper.IsCorrectItemForTransformationOperationsSelected(out fileName);
            if (!result)
            {
                return;
            }

            TransformationProvider.Transform(fileName,
              Options.Options.TransfomationFileNameRegexp, Options.Options.SourceFileRegexpMatchIndex);
        }

        #endregion Events

    }
}
