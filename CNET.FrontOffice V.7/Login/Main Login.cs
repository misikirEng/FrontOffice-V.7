using CNET.API.Manager;
using CNET.FP.Tool;
using CNET.FrontOffice_V._7.PMS.Common_Classes;
using CNET.Progress.Reporter;
using CNET_V7_Domain.Domain.SecuritySchema;
using CNET_V7_Domain.Misc;
using DevExpress.XtraEditors;
using ProcessManager;
using static CNET_V7_Domain.Misc.CommonTypes.CNET_ENUMS;

namespace CNET.FrontOffice_V._7
{

    public partial class MainLogin : XtraForm
    {

        public MainLogin()
        {
            InitializeComponent();
            lblCompanyName.Text = "";
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            if (Validate_Login())
            {
                UserDTO SelectedUser = UsersList.FirstOrDefault(x => x.Id == Convert.ToInt32(cbUsername.EditValue.ToString()));
                var authenticateResponse = await SystemInitalize.authenticate(SelectedUser.UserName, txtPassword.Text, system_InitalizeDTO.device.Id, system_InitalizeDTO.company.Tin);
                if (authenticateResponse.Success)
                {
                    LicenseAlert LA = new LicenseAlert();

                    if (!LA.CheckLicense())
                        LA.ShowDialog();

                    LocalBuffer.LocalBuffer.CurrentLoggedInUserEmployeeName = authenticateResponse.Data.personInfo.FirstName + " " + authenticateResponse.Data.personInfo.SecondName + " " + authenticateResponse.Data.personInfo.ThirdName;
                    LocalBuffer.LocalBuffer.CurrentLoggedInUser = SelectedUser;
                    IsAuthenticated = true;
                    this.Hide();


                    LocalBuffer.LocalBuffer.GetUserRoleAccess();
                    MasterPageForm Master = new MasterPageForm();
                    Master.FormClosed += Master_FormClosed;
                    Master.Show();
                }
                else
                {
                    SystemMessage.ShowModalInfoMessage(authenticateResponse.Message, "ERROR");
                    txtPassword.Text = null;
                }
                //else
                //{
                //
                //}
            }
        }

        private void Master_FormClosed(object? sender, FormClosedEventArgs e)
        {
            this.Close();
        }

