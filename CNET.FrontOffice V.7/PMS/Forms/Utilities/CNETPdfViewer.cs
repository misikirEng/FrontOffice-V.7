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

namespace CNET.FrontOffice_V._7.Forms.Utilities
{
    public partial class CNETPdfViewer : Form
    {
        public CNETPdfViewer()
        {
            InitializeComponent();
        }

        public void LoadPdf(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                pdfViewer1.LoadDocument(path);
            }
            else
            {
                XtraMessageBox.Show("Invalid Path!", "CNET_v2016", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
    }
}
