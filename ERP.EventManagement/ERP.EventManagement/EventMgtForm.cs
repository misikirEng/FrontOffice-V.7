using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraScheduler;
using CNET.ERP.Client.Common.UI;
using DevExpress.XtraEditors;
using DevExpress.Utils;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraBars;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Base.ViewInfo;
using ERP.EventManagement.DTO;
using CNET_V7_Domain.Domain.ConsigneeSchema;
using CNET_V7_Domain.Domain.ArticleSchema;
using CNET_V7_Domain;
using ProcessManager;
using CNET_V7_Domain.Domain.SettingSchema;
using CNET_V7_Domain.Domain.PmsSchema;
using ERP.EventManagement.Modals;
using DevExpress.CodeParser;
using DocumentPrint.DTO;
using CNET_V7_Domain.Misc.PmsDTO;
using CNET_V7_Domain.Domain.ViewSchema;

namespace ERP.EventManagement
{
    public enum GridArrangment
    {
        DAY_VIEW = 0,
        MONTH_VIEW = 1,
        DATA_VIEW = 2
    }

    public partial class EventMgtForm : UserControl //XtraForm
    {
        private List<EventHeaderDTO> _eventHeaderDtoList = new List<EventHeaderDTO>();
        private List<EventDetaildataDTO> _eventDetailDtoList = new List<EventDetaildataDTO>();

        private GridArrangment _currentArragment = GridArrangment.DAY_VIEW;

        private DateTime _currentDate;
        //public static List<ArticleDTO> AllArticleList { get; set; }
        public static List<VwOrderStationMapDTO> AllOrderStationMap { get; set; }
        public static List<VwArticleViewDTO> AllArticleList { get; set; }
        public void GetArticleByGSLRequirement()
        {
            List<VwRequiredGslDTO> requiredGslDetail = UIProcessManager.Get_VwRequiredGslDTO_By_VoucherDefn_Type(CNETConstantes.EVENT_REQUIREMENT_VOUCHER, CNETConstantes.LK_Required_GSL_Article);
            if (requiredGslDetail != null && requiredGslDetail.Count > 0)
            {
                AllArticleList = new List<VwArticleViewDTO>();
                foreach (VwRequiredGslDTO req in requiredGslDetail)
                {
                    var ar = UIProcessManager.GetArticleViewByGslType(req.GslType);
                    if (ar != null && ar.Count > 0)
                        AllArticleList.AddRange(ar);
                }
            }
            else
                AllArticleList = new List<VwArticleViewDTO>();
        }
        public EventMgtForm()
        {
            InitializeComponent();

            InitializeUI();

            GetArticleByGSLRequirement();
            AllOrderStationMap = UIProcessManager.Get_VwOrderStationMap(LocalBuffer.LocalBuffer.CurrentDevice.Id);
           



            List<SystemConstantDTO> osdList = LocalBuffer.LocalBuffer.SystemConstantDTOBufferList.Where(s => s.Category == CNETConstantes.EVENT_STATE).ToList();
            sluStatusSearch.DataSource = osdList;
            sluStatusSearch.DisplayMember = "Description";
            sluStatusSearch.ValueMember = "Id";



            List<SystemConstantDTO> eventCategList = LocalBuffer.LocalBuffer.SystemConstantDTOBufferList.Where(l => l.Category == CNETConstantes.EVENT_CATEGORY).ToList();
            sleCategorySearch.DataSource = eventCategList;
            sleCategorySearch.DisplayMember = "Description";
            sleCategorySearch.ValueMember = "Id";



        }

        #region Helper Methods

        private void InitializeUI()
        {
            //Type
            repoLukEditDateCriteria.Columns.Add(new LookUpColumnInfo("Column", "Date Criteria"));
            repoLukEditDateCriteria.DisplayMember = "Column";
            repoLukEditDateCriteria.ValueMember = "Column";

            beiDateCriteria.EditValueChanged += BeiDateCriteria_EditValueChanged;
            beiStartDate.EditValueChanged += BeiStartDate_EditValueChanged;

        }

