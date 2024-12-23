using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;

using CNET.ERP.Client.Common.UI;
using CNET.FrontOffice_V._7;
using DevExpress.XtraBars;
using DevExpress.XtraGrid;
using System.Globalization;
using DevExpress.XtraReports.UI;
using CNET_V7_Domain.Domain.ConsigneeSchema;
using CNET_V7_Domain.Domain.PmsSchema;
using ProcessManager;

//using CNET.FrontOffice_V._7.Enum;
namespace CNET.FrontOffice_V._7.HouseKeeping
{
    public partial class frmTaskAssignment : UILogicBase
    {

        private List<TaskSheetVM> currentMonth = new List<TaskSheetVM>();
        private static frmTaskSheetDetails taskSheetDetails = null;
        private static frmTaskSheetGrid taskSheetGrid = null;
        DateTime CurrentTime { get; set; }

        public static frmTaskSheetDetails Instance
        {
            get
            {
                if (taskSheetDetails == null)
                    taskSheetDetails = new frmTaskSheetDetails();
                return taskSheetDetails;
            }

        }
        public frmTaskAssignment()
        {
            InitializeComponent();
            //SecurityCheck("Task Assignment");
            try
            {
                DateTime? date = UIProcessManager.GetServiceTime();
                if (date != null)
                {
                    CurrentTime = date.Value;
                }
                else
                {
                    XtraMessageBox.Show("Error Getting Server Date Time !!", "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message, "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            CustomizeNavigator();
        }

        private void SecurityCheck(string parent)
        {
            List<String> allowedTabs = MasterPageForm.AllowedFunctionalities(parent);
            int closeedCounter = 0;
            if (!allowedTabs.Contains("Assign Task"))
            {
                bbiPrint.Enabled = false;
                closeedCounter++;
            }
            if (!allowedTabs.Contains("Task Sheet"))
            {
                barButtonItem1.Enabled = false;
                closeedCounter++;
            }
            if (!allowedTabs.Contains("Print"))
            {
                bbiPrint.Enabled = false;
                closeedCounter++;
            }
            if (closeedCounter == 3)
            {
                this.Enabled = false;
            }
        }


        private void bbiCancel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Close();
        }

        private void bbiNew_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            taskSheetDetails = new frmTaskSheetDetails();
            taskSheetDetails.SelectedHotelcode = SelectedHotelcode.Value;
            taskSheetDetails.FormClosed += frmTaskSheetDetails_FormClosing;
            taskSheetDetails.ShowDialog();
        }

        private void frmTaskSheetDetails_FormClosing(object sender, FormClosedEventArgs e)
        {
            currentMonth.Clear();
            grdTaskAssign.DataSource = null;
            PopulateGrid(DateTime.Now.Date);
        }
        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {


            try
            {
                int index = gridView1.GetFocusedDataSourceRowIndex();
                Object row = gridView1.GetRow(index);
                var task = row as TaskSheetVM;
                if (task != null)
                {
                    string d = task.taskdate;
                    frmTaskSheetGrid taskSheetGrid = new frmTaskSheetGrid(d);
                    taskSheetGrid.SelectedHotelcode = SelectedHotelcode;
                    taskSheetGrid.ShowDialog();
                }
            }
            catch (Exception)
            {


            }
        }


        public void PopulateGrid(DateTime whichDate)
        {
            List<TaskSheetVM> tasksheet = new List<TaskSheetVM>();
            int mon = CurrentTime.Month;
            List<HkassignmentDTO> list = new List<HkassignmentDTO>();
            list = UIProcessManager.SelectAllHKAssignment();
            if (list != null)
                list = list.Where(x => x.Consigneeunit == SelectedHotelcode).ToList();

            if (list != null && list.Count > 0)
            {
                List<DateTime> d = new List<DateTime>();
                d = list.Select(b => Convert.ToDateTime(b.Date))
                    .Distinct()
                    .ToList();

                List<HkassignmentDTO> h = list.Where(b => b.Date == whichDate.Date).ToList();
                if (h == null || h.Count == 0)
                {
                    return;
                }
                TaskSheetVM tsd = new TaskSheetVM();
                tsd.taskdate = whichDate.ToShortDateString();
                // tsd.task = h[0].task;
                string task = "";
                tsd.auto = "Yes";
                int totalSheets = h.Select(b => b.Employee).Distinct().Count();
                tsd.ttlsheet = totalSheets.ToString();
                double credit = 0;
                List<int> roomcode = new List<int>();
                foreach (HkassignmentDTO hh in h)
                {
                    if (!roomcode.Contains(hh.RoomDetail.Value)) roomcode.Add(hh.RoomDetail.Value);
                    else if (roomcode.Contains(hh.RoomDetail.Value)) continue;
                    double c = Convert.ToDouble(hh.Credit);
                    credit += c;
                }
                decimal dd = Math.Round(Convert.ToDecimal(credit), 2);
                tsd.ttlcrdt = dd.ToString();
                List<TaskDetail> tasks = new List<TaskDetail>();
                var groupedValues = h.GroupBy(b => b.Employee);
                foreach (var group in groupedValues)
                {
                    TaskDetail td = new TaskDetail();
                    td.AttendantCode = group.FirstOrDefault().Employee;
                    ConsigneeDTO p = UIProcessManager.GetConsigneeById(td.AttendantCode.Value);
                    if (p != null)
                    {
                        string fname = (p.FirstName != null) ? p.FirstName : "";
                        string faname = (p.SecondName != null) ? p.SecondName : "";
                        string lname = (p.ThirdName != null) ? p.ThirdName : "";
                        td.AttendantName = fname + " " + faname + " " + lname;
                    }
                    td.TotalRooms = group.Count().ToString();
                    decimal totalCredit = 0;
                    foreach (var g in group)
                    {
                        totalCredit += Convert.ToDecimal(g.Credit);
                    }
                    td.TotalCredit = totalCredit.ToString();
                    tasks.Add(td);
                }

                tsd.taskDetail = tasks;
                tasksheet.Add(tsd);
            }
            grdTaskAssign.DataSource = tasksheet;
        }

        private List<ConsigneeUnitDTO> iOrgUnit;
        private void frmTaskAssignment_Load(object sender, EventArgs e)
        {
            reiHotel.DisplayMember = "Name";
            reiHotel.ValueMember = "Id";
            reiHotel.DataSource = (LocalBuffer.LocalBuffer.HotelBranchBufferList.Select(x => new { x.Id, x.Name })).ToList();

            beiHotel.EditValue = LocalBuffer.LocalBuffer.CurrentDeviceConsigneeUnit;
            reiHotel.ReadOnly = !LocalBuffer.LocalBuffer.UserHasHotelBranchAccess;

            PopulateGrid(DateTime.Now.Date);
            Months();
            ShowSummary();

        }

        private void SearchByDate(object sender, EventArgs e)
        {
            if (((BarEditItem)(sender)).EditValue == null)
            {
                grdTaskAssign.DataSource = null;
                PopulateGrid(DateTime.Now.Date);
                return;
            }
            DateTime reqDate = (DateTime)((BarEditItem)(sender)).EditValue;
            grdTaskAssign.DataSource = null;
            PopulateGrid(reqDate);

        }

        private void ViewTaskOnRowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            if (e.Clicks == 2)
            {

                Object row = gridView1.GetFocusedRow();
                var task = row as TaskSheetVM;
                string date = task.taskdate;
                frmTaskSheetGrid tsg = new frmTaskSheetGrid(date);

                tsg.ShowDialog();
            }
        }
        private void Months()
        {
            // List<LookupDTO> lookupMonths = UIProcessManager.GetLookup("Month");
            List<string> months = new List<string>() {  "January",
                                                        "February",
                                                        "March",
                                                        "April",
                                                        "May",
                                                        "June",
                                                        "July",
                                                        "August",
                                                        "September",
                                                        "October",
                                                        "November",
                                                        "December"};

            SearchByMonth.DisplayMember = "description";
            SearchByMonth.ValueMember = "description";
            SearchByMonth.DataSource = months;
        }

