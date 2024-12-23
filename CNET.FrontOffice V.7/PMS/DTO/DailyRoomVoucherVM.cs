using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNET.FrontOffice_V._7
{
   public class DailyRoomVoucherVM
    {
        public int registrationId { get; set; }
        public string registrationNo { get; set; }
        public string roomNo { get; set; }
        public string consignee { get; set; }
        public decimal amount { get; set; }
        public bool isCharged { get; set; }
        public DateTime date { get; set; }

        public BindingList<DailyLineDTO> lineItems { get; set; }

        public DailyRoomVoucherVM()
        {
            lineItems = new BindingList<DailyLineDTO>();
        }
    }
   public class DailyLineDTO
   {
       public int articleCode { get; set; }
       public string Name { get; set; }
       public decimal? unitAmount { get; set; }
       public decimal? quantity { get; set; }
       public decimal? totalAmunt { get; set; }
   }
}
