using IntellectPlaywrightTest.Core;
using IntellectPlaywrightTest.Core.Interfaces;
using IntellectPlaywrightTest.Models;
using IntellectPlaywrightTest.Modules.Dashboard;
using IntellectPlaywrightTest.Modules.Membership.Components;
using Microsoft.Playwright;

namespace IntellectPlaywrightTest.Modules.Membership
{
    public class ShareWithdrawalPage : BasePage
    {
        private readonly ITestDataProvider _testDataProvider;
        private readonly SharewithdrawalLocators _locators;
        ShareWithdrawalFormComponent _formComponent;
        public ShareWithdrawalPage(IPage page, IWaitHelper waitHelper, NLog.ILogger logger, IRetryHelper retryHelper, IInputValidationHelper inputHelper, ITestDataProvider testDataProvider, ILocatorLoader locatorLoader) : base(page, waitHelper, logger, retryHelper)
        {
            _testDataProvider = testDataProvider ?? throw new ArgumentNullException(nameof(testDataProvider));
            _locators = locatorLoader?.LoadLocators<SharewithdrawalLocators>("ShareWithdrawalPage", MapPropertyToKey) ?? throw new ArgumentNullException(nameof(locatorLoader));
            _formComponent = new ShareWithdrawalFormComponent(page, waitHelper, logger, retryHelper, inputHelper ?? throw new ArgumentNullException(nameof(inputHelper)),_locators);
        }
        private static string MapPropertyToKey(string propertyName)
        {
            return propertyName switch
            {
                nameof(SharewithdrawalLocators.SharewhithdrawalIcon)=> "SharewithdrawalIcon",
                nameof(SharewithdrawalLocators.AdmissionNo)=> "admissionNo",
                nameof(SharewithdrawalLocators.SearchIcon)=> "searchicon",
                nameof(SharewithdrawalLocators.Product)=> "product",
                nameof(SharewithdrawalLocators.AccountNo) => "accountNo",
                nameof(SharewithdrawalLocators.ActivityType) => "activityType",
                nameof(SharewithdrawalLocators.VoucherType) => "voucherType",
                nameof(SharewithdrawalLocators.AmountInput) => "Amount",
                nameof(SharewithdrawalLocators.Savebtn) => "SaveBTN",
                nameof(SharewithdrawalLocators.LastTransaction) => "LastTransactionPopu",
                nameof(SharewithdrawalLocators.YesBtn) => "YesBtn",
                nameof(SharewithdrawalLocators.Okbtn) => "Okbtn",
                nameof(SharewithdrawalLocators.PrintVoucher) => "PrintVoucher",
                nameof(SharewithdrawalLocators.AcceptPrint) => "AcceptPrint",
                _ => propertyName // Default: use property name as-is
            };
        }
        public async Task ShareWithdralTranactionAsync(DashboardPage dashboardPage, SharewithdrawalData data)
        {
            try
            {
                Logger.Info("Starting share withdrawal process");
                await NavigateToShareWithdrawalAsync(dashboardPage);
                await _formComponent.FillAsync(data);
                await dashboardPage.NavigateToDashboardAsync();
                Logger.Info("Share withdrawal process completed");
            }
            catch (Exception ex) 
            {
                Logger.Error($"Share withdrawal failed:{ex.Message}");
                await TakeScreenshotAsync("share_withdrawal_failure");
                throw;
            }
        }
        private async Task NavigateToShareWithdrawalAsync(DashboardPage dashboardPage)
        {
            try
            {
                Logger.Debug("Navigating to Share Withdrawal page");
                await dashboardPage.HandleDashboardAlertAsync();
                await WaitHelper.WaitForPageLoadAsync();
                await dashboardPage.NavigateToMemebershipAsync();
                await ClickAsync(_locators.SharewhithdrawalIcon);
                await WaitHelper.WaitForPageLoadAsync();
                Logger.Debug("Successfully navigated to share Withrawal page");
            }
            catch (Exception ex) 
            {
                Logger.Error($"Failed to navigate to share withdrawal page:{ex.Message}");
                throw;
            }
        }
    }
}
