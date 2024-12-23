using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNET.FrontOffice_V._7
{
    public class GroupCheckoutVM
    {
        public bool IsSelected { get; set; }
        public string Color { get; set; }
        public int? FoStatus { get; set; }
        public bool isDateValid { get; set; }
        public bool isCardReturned { get; set; }

        public bool IsBillTTransfered { get; set; }

        public int RegId { get; set; }
        public string RegNo { get; set; }
        public string Name { get; set; }
        public string RoomType { get; set; }
        public string Room { get; set; }
        public DateTime ArrivalDate { get; set; }
        public DateTime DepartureDate { get; set; }

        public decimal RoomCharge { get; set; }
        public decimal ExtraBills { get; set; }
        public decimal Payment { get; set; }

        public decimal Discount { get; set; }

        public decimal Total { get; set; }

        public List<TransferedBills> TransferedBills { get; set; }

    }


    public class TransferedBills
    {
        public string VoucherCode { get; set; }
        public DateTime IssuedDate { get; set; }
        public string TransferedBy { get; set; }
        public DateTime TransferdDate { get; set; }

        public string BillType { get; set; }

        public decimal GrandTotal { get; set; }
    }
}
