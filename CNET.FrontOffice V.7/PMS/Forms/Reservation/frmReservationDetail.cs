using System.Drawing;
using System.Collections.Generic;
using System;
using System.Linq;
using CNET.ERP.Client.Common.UI;
using CNET.FrontOffice_V._7.PMS.Contracts;
using CNET.FrontOffice_V._7.Forms.Non_Navigatable_Modals;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraEditors;
using System.Windows.Forms;
using CNET.FrontOffice_V._7.PMS.Contracts;
using CNET_V7_Domain.Domain.PmsSchema;
using ProcessManager;
using CNET.FrontOffice_V._7.PMS.Common_Classes;
using DocumentPrint;

namespace CNET.FrontOffice_V._7.Forms
{
    public partial class frmReservationDetail : UILogicBase, ILogicHelper
    {
        DateTime CurrentTime;

        public int? LastRegState { get; set; }

        private List<RegistrationDetailVM> _regDetailDTO = null;
        public List<PackagesToPostDTO> _pkToPostList = null;

        private List<RegistrationDetailDTO> _regDetailList = null;

        /// <summary>
        /// //////////////////////// CONSTRUCTOR ///////////////////////////////
        /// </summary>
        public frmReservationDetail()
        {
            InitializeComponent();
            Utility.AdjustForm(this);
            Size = new Size(1100, 600);
            Location = new Point(450, 150);
            InitializeData();
            InitializeUI();
        }

        public void InitializeUI()
        {
            //gv_resDetail.BestFitColumns();   
            beiExpandCheckBx.CheckedChanged += beiExpandCheckBx_CheckedChanged;
        }



        public void InitializeData()
        {
            DateTime? _currentTime = UIProcessManager.GetServiceTime();
            if (_currentTime == null) return;
            CurrentTime = _currentTime.Value;

        }

