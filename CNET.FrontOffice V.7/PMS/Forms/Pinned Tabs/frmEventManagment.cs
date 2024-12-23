using ERP.EventManagement;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CNET.FrontOffice_V._7.Forms.Pinned_Tabs
{
    public partial class frmEventManagment : UILogicBase
    {
        public frmEventManagment(FromOpenedType EventTypeValue = FromOpenedType.EventManagement)
        {
            InitializeComponent();

            if (EventTypeValue == FromOpenedType.EventManagement)
            {
                EventMgtForm frmEventMgt = new EventMgtForm();
                pnlMain.Controls.Clear();
                pnlMain.Controls.Add(frmEventMgt);
                frmEventMgt.Dock = DockStyle.Fill;

            }
            else if (EventTypeValue == FromOpenedType.EventDocumentBrowser)
            {
                //frmEventDocumentBrowser frmEventMgt = new frmEventDocumentBrowser();
                //pnlMain.Controls.Clear();
                //pnlMain.Controls.Add(frmEventMgt);
                //frmEventMgt.Dock = DockStyle.Fill;
                //this.Text = "Event Document Browser";
            }
            else if (EventTypeValue == FromOpenedType.EventReports)
            {
                //frmReports frmEventMgt = new frmReports();
                //pnlMain.Controls.Clear();
                //pnlMain.Controls.Add(frmEventMgt);
                //frmEventMgt.Dock = DockStyle.Fill;
                //this.Text = "Event Reports";
            }
        }

        private void frmEventManagment_Load(object sender, EventArgs e)
        {

        }
        public enum FromOpenedType
        {
            EventManagement,
            EventDocumentBrowser,
            EventReports

        }
    }
}
