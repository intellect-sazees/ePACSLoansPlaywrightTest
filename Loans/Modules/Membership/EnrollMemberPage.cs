using IntellectPlaywrightTest.Core;
using IntellectPlaywrightTest.Core.Interfaces;
using IntellectPlaywrightTest.Models;
using IntellectPlaywrightTest.Modules.Dashboard;
using IntellectPlaywrightTest.Modules.Membership.Components;
using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntellectPlaywrightTest.Modules.Membership
{
    public class EnrollMemberPage :BasePage
    {
        private readonly ITestDataProvider _testDataProvider;
        private readonly ShareDepositLocaters _locators;
        ShareDepositFormComponent _formComponent;
        public EnrollMemberPage(IPage page, IWaitHelper waitHelper, NLog.ILogger logger, IRetryHelper retryHelper, IInputValidationHelper inputHelper, ITestDataProvider testDataProvider, ILocatorLoader locatorLoader) : base(page, waitHelper, logger, retryHelper)
        {
            _testDataProvider = testDataProvider ?? throw new ArgumentNullException(nameof(testDataProvider));
            _locators = locatorLoader?.LoadLocators<ShareDepositLocaters>("ShareDepositPage", MapPropertyToKey) ?? throw new ArgumentNullException(nameof(locatorLoader));
            _formComponent = new ShareDepositFormComponent(page, waitHelper, logger, retryHelper, inputHelper ?? throw new ArgumentNullException(nameof(inputHelper)), _locators);
        }
        private static string MapPropertyToKey(string propertyName)
        {
            return propertyName switch
            {
                //nameof(ShareDepositLocaters.SharedepositIcon) => "SharedepositIcon",
                //nameof(ShareDepositLocaters.AdmissionNo) => "admissionNo",
                //nameof(ShareDepositLocaters.SearchIcon) => "searchicon",
                //nameof(ShareDepositLocaters.Product) => "product",
                //nameof(ShareDepositLocaters.AccountNo) => "accountNo",
                //nameof(ShareDepositLocaters.ActivityType) => "activityType",
                //nameof(ShareDepositLocaters.VoucherType) => "voucherType",
                //nameof(ShareDepositLocaters.AmountInput) => "Amount",
                //nameof(ShareDepositLocaters.Savebtn) => "SaveBTN",
                //nameof(ShareDepositLocaters.LastTransaction) => "LastTransactionPopu",
                //nameof(ShareDepositLocaters.YesBtn) => "YesBtn",
                //nameof(ShareDepositLocaters.SaveAlert) => "SaveAlert",
                //nameof(ShareDepositLocaters.Okbtn) => "Okbtn",
                //nameof(ShareDepositLocaters.PrintVoucher) => "PrintVoucher",
                //nameof(ShareDepositLocaters.AcceptPrint) => "AcceptPrint",
                //nameof(ShareDepositLocaters.Showparameters) => "viewparameters",
                //nameof(ShareDepositLocaters.AtypeMaxAmount) => "MaximumAmountForAtype",
                //nameof(ShareDepositLocaters.AtypeMinAmount) => "MinimumAmountForAtype",
                //nameof(ShareDepositLocaters.BtypeMaxAmount) => "MaximumAmountForBtype",
                //nameof(ShareDepositLocaters.BtypeMinAmount) => "MinimumAmountForBtype",
                //nameof(ShareDepositLocaters.CloseShowparameters) => "closeviewparameters",
                //nameof(ShareDepositLocaters.showsharedepositparametertbl) => "parameterstbl",
                //nameof(ShareDepositLocaters.Particularstbl) => "Particularstbl",
                _ => propertyName // Default: use property name as-is
            };
        }
        private async Task NavigateToEnrollMemberAsync(DashboardPage dashboardPage)
        {
            try
            {
                Logger.Debug("Navigating to Enroll Member page");
                await WaitHelper.WaitForPageLoadAsync();
                await dashboardPage.HandleDashboardAlertAsync();
                await WaitHelper.WaitForPageLoadAsync();
                await dashboardPage.NavigateToMemebershipAsync();
                await ClickAsync(_locators.SharedepositIcon);
                await WaitHelper.WaitForPageLoadAsync();
                Logger.Debug("Successfully navigated to share deposit page");
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to Navigate to Share deposit :{ex.Message}");
                throw;
            }
        }
    }
}
