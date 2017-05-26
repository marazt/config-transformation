using System;
using Marazt.ConfigTransformation.Helpers;
using Marazt.ConfigTransformation.Transformation;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace Marazt.ConfigTransformation.Commands
{
    /// <summary>
    /// Base Constect menu command class
    /// </summary>
    internal abstract class AContextMenuCommand : IExtensionCommand
    {
        #region Properties

        /// <summary>
        /// Gets or sets the command set unique identifier.
        /// </summary>
        /// <value>
        /// The command set unique identifier.
        /// </value>
        public abstract Guid CmdSetGuid { get; }

        /// <summary>
        /// Gets or sets the command identifier.
        /// </summary>
        /// <value>
        /// The command identifier.
        /// </value>
        public abstract int CmdID { get; }

        #endregion Properties

        #region Events

        /// <summary>
        /// Befores the query status.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public void BeforeQueryStatus(object sender, EventArgs e)
        {

            // get the menu that fired the event
            var menuCommand = sender as OleMenuCommand;
            if (menuCommand == null)
            {
                return;
            }

            // start by assuming that the menu will not be shown
            menuCommand.Visible = false;
            menuCommand.Enabled = false;

            // ReSharper disable once RedundantAssignment
            IVsHierarchy hierarchy = null;
            // ReSharper disable once RedundantAssignment
            var itemID = VSConstants.VSITEMID_NIL;

            if (!SolutionHelper.IsSingleProjectItemSelection(out hierarchy, out itemID))
            {
                return;
            }


            var vsProject = (IVsProject) hierarchy;
            if (!SolutionHelper.ProjectSupportsTransforms(vsProject))
            {
                return;
            }

            var fileName = SolutionHelper.GetFileNameFromItem(vsProject, itemID);
            if (fileName == null || !IsTransformationFile(fileName))
            {
                return;
            }

            menuCommand.Visible = true;
            menuCommand.Enabled = true;
        }

        /// <summary>
        /// Commands the callback.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        public abstract void CommandCallback(object sender, EventArgs e);

        #endregion Events

        #region Methods


        /// <summary>
        /// Determines whether [is transformation file] [the specified file name].
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>[True] if file is transformation file, otherwise [False]</returns>
        private bool IsTransformationFile(string fileName)
        {
            return TransformationProvider.IsTransformationFile(fileName,
                Options.Options.TransfomationFileNameRegexp,
                Options.Options.SourceFileRegexpMatchIndex);
        }

        #endregion Methods

    }
}
