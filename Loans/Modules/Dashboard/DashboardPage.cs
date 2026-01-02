using ePACSLoans.Core;
using ePACSLoans.Core.Interfaces;
using ePACSLoans.Utilities.DataManagement;
using ePACSLoans.Utilities.Helpers;
using Microsoft.Playwright;
using ePACSLoans.Models.Locaters;

namespace ePACSLoans.Modules.Dashboard
{
    /// <summary>
    /// Page Object Model for Dashboard page
    /// Inherits from BasePage and uses dependency injection
    /// </summary>
    public class DashboardPage : BasePage
    {
        private readonly ITestDataProvider _testDataProvider;
        private readonly DashboardLocators _locators;
        /// <summary>
        /// Initializes a new instance of DashboardPage with dependency injection
        /// </summary>
        public DashboardPage(IPage page,IWaitHelper waitHelper,NLog.ILogger logger,IRetryHelper retryHelper,ITestDataProvider testDataProvider,Core.Interfaces.ILocatorLoader locatorLoader): base(page, waitHelper, logger, retryHelper)
        {
            _testDataProvider = testDataProvider ?? throw new ArgumentNullException(nameof(testDataProvider));
            _locators = locatorLoader?.LoadLocators<DashboardLocators>("DashboardPage", MapPropertyToKey)?? throw new ArgumentNullException(nameof(locatorLoader));
        }
        /// <summary>
        /// Maps property names to JSON keys for DashboardPage locators
        /// </summary>
        private static string MapPropertyToKey(string propertyName)
        {
            return propertyName switch
            {
                nameof(DashboardLocators.PacsInfoConfigAlert) => "pacsInfoConfigAllert",
                nameof(DashboardLocators.OKButton) => "okButton",
                nameof(DashboardLocators.NabardLogo) => "nabardLogo",
                nameof(DashboardLocators.BorrowingsIcon) => "borrowingsIcon",
                nameof(DashboardLocators.MembershipIcon)=> "membershipIcon",
                nameof(DashboardLocators.LoansIcon)=> "loansIcon",
                nameof(DashboardLocators.ConfigurationMenu)=> "ConfigurationMenu",
                _ => propertyName // Default: use property name as-is
            };
        }

