using IntellectPlaywrightTest.Core.Components;
using IntellectPlaywrightTest.Core.Interfaces;
using IntellectPlaywrightTest.Models;
using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntellectPlaywrightTest.Modules.Membership.Components
{
    public class ShareWithdrawalFormComponent : BaseFormComponent
    {
        private readonly IInputValidationHelper _inputHelper;
        private readonly SharewithdrawalLocators _locators;
        public ShareWithdrawalFormComponent(IPage page, IWaitHelper waitHelper, NLog.ILogger logger, IRetryHelper retryHelper, IInputValidationHelper inputHelper, SharewithdrawalLocators locators) : base(page, waitHelper, logger, retryHelper)
        {
            _inputHelper = inputHelper ?? throw new ArgumentNullException(nameof(inputHelper));
            _locators = locators ?? throw new ArgumentNullException(nameof(locators));
        }
        public override async Task FillAsync<T>(T data)
        {
            if (data is not SharewithdrawalData sharewithdrawalData)
            {
                
                throw new ArgumentException($"Expected ShareWithdrawalData, got {typeof(T).Name}", nameof(data));
            }
            try
            {
                Logger.Info("Starting to fill Share Withdrawal form");
                await FillAdmissionNumberAsync(sharewithdrawalData.AdmissionNo);
                await SearchAdmissionAsync();
                await FillProductAsync(sharewithdrawalData.Product);
                await FillAccountAsync(sharewithdrawalData.AccountNo);
                await FillActivityAsync(sharewithdrawalData.Activity);
                await FillVoucherTypeAsync(sharewithdrawalData.Voucher);
                await FillAmountAsync(sharewithdrawalData.Amount);
                await ClickSaveAsync();
                await ClickSaveAsync();
                await CheckLastTranstionAsync();
                await ClickOkButtonAsync();
                await PrintVoucherAsync();
                Logger.Info("Share withdrawal successfully");
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to fill Share withdrawal form:{ex.Message}");
                throw;
            }
        }
        private async Task FillAdmissionNumberAsync(string admissionnumber)
        {
            var input = Page.Locator(_locators.AdmissionNo);
            var filled = await _inputHelper.FillTextBoxValueAsync(input, admissionnumber);
            if (!filled)
            {
                throw new InvalidOperationException($"Failed to fill Admission No: {admissionnumber}");
            }
            Logger.Debug($"Filled Admission No: {admissionnumber}");
        }
        private async Task SearchAdmissionAsync()
        {
            try
            {
                await Page.ClickAsync(_locators.SearchIcon);
                Logger.Debug("Searchicon is clickable ");
            }
            catch (Exception ex)
            {
                Logger.Error($"Searchicon is not clcikable:{ex.Message}");
            }
        }
        private async Task FillProductAsync(string product)
        {
            try
            {
                await _inputHelper.SelectDropdownOptionAsync(Page.Locator(_locators.Product), product, SelectBy.Label);
                Logger.Debug($"Selected product: {product}");
            }
            catch (Exception ex)
            {
                Logger.Error($"Product is not selected :{ex.Message}");
            }
        }
        private async Task FillAccountAsync(string account)
        {
            try
            {
                await _inputHelper.SelectDropdownOptionAsync(Page.Locator(_locators.AccountNo), account, SelectBy.Index);
                Logger.Debug($"Selected account: {account}");
            }
            catch (Exception ex)
            {
                Logger.Error($"Account is not selected :{ex.Message}");
            }
        }
        private async Task FillActivityAsync(string activity)
        {
            try
            {
                await _inputHelper.SelectDropdownOptionAsync(Page.Locator(_locators.ActivityType), activity, SelectBy.Label);
                Logger.Debug($"Selected Activity:{activity}");
            }
            catch (Exception ex)
            {
                Logger.Error($"Activity is not selected :{ex.Message}");
            }
        }
        private async Task FillVoucherTypeAsync(string voucher)
        {
            try
            {
                await _inputHelper.SelectDropdownOptionAsync(Page.Locator(_locators.VoucherType), voucher, SelectBy.Label);
                Logger.Debug($"Select voucher type: {voucher}");
            }
            catch (Exception ex)
            {
                Logger.Error($"Voucher type is not select {ex.Message}");
            }
        }
        private async Task FillAmountAsync(string amount)
        {
            try
            {
                var input = Page.Locator(_locators.AmountInput);
                var filled = await _inputHelper.FillTextBoxValueAsync(input, amount);
                if (!filled)
                {
                    throw new InvalidOperationException($"Failed to fill Amount No: {amount}");
                }
                Logger.Debug($"Filled Amount No: {amount}");
            }
            catch (Exception ex)
            {
                Logger.Error($"Amount is not fillled:{ex.Message}");
            }
        }
        private async Task ClickSaveAsync()
        {
            try
            {
                await WaitHelper.WaitForElementClickableAsync(_locators.Savebtn);
                await Page.ClickAsync(_locators.Savebtn);
                Logger.Debug($"{_locators.Savebtn} is clickable");
            }
            catch (Exception ex)
            {
                Logger.Error($"Unable to click on save {ex.Message}");
            }
        }
        private async Task CheckLastTranstionAsync()
        {
            try
            {
                bool isVisible = await WaitHelper.WaitForElementVisibleAsync(_locators.LastTransaction);
                if (isVisible)
                {
                    await Page.ClickAsync(_locators.YesBtn);
                }
                Logger.Debug($"Last Transaction is not displayed: {_locators.LastTransaction}");
            }
            catch (Exception ex)
            {
                Logger.Error($"Last Transaction Popup has not been displayed {ex.Message}");
            }
        }
        private async Task ClickOkButtonAsync()
        {
            try
            {
                bool isVisble = await WaitHelper.WaitForElementVisibleAsync(_locators.Okbtn);
                if (isVisble)
                {
                    await Page.ClickAsync(_locators.Okbtn);
                }
                Logger.Debug($"");
            }
            catch (Exception ex)
            {
                Logger.Error($"{ex.Message}");
            }
        }
        private async Task PrintVoucherAsync()
        {
            try
            {
                bool isVisble = await WaitHelper.WaitForElementVisibleAsync(_locators.PrintVoucher);
                if (isVisble)
                {
                    await Page.ClickAsync(_locators.AcceptPrint);
                }
                Logger.Debug($"");
            }
            catch (Exception ex)
            {
                Logger.Error($"{ex.Message}");
            }
        }
    }
}
