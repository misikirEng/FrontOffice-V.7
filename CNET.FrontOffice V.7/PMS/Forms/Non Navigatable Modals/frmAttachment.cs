using CNET.FrontOffice_V._7;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


using CNET.ERP.Client.Common.UI;


namespace CNET.FrontOffice_V._7.Non_Navigatable_Modals
{
    public partial class frmAttachment : UILogicBase
    {
        public RegistrationListVM RegExt { get; set; }

        public bool IsFromProfile { get; set; }
        public string ConsigneeId { get; set; }
        public int IntType { get; set; }

        public frmAttachment()
        {
            InitializeComponent();

        }

        private bool InitializeData()
        {
            if (RegExt == null && !IsFromProfile) return false;
            if (IsFromProfile)
            {
                this.Text = this.Text + " ( " + ConsigneeId + " )";
            }
            else
            {
                this.Text = this.Text + " ( " + RegExt.Registration + " )";
            }
            try
            {
                CNETInfoReporter.WaitForm("Loading Attachments", "Please Wait...");

                List<SystemConstantDTO> lookups = LocalBuffer.SystemConstantDTOBufferList.Where(l => l.Type.ToLower() == "attachment catagory").ToList();
                if (lookups != null)
                {
                    lookups = lookups.Where(c => c.code != CNETConstantes.ATTACHMENT_CATAGORY_COMPANYLOGO
                        && c.code != CNETConstantes.ATTACHMENT_CATAGORY_CATALOGUE
                        && c.code != CNETConstantes.ATTACHMENT_CATAGORY_MANUAL).ToList();
                }

                string code = "";
                if (IsFromProfile)
                {
                    code = ConsigneeId;
                }
                else
                {
                    code = RegExt.Registration;
                }

                atta = new CNET_Attachments(code,IntType, false, lookups);
                atta.Dock = DockStyle.Fill;
                pnl_attachment.Controls.Clear();
                pnl_attachment.Controls.Add(atta);
                CNETInfoReporter.Hide();
                return true;

            }
            catch (Exception ex)
            {
                CNETInfoReporter.Hide();
                SystemMessage.ShowModalInfoMessage("Error in initializing attachments. Detail: " + ex.Message, "ERROR");
                return false;
            }
        }

        private void frmAttachment_Load(object sender, EventArgs e)
        {
            if (!InitializeData())
            {
                this.Close();
            }
        }

        private void bbiNew_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (atta == null) return;
            if (IsFromProfile)
            {
                atta.OpenNewAttachment(ConsigneeId, IntType);
            }
            else
            {
                atta.OpenNewAttachment(RegExt.Registration, IntType);
            }
        }

        private void bbiDelete_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (atta == null) return;

            DialogResult dr = MessageBox.Show("Are you sure to delete this attachment?", "Attachment" , MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dr == System.Windows.Forms.DialogResult.No)
                return;
            if (IsFromProfile)
            {
                atta.DeleteAttachment(ConsigneeId);
            }
            else
            {
                atta.DeleteAttachment(RegExt.Registration);

            }
        }

        private void bbiClose_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Close();
        }
    }
}
