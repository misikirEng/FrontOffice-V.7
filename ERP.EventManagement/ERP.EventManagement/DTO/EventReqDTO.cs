using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EventManagement
{
    public class EventReqDTO
    {
        public int SN { get; set; }
        public int EventDetailid { get; set; }
        public string EventDetailCode { get; set; }
        public string VoucherCode { get; set; }
        public int? VoucherId { get; set; }
        public int Articleid { get; set; }
        public string ArticleCode { get; set; }
        public string ArticleName { get; set; }
        public decimal Qty { get; set; }

        public decimal? AdditionalCharge { get; set; }

        public decimal? Discount { get; set; }
        public decimal UnitAmount { get; set; }
        public decimal? TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string LineItemNote { get; set; }
        public string VoucherNote { get; set; }
        public int LineItemCode { get; set; }

    }
}
