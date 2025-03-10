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
using CNET_V7_Domain.Misc.PmsDTO;
using CNET_V7_Domain.Domain.SettingSchema;
using ProcessManager;
using ERP.Attachement;
using CNET.Progress.Reporter;


namespace CNET.FrontOffice_V._7.Non_Navigatable_Modals
{
    public partial class frmAttachment : UILogicBase
    {
        public RegistrationListVMDTO RegExt { get; set; }

        public bool IsFromProfile { get; set; }
        public int ConsigneeId { get; set; }
        public int IntType { get; set; }

        private ERP_Attachments atta = null;

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
                Progress_Reporter.Show_Progress("Loading Attachments", "Please Wait...");

                List<SystemConstantDTO> lookups = LocalBuffer.LocalBuffer.SystemConstantDTOBufferList.Where(l => l.Category != null && l.Category.ToLower() == "attachment catagory").ToList();
                if (lookups != null)
                {
                    lookups = lookups.Where(c => c.Id != CNETConstantes.ATTACHMENT_CATAGORY_COMPANYLOGO
                        && c.Id != CNETConstantes.ATTACHMENT_CATAGORY_CATALOGUE
                        && c.Id != CNETConstantes.ATTACHMENT_CATAGORY_MANUAL).ToList();
                }

                int? code = null;
                if (IsFromProfile)
                {
                    code = ConsigneeId;
                }
                else
                {
                    FTPInterface.FTPAttachment.ORGUnitDefcode = LocalBuffer.LocalBuffer.CurrentDeviceConsigneeUnit.ToString();
                    code = RegExt.Id; 
                }

                atta = new ERP_Attachments(code.Value,IntType, false, lookups);
              
                atta.Dock = DockStyle.Fill;
                pnl_attachment.Controls.Clear();
                pnl_attachment.Controls.Add(atta);
                Progress_Reporter.Close_Progress();
                return true;

            }
            catch (Exception ex)
            {
                Progress_Reporter.Close_Progress();
                MessageBox.Show("Error in initializing attachments. Detail: " + ex.Message, "ERROR");
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
                atta.OpenNewAttachment(ConsigneeId,false, IntType);
            }
            else
            {
                atta.OpenNewAttachment(RegExt.Id,true, IntType);
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
                atta.DeleteAttachment(RegExt.Id);

            }
        }

        private void bbiClose_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Close();
        }
    }
}
