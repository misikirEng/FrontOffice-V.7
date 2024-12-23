using CNET.ERP.Client.Common.UI;
using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraLayout;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrintingLinks;
using DevExpress.XtraReports.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraGrid.Views.Base;
using CNET.FrontOffice_V._7;
using CNET.FrontOffice_V._7;
using DevExpress.Data;
using CNET.FrontOffice_V._7.Forms.State_Change;
using CNET_V7_Domain.Domain.SettingSchema;
using ProcessManager;
using CNET_V7_Domain.Misc.PmsDTO;
using CNET.FrontOffice_V._7.PMS.Common_Classes;
using CNET.ERP.Client.UI_Logic.PMS.Forms.CommonClass;
using CNET_V7_Domain.Domain.TransactionSchema;
using CNET_V7_Domain.Domain.CommonSchema;
using CNET.Progress.Reporter;

namespace CNET.FrontOffice_V._7.Forms.Non_Navigatable_Modals
{
    public partial class frmSplitBills : UILogicBase
    {

        private GridHitInfo downHitInfo = null;
        public string passedDate;
        int i = 0;
        int j = 0;
        int gy = 0;
        int ly = 0;
        List<Control> prevcon = new List<Control>();
        int verticalCount = 1;
        List<GridControl> prevGrds = new List<GridControl>();

        private int adSplitBill = 0;

        //maximum eight windows
        private const int TotalWindowsCount = 8;
        private List<SplitWindow> _splitWindows = new List<SplitWindow>(TotalWindowsCount);
        private int _currentWindowsCount = 0;

        //inhouse registrations
        private List<RegistrationListVMDTO> regListVM = new List<RegistrationListVMDTO>();



        /*** <<<< Properties >>>>**/
        public DateTime CurrentTime { get; set; }
        internal RegistrationListVMDTO RegExt { get; set; }

        /********************** CONSTRUCTOR *****************/
        public frmSplitBills()
        {

            InitializeComponent();

            InitializeUI();

        }

        #region Helper Methods

        private void InitializeUI()
        {
            this.WindowState = FormWindowState.Maximized;
        }


