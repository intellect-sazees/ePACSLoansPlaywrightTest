using ePACSLoans.Core.Interfaces;
using ePACSLoans.Models.Data;
using ePACSLoans.Models.Locaters;
using Microsoft.Playwright;
using NLog;
using System.Runtime.CompilerServices;

namespace ePACSLoans.Core.Components
{
    /// <summary>
    /// Base class for all form components
    /// Provides common functionality for form interactions
    /// </summary>
    public abstract class BaseFormComponent
    {
        protected IPage Page { get; }
        protected IWaitHelper WaitHelper { get; }
        protected NLog.ILogger Logger { get; }
        protected IRetryHelper RetryHelper { get; }

        /// <summary>
        /// Initializes a new instance of BaseFormComponent
        /// </summary>
        /// <param name="page">Playwright page instance</param>
        /// <param name="waitHelper">Wait helper instance</param>
        /// <param name="logger">Logger instance</param>
        /// <param name="retryHelper">Retry helper instance</param>
        protected BaseFormComponent(IPage page,IWaitHelper waitHelper,NLog.ILogger logger,IRetryHelper retryHelper)
        {
            Page = page ?? throw new ArgumentNullException(nameof(page));
            WaitHelper = waitHelper ?? throw new ArgumentNullException(nameof(waitHelper));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            RetryHelper = retryHelper ?? throw new ArgumentNullException(nameof(retryHelper));
        }
        /// <summary>
        /// Fills the form with provided data
        /// Must be implemented by derived components
        /// </summary>
        /// <param name="data">Data object containing form field values</param>
        public abstract Task FillAsync(Tuple<LandDeclarationData, LandDeclarationLocaters> data)  ;
        /// <summary>
        /// Validates that the form is filled correctly
        /// Optional - can be overridden by derived components
        /// </summary>
        /// <param name="data">Data object to validate against</param>
        /// <returns>True if validation passes, false otherwise</returns>
        public virtual async Task<bool> ValidateAsync<T>(T data) where T : class
        {
            // Default implementation - can be overridden
            Logger.Debug("Form validation not implemented, skipping");
            return true;
        }

        /// <summary>
        /// Clears all form fields
        /// Optional - can be overridden by derived components
        /// </summary>
        public virtual async Task ClearAsync()
        {
            Logger.Debug("Form clear not implemented, skipping");
        }

        /// <summary>
        /// Helper method to fill a text input field
        /// </summary>
        protected async Task FillTextFieldAsync(string selector, string value, int? timeout = null)
        {
            try
            {
                Logger.Debug($"Filling text field {selector} with value: {value}");
                await WaitHelper.WaitForElementVisibleAsync(selector, timeout);
                await Page.FillAsync(selector, value);
                Logger.Debug($"Successfully filled text field: {selector}");
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to fill text field: {selector}", ex);
                throw;
            }
        }

        /// <summary>
        /// Helper method to select an option from a dropdown
        /// </summary>
        protected async Task SelectDropdownOptionAsync(string selector, string value, int? timeout = null)
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
                Logger.Error($"Failed to select option from dropdown: {selector}", ex);
                throw;
            }
        }

        /// <summary>
        /// Helper method to click an element
        /// </summary>
        protected async Task ClickElementAsync(string selector, int? timeout = null)
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
                throw;
            }
        }

        /// <summary>
        /// Helper method to check if an element is visible
        /// </summary>
        protected async Task<bool> IsElementVisibleAsync(string selector, int? timeout = null)
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
    }
}

