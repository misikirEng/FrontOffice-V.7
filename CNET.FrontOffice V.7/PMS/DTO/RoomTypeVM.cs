
using CNET_V7_Domain.Domain.PmsSchema;
using DevExpress.XtraRichEdit.Import.Doc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CNET.FrontOffice_V._7
{
    public class RoomTypeVM : RoomTypeDTO
    { 
        public int assignedRooms { get; set; }
        public RoomTypeDTO AttachedObject { get; set; }

    }
}