        /// <summary>
        /// Handles the PACS Info Config alert if it appears
        /// </summary>
        public async Task HandleDashboardAlertAsync()
        {
            try
            {
                Logger.Debug("Checking for dashboard alert");
                
                var isPageLoaded = await WaitHelper.WaitForPageLoadAsync();
                if (!isPageLoaded)
                {
                    Logger.Warn("Page did not load completely");
                    return;
                }
                var isAlertVisible = await WaitHelper.WaitForElementVisibleAsync(_locators.PacsInfoConfigAlert, 3000);
                if (isAlertVisible)
                {
                    Logger.Info("PACS Info Config alert detected, closing it");
                    
                    var isOkButtonVisible = await WaitHelper.WaitForElementVisibleAsync(_locators.OKButton, 2000);
                    if (isOkButtonVisible)
                    {
                        await ClickAsync(_locators.OKButton);
                        await WaitHelper.WaitForElementToDisappearAsync(_locators.PacsInfoConfigAlert, 3000);
                        Logger.Info("Successfully closed dashboard alert");
                    }
                    else
                    {
                        Logger.Warn("OK button not visible in alert");
                        await TakeScreenshotAsync("alert_ok_button_missing");
                    }
                }
                else
                {
                    Logger.Debug("No dashboard alert present");
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to handle dashboard alert", ex);
                await TakeScreenshotAsync("dashboard_alert_failure");
                // Don't throw - alert handling is not critical
            }
        }

        /// <summary>
        /// Navigates to dashboard by clicking NABARD logo
        /// </summary>
        public async Task NavigateToDashboardAsync()
        {
            try
            {
                Logger.Info("Navigating to dashboard");
                
                var isNabardLogoVisible = await WaitHelper.WaitForElementVisibleAsync(_locators.NabardLogo, 5000);
                if (!isNabardLogoVisible)
                {
                    Logger.Warn("NABARD logo not visible");
                    await TakeScreenshotAsync("nabard_logo_not_visible");
                    return;
                }

                var isNabardLogoClickable = await WaitHelper.WaitForElementClickableAsync(_locators.NabardLogo, 2000);
                if (isNabardLogoClickable)
                {
                    await ClickAsync(_locators.NabardLogo);
                    await WaitHelper.WaitForPageLoadAsync();
                    Logger.Info("Successfully navigated to dashboard");
                }
                else
                {
                    Logger.Warn("NABARD logo not clickable");
                    await TakeScreenshotAsync("nabard_logo_not_clickable");
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to navigate to dashboard", ex);
                await TakeScreenshotAsync("navigate_dashboard_failure");
                throw;
            }
        }
        /// <summary>
        /// Navigates to Borrowings module
        /// </summary>
        public async Task NavigateToBorrowingsAsync()
        {
            try
            {
                Logger.Info("Navigating to Borrowings module");
                
                var isPageLoaded = await WaitHelper.WaitForPageLoadAsync();
                if (!isPageLoaded)
                {
                    Logger.Warn("Page did not load completely before navigating to Borrowings");
                }

                var isBorrowingsIconClickable = await WaitHelper.WaitForElementClickableAsync(_locators.BorrowingsIcon, 5000);
                if (isBorrowingsIconClickable)
                {
                    await ClickAsync(_locators.BorrowingsIcon);
                    await WaitHelper.WaitForPageLoadAsync();
                    Logger.Info("Successfully navigated to Borrowings module");
                }
                else
                {
                    Logger.Error("Borrowings icon not clickable");
                    await TakeScreenshotAsync("borrowings_icon_not_clickable");
                    throw new InvalidOperationException("Borrowings icon is not clickable");
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to navigate to Borrowings module", ex);
                await TakeScreenshotAsync("navigate_borrowings_failure");
                throw;
            }
        }
        public async Task NavigateToMemebershipAsync()
        {
            try
            {
                Logger.Info("Naviagte to Membership");
                var isPageLoaded = await WaitHelper.WaitForPageLoadAsync();
                if (!isPageLoaded)
                {
                    Logger.Warn("Page did not load completely before navigating to Membership");
                }
                var isMembershipIconClickable = await WaitHelper.WaitForElementClickableAsync(_locators.MembershipIcon, 5000);
                if (isMembershipIconClickable)
                {
                    await ClickAsync(_locators.MembershipIcon);
                    await WaitHelper.WaitForPageLoadAsync();
                    Logger.Info("Successfully navigated to Membership module");
                }
                else
                {
                    Logger.Error("Membership icon not clickable");
                    await TakeScreenshotAsync("membership_icon_not_clickable");
                    throw new InvalidOperationException("Membership icon is not clickable");
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to navigate to Membership module", ex);
                await TakeScreenshotAsync("navigate_membership_failure");
                throw;
            }
        }
        public async Task NaviagateToLoansAsync()
        {
            try
            {
                Logger.Info("Naviagte to Loans");
                var isPageLoaded = await WaitHelper.WaitForPageLoadAsync();
                if (!isPageLoaded)
                {
                    Logger.Warn("Page did not load completely before navigating to Membership");
                }
                var isLoansIconClickable = await WaitHelper.WaitForElementClickableAsync(_locators.LoansIcon, 5000);
                if (isLoansIconClickable)
                {
                    await ClickAsync(_locators.LoansIcon);
                    await WaitHelper.WaitForPageLoadAsync();
                    Logger.Info("Successfully navigated to Loans module");
                }
                else
                {
                    Logger.Error("Loans icon not clickable");
                    await TakeScreenshotAsync("Loans_icon_not_clickable");
                    throw new InvalidOperationException("Loans icon is not clickable");
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to navigate to Loans module", ex);
                await TakeScreenshotAsync("navigate_membership_failure");
                throw;
            }
        }
        public async Task ClickConfigurationMenuAsync()
        {
            try
            {
                await ClickAsync(_locators.ConfigurationMenu);
                Logger.Debug($"Click Configuration Menu");
            }
            catch (Exception ex)
            {
                Logger.Error($"{ex.Message}");
            }
        }

        /// <summary>
        /// Validates that the Dashboard page has loaded correctly
        /// Checks for the presence of key dashboard elements
        /// Uses helper method to validate multiple elements for robustness
        /// </summary>
        protected override async Task<bool> ValidatePageLoadedAsync()
        {
            try
            {
                // Define key elements that indicate dashboard is loaded
                // NABARD logo is the primary indicator, but we also check for Borrowings icon
                var keyElements = new[]
                {
                    _locators.NabardLogo,
                    _locators.BorrowingsIcon
                };

                // Use helper method - at least one element must be visible (flexible validation)
                // This handles cases where some elements might not be immediately visible
                var isValid = await ValidateAnyElementVisibleAsync(keyElements, timeout: 5000);
                
                if (isValid)
                {
                    Logger.Debug("Dashboard page loaded successfully - key elements are visible");
                    return true;
                }
                
                Logger.Warn("Dashboard page validation failed - key elements are not visible");
                return false;
            }
            catch (Exception ex)
            {
                Logger.Warn($"Dashboard page validation error: {ex.Message}");
                return false;
            }
        }
    }
}
