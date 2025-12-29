using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ePACSLoans.Core.Interfaces;
using Tesseract;

namespace ePACSLoans.Utilities.Helpers
{
    /// <summary>
    /// Helper class for input validation and form interactions
    /// Implements IInputValidationHelper for dependency injection support
    /// </summary>
    public class InputValidationHelper : IInputValidationHelper
    {
        private readonly NLog.ILogger _logger;
        public InputValidationHelper(NLog.ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        /// <summary>
        /// Checks if an input field is read-only
        /// </summary>
        public async Task<bool> CheckReadOnlyInputAsync(ILocator locator)
        {
            try
            {
                var attr = await locator.GetAttributeAsync("readonly");
                bool isReadOnly = !await locator.IsEditableAsync();
                
                if (attr != null || isReadOnly)
                {
                    _logger.Debug("Input field is read-only");
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to check if input is read-only, assuming it's not", ex);
                return false;
            }
        }
        public async Task<string?> GetValueinTextBoxAsync(ILocator locator)
        {
            try
            {
                var value = await locator.InputValueAsync();
                _logger.Debug($"Value of textbox is: {value}");
                return value;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get value from the text box");
                return null;
            }
        }
        /// <summary>
        /// Fills a text box with a value, clearing existing content first
        /// </summary>
        public async Task<bool> FillTextBoxValueAsync(ILocator locator, string value)
        {
            try
            {
                _logger.Debug($"Filling text box with value: {value}");
                await locator.FocusAsync();
                await locator.FillAsync(value);
                _logger.Debug("Successfully filled text box");
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error($"Failed to fill text box with value: {value}", ex);
                return false;
            }
        }

        /// <summary>
        /// Converts date from DD-MM-YYYY format to YYYY-MM-DD format
        /// </summary>
        public async Task<string?> ConvertToDDMMYYYYAsync(string date)
        {
            try
            {
                _logger.Debug($"Converting date from DD-MM-YYYY to YYYY-MM-DD: {date}");
                var formattedDate = DateTime.ParseExact(date, "dd-MM-yyyy", null).ToString("yyyy-MM-dd");
                _logger.Debug($"Converted date: {formattedDate}");
                return formattedDate;
            }
            catch (Exception ex)
            {
                _logger.Error($"Failed to convert date: {date}", ex);
                return null;
            }
        }

        /// <summary>
        /// Selects an option from a dropdown
        /// </summary>
        public async Task SelectDropdownOptionAsync(ILocator dropdown, string option, SelectBy selectBy = SelectBy.Label)
        {
            try
            {
                _logger.Debug($"Selecting dropdown option: {option} by {selectBy}");
                
                switch (selectBy)
                {
                    case SelectBy.Label:
                        await dropdown.SelectOptionAsync(new SelectOptionValue { Label = option });
                        break;

                    case SelectBy.Value:
                        await dropdown.SelectOptionAsync(new SelectOptionValue { Value = option });
                        break;

                    case SelectBy.Index:
                        if (int.TryParse(option, out int index))
                        {
                            await dropdown.SelectOptionAsync(new SelectOptionValue { Index = index });
                        }
                        else
                        {
                            throw new ArgumentException($"Invalid index value: {option}");
                        }
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(selectBy), "Invalid selection type");
                }
                
                _logger.Debug($"Successfully selected dropdown option: {option}");
            }
            catch (Exception ex)
            {
                _logger.Error($"Failed to select dropdown option: {option} by {selectBy}", ex);
                throw;
            }
        }

        // Legacy method for backward compatibility
        [Obsolete("Use FillTextBoxValueAsync instead")]
        public async Task<bool> FillTextBoxValue(ILocator locator, string value)
        {
            return await FillTextBoxValueAsync(locator, value);
        }

        // Legacy method for backward compatibility
        [Obsolete("Use ConvertToDDMMYYYYAsync instead")]
        public async Task<string> ConverToDDMMYYYY(string date)
        {
            return await ConvertToDDMMYYYYAsync(date) ?? string.Empty;
        }

        // Legacy method for backward compatibility
        [Obsolete("Use CheckReadOnlyInputAsync instead")]
        public async Task<bool> CheckReadOnlyInput(ILocator locator)
        {
            return await CheckReadOnlyInputAsync(locator);
        }
        [Obsolete("Use GetValueinTextBoxAsync instead")]
        public async Task<string?> GetTextBoxValue(ILocator locator)
        {
            return await GetValueinTextBoxAsync(locator);
        }
    }
}
