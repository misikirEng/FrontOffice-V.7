using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNET.FrontOffice_V._7
{
  public class FolioVM
    {
        public string voucherCode { get; set; }
        public string description { get; set; }
        public DateTime date { get; set; }
        public DayOfWeek weekDay { get; set; }
        public decimal debitCredit { get; set; }
        public string debitCreditFormatted { get; set; }
        public decimal balance { get; set; }
    }
}
