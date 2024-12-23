using CNET.FrontOffice_V._7;
using PMS_Dashboard;
using PMSReport;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CNET.ERP.Client.UI_Logic.PMS.Forms.Reports
{
    public partial class Reportfrm  : UILogicBase
    {
       frmPMSReports reports = null;
       public Reportfrm(bool isNightAudit, PMSDashboard pmsDashboard = null)
        {
            InitializeComponent();
            reports = new frmPMSReports(isNightAudit, pmsDashboard);
            reports.TopMost = false;
            reports.TopLevel = false;
            reports.Dock = DockStyle.Fill;
            reports.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            panelControl1.Controls.Add(reports);
            reports.Show();
        }
    }
}
