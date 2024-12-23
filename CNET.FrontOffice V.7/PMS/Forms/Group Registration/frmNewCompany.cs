
using CNET.FrontOffice_V._7.PMS.Common_Classes;
using CNET.FrontOffice_V._7.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CNET.FrontOffice_V._7.Group_Registration
{
    public partial class frmNewCompany : Form
    {

        public AddedCompany SavedCompany { get; set; }

        public frmNewCompany()
        {
            InitializeComponent();
        }

        #region Helper Methods
        

        private bool InitializeData()
        {
            try
            {
                SavedCompany = null;

                return true;
            }catch(Exception ex)
            {
                return false;
            }
        }

        #endregion


        #region Event Handlers

        private void bbiClose_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Hide();
        }

        #endregion

        private void bbiSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                List<Control> controls = new List<Control>
                {
                    teName,
                    teTIN,
                };

                IList<Control> invalidControls = CustomValidationRule.Validate(controls);

                if (invalidControls.Count > 0)
                {
                    return;
                }

                SavedCompany = new AddedCompany()
                {
                    Code = Guid.NewGuid().ToString(),
                    Name = teName.Text,
                    TIN = teTIN.Text,
                    PhoneNumber = tePhone.Text,
                    Email = teEmail.Text
                };

                teName.EditValue = null;
                teTIN.EditValue = null;
                tePhone.EditValue = null;
                teEmail.EditValue = null;

                DialogResult = DialogResult.OK;
                this.Hide();

            }catch(Exception ex)
            {
                SystemMessage.ShowModalInfoMessage("Exception in saving new Company. Detail::" + ex.Message, "ERROR");
            }
        }

        private void frmNewCompany_Load(object sender, EventArgs e)
        {
            if(!InitializeData())
            {
                DialogResult = DialogResult.Abort;
                this.Close();
            }
        }
    }

    public class AddedCompany
    {
        public int? Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string TIN { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
    }
}
