using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNET.FrontOffice_V._7
{
    public class AccompanyGuestVM
    {
        public int SN { get; set; }
        public int Id { get; set; }
        public string fullName { get; set; }
        public int rateCode { get; set; }
        public string consignee { get; set; }
        public int gslType { get; set; }
        public string address { get; set; }
        public string voucher { get; set; }
        public string nationalityName { get; set; }
        public string PersonGender { get; set; }
        public string requiredGSL { get; set; }
        public string IdentficationDescription { get; set; }
        public string Passport { get; set; }
        public string Title { get; set; }
        public bool IsSaved { get; set; }
    }
}
