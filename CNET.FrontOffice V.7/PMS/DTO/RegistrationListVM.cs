using DummyAPI.DummyView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNET.FrontOffice_V._7
{
    public class RegistrationListVM
    {
        public int AccompanyCount { get; set; }
        public int adult { get; set; }
        public bool AllowLatecheckout { get; set; }
        public DateTime Arrival { get; set; }
        public bool AuthorizeDirectBill { get; set; }
        public bool authorizeKeyReturn { get; set; }
        public int child { get; set; }
        public string Color { get; set; }
        public string Company { get; set; }
        public string Companycode { get; set; }
        public string Consignee { get; set; }
        public string Customer { get; set; }
        public DateTime Departure { get; set; }
        public bool IsCharged { get; set; }
        public bool IsDueout { get; set; }
        public bool IsMaster { get; set; }
        public string lastState { get; set; }
        public int LogCount { get; set; }
        public string Market { get; set; }
        public int NoOfRoom { get; set; }
        public bool NoPost { get; set; }
        public int NumOfNight { get; set; }
        public string OrganizationUnitDefintion { get; set; }
        public string Payment { get; set; }
        public bool postStayCharging { get; set; }
        public bool preStayCharging { get; set; }
        public string RateCodeHeader { get; set; }
        public string RegExtCode { get; set; }
        public string Registration { get; set; }
        public DateTime RegistrationDate { get; set; }
        public string ResType { get; set; }
        public string Room { get; set; }
        public string RoomCode { get; set; }
        public string RoomType { get; set; }
        public string RoomTypeDescription { get; set; }
        public string RTC { get; set; }
        public int ShareCount { get; set; }
        public string SN { get; set; }
        public int TravelCount { get; set; }
        public int WakeupCallCount { get; set; }
        public List<vw_TravelDetail> TravelDetails { get; set; }
        public List<ServiceRequestDTO> ServiceRequests { get; set; }
        public List<Message_View> LogMessages { get; set; }
    }
}