        public bool IsAuthenticated { get; set; }
        ResponseModel<SystemInitDTO> system_InitalizeDTOResponse { get; set; }
        SystemInitDTO system_InitalizeDTO { get; set; }
        List<UserDTO> UsersList { get; set; }
        private void MainLogin_Load(object sender, EventArgs e)
        {
            Progress_Reporter.Show_Progress("Initializing Login", "Please Wait........");
            string deviceName = System.Windows.Forms.SystemInformation.ComputerName;

            // MessageBox.Show("Device Name =("+ deviceName + ")");
            //"YohannesGuestHouse"
            var SysInitializationResponse = new SysInitializationRequest(deviceName, null, ((int)PLATFORM_TYPE.PLATFORM_TYPE_DESKTOP), false);
            try
            {
                system_InitalizeDTOResponse = Task.Run(async () => await SystemInitalize.SysInitialize(SysInitializationResponse)).Result;
            }
            catch (Exception io)
            {
                if (io.Message == "device name not found!")
                    SystemMessage.ShowModalInfoMessage("Failed to System Initalize !!" + Environment.NewLine + io.Message + Environment.NewLine + "Device Name = (" + deviceName + ")", "ERROR");
                else
                    SystemMessage.ShowModalInfoMessage("Failed to System Initalize !!" + Environment.NewLine + io.Message, "ERROR");

                this.Close();
                return;
            }
            if (system_InitalizeDTOResponse == null)
            {
                SystemMessage.ShowModalInfoMessage("Failed to System Initalize API !!", "ERROR");
                this.Close();
                return;
            }
            if (!system_InitalizeDTOResponse.Success)
            {
                // SystemMessage.ShowModalInfoMessage("SysInitialize Error" + Environment.NewLine+system_InitalizeDTOResponse.Message, "ERROR");
                if (system_InitalizeDTOResponse.Message == "device name not found!")
                    SystemMessage.ShowModalInfoMessage("Failed to System Initalize !!" + Environment.NewLine + system_InitalizeDTOResponse.Message + Environment.NewLine + "Device Name = (" + deviceName + ")", "ERROR");
                else
                    SystemMessage.ShowModalInfoMessage("Failed to System Initalize !!" + Environment.NewLine + system_InitalizeDTOResponse.Message, "ERROR");

                this.Close();
                return;
            }


            DateTime? ServerDatetime = UIProcessManager.GetServiceTime();

            if (ServerDatetime == null)
            {
                SystemMessage.ShowModalInfoMessage("Failed to Get Server Date Time !!", "ERROR");
                this.Close();
                return;
            }





            DateTime ComputerdateTime = DateTime.Now;



            string UTCDateandTime = ("UTC Date and Time: " + DateTime.UtcNow);


            TimeZoneInfo localTimeZone = TimeZoneInfo.Local;
            string TimeZone = ("Time Zone: " + localTimeZone.DisplayName);
            string StandardTimeName = ("Standard Time Name: " + localTimeZone.StandardName);
            string DaylightTimeName = ("Daylight Time Name: " + localTimeZone.DaylightName);




            if (ServerDatetime.Value.Date != ComputerdateTime.Date)
            {
                SystemMessage.ShowModalInfoMessage("There is a Difference b/n Server and Computer Date !!" + Environment.NewLine
                    + "Please Fix Date and Time Zone!!" + Environment.NewLine
                    + "Server Date Time is :- " + ServerDatetime.Value.ToString("MM/dd/yyyy HH:mm:ss") + Environment.NewLine
                    + "Computer Date Time is :- " + ComputerdateTime.ToString("MM/dd/yyyy HH:mm:ss"), "ERROR");
                this.Close();
                return;
            }


            system_InitalizeDTO = system_InitalizeDTOResponse.Data;
            lblCompanyName.Text = system_InitalizeDTO.company != null ? system_InitalizeDTO.company.FirstName : "Company Name";
            bool Valid = Check_License(system_InitalizeDTO.company.Tin);

            if (system_InitalizeDTO.company.MaritalStatus != null && system_InitalizeDTO.company.MaritalStatus == 2050)
            {
                LocalBuffer.LocalBuffer.SystemProviderCNET = true;
                lblVersion.Text = "Version 6.0";
                pcRedcloudLogo.Image = FrontOffice_V._7.Properties.Resources.cnetLogoEd4;
            }
            else
            {
                lblVersion.Text = "Version 1.0";
                pcRedcloudLogo.Image = FrontOffice_V._7.Properties.Resources.REDLogo;
            }

            //if (system_InitalizeDTO.company != null && system_InitalizeDTO.company.TransactionLimit != null && system_InitalizeDTO.company.TransactionLimit > 0)
            //{
            //    lblVersion.Text = "Version  " + system_InitalizeDTO.company.TransactionLimit;
            //    LocalBuffer.LocalBuffer.SystemVersion  = system_InitalizeDTO.company.TransactionLimit;
            //}


            if (!Valid)
            {
                this.Close();
                return;
            }


            if (system_InitalizeDTO.device.Remark != null && system_InitalizeDTO.device.Remark == "UPDATE" && system_InitalizeDTO.device.MachineName != "DESKTOP-JDV9NVL")
            {
                XtraMessageBox.Show("System Has Update !!", "ERP V6", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Dispatcher.StartUpdate();
                this.Close();
                return;
            }

            Progress_Reporter.Show_Progress("Getting Local System Buffer", "Please Wait........");
            LocalBuffer.LocalBuffer.LoadLocalBuffer();
            Progress_Reporter.Close_Progress();

            if (system_InitalizeDTO.users != null)
                UsersList = system_InitalizeDTO.users.ToList();

            cbUsername.Properties.DataSource = UsersList.Where(x => x.IsActive);
        }
        private bool Check_License(string TIN)
        {
            try
            {
                var response = UIProcessManager.Read_All_Licenses(system_InitalizeDTO.company.Tin);

                if (response == null)
                {
                    XtraMessageBox.Show("Failed To Read Sytem Licenses!", "ERP V6", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                if (!response.Success || response.Data == null)
                {
                    XtraMessageBox.Show("Failed To Read Sytem Licenses!\n" + response.Message, "ERP V6", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                if (response.Data.Any(x => x.SystemId == SystemConstants.Property_Management_System_SUB_SYSTEM && x.LicenseIsValid))
                    return true;
                else
                {
                    XtraMessageBox.Show("Your Product Is Not Licensed!", "ERP V6", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Validating Licnese Threw An Exception!\n" + ex.Message, "ERP V6", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
        public bool Validate_Login()
        {
            bool valid = false;
            if (cbUsername.EditValue != null && !string.IsNullOrEmpty(cbUsername.EditValue.ToString()) && !string.IsNullOrEmpty(txtPassword.Text))
            {
                valid = true;
            }
            return valid;
        }

        private void txtPassword_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnLogin.PerformClick();
            }
        }

        private void lblCompanyName_Click(object sender, EventArgs e)
        {

        }
    }


}
