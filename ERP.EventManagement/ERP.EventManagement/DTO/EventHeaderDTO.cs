using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EventManagement.DTO
{
    public class EventHeaderDTO
    {
        public int SN { get; set; }
        public int Id { get; set; }
        public string Code { get; set; }
        public int? Consignee1 { get; set; }
        public string Consignee1Name { get; set; } 
        public int? Consignee2 { get; set; } 
        public int? Consignee3 { get; set; }
        public int? Consignee4 { get; set; }
        public int? BookingTypeCode { get; set; }
        public string BookingTypeDesc { get; set; }
        public int? EventCategCode { get; set; }
        public string EventCategDesc { get; set; }
        public int? OsdCode { get; set; }
        public string OsdDescription { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Color { get; set; }

    }
}
