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
using CNET.FrontOffice_V._7.Forms.Non_Navigatable_Modals;
using CNET.FrontOffice_V._7.PMS.Common_Classes;
using CNET_V7_Domain.Domain.SettingSchema;
using ProcessManager;
using CNET_V7_Domain.Domain.ConsigneeSchema;
using CNET_V7_Domain.Domain.ViewSchema;
using CNET_V7_Domain.Domain.TransactionSchema;
using CNET_V7_Domain.Domain.CommonSchema;
using CNET_V7_Domain.Misc.PmsDTO;

namespace CNET.FrontOffice_V._7.Forms.State_Change
{
    public partial class frmWaitingList : UILogicBase
    {
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

        private RegistrationListVMDTO regExtension;
        private int adCode;
        internal RegistrationListVMDTO RegExtension
        {
            get { return regExtension; }
            set
            {
                regExtension = value;

            }
        }
        /********************** CONSTRUCTOR ******************/
        public frmWaitingList()
        {
            InitializeComponent();
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            lcStatus.Text = "You are changing to WAITING-LIST the guest below";
        }


        #region Helper Methods

        private bool InitializeData()
        {
            try
            {
                if (RegExtension == null)
                {
                    SystemMessage.ShowModalInfoMessage("Please select a registration!", "ERROR");
                    return false;
                }

                // Progress_Reporter.Show_Progress("Initializing Waiting form...");

                //check workflow
                ActivityDefinitionDTO workFlow = UIProcessManager.GetActivityDefinitionBydescriptionandreference(CNETConstantes.LU_ACTIVITY_DEFINATION_Waitinglist, CNETConstantes.REGISTRATION_VOUCHER).FirstOrDefault();

                if (workFlow != null)
                {

                    adCode = workFlow.Id;
                }
                else
                {
                    ////CNETInfoReporter.Hide();
                    SystemMessage.ShowModalInfoMessage("Please define workflow of WAITING LIST for Registration Voucher ", "ERROR");
                    return false;
                }

                //Check Activity Previlage
                var userRoleMapper = LocalBuffer.LocalBuffer.UserRoleMapperBufferList.FirstOrDefault(r => r.User == LocalBuffer.LocalBuffer.CurrentLoggedInUser.Id);
                if (userRoleMapper != null)
                {
                    var roleActivity = UIProcessManager.GetRoleActivityByroleandactivityDefinition(userRoleMapper.Role, adCode);
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
                if (RegExtension.AgentId != null)
                {
                    ConsigneeDTO orgA = LocalBuffer.LocalBuffer.ConsigneeViewBufferList.FirstOrDefault(x => x.Id == RegExtension.AgentId.Value);
                    if (orgA != null)
                    {
                        teAgent.Text = orgA.FirstName;
                    }

                }
                if (RegExtension.SourceId != null)
                {
                    ConsigneeDTO orgS = LocalBuffer.LocalBuffer.ConsigneeViewBufferList.FirstOrDefault(x => x.Id == RegExtension.SourceId.Value);
                    if (orgS != null)
                    {
                        teSource.Text = orgS.FirstName;
                    }
                }
                if (RegExtension.CompanyId != null)
                {
                    ConsigneeDTO orgC = LocalBuffer.LocalBuffer.ConsigneeViewBufferList.FirstOrDefault(x => x.Id == RegExtension.CompanyId.Value);
                    if (orgC != null)
                    {
                        teCompany.Text = orgC.FirstName;
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

        #region Event Handlers

        private void bbiClose_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Close();
        }


        private void bbiWaiting_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                // Progress_Reporter.Show_Progress("Saving Waiting-List Process", "Please Wait...");
                VoucherDTO voucher = UIProcessManager.GetVoucherById(RegExtension.Id);
                if (voucher == null)
                {
                    ////CNETInfoReporter.Hide();
                    SystemMessage.ShowModalInfoMessage("Unable to get voucher data!", "ERROR");
                }
                voucher.LastState = CNETConstantes.OSD_WAITLIST_STATE;
                if (UIProcessManager.UpdateVoucher(voucher) != null)
                {
                    DialogResult = System.Windows.Forms.DialogResult.OK;

                    //saving activity
                    DateTime? currentTime = UIProcessManager.GetServiceTime();
                    if (currentTime != null)
                    {
                        ActivityDTO activity = ActivityLogManager.SetupActivity(currentTime.Value, adCode, CNETConstantes.PMS_Pointer, "Six-PM made from another state");
                        activity.Reference = voucher.Id;
                        ActivityDTO SavedActivity = UIProcessManager.CreateActivity(activity);

                        if (SavedActivity == null)
                        {
                            XtraMessageBox.Show("WARNING: activity log for this state change is not saved", "CNET_v2016", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                    SystemMessage.ShowModalInfoMessage("State changed successfully!", "MESSAGE");
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
                SystemMessage.ShowModalInfoMessage("Error in saving six-PM. Detail:: " + ex.Message, "ERROR");
            }
        }

        private void frmSxPM_Load(object sender, EventArgs e)
        {
            if (!InitializeData())
            {
                this.Close();
            }
        }

        #endregion
    }
}