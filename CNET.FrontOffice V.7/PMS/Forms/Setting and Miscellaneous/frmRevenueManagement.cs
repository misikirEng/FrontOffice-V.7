using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CNET.ERP.Client.Common.UI;
using CNET.FrontOffice_V._7.PMS.Contracts;
using CNET.FrontOffice_V._7.Forms.Setting_and_Miscellaneous.Revenue_Management_Modals;
using CNET.FrontOffice_V._7.Properties;
using DevExpress.XtraBars;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraTab;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.DXErrorProvider;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using CNET_V7_Domain.Domain.PmsSchema;
using CNET_V7_Domain.Domain.SettingSchema;
using ProcessManager;
using CNET_V7_Domain.Domain.CommonSchema;
using CNET.FrontOffice_V._7.PMS.Common_Classes;
using CNET.FrontOffice_V._7.Validation;
using CNET_V7_Domain.Domain.ConsigneeSchema;
using CNET_V7_Domain.Domain.ArticleSchema;
using CNET_V7_Domain.Domain.ViewSchema;
using CNET.Progress.Reporter;
using DocumentPrint;

namespace CNET.FrontOffice_V._7.Forms
{
    public partial class frmRevenueManagement : UILogicBase
    {


        private List<RateCategoryDTO> rateCatList = null;
        private List<RateCodeHeaderDTO> rateList = null;

        /** Rate Header Variables **/
        private IList<Control> _invalidControls;

        private List<RateRoomTypeVM> selectedRoomTypes = new List<RateRoomTypeVM>();
        private List<PackageVM> packageList = new List<PackageVM>();
        private List<RateRoomTypeVM> rDTOList = new List<RateRoomTypeVM>();


        /** Rate Availability Variables **/
        private DateTime startMarkDate;
        private DateTime endMarkDate;
        private DataTable tableAvailability = new DataTable();


        private List<SystemConstantDTO> rateCompList = null;

        private RepositoryItemLookUpEdit cacCode = new RepositoryItemLookUpEdit();


        //default lookup values
        private int _defRateComp;
        private int _defCurrency;
        private int _defXRule;
        private int _defBusSource;
        private int _defMarket;

       // private BackgroundWorker bWorker = new BackgroundWorker();

        //properties
        public DateTime CurrentTime { get; set; }

        //******************* CONSTRUCTOR ************************//
        public frmRevenueManagement()
        {
            InitializeComponent();

            InitializeUI();
        }


        #region Helper Methods 

        public void InitializeUI()
        {
            // CNETFooterRibbon.ribbonControl = ribbonControl1;
            // Utility.AdjustRibbon(lciRibbonContainer);

            ////Rate Header Code
            //cacCode.Columns.Add(new LookUpColumnInfo("Description", "Rate Code"));
            //cacCode.DisplayMember = "Description";
            //cacCode.ValueMember = "Id";
            //cacCode.NullText = "";

            //beiCode.Edit = cacCode;

            beiRateCodeHeader.Visibility = BarItemVisibility.Never;

            //Currency in Rate Header
            cacCurrencyRateHeader.Properties.Columns.Add(new LookUpColumnInfo("Description", "Description"));
            cacCurrencyRateHeader.Properties.DisplayMember = "Description";
            cacCurrencyRateHeader.Properties.ValueMember = "Id";

            //Exchange Rule
            cacExchangeRateHeader.Properties.Columns.Add(new LookUpColumnInfo("Description", "Description"));
            cacExchangeRateHeader.Properties.DisplayMember = "Description";
            cacExchangeRateHeader.Properties.ValueMember = "Id";


            //Rate Category
            cacRateCategoryRateHeader.Properties.Columns.Add(new LookUpColumnInfo("Description", "Category"));
            cacRateCategoryRateHeader.Properties.DisplayMember = "Description";
            cacRateCategoryRateHeader.Properties.ValueMember = "Id";

            //Business Source
            cacSourceRateHeader.Properties.Columns.Add(new LookUpColumnInfo("Description", "Description"));
            cacSourceRateHeader.Properties.DisplayMember = "Description";
            cacSourceRateHeader.Properties.ValueMember = "Id";

            //Market
            cacMarketRateHeader.Properties.Columns.Add(new LookUpColumnInfo("Description", "Description"));
            cacMarketRateHeader.Properties.DisplayMember = "Description";
            cacMarketRateHeader.Properties.ValueMember = "Id";

            //Rate Header Article List
            GridColumn column = cacArticleTransactionRateHeader.Properties.View.Columns.AddField("Id");
            column.Width = 50; column.MaxWidth = 50;
            column.Visible = true;
            column = cacArticleTransactionRateHeader.Properties.View.Columns.AddField("Name");
            column.Visible = true;
            cacArticleTransactionRateHeader.Properties.DisplayMember = "Name";
            cacArticleTransactionRateHeader.Properties.ValueMember = "Id";

            //componenet
            clbcComponentsRateHeader.DisplayMember = "Description";
            clbcComponentsRateHeader.ValueMember = "Id";


            bbiEdit.Visibility = BarItemVisibility.Always;
            gv_rateAvail.CustomRowCellEdit += frmRevenueManagement_CustomRowCellEdit;
            gv_rateAvail.CustomDrawCell += frmRevenueManagement_CustomDrawCell;
            gv_rateAvail.MouseMove += frmRevenueManagement_MouseMove;
            gv_rateAvail.KeyUp += frmRevenueManagement_KeyUp;
            gc_rateAvail.KeyUp += gridControlAvailability_KeyUp;
            //  tcRevenueManagementMain.SelectedPageChanged += tcRevenueManagementMain_SelectedPageChanged;
            RepositoryItemMemoExEdit extEdit = new RepositoryItemMemoExEdit();
            extEdit.TextEditStyle = TextEditStyles.DisableTextEditor;

            bbiNew.ItemClick += cdeAvailability_Click;

            //bWorker.WorkerReportsProgress = true;
            //bWorker.WorkerSupportsCancellation = true;
            //bWorker.DoWork += bWorker_DoWork;
            //bWorker.ProgressChanged += bWorker_ProgressChanged;
            //bWorker.RunWorkerCompleted += bWorker_RunWorkerCompleted;

            repositoryItemCheckEdit2.CheckedChanged += repositoryItemCheckEdit2_CheckedChanged;
            repositoryItemCheckEdit3.CheckedChanged += repositoryItemCheckEdit3_CheckedChanged;

            repositoryItemDateEdit1.EditValueChanged += repositoryItemDateEdit1_EditValueChanged;


        }
        private bool InitializeData()
        {
            try
            {
              //  Progress_Reporter.Show_Progress("Initializing Data......", "Please Wait.......");
                DateTime? currentTime = UIProcessManager.GetServiceTime();
                if (currentTime == null)
                {
                    return false;
                }
                CurrentTime = currentTime.Value;

                deBeginDateRateHeader.DateTime = CurrentTime;
                deBeginDateRateHeader.Properties.MinValue = CurrentTime;
                deEndDateRateHeader.DateTime = CurrentTime;

                beiYear.EditValue = currentTime.Value.Date;

                /** Check Workflow **/



                //rate componenet list
                rateCompList = LocalBuffer.LocalBuffer.SystemConstantDTOBufferList.Where(l => l.Category == CNETConstantes.RATE_COMPONENT && l.IsActive).ToList();
                clbcComponentsRateHeader.DataSource = rateCompList;
                if (rateCompList != null)
                {
                    var defRateComp = rateCompList.FirstOrDefault(r => r.IsDefault);
                    if (defRateComp != null)
                    {
                        _defRateComp = defRateComp.Id;

                    }
                }


                //Currency List
                List<CurrencyDTO> currencyList = LocalBuffer.LocalBuffer.CurrencyBufferList;
                cacCurrencyRateHeader.Properties.DataSource = currencyList;
                if (currencyList != null)
                {
                    var defCurrency = currencyList.FirstOrDefault(r => r.IsDefault);
                    if (defCurrency != null)
                    {
                        _defCurrency = defCurrency.Id;
                        cacCurrencyRateHeader.EditValue = _defCurrency;
                    }
                }

                //Exchange Rule
                List<SystemConstantDTO> exchangeRuleList = LocalBuffer.LocalBuffer.SystemConstantDTOBufferList.Where(l => l.Category == CNETConstantes.EXCHANGE_RULE && l.IsActive).ToList();
                cacExchangeRateHeader.Properties.DataSource = exchangeRuleList;
                if (exchangeRuleList != null)
                {
                    var xRule = exchangeRuleList.FirstOrDefault(x => x.IsDefault);
                    if (xRule != null)
                    {
                        _defXRule = xRule.Id;
                        cacExchangeRateHeader.EditValue = _defXRule;
                    }
                }

                //Business Source 
                List<LookupDTO> busSoureceList = LocalBuffer.LocalBuffer.LookUpBufferList.Where(l => l.Type == CNETConstantes.BUSSINESS_SOURCE && l.IsActive).ToList();
                cacSourceRateHeader.Properties.DataSource = (busSoureceList);
                if (busSoureceList != null)
                {
                    var busSrc = busSoureceList.FirstOrDefault(b => b.IsDefault);
                    if (busSrc != null)
                    {
                        _defBusSource = busSrc.Id;
                        cacSourceRateHeader.EditValue = _defBusSource;
                    }
                }

                //Market
                List<LookupDTO> marketList = LocalBuffer.LocalBuffer.LookUpBufferList.Where(l => l.Type == CNETConstantes.MARKET && l.IsActive).ToList();
                cacMarketRateHeader.Properties.DataSource = (marketList);
                if (marketList != null)
                {
                    var mar = marketList.FirstOrDefault(m => m.IsDefault);
                    if (mar != null)
                    {
                        _defMarket = mar.Id;
                        cacMarketRateHeader.EditValue = _defMarket;
                    }
                }


                // Articles
                List<ArticleDTO> artList = UIProcessManager.GetArticleByGSLType(CNETConstantes.SERVICES).Where(a => a.IsActive && a.Preference == LocalBuffer.LocalBuffer.ACCOMODATION_PREFERENCE_CODE).ToList();
                cacArticleTransactionRateHeader.Properties.DataSource = (artList);


                leHotel.Properties.DisplayMember = "Name";
                leHotel.Properties.ValueMember = "Id";
                leHotel.Properties.DataSource = LocalBuffer.LocalBuffer.HotelBranchBufferList;


                //reiHotel.DisplayMember = "Description";
                //reiHotel.ValueMember = "Id";
                //reiHotel.DataSource = (iOrgUnit.Select(x => new { x.code, x.description })).ToList();

                //beiHotel.EditValue = MasterPageForm.CurrentDeviceOrganizationUnit;
                //reiHotel.ReadOnly = !MasterPageForm.UserHasHotelBranchAccess;

                //load rate category
                PopulateRateCategory(false);

             //   Progress_Reporter.Close_Progress();
                return true;
            }
            catch (Exception ex)
            {
               // Progress_Reporter.Close_Progress();
                SystemMessage.ShowModalInfoMessage("Error in initializing data. Detail:: " + ex.Message, "ERROR");
                return false;
            }
        }

        public List<int> GetDaysOfWeek(List<WeekDayDTO> cnetWeekDays)
        {

            List<int> weekL = new List<int>();

            foreach (WeekDayDTO wk in cnetWeekDays)
            {
                int weeks = 0;
                switch (wk.Day)
                {

                    case 1://Sunday
                        {
                            weeks = 0;
                            break;
                        }
                    case 2://Monday
                        {
                            weeks = 1;
                            break;
                        }
                    case 4://Tuesday
                        {
                            weeks = 2;
                            break;
                        }
                    case 8://Wednesday
                        {
                            weeks = 3;
                            break;
                        }
                    case 16://Thursday
                        {
                            weeks = 4;
                            break;
                        }
                    case 32://Friday
                        {
                            weeks = 5;

                            break;
                        }
                    case 64://Saturday
                        {
                            weeks = 6;
                            break;
                        }

                }

                weekL.Add(weeks);
            }
            return weekL;
        }

