using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using CNET.ERP.Client.Common.UI;
using CNET.FrontOffice_V._7.PMS.Contracts;
using CNET.ERP.Client.Common.UI.Library;

//using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraGrid.Views.Grid;
using CNET.FrontOffice_V._7.Forms.Non_Navigatable_Modals;
using CNET.ERP.ResourceProvider;

using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.BandedGrid.ViewInfo;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using CNET.FrontOffice_V._7.PMS.Contracts;
using CNET_V7_Domain.Domain.PmsSchema;
using ProcessManager;
using CNET_V7_Domain.Domain.CommonSchema;
using CNET.FrontOffice_V._7.Group_Registration;
using CNET.FrontOffice_V._7.PMS.Common_Classes;
using CNET_V7_Domain.Misc.PmsDTO;

//using FormValidation;using System.Windows.Forms;

namespace CNET.FrontOffice_V._7.Forms
{
    public partial class frmRateSearch : UILogicBase, ILogicHelper
    //public partial class frmRateSearch : DevExpress.XtraEditors.XtraForm
    {
        private DateTime _fromDate;
        private DateTime _toDate;
        private string _adultCount = string.Empty;
        private string _childCount = string.Empty;
        private string _roomCount = string.Empty;
        private int? _rateCode = null;
        private int? _roomType = null;

        private UILogicBase _requestingForm;
        public decimal TotalAmount { get; set; }
        public decimal FirstNightAmount { get; set; }
        public decimal AverageRateAmt { get; set; }
        private AvailableRateGeneratorDTO _availableRatesGenerated;
        private List<AvailableRateDTO> _availableRates;
        private List<RateCodeHeaderDTO> _reCodeHeaders;
        RateSearchCellClickedEventArgs _cellArgs;
        bool isCellSelected = false;

        public RateSearchCellClickedEventArgs cellArgs
        {
            get
            {

                EvaluateVisibilityforGroupSelector();
                return _cellArgs;
            }
            set
            {


                _cellArgs = value;

                EvaluateVisibilityforGroupSelector();


            }
        }

        private void EvaluateVisibilityforGroupSelector()
        {
            if (_cellArgs == null) rpgSelect.Enabled = false;
            else rpgSelect.Enabled = true;
        }



        //////////////////////// CONSTRUCTOR ////////////////////////
        public frmRateSearch()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            // Progress_Reporter.Show_Progress("Loading data.Please wait...");

            InitializeUI();
            PackageDetailList = UIProcessManager.SelectAllPackageDetail();
            AllExchangeRate = UIProcessManager.SelectAllExchangeRate();

            ////CNETInfoReporter.Hide();

            ApplyIcons();
        }

        private void ApplyIcons()
        {
            Image Image = Provider.GetImage("Search", ProviderType.APPLICATIONICON, PictureSize.Dimension_32X32);

            bbiSearch.Glyph = Image;
            bbiSearch.LargeGlyph = Image;

        }
        public event EventHandler<RateSearchCellClickedEventArgs> CellClicked;
        public event EventHandler<RateSearchCellClickedEventArgs> RateSelected;

        public void InitializeUI()
        {
            this.FormSize = new System.Drawing.Size(1050, 580);

            //if (!DesignMode)
            //{
            //    Utility.AdjustRibbon(lciRateSearch);
            //}

            CNETFooterRibbon.ribbonControl = rcRateSearch;
            gv_rateSearch.RowCellClick += frmRateSearch_RowCellClick;
            CellClicked += frmRateSearch_CellClicked;
            // CellClicked += xx;
            try
            {

                RateSelected += frmRateSearch_RateSelected;
            }
            catch
            { }
            this.StartPosition = FormStartPosition.CenterParent;

            gc_rateSearch.Cursor = Cursors.Hand;
            repositoryItemRadioGroup1.EditValueChanged += repositoryItemRadioGroup1_EditValueChanged;
            repositoryItemRadioGroup1.SelectedIndexChanged += repositoryItemRadioGroup1_SelectedIndexChanged;

            EvaluateVisibilityforGroupSelector();

        }

