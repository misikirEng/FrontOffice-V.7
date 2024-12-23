
using CNET_V7_Domain.Domain.PmsSchema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNET.FrontOffice_V._7
{
    class RateRoomTypeVM
    {
        public int Id { get; set; }
        public String roomType { get; set; }
        public Boolean value { get; set; }
        public RoomTypeVM AttachedObject { get; set; }
    }
}
