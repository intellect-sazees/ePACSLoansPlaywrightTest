using ePACSLoans.Core;
using ePACSLoans.Core.Interfaces;
using ePACSLoans.Utilities.Configuration;
using ePACSLoans.Utilities.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Concurrent;

namespace ePACSLoans.Utilities.DataManagement
{
    /// <summary>
    /// Test data reader implementing ITestDataProvider interface
    /// Provides JSON test data reading with caching for performance
    /// </summary>
    public class TestDataReader : ITestDataProvider
    {
        private readonly NLog.ILogger _logger;
        private readonly EnvironmentVariableHelper? _envHelper;
        private static readonly ConcurrentDictionary<string, JObject> _cache = new();

        public TestDataReader(NLog.ILogger logger, EnvironmentVariableHelper? envHelper = null)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _envHelper = envHelper ?? new EnvironmentVariableHelper(logger);
        }

        /// <summary>
        /// Gets test data for a specific module and test case
        /// </summary>
        public async Task<T> GetTestDataAsync<T>(string module, string testCase) where T : class
        {
            try
            {
                var filePath = GetTestDataFilePath(module);
                var section = $"{module}_{testCase}";
                
                _logger.Debug($"Loading test data for module: {module}, test case: {testCase}");
                
                var json = await GetCachedJsonAsync(filePath);
                var sectionData = json[section];

                if (sectionData == null)
                {
                    throw new KeyNotFoundException($"Test data section '{section}' not found in {filePath}");
                }

                var result = sectionData.ToObject<T>();
                if (result == null)
                {
                    throw new InvalidOperationException($"Failed to deserialize test data for {section}");
                }

                _logger.Debug($"Successfully loaded test data for {section}");
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error($"Failed to get test data for module: {module}, test case: {testCase}", ex);
                throw;
            }
        }

        /// <summary>
        /// Gets test data from a specific JSON file
        /// </summary>
        public async Task<T> GetTestDataFromFileAsync<T>(string filePath, string section) where T : class
        {
            try
            {
                _logger.Debug($"Loading test data from file: {filePath}, section: {section}");
                
                var json = await GetCachedJsonAsync(filePath);
                var sectionData = json[section];

                if (sectionData == null)
                {
                    throw new KeyNotFoundException($"Section '{section}' not found in {filePath}");
                }

                var result = sectionData.ToObject<T>();
                if (result == null)
                {
                    throw new InvalidOperationException($"Failed to deserialize test data for section '{section}'");
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.Error($"Failed to get test data from file: {filePath}, section: {section}", ex);
                throw;
            }
        }

        /// <summary>
        /// Gets a specific value from test data
        /// </summary>
        public async Task<string> GetDataAsync(string module, string testCase, string key)
        {
            try
            {
                var filePath = GetTestDataFilePath(module);
                var section = $"{module}_{testCase}";
                
                var json = await GetCachedJsonAsync(filePath);
                var sectionData = json[section];

                if (sectionData == null)
                {
                    throw new KeyNotFoundException($"Section '{section}' not found");
                }

                var value = sectionData[key]?.ToString();
                if (value == null)
                {
                    throw new KeyNotFoundException($"Key '{key}' not found in section '{section}'");
                }

                return value;
            }
            catch (Exception ex)
            {
                _logger.Error($"Failed to get data for module: {module}, test case: {testCase}, key: {key}", ex);
                throw;
            }
        }

        /// <summary>
        /// Updates test data value (legacy method for backward compatibility)
        /// </summary>
        public async Task UpdateDataAsync(string module, string testCase, string key, string value)
        {
            try
            {
                var filePath = GetTestDataFilePath(module);
                var json = await GetCachedJsonAsync(filePath);
                
                var section = $"{module}_{testCase}";
                var sectionData = json[section] as JObject;

                if (sectionData == null)
                {
                    throw new KeyNotFoundException($"Section '{section}' not found");
                }

                sectionData[key] = value;
                
                // Write back to file and clear cache
                await File.WriteAllTextAsync(filePath, json.ToString());
                _cache.TryRemove(filePath, out _);
                
                _logger.Info($"Updated test data: {module}.{testCase}.{key} = {value}");
            }
            catch (Exception ex)
            {
                _logger.Error($"Failed to update test data for module: {module}, test case: {testCase}, key: {key}", ex);
                throw;
            }
        }

        /// <summary>
        /// Legacy method for backward compatibility
        /// </summary>
        public string GetData(string filePath, string jsonObjectName, string inputKey)
        {
            try
            {
                var json = GetCachedJsonAsync(filePath).GetAwaiter().GetResult();
                var obj = json[jsonObjectName];
                
                if (obj == null)
                {
                    throw new KeyNotFoundException($"JSON object '{jsonObjectName}' not found in {filePath}");
                }

                var value = obj[inputKey]?.ToString();
                if (value == null)
                {
                    throw new KeyNotFoundException($"Key '{inputKey}' not found in '{jsonObjectName}'");
                }

                return value;
            }
            catch (Exception ex)
            {
                _logger.Error($"Failed to get data from {filePath}, object: {jsonObjectName}, key: {inputKey}", ex);
                return string.Empty;
            }
        }

        private async Task<JObject> GetCachedJsonAsync(string filePath)
        {
            return _cache.GetOrAdd(filePath, path =>
            {
                if (!File.Exists(path))
                {
                    throw new FileNotFoundException($"Test data file not found: {path}");
                }

                var content = File.ReadAllText(path);
                
                // Replace environment variables in JSON content
                if (_envHelper != null)
                {
                    content = _envHelper.ReplaceEnvironmentVariables(content);
                }
                
                return JObject.Parse(content);
            });
        }

        /// <summary>
        /// Gets the test data file path for a specific module
        /// Supports both old structure (TestData.json) and new structure (Module/ModuleTestData.json)
        /// </summary>
        private string GetTestDataFilePath(string module)
        {
            var pathHelper = new PathHelper();
            var basePath = pathHelper.getProjectPath();
            
            // Map module names to folder names
            var moduleFolder = module switch
            {
                "Login" or "Logincredantional" => "Login",
                "Borrowings" or "AccountCreation" or "AccountCreationData" => "Borrowings",
                "Dashboard" => "Dashboard",
                _ => module // Use module name as-is if no mapping
            };

            // Try new structure first: Resources/TestData/Module/ModuleTestData.json
            var newStructurePath = Path.Combine(basePath, FrameworkConstants.ResourcesFolder, 
                FrameworkConstants.TestDataFolder, moduleFolder, $"{moduleFolder}TestData.json");
            
            if (File.Exists(newStructurePath))
            {
                _logger.Debug($"Using new structure: {newStructurePath}");
                return newStructurePath;
            }

            // Fallback to old structure: Resources/TestData/TestData.json
            var oldStructurePath = Path.Combine(basePath, FrameworkConstants.ResourcesFolder, 
                FrameworkConstants.TestDataFolder, "TestData.json");
            
            if (File.Exists(oldStructurePath))
            {
                _logger.Debug($"Using old structure: {oldStructurePath}");
                return oldStructurePath;
            }

            // If neither exists, return new structure path (will throw FileNotFoundException)
            _logger.Warn($"Test data file not found for module '{module}'. Tried: {newStructurePath} and {oldStructurePath}");
            return newStructurePath;
        }
    }
}


