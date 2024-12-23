using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CNET.FrontOffice_V._7
{
    public class TaskSheetVM
    {
        public string taskdate { get; set; }
       // public string task { get; set; }
        public string auto { get; set; }
        public string ttlsheet { get; set; }
        public string ttlcrdt { get; set; }
        public List<TaskDetail> taskDetail { get; set;}
    }
    public class TaskDetail
    {
        public int? AttendantCode { get; set; }
        public string AttendantName {get;set;}
        public string TotalRooms {get;set;}
        public string TotalCredit {get;set;}
    }
}
