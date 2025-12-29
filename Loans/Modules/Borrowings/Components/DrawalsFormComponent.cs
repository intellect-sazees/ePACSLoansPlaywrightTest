using IntellectPlaywrightTest.Core.Components;
using IntellectPlaywrightTest.Core.Interfaces;
using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntellectPlaywrightTest.Models;

namespace IntellectPlaywrightTest.Modules.Borrowings.Components
{
    public class DrawalsFormComponent :BaseFormComponent
    {
        private readonly IInputValidationHelper _inputHelper;
        private readonly DrawalsLocators _locators;
        public DrawalsFormComponent(IPage page,IWaitHelper waitHelper,NLog.ILogger logger,IRetryHelper retryHelper,IInputValidationHelper inputHelper,DrawalsLocators locators): base(page, waitHelper, logger, retryHelper)
        {
            _inputHelper = inputHelper ?? throw new ArgumentNullException(nameof(inputHelper));
            _locators = locators ?? throw new ArgumentNullException(nameof(locators));
        }
        public override async Task FillAsync<T>(T data)
        {
            if (data is not DrawalsData drawalsData)
            {
                throw new ArgumentException($"Expected DrawalsData, got {typeof(T).Name}", nameof(data));
            }
            try
            {
                Logger.Info("Starting to fill Drawal Transaction form");
                await FillProductAsync(drawalsData.Product);
                await FillAccountNumberAsync(drawalsData.AccountNumber);
                await SearchAccountAsync();
                await FillDrawalAmountAsync(drawalsData.DrawalAmount);
                await FillVoucherTypeAsync(drawalsData.VoucherType);
                Logger.Info("Account creation form filled successfully");
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to fill account creation form", ex);
                throw;
            }
        }
        private async Task FillProductAsync(string product)
        {
            await _inputHelper.SelectDropdownOptionAsync(Page.Locator(_locators.Product),product,SelectBy.Label);
            Logger.Debug($"Selected product: {product}");
        }
        private async Task FillAccountNumberAsync(string accountNumber)
        {
            var input = Page.Locator(_locators.AccountNumberInput);
            var filled = await _inputHelper.FillTextBoxValueAsync(input, accountNumber);
            if (!filled)
            {
                throw new InvalidOperationException($"Failed to fill Account No: {accountNumber}");
            }
            Logger.Debug($"Filled Account No: {accountNumber}");
        }
        private async Task SearchAccountAsync()
        {
            try
            {
                await Page.ClickAsync(_locators.SearchIcon);
                Logger.Debug("Searchicon is clickable ");
            }
            catch (Exception ex)
            {
                Logger.Error("Searchicon is not clcikable");
            }
        }
        private async Task FillDrawalAmountAsync(string drawalAmount)
        {
            try
            {
                var input = Page.Locator(_locators.DrawalAmount);
                var filled = await _inputHelper.FillTextBoxValueAsync(input, drawalAmount);
                if (!filled)
                {
                    throw new InvalidCastException($"Faild to fill Drawal Amount :{drawalAmount}");
                }
                Logger.Debug($"Filled Drawal Amount :{drawalAmount}");
            }
            catch (Exception ex)
            {
                Logger.Error($"Error While Filling Drawal Amount :{ex.Message}");
            }
        }
        private async Task FillVoucherTypeAsync(string vouchertype)
        {
            try
            {
                await _inputHelper.SelectDropdownOptionAsync(Page.Locator(_locators.VoucherType), vouchertype, SelectBy.Label);
                Logger.Debug($"Selected Voucher type: {vouchertype}");
            }
            catch (Exception ex)
            {
                Logger.Error($"Error during voucher type selection{ex.Message}");
            }
        }
    }
    
}
