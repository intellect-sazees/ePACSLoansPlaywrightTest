using ePACSLoans.Core.Interfaces;
using Microsoft.Playwright;
using NUnit.Framework;
using ePACSLoans.Utilities.Common;
using ePACSLoans.Utilities;

namespace ePACSLoans.Core
{
    /// <summary>
    /// Base test class providing common setup and teardown for all tests
    /// </summary>
    public abstract class BaseTest
    {
        protected IPage? Page { get; private set; }
        protected IBrowserContext? Context { get; private set; }
        protected NLog.ILogger Logger { get; private set; }
        protected BrowserManager BrowserManager { get; private set; }

        [SetUp]
        public virtual async Task SetUp()
        {
            try
            {
                Logger = NLog.LogManager.GetCurrentClassLogger();
                BrowserManager = BrowserManager.Instance;

                // Initialize browser if not already initialized
                var config = ConfigurationManager.Instance;
                var browserType = Enum.Parse<BrowserType>(config.Browser, true);
                var videoDir = Path.Combine(
                    FrameworkConstants.TestReportsFolder,
                    FrameworkConstants.TestExecutionVideosFolder
                );

                await BrowserManager.InitializeAsync(browserType, config.Headless, videoDir);

                // Create isolated context for this test
                Context = await BrowserManager.CreateContextAsync();
                Page = await BrowserManager.CreatePageAsync(Context);

                Logger.Info($"Test setup completed for: {TestContext.CurrentContext.Test.Name}");
            }
            catch (Exception ex)
            {
                Logger?.Error($"Test setup failed: {TestContext.CurrentContext.Test.Name}", ex);
                throw;
            }
        }

        [TearDown]
        public virtual async Task TearDown()
        {
            try
            {
                if (Page != null)
                {
                    if (TestContext.CurrentContext.Result.Outcome.Status == NUnit.Framework.Interfaces.TestStatus.Failed)
                    {
                        var screenshotPath = Path.Combine(FrameworkConstants.TestReportsFolder,$"failure_{TestContext.CurrentContext.Test.Name}_{DateTime.Now:yyyyMMdd_HHmmss}.png");
                        await Page.ScreenshotAsync(new PageScreenshotOptions { Path = screenshotPath });
                        Logger.Info($"Failure screenshot saved: {screenshotPath}");
                    }
                }
                if (Context != null)
                {
                    await BrowserManager.CloseContextAsync(Context);
                }

                Logger.Info($"Test teardown completed for: {TestContext.CurrentContext.Test.Name}");
            }
            catch (Exception ex)
            {
                Logger?.Warn($"Test teardown error: {TestContext.CurrentContext.Test.Name  }: {ex.Message}");
            }
        }

        [OneTimeTearDown]
        public virtual async Task OneTimeTearDown()
        {
            try
            {
                AllureReportGenerator.GenerateHtmlReport();
                await BrowserManager.Instance.DisposeAsync();
                Logger?.Info("All tests completed, browser manager disposed");
            }
            catch (Exception ex)
            {
                Logger?.Error("Error during one-time teardown", ex);
            }
        }
    }
}

