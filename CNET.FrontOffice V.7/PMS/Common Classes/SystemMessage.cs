using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNET.FrontOffice_V._7.PMS.Common_Classes
{
    public class SystemMessage
    {
        public static void ShowModalInfoMessage(string message, string type = "MESSAGE", String caption = null)
        {
            String _Caption = String.Empty;

            if (type == "ERROR")
            {
                if (String.IsNullOrEmpty(caption))
                    _Caption = "Error";
                else
                    _Caption = caption;

                XtraMessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (type == "MESSAGE")
            {

                if (String.IsNullOrEmpty(caption))
                    _Caption = "Information";
                else
                    _Caption = caption;

                XtraMessageBox.Show(message, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
