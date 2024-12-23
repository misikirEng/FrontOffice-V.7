using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using CNET.ERP.Client.Common.UI;
using CNET.FrontOffice_V._7.PMS.Contracts;

using CNET.ERP.Client.Common.UI.Library;
using System.Windows.Forms;
using CNET.ERP.ResourceProvider;
using CNET.FrontOffice_V._7.PMS.Contracts;
using ProcessManager;
using CNET_V7_Domain.Domain.PmsSchema;

namespace CNET.FrontOffice_V._7.Forms.Non_Navigatable_Modals
{
    public partial class frmDailyDetail : UILogicBase, ILogicHelper, ICanCreate, ICanSave, ICanDelete
    {


        //////////////// CONSTRUCTOR /////////////////////

        public frmDailyDetail()
        {
            InitializeComponent();
            FormSize = new Size(1058, 582);
            ApplyIcons();

            // Progress_Reporter.Show_Progress("Loading data. Please Wait...");
            InitializeUI();
            InitializeData();
            ////CNETInfoReporter.Hide();

        }


        #region methods

        public void InitializeUI()
        {
            if (!DesignMode)
            {
                Utility.AdjustRibbon(lciRibbonHolder);
            }
            CNETFooterRibbon.ribbonControl = rcDailyDetail;

        }
        public void InitializeData()
        {
            try
            {
                List<RegistrationDetailDTO> registrationDetails = UIProcessManager.SelectAllRegistrationDetail();
                var filtered = (registrationDetails.Select(t => new
                {

                    Date = t.Date,
                    //
                    // TODO:
                    //currency = UIProcessManager.SelectCurrency(UIProcessManager.SelectAllCurrencyPreference()
                    //    .FirstOrDefault(c => c.Reference == UIProcessManager.SelectAllVoucher().FirstOrDefault(v =>
                    //    v.code == t.voucher).consignee).Currency).description,
                    RateCode = t.RateCode,
                    @fixed = t.IsFixedRate,
                    disAmt = 0,
                    percent = t.IsFixedRate != null && t.IsFixedRate.Value == false ? t.RateAmount : 0,
                    rateAmount = t.IsFixedRate != null && t.IsFixedRate.Value ? t.RateAmount : 0,
                    day = registrationDetails.Count(),
                    roomType = UIProcessManager.GetRoomTypeById(t.RoomType.Value).Description,
                    room = UIProcessManager.GetRoomDetailById(t.Room.Value).Description,
                    t.Adult,
                    t.Child,
                    t.ActualRtc
                })).Cast<object>().ToList();

                if (filtered.Count == 0)
                {
                    return;
                }

                gc_dailyDetail.DataSource = filtered;
            }
            catch (Exception dbEx)
            {
                MessageBox.Show("Error in intializing daily detail. DETAIL:: " + dbEx.Message, "CNET_ERPv2016", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ApplyIcons()
        {
            Image Image = Provider.GetImage("New", ProviderType.APPLICATIONICON, PictureSize.Dimension_32X32);

            bbiNew.Glyph = Image;
            bbiNew.LargeGlyph = Image;

            Image = Provider.GetImage("Save", ProviderType.APPLICATIONICON, PictureSize.Dimension_32X32);

            bbiSave.Glyph = Image;
            bbiSave.LargeGlyph = Image;

            Image = Provider.GetImage("Delete", ProviderType.APPLICATIONICON, PictureSize.Dimension_32X32);

            bbiDelete.Glyph = Image;
            bbiDelete.LargeGlyph = Image;


            Image = Provider.GetImage("Options", ProviderType.APPLICATIONICON, PictureSize.Dimension_32X32);

            bsiOptions.Glyph = Image;
            bsiOptions.LargeGlyph = Image;

        }
        public void OnCreate()
        {
            throw new NotImplementedException();
        }
        public SaveClickedResult OnSave()
        {
            throw new NotImplementedException();
        }

        public DeleteClickedResult OnDelete()
        {
            throw new NotImplementedException();
        }
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
        public void LoadData(UILogicBase requesterForm, object args)
        {
        }

        #endregion


        #region Event Handlers


        #endregion
    }
}
