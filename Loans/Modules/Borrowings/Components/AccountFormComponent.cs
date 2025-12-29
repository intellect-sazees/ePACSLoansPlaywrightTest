using IntellectPlaywrightTest.Core.Components;
using IntellectPlaywrightTest.Core.Interfaces;
using IntellectPlaywrightTest.Modules.Borrowings;
using IntellectPlaywrightTest.Utilities.Helpers;
using Microsoft.Playwright;
using IntellectPlaywrightTest.Models;
using NLog;

namespace IntellectPlaywrightTest.Modules.Borrowings.Components
{
    /// <summary>
    /// Form component for Account Creation form
    /// Handles all form field interactions for account creation
    /// </summary>
    public class AccountFormComponent : BaseFormComponent
    {
        private readonly IInputValidationHelper _inputHelper;
        private readonly AccountCreationLocators _locators;

        /// <summary>
        /// Initializes a new instance of AccountFormComponent
        /// </summary>
        public AccountFormComponent(IPage page,IWaitHelper waitHelper,NLog.ILogger logger,IRetryHelper retryHelper,IInputValidationHelper inputHelper,AccountCreationLocators locators): base(page, waitHelper, logger, retryHelper)
        {
            _inputHelper = inputHelper ?? throw new ArgumentNullException(nameof(inputHelper));
            _locators = locators ?? throw new ArgumentNullException(nameof(locators));
        }

        /// <summary>
        /// Fills the account creation form with provided data
        /// </summary>
        /// <param name="data">AccountCreationData object containing form field values</param>
        public override async Task FillAsync<T>(T data)
        {
            if (data is not AccountCreationData accountData)
            {
                throw new ArgumentException($"Expected AccountCreationData, got {typeof(T).Name}", nameof(data));
            }

            try
            {
                Logger.Info("Starting to fill account creation form");

                // Fill all form fields in order
                await FillProductAsync(accountData.Product);
                await FillPurposeAsync(accountData.Purpose);
                await FillSocietyLoanNoAsync(accountData.SocietyLoanNo);
                await FillAppliedDateAsync(accountData.AppliedDate);
                await FillAppliedAmountAsync(accountData.AppliedAmount);
                await FillSanctionDateAsync(accountData.SanctionDate);
                await FillSanctionAmountAsync(accountData.SanctionAmount);
                await FillRepaymentTypeAsync(accountData.RepaymentType);
                await FillRepaymentModeAsync(accountData.RepaymentMode);
                await FillROIAsync(accountData.ROI);
                await FillPenalROIAsync(accountData.PenalROI);
                await FillIOAROIAsync(accountData.IOAROI);
                await FillGestationPeriodMonthsAsync(accountData.GestationPeriodMonths);
                await FillLoanPeriodMonthsAsync(accountData.LoanPeriodMonths);

                Logger.Info("Account creation form filled successfully");
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to fill account creation form", ex);
                throw;
            }
        }

        /// <summary>
        /// Fills the Product dropdown
        /// </summary>
        private async Task FillProductAsync(string product)
        {
            await _inputHelper.SelectDropdownOptionAsync(Page.Locator(_locators.ProductList),product,SelectBy.Label);
            Logger.Debug($"Selected product: {product}");
        }
        /// <summary>
        /// Fills the Purpose dropdown
        /// </summary>
        private async Task FillPurposeAsync(string purpose)
        {
            await _inputHelper.SelectDropdownOptionAsync(Page.Locator(_locators.PurposeList),purpose,SelectBy.Label);
            Logger.Debug($"Selected purpose: {purpose}");
        }
        /// <summary>
        /// Fills the Society Loan No field
        /// </summary>
        private async Task FillSocietyLoanNoAsync(string societyLoanNo)
        {
            var input = Page.Locator(_locators.SocietyLoanNo);
            var filled = await _inputHelper.FillTextBoxValueAsync(input, societyLoanNo);
            if (!filled)
            {
                throw new InvalidOperationException($"Failed to fill Society Loan No: {societyLoanNo}");
            }
            Logger.Debug($"Filled Society Loan No: {societyLoanNo}");
        }

        /// <summary>
        /// Fills the Applied Date field
        /// </summary>
        private async Task FillAppliedDateAsync(string date)
        {
            var formattedDate = await _inputHelper.ConvertToDDMMYYYYAsync(date);
            if (string.IsNullOrEmpty(formattedDate))
            {
                throw new InvalidOperationException($"Failed to convert date: {date}");
            }
            await FillTextFieldAsync(_locators.AppliedDate, formattedDate);
            Logger.Debug($"Filled Applied Date: {formattedDate}");
        }

        /// <summary>
        /// Fills the Applied Amount field
        /// </summary>
        private async Task FillAppliedAmountAsync(string amount)
        {
            var input = Page.Locator(_locators.AppliedAmount);
            var filled = await _inputHelper.FillTextBoxValueAsync(input, amount);
            if (!filled)
            {
                throw new InvalidOperationException($"Failed to fill Applied Amount: {amount}");
            }
            Logger.Debug($"Filled Applied Amount: {amount}");
        }

