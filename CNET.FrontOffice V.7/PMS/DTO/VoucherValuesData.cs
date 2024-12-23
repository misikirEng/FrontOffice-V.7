using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace CNET.FrontOffice_V._7
{
  public  class VoucherValuesData
    {
        public DateTime IssuedDate { get; set; }
        public int voucherCode { get; set; }
        public int voucherDefinition { get; set; }
        public decimal? subTotal { get; set; }
        public decimal? VATtaxAmount { get; set; }
        public decimal? additionalCharge { get; set; }
        public decimal? discount { get; set; }
        public decimal? grandTotal { get; set; }
        public string voucherNote { get; set; }
    }
}
