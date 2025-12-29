namespace ePACSLoans.Core.Interfaces
{
    /// <summary>
    /// Interface for test data operations
    /// </summary>
    public interface ITestDataProvider
    {
        /// <summary>
        /// Gets test data for a specific module and test case
        /// </summary>
        Task<T> GetTestDataAsync<T>(string module, string testCase) where T : class;

        /// <summary>
        /// Gets test data from a specific JSON file
        /// </summary>
        Task<T> GetTestDataFromFileAsync<T>(string filePath, string section) where T : class;

        /// <summary>
        /// Gets a specific value from test data
        /// </summary>
        Task<string> GetDataAsync(string module, string testCase, string key);

        /// <summary>
        /// Updates test data value
        /// </summary>
        Task UpdateDataAsync(string module, string testCase, string key, string value);
    }
}


