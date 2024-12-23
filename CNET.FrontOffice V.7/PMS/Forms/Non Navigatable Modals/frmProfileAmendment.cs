using CNET.ERP.Client.Common.UI;
using CNET.FrontOffice_V._7.PMS.Contracts;
using CNET.FrontOffice_V._7;

using DevExpress.XtraEditors.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.Utils.Win;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Columns;
using CNET_V7_Domain.Misc.PmsDTO;
using CNET_V7_Domain.Domain.ViewSchema;
using ProcessManager;
using CNET_V7_Domain.Domain.SettingSchema;
using CNET.FrontOffice_V._7.PMS.Common_Classes;
using CNET_V7_Domain.Domain.TransactionSchema;
using System.Diagnostics;
using DevExpress.XtraSpreadsheet.Model.History;
using DevExpress.CodeParser;
using CNET_V7_Domain.Misc;

namespace CNET.FrontOffice_V._7.Forms.Non_Navigatable_Modals
{
    public partial class frmProfileAmendment : UILogicBase
    {
        RegistrationListVMDTO registrationExt;

        private const int CP_NOCLOSE_BUTTON = 0x200;
        private int adCode;
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams myCp = base.CreateParams;
                myCp.ClassStyle = myCp.ClassStyle | CP_NOCLOSE_BUTTON;
                return myCp;
            }
        }

        internal RegistrationListVMDTO RegistrationExt
        {
            get { return registrationExt; }
            set
            {
                registrationExt = value;

            }
        }


        /************************ CONSTRUCTOR *************************/
        public frmProfileAmendment()
        {
            InitializeComponent();
            InitializeUI();
        }


        #region Helper Methods

        private void InitializeUI()
        {
            this.StartPosition = FormStartPosition.CenterScreen;


            //Guest
            GridColumn columnGuest = leGuest.Properties.View.Columns.AddField("Id");
            columnGuest.Visible = false;
            columnGuest = leGuest.Properties.View.Columns.AddField("Code");
            columnGuest.Visible = true;
            columnGuest = leGuest.Properties.View.Columns.AddField("FirstName");
            columnGuest.Caption = "First Name";
            columnGuest.Visible = true;
            columnGuest = leGuest.Properties.View.Columns.AddField("SecondName");
            columnGuest.Caption = "Middle Name";
            columnGuest.Visible = true;
            columnGuest = leGuest.Properties.View.Columns.AddField("ThirdName");
            columnGuest.Caption = "Last Name";
            columnGuest.Visible = true;
            columnGuest = leGuest.Properties.View.Columns.AddField("BioId");
            columnGuest.Caption = "IdNumber";
            columnGuest.Visible = true;
            leGuest.Properties.DisplayMember = "FirstName";
            leGuest.Properties.ValueMember = "Id";

            //Contact
            GridColumn columnContact = leContact.Properties.View.Columns.AddField("Id");
            columnContact.Visible = false;
            columnContact = leContact.Properties.View.Columns.AddField("Code");
            columnContact.Visible = true;
            columnContact = leContact.Properties.View.Columns.AddField("FirstName");
            columnContact.Caption = "Trade Name";
            columnContact.Visible = true;
            columnContact = leContact.Properties.View.Columns.AddField("idNumber");
            columnContact.Visible = true;
            leContact.Properties.DisplayMember = "FirstName";
            leContact.Properties.ValueMember = "Id";

            //Source
            GridColumn columnSource = leSource.Properties.View.Columns.AddField("Id");
            columnSource.Visible = false;
            columnSource = leSource.Properties.View.Columns.AddField("Code");
            columnSource.Visible = true;
            columnSource = leSource.Properties.View.Columns.AddField("FirstName");
            columnSource.Caption = "First Name";
            columnSource.Visible = true;
            columnSource = leSource.Properties.View.Columns.AddField("SecondName");
            columnSource.Caption = "Middle Name";
            columnSource.Visible = true;
            columnSource = leSource.Properties.View.Columns.AddField("Tin");
            columnSource.Visible = true;
            leSource.Properties.DisplayMember = "FirstName";
            leSource.Properties.ValueMember = "Id";

            //Agent
            GridColumn columnAgent = leAgent.Properties.View.Columns.AddField("Id");
            columnAgent.Visible = false;
            columnAgent = leAgent.Properties.View.Columns.AddField("Code");
            columnAgent.Visible = true;
            columnAgent = leAgent.Properties.View.Columns.AddField("FirstName");
            columnAgent.Caption = "First Name";
            columnAgent.Visible = true;
            columnAgent = leAgent.Properties.View.Columns.AddField("Tin");
            columnAgent.Visible = true;
            leAgent.Properties.DisplayMember = "FirstName";
            leAgent.Properties.ValueMember = "Id";

            //Group
            GridColumn columnGroup = leGroup.Properties.View.Columns.AddField("Id");
            columnGroup.Visible = false;
            columnGroup = leGroup.Properties.View.Columns.AddField("Code");
            columnGroup.Visible = true;
            columnGroup = leGroup.Properties.View.Columns.AddField("FirstName");
            columnGroup.Caption = "First Name";
            columnGroup.Visible = true;
            columnGroup = leGroup.Properties.View.Columns.AddField("Tin");
            columnGroup.Visible = true;
            leGroup.Properties.DisplayMember = "FirstName";
            leGroup.Properties.ValueMember = "Id";

            //Company
            GridColumn columnCompany = leCompany.Properties.View.Columns.AddField("Id");
            columnCompany.Visible = false;
            columnCompany = leCompany.Properties.View.Columns.AddField("Code");
            columnCompany.Visible = true;
            columnCompany = leCompany.Properties.View.Columns.AddField("FirstName");
            columnCompany.Caption = "First Name";
            columnCompany.Visible = true;
            columnCompany = leCompany.Properties.View.Columns.AddField("Tin");
            columnCompany.Visible = true;
            leCompany.Properties.DisplayMember = "FirstName";
            leCompany.Properties.ValueMember = "Id";
        }

        private bool InitializeData()
        {

            try
            {
                if (RegistrationExt == null) return false;

                // Progress_Reporter.Show_Progress("loading consignees");



                //check workflow

                ActivityDefinitionDTO workFlow = UIProcessManager.GetActivityDefinitionBydescriptionandreference(CNETConstantes.LU_ACTIVITY_DEFINATION_ProfileAmended, CNETConstantes.REGISTRATION_VOUCHER).FirstOrDefault();

                if (workFlow != null)
                {

                    adCode = workFlow.Id;
                }
                else
                {
                    ////CNETInfoReporter.Hide();
                    SystemMessage.ShowModalInfoMessage("Please define workflow of PROFILE AMENDED for Registration Voucher ", "ERROR");
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

                GetAllGuests();
                GetAllContacts();
                GetAllSources();
                GetAllAgents();
                GetAllGroups();
                GetAllCompanies();

                //Assigned consignee and other consignee
                leGuest.EditValue = RegistrationExt.GuestId;
                leCompany.EditValue = RegistrationExt.CompanyId;
                leContact.EditValue = RegistrationExt.ContactId;
                leAgent.EditValue = RegistrationExt.AgentId;
                leGroup.EditValue = RegistrationExt.GroupId;
                leSource.EditValue = RegistrationExt.SourceId;


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

        //vw_ConsigneeView
        private void GetAllCompanies()
        {
            leCompany.Properties.DataSource = LocalBuffer.LocalBuffer.ConsigneeViewBufferList.Where(per => per.GslType == CNETConstantes.CUSTOMER).ToList();
        }
        private void GetAllGroups()
        {
            leGroup.Properties.DataSource = LocalBuffer.LocalBuffer.ConsigneeViewBufferList.Where(per => per.GslType == CNETConstantes.GROUP).ToList();
        }
        private void GetAllAgents()
        {
            leAgent.Properties.DataSource = LocalBuffer.LocalBuffer.ConsigneeViewBufferList.Where(per => per.GslType == CNETConstantes.AGENT).ToList();
        }
        private void GetAllSources()
        {
            leSource.Properties.DataSource = LocalBuffer.LocalBuffer.ConsigneeViewBufferList.Where(per => per.GslType == CNETConstantes.BUSINESSsOURCE).ToList();
        }
        private void GetAllGuests()
        {
            leGuest.Properties.DataSource = LocalBuffer.LocalBuffer.ConsigneeViewBufferList.Where(per => per.GslType == CNETConstantes.GUEST).ToList();
        }
        private void GetAllContacts()
        {
            leContact.Properties.DataSource = LocalBuffer.LocalBuffer.ConsigneeViewBufferList.Where(per => per.GslType == CNETConstantes.CONTACT).ToList();
        }

        #endregion


        #region Event Handlers





        private void bbiOk_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                DateTime? currentTime = UIProcessManager.GetServiceTime();
                bool isUpdate = true;
                int Consignee1 = -1;
                int Consignee2 = -1;
                int Consignee3 = -1;
                int Consignee4 = -1;
                int Consignee5 = -1;
                int Consignee6 = -1;
                var response = UIProcessManager.GetVoucherBufferById(RegistrationExt.Id);
                VoucherBuffer voucherbuffer = response.Data;

                if (leGuest.EditValue != null)
                    Int32.TryParse(leGuest.EditValue.ToString(), out Consignee1);

                if (leCompany.EditValue != null)
                    Int32.TryParse(leCompany.EditValue.ToString(), out Consignee2);
                if (leAgent.EditValue != null)
                    Int32.TryParse(leAgent.EditValue.ToString(), out Consignee3);
                if (leSource.EditValue != null)
                    Int32.TryParse(leSource.EditValue.ToString(), out Consignee4);
                if (leContact.EditValue != null)
                    Int32.TryParse(leContact.EditValue.ToString(), out Consignee5);
                if (leGroup.EditValue != null)
                    Int32.TryParse(leGroup.EditValue.ToString(), out Consignee6);


                voucherbuffer.Voucher.Consignee1 = Consignee1 == -1 ? null : Consignee1;
                voucherbuffer.Voucher.Consignee2 = Consignee2 == -1 ? null : Consignee2;
                voucherbuffer.Voucher.Consignee3 = Consignee3 == -1 ? null : Consignee3;
                voucherbuffer.Voucher.Consignee4 = Consignee4 == -1 ? null : Consignee4;
                voucherbuffer.Voucher.Consignee5 = Consignee5 == -1 ? null : Consignee5;
                voucherbuffer.Voucher.Consignee6 = Consignee6 == -1 ? null : Consignee6;

                voucherbuffer.Activity = ActivityLogManager.SetupActivity(currentTime.Value, adCode, CNETConstantes.PMS_Pointer);

                if (voucherbuffer.TransactionReferencesBuffer != null && voucherbuffer.TransactionReferencesBuffer.Count > 0)
                    voucherbuffer.TransactionReferencesBuffer.ToList().ForEach(x => x.ReferencedActivity = null);

                voucherbuffer.TransactionCurrencyBuffer = null;

                ResponseModel<VoucherBuffer> updated = UIProcessManager.UpdateVoucherBuffer(voucherbuffer);

                if (updated != null && updated.Success)
                    SystemMessage.ShowModalInfoMessage("Profile Amendment is successfull!", "MESSAGE");
                else
                    SystemMessage.ShowModalInfoMessage("UPDATING NOT  SUCCESSFUL!" + Environment.NewLine + updated.Message, "ERROR");
            }
            catch (Exception ex)
            {
                SystemMessage.ShowModalInfoMessage("ERROR:: " + ex.Message, "ERROR");
            }
        }

        private void bbiClose_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            this.Close();
        }

        private void frmProfileAmendment_Load(object sender, EventArgs e)
        {
            if (!InitializeData())
            {
                this.Close();
            }
        }

        #endregion


        private void leGuest_AddNewValue(object sender, AddNewValueEventArgs e)
        {
            this.SubForm = this;
            frmPerson person = new frmPerson("Guest");
            person.Text = "Guest";
            person.GSLType = CNETConstantes.GUEST;
            person.rpgScanFingerPrint.Visible = true;
            person.LoadEventArg.Args = "Guest";
            person.LoadData(this, null);
            if (person.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ///GetPerson();
                if (person.SavedPerson != null)
                {
                    leGuest.Properties.DataSource = null;
                    GetAllGuests();
                    e.Cancel = false;
                    e.NewValue = person.SavedPerson.Id;
                }
            }
        }

        private void leAgent_AddNewValue(object sender, AddNewValueEventArgs e)
        {
            frmOrganization organization = new frmOrganization();
            organization.GslType = CNETConstantes.AGENT;
            this.SubForm = this;
            organization.Text = "Travel Agent";
            organization.LoadEventArg.Args = "Travel Agent";
            organization.LoadEventArg.Sender = null;
            organization.LoadData(this, null);
            if (organization.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ///GetPerson();
                if (organization.SavedOrg != null)
                {
                    leAgent.Properties.DataSource = null;
                    GetAllAgents();
                    e.Cancel = false;
                    e.NewValue = organization.SavedOrg.Id;
                }
            }
        }

        private void leContact_AddNewValue(object sender, AddNewValueEventArgs e)
        {
            frmPerson person = new frmPerson("Contact");
            this.SubForm = this;
            person.Text = "Contact";
            person.GSLType = CNETConstantes.CONTACT;
            person.LoadEventArg.Args = "Contact";
            person.LoadEventArg.Sender = null;
            person.LoadData(this, null);
            if (person.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ///GetPerson();
                if (person.SavedPerson != null)
                {
                    leContact.Properties.DataSource = null;
                    GetAllContacts();
                    e.Cancel = false;
                    e.NewValue = person.SavedPerson.Id;
                }
            }
        }

        private void leCompany_AddNewValue(object sender, AddNewValueEventArgs e)
        {
            frmOrganization organization = new frmOrganization();
            organization.GslType = CNETConstantes.CUSTOMER;
            this.SubForm = this;
            organization.Text = "Company";
            organization.LoadEventArg.Args = "Company";
            organization.LoadEventArg.Sender = null;
            organization.LoadData(this, null);
            if (organization.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ///GetPerson();
                if (organization.SavedOrg != null)
                {
                    leCompany.Properties.DataSource = null;
                    GetAllCompanies();
                    e.Cancel = false;
                    e.NewValue = organization.SavedOrg.Id;
                }
            }
        }

        private void leGroup_AddNewValue(object sender, AddNewValueEventArgs e)
        {

            this.SubForm = this;
            frmOrganization organization = new frmOrganization();
            organization.GslType = CNETConstantes.GROUP;
            organization.Text = "Group";
            organization.LoadEventArg.Args = "Group";
            organization.LoadEventArg.Sender = null;
            organization.LoadData(this, null);
            if (organization.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ///GetPerson();
                if (organization.SavedOrg != null)
                {
                    leGroup.Properties.DataSource = null;
                    GetAllGroups();
                    e.Cancel = false;
                    e.NewValue = organization.SavedOrg.Id;
                }
            }
        }

        private void leSource_AddNewValue(object sender, AddNewValueEventArgs e)
        {

            frmOrganization organization = new frmOrganization();
            organization.GslType = CNETConstantes.BUSINESSsOURCE;
            this.SubForm = this;
            organization.Text = "Source";
            organization.LoadEventArg.Args = "Source";
            organization.LoadEventArg.Sender = null;
            organization.LoadData(this, null);
            if (organization.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ///GetPerson();
                if (organization.SavedOrg != null)
                {
                    leSource.Properties.DataSource = null;
                    GetAllSources();
                    e.Cancel = false;
                    e.NewValue = organization.SavedOrg.Id;

                }
            }
        }







    }
}
