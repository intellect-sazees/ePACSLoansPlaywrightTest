using ePACSLoans.Core.Interfaces;
using ePACSLoans.Modules.Dashboard;
using ePACSLoans.Modules.Login;
using ePACSLoans.Utilities.Common;
using ePACSLoans.Utilities.DataManagement;
using ePACSLoans.Utilities.Helpers;
using ePACSLoans.Utilities.LocatorManagement;
using Microsoft.Playwright;
using NLog;

namespace ePACSLoans.Core
{
    /// <summary>
    /// Factory for creating page objects with dependency injection
    /// Centralizes page object creation and reduces boilerplate code in tests
    /// </summary>
    public class PageFactory
    {
        private readonly PageFactoryContext _context;

        /// <summary>
        /// Initializes a new instance of PageFactory with the provided context
        /// </summary>
        public PageFactory(PageFactoryContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Generic method to create ANY page object that inherits from BasePage
        /// Uses reflection to automatically resolve dependencies and instantiate pages
        /// 
        /// ⚠️ IMPORTANT: You DON'T need to add a specific method for each page!
        /// This generic method works for ALL pages automatically.
        /// 
        /// The specific methods below (CreateLoginPage, etc.) are OPTIONAL convenience methods
        /// that provide better IntelliSense. You can delete them and just use CreatePage&lt;T&gt;().
        /// </summary>
        /// <typeparam name="T">Type of page object to create (must inherit from BasePage)</typeparam>
        /// <returns>Instance of the requested page object</returns>
        /// <example>
        /// // Works for ANY page without needing to add a method:
        /// var loginPage = pageFactory.CreatePage&lt;LoginPage&gt;();
        /// var dashboardPage = pageFactory.CreatePage&lt;DashboardPage&gt;();
        /// var myNewPage = pageFactory.CreatePage&lt;MyNewPage&gt;(); // Works automatically!
        /// var anyPage = pageFactory.CreatePage&lt;AnyPage&gt;(); // No method needed!
        /// </example>
        public T CreatePage<T>() where T : BasePage
        {
            var pageType = typeof(T);

            // Find the constructor that accepts IPage as first parameter
            var constructors = pageType.GetConstructors();
            var constructor = constructors.FirstOrDefault(c => 
                c.GetParameters().Length >= 4 && // At least BasePage parameters
                c.GetParameters()[0].ParameterType == typeof(IPage));

            if (constructor == null)
            {
                throw new InvalidOperationException(
                    $"No suitable constructor found for {pageType.Name}. " +
                    "Page objects must have a constructor that accepts IPage, IWaitHelper, ILogger, and IRetryHelper.");
            }

            var parameters = constructor.GetParameters();
            var args = new object[parameters.Length];

            // Automatically map dependencies based on parameter types
            for (int i = 0; i < parameters.Length; i++)
            {
                var paramType = parameters[i].ParameterType;
                if (paramType == typeof(IPage))
                    args[i] = _context.Page;
                else if (paramType == typeof(IWaitHelper))
                    args[i] = _context.WaitHelper;
                else if (paramType == typeof(NLog.ILogger))
                    args[i] = _context.Logger;
                else if (paramType == typeof(IRetryHelper))
                    args[i] = _context.RetryHelper;
                else if (paramType == typeof(IInputValidationHelper))
                    args[i] = _context.InputHelper;
                else if (paramType == typeof(ITestDataProvider))
                    args[i] = _context.TestDataProvider;
                else if (paramType == typeof(Core.Interfaces.ILocatorLoader))
                    args[i] = _context.LocatorLoader;
                else
                    throw new InvalidOperationException(
                        $"Cannot resolve parameter '{parameters[i].Name}' of type '{paramType.Name}' for {pageType.Name}. " +
                        $"Available dependencies: IPage, IWaitHelper, ILogger, IRetryHelper, IInputValidationHelper, ITestDataProvider, ILocatorLoader");
            }

            return (T)Activator.CreateInstance(pageType, args)!;
        }

        // ============================================================================
        // OPTIONAL CONVENIENCE METHODS (You can delete these if you prefer)
        // ============================================================================
        // These methods are optional and just call CreatePage&lt;T&gt;() internally.
        // They provide better IntelliSense but are NOT required.
        // You can use CreatePage&lt;LoginPage&gt;() instead of CreateLoginPage().
        // ============================================================================

        /// <summary>
        /// [OPTIONAL] Convenience method for LoginPage
        /// You can use CreatePage&lt;LoginPage&gt;() instead - this is just for better IntelliSense
        /// </summary>
        //public LoginPage CreateLoginPage()
        //{
        //    return CreatePage<LoginPage>();
        //}

        /// <summary>
        /// [OPTIONAL] Convenience method for DashboardPage
        /// You can use CreatePage&lt;DashboardPage&gt;() instead
        /// </summary>
        //public DashboardPage CreateDashboardPage()
        //{
        //    return CreatePage<DashboardPage>();
        //}

        /// <summary>
        /// [OPTIONAL] Convenience method for AccountCreationPage
        /// You can use CreatePage&lt;AccountCreationPage&gt;() instead
        /// </summary>
        //public AccountCreationPage CreateAccountCreationPage()
        //{
        //    return CreatePage<AccountCreationPage>();
        //}
    }

    /// <summary>
    /// Context class that holds all dependencies needed for page object creation
    /// This reduces the need to pass multiple parameters to the factory
    /// </summary>
    public class PageFactoryContext
    {
        public IPage Page { get; }
        public IWaitHelper WaitHelper { get; }
        public NLog.ILogger Logger { get; }
        public IRetryHelper RetryHelper { get; }
        public IInputValidationHelper InputHelper { get; }
        public ITestDataProvider TestDataProvider { get; }
        public Core.Interfaces.ILocatorLoader LocatorLoader { get; }

        /// <summary>
        /// Initializes a new instance of PageFactoryContext with all required dependencies
        /// </summary>
        public PageFactoryContext(
            IPage page,
            IWaitHelper waitHelper,
            NLog.ILogger logger,
            IRetryHelper retryHelper,
            IInputValidationHelper inputHelper,
            ITestDataProvider testDataProvider,
            Core.Interfaces.ILocatorLoader locatorLoader)
        {
            Page = page ?? throw new ArgumentNullException(nameof(page));
            WaitHelper = waitHelper ?? throw new ArgumentNullException(nameof(waitHelper));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            RetryHelper = retryHelper ?? throw new ArgumentNullException(nameof(retryHelper));
            InputHelper = inputHelper ?? throw new ArgumentNullException(nameof(inputHelper));
            TestDataProvider = testDataProvider ?? throw new ArgumentNullException(nameof(testDataProvider));
            LocatorLoader = locatorLoader ?? throw new ArgumentNullException(nameof(locatorLoader));
        }

        /// <summary>
        /// Creates a PageFactoryContext with all dependencies initialized from a test context
        /// This is a convenience method for test setup
        /// </summary>
        public static PageFactoryContext CreateFromTest(
            IPage page,
            NLog.ILogger logger)
        {
            var waitHelper = new WaitHelper(page);
            var retryHelper = new RetryHelper(logger);
            var inputHelper = new InputValidationHelper(logger);
            var testDataProvider = new TestDataReader(logger);
            var locatorLoader = new LocatorLoader(testDataProvider, logger);

            return new PageFactoryContext(
                page,
                waitHelper,
                logger,
                retryHelper,
                inputHelper,
                testDataProvider,
                locatorLoader);
        }
    }
}

