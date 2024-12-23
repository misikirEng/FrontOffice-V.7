using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNET.FrontOffice_V._7
{
    public class SalesSummaryVM
    {
        public int SN { get; set; }
        public string Code { get; set; }
        public string ArticleName { get; set; }
        public decimal Quantitiy { get; set; }
        public decimal UnitAmount { get; set; }
        public decimal TotalAmount { get; set; }

    }
}
