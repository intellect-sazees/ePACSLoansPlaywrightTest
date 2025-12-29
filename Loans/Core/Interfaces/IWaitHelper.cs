namespace ePACSLoans.Core.Interfaces
{
    /// <summary>
    /// Interface for wait operations in test automation
    /// </summary>
    public interface IWaitHelper
    {
        /// <summary>
        /// Waits for an element to become visible
        /// </summary>
        Task<bool> WaitForElementVisibleAsync(string selector, int? timeout = null);

        /// <summary>
        /// Waits for an element to become clickable
        /// </summary>
        Task<bool> WaitForElementClickableAsync(string selector, int? timeout = null);

        /// <summary>
        /// Waits for an element to disappear
        /// </summary>
        Task<bool> WaitForElementToDisappearAsync(string selector, int? timeout = null);

        /// <summary>
        /// Waits for specific text to appear in an element
        /// </summary>
        Task<bool> WaitForTextAsync(string selector, string text, int? timeout = null);

        /// <summary>
        /// Waits for page to load completely
        /// </summary>
        Task<bool> WaitForPageLoadAsync();

        /// <summary>
        /// Waits for navigation to complete
        /// </summary>
        Task<bool> WaitForNavigationAsync();

        /// <summary>
        /// Waits for URL to match expected value
        /// </summary>
        Task<bool> WaitForUrlAsync(string url, int? timeout = null);

        /// <summary>
        /// Waits for specified timeout
        /// </summary>
        Task<bool> WaitForTimeoutAsync(int milliseconds);

        /// <summary>
        /// Waits for a custom condition to be met
        /// </summary>
        Task<bool> WaitForConditionAsync(Func<Task<bool>> condition, int? timeout = null, int pollInterval = 500);
    }
}