        void frmRateSearch_Paint(object sender, PaintEventArgs e)
        {
            GridControl gridC = sender as GridControl;
            GridView gridView = gridC.FocusedView as GridView;
            BandedGridViewInfo viewinfo = gridView.GetViewInfo() as BandedGridViewInfo;
            BandedGridViewRects gridViewRects = viewinfo.ViewRects;
            Rectangle r = gridViewRects.BandPanel;
            GridColumnsInfo gci = viewinfo.ColumnsInfo;
            int y = gci[gridView.Columns["Fax"]].Bounds.Y - r.Height;
            int x = gci[gridView.Columns["Fax"]].Bounds.Right;
            Point p1 = new Point(x, y);
            int y2 = gridViewRects.Rows.Bottom;
            Point p2 = new Point(x, y2);
            e.Graphics.DrawLine(Pens.Red, p1, p2);
        }

        public frmRateDetailInfo frmRateDetailInfo;


        void frmRateSearch_RateSelected(object sender, RateSearchCellClickedEventArgs e)
        {
            if (e.RowName != "Physical Inventory" && e.CellValue.ToString() != "")
            {
                cellArgs = new RateSearchCellClickedEventArgs(e.RoomType.Value, e.RowCode.Value, e.RowName, e.CellValue.ToString());
            }

        }
        bool IsAverage = true;
        void repositoryItemRadioGroup1_SelectedIndexChanged(object sender, EventArgs e)
        {
            RadioGroup radioGroup = sender as RadioGroup;

            //  radioGroup.EditValue "Average Rate"
            if (radioGroup.Properties.Items[radioGroup.SelectedIndex].ToString().Equals("Total Rate"))
            {
                IsAverage = false;
                ShowAvailableRoomCount();
            }
            if (radioGroup.Properties.Items[radioGroup.SelectedIndex].ToString().Equals("Average Rate"))
            {
                IsAverage = true;
                ShowAvailableRoomCount();
            }
        }

        void repositoryItemRadioGroup1_EditValueChanged(object sender, EventArgs e)
        {

        }
        List<RateCodeDetailDTO> rateDetailList = UIProcessManager.SelectAllRateCodeDetail();

        void frmRateSearch_CellClicked(object sender, RateSearchCellClickedEventArgs e)
        {
            if (e != null)
            {
                TotalAmount = 0;
                FirstNightAmount = 0;
                AverageRateAmt = 0;
                var rate = _reCodeHeaders.FirstOrDefault(d => d.Id == e.RowCode);
                if (rate != null)
                {
                    int rateCode = rate.Id;
                    var availableRate = _availableRates.FirstOrDefault(r => r.RateCode == rateCode & r.RoomType == e.RoomType);
                    if (availableRate != null)
                    {
                        List<DailyRateCodeDTO> dailyRateCodes = availableRate.DailyRateCode.Select(s => new DailyRateCodeDTO()
                        {
                            RateCodeDetail = s.RateCodeDetail,
                            StayDate = s.StayDate,
                            UnitRoomRate = s.UnitRoomRate,
                            DayWeek = s.StayDate.Value.DayOfWeek,
                            Description = rateDetailList.FirstOrDefault(r => r.Id == s.RateCodeDetail) != null ? rateDetailList.FirstOrDefault(r => r.Id == s.RateCodeDetail).Description : String.Empty
                        }).ToList();



                        frmRateDetailInfo = new frmRateDetailInfo
                        {
                            RateSearchCellClickedEventArgs = e,
                            AvailableRate = availableRate
                        };
                        TotalAmount = availableRate.TotalAmount;
                        FirstNightAmount = availableRate.FirstNightAmount;
                        AverageRateAmt = availableRate.AverageAmount;
                        frmRateDetailInfo.AvailableRate = availableRate;
                        frmRateDetailInfo.AvailableRate.DailyRateCode = dailyRateCodes;
                        if (isCellSelected)
                        {
                            frmRateDetailInfo.ShowDialog();
                        }
                    }
                }
            }
        }

