using CNET.ERP.Client.Common.UI;
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
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraReports.UI;
using DevExpress.Data;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
using System.IO;
using System.Globalization;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraBars;
using System.Threading;
using DevExpress.Utils;
using System.Security.Cryptography;
using PMS_Dashboard;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using CNET_V7_Domain.Domain.PmsSchema;
using CNET_V7_Domain.Domain.SettingSchema;
using ProcessManager;
using CNET_V7_Domain.Misc.PmsDTO;
using CNET_V7_Domain.Domain.ConsigneeSchema;
using CNET.FrontOffice_V._7;
using CNET_V7_Domain.Domain.SecuritySchema;
using CNET_V7_Domain.Domain.ViewSchema;
using DevExpress.Xpo.DB;
using CNET_V7_Domain.Domain.TransactionSchema;
using System.Diagnostics;
using CNET_V7_Domain.Domain.CommonSchema;
using CNET_V7_Domain.Misc;
using CNET.FP.Tool;
using CNET_V7_Domain.Misc.PmsView;
using CNET_V7_Domain.Domain.ArticleSchema;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using CNET.Progress.Reporter;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Import.Doc;

namespace PMSReport
{
    public partial class frmPMSReports : XtraForm
    {

        public static DateTime currentDate;
        public static int? user = null;
        public static int? guest = null;
        public static int? consignee = null;
        public static int? company = null;
        public static string summaryCriteria = null;
        List<DailyResidentSummaryDTO> dailyResidentSummaries = new List<DailyResidentSummaryDTO>();
        //List<DailyResidentSummaryColumn> drsCols = new List<DailyResidentSummaryColumn>();
        DailyResidentSummaryDTO dailyResidentSummary = new DailyResidentSummaryDTO();
        //List<vw_RegistrationDocumentView> registrations = null;
        GridColumnSummaryItem item = null;
        GridGroupSummaryItem groupSumm = new GridGroupSummaryItem();
        GridColumn gcolCode = new GridColumn();
        DataGridViewRow growSummary = new DataGridViewRow();
        //public TrialBalanceReport TrBalance = null;

        //private List<vw_UserRoleWithReportAccessLevel> reports = null;
        //private List<vw_UserRoleWithReportAccessLevel> PMS = null;
        private TreeNode treeNodesReport;


        List<ReportTreeDTO> reportList = null; 
        string selectedReportName = "";

        private string reportArchivePath = "";



        private List<String> _approvedFunctionalities = null;

        private TextInfo myTI = new CultureInfo("en-US", false).TextInfo;

        private DateTime? issuedDate { get; set; }
        private DateTime? issueDateEnd { get; set; }
        private bool printByPortrait { get; set; }

        private bool isFromNightAudit = false;
        public ActivityDefinitionDTO DailyResidentSummaryActDef { get; set; }
        int? DailyResidentSummaryobjectstate = null;
        string FTPServerHost = "";
        string FTPServerFolder = "";
        string FTPServerUserName = "";
        string FTPServerPassword = "";
        bool EnableFTPSync = false;
        bool GenerateReportForNullData = false;
        PMSDashboard dash = null;


        /************** CONSTRUCTOR **********************/

        public frmPMSReports(bool isNightAudit, PMSDashboard PMSDashboard = null)
        {
            InitializeComponent();
            this.isFromNightAudit = isNightAudit;
            dash = PMSDashboard;
             

            GetFTPServerConfiguration();

            splitContainer1.Panel2Collapsed = false;
            splitContainer1.Panel1Collapsed = true;
            gcReports.EmbeddedNavigator.Appearance.BackColor = Color.LightYellow;
            gcReports.EmbeddedNavigator.Appearance.ForeColor = Color.Red;
            gcReports.EmbeddedNavigator.Buttons.Append.Visible = false;
            gcReports.EmbeddedNavigator.Buttons.CancelEdit.Visible = false;
            gcReports.EmbeddedNavigator.Buttons.EndEdit.Visible = false;
            gcReports.EmbeddedNavigator.Buttons.NextPage.Visible = false;
            gcReports.EmbeddedNavigator.Buttons.PrevPage.Visible = false;
            gcReports.EmbeddedNavigator.Buttons.Remove.Visible = false;
            gcReports.EmbeddedNavigator.Buttons.Edit.Visible = false;

            gcReports2.EmbeddedNavigator.Appearance.BackColor = Color.LightYellow;
            gcReports2.EmbeddedNavigator.Appearance.ForeColor = Color.Red;
            gcReports2.EmbeddedNavigator.Buttons.Append.Visible = false;
            gcReports2.EmbeddedNavigator.Buttons.CancelEdit.Visible = false;
            gcReports2.EmbeddedNavigator.Buttons.EndEdit.Visible = false;
            gcReports2.EmbeddedNavigator.Buttons.NextPage.Visible = false;
            gcReports2.EmbeddedNavigator.Buttons.PrevPage.Visible = false;
            gcReports2.EmbeddedNavigator.Buttons.Remove.Visible = false;
            gcReports2.EmbeddedNavigator.Buttons.Edit.Visible = false;

            var font = gvReports.Appearance.Row.Font;
            gvReports.Appearance.Row.Font = new System.Drawing.Font(font.FontFamily, 6.5f, FontStyle.Regular);

            gvReports.OptionsView.RowAutoHeight = true;
            gvReports.Appearance.Row.Options.UseTextOptions = true;
            gvReports.Appearance.Row.TextOptions.WordWrap = WordWrap.Wrap;
            gvReports.Appearance.FooterPanel.Options.UseTextOptions = true;
            gvReports.Appearance.FooterPanel.TextOptions.WordWrap = WordWrap.Wrap;

            gvReports.Appearance.FooterPanel.Font = new System.Drawing.Font(font.FontFamily, 6.5f, FontStyle.Regular);

            gvReports.AppearancePrint.Row.Options.UseTextOptions = true;
            gvReports.AppearancePrint.Row.TextOptions.WordWrap = WordWrap.Wrap;

            gvReports.AppearancePrint.Row.Font = new System.Drawing.Font(font.FontFamily, 6.5f, FontStyle.Regular);
            gvReports.AppearancePrint.FooterPanel.Font = new System.Drawing.Font(font.FontFamily, 6.5f, FontStyle.Regular);

            gvReports.RowHeight = 25;

            gvReports.DoubleClick += GvReports_DoubleClick;
            GetActitvityAndOrgunitdef();

        }
        private void GetFTPServerConfiguration()
        {
            //LoginPage.Authentication.LoadConfiguration();

            //EnableFTPSync = false;
            //Device FTPDevice = LoginPage.Authentication.DeviceBufferList.FirstOrDefault(x => x.preference == CNETConstantes.FTPServer);

            //if (FTPDevice != null && FTPDevice.isActive)
            //{
            //    EnableFTPSync = true;
            //    List<Configuration> FTPDeviceConfiguration = LoginPage.Authentication.ConfigurationBufferList.Where(x => x.reference == FTPDevice.code).ToList();
            //    if (FTPDeviceConfiguration != null && FTPDeviceConfiguration.Count > 0)
            //    {
            //        Configuration FTPUserName = FTPDeviceConfiguration.FirstOrDefault(x => x.attribute == "User Name");
            //        if (FTPUserName != null)
            //        {s
            //            FTPServerUserName = FTPUserName.currentValue;
            //        }
            //        Configuration FTPHost = FTPDeviceConfiguration.FirstOrDefault(x => x.attribute == "Host");
            //        if (FTPHost != null)
            //        {
            //            FTPServerHost = FTPHost.currentValue;
            //        }
            //        Configuration FTPPassword = FTPDeviceConfiguration.FirstOrDefault(x => x.attribute == "FTP Password");
            //        if (FTPPassword != null && !string.IsNullOrEmpty(FTPPassword.currentValue))
            //        {
            //            FTPServerPassword = Decrypt(FTPPassword.currentValue);
            //        }
            //        Configuration FTPFolder = FTPDeviceConfiguration.FirstOrDefault(x => x.attribute == "FTP URL");
            //        if (FTPFolder != null)
            //        {
            //            string orgdefcode = LoginPage.Authentication.OrganizationUnitBufferList.FirstOrDefault(x => x.reference == LoginPage.Authentication.DeviceObject.code).organizationUnitDefinition;
            //            OrganizationUnitDefinition branchhh = LoginPage.Authentication.OrganizationUnitDefinitionBufferList.FirstOrDefault(x => x.code == orgdefcode);
            //            if (branchhh == null)
            //            {
            //                MessageBox.Show("Please this device doesn't have a branch!", "CNET_2016", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //                FTPServerFolder = FTPFolder.currentValue + "/Reports/PMS Report/Head Office";
            //            }
            //            else
            //            {
            //                FTPServerFolder = FTPFolder.currentValue + "/Reports/PMS Report/" + branchhh.description;
            //            }
            //        }
            //        Configuration GenerateReportForNullReport = LoginPage.Authentication.ConfigurationBufferList.FirstOrDefault(x => x.preference == "PMS" && x.attribute == "Generate Report For Null Data");
            //        if (GenerateReportForNullReport != null)
            //        {
            //            GenerateReportForNullData = Convert.ToBoolean(GenerateReportForNullReport.currentValue);
            //        }
            //    }
            //}
            //else
            //{
            //    EnableFTPSync = false;
            //}
        }

