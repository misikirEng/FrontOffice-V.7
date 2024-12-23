using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNET.FrontOffice_V._7.PMS.DTO
{
    public class RegistrationDocumentVM
    {
        public string actualRTC { get; set; }
        public int adult { get; set; }
        public DateTime arrivalDate { get; set; }
        public string brandName { get; set; }
        public int child { get; set; }
        public string code { get; set; }
        public string color { get; set; }
        public string consignee { get; set; }
        public DateTime Date { get; set; }
        public DateTime departureDate { get; set; }
        public string font { get; set; }
        public string foStatus { get; set; }
        public string HKState { get; set; }
        public string idNumber { get; set; }
        public bool ismaster { get; set; }
        public DateTime IssuedDate { get; set; }
        public string Market { get; set; }
        public string name { get; set; }
        public string OrganizationUnitDefintion { get; set; }
        public string OtherConsignee { get; set; }
        public string PaymentMethod { get; set; }
        public string paymentType { get; set; }
        public decimal rateAmount { get; set; }
        public string rateCodeDetail { get; set; }
        public string rateCodeHeader { get; set; }
        public string RegHeaderCode { get; set; }
        public string regHeaderRemark { get; set; }
        public string remark { get; set; }
        public string requiredGSL { get; set; }
        public string resType { get; set; }
        public string room { get; set; }
        public int roomCount { get; set; }
        public string RoomNumber { get; set; }
        public string roomType { get; set; }
        public string RoomTypeDescription { get; set; }
        public string tradeName { get; set; }
        public string type { get; set; }
        public int voucherDefinition { get; set; }

    }
}
