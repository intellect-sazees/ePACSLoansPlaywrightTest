using ePACSLoans.Core.Exceptions;
using ePACSLoans.Core.Interfaces;
using Microsoft.Playwright;
using NLog;

namespace ePACSLoans.Core
{
    /// <summary>
    /// Base class for all page objects providing common functionality
    /// </summary>
    public abstract class BasePage
    {
        protected IPage Page { get; }
        protected IWaitHelper WaitHelper { get; }
        protected NLog.ILogger Logger { get; }
        protected IRetryHelper RetryHelper { get; }

        /// <summary>
        /// Initializes a new instance of BasePage
        /// </summary>
        /// <param name="page">Playwright page instance</param>
        /// <param name="waitHelper">Wait helper instance</param>
        /// <param name="logger">Logger instance</param>
        /// <param name="retryHelper">Retry helper instance</param>
        protected BasePage(IPage page, IWaitHelper waitHelper, NLog.ILogger logger, IRetryHelper retryHelper)
        {
            Page = page ?? throw new ArgumentNullException(nameof(page));
            WaitHelper = waitHelper ?? throw new ArgumentNullException(nameof(waitHelper));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            RetryHelper = retryHelper ?? throw new ArgumentNullException(nameof(retryHelper));
        }

        /// <summary>
        /// Navigates to a URL and waits for page load
        /// Automatically verifies page load and throws PageLoadException if validation fails
        /// </summary>
        /// <param name="url">URL to navigate to</param>
        /// <param name="maxLoadRetries">Maximum number of retries for page load verification (default: 2)</param>
        /// <exception cref="PageLoadException">Thrown when page fails to load correctly after retries</exception>
        protected async Task NavigateAsync(string url, int maxLoadRetries = 2)
        {
            try
            {
                Logger.Info($"Navigating to: {url}");
                await Page.GotoAsync(url);
                
                // Verify page is loaded correctly with retry logic
                var isLoaded = await IsPageLoadedAsync(maxRetries: maxLoadRetries);
                if (!isLoaded)
                {
                    var pageName = GetType().Name;
                    var errorMessage = $"Page '{pageName}' did not load correctly at URL: {url}. " +
                                      $"Required page elements were not found after {maxLoadRetries} verification attempts.";
                    
                    Logger.Error(errorMessage);
                    await TakeScreenshotAsync("page_load_failure");
                    
                    throw new PageLoadException(pageName, url, errorMessage);
                }
                
                Logger.Info($"Successfully navigated and verified page load: {url}");
            }
            catch (PageLoadException)
            {
                // Re-throw PageLoadException as-is
                throw;
            }
            catch (Exception ex)
            {
                var pageName = GetType().Name;
                Logger.Error($"Failed to navigate to {url}", ex);
                await TakeScreenshotAsync("navigation_failure");
                
                // Wrap in PageLoadException for consistency
                throw new PageLoadException(pageName, url, $"Navigation failed: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Verifies that the page has loaded correctly
        /// Checks for page load completion and page-specific validation
        /// Uses retry logic to handle transient load issues
        /// </summary>
        /// <param name="maxRetries">Maximum number of retry attempts (default: 2)</param>
        /// <param name="retryDelayMs">Delay between retries in milliseconds (default: 1000)</param>
        /// <returns>True if page is loaded, false otherwise</returns>
        protected virtual async Task<bool> IsPageLoadedAsync(int maxRetries = 2, int retryDelayMs = 1000)
        {
            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    Logger.Debug($"Page load verification attempt {attempt}/{maxRetries}");
                    
                    // Wait for page load completion
                    await WaitHelper.WaitForPageLoadAsync();
                    
                    // Wait a bit for dynamic content to render
                    await WaitHelper.WaitForTimeoutAsync(500);
                    
                    // Perform page-specific validation
                    var isValid = await ValidatePageLoadedAsync();
                    
                    if (isValid)
                    {
                        Logger.Debug($"Page load verification successful on attempt {attempt}");
                        return true;
                    }
                    
                    if (attempt < maxRetries)
                    {
                        Logger.Warn($"Page load validation failed on attempt {attempt}, retrying in {retryDelayMs}ms...");
                        await WaitHelper.WaitForTimeoutAsync(retryDelayMs);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Warn($"Page load verification attempt {attempt} failed: {ex.Message}");
                    
                    if (attempt < maxRetries)
                    {
                        await WaitHelper.WaitForTimeoutAsync(retryDelayMs);
                    }
                    else
                    {
                        Logger.Error($"Page load verification failed after {maxRetries} attempts", ex);
                    }
                }
            }
            
            return false;
        }

        /// <summary>
        /// Validates that page-specific elements are present
        /// Override this method in derived pages to check for specific elements
        /// </summary>
        /// <returns>True if page-specific validation passes, false otherwise</returns>
        protected virtual async Task<bool> ValidatePageLoadedAsync()
        {
            // Default implementation: just check if page is not null
            // Derived pages should override this to check for specific elements
            return Page != null;
        }

        /// <summary>
        /// Helper method to validate that multiple elements are visible
        /// Useful for pages that require multiple key elements to be present
        /// </summary>
        /// <param name="selectors">Array of element selectors to check</param>
        /// <param name="timeout">Timeout for each element check in milliseconds (default: 5000)</param>
        /// <param name="requireAll">If true, all elements must be visible; if false, at least one must be visible (default: true)</param>
        /// <returns>True if validation passes, false otherwise</returns>
        protected async Task<bool> ValidateElementsVisibleAsync(string[] selectors, int timeout = 5000, bool requireAll = true)
        {
            if (selectors == null || selectors.Length == 0)
            {
                Logger.Warn("No selectors provided for element validation");
                return false;
            }

            var results = new List<bool>();
            
            foreach (var selector in selectors)
            {
                try
                {
                    var isVisible = await IsVisibleAsync(selector, timeout);
                    results.Add(isVisible);
                    
                    if (isVisible)
                    {
                        Logger.Debug($"Element validation passed: {selector}");
                    }
                    else
                    {
                        Logger.Debug($"Element validation failed: {selector}");
                    }
                }
                catch (Exception ex)
                {
                    Logger.Warn($"Error checking element visibility for {selector}: {ex.Message}");
                    results.Add(false);
                }
            }

            if (requireAll)
            {
                var allVisible = results.All(r => r);
                if (!allVisible)
                {
                    Logger.Warn($"Not all required elements are visible. {results.Count(r => r)}/{selectors.Length} elements found");
                }
                return allVisible;
            }
            else
            {
                var anyVisible = results.Any(r => r);
                if (!anyVisible)
                {
                    Logger.Warn($"None of the expected elements are visible");
                }
                return anyVisible;
            }
        }

        /// <summary>
        /// Helper method to validate that at least one of several elements is visible
        /// Useful for pages that may have different layouts or states
        /// </summary>
        /// <param name="selectors">Array of element selectors to check</param>
        /// <param name="timeout">Timeout for each element check in milliseconds (default: 5000)</param>
        /// <returns>True if at least one element is visible, false otherwise</returns>
        protected async Task<bool> ValidateAnyElementVisibleAsync(string[] selectors, int timeout = 5000)
        {
            return await ValidateElementsVisibleAsync(selectors, timeout, requireAll: false);
        }

        /// <summary>
        /// Clicks an element with retry logic
        /// </summary>
        protected async Task ClickAsync(string selector, int? timeout = null)
        {
            try
            {
                Logger.Debug($"Clicking element: {selector}");
                await WaitHelper.WaitForElementClickableAsync(selector, timeout);
                await Page.ClickAsync(selector);
                Logger.Debug($"Successfully clicked element: {selector}");
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to click element: {selector}", ex);
                await TakeScreenshotAsync("click_failure");
                throw;
            }
        }

        /// <summary>
        /// Fills an input field with text
        /// </summary>
        protected async Task FillAsync(string selector, string value, int? timeout = null)
        {
            try
            {
                Logger.Debug($"Filling element {selector} with value: {value}");
                await WaitHelper.WaitForElementVisibleAsync(selector, timeout);
                await Page.FillAsync(selector, value);
                Logger.Debug($"Successfully filled element: {selector}");
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to fill element: {selector}", ex);
                await TakeScreenshotAsync("fill_failure");
                throw;
            }
        }

        /// <summary>
        /// Selects an option from a dropdown
        /// </summary>
        protected async Task SelectOptionAsync(string selector, string value, int? timeout = null)
        {
            try
            {
                Logger.Debug($"Selecting option '{value}' from dropdown: {selector}");
                await WaitHelper.WaitForElementVisibleAsync(selector, timeout);
                await Page.SelectOptionAsync(selector, value);
                Logger.Debug($"Successfully selected option: {value}");
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to select option from: {selector}", ex);
                await TakeScreenshotAsync("select_failure");
                throw;
            }
        }

        /// <summary>
        /// Gets text content from an element
        /// </summary>
        protected async Task<string> GetTextAsync(string selector, int? timeout = null)
        {
            try
            {
                await WaitHelper.WaitForElementVisibleAsync(selector, timeout);
                var text = await Page.Locator(selector).TextContentAsync();
                Logger.Debug($"Retrieved text from {selector}: {text}");
                return text ?? string.Empty;
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to get text from: {selector}", ex);
                throw;
            }
        }

        /// <summary>
        /// Checks if an element is visible
        /// </summary>
        protected async Task<bool> IsVisibleAsync(string selector, int? timeout = null)
        {
            try
            {
                return await WaitHelper.WaitForElementVisibleAsync(selector, timeout);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Takes a screenshot
        /// </summary>
        protected async Task TakeScreenshotAsync(string name)
        {
            try
            {
                var screenshotPath = Path.Combine(
                    FrameworkConstants.TestReportsFolder,
                    $"{name}_{DateTime.Now:yyyyMMdd_HHmmss}.png"
                );
                await Page.ScreenshotAsync(new PageScreenshotOptions { Path = screenshotPath });
                Logger.Info($"Screenshot saved: {screenshotPath}");
            }
            catch (Exception ex)
            {
                Logger.Warn($"Failed to take screenshot: {name}", ex);
            }
        }

        /// <summary>
        /// Executes JavaScript on the page
        /// </summary>
        protected async Task<T> EvaluateAsync<T>(string script)
        {
            try
            {
                Logger.Debug($"Executing JavaScript: {script}");
                var result = await Page.EvaluateAsync<T>(script);
                return result;
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to execute JavaScript: {script}", ex);
                throw;
            }
        }

        /// <summary>
        /// Waits for page to be ready
        /// </summary>
        protected async Task WaitForPageReadyAsync()
        {
            await WaitHelper.WaitForPageLoadAsync();
            await WaitHelper.WaitForTimeoutAsync(500); // Small delay for dynamic content
        }
    }
}


