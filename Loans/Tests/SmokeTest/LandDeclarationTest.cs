using Allure.NUnit;
using ePACSLoans.Models;
using ePACSLoans.Modules.Loans;
using ePACSLoans.Core;
using ePACSLoans.Modules.Dashboard;
using ePACSLoans.Modules.Login;
using ePACSLoans.Utilities.Common;
using ePACSLoans.Utilities.DataManagement;
using ePACSLoans.Utilities.Helpers;
using ePACSLoans.Utilities.LocatorManagement;
using NLog;

namespace ePACSLoans.Tests.SmokeTest
{
    [TestFixture]
    [Category("Smoke")]
    [AllureNUnit]
    [Obsolete("This test class is deprecated. Use Land Declaration instead.")]
    public class LandDeclarationTest :BaseTest
    {
        private LoginPage? _loginPage;
        private DashboardPage? _dashboardPage;
        private LandDeclarationPage? _landDeclarationPage;
        private LandDeclarationData? _testData;
        public override async Task SetUp()
        {
            await base.SetUp();
            try
            {
                // Initialize dependencies
                NLog.ILogger logger = LogManager.GetLogger("LandDeclaeationTest");
                var waitHelper = new WaitHelper(Page!);
                var retryHelper = new RetryHelper(logger);
                var inputHelper = new InputValidationHelper(logger);
                var testDataProvider = new TestDataReader(logger);
                var locatorLoader = new LocatorLoader(testDataProvider, logger);
                // Create page objects with dependency injection
                _loginPage = new LoginPage(Page!, waitHelper, logger, retryHelper, inputHelper, testDataProvider, locatorLoader);
                _dashboardPage = new DashboardPage(Page!, waitHelper, logger, retryHelper, testDataProvider, locatorLoader);
                _landDeclarationPage = new LandDeclarationPage(Page!, waitHelper, logger, retryHelper, inputHelper, testDataProvider, locatorLoader);
                // Load test data _testData
                _testData=await LoadTestDataAsync();
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
                await _loginPage!.SignInAsync(loginData.Url, loginData.Username, loginData.Password, loginData.LoginDate);
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
            LoginData objLogindata = new LoginData();
            objLogindata.Url = get("url");
            objLogindata.Username = get("username");
            objLogindata.Password = get("password");
            objLogindata.LoginDate = get("loginDate");
            return objLogindata;
        }
        private async Task<LandDeclarationData> LoadTestDataAsync()
        {
            try
            {
                var _objPath = new PathHelper();
                var basePath = _objPath.getProjectPath();
                var testDataPath = Path.Combine(basePath, FrameworkConstants.ResourcesFolder, FrameworkConstants.TestDataFolder, "TestData.json");
                var legacyReader = new TestDataReader(Logger);
                var get = (string key) => legacyReader.GetData(testDataPath, "LandDeclarationPage", key);
                LandDeclarationData obj = new LandDeclarationData()
                {
                    AdmissionNo = get("AdmissionNo"),
                    Product = get("Product"),
                    Crop=get("Crop"),
                    Village=get("Village"),
                    SurveyNo=get("SurveyNo"),
                    //DrawalAmount = get("DrawalAmount"),
                    //VoucherType = get("VoucherType"),
                };
                return obj;
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to load land declaration test data", ex);
                throw;
            }
        }
        [Test]
        [Description("Verify user can declare land details with all mandatory fields")]
        public async Task TestLandDeclarationAsync()
        {
            await _landDeclarationPage.DeclareLandDetailswithMandatoryfieldsAsync(_dashboardPage!, _testData!);
        }
    }
}
