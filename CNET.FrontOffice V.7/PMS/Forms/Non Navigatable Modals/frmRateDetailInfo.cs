using System.Collections.Generic;
using CNET.FrontOffice_V._7.PMS.Common_Classes;
using CNET_V7_Domain.Misc.PmsDTO;
using DevExpress.XtraEditors;

namespace CNET.FrontOffice_V._7.Forms.Non_Navigatable_Modals
{
    public partial class frmRateDetailInfo : XtraForm
    {
        private List<DailyRateCodeDTO> dailyRateCodes;
        private AvailableRateDTO availableRate;
        public frmRateDetailInfo()
        {
            InitializeComponent();
            gv_rateDetailInfo.Columns[1].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            gv_rateDetailInfo.Columns[1].AppearanceCell.Options.UseTextOptions = true;

            InitializeData();
        }


        public List<DailyRateCodeDTO> dailyRateCodeList { get; private set; }
        private void InitializeData()
        {
            if (availableRate != null)
            {
                gc_rateDetailInfo.DataSource = availableRate.DailyRateCode;

            }
            Utility.AddStatusBar(ribbonControl1);
        }

        public RateSearchCellClickedEventArgs RateSearchCellClickedEventArgs { get; set; }

        private void bbiClose_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Close();
        }

        internal AvailableRateDTO AvailableRate
        {
            get { return availableRate; }
            set
            {
                availableRate = value;
                dailyRateCodes = value.DailyRateCode;

                if (dailyRateCodes != null)
                {
                    gc_rateDetailInfo.DataSource = dailyRateCodes;
                    dailyRateCodeList = dailyRateCodes;
                }
                // SystemMessage.ShowStatusBarMessage(this, "Total Amount: " + availableRate.TotalAmount + " ," + "Average Amount: " + availableRate.AverageAmount + " ," + "First Night Amount: " + availableRate.FirstNightAmount);
            }
        }


    }
}