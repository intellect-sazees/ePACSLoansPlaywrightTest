
using NUnit.Framework;
using System.IO;
using System.Threading.Tasks;
using IntellectPlaywrightTest.Utilities;
using Newtonsoft.Json;
using IntellectPlaywrightTest.Modules.Login;
using IntellectPlaywrightTest.Core;
using IntellectPlaywrightTest.Utilities.Helpers;
using IntellectPlaywrightTest.Utilities.Common;
using IntellectPlaywrightTest.Utilities.DataManagement;
using IntellectPlaywrightTest.Utilities.LocatorManagement;
using IntellectPlaywrightTest.Models;
using NLog;

namespace IntellectPlaywrightTest.Tests.SetUps
{
    /// <summary>
    /// Legacy login test class - DEPRECATED
    /// Please use LoginFunctionalTests instead
    /// </summary>
    [TestFixture]
    [Obsolete("This test class is deprecated. Use LoginFunctionalTests instead.")]
    public class LoginTest : BaseTest
    {
        private LoginPage? _objLogin;
        private LoginData logindata = new LoginData();
        public override async Task SetUp()
        {
            await base.SetUp();

            try
            {
                // Initialize dependencies
                NLog.ILogger logger = LogManager.GetLogger("LoginTest");
                var waitHelper = new WaitHelper(Page!);
                var retryHelper = new RetryHelper(logger);
                var inputHelper = new InputValidationHelper(logger);
                var testDataProvider = new TestDataReader(logger);
                var locatorLoader = new LocatorLoader(testDataProvider, logger);

                // Create LoginPage with dependency injection
                _objLogin = new LoginPage(Page!,waitHelper,logger,retryHelper,inputHelper,testDataProvider,locatorLoader);

                // Load test data
                LoadTestData();

                // Sign in
                await _objLogin.SignInAsync(logindata.Url, logindata.Username, logindata.Password,logindata.LoginDate);
            }
            catch (Exception ex)
            {
                Logger.Error($"Test setup failed: {TestContext.CurrentContext.Test.Name}", ex);
                throw;
            }
        }
        private void LoadTestData()
        {
            var _objPath = new PathHelper();
            var projectPath = _objPath.getProjectPath();
            var TestDataPath = Path.Combine(projectPath, "Resources", "TestData", "TestData.json");
            var _objAccessInputsFromJSON = new AccessInputsFromJSON();
            Func<string, string> get = key => _objAccessInputsFromJSON.GetData(TestDataPath, "Logincredantional", key);
            logindata.Url = get("url");
            logindata.Username = get("username");
            logindata.Password = get("password");
            logindata.LoginDate = get("loginDate");
        }
        [TearDown]
        public override async Task TearDown()
        {
            try
            {
                if (_objLogin != null)
                {
                    await _objLogin.SignOutAsync();
                }
                AllureReportGenerator.GenerateHtmlReport();
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
