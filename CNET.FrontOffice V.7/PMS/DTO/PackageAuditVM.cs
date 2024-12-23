using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNET.FrontOffice_V._7.PMS.DTO
{
    public class PackageAuditVM
    {
        public int AdultCount { get; set; }
        public decimal? Amount { get; set; }
        public int ChildCount { get; set; }
        public string Guest { get; set; }
        public string Package { get; set; }
        public string RegNum { get; set; }
        public string Room { get; set; }
        public string RoomType { get; set; }
        public int SN { get; set; }
        public string VoucherCode { get; set; }
    }
}
