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
using CNET_V7_Domain.Misc.PmsDTO;
using CNET.FrontOffice_V._7.PMS.Common_Classes;
using CNET_V7_Domain.Domain.SettingSchema;
using ProcessManager;
using CNET_V7_Domain.Domain.ViewSchema;
using CNET_V7_Domain.Domain.TransactionSchema;
using CNET_V7_Domain.Domain.ConsigneeSchema;

namespace CNET.FrontOffice_V._7.Forms.State_Change
{
    public partial class frmConformation : UILogicBase
    {

        private int adCode;

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

        /*************** CONSTRUCTOR *********************/
        public frmConformation()
        {
            InitializeComponent();
            InitializeUI();

        }


        #region Helper Methods

        private void InitializeUI()
        {
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            lcStatus.Text = "You are conforming  the guest below";
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
                ActivityDefinitionDTO workFlow = UIProcessManager.GetActivityDefinitionBydescriptionandreference(CNETConstantes.LU_ACTIVITY_DEFINATION_Guaranteed, CNETConstantes.REGISTRATION_VOUCHER).FirstOrDefault();

                if (workFlow != null)
                {

                    adCode = workFlow.Id;
                }
                else
                {
                    ////CNETInfoReporter.Hide();
                    SystemMessage.ShowModalInfoMessage("Please define workflow of GUARANTED for Registration Voucher ", "ERROR");
                    return false;
                }

                //Check Activity Previlage
                var userRoleMapper = LocalBuffer.LocalBuffer.UserRoleMapperBufferList.FirstOrDefault(r => r.User == LocalBuffer.LocalBuffer.CurrentLoggedInUser.Id);
                if (userRoleMapper != null)
                {
                    var roleActivity = UIProcessManager.GetRoleActivityByactivityDefinition(adCode).FirstOrDefault(r => r.Role == userRoleMapper.Role && r.NeedsPassCode);
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
                GuestLedgerDTO gLedger = UIProcessManager.GetGuestLedger(RegExtension.Id,
                    RegExtension.Arrival.Date, RegExtension.Departure.Date, RegExtension.Room, null);
                if (gLedger != null)
                {
                    teTotalBill.Text = gLedger.TotalCredit.ToString();
                    tePaid.Text = gLedger.TotalPaid.ToString();
                    teRemaing.Text = gLedger.RemainingBalanceFormated;

                }

                teDeparture.Text = RegExtension.Departure.ToString();


                if (RegExtension.AgentId != null)
                {
                    ConsigneeDTO Agent = LocalBuffer.LocalBuffer.ConsigneeViewBufferList.FirstOrDefault(x => x.Id == RegExtension.AgentId);
                    teAgent.Text = Agent == null ? "" : Agent.FirstName;
                }
                if (RegExtension.SourceId != null)
                {
                    ConsigneeDTO Source = LocalBuffer.LocalBuffer.ConsigneeViewBufferList.FirstOrDefault(x => x.Id == RegExtension.SourceId);
                    teSource.Text = Source == null ? "" : Source.FirstName;
                }
                if (RegExtension.CompanyId != null)
                {
                    ConsigneeDTO Company = LocalBuffer.LocalBuffer.ConsigneeViewBufferList.FirstOrDefault(x => x.Id == RegExtension.CompanyId);
                    teCompany.Text = Company == null ? "" : Company.FirstName;
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

        private void bbConform_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                DateTime? currentTime = UIProcessManager.GetServiceTime();
                // Progress_Reporter.Show_Progress("Saving Conformation Process", "Please Wait...");

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

                voucherbuffer.Voucher.LastState = CNETConstantes.GAURANTED_STATE;
                voucherbuffer.Activity = ActivityLogManager.SetupActivity(currentTime.Value, adCode, CNETConstantes.PMS_Pointer, "Conformation made from another state");


                if (voucherbuffer.TransactionReferencesBuffer != null && voucherbuffer.TransactionReferencesBuffer.Count > 0)
                    voucherbuffer.TransactionReferencesBuffer.ToList().ForEach(x => x.ReferencedActivity = null);



                voucherbuffer.TransactionCurrencyBuffer = null;


                if (UIProcessManager.UpdateVoucherBuffer(voucherbuffer) != null)
                {
                    DialogResult = System.Windows.Forms.DialogResult.OK;

                    SystemMessage.ShowModalInfoMessage("State successfully Changed!", "MESSAGE");

                    try
                    {
                        //ReportGenerator reportGenerator = new ReportGenerator();
                        //reportGenerator.GenerateAttachmentReport(voucher.code, false, false);
                    }
                    catch (Exception) { }
                    ////CNETInfoReporter.Hide();

                }
                else
                {
                    SystemMessage.ShowModalInfoMessage("NOT SUCCESSFUL!", "ERROR");
                }
            }
            catch (Exception ex)
            {
                ////CNETInfoReporter.Hide();
                SystemMessage.ShowModalInfoMessage("Error in saving conformation. Detail:: " + ex.Message, "ERROR");
            }


        }

        private void bbiFolio_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (RegExtension != null)
            {
                frmFolio _frmFolio = new frmFolio();
                _frmFolio.RegistrationExt = RegExtension;

                _frmFolio.ShowDialog(this);
            }
        }

        private void frmConformation_Load(object sender, EventArgs e)
        {
            if (!InitializeData())
            {
                this.Close();
            }
        }

        #endregion




    }
}