using CNET_V7_Domain.Misc.PmsView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNET.FrontOffice_V._7
{
    public class RoomSearchVM
    {
        //public string Room { get; set; }
        //public string RoomType { get; set; }
        //public string HKStatus { get; set; }
        //public string HKSatausCode { get; set; }
        //public string FOStatus { get; set; }
        //public string HKColor { get; set; }
        //public string Floor { get; set; }
        //public string Feature { get; set; }
        //public string Remark { get; set; }
        //public RoomDetail RoomDetail { get; set; }

        public bool select { get; set; }
        public VwVoucherDetailWithRoomDetailViewDTO RoomDetailView { get; set; }
        public string FOStatus { get; set; }
        public string Feature { get; set; }
    }
}
