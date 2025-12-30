using ePACSLoans.Core;
using ePACSLoans.Core.Interfaces;
using ePACSLoans.Models.Data;
using ePACSLoans.Models.Locaters;
using ePACSLoans.Modules.Dashboard;
using ePACSLoans.Modules.Loans.Components;
using ePACSLoans.Utilities.Helpers;
using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePACSLoans.Modules.Loans
{
    public class CreditLimitPage : BasePage
    {
        private readonly ITestDataProvider _testDataProvider;
        private readonly CreditLimitLocators _locators;
        private readonly CreditLimitFormComponents _formComponent;
        public CreditLimitPage(IPage page, IWaitHelper waitHelper, NLog.ILogger logger, IRetryHelper retryHelper, IInputValidationHelper inputHelper, ITestDataProvider testDataProvider, ILocatorLoader locatorLoader) : base(page, waitHelper, logger, retryHelper)
        {
            _testDataProvider = testDataProvider ?? throw new ArgumentNullException(nameof(testDataProvider));
            // Load locators using LocatorLoader service
            _locators = locatorLoader?.LoadLocators<CreditLimitLocators>("CreditLimitPage", MapPropertyToKey) ?? throw new ArgumentNullException(nameof(locatorLoader));
            // Initialize form component with dependencies
            _formComponent = new CreditLimitFormComponents(page, waitHelper, logger, retryHelper, inputHelper ?? throw new ArgumentNullException(nameof(inputHelper)), _locators);
        }
        private static string MapPropertyToKey(string propertyName)
        {
            return propertyName switch
            {
                nameof(CreditLimitLocators.CreditLimitIcon)=> "CreditLimitIcon",
                nameof(CreditLimitLocators.VillageInput)=> "Village",
                nameof(CreditLimitLocators.ProductInput)=> "Product",
                nameof(CreditLimitLocators.ViewBtnInput)=> "ViewBtn",
                nameof(CreditLimitLocators.SelectInput)=> "SelectRecord",
                nameof(CreditLimitLocators.SaveBtn)=> "SaveBtn",
                nameof(CreditLimitLocators.SaveAlertinput)=> "SavePopup",
                nameof(CreditLimitLocators.OkbtnInput)=> "OkButton",
                _ => propertyName // Default: use property name as-is
            };
        }
        public async Task SaveCreditlimitAsync(DashboardPage dashboardPage, CreditLimitData data,CreditLimitLocators locators)
        {
            await NavigateToCreditLimitAsync(dashboardPage);
            await _formComponent.FillAsync(data, locators);
            var modalBody = Page.Locator(_locators.SaveAlertinput);
            await modalBody.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            string actualMessage = (await modalBody.InnerTextAsync()).Trim();
            AssertHelper.AssertTextContains(actualMessage, "Data saved successfully", "Success Popup Message");
            await Page.Locator(_locators.OkbtnInput).ClickAsync();
            await dashboardPage.NavigateToDashboardAsync();
        }
        private async Task NavigateToCreditLimitAsync(DashboardPage dashboardPage)
        {
            try
            {
                Logger.Debug("Navigating to  Credit Limit page");
                await dashboardPage.HandleDashboardAlertAsync();
                await dashboardPage.NaviagateToLoansAsync();
                await ClickAsync(_locators.CreditLimitIcon);
                await WaitHelper.WaitForPageLoadAsync();
                Logger.Debug("Successfully navigated to  Credit limit page");
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to navigate to  Land Declaration page :{ex}");
                throw;
            }
        }
        private async Task CreditLimitCalculationAsync(string admissionNo,string crop,string kharifAcomp,string kharifBcomp,string rabiAcomp,string rabiBcomp)
        {
            /*
             * 1.Convert Total land into least subunits
             * 2.Get Scale of finace Values Both Kharif and Rabi
             * 3.
             * */
        }
    }
}
