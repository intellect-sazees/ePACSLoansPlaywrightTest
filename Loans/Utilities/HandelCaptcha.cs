using ePACSLoans.Core;
using ePACSLoans.Utilities.Helpers;
using Microsoft.Playwright;
using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using Tesseract;

namespace ePACSLoans.Utilities
{
    public class HandleCaptcha
    {
        private readonly IElementHandle _captchaElement;
        public string Text { get; private set; }
        private readonly string _projectPath;
        public HandleCaptcha(IElementHandle captchaElement)
        {
            _captchaElement = captchaElement;
        }
        public async Task<bool> SolveCaptchaAsync()
        {
            if (_captchaElement == null)
                return false;
            try
            {
                var src = await _captchaElement.GetAttributeAsync("src");
                if (string.IsNullOrEmpty(src))
                    throw new IOException("Captcha src attribute is empty.");
                string base64Data = src.Contains(",") ? src.Substring(src.IndexOf(',') + 1) : src;
                byte[] imageBytes = Convert.FromBase64String(base64Data);
                using (var ms = new MemoryStream(imageBytes))
                using (var bmp = new Bitmap(ms))
                {
                    PathHelper _obj = new PathHelper();
                    var basePath = _obj.getProjectPath();
                    var _projectPath = Path.Combine(basePath, FrameworkConstants.ResourcesFolder,FrameworkConstants.TesseractOcrFolder);
                    string TesseractPath = _projectPath;// Path.Combine(_projectPath, "Resources", "Tesseract OCR");
                    // Makesure tessdata exists in that folder with 'eng.traineddata'
                    Environment.SetEnvironmentVariable("TESSDATA_PREFIX", TesseractPath);
                    using (var msPix = new MemoryStream())
                    {
                        bmp.Save(msPix, System.Drawing.Imaging.ImageFormat.Png);
                        msPix.Position = 0;
                        using (var engine = new TesseractEngine(TesseractPath, "eng", EngineMode.Default))
                        using (var pix = Pix.LoadFromMemory(msPix.ToArray()))
                        using (var page = engine.Process(pix))
                        {
                            Text = page.GetText()?.Trim();
                        }
                    }
                }
                //Log
                return !string.IsNullOrEmpty(Text);
            }
            catch (Exception ex)
            {
                //Log
                return false;
            }
        }
    }
}
