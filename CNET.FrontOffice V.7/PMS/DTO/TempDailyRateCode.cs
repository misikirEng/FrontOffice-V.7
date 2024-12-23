using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNET.FrontOffice_V._7.PMS.DTO
{
    public class TempDailyRateCode
    {
        public DayOfWeek DayWeek { get; set; }
        public string Description { get; set; }
        public int? RateCodeDetail { get; set; }
        public DateTime StayDate { get; set; }
        public decimal UnitRoomRate { get; set; }

    }
}
