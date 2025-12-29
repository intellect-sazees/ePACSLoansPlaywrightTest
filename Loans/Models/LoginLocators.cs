using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePACSLoans.Models
{
    /// <summary>
    /// Locators for Login page elements
    /// </summary>
    public class LoginLocators
    {
        public string UsernameInput { get; set; }// = string.Empty;
        public string PasswordInput { get; set; }// = string.Empty;
        public string DateInput { get; set; } //= string.Empty;
        public string CaptchaLabel { get; set; }// = string.Empty;
        public string CaptchaInput { get; set; } //= string.Empty;
        public string LoginButton { get; set; } //= string.Empty;
        public string InvalidCaptchaLabel { get; set; } //= string.Empty;
        public string SignoutMenu { get; set; }// = string.Empty;
        public string Signout { get; set; }// = string.Empty;
    }
}
