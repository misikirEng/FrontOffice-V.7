
using CNET.ERP.Client.Common.UI;
using CNET.FrontOffice_V._7;
using ProcessManager;
using CNET.FrontOffice_V._7.Forms.Non_Navigatable_Modals;
using CNET.FrontOffice_V._7.PMS.Common_Classes;
using CNET_V7_Domain.Domain.SettingSchema;
using CNET_V7_Domain.Domain.TransactionSchema;
using CNET_V7_Domain.Domain.ViewSchema;
using CNET_V7_Domain.Misc.PmsDTO;
using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CNET_V7_Domain.Domain.ConsigneeSchema;

namespace CNET.FrontOffice_V._7.Forms.State_Change
{
    public partial class frmNoShow : UILogicBase
    {
        private int? _activityDefCode = null;

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

        /**************** CONTRUCTOR **************/
        public frmNoShow()
        {
            InitializeComponent();
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
                if (RegExtension == null)
                {
                    SystemMessage.ShowModalInfoMessage("Please select a registration!", "ERROR");
                    return false;
                }

                // Progress_Reporter.Show_Progress("Initializing Conformation form...");

                //check workflow
                ActivityDefinitionDTO workFlow = UIProcessManager.GetActivityDefinitionBydescriptionandreference(CNETConstantes.LU_ACTIVITY_DEFINATION_NOSHOW, CNETConstantes.REGISTRATION_VOUCHER).FirstOrDefault();

                if (workFlow != null)
                {

                    _activityDefCode = workFlow.Id;
                }
                else
                {
                    ////CNETInfoReporter.Hide();
                    SystemMessage.ShowModalInfoMessage("Please define workflow of NO SHOW for Registration Voucher ", "ERROR");
                    return false;
                }

                //Check Activity Previlage
                var userRoleMapper = LocalBuffer.LocalBuffer.UserRoleMapperBufferList.FirstOrDefault(r => r.User == LocalBuffer.LocalBuffer.CurrentLoggedInUser.Id);
                if (userRoleMapper != null)
                {
                    var roleActivity = UIProcessManager.GetRoleActivityByactivityDefinition(_activityDefCode.Value).FirstOrDefault(r => r.Role == userRoleMapper.Role && r.NeedsPassCode);
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

                //get Guest Ledger
                GuestLedgerDTO gLedger = UIProcessManager.GetGuestLedger(RegExtension.Id, RegExtension.Arrival.Date, RegExtension.Departure.Date, RegExtension.Room, null);
                if (gLedger != null)
                {
                    teTotalBill.Text = gLedger.TotalCredit.ToString();
                    tePaid.Text = gLedger.TotalPaid.ToString();
                    teRemaing.Text = gLedger.RemainingBalanceFormated;

                }

                teDeparture.Text = RegExtension.Departure.ToString();


                if (RegExtension.AgentId != null)
                {
                    ConsigneeDTO orgA = LocalBuffer.LocalBuffer.ConsigneeViewBufferList.FirstOrDefault(x => x.Id == RegExtension.AgentId);
                    if (orgA != null)
                    {
                        teAgent.Text = orgA.FirstName;
                    }
                }

                if (RegExtension.SourceId != null)
                {
                    ConsigneeDTO orgA = LocalBuffer.LocalBuffer.ConsigneeViewBufferList.FirstOrDefault(x => x.Id == RegExtension.SourceId);
                    if (orgA != null)
                    {
                        teSource.Text = orgA.FirstName;
                    }
                }

                if (RegExtension.CompanyId != null)
                {
                    ConsigneeDTO orgA = LocalBuffer.LocalBuffer.ConsigneeViewBufferList.FirstOrDefault(x => x.Id == RegExtension.CompanyId);
                    if (orgA != null)
                    {
                        teCompany.Text = orgA.FirstName;
                    }
                }

                ////CNETInfoReporter.Hide();
                return true;
            }
            catch (Exception ex)
            {
                ////CNETInfoReporter.Hide();
                SystemMessage.ShowModalInfoMessage("Error in initializing form! Detail:: " + ex.Message, "ERROR");
                return false;
            }
        }

        #endregion

        #region Event Hanlders
        private void bbiClose_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Close();
        }
        private void bbiNoShow_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DialogResult result = XtraMessageBox.Show("Do you want to change this registration to NO-SHOW state?", "No Show", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == System.Windows.Forms.DialogResult.No) return;

            try
            {
                // Progress_Reporter.Show_Progress("Saving...");

                DateTime? CurrentTime = UIProcessManager.GetServiceTime();

                var response = UIProcessManager.GetVoucherBufferById(RegExtension.Id);
                if (response != null && response.Success)

                {
                    VoucherBuffer voucherBuffer = response.Data;


                    if (voucherBuffer != null)
                    {
                        voucherBuffer.Voucher.LastState = CNETConstantes.NO_SHOW_STATE;
                        voucherBuffer.Activity = ActivityLogManager.SetupActivity(CurrentTime.Value, _activityDefCode.Value, CNETConstantes.PMS_Pointer);
                        if (voucherBuffer.TransactionReferencesBuffer != null && voucherBuffer.TransactionReferencesBuffer.Count > 0)
                            voucherBuffer.TransactionReferencesBuffer.ToList().ForEach(x => x.ReferencedActivity = null);


                        voucherBuffer.TransactionCurrencyBuffer = null;

                        if (UIProcessManager.UpdateVoucherBuffer(voucherBuffer) != null)
                        {
                            XtraMessageBox.Show("State successfully Changed!", "Successfull Operation", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            DialogResult = System.Windows.Forms.DialogResult.OK;
                        }
                        else
                            XtraMessageBox.Show("State Not Changed!", "Successfull Operation", MessageBoxButtons.OK, MessageBoxIcon.Information);


                    }
                    else
                    {
                        XtraMessageBox.Show("Unable to get voucher information of the seleceted registration", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }


                }
                else
                {
                    XtraMessageBox.Show("Unable to get voucher information of the seleceted registration", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                ////CNETInfoReporter.Hide();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Error in processing no-show. DETAIL::" + ex.Message, "No Show", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ////CNETInfoReporter.Hide();
            }
        }

        private void frmNoShow_Load(object sender, EventArgs e)
        {
            if (!InitializeData())
            {
                this.Close();
            }
        }

        #endregion



    }
}
