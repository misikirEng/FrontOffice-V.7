using CNET.ERP.Client.Common.UI;
using CNET.FrontOffice_V._7;
using CNET.FrontOffice_V._7.Forms.Non_Navigatable_Modals;
using CNET.FrontOffice_V._7.Forms.State_Change;
using CNET.ERP.ResourceProvider;
using DevExpress.Utils.Win;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraLayout.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using CNET_V7_Domain.Domain.ConsigneeSchema;
using CNET_V7_Domain.Domain.PmsSchema;
using CNET_V7_Domain.Domain.TransactionSchema;
using ProcessManager;
using CNET_V7_Domain.Domain.SettingSchema;
using CNET.FrontOffice_V._7.PMS.Common_Classes;
using CNET_V7_Domain.Domain.CommonSchema;
using CNET.FrontOffice_V._7.Validation;
using System.Windows.Input;
using CNET.FrontOffice_V._7.PMS.DTO;
using CNET_V7_Domain.Domain.ViewSchema;
using DevExpress.Pdf.Native.BouncyCastle.Ocsp;
using DevExpress.Mvvm.POCO;
using CNET_V7_Domain.Misc.PmsDTO;
using CNET_V7_Domain.Misc.PmsView;
using CNET.ERP.Client.UI_Logic.PMS.Forms.Setting_and_Miscellaneous.DTO;
using CNET.Progress.Reporter;
using DocumentPrint;
using DevExpress.Map.Kml.Model;
using CNET_V7_Domain.Misc;
using DevExpress.Mvvm.Native;

namespace CNET.FrontOffice_V._7.Forms
{
    //public partial class frmReservation :XtraForm

    public partial class frmReservation : UILogicBase
    {
        //bool EarlyCheckIn = false;
        //bool EarlyCheckInChargeMandatory = false;
        //DateTime EarlyCheckInUntilTime = DateTime.ParseExact("07:00", "HH:mm", CultureInfo.InvariantCulture);
        //string EarlyCheckInArticle { get; set; }
        private bool _isRegistrationDetailGenerated;

        private List<DailyRateCodeDTO> dailyRateCodeList = new List<DailyRateCodeDTO>();
        private List<TempDailyRateCode> tempList = new List<TempDailyRateCode>();
        public List<RoomTypeDTO> roomTypeList = new List<RoomTypeDTO>();
        List<GeneratedRegistrationDTO> Registrationlist { get; set; }
        private frmTravelDetail frmTravelDetail = null;



        private string guestNote = "";
        private string companyNote = "";
        private NegotiationRateDTO negoRate = null;
        private RateCodeHeaderDTO negoRateRateHeader = null;
        private string NegotiatedRateDescription = "";

        private IList<Control> _invalidControls;

        private frmNote note = null;

        //private frmDoorLock _frmDoorLock = null;

        private frmRateSearch RateSearchForm;
        private RateCodeHeaderDTO selectedHeader;
        private int CurrencyCode;

        //  private GuestDTO _savedDTO = null;

        public int SelectedHotelcode { get; set; }

        int? roomCode;
        bool isRoomSelectedBeforeRoomType = false;

        private VoucherDTO voucher = null;
        private string currentVoCode = "";
        private int lastState;
        private bool isOverBooking;
        private int _referedVoucher;

        //for welcome message
        // private string _currentRoomSpace = "";

        //default lookup codes
        private int _defDiscountReason;
        private int _defOrigion;
        private int _defBusinessSource;
        private int _defMemberType;
        private int _defReservationType;
        private int _defMarket;
        private int _defPaymentMethod;
        private int _defSpecials;
        private int _defPurpose;

        private int _adCheckin, _adSixPm, _adWaiting, _adGuaranteed, _adRateAdjust;
        private int actvDefinition;


