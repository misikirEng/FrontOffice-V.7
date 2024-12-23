
using CNET_V7_Domain.Domain.PmsSchema;
using CNET_V7_Domain.Misc.PmsDTO; 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNET.FrontOffice_V._7
{
    public class GroupVM
    {
        public int SN { get; set; }
        public bool IsMaster { get; set; }
        public string RoomType { get; set; }
        public int RoomTypeCode { get; set; }
        public string Room { get; set; }
        public int RoomCode { get; set; }
        public string Guest { get; set; }
        public int? GuestCode { get; set; }
        public int Adult { get; set; }
        public int Child { get; set; }
        public int Numberofnight { get; set; }
        public DateTime Arrival { get; set; }
        public DateTime Departure { get; set; }
        public string Rate { get; set; }
        public decimal RateAmount { get; set; }
        public decimal TotalRateAmount { get; set; }
        public bool IsExisting { get; set; }
        public int RegCode { get; set; }

        public RateCodeHeaderDTO RateHeader { get; set; }
       public List<DailyRateCodeDTO> DilyRateCodeList { get; set; }
    }
}
