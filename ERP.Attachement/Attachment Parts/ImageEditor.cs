using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BitmapFilters;
using CNETAttachement;
using ImageContrast;

namespace CNET_ImageEditor
{
    public partial class imageEditor : DevExpress.XtraEditors.XtraForm
    {
        private const int CP_NOCLOSE_BUTTON = 0x200;
        private Bitmap originalBitmap = null;
        private Bitmap previewBitmap = null;
        private Bitmap resultBitmap = null;
        Bitmap bmpPicture;
        ImageAttributes iaPicture;
        ColorMatrix cmPicture;
        Graphics gfxPicture;
        Rectangle rctPicture;
        int cropX;
        int cropY;
        public Pen cropPen;
        int cropWidth;
        int cropHeight;
        private Image imageReset;
        private Image imageFilter;
        private Image imageBrg;
        private List<int> widthhhhh;
        List<double> ratios;
        public imageEditor()
        {
            InitializeComponent();
            widthhhhh = new List<int>();
            cmbSelectedCropBoxSize.Items.Add("100");
            widthhhhh.Add(100);
            cmbSelectedCropBoxSize.Items.Add("200");
            widthhhhh.Add(200);
            cmbSelectedCropBoxSize.Items.Add("300");
            widthhhhh.Add(300);
            cmbSelectedCropBoxSize.Items.Add("400");
            widthhhhh.Add(400);
            cmbSelectedCropBoxSize.Items.Add("500");
            widthhhhh.Add(500);
            cmbSelectedCropBoxSize.SelectedIndex = 0;
            cmbSelectedAspectRatio.Items.Add("Free Crop");
            ratios = new List<double>();
            cmbSelectedAspectRatio.Items.Add("4:3 Normal Display");
            ratios.Add(4.0 / 3.0);
            cmbSelectedAspectRatio.Items.Add("16:9 High Definition");
            ratios.Add(16.0 / 9.0);
            cmbSelectedAspectRatio.Items.Add("3:2 Digital Camera");
            ratios.Add(3.0 / 2.0);
            cmbSelectedAspectRatio.Items.Add("1:1 Square");
            ratios.Add(1.0);
            cmbSelectedAspectRatio.SelectedIndex = 0;
        }
        protected override CreateParams CreateParams
        {
            get
            {
                var myCp = base.CreateParams;
                myCp.ClassStyle = myCp.ClassStyle | CP_NOCLOSE_BUTTON;
                return myCp;
            }
        }
        private void imageEditor_Load(object sender, EventArgs e)
        {
            imageReset = pbEdit.Image;
            originalBitmap = (Bitmap)pbEdit.Image;
            imageFilter = pbEdit.Image;
            imageBrg = pbEdit.Image;
            previewBitmap = originalBitmap.CopyToSquareCanvas(pbEdit.Width);
            pbEdit.Image = previewBitmap;
            tbContrast.EditValue = 0;
            tbBrightness.EditValue = 0;
            OnCheckChangedEventHandler(sender, e);
        }
        private void bbiClose_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Hide();
        }
        public void bbiSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Hide();
        }
        private void ApplyFilter(bool preview)
        {
            if (previewBitmap == null)
            {
                return;
            }

            if (preview == true)
            {
                pbEdit.Image = previewBitmap.Contrast(tbContrast.Value);              
                imageFilter = pbEdit.Image;
                imageBrg = pbEdit.Image;
            }
            else
            {
                resultBitmap = originalBitmap.Contrast(tbContrast.Value);
                imageFilter = resultBitmap;
                imageBrg = pbEdit.Image;
            }
        }

        private void tbContrast_ValueChanged(object sender, EventArgs e)
        {
            lblContrastValue.Text = tbContrast.Value.ToString();

            ApplyFilter(true);}
        private void tbBrightness_ValueChanged(object sender, EventArgs e)
        {
            lblBrightnessValue.Text = tbBrightness.Value.ToString();
            pbEdit.Image = AdjustBrightness((Bitmap)imageBrg, tbBrightness.Value);
        }

        private void bbiRotate_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.AppStarting;
            pbEdit.Image.RotateFlip(RotateFlipType.Rotate90FlipNone);
            pbEdit.Refresh();
            Cursor = Cursors.Default;
            imageFilter = pbEdit.Image;
            imageBrg = pbEdit.Image;
            previewBitmap = (Bitmap)pbEdit.Image;
        }

       
        private void PreparePicture()
        {
            // If there's a pictureif (pbEdit.Image != null)
            {
                // Create new Bitmap object with the size of the picture
                bmpPicture = new Bitmap(pbEdit.Image.Width, pbEdit.Image.Height);

                // Image attributes for setting the attributes of the picture
                iaPicture = new System.Drawing.Imaging.ImageAttributes();
            }
        }

        private void FinalizePicture()
        {
            // Set the new color matrix
            iaPicture.SetColorMatrix(cmPicture);

            // Set the Graphics object from the bitmap
            gfxPicture = Graphics.FromImage(bmpPicture);

            // New rectangle for the picture, same size as the original picture
            rctPicture = new Rectangle(0, 0, pbEdit.Image.Width, pbEdit.Image.Height);

            // Draw the new image
            gfxPicture.DrawImage(pbEdit.Image, rctPicture, 0, 0, pbEdit.Image.Width, pbEdit.Image.Height, GraphicsUnit.Pixel, iaPicture);

            // Set the PictureBox to the new bitmap
            pbEdit.Image = bmpPicture;
        }

        private void pbEdit_MouseDown(object sender, MouseEventArgs e)
        {
            if (cmbSelectedAspectRatio.SelectedIndex == 0)
            {
                if (e.Button == MouseButtons.Left)
                {

                    Cursor = Cursors.Cross;

                    cropX = e.X;

                    cropY = e.Y;
                    cropPen = new Pen(Color.Black, 1);

                    cropPen.DashStyle = DashStyle.DashDotDot;
                }

                pbEdit.Refresh();
            }
        }

        private void simpleButton3_Click(object sender, EventArgs e)
        {
            pbEdit.Image = imageReset;
            if (pbEdit.Image.Height <= pbEdit.Height &&
               pbEdit.Image.Width <= pbEdit.Width)
            {
                pbEdit.SizeMode = PictureBoxSizeMode.CenterImage;
            }

            else
            {
                pbEdit.SizeMode = PictureBoxSizeMode.Zoom;
            }
            imageBrg = pbEdit.Image;
            imageFilter = pbEdit.Image;
            previewBitmap = (Bitmap)pbEdit.Image;
            tbContrast.EditValue = 0;
            tbBrightness.EditValue = 0;
            rbGrayscale.Checked = false;
            rbNegative.Checked = false;
            rbSepia.Checked = false;
            rbTransparency.Checked = false;
        }

        private void pbEdit_MouseMove(object sender, MouseEventArgs e)
        {
            if (pbEdit.Image == null)

                return;
            if (cmbSelectedAspectRatio.SelectedIndex == 0)
            {
                if (e.Button == MouseButtons.Left)
                {

                    pbEdit.Refresh();

                    cropWidth = e.X - cropX;

                    cropHeight = e.Y - cropY;
                    pbEdit.CreateGraphics().DrawRectangle(cropPen, cropX, cropY, cropWidth, cropHeight);

                }
            }

            else
            {
                if (e.Button == MouseButtons.Left)
                {
                    moveRect(e.Location);
                }
            }
        }

        private void pbEdit_MouseLeave(object sender, EventArgs e)
        {
            Cursor = Cursors.Default;
        }



        private void bbiCrop_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.Default;
            if (cmbSelectedAspectRatio.SelectedIndex > 0)
            {
                cropX = rec.X;
                cropY = rec.Y;
                cropWidth = rec.Width;
                cropHeight = rec.Height;
            }
            if (cropWidth < 1)
            {
                return;
            }
            Rectangle rect = new Rectangle(cropX, cropY, cropWidth, cropHeight);
            Bitmap OriginalImage = new Bitmap(pbEdit.Image, pbEdit.Width, pbEdit.Height);
            Bitmap _img = new Bitmap(cropWidth, cropHeight);
            Graphics g = Graphics.FromImage(_img);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.DrawImage(OriginalImage, 0, 0, rect, GraphicsUnit.Pixel);
            pbEdit.Image = _img;
            pbEdit.Width = _img.Width;
            pbEdit.Height = _img.Height;
            pbEdit.SizeMode = PictureBoxSizeMode.CenterImage;
            imageFilter = pbEdit.Image;
            imageBrg = pbEdit.Image; 
            previewBitmap = (Bitmap)pbEdit.Image;
            cropHeight = 0;
            cropWidth = 0;
        }
        
        private void OnCheckChangedEventHandler(object sender, EventArgs e)
        {
            if (pbEdit.Image != null) 
            {
              
                if (rbGrayscale.Checked == true)
                {
                    pbEdit.Image = imageFilter.DrawAsGrayscale();
                    imageBrg = pbEdit.Image;previewBitmap = (Bitmap)pbEdit.Image;
                        
                }
                
                else if (rbTransparency.Checked == true)
                {
                    pbEdit.Image = imageFilter.DrawWithTransparency();
                    imageBrg = pbEdit.Image;
                    previewBitmap = (Bitmap)pbEdit.Image;
                    
                }
                
                else if (rbNegative.Checked == true)
                {
                    pbEdit.Image = imageFilter.DrawAsNegative();
                    imageBrg = pbEdit.Image;
                    previewBitmap = (Bitmap)pbEdit.Image;
                   
                }
               
                else if (rbSepia.Checked == true)
                {
                    pbEdit.Image = imageFilter.DrawAsSepiaTone();
                    imageBrg = pbEdit.Image;
                    previewBitmap = (Bitmap)pbEdit.Image;
                    
                }
               
              
               
            }
        }

        public static Bitmap AdjustBrightness(Bitmap Image, int Value)
        {
            Bitmap TempBitmap = Image;
            float FinalValue = (float)Value / 255.0f;
            Bitmap NewBitmap = new Bitmap(TempBitmap.Width, TempBitmap.Height);
            Graphics NewGraphics = Graphics.FromImage(NewBitmap);
            float[][] FloatColorMatrix ={
                    new float[] {1, 0, 0, 0, 0},
                    new float[] {0, 1, 0, 0, 0},
                    new float[] {0, 0, 1, 0, 0},
                    new float[] {0, 0, 0, 1, 0},
                    new float[] {FinalValue, FinalValue, FinalValue, 1, 1}
                };

            ColorMatrix NewColorMatrix = new ColorMatrix(FloatColorMatrix);
            ImageAttributes Attributes = new ImageAttributes();
            Attributes.SetColorMatrix(NewColorMatrix);
            NewGraphics.DrawImage(TempBitmap, new Rectangle(0, 0, TempBitmap.Width, TempBitmap.Height), 0, 0, TempBitmap.Width, TempBitmap.Height, GraphicsUnit.Pixel, Attributes);
            Attributes.Dispose();
            NewGraphics.Dispose();
            return NewBitmap;
        }
        private Rectangle rec;
        private void DrawRectangle()
        {
            if (cmbSelectedAspectRatio.SelectedIndex > 0)
            {

                pbEdit.Refresh();
                int width = widthhhhh[cmbSelectedCropBoxSize.SelectedIndex];
                double aspect = ratios[cmbSelectedAspectRatio.SelectedIndex - 1];
                double height = width / aspect;
                rec = new Rectangle(0, 0, width, (int)height);
                pbEdit.CreateGraphics().DrawRectangle(Pens.Black, rec);

            }
        }

        private void moveRect(Point pt)
        {
            rec.X = pt.X;
            rec.Y = pt.Y;
            if (rec.Right > pbEdit.Width)
            {
                rec.X = pbEdit.Width - rec.Width;
            }
            if (rec.Top < 0)
            {
                rec.Y = 0;
            }
            if (rec.Left < 0)
            {
                rec.X = 0;
            }
            if (rec.Bottom > pbEdit.Height)
            {
                rec.Y = pbEdit.Height - rec.Height;
            }
            pbEdit.Refresh();
            int width = widthhhhh[cmbSelectedCropBoxSize.SelectedIndex];
            double aspect = ratios[cmbSelectedAspectRatio.SelectedIndex - 1];
            double height = width / aspect;
            rec = new Rectangle(rec.X, rec.Y, width, (int)height);
            pbEdit.CreateGraphics().DrawRectangle(Pens.Black, rec);
        }

        private void cmbSelectedAspectRatio_SelectedIndexChanged(object sender, EventArgs e)
        {
            DrawRectangle();
        }

        private void cmbSelectedCropBoxSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            DrawRectangle();
        }
    }
}
