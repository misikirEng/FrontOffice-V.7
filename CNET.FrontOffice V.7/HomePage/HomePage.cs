using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using CNET.ERP.Client.Common.UI;
using DevExpress.XtraEditors;
using System.Reflection;
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraTab.ViewInfo;
using System.Threading;
using PMS_Dashboard;
using static CNET.FrontOffice_V._7.MasterPageForm;
using CNET_V7_Domain.Domain.SecuritySchema;
using ProcessManager;
using CNET_V7_Domain.Domain.ConsigneeSchema;
using CNET_V7_Domain.Domain.ViewSchema;
using CNET_V7_Domain.Domain.SettingSchema;
using DevExpress.DataAccess.Native.Web;
using CNET_V7_Domain.Misc;
using CNET_V7_Domain.Domain.CommonSchema;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Reflection.Emit;
using CNET.API.Manager;
using CNET_V7_Domain.Misc.CommonTypes;
using DevExpress.XtraCharts.Designer;
using Newtonsoft.Json;
using System.Net.Http;
using DevExpress.Map.Kml;

namespace CNET.FrontOffice_V._7
{

    public partial class HomePage : XtraForm
    {

        //CNET_EmailViewer emailViewer;
        private ActivityDefinitionDTO passWordChangeActivityDefn;

        ////Person person;
        //OrganizationUnitDefinition department;
        //OrganizationUnitDefinition organization;
        //UserRoleMapper role;
        //vw_AllPersonView personDetail;
        //List<RoleActivity> roleActivites;
        public static string gender { get; set; }
        //List<Configuration> mConfigurations = Authentication.ConfigurationBufferList.Where(l => l.reference == CNETConstantes.Security).ToList();
        public bool status;

        private string _componentCode = "";

        private SelectedDashboardTask _selectedDashboardTaskHandler = null;
        public SelectedDashboardTask SelectedDashboardTaskHandler
        {
            get
            {
                return _selectedDashboardTaskHandler;
            }
            set
            {
                _selectedDashboardTaskHandler = value;
            }
        }

        //public SelectedMessage SelectedMessageHandler { get; set; }

        DateTime dt = new DateTime();

        //private void ElappsedTime()
        //{
        //  // txtTimePassed.Text = dt.AddSeconds(counter).ToString("HH:mm:ss");
        //}

        public HomePage()
        {
            //ShowLogActivity();
            InitializeComponent();
            //_componentCode = componentCode;
            //ethiopian_Calender1.lblGR.TextChanged +=lblGR_TextChanged;
            //timer1.Interval = 1000;
            //timer1.Start();
            //if (Authentication.ActivityDefnBufferList != null)
            //{
            //    var acDefn = Authentication.ActivityDefnBufferList.Where(x => x.description == CNETConstantes.LUK_Password_Change).FirstOrDefault();
            //    if (acDefn != null)
            //    {
            //        passWordChangeActivityDefn = acDefn.code;
            //    }
            //}
            //cmbDateCriteria.Text = "Daily";
        }
        //  private VoucherDocument vouchDoc = null;
        private void PopulateVoucherDashboard()
        {
            //UserRoleMapper currentUserRole = Authentication.UserRoleMapperBufferList.FirstOrDefault(p => p.user == Authentication.GetAuthorizedUser().code);
            //if (currentUserRole != null)
            //{
            //    List<VoucherDashBoardPrivilege> allowedVoucherDashbrdPrivileges = UIProcessManager.VoucherDashBoardPrivilegeSelect(null, currentUserRole.role, null);
            //    if (allowedVoucherDashbrdPrivileges != null && allowedVoucherDashbrdPrivileges.Count > 0)
            //    {
            //        List<string> vouchDefns = allowedVoucherDashbrdPrivileges.Select(p => p.voucherDefinition).ToList();
            //        if (vouchDefns != null && vouchDefns.Count > 0)
            //        {
            //            vouchDoc = new VoucherDocument(vouchDefns, null, true, true, selectedDateCriteria);
            //            pnlVoucherDashboard.Controls.Clear();
            //            vouchDoc.Dock = DockStyle.Fill;
            //            pnlVoucherDashboard.Controls.Add(vouchDoc);
            //        }
            //    }
            //    else
            //    {
            //        xtabVoucherDashboard.PageEnabled = false;
            //        lciVoucherDashbrdMenu.Visibility = LayoutVisibility.Never;
            //    }
            //}
        }

