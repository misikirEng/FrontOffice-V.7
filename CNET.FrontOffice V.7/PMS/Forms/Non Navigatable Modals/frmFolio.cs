using System;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using CNET.ERP.Client.Common.UI;
using DevExpress.XtraGrid.Views.Grid;
using CNET.FrontOffice_V._7;
using DevExpress.XtraReports.UI;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.Controls;
using DevExpress.Utils.Win;
using DevExpress.XtraEditors.Popup;
using DevExpress.XtraGrid.Editors;
using DevExpress.XtraLayout;
using CNET_V7_Domain.Domain.CommonSchema;
using ProcessManager;
using CNET.FrontOffice_V._7.PMS.Common_Classes;
using CNET_V7_Domain.Domain.SettingSchema;
using CNET_V7_Domain.Domain.ViewSchema;
using CNET_V7_Domain;
using CNET_V7_Domain.Domain.TransactionSchema;
using System.Diagnostics;
using CNET_V7_Domain.Misc.PmsDTO;
using CNET_V7_Domain.Misc.PmsView;
using DocumentPrint;
using DocumentPrint.DTO;
using CNET_V7_Domain.Domain.ConsigneeSchema;
using CNET.Progress.Reporter;

namespace CNET.FrontOffice_V._7.Forms.Non_Navigatable_Modals
{
    public partial class frmFolio : UILogicBase
    {

        private RegistrationListVMDTO registrationExt;
        private List<RoomChargesDTO> _selectedRoomCharges = new List<RoomChargesDTO>();
        private List<ExtraBillsDTO> _selectedExtras = new List<ExtraBillsDTO>();
        private List<PaymentHistoryDTO> _selectedPayments = new List<PaymentHistoryDTO>();
        private decimal _totalRoomCharge, _totalExtra, _totalPayments = 0;
        private decimal _taxRate;
        private decimal _discount;
        private List<RegistrationListVMDTO> regListVM = new List<RegistrationListVMDTO>();
        //  private List<OtherConsignee> OtherConsigneeList = null;
        private List<SelectedRegistrationDTO> _selectedFromRegistrations = new List<SelectedRegistrationDTO>();
        List<ExchangeRateDTO> ExchangeRateList { get; set; }
        #region Properties 
        public class SelectedRegistrationDTO
        {
            public int RegId { get; set; }
            public string RegCode { get; set; }
        }