        private bool InitializeData()
        {
            try
            {
                string[] dateCriteriaList = { "Daily", "Weekly", "Monthly", "At the day of", "Annually", "Date Range" };
                repoLukEditDateCriteria.DataSource = dateCriteriaList;
                beiDateCriteria.EditValue = "Monthly";


                _currentArragment = GridArrangment.MONTH_VIEW;
                int count = _eventHeaderDtoList.Count;
                statusBarBtnArrangment.Caption = String.Format("Month View [ {0} ]", count);
                gvEventMgt.OptionsView.AllowCellMerge = true;
                gvEventMgt.OptionsSelection.EnableAppearanceFocusedCell = true;
                gvEventMgt.OptionsSelection.EnableAppearanceFocusedRow = false;
                gvEventMgt.OptionsSelection.EnableAppearanceHideSelection = true;
                gvEventMgt.OptionsView.EnableAppearanceOddRow = false;
                GetHotelData();
                ShowMonthView();
                return true;
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Error in initializing Event Management. DETAIL:: " + ex.Message, "Event Management", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }


        private void GetHotelData()
        {
            reiHotel.DisplayMember = "Description";
            reiHotel.ValueMember = "Id";
            reiHotel.DataSource = (LocalBuffer.LocalBuffer.AllHotelBranchBufferList.Select(x => new { x.Id, x.Description })).ToList();

            beiHotel.EditValue = LocalBuffer.LocalBuffer.CurrentDeviceConsigneeUnit;
            reiHotel.ReadOnly = !LocalBuffer.LocalBuffer.UserHasHotelBranchAccess;
        }


        //Populate Event Header DTO
        private void PopulateEventData()
        {
            try
            {
                _eventHeaderDtoList.Clear();
                _eventDetailDtoList.Clear();

                if (beiStartDate.EditValue == null || beiStartDate.EditValue == null)
                {
                    XtraMessageBox.Show("Please select Date Criteria First!", "Event Management", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;

                }

                DateTime startDate = Convert.ToDateTime(beiStartDate.EditValue.ToString());
                DateTime endDate = Convert.ToDateTime(beiEndDate.EditValue.ToString());

                ////Progress_Reporter.Show_Progress("Populating Event Header", "Please Wait...", 1, 2);

                //Populate Event Header
                List<EventHeaderView> eventHeaderViewList = UIProcessManager.GetEventHeaderView(startDate.Date, endDate.Date.AddHours(23), SelectedHotelcode);
                if (eventHeaderViewList == null) eventHeaderViewList = new List<EventHeaderView>();

                if (eventHeaderViewList != null && eventHeaderViewList.Count > 0)
                {
                    if (beiCategory.EditValue != null && !string.IsNullOrEmpty(beiCategory.EditValue.ToString()))
                    {
                        eventHeaderViewList = eventHeaderViewList.Where(x => x.eventCateg == beiCategory.EditValue.ToString()).ToList();
                    }

                    if (beiStatus.EditValue != null && !string.IsNullOrEmpty(beiStatus.EditValue.ToString()))
                    {
                        eventHeaderViewList = eventHeaderViewList.Where(x => x.LastState == Convert.ToInt32(beiStatus.EditValue.ToString())).ToList();
                    }
                }



                int count = 0;
                foreach (var eHeader in eventHeaderViewList)
                {
                    count++;
                    EventHeaderDTO dto = new EventHeaderDTO()
                    {
                        SN = count,
                        BookingTypeCode = eHeader.bookingType,
                        BookingTypeDesc = eHeader.bookingTypeDesc,
                        Id = eHeader.id,
                        Code = eHeader.Code,
                        Color = eHeader.color,
                        Consignee1 = eHeader.consignee,
                        Consignee2 = eHeader.consignee2,
                        Consignee3 = eHeader.consignee3,
                        Consignee4 = eHeader.consignee4,
                        Consignee1Name = eHeader.consigneeName,
                        Date = eHeader.IssuedDate,
                        Description = eHeader.eventDesc,
                        StartDate = eHeader.startDate.Value,
                        EndDate = eHeader.endDate.Value,
                        EventCategCode = string.IsNullOrEmpty(eHeader.eventCateg) ? null : Convert.ToInt32(eHeader.eventCateg),
                        EventCategDesc = eHeader.eventCategDesc,
                        OsdCode = eHeader.LastState,
                        OsdDescription = eHeader.osdDesc

                    };

                    _eventHeaderDtoList.Add(dto);
                }


                ////Progress_Reporter.Show_Progress("Populating Event Detail", "Please Wait...", 1, 2);

                //Populate Event Detail
                List<EventDisplayView> eventDetailViewList = UIProcessManager.GetEventDisplayView(startDate.Date, endDate.Date.AddHours(23), SelectedHotelcode);
                if (eventDetailViewList == null) eventDetailViewList = new List<EventDisplayView>();
                count = 0;
                foreach (var evDetail in eventDetailViewList)
                {
                    count++;
                    EventDetaildataDTO dto = new EventDetaildataDTO()
                    {
                        SN = count,
                        Arrangementid = evDetail.spaceArrangment,
                        ArrangementDesc = evDetail.SpaceArrangementDes,
                        Code = evDetail.code,
                        Description = evDetail.EventDescription,
                        StartTime = evDetail.startTimeStamp,
                        EndTime = evDetail.endTimeStamp,
                        Hall = evDetail.HallDescription,
                        NumOfPerson = evDetail.noOfPerson,
                        Remark = evDetail.remark,
                        SpaceCode = evDetail.space,
                        TypeCode = evDetail.VoucherType,
                        TypeDesc = evDetail.EventDetailTypeDescription,
                        Voucher = evDetail.code
                    };

                    _eventDetailDtoList.Add(dto);
                }

                ////Progress_Reporter.Close_Progress();

            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Error in populating event headers. DETAIL:: " + ex.Message, "Event Management", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ////Progress_Reporter.Close_Progress();
            }
        }


        //Day View
        private void ShowDayView()
        {
            try
            {
                if (beiDateCriteria.EditValue.ToString() != "At the day of")
                {
                    beiDateCriteria.EditValue = "At the day of";

                }

                beiDateCriteria.Enabled = false;



                gcEventMgt.DataSource = null;
                gvEventMgt.RefreshData();
                gvEventMgt.Columns.Clear();

                //Populate Column
                DataTable dataTable = new DataTable();
                dataTable.Columns.Add(new DataColumn() { ColumnName = "Time", Caption = "" });

                //Populate Hall
                List<RoomDetailDTO> hallList = new List<RoomDetailDTO>();
                List<RoomTypeDTO> meetingRoomTypes = UIProcessManager.GetRoomTypeByConsigneeUnit(SelectedHotelcode.Value).Where(rt => rt.IsActive && rt.CanBeMeetingRoom.Value).ToList();
                if (meetingRoomTypes != null)
                {
                    foreach (var rt in meetingRoomTypes)
                    {
                        List<RoomDetailDTO> halls = UIProcessManager.GetRoomDetailByroomType(rt.Id);
                        if (halls != null && halls.Count > 0)
                        {
                            hallList.AddRange(halls);
                        }
                    }
                }
                if (hallList != null)
                {
                    foreach (var hall in hallList)
                    {
                        dataTable.Columns.Add(new DataColumn() { ColumnName = hall.Space.ToString(), Caption = hall.Description, DataType = typeof(EventDetaildataDTO) });
                    }
                }

                //Get Half hours of a day
                DateTime now = Convert.ToDateTime(beiStartDate.EditValue.ToString());
                var start = new DateTime(now.Year, now.Month, now.Day);
                var clockQuery = from offset in Enumerable.Range(12, 36) // 12: since we want to start the time to start from 06:00 and each interval is 30Min, 36: since we need 36 hours instad of 48 hours
                                 select start.AddMinutes(30 * offset);


                foreach (var time in clockQuery)
                {
                    DataRow row = dataTable.NewRow();
                    row[dataTable.Columns["Time"]] = time.ToString("HH:mm");

                    var mEvents = _eventDetailDtoList.Where(ev => ev.StartTime.Date == time.Date && (TimeSpan.Compare(Convert.ToDateTime(ev.StartTime.ToString("HH:mm")).TimeOfDay, time.TimeOfDay) <= 0 &&
                    TimeSpan.Compare(Convert.ToDateTime(ev.EndTime.ToString("HH:mm")).TimeOfDay, time.TimeOfDay) >= 0)).ToList();
                    if (mEvents != null)
                    {
                        foreach (var mEvent in mEvents)
                        {
                            try
                            {
                                row[dataTable.Columns[mEvent.SpaceCode.ToString()]] = mEvent;
                            }
                            catch (Exception ex) { }
                        }
                    }

                    dataTable.Rows.Add(row);
                }

                gcEventMgt.DataSource = dataTable;
                gcEventMgt.RefreshDataSource();
                gvEventMgt.RefreshData();
                gvEventMgt.Columns["Time"].Width = 40;
                gvEventMgt.RowHeight = 70;

            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Error in building schedule grid. Detail:: " + ex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        //Month View
        private void ShowMonthView()
        {
            try
            {
                beiDateCriteria.Enabled = true;

                gcEventMgt.DataSource = null;
                gvEventMgt.RefreshData();

                gvEventMgt.Columns.Clear();

                //Populate Column
                DataTable dataTable = new DataTable();
                dataTable.Columns.Add(new DataColumn() { ColumnName = "Month", Caption = "" });
                dataTable.Columns.Add(new DataColumn() { ColumnName = "Day", Caption = "" });

                //Populate Hall
                List<RoomDetailDTO> hallList = new List<RoomDetailDTO>();
                List<RoomTypeDTO> meetingRoomTypes = UIProcessManager.GetRoomTypeByConsigneeUnit(SelectedHotelcode.Value).Where(rt => rt.IsActive && rt.CanBeMeetingRoom.Value).ToList();
                if (meetingRoomTypes != null)
                {
                    foreach (var rt in meetingRoomTypes)
                    {
                        List<RoomDetailDTO> halls = UIProcessManager.GetRoomDetailByroomType(rt.Id);
                        if (halls != null && halls.Count > 0)
                        {
                            hallList.AddRange(halls);
                        }
                    }
                }
                if (hallList != null)
                {
                    foreach (var hall in hallList)
                    {
                        dataTable.Columns.Add(new DataColumn() { ColumnName = hall.Space.ToString(), Caption = hall.Description, DataType = typeof(EventHeaderDTO) });
                    }
                }

                DateTime startDate = Convert.ToDateTime(beiStartDate.EditValue.ToString());
                DateTime endDate = Convert.ToDateTime(beiEndDate.EditValue.ToString());
                List<DateTime> dateCount = new List<DateTime>();
                while (startDate.Month <= endDate.Month)
                {
                    int totalDays = DateTime.DaysInMonth(startDate.Year, startDate.Month);
                    for (int i = 1; i <= totalDays; i++)
                    {
                        dateCount.Add(new DateTime(startDate.Year, startDate.Month, i));
                    }

                    //increment month
                    if (startDate.Month == endDate.Month)
                        break;
                    startDate = startDate.AddMonths(1);
                }

                for (int i = 0; i < dateCount.Count; i++)
                {
                    DateTime now = dateCount.ElementAt(i);

                    DataRow row = dataTable.NewRow();
                    row[dataTable.Columns["Month"]] = now.ToString("MMMM");
                    row[dataTable.Columns["Day"]] = now.ToString("MMM dd, ddd");

                    var mEvents = _eventHeaderDtoList.Where(ev => (DateTime.Compare(ev.StartDate.Date, now.Date) <= 0 &&
                    DateTime.Compare(ev.EndDate.Date, now.Date) >= 0)).ToList();
                    if (mEvents != null)
                    {
                        foreach (var mEvent in mEvents)
                        {
                            int[] eventHalls = _eventDetailDtoList.Where(ev => ev.Voucher == mEvent.Code).Select(ev => ev.SpaceCode).ToArray();
                            if (eventHalls != null)
                            {
                                foreach (var hall in eventHalls)
                                {
                                    try
                                    {
                                        row[dataTable.Columns[hall.ToString()]] = mEvent;
                                    }
                                    catch (Exception ex) { }
                                }
                            }

                        }
                    }

                    dataTable.Rows.Add(row);
                }


                gcEventMgt.DataSource = dataTable;
                gcEventMgt.RefreshDataSource();
                gvEventMgt.RefreshData();

                gvEventMgt.Columns["Month"].Width = 80;
                gvEventMgt.Columns["Day"].Width = 80;

                gvEventMgt.RowHeight = 50;


            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Error in building month view event list. Detail:: " + ex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //Data View 
        private void ShowDataView()
        {
            try
            {
                beiDateCriteria.Enabled = true;

                gcEventMgt.DataSource = null;
                gvEventMgt.RefreshData();
                gvEventMgt.Columns.Clear();

                //Populate Column
                DataTable dTable = new DataTable();
                dTable.Columns.Add(new DataColumn() { ColumnName = "SN", Caption = "SN" });
                dTable.Columns.Add(new DataColumn() { ColumnName = "Code", Caption = "Code" });
                dTable.Columns.Add(new DataColumn() { ColumnName = "ConsigneeName", Caption = "Owner" });
                dTable.Columns.Add(new DataColumn() { ColumnName = "Description", Caption = "Description" });
                dTable.Columns.Add(new DataColumn() { ColumnName = "StartDate", Caption = "Start Date" });
                dTable.Columns.Add(new DataColumn() { ColumnName = "EndDate", Caption = "End Date" });
                dTable.Columns.Add(new DataColumn() { ColumnName = "BookingTypeDesc", Caption = "Booking Type" });
                dTable.Columns.Add(new DataColumn() { ColumnName = "EventCategDesc", Caption = "Event Category" });
                dTable.Columns.Add(new DataColumn() { ColumnName = "OsdDescription", Caption = "Status" });
                dTable.Columns.Add(new DataColumn() { ColumnName = "Color", Caption = "Color" });

                foreach (var eHeader in _eventHeaderDtoList)
                {
                    DataRow row = dTable.NewRow();
                    row[dTable.Columns["SN"]] = eHeader.SN;
                    row[dTable.Columns["Code"]] = eHeader.Code;
                    row[dTable.Columns["ConsigneeName"]] = eHeader.Consignee1Name;
                    row[dTable.Columns["Description"]] = eHeader.Description;
                    row[dTable.Columns["StartDate"]] = eHeader.StartDate.ToString("MMM dd, yyyy");
                    row[dTable.Columns["EndDate"]] = eHeader.EndDate.ToString("MMM dd, yyyy");
                    row[dTable.Columns["BookingTypeDesc"]] = eHeader.BookingTypeDesc;
                    row[dTable.Columns["EventCategDesc"]] = eHeader.EventCategDesc;
                    row[dTable.Columns["OsdDescription"]] = eHeader.OsdDescription;
                    row[dTable.Columns["Color"]] = eHeader.Color;

                    dTable.Rows.Add(row);
                }

                gcEventMgt.DataSource = dTable;
                gcEventMgt.RefreshDataSource();
                gvEventMgt.RefreshData();

                gvEventMgt.Columns["Color"].Visible = false;
                gvEventMgt.Columns[0].Width = 25;
                gvEventMgt.RowHeight = 30;


            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Error in building month view event list. Detail:: " + ex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion


        #region Event Hanlders 

        private void BeiDateCriteria_EditValueChanged(object sender, EventArgs e)
        {
            BarEditItem view = sender as BarEditItem;
            if (view == null) return;

            beiStartDate.EditValue = null;
            beiEndDate.EditValue = null;
            beiStartDate.Enabled = true;
            beiEndDate.Enabled = true;
            beiStartDate.Visibility = BarItemVisibility.Always;
            beiEndDate.Visibility = BarItemVisibility.Always;

            DateTime? currentTime = UIProcessManager.GetServiceTime();
            if (currentTime == null) return;
            _currentDate = currentTime.Value;

            string criteria = view.EditValue == null ? "" : view.EditValue.ToString();
            if (criteria.ToLower() == "daily")
            {
                beiEndDate.Visibility = BarItemVisibility.Never;
                beiStartDate.Caption = "Current Date";
                beiStartDate.Enabled = false;
                beiEndDate.Enabled = false;
                beiStartDate.EditValue = currentTime.Value;
                beiEndDate.EditValue = currentTime.Value;
            }
            else if (criteria.ToLower() == "weekly")
            {
                DateTime now = currentTime.Value;
                DayOfWeek day = now.DayOfWeek;
                int days = day - DayOfWeek.Monday;

                beiStartDate.EditValue = now.AddDays(-days);
                beiEndDate.EditValue = now.AddDays(6);
                beiStartDate.Enabled = false;
                beiEndDate.Enabled = false;
            }
            else if (criteria.ToLower() == "monthly")
            {
                DateTime now = currentTime.Value;
                int days = DateTime.DaysInMonth(now.Year, now.Month);
                beiStartDate.EditValue = new DateTime(now.Year, now.Month, 1);
                beiEndDate.EditValue = new DateTime(now.Year, now.Month, days);
                beiStartDate.Enabled = false;
                beiEndDate.Enabled = false;
            }
            else if (criteria.ToLower() == "annually")
            {
                DateTime now = currentTime.Value;
                beiStartDate.EditValue = new DateTime(now.Year, 1, 1);
                beiEndDate.EditValue = new DateTime(now.Year, 12, 31);
                beiStartDate.Enabled = false;
                beiEndDate.Enabled = false;
            }
            else if (criteria.ToLower() == "at the day of")
            {
                beiEndDate.Visibility = BarItemVisibility.Never;
                beiStartDate.Caption = "At the day of";
                beiStartDate.EditValue = currentTime.Value;
                beiEndDate.EditValue = currentTime.Value;

            }
            else if (criteria.ToLower() == "date range")
            {
                beiStartDate.EditValue = currentTime.Value;
                beiEndDate.EditValue = currentTime.Value.AddDays(1);
            }
            else
            {
                beiStartDate.EditValue = currentTime.Value;
                beiEndDate.EditValue = currentTime.Value.AddDays(1);
            }


        }

        private void BeiStartDate_EditValueChanged(object sender, EventArgs e)
        {

            if (beiStartDate.EditValue != null && (beiDateCriteria.EditValue != null && beiDateCriteria.EditValue.ToString() == "At the day of"))
            {
                DateTime date = Convert.ToDateTime(beiStartDate.EditValue);
                beiEndDate.EditValue = date;
            }
            else if (beiStartDate.EditValue == null && beiDateCriteria.EditValue != null && beiDateCriteria.EditValue.ToString() == "At the day of")
            {
                beiEndDate.EditValue = null;
            }
        }

        private void bbiAddEvent_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            EventEditor frmEventEditor = new EventEditor();
            frmEventEditor.SelectedHotelcode = SelectedHotelcode;
            frmEventEditor.EventDTO = null;
            frmEventEditor.ShowDialog();

        }

        private void EventMgtForm_Load(object sender, EventArgs e)
        {
            if (!InitializeData())
            {
                ribbonEventMgt.Enabled = false;
            }
        }


        private void bbiEventDetail_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }

        private void bbiRefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            PopulateEventData();

            //PopulateEventData();
            switch (_currentArragment)
            {
                case GridArrangment.DATA_VIEW:
                    ShowDataView();
                    break;
                case GridArrangment.DAY_VIEW:
                    ShowDayView();
                    break;
                case GridArrangment.MONTH_VIEW:
                    ShowMonthView();
                    break;
            }
        }

        private void gvEventMgt_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            try
            {
                if (_currentArragment == GridArrangment.DAY_VIEW)
                {
                    if (e.Column.FieldName != "Time")
                    {

                        var value = e.CellValue as EventDetaildataDTO;
                        if (value != null)
                        {
                            var evHeader = _eventHeaderDtoList.FirstOrDefault(ev => ev.Code == value.Voucher);
                            if (evHeader == null) return;

                            Color color = (!string.IsNullOrEmpty(evHeader.Color)) ? ColorTranslator.FromHtml(evHeader.Color) : ColorTranslator.FromHtml("black");
                            e.Graphics.DrawRectangle(new Pen(color, 1.5f), e.Bounds.X, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height);
                            e.Graphics.FillRectangle(new SolidBrush(color), new RectangleF(e.Bounds.X, e.Bounds.Y, e.Bounds.Width * 0.05f, e.Bounds.Height));
                            e.Graphics.DrawString(String.Format("{0} \r\n {1} \r\n from {2} to {3} \r\n {4}", evHeader.Consignee1Name, evHeader.Description, value.StartTime.ToString("HH:mm"), value.EndTime.ToString("HH:mm"), value.Description), e.Appearance.Font, Brushes.Black, new PointF(e.Bounds.X + 20, e.Bounds.Y + 5));
                            e.Graphics.Flush();
                            //e.DisplayText = value.EventDescription;
                            //e.CellValue = value.EventDescription;
                            e.Handled = true;

                        }
                    }
                    else
                    {
                        DateTime now = Convert.ToDateTime(DateTime.Now.ToString("HH:mm"));
                        if (now.TimeOfDay.Minutes < 20)
                        {
                            now = now.AddMinutes(-now.Minute);
                        }
                        else if (now.TimeOfDay.Minutes >= 20 && now.TimeOfDay.Minutes <= 40)
                        {
                            now = new DateTime(now.Year, now.Month, now.Day, now.Hour, 30, now.Second);
                        }
                        else if (now.TimeOfDay.Minutes > 40)
                        {
                            now = now.AddMinutes(now.Minute + (60 - now.Minute));
                            now = now.AddMinutes(-now.Minute);
                        }
                        DateTime value = Convert.ToDateTime(e.CellValue);
                        int compare = TimeSpan.Compare(now.TimeOfDay, value.TimeOfDay);
                        if (compare == 0)
                        {
                            e.Appearance.BackColor = ColorTranslator.FromHtml("black");
                            e.Appearance.ForeColor = ColorTranslator.FromHtml("white");
                        }
                        //TimeSpan.Compare(TimeSpan)
                    }
                }
                else if (_currentArragment == GridArrangment.MONTH_VIEW)
                {
                    if (e.Column.FieldName == "Month")
                    {
                        e.Appearance.TextOptions.HAlignment = HorzAlignment.Center;
                        var font = e.Appearance.Font;
                        e.Appearance.Font = new System.Drawing.Font(font.FontFamily, font.Size, FontStyle.Bold);

                    }
                    if (e.Column.FieldName != "Day" && e.Column.FieldName != "Month")
                    {
                        var evHeader = e.CellValue as EventHeaderDTO;
                        if (evHeader != null)
                        {

                            Color color = (!string.IsNullOrEmpty(evHeader.Color)) ? ColorTranslator.FromHtml(evHeader.Color) : ColorTranslator.FromHtml("black");
                            e.Graphics.DrawRectangle(new Pen(color, 1.5f), e.Bounds.X, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height);
                            e.Graphics.FillRectangle(new SolidBrush(color), new RectangleF(e.Bounds.X, e.Bounds.Y, e.Bounds.Width * 0.05f, e.Bounds.Height));
                            e.Graphics.DrawString(String.Format("{0} \r\n {1} \r\n from {2} to {3}", evHeader.Consignee1Name, evHeader.Description, evHeader.StartDate.ToString("ddd, dd"), evHeader.EndDate.ToString("ddd,dd")), e.Appearance.Font, Brushes.Black, new PointF(e.Bounds.X + 20, e.Bounds.Y + 5));

                            e.Graphics.Flush();


                            //e.DisplayText = value.EventDescription;
                            //e.CellValue = value.EventDescription;

                            e.Handled = true;

                        }
                    }
                    else
                    {
                        DateTime value = DateTime.Parse(e.CellValue.ToString());
                        int compare = DateTime.Compare(_currentDate.Date, value.Date);
                        if (compare == 0)
                        {
                            e.Appearance.BackColor = ColorTranslator.FromHtml("black");
                            e.Appearance.ForeColor = ColorTranslator.FromHtml("white");
                        }
                    }
                }
                else
                {
                    /******* DATA GRID VIEW ******/
                    if (e.Column.FieldName == "OsdDescription")
                    {

                        string color = gvEventMgt.GetRowCellValue(e.RowHandle, gvEventMgt.Columns["Color"]) as string;
                        if (!string.IsNullOrEmpty(color))
                        {
                            e.Appearance.BackColor = ColorTranslator.FromHtml(color);
                            e.Appearance.ForeColor = ColorTranslator.FromHtml("white");
                        }

                    }
                }

            }
            catch (Exception ex)
            {

            }
        }

        private void gvEventMgt_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            var val = e.Value;
        }

        private void gvEventMgt_RowStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs e)
        {

        }


        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                //ReportGenerator rg = new ReportGenerator();
                //rg.GenerateGridReport(gcEventMgt, "Event Management", DateTime.Now.ToLongDateString());
            }
            catch (Exception ex)
            {

            }
        }

        private void gvEventMgt_DoubleClick(object sender, EventArgs e)
        {
            DXMouseEventArgs ea = e as DXMouseEventArgs;
            GridView view = sender as GridView;
            GridHitInfo info = view.CalcHitInfo(ea.Location);
            if (info.InRow || info.InRowCell)
            {
                if (_currentArragment == GridArrangment.DAY_VIEW)
                {
                    EventDetaildataDTO dto = view.GetRowCellValue(info.RowHandle, info.Column) as EventDetaildataDTO;
                    if (dto != null)
                    {
                        EventHeaderDTO headerDTO = _eventHeaderDtoList.FirstOrDefault(h => h.Code == dto.Voucher);
                        if (headerDTO == null)
                        {
                            XtraMessageBox.Show("Unable to find Event Header", "Event Management", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        EventEditor frmEventEditor = new EventEditor();
                        frmEventEditor.SelectedHotelcode = SelectedHotelcode;
                        frmEventEditor.EventDTO = headerDTO;
                        frmEventEditor.ShowDialog();


                    }
                    else
                    {
                        XtraMessageBox.Show("Unable to find Event Detail", "Event Management", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                else if (_currentArragment == GridArrangment.MONTH_VIEW)
                {
                    EventHeaderDTO headerDTO = view.GetRowCellValue(info.RowHandle, info.Column) as EventHeaderDTO;
                    if (headerDTO != null)
                    {
                        EventEditor frmEventEditor = new EventEditor();
                        frmEventEditor.SelectedHotelcode = SelectedHotelcode;
                        frmEventEditor.EventDTO = headerDTO;
                        frmEventEditor.ShowDialog();
                    }
                    else
                    {
                        //XtraMessageBox.Show("Unable to find Event Header", "Event Management", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                else
                {

                    var row = view.GetRow(info.RowHandle) as DataRowView;
                    if (row != null)
                    {
                        var items = row.Row.ItemArray;
                        if (items != null && items.Length > 0)
                        {
                            var headerDTO = _eventHeaderDtoList.FirstOrDefault(h => h.Code == items[1]);
                            EventEditor frmEventEditor = new EventEditor();
                            frmEventEditor.SelectedHotelcode = SelectedHotelcode;
                            frmEventEditor.EventDTO = headerDTO;
                            frmEventEditor.ShowDialog();
                        }

                    }
                    else
                    {
                        //XtraMessageBox.Show("Unable to find Event Header", "Event Management", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                bbiRefresh.PerformClick();
            }
        }

        private void bbiPrint_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }

        private void bbiDayView_ItemClick(object sender, ItemClickEventArgs e)
        {
            _currentArragment = GridArrangment.DAY_VIEW;
            int count = _eventDetailDtoList.Where(ev => ev.StartTime.Date == _currentDate.Date).ToList().Count();
            statusBarBtnArrangment.Caption = String.Format("Day View [ {0} ]", count);
            gvEventMgt.OptionsView.AllowCellMerge = true;
            gvEventMgt.OptionsSelection.EnableAppearanceFocusedCell = true;
            gvEventMgt.OptionsSelection.EnableAppearanceFocusedRow = false;
            gvEventMgt.OptionsSelection.EnableAppearanceHideSelection = true;
            gvEventMgt.OptionsView.EnableAppearanceOddRow = false;
            ShowDayView();
        }

        private void bbiMonthView_ItemClick(object sender, ItemClickEventArgs e)
        {
            _currentArragment = GridArrangment.MONTH_VIEW;
            int count = _eventHeaderDtoList.Count;
            statusBarBtnArrangment.Caption = String.Format("Month View [ {0} ]", count);
            gvEventMgt.OptionsView.AllowCellMerge = true;
            gvEventMgt.OptionsSelection.EnableAppearanceFocusedCell = true;
            gvEventMgt.OptionsSelection.EnableAppearanceFocusedRow = false;
            gvEventMgt.OptionsSelection.EnableAppearanceHideSelection = true;
            gvEventMgt.OptionsView.EnableAppearanceOddRow = false;
            ShowMonthView();

        }

        private void bbiDataView_ItemClick(object sender, ItemClickEventArgs e)
        {
            _currentArragment = GridArrangment.DATA_VIEW;
            int count = _eventHeaderDtoList.Count;
            statusBarBtnArrangment.Caption = String.Format("Data View [ {0} ]", count);
            gvEventMgt.OptionsView.AllowCellMerge = false;
            gvEventMgt.OptionsSelection.EnableAppearanceFocusedCell = false;
            gvEventMgt.OptionsSelection.EnableAppearanceFocusedRow = true;
            gvEventMgt.OptionsSelection.EnableAppearanceHideSelection = false;
            gvEventMgt.OptionsView.EnableAppearanceOddRow = true;
            gvEventMgt.Appearance.OddRow.BackColor = ColorTranslator.FromHtml("GradientInactiveCaption");

            ShowDataView();

        }

        #endregion

        private void bbiClearFilter_ItemClick(object sender, ItemClickEventArgs e)
        {
            beiStatus.EditValue = null;
            beiCategory.EditValue = null;
        }

        private void bbiEventFolio_ItemClick(object sender, ItemClickEventArgs e)
        {
            int row = gvEventMgt.FocusedRowHandle;

            if (_currentArragment == GridArrangment.DAY_VIEW)
            {
                EventHeaderDTO dto = gvEventMgt.GetRowCellValue(row, gvEventMgt.FocusedColumn) as EventHeaderDTO;
                if (dto != null)
                {
                    EventFolio Folio = new EventFolio();
                    Folio.EventObjectState = dto.OsdCode.Value;
                    Folio.EventId = dto.Id;
                    Folio.EventCode = dto.Code;
                    Folio.ShowDialog();
                    bbiRefresh.PerformClick();


                }
                else
                {
                    XtraMessageBox.Show("Unable to find Event Detail", "Event Management", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            else if (_currentArragment == GridArrangment.MONTH_VIEW)
            {
                EventHeaderDTO headerDTO = gvEventMgt.GetRowCellValue(row, gvEventMgt.FocusedColumn) as EventHeaderDTO;
                if (headerDTO != null)
                {
                    EventFolio Folio = new EventFolio();
                    Folio.EventObjectState = headerDTO.OsdCode.Value;
                    Folio.EventId = headerDTO.Id;
                    Folio.EventCode = headerDTO.Code;
                    Folio.ShowDialog();
                    bbiRefresh.PerformClick();
                }
                else
                {
                    return;
                }
            }
            else
            {
                var rows = gvEventMgt.GetRow(row) as DataRowView;
                if (rows != null)
                {
                    var items = rows.Row.ItemArray;
                    if (items != null && items.Length > 0)
                    {
                        var headerdataDTO = _eventHeaderDtoList.FirstOrDefault(h => h.Code == items[1]);

                        if (headerdataDTO != null)
                        {
                            EventFolio Folio = new EventFolio();
                            Folio.EventObjectState = headerdataDTO.OsdCode.Value;
                            Folio.EventId = headerdataDTO.Id;
                            Folio.EventCode = headerdataDTO.Code;
                            Folio.ShowDialog();
                            bbiRefresh.PerformClick();
                        }
                        else
                        {
                            XtraMessageBox.Show("Unable to find Event Detail", "Event Management", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                }
            }

        }

        private void beiHotel_EditValueChanged(object sender, EventArgs e)
        {

            SelectedHotelcode = beiHotel.EditValue == null ? null : Convert.ToInt32(beiHotel.EditValue.ToString());

            bbiRefresh.PerformClick();
        }

        public int? SelectedHotelcode { get; set; }

    }
}
