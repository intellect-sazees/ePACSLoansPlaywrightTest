namespace ePACSLoans.Core.Interfaces
{
    /// <summary>
    /// Interface for loading element locators from JSON files
    /// Centralizes locator loading logic and removes duplication
    /// </summary>
    public interface ILocatorLoader
    {
        /// <summary>
        /// Loads locators for a specific page from JSON file
        /// </summary>
        /// <typeparam name="T">Type of locator class to return</typeparam>
        /// <param name="pageName">Name of the page (e.g., "LoginPage", "DashboardPage")</param>
        /// <param name="keyMapping">Optional function to map property names to JSON keys. 
        /// If null, uses property name as-is (case-insensitive)</param>
        /// <returns>Instance of locator class with all properties populated</returns>
        T LoadLocators<T>(string pageName, Func<string, string>? keyMapping = null) where T : class, new();

        /// <summary>
        /// Gets a single locator value by page name and key
        /// </summary>
        /// <param name="pageName">Name of the page</param>
        /// <param name="key">Locator key</param>
        /// <returns>Locator string value</returns>
        string GetLocator(string pageName, string key);
    }
}

