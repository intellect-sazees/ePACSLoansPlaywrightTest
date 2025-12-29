using ePACSLoans.Core;
using ePACSLoans.Core.Interfaces;
using System.Diagnostics;

namespace ePACSLoans.Utilities.Common
{
    /// <summary>
    /// Retry helper with exponential backoff for transient failures
    /// </summary>
    public class RetryHelper : IRetryHelper
    {
        private readonly NLog.ILogger _logger;
        private readonly int _defaultMaxAttempts;
        private readonly int _defaultDelayMs;

        public RetryHelper( NLog.ILogger logger, int defaultMaxAttempts = FrameworkConstants.DefaultRetryAttempts, int defaultDelayMs = FrameworkConstants.DefaultRetryDelay)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _defaultMaxAttempts = defaultMaxAttempts;
            _defaultDelayMs = defaultDelayMs;
        }

        /// <summary>
        /// Executes an action with retry logic
        /// </summary>
        public async Task<T> ExecuteWithRetryAsync<T>(Func<Task<T>> action, int maxAttempts = 0, int delayMs = 0)
        {
            maxAttempts = maxAttempts > 0 ? maxAttempts : _defaultMaxAttempts;
            delayMs = delayMs > 0 ? delayMs : _defaultDelayMs;

            Exception? lastException = null;

            for (int attempt = 1; attempt <= maxAttempts; attempt++)
            {
                try
                {
                    _logger.Debug($"Retry attempt {attempt}/{maxAttempts}");
                    return await action();
                }
                catch (Exception ex)
                {
                    lastException = ex;
                    _logger.Warn($"Attempt {attempt}/{maxAttempts} failed: {ex.Message}");

                    if (attempt < maxAttempts)
                    {
                        await Task.Delay(delayMs);
                    }
                }
            }

            _logger.Error($"All {maxAttempts} retry attempts failed", lastException);
            throw lastException ?? new Exception("Retry failed with unknown error");
        }

        /// <summary>
        /// Executes an action with retry logic (void return)
        /// </summary>
        public async Task ExecuteWithRetryAsync(Func<Task> action, int maxAttempts = 0, int delayMs = 0)
        {
            await ExecuteWithRetryAsync(async () =>
            {
                await action();
                return true;
            }, maxAttempts, delayMs);
        }

        /// <summary>
        /// Executes an action with exponential backoff retry
        /// </summary>
        public async Task<T> ExecuteWithExponentialBackoffAsync<T>(Func<Task<T>> action, int maxAttempts = 0, int initialDelayMs = 0)
        {
            maxAttempts = maxAttempts > 0 ? maxAttempts : _defaultMaxAttempts;
            initialDelayMs = initialDelayMs > 0 ? initialDelayMs : _defaultDelayMs;

            Exception? lastException = null;
            int delay = initialDelayMs;

            for (int attempt = 1; attempt <= maxAttempts; attempt++)
            {
                try
                {
                    _logger.Debug($"Exponential backoff retry attempt {attempt}/{maxAttempts} (delay: {delay}ms)");
                    return await action();
                }
                catch (Exception ex)
                {
                    lastException = ex;
                    _logger.Warn($"Attempt {attempt}/{maxAttempts} failed: {ex.Message}");

                    if (attempt < maxAttempts)
                    {
                        await Task.Delay(delay);
                        delay *= 2; // Exponential backoff
                    }
                }
            }

            _logger.Error($"All {maxAttempts} exponential backoff retry attempts failed", lastException);
            throw lastException ?? new Exception("Retry failed with unknown error");
        }
    }
}


