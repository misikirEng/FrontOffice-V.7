using CNET.FrontOffice_V._7.Infra;

using CNET_PostingRoutine.Journalization;
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

namespace CNET.FrontOffice_V._7.Journalization
{
    public partial class frmPostingRoutine : UILogicBase
    {
        public frmPostingRoutine(PostingRoutineHeader postingRoutineHeader)
        {
            InitializeComponent();

            

            JournalizeWizard wizard = new JournalizeWizard(postingRoutineHeader, false);
            wizard.Dock = DockStyle.Fill;
            mainPanel.Controls.Add(wizard);
        }
    }
}
