namespace ePACSLoans.Core.Interfaces
{
    /// <summary>
    /// Interface for retry operations
    /// </summary>
    public interface IRetryHelper
    {
        /// <summary>
        /// Executes an action with retry logic
        /// </summary>
        Task<T> ExecuteWithRetryAsync<T>(Func<Task<T>> action, int maxAttempts = 3, int delayMs = 1000);

        /// <summary>
        /// Executes an action with retry logic (void return)
        /// </summary>
        Task ExecuteWithRetryAsync(Func<Task> action, int maxAttempts = 3, int delayMs = 1000);

        /// <summary>
        /// Executes an action with exponential backoff retry
        /// </summary>
        Task<T> ExecuteWithExponentialBackoffAsync<T>(Func<Task<T>> action, int maxAttempts = 3, int initialDelayMs = 1000);
    }
}


