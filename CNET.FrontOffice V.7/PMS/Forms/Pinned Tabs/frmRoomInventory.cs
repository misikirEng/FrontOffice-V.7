using CNET.ERP.Client.Common.UI;
using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using CNET.FrontOffice_V._7.Forms.Non_Navigatable_Modals;
using System.Drawing;
using DevExpress.Utils;
using DevExpress.XtraCharts;
using DevExpress.XtraCharts;
using ProcessManager;
using CNET_V7_Domain.Domain.PmsSchema;
using CNET_V7_Domain.Domain.ViewSchema;
using DocumentPrint;

namespace CNET.FrontOffice_V._7.Forms
{
    public partial class frmRoomInventory : UILogicBase
    {
        DateTime defaultDateTime;
        public frmRoomInventory()
        {
            InitializeComponent();
            dataTable = new DataTable();
            DateTime? date = UIProcessManager.GetServiceTime();
            if (date != null)
            {
                defaultDateTime = date.Value;
            }
            else
            {
                XtraMessageBox.Show("Server DateTime Error !!", "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return;
            }
            //   defaultDateTime = UIProcessManager.GetServerDateTime("Gregorian", "Server");

            selectedYear = defaultDateTime.Year.ToString();
            selectedMonth = defaultDateTime.ToString("MMMM");
            beiYear.EditValue = selectedYear;
            beiMonth.EditValue = selectedMonth;

            reiHotel.DisplayMember = "Name";
            reiHotel.ValueMember = "Id";
            reiHotel.DataSource = (LocalBuffer.LocalBuffer.HotelBranchBufferList.Select(x => new { x.Id, x.Name })).ToList();

            if (LocalBuffer.LocalBuffer.HotelBranchBufferList != null && LocalBuffer.LocalBuffer.HotelBranchBufferList.Count > 0)
            { beiHotel.EditValue = LocalBuffer.LocalBuffer.CurrentDeviceConsigneeUnit; }
            //reiHotel.ReadOnly = !MasterPageForm.UserHasHotelBranchAccess;
            if (SelectedHotelcode != null)
            {
                _roomtypes = UIProcessManager.GetRoomTypeByConsigneeUnit(SelectedHotelcode.Value).Where(r => !r.IspseudoRoomType.Value && r.IsActive).ToList();
                SetRoomTypes(_roomtypes);

                BuildTableforSelectedDate();

            }

            summaryFields = new List<string>();

            CellClicked += frmDensityChart_CellClicked;

        }


        #region Class Variables
        private DataTable dataTable;
        private String selectedYear = String.Empty;
        private String selectedMonth = String.Empty;
        public const String DisabledDayIndicator = "X";
        private List<RoomTypeDTO> roomTypes;
        private List<RoomTypeDTO> _roomtypes;
        private DateTime _fromDate;
        private DateTime _toDate;
        #endregion

        #region Public Methods

        public void SetRoomTypes(List<RoomTypeDTO> roomTypes)
        {
            this.roomTypes = roomTypes.Distinct(new roomtypeEqualityComparer()).ToList();

        }

        public void ShowInfo(string roomType, int monthDay, int infoToShow)
        {
            if (roomTypes != null && roomTypes.Any())
            {
                if (dataTable.Rows.Count > 0)
                {
                    var result = dataTable.Rows.Find(roomType);

                    if (result != null)
                    {
                        if (monthDay <= 31 && monthDay > 0)
                        {
                            result[monthDay] = infoToShow;
                        }
                    }
                }
            }
        }

        public void ClearInfo()
        {
            if (roomTypes != null && roomTypes.Any())
            {
                foreach (DataRow rmType in dataTable.Rows)
                {
                    for (int i = 1; i <= 31; i++)
                    {
                        if (rmType[i] != DisabledDayIndicator)
                            rmType[i] = String.Empty;
                    }
                }
            }
        }

        #endregion

        #region Room Type Comparer

        class roomtypeEqualityComparer : IEqualityComparer<RoomTypeDTO>
        {
            public bool Equals(RoomTypeDTO x, RoomTypeDTO y)
            {
                return x.Description.Equals(y.Description) && x.Consigneeunit.Equals(y.Consigneeunit);
            }

            public int GetHashCode(RoomTypeDTO obj)
            {
                return obj.GetHashCode();
            }
        }

        #endregion

        #region Private Methods

        private void BuildTableData(String monthName, String year)
        {
            dataTable.Clear();

            if (String.IsNullOrEmpty(monthName) || String.IsNullOrEmpty(year))
            {
                //     InvalidEntry();

                return;
            }
            try
            {
                int daysInMonth = GetDaysInMonth(monthName, year);
                int _disabledDays = 31 - daysInMonth;

                BuildDataTableColumns();

                foreach (RoomTypeDTO roomType in roomTypes)
                {
                    DataRow row = dataTable.NewRow();


                    row[dataTable.Columns[0]] = roomType.Description;

                    int disabledDays = _disabledDays;

                    #region Disable Months Extra Days

                    while (disabledDays != 0)
                    {
                        row[dataTable.Columns[(32 - disabledDays)]] = DisabledDayIndicator;

                        disabledDays--;
                    }

                    #endregion

                    dataTable.Rows.Add(row);
                }

                dataTable.PrimaryKey = new[] { dataTable.Columns[0] };



                gc_roomAvailability.DataSource = dataTable;
                gc_roomAvailability.RefreshDataSource();

                gv_roomAvailability.PopulateColumns();

                gv_roomAvailability.Columns[0].BestFit();

                int bestFitWidth = gv_roomAvailability.Columns[1].GetBestWidth();
                summaryFields = new List<string>();

                for (int i = 1; i <= 31; i++)
                {
                    gv_roomAvailability.Columns[i].Width = bestFitWidth;

                    summaryFields.Add(i.ToString());


                }

                Summarize();
                if (_roomtypes.Count > 0)
                {
                    GetFromAndToDates();

                    // Progress_Reporter.Show_Progress("Loading data for Month of " + selectedMonth);

                    List<Tuple<string, int, int>> availableRooms = UIProcessManager.GetAvailableRoom(_roomtypes, _fromDate, _toDate);

                    foreach (var availableRoom in availableRooms)
                    {
                        //   Progress_Reporter.Show_Progress("Loading data for Month of " + availableRoom.Item2, availableRooms.Count);

                        ShowInfo(availableRoom.Item1, availableRoom.Item2, availableRoom.Item3);
                    }

                    // //CNETInfoReporter.Hide();
                }
                CreateChartControl(daysInMonth);
            }
            catch (Exception e)
            {
                //      throw e;
            }
        }

        private void CreateChartControl(int daysInMonth)
        {
            ccRIM.Series.Clear();
            Series series = new Series("Series1", ViewType.StackedSplineArea);
            ccRIM.Legend.Visible = false;
            series.Label.ResolveOverlappingMode = ResolveOverlappingMode.None;
            series.LabelsVisibility = DefaultBoolean.True;
            series.Label.TextOrientation = TextOrientation.Horizontal;
            //  series.Label.
            ccRIM.Series.Add(series);


            // Generate a data table and bind the series to it.
            series.DataSource = CreateChartData(daysInMonth);

            // Specify data members to bind the series.
            series.ArgumentScaleType = ScaleType.Numerical;
            series.ArgumentDataMember = "Argument";
            series.ValueScaleType = ScaleType.Numerical;
            series.ValueDataMembers.AddRange(new string[] { "Value" });

            // Set some properties to get a nice-looking chart.
            ((SideBySideBarSeriesView)series.View).ColorEach = false;
            ((SideBySideBarSeriesView)series.View).BarWidth = 0.1;
            ((XYDiagram)ccRIM.Diagram).AxisY.Visible = false;
            ccRIM.Legend.Visible = false;
        }

        private DataTable CreateChartData(int rowCount)
        {
            // Create an empty table.
            var currentViewer = gv_roomAvailability;
            DataTable table = new DataTable("Table1");
            int totalRoomsCount = _roomtypes.Where(rt => rt.NumberOfRooms != null).Sum(rt => rt.NumberOfRooms.Value);
            // Add two columns to the table.
            table.Columns.Add("Argument", typeof(Int32));
            table.Columns.Add("Value", typeof(Int32));

            // Add data rows to the table.
            Random rnd = new Random();
            DataRow row = null;
            for (int i = 1; i <= rowCount; i++)
            {
                GridColumnSummaryItem summaryItem = new GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, currentViewer.Columns[i].FieldName, "{0:#.##}");
                currentViewer.Columns[i].Summary.Add(summaryItem);
                int si = Convert.ToInt16(summaryItem.SummaryValue);
                currentViewer.Columns[i].Summary.Remove(summaryItem);
                if (si != null)
                {
                    row = table.NewRow();
                    row["Argument"] = i;
                    row["Value"] = totalRoomsCount - si;// rnd.Next(100);
                    table.Rows.Add(row);
                }
            }

            return table;
        }




