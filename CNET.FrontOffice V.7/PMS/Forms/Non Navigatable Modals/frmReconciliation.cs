using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using CNET_V7_Domain.Misc.PmsDTO;

namespace CNET.FrontOffice_V._7.Forms.Non_Navigatable_Modals
{
    public partial class frmReconciliation : DevExpress.XtraEditors.XtraForm
    {
        public frmReconciliation()
        {
            InitializeComponent();
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            gv_recon.Columns[3].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            gv_recon.Columns[3].AppearanceCell.Options.UseTextOptions = true;
        }
        string SelectedVoucher;
        List<VoucherReconcilationDTO> conciledReport;
        internal List<VoucherReconcilationDTO> ConciledReport
        {
            get { return conciledReport; }
            set
            {
                conciledReport = value;
                gc_recon.DataSource = conciledReport;

            }
        }
    }
}