        private DateTime currentDateTime { get; set; }
        private string selectedDateCriteria = "Daily";
        private void cmbDateCriteria_SelectedValueChanged(object sender, EventArgs e)
        {
            //if (cmbDateCriteria.Text != null)
            //{
            //    selectedDateCriteria = cmbDateCriteria.Text.ToString();
            //    currentDateTime = UIProcessManager.GetDataTime().Timestamp;
            //    lciFromDate.Visibility = LayoutVisibility.Always;
            //    lciToDate.Visibility = LayoutVisibility.Always;
            //    dtStartDate.Properties.ReadOnly = true;
            //    dtEndDate.Properties.ReadOnly = true;
            //    dtStartDate.Properties.ShowDropDown = ShowDropDown.Never;
            //    dtEndDate.Properties.ShowDropDown = ShowDropDown.Never;
            //    switch (selectedDateCriteria)
            //    {
            //        case "Daily":
            //            dtStartDate.EditValue = currentDateTime;
            //            dtEndDate.EditValue = currentDateTime;
            //            lciToDate.Visibility = LayoutVisibility.Never;
            //            break;
            //        case "Weekly":
            //            var weekStart = DayOfWeek.Monday;
            //            dtStartDate.EditValue = currentDateTime.AddDays(weekStart - currentDateTime.DayOfWeek); ;
            //            dtEndDate.EditValue = currentDateTime;
            //            break;
            //        case "Monthly":
            //            dtStartDate.EditValue = new DateTime(currentDateTime.Year, currentDateTime.Month, 1);
            //            dtEndDate.EditValue = currentDateTime;
            //            break;
            //        case "Annually":
            //            dtStartDate.EditValue = new DateTime(currentDateTime.Year, 1, 1);
            //            dtEndDate.EditValue = currentDateTime;
            //            break;
            //        case "At the day of":
            //            dtStartDate.EditValue = currentDateTime;
            //            dtEndDate.EditValue = currentDateTime;
            //            lciToDate.Visibility = LayoutVisibility.Never;
            //            dtStartDate.Properties.ReadOnly = false;
            //            dtStartDate.Properties.ShowDropDown = ShowDropDown.SingleClick;
            //            break;
            //        case "Date Range":
            //            dtStartDate.EditValue = currentDateTime;
            //            dtEndDate.EditValue = currentDateTime;
            //            dtStartDate.Properties.ReadOnly = false;
            //            dtEndDate.Properties.ReadOnly = false;
            //            dtStartDate.Properties.ShowDropDown = ShowDropDown.SingleClick;
            //            dtEndDate.Properties.ShowDropDown = ShowDropDown.SingleClick;
            //            break;
            //        case "Show All":
            //            dtStartDate.EditValue = null;
            //            dtEndDate.EditValue = null;
            //            lciFromDate.Visibility = LayoutVisibility.Never;
            //            lciToDate.Visibility = LayoutVisibility.Never;
            //            break;
            //    }
            //}
        }

        private static bool isPersonRecordLoaded { get; set; }
        //   private Dashboard persRecordDashbrd = null;
        private void PopulatePersonalRecords()
        {
            //persRecordDashbrd = new Dashboard();
            //pnlPersonalRecords.Controls.Clear();
            //persRecordDashbrd.Dock = DockStyle.Fill;
            //pnlPersonalRecords.Controls.Add(persRecordDashbrd);
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            PopulateVoucherDashboard();
        }

        private void btnShow_Click(object sender, EventArgs e)
        {
            //if (vouchDoc != null)
            //{
            //    vouchDoc.issuedDate = dtStartDate.EditValue != null ? dtStartDate.EditValue.ToString() : null;
            //    vouchDoc.issueDateEnd = dtEndDate.EditValue != null ? dtEndDate.EditValue.ToString() : null;
            //    vouchDoc.bbtnShow.PerformClick();
            //}
        }

        public Image byteArrayToImage(byte[] byteArrayIn)
        {
            Image returnImage = null;
            try
            {
                MemoryStream ms = new MemoryStream(byteArrayIn, 0, byteArrayIn.Length); ms.Write(byteArrayIn, 0, byteArrayIn.Length);
                returnImage = Image.FromStream(ms, true);
            }
            catch { }
            return returnImage;
        }