        public SaveClickedResult OnSave()
        {
            string tab = "";
            try
            {
                if (tcRevenueManagementMain.SelectedTabPage == tpRateHeader)
                {
                    tab = "Rate Header";
                    Progress_Reporter.Show_Progress("Saving Rate Header", "Please Wait.......");

                    #region Saving Rate Header

                    RateCodeHeaderDTO rateCodeHeader = new RateCodeHeaderDTO();
                    RateCodeHeaderDTO avialRateH = null;
                    RateComponentDTO rComp = new RateComponentDTO();

                    RateCodeHeaderDTO rcHeaderCode = null;

                    List<Control> controls = new List<Control>();
                    controls.Add(cacArticleTransactionRateHeader);
                    controls.Add(teRateDescriptionRateHeader);
                    controls.Add(teFolioTextRateHeader);
                    controls.Add(cacMarketRateHeader);
                    controls.Add(cacSourceRateHeader);
                    controls.Add(cacCurrencyRateHeader);
                    controls.Add(cacRateCategoryRateHeader);

                    IList<Control> invalidControls = CustomValidationRule.Validate(controls);
                    if (invalidControls.Count > 0)
                    {
                        Progress_Reporter.Close_Progress();
                        return new SaveClickedResult();

                    }
                    if (selectedRoomTypes.Count == 0)
                    {
                        Progress_Reporter.Close_Progress();
                        SystemMessage.ShowModalInfoMessage("Please select at least one room type for these rate code.", "ERROR");
                        return new SaveClickedResult();
                    }

                    List<ValidationInfo> validationInfos = new List<ValidationInfo>
                    {
                        new ValidationInfo(deEndDateRateHeader, CompareControlOperator.LessOrEqual,
                            conditionOperator: ConditionOperator.IsNotBlank)
                        {
                            Control = deBeginDateRateHeader,
                            IsValidated=true
                        },
                        new ValidationInfo(deBeginDateRateHeader, CompareControlOperator.GreaterOrEqual,
                            conditionOperator: ConditionOperator.IsNotBlank)
                        {
                            Control = deEndDateRateHeader,
                            IsValidated=true
                        }
                    };

                    invalidControls = CustomValidationRule.Validate2(validationInfos);

                    if (invalidControls.Count > 0)
                    {
                        Progress_Reporter.Close_Progress();
                        return new SaveClickedResult();

                    }
                    // controls.Clear();

                    rateCodeHeader.Description = teRateDescriptionRateHeader.Text;
                    rateCodeHeader.StartDate = deBeginDateRateHeader.DateTime;
                    rateCodeHeader.EndDate = deEndDateRateHeader.DateTime;
                    rateCodeHeader.Article = Convert.ToInt32(cacArticleTransactionRateHeader.EditValue.ToString());
                    rateCodeHeader.RateCatagory = Convert.ToInt32(cacRateCategoryRateHeader.EditValue.ToString());
                    rateCodeHeader.FolioText = teFolioTextRateHeader.Text;

                    if (rateCodeHeader.Article == null || rateCodeHeader.Article == 0)
                    {
                        Progress_Reporter.Close_Progress();
                        SystemMessage.ShowModalInfoMessage("Please select at least one article!", "ERROR");
                        return new SaveClickedResult(); ;
                    }

                    //if (beiRateHeader.EditValue == null || string.IsNullOrWhiteSpace(beiRateHeader.EditValue.ToString()))
                    //{
                    //    Progress_Reporter.Close_Progress();
                    //    SystemMessage.ShowModalInfoMessage("Unable to get rate header code!", "ERROR");
                    //    return new SaveClickedResult();
                    //}

                    if (cacMarketRateHeader.EditValue != null && !string.IsNullOrWhiteSpace(cacMarketRateHeader.EditValue.ToString()))
                    {
                        rateCodeHeader.Market = Convert.ToInt32(cacMarketRateHeader.EditValue.ToString());
                    }
                    else
                    {
                        rateCodeHeader.Market = null;
                    }
                    if (cacSourceRateHeader.EditValue != null && !string.IsNullOrWhiteSpace(cacSourceRateHeader.EditValue.ToString()))
                    {
                        rateCodeHeader.Source = Convert.ToInt32(cacSourceRateHeader.EditValue.ToString());
                    }
                    else
                    {
                        rateCodeHeader.Source = null;
                    }
                    if (!string.IsNullOrEmpty(teCommisionRateHeader.Text))
                    {
                        rateCodeHeader.Comission = Convert.ToDecimal(teCommisionRateHeader.Text);
                    }
                    rateCodeHeader.Consigneeunit = Convert.ToInt32(leHotel.EditValue);
                    rateCodeHeader.MinOccupancy = Convert.ToInt32(seMininumOccupancyRateHeader.Value);
                    rateCodeHeader.MaxOccupancy = Convert.ToInt32(seMaximumOccupancyRateHeader.Value);

                    if (!string.IsNullOrEmpty(teAdditionRateHeader.Text))
                    {
                        rateCodeHeader.Addition = Convert.ToInt32(teAdditionRateHeader.Text);
                    }

                    if (!string.IsNullOrEmpty(teMultiplicationRateHeader.Text))
                    {
                        rateCodeHeader.Multiplication = Convert.ToInt32(teMultiplicationRateHeader.Text);
                    }

                    if (cacCurrencyRateHeader.EditValue != null && !string.IsNullOrWhiteSpace(cacCurrencyRateHeader.EditValue.ToString()))
                    {
                        rateCodeHeader.CurrencyCode = Convert.ToInt32(cacCurrencyRateHeader.EditValue.ToString());
                    }
                    if (cacExchangeRateHeader.EditValue != null && !string.IsNullOrWhiteSpace(cacExchangeRateHeader.EditValue.ToString()))
                    {
                        rateCodeHeader.ExchangeRule = Convert.ToInt32(cacExchangeRateHeader.EditValue.ToString());
                    }

                    rateCodeHeader.Policy = meRemarkRateHeader.Text;


                    int editRateCode = Convert.ToInt32(beiRateCodeHeader.EditValue);
                    avialRateH = UIProcessManager.GetRateCodeHeaderById(editRateCode);

                    if (avialRateH != null)
                    {
                        #region Updating Rate Header

                        rateCodeHeader.Id = editRateCode;
                        // rcHeaderCode = editRateCode;

                        if (UIProcessManager.UpdateRateCodeHeader(rateCodeHeader) != null)
                        {

                            List<PackageVM> changedPckageRows = new List<PackageVM>();
                            int[] selected = gv_pkgSelection.GetSelectedRows();

                            for (int i = 0; i < selected.Length; i++)
                            {
                                PackageVM selectedPkg = gv_pkgSelection.GetRow(selected[i]) as PackageVM;
                                if (selectedPkg != null)
                                {
                                    changedPckageRows.Add(selectedPkg);
                                }
                            }


                            // Delete existing packages
                            List<RateCodePackageDTO> rcrmList = UIProcessManager.GetRateCodePackageByRateCodeHeader(editRateCode);
                            if (rcrmList != null)
                            {
                                foreach (RateCodePackageDTO rct in rcrmList)
                                {
                                    UIProcessManager.DeleteRateCodePackageById(rct.Id);
                                }


                            }

                            // Save the news ones
                            RateCodePackageDTO rcPackage = new RateCodePackageDTO();
                            foreach (PackageVM dto in changedPckageRows)
                            {

                                rcPackage = new RateCodePackageDTO();
                                rcPackage.RateCodeHeader = editRateCode;
                                rcPackage.PackageHeader = dto.Id;
                                rcPackage.PackageIncluded = dto.isIncluded.Value;
                                rcPackage.Quantity = Convert.ToDecimal(dto.quantity);
                                UIProcessManager.CreateRateCodePackage(rcPackage);

                            }


                            //Update Rate RoomTypes

                            List<RateCodeRoomTypeDTO> rateCodeRTList = UIProcessManager.GetRateCodeRoomTypeByrateCode(editRateCode);
                            if (rateCodeRTList != null)
                            {
                                foreach (var rt in rateCodeRTList)
                                {
                                    UIProcessManager.DeleteRateCodeRoomTypeById(rt.Id);
                                }
                            }

                            foreach (RateRoomTypeVM dto in selectedRoomTypes)
                            {
                                RateCodeRoomTypeDTO rcRoomType = new RateCodeRoomTypeDTO();
                                rcRoomType.RoomType = dto.Id;
                                rcRoomType.RateCodeHeader = editRateCode;

                                UIProcessManager.CreateRateCodeRoomType(rcRoomType);
                            }

                            List<RateComponentDTO> rCompList = UIProcessManager.GetRateComponentByrateCode(editRateCode);
                            if (rCompList != null && rCompList.Count > 0)
                            {
                                foreach (RateComponentDTO rc in rCompList)
                                {
                                    UIProcessManager.DeleteRateComponentById(rc.Id);

                                }
                            }
                            foreach (SystemConstantDTO checkedItem in clbcComponentsRateHeader.CheckedItems)
                            {

                                rComp = new RateComponentDTO();
                                rComp.Pointer = checkedItem.Id;
                                rComp.RateCode = editRateCode;
                                UIProcessManager.CreateRateComponent(rComp);
                            }


                            SystemMessage.ShowModalInfoMessage("Rate Header Edited Successfully!", "MESSAGE");
                        }
                        else
                        {
                            SystemMessage.ShowModalInfoMessage("Unable to update Rate Header!", "ERROR");
                        }
                        //GetRateHeader();
                        GetFilterRateHeader();
                        //List<RateCodeHeader> rateList = UIProcessManager.SelectAllRateCode();
                        //cacCode.DataSource = null;
                        //cacCode.DataSource = (rateList);

                        //refresh data
                        PopulateRateHeader(true);

                        #endregion

                    }
                    else
                    {
                        #region Creating New Rate Header

                        rcHeaderCode = UIProcessManager.CreateRateCodeHeader(rateCodeHeader);

                        if (rcHeaderCode.Id > 0)
                        {
                            /** Saving Rate-Packages **/
                            List<PackageVM> addedPckageRows = new List<PackageVM>();
                            int[] selected = gv_pkgSelection.GetSelectedRows();
                            for (int i = 0; i < selected.Length; i++)
                            {
                                PackageVM selectedPkg = gv_pkgSelection.GetRow(selected[i]) as PackageVM;
                                if (selectedPkg != null)
                                {
                                    addedPckageRows.Add(selectedPkg);
                                }
                            }
                            RateCodePackageDTO rcPackage = new RateCodePackageDTO();
                            foreach (PackageVM dto in addedPckageRows)
                            {

                                rcPackage = new RateCodePackageDTO();
                                rcPackage.RateCodeHeader = rcHeaderCode.Id;
                                rcPackage.PackageHeader = dto.Id;
                                rcPackage.PackageIncluded = dto.isIncluded.Value;
                                rcPackage.Quantity = Convert.ToDecimal(dto.quantity);
                                UIProcessManager.CreateRateCodePackage(rcPackage);

                            }

                            //** Saving Rate-Room Types **/
                            RateCodeRoomTypeDTO rcRoomType = new RateCodeRoomTypeDTO();
                            foreach (RateRoomTypeVM dto in selectedRoomTypes)
                            {

                                rcRoomType = new RateCodeRoomTypeDTO
                                {
                                    RoomType = dto.Id,
                                    RateCodeHeader = rcHeaderCode.Id
                                };

                                UIProcessManager.CreateRateCodeRoomType(rcRoomType);
                            }


                            //** Saving Rate Components **/
                            bool isCSaved = false;
                            foreach (SystemConstantDTO checkedItem in clbcComponentsRateHeader.CheckedItems)
                            {
                                // Lookup lk = UIProcessManager.GetLookupByKeyword(checkedItem.ToString()).FirstOrDefault();

                                rComp = new RateComponentDTO();
                                rComp.Pointer = checkedItem.Id;
                                rComp.RateCode = rcHeaderCode.Id;
                                UIProcessManager.CreateRateComponent(rComp);
                                isCSaved = true;
                            }
                            SystemMessage.ShowModalInfoMessage("Rate Header Saved Successfully", "MESSAGE");
                        }
                        else
                        {
                            SystemMessage.ShowModalInfoMessage("Unable To Save Rate Header", "MESSAGE");
                        }

                        //refresh rate header
                        PopulateRateHeader(true);

                        #endregion
                    }


                    ResetRateHeader();

                    return new SaveClickedResult();

                    #endregion



                }

                else if (tcRevenueManagementMain.SelectedTabPage == tpRateAvailability)
                {
                    tab = "Rate Availability";
                    Progress_Reporter.Show_Progress("Rate Availability", "Please Wait.......");
                    #region Saving Rate Availability

                    TreeListView tlView = treeList_rateAvailability.GetDataRecordByNode(treeList_rateAvailability.FocusedNode) as TreeListView;
                    if (tlView != null)
                    {
                        int rateCodeDetail = tlView.RateDeatil;
                        if (MainList.Count == 0) return new SaveClickedResult();

                        List<AvailabilityCalendarDTO> availabilityCalendars = (from availabilityStatuse in MainList
                                                                               where availabilityStatuse.Value != AvailabilityStatus.NONE
                                                                               select new AvailabilityCalendarDTO()
                                                                               {
                                                                                   Pointer = CNETConstantes.TABLE_RATE_CODE_DETAIL,
                                                                                   Reference = rateCodeDetail,
                                                                                   Date = availabilityStatuse.Key,
                                                                                   Year = availabilityStatuse.Key.Year,
                                                                                   Month = availabilityStatuse.Key.Month,
                                                                                   Day = availabilityStatuse.Key.Day,
                                                                                   AvailabilityStatus = availabilityStatuse.Value == AvailabilityStatus.OPEN ? 1 : 0,
                                                                                   Locked = false
                                                                               }).ToList();

                        List<AvailabilityCalendarDTO> AvailabilityCalendarlist = UIProcessManager.GetAvailabilityCalendarBypointerandreference(CNETConstantes.TABLE_RATE_CODE_DETAIL, rateCodeDetail);
                        if (AvailabilityCalendarlist != null)
                            AvailabilityCalendarlist.ForEach(x => UIProcessManager.DeleteAvailabilityCalendarById(x.Id));


                        foreach (AvailabilityCalendarDTO availabilityCalendar in availabilityCalendars)
                        {
                            UIProcessManager.CreateAvailabilityCalendar(availabilityCalendar);
                        }

                        SystemMessage.ShowModalInfoMessage("Saved Successfully", "MESSAGE");

                        return new SaveClickedResult();

                    }
                    #endregion
                }


                Progress_Reporter.Close_Progress();
                return new SaveClickedResult();
            }
            catch (Exception ex)
            {
                Progress_Reporter.Close_Progress();
                SystemMessage.ShowModalInfoMessage("Error in saving " + tab + ". Detail: " + ex.Message, "ERROR");
                return new SaveClickedResult();
            }

        }

