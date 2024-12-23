using System;
using System.Collections.Generic;
using System.Linq;

namespace CNET.FrontOffice_V._7
{
    public class RateCodeAudit
    {
        public int RegistrationId{ get; set; }
        public string RegistrationNumber { get; set; }
        public string RoomNumber { get; set; }
        public string GuestName { get; set; }
        public string Company { get; set; }
        public int Adult { get; set; }
        public int Child { get; set; }
        public int? RateCodeHeader { get; set; }
        public decimal RateCodeAmount { get; set; }
        public decimal RateAmount { get; set; }
        public decimal Variance { get; set; }
        public string CurrencyDescription { get; set; }
        public string CurrencyCode { get; set; }
        public DateTime ArrivalDate { get; set; }
        public DateTime DepartureDate { get; set; }
        public string RoomType { get; set; }
        public string RTC { get; set; }
        public int? RegistrationState { get; set; }
        public int? Consignee { get; set; }
        public string Color { get; set; }
    }
}
