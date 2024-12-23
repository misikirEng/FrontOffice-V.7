using CNET.ERP.ResourceProvider;
using DevExpress.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CNETResourceProvider.Resource_Provider
{
    public partial class IconBrowser : UserControl
    {
        public IconBrowser()
        {
            InitializeComponent();
             PopulateImageToGrid();
        }

        private void PopulateImageToGrid()
        {
            dtbl = new DataTable();
            dtbl.Columns.Add("SN", typeof(Int32));
            dtbl.Columns.Add("Image", typeof(Bitmap));
            dtbl.Columns.Add("Name", typeof(String));
            dtbl.Columns.Add("Size", typeof(String));
            dtbl.Columns.Add("Location", typeof(String));

            gridControl1.DataSource = dtbl;
            gridView1.RefreshData();
             
            List<object> checkedvalues=  chkCombo.Items.GetCheckedValues();

            DevExpress.Utils.ImageCollection imagecol = new DevExpress.Utils.ImageCollection();
            if ( checkedvalues.Contains("8X8")&& icIcon8 != null && icIcon8.Images != null)
            {
                PopulateImageList(icIcon8.Images, "icIcon8");
            }
            if (checkedvalues.Contains("16X16") && icicon16 != null && icicon16.Images != null)
            {
                PopulateImageList(icicon16.Images, "icicon16");
            }
            if (checkedvalues.Contains("20X20") && icIcon20 != null && icIcon20.Images != null)
            {
                PopulateImageList(icIcon20.Images, "icIcon20");
            }
            if (checkedvalues.Contains("24X24") && icicon24 != null && icicon24.Images != null)
            {
                PopulateImageList(icicon24.Images, "icicon24");
            }
            if (checkedvalues.Contains("28X28") && icicon28 != null && icicon28.Images != null)
            {
                PopulateImageList(icicon28.Images, "icicon28");
            }
            if (checkedvalues.Contains("32X32") && icIcon32 != null && icIcon32.Images != null)
            {
                PopulateImageList(icIcon32.Images, "icIcon32");
            }
            if (checkedvalues.Contains("40X40") && icIcon40 != null && icIcon40.Images != null)
            {
                PopulateImageList(icIcon40.Images, "icIcon40");
            }
            if (checkedvalues.Contains("RMS") && icRMSPOSItems != null && icRMSPOSItems.Images != null)
            {
                PopulateImageList(icRMSPOSItems.Images, "icRMSPOSItems");
            }
            if (checkedvalues.Contains("Other"))
            { 
                if (icDummyImages != null && icDummyImages.Images != null)
                {
                    PopulateImageList(icDummyImages.Images, "icDummyImages");
                }
                if (icLogoImages != null && icLogoImages.Images != null)
                {
                    PopulateImageList(icLogoImages.Images, "icLogoImages");
                }
                if (icLogOff != null && icLogOff.Images != null)
                {
                    PopulateImageList(icLogOff.Images, "icLogOff");
                }
                 
            }
        }

        public void PopulateImageList(Images imglist,string Location )
        {

            int index = 0;

            foreach (Image img in imglist)
            {
                DataRow row = dtbl.NewRow();

                Object[] obj = new Object[5];

                obj[0] = dtbl.Rows.Count;

                obj[1] = img;

                obj[2] = imglist.Keys[index];

                obj[3] = img.Size.Height + "X" + img.Size.Width;

                obj[4] = Location;

                row.ItemArray = obj;

                dtbl.Rows.Add(row);

                index++;

            }
            gridControl1.DataSource = dtbl;

            gridView1.RefreshData();
        }

        DataTable dtbl = new DataTable(); 

        private void CheckedChanged(object sender, EventArgs e)
        {
            pictureEdit1.EditValue = null;
            PopulateImageToGrid();
        }

        private void chkCombo_EditValueChanged(object sender, EventArgs e)
        {
            pictureEdit1.EditValue = null;
            PopulateImageToGrid();

        }
         

        private void gridView1_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            if (gridControl1.DataSource != null)
            {
                System.Data.DataRowView obj = (System.Data.DataRowView)gridView1.GetFocusedRow();
                if (obj != null)
                {
                    Image im = (Image)obj.Row.ItemArray[1];

                    if (obj != null)
                    {
                        pictureEdit1.EditValue = im;
                    }
                }
            }
            else
            {
                pictureEdit1.EditValue = null;
            }
        }
    }
}
