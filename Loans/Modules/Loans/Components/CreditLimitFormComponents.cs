using ePACSLoans.Core.Components;
using ePACSLoans.Core.Interfaces;
using ePACSLoans.Models.Data;
using ePACSLoans.Models.Locaters;
using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePACSLoans.Modules.Loans.Components
{
    public class CreditLimitFormComponents : BaseFormComponent<CreditLimitData,CreditLimitLocators>
    {
        private IPage page;
        private IWaitHelper waitHelper;
        private NLog.ILogger logger;
        private IRetryHelper retryHelper;
        private IInputValidationHelper _inputHelper;
        private CreditLimitLocators _locators;
        public CreditLimitFormComponents(IPage page, IWaitHelper waitHelper, NLog.ILogger logger, IRetryHelper retryHelper, IInputValidationHelper inputHelper, CreditLimitLocators locators) : base(page, waitHelper, logger, retryHelper)
        {
            _inputHelper = inputHelper ?? throw new ArgumentNullException(nameof(inputHelper));
            _locators = locators ?? throw new ArgumentNullException(nameof(locators));
        }
        public override async Task FillAsync(CreditLimitData creditlimitData, CreditLimitLocators creditlimitlocater)
        {
            try
            {
                Logger.Info("Starting to fill Credit Limit form");
                await FillVillageAsync(creditlimitData.Village);
                await FillProductAsync(creditlimitData.Product);
                await ClickViewBtnAsync();
                await SelectRecordAsync();
                Logger.Info("Credit Limit form filled successfully");
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to fill Credit limit form :{ex}");
                throw;
            }
        }
        private async Task FillVillageAsync(string village)
        {
            await _inputHelper.SelectDropdownOptionAsync(Page.Locator(_locators.VillageInput), village,SelectBy.Label);
            Logger.Debug($"Selected Village: {village}");
        }
        private async Task FillProductAsync(string product)
        {
            await _inputHelper.SelectDropdownOptionAsync(Page.Locator(_locators.ProductInput), product, SelectBy.Label);
            Logger.Debug($"Product {product}");
        }
        private async Task ClickViewBtnAsync()
        {
            await Page.Locator(_locators.ViewBtnInput).ClickAsync();
        }
        private async Task SelectRecordAsync()
        {
            await Page.Locator(_locators.SelectInput).ClickAsync();
        }
    }
}
