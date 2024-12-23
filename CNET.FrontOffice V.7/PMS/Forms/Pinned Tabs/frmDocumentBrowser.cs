using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CNET.ERP.Client.Common.UI;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraEditors;
using DevExpress.XtraReports.Design;
using DevExpress.XtraTab;
using DevExpress.XtraGrid.Columns;
using CNET.ERP.Client.UI_Logic.PMS.Forms.Non_Navigatable_Modals;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraBars;
using CNET_V7_Domain.Domain.SettingSchema;
using CNET_V7_Domain.Domain.SecuritySchema;
using CNET_V7_Domain.Domain.ViewSchema;
using CNET_V7_Domain.Misc.PmsView;
using ProcessManager;
using CNET_V7_Domain.Domain.CommonSchema;
using CNET_V7_Domain.Misc.PmsDTO;
using CNET_V7_Domain.Domain.PmsSchema;
using CNET.FrontOffice_V._7.PMS.Common_Classes;
using CNET.FrontOffice_V._7.Forms.Non_Navigatable_Modals;
using CNET.FrontOffice_V._7.Forms;
using CNET_V7_Domain.Domain.TransactionSchema;
using CNET.FrontOffice_V._7.PMS.DTO;
using CNET.POS.DocumentBrowser;
using CNET.Progress.Reporter;
using DocumentPrint;
using CNET_V7_Domain.Domain.ConsigneeSchema;

namespace CNET.FrontOffice_V._7
{
    public partial class frmDocumentBrowser : UILogicBase // XtraForm //UILogicBase
    {
        private bool isPersonLoaded = false;
        private bool isOrganizationLoaded = false;

        //Consignee Documents
        private UCConsigneeDocument consigneeDocument = null;

        // Voucher Documents
        private UCVoucherDocument voucherDocument = null;

        // Load Flags
        private bool flagGuest;
        private bool flagContact;
        private bool flagCompany;
        private bool flagAgent;
        private bool flagGroup;
        private bool flagSource;
        private bool flagCachReceipt;
        private bool flagCreditSales;
        private bool flagCashSales;
        private bool flagCreditNote;
        private bool flagPaidout;
        private bool flagRefund;
        private bool flagDebit;
        private bool flagDrc;
        private bool flagCashSalesSummary;
        private bool flagEventReq;
        private bool flagServiceRequest;
        private bool flagLostAndFound;
        private bool flagDailyRes;
        private bool flagCheckOut;
        private bool flagPackagecons;
        private bool flagLogbook;

        private DXMenuItem[] menuItems;


        public UserDTO CurrentUser { get; set; }
        public DeviceDTO CurrentDevice { get; set; }


        private List<VwRoomPoschargeViewDTO> _roomPosCharges = null;


        private List<VwTravelDetailDTO> _pickupLists;
        private List<VwTravelDetailDTO> _dropoffList;
        private List<VwPackageViewDTO> _pkgAuditViewList = null;

        /******************************* CONSTRUCTOR **********************************/
        public frmDocumentBrowser()
        {
            InitializeComponent();
            InitializeUI();
            InitializeData();

            try
            {
                //  TabSecurity(tcDocumentControl, "Document Browser");

                TabSecurity(tcProfiles, "Profiles Document");
                TabSecurity(tcVouchers, "Transactions Document");
                TabSecurity(tcCustomReport, "Others");
            }
            catch
            {

            }
        }



        #region Security

        public void TabSecurity(DevExpress.XtraTab.XtraTabControl parentTab, String parentName)
        {

            List<String> approvedFunctionalities = LocalBuffer.LocalBuffer.AllAccessMatrixFunctionList.Where(x => x.VisuaCompDesc == "Document Browser").Select(x => x.Description).ToList();

            if (!IsFunctionExists(approvedFunctionalities, parentName))
            {
                parentTab.Enabled = false;
                return;
            }

            foreach (XtraTabPage tabs in parentTab.TabPages)
            {
                if (!IsFunctionExists(approvedFunctionalities, tabs.Text))
                {
                    tabs.PageEnabled = false;
                }
            }
        }

        private static bool IsFunctionExists(List<string> approvedFunctionalities, string selectedName)
        {
            foreach (String str in approvedFunctionalities)
            {
                if (str.ToLower().Trim().Equals(selectedName.ToLower().Trim()))
                {
                    return true;
                }
            }
            return false;
        }

        #endregion

        //Initialize UI
        public void InitializeUI()
        {
            DXMenuItem itemView = new DXMenuItem("View", ItemView_Click);
            menuItems = new DXMenuItem[] { itemView };

            //Discount Reason
            ripLukPackageAudit.Columns.Add(new LookUpColumnInfo("Description", "Package"));
            ripLukPackageAudit.DisplayMember = "Description";
            ripLukPackageAudit.ValueMember = "Id";

        }

        private void ItemView_Click(Object sender, System.EventArgs e)
        {
            VwRoomPoschargeViewDTO model = gvPosCharges.GetFocusedRow() as VwRoomPoschargeViewDTO;
            if (model == null) return;

            DocumentPrint.ReportGenerator reportGenerator = new DocumentPrint.ReportGenerator();
            reportGenerator.GetAttachementReport(model.Referring.Value);
        }


        //Initialize Data
        public void InitializeData()

        {
            Progress_Reporter.Show_Progress("Loading data.", "Please Wait.......");

            DateTime? CurrentDate = UIProcessManager.GetServiceTime();

            if (CurrentDate == null)
            {
                tcProfiles.Enabled = false;
                tcVouchers.Enabled = false;
            }

            beiRoomPOSDate.EditValue = CurrentDate == null ? DateTime.Now : CurrentDate.Value;
            repDateEdit.MaxValue = CurrentDate == null ? DateTime.Now : CurrentDate.Value;


            tcProfiles.SelectedTabPage = tpGuest;
            tcVouchers.SelectedTabPage = tp_CashReceipt;
            PopulateGuest();
            GetHotelData();
            Progress_Reporter.Close_Progress();
        }
        public void GetHotelData()
        {


            reiHotelPosCharge.DisplayMember = "Name";
            reiHotelPosCharge.ValueMember = "Id";
            reiHotelPosCharge.DataSource = (LocalBuffer.LocalBuffer.HotelBranchBufferList.Select(x => new { x.Id, x.Name })).ToList();


            reiHotelTravel.DisplayMember = "Name";
            reiHotelTravel.ValueMember = "Id";
            reiHotelTravel.DataSource = (LocalBuffer.LocalBuffer.HotelBranchBufferList.Select(x => new { x.Id, x.Name })).ToList();


            reiHotelPackage.DisplayMember = "Name";
            reiHotelPackage.ValueMember = "Id";
            reiHotelPackage.DataSource = (LocalBuffer.LocalBuffer.HotelBranchBufferList.Select(x => new { x.Id, x.Name })).ToList();


            beiHotelPosCharge.EditValue = LocalBuffer.LocalBuffer.CurrentDeviceConsigneeUnit;
            beiHotelTravel.EditValue = LocalBuffer.LocalBuffer.CurrentDeviceConsigneeUnit;
            beiHotelPackage.EditValue = LocalBuffer.LocalBuffer.CurrentDeviceConsigneeUnit;

            //reiHotelPosCharge.ReadOnly = !UserHasHotelBranchAccess;
            //reiHotelTravel.ReadOnly = !UserHasHotelBranchAccess;
            //reiHotelPackage.ReadOnly = !UserHasHotelBranchAccess;


        }