        private async Task UserInformation()
        {
            try
            {

                //role = new UserRoleMapper();
                //department = new OrganizationUnitDefinition();
                //organization = new OrganizationUnitDefinition();
                //personDetail = new vw_AllPersonView();
                //roleActivites = new List<RoleActivity>();
                //vw_AllPersonView person = LocalBuffer.LocalBuffer.AllPersonViewBufferList.FirstOrDefault(x => x.code == Authentication.GetAuthorizedUser().person);
                //if (person != null)
                //{
                //    personDetail = Authentication.AllPersonViewBufferList.FirstOrDefault(d => d.code == person.code);

                //    if (personDetail != null)
                //    {
                //        txtUserName.Text = String.Format("{0} {1} {2}", personDetail.Ltitle, personDetail.firstName, personDetail.middleName);
                //        txtEmployeeName.EditValue = String.Format("{0} {1}", personDetail.firstName, personDetail.middleName);
                //        Attachment attachment = await Task.Run(() => UIProcessManager.GetAttachmentByReference(person.code).FirstOrDefault(d => d.type == CNETConstantes.PICTURE));
                //        if (attachment != null)
                //        {
                //            Image currentImage = null;
                //            if (!string.IsNullOrEmpty(attachment.url) && File.Exists(attachment.url))
                //            {
                //                currentImage = Image.FromFile(attachment.url);
                //            }
                //            else if (attachment.file != null)
                //            {
                //                currentImage = byteArrayToImage(attachment.file);
                //            }
                //            if (currentImage != null)
                //            {
                //                picUser.Invoke((MethodInvoker)delegate
                //                {
                //                    picUser.EditValue = currentImage;
                //                });
                //                //.EditValue = currentImage;
                //            }
                //            else
                //            {
                //                if (person.gender == CNETConstantes.FEMALE)
                //                {
                //                    picUser.Invoke((MethodInvoker)delegate
                //                    {
                //                        picUser.EditValue = Properties.Resources.noprofileFemale; ;
                //                    });

                //                    //picUser.EditValue = Properties.Resources.noprofileFemale;
                //                }
                //                else
                //                {
                //                    picUser.Invoke((MethodInvoker)delegate
                //                    {
                //                        picUser.EditValue = Properties.Resources.noprofileMale;
                //                    });
                //                    // picUser.EditValue = Properties.Resources.noprofileMale;
                //                }
                //            }
                //            gender = person.gender;
                //        }
                //        else
                //        {
                //            if (person.gender == CNETConstantes.FEMALE)
                //            {
                //                picUser.Invoke((MethodInvoker)delegate
                //                {
                //                    picUser.EditValue = Properties.Resources.noprofileMale;
                //                });
                //                //picUser.EditValue = Properties.Resources.noprofileFemale;
                //            }
                //            else
                //            {
                //                picUser.Invoke((MethodInvoker)delegate
                //                {
                //                    picUser.EditValue = Properties.Resources.noprofileMale;
                //                });
                //                //picUser.EditValue = Properties.Resources.noprofileMale;
                //            }
                //            gender = person.gender;
                //        }
                //    }
                //    User user = new User();
                //    user = Authentication.GetAuthorizedUser();
                //    role = Authentication.UserRoleMapperBufferList.FirstOrDefault(r => r.user == user.code);
                //    if (role != null)
                //    {
                //        organization = Authentication.OrganizationUnitDefinitionBufferList.FirstOrDefault(X => X.code == role.role);
                //        string departmentCode = "";
                //        if (organization != null)
                //        {
                //            txtRole.Text = organization.description;
                //        }
                //        Lookup lookup = Authentication.SystemConstantDTOBufferList.FirstOrDefault(t => t.type == CNETConstantes.PERSON_RELATION_TYPE && t.description == "Department");
                //        if (lookup != null)
                //        {
                //            Relation relation = new Relation();
                //            relation = Authentication.RelationBufferList.FirstOrDefault(d => d.referringObject == person.code && d.relationType == lookup.code);
                //            if (relation != null)
                //            {
                //                departmentCode = relation.referenceObject;
                //            }

                //            department = Authentication.OrganizationUnitDefinitionBufferList.FirstOrDefault(X => X.code == departmentCode);
                //            if (department != null)
                //                txtDepartment.Text = department.description;
                //        }
                //    }
                //}
                //lebl1.Text = "Welcome to " + Authentication.clientName;
                //txtUser.EditValue = Authentication.GetAuthorizedUser().userName;
                //decimal assnedRole = 0;
                //decimal allRole = 0;
                //decimal acessLevle = 0;
                //assnedRole = Convert.ToDecimal(UIProcessManager.SelectAllAccessMatrix().Count(d => d.role == role.role));
                //allRole = Convert.ToDecimal(UIProcessManager.SelectAllFunctionality().Count());
                //if (allRole > 0 && assnedRole > 0)
                //    acessLevle = Decimal.Round((assnedRole / allRole) * 100, 2);
                //txtAccessLevel.Text = acessLevle + " %";
                //txtReleaseDate.Text = " 06-04-2019  (V6.0.1.21)";
            }
            catch
            {

            }
        }
        private void ShowException()
        {
            gc_ActivityLog.DataSource = null;
        }
        private void ShowCustomLog()
        {
            gc_ActivityLog.DataSource = null;
        }

        private async Task ShowLogActivity()
        {
            //if (Authentication.GetAuthorizedUser() != null)
            //{
            //    string authorizedUser = Authentication.GetAuthorizedUser().code;
            //    if (!string.IsNullOrEmpty(authorizedUser))
            //    {
            //        List<ActivityView> allActivity = await Task.Run(() => UIProcessManager.selectActivityViewByDateandUser(authorizedUser, DateTime.Now.ToString(), null));
            //        if (allActivity != null)
            //        {
            //            gc_ActivityLog.Invoke((MethodInvoker)delegate
            //            {
            //                gc_ActivityLog.DataSource = allActivity;
            //            });

            //        }
            //    }
            //}
        }