        private void EditHandler()
        {
            RegistrationDetailVM reg = gv_resDetail.GetFocusedRow() as RegistrationDetailVM;
            if (reg == null) return;
            List<PackagesToPostVM> ptpDTO = reg.PackagesToPostDTOs;

            frmRegistrationDetail frmRegDetail = new frmRegistrationDetail();
            frmRegDetail.SelectedHotelcode = SelectedHotelcode;
            RegistrationDetailDTO regD = UIProcessManager.GetRegistrationDetailById(reg.Id);
            if (regD.Date != null && regD.Date.Value.Date >= CurrentTime.Date)
            {
                frmRegDetail.RegistarationDetail = regD;
                frmRegDetail.LastRegState = LastRegState.Value;

                if (frmRegDetail.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                {
                    _regDetailList = UIProcessManager.GetRegistrationDetailByvoucher(regVoucher);
                    PopulateRegDetail();

                }
            }
            else
            {
                SystemMessage.ShowModalInfoMessage("You can not edit the passed registration!", "ERROR");
            }

        }


        #region Populate Data

        private void PopulateRegDetail()
        {
            gc_resDetail.DataSource = null;

            if (_regDetailList != null)
            {
                MapRegDetailDTO(_regDetailList);
            }

            gc_resDetail.BeginUpdate();

            gc_resDetail.DataSource = _regDetailDTO.OrderBy(r => r.Date).ToList();
            gc_resDetail.RefreshDataSource();

            gc_resDetail.EndUpdate();

            //int dataRowCount = gv_resDetail.DataRowCount;
            //for (int rHandle = 0; rHandle < dataRowCount; rHandle++)
            //    gv_resDetail.SetMasterRowExpanded(rHandle, true);

        }

        private List<PackagesToPostVM> PopulatePkgToPostDTO(int regCode)
        {
            List<PackagesToPostVM> ptpList = new List<PackagesToPostVM>();
            //populate pkgToPostList
            if (_pkToPostList == null)
                _pkToPostList = UIProcessManager.SelectAllPackagesToPost();
            if (_pkToPostList == null) return null;

            ptpList = _pkToPostList.Where(r => r.RegistrationDetail == regCode).Select(r => new PackagesToPostVM()
            {
                PackageHeader = r.PackageHeader,
                PackageHeaderDescription = UIProcessManager.GetPackageHeaderById(r.PackageHeader).Description,
                Amount = r.Amount
            }).ToList();
            return ptpList;
        }

        private void MapRegDetailDTO(List<RegistrationDetailDTO> regDetailList)
        {
            if (_regDetailDTO == null)
            {
                _regDetailDTO = new List<RegistrationDetailVM>();
            }
            _regDetailDTO.Clear();

            foreach (var rDetail in regDetailList)
            {
                RegistrationDetailVM rdDTO = new RegistrationDetailVM();
                if (rDetail.RoomType != null)
                {
                    rdDTO.RoomType = rDetail.RoomType;
                    rdDTO.roomTypeDesc = UIProcessManager.GetRoomTypeById(rDetail.RoomType.Value).Description;
                }
                if (rDetail.Room != null)
                {
                    rdDTO.Room = rDetail.Room;
                    RoomDetailDTO rd = UIProcessManager.GetRoomDetailById(rDetail.Room.Value);
                    if (rd != null)
                    {
                        rdDTO.roomDesc = rd.Description;
                    }
                }
                if (rDetail.RateCode != null)
                {
                    var firstOrDefault = UIProcessManager.GetRateCodeDetailById(rDetail.RateCode.Value);
                    if (firstOrDefault !=
                        null)
                        rdDTO.rateCodeDetailDesc = firstOrDefault.Description;
                }
                if (rDetail.ActualRtc != null)
                {
                    rdDTO.ActualRtc = rDetail.ActualRtc;
                    rdDTO.actualRTCDesc = UIProcessManager.GetRoomTypeById(rDetail.ActualRtc.Value).Description;
                }
                if (rDetail.Market != null)
                {
                    rdDTO.Market = rDetail.Market;
                    rdDTO.marketDesc = LocalBuffer.LocalBuffer.SystemConstantDTOBufferList.FirstOrDefault(x => x.Id == rDetail.Market).Description;
                }
                if (rDetail.Source != null)
                {
                    rdDTO.Source = rDetail.Source;
                    rdDTO.sourceDesc = LocalBuffer.LocalBuffer.SystemConstantDTOBufferList.FirstOrDefault(x => x.Id == rDetail.Source).Description;
                }
                rdDTO.DayOfWeek = rDetail.Date.Value.DayOfWeek;
                rdDTO.RoomCount = rDetail.RoomCount;
                if (rDetail.RateAmount != null) rdDTO.RateAmount = Math.Round(rDetail.RateAmount.Value, 2);
                rdDTO.IsFixedRate = rDetail.IsFixedRate;
                rdDTO.Date = rDetail.Date;
                rdDTO.Adult = rDetail.Adult;
                rdDTO.Child = rDetail.Child;
                if (rDetail.Date != null) rdDTO.DayOfWeek = rDetail.Date.Value.DayOfWeek;
                rdDTO.Id = rDetail.Id;
                rdDTO.Day = rDetail.Day;
                rdDTO.Month = rDetail.Month;
                rdDTO.Year = rDetail.Year;


                rdDTO.PackagesToPostDTOs = PopulatePkgToPostDTO(rDetail.Id);

                _regDetailDTO.Add(rdDTO);
            }//end of foreach

        }

        #endregion



        #region Properties


        int regVoucher;
        internal int RegVoucher
        {
            get { return regVoucher; }
            set
            {
                regVoucher = value;

                // Progress_Reporter.Show_Progress("Loading data. Please Wait...");
                if (_regDetailList == null)
                {
                    _regDetailList = UIProcessManager.GetRegistrationDetailByvoucher(regVoucher);
                }

                PopulateRegDetail();

                ////CNETInfoReporter.Hide();
            }


        }

        internal List<RegistrationDetailDTO> RegDetailList
        {
            get { return _regDetailList; }
            set
            {
                _regDetailList = value;
            }
        }

        #endregion


        private void frmReservationDetail_FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e)
        {
            //  RegDetail = cdeDetail.Logic.GetData().Cast<RegistrationDetail>().ToList();
            // PckToPOst = filteredData;
            DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        public void LoadData(UILogicBase requesterForm, object args)
        {
            throw new NotImplementedException();
        }

        private void gv_resDetail_RowStyle(object sender, RowStyleEventArgs e)
        {
            GridView View = sender as GridView;
            if (e.RowHandle >= 0)
            {
                string category = View.GetRowCellDisplayText(e.RowHandle, View.Columns[2]);
                if (category == "Sunday" || category == "Saturday")
                {
                    e.Appearance.BackColor = Color.Aqua;
                    //  e.Appearance.BackColor2 = Color.SeaShell;
                    e.HighPriority = true;
                }
            }
        }

        private void gv_resDetail_DoubleClick(object sender, EventArgs e)
        {
            EditHandler();

        }

        private void gc_resDetail_ViewRegistered(object sender, DevExpress.XtraGrid.ViewOperationEventArgs e)
        {
            if (e.View.IsDetailView)
            {
                var amountCol = (e.View as GridView).Columns["Amount"];
                amountCol.Caption = "Amount";
                amountCol.Width = 20;
                amountCol.AppearanceHeader.Font = new System.Drawing.Font(amountCol.AppearanceHeader.Font.FontFamily,
                    amountCol.AppearanceHeader.Font.Size, FontStyle.Bold);
                amountCol.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Default;

                var pkgHeaderCol = (e.View as GridView).Columns["PackageHeader"];
                pkgHeaderCol.Caption = "Package";
                pkgHeaderCol.Width = 25;
                pkgHeaderCol.AppearanceHeader.Font = new System.Drawing.Font(pkgHeaderCol.AppearanceHeader.Font.FontFamily,
                    pkgHeaderCol.AppearanceHeader.Font.Size, FontStyle.Bold);
                pkgHeaderCol.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;

                var remarkCol = (e.View as GridView).Columns["Remark"];
                remarkCol.Caption = "Remark";
                remarkCol.AppearanceHeader.Font = new System.Drawing.Font(remarkCol.AppearanceHeader.Font.FontFamily,
                    remarkCol.AppearanceHeader.Font.Size, FontStyle.Bold);
                remarkCol.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;

                e.View.LayoutChanged();



            }
        }

        private void bbiEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            EditHandler();
        }

