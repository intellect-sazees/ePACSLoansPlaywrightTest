using ePACSLoans.Core.Components;
using ePACSLoans.Core.Interfaces;
using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ePACSLoans.Utilities.Helpers;
using ePACSLoans.Models.Locaters;
using ePACSLoans.Models.Data;
using ePACSLoans.Models;
using ePACSLoans.Models.Data;

namespace ePACSLoans.Modules.Loans.Components
{
    public class LandDeclarationFormComponents : BaseFormComponent<LandDeclarationData, LandDeclarationLocaters>
    {
        private IPage page;
        private IWaitHelper waitHelper;
        private NLog.ILogger logger;
        private IRetryHelper retryHelper;
        private IInputValidationHelper _inputHelper;
        private LandDeclarationLocaters _locators;
        public LandDeclarationFormComponents(IPage page, IWaitHelper waitHelper, NLog.ILogger logger, IRetryHelper retryHelper, IInputValidationHelper inputHelper, LandDeclarationLocaters locators) : base(page, waitHelper, logger, retryHelper)
        {
            _inputHelper = inputHelper ?? throw new ArgumentNullException(nameof(inputHelper));
            _locators = locators ?? throw new ArgumentNullException(nameof(locators));
        }
        public override async Task FillAsync(LandDeclarationData landDeclarationData, LandDeclarationLocaters datalocater)
        {
            try
            {
                Logger.Info("Starting to fill Land Declaration form");
                await FillAdmissionNoAsync(landDeclarationData.AdmissionNo);
                await ClickSearchBtn();
                await ClickAddAsync();
                await FillProductAsync(landDeclarationData.Product);
                await FillCropAsync(landDeclarationData.Crop);
                await FillVillageAsync(landDeclarationData.Village);
                await FillSurveyNoAsync(landDeclarationData.SurveyNo);
                var AvalAcers=await GetAvailableLandAcersAsync(datalocater);
                var AvalCents=await GetAvailableLandCentsAsync(datalocater);
                await FillDeclaredLandInAcersAsync(AvalAcers);
                await FillDeclaredLandInCentsAsync(AvalCents);
                await FillLandValuePerAcerAsync(landDeclarationData.LandValuePerAcer);
                await ClickSaveAsync();
                Logger.Info("Land DeclarationData form filled successfully");
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to fill Land DeclarationData form :{ex}");
                throw;
            }
        }
        public async Task FillAdmissionNoAsync(string admissionNo)
        {
            var input = Page.Locator(_locators.AdmissionNoInput);
            var filled = await _inputHelper.FillTextBoxValueAsync(input, admissionNo);
            if (!filled)
            {
                throw new InvalidOperationException($"Failed to fill Admission No: {admissionNo}");
            }
            Logger.Debug($"Filled Admission No: {admissionNo}");
        }
        public async Task ClickSearchBtn()
        {
            await Page.ClickAsync(_locators.Searchbtn);
        }
        public async Task ClickAddAsync()
        {
            await Page.ClickAsync(_locators.AddBtn);
        }
        public async Task FillProductAsync(string product)
        {
            await _inputHelper.SelectDropdownOptionAsync(Page.Locator(_locators.ProductInput), product, SelectBy.Label);
            Logger.Debug($"Selected product: {product}");
        }
        public async Task FillCropAsync(string crop)
        {
            await _inputHelper.SelectDropdownOptionAsync(Page.Locator(_locators.CropInput), crop, SelectBy.Label);
            Logger.Debug($"Selected Crop: {crop}");
        }
        public async Task FillVillageAsync(string village)
        {
            await _inputHelper.SelectDropdownOptionAsync(Page.Locator(_locators.VillageInput), village, SelectBy.Index);
            Logger.Debug($"Selected Village: {village}");
        }
        public async Task FillSurveyNoAsync(string surveyno)
        {
            await _inputHelper.SelectDropdownOptionAsync(Page.Locator(_locators.SurveyNoInput), surveyno, SelectBy.Index);
            Logger.Debug($"Survey no: {surveyno}");
        }
        public async Task<string> GetAvailableLandAcersAsync(LandDeclarationLocaters landDeclarationLocaters)
        {
            var a = Page.Locator(landDeclarationLocaters.TotalAvailableLandInAcersInput);
            var value = await _inputHelper.GetValueinTextBoxAsync(a);
            return value;
        }
        public async Task<string>GetAvailableLandCentsAsync(LandDeclarationLocaters landDeclarationLocaters)
        {
            var a = Page.Locator(landDeclarationLocaters.TotalAvailableLandInCentsInput);
            var value = await _inputHelper.GetValueinTextBoxAsync(a);
            return value;
        }
        public async Task FillDeclaredLandInAcersAsync(string diclandinacers)
        {
            var input = Page.Locator(_locators.LandDeclaredAcresInput);
            var filled = await _inputHelper.FillTextBoxValueAsync(input, diclandinacers);
            if (!filled)
            {
                throw new InvalidOperationException($"Failed to fill Land in acers: {diclandinacers}");
            }
            Logger.Debug($"Failed to fill Land in acers:{diclandinacers}");
        }
        public async Task FillDeclaredLandInCentsAsync(string diclandincents)
        {
            var input = Page.Locator(_locators.LandDeclaredCentsInput);
            var filled = await _inputHelper.FillTextBoxValueAsync(input, diclandincents);
            if (!filled)
            {
                throw new InvalidOperationException($"Failed to fill Land in cents: {diclandincents}");
            }
            Logger.Debug($"Failed to fill Land in cents:{diclandincents}");
        }
        public async Task FillLandValuePerAcerAsync(string landvalueperacer)
        {
            var input = Page.Locator(_locators.LandValuePerAcreInput);
            var filled = await _inputHelper.FillTextBoxValueAsync(input, landvalueperacer);
            if (!filled)
            {
                throw new InvalidOperationException($"Failed to fill Landvalueperacer: {landvalueperacer}");
            }
            Logger.Debug($"Filled landvalue per acer: {landvalueperacer}");
        }
        public async Task ClickSaveAsync()
        {
            await Page.Locator(_locators.SaveBtn).ClickAsync();
        }
        public async Task OkAltAsync()
        {
            await Page.Locator(_locators.OKAlt).ClickAsync();
        }
    }
}