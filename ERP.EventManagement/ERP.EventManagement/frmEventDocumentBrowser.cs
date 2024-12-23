using CNET.Client.Common;
using CNET.ERP.Client.Common.UI;
using DocumentBrowser;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CNET.ERP.EventManagement
{
    public partial class frmEventDocumentBrowser : UserControl
    {
        private VoucherDocument EventVoucherDocument = null;
        private VoucherDocument EventRequirementVoucherDocument = null; 

        public frmEventDocumentBrowser()
        {
            InitializeComponent();
            tcEventDocuments.SelectedTabPage = tpEventVoucher;
            tcEventDocuments_SelectedPageChanged(null, null);
        }

        private void tcEventDocuments_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            if (tcEventDocuments.SelectedTabPage == tpEventVoucher)
            {
                if (flageventvoucher) return;


                CNETInfoReporter.WaitForm("Loading " + tcEventDocuments.SelectedTabPage.Text + "...");
                EventVoucherDocument = new VoucherDocument(CNETConstantes.EVENT_VOUCHER.ToString());
                EventVoucherDocument.Dock = DockStyle.Fill;
                this.pcEventVoucher.Controls.Clear();
                this.pcEventVoucher.Controls.Add(EventVoucherDocument);
                flageventvoucher = true;
            }
            else if (tcEventDocuments.SelectedTabPage == tpEventRequirement)
            {
                if (flageventrequirementvoucher) return;

                CNETInfoReporter.WaitForm("Loading " + tcEventDocuments.SelectedTabPage.Text + "...");
                EventRequirementVoucherDocument = new VoucherDocument(CNETConstantes.EVENT_REQUIREMENT_VOUCHER.ToString());
                EventRequirementVoucherDocument.Dock = DockStyle.Fill;
                this.pcEventRequirement.Controls.Clear();
                this.pcEventRequirement.Controls.Add(EventRequirementVoucherDocument);
                flageventrequirementvoucher = true;

            }
            CNETInfoReporter.Hide();
        }

        public bool flageventvoucher { get; set; }

        public bool flageventrequirementvoucher { get; set; }

        private void frmEventDocumentBrowser_Load(object sender, EventArgs e)
        {
        }
    }
}
