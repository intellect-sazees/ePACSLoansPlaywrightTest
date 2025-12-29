using Microsoft.Playwright;

namespace ePACSLoans.Core.Interfaces
{
    /// <summary>
    /// Interface for input validation and form interaction operations
    /// </summary>
    public interface IInputValidationHelper
    {
        /// <summary>
        /// Checks if an input field is read-only
        /// </summary>
        Task<bool> CheckReadOnlyInputAsync(ILocator locator);
        /// <summary>
        /// Fetch data from the text box
        /// </summary>
        /// <param name="locator"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        Task<string?> GetValueinTextBoxAsync(ILocator locator);
        /// <summary>
        /// Fills a text box with a value, clearing existing content first
        /// </summary>
        Task<bool> FillTextBoxValueAsync(ILocator locator, string value);

        /// <summary>
        /// Converts date from DD-MM-YYYY format to YYYY-MM-DD format
        /// </summary>
        Task<string?> ConvertToDDMMYYYYAsync(string date);

        /// <summary>
        /// Selects an option from a dropdown
        /// </summary>
        Task SelectDropdownOptionAsync(ILocator dropdown, string option, SelectBy selectBy = SelectBy.Label);
    }

    /// <summary>
    /// Enumeration for dropdown selection methods
    /// </summary>
    public enum SelectBy
    {
        Label,
        Value,
        Index
    }
}


