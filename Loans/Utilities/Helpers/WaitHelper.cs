using Microsoft.Playwright;
using System;
using System.Threading.Tasks;
using ePACSLoans.Core.Interfaces;
using ePACSLoans.Core;

namespace ePACSLoans.Utilities.Helpers
{
    /// <summary>
    /// Provides explicit wait strategies for element interactions.
    /// Implements IWaitHelper interface for dependency injection support.
    /// </summary>
    public class WaitHelper : IWaitHelper
    {
        private readonly IPage _page;
        private readonly int _defaultTimeout;

        public WaitHelper(IPage page)
        {
            _page = page ?? throw new ArgumentNullException(nameof(page));
            _defaultTimeout = FrameworkConstants.DefaultTimeout;
        }
        public async Task<bool> WaitForElementVisibleAsync(string selector, int? timeout = null)
        {
            try
            {
                var timeoutMs = timeout ?? _defaultTimeout;
                await _page.WaitForSelectorAsync(selector, new PageWaitForSelectorOptions{State = WaitForSelectorState.Visible,Timeout = timeoutMs});
                return true;
            }
            catch (Exception ex) { return false; }
        }
        public async Task<bool> WaitForElementClickableAsync(string selector, int? timeout = null)
        {
            try
            {
                var timeoutMs = timeout ?? _defaultTimeout;
                await WaitForElementVisibleAsync(selector, timeoutMs);
                await _page.Locator(selector).WaitForAsync(new LocatorWaitForOptions{State = WaitForSelectorState.Attached,Timeout = timeoutMs});
                return true;
            }
            catch (Exception ex) { return false; }
        }
        public async Task<bool> WaitForElementToDisappearAsync(string selector, int? timeout = null)
        {
            try
            {
                var timeoutMs = timeout ?? _defaultTimeout;
                await _page.WaitForSelectorAsync(selector, new PageWaitForSelectorOptions{State = WaitForSelectorState.Hidden,Timeout = timeoutMs });
                return true;
            } 
            catch (Exception ex) { return false; }
        }
        public async Task<bool> WaitForTextAsync(string selector, string text, int? timeout = null)
        {
            try
            {
                var timeoutMs = timeout ?? _defaultTimeout;
                var locator = _page.Locator(selector);
                await locator.WaitForAsync(new() { Timeout = timeoutMs });
                var elementHandle = await locator.ElementHandleAsync() ?? throw new Exception($"Element not found: {selector}");
                await _page.WaitForFunctionAsync(@"(data) => {const el = data.element;const expected = data.expected;if (!el) return false;const value = el.value ?? el.textContent ?? '';return value.includes(expected);}",
                    new { element = elementHandle, expected = text },   // <-- CORRECT ARG FORMAT
                    new PageWaitForFunctionOptions
                    {
                        Timeout = timeoutMs
                    }
                );
                return true;
            }
            catch (Exception ex)
            {
                //log
                return false;
            }
        }
    public async Task<bool> WaitForPageLoadAsync()
        {
            try
            {
                await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                return true;
            }
            catch (Exception ex) { return false; }
        }
        public async Task<bool> WaitForNavigationAsync()
        {
            try
            {
                await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                return true;
            } catch (Exception ex) { return false; }
        }
        public async Task<bool> WaitForUrlAsync(string url, int? timeout = null)
        {
            try
            {
                var timeoutMs = timeout ?? _defaultTimeout;
                await _page.WaitForURLAsync(url, new PageWaitForURLOptions { Timeout = timeoutMs });
                return true;
            } catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<bool> WaitForTimeoutAsync(int milliseconds)
        {
            try
            {
                await _page.WaitForTimeoutAsync(milliseconds);
                return true;
            } catch (Exception ex) { return false; }
        }
        public async Task<bool> WaitForConditionAsync(Func<Task<bool>> condition, int? timeout = null, int pollInterval = 500)
        {
            var timeoutMs = timeout ?? _defaultTimeout;
            var endTime = DateTime.Now.AddMilliseconds(timeoutMs);
            while (DateTime.Now < endTime)
            {
                if (await condition()) return true;
                await Task.Delay(pollInterval);
            }
            return false;
        }
    }
}
