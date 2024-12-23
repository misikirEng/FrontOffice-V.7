using System;
using System.Collections.Generic;
using System.Linq;
using CNET.FrontOffice_V._7.PMS.Contracts;
using CNET.FrontOffice_V._7;
using CNET.ERP.Client.Common.UI;
using CNET.FrontOffice_V._7.Forms.Non_Navigatable_Modals;
using System.Windows.Forms;
using DevExpress.XtraGrid.Views.Grid;
using System.Drawing;
using CNET_V7_Domain.Misc.PmsDTO;
using CNET.FrontOffice_V._7.PMS.Common_Classes;
using CNET_V7_Domain.Domain.PmsSchema;
using ProcessManager;
using CNET_V7_Domain.Domain.SettingSchema;
using CNET_V7_Domain.Domain.CommonSchema;
using DocumentPrint;

namespace CNET.FrontOffice_V._7.Forms
{
    public partial class frmRateSummery : UILogicBase
    {
        private RegistrationListVMDTO regExtension;

        /** Property **/
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

        internal RegistrationListVMDTO RegExtension
        {
            get { return regExtension; }
            set
            {
                regExtension = value;

            }
        }

        /********************************   CONSTRUCTOR     ***************/
        public frmRateSummery()
        {
            InitializeComponent();

            InitializeUI();
        }


        #region Helper Methods

        public void InitializeUI()
        {
            if (!DesignMode)
            {
                Utility.AdjustRibbon(lciRibbonHolder);
            }
            CNETFooterRibbon.ribbonControl = rcRateSummery;
            this.StartPosition = FormStartPosition.CenterScreen;

        }

        public bool InitializeData()
        {
            try
            {
                if (RegExtension == null)
                {
                    SystemMessage.ShowModalInfoMessage("Please Select a Registration!", "ERROR");
                    return false;
                }

                // Progress_Reporter.Show_Progress("Loading data. please wait...");
                List<RegistrationDetailDTO> _registrationDetails = UIProcessManager.GetRegistrationDetailByvoucher(RegExtension.Id);
                if (_registrationDetails == null || _registrationDetails.Count == 0)
                {
                    SystemMessage.ShowModalInfoMessage("Unable to get registration detail.", "ERROR");
                    return false;
                }

                List<RateCodeDetailDTO> rateCodeLIst = UIProcessManager.SelectAllRateCodeDetail();

                List<RateSummaryVM> summaryList = new List<RateSummaryVM>();
                decimal totalCost = 0;
                decimal deposit = 0;
                TaxDTO tax = UIProcessManager.GetTaxById(CNETConstantes.VAT);
                decimal taxRate = tax == null ? 15 : tax.Amount;
                decimal serviceCharge = 0;
                //Service Charge
                var vfdList = UIProcessManager.SelectAllValueFactorDefinition();
                List<int> vfdCodeList = vfdList.Where(v => v.Type == CNETConstantes.ADDTIONAL_CHARGE).Select(v => v.Id).ToList();
                foreach (RegistrationDetailDTO reg in _registrationDetails)
                {
                    RateSummaryVM dto = new RateSummaryVM();
                    dto.regDetailCode = reg.Id;
                    dto.date = reg.Date.Value;
                    dto.day = reg.Date.Value.DayOfWeek;
                    RateCodeDetailDTO rateCodeDetail = rateCodeLIst.FirstOrDefault(r => r.Id == reg.RateCode);
                    if (rateCodeDetail != null)
                    {
                        RateCodeHeaderDTO rateCode = UIProcessManager.GetRateCodeHeaderById(rateCodeDetail.RateCodeHeader);
                        if (rateCode != null)
                        {
                            //Getting Service Charge
                            var vfList = UIProcessManager.GetValueFactorByreference(rateCode.Article);
                            if (vfList != null && vfdCodeList != null)
                            {
                                ValueFactorDTO vf = vfList.FirstOrDefault(v => vfdCodeList.Contains(v.ValueFactorDefinition.Value));
                                if (vf != null)
                                {
                                    ValueFactorDefinitionDTO vfd = UIProcessManager.GetValueFactorDefinitionById(vf.ValueFactorDefinition.Value);
                                    if (vfd != null)
                                        serviceCharge = vfd.Value;
                                }
                            }
                            dto.rateDescription = rateCode.Description;
                            dto.rateCode = rateCode.Id;
                            dto.rateCodeHeader = rateCode;
                        }
                    }
                    if (reg.RateAmount != null)
                    {

                        decimal factor = (((1 + (decimal)taxRate / 100)) * ((decimal)serviceCharge / 100)) + (1 + (decimal)taxRate / 100);
                        decimal rateAmt = Math.Round(reg.RateAmount.Value / factor, 2);
                        List<PackagesToPostDTO> pckList = UIProcessManager.GetPackagesToPostByRegistrationDetail(reg.Id).ToList();

                        decimal pckAmount = 0;
                        decimal quantity = 0;
                        if (pckList != null)
                        {
                            foreach (PackagesToPostDTO pk in pckList)
                            {
                                PackageHeaderDTO pkHeader = UIProcessManager.GetPackageHeaderById(pk.PackageHeader);
                                if (pkHeader != null)
                                {
                                    switch (pkHeader.CalculationRule.Value)
                                    {
                                        case CNETConstantes.Per_Person:
                                            quantity = Convert.ToDecimal(reg.Adult + reg.Child);
                                            break;
                                        case CNETConstantes.Flat_Rate:
                                            quantity = 1;
                                            break;
                                        case CNETConstantes.Per_Adult:
                                            quantity = Convert.ToDecimal(reg.Adult);
                                            break;
                                        case CNETConstantes.Per_Child:
                                            quantity = Convert.ToDecimal(reg.Child);
                                            break;
                                        case CNETConstantes.Per_Room:
                                            //To be implemented
                                            quantity = 1;
                                            break;
                                        default:
                                            quantity = 1;
                                            break;
                                    }
                                }

                                pckAmount += pk.Amount * (decimal)quantity;
                            }
                        }
                        pckAmount = Math.Round(pckAmount / factor, 2);
                        dto.packagesList = pckList;
                        dto.roomRevenue = Math.Round(rateAmt - pckAmount, 2);
                        dto.package = pckAmount;
                        dto.subTotal = Math.Round(rateAmt, 2);
                    }
                    dto.serCharge = (dto.subTotal * (decimal)serviceCharge) / 100;
                    dto.VAT = Math.Round(((decimal)taxRate * (dto.subTotal + dto.serCharge)) / 100, 2);
                    dto.grandTotal = Math.Round(dto.subTotal + dto.VAT + dto.serCharge, 2);
                    totalCost += dto.grandTotal;
                    summaryList.Add(dto);
                }
                teTotalCost.Text = totalCost.ToString();
                teDeposit.Text = deposit.ToString();
                teOutstandingCost.Text = (totalCost - deposit).ToString();
                gc_rateSummary.DataSource = summaryList;
                // cdeRateSummery.Properties.GridView.RowStyle += GridView_RowStyle;
                ////CNETInfoReporter.Hide();

                return true;
            }
            catch (Exception ex)
            {
                SystemMessage.ShowModalInfoMessage("Error in initializing rate summary. DETAIL:: " + ex.Message, "ERROR");
                return false;
            }
        }


