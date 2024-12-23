using CNET_V7_Domain.Domain.TransactionSchema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EventManagement
{
    public class LineItemDetailPMS
    {
        public LineItemDTO lineItem;
        public List<LineItemValueFactorDTO> lineItemValueFactor;
        public string articleName { get; set; }
        public string uom { get; set; }
    }
}
