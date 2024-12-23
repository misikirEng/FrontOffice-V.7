
using DevExpress.XtraEditors;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using CNET.ERP.Client.Common.UI;
using CNET.FrontOffice_V._7.Forms.Non_Navigatable_Modals;
using DevExpress.XtraGrid.Views.Grid;
using System.Drawing;
using System.Globalization;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraBars;
using System.Windows.Forms;
using CNET.FrontOffice_V._7.Forms.State_Change;
using CNET.FrontOffice_V._7.Forms.Vouchers;
using CNET.ERP.ResourceProvider;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraReports.UI;
using DevExpress.Utils.Menu;
using System.Text;
using System.Runtime.InteropServices;
using CNET_V7_Domain;
using CNET_V7_Domain.Domain.SecuritySchema;
using CNET_V7_Domain.Domain.ArticleSchema;
using CNET_V7_Domain.Domain.SettingSchema;
using CNET_V7_Domain.Domain.CommonSchema;
using CNET_V7_Domain.Domain.TransactionSchema;
using CNET.FrontOffice_V._7.PMS.DoorLockIntegration.DoorLock;
using CNET.FrontOffice_V._7.PMS.Common_Classes;
using CNET.FrontOffice_V._7.Forms;
using ProcessManager;
using CNET_V7_Domain.Misc.PmsDTO;
using CNET_V7_Domain.Misc.PmsView;
using CNET_V7_Domain.Domain.PmsSchema;
using CNET_V7_Domain.Domain.ViewSchema;
using CNET_V7_Domain.Domain.ConsigneeSchema;
using DevExpress.CodeParser;
using CNET.Progress.Reporter;
using CNET.FrontOffice_V._7.Non_Navigatable_Modals;
using static CNET.FrontOffice_V._7.MasterPageForm;
using DocumentPrint;

namespace CNET.FrontOffice_V._7
{
    public partial class RegistrationList : UILogicBase
    {

        frmTravelDetail _frmTravelDetail;
        frmRateSummery _frmRateSummery;
        frmReconciliation _frmReconciliation;
        frmRoomMove _frmRoomMove;
        frmAccompanyingGuest _frmAccompanyingGuest;
        frmProfileAmendment _frmProfileAmendment;
        frmDateAmendment _frmRegistrationDateAmendement;
        frmDailyRateDetail _frmDailyRateDetail;
        frmFolio _frmFolio;
        frmReservationDetail _frmRegistrationDetail;

        UserDTO currentUser = new UserDTO();
        DeviceDTO device = new DeviceDTO();

        DateTime? _currentTime;

        private ActivityDefinitionDTO adSeen = null;
        private List<ActivityDTO> allActivity = null;
        //  private List<ServiceRequestDTO> _serviceReqDtoList = new List<ServiceRequestDTO>();
        public List<TransactionReferenceDTO> AllTransactionReference { get; set; }

        private IDoorLock _doorLock = null;

        private List<RegistrationDocumentDTO> _regDocList = null;
        private List<RegistrationListVMDTO> regListVM = new List<RegistrationListVMDTO>();


        private bool isFromAdvanced = false;

        public DateTime? CurrentTime { get; set; }



        public string FilterKey { get; set; }


        public DashboardFilterRefresh DashboardFilterRefreshHandler { get; set; }

        private int _zoomLevel = 100;



        // List<VwMessageViewDTO> MessageList { get; set; }

        System.Timers.Timer Checkmessage { get; set; }

        ///////////////////////////// CONSTRUCTOR ////////////////////////////////////////////

        public RegistrationList()
        {
            InitializeComponent();

            DashboardFilterRefreshHandler = DashboardFilterRefresher;

            InitializeUI();

            InitializeData();

            StartMessageTimer();


        }

        private void StartMessageTimer()
        {
            ConfigurationDTO CheckMessageFrequency = LocalBuffer.LocalBuffer.ConfigurationBufferList.FirstOrDefault(x => x.Attribute == "Message Check Frequency");

            Checkmessage = new System.Timers.Timer();
            if (CheckMessageFrequency != null && CheckMessageFrequency.CurrentValue != "0")
                Checkmessage.Interval = Convert.ToInt16(CheckMessageFrequency.CurrentValue) * 1000;
            else
                Checkmessage.Interval = 60000;
            Checkmessage.AutoReset = true;
            Checkmessage.Enabled = true;
            Checkmessage.Elapsed += Checkmessage_Elapsed;
            Checkmessage.Start();
        }

        bool wait = false;
        private void Checkmessage_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            //if (!wait)
            //{
            //    wait = true;
            //    string loggedInUser = LocalBuffer.LocalBuffer.CurrentLoggedInUser.code;
            //    List<VwMessageViewDTO> checkLogBufferList = UIProcessManager.GetMessagesByTypeAndDestUser(CNETConstantes.INTERNAL_MESSAGE, loggedInUser);
            //    if (checkLogBufferList != null && checkLogBufferList.Count > 0)
            //    {
            //        if (checkLogBufferList.Count > MasterPageForm.LogBufferList.Count)
            //        {
            //            List<string> OldMessage = MasterPageForm.LogBufferList.Select(x => x.messageCode).ToList();
            //            List<string> AllMessage = checkLogBufferList.Select(x => x.messageCode).ToList();
            //            AllMessage.AddRange(OldMessage);

            //            List<messagecount> numberGroups = AllMessage.GroupBy(i => i).Select(grp => new messagecount { messagecode = grp.Key, count = grp.Count(), }).ToList();
            //            numberGroups = numberGroups.Where(x => x.count == 1).ToList();

            //            foreach (messagecount message in numberGroups)
            //            {
            //                Message_View Newmessage = checkLogBufferList.FirstOrDefault(x => x.messageCode == message.messagecode);
            //                string NewmessageInfo = "Registration:- " + Newmessage.regNumber + Environment.NewLine +
            //                              "Sender Name:-  " + Newmessage.senderName + Environment.NewLine +
            //                              "Message :-  " + Newmessage.message + Environment.NewLine;

            //                ShowAlertMessageInformation(NewmessageInfo);
            //            }
            //            MasterPageForm.LogBufferList = checkLogBufferList;
            //            this.Invoke((MethodInvoker)delegate
            //            {
            //                bbiRefresh.PerformClick();
            //            });
            //        }
            //    }
            //    wait = false;
            //}
            //else
            //{
            //}
        }

        // PopupNotifier popup { get; set; }
        public void ShowAlertMessageInformation(string Message)
        {
            try
            {
                //this.Invoke((MethodInvoker)delegate
                //{
                //    popup = new PopupNotifier();
                //    popup.Scroll = true;
                //    popup.Delay = 10000;
                //    popup.Image = Provider.GetImage("information", ProviderType.APPLICATIONICON, PictureSize.Dimension_16X16);
                //    popup.ContentFont = new System.Drawing.Font("Arial", 9, FontStyle.Regular);
                //    popup.ImagePadding = new System.Windows.Forms.Padding(2, 2, 10, 2);
                //    popup.TitleText = "New Message Notification";
                //    popup.ContentText = Message;
                //    popup.Popup();
                //});
            }
            catch (Exception io)
            {
            }
        }
        public class messagecount
        {
            public string messagecode { get; set; }
            public int count { get; set; }
        }



        //////////////////////////// METHODS  /////////////////////////////////////////////////

        #region SEcurity Method

        private void SecurityCheck(int? state)
        {
            EnableDisableBarItems("Options", bsiOptions);
            EnableDisableBarItems("Activities", bsiActivites, state);
            EnableDisableBarItems("Billing", bsiBilling);
            EnableDisableBarItems("Amendment", bsiAmendment);
            EnableDisableBarItems("Preview", bsiPreview);
        }