        private const int CP_NOCLOSE_BUTTON = 0x200;
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams myCp = base.CreateParams;
                myCp.ClassStyle = myCp.ClassStyle | CP_NOCLOSE_BUTTON;
                return myCp;
            }
        }

        internal RegistrationListVMDTO RegistrationExt
        {
            get { return registrationExt; }
            set
            {
                try
                {
                    if (value == null) return;
                    teCompany.Text = value.Company;
                    registrationExt = value;
                    teCustomer.Text = value.Guest;
                    teRegistration.Text = value.Registration;
                    teArrivalDate.Text = value.Arrival.ToString("ddd, dd MMMM yyyy hh:mm tt");
                    teDepartureDate.Text = value.Departure.ToString("ddd, dd MMMM yyyy hh:mm tt");
                    //teArrivalDate.Text = value.Arrival.ToString("D");
                    //teDepartureDate.Text = value.Departure.ToString("D");

                }
                catch (Exception ex)
                {
                    return;
                }

            }
        }


        private bool _isBillTransfer = false;
        public bool IsBillTransfer
        {
            get
            {
                return _isBillTransfer;
            }
            set
            {
                _isBillTransfer = value;
                if (value)
                {
                    rpgTransfer.Visible = true;
                    rpgDestination.Visible = true;
                    rpgTransferUtils1.Visible = true;
                    rpgReturn.Visible = true;

                    this.Text = "Bill Transfer";

                    gvRoomCharges.OptionsSelection.MultiSelect = true;
                    gvExtraBills.OptionsSelection.MultiSelect = true;
                    gvPaymentHistory.OptionsSelection.MultiSelect = true;

                }
            }
        }

        private int defWindow = -1;
        public int DefaultWindow
        {
            get
            {
                return defWindow;
            }
            set
            {
                defWindow = value;
            }
        }


        #endregion

        int? defaultCurrency = null;
       

        /// //////////////////// CONSTRUCTOR /////////////////////////////

        public frmFolio()
        {
            InitializeComponent();

            InitializeUI();

            var firstOrDefault = UIProcessManager.SelectAllCurrency().FirstOrDefault(r => r.IsDefault);
            if (firstOrDefault != null)
            {
                defaultCurrency = firstOrDefault.Id;
            }

            ExchangeRateList = UIProcessManager.SelectAllExchangeRate();

        }




        ///////////////////////////////// Helper Methods /////////
        #region Helper Methods

        private void InitializeUI()
        {
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += frmFolio_Load;
            ceSelectAll.CheckedChanged += ceSelectAll_CheckedChanged;
            repoToggle.Toggled += repoToggle_Toggled;
            ripoSlukEditFrom.Popup += ripoSlukEditFrom_Popup;
            gvFromOthers.SelectionChanged += gvFromOthers_SelectionChanged;
            ripoSlukEditFrom.AddNewValue += ripoSlukEditFrom_AddNewValue;


            //Destination-Guests
            GridColumn columnGuest = slukDestination.View.Columns.AddField("Registration");
            columnGuest.Visible = true;
            columnGuest = slukDestination.View.Columns.AddField("Id");
            columnGuest.Visible = false;
            columnGuest = slukDestination.View.Columns.AddField("Room");
            columnGuest.Visible = true;
            columnGuest = slukDestination.View.Columns.AddField("Guest");
            columnGuest.Caption = "Guest";
            columnGuest.Visible = true;
            columnGuest = slukDestination.View.Columns.AddField("RoomTypeDescription");
            columnGuest.Caption = "Room Type";
            columnGuest.Visible = true;
            //slukDestination.ValueMember = "Id";
            //slukDestination.DisplayMember = "Registration";

            //From-Guests
            GridColumn colm = ripoSlukEditFrom.View.Columns.AddField("Registration");
            colm.Visible = true;
            colm = ripoSlukEditFrom.View.Columns.AddField("Id");
            colm.Visible = false;
            colm = ripoSlukEditFrom.View.Columns.AddField("Room");
            colm.Visible = true;
            colm = ripoSlukEditFrom.View.Columns.AddField("Guest");
            colm.Caption = "Guest";
            colm.Visible = true;
            colm = ripoSlukEditFrom.View.Columns.AddField("Company");
            colm.Visible = true;

            ripoSlukEditFrom.ValueMember = "Id";
            ripoSlukEditFrom.DisplayMember = "Registration";



            //Windows
            lukEditWindow.Columns.Add(new LookUpColumnInfo("Description", "Window"));
            lukEditWindow.DisplayMember = "Description";
            lukEditWindow.ValueMember = "Id";

            gvRoomCharges.DoubleClick += GvRoomCharges_DoubleClick;
            gvExtraBills.DoubleClick += GvRoomCharges_DoubleClick;
            gvPaymentHistory.DoubleClick += GvRoomCharges_DoubleClick;

        }

        private void GvRoomCharges_DoubleClick(object sender, EventArgs e)
        {
            GridView view = sender as GridView;
            if (view == null) return;

            ReportGenerator rg = new ReportGenerator();
            RoomChargesDTO rcDto = view.GetRow(view.FocusedRowHandle) as RoomChargesDTO;
            ExtraBillsDTO extraDto = view.GetRow(view.FocusedRowHandle) as ExtraBillsDTO;
            PaymentHistoryDTO paymentDto = view.GetRow(view.FocusedRowHandle) as PaymentHistoryDTO;
            if (rcDto != null)
            {
                rg.GetAttachementReport(rcDto.voucherId);
            }
            else if (extraDto != null)
            {
                rg.GetAttachementReport(extraDto.invoiceId.Value);
            }
            else if (paymentDto != null)
            {
                rg.GetAttachementReport(paymentDto.receiptId.Value);
            }
        }

        void ripoSlukEditFrom_AddNewValue(object sender, AddNewValueEventArgs e)
        {
            if (_selectedFromRegistrations.Count == 0)
            {
                SystemMessage.ShowModalInfoMessage("No registration is selected!", "ERROR");
                return;
            }

            DialogResult dr = XtraMessageBox.Show("Are you sure to transfer all bills from the selected registrations?", "Bill Transfer", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dr == System.Windows.Forms.DialogResult.Yes)
            {
                //Transfer from others
                TransferBillsFromOthers();
            }


        }

        private void gvFromOthers_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            var view = sender as GridView;
            if (view == null) return;
            gvFromOthers = view;
            _selectedFromRegistrations.Clear();

            int[] selectedRows = view.GetSelectedRows();

            for (int i = 0; i < selectedRows.Length; i++)
            {
                RegistrationListVMDTO dto = view.GetRow(selectedRows[i]) as RegistrationListVMDTO;
                if (dto != null)
                    _selectedFromRegistrations.Add(new SelectedRegistrationDTO() { RegId = dto.Id, RegCode = dto.Registration });

            }
        }

        private void ripoSlukEditFrom_Popup(object sender, EventArgs e)
        {
            IPopupControl popupControl = sender as IPopupControl;
            PopupSearchLookUpEditForm f = popupControl.PopupWindow as PopupSearchLookUpEditForm;
            if (f != null)
            {
                SearchEditLookUpPopup popup = f.ActiveControl as SearchEditLookUpPopup;
                LayoutControlItem clearBtn = (LayoutControlItem)popup.lcgAction.Items[0];

                if (clearBtn != null)
                {
                    clearBtn.Control.Text = "Clear";
                    clearBtn.Control.Click += clearButton_Click;

                }

                LayoutControlItem addBtn = (LayoutControlItem)popup.lcgAction.Items[1];
                if (addBtn != null)
                {
                    addBtn.Control.Text = "GO";
                }

            }
        }

        private void ShowAttachment()
        {

        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            try
            {
                int[] selectedRows = gvFromOthers.GetSelectedRows();

                gvFromOthers.BeginUpdate();
                for (
                    int i = 0; i < selectedRows.Length; i++)
                {
                    gvFromOthers.UnselectRow(selectedRows[i]);

                }

                gvFromOthers.EndUpdate();
                gvFromOthers.RefreshData();
                _selectedFromRegistrations.Clear();
            }
            catch (Exception ex)
            {

            }

        }


        CurrencyDTO Birr { get; set; }
        private bool InitializeData()
        {
            try
            {

                sleCurrency.DataSource = LocalBuffer.LocalBuffer.CurrencyBufferList;
                Birr = LocalBuffer.LocalBuffer.CurrencyBufferList.FirstOrDefault(x => x.Description == "Birr");

                if (Birr != null)
                {
                    beiCurrency.EditValue = Birr.Id;
                }
                else if (LocalBuffer.LocalBuffer.CurrencyBufferList != null && LocalBuffer.LocalBuffer.CurrencyBufferList.Count > 0)
                {
                    beiCurrency.EditValue = LocalBuffer.LocalBuffer.CurrencyBufferList.FirstOrDefault().Id;
                }


                if (registrationExt == null) return false;

                // Progress_Reporter.Show_Progress("Please Wait...");

                if (IsBillTransfer)
                {

                    //current tax rate
                    TaxDTO tax = UIProcessManager.GetTaxById(CNETConstantes.VAT);
                    _taxRate = tax == null ? 15 : (decimal)tax.Amount;

                    //Dicount
                    _discount = UIProcessManager.GetDiscount(RegistrationExt.Id, CNETConstantes.CREDIT_NOTE_VOUCHER, null);

                    statusLabel.Text = "Total Room Charge = 0.0     Total Extra Bills = 0.0    Total Payments = 0.0    TOTAL SELECTED = 0.0";

                    //XtraMessageBox.Show("Note: Bill transfer requires workflow definition for Daily Room Charge, Credit Sales, Cash Sales, Credit Note, Paid out, Cash Reciept and Refund Vouchers", "CNET_v2016", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    // Progress_Reporter.Show_Progress("Loading In-house Guests..");

                    //load in-house guests
                    DateTime? CurrentTime = UIProcessManager.GetServiceTime();
                    if (CurrentTime != null)
                    {
                        if (regListVM != null)
                            regListVM.Clear();

                        regListVM = UIProcessManager.GetRegistrationViewModelData(null, null, CNETConstantes.CHECKED_IN_STATE, SelectedHotelcode);
                    }

                    slukDestination.DataSource = regListVM != null ? regListVM.Where(x => x.Id != registrationExt.Id).ToList() : null;
                    ripoSlukEditFrom.DataSource = regListVM != null ? regListVM.Where(x => x.Id != registrationExt.Id).ToList() : null;
                    gvFromOthers.RefreshData();




                }

                //populate window lookups
                List<SystemConstantDTO> windowsList = LocalBuffer.LocalBuffer.SystemConstantDTOBufferList.Where(l => l.Category == CNETConstantes.SPLIT_WINDOWS && l.IsActive).ToList();
                if (windowsList != null && windowsList.Count != 0)
                {
                    windowsList.Insert(0, new SystemConstantDTO() { Id = 0, Description = "All" });
                    lukEditWindow.DataSource = windowsList;

                    //var lukDef = windowsList.FirstOrDefault(l => l.isDefault);
                    //if (lukDef != null)
                    //{
                    //    beiWindow.EditValue = lukDef.code;
                    //}
                    if (DefaultWindow == -1)
                        beiWindow.EditValue = windowsList[0].Id;
                    else
                    {
                        var win = windowsList.FirstOrDefault(w => w.Value == DefaultWindow.ToString());
                        beiWindow.EditValue = win.Id;
                    }

                }


                //getting guest's fisical Transaction fs number
                List<VwTransactionReferenceFSnumberViewDTO> fsList = UIProcessManager.GetTransactionReferenceFSnumberViewById(RegistrationExt.Id);
                if (fsList != null)
                {

                    List<string> addedFs = new List<string>();
                    foreach (var fse in fsList)
                    {
                        if (fse.note != "check_out") continue; //only show check-out fs numbers
                        if (!addedFs.Contains(fse.FsNumber))
                        {
                            addedFs.Add(fse.FsNumber);
                        }
                    }

                    string fsToDisplay = "";
                    foreach (var fs in addedFs)
                    {
                        if (addedFs.Count > 1)
                        {
                            if (string.IsNullOrEmpty(fsToDisplay))
                            {
                                fsToDisplay = fsToDisplay + "" + fs;
                            }
                            else
                            {
                                fsToDisplay = fsToDisplay + ",   " + "" + fs;
                            }
                        }
                        else
                        {
                            fsToDisplay = fsToDisplay + "" + fs;
                        }
                    }
                    teFsNum.Text = fsToDisplay;
                }

                // getting other consignee's tin number 
                if (RegistrationExt.CompanyId != null)
                {

                    ConsigneeDTO CompanyConsignee = LocalBuffer.LocalBuffer.AllCustomerConsigneeViewlist.FirstOrDefault(x => x.Id == RegistrationExt.CompanyId.Value);
                    if (CompanyConsignee != null && !string.IsNullOrEmpty(CompanyConsignee.Tin))
                    {

                        teTIN.Text = "  " + CompanyConsignee.Tin;
                    }
                }

                ////CNETInfoReporter.Hide();

                //   PopulateGrids();
                //if (!PopulateGrids())
                //{
                //   ////CNETInfoReporter.Hide(); 
                //    return false;

                //}
                ////CNETInfoReporter.Hide();
                return true;
            }
            catch (Exception ex)
            {
                ////CNETInfoReporter.Hide();
                XtraMessageBox.Show("Error in initializing folio. DETAIL: " + ex.Message, "CNET_v2016", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return false;
            }
        }

        //private void PopulateRegListVM(List<RegistrationDocumentDTO> registrationDocumentBrowser)
        //{
        //    regListVM.Clear();

        //    foreach (RegistrationDocumentDTO regDocBro in registrationDocumentBrowser)
        //    {
        //        //string customer = "";
        //        var rd = new RegistrationListVM();
        //        rd.Registration = regDocBro.code;
        //        rd.Consignee = regDocBro.consignee;
        //        rd.RoomType = regDocBro.roomType;
        //        rd.RoomTypeDescription = regDocBro.RoomTypeDescription;
        //        rd.Market = regDocBro.Market;
        //        rd.Color = regDocBro.color;
        //        // rd.RegExtCode = regDocBro.RegistrationExtension;
        //        if (regDocBro.arrivalDate != null)
        //        {
        //            rd.RegistrationDate = regDocBro.arrivalDate;
        //            if (regDocBro.roomCount != null)
        //            {
        //                rd.NoOfRoom = regDocBro.roomCount;
        //            }
        //            //if (!string.IsNullOrEmpty(regDocBro.resType))
        //            //{
        //            //    rd.ResType = UIProcessManager.SelectLookup(regDocBro.resType).description;
        //            //}
        //            if (regDocBro.roomCount > 1)
        //            {
        //                rd.Room = "#" + regDocBro.roomCount;
        //            }
        //            else
        //            {
        //                rd.Room = regDocBro.RoomNumber;
        //            }

        //            rd.Customer = regDocBro.name;
        //            if (regDocBro.tradeName != null)
        //            {
        //                rd.Company = regDocBro.requiredGSL == CNETConstantes.REQ_GSL_COMPANY ? regDocBro.tradeName : "";
        //            }
        //            else
        //            {
        //                rd.Customer = regDocBro.name;
        //            }
        //            if (regDocBro.arrivalDate != null)
        //            {
        //                rd.Arrival = Convert.ToDateTime(regDocBro.arrivalDate);
        //            }
        //        }
        //        if (regDocBro.departureDate != null)
        //        {
        //            rd.Departure = Convert.ToDateTime(regDocBro.departureDate);
        //        }
        //        rd.Payment = regDocBro.PaymentMethod;
        //        rd.lastState = regDocBro.foStatus;
        //        if (!regListVM.Contains(rd))
        //            regListVM.Add(rd);
        //    }
        //}

        public void AdjustGrid()
        {
            //For Room Charges Grid
            gvRoomCharges.BeginUpdate();
            GridViewInfo info = gvRoomCharges.GetViewInfo() as GridViewInfo;
            int height = 28;
            GridRowInfo rInfo = info.RowsInfo.FindRow(0);

            if (rInfo != null)
            {
                //height = rInfo.Bounds.Height;
                height = (height * gvRoomCharges.RowCount) + (2 * height);
                if (height < 65) height = 65;
                lc_roomCharges.ControlMinSize = new Size(gcRoomCharges.Width, height);

                gvRoomCharges.LayoutChanged();

            }
            else
            {
                lc_roomCharges.ControlMinSize = new Size(gcExtraBills.Width, 100);
                gvRoomCharges.LayoutChanged();
            }

            gvRoomCharges.EndUpdate();

            // For ExtraBills Grid
            gvExtraBills.BeginUpdate();
            GridViewInfo infoExtra = gvExtraBills.GetViewInfo() as GridViewInfo;
            int heightExtra = 28;
            GridRowInfo rInfoExtra = infoExtra.RowsInfo.FindRow(0);

            if (rInfoExtra != null)
            {
                //heightExtra = rInfoExtra.Bounds.Height;
                heightExtra = (heightExtra * gvExtraBills.RowCount) + 2 * heightExtra;
                if (heightExtra < 65) heightExtra = 65;
                lc_extraBills.ControlMinSize = new Size(gcExtraBills.Width, heightExtra);

                gvExtraBills.LayoutChanged();
            }
            else
            {
                lc_extraBills.ControlMinSize = new Size(gcExtraBills.Width, 100);
                gvExtraBills.LayoutChanged();
            }

            gvExtraBills.EndUpdate();

            //For Payment History Grid

            gvPaymentHistory.BeginUpdate();
            GridViewInfo infoPayHist = gvPaymentHistory.GetViewInfo() as GridViewInfo;
            int heightPayHist = 28;
            GridRowInfo rInfoPayHist = infoPayHist.RowsInfo.FindRow(0);

            if (rInfoPayHist != null)
            {
                //heightPayHist = rInfoPayHist.Bounds.Height;
                heightPayHist = (heightPayHist * gvPaymentHistory.RowCount) + 2 * heightPayHist;
                if (heightPayHist < 65) heightPayHist = 65;
                lc_paymentHistry.ControlMinSize = new Size(gcPaymentHistory.Width, heightPayHist);

                gvPaymentHistory.LayoutChanged();
            }
            else
            {
                lc_paymentHistry.ControlMinSize = new Size(gcExtraBills.Width, 100);
                gvPaymentHistory.LayoutChanged();
            }

            gvPaymentHistory.EndUpdate();


            gvRoomCharges.BestFitColumns();
            gvExtraBills.BestFitColumns();
            gvPaymentHistory.BestFitColumns();
        }
        GuestLedgerDTO gLedger { get; set; }
        private bool PopulateGridsolddd()
        {
            try
            {
                ResetFields();

                if (IsBillTransfer)
                    statusLabel.Text = "Total Room Charge = 0.0     Total Extra Bills = 0.0    Total Payments = 0.0    TOTAL SELECTED = 0.0";

                if (RegistrationExt == null) return false;

                // Progress_Reporter.Show_Progress("Loading data...");

                int? window = beiWindow.EditValue == null ? null : Convert.ToInt32(beiWindow.EditValue);

                gLedger = UIProcessManager.GetGuestLedger(RegistrationExt.Id,
                     RegistrationExt.Arrival.Date, RegistrationExt.Departure.Date, RegistrationExt.Room, window);

                if (gLedger == null)
                {
                    ////CNETInfoReporter.Hide();
                    //SystemMessage.ShowModalInfoMessage("Unable to get guest ledger information", "ERROR");
                    return false;
                }

                teSubTotal.Text = gLedger.TotalRoomCharge.ToString("#,##0.00;(#,##0.00)");
                teSerCharge.Text = gLedger.RoomServiceCharge.ToString("#,##0.00;(#,##0.00)");
                teVAT.Text = gLedger.VAT.ToString("#,##0.00;(#,##0.00)");
                teGrandTotal.Text = gLedger.GrandTotal.ToString("#,##0.00;(#,##0.00)");
                tediscount.Text = gLedger.Discount.ToString("#,##0.00;(#,##0.00)");

                if (gLedger.RoomCharges != null)
                    gLedger.RoomCharges = gLedger.RoomCharges.OrderBy(x => x.date).ToList();

                gcRoomCharges.DataSource = gLedger.RoomCharges;
                gcRoomCharges.RefreshDataSource();
                gvRoomCharges.RefreshData();

                //gvRoomCharges.BestFitColumns();

                if (gLedger.ExtraBills != null)
                    gLedger.ExtraBills = gLedger.ExtraBills.OrderBy(x => x.date).ToList();

                teTotalBill.Text = gLedger.TotalExtraBill.ToString("#,##0.00;(#,##0.00)");
                gcExtraBills.DataSource = gLedger.ExtraBills;
                gcExtraBills.RefreshDataSource();
                gvExtraBills.RefreshData();
                //gvExtraBills.BestFitColumns();


                if (gLedger.PaymentHistoryList != null)
                    gLedger.PaymentHistoryList = gLedger.PaymentHistoryList.OrderBy(x => x.date).ToList();

                teTotalPaid.Text = gLedger.TotalPaid.ToString("#,##0.00;(#,##0.00)");
                gcPaymentHistory.DataSource = gLedger.PaymentHistoryList;
                gcPaymentHistory.RefreshDataSource();
                gvPaymentHistory.RefreshData();
                //gvPaymentHistory.BestFitColumns();




                teTotalCredit.Text = gLedger.TotalCredit.ToString("#,##0.00;(#,##0.00)");
                teRefund.Text = gLedger.Refund.ToString("#,##0.00;(#,##0.00)");
                teRemainingBalance.Text = gLedger.RemainingBalanceFormated;
                ////CNETInfoReporter.Hide();
                return true;
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Error in populating data. DETAIL::" + ex.Message, "CNET_v2016", MessageBoxButtons.OK, MessageBoxIcon.Error);

                ////CNETInfoReporter.Hide();
                return false;
            }
        }


        private bool PopulateGrids()
        {
            try
            {

                bool CalculateOtherRate = false;
                if (beiCurrency.EditValue != null && Convert.ToInt32(beiCurrency.EditValue) != Birr.Id)
                {
                    CalculateOtherRate = true;
                }


                ResetFields();

                if (IsBillTransfer)
                    statusLabel.Text = "Total Room Charge = 0.0     Total Extra Bills = 0.0    Total Payments = 0.0    TOTAL SELECTED = 0.0";

                if (RegistrationExt == null) return false;

                // Progress_Reporter.Show_Progress("Loading data...");

                int? window = beiWindow.EditValue == null ? null : Convert.ToInt32(beiWindow.EditValue);

                Progress_Reporter.Show_Progress("Getting Guest Ledger Data", "Please Wait ........");

                gLedger = UIProcessManager.GetGuestLedger(RegistrationExt.Id,
                     RegistrationExt.Arrival.Date, RegistrationExt.Departure.Date, RegistrationExt.Room, window == 0 ? null : window);
                Progress_Reporter.Close_Progress();
                if (gLedger == null)
                {
                    //CNETInfoReporter.Hide();
                    //SystemMessage.ShowModalInfoMessage("Unable to get guest ledger information", "ERROR");
                    return false;
                }
            
                if (CalculateOtherRate && defaultCurrency != null)
                {


                    List<ExchangeRateDTO> selectedExchangeRate = ExchangeRateList.Where(x => x.FromCurrency == Convert.ToInt32(beiCurrency.EditValue) &&( x.ToCurrency == defaultCurrency || x.ToCurrency == 0)).ToList();
                    if (selectedExchangeRate != null && selectedExchangeRate.Count > 0)
                    {
                        CalculateExchageRate();
                    }
                }


                if (gLedger.RoomCharges != null)
                    gLedger.RoomCharges = gLedger.RoomCharges.OrderBy(x => x.date).ToList();
                gcRoomCharges.DataSource = gLedger.RoomCharges;
                gcRoomCharges.RefreshDataSource();
                gvRoomCharges.RefreshData();



                teSubTotal.Text = gLedger.TotalRoomCharge.ToString("#,##0.00;(#,##0.00)");
                teSerCharge.Text = gLedger.RoomServiceCharge.ToString("#,##0.00;(#,##0.00)");
                teVAT.Text = gLedger.VAT.ToString("#,##0.00;(#,##0.00)");
                teGrandTotal.Text = gLedger.GrandTotal.ToString("#,##0.00;(#,##0.00)");
                tediscount.Text = gLedger.Discount.ToString("#,##0.00;(#,##0.00)");


                //gvRoomCharges.BestFitColumns();

                if (gLedger.ExtraBills != null)
                    gLedger.ExtraBills = gLedger.ExtraBills.OrderBy(x => x.date).ToList();
                teTotalBill.Text = gLedger.TotalExtraBill.ToString("#,##0.00;(#,##0.00)");
                gcExtraBills.DataSource = gLedger.ExtraBills;
                gcExtraBills.RefreshDataSource();
                gvExtraBills.RefreshData();
                //gvExtraBills.BestFitColumns();





                if (gLedger.PaymentHistoryList != null)
                    gLedger.PaymentHistoryList = gLedger.PaymentHistoryList.OrderBy(x => x.date).ToList();
                teTotalPaid.Text = gLedger.TotalPaid.ToString("#,##0.00;(#,##0.00)");
                gcPaymentHistory.DataSource = gLedger.PaymentHistoryList;
                gcPaymentHistory.RefreshDataSource();
                gvPaymentHistory.RefreshData();
                //gvPaymentHistory.BestFitColumns();



                teTotalCredit.Text = gLedger.TotalCredit.ToString("#,##0.00;(#,##0.00)");
                teRefund.Text = gLedger.Refund.ToString("#,##0.00;(#,##0.00)");
                teRemainingBalance.Text = gLedger.RemainingBalance.ToString("#,##0.00;(#,##0.00)");

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

        private void CalculateExchageRate()
        {
            Progress_Reporter.Show_Progress("Calculating Exchage Rate !!", "Please Wait ........");
            if (gLedger.RoomCharges != null)
            {
                foreach (RoomChargesDTO RoomCharge in gLedger.RoomCharges)
                {
                    decimal rate = GetExchangeRate(Convert.ToInt32(beiCurrency.EditValue), defaultCurrency, RoomCharge.date);
                    RoomCharge.amount = Math.Round((RoomCharge.amount / rate), 2);
                    RoomCharge.serviceCharge = Math.Round((RoomCharge.serviceCharge / rate), 2);

                }
            }
            TaxDTO VATTAX = LocalBuffer.LocalBuffer.TaxBufferList.FirstOrDefault(x => x.Id == CNETConstantes.VAT);
            gLedger.TotalRoomCharge = gLedger.RoomCharges.Sum(x => x.amount);
            gLedger.RoomServiceCharge = gLedger.RoomCharges.Sum(x => x.serviceCharge);
            gLedger.VAT = Convert.ToDecimal(Convert.ToDecimal(gLedger.TotalRoomCharge + gLedger.RoomServiceCharge) * Convert.ToDecimal(VATTAX.Amount / 100));
            gLedger.GrandTotal = gLedger.TotalRoomCharge + gLedger.RoomServiceCharge + gLedger.VAT;
            gLedger.Discount = gLedger.Discount;

            decimal totalPaid = 0;
            decimal Refund = 0;
            if (gLedger.ExtraBills != null)
            {
                foreach (ExtraBillsDTO ExtraBill in gLedger.ExtraBills)
                {
                    decimal rate = GetExchangeRate(Convert.ToInt32(beiCurrency.EditValue), defaultCurrency, ExtraBill.date);
                    ExtraBill.subTotal = Math.Round((ExtraBill.subTotal / rate), 2);
                    ExtraBill.VAT = Math.Round((ExtraBill.VAT / rate), 2);
                    ExtraBill.discount = Math.Round((ExtraBill.discount / rate), 2);
                    ExtraBill.serCharge = Math.Round((ExtraBill.serCharge / rate), 2);
                    ExtraBill.grandTotal = Math.Round((ExtraBill.grandTotal / rate), 2);
                    foreach (DailyLineItemDTO line in ExtraBill.lineItems)
                    {
                        line.totalAmunt = Math.Round((line.totalAmunt.Value / rate), 2);
                        line.unitAmount = Math.Round((line.unitAmount.Value / rate), 2);
                    }




                }
            }

            gLedger.TotalExtraBill = gLedger.ExtraBills.Sum(x => x.grandTotal);


            if (gLedger.PaymentHistoryList != null)
            {
                foreach (PaymentHistoryDTO PaymentHistory in gLedger.PaymentHistoryList)
                {
                    decimal rate = GetExchangeRate(Convert.ToInt32(beiCurrency.EditValue), defaultCurrency, PaymentHistory.date);
                    PaymentHistory.amount = Math.Round((PaymentHistory.amount.Value / rate), 2);


                    if ((PaymentHistory.voucherDef == CNETConstantes.CASHRECIPT || PaymentHistory.voucherDef == CNETConstantes.PAID_OUT_VOUCHER))
                    {
                        totalPaid += PaymentHistory.amount.Value;
                    }
                    else if (PaymentHistory.voucherDef == CNETConstantes.REFUND)
                    {
                        Refund += PaymentHistory.amount.Value;
                    }

                }
            }
            gLedger.TotalPaid = totalPaid;
            gLedger.TotalPaid = gLedger.PaymentHistoryList.Sum(x => x.amount.Value);

            gLedger.TotalCredit = Math.Round(gLedger.GrandTotal + gLedger.TotalExtraBill, 2);
            gLedger.Refund = Refund;
            gLedger.RemainingBalance = (gLedger.TotalCredit - gLedger.TotalPaid + gLedger.Refund);

            Progress_Reporter.Close_Progress();
        }

        public void ExpandAllRows(GridView View)
        {
            View.BeginUpdate();
            try
            {
                int dataRowCount = View.DataRowCount;
                for (int rHandle = 0; rHandle < dataRowCount; rHandle++)
                    View.SetMasterRowExpanded(rHandle, true);
            }
            finally
            {
                View.EndUpdate();
            }
        }

        public void CollapseAllRows(GridView View)
        {
            View.BeginUpdate();
            try
            {
                int dataRowCount = View.DataRowCount;
                for (int rHandle = 0; rHandle < dataRowCount; rHandle++)
                    View.SetMasterRowExpanded(rHandle, false);


            }
            finally
            {
                View.EndUpdate();
            }
        }

        private void UnselectAllRows(GridView view)
        {
            for (int i = 0; i < view.RowCount; i++)
            {
                view.UnselectRow(i);
            }
        }

        private void GetTotalRoomCharge(decimal subtotal, decimal totalServiceCharge)
        {
            decimal vat = (subtotal + totalServiceCharge - _discount) * _taxRate / 100;
            _totalRoomCharge = subtotal + totalServiceCharge - _discount + vat;
        }

        private void GetSelectedRoomCharges()
        {
            _selectedRoomCharges.Clear();
            decimal totalRC = 0;
            decimal totalSC = 0;
            int[] selectedList = gvRoomCharges.GetSelectedRows();
            if (selectedList == null && selectedList.Length <= 0) return;
            for (int i = 0; i < selectedList.Length; i++)
            {
                RoomChargesDTO dto = gvRoomCharges.GetRow(selectedList[i]) as RoomChargesDTO;
                if (dto != null)
                {
                    _selectedRoomCharges.Add(dto);
                    totalRC = totalRC + dto.amount;
                    totalSC = totalSC + dto.serviceCharge;
                }
            }

            GetTotalRoomCharge(totalRC, totalSC);

        }

        private void GetSelectedExtraCharges()
        {
            _selectedExtras.Clear();
            _totalExtra = 0;
            int[] selectedList = gvExtraBills.GetSelectedRows();
            if (selectedList == null && selectedList.Length <= 0) return;
            for (int i = 0; i < selectedList.Length; i++)
            {
                ExtraBillsDTO dto = gvExtraBills.GetRow(selectedList[i]) as ExtraBillsDTO;
                if (dto != null)
                {
                    _selectedExtras.Add(dto);
                    _totalExtra = _totalExtra + dto.grandTotal;
                }
            }

        }

        private void GetSelectedPayments()
        {
            _selectedPayments.Clear();
            _totalPayments = 0;
            int[] selectedList = gvPaymentHistory.GetSelectedRows();
            if (selectedList == null && selectedList.Length <= 0) return;
            for (int i = 0; i < selectedList.Length; i++)
            {
                PaymentHistoryDTO dto = gvPaymentHistory.GetRow(selectedList[i]) as PaymentHistoryDTO;
                if (dto != null)
                {
                    _selectedPayments.Add(dto);
                    if (dto.voucherDef == CNETConstantes.REFUND || (dto.voucherDef == CNETConstantes.CREDIT_NOTE_VOUCHER && _totalRoomCharge > 0))
                    {
                        continue;
                    }
                    if (dto.voucherDef == CNETConstantes.CASHRECIPT)
                    {
                        _totalPayments = _totalPayments - dto.amount.Value;

                    }
                    else
                    {
                        _totalPayments = _totalPayments + dto.amount.Value;
                    }
                }
            }

        }

        private bool TransferVoucher(List<TransactionReferenceDTO> traRefList, int voucherNumber, int voucherDef, string remark = null)
        {
            TransactionReferenceDTO tranRef = traRefList.FirstOrDefault(m => m.Referring == voucherNumber);
            if (tranRef != null)
            {

                //check workflow if remark == null, it is transfereing, unless otherwise it is returning
                int? activityDefCode = null;
                int? lastState = null;
                ActivityDefinitionDTO workFlow = UIProcessManager.GetActivityDefinitionBydescriptionandreference((remark == null ? CNETConstantes.LU_ACTIVITY_DEFINATION_TRANSFRED : CNETConstantes.LU_ACTIVITY_BILL_RETURNED), voucherDef).FirstOrDefault();

                if (workFlow != null)
                {

                    activityDefCode = workFlow.Id;
                    lastState = workFlow.State;
                }
                else
                {
                    SystemConstantDTO vd = LocalBuffer.LocalBuffer.VoucherDefinitionBufferlist.FirstOrDefault(x => x.Id == voucherDef);
                    XtraMessageBox.Show("Please define workflow of " + (remark == null ? " TRANSFERED" : "RETURNED") + " for " + (vd == null ? voucherDef.ToString() : vd.Description), "CNET_v2016", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }



                if (remark == null)
                {
                    //it it transfering
                    tranRef.Remark = tranRef.Referenced.ToString();
                    tranRef.Referenced = Convert.ToInt32(bbiDesination.EditValue);
                    tranRef.ReferencedVoucherDefn = CNETConstantes.REGISTRATION_VOUCHER;
                    tranRef.RelationType = CNETConstantes.DEFAULT_WINDOW;

                }
                else
                {
                    //it is returning 

                    //check if the destination has checked-out or not
                    VoucherDTO vo = UIProcessManager.GetVoucherById(Convert.ToInt32(remark));
                    if (vo == null)
                    {
                        SystemMessage.ShowModalInfoMessage("Unable to get registration data of code = " + remark, "ERROR");
                        return false;

                    }

                    if (vo.LastState == CNETConstantes.CHECKED_OUT_STATE)
                    {
                        SystemMessage.ShowModalInfoMessage("you can't return the bill to already checked out guest! guest code = " + remark, "ERROR");
                        return false;
                    }
                    tranRef.Referenced = Convert.ToInt32(remark);
                    tranRef.Remark = null;

                }


                //update the current tran. reference
                TransactionReferenceDTO isUpdated = UIProcessManager.UpdateTransactionReference(tranRef);

                if (isUpdated == null)
                {
                    SystemMessage.ShowModalInfoMessage("Unable to return bill with  Voucher = " + voucherNumber, "ERROR");
                    return false;
                }


                //saving activity;
                if (activityDefCode > 0)
                {
                    DateTime? CurrentTime = UIProcessManager.GetServiceTime();
                    if (CurrentTime != null)
                    {
                        ActivityDTO activity = ActivityLogManager.SetupActivity(CurrentTime.Value, activityDefCode.Value, CNETConstantes.PMS_Pointer);
                        activity.Reference = tranRef.Referring.Value;
                        ActivityDTO savedactivity = UIProcessManager.CreateActivity(activity);


                        //   string actCode = ActivityLogManager.CommitActivity(activity, activityDefCode, voucherDef, remark == null ? "Transfered from " + tranRef.remark : "Returned from " + RegistrationExt.Registration, CNETConstantes.PMS);
                        if (savedactivity != null)
                        {
                            VoucherDTO vo = UIProcessManager.GetVoucherById(tranRef.Referring.Value);
                            if (vo != null)
                            {
                                vo.LastActivity = savedactivity.Id;
                                vo.LastState = lastState.Value;
                            }
                        }


                        //save activity for registration
                        if (remark != null)
                        {
                            ActivityDTO regActivity = ActivityLogManager.SetupActivity(CurrentTime.Value, activityDefCode.Value, CNETConstantes.PMS_Pointer, string.Format("Bill with voucher # = {0} is transfered to {1}", voucherNumber, tranRef.Referenced));
                            regActivity.Reference = Convert.ToInt32(tranRef.Remark);

                            UIProcessManager.CreateActivity(regActivity);
                        }
                        else
                        {
                            ActivityDTO regActivity = ActivityLogManager.SetupActivity(CurrentTime.Value, activityDefCode.Value, CNETConstantes.PMS_Pointer, string.Format("Bill with voucher # = {0} is returned from {1}", voucherNumber, RegistrationExt.Registration));
                            regActivity.Reference = Convert.ToInt32(tranRef.Referenced);

                            UIProcessManager.CreateActivity(regActivity);
                        }
                    }

                }

            }
            else
            {

            }

            return true;

        }


        private void TransferBillsFromOthers()
        {
            try
            {
                int totalTask = _selectedFromRegistrations.Count;
                List<string> logMessages = new List<string>();
                List<string> failedRegList = new List<string>();
                List<SelectedRegistrationDTO> RegistrationUpdate = new List<SelectedRegistrationDTO>();
                // RegistrationUpdate.Add(registrationExt.Id);
                RegistrationUpdate.Add(new SelectedRegistrationDTO { RegId = registrationExt.Id, RegCode = registrationExt.Registration });

                for (int i = 0; i < totalTask; i++)
                {
                    SelectedRegistrationDTO regCode = _selectedFromRegistrations.ElementAt(i);
                    if (RegistrationUpdate.FirstOrDefault(x => x.RegId == regCode.RegId) == null)
                    {
                        RegistrationUpdate.Add(regCode);
                    }
                    // Progress_Reporter.Show_Progress(string.Format("transfering from {0}", regCode), "Please Wait", i + 1, totalTask);

                    List<TransactionReferenceDTO> traRefList = UIProcessManager.GetTransactionReferenceByreferenced(regCode.RegId);
                    if (traRefList == null || traRefList.Count == 0)
                    {
                        failedRegList.Add(regCode.RegCode);
                        logMessages.Add(string.Format("Empty transaction record for reg. number = {0}", regCode));
                        continue;
                    }

                    foreach (var tranRef in traRefList)
                    {
                        //If the bills are already transfered, continue
                        if (!string.IsNullOrEmpty(tranRef.Remark) && tranRef.Remark != RegistrationExt.Registration)
                        {
                            failedRegList.Add(regCode.RegCode);
                            logMessages.Add(string.Format("The bill {0} is already transfered from {1}", tranRef.Referring, tranRef.Remark));
                            continue;
                        }


                        VoucherDTO vo = UIProcessManager.GetVoucherById(tranRef.Referring.Value);
                        if (vo == null)
                        {
                            logMessages.Add(string.Format("unable to get voucher record for voucher. number = {0} and reg. number = {1}", tranRef.Referring, regCode));
                            continue;
                        }

                        //check voucher definition of the voucher
                        int[] allowedVouchers = {CNETConstantes.DAILY_ROOM_CHARGE_VOUCHER, CNETConstantes.CREDIT_NOTE_VOUCHER,
                                                    CNETConstantes.CREDITSALES,
                                                    CNETConstantes.CASH_SALES,
                                                    CNETConstantes.PAID_OUT_VOUCHER,
                                                    CNETConstantes.REFUND,
                                                    CNETConstantes.CASHRECIPT };
                        if (!allowedVouchers.Contains(vo.Definition))
                        {
                            continue;
                        }

                        //check workflow if remark == null, it is transfereing, unless otherwise it is returning
                        int? activityDefCode = null;
                        int? lastState = null;
                        ActivityDefinitionDTO workFlow = UIProcessManager.GetActivityDefinitionBydescriptionandreference(CNETConstantes.LU_ACTIVITY_DEFINATION_TRANSFRED, vo.Definition).FirstOrDefault();

                        if (workFlow != null)
                        {

                            activityDefCode = workFlow.Id;
                            lastState = workFlow.State;
                        }
                        else
                        {
                            var voDef = LocalBuffer.LocalBuffer.VoucherDefinitionBufferlist.FirstOrDefault(v => v.Id == vo.Definition);
                            logMessages.Add(string.Format("unable to get workflow definition of TRANSFERED for voucher. definition = {0}", (voDef == null ? vo.Definition.ToString() : voDef.Description)));
                            continue;
                        }


                        //update transaction reference
                        tranRef.Remark = tranRef.Referenced.ToString();
                        tranRef.Referenced = RegistrationExt.Id;
                        tranRef.RelationType = CNETConstantes.DEFAULT_WINDOW;


                        TransactionReferenceDTO flag = UIProcessManager.UpdateTransactionReference(tranRef);
                        if (flag == null)
                        {
                            logMessages.Add(string.Format("Bill transfer is not successfull for voucher. number = {0} and reg. number = {1}", tranRef.Referring, regCode));
                            continue;
                        }

                        //Save Activity and Update Voucher's last state and activity
                        DateTime? CurrentTime = UIProcessManager.GetServiceTime();
                        if (CurrentTime != null)
                        {
                            ActivityDTO activity = ActivityLogManager.SetupActivity(CurrentTime.Value, activityDefCode.Value, CNETConstantes.PMS_Pointer, "Transfered from " + tranRef.Remark);
                            activity.Reference = tranRef.Referring.Value;
                            ActivityDTO Savedactivity = UIProcessManager.CreateActivity(activity);
                            if (Savedactivity != null)
                            {
                                if (vo != null)
                                {
                                    vo.LastActivity = Savedactivity.Id;
                                    vo.LastState = lastState.Value;
                                    UIProcessManager.UpdateVoucher(vo);
                                }
                            }
                            //save activity for registration
                            activity = ActivityLogManager.SetupActivity(CurrentTime.Value, activityDefCode.Value, CNETConstantes.PMS_Pointer, string.Format("Bill with voucher # = {0} is transfered to {1}", vo.Code, tranRef.Referenced));
                            activity.Reference = Convert.ToInt32(tranRef.Remark);
                            UIProcessManager.CreateActivity(activity);
                        }

                    }

                }//end of for loop

                if (failedRegList.Count > 0 || logMessages.Count > 0)
                {
                    frmLogBillTransfer frmLog = new frmLogBillTransfer(logMessages, failedRegList);
                    frmLog.ShowDialog();
                }


                //foreach (string regcode in RegistrationUpdate)
                //{
                //    RegistrationList.SynchronizeRegistration(regcode);
                //}
                SystemMessage.ShowModalInfoMessage("Bill transfer is successfull!", "MESSAGE");
                ////CNETInfoReporter.Hide();

                //Refresh Folio
                PopulateGrids();
                AdjustGrid();


            }
            catch (Exception ex)
            {
                ////CNETInfoReporter.Hide();
                SystemMessage.ShowModalInfoMessage("Error has occurred during transfering bills. DETAIL:: " + ex.Message, "ERROR");
            }
        }

        //check selection contains only transfered bills (for returning)
        private bool IsSelectionContainsOnlyTransfered()
        {
            bool isTransfered = true;
            foreach (var item in _selectedRoomCharges)
            {

                if (string.IsNullOrEmpty(item.remark))
                {
                    isTransfered = false;

                    break;
                }
            }
            if (isTransfered)
            {
                foreach (var item in _selectedExtras)
                {
                    if (string.IsNullOrEmpty(item.remark))
                    {
                        isTransfered = false;
                        break;
                    }
                }
            }

            if (isTransfered)
            {
                foreach (var item in _selectedPayments)
                {
                    if (string.IsNullOrEmpty(item.remark))
                    {
                        isTransfered = false;
                        break;
                    }
                }
            }

            return isTransfered;


        }


        //check selection contains non-transfered bills (for Transfering)
        private bool IsSelectionContainsOnlyNonTransfered()
        {

            bool isTransfered = true;
            foreach (var item in _selectedRoomCharges)
            {

                if (!string.IsNullOrEmpty(item.remark))
                {
                    isTransfered = false;
                    break;
                }

                if (item.tranRefValue == 1)
                {
                    isTransfered = false;
                    break;
                }
            }
            if (isTransfered)
            {
                foreach (var item in _selectedExtras)
                {
                    if (!string.IsNullOrEmpty(item.remark))
                    {
                        isTransfered = false;
                        break;
                    }
                }
            }

            if (isTransfered)
            {
                foreach (var item in _selectedPayments)
                {
                    if (!string.IsNullOrEmpty(item.remark))
                    {
                        isTransfered = false;
                        break;
                    }
                }
            }

            return isTransfered;


        }

        private void ResetFields()
        {
            gcRoomCharges.DataSource = null;
            gcExtraBills.DataSource = null;
            gcPaymentHistory.DataSource = null;

            gvRoomCharges.RefreshData();
            gvExtraBills.RefreshData();
            gvPaymentHistory.RefreshData();

            teSubTotal.Text = "0.0";
            tediscount.Text = "0.0";
            teSerCharge.Text = "0.0";
            teVAT.Text = "0.0";
            teGrandTotal.Text = "0.0";
            teTotalBill.Text = "0.0";
            teTotalCredit.Text = "0.0";
            teTotalPaid.Text = "0.0";
            teRefund.Text = "0.0";
            teRemainingBalance.Text = "0.0";
        }

        #endregion


        #region Event Handlers


        private void repoToggle_Toggled(object sender, EventArgs e)
        {
            var view = sender as ToggleSwitch;
            if (view.IsOn)
            {
                beiFrom.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                bbiDesination.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                beiSelectAll.Enabled = false;
                bbiTransfer.Enabled = false;
                bbiReturn.Enabled = false;
            }
            else
            {
                beiFrom.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                bbiDesination.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                beiSelectAll.Enabled = true;
                bbiTransfer.Enabled = true;
                bbiReturn.Enabled = true;
            }
        }


        private void tcFolio_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {

        }

        private void gvFolio_RowStyle(object sender, RowStyleEventArgs e)
        {
            GridView View = sender as GridView;
            if (e.RowHandle >= 0)
            {
                string category = View.GetRowCellDisplayText(e.RowHandle, View.Columns[3]);
                if (category == "Sunday" || category == "Saturday")
                {
                    e.Appearance.BackColor = Color.Aqua;
                    //  e.Appearance.BackColor2 = Color.SeaShell;
                    e.HighPriority = true;
                }
            }


        }


        private void bbiClose_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Close();
        }

        private void bbiRefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            PopulateGrids();
            AdjustGrid();
        }


        private void bbiPrint_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                //if (tcFolio.SelectedTabPage.Equals(tpGuestFolio))
                //{
                //    GuestFolio report = new GuestFolio();
                //    report = new GuestFolio(gcFolio);
                //    report.FolioParametres(teCustomer.Text, teRegistration.Text, teCompany.Text, "", "", teArrivalDate.Text, teDepartureDate.Text);
                //    report.Watermark.Image = DocumentPrint.Resource1.NonFisical;
                //    report.Watermark.ImageAlign = ContentAlignment.MiddleCenter;
                //    report.Watermark.ImageTiling = false;

                //    //  report.Watermark.ImageViewMode = DevExpress.XtraPrinting.Drawing.ImageViewMode.;
                //    report.Watermark.ImageTransparency = 70;

                //    report.Watermark.PageRange = "all";
                //    report.Watermark.ShowBehind = true;

                //    ReportPrintTool pt = new ReportPrintTool(report);
                //    pt.ShowPreview();
                //}
                if (tcFolio.SelectedTabPage.Equals(tpGuestLedger))
                {
                    ReportGenerator reportGenerator = new ReportGenerator();
                    LedgerObjects lObject = new LedgerObjects()
                    {
                        ArrivalDate = teArrivalDate.Text,
                        CompanyName = teCompany.Text,
                        CustomerName = teCustomer.Text,
                        DepartureDate = teDepartureDate.Text,
                        Discount = tediscount.Text,
                        FsNo = teFsNum.Text,
                        GrandTotal = teGrandTotal.Text,
                        Plan = "",
                        Refund = teRefund.Text,
                        RegistrationNumber = teRegistration.Text,
                        RemainingBalance = teRemainingBalance.Text,
                        ServiceCharge = teSerCharge.Text,
                        SubTotal = teSubTotal.Text,
                        TINNo = teTIN.Text,
                        TotalCredit = teTotalCredit.Text,
                        TotalOtherBill = teTotalBill.Text,
                        TotalPaid = teTotalPaid.Text,
                        Vat = teVAT.Text,
                        ConsigneeId = RegistrationExt.GuestId,
                        ConsigneeUnit = registrationExt.ConsigneeUnit
                    };
                    if (registrationExt.CompanyId != null)
                    {
                        ConsigneeDTO consigneeDTO = LocalBuffer.LocalBuffer.AllCustomerConsigneeViewlist.FirstOrDefault(x => x.Id == registrationExt.CompanyId.Value);
                        if (consigneeDTO != null)
                            lObject.CompanyTin = consigneeDTO.Tin;
                    }
                    reportGenerator.GenerateGuestLedgerReport(gcRoomCharges, gcExtraBills, gcPaymentHistory, lObject);

                }
            }
            catch (Exception ex)
            {
                string innerMsg = ex.InnerException != null ? ex.InnerException.ToString() : "";
                XtraMessageBox.Show("ERROR! " + ex.Message + " INNER:: " + innerMsg, "CNET_v2016", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void bbiPDF_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }

        private void ceExpandAll_EditValueChanged(object sender, EventArgs e)
        {
            if (ceExpandAll.Checked)
            {
                ExpandAllRows(gvExtraBills);
            }
            else
            {
                // gvExtraBills.CollapseAllDetails();
                CollapseAllRows(gvExtraBills);
            }
        }

        private void frmFolio_Load(object sender, EventArgs e)
        {
            if (!InitializeData())
            {
                this.Close();
            }
            AdjustGrid();
        }

        private void gv_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            GridView view = sender as GridView;
            if (e.Column.Caption == "SN")
            {
                if (view.Equals(gvRoomCharges))
                {

                    RoomChargesDTO dto = view.GetRow(e.RowHandle) as RoomChargesDTO;
                    if (dto == null) return;
                    int sn = e.RowHandle + 1;
                    dto.SN = sn;
                    e.DisplayText = sn.ToString();
                }
                else if (view.Equals(gvExtraBills))
                {
                    ExtraBillsDTO dto = view.GetRow(e.RowHandle) as ExtraBillsDTO;
                    if (dto == null) return;
                    int sn = e.RowHandle + 1;
                    dto.SN = sn;
                    e.DisplayText = sn.ToString();

                    if (!string.IsNullOrEmpty(dto.remark))
                    {
                        e.Appearance.BackColor = ColorTranslator.FromHtml("255, 224, 192");
                    }

                }
                else if (view.Equals(gvPaymentHistory))
                {
                    PaymentHistoryDTO dto = view.GetRow(e.RowHandle) as PaymentHistoryDTO;
                    if (dto == null) return;
                    int sn = e.RowHandle + 1;
                    dto.SN = sn;
                    e.DisplayText = sn.ToString();

                }
            }
            if (view.Equals(gvRoomCharges))
            {
                RoomChargesDTO dto = view.GetRow(e.RowHandle) as RoomChargesDTO;
                if (dto == null) return;
                if (dto.tranRefValue == 1)
                {
                    e.Appearance.ForeColor = ColorTranslator.FromHtml("255, 99, 71");
                }
                if (!string.IsNullOrEmpty(dto.remark))
                {
                    //e.Appearance.BackColor = ColorTranslator.FromHtml("255, 224, 192");
                    e.Appearance.BackColor = Color.LightSkyBlue;
                }

            }
            else if (view.Equals(gvExtraBills))
            {
                ExtraBillsDTO dto = view.GetRow(e.RowHandle) as ExtraBillsDTO;
                if (dto == null) return;
                if (!string.IsNullOrEmpty(dto.remark))
                {
                    e.Appearance.BackColor = Color.LightSkyBlue;
                }
            }
            else if (view.Equals(gvPaymentHistory))
            {
                PaymentHistoryDTO dto = view.GetRow(e.RowHandle) as PaymentHistoryDTO;
                if (dto == null) return;
                if (!string.IsNullOrEmpty(dto.remark))
                {
                    //e.Appearance.BackColor = ColorTranslator.FromHtml("255, 224, 192");
                    e.Appearance.BackColor = Color.LightSkyBlue;
                }
            }
        }

        private void gvLineItemDetails_MasterRowExpanded_1(object sender, CustomMasterRowEventArgs e)
        {

        }

        private void gvExtraBills_MasterRowExpanded(object sender, CustomMasterRowEventArgs e)
        {
            // For ExtraBills Grid
            gvExtraBills.BeginUpdate();
            GridViewInfo infoExtra = gvExtraBills.GetViewInfo() as GridViewInfo;

            int heightExtra = 0;
            GridRowInfo rInfoExtra = infoExtra.RowsInfo.FindRow(e.RowHandle);


            if (rInfoExtra != null)
            {
                int oldHeight = lc_extraBills.ControlMinSize.Height;
                int detailHeight = 80;
                int newHeight = detailHeight + oldHeight;
                lc_extraBills.ControlMinSize = new Size(gcExtraBills.Width, newHeight);

                gvExtraBills.LayoutChanged();
            }

            gvExtraBills.EndUpdate();
        }

        private void gvExtraBills_MasterRowCollapsed(object sender, CustomMasterRowEventArgs e)
        {
            // For ExtraBills Grid
            gvExtraBills.BeginUpdate();
            GridViewInfo infoExtra = gvExtraBills.GetViewInfo() as GridViewInfo;

            int heightExtra = 0;
            GridRowInfo rInfoExtra = infoExtra.RowsInfo.FindRow(e.RowHandle);

            if (rInfoExtra != null)
            {
                int oldHeight = lc_extraBills.ControlMinSize.Height;
                int detailHeight = 80;
                int newHeight = (oldHeight - detailHeight);
                if (newHeight < 104) newHeight = 104;
                lc_extraBills.ControlMinSize = new Size(gcExtraBills.Width, newHeight);

                gvExtraBills.LayoutChanged();
            }

            gvExtraBills.EndUpdate();
        }

        private void gv_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            GridView view = sender as GridView;
            decimal totalSelected = 0;
            if (view.Equals(gvRoomCharges))
            {
                GetSelectedRoomCharges();
            }
            else if (view.Equals(gvExtraBills))
            {
                GetSelectedExtraCharges();
            }
            else if (view.Equals(gvPaymentHistory))
            {
                GetSelectedPayments();
            }

            totalSelected = _totalRoomCharge + _totalExtra + _totalPayments;
            statusLabel.Text = "Total Room Charge = " + Math.Round(_totalRoomCharge, 2) + "   Total Extra Bills = " + Math.Round(_totalExtra, 2) + "   Total Payments = " + Math.Round(_totalPayments, 2) +
                "      TOTAL SELECTED = " + Math.Round(totalSelected, 2).ToString("#,##0.00;(#,##0.00)");

            if (!IsSelectionContainsOnlyNonTransfered())
            {
                bbiTransfer.Enabled = false;
                bbiDesination.Enabled = false;

            }
            else
            {
                bbiTransfer.Enabled = true;
                bbiDesination.Enabled = true;
            }

            if (!IsSelectionContainsOnlyTransfered())
            {
                bbiReturn.Enabled = false;
            }
            else
            {
                bbiReturn.Enabled = true;
            }
        }

        private void ceSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            CheckEdit item = sender as CheckEdit;
            if (item == null) return;
            if (item.Checked)
            {
                gvRoomCharges.SelectAll();
                gvExtraBills.SelectAll();
                gvPaymentHistory.SelectAll();
            }
            else
            {
                UnselectAllRows(gvRoomCharges);
                UnselectAllRows(gvExtraBills);
                UnselectAllRows(gvPaymentHistory);
            }
        }

        private void gv_RowStyle(object sender, RowStyleEventArgs e)
        {
            GridView view = sender as GridView;

            if (view.Equals(gvRoomCharges))
            {
                RoomChargesDTO dto = view.GetRow(e.RowHandle) as RoomChargesDTO;
                if (dto == null) return;
                if (dto.tranRefValue == 1)
                {
                    //e.Appearance.ForeColor = ColorTranslator.FromHtml("255, 99, 71");
                }
                if (!string.IsNullOrEmpty(dto.remark))
                {
                    //e.Appearance.BackColor = ColorTranslator.FromHtml("255, 224, 192");

                    //System.Drawing.Font font = e.Appearance.Font;
                    //e.Appearance.Font = new System.Drawing.Font(font.FontFamily, font.Size, FontStyle.Bold);

                }

            }
            else if (view.Equals(gvExtraBills))
            {
                ExtraBillsDTO dto = view.GetRow(e.RowHandle) as ExtraBillsDTO;
                if (dto == null) return;
                if (!string.IsNullOrEmpty(dto.remark))
                {
                    //e.Appearance.BackColor = ColorTranslator.FromHtml("255, 224, 192");
                    //System.Drawing.Font font = e.Appearance.Font;
                    //e.Appearance.Font = new System.Drawing.Font(font.FontFamily, font.Size, FontStyle.Bold);
                }
            }
            else if (view.Equals(gvPaymentHistory))
            {
                PaymentHistoryDTO dto = view.GetRow(e.RowHandle) as PaymentHistoryDTO;
                if (dto == null) return;
                if (!string.IsNullOrEmpty(dto.remark))
                {
                    //e.Appearance.BackColor = ColorTranslator.FromHtml("255, 224, 192");
                    //System.Drawing.Font font = e.Appearance.Font;
                    //e.Appearance.Font = new System.Drawing.Font(font.FontFamily, font.Size, FontStyle.Bold);
                }
            }
        }

        private void gv_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {


            /*GridView view = sender as GridView;
            if (view.Equals(gvRoomCharges))
            {
                RoomChargesDTO dto = view.GetFocusedRow() as RoomChargesDTO;
                if (dto == null) return;
                i*f (!string.IsNullOrEmpty(dto.remark))
                {
                    //view.Appearance.FocusedRow.BackColor = ColorTranslator.FromHtml("255, 224, 192");
                    if (dto.tranRefValue == 1)
                    {
                        view.Appearance.FocusedRow.ForeColor = ColorTranslator.FromHtml("255, 99, 71");
                    }
                    else
                    {
                        view.Appearance.FocusedRow.ForeColor = ColorTranslator.FromHtml("Black");
                    }
                    //System.Drawing.Font font = view.Appearance.FocusedRow.Font;
                    // view.Appearance.FocusedRow.Font = new System.Drawing.Font(font.FontFamily, font.Size, FontStyle.Bold);

                }
                else
                {
                    view.Appearance.FocusedRow.BackColor = ColorTranslator.FromHtml("SkyBlue");
                    if (dto.tranRefValue == 1)
                    {
                        view.Appearance.FocusedRow.ForeColor = ColorTranslator.FromHtml("255, 99, 71");
                    }
                    else
                    {
                        view.Appearance.FocusedRow.ForeColor = ColorTranslator.FromHtml("Black");
                    }
                }

            }
            else if (view.Equals(gvExtraBills))
            {
                ExtraBillsDTO dto = view.GetFocusedRow() as ExtraBillsDTO;
                if (dto == null) return;
                if (!string.IsNullOrEmpty(dto.remark))
                {
                    view.Appearance.FocusedRow.BackColor = ColorTranslator.FromHtml("255, 224, 192");
                    view.Appearance.FocusedRow.ForeColor = ColorTranslator.FromHtml("Black");
                    //System.Drawing.Font font = view.Appearance.FocusedRow.Font;
                    //view.Appearance.FocusedRow.Font = new System.Drawing.Font(font.FontFamily, font.Size, FontStyle.Bold);
                }
                else
                {
                    view.Appearance.FocusedRow.BackColor = ColorTranslator.FromHtml("SkyBlue");
                    view.Appearance.FocusedRow.ForeColor = ColorTranslator.FromHtml("Black");
                }
            }
            else if (view.Equals(gvPaymentHistory))
            {
                PaymentHistoryDTO dto = view.GetFocusedRow() as PaymentHistoryDTO;
                if (dto == null) return;
                if (!string.IsNullOrEmpty(dto.remark))
                {
                    view.Appearance.FocusedRow.BackColor = ColorTranslator.FromHtml("255, 224, 192");
                    view.Appearance.FocusedRow.ForeColor = ColorTranslator.FromHtml("Black");
                    //System.Drawing.Font font = view.Appearance.FocusedRow.Font;
                    //view.Appearance.FocusedRow.Font = new System.Drawing.Font(font.FontFamily, font.Size, FontStyle.Bold);
                }
                else
                {
                    view.Appearance.FocusedRow.BackColor = ColorTranslator.FromHtml("SkyBlue");
                    view.Appearance.FocusedRow.ForeColor = ColorTranslator.FromHtml("Black");
                }
            }
            */


        }

        private void bbiTransfer_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (bbiDesination.EditValue == null)
            {
                XtraMessageBox.Show("You must select a destination first!", "CNET_v2016", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (_selectedRoomCharges.Count == 0 && _selectedPayments.Count == 0 && _selectedExtras.Count == 0)
            {
                XtraMessageBox.Show("Select at least one bill to transfer!", "CNET_v2016", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!IsSelectionContainsOnlyNonTransfered())
            {
                XtraMessageBox.Show("Selection contains already transfered bills!", "CNET_v2016", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DialogResult dRes = XtraMessageBox.Show("Do you want to transfer this bill(s)?", "Bill Transfer", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dRes == System.Windows.Forms.DialogResult.No)
            {
                return;
            }


            try
            {
                // Progress_Reporter.Show_Progress("Please wait..");

                List<TransactionReferenceDTO> traRefList = UIProcessManager.GetTransactionReferenceByreferenced(RegistrationExt.Id);
                if (traRefList == null)
                {
                    ////CNETInfoReporter.Hide();
                    XtraMessageBox.Show("Unable to get transaction reference record!", "CNET_v2016", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                bool flag = true;
                //transfer room charges
                // Progress_Reporter.Show_Progress("Transfering Room Charges");
                foreach (var item in _selectedRoomCharges)
                {

                    flag = TransferVoucher(traRefList, item.voucherId, item.voucherDef.Value);
                }

                //transfer extra bills
                // Progress_Reporter.Show_Progress("Transfering Extra Bills");
                foreach (var item in _selectedExtras)
                {
                    flag = TransferVoucher(traRefList, item.invoiceId.Value, item.voucherDef.Value);
                }


                //transfer payments
                // Progress_Reporter.Show_Progress("Transfering Payments");
                foreach (var item in _selectedPayments)
                {
                    flag = TransferVoucher(traRefList, item.receiptId.Value, item.voucherDef.Value);
                }

                ////CNETInfoReporter.Hide();

                if (flag)
                {
                    XtraMessageBox.Show("Bill transfer is successfull!", "CNET_v2016", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    XtraMessageBox.Show("Bill transfer is done but one or more operation is failed!", "CNET_v2016", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                }

                //refresh goes here
                PopulateGrids();


            }
            catch (Exception ex)
            {
                ////CNETInfoReporter.Hide();
                XtraMessageBox.Show("Error in transfering bill.. DETAIL::" + ex.Message, "CNET_v2016", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        private void bbiReturn_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {


            if (_selectedRoomCharges.Count == 0 && _selectedPayments.Count == 0 && _selectedExtras.Count == 0)
            {
                XtraMessageBox.Show("Select at least one bill to return!", "CNET_v2016", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!IsSelectionContainsOnlyTransfered())
            {
                XtraMessageBox.Show("Selection contains non-transfered bills!", "CNET_v2016", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DialogResult dRes = XtraMessageBox.Show("Do you want to return this bill(s)?", "Bill Return", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dRes == System.Windows.Forms.DialogResult.No)
            {
                return;
            }


            try
            {

                // Progress_Reporter.Show_Progress("Please wait..");

                List<TransactionReferenceDTO> traRefList = UIProcessManager.GetTransactionReferenceByreferenced(RegistrationExt.Id);
                if (traRefList == null)
                {
                    ////CNETInfoReporter.Hide();
                    XtraMessageBox.Show("Unable to get transaction reference record!", "CNET_v2016", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                bool flag = true;
                //transfer room charges
                // Progress_Reporter.Show_Progress("Returning Room Charges");
                foreach (var item in _selectedRoomCharges)
                {

                    flag = TransferVoucher(traRefList, item.voucherId, item.voucherDef.Value, item.remark);
                }

                //transfer extra bills
                // Progress_Reporter.Show_Progress("Returning Extra Bills");
                foreach (var item in _selectedExtras)
                {
                    flag = TransferVoucher(traRefList, item.invoiceId.Value, item.voucherDef.Value, item.remark);
                }


                //transfer payments
                // Progress_Reporter.Show_Progress("Returning Payments");
                foreach (var item in _selectedPayments)
                {
                    flag = TransferVoucher(traRefList, item.receiptId.Value, item.voucherDef.Value, item.remark);
                }

                ////CNETInfoReporter.Hide();

                if (flag)
                {
                    XtraMessageBox.Show("Bill Return is successfull!", "CNET_v2016", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    XtraMessageBox.Show("Bill return is done but one or more operation is failed!", "CNET_v2016", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                }

                //refresh goes here
                PopulateGrids();


            }
            catch (Exception ex)
            {
                ////CNETInfoReporter.Hide();
                XtraMessageBox.Show("Error in returning bill.. DETAIL::" + ex.Message, "CNET_v2016", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        //adjust the widths of the multi-row selector collumns in bill transfer folio view
        private void gcRoomCharges_Load(object sender, EventArgs e)
        {
            if (IsBillTransfer)
            {
                gvRoomCharges.VisibleColumns[0].Width = 30;
                gvExtraBills.VisibleColumns[0].Width = 30;
                gvPaymentHistory.VisibleColumns[0].Width = 30;

            }
        }

        private void bbiPrintRC_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                ReportGenerator reportGenerator = new ReportGenerator();
                LedgerObjects lObject = new LedgerObjects()
                {
                    ArrivalDate = teArrivalDate.Text,
                    CompanyName = teCompany.Text,
                    CustomerName = teCustomer.Text,
                    DepartureDate = teDepartureDate.Text,
                    Discount = tediscount.Text,
                    FsNo = teFsNum.Text,
                    GrandTotal = teGrandTotal.Text,
                    Plan = "",
                    Refund = teRefund.Text,
                    RegistrationNumber = teRegistration.Text,
                    RemainingBalance = teRemainingBalance.Text,
                    ServiceCharge = teSerCharge.Text,
                    SubTotal = teSubTotal.Text,
                    TINNo = teTIN.Text,
                    TotalCredit = teTotalCredit.Text,
                    TotalOtherBill = teTotalBill.Text,
                    TotalPaid = teTotalPaid.Text,
                    Vat = teVAT.Text,
                    ConsigneeId = RegistrationExt.GuestId,
                    ConsigneeUnit = RegistrationExt.ConsigneeUnit,
                    HeaderText = "Guest Ledger (Room Charge Only)",
                    SetWaterMark = true,
                    User = LocalBuffer.LocalBuffer.CurrentLoggedInUser.UserName

                };

                if (registrationExt.CompanyId != null)
                {
                    ConsigneeDTO consigneeDTO = LocalBuffer.LocalBuffer.AllCustomerConsigneeViewlist.FirstOrDefault(x => x.Id == registrationExt.CompanyId.Value);
                    if (consigneeDTO != null)
                        lObject.CompanyTin = consigneeDTO.Tin;
                }

                reportGenerator.GenerateGuestLedgerRoomCharge(gcRoomCharges, lObject);
            }

            catch (Exception ex)
            {
                string innerMsg = ex.InnerException != null ? ex.InnerException.ToString() : "";
                XtraMessageBox.Show("ERROR! " + ex.Message + " INNER:: " + innerMsg, "CNET_v2016", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();

                if (_selectedRoomCharges != null)
                {
                    _selectedRoomCharges.Clear();
                    _selectedRoomCharges = null;
                }

                if (_selectedExtras != null)
                {
                    _selectedExtras.Clear();
                    _selectedExtras = null;
                }


                if (_selectedPayments != null)
                {
                    _selectedPayments.Clear();
                    _selectedPayments = null;
                }



                if (regListVM != null)
                {
                    regListVM.Clear();
                    regListVM = null;
                }


                RegistrationExt = null;
            }
            base.Dispose(disposing);
        }

        private void gcPaymentHistory_Click(object sender, EventArgs e)
        {

        }

        private void beiFrom_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                int count = 0;
                if (_selectedFromRegistrations != null && _selectedFromRegistrations.Count > 0)
                {
                    count = _selectedFromRegistrations.Count;
                    beiFrom.Edit.NullText = string.Format("{0} reg. {1} selected", count, (count > 1 ? "are" : "is"));

                }
                else
                {
                    beiFrom.Edit.NullText = "Select Registrations";

                }
            }
            catch (Exception ex)
            {

            }
        }



        private void sleCurrency_EditValueChanged(object sender, EventArgs e)
        {


        }

        private void beiCurrency_EditValueChanged(object sender, EventArgs e)
        {
            PopulateGrids();

        }

        private decimal GetExchangeRate(int fromCurrency, int? toCurrency, DateTime DateTime)
        {
            decimal exchangeRate = 1;

            var ExchangeRate = ExchangeRateList.OrderByDescending(x => x.Date).FirstOrDefault(r => r.FromCurrency == fromCurrency && (r.ToCurrency == toCurrency || r.ToCurrency == 0) && r.Date.Date == DateTime.Date);

            if (ExchangeRate != null)
            {
                exchangeRate =
                    ExchangeRate.Buying;
            }
            else
            {
                var ExchangeRateLast = ExchangeRateList.OrderByDescending(x => x.Date).FirstOrDefault(r => r.FromCurrency == fromCurrency && (r.ToCurrency == toCurrency || r.ToCurrency == 0) && r.Date.Date < DateTime.Date);
                if (ExchangeRateLast != null)
                {
                    exchangeRate =
                        ExchangeRateLast.Buying;
                }
                else
                {
                    exchangeRate = 1;
                }

            }


            return exchangeRate;
        }

        public int SelectedHotelcode { get; set; }
    }
}