        public DeleteClickedResult OnDelete()
        {
            string tab = "";
            try
            {
                if (tcRevenueManagementMain.SelectedTabPage == tpRateHeader)
                {
                    tab = "Rate Header";
                    Progress_Reporter.Show_Progress("Deleting Rate Header", "Please Wait.......");
                    #region Deleting Rate Header

                    bool canDelete = false;
                    if (beiRateCodeHeader.EditValue != null)
                    {
                        canDelete = false;
                        int rateCode = Convert.ToInt32(beiRateCodeHeader.EditValue.ToString());
                        //List<RateDTO> rates = UIProcessManager.SelectAllRate(rateCode);
                        //if (rates.Count > 0)
                        //{
                        //    canDelete = true;
                        //}
                        List<RateCodeDetailDTO> rateDetails = UIProcessManager.GetRateCodeDetailByRateHeaderCode(rateCode);
                        if (rateDetails.Count > 0)
                        {
                            canDelete = true;
                        }
                        List<RateCodePackageDTO> ratecodePackeges = UIProcessManager.GetRateCodePackageByRateCodeHeader(rateCode);
                        if (ratecodePackeges.Count > 0)
                        {
                            canDelete = true;
                        }
                        List<RateCodeRateStrategyDTO> rateStrategies = UIProcessManager.GetRateCodeRateStrategyByrateCode(rateCode);
                        if (rateStrategies.Count > 0)
                        {
                            canDelete = true;
                        }
                        List<RateCodeRoomTypeDTO> rateRoomTypes = UIProcessManager.GetRateCodeRoomTypeByrateCode(rateCode);
                        if (rateRoomTypes.Count > 0)
                        {
                            canDelete = true;
                        }
                        List<RateComponentDTO> rateComponents = UIProcessManager.GetRateComponentByrateCode(rateCode);
                        if (rateComponents.Count > 0)
                        {
                            canDelete = true;
                        }

                        List<RateCodePackageDTO> ratePackages = UIProcessManager.GetRateCodePackageByRateCodeHeader(rateCode);
                        if (ratePackages != null)
                        {
                            ratePackages = ratePackages.Where(r => r.RateCodeHeader == rateCode).ToList();
                            if (ratePackages.Count > 0)
                            {
                                canDelete = true;
                            }
                        }
                        if (canDelete)
                        {
                            SystemMessage.ShowModalInfoMessage("This rate header can not be deleted. It has other transaction", "ERROR");
                        }
                        else
                        {
                            if (UIProcessManager.DeleteRateCodeHeaderById(rateCode))
                            {
                                SystemMessage.ShowModalInfoMessage("Deleted successfully.", "MESSAGE");

                            }
                            else
                            {
                                SystemMessage.ShowModalInfoMessage("Deleting not successfull!", "ERROR");
                            }
                        }
                    }

                    #endregion

                }
                else if (tcRevenueManagementMain.SelectedTabPage == tpRateCategory)
                {
                    tab = "Rate Category";
                    Progress_Reporter.Show_Progress("Deleting Rate Category", "Please Wait.......");
                    #region Deleting Rate Category

                    RateCategoryDTO rateCat = (RateCategoryDTO)treeList_rateCategory.GetDataRecordByNode(treeList_rateCategory.FocusedNode);
                    if (rateCat == null)
                    {
                        Progress_Reporter.Close_Progress();
                        SystemMessage.ShowModalInfoMessage("Please select rate category!", "ERROR");
                        return new DeleteClickedResult();
                    }
                    bool canDelete = false;

                    List<RateCategoryDTO> rateCateP = UIProcessManager.SelectAllRateCategory().Where(r => r.ParentId == rateCat.Id).ToList();
                    if (rateCateP.Count > 0)
                    {
                        canDelete = true;
                    }

                    List<RateCodeHeaderDTO> rateHeaders = UIProcessManager.GetRateCodeHeaderByrateCatagory(rateCat.Id);
                    if (rateHeaders.Count > 0)
                    {
                        canDelete = true;
                    }
                    if (canDelete)
                    {
                        SystemMessage.ShowModalInfoMessage("Can not be deleted it has other transaction", "ERROR");
                    }
                    else
                    {
                        if (UIProcessManager.DeleteRateCategoryById(rateCat.Id))
                        {
                            SystemMessage.ShowModalInfoMessage("Deleted successfully.", "MESSAGE");
                            PopulateRateCategory(true);
                        }
                        else
                        {
                            SystemMessage.ShowModalInfoMessage("Deleting is not successfull.", "ERROR");
                        }
                    }



                    #endregion

                }
                else if (tcRevenueManagementMain.SelectedTabPage == tpRateDetail)
                {
                    bool canDelete = false;

                    RateCodeHeaderDTO rHeader = gv_rateDetailHeaders.GetFocusedRow() as RateCodeHeaderDTO;
                    if (rHeader == null)
                    {
                        Progress_Reporter.Close_Progress();
                        SystemMessage.ShowModalInfoMessage("Please select rate header!", "ERROR");
                        return new DeleteClickedResult();
                    }

                    if (tcRateDetailChild.SelectedTabPage == tpRateDetailChild)
                    {
                        tab = "Rate Detail";
                        Progress_Reporter.Show_Progress("Deleting Rate Detail", "Please Wait.......");
                        #region Rate Detail

                        RateCodeDetailDTO rateDetail = (RateCodeDetailDTO)gv_rateDetail.GetFocusedRow();
                        if (rateDetail == null)
                        {
                            Progress_Reporter.Close_Progress();
                            SystemMessage.ShowModalInfoMessage("Please select rate detail!", "ERROR");
                            return new DeleteClickedResult();
                        }

                        List<RegistrationDetailDTO> regDetails = UIProcessManager.GetRegistrationDetailByrateCode(rateDetail.Id);
                        if (regDetails.Count > 0)
                        {
                            canDelete = true;
                        }
                        if (canDelete)
                        {
                            SystemMessage.ShowModalInfoMessage("Can not be deleted. It has other transaction", "ERROR");
                        }
                        else
                        {



                            List<WeekDayDTO> weekdaylist = UIProcessManager.GetWeekDaysByReferenceandPointer(rateDetail.Id, CNETConstantes.TABLE_PACKAGE_DETAIL);
                            if (weekdaylist != null)
                                weekdaylist.ForEach(x => UIProcessManager.DeleteWeekDayById(x.Id));

                            List<AvailabilityCalendarDTO> AvailabilityCalendarlist = UIProcessManager.GetAvailabilityCalendarBypointerandreference(CNETConstantes.TABLE_RATE_CODE_DETAIL, rateDetail.Id);
                            if (AvailabilityCalendarlist != null)
                                AvailabilityCalendarlist.ForEach(x => UIProcessManager.DeleteAvailabilityCalendarById(x.Id));

                            List<RateCodeDetailRoomTypeDTO> RateCodeDetailRoomTypelist = UIProcessManager.GetRateCodeDetailRoomTypeByrateCodeDetail(rateDetail.Id);
                            if (RateCodeDetailRoomTypelist != null)
                                RateCodeDetailRoomTypelist.ForEach(x => UIProcessManager.DeleteRateCodeDetailRoomTypeById(x.Id));

                            List<RateCodeDetailGuestCountDTO> RateCodeDetailGuestCountlist = UIProcessManager.GetRateCodeDetailGuestCountByrateCodeDetail(rateDetail.Id);
                            if (RateCodeDetailGuestCountlist != null)
                                RateCodeDetailGuestCountlist.ForEach(x => UIProcessManager.DeleteRateCodeDetailGuestCountById(x.Id));



                            if (UIProcessManager.DeleteRateCodeDetailById(rateDetail.Id))
                            {
                                SystemMessage.ShowModalInfoMessage("Deleted successfully.", "MESSAGE");
                                PopulateRateDetailGrid(rHeader.Id);
                            }
                            else
                            {
                                SystemMessage.ShowModalInfoMessage("Deleting is not successfull.", "ERROR");
                            }

                        }


                        #endregion
                    }
                    else if (tcRateDetailChild.SelectedTabPage == tpNegotiationRate)
                    {
                        tab = "Negotiated Rate";
                        Progress_Reporter.Show_Progress("Deleting Negotiated Rate", "Please Wait.......");
                        #region Negotiated Rate
                        NegotiatedViewVM negoRate = (NegotiatedViewVM)gv_negotiationRate.GetFocusedRow();
                        if (negoRate == null)
                        {
                            Progress_Reporter.Close_Progress();
                            SystemMessage.ShowModalInfoMessage("Please select negotiated rate!", "ERROR");
                            return new DeleteClickedResult();
                        }

                        if (UIProcessManager.DeleteNegotiationRateById(negoRate.Id))
                        {

                            SystemMessage.ShowModalInfoMessage("Deleted successfully.", "MESSAGE");

                            PopulateNegotiationGrid(rHeader.Id);
                        }
                        else
                        {
                            SystemMessage.ShowModalInfoMessage("Deleting not successfully.", "ERROR");
                        }


                        #endregion
                    }
                    else if (tcRateDetailChild.SelectedTabPage == tpStrategy)
                    {
                        tab = "Rate Strategy";
                        Progress_Reporter.Show_Progress("Deleting Rate Strategy", "Please Wait.......");
                        #region Rate Strategy

                        RateStrategyVM rateStra = (RateStrategyVM)gv_rateStrategy.GetFocusedRow();
                        if (rateStra == null)
                        {
                            Progress_Reporter.Close_Progress();
                            SystemMessage.ShowModalInfoMessage("Please select rate strategy!", "ERROR");
                            return new DeleteClickedResult();
                        }

                        List<WeekDayDTO> weekdaylist = UIProcessManager.GetWeekDaysByReferenceandPointer(rateStra.Id, CNETConstantes.TABLE_RATE_STRATEGY);
                        if (weekdaylist != null)
                            weekdaylist.ForEach(x => UIProcessManager.DeleteWeekDayById(x.Id));

                        List<RoomTypeRateStrategyDTO> RoomTypeRateStrategylist = UIProcessManager.GetRoomTypeRateStrategyByrateStrategy(rateStra.Id);
                        if (RoomTypeRateStrategylist != null)
                            RoomTypeRateStrategylist.ForEach(x => UIProcessManager.DeleteRoomTypeRateStrategyById(x.Id));

                        List<RateCodeRateStrategyDTO> RateCodeRateStrategylist = UIProcessManager.GetRateCodeRateStrategyByrateStrategy(rateStra.Id);
                        if (RateCodeRateStrategylist != null)
                            RateCodeRateStrategylist.ForEach(x => UIProcessManager.DeleteRateCodeRateStrategyById(x.Id));

                        List<RateCategoryRateStrategyDTO> RateCategoryRateStrategylist = UIProcessManager.GetRateCategoryRateStrategyByrateStrategy(rateStra.Id);
                        if (RateCategoryRateStrategylist != null)
                            RateCategoryRateStrategylist.ForEach(x => UIProcessManager.DeleteRateCategoryRateStrategyById(x.Id));




                        /*canDelete =*/


                        if (UIProcessManager.DeleteRateStrategyById(rateStra.Id))
                        {
                            SystemMessage.ShowModalInfoMessage("Deleted successfully.", "MESSAGE");
                            PopulateRateStrategy(rHeader.Id);
                        }
                        else
                        {
                            SystemMessage.ShowModalInfoMessage("Deleting is not successfull.", "ERROR");
                        }



                        #endregion
                    }

                }

                Progress_Reporter.Close_Progress();
                return new DeleteClickedResult();

            }
            catch (Exception ex)
            {
                Progress_Reporter.Close_Progress();
                SystemMessage.ShowModalInfoMessage("Error in saving " + tab + ". Detail: " + ex.Message, "ERROR");
                return new DeleteClickedResult();
            }
        }

        //resetting rate header
        private void ResetRateHeader()
        {
            beiRateCodeHeader.EditValue = null;

            teRateDescriptionRateHeader.Text = "";
            teFolioTextRateHeader.Text = "";
            leHotel.EditValue = null;
            deBeginDateRateHeader.DateTime = CurrentTime;
            deBeginDateRateHeader.Properties.MinValue = CurrentTime;
            deEndDateRateHeader.DateTime = CurrentTime;

            cacArticleTransactionRateHeader.EditValue = "";
            cacCurrencyRateHeader.EditValue = _defCurrency;
            cacExchangeRateHeader.EditValue = _defXRule;
            cacSourceRateHeader.EditValue = _defBusSource;
            cacRateCategoryRateHeader.EditValue = "";
            cacMarketRateHeader.EditValue = _defMarket;

            teCommisionRateHeader.Text = "";
            seMaximumOccupancyRateHeader.Value = 1;
            seMininumOccupancyRateHeader.Value = 1;
            teMultiplicationRateHeader.Text = "";
            teAdditionRateHeader.Text = "";
            meRemarkRateHeader.Text = "";
            //leHotel.EditValue = "";
            refreshPackageGrid(true);
            refreshRoomTypeGrid(true);

            for (int i = 0; i < rateCompList.Count; i++)
            {
                clbcComponentsRateHeader.SetItemChecked(i, false);
            }

        }

        /** Rate Category Methods **/
        #region Rate Category Methods

        //Populate Rate Category
        public void PopulateRateCategory(bool isRefresh)
        {
            try
            {
                if (rateCatList != null && !isRefresh) return;
                //Progress_Reporter.Show_Progress("Populating Rate Category");


                rateCatList = UIProcessManager.SelectAllRateCategory();

                //poupulate Rate Header's Rate Category lookup edit
                cacRateCategoryRateHeader.Properties.DataSource = rateCatList;
                cacRateCategoryRateHeader.Refresh();

                foreach (RateCategoryDTO rat in rateCatList)
                {
                    if (rat.Type != null && rat.Type > 0)
                    {
                        var lookup = LocalBuffer.LocalBuffer.SystemConstantDTOBufferList.FirstOrDefault(l => l.Id == rat.Type);
                        rat.Type = lookup == null ? null : lookup.Id;
                    }
                }
                treeList_rateCategory.BeginUpdate();
                treeList_rateCategory.DataSource = rateCatList;
                treeList_rateCategory.RefreshDataSource();
                treeList_rateCategory.EndUpdate();
                treeList_rateCategory.ExpandAll();

                Progress_Reporter.Close_Progress();
            }
            catch (Exception ex)
            {
                Progress_Reporter.Close_Progress();
                SystemMessage.ShowModalInfoMessage("Error in populating Rate Category. Detail: " + ex.Message, "ERROR");

            }
        }

        private void CreateNewRateCategoryPopUp()
        {
            frmRateCategory rateCategory = new frmRateCategory();

            if (rateCategory.ShowDialog(this) == DialogResult.OK)
            {
                PopulateRateCategory(true);
            }
        }

        #endregion

        /** Rate Header Methods **/
        #region Rate Header Methods

        //Populate Rate Header
        private void PopulateRateHeader(bool isRefresh)
        {
            if (gv_RoomTypeRateHeader.RowCount == 0 || gv_pkgSelection.RowCount == 0)
            {
                // LoadPackages();
                // LoadRoomType();
            }

            if (rateList != null && !isRefresh) return;

            Progress_Reporter.Show_Progress("Loading Rate Headers", "Please Wait.......");

            cacCode.DataSource = null;
            //gc_rateDetailHeaders.DataSource = null;
            //rateList = UIProcessManager.SelectAllRateCode();
            //cacCode.DataSource = (rateList);

            //gc_rateDetailHeaders.DataSource = rateList;
            //gc_rateDetailHeaders.RefreshDataSource();
            //gv_rateDetailHeaders.RefreshData();
            GetRateHeader();
            Progress_Reporter.Close_Progress();




        }

