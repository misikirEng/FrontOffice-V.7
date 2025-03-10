using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using CNET.ERP.Client.Common.UI;
using CNET.FrontOffice_V._7;
using ProcessManager;
using CNET.FrontOffice_V._7.PMS.Common_Classes;
using CNET_V7_Domain.Domain.CommonSchema;
using CNET_V7_Domain.Domain.PmsSchema;
using CNET_V7_Domain.Domain.SettingSchema;
using CNET_V7_Domain.Domain.TransactionSchema;
using CNET_V7_Domain.Domain.ViewSchema;
using CNET_V7_Domain.Misc.PmsDTO;
using CNET_V7_Domain.Misc.PmsView;
using DevExpress.Mvvm.POCO;
using DevExpress.Utils.Win;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Columns;
using CNET_V7_Domain.Domain.ConsigneeSchema;
using CNET_V7_Domain.Misc;

namespace CNET.FrontOffice_V._7.Forms.Non_Navigatable_Modals
{
    public partial class frmMultipleRoomCheckIn : UILogicBase
    {

        private List<MultibleCheckInDto> _dtoList = new List<MultibleCheckInDto>();
        private int? _adCode = null;

        public bool IsRoomAssignment { get; set; }
        public string currentVoCode { get; set; }

        public SelectedRoom SelectedRoomHandler { get; set; }
        public SelectedGuest SelectedPersonHandler { get; set; }

        RegistrationListVMDTO regExtension;
        internal RegistrationListVMDTO RegExtension
        {
            get { return regExtension; }
            set
            {
                regExtension = value;
            }
        }

        public VoucherBuffer CurrentVoucher { get; set; }
        public bool IsFromNightAudit { get; set; }

        /*********************** CONSTRUCTOR ***************************/
        public frmMultipleRoomCheckIn(bool isCheckIn)
        {
            InitializeComponent();
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            if (isCheckIn)
            {
                IsRoomAssignment = false;
                lcMultipleCheckIn.Text = @"You are checking in the guest(s) below";

            }
            else
            {
                IsRoomAssignment = true;
                lcMultipleCheckIn.Text = @"You are assigning rooms to the guest(s) below";
                rgpAutoRoomAssign.Visible = true;
                rpgRoomAssign.Visible = true;
                rgpCheckIn.Visible = false;
                rgpPrintCard.Visible = false;

            }


            InitializeUI();

        }


        #region Helper Methods

        private void InitializeUI()
        {
            GridColumn column = risle_guest.View.Columns.AddField("Id");
            column.Visible = false;
            column = risle_guest.View.Columns.AddField("Code");
            column.Visible = true;
            column = risle_guest.View.Columns.AddField("FirstName");
            column.Visible = true;
            column = risle_guest.View.Columns.AddField("SecondName");
            column.Visible = true;
            column = risle_guest.View.Columns.AddField("BioId");
            column.Caption = "Id Number";
            column.Visible = true;
            risle_guest.DisplayMember = "FirstName";
            risle_guest.ValueMember = "Id";

            SelectedRoomHandler = SelectedRoomEventHandler;
            SelectedPersonHandler = SelectedPersonEventHandler;
            risle_guest.EditValueChanged += risleGuest_EditValueChanged;
            ribeRoom.ButtonClick += ribeRoom_ButtonClick;
            risle_guest.AddNewValue += risleGuest_AddNewValue;
        }



