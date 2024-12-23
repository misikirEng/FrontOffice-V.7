using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using CNET.ERP.Client.Common.UI;
using CNET.FrontOffice_V._7; 
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraBars;
using CNET.FrontOffice_V._7.Forms.Non_Navigatable_Modals;
using CNET.FrontOffice_V._7.Forms.State_Change;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
using System.Text;
using System.IO; 
using DevExpress.XtraWizard;
using System.Threading;
using System.Threading.Tasks;
using DevExpress.XtraTreeList.Columns;
using System.Drawing.Printing;
using DevExpress.XtraPrinting.Export.Pdf;
using DevExpress.XtraGrid;
using PMSReport;
using CNET.FrontOffice_V._7.PMS.Common_Classes;
using CNET.FrontOffice_V._7.APICommunication;
using CNET_V7_Domain.Domain.SettingSchema;
using CNET.FrontOffice_V._7.PMS.DTO;
using CNET_V7_Domain.Domain.PmsSchema;
using CNET_V7_Domain.Domain.ArticleSchema;
using CNET_V7_Domain.Domain.SecuritySchema;
using CNET_V7_Domain.Misc.PmsDTO;
using CNET_V7_Domain.Domain.CommonSchema;
using CNET_V7_Domain.Domain.ConsigneeSchema;
using CNET_V7_Domain.Domain.TransactionSchema;
using CNET_V7_Domain.Domain.ViewSchema;
using CNET_V7_Domain.Misc.PmsView;
using DevExpress.Mvvm.POCO;
using DevExpress.XtraReports;
using DevExpress.Printing.Core.PdfExport.Metafile;
using DevExpress.XtraCharts;
using DevExpress.Utils.IoC;
using DevExpress.Office.Utils;

namespace CNET.FrontOffice_V._7.Night_Audit
{
    public partial class frmAuditNightAll : UILogicBase
    {

        #region fields

        private ConfigurationDTO reportArchiveConfig, defaultPrinterConfig, reportArchivePrint;

        //Registration List
        private List<RegistrationListVMDTO> _regListVM = new List<RegistrationListVMDTO>();
      
        private List<DailyRoomChargeDTO> dailyRoomChargeList = new List<DailyRoomChargeDTO>();


        private List<string> _chargeToPost = new List<string>();

        // Bucket Check
        private List<RoomTypeDTO> _roomTypeList;
        private List<RateCodeAudit> _rateCodeAuditList = new List<RateCodeAudit>();
        bool isBucketChecked = false;
        private ActivityDefinitionDTO _salesSummaryWorkflow = null;
        //sales summary


        private List<SalesSummarized> _salesSummaryList = null;
        private bool _isSalesSummarySaved = false;
        private int _roundSalesSummary = 2;
        private string _savedSummarizedVoucher = "";

        // F and B
        private List<OrderStationConsumption> OrderStationConsumptionList = new List<OrderStationConsumption>();
        private List<OrderStationConsumption> OrderStationConsumptionByStion = new List<OrderStationConsumption>();
        private int? Station = null;
        private string StationName = null;
        private int? shift;
        private int? DefualtStoreId;
        private DateTime date;
        private ActivityDefinitionDTO _rowConsumptionWorkflow = null;

        private decimal _roomDiscount = 0;
        private decimal _roomDiscountTax = 0;


        private GridControl _currentGrid;


        public static int organazationUnitDefn { get; set; }
      

        public DateTime CurrentTime { get; set; }
        public DeviceDTO CurrentDevice { get; set; }
        public UserDTO CurrentUser { get; set; }
         
        private frmPMSReports _frmReport;

      //  private BackgroundWorker bWorker = null;

        //automatic print 
        private System.Drawing.Font _printFont;
        private StreamReader _streamReader;

        private bool isReporteArchived;

        private ActivityDefinitionDTO adBucketCheck = null;

        private List<ArticleDTO> AllArticle { get; set; }

        //private JournalizeWizard _journalizeWizard = null;
        private bool _isJournalizationEnabled = false;
        private string _postingRoutineConfig = string.Empty;


        #endregion

        //********** CONSTRUCTOR *****************//
        public frmAuditNightAll()
        {

            InitializeComponent();
            InitializeUI();
                
            
        }

        #region Helper Methods

        private void InitializeUI()
        {

            gvBucketCheck.BestFitColumns();
            gvRoomTaxPostingMaster.OptionsBehavior.Editable = false;
            gvRoomTaxPostingDetail.OptionsBehavior.Editable = false;
            gvRoomTaxPostingMaster.RowStyle += GridRowShadeBasedOnState;

            gvDueOut.BestFitColumns();
            gvOverDueOUt.BestFitColumns();
            gvWaitinglist.BestFitColumns();
            gv6PM.BestFitColumns();
            gvGuaranted.BestFitColumns();
            gvRoomDiscpacy.BestFitColumns();
            gvClearHeldBills.BestFitColumns();
            gvBucketCheck.BestFitColumns();
            gvRoomTaxPostingMaster.BestFitColumns();


            

        }

        