        /// <summary>
        /// Fills the Sanction Date field
        /// </summary>
        private async Task FillSanctionDateAsync(string date)
        {
            var formattedDate = await _inputHelper.ConvertToDDMMYYYYAsync(date);
            if (string.IsNullOrEmpty(formattedDate))
            {
                throw new InvalidOperationException($"Failed to convert date: {date}");
            }
            await FillTextFieldAsync(_locators.SanctionDate, formattedDate);
            Logger.Debug($"Filled Sanction Date: {formattedDate}");
        }

        /// <summary>
        /// Fills the Sanction Amount field
        /// </summary>
        private async Task FillSanctionAmountAsync(string amount)
        {
            var input = Page.Locator(_locators.SanctionAmount);
            var filled = await _inputHelper.FillTextBoxValueAsync(input, amount);
            if (!filled)
            {
                throw new InvalidOperationException($"Failed to fill Sanction Amount: {amount}");
            }
            Logger.Debug($"Filled Sanction Amount: {amount}");
        }

        /// <summary>
        /// Fills the Repayment Type dropdown
        /// </summary>
        private async Task FillRepaymentTypeAsync(string repaymentType)
        {
            await _inputHelper.SelectDropdownOptionAsync(Page.Locator(_locators.RepaymentType),repaymentType,SelectBy.Label);
            Logger.Debug($"Selected Repayment Type: {repaymentType}");
        }

        /// <summary>
        /// Fills the Repayment Mode dropdown
        /// </summary>
        private async Task FillRepaymentModeAsync(string repaymentMode)
        {
            await _inputHelper.SelectDropdownOptionAsync(Page.Locator(_locators.RepaymentMode),repaymentMode,SelectBy.Label);
            Logger.Debug($"Selected Repayment Mode: {repaymentMode}");
        }

        /// <summary>
        /// Fills the ROI field (handles read-only check)
        /// </summary>
        private async Task FillROIAsync(string roi)
        {
            var input = Page.Locator(_locators.ROI);
            var isReadOnly = await _inputHelper.CheckReadOnlyInputAsync(input);
            if (!isReadOnly)
            {
                var filled = await _inputHelper.FillTextBoxValueAsync(input, roi);
                if (!filled)
                {
                    Logger.Warn($"Failed to fill ROI: {roi}");
                }
                else
                {
                    Logger.Debug($"Filled ROI: {roi}");
                }
            }
            else
            {
                Logger.Debug("ROI field is read-only, skipping");
            }
        }

        /// <summary>
        /// Fills the Penal ROI field (handles read-only check)
        /// </summary>
        private async Task FillPenalROIAsync(string penalROI)
        {
            var input = Page.Locator(_locators.PenalROI);
            var isReadOnly = await _inputHelper.CheckReadOnlyInputAsync(input);
            if (!isReadOnly)
            {
                var filled = await _inputHelper.FillTextBoxValueAsync(input, penalROI);
                if (!filled)
                {
                    Logger.Warn($"Failed to fill Penal ROI: {penalROI}");
                }
                else
                {
                    Logger.Debug($"Filled Penal ROI: {penalROI}");
                }
            }
            else
            {
                Logger.Debug("Penal ROI field is read-only, skipping");
            }
        }

        /// <summary>
        /// Fills the IOA ROI field (handles read-only check)
        /// </summary>
        private async Task FillIOAROIAsync(string ioaroi)
        {
            var input = Page.Locator(_locators.IOAROI);
            var isReadOnly = await _inputHelper.CheckReadOnlyInputAsync(input);
            if (!isReadOnly)
            {
                var filled = await _inputHelper.FillTextBoxValueAsync(input, ioaroi);
                if (!filled)
                {
                    Logger.Warn($"Failed to fill IOA ROI: {ioaroi}");
                }
                else
                {
                    Logger.Debug($"Filled IOA ROI: {ioaroi}");
                }
            }
            else
            {
                Logger.Debug("IOA ROI field is read-only, skipping");
            }
        }

        /// <summary>
        /// Fills the Gestation Period Months field (handles read-only check)
        /// </summary>
        private async Task FillGestationPeriodMonthsAsync(string months)
        {
            var input = Page.Locator(_locators.GestationPeriodMonths);
            var isReadOnly = await _inputHelper.CheckReadOnlyInputAsync(input);
            if (!isReadOnly)
            {
                var filled = await _inputHelper.FillTextBoxValueAsync(input, months);
                if (!filled)
                {
                    Logger.Warn($"Failed to fill Gestation Period Months: {months}");
                }
                else
                {
                    Logger.Debug($"Filled Gestation Period Months: {months}");
                }
            }
            else
            {
                Logger.Debug("Gestation Period Months field is read-only, skipping");
            }
        }
        /// <summary>
        /// Fills the Loan Period Months field
        /// </summary>
        private async Task FillLoanPeriodMonthsAsync(string months)
        {
            var input = Page.Locator(_locators.LoanPeriodMonths);
            var filled = await _inputHelper.FillTextBoxValueAsync(input, months);
            if (!filled)
            {
                throw new InvalidOperationException($"Failed to fill Loan Period Months: {months}");
            }
            Logger.Debug($"Filled Loan Period Months: {months}");
        }
    }
}