        private bool InitializeData()
        {
            try
            {
                if (RegExtension == null)
                {
                    SystemMessage.ShowModalInfoMessage("Please select a registration!", "ERROR");
                    return false;
                }

                // Progress_Reporter.Show_Progress("Initializing Data", "Please Wait...");

                #region Check Workflow

                if (!IsRoomAssignment)
                {
                    ActivityDefinitionDTO workFlow = UIProcessManager.GetActivityDefinitionBydescriptionandreference(CNETConstantes.LU_ACTIVITY_DEFINATION_CheckIN, CNETConstantes.REGISTRATION_VOUCHER).FirstOrDefault();

                    if (workFlow != null)
                    {
                        _adCode = workFlow.Id;
                    }
                    else
                    {
                        ////CNETInfoReporter.Hide();
                        SystemMessage.ShowModalInfoMessage("Please define workflow of CHECK-IN for Registration Voucher ", "ERROR");
                        return false;
                    }
                }
                else
                {
                    //Six PM
                    if (RegExtension.lastState == CNETConstantes.SIX_PM_STATE)
                    {
                        ActivityDefinitionDTO workFlowSixPM = UIProcessManager.GetActivityDefinitionBydescriptionandreference(CNETConstantes.LU_ACTIVITY_DEFINATION_6PM, CNETConstantes.REGISTRATION_VOUCHER).FirstOrDefault();

                        if (workFlowSixPM != null)
                        {
                            _adCode = workFlowSixPM.Id;
                        }
                        else
                        {
                            ////CNETInfoReporter.Hide();
                            SystemMessage.ShowModalInfoMessage("Please define workflow of SIX PM for Registration Voucher ", "ERROR");
                            return false;
                        }
                    }

                    //Waiting List
                    if (RegExtension.lastState == CNETConstantes.OSD_WAITLIST_STATE)
                    {
                        ActivityDefinitionDTO workFlowWaitingList = UIProcessManager.GetActivityDefinitionBydescriptionandreference(CNETConstantes.LU_ACTIVITY_DEFINATION_Waitinglist, CNETConstantes.REGISTRATION_VOUCHER).FirstOrDefault();


                        if (workFlowWaitingList != null)
                        {
                            _adCode = workFlowWaitingList.Id;
                        }
                        else
                        {
                            ////CNETInfoReporter.Hide();
                            SystemMessage.ShowModalInfoMessage("Please define workflow of WAITING LIST for Registration Voucher ", "ERROR");
                            return false;
                        }
                    }

                    //Guaranted
                    if (RegExtension.lastState == CNETConstantes.GAURANTED_STATE)
                    {
                        ActivityDefinitionDTO workFlowGuaranteed = UIProcessManager.GetActivityDefinitionBydescriptionandreference(CNETConstantes.LU_ACTIVITY_DEFINATION_Guaranteed, CNETConstantes.REGISTRATION_VOUCHER).FirstOrDefault();

                        if (workFlowGuaranteed != null)
                        {
                            _adCode = workFlowGuaranteed.Id;
                        }
                        else
                        {
                            ////CNETInfoReporter.Hide();
                            SystemMessage.ShowModalInfoMessage("Please define workflow of GUARANTEED for Registration Voucher ", "ERROR");
                            return false;
                        }
                    }
                }


                #endregion
                var response = UIProcessManager.GetVoucherBufferById(RegExtension.Id);
                if (response == null || !response.Success)
                {
                    ////CNETInfoReporter.Hide();
                    SystemMessage.ShowModalInfoMessage("Unable to get voucher data!", "ERROR");
                    return false;
                }
                CurrentVoucher = response.Data;

                if (RegExtension != null)
                {
                    this.Text += @" - " + RegExtension.Registration;


                    for (int i = 0; i < RegExtension.NoOfRoom; i++)
                    {
                        MultibleCheckInDto dto = new MultibleCheckInDto();
                        dto.company = RegExtension.Company;
                        dto.consigneeCode = null;
                        dto.regId = null;
                        dto.regNo = null;
                        dto.arrival = RegExtension.Arrival;
                        dto.departure = RegExtension.Departure;
                        dto.personNo = 1;
                        if (i == RegExtension.NoOfRoom - 1)
                        {
                            dto.consigneeCode = RegExtension.GuestId;

                            dto.regId = RegExtension.Id;
                            dto.regNo = RegExtension.Registration;
                        }
                        _dtoList.Add(dto);
                    }
                    gcMultipleCheckIn.DataSource = _dtoList;
                    gvMultipleCkeckIN.RefreshData();
                }


                PopulateGuest();


                ////CNETInfoReporter.Hide();

                return true;
            }
            catch (Exception ex)
            {
                ////CNETInfoReporter.Hide();
                XtraMessageBox.Show("Error in Initializing Data! Detail: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void PopulateGuest()
        {
            risle_guest.DataSource = LocalBuffer.LocalBuffer.AllGuestConsigneeViewlist;
        }

        private void OnSave()
        {
            try
            {
                gvMultipleCkeckIN.PostEditor();
                List<MultibleCheckInDto> dtoList = gvMultipleCkeckIN.DataSource as List<MultibleCheckInDto>;
                if (dtoList == null || dtoList.Count == 0)
                {
                    SystemMessage.ShowModalInfoMessage("Noting to save!", "ERROR");
                    return;
                }

                MultibleCheckInDto originalDto = dtoList.FirstOrDefault(d => !string.IsNullOrEmpty(d.regNo));
                if (originalDto == null)
                {
                    SystemMessage.ShowModalInfoMessage("Unable to get Origional Registration!!", "ERROR");
                    return;
                }

                var notAssigned = dtoList.Where(d => d.roomCode == null).ToList();
                if (notAssigned != null && notAssigned.Count > 0)
                {
                    DialogResult dr = XtraMessageBox.Show("Room is not assigned for one or more guests. Unassigned rooms will be merged to the origional registration. Do you want to continue?", "CNET ERP", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (dr == System.Windows.Forms.DialogResult.No)
                        return;
                }

                DateTime? currentTime = UIProcessManager.GetServiceTime();
                if (currentTime == null) return;

                int counter = 0;
                VoucherBuffer SavedBuffer = null;


                string infoMsg = IsRoomAssignment ? "Saving Room Assignment" : "Saving Multiple Room Check-In";
                // Progress_Reporter.Show_Progress(infoMsg, "Please Wait...");

                List<RegistrationDetailDTO> regDetailList = UIProcessManager.GetRegistrationDetailByvoucher(RegExtension.Id);
                if (regDetailList == null || regDetailList.Count == 0)
                {
                    ////CNETInfoReporter.Hide();
                    SystemMessage.ShowModalInfoMessage("Unable to get registration detail!", "ERROR");
                    return;
                }

                foreach (var dto in dtoList)
                {

                    if (!string.IsNullOrEmpty(dto.regNo) || dto.roomCode == null)
                    {
                        continue;
                    }


                    string currentVoCode = UIProcessManager.IdGenerater("Voucher", CNETConstantes.REGISTRATION_VOUCHER, 1, LocalBuffer.LocalBuffer.CurrentDeviceConsigneeUnit.Value, false, LocalBuffer.LocalBuffer.CurrentDevice.Id);

                    if (!string.IsNullOrEmpty(currentVoCode))
                    {
                        currentVoCode = currentVoCode;
                    }
                    else
                    {
                        ////CNETInfoReporter.Hide();
                        SystemMessage.ShowModalInfoMessage("There is a problem on id setting !!!", "ERROR");
                        return;
                    }


                    //save Voucher
                    VoucherBuffer voucherbuffer = new VoucherBuffer();
                    voucherbuffer.Voucher = new VoucherDTO()
                    {
                        Code = currentVoCode,
                        Definition = CNETConstantes.REGISTRATION_VOUCHER,
                        Consignee1 = dto.consigneeCode,
                        IssuedDate = currentTime.Value,
                        Year = currentTime.Value.Year,
                        Month = currentTime.Value.Month,
                        Day = currentTime.Value.Day,
                        IsIssued = true,
                        IsVoid = false,
                        GrandTotal = 0,
                        Period = LocalBuffer.LocalBuffer.GetPeriodCode(currentTime.Value),
                        OriginConsigneeUnit = SelectedHotelcode,
                        Type = CurrentVoucher.Voucher.Type,
                        Extension2 = CurrentVoucher.Voucher.Extension2,
                        Extension3 = CurrentVoucher.Voucher.Extension3,
                        HasEffect = false,
                        Consignee2 = CurrentVoucher.Voucher.Consignee2,
                        Consignee3 = CurrentVoucher.Voucher.Consignee3,
                        Consignee4 = CurrentVoucher.Voucher.Consignee4,
                        Consignee5 = CurrentVoucher.Voucher.Consignee5,
                        Consignee6 = CurrentVoucher.Voucher.Consignee6,
                        PaymentMethod = CNETConstantes.PAYMENTMETHODSCASH,
                        StartDate = dto.arrival,
                        EndDate = dto.departure

                    };

                    if (IsRoomAssignment)
                        voucherbuffer.Voucher.LastState = CurrentVoucher.Voucher.LastState;
                    else
                        voucherbuffer.Voucher.LastState = CNETConstantes.CHECKED_IN_STATE;
                    voucherbuffer.TransactionReferencesBuffer = new List<TransactionReferenceBuffer>();
                    TransactionReferenceBuffer TrBuffer = new TransactionReferenceBuffer();
                    TrBuffer.TransactionReference = new TransactionReferenceDTO()
                    {
                        ReferencingVoucherDefn = CNETConstantes.REGISTRATION_VOUCHER,
                        Referenced = originalDto.regId,
                        ReferencedVoucherDefn = CNETConstantes.REGISTRATION_VOUCHER,
                        Value = 0
                    };
                    TrBuffer.ReferencedActivity = ActivityLogManager.SetupActivity(currentTime.Value, _adCode.Value, CNETConstantes.PMS_Pointer, "From multiple room form");
                    voucherbuffer.TransactionReferencesBuffer.Add(TrBuffer);

                    //Save Activity
                    voucherbuffer.Activity = ActivityLogManager.SetupActivity(currentTime.Value, _adCode.Value, CNETConstantes.PMS_Pointer, "From multiple room form");









                    voucherbuffer.Voucher.LastUser = LocalBuffer.LocalBuffer.CurrentLoggedInUser.Id;
                    voucherbuffer.Voucher.LastDevice = LocalBuffer.LocalBuffer.CurrentDevice.Id;
                    voucherbuffer.TransactionCurrencyBuffer = null;

                    var Voucherdata = UIProcessManager.CreateVoucherBuffer(voucherbuffer);


                    if (Voucherdata == null || !Voucherdata.Success)
                    {
                        ////CNETInfoReporter.Hide();
                        SystemMessage.ShowModalInfoMessage("Voucher is not saved for " + dto.guest + Environment.NewLine + Voucherdata.Message, "ERROR");
                        return;
                    }
                    else
                    {
                        SavedBuffer = Voucherdata.Data;
                        counter++;
                    }




                    //Save Registration Detail
                    foreach (var regDetail in regDetailList)
                    {
                        RegistrationDetailDTO newRegDetail = new RegistrationDetailDTO()
                        {
                            Voucher = SavedBuffer.Voucher.Id,
                            ActualRtc = regDetail.ActualRtc,
                            Adjustment = regDetail.Adjustment,
                            Adult = regDetail.Adult,
                            Child = regDetail.Child,
                            Date = regDetail.Date,
                            Day = regDetail.Day,
                            IsClosed = regDetail.IsClosed,
                            IsFixedRate = regDetail.IsFixedRate,
                            Market = regDetail.Market,
                            RateAmount = regDetail.RateAmount,
                            RateCode = regDetail.RateCode,
                            Remark = regDetail.Remark,
                            Room = dto.roomCode,
                            RoomType = regDetail.RoomType,
                            RoomCount = 1,
                            Source = regDetail.Source,
                            Year = regDetail.Year
                        };
                        RegistrationDetailDTO savedRegDetail = UIProcessManager.CreateRegistrationDetail(newRegDetail);
                        if (savedRegDetail != null)
                        {
                            List<PackagesToPostDTO> pkToPostList = UIProcessManager.GetPackagesToPostByRegistrationDetail(regDetail.Id).ToList();
                            if (pkToPostList == null || pkToPostList.Count == 0)
                                continue;
                            foreach (var ptp in pkToPostList)
                            {
                                ptp.RegistrationDetail = savedRegDetail.Id;
                                UIProcessManager.CreatePackagesToPost(ptp);
                            }

                        }
                    }


                    //Save Registration Prevledge
                    RegistrationPrivllegeDTO previlage = UIProcessManager.GetRegistrationPrivllegeByvoucher(CurrentVoucher.Voucher.Id);
                    if (previlage != null)
                    {
                        previlage.Id = 0;
                        previlage.Voucher = SavedBuffer.Voucher.Id;

                    }
                    else
                    {
                        previlage = new RegistrationPrivllegeDTO();
                        previlage.Voucher = SavedBuffer.Voucher.Id;
                        previlage.Nopost = false;
                        previlage.AuthorizeDirectBill = false;
                        previlage.PreStayCharging = false;
                        previlage.PostStayCharging = false;
                        previlage.AllowLatecheckout = false;
                    }
                    UIProcessManager.CreateRegistrationPrivllege(previlage);

                }//end of foreach loop


                bool isSaved = true;
                foreach (var regDetail in regDetailList)
                {
                    if (counter == 0)
                    {
                        if (originalDto.roomCode == null)
                        {
                            isSaved = false;
                            break;
                        }
                        regDetail.Room = originalDto.roomCode;
                        regDetail.RoomCount = 1;
                        RegExtension.NoOfRoom = 1;
                    }
                    else
                    {
                        if (originalDto.roomCode != null)
                        {
                            regDetail.Room = originalDto.roomCode;
                            regDetail.RoomCount = 1;
                            RegExtension.NoOfRoom = 1;
                        }
                        else
                        {
                            regDetail.RoomCount = regDetail.RoomCount - counter;
                            RegExtension.NoOfRoom = regDetail.RoomCount.Value;
                        }
                    }
                    RegistrationDetailDTO flag = UIProcessManager.UpdateRegistrationDetail(regDetail);
                    if (counter == 0)
                        isSaved = flag != null;
                }//end of foreach

                //update to CHECK-IN state
                if (!IsRoomAssignment)
                {
                    var regD = regDetailList.FirstOrDefault(r => r.Room == null);
                    if (regD == null && isSaved && RegExtension.NoOfRoom == 1)
                    {
                        CurrentVoucher.Voucher.LastState = CNETConstantes.CHECKED_IN_STATE;
                        CurrentVoucher.Voucher.HasEffect = true;//Is Master
                        CurrentVoucher.Activity = ActivityLogManager.SetupActivity(currentTime.Value, _adCode.Value, CNETConstantes.PMS_Pointer, "From multiple room form");
                        //if (CurrentVoucher.TransactionReferencesBuffer != null && CurrentVoucher.TransactionReferencesBuffer.Count > 0)
                        //    CurrentVoucher.TransactionReferencesBuffer.ToList().ForEach(x => x.ReferencedActivity = new CNET_V7_Domain.Domain.CommonSchema.ActivityDTO());

                        CurrentVoucher.TransactionCurrencyBuffer = null;

                        ResponseModel<VoucherBuffer> saveFlag = UIProcessManager.UpdateVoucherBuffer(CurrentVoucher);

                    }
                    else
                    {
                        XtraMessageBox.Show("Origional registration is not changed to Check-In state!", "Multiple Room Assignment", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    }
                }


                if (isSaved)
                {
                    if (IsRoomAssignment)
                    {
                        SystemMessage.ShowModalInfoMessage("Multiple Room Assignment Is Successfull!!", "MESSAGE");

                    }
                    else
                    {
                        SystemMessage.ShowModalInfoMessage("Multiple Room CHECK-IN Is Successfull!!", "MESSAGE");
                    }

                    this.Close();
                }
                else
                {
                    if (IsRoomAssignment)
                    {
                        SystemMessage.ShowModalInfoMessage("Multiple Room Assignment Is Not Successfull!!", "ERROR");

                    }
                    else
                    {
                        SystemMessage.ShowModalInfoMessage("Multiple Room CHECK-IN Is Not Successfull!!", "ERROR");
                    }
                }

                //refresh dto;
                gcMultipleCheckIn.DataSource = null;
                gcMultipleCheckIn.DataSource = _dtoList;
                gvMultipleCkeckIN.RefreshData();

                ////CNETInfoReporter.Hide();

            }
            catch (Exception ex)
            {
                ////CNETInfoReporter.Hide();
                SystemMessage.ShowModalInfoMessage("Error in saving multiple room! Detail:: " + ex.Message, "ERROR");
            }
        }

        private void SelectedRoomEventHandler(RoomDetailDTO room)
        {
            if (room != null)
            {
                List<MultibleCheckInDto> getCheckInDtos = gvMultipleCkeckIN.DataSource as List<MultibleCheckInDto>;
                if (getCheckInDtos == null || getCheckInDtos.Count == 0) return;
                var dto = getCheckInDtos.FirstOrDefault(d => d.roomCode == room.Id);
                if (dto != null)
                {
                    SystemMessage.ShowModalInfoMessage("This room is already selected. Please change the room.", "ERROR");

                }
                else
                {
                    MultibleCheckInDto selectedDTO = (MultibleCheckInDto)gvMultipleCkeckIN.GetFocusedRow();
                    if (selectedDTO == null)
                    {
                        SystemMessage.ShowModalInfoMessage("Please Select a Guest!", "ERROR");
                        return;
                    }
                    selectedDTO.room = room.Description;
                    selectedDTO.roomCode = room.Id;
                    gcMultipleCheckIn.RefreshDataSource();
                    gvMultipleCkeckIN.RefreshData();
                    gvMultipleCkeckIN.FocusedColumn = gvMultipleCkeckIN.Columns["arrival"];
                }


            }
        }

        private void SelectedPersonEventHandler(ConsigneeDTO person)
        {
            if (person != null)
            {
                MultibleCheckInDto selectedDTO = (MultibleCheckInDto)gvMultipleCkeckIN.GetFocusedRow();
                if (selectedDTO == null)
                {
                    SystemMessage.ShowModalInfoMessage("Please Select a Guest!", "ERROR");
                    return;
                }
                selectedDTO.guest = person.FirstName + " " + person.SecondName + " " + person.ThirdName;
                selectedDTO.consigneeCode = person.Id;
                gcMultipleCheckIn.RefreshDataSource();
                gvMultipleCkeckIN.RefreshData();
                gvMultipleCkeckIN.FocusedColumn = gvMultipleCkeckIN.Columns["room"];

            }
        }

        #endregion

        #region Event Handlers 

        void risleGuest_EditValueChanged(object sender, EventArgs e)
        {
            SearchLookUpEdit view = sender as SearchLookUpEdit;
            if (view.EditValue == null) return;
            ConsigneeDTO personView = LocalBuffer.LocalBuffer.AllGuestConsigneeViewlist.FirstOrDefault(p => p.Id == Convert.ToInt32(view.EditValue.ToString()));
            if (personView != null)
            {
                SelectedPersonHandler.Invoke(personView);
            }
        }

        void risleGuest_AddNewValue(object sender, DevExpress.XtraEditors.Controls.AddNewValueEventArgs e)
        {
            this.SubForm = this;
            frmPerson person = new frmPerson("Guest");
            person.Text = "Guest";
            person.GSLType = CNETConstantes.GUEST;
            person.rpgScanFingerPrint.Visible = true;
            person.LoadEventArg.Args = "Guest";
            person.LoadData(this, this);
            person.LoadEventArg.Sender = null;
            if (person.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ///GetPerson();
                if (person.SavedPerson != null)
                {
                    risle_guest.DataSource = null;
                    PopulateGuest();
                    SelectedPersonHandler.Invoke(person.SavedPerson);
                }
            }

        }


        private void gvMultipleCkeckIN_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            if (e.Column.Caption == "SN")
                e.Value = e.ListSourceRowIndex + 1;
        }

        private void bbiClose_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Close();
        }

        private void bbCheckIn_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            OnSave();
        }

        private int FindRowHandleByRowObject(DevExpress.XtraGrid.Views.Grid.GridView view, object row)
        {
            if (row != null)
            {
                for (int i = 0; i < view.DataRowCount; i++)
                    if (row.Equals(view.GetRow(i)))
                        return i;
            }
            return DevExpress.XtraGrid.GridControl.InvalidRowHandle;
        }

        void ribeRoom_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            Dictionary<String, String> args = new Dictionary<string, string>();
            RoomTypeDTO roomType = UIProcessManager.GetRoomTypeById(RegExtension.RoomType.Value);
            if (!string.IsNullOrEmpty(RegExtension.Registration))
            {
                args.Add("Guest Name", RegExtension.Guest);
                args.Add("Arrival", RegExtension.Arrival.ToString());
                args.Add("Departure", RegExtension.Departure.ToString());
                args.Add("Nights", "");
                args.Add("RoomType", roomType.Id.ToString());
                args.Add("RoomTypeDescription", roomType.Description);

                if (IsRoomAssignment)
                    args.Add("CHECK_HK", "NO");
                else
                    args.Add("CHECK_HK", "YES");

                frmRoomSearch frmRoom = new frmRoomSearch();
                frmRoom.SelectedHotelcode = SelectedHotelcode;
                frmRoom.LoadData(this, args);
                frmRoom.ShowDialog();
            }

        }

        private void bbiRoomAssign_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            OnSave();
        }