        //Initialize Travel Detail
        private void InitializeTravelDetail()
        {
            //Transportation Type
            rilue_transpType.Columns.Add(new LookUpColumnInfo("Description", "Trans. Type"));
            rilue_transpType.DisplayMember = "Description";
            rilue_transpType.ValueMember = "Description";

            // Station
            rilue_station.Columns.Add(new LookUpColumnInfo("Description", "Station"));
            rilue_station.DisplayMember = "Description";
            rilue_station.ValueMember = "Id";


            //Carrier Arrival
            rilue_carrier.Columns.Add(new LookUpColumnInfo("Description", "Station"));
            rilue_carrier.DisplayMember = "Description";
            rilue_carrier.ValueMember = "Id";

            //Company
            GridColumn columnCompany = rislue_company.View.Columns.AddField("code");
            columnCompany.Visible = true;
            columnCompany = rislue_company.View.Columns.AddField("FirstName");
            columnCompany.Visible = true;
            rislue_company.DisplayMember = "FirstName";
            rislue_company.ValueMember = "Id";

            //room type
            rilue_roomTypeLookup.Columns.Add(new LookUpColumnInfo("Description", "Room Types"));
            rilue_roomTypeLookup.DisplayMember = "Description";
            rilue_roomTypeLookup.ValueMember = "Description";


            List<LookupDTO> stationList = LocalBuffer.LocalBuffer.LookUpBufferList.Where(l => l.Type == "Station").ToList();
            rilue_station.DataSource = stationList;

            List<LookupDTO> carrierList = LocalBuffer.LocalBuffer.LookUpBufferList.Where(l => l.Type == "Carrier").ToList();
            rilue_carrier.DataSource = carrierList;

            List<LookupDTO> transTypeList = LocalBuffer.LocalBuffer.LookUpBufferList.Where(l => l.Type == "Transportation Type").ToList();
            rilue_transpType.DataSource = transTypeList;

            rislue_company.DataSource = LocalBuffer.LocalBuffer.AllCustomerConsigneeViewlist;




        }

        // Populate Data
        #region Populate Data