        public string Decrypt(string cipherText)
        {
            try
            {
                if (string.IsNullOrEmpty(cipherText))
                    return null;

                string EncryptionKey = "MAKV2SPBNI99212";
                byte[] cipherBytes = Convert.FromBase64String(cipherText);
                using (Aes encryptor = Aes.Create())
                {
                    Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey,
                        new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                    encryptor.Key = pdb.GetBytes(32);
                    encryptor.IV = pdb.GetBytes(16);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(cipherBytes, 0, cipherBytes.Length);
                            cs.Close();
                        }
                        cipherText = Encoding.Unicode.GetString(ms.ToArray());
                    }
                }
                return cipherText;
            }
            catch
            {
                return null;
            }
        }
        public void GetActitvityAndOrgunitdef()
        {
            ActivityDefinitionDTO activityDef = UIProcessManager.GetActivityDefinitionByreference(CNETConstantes.Daily_Resident_Summary_Voucher).FirstOrDefault();
            if (activityDef != null)
            {
                DailyResidentSummaryActDef = activityDef;
                DailyResidentSummaryobjectstate = activityDef.State;
            }
            else
            {
                DailyResidentSummaryActDef = null;
                DailyResidentSummaryobjectstate = null;
            }

        }

        private void GvReports_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                GridView view = sender as GridView;
                if (view == null) return;
                if (string.IsNullOrEmpty(selectedReportName)) return;
               DocumentPrint.ReportGenerator rg = new DocumentPrint.ReportGenerator();

                if (selectedReportName == CNETConstantes.CASH_DROPPED_REPORT)
                {
                    SalesDocumentList salesDto = view.GetRow(view.FocusedRowHandle) as SalesDocumentList;
                    PaymentDocumentList paymentDTO = view.GetRow(view.FocusedRowHandle) as PaymentDocumentList;
                    if (salesDto != null)
                    {
                         rg.GetAttachementReport(salesDto.VoucherID);
                    }
                    else if (paymentDTO != null)
                    {
                         rg.GetAttachementReport(paymentDTO.VoucherID);
                    }


                }
                else if (selectedReportName == CNETConstantes.DAILY_SALES_SUMMARY)
                {
                    DataRowView row = view.GetRow(view.FocusedRowHandle) as DataRowView;
                    if (row != null && row.Row != null && row.Row.ItemArray != null && row.Row.ItemArray.Length > 0)
                    {
                        string code = row.Row.ItemArray[1] as string;
                        if (!string.IsNullOrEmpty(code))
                            rg.GetAttachementReport(code);
                    }
                }
                else if (selectedReportName == CNETConstantes.ROOM_POS_CHARGES)
                {
                    RoomPOSChargesReportDTO dto = view.GetRow(view.FocusedRowHandle) as RoomPOSChargesReportDTO;
                    if (dto != null)
                    {
                         rg.GetAttachementReport(dto.VoucherID);
                    }
                }


            }
            catch (Exception ex)
            {

            }
        }


        #region Helper Methods

        public bool InitializeData()
        {
            try
            {

                DateTime? currentTime = UIProcessManager.GetServiceTime();
                if (currentTime != null)
                {
                    currentDate = currentTime.Value;
                    rpItmDate.MaxValue = currentTime.Value;
                    rpItmDateEnd.MaxValue = currentTime.Value;
                }
                else
                {
                    return false;
                }


                ReportTreeLst.DataSource = CreateReportList();
                ReportTreeLst.RefreshDataSource();
                LoadReportArchiveConfig();

                /*
                //Load Security Data
                string category = "PMS Report";
                string SubSystemComponent = CNETConstantes.SECURITYFrontDeskRoutines;
                string currentRole = "";
                var role = LoginPage.Authentication.UserRoleMapperBufferList.Where(x => x.user == LoginPage.Authentication.GetAuthorizedUser().code).FirstOrDefault();
                if (role != null)
                    currentRole = role.role;

                List<vw_UserRoleWithReportAccessLevel> AlloweduserReportview = new List<vw_UserRoleWithReportAccessLevel>();
                AlloweduserReportview = UIProcessManager.GetReportSecurityByUser(LoginPage.Authentication.GetAuthorizedUser().code);

                //  var functions = UIProcessManager.GetFuncwithAccessMatView(currentRole, category, SubSystemComponent).Where(x => x.access == true).ToList();
                if (AlloweduserReportview != null && AlloweduserReportview.Count > 0)
                    _approvedFunctionalities = AlloweduserReportview.Select(x => x.reportType).ToList();
                */


                //Load Security Data
                //string category = "PMS Report";
                //string SubSystemComponent = CNETConstantes.SECURITYFrontDeskRoutines;
                //string currentRole = "";
                //var role = LoginPage.Authentication.UserRoleMapperBufferList.Where(x => x.user == LoginPage.Authentication.GetAuthorizedUser().code).FirstOrDefault();
                //if (role != null)
                //    currentRole = role.role;


                #region OLD Incorrect This is from Functionality
                //var functions = UIProcessManager.GetFuncwithAccessMatView(currentRole, category, SubSystemComponent).Where(x => x.access == true).ToList();
                //if (functions != null && functions.Count > 0)
                //    _approvedFunctionalities = functions.Select(x => x.description).ToList();




                // Initialize Search By Date Criteria
                string[] SearChByDate = { "Daily", "Weekly", "Monthly", "At the day of", "Annually", "Date Range", "Show All" };
                rpDateCriteria.DataSource = SearChByDate;
                rpDateCriteria.View.Columns.Add();
                rpDateCriteria.View.Columns[0].FieldName = "Column";
                rpDateCriteria.View.Columns[0].Caption = "Search By Date criteria";
                rpDateCriteria.View.Columns[0].Visible = true;
                rpDateCriteria.DisplayMember = "Column";
                rpDateCriteria.ValueMember = "Column";
                rpDateCriteria.EditValueChanged += BBDateSearch_EditValueChanged;
                gvReports.RowStyle += GvReports_RowStyle;
                ReportTreeLst.ExpandAll();

                List<ConsigneeUnitDTO> HotelBranchBufferList = new List<ConsigneeUnitDTO>();
                if (LocalBuffer.LocalBuffer.HotelBranchBufferList != null && LocalBuffer.LocalBuffer.HotelBranchBufferList.Count > 0)
                {
                    HotelBranchBufferList.AddRange(LocalBuffer.LocalBuffer.HotelBranchBufferList.OrderBy(x => x.Id).ToList());
                    HotelBranchBufferList.Add(new ConsigneeUnitDTO { Id = -1, Name = "ALL Hotel Branch" });
                }

                reiHotel.DisplayMember = "Name";
                reiHotel.ValueMember = "Id";
                reiHotel.DataSource = HotelBranchBufferList;


                if (LocalBuffer.LocalBuffer.HotelBranchBufferList != null && LocalBuffer.LocalBuffer.HotelBranchBufferList.Count > 0)
                {
                    beiHotel.EditValue = LocalBuffer.LocalBuffer.CurrentDeviceConsigneeUnit;
                    //reiHotel.ReadOnly = !UserHasHotelBranchAccess;
                }
                return true;
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Error in initializing report. DETAIL:: " + ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // Home.ShowModalInfoMessage("Error in initializing report. DETAIL:: " + ex.Message, "ERROR");
                return false;
            }
        }


        public void ClearGrid()
        {

            gcReports.DataSource = null;
            gvReports.Columns.Clear();
            gvReports.GroupSummary.Clear();

            gcReports2.DataSource = null;
            gvReports2.Columns.Clear();
            gvReports2.GroupSummary.Clear();
        }

        public MemoryStream ExportToPdfMemoryStream(string title, DateTime date)
        {
            bool flag = true;
            XtraReport1 report = null;
            MemoryStream stream = new MemoryStream();
            try
            {
                report = new XtraReport1(gcReports, null, title, date.ToShortDateString(), SelectedHotelDescription, printByPortrait, null);
                //XtraReport1 report = new XtraReport1(gcReports, null, title, date.Date.ToShortDateString());
                report.Landscape = !printByPortrait;
                //report.ExportToPdf(path);
                report.ExportToPdf(stream);

                return stream;

            }
            catch (Exception ex)
            {

                return null;
            }

            return stream;
        }

        public bool ExportToPdf(string path, string title, DateTime date, string defaultPrinterName = "")
        {
            bool flag = true;
            XtraReport1 report = null;
            try
            {
                report = new XtraReport1(gcReports, null, title, date.ToShortDateString(), SelectedHotelDescription, printByPortrait, null);
                //XtraReport1 report = new XtraReport1(gcReports, null, title, date.Date.ToShortDateString());
                report.Landscape = !printByPortrait;
                //report.ExportToPdf(path);
                report.ExportToPdf(path);


                if (GenerateReportForNullData || gvReports.RowCount > 0)
                {
                    //if (EnableFTPSync)
                    // UIProcessManager.UploadFTP(FTPServerHost, FTPServerUserName, FTPServerPassword, FTPServerFolder, path);
                }

                //print pdf
                try
                {
                    if (!string.IsNullOrEmpty(defaultPrinterName))
                        report.Print(defaultPrinterName);
                }
                catch (Exception ex)
                {
                    return true;
                }

            }
            catch (Exception ex)
            {

                return false;
            }

            return flag;
        }

        private void IntiallizeSummaryCriteria()
        {
            List<string> summaryCriteriaList = new List<string>() { "User", "Summary" };
            rpCashierSummaryCriteria.DataSource = summaryCriteriaList;
            bbCashierSummayBy.Caption = "Cashier Summary by";


            rpCashierSummaryCriteria.View.Columns.Add();
            rpCashierSummaryCriteria.View.Columns[0].FieldName = "Column";
            rpCashierSummaryCriteria.View.Columns[0].Caption = "Criteria";
            rpCashierSummaryCriteria.View.Columns[0].Visible = true;

            rpCashierSummaryCriteria.ValueMember = "Column";
            rpCashierSummaryCriteria.DisplayMember = "Column";

            rpCashierSummaryCriteria.EditValueChanged += SearchBySummaryCriteria;
            bbCashierSummayBy.EditValue = summaryCriteriaList.FirstOrDefault();
            summaryCriteria = summaryCriteriaList.FirstOrDefault();
        }

        private void InitializeUserName()
        {
            List<UserDTO> userNameList = UIProcessManager.SelectAllUser();
            rpNameList.DataSource = userNameList;
            bbnNameList.Caption = "User Name";

            rpNameList.View.Columns.Add();
            rpNameList.View.Columns[0].FieldName = "Id";
            rpNameList.View.Columns[0].Caption = "Id";
            rpNameList.View.Columns[0].Visible = false;

            rpNameList.View.Columns.Add();
            rpNameList.View.Columns[1].FieldName = "UserName";
            rpNameList.View.Columns[1].Caption = "User Name";
            rpNameList.View.Columns[1].Visible = true;

            rpNameList.ValueMember = "Id";
            rpNameList.DisplayMember = "UserName";

            rpNameList.EditValueChanged += SearchByUser;
        }

        private bool CheckSecurity(string reportName)
        {
            try
            {
                if (_approvedFunctionalities == null) return false;
                foreach (String str in _approvedFunctionalities)
                {
                    if (str.ToLower().Trim().Equals(reportName.ToLower().Trim()))
                    {
                        return true;
                    }
                }
                return false;


            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private void InitializeGuestName(DateTime date, string travelDetail)
        {
            rpNameList.DataSource = LocalBuffer.LocalBuffer.AllGuestConsigneeViewlist;
            bbnNameList.Caption = "Guest Name";

            rpNameList.View.Columns.Add();
            rpNameList.View.Columns[0].FieldName = "FirstName";
            rpNameList.View.Columns[0].Caption = "First Name";
            rpNameList.View.Columns[0].Visible = true;
            rpNameList.View.Columns.Add();
            rpNameList.View.Columns[1].FieldName = "SecondName";
            rpNameList.View.Columns[1].Caption = "Middle Name";
            rpNameList.View.Columns[1].Visible = true;

            rpNameList.ValueMember = "Id";
            rpNameList.DisplayMember = "FirstName";

            rpNameList.EditValueChanged += SearchByGuest;
        }
        private void Initializeconsignee()
        {

            rpNameList.DataSource = LocalBuffer.LocalBuffer.AllGuestConsigneeViewlist;
            bbnNameList.Caption = "Guest Name";
            bbnNameList.Tag = "consignee";
            rpNameList.View.Columns.Add();
            rpNameList.View.Columns[0].FieldName = "FirstName";
            rpNameList.View.Columns[0].Caption = "First Name";
            rpNameList.View.Columns[0].Visible = true;
            rpNameList.View.Columns.Add();
            rpNameList.View.Columns[0].FieldName = "SecondName";
            rpNameList.View.Columns[0].Caption = "Middle Name";
            rpNameList.View.Columns[0].Visible = true;

            rpNameList.ValueMember = "Id";
            rpNameList.DisplayMember = "FirstName";

            rpNameList.EditValueChanged += SearchByconsignee;
        }

        private void InitializeCompanyconsignee()
        {


            // LoadRoomDetailBuffer();
            rpNameList.DataSource = LocalBuffer.LocalBuffer.AllCustomerConsigneeViewlist;
            bbnNameList.Caption = "Company Name";
            bbnNameList.Tag = "company";

            rpNameList.View.Columns.Add();
            rpNameList.View.Columns[0].FieldName = "FirstName";
            rpNameList.View.Columns[0].Caption = "Name";
            rpNameList.View.Columns[0].Visible = true;

            rpNameList.ValueMember = "Id";
            rpNameList.DisplayMember = "FirstName";

            rpNameList.EditValueChanged += SearchByconsignee;
        }

        public void ReportShow(DateTime selectedDate, string reportName, DateTime? endDate = null)
        {
            selectedReportName = reportName;
            printByPortrait = false;

            if (endDate != null || isFromNightAudit)
            {
                issuedDate = selectedDate;
                issueDateEnd = selectedDate;
            }
            if (reportName == CNETConstantes.DAILY_RESIDENNT_SUMMARY)
            {
                if (SelectedHotelcode != null)
                {
                    ShowDailyResidentSummaryReport(selectedDate);
                }
                else
                {
                    XtraMessageBox.Show("Please Select Hotel Branch First !!");
                }
                //rpgSave.Visible = true;
            }
            else if (reportName == CNETConstantes.FOREIGN_EXCHANGE_SUMMARY)
            {
                ShowForeignExSummaryReport(selectedDate);
            }
            else if (reportName == CNETConstantes.FOREIGN_EXCHANGE_DETAIL)
            {
                ShowForeignExReport(selectedDate);
            }
            else if (reportName == CNETConstantes.DAILY_BUSINESS_REPORT)
            {
                if (SelectedHotelcode != null)
                {
                    ShowDailyBusinessReport(selectedDate);
                    rpgSave.Visible = false;
                }
                else
                {
                    XtraMessageBox.Show("Please Select Hotel Branch First !!");
                }
            }
            else if (reportName == CNETConstantes.MANAGERIAL_FLASH)
            {
                ShowManagerialFlashReport(selectedDate);
            }
            else if (reportName == CNETConstantes.TRIAL_BALANCE)
            {
                ShowTrialBalance(selectedDate);
            }
            else if (reportName == CNETConstantes.PACKAGE_REPORT)
            {
                ShowPackageReport(selectedDate);
            }
            else if (reportName == CNETConstantes.RATE_CHECK_REPORT)
            {
                ShowRateCheckReport(selectedDate);
            }
            else if (reportName == CNETConstantes.RATE_ADJUSTMENT)
            {
                ShowRateAdjustmentReport(selectedDate);
            }
            else if (reportName == CNETConstantes.CHECK_OUT_REPORT)
            {
                ShowLineitemTransactionReports(issuedDate.Value, issueDateEnd.Value, new List<int>() { CNETConstantes.CHECK_OUT_BILL_VOUCHER }, true, consignee, company);
            }
            else if (reportName == CNETConstantes.CASHIER_SUMMARY)
            {
                ShowCashierSummaryReport(selectedDate, user);
            }
            else if (reportName == CNETConstantes.CANCELATION_OF_THE_DAY)
            {
                ShowCancellationOfTheDay(selectedDate);
            }
            else if (reportName == CNETConstantes.NO_SHOW_REPORT)
            {
                ShowNoShowReport(selectedDate);
            }
            else if (reportName == CNETConstantes.CITY_LEDGER)
            {
                ShowCityledger(issuedDate.Value, issueDateEnd.Value);
            }
            else if (reportName == CNETConstantes.NATIONALITY_REPORT)
            {
                ShowNationalityReport(issuedDate.Value, issueDateEnd.Value);
            }
            else if (reportName == CNETConstantes.DEPOSIT_LEDGER)
            {
                ShowDepositLedger(selectedDate);
            }
            else if (reportName == CNETConstantes.Payment_Method_Detail_REPORT)
            {
                ShowPaymentMethodDetailReport(selectedDate);
            }
            else if (reportName == CNETConstantes.Payment_Method_Summary_REPORT)
            {
                ShowPaymentMethodSummaryReport(selectedDate);
            }
            //else if (reportName == CNETConstantes.CREDIT_CARDS_OF_THE_DAY)
            //{
            //    ShowCreditCardsOfTheDay(selectedDate);
            //}
            //else if (reportName == CNETConstantes.CHECK_REPORT_OF_THE_DAY)
            //{
            //    ShowCheckReportOfTheDay(selectedDate);
            //}
            else if (reportName == CNETConstantes.ARRIVAL_LIST)
            {
                ShowArrivalsList(selectedDate);
            }
            else if (reportName == CNETConstantes.UNASSIGNED_RESERVATION)
            {
                ShowUnassignedReservations(issuedDate.Value, issueDateEnd.Value);
            }
            else if (reportName == CNETConstantes.BILL_TRANSFER_REPORT)
            {
                ShowBillTransferReport(issuedDate.Value.Date, issueDateEnd.Value.Date.AddHours(23).AddMinutes(60));
            }
            else if (reportName == CNETConstantes.ARRIVED_LIST)
            {
                ShowArrivedList(selectedDate);
            }
            else if (reportName == CNETConstantes.STAY_OVERS)
            {
                ShowStayOvers(selectedDate);
            }
            else if (reportName == CNETConstantes.DUE_OUTS)
            {
                ShowDueOuts(selectedDate);
            }
            else if (reportName == CNETConstantes.DEPARTED_LIST)
            {
                ShowDepartedList(selectedDate);
            }
            else if (reportName == CNETConstantes.POST_MASTER_INHOUSE_LIST)
            {
                ShowPostmasterInhouseList(selectedDate);
            }
            else if (reportName == CNETConstantes.ROOM_MOVE)
            {
                ShowRoomMoveReport(selectedDate);
            }
            else if (reportName == CNETConstantes.POLICE_REPORT)
            {
                if (SelectedHotelcode != null)
                {
                    ShowPoliceReport(selectedDate);
                }
                else
                {
                    XtraMessageBox.Show("Please Select Hotel Branch First !!");
                }
            }
            else if (reportName == CNETConstantes.DETAIL_DAILY_SALES_TRANSACTION)
            {
                ShowDetailDailySalesTransaction(selectedDate);
            }
            else if (reportName == CNETConstantes.PICK_UP_REPORT)
            {
                ShowPickupReport(selectedDate, null);
            }
            else if (reportName == CNETConstantes.DROP_OFF_REPORT)
            {
                ShowDropoffReport(selectedDate, null);
            }
            else if (reportName == CNETConstantes.DISCRIPANCY_REPORT)
            {
                ShowDiscripancyOfTheDay(selectedDate);
            }
            else if (reportName == CNETConstantes.STATUS_REPORT)
            {
                ShowStatusReportForToday(selectedDate);
            }
            else if (reportName == CNETConstantes.TASK_ASSIGNMENT_REPORT)
            {
                ShowTodaysTaskAssignments(selectedDate);
            }
            //else if (reportCode == CNETConstantes.HK_ATTENDANTS_REPORTS)
            //{
            //    return;
            //}
            else if (reportName == CNETConstantes.HK_ACTIVITY_REPORT)
            {
                ShowHKActivity(selectedDate);
            }
            else if (reportName == CNETConstantes.HK_ROOMSTATUS_REPORT)
            {
                ShowHKRoomStatus(selectedDate);
            }
            else if (reportName == CNETConstantes.CASH_DROPPED_REPORT)
            {
                ShowCashDroppedReport(issuedDate.Value, issueDateEnd.Value);
            }
            else if (reportName == CNETConstantes.DAILY_SALES_SUMMARY)
            {
                ShowDailySalesSummary(issuedDate.Value, issueDateEnd.Value);
            }
            else if (reportName == CNETConstantes.ROOM_INCOME_REPORT)
            {
                ShowRoomIncomeReport(issuedDate.Value, issueDateEnd.Value);
            }
            else if (reportName == CNETConstantes.CASH_RECEIPT_REPORT)
            {
                ShowCRVTransactionReports(issuedDate.Value, issueDateEnd.Value, CNETConstantes.CASHRECIPT);
            }
            else if (reportName == CNETConstantes.CREDIT_SALES_REPORT)
            {
                ShowLineitemTransactionReports(issuedDate.Value, issueDateEnd.Value, new List<int>() { CNETConstantes.CREDITSALES }, false, null, null);
            }
            else if (reportName == CNETConstantes.CASH_SALES_REPORT)
            {
                ShowLineitemTransactionReports(issuedDate.Value, issueDateEnd.Value, new List<int>() { CNETConstantes.CASH_SALES }, false, null, null);
            }
            else if (reportName == CNETConstantes.REBATE_REPORT)
            {
                ShowNonlineitemTransactionReports(issuedDate.Value, issueDateEnd.Value, CNETConstantes.CREDIT_NOTE_VOUCHER);
            }
            else if (reportName == CNETConstantes.DEBIT_NOTE_REPORT)
            {
                ShowNonlineitemTransactionReports(issuedDate.Value, issueDateEnd.Value, CNETConstantes.BANKDEBITNOTE);
            }
            else if (reportName == CNETConstantes.PAID_OUT_REPORT)
            {
                ShowNonlineitemTransactionReports(issuedDate.Value, issueDateEnd.Value, CNETConstantes.PAID_OUT_VOUCHER);
            }
            else if (reportName == CNETConstantes.REFUND_REPORT)
            {
                ShowNonlineitemTransactionReports(issuedDate.Value, issueDateEnd.Value, CNETConstantes.REFUND);
            }
            else if (reportName == CNETConstantes.DAILY_ROOM_CHARGE_REPORT)
            {
                ShowLineitemTransactionReports(issuedDate.Value, issueDateEnd.Value, new List<int>() { CNETConstantes.DAILY_ROOM_CHARGE_VOUCHER }, false, null, null);
            }
            else if (reportName == CNETConstantes.ROOM_POS_CHARGES)
            {
                ShowRoomPOSCharges(issuedDate.Value, issueDateEnd.Value);
            }
            else if (reportName == CNETConstantes.SUMMARY_OF_SUMMARY)
            {
                ShowSummaryOfSummaryReport(issuedDate.Value, issueDateEnd.Value);
            }
            else if (reportName == CNETConstantes.PMS_Fiscal_Reconciliation)
            {
                ShowPMSFiscalReconciliationReport(issuedDate.Value, issueDateEnd.Value);
            }



            Progress_Reporter.Close_Progress();
        }
        public class ReportTreeDTO
        {
            public string reportCode { get; set; }
            public string reportDesc { get; set; }
            public string reportParent { get; set; }
        }
        private List<ReportTreeDTO> CreateReportList()
        {
            reportList = new List<ReportTreeDTO>();

            var allReports = UIProcessManager.SelectAllReport()
                .Where(p => p.Subsystem == CNETConstantes.PMS_Report && p.IsActive)
                .OrderBy(p => p.Index).ToList();
            if (allReports == null) return null;
            List<ReportDTO> cateList = allReports
                .GroupBy(p => p.Reference)
                .Select(p => p.First()).ToList();


            int cd = 1;
            foreach (var category in cateList)
            {
                SystemConstantDTO ReportCategory = LocalBuffer.LocalBuffer.SystemConstantDTOBufferList.FirstOrDefault(x=> x.Id == category.Reference);
                ReportTreeDTO report = new ReportTreeDTO();
                report.reportCode = "category" + cd;
                report.reportDesc = ReportCategory.Description;
                report.reportParent = null;
                reportList.Add(report);


                var reports = allReports.OrderBy(p => p.DefaultName).Where(p => p.Reference == category.Reference).ToList();
                if (reports != null)
                {
                    foreach (var rprt in reports)
                    {
                        ReportTreeDTO childRepo = new ReportTreeDTO();
                        childRepo.reportCode = rprt.Id.ToString();
                        childRepo.reportDesc = rprt.DefaultName;
                        childRepo.reportParent = "category" + cd;
                        reportList.Add(childRepo);
                    }
                }
                cd++;
            }
            return reportList;
        }

        private void LoadReportArchiveConfig()
        {
            //if (Authentication.DeviceObject != null)
            //{
            //    Configuration archivePathConfig = Authentication.ConfigurationBufferList.FirstOrDefault(c => c.reference == CNETConstantes.PMS && c.attribute == CNETConstantes.PMS_SETTING_ArchivePath);

            //    if (archivePathConfig == null)
            //    {
            //        splitContainer1.Panel1Collapsed = true;
            //        XtraMessageBox.Show("Unable to get Report Archive save path. Please check your configuration on Server Management and press refresh button.", "CNET_2016", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    }
            //    else
            //    {
            //        reportArchivePath = archivePathConfig.currentValue;
            //        splitContainer1.Panel2Collapsed = true;
            //    }
            //}

        }



        private void SearchByDateEnd()
        {
            bbnDateEnd.Reset();
            DateTime date = currentDate;
            issueDateEnd = date;
        }

        private void SaveAllDailyResidentSummary(List<DailyResidentSummaryReport> drsCols, DateTime selectedDate)
        {
            try
            {
                //DateTime begining = Convert.ToDateTime("0001-01-01");
                //List<DailyResidentSummaryDTO> beginingDailyResidentSummarylists = UIProcessManager.GetDailyResidentSummaryPrevBalance(null, begining, SelectedHotelcode);

                //if (beginingDailyResidentSummarylists != null && beginingDailyResidentSummarylists.Count > 0)
                //{
                //    beginingDailyResidentSummarylists.ForEach(x => PMSUIProcessManager.DeleteDailyResidentSummary(x.code));
                //}


                DateTime? SystemDateTime = UIProcessManager.GetServiceTime();
                if (drsCols != null && drsCols.Count > 0)
                {
                    if (DailyResidentSummaryActDef == null || DailyResidentSummaryActDef.Id == null)
                    {
                        XtraMessageBox.Show(String.Format("Please Fix Daily Resident Summary Voucher Activity!"), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    DateTime CurrentDateTime = selectedDate;
                    var max = drsCols.Count;
                    int min = 0;
                    int Voucherid = 0;
                    string Vouchercode = "";
                    List<DailyResidentSummaryDTO> DailyResidentSummarylists = UIProcessManager.GetDailyResidentSummaryBydateandconsigneeUnit(selectedDate, SelectedHotelcode.Value);
                    if (DailyResidentSummarylists != null && DailyResidentSummarylists.Count > 0)
                    {
                        DailyResidentSummarylists.ForEach(x => UIProcessManager.DeleteDailyResidentSummaryById(x.Id));
                    }

                    List<VoucherDTO> DailyVoucher = UIProcessManager.GetVoucherBydefinitionandoriginConsigneeUnitandissuedDate(CNETConstantes.Daily_Resident_Summary_Voucher, SelectedHotelcode.Value, CurrentDateTime.Date);
                    if (DailyVoucher != null && DailyVoucher.Count > 0)
                    {
                        Voucherid = DailyVoucher.FirstOrDefault().Id;
                        VoucherDTO DailyVou = DailyVoucher.FirstOrDefault();
                        decimal grandTotal = drsCols.Sum(x => Convert.ToDecimal(x.TodayTotal));
                        if (DailyVou.GrandTotal != grandTotal)
                        {
                            DailyVou.GrandTotal = grandTotal;
                            UIProcessManager.UpdateVoucher(DailyVou);
                        }
                    }
                    else
                    {
                        Progress_Reporter.Show_Progress("Getting Id Generation", "Please Wait..");
                        String generateID = UIProcessManager.IdGenerater("Voucher", CNETConstantes.Daily_Resident_Summary_Voucher, 0, LocalBuffer.LocalBuffer.CurrentDeviceConsigneeUnit.Value, false, LocalBuffer.LocalBuffer.CurrentDevice.Id);
                        if (!string.IsNullOrEmpty(generateID))
                        {
                            Vouchercode = generateID;
                        }
                        else
                        {
                            XtraMessageBox.Show(String.Format("Daily Resident Summary Voucher ID Generation Failed!"), "ID Generator", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        VoucherBuffer Dailvoubuffer = new VoucherBuffer();
                        Dailvoubuffer.Voucher = new VoucherDTO()
                        {
                            Code = Vouchercode,
                            Definition = CNETConstantes.Daily_Resident_Summary_Voucher,
                            Type = CNETConstantes.TRANSACTIONTYPENORMALTXN,
                            IssuedDate = CurrentDateTime,
                            IsIssued = true,
                            Year = CurrentDateTime.Year,
                            Month = CurrentDateTime.Month,
                            Day = CurrentDateTime.Day,
                            IsVoid = false,
                            GrandTotal = drsCols.Sum(x => Convert.ToDecimal(x.TodayTotal)),
                            LastState = DailyResidentSummaryobjectstate.Value
                        };

                        Dailvoubuffer.Activity = new ActivityDTO()
                        {
                            Pointer = CNETConstantes.VOUCHER_COMPONENET,
                            ActivityDefinition = DailyResidentSummaryActDef.Id,
                            TimeStamp = SystemDateTime.Value,
                            ConsigneeUnit = LocalBuffer.LocalBuffer.CurrentDeviceConsigneeUnit.Value,
                            Device = LocalBuffer.LocalBuffer.CurrentDevice.Id,
                            User = LocalBuffer.LocalBuffer.CurrentLoggedInUser.Id,
                            Year = SystemDateTime.Value.Year,
                            Platform = "1",
                        Month = SystemDateTime.Value.Month,
                            Day = SystemDateTime.Value.Day,
                        };



                        Dailvoubuffer.Voucher.LastUser = LocalBuffer.LocalBuffer.CurrentLoggedInUser.Id;
                        Dailvoubuffer.Voucher.LastDevice = LocalBuffer.LocalBuffer.CurrentDevice.Id;
                        Dailvoubuffer.TransactionCurrencyBuffer = null;

                       ResponseModel<VoucherBuffer> saved = UIProcessManager.CreateVoucherBuffer(Dailvoubuffer);

                        if (saved == null || !saved.Success)
                        {
                            XtraMessageBox.Show(String.Format("Saving Daily Resident Summary Voucher Failed!"+Environment.NewLine+ saved.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        Voucherid = saved.Data.Voucher.Id;
                    }



                    foreach (var Dailyres in drsCols)
                    {
                        min += 1;
                        if (Dailyres.Room == null) Dailyres.Room = "";

                        Progress_Reporter.Show_Progress("Saving to Database {0} of {1}", "Please Wait ........");
                        dailyResidentSummary = new DailyResidentSummaryDTO();
                        dailyResidentSummary.Voucher = Voucherid;
                        dailyResidentSummary.Date = CurrentDateTime;
                        dailyResidentSummary.Reference = Dailyres.Reg_No;
                        dailyResidentSummary.Companycode = Dailyres.ConsigneeCode;
                        dailyResidentSummary.Guestcode = Dailyres.GuestId;
                        dailyResidentSummary.Guest = Dailyres.Guest;
                        dailyResidentSummary.Company = Dailyres.Company;
                        dailyResidentSummary.Room = Dailyres.Room;
                        dailyResidentSummary.RateCodeHeader = "";
                        dailyResidentSummary.RoomRevenu = (Dailyres.RoomRevenue);
                        dailyResidentSummary.Package = (Dailyres.Package);
                        dailyResidentSummary.ServiceCharge = (Dailyres.ServiceCharge);
                        dailyResidentSummary.Vat = (Dailyres.VAT);
                        dailyResidentSummary.RoomTotal = (Dailyres.RoomTotal);
                        dailyResidentSummary.PosCharge = (Dailyres.POSCharge);
                        dailyResidentSummary.ToDayTotal = (Dailyres.TodayTotal);
                        dailyResidentSummary.Bbf = (Dailyres.BBF);
                        dailyResidentSummary.ToDateTotal = (Dailyres.toDateTotal);
                        dailyResidentSummary.Payment = (Dailyres.Payment);
                        dailyResidentSummary.Discount = (Dailyres.Discount);
                        dailyResidentSummary.Paidout = (Dailyres.Paidout);
                        dailyResidentSummary.Bcf = (Dailyres.BCF);
                        dailyResidentSummary.OutStanding = (Dailyres.Outstanding);
                        dailyResidentSummary.ConsigneeUnit = SelectedHotelcode.Value;
                        UIProcessManager.CreateDailyResidentSummary(dailyResidentSummary);

                    }
                    Progress_Reporter.Close_Progress();
                    //XtraMessageBox.Show("Saved All Successfully!", "Daily Resident Summary", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    List<DailyResidentSummaryDTO> DailyResidentSummarylists = UIProcessManager.GetDailyResidentSummaryBydateandconsigneeUnit(selectedDate, SelectedHotelcode.Value);
                    if (DailyResidentSummarylists != null && DailyResidentSummarylists.Count > 0)
                    {
                        DailyResidentSummarylists.ForEach(x => UIProcessManager.DeleteDailyResidentSummaryById(x.Id));
                    }
                }
            }
            catch
            {
            }
        }

       




        //private Tuple<string, string> CalculateMTDandYTD(string reference, decimal todayTotal, List<ReportHistory> previousHistory, DateTime selectedDate, DateTime prevDate, DateTime nextDate)
        //{
        //    string mtd = "0.00";
        //    string ytd = "0.00";
        //    var prevHistory = previousHistory.FirstOrDefault(h => h.reference == reference);
        //    if (prevHistory == null)
        //    {
        //        prevHistory = new ReportHistory() { mtd = "0.00", ytd = "0.00", reportValue = "0.00", date = prevDate };
        //    }
        //    decimal prevMTD = Convert.ToDecimal(prevHistory.mtd);
        //    decimal prevYTD = Convert.ToDecimal(prevHistory.ytd);
        //    if (selectedDate.Month == prevDate.Month && selectedDate.Year == prevDate.Year)
        //    {
        //        mtd = (prevMTD + todayTotal).ToString();
        //        ytd = (prevYTD + todayTotal).ToString();
        //    }
        //    else if (selectedDate.Month != prevDate.Month && selectedDate.Year == prevDate.Year)
        //    {
        //        mtd = todayTotal.ToString();
        //        ytd = (prevYTD + todayTotal).ToString();
        //    }
        //    else
        //    {
        //        mtd = todayTotal.ToString();
        //        ytd = todayTotal.ToString();
        //    }
        //    return new Tuple<string, string>(mtd, ytd);
        //}


        //private bool SaveDailyBusinessReport(List<ReportHistory> todaysExistedHistory, List<ReportHistory> previousHistory, DateTime selectedDate)
        //{
        //    try
        //    {

        //        DateTime prevDate = selectedDate.Subtract(TimeSpan.FromDays(1));
        //        DateTime nextDate = selectedDate.Add(TimeSpan.FromDays(1));
        //        DateTime lastYear = selectedDate.Subtract(TimeSpan.FromDays(365));

        //        if (previousHistory == null)
        //        {
        //            previousHistory = new List<ReportHistory>();
        //        }

        //        //Get last year's Report History
        //        List<ReportHistory> lastYearDBRHistory = PMSUIProcessManager.GetReportHistoryByReportAndDate(CNETConstantes.DAILY_BUSINESS_REPORT, lastYear);
        //        if (lastYearDBRHistory == null)
        //        {
        //            lastYearDBRHistory = new List<ReportHistory>();
        //        }



        //        List<ReportHistory> repoHistoryToSave = new List<ReportHistory>();
        //        List<ReportHistory> repoHistoryToUpdate = new List<ReportHistory>();

        //        foreach (var dailyBusiness in _dailyBusinessReports)
        //        {
        //            string prefix = dailyBusiness.ReportItems.ToLower();

        //            #region Daily Resident Summary
        //            if (dailyBusiness.dailyResidentSummaryList != null)
        //            {
        //                foreach (var resSummary in dailyBusiness.dailyResidentSummaryList)
        //                {

        //                    string reference = prefix + "_";
        //                    if (resSummary.Particulars != null)
        //                    {
        //                        reference = prefix + "_" + resSummary.Particulars.ToLower();
        //                    }
        //                    else
        //                    {
        //                        reference = prefix + "_Unknown ";
        //                    }

        //                    ReportHistory rHistory = new ReportHistory()
        //                    {
        //                        code = string.Empty,
        //                        report = CNETConstantes.DAILY_BUSINESS_REPORT,
        //                        description = resSummary.Particulars,
        //                        reference = reference,
        //                        date = selectedDate,
        //                        reportValue = resSummary.TotalToday.ToString(),
        //                        remark = ""

        //                    };

        //                    Tuple<string, string> mtdAndytdTuple = CalculateMTDandYTD(reference, resSummary.TotalToday, previousHistory, selectedDate, prevDate, nextDate);
        //                    rHistory.mtd = mtdAndytdTuple.Item1;
        //                    rHistory.ytd = mtdAndytdTuple.Item2;

        //                    resSummary.MonthTodate = Convert.ToDecimal(rHistory.mtd);
        //                    resSummary.YearToDate = Convert.ToDecimal(rHistory.ytd);

        //                    //get last year's status
        //                    var lastYearRepo = lastYearDBRHistory.FirstOrDefault(h => h.reference == reference);
        //                    if (lastYearRepo != null)
        //                    {
        //                        resSummary.LastYearDate = Convert.ToDecimal(lastYearRepo.reportValue);
        //                        resSummary.LastYearMonth = Convert.ToDecimal(lastYearRepo.mtd);
        //                        resSummary.LastYear = Convert.ToDecimal(lastYearRepo.ytd);
        //                    }
        //                    else
        //                    {
        //                        resSummary.LastYearDate = 0.00m;
        //                        resSummary.LastYearMonth = 0.00m;
        //                        resSummary.LastYear = 0.00m;
        //                    }


        //                    var repHist = todaysExistedHistory.FirstOrDefault(h => h.reference == reference);
        //                    if (repHist != null)
        //                    {
        //                        rHistory.code = repHist.code;
        //                        repoHistoryToUpdate.Add(rHistory);
        //                    }
        //                    else
        //                    {
        //                        repoHistoryToSave.Add(rHistory);
        //                    }

        //                }
        //            }

        //            #endregion

        //            #region Income Analysis
        //            if (dailyBusiness.incomeAnalysis != null)
        //            {
        //                foreach (var incomeAna in dailyBusiness.incomeAnalysis)
        //                {

        //                    string reference = prefix + "_";
        //                    if (incomeAna.Particulars != null)
        //                    {
        //                        reference = prefix + "_" + incomeAna.Particulars.ToLower();
        //                    }
        //                    else
        //                    {
        //                        reference = prefix + "_Unknown Income";
        //                    }


        //                    ReportHistory rHistory = new ReportHistory()
        //                    {
        //                        code = string.Empty,
        //                        report = CNETConstantes.DAILY_BUSINESS_REPORT,
        //                        description = incomeAna.Particulars,
        //                        reference = reference,
        //                        date = selectedDate,
        //                        reportValue = incomeAna.TotalToday.ToString(),
        //                        remark = ""

        //                    };

        //                    if (!incomeAna.IsPrevious)
        //                    {
        //                        Tuple<string, string> mtdAndytdTuple = CalculateMTDandYTD(reference, incomeAna.TotalToday, previousHistory, selectedDate, prevDate, nextDate);
        //                        rHistory.mtd = mtdAndytdTuple.Item1;
        //                        rHistory.ytd = mtdAndytdTuple.Item2;

        //                        incomeAna.MonthTodate = Convert.ToDecimal(rHistory.mtd);
        //                        incomeAna.YearToDate = Convert.ToDecimal(rHistory.ytd);


        //                    }
        //                    else
        //                    {
        //                        rHistory.mtd = incomeAna.MonthTodate.ToString();
        //                        rHistory.ytd = incomeAna.YearToDate.ToString();
        //                    }



        //                    //get last year's status
        //                    var lastYearRepo = lastYearDBRHistory.FirstOrDefault(h => h.reference == reference);
        //                    if (lastYearRepo != null)
        //                    {
        //                        incomeAna.LastYearDate = Convert.ToDecimal(lastYearRepo.reportValue);
        //                        incomeAna.LastYearMonth = Convert.ToDecimal(lastYearRepo.mtd);
        //                        incomeAna.LastYear = Convert.ToDecimal(lastYearRepo.ytd);
        //                    }
        //                    else
        //                    {
        //                        incomeAna.LastYearDate = 0.00m;
        //                        incomeAna.LastYearMonth = 0.00m;
        //                        incomeAna.LastYear = 0.00m;
        //                    }


        //                    var repHist = todaysExistedHistory.FirstOrDefault(h => h.reference == reference);
        //                    if (repHist != null)
        //                    {
        //                        rHistory.code = repHist.code;
        //                        repoHistoryToUpdate.Add(rHistory);
        //                    }
        //                    else
        //                    {
        //                        repoHistoryToSave.Add(rHistory);
        //                    }

        //                }
        //            }

        //            #endregion

        //            #region Sales Center
        //            if (dailyBusiness.salesCenters != null)
        //            {
        //                foreach (var salesCenter in dailyBusiness.salesCenters)
        //                {
        //                    string reference = prefix + "_";
        //                    if (salesCenter.MachineName != null)
        //                    {
        //                        reference = prefix + "_" + salesCenter.MachineName.ToLower();
        //                    }
        //                    else
        //                    {
        //                        reference = prefix + "_Unknown Device";
        //                    }
        //                    ReportHistory rHistory = new ReportHistory()
        //                    {
        //                        code = string.Empty,
        //                        report = CNETConstantes.DAILY_BUSINESS_REPORT,
        //                        description = salesCenter.MachineName == null ? "Unknown Device" : salesCenter.MachineName,
        //                        reference = reference,
        //                        date = selectedDate,
        //                        reportValue = salesCenter.TotalToday.ToString(),
        //                        remark = string.Format("{0},{1},{2}", salesCenter.Room, salesCenter.CityLedger, salesCenter.Cash)

        //                    };

        //                    if (!salesCenter.IsPrevious)
        //                    {
        //                        Tuple<string, string> mtdAndytdTuple = CalculateMTDandYTD(reference, salesCenter.TotalToday, previousHistory, selectedDate, prevDate, nextDate);
        //                        rHistory.mtd = mtdAndytdTuple.Item1;
        //                        rHistory.ytd = mtdAndytdTuple.Item2;

        //                        salesCenter.MonthToDate = Convert.ToDecimal(rHistory.mtd);
        //                        salesCenter.YearToDate = Convert.ToDecimal(rHistory.ytd);
        //                    }
        //                    else
        //                    {
        //                        rHistory.mtd = salesCenter.MonthToDate.ToString();
        //                        rHistory.ytd = salesCenter.YearToDate.ToString();
        //                    }

        //                    //get last year's status
        //                    var lastYearRepo = lastYearDBRHistory.FirstOrDefault(h => h.reference == reference);
        //                    if (lastYearRepo != null)
        //                    {
        //                        salesCenter.LastYearDate = Convert.ToDecimal(lastYearRepo.reportValue);
        //                        salesCenter.LastYearMonth = Convert.ToDecimal(lastYearRepo.mtd);
        //                        salesCenter.LastYear = Convert.ToDecimal(lastYearRepo.ytd);
        //                    }
        //                    else
        //                    {
        //                        salesCenter.LastYearDate = 0.00m;
        //                        salesCenter.LastYearMonth = 0.00m;
        //                        salesCenter.LastYear = 0.00m;
        //                    }


        //                    var repHist = todaysExistedHistory.FirstOrDefault(h => h.reference == reference);
        //                    if (repHist != null)
        //                    {
        //                        rHistory.code = repHist.code;
        //                        repoHistoryToUpdate.Add(rHistory);
        //                    }
        //                    else
        //                    {
        //                        repoHistoryToSave.Add(rHistory);
        //                    }

        //                }
        //            }

        //            #endregion

        //            #region Occupancy Summary
        //            if (dailyBusiness.occupancySummary != null)
        //            {
        //                foreach (var occupancy in dailyBusiness.occupancySummary)
        //                {

        //                    string reference = prefix + "_";
        //                    if (occupancy.RoomType != null)
        //                    {
        //                        reference = prefix + "_" + occupancy.RoomType.ToLower();
        //                    }
        //                    else
        //                    {
        //                        reference = prefix + "_Unknown Room Type";
        //                    }

        //                    ReportHistory rHistory = new ReportHistory()
        //                    {
        //                        code = string.Empty,
        //                        report = CNETConstantes.DAILY_BUSINESS_REPORT,
        //                        description = occupancy.RoomType,
        //                        reference = reference,
        //                        date = selectedDate,
        //                        reportValue = occupancy.Occupancy.ToString(),
        //                        mtd = occupancy.MTD.ToString(),
        //                        ytd = occupancy.YTD.ToString(),
        //                        remark = string.Format("{0},{1},{2},{3}", occupancy.Rooms, occupancy.Occupied, occupancy.Vacant, occupancy.ADR)

        //                    };

        //                    //Tuple<string, string> mtdAndytdTuple = CalculateMTDandYTD(reference, occupancy.Occupancy, previousHistory, selectedDate, prevDate, nextDate);
        //                    //rHistory.mtd = mtdAndytdTuple.Item1;
        //                    //rHistory.ytd = mtdAndytdTuple.Item2;

        //                    // occupancy.MTD = Convert.ToDecimal(rHistory.mtd);
        //                    // occupancy.YTD = Convert.ToDecimal(rHistory.ytd);

        //                    //get last year's status
        //                    var lastYearRepo = lastYearDBRHistory.FirstOrDefault(h => h.reference == reference);
        //                    if (lastYearRepo != null)
        //                    {
        //                        occupancy.LastYearDate = Convert.ToDecimal(lastYearRepo.reportValue);
        //                        occupancy.LastYearMonth = Convert.ToDecimal(lastYearRepo.mtd);
        //                        occupancy.LastYear = Convert.ToDecimal(lastYearRepo.ytd);
        //                    }
        //                    else
        //                    {
        //                        occupancy.LastYearDate = 0.00m;
        //                        occupancy.LastYearMonth = 0.00m;
        //                        occupancy.LastYear = 0.00m;
        //                    }


        //                    var repHist = todaysExistedHistory.FirstOrDefault(h => h.reference == reference);
        //                    if (repHist != null)
        //                    {
        //                        rHistory.code = repHist.code;
        //                        repoHistoryToUpdate.Add(rHistory);
        //                    }
        //                    else
        //                    {
        //                        repoHistoryToSave.Add(rHistory);
        //                    }

        //                }
        //            }

        //            #endregion

        //        }

        //        //Save Report History
        //        bool saveFlag = PMSUIProcessManager.CreateReportHistory(repoHistoryToSave);

        //        bool updateFlag = PMSUIProcessManager.UpdateReportHistory(repoHistoryToUpdate);


        //        if (saveFlag || updateFlag) return true;
        //        else return false;


        //    }
        //    catch (Exception ex)
        //    {
        //        XtraMessageBox.Show("Error in saving daily business report. DETAIL::" + ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //        // Home.ShowModalInfoMessage("Error in saving daily business report. DETAIL::" + ex.Message, "ERROR");
        //        return false;
        //    }
        //}


        //private bool SaveManagerialFlush(List<ReportHistory> todaysExistedHistory, DateTime selectedDate)
        //{
        //    try
        //    {

        //        DateTime prevDate = selectedDate.Subtract(TimeSpan.FromDays(1));
        //        DateTime nextDate = selectedDate.Add(TimeSpan.FromDays(1));
        //        DateTime lastYear = selectedDate.Subtract(TimeSpan.FromDays(365));

        //        List<ReportHistory> previousHistory = PMSUIProcessManager.GetReportHistoryByReportAndDate(CNETConstantes.MANAGERIAL_FLASH, prevDate);
        //        if (previousHistory == null)
        //        {
        //            previousHistory = new List<ReportHistory>();
        //        }

        //        //Get last year's Report History
        //        List<ReportHistory> lastYearDBRHistory = PMSUIProcessManager.GetReportHistoryByReportAndDate(CNETConstantes.MANAGERIAL_FLASH, lastYear);
        //        if (lastYearDBRHistory == null)
        //        {
        //            lastYearDBRHistory = new List<ReportHistory>();
        //        }



        //        List<ReportHistory> repoHistoryToSave = new List<ReportHistory>();
        //        List<ReportHistory> repoHistoryToUpdate = new List<ReportHistory>();

        //        foreach (var mangFlash in _managerialFlashList)
        //        {
        //            string reference = mangFlash.ReportItem.ToLower();

        //            ReportHistory rHistory = new ReportHistory()
        //            {
        //                code = string.Empty,
        //                report = CNETConstantes.MANAGERIAL_FLASH,
        //                description = mangFlash.ReportItem,
        //                reference = reference,
        //                date = selectedDate,
        //                reportValue = mangFlash.CurrentDate,
        //                remark = ""

        //            };

        //            decimal todaysTotal = 0;
        //            try
        //            {
        //                todaysTotal = Convert.ToDecimal(mangFlash.CurrentDate);
        //            }
        //            catch (Exception ex) { todaysTotal = 0; }

        //            Tuple<string, string> mtdAndytdTuple = CalculateMTDandYTD(reference, todaysTotal, previousHistory, selectedDate, prevDate, nextDate);
        //            rHistory.mtd = mtdAndytdTuple.Item1;
        //            rHistory.ytd = mtdAndytdTuple.Item2;

        //            mangFlash.CurrentMonth = string.Format("{0:N}", Convert.ToDecimal(rHistory.mtd));
        //            mangFlash.CurrentYear = string.Format("{0:N}", Convert.ToDecimal(rHistory.ytd));

        //            //get last year's status
        //            var lastYearRepo = lastYearDBRHistory.FirstOrDefault(h => h.reference == reference);
        //            if (lastYearRepo != null)
        //            {
        //                mangFlash.LastYearToday = string.Format("{0:N}", Convert.ToDecimal(lastYearRepo.reportValue));
        //                mangFlash.LastYearThisMonth = string.Format("{0:N}", Convert.ToDecimal(lastYearRepo.mtd));
        //                mangFlash.LastYearNow = string.Format("{0:N}", Convert.ToDecimal(lastYearRepo.ytd));

        //            }
        //            else
        //            {
        //                mangFlash.LastYearToday = "0.00";
        //                mangFlash.LastYearThisMonth = "0.00";
        //                mangFlash.LastYearNow = "0.00";
        //            }


        //            var repHist = todaysExistedHistory.FirstOrDefault(h => h.reference == reference);
        //            if (repHist != null)
        //            {
        //                rHistory.code = repHist.code;
        //                repoHistoryToUpdate.Add(rHistory);
        //            }
        //            else
        //            {
        //                repoHistoryToSave.Add(rHistory);
        //            }


        //        }



        //        bool saveFlag = PMSUIProcessManager.CreateReportHistory(repoHistoryToSave);

        //        bool updateFlag = PMSUIProcessManager.UpdateReportHistory(repoHistoryToUpdate);


        //        if (saveFlag || updateFlag) return true;
        //        else return false;


        //    }
        //    catch (Exception ex)
        //    {
        //        XtraMessageBox.Show("Error in saving Managerial Flush report. DETAIL::" + ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //        //  Home.ShowModalInfoMessage("Error in saving Managerial Flush report. DETAIL::" + ex.Message, "ERROR");
        //        return false;
        //    }
        //}

        #endregion

        #region Events Handlers


        private void SearchByUser(object sender, EventArgs e)
        {
            var editor = (sender as SearchLookUpEdit);

            if (editor.EditValue != null && (CNETConstantes.CASHIER_SUMMARY == selectedReportName || CNETConstantes.CASH_DROPPED_REPORT == selectedReportName))
            {
                string editorGsl = editor.EditValue.ToString();
                user = Convert.ToInt32(editorGsl);
            }
            else
            {
                user = null;
            }
        }

        private void gvReports_MasterRowExpanded(object sender, DevExpress.XtraGrid.Views.Grid.CustomMasterRowEventArgs e)
        {
            if (selectedReportName == CNETConstantes.CASH_DROPPED_REPORT)
            {
                GridView detailView = sender as GridView;
                GridView detailsView = detailView.GetDetailView(e.RowHandle, 0) as GridView;

                if (detailsView != null)
                {
                    detailsView.DoubleClick += GvReports_DoubleClick;

                    detailsView.Columns["SN"].MaxWidth = 25;
                    detailsView.Columns["SN"].AppearanceCell.TextOptions.HAlignment = HorzAlignment.Near;

                    detailsView.Columns["Date"].Width = 70;
                    detailsView.Columns["VoucherID"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    detailsView.Columns["VoucherID"].SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Custom;
                    detailsView.Columns["VoucherID"].SummaryItem.DisplayFormat = " Total= ";

                    detailsView.Columns["RoomCharge"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    detailsView.Columns["RoomCharge"].SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Sum;
                    detailsView.Columns["RoomCharge"].SummaryItem.DisplayFormat = " {0:N2}";

                    detailsView.Columns["Cash"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    detailsView.Columns["Cash"].DisplayFormat.FormatString = "{0:N2}";
                    detailsView.Columns["Cash"].SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Sum;
                    detailsView.Columns["Cash"].SummaryItem.DisplayFormat = " {0:N2}";

                    detailsView.Columns["Allowance"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    detailsView.Columns["Allowance"].DisplayFormat.FormatString = "{0:N2}";
                    detailsView.Columns["Allowance"].SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Sum;
                    detailsView.Columns["Allowance"].SummaryItem.DisplayFormat = " {0:N2}";

                    detailsView.Columns["CityLedger"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    detailsView.Columns["CityLedger"].DisplayFormat.FormatString = "{0:N2}";
                    detailsView.Columns["CityLedger"].SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Sum;
                    detailsView.Columns["CityLedger"].SummaryItem.DisplayFormat = " {0:N2}";

                    detailsView.BestFitColumns();
                }


                GridView detailsView1 = detailView.GetDetailView(e.RowHandle, 1) as GridView;

                if (detailsView1 != null)
                {
                    detailsView1.DoubleClick += GvReports_DoubleClick;
                    detailsView1.Columns["SN"].MaxWidth = 25;
                    detailsView1.Columns["SN"].AppearanceCell.TextOptions.HAlignment = HorzAlignment.Near;

                    detailsView1.Columns["VoucherID"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    detailsView1.Columns["VoucherID"].SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Custom;
                    detailsView1.Columns["VoucherID"].SummaryItem.DisplayFormat = " Total = ";

                    detailsView1.Columns["Amount"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    detailsView1.Columns["Amount"].DisplayFormat.FormatString = "{0:N2}";
                    detailsView1.Columns["Amount"].SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Sum;
                    detailsView1.Columns["Amount"].SummaryItem.DisplayFormat = " {0:N2}";

                    detailsView1.BestFitColumns();
                }


                GridView detailsView2 = detailView.GetDetailView(e.RowHandle, 2) as GridView;

                if (detailsView2 != null)
                {
                    detailsView2.DoubleClick += GvReports_DoubleClick;

                    detailsView2.Columns["SN"].MaxWidth = 25;
                    detailsView2.Columns["SN"].AppearanceCell.TextOptions.HAlignment = HorzAlignment.Near;

                    detailsView2.Columns["VoucherID"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    detailsView2.Columns["VoucherID"].SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Custom;
                    detailsView2.Columns["VoucherID"].SummaryItem.DisplayFormat = " Total = ";

                    detailsView2.Columns["Amount"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    detailsView2.Columns["Amount"].DisplayFormat.FormatString = "{0:N2}";
                    detailsView2.Columns["Amount"].SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Sum;
                    detailsView2.Columns["Amount"].SummaryItem.DisplayFormat = " {0:N2}";

                    detailsView2.BestFitColumns();
                }


            }
            else if (selectedReportName == CNETConstantes.STATUS_REPORT)
            {
                GridView detailView = sender as GridView;
                GridView detailView1 = detailView.GetDetailView(e.RowHandle, 0) as GridView;
                if (detailView1 != null)
                {

                    detailView1.Columns["SN"].Width = 25;
                    detailView1.Columns["SN"].AppearanceCell.TextOptions.HAlignment = HorzAlignment.Center;
                    detailView1.Columns["RoomType"].Width = 120;
                    detailView1.Columns["Floor"].Width = 70;
                    detailView1.Columns["RoomNumber"].Width = 100;
                    detailView1.Columns["RoomStatus"].Width = 100;
                }
            }
            else if (selectedReportName == CNETConstantes.DAILY_BUSINESS_REPORT)
            {
                GridView detailView = sender as GridView;

                GridView detailsViewFirst = detailView.GetDetailView(e.RowHandle, 0) as GridView;
                if (detailsViewFirst != null)
                {
                    detailsViewFirst.Columns["SN"].MaxWidth = 25;
                    detailsViewFirst.Columns["SN"].Width = 25;
                    detailsViewFirst.Columns["SN"].AppearanceCell.TextOptions.HAlignment = HorzAlignment.Near;

                    //detailsViewFirst.Columns["TotalToday"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    //detailsViewFirst.Columns["TotalToday"].DisplayFormat.FormatString = "{0:N2}";

                    //detailsViewFirst.Columns["MonthTodate"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    //detailsViewFirst.Columns["MonthTodate"].DisplayFormat.FormatString = "{0:N2}";

                    //detailsViewFirst.Columns["YearToDate"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    //detailsViewFirst.Columns["YearToDate"].DisplayFormat.FormatString = "{0:N2}";

                    //detailsViewFirst.Columns["LastYearDate"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    //detailsViewFirst.Columns["LastYearDate"].DisplayFormat.FormatString = "{0:N2}";

                    //detailsViewFirst.Columns["LastYearMonth"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    //detailsViewFirst.Columns["LastYearMonth"].DisplayFormat.FormatString = "{0:N2}";

                    //detailsViewFirst.Columns["LastYear"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    //detailsViewFirst.Columns["LastYear"].DisplayFormat.FormatString = "{0:N2}";


                    detailsViewFirst.BestFitColumns();
                }


                GridView detailsView = detailView.GetDetailView(e.RowHandle, 1) as GridView;

                if (detailsView != null)
                {
                    detailsView.Columns["SN"].MaxWidth = 25;
                    detailsView.Columns["SN"].Width = 25;
                    detailsView.Columns["SN"].AppearanceCell.TextOptions.HAlignment = HorzAlignment.Near;

                    detailsView.Columns["Particulars"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    detailsView.Columns["Particulars"].SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Custom;
                    detailsView.Columns["Particulars"].SummaryItem.DisplayFormat = " Grand Total= ";

                    detailsView.Columns["TotalToday"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    detailsView.Columns["TotalToday"].DisplayFormat.FormatString = "{0:N2}";
                    detailsView.Columns["TotalToday"].SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Sum;
                    detailsView.Columns["TotalToday"].SummaryItem.DisplayFormat = " {0:N2}";

                    detailsView.Columns["MonthTodate"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    detailsView.Columns["MonthTodate"].DisplayFormat.FormatString = "{0:N2}";
                    detailsView.Columns["MonthTodate"].SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Sum;
                    detailsView.Columns["MonthTodate"].SummaryItem.DisplayFormat = " {0:N2}";

                    detailsView.Columns["YearToDate"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    detailsView.Columns["YearToDate"].DisplayFormat.FormatString = "{0:N2}";
                    detailsView.Columns["YearToDate"].SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Sum;
                    detailsView.Columns["YearToDate"].SummaryItem.DisplayFormat = " {0:N2}";

                    detailsView.Columns["LastYearDate"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    detailsView.Columns["LastYearDate"].DisplayFormat.FormatString = "{0:N2}";
                    detailsView.Columns["LastYearDate"].SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Sum;
                    detailsView.Columns["LastYearDate"].SummaryItem.DisplayFormat = " {0:N2}";

                    detailsView.Columns["LastYearMonth"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    detailsView.Columns["LastYearMonth"].DisplayFormat.FormatString = "{0:N2}";
                    detailsView.Columns["LastYearMonth"].SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Sum;
                    detailsView.Columns["LastYearMonth"].SummaryItem.DisplayFormat = " {0:N2}";

                    detailsView.Columns["LastYear"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    detailsView.Columns["LastYear"].DisplayFormat.FormatString = "{0:N2}";
                    detailsView.Columns["LastYear"].SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Sum;
                    detailsView.Columns["LastYear"].SummaryItem.DisplayFormat = " {0:N2}";

                    detailsView.BestFitColumns();
                }


                GridView detailsView1 = detailView.GetDetailView(e.RowHandle, 2) as GridView;

                if (detailsView1 != null)
                {
                    detailsView1.Columns["SN"].MaxWidth = 25;
                    detailsView1.Columns["SN"].Width = 25;
                    detailsView1.Columns["SN"].AppearanceCell.TextOptions.HAlignment = HorzAlignment.Near;

                    detailsView1.Columns["MachineName"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    detailsView1.Columns["MachineName"].SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Custom;
                    detailsView1.Columns["MachineName"].SummaryItem.DisplayFormat = " Total = ";

                    detailsView1.Columns["Cash"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    detailsView1.Columns["Cash"].DisplayFormat.FormatString = "{0:N2}";
                    detailsView1.Columns["Cash"].SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Sum;
                    detailsView1.Columns["Cash"].SummaryItem.DisplayFormat = " {0:N2}";

                    detailsView1.Columns["Room"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    detailsView1.Columns["Room"].SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Sum;
                    detailsView1.Columns["Room"].SummaryItem.DisplayFormat = " {0:N2}";

                    detailsView1.Columns["CityLedger"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    detailsView1.Columns["CityLedger"].DisplayFormat.FormatString = "{0:N2}";
                    detailsView1.Columns["CityLedger"].SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Sum;
                    detailsView1.Columns["CityLedger"].SummaryItem.DisplayFormat = " {0:N2}";


                    detailsView1.Columns["TotalToday"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    detailsView1.Columns["TotalToday"].DisplayFormat.FormatString = "{0:N2}";
                    detailsView1.Columns["TotalToday"].SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Sum;
                    detailsView1.Columns["TotalToday"].SummaryItem.DisplayFormat = " {0:N2}";

                    detailsView1.Columns["MonthToDate"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    detailsView1.Columns["MonthToDate"].DisplayFormat.FormatString = "{0:N2}";
                    detailsView1.Columns["MonthToDate"].SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Sum;
                    detailsView1.Columns["MonthToDate"].SummaryItem.DisplayFormat = " {0:N2}";

                    detailsView1.Columns["YearToDate"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    detailsView1.Columns["YearToDate"].DisplayFormat.FormatString = "{0:N2}";
                    detailsView1.Columns["YearToDate"].SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Sum;
                    detailsView1.Columns["YearToDate"].SummaryItem.DisplayFormat = " {0:N2}";

                    detailsView1.Columns["LastYearDate"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    detailsView1.Columns["LastYearDate"].DisplayFormat.FormatString = "{0:N2}";
                    detailsView1.Columns["LastYearDate"].SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Sum;
                    detailsView1.Columns["LastYearDate"].SummaryItem.DisplayFormat = " {0:N2}";


                    detailsView1.Columns["LastYearMonth"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    detailsView1.Columns["LastYearMonth"].DisplayFormat.FormatString = "{0:N2}";
                    detailsView1.Columns["LastYearMonth"].SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Sum;
                    detailsView1.Columns["LastYearMonth"].SummaryItem.DisplayFormat = " {0:N2}";

                    detailsView1.Columns["LastYear"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    detailsView1.Columns["LastYear"].DisplayFormat.FormatString = "{0:N2}";
                    detailsView1.Columns["LastYear"].SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Sum;
                    detailsView1.Columns["LastYear"].SummaryItem.DisplayFormat = " {0:N2}";

                    detailsView1.BestFitColumns();
                }


                GridView detailsView2 = detailView.GetDetailView(e.RowHandle, 3) as GridView;

                if (detailsView2 != null)
                {

                    detailsView2.Columns["SN"].MaxWidth = 25;
                    detailsView2.Columns["SN"].Width = 25;
                    detailsView2.Columns["SN"].AppearanceCell.TextOptions.HAlignment = HorzAlignment.Near;

                    detailsView2.Columns["Cashier"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    detailsView2.Columns["Cashier"].SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Custom;
                    detailsView2.Columns["Cashier"].SummaryItem.DisplayFormat = " Total = ";

                    detailsView2.Columns["FromGuest"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    detailsView2.Columns["FromGuest"].DisplayFormat.FormatString = "{0:N2}";
                    detailsView2.Columns["FromGuest"].SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Sum;
                    detailsView2.Columns["FromGuest"].SummaryItem.DisplayFormat = " {0:N2}";

                    detailsView2.Columns["AdvancedDeposit"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    detailsView2.Columns["AdvancedDeposit"].DisplayFormat.FormatString = "{0:N2}";
                    detailsView2.Columns["AdvancedDeposit"].SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Sum;
                    detailsView2.Columns["AdvancedDeposit"].SummaryItem.DisplayFormat = " {0:N2}";

                    detailsView2.Columns["FromCredit"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    detailsView2.Columns["FromCredit"].DisplayFormat.FormatString = "{0:N2}";
                    detailsView2.Columns["FromCredit"].SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Sum;
                    detailsView2.Columns["FromCredit"].SummaryItem.DisplayFormat = " {0:N2}";

                    detailsView2.Columns["CashRecieved"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    detailsView2.Columns["CashRecieved"].DisplayFormat.FormatString = "{0:N2}";
                    detailsView2.Columns["CashRecieved"].SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Sum;
                    detailsView2.Columns["CashRecieved"].SummaryItem.DisplayFormat = " {0:N2}";

                    detailsView2.Columns["Total"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    detailsView2.Columns["Total"].DisplayFormat.FormatString = "{0:N2}";
                    detailsView2.Columns["Total"].SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Sum;
                    detailsView2.Columns["Total"].SummaryItem.DisplayFormat = " {0:N2}";

                    detailsView2.BestFitColumns();
                }

                GridView detailsView3 = detailView.GetDetailView(e.RowHandle, 4) as GridView;

                if (detailsView3 != null)
                {
                    detailsView3.CustomSummaryCalculate += DetailsView3_CustomSummaryCalculate;
                    detailsView3.Columns["SN"].MaxWidth = 25;
                    detailsView3.Columns["SN"].Width = 25;
                    detailsView3.Columns["SN"].AppearanceCell.TextOptions.HAlignment = HorzAlignment.Near;

                    detailsView3.Columns["RoomType"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    detailsView3.Columns["RoomType"].SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Custom;
                    detailsView3.Columns["RoomType"].SummaryItem.DisplayFormat = " Total = ";

                    detailsView3.Columns["Rooms"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    detailsView3.Columns["Rooms"].SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Sum;
                    detailsView3.Columns["Rooms"].SummaryItem.DisplayFormat = " {0:N2}";

                    detailsView3.Columns["Occupied"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    detailsView3.Columns["Occupied"].SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Sum;
                    detailsView3.Columns["Occupied"].SummaryItem.DisplayFormat = " {0:N2}";

                    detailsView3.Columns["Vacant"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    detailsView3.Columns["Vacant"].SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Sum;
                    detailsView3.Columns["Vacant"].SummaryItem.DisplayFormat = " {0:N2}";



                    #region Occupancy Summary

                    #region This year Occupancy Summary

                    #region To Day Occupancy Summary

                    detailsView3.Columns["Occupancy"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    detailsView3.Columns["Occupancy"].DisplayFormat.FormatString = " {0:N2} %";

                    Double FullRooms = Convert.ToDouble(detailsView3.Columns["Rooms"].SummaryItem.SummaryValue);
                    Double OccupiedRooms = Convert.ToDouble(detailsView3.Columns["Occupied"].SummaryItem.SummaryValue);
                    Double number = ((OccupiedRooms / FullRooms) * 100);

                    detailsView3.Columns["Occupancy"].SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Custom;
                    detailsView3.Columns["Occupancy"].SummaryItem.DisplayFormat = string.Format("{0:N2} %", number);

                    detailsView3.Columns["Occupancy"].Caption = "Occupancy [%]";



                    #endregion

                    #region This Month Occupancy Summary

                    detailsView3.Columns["MTD"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    detailsView3.Columns["MTD"].DisplayFormat.FormatString = " {0:N2} %";


                    Double MTDnumber = 0;

                    if (MTDnumber == double.NaN)
                        MTDnumber = 0;

                     MTDnumber = ((MTDTotalCoupancy / (MTDTimespan * FullRooms)) * 100);
                    detailsView3.Columns["MTD"].SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Custom;
                    detailsView3.Columns["MTD"].SummaryItem.DisplayFormat = string.Format("{0:N2} %", MTDnumber);
                    detailsView3.Columns["MTD"].Caption = "MTD [%]";





                    #endregion

                    #region This Year Occupancy Summary

                    detailsView3.Columns["YTD"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    detailsView3.Columns["YTD"].DisplayFormat.FormatString = "{0:N2} %";


                    Double YTDnumber = 0;

                     YTDnumber = ((YTDTotalCoupancy / (YTDTimespan * FullRooms)) * 100);

                    if (YTDnumber == double.NaN)
                        YTDnumber = 0;
                    detailsView3.Columns["YTD"].SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Custom;
                    detailsView3.Columns["YTD"].SummaryItem.DisplayFormat = string.Format("{0:N2} %", YTDnumber);
                    detailsView3.Columns["YTD"].Caption = "YTD [%]";


                    #endregion

                    #endregion


                    #region Last year Occupancy Summary

                    #region Last year Day Occupancy Summary

                    detailsView3.Columns["LastYearDate"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    detailsView3.Columns["LastYearDate"].DisplayFormat.FormatString = "{0:N2} %";

                    Double lastyearnumber = 0;
                    lastyearnumber = ((YearDateTotalCoupancy / (FullRooms)) * 100);

                    if (lastyearnumber == double.NaN)
                        lastyearnumber = 0;


                    detailsView3.Columns["LastYearDate"].SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Custom;
                    detailsView3.Columns["LastYearDate"].SummaryItem.DisplayFormat = string.Format("{0:N2} %", lastyearnumber);
                    detailsView3.Columns["LastYearDate"].Caption = "LastYearDate [%]";



                    #endregion

                    #region Last Month Occupancy Summary

                    detailsView3.Columns["LastYearMonth"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    detailsView3.Columns["LastYearMonth"].DisplayFormat.FormatString = "{0:N2} %";

                    Double yearMTDnumber = 0;

                    yearMTDnumber = ((YearMTDTotalCoupancy / (MTDTimespan * FullRooms)) * 100); 

                    if (yearMTDnumber == double.NaN)
                        yearMTDnumber = 0;

                    detailsView3.Columns["LastYearMonth"].SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Custom;
                    detailsView3.Columns["LastYearMonth"].SummaryItem.DisplayFormat = string.Format("{0:N2} %", yearMTDnumber);
                    detailsView3.Columns["LastYearMonth"].Caption = "LastYearMonth [%]";


                    #endregion

                    #region Last Year Occupancy Summary
                    var ccy = detailsView3.Columns["LastYear"];

                    detailsView3.Columns["LastYear"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    detailsView3.Columns["LastYear"].DisplayFormat.FormatString = "{0:N2} %";

                    Double LastYearYTDnumber = 0;

                    LastYearYTDnumber = ((YearYTDTotalCoupancy / (YTDTimespan * FullRooms)) * 100);

                    if (LastYearYTDnumber == double.NaN)
                        LastYearYTDnumber = 0;

                    detailsView3.Columns["LastYear"].SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Custom;
                    detailsView3.Columns["LastYear"].SummaryItem.DisplayFormat = string.Format("{0:N2} %", LastYearYTDnumber);
                    detailsView3.Columns["LastYear"].Caption = "LastYear [%]";


                    #endregion

                    #endregion


                    #endregion





                    detailsView3.BestFitColumns();
                }
            }
            else if (selectedReportName == CNETConstantes.FOREIGN_EXCHANGE_DETAIL)
            {
                GridView view = sender as GridView;
                GridView detailView = view.GetDetailView(e.RowHandle, 0) as GridView;
                if (detailView != null)
                {
                    detailView.OptionsSelection.EnableAppearanceFocusedCell = false;
                    detailView.Columns["Empty"].OptionsColumn.ShowCaption = false;

                    detailView.Columns["Currency"].Width = 60;
                    detailView.Columns["Abbrivation"].Width = 60;
                    detailView.Columns["Amount"].Width = 60;
                    detailView.Columns["Rate"].Width = 60;
                    detailView.Columns["AmountInBirr"].Width = 60;



                    detailView.Columns["AmountInBirr"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    detailView.Columns["AmountInBirr"].SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Sum;
                    detailView.Columns["AmountInBirr"].SummaryItem.DisplayFormat = " {0:N2}";
                }
            }
        }

        private void GvReports_RowStyle(object sender, RowStyleEventArgs e)
        {
            //GridView view = sender as GridView;
            //if (view == null) return;
            //if (selectedReportName == CNETConstantes.UNASSIGNED_RESERVATION)
            //{
            //    var dto = view.GetRow(e.RowHandle) as RegistrationReportInfoDTO;
            //    if (dto == null) return;


            //    e.Appearance.ForeColor = ColorTranslator.FromHtml(dto.Color);

            //}
        }

        private void DetailsView3_CustomSummaryCalculate(object sender, CustomSummaryEventArgs e)
        {
            GridColumnSummaryItem item = e.Item as GridColumnSummaryItem;
            if (item == null || item.FieldName != "ADR") return;
            if (e.IsTotalSummary)
            {
                try
                {
                    GridView view = sender as GridView;
                    List<OccupancySummary> occSummaryList = view.DataSource as List<OccupancySummary>;
                    if (occSummaryList == null || occSummaryList.Count == 0) return;
                    int totalOccupancy = occSummaryList.Sum(o => o.Occupied);
                    decimal totalValue = 0;
                    if (totalOccupancy != 0)
                    {
                        decimal adrTotal = 0;
                        foreach (var occ in occSummaryList)
                        {
                            adrTotal = adrTotal + occ.Occupied * occ.ADR;
                        }

                        totalValue = adrTotal / totalOccupancy;
                    }
                    else
                    {
                        totalValue = 0;
                    }

                    e.TotalValue = totalValue;
                    e.TotalValueReady = true;


                }
                catch (Exception ex)
                {

                }
            }
        }

        private void SearchBySummaryCriteria(object sender, EventArgs e)
        {
            var editor = (sender as SearchLookUpEdit);

            if (editor.EditValue != null && (selectedReportName == CNETConstantes.CASHIER_SUMMARY))
            {
                string editorGsl = editor.EditValue.ToString();
                summaryCriteria = editorGsl;
                if (summaryCriteria == "User")
                {
                    rpgSearchByUser.Enabled = true;
                }
                else if (summaryCriteria == "Summary")
                {
                    rpgSearchByUser.Enabled = false;
                }
            }
            else
            {
                summaryCriteria = null;
                rpgSearchByUser.Enabled = false;
            }
        }

        private void bbnShow_ItemClick(object sender, ItemClickEventArgs e)
        {
            //if (!CheckSecurity(selectedReportName))
            //{
            //    XtraMessageBox.Show("You don't have access to view this report", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);

            //    // Home.ShowModalInfoMessage("You don't have access to view this report", "ERROR");
            //    return;
            //}

            gcReports.DataSource = null;
            gcReports2.DataSource = null;
            gvReports.Columns.Clear();
            gvReports.GroupSummary.Clear();
            gvReports2.Columns.Clear();
            gvReports2.GroupSummary.Clear();
            pdfViewer1.ZoomMode = DevExpress.XtraPdfViewer.PdfZoomMode.FitToWidth;
            pdfViewer1.CloseDocument();
            pdfViewer1.Refresh();

            bool showLiveReport = false;
            if (bbnDate.EditValue != null)
            {
                DateTime selectedDate = Convert.ToDateTime(bbnDate.EditValue);
                bbnDate.EditValue = selectedDate;
                this.Text = selectedReportName;


                ClearGrid();
                //if (string.IsNullOrEmpty(reportArchivePath))
                //{
                //    showLiveReport = true;
                //}
                //else
                //{
                try
                {
                    
                    splitContainer1.Panel1Collapsed = false;
                    splitContainer1.Panel2Collapsed = true;

                    MemoryStream streamfile = null;
                    bool Exist = FTPInterface.FTPAttachment.InitalizeFTPAttachment(LocalBuffer.LocalBuffer.CompanyConsigneeData.Tin);
                    if (Exist)
                    {
                        FTPInterface.FTPAttachment.ORGUnitDefcode = SelectedHotelcode.ToString();
                        streamfile = FTPInterface.FTPAttachment.GetFileStreamFromFTP(selectedReportName, selectedDate);
                    }

                    if (streamfile != null)
                    { 
                        pdfViewer1.LoadDocument(streamfile);
                    }
                    else
                    {
                        showLiveReport = true;
                        //XtraMessageBox.Show(selectedReportName + " is not found in the Archived Reports by "+ selectedDate.Date.ToString("dd-MM-yyyy")+".", "Report Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    
                    if (showLiveReport)
                    {
                        splitContainer1.Panel2Collapsed = false;
                        splitContainer1.Panel1Collapsed = true;
                        Progress_Reporter.Show_Progress(String.Format("Showing {0} . . . ", selectedReportName), "Please Wait. . .");
                        ReportShow(selectedDate, selectedReportName);
                    }

                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show(ex.Message, "Error While Opening Report. . . ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                //}
            }
            Progress_Reporter.Close_Progress();
        }

        private void SearchByGuest(object sender, EventArgs e)
        {
            var editor = (sender as SearchLookUpEdit);

            if (editor.EditValue != null && ((selectedReportName == CNETConstantes.PICK_UP_REPORT) || (selectedReportName == CNETConstantes.DROP_OFF_REPORT)))
            {
                string editorGsl = editor.EditValue.ToString();
                guest = Convert.ToInt32(editorGsl);
            }
            else
            {
                guest = null;
            }
        }

        private void SearchByconsignee(object sender, EventArgs e)
        {
            var editor = (sender as SearchLookUpEdit);

            if (editor.EditValue != null && selectedReportName == CNETConstantes.CHECK_OUT_REPORT)
            {
                string editorGsl = editor.EditValue.ToString();

                company = Convert.ToInt32(editorGsl);
            }
            else
            {
                company = null;
            }
        }

        private void bbnExcel_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                SaveFileDialog sfd = new SaveFileDialog() { Title = "Save", DefaultExt = "xls", Filter = "*.xls|*.xls|*.xlsx|*.xlsx", FileName = "Exported PMS Report File" };
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    gcReports.ExportToXls(sfd.FileName);
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message, "PMS Report");
            }
        }

        private void bbnPDF_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                SaveFileDialog sfd = new SaveFileDialog() { Title = "Save", DefaultExt = "xls", Filter = "*.pdf|*.pdf", FileName = "Exported PMS Report File" };
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    gcReports.ExportToPdf(sfd.FileName);
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message, "PMS Report");
            }
        }

        private void bbnCSV_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                SaveFileDialog sfd = new SaveFileDialog()
                {
                    Title = "Save",
                    DefaultExt = "xls",
                    Filter = "|*.csv",
                    FileName = "Exported PMS Report File"
                };
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    gcReports.ExportToCsv(sfd.FileName);
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message, "PMS Report");
            }
        }

        private void bbnPrint_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (splitContainer1.Panel2Collapsed)
            {
                pdfViewer1.ShowPrintStatusDialog = true;
                pdfViewer1.Print();
            }
            else
            {


                DateTime? selectedDate = null;
                DateTime? selectedDateEnd = null;

                if (bbnDate.EditValue != null)
                {
                    selectedDate = (DateTime)bbnDate.EditValue;
                }
                else
                {
                    if (BBDateSearch.Visibility == BarItemVisibility.Always)
                    {
                        XtraMessageBox.Show("Please select the start date first.", "Date Range Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        XtraMessageBox.Show("Please select the date first.", "Date Range Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    return;
                }

                if (selectedReportName == CNETConstantes.DAILY_RESIDENNT_SUMMARY)
                {
                    //  SaveAllDailyResidentSummary(selectedDate.Value);
                }

                if (BBDateSearch.Visibility == BarItemVisibility.Always)
                {
                    if (issueDateEnd != null)
                    {
                        if (BBDateSearch.EditValue == "Daily")
                        {
                            selectedDateEnd = null;
                        }
                        else
                        {
                            selectedDateEnd = (DateTime)issueDateEnd;
                        }
                    }
                    else
                    {
                        XtraMessageBox.Show("Please select the end date first.", "Date Range Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                }

                XtraReport1 report = new XtraReport1();

                if (splitContainer2.Panel2Collapsed == false)
                {
                   // report = new XtraReport1(gcReports, gcReports2, selectedReportName, selectedDate.Value.Date.ToShortDateString(), SelectedHotelDescription, true, selectedDateEnd);

                   report = new XtraReport1(gcReports, gcReports2, selectedReportName, selectedDate.Value.Date.ToShortDateString(), SelectedHotelDescription, printByPortrait, selectedDateEnd);
                }
                else
                {
                   // report = new XtraReport1(gcReports, null, selectedReportName, selectedDate.Value.Date.ToShortDateString(), SelectedHotelDescription, true, selectedDateEnd);

                    report = new XtraReport1(gcReports, null, selectedReportName, selectedDate.Value.Date.ToShortDateString(), SelectedHotelDescription, printByPortrait, selectedDateEnd);
                }
                report.Landscape = !printByPortrait;
                ReportPrintTool pt = new ReportPrintTool(report);
                pt.ShowPreview();
            }
        }

        private void bbnSave_ItemClick(object sender, ItemClickEventArgs e)
        {
            var row = gvReports.GetRow(gvReports.FocusedRowHandle);
            if (row == null) return;




        }

        private void bbnSaveAll_ItemClick(object sender, ItemClickEventArgs e)
        {
            DateTime? selectedDate = null;
            DateTime? selectedDateEnd = null;

            if (bbnDate.EditValue != null)
            {
                selectedDate = (DateTime)bbnDate.EditValue;
            }
            else
            {
                if (BBDateSearch.Visibility == BarItemVisibility.Always)
                {
                    XtraMessageBox.Show("Please select the start date first.", "Date Range Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    XtraMessageBox.Show("Please select the date first.", "Date Range Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return;
            }
            if (selectedReportName == CNETConstantes.DAILY_RESIDENNT_SUMMARY)
            {
                // SaveAllDailyResidentSummary(selectedDate.Value);
            }
            else if (selectedReportName == CNETConstantes.DAILY_BUSINESS_REPORT)
            {
            }

        }

        private void bbnDate_ItemClick(object sender, ItemClickEventArgs e)
        {

        }

        private void gcReports_MouseHover(object sender, EventArgs e)
        {
            //gvReports.Focus();
        }

        private void bbiMultiExport_ItemClick(object sender, ItemClickEventArgs e)
        {

        }

        private void frmReports_Load(object sender, EventArgs e)
        {
            if (!InitializeData())
            {
                splitContainer1.Panel2Collapsed = false;
                splitContainer1.Panel1Collapsed = true;
                rbnCtrlReports.Enabled = false;
                gcReports.Enabled = false;
                gcReports2.Enabled = false;
                ReportTreeLst.Enabled = false;
            }
        }



        private void BBDateSearch_EditValueChanged(object sender, EventArgs e)
        {
            if (BBDateSearch.EditValue != null)
            {
                bbnDate.Reset();
                bbnDateEnd.Reset();
                bbnDateEnd.EditValue = null;
                bbnDate.EditValue = null;
                bbnDateEnd.Visibility = BarItemVisibility.Always;
                bbnDate.Visibility = BarItemVisibility.Always;

                if (BBDateSearch.EditValue.ToString() == "Daily")
                {
                    bbnDate.Reset();
                    bbnDateEnd.Visibility = BarItemVisibility.Never;
                    bbnDate.EditValue = null;
                    bbnDate.Enabled = true;
                    bbnDate.Caption = "Current Date";
                    //bbnDateEnd.Enabled = true;
                    //bbnDateEnd.EditValue = currentDate;
                    //bbnDateEnd.Enabled = false;
                    bbnDate.EditValue = currentDate;
                    bbnDate.Enabled = false;
                    DateTime date = Convert.ToDateTime(bbnDate.EditValue.ToString());
                    issuedDate = date;
                    issueDateEnd = date;
                    //bbnDateEnd.EditValue = currentDate;
                }
                else if (BBDateSearch.EditValue.ToString() == "Weekly")
                {
                    DateTime now = currentDate;
                    var weekStart = DayOfWeek.Monday;
                    DateTime firstdayOfTheWeek = now.AddDays(weekStart - now.DayOfWeek);

                    bbnDate.Reset();
                    bbnDate.EditValue = null;
                    bbnDate.Enabled = true;
                    bbnDate.Caption = "From";
                    bbnDate.EditValue = firstdayOfTheWeek;
                    issuedDate = Convert.ToDateTime(bbnDate.EditValue.ToString());
                    bbnDate.Enabled = false;
                    bbnDateEnd.EditValue = currentDate;
                    bbnDateEnd.Enabled = false;
                    SearchByDateEnd();
                }
                else if (BBDateSearch.EditValue.ToString() == "Monthly")
                {
                    bbnDate.Reset();
                    bbnDate.EditValue = null;
                    bbnDate.Reset();
                    bbnDate.EditValue = null;

                    bbnDate.Enabled = true;
                    bbnDate.Caption = "From";
                    DateTime now = currentDate;
                    var startDate = new DateTime(now.Year, now.Month, 1);
                    bbnDate.EditValue = startDate;
                    issuedDate = Convert.ToDateTime(bbnDate.EditValue.ToString());
                    bbnDate.Enabled = false;
                    bbnDateEnd.EditValue = currentDate;
                    bbnDateEnd.Enabled = false;
                    SearchByDateEnd();
                }
                else if (BBDateSearch.EditValue.ToString() == "Annually")
                {
                    bbnDate.Reset();
                    bbnDate.EditValue = null;
                    bbnDate.Reset();
                    bbnDate.EditValue = null;

                    bbnDate.Enabled = true;
                    bbnDate.Caption = "From";
                    DateTime now = currentDate;
                    var startDate = new DateTime(now.Year, 1, 1);
                    bbnDate.EditValue = startDate;
                    issuedDate = Convert.ToDateTime(bbnDate.EditValue.ToString());
                    bbnDate.Enabled = false;
                    bbnDateEnd.EditValue = currentDate;
                    bbnDateEnd.Enabled = false;
                    SearchByDateEnd();
                }
                else if (BBDateSearch.EditValue.ToString() == "At the day of")
                {
                    bbnDate.Reset();
                    bbnDateEnd.Visibility = BarItemVisibility.Never;
                    bbnDate.EditValue = null;
                    bbnDate.Enabled = true;
                    bbnDateEnd.Enabled = false;
                    bbnDate.Caption = "At the day of";
                    bbnDate.EditValueChanged += searchByAtTheDayOf;

                }
                else if (BBDateSearch.EditValue.ToString() == "Date Range")
                {
                    bbnDate.Reset();
                    bbnDateEnd.Reset();
                    bbnDate.EditValue = null;
                    bbnDateEnd.EditValue = null;
                    bbnDate.Enabled = true;
                    bbnDateEnd.Enabled = true;
                    bbnDate.Caption = "From";
                    bbnDate.EditValueChanged += FromDateSearch;
                    bbnDateEnd.EditValueChanged += SearchByRange;

                }
                else if (BBDateSearch.EditValue.ToString() == "Show All")
                {
                    bbnDate.Reset();
                    bbnDate.EditValue = null;
                    bbnDate.Reset();
                    bbnDate.EditValue = null;

                    bbnDate.Enabled = true;
                    bbnDate.Caption = "From";
                    DateTime now = currentDate;
                    var startDate = new DateTime(2000, 01, 01);
                    bbnDate.EditValue = startDate;
                    issuedDate = Convert.ToDateTime(bbnDate.EditValue.ToString());
                    bbnDate.Enabled = false;
                    bbnDateEnd.EditValue = currentDate;
                    bbnDateEnd.Enabled = false;
                    SearchByDateEnd();

                }
            }

        }

        private void searchByAtTheDayOf(object sender, EventArgs e)
        {
            if (bbnDate.EditValue != null && (BBDateSearch.EditValue != null && BBDateSearch.EditValue.ToString() == "At the day of"))
            {
                DateTime date = Convert.ToDateTime(bbnDate.EditValue);
                issuedDate = date;
                issueDateEnd = date;
            }
            else if (bbnDate.EditValue == null && BBDateSearch.EditValue != null && BBDateSearch.EditValue.ToString() == "At the day of")
            {
                issuedDate = null;
                issueDateEnd = null;
            }
        }

        private void SearchByRange(object sender, EventArgs e)
        {
            if (bbnDateEnd.EditValue != null)
            {
                DateTime date = Convert.ToDateTime(bbnDateEnd.EditValue.ToString());
                if (bbnDate.EditValue != null && (date >= Convert.ToDateTime(bbnDate.EditValue)))
                {
                    issueDateEnd = date;
                }
                else
                {
                    if (bbnDate.EditValue == null)
                    {
                        XtraMessageBox.Show("Please enter the start date first.",
                            "Date Range Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        //bbnDateEnd.Reset();
                        //bbnDateEnd.EditValue = null;
                    }
                    else if (date < Convert.ToDateTime(bbnDate.EditValue))
                    {
                        XtraMessageBox.Show("Please enter a valid date range.\nAn end date should not be less than the starting date.",
                            "Date Range Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        //bbnDateEnd.Reset();
                        //bbnDate.EditValue = null;
                        //bbnDateEnd.EditValue = null;
                    }
                }
            }
            else
            {
                issueDateEnd = null;
            }
        }

        private void FromDateSearch(object sender, EventArgs e)
        {
            if (bbnDate.EditValue != null)
            {
                if (bbnDateEnd.EditValue != null)
                {
                    DateTime date = Convert.ToDateTime(bbnDate.EditValue);
                    if (date.Date <= Convert.ToDateTime(bbnDateEnd.EditValue).Date)
                    {
                        issuedDate = date;
                        issueDateEnd = Convert.ToDateTime(bbnDateEnd.EditValue);
                    }
                    else
                    {
                        XtraMessageBox.Show("Please enter a valid date range.\nThe starting date should not be greater than the end date.",
                            "Date Range Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        bbnDateEnd.Reset();
                        bbnDate.EditValue = null;
                    }
                }
                else
                {
                    DateTime date = Convert.ToDateTime(bbnDate.EditValue);
                    issuedDate = date;
                }
            }
            else
            {
                issuedDate = null;
            }
        }

        private void ReportTreeLst_FocusedNodeChanged(object sender, FocusedNodeChangedEventArgs e)
        {
            if (e.Node != null && e.Node.Selected)
            {
                issuedDate = null;
                issueDateEnd = null;

                bbnDate.Enabled = true;
                bbnDate.Caption = "At the day of ";

                BBDateSearch.Visibility = BarItemVisibility.Never;
                bbnDateEnd.Visibility = BarItemVisibility.Never;
                BBDateSearch.EditValue = null;

                gcReports.DataSource = null;
                gcReports2.DataSource = null;
                gvReports.Columns.Clear();
                gvReports.GroupSummary.Clear();
                gvReports2.Columns.Clear();
                gvReports2.GroupSummary.Clear();
                user = null;
                guest = null;
                rpgSave.Visible = false;
                printByPortrait = false;
                pdfViewer1.ZoomMode = DevExpress.XtraPdfViewer.PdfZoomMode.FitToWidth;
                pdfViewer1.CloseDocument();
                pdfViewer1.Refresh();

                var row = (ReportTreeDTO)ReportTreeLst.GetDataRecordByNode(e.Node);

                //selectedReportName = row.reportCode;
                selectedReportName = row.reportDesc;

                if (e.Node.HasChildren)
                {
                    bbnDate.EditValue = null;
                    selectedReportName = null;
                    selectedReportName = "PMS Report";
                }
                else
                {
                    bbnDate.EditValue = currentDate;

                }

                if (selectedReportName == CNETConstantes.DAILY_RESIDENNT_SUMMARY)
                {
                    rpItmDate.MaxValue = rpItmDate.MaxValue.AddYears(1);
                }

                if (selectedReportName == CNETConstantes.CASHIER_SUMMARY)
                {
                    rpgSearchByUser.Visible = true;
                    rpgSearchByUser.Enabled = true;
                    splitContainer2.Panel2Collapsed = false;

                    InitializeUserName();
                    IntiallizeSummaryCriteria();
                }
                else if (selectedReportName == CNETConstantes.CASH_DROPPED_REPORT)
                {
                    rpgSearchByUser.Visible = true;
                    rpgSearchByUser.Enabled = true;

                    BBDateSearch.Visibility = BarItemVisibility.Always;
                    bbnDateEnd.Visibility = BarItemVisibility.Always;
                    BBDateSearch.EditValue = "Daily";

                    InitializeUserName();
                }
                else if (selectedReportName == CNETConstantes.UNASSIGNED_RESERVATION)
                {
                    BBDateSearch.Visibility = BarItemVisibility.Always;
                    bbnDateEnd.Visibility = BarItemVisibility.Always;
                    BBDateSearch.EditValue = "Daily";

                }
                else if (selectedReportName == CNETConstantes.BILL_TRANSFER_REPORT)
                {
                    BBDateSearch.Visibility = BarItemVisibility.Always;
                    bbnDateEnd.Visibility = BarItemVisibility.Always;
                    BBDateSearch.EditValue = "Daily";

                }
                else if (selectedReportName == CNETConstantes.NATIONALITY_REPORT)
                {
                    BBDateSearch.Visibility = BarItemVisibility.Always;
                    bbnDateEnd.Visibility = BarItemVisibility.Always;
                    BBDateSearch.EditValue = "Daily";

                }
                else if (selectedReportName == CNETConstantes.ROOM_POS_CHARGES)
                {
                    BBDateSearch.Visibility = BarItemVisibility.Always;
                    bbnDateEnd.Visibility = BarItemVisibility.Always;
                    BBDateSearch.EditValue = "Daily";

                }
                else if (selectedReportName == CNETConstantes.CHECK_OUT_REPORT)
                {
                    rpgSearchByUser.Visible = true;
                    rpgSearchByUser.Enabled = true;
                    splitContainer2.Panel2Collapsed = true;
                    BBDateSearch.Visibility = BarItemVisibility.Always;
                    bbnDateEnd.Visibility = BarItemVisibility.Always;
                    BBDateSearch.EditValue = "Daily";
                    InitializeCompanyconsignee();
                }
                else if (selectedReportName == CNETConstantes.PICK_UP_REPORT || selectedReportName == CNETConstantes.DROP_OFF_REPORT)
                {
                    rpgSearchByUser.Visible = true;
                    rpgSearchByUser.Enabled = true;
                    splitContainer2.Panel2Collapsed = true;
                    if (bbnDate.EditValue != null && bbnDate.EditValue.ToString() != "")
                    {
                        InitializeGuestName(Convert.ToDateTime(bbnDate.EditValue), selectedReportName);
                    }
                }
                else
                {
                    if (selectedReportName == CNETConstantes.CASH_RECEIPT_REPORT)
                    {
                        BBDateSearch.Visibility = BarItemVisibility.Always;
                        bbnDateEnd.Visibility = BarItemVisibility.Always;
                        BBDateSearch.EditValue = "Daily";
                    }
                    else if (selectedReportName == CNETConstantes.CREDIT_SALES_REPORT)
                    {
                        BBDateSearch.Visibility = BarItemVisibility.Always;
                        bbnDateEnd.Visibility = BarItemVisibility.Always;
                        BBDateSearch.EditValue = "Daily";
                    }
                    else if (selectedReportName == CNETConstantes.CASH_SALES_REPORT)
                    {
                        BBDateSearch.Visibility = BarItemVisibility.Always;
                        bbnDateEnd.Visibility = BarItemVisibility.Always;
                        BBDateSearch.EditValue = "Daily";
                    }
                    else if (selectedReportName == CNETConstantes.CHECK_OUT_REPORT)
                    {
                        /* rpgSearchByUser.Visible = true;
                         rpgSearchByUser.Enabled = true;
                         splitContainer2.Panel2Collapsed = true;
                         BBDateSearch.Visibility = BarItemVisibility.Always;
                         bbnDateEnd.Visibility = BarItemVisibility.Always;
                         BBDateSearch.EditValue = "Daily";
                         Initializeconsignee();*/
                    }
                    else if (selectedReportName == CNETConstantes.REBATE_REPORT)
                    {
                        BBDateSearch.Visibility = BarItemVisibility.Always;
                        bbnDateEnd.Visibility = BarItemVisibility.Always;
                        BBDateSearch.EditValue = "Daily";
                    }
                    else if (selectedReportName == CNETConstantes.DEBIT_NOTE_REPORT)
                    {
                        BBDateSearch.Visibility = BarItemVisibility.Always;
                        bbnDateEnd.Visibility = BarItemVisibility.Always;
                        BBDateSearch.EditValue = "Daily";
                    }
                    else if (selectedReportName == CNETConstantes.PAID_OUT_REPORT)
                    {
                        BBDateSearch.Visibility = BarItemVisibility.Always;
                        bbnDateEnd.Visibility = BarItemVisibility.Always;
                        BBDateSearch.EditValue = "Daily";
                    }
                    else if (selectedReportName == CNETConstantes.REFUND_REPORT)
                    {
                        BBDateSearch.Visibility = BarItemVisibility.Always;
                        bbnDateEnd.Visibility = BarItemVisibility.Always;
                        BBDateSearch.EditValue = "Daily";
                    }
                    else if (selectedReportName == CNETConstantes.DAILY_ROOM_CHARGE_REPORT)
                    {
                        BBDateSearch.Visibility = BarItemVisibility.Always;
                        bbnDateEnd.Visibility = BarItemVisibility.Always;
                        BBDateSearch.EditValue = "Daily";
                    }
                    else if (selectedReportName == CNETConstantes.CITY_LEDGER)
                    {
                        BBDateSearch.Visibility = BarItemVisibility.Always;
                        bbnDateEnd.Visibility = BarItemVisibility.Always;
                        BBDateSearch.EditValue = "Daily";
                    }
                    else if (selectedReportName == CNETConstantes.ROOM_INCOME_REPORT)
                    {
                        BBDateSearch.Visibility = BarItemVisibility.Always;
                        bbnDateEnd.Visibility = BarItemVisibility.Always;
                        BBDateSearch.EditValue = "Daily";
                    }
                    else if (selectedReportName == CNETConstantes.SUMMARY_OF_SUMMARY)
                    {
                        BBDateSearch.Visibility = BarItemVisibility.Always;
                        bbnDateEnd.Visibility = BarItemVisibility.Always;
                        BBDateSearch.EditValue = "Daily";
                    }
                    else if (selectedReportName == CNETConstantes.DAILY_SALES_SUMMARY)
                    {
                        BBDateSearch.Visibility = BarItemVisibility.Always;
                        bbnDateEnd.Visibility = BarItemVisibility.Always;
                        BBDateSearch.EditValue = "Daily";
                    }
                    else if (selectedReportName == CNETConstantes.PMS_Fiscal_Reconciliation)
                    {
                        rpgSearchByUser.Visible = true;
                        rpgSearchByUser.Enabled = true;
                        BBDateSearch.Visibility = BarItemVisibility.Always;
                        bbnDateEnd.Visibility = BarItemVisibility.Always;
                        BBDateSearch.EditValue = "Daily";
                    }

                    rpgSearchByUser.Visible = false;
                    rpgSearchByUser.Enabled = false;
                    splitContainer2.Panel2Collapsed = true;
                }


                this.Text = selectedReportName;

                //bbnShow.Enabled = SecurityCheck(selectedReportName);

            }
        }

        private void ReportTreeLst_CustomDrawNodeCell(object sender, CustomDrawNodeCellEventArgs e)
        {
            TreeList tree = sender as TreeList;
            if (e.Node != null)
            {
                if (e.Node == tree.FocusedNode)
                {
                    e.Graphics.FillRectangle(SystemBrushes.Window, e.Bounds);

                    Rectangle rect = new Rectangle(e.EditViewInfo.ContentRect.Left,
                        e.EditViewInfo.ContentRect.Top, Convert.ToInt32(e.Graphics.MeasureString(e.CellText, ReportTreeLst.Font).Width + 1),
                        Convert.ToInt32(e.Graphics.MeasureString(e.CellText, ReportTreeLst.Font).Height));


                    e.Graphics.FillRectangle(SystemBrushes.Highlight, rect);
                    e.Graphics.DrawString(e.CellText, ReportTreeLst.Font, SystemBrushes.HighlightText, rect);
                    e.Handled = true;
                }
            }
        }

        #endregion

        #region PMS Report Methods

        private void ShowForeignExReport(DateTime date)
        {
            printByPortrait = true;
            List<ForeignExchangeReport> forexReportList = UIProcessManager.GetForeignExchangeDetailReport(date, CNETConstantes.LU_ACTIVITY_DEFINATION_PREPARED, SelectedHotelcode);

            if (forexReportList == null)
                return;
            gcReports.DataSource = forexReportList;
            gcReports.RefreshDataSource();
            gvReports.RefreshData();

            gvReports.OptionsDetail.ShowDetailTabs = false;


            gvReports.Columns["GrandTotal"].SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Sum;
            gvReports.Columns["GrandTotal"].AppearanceCell.TextOptions.HAlignment = HorzAlignment.Far;
            gvReports.Columns["GrandTotal"].SummaryItem.DisplayFormat = "Total = {0:N2}";

            for (int i = 0; i < forexReportList.Count; i++)
                gvReports.ExpandMasterRow(i);

        }

        private void ShowForeignExSummaryReport(DateTime date)
        {
            DataTable dataTable = new DataTable();
            printByPortrait = true;
            List<ForignExchangeDTO> forignExchanges = UIProcessManager.GetForeignExchangeSummaryReport(date, CNETConstantes.LU_ACTIVITY_DEFINATION_PREPARED, SelectedHotelcode);
            if (forignExchanges != null && forignExchanges.Count > 0)
            {
                var users = forignExchanges.GroupBy(t => t.userCode).Select(t => t.First());
                List<ForexSummary> forexSummaryList = new List<ForexSummary>();

                dataTable.Columns.Add(new DataColumn() { Caption = "User", ColumnName = "user" });

                foreach (var user in users)
                {
                    var summary = forignExchanges.Where(t => t.userCode == user.userCode).GroupBy(t => t.currency).Select(t => new
                    {
                        Total = t.Sum(s => s.amount),
                        CurrName = string.Format("{0} ({1})", t.First().currencyDesc, t.First().sign),
                        CurrCode = t.First().currency
                    }).ToList();


                    List<Tuple<string, decimal>> tranList = new List<Tuple<string, decimal>>();
                    if (summary != null && summary.Count > 0)
                    {
                        foreach (var s in summary)
                        {
                            if (!dataTable.Columns.Contains(s.CurrCode.ToString()))
                            {
                                dataTable.Columns.Add(new DataColumn() { Caption = s.CurrName, ColumnName = s.CurrCode.ToString() });

                            }
                            tranList.Add(new Tuple<string, decimal>(s.CurrCode.ToString(), Math.Round(s.Total == null ? 0 : s.Total.Value, 2)));
                        }
                    }



                    DataRow row = dataTable.NewRow();
                    row[dataTable.Columns[0]] = user.userName;
                    foreach (var tran in tranList)
                    {
                        row[dataTable.Columns[tran.Item1]] = tran.Item2;
                    }
                    dataTable.Rows.Add(row);

                    forexSummaryList.Add(new ForexSummary() { User = user.userName, Transactions = tranList });
                }

            }
            gcReports.DataSource = dataTable;
            gcReports.RefreshDataSource();
            gvReports.RefreshData();


            int colCount = gvReports.Columns.Count;
            for (int i = 1; i < colCount; i++)
            {
                gvReports.Columns[i].SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Sum;
                gvReports.Columns[i].AppearanceCell.TextOptions.HAlignment = HorzAlignment.Far;
                gvReports.Columns[i].SummaryItem.DisplayFormat = "Total = {0:N2}";
            }

        }

        private void ShowSummaryOfSummaryReport(DateTime startDate, DateTime endDate)
        {
            List<ResidentSummaryOfSummaryReportDTO> summaryOfSummaryList = UIProcessManager.GetDailyResidentSummaryofSummary(startDate, endDate, SelectedHotelcode);

            if (summaryOfSummaryList == null)
                return;
            gcReports.DataSource = summaryOfSummaryList;
            gcReports.RefreshDataSource();
            gvReports.BestFitColumns();

            item = new GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum,
                        "RoomRevenue", " {0:N}");
            gvReports.Columns["RoomRevenue"].Summary.Add(item);

            item = new GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum,
                                    "Package", " {0:N}");
            gvReports.Columns["Package"].Summary.Add(item);

            item = new GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum,
                                    "ServiceCharge", " {0:N}");
            gvReports.Columns["ServiceCharge"].Summary.Add(item);

            item = new GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum,
                                    "Vat", " {0:N}");
            gvReports.Columns["Vat"].Summary.Add(item);

            item = new GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum,
                                    "RoomTotal", " {0:N}");
            gvReports.Columns["RoomTotal"].Summary.Add(item);

            item = new GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum,
                                    "POSCharge", " {0:N}");
            gvReports.Columns["POSCharge"].Summary.Add(item);

            item = new GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum,
                                    "TodayTotal", " {0:N}");
            gvReports.Columns["TodayTotal"].Summary.Add(item);

            item = new GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum,
                                    "BBF", " {0:N}");
            gvReports.Columns["BBF"].Summary.Add(item);

            item = new GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum,
                                    "toDateTotal", " {0:N}");
            gvReports.Columns["toDateTotal"].Summary.Add(item);

            item = new GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum,
                                    "Discount", " {0:N}");
            gvReports.Columns["Discount"].Summary.Add(item);

            item = new GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum,
                                                "Payment", " {0:N}");
            gvReports.Columns["Payment"].Summary.Add(item);

            item = new GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum,
                                                "Paidout", " {0:N}");
            gvReports.Columns["Paidout"].Summary.Add(item);

            item = new GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum,
                                                "BCF", " {0:N}");
            gvReports.Columns["BCF"].Summary.Add(item);

            item = new GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum,
                                                "Outstanding", " {0:N}");
            gvReports.Columns["Outstanding"].Summary.Add(item);
        }

        private void ShowRoomPOSCharges(DateTime startDate, DateTime endDate)
        {
            List<RoomPOSChargesReportDTO> roomPOSCharges = UIProcessManager.GetRoomPOSCharges(startDate, endDate, SelectedHotelcode);
            if (roomPOSCharges == null)
                return;
            gcReports.DataSource = roomPOSCharges;
            gcReports.RefreshDataSource();
            gvReports.BestFitColumns();

            gvReports.Columns["VoucherID"].Width = 80;
            gvReports.Columns["RegNo"].Width = 80;

            item = new GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum,
                        "Amount", " {0:N}");
            gvReports.Columns["Amount"].Summary.Add(item);
        }

        private void ShowCRVTransactionReports(DateTime startDate, DateTime endDate, int voucherDefn)
        {
            List<RegistrationNonLinitemVoucherDTO> nonLineItemTransactionReports = UIProcessManager.GetRegistrationNonLinitemVoucher(startDate, endDate, false, SelectedHotelcode, new List<int>() { voucherDefn }, null, null);

            if (nonLineItemTransactionReports == null)
                return;
            Progress_Reporter.Show_Progress("Binding " + selectedReportName + ". . . ", "Please Wait ........");

            gcReports.DataSource = nonLineItemTransactionReports;
            gcReports.RefreshDataSource();
            gvReports.BestFitColumns();

            gvReports.Columns["VoucherCode"].Width = 80;
            gvReports.Columns["RegNo"].Width = 80;

            item = new GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum,
                        "GrandTotal", " {0:N}");
            gvReports.Columns["GrandTotal"].Summary.Add(item);

        }

        private void ShowNonlineitemTransactionReports(DateTime startDate, DateTime endDate, int voucherDefn)
        {
            List<RegistrationNonLinitemVoucherDTO> nonLineItemTransactionReports = UIProcessManager.GetRegistrationNonLinitemVoucher(startDate, endDate, false, SelectedHotelcode, new List<int>() { voucherDefn }, null, null);

            if (nonLineItemTransactionReports == null)
                return;
            Progress_Reporter.Show_Progress("Binding " + selectedReportName + ". . . ", "Please Wait ........");

            gcReports.DataSource = nonLineItemTransactionReports;
            gcReports.RefreshDataSource();
            gvReports.BestFitColumns();

            gvReports.Columns["VoucherCode"].Width = 80;
            gvReports.Columns["RegNo"].Width = 80;

            item = new GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum,
                        "GrandTotal", " {0:N}");
            gvReports.Columns["GrandTotal"].Summary.Add(item);


        }

        private void ShowLineitemTransactionReports(DateTime startDate, DateTime endDate, List<int> voucherDefn, bool isCheckout, int? Consignee, int? company)
        {
            Progress_Reporter.Show_Progress("Binding " + selectedReportName + ". . . ", "Please Wait ........");

            List<RegistrationVoucherDTO> registrationVouchers = UIProcessManager.GetRegistrationVoucher(startDate, endDate, isCheckout, SelectedHotelcode, voucherDefn, Consignee, company);
            Progress_Reporter.Close_Progress();
            if (registrationVouchers == null)
                return;
            gcReports.DataSource = registrationVouchers;
            gcReports.RefreshDataSource();
            gvReports.BestFitColumns();

            gvReports.Columns["VoucherCode"].Width = 80;
            gvReports.Columns["RegNo"].Width = 80;
            item = new GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum,
                        "SubTotal", " {0:N}");
            gvReports.Columns["SubTotal"].Summary.Add(item);

            item = new GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum,
                        "Discount", " {0:N}");
            gvReports.Columns["Discount"].Summary.Add(item);

            item = new GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum,
                        "ServiceCharge", " {0:N}");
            gvReports.Columns["ServiceCharge"].Summary.Add(item);

            item = new GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum,
                        "VAT", " {0:N}");
            gvReports.Columns["VAT"].Summary.Add(item);

            item = new GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum,
                        "GrandTotal", " {0:N}");
            gvReports.Columns["GrandTotal"].Summary.Add(item);

            gvReports.Columns["SN"].VisibleIndex = 0;
            gvReports.Columns["VoucherCode"].VisibleIndex = 1;
            gvReports.Columns["Date"].VisibleIndex = 2;
            gvReports.Columns["RegNo"].VisibleIndex = 3;
            gvReports.Columns["Room"].VisibleIndex = 4;
            gvReports.Columns["NoOfNight"].VisibleIndex = 5;
            gvReports.Columns["Customer"].VisibleIndex = 6;
            gvReports.Columns["Company"].VisibleIndex = 7;
            gvReports.Columns["User"].VisibleIndex = 8;
            gvReports.Columns["PaymentType"].VisibleIndex = 9;
            gvReports.Columns["SubTotal"].VisibleIndex = 10;
            gvReports.Columns["Discount"].VisibleIndex = 11;
            gvReports.Columns["ServiceCharge"].VisibleIndex = 12;
            gvReports.Columns["VAT"].VisibleIndex = 13;
            gvReports.Columns["GrandTotal"].VisibleIndex = 14;

        }

        private void ShowPMSFiscalReconciliationReport(DateTime startDate, DateTime endDate)
        {
            List<FiscalReconciliationReportDTO> FiscalReconciliationReportList = UIProcessManager.GetPMSFiscalReconciliationReport(startDate, endDate, SelectedHotelcode);

            if (FiscalReconciliationReportList == null)
                return;
            gcReports.DataSource = FiscalReconciliationReportList;
            gcReports.RefreshDataSource();
            gvReports.ExpandMasterRow(0);
            gvReports.ExpandMasterRow(1);
            gvReports.ExpandMasterRow(2);
            gvReports.ExpandMasterRow(3);
            gvReports.ExpandMasterRow(4);

        }

        //private void GetDifferncebetweenCheckoutandroomcharge(DateTime startDate, DateTime endDate)
        //{

        //    //printByPortrait = true;
        //    List<vw_VoucherHeaderWithRoomNo> Error = new List<vw_VoucherHeaderWithRoomNo>();
        //    List<vw_VoucherHeaderWithRoomNo> CheckOutlineItemVouchers = PMSUIProcessManager.GetVoucherHeaderWithRoomNoByDate(startDate, endDate, 108, true, null, null);


        //    List<vw_VoucherHeaderWithRoomNo> DailyRoomChargelineItemVouchers = PMSUIProcessManager.GetVoucherHeaderWithRoomNoByDate(startDate, endDate, 158, false, null, null);

        //    List<CheckoutReportColumn> checkoutReportList = new List<CheckoutReportColumn>();
        //    int count = 0;

        //    List<string> RegNumberList = CheckOutlineItemVouchers.Select(x => x.RegNumber).Distinct().ToList(); ;

        //    foreach (string Reg in RegNumberList)
        //    {
        //        List<vw_VoucherHeaderWithRoomNo> CheckOutlineItem = CheckOutlineItemVouchers.Where(x => x.RegNumber == Reg).ToList();
        //        List<vw_VoucherHeaderWithRoomNo> RoomChargefortheReg = DailyRoomChargelineItemVouchers.Where(x => x.RegNumber == Reg).ToList();
        //        decimal CheckOutgrandTotal = CheckOutlineItem.Sum(x => x.grandTotal);
        //        decimal RoomChargegrandTotal = RoomChargefortheReg.Sum(x => x.grandTotal);
        //        decimal Differnce = Math.Abs(CheckOutgrandTotal - RoomChargegrandTotal);

        //        if (Differnce > 1 || RoomChargefortheReg.Count == 0)
        //        {
        //            foreach (vw_VoucherHeaderWithRoomNo vo in CheckOutlineItem)
        //            {
        //                count++;
        //                decimal discount = vo.discount == null ? 0 : vo.discount;
        //                decimal additionalCharge = vo.additionalCharge == null ? 0 : vo.additionalCharge;
        //                decimal taxAmount = vo.taxAmount == null ? 0 : vo.taxAmount;
        //                string RegNumb = vo.RegNumber;
        //                CheckoutReportColumn checkoutReport = new CheckoutReportColumn()
        //                {
        //                    SN = count,
        //                    VoucherID = vo.code,
        //                    Customer = vo.name,
        //                    RegNo = RegNumb,
        //                    Room = vo.RoomNumber,
        //                    Date = vo.IssuedDate,
        //                    Discount = Math.Round(discount, 2),
        //                    GrandTotal = Math.Round(vo.grandTotal, 2),
        //                    ServiceCharge = Math.Round(additionalCharge, 2),
        //                    SubTotal = vo.subTotal == null ? 0 : Math.Round(vo.subTotal.Value, 2),
        //                    VAT = Math.Round(taxAmount, 2),
        //                    User = vo.userName,
        //                    PaymentType = vo.PaymentMethod

        //                };

        //                checkoutReportList.Add(checkoutReport);
        //            }
        //        }

        //    }



        //    gcReports.DataSource = checkoutReportList;
        //    gcReports.RefreshDataSource();
        //    gvReports.BestFitColumns();

        //    gvReports.Columns["SN"].Width = 25;
        //    gvReports.Columns["SN"].AppearanceCell.TextOptions.HAlignment = HorzAlignment.Near;
        //    gvReports.Columns["VoucherID"].Width = 80;
        //    gvReports.Columns["RegNo"].Width = 80;
        //    item = new GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum,
        //                "SubTotal", " {0:N}");
        //    gvReports.Columns["SubTotal"].Summary.Add(item);

        //    item = new GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum,
        //                "Discount", " {0:N}");
        //    gvReports.Columns["Discount"].Summary.Add(item);

        //    item = new GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum,
        //                "ServiceCharge", " {0:N}");
        //    gvReports.Columns["ServiceCharge"].Summary.Add(item);

        //    item = new GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum,
        //                "VAT", " {0:N}");
        //    gvReports.Columns["VAT"].Summary.Add(item);

        //    item = new GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum,
        //                "GrandTotal", " {0:N}");
        //    gvReports.Columns["GrandTotal"].Summary.Add(item);

        //}

        //private void GetRegistrationsWhichhasnoDailyRoomCharge()
        //{
        //    DateTime startDate = UIProcessManager.GetDataTime().Timestamp.Date;
        //    DateTime endDate = UIProcessManager.GetDataTime().Timestamp.Date.AddDays(1);
        //    List<RegistrationDocumentDTO> RegistrationWithNoDailyRoomCharge = new List<RegistrationDocumentDTO>();
        //    List<CheckoutReportColumn> checkoutReportList = new List<CheckoutReportColumn>();
        //    int count = 0;

        //    List<RegistrationDocumentDTO> _regDocList = RegistrationDataChange(PMSUIProcessManager.GetRegistrationDocList(CNETConstantes.CHECKED_IN_STATE, null, null, SelectedHotelcode));
        //    List<vw_VoucherHeaderWithRoomNo> DailyRoomChargelineItemVouchers = PMSUIProcessManager.GetVoucherHeaderWithRoomNoByDate(startDate, endDate, 158, false, null, null);

        //    foreach (RegistrationDocumentDTO Registration in _regDocList)
        //    {
        //        List<vw_VoucherHeaderWithRoomNo> DailyRoomCharge = DailyRoomChargelineItemVouchers.Where(x => x.RegNumber == Registration.code).ToList();
        //        if (DailyRoomCharge == null || DailyRoomCharge.Count == 0)
        //        {
        //            RegistrationWithNoDailyRoomCharge.Add(Registration);

        //            count++;
        //            string RegNumb = Registration.code;
        //            CheckoutReportColumn checkoutReport = new CheckoutReportColumn()
        //            {
        //                SN = count,
        //                VoucherID = Registration.code,
        //                Customer = Registration.name,
        //                RegNo = RegNumb,
        //                Room = Registration.RoomNumber,
        //                Date = Registration.IssuedDate,
        //                PaymentType = Registration.PaymentMethod

        //            };

        //            checkoutReportList.Add(checkoutReport);


        //        }
        //    }

        //    gcReports.DataSource = checkoutReportList;
        //    gcReports.RefreshDataSource();
        //    gvReports.BestFitColumns();

        //    gvReports.Columns["SN"].Width = 25;
        //    gvReports.Columns["SN"].AppearanceCell.TextOptions.HAlignment = HorzAlignment.Near;
        //    gvReports.Columns["VoucherID"].Width = 80;
        //    gvReports.Columns["RegNo"].Width = 80;
        //    item = new GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum,
        //                "SubTotal", " {0:N}");
        //    gvReports.Columns["SubTotal"].Summary.Add(item);

        //    item = new GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum,
        //                "Discount", " {0:N}");
        //    gvReports.Columns["Discount"].Summary.Add(item);

        //    item = new GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum,
        //                "ServiceCharge", " {0:N}");
        //    gvReports.Columns["ServiceCharge"].Summary.Add(item);

        //    item = new GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum,
        //                "VAT", " {0:N}");
        //    gvReports.Columns["VAT"].Summary.Add(item);

        //    item = new GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum,
        //                "GrandTotal", " {0:N}");
        //    gvReports.Columns["GrandTotal"].Summary.Add(item);


        //}

        int MTDTimespan = 0;
        int YTDTimespan = 0;
        double MTDTotalCoupancy = 0;
        double YTDTotalCoupancy = 0;

        double YearDateTotalCoupancy = 0;
        double YearMTDTotalCoupancy = 0;
        double YearYTDTotalCoupancy = 0;
        private void ShowDailyBusinessReport(DateTime Date)
        {
            MTDTimespan = 0;
            YTDTimespan = 0;
            MTDTotalCoupancy = 0;
            YTDTotalCoupancy = 0; 
            YearDateTotalCoupancy = 0;
            YearMTDTotalCoupancy = 0;
            YearYTDTotalCoupancy = 0;
            printByPortrait = true;

            List<DailyBusinessReportDTO> _dailyBusinessReports = UIProcessManager.GetDailyBusinessReport(Date, SelectedHotelcode);

            if (_dailyBusinessReports == null)
                return;

            DateTime startOfMonth = new DateTime(Date.Year, Date.Month, 1);
            DateTime startOfYear = new DateTime(Date.Year, 1, 1);
            MTDTimespan = (Date - startOfMonth).Days + 1;
            YTDTimespan = (Date - startOfYear).Days + 1;

            foreach(DailyBusinessReportDTO dailyBusinessReport in _dailyBusinessReports)
            {
                if (dailyBusinessReport.occupancySummary != null)
                {
                    foreach (OccupancySummary occupancy in dailyBusinessReport.occupancySummary)
                    {
                        if (occupancy.MTD != null)
                        {
                            double mtd = (Convert.ToDouble(occupancy.MTD) * (Convert.ToDouble(MTDTimespan * occupancy.Rooms)) / 100);
                            MTDTotalCoupancy += mtd;
                        }
                        if (occupancy.MTD != null)
                        {
                            double mtd = (Convert.ToDouble(occupancy.YTD) * (Convert.ToDouble(YTDTimespan * occupancy.Rooms)) / 100);
                            YTDTotalCoupancy += mtd;
                        }
                        if (occupancy.LastYearDate != null)
                        {
                            double dateroom = ((Convert.ToDouble(occupancy.LastYearDate) * (occupancy.Rooms)) / 100);
                            YearDateTotalCoupancy += dateroom;
                        }
                        if (occupancy.LastYearMonth != null)
                        {
                            double mtd = (Convert.ToDouble(occupancy.LastYearMonth) * (Convert.ToDouble(MTDTimespan * occupancy.Rooms)) / 100);
                            YearMTDTotalCoupancy += mtd;
                        }
                        if (occupancy.LastYear != null)
                        {
                            double mtd = (Convert.ToDouble(occupancy.LastYear) * (Convert.ToDouble(YTDTimespan * occupancy.Rooms)) / 100);
                            YearYTDTotalCoupancy += mtd;
                        }
                    }
                }
            }


            gcReports.DataSource = _dailyBusinessReports;
            gcReports.RefreshDataSource();

            gvReports.ExpandMasterRow(0);
            gvReports.ExpandMasterRow(1);
            gvReports.ExpandMasterRow(2);
            gvReports.ExpandMasterRow(3);
            gvReports.ExpandMasterRow(4);

            GridView detailViewIncomeAny = gvReports.GetDetailView(1, 1) as GridView;
            if (detailViewIncomeAny != null)
            {
                var col = detailViewIncomeAny.Columns["IsPrevious"];
                col.Visible = false;
            }

            GridView detailViewSalesCenter = gvReports.GetDetailView(2, 2) as GridView;
            if (detailViewSalesCenter != null)
            {
                var col = detailViewSalesCenter.Columns["IsPrevious"];
                col.Visible = false;
            }


            GridView detailView = gvReports.GetDetailView(4, 4) as GridView;
            if (detailView != null)
            {

                detailView.Columns["ADR"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                detailView.Columns["ADR"].SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Custom;
                detailView.Columns["ADR"].SummaryItem.DisplayFormat = " {0:N2}";
                detailView.BestFitColumns();

            }


        }

        private void ShowManagerialFlashReport(DateTime date)
        {
            printByPortrait = true;

            List<ManagerialFlashReport> _managerialFlashList = UIProcessManager.GetManagerialFlashReport(date, SelectedHotelcode);

            if (_managerialFlashList == null)
                return;
            gcReports.DataSource = _managerialFlashList;


            gvReports.Columns["CurrentDate"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            gvReports.Columns["CurrentMonth"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            gvReports.Columns["CurrentYear"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            gvReports.Columns["LastYearToday"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            gvReports.Columns["LastYearThisMonth"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            gvReports.Columns["LastYearNow"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;

            gvReports.Columns["CurrentDate"].Caption = date.Year + " Day";
            gvReports.Columns["CurrentMonth"].Caption = date.Year + " Month";
            gvReports.Columns["CurrentYear"].Caption = date.Year + " Year";
            gvReports.Columns["LastYearToday"].Caption = date.Year - 1 + " Day";
            gvReports.Columns["LastYearThisMonth"].Caption = date.Year - 1 + " Month";
            gvReports.Columns["LastYearNow"].Caption = date.Year - 1 + " Year";

            gcReports.RefreshDataSource();
            gvReports.RefreshData();
            gvReports.BestFitColumns();


        }

        private void ShowRoomIncomeReport(DateTime startDate, DateTime endDate)
        {
            Progress_Reporter.Show_Progress("Binding Room Income Report. . . ", "Please Wait.......");
            List<RoomIncomeReport> roomIncomeReportList = UIProcessManager.GetRoomIncomeReport(startDate, endDate, SelectedHotelcode);

            if (roomIncomeReportList == null)
                return;
            gcReports.DataSource = roomIncomeReportList;
            gcReports.RefreshDataSource();
            gvReports.BestFitColumns();

            gvReports.Columns["Registration"].Width = 80;

            gvReports.Columns["RoomType"].GroupIndex = 0;

            item = new GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Custom,
                        "RoomType", "Total = ");
            gvReports.Columns["RoomType"].Summary.Add(item);

            item = new GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum,
                        "SubTotal", "{0:N}");
            gvReports.Columns["SubTotal"].Summary.Add(item);

            item = new GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum,
                        "ServiceCharge", "{0:N}");
            gvReports.Columns["ServiceCharge"].Summary.Add(item);

            item = new GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum,
                        "VAT", "{0:N}");
            gvReports.Columns["VAT"].Summary.Add(item);

            item = new GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum,
                        "Discount", "{0:N}");
            gvReports.Columns["Discount"].Summary.Add(item);

            item = new GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum,
                        "GrandTotal", "{0:N}");
            gvReports.Columns["GrandTotal"].Summary.Add(item);


            //'Total" Display Summary
            GridGroupSummaryItem displaySummary = new GridGroupSummaryItem();
            displaySummary.FieldName = "RateType";
            displaySummary.SummaryType = DevExpress.Data.SummaryItemType.Sum;
            displaySummary.DisplayFormat = "Total = ";
            displaySummary.ShowInGroupColumnFooter = gvReports.Columns["RateType"];
            gvReports.GroupSummary.Add(displaySummary);

            //Subtotal Summary
            GridGroupSummaryItem subTotal = new GridGroupSummaryItem();
            subTotal.FieldName = "SubTotal";
            subTotal.SummaryType = DevExpress.Data.SummaryItemType.Sum;
            subTotal.DisplayFormat = "{0:N2}";
            subTotal.ShowInGroupColumnFooter = gvReports.Columns["SubTotal"];
            gvReports.GroupSummary.Add(subTotal);

            //Service Charge Summary
            GridGroupSummaryItem serCharge = new GridGroupSummaryItem();
            serCharge.FieldName = "ServiceCharge";
            serCharge.SummaryType = DevExpress.Data.SummaryItemType.Sum;
            serCharge.DisplayFormat = "{0:N2}";
            serCharge.ShowInGroupColumnFooter = gvReports.Columns["ServiceCharge"];
            gvReports.GroupSummary.Add(serCharge);

            //VAT Summary
            GridGroupSummaryItem vatSummary = new GridGroupSummaryItem();
            vatSummary.FieldName = "VAT";
            vatSummary.SummaryType = DevExpress.Data.SummaryItemType.Sum;
            vatSummary.DisplayFormat = "{0:N2}";
            vatSummary.ShowInGroupColumnFooter = gvReports.Columns["VAT"];
            gvReports.GroupSummary.Add(vatSummary);

            //Discount Summary
            GridGroupSummaryItem discountSummaryItem = new GridGroupSummaryItem();
            discountSummaryItem.FieldName = "Discount";
            discountSummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Sum;
            discountSummaryItem.DisplayFormat = "{0:N2}";
            discountSummaryItem.ShowInGroupColumnFooter = gvReports.Columns["Discount"];
            gvReports.GroupSummary.Add(discountSummaryItem);

            //Grand Total Summary
            GridGroupSummaryItem summaryItem = new GridGroupSummaryItem();
            summaryItem.FieldName = "GrandTotal";
            summaryItem.SummaryType = DevExpress.Data.SummaryItemType.Sum;
            summaryItem.DisplayFormat = "{0:N2}";
            summaryItem.ShowInGroupColumnFooter = gvReports.Columns["GrandTotal"];
            gvReports.GroupSummary.Add(summaryItem);



            gvReports.ExpandAllGroups();

        }

        private void ShowDailySalesSummary(DateTime Date, DateTime endDate)
        {
            DataTable dataTable = new DataTable();
            printByPortrait = false;
            List<VwVoucherHeaderWithRegistrationViewDTO> salesSummaryList = UIProcessManager.GetDailySalesSummary(Date, endDate, SelectedHotelcode);

            if (salesSummaryList != null && salesSummaryList.Count > 0)
            {
                List<Tuple<int, string, decimal>> categoryPriceList = new List<Tuple<int, string, decimal>>();

                dataTable.Columns.Add(new DataColumn() { ColumnName = "SN", Caption = "SN" });
                dataTable.Columns.Add(new DataColumn() { ColumnName = "VoucherCode", Caption = "Voucher Code" });
                dataTable.Columns.Add(new DataColumn() { ColumnName = "Customer", Caption = "Customer" });
                dataTable.Columns.Add(new DataColumn() { ColumnName = "RegID", Caption = "Reg. #" });
                dataTable.Columns.Add(new DataColumn() { ColumnName = "RoomNum", Caption = "Room" });
                dataTable.Columns.Add(new DataColumn() { ColumnName = "RoomPOS", Caption = "Room POS Charge" });
                dataTable.Columns.Add(new DataColumn() { ColumnName = "CityLedger", Caption = "City Ledger" });
                dataTable.Columns.Add(new DataColumn() { ColumnName = "Allowance", Caption = "Allowance" });
                dataTable.Columns.Add(new DataColumn() { ColumnName = "Cash", Caption = "Cash" });
                int counter = 0;
                foreach (var sales in salesSummaryList)
                {
                    counter++;
                    DataRow row = dataTable.NewRow();

                    row[dataTable.Columns["SN"]] = counter;
                    row[dataTable.Columns["VoucherCode"]] = sales.VoucherCode;

                    row[dataTable.Columns["Customer"]] = string.IsNullOrEmpty(sales.Company) ? sales.Customer : sales.Company;





                    if (!string.IsNullOrEmpty(sales.RegCode))
                    {
                        row[dataTable.Columns["RegID"]] = sales.RegCode;
                        row[dataTable.Columns["RoomNum"]] = sales.RoomNumber;

                        if (sales.Note != "check_out")
                            row[dataTable.Columns["RoomPOS"]] = Math.Round(sales.GrandTotal == null ? 0 : sales.GrandTotal.Value, 2);


                    }
                    else
                    {

                        if (sales.Definition == CNETConstantes.CREDITSALES)
                        {
                            row[dataTable.Columns["CityLedger"]] = Math.Round(sales.GrandTotal == null ? 0 : sales.GrandTotal.Value, 2);


                        }
                        else
                        {
                            row[dataTable.Columns["Cash"]] = Math.Round(sales.GrandTotal == null ? 0 : sales.GrandTotal.Value, 2);

                        }

                        // }
                    }


                    //Segment To Parent Preferences
                    List<LineItemDTO> linetItemList = UIProcessManager.GetLineItemByvoucher(sales.VoucherId);
                    if (linetItemList != null)
                    {
                        foreach (var li in linetItemList)
                        {
                            ArticleDTO Artilces = UIProcessManager.GetArticleById(li.Article);
                            if (Artilces.Preference == null) continue;

                            int? rootCategory = GetRootPreference(Artilces.Preference);
                            if (rootCategory == null) continue;

                            PreferenceDTO pref = UIProcessManager.GetPreferenceById(rootCategory.Value);
                            if (pref == null) continue;

                            var rootPrice = categoryPriceList.FirstOrDefault(t => t.Item1 == rootCategory);

                            if (rootPrice != null)
                            {
                                categoryPriceList.Remove(rootPrice);
                                rootPrice = new Tuple<int, string, decimal>(pref.Id, pref.Description, (rootPrice.Item3 + li.TotalAmount));
                            }
                            else
                            {
                                rootPrice = new Tuple<int, string, decimal>(pref.Id, pref.Description, li.TotalAmount);

                            }

                            categoryPriceList.Add(rootPrice);


                        }
                    }

                    foreach (var category in categoryPriceList)
                    {
                        if (!dataTable.Columns.Contains(category.Item2.ToString()))
                        {
                            dataTable.Columns.Add(new DataColumn() { ColumnName = category.Item2.ToString(), Caption = category.Item2 });


                        }

                        row[dataTable.Columns[category.Item2]] = Math.Round(category.Item3, 2);

                    }
                    categoryPriceList.Clear();
                    if (!dataTable.Columns.Contains("tax"))
                        dataTable.Columns.Add(new DataColumn() { ColumnName = "tax", Caption = "TAX" });
                    if (!dataTable.Columns.Contains("ServiceCharge"))
                        dataTable.Columns.Add(new DataColumn() { ColumnName = "ServiceCharge", Caption = "Service Charge" });
                    if (!dataTable.Columns.Contains("Discount"))
                        dataTable.Columns.Add(new DataColumn() { ColumnName = "Discount", Caption = "Discount" });
                    if (!dataTable.Columns.Contains("GrandTotal"))
                        dataTable.Columns.Add(new DataColumn() { ColumnName = "GrandTotal", Caption = "Grand Total" });


                    row[dataTable.Columns["tax"]] = Math.Round(sales.TaxAmount, 2);
                    row[dataTable.Columns["ServiceCharge"]] = Math.Round(sales.ServiceCharge == null ? 0 : sales.ServiceCharge.Value, 2);
                    row[dataTable.Columns["Discount"]] = Math.Round(sales.Discount == null ? 0 : sales.Discount.Value, 2);
                    row[dataTable.Columns["GrandTotal"]] = Math.Round(sales.GrandTotal == null ? 0 : sales.GrandTotal.Value, 2);

                    dataTable.Rows.Add(row);

                    //dtoList.Add(dailySales);
                }

                gcReports.DataSource = dataTable;
                gcReports.RefreshDataSource();
                gvReports.RefreshData();
                //gvReports.BestFitColumns();

                gvReports.Columns["SN"].Width = 25;
                gvReports.Columns["VoucherCode"].BestFit();
                gvReports.Columns["Customer"].BestFit();


                gvReports.OptionsSelection.EnableAppearanceFocusedCell = false;
                gvReports.OptionsSelection.EnableAppearanceHideSelection = false;
                gvReports.OptionsSelection.EnableAppearanceFocusedRow = true;

                //Add Summary items for price columns
                for (int i = 5; i < dataTable.Columns.Count; i++)
                {

                    item = new GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum,
                            dataTable.Columns[i].ColumnName, " {0:N}");
                    gvReports.Columns[i].Summary.Add(item);

                    gvReports.Columns[i].AppearanceCell.TextOptions.HAlignment = HorzAlignment.Far;

                    if (gvReports.Columns[i].Name.Contains("9999") || gvReports.Columns[i].Name.Contains("8888") || gvReports.Columns[i].Name.ToLower().Contains("pref"))
                    {

                        gvReports.Columns[i].AppearanceHeader.BackColor = ColorTranslator.FromHtml("InactiveCaption");
                        gvReports.Columns[i].AppearanceCell.BackColor = ColorTranslator.FromHtml("InactiveCaption");
                        gvReports.Columns[i].AppearanceHeader.ForeColor = ColorTranslator.FromHtml("DarkBlue");
                        gvReports.Columns[i].AppearanceHeader.Options.UseBackColor = true;
                        gvReports.Columns[i].AppearanceHeader.Options.UseForeColor = true;
                        //InactiveCaption
                        //ControlLight
                        //LightGray
                        //LightBlue
                        //PaleGoldenrod
                    }

                }



                //move some columns to the last
                gvReports.Columns["tax"].VisibleIndex = dataTable.Columns.Count;
                gvReports.Columns["ServiceCharge"].VisibleIndex = dataTable.Columns.Count;
                gvReports.Columns["Discount"].VisibleIndex = dataTable.Columns.Count;
                gvReports.Columns["GrandTotal"].VisibleIndex = dataTable.Columns.Count;


                item = new GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum,
                           "", "Total = ");
                gvReports.Columns["RoomNum"].Summary.Add(item);
            }
        }

        private int? GetRootPreference(int? prefCode)
        {
            if (prefCode == null)
                return null;

            int? rootParent = null;
            PreferenceDTO root = UIProcessManager.GetPreferenceById(prefCode.Value);
            if (root == null || root.Id == root.ParentId) 
                return prefCode;
            else
            {
                if (root.ParentId == null)
                {
                    rootParent = root.Id;
                }
                else
                {
                    rootParent = GetRootPreference(root.ParentId.Value);
                }
            }

            return rootParent;
        }

        private void ShowCashDroppedReport(DateTime startDate, DateTime endDate)
        {
            printByPortrait = true;

            List<CashDropDocumentReportDTO> cashDropDocuments = UIProcessManager.GetCashDropreport(startDate, endDate, SelectedHotelcode, user);
             
            if (cashDropDocuments == null)
                return;

            gcReports.DataSource = cashDropDocuments;
            gcReports.RefreshDataSource();
            gvReports.BestFitColumns();

            decimal? SalescashOnHand = 0;
            if (cashDropDocuments != null && cashDropDocuments.Count > 0 )
                SalescashOnHand = cashDropDocuments.Where(s=> s.SalesDocuments != null ).Sum(x =>  x.SalesDocuments.Sum(s => s.Cash));

            decimal? PaymnetcashOnHand = 0;
            if (cashDropDocuments != null && cashDropDocuments.Count > 0)
                PaymnetcashOnHand = cashDropDocuments.Where(s => s.PaymentDocuments != null).Sum(x => x.PaymentDocuments.Sum(s => s.Amount));

            string totalBalance = string.Format("{0:N}", SalescashOnHand + PaymnetcashOnHand);

            item = new GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Custom,
                    "Total", "Total Cash On Hand = " + totalBalance);
            gvReports.Columns["Documents"].Summary.Add(item);

            gvReports.ExpandMasterRow(0);
            gvReports.ExpandMasterRow(1);
            gvReports.ExpandMasterRow(2);
        }

        private void ShowRateCheckReport(DateTime Date)
        {
            List<RateCheckReport> rateCheckReportList = UIProcessManager.GetRateCheckReport(Date, SelectedHotelcode);

            if (rateCheckReportList == null)
                return;

            gcReports.DataSource = rateCheckReportList.OrderBy(p => p.Variance);
            gcReports.RefreshDataSource();
            gvReports.BestFitColumns();
            if (rateCheckReportList != null && rateCheckReportList.Count > 0)
            {
                gvReports.Columns["Reg_No"].Width = 80;
                gvReports.Columns["RateAmount"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
                gvReports.Columns["RateCodeAmount"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
                gvReports.Columns["Variance"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            }


            gcReports.RefreshDataSource();

        }

        private void ShowRateAdjustmentReport(DateTime Date)
        {
            List<RateAdjustmentReport> rateAdjustmentReportList = UIProcessManager.GetRateAdjustmentReport(Date, SelectedHotelcode);

            if (rateAdjustmentReportList == null)
                return;

            gcReports.DataSource = rateAdjustmentReportList;
            gcReports.RefreshDataSource();
            gvReports.BestFitColumns();

            gvReports.Columns["Reg_No"].Width = 80;
            gvReports.Columns["Amount"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            gvReports.Columns["Value"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;


            gcReports.RefreshDataSource();

        }

        private void ShowDropoffReport(DateTime Date, int? Guest)
        {
            rpgSearchByUser.Visible = true;
            rpgSearchByUser.Enabled = true;

            List<TravelDetailReportDTO> Dropofflist = UIProcessManager.GetTravelDetailReport(CNETConstantes.TD_DROP_OFF, Date, Guest, SelectedHotelcode);

            if (Dropofflist == null)
                return;
            gcReports.DataSource = Dropofflist;
            gcReports.RefreshDataSource();
            gvReports.BestFitColumns();
            gvReports.Columns["TravelTimestamp"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            gvReports.Columns["TravelTimestamp"].DisplayFormat.FormatString = "yyyy-MM-dd hh:mm:ss";

        }

        private void ShowPickupReport(DateTime Date, int? Guest)
        {
            rpgSearchByUser.Visible = true;
            rpgSearchByUser.Enabled = true;

            List<TravelDetailReportDTO> PickUplist = UIProcessManager.GetTravelDetailReport(CNETConstantes.TD_PICK_UP, Date, Guest, SelectedHotelcode);

            if (PickUplist == null)
                return;
            gcReports.DataSource = PickUplist;
            gcReports.RefreshDataSource();
            gvReports.BestFitColumns();
            gvReports.Columns["TravelTimestamp"].DisplayFormat.FormatType = FormatType.DateTime;
            gvReports.Columns["TravelTimestamp"].DisplayFormat.FormatString = "yyyy-MM-dd hh:mm:ss";

        }

        private void ShowDetailDailySalesTransaction(DateTime Date)
        {
            List<DetailDailySalesTransactionsReportDTO> detailDailyList = UIProcessManager.GetDetailDailySalesTransaction(Date, SelectedHotelcode);
            if (detailDailyList == null)
                return;
            gcReports.DataSource = detailDailyList;
            gcReports.RefreshDataSource();
            gvReports.BestFitColumns();

            gvReports.Columns["Reg_No"].Width = 80;
            gvReports.Columns["SubTotal"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            gvReports.Columns["Discount"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            gvReports.Columns["AdditionalCharge"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            gvReports.Columns["Tax"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            gvReports.Columns["GrandTotal"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;

            item = new GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Custom,
            "Guest", "Total:  ");
            gvReports.Columns["Guest"].Summary.Add(item);

            item = new GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum,
                         "SubTotal", "{0:N}");
            gvReports.Columns["SubTotal"].Summary.Add(item);

            item = new GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum,
                         "Discount", "{0:N}");
            gvReports.Columns["Discount"].Summary.Add(item);

            item = new GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum,
                         "GrandTotal", "{0:N}");
            gvReports.Columns["GrandTotal"].Summary.Add(item);

            item = new GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum,
                         "AdditionalCharge", "{0:N}");
            gvReports.Columns["AdditionalCharge"].Summary.Add(item);

            item = new GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum,
                         "Tax", "{0:N}");
            gvReports.Columns["Tax"].Summary.Add(item);


            gcReports.RefreshDataSource();
        }

        private void ShowRoomMoveReport(DateTime Date)
        {
            List<RoomMovedReportDTO> roomMovedlist = UIProcessManager.GetRoomMovedReport(Date, SelectedHotelcode);
            if (roomMovedlist == null)
                return;
            gcReports.DataSource = roomMovedlist;
            gcReports.RefreshDataSource();
            gvReports.BestFitColumns();

            gvReports.Columns["Reg_No"].Width = 80;
            gvReports.Columns["RateAmount"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            gcReports.RefreshDataSource();
        }

        private void ShowPostmasterInhouseList(DateTime Date)
        {
            List<RegistrationReportInfoDTO> postMasterList = UIProcessManager.GetRegistrationByFilter("post master", Date, new List<int>() { CNETConstantes.CHECKED_IN_STATE }, SelectedHotelcode);

            if (postMasterList == null)
                return;
            gcReports.DataSource = postMasterList;
            gvReports.BestFitColumns();
            gvReports.Columns["Reg_No"].Width = 80;
            gcReports.RefreshDataSource();
        }

        private void ShowDepartedList(DateTime Date)
        {
            List<RegistrationReportInfoDTO> departedList = UIProcessManager.GetRegistrationByFilter("departed", Date, new List<int>() { CNETConstantes.CHECKED_OUT_STATE }, SelectedHotelcode);

            if (departedList == null)
                return;
            gcReports.DataSource = departedList;
            gvReports.BestFitColumns();
            gvReports.Columns["Reg_No"].Width = 80;
            gcReports.RefreshDataSource();
        }

        private void ShowDueOuts(DateTime Date)
        {
            List<RegistrationReportInfoDTO> dueOutList = UIProcessManager.GetRegistrationByFilter("due out", Date, new List<int>() { CNETConstantes.CHECKED_IN_STATE }, SelectedHotelcode);

            if (dueOutList == null)
                return;
            gcReports.DataSource = dueOutList;
            gvReports.BestFitColumns();
            gvReports.Columns["Reg_No"].Width = 80;
            gcReports.RefreshDataSource();
        }

        private void ShowStayOvers(DateTime Date)
        {
            List<RegistrationReportInfoDTO> stayOverList = UIProcessManager.GetRegistrationByFilter("stay over", Date, new List<int>() { CNETConstantes.CHECKED_IN_STATE }, SelectedHotelcode);

            if (stayOverList == null)
                return;
            gcReports.DataSource = stayOverList;
            gvReports.BestFitColumns();
            gvReports.Columns["Reg_No"].Width = 80;
            gcReports.RefreshDataSource();
        }

        private void ShowArrivedList(DateTime Date)
        {
            List<RegistrationReportInfoDTO> arrivedList = UIProcessManager.GetRegistrationByFilter("arrived", Date, new List<int>() { CNETConstantes.CHECKED_IN_STATE }, SelectedHotelcode);

            if (arrivedList == null)
                return;
            gcReports.DataSource = arrivedList;
            gvReports.BestFitColumns();
            gvReports.Columns["Reg_No"].Width = 80;
            gcReports.RefreshDataSource();
        }

        private void ShowArrivalsList(DateTime Date)
        {
            List<RegistrationReportInfoDTO> arrivalsList = UIProcessManager.GetRegistrationByFilter("arrived", Date, new List<int>() { CNETConstantes.SIX_PM_STATE, CNETConstantes.GAURANTED_STATE }, SelectedHotelcode);

            if (arrivalsList == null)
                return;
            gcReports.DataSource = arrivalsList;
            gvReports.BestFitColumns();
            gvReports.Columns["Reg_No"].Width = 80;
            gcReports.RefreshDataSource();
        }

        private void ShowUnassignedReservations(DateTime startDate, DateTime endDate)
        {
            printByPortrait = true;
            List<RegistrationReportInfoDTO> unassignedList = UIProcessManager.GetRegistrationByFilter("unassigned", startDate, new List<int>() { CNETConstantes.SIX_PM_STATE, CNETConstantes.OSD_WAITLIST_STATE, CNETConstantes.GAURANTED_STATE }, SelectedHotelcode);

            if (unassignedList == null)
                return;
            gcReports.DataSource = unassignedList;
            gvReports.BestFitColumns();
            gvReports.Columns["SN"].Width = 25;
            //gvReports.Columns["Color"].Visible = false;
            gcReports.RefreshDataSource();
        }

        private void ShowBillTransferReport(DateTime startDate, DateTime endDate)
        {
            printByPortrait = false;

            List<BillTransferReport> billTransferDtoList = UIProcessManager.GetBillTransferDetail(startDate, endDate, SelectedHotelcode);

            if (billTransferDtoList == null)
                return;

            gcReports.DataSource = billTransferDtoList;
            gvReports.BestFitColumns();
            //gvReports.Columns["Color"].Visible = false;
            gcReports.RefreshDataSource();

            for (int i = 0; i < billTransferDtoList.Count; i++)
                gvReports.ExpandMasterRow(i);
        }

        private void ShowCheckReportOfTheDay(DateTime Date)
        {
            try
            {
                gcReports.DataSource = null;
                return;
                gvReports.BestFitColumns();
                gvReports.Columns["Reg_No"].Width = 80;
                gvReports.Columns["Amount"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
                gcReports.RefreshDataSource();
            }
            catch
            {

            }
        }

        private void ShowCreditCardsOfTheDay(DateTime Date)
        {
            try
            {
                gcReports.DataSource = null;
                return;

                gvReports.BestFitColumns();
                gvReports.Columns["Reg_No"].Width = 80;
                gvReports.Columns["etbAmount"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
                gcReports.RefreshDataSource();

            }
            catch
            {

            }
        }
        private void ShowPaymentMethodDetailReport(DateTime Date)
        {
            try
            {
                gcReports.DataSource = null;
                List<RegistrationWithPaymentMethodDTO> nonLineItemTransactionReports = UIProcessManager.GetRegistrationWithPaymentMethodVoucher(Date.Date, Date.Date, false, SelectedHotelcode,
                    new List<int>() { CNETConstantes.CASHRECIPT, CNETConstantes.PAID_OUT_VOUCHER, CNETConstantes.BANKDEBITNOTE, CNETConstantes.REFUND }, null, null);

                if (nonLineItemTransactionReports == null)
                    return;
                Progress_Reporter.Show_Progress("Binding " + selectedReportName + ". . . ", "Please Wait ........");

                gcReports.DataSource = nonLineItemTransactionReports;
                gcReports.RefreshDataSource();
                gvReports.BestFitColumns();

                gvReports.Columns["PaymentMethod"].GroupIndex = 0;
                gvReports.BestFitColumns();

                gvReports.Columns["VoucherCode"].Width = 80;
                gvReports.Columns["RegNo"].Width = 80;

                gvReports.ExpandAllGroups();

                groupSumm = new GridGroupSummaryItem(DevExpress.Data.SummaryItemType.Sum,
                       "GrandTotal", gvReports.Columns["GrandTotal"], "{0:N}");
                gvReports.GroupSummary.Add(groupSumm);

                item = new GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum,
                            "GrandTotal", " {0:N}");
                gvReports.Columns["GrandTotal"].Summary.Add(item);

              

            }
            catch
            {

            }
        }


        private void ShowPaymentMethodSummaryReport(DateTime Date)
        {
            try
            {
                gcReports.DataSource = null;
                List<RegistrationPaymentMethodSummaryDTO> nonLineItemTransactionReports = UIProcessManager.GetRegistrationPaymentMethodSummary(Date.Date, Date.Date, false, SelectedHotelcode,
                    new List<int>() { CNETConstantes.CASHRECIPT, CNETConstantes.PAID_OUT_VOUCHER, CNETConstantes.BANKDEBITNOTE, CNETConstantes.REFUND }, null, null);

                if (nonLineItemTransactionReports == null)
                    return;
                Progress_Reporter.Show_Progress("Binding " + selectedReportName + ". . . ", "Please Wait ........");

                gcReports.DataSource = nonLineItemTransactionReports;
                gcReports.RefreshDataSource();
                gvReports.BestFitColumns();

                gvReports.Columns["PaymentMethod"].GroupIndex = 0;
                gvReports.BestFitColumns();

                gvReports.Columns["Date"].Width = 80; 

                gvReports.ExpandAllGroups();

                groupSumm = new GridGroupSummaryItem(DevExpress.Data.SummaryItemType.Sum,
                       "TotalAmount", gvReports.Columns["TotalAmount"], "{0:N}");
                gvReports.GroupSummary.Add(groupSumm);

                item = new GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum,
                            "TotalAmount", " {0:N}");
                gvReports.Columns["TotalAmount"].Summary.Add(item);

            }
            catch
            {

            }
        }

        


        private void ShowDepositLedger(DateTime Date)
        {
            List<DepositLedgerReportDTO> depositLedgerList = UIProcessManager.GetDepositLedger(Date, Date, SelectedHotelcode);
            if (depositLedgerList == null)
                return;
            gcReports.DataSource = depositLedgerList;
            gvReports.BestFitColumns();
            gvReports.Columns["Reg_No"].Width = 80;
            gvReports.Columns["DepositBalance"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;

            var item = new GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum,
                        "DepositBalance", "SUM = {0:N}");
            gvReports.Columns["DepositBalance"].Summary.Add(item);

            gcReports.RefreshDataSource();
        }

        private void ShowCityledger(DateTime startDate, DateTime endDate)
        {
            printByPortrait = true;
            Progress_Reporter.Show_Progress("Populating City Ledger Report", "Please Wait. . . ");

            List<CityLedgerDTO> cityLedgerList = UIProcessManager.GetRegistrationCityLedger(startDate, endDate, SelectedHotelcode);

            if (cityLedgerList == null)
                return;
            gcReports.DataSource = cityLedgerList;
            gvReports.BestFitColumns();
            gvReports.Columns["Reg_No"].Width = 80;
            gvReports.Columns["Amount"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            item = new GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum,
                   "Amount", "Total = {0:N2}");
            gvReports.Columns["Amount"].Summary.Add(item);
            gcReports.RefreshDataSource();

            Progress_Reporter.Close_Progress();
        }


        private void ShowNoShowReport(DateTime Date)
        {
            List<RegistrationReportInfoDTO> noShowList = UIProcessManager.GetRegistrationByFilter("noshow", Date, new List<int>() { CNETConstantes.NO_SHOW_STATE }, SelectedHotelcode);

            if (noShowList == null)
                return;

            gcReports.DataSource = noShowList;
            gvReports.BestFitColumns();
            gvReports.Columns["Reg_No"].Width = 80;
            gcReports.RefreshDataSource();
        }

        private void ShowCancellationOfTheDay(DateTime Date)
        {
            List<RegistrationReportInfoDTO> listCancelled = UIProcessManager.GetRegistrationByFilter("cancellation", Date, new List<int>() { CNETConstantes.OSD_CANCEL_STATE }, SelectedHotelcode);

            if (listCancelled == null)
                return;

            gcReports.DataSource = listCancelled;
            gvReports.BestFitColumns();
            gvReports.Columns["Reg_No"].Width = 80;
            gvReports.Columns["RateAmount"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            gcReports.RefreshDataSource();
        }

        private void ShowCashierSummaryReport(DateTime Date, int? user)
        {
            printByPortrait = true;
            rpgSearchByUser.Visible = true;
            CasherSummaryReportDTO data = UIProcessManager.GetCasherSummary(Date, user, SelectedHotelcode);

            if (data == null)
                return;

            gcReports.DataSource = data.CashierSummaryReportByUser == null ? new List<CashierSummaryReportByUser>() : data.CashierSummaryReportByUser;


            gvReports.Columns["User"].GroupIndex = 0;
            gvReports.Columns["PaymentMethod"].GroupIndex = 1;
            gvReports.Columns["Currency"].GroupIndex = 2;
            gvReports.BestFitColumns();



            gcReports2.DataSource = data.CashierSummaryReportBySummary == null ? new  List<CashierSummaryReportBySummary>(): data.CashierSummaryReportBySummary;
            gvReports2.Columns["PaymentMethod"].GroupIndex = 0;
            gvReports2.Columns["Currency"].GroupIndex = 1;
            gvReports2.BestFitColumns();

            gvReports.Columns["CurrencyAmount"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            gvReports.Columns["etbTotal"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            gvReports.Columns["Rate"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            gvReports.ExpandAllGroups();
            gcReports.RefreshDataSource();

            groupSumm = new GridGroupSummaryItem(DevExpress.Data.SummaryItemType.Sum,
                   "etbTotal", gvReports.Columns["etbTotal"], "{0:N}");
            gvReports.GroupSummary.Add(groupSumm);

            item = new GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum,
                       "etbTotal", "{0:N}");
            gvReports.Columns["etbTotal"].Summary.Add(item);


            gvReports2.BestFitColumns();
            gvReports2.Columns["CurrencyAmount"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            gvReports2.Columns["etbTotal"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            gvReports2.Columns["Rate"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            gvReports2.ExpandAllGroups();
            gcReports2.RefreshDataSource();

            groupSumm = new GridGroupSummaryItem(DevExpress.Data.SummaryItemType.Sum,
                   "etbTotal", gvReports2.Columns["etbTotal"], "{0:N}");
            gvReports2.GroupSummary.Add(groupSumm);

            item = new GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum,
                       "etbTotal", "{0:N}");
            gvReports2.Columns["etbTotal"].Summary.Add(item);
        }

        private void ShowNationalityReport(DateTime startDate, DateTime endDate)
        {
            printByPortrait = true;

            List<NationalityReportDTO> reportList = UIProcessManager.GetNationalityReport(startDate, endDate, SelectedHotelcode);
            if (reportList == null)
                return;



            gcReports.DataSource = reportList;
            gvReports.RefreshData();
            gvReports.Columns["Continent"].GroupIndex = 0;

            gvReports.Columns["Guetid"].Visible = false;

            gvReports.Columns["Amount"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            gvReports.Columns["Amount"].DisplayFormat.FormatString = "{0:N}";
            gvReports.Columns["Amount"].DisplayFormat.FormatType = FormatType.Custom;

            gvReports.Columns["NoOfCustomer"].Width = 70;


            groupSumm = new GridGroupSummaryItem(DevExpress.Data.SummaryItemType.Sum,
                   "NoOfCustomer", gvReports.Columns["NoOfCustomer"], "{0:}");
            gvReports.GroupSummary.Add(groupSumm);

            groupSumm = new GridGroupSummaryItem(DevExpress.Data.SummaryItemType.Sum,
                   "Amount", gvReports.Columns["Amount"], "{0:N}");
            gvReports.GroupSummary.Add(groupSumm);

            item = new GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Custom,
                       "PoliticalName", "TOTAL = ");
            gvReports.Columns["PoliticalName"].Summary.Add(item);


            item = new GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum,
                       "Amount", "{0:N}");
            gvReports.Columns["Amount"].Summary.Add(item);

            item = new GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum,
                       "NoOfCustomer", "{0:}");
            gvReports.Columns["NoOfCustomer"].Summary.Add(item);

            gvReports.BestFitColumns();
            gvReports.ExpandAllGroups();
        }

        private void ShowPackageReport(DateTime Date)
        {
            //ribbonPageGroup5.Visible = false;

            List<PackageReportDTO> packageReports = UIProcessManager.GetPackageReport(Date, SelectedHotelcode);
            if (packageReports == null)
                return;

            gcReports.DataSource = packageReports.OrderBy(p => p.Room).ToList();
            gvReports.Columns["RoomType"].GroupIndex = 0;
            gvReports.ExpandAllGroups();
            gvReports.BestFitColumns();

            groupSumm = new GridGroupSummaryItem()
            {
                SummaryType = DevExpress.Data.SummaryItemType.Count,
                FieldName = "RoomType"
            };
            gvReports.GroupSummary.Add(groupSumm);

            groupSumm = new GridGroupSummaryItem(DevExpress.Data.SummaryItemType.Sum,
                "Adult", gvReports.Columns["Adult"], "{0}");
            gvReports.GroupSummary.Add(groupSumm);

            groupSumm = new GridGroupSummaryItem(DevExpress.Data.SummaryItemType.Sum,
                "Child", gvReports.Columns["Child"], "{0}");
            gvReports.GroupSummary.Add(groupSumm);

            groupSumm = new GridGroupSummaryItem(DevExpress.Data.SummaryItemType.Sum,
                "UnitPackage", gvReports.Columns["UnitPackage"], "{0:N}");
            gvReports.GroupSummary.Add(groupSumm);

            groupSumm = new GridGroupSummaryItem(DevExpress.Data.SummaryItemType.Sum,
                "TotalPackage", gvReports.Columns["TotalPackage"], "{0:N}");
            gvReports.GroupSummary.Add(groupSumm);

            item = new GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum,
                        "UnitPackage", "Total = {0:N}");
            gvReports.Columns["UnitPackage"].Summary.Add(item);

            item = new GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum,
                        "TotalPackage", "Total = {0:N}");
            gvReports.Columns["TotalPackage"].Summary.Add(item);

            gvReports.Columns["Reg_No"].Width = 130;
            gvReports.Columns["Adult"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            gvReports.Columns["Child"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            gvReports.Columns["UnitPackage"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            gvReports.Columns["TotalPackage"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            gcReports.RefreshDataSource();

        }

        private void ShowPoliceReport(DateTime Date)
        {
            gvReports.Columns.Clear();

            List<PoliceReportDTO> listPolice = UIProcessManager.GetPoliceReports(Date, SelectedHotelcode);

            if (listPolice == null)
                return;

            // gvReports.Columns["DepartureDate"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            gcReports.DataSource = listPolice;
            gvReports.BestFitColumns();
            gvReports.Columns["ArrivalDate"].DisplayFormat.FormatString = "yyyy/mm/dd hh:mm:ss";
            gvReports.Columns["DepartureDate"].DisplayFormat.FormatString = "yyyy/mm/dd hh:mm:ss";
            gvReports.Columns["SN"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            gvReports.Columns["SN"].Width = 29;
            gvReports.Columns["Room"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            gcReports.RefreshDataSource();

            // SendPoliceReporttoGovernemnt(listPolice, Date);


        }

        //private void SendPoliceReporttoGovernemnt(List<PoliceReport> listPolice, DateTime ReportDate)
        //{
        //    try
        //    {
        //      Progress_Reporter.Show_Progress("Sending Police Report to API", "Please Wait...");
        //        OrganizationUnitDefinition SelectdBranch = LoginPage.Authentication.OrganizationUnitDefinitionBufferList.FirstOrDefault(x => x.code == SelectedHotelcode);
        //        Lookup companycodeLookup = LoginPage.Authentication.LookupBufferList.FirstOrDefault(x => x.type == "Organization Identification Type" && x.description.Trim().ToLower() == "company police code");
        //        Identification companycode = LoginPage.Authentication.IdentificationBufferList.FirstOrDefault(x => x.type == companycodeLookup.code && x.reference == LoginPage.Authentication.OrganizationBufferList.FirstOrDefault(y => y.type == CNETConstantes.COMPANY).code);
        //        Identification companyTIN = LoginPage.Authentication.IdentificationBufferList.FirstOrDefault(x => x.type == CNETConstantes.ORGANIZATION_IDENTIFICATION_TYPETIN && x.reference == LoginPage.Authentication.OrganizationBufferList.FirstOrDefault(y => y.type == CNETConstantes.COMPANY).code);
        //        if (companycode != null)
        //        {
        //            string imagename = "";
        //            List<PoliceReportGovernment> PoliceReportGovernmentList = new List<PoliceReportGovernment>();
        //            PoliceReportGovernmentList = listPolice.Select(x => new PoliceReportGovernment()
        //            {
        //                hotel = companycode.idNumber,
        //                branch = SelectdBranch == null ? "" : SelectdBranch.description,
        //                tin = companyTIN == null ? "" : companyTIN.idNumber,
        //                room = x.Room,
        //                guestName = x.Guest,
        //                gender = x.Gender,
        //                nationality = x.Nationality,
        //                doB = x.DOB,
        //                idType = x.ID_Description,
        //                idNumber = x.ID_Number,
        //                arrivalDate = x.ArrivalDate,
        //                departureDate = x.DepartureDate,
        //                purposeOfTravel = x.PurposeOfTravel,
        //                reportDate = ReportDate,
        //                company = GetRegistrationCompany(x.Reg_No),
        //                pax = GetRegistrationPax(x.Reg_No, ReportDate),
        //                registeredBy = GetRegistrationCheckInUser(x.Reg_No),
        //                image = GetGuestpasportimage(x.Guestcode, ref imagename),
        //                imagename = imagename,
        //                remark = "",
        //            }).ToList();

        //            if (PoliceReportAPIInterface.APICheck() && PoliceReportGovernmentList != null && PoliceReportGovernmentList.Count > 0)
        //                PoliceReportAPIInterface.APIPostRequest(PoliceReportGovernmentList);

        //          Progress_Reporter.Close_Progress();
        //        }

        //    }
        //    catch
        //    {

        //    }
        //}

        private string GetGuestpasportimage(string Guestcode, ref string imagename)
        {
            imagename = "";
            string Imagebase64String = null;
            //List<Attachment> GuestAttachmentList = UIProcessManager.GetAttachmentByReference(Guestcode);
            //if (GuestAttachmentList != null && GuestAttachmentList.Count > 0)
            //{
            //    Attachment GuestAttachment = GuestAttachmentList.FirstOrDefault(x => x.catagory == CNETConstantes.ATTACHMENT_CATAGORY_PASSPORT);
            //    if (GuestAttachment != null && !string.IsNullOrEmpty(GuestAttachment.url))
            //    {
            //        if (File.Exists(GuestAttachment.url))
            //        {
            //            imagename = Path.GetFileName(GuestAttachment.url);
            //            Imagebase64String = ImageToByteArray(GuestAttachment.url);
            //        }
            //    }
            //}
            return Imagebase64String;
        }
        private string ImageToByteArray(string Path)
        {
            string base64String = "";
            try
            {
                // Get a bitmap. The using statement ensures objects  
                // are automatically disposed from memory after use.  
                using (Bitmap bmp1 = new Bitmap(Path))
                {
                    ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);

                    // Create an Encoder object based on the GUID  
                    // for the Quality parameter category.  
                    System.Drawing.Imaging.Encoder myEncoder =
                        System.Drawing.Imaging.Encoder.Quality;

                    // Create an EncoderParameters object.  
                    // An EncoderParameters object has an array of EncoderParameter  
                    // objects. In this case, there is only one  
                    // EncoderParameter object in the array.  
                    EncoderParameters myEncoderParameters = new EncoderParameters(1);

                    // Save the bitmap as a JPG file with zero quality level compression.  
                    EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 20L);
                    myEncoderParameters.Param[0] = myEncoderParameter;
                    using (MemoryStream m = new MemoryStream())
                    {
                        bmp1.Save(m, jpgEncoder, myEncoderParameters);
                        byte[] imageBytes = m.ToArray();
                        // Convert byte[] to Base64 String
                        base64String = Convert.ToBase64String(imageBytes);
                    }
                }


            }
            catch
            {
                base64String = "";
            }
            return base64String;
        }
        private ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }

        private static System.Drawing.Image resizeImage(System.Drawing.Image imgToResize, Size size)
        {
            try
            {
                //Get the image current width  
                int sourceWidth = imgToResize.Width;
                //Get the image current height  
                int sourceHeight = imgToResize.Height;
                float nPercent = 0;
                float nPercentW = 0;
                float nPercentH = 0;
                //Calulate  width with new desired size  
                nPercentW = ((float)size.Width / (float)sourceWidth);
                //Calculate height with new desired size  
                nPercentH = ((float)size.Height / (float)sourceHeight);
                if (nPercentH < nPercentW)
                    nPercent = nPercentH;
                else
                    nPercent = nPercentW;
                //New Width  
                int destWidth = (int)(sourceWidth * nPercent);
                //New Height  
                int destHeight = (int)(sourceHeight * nPercent);
                Bitmap b = new Bitmap(destWidth, destHeight);
                Graphics g = Graphics.FromImage((System.Drawing.Image)b);
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                // Draw image with new width and height  
                g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
                g.Dispose();
                return (System.Drawing.Image)b;

            }
            catch
            {
                return imgToResize;

            }
        }


        private void ShowTrialBalance(DateTime Date)
        {
            printByPortrait = true;
            List<TrialBalanceReport> TrialBalance = UIProcessManager.GetTrialBalanceReport(Date, SelectedHotelcode);

            if (TrialBalance == null)
                return;


            gcReports.DataSource = TrialBalance;

            gvReports.Columns["Group"].GroupIndex = 0;

            gvReports.Columns["Balance"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            gvReports.Columns["Balance"].DisplayFormat.FormatString = "{0:N2}";

            //groupSumm = new GridGroupSummaryItem() { SummaryType = DevExpress.Data.SummaryItemType.Count, FieldName = "Group" };
            //gvReports.GroupSummary.Add(groupSumm);

            gvReports.Columns["Balance"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;

            groupSumm = new GridGroupSummaryItem(DevExpress.Data.SummaryItemType.Sum,
                "Balance", gvReports.Columns["Balance"], "etbTotal = {0:N}");
            gvReports.GroupSummary.Add(groupSumm);

            item = new GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum,
                        "Balance", "etbTotal = {0:N}");
            gvReports.Columns["Balance"].Summary.Add(item);

            gvReports.ExpandAllGroups();
            gvReports.BestFitColumns();
            gcReports.RefreshDataSource();


        }

        private void ShowDailyResidentSummaryReport(DateTime date)
        {
            ResponseModel<List <DailyResidentSummaryReport>> response = UIProcessManager.GetDailyResidentSummary(date, SelectedHotelcode, LocalBuffer.LocalBuffer.CurrentLoggedInUser.Id);


            if (response == null || !response.Success)
            {
                XtraMessageBox.Show("Error " + response.Message);
                return;

            }

            if(!string.IsNullOrEmpty(response.Message))
                XtraMessageBox.Show("Error " + response.Message);


            // SaveAllDailyResidentSummary(response.Data, date);
            gcReports.DataSource = response.Data;
            gvReports.BestFitColumns();
            gvReports.Columns["Room"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;

            item = new GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Custom,
            "Room", "Total:  ");
            gvReports.Columns["Room"].Summary.Add(item);

            item = new GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum,
                         "RoomRevenue", "{0:N}");
            gvReports.Columns["RoomRevenue"].Summary.Add(item);
            gvReports.Columns["RoomRevenue"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;

            item = new GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum,
                        "Package", "{0:N}");
            gvReports.Columns["Package"].Summary.Add(item);
            gvReports.Columns["Package"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;

            item = new GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum,
                        "ServiceCharge", "{0:N}");
            gvReports.Columns["ServiceCharge"].Summary.Add(item);
            gvReports.Columns["ServiceCharge"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;

            item = new GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum,
                        "VAT", "{0:N}");
            gvReports.Columns["VAT"].Summary.Add(item);
            gvReports.Columns["VAT"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;

            item = new GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum,
                        "RoomTotal", "{0:N}");
            gvReports.Columns["RoomTotal"].Summary.Add(item);
            gvReports.Columns["RoomTotal"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;

            item = new GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum,
                        "POSCharge", "{0:N}");
            gvReports.Columns["POSCharge"].Summary.Add(item);
            gvReports.Columns["POSCharge"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;

            item = new GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum,
                        "TodayTotal", "{0:N}");
            gvReports.Columns["TodayTotal"].Summary.Add(item);
            gvReports.Columns["TodayTotal"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;

            item = new GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum,
                        "BBF", "{0:N}");
            gvReports.Columns["BBF"].Summary.Add(item);
            gvReports.Columns["BBF"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;

            item = new GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum,
                        "toDateTotal", "{0:N}");
            gvReports.Columns["toDateTotal"].Summary.Add(item);
            gvReports.Columns["toDateTotal"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;

            item = new GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum,
                        "Payment", "{0:N}");
            gvReports.Columns["Payment"].Summary.Add(item);
            gvReports.Columns["Payment"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;

            //item = new GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum,
            //            "toDatePayment", "{0:N}");
            //gvReports.Columns["toDatePayment"].Summary.Add(item);
            //gvReports.Columns["toDatePayment"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;

            item = new GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum,
                        "Discount", "{0:N}");
            gvReports.Columns["Discount"].Summary.Add(item);
            gvReports.Columns["Discount"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;

            item = new GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum,
                        "Paidout", "{0:N}");
            gvReports.Columns["Paidout"].Summary.Add(item);
            gvReports.Columns["Paidout"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;

            item = new GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum,
                        "BCF", "{0:N}");
            gvReports.Columns["BCF"].Summary.Add(item);
            gvReports.Columns["BCF"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;

            item = new GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum,
                        "Outstanding", "{0:N}");
            gvReports.Columns["Outstanding"].Summary.Add(item);
            gvReports.Columns["Outstanding"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;

            gvReports.Columns["Reg_No"].Width = 50;
            gvReports.Columns["Guest"].Width = 60;
            gvReports.Columns["Company"].Width = 50;
            gvReports.Columns["RoomRevenue"].Width = 60;
            gvReports.Columns["Package"].Width = 60;
            gvReports.Columns["ServiceCharge"].Width = 60;
            gvReports.Columns["VAT"].Width = 60;
            gvReports.Columns["TodayTotal"].Width = 60;
            gvReports.Columns["POSCharge"].Width = 60;
            gvReports.Columns["BBF"].Width = 60;
            gvReports.Columns["toDateTotal"].Width = 60;
            gvReports.Columns["CRVNo"].Width = 60;
            gvReports.Columns["Payment"].Width = 60;
            gvReports.Columns["Discount"].Width = 60;
            gvReports.Columns["Paidout"].Width = 60;
            gvReports.Columns["BCF"].Width = 60;
            gvReports.Columns["Outstanding"].Width = 60;
            gvReports.Columns["ConsigneeCode"].Visible = false;
            gvReports.Columns["Reference"].Visible = false;


            gvReports.Columns["CRVNo"].Caption = "CRV No.";
            gvReports.Columns["Reg_No"].AppearanceCell.Options.UseTextOptions = true;



            gcReports.RefreshDataSource();
            gvReports.RefreshData();

        }

        //house keeping reports
        #region Housekeeping Reports
        private void ShowDiscripancyOfTheDay(DateTime selectedDate)
        {
            try
            {
                List<DiscripancyReportDTO> DiscripancyReport = UIProcessManager.GetDiscripancyReport(selectedDate, SelectedHotelcode);



                gcReports.DataSource = DiscripancyReport;
                gvReports.BestFitColumns();
            }
            catch (Exception ex)
            {

            }
        }

        private void ShowStatusReportForToday(DateTime selectedDate)
        {
            try
            {

                printByPortrait = false;

                List<Tuple<string, string, decimal>> categoryPriceList = new List<Tuple<string, string, decimal>>();

                DataTable dataTable = new DataTable();
                dataTable.Columns.Add(new DataColumn() { ColumnName = "Col1", Caption = "SN" });
                dataTable.Columns.Add(new DataColumn() { ColumnName = "Col2", Caption = "Room" });
                dataTable.Columns.Add(new DataColumn() { ColumnName = "Col3", Caption = "Status" });
                dataTable.Columns.Add(new DataColumn() { ColumnName = "Col4", Caption = "SN" });
                dataTable.Columns.Add(new DataColumn() { ColumnName = "Col5", Caption = "Room" });
                dataTable.Columns.Add(new DataColumn() { ColumnName = "Col6", Caption = "Status" });
                dataTable.Columns.Add(new DataColumn() { ColumnName = "Col7", Caption = "SN" });
                dataTable.Columns.Add(new DataColumn() { ColumnName = "Col8", Caption = "Room" });
                dataTable.Columns.Add(new DataColumn() { ColumnName = "Col9", Caption = "Status" });
                dataTable.Columns.Add(new DataColumn() { ColumnName = "Col10", Caption = "SN" });
                dataTable.Columns.Add(new DataColumn() { ColumnName = "Col11", Caption = "Room" });
                dataTable.Columns.Add(new DataColumn() { ColumnName = "Col12", Caption = "Status" });
                dataTable.Columns.Add(new DataColumn() { ColumnName = "Col13", Caption = "SN" });
                dataTable.Columns.Add(new DataColumn() { ColumnName = "Col14", Caption = "Room" });
                dataTable.Columns.Add(new DataColumn() { ColumnName = "Col15", Caption = "Status" });
                dataTable.Columns.Add(new DataColumn() { ColumnName = "Col16", Caption = "SN" });
                dataTable.Columns.Add(new DataColumn() { ColumnName = "Col17", Caption = "Room" });
                dataTable.Columns.Add(new DataColumn() { ColumnName = "Col18", Caption = "Status" });

              //  List<HKStatusDetailReport> HKDetailList = new List<HKStatusDetailReport>();


                List<VwRoomManagmentViewDTO> allRooms = UIProcessManager.GetAllRoomManagment(SelectedHotelcode);
                if (allRooms == null || allRooms.Count == 0) return;


                //if (filterwithHotelBranch && FilterRoomDetailcodeList != null)
                //    allRooms = allRooms.Where(x => FilterRoomDetailcodeList.Contains(x.roomDetailCode)).ToList();
                //Counters
                int occupiedCount = 0, vacantCount = 0, cleanCount = 0, dirtyCount = 0, inspectedCount = 0, pickupCount = 0, oooCount = 0, oosCount = 0;

                //HK Status List
                List<RegistrationStatusDTO> statusList = UIProcessManager.GetRegistrationStatusList(allRooms.Select(r => r.roomDetailCode).ToList(), selectedDate);
              
                if (statusList == null) statusList = new List<RegistrationStatusDTO>(); 
             

                allRooms = allRooms.OrderBy(r => r.Floor).ThenBy(r => r.roomNo).ToList();
                int count = 0; 
                decimal rowCo = Math.Ceiling(Convert.ToDecimal(allRooms.Count) / 6);
                int rowCount = Convert.ToInt32(rowCo);
                foreach (var room in allRooms)
                {

                    count++;
                    //HKStatusDetailReport hkDetail = new HKStatusDetailReport();
                    //hkDetail.SN = count;
                    //hkDetail.Floor = room.Floor;
                    //hkDetail.RoomType = room.RoomType;
                    //hkDetail.RoomNumber = room.roomNo;

                    //Room Status
                    string roomStatus = "";
                    int roomStatusCode=0;
                    var actDef = LocalBuffer.LocalBuffer.ActivityDefinitionBufferList.FirstOrDefault(a => a.Id == room.roomStatusCode);
                    if (actDef != null)
                    {
                        var luk = LocalBuffer.LocalBuffer.LookUpBufferList.FirstOrDefault(l => l.Id == actDef.Description);
                        if (luk != null)
                        {
                            roomStatus = luk.Description;
                            roomStatusCode = luk.Id;
                        }

                        if (actDef.Description == CNETConstantes.CLEAN)
                        {
                            cleanCount++;
                        }
                        else if (actDef.Description == CNETConstantes.Dirty)
                        {
                            dirtyCount++;
                        }
                        else if (actDef.Description == CNETConstantes.PICKUP)
                        {
                            pickupCount++;
                        }
                        else if (actDef.Description == CNETConstantes.OOO)
                        {
                            oooCount++;
                        }
                        else if (actDef.Description == CNETConstantes.OOS)
                        {
                            oosCount++;
                        }
                        else if (actDef.Description == CNETConstantes.INSPECTED)
                        {
                            inspectedCount++;
                        }
                        else
                        {
                            oosCount++;
                        }
                    }
                   // hkDetail.RoomStatus = roomStatus;

                    //HK Status
                  RegistrationStatusDTO status = statusList.FirstOrDefault(s => s.RoomNumber == room.roomDetailCode);
                    int foStatus = 0;
                    if (status != null)
                    {
                        if (status.FOStatus == "0")
                        {
                          //  hkDetail.HKStatus = "Vacant";
                            vacantCount++;
                        }
                        else if (status.FOStatus == "1")
                        {
                            foStatus = 1;
                           // hkDetail.HKStatus = "Occupied";
                            occupiedCount++;
                        }
                    }
                    else
                    {
                       // hkDetail.HKStatus = "Vacant";
                        vacantCount++;
                    }


                   // HKDetailList.Add(hkDetail);


                    if (count <= rowCount)
                    {
                        DataRow row = dataTable.NewRow();
                        row[dataTable.Columns["Col1"]] = count;
                        row[dataTable.Columns["Col2"]] = room.roomNo;
                        row[dataTable.Columns["Col3"]] = GetShortFormatOfRoomStatus(roomStatusCode, foStatus);

                        dataTable.Rows.Add(row);

                    }
                    else if (count > rowCount && count <= 2 * rowCount)
                    {
                        int index = (count - 1) % rowCount;
                        DataRow row = dataTable.Rows[index];
                        row[dataTable.Columns["Col4"]] = count;
                        row[dataTable.Columns["Col5"]] = room.roomNo;
                        row[dataTable.Columns["Col6"]] = GetShortFormatOfRoomStatus(roomStatusCode, foStatus);
                    }
                    else if (count > rowCount && count <= 3 * rowCount)
                    {
                        int index = (count - 1) % rowCount;
                        DataRow row = dataTable.Rows[index];
                        row[dataTable.Columns["Col7"]] = count;
                        row[dataTable.Columns["Col8"]] = room.roomNo;
                        row[dataTable.Columns["Col9"]] = GetShortFormatOfRoomStatus(roomStatusCode, foStatus);
                    }
                    else if (count > rowCount && count <= 4 * rowCount)
                    {
                        int index = (count - 1) % rowCount;
                        DataRow row = dataTable.Rows[index];
                        row[dataTable.Columns["Col10"]] = count;
                        row[dataTable.Columns["Col11"]] = room.roomNo;
                        row[dataTable.Columns["Col12"]] = GetShortFormatOfRoomStatus(roomStatusCode, foStatus);
                    }
                    else if (count > rowCount && count <= 5 * rowCount)
                    {
                        int index = (count - 1) % rowCount;
                        DataRow row = dataTable.Rows[index];
                        row[dataTable.Columns["Col13"]] = count;
                        row[dataTable.Columns["Col14"]] = room.roomNo;
                        row[dataTable.Columns["Col15"]] = GetShortFormatOfRoomStatus(roomStatusCode, foStatus);
                    }
                    else
                    {
                        int index = (count - 1) % rowCount;
                        DataRow row = dataTable.Rows[index];
                        row[dataTable.Columns["Col16"]] = count;
                        row[dataTable.Columns["Col17"]] = room.roomNo;
                        row[dataTable.Columns["Col18"]] = GetShortFormatOfRoomStatus(roomStatusCode, foStatus);
                    }



                }



                gcReports.DataSource = dataTable;
                gvReports.BestFitColumns();

                int SNWidth = 25;
                gvReports.Columns["Col1"].Width = SNWidth;
                gvReports.Columns["Col4"].Width = SNWidth;
                gvReports.Columns["Col7"].Width = SNWidth;
                gvReports.Columns["Col10"].Width = SNWidth;
                gvReports.Columns["Col13"].Width = SNWidth;
                gvReports.Columns["Col16"].Width = SNWidth;

                gvReports.Columns["Col2"].AppearanceCell.TextOptions.HAlignment = HorzAlignment.Center;
                gvReports.Columns["Col5"].AppearanceCell.TextOptions.HAlignment = HorzAlignment.Center;
                gvReports.Columns["Col8"].AppearanceCell.TextOptions.HAlignment = HorzAlignment.Center;
                gvReports.Columns["Col11"].AppearanceCell.TextOptions.HAlignment = HorzAlignment.Center;
                gvReports.Columns["Col14"].AppearanceCell.TextOptions.HAlignment = HorzAlignment.Center;
                gvReports.Columns["Col17"].AppearanceCell.TextOptions.HAlignment = HorzAlignment.Center;

                gvReports.Columns["Col2"].AppearanceHeader.TextOptions.HAlignment = HorzAlignment.Center;
                gvReports.Columns["Col5"].AppearanceHeader.TextOptions.HAlignment = HorzAlignment.Center;
                gvReports.Columns["Col8"].AppearanceCell.TextOptions.HAlignment = HorzAlignment.Center;
                gvReports.Columns["Col11"].AppearanceCell.TextOptions.HAlignment = HorzAlignment.Center;
                gvReports.Columns["Col14"].AppearanceCell.TextOptions.HAlignment = HorzAlignment.Center;
                gvReports.Columns["Col17"].AppearanceHeader.TextOptions.HAlignment = HorzAlignment.Center;

                gvReports.Columns["Col1"].AppearanceCell.BackColor = ColorTranslator.FromHtml("InactiveCaption");
                gvReports.Columns["Col4"].AppearanceCell.BackColor = ColorTranslator.FromHtml("InactiveCaption");
                gvReports.Columns["Col7"].AppearanceCell.BackColor = ColorTranslator.FromHtml("InactiveCaption");
                gvReports.Columns["Col10"].AppearanceCell.BackColor = ColorTranslator.FromHtml("InactiveCaption");
                gvReports.Columns["Col13"].AppearanceCell.BackColor = ColorTranslator.FromHtml("InactiveCaption");
                gvReports.Columns["Col16"].AppearanceCell.BackColor = ColorTranslator.FromHtml("InactiveCaption");
                //gvReports.Columns["Col5"].AppearanceCell.BackColor = ColorTranslator.FromHtml("InactiveCaption");
                //gvReports.Columns["Col6"].AppearanceCell.BackColor = ColorTranslator.FromHtml("InactiveCaption");

                //gvReports.ExpandMasterRow(0);
                //gvReports.ExpandMasterRow(1);
            }
            catch (Exception ex)
            {

            }
        }

        private string GetShortFormatOfRoomStatus(int roomStatus, int foStatus)
        {
            StringBuilder sb = new StringBuilder();
            if (foStatus == 0)
            {
                //vacant
                sb.Append("V");

            }
            else
            {
                //occupied
                sb.Append("O");
            }

            switch (roomStatus)
            {
                case CNETConstantes.CLEAN:
                    sb.Append("C");
                    break;
                case CNETConstantes.Dirty:
                    sb.Append("D");
                    break;
                case CNETConstantes.PICKUP:
                    sb.Append("P");
                    break;
                case CNETConstantes.OOO:
                    sb.Append("OO");
                    break;
                case CNETConstantes.OOS:
                    sb.Append("OS");
                    break;
                case CNETConstantes.INSPECTED:
                    sb.Append("I");
                    break;

            }

            return sb.ToString();
        }

        private void ShowTodaysTaskAssignments(DateTime selectedDate)
        {
            try
            {
                List<HKTaskAssignmentReportDTO> HKTaskAssignmentReport = UIProcessManager.GetTaskAssignmentReport(selectedDate, SelectedHotelcode);

                gcReports.DataSource = HKTaskAssignmentReport;
                gvReports.BestFitColumns();
            }
            catch (Exception ex) { }
        }

        private void ShowHKRoomStatus(DateTime selectedDate)
        {
            try
            {
                RoomStatus RoomStatusReport = new RoomStatus(selectedDate, SelectedHotelcode);
                RoomStatusReport.ShowDialog();
                
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ShowHKActivity(DateTime selectedDate)
        {
            try
            {
                const string HK_ACTIVITY = "HK_ACTIVITY";
                const string HK_ACTIVITY_DISC = "HK_ACTIVITY_DISC";
                List<HKActivityReportDTO> HKActivityReport = UIProcessManager.GetHouseKeepingActivityReport(selectedDate, SelectedHotelcode);


                gcReports.DataSource = HKActivityReport;
                gvReports.BestFitColumns();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        #endregion

        #endregion

        private void gcReports_Click(object sender, EventArgs e)
        {

        }

        private void gvReports_CustomSummaryCalculate(object sender, CustomSummaryEventArgs e)
        {
            if (selectedReportName == CNETConstantes.DAILY_BUSINESS_REPORT)
            {
                //    Double FullRooms = Convert.ToDouble(detailsView3.Columns["Rooms"].SummaryItem.SummaryValue);
                //    Double OccupiedRooms = Convert.ToDouble(detailsView3.Columns["Occupied"].SummaryItem.SummaryValue);
                //    string number = ((OccupiedRooms / FullRooms) * 100).ToString();

                //    detailsView3.Columns["Occupancy"].SummaryItem.DisplayFormat = " {0:N2}%";
                //    detailsView3.Columns["Occupancy"].SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Custom;
                //    detailsView3.Columns["Occupancy"].SummaryItem.Tag = number;
                //    detailsView3.Columns["Occupancy"].SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Average;
                //    detailsView3.Columns["Occupancy"].SummaryItem.DisplayFormat = " {0:N2}%";
                //}


                double _sumOfValues = 0;
                double _sumOfTotalValue = 0;
                try
                {
                    GridView View = sender as GridView;
                    // Initialization 
                    if (e.SummaryProcess == CustomSummaryProcess.Start)
                    {
                        _sumOfValues = 0;
                        _sumOfTotalValue = 0;
                    }

                    //Calculate
                    if (e.SummaryProcess == CustomSummaryProcess.Calculate)
                    {
                        double colValueColumnValue = Convert.ToDouble(View.GetRowCellValue(e.RowHandle, "Occupied"));
                        double colTotalValueColumnValue = Convert.ToDouble(View.GetRowCellValue(e.RowHandle, "Rooms"));
                        _sumOfValues += colValueColumnValue;
                        _sumOfTotalValue += colTotalValueColumnValue;
                    }

                    // Finalization 
                    if (e.SummaryProcess == CustomSummaryProcess.Finalize)
                    {
                        e.TotalValue = 0;
                        if (_sumOfTotalValue != 0)
                        {
                            e.TotalValue = (_sumOfValues / _sumOfTotalValue) * 100;
                        }


                    }
                }
                catch (System.Exception ex)
                {
                }
            }

        }

        public void DashboardRefresh()
        {
            if (dash.InvokeRequired)
            {
                dash.Invoke(new MethodInvoker(delegate { dash.btnRefresh.PerformClick(); }));
            }

        }

        public bool ExporteDashBoardReportToPdf(string path, string title, DateTime date, string defaultPrinterName = "")
        {
            bool flag = true;
            try
            {
                //if (dash.InvokeRequired)
                //{
                //    dash.Invoke(new MethodInvoker(delegate { dash.CreateDashboardReport(path); }));
                //}

                //if (EnableFTPSync)
                //    UIProcessManager.UploadFTP(FTPServerHost, FTPServerUserName, FTPServerPassword, FTPServerFolder, path);

                return flag;
            }
            catch (Exception ex)
            {

                return false;
            }

            return flag;
        }

        private void beiHotel_EditValueChanged(object sender, EventArgs e)
        {

            SelectedHotelcode = beiHotel.EditValue == null ? null : Convert.ToInt32(beiHotel.EditValue);

            if (SelectedHotelcode != null && SelectedHotelcode != -1)
            {
                ConsigneeUnitDTO branch = LocalBuffer.LocalBuffer.HotelBranchBufferList.FirstOrDefault(x => x.Id == SelectedHotelcode);
                if (branch != null)
                    SelectedHotelDescription = branch.Name;

            }
            else
            {
                SelectedHotelDescription = "";
                SelectedHotelcode = null;
            }
        }

        public int? SelectedHotelcode { get; set; }
        public string SelectedHotelDescription { get; set; }

        #endregion
    }
}
