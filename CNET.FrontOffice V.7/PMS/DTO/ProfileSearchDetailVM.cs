using System;
using System.Collections.Generic;
using System.Linq;

namespace CNET.FrontOffice_V._7
{
    internal class ProfileSearchDetailVM
    {
        public string room { get; set; }
        public string roomType { get; set; }
        public DateTime arrival { get; set; }
        public DateTime departure { get; set; }
        public string status { get; set; }
        public string groupCompany { get; set; }
        public string travelSource { get; set; }



        public Object AttachedObject1 { get; set; }
        public Object AttachedObject2 { get; set; }
    }
}
