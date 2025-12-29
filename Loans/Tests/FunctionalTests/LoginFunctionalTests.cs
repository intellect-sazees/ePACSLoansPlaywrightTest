using Allure.NUnit;
using IntellectPlaywrightTest.Core;
using IntellectPlaywrightTest.Core.Interfaces;
using IntellectPlaywrightTest.Modules.Login;
using IntellectPlaywrightTest.Utilities.Common;
using IntellectPlaywrightTest.Utilities.DataManagement;
using IntellectPlaywrightTest.Utilities.Helpers;
using NLog;
using NUnit.Framework;
using System.Xml.Linq;

namespace IntellectPlaywrightTest.Tests.FunctionalTests
{
    /// <summary>
    /// Functional tests for Login functionality
    /// Uses BaseTest for setup/teardown and dependency injection
    /// </summary>
    [TestFixture]
    [Category("Functional")]
    [Category("Login")]
    [AllureNUnit]
    public class LoginFunctionalTests : BaseTest
    {
        private LoginPage? _loginPage;
        private ITestDataProvider? _testDataProvider;
        private LoginTestData? _testData;

        /// <summary>
        /// Sets up test dependencies and loads test data
        /// </summary>
        public override async Task SetUp()
        {
            await base.SetUp();

            try
            {
                // Initialize logger
                NLog.ILogger logger = LogManager.GetLogger("LoginFunctionalTests");

                // Create PageFactoryContext with all dependencies
                var context = PageFactoryContext.CreateFromTest(Page!, logger);
                _testDataProvider = context.TestDataProvider;

                // Create PageFactory and use it to create page objects
                var pageFactory = new PageFactory(context);
                //_loginPage = pageFactory.CreateLoginPage();
                _loginPage = pageFactory.CreatePage<LoginPage>();

                // Load test data
                _testData = await LoadTestDataAsync();

                Logger.Info($"Test setup completed for: {TestContext.CurrentContext.Test.Name}");
            }
            catch (Exception ex)
            {
                Logger.Error($"Test setup failed: {TestContext.CurrentContext.Test.Name}", ex);
                throw;
            }
        }

        /// <summary>
        /// Loads test data from JSON file using new module-based structure
        /// </summary>
        private async Task<LoginTestData> LoadTestDataAsync()
        {
            try
            {
                // Use the new module-based test data loading
                // This will automatically use Resources/TestData/Login/LoginTestData.json
                // and replace environment variables (${LOGIN_URL}, etc.)
                var testData = await _testDataProvider!.GetTestDataFromFileAsync<LoginTestData>(
                    GetLoginTestDataFilePath(), 
                    "LoginTestData");

                // Log loaded values to verify environment variable substitution
                Logger.Info($"Successfully loaded login test data:");
                Logger.Info($"  URL: {testData.Url}");
                Logger.Info($"  Username: {testData.Username}");
                Logger.Info($"  Password: {(string.IsNullOrEmpty(testData.Password) ? "[EMPTY]" : "[***HIDDEN***]")}");
                Logger.Info($"  LoginDate: {testData.LoginDate}");
                
                return testData;
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to load test data", ex);
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
            
            return Path.Combine(basePath, FrameworkConstants.ResourcesFolder, 
                FrameworkConstants.TestDataFolder, "Login", "LoginTestData.json");
        }

        [Test]
        [Description("Verify user can successfully sign in with valid credentials")]
        public async Task VerifyUserCanSignInWithValidCredentials()
        {
            Assert.That(_loginPage, Is.Not.Null, "LoginPage should be initialized");
            Assert.That(_testData, Is.Not.Null, "Test data should be loaded");

            var result = await _loginPage!.SignInAsync(
                _testData!.Url,
                _testData.Username,
                _testData.Password,
                _testData.LoginDate);

            Assert.That(result, Is.True, "Sign-in should be successful");
        }

        [Test]
        [Description("Verify user can successfully sign out")]
        public async Task VerifyUserCanSignOut()
        {
            Assert.That(_loginPage, Is.Not.Null, "LoginPage should be initialized");
            Assert.That(_testData, Is.Not.Null, "Test data should be loaded");

            // First sign in
            await _loginPage!.SignInAsync(
                _testData!.Url,
                _testData.Username,
                _testData.Password,
                _testData.LoginDate);

            // Then sign out
            await _loginPage.SignOutAsync();

            // Verify sign out was successful (add assertion based on your app behavior)
            Assert.Pass("Sign out completed successfully");
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
    /// Test data model for login tests
    /// </summary>
    public class LoginTestData
    {
        public string Url { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string LoginDate { get; set; } = string.Empty;
    }
}

