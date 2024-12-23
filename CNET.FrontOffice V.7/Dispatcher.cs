using System;
using System.Collections.Generic;
using System.Linq;
using CNET.ERP.Client.Common.UI;
using DevExpress.XtraEditors;
using System.Windows.Forms; 
using PMSReport;
using CNET.FrontOffice_V._7.Forms;
using CNET.ERP.Client.UI_Logic.PMS.Forms;
using System.IO.Packaging;
using CNET.FrontOffice_V._7.Group_Registration;
using CNET.FrontOffice_V._7.Forms.Non_Navigatable_Modals;
using ProcessManager;
using CNET.ERP.Client.UI_Logic.PMS.Forms.Reports;
using CNET.FrontOffice_V._7.Night_Audit;
using CNET.FrontOffice_V._7.HouseKeeping;
using CNET.FrontOffice_V._7.Forms.Pinned_Tabs;
using System.Diagnostics;
using System.Configuration;
using CNET.API.Manager;
using System.Reflection.Emit;

namespace CNET.FrontOffice_V._7
{
    public class Dispatcher
    {
        public static bool IsFromSecurity { get; set; }
        public static MasterPageForm home { get; set; }
        public static UILogicBase SelectLogicBase(string path, string filterKey = null)
        {
            string[] splited = path.Split('_');
            string fullName = splited[0];
            string title = splited[1];
            if (String.IsNullOrEmpty(fullName))
            {
                throw new Exception();
            }
            fullName = fullName.ToLower().Trim();
            /*
             frmProperty property = null;
                        frmRevenueManagement revenueManagement = null;
                        frmPMSReports frmReport =null;
                        frmPackage package = null;

                        switch (fullName)
                        {
                            case "main menu//revenue management":
                                //if (!CheckSecurity(fullName))
                                //{
                                //    return null;
                                //}
                                revenueManagement = new frmRevenueManagement();

                               return revenueManagement;
                            case "main menu//property":
                                //if (!CheckSecurity(fullName))
                                //{
                                //    return null;
                                //}
                                property = new frmProperty();

                                return property;

                            case "main menu//package":
                                //if (!CheckSecurity(fullName))
                                //{
                                //    return null;
                                //}
                                package = new frmPackage();

                                return package;
                        }

                        switch (title)
                        {
                            case "Report":
                                frmReport = new frmPMSReports();
                                return frmReport;

                        }
            */

            fullName = fullName.ToLower().Trim();
            frmRevenueManagement revenueManagement = null;
            RegistrationList frmRegistrationList = null;
            frmDocumentBrowser frmDocumentBrowser = null;
            frmRoomInventory frmRoomInventory = null;
            frmRegistrationDensityChart frmRegistrationDensityChart = null;
            frmRoomStatus frmRoomStatus = null;
            frmReservation reservation = null;
            frmGroupRegistration frmGroupReg = null;
            frmEventManagment frmEventMgt = null;
            frmPerson person = null;
            frmOrganization organization = null;
            //frmReservationSearch reservationSearch = null;
            //frmProfileSearch profileSearch = null;
            frmRateSearch rateSearch = null;
            frmRateDetail rateDetail = null;
            frmRateSummery rateSummary = null;
            frmRoomSearch roomSearch = null;
            frmDailyDetail dailyDetail = null;
            frmTravelDetail travelDetail = null;
            frmProperty property = null;
            Reportfrm report = null;
            frmPackage package = null;
            // frmSetting setting = null;

            //  frmLicenseForm License = null;

            frmHouseKeepingMgmt houseKeeping = null;
            frmTaskAssignment taskAssignment = null;
            frmDescripancy descripancy = null;
            frmTurndwnMgmt turnDown = null;

            frmPackageAudit frmPackageAudit = null;

            //frmPostingRoutine frmPostingRoutine = null;
            //Postine Routine
            //List<PostingRoutineHeader> prHeaderList = ACCUIProcessManager.GetPostingRoutineHeadersByComponent(CNETConstantes.PMS);
            //if (prHeaderList != null)
            //{
            //    foreach (var pr in prHeaderList)
            //    {
            //        string caseString = "main menu//" + pr.description.ToLower();
            //        if  (fullName == caseString)
            //        {
            //            bool isAccountingLicensed = CommonLogics.CheckFinancialLicense();
            //            if (!isAccountingLicensed)
            //            {
            //                XtraMessageBox.Show("Financial Managment System is not licensed. F&B and Journalize are deactivated!", "Night Audit", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //                return null;
            //            }
            //            frmPostingRoutine = new frmPostingRoutine(pr);
            //            return frmPostingRoutine;
            //        }
            //    }
            //}

            switch (fullName)
            {
                case "main menu//document browser":
                    frmDocumentBrowser = new frmDocumentBrowser();
                    return frmDocumentBrowser;
                case "main menu//density chart":
                    frmRegistrationDensityChart = new frmRegistrationDensityChart();
                    return frmRegistrationDensityChart;
                case "main menu//room status":
                    frmRoomStatus = new frmRoomStatus();
                    return frmRoomStatus;

                case "main menu//registration document":
                    if (!CheckSecurity(fullName))
                    {
                        return null;
                    }
                    frmRegistrationList = new RegistrationList();

                    frmRegistrationList.FilterKey = filterKey;
                    MasterPageForm.DashboardRefresher = frmRegistrationList.DashboardFilterRefreshHandler;
                    return frmRegistrationList;

                case "main menu//room inventory":
                    if (!CheckSecurity(fullName))
                    {
                        return null;
                    }
                    frmRoomInventory = new frmRoomInventory();
                    return frmRoomInventory;
                case "main menu//package audit":
                    if (!CheckSecurity(fullName))
                    {
                        return null;
                    }
                    frmPackageAudit = new frmPackageAudit();
                    return frmPackageAudit;



                case "main menu//reservation":
                    //if (!CheckClosing())
                    //{
                    //    return null;
                    //}
                    if (!CheckSecurity(fullName))
                    {
                        return null;
                    }
                    reservation = new frmReservation(false);
                    reservation.LoadEventArg.Sender = null;
                    return reservation;

                case "main menu//check in":
                    //if (!CheckClosing())
                    //{
                    //    return null;
                    //}
                    if (!CheckSecurity(fullName))
                    {
                        return null;
                    }
                    reservation = new frmReservation(true);
                    reservation.LoadEventArg.Sender = null;

                    return reservation;


                case "main menu//group registration":
                    //if (!CheckClosing())
                    //{
                    //    return null;
                    //}
                    if (!CheckSecurity(fullName))
                    {
                        return null;
                    }
                    frmGroupReg = new frmGroupRegistration();
                    return frmGroupReg;

                case "main menu//guest":
                    if (!CheckSecurity(fullName))
                    {
                        return null;
                    }
                    person = new frmPerson(@"Guest");
                    person.Text = "Guest";
                    person.GSLType = CNETConstantes.GUEST;
                    person.rpgScanFingerPrint.Visible = true;
                    person.LoadEventArg.Args = "Guest";
                    person.LoadEventArg.Sender = null;

                    return person;

                case "main menu//contact":
                    if (!CheckSecurity(fullName))
                    {
                        return null;
                    }

                    person = new frmPerson("Contact");

                    person.Text = "Contact";
                    person.GSLType = CNETConstantes.CONTACT;
                    person.rpgScanFingerPrint.Visible = true;
                    person.LoadEventArg.Args = "Contact";
                    person.LoadEventArg.Sender = null;

                    return person;

                    break;


                case "main menu//company":
                    if (!CheckSecurity(fullName))
                    {
                        return null;
                    }
                    organization = new frmOrganization();
                    organization.Text = "Company";
                    organization.GslType = CNETConstantes.CUSTOMER;
                    organization.rcOrganization.Visible = true;
                    organization.LoadEventArg.Args = "Company";
                    organization.LoadEventArg.Sender = null;
                    return organization;

                    break;

                case "main menu//travel agent":
                    if (!CheckSecurity(fullName))
                    {
                        return null;
                    }
                    organization = new frmOrganization();
                    organization.GslType = CNETConstantes.AGENT;
                    organization.Text = "Travel Agent";
                    organization.rcOrganization.Visible = true;
                    organization.LoadEventArg.Args = "Travel Agent";
                    organization.LoadEventArg.Sender = null;
                    return organization;

                    break;

                case "main menu//source":
                    if (!CheckSecurity(fullName))
                    {
                        return null;
                    }
                    organization = new frmOrganization();
                    organization.GslType = CNETConstantes.BUSINESSsOURCE;
                    organization.Text = "Source";
                    organization.rcOrganization.Visible = true;
                    organization.LoadEventArg.Args = "Source";
                    organization.LoadEventArg.Sender = null;

                    return organization;

                case "main menu//group":
                    if (!CheckSecurity(fullName))
                    {
                        return null;
                    }
                    organization = new frmOrganization();
                    organization.GslType = CNETConstantes.GROUP;

                    organization.Text = "Group";
                    organization.rcOrganization.Visible = true;
                    organization.LoadEventArg.Args = "Group";
                    organization.LoadEventArg.Sender = null;
                    return organization;

                case "main menu//end of day":
                    if (!CheckSecurity(fullName))
                    {
                        return null;
                    }
                    //frmNightAudit au = new frmNightAudit();
                    //return au;
                    frmAuditNightAll frmNightAudit = new frmAuditNightAll();
                    return frmNightAudit;

                //    break;

                case "main menu//end of month":
                    //if (!CheckSecurity(fullName))
                    //{
                    //    return null;
                    //}
                    break;

                //case "main menu//end of month":
                //    if (!CheckSecurity(fullName))
                //    {
                //        return null;
                //    }
                //    break;

                //case "main menu//setting":
                //    if (!CheckSecurity(fullName))
                //    {
                //        return null;
                //    }
                //    setting = new frmSetting();

                //    return setting;


                case "main menu//property":
                    if (!CheckSecurity(fullName))
                    {
                        return null;
                    }
                    property = new frmProperty();

                    return property;

                case "main menu//package":
                    if (!CheckSecurity(fullName))
                    {
                        return null;
                    }
                    package = new frmPackage();

                    return package;

                case "main menu//revenue management":
                    if (!CheckSecurity(fullName))
                    {
                        return null;
                    }
                    revenueManagement = new frmRevenueManagement();

                    return revenueManagement;

                case "main menu//calendar":
                    if (!CheckSecurity(fullName))
                    {
                        return null;
                    }
                    break;

                case "main menu//budget":
                    if (!CheckSecurity(fullName))
                    {
                        return null;
                    }
                    break;
                    //case "main menu//license":
                    //     //if (!CheckSecurity(fullName))
                    //     //{
                    //     //    return null;
                    //     //}
                    //    License = new frmLicenseForm();
                    //    return License;
                    //    break;

            }

            /////////////////// For Non-Child Nodes /////////////////////
            switch (title)
            {

                case "Event Management":
                    //if (!CheckSecurity("main//" + fullName))
                    //{
                    //    return null;
                    //}
                    frmEventMgt = new frmEventManagment();

                    return frmEventMgt;

                //case "Event Document Browser":
                //    frmEventMgt = new frmEventManagment(CNET.ERP.Client.UI_Logic.PMS.Forms.Pinned_Tabs.frmEventManagment.FromOpenedType.EventDocumentBrowser);
                //    return frmEventMgt;
                //    break;
                //case "Event Reports":
                //    frmEventMgt = new frmEventManagment(CNET.ERP.Client.UI_Logic.PMS.Forms.Pinned_Tabs.frmEventManagment.FromOpenedType.EventReports);
                //    return frmEventMgt;
                //    break;
                case "ERP Update":
                    StartUpdate(true);
                    return null;
                    break;
                case "Task Assignment":
                    if (!CheckHKSecurity(title))
                    {
                        return null;
                    }
                    taskAssignment = new frmTaskAssignment();
                    return taskAssignment;
                    break;

                case "Room Management":
                    if (!CheckHKSecurity(title))
                    {
                        return null;
                    }
                    houseKeeping = new frmHouseKeepingMgmt();
                    return houseKeeping;
                // break;

                case "Discrepancy":
                    if (!CheckHKSecurity(title))
                    {
                        return null;
                    }
                    descripancy = new frmDescripancy();
                    return descripancy;
                // break;
                case "Turndown Management":
                    if (!CheckHKSecurity(title))
                    {
                        return null;
                    }
                    turnDown = new frmTurndwnMgmt();
                    return turnDown;
                    break;

                case "Report":
                    report = new Reportfrm(false, null);
                    return report;
                    //case "DAILY DETAIL":

                    //    dailyDetail = new frmDailyDetail();

                    //    return dailyDetail;

                    //    break;



            }

            return null;
        }
        #region Security