        //pakcages
        public void LoadPackages(int SelectedHotelcodefilter)
        {
            try
            {
                Progress_Reporter.Show_Progress("Populating Packages", "Please Wait.......");

                packageList.Clear();
                if (SelectedHotelcodefilter > 0)
                {
                    List<PackageHeaderDTO> pkgHeaderList = UIProcessManager.GetAllPackageHeaderByConsigneeUnit(SelectedHotelcodefilter);
                    PackageVM package = new PackageVM();
                    List<PackageDetailDTO> allPkgDetail = UIProcessManager.SelectAllPackageDetail();

                    //UIProcessManager.getPackageDetailsOnDate
                    foreach (PackageHeaderDTO pkg in pkgHeaderList)
                    {
                        package = new PackageVM
                        {
                            Id = pkg.Id,
                            pkgHeader = pkg.Description,
                            value = false,
                            isIncluded = true,
                            quantity = 0
                        };


                        // don't adding expire packages
                        bool canBeAdded = true;

                        List<PackageDetailDTO> filtered = allPkgDetail.Where(pd => pd.PackageHeader == pkg.Id && (pd.StartDate.Date <= CurrentTime.Date &&
                            pd.EndingDate.Date >= CurrentTime.Date)).ToList();
                        if (filtered == null || filtered.Count == 0) canBeAdded = false;

                        if (canBeAdded)
                            packageList.Add(package);
                    }


                }
                gc_pkgSelection.DataSource = packageList;
                gv_pkgSelection.RefreshData();

                refreshPackageGrid(false);

                Progress_Reporter.Close_Progress();
            }
            catch (Exception ex)
            {
                Progress_Reporter.Close_Progress();
                SystemMessage.ShowModalInfoMessage("Error in populating package list. Detail: " + ex.Message, "ERROR");
            }
        }

        private void refreshPackageGrid(bool isClear)
        {
            for (int i = 0; i < gv_pkgSelection.DataRowCount; i++)
            {
                if (isClear)
                {
                    gv_pkgSelection.UnselectRow(i);
                    continue;
                }

                bool value = (bool)gv_pkgSelection.GetRowCellValue(i, "value");
                if (value)
                {
                    gv_pkgSelection.SelectRow(i);
                }
                else
                    gv_pkgSelection.UnselectRow(i);
            }
        }

        //room types
        public void LoadRoomType(int SelectedHotelcodefilter)
        {
            try
            {
                rDTOList.Clear();
                if (SelectedHotelcodefilter != 0)
                {
                    Progress_Reporter.Show_Progress("Populating Room Types", "Please Wait.......");

                    List<RoomTypeDTO> roomTypeList = UIProcessManager.GetRoomTypeByConsigneeUnit(SelectedHotelcodefilter).Where(r => r.IsActive && (r.ActivationDate != null && r.ActivationDate.Value.Date <= CurrentTime.Date)).ToList();
                    RateRoomTypeVM rDTO = new RateRoomTypeVM();

                    foreach (RoomTypeDTO rt in roomTypeList)
                    {
                        rDTO = new RateRoomTypeVM();
                        rDTO.Id = rt.Id;
                        rDTO.roomType = rt.Description;
                        rDTO.value = false;
                        rDTOList.Add(rDTO);
                    }

                }

                gc_RoomTypeRateHeader.DataSource = rDTOList;
                gv_RoomTypeRateHeader.RefreshData();
                refreshRoomTypeGrid(false);
                Progress_Reporter.Close_Progress();
            }
            catch (Exception ex)
            {
                Progress_Reporter.Close_Progress();
                SystemMessage.ShowModalInfoMessage("Error in populating room types. Detail: " + ex.Message, "ERROR");
            }
        }
        private void refreshRoomTypeGrid(bool isClear)
        {
            for (int i = 0; i < gv_RoomTypeRateHeader.DataRowCount; i++)
            {
                if (isClear)
                {
                    gv_RoomTypeRateHeader.UnselectRow(i);
                    continue;
                }

                bool value = (bool)gv_RoomTypeRateHeader.GetRowCellValue(i, "value");
                if (value)
                {
                    gv_RoomTypeRateHeader.SelectRow(i);
                }
                else
                    gv_RoomTypeRateHeader.UnselectRow(i);
            }
        }

        //date validation
        public void validateArrivalAndDepartureDate()
        {
            if (deEndDateRateHeader.IsModified || deBeginDateRateHeader.IsModified)
            {
                List<ValidationInfo> validationInfos = new List<ValidationInfo>
            {
                new ValidationInfo(deEndDateRateHeader, CompareControlOperator.LessOrEqual,
                    conditionOperator: ConditionOperator.IsNotBlank)
                {
                    Control = deBeginDateRateHeader,
                    IsValidated=true
                },
                new ValidationInfo(deBeginDateRateHeader, CompareControlOperator.GreaterOrEqual,
                    conditionOperator: ConditionOperator.IsNotBlank)
                {
                    Control = deEndDateRateHeader,
                    IsValidated=true
                }
            };
                _invalidControls = CustomValidationRule.Validate2(validationInfos);

                if (_invalidControls.Count > 0)
                    SystemMessage.ShowModalInfoMessage("The start date can not be greate than the end date!!!", "ERROR");
                return;
            }
        }

        #endregion

        /** Rate Detail Methods **/
        #region Rate Detail Methods


        //populate rate detail
        public void PopulateRateDetailGrid(int rateCodeHeader)
        {
            try
            {
                Progress_Reporter.Show_Progress("Loading Rate Detail", "Please Wait.......");

                List<RateCodeDetailDTO> rateCodeDetail = UIProcessManager.GetRateCodeDetailByRateHeaderCode(rateCodeHeader).ToList();

                gc_rateDetail.DataSource = rateCodeDetail;
                gc_rateDetail.RefreshDataSource();
                gv_rateDetail.RefreshData();
                Progress_Reporter.Close_Progress();
            }
            catch (Exception ex)
            {
                Progress_Reporter.Close_Progress();
                SystemMessage.ShowModalInfoMessage("Error in populating Rate Detail. Detail: " + ex.Message, "ERROR");
            }
        }

        //populate Negotiated Rate
        public void PopulateNegotiationGrid(int rateCodeHeader) // call this if only negotiated tab page is active
        {
            try
            {
                gc_negotiationRate.DataSource = null;
                Progress_Reporter.Show_Progress("Populating Negotiated Rate", "Please Wait.......");
                List<NegotiationRateDTO> negotiatedRateList = UIProcessManager.GetNegotiationRateByrateCode(rateCodeHeader);
                List<NegotiatedViewVM> dtoList = new List<NegotiatedViewVM>();
                foreach (NegotiationRateDTO negoRate in negotiatedRateList)
                {
                    NegotiatedViewVM dto = new NegotiatedViewVM();
                    dto.rateCode = negoRate.RateCode;
                    ConsigneeDTO Consignee = LocalBuffer.LocalBuffer.ConsigneeViewBufferList.FirstOrDefault(p => p.Id == negoRate.Consignee);
                    if (Consignee != null)
                    {
                        dto.consignee = negoRate.Consignee;
                        dto.gslType = CNETConstantes.GUEST;
                        dto.Id = negoRate.Id;
                        dto.gslTypeDescription = Consignee.IsPerson ? "Guest" : "Company";
                        dto.name = Consignee.ThirdName + " " + Consignee.FirstName + " " + Consignee.SecondName;
                        dto.identification = Consignee.Tin;

                    }

                    dtoList.Add(dto);
                }

                gc_negotiationRate.DataSource = dtoList;
                gv_negotiationRate.RefreshData();
                Progress_Reporter.Close_Progress();
            }
            catch (Exception)
            {
                SystemMessage.ShowModalInfoMessage("Error in populating negotated rate", "ERROR");
                Progress_Reporter.Close_Progress();
            }
        }

        //Populate Rate Strategy
        public void PopulateRateStrategy(int rateCodeHeader)
        {
            try
            {
                Progress_Reporter.Show_Progress("Loading Rate Strategy", "Please Wait.......");
                gc_rateStrategy.DataSource = null;
                var rateCodeRateStrategy = UIProcessManager.GetRateCodeRateStrategyByRateHeader(rateCodeHeader);
                List<RateStrategyDTO> allRateStrategies = UIProcessManager.SelectAllRateStrategy().ToList();
                if (allRateStrategies != null)
                {


                    List<RateStrategyVM> rateStrategies = (from rcrs in rateCodeRateStrategy
                                                           join rs in allRateStrategies
                                                               on rcrs.RateStrategy equals rs.Id
                                                           select new RateStrategyVM()
                                                           {
                                                               Id = rs.Id,
                                                               //Description = rs.Description,
                                                               RestrictionType = LocalBuffer.LocalBuffer.SystemConstantDTOBufferList.FirstOrDefault(l => l.Id == rs.RestrictionType).Description,
                                                               //Condition = LocalBuffer.LocalBuffer.SystemConstantDTOBufferList.FirstOrDefault(l => l.Id == rs.Condition).Description,
                                                               Value = rs.Value,
                                                               ValueType = rs.IsPercent ? "Percent (%)" : "Amount"
                                                           }).ToList();


                    gc_rateStrategy.DataSource = rateStrategies;
                    gv_rateStrategy.RefreshData();
                }

                Progress_Reporter.Close_Progress();
            }
            catch (Exception ex)
            {
                Progress_Reporter.Close_Progress();
                SystemMessage.ShowModalInfoMessage("Error in populating rate strategy. Detail: " + ex.Message, "ERROR");

            }
        }

        #endregion

        /** Rate Availability Methods **/
        #region Rate Availability Methods

        public void BuildTreeListView(bool isRefresh)
        {
            try
            {
                if (!isRefresh && treeList_rateAvailability.AllNodesCount > 0) return;

                Progress_Reporter.Show_Progress("Loading Rate Details", "Please Wait.......");

                List<RateCodeHeaderDTO> rateCodeList = UIProcessManager.SelectAllRateCodeHeader();
                treeList_rateAvailability.DataSource = null;

                List<TreeListView> treeListViews = new List<TreeListView>();
                String oldValue = String.Empty;

                foreach (RateCodeHeaderDTO rd in rateCodeList)
                {
                    List<RateCodeDetailDTO> rateCodeDeatails = UIProcessManager.GetRateCodeDetailByRateHeaderCode(rd.Id);
                    if (rateCodeDeatails == null || rateCodeDeatails.Count == 0) continue;

                    TreeListView tlv = new TreeListView();
                    tlv.ID = Guid.NewGuid().ToString();
                    tlv.Value = rd.Description;// "V1" + new Random().Next();
                    tlv.ParentID = tlv.ID;
                    tlv.RateCode = rd.Id;
                    tlv.description = rd.Description;
                    oldValue = tlv.ID;
                    //tlv.RegistrationDetail = rd.room;

                    treeListViews.Add(tlv);
                    foreach (RateCodeDetailDTO ptp in rateCodeDeatails)
                    {
                        tlv = new TreeListView();
                        var x = Guid.NewGuid();
                        var old = x;
                        tlv.ParentID = oldValue;
                        tlv.Value = ptp.Description;// "P1" + x;
                        tlv.ID = Guid.NewGuid().ToString();
                        tlv.RateDeatil = ptp.Id;
                        tlv.description = "         " + ptp.Description;
                        treeListViews.Add(tlv);
                    }
                }
                treeList_rateAvailability.DataSource = treeListViews;
                treeList_rateAvailability.RefreshDataSource();
                treeList_rateAvailability.ExpandAll();

                Progress_Reporter.Close_Progress();
            }
            catch (Exception ex)
            {
                Progress_Reporter.Close_Progress();
                SystemMessage.ShowModalInfoMessage("Error in populate rate details. Details: " + ex.Message, "ERROR");
            }
        }

        public void PopulateAvailabilityCalendar(int rateCodeDetail)
        {

            List<AvailabilityCalendarDTO> availabilityCalendars = UIProcessManager.GetAvailabilityCalendarBypointerandreference(CNETConstantes.TABLE_RATE_CODE_DETAIL, rateCodeDetail);
            if (availabilityCalendars == null) return;

            int taskCount = availabilityCalendars.Count();

            Progress_Reporter.Show_Progress("Loading AvailabilityCalendar.", "Please Wait.......");
            foreach (AvailabilityCalendarDTO ac in availabilityCalendars)
            {
                AddToMainListExternal(ac.Date, ac.AvailabilityStatus == 1 ? AvailabilityStatus.OPEN : AvailabilityStatus.CLOSED);
            }
            Progress_Reporter.Show_Progress("Ploting rate availability...", "Please Wait.......");
            PerformPostMainListEditionActions();
            RedrawAvailability();
            Progress_Reporter.Close_Progress();
        }

        public void MaintainAvailabilityCalender(List<DateTime> dateList, List<int> daysOfWeek, int rateCode)
        {
            try
            {
                if (dateList == null || dateList.Count == 0)
                {
                    return;
                }
                Progress_Reporter.Show_Progress("Saving Availability Calendar", "Please Wait.......");

                AvailabilityCalendarDTO aCalender = new AvailabilityCalendarDTO();
                bool isASaved = true;
                foreach (DateTime date in dateList)
                {
                    if (daysOfWeek.Contains(Convert.ToInt32(date.DayOfWeek)))
                    {
                        aCalender = new AvailabilityCalendarDTO();
                        aCalender.Pointer = CNETConstantes.TABLE_RATE_CODE_HEADER;
                        aCalender.Reference = rateCode;
                        aCalender.Date = date;
                        aCalender.Year = date.Year;
                        aCalender.Month = date.Month;
                        aCalender.Day = date.Day;
                        aCalender.AvailabilityStatus = 1;
                        aCalender.Locked = false;
                    }
                    else
                    {
                        aCalender = new AvailabilityCalendarDTO();
                        aCalender.Pointer = CNETConstantes.TABLE_RATE_CODE_HEADER;
                        aCalender.Reference = rateCode;
                        aCalender.Date = date;
                        aCalender.Year = date.Year;
                        aCalender.Month = date.Month;
                        aCalender.Day = date.Day;
                        aCalender.AvailabilityStatus = 0;
                        aCalender.Locked = false;

                    }
                    AvailabilityCalendarDTO result = UIProcessManager.CreateAvailabilityCalendar(aCalender);
                    if (result == null)
                    {
                        isASaved = false;
                        break;
                    }

                }

                if (isASaved)
                {

                    SystemMessage.ShowModalInfoMessage("Availability Calendar Saved", "MESSAGE");
                }

                Progress_Reporter.Close_Progress();
            }
            catch (Exception ex)
            {
                Progress_Reporter.Close_Progress();
                SystemMessage.ShowModalInfoMessage("Error in saving availablity calendar. Detail: " + ex.Message, "ERROR");
            }
        }