        private void frmMultipleRoomCheckIn_Load(object sender, EventArgs e)
        {
            if (!InitializeData())
            {
                this.Close();
            }
        }

        private void bbiAutoRoomAssignment_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                // Progress_Reporter.Show_Progress("Auto-Room Assigning", "Please Wait...");

                List<MultibleCheckInDto> dtoList = gvMultipleCkeckIN.DataSource as List<MultibleCheckInDto>;
                if (dtoList == null || dtoList.Count == 0)
                {
                    ////CNETInfoReporter.Hide();
                    SystemMessage.ShowModalInfoMessage("There is no available room in the current room type (" + RegExtension.RoomTypeDescription + ")", "ERROR");
                    return;
                }

                List<int> pseudoRooms = new List<int>();
                List<RoomTypeDTO> pseudoRoomList = UIProcessManager.GetRoomTypeByispseudoRoomType(true);
                if (pseudoRoomList != null)
                    pseudoRooms = pseudoRoomList.Select(p => p.Id).ToList();
                List<int> stateList = new List<int>() { CNETConstantes.CHECKED_IN_STATE, CNETConstantes.OSD_WAITLIST_STATE, CNETConstantes.GAURANTED_STATE, CNETConstantes.OSD_WAITLIST_STATE };//, CNETConstantes.OSD_Category_Transaction };

