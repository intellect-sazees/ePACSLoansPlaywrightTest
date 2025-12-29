using ePACSLoans.Core;
using ePACSLoans.Core.Interfaces;
using ePACSLoans.Models;
using ePACSLoans.Utilities.DataManagement;
using ePACSLoans.Utilities.Helpers;
using Microsoft.Playwright;
namespace ePACSLoans.Modules.Login
{
    public class LoginPage : BasePage
    {
        private readonly IInputValidationHelper _inputHelper;
        private readonly ITestDataProvider _testDataProvider;
        private readonly LoginLocators _locators;
        public LoginPage(IPage page,IWaitHelper waitHelper,NLog.ILogger logger,IRetryHelper retryHelper,IInputValidationHelper inputHelper,ITestDataProvider testDataProvider,Core.Interfaces.ILocatorLoader locatorLoader): base(page, waitHelper, logger, retryHelper)
        {
            _inputHelper = inputHelper ?? throw new ArgumentNullException(nameof(inputHelper));
            _testDataProvider = testDataProvider ?? throw new ArgumentNullException(nameof(testDataProvider));
            _locators = locatorLoader?.LoadLocators<LoginLocators>("LoginPage", MapPropertyToKey) 
                ?? throw new ArgumentNullException(nameof(locatorLoader));
        }
        private static string MapPropertyToKey(string propertyName)
        {
            return propertyName switch
            {
                nameof(LoginLocators.UsernameInput) => "Usernameinput",
                nameof(LoginLocators.PasswordInput) => "PasswordInput",
                nameof(LoginLocators.DateInput) => "DateInput",
                nameof(LoginLocators.CaptchaLabel) => "captchaLabel",
                nameof(LoginLocators.CaptchaInput) => "captchaInput",
                nameof(LoginLocators.LoginButton) => "loginButton",
                nameof(LoginLocators.InvalidCaptchaLabel) => "invalidCaptchaLable",
                nameof(LoginLocators.SignoutMenu) => "signoutMenu",
                nameof(LoginLocators.Signout) => "signout",
                _ => propertyName // Default: use property name as-is
            };
        }
        public async Task<bool> SignInAsync(string url, string username, string password, string date)
        {
            try
            {
                Logger.Info($"Starting sign-in process for user: {username}");
                await NavigateAsync(url);
                var reached = await WaitHelper.WaitForUrlAsync(url);
                if (!reached)
                {
                    Logger.Warn($"Failed to reach URL: {url}");
                    await TakeScreenshotAsync("navigation_failure");
                    return false;
                }
                Logger.Debug("Successfully navigated to login page");
                await FillUsernameAsync(username);
                await FillPasswordAsync(password);
                await FillDateAsync(date);
                var captchaSolved = await SolveCaptchaAsync();
                if (!captchaSolved)
                {
                    Logger.Warn("Failed to solve captcha");
                    await TakeScreenshotAsync("captcha_failure");
                    return false;
                }
                await ClickAsync(_locators.LoginButton);
                await WaitHelper.WaitForTimeoutAsync(2000);
                if (await IsVisibleAsync(_locators.InvalidCaptchaLabel))
                {
                    Logger.Warn("Invalid captcha detected, retrying sign-in");
                    return await RetryHelper.ExecuteWithRetryAsync(() => SignInAsync(url, username, password, date),maxAttempts: 2,delayMs: 1000);
                }
                Logger.Info($"Successfully signed in for user: {username}");
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error($"Sign-in failed for user: {username}", ex);
                await TakeScreenshotAsync("signin_failure");
                throw;
            }
        }
        private async Task FillUsernameAsync(string username)
        {
            try
            {
                Logger.Debug($"Filling username: {username}");
                var usernameInput = Page.Locator(_locators.UsernameInput);
                var filled = await _inputHelper.FillTextBoxValueAsync(usernameInput, username);
                if (!filled)
                {
                    throw new InvalidOperationException("Failed to fill username field");
                }

                var isTextPresent = await WaitHelper.WaitForTextAsync(_locators.UsernameInput, username, 1000);
                if (!isTextPresent)
                {
                    Logger.Warn("Username text not present after filling");
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to fill username", ex);
                throw;
            }
        }
        private async Task FillPasswordAsync(string password)
        {
            try
            {
                Logger.Debug("Filling password field");
                var passwordInput = Page.Locator(_locators.PasswordInput);
                var filled = await _inputHelper.FillTextBoxValueAsync(passwordInput, password);
                
                if (!filled)
                {
                    throw new InvalidOperationException("Failed to fill password field");
                }

                var isPasswordPresent = await WaitHelper.WaitForTextAsync(_locators.PasswordInput, password, 1000);
                if (!isPasswordPresent)
                {
                    Logger.Warn("Password text not present after filling");
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to fill password", ex);
                throw;
            }
        }
        private async Task FillDateAsync(string date)
        {
            try
            {
                Logger.Debug($"Filling date: {date}");
                var formattedDate = await _inputHelper.ConvertToDDMMYYYYAsync(date);
                if (string.IsNullOrEmpty(formattedDate))
                {
                    throw new InvalidOperationException($"Failed to convert date: {date}");
                }
                await FillAsync(_locators.DateInput, formattedDate);
                var isDatePresent = await WaitHelper.WaitForTextAsync(_locators.DateInput, formattedDate, 1000);
                if (!isDatePresent)
                {
                    Logger.Warn("Date not present after filling");
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to fill date: {date}", ex);
                throw;
            }
        }
        private async Task<bool> SolveCaptchaAsync()
        {
            try
            {
                Logger.Debug("Attempting to solve captcha");
                var captchaElement = await Page.QuerySelectorAsync(_locators.CaptchaLabel);
                
                if (captchaElement == null)
                {
                    Logger.Warn("Captcha element not found");
                    return false;
                }
                var captchaSolver = new Utilities.HandleCaptcha(captchaElement);
                var solved = await captchaSolver.SolveCaptchaAsync();
                var solvedText = captchaSolver.Text ?? string.Empty;
                if (string.IsNullOrEmpty(solvedText))
                {
                    Logger.Warn("Failed to extract captcha text");
                    return false;
                }
                Logger.Debug($"Captcha solved: {solvedText}");
                await FillAsync(_locators.CaptchaInput, solvedText);
                var captchaInput = Page.Locator(_locators.CaptchaInput);
                var isFillCaptcha = await _inputHelper.FillTextBoxValueAsync(captchaInput, solvedText);
                var isCaptchaPresent = await WaitHelper.WaitForTextAsync(_locators.CaptchaInput, solvedText, null);
                
                if (!isFillCaptcha || !isCaptchaPresent)
                {
                    Logger.Warn("Captcha text not properly filled");
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to solve captcha", ex);
                return false;
            }
        }
        public async Task SignOutAsync()
        {
            try
            {
                Logger.Info("Starting sign-out process");
                await ClickAsync(_locators.SignoutMenu);
                var isSignoutMenuClickable = await WaitHelper.WaitForElementClickableAsync(_locators.SignoutMenu);
                
                if (!isSignoutMenuClickable)
                {
                    Logger.Warn("Signout menu not clickable");
                }
                await WaitHelper.WaitForTimeoutAsync(500);
                await ClickAsync(_locators.Signout);
                var isSignOutClickable = await WaitHelper.WaitForElementClickableAsync(_locators.Signout);
                if (!isSignOutClickable)
                {
                    Logger.Warn("Signout button not clickable");
                }
                Logger.Info("Successfully signed out");
            }
            catch (Exception ex)
            {
                Logger.Error("Sign-out failed", ex);
                await TakeScreenshotAsync("signout_failure");
                throw;
            }
        }
        protected override async Task<bool> ValidatePageLoadedAsync()
        {
            try
            {
                var requiredElements = new[]
                {
                    _locators.UsernameInput,
                    _locators.PasswordInput,
                    _locators.LoginButton
                };
                var isValid = await ValidateElementsVisibleAsync(requiredElements, timeout: 5000, requireAll: true);
                if (isValid)
                {
                    Logger.Debug("Login page loaded successfully - all required elements are visible");
                    return true;
                }
                Logger.Warn("Login page validation failed - one or more required elements are not visible");
                return false;
            }
            catch (Exception ex)
            {
                Logger.Warn($"Login page validation error: {ex.Message}");
                return false;
            }
        }
    }
}
