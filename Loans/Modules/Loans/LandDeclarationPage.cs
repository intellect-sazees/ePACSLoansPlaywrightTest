using ePACSLoans.Modules.Loans.Components;
using ePACSLoans.Core;
using ePACSLoans.Core.Interfaces;
using ePACSLoans.Modules.Dashboard;
using Microsoft.Playwright;
using ePACSLoans.Utilities.Helpers;
using ePACSLoans.Models.Locaters;
using ePACSLoans.Models.Data;

namespace ePACSLoans.Modules.Loans
{
    public class LandDeclarationPage : BasePage
    {
        private readonly ITestDataProvider _testDataProvider;
        private readonly LandDeclarationLocaters _locators;
        private readonly LandDeclarationFormComponents _formComponent;
        public LandDeclarationPage(IPage page,IWaitHelper waitHelper,NLog.ILogger logger,IRetryHelper retryHelper,IInputValidationHelper inputHelper,ITestDataProvider testDataProvider,ILocatorLoader locatorLoader) : base(page, waitHelper, logger, retryHelper)
        {
            _testDataProvider = testDataProvider ?? throw new ArgumentNullException(nameof(testDataProvider));
            // Load locators using LocatorLoader service
            _locators = locatorLoader?.LoadLocators<LandDeclarationLocaters>("LandDeclarationPage", MapPropertyToKey)?? throw new ArgumentNullException(nameof(locatorLoader));
            // Initialize form component with dependencies
            _formComponent = new LandDeclarationFormComponents(page,waitHelper,logger,retryHelper,inputHelper ?? throw new ArgumentNullException(nameof(inputHelper)),_locators);
        }
        private static string MapPropertyToKey(string propertyName)
        {
            return propertyName switch
            {
                nameof(LandDeclarationLocaters.LandDeclarationIcon) => "LandDeclarationIcon",
                nameof(LandDeclarationLocaters.AdmissionNoInput)=> "AdmissionNo",
                nameof(LandDeclarationLocaters.Searchbtn)=> "iconsearch",
                nameof(LandDeclarationLocaters.AddBtn)=> "AddLandDetails",
                nameof(LandDeclarationLocaters.ProductInput)=> "Product",
                nameof(LandDeclarationLocaters.CropInput)=> "Crop",
                nameof(LandDeclarationLocaters.VillageInput)=> "Village",
                nameof(LandDeclarationLocaters.SurveyNoInput)=> "SurveyNo",
                nameof(LandDeclarationLocaters.TotalLandInAcersInput)=> "TotalLandInAcers",
                nameof(LandDeclarationLocaters.TotalLandInCentsInpu)=> "TotalLandInCents",
                nameof(LandDeclarationLocaters.CultivatableLandInAcersInput)=> "CultivatableLandInAcers",
                nameof(LandDeclarationLocaters.CultivatableLandInCentsInput)=> "CultivatableLandInCents",
                nameof(LandDeclarationLocaters.TotalAvailableLandInAcersInput)=> "TotalAvailableLandInAcers",
                nameof(LandDeclarationLocaters.TotalAvailableLandInCentsInput)=> "TotalAvailableLandInCents",
                nameof(LandDeclarationLocaters.LandDeclaredAcresInput)=> "LandDeclaredAcres",
                nameof(LandDeclarationLocaters.LandDeclaredCentsInput)=> "LandDeclaredCents",
                nameof(LandDeclarationLocaters.LandValuePerAcreInput)=> "LandValuePerAcre",
                nameof(LandDeclarationLocaters.SaveBtn)=> "Save",
                nameof(LandDeclarationLocaters.Savepopup)=> "SavePopup",
                nameof(LandDeclarationLocaters.Okbtn)=> "OkButton",
                nameof(LandDeclarationLocaters.AdnissionValidationAlert)=> "AmissionnoValidation",
                nameof(LandDeclarationLocaters.OKAlt)=> "Okbtn",
                _ => propertyName // Default: use property name as-is
            };
        }
        public async Task DeclareLandDetailswithMandatoryfieldsAsync(DashboardPage dashboardPage,LandDeclarationData data)
        {
            await NavigateToLandDeclarationAsync(dashboardPage);
            await _formComponent.FillAsync(data, _locators);
            var modalBody = Page.Locator(_locators.Savepopup);
            await modalBody.WaitForAsync(new LocatorWaitForOptions{State = WaitForSelectorState.Visible,Timeout = 10000});
            string actualMessage = (await modalBody.InnerTextAsync()).Trim();
            AssertHelper.AssertTextContains(actualMessage,"Data saved successfully","Success Popup Message");
            await Page.Locator(_locators.Okbtn).ClickAsync();
            await dashboardPage.NavigateToDashboardAsync();
        }
        public async Task AdmissionNoValidationAsync(DashboardPage dashboardPage, LandDeclarationData data)
        {
            await NavigateToLandDeclarationAsync(dashboardPage);
            string custno = data.AdmissionNo;
            custno = "1234567";
            await _formComponent.FillAdmissionNoAsync(custno);
            await _formComponent.ClickSearchBtn();
            string actualAlert=await Page.Locator(_locators.AdnissionValidationAlert).InnerTextAsync();
            bool isPass=AssertHelper.AssertTextContains(actualAlert, "Admission Number Does Not Exist", "Validation Popup Message");
            if (!isPass)
            {
                await TakeScreenshotAsync("ValidationFaild");
            }
            await _formComponent.OkAltAsync();
            await dashboardPage.NavigateToDashboardAsync();
            await dashboardPage.HandleDashboardAlertAsync();
        }
        public async Task ValidateAvailableLandAsync(DashboardPage dashboardPage, LandDeclarationData data)
        {
            await NavigateToLandDeclarationAsync(dashboardPage);
            await _formComponent.FillAdmissionNoAsync(data.AdmissionNo);
            await _formComponent.ClickSearchBtn();
            await _formComponent.ClickAddAsync();
            await _formComponent.FillProductAsync(data.Product);
            await _formComponent.FillCropAsync(data.Crop);
            await _formComponent.FillVillageAsync(data.Village);
            await _formComponent.FillSurveyNoAsync(data.SurveyNo);
            string s=await _formComponent.GetAvailableLandAcersAsync(_locators);
            int i = Convert.ToInt32(s);
            i = i +1;
            string Acers=i.ToString();
            string a = await _formComponent.GetAvailableLandCentsAsync(_locators);
            int j=Convert.ToInt32(a);
            j = j +1;
            string Cents=j.ToString();
            await _formComponent.FillDeclaredLandInAcersAsync(Acers);
            await _formComponent.FillDeclaredLandInCentsAsync(Cents);
            string actualAlert = await Page.Locator("//div[@class='sweet-alert showSweetAlert visible']/h2").InnerTextAsync();
            bool isPass = AssertHelper.AssertTextContains(actualAlert, "Declared Land value should not be greater than Available LandAvailable Land", "Validation Popup Message");
        }
        private async Task NavigateToLandDeclarationAsync(DashboardPage dashboardPage)
        {
            try
            {
                Logger.Debug("Navigating to  Land Declaration page");
                await dashboardPage.HandleDashboardAlertAsync();
                await dashboardPage.NaviagateToLoansAsync();
                await ClickAsync(_locators.LandDeclarationIcon);
                await WaitHelper.WaitForPageLoadAsync();
                Logger.Debug("Successfully navigated to  Land Declaration page");
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to navigate to  Land Declaration page", ex);
                throw;
            }
        }
    }
}