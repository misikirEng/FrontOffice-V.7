using CNET.API.Manager;
using CNET.FrontOffice_V._7.PMS.Common_Classes;
using CNET_V7_Domain.Domain.SecuritySchema;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CNET.FrontOffice_V._7
{
    public partial class frmNeedPassword : Form
    {
        public bool IsAutenticated { get; set; }
        public frmNeedPassword()
        {
            InitializeComponent();
            lkUser.Properties.DataSource = LocalBuffer.LocalBuffer.UserBufferList.Where(x => x.IsActive).ToList();
            lkUser.Properties.DisplayMember = "UserName";
            lkUser.Properties.ValueMember = "Id";

            lkUser.EditValue = LocalBuffer.LocalBuffer.CurrentLoggedInUser.Id;
            lkUser.ReadOnly = true;
        }
        public frmNeedPassword(bool verifyuser)
        {
            InitializeComponent();
            lkUser.Properties.DataSource = LocalBuffer.LocalBuffer.UserBufferList.Where(x => x.IsActive).ToList();
            lkUser.Properties.DisplayMember = "UserName";
            lkUser.Properties.ValueMember = "Id";

            lkUser.EditValue = LocalBuffer.LocalBuffer.CurrentLoggedInUser.Id;
            lkUser.ReadOnly = true;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            IsAutenticated = false;
            this.Close();
        }

        private async void btnAuthenticate_Click(object sender, EventArgs e)
        {
            IsAutenticated = false;
            if (Validate_Login())
            {
                UserDTO SelectedUser = LocalBuffer.LocalBuffer.UserBufferList.FirstOrDefault(x => x.Id == Convert.ToInt32(lkUser.EditValue.ToString()));
                var authenticateResponse = await SystemInitalize.authenticate(SelectedUser.UserName, txtPassword.Text, LocalBuffer.LocalBuffer.CurrentDevice.Id,  LocalBuffer.LocalBuffer.CompanyConsigneeData.Tin);
                if (authenticateResponse.Success)
                {
                    IsAutenticated = true;
                }
                else
                {
                    SystemMessage.ShowModalInfoMessage("Error !!"+ Environment.NewLine+ authenticateResponse.Message);
                }
                this.Close();
            }
        }
        public bool Validate_Login()
        {
            bool valid = false;
            if (lkUser.EditValue != null && !string.IsNullOrEmpty(lkUser.EditValue.ToString()) && !string.IsNullOrEmpty(txtPassword.Text))
            {
                valid = true;
            }
            else
                SystemMessage.ShowModalInfoMessage("Please Select User and Enter Password !!");

            return valid;
        }
    }
}