        private void DisableButtons()
        {
            DisableBarItems("Options", bsiOptions);
            DisableBarItems("Activities", bsiActivites);
            DisableBarItems("Billing", bsiBilling);
            DisableBarItems("Amendment", bsiAmendment);
            DisableBarItems("Preview", bsiPreview);
        }
        private void DisableBarItems(String catagory, BarSubItem barItem)
        {

            bool oneTrue = false;
            foreach (BarItemLink itemLink in barItem.ItemLinks)
            {
                itemLink.Item.Enabled = false;
            }
            barItem.Enabled = true;
        }
        private void EnableDisableBarItems(String catagory, BarSubItem barItem, int? state = null)
        {

            List<String> approvedFunctionalities = LocalBuffer.LocalBuffer.AllAccessMatrixFunctionList.Where(x => x.VisuaCompDesc == "Registration Document").Select(x => x.Description).ToList();

            bool oneTrue = false;
            foreach (BarItemLink itemLink in barItem.ItemLinks)
            {
                if (itemLink.DisplayCaption != "Reinstate")
                {
                    if (itemLink.DisplayCaption != "Package")
                    {
                        if (!IsFunctionExists(approvedFunctionalities, itemLink.DisplayCaption))
                        {
                            itemLink.Item.Enabled = false;
                        }
                        else
                        {
                            oneTrue = true;
                        }
                    }
                }
                else
                {
                    if (state == CNETConstantes.CHECKED_IN_STATE)
                    {
                        if (!IsFunctionExists(approvedFunctionalities, "Checked In Reinstate"))
                        {
                            itemLink.Item.Enabled = false;
                        }
                        else
                        {
                            oneTrue = true;
                        }
                    }
                    else if (state == CNETConstantes.CHECKED_OUT_STATE)
                    {
                        if (!IsFunctionExists(approvedFunctionalities, "Checked Out Reinstate"))
                        {
                            itemLink.Item.Enabled = false;
                        }
                        else
                        {
                            oneTrue = true;
                        }
                    }
                    else if (state == CNETConstantes.OSD_CANCEL_STATE)
                    {
                        if (!IsFunctionExists(approvedFunctionalities, "Cancel Reinstate"))
                        {
                            itemLink.Item.Enabled = false;
                        }
                        else
                        {
                            oneTrue = true;
                        }
                    }
                    //else if (state == CNETConstantes.ONLINE_CHECKED_OUT_STATE)
                    //{
                    //    if (!IsFunctionExists(approvedFunctionalities, "Cancel Reinstate"))
                    //    {
                    //        itemLink.Item.Enabled = false;
                    //    }
                    //    else
                    //    {
                    //        oneTrue = true;
                    //    }
                    //}

                }
            }
            if (!oneTrue)
            {
                barItem.Enabled = false;
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



        #endregion

        #region Filtering Methods

        public List<RegistrationListVMDTO> FilterByState(List<RegistrationListVMDTO> regVM)
        {
            List<RegistrationListVMDTO> filteredList = regVM;
            if (beiGuest.EditValue != null && beiGuest.EditValue != "" && filteredList != null)
            {
                filteredList = filteredList.Where(l => l.Guest != null && l.Guest.ToLower().Contains(beiGuest.EditValue.ToString().ToLower())).ToList();
            }
            if (beiRoom.EditValue != null && beiRoom.EditValue != "" && filteredList != null)
            {
                filteredList = filteredList.Where(l => l.Room != null && l.Room.Contains(beiRoom.EditValue.ToString())).ToList();
            }

            if (filteredList == null) return null;
            BindGridDataSource(filteredList);

            return filteredList;
        }

        private void RefreshRegDocBrowser()
        {
            if (SelectedHotelcode == null)
            {
                SystemMessage.ShowModalInfoMessage("Please Select Hotel !!");
                return;
            }
            if (beiState.EditValue != null && !string.IsNullOrEmpty(beiState.EditValue.ToString()))
            {
                int? state = Convert.ToInt32(beiState.EditValue);
                if (state == CNETConstantes.SIX_PM_STATE || state == CNETConstantes.GAURANTED_STATE || state == CNETConstantes.OSD_WAITLIST_STATE)
                {
                    // if (beiArrival.EditValue == null) beiArrival.EditValue = CurrentTime;
                }
                else if (state == CNETConstantes.CHECKED_OUT_STATE)
                {
                    // if (beiDeparture.EditValue == null) beiDeparture.EditValue = CurrentTime;
                }
                if (state == CNETConstantes.SIX_PM_STATE || state == CNETConstantes.GAURANTED_STATE)
                {
                    LocalBuffer.LocalBuffer.LoadVoucherConsigneeBuffer();
                    // MasterPageForm.LoadLogBuffer();
                }
                else if (state == 0)
                    state = null;

                LoadRegistrationDocument(state, (DateTime?)beiArrival.EditValue, (DateTime?)beiDeparture.EditValue);
                BindGridDataSource(regListVM);
            }
            else
                SystemMessage.ShowModalInfoMessage("Please Select State First !!");

        }

        private List<RegistrationListVMDTO> FilterByGuest(int consigneeCode)
        {

            if (regListVM == null) return null;
            List<RegistrationListVMDTO> filteredList = null;
            if (consigneeCode != null)
            {
                filteredList = regListVM.Where(l => l.GuestId == consigneeCode).ToList();
            }
            if (beiRoom.EditValue != null && beiRoom.EditValue != "" && filteredList != null)
            {
                filteredList = filteredList.Where(l => l.Room != null && l.Room.Contains(beiRoom.EditValue.ToString())).ToList();
            }
            if (beiCompanySearch.EditValue != null && beiCompanySearch.EditValue != "" && filteredList != null)
            {
                if (beiCompanySearch.Caption == "Company ")
                {
                    int company = Convert.ToInt32(beiCompanySearch.EditValue);
                    filteredList = regListVM.Where(l => l.CompanyId == company).ToList();
                    //filteredList = FilterOtherConsignees(filteredList);

                }
                else if (beiCompanySearch.Caption == "       Agent")
                {
                    int agent = Convert.ToInt32(beiCompanySearch.EditValue);
                    filteredList = regListVM.Where(l => l.AgentId == agent).ToList();
                    // filteredList = FilterOtherConsignees(filteredList);
                }
                else if (beiCompanySearch.Caption == "     Source")
                {
                    int Source = Convert.ToInt32(beiCompanySearch.EditValue);
                    filteredList = regListVM.Where(l => l.SourceId == Source).ToList();
                    //filteredList = FilterOtherConsignees(filteredList);
                }
                else if (beiCompanySearch.Caption == "      Group")
                {
                    int Group = Convert.ToInt32(beiCompanySearch.EditValue);
                    filteredList = regListVM.Where(l => l.GroupId == Group).ToList();
                    //filteredList = FilterOtherConsignees(filteredList);
                }
                else if (beiCompanySearch.Caption == "   Contact")
                {
                    int Contact = Convert.ToInt32(beiCompanySearch.EditValue);
                    filteredList = regListVM.Where(l => l.ContactId == Contact).ToList();
                    //filteredList = FilterOtherConsignees(filteredList);
                }
            }
            if (filteredList == null) return null;
            BindGridDataSource(filteredList);

            return filteredList;
        }

        private List<RegistrationListVMDTO> FilterByCompany()
        {

            List<RegistrationListVMDTO> filteredList = null;
            //  List<RegistrationListVM> Customfilter = (List<RegistrationListVM>)grid_regDoc.DataSource;

            if (regListVM == null) return null;
            if (beiCompanySearch.EditValue != null && beiCompanySearch.EditValue != "")
            {
                if (beiCompanySearch.Caption == "Company ")
                {
                    int company = Convert.ToInt32(beiCompanySearch.EditValue);
                    filteredList = regListVM.Where(l => l.CompanyId == company).ToList();
                    //filteredList = FilterOtherConsignees(filteredList);
                }
                else if (beiCompanySearch.Caption == "       Agent")
                {
                    int agent = Convert.ToInt32(beiCompanySearch.EditValue);
                    filteredList = regListVM.Where(l => l.AgentId == agent).ToList();
                    // filteredList = FilterOtherConsignees(filteredList);
                }
                else if (beiCompanySearch.Caption == "     Source")
                {
                    int Source = Convert.ToInt32(beiCompanySearch.EditValue);
                    filteredList = regListVM.Where(l => l.SourceId == Source).ToList();
                    //filteredList = FilterOtherConsignees(filteredList);
                }
                else if (beiCompanySearch.Caption == "      Group")
                {
                    int Group = Convert.ToInt32(beiCompanySearch.EditValue);
                    filteredList = regListVM.Where(l => l.GroupId == Group).ToList();
                    //filteredList = FilterOtherConsignees(filteredList);
                }
                else if (beiCompanySearch.Caption == "   Contact")
                {
                    int Contact = Convert.ToInt32(beiCompanySearch.EditValue);
                    filteredList = regListVM.Where(l => l.ContactId == Contact).ToList();
                    //filteredList = FilterOtherConsignees(filteredList);
                }

                //if (beiCompanySearch.Caption == "Company ")
                //{
                //    PopulateSearchCriteria("Company");
                //    filteredList = FilterOtherConsignees(regListVM);
                //}
                //else if (beiCompanySearch.Caption == "       Agent")
                //{
                //    PopulateSearchCriteria("Agent");
                //    filteredList = FilterOtherConsignees(regListVM);
                //}
                //else if (beiCompanySearch.Caption == "     Source")
                //{
                //    PopulateSearchCriteria("Source");
                //    filteredList = FilterOtherConsignees(regListVM);
                //}
                //else if (beiCompanySearch.Caption == "      Group")
                //{
                //    PopulateSearchCriteria("Group");
                //    filteredList = FilterOtherConsignees(regListVM);
                //}
                //else if (beiCompanySearch.Caption == "   Contact")
                //{
                //    PopulateSearchCriteria("Contact");
                //    filteredList = FilterOtherConsignees(regListVM);
                //}
            }

            if (beiGuest.EditValue != null && beiGuest.EditValue != "" && filteredList != null)
            {
                filteredList = filteredList.Where(
                        l => l.Guest != null && l.Guest == beiGuest.EditValue.ToString().ToLower()).ToList();
            }
            if (beiRoom.EditValue != null && beiRoom.EditValue != "" && filteredList != null)
            {
                filteredList = filteredList.Where(l => (l.Room != null && l.Room.Contains(beiRoom.EditValue.ToString()))).ToList();
            }

            /*
             if (rpgDateRange.Visible && beiStartDate.EditValue != null && beiEndDate.EditValue != null)
              {
                  DateTime Startdate = Convert.ToDateTime(beiStartDate.EditValue.ToString()).Date;
                  DateTime Enddate = Convert.ToDateTime(beiEndDate.EditValue.ToString()).Date.AddHours(23).AddMinutes(59).AddSeconds(59);

                  if (Startdate <= Enddate)
                  {
                    filteredList=  FilterByDateRange(Startdate, Enddate, filteredList);
                  }
             }*/

            if (filteredList == null) return null;
            BindGridDataSource(filteredList);

            return filteredList;
        }

        private List<RegistrationListVMDTO> FilterByRegNumber(string regNumSearchKey)
        {
            if (regListVM == null) return null;
            if (string.IsNullOrEmpty(regNumSearchKey)) return null;
            List<RegistrationListVMDTO> filteredList = regListVM.Where(r => r.Registration.Contains(regNumSearchKey)).ToList();
            if (filteredList == null) return null;
            BindGridDataSource(filteredList);

            return filteredList;
        }
        private List<RegistrationListVMDTO> FilterByDoorlock(string regNumSearchKey)
        {
            if (regListVM == null) return null;
            if (string.IsNullOrEmpty(regNumSearchKey)) return null;
            List<RegistrationListVMDTO> filteredList = regListVM.Where(r => r.Doorlock != null && r.Doorlock.Contains(regNumSearchKey) /*== regNumSearchKey*/).ToList();
            if (filteredList == null || filteredList.Count == 0)
            {
                SystemMessage.ShowModalInfoMessage("There Is No Room for the Door Lock Key ("+ regNumSearchKey + ") !!", "ERROR");
                BindGridDataSource(regListVM);
                return regListVM;
            }
            else
                BindGridDataSource(filteredList);

            return filteredList;
        }
        private List<RegistrationListVMDTO> FilterByDateRange(DateTime startDate, DateTime endDate)
        {
            LoadRegistrationDocument(Convert.ToInt32(beiState.EditValue), null, null);
            if (regListVM == null || beiRangeBy.EditValue == null) return null;
            List<RegistrationListVMDTO> filteredList = null;
            if (beiRangeBy.EditValue.ToString() == "Arrival")
            {
                //Date Range By Arrival
                filteredList = regListVM.Where(r => DateTime.Compare(r.Arrival.Date, startDate) >= 0 && DateTime.Compare(r.Arrival.Date, endDate) <= 0).ToList();

            }
            else if (beiRangeBy.EditValue.ToString() == "Departure")
            {
                //Date Range By Departure
                filteredList = regListVM.Where(r => DateTime.Compare(r.Departure.Date, startDate) >= 0 && DateTime.Compare(r.Departure.Date, endDate) <= 0).ToList();

            }
            else if (beiRangeBy.EditValue.ToString() == "Issued Date")
            {
                //Date Range By Issued Date
                filteredList = regListVM.Where(r => DateTime.Compare(r.RegistrationDate.Date, startDate) >= 0 && DateTime.Compare(r.RegistrationDate.Date, endDate) <= 0).ToList();

            }
            if (filteredList == null) return null;
            BindGridDataSource(filteredList);

            return filteredList;
        }
        private List<RegistrationListVMDTO> FilterByDateRange(DateTime startDate, DateTime endDate, List<RegistrationListVMDTO> filtered)
        {
            if (filtered != null)
            {
                regListVM = filtered;
            }
            else
            {
                LoadRegistrationDocument(Convert.ToInt32(beiState.EditValue), null, null);
            }
            List<RegistrationListVMDTO> filteredList = null;

            if (beiRangeBy.EditValue == null || string.IsNullOrEmpty(beiRangeBy.EditValue.ToString()))
            {
                SystemMessage.ShowModalInfoMessage("Please Select Range Search By.", "ERROR");
                return null;
            }

            if (beiRangeBy.EditValue.ToString() == "Arrival")
            {
                //Date Range By Arrival
                filteredList = regListVM.Where(r => DateTime.Compare(r.Arrival.Date, startDate) >= 0 && DateTime.Compare(r.Arrival.Date, endDate) <= 0).ToList();

            }
            else if (beiRangeBy.EditValue.ToString() == "Departure")
            {
                //Date Range By Departure
                filteredList = regListVM.Where(r => DateTime.Compare(r.Departure.Date, startDate) >= 0 && DateTime.Compare(r.Departure.Date, endDate) <= 0).ToList();

            }
            else if (beiRangeBy.EditValue.ToString() == "Issued Date")
            {
                //Date Range By Issued Date
                filteredList = regListVM.Where(r => DateTime.Compare(r.RegistrationDate.Date, startDate) >= 0 && DateTime.Compare(r.RegistrationDate.Date, endDate) <= 0).ToList();

            }
            if (filteredList == null) return null;
            BindGridDataSource(filteredList);

            return filteredList;
        }

        private List<RegistrationListVMDTO> FilterByMarket(string marketKey)
        {
            if (regListVM == null) return null;
            if (string.IsNullOrEmpty(marketKey)) return null;
            List<RegistrationListVMDTO> filteredList = regListVM.Where(r => r.Market.Contains(marketKey)).ToList();
            if (filteredList == null) return null;
            BindGridDataSource(filteredList);

            return filteredList;
        }

        private List<RegistrationListVMDTO> FilterByRoom(string roomKey)
        {
            List<RegistrationListVMDTO> filteredList = new List<RegistrationListVMDTO>();
            filteredList = regListVM.Where(l => l.Room != null ? l.Room.Contains(roomKey) : false).ToList();

            BindGridDataSource(filteredList);

            return filteredList;
        }

        private void PopulateSearchCriteria(string advancSearch = "", string dashboard = "", string roomNum = "")
        {
            beiSearchCriteria.EditValue = null;
            string state = "All";

            if (beiState.EditValue != null)
            {
                if (beiState.EditValue.ToString() == "0")
                {
                    state = "All";
                }
                else
                {

                    state = riluState.GetDisplayTextByKeyValue(beiState.EditValue.ToString());
                }
            }

            string room = "";
            if (!string.IsNullOrEmpty(roomNum))
            {
                room = roomNum;
            }

            string guest = "";
            if (beiGuest.EditValue != null)
            {
                guest = risleGuest.GetDisplayTextByKeyValue(beiGuest.EditValue.ToString());
            }

            string arrival = "";
            if (beiArrival.EditValue != null)
            {
                DateTime val = (DateTime)beiArrival.EditValue;
                arrival = val.ToShortDateString();
            }

            string departure = "";
            if (beiDeparture.EditValue != null)
            {
                DateTime val = (DateTime)beiDeparture.EditValue;
                departure = val.ToShortDateString();
            }

            string company = "";
            if (advancSearch.Contains("Company") && beiCompanySearch.EditValue != null)
            {
                company = risleCompany.GetDisplayTextByKeyValue(beiCompanySearch.EditValue.ToString());
            }

            string agent = "";
            if (advancSearch.Contains("Agent") && beiCompanySearch.EditValue != null)
            {
                agent = risleCompany.GetDisplayTextByKeyValue(beiCompanySearch.EditValue.ToString());
            }

            string source = "";
            if (advancSearch.Contains("Source") && beiCompanySearch.EditValue != null)
            {
                source = risleCompany.GetDisplayTextByKeyValue(beiCompanySearch.EditValue.ToString());
            }

            string group = "";
            if (advancSearch.Contains("Group") && beiCompanySearch.EditValue != null)
            {
                group = risleCompany.GetDisplayTextByKeyValue(beiCompanySearch.EditValue.ToString());
            }

            string contact = "";
            if (advancSearch.Contains("Contact") && beiCompanySearch.EditValue != null)
            {
                contact = risleCompany.GetDisplayTextByKeyValue(beiCompanySearch.EditValue.ToString());
            }

            string market = "";
            if (advancSearch.Contains("Market") && beiMarket.EditValue != null)
            {
                market = beiMarket.EditValue.ToString();
            }

            string regNumber = "";
            if (advancSearch.Contains("Registration") && beiMarket.EditValue != null)
            {
                regNumber = beiMarket.EditValue.ToString();
            }

            string advancedSearch = advancSearch;

            string dashboardFilter = dashboard;

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("State = {0}", state);
            sb.Append("     ");


            if (!string.IsNullOrEmpty(room))
            {
                sb.AppendFormat("Room = {0}", room);
                sb.Append("     ");
            }

            if (!string.IsNullOrEmpty(guest))
            {
                sb.AppendFormat("Guest = {0}", guest);
                sb.Append("     ");
            }
            if (!string.IsNullOrEmpty(arrival))
            {
                sb.AppendFormat("Arrival = {0}", arrival);
                sb.Append("     ");
            }

            if (!string.IsNullOrEmpty(departure))
            {
                sb.AppendFormat("Departure = {0}", departure);
                sb.Append("     ");

            }

            if (!string.IsNullOrEmpty(company))
            {
                sb.AppendFormat("Company = {0}", company);
                sb.Append("     ");
            }
            if (!string.IsNullOrEmpty(market))
            {
                sb.AppendFormat("Market = {0}", market);
                sb.Append("     ");
            }

            if (!string.IsNullOrEmpty(regNumber))
            {
                sb.AppendFormat("Reg. No = {0}", regNumber);
                sb.Append("     ");

            }

            if (!string.IsNullOrEmpty(advancedSearch))
            {
                sb.AppendFormat("Advanced Search = {0}", advancedSearch);
                sb.Append("     ");
            }

            if (!string.IsNullOrEmpty(dashboardFilter))
            {
                sb.AppendFormat("Dashboard Filter = {0}", dashboardFilter);
                sb.Append("     ");

            }


            beiSearchCriteria.EditValue = sb.ToString();
        }

        //filter from dashboard
        private void PopulateByDasboardFilterKey()
        {
            /*
            if (FilterKey == "Guest's Birthday")
            {

                beiDeparture.EditValue = null;
                beiArrival.EditValue = null;
                beiState.EditValue = CNETConstantes.CHECKED_IN_STATE;
                if (regListVM == null) return;
                var filtered = regListVM.Where(r => r.lastState == CNETConstantes.CHECKED_IN_STATE).ToList();
                if (filtered != null && filtered.Count > 0)
                {
                    List<int> filteredGuest = new List<int>();//  UIProcessManager.GetAllPersonViewWithTodaysBirthday(CurrentTime.Value).Select(p => p.code).ToArray();
                    if (filteredGuest != null)
                    {
                        filtered = filtered.Where(r => filteredGuest.Contains(r.GuestId.Value)).ToList();
                    }
                }
                BindGridDataSource(filtered);
            }

            else if (FilterKey == "Over Due Out")
            {
                if (regListVM == null) return;
                beiDeparture.EditValue = null;
                beiArrival.EditValue = null;
                beiState.EditValue = CNETConstantes.CHECKED_IN_STATE;
                var filtered = regListVM.Where(r => r.lastState == CNETConstantes.CHECKED_IN_STATE && r.Departure.Date < CurrentTime.Value.Date).ToList();
                BindGridDataSource(filtered);
            }
            else if (FilterKey == "No Show")
            {
                LoadRegistrationDocument(CNETConstantes.NO_SHOW_STATE, CurrentTime, null);
                beiDeparture.EditValue = null;
                beiArrival.EditValue = CurrentTime;
                beiState.EditValue = CNETConstantes.NO_SHOW_STATE;
                if (regListVM == null) return;
                BindGridDataSource(regListVM);
            }
            else if (FilterKey == "Unassigned Reservation")
            {
                LoadRegistrationDocument(null, null, null);
                beiDeparture.EditValue = null;
                beiArrival.EditValue = null;
                beiState.EditValue = "";
                if (regListVM == null) return;
                var filtered = regListVM.Where(r => string.IsNullOrEmpty(r.Room) || r.Room.Contains("#")).ToList();
                BindGridDataSource(filtered);
            }
            else if (FilterKey == "Waiting List")
            {
                LoadRegistrationDocument(CNETConstantes.OSD_WAITLIST_STATE, CurrentTime, null);

                beiDeparture.EditValue = null;
                beiArrival.EditValue = CurrentTime;
                beiState.EditValue = CNETConstantes.OSD_WAITLIST_STATE;
                if (regListVM == null) return;
                BindGridDataSource(regListVM);
            }

            else if (FilterKey == "Arrival List")
            {
                LoadRegistrationDocument(null, null, null);
                if (regListVM == null) return;

                beiDeparture.EditValue = null;
                beiArrival.EditValue = CurrentTime;
                beiState.EditValue = "";
                var filtered = regListVM.Where(r => (r.lastState == CNETConstantes.SIX_PM_STATE || r.lastState == CNETConstantes.GAURANTED_STATE) && r.Arrival.Date == CurrentTime.Value.Date).ToList();
                BindGridDataSource(filtered);
            }
            else if (FilterKey == "Stay Overs")
            {
                beiDeparture.EditValue = null;
                beiArrival.EditValue = null;
                beiState.EditValue = CNETConstantes.CHECKED_IN_STATE;
                if (regListVM == null) return;
                var filtered = regListVM.Where(r => r.lastState == CNETConstantes.CHECKED_IN_STATE && r.Arrival.Date < CurrentTime.Value.Date && r.Departure.Date > CurrentTime.Value.Date).ToList();
                BindGridDataSource(filtered);
            }
            else if (FilterKey == "Departures")
            {
                LoadRegistrationDocument(CNETConstantes.CHECKED_OUT_STATE, null, CurrentTime);
                beiDeparture.EditValue = CurrentTime;
                beiArrival.EditValue = null;
                beiState.EditValue = CNETConstantes.CHECKED_OUT_STATE;
                if (regListVM == null) return;
                BindGridDataSource(regListVM);
            }
            else if (FilterKey == "Due Out")
            {
                beiDeparture.EditValue = CurrentTime;
                beiArrival.EditValue = null;
                beiState.EditValue = CNETConstantes.CHECKED_IN_STATE;
                RefreshRegDocBrowser();
            }

            else if (FilterKey == "Post Masters")
            {
                if (regListVM == null) return;
                List<string> pseudoRooms = null;
                List<RoomTypeDTO> pseudoRoomList = UIProcessManager.GetRoomTypeByispseudoRoomType(true);
                if (pseudoRoomList != null)
                    pseudoRooms = pseudoRoomList.Select(p => p.Description).ToList();
                if (pseudoRooms != null)
                {
                    var filtered = regListVM.Where(r => r.lastState == CNETConstantes.CHECKED_IN_STATE && pseudoRooms.Contains(r.RoomTypeDescription)).ToList();
                    BindGridDataSource(filtered);
                }
            }
            else if (FilterKey == "Arrived")
            {
                LoadRegistrationDocument(CNETConstantes.CHECKED_IN_STATE, CurrentTime, null);
                beiDeparture.EditValue = null;
                beiArrival.EditValue = CurrentTime;
                beiState.EditValue = CNETConstantes.CHECKED_IN_STATE;
                if (regListVM == null) return;
                BindGridDataSource(regListVM);
            }
            else if (FilterKey == "Cancellations")
            {
                LoadRegistrationDocument(CNETConstantes.OSD_CANCEL_STATE, CurrentTime, null);
                beiDeparture.EditValue = null;
                beiArrival.EditValue = CurrentTime;
                beiState.EditValue = CNETConstantes.OSD_CANCEL_STATE;
                if (regListVM == null) return;
                BindGridDataSource(regListVM);
            }
            else if (FilterKey == "Today's Pickup")
            {
                LoadRegistrationDocument(null, null, null);
                beiDeparture.EditValue = null;
                beiArrival.EditValue = null;
                beiState.EditValue = "";
                if (regListVM == null)
                {
                   ////CNETInfoReporter.Hide();
                    return;
                }

                var allTravelDetail = UIProcessManager.GetTravelDetailViewByDateAndActionRequired(CurrentTime.Value, CNETConstantes.TD_PICK_UP);
                if (allTravelDetail != null && allTravelDetail.Count > 0)
                {
                    string[] regCodes = allTravelDetail.GroupBy(t => t.code).Select(t => t.First()).ToList().Select(t => t.code).ToArray();

                    var filtered = regListVM.Where(r => regCodes.Contains(r.Registration)).ToList();
                    BindGridDataSource(filtered);
                }
                else
                {
                    var filtered = new List<RegistrationListVMDTO>();
                    BindGridDataSource(filtered);
                }

            }
            else if (FilterKey == "Today's Drop-off")
            {
                LoadRegistrationDocument(null, null, null);
                beiDeparture.EditValue = null;
                beiArrival.EditValue = null;
                beiState.EditValue = "";
                if (regListVM == null) return;

                var allTravelDetail = UIProcessManager.GetTravelDetailViewByDateAndActionRequired(CurrentTime.Value, CNETConstantes.TD_DROP_OFF);
                if (allTravelDetail != null && allTravelDetail.Count > 0)
                {
                    string[] regCodes = allTravelDetail.GroupBy(t => t.code).Select(t => t.First()).ToList().Select(t => t.code).ToArray();

                    var filtered = regListVM.Where(r => regCodes.Contains(r.Registration)).ToList();
                    BindGridDataSource(filtered);
                }
                else
                {
                    var filtered = new List<RegistrationListVMDTO>();
                    BindGridDataSource(filtered);
                }
            }
            else if (FilterKey == "Customer High Balance")
            {
                LoadRegistrationDocument(null, null, null);
                beiDeparture.EditValue = null;
                beiArrival.EditValue = null;
                beiState.EditValue = CNETConstantes.CHECKED_IN_STATE;
                if (regListVM == null) return;

                var config = LocalBuffer.LocalBuffer.ConfigurationBufferList.FirstOrDefault(c => c.Reference == CNETConstantes.PMS_Pointer.ToString() && c.Attribute == CNETConstantes.PMS_SETTING_CustHighBalance);

                if (config == null) return;

                decimal threshold = Convert.ToDecimal(config.CurrentValue);

                //var chbList = null;// UIProcessManager.GetCustomerHighBalance(CurrentTime.Value.AddDays(-1), threshold);
                //if (chbList == null || chbList.Count == 0)
                //{
                //    var filtered = new List<RegistrationListVMDTO>();
                //    BindGridDataSource(filtered);
                //}
                //else
                //{
                //    string[] regCHBList = chbList.Select(r => r.code).ToArray();
                //    var filtered = regListVM.Where(r => regCHBList.Contains(r.Registration)).ToList();
                //    BindGridDataSource(filtered);
                //}
            }
            else if (FilterKey == "Room Moved Registrations")
            {
                LoadRegistrationDocument(null, null, null);
                beiDeparture.EditValue = null;
                beiArrival.EditValue = null;
                beiState.EditValue = CNETConstantes.CHECKED_IN_STATE;
                if (regListVM == null)
                {
                   ////CNETInfoReporter.Hide();
                    return;
                }
                List<spGetRegActivities_Result> actList = UIProcessManager.GetRegistrationActivity(CNETConstantes.LU_ACTIVITY_DEFINATION_ROOM_MOVED, CurrentTime.Value);


                if (actList == null || actList.Count == 0)
                {
                    var filtered = new List<RegistrationListVMDTO>();
                    BindGridDataSource(filtered);
                }
                else
                {
                    string[] regList = actList.Select(r => r.code).ToArray();
                    var filtered = regListVM.Where(r => regList.Contains(r.Registration)).ToList();
                    BindGridDataSource(filtered);
                }

               ////CNETInfoReporter.Hide();
                PopulateSearchCriteria("", FilterKey);
            }

            PopulateSearchCriteria("", FilterKey);

            */
            FilterKey = null;
        }

        private void DashboardFilterRefresher(string FilterKey)
        {
            try
            {
                //Progress_Reporter.Show_Progress("Please Wait...");
                //load all registrations
                //LoadRegistrationDocument(null, null, null);
                if (FilterKey == "Guest's Birthday")
                {

                    beiDeparture.EditValue = null;
                    beiArrival.EditValue = null;
                    beiState.EditValue = CNETConstantes.CHECKED_IN_STATE;
                    if (regListVM == null) return;
                    var filtered = regListVM.Where(r => r.lastState == CNETConstantes.CHECKED_IN_STATE).ToList();
                    if (filtered != null && filtered.Count > 0)
                    {
                        int[] filteredGuest = LocalBuffer.LocalBuffer.AllGuestConsigneeViewlist.Where(x => x.StartDate != null && x.StartDate.Value.Day == CurrentTime.Value.Day && x.StartDate.Value.Month == CurrentTime.Value.Month).Select(p => p.Id).ToArray();
                        if (filteredGuest != null)
                        {
                            filtered = filtered.Where(r => r.GuestId != null && filteredGuest.Contains(r.GuestId.Value)).ToList();
                        }
                    }
                    BindGridDataSource(filtered);
                }

                if (FilterKey == "Over Due Out")
                {
                    if (regListVM == null)
                    {
                        ////CNETInfoReporter.Hide();
                        return;

                    }
                    beiDeparture.EditValue = null;
                    beiArrival.EditValue = null;
                    beiState.EditValue = CNETConstantes.CHECKED_IN_STATE;
                    var filtered = regListVM.Where(r => r.lastState == CNETConstantes.CHECKED_IN_STATE && r.Departure.Date < CurrentTime.Value.Date).ToList();
                    BindGridDataSource(filtered);
                }
                else if (FilterKey == "No Show")
                {
                    LoadRegistrationDocument(CNETConstantes.NO_SHOW_STATE, CurrentTime, null);
                    beiDeparture.EditValue = null;
                    beiArrival.EditValue = CurrentTime;
                    beiState.EditValue = CNETConstantes.NO_SHOW_STATE;
                    if (regListVM == null)
                    {
                        ////CNETInfoReporter.Hide();
                        return;
                    }
                    BindGridDataSource(regListVM);
                }
                else if (FilterKey == "Unassigned Reservation")
                {
                    LoadRegistrationDocument(null, null, null);
                    beiDeparture.EditValue = null;
                    beiArrival.EditValue = null;
                    beiState.EditValue = "";
                    if (regListVM == null)
                    {
                        ////CNETInfoReporter.Hide();
                        return;
                    }
                    var filtered = regListVM.Where(r => string.IsNullOrEmpty(r.Room) || r.Room.Contains("#")).ToList();
                    BindGridDataSource(filtered);
                }
                else if (FilterKey == "Waiting List")
                {
                    LoadRegistrationDocument(CNETConstantes.OSD_WAITLIST_STATE, CurrentTime, null);

                    beiDeparture.EditValue = null;
                    beiArrival.EditValue = CurrentTime;
                    beiState.EditValue = CNETConstantes.OSD_WAITLIST_STATE;
                    if (regListVM == null)
                    {
                        ////CNETInfoReporter.Hide();
                        return;
                    }
                    BindGridDataSource(regListVM);
                }

                else if (FilterKey == "All Arrivals")
                {
                    LoadRegistrationDocument(null, null, null);
                    if (regListVM == null)
                    {
                        ////CNETInfoReporter.Hide();
                        return;
                    }

                    beiDeparture.EditValue = null;
                    beiArrival.EditValue = CurrentTime;
                    beiState.EditValue = "";
                    var filtered = regListVM.Where(r => (r.lastState == CNETConstantes.SIX_PM_STATE || r.lastState == CNETConstantes.GAURANTED_STATE) && r.Arrival.Date == CurrentTime.Value.Date).ToList();
                    BindGridDataSource(filtered);
                }
                else if (FilterKey == "Stay Overs")
                {
                    beiDeparture.EditValue = null;
                    beiArrival.EditValue = null;
                    beiState.EditValue = CNETConstantes.CHECKED_IN_STATE;
                    if (regListVM == null)
                    {
                        ////CNETInfoReporter.Hide();
                        return;
                    }
                    var filtered = regListVM.Where(r => r.lastState == CNETConstantes.CHECKED_IN_STATE && r.Arrival.Date < CurrentTime.Value.Date && r.Departure.Date > CurrentTime.Value.Date).ToList();
                    BindGridDataSource(filtered);
                }
                else if (FilterKey == "Departures")
                {
                    LoadRegistrationDocument(CNETConstantes.CHECKED_OUT_STATE, null, CurrentTime);
                    beiDeparture.EditValue = CurrentTime;
                    beiArrival.EditValue = null;
                    beiState.EditValue = CNETConstantes.CHECKED_OUT_STATE;
                    if (regListVM == null)
                    {
                        ////CNETInfoReporter.Hide();
                        return;
                    }
                    BindGridDataSource(regListVM);
                }
                else if (FilterKey == "Due Out")
                {
                    beiDeparture.EditValue = CurrentTime;
                    beiArrival.EditValue = null;
                    beiState.EditValue = CNETConstantes.CHECKED_IN_STATE;
                    RefreshRegDocBrowser();
                }

                else if (FilterKey == "Post Masters")
                {
                    if (regListVM == null)
                    {
                        ////CNETInfoReporter.Hide();
                        return;
                    }
                    List<int> pseudoRooms = null;
                    List<RoomTypeDTO> pseudoRoomList = UIProcessManager.GetRoomTypeByispseudoRoomType(true);
                    if (pseudoRoomList != null)
                        pseudoRooms = pseudoRoomList.Select(p => p.Id).ToList();
                    if (pseudoRooms != null)
                    {
                        var filtered = regListVM.Where(r => r.RoomType != null && r.lastState == CNETConstantes.CHECKED_IN_STATE && pseudoRooms.Contains(r.RoomType.Value)).ToList();
                        BindGridDataSource(filtered);
                    }
                }
                else if (FilterKey == "Arrived")
                {
                    LoadRegistrationDocument(CNETConstantes.CHECKED_IN_STATE, CurrentTime, null);
                    beiDeparture.EditValue = null;
                    beiArrival.EditValue = CurrentTime;
                    beiState.EditValue = CNETConstantes.CHECKED_IN_STATE;
                    if (regListVM == null)
                    {
                        ////CNETInfoReporter.Hide();
                        return;
                    }
                    BindGridDataSource(regListVM);
                }
                else if (FilterKey == "Cancellations")
                {
                    LoadRegistrationDocument(CNETConstantes.OSD_CANCEL_STATE, CurrentTime, null);
                    beiDeparture.EditValue = null;
                    beiArrival.EditValue = CurrentTime;
                    beiState.EditValue = CNETConstantes.OSD_CANCEL_STATE;
                    if (regListVM == null)
                    {
                        ////CNETInfoReporter.Hide();
                        return;
                    }
                    BindGridDataSource(regListVM);
                }
                else if (FilterKey == "Today's Pickup")
                {
                    LoadRegistrationDocument(null, null, null);
                    beiDeparture.EditValue = null;
                    beiArrival.EditValue = null;
                    beiState.EditValue = "";
                    if (regListVM == null)
                    {
                        ////CNETInfoReporter.Hide();
                        return;
                    }

                    var allTravelDetail = UIProcessManager.GetTravelDetailView(CNETConstantes.TD_PICK_UP, CurrentTime.Value, SelectedHotelcode);
                    if (allTravelDetail != null && allTravelDetail.Count > 0)
                    {
                        int[] regCodes = allTravelDetail.GroupBy(t => t.Id).Select(t => t.First()).ToList().Select(t => t.Id).ToArray();

                        var filtered = regListVM.Where(r => regCodes.Contains(r.Id)).ToList();
                        BindGridDataSource(filtered);
                    }
                    else
                    {
                        var filtered = new List<RegistrationListVMDTO>();
                        BindGridDataSource(filtered);
                    }

                }
                else if (FilterKey == "Today's Drop-off")
                {
                    LoadRegistrationDocument(null, null, null);
                    beiDeparture.EditValue = null;
                    beiArrival.EditValue = null;
                    beiState.EditValue = "";
                    if (regListVM == null) return;

                    var allTravelDetail = UIProcessManager.GetTravelDetailView(CNETConstantes.TD_DROP_OFF, CurrentTime.Value, SelectedHotelcode);
                    if (allTravelDetail != null && allTravelDetail.Count > 0)
                    {
                        int[] regCodes = allTravelDetail.GroupBy(t => t.Id).Select(t => t.First()).ToList().Select(t => t.Id).ToArray();

                        var filtered = regListVM.Where(r => regCodes.Contains(r.Id)).ToList();
                        BindGridDataSource(filtered);
                    }
                    else
                    {
                        var filtered = new List<RegistrationListVMDTO>();
                        BindGridDataSource(filtered);
                    }
                }
                else if (FilterKey == "Customer High Balance")
                {
                    LoadRegistrationDocument(null, null, null);
                    beiDeparture.EditValue = null;
                    beiArrival.EditValue = null;
                    beiState.EditValue = CNETConstantes.CHECKED_IN_STATE;
                    if (regListVM == null)
                    {
                        ////CNETInfoReporter.Hide();
                        return;

                    }

                    var config = LocalBuffer.LocalBuffer.ConfigurationBufferList.FirstOrDefault(c => c.Reference == CNETConstantes.PMS_Pointer.ToString() && c.Attribute == CNETConstantes.PMS_SETTING_CustHighBalance);

                    if (config == null)
                    {
                        ////CNETInfoReporter.Hide();
                        return;

                    }

                    decimal threshold = Convert.ToDecimal(config.CurrentValue);

                    List<int> chbList = UIProcessManager.GetCustomerHighBalance(CurrentTime.Value, SelectedHotelcode);
                    if (chbList == null || chbList.Count == 0)
                    {
                        var filtered = new List<RegistrationListVMDTO>();
                        BindGridDataSource(filtered);
                    }
                    else
                    {
                        var filtered = regListVM.Where(r => chbList.Contains(r.Id)).ToList();
                        BindGridDataSource(filtered);
                    }


                    ////CNETInfoReporter.Hide();
                    PopulateSearchCriteria("", FilterKey);
                }
                else if (FilterKey == "Room Moved Registrations")
                {
                    LoadRegistrationDocument(null, null, null);
                    beiDeparture.EditValue = null;
                    beiArrival.EditValue = null;
                    beiState.EditValue = CNETConstantes.CHECKED_IN_STATE;
                    if (regListVM == null)
                    {
                        ////CNETInfoReporter.Hide();
                        return;
                    }
                    ActivityDefinitionDTO workFlow = UIProcessManager.GetActivityDefinitionBydescriptionandreference(CNETConstantes.LU_ACTIVITY_DEFINATION_ROOM_MOVED, CNETConstantes.REGISTRATION_VOUCHER).FirstOrDefault();

                    if (workFlow != null)
                    {
                        //  List<spGetRegActivities_Result> actList = UIProcessManager.GetRegistrationActivity(CNETConstantes.LU_ACTIVITY_DEFINATION_ROOM_MOVED, CurrentTime.Value);
                        List<ActivityDTO> actList = UIProcessManager.GetActivityByactivityDefinitionandDate(workFlow.Id, CurrentTime.Value);

                        if (actList == null || actList.Count == 0)
                        {
                            var filtered = new List<RegistrationListVMDTO>();
                            BindGridDataSource(filtered);
                        }
                        else
                        {
                            int[] regList = actList.Select(r => r.Reference).ToArray();
                            var filtered = regListVM.Where(r => regList.Contains(r.Id)).ToList();
                            BindGridDataSource(filtered);
                        }
                    }
                    ////CNETInfoReporter.Hide();
                    PopulateSearchCriteria("", FilterKey);
                }
                else
                {
                    beiState.EditValue = CNETConstantes.CHECKED_IN_STATE;



                }



            }
            catch (Exception ex)
            {
                ////CNETInfoReporter.Hide();
            }
        }

        #endregion

        #region Helper Methods

        private void InitializeUI()
        {
            //Icons
            ApplyIcons();

            //state list
            GridColumn col = riluState.View.Columns.AddField("Description");
            col.Visible = true;
            col.Caption = "State";
            col = riluState.View.Columns.AddField("Value");
            col.Caption = "Color";
            col.Visible = true;
            riluState.DisplayMember = "Description";
            riluState.ValueMember = "Id";

            //Guest List
            GridColumn column = risleGuest.View.Columns.AddField("Id");
            column.Visible = false;
            column = risleGuest.View.Columns.AddField("Code");
            column.Visible = true;
            column = risleGuest.View.Columns.AddField("FirstName");
            column.Caption = "First Name";
            column.Visible = true;
            column = risleGuest.View.Columns.AddField("SecondName");
            column.Caption = "Middle Name";
            column.Visible = true;
            column = risleGuest.View.Columns.AddField("BioId");
            column.Visible = true;
            risleGuest.DisplayMember = "FirstName";
            risleGuest.ValueMember = "Id";

            // Company List
            GridColumn columnContact = risleCompany.View.Columns.AddField("Code");
            columnContact.Visible = true;
            columnContact = risleCompany.View.Columns.AddField("Id");
            columnContact.Visible = false;
            columnContact = risleCompany.View.Columns.AddField("FirstName");
            columnContact.Caption = "Trade Name";
            columnContact.Visible = true;
            columnContact = risleCompany.View.Columns.AddField("Tin");
            columnContact.Visible = true;
            risleCompany.DisplayMember = "FirstName";
            risleCompany.ValueMember = "Id";


            riluState.BestFitMode = BestFitMode.BestFitResizePopup;
            riluState.View.OptionsView.ShowIndicator = false;

            // setup Event Handlers
            ritRoom.KeyDown += ritRoom_KeyDown;
            ritRoom.EditValueChanged += ritRoom_EditValueChanged;
            riteCompany.KeyDown += riteCompany_KeyDown;
            risleGuest.EditValueChanged += risleGuestSearch_EditValueChanged;
            risleCompany.EditValueChanged += risleCompany_EditValueChanged;
            riluState.View.CustomDrawCell += View_CustomDrawCell;
            ceSeen.CheckedChanged += ceSeen_CheckedChanged;

            //grid_regDoc.EmbeddedNavigator.Appearance.BackColor = Color.LightYellow;
            //grid_regDoc.EmbeddedNavigator.Appearance.ForeColor = Color.Red;
            //grid_regDoc.EmbeddedNavigator.Buttons.Append.Visible = false;
            //grid_regDoc.EmbeddedNavigator.Buttons.CancelEdit.Visible = false;
            //grid_regDoc.EmbeddedNavigator.Buttons.EndEdit.Visible = false;
            //grid_regDoc.EmbeddedNavigator.Buttons.NextPage.Visible = false;
            //grid_regDoc.EmbeddedNavigator.Buttons.PrevPage.Visible = false;
            //grid_regDoc.EmbeddedNavigator.Buttons.Remove.Visible = false;
            //grid_regDoc.EmbeddedNavigator.Buttons.Edit.Visible = false;

            repoZoomTrack.EditValueChanged += RepoZoomTrack_EditValueChanged;
        }


        private void InitializeData()
        {
            try
            {
                // Progress_Reporter.Show_Progress("Initializing Data");
                // Device and CurrentUser
                device = LocalBuffer.LocalBuffer.CurrentDevice;
                currentUser = LocalBuffer.LocalBuffer.CurrentLoggedInUser;


                //get Door lock device
                DoorLockFactory dLockFactory = new DoorLockFactory();
                _doorLock = dLockFactory.GetDoorLock(false);

                CurrentTime = UIProcessManager.GetServiceTime();
                if (CurrentTime == null)
                {
                    SystemMessage.ShowModalInfoMessage("Unable to get server time!", "ERROR");
                }
                else
                {
                    beiStartDate.EditValue = CurrentTime.Value;
                    beiEndDate.EditValue = CurrentTime.Value;
                }



                List<VwRegistrationDocumentViewDTO> SIX_PM_STATERegstration = UIProcessManager.GetRegistrationDocumentViewByStartdateEnddateStateandConsigneeUnit(CurrentTime.Value, CurrentTime.Value, CNETConstantes.SIX_PM_STATE, SelectedHotelcode);

                 

                if (SIX_PM_STATERegstration != null && SIX_PM_STATERegstration.Count > 0)
                {
                    bbiFooter6PMCheckin.Caption = string.Format("6PM Checkin [ {0} ]", SIX_PM_STATERegstration.Count);

                }
                else
                {
                    bbiFooter6PMCheckin.Caption = string.Format("6PM Checkin [ {0} ]", 0);
                }


                // AllTransactionReference = UIProcessManager.SelectAllTransactionReference();

                // Object States
                List<SystemConstantDTO> stateList = LocalBuffer.LocalBuffer.ObjectStateDefinitionBufferList.Where(o => o.Category == "Front Office").ToList();
                SystemConstantDTO all = new SystemConstantDTO();
                all.Id = 0;
                all.Description = "All";
                stateList.Add(all);
                riluState.DataSource = stateList;


                //populate person and org
                PopulateGuestLookup();
                PopulateCompanyLookup(CNETConstantes.CUSTOMER);


                reiHotel.DisplayMember = "Name";
                reiHotel.ValueMember = "Id";
                reiHotel.DataSource = (LocalBuffer.LocalBuffer.HotelBranchBufferList.Select(x => new { x.Id, x.Name })).ToList();

                if (LocalBuffer.LocalBuffer.HotelBranchBufferList != null && LocalBuffer.LocalBuffer.HotelBranchBufferList.Count > 0)
                {
                    beiHotel.EditValue = LocalBuffer.LocalBuffer.CurrentDeviceConsigneeUnit;
                    reiHotel.ReadOnly = !LocalBuffer.LocalBuffer.UserHasHotelBranchAccess;
                }

                //Initial Data Load
                beiState.EditValue = CNETConstantes.CHECKED_IN_STATE;
                beiArrival.EditValue = null;
                beiDeparture.EditValue = null;


                //this is commented b/c when beiState is set by check in it does the same thing 
                // LoadRegistrationDocument(CNETConstantes.CHECKED_IN_STATE, null, null);
                BindGridDataSource(regListVM);
                if (Dispatcher.CheckClosing())
                {
                    SecurityCheck(CNETConstantes.CHECKED_IN_STATE);
                }
                else
                {
                    DisableButtons();
                }
                Initalizing = false;
                ////CNETInfoReporter.Hide();
            }
            catch (Exception ex)
            {
                ////CNETInfoReporter.Hide();
                SystemMessage.ShowModalInfoMessage("Exception has occurred in initializing data. DETAIL: " + ex.Message, "ERROR");
            }
        }
        public bool Initalizing = true;
        // Get All Guests
        private void PopulateGuestLookup()
        {
            try
            {
                risleGuest.DataSource = null;
                risleGuest.DataSource = LocalBuffer.LocalBuffer.AllGuestConsigneeViewlist;
                risleGuest.View.RefreshData();
            }
            catch (Exception ex)
            {
                //ignore
            }

        }

        // Get All Companies
        private void PopulateCompanyLookup(int type)
        {
            try
            {
                risleCompany.DataSource = null;
                List<ConsigneeDTO> allConsigneeViewConDtos = new List<ConsigneeDTO>();
                allConsigneeViewConDtos = LocalBuffer.LocalBuffer.ConsigneeViewBufferList.Where(x => x.IsActive && x.GslType == type).ToList();
                //if (type == CNETConstantes.CONTACT)
                //{
                //    var contactList = LocalBuffer.LocalBuffer.ConsigneeViewBufferList.Where(p => p.ConsigneeIsActive  && p.GslType == CNETConstantes.CONTACT).ToList();
                //    if (contactList == null) return;

                //    foreach (var con in contactList)
                //    {
                //        vw_VoucherConsignee dto = new vw_VoucherConsignee();
                //        dto.code = con.code;
                //        dto.name = con.firstName + " " + con.middleName + " " + con.lastName;

                //        //Identification
                //        var ident = LocalBuffer.LocalBuffer.IdentificationBufferList.FirstOrDefault(i => i.reference == con.code);
                //        dto.idNumber = ident == null ? "" : ident.idNumber;

                //        dto.idNumber = ident == null ? "" : ident.idNumber;
                //        allConsigneeViewConDtos.Add(dto);
                //    }
                //}
                //else
                //{

                //    var orgCompanyList = LocalBuffer.LocalBuffer.OrganizationBufferList.Where(o => o.type == type && o.isActive).ToList();
                //    if (orgCompanyList == null) return;

                //    foreach (var con in orgCompanyList)
                //    {
                //        vw_VoucherConsignee dto = new vw_VoucherConsignee();
                //        dto.code = con.code;
                //        dto.name = con.tradeName;

                //        //Identification
                //        var ident = LocalBuffer.LocalBuffer.IdentificationBufferList.FirstOrDefault(i => i.reference == con.code);
                //        dto.idNumber = ident == null ? "" : ident.idNumber;
                //        allConsigneeViewConDtos.Add(dto);
                //    }
                //}
                risleCompany.DataSource = allConsigneeViewConDtos;
                risleCompany.View.RefreshData();
            }
            catch (Exception ex)
            {
                //ignore
            }
        }

        private void ApplyIcons()
        {
            Image Image = Provider.GetImage("Search", ProviderType.APPLICATIONICON, PictureSize.Dimension_32X32);

            bsiAdvancedSearch.Glyph = Image;
            bsiAdvancedSearch.LargeGlyph = Image;

        }

        private List<RegistrationListVMDTO> FilterOtherConsignees(List<RegistrationListVMDTO> filteredList)
        {
            List<VoucherConsigneeListDTO> agentList =
                UIProcessManager.GetVoucherConsigneeListByconsignee(Convert.ToInt32(beiCompanySearch.EditValue));
            List<int> voList = agentList.Select(r => r.Voucher).Distinct().ToList();
            List<RegistrationListVMDTO> companyfilter = (from vo in voList where filteredList.Select(r => r.Id).Contains(vo) select filteredList.FirstOrDefault(r => r.Id == vo)).Distinct().ToList();
            return companyfilter;
        }

        private void LoadRegistrationDocument(int? state, DateTime? arrivalDate, DateTime? departureDate)
        {
            Progress_Reporter.Show_Progress("Loading Registrations...", "Please Wait.......");


            //if (_regDocList != null)
            //    _regDocList.Clear();
            //_regDocList = MasterPageForm.RegistrationDataChange(UIProcessManager.GetRegistrationDocList(state, arrivalDate, departureDate,SelectedHotelcode));

            //if (_regDocList == null) return;

            //if(SelectedHotelcode != null)
            //{
            //    _regDocList = _regDocList.Where(x=> x.OrganizationUnitDefintion == SelectedHotelcode).ToList();
            //}

            //PopulateRegListVM(_regDocList);

            if (regListVM != null && regListVM.Count > 0)
                regListVM.Clear();
            regListVM = UIProcessManager.GetRegistrationViewModelData(arrivalDate, departureDate, state, SelectedHotelcode);


            if (regListVM == null)
                regListVM = new List<RegistrationListVMDTO>();


            var daaa = regListVM.Where(x => x.Id == 120995).ToList();

            int totalLog = 0;
            int totalNoPost = 0;
            int totalTravelDetail = 0;
            int totalAccompanyGuest = 0;
            int totalShare = 0;
            int totalWakeupCall = 0;
            int totalMasters = 0;
            int totalDueouts = 0;
            int totalDailyCharged = 0;

            totalNoPost = regListVM.Where(x => x.NoPost == true).ToList().Count;
            totalTravelDetail += regListVM.Sum(x => x.TravelCount);
            totalAccompanyGuest += regListVM.Sum(x => x.AccompanyCount);
            totalShare += regListVM.Sum(x => x.ShareCount);


            var logmessageregistration = regListVM.Where(x => x.LogMessages > 0).ToList();

            if (logmessageregistration != null && logmessageregistration.Count > 0)
                totalLog += logmessageregistration.Count();
            else
                totalLog = 0;

            var departureregistration = regListVM.Where(x => x.Departure.Date == CurrentTime.Value.Date).ToList();
            if (departureregistration != null && departureregistration.Count > 0)
                totalDueouts = departureregistration.Count();
            else
                totalDueouts = 0;


            totalWakeupCall += regListVM.Sum(x => x.WakeupCallCount);
            totalMasters = regListVM.Where(x => x.IsMaster != null && x.IsMaster.Value) == null ? 0 : regListVM.Where(x => x.IsMaster != null && x.IsMaster.Value).ToList().Count;
            totalDailyCharged = regListVM.Where(x => x.IsCharged).Count();
            //Update Footer Status
            bbiFooterNoPost.Caption = string.Format("No Post [ {0} ]", totalNoPost);
            bbiFooterTravelDetail.Caption = string.Format("Travel Detail [ {0} ]", totalTravelDetail);
            bbiFooterAccompanyGuest.Caption = string.Format("Accompany Guest [ {0} ]", totalAccompanyGuest);
            bbiFooterShare.Caption = string.Format("Room Share [ {0} ]", totalShare);
            bbiFooterLogBook.Caption = string.Format("Log Message [ {0} ]", totalLog);
            bbiFooterWakeupCall.Caption = string.Format("Wake-Up Call [ {0} ]", totalWakeupCall);
            bbiFooterMasterGuest.Caption = string.Format("Master Guest [ {0} ]", totalMasters);
            bbiFooterDueOuts.Caption = string.Format("Dueouts [ {0} ]", totalDueouts);
            bbiFooterChargedRooms.Caption = string.Format("Daily Charged [ {0} ]", totalDailyCharged);
            Progress_Reporter.Close_Progress();
        }

        /*
                private void PopulateRegListVM(List<RegistrationDocumentDTO> registrationDocumentBrowser)
                {
                    regListVM.Clear();

                    string loggedInUser = LocalBuffer.LocalBuffer.CurrentLoggedInUser.code;
                    if (adSeen == null)
                    {
                        adSeen = LocalBuffer.LocalBuffer.ActivityDefnBufferList.FirstOrDefault(a => a.component == CNETConstantes.VOUCHER_COMPONENET && a.reference == CNETConstantes.MESSAGE.ToString() && a.description == CNETConstantes.LU_ACTIVITY_SEEN);
                    }

                    if (adSeen != null)
                    {
                        allActivity = UIProcessManager.GetActivitiesByUserAndActivityDefn(loggedInUser, adSeen.code);

                    }

                    //Room Charege List
                    List<string> chargedRegList = null;
                    if (LocalBuffer.LocalBuffer.RoomChargeBufferList != null && LocalBuffer.LocalBuffer.RoomChargeBufferList.Count > 0)
                    {
                        chargedRegList = LocalBuffer.LocalBuffer.RoomChargeBufferList.Select(r => r.sourceCharge).ToList();
                    }
                    if (chargedRegList == null) chargedRegList = new List<string>();


                    //For Footer Status
                    int totalLog = 0;
                    int totalNoPost = 0;
                    int totalTravelDetail = 0;
                    int totalAccompanyGuest = 0;
                    int totalShare = 0;
                    int totalWakeupCall = 0;
                    int totalMasters = 0;
                    int totalDueouts = 0;
                    int totalDailyCharged = 0;
                    int LogCount = 0;
                    int TravelCount = 0;
                    bool NoPos = false,
                        AuthorizeDirectBill = false,
                         preStayCharging = false,
                         postStayCharging = false,
                          AllowLatecheckout = false,
                          authorizeKeyReturn = false;

                    try
                    {

                        regListVM = (from regDocBro in registrationDocumentBrowser
                                     join message in MasterPageForm.LogBufferList.GroupBy(x => x.regNumber)
                                             on regDocBro.code equals message.Key
                                             into a
                                     from b in a.DefaultIfEmpty()
                                     join TRAVEL in LocalBuffer.LocalBuffer.TravelDetailBufferList.GroupBy(x => x.code)
                                         on regDocBro.code equals TRAVEL.Key
                                         into c
                                     from d in c.DefaultIfEmpty()
                                     join Service in MasterPageForm.ServiceRequestBufferList.GroupBy(x => x.RegNumber)
                                         on regDocBro.code equals Service.Key
                                         into e
                                     from f in e.DefaultIfEmpty()
                                     join Accompany in LocalBuffer.LocalBuffer.AccompanyGuestBufferList.GroupBy(x => x.voucher)
                                         on regDocBro.code equals Accompany.Key
                                         into g
                                     from h in g.DefaultIfEmpty()
                                     join WakeupCall in LocalBuffer.LocalBuffer.WakeupCallBufferList.GroupBy(x => x.reference)
                                         on regDocBro.code equals WakeupCall.Key
                                         into i
                                     from j in i.DefaultIfEmpty()
                                     join Sharereference in LocalBuffer.LocalBuffer.ShareBufferList.GroupBy(x => x.referenceObject)
                                         on regDocBro.code equals Sharereference.Key
                                         into k
                                     from l in k.DefaultIfEmpty()
                                     join Sharereferring in LocalBuffer.LocalBuffer.ShareBufferList.GroupBy(x => x.referringObject)
                                         on regDocBro.code equals Sharereferring.Key
                                         into m
                                     from n in m.DefaultIfEmpty()
                                     join RegistrationPre in LocalBuffer.LocalBuffer.RegistrationPrevilageBufferList.GroupBy(x => x.voucher)
                                         on regDocBro.code equals RegistrationPre.Key
                                         into o
                                     from p in o.DefaultIfEmpty()
                                     select new RegistrationListVMDTO
                                     {
                                         OrganizationUnitDefintion = regDocBro.OrganizationUnitDefintion,
                                         adult = regDocBro.adult == null ? 1 : regDocBro.adult,
                                         child = regDocBro.child == null ? 0 : regDocBro.child,
                                         Registration = string.IsNullOrEmpty(regDocBro.code) ? "" : regDocBro.code,
                                         Consignee = regDocBro.consignee == null ? "" : regDocBro.consignee,
                                         RoomType = regDocBro.roomType == null ? "" : regDocBro.roomType,
                                         RTC = regDocBro.actualRTC == null ? "" : regDocBro.actualRTC,
                                         RoomCode = regDocBro.room == null ? "" : regDocBro.room,
                                         RoomTypeDescription = regDocBro.RoomTypeDescription == null ? "" : regDocBro.RoomTypeDescription,
                                         Market = regDocBro.Market == null ? "" : regDocBro.Market,
                                         Color = regDocBro.color == null ? "" : regDocBro.color,
                                         RateCodeHeader = regDocBro.rateCodeHeader == null ? "" : regDocBro.rateCodeHeader,
                                         RegistrationDate = regDocBro.arrivalDate == null ? CurrentTime.Value : regDocBro.arrivalDate,
                                         NoOfRoom = regDocBro.roomCount == null ? 1 : regDocBro.roomCount,
                                         Room = regDocBro.roomCount > 1 ? ("#" + regDocBro.roomCount) : regDocBro.RoomNumber,
                                         Customer = regDocBro.name == null ? "" : regDocBro.name,
                                         Companycode = regDocBro.OtherConsignee == null ? "" : regDocBro.OtherConsignee,
                                         Company = regDocBro.tradeName == null ? regDocBro.name : (regDocBro.requiredGSL == CNETConstantes.REQ_GSL_COMPANY ? regDocBro.tradeName : ""),
                                         Arrival = regDocBro.arrivalDate == null ? CurrentTime.Value : Convert.ToDateTime(regDocBro.arrivalDate),
                                         Departure = regDocBro.departureDate == null ? CurrentTime.Value : Convert.ToDateTime(regDocBro.departureDate),
                                         NumOfNight = (int)(regDocBro.departureDate.Date - regDocBro.arrivalDate.Date).TotalDays,
                                         Payment = regDocBro.PaymentMethod == null ? "" : regDocBro.PaymentMethod,
                                         lastState = regDocBro.foStatus == null ? "" : regDocBro.foStatus,
                                         LogMessages = b == null ? null : GetMessage(b.ToList(), out LogCount),
                                         LogCount = b == null ? 0 : LogCount,
                                         TravelDetails = d == null ? null : d.ToList(),
                                         TravelCount = d == null ? 0 : d.ToList().Count,
                                         ServiceRequests = f == null ? null : f.ToList(),
                                         AccompanyCount = h == null ? 0 : h.ToList().Count,
                                         WakeupCallCount = j == null ? 0 : j.ToList().Count,
                                         ShareCount =  (l == null ? 0 : l.ToList().Count) + (n == null ? 0 : n.ToList().Count),
                                         NoPost = p == null ? false :  GetRegistrationPrevilage(regDocBro.code, p.ToList(), ref NoPos, ref AuthorizeDirectBill,
                                         ref preStayCharging, ref postStayCharging, ref AllowLatecheckout, ref authorizeKeyReturn),
                                         AuthorizeDirectBill = AuthorizeDirectBill,
                                         authorizeKeyReturn = authorizeKeyReturn,
                                         preStayCharging = postStayCharging,
                                         postStayCharging = preStayCharging,
                                         AllowLatecheckout = AllowLatecheckout,
                                         IsDueout = getIsDueout(regDocBro.departureDate, ref totalDueouts),
                                         IsCharged = getIsCharged(regDocBro.code, chargedRegList, ref totalDailyCharged),
                                         IsMaster = regDocBro.ismaster == null ? false : regDocBro.ismaster 
                                         //IsRegistrationMaster(regDocBro.code, registrationDocumentBrowser)
                                         //IsMaster = string.IsNullOrEmpty(regDocBro.regHeaderRemark) ? false : true,
                                     }).ToList();
                    }
                    catch (Exception io)
                    {
                    }
                    totalNoPost = regListVM.Where(x => x.NoPost == true).ToList().Count;
                    totalTravelDetail += regListVM.Sum(x => x.TravelCount);
                    totalAccompanyGuest += regListVM.Sum(x => x.AccompanyCount);
                    totalShare += regListVM.Sum(x => x.ShareCount);
                    totalLog += regListVM.Sum(x => x.LogCount);
                    totalWakeupCall += regListVM.Sum(x => x.WakeupCallCount);
                    totalMasters = regListVM.Where(x => x.IsMaster) == null ? 0 : regListVM.Where(x => x.IsMaster).ToList().Count;
                    //Update Footer Status
                    bbiFooterNoPost.Caption = string.Format("No Post [ {0} ]", totalNoPost);
                    bbiFooterTravelDetail.Caption = string.Format("Travel Detail [ {0} ]", totalTravelDetail);
                    bbiFooterAccompanyGuest.Caption = string.Format("Accompany Guest [ {0} ]", totalAccompanyGuest);
                    bbiFooterShare.Caption = string.Format("Room Share [ {0} ]", totalShare);
                    bbiFooterLogBook.Caption = string.Format("Log Message [ {0} ]", totalLog);
                    bbiFooterWakeupCall.Caption = string.Format("Wake-Up Call [ {0} ]", totalWakeupCall);
                    bbiFooterMasterGuest.Caption = string.Format("Master Guest [ {0} ]", totalMasters);
                    bbiFooterDueOuts.Caption = string.Format("Dueouts [ {0} ]", totalDueouts);
                    bbiFooterChargedRooms.Caption = string.Format("Daily Charged [ {0} ]", totalDailyCharged);
                }
        */
        private int GetShareCount(IGrouping<string, RelationDTO> l, IGrouping<string, RelationDTO> n)
        {
            int share = (l == null ? 0 : l.ToList().Count) + (n == null ? 0 : n.ToList().Count);

            return share;
        }

        private bool IsRegistrationMaster(int registrationcode, List<RegistrationDocumentDTO> registrationDocumentBrowser)
        {
            // return false;
            // Progress_Reporter.Show_Progress("Checking if Registration is Master");
            bool ismaster = false;
            // List<TransactionReference> refere =  AllTransactionReference.Where(x=> x.referenced == registrationcode).ToList();
            List<TransactionReferenceDTO> refere = UIProcessManager.GetTransactionReferenceByreferenced(registrationcode);
            if (refere != null && refere.Count > 0 && registrationDocumentBrowser != null && registrationDocumentBrowser.Count > 0)
            {
                List<int> referingcode = refere.Select(x => x.Referring.Value).ToList();

                List<int> Registrationcode = registrationDocumentBrowser.Select(x => x.Id).ToList();

                List<int> MasterList = Registrationcode.Intersect(referingcode).ToList();

                if (MasterList != null && MasterList.Count > 0)
                {
                    ////CNETInfoReporter.Hide();
                    ismaster = true;
                    return true;
                }
            }
            ////CNETInfoReporter.Hide();
            return ismaster;
        }


        //public bool GetRegistrationPrevilage(string Registration, List<RegistrationPrivllegeDTO> RegistrationPrevilageList, ref bool NoPost, ref bool AuthorizeDirectBill, ref bool preStayCharging,
        //    ref bool postStayCharging, ref bool AllowLatecheckout, ref bool authorizeKeyReturn)
        //{

        //    if (RegistrationPrevilageList != null && RegistrationPrevilageList.Count > 0)
        //    {
        //        RegistrationPrivllegeDTO RegistrationPrevilage = RegistrationPrevilageList.FirstOrDefault();
        //        if (RegistrationPrevilage != null)
        //        {
        //            NoPost = RegistrationPrevilage.noPost == null ? false : RegistrationPrevilage.noPost.Value;
        //            AuthorizeDirectBill = RegistrationPrevilage.authorizeDirectBill == null ? false : RegistrationPrevilage.authorizeDirectBill.Value;
        //            preStayCharging = RegistrationPrevilage.preStayCharging == null ? false : RegistrationPrevilage.preStayCharging.Value;
        //            postStayCharging = RegistrationPrevilage.postStayCharging == null ? false : RegistrationPrevilage.postStayCharging.Value;
        //            AllowLatecheckout = RegistrationPrevilage.AllowLatecheckout == null ? false : RegistrationPrevilage.AllowLatecheckout.Value;
        //            authorizeKeyReturn = RegistrationPrevilage.authorizeKeyReturn == null ? false : RegistrationPrevilage.authorizeKeyReturn.Value;
        //        }
        //    }

        //    return NoPost;
        //}

        public bool getIsDueout(DateTime Departure, ref int totalDueouts)
        {
            bool IsDueout = false;
            int tot = 0;
            //Check Due outs and update the count
            if (Departure.Date == CurrentTime.Value.Date)
            {
                IsDueout = true;
                tot++;
            }
            totalDueouts += tot;
            return IsDueout;
        }

        public bool getIsCharged(string Registration, List<string> chargedRegList, ref int totalDailyCharged)
        {
            bool IsCharged = false;
            int tot = 0;
            //check if the registration is chareged or no
            if (chargedRegList.Contains(Registration))
            {
                IsCharged = true;
                tot++;
            }

            totalDailyCharged += tot;
            return IsCharged;
        }



        //private void PopulateByRegDocView(List<RegistrationDocumentDTO> registrationDocumentBrowser)
        //{
        //    regListVM.Clear();

        //    foreach (RegistrationDocumentDTO regDocBro in registrationDocumentBrowser)
        //    {

        //        RoomType RoomTy = LocalBuffer.LocalBuffer.RoomTypeBufferList.FirstOrDefault(x => x.code == regDocBro.roomType);
        //        ObjectStateDefinition OBJ = LocalBuffer.LocalBuffer.ObjectStateDefinitionBufferList.FirstOrDefault(x => x.code == regDocBro.foStatus);
        //        //string customer = "";
        //        var rd = new RegistrationListVMDTO();
        //        rd.Registration = regDocBro.code;
        //        rd.Consignee = regDocBro.consignee;
        //        rd.RoomType = regDocBro.roomType;
        //        rd.RoomTypeDescription = RoomTy.description;
        //        rd.Market = regDocBro.Market;
        //        rd.Color = OBJ.color;


        //        // rd.RegExtCode = regDocBro.RegistrationExtension;
        //        if (regDocBro.arrivalDate != null)
        //        {
        //            rd.RegistrationDate = regDocBro.arrivalDate;
        //            if (regDocBro.roomCount != null)
        //            {
        //                rd.NoOfRoom = regDocBro.roomCount;
        //            }
        //            //if (!string.IsNullOrEmpty(regDocBro.resType))
        //            //{
        //            //    rd.ResType = UIProcessManager.SelectLookup(regDocBro.resType).description;
        //            //}
        //            if (regDocBro.roomCount > 1)
        //            {
        //                rd.Room = "#" + regDocBro.roomCount;
        //            }
        //            else
        //            {
        //                RoomDetailDTO rooms = LocalBuffer.LocalBuffer.RoomDetailBufferList.FirstOrDefault(x => x.code == regDocBro.room);

        //                rd.Room = rooms.description;
        //            }

        //            string CustomerName = "";
        //            if (!string.IsNullOrEmpty(regDocBro.consignee))
        //            {
        //                CustomerName = LocalBuffer.LocalBuffer.ConsigneeViewBufferList.FirstOrDefault(x => x.code == regDocBro.GuestId).name;
        //            }
        //            rd.Guest = CustomerName;
        //            string CompanyName = "";

        //            if (!string.IsNullOrEmpty(regDocBro.OtherConsignee))
        //            {
        //                CompanyName = LocalBuffer.LocalBuffer.AllVoucherConsigneeBufferList.FirstOrDefault(x => x.code == regDocBro.OtherConsignee).name;
        //                rd.Company = regDocBro.requiredGSL == CNETConstantes.REQ_GSL_COMPANY ? CompanyName : "";
        //            }
        //            else
        //            {
        //                rd.Customer = CustomerName;
        //            }
        //            if (regDocBro.arrivalDate != null)
        //            {
        //                rd.Arrival = Convert.ToDateTime(regDocBro.arrivalDate);
        //            }
        //        }
        //        if (regDocBro.departureDate != null)
        //        {
        //            rd.Departure = Convert.ToDateTime(regDocBro.departureDate);
        //        }

        //        Lookup Payment = LocalBuffer.LocalBuffer.SystemConstantDTOBufferList.FirstOrDefault(x => x.code == regDocBro.paymentType);

        //        rd.Payment = Payment.description;
        //        rd.lastState = regDocBro.foStatus;
        //        if (!regListVM.Contains(rd))
        //            regListVM.Add(rd);
        //    }
        //}

        private void BindGridDataSource(List<RegistrationListVMDTO> registrationList)
        {
            //grid_regDoc.BeginUpdate();
            grid_regDoc.DataSource = null;
            isFromAdvanced = false;

            if (registrationList == null) return;

            grid_regDoc.DataSource = registrationList;
            grid_regDoc.Refresh();
            gridView_regDoc.RefreshData();
            //gridView_regDoc.TopRowIndex = 0;


            if (registrationList.Count == 0)
            {
                SwitchControlsByState(null);
            }
            else
            {
                int state = Convert.ToInt32(beiState.EditValue);
                // RegistrationListVMDTO firstRow = registrationList.ElementAt(0);
                SwitchControlsByState(state);
                SecurityCheck(state);
            }


            //grid_regDoc.EndUpdate();
            grid_regDoc.Refresh();
            grid_regDoc.RefreshDataSource();




        }

        //Helper method to show checkout form based on the value of the passed parameters
        private bool PerfomCheckout(RegistrationListVMDTO regExten, bool isWithoutRecept, bool isWithZeroBalance = false, bool isReinstate = false)
        {
            bool flag = false;
            try
            {
                var response = UIProcessManager.GetVoucherBufferById(regExten.Id);

                if (response == null || !response.Success) return false;

                VoucherBuffer voucherBuffer = response.Data;
                if (voucherBuffer == null) return false;
                var osd = LocalBuffer.LocalBuffer.ObjectStateDefinitionBufferList.FirstOrDefault(o => o.Id == voucherBuffer.Voucher.LastState);
                string state = osd == null ? "" : osd.Description;
                if (voucherBuffer.Voucher.LastState != CNETConstantes.CHECKED_IN_STATE && voucherBuffer.Voucher.LastState != CNETConstantes.ONLINE_CHECKED_OUT_STATE && voucherBuffer.Voucher.LastState != CNETConstantes.OSD_PENDING_STATE && voucherBuffer.Voucher.LastState != CNETConstantes.NO_SHOW_STATE)
                {
                    SystemMessage.ShowModalInfoMessage(state + " state can not be changed to Check Out state!", "ERROR");
                    return false;
                }

                frmCheckOut frmCheckOut = new frmCheckOut();
                frmCheckOut.RegExtension = regExten;
                frmCheckOut.VoucherBuffer = voucherBuffer;
                frmCheckOut.IsReinstate = isReinstate;
                frmCheckOut.checkOutWithOutReceipt = isWithoutRecept;
                frmCheckOut.IsWithZeroBalance = isWithZeroBalance;
                if (frmCheckOut.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    flag = true;

                }

                return flag;
            }
            catch (Exception ex)
            {
                SystemMessage.ShowModalInfoMessage("Error in processing checkout. DETAIL::" + ex.Message, "ERROR");

                return false;
            }
        }

        // Disable and Enable Menu Constrols based on the selected state
        private void SwitchControlsByState(int? state)
        {
            if (state != null)
            {

                #region Check-Out

                if (state == CNETConstantes.CHECKED_OUT_STATE)
                {
                    // Options: Enable only Add-on menu
                    bsiOptions.Enabled = true;

                    bbiAccompyingGuest.Enabled = false;
                    bbiAttachment.Enabled = true;
                    bbiHistory.Enabled = true;
                    bbiHouseKeeping.Enabled = false;
                    bbiMessages.Enabled = true;
                    bbiRouting.Enabled = false;
                    bbiShares.Enabled = false;
                    bbiWakeupCalls.Enabled = false;
                    bbiPrevilage.Enabled = false;
                    bbiScheduledActivity.Enabled = false;
                    bbiTravelDetail.Enabled = false;
                    bbiIssueCard.Enabled = false;
                    bbiRoomAssignment.Enabled = false;
                    bbiReplicate.Enabled = true;
                    bbiSetAlarm.Enabled = false;


                    //Activities
                    bsiActivites.Enabled = true;

                    bbiWaitlist.Enabled = false;
                    bbiSixPM.Enabled = false;
                    bbiConformation.Enabled = false;
                    bbiCancel.Enabled = false;
                    bbiCheckIN.Enabled = false;
                    bbiQuickCheckIn.Enabled = false;
                    bbiCheckOut.Enabled = false;
                    bbiGroupCheckout.Enabled = true;
                    bbiPostMaster.Enabled = false;
                    bbiReinstate.Enabled = true;

                    //Amendment
                    bsiAmendment.Enabled = false;

                    //Billing
                    bsiBilling.Enabled = true;
                    bbiManualCharge.Enabled = false;
                    bbiBillTransfer.Enabled = false;
                    bbiBillSplit.Enabled = false;
                    bbiRoomCharges.Enabled = true;
                    //bbiRoomCharges.Enabled = false;
                    bbiCashReceipt.Enabled = true;
                    bbiRebate.Enabled = false;
                    bbiPaidOut.Enabled = false;
                    bbiRefund.Enabled = true;


                    //Preview
                    bsiPreview.Enabled = true;
                    bbiFolio.Enabled = true;
                    bbiProformaFolio.Enabled = true;
                    bbiRateSummary.Enabled = true;
                    bbiRegCard.Enabled = true;

                    //PREVIEW
                    bsiPreview.Enabled = true;
                }

                #endregion

                #region Online Check-Out
                else if (state == CNETConstantes.ONLINE_CHECKED_OUT_STATE)
                {
                    // Options: Enable only Add-on menu
                    bsiOptions.Enabled = false;

                    bbiAccompyingGuest.Enabled = false;
                    bbiAttachment.Enabled = false;
                    bbiHistory.Enabled = false;
                    bbiHouseKeeping.Enabled = false;
                    bbiMessages.Enabled = false;
                    bbiRouting.Enabled = false;
                    bbiShares.Enabled = false;
                    bbiWakeupCalls.Enabled = false;
                    bbiPrevilage.Enabled = false;
                    bbiScheduledActivity.Enabled = false;
                    bbiTravelDetail.Enabled = false;
                    bbiIssueCard.Enabled = false;
                    bbiRoomAssignment.Enabled = false;
                    bbiReplicate.Enabled = false;
                    bbiSetAlarm.Enabled = false;


                    //Activities
                    bsiActivites.Enabled = true;

                    bbiWaitlist.Enabled = false;
                    bbiSixPM.Enabled = false;
                    bbiConformation.Enabled = false;
                    bbiCancel.Enabled = false;
                    bbiCheckIN.Enabled = false;
                    bbiQuickCheckIn.Enabled = false;
                    bbiCheckOut.Enabled = true;
                    bbiGroupCheckout.Enabled = false;
                    bbiPostMaster.Enabled = false;
                    bbiReinstate.Enabled = true;

                    //Amendment
                    bsiAmendment.Enabled = false;

                    //Billing
                    bsiBilling.Enabled = false;
                    bbiManualCharge.Enabled = false;
                    bbiBillTransfer.Enabled = false;
                    bbiBillSplit.Enabled = false;
                    bbiRoomCharges.Enabled = false;
                    //bbiRoomCharges.Enabled = false;
                    bbiCashReceipt.Enabled = false;
                    bbiRebate.Enabled = false;
                    bbiPaidOut.Enabled = false;
                    bbiRefund.Enabled = false;


                    //Preview
                    bsiPreview.Enabled = true;
                    bbiFolio.Enabled = true;
                    bbiProformaFolio.Enabled = true;
                    bbiRateSummary.Enabled = true;
                    bbiRegCard.Enabled = true;

                    //PREVIEW
                    bsiPreview.Enabled = true;
                }
                #endregion

                #region Check-IN

                else if (state == CNETConstantes.CHECKED_IN_STATE)
                {
                    // Options: Enable only Add-on menu
                    bsiOptions.Enabled = true;
                    bbiAccompyingGuest.Enabled = true;
                    bbiReplicate.Enabled = true;
                    bbiAttachment.Enabled = true;
                    bbiHistory.Enabled = true;
                    bbiHouseKeeping.Enabled = true;
                    bbiMessages.Enabled = true;
                    bbiRouting.Enabled = true;
                    bbiShares.Enabled = true;
                    bbiWakeupCalls.Enabled = true;
                    bbiPrevilage.Enabled = true;
                    bbiScheduledActivity.Enabled = true;
                    bbiTravelDetail.Enabled = true;
                    bbiIssueCard.Enabled = true;
                    bbiRoomAssignment.Enabled = true;
                    bbiSetAlarm.Enabled = true;

                    //Activities
                    bsiActivites.Enabled = true;
                    bbiWaitlist.Enabled = false;
                    bbiSixPM.Enabled = false;
                    bbiConformation.Enabled = false;
                    bbiCancel.Enabled = false;
                    bbiCheckIN.Enabled = false;
                    bbiQuickCheckIn.Enabled = false;
                    bbiCheckOut.Enabled = true;
                    bbiGroupCheckout.Enabled = true;
                    bbiPostMaster.Enabled = true;
                    bbiReinstate.Enabled = true;

                    //Amendment
                    bsiAmendment.Enabled = true;
                    bbiProfileAmendment.Enabled = true;
                    bbiDateAmendment.Enabled = true;
                    bbiRegDetail.Enabled = true;
                    bbiRateAdjustment.Enabled = true;
                    bbiMoveRoom.Enabled = true;
                    bbiPaymentOptions.Enabled = true;
                    bbiOther.Enabled = true;

                    //Billing
                    bsiBilling.Enabled = true;
                    bbiManualCharge.Enabled = true;
                    bbiBillTransfer.Enabled = true;
                    bbiBillSplit.Enabled = true;
                    bbiRoomCharges.Enabled = true;
                    bbiCashReceipt.Enabled = true;
                    bbiRebate.Enabled = true;
                    bbiPaidOut.Enabled = true;
                    bbiRefund.Enabled = false;

                    //Preview
                    bsiPreview.Enabled = true;
                    bbiFolio.Enabled = true;
                    bbiProformaFolio.Enabled = true;
                    bbiRateSummary.Enabled = true;
                    bbiRegCard.Enabled = true;

                    //PREVIEW
                    bsiPreview.Enabled = true;

                }

                #endregion

                #region Six-PM

                else if (state == CNETConstantes.SIX_PM_STATE)
                {
                    // Options: Enable only Add-on menu
                    bsiOptions.Enabled = true;
                    bbiAccompyingGuest.Enabled = true;
                    bbiReplicate.Enabled = true;
                    bbiAttachment.Enabled = true;
                    bbiHistory.Enabled = true;
                    bbiHouseKeeping.Enabled = true;
                    bbiMessages.Enabled = true;
                    bbiRouting.Enabled = false;
                    bbiShares.Enabled = true;
                    bbiWakeupCalls.Enabled = false;
                    bbiPrevilage.Enabled = true;
                    bbiScheduledActivity.Enabled = true;
                    bbiTravelDetail.Enabled = true;
                    bbiIssueCard.Enabled = false;
                    bbiRoomAssignment.Enabled = true;
                    bbiSetAlarm.Enabled = false;

                    //Activities
                    bsiActivites.Enabled = true;
                    bbiWaitlist.Enabled = true;
                    bbiSixPM.Enabled = false;
                    bbiConformation.Enabled = true;
                    bbiCancel.Enabled = true;
                    bbiCheckIN.Enabled = true;
                    bbiGroupCheckout.Enabled = false;
                    bbiQuickCheckIn.Enabled = true;
                    bbiCheckOut.Enabled = false;
                    bbiPostMaster.Enabled = true;
                    bbiReinstate.Enabled = false;

                    //Amendment
                    bsiAmendment.Enabled = true;
                    bbiProfileAmendment.Enabled = true;
                    bbiDateAmendment.Enabled = true;
                    bbiRegDetail.Enabled = true;
                    bbiRateAdjustment.Enabled = true;
                    bbiMoveRoom.Enabled = true;
                    bbiPaymentOptions.Enabled = true;
                    bbiOther.Enabled = true;

                    //Billing
                    bsiBilling.Enabled = true;
                    bbiManualCharge.Enabled = false;
                    bbiBillTransfer.Enabled = false;
                    bbiRoomCharges.Enabled = true;
                    //bbiRoomCharges.Enabled = false;
                    bbiBillSplit.Enabled = false;
                    bbiCashReceipt.Enabled = true;
                    bbiRebate.Enabled = false;
                    bbiPaidOut.Enabled = true;
                    bbiRefund.Enabled = false;

                    //Preview
                    bsiPreview.Enabled = true;
                    bbiFolio.Enabled = true;
                    bbiProformaFolio.Enabled = true;
                    bbiRateSummary.Enabled = true;
                    bbiRegCard.Enabled = true;

                    //PREVIEW
                    bsiPreview.Enabled = true;
                }

                #endregion

                #region Pending

                else if (state == CNETConstantes.OSD_PENDING_STATE)
                {
                    // Options: Enable only Add-on menu
                    bsiOptions.Enabled = true;
                    bbiAccompyingGuest.Enabled = true;
                    bbiReplicate.Enabled = true;
                    bbiAttachment.Enabled = true;
                    bbiHistory.Enabled = true;
                    bbiHouseKeeping.Enabled = true;
                    bbiMessages.Enabled = true;
                    bbiRouting.Enabled = true;
                    bbiShares.Enabled = true;
                    bbiWakeupCalls.Enabled = true;
                    bbiPrevilage.Enabled = true;
                    bbiScheduledActivity.Enabled = true;
                    bbiTravelDetail.Enabled = true;
                    bbiIssueCard.Enabled = true;
                    bbiRoomAssignment.Enabled = true;
                    bbiSetAlarm.Enabled = false;



                    //Activities
                    bsiActivites.Enabled = true;
                    bbiWaitlist.Enabled = false;
                    bbiSixPM.Enabled = false;
                    bbiConformation.Enabled = false;
                    bbiCancel.Enabled = false;
                    bbiCheckIN.Enabled = true;
                    bbiQuickCheckIn.Enabled = true;
                    bbiCheckOut.Enabled = true;
                    bbiGroupCheckout.Enabled = true;
                    bbiPostMaster.Enabled = false;
                    bbiReinstate.Enabled = false;

                    //Amendment
                    bsiAmendment.Enabled = true;
                    bbiProfileAmendment.Enabled = true;
                    bbiDateAmendment.Enabled = true;
                    bbiRegDetail.Enabled = true;
                    bbiRateAdjustment.Enabled = true;
                    bbiMoveRoom.Enabled = false;
                    bbiPaymentOptions.Enabled = true;
                    bbiOther.Enabled = true;

                    //Billing
                    bsiBilling.Enabled = true;
                    bbiManualCharge.Enabled = false;
                    bbiBillTransfer.Enabled = false;
                    bbiRoomCharges.Enabled = false;
                    bbiBillSplit.Enabled = false;
                    bbiCashReceipt.Enabled = true;
                    bbiRebate.Enabled = false;
                    bbiPaidOut.Enabled = true;
                    bbiRefund.Enabled = false;

                    //Preview
                    bsiPreview.Enabled = true;
                    bbiFolio.Enabled = true;
                    bbiProformaFolio.Enabled = true;
                    bbiRateSummary.Enabled = true;
                    bbiRegCard.Enabled = true;

                    //PREVIEW
                    bsiPreview.Enabled = true;
                }

                #endregion

                #region Guaranteed

                else if (state == CNETConstantes.GAURANTED_STATE)
                {
                    // Options: Enable only Add-on menu
                    bsiOptions.Enabled = true;
                    bbiAccompyingGuest.Enabled = true;
                    bbiReplicate.Enabled = true;
                    bbiAttachment.Enabled = true;
                    bbiHistory.Enabled = true;
                    bbiHouseKeeping.Enabled = true;
                    bbiMessages.Enabled = true;
                    bbiRouting.Enabled = false;
                    bbiShares.Enabled = true;
                    bbiWakeupCalls.Enabled = false;
                    bbiPrevilage.Enabled = true;
                    bbiScheduledActivity.Enabled = true;
                    bbiTravelDetail.Enabled = true;
                    bbiIssueCard.Enabled = true;
                    bbiRoomAssignment.Enabled = true;
                    bbiReinstate.Enabled = false;
                    bbiSetAlarm.Enabled = false;


                    //Activities
                    bsiActivites.Enabled = true;
                    bbiWaitlist.Enabled = true;
                    bbiSixPM.Enabled = true;
                    bbiConformation.Enabled = false;
                    bbiCancel.Enabled = true;
                    bbiCheckIN.Enabled = true;
                    bbiQuickCheckIn.Enabled = true;
                    bbiCheckOut.Enabled = false;
                    bbiGroupCheckout.Enabled = false;
                    bbiPostMaster.Enabled = true;

                    //Amendment
                    bsiAmendment.Enabled = true;
                    bbiProfileAmendment.Enabled = true;
                    bbiDateAmendment.Enabled = true;
                    bbiRegDetail.Enabled = true;
                    bbiRateAdjustment.Enabled = true;
                    bbiMoveRoom.Enabled = true;
                    bbiPaymentOptions.Enabled = true;
                    bbiOther.Enabled = true;

                    //Billing
                    bsiBilling.Enabled = true;
                    bbiManualCharge.Enabled = false;
                    bbiRoomCharges.Enabled = true;
                    //bbiRoomCharges.Enabled = false;
                    bbiBillTransfer.Enabled = false;
                    bbiBillSplit.Enabled = false;
                    bbiCashReceipt.Enabled = true;
                    bbiRebate.Enabled = false;
                    bbiPaidOut.Enabled = true;
                    bbiRefund.Enabled = false;

                    //Preview
                    bsiPreview.Enabled = true;
                    bbiFolio.Enabled = true;
                    bbiProformaFolio.Enabled = true;
                    bbiRateSummary.Enabled = true;
                    bbiRegCard.Enabled = true;

                    //PREVIEW
                    bsiPreview.Enabled = true;
                }

                #endregion

                #region Canceled

                else if (state == CNETConstantes.OSD_CANCEL_STATE)
                {
                    // Options: Enable only Add-on menu
                    bsiOptions.Enabled = false;
                    bbiAccompyingGuest.Enabled = true;
                    bbiAttachment.Enabled = true;
                    bbiHistory.Enabled = true;
                    bbiHouseKeeping.Enabled = true;
                    bbiMessages.Enabled = true;
                    bbiRouting.Enabled = true;
                    bbiShares.Enabled = true;
                    bbiWakeupCalls.Enabled = true;
                    bbiPrevilage.Enabled = true;
                    bbiScheduledActivity.Enabled = true;
                    bbiTravelDetail.Enabled = true;
                    bbiIssueCard.Enabled = false;
                    bbiRoomAssignment.Enabled = true;
                    bbiSetAlarm.Enabled = false;


                    //Activities
                    bsiActivites.Enabled = true;

                    bbiWaitlist.Enabled = false;
                    bbiSixPM.Enabled = false;
                    bbiConformation.Enabled = false;
                    bbiCancel.Enabled = false;
                    bbiCheckIN.Enabled = false;
                    bbiQuickCheckIn.Enabled = false;
                    bbiCheckOut.Enabled = false;
                    bbiGroupCheckout.Enabled = false;
                    bbiPostMaster.Enabled = false;
                    bbiReinstate.Enabled = true;

                    //Amendment
                    bsiAmendment.Enabled = false;
                    bbiProfileAmendment.Enabled = false;
                    bbiDateAmendment.Enabled = true;
                    bbiRegDetail.Enabled = false;
                    bbiRateAdjustment.Enabled = false;
                    bbiMoveRoom.Enabled = false;
                    bbiPaymentOptions.Enabled = false;
                    bbiOther.Enabled = false;

                    //Billing
                    bsiBilling.Enabled = false;

                    //PREVIEW
                    bsiPreview.Enabled = false;
                }

                #endregion

                #region Waiting

                else if (state == CNETConstantes.OSD_WAITLIST_STATE)
                {
                    // Options: Enable only Add-on menu
                    bsiOptions.Enabled = true;
                    bbiAccompyingGuest.Enabled = true;
                    bbiReplicate.Enabled = true;
                    bbiAttachment.Enabled = true;
                    bbiHistory.Enabled = true;
                    bbiHouseKeeping.Enabled = true;
                    bbiMessages.Enabled = true;
                    bbiRouting.Enabled = false;
                    bbiShares.Enabled = true;
                    bbiWakeupCalls.Enabled = false;
                    bbiPrevilage.Enabled = true;
                    bbiScheduledActivity.Enabled = true;
                    bbiTravelDetail.Enabled = true;
                    bbiIssueCard.Enabled = false;
                    bbiRoomAssignment.Enabled = true;
                    bbiSetAlarm.Enabled = false;
                    //Activities
                    bsiActivites.Enabled = true;
                    bbiWaitlist.Enabled = false;
                    bbiSixPM.Enabled = true;
                    bbiConformation.Enabled = true;
                    bbiCancel.Enabled = true;
                    bbiCheckIN.Enabled = true;
                    bbiQuickCheckIn.Enabled = true;
                    bbiCheckOut.Enabled = false;
                    bbiGroupCheckout.Enabled = false;
                    bbiPostMaster.Enabled = true;
                    bbiReinstate.Enabled = false;

                    //Amendment
                    bsiAmendment.Enabled = true;
                    bbiProfileAmendment.Enabled = true;
                    bbiDateAmendment.Enabled = true;
                    bbiRegDetail.Enabled = true;
                    bbiRateAdjustment.Enabled = true;
                    bbiMoveRoom.Enabled = true;
                    bbiPaymentOptions.Enabled = true;
                    bbiOther.Enabled = true;

                    //Billing
                    bsiBilling.Enabled = false;

                    //PREVIEW
                    bsiPreview.Enabled = true;
                }

                #endregion

                #region No Show
                else if (state == CNETConstantes.NO_SHOW_STATE)
                {
                    // Options: Enable only Add-on menu
                    bsiOptions.Enabled = false;
                    bbiAccompyingGuest.Enabled = false;
                    bbiReplicate.Enabled = false;
                    bbiAttachment.Enabled = false;
                    bbiHistory.Enabled = false;
                    bbiHouseKeeping.Enabled = false;
                    bbiMessages.Enabled = false;
                    bbiRouting.Enabled = false;
                    bbiShares.Enabled = false;
                    bbiWakeupCalls.Enabled = false;
                    bbiPrevilage.Enabled = false;
                    bbiScheduledActivity.Enabled = false;
                    bbiTravelDetail.Enabled = false;
                    bbiIssueCard.Enabled = false;
                    bbiRoomAssignment.Enabled = false;
                    bbiSetAlarm.Enabled = false;

                    //Activities
                    bsiActivites.Enabled = true;
                    bbiWaitlist.Enabled = false;
                    bbiSixPM.Enabled = false;
                    bbiConformation.Enabled = false;
                    bbiCancel.Enabled = false;
                    bbiCheckIN.Enabled = false;
                    bbiQuickCheckIn.Enabled = false;
                    bbiCheckOut.Enabled = true;
                    bbiGroupCheckout.Enabled = true;
                    bbiPostMaster.Enabled = true;
                    bbiReinstate.Enabled = false;

                    //Amendment
                    bsiAmendment.Enabled = true;
                    bbiProfileAmendment.Enabled = true;
                    bbiDateAmendment.Enabled = true;
                    bbiRegDetail.Enabled = true;
                    bbiRateAdjustment.Enabled = true;
                    bbiMoveRoom.Enabled = false;
                    bbiPaymentOptions.Enabled = true;
                    bbiOther.Enabled = true;

                    //Billing
                    bsiBilling.Enabled = true;
                    bbiManualCharge.Enabled = true;
                    bbiRoomCharges.Enabled = false;
                    bbiBillTransfer.Enabled = true;
                    bbiBillSplit.Enabled = true;
                    bbiCashReceipt.Enabled = true;
                    bbiRebate.Enabled = true;
                    bbiPaidOut.Enabled = true;
                    bbiRefund.Enabled = true;

                    //Preview
                    bsiPreview.Enabled = true;
                    bbiFolio.Enabled = true;
                    bbiProformaFolio.Enabled = true;
                    bbiRateSummary.Enabled = true;
                    bbiRegCard.Enabled = true;

                    //PREVIEW
                    bsiPreview.Enabled = true;
                }
                #endregion

            }
            else
            {
                #region Empty

                // Options: Enable only Add-on menu
                bsiOptions.Enabled = false;

                //Activities
                bsiActivites.Enabled = false;

                //Amendment
                bsiAmendment.Enabled = false;

                //Billing
                bsiBilling.Enabled = false;

                //PREVIEW
                bsiPreview.Enabled = false;

                #endregion

            }

            if (Dispatcher.CheckClosing(false))
            {
                //SecurityCheck(state);
            }
            else
            {
                DisableButtons();
            }
        }


        //Populate Logbooks
        private void PopulateLogBooks(RegistrationListVMDTO regVM)
        {
            try
            {
                gc_logBook.DataSource = null;
                if (regVM.LogMessages != null && regVM.LogMessages > 0)
                {
                    List<VwMessageViewDTO> logList = UIProcessManager.GetMessageViewByRegistrationId(regVM.Id);
                    if (logList == null || logList.Count == 0) return;
                    List<LogBookVM> lbDtoList = new List<LogBookVM>();
                    foreach (var l in logList)
                    {
                        LogBookVM dto = new LogBookVM();
                        dto.Message = l.Message;
                        dto.Device = l.SenderDevice;
                        dto.Date = l.IssuedDate;
                        dto.Author = l.SenderName;
                        dto.IsSeen = l.IsVoid;
                        dto.LogMessage = l;
                        lbDtoList.Add(dto);
                    }
                    gc_logBook.DataSource = lbDtoList;
                }
                cardView1.RefreshData();
            }
            catch (Exception ex)
            {
                SystemMessage.ShowModalInfoMessage("Error in loading logs. Detail:: " + ex.Message, "ERROR");
            }
        }

        //Populate Travel Details
        private void PopulateTravelDetails(RegistrationListVMDTO regVM)
        {
            try
            {
                gc_logBook.DataSource = null;
                if (regVM.TravelDetails != null && regVM.TravelDetails > 0)
                {
                    List<VwTravelDetailDTO> travelDetailList = UIProcessManager.GetTravelDetailByRegistrationId(regVM.Id);
                    if (travelDetailList == null || travelDetailList.Count == 0) return;
                    List<TravelDetailVM> tdDtoList = new List<TravelDetailVM>();
                    foreach (var tDetail in travelDetailList)
                    {
                        TravelDetailVM dto = new TravelDetailVM();

                        dto.Type = tDetail.TransactionType;
                        dto.Station = tDetail.FromStation;
                        dto.TransNum = tDetail.TradeName;
                        dto.Time = tDetail.TravelTimestamp;
                        dto.TravelDetail = tDetail;
                        tdDtoList.Add(dto);
                    }

                    gcTravelDetail.DataSource = tdDtoList;
                    gvTravelDetail.RefreshData();
                    gvTravelDetail.BestFitColumns();
                }

            }
            catch (Exception ex)
            {
                SystemMessage.ShowModalInfoMessage("Error in loading travel detail. Detail:: " + ex.Message, "ERROR");
            }
        }

        #endregion

        #region Event Handlers

        void ceSeen_CheckedChanged(object sender, EventArgs e)
        {
            try
            {

                DialogResult dr = XtraMessageBox.Show("Do you want to change read status?", "Log Book", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (dr == System.Windows.Forms.DialogResult.No) return;

                CheckEdit view = sender as CheckEdit;
                if (view == null) return;
                LogBookVM dto = cardView1.GetFocusedRow() as LogBookVM;
                if (dto == null) return;
                VwMessageViewDTO msg = dto.LogMessage;
                if (msg == null) return;

                // Progress_Reporter.Show_Progress("Changing Status");

                if (adSeen == null)
                    adSeen = UIProcessManager.GetActivityDefinitionBydescriptionandreference(CNETConstantes.LU_ACTIVITY_SEEN, CNETConstantes.MESSAGE).FirstOrDefault();
                if (adSeen != null)
                {
                    if (view.Checked)
                    {
                        if (msg.IsVoid)
                        {
                            ////CNETInfoReporter.Hide();
                            return;
                        }
                        DateTime? currentTime = UIProcessManager.GetServiceTime();
                        if (currentTime != null)
                        {
                            List<ActivityDTO> activityList = null;
                            if (allActivity != null)
                                activityList = allActivity.Where(a => a.Reference == msg.Id).ToList();
                            if (activityList == null || activityList.Count == 0)
                            {
                                ActivityDTO act = ActivityLogManager.SetupActivity(currentTime.Value, adSeen.Id, CNETConstantes.VOUCHER_COMPONENET);
                                act.Reference = msg.Id;
                                UIProcessManager.CreateActivity(act);
                                if (adSeen != null)
                                {
                                    allActivity = UIProcessManager.GetActivityByactivityDefinitionanduser(adSeen.Id, LocalBuffer.LocalBuffer.CurrentLoggedInUser.Id);
                                }
                                dto.LogMessage.IsVoid = true;
                            }

                        }
                    }
                    else
                    {
                        List<ActivityDTO> activityList = null;
                        if (allActivity != null)
                            activityList = allActivity.Where(a => a.Reference == msg.Id).ToList();
                        if (activityList != null || activityList.Count > 0)
                        {
                            ActivityDTO activity = activityList.FirstOrDefault();
                            bool flag = UIProcessManager.DeleteActivityById(activity.Id);
                            if (adSeen != null)
                            {
                                allActivity = UIProcessManager.GetActivityByactivityDefinitionanduser(adSeen.Id, LocalBuffer.LocalBuffer.CurrentLoggedInUser.Id);
                            }
                            dto.LogMessage.IsVoid = false;
                        }
                    }
                }

                ////CNETInfoReporter.Hide();
            }
            catch (Exception ex)
            {

            }
        }

        void risleCompany_EditValueChanged(object sender, EventArgs e)
        {
            SearchLookUpEdit searchLookUpEdit = sender as SearchLookUpEdit;
            if (searchLookUpEdit == null)
                return;

            beiCompanySearch.EditValue = searchLookUpEdit.EditValue;

            List<RegistrationListVMDTO> Customfilter = FilterByCompany();
            //Filter by DateRange
            if (rpgDateRange.Visible && beiStartDate.EditValue != null && beiEndDate.EditValue != null)
            {
                DateTime Startdate = Convert.ToDateTime(beiStartDate.EditValue.ToString()).Date;
                DateTime Enddate = Convert.ToDateTime(beiEndDate.EditValue.ToString()).Date.AddHours(23).AddMinutes(59).AddSeconds(59);

                if (Startdate > Enddate)
                {
                    SystemMessage.ShowModalInfoMessage("Start Date Can't Be greater Than End Date", "ERROR");
                    beiEndDate.EditValue = Startdate;
                }
                else
                {
                    if (Customfilter != null)
                    {
                        FilterByDateRange(Startdate, Enddate, Customfilter);

                    }
                    else
                    {
                        FilterByDateRange(Startdate, Enddate);
                    }
                }
            }
        }

        void riteCompany_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                TextEdit bu = sender as TextEdit;
                List<RegistrationListVMDTO> filteredList = null;

                if (beiState.EditValue != null)
                {
                    LoadRegistrationDocument(Convert.ToInt32(beiState.EditValue), null, null);
                }

                if (bu != null && bu.Text != "")
                {
                    filteredList =
                        regListVM.Where(
                            l => (l.Company != null && l.Company.Contains(bu.Text.ToUpper()))).ToList();
                }

                if (beiGuest.EditValue != null && beiGuest.EditValue != "" && filteredList != null)
                {
                    filteredList =
                        filteredList.Where(
                            l => l.Guest != null && l.Guest == beiGuest.EditValue.ToString().ToLower()).ToList();
                }
                if (beiRoom.EditValue != null && beiRoom.EditValue != "" && filteredList != null)
                {
                    filteredList = filteredList.Where(l => bu != null && (l.Room != null && l.Room.Contains(beiRoom.EditValue.ToString()))).ToList();
                }

                BindGridDataSource(filteredList);
            }
        }

        void risleGuestSearch_EditValueChanged(object sender, EventArgs e)
        {
            SearchLookUpEdit searchLookUpEdit = sender as SearchLookUpEdit;
            if (searchLookUpEdit == null)
                return;

            beiGuest.EditValue = searchLookUpEdit.EditValue;
            PopulateSearchCriteria();
            if (beiGuest.EditValue != null)
                FilterByGuest(Convert.ToInt32(beiGuest.EditValue));

        }

        void ritRoom_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.KeyCode == Keys.Enter)
            {
                TextEdit bu = sender as TextEdit;

                List<RegistrationListVMDTO> filteredList = new List<RegistrationListVMDTO>();


                if (regListVM == null) return;
                if (beiGuest.EditValue != null)
                {
                    filteredList = FilterByGuest(Convert.ToInt32(beiGuest.EditValue));
                    if (filteredList != null)
                        filteredList = filteredList.Where(l => l.Room != null ? l.Room.Contains(bu.Text) : false).ToList();
                    else
                        filteredList = regListVM.Where(l => l.Room != null ? l.Room.Contains(bu.Text) : false).ToList();
                }
                else if (beiCompanySearch.EditValue != null)
                {
                    filteredList = FilterByCompany();
                    if (filteredList != null)
                        filteredList = filteredList.Where(l => l.Room != null ? l.Room.Contains(bu.Text) : false).ToList();
                    else
                        filteredList = regListVM.Where(l => l.Room != null ? l.Room.Contains(bu.Text) : false).ToList();
                }
                else
                {
                    filteredList = regListVM.Where(l => l.Room != null ? l.Room.Contains(bu.Text) : false).ToList();
                }



                if (filteredList != null)
                    BindGridDataSource(filteredList);
            }
        }

        private void bbiTravelDetail_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            _frmTravelDetail = new frmTravelDetail();
            RegistrationListVMDTO regExten = (RegistrationListVMDTO)gridView_regDoc.GetFocusedRow();
            //RegistrationListVMDTO regExten = (RegistrationListVMDTO)gridView_regDoc.GetRow(gridView_regDoc.GetSelectedRows()[0]);
            if (regExten != null)
            {
                _frmTravelDetail.RegExtension = regExten;
                _frmTravelDetail.ShowDialog(this);
            }
            else
            {
                SystemMessage.ShowModalInfoMessage("Please Select One registration", "ERROR");
            }
        }

        private void bbiDailyDetail_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //Home.OpenForm(new frmDailyDetail(), "DAILY DETAIL", null);
        }

        private void bbiRateDetail_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            // Home.OpenForm(new frmRateDetail(), "RATE DETAIL", null);
        }

        private void bbiRefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            // Progress_Reporter.Show_Progress("Refreshing Data ..........", "Please Wait..");
            try
            {
                //  // AllTransactionReference = UIProcessManager.SelectAllTransactionReference();
                RefreshRegDocBrowser();

                //filter state
                FilterByState(regListVM);

                if (_doorLock != null)
                {
                    //Filter by guest card
                    string cardSn = _doorLock.GetCardSN(false);
                    if (!string.IsNullOrEmpty(cardSn) && cardSn != "FFFFFFFF")
                    {
                        //var voExtDoorLock = LocalBuffer.LocalBuffer.VoucherExtensionBufferList.FirstOrDefault(v => v.Type == CNETConstantes.VOUCHER_EXTENTION_DEFINITION_DOOR_LOCK && v.VoucherDefinition == CNETConstantes.REGISTRATION_VOUCHER);
                        //if (voExtDoorLock != null)
                        //{
                        FilterByDoorlock(cardSn);
                        // }
                    }
                }
                else
                {

                    //filter guest
                    if (beiGuest.EditValue != null && !string.IsNullOrEmpty(beiGuest.EditValue.ToString()))
                        FilterByGuest(Convert.ToInt32(beiGuest.EditValue));
                    //filter company
                    List<RegistrationListVMDTO> Customfilter = FilterByCompany();

                    //Filter by registration number
                    if (beiMarket.EditValue != null && !string.IsNullOrEmpty(beiMarket.EditValue.ToString()))
                        FilterByRegNumber(beiMarket.EditValue.ToString());

                    //Filter by DateRange
                    if (rpgDateRange.Visible && beiStartDate.EditValue != null && beiEndDate.EditValue != null)
                    {
                        DateTime Startdate = Convert.ToDateTime(beiStartDate.EditValue.ToString()).Date;
                        DateTime Enddate = Convert.ToDateTime(beiEndDate.EditValue.ToString()).Date.AddHours(23).AddMinutes(59).AddSeconds(59);

                        if (Startdate > Enddate)
                        {
                            SystemMessage.ShowModalInfoMessage("Start Date Can't Be greater Than End Date", "ERROR");
                            beiEndDate.EditValue = Startdate;
                        }
                        else
                        {
                            if (Customfilter != null)
                            {
                                FilterByDateRange(Startdate, Enddate, Customfilter);

                            }
                            else
                            {
                                FilterByDateRange(Startdate, Enddate);
                            }
                        }
                    }
                }

                beiRoom.EditValue = "";
                beiGuest.EditValue = "";
                // beiCompanySearch.EditValue = "";
                ////CNETInfoReporter.Hide();
            }
            catch
            {
                ////CNETInfoReporter.Hide();
            }
        }

        private void barButtonItem8_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            _frmReconciliation = new frmReconciliation();
            RegistrationListVMDTO regExten = (RegistrationListVMDTO)gridView_regDoc.GetFocusedRow();
            //RegistrationListVMDTO regExten = (RegistrationListVMDTO)gridView_regDoc.GetRow(gridView_regDoc.GetSelectedRows()[0]);
            if (regExten.Registration != null)
            {
                List<VoucherReconcilationDTO> conciledReport = UIProcessManager.GetVoucherReconcilationByType("folio", regExten.Id, regExten.Arrival, regExten.Departure);
                _frmReconciliation.ConciledReport = conciledReport;
                _frmReconciliation.ShowDialog(this);
            }
            else
            {
                SystemMessage.ShowModalInfoMessage("Please Select One registration", "ERROR");
            }
        }

        private void barButtonItem19_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            RegistrationListVMDTO regExten = (RegistrationListVMDTO)gridView_regDoc.GetFocusedRow();
           

            //_frmRoomMove = new frmRoomMove();
            //_frmRoomMove.SelectedHotelcode = SelectedHotelcode;
            //RegistrationListVMDTO regExten = (RegistrationListVMDTO)gridView_regDoc.GetRow(gridView_regDoc.GetSelectedRows()[0]);
            if (regExten != null)
            {
                if (regExten.Departure.Date <= CurrentTime.Value.Date)
                {
                    SystemMessage.ShowModalInfoMessage("Please make Date Amendment before Room Move !!", "ERROR");
                    return;
                }



                _frmRoomMove = new frmRoomMove();
                _frmRoomMove.SelectedHotelcode = SelectedHotelcode;
                if (regExten.Room.Contains("#"))
                {
                    SystemMessage.ShowModalInfoMessage("This registration has unassigned rooms!", "ERROR");
                    return;

                }
                else
                {
                    if (_frmRoomMove.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                    {
                        List<RegistrationListVMDTO> filteredList = null;
                        if (regListVM == null) return;
                        if (beiGuest.EditValue != null)
                        {
                            filteredList = FilterByGuest(Convert.ToInt32(beiGuest.EditValue));

                        }
                        else if (beiCompanySearch.EditValue != null)
                        {
                            filteredList = FilterByCompany();

                        }
                        else if (beiRoom.EditValue != null)
                        {
                            if (filteredList != null)
                                filteredList = filteredList.Where(l => l.Room != null ? l.Room.Contains(beiRoom.EditValue.ToString()) : false).ToList();
                            else
                                filteredList = regListVM.Where(l => l.Room != null ? l.Room.Contains(beiRoom.EditValue.ToString()) : false).ToList();
                        }
                        else
                        {
                            RefreshRegDocBrowser();
                        }
                        if (filteredList != null)
                            BindGridDataSource(filteredList);
                    }
                }

            }
            else
            {
                SystemMessage.ShowModalInfoMessage("Please Select One registration", "ERROR");
            }
        }

        private void bbiAccompyingGuest_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            _frmAccompanyingGuest = new frmAccompanyingGuest();
            RegistrationListVMDTO regExten = (RegistrationListVMDTO)gridView_regDoc.GetFocusedRow();
            //RegistrationListVMDTO regExten = (RegistrationListVMDTO)gridView_regDoc.GetRow(gridView_regDoc.GetSelectedRows()[0]);
            if (regExten.Registration != null)
            {
                _frmAccompanyingGuest.Text += @" - " + regExten.Registration + @" - " + regExten.Room;
                _frmAccompanyingGuest.RegistrationExt = regExten;
                _frmAccompanyingGuest.ShowDialog(this);
            }
            else
            {
                SystemMessage.ShowModalInfoMessage("Please Select One registration", "ERROR");
            }

        }

        private void bbiDailyRateDetail_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            _frmDailyRateDetail = new frmDailyRateDetail();
            RegistrationListVMDTO regExten = (RegistrationListVMDTO)gridView_regDoc.GetFocusedRow();
            //RegistrationListVMDTO regExten = (RegistrationListVMDTO)gridView_regDoc.GetRow(gridView_regDoc.GetSelectedRows()[0]);
            if (regExten.Registration != null)
            {
                _frmDailyRateDetail.ShowDialog(this);
            }
            else
            {
                SystemMessage.ShowModalInfoMessage("Please Select One registration", "ERROR");
            }

        }

        private void bbiMoveRoom_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            RegistrationListVMDTO regExten = (RegistrationListVMDTO)gridView_regDoc.GetFocusedRow();
            //RegistrationListVMDTO regExten = (RegistrationListVMDTO)gridView_regDoc.GetRow(gridView_regDoc.GetSelectedRows()[0]);
            if (regExten.Registration != null)
            {
                if (regExten.Departure.Date <= CurrentTime.Value.Date)
                {
                    SystemMessage.ShowModalInfoMessage("Please make Date Amendment before Room Move !!", "ERROR");
                    return;
                }

                _frmRoomMove = new frmRoomMove();
                _frmRoomMove.SelectedHotelcode = SelectedHotelcode;


                if (regExten.Room == null)//regExten.Room.Contains("#")
                {
                    SystemMessage.ShowModalInfoMessage("Please assign a room to this registration before you move it.", "ERROR");
                }
                else
                {
                    if (regExten.Room.Contains("#"))
                    {
                        SystemMessage.ShowModalInfoMessage("This is multi room reservation. You can not move it.", "ERROR");
                    }
                    else
                    {
                        _frmRoomMove.RegExtension = regExten;
                        if (_frmRoomMove.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                        {
                            RefreshRegDocBrowser();

                            List<RegistrationListVMDTO> filteredList = null;
                            if (regListVM == null) return;
                            if (beiGuest.EditValue != null && !string.IsNullOrEmpty(beiGuest.EditValue.ToString()))
                            {
                                filteredList = FilterByGuest(Convert.ToInt32(beiGuest.EditValue));

                            }
                            else if (beiCompanySearch.EditValue != null && !string.IsNullOrEmpty(beiCompanySearch.EditValue.ToString()))
                            {
                                filteredList = FilterByCompany();

                            }
                            else if (beiRoom.EditValue != null && !string.IsNullOrEmpty(beiRoom.EditValue.ToString()))
                            {
                                if (filteredList != null)
                                    filteredList = filteredList.Where(l => l.Room != null ? l.Room.Contains(beiRoom.EditValue.ToString()) : false).ToList();
                                else
                                    filteredList = regListVM.Where(l => l.Room != null ? l.Room.Contains(beiRoom.EditValue.ToString()) : false).ToList();
                            }

                            if (filteredList != null)
                                BindGridDataSource(filteredList);
                        }
                    }

                }

            }
            else
            {
                SystemMessage.ShowModalInfoMessage("Please Select One registration", "ERROR");
            }
        }

        private void bbiRegDateAmendment_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                _frmRegistrationDateAmendement = new frmDateAmendment();
                _frmRegistrationDateAmendement.SelectedHotelcode = SelectedHotelcode;
                RegistrationListVMDTO regExten = (RegistrationListVMDTO)gridView_regDoc.GetFocusedRow();
                //RegistrationListVMDTO regExten = (RegistrationListVMDTO)gridView_regDoc.GetRow(gridView_regDoc.GetSelectedRows()[0]);
                if (regExten.Registration != null)
                {
                    _frmRegistrationDateAmendement.RegistrationExt = regExten;
                    if (_frmRegistrationDateAmendement.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                    {


                        List<RegistrationListVMDTO> filteredList = null;
                        if (regListVM == null) return;
                        if (beiGuest.EditValue != null && !string.IsNullOrEmpty(beiGuest.EditValue.ToString()))
                        {
                            filteredList = FilterByGuest(Convert.ToInt32(beiGuest.EditValue));

                        }
                        else if (beiCompanySearch.EditValue != null && !string.IsNullOrEmpty(beiCompanySearch.EditValue.ToString()))
                        {
                            filteredList = FilterByCompany();

                        }
                        else if (beiRoom.EditValue != null && !string.IsNullOrEmpty(beiRoom.EditValue.ToString()))
                        {
                            if (filteredList != null)
                                filteredList = filteredList.Where(l => l.Room != null ? l.Room.Contains(beiRoom.EditValue.ToString()) : false).ToList();
                            else
                                filteredList = regListVM.Where(l => l.Room != null ? l.Room.Contains(beiRoom.EditValue.ToString()) : false).ToList();
                        }

                        else if (beiSearchCriteria.EditValue != null && beiSearchCriteria.EditValue.ToString().Contains("Over Due Out"))
                        {
                            RefreshRegDocBrowser();
                            filteredList = regListVM.Where(r => r.lastState == CNETConstantes.CHECKED_IN_STATE && r.Departure.Date < CurrentTime.Value.Date).ToList();

                        }
                        else
                        {
                            RefreshRegDocBrowser();
                        }
                        if (filteredList != null)
                            BindGridDataSource(filteredList);
                    }
                }
                else
                {
                    SystemMessage.ShowModalInfoMessage("Please Select One registration", "ERROR");
                }
            }
            catch (Exception ex)
            {
                SystemMessage.ShowModalInfoMessage("ERROR in preparing date amendment." + ex.Message, "ERROR");
            }
        }

        private void bbiRegDetail_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                _frmRegistrationDetail = new frmReservationDetail();
                _frmRegistrationDetail.SelectedHotelcode = SelectedHotelcode;
                RegistrationListVMDTO regExten = (RegistrationListVMDTO)gridView_regDoc.GetFocusedRow();
                //RegistrationListVMDTO regExten = (RegistrationListVMDTO)gridView_regDoc.GetRow(gridView_regDoc.GetSelectedRows()[0]);
                if (regExten != null)
                {
                    _frmRegistrationDetail.RegVoucher = regExten.Id;
                    _frmRegistrationDetail.LastRegState = regExten.lastState;
                    _frmRegistrationDetail.Text += @" - " + regExten.Registration;
                    _frmRegistrationDetail.ShowDialog(this);


                    RefreshRegDocBrowser();

                }
                else
                {
                    SystemMessage.ShowModalInfoMessage("Please Select One registration", "ERROR");
                }
            }
            catch (Exception ex)
            {
                SystemMessage.ShowModalInfoMessage("Error in Registration Detail. DETAIL::" + ex.Message, "ERROR");
            }
        }

        private void bbiProfiles_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            _frmProfileAmendment = new frmProfileAmendment();
            RegistrationListVMDTO regExten = (RegistrationListVMDTO)gridView_regDoc.GetFocusedRow();
            //RegistrationListVMDTO regExten = (RegistrationListVMDTO)gridView_regDoc.GetRow(gridView_regDoc.GetSelectedRows()[0]);
            if (regExten != null)
            {
                _frmProfileAmendment.RegistrationExt = regExten;
                if (_frmProfileAmendment.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                {

                    List<RegistrationListVMDTO> filteredList = null;
                    if (regListVM == null) return;
                    if (beiGuest.EditValue != null)
                    {
                        filteredList = FilterByGuest(Convert.ToInt32(beiGuest.EditValue));

                    }
                    else if (beiCompanySearch.EditValue != null)
                    {
                        filteredList = FilterByCompany();

                    }
                    else if (beiRoom.EditValue != null)
                    {
                        if (filteredList != null)
                            filteredList = filteredList.Where(l => l.Room != null ? l.Room.Contains(beiRoom.EditValue.ToString()) : false).ToList();
                        else
                            filteredList = regListVM.Where(l => l.Room != null ? l.Room.Contains(beiRoom.EditValue.ToString()) : false).ToList();
                    }
                    else
                    {
                        RefreshRegDocBrowser();
                    }
                    if (filteredList != null)
                        BindGridDataSource(filteredList);
                }
            }
            else
            {
                SystemMessage.ShowModalInfoMessage("Please Select One registration", "ERROR");
            }
        }

        private void bbiFolio_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            _frmFolio = new frmFolio();
            RegistrationListVMDTO regExten = (RegistrationListVMDTO)gridView_regDoc.GetFocusedRow();
            //  RegistrationListVMDTO regExten = (RegistrationListVMDTO)gridView_regDoc.GetRow(gridView_regDoc.GetSelectedRows()[0]);
            if (regExten != null)
            {

                _frmFolio.RegistrationExt = regExten;

                _frmFolio.ShowDialog(this);
            }
            else
            {
                SystemMessage.ShowModalInfoMessage("Please Select One registration", "ERROR");
            }
        }

        private void bbiConformation_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            RegistrationListVMDTO regExten = (RegistrationListVMDTO)gridView_regDoc.GetFocusedRow();
            //RegistrationListVMDTO regExten = (RegistrationListVMDTO)gridView_regDoc.GetRow(gridView_regDoc.GetSelectedRows()[0]);
            if (regExten != null)
            {
                VoucherDTO vo = UIProcessManager.GetVoucherById(regExten.Id);

                string state = LocalBuffer.LocalBuffer.ObjectStateDefinitionBufferList.FirstOrDefault(x => x.Id == vo.LastState) != null ? LocalBuffer.LocalBuffer.ObjectStateDefinitionBufferList.FirstOrDefault(x => x.Id == vo.LastState).Description : string.Empty;
                if (vo.LastState == CNETConstantes.OSD_WAITLIST_STATE || vo.LastState == CNETConstantes.SIX_PM_STATE || vo.LastState == CNETConstantes.OSD_CANCEL_STATE)
                {
                    DialogResult dr = MessageBox.Show("Do you want to change  " + state + " state to Conformation state?", "State Change Conformation", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);

                    if (dr == DialogResult.Yes)
                    {
                        frmConformation frmConform = new frmConformation();
                        frmConform.RegExtension = regExten;
                        if (frmConform.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            RefreshRegDocBrowser();
                        }
                    }
                    else if (dr == DialogResult.Cancel)
                    {
                        SystemMessage.ShowModalInfoMessage("state change cancelled!", "MESSAGE");
                    }

                }
                else
                {
                    SystemMessage.ShowModalInfoMessage(state + " state can not be changed to Conformation state!", "ERROR");
                }
            }
            else
            {
                SystemMessage.ShowModalInfoMessage("No registration selected!", "ERROR");
            }
        }

        private void bbiCancel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            RegistrationListVMDTO regExten = (RegistrationListVMDTO)gridView_regDoc.GetFocusedRow();
            if (regExten != null)
            {
                DateTime? CurrentTime = UIProcessManager.GetServiceTime();
                DialogResult dr = MessageBox.Show("Do you want to cancel this registration?", "Night Audit", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                if (dr == DialogResult.Yes)
                {
                    var response = UIProcessManager.GetVoucherBufferById(regExten.Id);
                    if (response == null || !response.Success)
                    {
                        SystemMessage.ShowModalInfoMessage("Unble to get voucher information of the selected registration. Please try agin!", "ERROR");
                        return;
                    }

                    VoucherBuffer vo = response.Data;
                    if (vo == null)
                    {
                        SystemMessage.ShowModalInfoMessage("Unble to get voucher information of the selected registration. Please try agin!", "ERROR");
                        return;
                    }


                    List<TransactionReferenceDTO> transactionReference = UIProcessManager.GetTransactionReferenceByreferenced(regExten.Id);
                    if (regExten.lastState == CNETConstantes.GAURANTED_STATE && transactionReference.Count > 0)
                    {
                        DialogResult dResult =
                                 MessageBox.Show(@"This registration has active transaction and can not be cancelled.Do you want to change to NO-SHOW State?",
                                     @"State Change Conformation", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                        if (dResult == DialogResult.Yes)
                        {

                            //workflow
                            ActivityDefinitionDTO workFlow = UIProcessManager.GetActivityDefinitionBydescriptionandreference(CNETConstantes.LU_ACTIVITY_DEFINATION_NOSHOW, CNETConstantes.REGISTRATION_VOUCHER).FirstOrDefault();

                            if (workFlow == null)
                            {
                                SystemMessage.ShowModalInfoMessage("Please Define Workflow of NO-SHOW for REGISTRATION Voucher!", "ERROR");
                                return;
                            }

                            vo.Voucher.LastState = CNETConstantes.NO_SHOW_STATE;
                            vo.Activity = ActivityLogManager.SetupActivity(CurrentTime.Value, workFlow.Id, CNETConstantes.PMS_Pointer, "");
                            if (vo.TransactionReferencesBuffer != null && vo.TransactionReferencesBuffer.Count > 0)
                                vo.TransactionReferencesBuffer.ToList().ForEach(x => x.ReferencedActivity = null);


                            vo.TransactionCurrencyBuffer = null;

                            if (UIProcessManager.UpdateVoucherBuffer(vo) != null)
                            {
                                XtraMessageBox.Show("Successfully Changed to No-Show State", "Successfull Operation", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                RefreshRegDocBrowser();

                            }
                        }
                        else if (dResult == DialogResult.No)
                        {
                            SystemMessage.ShowModalInfoMessage("state change cancelled!", "ERROR");
                            return;
                        }
                    }
                    else
                    {

                        frmCancellation frmCancel = new frmCancellation();
                        frmCancel.RegExtension = regExten;
                        if (frmCancel.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            RefreshRegDocBrowser();
                        }

                    }
                }
            }
            else
            {
                SystemMessage.ShowModalInfoMessage("No registration selected!", "ERROR");
            }
        }

        private void bbiWaitlist_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            RegistrationListVMDTO regExten = (RegistrationListVMDTO)gridView_regDoc.GetFocusedRow();
            //RegistrationListVMDTO regExten = (RegistrationListVMDTO)gridView_regDoc.GetRow(gridView_regDoc.GetSelectedRows()[0]);
            if (regExten != null)
            {

                var osd = LocalBuffer.LocalBuffer.ObjectStateDefinitionBufferList.FirstOrDefault(o => o.Id == regExten.lastState);

                string state = osd == null ? string.Empty : osd.Description;
                if (regExten.lastState == CNETConstantes.SIX_PM_STATE || regExten.lastState == CNETConstantes.GAURANTED_STATE || regExten.lastState == CNETConstantes.OSD_CANCEL_STATE)
                {
                    DialogResult dr = MessageBox.Show("Do you want to change " + state + " state to waitinglist?", "State Change Conformation", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);

                    if (dr == DialogResult.Yes)
                    {
                        frmWaitingList frmWaitList = new frmWaitingList();
                        frmWaitList.RegExtension = regExten;
                        if (frmWaitList.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            if (currentUser != null && device != null)
                            {
                                System.Diagnostics.Debug.Assert(CurrentTime != null, "CurrentTime != null");


                            }
                            RefreshRegDocBrowser();
                        }
                    }
                    else if (dr == DialogResult.Cancel)
                    {
                        SystemMessage.ShowModalInfoMessage("state changing cancelled!", "MESSAGE");
                    }

                }
                else
                {
                    SystemMessage.ShowModalInfoMessage(state + " state can not be changed to waitinglist!", "ERROR");
                }
            }
            else
            {
                SystemMessage.ShowModalInfoMessage("No registration selected!", "ERROR");
            }
        }

        private void bbiCheckOut_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {

                PMSDataLogger.LogMessage("frmRegistrationList", "******************************************************************");
                PMSDataLogger.LogMessage("frmRegistrationList", "CheckOut Clicked");
                bool isZeroCheckout = false;
                bool isReInstate = false;
                bool isWithoutReciept = false;

                RegistrationListVMDTO regExten = (RegistrationListVMDTO)gridView_regDoc.GetFocusedRow();
                //RegistrationListVMDTO regExten = (RegistrationListVMDTO)gridView_regDoc.GetRow(gridView_regDoc.GetSelectedRows()[0]);
                if (regExten == null)
                {
                    SystemMessage.ShowModalInfoMessage("Please Select a registration.", "ERROR");
                    return;
                }
                if (regExten != null)
                {
                    DateTime? currentTime = UIProcessManager.GetServiceTime();
                    if (currentTime == null) return;

                    //check Activity
                    ActivityDTO activity = null;
                    ActivityDefinitionDTO ad =
                    UIProcessManager.GetActivityDefinitionBydescriptionandreference(CNETConstantes.LU_ACTIVITY_DEFINATION_REINSTATE, CNETConstantes.REGISTRATION_VOUCHER).FirstOrDefault();

                    if (ad != null)
                    {
                        activity = ActivityLogManager.GetActivity(regExten.Id, ad.Id);
                    }

                    // if there is activity with "Reinstate"
                    if (activity != null)
                    {

                        // if the remark is "Without Receipt Reprint"
                        if (activity.Remark == "Without receipt reprint")
                        {
                            isWithoutReciept = true;
                            isReInstate = true;

                        }
                        else if (activity.Remark == "With receipt reprint")
                        {
                            isWithoutReciept = false;
                            isReInstate = true;
                        }
                        else
                        {
                            isWithoutReciept = false;
                            isReInstate = false;
                        }
                    }
                    else
                    {
                        isWithoutReciept = false;
                        isReInstate = false;
                    }

                    //Check any daily room charge post for this registration
                    List<VwDailyChargePostingViewDTO> dailyCharge = UIProcessManager.GetCheckOutDetailViewByVoucher(regExten.Id, CNETConstantes.DAILY_ROOM_CHARGE_VOUCHER, null);


                    if (dailyCharge == null)
                    {
                        var transferedBills = UIProcessManager.GetTransferBill(regExten.Id, null, true);

                        if (isReInstate) //if it is reInstate, check only transfered bills
                        {
                            if (transferedBills != null && transferedBills.Count > 0)
                            {
                                var filtered = transferedBills.Where(t => t.Definition == CNETConstantes.DAILY_ROOM_CHARGE_VOUCHER).ToList();
                                if (filtered != null && filtered.Count > 0)
                                {
                                    //check if this guest has atleast one room charge from activity table
                                    //var workflowRegVoucher = UIProcessManager.GetActivityDefinitionBydescriptionandreference(CNETConstantes.LU_ACTIVITY_DEFINATION_ROOMCHARGE, CNETConstantes.REGISTRATION_VOUCHER).FirstOrDefault();

                                    //if (workflowRegVoucher != null)
                                    //{
                                    //    var activityRoomCharge = UIProcessManager.GetActivityByreference(regExten.Id).FirstOrDefault(a => a.ActivityDefinition == workflowRegVoucher.Id);
                                    //    if (activityRoomCharge != null)
                                    // {
                                    DialogResult dialog = MessageBox.Show(@"Room charges of this registration has been transfered. Do you want to check out with zero balance?", "CHECK OUT", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                    if (dialog == DialogResult.No)
                                    {
                                        return;
                                    }
                                    else
                                    {
                                        isZeroCheckout = true;
                                    }
                                    // }
                                    //  }
                                }
                            }
                        }
                        else
                        {
                            if (transferedBills == null || transferedBills.Count == 0)
                            {
                                SystemMessage.ShowModalInfoMessage("Unable to get room charge posting or room charge posting has not been made for this registration", "ERROR");
                                return;
                            }
                            var filtered = transferedBills.Where(t => t.Definition == CNETConstantes.DAILY_ROOM_CHARGE_VOUCHER).ToList();
                            if (filtered == null || filtered.Count == 0)
                            {
                                SystemMessage.ShowModalInfoMessage("Unable to get room charge posting or room charge posting has not been made for this registration", "ERROR");
                                return;
                            }
                            else
                            {
                                //check if this guest has atleast one room charge from activity table
                                /* var workflowRegVoucher = UIProcessManager.GetActivityDefinitionBydescriptionandreference(CNETConstantes.LU_ACTIVITY_DEFINATION_ROOMCHARGE, CNETConstantes.REGISTRATION_VOUCHER).FirstOrDefault();
                                 if (workflowRegVoucher != null)
                                 {
                                     var activityRoomCharge = UIProcessManager.GetActivityByreference(regExten.Id).FirstOrDefault(a => a.ActivityDefinition == workflowRegVoucher.Id);
                                     if (activityRoomCharge == null)
                                     {
                                         SystemMessage.ShowModalInfoMessage("Unable to get room charge posting or room charge posting has not been made for this registration", "ERROR");
                                         return;
                                     }
                                 }*/

                                DialogResult dialog = MessageBox.Show(@"Room charges of this registration has been transfered. Do you want to check out with zero balance?", "CHECK OUT", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                if (dialog == DialogResult.No)
                                {
                                    return;
                                }
                                else
                                {
                                    isZeroCheckout = true;
                                }
                            }
                        }
                    }
                    if (currentTime.Value != null && regExten.Departure.Date != CurrentTime.Value.Date)
                    {
                        SystemMessage.ShowModalInfoMessage("You can not check out non due out guest. Please amend its date!"+
                            Environment.NewLine+"Server date time: "+currentTime.Value.Date.ToShortDateString() 
                            + Environment.NewLine+"Departure date time "+ regExten.Departure.Date.ToShortDateString(), "ERROR");
                        return;
                    }


                    // Check this registration has room sharing
                    DialogResult drOut = DialogResult.Yes;
                    List<RelationDTO> relations = UIProcessManager.GetRelationalStateByvoucher(regExten.Id);
                    if (relations != null && relations.Count > 0)
                    {
                        drOut = MessageBox.Show(@"This registration has room sharing, Do you want to break the share?", "CHECK OUT", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                        if (drOut == DialogResult.No)
                        {
                            return;
                        }
                    }

                    if (drOut == DialogResult.Yes)
                    {

                        foreach (var rel in relations)
                        {
                            rel.RelationLevel = 0;
                            UIProcessManager.UpdateRelation(rel);
                        }


                        //Perform Checkout
                        if (PerfomCheckout(regExten, isWithoutReciept, isZeroCheckout, isReInstate))
                        {
                            RefreshRegDocBrowser();
                        }
                    }
                }
                else
                {
                    SystemMessage.ShowModalInfoMessage("No registration selected!", "ERROR");
                }
            }
            catch (Exception io)
            {
                XtraMessageBox.Show("Error in Check Out " + Environment.NewLine + io.Message, "Check Out ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void LogMessage(string p)
        {
            throw new NotImplementedException();
        }

        private void bbiPostMaster_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            RegistrationListVMDTO regExten = (RegistrationListVMDTO)gridView_regDoc.GetFocusedRow();
            //RegistrationListVMDTO regExten = (RegistrationListVMDTO)gridView_regDoc.GetRow(gridView_regDoc.GetSelectedRows()[0]);
            if (regExten != null)
            {
                List<RelationDTO> relations = UIProcessManager.GetRelationalStateByvoucher(regExten.Id);
                if (relations.Count > 0)
                {
                    DialogResult drPending = MessageBox.Show(@"This registration has room sharing, Do you want to break the share?", "Changing to Pending State", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    if (drPending == DialogResult.No)
                    {
                        return;
                    }


                }
                var osd = LocalBuffer.LocalBuffer.ObjectStateDefinitionBufferList.FirstOrDefault(o => o.Id == regExten.lastState);
                string state = osd == null ? "" : osd.Description;
                if (regExten.lastState != CNETConstantes.OSD_PENDING_STATE)
                {
                    DialogResult dr = MessageBox.Show("Do you want to change " + state + " state to Pending state?", "State Change Conformation", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);

                    if (dr == DialogResult.Yes)
                    {
                        foreach (var rel in relations)
                        {
                            rel.RelationLevel = 0;
                            UIProcessManager.UpdateRelation(rel);
                        }

                        frmPending frmPending = new frmPending();
                        frmPending.SelectedHotelcode = SelectedHotelcode;
                        frmPending.RegExtension = regExten;
                        if (frmPending.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            RefreshRegDocBrowser();
                        }
                    }
                    else if (dr == DialogResult.Cancel)
                    {
                        SystemMessage.ShowModalInfoMessage("state changing cancelled!", "MESSAGE");
                    }

                }
                else
                {
                    SystemMessage.ShowModalInfoMessage(state + " state can not be changed to Pending state!", "ERROR");
                }
            }
            else
            {
                SystemMessage.ShowModalInfoMessage("No registration selected!", "ERROR");
            }
        }

        private void barButtonItem15_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                frmRegistrationPrivileges _frmRegistrationPrivileges = new frmRegistrationPrivileges();
                RegistrationListVMDTO regExten = (RegistrationListVMDTO)gridView_regDoc.GetFocusedRow();
                //RegistrationListVMDTO regExten = (RegistrationListVMDTO)gridView_regDoc.GetRow(gridView_regDoc.GetSelectedRows()[0]);
                if (regExten != null)
                {
                    RegistrationPrivllegeDTO regPrev = UIProcessManager.GetRegistrationPrivllegeByvoucher(regExten.Id);

                    _frmRegistrationPrivileges.RegistrationExt = regExten;
                    if (regPrev != null)
                    {
                        _frmRegistrationPrivileges.RegistrationPrivllege = regPrev;
                    }
                    if (_frmRegistrationPrivileges.ShowDialog(this) == DialogResult.OK)
                    {

                        RefreshRegDocBrowser();
                    }
                }
                else
                {
                    SystemMessage.ShowModalInfoMessage("Please Select One registration", "ERROR");
                }
            }
            catch (Exception ex)
            {
                SystemMessage.ShowModalInfoMessage("ERROR! " + ex.Message, "ERROR");
            }
        }

        private void bbiRateSummary_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            // Home.OpenForm(new frmRateSummery(), "RATE SUMMERY", null);
            _frmRateSummery = new frmRateSummery();
            RegistrationListVMDTO regExten = (RegistrationListVMDTO)gridView_regDoc.GetFocusedRow();
            //RegistrationListVMDTO regExten = (RegistrationListVMDTO)gridView_regDoc.GetRow(gridView_regDoc.GetSelectedRows()[0]);
            if (regExten != null)
            {
                _frmRateSummery.RegExtension = regExten;
                _frmRateSummery.ShowDialog(this);
            }
            else
            {
                SystemMessage.ShowModalInfoMessage("Please Select One registration", "ERROR");
            }
        }

        private void bbiReplicate_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            RegistrationListVMDTO regExten = (RegistrationListVMDTO)gridView_regDoc.GetFocusedRow();
            //RegistrationListVMDTO regExten = (RegistrationListVMDTO)gridView_regDoc.GetRow(gridView_regDoc.GetSelectedRows()[0]);

            if (regExten != null)
            {
                ActivityDefinitionDTO workFlow = UIProcessManager.GetActivityDefinitionBydescriptionandreference(CNETConstantes.LU_ACTIVITY_DEFINATION_ADD_ON_MADE, CNETConstantes.REGISTRATION_VOUCHER).FirstOrDefault();

                if (workFlow == null)
                {
                    SystemMessage.ShowModalInfoMessage("Please define workflow of ADD-ON MADE for Registration Voucher ", "ERROR");
                    return;
                }

                //Check Activity Previlage
                var userRoleMapper = LocalBuffer.LocalBuffer.UserRoleMapperBufferList.FirstOrDefault(r => r.User == LocalBuffer.LocalBuffer.CurrentLoggedInUser.Id);
                if (userRoleMapper != null)
                {
                    var roleActivity = UIProcessManager.GetRoleActivityByactivityDefinition(workFlow.Id).FirstOrDefault(r => r.Role == userRoleMapper.Role && r.NeedsPassCode);
                    if (roleActivity != null)
                    {
                        frmNeedPassword frmNeedPass = new frmNeedPassword(true);

                        frmNeedPass.ShowDialog();
                        if (!frmNeedPass.IsAutenticated)
                        {
                            ////CNETInfoReporter.Hide();
                            return;
                        }

                    }

                }


                frmReservation reservation = null;
                if (regExten.lastState == CNETConstantes.CHECKED_IN_STATE)
                {
                    reservation = new frmReservation(true);
                }
                else
                {
                    reservation = new frmReservation(false);
                }

                reservation.IsReplicate = true;
                reservation.ExistedRegistration = regExten;

                if (reservation.ShowDialog() == DialogResult.OK)
                {
                    if (currentUser != null && device != null)
                    {

                        DateTime? currentTime = UIProcessManager.GetServiceTime();

                        if (currentTime != null)
                        {
                            ActivityDTO activity = ActivityLogManager.SetupActivity(currentTime.Value, workFlow.Id, CNETConstantes.PMS_Pointer);
                            activity.Reference = regExten.Id;
                            UIProcessManager.CreateActivity(activity);

                        }
                    }
                    RefreshRegDocBrowser();
                }
            }
            else
            {
                SystemMessage.ShowModalInfoMessage("Please Select One registration", "ERROR");
            }

        }

        //shares
        private void bbiShares_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            RegistrationListVMDTO regExten = (RegistrationListVMDTO)gridView_regDoc.GetFocusedRow();
            //RegistrationListVMDTO regExten = (RegistrationListVMDTO)gridView_regDoc.GetRow(gridView_regDoc.GetSelectedRows()[0]);
            if (regExten != null)
            {
                if (regExten.lastState != CNETConstantes.CHECKED_IN_STATE)
                {
                    SystemMessage.ShowModalInfoMessage(" You can not share with non check-in guest!", "ERROR");
                    return;
                }




                //check workflow
                ActivityDefinitionDTO workFlow = UIProcessManager.GetActivityDefinitionBydescriptionandreference(CNETConstantes.LU_ACTIVITY_DEFINATION_SHAREMADE, CNETConstantes.REGISTRATION_VOUCHER).FirstOrDefault();
                if (workFlow == null)
                {
                    SystemMessage.ShowModalInfoMessage("Please define workflow of SHARE MADE for Registration Voucher ", "ERROR");
                    return;
                }

                //Check Activity Previlage
                var userRoleMapper = LocalBuffer.LocalBuffer.UserRoleMapperBufferList.FirstOrDefault(r => r.User == LocalBuffer.LocalBuffer.CurrentLoggedInUser.Id);
                if (userRoleMapper != null)
                {
                    var roleActivity = UIProcessManager.GetRoleActivityByactivityDefinition(workFlow.Id).FirstOrDefault(r => r.Role == userRoleMapper.Role && r.NeedsPassCode);
                    if (roleActivity != null)
                    {
                        frmNeedPassword frmNeedPass = new frmNeedPassword(true);

                        frmNeedPass.ShowDialog();
                        if (!frmNeedPass.IsAutenticated)
                        {
                            ////CNETInfoReporter.Hide();
                            return;
                        }

                    }

                }

                // Check if Room is Share
                List<RelationDTO> AllRelation = UIProcessManager.SelectAllRelation();

                RelationDTO regrelation = AllRelation.FirstOrDefault(r => r.RelationType == CNETConstantes.LK_ROOM_SHARE && r.ReferringObject == regExten.Id && r.RelationLevel == 1);

                if (regrelation != null)
                {
                    SystemMessage.ShowModalInfoMessage("The Room is a Shared room Please Select the Master to share room !", "ERROR");
                    return;
                }

                frmReservation reservation = new frmReservation(true);
                reservation.IsShare = true;
                reservation.ExistedRegistration = regExten;
                reservation.LoadEventArg.Sender = null;

                if (reservation.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    if (currentUser != null && device != null)
                    {
                        DateTime? currentTime = UIProcessManager.GetServiceTime();
                        if (currentTime != null)
                        {
                            ActivityDTO activity = ActivityLogManager.SetupActivity(currentTime.Value, workFlow.Id, CNETConstantes.PMS_Pointer);
                            activity.Reference = regExten.Id;
                            UIProcessManager.CreateActivity(activity);

                        }
                    }
                    RefreshRegDocBrowser();
                }

            }
            else
            {
                SystemMessage.ShowModalInfoMessage("Please Select One registration", "ERROR");
            }

        }

        void ritRoom_EditValueChanged(object sender, EventArgs e)
        {
            TextEdit bu = sender as TextEdit;

            List<RegistrationListVMDTO> filteredList = new List<RegistrationListVMDTO>();

            PopulateSearchCriteria();
            if (regListVM == null) return;
            if (beiGuest.EditValue != null && !string.IsNullOrEmpty(beiGuest.EditValue.ToString()))
            {
                filteredList = FilterByGuest(Convert.ToInt32(beiGuest.EditValue));
                if (filteredList != null)
                    filteredList = filteredList.Where(l => l.Room != null ? l.Room.Contains(bu.Text) : false).ToList();
                else
                    filteredList = regListVM.Where(l => l.Room != null ? l.Room.Contains(bu.Text) : false).ToList();
            }
            else if (beiCompanySearch.EditValue != null && !string.IsNullOrEmpty(beiCompanySearch.EditValue.ToString()))
            {
                filteredList = FilterByCompany();
                if (filteredList != null)
                    filteredList = filteredList.Where(l => l.Room != null ? l.Room.Contains(bu.Text) : false).ToList();
                else
                    filteredList = regListVM.Where(l => l.Room != null ? l.Room.Contains(bu.Text) : false).ToList();
            }
            else
            {
                filteredList = regListVM.Where(l => l.Room != null ? l.Room.Contains(bu.Text) : false).ToList();
            }

            PopulateSearchCriteria("", "", bu.Text);
            if (filteredList != null)
                BindGridDataSource(filteredList);
        }

        private void beiState_EditValueChanged(object sender, EventArgs e)
        {

            grid_regDoc.DataSource = null;
            if (isFromAdvanced) return;

            if (!string.IsNullOrEmpty(FilterKey)) return;

            if (beiState.EditValue != null && !string.IsNullOrEmpty(beiState.EditValue.ToString()))
            {
                PopulateSearchCriteria();

                beiArrival.EditValue = null;
                beiDeparture.EditValue = null;

                int state = Convert.ToInt32(beiState.EditValue);
                if (state == CNETConstantes.SIX_PM_STATE || state == CNETConstantes.GAURANTED_STATE || state == CNETConstantes.OSD_WAITLIST_STATE || state == CNETConstantes.OSD_CANCEL_STATE)
                {
                    beiArrival.EditValue = CurrentTime;
                    DateTime? arrivalTime = beiArrival.EditValue == null ? DateTime.Now : CurrentTime.Value;

                    LoadRegistrationDocument(Convert.ToInt32(beiState.EditValue), arrivalTime, null);
                }
                else if (state == CNETConstantes.CHECKED_OUT_STATE)
                {
                    beiDeparture.EditValue = CurrentTime;
                    DateTime? departureTime = beiDeparture.EditValue == null ? DateTime.Now : CurrentTime.Value;
                    LoadRegistrationDocument(Convert.ToInt32(beiState.EditValue), null, departureTime);
                }
                else if (state == 0)
                {
                    LoadRegistrationDocument(null, null, null);
                }
                else
                {
                    LoadRegistrationDocument(Convert.ToInt32(beiState.EditValue), null, null);
                }

                //filter state
                FilterByState(regListVM);
                //filter guest
                if (beiGuest.EditValue != null && !string.IsNullOrEmpty(beiGuest.EditValue.ToString()))
                    FilterByGuest(Convert.ToInt32(beiState.EditValue));
                //filter company
                FilterByCompany();

                if (state == 0)
                    SwitchControlsByState(null);

            }

        }

        private void bbiSixPM_ItemClick(object sender, ItemClickEventArgs e)
        {
            DateTime? CurrentTime = UIProcessManager.GetServiceTime();
            RegistrationListVMDTO regExten = (RegistrationListVMDTO)gridView_regDoc.GetFocusedRow();
            //RegistrationListVMDTO regExten = (RegistrationListVMDTO)gridView_regDoc.GetRow(gridView_regDoc.GetSelectedRows()[0]);
            if (regExten != null)
            {
                var osd = LocalBuffer.LocalBuffer.ObjectStateDefinitionBufferList.FirstOrDefault(o => o.Id == regExten.lastState);
                string state = osd == null ? string.Empty : osd.Description;
                if (regExten.lastState == CNETConstantes.OSD_WAITLIST_STATE || regExten.lastState == CNETConstantes.GAURANTED_STATE || regExten.lastState == CNETConstantes.OSD_CANCEL_STATE)
                {

                    List<TransactionReferenceDTO> transactionReference = UIProcessManager.GetTransactionReferenceByreferenced(regExten.Id);
                    if (regExten.lastState == CNETConstantes.GAURANTED_STATE && transactionReference.Count > 0)
                    {
                        DialogResult dResult =
                                 MessageBox.Show(@"This registration has active transaction and can not be changed to SIX-PM.Do you want to change to NO-SHOW State?",
                                     @"State Change Conformation", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                        if (dResult == DialogResult.Yes)
                        {
                            var response = UIProcessManager.GetVoucherBufferById(regExten.Id);
                            if (response == null || !response.Success)
                            {

                                SystemMessage.ShowModalInfoMessage("Unble to get voucher information of the selected registration. Please try agin!", "ERROR");
                                return;
                            }

                            VoucherBuffer vo = response.Data;
                            if (vo == null)
                            {
                                SystemMessage.ShowModalInfoMessage("Unble to get voucher information of the selected registration. Please try agin!", "ERROR");
                                return;
                            }
                            //workflow
                            ActivityDefinitionDTO workFlow = UIProcessManager.GetActivityDefinitionBydescriptionandreference(CNETConstantes.LU_ACTIVITY_DEFINATION_NOSHOW, CNETConstantes.REGISTRATION_VOUCHER).FirstOrDefault();

                            if (workFlow == null)
                            {
                                SystemMessage.ShowModalInfoMessage("Please Define Workflow of NO-SHOW for REGISTRATION Voucher!", "ERROR");
                                return;
                            }

                            vo.Voucher.LastState = CNETConstantes.NO_SHOW_STATE;
                            vo.Activity = ActivityLogManager.SetupActivity(CurrentTime.Value, workFlow.Id, CNETConstantes.PMS_Pointer);


                            if (vo.TransactionReferencesBuffer != null && vo.TransactionReferencesBuffer.Count > 0)
                                vo.TransactionReferencesBuffer.ToList().ForEach(x => x.ReferencedActivity = null);


                            vo.TransactionCurrencyBuffer = null;

                            if (UIProcessManager.UpdateVoucherBuffer(vo) != null)
                            {
                                XtraMessageBox.Show("Successfully Changed to No-Show State", "Successfull Operation", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                RefreshRegDocBrowser();

                            }
                        }
                        else if (dResult == DialogResult.No)
                        {
                            SystemMessage.ShowModalInfoMessage("state change cancelled!", "ERROR");
                            return;
                        }
                    }
                    else
                    {

                        DialogResult dr = MessageBox.Show("Do you want to change  " + state + " state to 6 PM?", "State Change Conformation", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);

                        if (dr == DialogResult.Yes)
                        {
                            frmSxPM frm6pm = new frmSxPM();
                            frm6pm.RegExtension = regExten;
                            if (frm6pm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            {
                                if (currentUser != null && device != null)
                                {
                                    System.Diagnostics.Debug.Assert(CurrentTime != null, "CurrentTime != null");

                                }
                                RefreshRegDocBrowser();
                            }
                        }
                        else if (dr == DialogResult.Cancel)
                        {
                            SystemMessage.ShowModalInfoMessage("state change cancelled!", "MESSAGE");
                        }
                    }

                }
                else
                {
                    SystemMessage.ShowModalInfoMessage(state + " can not be changed to 6 PM!", "ERROR");
                }
            }
            else
            {
                SystemMessage.ShowModalInfoMessage("No registration selected!", "ERROR");
            }
        }

        private void bbiQuickCheckIn_ItemClick(object sender, ItemClickEventArgs e)
        {
            RegistrationListVMDTO regExten = (RegistrationListVMDTO)gridView_regDoc.GetFocusedRow();
            //RegistrationListVMDTO regExten = (RegistrationListVMDTO)gridView_regDoc.GetRow(gridView_regDoc.GetSelectedRows()[0]);
            if (regExten != null)
            {
                DateTime? currentTime = UIProcessManager.GetServiceTime();
                if (currentTime == null)
                    return;

                if (regExten.Arrival.Date != currentTime.Value.Date)
                {
                    SystemMessage.ShowModalInfoMessage("You can not check in a guest whose arrival date is not equal to today.", "ERROR");
                    return;
                }
                else
                {
                    var osdBuffer = LocalBuffer.LocalBuffer.ObjectStateDefinitionBufferList.FirstOrDefault(osd => osd.Id == regExten.lastState);

                    if (regExten.lastState != CNETConstantes.CHECKED_IN_STATE)
                    {
                        if (regExten.NoOfRoom > 1)
                        {

                            DialogResult dr = MessageBox.Show(
                                @"This is multiple rooms check in.Do you want continue changing  " + (osdBuffer == null ? "Current" : osdBuffer.Description) +
                                " state to Check In ?", "State Change Conformation",
                                MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);
                            if (dr == DialogResult.Yes)
                            {
                                frmMultipleRoomCheckIn frmMultipleRoomCheckIn = new frmMultipleRoomCheckIn(true);
                                frmMultipleRoomCheckIn.SelectedHotelcode = SelectedHotelcode;
                                frmMultipleRoomCheckIn.RegExtension = regExten;
                                frmMultipleRoomCheckIn.ShowDialog();
                            }
                            else if (dr == DialogResult.Cancel)
                            {
                                SystemMessage.ShowModalInfoMessage("state change cancelled!", "MESSAGE");
                            }
                        }
                        else
                        {
                            DialogResult dr = MessageBox.Show(
                                "Do you want to change  " + (osdBuffer == null ? "Current" : osdBuffer.Description) + " state to Check In ?", "State Change Conformation",
                                MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);

                            if (dr == DialogResult.Yes)
                            {


                                frmCheckIn frmCheckIn = new frmCheckIn();
                                frmCheckIn.SelectedHotelcode = SelectedHotelcode;
                                frmCheckIn.RegExtension = regExten;
                                if (frmCheckIn.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                                {

                                    RefreshRegDocBrowser();
                                }
                            }
                            else if (dr == DialogResult.Cancel)
                            {
                                SystemMessage.ShowModalInfoMessage("state change cancelled!", "MESSAGE");
                            }
                        }


                    }
                    else
                    {
                        SystemMessage.ShowModalInfoMessage("The current state can not be changed to Check-In state",
                            "ERROR");
                    }
                }
            }
            else
            {
                SystemMessage.ShowModalInfoMessage("No registration selected!", "ERROR");
            }
        }

        private void bbiReinstate_ItemClick(object sender, ItemClickEventArgs e)
        {
            RegistrationListVMDTO regExten = (RegistrationListVMDTO)gridView_regDoc.GetFocusedRow();
            if (regExten != null)
            {

                string state = LocalBuffer.LocalBuffer.ObjectStateDefinitionBufferList.FirstOrDefault(x => x.Id == regExten.lastState) != null ? LocalBuffer.LocalBuffer.ObjectStateDefinitionBufferList.FirstOrDefault(x => x.Id == regExten.lastState).Description : string.Empty;
                if (regExten.lastState == CNETConstantes.ONLINE_CHECKED_OUT_STATE || regExten.lastState == CNETConstantes.CHECKED_OUT_STATE || regExten.lastState == CNETConstantes.OSD_CANCEL_STATE || regExten.lastState == CNETConstantes.CHECKED_IN_STATE)
                {
                    if (regExten.lastState == CNETConstantes.OSD_CANCEL_STATE && regExten.Arrival.Date < CurrentTime.Value.Date)
                    {
                        SystemMessage.ShowModalInfoMessage("You can't reinstate this registration. Arrival Date is less than the current date", "ERROR");
                        return;
                    }
                    else if ((regExten.lastState == CNETConstantes.ONLINE_CHECKED_OUT_STATE || regExten.lastState == CNETConstantes.CHECKED_OUT_STATE) && regExten.Departure.Date != CurrentTime.Value.Date)
                    {
                        SystemMessage.ShowModalInfoMessage("You can't reinstate this registration. Departure Date is less than the current date", "ERROR");
                        return;
                    }
                    DialogResult dr = MessageBox.Show("Do you want to reinstate  " + state + "?", "State Change Conformation", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);

                    if (dr == DialogResult.Yes)
                    {

                        frmReInState frmReInst = new frmReInState();
                        frmReInst.SelectedHotelcode = SelectedHotelcode;
                        frmReInst.RegExtension = regExten;
                        if (frmReInst.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {

                            RefreshRegDocBrowser();
                        }
                    }
                    else if (dr == DialogResult.Cancel)
                    {
                        SystemMessage.ShowModalInfoMessage("state change cancelled!", "MESSAGE");
                    }

                }
                else
                {
                    SystemMessage.ShowModalInfoMessage(state + " can not be changed to Check In in the Reinstate process!", "ERROR");
                }
            }
            else
            {
                SystemMessage.ShowModalInfoMessage("No registration selected!", "ERROR");
            }
        }

        private void bbiMessages_ItemClick(object sender, ItemClickEventArgs e)
        {
            RegistrationListVMDTO regExten = (RegistrationListVMDTO)gridView_regDoc.GetFocusedRow();
            //RegistrationListVMDTO regExten = (RegistrationListVMDTO)gridView_regDoc.GetRow(gridView_regDoc.GetSelectedRows()[0]);
            if (regExten != null)
            {
                frmMessaging frmMessaging = new frmMessaging();
                frmMessaging.SelectedHotelcode = SelectedHotelcode;
                frmMessaging.RegExt = regExten;
                frmMessaging.Show();

                //Generated_ID id = null;
                //string vCode = "";
                //id = UIProcessManager.IdGenerater("Voucher", device,
                //    CNETConstantes.MESSAGE.ToString(),
                //    CNETConstantes.VOUCHER_COMPONENET, 0);
                //if (id != null)
                //{
                //    vCode = id.GeneratedNewId;
                //}
                //else
                //{
                //    SystemMessage.ShowModalInfoMessage("There is a problem on id setting!!!", "ERROR");
                //    return;
                //}
                //// string vCode = UIProcessManager.GetCurrentIdByDevice("Voucher", device, CNETConstantes.MESSAGE.ToString(), CNETConstantes.VOUCHER_COMPONENET);
                //if (!string.IsNullOrEmpty(vCode))
                //{
                //    Voucher vo = UIProcessManager.SelectVoucher(regExten.Registration);
                //    if (vo == null)
                //    {
                //        SystemMessage.ShowModalInfoMessage("Error in getting voucher detail.", "ERROR");
                //        return;
                //    }
                //    frmPMSVouchercs frmMessage = new frmPMSVouchercs(@"Message");
                //    // frmMessage.Text += @"Message";
                //    frmMessage.loiPaymentType.Text = "Message Type";
                //    frmMessage.lciPurpose.Text = "Message";
                //    frmMessage.lciAmount.Visibility = LayoutVisibility.Never;
                //    // frmMessage.lciNumbers.Visibility = LayoutVisibility.Never;
                //    frmMessage.Voucher = vo;
                //    frmMessage.RegistrationExt = regExten;
                //    frmMessage.ShowDialog();
                //}
                //else
                //{
                //    SystemMessage.ShowModalInfoMessage("Please assign id setting for message voucher first!!!!", "ERROR");
                //}
            }
            else
            {
                SystemMessage.ShowModalInfoMessage("No registration selected!", "ERROR");
            }
        }

        private void bbiDepositReceipt_ItemClick(object sender, ItemClickEventArgs e)
        {
            RegistrationListVMDTO regExten = (RegistrationListVMDTO)gridView_regDoc.GetFocusedRow();
            //RegistrationListVMDTO regExten = (RegistrationListVMDTO)gridView_regDoc.GetRow(gridView_regDoc.GetSelectedRows()[0]);
            if (regExten != null)
            {

                frmPMSVouchercs frmCashReceipt = new frmPMSVouchercs(CNETConstantes.CASHRECIPT);
                frmCashReceipt.RegistrationExt = regExten;

                frmCashReceipt.ShowDialog();

            }
            else
            {
                SystemMessage.ShowModalInfoMessage("No registration selected!", "ERROR");
            }
        }

        private void bbiDebitNote_ItemClick(object sender, ItemClickEventArgs e)
        {
            RegistrationListVMDTO regExten = (RegistrationListVMDTO)gridView_regDoc.GetFocusedRow();
            //RegistrationListVMDTO regExten = (RegistrationListVMDTO)gridView_regDoc.GetRow(gridView_regDoc.GetSelectedRows()[0]);
            if (regExten != null)
            {

                frmPMSVouchercs frmVouchers = new frmPMSVouchercs(CNETConstantes.BANKDEBITNOTE);
                frmVouchers.RegistrationExt = regExten;

                frmVouchers.ShowDialog();
            }
            else
            {
                SystemMessage.ShowModalInfoMessage("No registration selected!", "ERROR");
            }
        }

        private void bbiCreditNote_ItemClick(object sender, ItemClickEventArgs e)
        {
            RegistrationListVMDTO regExten = (RegistrationListVMDTO)gridView_regDoc.GetFocusedRow();
            //RegistrationListVMDTO regExten = (RegistrationListVMDTO)gridView_regDoc.GetRow(gridView_regDoc.GetSelectedRows()[0]);
            if (regExten != null)
            {
                frmPMSVouchercs frmVochers = new frmPMSVouchercs(CNETConstantes.CREDIT_NOTE_VOUCHER);
                frmVochers.RegistrationExt = regExten;
                frmVochers.ShowDialog();
            }
            else
            {
                SystemMessage.ShowModalInfoMessage("No registration selected!", "ERROR");
            }
        }

        private void bbiRefund_ItemClick(object sender, ItemClickEventArgs e)
        {
            RegistrationListVMDTO regExten = (RegistrationListVMDTO)gridView_regDoc.GetFocusedRow();
            //RegistrationListVMDTO regExten = (RegistrationListVMDTO)gridView_regDoc.GetRow(gridView_regDoc.GetSelectedRows()[0]);
            if (regExten != null)
            {
                frmPMSVouchercs frmVouchers = new frmPMSVouchercs(CNETConstantes.REFUND);
                frmVouchers.RegistrationExt = regExten;
                frmVouchers.ShowDialog();
            }
            else
            {
                SystemMessage.ShowModalInfoMessage("No registration selected!", "ERROR");
            }
        }

        private void bbiRateAdjustment_ItemClick(object sender, ItemClickEventArgs e)
        {
            RegistrationListVMDTO regExten = (RegistrationListVMDTO)gridView_regDoc.GetFocusedRow();
            //RegistrationListVMDTO regExten = (RegistrationListVMDTO)gridView_regDoc.GetRow(gridView_regDoc.GetSelectedRows()[0]);
            if (regExten != null)
            {
                frmRateAdjustmnet frmRateAdjustmnet = new frmRateAdjustmnet();
                frmRateAdjustmnet.RegistrationExt = regExten;
                if (frmRateAdjustmnet.ShowDialog() == DialogResult.OK)
                {

                    //noting to do
                }
            }
            else
            {
                SystemMessage.ShowModalInfoMessage("No registration selected!", "ERROR");
            }
        }

        private void dpLogBook_Collapsed(object sender, DevExpress.XtraBars.Docking.DockPanelEventArgs e)
        {
            gc_logBook.DataSource = null;
            gc_logBook.RefreshDataSource();
            cardView1.RefreshData();
        }

        private void dpLogBook_Expanded(object sender, DevExpress.XtraBars.Docking.DockPanelEventArgs e)
        {
            RegistrationListVMDTO regVm = gridView_regDoc.GetFocusedRow() as RegistrationListVMDTO;
            if (regVm == null) return;
            if (regVm.LogMessages == null) return;

            PopulateLogBooks(regVm);
        }


        private void dpTravelDetail_Collapsed(object sender, DevExpress.XtraBars.Docking.DockPanelEventArgs e)
        {
            gcTravelDetail.DataSource = null;
            gcTravelDetail.RefreshDataSource();
            gvTravelDetail.RefreshData();
        }

        private void dpTravelDetail_Expanded(object sender, DevExpress.XtraBars.Docking.DockPanelEventArgs e)
        {
            RegistrationListVMDTO regVm = gridView_regDoc.GetFocusedRow() as RegistrationListVMDTO;
            if (regVm == null) return;
            if (regVm.TravelDetails == null) return;

            PopulateTravelDetails(regVm);
        }

        private void bbiRoomAssignment_ItemClick(object sender, ItemClickEventArgs e)
        {
            RegistrationListVMDTO regExten = (RegistrationListVMDTO)gridView_regDoc.GetFocusedRow();
            //RegistrationListVMDTO regExten = (RegistrationListVMDTO)gridView_regDoc.GetRow(gridView_regDoc.GetSelectedRows()[0]);
            if (regExten != null)
            {
                if (string.IsNullOrEmpty(regExten.Room))
                {
                    DialogResult dr = MessageBox.Show(
                        @"Do you want to assign a room to this guests ?",
                        @"Room Assignment",
                        MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);
                    if (dr == DialogResult.Yes)
                    {
                        frmMultipleRoomCheckIn frmMultipleRoomCheckIn = new frmMultipleRoomCheckIn(false);
                        frmMultipleRoomCheckIn.SelectedHotelcode = SelectedHotelcode;
                        frmMultipleRoomCheckIn.RegExtension = regExten;
                        frmMultipleRoomCheckIn.ShowDialog();
                    }
                    else if (dr == DialogResult.Cancel)
                    {
                        SystemMessage.ShowModalInfoMessage("Room Assignment cancelled!", "MESSAGE");
                    }
                }
                else if (regExten.Room.Contains("#"))
                {

                    // string state = UIProcessManager.SelectObjectStateDefinition(vo.LastObjectState) != null ? UIProcessManager.SelectObjectStateDefinition(vo.LastObjectState).description : string.Empty;
                    if (regExten.lastState != CNETConstantes.CHECKED_IN_STATE)
                    {
                        if (regExten.NoOfRoom >= 1)
                        {
                            DialogResult dr = MessageBox.Show(
                                @"This is multiple rooms reservation.Do you want continue assigning rooms to separate guests ?",
                                @"Room Assignment",
                                MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);
                            if (dr == DialogResult.Yes)
                            {
                                frmMultipleRoomCheckIn frmMultipleRoomCheckIn = new frmMultipleRoomCheckIn(false);
                                frmMultipleRoomCheckIn.SelectedHotelcode = SelectedHotelcode;
                                frmMultipleRoomCheckIn.RegExtension = regExten;
                                frmMultipleRoomCheckIn.ShowDialog();
                            }
                            else if (dr == DialogResult.Cancel)
                            {
                                SystemMessage.ShowModalInfoMessage("Room Assignment cancelled!", "MESSAGE");
                            }
                        }
                    }
                    else
                    {
                        SystemMessage.ShowModalInfoMessage("Invalid Operation!", "ERROR");
                    }
                }
                else
                {
                    SystemMessage.ShowModalInfoMessage("Room is already assigned to this registration", "ERROR");
                }
            }
            else
            {
                SystemMessage.ShowModalInfoMessage("No registration selected!", "ERROR");
            }
        }

        private void bbiPOSCharge_ItemClick(object sender, ItemClickEventArgs e)
        {
            RegistrationListVMDTO regExten = (RegistrationListVMDTO)gridView_regDoc.GetFocusedRow();
            //RegistrationListVMDTO regExten = (RegistrationListVMDTO)gridView_regDoc.GetRow(gridView_regDoc.GetSelectedRows()[0]);
            if (regExten != null)
            {
                frmManualCharge frmManualCharge = new frmManualCharge();
                frmManualCharge.RegExt = regExten;
                frmManualCharge.Show();

            }
        }

        private void bbiRoomCharges_ItemClick(object sender, ItemClickEventArgs e)
        {
            frmRoomCharge _frmRoomCharge = new frmRoomCharge();
            RegistrationListVMDTO regExten = (RegistrationListVMDTO)gridView_regDoc.GetFocusedRow();
            //RegistrationListVMDTO regExten = (RegistrationListVMDTO)gridView_regDoc.GetRow(gridView_regDoc.GetSelectedRows()[0]);
            if (regExten.Registration != null)
            {
                _frmRoomCharge.RegExtension = regExten;
                _frmRoomCharge.ShowDialog();
            }
            else
            {
                SystemMessage.ShowModalInfoMessage("No registration selected!", "ERROR");
            }
        }

        private void bbiPaidOut_ItemClick(object sender, ItemClickEventArgs e)
        {
            RegistrationListVMDTO regExten = (RegistrationListVMDTO)gridView_regDoc.GetFocusedRow();
            //RegistrationListVMDTO regExten = (RegistrationListVMDTO)gridView_regDoc.GetRow(gridView_regDoc.GetSelectedRows()[0]);
            if (regExten.Registration != null)
            {
                frmPMSVouchercs frmCashReceipt = new frmPMSVouchercs(CNETConstantes.PAID_OUT_VOUCHER);
                frmCashReceipt.RegistrationExt = regExten;
                frmCashReceipt.ShowDialog();
            }
            else
            {
                SystemMessage.ShowModalInfoMessage("No registration selected!", "ERROR");
            }
        }


        private void beiDeparture_EditValueChanged(object sender, EventArgs e)
        {
            PopulateSearchCriteria();
            if (isFromAdvanced) return;

            if (!string.IsNullOrEmpty(FilterKey)) return;

            //if (beiState.EditValue != null && beiDeparture.EditValue != null)
            //{

            //    if (beiArrival.EditValue != null)
            //    {
            //        LoadRegistrationDocument(beiState.EditValue.ToString(), (DateTime)beiArrival.EditValue, (DateTime)beiDeparture.EditValue);

            //    }
            //    else
            //    {
            //        LoadRegistrationDocument(beiState.EditValue.ToString(), null, (DateTime)beiDeparture.EditValue);
            //    }

            //    BindGridDataSource(regListVM);
            //}
        }

        private void beiArrival_EditValueChanged(object sender, EventArgs e)
        {
            PopulateSearchCriteria();
            if (isFromAdvanced) return;
            if (!string.IsNullOrEmpty(FilterKey)) return;

            //if (beiArrival.EditValue != null && beiState.EditValue != null)
            //{

            //    if (beiState.EditValue != null && beiArrival.EditValue != null)
            //    {

            //        if (beiDeparture.EditValue != null)
            //        {
            //            LoadRegistrationDocument(beiState.EditValue.ToString(), (DateTime)beiArrival.EditValue, (DateTime)beiDeparture.EditValue);

            //        }
            //        else
            //        {
            //            LoadRegistrationDocument(beiState.EditValue.ToString(), (DateTime)beiArrival.EditValue, null);
            //        }

            //        BindGridDataSource(regListVM);
            //    }


            //}
        }

        private void bbiArrivals_ItemClick(object sender, ItemClickEventArgs e)
        {
            isFromAdvanced = true;

            beiState.EditValue = 0;
            beiArrival.EditValue = CurrentTime.Value.Date;
            beiDeparture.EditValue = null;
            PopulateSearchCriteria("Arrivals");
            LoadRegistrationDocument(null, (DateTime?)beiArrival.EditValue, (DateTime?)beiDeparture.EditValue);
            List<RegistrationListVMDTO> filtered = null;
            if (regListVM != null)
            {
                filtered = regListVM.Where(r =>
                                r.lastState == CNETConstantes.SIX_PM_STATE || r.lastState == CNETConstantes.GAURANTED_STATE ||
                                r.lastState == CNETConstantes.OSD_WAITLIST_STATE).ToList();
            }


            BindGridDataSource(filtered);
        }

        private void bbiArrived_ItemClick(object sender, ItemClickEventArgs e)
        {
            isFromAdvanced = true;

            beiDeparture.EditValue = null;
            beiArrival.EditValue = CurrentTime;
            beiState.EditValue = CNETConstantes.CHECKED_IN_STATE;
            PopulateSearchCriteria("Arrived");
            List<RegistrationListVMDTO> filtered = null;
            if (CurrentTime != null)
            {
                LoadRegistrationDocument(CNETConstantes.CHECKED_IN_STATE, CurrentTime.Value, null);
            }

            if (filtered == null) return;
            BindGridDataSource(filtered);
        }

        private void bbiInHouse_ItemClick(object sender, ItemClickEventArgs e)
        {
            isFromAdvanced = true;

            beiDeparture.EditValue = null;
            beiArrival.EditValue = null;
            beiState.EditValue = CNETConstantes.CHECKED_IN_STATE;
            PopulateSearchCriteria("InHouse");
            RefreshRegDocBrowser();
        }

        private void bbiStayOver_ItemClick(object sender, ItemClickEventArgs e)
        {
            isFromAdvanced = true;

            beiDeparture.EditValue = null;
            beiArrival.EditValue = null;
            beiState.EditValue = CNETConstantes.CHECKED_IN_STATE;
            PopulateSearchCriteria("Stay Overs");
            List<RegistrationListVMDTO> filtered = null;
            if (CurrentTime != null)
            {
                LoadRegistrationDocument(CNETConstantes.CHECKED_IN_STATE, null, null);
                if (regListVM != null)
                {
                    filtered = regListVM.Where(r => r.Departure.Date > CurrentTime.Value.Date).ToList();
                }
            }

            if (filtered == null) return;
            BindGridDataSource(filtered);
        }

        private void bbiDueOut_ItemClick(object sender, ItemClickEventArgs e)
        {
            isFromAdvanced = true;

            beiDeparture.EditValue = CurrentTime;
            beiArrival.EditValue = null;
            beiState.EditValue = CNETConstantes.CHECKED_IN_STATE;
            PopulateSearchCriteria("Due Outs");
            RefreshRegDocBrowser();
        }

        private void bbiDeparted_ItemClick(object sender, ItemClickEventArgs e)
        {
            isFromAdvanced = true;

            beiArrival.EditValue = null;
            beiState.EditValue = CNETConstantes.CHECKED_OUT_STATE;
            PopulateSearchCriteria("Departed");
            RefreshRegDocBrowser();
        }

        private void bbiCompany_ItemClick(object sender, ItemClickEventArgs e)
        {
            beiMarket.Visibility = BarItemVisibility.Never;
            beiCompanySearch.Visibility = BarItemVisibility.Always;
            beiCompanySearch.Caption = "Company ";
            beiCompanySearch.Width = 100;

            PopulateCompanyLookup(CNETConstantes.CUSTOMER);
        }

        private void bbiAgent_ItemClick(object sender, ItemClickEventArgs e)
        {
            beiMarket.Visibility = BarItemVisibility.Never;
            beiCompanySearch.Visibility = BarItemVisibility.Always;
            beiCompanySearch.Caption = "       Agent";
            beiCompanySearch.Width = 100;
            PopulateCompanyLookup(CNETConstantes.AGENT);
        }

        private void bbiSource_ItemClick(object sender, ItemClickEventArgs e)
        {
            beiMarket.Visibility = BarItemVisibility.Never;
            beiCompanySearch.Visibility = BarItemVisibility.Always;
            beiCompanySearch.Caption = "     Source";
            beiCompanySearch.Width = 100;
            PopulateCompanyLookup(CNETConstantes.BUSINESSsOURCE);
        }

        private void bbiGroup_ItemClick(object sender, ItemClickEventArgs e)
        {
            beiMarket.Visibility = BarItemVisibility.Never;
            beiCompanySearch.Visibility = BarItemVisibility.Always;
            beiCompanySearch.Caption = "      Group";
            beiCompanySearch.Width = 100;
            PopulateCompanyLookup(CNETConstantes.GROUP);
        }

        private void bbiContact_ItemClick(object sender, ItemClickEventArgs e)
        {
            beiMarket.Visibility = BarItemVisibility.Never;
            beiCompanySearch.Visibility = BarItemVisibility.Always;
            beiCompanySearch.Caption = "   Contact";
            beiCompanySearch.Width = 100;
            PopulateCompanyLookup(CNETConstantes.CONTACT);
        }

        private void bbiMarket_ItemClick(object sender, ItemClickEventArgs e)
        {
            beiMarket.Visibility = BarItemVisibility.Always;
            beiMarket.Width = 100;
            beiMarket.Caption = "     Market";
            beiCompanySearch.Visibility = BarItemVisibility.Never;
        }

        private void bbiRegistrationNum_ItemClick(object sender, ItemClickEventArgs e)
        {
            beiMarket.Visibility = BarItemVisibility.Always;
            beiMarket.Caption = "   Reg. No";
            beiMarket.Width = 100;
            beiCompanySearch.Visibility = BarItemVisibility.Never;
        }

        private void bbiRebate_ItemClick(object sender, ItemClickEventArgs e)
        {
            RegistrationListVMDTO regExten = (RegistrationListVMDTO)gridView_regDoc.GetFocusedRow();
            //RegistrationListVMDTO regExten = (RegistrationListVMDTO)gridView_regDoc.GetRow(gridView_regDoc.GetSelectedRows()[0]);
            if (regExten.Registration != null)
            {
                frmPMSVouchercs frmBilling = new frmPMSVouchercs(CNETConstantes.CREDIT_NOTE_VOUCHER);
                frmBilling.RegistrationExt = regExten;
                frmBilling.ShowDialog();
            }
            else
            {
                SystemMessage.ShowModalInfoMessage("No registration selected!", "ERROR");
            }
        }

        private void gridView_regDoc_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            GridView view = sender as GridView;

            ////string regVoucher = gridView_regDoc.GetFocusedRowCellDisplayText("Registration");
            RegistrationListVMDTO regListVM = (RegistrationListVMDTO)view.GetRow(e.RowHandle);
            if (regListVM == null) return;

            //if (beiState.EditValue == "")
            //{
            //    SwitchControlsByState(regListVM.lastState);

            //}

            gridView_regDoc.Appearance.FocusedRow.ForeColor = ColorTranslator.FromHtml(regListVM.Color);
            gridView_regDoc.Appearance.FocusedCell.ForeColor = ColorTranslator.FromHtml(regListVM.Color);
            gridView_regDoc.Appearance.SelectedRow.ForeColor = ColorTranslator.FromHtml(regListVM.Color);

            e.Appearance.ForeColor = ColorTranslator.FromHtml(regListVM.Color);


            if (regListVM.LogMessages > 0)
            {
                e.Appearance.Font = new System.Drawing.Font(e.Appearance.Font.FontFamily, e.Appearance.Font.Size, FontStyle.Bold);
            }
            else
            {
                e.Appearance.Font = new System.Drawing.Font(e.Appearance.Font.FontFamily, e.Appearance.Font.Size, FontStyle.Regular);
            }

            if (e.Column.Caption == "SN")
            {
                //regListVM.SN = (e.RowHandle + 1).ToString();
                //e.DisplayText = (e.RowHandle + 1).ToString();
                List<short> addedImages = new List<short>();

                if (regListVM.IsMaster != null && regListVM.IsMaster.Value)
                {
                    addedImages.Add(6);
                }
                if (regListVM.WakeupCallCount > 0)
                {
                    addedImages.Add(0);
                }
                if (regListVM.TravelCount > 0)
                {
                    addedImages.Add(1);
                }
                if (regListVM.AccompanyCount > 0)
                {
                    addedImages.Add(2);
                }
                if (regListVM.LogMessages != null && regListVM.LogMessages > 0)
                {
                    addedImages.Add(3);
                }
                if (regListVM.ShareCount > 0)
                {
                    addedImages.Add(4);
                }
                if (regListVM.NoPost == true)
                {
                    addedImages.Add(5);
                }
                if (regListVM.IsCharged)
                {
                    addedImages.Add(8);
                }
                if (regListVM.ServiceRequests != null && regListVM.ServiceRequests > 0)
                {
                    addedImages.Add(9);
                }

                System.Drawing.Size imgSize = imageCollection1.ImageSize;
                Rectangle bounds = e.Bounds;
                bounds.Width = imgSize.Width;
                bounds.X = 10;

                foreach (var index in addedImages)
                {
                    e.Graphics.DrawImage(imageCollection1.Images[index], bounds);
                    bounds.X = bounds.Right + 2;
                    e.Graphics.Flush();
                }




                //var regList = grid_regDoc.DataSource as List<RegistrationListVM>;
                //if (regList != null && regList.Count > 0)
                //{
                //    int maxReg = regList.Max(r => r.WakeupCallCount + Math.Min(r.TravelCount, 1) + r.AccompanyCount + r.ShareCount + r.NoPostCount);
                //    e.Column.Width = maxReg * 16 + 12;
                //}

                e.Handled = true;


            }
        }

        void View_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            GridView item = sender as GridView;
            if (item == null) return;
            SystemConstantDTO osd = item.GetRow(e.RowHandle) as SystemConstantDTO;
            if (item != null)
            {
                if (e.Column.Caption == "Color")
                {
                    e.Appearance.ForeColor = ColorTranslator.FromHtml(osd.Value);
                }



            }
        }

        private void gridView_regDoc_RowStyle(object sender, RowStyleEventArgs e)
        {
            GridView view = sender as GridView;
            RegistrationListVMDTO regListVM = (RegistrationListVMDTO)view.GetRow(e.RowHandle);
            if (regListVM == null) return;
            gridView_regDoc.Appearance.FocusedRow.ForeColor = ColorTranslator.FromHtml(regListVM.Color);
            gridView_regDoc.Appearance.FocusedCell.ForeColor = ColorTranslator.FromHtml(regListVM.Color);
            gridView_regDoc.Appearance.SelectedRow.ForeColor = ColorTranslator.FromHtml(regListVM.Color);

            // e.Appearance.ForeColor = ColorTranslator.FromHtml(regListVM.Color);

            if (regListVM.LogMessages > 0)
            {
                e.Appearance.Font = new System.Drawing.Font(e.Appearance.Font.FontFamily, e.Appearance.Font.Size, FontStyle.Bold);
            }


            //   SecurityCheck(regListVM.lastState);



        }

        private void gridView_regDoc_DoubleClick(object sender, EventArgs e)
        {
            String text = "Folio";
            //List<String> allowedTabs = MasterPageForm.AllowedFunctionalities("Registration Document");
            //if (!MasterPageForm.checkExistance(allowedTabs, text))
            //{
            //    XtraMessageBox.Show("You Are Not Allowed For This Operation", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    return;
            //}
            GridView view = sender as GridView;
            _frmFolio = new frmFolio();
            RegistrationListVMDTO regExten = (RegistrationListVMDTO)view.GetFocusedRow();
            if (regExten != null)
            {
                _frmFolio.RegistrationExt = regExten;
                _frmFolio.ShowDialog(this);
            }
            else
            {
                SystemMessage.ShowModalInfoMessage("Please Select One registration", "ERROR");
            }
        }

        private void gridView_regDoc_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            GridView view = sender as GridView;

            //string regVoucher = gridView_regDoc.GetFocusedRowCellDisplayText("Registration");
            RegistrationListVMDTO regListVM = (RegistrationListVMDTO)view.GetFocusedRow();
            if (regListVM == null) return;

            if (beiState.EditValue == null || beiState.EditValue.ToString() == "")
            {
                SwitchControlsByState(regListVM.lastState);

            }


            gridView_regDoc.Appearance.FocusedRow.BackColor = ColorTranslator.FromHtml("SkyBlue");
            gridView_regDoc.Appearance.FocusedRow.ForeColor = ColorTranslator.FromHtml(regListVM.Color);


            gridView_regDoc.Appearance.FocusedCell.BackColor = ColorTranslator.FromHtml("SkyBlue");
            gridView_regDoc.Appearance.FocusedCell.ForeColor = ColorTranslator.FromHtml(regListVM.Color);

            gridView_regDoc.Appearance.SelectedRow.BackColor = ColorTranslator.FromHtml("SkyBlue");
            gridView_regDoc.Appearance.SelectedRow.ForeColor = ColorTranslator.FromHtml(regListVM.Color);

            if (regListVM.LogMessages > 0)
            {

                gridView_regDoc.Appearance.FocusedRow.Font = new System.Drawing.Font(gridView_regDoc.Appearance.FocusedRow.Font.FontFamily, gridView_regDoc.Appearance.FocusedRow.Font.Size, FontStyle.Bold);
                gridView_regDoc.Appearance.FocusedCell.Font = new System.Drawing.Font(gridView_regDoc.Appearance.FocusedCell.Font.FontFamily, gridView_regDoc.Appearance.FocusedCell.Font.Size, FontStyle.Bold);
                gridView_regDoc.Appearance.SelectedRow.Font = new System.Drawing.Font(gridView_regDoc.Appearance.SelectedRow.Font.FontFamily, gridView_regDoc.Appearance.SelectedRow.Font.Size, FontStyle.Bold);
            }

            if (regListVM.LogMessages != null && regListVM.LogMessages > 0)
            {

                dpLogBook.Text = string.Format("Log Book ({0})", regListVM.LogMessages);
            }
            else
            {
                dpLogBook.Text = "Log Book";
            }

            if (regListVM.TravelCount > 0)
            {
                dpTravelDetail.Text = string.Format("Travel Detail ({0})", regListVM.TravelCount);
            }
            else
            {
                dpTravelDetail.Text = "Travel Detail";
            }

            if (regListVM.ServiceRequests != null && regListVM.ServiceRequests > 0)
            {
                dpServiceReq.Text = string.Format("Service Request ({0})", regListVM.ServiceRequests);
            }
            else
            {
                dpServiceReq.Text = "Service Request";
            }

        }

        private void bbiSF_print_ItemClick(object sender, ItemClickEventArgs e)
        {
            RegistrationListVMDTO regListVM = gridView_regDoc.GetFocusedRow() as RegistrationListVMDTO;
            GuestFolio report = new GuestFolio(gc_folioSidePane);
            report.FolioParametres(regListVM.Guest, regListVM.Registration, "", "", "", regListVM.Arrival.ToShortDateString(), regListVM.Departure.ToShortDateString());
            // report.Watermark.Image = DocumentPrint.Resource1.NonFisical;
            report.Watermark.ImageAlign = ContentAlignment.MiddleCenter;
            report.Watermark.ImageTiling = false;

            //  report.Watermark.ImageViewMode = DevExpress.XtraPrinting.Drawing.ImageViewMode.;
            report.Watermark.ImageTransparency = 70;

            report.Watermark.PageRange = "all";
            report.Watermark.ShowBehind = true;

            ReportPrintTool pt = new ReportPrintTool(report);
            pt.ShowPreview();
        }

        private void beiMarket_EditValueChanged(object sender, EventArgs e)
        {
            if (beiMarket.Caption.Contains("Reg. No"))
            {
                PopulateSearchCriteria("Registration");
                if (beiMarket.EditValue != null && !string.IsNullOrEmpty(beiMarket.EditValue.ToString()))
                    FilterByRegNumber(beiMarket.EditValue.ToString());
            }
            else
            {
                PopulateSearchCriteria("Market");
            }


        }

        private void bbiRegCard_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {

                RegistrationListVMDTO regListVM = gridView_regDoc.GetFocusedRow() as RegistrationListVMDTO;
                if (regListVM != null)
                {
                    Progress_Reporter.Show_Progress("Generating attachment print preview", "Please Wait......");
                    reportGenerator = new ReportGenerator();
                    reportGenerator.GetPMSRegistrationCard(regListVM.Id);
                    Progress_Reporter.Close_Progress();
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Error in generating print preview. DETAIL:: " + ex.Message, "CNET_v2016", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void gv_regListActivity_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            //if (e.Column.Caption == "SN")
            //{
            //    e.DisplayText = (e.RowHandle + 1).ToString();
            //}
        }

        private void bbiConformationCard_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                RegistrationListVMDTO regListVM = gridView_regDoc.GetFocusedRow() as RegistrationListVMDTO;
                if (regListVM != null)
                {
                    Progress_Reporter.Show_Progress("Generating attachment print preview", "Please Wait......");
                    ReportGenerator reportGenerator = new ReportGenerator();
                    reportGenerator.GetPMSConformationAttachement(regListVM.Id, LocalBuffer.LocalBuffer.CurrentLoggedInUserEmployeeName);

                    Progress_Reporter.Close_Progress();
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Error in generating print preview. DETAIL:: " + ex.Message, "CNET_v2016", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void bbiPrint_ItemClick(object sender, ItemClickEventArgs e)
        {
            ReportGenerator rpg = new ReportGenerator();

            rpg.GenerateGridReport(grid_regDoc, "Registrations List", CurrentTime.Value.ToShortDateString(), "Search Criteria: " + beiSearchCriteria.EditValue == null ? "" : beiSearchCriteria.EditValue.ToString());

        }

        private void bbiSetAlarm_ItemClick(object sender, ItemClickEventArgs e)
        {
            RegistrationListVMDTO regListVM = gridView_regDoc.GetFocusedRow() as RegistrationListVMDTO;
            if (regListVM != null)
            {
                frmWakeupCall frmSetAlarm = new frmWakeupCall();
                frmSetAlarm.RegExt = regListVM;
                frmSetAlarm.Show();

            }
        }

        private void bbiPostMasters_ItemClick(object sender, ItemClickEventArgs e)
        {
            List<int> pseudoRooms = null;

            List<RoomTypeDTO> pseudoRoomList = UIProcessManager.GetRoomTypeByispseudoRoomType(true);
            if (pseudoRoomList != null)
                pseudoRooms = pseudoRoomList.Select(p => p.Id).ToList();

            if (pseudoRooms == null || pseudoRooms.Count == 0)
            {
                XtraMessageBox.Show("Unable to get Pseudo Rooms", "Post Master", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            LoadRegistrationDocument(CNETConstantes.CHECKED_IN_STATE, (DateTime?)beiArrival.EditValue, (DateTime?)beiDeparture.EditValue);
            List<RegistrationListVMDTO> filtered = null;
            if (regListVM != null)
            {
                filtered = regListVM.Where(r => pseudoRooms.Contains(r.RoomType.Value)).ToList();
            }


            BindGridDataSource(filtered);
        }

        private void bbiBillSplit_ItemClick(object sender, ItemClickEventArgs e)
        {
            RegistrationListVMDTO regExten = (RegistrationListVMDTO)gridView_regDoc.GetFocusedRow();
            //RegistrationListVMDTO regExten = (RegistrationListVMDTO)gridView_regDoc.GetRow(gridView_regDoc.GetSelectedRows()[0]);
            frmSplitBills frmSplitBills = new frmSplitBills();
            frmSplitBills.SelectedHotelcode = SelectedHotelcode;
            frmSplitBills.RegExt = regExten;
            if (frmSplitBills.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                VoucherDTO vo = UIProcessManager.GetVoucherById(regExten.Id);
                if (vo != null && vo.LastState == CNETConstantes.CHECKED_OUT_STATE)
                {
                    RefreshRegDocBrowser();
                }
            }
        }

        private void bbiBillTransfer_ItemClick(object sender, ItemClickEventArgs e)
        {
            RegistrationListVMDTO regExten = (RegistrationListVMDTO)gridView_regDoc.GetFocusedRow();

            //RegistrationListVMDTO regExten = (RegistrationListVMDTO)gridView_regDoc.GetRow(gridView_regDoc.GetSelectedRows()[0]);
            if (regExten != null)
            {
                frmFolio frmFolio = new frmFolio();
                frmFolio.SelectedHotelcode = SelectedHotelcode;
                frmFolio.RegistrationExt = regExten;
                frmFolio.IsBillTransfer = true;


                frmFolio.ShowDialog(this);
            }
            else
            {
                SystemMessage.ShowModalInfoMessage("Please Select One registration", "ERROR");
            }
        }

        private void RegistrationList_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(FilterKey))
            {
                PopulateByDasboardFilterKey();
            }
        }

        private void bbiClearFilter_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                rpgDateRange.Visible = false;
                beiRangeBy.EditValue = "";
                if (CurrentTime != null)
                {
                    beiStartDate.EditValue = CurrentTime.Value;
                    beiEndDate.EditValue = CurrentTime.Value;
                }
                beiRoom.EditValue = null;
                beiGuest.EditValue = null;
                beiState.EditValue = CNETConstantes.CHECKED_IN_STATE;
                beiArrival.EditValue = null;
                beiDeparture.EditValue = null;
                beiCompanySearch.EditValue = null;
                beiMarket.EditValue = null;

                PopulateSearchCriteria();
            }
            catch (Exception) { }
        }

        #endregion


        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();

                _frmTravelDetail = null;
                _frmRateSummery = null;
                _frmReconciliation = null;
                _frmRoomMove = null;
                _frmAccompanyingGuest = null;
                _frmProfileAmendment = null;
                _frmRegistrationDateAmendement = null;
                _frmDailyRateDetail = null;
                _frmFolio = null;
                _frmRegistrationDetail = null;

                currentUser = null;
                device = null;

                _currentTime = null;
                _regDocList = null;
                regListVM = null;
                //_personList = null; 

                FilterKey = null;
            }
            base.Dispose(disposing);
        }

        private void bbiOther_ItemClick(object sender, ItemClickEventArgs e)
        {
            RegistrationListVMDTO regExten = (RegistrationListVMDTO)gridView_regDoc.GetFocusedRow();
            //RegistrationListVMDTO regExten = (RegistrationListVMDTO)gridView_regDoc.GetRow(gridView_regDoc.GetSelectedRows()[0]);
            if (regExten != null)
            {
                frmOtherFields frmOtherFields = new frmOtherFields();
                frmOtherFields.RegExt = regExten;
                frmOtherFields.ShowDialog();
            }
            else
            {
                SystemMessage.ShowModalInfoMessage("Please Select One registration", "ERROR");
            }
        }

        private void bbiPaymentOptions_ItemClick(object sender, ItemClickEventArgs e)
        {
            RegistrationListVMDTO regExten = (RegistrationListVMDTO)gridView_regDoc.GetFocusedRow();
            //RegistrationListVMDTO regExten = (RegistrationListVMDTO)gridView_regDoc.GetRow(gridView_regDoc.GetSelectedRows()[0]);
            if (regExten != null)
            {
                frmPaymentOptions frmPaymentOptions = new frmPaymentOptions();
                frmPaymentOptions.RegExt = regExten;
                if (frmPaymentOptions.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    RefreshRegDocBrowser();
                }
            }
            else
            {
                SystemMessage.ShowModalInfoMessage("Please Select One registration", "ERROR");
            }
        }

        private void bbiProformaFolio_ItemClick(object sender, ItemClickEventArgs e)
        {
            RegistrationListVMDTO regExten = (RegistrationListVMDTO)gridView_regDoc.GetFocusedRow();
            if (regExten != null)
            {
                frmProformaFolio profFolio = new frmProformaFolio();
                profFolio.RegExt = regExten;
                profFolio.ShowDialog();

            }
            else
            {
                SystemMessage.ShowModalInfoMessage("Please Select One registration", "ERROR");
            }
            //RegistrationListVMDTO regExten = (RegistrationListVMDTO)gridView_regDoc.GetRow(gridView_regDoc.GetSelectedRows()[0]);

        }

        private void bbiAttachment_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                //RegistrationListVMDTO regExten = (RegistrationListVMDTO)gridView_regDoc.GetRow(gridView_regDoc.GetSelectedRows()[0]);
                //frmAttachment frmAttachment = new frmAttachment();
                //frmAttachment.RegExt = regExten;
                //frmAttachment.ShowDialog();
            }
            catch (Exception ex)
            {

            }
        }

        private void cardView1_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {

        }

        private void bbiIssueCard_ItemClick(object sender, ItemClickEventArgs e)
        {
            RegistrationListVMDTO regExten = (RegistrationListVMDTO)gridView_regDoc.GetFocusedRow();
            //RegistrationListVMDTO regExten = (RegistrationListVMDTO)gridView_regDoc.GetRow(gridView_regDoc.GetSelectedRows()[0]);
            if (regExten != null)
            {
                frmDoorLock frmDoorLock = new frmDoorLock();
                frmDoorLock.RegExt = regExten;
                frmDoorLock.ShowDialog();
            }
            else
            {
                SystemMessage.ShowModalInfoMessage("Please Select One registration", "ERROR");
            }
        }

        private void bbiSync_ItemClick(object sender, ItemClickEventArgs e)
        {
            //RegistrationListVMDTO regExten = (RegistrationListVMDTO)gridView_regDoc.GetRow(gridView_regDoc.GetSelectedRows()[0]);
            //if (regExten == null) return;

            //CommonLogics.Synchronize(regExten.Registration, CNETConstantes.REGISTRATION_VOUCHER.ToString(), CNETConstantes.VOUCHER_COMPONENET, true);
        }



        private void bbiPrintPocketCard_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                var dto = gvTravelDetail.GetFocusedRow() as TravelDetailVM;
                if (dto != null)
                {
                    var selected = dto.TravelDetail;
                    if (selected != null)
                    {
                        //PickupObjectList pickup = new PickupObjectList()
                        //{
                        //    Carrer = selected.carrier,
                        //    Guest = selected.name,
                        //    Company = selected.tradeName,
                        //    TransportationNo = selected.transportationNo,

                        //};

                        //ReportGenerator rg = new ReportGenerator();

                        //rg.PrintPickUp(pickup);
                    }
                    else
                    {
                        SystemMessage.ShowModalInfoMessage("Please select travel detail first.", "ERROR");

                    }
                }
                else
                {
                    SystemMessage.ShowModalInfoMessage("Please select travel detail first.", "ERROR");
                }
            }
            catch (Exception ex)
            {
                SystemMessage.ShowModalInfoMessage("Error in printing pick-up card. DETAIL:: " + ex.Message, "ERROR");
            }
        }

        private void bbiFooterButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            var btn = e.Item;
            if (btn == null) return;
            List<RegistrationListVMDTO> filteredList = new List<RegistrationListVMDTO>();


            if (btn.Name == bbiFooterNoPost.Name)
            {
                filteredList = regListVM.Where(r => r.NoPost == true).ToList();
            }
            else if (btn.Name == bbiFooterTravelDetail.Name)
            {
                filteredList = regListVM.Where(r => r.TravelCount > 0).ToList();
            }
            else if (btn.Name == bbiFooterAccompanyGuest.Name)
            {
                filteredList = regListVM.Where(r => r.AccompanyCount > 0).ToList();
            }
            else if (btn.Name == bbiFooterShare.Name)
            {
                filteredList = regListVM.Where(r => r.ShareCount > 0).ToList();
            }
            else if (btn.Name == bbiFooterLogBook.Name)
            {

                filteredList = regListVM.Where(r => r.LogMessages != null && r.LogMessages > 0).ToList();
            }
            else if (btn.Name == bbiFooterWakeupCall.Name)
            {
                filteredList = regListVM.Where(r => r.WakeupCallCount > 0).ToList();
            }
            else if (btn.Name == bbiFooterMasterGuest.Name)
            {
                filteredList = regListVM.Where(r => r.IsMaster.Value).ToList();
            }
            else if (btn.Name == bbiFooterDueOuts.Name)
            {
                filteredList = regListVM.Where(r => r.IsDueout).ToList();
            }
            else if (btn.Name == bbiFooterChargedRooms.Name)
            {
                filteredList = regListVM.Where(r => r.IsCharged).ToList();
            }
            else if (btn.Name == bbiFooter6PMCheckin.Name)
            {
                beiState.EditValue = CNETConstantes.SIX_PM_STATE;
                LoadRegistrationDocument(CNETConstantes.SIX_PM_STATE, (DateTime?)beiArrival.EditValue, (DateTime?)beiDeparture.EditValue);
                BindGridDataSource(regListVM);
                // filteredList = regListVM.Where(r => r.IsCharged).ToList();
            }
            if (btn.Name != bbiFooter6PMCheckin.Name)
            {
                BindGridDataSource(filteredList);
            }
        }

        private void bbiGroupCheckout_ItemClick(object sender, ItemClickEventArgs e)
        {
            PMSDataLogger.LogMessage("frmRegistrationList", "Group Checkout Clicked");
            RegistrationListVMDTO regExten = (RegistrationListVMDTO)gridView_regDoc.GetFocusedRow();
            //RegistrationListVMDTO regExten = (RegistrationListVMDTO)gridView_regDoc.GetRow(gridView_regDoc.GetSelectedRows()[0]);
            if (regExten != null)
            {
                frmGroupCheckout frmGroupCheckout = new frmGroupCheckout();
                frmGroupCheckout.SelectedHotelcode = SelectedHotelcode;
                frmGroupCheckout.RegExtension = regExten;
                if (frmGroupCheckout.ShowDialog() == DialogResult.OK)
                {
                    RefreshRegDocBrowser();
                }
            }
            else
            {
                SystemMessage.ShowModalInfoMessage("Please Select One registration", "ERROR");
            }
        }
        ReportGenerator reportGenerator { get; set; }
        private void bbiShowOnTab_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {

                RegistrationListVMDTO regListVM = gridView_regDoc.GetFocusedRow() as RegistrationListVMDTO;
                if (regListVM != null)
                {
                    Progress_Reporter.Show_Progress("Generating attachment print preview", "Please Wait......");
                    //btnCloseTabView.Visibility = BarItemVisibility.Always;
                    //Task.Run(() => ShowRegistrationOnTab(regListVM.Id));
                    ShowRegistrationOnTab(regListVM.Id);
                    Progress_Reporter.Close_Progress();
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Error in generating print preview. DETAIL:: " + ex.Message, "CNET_v2016", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        public void ShowRegistrationOnTab(int Id)
        {
            MasterPageForm.PauseVideo();
            reportGenerator = new ReportGenerator();
            reportGenerator.GetPMSRegistrationCard(Id, true);
            Progress_Reporter.Close_Progress();
            MasterPageForm.PlayVideo();
        }

        private void beiDateRange_ItemClick(object sender, ItemClickEventArgs e)
        {
            rpgDateRange.Visible = true;
        }

        private void RepoZoomTrack_EditValueChanged(object sender, EventArgs e)
        {
            ZoomTrackBarControl control = sender as ZoomTrackBarControl;
            System.Drawing.Font font = gridView_regDoc.Appearance.Row.Font;

            gridView_regDoc.Appearance.Row.Font = new System.Drawing.Font(font.FontFamily, control.Value, font.Style);


            double zoomValue = Math.Round((control.Value / 9.0) * 100, 2);
            beiZoomTrack.Caption = string.Format("{0}%", zoomValue);

        }

        private void bbiServiceRequest_ItemClick(object sender, ItemClickEventArgs e)
        {
            RegistrationListVMDTO regListVM = gridView_regDoc.GetFocusedRow() as RegistrationListVMDTO;
            frmServiceRequest serReqForm = new frmServiceRequest();
            serReqForm.RegistrationEX = regListVM;
            serReqForm.ShowDialog();
        }

        private void dpServiceReq_Collapsed(object sender, DevExpress.XtraBars.Docking.DockPanelEventArgs e)
        {
            gcServReq.DataSource = null;
            gvServReq.RefreshData();
        }

        private void dpServiceReq_Expanded(object sender, DevExpress.XtraBars.Docking.DockPanelEventArgs e)
        {
            gcServReq.DataSource = null;
            gvServReq.RefreshData();
            RegistrationListVMDTO regVm = gridView_regDoc.GetFocusedRow() as RegistrationListVMDTO;
            if (regVm == null) return;
            if (regVm.ServiceRequests != null && regVm.ServiceRequests > 0)
            {
                List<VwServiceRequestDTO> servicerequest = UIProcessManager.GetServiceRequestByRegistrationId(regVm.Id);

                gcServReq.DataSource = servicerequest;
                gvServReq.RefreshData();

                for (int i = 0; i < regVm.ServiceRequests; i++)
                {
                    gvServReq.ExpandMasterRow(i);
                }
            }
            else
            {

                gcServReq.DataSource = null;
                gvServReq.RefreshData();
            }


        }

        private void dpActivity_Expanded(object sender, DevExpress.XtraBars.Docking.DockPanelEventArgs e)
        {

            gc_regListActivity.DataSource = null;

            RegistrationListVMDTO regExten = (RegistrationListVMDTO)gridView_regDoc.GetFocusedRow();
            //RegistrationListVMDTO regExten = (RegistrationListVMDTO)gridView_regDoc.GetRow(gridView_regDoc.GetSelectedRows()[0]);
            if (regExten != null)
            {
                GetandpopulateActivity(regExten);
            }
            else
            {
                SystemMessage.ShowModalInfoMessage("No registration selected!", "ERROR");
            }
        }

        private void GetandpopulateActivity(RegistrationListVMDTO regExten)
        {
            gv_regListActivity.ViewCaption = "Activities on " + regExten.Registration + " (Room - " + regExten.Room + ")";
            gc_regListActivity.DataSource = null;
            List<ActivityVM> dtoList = new List<ActivityVM>();
            List<VwActivityDetailViewDTO> activityList = UIProcessManager.GetActivityDetailView(regExten.Id, null, null);
            if (activityList != null && activityList.Count > 0)
            {
                activityList = activityList.OrderBy(x => x.TimeStamp).ToList();
                int count = 0;

                //string Activitydefdesc = LocalBuffer.LocalBuffer.SystemConstantDTOBufferList.FirstOrDefault();

                foreach (var act in activityList)
                {
                    ActivityVM dto = new ActivityVM();
                    count++;
                    dto.SN = count;
                    dto.Workflow = act.ActivitiyDefinition_Desc + " by " + act.UserName;
                    dto.DateAndDevice = string.Format("[{0:G}][{1}]", act.TimeStamp, act.DeviceName);
                    dto.Remark = act.Remark;
                    dtoList.Add(dto);
                }
            }
            gc_regListActivity.DataSource = dtoList;
            gc_regListActivity.RefreshDataSource();
            gv_regListActivity.RefreshData();
        }

        private void dpActivity_Collapsed(object sender, DevExpress.XtraBars.Docking.DockPanelEventArgs e)
        {
            dpActivity.Text = @"Activities";
            gc_regListActivity.DataSource = null;
            gc_regListActivity.RefreshDataSource();
        }

        private void bbiLostAndFound_ItemClick(object sender, ItemClickEventArgs e)
        {
            frmLostAndFound frmLandF = new frmLostAndFound();
            frmLandF.ShowDialog();
        }

        private void grid_regDoc_Click(object sender, EventArgs e)
        {
            if (dpActivity.Visibility == DevExpress.XtraBars.Docking.DockVisibility.Visible)
            {
                RegistrationListVMDTO regExten = (RegistrationListVMDTO)gridView_regDoc.GetFocusedRow();
                if (regExten != null)
                {
                    GetandpopulateActivity(regExten);
                }
            }
            if (dpLogBook.Visibility == DevExpress.XtraBars.Docking.DockVisibility.Visible)
            {
                RegistrationListVMDTO regVm = gridView_regDoc.GetFocusedRow() as RegistrationListVMDTO;
                if (regVm == null) return;

                PopulateLogBooks(regVm);
            }
            if (dpTravelDetail.Visibility == DevExpress.XtraBars.Docking.DockVisibility.Visible)
            {
                RegistrationListVMDTO regVm = gridView_regDoc.GetFocusedRow() as RegistrationListVMDTO;
                if (regVm == null) return;

                PopulateTravelDetails(regVm);
            }
            if (dpServiceReq.Visibility == DevExpress.XtraBars.Docking.DockVisibility.Visible)
            {
                gcServReq.DataSource = null;
                gvServReq.RefreshData();
                RegistrationListVMDTO regVm = gridView_regDoc.GetFocusedRow() as RegistrationListVMDTO;
                if (regVm == null) return;
                if (regVm.ServiceRequests == null || regVm.ServiceRequests == 0) return;

                gcServReq.DataSource = regVm.ServiceRequests;
                gvServReq.RefreshData();

                for (int i = 0; i < regVm.ServiceRequests; i++)
                {
                    gvServReq.ExpandMasterRow(i);
                }
            }

            if (dpCustomerProfile.Visibility == DevExpress.XtraBars.Docking.DockVisibility.Visible)
            {
                gcConsigneeDetail.DataSource = null;

                RegistrationListVMDTO regExten = (RegistrationListVMDTO)gridView_regDoc.GetFocusedRow();
                //RegistrationListVMDTO regExten = (RegistrationListVMDTO)gridView_regDoc.GetRow(gridView_regDoc.GetSelectedRows()[0]);
                if (regExten != null)
                {
                    GetandCustomerProfile(regExten);
                }
                else
                {
                    SystemMessage.ShowModalInfoMessage("No registration selected!", "ERROR");
                }
            }
        }

        private void dpCustomerProfile_Expanded(object sender, DevExpress.XtraBars.Docking.DockPanelEventArgs e)
        {
            gcConsigneeDetail.DataSource = null;

            RegistrationListVMDTO regExten = (RegistrationListVMDTO)gridView_regDoc.GetFocusedRow();
            //RegistrationListVMDTO regExten = (RegistrationListVMDTO)gridView_regDoc.GetRow(gridView_regDoc.GetSelectedRows()[0]);
            if (regExten != null)
            {
                GetandCustomerProfile(regExten);
            }
            else
            {
                SystemMessage.ShowModalInfoMessage("No registration selected!", "ERROR");
            }
        }

        private void GetandCustomerProfile(RegistrationListVMDTO regExten)
        {
            gvConsigneeDetail.ViewCaption = "Customer Profile on " + regExten.Registration + " (Room - " + regExten.Room + ")";
            gcConsigneeDetail.DataSource = null;
            List<RightCustomer_view> listOfRightView = new List<RightCustomer_view>();

            listOfRightView.AddRange(GetAndPopulateConsigneeInfo(regExten.GuestId.Value, "Main Consignee: - Guest"));

            if (regExten.CompanyId != null)
                listOfRightView.AddRange(GetAndPopulateConsigneeInfo(regExten.CompanyId.Value, "Other Consignee: - Company"));

            if (regExten.AgentId != null)
                listOfRightView.AddRange(GetAndPopulateConsigneeInfo(regExten.AgentId.Value, "Other Consignee: - Agent"));

            if (regExten.ContactId != null)
                listOfRightView.AddRange(GetAndPopulateConsigneeInfo(regExten.ContactId.Value, "Other Consignee: - Contact"));

            if (regExten.GroupId != null)
                listOfRightView.AddRange(GetAndPopulateConsigneeInfo(regExten.CompanyId.Value, "Other GroupId: - Group"));

            if (regExten.SourceId != null)
                listOfRightView.AddRange(GetAndPopulateConsigneeInfo(regExten.SourceId.Value, "Other Consignee: - Source"));



            gcConsigneeDetail.DataSource = listOfRightView;
            gcConsigneeDetail.RefreshDataSource();
            gvConsigneeDetail.RefreshData();
        }

        public List<RightCustomer_view> GetAndPopulateConsigneeInfo(int consigneeId, string Type)
        {
            ConsigneeBuffer GuestConsgineeBuffer = UIProcessManager.GetConsigneeBufferById(consigneeId);

            List<RightCustomer_view> listOfRightView = new List<RightCustomer_view>();
            RightCustomer_view rv = new RightCustomer_view();
            //Consignee Name 
            rv = new RightCustomer_view();
            rv.Detail = Type;// "Main Consignee:- Guest";
            rv.RightValue = "Name";
            rv.RightKey = GuestConsgineeBuffer.consignee.FirstName + " " + GuestConsgineeBuffer.consignee.SecondName;
            listOfRightView.Add(rv);

            rv = new RightCustomer_view();
            rv.Detail = Type;//"Main Consignee:- Guest";
            rv.RightValue = "TIN";
            rv.RightKey = GuestConsgineeBuffer.consignee.Tin;
            listOfRightView.Add(rv);


            ConsigneeDTO Guestdata = LocalBuffer.LocalBuffer.AllGuestConsigneeViewlist.FirstOrDefault(x => x.Id == consigneeId);
            if (Guestdata != null)
            {
                //Consignee Identification 
                rv = new RightCustomer_view();
                rv.Detail = Type;//"Main Consignee:- Guest";
                rv.RightValue = "Passport Id";
                rv.RightKey = Guestdata.PassportId;
                listOfRightView.Add(rv);
            }



            //Consignee Identification 
            //List<IdentificationDTO> GuestIdentification = LocalBuffer.LocalBuffer.IdentificationBufferList.Where(x => x.Consignee == consigneeId).ToList();
            //foreach (var GuestIden in GuestIdentification)
            //{
            //    rv = new RightCustomer_view();
            //    rv.Detail = Type;//"Main Consignee:- Guest";
            //    rv.RightValue = GuestIden.Description;
            //    rv.RightKey = GuestIden.IdNumber;
            //    listOfRightView.Add(rv);

            //}
            //Consignee Address  
            if (GuestConsgineeBuffer != null && GuestConsgineeBuffer.consigneeUnits != null && GuestConsgineeBuffer.consigneeUnits.ToList().Count > 0)
            {
                foreach (ConsigneeUnitDTO GuestIden in GuestConsgineeBuffer.consigneeUnits)
                {
                    rv = new RightCustomer_view();
                    rv.Detail = Type;//"Main Consignee:- Guest";
                    rv.RightValue = "Branch";
                    rv.RightKey = GuestIden.Name;
                    listOfRightView.Add(rv);


                    rv = new RightCustomer_view();
                    rv.Detail = Type;// "Main Consignee:- Guest";
                    rv.RightValue = "Address";
                    rv.RightKey = GuestIden.Phone1;
                    listOfRightView.Add(rv);

                }

            }
            return listOfRightView;
        }


        private void bbiPrintpackagecoupon_ItemClick(object sender, ItemClickEventArgs e)
        {
            DateTime? date = UIProcessManager.GetServiceTime();

            RegistrationListVMDTO regExten = (RegistrationListVMDTO)gridView_regDoc.GetFocusedRow();
            //RegistrationListVMDTO regExten = (RegistrationListVMDTO)gridView_regDoc.GetRow(gridView_regDoc.GetSelectedRows()[0]);
            if (regExten.Registration != null && regExten.RateCodeHeader != null)
            {
                List<VwPackageToPostViewDTO> AllPackageList = UIProcessManager.GetPostingPackageToPostViewByRegistrationCode(regExten.Id);

                if (AllPackageList != null && AllPackageList.Count > 0)
                {
                    frmRatePackagePrint package = new frmRatePackagePrint(regExten, AllPackageList);
                    package.ShowDialog();
                }
                else
                {
                    SystemMessage.ShowModalInfoMessage("There are no Packages for this registration " + regExten.Registration, "ERROR");
                }
            }
            else
            {
                SystemMessage.ShowModalInfoMessage("No registration selected!", "ERROR");
            }
        }

        private void beiHotel_EditValueChanged(object sender, EventArgs e)
        {

            SelectedHotelcode = beiHotel.EditValue == null ? 0 : Convert.ToInt32(beiHotel.EditValue);
            if (!Initalizing)
                bbiRefresh.PerformClick();
        }

        private void btnCloseTabView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (reportGenerator != null)
            {
                Invoke(new Action(() =>
                {
                    reportGenerator.CloseTabViewer();
                }));
                btnCloseTabView.Visibility = BarItemVisibility.Never;
            }
        }

        public int SelectedHotelcode { get; set; }
    }
}
public class RightCustomer_view
{
    public string Reference { get; set; }
    public string RightKey { get; set; }
    public string RightValue { get; set; }
    public string Detail { get; set; }
}
namespace Extensions
{
    public static class Extension
    {
        public static object GetRowByKeyValue(this RepositoryItemSearchLookUpEdit edit, object key)
        {
            var listSource = edit.DataSource as IListSource;
            IList list;
            if (listSource != null)
            {
                list = listSource.GetList();
            }
            else
            {
                list = edit.DataSource as IList;
            }
            var rowHandle = edit.GetIndexByKeyValue(key);
            var value = list[rowHandle];
            return value;
        }
    }
}