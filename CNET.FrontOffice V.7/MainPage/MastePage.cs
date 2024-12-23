using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Xml;
using CNET.ERP.Client.Common.UI.Library.Navigator;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraTabbedMdi;
using System.Collections.Generic;
using System.Diagnostics;
using DevExpress.XtraBars;
using DevExpress.XtraRichEdit.Model.History;
using DevExpress.XtraTab;
using System.IO;
using CNET.ERP.Client.Common.UI;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Globalization;
using DevExpress.Utils.Drawing;
using CNET.FrontOffice_V._7.PMS.Contracts;
using CNET_V7_Domain.Domain.ConsigneeSchema;
using CNET.ERP.Client.UI_Logic.PMS.Enum;
using CNET.Progress.Reporter;
using CNET_V7_Domain.Domain.SettingSchema;
using ProcessManager;
using CNET.FrontOffice_V._7.Forms.Non_Navigatable_Modals;
using CNET.POS.FiscalPrinter;
using CNET.POS.Settings;
using CNET.FP.Tool;

namespace CNET.FrontOffice_V._7
{

    internal struct LASTINPUTINFO
    {
        public uint cbSize;
        public uint dwTime;
    }
    public partial class MasterPageForm : XtraForm, ICNETNavigatable
    {

        public delegate void SelectedDashboardTask(string filter);
        public delegate void DashboardFilterRefresh(string filter);
        private CNETPageNavigator navigator;
        //  public static List<PMSSecurity> AllowedPmsSecurity = new List<PMSSecurity>();
        private bool _isFromDashboard = false;
        private string filterKey = null;
        private static frmDisplay _frmDisplay;
        public static HomePage home;
        // private static RegistrationList registrationForm = null;
        private static MasterPageForm globalHome = null;
        private static List<XtraForm> openedForms = new List<XtraForm>();
        private static List<String> openedFormsTabbed = new List<String>();

        public SelectedDashboardTask MySelectedDashboardTask { get; set; }
        public static DashboardFilterRefresh DashboardRefresher { get; set; }

        #region Buffer List
        //public static List<Message_View> LogBufferList = null;
        //public static List<ServiceRequestDTO> ServiceRequestBufferList = new List<ServiceRequestDTO>();
        //public static List<vw_VoucherConsignee> AllGuestVoucherConsignee = new List<vw_VoucherConsignee>();
        //public static List<vw_VoucherConsignee> AllCompanyVoucherConsignee = new List<vw_VoucherConsignee>();
        //public static List<OrganizationUnitDefinition> HotelBranchBufferList = new List<OrganizationUnitDefinition>();
        // public SelectedMessage SelectedMessage { get; set; }
        // public static List<Language> languageList { get; set; }

        //  public SelectedDashboardTask MySelectedDashboardTask { get; set; }
        //  public static DashboardFilterRefresh DashboardRefresher { get; set; }
        #endregion

        //******************************* CONSTRUCTOR ****************************//
        public MasterPageForm()
        {
            //PMSSecurityCheck();
            InitializeComponent();

            //attach the deligate with the event handler
            MySelectedDashboardTask = SelectedDashboardTaskHandler;
            //  SelectedMessage = SelectedMessageHandler;

            CreateDisplayForm();
            CreateHome();
            CreateNavigator();
            //LoadLogBuffer();

            //LoadVoucherConsigneeBuffer();

            //LoadServiceRequestBuffer();
            //GetLanguageData();

            //HotelBranchBufferList = GetHotelBranchList();
            //CurrentDeviceOrganizationUnit = GetCurrentDeviceOrganizationUnit();

            //POSSettingCache.CurrentUser = LocalBuffer.LocalBuffer.GetAuthorizedUser();
            //POSSettingCache.CurrentUserName = LocalBuffer.LocalBuffer.GetAuthorizedUser().userName;
            //POSSettingCache.PosMachineid = LocalBuffer.LocalBuffer.CurrentDevice.code;
            //UserHasHotelBranchAccess = CheckIfTheUserHasHotelBranchAccess();
            //if (!UserHasHotelBranchAccess)
            //{
            //    HotelBranchBufferList = HotelBranchBufferList.Where(x => x.code == CurrentDeviceOrganizationUnit).ToList();
            //}

            //CheckEarlyCheckinSetting();
        }

        //public List<OrganizationUnitDefinition> GetHotelBranchList()
        //{
        //    try
        //    {
        //        string companyCode = "";
        //        var com = LocalBuffer.LocalBuffer.OrganizationBufferList.Where(x => x.type == CNETConstantes.COMPANY);
        //        if (com != null && com.Count() > 0)
        //        {
        //            companyCode = com.FirstOrDefault().code;
        //        }
        //        List<OrganizationUnitDefinition> orgs = LocalBuffer.LocalBuffer.OrganizationUnitDefinitionBufferList.Where(b => b.type == CNETConstantes.ORG_UNIT_TYPE_BRUNCH && b.specialization == CNETConstantes.ORG_UNIT_TYPE_HOTEL).ToList();
        //        List<OrganizationUnit> orgUnits = LocalBuffer.LocalBuffer.OrganizationUnitBufferList.Where(x => x.reference == companyCode).ToList();

