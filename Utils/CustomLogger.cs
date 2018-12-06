using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Extensions.Logging;

namespace MerryClosets.Utils
{
    public class CustomLogger
    {
        private readonly ILogger _logger;
        private readonly string _fileDirectory;
        private readonly string _filePath;
        private readonly List<string> _logLevels;

        public CustomLogger(ILogger logger)
        {
            _logger = logger;
            _logLevels = new List<string> { "TRACE", "DEBUG", "INFORMATION", "WARNING", "ERROR", "CRITICAL", "NONE" };
            _fileDirectory = Path.GetFullPath(@"LoggerFiles");
            _filePath = Path.GetFullPath(@"LoggerFiles/log.txt");
        }

        private void writeLogToFile(EventId logId, string message, LogLevel logLevel, params object[] args)
        {
            string timestamp = DateUtils.GetCurrentDateTimestamp();
            string logMessageFormated = String.Format(message, args);

            int level = (int)logLevel;

            string formated = String.Format("[{0} {1}]:     EVENTID: {2}    MESSAGE: {3}\n", timestamp, _logLevels[level], logId.ToString(), logMessageFormated);

            DirectoryInfo di = Directory.CreateDirectory(_fileDirectory);

            using (System.IO.FileStream file = new FileStream(_filePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
            using(System.IO.StreamWriter sw = new StreamWriter(file)){
                sw.WriteAsync(formated);
            }
        }

        public void logInformation(EventId logId, string message, params object[] args)
        {
            _logger.LogInformation(logId, message, args); //write logger to the console
            writeLogToFile(logId, message, LogLevel.Information, args);
        }

        public void logDebug(EventId logId, string message, params object[] args)
        {
            _logger.LogDebug(logId, message, args); //write logger to the console
            writeLogToFile(logId, message, LogLevel.Debug, args);
        }

        public void logError(EventId logId, string message, params object[] args)
        {
            _logger.LogError(logId, message, args); //write logger to the console
            writeLogToFile(logId, message, LogLevel.Error, args);
        }

        public void logTrace(EventId logId, string message, params object[] args)
        {
            _logger.LogTrace(logId, message, args); //write logger to the console
            writeLogToFile(logId, message, LogLevel.Trace, args);
        }

        public void logWarning(EventId logId, string message, params object[] args)
        {
            _logger.LogWarning(logId, message, args); //write logger to the console
            writeLogToFile(logId, message, LogLevel.Warning, args);
        }

        public void logCritical(EventId logId, string message, params object[] args)
        {
            _logger.LogCritical(logId, message, args); //write logger to the console
            writeLogToFile(logId, message, LogLevel.Critical, args);
        }
    }
}