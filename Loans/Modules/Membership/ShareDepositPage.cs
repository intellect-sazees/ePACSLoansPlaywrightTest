using IntellectPlaywrightTest.Core;
using IntellectPlaywrightTest.Core.Interfaces;
using IntellectPlaywrightTest.Models;
using IntellectPlaywrightTest.Modules.Dashboard;
using IntellectPlaywrightTest.Modules.Membership.Components;
using Microsoft.Playwright;



namespace IntellectPlaywrightTest.Modules.Membership
{
    public class ShareDepositPage :BasePage
    {
        private readonly ITestDataProvider _testDataProvider;
        private readonly ShareDepositLocaters _locators;
        ShareDepositFormComponent _formComponent;
        public ShareDepositPage(IPage page, IWaitHelper waitHelper, NLog.ILogger logger, IRetryHelper retryHelper, IInputValidationHelper inputHelper, ITestDataProvider testDataProvider, ILocatorLoader locatorLoader) : base(page, waitHelper, logger, retryHelper)
        {
            _testDataProvider = testDataProvider ?? throw new ArgumentNullException(nameof(testDataProvider));
            _locators = locatorLoader?.LoadLocators<ShareDepositLocaters>("ShareDepositPage", MapPropertyToKey)?? throw new ArgumentNullException(nameof(locatorLoader));
            _formComponent = new ShareDepositFormComponent(page, waitHelper, logger, retryHelper, inputHelper ?? throw new ArgumentNullException(nameof(inputHelper)), _locators);
        }

        private static string MapPropertyToKey(string propertyName)
        {
            return propertyName switch
            {
                nameof(ShareDepositLocaters.SharedepositIcon) => "SharedepositIcon",
                nameof(ShareDepositLocaters.AdmissionNo)=> "admissionNo",
                nameof(ShareDepositLocaters.SearchIcon)=> "searchicon",
                nameof(ShareDepositLocaters.Product)=> "product",
                nameof(ShareDepositLocaters.AccountNo)=> "accountNo",
                nameof(ShareDepositLocaters.ActivityType)=> "activityType",
                nameof(ShareDepositLocaters.VoucherType)=> "voucherType",
                nameof(ShareDepositLocaters.AmountInput)=> "Amount",
                nameof(ShareDepositLocaters.Savebtn)=> "SaveBTN",
                nameof(ShareDepositLocaters.LastTransaction)=> "LastTransactionPopu",
                nameof(ShareDepositLocaters.YesBtn)=> "YesBtn",
                nameof(ShareDepositLocaters.SaveAlert)=> "SaveAlert",
                nameof(ShareDepositLocaters.Okbtn)=> "Okbtn",
                nameof(ShareDepositLocaters.PrintVoucher)=> "PrintVoucher",
                nameof(ShareDepositLocaters.AcceptPrint)=> "AcceptPrint",
                nameof(ShareDepositLocaters.Showparameters)=> "viewparameters",
                nameof(ShareDepositLocaters.AtypeMaxAmount)=> "MaximumAmountForAtype",
                nameof(ShareDepositLocaters.AtypeMinAmount)=> "MinimumAmountForAtype",
                nameof(ShareDepositLocaters.BtypeMaxAmount)=> "MaximumAmountForBtype",
                nameof(ShareDepositLocaters.BtypeMinAmount)=> "MinimumAmountForBtype",
                nameof(ShareDepositLocaters.CloseShowparameters)=> "closeviewparameters",
                nameof(ShareDepositLocaters.showsharedepositparametertbl)=> "parameterstbl",
                nameof(ShareDepositLocaters.Particularstbl)=> "Particularstbl",
                _ => propertyName // Default: use property name as-is
            };
        }
        public async Task ShareDepositTranactionAsync(DashboardPage dashboardPage,ShareDepositData data)
        {
            try
            {
                Logger.Info("Starting share withdrawal process");
                await NavigateToSharedepositAsync(dashboardPage);
                await _formComponent.FillAsync(data);
                await _formComponent.ClickSaveAsync();
                await _formComponent.ClickSaveAsync();
                await _formComponent.CheckLastTranstionAsync();
                await _formComponent.VerifyValidSaveAlertAsync();
                await _formComponent.PrintVoucherAsync();
                await dashboardPage.NavigateToDashboardAsync();
                Logger.Info("Share Deposit process completed");
            }
            catch (Exception ex)
            {
                Logger.Error($"Share deposit failed:{ex.Message}");
                await TakeScreenshotAsync("share_deposit_failure");
                throw;
            }
        }
        public async Task ShareDepositAmonutValidationAsync(DashboardPage dashboardPage, ShareDepositData data)
        {
            try
            {
                Logger.Info("Starting share withdrawal process");
                await NavigateToSharedepositAsync(dashboardPage);
                var (maxShareCapClassA,minShareCapClassA,maxShareCapClassB,minShareCapClassB)=await _formComponent.GetAuthorizedShareAmountAsync();
                await _formComponent.FillAsync(data);
                var admissionNo = await _formComponent.GetProcessedAdmissionDigit();
                if (admissionNo == 1)
                {
                    string temp=await _formComponent.Getcustomersahrebalance();
                    decimal shareBalance=Convert.ToDecimal(temp);
                    //maxShareCapClassA+
                }
                else if (admissionNo == 2)
                {

                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Share deposit amount validation failed:{ex.Message}");
                await TakeScreenshotAsync("share_deposit_amount_validationfailure");
                throw;
            }
        }
        private async Task NavigateToSharedepositAsync(DashboardPage dashboardPage)
        {
            try
            {
                Logger.Debug("Navigating to Share Deposit page");
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
