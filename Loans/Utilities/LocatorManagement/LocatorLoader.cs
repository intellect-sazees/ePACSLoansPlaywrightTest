using ePACSLoans.Core;
using ePACSLoans.Core.Interfaces;
using ePACSLoans.Utilities.DataManagement;
using ePACSLoans.Utilities.Helpers;
using NLog;

namespace ePACSLoans.Utilities.LocatorManagement
{
    /// <summary>
    /// Service for loading element locators from JSON files
    /// Centralizes locator loading logic and removes duplication across pages
    /// </summary>
    public class LocatorLoader : ILocatorLoader
    {
        private readonly ITestDataProvider _testDataProvider;
        private readonly NLog.ILogger _logger;
        private readonly string _locatorsFilePath;

        /// <summary>
        /// Initializes a new instance of LocatorLoader
        /// </summary>
        public LocatorLoader(ITestDataProvider testDataProvider, NLog.ILogger logger)
        {
            _testDataProvider = testDataProvider ?? throw new ArgumentNullException(nameof(testDataProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _locatorsFilePath = GetLocatorsFilePath();
        }

        /// <summary>
        /// Loads locators for a specific page from JSON file
        /// Uses a mapping function to map property names to JSON keys
        /// </summary>
        /// <typeparam name="T">Type of locator class to return</typeparam>
        /// <param name="pageName">Name of the page (e.g., "LoginPage", "DashboardPage")</param>
        /// <param name="keyMapping">Optional function to map property names to JSON keys. 
        /// If null, uses property name as-is (case-insensitive)</param>
        /// <returns>Instance of locator class with all properties populated</returns>
        public T LoadLocators<T>(string pageName, Func<string, string>? keyMapping = null) where T : class, new()
        {
            try
            {
                _logger.Debug($"Loading locators for page: {pageName}");

                // Use TestDataReader's GetData method for backward compatibility
                var legacyReader = _testDataProvider as TestDataReader;
                if (legacyReader == null)
                {
                    throw new InvalidOperationException("LocatorLoader requires TestDataReader implementation");
                }

                var get = (string key) => legacyReader.GetData(_locatorsFilePath, pageName, key);
                
                var locatorInstance = new T();
                var properties = typeof(T).GetProperties();
                var loadedCount = 0;

                foreach (var property in properties)
                {
                    if (!property.CanWrite || property.PropertyType != typeof(string))
                        continue;

                    try
                    {
                        // Determine the JSON key to use
                        var jsonKey = keyMapping != null 
                            ? keyMapping(property.Name) 
                            : property.Name; // Default: use property name

                        // Try to get the locator value
                        var locatorValue = get(jsonKey);
                        
                        if (string.IsNullOrEmpty(locatorValue))
                        {
                            _logger.Warn($"Locator key '{jsonKey}' not found or empty for page '{pageName}', property '{property.Name}'");
                            continue;
                        }

                        property.SetValue(locatorInstance, locatorValue);
                        loadedCount++;
                    }
                    catch (Exception ex)
                    {
                        _logger.Warn($"Failed to load locator for property '{property.Name}' in page '{pageName}': {ex.Message}");
                        // Continue loading other properties
                    }
                }

                _logger.Info($"Successfully loaded {loadedCount}/{properties.Length} locators for page: {pageName}");
                return locatorInstance;
            }
            catch (Exception ex)
            {
                _logger.Error($"Failed to load locators for page: {pageName}", ex);
                throw;
            }
        }

        /// <summary>
        /// Gets a single locator value by page name and key
        /// </summary>
        /// <param name="pageName">Name of the page</param>
        /// <param name="key">Locator key (JSON key name)</param>
        /// <returns>Locator string value</returns>
        public string GetLocator(string pageName, string key)
        {
            try
            {
                var legacyReader = _testDataProvider as TestDataReader;
                if (legacyReader == null)
                {
                    throw new InvalidOperationException("LocatorLoader requires TestDataReader implementation");
                }

                return legacyReader.GetData(_locatorsFilePath, pageName, key);
            }
            catch (Exception ex)
            {
                _logger.Error($"Failed to get locator for page: {pageName}, key: {key}", ex);
                throw;
            }
        }

        /// <summary>
        /// Gets the path to the locators JSON file
        /// </summary>
        private string GetLocatorsFilePath()
        {
            try
            {
                var pathHelper = new PathHelper();
                var basePath = pathHelper.getProjectPath();
                
                var locatorsPath = Path.Combine(
                    basePath,
                    FrameworkConstants.ResourcesFolder,
                    FrameworkConstants.ElementLocatorsFolder,
                    "ElementLocaters.json"
                );

                if (!File.Exists(locatorsPath))
                {
                    _logger.Warn($"Locators file not found at: {locatorsPath}");
                }

                return locatorsPath;
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to get locators file path", ex);
                throw;
            }
        }
    }
}

