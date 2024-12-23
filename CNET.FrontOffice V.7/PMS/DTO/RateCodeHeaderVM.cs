using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNET.FrontOffice_V._7.PMS.DTO
{

    public class RateCodeHeaderVM
    {

        public decimal? addition { get; set; }
        public int article { get; set; }
        public string Hotel { get; set; }
        public int Hotelcode { get; set; }
        public string category { get; set; }
        public int code { get; set; }
        public decimal? comission { get; set; }
        public int currencyCode { get; set; }
        public string description { get; set; }
        public DateTime? endDate { get; set; }
        public string exchangeRule { get; set; }
        public string folioText { get; set; }
        public string market { get; set; }
        public int? maxOccupancy { get; set; }
        public int? minOccupancy { get; set; }
        public decimal? multiplication { get; set; }
        public string remark { get; set; }
        public string source { get; set; }
        public DateTime? startDate { get; set; }
    }
}
