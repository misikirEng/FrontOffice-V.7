using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using CNET.ERP.Client.Common.UI;
using CNET.FrontOffice_V._7.PMS.Contracts;
using CNET.FrontOffice_V._7;

using DevExpress.XtraBars;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraTab;
using DevExpress.XtraEditors;
using CNET_V7_Domain.Domain.PmsSchema;
using CNET_V7_Domain.Domain.ConsigneeSchema;
using ProcessManager;
using CNET.FrontOffice_V._7.PMS.Common_Classes;
using DevExpress.Data.Svg;
using DocumentPrint;
using CNET_V7_Domain.Domain.ArticleSchema;

namespace CNET.FrontOffice_V._7.Forms
{
    //public partial class frmPackage : XtraForm
    public partial class frmPackage : UILogicBase
    {
        private List<PackageDetailDTO> _packageDetailList;
        private List<PackageHeaderDTO> _pkgHeaderList = null;

        /******************* CONSTRUCTOR *********************/
        public frmPackage()
        {
            InitializeComponent();
            InitializeUI();
            InitializeData();

            tlOrganizationUnit.ParentFieldName = "parentId";
            tlOrganizationUnit.KeyFieldName = "Id";
            tlOrganizationUnit.DataSource = LocalBuffer.LocalBuffer.HotelBranchBufferList.Select(x => new { x.Id, x.ParentId, Hotel = x.Name }).ToList();
            tlOrganizationUnit_FocusedNodeChanged(null, null);
        }

        #region Helper Methods

        public void InitializeUI()
        {
            //CNETFooterRibbon.ribbonControl = ribbonControl1;
            //Utility.AdjustRibbon(lciRibbonHolder);
        }

        public void InitializeData()
        {
            // PopulatePackageHeaderGrid(false);
        }

        public void OnDelete()
        {
            if (tcPackage.SelectedTabPage == tpPackageHeader)
            {
                bool canDelete = true;
                PackageHeaderView pkgHeader = ((PackageHeaderView)gv_pkgHeader.GetFocusedRow());
                if (pkgHeader == null)
                {
                    SystemMessage.ShowModalInfoMessage("Please select package header.", "ERROR");
                    return;
                }
                // Progress_Reporter.Show_Progress("Deleting Package Header");

                List<PackageDetailDTO> packageDetails = UIProcessManager.GetPackageDetailByHeader(pkgHeader.Id);
                if (packageDetails != null && packageDetails.Count > 0)
                {
                    canDelete = false;
                }
                List<PackagesToPostDTO> packagesToPosts = UIProcessManager.GetPackagesToPostByHeader(pkgHeader.Id);
                if (packagesToPosts != null && packagesToPosts.Count > 0)
                {
                    canDelete = false;
                }
                List<RateCodePackageDTO> rateCodePackages = UIProcessManager.GetRateCodePackageBypackageHeader(pkgHeader.Id);
                if (rateCodePackages != null && rateCodePackages.Count > 0)
                {
                    canDelete = false;
                }
                if (canDelete)
                {
                    try
                    {
                        List<PostingScheduleDTO> PostingScheduleList = UIProcessManager.GetPostingScheduleBypackageHeader(pkgHeader.Id);
                        if (PostingScheduleList != null)
                            PostingScheduleList.ForEach(x => UIProcessManager.DeletePostingScheduleById(x.Id));

                        if (UIProcessManager.DeletePackageHeaderById(pkgHeader.Id))
                        {
                            ////CNETInfoReporter.Hide();
                            SystemMessage.ShowModalInfoMessage("Deleted Successfully", "MESSAGE");
                            PopulatePackageHeaderGrid(true);
                        }
                        else
                        {
                            SystemMessage.ShowModalInfoMessage("Deleting failed. Please check if it is used in other trasactions!", "ERROR");
                        }
                    }
                    catch (Exception ex)
                    {
                        ////CNETInfoReporter.Hide();
                        SystemMessage.ShowModalInfoMessage("Deleting failed! Detail: " + ex.Message, "ERROR");
                    }

                }
                else
                {
                    SystemMessage.ShowModalInfoMessage("You can not delete. It has other transaction!", "ERROR");
                }

                ////CNETInfoReporter.Hide();
            }
            else if (tcPackage.SelectedTabPage == tpPackageDetail)
            {
                PackageDetailDTO packageDetail = ((PackageDetailDTO)gv_pkgDetailMain.GetFocusedRow());
                if (packageDetail == null)
                {
                    SystemMessage.ShowModalInfoMessage("Please select package detail", "ERROR");
                    return;
                }
                try
                {
                    // Progress_Reporter.Show_Progress("Deleting Package Detail");

                    List<WeekDayDTO> weekdaylist = UIProcessManager.GetWeekDaysByreference(packageDetail.Id);
                    if (weekdaylist != null)
                        weekdaylist.ForEach(x => UIProcessManager.DeleteWeekDayById(x.Id));

                    if (UIProcessManager.DeletePackageDetailById(packageDetail.Id))
                    {
                        ////CNETInfoReporter.Hide();
                        SystemMessage.ShowModalInfoMessage("Deleted Successfully", "MESSAGE");

                        //refresh
                        PackageHeaderView pkgHeader = gv_pkgDetailList.GetFocusedRow() as PackageHeaderView;
                        if (pkgHeader == null) return;
                        PopulatePackageDetailGrid(pkgHeader.Id);
                    }
                    else
                    {
                        SystemMessage.ShowModalInfoMessage("Can not delete. It has other transaction!", "ERROR");
                    }
                }
                catch (Exception ex)
                {
                    ////CNETInfoReporter.Hide();
                    SystemMessage.ShowModalInfoMessage("Deleting failed! Detail: " + ex.Message, "ERROR");
                }
            }


        }

