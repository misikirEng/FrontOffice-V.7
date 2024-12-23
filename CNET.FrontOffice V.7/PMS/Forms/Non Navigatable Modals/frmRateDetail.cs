using System;
using System.Drawing;
using CNET.FrontOffice_V._7.PMS.Contracts;
using CNET.ERP.Client.Common.UI.Library;
using CNET.ERP.ResourceProvider;
using CNET.FrontOffice_V._7.PMS.Contracts;

namespace CNET.FrontOffice_V._7.Forms
{
    public partial class frmRateDetail : UILogicBase, ILogicHelper, ICanCreate, ICanSave, ICanDelete
    {
        public frmRateDetail()
        {
            InitializeComponent();
            FormSize = new Size(500, 450);
            ApplyIcons();
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


            Image = Provider.GetImage("Search", ProviderType.APPLICATIONICON, PictureSize.Dimension_32X32);

            bbiSearch.Glyph = Image;
            bbiSearch.LargeGlyph = Image;
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

        public void InitializeUI()
        {
            if (!DesignMode)
            {
                Utility.AdjustRibbon(lciRibbonHolder);
            }
            CNETFooterRibbon.ribbonControl = rcRateDetail;
        }

        public void InitializeData()
        {
        }

        public void LoadData(UILogicBase requesterForm, object args)
        {
        }
        private void bbiSearch_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //Home.OpenForm(this, "RATE SEARCH", null);
        }

        private void bbiSummery_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //Home.OpenForm(this, "RATE SUMMERY", null);
        }
    }
}
