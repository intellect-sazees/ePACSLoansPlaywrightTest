using Allure.NUnit;
using IntellectPlaywrightTest.Core;
using IntellectPlaywrightTest.Models;
using IntellectPlaywrightTest.Modules.Dashboard;
using IntellectPlaywrightTest.Modules.Login;
using IntellectPlaywrightTest.Modules.Membership;
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

namespace IntellectPlaywrightTest.Tests.FunctionalTests
{
    [TestFixture]
    [Category("Functional")]
    [Obsolete("This test class is deprecated. Use SharedepositSmokeTests instead.")]
    [AllureNUnit]
    public class ShareDepositFunctionalTest:BaseTest
    {
        private LoginPage? _loginPage;
        private DashboardPage? _dashboardPage;
        private ShareDepositPage? _sharedepositPage;
        private ShareDepositData? _testData;
        public override async Task SetUp()
        {
            await base.SetUp();
            try
            {
                NLog.ILogger logger = LogManager.GetLogger("ShareDeposit Test");
                var waitHelper = new WaitHelper(Page!);
                var retryHelper = new RetryHelper(logger);
                var inputHelper = new InputValidationHelper(logger);
                var testDataProvider = new TestDataReader(logger);
                var locatorLoader = new LocatorLoader(testDataProvider, logger);
                _sharedepositPage = new ShareDepositPage(Page!, waitHelper, logger, retryHelper, inputHelper, testDataProvider, locatorLoader);
                _loginPage = new LoginPage(Page!, waitHelper, logger, retryHelper, inputHelper, testDataProvider, locatorLoader);
                _dashboardPage = new DashboardPage(Page!, waitHelper, logger, retryHelper, testDataProvider, locatorLoader);
                _testData = await LoadTestDataAsync();
                await SignInAsync();
                Logger.Info($"Test setup completed for: {TestContext.CurrentContext.Test.Name}");
            }
            catch (Exception ex)
            {
                Logger.Error($"Test setup faliure :{ex.Message}");
            }
        }
        private async Task SignInAsync()
        {
            try
            {
                var loginData = await LoadLoginDataAsync();
                await _loginPage!.SignInAsync(loginData.Url, loginData.Username, loginData.Password, loginData.LoginDate);
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to sign in during setup:{ex.Message}");
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
        private async Task<ShareDepositData> LoadTestDataAsync()
        {
            try
            {
                var _objPath = new PathHelper();
                var basePath = _objPath.getProjectPath();
                var testDataPath = Path.Combine(basePath, FrameworkConstants.ResourcesFolder, FrameworkConstants.TestDataFolder, "TestData.json");
                var legacyReader = new TestDataReader(Logger);
                var get = (string key) => legacyReader.GetData(testDataPath, "ShareDepositPage", key);
                ShareDepositData obj = new ShareDepositData()
                {
                    AdmissionNo = get("AdmissionNo"),
                    Product = get("Product"),
                    AccountNo = get("AccountNo"),
                    Activity = get("ActivityType"),
                    Voucher = get("Vouchertype"),
                    Amount = get("Amount"),
                };
                return obj;
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to sign in during setup:{ex.Message}");
                throw;
            }
        }
        [Test]
        [Description("Verify that the system validates the minimum and maximum share deposit amount")]
        public async Task ShareDepositAmountValidationAsync()
        {
            await _sharedepositPage!.ShareDepositAmonutValidationAsync(_dashboardPage!, _testData!);
            Assert.Pass("Share deposit Transaction completed successfully");
        }
    }
}
