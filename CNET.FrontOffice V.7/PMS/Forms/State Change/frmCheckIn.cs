using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using CNET.FrontOffice_V._7;
using CNET.ERP.Client.Common.UI;
using DevExpress.XtraGrid.Columns;
using DevExpress.Utils.Win;
using DevExpress.XtraEditors.Controls;
using CNET.FrontOffice_V._7.Forms.Non_Navigatable_Modals;
using ProcessManager;
using CNET_V7_Domain.Domain.TransactionSchema;
using CNET.FrontOffice_V._7.PMS.Common_Classes;
using CNET_V7_Domain.Domain.SettingSchema;
using CNET_V7_Domain.Domain.PmsSchema;
using CNET_V7_Domain.Domain.ViewSchema;
using CNET.FrontOffice_V._7.Validation;
using CNET_V7_Domain.Misc.PmsDTO;

namespace CNET.FrontOffice_V._7.Forms.State_Change
{
    public partial class frmCheckIn : UILogicBase
    {

        private VwConsigneeViewDTO _company = null;
        private VwConsigneeViewDTO _agent = null;
        private VwConsigneeViewDTO _source = null;
        private VwConsigneeViewDTO _group = null;
        private VwConsigneeViewDTO _contact = null;

        private List<VoucherConsigneeListDTO> _accompanyGuestList = null;

        private int ad;

        private RoomDetailDTO _selectedRoomDetail = null;
        private List<SystemConstantDTO> paymentList = null;

