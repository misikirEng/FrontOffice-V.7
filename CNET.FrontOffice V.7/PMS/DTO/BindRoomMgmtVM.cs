using CNET.ERP.Client.Common.UI; 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNET.FrontOffice_V._7
{
  
      
    public class BindRoomMgmtVM
    {

        public BindRoomMgmtVM(bool Checked, string SN, string roomNo, string roomType, string roomStatus, string foStatus, string resStatus,
           string floor, int code)
        {
            this.roomNo = roomNo;
            this.roomType = roomType;
            this.roomStatus = roomStatus;
            this.foStatus = foStatus;
            this.resStatus = resStatus;
            this.floor = floor;
            this.code = code;
            this.SN = SN;
            this.Check = Checked;
        }
        public string roomNo { get; set; }
        public string roomType { get; set; }
        public string roomStatus { get; set; }
        public string foStatus { get; set; }
        public string resStatus { get; set; }
        public string floor { get; set; }
        public int code { get; set; }
        public string SN { get; set; }
        public bool Check { get; set; }

    }
}
