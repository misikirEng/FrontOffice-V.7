using DevExpress.XtraPrinting;
using DevExpress.XtraReports.UI;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;

namespace V7TestApp
{
    public partial class XtraPrintOut : DevExpress.XtraReports.UI.XtraReport
    {
        public XtraPrintOut(IPrintable control1)
        {
            InitializeComponent();
            printableComponentContainer1.PrintableComponent = control1;
        }
    }
}