        public bool InitializeData()
        {
            try
            {
                welcomeWizardPage1.AllowNext = false;
                DateTime? CurrentTime2 = UIProcessManager.GetServiceTime();
                if (CurrentTime2 == null)
                {
                    return false;
                }

                List<PeriodDTO> PeriodList = LocalBuffer.PeriodBufferList.Where(x => x.Type == CNETConstantes.PERIOD_TYPE_PMS).ToList();
                if (PeriodList == null || PeriodList.Count == 0)
                {
                    XtraMessageBox.Show("There is no PMS Period Maintained !!!", "Night Audit", MessageBoxButtons.OK, MessageBoxIcon.Error);
                     
                    return false;
                }
               // CurrentTime = CurrentTime2.Value;
               // GetandPopulatePeriodwithnoAuditDone();
                GetHotelandData();
                _frmReport = new frmPMSReports(true, MasterPageForm.home.dashboard);
                try
                {
                    _frmReport.DashboardRefresh();
                }
                catch (IOException ex) { }
                //Check Night Audit Time
                ConfigurationDTO nightAuditTimeConfig = null;
                List<ConfigurationDTO> pmsSettings = LocalBuffer.ConfigurationBufferList.Where(c => c.Reference == CNETConstantes.PMS_Pointer.ToString()).ToList();
                if (pmsSettings != null)
                {
                    nightAuditTimeConfig = pmsSettings.FirstOrDefault(c => c.Attribute == CNETConstantes.PMS_SETTING_NightAuditTime);

                }
                if (nightAuditTimeConfig != null)
                {
                    DateTime nAuditTime = DateTime.ParseExact(nightAuditTimeConfig.CurrentValue, "HH:mm", CultureInfo.InvariantCulture);
                   
                    if (TimeSpan.Compare(CurrentTime.TimeOfDay, nAuditTime.TimeOfDay) == -1)
                    {
                        SystemMessage.ShowModalInfoMessage("Early Night Audit Operation! Night Audit Time is " + nightAuditTimeConfig.CurrentValue, "ERROR");
                        return false;
                    }
                }
                // Disable Summary if false
                ConfigurationDTO SummaryVoucher = null;
                if (pmsSettings != null)
                {
                    SummaryVoucher = pmsSettings.FirstOrDefault(c => c.Attribute == CNETConstantes.PMS_SETTING_SummaryVoucher);
                    if (SummaryVoucher != null && !Convert.ToBoolean(SummaryVoucher.CurrentValue))
                    {
                        wpSummarizeTransaction.Visible = false;
                    }
                }
                
                CurrentDevice = LocalBuffer.CurrentDevice;
                CurrentUser = LocalBuffer.CurrentLoggedInUser;

                deDateIssue.DateTime = CurrentTime;

                //get organization unit definition
              
                    organazationUnitDefn = LocalBuffer.CurrentDeviceConsigneeUnit.Value;
                

                string id = null;
                if (CurrentDevice != null)
                {

                    id = UIProcessManager.IdGenerater("Voucher", CNETConstantes.CASHSALESSUMMRY,0,LocalBuffer.CurrentDeviceConsigneeUnit.Value,false,CurrentDevice.Id);
                }
                if (id != null)
                {
                    teVoucherNo.Text = id;
                    deDateIssue.DateTime = CurrentTime;
                }
                else
                {
                    SystemMessage.ShowModalInfoMessage("There is a problem on id setting for cash sales summary", "ERROR");
                    return false;
                }

                //check Financial Management system License to activate F&B and Journalize Pages
                bool isAccountingLicensed = false;// CommonLogics.CheckFinancialLicense();
                if (!isAccountingLicensed)
                {
                    wpFandB.Visible = false;
                    wpJournalizeRevenue.Visible = false;
                    XtraMessageBox.Show("Financial Managment System is not licensed. F&B and Journalize are deactivated!", "Night Audit", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                }

                //Check PMS Setting of Posting Routine 
                /* ConfigurationDTO isJournalizationEnabledConfig = LocalBuffer.ConfigurationBufferList.FirstOrDefault(c => c.Reference == CNETConstantes.PMS_Pointer.ToString() && c.Attribute == CNETConstantes.PMS_SETTING_EnableJournalization);
                 ConfigurationDTO postRoutConfig = LocalBuffer.ConfigurationBufferList.FirstOrDefault(c => c.Reference == CNETConstantes.PMS_Pointer.ToString() && c.Attribute == CNETConstantes.PMS_SETTING_PostineRoutine);

                 if (postRoutConfig == null || isJournalizationEnabledConfig == null)
                 {
                     XtraMessageBox.Show("Unable to get pms setting of journalization enabled or posting routine.", "Night Audit", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                     return false;
                 }

                 _isJournalizationEnabled = Convert.ToBoolean(isJournalizationEnabledConfig.CurrentValue);
                 _postingRoutineConfig = postRoutConfig.CurrentValue;
                 if(!_isJournalizationEnabled)
                 {
                     wpJournalizeRevenue.Visible = false;
                 }
                 */
                _isJournalizationEnabled = false;
                if (!_isJournalizationEnabled)
                {
                    wpJournalizeRevenue.Visible = false;
                }

                _salesSummaryWorkflow = UIProcessManager.GetActivityDefinitionBydescriptionandreference(CNETConstantes.LU_ACTIVITY_DEFINATION_PREPARED, CNETConstantes.CASHSALESSUMMRY).FirstOrDefault();

                if (_salesSummaryWorkflow == null)
                {
                    SystemMessage.ShowModalInfoMessage("Please define workflow of PREPARED for Cash Sales Summary Voucher ", "ERROR");
                    return false;
                }


                _rowConsumptionWorkflow = UIProcessManager.GetActivityDefinitionBydescriptionandreference(CNETConstantes.LU_ACTIVITY_DEFINATION_PREPARED, CNETConstantes.ROW_MATERIAL_CONSUMPTION_VOUCHER).FirstOrDefault();

                if (_salesSummaryWorkflow == null)
                {
                    SystemMessage.ShowModalInfoMessage("Please define workflow of PREPARED for ROW MATERIAL CONSUMPTION Voucher ", "ERROR");
                    return false;
                }
                

                LoadReportArchiveConfig();

                //All Article
                AllArticle = UIProcessManager.SelectAllArticle();


                //load all roomTypes -> for bucket check

                //populate some page data

                //PopulateOverDueOuts();
                //PopulateDueOuts();
                //PopulateWaitings();
                //PopulateSixPMs();
                //PopulateGuranteed();
                //PopulateDiscrepancy();
                //PopulateReportTree();
                adBucketCheck = UIProcessManager.GetActivityDefinitionBydescriptionandreference(CNETConstantes.LU_ACTIVITY_DEFINATION_BUCKETCHECKED, CNETConstantes.REGISTRATION_VOUCHER).FirstOrDefault();
                 
                 if (adBucketCheck == null)
                 {
                     XtraMessageBox.Show("Unable to get activity definition for bucket check. Please, define BUCKET CHECK workflow for Registration Voucher.", "CNET_v2016", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                     return false;
                 }

                /*
                //setup background worker for report archive operation
                bWorker = new BackgroundWorker();
                bWorker.WorkerReportsProgress = true;
                bWorker.DoWork += bWorker_DoWork;
                bWorker.RunWorkerCompleted += bWorker_RunWorkerCompleted;
                bWorker.ProgressChanged += bWorker_ProgressChanged;
                */
                return true;
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Error in initializing data. DETAIL::" + ex.Message, "CNET_2016", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            
        }
        private int? CurrentDeviceOrganizationUnit = null;
        private bool UserHasHotelBranchAccess { get; set; }
        private List<ConsigneeUnitDTO> iOrgUnit;
        public void GetHotelandData()
        {
            iOrgUnit = LocalBuffer.HotelBranchBufferList;

            CurrentDeviceOrganizationUnit = LocalBuffer.CurrentDeviceConsigneeUnit;
            UserHasHotelBranchAccess = true;

            leHotel.Properties.DisplayMember = "Name";
            leHotel.Properties.ValueMember = "Id";
            leHotel.Properties.DataSource = (LocalBuffer.HotelBranchBufferList.Select(x => new { x.Id, x.Name })).ToList();


            leHotel.EditValue = CurrentDeviceOrganizationUnit;
            leHotel.Properties.ReadOnly = !UserHasHotelBranchAccess;
        }
         
         
        List<PeriodDTO> AuditPeriodwithNoAuditdoneList = new List<PeriodDTO>();
        List<PeriodDTO> AuditPeriodList = new List<PeriodDTO>();
        private void GetandPopulatePeriodwithnoAuditDone()
        {
            DateTime? TodayDate = UIProcessManager.GetServiceTime();
            //LastClosingValidation LastClosingValidation = UIProcessManager.SelectClosingValidationByComponentandBranch(CNETConstantes.PMS_Pointer, 1, SelectedHotelcode);

            //if (LastClosingValidation != null )
            //{
            //    PeriodDTO Periods = new PeriodDTO();
            //    PeriodDTO PeriodLast = LocalBuffer.PeriodBufferList.FirstOrDefault(x => x.Id == LastClosingValidation.Period);

               

            //    string ErrorList = "";
            //    for (DateTime date = PeriodLast.Start.AddDays(1); date <= TodayDate; date = date.AddDays(1))
            //    {
            //        Periods = LocalBuffer.PeriodBufferList.FirstOrDefault(x => x.Start.Date == date.Date && x.Type == CNETConstantes.PERIOD_TYPE_PMS);
            //        if (Periods == null)
            //        {
            //            ErrorList += Environment.NewLine + date.ToShortDateString() ;
            //        }
            //        else
            //        {
            //            AuditPeriodwithNoAuditdoneList.Add(Periods);
            //        }
            //    }

            //    if(!string.IsNullOrEmpty(ErrorList))
            //    {
            //        XtraMessageBox.Show("There is no PMS Period Maintained for " + ErrorList + " Please Fix This !!", "Night Audit", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //        return;
            //    }

            //    sleAuditDate.Properties.DataSource = AuditPeriodwithNoAuditdoneList;
            //    sleAuditDate.Properties.DisplayMember = "periodName"; 
            //    sleAuditDate.Properties.ValueMember = "code";
            //    if (AuditPeriodwithNoAuditdoneList != null && AuditPeriodwithNoAuditdoneList.Count > 0)
            //    {
            //        sleAuditDate.EditValue = AuditPeriodwithNoAuditdoneList.FirstOrDefault().Id;
            //    }
            //}
            //else
            //{
                PeriodDTO PeriodList = LocalBuffer.PeriodBufferList.FirstOrDefault(x => x.Start.Date == TodayDate.Value.Date && x.Type == CNETConstantes.PERIOD_TYPE_PMS);
                if (PeriodList == null)
                {
                    XtraMessageBox.Show("There is no PMS Period Maintained for today !!!", "Night Audit", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                AuditPeriodwithNoAuditdoneList.Add(PeriodList);
                sleAuditDate.Properties.DataSource = AuditPeriodwithNoAuditdoneList;
                sleAuditDate.Properties.DisplayMember = "PeriodName";
                sleAuditDate.Properties.ValueMember = "Id";

           // }

        }

        //Helper method to show checkout form based on the value of the passed parameters
        private bool PerfomCheckout(RegistrationListVMDTO regExten, bool isWithoutRecept, bool isWithZeroBalance = false, bool isReinstate = false)
        {
            bool flag = false;
            try
            {
                VoucherBuffer vo = UIProcessManager.GetVoucherBufferById(regExten.Id);
                if (vo == null) return false;
                var osd = LocalBuffer.ObjectStateDefinitionBufferList.FirstOrDefault(o => o.Id == vo.Voucher.LastState);
                string state = osd == null ? "" : osd.Description;
                if (vo.Voucher.LastState != CNETConstantes.CHECKED_IN_STATE)
                {
                    SystemMessage.ShowModalInfoMessage(state + " state can not be changed to Check Out state!", "ERROR");
                    return false;
                }


                frmCheckOut frmCheckOut = new frmCheckOut();
                
                frmCheckOut.RegExtension = regExten;
                frmCheckOut.VoucherBuffer = vo;
                frmCheckOut.IsReinstate = isReinstate;
                frmCheckOut.checkOutWithOutReceipt = isWithoutRecept;
                frmCheckOut.IsWithZeroBalance = isWithZeroBalance;
                frmCheckOut.IsFromNightAudit = true;
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

        private void LoadRegistrationDocument(int state, DateTime? arrivalDate, DateTime? departureDate)
        {
            if (_regListVM != null)
                _regListVM.Clear(); 


            _regListVM = UIProcessManager.GetRegistrationViewModelData(arrivalDate, departureDate,state,SelectedHotelcode.Value);

        }
 
        //updating room status to dirty after night audit completion
        private bool ChangeAllRoomsState()
        {
            RoomDetailDTO flag = null;
            try
            {
               // //CNETInfoReporter.WaitForm("Changing Room Status..");

                List<RoomTypeDTO> pseudoRoomList = UIProcessManager.GetRoomTypeByispseudoRoomType(true);
                List<int> pseudoRooms = new List<int>();
                if (pseudoRoomList != null)
                    pseudoRooms = pseudoRoomList.Select(p => p.Id).ToList();

                List<RoomDetailDTO> rmDetails = null;
                ActivityDefinitionDTO adOOO = UIProcessManager.GetActivityDefinitionBycomponetanddescription(CNETConstantes.PMS_Pointer, CNETConstantes.OOO).FirstOrDefault();
                ActivityDefinitionDTO adOOS = UIProcessManager.GetActivityDefinitionBycomponetanddescription(CNETConstantes.PMS_Pointer, CNETConstantes.OOS).FirstOrDefault();
                
                if (pseudoRooms != null && pseudoRooms.Count > 0)
                {
                    rmDetails = UIProcessManager.SelectAllRoomDetail().Where(rd => !pseudoRooms.Contains(rd.RoomType)).ToList();

                }
                else
                {
                    rmDetails = UIProcessManager.SelectAllRoomDetail();
                }

                if (rmDetails != null)
                {
                    ////CNETInfoReporter.WaitForm("Updating rooms status to dirty", rmDetails.Count);
                    ActivityDefinitionDTO adDirty = UIProcessManager.GetActivityDefinitionBycomponetanddescription(CNETConstantes.PMS_Pointer, CNETConstantes.Dirty).FirstOrDefault();
                    if (adDirty != null)
                    {
                        foreach (var rd in rmDetails)
                        {
                            //skip for out of order and out of service rooms
                            if (adOOO != null && rd.LastState == adOOO.Id) continue;
                            if (adOOS != null && rd.LastState == adOOS.Id) continue;

                            rd.LastState = adDirty.Id;
                            flag = UIProcessManager.UpdateRoomDetail(rd);
                            logActiviy(rd);
                        }

                    }
                    //CNETInfoReporter.Hide();
                }

                return flag == null? false : true ;

            }
            catch (Exception ex)
            {
                //CNETInfoReporter.Hide();
                return false;
            }
        }
        private void logActiviy(RoomDetailDTO rmd)
        {
            try
            {
                ActivityDTO act = new ActivityDTO();
                act.ActivityDefinition = rmd.LastState;
                act.TimeStamp = CurrentTime.ToLocalTime(); 
                act.Year = CurrentTime.Year;
                act.Month = CurrentTime.Month;
                act.Date = CurrentTime.Day;
                act.Reference = rmd.Id;
                act.User = LocalBuffer.CurrentLoggedInUser.Id;
                act.Pointer = CNETConstantes.HouseKeeping_Mgt; 
                act.Device = LocalBuffer.CurrentDevice.Id; 
                act.ConsigneeUnit = LocalBuffer.CurrentDeviceConsigneeUnit;
                act.Remark = "Night Audit";
                UIProcessManager.CreateActivity(act);
            }
            catch (Exception e) { }
        }
        private void LoadReportArchiveConfig()
        {
            if (CurrentDevice != null)
            {
                List<ConfigurationDTO> pmsSettings = LocalBuffer.ConfigurationBufferList.Where(c => c.Reference == CNETConstantes.PMS_Pointer.ToString()).ToList();
                if (pmsSettings != null)
                {
                    reportArchiveConfig = pmsSettings.FirstOrDefault(c => c.Attribute == CNETConstantes.PMS_SETTING_ArchivePath);
                    reportArchivePrint = pmsSettings.FirstOrDefault(c => c.Attribute == CNETConstantes.PMS_SETTING_ArchivePrint);
                }
                if (reportArchiveConfig == null)
                {
                    XtraMessageBox.Show("Unable to get Report Archive save path. Please check your configuration on Server Management and press refresh button.", "CNET_2016", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(reportArchiveConfig.CurrentValue))
                        XtraMessageBox.Show("Unable to get Report Archive save path. Please check your configuration on Server Management and press refresh button.", "CNET_2016", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else
                    {

                        label_reportArchivePath.Text = "Archive Path:   " + reportArchiveConfig.CurrentValue;
                    }
                }
            }

            //Default printer
            List<ConfigurationDTO> regVocConfigs = LocalBuffer.ConfigurationBufferList.Where(c => c.Reference == CNETConstantes.REGISTRATION_VOUCHER.ToString()).ToList();
            if (regVocConfigs != null)
            {
                defaultPrinterConfig = regVocConfigs.FirstOrDefault(c => c.Attribute == "Default Printer");
            }
            if (defaultPrinterConfig == null)
            {
                XtraMessageBox.Show("Please set default printer setting for registration voucher!", "CNET_2016", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                
            }
            else
            {
                lc_defaultPrinter.Text = "Default Printer:   " + defaultPrinterConfig.CurrentValue;
            }
        }

        private void PopulateOverDueOuts()
        {
            gcOverDueOut.DataSource = null;
            LoadRegistrationDocument(CNETConstantes.CHECKED_IN_STATE, null, null);
            if (_regListVM != null)
            {
                var filtered = _regListVM.Where(r => r.Departure < CurrentTime.Date).ToList();
                gcOverDueOut.DataSource = filtered;
                gvOverDueOUt.RefreshData();

            }
        }

        private void PopulateDueOuts()
        {
            gcDueOut.DataSource = null;
            LoadRegistrationDocument(CNETConstantes.CHECKED_IN_STATE, null, CurrentTime);
            gcDueOut.DataSource = _regListVM;
            gvDueOut.RefreshData();
        }

        private void PopulateWaitings()
        {
            gcWaitinglist.DataSource = null;
            LoadRegistrationDocument(CNETConstantes.OSD_WAITLIST_STATE, CurrentTime, null);
            gcWaitinglist.DataSource = _regListVM;
            gvWaitinglist.RefreshData();
        }

        private void PopulateSixPMs()
        {
            gc6PM.DataSource = null;
            LoadRegistrationDocument(CNETConstantes.SIX_PM_STATE, CurrentTime, null);
            gc6PM.DataSource = _regListVM;
            gv6PM.RefreshData();
        }

        private void PopulateGuranteed()
        {
            gcGauranted.DataSource = null;
            LoadRegistrationDocument(CNETConstantes.GAURANTED_STATE, CurrentTime, null);
            gcGauranted.DataSource = _regListVM;
            gvGuaranted.RefreshData();
        }

        private void PopulateHeldBills()
        {
            gcClearHeldBills.DataSource = null;
            List<HeldTransaction> allHelTrans = UIProcessManager.GetRMSHeldTransaction(CNETConstantes.ORDER, CNETConstantes.OSD_HELD_STATE);
            if (allHelTrans == null) return;

            if(SelectedHotelcode != null)
            {
                allHelTrans = allHelTrans.Where(x => x.orgUnit == SelectedHotelcode).ToList();
            }


            var filtered = allHelTrans.Where(h => h.TimeStamp.Date == CurrentTime.Date);
            gcClearHeldBills.DataSource = filtered;
            gvClearHeldBills.RefreshData();
        }

        private void PopulateRoomAndTax()
        {
            _chargeToPost.Clear();
            BindingList<DailyRoomVoucherVM> _drcToChargeList = new BindingList<DailyRoomVoucherVM>();
            gcRoomTaxPosting.DataSource = null;
            List<int> pseudoRooms = null;

        
            if (_roomTypeList != null)
            {
                var filtered = _roomTypeList.Where(rt => rt.IspseudoRoomType.Value || rt.CanBeMeetingRoom.Value).ToList();
                if(filtered != null)
                {
                    pseudoRooms = filtered.Select(p => p.Id).ToList();
                }
            }

            wpRoomTaxPosting.AllowNext = true;

            dailyRoomChargeList = new List<DailyRoomChargeDTO>();

            LoadRegistrationDocument(CNETConstantes.CHECKED_IN_STATE, null, null);
            List<RegistrationListVMDTO> regToCharge = null;
            if (_regListVM != null)
            {
                regToCharge = _regListVM.Where(r => r.Departure.Date >= CurrentTime.Date).ToList();

            }

            if (regToCharge != null)
            {
                foreach (var reg in regToCharge)
                {

                    //get applicable tax
                    int? accArticle = null;
                    var rateCodeHeader = UIProcessManager.GetRateCodeHeaderById(reg.RateCodeHeader.Value);
                    if (rateCodeHeader != null)
                    {
                        accArticle = rateCodeHeader.Article;
                    }
                    else
                    {
                        SystemMessage.ShowModalInfoMessage("Unable to find registration's rate", "ERROR");
                        return;
                    }

                    if (accArticle == null)
                    {
                        SystemMessage.ShowModalInfoMessage("Unable to find rate's article", "ERROR");
                        return;
                    }

                    TaxDTO tax = CommonLogics.GetApplicableTax(reg.Id, CNETConstantes.DAILY_ROOM_CHARGE_VOUCHER, reg.GuestId, accArticle);
                    if (!string.IsNullOrEmpty(tax.Remark))
                    {
                        SystemMessage.ShowModalInfoMessage(tax.Remark, "ERROR");
                        return;
                    }

                    DailyRoomChargeDTO dailyRoomCharge = UIProcessManager.GetDailyRoomChargePostingByRegistration(reg.Id, CurrentTime.Date, CNETConstantes.REGISTRATION_VOUCHER, CNETConstantes.CHECKED_IN_STATE, tax,null,null);
                    if (dailyRoomCharge != null)
                    {
                        dailyRoomChargeList.Add(dailyRoomCharge);
                    }

                }
            }

            foreach (DailyRoomChargeDTO dr in dailyRoomChargeList)
            {
               // RegistrationDetail rd = UIProcessManager.GetRegistrationDetailByVoucher(dr.registrationVoucher).FirstOrDefault();
                RegistrationDetailDTO rd = UIProcessManager.GetRegistrationDetailByvoucher(dr.registrationId).FirstOrDefault(d => d.Date.Value.Date == CurrentTime.Date);
                if (rd != null && pseudoRooms != null)
                {
                    if (pseudoRooms.Contains(rd.RoomType.Value))
                    {
                        continue;
                    }
                }

                

                VwConsigneeViewDTO per = new VwConsigneeViewDTO();
                if (dr.dailyRoomChargeVoucher != null)
                {
                    per = LocalBuffer.ConsigneeViewBufferList.FirstOrDefault(p => p.Id == dr.dailyRoomChargeVoucher.Consignee1);
                }

                DailyRoomVoucherVM dto = new DailyRoomVoucherVM();
                dto.registrationId = dr.registrationId;
                dto.registrationNo = dr.registrationVoucher;
                dto.roomNo = dr.room;
                if (per != null)
                {
                    dto.consignee = per.FirstName + " " + per.SecondName + " " + per.ThirdName;
                }
                if (dr.dailyRoomChargeVoucher != null) dto.amount = Math.Round(dr.dailyRoomChargeVoucher.GrandTotal == null ? 0 : dr.dailyRoomChargeVoucher.GrandTotal.Value, 2);

                List<int> dList = UIProcessManager.GetDailyRoomVoucherByReg(dr.registrationId, CurrentTime);
                if (dList == null || dList.Count == 0)
                {
                    dto.isCharged = false;
                    _chargeToPost.Add(dto.registrationNo);
                }
                else
                {
                    dto.isCharged = true;
                }

                _drcToChargeList.Add(dto);

                
                foreach (LineItemDetails itemD in dr.lineItemList)
                {
                    if (itemD != null)
                    {
                        var article =  UIProcessManager.GetArticleById(itemD.lineItems.Article);

                        dto.lineItems.Add(new DailyLineDTO()
                        {
                            articleCode = itemD.lineItems.Article,
                            quantity = itemD.lineItems.Quantity,
                            totalAmunt = itemD.lineItems.TotalAmount != null ? Math.Round(itemD.lineItems.TotalAmount.Value, 2) : 0,
                            Name =
                               article != null
                                    ? article.Name
                                    : String.Empty,
                            unitAmount = itemD.lineItems.UnitAmount != null ? Math.Round(itemD.lineItems.UnitAmount.Value, 2) : 0
                        });

                    }

                }

            }
            if(_chargeToPost.Count == 0)
                wpRoomTaxPosting.AllowNext = true ;
            else
                wpRoomTaxPosting.AllowNext = false;
            gcRoomTaxPosting.DataSource = _drcToChargeList;
            gvRoomTaxPostingMaster.RefreshData();
        }

        private void PopulateBucketCheck()
        {
            _rateCodeAuditList.Clear();
            //load Inhouse Registrations
            LoadRegistrationDocument(CNETConstantes.CHECKED_IN_STATE, null, null);
            if(_regListVM == null) return;
            foreach (RegistrationListVMDTO regDoc in _regListVM)
            {
                RateCodeAudit rateCodeAudit = new RateCodeAudit();
                rateCodeAudit.RegistrationId = regDoc.Id;
                rateCodeAudit.RegistrationNumber = regDoc.Registration;
                rateCodeAudit.RoomNumber = regDoc.Room;
                rateCodeAudit.GuestName = regDoc.Guest;
                rateCodeAudit.Consignee = regDoc.GuestId;
                rateCodeAudit.Company = regDoc.Company;
                rateCodeAudit.Adult = regDoc.adult == null ? 1 : regDoc.adult;
                rateCodeAudit.Child = regDoc.adult == null ? 0 : regDoc.child;
                rateCodeAudit.RateCodeHeader = regDoc.RateCodeHeader;
                string currencyDescription = "";
                if (regDoc.RateCodeHeader != null)
                {
                    RateCodeHeaderDTO Header = UIProcessManager.GetRateCodeHeaderById(regDoc.RateCodeHeader.Value);
                    if (Header != null && Header.CurrencyCode != null)
                    {
                        currencyDescription = LocalBuffer.CurrencyBufferList.FirstOrDefault(x => x.Id == Header.CurrencyCode).Description;
                    }
                }

                rateCodeAudit.CurrencyDescription = currencyDescription;
                rateCodeAudit.RateAmount = regDoc.RateAmount == null? 0: regDoc.RateAmount.Value;
                rateCodeAudit.RateCodeAmount = UIProcessManager.GetUnitRoomRate(regDoc.RateCodeDetail.Value, regDoc.RoomType.Value, rateCodeAudit.Child, rateCodeAudit.Adult);
                rateCodeAudit.Variance = Math.Round( rateCodeAudit.RateCodeAmount - rateCodeAudit.RateAmount, 2);
                //currency
                rateCodeAudit.ArrivalDate = regDoc.Arrival;
                rateCodeAudit.DepartureDate = regDoc.Departure;
                rateCodeAudit.RoomType = regDoc.RoomTypeDescription;
                RoomTypeDTO rt = _roomTypeList.FirstOrDefault(r => r.Id == regDoc.ActualRTC);
                if (rt != null)
                {
                    rateCodeAudit.RTC = rt.Description;

                }
                rateCodeAudit.RegistrationState = regDoc.lastState;
                rateCodeAudit.Color = regDoc.Color;
                _rateCodeAuditList.Add(rateCodeAudit);
         
            }
            gcBucketCheck.DataSource = _rateCodeAuditList;
            gvBucketCheck.RefreshData();
        }

        // Populate All pages Grid Data
        private void PopulateGridData(BaseWizardPage page)
        {

            try
            {
                //CNETInfoReporter.WaitForm("Loading Data", "Please Wait...");

                #region Overdueouts
                if (page == wpOverDueOut)
                {
                    PopulateOverDueOuts();
                }

                #endregion

                #region DueOuts
                else if (page == wpDueOut)
                {
                    PopulateDueOuts();
                }

                #endregion

                #region Waiting
                else if (page == wpWaitinglist)
                {
                    PopulateWaitings();
                }

                #endregion

                #region 6PM
                else if (page == wp6PM)
                {
                    PopulateSixPMs();
                }

                #endregion

                #region Guaranteed
                else if (page == wpGuaranted)
                {
                    PopulateGuranteed();
                }

                #endregion

                #region Room Discripancy
                else if (page == wpRoomDiscipancy)
                {
                    PopulateDiscrepancy();
                }

                #endregion

                #region Clear Held Bills
                else if (page == wpClearHeldBill)
                {
                    PopulateHeldBills();
                }

                #endregion

                #region Bucket Check
                else if (page == wpBucketCheck)
                {
                    PopulateBucketCheck();
                }

                #endregion

                #region Room and Tax Posting
                else if (page == wpRoomTaxPosting)
                {
                    PopulateRoomAndTax();
                }

                #endregion

                #region Summarize Transaction
                else if (page == wpSummarizeTransaction)
                {
                    PopulateSalesSummary();
                }

                #endregion

                #region F and B
                else if (page == wpFandB)
                {
                    PopulateOrderStations();
                }

                #endregion

                #region Journalize Revenue
                else if (page == wpJournalizeRevenue)
                {
                    //if(_journalizeWizard != null)
                    //{
                    //    _journalizeWizard.PopulateVouchersData();
                    //}
                }

                #endregion

                #region Report Archive
                else if (page == wpReportArchive)
                {
                    LoadReportArchiveConfig();
                    PopulateReportTree();
                }



                #endregion

                //CNETInfoReporter.Hide();

            }
            catch (Exception ex)
            {
                //CNETInfoReporter.Hide();
                XtraMessageBox.Show("Error in populating data. DETAIL::" + ex.Message, "CNET_2016", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ControlNextButton(BaseWizardPage page)
        {
            #region Overdueouts
            if (page == wpOverDueOut)
            {
                _currentGrid = gcOverDueOut;

                if (gvOverDueOUt.RowCount > 0)
                {
                    wpOverDueOut.AllowNext = false;
                }
                else
                {
                    wpOverDueOut.AllowNext = true;
                }

            }

            #endregion

            #region DueOuts
            else if (page == wpDueOut)
            {
                _currentGrid = gcDueOut;
                if (gvDueOut.RowCount > 0)
                {
                    wpDueOut.AllowNext = false;
                }
                else
                {
                    wpDueOut.AllowNext = true;
                }
            }

            #endregion

            #region Waiting
            else if (page == wpWaitinglist)
            {
                _currentGrid = gcWaitinglist;
                if (gvWaitinglist.RowCount > 0)
                {
                    wpWaitinglist.AllowNext = false;
                }
                else
                {
                    wpWaitinglist.AllowNext = true;
                }
            }

            #endregion

            #region 6PM
            else if (page == wp6PM)
            {
                _currentGrid = gc6PM;
                if (gv6PM.RowCount > 0)
                {
                    wp6PM.AllowNext = false;
                }
                else
                {
                    wp6PM.AllowNext = true;
                }
            }

            #endregion

            #region Guaranteed
            else if (page == wpGuaranted)
            {
                _currentGrid = gcGauranted;
                if (gvGuaranted.RowCount > 0)
                {
                    wpGuaranted.AllowNext = false;
                }
                else
                {
                    wpGuaranted.AllowNext = true;
                }
            }

            #endregion

            #region Room Discripancy

            else if (page == wpRoomDiscipancy)
            {
                _currentGrid = gcRoomDiscripacy;
            }

            #endregion

            #region Clear Held Bills
            else if (page == wpClearHeldBill)
            {
                _currentGrid = gcClearHeldBills;
                if (gvClearHeldBills.RowCount > 0)
                {
                    wpClearHeldBill.AllowNext = false;
                }
                else
                {
                    wpClearHeldBill.AllowNext = true;
                }
            }

            #endregion

            #region Bucket Check
            else if (page == wpBucketCheck)
            {
                _currentGrid = gcBucketCheck;
                int[] selectedRows = gvBucketCheck.GetSelectedRows();
                if (selectedRows.Length == gvBucketCheck.RowCount)
                {
                    wpBucketCheck.AllowNext = true;
                }
                else
                {
                    wpBucketCheck.AllowNext = false;
                }
            }

            #endregion

            #region Room and Tax Posting
            else if (page == wpRoomTaxPosting)
            {
                //To Do
            }

            #endregion

            #region Summarize Transaction
            else if (page == wpSummarizeTransaction)
            {
                _currentGrid = gcSummarizeTransaction;
                //To Do
            }

            #endregion

            #region F and B
            else if (page == wpFandB)
            {
                _currentGrid = gcFandBVouchers;
                //To Do
            }

            #endregion

            #region Journalize Revenue
            else if (page == wpJournalizeRevenue)
            {
                //To Do
            }

            #endregion

            #region Report Archive
            else if (page == wpReportArchive)
            {
                wpReportArchive.AllowNext = false;
            }
          

           

            #endregion
            //wpOverDueOut.AllowNext = true;
        }

        private void HandleWizarPageChange(BaseWizardPage page)
        {
            #region Overdueouts
            if (page == wpOverDueOut)
            {
                rpgJournalize.Visible = false;
                rpgSave.Visible = false;
                rpgPost.Visible = false;
                rpgOptions.Visible = false;
                rpgRefresh.Visible = true;
                rpgAmendDate.Visible = true;
                rpgCheckOut.Visible = true;
                rpgCheckIn.Visible = false;
                rpgCancel.Visible = false;
                rpgPrint.Visible = true;
                rpgPush.Visible = false;
                rpgVoid.Visible = false;
                rpgReport.Visible = false;
                rpgReportUtils.Visible = false;
                rpgExport.Visible = true;
                
               
            }

            #endregion

            #region DueOuts
            else if (page == wpDueOut)
            {
                rpgJournalize.Visible = false;
                rpgSave.Visible = false;
                rpgPost.Visible = false;
                rpgOptions.Visible = false;
                rpgRefresh.Visible = true;
                rpgAmendDate.Visible = true;
                rpgCheckOut.Visible = true;
                rpgCheckIn.Visible = false;
                rpgCancel.Visible = false;
                rpgPrint.Visible = true;
                rpgPush.Visible = false;
                rpgVoid.Visible = false;
                rpgReport.Visible = false;
                rpgReportUtils.Visible = false;
                rpgExport.Visible = true;
            }

            #endregion

            #region Waiting
            else if (page == wpWaitinglist)
            {
                rpgJournalize.Visible = false;
                rpgSave.Visible = false;
                rpgPost.Visible = false;
                rpgOptions.Visible = false;
                rpgRefresh.Visible = true;
                rpgAmendDate.Visible = true;
                rpgCheckOut.Visible = false;
                rpgCheckIn.Visible = true;
                rpgCancel.Visible = true;
                rpgPrint.Visible = true;
                rpgPush.Visible = false;
                rpgVoid.Visible = false;
                rpgReport.Visible = false;
                rpgReportUtils.Visible = false;
                rpgExport.Visible = true;
            }

            #endregion

            #region 6PM
            else if (page == wp6PM)
            {
                rpgJournalize.Visible = false;
                rpgSave.Visible = false;
                rpgPost.Visible = false;
                rpgOptions.Visible = false;
                rpgRefresh.Visible = true;
                rpgAmendDate.Visible = true;
                rpgCheckOut.Visible = false;
                rpgCheckIn.Visible = true;
                rpgCancel.Visible = true;
                rpgPrint.Visible = true;
                rpgPush.Visible = false;
                rpgVoid.Visible = false;
                rpgReport.Visible = false;
                rpgReportUtils.Visible = false;
                rpgExport.Visible = true;
            }

            #endregion

            #region Guaranteed
            else if (page == wpGuaranted)
            {
                rpgJournalize.Visible = false;
                rpgSave.Visible = false;
                rpgPost.Visible = false;
                rpgOptions.Visible = false;
                rpgRefresh.Visible = true;
                rpgAmendDate.Visible = true;
                rpgCheckOut.Visible = false;
                rpgCheckIn.Visible = true;
                rpgCancel.Visible = true;
                rpgPrint.Visible = true;
                rpgPush.Visible = false;
                rpgVoid.Visible = false;
                rpgReport.Visible = false;
                rpgReportUtils.Visible = false;
                rpgNoShow.Visible = true;
                rpgExport.Visible = true;
            }

            #endregion

            #region Room Discripancy
            else if (page == wpRoomDiscipancy)
            {
                rpgJournalize.Visible = false;
                rpgSave.Visible = false;
                rpgPost.Visible = false;
                rpgOptions.Visible = false;
                rpgRefresh.Visible = true;
                rpgAmendDate.Visible = false;
                rpgCheckOut.Visible = false;
                rpgCheckIn.Visible = false;
                rpgCancel.Visible = false;
                rpgPrint.Visible = true;
                rpgPush.Visible = false;
                rpgVoid.Visible = false;
                rpgReport.Visible = false;
                rpgReportUtils.Visible = false;
                rpgNoShow.Visible = false;
                rpgExport.Visible = true;
            }

            #endregion

            #region Clear Held Bills
            else if (page == wpClearHeldBill)
            {
                rpgJournalize.Visible = false;
                rpgSave.Visible = false;
                rpgPost.Visible = false;
                rpgOptions.Visible = false;
                rpgRefresh.Visible = true;
                rpgAmendDate.Visible = false;
                rpgCheckOut.Visible = false;
                rpgCheckIn.Visible = false;
                rpgCancel.Visible = false;
                rpgPrint.Visible = true;
                rpgPush.Visible = true;
                rpgVoid.Visible = true;
                rpgReport.Visible = false;
                rpgReportUtils.Visible = false;
                rpgNoShow.Visible = false;
                rpgExport.Visible = true;
            }

            #endregion

            #region Bucket Check
            else if (page == wpBucketCheck)
            {
                rpgJournalize.Visible = false;
                rpgSave.Visible = false;
                rpgPost.Visible = false;
                rpgOptions.Visible = false;
                rpgRefresh.Visible = true;
                rpgAmendDate.Visible = false;
                rpgCheckOut.Visible = false;
                rpgCheckIn.Visible = false;
                rpgCancel.Visible = false;
                rpgPrint.Visible = true;
                rpgPush.Visible = false;
                rpgVoid.Visible = false;
                rpgReport.Visible = false;
                rpgReportUtils.Visible = false;
                rpgNoShow.Visible = false;
                rpgExport.Visible = true;
            }

            #endregion

            #region Room and Tax Posting
            else if (page == wpRoomTaxPosting)
            {
                rpgJournalize.Visible = false;
                rpgSave.Visible = false;
                rpgPost.Visible = true;
                rpgOptions.Visible = false;
                rpgRefresh.Visible = true;
                rpgAmendDate.Visible = false;
                rpgCheckOut.Visible = false;
                rpgCheckIn.Visible = false;
                rpgCancel.Visible = false;
                rpgPrint.Visible = true;
                rpgPush.Visible = false;
                rpgVoid.Visible = false;
                rpgReport.Visible = false;
                rpgReportUtils.Visible = false;
                rpgNoShow.Visible = false;
                rpgExport.Visible = false;
            }

            #endregion

            #region Summarize Transaction
            else if (page == wpSummarizeTransaction)
            {
                rpgJournalize.Visible = false;
                rpgSave.Visible = true;
                rpgPost.Visible = false;
                rpgOptions.Visible = false;
                rpgRefresh.Visible = true;
                rpgAmendDate.Visible = false;
                rpgCheckOut.Visible = false;
                rpgCheckIn.Visible = false;
                rpgCancel.Visible = false;
                rpgPrint.Visible = true;
                rpgPush.Visible = false;
                rpgVoid.Visible = false;
                rpgReport.Visible = false;
                rpgReportUtils.Visible = false;
                rpgNoShow.Visible = false;
                rpgExport.Visible = true;
            }

            #endregion

            #region F and B
            else if (page == wpFandB)
            {
                rpgJournalize.Visible = false;
                rpgSave.Visible = true;
                rpgPost.Visible = false;
                rpgOptions.Visible = false;
                rpgRefresh.Visible = true;
                rpgAmendDate.Visible = false;
                rpgCheckOut.Visible = false;
                rpgCheckIn.Visible = false;
                rpgCancel.Visible = false;
                rpgPrint.Visible = true;
                rpgPush.Visible = false;
                rpgVoid.Visible = false;
                rpgReport.Visible = false;
                rpgReportUtils.Visible = false;
                rpgNoShow.Visible = false;
                rpgExport.Visible = true;
            }

            #endregion

            #region Journalize Revenue 
            else if (page == wpJournalizeRevenue)
            {
                rpgJournalize.Visible = true;
                rpgSave.Visible = false;
                rpgPost.Visible = false;
                rpgOptions.Visible = false;
                rpgRefresh.Visible = true;
                rpgAmendDate.Visible = false;
                rpgCheckOut.Visible = false;
                rpgCheckIn.Visible = false;
                rpgCancel.Visible = false;
                rpgPrint.Visible = true;
                rpgPush.Visible = false;
                rpgVoid.Visible = false;
                rpgReport.Visible = false;
                rpgReportUtils.Visible = false;
                rpgNoShow.Visible = false;
                rpgExport.Visible = false;
            }

            #endregion

            #region Report Archive
            else if (page == wpReportArchive)
            {
                rpgJournalize.Visible = false;
                rpgSave.Visible = false;
                rpgPost.Visible = false;
                rpgOptions.Visible = false;
                rpgRefresh.Visible = true;
                rpgAmendDate.Visible = false;
                rpgCheckOut.Visible = false;
                rpgCheckIn.Visible = false;
                rpgCancel.Visible = false;
                rpgPrint.Visible = false;
                rpgPush.Visible = false;
                rpgVoid.Visible = false;
                rpgReport.Visible = true;
                rpgReportUtils.Visible = false;
                rpgNoShow.Visible = false;
                rpgExport.Visible = false;
            }

            else
            {
                rpgJournalize.Visible = false;
                rpgSave.Visible = false;
                rpgPost.Visible = false;
                rpgOptions.Visible = false;
                rpgRefresh.Visible = false;
                rpgAmendDate.Visible = false;
                rpgCheckOut.Visible = false;
                rpgCheckIn.Visible = false;
                rpgCancel.Visible = false;
                rpgPrint.Visible = false;
                rpgPush.Visible = false;
                rpgVoid.Visible = false;
                rpgReport.Visible = false;
                rpgReportUtils.Visible = false;
                rpgNoShow.Visible = false;
                rpgExport.Visible = false;
            }

            #endregion
        }

        private void PopulateReportTree()
        {
            treeListReports.DataSource = null;

            
   
            List<ReportDTO> reportList = UIProcessManager.SelectAllReport();
            if (reportList == null)
            {
                return;
            }


            List<ReportDTO> filtered = reportList.Where(r => r.ReportType == CNETConstantes.Report_Type_Night_Audit && r.Category == CNETConstantes.Report_Category_Night_Audit && r.IsActive == true).ToList();
            ReportDTO DashboardReport = new ReportDTO
            {
                Code = "Rp111111111",
                Index=0,
                Pointer = CNETConstantes.PMS_Report,
                Category = CNETConstantes.Report_Category_Night_Audit,
                ReportType = CNETConstantes.Report_Type_Night_Audit,
                Reference = "",
                DefaultName="Dashboard Report",
                CustomName = "Dashboard Report",
                Description = "",
                IsActive=true,
                Url="",
                Remark="" 
            };

            filtered.Add(DashboardReport);

            if (filtered != null && filtered.Count > 0)
                filtered = filtered.OrderByDescending(x=> x.CustomName).ToList();
          //  here

            treeListReports.DataSource = filtered;
            treeListReports.RefreshDataSource();

            ceBx.CheckState = CheckState.Checked;


        }

        private void PopulateDiscrepancy()
        {
            try
            {
                /*  List<DiscrepancyDTO> allDisc = UIProcessManager.SelectAllDiscrepancy();
                  if (allDisc == null) return;
                  var filtered = allDisc.Where(b => Convert.ToDateTime(b.Date) == CurrentTime.Date).ToList();
                 List<VwRoomManagmentViewDTO> roomManagmentlist = UIProcessManager.GetAllRoomManagment();
                 List<string> RoomtypeDescList = _roomTypeList.Select(x => x.Description).ToList(); ;

                  if (filtered == null) return;
                  List<DiscripancyReport> holdDiscripancy = new List<DiscripancyReport>();

                  foreach (DiscrepancyDTO discripancy in filtered)
                  {
                      DiscripancyReport ddto = new DiscripancyReport();
                      int? roomcode = discripancy.RoomDetail;
                      VwRoomManagmentViewDTO rr = roomManagmentlist.FirstOrDefault(r => r.roomDetailCode == roomcode);
                      if (RoomtypeDescList.Contains(rr.RoomType))
                      {
                          if (rr != null)
                          {
                              ddto.RoomNo = rr.roomNo;
                              ddto.RoomType = rr.RoomType;
                              ddto.RoomStatus = rr.rmstatus;
                          }
                          LookupDTO look = LocalBuffer.LookupBufferList.FirstOrDefault(l => l.code == discripancy.hkValue);
                          if (look != null)
                          {
                              ddto.HkStatus = look.Description;

                          }

                          RegistrationStatusDTO foandresStatus = UIProcessManager.GetRegistrationStatus(roomcode.Value, CurrentTime.Date);
                          if (foandresStatus != null)
                          {
                              string fstatus = foandresStatus.FOStatus;
                              if (fstatus == null & foandresStatus.registrationStatus == null) { ddto.FoStatus = ""; ddto.ResStatus = ""; }
                              else
                              {
                                  if (fstatus.Equals("1")) { ddto.FoStatus = "Ocuppied"; }
                                  else if (fstatus.Equals("0")) { ddto.FoStatus = "Vacant"; }
                                  ddto.ResStatus = foandresStatus.registrationStatus;
                              }

                          }
                          ddto.HkPerson = discripancy.FoValue;
                          ddto.Discrepancy = discripancy.DiscrepancyType;
                          //ddto.roomdetail = discripancy.roomDetail;
                          ddto.Date = discripancy.Date.ToString();
                          DateTime dt = Convert.ToDateTime(discripancy.Date);
                          string g = (dt).Date.ToString("yyyy-MM-dd");
                          DateTime tt = DateTime.ParseExact(g, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
                          string fp = "";
                          List<RegistrationDetailDTO> regFop = UIProcessManager.GetRegistrationDetailByRoomAndDate(roomcode.Value, CurrentTime.Date);
                          if (regFop != null)
                          {
                              if (regFop.Count == 0)
                              {
                                  ddto.FoPerson = "0";
                              }
                              else
                              {
                                  int totalFoPerson = 0;
                                  foreach (RegistrationDetailDTO regD in regFop)
                                  {
                                      totalFoPerson += Convert.ToInt32(regD.Adult);
                                      ddto.FoPerson = totalFoPerson.ToString();
                                  }
                              }

                          }
                          holdDiscripancy.Add(ddto);
                      }
                  }
                */
                gcRoomDiscripacy.DataSource = null;// holdDiscripancy;
                gvRoomDiscpacy.RefreshData();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Error loading room discripancy. DETAIL: " + ex.Message, "CNET_v2016", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Report Exporting
        private void treeListReports_AfterCheckNode(object sender, NodeEventArgs e)
        {
            TreeListNode focusedNode = e.Node;

            if (focusedNode != null)
            {
                if (focusedNode.Checked)
                    focusedNode.CheckAll();
                else
                    focusedNode.UncheckAll();
            }
        }

        private string GetExportPath(string basePath)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(basePath);
            sb.Append("\\");
            sb.Append(SelectedHotelcode);
            sb.Append("\\");
            sb.Append(CurrentTime.ToString("dd-MM-yyy"));

            string path = sb.ToString();
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            return path;
        }

        private ReportPathHolder[] GetCheckedReportPaths()
        {
            List<TreeListNode> nodeList = treeListReports.GetAllCheckedNodes();
            if (nodeList == null || nodeList.Count == 0) return null;
            ReportPathHolder[] pathList = new ReportPathHolder[nodeList.Count];
            int index = 0;

            TreeListColumn colCategory = treeListReports.Columns["category"];
            TreeListColumn colCode = treeListReports.Columns["code"];
            TreeListColumn colUrl= treeListReports.Columns["url"];

            foreach (TreeListNode node in nodeList)
            {
                ReportPathHolder pathHolder = new ReportPathHolder();
                if (node.HasChildren) continue;

                pathHolder.SavePath = @"" + node.GetValue(colCategory) + "\\" + node.GetValue(colCode);
                pathHolder.UrlPath = @"" + node.GetValue(colCategory) + "\\" + node.GetValue(colUrl);
                pathHolder.Name = node.GetDisplayText(0);
                pathHolder.Code = node.GetValue(colCode) as string;
                pathList[index] = pathHolder;

                index++;
            }


            return pathList;
        }

        private class ReportPathHolder
        {
            public string SavePath { get; set; }
            public string UrlPath { get; set; }
            public string Name { get; set; }
            public string Code { get; set; }
           
        }

        private string GetFullPath(TreeListNode node, string pathSeparator)
        {
            if (node == null) return "";
            string result = "";
            while (node != null)
            {
                result = pathSeparator + node.GetDisplayText(0) + result;
                node = node.ParentNode;
            }

            return result;
        }


        private bool PrintDocument(string path)
        {
            try
            {
                _streamReader= new StreamReader(path);
                try
                {
                    _printFont = new System.Drawing.Font("Arial", 10);
                    PrintDocument pd = new PrintDocument();
                    //pd.PrinterSettings
                    pd.PrintPage += new PrintPageEventHandler( pd_PrintPage);
                    pd.Print();
                }
                finally
                {
                    
                    _streamReader.Close();
                    
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        void pd_PrintPage(object sender, PrintPageEventArgs ev)
        {
            float linesPerPage = 0;
            float yPos = 0;
            int count = 0;
            float leftMargin = ev.MarginBounds.Left;
            float topMargin = ev.MarginBounds.Top;
            String line = null;

            // Calculate the number of lines per page.
            linesPerPage = ev.MarginBounds.Height /
               _printFont.GetHeight(ev.Graphics);

            // Iterate over the file, printing each line.
            while (count < linesPerPage &&
               ((line = _streamReader.ReadLine()) != null))
            {
                yPos = topMargin + (count * _printFont.GetHeight(ev.Graphics));
                ev.Graphics.DrawString(line, _printFont, Brushes.Black,
                   leftMargin, yPos, new StringFormat());
                count++;
            }

            // If more lines exist, print another page.
            if (line != null)
                ev.HasMorePages = true;
            else
                ev.HasMorePages = false;
        }

        private void PopulateSalesSummary()
        {
            try
            {
                List<SalesSummaryVM> dtoList = new List<SalesSummaryVM>();
                gcSummarizeTransaction.DataSource = null;
                gcSummarizeTransaction.RefreshDataSource();

                //get round value configuration
                var config = LocalBuffer.ConfigurationBufferList.FirstOrDefault(c => c.Reference == CNETConstantes.CASHSALESSUMMRY.ToString() && c.Attribute.ToLower() == "Round Digit Unit Price");
                if (config != null)
                    _roundSalesSummary = Convert.ToInt32(config.CurrentValue);

                //CNETInfoReporter.WaitForm("Populating sales summary", "Please Wait...");

                /*DateTime? CurrentTime2 = CommonLogics.GetServiceTime(true);
                if (CurrentTime2 == null)
                {
                    wpSummarizeTransaction.AllowNext = false;
                    return;
                }
                CurrentTime = CurrentTime2.Value;
                 */

                _salesSummaryList = GetSalesSummaryByDate(CurrentTime,SelectedHotelcode.Value);
                if (_salesSummaryList == null || _salesSummaryList.Count == 0)
                {
                    wpSummarizeTransaction.AllowNext = true;
                    //CNETInfoReporter.Hide();
                    return;
                }
                else
                {
                    wpSummarizeTransaction.AllowNext = false;
                }

                

                
                var totalRebate = UIProcessManager.GetTotalRebate(CurrentTime.Date,SelectedHotelcode);
                if (totalRebate != null)
                {
                    _roomDiscount = totalRebate.totalTaxable == null ? 0 : totalRebate.totalTaxable.Value;
                    _roomDiscountTax = totalRebate.totalTax == null ? 0 : totalRebate.totalTax.Value;
                }

                decimal subtotal = _salesSummaryList.Sum(s => s.taxableAmt == null ? 0: s.taxableAmt.Value);
                decimal serCharge = _salesSummaryList.Sum(s =>s.serviceCharge == null? 0: s.serviceCharge.Value);
                decimal discount = _salesSummaryList.Sum(s => s.discount == null ? 0 : s.discount.Value) + _roomDiscount;
                decimal vat = _salesSummaryList.Sum(s => s.tax == null ? 0 : s.taxAmt.Value) - _roomDiscountTax;
                decimal grandTotal = _salesSummaryList.Sum(s =>s.totalAmt ==null ? 0:  s.totalAmt.Value);

                teDiscount.Text = string.Format("{0:N}", Math.Round(discount, _roundSalesSummary));
                teSubTotal.Text = string.Format("{0:N}", Math.Round(subtotal, _roundSalesSummary));
                teSerCharge.Text = string.Format("{0:N}", Math.Round(serCharge, _roundSalesSummary));
                teVAT.Text = string.Format("{0:N}", Math.Round(Math.Abs(vat ), _roundSalesSummary));
                teGrandTotal.Text = string.Format("{0:N}", Math.Round(grandTotal + (vat) - discount + serCharge, _roundSalesSummary));
                


                int counter = 1;
                foreach (var sale in _salesSummaryList)
                {
                    SalesSummaryVM dto = new SalesSummaryVM()
                    {
                        SN = counter,
                        ArticleName = sale.articleName,
                        Code = sale.articleCode,
                        Quantitiy = Math.Round(sale.totalQuantity.Value, _roundSalesSummary),
                        TotalAmount = Math.Round( sale.totalAmt.Value, _roundSalesSummary),
                        UnitAmount = Math.Round(sale.UnitAmt.Value, 2)
                    };

                    dtoList.Add(dto);

                    counter++;
                }
                gcSummarizeTransaction.DataSource = dtoList;
                gvSummarizeTransaction.RefreshData();

                //CNETInfoReporter.Hide();
            }
            catch (Exception ex)
            {
                wpSummarizeTransaction.AllowNext = true;
                //CNETInfoReporter.Hide();
                SystemMessage.ShowModalInfoMessage("Error in populating  sales summary. Detail: " + ex.Message, "ERROR");
            }
        }

       

        private void PopulateJournalize()
        {
            //pnlJournalize.Controls.Clear();
            //PostingRoutineHeaderDTO prHeader = ACCUIProcessManager.GetPostingRoutineHeadersByComponent(CNETConstantes.PMS_Pointer).FirstOrDefault(p => p.code == _postingRoutineConfig) ;
            //if(prHeader == null)
            //{
            //    SystemMessage.ShowModalInfoMessage("Unable to get posting routine Header", "ERROR");
            //    return;
            //}
            ////Assign Finish Wizard Delegate
            //FinishJournalizeWizard finishJournalizeWizard = FinishJournalizeWizard;
            //_journalizeWizard = new JournalizeWizard(prHeader, true, finishJournalizeWizard);
            //_journalizeWizard.Dock = DockStyle.Fill;
            //pnlJournalize.Controls.Add(_journalizeWizard);
            //wpJournalizeRevenue.AllowNext = false;
        }

        private void FinishJournalizeWizard()
        {
            wpJournalizeRevenue.AllowNext = true;
            wcNightAudit.SetNextPage(); 
        }

        // F and B
        private void PopulateOrderStations()
        {
            try
            {
                //CNETInfoReporter.WaitForm("Reading unique order station Map .... ", "Please Wait..", 1, 5);
                List<GetAllOrderStationMapView> orderStioList = new List<GetAllOrderStationMapView>();
                orderStioList = new List<GetAllOrderStationMapView>();
                GetAllOrderStationMapView orderStation = new GetAllOrderStationMapView();
                List<GetAllOrderStationMapView> orderStations = new List<GetAllOrderStationMapView>();
                orderStations = new List<GetAllOrderStationMapView>();
                //CNETInfoReporter.WaitForm("Reading All Product Consumption.... ", "Please Wait..", 2, 5);
                orderStioList = UIProcessManager.GetUniqueOrderStationByPoSMachine();

                List<DeviceDTO> branchdevice =  UIProcessManager.GetDeviceByConsigneeunit(SelectedHotelcode.Value);

                List<int> referecelist = branchdevice.Select(x=> x.Id).ToList();
                orderStioList = orderStioList.Where(x=> referecelist.Contains(x.stationDevice.Value)).ToList(); 

                foreach (var xx in orderStioList)
                {
                    orderStation = new GetAllOrderStationMapView();
                    orderStation.stationDevice = xx.stationDevice;
                    orderStation.posDevice = xx.posDevice;
                    orderStation.name = xx.name;
                    orderStations.Add(orderStation);
                }
                //CNETInfoReporter.WaitForm("Reading All Product Consumption.... ", "Please Wait..", 5, 5);

                gcFandBVouchers.DataSource = orderStations;
                gvFandBVouchers.RefreshData();

                //CNETInfoReporter.Hide();
            }
            catch (Exception ex)
            {
                //CNETInfoReporter.Hide();
                SystemMessage.ShowModalInfoMessage("Error in populating order station map. Detail:: " + ex.Message, "ERROR");
                
            }
        }

        private List<ArticleConsumption> MapArticleConsumption(List<OrderStationConsumption> consumptions)
        {
            List<ArticleConsumption> consumtionArticleConsumptionses = new List<ArticleConsumption>();
            consumtionArticleConsumptionses = new List<ArticleConsumption>();
            ArticleConsumption ariclecon = new ArticleConsumption();
            foreach (var cc in consumptions)
            {
                foreach (var bb in cc.consumptionSummary)
                {

                    ariclecon = new ArticleConsumption();
                    ariclecon.ArticleCode = bb.ArticleCode;
                    ariclecon.ArticleName = bb.ArticleName;
                    ariclecon.qtyConsumed = bb.qtyConsumed;
                    ariclecon.totalCostOfConsumption = bb.totalCostOfConsumption;
                    ariclecon.unitCost = bb.unitCost;
                    consumtionArticleConsumptionses.Add(ariclecon);
                }
            }
            return consumtionArticleConsumptionses;
        }

        private void GetSataionDefaultStore()
        {

            var stationSettings = LocalBuffer.ConfigurationBufferList.Where(x=> x.Reference == Station.ToString()).ToList();
            if (stationSettings.Count > 0)
            {

                var stationSetting = stationSettings.Where(s => s.Attribute == "Default Store").FirstOrDefault();
                if (stationSetting != null)
                {

                    string  DefualtStore = stationSetting.CurrentValue;
                    if (DefualtStore != "")
                    {
                        var Store = UIProcessManager.SelectAllConsigneeUnit();
                        if (Store.Count > 0)
                        {
                            var Stor = Store.Where(d => d.Description == DefualtStore).FirstOrDefault();
                            if (Stor != null)
                            {
                                DefualtStoreId = Stor.Id;
                            }
                        }
                    }

                }

            }
        }

    

        private bool SaveFandB()
        {
            try
            {
                if (gvFandBLineItems.RowCount <= 0) return false;

                string CurrentVoucherNo = "";

                DialogResult result =
                    XtraMessageBox.Show("Are you sure to save row material consamption voucher?",
                        "CNET_ERP2016", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                if (result == DialogResult.No) return false;

                if (Station != null)
                {
                    bool isSaved = false;
                    string defaultStore = "";
                    decimal grandTotal = 0;
                    VoucherDTO voucher = new VoucherDTO();
                    List<LineItemBuffer> lineItems = new List<LineItemBuffer>();
                    //CNETInfoReporter.WaitForm("Reading Order Station Store Mapping .... ", "Please Wait..", 1, 8);
                    GetSataionDefaultStore();
                    if (DefualtStoreId == null)
                    {
                        SystemMessage.ShowModalInfoMessage("Please Set Default store for staion " + StationName);
                    }
                    //CNETInfoReporter.WaitForm("Reading Current Id From Database .... ", "Please Wait..", 1, 8);

                    string idd = UIProcessManager.IdGenerater("Voucher",CNETConstantes.ROW_MATERIAL_CONSUMPTION_VOUCHER,1,LocalBuffer.CurrentDeviceConsigneeUnit.Value,false, LocalBuffer.CurrentDevice.Id);

                    if (!string.IsNullOrEmpty(idd))
                    {
                        CurrentVoucherNo = idd;
                    }
                    else
                    {
                        //CNETInfoReporter.Hide();
                        SystemMessage.ShowModalInfoMessage("There is a problem on id setting!!!", "ERROR");
                        return false; ;
                    }

                    if (OrderStationConsumptionByStion.Count > 0)
                    {
                        //CNETInfoReporter.WaitForm("Extracting Line Items From Order station map list .... ",
                        
                        lineItems = new List<LineItemBuffer>();

                        foreach (var orders in OrderStationConsumptionByStion)
                        {
                            foreach (var order in orders.consumptionSummary)
                            {
                                LineItemBuffer lineItemBuff = new LineItemBuffer();
                                lineItemBuff.LineItem = new LineItemDTO();
                                lineItemBuff.LineItem.Article = order.ArticleId;
                                lineItemBuff.LineItem.UnitAmount = Convert.ToDecimal(order.unitCost);
                                lineItemBuff.LineItem.Quantity = order.qtyConsumed;
                                lineItemBuff.LineItem.TotalAmount = Convert.ToDecimal(order.totalCostOfConsumption);
                                int uom = CNETConstantes.UNITOFMEASURMENTPCS;
                                if (AllArticle != null && AllArticle.Count > 0)
                                {
                                    ArticleDTO art = AllArticle.FirstOrDefault(x => x.Id == lineItemBuff.LineItem.Article);
                                    if (art != null)
                                    {
                                        uom = art.Uom;
                                    }
                                }
                                lineItemBuff.LineItem.Uom = uom;
                                lineItemBuff.LineItem.TaxableAmount = 0;
                                lineItemBuff.LineItem.TaxAmount = 0;
                                lineItemBuff.LineItem.CalculatedCost = Convert.ToDecimal(order.totalCostOfConsumption);
                                lineItems.Add(lineItemBuff);
                                grandTotal += Convert.ToDecimal(lineItemBuff.LineItem.TotalAmount);
                            }
                        }
                        //CNETInfoReporter.WaitForm("Checking Voucher Id Exixtance.... ", "Please Wait..", 3, 8);
                        //var voucherIsExist = UIProcessManager.GetvoucheryId(CurrentVoucherNo);
                        //if (voucherIsExist != null)
                        //{
                        //    XtraMessageBox.Show("Voucher No:-" + CurrentVoucherNo + " is exist", "CNET_ERP2016",
                        //        MessageBoxButtons.OK, MessageBoxIcon.Error);
                        //    //CNETInfoReporter.Hide();
                        //    return true;
                        //}
                        VoucherBuffer voucherBuffer = new VoucherBuffer();
                        voucherBuffer.Voucher = new VoucherDTO();
                        voucherBuffer.Voucher.Code = CurrentVoucherNo;
                        voucherBuffer.Voucher.Type = CNETConstantes.TRANSACTIONTYPENORMALTXN;
                        voucherBuffer.Voucher.Definition = CNETConstantes.ROW_MATERIAL_CONSUMPTION_VOUCHER;
                        voucherBuffer.Voucher.IssuedDate = CurrentTime;
                        voucherBuffer.Voucher.IsIssued = true;
                        voucherBuffer.Voucher.Year = CurrentTime.Year;
                        voucherBuffer.Voucher.Month = CurrentTime.Month;
                        voucherBuffer.Voucher.Date = CurrentTime.Day; 
                        voucherBuffer.Voucher.GrandTotal = grandTotal;
                        voucherBuffer.Voucher.SubTotal = grandTotal;
                        voucherBuffer.Voucher.AdditionalCharge = 0;
                        voucherBuffer.Voucher.Discount = 0;
                        voucherBuffer.Voucher.SourceStore = DefualtStoreId;
                        voucherBuffer.Voucher.HasEffect = true;
                        voucherBuffer.Voucher.LastState = _rowConsumptionWorkflow.State;
                        voucherBuffer.LineItemsBuffer = new List<LineItemBuffer>();
                        voucherBuffer.LineItemsBuffer = lineItems;
                        voucherBuffer.Activity = ActivityLogManager.SetupActivity(CurrentTime, _rowConsumptionWorkflow.Id, CNETConstantes.PMS_Pointer, "F and B Consumption Analysis during night audit");

                        //CNETInfoReporter.WaitForm("Saving Voucher.... ", "Please Wait..", 4, 8);
                        if (UIProcessManager.CreateVoucher(voucher) != null)
                        {
                            SystemMessage.ShowModalInfoMessage("Voucher Saved Successfully!!");
                            
                        }
                        else
                            SystemMessage.ShowModalInfoMessage("Voucher Not Saved !!");

                    } 
                }
                else
                {
                    SystemMessage.ShowModalInfoMessage("Please Select Order station !!", "ERROR"); 
                }
                
                //CNETInfoReporter.Hide();
                return false;
            }
            catch (Exception ex)
            {
                //CNETInfoReporter.Hide();
                SystemMessage.ShowModalInfoMessage("Error in saving F and B. Detail:: " + ex.Message, "ERROR");
                return false;
            }
        }

        #endregion

        /************* Event Handlers **************/
        #region Event Handlers

        void bWorker_DoWork(object sender, DoWorkEventArgs e)
        {
         /*
          //  _frmReport = new frmPMSReports(true);
            
            if (reportArchiveConfig == null)
            {
                // XtraMessageBox.Show("Unable to get Report Archive save path. Please check your configuration on Server Management.", "CNET_2016", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            ReportPathHolder[] pathList = GetCheckedReportPaths();
            if (pathList == null) return;

            int totalCount = pathList.Length;
            int counter = 0;
            int lastPerc = 0;
            string baseDir = GetExportPath(reportArchiveConfig.currentValue);
            UserStateHolder userState = new UserStateHolder();
            foreach (ReportPathHolder pHolder in pathList)
            {
                if (pHolder.Code != "Rp111111111")
                {
                    counter = counter + 1;
                    decimal value = (decimal)counter / (decimal)totalCount;
                    int percentage = Convert.ToInt32(value * (decimal)100);
                    int eachPerc = percentage / 3;
                    userState.code = pHolder.Code;
                    try
                    {

                        userState.Status = 0;
                        bWorker.ReportProgress(eachPerc * 1 + lastPerc, userState);
                        _frmReport.SelectedHotelcode = SelectedHotelcode;
                        _frmReport.ClearGrid();
                        _frmReport.ReportShow(CurrentTime, pHolder.Code);


                        userState.Status = 1;
                        bWorker.ReportProgress(eachPerc * 2 + lastPerc, userState);
                        string defaultPrinter = string.Empty;
                        if (defaultPrinterConfig != null)
                        {
                            defaultPrinter = defaultPrinterConfig.currentValue;
                            if (reportArchivePrint != null)
                            {
                                bool isPrint = Convert.ToBoolean(reportArchivePrint.currentValue);
                                if (!isPrint)
                                    defaultPrinter = null;
                            }
                        }


                        bool result = _frmReport.ExportToPdf(baseDir + @"\" + pHolder.Name + ".pdf", pHolder.Name, CurrentTime, defaultPrinter);


                        if (result)
                        {
                            //bWorker.ReportProgress(4, pHolder.Code);
                            //PrintDocument(baseDir + @"\" + CurrentTime.ToString("dd-MM-yyy") + ".pdf");

                            //done
                            userState.Status = 2;

                            bWorker.ReportProgress(eachPerc * 3 + lastPerc, userState);
                            lastPerc = eachPerc * 3 + lastPerc;
                            Thread.Sleep(3000);
                        }

                    }
                    catch (Exception ex)
                    {
                        userState.Status = 3;
                        bWorker.ReportProgress(eachPerc * 3 + lastPerc, userState);
                        Thread.Sleep(3000);
                    }

                }
                else
                {
                    _frmReport.ExporteDashBoardReportToPdf(baseDir + @"\Dashboard Report.pdf", "Dash Board Report", CurrentTime);
                }
            }
          
*/

        }

        private class UserStateHolder
        {
            public int Status { get; set; }
            public string code { get; set; }
        }

        void bWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

            int percentage = e.ProgressPercentage;
            UserStateHolder userState = (UserStateHolder)e.UserState;
            if (userState.Status == 0)
            {
                UpdateArchiveStatus(userState.code, "Preparing");
            }
            else if (userState.Status == 1)
            {
                UpdateArchiveStatus(userState.code, "Exporting");
            }
            else if (userState.Status == 2)
            {
                UpdateArchiveStatus(userState.code, "Done");
            }
            else if (userState.Status == 3)
            {
                UpdateArchiveStatus(userState.code, "Failed");
            }
            else if (userState.Status == 4)
            {
                UpdateArchiveStatus(userState.code, "Printing");
            }

            progressBarReports.EditValue = percentage;
        }

        void bWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            wpReportArchive.AllowNext = true;
            progressBarReports.EditValue = 100;
        }

        private void gvFandBVouchers_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            try
            {
               
                //CNETInfoReporter.WaitForm("Assigning order station .... ", "Please Wait..", 1, 5);
                var station = (GetAllOrderStationMapView)gvFandBVouchers.GetFocusedRow();
                OrderStationConsumptionByStion = new List<OrderStationConsumption>();
                if (station != null)
                {
                    Station = station.stationDevice;
                    StationName = station.name;
                    OrderStationConsumptionByStion.Clear();
                    gcFandBLineItems.DataSource = OrderStationConsumptionByStion;
                    gvFandBLineItems.RefreshData();

                    if (OrderStationConsumptionList != null && OrderStationConsumptionList.Count > 0)
                    {
                        //CNETInfoReporter.WaitForm("Reading assiged Product for each station", "Please Wait..", 2, 5);
                        OrderStationConsumptionByStion =
                            OrderStationConsumptionList.Where(
                                d => d.orderStation.stationDevice.ToString() == station.stationDevice.ToString())
                                .ToList();
                        List<ArticleConsumption> lists = MapArticleConsumption(OrderStationConsumptionByStion);
                        gcFandBLineItems.DataSource = lists;
                        gvFandBLineItems.RefreshData();
                    }
                    else
                    {
                        //CNETInfoReporter.WaitForm("Reading assiged Product for station", "Please Wait..", 2, 5);
                        OrderStationConsumptionList = UIProcessManager.GetConsumptionCalculation(shift.Value, CurrentTime, organazationUnitDefn, station.stationDevice.Value,2, true);

                        OrderStationConsumptionByStion =
                            OrderStationConsumptionList.Where(
                                d => d.orderStation.stationDevice.ToString() == station.stationDevice.ToString())
                                .ToList();
                        List<ArticleConsumption> lists = MapArticleConsumption(OrderStationConsumptionByStion);
                        gcFandBLineItems.DataSource = lists;
                        gvFandBLineItems.RefreshData();

                    }
                }
                //CNETInfoReporter.WaitForm("Reading assiged Product for station" + StationName, "Please Wait..", 5, 5);
                //CNETInfoReporter.Hide();
            }
            catch (Exception ex)
            {

                //CNETInfoReporter.Hide();
                SystemMessage.ShowModalInfoMessage("Error in reading assigned product for station. Detail:: " + ex.Message, "ERROR");
            }
            
        }

        private void bbiJournalize_ItemClick(object sender, ItemClickEventArgs e)
        {
            //if(_journalizeWizard != null)
            //{
            //    _journalizeWizard.JournalizePost();
            //}
        }

        private void GridRowShadeBasedOnState(object sender, DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs e)
        {
            GridView View = sender as GridView;


            if (e.RowHandle >= 0)
            {
                bool isCharged = (bool)View.GetRowCellValue(e.RowHandle, "isCharged");
                if (!isCharged)
                {
                    // wpRoomTaxPosting.AllowNext = false;

                    View.SelectRow(e.RowHandle);
                    View.Appearance.FocusedRow.BackColor = ColorTranslator.FromHtml("SkyBlue");
                    View.Appearance.FocusedRow.ForeColor = Color.Black;


                    View.Appearance.FocusedCell.BackColor = ColorTranslator.FromHtml("SkyBlue");
                    View.Appearance.FocusedCell.ForeColor = Color.Black;

                    View.Appearance.SelectedRow.BackColor = ColorTranslator.FromHtml("SkyBlue");
                    View.Appearance.SelectedRow.ForeColor = Color.Black;

                    e.Appearance.ForeColor = Color.Black;

                }
                else
                {
                    //It is posted for this registration

                    View.Appearance.FocusedRow.BackColor = ColorTranslator.FromHtml("SkyBlue");
                    View.Appearance.FocusedRow.ForeColor = Color.Blue;


                    View.Appearance.FocusedCell.BackColor = ColorTranslator.FromHtml("SkyBlue");
                    View.Appearance.FocusedCell.ForeColor = Color.Blue;

                    View.Appearance.SelectedRow.BackColor = ColorTranslator.FromHtml("SkyBlue");
                    View.Appearance.SelectedRow.ForeColor = Color.Blue;

                    e.Appearance.ForeColor = Color.Blue;
                    View.UnselectRow(e.RowHandle);
                }
            }
        }

        private void bbiSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (wcNightAudit.SelectedPage == wpFandB)
                {
                    SaveFandB();
                }
                else if (wcNightAudit.SelectedPage == wpSummarizeTransaction)
                {

                    VoucherBuffer VoucherBuffer = new VoucherBuffer();
                    String id = null;
                    id = UIProcessManager.IdGenerater("Voucher",CNETConstantes.CASHSALESSUMMRY,1,LocalBuffer.CurrentDeviceConsigneeUnit.Value,false,LocalBuffer.CurrentDevice.Id);
                    if (!string.IsNullOrEmpty(id))
                    {
                        VoucherBuffer.Voucher.Code = id;
                        teVoucherNo.Text = id;
                    }
                    else
                    {
                        SystemMessage.ShowModalInfoMessage("There is a problem on id setting!!!", "ERROR");
                        return;
                    }

                    if (_isSalesSummarySaved)
                    {
                        SystemMessage.ShowModalInfoMessage("Sales Summary is already Saved!", "ERROR");
                        return;
                    }
                    if (_salesSummaryList == null || _salesSummaryList.Count == 0)
                    {
                        SystemMessage.ShowModalInfoMessage("No sales summary to save!", "ERROR");
                        return;
                    }

                    //CNETInfoReporter.WaitForm("Saving Sales Summary", "Please Wait...");
                    //vou.code = UIProcessManager.GetCurrentIdByDevice("Voucher", device, CNETConstantes.CASHSALESSUMMRY.ToString(), CNETConstantes.VOUCHER_COMPONENET);
                    VoucherBuffer.Voucher.Definition = CNETConstantes.CASHSALESSUMMRY;
                    VoucherBuffer.Voucher.Type = CNETConstantes.TRANSACTIONTYPENORMALTXN; 
                    VoucherBuffer.Voucher.IssuedDate = CurrentTime;
                    VoucherBuffer.Voucher.Year = CurrentTime.Year;
                    VoucherBuffer.Voucher.Month = CurrentTime.Month;
                    VoucherBuffer.Voucher.Date = CurrentTime.Day;
                    VoucherBuffer.Voucher.IsIssued = _salesSummaryWorkflow.IssuingEffect;
                    VoucherBuffer.Voucher.IsVoid = false;
                    VoucherBuffer.Voucher.LastState = _salesSummaryWorkflow.State;
                    VoucherBuffer.Voucher.Period = LocalBuffer.GetPeriodCode(CurrentTime);
                    VoucherBuffer.Voucher.GrandTotal = !string.IsNullOrEmpty(teGrandTotal.Text) ? Convert.ToDecimal(teGrandTotal.Text) : 0;

                    VoucherBuffer.LineItemsBuffer = new List<LineItemBuffer>();
                    foreach (var sales in _salesSummaryList)
                    {
                        LineItemBuffer lineItemBuffer = new LineItemBuffer();
                        lineItemBuffer.LineItem = new LineItemDTO()
                        {
                            Article = sales.Articleid,
                            UnitAmount = sales.UnitAmt,
                            Quantity = sales.totalQuantity == null ? 0 : sales.totalQuantity.Value,
                            Uom = CNETConstantes.UNITOFMEASURMENTPCS,
                            TotalAmount = sales.totalAmt,
                            TaxableAmount = sales.taxableAmt == null ? 0 : sales.taxableAmt.Value,
                            Tax = sales.tax,
                            TaxAmount = sales.taxAmt.Value,
                            CalculatedCost = sales.calcCost == null ? 0 : sales.calcCost.Value,
                            Remark = "Sales Summary in Night Audit"
                        };
                       
                        ArticleDTO art = UIProcessManager.GetArticleById(sales.Articleid);
                       
                        if (sales.discount != null && sales.discount.Value > 0 && sales.serviceCharge != null && sales.serviceCharge.Value > 0)
                        {
                            lineItemBuffer.LineItemValueFactors = new List<LineItemValueFactorDTO>();
                            if (sales.discount != null && sales.discount.Value > 0)
                            {
                                LineItemValueFactorDTO liva = new LineItemValueFactorDTO()
                                {
                                    IsDiscount = true,
                                    Amount = sales.discount.Value
                                };
                                if (art != null && art.Preference == LocalBuffer.ACCOMODATION_PREFERENCE_CODE && _roomDiscount >0)
                                {
                                    liva.Amount += _roomDiscount;
                                }
                                lineItemBuffer.LineItemValueFactors.Add(liva);

                            }

                            if (sales.serviceCharge != null && sales.serviceCharge.Value > 0)
                            {

                                LineItemValueFactorDTO liva = new LineItemValueFactorDTO()
                                {
                                    IsDiscount = false,
                                    Amount = sales.serviceCharge.Value
                                };
                                lineItemBuffer.LineItemValueFactors.Add(liva);
                            }
                        }
                        VoucherBuffer.LineItemsBuffer.Add(lineItemBuffer);
                    }

                    VoucherBuffer.Voucher.AdditionalCharge = _salesSummaryList.Sum(s => s.serviceCharge == null ? 0 : s.serviceCharge);
                    VoucherBuffer.Voucher.Discount = _salesSummaryList.Sum(s => s.discount == null ? 0 : s.discount.Value) + _roomDiscount;
                    VoucherBuffer.Voucher.SubTotal = _salesSummaryList.Sum(s => s.taxableAmt == null ? 0 : s.taxableAmt.Value);

                    VoucherBuffer.TaxTransactions= new List<TaxTransactionDTO>(); 
                     
                    var taxGrouped = _salesSummaryList.GroupBy(s => s.tax).Select(s => new
                    {
                        taxType = s.First().tax,
                        taxableAmount = s.Sum(a => a.taxableAmt == null ? 0 : a.taxableAmt.Value),
                        taxAmount = s.Sum(a => a.taxAmt == null ? 0 : a.taxAmt.Value)
                    }).ToList();
                    if (taxGrouped != null && taxGrouped.Count > 0)
                    {
                        foreach (var tx in taxGrouped)
                        {
                            TaxTransactionDTO tTran = new TaxTransactionDTO()
                            {
                                TaxType = tx.taxType,
                                TaxableAmount = tx.taxableAmount,
                                TaxAmount = tx.taxAmount
                            };

                            //Substract Discount Tax
                            if (tx.taxType == CNETConstantes.VAT)
                                tTran.TaxAmount = Math.Abs((tTran.TaxAmount == null ? 0 :tTran.TaxAmount.Value) - _roomDiscountTax);

                            VoucherBuffer.TaxTransactions.Add(tTran);
                        }
                    }
                    VoucherBuffer.Activity = ActivityLogManager.SetupActivity(CurrentTime, _salesSummaryWorkflow.Id, CNETConstantes.CASHSALESSUMMRY, "Night Audit Sales Summary");


                    if (UIProcessManager.CreateVoucherBuffer(VoucherBuffer) != null)
                    { 
                        SystemMessage.ShowModalInfoMessage("Summary is saved successfully!", "MESSAGE");
                        _isSalesSummarySaved = true;
                        wpSummarizeTransaction.AllowNext = true;

                    }
                    else
                    {
                        SystemMessage.ShowModalInfoMessage("Summary is not saved!", "ERROR");
                    }

                    //CNETInfoReporter.Hide();
                }
            }
            catch (Exception ex)
            {
                //CNETInfoReporter.Hide();
                SystemMessage.ShowModalInfoMessage("Error in saving sales summary. DETAIL: " + ex.Message, "ERROR");
            }
        }

        private bool IsBucketChecked(int regId, DateTime date)
        {
             List<ActivityDTO> activityList = UIProcessManager.GetActivityByreference(regId);
             if (activityList != null)
             {
                ActivityDTO activity = activityList.FirstOrDefault(a => a.ActivityDefinition == adBucketCheck.Id && a.TimeStamp.Value.Date == date.Date);
                 if (activity == null)
                     return false;
                 else
                     return true;
             }
             return false;
        }

        private void wcNightAudit_NextClick(object sender, DevExpress.XtraWizard.WizardCommandButtonClickEventArgs e)
        {
            #region Over Due Out

            if (e.Page == wpOverDueOut)
            {
                if (gvOverDueOUt.RowCount > 0)
                {
                    wpOverDueOut.AllowNext = false;
                }
                else
                {
                    wpOverDueOut.AllowNext = true;
                }
            }

            #endregion

            

            if (e.Page == wpBucketCheck)
            {
                List<RateCodeAudit> selectedRateCodeAudits = new List<RateCodeAudit>();
                int[] selectedRows = gvBucketCheck.GetSelectedRows();
                if (selectedRows != null && selectedRows.Length > 0)
                {
                    for (int i = 0; i < selectedRows.Length; i++)
                    {
                        RateCodeAudit rca = gvBucketCheck.GetRow(selectedRows[i]) as RateCodeAudit;
                        selectedRateCodeAudits.Add(rca);
                    }

                    //CNETInfoReporter.WaitForm("Saving activity", "Please Wait...");
                    try
                    {

                        foreach (var rcAudit in selectedRateCodeAudits)
                        {

                            if (IsBucketChecked(rcAudit.RegistrationId, CurrentTime)) continue;

                            ActivityDTO activity = ActivityLogManager.SetupActivity(CurrentTime, adBucketCheck.Id, CNETConstantes.PMS_Pointer, "Bucket Checked");
                            activity.Reference = rcAudit.RegistrationId;
                            UIProcessManager.CreateActivity(activity);
                        }

                        isBucketChecked = true;
                        //CNETInfoReporter.Hide();
                    }
                    catch (Exception ex)
                    {
                        XtraMessageBox.Show("Exception has occurred in saving activity for bucket check. DETAIL::" + ex.Message, "CNET_v2016", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        isBucketChecked = false;
                        //CNETInfoReporter.Hide();
                    }

                    
                }
               

            }
            else if (e.Page == wpReportArchive)
            {
                //if (!isReporteArchived)
                //{
                //    DialogResult dr = XtraMessageBox.Show("No Report is archived. Do you want to continue to the next page?", "CNET_v2016", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                //    if (dr == DialogResult.No)
                //    {
                //        wcNightAudit.SetPreviousPage();
                //    }

                //}
            }

        }
        
        private void wcNightAudit_FinishClick(object sender, CancelEventArgs e)
        {
            
            this.Close();
        }

        private void bbiAmendDate_ItemClick(object sender, ItemClickEventArgs e)
        {
            RegistrationListVMDTO selectedReg = new RegistrationListVMDTO();
           
            if (wcNightAudit.SelectedPage == wpOverDueOut)
            {
                 selectedReg = (RegistrationListVMDTO)gvOverDueOUt.GetFocusedRow();

            }
            else if (wcNightAudit.SelectedPage == wpDueOut)
            {
                 selectedReg = (RegistrationListVMDTO)gvDueOut.GetFocusedRow();

            }
            else if (wcNightAudit.SelectedPage == wpWaitinglist)
            {
                 selectedReg = (RegistrationListVMDTO)gvWaitinglist.GetFocusedRow();

            }
            else if (wcNightAudit.SelectedPage == wp6PM)
            {
                 selectedReg = (RegistrationListVMDTO)gv6PM.GetFocusedRow();

            }
            else if (wcNightAudit.SelectedPage == wpGuaranted)
            {
                 selectedReg = (RegistrationListVMDTO)gvGuaranted.GetFocusedRow();

            }
            else if (wcNightAudit.SelectedPage == wpBucketCheck)
            {
                selectedReg = (RegistrationListVMDTO)gvBucketCheck.GetFocusedRow();

            }
            else if (wcNightAudit.SelectedPage == wpRoomTaxPosting)
            {
                DailyRoomVoucherVM selectedRegRoomVoucherDto = (DailyRoomVoucherVM)gvRoomTaxPostingMaster.GetFocusedRow();
                if (selectedRegRoomVoucherDto != null)
                {
                    VoucherDTO regEx =
                        UIProcessManager.GetVoucherById(selectedRegRoomVoucherDto.registrationId);
                    if (regEx!=null)
                    {
                        selectedReg.Arrival = regEx.StartDate.Value;
                        selectedReg.Departure = regEx.EndDate.Value;
                    }
                    selectedReg.Registration = selectedRegRoomVoucherDto.registrationNo;
                    selectedReg.Room = selectedRegRoomVoucherDto.roomNo;
                    selectedReg.Guest = selectedRegRoomVoucherDto.consignee;
                }

            }
            if (selectedReg != null && selectedReg.Registration != null)
            {
                frmDateAmendment frmDAte = new frmDateAmendment();
                frmDAte.SelectedHotelcode = SelectedHotelcode.Value;
                frmDAte.RegistrationExt = selectedReg;
                frmDAte.IsFromNightAudit = true;
                if (frmDAte.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    PopulateGridData(wcNightAudit.SelectedPage);
                }
            }
            else
            {
                SystemMessage.ShowModalInfoMessage("No registration is selected!!!", "ERROR");
            }
        }

        private void bbiCheckOut_ItemClick(object sender, ItemClickEventArgs e)
        {
            bool isZeroCheckout = false;
            RegistrationListVMDTO selectedReg = new RegistrationListVMDTO();
            if (wcNightAudit.SelectedPage == wpOverDueOut)
            {
                selectedReg = (RegistrationListVMDTO)gvOverDueOUt.GetFocusedRow();

            }
            else if (wcNightAudit.SelectedPage == wpDueOut)
            {
                selectedReg = (RegistrationListVMDTO)gvDueOut.GetFocusedRow();

            }
            if (selectedReg != null)
            {
                List<int> charges = UIProcessManager.GetDailyRoomVoucherByReg(selectedReg.Id, CurrentTime);
                //Check any daily room charge post for this registration
                if (charges == null || charges.Count == 0)
                {
                    var transferedBills = UIProcessManager.GetTransferBill(selectedReg.Id, null, true); 
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

                ActivityDTO activity = null;
                ActivityDefinitionDTO ad = UIProcessManager.GetActivityDefinitionBydescriptionandreference(CNETConstantes.LU_ACTIVITY_DEFINATION_REINSTATE,CNETConstantes.REGISTRATION_VOUCHER).FirstOrDefault();
                if (ad != null)
                {
                    activity = UIProcessManager.GetActivityByreferenceandactivityDefinition(selectedReg.Id, ad.Id).FirstOrDefault();
                }
                if (activity != null)
                {
                    // if the remark is "With Receipt Reprint"
                    if (activity.Remark == "With receipt reprint")
                    {

                        if (PerfomCheckout(selectedReg, false, isZeroCheckout, true))
                        {
                            PopulateGridData(wcNightAudit.SelectedPage);
                        }
                    }

                    // if the remark is "Without Receipt Reprint"
                    else
                    {
                        if (PerfomCheckout(selectedReg, true, isZeroCheckout, true))
                        {
                            PopulateGridData(wcNightAudit.SelectedPage);
                        }
                    }
                }

                    // if there is NO activity with "Reinstate".
                else
                {

                    if (PerfomCheckout(selectedReg, false, isZeroCheckout))
                    {
                        PopulateGridData(wcNightAudit.SelectedPage);
                    }
                }
            }
        }

        private void bbiCheckIn_ItemClick(object sender, ItemClickEventArgs e)
        {
            RegistrationListVMDTO selectedReg = new RegistrationListVMDTO();
            if (wcNightAudit.SelectedPage == wpWaitinglist)
            {
                selectedReg = (RegistrationListVMDTO)gvWaitinglist.GetFocusedRow();

            }
            else if (wcNightAudit.SelectedPage == wp6PM)
            {
                selectedReg = (RegistrationListVMDTO)gv6PM.GetFocusedRow();

            }
            else if (wcNightAudit.SelectedPage == wpGuaranted)
            {
                selectedReg = (RegistrationListVMDTO) gvGuaranted.GetFocusedRow();
            }
            if (selectedReg != null)
            {
                DateTime? currentTime = UIProcessManager.GetServiceTime();
                if (currentTime == null)
                    return;

                if (selectedReg.Arrival.Date != currentTime.Value.Date)
                {
                    SystemMessage.ShowModalInfoMessage("You can not check in a guest whose arrival date is not equal to today.", "ERROR");
                    return;
                }
                else
                {
                    var osdBuffer = LocalBuffer.ObjectStateDefinitionBufferList.FirstOrDefault(osd => osd.Id == selectedReg.lastState);

                    if (selectedReg.lastState != CNETConstantes.CHECKED_IN_STATE)
                    {
                        if (selectedReg.NoOfRoom > 1)
                        {

                            DialogResult dr = MessageBox.Show(
                                @"This is multiple rooms check in.Do you want continue changing  " + (osdBuffer == null ? "Current" : osdBuffer.Description) +
                                " state to Check In ?", "State Change Conformation",
                                MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);
                            if (dr == DialogResult.Yes)
                            {
                                frmMultipleRoomCheckIn frmMultipleRoomCheckIn = new frmMultipleRoomCheckIn(true);
                                frmMultipleRoomCheckIn.SelectedHotelcode = SelectedHotelcode.Value;
                                frmMultipleRoomCheckIn.RegExtension = selectedReg;
                                frmMultipleRoomCheckIn.IsFromNightAudit = true;
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
                                frmCheckIn.SelectedHotelcode = SelectedHotelcode.Value;
                                frmCheckIn.RegExtension = selectedReg;
                                if (frmCheckIn.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                                {
                                    PopulateGridData(wcNightAudit.SelectedPage);
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

        private void bbiCancel_ItemClick(object sender, ItemClickEventArgs e)
        {
            RegistrationListVMDTO selectedReg = new RegistrationListVMDTO();
            if (wcNightAudit.SelectedPage == wpWaitinglist)
            {
                selectedReg = (RegistrationListVMDTO)gvWaitinglist.GetFocusedRow();

            }
            else if (wcNightAudit.SelectedPage == wp6PM)
            {
                selectedReg = (RegistrationListVMDTO)gv6PM.GetFocusedRow();

            }
            else if (wcNightAudit.SelectedPage == wpGuaranted)
            {
                selectedReg = (RegistrationListVMDTO)gvGuaranted.GetFocusedRow();

            }
            if (selectedReg != null)
            {
                DialogResult dr = MessageBox.Show("Do you want to cancel this registration?", "Night Audit", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                if (dr == DialogResult.Yes)
                {
                    VoucherBuffer voucherbuffer = UIProcessManager.GetVoucherBufferById(selectedReg.Id);
                    if (voucherbuffer == null)
                    {
                        SystemMessage.ShowModalInfoMessage("Unble to get voucher information of the selected registration. Please try agin!", "ERROR");
                        return;
                    }


                    List<TransactionReferenceDTO> transactionReference = UIProcessManager.GetTransactionReferenceByreferenced(selectedReg.Id);
                      
                    if (selectedReg.lastState == CNETConstantes.GAURANTED_STATE && transactionReference.Count > 0)
                    {
                        DialogResult dResult =
                                 MessageBox.Show(@"This registration has active transaction and can not be cancelled.Do you want to change to NO-SHOW State?",
                                     @"State Change Conformation", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                        if (dResult == DialogResult.Yes)
                        {
                            ActivityDefinitionDTO adActivityDefinition = UIProcessManager.GetActivityDefinitionBydescriptionandreference( CNETConstantes.LU_ACTIVITY_DEFINATION_NOSHOW,CNETConstantes.REGISTRATION_VOUCHER).FirstOrDefault();
                            
                            if (adActivityDefinition == null)
                            {
                                SystemMessage.ShowModalInfoMessage("Please Define Workflow of NO-SHOW for REGISTRATION Voucher!", "ERROR");
                                return;
                            }
                            voucherbuffer.Voucher.LastState = CNETConstantes.NO_SHOW_STATE;
                            voucherbuffer.Activity = ActivityLogManager.SetupActivity(CurrentTime, adActivityDefinition.Id, CNETConstantes.PMS_Pointer );

                            if (UIProcessManager.UpdateVoucherBuffer(voucherbuffer) != null)
                            { 
                                XtraMessageBox.Show("Successfully Changed to No-Show State", "Successfull Operation", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                PopulateGridData(wcNightAudit.SelectedPage);
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
                        frmCancel.RegExtension = selectedReg;
                        frmCancel.IsFromNightAudit = true;
                        if (frmCancel.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            PopulateGridData(wcNightAudit.SelectedPage);
                        }

                    }
                }
            }
        }

        
        private void bbiPush_ItemClick(object sender, ItemClickEventArgs e)
        {
            HeldTransaction heldTran = (HeldTransaction)gvClearHeldBills.GetFocusedRow();
            try
            {
                if (heldTran != null)
                {
                    //CNETInfoReporter.WaitForm("Pushing...");
                    VoucherBuffer voucherbuffer = UIProcessManager.GetVoucherBufferById(heldTran.VoucherId);

                    if (voucherbuffer == null)
                    {
                        //CNETInfoReporter.Hide();
                        SystemMessage.ShowModalInfoMessage("unable to get voucher!", "ERROR");
                        return;
                    }

                    var pushWorkflow = UIProcessManager.GetActivityDefinitionBydescriptionandreference(CNETConstantes.LU_ACTIVITY_DEFINATION_PUSHED, voucherbuffer.Voucher.Definition.Value).FirstOrDefault();

                    var voDef = LocalBuffer.SystemConstantDTOBufferList.FirstOrDefault(a => a.Id == voucherbuffer.Voucher.Definition);

                    if (pushWorkflow == null)
                    {
                        SystemMessage.ShowModalInfoMessage(string.Format("Please Define Workflow of PUSHED for {0} Voucher!", voDef == null ? "For The Current": voDef.Description.ToUpper()), "ERROR");
                        //CNETInfoReporter.Hide();
                        return;
                    }

                    List<ActivityDTO> actList = UIProcessManager.GetActivityByreferenceandactivityDefinition(heldTran.VoucherId, pushWorkflow.Id).ToList();
                    if (actList.Count == 0)
                    {

                        voucherbuffer.Activity = ActivityLogManager.SetupActivity(CurrentTime, pushWorkflow.Id, CNETConstantes.PMS_Pointer);
                        voucherbuffer.Voucher.IssuedDate = voucherbuffer.Voucher.IssuedDate.AddDays(1);
                        voucherbuffer.Voucher.LastState = pushWorkflow.State;
                        if (UIProcessManager.UpdateVoucherBuffer(voucherbuffer) != null)
                        {
                            SystemMessage.ShowModalInfoMessage("The bill pushed successfully!!!", "MESSAGE");
                            PopulateGridData(wcNightAudit.SelectedPage);
                        }
                        else
                        {
                            SystemMessage.ShowModalInfoMessage("Operation not successful!!!", "ERROR");
                        }
                    }
                    else
                    {
                        SystemMessage.ShowModalInfoMessage("It is already pushed !!!", "ERROR");
                    }
                    //CNETInfoReporter.Hide();
                }

            }
            catch (Exception ex)
            {
                SystemMessage.ShowModalInfoMessage("Error in pushing bill. DETAIL::" + ex.Message, "ERROR");
                //CNETInfoReporter.Hide();
            }
        }

        private void bbiVoid_ItemClick(object sender, ItemClickEventArgs e)
        {
            HeldTransaction selected = (HeldTransaction)gvClearHeldBills.GetFocusedRow();
            if (selected != null)
            {
                DialogResult result = XtraMessageBox.Show("Do you want to void this bill?", "Void Bill", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == System.Windows.Forms.DialogResult.No) return;
                try
                {
                    //CNETInfoReporter.WaitForm("Pushing...");
                    VoucherBuffer voucherbuffer = UIProcessManager.GetVoucherBufferById(selected.VoucherId);

                    if (voucherbuffer == null)
                    {
                        //CNETInfoReporter.Hide();
                        SystemMessage.ShowModalInfoMessage("unable to get voucher!", "ERROR");
                        return;
                    }


                    var voidWorkflow = UIProcessManager.GetActivityDefinitionBydescriptionandreference(CNETConstantes.LU_ACTIVITY_DEFINATION_VOID, voucherbuffer.Voucher.Definition.Value).FirstOrDefault();

                    var voDef = LocalBuffer.SystemConstantDTOBufferList.FirstOrDefault(a => a.Id == voucherbuffer.Voucher.Definition);



                    if (voidWorkflow == null)
                    {
                        SystemMessage.ShowModalInfoMessage(string.Format("Please Define Workflow of VOID for {0} Voucher!"), voDef == null ? "For the Current" : voDef.Description.ToUpper(), "ERROR");
                        //CNETInfoReporter.Hide();
                        return;
                    }

                    voucherbuffer.Activity = ActivityLogManager.SetupActivity(CurrentTime, voidWorkflow.Id, CNETConstantes.PMS_Pointer);

                    voucherbuffer.Voucher.LastState = voidWorkflow.State;
                    voucherbuffer.Voucher.IsVoid = true; 
                    if (UIProcessManager.UpdateVoucherBuffer(voucherbuffer) != null)
                    {
                        SystemMessage.ShowModalInfoMessage("The bill is void successfully!!!", "MESSAGE");
                        PopulateHeldBills();
                    }
                    else
                    {
                        SystemMessage.ShowModalInfoMessage("Operation not successful!!!", "ERROR");
                    }


                    //CNETInfoReporter.Hide();
                }
                catch (Exception ex)
                {
                    SystemMessage.ShowModalInfoMessage("Error in making bill VOID.DETAIL::" + ex.Message, "ERROR");
                    //CNETInfoReporter.Hide();
                }
            }
        }


        private void wcNightAudit_SelectedPageChanged(object sender, DevExpress.XtraWizard.WizardPageChangedEventArgs e)
        {
            if (e.Page == welcomeWizardPage1 || e.Page == completionWizardPage1)
            {
                rcNightAudit.Visible = false;

            }
            else
            {
                rcNightAudit.Visible = true;
            }

            HandleWizarPageChange(e.Page);

            dockPanel_attachments.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Hidden;
            if (e.Page == wpBucketCheck)
            {
                dockPanel_attachments.Visibility = DevExpress.XtraBars.Docking.DockVisibility.AutoHide;
            }
            
            if(_isJournalizationEnabled && e.Page == wpJournalizeRevenue)
            {
                PopulateJournalize();
            }

            PopulateGridData(e.Page);

            if (e.Page == wpReportArchive)
            {
                
                wpReportArchive.AllowBack = true;

            }
            else if (e.Page == completionWizardPage1)
            {

                string period = sleAuditDate.EditValue.ToString();

                //ClosingValidation val = new ClosingValidation
                //{
                //    Component = CNETConstantes.PMS,
                //    Period = period,
                //    Status = 1,
                //    Device = LocalBuffer.DeviceObject.code,
                //    User = LocalBuffer.CurrentLoggedInUser.code,
                //    Remark = SelectedHotelcode

                //};
                //UIProcessManager.CreateClosingValidation(val);

                //CNETInfoReporter.WaitForm("Changing room status...", "Please Wait", 1, 2);
                bool flag = ChangeAllRoomsState();
                if (!flag)
                {
                    XtraMessageBox.Show("WARNING: Room(s) status has not been changed.", "CNET_v2016", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                //CNETInfoReporter.WaitForm("Incrementing current date...", "Please Wait", 2, 2);
                //flag = UIProcessManager.IncrementEventBasedDate();
                //if (!flag)
                //{
                //    XtraMessageBox.Show("WARNING: Current date has not been changed", "CNET_v2016", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                //}

                //CNETInfoReporter.Hide();

                DateTime? currentDate = UIProcessManager.GetServiceTime();
                string dateMessage = "";
                if (currentDate != null)
                {
                    dateMessage = string.Format("Current date is changed to: {0}", currentDate.Value.ToString("dd-MM-yyyy"));
                }

                lblFinish.Text = "You have successfully finished night audit process! " + dateMessage;
            }

            
            ControlNextButton(e.Page);
            
        }

        private void wcNightAudit_SelectedPageChanging(object sender, WizardPageChangingEventArgs e)
        {
            if (e.Page == wpOverDueOut)
            {
                rcNightAudit.Visible = true;
            }
        }

        private void bbiPostAll_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (dailyRoomChargeList == null || dailyRoomChargeList.Count == 0)
            {
                SystemMessage.ShowModalInfoMessage("nothing to post!", "ERROR");
                return;
            }
            if (_chargeToPost == null || _chargeToPost.Count == 0)
            {
                SystemMessage.ShowModalInfoMessage("nothing to post!", "ERROR");
                return;
            }

            List<DailyRoomChargeDTO> filtered = dailyRoomChargeList.Where(d => _chargeToPost.Contains(d.registrationVoucher)).ToList();

            if (filtered == null || filtered.Count == 0)
            {
                SystemMessage.ShowModalInfoMessage("nothing to post!", "ERROR");
                return;
            }
           // OrganizationUnit HotelOrgunit = LocalBuffer.OrganizationUnitBufferList.FirstOrDefault(x=> x.reference == dailyRoomChargeList.reg);
        //    MessageBox.Show("check and implement");
            CommonLogics.PostRoomCharge(filtered, CurrentTime, CurrentDevice, SelectedHotelcode.Value);
           
            PopulateGridData(wcNightAudit.SelectedPage);
        }

        private void bbiRefresh_ItemClick(object sender, ItemClickEventArgs e)
        {
            PopulateGridData(wcNightAudit.SelectedPage);
        }

        private void bbiArchiveReport_ItemClick(object sender, ItemClickEventArgs e)
        {
            isReporteArchived = true;
            progressBarReports.EditValue = 0;
          /*  if (!bWorker.IsBusy)
            {
                bWorker.RunWorkerAsync();
            }*/


            ArchiveReport();

        }

        private void ArchiveReport()
        {
            if (reportArchiveConfig == null)
            {
                // XtraMessageBox.Show("Unable to get Report Archive save path. Please check your configuration on Server Management.", "CNET_2016", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            ReportPathHolder[] pathList = GetCheckedReportPaths();
            if (pathList == null) return;

            int totalCount = pathList.Length;
            int counter = 0;
            int lastPerc = 0;
            string baseDir = GetExportPath(reportArchiveConfig.CurrentValue);
            UserStateHolder userState = new UserStateHolder();
            foreach (ReportPathHolder pHolder in pathList)
            {
                if (pHolder.Code != "Rp111111111")
                {
                    counter = counter + 1;
                    decimal value = (decimal)counter / (decimal)totalCount;
                    int percentage = Convert.ToInt32(value * (decimal)100);
                    int eachPerc = percentage / 3;
                    userState.code = pHolder.Code;
                    try
                    {

                        userState.Status = 0;
                        //  bWorker.ReportProgress(eachPerc * 1 + lastPerc, userState);
                        ReportProgressChanged(userState, eachPerc * 2 + eachPerc * 1 + lastPerc);
                        _frmReport.SelectedHotelcode = SelectedHotelcode;
                        _frmReport.ClearGrid();
                        _frmReport.ReportShow(CurrentTime, pHolder.Code);


                        userState.Status = 1;
                        //  bWorker.ReportProgress(eachPerc * 2 + lastPerc, userState);
                        ReportProgressChanged(userState,eachPerc * 2 + lastPerc);
                        string defaultPrinter = string.Empty;
                        if (defaultPrinterConfig != null)
                        {
                            defaultPrinter = defaultPrinterConfig.CurrentValue;
                            if (reportArchivePrint != null)
                            {
                                bool isPrint = Convert.ToBoolean(reportArchivePrint.CurrentValue);
                                if (!isPrint)
                                    defaultPrinter = null;
                            }
                        }


                        bool result = _frmReport.ExportToPdf(baseDir + @"\" + pHolder.Name + ".pdf", pHolder.Name, CurrentTime, defaultPrinter);


                        if (result)
                        {
                            //bWorker.ReportProgress(4, pHolder.Code);
                            //PrintDocument(baseDir + @"\" + CurrentTime.ToString("dd-MM-yyy") + ".pdf");

                            //done
                            userState.Status = 2;

                         //   bWorker.ReportProgress(eachPerc * 3 + lastPerc, userState);
                            lastPerc = eachPerc * 3 + lastPerc;
                            ReportProgressChanged(userState, lastPerc);
                          //  Thread.Sleep(3000);
                        }

                    }
                    catch (Exception ex)
                    {
                        userState.Status = 3;
                     //   bWorker.ReportProgress(eachPerc * 3 + lastPerc, userState);
                        ReportProgressChanged(userState, eachPerc * 3 + lastPerc);
                       // Thread.Sleep(3000);
                    }

                }
                else
                {
                    _frmReport.ExporteDashBoardReportToPdf(baseDir + @"\Dashboard Report.pdf", "Dash Board Report", CurrentTime);
                }
            }
        }
        void ReportProgressChanged(UserStateHolder userState, int ProgressPercentage)
        {

            //int percentage = e.ProgressPercentage;
            //UserStateHolder userState = (UserStateHolder)e.UserState;
            if (userState.Status == 0)
            {
                UpdateArchiveStatus(userState.code, "Preparing");
            }
            else if (userState.Status == 1)
            {
                UpdateArchiveStatus(userState.code, "Exporting");
            }
            else if (userState.Status == 2)
            {
                UpdateArchiveStatus(userState.code, "Done");
            }
            else if (userState.Status == 3)
            {
                UpdateArchiveStatus(userState.code, "Failed");
            }
            else if (userState.Status == 4)
            {
                UpdateArchiveStatus(userState.code, "Printing");
            }

            progressBarReports.EditValue = ProgressPercentage;
        }

        private void UpdateArchiveStatus(string reportCode, string status)
        {
            List<ReportDTO> allReports = treeListReports.DataSource as List<ReportDTO>;
            if (allReports == null) return;
            ReportDTO report = allReports.FirstOrDefault(r => r.Code == reportCode);
            report.Remark = status;
            treeListReports.DataSource = allReports;
            treeListReports.RefreshDataSource();
        }

        private void bbiOpenPdf_ItemClick(object sender, ItemClickEventArgs e)
        {
            //ListBoxItem lbi = listBxReports.SelectedItem as ListBoxItem;
            //if (lbi != null)
            //{
            //    CNETPdfViewer pdfView = new CNETPdfViewer();
            //    pdfView.LoadPdf(lbi.Value.ToString());
            //    pdfView.Show();
            //}
        }

        private void bbiBrowsePdf_ItemClick(object sender, ItemClickEventArgs e)
        {
            //ListBoxItem lbi = listBxReports.SelectedItem as ListBoxItem;
            //if (lbi != null)
            //{
            //    string fullPath = lbi.Value.ToString();
            //    if (string.IsNullOrEmpty(fullPath)) return;
            //    string[] splited = fullPath.Split('\\');
            //    StringBuilder sb = new StringBuilder();
            //    if (splited != null)
            //    {
            //        for (int i = 0; i < (splited.Length - 1); i++)
            //        {
            //            sb.Append(splited[i]);
            //            if (i != (splited.Length - 2))
            //                sb.Append("\\");

            //        }
            //    }

            //    string folderPath = sb.ToString();

            //    System.Diagnostics.Process.Start(@folderPath);

            //}
        }

        private void ceBx_CheckedChanged(object sender, EventArgs e)
        {

            TreeListNodes nodeList = treeListReports.Nodes;
            if (nodeList != null)
            {
                foreach (TreeListNode node in nodeList)
                {
                    if (ceBx.Checked)
                    {
                        node.CheckAll();
                    }
                    else
                    {
                        node.UncheckAll();
                    }
                }
            }
        }

        private void gvBucketCheck_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            if (e.Column.Caption == "SN")
                e.DisplayText = (e.RowHandle + 1).ToString();

            if (e.Column.Caption == "Variance")
            {
                try
                {
                    //long value = Convert.ToInt64(e.DisplayText);
                    if (e.DisplayText != "0.00")
                    {
                        e.Appearance.BackColor = ColorTranslator.FromHtml("Red");
                        e.Appearance.ForeColor = ColorTranslator.FromHtml("white");
                    }
                }
                catch (Exception ex)
                {

                }
            }
        }

        private void gvBucketCheck_RowStyle_1(object sender, RowStyleEventArgs e)
        {
            //GridView view = sender as GridView;

            ////string regVoucher = gridView_regDoc.GetFocusedRowCellDisplayText("Registration");
            //RateCodeAudit regListVM = (RateCodeAudit)view.GetFocusedRow();
            //if (regListVM == null) return;

            //gvBucketCheck.Appearance.FocusedRow.ForeColor = ColorTranslator.FromHtml(regListVM.Color);
            //gvBucketCheck.Appearance.FocusedCell.ForeColor = ColorTranslator.FromHtml(regListVM.Color);
            //gvBucketCheck.Appearance.SelectedRow.ForeColor = ColorTranslator.FromHtml(regListVM.Color);
            //e.Appearance.ForeColor = ColorTranslator.FromHtml(regListVM.Color);

           


        }

        private void wcNightAudit_CustomizeCommandButtons(object sender, CustomizeCommandButtonsEventArgs e)
        {
            e.PrevButton.Visible = false;
            e.CancelButton.Visible = false;
        }

        private void gv_DataSourceChanged(object sender, EventArgs e)
        {
            ControlNextButton(wcNightAudit.SelectedPage);
        }

        private void frmAuditNightAll_Load(object sender, EventArgs e)
        {
            if (!InitializeData())
            {
                wcNightAudit.Enabled = false;
            }

        }
        private void gv_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            if (e.Column.Caption == "SN")
                e.DisplayText = (e.RowHandle +1).ToString();
        }


        private void KeepSelection(object sender, MouseEventArgs e)
        {
            try
            {
                GridView view = sender as GridView;
                DevExpress.XtraGrid.Views.Grid.ViewInfo.GridHitInfo hi = view.CalcHitInfo(e.Location);
                if (hi.InRow)
                {
                    if (hi.Column.FieldName != "DX$CheckboxSelectorColumn")
                    {
                        int[] selectedRows = view.GetSelectedRows();
                        bool isSelected = view.IsRowSelected(hi.RowHandle);
                        BeginInvoke(new System.Action(() =>
                        {
                            for (int i = 0; i < selectedRows.Length; i++)
                                view.SelectRow(selectedRows[i]);
                            if (!isSelected) view.UnselectRow(hi.RowHandle);
                        }));
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void gvBucketCheck_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            if (gvBucketCheck.RowCount == gvBucketCheck.SelectedRowsCount)
            {
                wpBucketCheck.AllowNext = true;
            }
            else
            {
                wpBucketCheck.AllowNext = false;
            }
        }

        private void bbiNoShow_ItemClick(object sender, ItemClickEventArgs e)
        {
            RegistrationListVMDTO model = gvGuaranted.GetFocusedRow() as RegistrationListVMDTO;
            if(model == null) return;

            frmNoShow noShowForm = new frmNoShow();
            noShowForm.RegExtension = model;
            noShowForm.IsFromNightAudit = true;
            if (noShowForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                PopulateGridData(wcNightAudit.SelectedPage);
            }
        }

        private void treeListReports_SelectionChanged(object sender, EventArgs e)
        {
           
        }

        private void treeListReports_NodeChanged(object sender, NodeChangedEventArgs e)
        {
            //if (treeListReports.GetAllCheckedNodes().Count > 0)
            //{
            //    wpReportArchive.AllowNext = true;
            //}
            //else
            //{
            //    wpReportArchive.AllowNext = false;

            //}
        }

        private void treeListReports_CustomDrawNodeCell(object sender, CustomDrawNodeCellEventArgs e)
        {
            if (e.Column.Caption == "Status")
            {
                if (e.CellValue == null) return;
                
                if (e.CellValue.ToString() == "Preparing")
                {
                    e.Appearance.ForeColor = ColorTranslator.FromHtml("Orange");
                }
                else if (e.CellValue.ToString() == "Exporting")
                {
                    e.Appearance.ForeColor = ColorTranslator.FromHtml("Blue");
                }

                else if (e.CellValue.ToString() == "Done")
                {
                    e.Appearance.ForeColor = ColorTranslator.FromHtml("Green");
                }

                else if (e.CellValue.ToString() == "Failed")
                {
                    e.Appearance.ForeColor = ColorTranslator.FromHtml("Red");
                }

                else if (e.CellValue.ToString() == "Printing")
                {
                    e.Appearance.ForeColor = ColorTranslator.FromHtml("Cyan");
                }

                
            }
        }

        private void bbiPrint_ItemClick(object sender, ItemClickEventArgs e)
        {
            BaseWizardPage page = wcNightAudit.SelectedPage;
           /* ReportGenerator repGen = new ReportGenerator();
            #region Overdueouts
            if (page == wpOverDueOut)
            {

               
               repGen.GenerateGridReport(gcOverDueOut, "Over Due Outs", CurrentTime.ToShortDateString());
                
            }

            #endregion

            #region DueOuts
            else if (page == wpDueOut)
            {
                repGen.GenerateGridReport(gcDueOut, "Due Outs", CurrentTime.ToShortDateString());
            }

            #endregion

            #region Waiting
            else if (page == wpWaitinglist)
            {
                repGen.GenerateGridReport(gcWaitinglist, "Waiting List", CurrentTime.ToShortDateString());
            }

            #endregion

            #region 6PM
            else if (page == wp6PM)
            {
                repGen.GenerateGridReport(gc6PM, "Six-PM Reservations", CurrentTime.ToShortDateString());
            }

            #endregion

            #region Guaranteed
            else if (page == wpGuaranted)
            {
                repGen.GenerateGridReport(gcGauranted, "Guaranteed Reservations", CurrentTime.ToShortDateString());
            }

            #endregion

            #region Room Discripancy

            else if (page == wpRoomDiscipancy)
            {
                repGen.GenerateGridReport(gcRoomDiscripacy, "Room Discripancies", CurrentTime.ToShortDateString());
            }

            #endregion

            #region Clear Held Bills
            else if (page == wpClearHeldBill)
            {
                repGen.GenerateGridReport(gcClearHeldBills, "Held Bills", CurrentTime.ToShortDateString());
            }

            #endregion

            #region Bucket Check
            else if (page == wpBucketCheck)
            {
                repGen.GenerateGridReport(gcBucketCheck, "Bucket Checks", CurrentTime.ToShortDateString());
            }

            #endregion

            #region Room and Tax Posting
            else if (page == wpRoomTaxPosting)
            {
                repGen.GenerateGridReport(gcRoomTaxPosting, "Room and Tax Postings", CurrentTime.ToShortDateString());
            }

            #endregion

            */
        }

        private void gv6PM_PrintInitialize(object sender, DevExpress.XtraGrid.Views.Base.PrintInitializeEventArgs e)
        {

        }

        private void gv6PM_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            GridView view = sender as GridView;

            if (e.Column.Caption == "SN")
            {
                e.DisplayText = (e.RowHandle + 1).ToString();

                var dto = view.GetRow(e.RowHandle) as RegistrationListVMDTO;
                if(dto != null)
                {
                    dto.SN = e.DisplayText;
                }

            }
        }

        private void gcRoomDiscripacy_Click(object sender, EventArgs e)
        {

        }

        private void gvFandBVouchers_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            if (e.Column.Caption == "SN")
            {
                e.DisplayText = (e.RowHandle + 1).ToString();
            }
        }

        private void bbiExportPDF_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                if (_currentGrid == null) return;
                SaveFileDialog fileDialog = new SaveFileDialog()
                {
                    Title = "PDF Export",
                    DefaultExt = "pdf",
                    Filter = "*.pdf|*.pdf",
                    FileName = "Exported_Registrations"
                };
                if (fileDialog.ShowDialog() == DialogResult.OK)
                {
                    _currentGrid.ExportToPdf(fileDialog.FileName);
                }


            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Error in exporting posting routine. Detail:: " + ex.Message, "Posting Routine", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        private void bbiExportExcel_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                if (_currentGrid == null) return;
                SaveFileDialog fileDialog = new SaveFileDialog()
                {
                    Title = "Excel Export",
                    DefaultExt = "xls",
                    Filter = "*.xls|*.xls",
                    FileName = "Exported_Registrations"
                };
                if (fileDialog.ShowDialog() == DialogResult.OK)
                {
                    _currentGrid.ExportToXls(fileDialog.FileName);
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Error in exporting posting routine. Detail:: " + ex.Message, "Posting Routine", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        private void bbiExportCSV_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                if (_currentGrid == null) return;
                SaveFileDialog fileDialog = new SaveFileDialog()
                {
                    Title = "CVS Export",
                    DefaultExt = "csv",
                    Filter = "*.csv|*.csv",
                    FileName = "Exported_Registrations"
                };
                if (fileDialog.ShowDialog() == DialogResult.OK)
                {
                    _currentGrid.ExportToCsv(fileDialog.FileName);
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Error in exporting posting routine. Detail:: " + ex.Message, "Posting Routine", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
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

                reportArchiveConfig = null;
                defaultPrinterConfig = null;

                if (_regListVM != null)
                {
                    _regListVM.Clear();
                    _regListVM = null;
                }

                if (dailyRoomChargeList != null)
                {
                    dailyRoomChargeList.Clear();
                    dailyRoomChargeList = null;
                }

                if (_roomTypeList != null)
                {
                    _roomTypeList.Clear();
                    _roomTypeList = null;
                }

                if (_rateCodeAuditList != null)
                {
                    _rateCodeAuditList.Clear();
                    _rateCodeAuditList = null;
                }

                if (_frmReport != null)
                {
                    _frmReport.Dispose();
                    _frmReport = null;
                }

                //if (bWorker != null)
                //{
                //    bWorker.Dispose();
                //    bWorker = null;
                //}


                _printFont = null;
                CurrentUser = null;
                CurrentDevice = null;
                CurrentUser = null;
                adBucketCheck = null;

            }
            base.Dispose(disposing);
        }
        private void gv_DoubleClick(object sender, EventArgs e)
        {
            GridView view = sender as GridView;
            if (view == null) return;

            var dto = view.GetFocusedRow() as RegistrationListVMDTO;
            if (dto == null) return;

            frmFolio _frmFolio = new frmFolio();
            _frmFolio.SelectedHotelcode = SelectedHotelcode.Value;
            _frmFolio.RegistrationExt = dto;

            _frmFolio.ShowDialog(this);
        }
        //private CNET_Attachments atta = null;
        private void dockPanel_attachments_Expanding(object sender, DevExpress.XtraBars.Docking.DockPanelCancelEventArgs e)
        {
            //RateCodeAudit dto = gvBucketCheck.GetFocusedRow() as RateCodeAudit;
            //if (dto == null) return;

            //try
            //{
            //    //CNETInfoReporter.WaitForm("Loading Attachments", "Please Wait...");

            //    List<Lookup> lookups = LocalBuffer.LookupBufferList.Where(l => l.type.ToLower() == "attachment catagory").ToList();
            //    if (lookups != null)
            //    {
            //        lookups = lookups.Where(c => c.code != CNETConstantes.ATTACHMENT_CATAGORY_COMPANYLOGO
            //            && c.code != CNETConstantes.ATTACHMENT_CATAGORY_CATALOGUE
            //            && c.code != CNETConstantes.ATTACHMENT_CATAGORY_MANUAL).ToList();
            //    }

            //    string code = dto.RegistrationNumber;

            //    atta = new CNET_Attachments(code,CNETConstantes.REGISTRATION_VOUCHER, true, lookups);
            //    atta.Dock = DockStyle.Fill;
            //    pnl_attachments.Controls.Clear();
            //    pnl_attachments.Controls.Add(atta);
            //    //CNETInfoReporter.Hide();

            //}
            //catch (Exception ex)
            //{
            //    //CNETInfoReporter.Hide();
            //    SystemMessage.ShowModalInfoMessage("Error in initializing attachments. Detail: " + ex.Message, "ERROR");
            //}
        }
        private void sleAuditDate_EditValueChanged(object sender, EventArgs e)
        {
            if (sleAuditDate.EditValue != null && !string.IsNullOrEmpty(sleAuditDate.EditValue.ToString()))
            {
                PeriodDTO selectedPeriod = LocalBuffer.PeriodBufferList.FirstOrDefault(x=> x.Id == Convert.ToInt32(sleAuditDate.EditValue));
                if (selectedPeriod != null)
                {
                    List<PeriodDTO> Previousperiod = AuditPeriodwithNoAuditdoneList.Where(x => x.Start < selectedPeriod.Start).ToList();
                    if (Previousperiod != null && Previousperiod.Count > 0)
                    {
                        MessageBox.Show("Please there are " + Previousperiod.Count + " days which Night Audit has not been Done " +
                            Environment.NewLine + "First Complete The Night Audit and then Select the Next date !!", "Night Audit", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        sleAuditDate.EditValue = AuditPeriodwithNoAuditdoneList.FirstOrDefault().Id;
                    }
                    else
                    {
                        CurrentTime = selectedPeriod.Start;
                        if (SelectedHotelcode != null)
                            welcomeWizardPage1.AllowNext = true;
                    }
                }


            }
            else
            {
                welcomeWizardPage1.AllowNext = false;
            }
        }
        private void sleAuditDate_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            LookUpEdit edit = sender as LookUpEdit;
            if (e.DisplayText != "")
            {
              PeriodDTO  Periods = LocalBuffer.PeriodBufferList.FirstOrDefault(x => x.Id == (int)e.Value);
              e.DisplayText = e.DisplayText + " ,   Date:- " + Periods.Start.ToShortDateString();    
            }
        }
        private void leHotel_EditValueChanged(object sender, EventArgs e)
        {
            SelectedHotelcode = leHotel.EditValue == null ? null : Convert.ToInt32(leHotel.EditValue);
            _roomTypeList = UIProcessManager.GetRoomTypeByConsigneeUnit(SelectedHotelcode.Value);
            GetandPopulatePeriodwithnoAuditDone(); 
        }
        public int? SelectedHotelcode { get; set; }

        public List<SalesSummarized> GetSalesSummaryByDate(DateTime CurrentTime, int SelectedHotelcode)
        {
            List<SalesSummarized> salesSummarized = null;
            List<VoucherDetailReportViewDTO> SalesFilter = UIProcessManager.GetVoucherDetailReportForSummary(CurrentTime, SelectedHotelcode);
            if (SalesFilter != null && SalesFilter.Count > 0)
            {
                salesSummarized = SalesFilter.GroupBy(x => new { x.ArticleId, x.ArticleCode, x.ArticleName, x.Tax }).Select(g => new
                SalesSummarized()
                {
                    Articleid = g.Key.ArticleId,
                    articleCode = g.Key.ArticleCode,
                    articleName = g.Key.ArticleName,
                    tax = g.Key.Tax.Value,
                    UnitAmt = g.Sum(u => u.UnitAmount) / (decimal)g.Sum(u => u.Quantity),
                    totalQuantity = g.Sum(u => u.Quantity),
                    totalAmt = g.Sum(u => u.TotalAmount),
                    taxableAmt = g.Sum(u => u.TaxableAmount),
                    taxAmt = g.Sum(u => u.TaxAmount),
                    discount = g.Sum(u => u.Discount),
                    serviceCharge = g.Sum(u => u.AdditionalCharge)
                }).ToList();
            }
            return salesSummarized;
        }


    }
    public class SalesSummarized
    {
        public int Articleid { get; set; }
        public string articleCode { get; set; }
        public string articleName { get; set; }
        public int tax { get; set; }
        public decimal? UnitAmt { get; set; }
        public double? totalQuantity { get; set; }
        public decimal? totalAmt { get; set; }
        public decimal? taxableAmt { get; set; }
        public decimal? taxAmt { get; set; }
        public decimal? calcCost { get; set; }
        public decimal? discount { get; set; }
        public decimal? serviceCharge { get; set; }
    }
}