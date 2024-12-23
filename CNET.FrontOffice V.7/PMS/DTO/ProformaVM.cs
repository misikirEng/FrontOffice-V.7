using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNET.FrontOffice_V._7
{
    public class ProformaVM
    {
        public int SN { get; set; }
        public DateTime Date { get; set; }
        public int RateCode { get; set; }
        public string RateDesc { get; set; }
        public string PackageDesc { get; set; }
        public int NoOfPerson { get; set; }
        public decimal Amount { get; set; }

        public decimal RoomCharge { get; set; }
        public decimal Package { get; set; }
        public decimal ServiceCharge { get; set; }
        public decimal Subtotal { get; set; }
        public decimal VAT { get; set; }

    }
}