        //        List<OrganizationUnitDefinition> branchesOnly = (from org in orgs
        //                                                         join ou in orgUnits on org.code equals ou.organizationUnitDefinition
        //                                                         select org).ToList();
        //        Identification CompanyIden = LocalBuffer.LocalBuffer.IdentificationBufferList.FirstOrDefault(x => x.reference == companyCode && x.type == CNETConstantes.ORGANIZATION_IDENTIFICATION_TYPETIN);
        //        POSSettingCache.Comapny_TIN = CompanyIden != null ? CompanyIden.idNumber : "";
        //        return branchesOnly;

        //    }
        //    catch { return null; }
        //}
        //public string GetCurrentDeviceOrganizationUnit()
        //{
        //    string OrganizationUnitcode = "";
        //    OrganizationUnit deviceorg = LocalBuffer.LocalBuffer.OrganizationUnitBufferList.FirstOrDefault(x => x.reference == LocalBuffer.LocalBuffer.CurrentDevice.code);
        //    if (deviceorg != null && !string.IsNullOrEmpty(deviceorg.organizationUnitDefinition))
        //    {
        //        OrganizationUnitcode = deviceorg.organizationUnitDefinition;
        //        POSSettingCache.Pos_OUD = OrganizationUnitcode;
        //        // SelectedHotelcode = deviceorg.organizationUnitDefinition;
        //    }
        //    return OrganizationUnitcode;
        //}
        //private bool CheckIfTheUserHasHotelBranchAccess()
        //{
        //    List<viewFunctWithAccessM> retVal = new List<viewFunctWithAccessM>();

        //    try
        //    {
        //        String SubSystemComponent = CNETConstantes.SECURITYRegistrationDocument;
        //        string currentRole = "";
        //        var role = LocalBuffer.LocalBuffer.UserRoleMapperBufferList.Where(x => x.user == LocalBuffer.LocalBuffer.CurrentLoggedInUser.Id).FirstOrDefault();
        //        if (role != null)
        //            currentRole = role.role;

        //        retVal.AddRange(UIProcessManager.GetFuncwithAccessMatView(currentRole, "Access", SubSystemComponent).Where(x => x.access == true).ToList());
        //        if (retVal != null && retVal.FirstOrDefault(x => x.description == "Branch Hotel Access") != null)
        //        {
        //            return true;
        //        }

        //    }
        //    catch (Exception ex)
        //    {

        //        return false;
        //    }
        //    return false;
        //}


        #region Helper Method

        public static void PauseVideo()
        {
            if (_frmDisplay != null)
            {
                var player = _frmDisplay.GetWindowsMediaPlayer();
                if (player.playState == WMPLib.WMPPlayState.wmppsPlaying)
                    player.Ctlcontrols.pause();
            }
        }

        public static void PlayVideo()
        {
            if (_frmDisplay != null)
            {
                var player = _frmDisplay.GetWindowsMediaPlayer();
                if (player.playState == WMPLib.WMPPlayState.wmppsPaused)
                    player.Ctlcontrols.play();
            }
        }

        public static List<String> AllowedFunctionalities(String parent)
        {
            return null;// AllowedPmsSecurity.Where(x => x.Parent == parent).ToList().Select(x => x.Functionality).ToList();
        }


        public void SelectedDashboardTaskHandler(string filterkey)
        {
            this.filterKey = filterkey;
            _isFromDashboard = true;
            ShowForm(@"Main Menu//Registration Document_Registration Document", 425);
        }

        public void SelectedMessageHandler(/*Message_View selectedMessage*/)
        {
            /*  frmMessaging frmMessaging = new frmMessaging();

              frmMessaging.ForwardedMessage = selectedMessage;
              frmMessaging.IsFromMsgBrowser = true;

              frmMessaging.Show();*/
        }

        private void CreateDisplayForm()
        {
            try
            {

                //read device configuration
                DeviceDTO sigDevice = UIProcessManager.GetDeviceByhostandpreference(LocalBuffer.LocalBuffer.CurrentDevice.Id, CNETConstantes.DEVICE_SIGNATURE);
                if (sigDevice == null) return;


                bool flag = ScribbleWinTab.TabInfo.IsTabAvailable();
                if (!flag) return;
                //617, 476
                var screenList = Screen.AllScreens;

                //foreach (Screen s in screenList)
                //{
                //    MessageBox.Show(s.DeviceName+" "+ s.Primary+" "+s.WorkingArea );
                //}

                //int count = screenList.Length;
                //Screen winTab = screenList[count - 1];

                Screen winTab = screenList.FirstOrDefault(x => !x.Primary);
                if (winTab == null) return;

                var configUrl = LocalBuffer.LocalBuffer.ConfigurationBufferList.FirstOrDefault(c => c.Reference == sigDevice.Id.ToString() && c.Attribute.ToLower() == "url");
                if (configUrl == null || string.IsNullOrEmpty(configUrl.CurrentValue)) return;

                if (_frmDisplay == null)
                    _frmDisplay = new frmDisplay(winTab, configUrl.CurrentValue);

                _frmDisplay.Show();
                _frmDisplay.Refresh();
            }
            catch (Exception ex) { }
        }