        void frmRateSearch_RowCellClick(object sender, RowCellClickEventArgs e)
        {
            if (gv_rateSearch.FocusedColumn.VisibleIndex == 0) return;

            GridView view = sender as GridView;
            cellArgs = null;
            DataRowView row = (DataRowView)view.GetFocusedRow();
            var firstOrDefault = roomTypes.FirstOrDefault(u => u.Description == e.Column.ToString());
            int Ratecode = Convert.ToInt32(row[0].ToString());
            String RateName = row[1].ToString();
            if (firstOrDefault != null)
            {
                var roomtype = firstOrDefault.Id;
                if (RateName != "Physical Inventory" && e.CellValue.ToString() != "")
                {
                    cellArgs = new RateSearchCellClickedEventArgs(roomtype, Ratecode, RateName, e.CellValue.ToString());
                    bool canpass = CheckRate(Ratecode, e.CellValue.ToString());
                    if (!canpass)
                    {
                        bbiSelect.Enabled = false;
                        return;
                    }
                    else
                    {

                        bbiSelect.Enabled = true;
                    }
                }
            }
            if (e.Clicks == 2 && RateName != "Physical Inventory"/* Ratecode != "Physical Inventory"*/ && e.CellValue.ToString() != "")// Double Click
            {
                var roomtype = firstOrDefault.Id;
                cellArgs = new RateSearchCellClickedEventArgs(roomtype, Ratecode, RateName, e.CellValue.ToString());
                bool canpass = CheckRate(Ratecode, e.CellValue.ToString());

                if (!canpass)
                {
                    bbiSelect.Enabled = false;
                    return;
                }
                else
                {

                    bbiSelect.Enabled = true;
                }

                //  object cellValue = e.CellValue;
                isCellSelected = false;
                FireCellCliked();
                FireRateSelected();



                this.Hide();
            }
        }

