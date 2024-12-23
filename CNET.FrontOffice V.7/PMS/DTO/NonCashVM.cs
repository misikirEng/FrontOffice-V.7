
using CNET_V7_Domain.Domain.ConsigneeSchema;
using CNET_V7_Domain.Domain.ViewSchema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNET.FrontOffice_V._7
{
    public class NonCashVM
    {
        public string ReferenceNum { get; set; }
        public string ReceivedAmount { get; set; }
        public DateTime ReceivedDate { get; set; }
        public int? CurrencyCode { get; set; }
        public string CurrencyDesc { get; set; }
        public int? PaymentTypeCode { get; set; }
        public string PaymentTypeDesc { get; set; }
        public string SelectedConsignee { get; set; }
        public decimal CurrencyRate { get; set; }

        public List<VwConsigneeViewDTO> ConsigneeList { get; set; }
    }
}
