using IntellectPlaywrightTest.Core.Components;
using IntellectPlaywrightTest.Core.Interfaces;
using IntellectPlaywrightTest.Models;
using IntellectPlaywrightTest.Utilities.Helpers;
using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tesseract;

namespace IntellectPlaywrightTest.Modules.Membership.Components
{
    public class ShareDepositFormComponent : BaseFormComponent
    {
        private readonly IInputValidationHelper _inputHelper;
        private readonly ShareDepositLocaters _locators;
        public ShareDepositFormComponent(IPage page, IWaitHelper waitHelper, NLog.ILogger logger, IRetryHelper retryHelper, IInputValidationHelper inputHelper, ShareDepositLocaters locators) : base(page, waitHelper, logger, retryHelper)
        {
            _inputHelper = inputHelper ?? throw new ArgumentNullException(nameof(inputHelper));
            _locators = locators ?? throw new ArgumentNullException(nameof(locators));
        }
        public override async Task FillAsync<T>(T data)
        {
            if (data is not ShareDepositData sharedepositData)
            {
                throw new ArgumentException($"Expected ShareDepositData, got {typeof(T).Name}", nameof(data));
            }
            try
            {
                Logger.Info("Starting to fill Share Deposit form");
                await FillAdmissionNumberAsync(sharedepositData.AdmissionNo);
                await FillProductAsync(sharedepositData.Product);
                await FillAccountAsync(sharedepositData.AccountNo);
                await FillActivityAsync(sharedepositData.Activity);
                await FillVoucherTypeAsync(sharedepositData.Voucher);
                await FillAmountAsync(sharedepositData.Amount);
                Logger.Info("Share deposit successfully");
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to sign in during setup:{ex.Message}");
                throw;
            }
        }
        public async Task FillAdmissionNumberAsync(string admissionnumber)
        {
            var input = Page.Locator(_locators.AdmissionNo);
            var filled = await _inputHelper.FillTextBoxValueAsync(input, admissionnumber);
            if (!filled)
            {
                throw new InvalidOperationException($"Failed to fill Admission No: {admissionnumber}");
            }
            await SearchAdmissionAsync();
            Logger.Debug($"Filled Admission No: {admissionnumber}");
        }
        public async Task SearchAdmissionAsync()
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
        public async Task FillProductAsync(string product)
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
        public async Task FillAccountAsync(string account)
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
        public async Task FillActivityAsync(string activity)
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
        public async Task FillVoucherTypeAsync(string voucher)
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
        public async Task FillAmountAsync(string amount)
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
        public async Task ClickSaveAsync()
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
        public async Task CheckLastTranstionAsync()
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
        public async Task VerifyValidSaveAlertAsync()
        {
            try
            {
                bool isVisble = await WaitHelper.WaitForElementVisibleAsync(_locators.SaveAlert);
                if (isVisble)
                {
                    AssertHelper.AssertElementVisible(isVisble, _locators.SaveAlert);
                    await Page.ClickAsync(_locators.Okbtn);
                }
                Logger.Debug($"");
            }
            catch (Exception ex)
            {
                Logger.Error($"{ex.Message}");
            }
        }
        public async Task PrintVoucherAsync()
        {
            try
            {
                bool isVisble = await WaitHelper.WaitForElementVisibleAsync(_locators.PrintVoucher);
                if (isVisble)
                {
                    await Page.ClickAsync(_locators.AcceptPrint);
                }
            }
            catch (Exception ex)
            {
                Logger.Error($".{ex.Message}");
            }
        }
        public async Task<(decimal MaxamountforAtype,decimal MinamountforAtype,decimal MaxamountforBtype,decimal MinamountforBtype)> GetAuthorizedShareAmountAsync()
        {
            try
            {
                await Page.ClickAsync(_locators.Showparameters);
                var tableHelper = new HTMLTableHelper(this.Page,_locators.showsharedepositparametertbl);
                var aMaxText = await tableHelper.GetValueByDescriptionAsync("Maximum Individual Share Balance For Member", "Parameter Value");
                var aMinText = await tableHelper.GetValueByDescriptionAsync("Minimum Individual Share Balance For Member", "Parameter Value");
                var bMaxText = await tableHelper.GetValueByDescriptionAsync("Total Authorized Share Capital Maximum Amount Limit of Society- Associate Member", "Parameter Value");
                var bMinText = await tableHelper.GetValueByDescriptionAsync("Total Authorized Share Capital Maximum Amount Limit of Society- Member", "Parameter Value");
                decimal aMax = decimal.Parse(bMaxText.Replace(",", "").Trim());
                decimal aMin = decimal.Parse(aMinText.Replace(",", "").Trim());
                decimal bMax = decimal.Parse(bMaxText.Replace(",", "").Trim());
                decimal bMin = decimal.Parse(aMinText.Replace(",", "").Trim());
                await Page.ClickAsync(_locators.CloseShowparameters);
                return (aMax, aMin, bMax, bMin);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                return (0, 0, 0, 0);
            }
        }
        public async Task<int> GetProcessedAdmissionDigit()
        {
            try
            {
                string admissionNo = await Page.EvaluateAsync<string>("document.getElementById('Admissionno').value");
                if (string.IsNullOrWhiteSpace(admissionNo))
                    return 0;
                if (admissionNo.Length > 5)
                {
                    if (admissionNo.StartsWith("0"))
                    {
                        return int.TryParse(admissionNo.Substring(1, 1), out int a)? a: 0;
                    }
                    return int.TryParse(admissionNo.Substring(0, 1), out int b)? b: 0;
                }
                return 0;
            }
            catch (Exception ex)
            {
                Logger.Error($"Error in GetProcessedAdmissionDigit: {ex.Message}");
                return 0;
            }
        }
        public async Task<string> Getcustomersahrebalance()
        {
            try
            {
                var tableHelper = new HTMLTableHelper(this.Page,_locators.Particularstbl);
                string cusShareBal= await tableHelper.GetValueByDescriptionAsync("Paid up Capital - Individual", "Balance (  )");
                return cusShareBal;
                Logger.Debug($"");
            }
            catch (Exception ex)
            {
                return "0";
                Logger.Error(ex.Message);
            }
        }
    }
}