        List<PackageDetailDTO> PackageDetailList { get; set; }
        public bool CheckRate(int Ratecode, string value)
        {
            bool pass = false;
            decimal selectedamount = 0;
            decimal Packageamount = 0;
            if (!string.IsNullOrEmpty(value))
            {
                selectedamount = Convert.ToDecimal(value);
            }
            decimal TotalPackageamount = 0;
            RateCodeHeaderDTO rateCode = UIProcessManager.GetRateCodeHeaderById(Ratecode);
            if (rateCode != null)
            {
                List<RateCodePackageDTO> rateCodePackages = UIProcessManager.GetRateCodePackagesByrateCodeHeader(rateCode.Id);
                foreach (RateCodePackageDTO package in rateCodePackages)
                {
                    Packageamount = 0;
                    PackageDetailDTO packagedetail = PackageDetailList.FirstOrDefault(x => x.PackageHeader == package.PackageHeader);
                    if (packagedetail != null)
                    {
                        selectedamount = selectedamount * GetLatestExchangeRate(rateCode.CurrencyCode);

                        Packageamount = packagedetail.Price.Value * GetLatestExchangeRate(rateCode.CurrencyCode);

                        TotalPackageamount += Packageamount;
                    }
                }
            }

            if (selectedamount <= TotalPackageamount)
            {
                if (XtraMessageBox.Show("The Room rate is less than the package !!" + Environment.NewLine +
                      "The room rate is " + selectedamount + " and Package is " + TotalPackageamount + " !!" + Environment.NewLine + "Would you like to Continue ??", "CNETERPV6", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == System.Windows.Forms.DialogResult.OK)
                {
                    pass = true;
                }
            }
            else
            {
                pass = true;

            }
            return pass;
        }
        List<ExchangeRateDTO> AllExchangeRate { get; set; }
        private decimal GetLatestExchangeRate(int fromcurrency)
        {
            int defaultCurrency = 0;
            decimal exchangeRate = 1;
            var firstOrDefault = LocalBuffer.LocalBuffer.CurrencyBufferList.FirstOrDefault(r => r.IsDefault);
            if (firstOrDefault != null)
            {
                defaultCurrency = firstOrDefault.Id;
            }
            if (fromcurrency == defaultCurrency)
            {
                exchangeRate = 1;
            }
            else
            {
                if (AllExchangeRate != null && AllExchangeRate.Count > 0)
                {
                    var CNETLibrary = AllExchangeRate.OrderByDescending(r => r.Date).FirstOrDefault(r => r.FromCurrency == fromcurrency && (r.ToCurrency == defaultCurrency || r.ToCurrency == 0));
                    if (CNETLibrary != null)
                        exchangeRate = CNETLibrary.Buying;
                }
                else
                {
                    exchangeRate = 1;
                }
            }
            return exchangeRate;
        }

        public void InitializeData()
        {
            dataTable.PrimaryKey = null;
            dataTable.Columns.Clear();
            //List<RoomType> tests = UIProcessManager.SelectAllRoomType().Where(r => r.isActive).ToList();
            List<RoomTypeDTO> tests = UIProcessManager.GetRoomTypeByConsigneeUnit(SelectedHotelcode).Where(r => r.IsActive).ToList();

            DataColumn dataColumn = dataTable.Columns.Add();
            dataColumn.Caption = "Id";
            dataColumn = dataTable.Columns.Add();
            dataColumn.Caption = "Name";

            PopulateRoomTypes(tests);
            ClearSearchResut();

            if (_requestingForm.GetType() == typeof(frmGroupRegistration))
            {
                if (_availableRates == null || _availableRates.Count == 0)
                {
                    GetStaticComponents();
                }
            }
            else
            {
                GetStaticComponents();
            }

            ShowAvailableRoomCount();

            dataTable.PrimaryKey = new[] { dataTable.Columns[1] };
            gc_rateSearch.Cursor = Cursors.Hand;

            gc_rateSearch.DataSource = dataTable;

            gv_rateSearch.PopulateColumns();
            gv_rateSearch.Columns[0].Visible = false;
            gv_rateSearch.Columns[1].BestFit();
        }

        public void SetFromAndToDates()
        {
            _fromDate = Convert.ToDateTime(beiStartDate.EditValue);
            _toDate = Convert.ToDateTime(beiEndDate.EditValue);
        }

        #region UI

        /*
         * DATA POPULATING STEPS
         * 
         * first populate the columns  by using PopulateRoomTypes
         *use ShowSearchResult 
         * 
         */

        #region Private Class Variables

        private List<String> valueName = new List<string>();
        private List<RoomTypeDTO> roomTypes = new List<RoomTypeDTO>();
        private DataTable dataTable = new DataTable();

        #endregion

        #region Public Methods

        public void AddNameValues(String nameValue)
        {
            if (String.IsNullOrEmpty(nameValue)) return;

            if (IsNewValueName(nameValue)) return;

            valueName.Add(nameValue);
        }

        public void PopulateRoomTypes(List<RoomTypeDTO> roomTypes)
        {
            if (roomTypes == null || !roomTypes.Any()) return;

            roomTypes = roomTypes.Distinct().ToList();

            this.roomTypes = roomTypes;

            foreach (RoomTypeDTO roomType in roomTypes)
            {
                DataColumn dataColumn = dataTable.Columns.Add();
                dataColumn.Caption = roomType.Description;


            }
        }

        public void ShowSearchResult(int valueCode, String valueName, String roomTypeDescription, String value)
        {
            if (String.IsNullOrEmpty(valueName) || String.IsNullOrEmpty(roomTypeDescription))
                return;

            if (!roomTypes.Any()) return;

            DataRow row = null;

            if (IsNewValueName(valueName)) //create a new row
            {
                row = CreateNewRow(valueCode, valueName);
                AddNameValues(valueName);
                FindAndAssignColumnValue(roomTypeDescription, value, row);
            }
            else
            {
                //Find the row
                foreach (DataRow _row in dataTable.Rows)
                {
                    if (IsStringEqual(_row[dataTable.Columns[1]].ToString(), valueName))
                    {
                        foreach (DataColumn _column in dataTable.Columns)
                        {
                            if (dataTable.Columns.IndexOf(_column) == 1) continue;

                            if (IsStringEqual(_column.Caption.ToString(), roomTypeDescription))
                            {
                                row = _row;
                                goto FINISHEDASSIGNING;
                            }
                        }
                    }
                }

                FINISHEDASSIGNING:
                if (row != null)
                    FindAndAssignColumnValue(roomTypeDescription, value, row);
            }
        }

        public void ClearSearchResut()
        {
            if (!(dataTable.Columns.Count > 1 && dataTable.Rows.Count > 0)) return;

            foreach (DataColumn column in dataTable.Columns)
            {
                if (dataTable.Columns.IndexOf(column) == 0) continue;

                foreach (DataRow row in dataTable.Rows)
                {
                    row[column] = String.Empty;
                }

            }

        }

        public void ClearRoomType()
        {
            dataTable.Clear();

            dataTable.Rows.Clear();

            dataTable.Columns.Clear();
        }

        #endregion

        #region Private Methods

        private bool IsStringEqual(String value1, String value2)
        {
            if (string.IsNullOrEmpty(value1) || String.IsNullOrEmpty(value2))
                return false;

            return value1.ToLower().Trim().Equals(value2.ToLower().Trim());
        }

        private bool IsNewValueName(String nameValue)
        {
            foreach (DataRow row in dataTable.Rows)
            {
                String value = row[dataTable.Columns[1]].ToString();

                if (IsStringEqual(value, nameValue))
                {
                    return false;
                }
            }

            return true;
        }

        private DataRow CreateNewRow(int valuecode, String valueName)
        {
            DataRow row = dataTable.NewRow();
            row[dataTable.Columns[0]] = valuecode;
            row[dataTable.Columns[1]] = valueName;
            dataTable.Rows.Add(row);

            return row;
        }

        private Boolean FindAndAssignColumnValue(String roomTypeDescription, String value, DataRow row)
        {
            if (String.IsNullOrEmpty(value) || String.IsNullOrEmpty(roomTypeDescription))
                return false;

            if (!roomTypes.Any()) return false;

            foreach (DataColumn column in dataTable.Columns)
            {
                if (IsStringEqual(column.Caption, roomTypeDescription))
                {
                    row[column] = value;

                    return true;
                }
            }

            return false;
        }
        List<Tuple<string, int>> availableRoomCounts;
        public void GetStaticComponents()
        {
            _availableRates = new List<AvailableRateDTO>();
            availableRoomCounts =
                       UIProcessManager.GetAvailableRoomCount(roomTypes, _fromDate, _toDate);
            RegistrationInfoDTO registration = new RegistrationInfoDTO();
            if (_rateCode != null)//&& _rateCode != "Physical Inventory")
            {
                //RateCodeHeader rateHeader = UIProcessManager.SelectAllRateCode().FirstOrDefault(r => r.description == _rateCode);
                //if (rateHeader != null) registration.RateCode = rateHeader.code;
                registration.RateCode = null;
            }

            else
            {
                registration.RateCode = null;
            }
            short counter;

            short.TryParse(_adultCount, out counter);
            registration.AdultCount = counter;
            short.TryParse(_childCount, out counter);
            registration.ChildCount = counter;
            registration.ArrivalDate = _fromDate;
            registration.DepartureDate = _toDate;

            registration.RTC = _roomType;

            short.TryParse(_roomCount, out counter);
            registration.RoomCount = counter;

            _availableRatesGenerated = UIProcessManager.GetAvailableRates(registration, SelectedHotelcode);
            if (_availableRatesGenerated != null)
            {
                if (_availableRatesGenerated.availableRates != null)
                    _availableRates = _availableRatesGenerated.availableRates;

                if (_availableRatesGenerated.rateCodeHeaders != null)
                    _reCodeHeaders = _availableRatesGenerated.rateCodeHeaders;
            }
        }

        private void ShowAvailableRoomCount()
        {
            SetFromAndToDates();

            if (roomTypes.Count > 0)
            {


                foreach (var roomType in availableRoomCounts)
                {
                    ShowSearchResult(9999, "Physical Inventory", roomType.Item1, roomType.Item2.ToString());
                }


                if (_reCodeHeaders != null)
                {
                    foreach (var rateHeader in _reCodeHeaders)
                    {
                        foreach (var roomType in roomTypes)
                        {

                            AvailableRateDTO availableRate =
                                _availableRates.FirstOrDefault(
                                    r => r.RateCode == rateHeader.Id & r.RoomType == roomType.Id);

                            // Check Rate Strategy
                            if (availableRate != null)
                            {
                                bool rateStrategyFlag = true;
                                var roomInventory = availableRoomCounts.FirstOrDefault(r => r.Item1 == roomType.Description);
                                if (roomInventory != null)
                                {
                                    rateStrategyFlag = ApplyRateStrategy(roomType.NumberOfRooms.Value, roomInventory.Item2, rateHeader.Id, _fromDate);
                                }

                                if (!rateStrategyFlag) continue;

                            }


                            ShowSearchResult(rateHeader.Id, rateHeader.Description, String.Empty, String.Empty);

                            if (availableRate != null)
                            {
                                if (IsAverage)
                                {
                                    ShowSearchResult(rateHeader.Id, rateHeader.Description, roomType.Description,
                                        availableRate.AverageAmount.ToString(CultureInfo.InvariantCulture));
                                }
                                else
                                {
                                    ShowSearchResult(rateHeader.Id, rateHeader.Description, roomType.Description,
                                           availableRate.TotalAmount.ToString(CultureInfo.InvariantCulture));
                                }
                                //break;
                            }
                        }
                    }

                }
            }
        }

        private bool ApplyRateStrategy(int totalRoom, int availabeRoom, int rateCode, DateTime date)
        {
            /*
            List<vw_RateStrategyView> rateStrategyViewList =  UIProcessManager.GetRateStrategyView(rateCode, date);
            if (rateStrategyViewList == null || rateStrategyViewList.Count == 0) return true;

            vw_RateStrategyView occupiedStrategy = rateStrategyViewList.FirstOrDefault(r => r.condition == CNETConstantes.RATE_STRATEGY_CONDITION_OCCUPIED);
            vw_RateStrategyView openStrategy = rateStrategyViewList.FirstOrDefault(r => r.condition == CNETConstantes.RATE_STRATEGY_CONDITION_OPEN);
            if (occupiedStrategy != null && openStrategy != null)
            {
                if (occupiedStrategy.priority.Value <= openStrategy.priority.Value)
                {

                    if (occupiedStrategy != null)
                    {
                        decimal conditionValue = 0;
                        if (occupiedStrategy.isPercent)
                            conditionValue = (totalRoom - availabeRoom) / totalRoom * 100;
                        else
                            conditionValue = totalRoom - availabeRoom;

                        if (conditionValue >= occupiedStrategy.value)
                        {
                            if (occupiedStrategy.restrictionType == CNETConstantes.RATE_STRATEGY_RESTRICTION_OPEN)
                                return true;
                            else
                                return false;
                        }

                    }


                    if (openStrategy != null)
                    {
                        decimal conditionValue = 0;
                        if (openStrategy.isPercent)
                            conditionValue = (availabeRoom) / totalRoom * 100;
                        else
                            conditionValue = availabeRoom;

                        if (conditionValue >= openStrategy.value)
                        {
                            if (openStrategy.restrictionType == CNETConstantes.RATE_STRATEGY_RESTRICTION_OPEN)
                                return true;
                            else
                                return false;
                        }

                    }
                }
                else
                {
                    if (openStrategy != null)
                    {
                        decimal conditionValue = 0;
                        if (openStrategy.isPercent)
                            conditionValue = (availabeRoom) / totalRoom * 100;
                        else
                            conditionValue = availabeRoom;

                        if (conditionValue >= openStrategy.value)
                        {
                            if (openStrategy.restrictionType == CNETConstantes.RATE_STRATEGY_RESTRICTION_OPEN)
                                return true;
                            else
                                return false;
                        }

                    }


                    if (occupiedStrategy != null)
                    {
                        decimal conditionValue = 0;
                        if (occupiedStrategy.isPercent)
                            conditionValue = (totalRoom - availabeRoom) / totalRoom * 100;
                        else
                            conditionValue = totalRoom - availabeRoom;

                        if (conditionValue >= occupiedStrategy.value)
                        {
                            if (occupiedStrategy.restrictionType == CNETConstantes.RATE_STRATEGY_RESTRICTION_OPEN)
                                return true;
                            else
                                return false;
                        }

                    }
                }
            }
            else
            {
                if (occupiedStrategy != null)
                {
                    decimal conditionValue = 0;
                    if (occupiedStrategy.isPercent)
                        conditionValue = (totalRoom - availabeRoom) / totalRoom * 100;
                    else
                        conditionValue = totalRoom - availabeRoom;

                    if (conditionValue >= occupiedStrategy.value)
                    {
                        if (occupiedStrategy.restrictionType == CNETConstantes.RATE_STRATEGY_RESTRICTION_OPEN)
                            return true;
                        else
                            return false;
                    }

                }


                if (openStrategy != null)
                {
                    decimal conditionValue = 0;
                    if (openStrategy.isPercent)
                        conditionValue = (availabeRoom) / totalRoom * 100;
                    else
                        conditionValue = availabeRoom;

                    if (conditionValue >= openStrategy.value)
                    {
                        if (openStrategy.restrictionType == CNETConstantes.RATE_STRATEGY_RESTRICTION_OPEN)
                            return true;
                        else
                            return false;
                    }

                }
            }
          
            */
            return true;
        }

        #endregion

        #endregion

        public void LoadData(UILogicBase requesterForm, object args)
        {
            _requestingForm = requesterForm;
            if (requesterForm.GetType() == typeof(frmReservation) || requesterForm.GetType() == typeof(frmGroupRegistration))
            {
                var arguments = (Dictionary<string, string>)args;

                beiStartDate.EditValue = arguments["fromDate"];
                beiEndDate.EditValue = arguments["toDate"];

                _adultCount = arguments["adultCount"];
                _childCount = arguments["childCount"];
                _roomCount = arguments["roomCount"];
                //_rateCode = arguments["rateCode"];
                //_roomType = arguments["roomType"];


                int rat = 0;
                Int32.TryParse(arguments["rateCode"], out rat);//
                _rateCode = rat == 0 ? null : rat;

                int romt = 0;
                Int32.TryParse(arguments["roomType"], out romt);//
                _roomType = romt == 0 ? null : romt;

                Text = @"Rate Search " + _fromDate.ToShortDateString() + @" - " + _toDate.ToShortDateString();
            }
            if (requesterForm.GetType() == typeof(frmDateAmendment) || requesterForm.GetType() == typeof(frmRegistrationDetail) || requesterForm.GetType() == typeof(frmRoomMove))
            {
                var arguments = (Dictionary<string, string>)args;

                beiStartDate.EditValue = arguments["fromDate"];
                beiEndDate.EditValue = arguments["toDate"];

                _adultCount = arguments["adultCount"];
                _childCount = arguments["childCount"];

                int rat = 0;
                Int32.TryParse(arguments["rateCode"], out rat);//
                _rateCode = rat == 0 ? null : rat;

                int romt = 0;
                Int32.TryParse(arguments["roomType"], out romt);//
                _roomType = romt == 0 ? null : romt;


                Text = @"Rate Search " + _fromDate.ToShortDateString() + @" - " + _toDate.ToShortDateString();
            }

        }

        private void bbiSearch_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ShowAvailableRoomCount();
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

        private void beiEndDate_EditValueChanged(object sender, EventArgs e)
        {
            ShowAvailableRoomCount();
            Text = @"Rate Search " + _fromDate.ToShortDateString() + @" - " + _toDate.ToShortDateString();
        }

        private void beiStartDate_EditValueChanged(object sender, EventArgs e)
        {
            ShowAvailableRoomCount();
            Text = @"Rate Search " + _fromDate.ToShortDateString() + @" - " + _toDate.ToShortDateString();
        }

        private void rideStartDate_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void rideEndDate_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void beiRate_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void bbiRateInfo_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            // frmRateSearch_RowCellClick(this,new  RowCellClickEventArgs e);
            isCellSelected = true;
            FireCellCliked();
        }

