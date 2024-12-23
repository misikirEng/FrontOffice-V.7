using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using CNET.FrontOffice_V._7;

using CNET.ERP.Client.Common.UI;
using ProcessManager;
using CNET_V7_Domain.Domain.PmsSchema;

namespace CNET.FrontOffice_V._7.Forms.Non_Navigatable_Modals
{
    public partial class frmDailyRateDetail : XtraForm
    {
        RateSummaryVM focusedRateSum;

        List<DailyRateVM> dailyDetailList = new List<DailyRateVM>();
        internal RateSummaryVM FocusedRateSum
        {
            get { return focusedRateSum; }
            set
            {
                focusedRateSum = value;

            }

        }

        //Properties
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

        /******** CONSTRUCTOR ****************/
        public frmDailyRateDetail()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
        }


        #region helper methods

        private bool InitializeData()
        {
            try
            {
                if (FocusedRateSum == null) return false;
                // Progress_Reporter.Show_Progress("loading data...");

                DailyRateVM obj = new DailyRateVM();
                obj.articleCode = FocusedRateSum.rateCodeHeader == null ? 0 : FocusedRateSum.rateCodeHeader.Article;
                obj.description = "Room Revenue";

                RegistrationDetailDTO registrationDetail = UIProcessManager.GetRegistrationDetailById(FocusedRateSum.regDetailCode);
                decimal rateAmt = registrationDetail != null ? registrationDetail.RateAmount.Value : 0;
                obj.amount = Math.Round(rateAmt, 2);
                dailyDetailList.Add(obj);
                if (FocusedRateSum.packagesList != null)
                {
                    foreach (PackagesToPostDTO pk in FocusedRateSum.packagesList)
                    {
                        obj = new DailyRateVM();
                        PackageHeaderDTO pack = UIProcessManager.GetPackageHeaderById(pk.PackageHeader);
                        if (pack != null)
                        {
                            obj.articleCode = pack.Article;
                            obj.description = pack.Description;
                        }
                        obj.amount = pk.Amount;
                        dailyDetailList.Add(obj);
                    }

                }
                gc_dailyRateDetail.DataSource = dailyDetailList;
                teSubTotal.Text = FocusedRateSum.subTotal.ToString();
                teAverage.Text = FocusedRateSum.serCharge.ToString();
                teVat.Text = FocusedRateSum.VAT.ToString();
                teDiscount.Text = "0";
                teGrandTotal.Text = FocusedRateSum.grandTotal.ToString();

                ////CNETInfoReporter.Hide();
                return true;
            }
            catch (Exception ex)
            {
                ////CNETInfoReporter.Hide();
                XtraMessageBox.Show("Error in initializing daily rate detail. DETAIL: " + ex.Message, "CNET_v2016", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        #endregion


        #region event handlers
        private void bbiClose_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Close();
        }

        private void frmDailyRateDetail_Load(object sender, EventArgs e)
        {
            if (!InitializeData())
            {
                this.Close();
            }
        }

        #endregion


        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();

                if (dailyDetailList != null)
                {
                    dailyDetailList.Clear();
                    dailyDetailList = null;
                }

                focusedRateSum = null;
            }
            base.Dispose(disposing);
        }





    }
}
