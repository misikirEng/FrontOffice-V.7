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
using DevExpress.XtraEditors.Controls;
using CNET.FrontOffice_V._7.Forms.Non_Navigatable_Modals;
using CNET_V7_Domain.Domain.SettingSchema;
using ProcessManager;
using CNET.FrontOffice_V._7.PMS.Common_Classes;
using CNET_V7_Domain.Domain.ViewSchema;
using CNET_V7_Domain.Domain.TransactionSchema;
using CNET_V7_Domain.Misc.PmsDTO;
using DevExpress.Mvvm.Native;
using CNET_V7_Domain.Domain.ConsigneeSchema;

namespace CNET.FrontOffice_V._7.Forms.State_Change
{
    public partial class frmCancellation : UILogicBase
    {
        private int? activityDefCode = null;


        public bool IsFromNightAudit { get; set; }

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

        RegistrationListVMDTO regExtension;
        internal RegistrationListVMDTO RegExtension
        {
            get { return regExtension; }
            set
            {

                regExtension = value;

            }
        }


        /******************** CONSTRUCTOR ***************************/
        public frmCancellation()
        {
            InitializeComponent();
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            lcStatus.Text = @"You are cancelling the guest below";

            InitializeUI();
        }


        #region Helper Methods

        private void InitializeUI()
        {
            //Reason
            cacReason.Properties.Columns.Add(new LookUpColumnInfo("description", "Reason"));
            cacReason.Properties.DisplayMember = "description";
            cacReason.Properties.ValueMember = "code";
        }


