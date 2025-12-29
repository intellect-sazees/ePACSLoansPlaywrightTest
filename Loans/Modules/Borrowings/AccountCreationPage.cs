using IntellectPlaywrightTest.Core;
using IntellectPlaywrightTest.Core.Interfaces;
using IntellectPlaywrightTest.Modules.Borrowings.Components;
using IntellectPlaywrightTest.Modules.Dashboard;
using IntellectPlaywrightTest.Utilities.DataManagement;
using IntellectPlaywrightTest.Utilities.Helpers;
using IntellectPlaywrightTest.Models;
using Microsoft.Playwright;

namespace IntellectPlaywrightTest.Modules.Borrowings
{
    /// <summary>
    /// Page Object Model for Account Creation page in Borrowings module
    /// Inherits from BasePage and uses dependency injection
    /// Uses Component-Based POM pattern with AccountFormComponent
    /// </summary>
    public class AccountCreationPage : BasePage
    {
        private readonly ITestDataProvider _testDataProvider;
        private readonly AccountCreationLocators _locators;
        private readonly AccountFormComponent _formComponent;

        /// <summary>
        /// Initializes a new instance of AccountCreationPage with dependency injection
        /// </summary>
        public AccountCreationPage(
            IPage page,
            IWaitHelper waitHelper,
           NLog.ILogger logger,
            IRetryHelper retryHelper,
            IInputValidationHelper inputHelper,
            ITestDataProvider testDataProvider,
            Core.Interfaces.ILocatorLoader locatorLoader)
            : base(page, waitHelper, logger, retryHelper)
        {
            _testDataProvider = testDataProvider ?? throw new ArgumentNullException(nameof(testDataProvider));
            
            // Load locators using LocatorLoader service
            _locators = locatorLoader?.LoadLocators<AccountCreationLocators>("AccountCreationPage", MapPropertyToKey)
                ?? throw new ArgumentNullException(nameof(locatorLoader));
            
            // Initialize form component with dependencies
            _formComponent = new AccountFormComponent(
                page,
                waitHelper,
                logger,
                retryHelper,
                inputHelper ?? throw new ArgumentNullException(nameof(inputHelper)),
                _locators);
        }

        /// <summary>
        /// Maps property names to JSON keys for AccountCreationPage locators
        /// </summary>
        private static string MapPropertyToKey(string propertyName)
        {
            return propertyName switch
            {
                nameof(AccountCreationLocators.AccountCreationIcon) => "accountCreationIcon",
                nameof(AccountCreationLocators.ProductList) => "productList",
                nameof(AccountCreationLocators.PurposeList) => "purpusList",
                nameof(AccountCreationLocators.SocietyLoanNo) => "socityLoanNo",
                nameof(AccountCreationLocators.AppliedDate) => "appliedDate",
                nameof(AccountCreationLocators.AppliedAmount) => "appliedAmount",
                nameof(AccountCreationLocators.SanctionDate) => "sanctionDate",
                nameof(AccountCreationLocators.SanctionAmount) => "sanctionAmount",
                nameof(AccountCreationLocators.RepaymentType) => "repaymentType",
                nameof(AccountCreationLocators.RepaymentMode) => "repaymentMode",
                nameof(AccountCreationLocators.ROI) => "roi",
                nameof(AccountCreationLocators.PenalROI) => "penalRoi",
                nameof(AccountCreationLocators.IOAROI) => "IOARoi",
                nameof(AccountCreationLocators.GestationPeriodMonths) => "gestationPeriodMonths",
                nameof(AccountCreationLocators.LoanPeriodMonths) => "loanPeriodMonths",
                nameof(AccountCreationLocators.SaveButton) => "saveBTN",
                nameof(AccountCreationLocators.SaveAlert) => "SaveAlert",
                nameof(AccountCreationLocators.OKButton) => "OK",
                nameof(AccountCreationLocators.CloseTask)=> "CloseTaskPopup",
                _ => propertyName // Default: use property name as-is
            };
        }

        /// <summary>
        /// Navigates to account creation page and fills the form
        /// Uses Component-Based POM pattern - delegates form filling to AccountFormComponent
        /// </summary>
        public async Task CreateBorrowingAccountAsync(
            DashboardPage dashboardPage,
            AccountCreationData data)
        {
            try
            {
                Logger.Info("Starting account creation process");

                // Navigate to account creation
                await NavigateToAccountCreationAsync(dashboardPage);

                // Fill all form fields using form component
                await _formComponent.FillAsync(data);

                // Save the form
                await SaveAccountAsync();

                Logger.Info("Account creation process completed");
            }
            catch (Exception ex)
            {
                Logger.Error("Account creation failed", ex);
                await TakeScreenshotAsync("account_creation_failure");
                throw;
            }
        }

        /// <summary>
        /// Navigates to account creation page
        /// </summary>
        private async Task NavigateToAccountCreationAsync(DashboardPage dashboardPage)
        {
            try
            {
                Logger.Debug("Navigating to account creation page");
                await dashboardPage.HandleDashboardAlertAsync();
                await dashboardPage.NavigateToBorrowingsAsync();
                await ClickAsync(_locators.AccountCreationIcon);
                await WaitHelper.WaitForPageLoadAsync();
                Logger.Debug("Successfully navigated to account creation page");
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to navigate to account creation page", ex);
                throw;
            }
        }

        // Form filling methods have been moved to AccountFormComponent
        // This follows Component-Based POM pattern for better organization and reusability

        /// <summary>
        /// Saves the account creation form
        /// </summary>
        private async Task SaveAccountAsync()
        {
            try
            {
                Logger.Info("Saving account creation form");
                await ClickAsync(_locators.SaveButton);
                await ClickAsync(_locators.SaveButton);
                await WaitHelper.WaitForTimeoutAsync(1000);
                
                // Handle save alert if present
                var isAlertVisible = await IsVisibleAsync(_locators.SaveAlert);
                if (isAlertVisible)
                {
                    Logger.Debug("Save alert detected, clicking OK");
                    await ClickAsync(_locators.OKButton);
                    await WaitHelper.WaitForElementToDisappearAsync(_locators.SaveAlert, 3000);
                    await ClickAsync(_locators.CloseTask);
                }

                Logger.Info("Account creation form saved successfully");
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to save account creation form", ex);
                await TakeScreenshotAsync("save_account_failure");
                throw;
            }
        }

        /// <summary>
        /// Validates that the Account Creation page has loaded correctly
        /// Checks for the presence of key account creation page elements
        /// Uses helper method to validate multiple elements for robustness
        /// </summary>
        protected override async Task<bool> ValidatePageLoadedAsync()
        {
            try
            {
                // Define key elements that must be visible for account creation page to be considered loaded
                // Product and Purpose dropdowns are critical, Save button confirms form is ready
                var requiredElements = new[]
                {
                    _locators.ProductList,
                    _locators.PurposeList,
                    _locators.SaveButton
                };

                // Use helper method to validate all required elements are visible
                var isValid = await ValidateElementsVisibleAsync(requiredElements, timeout: 5000, requireAll: true);
                
                if (isValid)
                {
                    Logger.Debug("Account Creation page loaded successfully - all required elements are visible");
                    return true;
                }
                
                Logger.Warn("Account Creation page validation failed - one or more required elements are not visible");
                return false;
            }
            catch (Exception ex)
            {
                Logger.Warn($"Account Creation page validation error: {ex.Message}");
                return false;
            }
        }
    }
}