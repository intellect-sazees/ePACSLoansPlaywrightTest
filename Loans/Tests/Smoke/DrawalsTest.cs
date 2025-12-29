using IntellectPlaywrightTest.Core;
using IntellectPlaywrightTest.Modules.Borrowings;
using IntellectPlaywrightTest.Modules.Dashboard;
using IntellectPlaywrightTest.Modules.Login;
using IntellectPlaywrightTest.Utilities.Common;
using IntellectPlaywrightTest.Utilities.DataManagement;
using IntellectPlaywrightTest.Utilities.Helpers;
using IntellectPlaywrightTest.Utilities.LocatorManagement;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntellectPlaywrightTest.Models;
using Allure.NUnit;

namespace IntellectPlaywrightTest.Tests.Smoke
{
    [TestFixture]
    [Category("Smoke")]
    [AllureNUnit]
    [Obsolete("This test class is deprecated. Use DrawalsSmokeTests instead.")]
    public class DrawalsTest:BaseTest
    {
        private LoginPage? _loginPage;
        private DashboardPage? _dashboardPage;
        private DrawalsPage? _drawalsPage;
        private DrawalsData? _testData;
        public override async Task SetUp()
        {
            await base.SetUp();
            try
            {
                // Initialize dependencies
                NLog.ILogger logger = LogManager.GetLogger("DrawalsTest");
                var waitHelper = new WaitHelper(Page!);
                var retryHelper = new RetryHelper(logger);
                var inputHelper = new InputValidationHelper(logger);
                var testDataProvider = new TestDataReader(logger);
                var locatorLoader = new LocatorLoader(testDataProvider, logger);
                // Create page objects with dependency injection
                _loginPage = new LoginPage(Page!,waitHelper,logger,retryHelper,inputHelper,testDataProvider,locatorLoader);
                _dashboardPage = new DashboardPage(Page!, waitHelper,logger,retryHelper,testDataProvider,locatorLoader);
                _drawalsPage = new DrawalsPage(Page!,waitHelper,logger,retryHelper,inputHelper,testDataProvider,locatorLoader);
                // Load test data
                _testData = await LoadTestDataAsync();
                // Sign in before tests
                await SignInAsync();
                Logger.Info($"Test setup completed for: {TestContext.CurrentContext.Test.Name}");
            }
            catch (Exception ex)
            {
            }
        }
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
                throw;
            }
        }
        private async Task<LoginData> LoadLoginDataAsync()
        {
            var _objPath = new PathHelper();
            var basePath = _objPath.getProjectPath();
            var testDataPath = Path.Combine(basePath, FrameworkConstants.ResourcesFolder, FrameworkConstants.TestDataFolder, "TestData.json");
            var legacyReader = new TestDataReader(Logger);
            var get = (string key) => legacyReader.GetData(testDataPath, "Logincredantional", key);
            return new LoginData
            {
                Url = get("url"),
                Username = get("username"),
                Password = get("password"),
                LoginDate = get("loginDate")
            };
        }
        private async Task<DrawalsData> LoadTestDataAsync()
        {
            try
            {
                var _objPath = new PathHelper();
                var basePath = _objPath.getProjectPath();
                var testDataPath = Path.Combine(basePath, FrameworkConstants.ResourcesFolder,FrameworkConstants.TestDataFolder, "TestData.json");
                var legacyReader = new TestDataReader(Logger);
                var get = (string key) => legacyReader.GetData(testDataPath, "DrawalsTransactionData", key);
                DrawalsData obj = new DrawalsData()
                {
                    Product = get("Product"),
                    AccountNumber=get("AccountNumber"),
                    DrawalAmount=get("DrawalAmount"),
                    VoucherType = get("VoucherType"),
                };
                return obj;
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to load account creation test data", ex);
                throw;
            }
        }
        [Test]
        [Description("Verify user can save a borrowing drawals transation with all mandatory fields")]
        public async Task DrawalsTranstionAsync()
        {
            Assert.That(_drawalsPage, Is.Not.Null, "Drawals page should be initialized");
            Assert.That(_dashboardPage, Is.Not.Null, "DashboardPage should be initialized");
            Assert.That(_testData, Is.Not.Null, "Test data should be loaded");
            await _drawalsPage!.CaptureBorrowingDrawalsAsync(_dashboardPage!, _testData!);
            Assert.Pass("Drawals Transaction completed successfully");
        }
    }
}