        private void beiExpandCheckBx_CheckedChanged(object sender, EventArgs e)
        {

        }


        private void bci_expandAll_CheckedChanged(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (bci_expandAll.Checked)
            {
                gv_resDetail.BeginUpdate();
                try
                {
                    int dataRowCount = gv_resDetail.DataRowCount;
                    for (int rHandle = 0; rHandle < dataRowCount; rHandle++)
                        gv_resDetail.SetMasterRowExpanded(rHandle, true);
                }
                finally
                {
                    gv_resDetail.EndUpdate();
                }
            }
            else
            {
                gv_resDetail.BeginUpdate();
                try
                {
                    int dataRowCount = gv_resDetail.DataRowCount;
                    for (int rHandle = 0; rHandle < dataRowCount; rHandle++)
                        gv_resDetail.SetMasterRowExpanded(rHandle, false);
                }
                finally
                {
                    gv_resDetail.EndUpdate();
                }
            }
        }

        private void bbiClose_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Close();
        }

        private void gc_resDetail_Click(object sender, EventArgs e)
        {

        }


        private void bbiPdfExport_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                SaveFileDialog sfd = new SaveFileDialog() { Title = "Save", DefaultExt = "xls", Filter = "*.pdf|*.pdf", FileName = this.Text };
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    gc_resDetail.ExportToPdf(sfd.FileName);
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message, "Reservation Detail");
            }
        }

        private void bbiExcelReport_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                SaveFileDialog sfd = new SaveFileDialog() { Title = "Save", DefaultExt = "xls", Filter = "*.xls|*.xls|*.xlsx|*.xlsx", FileName = this.Text };
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    gc_resDetail.ExportToXls(sfd.FileName);
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message, "Reservation Detail");
            }
        }

        private void bbiPrint_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ReportGenerator reportGenerator = new ReportGenerator();
            reportGenerator.GenerateGridReport(gc_resDetail, this.Text, CurrentTime.ToShortDateString());
        }

        public int SelectedHotelcode { get; set; }
    }
}
