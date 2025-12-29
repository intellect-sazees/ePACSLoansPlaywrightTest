using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePACSLoans.Utilities.Helpers
{
    public class HTMLTableHelper
    {
        private readonly IPage _page;
        private readonly string _tableSelector;
        private readonly string _nextButtonSelector;
        private readonly string _prevButtonSelector;
        public HTMLTableHelper(IPage page, string tableSelector = null, string nextButtonSelector = null, string prevButtonSelector = null)
        {
            _page = page ?? throw new ArgumentNullException(nameof(page));
            _tableSelector = tableSelector;
            _nextButtonSelector = nextButtonSelector;
            _prevButtonSelector = prevButtonSelector;
        }
        private async Task<List<string>> GetHeadersAsync()
        {
            var table = _page.Locator(_tableSelector);
            var headers = await table.Locator("thead th").AllInnerTextsAsync();
            if (headers == null || headers.Count == 0)
                throw new Exception("Table headers not found or table may be empty.");
            return headers.ToList();
        }
        private async Task<ILocator> GetRowsLocatorAsync()
        {
            var table = _page.Locator(_tableSelector);
            return table.Locator("tbody tr");
        }
        public async Task<ILocator?> FindRowAsync(string columnName, string value)
        {
            var headers = await GetHeadersAsync();
            int colIndex = headers.FindIndex(h => string.Equals(h.Trim(), columnName.Trim(), StringComparison.OrdinalIgnoreCase));
            if (colIndex < 0)
                throw new Exception($"Column '{columnName}' not found.");
            var rows = await GetRowsLocatorAsync();
            int rowCount = await rows.CountAsync();
            for (int i = 0; i < rowCount; i++)
            {
                var cellText = await rows.Nth(i).Locator("td").Nth(colIndex).InnerTextAsync();
                if (string.Equals(cellText.Trim(), value.Trim(), StringComparison.OrdinalIgnoreCase))
                    return rows.Nth(i);
            }
            return null;
        }
        public async Task<ILocator?> FindRowMultipleConditionsAsync(Dictionary<string, string> conditions)
        {
            if (conditions == null || conditions.Count == 0)
                throw new ArgumentException("Conditions dictionary is null or empty.");

            var headers = await GetHeadersAsync();
            var rows = await GetRowsLocatorAsync();
            int rowCount = await rows.CountAsync();
            foreach (var colName in conditions.Keys)
            {
                if (!headers.Any(h => string.Equals(h.Trim(), colName.Trim(), StringComparison.OrdinalIgnoreCase)))
                    throw new Exception($"Column '{colName}' not found.");
            }
            for (int r = 0; r < rowCount; r++)
            {
                bool allMatch = true;
                foreach (var kvp in conditions)
                {
                    int colIndex = headers.FindIndex(h => string.Equals(h.Trim(), kvp.Key.Trim(), StringComparison.OrdinalIgnoreCase));
                    var cellText = await rows.Nth(r).Locator("td").Nth(colIndex).InnerTextAsync();
                    if (!string.Equals(cellText.Trim(), kvp.Value.Trim(), StringComparison.OrdinalIgnoreCase))
                    {
                        allMatch = false;
                        break;
                    }
                }
                if (allMatch)
                    return rows.Nth(r);
            }
            return null;
        }
        public async Task<Dictionary<string, string>> GetRowAsDictionaryAsync(ILocator row)
        {
            if (row == null)
                throw new ArgumentNullException(nameof(row));
            var headers = await GetHeadersAsync();
            var cells = await row.Locator("td").AllInnerTextsAsync();

            var dict = new Dictionary<string, string>();
            for (int i = 0; i < headers.Count; i++)
            {
                dict[headers[i]] = i < cells.Count ? cells[i].Trim() : "";
            }
            return dict;
        }
        public async Task<bool> ClickCheckboxAsync(ILocator row)
        {
            if (row == null)
                throw new ArgumentNullException(nameof(row));
            var checkbox = row.Locator("input[type=checkbox]");
            if (await checkbox.CountAsync() == 0 || !await checkbox.IsVisibleAsync())
                return false;
            try
            {
                await checkbox.ClickAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public async Task<bool> ClickActionAsync(ILocator row, string actionSelector)
        {
            if (row == null)
                throw new ArgumentNullException(nameof(row));
            if (string.IsNullOrWhiteSpace(actionSelector))
                throw new ArgumentNullException(nameof(actionSelector));
            var actionElem = row.Locator(actionSelector);
            if (await actionElem.CountAsync() == 0 || !await actionElem.IsVisibleAsync())
                return false;
            try
            {
                await actionElem.ClickAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public async Task<bool> BulkSelectCheckboxesAsync()
        {
            var rows = await GetRowsLocatorAsync();
            int rowCount = await rows.CountAsync();

            bool allClicked = true;

            for (int i = 0; i < rowCount; i++)
            {
                var checkbox = rows.Nth(i).Locator("input[type=checkbox]");
                if (await checkbox.CountAsync() == 0 || !await checkbox.IsVisibleAsync())
                    continue;
                try
                {
                    await checkbox.CheckAsync();
                }
                catch
                {
                    allClicked = false;
                }
            }
            return allClicked;
        }
        public async Task<bool> GoToNextPageAsync()
        {
            if (string.IsNullOrWhiteSpace(_nextButtonSelector))
                throw new Exception("Next page selector not configured.");
            var nextButton = _page.Locator(_nextButtonSelector);
            if (await nextButton.CountAsync() == 0 || !await nextButton.IsEnabledAsync())
                return false;
            try
            {
                await nextButton.ClickAsync();
                await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public async Task<bool> GoToPreviousPageAsync()
        {
            if (string.IsNullOrWhiteSpace(_prevButtonSelector))
                throw new Exception("Previous page selector not configured.");

            var prevButton = _page.Locator(_prevButtonSelector);
            if (await prevButton.CountAsync() == 0 || !await prevButton.IsEnabledAsync())
                return false;

            try
            {
                await prevButton.ClickAsync();
                await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                return true;
            }
            catch
            {
                return false;
            }
        }
        //public async Task<string> GetValueByDescriptionAsync(string description, string columnName)
        //{
        //    try
        //    {
        //        if (string.IsNullOrWhiteSpace(description))
        //            throw new ArgumentNullException(nameof(description));
        //        // 1. Get headers and find the "Parameter Value" column index
        //        var headers = await GetHeadersAsync();
        //        int valueColIndex = headers.FindIndex(h => string.Equals(h.Trim(), columnName.Trim(), StringComparison.OrdinalIgnoreCase));
        //        if (valueColIndex < 0)
        //            throw new Exception($"Column '{columnName}' not found in table '{_tableSelector}'.");
        //        // 2. Get rows and search for the matching Parameter Description
        //        var rows = await GetRowsLocatorAsync();
        //        int rowCount = await rows.CountAsync();
        //        for (int i = 0; i < rowCount; i++)
        //        {
        //            var row = rows.Nth(i);
        //            var cells = row.Locator("td");
        //            // Assuming first column is "Parameter Description"
        //            string descText = (await cells.Nth(0).InnerTextAsync()).Trim();
        //            if (string.Equals(descText, description.Trim(), StringComparison.OrdinalIgnoreCase))
        //            {
        //                string rawValue = (await cells.Nth(valueColIndex).InnerTextAsync()).Trim();

        //                // Check if the value is empty or null, then fallback to input value
        //                if (string.IsNullOrEmpty(rawValue))
        //                {
        //                    rawValue = (await cells.Nth(valueColIndex).InputValueAsync()).Trim();
        //                } 
        //                string descText1 = (await cells.Nth(0).InnerTextAsync()).Trim();
        //                if (string.Equals(descText, description.Trim(), StringComparison.OrdinalIgnoreCase))
        //                {
        //                    // Use JavaScript to directly get the value of the input
        //                    string inputValue = await cells.Nth(valueColIndex).Locator("input")
        //                        .EvaluateAsync<string>("element => element.value");

        //                    return inputValue?.Trim();
        //                }
        //                //string rawValue = (await cells.Nth(valueColIndex).InnerTextAsync()).Trim();
        //                //if (rawValue == null || rawValue == "")
        //                //{
        //                //    string rowValue2 = (await cells.Nth(valueColIndex).InputValueAsync()).Trim();
        //                //    return rowValue2;
        //                //}
        //                return rawValue;
        //            }
        //        }
        //        throw new Exception($"Row with description '{description}' not found in table '{_tableSelector}'.");
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }
        //}
        public async Task<string> GetValueByDescriptionAsync(string description, string columnName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(description))
                    throw new ArgumentNullException(nameof(description));

                // Get headers and find column index
                var headers = await GetHeadersAsync();
                int valueColIndex = headers.FindIndex(h =>
                    string.Equals(h.Trim(), columnName.Trim(), StringComparison.OrdinalIgnoreCase));

                if (valueColIndex < 0)
                    throw new Exception($"Column '{columnName}' not found in table '{_tableSelector}'.");

                // Get rows
                var rows = await GetRowsLocatorAsync();
                int rowCount = await rows.CountAsync();

                for (int i = 0; i < rowCount; i++)
                {
                    var row = rows.Nth(i);
                    var cells = row.Locator("td");

                    // First column = Description
                    string descText = (await cells.Nth(0).InnerTextAsync()).Trim();

                    if (!string.Equals(descText, description.Trim(), StringComparison.OrdinalIgnoreCase))
                        continue;

                    // Try to read td text
                    string rawValue = (await cells.Nth(valueColIndex).InnerTextAsync())?.Trim();

                    // ✅ If td is null or empty → read input value
                    if (string.IsNullOrWhiteSpace(rawValue))
                    {
                        string inputValue = await cells.Nth(valueColIndex)
                            .Locator("input")
                            .EvaluateAsync<string>("element => element.value");

                        return inputValue?.Trim();
                    }

                    return rawValue;
                }

                throw new Exception($"Row with description '{description}' not found in table '{_tableSelector}'.");
            }
            catch (Exception ex) 
            {
                return null;
            }
        }

    }
}
