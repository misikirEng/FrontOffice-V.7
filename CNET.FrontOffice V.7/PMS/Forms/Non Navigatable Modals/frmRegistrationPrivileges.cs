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
using CNET_V7_Domain.Misc.PmsDTO;
using CNET_V7_Domain.Domain.PmsSchema;
using ProcessManager;
using CNET_V7_Domain.Domain.SettingSchema;
using CNET.FrontOffice_V._7.PMS.Common_Classes;
using CNET_V7_Domain.Domain.CommonSchema;
using CNET_V7_Domain.Domain.TransactionSchema;

namespace CNET.FrontOffice_V._7.Forms.Non_Navigatable_Modals
{
    public partial class frmRegistrationPrivileges : DevExpress.XtraEditors.XtraForm
    {
        RegistrationListVMDTO registrationExt;
        public frmRegistrationPrivileges()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
        }
        internal RegistrationListVMDTO RegistrationExt
        {
            get { return registrationExt; }
            set
            {
                registrationExt = value;
                teRegistrationNo.Text = value.Registration;

            }
        }

        private RegistrationPrivllegeDTO registrationPrivllege;
        private bool isEdit = false;
        internal RegistrationPrivllegeDTO RegistrationPrivllege
        {
            get { return registrationPrivllege; }
            set
            {
                registrationPrivllege = value;
                teRegistrationNo.Text = value.Voucher.ToString();
                ceNoPost.Checked = value.Nopost;
                ceAouthorize.Checked = value.AuthorizeDirectBill;
                cePreStayCharging.Checked = value.PreStayCharging;
                cePostStayCheckOut.Checked = value.PostStayCharging;
                ceAllowLateCheckOut.Checked = value.AllowLatecheckout;
                ceAuthorizeKeyReturn.Checked = value.AuthorizeKeyReturn == null ? false : value.AuthorizeKeyReturn;
                teRemark.Text = value.Remark;
                isEdit = true;
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


        private int? adPrev = null;

        public DateTime CurrentTime { get; set; }

        private bool InitializeData()
        {
            try
            {
                var dateTime = UIProcessManager.GetServiceTime();
                if (dateTime == null) return false;
                CurrentTime = dateTime.Value;


                //Workflow
                ActivityDefinitionDTO workflow = UIProcessManager.GetActivityDefinitionBydescriptionandreference(CNETConstantes.LU_ACTIVITY_DEFINATION_PREVILAGED, CNETConstantes.REGISTRATION_VOUCHER).FirstOrDefault();

                if (workflow != null)
                {

                    adPrev = workflow.Id;
                }
                else
                {
                    SystemMessage.ShowModalInfoMessage("Please define workflow of PREVILAGE for Registration Voucher ", "ERROR");
                    return false;
                }

                //Check Activity Previlage
                var userRoleMapper = LocalBuffer.LocalBuffer.UserRoleMapperBufferList.FirstOrDefault(r => r.User == LocalBuffer.LocalBuffer.CurrentLoggedInUser.Id);
                if (userRoleMapper != null)
                {
                    var roleActivity = UIProcessManager.GetRoleActivityByactivityDefinition(adPrev.Value).FirstOrDefault(r => r.Role == userRoleMapper.Role && r.NeedsPassCode);
                    if (roleActivity != null)
                    {
                        frmNeedPassword frmNeedPass = new frmNeedPassword(true);

                        frmNeedPass.ShowDialog();
                        if (!frmNeedPass.IsAutenticated)
                        {
                            return false;
                        }

                    }

                }

                return true;
            }
            catch (Exception ex)
            {
                SystemMessage.ShowModalInfoMessage("Error in initializing. Detail:: " + ex.Message, "ERROR");
                return false;
            }

        }

        private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Close();
        }

        public void Reset()
        {
            ceNoPost.Checked = false;
            ceAllowLateCheckOut.Checked = false;
            ceAouthorize.Checked = false;
            cePreStayCharging.Checked = false;
            cePostStayCheckOut.Checked = false;
            teRemark.Text = "";
        }

        //SAVE
        private void barButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            RegistrationPrivllegeDTO previlage = new RegistrationPrivllegeDTO();
            previlage.Voucher = RegistrationExt.Id;
            previlage.Nopost = ceNoPost.Checked;
            previlage.AuthorizeDirectBill = ceAouthorize.Checked;
            previlage.PreStayCharging = cePreStayCharging.Checked;
            previlage.PostStayCharging = cePostStayCheckOut.Checked;
            previlage.AllowLatecheckout = ceAllowLateCheckOut.Checked;
            previlage.AuthorizeKeyReturn = ceAuthorizeKeyReturn.Checked;
            previlage.Remark = teRemark.Text;
            if (isEdit)
            {
                previlage.Id = RegistrationPrivllege.Id;
                if (UIProcessManager.UpdateRegistrationPrivllege(previlage) != null)
                {

                    ActivityDTO activity = ActivityLogManager.SetupActivity(CurrentTime, adPrev.Value, CNETConstantes.PMS_Pointer, "");
                    activity.Reference = RegistrationExt.Id;
                    UIProcessManager.CreateActivity(activity);

                    SystemMessage.ShowModalInfoMessage("Updated Successfully!!!", "MESSAGE");
                    DialogResult = DialogResult.OK;
                    Reset();
                }
                else
                {
                    SystemMessage.ShowModalInfoMessage("Updating successful!!!", "ERROR");
                }
            }
            else
            {
                RegistrationPrivllegeDTO check = UIProcessManager.GetRegistrationPrivllegeByvoucher(registrationExt.Id);
                if (check != null)
                {
                    if (UIProcessManager.CreateRegistrationPrivllege(previlage) != null)
                    {
                        DialogResult = DialogResult.OK;

                        ActivityDTO activity = ActivityLogManager.SetupActivity(CurrentTime, adPrev.Value, CNETConstantes.PMS_Pointer, "");
                        activity.Reference = RegistrationExt.Id;
                        UIProcessManager.CreateActivity(activity);

                        SystemMessage.ShowModalInfoMessage("Saved Successfully!!!", "MESSAGE");
                        Reset();
                    }
                    else
                    {
                        SystemMessage.ShowModalInfoMessage("Not Saved!!!", "ERROR");
                    }

                }
                else
                {
                    check.Voucher = RegistrationExt.Id;
                    check.Nopost = ceNoPost.Checked;
                    check.AuthorizeDirectBill = ceAouthorize.Checked;
                    check.PreStayCharging = cePreStayCharging.Checked;
                    check.PostStayCharging = cePostStayCheckOut.Checked;
                    check.AllowLatecheckout = ceAllowLateCheckOut.Checked;
                    check.AuthorizeKeyReturn = ceAuthorizeKeyReturn.Checked;
                    check.Remark = teRemark.Text;

                    if (UIProcessManager.UpdateRegistrationPrivllege(check) != null)
                    {

                        ActivityDTO activity = ActivityLogManager.SetupActivity(CurrentTime, adPrev.Value, CNETConstantes.PMS_Pointer, "");
                        activity.Reference = RegistrationExt.Id;
                        UIProcessManager.CreateActivity(activity);

                        SystemMessage.ShowModalInfoMessage("Updated Successfully!!!", "MESSAGE");
                        DialogResult = DialogResult.OK;
                        Reset();
                    }
                    else
                    {
                        SystemMessage.ShowModalInfoMessage("Updating successful!!!", "ERROR");
                    }

                }

            }
            //  MasterPageForm.LoadNoPostPrevilegeBuffer();

        }

