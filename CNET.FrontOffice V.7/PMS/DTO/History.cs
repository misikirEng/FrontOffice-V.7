
using System;
using System.Collections.Generic;
using System.Linq;

namespace CNET.FrontOffice_V._7
{
    internal class HistoryDTO
    {
        public String attribute { get; set; }
        public String value { get; set; }
        public RoomTypeVM attachedObj { get; set; }
    }
}
