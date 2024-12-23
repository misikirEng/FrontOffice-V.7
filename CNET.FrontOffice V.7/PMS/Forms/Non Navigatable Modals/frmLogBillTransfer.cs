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
    public partial class frmLogBillTransfer : Form
    {
        public frmLogBillTransfer(List<string> logMessages, List<string> failedRegList )
        {
            InitializeComponent();

            listBoxLogMessages.DataSource = logMessages;
            listBoxFailedReg.DataSource = failedRegList;

        }


        

        private void bbiClose_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Close();
        }
    }
}
