using System;
using System.Diagnostics;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace Marazt.ConfigTransformation.Logging
{
    /// <summary>
    /// SimpleDebug logger
    /// </summary>
    public static class Logger
    {
        #region Constants

        /// <summary>
        /// The information message
        /// </summary>
        private const string InfoMessage = "[Config Transformation][Info]: {0}";

        /// <summary>
        /// The error message
        /// </summary>
        private const string ErrorMessage = "[Config Transformation][Error]: {0}";

        /// <summary>
        /// The warning message
        /// </summary>
        private const string WarningMessage = "[Config Transformation][Warning]: {0}";

        #endregion Constants


        #region Methods

        /// <summary>
        /// Logs the information.
        /// </summary>
        /// <param name="message">The message.</param>
        public static void LogInfo(string message)
        {
            Log(string.Format(InfoMessage, message));
        }

        /// <summary>
        /// Logs the warning.
        /// </summary>
        /// <param name="message">The message.</param>
        public static void LogWarning(string message)
        {
            Log(string.Format(WarningMessage, message));
        }


        /// <summary>
        /// Logs the error.
        /// </summary>
        /// <param name="message">The message.</param>
        public static void LogError(string message)
        {
            Log(string.Format(ErrorMessage, message));
        }


        /// <summary>
        /// Logs the error.
        /// </summary>
        /// <param name="ex">The ex.</param>
        public static void LogError(Exception ex)
        {
            Log(string.Format(ErrorMessage, ex.Message));
        }

        /// <summary>
        /// Logs the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        private static void Log(string message)
        {
            Debug.WriteLine(message);
            WriteToVsGeneralOutput(message);
        }

        /// <summary>
        /// Writes to vs general output.
        /// </summary>
        /// <param name="message">The message.</param>
        private static void WriteToVsGeneralOutput(string message)
        {
            var outputWindow = Package.GetGlobalService(typeof(SVsOutputWindow)) as IVsOutputWindow;
            if (outputWindow == null)
            {
                return;
            }

            var generalGuidPane = VSConstants.OutputWindowPaneGuid.GeneralPane_guid;
            IVsOutputWindowPane generalPane;

            var hr = outputWindow.GetPane(generalGuidPane, out generalPane);

            if (hr != VSConstants.S_OK || generalPane == null)
            {
                generalPane = CreateGeneralOutputWindowPane(outputWindow, generalGuidPane);
            }

            generalPane.Activate();
            generalPane.OutputString(message + Environment.NewLine);
            generalPane.FlushToTaskList();

        }

        /// <summary>
        /// Creates the general output window pane.
        /// http://social.msdn.microsoft.com/Forums/vstudio/en-US/69ebd53e-96ca-4c74-b933-e1a12136ea57/vspackage-writing-to-general-output-window-pane-vs-2010?forum=vsx
        /// </summary>
        /// <param name="outputWindow">The output window.</param>
        /// <param name="generalGuidPane">The general unique identifier pane.</param>
        /// <returns>Instance of the IVsOutputWindowPane or null</returns>
        private static IVsOutputWindowPane CreateGeneralOutputWindowPane(IVsOutputWindow outputWindow, Guid generalGuidPane)
        {
            var hr = outputWindow.CreatePane(generalGuidPane, "General", 1, 0);

            if (hr != VSConstants.S_OK)
            {
                return null;
            }

            IVsOutputWindowPane generalPane;
            hr = outputWindow.GetPane(generalGuidPane, out generalPane);

            return hr != VSConstants.S_OK ? null : generalPane;
        }

        #endregion Methods

    }
}