        public void ClearAvailabilityCalender()
        {
            Progress_Reporter.Show_Progress("Clearing Data", "Please Wait.......");

            if (MainList != null)
                MainList.Clear();

            if (AvailabilityData != null)
                AvailabilityData.Clear();

            CreateAvailabilityCalenderData();
            RedrawAvailability();
            PerformPostMainListEditionActions();

            Progress_Reporter.Close_Progress();
        }

        private void CreateAvailabilityCalenderData()
        {
            gv_rateAvail.BeginUpdate();
            var dt = new DateTime(2000, 1, 1);

            for (var i = 1; i <= 12; i++, dt = dt.AddMonths(1))
            {
                AvailabilityControlObject monthCO = new AvailabilityControlObject();
                monthCO.Month = i;

                if (!string.IsNullOrEmpty(SelectedYear))
                    monthCO.Year = Convert.ToInt16(SelectedYear);
                else
                    monthCO.Year = CurrentTime.Year;

                monthCO.Name = (dt.ToString("MMMM"));

                monthCO.DayChanged += monthCO_DayChanged;

                var daysInMonth = DateTime.DaysInMonth(dt.Year, dt.Month);

                for (var j = 1; j <= 31; j++)
                {

                    #region Disable the Day if its not a month day

                    if (j > daysInMonth)//Disable the date
                    {
                        foreach (var _property in monthCO.GetType().GetProperties())
                        {
                            if (_property.Name == "Day" + j.ToString())
                            {

                                var v = ((DayCO)_property.GetValue(monthCO));

                                v.DayColor = Color.Black;

                                v.IsEnabled = false;
                                v.Value = "X";
                                //v.Value = "X";

                            }
                        }



                    }
                    #endregion

                    #region Mark Holidays


                    if (ISHoliday(i, j))
                    {
                        //set isHoliday to true;

                    }

                    #endregion

                }

                AvailabilityData.Add(monthCO);


            }
            List<KeyValuePair<string, string>> cols = new List<KeyValuePair<string, string>>();
            cols.Add(new KeyValuePair<string, string>("Name", "Name"));


            //cdeAvailability.Properties.Columns = new GridColumn[32];


            GridColumn col = new GridColumn();
            col.FieldName = "Name";
            col.Visible = true;
            col.VisibleIndex = 0;
            gv_rateAvail.Columns.Clear();
            gv_rateAvail.Columns.Add(col);

            for (var i = 1; i <= 31; i++)
            {

                cols.Add(new KeyValuePair<string, string>(("Day" + i.ToString() + ".Value"), i.ToString()));

                GridColumn col1 = new GridColumn();
                col1.Caption = i.ToString();

                col1.FieldName = ("Day" + i.ToString() + ".Value");
                col1.Visible = true;
                col1.VisibleIndex = i;

                gv_rateAvail.Columns.Add(col1);

            }

            gc_rateAvail.DataSource = (AvailabilityData);
            gc_rateAvail.RefreshDataSource();

            gv_rateAvail.OptionsView.ShowButtonMode = ShowButtonModeEnum.ShowAlways;

            for (var i = 1; i <= 31; i++)
                gv_rateAvail.Columns[i].BestFit();

            gv_rateAvail.Columns[0].OptionsColumn.ReadOnly = true;
            gv_rateAvail.Appearance.FocusedCell.BackColor = Color.OrangeRed;
            gv_rateAvail.Appearance.FocusedCell.Options.UseBackColor = true;
            gv_rateAvail.Appearance.FocusedRow.BackColor = Color.Transparent;
            gv_rateAvail.Columns[0].Width = 200;

            gv_rateAvail.EndUpdate();
            gv_rateAvail.RefreshData();

        }

        private void cdeAvailability_Click(object sender, EventArgs e)
        {
            MarkDateRange(new DateTime(2016, 1, 1), new DateTime(2016, 1, 10));
        }

        private void bsiClearAvailability_ItemClick(object sender, ItemClickEventArgs e)
        {

            ClearAvailabilityCalender();


        }

        private void Year_ItemClick(object sender, ItemClickEventArgs e)
        {
            //cdeAvailability.Logic.ShowInformation(e.Item.Caption);
        }

        public string SelectedYear;
        public void SetMarkedDate(DateTime dateTime)
        {
            SelectedYear = dateTime.Year.ToString();
            if (gv_rateAvail.ViewCaption != SelectedYear)
            {
                return;
            }
            var monthName = dateTime.ToString("MMMM");
            var monthRowIndex = -1;

            foreach (DataRow row in tableAvailability.Rows)
            {
                monthRowIndex++;
                if (row[0] == null)
                {
                    continue;
                }
                if (row[0].ToString().Equals(monthName))
                {
                    break;
                }
            }

            gv_rateAvail.SetRowCellValue(monthRowIndex, gv_rateAvail.Columns[dateTime.Day], AvailabilityControlObject.GetStatusIndicator(CurrentAvailaibityStatus));

            AvailabilityMarkedDates.Add(dateTime);
        }

        public void MarkDateRange(DateTime startDate, DateTime endDate)
        {
            gv_rateAvail.BeginUpdate();
            var _StartDate = new DateTime();
            var _EndDate = new DateTime();

            if (startDate <= endDate)
            {
                _StartDate = startDate;
                _EndDate = endDate;
            }
            else
            {
                _StartDate = endDate;
                _EndDate = startDate;
            }

            while (_StartDate < _EndDate)
            {
                SetMarkedDate(_StartDate);
                _StartDate = _StartDate.AddDays(1);
            }
            gv_rateAvail.EndUpdate();
        }

        //public void ClearTableAvailability()
        //{
        //    foreach (DataRow row in tableAvailability.Rows)
        //    {
        //        var index = 0;

        //        foreach (var r in row.ItemArray)
        //        {
        //            if (r.ToString().Equals("x"))
        //            {
        //                row.ItemArray[index] = string.Empty;
        //                row[index] = string.Empty;
        //            }

        //            index++;
        //        }
        //    }

        //    cdeAvailability.GetGridView().RefreshData();

        //    AvailabilityMarkedDates.Clear();

        //}

        public List<DateTime> GetMarkedDate()
        {


            return AvailabilityMarkedDates;
        }

        private void RemoveFromMainList(DateTime dt)
        {
            if (MainList.ContainsKey(dt))
                MainList.Remove(dt);

            PerformPostMainListEditionActions();
        }

        public void AddToMainListExternal(DateTime datetime, AvailabilityStatus status)
        {
            AddToMainList(datetime, status);

            //RedrawAvailability();
        }

        private void RedrawAvailability()
        {
            List<DayCO> edit = new List<DayCO>();

            foreach (DateTime time in MainList.Keys)
            {
                foreach (AvailabilityControlObject availabiliy in AvailabilityData)
                {

                    foreach (var props in availabiliy.GetType().GetProperties())
                    {

                        for (int i = 1; i < 31; i++)
                        {
                            if (props.Name == "Day" + i.ToString())
                            {
                                var DayObj = ((DayCO)props.GetValue(availabiliy));

                                if (time.Equals(DayObj.GetDate()))
                                {

                                    DayObj.Status = MainList[DayObj.GetDate()];
                                    //DayObj.Value = AvailabilityControlObject.GetStatusIndicator(DayObj.Status);
                                    edit.Add(DayObj);

                                }
                            }
                        }

                    }
                }
            }

            foreach (var e in edit)
            {
                e.Value = AvailabilityControlObject.GetStatusIndicator(e.Status);

            }

            gv_rateAvail.RefreshData();

        }

        private void PerformPostMainListEditionActions()
        {
            gv_rateAvail.PostEditor();

            gv_rateAvail.FocusedColumn = gv_rateAvail.Columns[0];

            gv_rateAvail.ViewCaption = SelectedYear + "(" + MainList.Count().ToString() + " days selected)";

        }



        private void AddToMainList(DateTime dt, AvailabilityStatus status)
        {
            if (MainList.ContainsKey(dt))
            {
                MainList[dt] = status;
                return;
            }
            else
                MainList.Add(dt, status);

            //PerformPostMainListEditionActions();

        }

        //List<DateTime> mainList = new List<DateTime>();
        Dictionary<DateTime, AvailabilityStatus> MainList = new Dictionary<DateTime, AvailabilityStatus>();

        private bool ISHoliday(int monthNumber, int dayNumber)
        {
            return false;
        }

        public List<DateTime> AvailabilityMarkedDates = new List<DateTime>();

        private bool dontFireCheckedEvent;


        public static AvailabilityStatus CurrentAvailaibityStatus = AvailabilityStatus.OPEN;

        private void ChangeStatus(AvailabilityStatus availabilityStatus)
        {
            CurrentAvailaibityStatus = availabilityStatus;
        }

        private BarSubItem bsiStatus;
        private BarCheckItem bciOpen;
        private BarCheckItem bciClosed;

        List<AvailabilityControlObject> AvailabilityData = new List<AvailabilityControlObject>();




        #endregion

        #endregion


        #region Event Handlers

        private void bbiRefresh_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (tcRevenueManagementMain.SelectedTabPage == tpRateCategory)
            {
                PopulateRateCategory(true);
            }
            else if (tcRevenueManagementMain.SelectedTabPage == tpRateHeader)
            {
                PopulateRateHeader(true);
            }
            else if (tcRevenueManagementMain.SelectedTabPage == tpRateDetail)
            {
                Progress_Reporter.Show_Progress("Loading Rate Headers", "Please Wait.......");
                //cacCode.DataSource = null;
                //gc_rateDetailHeaders.DataSource = null;
                //rateList = UIProcessManager.SelectAllRateCode();
                //cacCode.DataSource = (rateList);

                //gc_rateDetailHeaders.DataSource = rateList;
                //gc_rateDetailHeaders.RefreshDataSource();
                //gv_rateDetailHeaders.RefreshData();
                GetRateHeader();
                Progress_Reporter.Close_Progress();


            }
            else if (tcRevenueManagementMain.SelectedTabPage == tpRateAvailability)
            {
                BuildTreeListView(true);
            }
        }