        /** Properties **/
        private const int CP_NOCLOSE_BUTTON = 0x200;
        private int _defCreditCardType;
        private int _defCurrency;
        public DateTime CheckOutDateTime { get; set; }
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams myCp = base.CreateParams;
                myCp.ClassStyle = myCp.ClassStyle | CP_NOCLOSE_BUTTON;
                return myCp;
            }
        }

        public bool IsShare { get; set; }
        public bool IsReplicate { get; set; }
        public bool IsWalkinCheckin { get; set; }

        public SelectedGuest SelectedGuestHandler { get; set; }
        public SelectedRoom SelectedRoomHandler { get; set; }

        public RegistrationListVMDTO ExistedRegistration { get; set; }


        /////////////////////////////// CONSTRUCTOR /////////////////////////////////////
        public frmReservation(bool isWalkinCheckin)
        {
            InitializeComponent();

            IsWalkinCheckin = isWalkinCheckin;

            FormSize = new Size(950, 582);
            ApplyIcons();
            this.StartPosition = FormStartPosition.CenterScreen;
            InitializeUI();

            ConfigurationDTO Checkoutconfig = LocalBuffer.LocalBuffer.ConfigurationBufferList.FirstOrDefault(c => c.Reference == CNETConstantes.PMS_Pointer.ToString() && c.Attribute == CNETConstantes.PMS_SETTING_CheckOutTime);
            if (Checkoutconfig != null)
            {
                try
                {
                    CheckOutDateTime = Convert.ToDateTime(Checkoutconfig.CurrentValue);
                }
                catch
                {
                }
            }
            //SecurityCheck("Registration Document");

        }


        #region Helper Methods


        #region SEcurity Method

        private void EnableDisableSecuredComponents()
        {
            if (LocalBuffer.LocalBuffer.AllAccessMatrixFunctionList == null || LocalBuffer.LocalBuffer.AllAccessMatrixFunctionList.Count == 0) return;
            List<String> approvedFunctionalities = LocalBuffer.LocalBuffer.AllAccessMatrixFunctionList.Select(x => x.Description).ToList();
            if (approvedFunctionalities == null) return;
            if (!IsFunctionExists(approvedFunctionalities, "Rate Adjustment"))
            {
                lcgRateAdjustment.Enabled = false;
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

        //private List<viewFunctWithAccessM> GetAllSecuredFunctions()
        //{
        //    List<viewFunctWithAccessM> retVal = new List<viewFunctWithAccessM>();

        //    try
        //    { 
        //        String SubSystemComponent = CNETConstantes.SECURITYRegistrationDocument;
        //        int currentRole;
        //        var role = LocalBuffer.LocalBuffer.UserRoleMapperBufferList.Where(x => x.User == LocalBuffer.LocalBuffer.CurrentLoggedInUser.Id).FirstOrDefault();
        //        if (role != null)
        //            currentRole = role.Role;

        //        retVal.AddRange(UIProcessManager.GetFuncwithAccessMatView(currentRole, "Amendment", SubSystemComponent).Where(x => x.access == true).ToList());
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //    return retVal;
        //}

        #endregion

        //Initalize UI
        public void InitializeUI()
        {
            //Guest 
            GridColumn column = cacLastName.Properties.View.Columns.AddField("Id");
            column.Visible = false;
            column = cacLastName.Properties.View.Columns.AddField("Code");
            column.Visible = true;
            column = cacLastName.Properties.View.Columns.AddField("FirstName");
            column.Caption = "First Name";
            column.Visible = true;
            column = cacLastName.Properties.View.Columns.AddField("SecondName");
            column.Caption = "Middle Name";
            column.Visible = true;
            column = cacLastName.Properties.View.Columns.AddField("ThirdName");
            column.Caption = "Last Name";
            column.Visible = true;
            column = cacLastName.Properties.View.Columns.AddField("BioId");
            column.Caption = "Id Number";
            column.Visible = true;
            cacLastName.Properties.DisplayMember = "FirstName";
            cacLastName.Properties.ValueMember = "Id";

            //Contact
            GridColumn columnContact = cacContact.Properties.View.Columns.AddField("Id");
            columnContact.Visible = false;
            columnContact = cacContact.Properties.View.Columns.AddField("Code");
            columnContact.Visible = true;
            columnContact = cacContact.Properties.View.Columns.AddField("FirstName");
            columnContact.Caption = "First Name";
            columnContact.Visible = true;
            columnContact = cacContact.Properties.View.Columns.AddField("SecondName");
            columnContact.Caption = "Middle Name";
            columnContact.Visible = true;
            columnContact = cacContact.Properties.View.Columns.AddField("Tin");
            columnContact.Visible = true;
            cacContact.Properties.DisplayMember = "FirstName";
            cacContact.Properties.ValueMember = "Id";

            //Company
            GridColumn columnCompany = cacCompany.Properties.View.Columns.AddField("Id");
            columnCompany.Visible = false;
            columnCompany = cacCompany.Properties.View.Columns.AddField("Code");
            columnCompany.Visible = true;
            columnCompany = cacCompany.Properties.View.Columns.AddField("FirstName");
            columnCompany.Caption = "Name";
            columnCompany.Visible = true;
            columnCompany = cacCompany.Properties.View.Columns.AddField("Tin");
            columnCompany.Visible = true;
            cacCompany.Properties.DisplayMember = "FirstName";
            cacCompany.Properties.ValueMember = "Id";

            //Group
            GridColumn columnGroup = cacGroup.Properties.View.Columns.AddField("Id");
            columnGroup.Visible = false;
            columnGroup = cacGroup.Properties.View.Columns.AddField("Code");
            columnGroup.Visible = true;
            columnGroup = cacGroup.Properties.View.Columns.AddField("FirstName");
            columnGroup.Caption = "Name";
            columnGroup.Visible = true;
            columnGroup = cacGroup.Properties.View.Columns.AddField("Tin");
            columnGroup.Visible = true;
            cacGroup.Properties.DisplayMember = "FirstName";
            cacGroup.Properties.ValueMember = "Id";

            //Agent
            GridColumn columnAgent = cacAgent.Properties.View.Columns.AddField("Id");
            columnAgent.Visible = false;
            columnAgent = cacAgent.Properties.View.Columns.AddField("Code");
            columnAgent.Visible = true;
            columnAgent = cacAgent.Properties.View.Columns.AddField("FirstName");
            columnAgent.Caption = "Name";
            columnAgent.Visible = true;
            columnAgent = cacAgent.Properties.View.Columns.AddField("Tin");
            columnAgent.Visible = true;
            cacAgent.Properties.DisplayMember = "FirstName";
            cacAgent.Properties.ValueMember = "Id";

            //Source
            GridColumn columnSource = cacSourceHeader.Properties.View.Columns.AddField("Id");
            columnSource.Visible = false;
            columnSource = cacSourceHeader.Properties.View.Columns.AddField("Code");
            columnSource.Visible = true;
            columnSource = cacSourceHeader.Properties.View.Columns.AddField("FirstName");
            columnSource.Visible = true;
            columnSource = cacSourceHeader.Properties.View.Columns.AddField("Tin");
            columnSource.Visible = true;
            cacSourceHeader.Properties.DisplayMember = "FirstName";
            cacSourceHeader.Properties.ValueMember = "Id";


            //RTC
            cacRTC.Properties.Columns.Add(new LookUpColumnInfo("Description", "Description"));
            cacRTC.Properties.DisplayMember = "Description";
            cacRTC.Properties.ValueMember = "Id";

            //Room Type
            cacRoomType.Properties.Columns.Add(new LookUpColumnInfo("Description", "Room Types"));
            cacRoomType.Properties.DisplayMember = "Description";
            cacRoomType.Properties.ValueMember = "Id";

            //Member Type
            cacMemberType.Properties.Columns.Add(new LookUpColumnInfo("Description", "Member Type"));
            cacMemberType.Properties.DisplayMember = "Description";
            cacMemberType.Properties.ValueMember = "Id";

            //Discount Reason
            cacReason.Properties.Columns.Add(new LookUpColumnInfo("Description", "Discount Reason"));
            cacReason.Properties.DisplayMember = "Description";
            cacReason.Properties.ValueMember = "Id";

            //Origion
            cacOrigion.Properties.Columns.Add(new LookUpColumnInfo("Description", "Origin"));
            cacOrigion.Properties.DisplayMember = "Description";
            cacOrigion.Properties.ValueMember = "Id";

            //Business Source
            cacSourceFooter.Properties.Columns.Add(new LookUpColumnInfo("Description", "Business Source"));
            cacSourceFooter.Properties.DisplayMember = "Description";
            cacSourceFooter.Properties.ValueMember = "Id";

            //Reservation Type
            cacResType.Properties.Columns.Add(new LookUpColumnInfo("Description", "Reservation Type"));
            cacResType.Properties.DisplayMember = "Description";
            cacResType.Properties.ValueMember = "Id";

            //Market
            cacMarket.Properties.Columns.Add(new LookUpColumnInfo("Description", "Market"));
            cacMarket.Properties.DisplayMember = "Description";
            cacMarket.Properties.ValueMember = "Id";

            //Payment Method
            cacPayment.Properties.Columns.Add(new LookUpColumnInfo("Description", "Payment Methods"));
            cacPayment.Properties.DisplayMember = "Description";
            cacPayment.Properties.ValueMember = "Id";

            //Payment Method
            cacCreditCardTypes.Properties.Columns.Add(new LookUpColumnInfo("Description", "Payment Methods"));
            cacCreditCardTypes.Properties.DisplayMember = "Description";
            cacCreditCardTypes.Properties.ValueMember = "Id";

            //Specials
            cacSpecials.Properties.Columns.Add(new LookUpColumnInfo("Description", "Specials"));
            cacSpecials.Properties.DisplayMember = "Description";
            cacSpecials.Properties.ValueMember = "Id";

            memoPurposeTravel.Properties.DisplayMember = "Description";
            memoPurposeTravel.Properties.ValueMember = "Id";



            //Currency
            cacCurrency.Properties.Columns.Add(new LookUpColumnInfo("Description", "Name"));
            cacCurrency.Properties.DisplayMember = "Description";
            cacCurrency.Properties.ValueMember = "Id";

            //footer
            CNETFooterRibbon.ribbonControl = ribbonControl1;


            //Event Handlers
            cacRoomType.EditValueChanged += cacRoomType_EditValueChanged;
            deDeparture.EditValueChanged += DeDepartureOnEditValueChanged;
            teNights.EditValueChanged += TeNightsOnEditValueChanged;
            deArrival.EditValueChanged += DeArrivalOnEditValueChanged;
            cacLastName.EditValueChanged += cacLastName_EditValueChanged;
            SelectedGuestHandler = SelectedGuestEventHandler;
            SelectedRoomHandler = SelectedRoomEventHandler;
            cacLastName.CustomDisplayText += cacLastName_CustomDisplayText;
            deArrival.DateTimeChanged += deArrival_DateTimeChanged;
            rirgStatus.SelectedIndexChanged += rirgStatus_SelectedIndexChanged;
            spinEdit1.InvalidValue += spinEdit1_InvalidValue;
            cacCompany.EditValueChanged += cacCompany_EditValueChanged;
            //cacContact.Popup += cacContact_Popup;
            //cacCompany.Popup += cacCompany_Popup;
            //cacAgent.Popup += cacAgent_Popup;
            //cacGroup.Popup += cacGroup_Popup;
            //cacSourceHeader.Popup += cacSourceHeader_Popup;

            statLblGuestNote.Click += tsstStatus_Click;
            statLblCompanyNote.Click += tsstStatusCompany_Click;

            //enable disable components
            rpgReservationDetail.Enabled = false;


            reiHotel.DisplayMember = "Name";
            reiHotel.ValueMember = "Id";
            reiHotel.DataSource = (LocalBuffer.LocalBuffer.HotelBranchBufferList.Select(x => new { x.Id, x.Name })).ToList();

            if (LocalBuffer.LocalBuffer.HotelBranchBufferList != null && LocalBuffer.LocalBuffer.HotelBranchBufferList.Count > 0)
            {
                beiHotel.EditValue = LocalBuffer.LocalBuffer.CurrentDeviceConsigneeUnit;
                reiHotel.ReadOnly = !LocalBuffer.LocalBuffer.UserHasHotelBranchAccess;
            }
            //beiHotel.EditValue = LocalBuffer.LocalBuffer.CurrentDevice;
        }

        DateTime? CurrentTime = DateTime.Now;
        // Initialilze Data
        public bool InitializeData()
        {
            try
            {
                // Progress_Reporter.Show_Progress("Initializing data", "Please Wait...");

                CurrentTime = UIProcessManager.GetServiceTime();
                if (CurrentTime == null)
                {
                    ////CNETInfoReporter.Hide();
                    return false;
                }

                //disable some UI components based on the security functionality
                EnableDisableSecuredComponents();


                //check workflow

                ActivityDefinitionDTO workFlowRateAdjust = UIProcessManager.GetActivityDefinitionBydescriptionandreference(CNETConstantes.LU_ACTIVITY_DEFINATION_RATE_ADJUSTED, CNETConstantes.REGISTRATION_VOUCHER).FirstOrDefault();

                if (workFlowRateAdjust != null)
                {

                    _adRateAdjust = workFlowRateAdjust.Id;
                }
                else
                {
                    ////CNETInfoReporter.Hide();
                    SystemMessage.ShowModalInfoMessage("Please define workflow of RATE ADJUSTED for Registration Voucher ", "ERROR");
                    return false;
                }



                if (IsWalkinCheckin || IsReplicate || IsShare/*&& !IsReplicate && !IsShare*/)
                {

                    ActivityDefinitionDTO workFlow = UIProcessManager.GetActivityDefinitionBydescriptionandreference(CNETConstantes.LU_ACTIVITY_DEFINATION_CheckIN, CNETConstantes.REGISTRATION_VOUCHER).FirstOrDefault();

                    if (workFlow != null)
                    {
                        _adCheckin = workFlow.Id;
                    }
                    else
                    {
                        ////CNETInfoReporter.Hide();
                        SystemMessage.ShowModalInfoMessage("Please define workflow of CHECK-IN for Registration Voucher ", "ERROR");
                        return false;
                    }

                    //Check Activity Previlage
                    var userRoleMapper = LocalBuffer.LocalBuffer.UserRoleMapperBufferList.FirstOrDefault(r => r.User == LocalBuffer.LocalBuffer.CurrentLoggedInUser.Id);
                    if (userRoleMapper != null)
                    {
                        var roleActivity = UIProcessManager.GetRoleActivityByactivityDefinition(_adCheckin).FirstOrDefault(r => r.Role == userRoleMapper.Role && r.NeedsPassCode);
                        if (roleActivity != null)
                        {
                            frmNeedPassword frmNeedPass = new frmNeedPassword(true);
                            frmNeedPass.ShowDialog();
                            if (!frmNeedPass.IsAutenticated)
                            {
                                ////CNETInfoReporter.Hide();
                                return false;
                            }
                        }
                    }
                }
                else if (!IsWalkinCheckin/* && !IsReplicate && !IsShare*/)
                {
                    //Six PM
                    ActivityDefinitionDTO workFlowSixPM = UIProcessManager.GetActivityDefinitionBydescriptionandreference(CNETConstantes.LU_ACTIVITY_DEFINATION_6PM, CNETConstantes.REGISTRATION_VOUCHER).FirstOrDefault();


                    if (workFlowSixPM != null)
                    {

                        _adSixPm = workFlowSixPM.Id;
                    }
                    else
                    {
                        ////CNETInfoReporter.Hide();
                        SystemMessage.ShowModalInfoMessage("Please define workflow of SIX PM for Registration Voucher ", "ERROR");
                        return false;
                    }

                    //Check Activity Previlage
                    var userRoleMapper = LocalBuffer.LocalBuffer.UserRoleMapperBufferList.FirstOrDefault(r => r.User == LocalBuffer.LocalBuffer.CurrentLoggedInUser.Id);
                    if (userRoleMapper != null)
                    {
                        var roleActivity = UIProcessManager.GetRoleActivityByactivityDefinition(_adSixPm).FirstOrDefault(r => r.Role == userRoleMapper.Role && r.NeedsPassCode);
                        if (roleActivity != null)
                        {
                            frmNeedPassword frmNeedPass = new frmNeedPassword(true);

                            frmNeedPass.ShowDialog();
                            if (!frmNeedPass.IsAutenticated)
                            {
                                ////CNETInfoReporter.Hide();
                                return false;
                            }

                        }

                    }

                    //Waiting 
                    ActivityDefinitionDTO workFlowWaitingList = UIProcessManager.GetActivityDefinitionBydescriptionandreference(CNETConstantes.LU_ACTIVITY_DEFINATION_Waitinglist, CNETConstantes.REGISTRATION_VOUCHER).FirstOrDefault();


                    if (workFlowWaitingList != null)
                    {

                        _adWaiting = workFlowWaitingList.Id;
                    }
                    else
                    {
                        ////CNETInfoReporter.Hide();
                        SystemMessage.ShowModalInfoMessage("Please define workflow of WAITING LIST for Registration Voucher ", "ERROR");
                        return false;
                    }

                    //Guaranted
                    ActivityDefinitionDTO workFlowGuaranteed = UIProcessManager.GetActivityDefinitionBydescriptionandreference(CNETConstantes.LU_ACTIVITY_DEFINATION_Guaranteed, CNETConstantes.REGISTRATION_VOUCHER).FirstOrDefault();


                    if (workFlowGuaranteed != null)
                    {

                        _adGuaranteed = workFlowGuaranteed.Id;
                    }
                    else
                    {
                        ////CNETInfoReporter.Hide();
                        SystemMessage.ShowModalInfoMessage("Please define workflow of GUARANTEED for Registration Voucher ", "ERROR");
                        return false;
                    }
                }


                if (IsWalkinCheckin)
                {
                    bbiSave.Visibility = BarItemVisibility.Never;
                    bbiCheckIn.Visibility = BarItemVisibility.Always;
                    teNoOfRooms.Properties.ReadOnly = true;

                    /*
                    //Check Configuration
                    var EarlyCheckInConfig = LocalBuffer.LocalBuffer.ConfigurationBufferList.FirstOrDefault(c => c.reference == CNETConstantes.PMS && c.attribute == CNETConstantes.PMS_SETTING_EarlyCheckIn);
                    if (EarlyCheckInConfig != null)
                    {
                        EarlyCheckIn = Convert.ToBoolean(EarlyCheckInConfig.currentValue);
                    }
                    if (EarlyCheckIn)
                    {
                        var EarlyCheckInChargeMandatoryConfig = LocalBuffer.LocalBuffer.ConfigurationBufferList.FirstOrDefault(c => c.reference == CNETConstantes.PMS && c.attribute == CNETConstantes.PMS_SETTING_EarlyCheckInChargeMandatory);
                        if (EarlyCheckInChargeMandatoryConfig != null)
                        {
                            EarlyCheckInChargeMandatory = Convert.ToBoolean(EarlyCheckInChargeMandatoryConfig.currentValue);
                        }
                        
                        var EarlyCheckInUntilTimeConfig = LocalBuffer.LocalBuffer.ConfigurationBufferList.FirstOrDefault(c => c.reference == CNETConstantes.PMS && c.attribute == CNETConstantes.PMS_SETTING_EarlyCheckInUntilTime);
                        if (EarlyCheckInUntilTimeConfig != null)
                        {
                            try
                            {
                                EarlyCheckInUntilTime = EarlyCheckInUntilTimeConfig == null ? DateTime.ParseExact("06:30", "HH:mm", CultureInfo.InvariantCulture) : DateTime.ParseExact(EarlyCheckInUntilTimeConfig.currentValue, "HH:mm", CultureInfo.InvariantCulture);
                            }
                            catch
                            {
                               ////CNETInfoReporter.Hide();
                                SystemMessage.ShowModalInfoMessage("Please Fix Early Check-In Until Time Setting !!", "ERROR");
                                return false;
                            }
                        }
                        else
                        {
                           ////CNETInfoReporter.Hide();
                            SystemMessage.ShowModalInfoMessage("Please Fix Early Check-In Until Time Setting !!", "ERROR");
                            return false;
                        }

                        var EarlyCheckInArticleConfig = LocalBuffer.LocalBuffer.ConfigurationBufferList.FirstOrDefault(c => c.reference == CNETConstantes.PMS && c.attribute == CNETConstantes.PMS_SETTING_EarlyCheckInArticle);
                        if (EarlyCheckInArticleConfig != null && !string.IsNullOrEmpty(EarlyCheckInArticleConfig.currentValue.ToString()))
                        {
                            var Article = UIProcessManager.SelectArticle(EarlyCheckInArticleConfig.currentValue);
                            if (Article != null)
                            {
                                EarlyCheckInArticle = EarlyCheckInArticleConfig.currentValue.ToString();

                            }
                            else
                            {
                               ////CNETInfoReporter.Hide();
                                SystemMessage.ShowModalInfoMessage("Please Fix Early Check-In Article Setting !!"+ Environment.NewLine+"The Article Doesn't Exist.", "ERROR");
                                return false;
                            }
                        }
                        else
                        {
                           ////CNETInfoReporter.Hide();
                            SystemMessage.ShowModalInfoMessage("Please Fix Early Check-In Article Setting !!", "ERROR");
                            return false;
                        }

                    }*/
                    //if (EarlyCheckIn)
                    //{
                    //    deArrival.Enabled = true;
                    //    deArrival.Properties.ReadOnly = false;
                    //    deArrival.Properties.MinValue = DateTime.Now.AddDays(-1);
                    //    deArrival.Properties.MaxValue = CurrentTime.Value;
                    //    deDeparture.Properties.MinValue = CurrentTime.Value;
                    //}
                    //else
                    //{
                    deArrival.Enabled = false;
                    deArrival.Properties.ReadOnly = true;
                    deArrival.Properties.MinValue = CurrentTime.Value;
                    deDeparture.Properties.MinValue = CurrentTime.Value;
                    //rpgIssueCard.Visible = true;
                    //}

                    rpgState.Visible = false;
                    this.Text = "CHECK-IN";
                }
                else
                {
                    this.Text = "RESERVATION";
                    deArrival.Properties.MinValue = CurrentTime.Value;
                    deDeparture.Properties.MinValue = CurrentTime.Value;
                }

                #region Populate Lookup Searches

                //Guest List
                cacLastName.Properties.DataSource = LocalBuffer.LocalBuffer.AllGuestConsigneeViewlist;

                ////Room Type List
                //roomTypeList = UIProcessManager.SelectAllRoomType().Where(r => r.isActive && (r.activationDate != null && r.activationDate.Value.Date <= CurrentTime.Value.Date)).ToList();
                //cacRoomType.Properties.DataSource = (roomTypeList);
                //cacRTC.Properties.DataSource = roomTypeList;

                //TO DO: Member Type
                List<LookupDTO> memberType = LocalBuffer.LocalBuffer.LookUpBufferList.Where(l => l.Type == CNETConstantes.MEMBER_TYPE && l.IsActive).ToList();
                cacMemberType.Properties.DataSource = memberType;
                if (memberType != null)
                {
                    var memberTypeDefault = memberType.FirstOrDefault(c => c.IsDefault);
                    if (memberTypeDefault != null)
                    {
                        cacMemberType.EditValue = (memberTypeDefault.Id);
                        _defMemberType = memberTypeDefault.Id;
                    }
                }

                // Discount Reason
                List<LookupDTO> discReasonList = LocalBuffer.LocalBuffer.LookUpBufferList.Where(l => l.Type == CNETConstantes.DISCOUNT_REASONS && l.IsActive).ToList();
                cacReason.Properties.DataSource = discReasonList;
                if (discReasonList != null)
                {
                    var disReasonDefault = discReasonList.FirstOrDefault(c => c.IsDefault);
                    if (disReasonDefault != null)
                    {
                        cacReason.EditValue = (disReasonDefault.Id);
                        _defDiscountReason = disReasonDefault.Id;
                    }
                }

                //Origion
                List<LookupDTO> origionList = LocalBuffer.LocalBuffer.LookUpBufferList.Where(l => l.Type == CNETConstantes.LIST_OF_ORGIN && l.IsActive).ToList();
                cacOrigion.Properties.DataSource = origionList;
                if (origionList != null)
                {
                    var origionDefault = origionList.FirstOrDefault(c => c.IsDefault);
                    if (origionDefault != null)
                    {
                        cacOrigion.EditValue = (origionDefault.Id);
                        _defOrigion = origionDefault.Id;
                    }
                }


                //Business Source 
                List<LookupDTO> busSoureceList = LocalBuffer.LocalBuffer.LookUpBufferList.Where(l => l.Type == CNETConstantes.BUSSINESS_SOURCE && l.IsActive).ToList();
                if (busSoureceList != null)
                {
                    cacSourceFooter.Properties.DataSource = (busSoureceList.OrderByDescending(c => c.IsDefault).ToList());
                    var busSourceDefault = busSoureceList.FirstOrDefault(c => c.IsDefault);
                    if (busSourceDefault != null)
                    {
                        cacSourceFooter.EditValue = (busSourceDefault.Id);
                        _defBusinessSource = busSourceDefault.Id;
                    }
                }

                //Reservation Type List
                List<SystemConstantDTO> resTypeList = LocalBuffer.LocalBuffer.SystemConstantDTOBufferList.Where(l => l.Category == CNETConstantes.RESERVATION_TYPE && l.IsActive).ToList();
                cacResType.Properties.DataSource = resTypeList;
                if (resTypeList != null)
                {
                    var resTypeDefault = resTypeList.FirstOrDefault(c => c.IsDefault);
                    if (resTypeDefault != null)
                    {
                        cacResType.EditValue = (resTypeDefault.Id);
                        _defReservationType = resTypeDefault.Id;
                    }

                }

                //Market 
                List<LookupDTO> marketList = LocalBuffer.LocalBuffer.LookUpBufferList.Where(l => l.Type == CNETConstantes.MARKET && l.IsActive).ToList();
                cacMarket.Properties.DataSource = marketList;
                if (marketList != null)
                {
                    var marketDefault = marketList.FirstOrDefault(c => c.IsDefault);
                    if (marketDefault != null)
                    {
                        cacMarket.EditValue = (marketDefault.Id);
                        _defMarket = marketDefault.Id;
                    }
                }

                //Payment Methods
                List<SystemConstantDTO> paymentList = LocalBuffer.LocalBuffer.SystemConstantDTOBufferList.Where(l => l.Category == CNETConstantes.PAYMENT_METHODS && l.IsActive).ToList();
                if (paymentList != null)
                {
                    paymentList = paymentList.Where(
                            r =>
                                r.Id == CNETConstantes.PAYMENTMETHODSCASH ||
                                //r.code == CNETConstantes.PAYMENTMETHODS_DIRECT_BILL ||
                                r.Id == CNETConstantes.PAYMNET_METHOD_CREDITCARD).ToList();
                    cacPayment.Properties.DataSource = (paymentList.OrderByDescending(c => c.IsDefault).ToList());
                    var paymentDefault = paymentList.FirstOrDefault(c => c.IsDefault);
                    if (paymentDefault != null)
                    {
                        cacPayment.EditValue = (paymentDefault.Id);
                        _defPaymentMethod = paymentDefault.Id;
                    }
                }

                //credit card type
                List<SystemConstantDTO> creditCardTypes = LocalBuffer.LocalBuffer.SystemConstantDTOBufferList.Where(l => l.Category == CNETConstantes.CREDIT_CARD_TYPES && l.IsActive).ToList();
                if (paymentList != null)
                {
                    cacCreditCardTypes.Properties.DataSource = creditCardTypes;
                    var creditTypeDef = creditCardTypes.FirstOrDefault(l => l.IsDefault);
                    if (creditTypeDef != null)
                    {
                        cacCreditCardTypes.EditValue = (creditTypeDef.Id);
                        _defCreditCardType = creditTypeDef.Id;
                    }
                }

                //currency
                List<CurrencyDTO> currencyList = LocalBuffer.LocalBuffer.CurrencyBufferList;
                cacCurrency.Properties.DataSource = currencyList;
                if (currencyList != null)
                {
                    var defCurrency = currencyList.FirstOrDefault(c => c.IsDefault == true);
                    if (defCurrency != null)
                    {
                        cacCurrency.EditValue = defCurrency.Id;
                        _defCurrency = defCurrency.Id;

                    }
                }

                //Specials
                List<LookupDTO> specialsList = LocalBuffer.LocalBuffer.LookUpBufferList.Where(l => l.Type == CNETConstantes.SPECIALS && l.IsActive).ToList();

                cacSpecials.Properties.DataSource = specialsList;
                if (specialsList != null)
                {
                    var specialsDefault = specialsList.FirstOrDefault(c => c.IsDefault);
                    if (specialsDefault != null)
                    {
                        cacSpecials.EditValue = (specialsDefault.Id);
                        _defSpecials = specialsDefault.Id;
                    }
                }


                //Purpose of Travel
                List<LookupDTO> PurposeList = LocalBuffer.LocalBuffer.LookUpBufferList.Where(l => l.Type == CNETConstantes.TravelPurpose && l.IsActive).ToList();

                if (PurposeList != null)
                {
                    memoPurposeTravel.Properties.DataSource = PurposeList;
                    var PurposeDefault = PurposeList.FirstOrDefault(c => c.IsDefault);
                    if (PurposeDefault != null)
                    {
                        memoPurposeTravel.EditValue = (PurposeDefault.Id);
                        _defPurpose = PurposeDefault.Id;
                    }
                }




                //Initialize Other Consigness 
                GetAllContacts();
                GetAllSources();
                GetAllAgents();
                GetAllGroups();
                GetAllCompanies();


                if (IsWalkinCheckin && IsReplicate)
                {
                    cacResType.EditValue = CNETConstantes.RESERVATIONTYPECHECKIN;

                    // state list
                    var states = LocalBuffer.LocalBuffer.ObjectStateDefinitionBufferList.Where(s =>
                          s.Id == CNETConstantes.SIX_PM_STATE ||
                          s.Id == CNETConstantes.OSD_WAITLIST_STATE ||
                          s.Id == CNETConstantes.GAURANTED_STATE ||
                          s.Id == CNETConstantes.CHECKED_IN_STATE
                          ).ToList();
                    cacObjectState.DataSource = states;

                    beiStatus.BeginUpdate();
                    beiStatus.EditValue = CNETConstantes.CHECKED_IN_STATE;
                    beiStatus.Refresh();
                    beiStatus.EndUpdate();



                }
                else
                {
                    //it is either replication with reservation or only reservation
                    if (!IsWalkinCheckin)
                    {
                        // state list
                        var states = LocalBuffer.LocalBuffer.ObjectStateDefinitionBufferList.Where(s =>
                              s.Id == CNETConstantes.SIX_PM_STATE ||
                              s.Id == CNETConstantes.OSD_WAITLIST_STATE ||
                              s.Id == CNETConstantes.GAURANTED_STATE
                              ).ToList();
                        cacObjectState.DataSource = states;

                        beiStatus.BeginUpdate();
                        beiStatus.EditValue = CNETConstantes.SIX_PM_STATE;
                        beiStatus.Refresh();
                        beiStatus.EndUpdate();
                    }

                }

                #endregion

                deExpDate.Properties.MinValue = CurrentTime.Value;
                deExpDate.EditValue = CurrentTime.Value;
                teNoOfRooms.Text = "1";
                deArrival.DateTime = CurrentTime.Value;
                deDeparture.DateTime = CurrentTime.Value.AddDays(1);
                teNights.Text = "1";

                if (!IsReplicate)
                {
                    rgFactor.EditValue = @"value";
                }

                /** Populate Shares **/
                if (IsShare)
                {
                    if (ExistedRegistration == null)
                    {
                        ////CNETInfoReporter.Hide();
                        SystemMessage.ShowModalInfoMessage("Please select a registration to share!", "ERROR");
                        return false;
                    }
                    beRoom.ButtonClick -= beRoom_ButtonClick;
                    beRoom.Enabled = false;
                    this.Text = this.Text + @" (Share)";
                    _referedVoucher = ExistedRegistration.Id;

                    RegistrationDetailDTO regDetail = UIProcessManager.GetRegistrationDetailByvoucher(ExistedRegistration.Id).LastOrDefault();
                    if (regDetail != null)
                    {
                        cacRoomType.EditValue = regDetail.RoomType;
                        //beRateCode.EditValue = regDetail.rateCodeDetail;
                        //teRate.Text = (regDetail.rateAmount != null ? regDetail.rateAmount.Value : 0.0m).ToString();
                        beRoom.EditValue = regDetail.Room;
                        roomCode = regDetail.Room;
                        beRoom.Text = ExistedRegistration.Room;
                        cacRoomType.Enabled = false;
                        beRoom.Enabled = false;

                    }

                }

                /** Populate Replication **/
                if (IsReplicate)
                {
                    rpgState.Visible = true;
                    if (ExistedRegistration == null)
                    {
                        ////CNETInfoReporter.Hide();
                        SystemMessage.ShowModalInfoMessage("Please select a registration to replicate!", "ERROR");
                        return false;
                    }
                    this.Text = this.Text + @" (Replicate)";
                    deArrival.DateTime = CurrentTime.Value;
                    PopulateReservation(ExistedRegistration.Id);

                    deArrival.Properties.BeginUpdate();

                    if (ExistedRegistration.Arrival.Date < CurrentTime.Value.Date || ExistedRegistration.Arrival.Date > CurrentTime.Value.Date)
                    {
                        deArrival.DateTime = CurrentTime.Value;
                    }
                    else
                    {
                        deArrival.DateTime = ExistedRegistration.Arrival;
                    }

                    deArrival.RefreshEditValue();

                    deArrival.Properties.EndUpdate();

                    deDeparture.Properties.BeginUpdate();

                    if (ExistedRegistration.Departure.Date < deArrival.DateTime.Date)
                    {
                        deDeparture.DateTime = deArrival.DateTime.AddDays(1);
                    }
                    else
                    {
                        deDeparture.DateTime = ExistedRegistration.Departure;
                    }

                    deDeparture.RefreshEditValue();
                    deDeparture.Properties.EndUpdate();
                }

                //Generated Id to display
                currentVoCode = UIProcessManager.IdGenerater("Voucher", CNETConstantes.REGISTRATION_VOUCHER, 0, LocalBuffer.LocalBuffer.CurrentDeviceConsigneeUnit.Value, false, LocalBuffer.LocalBuffer.CurrentDevice.Id);

                if (currentVoCode != null)
                {
                    this.Text = this.Text + @" - " + currentVoCode;
                }
                else
                {
                    ////CNETInfoReporter.Hide();
                    SystemMessage.ShowModalInfoMessage("There is a problem on id setting!!!", "ERROR");
                    return false;
                }

                ////CNETInfoReporter.Hide();
                return true;

            }
            catch (Exception ex)
            {
                ////CNETInfoReporter.Hide();
                XtraMessageBox.Show("Error in initializing reservation form. DETAIL::" + ex.Message, "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        // Apply Icons
        private void ApplyIcons()
        {
            Image Image = Provider.GetImage("New", ProviderType.APPLICATIONICON, PictureSize.Dimension_32X32);

            bbiNew.Glyph = Image;
            bbiNew.LargeGlyph = Image;

            Image = Provider.GetImage("Save", ProviderType.APPLICATIONICON, PictureSize.Dimension_32X32);

            bbiSave.Glyph = Image;
            bbiSave.LargeGlyph = Image;


            Image = Provider.GetImage("Search", ProviderType.APPLICATIONICON, PictureSize.Dimension_32X32);

            bbiSearch.Glyph = Image;
            bbiSearch.LargeGlyph = Image;

        }

        // Get All Companies
        private void GetAllCompanies()
        {

            cacCompany.Properties.DataSource = LocalBuffer.LocalBuffer.AllCustomerConsigneeViewlist;
        }

        // Get All Groups
        private void GetAllGroups()
        {

            var orgGroupList = LocalBuffer.LocalBuffer.ConsigneeViewBufferList.Where(o => o.GslType == CNETConstantes.GROUP && o.IsActive).ToList();
            cacGroup.Properties.DataSource = orgGroupList;
        }

        // Get All Agents
        private void GetAllAgents()
        {
            var orgAgentList = LocalBuffer.LocalBuffer.ConsigneeViewBufferList.Where(o => o.GslType == CNETConstantes.AGENT && o.IsActive == true).ToList();
            cacAgent.Properties.DataSource = orgAgentList;
        }

        // Get All Sources
        private void GetAllSources()
        {
            var orgSoureceList = LocalBuffer.LocalBuffer.ConsigneeViewBufferList.Where(o => o.GslType == CNETConstantes.BUSINESSsOURCE && o.IsActive).ToList();

            cacSourceHeader.Properties.DataSource = orgSoureceList;
        }

        // Get All Contacts
        private void GetAllContacts()
        {

            var contactList = LocalBuffer.LocalBuffer.ConsigneeViewBufferList.Where(p => p.IsActive && p.GslType == CNETConstantes.CONTACT).ToList();

            cacContact.Properties.DataSource = contactList;
        }


        // Populate Reservation
        private void PopulateReservation(int regVoucher)
        {
            // Progress_Reporter.Show_Progress("Populating fields. Please Wait...");

            VoucherDTO vo = UIProcessManager.GetVoucherById(regVoucher);
            if (vo != null)
            {
                cacLastName.EditValue = vo.Consignee1;
                if (vo.LastState != CNETConstantes.CHECKED_OUT_STATE && vo.LastState != CNETConstantes.OSD_CANCEL_STATE)
                {
                    beiStatus.BeginUpdate();
                    beiStatus.EditValue = vo.LastState;
                    beiStatus.Refresh();

                    beiStatus.EndUpdate();
                }

                if (vo.LastState == CNETConstantes.CHECKED_IN_STATE)
                {
                    lastState = vo.LastState;
                    beiStatus.BeginUpdate();
                    beiStatus.EditValue = vo.LastState;
                    beiStatus.Refresh();

                    beiStatus.EndUpdate();
                    deArrival.Enabled = false;
                }

            }

            List<RegistrationDetailDTO> regDetailList = UIProcessManager.GetRegistrationDetailByvoucher(regVoucher);

            if (regDetailList != null)
            {
                RegistrationDetailDTO regDetail = regDetailList.FirstOrDefault();
                spinEdit1.Text = regDetail.Child.ToString();
                spinEdit2.Text = regDetail.Adult.ToString();
                cacRoomType.EditValue = regDetail.RoomType;
                cacRTC.EditValue = regDetail.ActualRtc;
                teValue.Text = regDetail.Adjustment.ToString();
                teNoOfRooms.Text = regDetail.RoomCount.ToString();
                RateCodeDetailDTO RateDetailByVo = UIProcessManager.GetRateCodeDetailById(regDetail.RateCode.Value);
                if (RateDetailByVo != null)
                {
                    RateCodeHeaderDTO rateCode = UIProcessManager.GetRateCodeHeaderById(RateDetailByVo.RateCodeHeader);
                    if (rateCode != null)
                    {
                        beRateCode.Text = rateCode.Description;
                    }
                }
            }

            //Rate Ajdustment
            RateAdjustmentDTO rateAdjust = UIProcessManager.GetRateAdjustmentByvoucher(regVoucher);
            if (rateAdjust != null)
            {

                if (rateAdjust.IsPercent)
                {
                    rgFactor.Properties.BeginUpdate();
                    rgFactor.EditValue = "percent";
                    rgFactor.Refresh();
                    rgFactor.Properties.EndUpdate();

                    tePercent.Properties.BeginUpdate();
                    tePercent.Text = rateAdjust.Amount.ToString();
                    tePercent.Properties.EndUpdate();


                    teValue.Text = rateAdjust.Value.ToString();
                    teValue.Refresh();

                }
                else
                {
                    rgFactor.EditValue = "value";
                    teAmount.Text = rateAdjust.Amount.ToString();
                    teValue.Text = rateAdjust.Value.ToString();
                }

                cacReason.EditValue = rateAdjust.Reason;
            }
            else
            {
                rgFactor.Properties.BeginUpdate();
                rgFactor.EditValue = @"value";
                rgFactor.Refresh();
                rgFactor.Properties.EndUpdate();
            }
            if (vo.Consignee2 != null)
                cacCompany.EditValue = vo.Consignee2;
            if (vo.Consignee4 != null)
                cacContact.EditValue = vo.Consignee4;
            if (vo.Consignee3 != null)
                cacAgent.EditValue = vo.Consignee3;
            if (vo.Consignee6 != null)
                cacGroup.EditValue = vo.Consignee6;
            if (vo.Consignee5 != null)
                cacSourceHeader.EditValue = vo.Consignee5;



            deArrival.DateTime = vo.StartDate.Value;
            deDeparture.DateTime = vo.EndDate.Value;
            cacSpecials.EditValue = vo.Extension3;
            cacResType.EditValue = vo.Type;
            cacOrigion.EditValue = vo.Extension2;
            memoPurposeTravel.EditValue = vo.Purpose;

            //List<NonCashTransactionDTO> nonCashTransactionList = UIProcessManager.GetNonCashTransactionByvoucher(vo.Id);
            //if (nonCashTransactionList != null && nonCashTransactionList.Count > 0)
            //{
            //    var nonCashTransaction = nonCashTransactionList.FirstOrDefault();
            //    cacPayment.EditValue = nonCashTransaction.PaymentMethod;
            //    if (!IsReplicate)
            //    {
            //        if (nonCashTransaction.MaturityDate != null) deExpDate.EditValue = nonCashTransaction.MaturityDate.Value;
            //        teCreditCardNo.Text = nonCashTransaction.Number;
            //    }


            //    // cacCurr.EditValue = nonCashTransactionList.currencyCode;
            //}

            //if (!IsReplicate)
            //{
            //    List<VoucherExtensionTransaction> voExtenTransList = UIProcessManager.SelectAllVoucherExtensionTransaction();
            //    if (voExtenTransList != null)
            //    {
            //        List<VoucherExtensionTransaction> filtered = voExtenTransList.Where(e => e.voucher == regVoucher).ToList();
            //        foreach (VoucherExtensionTransaction voExt in filtered)
            //        {
            //            if (voExt.voucherExtension == CNETConstantes.VOUCHER_EXT_CRSNO)
            //            {
            //                teCRSNo.Text = voExt.number;
            //            }
            //            else if (voExt.voucherExtension == CNETConstantes.VOUCHER_EXT_ApprovalCode)
            //            {
            //                teApprovalCode.Text = voExt.number;
            //            }
            //            else if (voExt.voucherExtension == CNETConstantes.VOUCHER_EXT_ApprovalAmount)
            //            {
            //                teApprovalAmt.Text = voExt.number;
            //            }
            //            else if (voExt.voucherExtension == CNETConstantes.VOUCHER_EXT_VoucherNo)
            //            {
            //                teVoucherNum.Text = voExt.number;
            //            }

            //        }
            //    }
            //}

        }

        // Auto Complete Free Rooms
        private void AutoCompleteFreeRooms()
        {
            if (cacRoomType.EditValue != null && cacRoomType.EditValue != "")
            {
                AutoCompleteStringCollection collection = new AutoCompleteStringCollection();
                if (cacRoomType.EditValue != null && !string.IsNullOrEmpty(cacRoomType.EditValue.ToString()))
                {
                    List<int> stateList = new List<int>() { CNETConstantes.CHECKED_IN_STATE, CNETConstantes.OSD_WAITLIST_STATE, CNETConstantes.GAURANTED_STATE, CNETConstantes.OSD_WAITLIST_STATE };//, CNETConstantes.OSD_Category_Transaction };

                    //  List<int> stateList = new List<int>() { CNETConstantes.CHECKED_OUT_STATE, CNETConstantes.OSD_CANCEL_STATE };
                    List<VwVoucherDetailWithRoomDetailViewDTO> avRooms = UIProcessManager.GetAvailabeRoomsByDateAndState(deArrival.DateTime.Date, deDeparture.DateTime.Date, stateList, Convert.ToInt32(cacRoomType.EditValue)).ToList();
                    if (avRooms != null && avRooms.Count > 0)
                    {
                        string[] roomsListdesStrings = avRooms.Select(r => r.RoomDescription).ToArray();
                        collection.AddRange(roomsListdesStrings);
                        beRoom.MaskBox.AutoCompleteSource = AutoCompleteSource.CustomSource;
                        beRoom.MaskBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                        beRoom.MaskBox.AutoCompleteCustomSource = collection;
                    }
                }
            }
        }

        private void PopulateMoreFields(int voId, string voCode, DateTime CurrentTime)
        {
            voucher = new VoucherDTO
            {
                Code = voCode,
                Type = Convert.ToInt32(cacResType.EditValue), //beginning Voucher
                Definition = CNETConstantes.REGISTRATION_VOUCHER, //Registration Voucher
                OriginConsigneeUnit = SelectedHotelcode,
                Consignee1 = Convert.ToInt32(cacLastName.EditValue),
                Consignee2 = (cacCompany.EditValue == null || string.IsNullOrEmpty(cacCompany.EditValue.ToString())) ? null : Convert.ToInt32(cacCompany.EditValue),
                Consignee3 = (cacAgent.EditValue == null || string.IsNullOrEmpty(cacAgent.EditValue.ToString())) ? null : Convert.ToInt32(cacAgent.EditValue),
                Consignee4 = (cacSourceHeader.EditValue == null || string.IsNullOrEmpty(cacSourceHeader.EditValue.ToString())) ? null : Convert.ToInt32(cacSourceHeader.EditValue),
                Consignee5 = (cacContact.EditValue == null || string.IsNullOrEmpty(cacContact.EditValue.ToString())) ? null : Convert.ToInt32(cacContact.EditValue),
                Consignee6 = (cacGroup.EditValue == null || string.IsNullOrEmpty(cacGroup.EditValue.ToString())) ? null : Convert.ToInt32(cacGroup.EditValue),
                IssuedDate = CurrentTime,
                Year = CurrentTime.Year,
                Month = CurrentTime.Month,
                Day = CurrentTime.Day,
                IsIssued = true,
                IsVoid = false,
                GrandTotal = 0,
                Period = LocalBuffer.LocalBuffer.GetPeriodCode(CurrentTime),
                StartDate = (DateTime?)deArrival.EditValue,
                EndDate = (DateTime?)deDeparture.EditValue,
                HasEffect = false,
                Extension2 = (cacOrigion.EditValue == null || string.IsNullOrEmpty(cacOrigion.EditValue.ToString())) ? null : cacOrigion.EditValue.ToString(),
                Extension3 = (cacSpecials.EditValue == null || string.IsNullOrEmpty(cacSpecials.EditValue.ToString())) ? null : cacSpecials.EditValue.ToString(),
            };
            if (IsWalkinCheckin && !IsReplicate)
            {
                voucher.LastState = CNETConstantes.CHECKED_IN_STATE;
                actvDefinition = _adCheckin;

            }
            else
            {
                int? regState;
                if (isOverBooking)
                {
                    regState = CNETConstantes.OSD_WAITLIST_STATE;
                    isOverBooking = false;

                }
                else
                {
                    regState = Convert.ToInt32(beiStatus.EditValue);
                }
                if (regState == null)
                {
                    SystemMessage.ShowModalInfoMessage("Please set object state of reservation type", "ERROR");
                    return;
                }
                voucher.LastState = regState.Value;


                //guaranted state

                if (regState == CNETConstantes.GAURANTED_STATE)
                {
                    voucher.LastState = CNETConstantes.GAURANTED_STATE;
                    actvDefinition = _adGuaranteed;
                }
                else if (regState == CNETConstantes.OSD_WAITLIST_STATE)
                {
                    voucher.LastState = CNETConstantes.OSD_WAITLIST_STATE;
                    actvDefinition = _adWaiting;

                }
                else if (regState == CNETConstantes.CHECKED_IN_STATE)
                {
                    voucher.LastState = CNETConstantes.CHECKED_IN_STATE;
                    actvDefinition = _adCheckin;
                }
                else
                {
                    voucher.LastState = CNETConstantes.SIX_PM_STATE;
                    actvDefinition = _adSixPm;
                }
            }


            if (!string.IsNullOrEmpty(voCode))
            {
                //RegistrationExtension registrationHeader = new RegistrationExtension
                //{
                //    code = String.Empty, // only on algorithm invokation
                //    arrivalDate = (DateTime?)deArrival.EditValue,
                //    departureDate = (DateTime?)deDeparture.EditValue,
                //    voucher = voCode, // voucher (assign)
                //    origin = cacOrigion.EditValue == null ? "" : cacOrigion.EditValue.ToString(),
                //    resType = cacResType.EditValue == null ? "" : cacResType.EditValue.ToString(),
                //    Specials = cacSpecials.EditValue == null ? "" : cacSpecials.EditValue.ToString(),
                //    ismaster = false,
                //    remark = cacMemberType.EditValue == null ? "" : cacMemberType.EditValue.ToString()
                //};


                RegistrationDetailDTO registrationDetail = new RegistrationDetailDTO
                {
                    Date = CurrentTime,
                    Adult = (int?)spinEdit2.Value,
                    Child = (int?)spinEdit1.Value,
                    RoomCount = Convert.ToInt32(teNoOfRooms.Text),
                    RoomType = Convert.ToInt32(cacRoomType.EditValue),
                    Room = roomCode
                };

                if (teRate.Text != String.Empty)
                {
                    registrationDetail.RateAmount = Convert.ToDecimal(teRate.Text);
                }
                if (rgFactor != null && rgFactor.EditValue.ToString() == "value")
                {
                    registrationDetail.Adjustment = !string.IsNullOrEmpty(teAmount.Text) ? Convert.ToDecimal(teAmount.Text) : 0;
                }
                else
                {
                    decimal adjustMent = !string.IsNullOrEmpty(tePercent.Text) ? Convert.ToDecimal(tePercent.Text) : 0;
                    adjustMent = adjustMent / 100;
                    registrationDetail.Adjustment = dailyRateCodeList.Average(r => r.UnitRoomRate) * adjustMent;
                }
                registrationDetail.ActualRtc = Convert.ToInt32(cacRTC.EditValue);
                registrationDetail.IsFixedRate = ceFixedRate.Checked;
                registrationDetail.Market = Convert.ToInt32(cacMarket.EditValue);
                registrationDetail.Source = Convert.ToInt32(cacSourceFooter.EditValue);

                if (ceFixedRate.Checked)
                {
                    Registrationlist = UIProcessManager.RegistrationDetailGenerator(voId, (DateTime)deArrival.EditValue, (DateTime)deDeparture.EditValue, registrationDetail, GetDailyRateCode(dailyRateCodeList));
                }
                else
                {
                    dailyRateCodeList.Clear();
                    foreach (var tm in tempList)
                    {
                        DailyRateCodeDTO daily = new DailyRateCodeDTO();
                        daily.DayWeek = tm.DayWeek;
                        daily.Description = tm.Description;
                        daily.RateCodeDetail = tm.RateCodeDetail;
                        daily.StayDate = tm.StayDate;
                        daily.UnitRoomRate = tm.UnitRoomRate;
                        dailyRateCodeList.Add(daily);
                    }
                    Registrationlist = UIProcessManager.RegistrationDetailGenerator(voId, (DateTime)deArrival.EditValue, (DateTime)deDeparture.EditValue, registrationDetail, dailyRateCodeList);

                }


                _isRegistrationDetailGenerated = true;
            }
        }

        private List<DailyRateCodeDTO> GetDailyRateCode(IReadOnlyList<DailyRateCodeDTO> dailyRateCodes)
        {
            List<DailyRateCodeDTO> tempDailyRateCodes = dailyRateCodes.ToList();
            if (RateSearchForm != null)
            {
                int i = 0;
                foreach (var da in tempDailyRateCodes)
                {
                    da.UnitRoomRate = RateSearchForm.FirstNightAmount;
                    i++;
                }

            }
            return tempDailyRateCodes;
        }

        private void SelectedGuestEventHandler(ConsigneeDTO person)
        {
            cacLastName.EditValue = person.Id;
            teTitle.EditValue = person.Title;
            tePassPort.EditValue = person.BioId;

        }

        private void SelectedRoomEventHandler(RoomDetailDTO room)
        {
            if (room != null)
            {
                beRoom.Tag = room.Id;
                beRoom.EditValue = room.Description;
                roomCode = room.Id;
                if (cacRoomType.EditValue == null || cacRoomType.EditValue == "")
                {

                    //_currentRoomSpace = room.space;
                    isRoomSelectedBeforeRoomType = true;
                    cacRoomType.EditValue = room.RoomType;

                }

            }
        }

        private void SendWelcomeMessage(string roomSpace, string voucherCode, string consignee, DateTime CurrentTime)
        {
        }

        public void ResetForm()
        {
            DateTime? CurrentTime = UIProcessManager.GetServiceTime();

            if (CurrentTime == null)
            {
                this.Close();
            }

            //Generated Id to display 
            string currentVoCode = UIProcessManager.IdGenerater("Voucher", CNETConstantes.REGISTRATION_VOUCHER, 0, LocalBuffer.LocalBuffer.CurrentDeviceConsigneeUnit.Value, false, LocalBuffer.LocalBuffer.CurrentDevice.Id);

            if (!string.IsNullOrEmpty(currentVoCode))
            {
                currentVoCode = currentVoCode;
                if (IsReplicate)
                    this.Text = IsWalkinCheckin ? "CHECK IN (Replicate) - " + currentVoCode : "RESERVATION (Replicate) - " + currentVoCode;
                else if (IsShare)
                    this.Text = IsWalkinCheckin ? "CHECK IN (Share) - " + currentVoCode : "RESERVATION (Share) - " + currentVoCode;
                else
                    this.Text = IsWalkinCheckin ? "CHECK IN - " + currentVoCode : "RESERVATION - " + currentVoCode;
            }
            else
            {
                ////CNETInfoReporter.Hide();
                SystemMessage.ShowModalInfoMessage("There is a problem on id setting!!!", "ERROR");
                this.Close();
            }

            teRate.Properties.Appearance.BackColor = Color.White;
            NegotiatedRateDescription = "";
            RateSearchForm = null;
            cacLastName.EditValue = "";
            teTitle.Text = "";
            tePassPort.Text = "";
            teValue.Text = "";
            beRoom.Text = "";
            beRoom.EditValue = null;
            beRoom.Tag = null;
            beRateCode.Text = "";
            teRate.Text = "";
            cbePackages.Properties.Items.Clear();
            layoutControlItem44.AppearanceItemCaption.ForeColor = Color.Black;
            // teLanguage.Text = "";
            //teVIP.Text = "";
            teCountry.Text = "";
            beAddress.Text = "";
            cacCompany.EditValue = "";
            cacContact.EditValue = "";
            cacSourceHeader.EditValue = "";
            cacAgent.EditValue = "";
            cacGroup.EditValue = "";
            cacMemberType.EditValue = "";
            deArrival.DateTime = CurrentTime == null ? DateTime.Now : CurrentTime.Value;
            teNights.Text = "1";
            spinEdit1.Value = 0;
            spinEdit2.Value = 1;
            teNoOfRooms.Text = "1";
            cacRoomType.EditValue = "";
            cacRoomType.EditValue = null;
            cacRTC.EditValue = "";
            ceFixedRate.Checked = true;
            teCreditCardNo.Text = "";
            teCRSNo.Text = "";
            deExpDate.EditValue = CurrentTime == null ? DateTime.Now : CurrentTime.Value;
            teApprovalAmt.Text = "";
            teApprovalCode.Text = "";
            teAmount.Text = "";
            teVoucherNum.Text = "";

            memoPurposeTravel.EditValue = _defPurpose;
            cacReason.EditValue = _defDiscountReason;
            cacOrigion.EditValue = _defOrigion;
            cacSourceFooter.EditValue = _defBusinessSource;
            cacMarket.EditValue = _defMarket;
            cacResType.EditValue = _defReservationType;
            cacPayment.EditValue = _defPaymentMethod;
            cacCreditCardTypes.EditValue = _defCreditCardType;
            cacSpecials.EditValue = _defSpecials;
            cacCurrency.EditValue = _defCurrency;
            if (IsWalkinCheckin)
            {
                cacResType.EditValue = CNETConstantes.RESERVATIONTYPECHECKIN;
            }

            _isRegistrationDetailGenerated = false;
            if (dailyRateCodeList != null)
                dailyRateCodeList.Clear();
            if (tempList != null)
                tempList.Clear();
            if (Registrationlist != null)
                Registrationlist.Clear();
            ;

            frmTravelDetail = null;

        }

        // on save clicked
        public void OnSave()
        {
            // Progress_Reporter.Show_Progress("Saving...", 10);

            String isSaved = "";
            ResponseModel<VoucherBuffer> isSavedV = null;

            try
            {
                //
                // TODO: Message to be displayed
                //
                List<Control> controls = new List<Control>
                {
                    cacLastName,
                    beRateCode,
                    cacRTC,
                    cacRoomType,
                    cacResType
                };
                int payCode = cacPayment.EditValue != null ? Convert.ToInt32(cacPayment.EditValue) : 0;

                if (cacPayment.EditValue != "")
                {
                    if (payCode == CNETConstantes.PAYMNET_METHOD_CREDITCARD)
                    {
                        controls.Add(teCreditCardNo);
                    }
                    controls.Add(deExpDate);
                }
                string title = this.Text.ToUpper();
                if (IsWalkinCheckin)
                {
                    controls.Add(beRoom);
                }

                if (cacLastName.EditValue == null)
                    cacLastName.EditValue = "";

                IList<Control> invalidControls = CustomValidationRule.Validate(controls);

                if (invalidControls.Count > 0)
                {
                    _isRegistrationDetailGenerated = false;
                    ////CNETInfoReporter.Hide();
                    return;

                }

                //Get Current Date and Time
                DateTime? CurrentTime = UIProcessManager.GetServiceTime();
                if (CurrentTime == null)
                {
                    _isRegistrationDetailGenerated = false;
                    ////CNETInfoReporter.Hide();
                    return;
                }

                if (IsWalkinCheckin && !LocalBuffer.LocalBuffer.EarlyCheckIn)
                {
                    DateTime startDate = (DateTime)deArrival.EditValue;
                    if (startDate.Date != CurrentTime.Value.Date)
                    {
                        XtraMessageBox.Show("Date has been changed since the reservation form opended. Please re-open the form!", "CNET_v2016", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        this.Close();
                        return;
                    }

                }

                int currentNoOfNumbers = 0;
                int availableRoomNum = 0;
                if (!string.IsNullOrEmpty(teNoOfRooms.Text))
                {
                    List<RoomTypeDTO> roomTypes = new List<RoomTypeDTO>();
                    RoomTypeDTO rt = UIProcessManager.GetRoomTypeById(Convert.ToInt32(cacRoomType.EditValue));
                    roomTypes.Add(rt);
                    List<Tuple<string, int>> availableRoomsMin = UIProcessManager.GetAvailableRoomCount(roomTypes, deArrival.DateTime.Date, deDeparture.DateTime.Date);
                    // Tuple<string, int, string> availableRooms = UIProcessManager.GetTotalAvailableRoom(deArrival.DateTime, deDeparture.DateTime, roomTypes).FirstOrDefault();
                    if (availableRoomsMin != null)
                    {
                        availableRoomNum = availableRoomsMin.FirstOrDefault().Item2;// Convert.ToInt32(availableRooms.Item3);
                    }
                    currentNoOfNumbers = Convert.ToInt32(teNoOfRooms.Text);
                }
                if (currentNoOfNumbers > availableRoomNum)
                {
                    if (!IsShare)
                    {
                        DialogResult dr = MessageBox.Show(@"There is no enough room for the specified room type. Do you want to continue with  overbooking?", @"Overbooking", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                        if (dr == DialogResult.Yes)
                        {
                            isOverBooking = true;
                        }
                        else
                        {
                            ////CNETInfoReporter.Hide();
                            return;

                        }
                    }
                }



                string currentVoCode = UIProcessManager.IdGenerater("Voucher", CNETConstantes.REGISTRATION_VOUCHER, 1, LocalBuffer.LocalBuffer.CurrentDeviceConsigneeUnit.Value, false, LocalBuffer.LocalBuffer.CurrentDevice.Id);

                if (!string.IsNullOrEmpty(currentVoCode))
                {
                    currentVoCode = currentVoCode;
                }
                else
                {
                    SystemMessage.ShowModalInfoMessage("There is a problem on id setting!!!", "ERROR");
                    ////CNETInfoReporter.Hide();
                    return;

                }


                if (!_isRegistrationDetailGenerated)
                {
                    PopulateMoreFields(0, currentVoCode, CurrentTime.Value);
                }


                if (Registrationlist == null || Registrationlist.Count == 0)
                {
                    _isRegistrationDetailGenerated = false;
                    SystemMessage.ShowModalInfoMessage("No registration detail generated. Please check the rate code given.", "ERROR");
                    ////CNETInfoReporter.Hide();
                    return;
                }
                VoucherBuffer voucherBufferDTO = new VoucherBuffer();
                if (voucher != null)
                {
                    //Check Voucher Availabillity
                    VoucherDTO vouch = UIProcessManager.GetVoucherByCode(voucher.Code);
                    if (vouch != null)
                    {
                        SystemMessage.ShowModalInfoMessage("There is a problem on voucher Id setting. There is existing voucher with the current code -> " + voucher.Code, "ERROR");
                        ////CNETInfoReporter.Hide();
                        return;

                    }
                    voucherBufferDTO.Voucher = voucher;
                    voucherBufferDTO.Voucher.Code = currentVoCode;
                    // isSavedV = UIProcessManager.CreateVoucher(voucher);

                }
                DateTime? departureDate = ((DateTime?)deDeparture.EditValue).Value.Date.AddHours(CheckOutDateTime.Hour).AddMinutes(CheckOutDateTime.Minute);
                voucherBufferDTO.Voucher.StartDate = (DateTime?)deArrival.EditValue;
                voucherBufferDTO.Voucher.EndDate = departureDate;
                voucherBufferDTO.Voucher.PaymentMethod = cacPayment.EditValue == null ? CNETConstantes.PAYMENTMETHODSCASH : Convert.ToInt32(cacPayment.EditValue);

                voucherBufferDTO.Voucher.Purpose = memoPurposeTravel.EditValue == null ? null : Convert.ToInt32(memoPurposeTravel.EditValue);

                /* 
                 RegistrationExtension registrationHeader = new RegistrationExtension
                 {
                     code = String.Empty, // only on algorithm invokation
                     arrivalDate = (DateTime?)deArrival.EditValue,
                     departureDate = departureDate,
                     OrganizationUnitDefinition = SelectedHotelcode,
                     voucher = currentVoCode, // voucher (assign)
                     origin = cacOrigion.EditValue == null ? "" : cacOrigion.EditValue.ToString(),
                     resType = cacResType.EditValue == null ? "" : cacResType.EditValue.ToString(),
                     paymentType = cacPayment.EditValue == null ? CNETConstantes.PAYMENTMETHODSCASH : cacPayment.EditValue.ToString(),
                     Specials = cacSpecials.EditValue == null ? "" : cacSpecials.EditValue.ToString(),
                     ismaster = false,
                     remark = memoPurposeTravel.EditValue == null ? "" : memoPurposeTravel.EditValue.ToString()
                 };
                */

                //if (!IsShare && !IsReplicate)
                //{
                if (actvDefinition != null && actvDefinition > 0)
                {
                    voucherBufferDTO.Activity = ActivityLogManager.SetupActivity(CurrentTime.Value, actvDefinition, CNETConstantes.PMS_Pointer, "");
                }
                //}
                //else
                //{
                //    voucherBufferDTO.Activity = new ActivityDTO();

                //}
                if (selectedHeader != null)
                {
                    if (selectedHeader.ExchangeRule == CNETConstantes.ReservationDayRate)
                    {
                        if (!IsWalkinCheckin)
                            voucherBufferDTO.Voucher.ExchangeRate = UIProcessManager.GetLatestExchangeRate(CurrencyCode);
                    }
                    else if (selectedHeader.ExchangeRule == CNETConstantes.ArrivalDayRate)
                    {
                        if (IsWalkinCheckin)
                            voucherBufferDTO.Voucher.ExchangeRate = UIProcessManager.GetLatestExchangeRate(CurrencyCode);
                    }
                    else
                    {
                        voucherBufferDTO.Voucher.ExchangeRate = null;
                    }
                }


                voucherBufferDTO.Voucher.LastUser = LocalBuffer.LocalBuffer.CurrentLoggedInUser.Id;
                voucherBufferDTO.Voucher.LastDevice = LocalBuffer.LocalBuffer.CurrentDevice.Id;
                voucherBufferDTO.TransactionCurrencyBuffer = null;

                isSavedV = UIProcessManager.CreateVoucherBuffer(voucherBufferDTO);

                if (isSavedV != null && isSavedV.Success)
                {
                    //Save Activity

                    //updateidsetting 


                    if (!String.IsNullOrEmpty(currentVoCode))
                    {


                        // Registration Previledge
                        RegistrationPrivllegeDTO previlage = new RegistrationPrivllegeDTO();
                        previlage.Voucher = isSavedV.Data.Voucher.Id;
                        previlage.Nopost = false;
                        previlage.AuthorizeDirectBill = false;
                        previlage.PreStayCharging = false;
                        previlage.PostStayCharging = false;
                        previlage.AllowLatecheckout = false;
                        previlage.AuthorizeKeyReturn = false;
                        previlage.Remark = "";

                        //Payment, Credit card, Expired date
                        if (cacPayment.EditValue != null && !string.IsNullOrEmpty(cacPayment.EditValue.ToString()))
                        {

                            //If It is Direct Billing, Save to Registration Privileges
                            if (payCode == CNETConstantes.PAYMENTMETHODS_DIRECT_BILL)
                            {
                                previlage.AuthorizeDirectBill = true;
                            }
                            else if (payCode != CNETConstantes.PAYMENTMETHODSCASH)
                            {
                                /*
                                NonCashTransactionDTO nonCahTrans = new NonCashTransactionDTO(); 
                                nonCahTrans.Voucher = isSavedV.Voucher.Id;
                                nonCahTrans.Consignee = Convert.ToInt32( cacLastName.EditValue);
                                nonCahTrans.PaymentMethod = Convert.ToInt32(cacPayment.EditValue);
                                nonCahTrans.PaymentProcessor = cacCreditCardTypes.EditValue == null ? null : Convert.ToInt32(cacCreditCardTypes.EditValue);
                                nonCahTrans.Index = 1;
                                nonCahTrans.IssueDate = CurrentTime.Value;
                                nonCahTrans.MaturityDate = deExpDate.EditValue != null ? Convert.ToDateTime(deExpDate.EditValue) : CurrentTime;
                                if (teCreditCardNo.Text != "")
                                {
                                    nonCahTrans.Number = teCreditCardNo.Text;
                                }
                                else
                                {
                                    nonCahTrans.Number = "No Number given";
                                }
                                nonCahTrans.Currency = cacCurrency.EditValue == null ? CurrencyCode : Convert.ToInt32(cacCurrency.EditValue);
                                nonCahTrans.Amount = 0;
                                nonCahTrans.Executed = true;
                                NonCashTransactionDTO isNonCashTranSaved = UIProcessManager.CreateNonCashTransaction(nonCahTrans);

                                if (isNonCashTranSaved != null)
                                {

                                    //save voucher extension and voucher extension transactions
                                    if (!string.IsNullOrEmpty(teCRSNo.Text))
                                    {
                                        isSavedV.Voucher.Extension1 = teCRSNo.Text;
                                    }
                                    if (!string.IsNullOrEmpty(teApprovalCode.Text))
                                    {
                                        isSavedV.Voucher.Extension2 = teApprovalCode.Text;
                                    }
                                    if (!string.IsNullOrEmpty(teApprovalAmt.Text))
                                    {
                                        isSavedV.Voucher.Extension3 = teCRSNo.Text;
                                    }

                                }
                                else
                                {
                                    XtraMessageBox.Show("Non-Cash Payment transactions are not saved!", "Non-cash payment", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                }*/
                            }
                        }

                        //save registration Previledge
                        RegistrationPrivllegeDTO isP = UIProcessManager.CreateRegistrationPrivllege(previlage);

                        /*
                                                // UIProcessManager.getValueFactorDefination()
                                                ValueFactorDTO valueF = new ValueFactorDTO
                                                {
                                                    Code = String.Empty,
                                                    Pointer = CNETConstantes.PERSON,
                                                    Reference = cacLastName.EditValue.ToString(),
                                                    ValueFactorDefinition = "OBJVAFA703864",
                                                    Remark = cacReason.Text
                                                };
                                                UIProcessManager.CreateValueFactor(valueF);
                        */

                        //Travel Detail
                        if (frmTravelDetail != null)
                        {
                            if (frmTravelDetail.AddedTravelDetails != null)
                            {
                                foreach (var td in frmTravelDetail.AddedTravelDetails)
                                {
                                    td.Voucher = isSavedV.Data.Voucher.Id;
                                    UIProcessManager.CreateTravelDetail(td);
                                }
                            }
                            //MasterPageForm.LoadTravelDetailBuffer();
                        }

                        //if (selectedHeader != null)
                        //{
                        //    if (selectedHeader.ExchangeRule == CNETConstantes.ReservationDayRate)
                        //    {
                        //        if (!IsWalkinCheckin)
                        //            isSavedV.Voucher.ExchangeRate = UIProcessManager.ApplyExchangeRate(isSavedV.Voucher.Id, CNETConstantes.ReservationDayRate, "Reservation", CurrencyCode);
                        //    }
                        //    else if (selectedHeader.ExchangeRule == CNETConstantes.ArrivalDayRate)
                        //    {
                        //        if (IsWalkinCheckin)
                        //            isSavedV.Voucher.ExchangeRate = UIProcessManager.ApplyExchangeRate(isSavedV.Voucher.Id, CNETConstantes.ArrivalDayRate, "CheckIn", CurrencyCode);
                        //    }
                        //    else
                        //    {
                        //        isSavedV.Voucher.ExchangeRate = null;
                        //    }
                        //}

                        //#region Registration Header, Registration Detail, Package To Post
                        //
                        // TODO: call algorithm
                        //



                        //create rate adjustment
                        RateAdjustmentDTO rateAdjustment = new RateAdjustmentDTO();
                        rateAdjustment.Voucher = isSavedV.Data.Voucher.Id;
                        rateAdjustment.StartDate = deArrival.DateTime;
                        rateAdjustment.EndDate = deDeparture.DateTime;
                        if (rgFactor.EditValue != null && rgFactor.EditValue.ToString() == "value")
                        {
                            rateAdjustment.IsPercent = false;
                            if (!string.IsNullOrWhiteSpace(teAmount.Text))
                            {
                                rateAdjustment.Amount = Convert.ToDecimal(teAmount.Text);

                            }
                            else
                            {
                                rateAdjustment.Value = 0;
                            }
                        }
                        else
                        {
                            rateAdjustment.IsPercent = true;

                            if (!string.IsNullOrWhiteSpace(tePercent.Text))
                            {
                                rateAdjustment.Amount = Convert.ToDecimal(tePercent.Text);
                            }
                            else
                            {
                                rateAdjustment.Value = 0;

                            }
                        }


                        if (!string.IsNullOrWhiteSpace(teValue.Text))
                        {
                            rateAdjustment.Value = Convert.ToDecimal(teValue.Text);
                        }
                        else
                        {
                            rateAdjustment.Value = 0;
                        }
                        rateAdjustment.Reason = cacReason.EditValue == null ? null : Convert.ToInt32(cacReason.EditValue);
                        rateAdjustment.Remark = "";
                        if (rateAdjustment.Value != 0)
                        {
                            if (UIProcessManager.CreateRateAdjustment(rateAdjustment) == null)
                            {
                                SystemMessage.ShowModalInfoMessage("There is error saving the rate adjustment history.", "MESSAGE");
                            }
                            else
                            {
                                ActivityDTO activity = ActivityLogManager.SetupActivity(CurrentTime.Value, _adRateAdjust, CNETConstantes.PMS_Pointer, "");
                                string remark = "";
                                if (rgFactor.EditValue != null && rgFactor.EditValue.ToString() == "value")
                                {

                                    remark = "Amount: " + rateAdjustment.Amount;

                                }
                                else if (rgFactor.EditValue != null && rgFactor.EditValue.ToString() == "percent")
                                {

                                    remark = "Percent: " + rateAdjustment.Amount;

                                }
                                activity.Remark = remark;
                                activity.Reference = isSavedV.Data.Voucher.Id;
                                UIProcessManager.CreateActivity(activity);
                            }

                        }

                        // the next registrationDetails comes from grid
                        //_registrationDetails = new List<RegistrationDetail>();
                        foreach (GeneratedRegistrationDTO GeneratedDetail in Registrationlist)
                        {

                            GeneratedDetail.registrationDetail.Adjustment = rateAdjustment.Amount;

                            GeneratedDetail.registrationDetail.Voucher = isSavedV.Data.Voucher.Id;

                            RegistrationDetailDTO newRegistration = UIProcessManager.CreateRegistrationDetail(GeneratedDetail.registrationDetail);

                            //Save Package to Post

                            if (newRegistration != null)
                            {
                                if (GeneratedDetail.packagesToPost != null)
                                {
                                    foreach (PackagesToPostDTO packagetoPost in GeneratedDetail.packagesToPost)
                                    {
                                        packagetoPost.RegistrationDetail = newRegistration.Id;
                                        UIProcessManager.CreatePackagesToPost(packagetoPost);
                                    }
                                }
                            }
                        }

                    }




                    // DialogResult = System.Windows.Forms.DialogResult.OK;
                    if (IsShare)
                    {
                        //  List<RelationDTO> relationList = new List<RelationDTO>();
                        RelationDTO relation = new RelationDTO();
                        relation.RelationType = CNETConstantes.LK_ROOM_SHARE;
                        relation.ReferencedObject = _referedVoucher;
                        relation.ReferringObject = isSavedV.Data.Voucher.Id;
                        relation.RelationLevel = 1;
                        //relationList.Add(relation);
                        RelationDTO savedrelation = UIProcessManager.CreateRelation(relation);
                        if (savedrelation == null)
                        {
                            SystemMessage.ShowModalInfoMessage("There is error saving sharing relation history.", "MESSAGE");
                        }
                        else
                        {
                            isSavedV.Data.Voucher.HasEffect = true;
                            isSavedV.Data.Activity = ActivityLogManager.SetupActivity(CurrentTime.Value, actvDefinition, CNETConstantes.PMS_Pointer);


                            if (isSavedV.Data.TransactionReferencesBuffer != null && isSavedV.Data.TransactionReferencesBuffer.Count > 0)
                                isSavedV.Data.TransactionReferencesBuffer.ToList().ForEach(x => x.ReferencedActivity = null);


                            isSavedV.Data.TransactionCurrencyBuffer = null;

                            UIProcessManager.UpdateVoucherBuffer(isSavedV.Data);
                        }

                    }

                    // Charge during check-in
                    if (voucher.LastState == CNETConstantes.CHECKED_IN_STATE)
                    {
                        try
                        {
                            //Check Configuration
                            var chargeAtCheckin = LocalBuffer.LocalBuffer.ConfigurationBufferList.FirstOrDefault(c => c.Reference == CNETConstantes.PMS_Pointer.ToString() && c.Attribute == CNETConstantes.PMS_SETTING_ChargeAtCheckIn);
                            bool canCharge = false;
                            if (chargeAtCheckin != null)
                            {
                                try
                                {
                                    canCharge = Convert.ToBoolean(chargeAtCheckin.CurrentValue);
                                }
                                catch
                                {
                                    canCharge = false;
                                }
                            }

                            if (canCharge)
                            {

                                int? rateCodeHeader = null;


                                GeneratedRegistrationDTO currentDetail = Registrationlist.FirstOrDefault(x => x.registrationDetail.Date.Value.Date == CurrentTime.Value.Date);

                                if (currentDetail != null)
                                {
                                    var rateDetail = UIProcessManager.GetRateCodeDetailById(currentDetail.registrationDetail.RateCode.Value);
                                    if (rateDetail != null)
                                    {
                                        var rHeader = UIProcessManager.GetRateCodeHeaderById(rateDetail.RateCodeHeader);
                                        if (rHeader != null)
                                        {
                                            rateCodeHeader = rHeader.Id;
                                        }
                                    }
                                }
                                if (rateCodeHeader != null)
                                    CommonLogics.ChargeAtCheckin(isSavedV.Data.Voucher.Id, CurrentTime.Value, rateCodeHeader.Value, voucher.Consignee1, SelectedHotelcode, false);
                            }



                            if (LocalBuffer.LocalBuffer.EarlyCheckIn && LocalBuffer.LocalBuffer.EarlyCheckInArticle != null && CurrentTime.Value <= LocalBuffer.LocalBuffer.EarlyCheckInUntilTime)
                            {
                                if (LocalBuffer.LocalBuffer.EarlyCheckInChargeMandatory)
                                    XtraMessageBox.Show("The Customer is Early Check-In." + Environment.NewLine + "There will be an Early Room Charge !!", "CNET-ERPV6", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                if (LocalBuffer.LocalBuffer.EarlyCheckInChargeMandatory || XtraMessageBox.Show("The Customer is Early Check-In." + Environment.NewLine + "Do you want to Early Room Charge ??", "CNET-ERPV6", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK)
                                {
                                    CommonLogics.ChargeAtEarlyCheckin(isSavedV.Data.Voucher.Id, CurrentTime.Value, voucher.Consignee1.Value, SelectedHotelcode, LocalBuffer.LocalBuffer.EarlyCheckInArticle, true);
                                }
                            }
                        }
                        catch (Exception ex) { }
                    }



                    dailyRateCodeList.Clear();

                    SystemMessage.ShowModalInfoMessage("Successfully Saved", "MESSAGE");


                    #region Door Lock

                    if (IsWalkinCheckin)
                    {
                        //read device setting
                        DeviceDTO doorLockDevice = UIProcessManager.GetDeviceByhostandpreference(LocalBuffer.LocalBuffer.CurrentDevice.Id, CNETConstantes.DEVICE_DOOR_LOCK);
                        if (doorLockDevice != null)
                        {
                            RegistrationListVMDTO regVM = new RegistrationListVMDTO()
                            {
                                Id = isSavedV.Data.Voucher.Id,
                                Registration = currentVoCode,
                                Arrival = deArrival.DateTime,
                                Departure = deDeparture.DateTime,
                                RoomCode = roomCode,
                                Room = beRoom.EditValue.ToString(),
                                RoomType = Convert.ToInt32(cacRoomType.EditValue),
                                RoomTypeDescription = cacRoomType.Text,
                                Guest = cacLastName.Text,

                            };

                            frmDoorLock _frmDoorLock = new frmDoorLock();
                            _frmDoorLock.RegExt = regVM;
                            _frmDoorLock.ShowDialog();
                        }
                    }

                    #endregion


                    if (IsReplicate || IsShare)
                        DialogResult = System.Windows.Forms.DialogResult.OK;
                    cacLastName.Focus();
                    if (isSavedV != null)
                    {
                        //send welcome message
                        // SendWelcomeMessage(_currentRoomSpace, voucher.code, voucher.consignee, CurrentTime.Value);


                        if (voucher.LastState != null)
                        {
                            try
                            {
                                Progress_Reporter.Show_Progress("Generating attachment print preview", "Please Wait.......");
                                ReportGenerator reportGenerator = new ReportGenerator();
                                if (voucher.LastState == CNETConstantes.CHECKED_IN_STATE)
                                {
                                    bool isDeviceAvailable = false;

                                    try
                                    {
                                        //read device setting
                                        DeviceDTO winTabDevice = UIProcessManager.GetDeviceByhostandpreference(LocalBuffer.LocalBuffer.CurrentDevice.Id, CNETConstantes.DEVICE_SIGNATURE);
                                        if (winTabDevice != null)
                                        {
                                            isDeviceAvailable = true;
                                        }
                                    }
                                    catch (Exception ex) { }

                                    bool flag = ScribbleWinTab.TabInfo.IsTabAvailable() && isDeviceAvailable;
                                    if (flag)
                                    {
                                        MasterPageForm.PauseVideo();
                                        reportGenerator.GetPMSRegistrationCard(isSavedV.Data.Voucher.Id, true);
                                        MasterPageForm.PlayVideo();

                                    }
                                    else
                                        reportGenerator.GetPMSRegistrationCard(isSavedV.Data.Voucher.Id);


                                }
                                else if (voucher.LastState == CNETConstantes.GAURANTED_STATE)
                                {
                                    reportGenerator.GetPMSConformationAttachement(isSavedV.Data.Voucher.Id, LocalBuffer.LocalBuffer.CurrentLoggedInUserEmployeeName);
                                }
                                Progress_Reporter.Close_Progress();
                            }
                            catch (Exception ex) { }


                        }
                    }



                    //Synchronization
                    //CommonLogics.SynchronizeObject(currentVoCode, CNETSyncObjectTypes.REGISTRATION, CNETConstantes.REGISTRATION_VOUCHER, _adSyncout);

                    //CommonLogics.Synchronize(currentVoCode, CNETConstantes.REGISTRATION_VOUCHER.ToString(), CNETConstantes.VOUCHER_COMPONENET, true);

                    _isRegistrationDetailGenerated = false;
                    ResetForm();
                    CustomValidationRule.RemoveInvalidatedControls(controls);

                    // layoutControlItem38
                    //#endregion             
                    ////CNETInfoReporter.Hide();
                    return;
                }
                else
                {
                    ////CNETInfoReporter.Hide();
                    SystemMessage.ShowModalInfoMessage("Error occured during registration." + Environment.NewLine + isSavedV.Message, "MESSAGE");
                    return;
                }
            }
            catch (Exception ex)
            {
                ////CNETInfoReporter.Hide();
                SystemMessage.ShowModalInfoMessage("Error occured during registration. DETAIL::" + ex.Message, "MESSAGE");
                return;
            }

            Progress_Reporter.Close_Progress();
        }



        //Populate Negotiated Rate
        private void PopulateNegotiatedRate()
        {
            int? consignee = null;
            if (cacLastName.EditValue != null && !string.IsNullOrEmpty(cacLastName.EditValue.ToString()))
            {
                consignee = Convert.ToInt32(cacLastName.EditValue);
                negoRate = UIProcessManager.GetNegotiationRateByConsignee(consignee.Value).FirstOrDefault();

            }
            if (negoRate == null && cacCompany.EditValue != null && !string.IsNullOrEmpty(cacCompany.EditValue.ToString()))
            {
                consignee = Convert.ToInt32(cacCompany.EditValue);
                List<NegotiationRateDTO> allNegoRates = UIProcessManager.GetNegotiationRateByConsignee(consignee.Value);
                negoRate = allNegoRates.FirstOrDefault();
            }

            if (negoRate == null)
            {
                beRateCode.Enabled = true;
                //load default room types
                cacRoomType.Properties.DataSource = null;
                cacRoomType.Properties.DataSource = roomTypeList;
                cacRTC.Properties.DataSource = null;
                cacRTC.Properties.DataSource = roomTypeList;

                return;

            }
            else
            {
                //beRateCode.Enabled = false;
            }

            //load room types of the negotated rooms
            // var negotatedRateRoomTypes = UIProcessManager.GetRoomTypeByRateCode(negoRate.RateCode);
            List<RateCodeRoomTypeDTO> negotatedRateRoomTypes = UIProcessManager.GetRateCodeRoomTypeByrateCode(negoRate.RateCode);
            if (negotatedRateRoomTypes == null || negotatedRateRoomTypes.Count == 0)
            {
                DialogResult dr = XtraMessageBox.Show("The Negotiated Rate has no any room type. Do you want to cancel negotation?", "CNET_v2016", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dr == System.Windows.Forms.DialogResult.Yes)
                {
                    beRateCode.Enabled = true;
                    negoRate = null;

                    //load default room types
                    cacRoomType.Properties.DataSource = null;
                    cacRoomType.Properties.DataSource = roomTypeList;
                    cacRTC.Properties.DataSource = null;
                    cacRTC.Properties.DataSource = roomTypeList;
                    return;
                }
            }
            cacRoomType.Properties.DataSource = null;
            cacRoomType.Properties.DataSource = negotatedRateRoomTypes;
            cacRTC.Properties.DataSource = null;
            cacRTC.Properties.DataSource = negotatedRateRoomTypes;

            beRateCode.Text = "";
            teRate.Text = "";
            teTotal.Text = "";
            lcCurrency.Text = "";
            NegotiatedRateDescription = "";
            RateCodeHeaderDTO rCode = UIProcessManager.GetRateCodeHeaderById(negoRate.RateCode);
            if (rCode != null)
            {
                NegotiatedRateDescription = rCode.Description;
                beRateCode.Text = rCode.Description;
                negoRateRateHeader = rCode;
            }
            else
            {
                XtraMessageBox.Show("Unable to find the Rate Header of the customer's negotated rate", "CNET_v2016", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                beRateCode.Enabled = true;
                negoRate = null;
                //load default room types
                cacRoomType.Properties.DataSource = null;
                cacRoomType.Properties.DataSource = roomTypeList;
                cacRTC.Properties.DataSource = null;
                cacRTC.Properties.DataSource = roomTypeList;
            }

        }

        //Get Available Rate for Negotiated Rate
        private void GetAvialableNegotatedRate()
        {
            if (negoRate == null) return;
            RegistrationInfoDTO reg = new RegistrationInfoDTO();
            reg.AdultCount = Convert.ToInt32(spinEdit2.EditValue.ToString());
            reg.ChildCount = Convert.ToInt32(spinEdit1.EditValue.ToString());
            reg.ArrivalDate = deArrival.DateTime;
            reg.DepartureDate = deDeparture.DateTime;
            reg.RoomCount = Convert.ToInt32(teNoOfRooms.EditValue.ToString());
            reg.RateCode = null;
            reg.RoomType = cacRoomType.EditValue == null ? null : Convert.ToInt32(cacRoomType.EditValue);
            List<RateCodeHeaderDTO> _reCodeHeaders = null;
            List<AvailableRateDTO> avRateList = null;
            AvailableRateGeneratorDTO avRateGeneratedList = UIProcessManager.GetAvailableRates(reg, SelectedHotelcode);

            if (avRateGeneratedList != null)
            {
                avRateList = avRateGeneratedList.availableRates;
                _reCodeHeaders = avRateGeneratedList.rateCodeHeaders;
            }
            AvailableRateDTO rate = null;
            if (avRateList != null)
            {
                rate = avRateList.FirstOrDefault(r => r.RateCode == negoRateRateHeader.Id);

            }

            if (rate != null)
            {
                selectedHeader = UIProcessManager.GetRateCodeHeaderById(negoRateRateHeader.Id);

                teRate.Text = rate.FirstNightAmount.ToString();
                teTotal.Text = rate.TotalAmount.ToString();

                //Get currency
                CurrencyDTO currency = UIProcessManager.GetCurrencyById(selectedHeader.CurrencyCode);
                if (currency != null)
                {
                    CurrencyCode = currency.Id;
                    lcCurrency.Text = currency.Description;
                }

                List<RateCodePackageDTO> RatecodePackaes = UIProcessManager.GetRateCodePackageByRateCodeHeader(negoRateRateHeader.Id);
                if (RatecodePackaes != null)
                {
                    foreach (RateCodePackageDTO Rapk in RatecodePackaes)
                    {
                        PackageHeaderDTO PkHeader = UIProcessManager.GetPackageHeaderById(Rapk.Id);
                        cbePackages.Properties.Items.Add(PkHeader.Description);
                    }
                    if (RatecodePackaes.Count > 0)
                    {
                        layoutControlItem44.AppearanceItemCaption.ForeColor = Color.Blue;
                    }

                }

                if (rate.AverageAmount != rate.FirstNightAmount)
                {
                    teRate.Properties.Appearance.BackColor = Color.Orange;
                }
                else
                {
                    teRate.Properties.Appearance.BackColor = Color.White;
                }


                //Daily Rate Code Detail
                tempList.Clear();
                List<DailyRateCodeDTO> dailyRateCodes = rate.DailyRateCode.Select(s => new DailyRateCodeDTO()
                {
                    RateCodeDetail = s.RateCodeDetail,
                    StayDate = s.StayDate,
                    UnitRoomRate = s.UnitRoomRate,
                    DayWeek = s.StayDate.Value.DayOfWeek,
                    Description = selectedHeader.Description,
                }).ToList();

                if (dailyRateCodes != null)
                {
                    foreach (var dr in dailyRateCodes)
                    {
                        TempDailyRateCode rtCode = new TempDailyRateCode();
                        rtCode.DayWeek = dr.DayWeek.Value;
                        rtCode.Description = dr.Description;
                        rtCode.RateCodeDetail = dr.RateCodeDetail;
                        rtCode.StayDate = dr.StayDate.Value;
                        rtCode.UnitRoomRate = dr.UnitRoomRate;

                        tempList.Add(rtCode);

                    }

                }


            }
            else
            {
                XtraMessageBox.Show("Unable to find the available rate of the customer's negotated rate", "CNET_v2016", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                beRateCode.Enabled = true;
                negoRate = null;
                //load default room types
                cacRoomType.Properties.DataSource = null;
                cacRoomType.Properties.DataSource = roomTypeList;
                cacRTC.Properties.DataSource = null;
                cacRTC.Properties.DataSource = roomTypeList;
            }

        }

        // Validate Arrival and Departure Date
        public void validateArrivalAndDepartureDate()
        {
            if (deDeparture.IsModified || deArrival.IsModified)
            {
                List<ValidationInfo> validationInfos = new List<ValidationInfo>
            {
                new ValidationInfo(deDeparture, CompareControlOperator.LessOrEqual,
                    conditionOperator: ConditionOperator.IsNotBlank)
                {
                    Control = deArrival,
                    IsValidated=true
                },
                new ValidationInfo(deArrival, CompareControlOperator.GreaterOrEqual,
                    conditionOperator: ConditionOperator.IsNotBlank)
                {
                    Control = deDeparture,
                    IsValidated=true
                }
            };
                _invalidControls = CustomValidationRule.Validate2(validationInfos);

                if (_invalidControls.Count > 0)
                    return;
            }
        }




        #endregion


        //////////////////////////////// EVENT HANDLRES ///////////////////////////////////////////
        #region Event Handlers

        private void cacLastName_EditValueChanged(object sender, EventArgs e)
        {
            if (cacLastName.EditValue != null && !string.IsNullOrEmpty(cacLastName.EditValue.ToString()))
            {
                teTitle.Text = "";
                tePassPort.Text = "";
                beAddress.Text = "";
                string countryValue = "";


                ConsigneeDTO per = LocalBuffer.LocalBuffer.AllGuestConsigneeViewlist.FirstOrDefault(x => x.Id == Convert.ToInt32(cacLastName.EditValue));



                if (per.Title != null)
                {
                    LookupDTO Titlelk = LocalBuffer.LocalBuffer.LookUpBufferList.FirstOrDefault(x => x.Id == per.Title);
                    if (Titlelk != null)
                        teTitle.Text = Titlelk.Description;
                }
                else
                {
                    teTitle.Text = "";
                }
                if (per.BioId != null)
                {
                    tePassPort.Text = per.BioId;
                }
                else
                {
                    tePassPort.Text = "";
                }

                if (per.Note != null)
                {
                    guestNote = per.Note;
                }
                else
                {
                    statLblGuestNote.Visible = false;
                }
                if (per.Nationality != null)
                {
                    CountryDTO country = LocalBuffer.LocalBuffer.CountryBufferList.FirstOrDefault(x => x.Id == per.Nationality);
                    if (country != null)
                        teCountry.Text = country.Name;
                }
                else
                {
                    teCountry.Text = "";
                }




                //addString = address.Aggregate(addString, (current, add) => current + (LocalBuffer.LocalBuffer.PreferenceBufferList.FirstOrDefault(p => p.code == add.preference).description + " " + ":" + add.value + Environment.NewLine));
                //beAddress.Text = addString;



                //Popuate Negotated Rate
                PopulateNegotiatedRate();
            }

        }

        void tsstStatus_Click(object sender, EventArgs e)
        {
            if (note != null) note.Close();
            if (!string.IsNullOrEmpty(guestNote))
            {
                note = new frmNote();
                note.NoteContent = guestNote;
                note.ShowDialog();
            }
        }

        void tsstStatusCompany_Click(object sender, EventArgs e)
        {
            if (note != null) note.Close();
            if (!string.IsNullOrEmpty(companyNote))
            {
                note = new frmNote();
                note.NoteContent = companyNote;
                note.ShowDialog();
            }
        }



        /** Other Consignees Events **/
        #region Other Consignees Events

        void cacCompany_EditValueChanged(object sender, EventArgs e)
        {

            PopulateNegotiatedRate();

            //populate guest's note
            if (cacCompany.EditValue != null && !string.IsNullOrEmpty(cacCompany.EditValue.ToString()))
            {

                ConsigneeDTO vwConsigneeView = LocalBuffer.LocalBuffer.AllCustomerConsigneeViewlist.FirstOrDefault(x => x.Id == Convert.ToInt32(cacCompany.EditValue));

                if (vwConsigneeView != null && vwConsigneeView.Note != null)
                {
                    companyNote = vwConsigneeView.Note;
                    statLblCompanyNote.Visible = true;
                }
                else
                {
                    statLblCompanyNote.Visible = false;
                }
            }
        }

        //private void cacSourceHeader_Popup(object sender, EventArgs e)
        //{
        //    IPopupControl popupControl = sender as IPopupControl;
        //    // LayoutControl layoutControl = popupControl.PopupWindow.Controls[0] as LayoutControl;
        //    SimpleButton clearButton = popupControl.PopupWindow.Controls[0] as SimpleButton;//((LayoutControlItem)layoutControl.Items.FindByName("lciNew")).Control as SimpleButton;

        //    if (clearButton != null)
        //    {
        //        clearButton.Click -= new EventHandler(cacSource_Click);
        //        clearButton.Click += new EventHandler(cacSource_Click);
        //    }
        //}

        //void cacGroup_Popup(object sender, EventArgs e)
        //{
        //    IPopupControl popupControl = sender as IPopupControl;
        //    SimpleButton clearButton = popupControl.PopupWindow.Controls[0] as SimpleButton;

        //    if (clearButton != null)
        //    {
        //        clearButton.Click -= new EventHandler(cacGroup_Click);
        //        clearButton.Click += new EventHandler(cacGroup_Click);
        //    }
        //}

        //void cacAgent_Popup(object sender, EventArgs e)
        //{
        //    IPopupControl popupControl = sender as IPopupControl;
        //    SimpleButton clearButton = popupControl.PopupWindow.Controls[0] as SimpleButton;
        //    if (clearButton != null)
        //    {
        //        clearButton.Click -= new EventHandler(cacAgent_Click);
        //        clearButton.Click += new EventHandler(cacAgent_Click);
        //    }
        //}

        //void cacContact_Popup(object sender, EventArgs e)
        //{
        //    IPopupControl popupControl = sender as IPopupControl;
        //    SimpleButton clearButton = popupControl.PopupWindow.Controls[0] as SimpleButton;
        //    if (clearButton != null)
        //    {
        //        clearButton.Click -= new EventHandler(cacContact_Click);
        //        clearButton.Click += new EventHandler(cacContact_Click);
        //    }
        //}

        //void cacCompany_Popup(object sender, EventArgs e)
        //{
        //    IPopupControl popupControl = sender as IPopupControl;
        //    SimpleButton clearButton = popupControl.PopupWindow.Controls[0] as SimpleButton;
        //    if (clearButton != null)
        //    {
        //        clearButton.Click -= new EventHandler(cacCompany_Click);
        //        clearButton.Click += new EventHandler(cacCompany_Click);
        //    }
        //}

        //void cacCompany_Click(object sender, EventArgs e)
        //{
        //    frmOrganization organization = new frmOrganization();
        //    organization.GslType = CNETConstantes.CUSTOMER;
        //    organization.Text = "Company";
        //    organization.LoadEventArg.Args = "Company";
        //    organization.LoadEventArg.Sender = null;
        //    organization.LoadData(this, null);
        //    if (organization.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        //    {
        //        ///GetPerson();
        //        if (organization.SavedOrg != null)
        //        {
        //            cacCompany.Properties.DataSource = null;
        //            GetAllCompanies();
        //            cacCompany.EditValue = organization.SavedOrg.Id;
        //        }
        //    }

        //}
        //void cacAgent_Click(object sender, EventArgs e)
        //{
        //    frmOrganization organization = new frmOrganization();
        //    organization.GslType = CNETConstantes.AGENT;
        //    organization.Text = "Travel Agent";
        //    organization.LoadEventArg.Args = "Travel Agent";
        //    organization.LoadEventArg.Sender = null;
        //    organization.LoadData(this, null);
        //    if (organization.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        //    {
        //        ///GetPerson();
        //        if (organization.SavedOrg != null)
        //        {
        //            cacAgent.Properties.DataSource = null;
        //            GetAllAgents();
        //            cacAgent.EditValue = organization.SavedOrg.Id;
        //        }
        //    }

        //}
        //void cacSource_Click(object sender, EventArgs e)
        //{
        //    frmOrganization organization = new frmOrganization();
        //    organization.GslType = CNETConstantes.BUSINESSsOURCE;
        //    organization.Text = "Source";
        //    organization.LoadEventArg.Args = "Source";
        //    organization.LoadEventArg.Sender = null;
        //    organization.LoadData(this, null);
        //    if (organization.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        //    {
        //        ///GetPerson();
        //        if (organization.SavedOrg != null)
        //        {
        //            cacSourceHeader.Properties.DataSource = null;
        //            GetAllSources();
        //            cacSourceHeader.EditValue = organization.SavedOrg.Id;
        //        }
        //    }

        //}
        //void cacGroup_Click(object sender, EventArgs e)
        //{
        //    frmOrganization organization = new frmOrganization();
        //    organization.GslType = CNETConstantes.GROUP;
        //    organization.Text = "Group";
        //    organization.LoadEventArg.Args = "Group";
        //    organization.LoadEventArg.Sender = null;
        //    organization.LoadData(this, null);
        //    if (organization.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        //    {
        //        ///GetPerson();
        //        if (organization.SavedOrg != null)
        //        {
        //            cacGroup.Properties.DataSource = null;
        //            GetAllGroups();
        //            cacGroup.EditValue = organization.SavedOrg.Id;
        //        }
        //    }

        //}
        //void cacContact_Click(object sender, EventArgs e)
        //{
        //    frmPerson person = new frmPerson("Contact");

        //    person.Text = "Contact";
        //    person.LoadEventArg.Args = "Contact";
        //    person.GSLType = CNETConstantes.CONTACT;
        //    person.LoadEventArg.Sender = null;
        //    person.LoadData(this, null);
        //    if (person.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        //    {
        //        ///GetPerson();
        //        if (person.SavedPerson != null)
        //        {
        //            cacContact.Properties.DataSource = null;
        //            GetAllContacts();
        //            cacContact.EditValue = person.SavedPerson.Id;
        //        }
        //    }

        //}

        #endregion

        void spinEdit1_InvalidValue(object sender, InvalidValueExceptionEventArgs e)
        {
            e.ErrorText = "Please enter valid inputs!";
        }


        void cacLastName_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            SearchLookUpEdit edit = sender as SearchLookUpEdit;
            //if (edit.EditValue != "" && edit.EditValue != null)
            //{
            //    var row = (Person_View) UIProcessManager.GetPersonViewByCode(edit.EditValue.ToString());
            //    if (row != null)
            //    {
            //        e.DisplayText = row.firstName + "  " + row.middleName + "  " + row.lastName;
            //    }
            //}
        }

        void frmRateSearch_RateSelected(object sender, RateSearchCellClickedEventArgs e)
        {
            if (cacRoomType.EditValue == "" || cacRoomType.EditValue == null)
            {
                cacRoomType.EditValue = e.RoomType;
            }
            teRate.Text = e.CellValue;
            beRateCode.Text = e.RowName;
            CurrencyCode = 0;
            lcCurrency.Text = "";
            teTotal.Text = "";
            teAmount.Text = "";
            tePercent.Text = "";
            teValue.Text = "";
            selectedHeader = null;
            cbePackages.Properties.Items.Clear();
            layoutControlItem44.AppearanceItemCaption.ForeColor = Color.Black;
            if (e.RowCode != null && e.RowCode > 0)
            {
                RateCodeHeaderDTO rateCode = UIProcessManager.GetRateCodeHeaderById(e.RowCode.Value);
                if (rateCode != null)
                {
                    selectedHeader = rateCode;
                    CurrencyDTO currency = LocalBuffer.LocalBuffer.CurrencyBufferList.FirstOrDefault(c => c.Id == rateCode.CurrencyCode);
                    if (currency != null)
                    {
                        CurrencyCode = currency.Id;
                        lcCurrency.Text = currency.Description;
                    }
                    List<PackageHeaderDTO> rateCodePackages = new List<PackageHeaderDTO>();
                    List<RateCodePackageDTO> RatecodePackaes = UIProcessManager.GetRateCodePackageByRateCodeHeader(rateCode.Id);
                    if (RatecodePackaes != null)
                    {
                        foreach (RateCodePackageDTO Rapk in RatecodePackaes)
                        {
                            PackageHeaderDTO PkHeader = UIProcessManager.GetPackageHeaderById(Rapk.PackageHeader);
                            if (PkHeader != null)
                                rateCodePackages.Add(PkHeader);
                        }
                    }


                    foreach (var pk in rateCodePackages)
                    {
                        cbePackages.Properties.Items.Add(pk.Description);
                    }
                    if (rateCodePackages.Count > 0)
                    {
                        layoutControlItem44.AppearanceItemCaption.ForeColor = Color.Blue;
                    }

                }
            }
            if (RateSearchForm != null)
            {
                if (RateSearchForm.AverageRateAmt != RateSearchForm.FirstNightAmount)
                {
                    teRate.Properties.Appearance.BackColor = Color.Orange;
                }
                else
                {
                    teRate.Properties.Appearance.BackColor = Color.White;
                }
                teTotal.Text = RateSearchForm.TotalAmount.ToString();
            }
        }

        void cacRoomType_EditValueChanged(object sender, EventArgs e)
        {
            beRoom.Tag = null;
            beRoom.EditValue = "";
            beRoom.EditValue = null;
            int roomType = 0;
            RoomTypeDTO rt = new RoomTypeDTO();
            if (cacRoomType.EditValue != null && !string.IsNullOrEmpty(cacRoomType.EditValue.ToString()))
            {
                roomType = Convert.ToInt32(cacRoomType.EditValue);
                rt = UIProcessManager.GetRoomTypeById(roomType);
            }

            cacRTC.EditValue = roomType;

            if (spinEdit2.EditValue != null && spinEdit2.EditValue != @"")
            {
                if (rt != null)
                {
                    if (spinEdit2.Value > rt.MaxAdults)
                    {
                        bbiSave.Enabled = false;

                        SystemMessage.ShowModalInfoMessage("You can not assign more than  " + rt.MaxAdults + " adults to this room type.", "ERROR");
                    }
                    else
                    {
                        bbiSave.Enabled = true;


                    }
                }
            }
            if (spinEdit1.EditValue != null && spinEdit1.EditValue != "")
            {
                if (rt != null)
                {
                    if (spinEdit1.Value > rt.MaxChildren)
                    {
                        bbiSave.Enabled = false;
                        SystemMessage.ShowModalInfoMessage("You can not assign more than  " + rt.MaxChildren + " child/children to this room type.", "ERROR");
                    }
                    else if (spinEdit2.Value > rt.MaxAdults)
                    {
                        bbiSave.Enabled = false;
                    }
                    else
                    {
                        bbiSave.Enabled = true;
                    }
                }
            }
            if (!IsShare && !isRoomSelectedBeforeRoomType)
            {
                beRoom.Text = "";
                roomCode = null;
            }

            isRoomSelectedBeforeRoomType = false;

            //Get Available Negotate Rate of the selected room type
            if (negoRate != null && bbiSave.Enabled)
            {
                GetAvialableNegotatedRate();

            }

          //  AutoCompleteFreeRooms();
        }




        private void sbLastNameOpenForm_Click(object sender, EventArgs e)
        {
            //
            // Home.OpenForm(this, "MENU//PROFILE//GUEST", null);
            //
        }
        private void bbiSearch_ItemClick(object sender, ItemClickEventArgs e)
        {
            //Home.OpenForm(this, "RESERVATION SEARCH", null);
        }


        private void rirgStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            RadioGroup edit = sender as RadioGroup;
            //   edit.SelectedIndex = 0;
        }
        private void bbiDailyDetail_ItemClick(object sender, ItemClickEventArgs e)
        {
            //Home.OpenForm(this, "DAILY DETAIL_DAILY DETAIL", null);
            //frmDailyDetail frmDailyDetail = new frmDailyDetail();
            //frmDailyDetail.Show();

        }
        private void bbiTravelDetail_ItemClick(object sender, ItemClickEventArgs e)
        {
            RegistrationListVMDTO RegEx = new RegistrationListVMDTO()
            {
                Guest = cacLastName.Text,
                Arrival = deArrival.DateTime,
                Departure = deDeparture.DateTime
            };
            frmTravelDetail = new frmTravelDetail();
            frmTravelDetail.RegExtension = RegEx;
            frmTravelDetail.IsFromReservation = true;
            frmTravelDetail.Show();
        }

        private void TeNightsOnEditValueChanged(object sender, EventArgs eventArgs)
        {
            lbDayuse.Text = "";
            if (!Equals(deArrival.EditValue, null) & !string.IsNullOrEmpty(teNights.Text))
            {
                DateTime dtArrival = (DateTime)deArrival.EditValue;
                DateTime date = dtArrival.AddDays(Convert.ToDouble(teNights.Text));
                deDeparture.EditValue = date;
            }
            else if (!Equals(deDeparture.EditValue, null) & !string.IsNullOrEmpty(teNights.Text))
            {
                DateTime dtDeparture = (DateTime)deDeparture.EditValue;
                DateTime date = dtDeparture.Subtract(TimeSpan.FromDays(Convert.ToDouble(teNights.Text)));

                deArrival.EditValue = date;
            }
            if (teNights.Text == "0")
            {
                lbDayuse.Text = "Day Use";
            }
        }
        void deArrival_DateTimeChanged(object sender, EventArgs e)
        {
            //var dateEdit = sender as DateEdit;
            //if (dateEdit != null)
            //    // teETA.EditValue = dateEdit.EditValue;
            //    if (String.IsNullOrEmpty(teNights.Text))
            //    {
            //        teNights.EditValue = "1";
            //    }
        }
        private void DeArrivalOnEditValueChanged(object sender, EventArgs eventArgs)
        {
            DateTime dtArrival = (DateTime)deArrival.EditValue;
            DateTime dtDeparture = deDeparture.DateTime;
            if (dtArrival.Date >= dtDeparture.Date)
            {
                //validateArrivalAndDepartureDate();
                // deDeparture.DateTime = dtArrival.AddDays(Convert.ToDouble(teNights.Text));
                deDeparture.DateTime = dtArrival.AddDays(1);
                teNights.Text = "1";

            }
            else
            {
                teNights.Text = Convert.ToInt32((dtDeparture - dtArrival).TotalDays).ToString();
            }
            if (!IsShare)
            {
                beRoom.Text = "";
                roomCode = null;
            }
            beRateCode.Text = "";
            teRate.Text = "";
            lcCurrency.Text = "";
            teTotal.Text = "";
            _isRegistrationDetailGenerated = false;
            //AutoCompleteFreeRooms();
        }
        private void DeDepartureOnEditValueChanged(object sender, EventArgs eventArgs)
        {
            DateTime dtArrival = (DateTime)deArrival.EditValue;
            if (deDeparture.EditValue == null) return;

            DateTime dtDeparture = (DateTime)deDeparture.EditValue;

            if (dtArrival.Date > dtDeparture.Date)
            {
                deDeparture.DateTime = dtArrival.AddDays(1);
                teNights.Text = "1";
                //validateArrivalAndDepartureDate();
                // deDeparture.DateTime = dtArrival.AddDays(Convert.ToDouble(teNights.Text));


            }
            else
            {
                teNights.Text = Convert.ToInt32((dtDeparture - dtArrival).TotalDays).ToString();

                //if (!Equals(deDeparture.EditValue, "") && !string.IsNullOrEmpty(teNights.Text))
                //{
                //    DateTime date = dtDeparture.Subtract(TimeSpan.FromDays(Convert.ToDouble(teNights.Text)));
                //    deArrival.EditValue = date;
                //}
            }
            beRateCode.Text = "";
            if (!IsShare)
            {
                roomCode = null;
                beRoom.Tag = null;
                beRoom.EditValue = "";
                beRoom.EditValue = null;
            }
            teTotal.Text = "";
            teRate.Text = "";
            lcCurrency.Text = "";
            _isRegistrationDetailGenerated = false;
            //AutoCompleteFreeRooms();
        }
        private void beRoom_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            Dictionary<String, String> args = new Dictionary<string, string>();
            args.Add("Guest Name", cacLastName.Text);
            args.Add("Arrival", deArrival.Text);
            args.Add("Departure", deDeparture.Text);
            args.Add("Nights", teNights.Text);

            if (cacRoomType.EditValue != null)
                args.Add("RoomType", cacRoomType.EditValue.ToString());
            else
                args.Add("RoomType", "");

            args.Add("RoomTypeDescription", cacRoomType.Text);

            if (IsWalkinCheckin || lastState == CNETConstantes.CHECKED_IN_STATE)
            {
                args.Add("CHECK_HK", "YES");
            }
            else
            {
                args.Add("CHECK_HK", "NO");
            }


            //  Home.OpenForm(this, "ROOM SEARCH", args);
            frmRoomSearch frmRoom = new frmRoomSearch();
            frmRoom.SelectedHotelcode = SelectedHotelcode;
            frmRoom.LoadData(this, args);
            if (frmRoom.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {

            }
        }
        private void beRateCode_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            Dictionary<String, String> args = new Dictionary<string, string>
            {
                {"fromDate", deArrival.Text},
                {"toDate", deDeparture.Text},
                {"adultCount", spinEdit2.Text},
                {"childCount", spinEdit1.Text},
                {"roomCount", teNoOfRooms.Text},
                {"rateCode", beRateCode.Text},
                {"roomType", cacRTC.EditValue!=null?cacRTC.EditValue.ToString():String.Empty}
            };

            //frmRateSearch RateSearchForm = (frmRateSearch)Home.OpenForm(this, "RATE SEARCH", args);
            // this.RateSearchForm = RateSearchForm;
            //RateSearchForm = new frmRateSearch();

            frmRateSearch RateSearchForm = new frmRateSearch();
            RateSearchForm.SelectedHotelcode = SelectedHotelcode;
            RateSearchForm.LoadData(this, args);
            this.RateSearchForm = RateSearchForm;
            RateSearchForm.FormClosing += RateSearchForm_FormClosing;
            RateSearchForm.RateSelected += frmRateSearch_RateSelected;
            RateSearchForm.ShowDialog();


        }
        void RateSearchForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (RateSearchForm.frmRateDetailInfo != null)
            {

                dailyRateCodeList = RateSearchForm.frmRateDetailInfo.dailyRateCodeList;
                tempList.Clear();
                foreach (var dr in dailyRateCodeList)
                {
                    TempDailyRateCode rtCode = new TempDailyRateCode();
                    rtCode.DayWeek = dr.DayWeek.Value;
                    rtCode.Description = dr.Description;
                    rtCode.RateCodeDetail = dr.RateCodeDetail;
                    rtCode.StayDate = dr.StayDate.Value;
                    rtCode.UnitRoomRate = dr.UnitRoomRate;
                    tempList.Add(rtCode);
                }
            }

        }
        private void bbiComments_ItemClick(object sender, ItemClickEventArgs e)
        {
            ShowComment();
        }


        private void bbiRegistrationDetail_ItemClick(object sender, ItemClickEventArgs e)
        {

            DateTime? currentTime = UIProcessManager.GetServiceTime();
            if (currentTime == null) return;

            PopulateMoreFields(voucher.Id, currentVoCode, currentTime.Value);
            _isRegistrationDetailGenerated = false;
            if (Registrationlist != null)
            {
                frmReservationDetail moreFields = new frmReservationDetail();
                moreFields.SelectedHotelcode = SelectedHotelcode;
                moreFields.RegDetailList = Registrationlist.Select(x => x.registrationDetail).ToList();
                moreFields.Text += @" - " + currentVoCode;
                moreFields.RegVoucher = voucher.Id;// currentVoCode;
                // moreFields.PckToPOst = _packagesToPosts.GroupBy(g => g.registrationDetail + g.amount + g.packageHeader).Select(x => x.FirstOrDefault()).ToList(); ;

                moreFields._pkToPostList = Registrationlist.FirstOrDefault().packagesToPost;

                if (moreFields.ShowDialog(this) == DialogResult.OK)
                {
                    // _registrationDetails = moreFields.RegDetail;
                    //_packagesToPosts = moreFields.PckToPOst.Distinct().ToList();
                }
            }
        }

        private void teNights_TextChanged(object sender, EventArgs e)
        {
            _isRegistrationDetailGenerated = false;
        }

        private void beRateCode_EditValueChanged(object sender, EventArgs e)
        {
            if (beRateCode.Text == "")
            {
                rpgReservationDetail.Enabled = false;
                cbePackages.Properties.Items.Clear();
                layoutControlItem44.AppearanceItemCaption.ForeColor = Color.Black;
                //bbiMoreFields.Enabled = false;
            }
            else
            {
                rpgReservationDetail.Enabled = true;
                //  bbiMoreFields.Enabled = true;
            }
        }

        private void frmReservation_Load(object sender, EventArgs e)
        {

            bool flag = InitializeData();
            if (!flag)
            {
                this.Close();
            }
        }

        private void bbiSave_ItemClick(object sender, ItemClickEventArgs e)
        {
            OnSave();
        }

        private void beAddGuest_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                // Home.OpenForm(this, "MENU//PROFILE//GUEST", this);
                this.SubForm = this;
                frmPerson frmperson = new frmPerson("Guest");
                frmperson.GSLType = CNETConstantes.GUEST;
                frmperson.Text = "Guest";
                frmperson.rpgScanFingerPrint.Visible = true;
                frmperson.LoadEventArg.Args = "Guest";
                frmperson.LoadData(this, this);
                frmperson.LoadEventArg.Sender = null;
                if (frmperson.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    ///GetPerson();
                    if (frmperson.SavedPerson != null)
                    {

                        LocalBuffer.LocalBuffer.AllGuestConsigneeViewlist.Add(frmperson.SavedPerson);


                        cacLastName.Properties.DataSource = null;
                        cacLastName.Properties.DataSource = LocalBuffer.LocalBuffer.AllGuestConsigneeViewlist;
                        cacLastName.EditValue = frmperson.SavedPerson.Id;
                    }
                }

            }
            catch (Exception ex)
            {

            }
        }

        private void cacPayment_EditValueChanged(object sender, EventArgs e)
        {
            int pay = Convert.ToInt32(cacPayment.EditValue);
            if (pay == CNETConstantes.PAYMENTMETHODSCASH || pay == CNETConstantes.PAYMENTMETHODS_DIRECT_BILL)
            {

                layoutControlItem23.Visibility = LayoutVisibility.Never;
                layoutControlItem36.Visibility = LayoutVisibility.Never;
                layoutControlItem9.Visibility = LayoutVisibility.Never;
                layoutControlItem41.Visibility = LayoutVisibility.Never;
                layoutControlItem42.Visibility = LayoutVisibility.Never;
                layoutControlItem28.Visibility = LayoutVisibility.Never;
                layoutControlItem45.Visibility = LayoutVisibility.Never;



            }
            else
            {


                layoutControlItem23.Visibility = LayoutVisibility.Always;
                layoutControlItem36.Visibility = LayoutVisibility.Always;
                layoutControlItem9.Visibility = LayoutVisibility.Always;
                layoutControlItem41.Visibility = LayoutVisibility.Always;
                layoutControlItem42.Visibility = LayoutVisibility.Always;
                layoutControlItem28.Visibility = LayoutVisibility.Always;
                layoutControlItem45.Visibility = LayoutVisibility.Always;
            }
        }

        private void bbiNew_ItemClick(object sender, ItemClickEventArgs e)
        {
            ResetForm();
        }

        private void bbiClose_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.Close();
        }

        private void spinEdit2_EditValueChanged(object sender, EventArgs e)
        {

            beRateCode.Text = "";
            teRate.Text = "";
            lcCurrency.Text = "";
            teTotal.Text = "";

            int roomType;
            RoomTypeDTO rt = new RoomTypeDTO();
            if (cacRoomType.EditValue != null && !string.IsNullOrEmpty(cacRoomType.EditValue.ToString()))
            {
                roomType = Convert.ToInt32(cacRoomType.EditValue);
                rt = UIProcessManager.GetRoomTypeById(roomType);
                if (spinEdit2.EditValue != null && spinEdit2.EditValue != "")
                {
                    if (rt != null)
                    {
                        if (spinEdit2.Value > rt.MaxAdults)
                        {
                            bbiSave.Enabled = false;
                            SystemMessage.ShowModalInfoMessage("You can not assign more than  " + rt.MaxAdults + " adults to this room type.", "ERROR");


                        }
                        else
                        {
                            bbiSave.Enabled = true;

                            //get available nego rate
                            GetAvialableNegotatedRate();

                        }
                    }
                }

            }
        }

        private void spinEdit1_EditValueChanged(object sender, EventArgs e)
        {
            beRateCode.Text = "";
            teRate.Text = "";
            lcCurrency.Text = "";
            teTotal.Text = "";
        }

        private void beRoom_EditValueChanged(object sender, EventArgs e)
        {
            if (beRoom.Tag != null)
            {
                RoomDetailDTO rd = UIProcessManager.GetRoomDetailById((int)beRoom.Tag);
                if (rd != null)
                {
                    if (cacRoomType.EditValue != null && !string.IsNullOrEmpty(cacRoomType.EditValue.ToString()) && rd.RoomType == Convert.ToInt32(cacRoomType.EditValue))
                    {
                        //_currentRoomSpace = rd.space;
                        SelectedRoomHandler.Invoke(rd);
                    }
                    else
                        SelectedRoomHandler.Invoke(rd);

                }
            }
        }


        private void spinEdit2_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void spinEdit1_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {

            int roomType;
            RoomTypeDTO rt = new RoomTypeDTO();
            if (cacRoomType.EditValue != null && !string.IsNullOrEmpty(cacRoomType.EditValue.ToString()))
            {
                roomType = Convert.ToInt32(cacRoomType.EditValue);
                rt = UIProcessManager.GetRoomTypeById(roomType);
                if (spinEdit1.EditValue != null && spinEdit1.EditValue != "")
                {
                    if (rt != null)
                    {
                        if (spinEdit1.Value > rt.MaxChildren)
                        {
                            bbiSave.Enabled = false;
                            SystemMessage.ShowModalInfoMessage("You can not assign more than  " + rt.MaxChildren + " child/children to this room type.", "ERROR");
                            e.Cancel = true;
                        }
                        else
                        {
                            bbiSave.Enabled = true;
                        }
                    }
                }

            }
        }

        /** Key Down Events **/
        #region Key Down Events 

        private void cacRoomType_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            LookUpEdit edit = sender as LookUpEdit;
            if (e.KeyData == Keys.Back || e.KeyData == Keys.Delete)
            {
                // edit.ClosePopup();
                edit.EditValue = "";

            }
            else
            {
                //edit.ShowPopup();
                edit.Properties.ImmediatePopup = true;
            }
            e.Handled = true;
        }

        private void cacCompany_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            LookUpEdit edit = sender as LookUpEdit;
            if (e.KeyData == Keys.Back || e.KeyData == Keys.Delete)
            {
                edit.ClosePopup();
                edit.EditValue = "";

            }
            e.Handled = true;
        }

        private void cacContact_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            LookUpEdit edit = sender as LookUpEdit;
            if (e.KeyData == Keys.Back || e.KeyData == Keys.Delete)
            {
                edit.ClosePopup();
                edit.EditValue = "";

            }
            e.Handled = true;
        }

        private void cacSourceHeader_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            LookUpEdit edit = sender as LookUpEdit;
            if (e.KeyData == Keys.Back || e.KeyData == Keys.Delete)
            {
                edit.ClosePopup();
                edit.EditValue = "";

            }
            e.Handled = true;
        }

        private void cacAgent_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            LookUpEdit edit = sender as LookUpEdit;
            if (e.KeyData == Keys.Back || e.KeyData == Keys.Delete)
            {
                edit.ClosePopup();
                edit.EditValue = "";

            }
            e.Handled = true;
        }

        private void cacGroup_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            LookUpEdit edit = sender as LookUpEdit;
            if (e.KeyData == Keys.Back || e.KeyData == Keys.Delete)
            {
                edit.ClosePopup();
                edit.EditValue = "";

            }
            e.Handled = true;
        }

        private void cacResType_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            LookUpEdit edit = sender as LookUpEdit;
            if (e.KeyData == Keys.Back || e.KeyData == Keys.Delete)
            {
                edit.ClosePopup();
                edit.EditValue = "";

            }
            e.Handled = true;
        }

        private void cacMarket_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            LookUpEdit edit = sender as LookUpEdit;
            if (e.KeyData == Keys.Back || e.KeyData == Keys.Delete)
            {
                edit.ClosePopup();
                edit.EditValue = "";

            }
            e.Handled = true;
        }

        private void cacSourceFooter_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            LookUpEdit edit = sender as LookUpEdit;
            if (e.KeyData == Keys.Back || e.KeyData == Keys.Delete)
            {
                edit.ClosePopup();
                edit.EditValue = "";

            }
            e.Handled = true;
        }

        private void cacOrigion_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            LookUpEdit edit = sender as LookUpEdit;
            if (e.KeyData == Keys.Back || e.KeyData == Keys.Delete)
            {
                edit.ClosePopup();
                edit.EditValue = "";

            }
            e.Handled = true;
        }

        private void cacPayment_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            LookUpEdit edit = sender as LookUpEdit;
            if (e.KeyData == Keys.Back || e.KeyData == Keys.Delete)
            {
                edit.ClosePopup();
                edit.EditValue = "";

            }
            e.Handled = true;
        }

        private void cacSpecials_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            LookUpEdit edit = sender as LookUpEdit;
            if (e.KeyData == Keys.Back || e.KeyData == Keys.Delete)
            {
                edit.ClosePopup();
                edit.EditValue = "";

            }
            e.Handled = true;
        }

        #endregion

        private void cacRTC_EditValueChanged(object sender, EventArgs e)
        {
            beRateCode.Text = "";
            teRate.Text = "";
            teTotal.Text = "";
            lcCurrency.Text = "";
            if (!string.IsNullOrEmpty(NegotiatedRateDescription))
            {
                beRateCode.Text = NegotiatedRateDescription;
            }
        }

        private void beRoom_InvalidValue(object sender, InvalidValueExceptionEventArgs e)
        {
            e.ErrorText = "Invalid value. Either you entered a room which does not exist or the no. of rooms is greater than 1.";
        }

        private void beRoom_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!string.IsNullOrEmpty(beRoom.Text))
            {


                //List<RoomDetail> roomList = UIProcessManager.GetUnassignedRoomsByState(deArrival.DateTime,
                //    deDeparture.DateTime,
                //    CNETConstantes.CHECKED_OUT_STATE).Where(r=>r.isActive != null && r.IsActive).ToList();
                //RoomDetail rd = new RoomDetail();
                //if (cacRoomType.EditValue != "" && cacRoomType.EditValue != null)
                //{
                //    rd =
                //        roomList.FirstOrDefault(
                //            r => r.description == beRoom.Text && r.roomType == cacRoomType.EditValue.ToString());
                //}
                //else
                //{
                //    rd =
                //        roomList.FirstOrDefault(
                //            r => r.description == beRoom.Text);
                //}
                //if (rd != null)
                //{
                //    SelectedRoomHandler.Invoke(rd);
                //}
                //else
                //{
                //    roomCode = "";
                //    e.Cancel = true;
                //    _isRegistrationDetailGenerated = false;
                //}
                if (!string.IsNullOrEmpty(teNoOfRooms.Text))
                {
                    if (Convert.ToDecimal(teNoOfRooms.Text) > 1)
                    {
                        e.Cancel = true;
                        beRoom.Enabled = false;
                        SystemMessage.ShowModalInfoMessage("You can not select room if the number of rooms is greater than 1.", "ERROR");
                        beRoom.Text = "";
                        roomCode = null;
                    }
                    else
                    {
                        beRoom.Enabled = true;
                    }
                }
            }
            else
            {
                roomCode = null;
                _isRegistrationDetailGenerated = false;
            }

        }

        private void teNoOfRooms_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!string.IsNullOrEmpty(beRoom.Text))
            {
                if (!string.IsNullOrEmpty(teNoOfRooms.Text))
                {
                    if (Convert.ToDecimal(teNoOfRooms.Text) > 1)
                    {
                        e.Cancel = true;
                        beRoom.Enabled = false;
                        SystemMessage.ShowModalInfoMessage("You can not select room if the number of rooms is greater than 1.", "ERROR");
                        if (!IsShare)
                        {
                            beRoom.Text = "";
                            roomCode = null;
                        }
                    }
                    else
                    {
                        if (!IsShare)
                        {
                            beRoom.Enabled = true;
                        }
                    }

                }
            }
        }

        private void teNoOfRooms_InvalidValue(object sender, InvalidValueExceptionEventArgs e)
        {
            e.ErrorText = "You can not select a room if the number of rooms is greater than 1. Please delete the selected room.";
        }

        private void beRateCode_InvalidValue(object sender, InvalidValueExceptionEventArgs e)
        {
            e.ErrorText = "Please select an available rate.";
            AutoValidate = AutoValidate.EnableAllowFocusChange;
        }

        private void cacRoomType_InvalidValue(object sender, InvalidValueExceptionEventArgs e)
        {
            beRoom.Tag = null;
            beRoom.EditValue = null;
            e.ErrorText = "Please select room type.";
            AutoValidate = AutoValidate.EnableAllowFocusChange;
        }

        private void cacRTC_InvalidValue(object sender, InvalidValueExceptionEventArgs e)
        {
            e.ErrorText = "Please select RTC room type.";
            AutoValidate = AutoValidate.EnableAllowFocusChange;
        }

        private void cacLastName_InvalidValue(object sender, InvalidValueExceptionEventArgs e)
        {
            e.ErrorText = "Please select guest.";
            AutoValidate = AutoValidate.EnableAllowFocusChange;
        }

        private void teDiscountAmt_EditValueChanged(object sender, EventArgs e)
        {
            if (teAmount.Text != "")
            {
                if (!IsReplicate)
                {
                    if (dailyRateCodeList.Count == 0)
                    {
                        teAmount.Text = "";
                        SystemMessage.ShowModalInfoMessage("Please select available rate.", "ERROR");

                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(teAmount.Text))
                        {
                            decimal rateAmount = dailyRateCodeList.Average(r => r.UnitRoomRate);
                            decimal factor = !string.IsNullOrEmpty(teAmount.Text) ? Convert.ToDecimal(teAmount.Text) : 0;
                            decimal result = Math.Round((rateAmount - factor), 2);
                            teValue.Text = Math.Round(factor, 2).ToString();
                            teTotal.Text = result.ToString();
                            if (result < 0)
                            {
                                SystemMessage.ShowModalInfoMessage("Over rate adjustment!", "ERROR");
                                bbiSave.Enabled = false;
                                bbiCheckIn.Enabled = false;
                            }
                            else
                            {
                                bbiSave.Enabled = true;
                                bbiCheckIn.Enabled = true;
                            }
                        }
                    }
                }
            }

        }

        private void rgFactor_EditValueChanged(object sender, EventArgs e)
        {


            if (rgFactor.EditValue.ToString() == "value")
            {
                tePercent.Enabled = false;
                teAmount.Enabled = true;
                if (!IsReplicate)
                {
                    tePercent.Text = "";
                    teValue.Text = "";
                }

            }
            else if (rgFactor.EditValue.ToString() == "percent")
            {
                teAmount.Enabled = false;
                if (!IsReplicate)
                {
                    teAmount.Text = "";
                    teValue.Text = "";
                }
                tePercent.Enabled = true;

            }


        }

        private void teNoOfRooms_EditValueChanged(object sender, EventArgs e)
        {

            if (!string.IsNullOrEmpty(teNoOfRooms.Text))
            {
                if (Convert.ToDecimal(teNoOfRooms.Text) > 1)
                {
                    beRoom.Enabled = false;
                    if (!IsShare)
                    {
                        beRoom.Text = "";
                        roomCode = null;
                    }
                }
                else
                {
                    if (!IsShare)
                    {
                        beRoom.Enabled = true;
                    }
                }

            }

        }

        private void tePercent_EditValueChanged(object sender, EventArgs e)
        {
            if (tePercent.Text != "")
            {
                if (!IsReplicate)
                {
                    if (dailyRateCodeList.Count == 0)
                    {
                        SystemMessage.ShowModalInfoMessage("Please select available rate.", "ERROR");
                        tePercent.Text = "";
                    }
                    else
                    {
                        decimal rateAmount = dailyRateCodeList.Average(r => r.UnitRoomRate);
                        decimal factor = !string.IsNullOrEmpty(tePercent.Text) ? (Convert.ToDecimal(tePercent.Text) * rateAmount) / 100 : 0;
                        decimal result = Math.Round(rateAmount - factor, 2);
                        teValue.Text = Math.Round(factor, 2).ToString();
                        teTotal.Text = Math.Round(result, 2).ToString();
                    }
                }
            }

        }

        private void ceFixedRate_CheckedChanged(object sender, EventArgs e)
        {

            _isRegistrationDetailGenerated = false;
            if (ceFixedRate.Checked)
            {
                if (RateSearchForm != null)
                {
                    teRate.Properties.Appearance.BackColor = Color.White;
                    teRate.Text = RateSearchForm.FirstNightAmount.ToString();
                }
            }
            else
            {
                if (RateSearchForm != null)
                {
                    if (RateSearchForm.AverageRateAmt != RateSearchForm.FirstNightAmount)
                    {
                        teRate.Properties.Appearance.BackColor = Color.Orange;
                        teRate.Text = RateSearchForm.AverageRateAmt.ToString();
                    }
                }
            }
            // dailyRateCodeList = ceFixedRate.Checked ? GetDailyRateCode(tempDailyRateCodeList) : tempDailyRateCodeList;
        }

        private void beiStatus_EditValueChanged(object sender, EventArgs e)
        {
            lastState = Convert.ToInt32(beiStatus.EditValue);
            DateTime? CurrentTime = UIProcessManager.GetServiceTime();
            if (CurrentTime == null)
            {
                return;
            }
            if (Convert.ToInt32(beiStatus.EditValue) == CNETConstantes.CHECKED_IN_STATE)
            {
                string suffix = IsReplicate ? "(Replicate)" : "";
                this.Text = "CHECK-IN " + suffix + " -" + currentVoCode;
                deArrival.Enabled = false;
                deArrival.DateTime = CurrentTime.Value;
                deArrival.Properties.ReadOnly = true;
                bbiSave.Visibility = BarItemVisibility.Never;
                bbiCheckIn.Visibility = BarItemVisibility.Always;


            }
            else
            {

                string suffix = IsReplicate ? "(Replicate)" : "";
                this.Text = "RESERVATION " + suffix + " -" + currentVoCode;
                deArrival.Enabled = true;
                deArrival.Properties.ReadOnly = false;
                bbiSave.Visibility = BarItemVisibility.Always;
                bbiCheckIn.Visibility = BarItemVisibility.Never;
            }
        }

        #endregion

        private void teAmount_Click(object sender, EventArgs e)
        {
            teAmount.EditValue = null;
        }

        private void tePercent_Click(object sender, EventArgs e)
        {
            tePercent.EditValue = null;
        }

        private void tsstStatus_DoubleClick_1(object sender, EventArgs e)
        {

        }

        private void beiHotel_EditValueChanged(object sender, EventArgs e)
        {

            SelectedHotelcode = beiHotel.EditValue == null ? 0 : Convert.ToInt32(beiHotel.EditValue);

            if (SelectedHotelcode != null)
            {
                //Room Type List
                roomTypeList = UIProcessManager.GetRoomTypeByConsigneeUnit(SelectedHotelcode).Where(r => r.IsActive && (r.ActivationDate != null && r.ActivationDate.Value.Date <= CurrentTime.Value.Date)).ToList();
                cacRoomType.Properties.DataSource = (roomTypeList);
                cacRTC.Properties.DataSource = roomTypeList;

            }
            else
            {
                XtraMessageBox.Show("Please Selecte Hotel Branch !!");
                cacRoomType.Properties.DataSource = null;
                cacRTC.Properties.DataSource = null;
            }
        }

        private void cacCompany_AddNewValue(object sender, AddNewValueEventArgs e)
        {
            frmOrganization organization = new frmOrganization();
            organization.GslType = CNETConstantes.CUSTOMER;
            organization.Text = "Company";
            organization.LoadEventArg.Args = "Company";
            organization.LoadEventArg.Sender = null;
            organization.LoadData(this, null);
            if (organization.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ///GetPerson();
                if (organization.SavedOrg != null)
                {
                    cacCompany.Properties.DataSource = null;
                    GetAllCompanies();
                    e.Cancel = false;
                    e.NewValue = organization.SavedOrg.Id;
                }
            }
        }

        private void cacContact_AddNewValue(object sender, AddNewValueEventArgs e)
        {
            frmPerson person = new frmPerson("Contact");

            person.Text = "Contact";
            person.LoadEventArg.Args = "Contact";
            person.GSLType = CNETConstantes.CONTACT;
            person.LoadEventArg.Sender = null;
            person.LoadData(this, null);
            if (person.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ///GetPerson();
                if (person.SavedPerson != null)
                {
                    cacContact.Properties.DataSource = null;
                    GetAllContacts();
                    e.Cancel = false;
                    e.NewValue = person.SavedPerson.Id;
                }
            }
        }

        private void cacSourceHeader_AddNewValue(object sender, AddNewValueEventArgs e)
        {
            frmOrganization organization = new frmOrganization();
            organization.GslType = CNETConstantes.BUSINESSsOURCE;
            organization.Text = "Source";
            organization.LoadEventArg.Args = "Source";
            organization.LoadEventArg.Sender = null;
            organization.LoadData(this, null);
            if (organization.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ///GetPerson();
                if (organization.SavedOrg != null)
                {
                    cacSourceHeader.Properties.DataSource = null;
                    GetAllSources();
                    e.Cancel = false;
                    e.NewValue = organization.SavedOrg.Id;
                }
            }
        }

        private void cacAgent_AddNewValue(object sender, AddNewValueEventArgs e)
        {
            frmOrganization organization = new frmOrganization();
            organization.GslType = CNETConstantes.AGENT;
            organization.Text = "Travel Agent";
            organization.LoadEventArg.Args = "Travel Agent";
            organization.LoadEventArg.Sender = null;
            organization.LoadData(this, null);
            if (organization.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ///GetPerson();
                if (organization.SavedOrg != null)
                {
                    cacAgent.Properties.DataSource = null;
                    GetAllAgents();
                    e.Cancel = false;
                    e.NewValue = organization.SavedOrg.Id;
                }
            }

        }

        private void cacGroup_AddNewValue(object sender, AddNewValueEventArgs e)
        {
            frmOrganization organization = new frmOrganization();
            organization.GslType = CNETConstantes.GROUP;
            organization.Text = "Group";
            organization.LoadEventArg.Args = "Group";
            organization.LoadEventArg.Sender = null;
            organization.LoadData(this, null);
            if (organization.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ///GetPerson();
                if (organization.SavedOrg != null)
                {
                    cacGroup.Properties.DataSource = null;
                    GetAllGroups();
                    e.Cancel = false;
                    e.NewValue = organization.SavedOrg.Id;
                }
            }

        }
    }



    public delegate void SelectedGuest(ConsigneeDTO person);
    public delegate void SelectedRoom(RoomDetailDTO room);
    public delegate void SelectedRoomList(List<RoomDetailDTO> room);

}