        private void ceAouthorize_CheckedChanged(object sender, EventArgs e)
        {
            //NonCashTransaction nCashTransaction =
            //        UIProcessManager.SelectAllNonCashTransaction()
            //            .FirstOrDefault(r => r.voucher == RegistrationExt.Registration);
            //if (ceAouthorize.Checked)
            //{

            //    if (nCashTransaction != null)
            //    {
            //        nCashTransaction.paymentMethod = CNETConstantes.PAYMENTMETHODS_DIRECT_BILL;
            //        UIProcessManager.UpdateNonCashTransaction(nCashTransaction);
            //    }
            //}
            //else
            //{
            //    if (nCashTransaction != null)
            //    {
            //        if (RegistrationExt.Payment=="Credit Card")
            //        {
            //            nCashTransaction.paymentMethod = CNETConstantes.PAYMNET_METHOD_CREDITCARD;
            //        }
            //        else
            //        {
            //            nCashTransaction.paymentMethod = CNETConstantes.PAYMENTMETHODSCASH;
            //        }
            //        UIProcessManager.UpdateNonCashTransaction(nCashTransaction);
            //    }
            //}
        }

        private void ceAouthorize_Click(object sender, EventArgs e)
        {

            CheckEdit edit = sender as CheckEdit;

            if (edit != null && !edit.Checked)
            {
                VoucherDTO updatevou = UIProcessManager.GetVoucherById(RegistrationExt.Id);

                if (updatevou != null)
                {
                    updatevou.PaymentMethod = CNETConstantes.PAYMENTMETHODS_DIRECT_BILL;
                    UIProcessManager.UpdateVoucher(updatevou);
                }
            }
            else
            {
                VoucherDTO updatevou = UIProcessManager.GetVoucherById(RegistrationExt.Id);
                if (updatevou != null)
                {
                    if (RegistrationExt.PaymentDesc == "Credit Card")
                    {
                        updatevou.PaymentMethod = CNETConstantes.PAYMNET_METHOD_CREDITCARD;
                    }
                    else
                    {
                        updatevou.PaymentMethod = CNETConstantes.PAYMENTMETHODSCASH;
                    }
                    UIProcessManager.UpdateVoucher(updatevou);
                }
            }
        }

        private void frmRegistrationPrivileges_Load(object sender, EventArgs e)
        {
            if (!InitializeData())
            {
                this.Close();
            }
        }
    }
}