        private static bool CheckHKSecurity(String selectedName)
        {
            try
            {
                 IsFromSecurity = true;
                 List<String> approvedFunctionalities = LocalBuffer.LocalBuffer.AllAccessMatrixFunctionList.Select(x => x.Description).ToList();

                  if (IsFunctionExists(approvedFunctionalities, selectedName))
                  {
                      return true;
                  }
                  else
                  {
                      return false;
                  }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private static bool IsFunctionExists(List<string> approvedFunctionalities, string selectedName)
        {
            foreach (String str in approvedFunctionalities)
            {
                if (str.ToLower().Trim().Equals(selectedName.ToLower().Trim()))
                {
                    return true;
                }
            }
            return false;
        }
       
        private static bool CheckSecurity(String fullName)
        {
            try
            {
                IsFromSecurity = true;

                String selectedName = fullName.Split("//".ToCharArray()).ElementAt(2);
                List<String> approvedFunctionalities = LocalBuffer.LocalBuffer.AllAccessMatrixFunctionList.Select(x => x.Description).ToList();

                if (IsFunctionExists(approvedFunctionalities, selectedName))
                {
                    return true;
                }
                else
                {
                    return false;
                } 
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private static void GetCatagoryPMS(string selectedName)
        {
            /* switch (selectedName)
             {
                 case "reservation":
                     catagory = "New Registration";
                     SubSystemComponent = CNETConstantes.SECURITYFrontDeskRoutines;
                     break;
                 case "check in":
                     catagory = "New Registration";
                     SubSystemComponent = CNETConstantes.SECURITYFrontDeskRoutines;
                     break;
                 case "group registration":
                     catagory = "New Registration";
                     SubSystemComponent = CNETConstantes.SECURITYFrontDeskRoutines;
                     break;
                 case "guest":
                     catagory = "New Profile";
                     SubSystemComponent = CNETConstantes.SECURITYFrontDeskRoutines;
                     break;
                 case "contact":
                     catagory = "New Profile";
                     SubSystemComponent = CNETConstantes.SECURITYFrontDeskRoutines;
                     break;
                 case "company":
                     catagory = "New Profile";
                     SubSystemComponent = CNETConstantes.SECURITYFrontDeskRoutines;
                     break;
                 case "travel agent":
                     catagory = "New Profile";
                     SubSystemComponent = CNETConstantes.SECURITYFrontDeskRoutines;
                     break;
                 case "source":
                     catagory = "New Profile";
                     SubSystemComponent = CNETConstantes.SECURITYFrontDeskRoutines;
                     break;
                 case "group":
                     catagory = "New Profile";
                     SubSystemComponent = CNETConstantes.SECURITYFrontDeskRoutines;
                     break;
                 case "end of day":
                     catagory = "Night Audit";
                     SubSystemComponent = CNETConstantes.SECURITYFrontDeskRoutines;
                     break;
                 case "end of month":
                     catagory = "Night Audit";
                     SubSystemComponent = CNETConstantes.SECURITYFrontDeskRoutines;
                     break;
                 case "setting":
                     catagory = "Setting And Miscellaneous";
                     SubSystemComponent = CNETConstantes.SECURITYFrontDeskRoutines;
                     break;
                 case "lookup":
                     catagory = "Setting And Miscellaneous";
                     SubSystemComponent = CNETConstantes.SECURITYFrontDeskRoutines;
                     break;
                 case "property":
                     catagory = "Setting And Miscellaneous";
                     SubSystemComponent = CNETConstantes.SECURITYFrontDeskRoutines;
                     break;
                 case "package":
                     catagory = "Setting And Miscellaneous";
                     SubSystemComponent = CNETConstantes.SECURITYFrontDeskRoutines;
                     break;
                 case "revenue management":
                     catagory = "Setting And Miscellaneous";
                     SubSystemComponent = CNETConstantes.SECURITYFrontDeskRoutines;
                     break;
                 case "calendar":
                     catagory = "Setting And Miscellaneous";
                     SubSystemComponent = CNETConstantes.SECURITYFrontDeskRoutines;
                     break;
                 case "budget":
                     catagory = "Setting And Miscellaneous";
                     SubSystemComponent = CNETConstantes.SECURITYFrontDeskRoutines;
                     break;
                 case "Task Assignment":
                     catagory = "Housekeeping";
                     SubSystemComponent = CNETConstantes.SECURITYHKManagementSystem;
                     break;
                 case "Room Management":
                     catagory = "Housekeeping";
                     SubSystemComponent = CNETConstantes.SECURITYHKManagementSystem;
                     break;
                 case "Discrepancy":
                     catagory = "Housekeeping";
                     SubSystemComponent = CNETConstantes.SECURITYHKManagementSystem;
                     break;
                 case "Turndown Management":
                     catagory = "Housekeeping";
                     SubSystemComponent = CNETConstantes.SECURITYHKManagementSystem;
                     break;
                 case "room inventory":
                     catagory = "Document Browser";
                     SubSystemComponent = CNETConstantes.SECURITYDocumentBrowser;
                     break;
                 case "package audit":
                     catagory = "Document Browser";
                     SubSystemComponent = CNETConstantes.SECURITYDocumentBrowser;
                     break;

                 case "event management":
                     catagory = "Document Browser";
                     SubSystemComponent = CNETConstantes.SECURITYDocumentBrowser;
                     break;

                 case "registration document":
                     catagory = "Document Browser";
                     SubSystemComponent = CNETConstantes.SECURITYDocumentBrowser;
                     break;
             }*/
        }

        private static String catagory = "";
        private static String SubSystemComponent = "";
        public static bool CheckClosing(bool showmessage = true)
        {
            /*   bool enforceClosing = false;
               Configuration EnforceClosing = LocalBuffer.LocalBuffer.ConfigurationBufferList.FirstOrDefault(x => x.preference == "PMS" && x.attribute == "Enforce Closing");
               if (EnforceClosing != null)
               {
                   enforceClosing = Convert.ToBoolean(EnforceClosing.currentValue);
               }

               if (enforceClosing)
               {
                   List<Period> PeriodList = LocalBuffer.LocalBuffer.PeriodBufferList.Where(x => x.type == CNETConstantes.PERIOD_TYPE_PMS).ToList();
                   if (PeriodList == null || PeriodList.Count == 0)
                   {
                       if (showmessage)
                       {
                           XtraMessageBox.Show("There is no PMS Period Maintained !!!", "Night Audit", MessageBoxButtons.OK, MessageBoxIcon.Error);
                       }
                       return false;
                   }
                   DateTime TodayDate = UIProcessManager.GetDataTime().Timestamp;
                   Period TodayPMSPeriod = LocalBuffer.LocalBuffer.PeriodBufferList.FirstOrDefault(x => x.start.Date == TodayDate.Date && x.type == CNETConstantes.PERIOD_TYPE_PMS);
                   if (TodayPMSPeriod == null)
                   {
                       if (showmessage)
                       {
                           XtraMessageBox.Show("There is no PMS Period Maintained For Today !!!", "Night Audit", MessageBoxButtons.OK, MessageBoxIcon.Error);
                       }
                       return false;

                   }
                   else
                   {
                       List<ClosingValidation> TodayClosing = UIProcessManager.SelectClosingValidationByPeriod(TodayPMSPeriod.code);
                       if (TodayClosing != null && TodayClosing.Count > 0)
                       {
                           if (showmessage)
                           {
                               XtraMessageBox.Show("Today's Night Audit is already made  !!!", "Night Audit", MessageBoxButtons.OK, MessageBoxIcon.Error);
                           }
                           return false;
                       }
                   }


                   Period YesterdayPMSPeriod = LocalBuffer.LocalBuffer.PeriodBufferList.FirstOrDefault(x => x.start.Date == TodayDate.AddDays(-1).Date && x.type == CNETConstantes.PERIOD_TYPE_PMS);
                   if (YesterdayPMSPeriod != null)
                   {
                       List<ClosingValidation> YesterdayClosing = UIProcessManager.SelectClosingValidationByPeriod(YesterdayPMSPeriod.code);
                       if (YesterdayClosing == null || YesterdayClosing.Count == 0)
                       {
                           if (showmessage)
                           {
                               XtraMessageBox.Show("Yesterday Night Audit was not made. Please Night Audit is mandatory!!!", "Night Audit", MessageBoxButtons.OK, MessageBoxIcon.Error);
                           }
                           return false;
                       }
                   }
                   else
                   {
                       if (showmessage)
                       {
                           XtraMessageBox.Show("There is no PMS Period Maintained For Yesterday !!!", "Night Audit", MessageBoxButtons.OK, MessageBoxIcon.Error);
                       }
                       return false;
                   }
               }*/
            return true;
        }

        #endregion

        public static void StartUpdate(bool fromhome = false)
        {
            if (Environment.Is64BitProcess)
            {
                UpdateLauncherConfigurations();
                var p = new Process();
                p.StartInfo.FileName = "ERPUpdaterV1.1.exe";
                p.Start();
                if (fromhome)
                    home.Close();
            }
            else
                XtraMessageBox.Show("The Application is not 64bit Contact System Administrator Need Manual Update !!");
        }
        public static bool UpdateLauncherConfigurations()
        {
            try
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(Application.StartupPath + "//ERPUpdaterV1.1.dll");
                config.AppSettings.Settings["Launcher"].Value = "true";
                config.AppSettings.Settings["API"].Value = HttpSinglton.BaseAddressValue;
                config.Save();
                return true;
            }
            catch (Exception io)
            {
                return false;
            }
        } 
    }
}
