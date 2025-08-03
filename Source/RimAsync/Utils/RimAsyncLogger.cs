using System;
using Verse;

namespace RimAsync.Utils
{
    /// <summary>
    /// Centralized logging system for RimAsync with structured log levels
    /// Provides Debug, Info, Warning, and Error logging with configurable levels
    /// </summary>
    public static class RimAsyncLogger
    {
        /// <summary>
        /// Available log levels for filtering
        /// </summary>
        public enum LogLevel
        {
            Debug = 0,
            Info = 1,
            Warning = 2,
            Error = 3
        }

        /// <summary>
        /// Current minimum log level - messages below this level will be filtered out
        /// </summary>
        public static LogLevel MinimumLevel { get; set; } = LogLevel.Info;

        /// <summary>
        /// Enable debug logging (overrides MinimumLevel for debug messages)
        /// </summary>
        public static bool EnableDebugLogging { get; set; } = false;

        /// <summary>
        /// Prefix for all RimAsync log messages
        /// </summary>
        private const string LOG_PREFIX = "[RimAsync]";

        /// <summary>
        /// Log a debug message (only shown when debug logging is enabled)
        /// </summary>
        /// <param name="message">Debug message</param>
        /// <param name="context">Optional context for the message</param>
        public static void Debug(string message, string context = null)
        {
            if (!EnableDebugLogging && MinimumLevel > LogLevel.Debug)
                return;

            var formattedMessage = FormatMessage("DEBUG", message, context);
            Log.Message(formattedMessage);
        }

        /// <summary>
        /// Log an informational message
        /// </summary>
        /// <param name="message">Information message</param>
        /// <param name="context">Optional context for the message</param>
        public static void Info(string message, string context = null)
        {
            if (MinimumLevel > LogLevel.Info)
                return;

            var formattedMessage = FormatMessage("INFO", message, context);
            Log.Message(formattedMessage);
        }

        /// <summary>
        /// Log a warning message
        /// </summary>
        /// <param name="message">Warning message</param>
        /// <param name="context">Optional context for the message</param>
        public static void Warning(string message, string context = null)
        {
            if (MinimumLevel > LogLevel.Warning)
                return;

            var formattedMessage = FormatMessage("WARN", message, context);
            Log.Warning(formattedMessage);
        }

        /// <summary>
        /// Log an error message
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="context">Optional context for the message</param>
        public static void Error(string message, string context = null)
        {
            var formattedMessage = FormatMessage("ERROR", message, context);
            Log.Error(formattedMessage);
        }

        /// <summary>
        /// Log an error with exception details
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="exception">Exception to log</param>
        /// <param name="context">Optional context for the message</param>
        public static void Error(string message, Exception exception, string context = null)
        {
            var formattedMessage = FormatMessage("ERROR", $"{message}: {exception}", context);
            Log.Error(formattedMessage);
        }

        /// <summary>
        /// Log performance-related information
        /// </summary>
        /// <param name="operation">Operation name</param>
        /// <param name="duration">Duration in milliseconds</param>
        /// <param name="additionalInfo">Additional performance information</param>
        public static void Performance(string operation, long duration, string additionalInfo = null)
        {
            var perfMessage = $"PERF: {operation} took {duration}ms";
            if (!string.IsNullOrEmpty(additionalInfo))
                perfMessage += $" | {additionalInfo}";

            Debug(perfMessage, "Performance");
        }

        /// <summary>
        /// Log initialization steps with consistent formatting
        /// </summary>
        /// <param name="component">Component being initialized</param>
        /// <param name="step">Initialization step</param>
        /// <param name="success">Whether the step was successful</param>
        public static void InitStep(string component, string step, bool success = true)
        {
            var level = success ? "INFO" : "ERROR";
            var status = success ? "✅" : "❌";
            var message = $"INIT: {component} - {step} {status}";

            if (success)
                Info(message, "Initialization");
            else
                Error(message, "Initialization");
        }

        /// <summary>
        /// Log multiplayer-related information with special formatting
        /// </summary>
        /// <param name="message">Multiplayer message</param>
        /// <param name="isAsyncTime">Whether AsyncTime is involved</param>
        public static void Multiplayer(string message, bool isAsyncTime = false)
        {
            var mpMessage = isAsyncTime ? $"MP+AsyncTime: {message}" : $"MP: {message}";
            Info(mpMessage, "Multiplayer");
        }

        /// <summary>
        /// Log async operation information
        /// </summary>
        /// <param name="operation">Operation name</param>
        /// <param name="status">Operation status (Started, Completed, Failed, etc.)</param>
        /// <param name="details">Additional details</param>
        public static void AsyncOp(string operation, string status, string details = null)
        {
            var asyncMessage = $"ASYNC: {operation} - {status}";
            if (!string.IsNullOrEmpty(details))
                asyncMessage += $" | {details}";

            Info(asyncMessage, "AsyncOperations");
        }

        /// <summary>
        /// Format log message with consistent structure
        /// </summary>
        /// <param name="level">Log level string</param>
        /// <param name="message">Message content</param>
        /// <param name="context">Optional context</param>
        /// <returns>Formatted log message</returns>
        private static string FormatMessage(string level, string message, string context)
        {
            var timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
            var contextPart = !string.IsNullOrEmpty(context) ? $"[{context}] " : "";

            return $"{LOG_PREFIX} {timestamp} {level}: {contextPart}{message}";
        }

        /// <summary>
        /// Configure logging based on settings
        /// </summary>
        /// <param name="enableDebug">Enable debug logging</param>
        /// <param name="minimumLevel">Minimum log level</param>
        public static void Configure(bool enableDebug, LogLevel minimumLevel = LogLevel.Info)
        {
            EnableDebugLogging = enableDebug;
            MinimumLevel = minimumLevel;

            Info($"Logging configured - Debug: {enableDebug}, MinLevel: {minimumLevel}", "Logger");
        }

        /// <summary>
        /// Get current logging configuration as string
        /// </summary>
        /// <returns>Current logging configuration</returns>
        public static string GetConfiguration()
        {
            return $"Debug: {EnableDebugLogging}, MinLevel: {MinimumLevel}";
        }
    }
}