        private bool PopulatePosChargeTransferData(bool isRefresh)
        {
            try
            {
                if (isRefresh || _roomPosCharges == null)
                {
                    gcPosCharges.DataSource = null;
                    _roomPosCharges = UIProcessManager.GetAllRoomPosCharges(Convert.ToDateTime(beiRoomPOSDate.EditValue), Convert.ToDateTime(beiRoomPOSDate.EditValue), selectedHotelPosCharge);

                    gcPosCharges.DataSource = _roomPosCharges;
                    gvPosCharges.RefreshData();
                }

                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private bool PopulateTravelDetails(bool isRefresh)
        {
            try
            {

                Progress_Reporter.Show_Progress("Loading Data...", "Please Wait.......");
                if (tcTravelDetail.SelectedTabPage == tpPickups)
                {
                    if (_pickupLists == null || isRefresh)
                    {
                        gcTravelDetail.DataSource = null;
                        DateTime CurrentDate = beiTravDetailDate.EditValue == null ? DateTime.Now : Convert.ToDateTime(beiTravDetailDate.EditValue.ToString());
                        _pickupLists = UIProcessManager.GetTravelDetailView(CNETConstantes.TD_PICK_UP, CurrentDate, selectedHotelTravel);
                        if (_pickupLists == null)
                        {
                            _pickupLists = new List<VwTravelDetailDTO>();
                            gcTravelDetail.DataSource = _pickupLists;
                            gvTravelDetail.RefreshData();
                            return true;
                        }

                        List<VwTravelDetailDTO> filtered = _pickupLists.GroupBy(r => r.Code).Select(r => r.First()).ToList();
                        if (beiRoomTypeFilter.EditValue != null)
                        {
                            filtered = filtered.Where(td => td.RoomTypeDescription == beiRoomTypeFilter.EditValue.ToString()).ToList();
                        }

                        if (beiCompanyFilter.EditValue != null && filtered != null)
                        {
                            filtered = filtered.Where(td => td.TradeName == beiCompanyFilter.EditValue.ToString()).ToList();

                        }

                        if (beiStationFilter.EditValue != null && filtered != null)
                        {
                            filtered = filtered.Where(td => td.FromStationCode == Convert.ToInt32(beiStationFilter.EditValue)).ToList();

                        }

                        if (beiTransportationType.EditValue != null && filtered != null)
                        {
                            filtered = filtered.Where(td => td.TransactionType == beiTransportationType.EditValue.ToString()).ToList();

                        }

                        if (beiCarrierFilter.EditValue != null && filtered != null)
                        {
                            filtered = filtered.Where(td => td.Carrier == beiCarrierFilter.EditValue.ToString()).ToList();

                        }


                        gcTravelDetail.DataSource = filtered;
                        gvTravelDetail.RefreshData();

                    }
                    else
                    {
                        gcTravelDetail.DataSource = _pickupLists;
                        gvTravelDetail.RefreshData();
                    }
                }
                else if (tcTravelDetail.SelectedTabPage == tpDropOffs)
                {
                    if (_dropoffList == null || isRefresh)
                    {
                        gcTravelDetail.DataSource = null;
                        DateTime CurrentDate = beiTravDetailDate.EditValue == null ? DateTime.Now : Convert.ToDateTime(beiTravDetailDate.EditValue.ToString());
                        _dropoffList = UIProcessManager.GetTravelDetailView(CNETConstantes.TD_DROP_OFF, CurrentDate, selectedHotelTravel);

                        if (_dropoffList == null)
                        {
                            _dropoffList = new List<VwTravelDetailDTO>();
                            gcTravelDetail.DataSource = _dropoffList;
                            gvTravelDetail.RefreshData();
                            return true;

                        }

                        List<VwTravelDetailDTO> filtered = _dropoffList.GroupBy(r => r.Code).Select(r => r.First()).ToList();
                        if (beiRoomTypeFilter.EditValue != null)
                        {
                            filtered = filtered.Where(td => td.RoomTypeDescription == beiRoomTypeFilter.EditValue.ToString()).ToList();
                        }

                        if (beiCompanyFilter.EditValue != null && filtered != null)
                        {
                            filtered = filtered.Where(td => td.TradeName == beiCompanyFilter.EditValue.ToString()).ToList();

                        }

                        if (beiStationFilter.EditValue != null && filtered != null)
                        {
                            filtered = filtered.Where(td => td.FromStationCode == Convert.ToInt32(beiStationFilter.EditValue.ToString())).ToList();

                        }

                        if (beiCarrierFilter.EditValue != null && filtered != null)
                        {
                            filtered = filtered.Where(td => td.Carrier == beiCarrierFilter.EditValue.ToString()).ToList();

                        }


                        gcTravelDetail.DataSource = filtered;
                        gvTravelDetail.RefreshData();
                    }
                    else
                    {
                        gcTravelDetail.DataSource = _dropoffList;
                        gvTravelDetail.RefreshData();
                    }
                }

                Progress_Reporter.Close_Progress();
                return true;
            }
            catch (Exception ex)
            {
                Progress_Reporter.Close_Progress();
                SystemMessage.ShowModalInfoMessage("Error in loading travel detail data. DETAIL: " + ex.Message, "ERROR");
                return false;
            }
        }

        private void PopulatePackageAuditList(bool isRefresh)
        {
            try
            {
                Progress_Reporter.Show_Progress("Loading Package Audit", "Please Wait...");

                if (isRefresh || _pkgAuditViewList == null)
                {
                    DateTime date = Convert.ToDateTime(beiDate.EditValue.ToString());
                    int? pkgHeader = beiPackage.EditValue == null ? null : Convert.ToInt32(beiPackage.EditValue);
                    _pkgAuditViewList = UIProcessManager.GetPackageView(date, selectedHotelPackage);
                    if (pkgHeader != null && _pkgAuditViewList != null)
                        _pkgAuditViewList = _pkgAuditViewList.Where(p => p.pkgHeaderId == pkgHeader).ToList();
                }
                List<PackageAuditVM> _dtoList = new List<PackageAuditVM>();
                gcPackageAudit.DataSource = null;


                if (_pkgAuditViewList != null)
                {
                    foreach (var pkgAudit in _pkgAuditViewList)
                    {
                        PackageAuditVM dto = new PackageAuditVM();
                        dto.Guest = pkgAudit.Guest;
                        dto.RegNum = pkgAudit.RegNum;
                        dto.Room = pkgAudit.Room;
                        dto.RoomType = pkgAudit.RoomType;
                        dto.VoucherCode = pkgAudit.VoucherCode;
                        dto.Amount = pkgAudit.Amount;
                        dto.Package = pkgAudit.Package;
                        dto.AdultCount = pkgAudit.AdultCount.Value;
                        dto.ChildCount = pkgAudit.ChildCount.Value;
                        _dtoList.Add(dto);
                    }

                    _dtoList.ForEach(x => x.SN = (_dtoList.IndexOf(x) + 1));
                }
                gcPackageAudit.DataSource = _dtoList;
                gvPackageAudit.RefreshData();

                Progress_Reporter.Close_Progress();
            }
            catch (Exception ex)
            {
                Progress_Reporter.Close_Progress();
                SystemMessage.ShowModalInfoMessage("Error in loading package audits. Detail: " + ex.Message, "ERROR");
            }
        }

        //Profiles

        private void PopulateSource()
        {
            consigneeDocument = new UCConsigneeDocument(CNETConstantes.BUSINESSsOURCE);
            consigneeDocument.Dock = DockStyle.Fill;
            consigneeDocument.gvConsignee.DoubleClick += gvDocBrowser_DoubleClick;
            this.pnlControl_Source.Controls.Clear();
            this.pnlControl_Source.Controls.Add(consigneeDocument);

            flagSource = true;
        }

        private void PopulateGroup()
        {
            consigneeDocument = new UCConsigneeDocument(CNETConstantes.GROUP);
            consigneeDocument.Dock = DockStyle.Fill;
            consigneeDocument.gvConsignee.DoubleClick += gvDocBrowser_DoubleClick;
            this.pnlControl_Group.Controls.Clear();
            this.pnlControl_Group.Controls.Add(consigneeDocument);

            flagGroup = true;
        }

        private void PopulateAgent()
        {
            consigneeDocument = new UCConsigneeDocument(CNETConstantes.AGENT);
            consigneeDocument.Dock = DockStyle.Fill;
            consigneeDocument.gvConsignee.DoubleClick += gvDocBrowser_DoubleClick;
            this.pnlControl_Agent.Controls.Clear();
            this.pnlControl_Agent.Controls.Add(consigneeDocument);

            flagAgent = true;
        }

        private void PopulateCompany()
        {

            consigneeDocument = new UCConsigneeDocument(CNETConstantes.CUSTOMER);
            consigneeDocument.Dock = DockStyle.Fill;
            consigneeDocument.gvConsignee.DoubleClick += gvDocBrowser_DoubleClick;
            this.pnlCtrlCompany.Controls.Clear();
            this.pnlCtrlCompany.Controls.Add(consigneeDocument);

            flagCompany = true;

        }

        private void PopulateContact()
        {
            consigneeDocument = new UCConsigneeDocument(CNETConstantes.CONTACT);
            consigneeDocument.Dock = DockStyle.Fill;
            consigneeDocument.gvConsignee.DoubleClick += gvDocBrowser_DoubleClick;
            this.pnlControl_Contact.Controls.Clear();
            this.pnlControl_Contact.Controls.Add(consigneeDocument);
            flagContact = true;
        }

        private void PopulateGuest()
        {
            consigneeDocument = new UCConsigneeDocument(CNETConstantes.GUEST);
            consigneeDocument.Dock = DockStyle.Fill;
            consigneeDocument.gvConsignee.DoubleClick += gvDocBrowser_DoubleClick;
            this.pnlControl_Guest.Controls.Clear();
            this.pnlControl_Guest.Controls.Add(consigneeDocument);
            flagGuest = true;
        }

        // Vouchers

        private void PopulateCashReceipt()
        {
            voucherDocument = new UCVoucherDocument(CNETConstantes.CASHRECIPT);
            voucherDocument.Dock = DockStyle.Fill;
            this.pnl_CashReceipt.Controls.Clear();
            this.pnl_CashReceipt.Controls.Add(voucherDocument);
            flagCachReceipt = true;
        }

        private void PopulateCreditSales()
        {
            voucherDocument = new UCVoucherDocument(CNETConstantes.CREDITSALES);
            //voucherDocument.RoomPOSChargeButtonClicked += voucherDocument_RoomPOSChargeButtonClicked;
            voucherDocument.Dock = DockStyle.Fill;
            this.pnl_creditSales.Controls.Clear();
            this.pnl_creditSales.Controls.Add(voucherDocument);
            flagCreditSales = true;
        }

        private void PopulateCashSales()
        {
            voucherDocument = new UCVoucherDocument(CNETConstantes.CASH_SALES);
            //voucherDocument.RoomPOSChargeButtonClicked += voucherDocument_RoomPOSChargeButtonClicked;
            voucherDocument.Dock = DockStyle.Fill;
            this.pnl_cashSales.Controls.Clear();
            this.pnl_cashSales.Controls.Add(voucherDocument);
            flagCashSales = true;
        }

        private void PopulateCashSalesSummary()
        {
            voucherDocument = new UCVoucherDocument(CNETConstantes.CASHSALESSUMMRY);
            voucherDocument.Dock = DockStyle.Fill;
            this.pnl_cashSalesSummary.Controls.Clear();
            this.pnl_cashSalesSummary.Controls.Add(voucherDocument);
            flagCashSalesSummary = true;
        }

        private void PopulateCreditNotes()
        {
            voucherDocument = new UCVoucherDocument(CNETConstantes.CREDIT_NOTE_VOUCHER);
            voucherDocument.Dock = DockStyle.Fill;
            this.pnl_creditNote.Controls.Clear();
            this.pnl_creditNote.Controls.Add(voucherDocument);

            flagCreditNote = true;
        }

        private void PopulatePaidOuts()
        {
            voucherDocument = new UCVoucherDocument(CNETConstantes.PAID_OUT_VOUCHER);
            voucherDocument.Dock = DockStyle.Fill;
            this.pnl_paidOut.Controls.Clear();
            this.pnl_paidOut.Controls.Add(voucherDocument);
            flagPaidout = true;
        }

        private void PopulateRefunds()
        {
            voucherDocument = new UCVoucherDocument(CNETConstantes.REFUND);
            voucherDocument.Dock = DockStyle.Fill;
            this.pnl_refund.Controls.Clear();
            this.pnl_refund.Controls.Add(voucherDocument);
            flagRefund = true;
        }

        private void PopulateDebits()
        {
            voucherDocument = new UCVoucherDocument(CNETConstantes.BANKDEBITNOTE);
            voucherDocument.Dock = DockStyle.Fill;
            this.pnl_debit.Controls.Clear();
            this.pnl_debit.Controls.Add(voucherDocument);

            flagDebit = true;
        }

        private void PopulateDRC()
        {
            voucherDocument = new UCVoucherDocument(CNETConstantes.DAILY_ROOM_CHARGE_VOUCHER);
            voucherDocument.Dock = DockStyle.Fill;
            this.pnl_drc.Controls.Clear();
            this.pnl_drc.Controls.Add(voucherDocument);

            flagDrc = true;
        }

        private void PopulateEventReq()
        {
            voucherDocument = new UCVoucherDocument(CNETConstantes.EVENT_REQUIREMENT_VOUCHER);
            voucherDocument.Dock = DockStyle.Fill;
            this.pnlEventReq.Controls.Clear();
            this.pnlEventReq.Controls.Add(voucherDocument);

            flagEventReq = true;

        }
        private void PopulateDailyResi()
        {
            voucherDocument = new UCVoucherDocument(CNETConstantes.Daily_Resident_Summary_Voucher);
            voucherDocument.Dock = DockStyle.Fill;
            this.pnlDailResidentSummary.Controls.Clear();
            this.pnlDailResidentSummary.Controls.Add(voucherDocument);

            flagDailyRes = true;

        }
        private void PopulateCheckOut()
        {
            voucherDocument = new UCVoucherDocument(CNETConstantes.CHECK_OUT_BILL_VOUCHER);
            voucherDocument.Dock = DockStyle.Fill;
            this.pnlCheckOut.Controls.Clear();
            this.pnlCheckOut.Controls.Add(voucherDocument);

            flagCheckOut = true;

        }
        private void PopulatePackagecons()
        {
            voucherDocument = new UCVoucherDocument(CNETConstantes.PACKAGE_CONSUMPTION_VOUCHER);
            voucherDocument.Dock = DockStyle.Fill;
            this.pnlPackageCons.Controls.Clear();
            this.pnlPackageCons.Controls.Add(voucherDocument);

            flagPackagecons = true;

        }
        private void PopulateLogBook()
        {
            voucherDocument = new UCVoucherDocument(CNETConstantes.MESSAGE);
            voucherDocument.Dock = DockStyle.Fill;
            this.pnlLogBook.Controls.Clear();
            this.pnlLogBook.Controls.Add(voucherDocument);

            flagLogbook = true;

        }
        private void PopulateServiceReq()
        {
            voucherDocument = new UCVoucherDocument(CNETConstantes.SERVICE_REQUEST_VOUCHER);
            voucherDocument.Dock = DockStyle.Fill;
            this.pnlServiceRequest.Controls.Clear();
            this.pnlServiceRequest.Controls.Add(voucherDocument);

            flagServiceRequest = true;

        }

        private void PopulateLostAndFound()
        {
            voucherDocument = new UCVoucherDocument(CNETConstantes.LOST_AND_FOUND_VOUCHER);
            voucherDocument.Dock = DockStyle.Fill;
            this.pnlLostAndFound.Controls.Clear();
            this.pnlLostAndFound.Controls.Add(voucherDocument);

            flagLostAndFound = true;

        }

        #endregion


        // Event Handlers
        #region Event Handlers

        private void voucherDocument_RoomPOSChargeButtonClicked(object sender, VoucherRoomPOSChargeClicked e)
        {
            var row = voucherDocument.gvVoucher.GetFocusedRow();
            Type type = row.GetType();

            VoucherValuesData voucher = null;
            //if (type.Name == CNETConstantes.VoucherHeaderView)
            //{
            VwVoucherHeaderDTO voucherheavy = row as VwVoucherHeaderDTO;
            voucher = new VoucherValuesData
            {
                IssuedDate = voucherheavy.IssuedDate,
                voucherCode = voucherheavy.Id,
                voucherDefinition = voucherheavy.DefinitionId,
                subTotal = voucherheavy.SubTotal,
                VATtaxAmount = voucherheavy.VattaxAmount,
                additionalCharge = voucherheavy.AddCharge,
                discount = voucherheavy.Discount,
                grandTotal = voucherheavy.GrandTotal,
                voucherNote = voucherheavy.Note
            };
            //}
            //else if (type.Name == CNETConstantes.VoucherHeaderViewLight)
            //{
            //    vw_VoucherHeaderLight voucherLight = row as vw_VoucherHeaderLight;
            //    voucher = new VoucherValuesData
            //    {
            //        IssuedDate = voucherLight.IssuedDate,
            //        voucherCode = voucherLight.voucherCode,
            //        voucherDefinition = voucherLight.voucherDefinition,
            //        subTotal = voucherLight.subTotal,
            //        VATtaxAmount = voucherLight.VATtaxAmount,
            //        additionalCharge = voucherLight.additionalCharge,
            //        discount = voucherLight.discount,
            //        grandTotal = voucherLight.grandTotal,
            //        voucherNote = voucherLight.voucherNote
            //    };
            //}


            if (voucher == null) return;

            DateTime? CurrentTime = UIProcessManager.GetServiceTime();
            if (CurrentTime == null) return;
            if (voucher.IssuedDate.Date != CurrentTime.Value.Date)
            {
                SystemMessage.ShowModalInfoMessage("Voucher's Issued Date is not equal to today!", "ERROR");
                return;
            }

            //if this voucher is made during check out, abort operation
            if (voucher.voucherNote != null && voucher.voucherNote == "check_out")
            {
                SystemMessage.ShowModalInfoMessage("This voucher is made by checkout!", "ERROR");
                return;
            }

            //to check already it is charged
            List<VwRoomPoschargeViewDTO> allRoomPosCharges = UIProcessManager.GetAllRoomPosCharges(CurrentTime.Value, CurrentTime.Value, selectedHotelPosCharge);
            if (allRoomPosCharges != null)
            {
                allRoomPosCharges = allRoomPosCharges.Where(p => p.Referringid == voucher.voucherCode).ToList();
            }

            if (allRoomPosCharges != null && allRoomPosCharges.Count > 0)
            {
                SystemMessage.ShowModalInfoMessage("There is room pos charge with this voucher", "ERROR");
                return;
            }

            try
            {

                frmRoomPosCharge frmRoomPosCharge = new frmRoomPosCharge();
                frmRoomPosCharge.SelectedHotelcode = selectedHotelPosCharge.Value;
                frmRoomPosCharge.Voucher = voucher;
                frmRoomPosCharge.Show();
            }
            catch (Exception ex)
            {

            }
        }


        /// <summary>
        /// this method handles edit operation for the selected tab page object. 
        /// </summary>
        private void HandleDocumentEdit()
        {
            if (tcDocumentControl.SelectedTabPage == tpProfiles)
            {
                String text = tcProfiles.SelectedTabPage.Text;
                //List<String> allowedTabs = MasterPageForm.AllowedFunctionalities("Profile");
                //if (!MasterPageForm.checkExistance(allowedTabs, text))
                //{
                //    XtraMessageBox.Show("You Are Not Allowed For This Operation", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //    return;
                //}

                #region Guest
                if (tcProfiles.SelectedTabPage == tpGuest)
                {
                    try
                    {

                        var row = (VwConsigneeViewDTO)consigneeDocument.gvConsignee.GetFocusedRow();
                        if (row != null)
                        {
                            ConsigneeDTO data = LocalBuffer.LocalBuffer.ConsigneeViewBufferList.FirstOrDefault(x => x.Id == row.Id);
                            if (data != null)
                            {
                                frmPerson personForm = new frmPerson(@"Guest");
                                personForm.Text = "Guest";
                                personForm.GSLType = CNETConstantes.GUEST;
                                personForm.rpgScanFingerPrint.Visible = true;
                                personForm.LoadEventArg.Args = "Guest";
                                personForm.LoadEventArg.Sender = null;
                                personForm.PersonToEdit = data;
                                personForm.ShowDialog();
                            }
                        }


                    }
                    catch (Exception ex)
                    {
                        XtraMessageBox.Show("Internal Application Error. DETAIL:: " + ex.Message, "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                }

                #endregion

                #region Contact
                else if (tcProfiles.SelectedTabPage == tpContact)
                {
                    try
                    {
                        var row = (VwConsigneeViewDTO)consigneeDocument.gvConsignee.GetFocusedRow();

                        if (row != null)
                        {
                            ConsigneeDTO data = LocalBuffer.LocalBuffer.ConsigneeViewBufferList.FirstOrDefault(x => x.Id == row.Id);
                            if (data != null)
                            {
                                frmPerson personForm = new frmPerson(@"Contact");
                                personForm.GSLType = CNETConstantes.CONTACT;
                                personForm.Text = "Contact";
                                personForm.rpgScanFingerPrint.Visible = true;
                                personForm.LoadEventArg.Args = "Contact";
                                personForm.LoadEventArg.Sender = null;
                                personForm.PersonToEdit = data;

                                personForm.ShowDialog();
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        XtraMessageBox.Show("Internal Application Error. DETAIL:: " + ex.Message, "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                #endregion

                #region Company
                else if (tcProfiles.SelectedTabPage == tpCompany)
                {
                    try
                    {
                        var row = (VwConsigneeViewDTO)consigneeDocument.gvConsignee.GetFocusedRow();
                        if (row != null)
                        {
                            frmOrganization orgForm = new frmOrganization();
                            orgForm.GslType = CNETConstantes.CUSTOMER;
                            orgForm.Text = "Company";
                            orgForm.rcOrganization.Visible = true;
                            orgForm.LoadEventArg.Args = "Company";
                            orgForm.LoadEventArg.Sender = null;
                            orgForm.OrgToEdit = row;
                            orgForm.ShowDialog();
                        }
                    }
                    catch (Exception ex)
                    {
                        XtraMessageBox.Show("Internal Application Error. DETAIL:: " + ex.Message, "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                }

                #endregion

                #region Travel Agent

                else if (tcProfiles.SelectedTabPage == tpTravelAgent)
                {
                    try
                    {
                        var row = (VwConsigneeViewDTO)consigneeDocument.gvConsignee.GetFocusedRow();
                        if (row != null)
                        {
                            frmOrganization orgForm = new frmOrganization();
                            orgForm.GslType = CNETConstantes.AGENT;
                            orgForm.Text = "Travel Agent";
                            orgForm.rcOrganization.Visible = true;
                            orgForm.LoadEventArg.Args = "Travel Agent";
                            orgForm.LoadEventArg.Sender = null;
                            orgForm.OrgToEdit = row;
                            orgForm.ShowDialog();
                        }
                    }
                    catch (Exception ex)
                    {
                        XtraMessageBox.Show("Internal Application Error. DETAIL:: " + ex.Message, "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                #endregion

                #region Group

                else if (tcProfiles.SelectedTabPage == tpGroup)
                {
                    try
                    {
                        var row = (VwConsigneeViewDTO)consigneeDocument.gvConsignee.GetFocusedRow();
                        if (row != null)
                        {
                            frmOrganization orgForm = new frmOrganization();
                            orgForm.GslType = CNETConstantes.GROUP;
                            orgForm.Text = "Group";
                            orgForm.rcOrganization.Visible = true;
                            orgForm.LoadEventArg.Args = "Group";
                            orgForm.LoadEventArg.Sender = null;
                            orgForm.OrgToEdit = row;
                            orgForm.ShowDialog();

                        }

                    }
                    catch (Exception ex)
                    {
                        XtraMessageBox.Show("Internal Application Error. DETAIL:: " + ex.Message, "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                #endregion

                #region Business Source

                else if (tcProfiles.SelectedTabPage == tpSource)
                {
                    try
                    {
                        var row = (VwConsigneeViewDTO)consigneeDocument.gvConsignee.GetFocusedRow();
                        if (row != null)
                        {
                            frmOrganization orgForm = new frmOrganization();
                            orgForm.GslType = CNETConstantes.BUSINESSsOURCE;
                            orgForm.Text = @"Source";
                            orgForm.rcOrganization.Visible = true;
                            orgForm.LoadEventArg.Args = "Source";
                            orgForm.LoadEventArg.Sender = null;
                            orgForm.OrgToEdit = row;
                            orgForm.ShowDialog();
                        }

                    }
                    catch (Exception ex)
                    {
                        XtraMessageBox.Show("Internal Application Error. DETAIL:: " + ex.Message, "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                #endregion
            }
        }

        //Edit Button Clicked
        private void bbiEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            HandleDocumentEdit();
        }

        //// Context Menu of organization's Edit button clicked
        //void OrganizationDocument_EditButtonClicked(object sender, OrganizationEditItemClicked e)
        //{
        //    HandleDocumentEdit();
        //}

        //// Context menu of person's Edit button clicked
        //private void PersonDocument_EditButtonClicked(object sender, PersonEditItemClicked e)
        //{
        //    HandleDocumentEdit();
        //}

        //Doblie click of the document browsers' grid view
        private void gvDocBrowser_DoubleClick(object sender, EventArgs e)
        {
            HandleDocumentEdit();
        }

        // PROFILE: Selected Tab Page Changed
        private void tcProfiles_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            Progress_Reporter.Show_Progress("Loading " + tcProfiles.SelectedTabPage.Text + "...", "Please Wait.......");

            try
            {

                if (tcProfiles.SelectedTabPage == tpGuest)
                {
                    //if (personDocument == null)
                    if (!flagGuest)
                    {
                        PopulateGuest();
                    }

                }
                else if (tcProfiles.SelectedTabPage == tpContact)
                {
                    //if (personDocument == null)
                    if (!flagContact)
                    {
                        PopulateContact();
                    }

                }
                else if (tcProfiles.SelectedTabPage == tpCompany)
                {
                    if (!flagCompany)
                        PopulateCompany();

                }
                else if (tcProfiles.SelectedTabPage == tpTravelAgent)
                {
                    if (!flagAgent)
                        PopulateAgent();

                }
                else if (tcProfiles.SelectedTabPage == tpGroup)
                {
                    if (!flagGroup)
                        PopulateGroup();

                }
                else if (tcProfiles.SelectedTabPage == tpSource)
                {
                    if (!flagSource)
                        PopulateSource();

                }

                Progress_Reporter.Close_Progress();
            }
            catch (Exception ex)
            {
                Progress_Reporter.Close_Progress();
                SystemMessage.ShowModalInfoMessage("Error: " + ex.Message, "ERROR");
            }


        }

        // VOUCHER: Selected Tab Page Changed
        private void tcVouchers_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            try
            {
                Progress_Reporter.Show_Progress("Loading " + tcVouchers.SelectedTabPage.Text + "...", "Please Wait.......");
                if (tcVouchers.SelectedTabPage == tp_CashReceipt)
                {
                    if (!flagCachReceipt)
                        PopulateCashReceipt();

                }
                else if (tcVouchers.SelectedTabPage == tp_creditSales)
                {
                    if (!flagCreditSales)
                        PopulateCreditSales();

                }
                else if (tcVouchers.SelectedTabPage == tp_creditNote)
                {
                    if (!flagCreditNote)
                        PopulateCreditNotes();
                }
                else if (tcVouchers.SelectedTabPage == tp_cashSales)
                {
                    if (!flagCashSales)
                        PopulateCashSales();
                }
                else if (tcVouchers.SelectedTabPage == tp_cashSalesSummary)
                {
                    if (!flagCashSalesSummary)
                        PopulateCashSalesSummary();
                }
                else if (tcVouchers.SelectedTabPage == tp_paidOut)
                {
                    if (!flagPaidout)
                        PopulatePaidOuts();
                }
                else if (tcVouchers.SelectedTabPage == tp_refund)
                {
                    if (!flagRefund)
                        PopulateRefunds();
                }
                else if (tcVouchers.SelectedTabPage == tp_debit)
                {
                    if (!flagDebit)
                        PopulateDebits();
                }
                else if (tcVouchers.SelectedTabPage == tp_dailyRC)
                {
                    if (!flagDrc)
                        PopulateDRC();
                }
                else if (tcVouchers.SelectedTabPage == tp_EventRequirement)
                {
                    if (!flagEventReq)
                        PopulateEventReq();
                }
                else if (tcVouchers.SelectedTabPage == tp_ServiceRequest)
                {
                    if (!flagEventReq)
                        PopulateServiceReq();
                }
                else if (tcVouchers.SelectedTabPage == tp_LostAndFound)
                {
                    if (!flagLostAndFound)
                        PopulateLostAndFound();
                }
                else if (tcVouchers.SelectedTabPage == tp_DailyResident)
                {
                    if (!flagDailyRes)
                        PopulateDailyResi();
                }
                else if (tcVouchers.SelectedTabPage == tp_CheckOut)
                {
                    if (!flagCheckOut)
                        PopulateCheckOut();
                }
                else if (tcVouchers.SelectedTabPage == tp_PackageCons)
                {
                    if (!flagPackagecons)
                        PopulatePackagecons();
                }
                else if (tcVouchers.SelectedTabPage == tp_LogBook)
                {
                    if (!flagLogbook)
                        PopulateLogBook();
                }
                Progress_Reporter.Close_Progress();
            }
            catch (Exception ex)
            {
                Progress_Reporter.Close_Progress();
                SystemMessage.ShowModalInfoMessage("Error: " + ex.Message, "ERROR");
            }
        }

        // TRAVEL DETAIL : Selected Tab page changed
        private void tcTravelDetail_SelectedPageChanged(object sender, TabPageChangedEventArgs e)
        {
            PopulateTravelDetails(false);
        }

        // Selected Tab page changes for document control
        // when transactions(vouchers) tab is clicked, load cash receipt documents by default
        private void tcDocumentControl_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            Progress_Reporter.Show_Progress("Loading " + tcVouchers.SelectedTabPage.Text + "...", "Please Wait.......");
            if (tcDocumentControl.SelectedTabPage == tpTransactions)
            {
                //disalbe delete and edit buttons
                // bbi_deleteDocument.Enabled = false;
                // bbi_editDocument.Enabled = false;

                if (tcVouchers.SelectedTabPage == tp_CashReceipt)
                {
                    if (!flagCachReceipt)
                        PopulateCashReceipt();

                }
            }
            else
            {
                //enable delete and edit buttons
                // bbi_deleteDocument.Enabled = true;
                // bbi_editDocument.Enabled = true;
            }

            Progress_Reporter.Close_Progress();
        }

        //delete button clicked
        private void bbiRemoveRoomType_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }



        private void bbiPrint_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ReportGenerator reportGen = new ReportGenerator();
            DateTime? CurrentTime = UIProcessManager.GetServiceTime();
            if (CurrentTime == null) return;
            reportGen.GenerateGridReport(gcPosCharges, "Room POS Charges", CurrentTime.Value.ToShortDateString());
        }

        private void bbiRefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (!PopulatePosChargeTransferData(true))
            {
                XtraMessageBox.Show("Unable to populate room pos charges", "CNET_v2016", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        private void gvPosCharges_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            if (e.Column.Caption == "SN")
            {
                e.DisplayText = (e.RowHandle + 1).ToString();
            }
        }


        private void bbiRemove_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                DialogResult dRes = XtraMessageBox.Show("Do you really want to remove this room POS Charge?", "Remove Room POS Charge", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (dRes == System.Windows.Forms.DialogResult.No)
                {
                    return;
                }

                VwRoomPoschargeViewDTO model = gvPosCharges.GetFocusedRow() as VwRoomPoschargeViewDTO;
                if (model == null) return;

                //check workflow
                int? activityDefCode = null;
                ActivityDefinitionDTO workFlow = UIProcessManager.GetActivityDefinitionBydescriptionandreference(CNETConstantes.LU_ACTIVITY_DEFINATION_ROOM_POS_CHARGE_REMOVED, model.Definition.Value).FirstOrDefault();

                if (workFlow != null)
                {

                    activityDefCode = workFlow.Id;
                }
                else
                {
                    SystemMessage.ShowModalInfoMessage("Please define workflow of ROOM POS CHARGE REMOVED for Credit Sales and Cash Sales Voucher ", "ERROR");
                    return;
                }

                Progress_Reporter.Show_Progress("Removing room pos charge..", "Please Wait.......");
                List<TransactionReferenceDTO> traRefList = UIProcessManager.GetTransactionReferenceByreferenced(model.Referenced.Value);
                if (traRefList != null)
                {
                    TransactionReferenceDTO tranRef = traRefList.FirstOrDefault(m => m.Referring == model.Referring);
                    if (tranRef != null)
                    {
                        if (UIProcessManager.DeleteTransactionReferenceById(tranRef.Id))
                        {
                            //saving activity;
                            DateTime? CurrentTime = UIProcessManager.GetServiceTime();
                            if (CurrentTime != null)
                            {
                                var response = UIProcessManager.GetVoucherBufferById(tranRef.Referring.Value);

                                VoucherBuffer vocuherBuffer = response.Data;
                                vocuherBuffer.Activity = ActivityLogManager.SetupActivity(CurrentTime.Value, activityDefCode.Value, CNETConstantes.PMS_Pointer, "Registration Code = " + model.Referenced);
                                vocuherBuffer.Voucher.LastState = workFlow.State.Value;
                                if (vocuherBuffer.TransactionReferencesBuffer != null && vocuherBuffer.TransactionReferencesBuffer.Count > 0)
                                    vocuherBuffer.TransactionReferencesBuffer.ToList().ForEach(x => x.ReferencedActivity = null);

                                vocuherBuffer.TransactionCurrencyBuffer = null;

                                UIProcessManager.UpdateVoucherBuffer(vocuherBuffer);

                            }

                            Progress_Reporter.Close_Progress();
                            XtraMessageBox.Show("Room POS Charge is removed!", "CNET_v2016", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            PopulatePosChargeTransferData(true);
                        }
                        else
                        {
                            Progress_Reporter.Close_Progress();
                            XtraMessageBox.Show("Room POS Charge is not removed!", "CNET_v2016", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        Progress_Reporter.Close_Progress();
                        XtraMessageBox.Show("Unable to get transaction reference record!", "CNET_v2016", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    Progress_Reporter.Close_Progress();
                    XtraMessageBox.Show("Unable to get transaction reference record!", "CNET_v2016", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch (Exception ex)
            {
                Progress_Reporter.Close_Progress();
                XtraMessageBox.Show("Error in removing room pos charge. DETAIL::" + ex.Message, "CNET_v2016", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        private void bbiTransfer_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            VwRoomPoschargeViewDTO model = gvPosCharges.GetFocusedRow() as VwRoomPoschargeViewDTO;
            if (model == null) return;

            frmTransferPosCharge frmTransferPosCharge = new frmTransferPosCharge();
            frmTransferPosCharge.SelectedHotelcode = selectedHotelPosCharge.Value;
            frmTransferPosCharge.RoomPosCharge = model;
            if (frmTransferPosCharge.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                PopulatePosChargeTransferData(true);
            }


        }

        private void gvPosCharges_DoubleClick(object sender, EventArgs e)
        {
            VwRoomPoschargeViewDTO model = gvPosCharges.GetFocusedRow() as VwRoomPoschargeViewDTO;
            if (model == null) return;


            DocumentPrint.ReportGenerator reportGenerator = new DocumentPrint.ReportGenerator();
            reportGenerator.GetAttachementReport(model.Referring.Value);
        }

        private void gvPosCharges_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            if (e.HitInfo.InRow)
            {
                GridView view = sender as GridView;
                view.FocusedRowHandle = e.HitInfo.RowHandle;
                foreach (DXMenuItem item in menuItems)
                {
                    e.Menu.Items.Add(item);
                }
            }
        }

        private void bbiShowTravelDetail_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            PopulateTravelDetails(true);
        }

        private void bbiPrintTravelDetail_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ReportGenerator rg = new ReportGenerator();
            if (tcTravelDetail.SelectedTabPage == tpPickups)
            {
                rg.GenerateGridReport(gcTravelDetail, "Pick Up Travel Details", beiTravDetailDate.EditValue.ToString());
            }
            else if (tcTravelDetail.SelectedTabPage == tpDropOffs)
            {
                rg.GenerateGridReport(gcTravelDetail, "Drop Off Travel Details", beiTravDetailDate.EditValue.ToString());
            }
        }

        private void bbiClearTdCriteria_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            beiRoomTypeFilter.EditValue = null;
            beiCompanyFilter.EditValue = null;
            beiStationFilter.EditValue = null;
            beiCarrierFilter.EditValue = null;
            beiTransportationType.EditValue = null;
        }

        private void beiDate_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }


        #region Package Audit 
        private void bbiShow_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            PopulatePackageAuditList(true);
        }

        private void bbiPrintPackAudit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                ReportGenerator rg = new ReportGenerator();
                DateTime date = Convert.ToDateTime(beiDate.EditValue.ToString());
                rg.GenerateGridReport(gcPackageAudit, "Package Audit", date.ToShortDateString());
            }
            catch (Exception ex)
            {
                SystemMessage.ShowModalInfoMessage("Error in printing package audit. Detail: " + ex.Message, "ERROR");
            }
        }

        private void gvPackageAudit_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            GridView view = sender as GridView;
            if (view == null) return;
            if (e.Column.Caption == "SN")
            {
                PackageAuditVM dto = view.GetRow(e.RowHandle) as PackageAuditVM;
                if (dto != null)
                    dto.SN = e.RowHandle + 1;
                e.DisplayText = (e.RowHandle + 1).ToString();
            }
        }

        private void bbiClearFilterPackageAudit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            beiPackage.EditValue = null;

            DateTime? CurrentDate = UIProcessManager.GetServiceTime();
            if (CurrentDate == null)
            {
                tcVouchers.Enabled = false;
            }
            beiDate.EditValue = CurrentDate.Value;
        }

        #endregion

        private void bbiPrintCard_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                var selected = gvTravelDetail.GetFocusedRow() as CNET_V7_Domain.Domain.ViewSchema.VwTravelDetailViewDTO;
                if (selected != null)
                {
                    //PickupObjectList pickup = new PickupObjectList()
                    //{
                    //    Carrer = selected.carrier,
                    //    Guest = selected.name,
                    //    Company = selected.tradeName,
                    //    TransportationNo = selected.transportationNo,

                    //};

                    //ReportGenerator rg = new ReportGenerator();

                    //rg.PrintPickUp(pickup);

                }
                else
                {
                    SystemMessage.ShowModalInfoMessage("Please select travel detail first.", "ERROR");
                }
            }
            catch (Exception ex)
            {
                SystemMessage.ShowModalInfoMessage("Error in printing pick-up card. DETAIL:: " + ex.Message, "ERROR");
            }
        }







        #endregion

        private void xtcCustomReport_SelectedPageChanged(object sender, TabPageChangedEventArgs e)
        {
            try
            {
                Progress_Reporter.Show_Progress("Loading " + tcCustomReport.SelectedTabPage.Text + "...", "Please Wait.......");
                if (tcCustomReport.SelectedTabPage == tp_POSCharges)
                {
                    if (!PopulatePosChargeTransferData(false))
                    {
                        XtraMessageBox.Show("Unable to populate room pos charges", "CNET_v2016", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    }
                }
                else if (tcCustomReport.SelectedTabPage == tp_TravelDetails)
                {
                    DateTime? CurrentDate = UIProcessManager.GetServiceTime();
                    if (CurrentDate == null)
                    {
                        tcVouchers.Enabled = false;
                    }
                    beiTravDetailDate.EditValue = CurrentDate.Value;

                    //To Do: load room types
                    //  InitializeTravelDetail();
                }
                else if (tcCustomReport.SelectedTabPage == tp_PackageAudit)
                {
                    DateTime? CurrentDate = UIProcessManager.GetServiceTime();
                    if (CurrentDate == null)
                    {
                        tcVouchers.Enabled = false;
                    }
                    beiDate.EditValue = CurrentDate.Value;

                    //populate packages

                    PopulatePackageAuditList(true);
                }
                Progress_Reporter.Close_Progress();
            }
            catch (Exception ex)
            {
                Progress_Reporter.Close_Progress();
                SystemMessage.ShowModalInfoMessage("Error: " + ex.Message, "ERROR");
            }
        }

        private void beiHotelPosCharge_EditValueChanged(object sender, EventArgs e)
        {
            selectedHotelPosCharge = beiHotelPosCharge.EditValue == null ? null : Convert.ToInt32(beiHotelPosCharge.EditValue);


        }

        private void beiHotelTravel_EditValueChanged(object sender, EventArgs e)
        {
            List<RoomTypeDTO> roomType = null;
            selectedHotelTravel = beiHotelTravel.EditValue == null ? null : Convert.ToInt32(beiHotelTravel.EditValue);

            if (selectedHotelPackage != null)
                roomType = UIProcessManager.GetRoomTypeByConsigneeUnit(selectedHotelTravel.Value);
            else
                roomType = UIProcessManager.SelectAllRoomType();
            rilue_roomTypeLookup.DataSource = (roomType);


        }

        private void beiHotelPackage_EditValueChanged(object sender, EventArgs e)
        {

            selectedHotelPackage = beiHotelPackage.EditValue == null ? null : Convert.ToInt32(beiHotelPackage.EditValue);

            List<PackageHeaderDTO> pkgHeaderList = null;

            if (selectedHotelPackage != null)
                pkgHeaderList = UIProcessManager.GetAllPackageHeaderByConsigneeUnit(selectedHotelPackage.Value);
            else
                pkgHeaderList = UIProcessManager.SelectAllPackageHeader();

            ripLukPackageAudit.DataSource = pkgHeaderList;
        }

        public int? selectedHotelPosCharge { get; set; }
        public int? selectedHotelPackage { get; set; }
        public int? selectedHotelTravel { get; set; }

        private void frmDocumentBrowser_Load(object sender, EventArgs e)
        {
            InitializeTravelDetail();
        }
    }
}
