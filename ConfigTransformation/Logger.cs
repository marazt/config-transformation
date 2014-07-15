using System;
using System.Diagnostics;

namespace Marazt.ConfigTransformation
{
    /// <summary>
    /// SimpleDebug logger
    /// </summary>
    public class Logger
    {
        #region Constants

        private const string InfoMessage = "[ConfigTransformation][Info]: {0}";
        private const string ErrorMessage = "[ConfigTransformation][Error]: {0}";

        #endregion Constants


        #region Methods

        /// <summary>
        /// Logs the information.
        /// </summary>
        /// <param name="msg">The MSG.</param>
        public static void LogInfo(string msg)
        {
            Debug.WriteLine(InfoMessage, msg);
        }

        /// <summary>
        /// Logerrors the specified MSG.
        /// </summary>
        /// <param name="msg">The MSG.</param>
        public static void LogError(string msg)
        {
            Debug.WriteLine(ErrorMessage, msg);
        }


        /// <summary>
        /// Logs the error.
        /// </summary>
        /// <param name="ex">The ex.</param>
        public static void LogError(Exception ex)
        {
            Debug.WriteLine(ErrorMessage, ex.Message);
        }

        #endregion Methods

    }
}