        //properties
        private const int CP_NOCLOSE_BUTTON = 0x200;
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams myCp = base.CreateParams;
                myCp.ClassStyle = myCp.ClassStyle | CP_NOCLOSE_BUTTON;
                return myCp;
            }
        }

        public bool IsFromNightAudit { get; set; }

        public SelectedRoom SelectedRoomHandler { get; set; }

        RegistrationListVMDTO regExtension;
        internal RegistrationListVMDTO RegExtension
        {
            get { return regExtension; }
            set
            {
                regExtension = value;

            }
        }

        /********************** CONSTRUCTOR *********************/
        public frmCheckIn()
        {
            InitializeComponent();
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            lcStatus.Text = "You are checking in the guest below";

            InitializeUI();

            SelectedRoomHandler = SelectedRoomEventHandler;


        }


        #region Helper Methods

        private void InitializeUI()
        {

            //Payment Type

            le_PaymentType.Properties.Columns.Add(new LookUpColumnInfo("Description", "Payment Methods"));
            le_PaymentType.Properties.DisplayMember = "Description";
            le_PaymentType.Properties.ValueMember = "Id";


            // Room Type
            leRoomType.Properties.Columns.Add(new LookUpColumnInfo("Description", "Room Types"));
            leRoomType.Properties.DisplayMember = "Description";
            leRoomType.Properties.ValueMember = "Id";


            //Guest 
            GridColumn column = cac_guest.Properties.View.Columns.AddField("Id");
            column.Visible = false;
            column = cac_guest.Properties.View.Columns.AddField("Code");
            column.Visible = true;
            column = cac_guest.Properties.View.Columns.AddField("FirstName");
            column.Caption = "First Name";
            column = cac_guest.Properties.View.Columns.AddField("SecondName");
            column.Caption = "Middle Name";
            column.Visible = true;
            column = cac_guest.Properties.View.Columns.AddField("BioId");
            column.Caption = "Id Number";
            column.Visible = true;
            cac_guest.Properties.DisplayMember = "FirstName";
            cac_guest.Properties.ValueMember = "Id";

            //Contact
            GridColumn columnContact = cac_contact.Properties.View.Columns.AddField("Id");
            columnContact.Visible = false;
            columnContact = cac_contact.Properties.View.Columns.AddField("Code");
            columnContact.Visible = true;
            columnContact = cac_contact.Properties.View.Columns.AddField("FirstName");
            columnContact.Caption = "First Name";
            columnContact.Visible = true;
            columnContact = cac_contact.Properties.View.Columns.AddField("SecondName");
            columnContact.Caption = "Middle Name";
            columnContact.Visible = true;
            columnContact = cac_contact.Properties.View.Columns.AddField("BioId");
            columnContact.Caption = "Id Number";
            columnContact.Visible = true;
            cac_contact.Properties.DisplayMember = "FirstName";
            cac_contact.Properties.ValueMember = "Id";

            //Company
            GridColumn columnCompany = cac_company.Properties.View.Columns.AddField("Id");
            columnCompany.Visible = false;
            columnCompany = cac_company.Properties.View.Columns.AddField("Code");
            columnCompany.Visible = true;
            columnCompany = cac_company.Properties.View.Columns.AddField("FirstName");
            columnCompany.Caption = "Trade name";
            columnCompany.Visible = true;
            columnCompany = cac_company.Properties.View.Columns.AddField("Tin");
            columnCompany.Visible = true;
            cac_company.Properties.DisplayMember = "FirstName";
            cac_company.Properties.ValueMember = "Id";

            //Group
            GridColumn columnGroup = cac_group.Properties.View.Columns.AddField("Id");
            columnGroup.Visible = false;
            columnGroup = cac_group.Properties.View.Columns.AddField("Code");
            columnGroup.Visible = true;
            columnGroup = cac_group.Properties.View.Columns.AddField("FirstName");
            columnGroup.Caption = "First Name";
            columnGroup.Visible = true;
            columnGroup = cac_group.Properties.View.Columns.AddField("SecondName");
            columnGroup.Caption = "Middle Name";
            columnGroup.Visible = true;
            columnGroup = cac_group.Properties.View.Columns.AddField("Tin");
            columnGroup.Visible = true;
            cac_group.Properties.DisplayMember = "FirstName";
            cac_group.Properties.ValueMember = "Id";

            //Agent
            GridColumn columnAgent = cac_agent.Properties.View.Columns.AddField("Id");
            columnAgent.Visible = false;
            columnAgent = cac_agent.Properties.View.Columns.AddField("Code");
            columnAgent.Visible = true;
            columnAgent = cac_agent.Properties.View.Columns.AddField("FirstName");
            columnAgent.Caption = "First Name";
            columnAgent.Visible = true;
            columnAgent = cac_agent.Properties.View.Columns.AddField("SecondName");
            columnAgent.Caption = "Middle Name";
            columnAgent.Visible = true;
            columnAgent = cac_agent.Properties.View.Columns.AddField("Tin");
            columnAgent.Visible = true;
            cac_agent.Properties.DisplayMember = "FirstName";
            cac_agent.Properties.ValueMember = "Id";

            //Source
            GridColumn columnSource = cac_source.Properties.View.Columns.AddField("Id");
            columnSource.Visible = false;
            columnSource = cac_source.Properties.View.Columns.AddField("Code");
            columnSource.Visible = true;
            columnSource = cac_source.Properties.View.Columns.AddField("FirstName");
            columnSource.Caption = "First Name";
            columnSource.Visible = true;
            columnSource = cac_source.Properties.View.Columns.AddField("SecondName");
            columnSource.Caption = "Middle Name";
            columnSource.Visible = true;
            columnSource = cac_source.Properties.View.Columns.AddField("Tin");
            columnSource.Visible = true;
            cac_source.Properties.DisplayMember = "FirstName";
            cac_source.Properties.ValueMember = "Id";


        }


        private bool InitializeData()
        {
            try
            {
                if (RegExtension == null)
                {
                    SystemMessage.ShowModalInfoMessage("Please select a registration", "ERROR");
                    return false;
                }

                DateTime? currentTime = UIProcessManager.GetServiceTime();
                if (currentTime == null) return false;


                // Progress_Reporter.Show_Progress("Loading data. Please wait...");

                //Guaranted
                ActivityDefinitionDTO workFlowGuaranteed = UIProcessManager.GetActivityDefinitionBydescriptionandreference(CNETConstantes.LU_ACTIVITY_DEFINATION_CheckIN, CNETConstantes.REGISTRATION_VOUCHER).FirstOrDefault();

                if (workFlowGuaranteed != null)
                {

                    ad = workFlowGuaranteed.Id;
                }
                else
                {
                    ////CNETInfoReporter.Hide();
                    SystemMessage.ShowModalInfoMessage("Please define workflow of CHECK IN for Registration Voucher ", "ERROR");
                    return false;
                }

                //Check Activity Previlage
                var userRoleMapper = LocalBuffer.LocalBuffer.UserRoleMapperBufferList.FirstOrDefault(r => r.User == LocalBuffer.LocalBuffer.CurrentLoggedInUser.Id);
                if (userRoleMapper != null)
                {
                    var roleActivity = UIProcessManager.GetRoleActivityByactivityDefinition(ad).FirstOrDefault(r => r.Role == userRoleMapper.Role && r.NeedsPassCode);
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


                //room type
                List<RoomTypeDTO> roomType = UIProcessManager.GetRoomTypeByConsigneeUnit(SelectedHotelcode).Where(r => r.IsActive && (r.ActivationDate != null && r.ActivationDate.Value.Date <= currentTime.Value.Date)).ToList();
                leRoomType.Properties.DataSource = (roomType);

                //Payment Methods
                paymentList = LocalBuffer.LocalBuffer.SystemConstantDTOBufferList.Where(l => l.Category == CNETConstantes.PAYMENT_METHODS && l.IsActive).ToList();
                paymentList =
                paymentList.Where(
                    r =>
                        r.Id == CNETConstantes.PAYMENTMETHODSCASH ||
                        //r.code == CNETConstantes.PAYMENTMETHODS_DIRECT_BILL ||
                        r.Id == CNETConstantes.PAYMNET_METHOD_CREDITCARD).ToList();
                le_PaymentType.Properties.DataSource = (paymentList.OrderByDescending(c => c.IsDefault).ToList());

                //Profiles
                GetAllGuests();
                GetAllCompanies();
                GetAllContacts();
                GetAllAgents();
                GetAllSources();
                GetAllGroups();

                //Load Other Consignees
                LoadOtherConsignees(RegExtension);


                //populate other fields
                teRegNo.Text = RegExtension.Registration;
                cac_guest.EditValue = RegExtension.GuestId;
                if (paymentList != null)
                {
                    SystemConstantDTO payment = paymentList.Where(p => p.Description == RegExtension.PaymentDesc).FirstOrDefault();
                    if (payment != null)
                    {
                        le_PaymentType.EditValue = payment.Id;
                        le_PaymentType.RefreshEditValue();
                    }
                }


                leRoomType.EditValue = RegExtension.RoomType;
                be_Room.Text = RegExtension.Room;
                te_arrival.Text = RegExtension.Arrival.ToShortDateString();
                te_departure.Text = RegExtension.Departure.ToShortDateString();

                //get Guest Ledger
                GuestLedgerDTO gLedger = UIProcessManager.GetGuestLedger(RegExtension.Id, RegExtension.Arrival.Date, RegExtension.Departure.Date, RegExtension.Room, null);
                if (gLedger != null)
                {
                    teTotalBill.Text = gLedger.TotalCredit.ToString();
                    tePaid.Text = gLedger.TotalPaid.ToString();
                    teRemaing.Text = gLedger.RemainingBalanceFormated;

                }

                if (RegExtension != null && RegExtension.RoomCode != null)
                {
                    RoomDetailDTO rd = UIProcessManager.GetRoomDetailById(RegExtension.RoomCode.Value);
                    SelectedRoomHandler.Invoke(rd);
                }
                ////CNETInfoReporter.Hide();

                return true;
            }
            catch (Exception ex)
            {
                ////CNETInfoReporter.Hide();
                XtraMessageBox.Show("Error in initializing form. DETAIL: " + ex.Message, "Check In", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

        }

        // Load Other Consignees
        private void LoadOtherConsignees(RegistrationListVMDTO Registration)
        {
            // Progress_Reporter.Show_Progress("Loading other consignees...");
            //List<OtherConsignee> otherConsignes = UIProcessManager.GetOtherConsigneesListByVoucher(regCode);
            //if (otherConsignes == null || otherConsignes.Count == 0) return;
            //_accompanyGuestList = otherConsignes.Where(oc => oc.requiredGSL == CNETConstantes.ACCOMPANYING_GUEST_REQUIRED_GSL_CODE).ToList();

            if (Registration.CompanyId != null)
            {
                cac_company.EditValue = Registration.CompanyId;
                //  _company = con;
            }
            if (Registration.ContactId != null)
            {
                cac_contact.EditValue = Registration.ContactId;
                //_contact = con;
            }
            if (Registration.AgentId != null)
            {
                cac_agent.EditValue = Registration.AgentId;
                //_agent = con;
            }
            if (Registration.GroupId != null)
            {
                cac_group.EditValue = Registration.GroupId;
                // _group = con;
            }
            if (Registration.GroupId != null)
            {
                cac_source.EditValue = Registration.GroupId;
                // _source = con;
            }
            ////CNETInfoReporter.Hide();

        }

        private void SelectedRoomEventHandler(RoomDetailDTO room)
        {

            if (room != null)
            {
                be_Room.EditValue = room.Description;
                _selectedRoomDetail = room;
                ActivityDefinitionDTO ad = UIProcessManager.GetActivityDefinitionById(room.LastState.Value);
                if (ad == null) return;
                if (ad.Description != CNETConstantes.CLEAN && ad.Description != CNETConstantes.INSPECTED)
                {
                    XtraMessageBox.Show("Room - " + room.Description + " is not clean or inspected", "CNET_v2016", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    be_Room.EditValue = null;
                }

                // get FO status
                //DateTime? currentTime = UIProcessManager.GetServiceTime();
                //if (currentTime != null)
                //{
                //    var foStatus = UIProcessManager.GetRegistrationStatus(room.Id, currentTime.Value.Date);
                //    if (foStatus != null)
                //    {
                //        if (foStatus.FOStatus == "1")
                //        {
                //            XtraMessageBox.Show("Room - " + room.Description + " is occupied!", "CNET_v2016", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //            be_Room.EditValue = null;
                //        }
                //    }
                //}

            }
        }

        // Auto Complete Free Rooms
        private void AutoCompleteFreeRooms()
        {
            if (regExtension.RoomType != null)
            {
                AutoCompleteStringCollection collection = new AutoCompleteStringCollection();
                List<RoomDetailDTO> roomList = UIProcessManager.GetUnassignedRoomsByState(RegExtension.Arrival, RegExtension.Departure, CNETConstantes.CHECKED_OUT_STATE).Where(t => t.RoomType == RegExtension.RoomType).ToList();
                if (roomList.Count > 0)
                {
                    string[] roomsListdesStrings = roomList.Select(r => r.Description).ToArray();
                    collection.AddRange(roomsListdesStrings);
                    be_Room.MaskBox.AutoCompleteSource = AutoCompleteSource.CustomSource;
                    be_Room.MaskBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                    be_Room.MaskBox.AutoCompleteCustomSource = collection;
                }
            }
        }

        // Get All Guests
        private void GetAllGuests()
        {
            //Guest List
            cac_guest.Properties.DataSource = LocalBuffer.LocalBuffer.AllGuestConsigneeViewlist;
        }

        // Get All Companies
        private void GetAllCompanies()
        {
            if (LocalBuffer.LocalBuffer.AllCustomerConsigneeViewlist == null) return;

            cac_company.Properties.DataSource = LocalBuffer.LocalBuffer.AllCustomerConsigneeViewlist;
        }

        // Get All Groups
        private void GetAllGroups()
        {

            var orgGroupList = LocalBuffer.LocalBuffer.ConsigneeViewBufferList.Where(o => o.GslType == CNETConstantes.GROUP && o.IsActive).ToList();
            if (orgGroupList == null) return;
            cac_group.Properties.DataSource = orgGroupList;
        }

        // Get All Agents
        private void GetAllAgents()
        {
            var orgAgentList = LocalBuffer.LocalBuffer.ConsigneeViewBufferList.Where(o => o.GslType == CNETConstantes.AGENT && o.IsActive).ToList();
            if (orgAgentList == null) return;


            cac_agent.Properties.DataSource = orgAgentList;
        }

        // Get All Sources
        private void GetAllSources()
        {
            var orgSoureceList = LocalBuffer.LocalBuffer.ConsigneeViewBufferList.Where(o => o.GslType == CNETConstantes.BUSINESSsOURCE && o.IsActive).ToList();
            if (orgSoureceList == null) return;

            cac_source.Properties.DataSource = orgSoureceList;
        }

        // Get All Contacts
        private void GetAllContacts()
        {

            var contactList = LocalBuffer.LocalBuffer.ConsigneeViewBufferList.Where(p => p.IsActive && p.GslType == CNETConstantes.CONTACT).ToList();
            if (contactList == null) return;
            cac_contact.Properties.DataSource = contactList;

        }

        #endregion

        #region Event Handlers

        private void bbiIssueCard_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmDoorLock frmDoorLock = new frmDoorLock();
            frmDoorLock.RegExt = RegExtension;
            frmDoorLock.ShowDialog();
        }

        private void bbiRegCard_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            MasterPageForm.PauseVideo();
            try
            {

                if (RegExtension != null)
                {
                    // Progress_Reporter.Show_Progress("Generating attachment print preview", "Please Wait..", 22, 23);
                    //ReportGenerator reportGenerator = new ReportGenerator();
                    //reportGenerator.GenerateAttachmentReport(RegExtension.Registration, true, true);
                    ////CNETInfoReporter.Hide();
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Error in generating print preview. DETAIL:: " + ex.Message, "CNET_v2016", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            MasterPageForm.PlayVideo();
        }



        private void bbiClose_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Close();
        }

        private void bbCheckIn_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                List<Control> controls = new List<Control>
                {
                    le_PaymentType,
                    leRoomType,
                    be_Room,
                    te_arrival,
                    te_departure,
                    cac_guest,

                };

                IList<Control> invalidControls = CustomValidationRule.Validate(controls);

                if (invalidControls.Count > 0)
                {
                    return;
                }
                // Room Move


                // Progress_Reporter.Show_Progress("Checking In Guest ...", "Please Wait...");

                DateTime? currentTime = UIProcessManager.GetServiceTime();
                if (currentTime == null)
                {
                    ////CNETInfoReporter.Hide();
                    return;
                }



                bool isMoved = RegExtension.Room != be_Room.Text;
                if (leRoomType.EditValue == null)
                {
                    ////CNETInfoReporter.Hide();
                    SystemMessage.ShowModalInfoMessage("Unable to save room type. Please try agin!", "ERROR");
                    return;
                }

                if (_selectedRoomDetail != null)
                {
                    RegistrationDetailDTO regDetail = new RegistrationDetailDTO()
                    {
                        Room = _selectedRoomDetail.Id,
                        RoomType = Convert.ToInt32(leRoomType.EditValue),
                        Remark = null, // Remark Null value is important since we are not going to update the rate here
                    };
                    if (isMoved)
                    {
                        if (!UIProcessManager.RoomMove(RegExtension.Id, RegExtension.Arrival, RegExtension.Departure, regDetail, new List<DailyRateCodeDTO>()))
                        {
                            ////CNETInfoReporter.Hide();
                            SystemMessage.ShowModalInfoMessage("Unable to save room update. Please try agin!", "ERROR");
                            return;
                        }
                    }

                }
                else
                {
                    ////CNETInfoReporter.Hide();
                    SystemMessage.ShowModalInfoMessage("Unable to save room update. Please try agin!", "ERROR");
                    return;
                }
                try
                {
                    #region Update Other Consignees




                    #endregion
                }
                catch (Exception ex)
                {
                    //ignore
                }
                var response = UIProcessManager.GetVoucherBufferById(RegExtension.Id);
                if (response == null || !response.Success)
                {
                    ////CNETInfoReporter.Hide();
                    SystemMessage.ShowModalInfoMessage("Unable to get voucher data!", "ERROR");
                    return;
                }


                VoucherBuffer voucherbuffer = response.Data;
                if (voucherbuffer == null)
                {
                    ////CNETInfoReporter.Hide();
                    SystemMessage.ShowModalInfoMessage("Unable to get voucher data!", "ERROR");
                    return;
                }


                voucherbuffer.Voucher.Consignee1 = null;
                voucherbuffer.Voucher.Consignee2 = null;
                voucherbuffer.Voucher.Consignee3 = null;
                voucherbuffer.Voucher.Consignee4 = null;
                voucherbuffer.Voucher.Consignee5 = null;
                voucherbuffer.Voucher.Consignee6 = null;

                if (cac_guest.EditValue != null)
                    voucherbuffer.Voucher.Consignee1 = Int32.Parse(cac_guest.EditValue.ToString());

                if (cac_company.EditValue != null)
                    voucherbuffer.Voucher.Consignee2 = Int32.Parse(cac_company.EditValue.ToString());

                if (cac_contact.EditValue != null)
                    voucherbuffer.Voucher.Consignee4 = Int32.Parse(cac_contact.EditValue.ToString());

                if (cac_agent.EditValue != null)
                    voucherbuffer.Voucher.Consignee3 = Int32.Parse(cac_agent.EditValue.ToString());

                if (cac_group.EditValue != null)
                    voucherbuffer.Voucher.Consignee6 = Int32.Parse(cac_group.EditValue.ToString());

                if (cac_source.EditValue != null)
                    voucherbuffer.Voucher.Consignee5 = Int32.Parse(cac_source.EditValue.ToString());

                voucherbuffer.Voucher.StartDate = regExtension.Arrival;
                voucherbuffer.Voucher.EndDate = regExtension.Departure;
                voucherbuffer.Voucher.PaymentMethod = Convert.ToInt32(le_PaymentType.EditValue);

                voucherbuffer.Activity = ActivityLogManager.SetupActivity(currentTime.Value, ad, CNETConstantes.PMS_Pointer, "Check-In made from Another State");
                voucherbuffer.Voucher.LastState = CNETConstantes.CHECKED_IN_STATE;
                if (voucherbuffer.TransactionReferencesBuffer != null && voucherbuffer.TransactionReferencesBuffer.Count > 0)
                    voucherbuffer.TransactionReferencesBuffer.ToList().ForEach(x => x.ReferencedActivity = null);


                voucherbuffer.TransactionCurrencyBuffer = null;


                if (UIProcessManager.UpdateVoucherBuffer(voucherbuffer) != null)
                {

                    DialogResult = System.Windows.Forms.DialogResult.OK;

                    //update exchange rate
                    var rateHeader = UIProcessManager.GetRateCodeHeaderById(RegExtension.RateCodeHeader.Value);
                    if (rateHeader != null)
                    {
                        if (rateHeader.ExchangeRule == CNETConstantes.ArrivalDayRate)
                        {
                            UIProcessManager.ApplyExchangeRate(RegExtension.Id, CNETConstantes.ArrivalDayRate, "CheckIn", rateHeader.CurrencyCode);
                        }
                    }




                    SystemMessage.ShowModalInfoMessage("State changed successfully!", "MESSAGE");

                    // Charge during check-in
                    try
                    {
                        if (voucherbuffer.Voucher.LastState == CNETConstantes.CHECKED_IN_STATE)
                        {
                            //Check Configuration
                            var chargeAtCheckin = LocalBuffer.LocalBuffer.ConfigurationBufferList.FirstOrDefault(c => c.Reference == CNETConstantes.PMS_Pointer.ToString() && c.Attribute == CNETConstantes.PMS_SETTING_ChargeAtCheckIn);
                            bool canCharge = false;
                            if (chargeAtCheckin != null)
                            {
                                canCharge = Convert.ToBoolean(chargeAtCheckin.CurrentValue);
                            }

                            if (canCharge)
                            {

                                int? rateCodeHeader = RegExtension.RateCodeHeader;
                                if (rateCodeHeader != null)
                                    CommonLogics.ChargeAtCheckin(voucherbuffer.Voucher.Id, currentTime.Value, rateCodeHeader.Value, voucherbuffer.Voucher.Consignee1, RegExtension.ConsigneeUnit.Value, false);
                            }



                            if (LocalBuffer.LocalBuffer.EarlyCheckIn && LocalBuffer.LocalBuffer.EarlyCheckInArticle != null && currentTime <= LocalBuffer.LocalBuffer.EarlyCheckInUntilTime)
                            {
                                if (LocalBuffer.LocalBuffer.EarlyCheckInChargeMandatory)
                                    XtraMessageBox.Show("The Customer is Early Check-In." + Environment.NewLine + "There will be an Early Room Charge !!", "CNET-ERPV6", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                if (LocalBuffer.LocalBuffer.EarlyCheckInChargeMandatory || XtraMessageBox.Show("The Customer is Early Check-In." + Environment.NewLine + "Do you want to Early Room Charge ??", "CNET-ERPV6", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK)
                                {
                                    CommonLogics.ChargeAtEarlyCheckin(voucherbuffer.Voucher.Id, currentTime.Value, voucherbuffer.Voucher.Consignee1.Value, SelectedHotelcode, LocalBuffer.LocalBuffer.EarlyCheckInArticle, true);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }

                    MasterPageForm.PauseVideo();
                    try
                    {
                        // Generating attachment print preview
                        // Progress_Reporter.Show_Progress("Generating attachment print preview", "Please Wait..", 22, 23);
                        //bool isDeviceAvailable = false;
                        //ReportGenerator reportGenerator = new ReportGenerator();
                        //try
                        //{
                        //    //read device setting
                        //    DeviceDTO winTabDevice = UIProcessManager.GetDeviceByhostandpreference(LocalBuffer.LocalBuffer.CurrentDevice.Id, CNETConstantes.DEVICE_SIGNATURE).FirstOrDefault();
                        //    if (winTabDevice != null)
                        //    {
                        //        isDeviceAvailable = true;
                        //    }
                        //}
                        //catch (Exception ex) { }

                        //bool flag = ScribbleWinTab.TabInfo.IsTabAvailable() && isDeviceAvailable;

                        //reportGenerator.GenerateAttachmentReport(voucherbuffer.Voucher.Id, true, true, flag);

                        ////CNETInfoReporter.Hide();
                    }
                    catch (Exception ex)
                    {
                        //ignore
                    }
                    MasterPageForm.PlayVideo();
                }
                else
                {
                    ////CNETInfoReporter.Hide();
                    SystemMessage.ShowModalInfoMessage("NOT SUCCESSFUL!", "ERROR");
                }



            }
            catch (Exception ex)
            {
                ////CNETInfoReporter.Hide();
                XtraMessageBox.Show("ERROR! " + ex.Message, "CNET_v2016", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        public static void SynchronizeRegistration(string Registration)
        {


        }

        private void be_Room_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            Dictionary<String, String> args = new Dictionary<string, string>();
            int nights = Convert.ToInt32((RegExtension.Departure - RegExtension.Arrival).TotalDays);
            args.Add("Guest Name", cac_guest.Text);
            args.Add("Arrival", RegExtension.Arrival.ToString());
            args.Add("Departure", RegExtension.Departure.ToString());
            args.Add("Nights", nights.ToString());
            args.Add("RoomType", leRoomType.EditValue.ToString());
            args.Add("RoomTypeDescription", leRoomType.Text.ToString());

            args.Add("CHECK_HK", "YES");
            //  Home.OpenForm(this, "ROOM SEARCH", args);
            frmRoomSearch frmRoom = new frmRoomSearch();
            frmRoom.SelectedHotelcode = SelectedHotelcode;
            frmRoom.LoadData(this, args);
            if (frmRoom.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {

            }
        }

        private void be_Room_EditValueChanged(object sender, EventArgs e)
        {

            //if (rd != null)
            //{
            //      SelectedRoomHandler.Invoke(rd);
            //}
        }

        private void be_Room_Validating(object sender, CancelEventArgs e)
        {
            if (!string.IsNullOrEmpty(be_Room.Text))
            {
                List<RoomDetailDTO> roomList = UIProcessManager.GetUnassignedRoomsByState(RegExtension.Arrival, RegExtension.Departure, CNETConstantes.CHECKED_OUT_STATE).Where(r => r.IsActive != null && r.IsActive).ToList();
                RoomDetailDTO rd = new RoomDetailDTO();
                if (RegExtension.RoomType != null)
                {
                    rd =
                        roomList.FirstOrDefault(
                            r => r.Description == be_Room.Text && r.RoomType == RegExtension.RoomType);
                }
                else
                {
                    rd =
                        roomList.FirstOrDefault(
                            r => r.Description == be_Room.Text);
                }
                if (rd != null)
                {
                    SelectedRoomHandler.Invoke(rd);
                }
                else
                {
                    e.Cancel = true;
                }
            }
        }

        private void leRoomType_EditValueChanged(object sender, EventArgs e)
        {
            be_Room.Text = "";
        }

        private void cac_guest_AddNewValue(object sender, DevExpress.XtraEditors.Controls.AddNewValueEventArgs e)
        {
            try
            {


                this.SubForm = this;
                frmPerson frmperson = new frmPerson("Guest");
                frmperson.Text = "Guest";
                frmperson.GSLType = CNETConstantes.GUEST;
                frmperson.rpgScanFingerPrint.Visible = true;
                frmperson.LoadEventArg.Args = "Guest";
                frmperson.LoadData(this, this);
                frmperson.LoadEventArg.Sender = null;
                if (frmperson.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    ///GetPerson();
                    if (frmperson.SavedPerson != null)
                    {
                        GetAllGuests();
                        cac_guest.EditValue = frmperson.SavedPerson.Id;
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void cac_company_AddNewValue(object sender, AddNewValueEventArgs e)
        {
            this.SubForm = this;
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
                    cac_company.Properties.DataSource = null;
                    GetAllCompanies();
                    cac_company.EditValue = organization.SavedOrg.Code;
                }
            }
        }

        private void cac_agent_AddNewValue(object sender, AddNewValueEventArgs e)
        {
            this.SubForm = this;
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
                    cac_agent.Properties.DataSource = null;
                    GetAllAgents();
                    cac_agent.EditValue = organization.SavedOrg.Code;
                }
            }
        }

        private void cac_source_AddNewValue(object sender, AddNewValueEventArgs e)
        {
            this.SubForm = this;
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
                    cac_source.Properties.DataSource = null;
                    GetAllSources();
                    cac_source.EditValue = organization.SavedOrg.Code;
                }
            }
        }

        private void cac_group_AddNewValue(object sender, AddNewValueEventArgs e)
        {
            this.SubForm = this;
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
                    cac_group.Properties.DataSource = null;
                    GetAllGroups();
                    cac_group.EditValue = organization.SavedOrg.Code;
                }
            }
        }

        private void cac_contact_AddNewValue(object sender, AddNewValueEventArgs e)
        {
            this.SubForm = this;
            frmPerson person = new frmPerson("Contact");
            person.GSLType = CNETConstantes.CONTACT;
            person.Text = "Contact";
            person.LoadEventArg.Args = "Contact";
            person.LoadEventArg.Sender = null;
            person.LoadData(this, null);
            if (person.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ///GetPerson();
                if (person.SavedPerson != null)
                {
                    cac_contact.Properties.DataSource = null;
                    GetAllContacts();
                    cac_contact.EditValue = person.SavedPerson.Id;
                }
            }
        }

        private void frmCheckIn_Load(object sender, EventArgs e)
        {
            if (!InitializeData())
            {
                this.Close();
            }
        }

        #endregion






        public int SelectedHotelcode { get; set; }
    }
}