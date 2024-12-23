using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

using DevExpress.XtraEditors;
using CNET.FrontOffice_V._7;

using CNET.ERP.Client.Common.UI;
using CNET.FrontOffice_V._7.Forms.Vouchers;
using CNET.FrontOffice_V._7.Forms.Non_Navigatable_Modals;
using DevExpress.XtraEditors.Controls;
using CNET_V7_Domain.Misc.PmsDTO;
using CNET_V7_Domain.Domain.TransactionSchema;
using CNET_V7_Domain.Domain.SettingSchema;
using ProcessManager;
using CNET.FrontOffice_V._7.PMS.Common_Classes;
using CNET_V7_Domain.Domain.ViewSchema;
using CNET_V7_Domain.Domain.CommonSchema;
using CNET.FP.Tool;
using CNET.POS.Settings;
using CNET.FrontOffice_V._7.Night_Audit;
using CNET_V7_Domain;
using DevExpress.XtraVerticalGrid;
using CNET_V7_Domain.Domain.ConsigneeSchema;
using CNET_V7_Domain.Domain.PmsSchema;
using CNET_V7_Domain.Misc;

namespace CNET.FrontOffice_V._7.Forms.State_Change
{
    public partial class frmCheckOut : UILogicBase
    {

        private int? adCode;
        private GuestLedgerDTO gLedger;
        //private List<vw_OtherConsigneeView> allOtherConsignees;

        private int? _currentCompanyCode = null;

        /** Properties **/
        public bool checkOutWithOutReceipt { get; set; }
        public bool IsReinstate { get; set; }
        public bool IsWithZeroBalance { get; set; }
        private int _window = -1;
        public int Window
        {
            get
            {
                return _window;
            }
            set
            {
                _window = value;
            }
        }

        public bool IsFromNightAudit { get; set; }

        private bool isKeyReturnAuthorized = false;

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

