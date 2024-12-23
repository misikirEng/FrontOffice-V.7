using CNET_V7_Domain.Domain.CommonSchema;
using CNET_V7_Domain.Domain.PmsSchema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNET.FrontOffice_V._7
{
    public class RegistrationDetailVM : RegistrationDetailDTO
    {


        public string actualRTCDesc { get; set; }
        public string marketDesc { get; set; }
        public string RegistrationHeaderDesc { get; set; } 
        public string roomDesc { get; set; }
        public string rateCodeDetailDesc { get; set; }
        
        public string roomTypeDesc { get; set; }
        public string sourceDesc { get; set; }
        public virtual List<PackagesToPostVM> PackagesToPostDTOs { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public RegistrationDetailVM()
        {
            PackagesToPostDTOs = new List<PackagesToPostVM>();
        }

    }
}