        private void FireCellCliked()
        {
            if (cellArgs == null)
            {
                SystemMessage.ShowModalInfoMessage("No Rate Amount Selected", "ERROR");
                return;
            }

            if (CellClicked != null)
                CellClicked(this, cellArgs);
        }


        private void FireRateSelected()
        {
            if (cellArgs == null || cellArgs.RowName == "Physical Inventory")
            {
                SystemMessage.ShowModalInfoMessage("No Rate Selected", "ERROR");
                return;
            }
            if (RateSelected != null)
                RateSelected(this, cellArgs);
        }
        private void bbiSelect_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            // DialogResult = System.Windows.Forms.DialogResult.OK;
            isCellSelected = false;
            FireCellCliked();
            FireRateSelected();
            this.Hide();


        }

        private void frmRateSearch_Load(object sender, EventArgs e)
        {
            // Progress_Reporter.Show_Progress("Loading data.Please wait...");
            InitializeData();

            ////CNETInfoReporter.Hide();
        }

        private void bbiClose_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Close();
        }

        public int SelectedHotelcode { get; set; }
    }


    public class RateSearchCellClickedEventArgs : EventArgs
    {

        public RateSearchCellClickedEventArgs(int roomType, int rowCode, String rowName, String cellValue)
        {
            this.RoomType = roomType;
            this.RowCode = rowCode;
            this.RowName = rowName;
            this.CellValue = cellValue;

        }


        public int? RoomType { get; set; }

        public int? RowCode { get; set; }
        public String RowName { get; set; }

        public String CellValue { get; set; }




    }
}