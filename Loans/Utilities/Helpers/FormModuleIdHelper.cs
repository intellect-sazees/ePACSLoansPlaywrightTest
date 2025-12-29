using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Tesseract;

namespace ePACSLoans.Utilities.Helpers
{
    public class FormModuleIdHelper
    {
        private readonly IPage _page;
        string formId;
        string moduleId;
        public FormModuleIdHelper(IPage page)
        {
            _page = page ?? throw new ArgumentNullException(nameof(page));
        }
        public async Task<(string FormId, string ModuleId)> GetFormIDandModuleID()
        {
            try
            {
                string currentUrl =_page.Url;
                var uri = new Uri(currentUrl);
                var queryParams = System.Web.HttpUtility.ParseQueryString(uri.Query);
                formId = queryParams["formid"] ?? "";
                moduleId = queryParams["moduleid"] ?? "";
                return (formId, moduleId);
            }
            catch (Exception ex)
            {
                return ("", "");
            }
        }
    }
}
