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
using ProcessManager;
using CNET_V7_Domain.Misc;
using CNET_V7_Domain.Misc.CommonTypes;
using DevExpress.XtraScheduler.iCalendar.Components;

namespace CNET.FrontOffice_V._7
{
    public partial class LicenseAlert : DevExpress.XtraEditors.XtraForm
    {

        int alertdays = 15;
        private List<LicenseAlertDto> licenses { get; set; }
        public string message;
        public LicenseAlert()
        {
            InitializeComponent();
        }

        public bool CheckLicense()
        {
            bool HasInvalidLicense = false;
            bool HasExpiredLicense = false;
            bool HasWarningLicense = false;
            int MinExpirydate = 365;
            bool allgood = true;
            DateTime now = DateTime.Now;
            ResponseModel<List<LicenseDTO>> response = UIProcessManager.ReadLicense(LocalBuffer.LocalBuffer.CompanyConsigneeData.Tin);
            licenses = new List<LicenseAlertDto>();
            if (response != null && response.Success)
            {
                foreach (LicenseDTO li in response.Data)
                {
                    var laDto = new LicenseAlertDto();
                    laDto.subsystem = li.System;
                    if (li.LicenseIsValid)
                    {
                        int remainingdays = (li.ExpiryDate.Date - now.Date).Days;

                        if (MinExpirydate > remainingdays)
                            MinExpirydate = remainingdays;

                        if (remainingdays <= 0)
                        {
                            allgood = false;
                            HasExpiredLicense = true;
                            laDto.days = remainingdays;
                            laDto.Message = "EXPIRED";
                            laDto.IsValid = li.LicenseIsValid;
                        }
                        else if (remainingdays <= alertdays)
                        {
                            allgood = false;
                            HasWarningLicense = true;
                            laDto.days = remainingdays;
                            laDto.Message = "Near Expiry Date !!";
                            laDto.IsValid = li.LicenseIsValid;
                        }

                    }
                    else
                    {
                        allgood = false;
                        HasInvalidLicense = true;
                        laDto.days = 0;
                        laDto.Message =string.IsNullOrEmpty( li.Message) ? "EXPIRED" : li.Message;
                        laDto.IsValid = li.LicenseIsValid;
                    }

                    licenses.Add(laDto);
                }
            }
            else
                allgood = false;


            return allgood;
        }

        private void cbtMore_CheckedChanged(object sender, EventArgs e)
        {
            if (cbtMore.Checked)
            {
                cbtMore.Text = "Less";
                gCtrlLicenseAlert.Visible = true;
                gCtrlLicenseAlert.RefreshDataSource();
            }
            else
            {
                cbtMore.Text = "More";
                gCtrlLicenseAlert.Visible = false;
            }
        }

        private void LicenseAlert_Load(object sender, EventArgs e)
        {
            if (licenses != null && licenses.Count > 0)
            {
                string mainMsg = string.Empty;
                if (licenses.Any(l => l.days > 0))
                {
                    int minDay = licenses.Where(x => x.IsValid).Select(v => v.days).ToList().Min();
                    if (licenses.Any(l => l.days <= 0))
                    {
                        mainMsg = string.Format("are expired/invalid and \nsome others will expire in {0} day(s)!", minDay);
                    }
                    else
                    {
                        mainMsg = string.Format("will expire in {0} day(s)!", minDay);
                    }
                }
                else if (licenses.Any(l => l.days <= 0))
                {
                    mainMsg = "are expired/invalid!";
                }

                message = "Some of your licenses " + mainMsg;

                lblMessage.Text = message;
                gCtrlLicenseAlert.DataSource = licenses;
            }
        }

        private void gvwLicenseAlert_RowStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs e)
        {
            if (e.RowHandle >= 0)
            {
                bool val = (bool)gvwLicenseAlert.GetRowCellValue(e.RowHandle, "IsValid");
                if (val == false)
                {
                    e.Appearance.ForeColor = Color.Red;
                }

            }
        }
    }

    class LicenseAlertDto
    {
        public string subsystem { get; set; }
        public int days { get; set; }
        public string Message { get; set; }
        public bool IsValid { get; set; }
    }

}