using CNET.ERP.Client.Common.UI;
using CNET.FrontOffice_V._7;
using CNET.FrontOffice_V._7.Forms.Non_Navigatable_Modals;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CNET_V7_Domain.Misc.PmsDTO;
using CNET_V7_Domain.Domain.SettingSchema;
using ProcessManager;
using CNET.FrontOffice_V._7.PMS.Common_Classes;
using CNET_V7_Domain.Domain.PmsSchema;
using CNET_V7_Domain.Domain.TransactionSchema;
using CNET_V7_Domain.Misc.PmsView;
using DocumentPrint;
using CNET_V7_Domain.Misc;
using CNET.Progress.Reporter;
using CNET_V7_Domain.Domain.CommonSchema;

namespace CNET.FrontOffice_V._7.Forms.State_Change
{
    public partial class frmGroupCheckout : UILogicBase
    {
        private List<RegistrationListVMDTO> regListVM = new List<RegistrationListVMDTO>();

        List<string> logList = new List<string>();

        public RegistrationListVMDTO RegExtension { get; set; }

        private int? adCheckout = null;
        private int? adRegVoucherRoomCharge = null;

        private int? _adCodeDateAmend = null;

        private bool _isDoorLockHosted = false;

        private ConfigurationDTO configReturnCard;
        private ReconciliationSummaryDTO ReconciliationSummary;
        private List<ReconciliationDetailDTO> ReconciliationDetailDTO;

        public frmGroupCheckout()
        {
            InitializeComponent();

            InitializeUI();
        }

        #region Helper Methods

        private void InitializeUI()
        {
            repoCheckEdit.EditValueChanged += RepoCheckEdit_EditValueChanged;
        }

        DateTime? currentTime = DateTime.Now;

