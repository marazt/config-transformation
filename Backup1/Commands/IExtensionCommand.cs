using System;

namespace Marazt.ConfigTransformation.Commands
{
    /// <summary>
    /// Interface representing encapsulation of the command to be registered in the extension
    /// </summary>
    public interface IExtensionCommand
    {
        #region Properties


        /// <summary>
        /// Gets the command set unique identifier.
        /// </summary>
        /// <value>
        /// The command set unique identifier.
        /// </value>
        Guid CmdSetGuid { get; }

        /// <summary>
        /// Gets the command identifier.
        /// </summary>
        /// <value>
        /// The command identifier.
        /// </value>
        int CmdID { get; }

        #endregion Properties


        #region Events

        /// <summary>
        /// Befores the query status.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        void BeforeQueryStatus(object sender, EventArgs e);

        /// <summary>
        /// Commands the callback.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        void CommandCallback(object sender, EventArgs e);

        #endregion Events

    }
}
