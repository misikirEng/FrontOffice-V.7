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
using ProcessManager;
using CNET_V7_Domain.Domain.SettingSchema;
using CNET_V7_Domain.Domain.TransactionSchema;
using CNET_V7_Domain.Domain.ViewSchema;
using System.Diagnostics;
using CNET_V7_Domain.Domain.CommonSchema;
using CNET_V7_Domain.Misc.PmsDTO;
using CNET_V7_Domain.Domain.ConsigneeSchema;

namespace CNET.FrontOffice_V._7.Forms.State_Change
{
    public partial class frmSxPM : UILogicBase
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
        public frmSxPM()
        {
            InitializeComponent();

            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            lcStatus.Text = "You are changing to 6 PM the guest below";
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

                // Progress_Reporter.Show_Progress("Initializing 6PM form...");

                //check workflow
                ActivityDefinitionDTO workFlow = UIProcessManager.GetActivityDefinitionBydescriptionandreference(CNETConstantes.LU_ACTIVITY_DEFINATION_6PM, CNETConstantes.REGISTRATION_VOUCHER).FirstOrDefault();

                if (workFlow != null)
                {

                    adCode = workFlow.Id;
                }
                else
                {
                    ////CNETInfoReporter.Hide();
                    SystemMessage.ShowModalInfoMessage("Please define workflow of 6PM for Registration Voucher ", "ERROR");
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
                teDeparture.Text = RegExtension.Departure.Date.ToString();

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
        public static void SynchronizeRegistration(string Registration)
        {
        }

        private void bbiSixPM_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                DateTime? CurrentTime = UIProcessManager.GetServiceTime();
                if (CurrentTime == null) return;
                // Progress_Reporter.Show_Progress("Saving Six-PM Process", "Please Wait...");\



                var response = UIProcessManager.GetVoucherBufferById(RegExtension.Id);

                if (response == null || !response.Success)
                {
                    SystemMessage.ShowModalInfoMessage("Unable to get voucher!", "ERROR");
                    return;
                }

                VoucherBuffer voucher = response.Data;

                if (voucher == null)
                {
                    SystemMessage.ShowModalInfoMessage("Unable to get voucher!", "ERROR");
                    return;
                }
                voucher.Voucher.LastState = CNETConstantes.SIX_PM_STATE;

                voucher.Activity = ActivityLogManager.SetupActivity(CurrentTime.Value, adCode, CNETConstantes.PMS_Pointer, "Six-PM made from another state");
                if (voucher.TransactionReferencesBuffer != null && voucher.TransactionReferencesBuffer.Count > 0)
                    voucher.TransactionReferencesBuffer.ToList().ForEach(x => x.ReferencedActivity = null);


                voucher.TransactionCurrencyBuffer = null;

                if (UIProcessManager.UpdateVoucherBuffer(voucher) != null)
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