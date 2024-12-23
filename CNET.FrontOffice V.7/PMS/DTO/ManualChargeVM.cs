using CNET_V7_Domain.Domain.TransactionSchema;
using CNET_V7_Domain.Misc.PmsDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 

namespace CNET.FrontOffice_V._7
{
    public class ManualChargeVM
    {
        public int Code { get; set; }
        public string Description { get; set; }
        public decimal Quantity { get; set; }
        public decimal taxableAmount { get; set; }
        public decimal taxAmount { get; set; }
        public decimal totalAmount { get; set; }
        public decimal unitAmt { get; set; }
        public int tax { get; set; }
        public decimal Discount { get; set; }

        public LineItemDTO LineItem { get; set; }
        public LineItemDetails LiDetails { get; set; }
        public List<LineItemValueFactorDTO> LineItemValueFactor { get; set; }

    }
}
