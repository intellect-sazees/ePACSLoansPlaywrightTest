using ePACSLoans.Core.Interfaces;
using NLog;
using ILogger = NLog.ILogger;

namespace ePACSLoans.Utilities.Common
{
    /// <summary>
    /// NLog-based logger implementation
    /// </summary>
    public class Logger(string name = "Framework")
    {
        private readonly NLog.ILogger _logger = LogManager.GetLogger(name);

        public void Debug(string message)
        {
            _logger.Debug(message);
        }

        public void Debug(string message, Exception exception)
        {
            _logger.Debug(exception, message);
        }

        public void Info(string message)
        {
            _logger.Info(message);
        }

        public void Info(string message, Exception exception)
        {
            _logger.Info(exception, message);
        }

        public void Warning(string message)
        {
            _logger.Warn(message);
        }

        public void Warning(string message, Exception exception)
        {
            _logger.Warn(exception, message);
        }

        public void Error(string message)
        {
            _logger.Error(message);
        }

        public void Error(string message, Exception exception)
        {
            _logger.Error(exception, message);
        }

        public void Fatal(string message)
        {
            _logger.Fatal(message);
        }

        public void Fatal(string message, Exception exception)
        {
            _logger.Fatal(exception, message);
        }
    }
}


