using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EventManagement.DTO
{
    public class EventDetaildataDTO
    {
        public int SN { get; set; }
        public int Id { get; set; }
        public string Code { get; set; }
        public string Voucher { get; set; }
        public int? TypeCode { get; set; }
        public string TypeDesc { get; set; }
        public string Description {get; set; }
        public int SpaceCode { get; set; }
        public string Hall { get; set; }
        public string ArrangementDesc { get; set; }
        public int Arrangementid { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int NumOfPerson { get; set; }
        public string Remark { get; set; }
    }
}