        private async Task ShowPasswordChangeLog()
        {

            if (LocalBuffer.LocalBuffer.CurrentLoggedInUser != null)
            {
                List<ActivityViewDTO> passwordChangeActivity = await Task.Run(() => UIProcessManager.GetActivityViewByUserandActivitDesctiption(LocalBuffer.LocalBuffer.CurrentLoggedInUser.Id, CNETConstantes.LUK_Password_Change));
                if (passwordChangeActivity != null)
                {
                    gc_passwordChangeLog.Invoke((MethodInvoker)delegate
                    {
                        gc_passwordChangeLog.DataSource = passwordChangeActivity;
                    });

                }
            }

        }
        public static bool changePasswordStatus { get; set; }
        public string UserName;
        public string Password;


        //private List<Device> device;
        //private List<DeviceView> devices;
        List<NotificationHeader> _datasources = new List<NotificationHeader>();

        private void clear()
        {
            if (txtNewPassword.InvokeRequired)
            {
                txtNewPassword.Invoke((MethodInvoker)delegate { txtNewPassword.Text = string.Empty; });
            }
            else
                txtNewPassword.Text = "";
            if (txtOldPassword.InvokeRequired)
                txtOldPassword.Invoke((MethodInvoker)delegate { txtOldPassword.Text = string.Empty; });
            else
                txtOldPassword.Text = "";
            if (txtConfirmPassword.InvokeRequired)
                txtConfirmPassword.Invoke((MethodInvoker)delegate { txtConfirmPassword.Text = string.Empty; });
            else
                txtConfirmPassword.Text = "";
        }


        private async void Home_Load(object sender, EventArgs e)
        {
            xtraTabControl1.AutoSize = true;
            await UserInformation();
            await Task.Run(() => PopulateCurrentDate());
            await Task.Run(() => PopulateCalender());
            ShowPmsDashboard();

            txtUserName.Text = LocalBuffer.LocalBuffer.CurrentLoggedInUserEmployeeName;
            lblCompanyWelcome.Text = "Welcome " + LocalBuffer.LocalBuffer.CompanyName;
            txtEmployeeName.Text = LocalBuffer.LocalBuffer.CurrentLoggedInUserEmployeeName;
            txtUser.Text = LocalBuffer.LocalBuffer.CurrentLoggedInUser.UserName;

            picUser.Image = LocalBuffer.LocalBuffer.CompanyDefaultLogo;

            var datetime = UIProcessManager.GetServiceTime();
            txtGregCalender.Text = datetime.Value.ToLongDateString();

            if (LocalBuffer.LocalBuffer.SystemProviderCNET)
            {
                LocalBuffer.LocalBuffer.SystemProviderCNET = true;
                txtSystemVersion.Text = "6.0";
                picCNETlogo.Image = FrontOffice_V._7.Properties.Resources.cnetLogoEd4;
            }
            else
            {
                txtSystemVersion.Text = "1.0";
                picCNETlogo.Image = FrontOffice_V._7.Properties.Resources.REDLogo;
            }


            UserRoleMapperDTO UserRole = UIProcessManager.GetUserRoleMapperByUser(LocalBuffer.LocalBuffer.CurrentLoggedInUser.Id);
            if (UserRole != null)
            {
                ConsigneeUnitDTO consigneeUnitDTO = UIProcessManager.GetConsigneeUnitById(UserRole.Role);
                if (consigneeUnitDTO != null)
                {
                    txtRole.EditValue = consigneeUnitDTO.Name;
                }
            }

            //currentDate.ToShortDateString();
            //if (_componentCode == CNETConstantes.PMS)
            //{
            //    ShowPmsDashboard();
            //}
            //else
            //{
            //    //xtabMessage.PageEnabled = false;
            //    xtabActivity.PageEnabled = true;
            //    xtabDashBoard.PageEnabled = false;
            //    xtraTabControl1.SelectedTabPage = xtabActivity;
            //}

            lbl_licenseExpires.Visible = false;
            lbl_licenseExpires.Text = string.Empty;

            //#region license checkup
            //try
            //{
            //    lbl_licenseExpires.Visible = false;
            //    lbl_licenseExpires.Text = string.Empty;
            //    if (Authentication.showExpireNotice)
            //    {
            //        lbl_licenseExpires.Visible = true;
            //        lbl_licenseExpires.Text = Authentication.getLicenseNoticeMessage();
            //    }
            //}
            //catch (Exception ex)
            //{
            //}
            //#endregion

            lblCompanyWelcome.Focus();

            //if (_componentCode == CNETConstantes.BACKOFFICECLIENT)
            //{
            //    LoadNotifications();
            //}
            //else
            //{
            //    xtabTaskNotification.PageEnabled = false;
            //}
            //if (CheckPlacementNotificationSecurity() && _componentCode == CNETConstantes.BACKOFFICECLIENT)
            //{
            //    Thread HRMSPlacementNotificationThread = new Thread(() =>
            //         {
            //             CheckandshowHRMSPlacement();
            //         }) { IsBackground = true };
            //    HRMSPlacementNotificationThread.Start();
            //}


           /* //For License Repot Purpose
            xtabCalender.PageEnabled = true;
            xtabCalender.Visible = true;
            xtabCalender.PageVisible = true;
            GetLicenseData();
           */
        }
        public void GetLicenseData()
        {

            ReportModel d = new ReportModel();
            d.Get();

            gridControl1.DataSource = d.reportdata;
        }

