using CNET.ERP.Client.Common.UI;
using DevExpress.XtraEditors;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Printing;
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
using DevExpress.XtraPrinting;
using System.Runtime.InteropServices;
using ProcessManager;
using CNET_V7_Domain.Domain.PmsSchema;
using CNET.FrontOffice_V._7;
using CNET_V7_Domain.Misc.PmsView;

namespace PMSReport
{
    public partial class RoomStatus : XtraForm
    {


        #region Declaration

        public DateTime? selectedDate { get; set; }

        BindingList<RommStateDTO> schedule = new BindingList<RommStateDTO>();
        BindingList<RoomDetailDTO> scheduleResources = new BindingList<RoomDetailDTO>();


        //Microsoft.Office.Interop.Excel.Application xlApp = null;
        //Microsoft.Office.Interop.Excel.Workbook xlWorkBook = null;
        //Microsoft.Office.Interop.Excel.Worksheet xlWorkSheet = null;

        int? SelectedHotelcode { get; set; }

        Color Clean = Color.FromName("");


        #endregion

        #region Constractor
        public RoomStatus(DateTime selecteddate, int? Hotelcode)
        {
            InitializeComponent();
            SelectedHotelcode = Hotelcode;

            schedulerControl1.ActiveViewType = SchedulerViewType.Gantt;

            SetColorSchema();
            InitResourcesCustom();
            GetAndPopulateData();
            schedule.Clear();
            PopulateResourceData();

            selectedDate = UIProcessManager.GetServiceTime();
            if (selectedDate == null)
            {
                XtraMessageBox.Show("Server DateTime Error !!", "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            beiDateTime.EditValue = selecteddate;

        }
        #endregion

        #region Public Method
        public void SetColorSchema()
        {
            schedulerStorage1.Appointments.Labels[0].Color = Color.LightGreen;
            schedulerStorage1.Appointments.Labels[1].Color = Color.Red;
            schedulerStorage1.Appointments.Labels[2].Color = Color.LightBlue;
            schedulerStorage1.Appointments.Labels[3].Color = Color.Pink;
            schedulerStorage1.Appointments.Labels[4].Color = Color.Yellow;
            schedulerStorage1.Appointments.Labels[5].Color = Color.WhiteSmoke;
            schedulerStorage1.Appointments.Labels[6].Color = Color.Violet;
            schedulerStorage1.Appointments.Labels[7].Color = Color.IndianRed;
            schedulerStorage1.Appointments.Labels[8].Color = Color.Brown;
            schedulerStorage1.Appointments.Labels[9].Color = Color.CadetBlue;
        }


        List<RoomDetailDTO> AllRoomList { get; set; }
        List<RoomTypeDTO> FilterRoomType { get; set; }

        List<RoomDetailDTO> FilterRoomList { get; set; }
        public void PopulateResourceData()
        {
            scheduleResources.Clear();
            FilterRoomList = new List<RoomDetailDTO>();
            //if (beiSpaceType.EditValue != null && !string.IsNullOrEmpty(beiSpaceType.EditValue.ToString()))
            //{
            // List<RoomDetail> RoomList = LoginPage.Authentication.RoomDetailBufferList;

            if (SelectedHotelcode != null)
                FilterRoomType = UIProcessManager.GetRoomTypeByConsigneeUnit(SelectedHotelcode.Value);
            else
                FilterRoomType = UIProcessManager.SelectAllRoomType();

            if (FilterRoomType != null)
            {
                List<int> FilterRoomTypecodelist = FilterRoomType.Select(x => x.Id).ToList();
                AllRoomList = UIProcessManager.SelectAllRoomDetail();
                if (FilterRoomTypecodelist != null && AllRoomList != null)
                {
                    FilterRoomList = AllRoomList.Where(x => FilterRoomTypecodelist.Contains(x.RoomType)).ToList(); ;
                }
            }
            sleRoomType.DataSource = FilterRoomType;
            sleRoomType.ValueMember = "Id";
            sleRoomType.DisplayMember = "Description";

            scheduleResources = new BindingList<RoomDetailDTO>(FilterRoomList);
            schedulerStorage1.Resources.DataSource = scheduleResources;
            //}

        }
        public void PopulateScheduleData(bool showmessage = true)
        {
            schedule.Clear();
            if (true)
            {
                try
                {

                    CreateData();
                }
                catch (Exception io)
                {
                    XtraMessageBox.Show("There is an Error getting Reservation", "CNETERP_V6", MessageBoxButtons.OK, MessageBoxIcon.Error);

                }
            }
            else
            {
                //if (showmessage)
                //{
                //    Lookup space = LoginPage.Authentication.LookupBufferList.FirstOrDefault(x => x.code == beiSpaceType.EditValue.ToString());
                //    XtraMessageBox.Show("There is No Reservation in " + space.description + " for " + SelectedDate.Date.ToShortDateString(), "CNETERP_V6", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //}
            }
            schedulerStorage1.RefreshData();
        }
        public void GetAndPopulateData()
        {
            //sleRoomType.DataSource = Authentication.LookupBufferList.Where(x => x.type == "Space Type");
            //sleRoomType.DisplayMember = "description";
            //sleRoomType.ValueMember = "code";


        }

        public void InitResourcesCustom()
        {
            ResourceMappingInfo mappings = this.schedulerStorage1.Resources.Mappings;
            mappings.Id = "Id";
            mappings.Caption = "Description";

            AppointmentMappingInfo apMappings = this.schedulerStorage1.Appointments.Mappings;
            apMappings.ResourceId = "room";
            apMappings.Start = "startTimeStamp";
            apMappings.End = "endTimeStamp";
            apMappings.Subject = "description";
            apMappings.Description = "description";
            apMappings.Label = "color";

        }
        public bool checkValues()
        {
            if (beiDateTime.EditValue == null || string.IsNullOrEmpty(beiDateTime.EditValue.ToString()))
            {
                XtraMessageBox.Show("Please Select Date Time First !!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            //if (beiSpaceType.EditValue == null || string.IsNullOrEmpty(beiSpaceType.EditValue.ToString()))
            //{
            //    XtraMessageBox.Show("Please Select Space Type First !!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    return false;
            //}
            return true;
        }
        public void CreateXls(List<ScheduleData> ScheduleDataList, List<DateTime> ColnumList, List<string> Resurcelist, bool comparedate = true)
        {
            /*
            #region Create Excel and Sheet
            xlApp = new Microsoft.Office.Interop.Excel.Application();
            xlApp.WorkbookBeforeClose += xlApp_WorkbookBeforeClose;

            xlApp.Application.DisplayAlerts = false;
            xlApp.DisplayAlerts = false;


            if (xlApp == null)
            {
                MessageBox.Show("MicroSoft Excel is not properly installed!!");
                return;
            }


            object misValue = System.Reflection.Missing.Value;

            xlWorkBook = xlApp.Workbooks.Add(misValue);
            xlWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

            //Populate Company Name and cell merge
            Microsoft.Office.Interop.Excel.Range FirstCompanyRange = (Microsoft.Office.Interop.Excel.Range)xlWorkSheet.Cells[1, 16];
            Microsoft.Office.Interop.Excel.Range LastCompanyRange = (Microsoft.Office.Interop.Excel.Range)xlWorkSheet.Cells[1, 30];
            Microsoft.Office.Interop.Excel.Range CompanyMergerrange = xlWorkSheet.Range[FirstCompanyRange.Cells.Address, LastCompanyRange.Cells.Address];
            CompanyMergerrange.Merge();
            CompanyMergerrange.Font.Size = 15;
            CompanyMergerrange.Font.Bold = true;
            CompanyMergerrange.Font.Underline = true;
            xlWorkSheet.Cells[1, 16] = LocalBuffer.LocalBuffer.CompanyName;

            //Populate cell columon time
            List<string> Collist = new List<string> { "SN", "Resource" };
            Collist.AddRange(ColnumList.Select(X => X.ToString("HH:mm")));
            int colnum = 1;
            foreach (string col in Collist)
            {
                xlWorkSheet.Cells[3, colnum] = col;

                colnum++;
            }
            Collist.Remove("Resource");
            Collist.Remove("SN");


            //code and resources cell color and fit
            Microsoft.Office.Interop.Excel.Range firstcol = (Microsoft.Office.Interop.Excel.Range)xlWorkSheet.Cells[3, 1];
            Microsoft.Office.Interop.Excel.Range lastcol = (Microsoft.Office.Interop.Excel.Range)xlWorkSheet.Cells[3, 2];
            Microsoft.Office.Interop.Excel.Range colmunrange = xlWorkSheet.Range[firstcol.Cells.Address, lastcol.Cells.Address];
            colmunrange.Cells.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
            colmunrange.Cells.Font.Bold = true;
            colmunrange.EntireRow.AutoFit();
            colmunrange.Rows.AutoFit();
            colmunrange.Columns.AutoFit();


            //Time cell color and orintaion for text
            firstcol = (Microsoft.Office.Interop.Excel.Range)xlWorkSheet.Cells[3, 3];
            lastcol = (Microsoft.Office.Interop.Excel.Range)xlWorkSheet.Cells[3, Collist.Count + 2];
            colmunrange = xlWorkSheet.Range[firstcol.Cells.Address, lastcol.Cells.Address];
            colmunrange.Cells.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
            colmunrange.Cells.Font.Bold = true;
            colmunrange.Cells.Orientation = 90;
            colmunrange.EntireRow.AutoFit();
            colmunrange.Rows.AutoFit();
            colmunrange.Columns.AutoFit();
            #endregion

            #region populate cell resource and appointment

            int SNCount = 1;
            int resrownum = 4;
            foreach (string Res in Resurcelist)
            {
                List<ScheduleData> ResScheduleData = ScheduleDataList.Where(r => r.Resource == Res).ToList();

                #region Populate Room Status Data
                if (ResScheduleData != null && ResScheduleData.Count > 0)
                {
                    xlWorkSheet.Cells[resrownum, 1] = SNCount;
                    SNCount++;
                    xlWorkSheet.Cells[resrownum, 2] = Res;
                    foreach (ScheduleData she in ResScheduleData)
                    {
                        List<DateTime> Collvalue = new List<DateTime>();
                        Collvalue = ColnumList.Where(x => Convert.ToDateTime(x) >= she.StartTimeStamp && Convert.ToDateTime(x) < she.EndTimeStamp).ToList();
                        foreach (DateTime col in Collvalue)
                        {
                            int value = ColnumList.IndexOf(col);
                            if (value > 0)
                            {
                                value = value + 3;
                                xlWorkSheet.Cells[resrownum, value] = she.Description;
                                if (Collvalue.Count == 1)
                                {
                                    Microsoft.Office.Interop.Excel.Range CellRange = (Microsoft.Office.Interop.Excel.Range)xlWorkSheet.Cells[resrownum, value];
                                    CellRange.Interior.Color = System.Drawing.ColorTranslator.ToOle(she.Color);
                                    CellRange.BorderAround(Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous, Microsoft.Office.Interop.Excel.XlBorderWeight.xlThin, Microsoft.Office.Interop.Excel.XlColorIndex.xlColorIndexAutomatic, Microsoft.Office.Interop.Excel.XlColorIndex.xlColorIndexAutomatic);
                                }
                            }
                        }
                        if (Collvalue.Count > 1)
                        {
                            Microsoft.Office.Interop.Excel.Range FirstRange = (Microsoft.Office.Interop.Excel.Range)xlWorkSheet.Cells[resrownum, (ColnumList.IndexOf(Collvalue.FirstOrDefault()) + 3)];
                            Microsoft.Office.Interop.Excel.Range LastRange = (Microsoft.Office.Interop.Excel.Range)xlWorkSheet.Cells[resrownum, (ColnumList.IndexOf(Collvalue.LastOrDefault()) + 3)];
                            Microsoft.Office.Interop.Excel.Range Mergerrange = xlWorkSheet.Range[FirstRange.Cells.Address, LastRange.Cells.Address];
                            Mergerrange.Merge();
                            Mergerrange.Interior.Color = System.Drawing.ColorTranslator.ToOle(she.Color);
                            Mergerrange.BorderAround(Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous, Microsoft.Office.Interop.Excel.XlBorderWeight.xlThin, Microsoft.Office.Interop.Excel.XlColorIndex.xlColorIndexAutomatic, Microsoft.Office.Interop.Excel.XlColorIndex.xlColorIndexAutomatic);
                        }
                    }
                    resrownum++;
                }
                #endregion


                if ((ResScheduleData == null || ResScheduleData.Count == 0))
                {
                    xlWorkSheet.Cells[resrownum, 1] = SNCount;
                    SNCount++;
                    xlWorkSheet.Cells[resrownum, 2] = Res;
                    resrownum++;
                }

            }
            xlApp.Visible = true;

            #endregion
            */

        }

        #endregion

        #region Private Method
        private void btnShow_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (checkValues())
            {
                PopulateScheduleData();
            }
        }
        private void schedulerControl1_DoubleClick(object sender, EventArgs e)
        {

        }
        private void beiDateTime_EditValueChanged(object sender, EventArgs e)
        {
            if (beiDateTime.EditValue != null && !string.IsNullOrEmpty(beiDateTime.EditValue.ToString()))
            {

                DateTime scheduleDateTime = Convert.ToDateTime(beiDateTime.EditValue.ToString());
                DateTime scheduleStartDateTime = scheduleDateTime.Date;
                DateTime scheduleEndDateTime = scheduleDateTime.Date;

                schedulerControl1.Start = scheduleStartDateTime.Date;
                schedulerControl1.LimitInterval.Start = scheduleStartDateTime.Date;
                schedulerControl1.LimitInterval.End = scheduleEndDateTime.Date.AddDays(1).Date;
                schedulerControl1.DayView.WorkTime.Start = scheduleStartDateTime.TimeOfDay;
                schedulerControl1.DayView.WorkTime.End = scheduleEndDateTime.Date.AddDays(1).TimeOfDay;
                schedulerControl1.DayView.VisibleTime.Start = scheduleStartDateTime.TimeOfDay;
                schedulerControl1.DayView.VisibleTime.End = scheduleEndDateTime.AddDays(1).TimeOfDay;
                schedule.Clear();

            }

        }
        private void schedulerControl1_EditAppointmentFormShowing(object sender, AppointmentFormEventArgs e)
        {
            e.Handled = true;
        }
        private void beiSpaceType_EditValueChanged(object sender, EventArgs e)
        {

            if (beiRoomType.EditValue != null && !string.IsNullOrEmpty(beiRoomType.EditValue.ToString()))
            {

                FilterRoomList = AllRoomList.Where(x => x.RoomType == Convert.ToInt32(beiRoomType.EditValue)).ToList(); ;
                scheduleResources = new BindingList<RoomDetailDTO>(FilterRoomList);
                schedulerStorage1.Resources.DataSource = scheduleResources;

            }
            else
            {
                if (FilterRoomType != null)
                {
                    List<int> FilterRoomTypecodelist = FilterRoomType.Select(x => x.Id).ToList();
                    if (FilterRoomTypecodelist != null && AllRoomList != null)
                    {
                        FilterRoomList = AllRoomList.Where(x => FilterRoomTypecodelist.Contains(x.RoomType)).ToList(); ;
                    }
                }
                scheduleResources = new BindingList<RoomDetailDTO>(FilterRoomList);
                schedulerStorage1.Resources.DataSource = scheduleResources;
            }
        }
        private void tcReservationControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if (tcReservationControl.SelectedTab == tbpDocument && ReservationDocument == null)
            //{
            //    Progress_Reporter.Show_Progress("Opening Documet Browser");
            //    ReservationDocument = new VoucherDocument(CNETConstantes.ITEM_RESREVATION_VOUCHER.ToString());
            //    ReservationDocument.Dock = DockStyle.Fill;
            //    this.pnlCtrlDocumentBrowser.Controls.Clear();
            //    this.pnlCtrlDocumentBrowser.Controls.Add(ReservationDocument);
            //   //CNETInfoReporter.Hide();
            //}
        }

       /* private void xlApp_WorkbookBeforeClose(Microsoft.Office.Interop.Excel.Workbook Wb, ref bool Cancel)
        {
            xlWorkBook.Close();
            xlApp.Quit();
            xlWorkBook = null;
            Marshal.ReleaseComObject(xlWorkSheet);
            Marshal.ReleaseComObject(xlApp);
            GC.Collect();
        }*/

        private void btnPrint_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (checkValues())
                {

                    DateTime SelectedDate = Convert.ToDateTime(beiDateTime.EditValue.ToString());
                    BindingList<RommStateDTO> scheduledata = (BindingList<RommStateDTO>)schedulerStorage1.Appointments.DataSource;
                    BindingList<RoomDetailDTO> RoomList = (BindingList<RoomDetailDTO>)schedulerStorage1.Resources.DataSource;

                    if (RoomList == null || RoomList.Count == 0)
                    {
                        XtraMessageBox.Show("There is No Room !!", "CNETERP_V6", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    if (scheduledata == null || scheduledata.Count == 0)
                    {
                        XtraMessageBox.Show("There is No RoomStatus Data ! ", "CNETERP_V6", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    List<string> Resorcelist = RoomList.Select(x => x.Description).ToList();

                    List<ScheduleData> XLSScheduleData = scheduledata.Select(x => new ScheduleData()
                    {
                        Description = x.description,
                        Resource = x.roomNumber,
                        IsClean = x.description == "Clean" ? true : false,
                        StartTimeStamp = x.startTimeStamp,
                        EndTimeStamp = x.endTimeStamp
                    }).ToList();

                    XLSScheduleData.Where(x => x.IsClean).ToList().ForEach(c => c.Color = Color.LightGreen);
                    XLSScheduleData.Where(x => !x.IsClean).ToList().ForEach(c => c.Color = Color.Red);

                    List<string> ColnumList = new List<string>();

                    DateTime start = Convert.ToDateTime(beiDateTime.EditValue).Date;
                    List<DateTime> clockQuery = (from offset in Enumerable.Range(0, 96)
                                                 select start.AddMinutes(15 * offset)).ToList();

                    CreateXls(XLSScheduleData, clockQuery, Resorcelist, false);
                }

            }
            catch (Exception io)
            {
            }
        }

        #endregion


        public void CreateData()
        {

            selectedDate = Convert.ToDateTime(beiDateTime.EditValue.ToString());
            List<VwRoomActivityViewDTO> TodayHKActivities = UIProcessManager.GetRoomActivityView(selectedDate.Value, SelectedHotelcode).ToList();
            List<VwRoomActivityViewDTO> YesterdayHKActivities = UIProcessManager.GetRoomActivityView(selectedDate.Value.AddDays(-1), SelectedHotelcode).ToList();

            List<RommStateDTO> DTOList = new List<RommStateDTO>();
            RommStateDTO DTO = new RommStateDTO { };

            List<int> RoomNumbers = TodayHKActivities.Select(x => x.reference).Distinct().ToList();

            foreach (int code in RoomNumbers)
            {
                List<VwRoomActivityViewDTO> FilterTestdatalist = TodayHKActivities.Where(x => x.reference == code).OrderByDescending(x => x.TimeStamp).ToList();


                for (int i = 0; i < FilterTestdatalist.Count; i++)
                {
                    DTO = new RommStateDTO
                    {
                        description = FilterTestdatalist[i].ActivitiyDefinition_Desc,
                        room = FilterTestdatalist[i].reference,
                        roomNumber = FilterTestdatalist[i].RoomNumber,
                        endTimeStamp = FilterTestdatalist[i].TimeStamp.Value,
                        startTimeStamp = FilterTestdatalist[i].TimeStamp.Value,
                        //color = ""

                    };

                    if (i == 0)
                        DTO.endTimeStamp = selectedDate.Value.Date.AddHours(23).AddMinutes(59);
                    else
                        DTO.endTimeStamp = FilterTestdatalist[i - 1].TimeStamp.Value;

                    if ((FilterTestdatalist.Count - 1) == i)
                    {
                        VwRoomActivityViewDTO Yesterdaylastdata = YesterdayHKActivities.OrderByDescending(s => s.TimeStamp).ToList().FirstOrDefault(x => x.reference == code);
                        if (Yesterdaylastdata != null)
                        {
                            if (DTO.description != Yesterdaylastdata.ActivitiyDefinition_Desc)
                            {
                                RommStateDTO yestDTO = new RommStateDTO
                                {
                                    description = Yesterdaylastdata.ActivitiyDefinition_Desc,
                                    room = Yesterdaylastdata.reference,
                                    roomNumber = Yesterdaylastdata.RoomNumber,
                                    startTimeStamp = Yesterdaylastdata.TimeStamp.Value,
                                    endTimeStamp = FilterTestdatalist[i].TimeStamp.Value,
                                   // color = ""

                                };

                                if (yestDTO.description == "Dirty")
                                    yestDTO.color = "1";
                                else if (yestDTO.description == "Clean")
                                    yestDTO.color = "0";

                                DTOList.Add(yestDTO);
                            }
                            else
                            {
                                DTO.startTimeStamp = Yesterdaylastdata.TimeStamp.Value;
                            }
                        }
                    }


                    if (DTO.description == "Dirty")
                        DTO.color = "1";
                    else if (DTO.description == "Clean")
                        DTO.color = "0";

                    DTOList.Add(DTO);
                }

            }



            schedule = new BindingList<RommStateDTO>(DTOList);
            schedulerStorage1.Appointments.DataSource = schedule;
            schedulerStorage1.RefreshData();



        }

        private void beiRoomType_EditValueChanged(object sender, EventArgs e)
        {
            if (beiRoomType.EditValue != null && !string.IsNullOrEmpty(beiRoomType.EditValue.ToString()))
            {

                FilterRoomList = AllRoomList.Where(x => x.RoomType == Convert.ToInt32(beiRoomType.EditValue)).ToList(); ;
                scheduleResources = new BindingList<RoomDetailDTO>(FilterRoomList);
                schedulerStorage1.Resources.DataSource = scheduleResources;

            }
            else
            {
                if (FilterRoomType != null)
                {
                    List<int> FilterRoomTypecodelist = FilterRoomType.Select(x => x.Id).ToList();
                    if (FilterRoomTypecodelist != null && AllRoomList != null)
                    {
                        FilterRoomList = AllRoomList.Where(x => FilterRoomTypecodelist.Contains(x.RoomType)).ToList(); ;
                    }
                }
                scheduleResources = new BindingList<RoomDetailDTO>(FilterRoomList);
                schedulerStorage1.Resources.DataSource = scheduleResources;
            }
        }

        private void btnClose_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Close();
        }

    }

    public class RommStateDTO
    {
        public string description { get; set; }
        public int room { get; set; }
        public string roomNumber { get; set; }
        public DateTime endTimeStamp { get; set; }
        public DateTime startTimeStamp { get; set; }
        public string color { get; set; }
    }

    public class ScheduleData
    {
        public string Description { get; set; }
        public DateTime StartTimeStamp { get; set; }
        public DateTime EndTimeStamp { get; set; }
        public string Resource { get; set; }
        public Color Color { get; set; }
        public bool IsClean { get; set; }

    }


}
