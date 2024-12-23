
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CNET_V7_Domain.Domain.PmsSchema;

namespace CNET.FrontOffice_V._7
{
  public  class RateDetailVM
    {
        public String code { get; set; }
        public String description { get; set; }
        public String parent { get; set; }
        public RateCodeHeaderDTO rateCodeObj { get; set; }
    }
}
