using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using CNET.FrontOffice_V._7;
using CNET.ERP.Client.Common.UI;
using CNET.FrontOffice_V._7.Forms.Non_Navigatable_Modals;
using ProcessManager;
using CNET_V7_Domain.Misc.PmsDTO;
using CNET.FrontOffice_V._7.PMS.Common_Classes;
using CNET_V7_Domain.Domain.SettingSchema;
using CNET_V7_Domain.Domain.CommonSchema;
using DevExpress.Mvvm.POCO;
using CNET_V7_Domain.Domain.TransactionSchema;

namespace CNET.FrontOffice_V._7.Forms.State_Change
{
    public partial class frmReInState : UILogicBase
    {
        private int? activityDefCode = null;

        // properties
        private RegistrationListVMDTO regExtension;
        internal RegistrationListVMDTO RegExtension
        {
            get { return regExtension; }
            set
            {
                regExtension = value;

            }
        }

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


        /// ////////////////// CONSTRUCTOR //////////////////////////
        public frmReInState()
        {
            InitializeComponent();
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;


            InitializeUI();
        }



        #region Helper Methods

        private void InitializeUI()
        {

        }

        private bool InitializeData()
        {
            try
            {
                if (RegExtension == null) return false;
                if (RegExtension.lastState == CNETConstantes.CHECKED_IN_STATE)
                {
                    this.Width = 650;
                }
                // Progress_Reporter.Show_Progress("Initializing Re-Instate...");
                int configMin = 360;
                if (RegExtension.lastState == CNETConstantes.CHECKED_IN_STATE)
                {
                    DateTime? currentTime = UIProcessManager.GetServiceTime();
                    if (currentTime != null)
                    {
                        //read configuration 
                        var config = LocalBuffer.LocalBuffer.ConfigurationBufferList.FirstOrDefault(c => c.Reference == CNETConstantes.PMS_Pointer.ToString() && c.Attribute == CNETConstantes.PMS_SETTING_UndoCheckinMin);
                        if (config != null)
                        {
                            DateTime currentDate = currentTime.Value;
                            if (currentDate.Date != RegExtension.Arrival.Date)
                            {
                                ////CNETInfoReporter.Hide();
                                SystemMessage.ShowModalInfoMessage("Undo Check-In time is over for this guest!", "ERROR");
                                return false;
                            }
                            //int currentMin = currentTime.Value.Minute;
                            //int arrivalMin = RegExtension.Arrival.Minute;
                            int minDiff = (currentTime.Value.Subtract(RegExtension.Arrival)).Minutes;
                            try
                            {
                                configMin = Convert.ToInt32(config.CurrentValue);
                            }
                            catch
                            {
                                SystemMessage.ShowModalInfoMessage("Undo Check-In time setting error! " + Environment.NewLine + "Undo Check-In time Default time 6 Hrs !!", "ERROR");
                            }
                            if (minDiff > configMin)
                            {
                                ////CNETInfoReporter.Hide();
                                SystemMessage.ShowModalInfoMessage("Undo Check-In time is over for this guest!", "ERROR");
                                return false;
                            }
                        }
                        else
                        {
                            SystemMessage.ShowModalInfoMessage("No Undo Check-In time setting!! ", "ERROR");
                            return false;
                        }
                    }
                    lcStatus.Text = @"You are reinstating the Check-In guest to Guaranted State.";
                }
                else if (RegExtension.lastState == CNETConstantes.OSD_CANCEL_STATE)
                {
                    lcStatus.Text = @"You are reinstating the Canceled guest.";
                }
                else if (RegExtension.lastState == CNETConstantes.CHECKED_OUT_STATE)
                {
                    lcStatus.Text = @"You are reinstating the Checked-Out guest.";
                }



                //check workflow

                ActivityDefinitionDTO workFlow = UIProcessManager.GetActivityDefinitionBydescriptionandreference(CNETConstantes.LU_ACTIVITY_DEFINATION_REINSTATE, CNETConstantes.REGISTRATION_VOUCHER).FirstOrDefault();

                if (workFlow != null)
                {
                    activityDefCode = workFlow.Id;
                }
                else
                {
                    ////CNETInfoReporter.Hide();
                    SystemMessage.ShowModalInfoMessage("Please define workflow of RE-INSTATE for Registration Voucher", "ERROR");
                    return false;
                }
                //Check Activity Previlage
                var userRoleMapper = LocalBuffer.LocalBuffer.UserRoleMapperBufferList.FirstOrDefault(r => r.User == LocalBuffer.LocalBuffer.CurrentLoggedInUser.Id);
                if (userRoleMapper != null)
                {
                    var roleActivity = UIProcessManager.GetRoleActivityByactivityDefinition(activityDefCode.Value).FirstOrDefault(r => r.Role == userRoleMapper.Role && r.NeedsPassCode);
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
                if (RegExtension.GuestId != null)
                {
                    int index = RegExtension.Guest.IndexOf("(", StringComparison.Ordinal);
                    teGuest.Text = index > 0 ? RegExtension.Guest.Substring(0, index) : RegExtension.Guest;
                }
                teRoom.Text = RegExtension.Room;
                tePaymentType.Text = RegExtension.PaymentDesc;
                teDeparture.Text = RegExtension.Departure.Date.ToString();

                //for cancellation, disable cbeAction combo
                if (RegExtension.lastState == CNETConstantes.OSD_CANCEL_STATE || RegExtension.lastState == CNETConstantes.CHECKED_IN_STATE)
                {
                    cbeAction.Enabled = false;
                }



                //get Guest Ledger
                GuestLedgerDTO gLedger = UIProcessManager.GetGuestLedger(RegExtension.Id, RegExtension.Arrival.Date, RegExtension.Departure.Date, RegExtension.Room, null);
                if (gLedger != null)
                {
                    teTotalBill.Text = gLedger.TotalCredit.ToString();
                    tePaid.Text = gLedger.TotalPaid.ToString();
                    teRemaing.Text = gLedger.RemainingBalanceFormated;
                }
                if (RegExtension.AgentId != null)
                {
                    teAgent.Text = LocalBuffer.LocalBuffer.AllCustomerConsigneeViewlist.FirstOrDefault(x => x.Id == RegExtension.AgentId).FirstName;
                }
                if (RegExtension.SourceId != null)
                {
                    teSource.Text = LocalBuffer.LocalBuffer.AllCustomerConsigneeViewlist.FirstOrDefault(x => x.Id == RegExtension.SourceId).FirstName;
                }
                if (RegExtension.CompanyId != null)
                {
                    teCompany.Text = LocalBuffer.LocalBuffer.AllCustomerConsigneeViewlist.FirstOrDefault(x => x.Id == RegExtension.CompanyId).FirstName;
                }
                ////CNETInfoReporter.Hide();

                return true;
            }
            catch (Exception ex)
            {
                ////CNETInfoReporter.Hide();
                XtraMessageBox.Show("Error in initializing form. Detail: " + ex.Message, "CNET_v2016", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        #endregion

        #region Event Handlers

        private void frmReInState_Load(object sender, EventArgs e)
        {
            if (!InitializeData())
            {
                this.Close();
            }
        }
        public static void SynchronizeRegistration(string Registration)
        {

        }
        private void bbiReinstate_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                DateTime? CurrentTime = UIProcessManager.GetServiceTime();
                if (CurrentTime == null) return;

                bool flag = false;
                if (LocalBuffer.LocalBuffer.CurrentLoggedInUser.Id != null && LocalBuffer.LocalBuffer.CurrentDevice != null)
                {
                    if (RegExtension.lastState == CNETConstantes.CHECKED_IN_STATE)
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
                        ////CNETInfoReporter.Hide();
                        // }
                    }
                    else
                    {
                        // Progress_Reporter.Show_Progress("Checking Room Occupancy...");
                        bool isRoomOccupied = CommonLogics.IsRoomOccupied(RegExtension.Id, RegExtension.RoomCode, RegExtension.RoomType.Value, RegExtension.Arrival.Date, RegExtension.Departure.Date, CurrentTime.Value, SelectedHotelcode);
                        ////CNETInfoReporter.Hide();
                        if (isRoomOccupied)
                        {
                            DialogResult dr = MessageBox.Show("Can't reinstate this registration. The current room is occupied! Do you want to move the room?", "Re-Instate", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                            if (dr == System.Windows.Forms.DialogResult.No)
                            {

                                return;
                            }

                            if (RegExtension.Room == null || RegExtension.Room.Contains("#"))
                            {
                                return;
                            }

                            frmRoomMove frmRoomMove = new frmRoomMove();
                            frmRoomMove.SelectedHotelcode = SelectedHotelcode;
                            frmRoomMove.RegExtension = RegExtension;
                            if (frmRoomMove.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                            {
                                return;
                            }
                        }
                    }

                    ActivityDTO mActivity = ActivityLogManager.SetupActivity(CurrentTime.Value, activityDefCode.Value, CNETConstantes.PMS_Pointer, cbeAction.Text);
                    mActivity.Reference = RegExtension.Id;
                    if (RegExtension.lastState == CNETConstantes.CHECKED_OUT_STATE || RegExtension.lastState == CNETConstantes.ONLINE_CHECKED_OUT_STATE)
                    {
                        ActivityDTO activityCode = UIProcessManager.CreateActivity(mActivity);
                        if (activityCode != null)
                        {
                            flag = true;
                        }

                        if (flag)
                        {
                            VoucherDTO voucher = UIProcessManager.GetVoucherById(RegExtension.Id);
                            if (voucher == null)
                            {
                                SystemMessage.ShowModalInfoMessage("Unable to get voucher!", "ERROR");
                                return;
                            }
                            voucher.LastState = CNETConstantes.CHECKED_IN_STATE;
                            if (UIProcessManager.UpdateVoucher(voucher) != null)
                            {
                                DialogResult = System.Windows.Forms.DialogResult.OK;

                                SystemMessage.ShowModalInfoMessage("State changed successfully!", "MESSAGE");
                                //SynchronizeRegistration(voucher.code);
                            }
                            else
                            {
                                SystemMessage.ShowModalInfoMessage("NOT SUCCESSFUL!", "ERROR");
                            }
                        }
                        else
                        {
                            SystemMessage.ShowModalInfoMessage("NOT SUCCESSFUL! ", "ERROR");
                        }
                    }
                    // for cancel registrations
                    else if (RegExtension.lastState == CNETConstantes.OSD_CANCEL_STATE)
                    {
                        ActivityDefinitionDTO actDef =
                                             UIProcessManager.GetActivityDefinitionBydescriptionandreference(CNETConstantes.LU_ACTIVITY_DEFINATION_Cancel, CNETConstantes.REGISTRATION_VOUCHER).FirstOrDefault();


                        if (actDef == null)
                        {
                            SystemMessage.ShowModalInfoMessage("Unable to get the last workflow of CANCEL", "ERROR");
                            return;
                        }
                        ActivityDTO activity = UIProcessManager.GetActivityByreference(RegExtension.Id).FirstOrDefault(a => a.ActivityDefinition == actDef.Id);
                        if (activity != null && !string.IsNullOrEmpty(activity.Remark))
                        {
                            string[] splited = activity.Remark.Split(',');
                            if (splited.Length == 2)
                            {
                                mActivity.Platform = "1";
                                mActivity.ActivityDefinition = activityDefCode.Value;
                                mActivity.Remark = "From Canceled State";

                                ActivityDTO savedactivityCode = UIProcessManager.CreateActivity(mActivity);
                                //  string activityCode = ActivityLogManager.CommitActivity(mActivity, activityDefCode, CNETConstantes.REGISTRATION_VOUCHER, "From Canceled State", CNETConstantes.PMS);
                                if (savedactivityCode != null)
                                {
                                    flag = true;
                                }

                                if (flag)
                                {
                                    VoucherDTO voucher = UIProcessManager.GetVoucherById(RegExtension.Id);
                                    if (voucher == null)
                                    {
                                        SystemMessage.ShowModalInfoMessage("Unable to get voucher!", "ERROR");
                                        return;
                                    }
                                    voucher.LastState = Convert.ToInt32(splited[1]);
                                    if (UIProcessManager.UpdateVoucher(voucher) != null)
                                    {
                                        DialogResult = System.Windows.Forms.DialogResult.OK;

                                        SystemMessage.ShowModalInfoMessage("State changed successfully!", "MESSAGE");
                                        //  SynchronizeRegistration(voucher.code);

                                    }
                                    else
                                    {
                                        SystemMessage.ShowModalInfoMessage("NOT SUCCESSFUL!", "ERROR");
                                    }

                                }
                                else
                                {
                                    SystemMessage.ShowModalInfoMessage("NOT SUCCESSFUL!", "ERROR");
                                }
                            }
                            else
                            {
                                SystemMessage.ShowModalInfoMessage("Unable to get last activity log for this voucher", "ERROR");
                            }
                        }
                        else
                        {
                            SystemMessage.ShowModalInfoMessage("Unable to get last activity log for this voucher", "ERROR");
                        }
                    }
                    else if (RegExtension.lastState == CNETConstantes.CHECKED_IN_STATE)
                    {
                        VoucherDTO voucher = UIProcessManager.GetVoucherById(RegExtension.Id);
                        if (voucher == null)
                        {
                            SystemMessage.ShowModalInfoMessage("Unable to get voucher!", "ERROR");
                            return;
                        }
                        voucher.LastState = CNETConstantes.GAURANTED_STATE;
                        if (UIProcessManager.UpdateVoucher(voucher) != null)
                        {
                            DialogResult = System.Windows.Forms.DialogResult.OK;

                            //save undo-checkin activity


                            mActivity.ActivityDefinition = activityDefCode.Value;
                            mActivity.Remark = "Undo Check-In";
                            ActivityDTO savedactivityCode = UIProcessManager.CreateActivity(mActivity);


                            // string activityCode = ActivityLogManager.CommitActivity(mActivity, activityDefCode, CNETConstantes.REGISTRATION_VOUCHER, "Undo Check-In", CNETConstantes.PMS);


                            //Void All Transactions
                            List<TransactionReferenceDTO> transactionReference = UIProcessManager.GetTransactionReferenceByreferenced(RegExtension.Id);
                            if (transactionReference.Count > 0)
                            {
                                foreach (var tranRef in transactionReference)
                                {
                                    VoucherDTO vo = UIProcessManager.GetVoucherById(tranRef.Referring.Value);
                                    if (vo != null)
                                    {
                                        vo.IsVoid = true;
                                        VoucherDTO isUpdated = UIProcessManager.UpdateVoucher(vo);
                                        if (isUpdated != null)
                                        {
                                            if (vo.Definition == CNETConstantes.DAILY_ROOM_CHARGE_VOUCHER)
                                            {
                                                UIProcessManager.DeleteTransactionReferenceById(tranRef.Id);
                                            }

                                            //save void activity
                                            //check workflow
                                            ActivityDefinitionDTO workFlow = UIProcessManager.GetActivityDefinitionBydescriptionandreference(CNETConstantes.LU_ACTIVITY_DEFINATION_VOID, vo.Definition).FirstOrDefault();

                                            if (workFlow != null)
                                            {
                                                ActivityDTO activity = ActivityLogManager.SetupActivity(CurrentTime.Value, workFlow.Id, CNETConstantes.VOUCHER_COMPONENET, "voucher is void during Undo-Check In");

                                                ActivityDTO saveactivity = UIProcessManager.CreateActivity(activity);
                                                if (saveactivity != null)
                                                {
                                                    vo.LastActivity = saveactivity.Id;
                                                    vo.LastState = workFlow.State.Value;
                                                    UIProcessManager.UpdateVoucher(vo);
                                                }
                                            }

                                            //   MasterPageForm.LoadRoomChargesBuffer();
                                        }

                                    }
                                }
                            }

                            SystemMessage.ShowModalInfoMessage("State changed successfully!", "MESSAGE");
                            // SynchronizeRegistration(voucher.code);

                        }
                        else
                        {
                            SystemMessage.ShowModalInfoMessage("NOT SUCCESSFUL!", "ERROR");
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                SystemMessage.ShowModalInfoMessage("ERROR! " + ex.Message, "ERROR");
            }

        }

        private void bbiClose_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Close();
        }

        private void bbiReturnCard_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmDoorLock frmDoorLock = new frmDoorLock();
            frmDoorLock.RegExt = RegExtension;
            frmDoorLock.ShowDialog();
        }

        #endregion

        public int SelectedHotelcode { get; set; }
    }
}