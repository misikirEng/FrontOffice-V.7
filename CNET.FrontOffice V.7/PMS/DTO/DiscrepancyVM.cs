using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNET.FrontOffice_V._7
{
    class DiscrepancyVM
    {
        public string roomno { get; set; }
        public string roomtype { get; set; }
        public string roomstatus { get; set; }
        public string hkstatus { get; set; }
        public string fostatus { get; set; }
        public string resstatus { get; set; }
        public string foperson { get; set; }
        public int? hkperson { get; set; }
        public int? discrepancy { get; set; }
        public int? roomdetail { get; set; }
        public string date { get; set; }
    }
}
