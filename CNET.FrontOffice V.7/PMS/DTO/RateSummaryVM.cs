
using CNET_V7_Domain.Domain.PmsSchema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CNET.FrontOffice_V._7
{
   public class RateSummaryVM
    {
        public int regDetailCode { get; set; }
        public DateTime date { get; set; }
        public DayOfWeek day { get; set; }
        public string rateDescription { get; set; }
        public int rateCode { get; set; }
        public decimal roomRevenue { get; set; }
        public decimal package { get; set; }
        public decimal subTotal { get; set; }
        public decimal serCharge { get; set; }
        public decimal VAT { get; set; }
        public decimal grandTotal { get; set; }
        public RateCodeHeaderDTO rateCodeHeader { get; set; }
        public List<PackagesToPostDTO> packagesList { get; set; }
    }
}