        private bool InitializeData()
        {
            try
            {
                if (RegExt == null)
                {
                    SystemMessage.ShowModalInfoMessage("Please select registration!", "ERROR");
                    return false;
                }

                DateTime? currentTime = UIProcessManager.GetServiceTime();
                if (currentTime == null) return false;

                CurrentTime = currentTime.Value;

                Progress_Reporter.Show_Progress("Initializing Data", "Please Wait.......");

                //check workflow

                ActivityDefinitionDTO workFlow = UIProcessManager.GetActivityDefinitionBydescriptionandreference(CNETConstantes.LU_ACTIVITY_DEFINATION_SPLIT, CNETConstantes.REGISTRATION_VOUCHER).FirstOrDefault();

                if (workFlow != null)
                {

                    adSplitBill = workFlow.Id;
                }
                else
                {
                    Progress_Reporter.Close_Progress();
                    SystemMessage.ShowModalInfoMessage("Please define workflow of SPLIT for Registration Voucher ", "ERROR");
                    return false;
                }

                //Check Activity Previlage
                var userRoleMapper = LocalBuffer.LocalBuffer.UserRoleMapperBufferList.FirstOrDefault(r => r.User == LocalBuffer.LocalBuffer.CurrentLoggedInUser.Id);
                if (userRoleMapper != null)
                {
                    var roleActivity = UIProcessManager.GetRoleActivityByactivityDefinition(adSplitBill).FirstOrDefault(r => r.Role == userRoleMapper.Role && r.NeedsPassCode);
                    if (roleActivity != null)
                    {
                        frmNeedPassword frmNeedPass = new frmNeedPassword(true);

                        frmNeedPass.ShowDialog();
                        if (!frmNeedPass.IsAutenticated)
                        {
                            Progress_Reporter.Close_Progress();
                            return false;
                        }

                    }

                }


                //load in-house guests
                if (regListVM != null)
                    regListVM.Clear();

                regListVM = UIProcessManager.GetRegistrationViewModelData(null, null, CNETConstantes.CHECKED_IN_STATE, SelectedHotelcode);


                //populate windows
                PopulateWindows();

                Progress_Reporter.Close_Progress();

                return true;
            }
            catch (Exception ex)
            {
                Progress_Reporter.Close_Progress();
                XtraMessageBox.Show("Error in initializing data. Detail:: " + ex.Message, "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

        }

        private void PopulateWindows()
        {
            Progress_Reporter.Show_Progress("Creating Windows...", "Please Wait ...");

            _currentWindowsCount = 0;
            j = 0;
            gy = 0;
            ly = 0;
            prevcon = new List<Control>();
            verticalCount = 1;
            _splitWindows.Clear();

            if (layoutControl1.Controls != null && layoutControl1.Controls.Count > 0)
            {
                layoutControl1.Controls.Clear();
                layoutControl1.Refresh();
            }

            //populate split windows
            for (int i = 0; i < TotalWindowsCount; i++)
            {
                SplitWindow window = new SplitWindow()
                {
                    WindowNumber = i + 1,
                    Balance = "0.0"
                };
                _splitWindows.Insert(i, window);
            }

            //populate guest ledger
            if (!PopulateGuestLedger())
            {
                Progress_Reporter.Close_Progress();
                return;
            }

            //show default windows
            int maxWin = 0;
            foreach (var win in _splitWindows)
            {
                if (win.GetSplitDtoList().Count > 0)
                {
                    if (win.WindowNumber > maxWin) maxWin = win.WindowNumber;

                }
            }

            for (int i = 0; i < maxWin; i++)
            {
                CreateWindows(_splitWindows[i]);
            }

            if (_currentWindowsCount == 1)
            {
                bbiNew.PerformClick();
            }

            Progress_Reporter.Close_Progress();

        }

        private bool PopulateGuestLedger()
        {
            try
            {
                Progress_Reporter.Show_Progress("Loading data...", "Please Wait.......");
                GuestLedgerDTO gLedger = UIProcessManager.GetGuestLedger(RegExt.Id, RegExt.Arrival.Date, RegExt.Departure.Date, RegExt.Room, null);

                if (gLedger == null)
                {
                    //SystemMessage.ShowModalInfoMessage("Unable to get guest ledger information", "ERROR");
                    return false;
                }


                //populate Room Charge splitBillDTO
                if (gLedger.RoomCharges != null && gLedger.RoomCharges.Count > 0)
                {
                    foreach (var rc in gLedger.RoomCharges)
                    {
                        SplitDTO splitDto = new SplitDTO()
                        {
                            VoucherId = rc.voucherId,
                            VoucherCode = rc.voucherCode,
                            Purpose = rc.roomRate,
                            Date = rc.date,
                            Amount = rc.grandTotal,
                            PrintStatus = rc.tranRefValue
                        };



                        var luk = LocalBuffer.LocalBuffer.SystemConstantDTOBufferList.FirstOrDefault(l => l.Id == rc.relationType);
                        SplitWindow window = null;
                        if (rc.relationType == null || luk == null)
                        {
                            //add to the default window
                            window = _splitWindows.ElementAt(0);

                        }
                        else
                        {
                            window = _splitWindows.FirstOrDefault(w => w.WindowNumber == Convert.ToInt32(luk.Value));

                        }

                        //if room charge is already printed, disable the window
                        if (rc.tranRefValue == 1)
                        {
                            window.Enabled = false;
                        }

                        window.AddSplitDTO(splitDto);
                    }
                }

                //populate Extra bill splitBillDTO
                if (gLedger.ExtraBills != null && gLedger.ExtraBills.Count > 0)
                {
                    foreach (var rc in gLedger.ExtraBills)
                    {
                        SplitDTO splitDto = new SplitDTO()
                        {
                            VoucherId = rc.invoiceId.Value,
                            VoucherCode = rc.invoicecode,
                            Purpose = "Extra Bill",
                            Date = rc.date,
                            Amount = rc.grandTotal
                        };


                        var luk = LocalBuffer.LocalBuffer.SystemConstantDTOBufferList.FirstOrDefault(l => l.Id == rc.relationType);

                        if (rc.relationType == null || luk == null || luk.Id == 0)
                        {
                            //add to the default window
                            SplitWindow window = _splitWindows.ElementAt(0);
                            window.AddSplitDTO(splitDto);
                        }
                        else
                        {
                            SplitWindow window = null;
                            if (luk.Id == CNETConstantes.VOUCHER_RELATION_TYPE_NES_REF || luk.Id == CNETConstantes.VOUCHER_RELATION_TYPE_EXT_REF ||
                                luk.Id == CNETConstantes.VOUCHER_RELATION_TYPE_INT_REF)
                            {
                                window = _splitWindows.ElementAt(0);
                            }
                            else
                            {
                                window = _splitWindows.FirstOrDefault(w => w.WindowNumber == Convert.ToInt32(luk.Value));
                            }
                            window.AddSplitDTO(splitDto);
                        }
                    }
                }


                //populate Payments splitBillDTO
                if (gLedger.PaymentHistoryList != null && gLedger.PaymentHistoryList.Count > 0)
                {


                    foreach (var rc in gLedger.PaymentHistoryList)
                    {
                        SplitDTO splitDto = new SplitDTO()
                        {
                            VoucherId = rc.receiptId.Value,
                            VoucherCode = rc.receiptNo,
                            Purpose = rc.voucherType,
                            Date = rc.date,
                            Amount = rc.IsCredit ? (-1 * rc.amount.Value) : rc.amount.Value
                        };

                        //if it is net-off
                        if (rc.voucherDef == CNETConstantes.CREDIT_NOTE_VOUCHER)
                        {
                            TaxDTO tax = CommonLogics.GetApplicableTax(RegExt.Id, rc.voucherDef.Value, RegExt.GuestId, null);
                            if (!string.IsNullOrEmpty(tax.Remark))
                            {
                                SystemMessage.ShowModalInfoMessage(tax.Remark, "ERROR");
                                return false;
                            }

                            decimal taxRate = (decimal)tax.Amount;
                            decimal subtotal = Math.Round(rc.amount.Value * ((taxRate / 100) + 1), 2);
                            splitDto.Amount = subtotal;
                        }


                        var luk = LocalBuffer.LocalBuffer.SystemConstantDTOBufferList.FirstOrDefault(l => l.Id == rc.relationType);

                        if (rc.relationType == null || luk == null || luk.Id == 0)
                        {
                            //add to the default window
                            SplitWindow window = _splitWindows.ElementAt(0);
                            window.AddSplitDTO(splitDto);
                        }
                        else
                        {
                            SplitWindow window = _splitWindows.FirstOrDefault(w => w.WindowNumber == Convert.ToInt32(luk.Value));
                            window.AddSplitDTO(splitDto);
                        }
                    }
                }

                Progress_Reporter.Close_Progress();
                return true;
            }
            catch (Exception ex)
            {
                Progress_Reporter.Close_Progress();
                XtraMessageBox.Show("Error in populating data. DETAIL::" + ex.Message, "CNET_v2016", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void CreateColumns(GridView gView, Dictionary<string, string> fieldCaption, SearchLookUpEdit searchLookupEdit = null, int? colWidth = null)
        {
            if (fieldCaption == null || fieldCaption.Count == 0) return;
            foreach (var field in fieldCaption.Keys)
            {
                GridColumn col1 = new GridColumn();
                col1.Name = "col_" + field;
                col1.FieldName = field;
                col1.Caption = fieldCaption[field];
                if (col1.Caption.ToLower().Trim().Contains("amount"))
                {
                    var item = new GridColumnSummaryItem(SummaryItemType.Sum,
                        field, "{0:N}");

                    col1.Summary.Add(item);
                }

                col1.Visible = true;
                if (colWidth != null)
                {
                    col1.Width = colWidth.Value;
                }

                gView.Columns.Add(col1);
            }

            if (searchLookupEdit != null)
            {
                searchLookupEdit.Properties.View = gView;
            }
        }

        private void CreateWindows(SplitWindow window)
        {
            if (window == null) return;

            int gx = 12;
            int lx = 0;
            int gpadding = 60;
            int lpadding = 60;

            int count = 0;

            int gheight = 260;
            int gwidth = 156 * 2;
            int lheight = 350;
            int lwidth = 177 * 2;

            foreach (Control c in layoutControl1.Controls)
            {
                string h = c.Name;
                if (h.Contains("grid"))
                {
                    count++;
                }
            }
            if (count != 0)
            {
                gx += (((count % 4) - 1) * 165 * 2);
                lx += (((count % 4) - 1) * 165 * 2);
            }

            Point frGcontrol = new Point();
            Point frLcontrol = new Point();

            #region Control Names

            string gridControlName = "gridcontrol" + _currentWindowsCount;
            string layoutControlName = "layoutcontrol" + _currentWindowsCount;
            string gridViewName = "gridview" + _currentWindowsCount;
            string CustomercomboName = "Customer" + _currentWindowsCount;
            string Customerlable = "Window " + (_currentWindowsCount + 1);
            string GrandTotalName = "GrandTotal" + _currentWindowsCount;

            #endregion


            if (count == 0)
            {
                frGcontrol = new Point(gx, gy);
                frLcontrol = new Point(lx, ly);
            }
            else if (count % 4 == 0)
            {
                int k = 0;
                gx = 12;
                lx = 0;
                gy = gy + gheight + gpadding + verticalCount;
                ly = ly + lheight + lpadding + verticalCount;

                frGcontrol = new Point(gx, gy);
                frLcontrol = new Point(lx, ly);

                verticalCount++;
            }
            else
            {
                gx = gx + 165 * 2; //177
                lx = lx + 165 * 2;
                frGcontrol = new Point(gx, gy);
                frLcontrol = new Point(lx, ly);
            }

            //bill grids
            GridControl gcBills = new GridControl();
            GridView gvBills = new GridView();

            //combo grid view
            GridView comboGridView = new GridView();
            Label GrandTotal = new Label();
            Label Customer = new Label();

            SearchLookUpEdit combo = new SearchLookUpEdit();

            #region Combo =>  In-house Regstrations

            string[] comboFields = { "Id", "Registration", "Guest", "Room", "RoomTypeDescription" };
            string[] comboCaptions = { "Id", "Reg. Code", "Guest", "Room", "Room Type" };

            //create column
            CreateColumns(comboGridView, GetKeyValuePair(comboFields, comboCaptions), combo);
            combo.Name = CustomercomboName;
            combo.Location = new Point(frGcontrol.X + 65, frGcontrol.Y);
            combo.Size = new System.Drawing.Size(30, 20);
            combo.MinimumSize = new System.Drawing.Size(30, 20);
            combo.Width = gwidth - 65;
            combo.Properties.NullText = "";
            combo.Tag = window.WindowNumber;
            combo.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Default;
            combo.BringToFront();
            combo.Properties.ValueMember = "Id";
            combo.Properties.DisplayMember = "Guest";
            combo.Properties.DataSource = regListVM;
            combo.EditValue = RegExt.Id;

            #endregion

            #region customer lable
            Customer.Text = Customerlable;
            Customer.Location = new Point(frGcontrol.X, frGcontrol.Y);
            Customer.Size = new System.Drawing.Size(65, 30);
            Customer.BringToFront();
            #endregion

            #region  GrandTotal Lable
            GrandTotal.Name = GrandTotalName;
            GrandTotal.Text = "Balance: " + window.Balance;
            GrandTotal.Location = new Point(frGcontrol.X + 120, frGcontrol.Y + 185 + 80);
            GrandTotal.Size = new System.Drawing.Size(120, 15);
            GrandTotal.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            GrandTotal.Font = new System.Drawing.Font("Arial", 12, FontStyle.Regular);
            GrandTotal.BringToFront();
            #endregion

            // LayoutControlItem layoutControlItem = new LayoutControlItem();

            ((ISupportInitialize)(gcBills)).BeginInit();
            ((ISupportInitialize)(gvBills)).BeginInit();
            // ((ISupportInitialize)(layoutControlItem)).BeginInit();

            //layoutControl1.Controls.Add(GrandTotal);
            layoutControl1.Controls.Add(gcBills);
            layoutControl1.Controls.Add(Customer);
            layoutControl1.Controls.Add(combo);



            _currentWindowsCount++;


            #region  gridview and gridview columns

            string[] gridFields = { "SN", "VoucherCode", "Purpose", "Date", "Amount" };
            string[] gridCaptions = { "SN", "Voucher No.", "Purpose", "Date", "Amount" };

            CreateColumns(gvBills, GetKeyValuePair(gridFields, gridCaptions), null, 120);
            gvBills.OptionsView.ShowFooter = true;
            gvBills.CustomDrawCell += gvBills_CustomDrawCell;
            gvBills.OptionsView.ShowIndicator = false;
            gvBills.Appearance.EvenRow.Options.UseBackColor = true;
            gvBills.Appearance.EvenRow.BackColor = ColorTranslator.FromHtml("GradientInactiveCaption");
            gvBills.OptionsView.EnableAppearanceEvenRow = true;
            gvBills.OptionsSelection.EnableAppearanceHideSelection = true;
            gvBills.OptionsSelection.EnableAppearanceFocusedCell = false;
            gvBills.OptionsDetail.EnableMasterViewMode = false;
            gvBills.OptionsCustomization.AllowColumnMoving = false;
            gvBills.Tag = window.WindowNumber;


            #endregion

            #region gridcontrol

            gvBills.OptionsView.ShowGroupPanel = false;
            gcBills.AllowDrop = true;
            gcBills.Location = new Point(frGcontrol.X, frGcontrol.Y + 30);
            gcBills.MainView = gvBills;
            gcBills.Name = gridControlName;
            gcBills.Size = new System.Drawing.Size(gwidth, gheight);
            gcBills.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { gvBills });
            gcBills.Tag = window.WindowNumber;

            gcBills.DragDrop += new DragEventHandler(gridControl_DragDrop);
            gcBills.DragOver += new DragEventHandler(gridControl_DragOver);
            gvBills.GridControl = gcBills;
            gvBills.Name = gridViewName;
            gvBills.OptionsBehavior.Editable = false;
            gvBills.OptionsBehavior.ReadOnly = false;
            gvBills.OptionsSelection.EnableAppearanceFocusedCell = false;
            gvBills.MouseDown += new MouseEventHandler(gridView_MouseDown);
            gvBills.MouseMove += new MouseEventHandler(gridView_MouseMove);
            gvBills.RowStyle += gvBills_RowStyle;
            if (window.WindowNumber != 1)
            {
                gcBills.Enabled = window.Enabled;
                combo.Enabled = window.Enabled;
                gvBills.Appearance.Row.ForeColor = ColorTranslator.FromHtml("255, 99, 71");
            }


            layoutControl1.ResumeLayout(false);
            ((ISupportInitialize)(gcBills)).EndInit();
            ((ISupportInitialize)(gvBills)).EndInit();
            gcBills.ForceInitialize();
            gcBills.BeginUpdate();
            try
            {
                gcBills.DataSource = null;
                gcBills.DataSource = window.GetSplitDtoList();
                gcBills.RefreshDataSource();
                gvBills.RefreshData();
                gvBills.BestFitColumns();
            }
            finally
            {
                gcBills.EndUpdate();
            }
            int scrollheight = verticalCount * ly + lpadding + verticalCount + 50;
            xtraScrollableControl1.AutoScrollMinSize = new Size(100, scrollheight);
            #endregion


        }



        private Dictionary<string, string> GetKeyValuePair(string[] fields, string[] values)
        {
            if (fields == null || fields.Length == 0) return null;
            if (values == null || values.Length == 0) return null;

            if (fields.Length != values.Length)
            {
                throw new Exception("Key and Value length should be the same");

            }

            Dictionary<string, string> fcDict = new Dictionary<string, string>();
            for (int i = 0; i < fields.Length; i++)
            {
                if (fcDict.ContainsKey(fields[i]))
                {
                    throw new Exception("Key is already added to the dictionary");
                }
                fcDict.Add(fields[i], values[i]);
            }

            return fcDict;

        }

        private void MoveRows(GridHitInfo srcHitInfo, GridHitInfo hitInfo, GridControl g)
        {

            if (srcHitInfo == hitInfo) return;

            GridView gvSource = srcHitInfo.View;
            GridView gvDest = hitInfo.View;

            SplitDTO dto = gvSource.GetRow(srcHitInfo.RowHandle) as SplitDTO;

            int srcWindowNum = (int)gvSource.Tag;
            int destWindowNum = (int)gvDest.Tag;

            SplitWindow windowSrc = _splitWindows.FirstOrDefault(w => w.WindowNumber == srcWindowNum);
            SplitWindow windowDest = _splitWindows.FirstOrDefault(w => w.WindowNumber == destWindowNum);


            if (windowSrc == null || windowDest == null) return;

            if (dto == null) return;

            //check window-1 has at-least one item
            if (srcWindowNum == 1 && windowSrc.GetSplitDtoList().Count == 1)
            {
                XtraMessageBox.Show(this, "You can not move the last item", "CNET ERP", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            if (dto.PrintStatus == 1)
            {
                XtraMessageBox.Show(this, "This bill has been already printed!", "CNET ERP", MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
                return;
            }

            //update source grid
            gvSource.BeginDataUpdate();

            windowSrc.RemoveSplitDTO(dto);
            gvSource.GridControl.DataSource = windowSrc.GetSplitDtoList();
            gvSource.RefreshData();
            gvSource.BestFitColumns();

            gvSource.EndDataUpdate();


            //Update destination grid
            gvDest.BeginDataUpdate();

            windowDest.AddSplitDTO(dto);
            gvDest.GridControl.DataSource = windowDest.GetSplitDtoList();
            gvDest.RefreshData();
            gvDest.BestFitColumns();

            //gvDest.RefreshEditor(true);
            gvDest.EndDataUpdate();

        }

        public void ClearControls()
        {
            //if (POSUICore.posuicoreform != null)
            //{
            //    POSUICore.posuicoreform.ClearSelectedItems();
            //}

            //POSUIDataCache.ClearRMS();
            //if (UIRMS.uirmsform != null)
            //{
            //    UIRMS.uirmsform.ClearEditValueData();
            //    UIRMS.uirmsform.ClearTableAndCoverKeyPad();
            //}
            //DataManager.RefreshCustomerDisplay("POS Made by CnetSoft", DataManager.GrandTotalForDisply);

            //POSUICore.posuicoreform.pnlRightMenu.Controls.Clear();

            //POSUICore.posuicoreform.pnlRightMenu.Controls.Add(UIPayment.uipaymentform.categoryPrev);

            //POSUICore.posuicoreform.GetContentsPanel().Controls.Clear();

            //POSUICore.posuicoreform.GetContentsPanel().Controls.Add(UIPayment.uipaymentform.contentPrev);
            //UIPayment.uipaymentform.Keypad.ClearAll();
            //UIPayment.uipaymentform.IsPageShowing = false;
            //UIPayment.uipaymentform.SetDefaultPayment();

            //DataManager.IsReferenced = false;
            //DataManager.priceExtracted = false;
            //DataManager.UpdatedVo = null;
            //POSUICore.posuicoreform.tlRightHeader1.PerformItemClick();


            //POSSettingCache.SelectedCustomer = new CustomerDTO();


            //DataManager.nonCashTrxn = null;

            //POSUIDataCache.SettlemetItems = null;
            //UIPayment.uipaymentform.ActivateSelectionGrid();
            //if (DataManager.approvedFunctionalities.Contains(CNETConstantes.SEC_RMS_VOID))
            //{
            //    POSUICore.posuicoreform.tileItem5.Enabled = true;
            //    POSUICore.posuicoreform.tileItem5.Visible = true;
            //}

            //POSUICore.posuicoreform.CheckUserRole();
            //UIPayment.uipaymentform.ClearTotal();

            //POSUICore.posuicoreform.ClearSelectedItems();


            //POSUIDataCache.ClearRMS();
            //if (UIRMS.uirmsform != null)
            //{
            //    UIRMS.uirmsform.ClearEditValueData();
            //    UIRMS.uirmsform.ClearTableAndCoverKeyPad();
            //}
            //if (!POSUICore.posuicoreform.IsDisposed)
            //{
            //    UIPayment.uipaymentform.ActivateSelectionGrid();
            //    POSUICore.posuicoreform.tlRightHeader1.PerformItemClick();
            //    POSUICore.posuicoreform.HideFlyout();

            //}
            //RMSPOS.InitializeBufferVoucher();
            //UIPayment.uipaymentform.SetDefaultTransaction();
            //UIPayment.uipaymentform.RefreshPOSControl();
        }

        private void print()
        {

        }

        //Helper method to show checkout form based on the value of the passed parameters
        private bool PerfomCheckout(RegistrationListVMDTO regExten, bool isWithoutRecept, bool isWithZeroBalance = false, int window = -1)
        {
            bool flag = false;
            try
            {
                var response = UIProcessManager.GetVoucherBufferById(regExten.Id);
                if (response == null || !response.Success) return false;

                VoucherBuffer voucherBuffer = response.Data;

                if (voucherBuffer == null) return false;
                var osd = LocalBuffer.LocalBuffer.ObjectStateDefinitionBufferList.FirstOrDefault(o => o.Id == voucherBuffer.Voucher.LastState);
                string state = osd == null ? "" : osd.Description;
                if (voucherBuffer.Voucher.LastState != CNETConstantes.CHECKED_IN_STATE && voucherBuffer.Voucher.LastState != CNETConstantes.OSD_PENDING_STATE && voucherBuffer.Voucher.LastState != CNETConstantes.NO_SHOW_STATE)
                {
                    SystemMessage.ShowModalInfoMessage(state + " state can not be changed to Check Out state!", "ERROR");
                    return false;
                }

                frmCheckOut frmCheckOut = new frmCheckOut();
                frmCheckOut.RegExtension = regExten;
                frmCheckOut.VoucherBuffer = voucherBuffer;
                frmCheckOut.Window = window;
                frmCheckOut.checkOutWithOutReceipt = isWithoutRecept;
                frmCheckOut.IsWithZeroBalance = isWithZeroBalance;
                if (frmCheckOut.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    flag = true;

                }

                return flag;
            }
            catch (Exception ex)
            {
                SystemMessage.ShowModalInfoMessage("Error in processing checkout. DETAIL::" + ex.Message, "ERROR");

                return false;
            }
        }


        #endregion


        #region Event Handlers

        private void bbiCheckOut_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                List<Control> k = new List<Control>();

                GridControl Focused = new GridControl();
                List<GridControl> Fgr = new List<GridControl>();
                for (int j = 0; j < _currentWindowsCount; j++)
                {
                    string gname = "gridcontrol" + j;
                    k = layoutControl1.Controls.Find(gname, true).ToList();
                    GridControl g = k[0] as GridControl;
                    if (g != null && g.ContainsFocus)
                    {
                        Fgr.Add(g);
                        break;
                    }

                }

                if (Fgr.Count <= 0)
                {
                    XtraMessageBox.Show("You have not selected a window!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                Focused = Fgr.FirstOrDefault();
                int window = Convert.ToInt32(Focused.Tag);
                if (window == 1)
                {
                    XtraMessageBox.Show("Window 1 can't be checked-out!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                bool isZeroCheckout = false;

                DateTime? currentTime = UIProcessManager.GetServiceTime();
                if (currentTime == null) return;

                if (RegExt == null)
                {
                    SystemMessage.ShowModalInfoMessage("Please Select a registration.", "ERROR");
                    return;
                }
                if (RegExt.Registration != null)
                {
                    DateTime? CurrentDate = UIProcessManager.GetServiceTime();
                    if (CurrentDate == null) return;

                    //Check any daily room charge post for this registration
                    //if (UIProcessManager.GetDailyChargePosting(RegExt.Registration, CNETConstantes.DAILY_ROOM_CHARGE_VOUCHER.ToString(), "ALL") == null)
                    //{
                    //    var transferedBills = UIProcessManager.GetTransferedBillsBySource(RegExt.Registration, null);
                    //    if (transferedBills == null || transferedBills.Count == 0)
                    //    {
                    //        SystemMessage.ShowModalInfoMessage("Unable to get room charge posting or room charge posting has not been made for this registration", "ERROR");
                    //        return;
                    //    }

                    //    var filtered = transferedBills.Where(t => t.voucherDefinition == CNETConstantes.DAILY_ROOM_CHARGE_VOUCHER).ToList();
                    //    if (filtered == null || filtered.Count == 0)
                    //    {
                    //        SystemMessage.ShowModalInfoMessage("Unable to get room charge posting or room charge posting has not been made for this registration", "ERROR");
                    //        return;
                    //    }
                    //    else
                    //    {
                    //        DialogResult dialog = MessageBox.Show(@"Room charges of this registration has been transfered. Do you want to check out with zero balance?", "CHECK OUT", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    //        if (dialog == DialogResult.No)
                    //        {
                    //            return;
                    //        }
                    //        else
                    //        {
                    //            isZeroCheckout = true;
                    //        }
                    //    }


                    //}


                    if (currentTime.Value != null && RegExt.Departure.Date != CurrentDate.Value.Date)
                    {
                        SystemMessage.ShowModalInfoMessage("You can not check out non due out guest. Please amend its date!", "ERROR");
                        return;
                    }

                    // Check this registration has room sharing
                    DialogResult drOut = DialogResult.Yes;
                    List<RelationDTO> relations = UIProcessManager.SelectAllRelation().Where(r => (r.ReferencedObject == RegExt.Id || r.ReferringObject == RegExt.Id) && r.RelationLevel == 1).ToList();
                    if (relations.Count > 0)
                    {
                        drOut = MessageBox.Show(@"This registration has room sharing, Do you want to break the share?", "CHECK OUT", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                        if (drOut == DialogResult.No)
                        {
                            return;
                        }


                    }

                    if (drOut == DialogResult.Yes)
                    {

                        ActivityDTO activity = null;
                        ActivityDefinitionDTO ad =
                        UIProcessManager.GetActivityDefinitionBydescriptionandreference(CNETConstantes.LU_ACTIVITY_DEFINATION_REINSTATE, CNETConstantes.REGISTRATION_VOUCHER).LastOrDefault();
                        if (ad != null)
                        {

                            activity = ActivityLogManager.GetActivity(RegExt.Id, ad.Id);
                        }
                        // if there is activity with "Reinstate"
                        if (activity != null)
                        {
                            // if the remark is "With Receipt Reprint"
                            if (activity.Remark == "With receipt reprint")
                            {

                                if (PerfomCheckout(RegExt, false, isZeroCheckout, window))
                                {
                                    //populate windows
                                    PopulateWindows();
                                }
                            }

                            // if the remark is "Without Receipt Reprint"
                            else
                            {
                                if (PerfomCheckout(RegExt, true, isZeroCheckout, window))
                                {
                                    //populate windows
                                    PopulateWindows(); ;
                                }
                            }
                        }

                        // if there is NO activity with "Reinstate".
                        else
                        {

                            if (PerfomCheckout(RegExt, false, isZeroCheckout, window))
                            {
                                //populate windows
                                PopulateWindows();
                            }
                        }
                    }
                }
                else
                {
                    SystemMessage.ShowModalInfoMessage("No registration selected!", "ERROR");
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Error in check-out guest. Detail:: " + ex.Message, "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        private void gvBills_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
        {
            GridView view = sender as GridView;
            if (e.Column.Caption == "SN")
            {
                SplitDTO dto = view.GetRow(e.RowHandle) as SplitDTO;
                if (dto != null) dto.SN = (e.RowHandle + 1).ToString();
                e.DisplayText = (e.RowHandle + 1).ToString();
            }
            if (e.Column.FieldName == "Id")
            {
                e.DisplayText = "";

            }
        }
        private void gvBills_RowStyle(object sender, RowStyleEventArgs e)
        {
            GridView view = sender as GridView;
            if (view == null) return;

            SplitDTO dto = (SplitDTO)view.GetRow(e.RowHandle);
            if (dto == null) return;
            if (dto.PrintStatus == 1)
            {
                e.Appearance.ForeColor = ColorTranslator.FromHtml("255, 99, 71");
            }

        }
        private void bbiCancel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            int gx = 12;
            int gy = 0;
            int lx = 0;
            int ly = 0;
            int gpadding = 20;
            int lpadding = 24;
            int verticalCount = 1;
            int count = 0;

            int gheight = 200;
            int gwidth = 100;
            int lheight = 204;
            int lwidth = 116;

            foreach (Control c in layoutControl1.Controls)
            {
                string h = c.Name;
                if (h.Contains("grid"))
                {
                    count++;
                }
            }
            if (count != 0)
            {
                gx += (((count % 7) - 1) * 116);
                lx += (((count % 7) - 1) * 116);
            }

            while (count < 35)
            {
                Point frGcontrol = new Point();
                Point frLcontrol = new Point();

                string gridControlName = "gridcontrol" + count;
                string layoutControlName = "layoutcontrol" + count;
                string gridViewName = "gridview" + count;

                GridControl gridControl = new GridControl();
                gridControl.Name = gridControlName;

                if (count == 0)
                {
                    frGcontrol = new Point(gx, gy);
                    frLcontrol = new Point(lx, ly);
                }
                else if (count % 7 == 0)
                {
                    int k = 0;
                    gx = 12;
                    lx = 0;
                    gy = gy + gheight + gpadding + verticalCount;
                    ly = ly + lheight + lpadding + verticalCount;

                    frGcontrol = new Point(gx, gy);
                    frLcontrol = new Point(lx, ly);

                    verticalCount++;

                }
                else
                {
                    gx = gx + 116;
                    lx = lx + 116;
                    frGcontrol = new Point(gx, gy);
                    frLcontrol = new Point(lx, ly);
                }

                GridView gridView = new GridView();
                LayoutControlItem layoutControlItem = new LayoutControlItem();

                ((ISupportInitialize)(gridControl)).BeginInit();
                ((ISupportInitialize)(gridView)).BeginInit();
                ((ISupportInitialize)(layoutControlItem)).BeginInit();
                layoutControl1.Controls.Add(gridControl);

                //gcBills
                gridControl.AllowDrop = true;
                gridControl.Location = frGcontrol;//new Point(244, 38);
                gridControl.MainView = gridView;
                gridControl.Name = gridControlName;
                gridControl.Size = new System.Drawing.Size(gwidth, gheight);
                //gcBills.DataSource = dataTable1.Clone();

                // gcBills.TabIndex = 
                gridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { gridView });

                gridControl.DragDrop += new DragEventHandler(gridControl_DragDrop);
                gridControl.DragOver += new DragEventHandler(gridControl_DragOver);
                //GridView
                gridView.GridControl = gridControl;
                gridView.Name = gridViewName;
                gridView.OptionsBehavior.Editable = false;

                gridView.MouseDown += new MouseEventHandler(gridView_MouseDown);
                gridView.MouseMove += new MouseEventHandler(gridView_MouseMove);

                //layout contro;
                layoutControlItem.Control = gridControl;
                layoutControlItem.CustomizationFormText = "layoutControlItem1";
                layoutControlItem.Location = frLcontrol;//new Point(232, 26);
                layoutControlItem.Name = layoutControlName;
                layoutControlItem.Size = new Size(lwidth, lheight);
                layoutControlItem.Text = "layoutControlItem1";
                layoutControlItem.TextSize = new Size(0, 0);
                layoutControlItem.TextToControlDistance = 0;
                layoutControlItem.TextVisible = false;

                this.layoutControlGroup1.Items.AddRange(new BaseLayoutItem[] { layoutControlItem });

                ((ISupportInitialize)(gridControl)).EndInit();
                ((ISupportInitialize)(gridView)).EndInit();
                count++;

            }
            int scrollheight = verticalCount * lheight + lpadding + verticalCount + 50;
            xtraScrollableControl1.AutoScrollMinSize = new Size(100, scrollheight);
        }

        private void gridControl_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(GridHitInfo)))
            {
                e.Effect = DragDropEffects.Move;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void gridControl_DragDrop(object sender, DragEventArgs e)
        {
            GridControl grid = sender as GridControl;
            GridView view = grid.MainView as GridView;
            GridHitInfo srcHitInfo = e.Data.GetData(typeof(GridHitInfo)) as GridHitInfo;
            GridHitInfo hitInfo = view.CalcHitInfo(grid.PointToClient(new Point(e.X, e.Y)));
            MoveRows(srcHitInfo, hitInfo, grid);
            view.RefreshData();
        }

        private void gridView_MouseDown(object sender, MouseEventArgs e)
        {
            GridView view = sender as GridView;
            downHitInfo = null;
            GridHitInfo hitInfo = view.CalcHitInfo(new Point(e.X, e.Y));
            if (Control.ModifierKeys != Keys.None) return;
            if (e.Button == MouseButtons.Left && hitInfo.InRow && hitInfo.RowHandle != GridControl.NewItemRowHandle)
                downHitInfo = hitInfo;
        }

        private void gridView_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {

                GridView view = sender as GridView;

                if (e.Button == MouseButtons.Left && downHitInfo != null)
                {
                    Size dragSize = SystemInformation.DragSize;
                    Rectangle dragRect = new Rectangle(new Point(downHitInfo.HitPoint.X - dragSize.Width / 2,
                        downHitInfo.HitPoint.Y - dragSize.Height / 2), dragSize);

                    if (!dragRect.Contains(new Point(e.X, e.Y)))
                    {
                        view.GridControl.DoDragDrop(downHitInfo, DragDropEffects.Move);
                        downHitInfo = null;
                        view.RefreshData();
                    }
                }
            }
            catch (Exception ex)
            {
                DevExpress.XtraEditors.XtraMessageBox.Show(ex.StackTrace, "Error");
            }
        }

        private void bbiPrintSplit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //int ListCount = 0;
            //foreach (List<LineItemObj> lll in allG)
            //{
            //    if (lll.Count > 0)
            //    {
            //        ListCount++;
            //    }

            //}

            //if (ListCount <= 1)
            //{
            //    XtraMessageBox.Show("Can't print B/c the bill have not splitted", "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    return;
            //}

            //string addtionalChargePram = null;
            //string discountParam = null;
            //DataManager.priceExtracted = true;
            //int count = 0;
            //if (POSSettingCache.Additionalchargeflexible && !UIPayment.uipaymentform.chkAddtionalCharge.Checked)
            //{
            //    addtionalChargePram = "0";
            //}
            //if (POSSettingCache.Discountflexible && !UIPayment.uipaymentform.checkDiscount.Checked)
            //{
            //    discountParam = "0";
            //}
            //List<LineItemObj> _LineItemObjs = new List<LineItemObj>();
            //foreach (List<LineItemObj> ll in allG)
            //{
            //    count++;

            //    DataManager.lineItemDetailsCollectior = new List<LineItemDetails>();

            //    string RGrdCtrlName = "Customer" + count;
            //    SearchLookUpEdit customer = layoutControl1.Controls.Find(RGrdCtrlName, true).FirstOrDefault() as SearchLookUpEdit;

            //    if (customer.EditValue != null && customer.EditValue != "")
            //    {
            //        GridView view = customer.Properties.View;
            //        CustomerDTO cmbConsignees = (CustomerDTO)customer.Properties.View.GetRow(view.FocusedRowHandle);
            //        POSSettingCache.SelectedCustomer = cmbConsignees;
            //        if (POSSettingCache.SelectedCustomer != null && POSSettingCache.SelectedCustomer.Code != null)
            //        {
            //            DataManager._posBuffer.voucherObj.voucher.consignee = POSSettingCache.SelectedCustomer.Code;
            //        }
            //    }
            //    else
            //    {
            //        POSSettingCache.SelectedCustomer = null;
            //        DataManager._posBuffer.voucherObj.voucher.consignee = "Null";
            //        DataManager._posBuffer.voucherObj.voucher.component = "Null";
            //    }



            //    // List<SearchLookUpEdit> combo = (List<SearchLookUpEdit>)layoutControl1.Controls.Contains(
            //    _LineItemObjs = new List<LineItemObj>();
            //    if (ll != null && ll.Count > 0)
            //    {
            //        foreach (LineItemObj lll in ll)
            //        {
            //            LineItemObj _lineItemObj = new LineItemObj();
            //            LineItemDetails lineitemdetail = UIProcessManager.LineItemDetailClculator(DataManager._posBuffer.voucherObj.voucher, lll.lineItem, true, addtionalChargePram, false, discountParam);
            //            DataManager.lineItemDetailsCollectior.Add(lineitemdetail);
            //            _lineItemObj.lineItem = lineitemdetail.lineItems;
            //            _lineItemObj.lineItemValueFactor = lineitemdetail.lineItemValueFactor;
            //            _lineItemObj.lineItemModfier = lll.lineItemModfier;
            //            _lineItemObj.Name = lll.Name;
            //            _lineItemObj.Quantity = lll.Quantity;
            //            _lineItemObj.TotalPrice = lll.TotalPrice;
            //            _lineItemObj.UnitPrice = lll.UnitPrice;
            //            _lineItemObj.retrievedLineItems = lll.retrievedLineItems;
            //            _lineItemObj.category = lll.category;
            //            _LineItemObjs.Add(_lineItemObj);
            //        }

            //        DataManager._posBuffer.voucherObj.lineItemObjs = new List<LineItemObj>();
            //        DataManager._posBuffer.voucherObj.lineItemObjs.AddRange(_LineItemObjs);
            //        DataManager.voucherFinal = UIProcessManager.CalculatedVoucher(DataManager._posBuffer.voucherObj.voucher, DataManager.lineItemDetailsCollectior, false, null);
            //        DataManager._posBuffer.voucherObj.voucherValues = new VoucherValue();
            //        DataManager._posBuffer.voucherObj.voucherValues = DataManager.voucherFinal.voucherValues;
            //        DataManager._posBuffer.voucherObj.taxTransaction = new List<TaxTransaction>();
            //        DataManager._posBuffer.voucherObj.taxTransaction = DataManager.voucherFinal.taxTransactions;
            //        DataManager._posBuffer.voucherObj.voucher.grandTotal = DataManager.voucherFinal.voucher.grandTotal;
            //        print();
            //    }
            //}
            //ClearControls();
            //this.Close();
        }

        private void bbiNew_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (_currentWindowsCount >= 8)
                {
                    XtraMessageBox.Show("The maximum allowed split window is 8.", "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                for (int j = 0; j < _currentWindowsCount; j++)
                {
                    string gname = "gridcontrol" + j;
                    prevcon = layoutControl1.Controls.Find(gname, true).ToList();
                    GridControl g = prevcon[0] as GridControl;
                    prevGrds.Add(g);
                }

                //List<LineItemObj> emptySource = new List<LineItemObj>();
                //allG.Insert(allG.Count, emptySource);
                //copy.Clear();
                //copy = allG.Select(x => x.ToList()).ToList();
                SplitWindow window = _splitWindows.FirstOrDefault(w => w.WindowNumber == _currentWindowsCount + 1);
                CreateWindows(window);
                if (_currentWindowsCount == 2)
                {

                    this.Width = (312 * 2) + (50 * 2);
                    this.CenterToScreen();

                }
                else if (_currentWindowsCount == 3)
                {

                    this.Width = (312 * 3) + (50 * 3);
                    this.CenterToScreen();

                }
                else if (_currentWindowsCount == 4)
                {
                    this.Width = (312 * 4) + (50 * 4);
                    this.CenterToScreen();
                }
                else if (_currentWindowsCount == 5)
                {
                    this.Height = (260 * 2) + (250);
                    this.CenterToScreen();
                }

            }
            catch (Exception ex)
            {
                SystemMessage.ShowModalInfoMessage("Error in adding new window. Detail:: " + ex.Message, "ERROR");
            }


        }

        private void bbiSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                List<SystemConstantDTO> winLookups = LocalBuffer.LocalBuffer.SystemConstantDTOBufferList.Where(l => l.Category == CNETConstantes.SPLIT_WINDOWS).ToList();
                if (winLookups == null || winLookups.Count == 0)
                {
                    SystemMessage.ShowModalInfoMessage("Please Maintain Split Window Lookups first!", "ERROR");
                    return;
                }

                Progress_Reporter.Show_Progress("Saving Bill Split..", "Please Wait.......");

                List<TransactionReferenceDTO> traRefList = UIProcessManager.GetTransactionReferenceByreferenced(RegExt.Id);
                if (traRefList == null || traRefList.Count == 0)
                {
                    Progress_Reporter.Close_Progress();
                    XtraMessageBox.Show("Unable to get transaction reference record!", "CNET_v2016", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }



                bool flag = true;
                int winCount = 0;
                List<int> newGuestList = new List<int>();
                List<int> RegistrationUpdate = new List<int>();
                foreach (var window in _splitWindows)
                {
                    List<SplitDTO> splitDtoList = window.GetSplitDtoList();
                    if (splitDtoList == null || splitDtoList.Count == 0) continue;

                    winCount++;

                    int? newGuest = null;
                    var guestControl = layoutControl1.Controls.Find("Customer" + (window.WindowNumber - 1), true).FirstOrDefault();
                    if (guestControl != null)
                    {
                        SearchLookUpEdit gView = guestControl as SearchLookUpEdit;
                        newGuest = gView.EditValue == null ? null : Convert.ToInt32(gView.EditValue);

                    }

                    if (newGuest != null)
                    {
                        RegistrationUpdate.Add(newGuest.Value);
                    }
                    //add to newGuestList
                    if (newGuest != null && RegExt.Id != newGuest)
                    {
                        if (newGuest != null)
                        {
                            newGuestList.Add(newGuest.Value);
                        }
                    }

                    foreach (var dto in splitDtoList)
                    {
                        var luk = winLookups.FirstOrDefault(l => l.Value.Trim() == window.WindowNumber.ToString());
                        if (luk == null) continue;
                        var tranRef = traRefList.FirstOrDefault(t => t.Referring == dto.VoucherId);
                        if (tranRef == null) continue;
                        if (newGuest != null && newGuest != RegExt.Id)
                        {
                            tranRef.Referenced = newGuest;
                            tranRef.Remark = RegExt.Id.ToString();
                            tranRef.RelationType = CNETConstantes.DEFAULT_WINDOW;
                        }
                        else
                        {
                            tranRef.RelationType = luk.Id;
                        }
                        if (UIProcessManager.UpdateTransactionReference(tranRef) == null)
                        {
                            if (flag)
                                flag = false;
                        }
                    }

                }

                Progress_Reporter.Close_Progress();
                if (flag)
                {
                    //foreach (string regcode in RegistrationUpdate)
                    //{
                    //    RegistrationList.SynchronizeRegistration(regcode);
                    //}
                    XtraMessageBox.Show("Bill split is successfull!", "CNET_v2016", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    string newGuests = "";
                    foreach (var ng in newGuestList)
                    {
                        newGuests = newGuests + ", " + ng;
                    }
                    DateTime? currentTime = UIProcessManager.GetServiceTime();
                    if (currentTime != null)
                    {
                        ActivityDTO activity = ActivityLogManager.SetupActivity(currentTime.Value, adSplitBill, CNETConstantes.PMS_Pointer, "Split Windows = " + winCount + "    bills transfered to = " + newGuests);
                        activity.Reference = RegExt.Id;
                        UIProcessManager.CreateActivity(activity);
                    }
                }
                else
                    XtraMessageBox.Show("Bill split is done but one or more bills are failed to split!", "CNET_v2016", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (Exception ex)
            {
                Progress_Reporter.Close_Progress();
                XtraMessageBox.Show("Error in saving bill split. Detail:: " + ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        private void bbiBreakItem_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                List<Control> k = new List<Control>();

                GridControl Focused = new GridControl();
                List<GridControl> Fgr = new List<GridControl>();
                for (int j = 0; j < _currentWindowsCount; j++)
                {
                    string gname = "gridcontrol" + j;
                    k = layoutControl1.Controls.Find(gname, true).ToList();
                    GridControl g = k[0] as GridControl;
                    if (g != null && g.ContainsFocus)
                    {
                        Fgr.Add(g);
                        break;
                    }

                }

                if (Fgr.Count <= 0)
                {
                    XtraMessageBox.Show("You have not selected an item!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                Focused = Fgr.FirstOrDefault();
                GridView RightGridVw = (GridView)Focused.ViewCollection[0] as GridView;

                SplitDTO dto = (SplitDTO)RightGridVw.GetRow(RightGridVw.FocusedRowHandle);
                if (dto == null) return;

                if (dto.PrintStatus == 1)
                {
                    XtraMessageBox.Show(this, "This bill has been already printed!", "CNET ERP", MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation);
                    return;
                }

                VoucherDTO vo = UIProcessManager.GetVoucherById(dto.VoucherId);
                if (vo == null)
                {
                    SystemMessage.ShowModalInfoMessage("Unable to get voucher data!", "ERROR");
                    return;
                }

                frmSplitItem frmSplitVoucher = new frmSplitItem();
                frmSplitVoucher.RegistrationExt = RegExt;
                frmSplitVoucher.CurrentVoucher = vo;
                if (frmSplitVoucher.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    PopulateWindows();

                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Error in breaking an item! Detail:: " + ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }

        }

        private void bbiFolio_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                List<Control> k = new List<Control>();

                GridControl Focused = new GridControl();
                List<GridControl> Fgr = new List<GridControl>();
                for (int j = 0; j < _currentWindowsCount; j++)
                {
                    string gname = "gridcontrol" + j;
                    k = layoutControl1.Controls.Find(gname, true).ToList();
                    GridControl g = k[0] as GridControl;
                    if (g != null && g.ContainsFocus)
                    {
                        Fgr.Add(g);
                        break;
                    }

                }

                if (Fgr.Count <= 0)
                {
                    XtraMessageBox.Show("You have not selected a window!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                Focused = Fgr.FirstOrDefault();
                int window = Convert.ToInt32(Focused.Tag);
                frmFolio _frmFolio = new frmFolio();
                _frmFolio.RegistrationExt = RegExt;
                _frmFolio.DefaultWindow = window;
                _frmFolio.ShowDialog();

            }
            catch (Exception ex)
            {

            }
        }


        private void SplitBills_Load(object sender, EventArgs e)
        {
            if (!InitializeData())
            {
                this.Close();
            }
        }


        #endregion




        public int SelectedHotelcode { get; set; }
    }
}
