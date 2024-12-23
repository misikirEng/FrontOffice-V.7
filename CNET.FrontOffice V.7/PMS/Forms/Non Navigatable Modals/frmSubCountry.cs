using CNET.ERP.Client.Common.UI;
using ProcessManager;
using CNET_V7_Domain.Domain.CommonSchema;
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

namespace CNET.FrontOffice_V._7.Forms.Non_Navigatable_Modals
{
    public partial class frmSubCountry : XtraForm
    {
        #region Declaration
        public static SubCountryDTO SavedCountry = null;
        #endregion

        #region Constractor
        public frmSubCountry()
        {
            InitializeComponent();
            List<CountryDTO> ListCountry = LocalBuffer.LocalBuffer.CountryBufferList;

            sleCountry.Properties.DataSource = ListCountry;
            sleCountry.Properties.DisplayMember = "Name";
            sleCountry.Properties.ValueMember = "Id";
        }
        #endregion

        #region Methods
        private void btnAddNew_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ClearControl();
        }
        private void ClearControl()
        {
            txtAlterName.Text = "";
            txtLongitude.Text = "";
            txtParent.Text = "";
            txtPopulation.Text = "";
            txtRemark.Text = "";
            txtSubCountryName.Text = "";
            txtSubType.Text = "";
            sleCountry.EditValue = null;
        }
        private void btnClose_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Close();
        }
        private void btnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (sleCountry.EditValue == null || string.IsNullOrEmpty(sleCountry.EditValue.ToString()))
            {
                XtraMessageBox.Show("Please Select Country First", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (txtSubCountryName.EditValue == null || string.IsNullOrEmpty(txtSubCountryName.EditValue.ToString()))
            {
                XtraMessageBox.Show("Please Write SubCountry Name", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (txtPopulation.EditValue != null && !string.IsNullOrEmpty(txtPopulation.EditValue.ToString()))
            {
                try
                {
                    int Population = Convert.ToInt32(txtPopulation.EditValue.ToString());
                }
                catch
                {
                    XtraMessageBox.Show("Please Write Population in a correct Number Format", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            if (txtLongitude.EditValue == null)
                txtLongitude.EditValue = "";
            if (txtLatitude.EditValue == null)
                txtLatitude.EditValue = "";
            decimal Longitude = 0;
            decimal.TryParse(txtLongitude.EditValue.ToString(), out Longitude);
            decimal Lattitude = 0;
            decimal.TryParse(txtLatitude.EditValue.ToString(), out Lattitude);

            SubCountryDTO NewSub = new SubCountryDTO
            {
                Country = sleCountry.EditValue == null ? 1 : Convert.ToInt32(sleCountry.EditValue),
                Name = txtSubCountryName.EditValue == null ? null : txtSubCountryName.EditValue.ToString(),
                AlternativeName = txtAlterName.EditValue == null ? null : txtAlterName.EditValue.ToString(),
                Type = txtSubType.EditValue == null ? null : Convert.ToInt32(txtSubType.EditValue),
                Longitude = Longitude,
                Lattitude = Lattitude,
                ParentId = txtParent.EditValue == null ? null : Convert.ToInt32(txtParent.EditValue.ToString()),
                Population = txtPopulation.Text == "" ? 0 : Convert.ToInt32(txtPopulation.EditValue.ToString()),
                Remark = txtRemark.Text
            };
            SavedCountry = UIProcessManager.CreateSubCountry(NewSub);

            ClearControl();
        }
        #endregion
    }
}