        VoucherBuffer voucherBuffer;
        internal VoucherBuffer VoucherBuffer
        {
            get { return voucherBuffer; }
            set
            {
                voucherBuffer = value;
            }
        }
        RegistrationListVMDTO regExtension;
        internal RegistrationListVMDTO RegExtension
        {
            get { return regExtension; }
            set
            {
                try
                {
                    regExtension = value;


                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show("Error in preparing checkout", "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    ////CNETInfoReporter.Hide();
                    return;
                }
            }
        }

        /**************** CONSTRUCTOR **********************/
        public frmCheckOut()
        {
            InitializeComponent();

            PMSDataLogger.LogMessage("frmCheckOut", "Open CheckOut Form.");
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;

            //Windows 
            lukCompany.Properties.Columns.Add(new LookUpColumnInfo("Code", "Code"));
            lukCompany.Properties.Columns.Add(new LookUpColumnInfo("FirstName", "Name"));
            lukCompany.Properties.Columns.Add(new LookUpColumnInfo("Tin", "TIN"));
            lukCompany.Properties.DisplayMember = "FirstName";
            lukCompany.Properties.ValueMember = "Id";
        }

        #region Helper Methods

        private bool InitializeData()
        {
            try
            {



                if (RegExtension == null) return false;

                // Progress_Reporter.Show_Progress("Initializing data");

                //check workflow
                ActivityDefinitionDTO workFlow = UIProcessManager.GetActivityDefinitionBydescriptionandreference(CNETConstantes.LU_ACTIVITY_DEFINATION_CheckOut, CNETConstantes.REGISTRATION_VOUCHER).FirstOrDefault();

                if (workFlow != null)
                {

                    adCode = workFlow.Id;
                }
                else
                {
                    ////CNETInfoReporter.Hide();
                    SystemMessage.ShowModalInfoMessage("Please define workflow of CHECK-OUT for Registration Voucher ", "ERROR");
                    return false;
                }

                //Check Activity Previlage
                var userRoleMapper = LocalBuffer.LocalBuffer.UserRoleMapperBufferList.FirstOrDefault(r => r.User == LocalBuffer.LocalBuffer.CurrentLoggedInUser.Id);
                if (userRoleMapper != null)
                {
                    var roleActivity = UIProcessManager.GetRoleActivityByactivityDefinition(adCode.Value).FirstOrDefault(r => r.Role == userRoleMapper.Role && r.NeedsPassCode);
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


                teRegNo.Text = RegExtension.Registration;
                if (RegExtension.Guest != null)
                {
                    int index = RegExtension.Guest.IndexOf("(", StringComparison.Ordinal);
                    teGuest.Text = index > 0 ? RegExtension.Guest.Substring(0, index) : RegExtension.Guest;
                }
                teRoom.Text = RegExtension.Room;
                tePaymentType.Text = RegExtension.PaymentDesc;
                teArrival.Text = RegExtension.Arrival.Date.ToString();
                teDeparture.Text = RegExtension.Departure.Date.ToString();

                if (!CheckLateCheckout())
                {
                    ////CNETInfoReporter.Hide();
                    return false;
                }

                PopulateGuestLedger(RegExtension);

                if (RegExtension.CompanyId != null)
                {
                    layoutStatus.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;

                    ConsigneeDTO CompanyDTO = LocalBuffer.LocalBuffer.ConsigneeViewBufferList.FirstOrDefault(x => x.Id == RegExtension.CompanyId);

                    lukCompany.Properties.DataSource = new List<ConsigneeDTO>() { CompanyDTO };

                    if (CompanyDTO != null)
                        lukCompany.EditValue = CompanyDTO.Id;
                }
                else
                    layoutStatus.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;




                //Agent
                if (RegExtension.AgentId != null)
                {
                    ConsigneeDTO agent = LocalBuffer.LocalBuffer.ConsigneeViewBufferList.FirstOrDefault(x => x.Id == RegExtension.AgentId);
                    if (agent != null)
                    {
                        teAgent.Text = agent == null ? "" : agent.FirstName;
                    }
                }


                //Source
                if (RegExtension.SourceId != null)
                {
                    ConsigneeDTO sourceDTO = LocalBuffer.LocalBuffer.ConsigneeViewBufferList.FirstOrDefault(x => x.Id == RegExtension.SourceId);
                    if (sourceDTO != null)
                    {
                        teSource.Text = sourceDTO == null ? "" : sourceDTO.FirstName;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                ////CNETInfoReporter.Hide();
                XtraMessageBox.Show("Error in initializaing data.Detail: " + ex.Message, "CNET_v2016", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private bool CheckRemainingBalance()
        {

            if (gLedger == null)
            {
                SystemMessage.ShowModalInfoMessage("Unable to get guest ledger information", "ERROR");
                return false;
            }

            if (gLedger.RemainingBalance < 1 && gLedger.RemainingBalance > -1)
            {
                return true;
            }
            else if (gLedger.RemainingBalance < -1)
            {
                SystemMessage.ShowModalInfoMessage("You can not check out this guest since (s)he has made overpayment!!!",
                    "ERROR");

                return false;
            }
            else
            {
                frmPMSVouchercs frmCashReceipt = new frmPMSVouchercs(CNETConstantes.CASHRECIPT);
                frmCashReceipt.RegistrationExt = RegExtension;
                frmCashReceipt.CurrentWindow = Window;
                if (Window > 0)
                    frmCashReceipt.IsFromSplitWindow = true;

                if (frmCashReceipt.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    PopulateGuestLedger(RegExtension);
                    if (gLedger == null)
                    {
                        SystemMessage.ShowModalInfoMessage("Unable to get guest ledger information", "ERROR");
                        return false;
                    }

                    if (gLedger.RemainingBalance < 1 && gLedger.RemainingBalance > -1)
                    {
                        return true;
                    }
                    else if (gLedger.RemainingBalance < -1)
                    {
                        SystemMessage.ShowModalInfoMessage("You can not check out this guest since (s)he has made overpayment!!!",
                            "ERROR");

                        return false;
                    }
                    else
                    {
                        SystemMessage.ShowModalInfoMessage("You can not check out this guest with this remaining balance !!!",
                            "ERROR");
                        return false;
                    }

                }
                else
                {
                    PopulateGuestLedger(RegExtension);
                    return false;
                }
            }

            //return true;
        }

        private bool CheckLateCheckout()
        {
            try
            {
                var regPrev = UIProcessManager.GetRegistrationPrivllegeByvoucher(RegExtension.Id);
                if (regPrev == null)
                {
                    SystemMessage.ShowModalInfoMessage("Unable to get registration previlege!", "ERROR");
                    return false;
                }

                isKeyReturnAuthorized = regPrev.AuthorizeKeyReturn == null ? false : regPrev.AuthorizeKeyReturn;

                if (regPrev.AllowLatecheckout) return true;


                var configUseLateCheckout = LocalBuffer.LocalBuffer.ConfigurationBufferList.FirstOrDefault(c => c.Reference == CNETConstantes.PMS_Pointer.ToString() && c.Attribute == CNETConstantes.PMS_SETTING_UseLateCheckout);
                var configLateTime = LocalBuffer.LocalBuffer.ConfigurationBufferList.FirstOrDefault(c => c.Reference == CNETConstantes.PMS_Pointer.ToString() && c.Attribute == CNETConstantes.PMS_SETTING_LateCheckoutTime);
                var configPunshment = LocalBuffer.LocalBuffer.ConfigurationBufferList.FirstOrDefault(c => c.Reference == CNETConstantes.PMS_Pointer.ToString() && c.Attribute == CNETConstantes.PMS_SETTING_LateCheckoutRequiredPayment);

                if (configUseLateCheckout == null || configLateTime == null || configPunshment == null)
                {
                    XtraMessageBox.Show("System has not found late check-out time setting or late check-out required payment setting!", "Late Check-out", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return true;
                }

                if (!Convert.ToBoolean(configUseLateCheckout.CurrentValue))
                {
                    return true;
                }

                DateTime? currentTime = UIProcessManager.GetServiceTime();
                if (currentTime == null) return false;

                DateTime lateCheckoutTime = DateTime.ParseExact(configLateTime.CurrentValue, "HH:mm:ss", CultureInfo.InvariantCulture);
                if (currentTime.Value > lateCheckoutTime.AddMinutes(15))
                {
                    int punshment = Convert.ToInt32(configPunshment.CurrentValue);
                    if (punshment > 100 || punshment <= 0)
                    {
                        SystemMessage.ShowModalInfoMessage("Invalid Late Check-out Required Payment Value! Value must be between 0 and 100 %", "ERROR");
                        return false;
                    }


                    DialogResult dr = XtraMessageBox.Show("This Check-out is Late! Do you want to charge " + configPunshment.CurrentValue + " % of the room charge?",
                        "Late Check-out", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dr == System.Windows.Forms.DialogResult.No)
                    {
                        return false;
                    }
                    else
                    {

                        bool flag = false;

                        // Progress_Reporter.Show_Progress("posting room charge to the guest", "Please Wait...");
                        #region Post Room Charge

                        //get applicable tax
                        int? accArticle = null;
                        var rateCodeHeader = UIProcessManager.GetRateCodeHeaderById(RegExtension.RateCodeHeader.Value);
                        if (rateCodeHeader != null)
                        {
                            accArticle = rateCodeHeader.Article;
                        }
                        else
                        {
                            SystemMessage.ShowModalInfoMessage("Unable to find registration's rate", "ERROR");
                            return false;
                        }

                        if (accArticle == null)
                        {
                            SystemMessage.ShowModalInfoMessage("Unable to find rate's article", "ERROR");
                            return false;
                        }

                        TaxDTO tax = CommonLogics.GetApplicableTax(RegExtension.Id, CNETConstantes.DAILY_ROOM_CHARGE_VOUCHER, RegExtension.GuestId, accArticle);
                        if (!string.IsNullOrEmpty(tax.Remark))
                        {
                            SystemMessage.ShowModalInfoMessage(tax.Remark, "ERROR");
                            return false;
                        }
                        //get last registration date
                        DateTime lastDate = (RegExtension.Arrival.Date == RegExtension.Departure.Date) ? RegExtension.Arrival.Date : (RegExtension.Departure.Date.Subtract(TimeSpan.FromDays(1)));
                        DailyRoomChargeDTO dailyRoomCharge = UIProcessManager.GetDailyRoomChargePostingByRegistration(RegExtension.Id, lastDate.Date, CNETConstantes.REGISTRATION_VOUCHER, CNETConstantes.CHECKED_IN_STATE, tax, punshment, null);
                        if (dailyRoomCharge == null)
                        {
                            SystemMessage.ShowModalInfoMessage("Unable to populate room charge to post!", "ERROR");
                            return false;
                        }

                        CommonLogics.PostRoomCharge(new List<DailyRoomChargeDTO>() { dailyRoomCharge }, currentTime.Value, LocalBuffer.LocalBuffer.CurrentDevice, RegExtension.ConsigneeUnit.Value, true);

                        #endregion

                        return true;
                    }


                }
                else
                {
                    return true;
                }


            }
            catch (Exception ex)
            {
                SystemMessage.ShowModalInfoMessage("Error in checking late-checkout. Detail:: " + ex.Message, "ERROR");
                return false;
            }
        }

        private void PopulateGuestLedger(RegistrationListVMDTO value)
        {
            if (Window > 0)
            {
                var luk = LocalBuffer.LocalBuffer.SystemConstantDTOBufferList.FirstOrDefault(l => l.Category == CNETConstantes.SPLIT_WINDOWS && l.Value == Window.ToString());
                if (luk != null)
                {
                    gLedger = UIProcessManager.GetGuestLedger(value.Id, value.Arrival.Date, value.Departure.Date, value.Room, luk.Id);
                }
            }
            else
            {
                gLedger = UIProcessManager.GetGuestLedger(value.Id, value.Arrival.Date, value.Departure.Date, value.Room, null);
            }
            if (gLedger != null)
            {
                teTotalBill.Text = gLedger.TotalCredit.ToString();
                tePaid.Text = gLedger.TotalPaid.ToString();
                teRemaing.Text = gLedger.RemainingBalanceFormated;
            }
        }
        #endregion



        #region Event Handlers

        private void bbCheckOut_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            // Progress_Reporter.Show_Progress("Please Wait...");

            PMSDataLogger.LogMessage("frmCheckOut", "Check out button Clicked.");
            try
            {
                if (!isKeyReturnAuthorized)
                {
                    var configReturnCard = LocalBuffer.LocalBuffer.ConfigurationBufferList.FirstOrDefault(c => c.Reference == CNETConstantes.PMS_Pointer.ToString() && c.Attribute == CNETConstantes.PMS_SETTING_EnforceCheckoutCardReturn);
                    if (configReturnCard != null)
                    {
                        bool flag = Convert.ToBoolean(configReturnCard.CurrentValue);
                        if (flag)
                        {

                            //var voExtDoorLock = LocalBuffer.LocalBuffer.VoucherExtensionBufferList.FirstOrDefault(v => v.Type == CNETConstantes.VOUCHER_EXTENTION_DEFINITION_DOOR_LOCK && v.VoucherDefinition == CNETConstantes.REGISTRATION_VOUCHER);
                            //if (voExtDoorLock != null)
                            //{
                            if (!string.IsNullOrEmpty(RegExtension.Doorlock))
                            {
                                ////CNETInfoReporter.Hide();
                                SystemMessage.ShowModalInfoMessage("Please return guest's door lock card first!", "ERROR");
                                return;
                            }
                            // }

                        }

                    }
                }
                if (checkOutWithOutReceipt || IsWithZeroBalance)
                {
                    if (regExtension.PaymentDesc != "Direct Bill" && !IsWithZeroBalance)
                    {
                        if (!CheckRemainingBalance())
                        {
                            ////CNETInfoReporter.Hide();
                            return;

                        }
                    }
                    DialogResult dr = MessageBox.Show(@"Do you want to Check Out this guest?",
                                @"State Change Conformation", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);
                    DateTime? CurrentTime = UIProcessManager.GetServiceTime();
                    if (dr == DialogResult.Yes)
                    {

                        PMSDataLogger.LogMessage("frmCheckOut", "Updating CHECKED OUT STATE without Receipt");
                        VoucherBuffer.Voucher.Extension1 = null;
                        voucherBuffer.Voucher.LastState = CNETConstantes.CHECKED_OUT_STATE;

                        PMSDataLogger.LogMessage("frmCheckOut", "Setup Activity For Checkout");
                        voucherBuffer.Activity = ActivityLogManager.SetupActivity(CurrentTime.Value, adCode.Value, CNETConstantes.PMS_Pointer, "Checked Out");
                        if (voucherBuffer.TransactionReferencesBuffer != null && voucherBuffer.TransactionReferencesBuffer.Count > 0)
                            voucherBuffer.TransactionReferencesBuffer.ToList().ForEach(x => x.ReferencedActivity = null);



                        voucherBuffer.TransactionCurrencyBuffer = null;
                        ResponseModel<VoucherBuffer> isVoucherUpdated = UIProcessManager.UpdateVoucherBuffer(voucherBuffer);

                        //if(UIProcessManager.UpdateVoucherBuffer(voucherBuffer) != null)
                        if (isVoucherUpdated != null && isVoucherUpdated.Success)
                        
                        {
                            PMSDataLogger.LogMessage("frmCheckOut", voucherBuffer.Voucher.Code + " Voucher CHECKED OUT STATE without Receipt Done");

                            PMSDataLogger.LogMessage("frmCheckOut"," Update Room to Dirty State");
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

                            DialogResult = System.Windows.Forms.DialogResult.OK;
                            SystemMessage.ShowModalInfoMessage("State changed successfully!", "MESSAGE");

                        }
                        else
                        {
                            PMSDataLogger.LogMessage("frmCheckOut", voucherBuffer.Voucher.Code + " Voucher Update CHECKED OUT STATE without Receipt Fail !!");
                            PMSDataLogger.LogMessage("frmFrontOfficePOS", "Voucher Update Fail !!" + isVoucherUpdated.Message);
                            SystemMessage.ShowModalInfoMessage("NOT SUCCESSFUL!", "ERROR");
                        }

                    }
                    else
                    {
                        PMSDataLogger.LogMessage("frmCheckOut", "state change cancelled!");
                        SystemMessage.ShowModalInfoMessage("state change cancelled!", "MESSAGE");
                    }
                }
                else
                {
                    //FiscalPrinters fpSetup = new FiscalPrinters();
                    ////bool isFpOpened = fpSetup.OpenFisicalPrinter(RegExtension.Consignee);
                    //if (POS_Settings.IsError)
                    //{
                    //    ////CNETInfoReporter.Hide();
                    //    XtraMessageBox.Show("Unable to connect with fisical printer", "CNET_2016", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //    return;
                    //}

                    //if (!fpSetup.CheckPrinterStatusForPrinting())
                    //{
                    //    XtraMessageBox.Show("Printer is Out Of Order", "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //   ////CNETInfoReporter.Hide();
                    //    FiscalPrinters.DisposeInstance();
                    //    return;
                    //}
                    ////CNETInfoReporter.Hide();

                    // if payment is DIRECT BILL

                    if (regExtension.PaymentDesc == "Direct Bill")
                    {
                        PMSDataLogger.LogMessage("frmCheckOut", "CHECKED OUT Direct Bill");
                        DialogResult dr = MessageBox.Show(@"Do you want to Check Out this guest?", @"State Change Conformation", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);

                        if (dr == DialogResult.Yes)
                        {

                            PMSDataLogger.LogMessage("frmCheckOut", "CHECKING OUT Direct Bill");
                            frmFrontOfficePOS frmFrontOffice = new frmFrontOfficePOS(true);
                            frmFrontOffice.isCheckOut = true;
                            frmFrontOffice.RegExtension = RegExtension;
                            frmFrontOffice.VoucherBuffer = VoucherBuffer;
                            frmFrontOffice.Window = Window;
                            frmFrontOffice.IsReinstate = IsReinstate;
                            DialogResult dResult = frmFrontOffice.ShowDialog();
                            if (dResult == DialogResult.OK)
                            {
                                PMSDataLogger.LogMessage("frmCheckOut", "frmFrontOfficePOS DialogResult OK");
                                DateTime? CurrentTime = UIProcessManager.GetServiceTime();
                                if (CurrentTime != null)
                                {
                                    PMSDataLogger.LogMessage("frmCheckOut", "Setup Activity For Checkout Window =  " + Window);
                                    ActivityDTO activity = ActivityLogManager.SetupActivity(CurrentTime.Value, adCode.Value, CNETConstantes.PMS_Pointer, "Checkout -> Window = " + Window);
                                    activity.Reference = voucherBuffer.Voucher.Id;
                                    UIProcessManager.CreateActivity(activity);

                                    bbCheckOut.Enabled = false;
                                }
                                DialogResult = System.Windows.Forms.DialogResult.OK;

                                //show guest ledger form
                                frmFolio _frmFolio = new frmFolio();
                                RegistrationListVMDTO regExten = RegExtension;
                                if (regExten != null)
                                {
                                    _frmFolio.RegistrationExt = regExten;
                                    _frmFolio.ShowDialog(this);
                                }


                            }
                            else if (dResult == DialogResult.Ignore)
                            {
                                PMSDataLogger.LogMessage("frmCheckOut", "frmFrontOfficePOS DialogResult Ignore");
                                DateTime? CurrentTime = UIProcessManager.GetServiceTime();
                                if (CurrentTime != null)
                                {
                                    PMSDataLogger.LogMessage("frmCheckOut", "Setup Activity For Checkout Ignore Window =  " + Window);
                                    ActivityDTO activity = ActivityLogManager.SetupActivity(CurrentTime.Value, adCode.Value, CNETConstantes.PMS_Pointer, "Checkout Ignore -> Window = " + Window);
                                    activity.Reference = voucherBuffer.Voucher.Id;
                                    UIProcessManager.CreateActivity(activity);

                                }
                                DialogResult = System.Windows.Forms.DialogResult.OK;
                            }
                            else
                            {
                                PMSDataLogger.LogMessage("frmCheckOut", "FISCAL RECEIPT IS NOT SUCCESSFUL!");
                                SystemMessage.ShowModalInfoMessage("FISCAL RECEIPT IS NOT SUCCESSFUL!", "ERROR");
                            }
                        }
                        else if (dr == DialogResult.Cancel)
                        {
                            PMSDataLogger.LogMessage("frmCheckOut", "state change cancelled!");
                            SystemMessage.ShowModalInfoMessage("state change cancelled!", "MESSAGE");
                        }

                    }
                    // if payment is CASH
                    else
                    {

                        PMSDataLogger.LogMessage("frmCheckOut", "CHECKED OUT Checking Remaining Balance");
                        if (CheckRemainingBalance())
                        {
                            DialogResult dr = MessageBox.Show(@"Do you want to Check Out this guest?",
                                @"State Change Conformation", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);

                            if (dr == DialogResult.Yes)
                            {

                                PMSDataLogger.LogMessage("frmCheckOut", "CHECKING OUT Checking Remaining Balance");
                                frmFrontOfficePOS frmFrontOffice = new frmFrontOfficePOS();
                                frmFrontOffice.isCheckOut = true;
                                frmFrontOffice.RegExtension = RegExtension;
                                frmFrontOffice.VoucherBuffer = VoucherBuffer;
                                frmFrontOffice.Window = Window;
                                frmFrontOffice.IsReinstate = IsReinstate;

                                DialogResult dResult = frmFrontOffice.ShowDialog();
                                if (dResult == DialogResult.OK)
                                {

                                    PMSDataLogger.LogMessage("frmCheckOut", "frmFrontOfficePOS DialogResult OK");
                                    PMSDataLogger.LogMessage("frmCheckOut", "CHECKING OUT Checking Remaining Balance");
                                    DateTime? CurrentTime = UIProcessManager.GetServiceTime();
                                    if (CurrentTime != null)
                                    {
                                        PMSDataLogger.LogMessage("frmCheckOut", "Setup Activity For Checkout Window =  " + Window);
                                        ActivityDTO activity = ActivityLogManager.SetupActivity(CurrentTime.Value, adCode.Value, CNETConstantes.PMS_Pointer, "Checkout -> Window = " + Window);
                                        activity.Reference = voucherBuffer.Voucher.Id;
                                        UIProcessManager.CreateActivity(activity);
                                        bbCheckOut.Enabled = false;

                                    }
                                    DialogResult = System.Windows.Forms.DialogResult.OK;

                                    //show guest ledger form
                                    frmFolio _frmFolio = new frmFolio();

                                    RegistrationListVMDTO regExten = RegExtension;
                                    if (regExten != null)
                                    {
                                        _frmFolio.RegistrationExt = regExten;
                                        _frmFolio.ShowDialog(this);
                                    }
                                }
                                else if (dResult == DialogResult.Ignore)
                                {
                                    PMSDataLogger.LogMessage("frmCheckOut", "frmFrontOfficePOS DialogResult Ignore");
                                    DateTime? CurrentTime = UIProcessManager.GetServiceTime();
                                    if (CurrentTime != null)
                                    {
                                        PMSDataLogger.LogMessage("frmCheckOut", "Setup Activity For Checkout Ignore Window =  " + Window);
                                        ActivityDTO activity = ActivityLogManager.SetupActivity(CurrentTime.Value, adCode.Value, CNETConstantes.PMS_Pointer, "Checkout Ignore -> Window = " + Window);
                                        activity.Reference = voucherBuffer.Voucher.Id;
                                        UIProcessManager.CreateActivity(activity);

                                    }
                                    DialogResult = System.Windows.Forms.DialogResult.OK;
                                }
                                else
                                {
                                    PMSDataLogger.LogMessage("frmCheckOut", "FISCAL RECEIPT IS NOT SUCCESSFUL!");
                                    SystemMessage.ShowModalInfoMessage("FISCAL RECEIPT IS NOT SUCCESSFUL!", "ERROR");
                                }
                            }
                            else if (dr == DialogResult.Cancel)
                            {
                                PMSDataLogger.LogMessage("frmCheckOut", "State change cancelled!");
                                SystemMessage.ShowModalInfoMessage("state change cancelled!", "MESSAGE");
                            }
                        }//end of check remaing balace


                    }//end of cash entry


                }

                ////CNETInfoReporter.Hide();

            }
            catch (Exception ex)
            {
                PMSDataLogger.LogMessage("frmCheckOut", "Exception Error", true, ex);
                SystemMessage.ShowModalInfoMessage("ERROR: " + ex.Message, "ERROR");
                ////CNETInfoReporter.Hide();
            }
        }
        private void logActiviy(RoomDetailDTO rmd)
        {
            try
            {
                DateTime? currentTime = UIProcessManager.GetServiceTime();
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
                act.ConsigneeUnit = regExtension.ConsigneeUnit;
                act.Remark = "Event Checkout";
                UIProcessManager.CreateActivity(act);
            }
            catch (Exception e) { }
        }
        private void bbiClose_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Close();
        }

        private void bbiViewFolio_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (RegExtension != null)
            {

                frmFolio _frmFolio = new frmFolio();
                _frmFolio.RegistrationExt = RegExtension;

                _frmFolio.ShowDialog(this);
            }
        }

        private void frmCheckOut_Load(object sender, EventArgs e)
        {
            if (!InitializeData())
            {
                this.Close();
            }
        }

        private void bbiReturnCard_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmDoorLock frmDoorLock = new frmDoorLock();
            frmDoorLock.RegExt = RegExtension;
            frmDoorLock.ShowDialog();

            if (frmDoorLock.CardReturned)
                RegExtension.Doorlock = null;
        }

        private void bbiViewAttachment_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                //frmAttachment frmAttachment = new frmAttachment();
                //frmAttachment.RegExt = RegExtension;
                //frmAttachment.ShowDialog();
            }
            catch (Exception ex)
            {

            }
        }

        private void lukCompany_EditValueChanged(object sender, EventArgs e)
        {
            LookUpEdit view = sender as LookUpEdit;
            if (view != null && view.EditValue != null)
            {
                //  _currentCompanyCode = view.EditValue.ToString();
            }
        }
        #endregion


    }
}