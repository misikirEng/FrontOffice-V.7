using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CNET.FrontOffice_V._7
{
    public class SplitDTO
    {
        public string SN { get; set; }
        public int VoucherId { get; set; }
        public string VoucherCode { get; set; }
        public string Purpose { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public decimal PrintStatus { get; set; }

    }
}
