using ePACSLoans.Utilities.Helpers;
using NLog;

namespace ePACSLoans.Utilities.Configuration
{
    /// <summary>
    /// Helper class for reading environment variables
    /// Supports reading from .env files and system environment variables
    /// </summary>
    public class EnvironmentVariableHelper
    {
        private readonly NLog.ILogger _logger;
        private static Dictionary<string, string>? _envCache;

        public EnvironmentVariableHelper(NLog.ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            LoadEnvironmentVariables();
        }

        /// <summary>
        /// Gets an environment variable value
        /// </summary>
        /// <param name="key">Environment variable key</param>
        /// <param name="defaultValue">Default value if not found</param>
        /// <returns>Environment variable value or default value</returns>
        public string GetEnvironmentVariable(string key, string? defaultValue = null)
        {
            try
            {
                // First check cache (from .env file)
                if (_envCache != null && _envCache.TryGetValue(key, out var cachedValue))
                {
                    return cachedValue;
                }

                // Then check system environment variables
                var value = Environment.GetEnvironmentVariable(key);
                if (!string.IsNullOrEmpty(value))
                {
                    return value;
                }

                // Return default if provided
                if (defaultValue != null)
                {
                    _logger.Warn($"Environment variable '{key}' not found, using default value");
                    return defaultValue;
                }

                throw new KeyNotFoundException($"Environment variable '{key}' not found and no default value provided");
            }
            catch (Exception ex)
            {
                _logger.Error($"Failed to get environment variable '{key}'", ex);
                throw;
            }
        }

        /// <summary>
        /// Gets an environment variable value or throws if not found
        /// </summary>
        public string GetRequiredEnvironmentVariable(string key)
        {
            var value = GetEnvironmentVariable(key);
            if (string.IsNullOrEmpty(value))
            {
                throw new InvalidOperationException($"Required environment variable '{key}' is not set. Please check your .env file or system environment variables.");
            }
            return value;
        }

        /// <summary>
        /// Loads environment variables from .env file if it exists
        /// </summary>
        private void LoadEnvironmentVariables()
        {
            try
            {
                // Use PathHelper to get project root, not current directory
                // Current directory might be bin/Debug/net8.0 when tests run
                var pathHelper = new PathHelper();
                var projectPath = pathHelper.getProjectPath();
                var envPath = Path.Combine(projectPath, ".env");
                
                _logger.Debug($"Looking for .env file at: {envPath}");
                
                if (!File.Exists(envPath))
                {
                    _logger.Info($".env file not found at {envPath}. Using system environment variables only.");
                    return;
                }

                _envCache = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                var lines = File.ReadAllLines(envPath);

                foreach (var line in lines)
                {
                    var trimmedLine = line.Trim();
                    
                    // Skip empty lines and comments
                    if (string.IsNullOrEmpty(trimmedLine) || trimmedLine.StartsWith("#"))
                        continue;

                    // Parse KEY=VALUE format
                    var index = trimmedLine.IndexOf('=');
                    if (index > 0)
                    {
                        var key = trimmedLine.Substring(0, index).Trim();
                        var value = trimmedLine.Substring(index + 1).Trim();
                        
                        // Remove quotes if present
                        if ((value.StartsWith("\"") && value.EndsWith("\"")) ||
                            (value.StartsWith("'") && value.EndsWith("'")))
                        {
                            value = value.Substring(1, value.Length - 2);
                        }

                        _envCache[key] = value;
                    }
                }

                _logger.Info($"Loaded {_envCache.Count} environment variables from .env file");
            }
            catch (Exception ex)
            {
                _logger.Warn($"Failed to load .env file: {ex.Message}. Using system environment variables only.");
            }
        }

        /// <summary>
        /// Replaces environment variable placeholders in a string (e.g., ${VAR_NAME})
        /// </summary>
        public string ReplaceEnvironmentVariables(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            var result = input;
            var pattern = @"\$\{([^}]+)\}";

            result = System.Text.RegularExpressions.Regex.Replace(result, pattern, match =>
            {
                var varName = match.Groups[1].Value;
                try
                {
                    return GetEnvironmentVariable(varName, match.Value); // Return original if not found
                }
                catch
                {
                    return match.Value; // Return original placeholder if not found
                }
            });

            return result;
        }
    }
}

