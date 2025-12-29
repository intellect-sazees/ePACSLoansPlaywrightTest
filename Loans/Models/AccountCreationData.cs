using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntellectPlaywrightTest.Models
{
    /// <summary>
    /// Data model for account creation
    /// </summary>
    public class AccountCreationData
    {
        public string Product { get; set; } = string.Empty;
        public string Purpose { get; set; } = string.Empty;
        public string SocietyLoanNo { get; set; } = string.Empty;
        public string AppliedDate { get; set; } = string.Empty;
        public string AppliedAmount { get; set; } = string.Empty;
        public string SanctionDate { get; set; } = string.Empty;
        public string SanctionAmount { get; set; } = string.Empty;
        public string RepaymentType { get; set; } = string.Empty;
        public string RepaymentMode { get; set; } = string.Empty;
        public string ROI { get; set; } = string.Empty;
        public string PenalROI { get; set; } = string.Empty;
        public string IOAROI { get; set; } = string.Empty;
        public string GestationPeriodMonths { get; set; } = string.Empty;
        public string LoanPeriodMonths { get; set; } = string.Empty;
    }
}