        private void bbiPrint_ItemClick(object sender, ItemClickEventArgs e)
        {
            //ShowPrintPreview(gridControl1);
            HouseKeepingReport report = new HouseKeepingReport();
            report = new HouseKeepingReport(grdTaskAssign, false, "", "", "HouseKeeping Task Assignment Report");

            ReportPrintTool pt = new ReportPrintTool(report);
            pt.ShowPreview();
        }
        private void ShowPrintPreview(GridControl grid)
        {

            if (!grid.IsPrintingAvailable)
            {
                MessageBox.Show("Printing library not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            grid.ShowPrintPreview();
            //grid.Print();
        }
        private void ShowSummary()
        {
            gridView1.OptionsView.ShowFooter = true;
            var item1 = new GridColumnSummaryItem
                (DevExpress.Data.SummaryItemType.Sum, "ttlsheet", "Total Sheet:{0}");
            gridView1.Columns["ttlsheet"].Summary.Add(item1);

            var item2 = new GridColumnSummaryItem
            (DevExpress.Data.SummaryItemType.Sum, "ttlcrdt", "Total Credit: {0}");
            gridView1.Columns["ttlcrdt"].Summary.Add(item2);
        }
        List<TaskSheetVM> selected = new List<TaskSheetVM>();
        private void MonthValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (monthEdit.EditValue == null)
                {
                    grdTaskAssign.DataSource = null;
                    PopulateGrid(DateTime.Now.Date);
                    return;
                }
                string month = "";
                if (monthEdit.EditValue != null)
                {
                    month = monthEdit.EditValue.ToString();
                }

                int monthDigit = DateTime.ParseExact(month, "MMMM", CultureInfo.InvariantCulture).Month;
                int yearDigit = 0;
                if (dateEdit.EditValue != null)
                {
                    DateTime dt = (DateTime)dateEdit.EditValue;
                    yearDigit = dt.Year;
                }
                else
                {
                    yearDigit = DateTime.Now.Year;
                }

                PopulateByMonth(yearDigit, monthDigit);

            }
            catch (Exception ex)
            {

            }
        }
        private void CustomizeNavigator()
        {
            ControlNavigator na = grdTaskAssign.EmbeddedNavigator;
            na.Buttons.Remove.Visible = false;
            na.Buttons.EndEdit.Visible = false;
            na.Buttons.Edit.Visible = false;
            na.Buttons.CancelEdit.Visible = false;
            na.Buttons.Append.Visible = false;
        }