        public class ReportModel
        {
            public List<ReportLicenseDTO> reportdata = null;
            // https://localhost:7196
            Uri address = new Uri("http://196.191.244.136:7009/api");
            private HttpClient client;
            public void Get()
            {
                reportdata = new List<ReportLicenseDTO>();
                client = new HttpClient(); 
                client.BaseAddress = address;

                HttpResponseMessage response = client.GetAsync(address + "/License/GetCustomerLicense").Result;

                if (response.IsSuccessStatusCode)
                {
                    string data = response.Content.ReadAsStringAsync().Result;

                    var result = JsonConvert.DeserializeObject<ResponseModel<List<ReportLicenseDTO>>>(data);
                    if (result.Success)
                        reportdata = result.Data;
                }

            }
        }
        public class ReportLicenseDTO
        {
            public int CustomerId { get; set; }
            public string CustomerName { get; set; }
            public string TIN { get; set; }
            public string MRC { get; set; }

            public int SystemId { get; set; }

            public string System { get; set; }

            public string SubSystem { get; set; }

            public string Licensecode { get; set; }

            public string? Message { get; set; }
            public DateTime ExpiryDate { get; set; }
            public int RemainingDays { get; set; }

            public bool LicenseIsValid { get; set; }
        }
        public bool CheckPlacementNotificationSecurity()
        {
            try
            {
                //string currentRole = "";
                //var role = LocalBuffer.LocalBuffer.UserRoleMapperBufferList.Where(x => x.user == LocalBuffer.LocalBuffer.CurrentLoggedInUser.Id).FirstOrDefault();

                //if (role != null)
                //    currentRole = role.role;

                //String category = "Human Resource";

                //var functions = LocalBuffer.LocalBuffer.FuncWithAccessMatrix.Where(x => x.role == currentRole && x.category == category && x.access == true);

                //List<String> approvedFunctionalities = functions.Select(x => x.visuaCompDesc).ToList();

                //return approvedFunctionalities.Contains("Placement Notification");
                return false;
            }
            catch
            {
                return false;
            }
        }
        private void CheckandshowHRMSPlacement()
        {
            //case "Enable Placement Nofification":

            //          case "Placement Notify Before":
            //Configuration EnablePlacementNotificationForm = LocalBuffer.LocalBuffer.ConfigurationBufferList.FirstOrDefault(x => x.reference == "HRMS Setting" && x.attribute == "Enable Placement Nofification");
            //if (EnablePlacementNotificationForm != null && !string.IsNullOrEmpty(EnablePlacementNotificationForm.currentValue) && Convert.ToBoolean(EnablePlacementNotificationForm.currentValue))
            //{
            //    Configuration PlacementNotifyBeforeConfig = LocalBuffer.LocalBuffer.ConfigurationBufferList.FirstOrDefault(x => x.reference == "HRMS Setting" && x.attribute == "Placement Notify Before");

            //    PlacementNotification PlacementNotificationForm = new PlacementNotification();

            //    if (PlacementNotificationForm != null && !string.IsNullOrEmpty(PlacementNotifyBeforeConfig.currentValue))
            //    {
            //        try
            //        {
            //            PlacementNotificationForm.NotificationDays = Int32.Parse(PlacementNotifyBeforeConfig.currentValue);
            //        }
            //        catch
            //        {

            //        }
            //    }


            //    PlacementNotificationForm.GetPlacementForNotification();

            //    if (PlacementNotificationForm.PlacmentDTOList != null && PlacementNotificationForm.PlacmentDTOList.Count > 0)
            //    {
            //        PlacementNotificationForm.ShowDialog();
            //    }

            //}
        }


