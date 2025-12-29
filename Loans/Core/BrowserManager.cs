using ePACSLoans.Core.Interfaces;
using Microsoft.Playwright;
using NLog;
using System.Collections.Concurrent;
using System.Xml.Linq;

namespace ePACSLoans.Core
{
    /// <summary>
    /// Singleton browser manager for thread-safe browser lifecycle management
    /// Supports parallel test execution with isolated contexts
    /// </summary>
    public class BrowserManager : IDisposable
    {
        private static readonly Lazy<BrowserManager> _instance = 
            new Lazy<BrowserManager>(() => new BrowserManager(), 
                System.Threading.LazyThreadSafetyMode.ExecutionAndPublication);

        public static BrowserManager Instance => _instance.Value;

        private IPlaywright? _playwright;
        private IBrowser? _browser;
        private readonly ConcurrentDictionary<int, IBrowserContext> _contexts = new();
        private readonly NLog.ILogger _logger;
        private BrowserType _browserType = BrowserType.Chromium;
        private bool _headless = false;
        private string? _videoDirectory;

        private BrowserManager()
        {
            // Logger will be injected via DI in future, for now use a simple implementation
           _logger = LogManager.GetLogger("BrowserManager");
        }

        /// <summary>
        /// Initializes the Playwright instance and browser
        /// </summary>
        public async Task InitializeAsync(BrowserType browserType = BrowserType.Chromium, bool headless = false, string? videoDirectory = null)
        {
            try
            {
                _browserType = browserType;
                _headless = headless;
                _videoDirectory = videoDirectory;

                _logger.Info($"Initializing Playwright with browser: {browserType}, Headless: {headless}");

                if (_playwright == null)
                {
                    _playwright = await Playwright.CreateAsync();
                }

                if (_browser == null)
                {
                    _browser = await LaunchBrowserAsync(browserType, headless);
                    _logger.Info($"Browser launched successfully: {browserType}");
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to initialize browser", ex);
                throw;
            }
        }

        /// <summary>
        /// Creates a new browser context for test isolation
        /// </summary>
        public async Task<IBrowserContext> CreateContextAsync(BrowserNewContextOptions? options = null)
        {
            if (_browser == null)
            {
                await InitializeAsync(_browserType, _headless, _videoDirectory);
            }

            try
            {
                var contextOptions = options ?? new BrowserNewContextOptions();

                // Set video recording if directory is provided
                if (!string.IsNullOrEmpty(_videoDirectory))
                {
                    contextOptions.RecordVideoDir = _videoDirectory;
                    contextOptions.RecordVideoSize = new() 
                    { 
                        Width = FrameworkConstants.VideoWidth, 
                        Height = FrameworkConstants.VideoHeight 
                    };
                }

                var context = await _browser!.NewContextAsync(contextOptions);
                var contextId = context.GetHashCode();
                _contexts.TryAdd(contextId, context);

                _logger.Info($"Created new browser context: {contextId}");
                return context;
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to create browser context", ex);
                throw;
            }
        }

        /// <summary>
        /// Creates a new page from a context
        /// </summary>
        public async Task<IPage> CreatePageAsync(IBrowserContext context)
        {
            try
            {
                var page = await context.NewPageAsync();
                _logger.Debug($"Created new page in context");
                return page;
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to create page", ex);
                throw;
            }
        }

        /// <summary>
        /// Closes a browser context
        /// </summary>
        public async Task CloseContextAsync(IBrowserContext context)
        {
            try
            {
                var contextId = context.GetHashCode();
                await context.CloseAsync();
                _contexts.TryRemove(contextId, out _);
                _logger.Info($"Closed browser context: {contextId}");
            }
            catch (Exception ex)
            {
                _logger.Warn($"Failed to close context", ex);
            }
        }
        private async Task<IBrowser> LaunchBrowserAsync(BrowserType browserType, bool headless)
        {
            var launchOptions = new BrowserTypeLaunchOptions
            {
                Headless = headless,
                Args = headless ? Array.Empty<string>() : new[] { "--start-maximized" }
            };

            return browserType switch
            {
                BrowserType.Chromium => await _playwright!.Chromium.LaunchAsync(launchOptions),
                BrowserType.Firefox => await _playwright!.Firefox.LaunchAsync(launchOptions),
              //  BrowserType.WebKit => await _playwright!.WebKit.LaunchAsync(launchOptions),
                _ => throw new ArgumentException($"Unsupported browser type: {browserType}")
            };
        }
        /// <summary>
        /// Disposes all resources
        /// </summary>
        public async Task DisposeAsync()
        {
            try
            {
                // Close all contexts
                foreach (var context in _contexts.Values)
                {
                    await CloseContextAsync(context);
                }
                _contexts.Clear();

                // Close browser
                if (_browser != null)
                {
                    await _browser.CloseAsync();
                    _browser = null;
                }

                // Dispose Playwright
                _playwright?.Dispose();
                _playwright = null;

                _logger.Info("BrowserManager disposed successfully");
            }
            catch (Exception ex)
            {
                _logger.Error("Error during BrowserManager disposal", ex);
            }
        }

        public void Dispose()
        {
            DisposeAsync().GetAwaiter().GetResult();
        }
    }

    /// <summary>
    /// Browser type enumeration
    /// </summary>
    public enum BrowserType
    {
        Chromium,
        Firefox,
        WebKit
    }
}


