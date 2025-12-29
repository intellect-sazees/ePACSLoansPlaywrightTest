using IntellectPlaywrightTest.Core.Components;
using IntellectPlaywrightTest.Core.Interfaces;
using IntellectPlaywrightTest.Models;
using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntellectPlaywrightTest.Modules.FAS.Components
{
    public class TranactionReceptFormComponent : BaseFormComponent
    {
        private readonly IInputValidationHelper _inputHelper;
        private readonly TrasactionReceptLocaters _locators;
        public TranactionReceptFormComponent(IPage page, IWaitHelper waitHelper, NLog.ILogger logger, IRetryHelper retryHelper, IInputValidationHelper inputHelper, TrasactionReceptLocaters locators) : base(page, waitHelper, logger, retryHelper)
        {
            _inputHelper = inputHelper ?? throw new ArgumentNullException(nameof(inputHelper));
            _locators = locators ?? throw new ArgumentNullException(nameof(locators));
        }
        public override async Task FillAsync<T>(T data)
        {
            if (data is not TransactionReceptData transactionreseptData)
            {
                throw new ArgumentException($"Expected TransactioReceptData, got {typeof(T).Name}", nameof(data));
            }

            try
            {
                Logger.Info("Starting to fill Transaction Recept form");

                // Fill all form fields in order
                await FillAdmissionNumberAsync(transactionreseptData.AdmissionNo);
                await SearchAdmissionAsync();
                await FillProductAsync(transactionreseptData.Product);
                Logger.Info("Account creation form filled successfully");
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to fill account creation form", ex);
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
                Logger.Error("Searchicon is not clcikable");
            }
        }
        private async Task FillProductAsync(string product)
        {
            await _inputHelper.SelectDropdownOptionAsync(Page.Locator(_locators.Product), product, SelectBy.Label);
            Logger.Debug($"Selected product: {product}");
        }
    }
}
