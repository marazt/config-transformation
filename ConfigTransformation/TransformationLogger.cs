using System;
using Microsoft.Web.XmlTransform;

namespace Marazt.ConfigTransformation
{
    /// <summary>
    /// Transformation logger
    /// </summary>
    public class TransformationLogger : IXmlTransformationLogger
    {
        /// <summary>
        /// Logs the message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="messageArgs">The message arguments.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public void LogMessage(string message, params object[] messageArgs)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Logs the message.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="message">The message.</param>
        /// <param name="messageArgs">The message arguments.</param>
        public void LogMessage(MessageType type, string message, params object[] messageArgs)
        {
            Logger.LogInfo(string.Format(message, messageArgs));
        }

        /// <summary>
        /// Logs the warning.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="messageArgs">The message arguments.</param>
        public void LogWarning(string message, params object[] messageArgs)
        {
            Logger.LogWarning(string.Format(message, messageArgs));
        }

        /// <summary>
        /// Logs the warning.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="message">The message.</param>
        /// <param name="messageArgs">The message arguments.</param>
        public void LogWarning(string file, string message, params object[] messageArgs)
        {
            Logger.LogWarning(string.Format(message, messageArgs));
        }

        /// <summary>
        /// Logs the warning.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="lineNumber">The line number.</param>
        /// <param name="linePosition">The line position.</param>
        /// <param name="message">The message.</param>
        /// <param name="messageArgs">The message arguments.</param>
        public void LogWarning(string file, int lineNumber, int linePosition, string message, params object[] messageArgs)
        {
            Logger.LogWarning(string.Format(message, messageArgs));
        }

        /// <summary>
        /// Logs the error.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="messageArgs">The message arguments.</param>
        public void LogError(string message, params object[] messageArgs)
        {
            Logger.LogError(string.Format(message, messageArgs));
        }

        /// <summary>
        /// Logs the error.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="message">The message.</param>
        /// <param name="messageArgs">The message arguments.</param>
        public void LogError(string file, string message, params object[] messageArgs)
        {
            Logger.LogError(string.Format(message, messageArgs));
        }

        /// <summary>
        /// Logs the error.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="lineNumber">The line number.</param>
        /// <param name="linePosition">The line position.</param>
        /// <param name="message">The message.</param>
        /// <param name="messageArgs">The message arguments.</param>
        public void LogError(string file, int lineNumber, int linePosition, string message, params object[] messageArgs)
        {
            Logger.LogError(string.Format(message, messageArgs));
        }

        /// <summary>
        /// Logs the error from exception.
        /// </summary>
        /// <param name="ex">The ex.</param>
        public void LogErrorFromException(Exception ex)
        {
            Logger.LogError(ex);
        }

        /// <summary>
        /// Logs the error from exception.
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <param name="file">The file.</param>
        public void LogErrorFromException(Exception ex, string file)
        {
            Logger.LogError(ex);
        }

        /// <summary>
        /// Logs the error from exception.
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <param name="file">The file.</param>
        /// <param name="lineNumber">The line number.</param>
        /// <param name="linePosition">The line position.</param>
        public void LogErrorFromException(Exception ex, string file, int lineNumber, int linePosition)
        {
            Logger.LogError(ex);
        }

        /// <summary>
        /// Starts the section.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="messageArgs">The message arguments.</param>
        public void StartSection(string message, params object[] messageArgs)
        {
            Logger.LogInfo(string.Format(message, messageArgs));
        }

        /// <summary>
        /// Starts the section.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="message">The message.</param>
        /// <param name="messageArgs">The message arguments.</param>
        public void StartSection(MessageType type, string message, params object[] messageArgs)
        {
            Logger.LogInfo(string.Format(message, messageArgs));
        }

        /// <summary>
        /// Ends the section.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="messageArgs">The message arguments.</param>
        public void EndSection(string message, params object[] messageArgs)
        {
            Logger.LogInfo(string.Format(message, messageArgs));
        }

        /// <summary>
        /// Ends the section.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="message">The message.</param>
        /// <param name="messageArgs">The message arguments.</param>
        public void EndSection(MessageType type, string message, params object[] messageArgs)
        {
            Logger.LogInfo(string.Format(message, messageArgs));
        }
    }
}
