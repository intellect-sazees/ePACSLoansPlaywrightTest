using IntellectPlaywrightTest.Core;
using IntellectPlaywrightTest.Core.Interfaces;
using IntellectPlaywrightTest.Modules.Borrowings.Components;
using IntellectPlaywrightTest.Modules.Dashboard;
using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntellectPlaywrightTest.Models;

namespace IntellectPlaywrightTest.Modules.Borrowings
{
    public class DrawalsPage:BasePage
    {
        private readonly ITestDataProvider _testDataProvider;
        private readonly DrawalsLocators _locators;
        private readonly DrawalsFormComponent _formComponent;
        public DrawalsPage(IPage page,IWaitHelper waitHelper,NLog.ILogger logger,IRetryHelper retryHelper,IInputValidationHelper inputHelper,ITestDataProvider testDataProvider,Core.Interfaces.ILocatorLoader locatorLoader): base(page, waitHelper, logger, retryHelper)
        {
            _testDataProvider = testDataProvider ?? throw new ArgumentNullException(nameof(testDataProvider));
            _locators = locatorLoader?.LoadLocators<DrawalsLocators>("DrawalsPage", MapPropertyToKey)?? throw new ArgumentNullException(nameof(locatorLoader));
            _formComponent = new DrawalsFormComponent(page,waitHelper,logger,retryHelper,inputHelper ?? throw new ArgumentNullException(nameof(inputHelper)),_locators);
        }
        private static string MapPropertyToKey(string propertyName)
        {
            return propertyName switch
            {
                nameof(DrawalsLocators.DrawalsIcon) => "DrawalsIcon",
                nameof(DrawalsLocators.Product)=> "Product",
                nameof(DrawalsLocators.AccountNumberInput)=> "AccountNumberInput",
                nameof(DrawalsLocators.SearchIcon)=> "SearchIcon",
                nameof(DrawalsLocators.DrawalAmount)=> "DrawalAmount",
                nameof(DrawalsLocators.VoucherType)=> "VoucherType",
                nameof(DrawalsLocators.SaveBtn)=> "SaveBtn",
                _ => propertyName 
            };
        }
        public async Task CaptureBorrowingDrawalsAsync(DashboardPage dashboardPage,DrawalsData data)
        {
            try
            {
                Logger.Info("Starting Drawals Transation process");

                await NavigateToDrawalsAsync(dashboardPage);

                await _formComponent.FillAsync(data);

                await SaveDrawalsTransationAsync();

                Logger.Info("Drawals Transation process completed");
            }
            catch (Exception ex)
            {
                Logger.Error("Drawals Tranaction failed", ex);
                await TakeScreenshotAsync("drawals_transtion_failure");
                throw;
            }
        }

        private async Task NavigateToDrawalsAsync(DashboardPage dashboardPage)
        {
            try
            {
                Logger.Debug("Navigating to Drawals page");
                await dashboardPage.HandleDashboardAlertAsync();
                await dashboardPage.NavigateToBorrowingsAsync();
                await ClickAsync(_locators.DrawalsIcon);
                await WaitHelper.WaitForPageLoadAsync();
                Logger.Debug("Successfully navigated to Drawals page");
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to navigate to Drawals page", ex);
                throw;
            }
        }
        private async Task SaveDrawalsTransationAsync()
        {
            try
            {
                Logger.Info("Saving Drawals Transation form");
                await ClickAsync(_locators.SaveBtn);
                await WaitHelper.WaitForTimeoutAsync(1000);

                //// Handle save alert if present
                //var isAlertVisible = await IsVisibleAsync(_locators.SaveAlert);
                //if (isAlertVisible)
                //{
                //    Logger.Debug("Save alert detected, clicking OK");
                //    await ClickAsync(_locators.OKButton);
                //    await WaitHelper.WaitForElementToDisappearAsync(_locators.SaveAlert, 3000);
                //    await ClickAsync(_locators.CloseTask);
                //}

                Logger.Info("Drawals Transaction saved successfully");
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to save Drawals Transaction", ex);
                await TakeScreenshotAsync("Drawals_Transaction_failure");
                throw;
            }
        }

    }
    
    
}