        private DateTime GetFocusedDate()
        {
            String day = gv_roomAvailability.FocusedColumn.AbsoluteIndex.ToString();

            DateTime result = Convert.ToDateTime(day + selectedMonth + selectedYear + " 12:08:54.183");

            return result;



        }



        public event EventHandler<DensityChartClickedEventArgs> CellClicked;

        private List<String> summaryFields;

        public void Summarize()
        {
            var currentViewer = gv_roomAvailability;
            System.Drawing.Font font = currentViewer.Appearance.FooterPanel.Font;
            currentViewer.Appearance.FooterPanel.ForeColor = ColorTranslator.FromHtml("Green");
            currentViewer.Appearance.FooterPanel.Font = new System.Drawing.Font(font.FontFamily, font.Size, FontStyle.Bold);

            currentViewer.OptionsView.ShowFooter = true;
            if (currentViewer.Columns.Count == 0) return;

            if (summaryFields != null)
            {

                foreach (String sf in summaryFields)
                {
                    try
                    {
                        int index = Convert.ToInt16(sf);

                        GridColumnSummaryItem summaryItem = new GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, currentViewer.Columns[Convert.ToInt16(sf)].FieldName, "{0:#.##}");
                        currentViewer.Columns[index].Summary.Add(summaryItem);
                        currentViewer.Columns[Convert.ToInt16(sf)].BestFit();
                    }
                    catch
                    {
                    }
                }
            }
            GridColumnSummaryItem summaryItemTotal = new GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Custom, currentViewer.Columns[0].FieldName, "Total");
            currentViewer.Columns[0].Summary.Add(summaryItemTotal);
            currentViewer.Columns[0].BestFit();
        }

        private void BuildDataTableColumns()
        {
            dataTable.PrimaryKey = null;
            dataTable.Columns.Clear();
            //Build Columns
            //dataTable.Rows.Clear();
            for (int i = 0; i <= 31; i++)
            {
                if (i == 0)
                {
                    DataColumn nameColumn = dataTable.Columns.Add();
                    nameColumn.Caption = "Room Type";
                }
                else
                {
                    DataColumn monthDay = dataTable.Columns.Add();


                    monthDay.Caption = i.ToString();
                }
            }
        }

        private int GetDaysInMonth(String monthName, String year)
        {
            int _year = Convert.ToInt16(year);
            int month = DateTime.ParseExact(monthName, "MMMM", new CultureInfo("en-US")).Month;

            var daysInMonth = DateTime.DaysInMonth(_year, month);

            return daysInMonth;
        }

        private void InvalidEntry()
        {
            //TODO: show validation

            XtraMessageBox.Show("Please Enter Month and Year First.", "Validation Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);

        }
        private void frmDensityChart_Load(object sender, EventArgs e)
        {
            PopulateYearCombo();

            //_roomtypes = UIProcessManager.SelectAllRoomType().Where(r => !r.isPseudoRoom.Value && r.isActive).ToList();
            //SetRoomTypes(_roomtypes);

            beiDateTime.EditValue = defaultDateTime;

            if (SelectedHotelcode != null)
                GetRoomInventoryForcast(Convert.ToDateTime(beiDateTime.EditValue.ToString()));
        }

        private void PopulateYearCombo()
        {
            int startYear = 2016;
            int endYear = 2030;
            while (startYear <= endYear)
            {
                ricbYear.Items.Add(startYear);
                startYear++;
            }

            bbiShowPrintPreview.ItemClick += bbiShowPrintPreview_ItemClick;

            ricbYear.EditValueChanging += ricbYear_EditValueChanging;
            ricbMonth.EditValueChanging += ricbMonth_EditValueChanging;
        }

        private void BuildTableforSelectedDate()
        {
            if (selectedMonth == null || selectedYear == null)
            {
                //   InvalidEntry();
                //   return;

            }

            BuildTableData(selectedMonth, selectedYear);

        }

        private void GetFromAndToDates()
        {
            _fromDate = Convert.ToDateTime("1" + selectedMonth + selectedYear + " 12:08:54.183");
            _toDate =
                Convert.ToDateTime(GetDaysInMonth(selectedMonth, selectedYear) + selectedMonth + selectedYear + " 12:08:54.183");
        }

        #endregion

        #region Event Listener

        void bbiShowPrintPreview_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }

        void ricbMonth_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {
            selectedMonth = e.NewValue.ToString();

            BuildTableforSelectedDate();
        }

        void ricbYear_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {
            selectedYear = e.NewValue.ToString();
            BuildTableforSelectedDate();
        }

        void frmDensityChart_CellClicked(object sender, DensityChartClickedEventArgs e)
        {

        }



        #endregion

        private void bbiRefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (SelectedHotelcode == null)
            {
                XtraMessageBox.Show("Select Hotel First !!");
                return;
            }

            if (SelectedHotelcode != null)
            {
                _roomtypes = UIProcessManager.GetRoomTypeByConsigneeUnit(SelectedHotelcode.Value).Where(r => !r.IspseudoRoomType.Value && r.IsActive).ToList();
                SetRoomTypes(_roomtypes);
            }
            if (xtcTabControl.SelectedTabPage == xtpMonthlyRoomInventory)
            {
                BuildTableforSelectedDate();
            }
            else if (xtcTabControl.SelectedTabPage == xtpDailyRoomInventory)
            {
                if (beiDateTime.EditValue == null || string.IsNullOrEmpty(beiDateTime.EditValue.ToString()))
                {
                    XtraMessageBox.Show("Select Date Time First.", "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                GetRoomInventoryForcast(Convert.ToDateTime(beiDateTime.EditValue.ToString()));
            }
            // //CNETInfoReporter.Hide();
        }

        private void gv_roomAvailability_RowCellClick(object sender, RowCellClickEventArgs e)
        {
            if (gv_roomAvailability.FocusedColumn.VisibleIndex == 0) return;

            GridView view = sender as GridView;

            DataRowView row = (DataRowView)view.GetFocusedRow();

            var roomtype = roomTypes.Where(u => u.Description == row[0].ToString()).FirstOrDefault();

            DateTime date = GetFocusedDate();

            DensityChartClickedEventArgs args = new DensityChartClickedEventArgs(roomtype, date, e.CellValue.ToString());

            if (CellClicked != null)
                CellClicked(this, args);
        }

        private void gv_roomAvailability_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
        {
            string str = defaultDateTime.ToString("MMMM");
            if (defaultDateTime.Year.ToString() == beiYear.EditValue.ToString() && beiMonth.EditValue.ToString() == defaultDateTime.ToString("MMMM") && e.Column.FieldName == "Column" + (defaultDateTime.Day + 1).ToString())
            {
                // e.Appearance.BackColor = Color.Red;
                SolidBrush brush = new SolidBrush(Color.FromArgb(150, Color.Red));
                e.Painter.DrawObject(e.Info);
                e.Graphics.FillRectangle(brush, e.Bounds);
                Color foreColor = e.Info.State == DevExpress.Utils.Drawing.ObjectState.Hot ? Color.Blue : Color.Red;
                int ident = e.Info.State == DevExpress.Utils.Drawing.ObjectState.Hot ? 1 : 2;
                Rectangle textBounds = new Rectangle(e.Bounds.X + ident, e.Bounds.Y + ident, e.Bounds.Width - 10, e.Bounds.Height - 10);
                e.Painter.DrawCaption(e.Info, e.Info.Caption, e.Appearance.Font, new SolidBrush(foreColor), textBounds, StringFormat.GenericDefault);
                e.Handled = true;
            }
        }

        private void bbiShowPrintPreview_ItemClick_1(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (xtcTabControl.SelectedTabPage == xtpMonthlyRoomInventory)
            {
                DateTime? currentTime = UIProcessManager.GetServiceTime();
                if (currentTime == null) return;
                ReportGenerator rg = new ReportGenerator();
                rg.GenerateGridAndChartReport(gc_roomAvailability, ccRIM, "Room Inventory", currentTime.Value);
            }
            else if (xtcTabControl.SelectedTabPage == xtpDailyRoomInventory)
            {
                DateTime? currentTime = UIProcessManager.GetServiceTime();
                if (currentTime == null) return;
                ReportGenerator rg = new ReportGenerator();
                rg.GenerateGridAndChartReport(gcDailyRoomInventory, ccRealTimeOcc, "Room Inventory", currentTime.Value);
            }
        }

        private void xtcTabControl_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            if (xtcTabControl.SelectedTabPage == xtpMonthlyRoomInventory)
            {
                rpgMonth.Visible = true;
                rpgDate.Visible = false;
            }
            else if (xtcTabControl.SelectedTabPage == xtpDailyRoomInventory)
            {
                rpgMonth.Visible = false;
                rpgDate.Visible = true;

            }
        }

        public void GetRoomInventoryForcast(DateTime Date)
        {
            //_roomtypes = UIProcessManager.SelectAllRoomTypebyBranch(SelectedHotelcode).Where(r => !r.isPseudoRoom.Value && r.isActive).ToList();
            //SetRoomTypes(_roomtypes);
            decimal WholeTotalRoom = 0;
            decimal WholeRealTimeAvailable = 0;
            decimal WholeForcastedAvailable = 0;

            List<VwRegistrationDocumentViewDTO> DateRegistrationDocumentList = UIProcessManager.GetRegistrationDocumentViewByDate(Date);
            List<DailyRoomInventoryDTO> DailyRoomInventoryList = new List<DailyRoomInventoryDTO>();

            if (DateRegistrationDocumentList != null && DateRegistrationDocumentList.Count > 0)
            {
                DateRegistrationDocumentList = DateRegistrationDocumentList.GroupBy(x => x.Id).Select(y => y.FirstOrDefault()).ToList();
                //  foreach (RoomType roomT in LocalBuffer.LocalBuffer.RoomTypeBufferList)
                List<RoomDetailDTO> RoomDetailBufferList = UIProcessManager.SelectAllRoomDetail();
                foreach (RoomTypeDTO roomT in _roomtypes)
                {

                    List<RoomDetailDTO> RoomDetailList = RoomDetailBufferList.Where(x => x.RoomType == roomT.Id).ToList();
                    List<int> roomcode = RoomDetailList.Select(x => x.Id).ToList();
                    List<VwRegistrationDocumentViewDTO> RoomTypeRegistrationDocumentList = DateRegistrationDocumentList.Where(x => x.Room != null && roomcode.Contains(x.Room.Value)).ToList();
                    List<VwRegistrationDocumentViewDTO> InHouseRegistrationDocumentList = RoomTypeRegistrationDocumentList.Where(x => x.StartDate.Value.Date <= Date.Date && x.EndDate.Value.Date >= Date.Date && x.LastState == CNETConstantes.CHECKED_IN_STATE).ToList();
                    List<VwRegistrationDocumentViewDTO> StayOverRegistrationDocumentList = RoomTypeRegistrationDocumentList.Where(x => x.StartDate.Value.Date <= Date.Date && x.EndDate.Value.Date > Date.Date && x.LastState == CNETConstantes.CHECKED_IN_STATE).ToList();
                    List<VwRegistrationDocumentViewDTO> CheckOutRegistrationDocumentList = RoomTypeRegistrationDocumentList.Where(x => x.EndDate.Value.Date == Date.Date && x.LastState == CNETConstantes.CHECKED_OUT_STATE).ToList();
                    List<VwRegistrationDocumentViewDTO> DueOutRegistrationDocumentList = RoomTypeRegistrationDocumentList.Where(x => x.EndDate.Value.Date == Date.Date && x.LastState == CNETConstantes.CHECKED_IN_STATE).ToList();
                    List<VwRegistrationDocumentViewDTO> SIXPMRegistrationDocumentList = RoomTypeRegistrationDocumentList.Where(x => x.LastState == CNETConstantes.SIX_PM_STATE).ToList();

                    int TotalRoom = RoomDetailList.Count;
                    decimal RealTimeAvailable = TotalRoom - StayOverRegistrationDocumentList.Count - DueOutRegistrationDocumentList.Count;
                    decimal ForcastedAvailable = TotalRoom - StayOverRegistrationDocumentList.Count - SIXPMRegistrationDocumentList.Count;


                    WholeTotalRoom += TotalRoom;
                    WholeRealTimeAvailable += RealTimeAvailable;
                    WholeForcastedAvailable += ForcastedAvailable;


                    decimal RealTimeAvailableocc = 0;
                    decimal ForcastedAvailableocc = 0;
                    if (TotalRoom > 0)
                    {
                        RealTimeAvailableocc = (RealTimeAvailable / TotalRoom) * 100;
                        ForcastedAvailableocc = (ForcastedAvailable / TotalRoom) * 100;
                    }

                    DailyRoomInventoryDTO DailyRoomInventory = new DailyRoomInventoryDTO
                    {
                        TotalRegistration = RoomTypeRegistrationDocumentList.Count,
                        RoomTypecode = roomT.Id,
                        RoomTypeDescription = roomT.Description,
                        TotalRoom = TotalRoom,
                        InHouseRoom = InHouseRegistrationDocumentList.Count,
                        StayOverRoom = StayOverRegistrationDocumentList.Count,
                        DueOutRoom = DueOutRegistrationDocumentList.Count,
                        CheckOutRoom = CheckOutRegistrationDocumentList.Count,
                        SIXPMRoom = SIXPMRegistrationDocumentList.Count,
                        RealTimeAvailableRoom = RealTimeAvailable,
                        ForcastedAvailableRoom = ForcastedAvailable,
                        RealTimeOccupancyRoom = RealTimeAvailableocc,
                        ForcastedOccupancyRoom = ForcastedAvailableocc,
                    };
                    DailyRoomInventoryList.Add(DailyRoomInventory);
                }
            }
            gcDailyRoomInventory.DataSource = DailyRoomInventoryList;
            decimal WholeRealTimeAvailableocc = 0;
            decimal WholeForcastedAvailableocc = 0;

            if (WholeTotalRoom > 0)
                WholeRealTimeAvailableocc = ((WholeRealTimeAvailable / WholeTotalRoom) * 100);
            gvDailyRoomInventory.Columns["RealTimeOccupancyRoom"].SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Custom;
            gvDailyRoomInventory.Columns["RealTimeOccupancyRoom"].SummaryItem.DisplayFormat = string.Format("{0:N2} %", WholeRealTimeAvailableocc);

            if (WholeTotalRoom > 0)
                WholeForcastedAvailableocc = ((WholeForcastedAvailable / WholeTotalRoom) * 100);
            gvDailyRoomInventory.Columns["ForcastedOccupancyRoom"].SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Custom;
            gvDailyRoomInventory.Columns["ForcastedOccupancyRoom"].SummaryItem.DisplayFormat = string.Format("{0:N2} %", WholeForcastedAvailableocc);



            CreateChartData(WholeTotalRoom, WholeRealTimeAvailable, WholeForcastedAvailable);

        }

        public void CreateChartData(decimal TotalRoom, decimal RealTimeAvailable, decimal ForcastedAvailable)
        {
            SeriesPoint RealTimeAvailablesp = ccRealTimeOcc.Series[0].Points[0];
            RealTimeAvailablesp.Values = new double[] { Convert.ToDouble(RealTimeAvailable) };
            RealTimeAvailablesp.Argument = "RealTime Availability (" + RealTimeAvailable + ")";

            SeriesPoint RealTimeOccupiedsp = ccRealTimeOcc.Series[0].Points[1];
            RealTimeOccupiedsp.Values = new double[] { Convert.ToDouble(TotalRoom - RealTimeAvailable) };
            RealTimeOccupiedsp.Argument = "RealTime Occupancy (" + (Convert.ToDouble(TotalRoom - RealTimeAvailable)) + ")";



            SeriesPoint ForcastedAvailablesp = ccRealTimeOcc.Series[1].Points[0];
            ForcastedAvailablesp.Values = new double[] { Convert.ToDouble(ForcastedAvailable) };
            ForcastedAvailablesp.Argument = "Forcasted Availability (" + ForcastedAvailable + ")";

            SeriesPoint ForcastedOccupiedsp = ccRealTimeOcc.Series[1].Points[1];
            ForcastedOccupiedsp.Values = new double[] { Convert.ToDouble(TotalRoom - ForcastedAvailable) };
            ForcastedOccupiedsp.Argument = "Forcasted Occupancy (" + (Convert.ToDouble(TotalRoom - ForcastedAvailable)) + ")";

            ccRealTimeOcc.RefreshData();
            ccRealTimeOcc.Refresh();

        }

        private void beiHotel_EditValueChanged(object sender, EventArgs e)
        {

            SelectedHotelcode = beiHotel.EditValue == null ? null : Convert.ToInt32(beiHotel.EditValue);

            // beiDateTime.EditValue = defaultDateTime; 
            // _roomtypes = UIProcessManager.SelectAllRoomTypebyBranch(SelectedHotelcode).Where(r => !r.isPseudoRoom.Value && r.isActive).ToList();
            // SetRoomTypes(_roomtypes);
            //// bbiRefresh.PerformClick();

            ////BuildTableforSelectedDate();
            //GetRoomInventoryForcast(Convert.ToDateTime(beiDateTime.EditValue.ToString()));
        }

        public int? SelectedHotelcode { get; set; }




    }




    public class DensityChartClickedEventArgs : EventArgs
    {
        public DensityChartClickedEventArgs(RoomTypeDTO roomType, DateTime dateTime, String value)
        {
            this.RoomType = roomType;
            this.DateTime = dateTime;
            this.Value = value;

        }


        public RoomTypeDTO RoomType { get; set; }

        public DateTime DateTime { get; set; }

        public String Value { get; set; }


    }

    public class DailyRoomInventoryDTO
    {
        public int RoomTypecode { get; set; }
        public string RoomTypeDescription { get; set; }
        public int TotalRoom { get; set; }
        public int TotalRegistration { get; set; }
        public int InHouseRoom { get; set; }
        public int StayOverRoom { get; set; }
        public int DueOutRoom { get; set; }
        public int CheckOutRoom { get; set; }
        public int SIXPMRoom { get; set; }
        public decimal RealTimeAvailableRoom { get; set; }
        public decimal ForcastedAvailableRoom { get; set; }
        public decimal RealTimeOccupancyRoom { get; set; }
        public decimal ForcastedOccupancyRoom { get; set; }
    }
}
