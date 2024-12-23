using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNET.FrontOffice_V._7
{
    public class JournalizeVM
    {
        public int SN { get; set; }
        public string JournalNumber { get; set; }
        public string JournalType { get; set; }
        public string Voucher { get; set; }
        public string Account { get; set; }
        public string AccNumber { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public string ControlAccount { get; set; }
        public string NormalBalance { get; set; }
        public string AccountCategory { get; set; }
        public string AccountType { get; set; }
        public DateTime Date { get; set; }

    }
}
