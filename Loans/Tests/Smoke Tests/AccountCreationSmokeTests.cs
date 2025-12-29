using Allure.NUnit;
using IntellectPlaywrightTest.Core;
using IntellectPlaywrightTest.Core.Interfaces;
using IntellectPlaywrightTest.Modules.Borrowings;
using IntellectPlaywrightTest.Modules.Dashboard;
using IntellectPlaywrightTest.Modules.Login;
using IntellectPlaywrightTest.Utilities.Common;
using IntellectPlaywrightTest.Utilities.DataManagement;
using IntellectPlaywrightTest.Utilities.Helpers;
using IntellectPlaywrightTest.Models;
using NLog;
using NUnit.Framework;

namespace IntellectPlaywrightTest.Tests.SmokeTests
{
    /// <summary>
    /// Smoke tests for Account Creation functionality
    /// Uses BaseTest for setup/teardown and dependency injection
    /// </summary>
    [TestFixture]
    [Category("Smoke")]
    [Category("Borrowings")]
    [Category("AccountCreation")]
    [AllureNUnit]
    public class AccountCreationSmokeTests : BaseTest
    {
        private LoginPage? _loginPage;
        private DashboardPage? _dashboardPage;
        private AccountCreationPage? _accountCreationPage;
        private ITestDataProvider? _testDataProvider;
        private AccountCreationData? _testData;

        /// <summary>
        /// Sets up test dependencies and loads test data
        /// </summary>
        public override async Task SetUp()
        {
            await base.SetUp();

            try
            {
                // Initialize logger
                NLog.ILogger logger = LogManager.GetLogger("AccountCreationSmokeTests");

                // Create PageFactoryContext with all dependencies
                var context = PageFactoryContext.CreateFromTest(Page!, logger);
                _testDataProvider = context.TestDataProvider;

                // Create PageFactory and use it to create page objects
                var pageFactory = new PageFactory(context);
                _loginPage= pageFactory.CreatePage<LoginPage>();
                _dashboardPage = pageFactory.CreatePage<DashboardPage>();
                _accountCreationPage = pageFactory.CreatePage<AccountCreationPage>(); 
                          

                // Load test data
                _testData = await LoadTestDataAsync();

                // Sign in before tests
                await SignInAsync();

                Logger.Info($"Test setup completed for: {TestContext.CurrentContext.Test.Name}");
            }
            catch (Exception ex)
            {
                Logger.Error($"Test setup failed: {TestContext.CurrentContext.Test.Name}", ex);
                throw;
            }
        }

        /// <summary>
        /// Signs in to the application
        /// </summary>
        private async Task SignInAsync()
        {
            try
            {
                var loginData = await LoadLoginDataAsync();
                await _loginPage!.SignInAsync(loginData.Url,loginData.Username,loginData.Password,loginData.LoginDate);
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to sign in during setup", ex);
            }
        }

        /// <summary>
        /// Loads login test data using new module-based structure
        /// </summary>
        private async Task<LoginTestData> LoadLoginDataAsync()
        {
            try
            {
                var testData = await _testDataProvider!.GetTestDataFromFileAsync<LoginTestData>(
                    GetLoginTestDataFilePath(), 
                    "LoginTestData");

                Logger.Info("Successfully loaded login test data");
                return testData;
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to load login test data", ex);
                throw;
            }
        }

        /// <summary>
        /// Gets the path to Login test data file
        /// </summary>
        private string GetLoginTestDataFilePath()
        {
            var pathHelper = new PathHelper();
            var basePath = pathHelper.getProjectPath();
            return Path.Combine(basePath, FrameworkConstants.ResourcesFolder, FrameworkConstants.TestDataFolder, "Login", "LoginTestData.json");
        }

        /// <summary>
        /// Loads account creation test data
        /// </summary>
        private async Task<AccountCreationData> LoadTestDataAsync()
        {
            try
            {
                //var currentDir = Directory.GetCurrentDirectory();
                //var basePath = Path.GetFullPath(Path.Combine(currentDir, "..", "..", ".."));

                //if (currentDir.Contains("bin"))
                //{
                //    basePath = Path.GetFullPath(Path.Combine(currentDir, "..", "..", "..", "..", ".."));
                //}

                //var testDataPath = Path.Combine(basePath, FrameworkConstants.ResourcesFolder, 
                //    FrameworkConstants.TestDataFolder, "Borrowings", "AccountCreationTestData.json");

                //NLog.ILogger logger = NLog.LogManager.GetLogger("AccountCreationSmokeTests");
                //var legacyReader = new TestDataReader(logger);
                //var get = (string key) => legacyReader.GetData(testDataPath, "AccountCreationData", key);
                var testData = await _testDataProvider!.GetTestDataFromFileAsync<AccountCreationData>(
                    GetAccountCreationTestDataFilePath(), 
                    "AccountCreationTestData");

                Logger.Info("Successfully loaded account creation test data");
                return testData;
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to load test data", ex);
                throw;
            }
        }

        /// <summary>
        /// Gets the path to Account Creation test data file
        /// </summary>
        private string GetAccountCreationTestDataFilePath()
        {
            var pathHelper = new PathHelper();
            var basePath = pathHelper.getProjectPath();
            
            return Path.Combine(basePath, FrameworkConstants.ResourcesFolder, 
                FrameworkConstants.TestDataFolder, "Borrowings", "AccountCreationTestData.json");
        }

        [Test]
        [Description("Verify user can create a borrowing account with all mandatory fields")]
        public async Task VerifyUserCanCreateBorrowingAccount()
        {
            Assert.That(_accountCreationPage, Is.Not.Null, "AccountCreationPage should be initialized");
            Assert.That(_dashboardPage, Is.Not.Null, "DashboardPage should be initialized");
            Assert.That(_testData, Is.Not.Null, "Test data should be loaded");

            await _accountCreationPage!.CreateBorrowingAccountAsync(_dashboardPage!, _testData!);

            // Add assertions based on your application behavior
            // For example, verify success message, account ID, etc.
            Assert.Pass("Account creation completed successfully");
        }

        public override async Task TearDown()
        {
            try
            {
                if (_loginPage != null)
                {
                    await _loginPage.SignOutAsync();
                }
            }
            catch (Exception ex)
            {
                Logger.Warn("Error during sign out in teardown", ex);
            }
            finally
            {
                await base.TearDown();
            }
        }
    }

    /// <summary>
    /// Login test data model
    /// </summary>
    public class LoginTestData
    {
        public string Url { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string LoginDate { get; set; } = string.Empty;
    }
}

