using CNET.ERP.Client.Common.UI; 
using System;
using System.Collections.Generic;
using System.Linq;

namespace CNET.FrontOffice_V._7
{
    public class FormSetting
    {
        public static List<String> SingleInstanceFormsList = new List<string>();
        public static List<String> TabbedFormsList = new List<string>();


        public static Dictionary<Type, String> FormIdentifierObjects = new Dictionary<Type, string>();


        public FormSetting()
        {
            SingleInstanceFormsList.Add("CNET PMS//FOR TESTING//REGISTRATION");
            SingleInstanceFormsList.Add("CNET PMS//FOR TESTING//REGISTRATION SEARCH");

            TabbedFormsList.Add("PROPERTY");
            TabbedFormsList.Add("DOCUMENT BROWSER");
            TabbedFormsList.Add("PACKAGE");
            TabbedFormsList.Add("SETTING");
            TabbedFormsList.Add("LOOKUP");
            TabbedFormsList.Add("END OF DAY");
            TabbedFormsList.Add("REGISTRATION DOCUMENT");
            TabbedFormsList.Add("ROOM INVENTORY");
            TabbedFormsList.Add("PACKAGE AUDIT");
            TabbedFormsList.Add("EVENT MANAGEMENT");
            TabbedFormsList.Add("REPORT");
            TabbedFormsList.Add("REVENUE MANAGEMENT");
            TabbedFormsList.Add("FISCAL PRINTER"); //Fisical Printer
            TabbedFormsList.Add("EVENT DOCUMENT BROWSER");
            TabbedFormsList.Add("EVENT REPORTS");
            // Housekeeping 
            TabbedFormsList.Add("TASK ASSIGNMENT");
            TabbedFormsList.Add("ROOM MANAGEMENT");
            TabbedFormsList.Add("DISCREPANCY");
            TabbedFormsList.Add("TURNDOWN MANAGEMENT");

            TabbedFormsList.Add("LICENSE");
            TabbedFormsList.Add("DENSITY CHART");
            TabbedFormsList.Add("ROOM STATUS");

            //Postine Routine
         /*   List<PostingRoutineHeader> prHeaderList = ACCUIProcessManager.GetPostingRoutineHeadersByComponent(CNETConstantes.PMS);
            if(prHeaderList != null)
            {
                foreach(var pr in prHeaderList)
                {
                    TabbedFormsList.Add(pr.description.ToUpper());
                }
            }*/
       

        }


        public static Boolean FormIsTabbed(String fullPath)
        {
            if (!TabbedFormsList.Any())
            {
                new FormSetting();
            }
            if (String.IsNullOrEmpty(fullPath))
            {
                throw new Exception("Incorrect Path Entered");
            }
            string[] splited = fullPath.Split('_');
            fullPath = splited[1].ToUpper();

            if (TabbedFormsList.Contains(fullPath))
            {
                return true;
            }
            return false;
        }
    }
}