        int selectedHotelcode { get; set; }
        private void PopulatePackageHeaderGrid(bool isRefresh)
        {
            try
            {
                if (!isRefresh && _pkgHeaderList != null) return;

                //// Progress_Reporter.Show_Progress("Loading Pakage Headers");
                gc_pkgHeader.DataSource = null;

                //_pkgHeaderList = UIProcessManager.SelectAllPackage();
                _pkgHeaderList = UIProcessManager.GetAllPackageHeaderByConsigneeUnit(selectedHotelcode);
                if (_pkgHeaderList == null || _pkgHeaderList.Count == 0)
                {
                    ////CNETInfoReporter.Hide();
                    return;
                }


                List<PackageHeaderView> _packageHeadersView = new List<PackageHeaderView>();
                foreach (PackageHeaderDTO ph in _pkgHeaderList)
                {
                    PackageHeaderView phv = new PackageHeaderView();
                    phv.Id = ph.Id;
                    phv.Description = ph.Description;

                    if (ph.Article != null)
                    {
                        ArticleDTO Packagearticle = UIProcessManager.GetArticleById(ph.Article);
                        if (Packagearticle != null)
                        {
                            phv.Article = Packagearticle.Name;

                        }
                    }

                    var lukGroup = LocalBuffer.LocalBuffer.LookUpBufferList.FirstOrDefault(l => l.Id == ph.PakageGroup);
                    phv.Group = lukGroup == null ? "" : lukGroup.Description;

                    var lukType = LocalBuffer.LocalBuffer.LookUpBufferList.FirstOrDefault(l => l.Id == ph.Type);
                    phv.Type = lukType == null ? "" : lukType.Description;

                    var currency = LocalBuffer.LocalBuffer.CurrencyBufferList.FirstOrDefault(c => c.Id == ph.CurrencyPreference);
                    phv.Currency = currency == null ? "" : currency.Description;

                    var lukPostingRhyt = LocalBuffer.LocalBuffer.SystemConstantDTOBufferList.FirstOrDefault(l => l.Id == ph.PostingRhythm);
                    phv.PostingRhythm = lukPostingRhyt == null ? "" : lukPostingRhyt.Description;

                    var hoteloud = LocalBuffer.LocalBuffer.HotelBranchBufferList.FirstOrDefault(x => x.Id == selectedHotelcode);
                    if (hoteloud != null)
                        phv.Hotel = hoteloud.Description;

                    _packageHeadersView.Add(phv);
                }


                gc_pkgHeader.DataSource = _packageHeadersView;
                gv_pkgHeader.RefreshData();

                GetAllPackageHeder();


                ////CNETInfoReporter.Hide();
            }
            catch (Exception ex)
            {
                //////CNETInfoReporter.Hide();
                SystemMessage.ShowModalInfoMessage("Exception has occured in populating package headers. DETAIL:: " + ex.Message, "ERROR");

            }

        }
        public void GetAllPackageHeder()
        {
            List<PackageHeaderView> _packageHeadersView = new List<PackageHeaderView>();
            foreach (ConsigneeUnitDTO v in LocalBuffer.LocalBuffer.HotelBranchBufferList)
            {
                List<PackageHeaderDTO> _pkgHeaderList = UIProcessManager.GetAllPackageHeaderByConsigneeUnit(v.Id);
                if (_pkgHeaderList != null && _pkgHeaderList.Count > 0)
                {
                    foreach (PackageHeaderDTO ph in _pkgHeaderList)
                    {
                        PackageHeaderView phv = new PackageHeaderView();

                        phv.Id = ph.Id;
                        phv.Description = ph.Description;

                        if (ph.Article != null && ph.Article >0)
                            phv.Article = UIProcessManager.GetArticleById(ph.Article).Name;

                        var lukGroup = LocalBuffer.LocalBuffer.SystemConstantDTOBufferList.FirstOrDefault(l => l.Id == ph.PakageGroup);
                        phv.Group = lukGroup == null ? "" : lukGroup.Description;

                        var lukType = LocalBuffer.LocalBuffer.SystemConstantDTOBufferList.FirstOrDefault(l => l.Id == ph.Type);
                        phv.Type = lukType == null ? "" : lukType.Description;

                        var currency = LocalBuffer.LocalBuffer.CurrencyBufferList.FirstOrDefault(c => c.Id == ph.CurrencyPreference);
                        phv.Currency = currency == null ? "" : currency.Description;

                        var lukPostingRhyt = LocalBuffer.LocalBuffer.SystemConstantDTOBufferList.FirstOrDefault(l => l.Id == ph.PostingRhythm);
                        phv.PostingRhythm = lukPostingRhyt == null ? "" : lukPostingRhyt.Description;

                        phv.Hotel = v.Name;

                        _packageHeadersView.Add(phv);
                    }
                }
            }
            gc_pkgDetailList.DataSource = _packageHeadersView;
            gv_pkgDetailList.RefreshData();
            gv_pkgDetailList.ExpandAllGroups();
            gv_pkgHeader.OptionsView.ShowGroupPanel = false;

        }
        private void PopulatePackageDetailGrid(int packageHeader)
        {
            try
            {
                var packageDetails = UIProcessManager.GetPackageDetailByHeader(packageHeader);
                gc_pkgDetailMain.DataSource = null;
                gv_pkgDetailMain.RefreshData();
                if (packageDetails == null || packageDetails.Count == 0)
                {
                    return;

                }

                _packageDetailList = packageDetails.Select(s => new PackageDetailDTO()
                {
                    Id = s.Id,
                    Price = s.Price != null ? Math.Round(s.Price.Value, 2) : 0,
                    EndingDate = s.EndingDate,
                    StartDate = s.StartDate,
                    Allowance = s.Allowance != null ? Math.Round(s.Allowance.Value, 2) : 0
                }).ToList();

                gc_pkgDetailMain.DataSource = _packageDetailList;
                gv_pkgDetailMain.RefreshData();

            }
            catch (Exception ex)
            {
                ////CNETInfoReporter.Hide();
                SystemMessage.ShowModalInfoMessage("Exception has occured in populating package details. DETAIL:: " + ex.Message, "ERROR");

            }
        }