                // List<int> stateList = new List<int>() { CNETConstantes.CHECKED_OUT_STATE, CNETConstantes.OSD_CANCEL_STATE };

                List<VwVoucherDetailWithRoomDetailViewDTO> avRooms = UIProcessManager.GetAvailabeRoomsByDateAndState(RegExtension.Arrival.Date, RegExtension.Departure.Date,
                        stateList, RegExtension.RoomType).ToList();

                if (avRooms == null || avRooms.Count == 0)
                {
                    ////CNETInfoReporter.Hide();
                    SystemMessage.ShowModalInfoMessage("There is no available room in the current room type (" + RegExtension.RoomTypeDescription + ")", "ERROR");
                    return;
                }
                List<int> addedRooms = dtoList.Select(d => d.roomCode).ToList();
                if (addedRooms != null && addedRooms.Count > 0)
                {
                    avRooms = avRooms.Where(r => !addedRooms.Contains(r.Code)).ToList();
                }

                if (avRooms.Count < dtoList.Count)
                {
                    for (int i = 0; i < avRooms.Count; i++)
                    {
                        var avRoom = avRooms.ElementAt(i);
                        var dto = dtoList.ElementAt(i);
                        dto.roomCode = avRoom.Code;
                        dto.room = avRoom.RoomDescription;
                    }
                }
                else
                {

                    for (int i = 0; i < dtoList.Count; i++)
                    {
                        var avRoom = avRooms.ElementAt(i);
                        var dto = dtoList.ElementAt(i);
                        dto.roomCode = avRoom.Code;
                        dto.room = avRoom.RoomDescription;
                    }

                }

                gcMultipleCheckIn.RefreshDataSource();
                gvMultipleCkeckIN.RefreshData();
                gvMultipleCkeckIN.FocusedColumn = gvMultipleCkeckIN.Columns["arrival"];

                ////CNETInfoReporter.Hide();

            }
            catch (Exception ex)
            {
                ////CNETInfoReporter.Hide();
                SystemMessage.ShowModalInfoMessage("Error in auto room assignment. Detail: " + ex.Message, "ERROR");
            }
        }
        #endregion


        private class MultibleCheckInDto
        {
            public String company { get; set; }
            public int? regId { get; set; }
            public string regNo { get; set; }
            public String guest { get; set; }
            public int? consigneeCode { get; set; }
            public int personNo { get; set; }
            public String room { get; set; }
            public int roomCode { get; set; }
            public DateTime arrival { get; set; }
            public DateTime departure { get; set; }
        }







        public int SelectedHotelcode { get; set; }
    }
}