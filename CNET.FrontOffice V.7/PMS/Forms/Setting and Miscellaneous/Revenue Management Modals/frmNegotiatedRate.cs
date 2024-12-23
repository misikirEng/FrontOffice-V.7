using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;

using CNET.FrontOffice_V._7.PMS.Contracts;
using CNET.FrontOffice_V._7;
using CNET.ERP.Client.Common.UI;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Columns;
using CNET_V7_Domain.Domain.SettingSchema;
using ProcessManager;
using CNET.FrontOffice_V._7.PMS.Common_Classes;
using CNET_V7_Domain.Domain.ViewSchema;
using CNET_V7_Domain.Domain.PmsSchema;
using CNET.FrontOffice_V._7.Validation;
using CNET_V7_Domain.Domain.ConsigneeSchema;

namespace CNET.FrontOffice_V._7.Forms.Setting_and_Miscellaneous.Revenue_Management_Modals
{
    public partial class frmNegotiatedRate : XtraForm//UILogicBase
    {
        public Boolean IsThisEdit;

        private NegotiatedViewVM _editedNegotiatedRate;

        /** Properties **/
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
        public int RateHeaderCode { get; set; }
        //public string AdSyncout { get; set; }
        internal NegotiatedViewVM EditedNegotiatedRate
        {
            get { return _editedNegotiatedRate; }
            set
            {
                _editedNegotiatedRate = value;

            }
        }

        //********** CONSTRUCTOR *****************//
        public frmNegotiatedRate()
        {
            InitializeComponent();
            InitializeUI();

        }


        #region Helper Methods
        public void InitializeUI()
        {
            Utility.AdjustForm(this);

            //gsl types
            cacType.Properties.Columns.Add(new LookUpColumnInfo("Description", "Type"));
            cacType.Properties.DisplayMember = "Description";
            cacType.Properties.ValueMember = "Id";

            //Consignee   
            GridColumn columnCompany = cacConsignee.Properties.View.Columns.AddField("Id");
            columnCompany.Visible = true;
            columnCompany = cacConsignee.Properties.View.Columns.AddField("name");
            columnCompany.Visible = true;
            columnCompany = cacConsignee.Properties.View.Columns.AddField("idNumber");
            columnCompany.Visible = true;
            cacConsignee.Properties.DisplayMember = "name";
            cacConsignee.Properties.ValueMember = "Id";
        }


        public bool InitializeData()
        {
            try
            {
                //gsl types
                List<SystemConstantDTO> types = LocalBuffer.LocalBuffer.SystemConstantDTOBufferList.Where(t => t.Id == CNETConstantes.GUEST || t.Id == CNETConstantes.CUSTOMER).ToList();
                cacType.Properties.DataSource = types;

                if (EditedNegotiatedRate != null)
                {
                    if (EditedNegotiatedRate.gslType == CNETConstantes.GUEST)
                    {
                        cacType.EditValue = CNETConstantes.GUEST;
                    }
                    if (EditedNegotiatedRate.gslType == CNETConstantes.CUSTOMER)
                    {
                        cacType.EditValue = CNETConstantes.CUSTOMER;
                    }
                    cacConsignee.EditValue = EditedNegotiatedRate.consignee;
                    IsThisEdit = true;
                    this.Text += " - Edit";
                }
                else
                {
                    this.Text += " - NEW";
                }

                return true;
            }
            catch (Exception ex)
            {
                ////CNETInfoReporter.Hide();
                SystemMessage.ShowModalInfoMessage("Error in initializing data. Detail: " + ex.Message, "ERROR");
                return false;
            }
        }

        private void GetAllGuest()
        {
            cacConsignee.Properties.DataSource = null;
            cacConsignee.Properties.DataSource = LocalBuffer.LocalBuffer.ConsigneeViewBufferList.Where(o => o.GslType == CNETConstantes.GUEST && o.IsActive).ToList(); ;
        }

        private void GetAllCompanies()
        {
            cacConsignee.Properties.DataSource = null;
            cacConsignee.Properties.DataSource = LocalBuffer.LocalBuffer.ConsigneeViewBufferList.Where(o => o.GslType == CNETConstantes.CUSTOMER && o.IsActive).ToList(); ;
        }

        #endregion


        #region Event Handlers

        private void bbiOk_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                List<Control> controls = new List<Control>();
                controls.Add(cacType);
                controls.Add(cacConsignee);

                IList<Control> invalidControls = CustomValidationRule.Validate(controls);

                if (invalidControls.Count > 0)
                    return;

                // Progress_Reporter.Show_Progress("Saving Negotiated Rate");

                NegotiationRateDTO negoRate = new NegotiationRateDTO();
                negoRate.Consignee = Convert.ToInt32(cacConsignee.EditValue.ToString());
                negoRate.RateCode = RateHeaderCode;

                if (IsThisEdit)
                {

                    negoRate.Id = EditedNegotiatedRate.Id;
                    if (UIProcessManager.UpdateNegotiatedRate(negoRate) != null)
                    {
                        ////CNETInfoReporter.Hide();
                        DialogResult = System.Windows.Forms.DialogResult.OK;
                        this.Close();
                    }
                }
                else
                {

                    if (UIProcessManager.CreateNegotiationRate(negoRate) != null)
                    {
                        DialogResult = System.Windows.Forms.DialogResult.OK;

                    }
                }
                cacType.EditValue = "";
                cacConsignee.EditValue = "";

                ////CNETInfoReporter.Hide();

            }
            catch (Exception ex)
            {
                ////CNETInfoReporter.Hide();
                SystemMessage.ShowModalInfoMessage("Error in saving negotiated rate. Detail: " + ex.Message, "ERROR");

            }
        }

        private void bbiClose_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Close();
        }

        private void bbiReset_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            cacType.EditValue = "";
            cacConsignee.EditValue = "";
        }

        private void frmNegotiatedRate_Load(object sender, EventArgs e)
        {
            if (!InitializeData())
            {
                this.Close();
            }
        }

        private void cacType_KeyDown(object sender, KeyEventArgs e)
        {
            LookUpEdit edit = sender as LookUpEdit;
            if (e.KeyData == Keys.Back || e.KeyData == Keys.Delete)
            {
                edit.ClosePopup();
                edit.EditValue = null;

            }
            e.Handled = true;
        }

        private void cacConsignee_KeyDown(object sender, KeyEventArgs e)
        {
            LookUpEdit edit = sender as LookUpEdit;
            if (e.KeyData == Keys.Back || e.KeyData == Keys.Delete)
            {
                edit.ClosePopup();
                edit.EditValue = null;

            }
            e.Handled = true;
        }

        private void cacType_EditValueChanged(object sender, EventArgs e)
        {
            if (cacType.EditValue != null && cacType.EditValue != "")
            {
                if ((int)cacType.EditValue == CNETConstantes.GUEST)
                {
                    GetAllGuest();
                }
                else
                {
                    GetAllCompanies();
                }

            }
        }

        #endregion








    }
}
