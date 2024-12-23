using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNET.FrontOffice_V._7.PMS.DTO
{
    public class ForeignExTranDTO
    {
        public decimal Amount { get; set; }
        public int? CurrencyCode { get; set; }
        public string CurrencyDesc { get; set; }
        public decimal ExRate { get; set; }
        public bool IsAssigned { get; set; }
        public string Remark { get; set; }
        public int SN { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