        private void bbiNew_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (tcRevenueManagementMain.SelectedTabPage == tpRateCategory)
            {
                CreateNewRateCategoryPopUp();
            }
            else if (tcRevenueManagementMain.SelectedTabPage == tpRateHeader)
            {
                ResetRateHeader();
            }
            else if (tcRevenueManagementMain.SelectedTabPage == tpRateDetail)
            {
                RateCodeHeaderDTO rHeader = gv_rateDetailHeaders.GetFocusedRow() as RateCodeHeaderDTO;
                if (rHeader == null)
                {
                    SystemMessage.ShowModalInfoMessage("Please select rate header!", "ERROR");
                    return;
                }

                if (tcRateDetailChild.SelectedTabPage == tpRateDetailChild)
                {

                    //Rate Detail Creator form
                    frmRateDetailCreator creatorForm = new frmRateDetailCreator();
                    creatorForm.SelectedRateCodeHeader = new RateCodeHeaderDTO()
                    {
                        Id = rHeader.Id,
                        Description = rHeader.Description,
                        Article = rHeader.Article,
                        Consigneeunit = rHeader.Consigneeunit,
                        RateCatagory = rHeader.RateCatagory,
                        StartDate = rHeader.StartDate,
                        EndDate = rHeader.EndDate,
                        FolioText = rHeader.FolioText,
                        Market = rHeader.Market,
                        Source = rHeader.Source,
                        Comission = rHeader.Comission,
                        MinOccupancy = rHeader.MinOccupancy,
                        MaxOccupancy = rHeader.MaxOccupancy,
                        Addition = rHeader.Addition,
                        Multiplication = rHeader.Multiplication,
                        CurrencyCode = rHeader.CurrencyCode,
                        ExchangeRule = rHeader.ExchangeRule,
                        Remark = rHeader.Remark,
                    };
                    //creatorForm.AdSyncCode = _adSyncout;

                    creatorForm.RateHotelCode = rHeader.Id;
                    creatorForm.Tag = this;
                    DialogResult dialogResult = creatorForm.ShowDialog(this);

                    if (dialogResult == DialogResult.OK)
                    {
                        PopulateRateDetailGrid(rHeader.Id);
                    }
                }
                else if (tcRateDetailChild.SelectedTabPage == tpNegotiationRate)
                {
                    frmNegotiatedRate creatorForm = new frmNegotiatedRate();
                    creatorForm.RateHeaderCode = rHeader.Id;
                    //creatorForm.AdSyncout = _adSyncout;

                    DialogResult dialogResult = creatorForm.ShowDialog(this);

                    if (dialogResult == DialogResult.OK)
                    {
                        PopulateNegotiationGrid(rHeader.Id);
                        SystemMessage.ShowModalInfoMessage("Saved Successfully", "MESSAGE");
                    }
                    else if (dialogResult == DialogResult.No)
                    {
                        SystemMessage.ShowModalInfoMessage("Save Failed", "MESSAGE");
                    }
                }

                else if (tcRateDetailChild.SelectedTabPage == tpStrategy)
                {
                    frmStrategyEditor creatorForm = new frmStrategyEditor();
                    creatorForm.RateHeaderCode = rHeader.Id;
                    //creatorForm.AdSyncout = _adSyncout;
                    DialogResult dialogResult = creatorForm.ShowDialog(this);

                    if (dialogResult == DialogResult.OK)
                    {
                        PopulateRateStrategy(rHeader.Id);
                    }
                }

            }


        }

        private void bbiEdit_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (tcRevenueManagementMain.SelectedTabPage == tpRateCategory)
            {
                RateCategoryDTO dto = treeList_rateCategory.GetDataRecordByNode(treeList_rateCategory.FocusedNode) as RateCategoryDTO;
                if (dto == null)
                {
                    SystemMessage.ShowModalInfoMessage("Please select rate category!", "ERROR");
                    return;
                }
                frmRateCategory rateCategory = new frmRateCategory
                {
                    EditedRateCategory = dto,
                    Tag = this
                };

                if (rateCategory.ShowDialog(this) == DialogResult.OK)
                {
                    PopulateRateCategory(true);
                    SystemMessage.ShowModalInfoMessage("Rate Category Edited Successfully", "MESSAGE");
                }
            }
            else if (tcRevenueManagementMain.SelectedTabPage == tpRateDetail)
            {
                RateCodeHeaderDTO rHeader = gv_rateDetailHeaders.GetFocusedRow() as RateCodeHeaderDTO;
                if (rHeader == null)
                {
                    SystemMessage.ShowModalInfoMessage("Please select rate header!", "ERROR");
                    return;
                }

                if (tcRateDetailChild.SelectedTabPage == tpRateDetailChild)
                {

                    RateCodeDetailDTO rateCodeDetail = ((RateCodeDetailDTO)gv_rateDetail.GetFocusedRow());
                    if (rateCodeDetail == null)
                    {
                        SystemMessage.ShowModalInfoMessage("Please select rate detail!", "ERROR");
                        return;
                    }

                    //Rate Detail Editor form
                    frmRateDetailCreator creatorForm = new frmRateDetailCreator { Tag = this };
                    if (rateCodeDetail != null)
                    {
                        creatorForm.RateHotelCode = rHeader.Id;
                        /* rateCodeDetail.RateCodeHeader1 = new RateCodeHeaderDTO(){
                             code = rHeader.code,
                             description = rHeader.description,
                             article = rHeader.article,
                             category = rHeader.category,
                             startDate = rHeader.startDate,
                             endDate = rHeader.endDate,
                             folioText = rHeader.folioText,
                             market = rHeader.market,
                             source = rHeader.source,
                             commission = rHeader.commission,
                             minOccupancy = rHeader.minOccupancy,
                             maxOccupancy = rHeader.maxOccupancy,
                             addition = rHeader.addition,
                             multiplication = rHeader.multiplication,
                             currencyCode = rHeader.currencyCode,
                             exchangeRule = rHeader.exchangeRule,
                             remark = rHeader.remark,
                         };
                         */

                        creatorForm.EditedRateCodeDetail = rateCodeDetail;
                    }

                    //creatorForm.AdSyncCode = _adSyncout;
                    DialogResult dialogResult = creatorForm.ShowDialog(this);

                    if (dialogResult == DialogResult.OK)
                    {
                        PopulateRateDetailGrid(rHeader.Id);
                    }
                }
                else if (tcRateDetailChild.SelectedTabPage == tpNegotiationRate)
                {
                    NegotiatedViewVM negotiatedRate = ((NegotiatedViewVM)gv_negotiationRate.GetFocusedRow());

                    if (negotiatedRate == null)
                    {
                        SystemMessage.ShowModalInfoMessage("Please select negotiated rate!", "ERROR");
                        return;
                    }

                    frmNegotiatedRate creatorForm = new frmNegotiatedRate { Tag = this };
                    creatorForm.EditedNegotiatedRate = negotiatedRate;
                    creatorForm.RateHeaderCode = rHeader.Id;
                    //creatorForm.AdSyncout = _adSyncout;
                    DialogResult dialogResult = creatorForm.ShowDialog(this);

                    if (dialogResult == DialogResult.OK)
                    {
                        SystemMessage.ShowModalInfoMessage("Edited Successfully", "MESSAGE");
                        PopulateNegotiationGrid(rHeader.Id);

                    }
                    else if (dialogResult == DialogResult.No)
                    {
                        SystemMessage.ShowModalInfoMessage("Edit Failed", "ERROR");
                    }

                }

                else if (tcRateDetailChild.SelectedTabPage == tpStrategy)
                {

                    RateStrategyVM rateStrategyView = ((RateStrategyVM)gv_rateStrategy.GetFocusedRow());
                    if (rateStrategyView == null)
                    {
                        SystemMessage.ShowModalInfoMessage("Please select rate strategy!", "ERROR");
                    }

                    frmStrategyEditor creatorForm = new frmStrategyEditor { Tag = this };
                    RateStrategyDTO rateStrategy = UIProcessManager.GetRateStrategyById(rateStrategyView.Id);
                    creatorForm.EditedRateStrategy = rateStrategy;
                    creatorForm.RateHeaderCode = rHeader.Id;
                    //creatorForm.AdSyncout = _adSyncout;
                    DialogResult dialogResult = creatorForm.ShowDialog(this);

                    if (dialogResult == DialogResult.OK)
                    {
                        PopulateRateStrategy(rHeader.Id);
                    }
                }

            }


        }

        private void bbiSave_ItemClick(object sender, ItemClickEventArgs e)
        {
            OnSave();
        }

        private void bbiDelete_ItemClick(object sender, ItemClickEventArgs e)
        {
            DialogResult dr = XtraMessageBox.Show("Are you sure to delete?", "CNET_v2016", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dr == DialogResult.No) return;
            OnDelete();
        }

        private void bbiPrint_ItemClick(object sender, ItemClickEventArgs e)
        {
            ReportGenerator repGen = new ReportGenerator();
            if (tcRevenueManagementMain.SelectedTabPage == tpRateCategory)
            {
                repGen.GenerateGridReport(treeList_rateCategory, "Rate Category", CurrentTime.ToShortDateString());
            }
            else if (tcRevenueManagementMain.SelectedTabPage == tpRateDetail)
            {
                RateCodeHeaderDTO rHeader = gv_rateDetailHeaders.GetFocusedRow() as RateCodeHeaderDTO;
                if (rHeader == null)
                {
                    SystemMessage.ShowModalInfoMessage("Please select rate header!", "ERROR");
                    return;
                }
                if (tcRateDetailChild.SelectedTabPage == tpRateDetailChild)
                {

                    repGen.GenerateGridReport(gc_rateDetail, "Rate Detail list for " + rHeader.Description, CurrentTime.ToShortDateString());
                }
                else if (tcRateDetailChild.SelectedTabPage == tpNegotiationRate)
                {
                    repGen.GenerateGridReport(gc_negotiationRate, "Negotiations for " + rHeader.Description, CurrentTime.ToShortDateString());

                }

                else if (tcRateDetailChild.SelectedTabPage == tpStrategy)
                {
                    repGen.GenerateGridReport(gc_rateStrategy, "Strategies  for " + rHeader.Description, CurrentTime.ToShortDateString());

                }
            }
            //else if (tcRevenueManagementMain.SelectedTabPage == tpRateAvailability)
            //{
            //    //To Do
            //}
        }

        private void frmRevenueManagement_Load(object sender, EventArgs e)
        {
            if (!InitializeData())
            {
                ribbonControl1.Enabled = false;
                tcRevenueManagementMain.Enabled = false;
            }
        }

        private void tcRevenueManagementMain_SelectedPageChanged(object sender, TabPageChangedEventArgs e)
        {
            XtraTabControl view = sender as XtraTabControl;
            if (view.SelectedTabPage == tpRateCategory)
            {
                bbiEdit.Visibility = BarItemVisibility.Always;
                beiRateCodeHeader.Visibility = BarItemVisibility.Never;
                ribbonPageGroup4.Visible = false;
                ribbonPageGroup3.Visible = true;
                rpgRateAva.Visible = false;
                rpgRateAva2.Visible = false;

                PopulateRateCategory(false);
            }

            else if (view.SelectedTabPage == tpRateHeader)
            {
                bbiEdit.Visibility = BarItemVisibility.Never;
                beiRateCodeHeader.Visibility = BarItemVisibility.Always;
                ribbonPageGroup4.Visible = true;
                ribbonPageGroup3.Visible = true;
                rpgRateAva.Visible = false;
                rpgRateAva2.Visible = false;

                PopulateRateHeader(false);
            }

            else if (view.SelectedTabPage == tpRateDetail)
            {
                bbiEdit.Visibility = BarItemVisibility.Always;
                beiRateCodeHeader.Visibility = BarItemVisibility.Never;
                ribbonPageGroup4.Visible = false;
                ribbonPageGroup3.Visible = true;
                rpgRateAva.Visible = false;
                rpgRateAva2.Visible = false;

                if (rateList == null)
                {
                    Progress_Reporter.Show_Progress("Loading Rate Headers", "Please Wait.......");
                    //cacCode.DataSource = null;
                    //gc_rateDetailHeaders.DataSource = null;
                    //rateList = UIProcessManager.SelectAllRateCode();
                    //cacCode.DataSource = (rateList);

                    //gc_rateDetailHeaders.DataSource = rateList;
                    //gc_rateDetailHeaders.RefreshDataSource();
                    //gv_rateDetailHeaders.RefreshData();
                    GetRateHeader();


                    Progress_Reporter.Close_Progress();

                }
            }
            else if (view.SelectedTabPage == tpRateAvailability)
            {
                bbiEdit.Visibility = BarItemVisibility.Never;
                ribbonPageGroup3.Visible = false;
                rpgRateAva.Visible = true;
                //rpgRateAva2.Visible = true;

                BuildTreeListView(false);
            }

        }

        public void GetRateHeader()
        {
            // cacCode.DataSource = null;
            gc_rateDetailHeaders.DataSource = null;
            rateList = UIProcessManager.SelectAllRateCodeHeader();

            //List<RateCodeHeaderDTO> rateDTOList = ChangeRatecodeHeaderDTO(rateList);
            if (!LocalBuffer.LocalBuffer.UserHasHotelBranchAccess)
            {
                rateList = rateList.Where(x => x.Consigneeunit == LocalBuffer.LocalBuffer.CurrentDeviceConsigneeUnit).ToList();
            }
            RateHotel.DataSource = LocalBuffer.LocalBuffer.HotelBranchBufferList;
            reiRateCodeHeader.DataSource = rateList;
            reiViewRateCodeHeader.ExpandAllGroups();
            repositoryItemSearchLookUpEdit1.DataSource = rateList;
            repositoryItemSearchLookUpEdit1View.ExpandAllGroups();


            //lkhotel.DataSource = LocalBuffer.LocalBuffer.HotelBranchBufferList;
            //reiRateHeader.DataSource = rateList;
            //reivRateHeader.ExpandAllGroups();
            //repositoryItemSearchLookUpEdit1.DataSource = rateList;
            //repositoryItemSearchLookUpEdit1View.ExpandAllGroups();

            //  GetFilterRateHeader();
            //  cacCode.DataSource = rateDTOList.Where(x => x.Hotelcode == SelectedHotelcode).ToList();

            RateHotel2.DataSource = LocalBuffer.LocalBuffer.HotelBranchBufferList;
            gc_rateDetailHeaders.DataSource = rateList;
            gc_rateDetailHeaders.RefreshDataSource();
            gv_rateDetailHeaders.RefreshData();
            gv_rateDetailHeaders.ExpandAllGroups();
        }
        public void GetFilterRateHeader()
        {
            List<RateCodeHeaderDTO> rateList = UIProcessManager.GetRateCodeHeaderByconsigneeunit(SelectedHotelcode);

            cacCode.DataSource = null;
            cacCode.DataSource = rateList;

        }


        //public List<RateCodeHeaderVMDTO> ChangeRatecodeHeaderDTO(List<RateCodeHeaderDTO> rateList)
        //{
        //    string hotelcode="";
        //    List<RateCodeHeaderVMDTO> data = rateList.Select(x => new RateCodeHeaderVMDTO()
        //    { 
        //    code = x.Id,
        //    Hotel = GetHotelName(x.Id, ref hotelcode),
        //    Hotelcode = hotelcode,
        //    description = x.description,
        //    article = x.Article,
        //    category = x.RateCatagory,
        //    startDate = x.StartDate,
        //    endDate = x.EndDate,
        //    folioText = x.FolioText,
        //    market = x.Market,
        //    source = x.Source,
        //    commission = x.Comission,
        //    minOccupancy = x.MinOccupancy,
        //    maxOccupancy = x.MaxOccupancy,
        //    addition = x.Addition,
        //    multiplication = x.Multiplication,
        //    currencyCode = x.CurrencyCode,
        //    exchangeRule = x.ExchangeRule,
        //    remark = x.Remark,
        //    }).ToList();


        //    return data;
        //}
        //private string GetHotelName(int id, ref int hotelcode )
        //{
        //    string HotelName = "UnKnown Hotel";

        //    hotelcode = id;
        //    ConsigneeUnitDTO unit = LocalBuffer.LocalBuffer.HotelBranchBufferList.FirstOrDefault(x=> x.Id == id);
        //    if (unit != null)
        //    {
        //        ConsigneeDTO HotelNameDef = LocalBuffer.LocalBuffer.ConsigneeBufferList.FirstOrDefault(x=> x.Id  == unit.Consignee);

        //        if (HotelNameDef != null)
        //        {
        //            hotelcode = HotelNameDef.Id;
        //            HotelName = HotelNameDef.FirstName + " "+ HotelNameDef.SecondName;
        //        }
        //    }
        //    return  HotelName;
        //}
        /** Rate Category Event Handlers **/
        #region Rate Category Event Handlers

        private void treeList_rateCategory_DoubleClick(object sender, EventArgs e)
        {
            RateCategoryDTO dto = treeList_rateCategory.GetDataRecordByNode(treeList_rateCategory.FocusedNode) as RateCategoryDTO;
            if (dto == null)
            {
                SystemMessage.ShowModalInfoMessage("Please select rate category!", "ERROR");
                return;
            }
            frmRateCategory rateCategory = new frmRateCategory
            {
                EditedRateCategory = dto,
                Tag = this
            };

            if (rateCategory.ShowDialog(this) == DialogResult.OK)
            {
                PopulateRateCategory(true);
            }
        }

        #endregion

        /** Rate Header Event Handlers **/
        #region Rate Header Event Handlers

        private void cacRateCategoryRateHeader_EditValueChanged(object sender, EventArgs e)
        {
            if (cacRateCategoryRateHeader.EditValue != "" && cacRateCategoryRateHeader.EditValue != null)
            {
                RateCategoryDTO rateCategory = UIProcessManager.GetRateCategoryById(Convert.ToInt32(cacRateCategoryRateHeader.EditValue.ToString()));
                if (rateCategory != null)
                {
                    deBeginDateRateHeader.Properties.MinValue = rateCategory.StartDate.Value;
                    deBeginDateRateHeader.Properties.MaxValue = rateCategory.EndDate.Value;

                    deEndDateRateHeader.Properties.MinValue = rateCategory.StartDate.Value;
                    deEndDateRateHeader.Properties.MaxValue = rateCategory.EndDate.Value;

                    if (rateCategory.StartDate != null) deBeginDateRateHeader.DateTime = rateCategory.StartDate.Value;
                    if (rateCategory.EndDate != null) deEndDateRateHeader.DateTime = rateCategory.EndDate.Value;
                }
            }
            else
            {
                deBeginDateRateHeader.Properties.MinValue = CurrentTime;
                deEndDateRateHeader.Properties.MinValue = CurrentTime;
            }
        }

        private void deBeginDateRateHeader_EditValueChanged(object sender, EventArgs e)
        {
            DateTime dtSDateTime = (DateTime)deBeginDateRateHeader.EditValue;
            DateTime dtEnDateTime = deEndDateRateHeader.DateTime;

            if (dtSDateTime > dtEnDateTime) deEndDateRateHeader.DateTime = dtSDateTime.AddDays(1);
        }

        private void deEndDateRateHeader_EditValueChanged(object sender, EventArgs e)
        {
            DateTime dtSDateTime = (DateTime)deBeginDateRateHeader.EditValue;
            DateTime dtEnDateTime = deEndDateRateHeader.DateTime;

            if (dtSDateTime > dtEnDateTime) validateArrivalAndDepartureDate();
        }

        private void cacArticleTransactionRateHeader_KeyDown(object sender, KeyEventArgs e)
        {
            LookUpEdit edit = sender as LookUpEdit;
            if (e.KeyData == Keys.Back || e.KeyData == Keys.Delete)
            {
                edit.ClosePopup();
                edit.EditValue = null;

            }
            e.Handled = true;
        }

        private void cacCurrencyRateHeader_KeyDown(object sender, KeyEventArgs e)
        {
            LookUpEdit edit = sender as LookUpEdit;
            if (e.KeyData == Keys.Back || e.KeyData == Keys.Delete)
            {
                edit.ClosePopup();
                edit.EditValue = null;

            }
            e.Handled = true;
        }

        private void cacRateCategoryRateHeader_KeyDown(object sender, KeyEventArgs e)
        {
            LookUpEdit edit = sender as LookUpEdit;
            if (e.KeyData == Keys.Back || e.KeyData == Keys.Delete)
            {
                edit.ClosePopup();
                edit.EditValue = null;

            }
            e.Handled = true;
        }

        private void cacExchangeRateHeader_KeyDown(object sender, KeyEventArgs e)
        {
            LookUpEdit edit = sender as LookUpEdit;
            if (e.KeyData == Keys.Back || e.KeyData == Keys.Delete)
            {
                edit.ClosePopup();
                edit.EditValue = null;

            }
            e.Handled = true;
        }

        private void cacSourceRateHeader_KeyDown(object sender, KeyEventArgs e)
        {
            LookUpEdit edit = sender as LookUpEdit;
            if (e.KeyData == Keys.Back || e.KeyData == Keys.Delete)
            {
                edit.ClosePopup();
                edit.EditValue = null;

            }
            e.Handled = true;
        }

        private void cacMarketRateHeader_KeyDown(object sender, KeyEventArgs e)
        {
            LookUpEdit edit = sender as LookUpEdit;
            if (e.KeyData == Keys.Back || e.KeyData == Keys.Delete)
            {
                edit.ClosePopup();
                edit.EditValue = null;

            }
            e.Handled = true;
        }

        private void beiCode_EditValueChanged(object sender, EventArgs e)
        {
            //try
            //{
            //    if (beiCode.EditValue != null && !string.IsNullOrWhiteSpace(beiCode.EditValue.ToString()))
            //    {
            //        string rateCode = beiCode.EditValue.ToString();
            //        RateCodeHeader rate = UIProcessManager.SelectRateCode(rateCode);

            //        if (rate != null)
            //        {
            //            teRateDescriptionRateHeader.Text = rate.description;
            //            teFolioTextRateHeader.Text = rate.folioText;
            //            deBeginDateRateHeader.EditValue = rate.startDate;
            //            deEndDateRateHeader.EditValue = rate.endDate;
            //            cacArticleTransactionRateHeader.EditValue = rate.article;
            //            cacCurrencyRateHeader.EditValue = rate.currencyCode;
            //            cacRateCategoryRateHeader.EditValue = rate.category;
            //            cacExchangeRateHeader.EditValue = rate.exchangeRule;
            //            cacSourceRateHeader.EditValue = rate.source;
            //            cacMarketRateHeader.EditValue = rate.market;
            //            seMaximumOccupancyRateHeader.Value = Convert.ToDecimal(rate.maxOccupancy);
            //            seMininumOccupancyRateHeader.Value = Convert.ToDecimal(rate.minOccupancy);
            //            teCommisionRateHeader.Text = rate.commission.ToString();
            //            teMultiplicationRateHeader.Text = rate.multiplication.ToString();
            //            teAdditionRateHeader.Text = rate.addition.ToString();
            //            meRemarkRateHeader.Text = rate.remark;

            //            OrganizationUnit org = LocalBuffer.LocalBuffer.OrganizationUnitBufferList.FirstOrDefault(x=> x.reference == rate.code);
            //            if (org != null)
            //            {
            //                leHotel.EditValue = org.organizationUnitDefinition;
            //            }
            //            else
            //            { leHotel.EditValue = null; }

            //        }

            //        List<RoomType> roomTypeList = UIProcessManager.GetRoomTypeByRateCode(rateCode);
            //        foreach (RateRoomTypeDTO dto in rDTOList)
            //        {
            //            dto.value = false;

            //        }
            //        if (roomTypeList.Count > 0)
            //        {
            //            foreach (RoomType rt in roomTypeList)
            //            {
            //                foreach (RateRoomTypeDTO dt in rDTOList)
            //                {

            //                    if (dt.code == rt.code)
            //                    {
            //                        dt.value = true;

            //                    }

            //                }

            //            }

            //        }
            //        gc_RoomTypeRateHeader.DataSource = rDTOList;
            //        refreshRoomTypeGrid(false);

            //        List<PackageHeader> pckList = UIProcessManager.GetPackagesByRateCode(rateCode);
            //        foreach (PackageDTO dt in packageList)
            //        {
            //            dt.value = false;
            //            dt.isIncluded = true;
            //        }

            //        if (pckList.Count > 0)
            //        {
            //            foreach (PackageHeader pk in pckList)
            //            {
            //                RateCodePackage rcp = UIProcessManager.GetRateCodePackagesByRateHeader(rateCode).Where(t => t.packageHeader == pk.code).FirstOrDefault();

            //                foreach (PackageDTO dt in packageList)
            //                {

            //                    if (dt.code == pk.code)
            //                    {
            //                        dt.value = true;
            //                        if (rcp != null)
            //                        {
            //                            dt.isIncluded = rcp.packageIncluded;
            //                            dt.quantity = rcp.quantity;
            //                        }

            //                    }


            //                }


            //            }


            //        }

            //        gc_pkgSelection.DataSource = null;
            //        gc_pkgSelection.BeginUpdate();
            //        gc_pkgSelection.DataSource = packageList;
            //        gc_pkgSelection.RefreshDataSource();
            //        gc_pkgSelection.EndUpdate();
            //        refreshPackageGrid(false);

            //        List<RateComponent> rCompList = UIProcessManager.GetRateComponents(rateCode);
            //        if (rateCompList != null)
            //        {
            //            for (int i = 0; i < rateCompList.Count; i++)
            //            {
            //                clbcComponentsRateHeader.SetItemChecked(i, false);
            //            }
            //        }
            //        foreach (RateComponent rf in rCompList)
            //        {
            //            for (int i = 0; i < rateCompList.Count; i++)
            //            {
            //                if (rateCompList[i].code == rf.component)
            //                {
            //                    clbcComponentsRateHeader.SetItemChecked(i, true);
            //                }
            //            }
            //        }


            //    }
            //}
            //catch (Exception ex)
            //{
            //    SystemMessage.ShowModalInfoMessage("Exception has occured in populated rate header. Detail: " + ex.Message, "ERROR");
            //}
        }

        private void teRateDescriptionRateHeader_TextChanged(object sender, EventArgs e)
        {
            if (sender != null)
            {
                TextEdit txt = (TextEdit)sender;
                teFolioTextRateHeader.Text = txt.Text;
            }
        }
        private void cacArticleTransactionRateHeader_InvalidValue(object sender, InvalidValueExceptionEventArgs e)
        {
            e.ErrorText = "Please select valid article.";
            AutoValidate = AutoValidate.EnableAllowFocusChange;
        }
        private void teRateDescriptionRateHeader_InvalidValue(object sender, InvalidValueExceptionEventArgs e)
        {
            e.ErrorText = "Please enter rate name.";
            AutoValidate = AutoValidate.EnableAllowFocusChange;
        }

        private void gv_RoomTypeRateHeader_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            selectedRoomTypes.Clear();
            foreach (var i in gv_RoomTypeRateHeader.GetSelectedRows())
            {
                RateRoomTypeVM rowObject = gv_RoomTypeRateHeader.GetRow(i) as RateRoomTypeVM;
                selectedRoomTypes.Add(rowObject);
            }
        }

        private void KeepSelection(object sender, MouseEventArgs e)
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

        #endregion

        /** Rate Detail Event Handlers **/
        #region Rate Detail Event Handlers

        private void tcRateDetailChild_SelectedPageChanged(object sender, TabPageChangedEventArgs e)
        {

            RateCodeHeaderDTO rateCodeHeader = (RateCodeHeaderDTO)gv_rateDetailHeaders.GetFocusedRow();
            if (rateCodeHeader == null)
            {
                return;
            }

            if (tcRateDetailChild.SelectedTabPage == tpRateDetailChild)
            {
                bbiEdit.Visibility = BarItemVisibility.Always;
                PopulateRateDetailGrid(rateCodeHeader.Id);

            }
            else if (tcRateDetailChild.SelectedTabPage == tpNegotiationRate)
            {
                bbiEdit.Visibility = BarItemVisibility.Always;

                PopulateNegotiationGrid(rateCodeHeader.Id);
            }
            else if (tcRateDetailChild.SelectedTabPage == tpStrategy)
            {
                bbiEdit.Visibility = BarItemVisibility.Always;

                PopulateRateStrategy(rateCodeHeader.Id);
            }

        }

        private void gv_negotiationRate_DoubleClick(object sender, EventArgs e)
        {
            bbiEdit_ItemClick(sender, new ItemCancelEventArgs(null, null, true));

        }

        private void gv_negotiationRate_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
        {
            if (e.Column.Caption == "SN")
            {
                e.DisplayText = (e.RowHandle + 1).ToString();
            }
        }

        private void gv_rateStrategy_DoubleClick(object sender, EventArgs e)
        {
            bbiEdit_ItemClick(sender, new ItemCancelEventArgs(null, null, true));
        }

        private void gv_rateStrategy_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
        {
            if (e.Column.Caption == "SN")
            {
                e.DisplayText = (e.RowHandle + 1).ToString();
            }
        }

        private void gv_rateDetail_DoubleClick(object sender, EventArgs e)
        {

            bbiEdit_ItemClick(sender, new ItemCancelEventArgs(null, null, true));
        }

        private void gv_rateDetail_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
        {
            if (e.Column.Caption == "SN")
            {
                e.DisplayText = (e.RowHandle + 1).ToString();
            }
        }

        private void gv_rateDetailHeaders_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
        {
            GridView view = sender as GridView;
            RateCodeHeaderDTO rateCodeHeader = (RateCodeHeaderDTO)view.GetFocusedRow();
            if (rateCodeHeader == null)
            {
                return;
            }
            if (tcRateDetailChild.SelectedTabPage == tpRateDetailChild)
            {
                PopulateRateDetailGrid(rateCodeHeader.Id);
            }

            else if (tcRateDetailChild.SelectedTabPage == tpNegotiationRate)
            {
                PopulateNegotiationGrid(rateCodeHeader.Id);
            }

            else if (tcRateDetailChild.SelectedTabPage == tpStrategy)
            {
                PopulateRateStrategy(rateCodeHeader.Id);
            }

        }

        #endregion

        /** Rate Availability Event Handlers **/
        #region Rate Availability Event Handlers

        private void bbiCancelRateAvail_ItemClick(object sender, ItemClickEventArgs e)
        {
            //if (bWorker != null)
            //{
            //    bWorker.CancelAsync();
            //}
        }

        private CancellationTokenSource CancelTokenSource;
        private void PopulateRateAvail(int rateCodeDetail)
        {
            CancelTokenSource = new CancellationTokenSource();
            CancellationToken token = CancelTokenSource.Token;
            var task = Task.Factory.StartNew(() => AvailabilityWorker(rateCodeDetail), token);
            task.ContinueWith(t =>
            {

            }, token);


        }

        private void AvailabilityWorker(int rateCodeDetail)
        {
            Thread.CurrentThread.Priority = ThreadPriority.Highest;
            List<AvailabilityCalendarDTO> availabilityCalendars = UIProcessManager.GetAvailabilityCalendarBypointerandreference(CNETConstantes.TABLE_RATE_CODE_DETAIL, rateCodeDetail);
            if (availabilityCalendars == null || availabilityCalendars.Count == 0) return;

            foreach (AvailabilityCalendarDTO ac in availabilityCalendars)
            {
                AddToMainListExternal(ac.Date, ac.AvailabilityStatus == 1 ? AvailabilityStatus.OPEN : AvailabilityStatus.CLOSED);

                Invoke((MethodInvoker)(() =>
                {
                    // gc_rateAvail.DataSource = 
                }));

            }//end of foreach

        }
        private void PopualateAvailabilityCalendar(int rateCodeDetail)
        {
            List<AvailabilityCalendarDTO> availabilityCalendars = UIProcessManager.GetAvailabilityCalendarBypointerandreference(CNETConstantes.TABLE_RATE_CODE_DETAIL, rateCodeDetail);
            int taskCount = availabilityCalendars.Count();
            //int iteration = taskCount / 100;
            int count = 0;

            AvailCalHolder acHolder = new AvailCalHolder();
            acHolder.TotalTask = taskCount;

            foreach (AvailabilityCalendarDTO ac in availabilityCalendars)
            {
                //if (bWorker.CancellationPending)
                //{
                //    e.Cancel = true;
                //    return;
                //}
                ////if (iteration != 0 && bWorker != null && (count %iteration == 0) && bWorker.WorkerReportsProgress)
                //if (bWorker != null && bWorker.WorkerReportsProgress)
                //{
                //    count++;
                //    bWorker.ReportProgress(count, acHolder);
                AddToMainListExternal(ac.Date, ac.AvailabilityStatus == 1 ? AvailabilityStatus.OPEN : AvailabilityStatus.CLOSED);
                //acHolder.acDate = ac.Date;

                //}
            }//end of foreach
            PerformPostMainListEditionActions();
            RedrawAvailability();


        }

        //void bWorker_DoWork(object sender, DoWorkEventArgs e)
        //{
        //    int rateCodeDetail = (int)e.Argument;
        //    if (bWorker != null)
        //    {
        //        List<AvailabilityCalendarDTO> availabilityCalendars = UIProcessManager.GetAvailabilityCalendarBypointerandreference(CNETConstantes.TABLE_RATE_CODE_DETAIL, rateCodeDetail);
        //        int taskCount = availabilityCalendars.Count();
        //        //int iteration = taskCount / 100;
        //        int count = 0;

        //        AvailCalHolder acHolder = new AvailCalHolder();
        //        acHolder.TotalTask = taskCount;

        //        foreach (AvailabilityCalendarDTO ac in availabilityCalendars)
        //        {
        //            if (bWorker.CancellationPending)
        //            {
        //                e.Cancel = true;
        //                return;
        //            }
        //            //if (iteration != 0 && bWorker != null && (count %iteration == 0) && bWorker.WorkerReportsProgress)
        //            if (bWorker != null && bWorker.WorkerReportsProgress)
        //            {
        //                count++;
        //                bWorker.ReportProgress(count, acHolder);
        //                AddToMainListExternal(ac.Date, ac.AvailabilityStatus == 1 ? AvailabilityStatus.OPEN : AvailabilityStatus.CLOSED);
        //                acHolder.acDate = ac.Date;

        //            }




        //        }//end of foreach


        //    }
        //}

        void bWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                XtraMessageBox.Show("Cancel populating availability calendar?", "CNET_v2016", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (e.Error != null)
            {
                XtraMessageBox.Show("Error in populating availability calendar", "CNET_v2016", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                Progress_Reporter.Show_Progress("Ploting rate availability...", "Please Wait.......");
                PerformPostMainListEditionActions();
                RedrawAvailability();
                Progress_Reporter.Close_Progress();
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        void bWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            AvailCalHolder acHolder = (AvailCalHolder)e.UserState;

            Progress_Reporter.Show_Progress("Loading data for Month of " + acHolder.acDate.ToString("MMMM"), "Please Wait.......");
           
        }


        private void gridControlAvailability_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ShiftKey)
            {
                var info = gv_rateAvail.CalcHitInfo(Cursor.Position);
                if (info.InRowCell)
                {
                    if (ModifierKeys == Keys.Shift)
                    {
                        endMarkDate = new DateTime(Convert.ToInt32(gv_rateAvail.ViewCaption), info.RowHandle + 1, info.Column.VisibleIndex);
                        MessageBox.Show(endMarkDate.ToShortDateString());
                    }
                }
            }
        }
        private void frmRevenueManagement_KeyUp(object sender, KeyEventArgs e)
        {

            var info = gv_rateAvail.CalcHitInfo(Cursor.Position);

            if (ModifierKeys == Keys.Shift)
            {
                if (info.InRowCell)
                {
                    endMarkDate = new DateTime(Convert.ToInt32(gv_rateAvail.ViewCaption), info.RowHandle + 1, info.Column.VisibleIndex);
                }
            }
        }

        private void frmRevenueManagement_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
        {
            var gridView = sender as GridView;
            if (gridView != null)
            {
                AvailabilityControlObject selected = (AvailabilityControlObject)(gridView.GetRow(e.RowHandle));

                if (selected != null)
                {
                    foreach (var props in selected.GetType().GetProperties())
                    {
                        if (e.Column.VisibleIndex > 0)
                            if (props.Name == "Day" + e.Column.VisibleIndex.ToString())
                            {
                                var value = ((DayCO)props.GetValue(selected));

                                e.RepositoryItem = value.RepoItem;

                                break;
                            }
                    }
                }
            }
        }

        private void treeList_rateAvailability_FocusedNodeChanged(object sender, DevExpress.XtraTreeList.FocusedNodeChangedEventArgs e)
        {
            ClearAvailabilityCalender();
            TreeListView tlView = treeList_rateAvailability.GetDataRecordByNode(treeList_rateAvailability.FocusedNode) as TreeListView;
            if (tlView == null) return;
            int rateCodeDetail = tlView.RateDeatil;
            if (rateCodeDetail == 0) return;

            PopulateAvailabilityCalendar(rateCodeDetail);

            //if (!bWorker.IsBusy)
            //{
            //    bWorker.RunWorkerAsync(rateCodeDetail);
            //}
        }

        //close checked
        void repositoryItemCheckEdit3_CheckedChanged(object sender, EventArgs e)
        {
            var view = (CheckEdit)sender;
            if (dontFireCheckedEvent)
            {
                dontFireCheckedEvent = false;
                return;
            }

            dontFireCheckedEvent = true;

            ChangeStatus(AvailabilityStatus.CLOSED);
        }

        //open checked
        void repositoryItemCheckEdit2_CheckedChanged(object sender, EventArgs e)
        {
            var view = (CheckEdit)sender;
            if (dontFireCheckedEvent)
            {
                dontFireCheckedEvent = false;
                return;
            }

            dontFireCheckedEvent = true;

            ChangeStatus(AvailabilityStatus.OPEN);
        }


        void monthCO_DayChanged(object sender, DayStatusChangedEventArgs e)
        {
            DateTime dt = e.Day.GetDate();

            if (e.Status == AvailabilityStatus.OPEN || e.Status == AvailabilityStatus.CLOSED)
            {


                AddToMainList(dt, e.Status);


            }
            else
            {
                RemoveFromMainList(dt);

            }


        }

        void repositoryItemDateEdit1_EditValueChanged(object sender, EventArgs e)
        {
            var view = sender as DateEdit;
            if (view == null) return;
            DateTime value = view.DateTime;

        }


        private void frmRevenueManagement_MouseMove(object sender, MouseEventArgs e)
        {
            //var view = sender as GridView;
            //var info = view.CalcHitInfo(e.Location);


            //try
            //{

            //    {
            //        if (info.InRowCell)
            //        {
            //            if (info.RowHandle >= 0)
            //            {
            //                if (info.Column.VisibleIndex > 0)
            //                {

            //                    AvailabilityControlObject obj = (AvailabilityControlObject)view.GetRow(info.RowHandle);

            //                    if (ModifierKeys == Keys.Control)
            //                        foreach (var props in obj.GetType().GetProperties())
            //                        {
            //                            if (info.Column.VisibleIndex > 0)
            //                                if (props.Name == "Day" + info.Column.VisibleIndex.ToString())
            //                                {
            //                                    var value = ((DayCO)props.GetValue(obj));

            //                                    value.Value = AvailabilityControlObject.GetStatusIndicator(CurrentAvailaibityStatus);
            //                                }
            //                        }

            //                    if (ModifierKeys == Keys.Shift)
            //                    {
            //                        startMarkDate = new DateTime(Convert.ToInt32(view.ViewCaption), info.RowHandle + 1, info.Column.VisibleIndex);

            //                    }

            //                }
            //            }
            //        }


            //    }
            //    view.RefreshData();

            //}
            //catch (Exception)
            //{
            //}
        }
        private void frmRevenueManagement_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
        {
            GridView view = sender as GridView;

            AvailabilityControlObject avail = (AvailabilityControlObject)view.GetRow(e.RowHandle);

            if (e.CellValue.ToString() != AvailabilityControlObject.NOSTATUS_INDICATOR)
            {
                if (e.Column.VisibleIndex > 0)
                    foreach (var props in avail.GetType().GetProperties())
                    {
                        if (props.Name == "Day" + e.Column.VisibleIndex.ToString())
                        {
                            var dayValue = ((DayCO)props.GetValue(avail));


                            string stat = AvailabilityControlObject.GetStatusIndicator(dayValue.Status);

                            if (e.CellValue.ToString() == AvailabilityControlObject.OPEN_INDICATOR)
                            {

                                e.Appearance.BackColor = dayValue.DayColor;
                            }
                            else if (e.CellValue.ToString() == AvailabilityControlObject.CLOSED_INDICATOR)
                                e.Appearance.BackColor = Color.FromArgb(0xFF, 0x33, 0xFF);
                            else if (e.CellValue.ToString() == "X")
                                e.Appearance.BackColor = SystemColors.ActiveCaptionText;

                        }

                    }

                e.Appearance.Options.UseBackColor = true;
            }

            //if (e.CellValue.ToString() == "X")
            //{
            //    e.Appearance.BackColor = Color.Black;
            //    e.Appearance.Options.UseBackColor = true;
            //}
        }

        #endregion

        #endregion

        private class AvailCalHolder
        {
            public int TotalTask { get; set; }
            public DateTime acDate { get; set; }
        }

        private void beiHotel_EditValueChanged(object sender, EventArgs e)
        {

            //SelectedHotelcode = beiHotel.EditValue == null ? null : beiHotel.EditValue.ToString();
            //GetFilterRateHeader();
            //ResetRateHeader();
        }


        public int SelectedHotelcode { get; set; }

        private void leHotel_EditValueChanged(object sender, EventArgs e)
        {

            int SelectedHotelcodefilter = leHotel.EditValue == null ? 0 : Convert.ToInt32(leHotel.EditValue.ToString());
            LoadRoomType(SelectedHotelcodefilter);
            LoadPackages(SelectedHotelcodefilter);
        }

        private void beiRateHeader_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (beiRateCodeHeader.EditValue != null && !string.IsNullOrWhiteSpace(beiRateCodeHeader.EditValue.ToString()))
                {
                    int rateCode = Convert.ToInt32(beiRateCodeHeader.EditValue.ToString());
                    RateCodeHeaderDTO rate = UIProcessManager.GetRateCodeHeaderById(rateCode);

                    if (rate != null)
                    {
                        teRateDescriptionRateHeader.Text = rate.Description;
                        teFolioTextRateHeader.Text = rate.FolioText;
                        deBeginDateRateHeader.EditValue = rate.StartDate;
                        deEndDateRateHeader.EditValue = rate.EndDate;
                        cacArticleTransactionRateHeader.EditValue = rate.Article;
                        cacCurrencyRateHeader.EditValue = rate.CurrencyCode;
                        cacRateCategoryRateHeader.EditValue = rate.RateCatagory;
                        cacExchangeRateHeader.EditValue = rate.ExchangeRule;
                        cacSourceRateHeader.EditValue = rate.Source;
                        cacMarketRateHeader.EditValue = rate.Market;
                        seMaximumOccupancyRateHeader.Value = Convert.ToDecimal(rate.MaxOccupancy);
                        seMininumOccupancyRateHeader.Value = Convert.ToDecimal(rate.MinOccupancy);
                        teCommisionRateHeader.Text = rate.Comission.ToString();
                        teMultiplicationRateHeader.Text = rate.Multiplication.ToString();
                        teAdditionRateHeader.Text = rate.Addition.ToString();
                        meRemarkRateHeader.Text = rate.Policy;
                        leHotel.EditValue = rate.Consigneeunit;

                    }

                    List<RateCodeRoomTypeDTO> roomTypeList = UIProcessManager.GetRateCodeRoomTypeByrateCode(rateCode);
                    foreach (RateRoomTypeVM dto in rDTOList)
                    {
                        dto.value = false;

                    }
                    if (roomTypeList.Count > 0)
                    {
                        foreach (RateCodeRoomTypeDTO rt in roomTypeList)
                        {
                            foreach (RateRoomTypeVM dt in rDTOList)
                            {

                                if (dt.Id == rt.RoomType)
                                {
                                    dt.value = true;

                                }

                            }

                        }

                    }
                    gc_RoomTypeRateHeader.DataSource = rDTOList;
                    refreshRoomTypeGrid(false);

                    List<RateCodePackageDTO> pckList = UIProcessManager.GetRateCodePackagesByrateCodeHeader(rateCode);
                    foreach (PackageVM dt in packageList)
                    {
                        dt.value = false;
                        dt.isIncluded = true;
                    }

                    if (pckList.Count > 0)
                    {
                        foreach (RateCodePackageDTO pk in pckList)
                        {
                            foreach (PackageVM dt in packageList)
                            {
                                if (dt.Id == pk.PackageHeader)
                                {
                                    dt.value = true;
                                    dt.isIncluded = pk.PackageIncluded;
                                    dt.quantity = pk.Quantity;
                                }
                            }
                        }
                    }

                    gc_pkgSelection.DataSource = null;
                    gc_pkgSelection.BeginUpdate();
                    gc_pkgSelection.DataSource = packageList;
                    gc_pkgSelection.RefreshDataSource();
                    gc_pkgSelection.EndUpdate();
                    refreshPackageGrid(false);

                    List<RateComponentDTO> rCompList = UIProcessManager.GetRateComponentByrateCode(rateCode);
                    if (rateCompList != null)
                    {
                        for (int i = 0; i < rateCompList.Count; i++)
                        {
                            clbcComponentsRateHeader.SetItemChecked(i, false);
                        }
                    }
                    foreach (RateComponentDTO rf in rCompList)
                    {
                        for (int i = 0; i < rateCompList.Count; i++)
                        {
                            if (rateCompList[i].Id == rf.Pointer)
                            {
                                clbcComponentsRateHeader.SetItemChecked(i, true);
                            }
                        }
                    }


                }
            }
            catch (Exception ex)
            {
                SystemMessage.ShowModalInfoMessage("Exception has occured in populated rate header. Detail: " + ex.Message, "ERROR");
            }
        }




    }



    public enum AvailabilityStatus
    {
        OPEN = 1,
        CLOSED = 0,
        NONE = -1
    }
}