        private bool InitializeData()
        {
            try
            {
                if (RegExtension == null) return false;
                teName.Text = RegExtension.Guest;
                teCompany.Text = RegExtension.Company;
                teRegNo.Text = RegExtension.Registration;
                teRoomType.Text = RegExtension.RoomTypeDescription;
                teRoom.Text = RegExtension.Room;



                // Progress_Reporter.Show_Progress("Initializing Data", "Please Wait");

              currentTime = UIProcessManager.GetServiceTime();
                if (currentTime == null)
                {
                    ////CNETInfoReporter.Hide();
                    return false;
                }

                repoDepDate.MinValue = currentTime.Value;


                //Workflow
                //check workflow
                ActivityDefinitionDTO workFlow = UIProcessManager.GetActivityDefinitionBydescriptionandreference(CNETConstantes.LU_ACTIVITY_DEFINATION_CheckOut, CNETConstantes.REGISTRATION_VOUCHER).FirstOrDefault();

                if (workFlow != null)
                {

                    adCheckout = workFlow.Id;
                }
                else
                {
                    ////CNETInfoReporter.Hide();
                    SystemMessage.ShowModalInfoMessage("Please define workflow of CHECK-OUT for Registration Voucher ", "ERROR");
                    return false;
                }

                //check DateAmendment workflow

                ActivityDefinitionDTO workFlowDateAmend = UIProcessManager.GetActivityDefinitionBydescriptionandreference(CNETConstantes.LU_ACTIVITY_DEFINATION_DATE_AMENDED, CNETConstantes.REGISTRATION_VOUCHER).FirstOrDefault();

                if (workFlowDateAmend != null)
                {

                    _adCodeDateAmend = workFlowDateAmend.Id;
                }
                else
                {
                    ////CNETInfoReporter.Hide();
                    SystemMessage.ShowModalInfoMessage("Please define workflow of DATE AMENDED for Registration Voucher ", "ERROR");
                    return false;
                }

                //Check Activity Previlage
                var userRoleMapper = LocalBuffer.LocalBuffer.UserRoleMapperBufferList.FirstOrDefault(r => r.User == LocalBuffer.LocalBuffer.CurrentLoggedInUser.Id);
                if (userRoleMapper != null)
                {
                    var roleActivity = UIProcessManager.GetRoleActivityByactivityDefinition(adCheckout.Value).FirstOrDefault(r => r.Role == userRoleMapper.Role && r.NeedsPassCode);
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

                var userRoleMapperDateAmend = LocalBuffer.LocalBuffer.UserRoleMapperBufferList.FirstOrDefault(r => r.User == LocalBuffer.LocalBuffer.CurrentLoggedInUser.Id);
                if (userRoleMapperDateAmend != null)
                {
                    var roleActivity = UIProcessManager.GetRoleActivityByactivityDefinition(_adCodeDateAmend.Value).FirstOrDefault(r => r.Role == userRoleMapperDateAmend.Role && r.NeedsPassCode);
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

                //Check ROOM CHARGE MADE for REGISTRATION VOUCHER
                var workflowRegVoucher = UIProcessManager.GetActivityDefinitionBydescriptionandreference(CNETConstantes.LU_ACTIVITY_DEFINATION_ROOMCHARGE, CNETConstantes.REGISTRATION_VOUCHER).FirstOrDefault();

                if (workflowRegVoucher != null)
                {

                    adRegVoucherRoomCharge = workflowRegVoucher.Id;
                }
                else
                {
                    ////CNETInfoReporter.Hide();
                    SystemMessage.ShowModalInfoMessage("Please define workflow of ROOM-CHARGE-MADE for Registration Voucher ", "ERROR");
                    return false;
                }


                configReturnCard = LocalBuffer.LocalBuffer.ConfigurationBufferList.FirstOrDefault(c => c.Reference == CNETConstantes.PMS_Pointer.ToString() && c.Attribute == CNETConstantes.PMS_SETTING_EnforceCheckoutCardReturn);
                //voExtDoorLock = LocalBuffer.LocalBuffer.VoucherExtensionBufferList.FirstOrDefault(v => v.Type == CNETConstantes.VOUCHER_EXTENTION_DEFINITION_DOOR_LOCK && v.VoucherDefn == CNETConstantes.REGISTRATION_VOUCHER);


                //Check if Door lock is hosted
                //read device setting
                DeviceDTO doorLockDevice = UIProcessManager.GetDeviceByhostandpreference(LocalBuffer.LocalBuffer.CurrentDevice.Id, CNETConstantes.DEVICE_DOOR_LOCK);
                if (doorLockDevice == null)
                {
                    _isDoorLockHosted = false;
                }
                else
                {
                    _isDoorLockHosted = true;
                }


                if (regListVM != null)
                    regListVM.Clear();
                regListVM = UIProcessManager.GetRegistrationViewModelData(null, null, CNETConstantes.CHECKED_IN_STATE, SelectedHotelcode);


                //Populate Grid
                if (!PopulateTransferBills())
                {
                    ////CNETInfoReporter.Hide();
                    return false;
                }

                ceCheckAll.Checked = true;

                List<ReconciliationSummaryDTO> ReconciliationSummaryList = UIProcessManager.SelectAllReconciliationSummary();
                if (ReconciliationSummaryList != null && ReconciliationSummaryList.Count > 0)
                    ReconciliationSummary = ReconciliationSummaryList.FirstOrDefault(x => x.Name == "folio");


                ReconciliationDetailDTO = UIProcessManager.SelectAllReconciliationDetail();

                if (ReconciliationSummary != null)
                    ReconciliationDetailDTO = ReconciliationDetailDTO.Where(x => x.ReconSum == ReconciliationSummary.Id).ToList();

                ////CNETInfoReporter.Hide();
                return true;
            }
            catch (Exception ex)
            {
                ////CNETInfoReporter.Hide();
                SystemMessage.ShowModalInfoMessage("Error in initializing Group Checkout:: Detail: " + ex.Message, "ERROR");
                return false;
            }
        }

        private bool PopulateTransferBills()
        {
            DateTime? currentTime = UIProcessManager.GetServiceTime();
            if (currentTime == null)
            {
                return false;
            }
            Progress_Reporter.Show_Progress("Getting bill transfered registrations", "Please Wait...");

            if (regListVM != null)
                regListVM.Clear();
            regListVM = UIProcessManager.GetRegistrationViewModelData(null, null, CNETConstantes.CHECKED_IN_STATE, SelectedHotelcode);

            List<VwTransferBillViewDTO> transferDetailList = UIProcessManager.GetTransferBill(RegExtension.Id, null, false);
            if (transferDetailList != null && transferDetailList.Count > 0)
            {
                Progress_Reporter.Show_Progress("Preparing Registrations", "Please Wait...");
                List<GroupCheckoutVM> dtoList = new List<GroupCheckoutVM>();
                var slaveList = transferDetailList.GroupBy(t => t.Source).Select(t => t.First()).ToList();
                int counter = 0;
                foreach (var slave in slaveList)
                {
                    counter++;
                    // Progress_Reporter.Show_Progress("Preparing Registrations", "Please Wait...", counter, transferDetailList.Count);

                    RegistrationListVMDTO Group = regListVM.FirstOrDefault(x => x.Id.ToString() == slave.Source);
                    if (Group != null)
                    {

                        GroupCheckoutVM dto = new GroupCheckoutVM();

                        dto.Color = Group.Color;
                        dto.FoStatus = Group.lastState;
                        dto.RegId = Group.Id;
                        dto.RegNo = Group.Registration;
                        dto.Name = Group.Guest;
                        dto.RoomType = Group.RoomTypeDescription;
                        dto.Room = Group.Room;
                        dto.ArrivalDate = Group.Arrival;
                        dto.DepartureDate = Group.Departure;

                        if (dto.FoStatus == CNETConstantes.CHECKED_OUT_STATE)
                        {
                            dto.IsSelected = false;
                        }
                        else
                        {
                            dto.IsSelected = true;
                        }

                        if (currentTime.Value.Date == dto.DepartureDate.Date)
                        {
                            dto.isDateValid = true;
                        }
                        else
                        {
                            dto.isDateValid = false;
                        }

                        dto.isCardReturned = true;




                        //check card return;
                        if (_isDoorLockHosted && configReturnCard != null)
                        {
                            bool flag = Convert.ToBoolean(configReturnCard.CurrentValue);
                            if (flag)
                            {
                                if (!string.IsNullOrEmpty(RegExtension.Doorlock))
                                {
                                    dto.isCardReturned = false;

                                }

                            }

                        }

                        //Sum Room Charges
                        var drcList = transferDetailList.Where(t => t.Source == Group.Id.ToString() && t.Definition == CNETConstantes.DAILY_ROOM_CHARGE_VOUCHER).ToList();
                        if (drcList != null && drcList.Count > 0)
                        {
                            dto.RoomCharge = drcList.Sum(t => t.GrandTotal);
                        }

                        //Sum Extra Bills
                        var extraBills = transferDetailList.Where(t => t.Source == Group.Id.ToString() && (t.Definition == CNETConstantes.CASH_SALES || t.Definition == CNETConstantes.CREDITSALES)).ToList();
                        if (extraBills != null && extraBills.Count > 0)
                        {
                            dto.ExtraBills = extraBills.Sum(t => t.GrandTotal);
                        }

                        //Sum payments
                        var payments = transferDetailList.Where(t => t.Source == Group.Id.ToString() && (t.Definition == CNETConstantes.CASHRECIPT || t.Definition == CNETConstantes.PAID_OUT_VOUCHER || t.Definition == CNETConstantes.REFUND)).ToList();
                        if (payments != null && payments.Count > 0)
                        {

                            foreach (var paym in payments)
                            {
                                bool IsDebit = GetDebitInfo(paym.Definition);

                                if (IsDebit != null && IsDebit)
                                    dto.Payment = dto.Payment + paym.GrandTotal;
                                else
                                    dto.Payment = dto.Payment - paym.GrandTotal;
                            }

                        }

                        //Sum Discounts
                        var discounts = transferDetailList.Where(t => t.Source == Group.Id.ToString() && (t.Definition == CNETConstantes.CREDIT_NOTE_VOUCHER)).ToList();
                        if (discounts != null && discounts.Count > 0)
                        {
                            dto.Discount = discounts.Sum(t => t.GrandTotal);
                        }

                        //Transfer Details
                        List<TransferedBills> transferBillList = new List<TransferedBills>();
                        var transferLists = transferDetailList.Where(t => t.Source == Group.Id.ToString()).ToList();
                        foreach (var tran in transferLists)
                        {
                            TransferedBills transferDto = new TransferedBills();
                            transferDto.VoucherCode = tran.Code;
                            transferDto.IssuedDate = tran.IssuedDate;
                            transferDto.TransferedBy = tran.TransferedBy;
                            transferDto.TransferdDate = tran.IssuedDate;

                            bool IsDebit = GetDebitInfo(tran.Definition);

                            if (IsDebit)
                                transferDto.GrandTotal = Math.Round(tran.GrandTotal, 2);
                            else
                                transferDto.GrandTotal = -1 * Math.Round(tran.GrandTotal, 2);

                            var voDef = LocalBuffer.LocalBuffer.SystemConstantDTOBufferList.FirstOrDefault(v => v.Id == tran.Definition);
                            if (voDef != null)
                            {
                                transferDto.BillType = voDef.Description;
                            }

                            transferBillList.Add(transferDto);
                        }

                        dto.Total = dto.RoomCharge - dto.Discount + dto.Payment;
                        dto.TransferedBills = transferBillList;
                        dtoList.Add(dto);
                    }
                }


                Progress_Reporter.Show_Progress("Preparing Registrations is completed", "Please Wait...");
                gcGroupCheckout.DataSource = dtoList;
                gvGroupCheckout.RefreshData();
                gvGroupCheckout.FocusedRowHandle = 0;
                gvGroupCheckout.TopRowIndex = 0;

                if (dtoList.Count == 0)
                {
                    SwithControls(null);
                }
                else
                {
                    var firstRow = dtoList.ElementAt(0);
                    SwithControls(firstRow.FoStatus);
                }
                Progress_Reporter.Close_Progress();

                ////CNETInfoReporter.Hide();

            }

            return true;
        }

        private bool GetDebitInfo(int? definition)
        {
            bool isdebit = false;
            if (definition != null && ReconciliationDetailDTO != null && ReconciliationDetailDTO.Count > 0)
            {
                ReconciliationDetailDTO filter = ReconciliationDetailDTO.FirstOrDefault(x => x.VoucherDefinition == definition.Value);
                if (filter != null)
                {
                    isdebit = filter.IsDebit;
                }
            }
            return isdebit;
        }

        private void SwithControls(int? state)
        {
            if (state != null)
            {
                if (state == CNETConstantes.CHECKED_IN_STATE)
                {
                    bbiAmendDate.Enabled = true;
                    bbiGroupDateAmend.Enabled = true;
                    bbiCheckout.Enabled = true;
                    bbiReturnCard.Enabled = true;

                }
                else if (state == CNETConstantes.CHECKED_OUT_STATE)
                {
                    bbiAmendDate.Enabled = false;
                    bbiGroupDateAmend.Enabled = false;
                    bbiCheckout.Enabled = false;
                    bbiReturnCard.Enabled = false;
                }
            }
            else
            {
                bbiAmendDate.Enabled = false;
                bbiGroupDateAmend.Enabled = false;
                bbiCheckout.Enabled = false;
                bbiReturnCard.Enabled = false;
            }
        }

        private bool ValidateForCheckout(int regVoucher, ref List<string> logList)
        {
            //Check any daily room charge post for this registration
            List<VwDailyChargePostingViewDTO> dailyCharge = UIProcessManager.GetCheckOutDetailViewByVoucher(regVoucher, CNETConstantes.DAILY_ROOM_CHARGE_VOUCHER, null);

            if (dailyCharge == null)
            {
                var transferedBills = UIProcessManager.GetTransferBill(regVoucher, null, true);
                if (transferedBills == null || transferedBills.Count == 0)
                {
                    logList.Add(string.Format("{0} :: Unable to get room charge posting or room charge posting has not been made", regVoucher));
                    return false;
                }

                var filtered = transferedBills.Where(t => t.Definition == CNETConstantes.DAILY_ROOM_CHARGE_VOUCHER).ToList();
                if (filtered == null || filtered.Count == 0)
                {
                    logList.Add(string.Format("{0} :: Unable to get room charge posting or room charge posting has not been made", regVoucher));
                    return false;
                }
                else
                {
                    //check if this guest has atleast one room charge from activity table

                    //var activity = UIProcessManager.GetActivityByreference(regVoucher).FirstOrDefault(a => a.ActivityDefinition == adRegVoucherRoomCharge);
                    //if (activity == null)
                    //{
                    //    logList.Add(string.Format("{0} :: Unable to get room charge posting or room charge posting has not been made", regVoucher));
                    //    return false;
                    //}

                    return true;
                }


            }
            else
            {
                logList.Add(string.Format("{0} :: There are non-transfered bills!", regVoucher));
                return false;
            }
        }

        private bool GroupDateAmend(GroupCheckoutVM dto, DateTime currentDate, DateTime departureDate)
        {
            bool flag = true;

            try
            {

                //update Rate Adjustment
                RateAdjustmentDTO rateAdju = UIProcessManager.GetRateAdjustmentByvoucher(dto.RegId);
                if (rateAdju != null)
                {
                    rateAdju.EndDate = departureDate;
                    UIProcessManager.UpdateRateAdjustment(rateAdju);
                }


                //existed registration detail
                List<RegistrationDetailDTO> existedRegDetails = UIProcessManager.GetRegistrationDetailByvoucher(dto.RegId);
                if (existedRegDetails == null)
                {
                    return false;
                }

                var lastRegDetail = existedRegDetails.OrderByDescending(r => r.Date.Value.Date).FirstOrDefault();
                if (lastRegDetail == null) return false;

                RegistrationInfoDTO registration = new RegistrationInfoDTO();
                registration.RoomType = lastRegDetail.RoomType;
                registration.RTC = lastRegDetail.ActualRtc;

                var rDetail = UIProcessManager.GetRateCodeDetailById(lastRegDetail.RateCode.Value);
                if (rDetail != null)
                {
                    var rHeader = UIProcessManager.GetRateCodeHeaderById(rDetail.RateCodeHeader);
                    registration.RateCode = rHeader == null ? null : rHeader.Id;
                }

                registration.AdultCount = lastRegDetail.Adult.Value;
                registration.ChildCount = lastRegDetail.Child.Value;
                registration.ArrivalDate = dto.ArrivalDate;
                registration.DepartureDate = departureDate;
                registration.RoomCount = lastRegDetail.RoomCount.Value;


                List<GeneratedRegistrationDTO> Amenddata = UIProcessManager.AmendRegistrationDate(dto.FoStatus.Value, lastRegDetail, registration, dto.ArrivalDate.Date, departureDate.Date, SelectedHotelcode);


                if (Amenddata != null)
                {
                    // the next registrationDetails comes from grid
                    //_registrationDetails = new List<RegistrationDetail>();
                    foreach (GeneratedRegistrationDTO Gendata in Amenddata)
                    {

                        var reg = existedRegDetails.FirstOrDefault(r => r.Date.Value.Date == Gendata.registrationDetail.Date.Value.Date);
                        if (reg != null) continue;

                        Gendata.registrationDetail.Voucher = dto.RegId;

                        RegistrationDetailDTO savedRegistrationdetail = UIProcessManager.CreateRegistrationDetail(Gendata.registrationDetail);

                        if (savedRegistrationdetail != null)
                        {
                            foreach (PackagesToPostDTO packagetoPost in Gendata.packagesToPost)
                            {
                                packagetoPost.RegistrationDetail = savedRegistrationdetail.Id;

                                UIProcessManager.CreatePackagesToPost(packagetoPost);
                            }
                        }
                    }

                    var response = UIProcessManager.GetVoucherBufferById(dto.RegId);
                    if (response != null && response.Success)
                    {

                        VoucherBuffer voucherBuffer = response.Data;

                        if (voucherBuffer != null)
                        {
                            voucherBuffer.Voucher.StartDate = dto.ArrivalDate;
                            voucherBuffer.Voucher.EndDate = departureDate;

                            voucherBuffer.Activity = ActivityLogManager.SetupActivity(currentDate, _adCodeDateAmend.Value, CNETConstantes.PMS_Pointer, "Date is amended. Arrival: " + dto.ArrivalDate + ", Departure: " + departureDate);
                            if (voucherBuffer.TransactionReferencesBuffer != null && voucherBuffer.TransactionReferencesBuffer.Count > 0)
                                voucherBuffer.TransactionReferencesBuffer.ToList().ForEach(x => x.ReferencedActivity = null);

                            voucherBuffer.TransactionCurrencyBuffer = null;

                            ResponseModel<VoucherBuffer> updated = UIProcessManager.UpdateVoucherBuffer(voucherBuffer);
                            if (updated == null || !updated.Success)
                                SystemMessage.ShowModalInfoMessage("Amendment Failed !!" + Environment.NewLine + updated.Message, "Error");
                        }
                    }
                }
                else
                {
                    return false;
                }



            }
            catch (Exception ex)
            {
                flag = false;
            }

            return flag;
        }

        #endregion

        #region Event Handlers



        private void frmGroupCheckout_Load(object sender, EventArgs e)
        {
            PMSDataLogger.LogMessage("frmGroupCheckOut", "Group Checkout form opening.");
            if (!InitializeData())
            {
                this.Close();
            }
        }

        private void bbiCheckout_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                DialogResult dr = MessageBox.Show("Do you want to checkout the selected registrations?", "Group Checkout", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dr == DialogResult.No)
                {
                    return;
                }

                PMSDataLogger.LogMessage("frmGroupCheckOut", "Group Checkout the selected registrations.");

                logList.Clear();
                listBoxLog.DataSource = null;
                listBoxLog.Refresh();


                List<GroupCheckoutVM> dtoList = gvGroupCheckout.DataSource as List<GroupCheckoutVM>;
                if (dtoList == null && dtoList.Count == 0)
                {
                    PMSDataLogger.LogMessage("frmGroupCheckOut", "No registrations to checkout!");

                    SystemMessage.ShowModalInfoMessage("No registrations to checkout!", "ERROR");
                    return;
                }

                var selectedDtoList = dtoList.Where(r => r.IsSelected).ToList();
                if (selectedDtoList == null && selectedDtoList.Count == 0)
                {

                    PMSDataLogger.LogMessage("frmGroupCheckOut", "No Selected registrations to checkout!");
                    SystemMessage.ShowModalInfoMessage("No Selected Registrations to Checkout!", "ERROR");
                    return;
                }

                // Progress_Reporter.Show_Progress("Preparing to checkout", "Please Wait", 1, selectedDtoList.Count + 1);

                DateTime? currentTime = UIProcessManager.GetServiceTime();
                if (currentTime == null)
                {
                    ////CNETInfoReporter.Hide();
                    return;
                }

                int totalCount = selectedDtoList.Count;
                int currentCount = 1;
                foreach (var dto in selectedDtoList)
                {
                    currentCount = currentCount + 1;
                    // Progress_Reporter.Show_Progress(string.Format("{0} of {1}", currentCount, totalCount), "Please Wait", currentCount, totalCount);



                    if (dto.FoStatus == CNETConstantes.CHECKED_OUT_STATE)
                    {
                        logList.Add(string.Format("{0} :: is already checked out!", dto.RegNo));
                        continue;
                    }

                    if (!dto.isDateValid)
                    {
                        logList.Add(string.Format("{0} :: Departure date is invalid", dto.RegNo));
                        continue;
                    }

                    if (!dto.isCardReturned)
                    {
                        logList.Add(string.Format("{0} :: Key Card is not returned", dto.RegNo));
                        continue;
                    }

                    if (!ValidateForCheckout(dto.RegId, ref logList))
                    {
                        continue;
                    }
                    var response = UIProcessManager.GetVoucherBufferById(dto.RegId);

                    if (response == null || !response.Success)
                    {
                        logList.Add(string.Format("{0} :: Reg. Voucher is not found", dto.RegNo));
                        continue;
                    }

                    VoucherBuffer voucher = response.Data;

                    if (voucher == null)
                    {
                        logList.Add(string.Format("{0} :: Reg. Voucher is not found", dto.RegNo));
                        continue;
                    }
                    voucher.Voucher.Extension1 = null;
                    PMSDataLogger.LogMessage("frmGroupCheckOut", "Group Checkout " + voucher.Voucher.Code + " Registration LastObjectState Changed.");
                    voucher.Voucher.LastState = CNETConstantes.CHECKED_OUT_STATE;
                    PMSDataLogger.LogMessage("frmGroupCheckOut", "Group Checkout Updating" + voucher.Voucher.Code + " Registration LastObjectState Changed.");

                    voucher.Activity = ActivityLogManager.SetupActivity(currentTime.Value, adCheckout.Value, CNETConstantes.PMS_Pointer, "Grouped Checked Out");
                    if (voucher.TransactionReferencesBuffer != null && voucher.TransactionReferencesBuffer.Count > 0)
                        voucher.TransactionReferencesBuffer.ToList().ForEach(x => x.ReferencedActivity = null);


                    voucher.TransactionCurrencyBuffer = null;

                    PMSDataLogger.LogMessage("frmGroupCheckOut", "Group Checkout Updating " + voucher.Voucher.Code + " ...");
                    if (UIProcessManager.UpdateVoucherBuffer(voucher) != null)
                    {
                        PMSDataLogger.LogMessage("frmGroupCheckOut", "Group Checkout Updating " + voucher.Voucher.Code + " Registration Done.");

                        PMSDataLogger.LogMessage("frmGroupCheckOut", " Update Room to Dirty State");
                        //Update Room to Dirty State
                        List<RoomTypeDTO> pseudoRoomList = UIProcessManager.GetRoomTypeByispseudoRoomType(true);
                        List<int> pseudoRooms = new List<int>();
                        if (pseudoRoomList != null)
                            pseudoRooms = pseudoRoomList.Select(p => p.Id).ToList();
                        RoomDetailDTO rd = UIProcessManager.GetRoomDetailById(RegExtension.RoomCode.Value);
                        if (rd != null)
                        {

                            //Get Activity Definition For Dirty
                            ActivityDefinitionDTO adDirty = UIProcessManager.GetActivityDefinitionBycomponetanddescription(CNETConstantes.PMS_Pointer, CNETConstantes.Dirty).FirstOrDefault();
                            if (adDirty != null)
                            {
                                if (pseudoRooms != null && !pseudoRooms.Contains(rd.RoomType))
                                {
                                    rd.LastState = adDirty.Id;
                                    UIProcessManager.UpdateRoomDetail(rd);
                                    logActiviy(rd);
                                }
                            }
                        }
                    }
                    else
                    {
                        PMSDataLogger.LogMessage("frmGroupCheckOut", "Group Checkout Updating " + voucher.Voucher.Code + " Registration Fail.");
                        logList.Add(string.Format("{0} :: checkout is not successfull", dto.RegNo));
                        continue;
                    }
                }


                listBoxLog.DataSource = logList;
                listBoxLog.Refresh();
                PopulateTransferBills();

                ////CNETInfoReporter.Hide();
            }
            catch (Exception ex)
            {
                PMSDataLogger.LogMessage("frmGroupCheckOut", "Group Checkout Checkout Error", true, ex);
                ////CNETInfoReporter.Hide();
                SystemMessage.ShowModalInfoMessage("Error in group checkout. DETAIL:: " + ex.Message, "ERROR");
            }
        }
        private void logActiviy(RoomDetailDTO rmd)
        {
            try
            {
                ActivityDTO act = new ActivityDTO();
                act.ActivityDefinition = rmd.LastState.Value;
                act.TimeStamp = currentTime.Value.ToLocalTime();
                act.Year = currentTime.Value.Year;
                act.Month = currentTime.Value.Month;
                act.Day = currentTime.Value.Day;
                act.Reference = rmd.Id;
                act.Platform = "1";
                act.User = LocalBuffer.LocalBuffer.CurrentLoggedInUser.Id;
                act.Device = LocalBuffer.LocalBuffer.CurrentDevice.Id;
                act.ConsigneeUnit = RegExtension.ConsigneeUnit;
                act.Remark = "Event Checkout";
                UIProcessManager.CreateActivity(act);
            }
            catch (Exception e) { }
        }
        private void bbiAmendDate_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            GroupCheckoutVM dto = gvGroupCheckout.GetFocusedRow() as GroupCheckoutVM;
            if (dto == null)
            {
                SystemMessage.ShowModalInfoMessage("Please select a registration first!", "ERROR");
                return;
            }

