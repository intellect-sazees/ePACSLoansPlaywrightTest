using ePACSLoans.Utilities.Helpers;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace ePACSLoans.Core
{
    /// <summary>
    /// Manages application configuration from appsettings.json files
    /// </summary>
    public class ConfigurationManager
    {
        private static readonly Lazy<ConfigurationManager> _instance = new Lazy<ConfigurationManager>(() => new ConfigurationManager());
        public static ConfigurationManager Instance => _instance.Value;
        PathHelper _pathHelper = new PathHelper();
        private IConfigurationRoot _configuration;
        private ConfigurationManager()
        {
            LoadConfiguration();
        }

        /// <summary>
        /// Loads configuration from appsettings.json files
        /// </summary>
        private void LoadConfiguration()
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
            var basePath = _pathHelper.getProjectPath();
            var builder = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile(FrameworkConstants.AppSettingsFile, optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
            _configuration = builder.Build();
        }
        public string GetValue(string key)
        {
            return _configuration[key] ?? throw new KeyNotFoundException($"Configuration key '{key}' not found");
        }

        /// <summary>
        /// Gets a configuration value with default
        /// </summary>
        public string GetValue(string key, string defaultValue)
        {
            return _configuration[key] ?? defaultValue;
        }

        /// <summary>
        /// Gets a configuration section
        /// </summary>
        public IConfigurationSection GetSection(string key)
        {
            return _configuration.GetSection(key);
        }

        /// <summary>
        /// Gets typed configuration
        /// </summary>
        public T Get<T>(string sectionName) where T : class, new()
        {
            var section = GetSection(sectionName);
            var instance = new T();
            section.Bind(instance);
            return instance;
        }

        // Common configuration properties
        public string BaseUrl => GetValue("Application:BaseUrl", "https://localhost");
        public string Browser => GetValue("Application:Browser", "Chromium");
        public bool Headless => bool.Parse(GetValue("Application:Headless", "false"));
        public int DefaultTimeout => int.Parse(GetValue("Application:DefaultTimeout", FrameworkConstants.DefaultTimeout.ToString()));
    }
}

