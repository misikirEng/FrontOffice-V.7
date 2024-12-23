
using CNET.FrontOffice_V._7;
using ProcessManager;
using CNET_V7_Domain.Misc.PmsDTO;
using DevExpress.XtraCharts;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraReports.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using static CNET.FrontOffice_V._7.MasterPageForm;
using PMSReport;

namespace PMS_Dashboard
{
    public partial class PMSDashboard : UserControl
    {
        public DateTime? CurrentTime { get; set; }
        public SelectedDashboardTask SelectedDashboardTaskHandler { get; set; }

        private void PMSDashboard_Load(object sender, EventArgs e)
        {
        } 
       
        
        public MemoryStream ExportToPdfMemoryStream( )
        {
            bool flag = true;
            XtraReport1 report = null;
            MemoryStream stream = new MemoryStream();
            try
            {
                var report2 = new XtraReport2(gc_pmsDashboard, chartControl3, chart_roomOccupancy, chart_reservationCount, gcRoomStats, "PMS DashBoard Report", DateTime.Now.ToShortDateString());
 
                report2.ExportToPdf(stream);
                return stream;

            }
            catch (Exception ex)
            {

                return null;
            }

            return stream;
        }


        public int? selectedHotelcode { get; set; }
        public PMSDashboard(SelectedDashboardTask selectedDashboardTaskHandler)
        {
            InitializeComponent();


            SelectedDashboardTaskHandler = selectedDashboardTaskHandler;
            CurrentTime = UIProcessManager.GetServiceTime();
            if (CurrentTime == null)
            {
                XtraMessageBox.Show("Server DateTime Error !!", "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            GetHotelData();

            //while (PopulateRegistration == null || !PopulateRegistration.IsAlive)
            //{
            //    PopulateRegistration = new Thread(() =>
            //    {
            //        PMSDashBoardReport GetPMSDashBoardReport = UIProcessManager.GetPMSDashBoardReport(CurrentTime, selectedHotelcode.Value);

            //        PopulateRegistrationInfoData(GetPMSDashBoardReport.DashboardInfo);
            //        PopulateRoomInfoData(GetPMSDashBoardReport.VacantInfo, GetPMSDashBoardReport.OccupiedInfo, GetPMSDashBoardReport.HouseKeepingInfo);
            //        PopulateResStaus(GetPMSDashBoardReport.MonthlyReservationCountList);
            //        PopulateInitialGrid(GetPMSDashBoardReport.RommStatusList);
            //    })
            //    { IsBackground = true };
            //    PopulateRegistration.Start();
            //}
        }

        public void GetHotelData()
        {
            leHotel.Properties.DisplayMember = "Name";
            leHotel.Properties.ValueMember = "Id";
            leHotel.Properties.DataSource = (LocalBuffer.LocalBuffer.HotelBranchBufferList.Select(x => new { x.Id, x.Name })).ToList();

            leHotel.EditValue = LocalBuffer.LocalBuffer.CurrentDeviceConsigneeUnit;

            leHotel.Properties.ReadOnly = !LocalBuffer.LocalBuffer.UserHasHotelBranchAccess;
        }

        public void PopulateRegistrationInfoData(List<DashboardDTO> DashboardInfo)
        {
            try
            {
                gc_pmsDashboard.Invoke((MethodInvoker)delegate ()
                {
                    gc_pmsDashboard.DataSource = DashboardInfo;
                    gc_pmsDashboard.RefreshDataSource();
                });

            }
            catch (Exception io)
            {
            }
        }
        public void PopulateRoomInfoData(RoomInfoData VacantInfo, RoomInfoData OccupiedInfo, List<RoomInfoData> HouseKeepingInfo)
        {
            try
            {
                var spVacant = chart_roomOccupancy.Series[0].Points[0];
                if (VacantInfo != null)
                {
                    spVacant.Values = VacantInfo.value;
                    spVacant.Argument = VacantInfo.Argument;


                    var spOccupied = chart_roomOccupancy.Series[0].Points[1];
                    spOccupied.Values = OccupiedInfo.value;
                    spOccupied.Argument = OccupiedInfo.Argument;


                    chartControl3.Invoke((MethodInvoker)delegate ()
                    {
                        if (HouseKeepingInfo.Count >= 6)

                        {
                            chartControl3.Series[0].Points[0].Values = HouseKeepingInfo[0].value;
                            chartControl3.Series[0].Points[0].Argument = HouseKeepingInfo[0].Argument;

                            chartControl3.Series[0].Points[1].Values = HouseKeepingInfo[1].value;
                            chartControl3.Series[0].Points[1].Argument = HouseKeepingInfo[1].Argument;

                            chartControl3.Series[0].Points[2].Values = HouseKeepingInfo[2].value;
                            chartControl3.Series[0].Points[2].Argument = HouseKeepingInfo[2].Argument;

                            chartControl3.Series[0].Points[3].Values = HouseKeepingInfo[3].value;
                            chartControl3.Series[0].Points[3].Argument = HouseKeepingInfo[3].Argument;

                            chartControl3.Series[0].Points[4].Values = HouseKeepingInfo[4].value;
                            chartControl3.Series[0].Points[4].Argument = HouseKeepingInfo[4].Argument;

                            chartControl3.Series[0].Points[5].Values = HouseKeepingInfo[5].value;
                            chartControl3.Series[0].Points[5].Argument = HouseKeepingInfo[5].Argument;

                        }

                    });


                }

            }
            catch (Exception io)
            {
            }
        }
        public void PopulateResStaus(List<Tuple<string, int, int>> resCountList)
        {
            try
            {
                if (resCountList == null)
                {
                    return;
                }
                var sPointList = new SeriesPoint[12];

                chart_reservationCount.Invoke((MethodInvoker)delegate ()
                {
                    chart_reservationCount.Series[0].Points.Clear();
                });


                var values = new List<string>();
                var totalCount = 0;
                var Count = 0;
                foreach (var resCount in resCountList)
                {
                    if (!values.Contains(resCount.Item1))
                    {
                        values.Add(resCount.Item1);
                        totalCount = resCount.Item2;
                        var sPoint = new DevExpress.XtraCharts.SeriesPoint(resCount.Item1, new double[] { resCount.Item3 });
                        sPointList[Count] = sPoint;
                        Count++;
                    }
                }

                if (resCountList == null || resCountList.Count == 0)
                    return;
                chart_reservationCount.Invoke((MethodInvoker)delegate ()
                {
                    chart_reservationCount.Series[0].Points.AddRange(sPointList);
                    chart_reservationCount.RefreshData();
                    chart_reservationCount.Refresh();
                    chart_reservationCount.Titles[0].Text = "Reservations (" + CurrentTime.Value.Year + ") Total=" + totalCount;
                });
            }
            catch (Exception io)
            {
            }
        }
        private void PopulateInitialGrid(List<RoomStats> RommStatusList)
        {
            gcRoomStats.Invoke((MethodInvoker)delegate ()
            {
                gcRoomStats.DataSource = RommStatusList;
                gcRoomStats.RefreshDataSource();
            });
        }

        public void bWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
            }
        }
        private void bWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
        }