        private void ShowRateDetail()
        {
            frmDailyRateDetail _frmDailyRateDetail = new frmDailyRateDetail();
            RateSummaryVM focusedDTo = (RateSummaryVM)gv_rateSummary.GetFocusedRow();
            if (focusedDTo != null)
            {
                _frmDailyRateDetail.FocusedRateSum = focusedDTo;
                _frmDailyRateDetail.ShowDialog(this);
            }
        }

        #endregion

        #region Event Handlers

        private void bbiPrintExport_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Close();
        }

        private void bbiDetail_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ShowRateDetail();
        }

        private void gv_rateSummary_RowStyle(object sender, RowStyleEventArgs e)
        {
            GridView View = sender as GridView;
            if (e.RowHandle >= 0)
            {
                string category = View.GetRowCellDisplayText(e.RowHandle, View.Columns[0]);
                if (category == "Sunday" || category == "Saturday")
                {
                    e.Appearance.BackColor = Color.Red;
                    e.Appearance.BackColor2 = Color.SeaShell;
                }
            }
        }

        private void gv_rateSummary_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            if (e.Column.Caption == "SN")
                e.DisplayText = (e.RowHandle + 1).ToString();
        }

        private void gv_rateSummary_DoubleClick(object sender, EventArgs e)
        {
            ShowRateDetail();
        }

        private void bbiPrint_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ReportGenerator rg = new ReportGenerator();
            DateTime? currentTime = UIProcessManager.GetServiceTime();
            if (currentTime != null)
            {
                rg.GenerateGridReport(gc_rateSummary, "Rate Summary", currentTime.Value.ToShortDateString());
            }
        }

        #endregion

        private void frmRateSummery_Load(object sender, EventArgs e)
        {
            if (!InitializeData())
            {
                this.Close();
            }
        }




    }
}
