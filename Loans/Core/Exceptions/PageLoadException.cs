namespace ePACSLoans.Core.Exceptions
{
    /// <summary>
    /// Exception thrown when a page fails to load correctly
    /// Provides context about which page failed and the URL
    /// </summary>
    public class PageLoadException : Exception
    {
        /// <summary>
        /// Gets the name of the page that failed to load
        /// </summary>
        public string PageName { get; }

        /// <summary>
        /// Gets the URL that was being navigated to
        /// </summary>
        public string Url { get; }

        /// <summary>
        /// Initializes a new instance of PageLoadException
        /// </summary>
        /// <param name="pageName">Name of the page that failed to load</param>
        /// <param name="url">URL that was being navigated to</param>
        /// <param name="innerException">The inner exception that caused this exception</param>
        public PageLoadException(string pageName, string url, Exception? innerException = null)
            : base($"Page '{pageName}' failed to load correctly at URL '{url}'. " +
                   $"The page may not have loaded completely or required elements are missing.", innerException)
        {
            PageName = pageName ?? throw new ArgumentNullException(nameof(pageName));
            Url = url ?? throw new ArgumentNullException(nameof(url));
        }

        /// <summary>
        /// Initializes a new instance of PageLoadException with a custom message
        /// </summary>
        /// <param name="pageName">Name of the page that failed to load</param>
        /// <param name="url">URL that was being navigated to</param>
        /// <param name="message">Custom error message</param>
        /// <param name="innerException">The inner exception that caused this exception</param>
        public PageLoadException(string pageName, string url, string message, Exception? innerException = null)
            : base(message, innerException)
        {
            PageName = pageName ?? throw new ArgumentNullException(nameof(pageName));
            Url = url ?? throw new ArgumentNullException(nameof(url));
        }
    }
}