        private void gv_pmsDashboard_DoubleClick(object sender, EventArgs e)
        {
            var view = sender as GridView;
            var dto = view.GetFocusedRow() as DashboardDTO;
            if (dto == null)
            {
                return;
            }
            SelectedDashboardTaskHandler.Invoke(dto.Task);
        }
        private Thread PopulateRegistration { get; set; }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                while (PopulateRegistration == null || !PopulateRegistration.IsAlive)
                {
                    PopulateRegistration = new Thread(() =>
                    {
                        PMSDashBoardReport GetPMSDashBoardReport = UIProcessManager.GetPMSDashBoardReport(CurrentTime.Value, selectedHotelcode.Value);
                        if (GetPMSDashBoardReport != null)
                        {
                            if (GetPMSDashBoardReport.DashboardInfo != null)
                                PopulateRegistrationInfoData(GetPMSDashBoardReport.DashboardInfo);
                            if (GetPMSDashBoardReport.DashboardInfo != null)
                                PopulateRoomInfoData(GetPMSDashBoardReport.VacantInfo, GetPMSDashBoardReport.OccupiedInfo, GetPMSDashBoardReport.HouseKeepingInfo);
                            if (GetPMSDashBoardReport.MonthlyReservationCountList != null)
                                PopulateResStaus(GetPMSDashBoardReport.MonthlyReservationCountList);
                            if (GetPMSDashBoardReport.RommStatusList != null)
                                PopulateInitialGrid(GetPMSDashBoardReport.RommStatusList);

                        }

                    })
                    { IsBackground = true };
                    PopulateRegistration.Start();
                }
            }
            catch (Exception io)
            {
            }
        }


        public void DashboardRefresh()
        {
            btnRefresh.PerformClick();
        }

        public void CreateDashboardReport(string path)
        {
            var report2 = new XtraReport2(gc_pmsDashboard, chartControl3, chart_roomOccupancy, chart_reservationCount, gcRoomStats, "PMS DashBoard Report", DateTime.Now.ToShortDateString());
            report2.ExportToPdf(path);
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            var report2 = new XtraReport2(gc_pmsDashboard, chartControl3, chart_roomOccupancy, chart_reservationCount, gcRoomStats, "PMS DashBoard Report", DateTime.Now.ToShortDateString());
            report2.ShowPreview();
        }

        private void btnExporttoXLS_Click(object sender, EventArgs e)
        {
            var sfd = new SaveFileDialog();


            sfd.Title = "Dashboard Report";
            sfd.FileName = "Dashboard Report " + CurrentTime.Value.ToShortDateString().Replace('/', '-');
            sfd.Filter = "*.xls|*.xls|*.xlsx|*.xlsx";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                gc_pmsDashboard.ExportToXlsx(sfd.FileName);
                if (File.Exists(sfd.FileName))
                {
                    if (MessageBox.Show("Do you want to open the Excel File", "Dashboard Report", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                    {
                        try
                        {
                            System.Diagnostics.Process.Start(sfd.FileName);
                        }
                        catch
                        {
                        }
                    }
                }
            }


            sfd.Title = "Room statistics Report";
            sfd.FileName = "Room statistics Report " + CurrentTime.Value.ToShortDateString().Replace('/', '-');
            sfd.Filter = "*.xls|*.xls|*.xlsx|*.xlsx";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                gcRoomStats.ExportToXlsx(sfd.FileName);
                if (File.Exists(sfd.FileName))
                {
                    if (MessageBox.Show("Do you want to open the Excel File", "Dashboard Report", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                    {
                        try
                        {
                            System.Diagnostics.Process.Start(sfd.FileName);
                        }
                        catch
                        {
                        }
                    }
                }
            }
        }




        private void leHotel_EditValueChanged(object sender, EventArgs e)
        {
            selectedHotelcode = leHotel.EditValue == null ? null : Convert.ToInt32(leHotel.EditValue);
            btnRefresh.PerformClick();
        }

    }


}
