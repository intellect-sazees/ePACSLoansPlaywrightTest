using IntellectPlaywrightTest.Core;
using IntellectPlaywrightTest.Core.Interfaces;
using IntellectPlaywrightTest.Models;
using IntellectPlaywrightTest.Modules.Dashboard;
using IntellectPlaywrightTest.Modules.FAS.Components;
using IntellectPlaywrightTest.Modules.Membership;
using IntellectPlaywrightTest.Utilities.DataManagement;
using IntellectPlaywrightTest.Utilities.Helpers;
using Microsoft.Playwright;
using static System.Runtime.InteropServices.JavaScript.JSType;
namespace IntellectPlaywrightTest.Modules.FAS
{
    public class TranactionReceptsPage : BasePage
    {
        private readonly ITestDataProvider _testDataProvider;
        private readonly TrasactionReceptLocaters _locators;
        private readonly TranactionReceptFormComponent _formComponent;
        ShareDepositPage shareDepositPage;
        public TranactionReceptsPage(IPage page,IWaitHelper waitHelper,NLog.ILogger logger,IRetryHelper retryHelper,IInputValidationHelper inputHelper,ITestDataProvider testDataProvider,Core.Interfaces.ILocatorLoader locatorLoader): base(page, waitHelper, logger, retryHelper)
        {
            _testDataProvider = testDataProvider ?? throw new ArgumentNullException(nameof(testDataProvider));
            _locators = locatorLoader?.LoadLocators<TrasactionReceptLocaters>("TransactionReceiptPage", MapPropertyToKey)
                ?? throw new ArgumentNullException(nameof(locatorLoader));
            _formComponent = new TranactionReceptFormComponent(page,waitHelper,logger,retryHelper,inputHelper ?? throw new ArgumentNullException(nameof(inputHelper)),_locators);
            //shareDepositPage = new ShareDepositPage(page,waitHelper,logger,retryHelper,inputHelper,testDataProvider,locatorLoader);
        }
        private static string MapPropertyToKey(string propertyName)
        {

            return propertyName switch
            {
                nameof(TrasactionReceptLocaters.ModuleId)=> "ShareReceptModule",
                nameof(TrasactionReceptLocaters.FormId)=> "ShareReceptForm",
                nameof(TrasactionReceptLocaters.AdmissionNo) => "admissionNo",
                nameof(TrasactionReceptLocaters.SearchIcon)=> "searchicon",
                nameof(TrasactionReceptLocaters.Product) => "product",
                _ => propertyName
            };
        }
        public async Task<(string module,string form)> GetForm()
        {
           var (Module,Form) = await shareDepositPage.GetShareDepositFormModulId();
            return (Module, Form); 
        }
        public  async Task ShareTransactionRecept(string moduleId,string formId)
        {
            try
            {
                var testData = await _testDataProvider!.GetTestDataFromFileAsync<TransactionReceptData>(
                  GetTransactionReceiptData(),
                  "TransactioReceptData");

                _formComponent.FillAsync(testData);
            }
            catch (Exception ex)
            {

            }
        }
        private string GetTransactionReceiptData()
        {
            var pathHelper = new PathHelper();
            var basePath = pathHelper.getProjectPath();

            return Path.Combine(basePath, FrameworkConstants.ResourcesFolder,"TestData", "TestData.json");
        }
    }
}