        private void PopulateByMonth(int year, int month)
        {
            List<TaskSheetVM> monthTask = new List<TaskSheetVM>();
            List<HkassignmentDTO> list = new List<HkassignmentDTO>();
            list = UIProcessManager.SelectAllHKAssignment().Where
                (b => b.Date.Value.Year == year && b.Date.Value.Month == month && b.Consigneeunit == SelectedHotelcode).ToList();

            if (list != null && list.Count > 0)
            {

                List<DateTime> d = new List<DateTime>();
                d = list.Select(b => Convert.ToDateTime(b.Date))
                    .Distinct()
                    .ToList();

                foreach (DateTime time in d)
                {
                    List<HkassignmentDTO> h = list.Where(b => b.Date == time.Date).ToList();
                    if (h == null || h.Count == 0)
                    {
                        return;
                    }
                    TaskSheetVM tsd = new TaskSheetVM();
                    tsd.taskdate = time.ToShortDateString();
                    // tsd.task = h[0].task;
                    string task = "";
                    tsd.auto = "Yes";
                    int totalSheets = h.Select(b => b.Employee).Distinct().Count();
                    tsd.ttlsheet = totalSheets.ToString();
                    double credit = 0;
                    List<int> roomcode = new List<int>();
                    foreach (HkassignmentDTO hh in h)
                    {
                        if (!roomcode.Contains(hh.RoomDetail.Value)) roomcode.Add(hh.RoomDetail.Value);
                        else if (roomcode.Contains(hh.RoomDetail.Value)) continue;
                        double c = Convert.ToDouble(hh.Credit);
                        credit += c;
                    }
                    decimal dd = Math.Round(Convert.ToDecimal(credit), 2);
                    tsd.ttlcrdt = dd.ToString();
                    List<TaskDetail> tasks = new List<TaskDetail>();
                    var groupedValues = h.GroupBy(b => b.Employee);
                    foreach (var group in groupedValues)
                    {
                        TaskDetail td = new TaskDetail();
                        td.AttendantCode = group.FirstOrDefault().Employee;
                        ConsigneeDTO p = UIProcessManager.GetConsigneeById(td.AttendantCode.Value);
                        if (p != null)
                        {
                            string fname = (p.FirstName != null) ? p.FirstName : "";
                            string faname = (p.SecondName != null) ? p.SecondName : "";
                            string lname = (p.ThirdName != null) ? p.ThirdName : "";
                            td.AttendantName = fname + " " + faname + " " + lname;
                        }
                        td.TotalRooms = group.Count().ToString();
                        decimal totalCredit = 0;
                        foreach (var g in group)
                        {
                            totalCredit += Convert.ToDecimal(g.Credit);
                        }
                        td.TotalCredit = totalCredit.ToString();
                        tasks.Add(td);
                    }
                    tsd.taskDetail = tasks;
                    monthTask.Add(tsd);

                }
            }
            grdTaskAssign.DataSource = null;
            grdTaskAssign.DataSource = monthTask;
        }

        private void beiHotel_EditValueChanged(object sender, EventArgs e)
        {
            SelectedHotelcode = beiHotel.EditValue == null ? null : Convert.ToInt32(beiHotel.EditValue);
            dateEdit.EditValue = null;
            monthEdit.EditValue = null;
            PopulateGrid(DateTime.Now.Date);
        }
        public int? SelectedHotelcode { get; set; }
    }
}