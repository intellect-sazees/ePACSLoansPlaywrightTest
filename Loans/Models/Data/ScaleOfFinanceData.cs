using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePACSLoans.Models.Data
{
    public class ScaleOfFinanceData
    {
        public string Product {  get; set; }=string.Empty;
        public string Purpose { get; set; } = string.Empty;
        public string RabiCash { get; set; } = string.Empty;
        public string RabiKind { get; set; } = string.Empty;
        public string KharifCash {  get; set; } = string.Empty;
        public string KharifKind { get;set; } = string.Empty;
        public string EffectiveDate { get; set; } = string.Empty;
    }
}
