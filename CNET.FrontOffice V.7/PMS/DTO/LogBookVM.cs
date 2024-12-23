
using CNET_V7_Domain.Domain.ViewSchema;
using CNET_V7_Domain.Misc.PmsView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNET.FrontOffice_V._7
{
    public class LogBookVM
    {
        public string Message { get; set; }
        public DateTime Date { get; set; }
        public string Author { get; set; }
        public bool IsSeen { get; set; }
        public string Device { get; set; }

        public VwMessageViewDTO LogMessage { get; set; }
    }
}