            var RegExt = regListVM.FirstOrDefault(r => r.Registration == dto.RegNo);
            if (RegExt == null)
            {
                SystemMessage.ShowModalInfoMessage("Unable to find the registration!", "ERROR");
                return;
            }

            var _frmRegistrationDateAmendement = new frmDateAmendment();
            _frmRegistrationDateAmendement.SelectedHotelcode = SelectedHotelcode;
            _frmRegistrationDateAmendement.RegistrationExt = RegExt;
            if (_frmRegistrationDateAmendement.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                // Progress_Reporter.Show_Progress("Refreshing Data", "Please Wait...", 1, 2);

                if (regListVM != null)
                    regListVM.Clear();
                regListVM = UIProcessManager.GetRegistrationViewModelData(null, null, CNETConstantes.CHECKED_IN_STATE, SelectedHotelcode);

                // Progress_Reporter.Show_Progress("Refreshing Group Checkouts", "Please Wait...", 2, 2);
                PopulateTransferBills();

                ////CNETInfoReporter.Hide();
            }

        }

        private void bbiRefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                PopulateTransferBills();

            }
            catch (Exception ex)
            {
                SystemMessage.ShowModalInfoMessage("Error in loading group checkout. DETAIL:: " + ex.Message, "ERROR");
            }
        }

        private void bbiFolio_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            GroupCheckoutVM dto = gvGroupCheckout.GetFocusedRow() as GroupCheckoutVM;
            if (dto == null)
            {
                SystemMessage.ShowModalInfoMessage("Please select a registration first!", "ERROR");
                return;
            }

            var RegExt = regListVM.FirstOrDefault(r => r.Registration == dto.RegNo);
            if (RegExt == null)
            {
                SystemMessage.ShowModalInfoMessage("Unable to find the registration!", "ERROR");
                return;
            }

            //show guest ledger form
            frmFolio _frmFolio = new frmFolio();
            _frmFolio.RegistrationExt = RegExt;
            _frmFolio.ShowDialog(this);

        }

        private void bbiPrint_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                DateTime? currentTimeDate = UIProcessManager.GetServiceTime();
                if (currentTimeDate == null) return;

                ReportGenerator rg = new ReportGenerator();
                rg.GenerateGridReport(gcGroupCheckout, "Group Checkout List", currentTimeDate.Value.ToShortDateString());


            }
            catch (Exception ex)
            {
                SystemMessage.ShowModalInfoMessage("Error in printing grid. DETAIL:: " + ex.Message, "ERROR");
            }
        }

        private void bbiClose_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void gvGroupCheckout_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            GridView view = sender as GridView;

            var dto = view.GetRow(e.FocusedRowHandle) as GroupCheckoutVM;
            if (dto == null) return;

            SwithControls(dto.FoStatus);

            view.Appearance.FocusedRow.BackColor = ColorTranslator.FromHtml("SkyBlue");
            view.Appearance.FocusedRow.ForeColor = ColorTranslator.FromHtml(dto.Color);


            view.Appearance.FocusedCell.BackColor = ColorTranslator.FromHtml("SkyBlue");
            view.Appearance.FocusedCell.ForeColor = ColorTranslator.FromHtml(dto.Color);

            view.Appearance.SelectedRow.BackColor = ColorTranslator.FromHtml("SkyBlue");
            view.Appearance.SelectedRow.ForeColor = ColorTranslator.FromHtml(dto.Color);
        }

        private void gvGroupCheckout_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            GridView view = sender as GridView;

            var dto = view.GetRow(e.RowHandle) as GroupCheckoutVM;
            if (dto == null) return;

            view.Appearance.FocusedRow.ForeColor = ColorTranslator.FromHtml(dto.Color);
            view.Appearance.FocusedCell.ForeColor = ColorTranslator.FromHtml(dto.Color);
            view.Appearance.SelectedRow.ForeColor = ColorTranslator.FromHtml(dto.Color);

            if (e.Column.Caption == "Departure")
            {
                if (!dto.isDateValid)
                    e.Appearance.BackColor = ColorTranslator.FromHtml("red");
            }

            e.Appearance.ForeColor = ColorTranslator.FromHtml(dto.Color);

            if (!dto.isCardReturned)
            {
                e.Appearance.BackColor = ColorTranslator.FromHtml("Info");
            }
        }

        private void gvGroupCheckout_RowStyle(object sender, RowStyleEventArgs e)
        {
            //GridView view = sender as GridView;

            //var dto = view.GetRow(e.RowHandle) as GroupCheckoutVM;
            //if (dto == null) return;

            //view.Appearance.FocusedRow.ForeColor = ColorTranslator.FromHtml(dto.Color);
            //view.Appearance.FocusedCell.ForeColor = ColorTranslator.FromHtml(dto.Color);
            //view.Appearance.SelectedRow.ForeColor = ColorTranslator.FromHtml(dto.Color);

            //e.Appearance.ForeColor = ColorTranslator.FromHtml(dto.Color);

            //if (!dto.isCardReturned)
            //{
            //    e.Appearance.BackColor = ColorTranslator.FromHtml("Info");
            //}
        }

        private void ceCheckAll_CheckedChanged(object sender, EventArgs e)
        {
            CheckEdit view = sender as CheckEdit;
            List<GroupCheckoutVM> dtoList = gvGroupCheckout.DataSource as List<GroupCheckoutVM>;
            if (dtoList != null && dtoList.Count > 0)
            {
                foreach (var dto in dtoList)
                {
                    dto.IsSelected = view.Checked;
                }

                gvGroupCheckout.RefreshData();
            }


        }

        private void gvGroupCheckout_MasterRowExpanded(object sender, CustomMasterRowEventArgs e)
        {
            GridView detailView = sender as GridView;
            GridView childView = detailView.GetDetailView(e.RowHandle, 0) as GridView;
            if (childView != null)
            {
                childView.OptionsBehavior.Editable = false;
            }
        }

        private void bbiReturnCard_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            GroupCheckoutVM dto = gvGroupCheckout.GetFocusedRow() as GroupCheckoutVM;
            if (dto == null)
            {
                SystemMessage.ShowModalInfoMessage("Please select a registration first!", "ERROR");
                return;
            }

            var RegExt = regListVM.FirstOrDefault(r => r.Registration == dto.RegNo);
            if (RegExt == null)
            {
                SystemMessage.ShowModalInfoMessage("Unable to find the registration!", "ERROR");
                return;
            }
            frmDoorLock frmDoorLock = new frmDoorLock();
            frmDoorLock.RegExt = RegExt;
            frmDoorLock.ShowDialog();
        }

        private void bbiMasterFolio_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //show guest ledger form
            frmFolio _frmFolio = new frmFolio();
            _frmFolio.RegistrationExt = RegExtension;
            _frmFolio.ShowDialog(this);
        }

        private void ceExpandAll_CheckedChanged(object sender, EventArgs e)
        {
            CheckEdit view = sender as CheckEdit;
            int dataRowCount = gvGroupCheckout.DataRowCount;
            for (int rHandle = 0; rHandle < dataRowCount; rHandle++)
                gvGroupCheckout.SetMasterRowExpanded(rHandle, view.Checked);
        }

        private void RepoCheckEdit_EditValueChanged(object sender, EventArgs e)
        {
            gvGroupCheckout.FocusedColumn = gvGroupCheckout.Columns[0];
            gvGroupCheckout.RefreshData();
        }


        #endregion

        private void beiGroupDateAmend_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DialogResult dr = MessageBox.Show("Do you want to amend the selected registrations' Departure Date?", "Group Date Amendment", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dr == DialogResult.No)
            {
                return;
            }

            List<GroupCheckoutVM> dtoList = gvGroupCheckout.DataSource as List<GroupCheckoutVM>;
            if (dtoList == null && dtoList.Count == 0)
            {

                SystemMessage.ShowModalInfoMessage("No registrations to amend the date!", "ERROR");
                return;
            }

            var selectedDtoList = dtoList.Where(r => r.IsSelected).ToList();
            if (selectedDtoList == null && selectedDtoList.Count == 0)
            {

                SystemMessage.ShowModalInfoMessage("No Selected Registrations to amend the date!", "ERROR");
                return;
            }

            if (beiDepartureDate.EditValue == null || string.IsNullOrEmpty(beiDepartureDate.EditValue.ToString()))
            {
                SystemMessage.ShowModalInfoMessage("Please Select Departure Date First!", "ERROR");
                return;
            }

            DateTime deptDate = DateTime.Parse(beiDepartureDate.EditValue.ToString());
            int count = 0;
            int totalCount = selectedDtoList.Count;


            DateTime? currentTime = UIProcessManager.GetServiceTime();
            if (currentTime == null)
            {
                return;
            }
            DateTime CurrentTime = currentTime.Value;

            foreach (var dto in selectedDtoList)
            {
                count = count + 1;
                // Progress_Reporter.Show_Progress("Saving Date Amendment", string.Format("{0} of {1}", count , totalCount), count, totalCount);

                GroupDateAmend(dto, CurrentTime, deptDate);
            }

            ////CNETInfoReporter.Hide();

            SystemMessage.ShowModalInfoMessage("Group Date Amendment is completed!", "MESSAGE");


            // Progress_Reporter.Show_Progress("Refreshing Data", "Please Wait...", 1, 2);

            if (regListVM != null)
                regListVM.Clear();
            regListVM = UIProcessManager.GetRegistrationViewModelData(null, null, CNETConstantes.CHECKED_IN_STATE, SelectedHotelcode);

            // Progress_Reporter.Show_Progress("Refreshing Group Checkouts", "Please Wait...", 2, 2);
            PopulateTransferBills();

            ////CNETInfoReporter.Hide();

        }

        public static int SelectedHotelcode { get; set; }
    }
}