        private void PopulateCalender()
        {
            //List<ScheduleDTO> allSchedules = new List<ScheduleDTO>();
            //DateTime currentDate = UIProcessManager.GetDataTime().Timestamp;
            //List<ViewHolidayPeriod> holidays = UIProcessManager.GetHolidayPeriods();

            //List<ScheduleDTO> hDTO = (from hdays in holidays
            //                          where hdays.ForcastPeriodStart.HasValue && hdays.ForcastPeriodEnd.HasValue
            //                          select new ScheduleDTO
            //                          {
            //                              Description = hdays.HolidayDesc,
            //                              EndDate = new DateTime(hdays.ForcastPeriodEnd.Value.Year, hdays.ForcastPeriodEnd.Value.Month, hdays.ForcastPeriodEnd.Value.Day, 23, 59, 59, 999),
            //                              IsReccring = true,
            //                              StartDate = hdays.ForcastPeriodStart.Value,
            //                              Subject = "Holiday",
            //                              Type = ScheduleType.Holiday,
            //                              ScheduleColor = Brushes.MediumPurple
            //                          }).ToList();
            //allSchedules.AddRange(hDTO);
            //HRCalandar calnd = new HRCalandar(allSchedules);
            //pnlCalender.Invoke((MethodInvoker)delegate
            //{
            //    pnlCalender.Controls.Clear();
            //    calnd.Dock = DockStyle.Fill;
            //    pnlCalender.Controls.Add(calnd);
            //});

        }

        private void PopulateCurrentDate()
        {
            //DateTime currentDate = UIProcessManager.GetDataTime().Timestamp;

            //Period currentPeriod = Authentication.PeriodBufferList.Where(uip => uip.type == CNETConstantes.PERIOD_TYPE_ACCOUNTING &&
            //                uip.start <= Convert.ToDateTime(currentDate) && uip.end >= Convert.ToDateTime(currentDate)).FirstOrDefault();

            //if (currentPeriod != null)
            //    txtAccPeriod.Invoke((MethodInvoker)delegate { txtAccPeriod.Text = currentPeriod.periodName; });

            //txtAccPeriod.Invoke((MethodInvoker)delegate { txtGregCalender.Text = String.Format("{0} G.C. /{1} E.C./", currentDate.ToLongDateString(), currentDate.ToEthiopianDateString()); });
        }

        public double TimeToExpiryDate(string _licenseCode, string _compCode, string _editionCode, string _type)
        {
            double _remainingDays = -1;
            //try
            //{
            //    TimeSpan remainingDays = UIProcessManager.GetRemainingTimeSpan(_compCode, _editionCode, _type);
            //    if (remainingDays != null)
            //        _remainingDays = Math.Ceiling(remainingDays.TotalDays);
            //}
            //catch (Exception)
            //{
            //    return -1;
            //}
            return _remainingDays;
        }

