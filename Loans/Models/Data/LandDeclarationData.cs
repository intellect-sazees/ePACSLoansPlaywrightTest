using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePACSLoans.Models.Data
{
    public class LandDeclarationData
    {
        public string AdmissionNo { get; set; } = string.Empty;
        public string Product { get; set; } = string.Empty;
        public string Crop { get; set; } = string.Empty;
        public string Village { get; set; } = string.Empty;
        public string SurveyNo { get; set; } = string.Empty;
        public string LandValuePerAcer { get; set; } = string.Empty;
    }
}
