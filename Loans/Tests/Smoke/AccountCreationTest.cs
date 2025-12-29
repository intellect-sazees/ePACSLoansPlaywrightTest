using Allure.NUnit;
using IntellectPlaywrightTest.Core;
using IntellectPlaywrightTest.Core.Interfaces;
using IntellectPlaywrightTest.Modules.Borrowings;
using IntellectPlaywrightTest.Modules.Dashboard;
using IntellectPlaywrightTest.Modules.Login;
using IntellectPlaywrightTest.Utilities.Common;
using IntellectPlaywrightTest.Utilities.DataManagement;
using IntellectPlaywrightTest.Utilities.LocatorManagement;
using IntellectPlaywrightTest.Utilities.Helpers;
using IntellectPlaywrightTest.Models;
using NLog;
using NUnit.Framework;

namespace IntellectPlaywrightTest.Tests.Smoke
{
    /// <summary>
    /// Legacy smoke test for Account Creation - DEPRECATED
    /// Please use AccountCreationSmokeTests instead
    /// </summary>
    [TestFixture]
    [Category("Smoke")]
    [AllureNUnit]
    [Obsolete("This test class is deprecated. Use AccountCreationSmokeTests instead.")]
    public class AccountCreationTest : BaseTest
    {
        private LoginPage? _loginPage;
        private DashboardPage? _dashboardPage;
        private AccountCreationPage? _accountCreationPage;
        private AccountCreationData? _testData;

        public override async Task SetUp()
        {
            await base.SetUp();

            try
            {
                // Initialize dependencies
                NLog.ILogger logger = LogManager.GetLogger("AccountCreationTest");
                var waitHelper = new WaitHelper(Page!);
                var retryHelper = new RetryHelper(logger);
                var inputHelper = new InputValidationHelper(logger);
                var testDataProvider = new TestDataReader(logger);
                var locatorLoader = new LocatorLoader(testDataProvider, logger);
                // Create page objects with dependency injection
                _loginPage = new LoginPage(Page!,waitHelper,logger,retryHelper,inputHelper,testDataProvider,locatorLoader);

                _dashboardPage = new DashboardPage(Page!,waitHelper,logger,retryHelper,testDataProvider,locatorLoader);

                _accountCreationPage = new AccountCreationPage(Page!,waitHelper,logger,retryHelper,inputHelper,testDataProvider,locatorLoader);

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

            var testDataPath = Path.Combine(basePath, FrameworkConstants.ResourcesFolder, 
                FrameworkConstants.TestDataFolder, "TestData.json");

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

        private async Task<AccountCreationData> LoadTestDataAsync()
        {
            try
            {
                
                var _objPath = new PathHelper();
                var basePath = _objPath.getProjectPath();
                var testDataPath = Path.Combine(basePath, FrameworkConstants.ResourcesFolder, 
                    FrameworkConstants.TestDataFolder, "TestData.json");

                var legacyReader = new TestDataReader(Logger);
                var get = (string key) => legacyReader.GetData(testDataPath, "AccountCreationData", key);

                return new AccountCreationData
                {
                    Product = get("product"),
                    Purpose = get("purpose"),
                    SocietyLoanNo = get("socityloanno"),
                    AppliedDate = get("applieddate"),
                    AppliedAmount = get("appliedamount"),
                    SanctionDate = get("sanctionddate"),
                    SanctionAmount = get("sanctiondamount"),
                    RepaymentType = get("repaymenttype"),
                    RepaymentMode = get("repaymentmode"),
                    ROI = get("roi"),
                    PenalROI = get("penalroi"),
                    IOAROI = get("ioaroi"),
                    GestationPeriodMonths = get("gestationperiodmonths"),
                    LoanPeriodMonths = get("loanperiodmonths")
                };
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to load account creation test data", ex);
                throw;
            }
        }

        [Test]
        [Description("Verify user can create a borrowing account with all mandatory fields")]
        public async Task CreatAccountAsync()
        {
            Assert.That(_accountCreationPage, Is.Not.Null, "AccountCreationPage should be initialized");
            Assert.That(_dashboardPage, Is.Not.Null, "DashboardPage should be initialized");
            Assert.That(_testData, Is.Not.Null, "Test data should be loaded");

            await _accountCreationPage!.CreateBorrowingAccountAsync(_dashboardPage!, _testData!);

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
}