        public void CreateHome()
        {
            home = new HomePage();
            //home.SelectedDashboardTaskHandler = MySelectedDashboardTask;
            //home.SelectedMessageHandler = SelectedMessage;
            home.MdiParent = this;
            home.picCNETlogo.DoubleClick += picCNETlogo_DoubleClick;
            home.SelectedDashboardTaskHandler = MySelectedDashboardTask;
            TabManager.MdiParent = this;
            TabManager.Pages[home].ShowPinButton = DefaultBoolean.True;
            TabManager.Pages[home].Pinned = true;
            TabManager.Pages[home].AllowPin = true;
            TabManager.Pages[home].ShowCloseButton = DefaultBoolean.False;
            TabManager.Pages[home].MdiChild.Activate();

            home.Show();

            // LocalBuffer.LocalBuffer.mainlogin.Hide();
        }

        private void BtnUpdateApplication_Click(object? sender, EventArgs e)
        {
            var p = new Process();
            p.StartInfo.FileName = "ERPUpdaterV1.1.exe";  // just for example, you can use yours.
            p.Start();
            this.Close();
        }

        private void picCNETlogo_DoubleClick(object sender, EventArgs e)
        {
            Progress_Reporter.Show_Progress("Getting Local System Buffer", "Please Wait........");
            LocalBuffer.LocalBuffer.RefreshLocalBuffer();
            Progress_Reporter.Close_Progress();
        }

        public void CreateNavigator()
        {
            NavigatorSettingManager navSettingMgr = new NavigatorSettingManager();
            navSettingMgr.GenerateNavigationXml();
            navigator = new CNETPageNavigator(this, this);
            navigator.ShowCheckbox(false);
            navigator.DTO = navSettingMgr.DTO;
            navigator.ShowNavigator();
            Progress_Reporter.Close_Progress();
            CNETPageNavigator.LogOutClicked += CNETPageNavigator_LogOutClicked;
        }

        private void ShowLogicBase(UILogicBase logicBase, IWin32Window requester = null, XtraForm fisicalPrinter = null)
        {
            if (logicBase == null && fisicalPrinter == null)
            {
                return;
            }


            if (fisicalPrinter != null)
            {
                fisicalPrinter.FormClosed += fisicalPrinter_FormClosed;
                fisicalPrinter.MdiParent = this;// GetGlobalHome();

                string formFullName = fisicalPrinter.Tag as string;
                if (string.IsNullOrEmpty(formFullName)) return;
                openedFormsTabbed.Add(formFullName);
                fisicalPrinter.Show();
                return;
            }

            XtraForm form = (XtraForm)logicBase;

            // form.SuspendLayout();
            //form.Load += form_Shown;
            form.FormClosed += form_FormClosed;
            logicBase.SubForm = form;
            form.Tag = logicBase;
            try
            {
                if (FormSetting.FormIsTabbed(logicBase.FormFullName))
                {
                    String[] nameArray = logicBase.FormFullName.Split('_');
                    String name = nameArray[nameArray.Length - 1];
                    String lowerName = name.ToLower();
                    form.MdiParent = this;// GetGlobalHome();
                    openedFormsTabbed.Add(logicBase.FormFullName);
                    form.Show();

                }
                else
                {
                    form.Load += form_Load;
                    form.Visible = false;
                    form.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
                    openedForms.Add(form);

                    if (requester != null)
                    {
                        form.Owner = (XtraForm)requester;
                    }
                    form.ShowDialog();

                }

            }
            catch (Exception e)
            {
                HandlePartException(e, ExceptionHandling.SHOWMESSAGEANDLOG, "Loading or Showing form Methods");
            }
        }

        private static void SaveResultProcess(UILogicBase logic, SaveClickedResult result)
        {
            System.Action ShowMessage = () =>
            {

                if (result.MessageType == MessageType.MESSAGEBOX)
                {
                    ShowModalInfoMessage("Saved Successfully.", "MESSAGE");
                }
                if (result.MessageType == MessageType.ALLERT)
                {
                    ShowAlertInformation("Saved Successfully.", "MESSAGE", logic);
                }
            };

            if (result != null)
            {
                switch (result.SaveResult)
                {
                    case SaveResult.SAVE_SUCESSESFULLY:
                        ShowMessage();
                        break;
                    case SaveResult.SAVE_THENRESET:
                        ShowMessage();
                        logic.Reset();
                        break;
                    case SaveResult.SAVE_THENSHOWNOTHINGREESET:
                        logic.Reset();
                        break;
                }
            }
        }