        #endregion

        #region Event Handlers

        private void bbiNew_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (tcPackage.SelectedTabPage == tpPackageHeader)
            {
                frmPackageHeader packageEditor = new frmPackageHeader();
                packageEditor.selectedhotel = selectedHotelcode;
                DialogResult dialogResult = packageEditor.ShowDialog(this);

                if (dialogResult == DialogResult.OK)
                {
                    PopulatePackageHeaderGrid(true);
                }
            }
            else if (tcPackage.SelectedTabPage == tpPackageDetail)
            {
                PackageHeaderView pkgHeader = gv_pkgDetailList.GetFocusedRow() as PackageHeaderView;
                if (pkgHeader == null)
                {
                    SystemMessage.ShowModalInfoMessage("Please select package header!", "ERROR");
                    return;
                }
                frmPackageDetail creatorForm = new frmPackageDetail();
                creatorForm.PackageHeader = pkgHeader;
                creatorForm.Tag = this;

                DialogResult dialogResult = creatorForm.ShowDialog(this);

                if (dialogResult == DialogResult.OK)
                {
                    PopulatePackageDetailGrid(pkgHeader.Id);
                }
            }


        }

        private void bbiEdit_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (tcPackage.SelectedTabPage == tpPackageHeader)
            {
                PackageHeaderView packageHeaderView = ((PackageHeaderView)gv_pkgHeader.GetFocusedRow());
                if (packageHeaderView == null) return;
                frmPackageHeader frmPackageHeader = new frmPackageHeader();
                frmPackageHeader.selectedhotel = selectedHotelcode;
                frmPackageHeader.EditedPackageHeader = packageHeaderView;
                frmPackageHeader.Tag = this;

                DialogResult dialogResult = frmPackageHeader.ShowDialog(this);

                if (dialogResult == DialogResult.OK)
                {
                    PopulatePackageHeaderGrid(true);
                }
            }
            else if (tcPackage.SelectedTabPage == tpPackageDetail)
            {
                PackageDetailDTO packageDetail = ((PackageDetailDTO)gv_pkgDetailMain.GetFocusedRow());
                if (packageDetail == null) return;
                PackageHeaderView pkgHeader = gv_pkgDetailList.GetFocusedRow() as PackageHeaderView;
                if (pkgHeader == null) return;

                frmPackageDetail creatorForm = new frmPackageDetail { Tag = this };
                creatorForm.EditedPackageDetail = packageDetail;
                creatorForm.PackageHeader = pkgHeader;
                DialogResult dialogResult = creatorForm.ShowDialog(this);

                if (dialogResult == DialogResult.OK)
                {

                    PopulatePackageDetailGrid(pkgHeader.Id);
                }
            }

        }

        private void bbiDelete_ItemClick(object sender, ItemClickEventArgs e)
        {
            OnDelete();
        }

        private void tcPackage_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            if (tcPackage.SelectedTabPage == tpPackageHeader)
            {
                PopulatePackageHeaderGrid(false);

            }
            else if (tcPackage.SelectedTabPage == tpPackageDetail)
            {
                PackageHeaderView pkgHeader = gv_pkgDetailList.GetFocusedRow() as PackageHeaderView;
                if (pkgHeader == null) return;
                PopulatePackageDetailGrid(pkgHeader.Id);
            }

        }

        private void gv_pkgHeader_DoubleClick(object sender, EventArgs e)
        {
            bbiEdit_ItemClick(sender, new ItemCancelEventArgs(null, null, true));
        }

        private void gv_pkgDetailList_Click(object sender, EventArgs e)
        {
            PackageHeaderView pkgHeader = gv_pkgDetailList.GetFocusedRow() as PackageHeaderView;
            if (pkgHeader == null) return;
            PopulatePackageDetailGrid(pkgHeader.Id);
        }

        private void gv_pkgDetailMain_DoubleClick(object sender, EventArgs e)
        {

            bbiEdit_ItemClick(sender, new ItemCancelEventArgs(null, null, true));
        }

        private void gv_pkgDetailList_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            if (e.Column == gCol_Icon && e.IsGetData)
            {
                e.Value = imageCollection1.Images[1];
            }
        }

        private void bbiPrint_ItemClick(object sender, ItemClickEventArgs e)
        {
            DateTime? currentTime = UIProcessManager.GetServiceTime();


            ReportGenerator reportGen = new ReportGenerator();
            if (tcPackage.SelectedTabPage == tpPackageHeader)
            {
                reportGen.GenerateGridReport(gc_pkgHeader, "Package Headers", currentTime.Value.ToShortDateString());
            }
            else if (tcPackage.SelectedTabPage == tpPackageDetail)
            {
                PackageHeaderView pkgHeader = gv_pkgDetailList.GetFocusedRow() as PackageHeaderView;
                if (pkgHeader == null) return;
                reportGen.GenerateGridReport(gc_pkgDetailMain, "Package Details for " + pkgHeader.Description, currentTime.Value.ToShortDateString());
            }
        }

        #endregion




        private void tlOrganizationUnit_FocusedNodeChanged(object sender, DevExpress.XtraTreeList.FocusedNodeChangedEventArgs e)
        {
            if (tlOrganizationUnit.FocusedNode != null)
            {
                string code =
                   Convert.ToString(tlOrganizationUnit.FocusedNode.GetValue("Id"));
                if (!string.IsNullOrEmpty(code))
                {
                    selectedHotelcode = Convert.ToInt32(code);

                    PopulatePackageHeaderGrid(true);
                }
            }
        }

    }
}
