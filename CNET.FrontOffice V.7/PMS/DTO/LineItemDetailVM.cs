using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNET.FrontOffice_V._7.PMS.DTO
{
    public class LineItemDetailVM
    {
        public string Code { get; set; }
        public int article { get; set; }
        public string name { get; set; }
        public decimal quantity { get; set; }
        public decimal? unitAmt { get; set; }
        public decimal totalAmount { get; set; }


    }
}