        private bool InitializeData()
        {
            try
            {
                if (RegExtension == null) return false;

                // Progress_Reporter.Show_Progress("Intializing Cancellation...");

                //check workflow

                ActivityDefinitionDTO workFlow = UIProcessManager.GetActivityDefinitionBydescriptionandreference(CNETConstantes.LU_ACTIVITY_DEFINATION_Cancel, CNETConstantes.REGISTRATION_VOUCHER).FirstOrDefault();

                if (workFlow != null)
                {

                    activityDefCode = workFlow.Id;
                }
                else
                {
                    ////CNETInfoReporter.Hide();
                    SystemMessage.ShowModalInfoMessage("Please define workflow of CANCEL for Registration Voucher", "ERROR");
                    return false;
                }

                //Check Activity Previlage
                var userRoleMapper = LocalBuffer.LocalBuffer.UserRoleMapperBufferList.FirstOrDefault(r => r.User == LocalBuffer.LocalBuffer.CurrentLoggedInUser.Id);
                if (userRoleMapper != null)
                {
                    var roleActivity = UIProcessManager.GetRoleActivityByactivityDefinition(activityDefCode.Value).FirstOrDefault(r => r.Role == userRoleMapper.Role);//  && r.NeedsPassCode);
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
                    int index = RegExtension.Guest.IndexOf("(");
                    if (index > 0)
                    {
                        teGuest.Text = RegExtension.Guest.Substring(0, index);
                    }
                    else
                    {
                        teGuest.Text = RegExtension.Guest;
                    }
                }
                teRoom.Text = RegExtension.Room;
                tePaymentType.Text = RegExtension.PaymentDesc;
                teDeparture.Text = RegExtension.Departure.Date.ToString();

                //get Guest Ledger
                GuestLedgerDTO gLedger = UIProcessManager.GetGuestLedger(RegExtension.Id,
                    RegExtension.Arrival.Date, RegExtension.Departure.Date, RegExtension.Room, null);
                if (gLedger != null)
                {
                    teTotalBill.Text = gLedger.TotalCredit.ToString();
                    tePaid.Text = gLedger.TotalPaid.ToString();
                    teRemaing.Text = gLedger.RemainingBalanceFormated;

                }

                VoucherDTO Voucher = UIProcessManager.GetVoucherById(RegExtension.Id);


                if (Voucher.Consignee3 != null)
                {
                    ConsigneeDTO Agent = LocalBuffer.LocalBuffer.AllCustomerConsigneeViewlist.FirstOrDefault(x => x.Id == Voucher.Consignee3);
                    teAgent.Text = Agent.FirstName;
                }

                if (Voucher.Consignee4 != null)
                {
                    ConsigneeDTO Source = LocalBuffer.LocalBuffer.AllCustomerConsigneeViewlist.FirstOrDefault(x => x.Id == Voucher.Consignee3);
                    teSource.Text = Source.FirstName;
                }

                if (Voucher.Consignee2 != null)
                {
                    ConsigneeDTO Company = LocalBuffer.LocalBuffer.AllCustomerConsigneeViewlist.FirstOrDefault(x => x.Id == Voucher.Consignee2);

                    teCompany.Text = Company.FirstName;
                }




                List<SystemConstantDTO> cancelReasons = LocalBuffer.LocalBuffer.SystemConstantDTOBufferList.Where(x => x.Category == "Cancellation Reasons").ToList();
                if (cancelReasons != null)
                {
                    cacReason.Properties.DataSource = (cancelReasons.OrderByDescending(c => c.IsDefault).ToList());
                    SystemConstantDTO canLookup = cancelReasons.FirstOrDefault(c => c.IsDefault);
                    if (canLookup != null)
                        cacReason.EditValue = (canLookup.Id);

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

        private void bbiClose_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Close();
        }


        private void bbCancel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                var response = UIProcessManager.GetVoucherBufferById(RegExtension.Id);
                if (response == null || !response.Success)
                {
                    SystemMessage.ShowModalInfoMessage("Unable to get voucher!", "ERROR");
                    return;
                }

                VoucherBuffer voucherBuff = response.Data;
                if (voucherBuff == null)
                {
                    SystemMessage.ShowModalInfoMessage("Unable to get voucher!", "ERROR");
                    return;
                }

                //var voExtDoorLock = LocalBuffer.LocalBuffer.VoucherExtensionBufferList.FirstOrDefault(v => v.Type == CNETConstantes.VOUCHER_EXTENTION_DEFINITION_DOOR_LOCK && v.VoucherDefinition == CNETConstantes.REGISTRATION_VOUCHER);
                //if (voExtDoorLock != null)
                //{
                // Progress_Reporter.Show_Progress("Checking door lock card return");

                if (!string.IsNullOrEmpty(voucherBuff.Voucher.Extension1))
                {
                    ////CNETInfoReporter.Hide();
                    SystemMessage.ShowModalInfoMessage("Please return guest's door lock card first!", "ERROR");
                    rpgReturn.Visible = true;
                    return;
                }
                ////CNETInfoReporter.Hide();

                // }

                // Progress_Reporter.Show_Progress("Cancelling registration");

                DateTime? CurrentTime = UIProcessManager.GetServiceTime();
                if (CurrentTime == null) return;

                voucherBuff.Voucher.LastState = CNETConstantes.OSD_CANCEL_STATE;

                if (voucherBuff.TransactionReferencesBuffer != null && voucherBuff.TransactionReferencesBuffer.Count > 0)
                    voucherBuff.TransactionReferencesBuffer.ForEach(x => x.ReferencedActivity = null);

                voucherBuff.Activity = ActivityLogManager.SetupActivity(CurrentTime.Value, activityDefCode.Value, CNETConstantes.PMS_Pointer, cacReason.Text + "," + regExtension.lastState);

                voucherBuff.TransactionCurrencyBuffer = null;

                if (UIProcessManager.UpdateVoucherBuffer(voucherBuff) != null)
                {
                    SystemMessage.ShowModalInfoMessage("State changed successfully!", "MESSAGE");
                    DialogResult = System.Windows.Forms.DialogResult.OK;
                }
                else
                {
                    SystemMessage.ShowModalInfoMessage("NOT SUCCESSFUL!", "ERROR");
                }


                ////CNETInfoReporter.Hide();
            }
            catch (Exception ex)
            {
                ////CNETInfoReporter.Hide();
                SystemMessage.ShowModalInfoMessage("Error in cancelling registration. Detail:: " + ex.Message, "ERROR");

            }
        }


        private void frmCancellation_Load(object sender, EventArgs e)
        {
            if (!InitializeData())
            {
                this.Close();
            }
        }








        #endregion

        private void bbiReturnCard_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmDoorLock frmDoorLock = new frmDoorLock();
            frmDoorLock.RegExt = RegExtension;
            frmDoorLock.ShowDialog();
        }
    }
}