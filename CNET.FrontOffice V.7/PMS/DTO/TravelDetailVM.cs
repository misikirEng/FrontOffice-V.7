
using CNET_V7_Domain.Domain.ViewSchema;
using CNET_V7_Domain.Misc.PmsView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CNET.FrontOffice_V._7
{
    public class TravelDetailVM
    {
        public string Type { get; set; }
        public string Station { get; set; }
        public string TransNum { get; set; }
        public DateTime Time { get; set; }

        public VwTravelDetailDTO TravelDetail { get; set; }
    }
}
