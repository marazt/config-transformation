using System;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Shell;

namespace Marazt.ConfigTransformation.Commands
{
    /// <summary>
    /// Helper class for command registration
    /// </summary>
    internal static class CommandHelper
    {

        #region Properties

        /// <summary>
        /// The commands to be registered
        /// </summary>
        private static readonly Type[] CommandsToBeRegistered =
        {
            typeof(ProcessTransformartionCommand),
            typeof(CompareFilesCommand)
        };

        #endregion Properties

        #region Methods


        /// <summary>
        /// Registers the menu command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="menuService">The menu service.</param>
        private static void RegisterMenuCommand(IExtensionCommand command, OleMenuCommandService menuService)
        {
            var comparisonMenuCommandID = new CommandID(command.CmdSetGuid, command.CmdID);
            var comparisonMenuItem = new OleMenuCommand(command.CommandCallback, comparisonMenuCommandID);
            comparisonMenuItem.BeforeQueryStatus += command.BeforeQueryStatus;
            menuService.AddCommand(comparisonMenuItem);
        }


        /// <summary>
        /// Registers the commands.
        /// </summary>
        /// <param name="menuService">The menu service.</param>
        public static void RegisterCommands(OleMenuCommandService menuService)
        {
            foreach (var cmdType in CommandsToBeRegistered)
            {
                RegisterMenuCommand((IExtensionCommand)Activator.CreateInstance(cmdType), menuService);
            }
        }

        #endregion Methods
    }
}