        private async void sbApply_Click(object sender, EventArgs e)
        {
            bool successfullyTracked = false;
            if (LocalBuffer.LocalBuffer.ConfigurationBufferList == null)
                return;
            ConfigurationDTO mConfigurationPsExpi = LocalBuffer.LocalBuffer.ConfigurationBufferList.Where(l => l.Attribute.ToLower() == "minimum password length").FirstOrDefault();
            status = false;
            if (mConfigurationPsExpi != null)
            {
                if (Convert.ToInt16(mConfigurationPsExpi.CurrentValue) > txtNewPassword.Text.ToString().Length)
                {
                    XtraMessageBox.Show("NB: You used week passward", "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            else
            {
                XtraMessageBox.Show("NB: Please Set Minimum Password Length First!", "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Information);
                clear();
                changePasswordStatus = false;
                return;
            }
            bool OldPassIsAutenticated = false;
            var authenticateResponse = await SystemInitalize.authenticate(txtUser.Text, txtOldPassword.Text, LocalBuffer.LocalBuffer.CurrentDevice.Id, LocalBuffer.LocalBuffer.CompanyConsigneeData.Tin);
            if (authenticateResponse.Success)
            {
                OldPassIsAutenticated = true;
            }
            if (OldPassIsAutenticated)
            {
                if (!string.IsNullOrEmpty(txtNewPassword.Text))
                {
                    if (txtNewPassword.Text != txtOldPassword.Text)
                    {
                        if (txtNewPassword.Text == txtConfirmPassword.Text)
                        {
                            UserDTO mUser = LocalBuffer.LocalBuffer.CurrentLoggedInUser;
                            if (mUser != null)
                            {
                                UserUpdateDTO updateuser = new UserUpdateDTO()
                                {
                                    userId = mUser.Id,
                                    oldUserName = mUser.UserName,
                                    newUserName = mUser.UserName,
                                    oldPassword = txtOldPassword.Text,
                                    newPassword = txtNewPassword.Text,
                                    isActive = mUser.IsActive,
                                    person = mUser.Person,
                                    isAdmin = false,
                                    changePassword = true
                                };
                                ResponseModel<UserDTO> userupdated = UIProcessManager.UpdateUser(updateuser);

                                if (userupdated != null && userupdated.Success)
                                {
                                    XtraMessageBox.Show("Password Change Successful", "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                    #region Activity
                                    if (LocalBuffer.LocalBuffer.ActivityDefinitionBufferList == null)
                                        return;
                                    passWordChangeActivityDefn = LocalBuffer.LocalBuffer.ActivityDefinitionBufferList.Where(x => x.Description == CNETConstantes.LUK_Password_Change).FirstOrDefault();
                                    if (passWordChangeActivityDefn == null)
                                    {
                                        ActivityDefinitionDTO acd = new ActivityDefinitionDTO()
                                        {
                                            Reference = CNETConstantes.Employee,
                                            Description = CNETConstantes.LUK_Password_Change,
                                            State = CNETConstantes.Activity_Updated,
                                            Index = 0,
                                            NeedsPassCode = false,
                                            IssuingEffect = false,
                                            IsManual = false,
                                            RequiredTime = 0,
                                            MaxRepeat = 0,
                                            Sequence = false,
                                            IsPrint = false
                                        };
                                        passWordChangeActivityDefn = UIProcessManager.CreateActivityDefinition(acd);

                                        if (passWordChangeActivityDefn == null)
                                            return;
                                    }



                                    DateTime? currentDate = UIProcessManager.GetServiceTime();
                                    UserDTO currentUser = LocalBuffer.LocalBuffer.CurrentLoggedInUser;
                                    DeviceDTO device = LocalBuffer.LocalBuffer.CurrentDevice;

                                    ActivityDTO activity = new ActivityDTO()
                                    {
                                        Reference = LocalBuffer.LocalBuffer.CurrentLoggedInUser.Id,
                                        ConsigneeUnit = LocalBuffer.LocalBuffer.CurrentDeviceConsigneeUnit,
                                        TimeStamp = currentDate.Value,
                                        Year = currentDate.Value.Year,
                                        ActivityDefinition = passWordChangeActivityDefn.Id,
                                        Month = currentDate.Value.Month,
                                        Day = currentDate.Value.Day,
                                        Device = device.Id,
                                        Pointer = CNETConstantes.CONSIGNEE,
                                        Platform = "1",
                                        User = LocalBuffer.LocalBuffer.CurrentLoggedInUser.Id
                                    };



                                    successfullyTracked = UIProcessManager.CreateActivity(activity) != null ? true : false;
                                    if (!successfullyTracked)
                                        XtraMessageBox.Show("Activity not Saved", "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Information);


                                    #endregion

                                    changePasswordStatus = true;
                                    status = true;
                                    await ShowPasswordChangeLog();
                                    clear();

                                }
                                else
                                {
                                    XtraMessageBox.Show("User Update Fail " + Environment.NewLine + userupdated.Message, "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                }
                            }
                        }
                        else
                        {
                            XtraMessageBox.Show("Your Password Didn't Match", "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            clear();
                            changePasswordStatus = false;
                        }
                    }
                    else
                    {
                        XtraMessageBox.Show("Your New Password Is the same as the Old Password !!!", "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        clear();
                        changePasswordStatus = false;
                    }
                }
                else
                {
                    XtraMessageBox.Show("Please Enter New PassWord !!!", "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    clear();
                    changePasswordStatus = false;
                }
            }
            else
            {
                XtraMessageBox.Show("Your Old password incorrect", "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                clear();
                changePasswordStatus = false;
            }

        }

        private void sbCancel_Click(object sender, EventArgs e)
        {
            clear();
        }

        private void txtNewPassword_EditValueChanged(object sender, EventArgs e)
        {
            int N = 0;
            int L = txtNewPassword.Text.Length;
            if (L == 0)
                lblstatus.BackColor = Color.White;
            lblstatus.Text = "";
            if (Regex.IsMatch(txtNewPassword.Text, @"[\d]", RegexOptions.ECMAScript))
                N += 10;
            if (Regex.IsMatch(txtNewPassword.Text, @"[a-z]", RegexOptions.ECMAScript))
                N += 26;
            if (Regex.IsMatch(txtNewPassword.Text, @"[A-Z]", RegexOptions.ECMAScript))
                N += 26;
            if (Regex.IsMatch(txtNewPassword.Text, @"[~`!@#$%\^\&\*\(\)\-_\+=\[\{\]\}\|\\;:'\""<\,>\.\?\/£]", RegexOptions.ECMAScript) && txtNewPassword.Text.Length > 8)
                N += 33;
            if (N > 0)
            {
                int H = Convert.ToInt32(L * (Math.Round(Math.Log(N) / Math.Log(2))));
                if (H <= 31)
                {
                    lblstatus.BackColor = Color.Red; lblstatus.Text = "Weak Password";
                }
                else if (H <= 45)
                {
                    lblstatus.BackColor = Color.Yellow; lblstatus.Text = "Medium Password";
                }
                else if (H <= 70)
                {
                    lblstatus.BackColor = Color.Green; lblstatus.Text = "Strong Password";
                }
                else
                {
                    lblstatus.BackColor = Color.Green; lblstatus.Text = "Strong Password";

                }
            }
        }

        private async void ilbcLogsList_SelectedValueChanged_1(object sender, EventArgs e)
        {
            //if (ilbcLogsList.SelectedIndex == 0)
            //{
            //    await ShowLogActivity();
            //}
            //else if (ilbcLogsList.SelectedIndex == 1)
            //{
            //    ShowException();
            //}
            //else if (ilbcLogsList.SelectedIndex == 2)
            //{
            //    ShowCustomLog();
            //}
        }

        private async void xtraTabControl1_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            if (xtraTabControl1.SelectedTabPage == xtabSecurity)
            {
                await ShowPasswordChangeLog();
            }
            //if (xtraTabControl1.SelectedTabPage == xtabActivity)
            //{
            //    await ShowLogActivity();
            //}
            //else
            //else if (xtraTabControl1.SelectedTabPage == xtabPersonalRecord)
            //{
            //    if (persRecordDashbrd == null || !isPersonRecordLoaded)
            //    {
            //        PopulatePersonalRecords();
            //        isPersonRecordLoaded = true;
            //    }
            //}
            //else if (xtraTabControl1.SelectedTabPage == xtabMessage)
            //{
            //    pnl_messages.Controls.Clear();
            //    MessageViewer msgViewer = new MessageViewer(Authentication.CurrentLoggedInUser.userName, Authentication.CurrentLoggedInUser.code, SelectedMessageHandler)
            //    {
            //        Dock = DockStyle.Fill
            //    };
            //    pnl_messages.Controls.Add(msgViewer);
            //}
            //else if (xtraTabControl1.SelectedTabPage == xtabEmail)
            //{
            //    showEmail();
            //}
            //else if (xtraTabControl1.SelectedTabPage == xtabVoucherDashboard)
            //{
            //    if (vouchDoc == null)
            //    {
            //        PopulateVoucherDashboard();
            //    }
            //}
        }

        public static PMSDashboard dashboard = null;
        private void ShowPmsDashboard()
        {
            // TimestampAndLog date = UIProcessManager.GetDataTime();
            //  if (date.description == null)
            //{
            //  dashboard = new PMSDashboard(date.Timestamp, SelectedDashboardTaskHandler) { Dock = DockStyle.Fill };
            dashboard = new PMSDashboard(SelectedDashboardTaskHandler) { Dock = DockStyle.Fill };
            pnl_dashboard.Controls.Clear();
            pnl_dashboard.Controls.Add(dashboard);
            pnl_dashboard.Refresh();

            //}
            //else
            //{
            //    XtraMessageBox.Show(date.description, "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    return;
            //}
        }
        private void showEmail()
        {
            //string personCode = LocalBuffer.LocalBuffer.GetAuthorizedUser().person;
            //if (!string.IsNullOrEmpty(personCode))
            //{
            //    emailViewer = new CNET_EmailViewer(personCode) { Dock = DockStyle.Fill };
            //    pnl_email.Controls.Clear();
            //    pnl_email.Controls.Add(emailViewer);
            //}
        }

        private void gv_activityLog_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            if (e.Column.Caption == "SN")
            {
                e.DisplayText = (e.RowHandle + 1).ToString();
            }
        }

        private void gvPasswordLog_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            if (e.Column.Caption == "SN")
            {
                e.DisplayText = (e.RowHandle + 1).ToString();
            }
        }

        private void lbl_licenseExpires_Click(object sender, EventArgs e)
        {
            //Authentication.ShowExpireNotice();
        }

        private void dtStartDate_EditValueChanged(object sender, EventArgs e)
        {
            if (cmbDateCriteria.Text == "At the day of")
            {
                dtEndDate.EditValue = dtStartDate.EditValue;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Title = "Save";
                sfd.DefaultExt = "xls";
                sfd.Filter = "*.xls|*.xls|*.xlsx|*.xlsx";
                sfd.FileName = "Exported License Document File";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    gridControl1.ExportToXls(sfd.FileName);
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message, "CNET Document Browser", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
    public class NotificationHeader
    {
        public string storeCode { get; set; }
        public string storeName { get; set; }
        public int count { get; set; }
        public string type { get; set; }
        public List<NotificationDetail> Detail { get; set; }

    }
    public class NotificationDetail
    {
        public string articleCode { get; set; }
        public string articleName { get; set; }
        public string uom { get; set; }
        public string batch { get; set; }
        public string batchBalance { get; set; }
        public string prodDate { get; set; }
        public string expiryDate { get; set; }
        public string lifeTime { get; set; }
        public string remainingLifetime { get; set; }


        public decimal minLevel { get; set; }
        public decimal eoq { get; set; }
        public decimal balance { get; set; }
        public decimal difference { get; set; }
    }
}