        public static MasterPageForm GetGlobalHome()
        {
            if (globalHome == null)
            {
                globalHome = new MasterPageForm();
            }
            return globalHome;
        }

        public void ShowForm(string formFullName, int imageIndex)
        {
            if (ActivatePage(formFullName))
            {
                if (!_isFromDashboard)
                {
                    return;

                }
                else
                {
                    _isFromDashboard = false;
                    if (DashboardRefresher != null)
                    {
                        DashboardRefresher.Invoke(filterKey);
                        return;
                    }
                }
            }
            if (FormSetting.FormIsTabbed(formFullName))
            {
                if ((TabManager.TabPageWidth * (TabManager.Pages.Count + 2)) > TabManager.Bounds.Width)
                {
                    acHome.Show(this, "Information",
                        "Cloud ERP V7 can't open any more windows , Please close Already opened windows");

                    return;
                }
            }


            if (formFullName.ToLower() == "main menu//fiscal printer_fiscal printer")
            {
                try
                {
                    //if (Authentication.DeviceObject.preference != CNETConstantes.PMSPREF)
                    //{
                    //    XtraMessageBox.Show("Please, register this device to PMS POS", "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //    return;
                    //}

                    POS_Settings.System_Constants_Buffer = LocalBuffer.LocalBuffer.SystemConstantDTOBufferList;
                    POS_Settings.Tax_Buffer = LocalBuffer.LocalBuffer.TaxBufferList;
                    POS_Settings.User_Buffer = LocalBuffer.LocalBuffer.UserBufferList;
                    POS_Settings.LogedIn_User = LocalBuffer.LocalBuffer.CurrentLoggedInUser;
                    POS_Settings.Machine_Preference = LocalBuffer.LocalBuffer.CurrentDevice.Preference.Value;
                    FiscalPrinters.Instance = null;
                    POS_Settings.IsError = true;
                    POS_Settings.Machine_Consginee_Unit = LocalBuffer.LocalBuffer.CurrentDeviceConsigneeUnit.Value;
                    POS_Settings.Machine_ID = LocalBuffer.LocalBuffer.CurrentDevice.Id;
                    POS_Settings.Get_POS_Settings(LocalBuffer.LocalBuffer.ConfigurationBufferList);
                    FiscalPrinters.GetInstance();


                    //setup fisical printer
                    Progress_Reporter.Show_Progress("Connecting to fisical printer", "Please Wait.....");

                    if (!POS_Settings.IsError)
                    {
                        Progress_Reporter.Close_Progress();
                        XtraMessageBox.Show("Printer is Out Of Order", "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    FiscalPrinter fiscalPrinter = new FiscalPrinter();

                    if (!POS_Settings.IsError)
                    {
                        Progress_Reporter.Close_Progress();
                        XtraMessageBox.Show("Printer is Out Of Order", "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    Progress_Reporter.Close_Progress();


                    fiscalPrinter.Tag = formFullName;
                    IWin32Window requester = null;
                    if (TabManager.SelectedPage != null)
                    {
                        requester = TabManager.SelectedPage.MdiChild;
                    }
                    ShowLogicBase(null, requester, fiscalPrinter);
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show("Exception has occured! Detail: " + ex.Message +Environment.NewLine + ex.InnerException != null ? ex.InnerException.Message : "" , "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Error); 
                    return;
                }
            }
            else
            {

                UILogicBase logicBase = null;
                logicBase = GetLogicBase(formFullName);

                if (logicBase == null)
                {
                    // ShowModalInfoMessage("No Form is Created for the selected Menu", "MESSAGE");
                }
                IWin32Window requester = null;
                if (TabManager.SelectedPage != null)
                {
                    requester = TabManager.SelectedPage.MdiChild;
                }
                ShowLogicBase(logicBase, requester);
            }
        }

        public static void ShowModalInfoMessage(string message, string type, String caption = null)
        {
            String _Caption = String.Empty;

            if (type == "ERROR")
            {
                if (String.IsNullOrEmpty(caption)) _Caption = "Error";
                else _Caption = caption;

                XtraMessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            if (type == "MESSAGE")
            {

                if (String.IsNullOrEmpty(caption)) _Caption = "Information"; else _Caption = caption;

                XtraMessageBox.Show(message, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private static void InitializeLogicUI(UILogicBase logicBase)
        {
            try
            {
                (logicBase as ILogicHelper).InitializeUI();
            }
            catch (Exception e)
            {
                HandlePartException(e, ExceptionHandling.SHOWMESSAGEANDLOG, "Error While Initializing UI");
            }
        }

        private static void InitializeLogicBase(UILogicBase logicBase)
        {
            if (logicBase is ILogicHelper)
            {
                InitializeLogicUI(logicBase);

                // Utility.AdjustForm((XtraForm)(logicBase.SubForm));

                Progress_Reporter.Show_Progress("Initializing Data", "Please Wait ........");

                try
                {
                    (logicBase as ILogicHelper).InitializeData();
                }
                catch (Exception e)
                {
                    HandlePartException(e, ExceptionHandling.SHOWMESSAGEANDLOG, "Error While Initializing Data");
                }

                try
                {
                    if (logicBase.LoadEventArg != null)
                    {
                    }
                }
                catch (Exception e)
                {
                    HandlePartException(e, ExceptionHandling.SHOWMESSAGEANDLOG, "Error While Loading Data");
                }
            }

            Progress_Reporter.Show_Progress("Initializing Form Operations", "Please Wait ........");
            BindCanMethods(logicBase);
            Progress_Reporter.Close_Progress();
        }

        private static void BindCanMethods(UILogicBase logicBase)
        {
            try
            {
                if (logicBase == null)
                {
                    return;
                }
                if (logicBase.CNETFooterRibbon == null)
                {
                    return;
                }
                if (logicBase is ICanSave)
                {
                    if (logicBase.CNETFooterRibbon.ribbonControl != null)
                    {
                        foreach (var item in logicBase.CNETFooterRibbon.ribbonControl.Items)
                        {
                            if (item is BarButtonItem)
                            {
                                if ((item as BarButtonItem).Caption == "Save")
                                {
                                    (item as BarButtonItem).Tag = logicBase;
                                    (item as BarButtonItem).ItemClick += Save_ItemClick;
                                    break;
                                }
                            }
                        }
                    }
                }


                if (logicBase is ICanDelete)
                {
                    if (logicBase.CNETFooterRibbon.ribbonControl != null)
                    {
                        foreach (var item in logicBase.CNETFooterRibbon.ribbonControl.Items)
                        {
                            if (item is BarButtonItem)
                            {
                                if ((item as BarButtonItem).Caption == "Delete")
                                {
                                    (item as BarButtonItem).Tag = logicBase;
                                    (item as BarButtonItem).ItemClick += Delete_ItemClick;
                                    break;
                                }
                            }
                        }
                    }
                }

                if (logicBase is ICanCreate)
                {
                    if (logicBase.CNETFooterRibbon.ribbonControl != null)
                    {
                        foreach (var item in logicBase.CNETFooterRibbon.ribbonControl.Items)
                        {
                            if (item is BarButtonItem)
                            {
                                if ((item as BarButtonItem).Caption == "New")
                                {
                                    (item as BarButtonItem).Tag = logicBase;
                                    (item as BarButtonItem).ItemClick += New_ItemClick;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                HandlePartException(e, ExceptionHandling.SHOWMESSAGEANDLOG, "Error While Initializing UI");
            }
        }

        private static void DeleteResultProcess(UILogicBase logic, DeleteClickedResult result)
        {
            System.Action ShowMessage = () =>
            {

                if (result.MessageType == MessageType.MESSAGEBOX)
                {
                    ShowModalInfoMessage("Deleted Successfully.", "MESSAGE");
                }
                if (result.MessageType == MessageType.ALLERT)
                {
                    ShowAlertInformation("Deleted Successfully.", "MESSAGE", logic);
                }
            };


            if (result != null)
            {
                switch (result.DeleteResult)
                {
                    case DeleteResult.DELETE_SUCESSESFULLY:
                        ShowMessage();
                        break;
                    case DeleteResult.DELETE_THENRESET:
                        ShowMessage();
                        logic.Reset();
                        break;
                    case DeleteResult.DELETE_THENSHOWNOTHINGRESET:
                        logic.Reset();
                        break;
                }
            }
        }
        private static void ShowAlertInformation(string message, string messageType, UILogicBase logic)
        {
            logic.ShowAlertInformation(message, messageType);
        }

        private static String BuildExceptionDetailMessage(Exception ex)
        {

            String msg = String.Empty;

            msg = ex.Message + Environment.NewLine;

            try
            {
                StackTrace st = new StackTrace(ex, true);
                StackFrame frame = st.GetFrame(0);

                string fileName = frame.GetFileName();
                if (fileName != null)
                    msg += "File: " + fileName + Environment.NewLine;
                //Get the method name
                string methodName = frame.GetMethod().Name;
                if (methodName != null)
                    msg += "Method: " + methodName + Environment.NewLine;


                //Get the line number from the stack frame
                int line = frame.GetFileLineNumber();


                //Get the column number
                int col = frame.GetFileColumnNumber();

                if (line != 0 && col != 0)
                    msg += "Line No and Col: " + line.ToString() + " : " + col.ToString() + Environment.NewLine;

            }
            catch { };

            return msg;
        }
        private static void LogException(Exception e)
        {
        }
        private static bool CheckCNETValidation(UILogicBase logic)
        {
            /* if (logic is ICanCNETValidate)
             {
                 try
                 {
                     return (logic as ICanCNETValidate).IsFormValid();
                 }
                 catch (Exception e)
                 {
                     HandlePartException(e, ExceptionHandling.SHOWMESSAGEANDLOG, "Error Executing form Validation");
                 }
             }
            */
            return true;
        }

        #endregion

        #region Event Handlers

        private void CNETPageNavigator_LogOutClicked(object sender, EventArgs e)
        {
            timer1.Stop();
            this.Hide();
            // Authentication.mainlogin.Show();
            // Authentication.mainlogin.txtPassword.Text = "";
        }

        private void TabManager1_CustomDocumentSelectorSettings(object sender, DevExpress.XtraTabbedMdi.CustomDocumentSelectorSettingsEventArgs e)
        {
            e.Selector.Hide();
        }
        private void xtraTabbedMdiManager1_SelectedPageChanged(object sender, System.EventArgs e)
        {
            var _TamManager = sender as CNETTabbedMdiManager;
            if (_TamManager.SelectedPage == null)
            {
                return;
            }


            if (_TamManager.SelectedPage.MdiChild == home)
            {
                foreach (XtraForm form in openedForms)
                {
                    if (form.Owner == home)
                    {
                        form.SuspendLayout();
                        form.Visible = true;
                        form.ResumeLayout(false);
                    }
                    else
                    {
                        form.SuspendLayout();
                        form.Visible = false;
                        form.ResumeLayout(false);
                    }
                }
            }

            else
            {
                foreach (XtraForm form in openedForms)
                {
                    form.SuspendLayout();
                    form.Visible = false;
                    form.ResumeLayout(false);
                }
            }
        }

        private static void New_ItemClick(object sender, ItemClickEventArgs e)
        {
            var button = e.Item as BarButtonItem;

            var logic = button.Tag as UILogicBase;

            Progress_Reporter.Show_Progress("Running Pre Record Creation Tasks", "Please Wait ........");
            if (logic.BeforeCreating != null)
            {
                try
                {
                    logic.BeforeCreating.Invoke(new CNETFormEventArgs(logic));
                }
                catch (Exception ex)
                {
                    HandlePartException(ex, ExceptionHandling.SHOWMESSAGEANDLOG, "Error Executing Before Creating Functions");
                }
            }

            Progress_Reporter.Show_Progress("Creating the Current Record", "Please Wait ........");
            try
            {
                (logic as ICanCreate).OnCreate();
            }
            catch (Exception ex)
            {
                HandlePartException(ex, ExceptionHandling.SHOWMESSAGEANDLOG, "Error Executing Creating Functions");
            }

            Progress_Reporter.Show_Progress("Running Post Record Creation Tasks", "Please Wait ........");
            if (logic.AfterCreating != null)
            {
                try
                {
                    logic.AfterCreating.Invoke(new CNETFormEventArgs(logic));
                }
                catch (Exception ex)
                {
                    HandlePartException(ex, ExceptionHandling.SHOWMESSAGEANDLOG, "Error Executing After Creating record Functions");
                }
            }
            Progress_Reporter.Close_Progress();
        }
        private static void form_Shown(object sender, EventArgs e)
        {
            var form = (sender as XtraForm);
            var logicBase = (UILogicBase)(sender as XtraForm).Tag;

            InitializeLogicBase(logicBase);
            form.ResumeLayout();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            //  if (_frmDisplay != null)
            //      _frmDisplay.Close();
            //this.Close();
            //Application.Exit();
        }

        private void MasterPageForm_Load(object sender, EventArgs e)
        {

            timer1.Interval = 1000;
            timer1.Start();
            // navigator.txtSearch.Focus();
            this.Text = "Cloud ERP V7 - Property Managment System";

            try
            {
                // LocalBuffer.LocalBuffer.LoadLocalBuffer(); 
                this.Text = string.Format("{0} ({1})", this.Text, LocalBuffer.LocalBuffer.CompanyConsigneeData.FirstName);
            }
            catch
            {

            }
        }

        private void MasterPageForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_frmDisplay != null)
                _frmDisplay.Close();
            // Application.Exit();
        }

        private void MasterPageForm_Shown(object sender, EventArgs e)
        {
            //Progress_Reporter.Show_Progress("Loading Navigator Data");

            //navigator = new CNETPageNavigator(this, this);
            //navigator.SearchAndApplyIconsFromLib = false;
            //// CNETPageNavigator.SourceFilePath = //@"\\192.168.1.5\Resources\CNET V6 App Resources\UI\CNET_UI_DLLLibrary_PMS\XML\NavigationList - Edited.xml";
            //var xmlSource = Resources.NavigationList___Edited;
            //XmlDocument doc = new XmlDataDocument();
            //doc.LoadXml(xmlSource);
            //navigator.SourceDoc = doc;
            //navigator.defaultImageIndex = 324;

            //navigator.SearchAndApplyIconsFromLib = false;


            //navigator.ShowCheckbox(false);
            //navigator.SearchAndApplyIconsFromLib = true;
            //// navigator.SetIconResourceManager(IconRsource.ResourceManager);


            //navigator.ShowNavigator();
            ////  OpenTopPinnedForms();
            //Progress_Reporter.Close_Progress();
        }

        private static void form_Load(object sender, EventArgs e)
        {
            var form = (sender as XtraForm);

            var logicBase = (UILogicBase)(sender as XtraForm).Tag;



            //   InitializeLogicUI(logicBase);

            //  form.SuspendLayout();
        }

        private void fisicalPrinter_FormClosed(object sender, FormClosedEventArgs e)
        {
            var form = sender as XtraForm;
            if (form == null) return;

            string formFullName = form.Tag as string;
            if (string.IsNullOrEmpty(formFullName)) return;
            openedFormsTabbed.Remove(formFullName);
            form.Tag = null;
        }

        private static void Save_ItemClick(object sender, ItemClickEventArgs e)
        {
            var button = e.Item as BarButtonItem;

            var logic = button.Tag as UILogicBase;

            Progress_Reporter.Show_Progress("Running Pre Save Tasks", "Please Wait ........");
            if (logic.BeforeSaving != null)
            {
                try
                {
                    logic.BeforeSaving.Invoke(new CNETFormEventArgs(logic));
                }
                catch (Exception ex)
                {
                    HandlePartException(ex, ExceptionHandling.SHOWMESSAGEANDLOG, "Error Executing before Save Functions");
                }
            }
            if (!CheckCNETValidation(logic))
            {
                return;
            }
            Progress_Reporter.Show_Progress("Saving the Current Record", "Please Wait ........");
            try
            {
                var result = (logic as ICanSave).OnSave();
                Progress_Reporter.Close_Progress();

                SaveResultProcess(logic, result);
            }
            catch (Exception ex)
            {
                Progress_Reporter.Close_Progress();

                HandlePartException(ex, ExceptionHandling.SHOWMESSAGEANDLOG, "Error Executing Saving Functions");
            }


            Progress_Reporter.Show_Progress("Running Post Save Tasks", "Please Wait ........");
            if (logic.AfterSaving != null)
            {
                try
                {
                    logic.AfterSaving.Invoke(new CNETFormEventArgs(logic));
                }
                catch (Exception ex)
                {
                    HandlePartException(ex, ExceptionHandling.SHOWMESSAGEANDLOG, "Error Executing After Save Functions");
                }
            }
            Progress_Reporter.Close_Progress();
        }

        private static void Delete_ItemClick(object sender, ItemClickEventArgs e)
        {
            var button = e.Item as BarButtonItem;

            var logic = button.Tag as UILogicBase;

            Progress_Reporter.Show_Progress("Running Pre Delete Tasks", "Please Wait ........");
            if (logic.BeforeDeleting != null)
            {
                try
                {
                    logic.BeforeDeleting.Invoke(new CNETFormEventArgs(logic));
                }
                catch (Exception ex)
                {
                    HandlePartException(ex, ExceptionHandling.SHOWMESSAGEANDLOG, "Error Executing Before Delete Functions");
                }
            }
            Progress_Reporter.Show_Progress("Deleting the Current Record", "Please Wait ........");
            try
            {
                var result = (logic as ICanDelete).OnDelete();

                Progress_Reporter.Close_Progress();



                DeleteResultProcess(logic, result);
            }
            catch (Exception ex)
            {
                Progress_Reporter.Close_Progress();

                HandlePartException(ex, ExceptionHandling.SHOWMESSAGEANDLOG, "Error Executing Delete Functions");
            }


            Progress_Reporter.Show_Progress("Running After Delete Tasks", "Please Wait ........");
            if (logic.AfterDeleting != null)
            {
                try
                {
                    logic.AfterDeleting.Invoke(new CNETFormEventArgs(logic));
                }
                catch (Exception ex)
                {
                    HandlePartException(ex, ExceptionHandling.SHOWMESSAGEANDLOG, "Error Executing After Delete Functions");
                }
            }
            Progress_Reporter.Close_Progress();
        }

        public static uint GetIdleTime()
        {

            var lastUserAction = new LASTINPUTINFO();
            lastUserAction.cbSize = (uint)Marshal.SizeOf(lastUserAction);
            lastUserAction.cbSize = (uint)Marshal.SizeOf(lastUserAction);
            GetLastInputInfo(ref lastUserAction);
            return ((uint)Environment.TickCount - lastUserAction.dwTime);
        }

        private void form_FormClosed(object sender, FormClosedEventArgs e)
        {
            if ((sender as XtraForm).Owner == home)
            {
                openedForms.Remove(sender as XtraForm);
            }
            var logic = sender as UILogicBase;

            openedFormsTabbed.Remove(logic.FormFullName);
            logic.FormFullName = "";
        }

        public void RecentUpdated()
        {



        }

        public Boolean ActivatePage(String formFullName)
        {
            try
            {
                foreach (XtraMdiTabPage tab in TabManager.Pages)
                {
                    if (tab.MdiChild.Tag != null)
                    {
                        if (tab.MdiChild.Tag.GetType().BaseType == typeof(UILogicBase))
                        {
                            if (formFullName.Equals(((UILogicBase)tab.MdiChild.Tag).FormFullName))
                            {
                                //if(_isFromDashboard)
                                //    TabManager.Pages.Remove(tab);
                                TabManager.SelectedPage = tab;

                                return true;
                            }
                        }
                        else if (formFullName.ToLower() == "main menu//fiscal printer_fiscal printer" && tab.MdiChild.Tag == formFullName)
                        {
                            TabManager.SelectedPage = tab;
                            return true;

                        }
                    }
                }
            }
            catch
            {
                return false;
            }
            return false;
        }

        private UILogicBase GetLogicBase(string formFullName)
        {
            UILogicBase logicBase = null;
            try
            {
                Progress_Reporter.Show_Progress("Initializing Form Components", "Please Wait ........");
                Dispatcher.home = this; //GetGlobalHome();
                if (!_isFromDashboard)
                    filterKey = null;
                logicBase = Dispatcher.SelectLogicBase(formFullName, filterKey);
                filterKey = null;
                if (logicBase != null)
                {
                    logicBase.FormFullName = formFullName;
                    if (FormSetting.FormIsTabbed(logicBase.FormFullName))
                    {
                        if (formFullName == "Main Menu//Revenue Management_Revenue Management" ||
                            formFullName == "Main Menu//Property_Property")
                        {
                            logicBase.MdiParent = this;
                            logicBase.Show();
                        }
                        InitializeLogicBase(logicBase);
                    }
                }
                else
                {
                    if (Dispatcher.IsFromSecurity)
                    {
                        XtraMessageBox.Show("You Are Not Allowed For This Operation", "PMS Security", MessageBoxButtons.OK, MessageBoxIcon.Stop);

                    }
                }

                Dispatcher.IsFromSecurity = false;
            }
            catch (Exception e)
            {
                HandlePartException(e, ExceptionHandling.SHOWMESSAGEANDLOG, "Dispatcher Has an Error");
            }

            Progress_Reporter.Close_Progress();

            return logicBase;
        }


        private static void HandlePartException(Exception e, ExceptionHandling exceptionHandling = ExceptionHandling.SHOWMESSAGEANDLOGTHENTHROW, String messageBoxMessage = null)
        {
            Action<String> showErrorMessageBox = m =>
            {
                XtraMessageBox.Show(m, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            };
            Action<Exception> showErrorMessage = ex =>
            {

                if (String.IsNullOrEmpty(messageBoxMessage))
                {
                    showErrorMessageBox("Error Description :" + ex.Message);
                }
                else
                {
                    String exMsg = BuildExceptionDetailMessage(ex);

                    var msg = messageBoxMessage + Environment.NewLine +
                        "Error Description :" + exMsg;

                    showErrorMessageBox(msg);
                }
            };

            switch (exceptionHandling)
            {
                case ExceptionHandling.RETURN:

                case ExceptionHandling.SHOWMESSAGE:
                    showErrorMessage(e);

                    break;

                case ExceptionHandling.SHOWMESSAGEANDLOG:

                    showErrorMessage(e);

                    LogException(e);


                    break;
                case ExceptionHandling.SHOWMESSAGEANDLOGTHENTHROW:

                    showErrorMessage(e);

                    LogException(e);

                    goto case ExceptionHandling.THROW;

                    break;
                case ExceptionHandling.THROW:
                    if (e != null)
                    {
                        //  throw e;
                    }
                    break;
            }
        }


        #endregion

        [DllImport("User32.dll")]
        private static extern bool GetLastInputInfo(ref LASTINPUTINFO Dummy);
        [DllImport("Kernel32.dll")]
        private static extern uint GetLastError();

        internal static void LoadTravelDetailBuffer()
        {
            throw new NotImplementedException();
        }
    }

    public class CNETTabbedMdiManager : XtraTabbedMdiManager, IXtraTabProperties
    {
        public CNETTabbedMdiManager()
            : base()
        {

        }

        protected override DocumentSelector CreateDocumentSelector(XtraMdiTabPage startPage, XtraMdiTabPage nextPage)
        {
            return new CustomDocumentSelector(this, startPage, nextPage);
        }
    }

    public class CustomDocumentSelector : DocumentSelector
    {
        public CustomDocumentSelector(XtraTabbedMdiManager manager, XtraMdiTabPage startPage, XtraMdiTabPage nextPage)
            : base(manager, startPage, nextPage)
        {
        }
        protected override void DrawPreview(GraphicsCache cache)
        {
        }
    }
}


