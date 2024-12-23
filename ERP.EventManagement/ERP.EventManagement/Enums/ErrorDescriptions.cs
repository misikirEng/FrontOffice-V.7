using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EventManagement.Enums
{
    public class ErrorDescriptions
    {
        public static int IDSETTINGNOTDEFINED = 0;
        public static int EVENTOWNERNOTDEFINED = 1;
        public static int EVENTOBJECTSTATENOTDEFINED = 2;
        public static int REQUIREDGSLISNOTDEFINED = 3;
        public static int REQUIREDGSLAGENTNOTDEFINED = 4;
        public static int REQUIREDGSLBUSSINESSNOTDEFINED = 5;
        public static int REQUIREDGSLCONTACTNOTDEFINED = 6;
        public static int STARTCANNOTGREATERTHANENDDATE = 7;
        public static int RESERVATIONTYPEISNOTDEFINED = 8;
        public static int EVENTCATEGORYNOTDEFINED = 9;
        public static int EVENTPAYMENTTYPENOTDEFINED = 10;
        public static int EVENTORGANIZERNOTSELECTED = 11;
        public static int EVENTBUSSINESSSOURCENOTSELECTED = 12;
        public static int EVENTCONTACTNOTSELECTED = 13;

    }
}
