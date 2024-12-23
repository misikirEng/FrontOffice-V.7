using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CNET.FrontOffice_V._7
{
    public class WakeupVM
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public DateTime Time { get; set; }
        public string Note { get; set; }
        public int? regCode { get; set; }
        public int scheduleCode { get; set; }
        public int headerType { get; set; }
        public int? headerCode { get; set; }
        public int detailCode { get; set; }

        public List<WakeupDetailVM> WakeUpDetails { get; set; }

    }

    public class WakeupDetailVM
    {
        public string dayMonth { get; set; }
        public string startTime { get; set; }
        public string endTime { get; set; }
        public int dayOfMonth { get; set; }
        public int detailCode { get; set; }
        public string remark { get; set; }
    }
}
