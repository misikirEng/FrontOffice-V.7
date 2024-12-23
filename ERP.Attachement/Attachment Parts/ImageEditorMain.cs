using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using DevExpress.UserSkins;
using DevExpress.Skins;
using DevExpress.LookAndFeel;

namespace CNETImageEditor
{
    public partial class CNET_Image_Editor: UserControl
    {
        public CNET_Image_Editor()
        {
            Application.EnableVisualStyles();
            BonusSkins.Register();
            SkinManager.EnableFormSkins();
            UserLookAndFeel.Default.SetSkinStyle("Metropolis");
            
            
            InitializeComponent();
            forminatalize();
        }
        #region imageeditor

        // image processing stuff
        Bitmap bmpPicture;
        System.Drawing.Imaging.ImageAttributes iaPicture;
        System.Drawing.Imaging.ColorMatrix cmPicture;
        Graphics gfxPicture;
        Rectangle rctPicture;
        float brightness;
        float contrast;

        // cropping view stuff
        Rectangle CropRect;
        Rectangle rcLT, rcRT, rcLB, rcRB;
        Rectangle picz;
        Rectangle rcOld, rcNew;
        Rectangle rcOriginal;
        Rectangle rcBegin;
        SolidBrush BrushRect;
        HatchBrush BrushRectSmall;
        //   SolidBrush Brushpicturebox;
        Color BrushColor;

        int AlphaBlend;
        int nSize;
        int nWd;
        int nHt;
        int nResizeRT;
        int nResizeBL;
        int nResizeLT;
        int nResizeRB;
        int nThatsIt;
        int nCropRect;
        int CropWidth;

        int imageWidth;
        int imageHeight;
        int HeightOffset;

        double CropAspectRatio;
        double ImageAspectRatio;
        double ZoomedRatio;

        Point ptOld;
        Point ptNew;

        string imageStats;
        string filename;



        List<double> ratios;

        public void forminatalize()
        {
            // double buffer
            this.SetStyle(
                  ControlStyles.AllPaintingInWmPaint |
                  ControlStyles.UserPaint |
                  ControlStyles.DoubleBuffer, true);

            // build list of crop ratios
            ratios = new List<double>();
            sbSelectedAspectRatio.Items.Add("4:3  (1.3)  Normal Display");
            ratios.Add(4.0 / 3.0);
            sbSelectedAspectRatio.Items.Add("16:9 (1.78) High Definition");
            ratios.Add(16.0 / 9.0);
            sbSelectedAspectRatio.Items.Add("3:2 (1.5) Digital Camera");
            ratios.Add(3.0 / 2.0);
            sbSelectedAspectRatio.Items.Add("1:1 (1.0) Square");
            ratios.Add(1.0);

            sbSelectedAspectRatio.SelectedIndex = 0;

            // build list of common sizes
            cmbSelectedCropBoxSize.Items.Add("320");
            cmbSelectedCropBoxSize.Items.Add("640");
            cmbSelectedCropBoxSize.Items.Add("800");
            cmbSelectedCropBoxSize.Items.Add("1024");

            cmbSelectedCropBoxSize.SelectedIndex = 0;
            CropWidth = Convert.ToInt16(cmbSelectedCropBoxSize.Text);

            numUpDnBrightnessImage_ValueChanged(null, null);
            numUpDnContrastImage_ValueChanged(null, null);

            // offset to make width & height proportional to image
            //HeightOffset = panel1.Height + statusStrip1.Height +
            //                SystemInformation.CaptionHeight + (SystemInformation.BorderSize.Height * 2);

            // do initializations
            UpdateAspectRatio();
            InitializeCropRectangle();
        }


       private void InitializeCropRectangle()
        {
            AlphaBlend = 48;

            nSize = 8;
            nWd = CropWidth = Convert.ToInt16(cmbSelectedCropBoxSize.Text);
            nHt = 1;

            nThatsIt = 0;
            nResizeRT = 0;
            nResizeBL = 0;
            nResizeLT = 0;
            nResizeRB = 0;

            //   CropAspectRatio = ratios[cmbSelectedAspectRatio.SelectedIndex];

            BrushColor = Color.Red;
            BrushRect = new SolidBrush(Color.FromArgb(AlphaBlend, BrushColor.R, BrushColor.G, BrushColor.B));

            // BrushColor = Color.White;
            // Brushpicturebox = new SolidBrush(Color.FromArgb(AlphaBlend, BrushColor.R, BrushColor.G, BrushColor.B));


            BrushColor = Color.Red;
            BrushRectSmall = new HatchBrush(HatchStyle.Percent50, Color.FromArgb(192, BrushColor.R, BrushColor.G, BrushColor.B));



            ptOld = new Point(0, 0);
            rcBegin = new Rectangle();
            rcOriginal = new Rectangle(0, 0, 0, 0);
            picz = new Rectangle(0, 0, pbEdit.Width, pbEdit.Height);
            rcLT = new Rectangle(0, 0, nSize, nSize);
            rcRT = new Rectangle(0, 0, nSize, nSize);
            rcLB = new Rectangle(0, 0, nSize, nSize);
            rcRB = new Rectangle(0, 0, nSize, nSize);
            rcOld = CropRect = new Rectangle(0, 0, nWd, nHt);

            AdjustResizeRects();
        }


        private void LoadImage(string file)
        {
            Cursor = Cursors.AppStarting;
            if (file == null)
                return;
            pbEdit.Image = Image.FromFile(file);

            imageWidth = pbEdit.Image.Width;
            imageHeight = pbEdit.Image.Height;

            imageStats = String.Format("{0} | {1}x{2} | Aspect {3:0.0}",
                System.IO.Path.GetFileName(file), imageWidth, imageHeight,
                (double)((double)imageWidth / (double)imageHeight)
                );


            // logic for portrait mode ???
            if (imageWidth > imageHeight)
            {
                ImageAspectRatio = (double)imageWidth / (double)imageHeight;
                this.Width = 800 + (SystemInformation.BorderSize.Width * 2);
                this.Height = (int)((this.Width / ImageAspectRatio)) + HeightOffset;
            }
            else
            {
                ImageAspectRatio = (double)imageHeight / (double)imageWidth;
                this.Height = 800;
                this.Width = (int)((this.Height / ImageAspectRatio)) + HeightOffset;
            }

            //  sbCenter_Click(null, null);
            //  Form1_ResizeEnd(null, null);
            Cursor = Cursors.Default;
            cropcenter();
        }
        public void LoadImage(Image image)
        {
            Cursor = Cursors.AppStarting;

            pbEdit.Image = image;

            imageWidth = image.Width;
            imageHeight = image.Height;

            imageStats = String.Format("{0} | {1}x{2} | Aspect {3:0.0}",
                "image.jpg", imageWidth, imageHeight,
                (double)((double)imageWidth / (double)imageHeight)
                );


            // logic for portrait mode ???
            if (imageWidth > imageHeight)
            {
                ImageAspectRatio = (double)imageWidth / (double)imageHeight;
                this.Width = 800 + (SystemInformation.BorderSize.Width * 2);
                this.Height = (int)((this.Width / ImageAspectRatio)) + HeightOffset;
            }
            else
            {
                ImageAspectRatio = (double)imageHeight / (double)imageWidth;
                this.Height = 800;
                this.Width = (int)((this.Height / ImageAspectRatio)) + HeightOffset;
            }

            //  sbCenter_Click(null, null);
            //  Form1_ResizeEnd(null, null);
            Cursor = Cursors.Default;
            cropcenter();
        }
        public void rotate()
        {
            Cursor = Cursors.AppStarting;
            pbEdit.Image.RotateFlip(RotateFlipType.Rotate90FlipNone);
            pbEdit.Refresh();
            Cursor = Cursors.Default;
        }
        public void cropcenter()
        {
            UpdateAspectRatio();

            CropRect.X = (pbEdit.ClientRectangle.Width - CropRect.Width) / 2;
            CropRect.Y = (pbEdit.ClientRectangle.Height - CropRect.Height) / 2;

            AdjustResizeRects();
            pbEdit.Refresh();
        }
        public void AdjustResizeRects()
        {
            rcLT.X = CropRect.Left;
            rcLT.Y = CropRect.Top;

            rcRT.X = CropRect.Right;
            // rcRT.X = CropRect.Right - rcRT.Width;
            rcRT.Y = CropRect.Top;

            rcLB.X = CropRect.Left;
            // rcLB.Y = CropRect.Bottom - rcLB.Height;
            rcLB.Y = CropRect.Bottom;

            rcRB.X = CropRect.Right;
            //rcRB.X = CropRect.Right - rcRB.Width;
            // rcRB.Y = CropRect.Bottom - rcRB.Height;
            rcRB.Y = CropRect.Bottom;
        }
        public void pictureboxpaint(PaintEventArgs e)
        {
            if (pbEdit.Image == null)
            {
                // display checkerboard
                bool xGrayBox = true;
                int backgroundX = 0;
                while (backgroundX < pbEdit.Width)
                {
                    int backgroundY = 0;
                    bool yGrayBox = xGrayBox;
                    while (backgroundY < pbEdit.Height)
                    {
                        int recWidth = (int)((backgroundX + 50 > pbEdit.Width) ? pbEdit.Width - backgroundX : 50);
                        int recHeight = (int)((backgroundY + 50 > pbEdit.Height) ? pbEdit.Height - backgroundY : 50);
                        e.Graphics.FillRectangle(((Brush)(yGrayBox ? Brushes.LightGray : Brushes.Gainsboro)), backgroundX, backgroundY, recWidth + 2, recHeight + 2);
                        backgroundY += 50;
                        yGrayBox = !yGrayBox;
                    }
                    backgroundX += 50;
                    xGrayBox = !xGrayBox;
                }
            }
            else
            {
                //Rectangle aa=new Rectangle(picz-CropRect;);
                // main crop box 
                e.Graphics.FillRectangle((BrushRect), CropRect);
                //  e.Graphics.FillRectangle((Brushpicturebox), picz);

                // corner drag boxes
                e.Graphics.FillRectangle((BrushRectSmall), rcLT);
                e.Graphics.FillRectangle((BrushRectSmall), rcRT);
                e.Graphics.FillRectangle((BrushRectSmall), rcLB);
                e.Graphics.FillRectangle((BrushRectSmall), rcRB);

                AdjustResizeRects();

            }
            base.OnPaint(e);
        }

        public void pictureboxmousemove(MouseEventArgs e)
        {
            if (pbEdit.Image == null)
                return;
            #region
            Point pt = new Point(e.X, e.Y);

            if (rcLT.Contains(pt))
                Cursor = Cursors.SizeNWSE;
            else
                if (rcRT.Contains(pt))
                    Cursor = Cursors.SizeNESW;
                else
                    if (rcLB.Contains(pt))
                        Cursor = Cursors.SizeNESW;
                    else
                        if (rcRB.Contains(pt))
                            Cursor = Cursors.SizeNWSE;
                        else
                            if (CropRect.Contains(pt))
                                Cursor = Cursors.SizeAll;
                            else
                                Cursor = Cursors.Default;


            if (e.Button == MouseButtons.Left)
            {
                if (nResizeRB == 1)
                {
                    rcNew.X = CropRect.X;
                    rcNew.Y = CropRect.Y;
                    rcNew.Width = pt.X - rcNew.Left;
                    rcNew.Height = pt.Y - rcNew.Top;

                    if (rcNew.X > rcNew.Right)
                    {
                        rcNew.Offset(-nWd, 0);
                        if (rcNew.X < 0)
                            rcNew.X = 0;
                    }
                    if (rcNew.Y > rcNew.Bottom)
                    {
                        rcNew.Offset(0, -nHt);
                        if (rcNew.Y < 0)
                            rcNew.Y = 0;
                    }

                    DrawDragRect(e);
                    rcOld = CropRect = rcNew;
                    Cursor = Cursors.SizeNWSE;
                }
                else
                    if (nResizeBL == 1)
                    {
                        rcNew.X = pt.X;
                        rcNew.Y = CropRect.Y;
                        rcNew.Width = CropRect.Right - pt.X;
                        rcNew.Height = pt.Y - rcNew.Top;

                        if (rcNew.X > rcNew.Right)
                        {
                            rcNew.Offset(nWd, 0);
                            if (rcNew.Right > ClientRectangle.Width)
                                rcNew.Width = ClientRectangle.Width - rcNew.X;
                        }
                        if (rcNew.Y > rcNew.Bottom)
                        {
                            rcNew.Offset(0, -nHt);
                            if (rcNew.Y < 0)
                                rcNew.Y = 0;
                        }

                        DrawDragRect(e);
                        rcOld = CropRect = rcNew;
                        Cursor = Cursors.SizeNESW;
                    }
                    else
                        if (nResizeRT == 1)
                        {
                            rcNew.X = CropRect.X;
                            rcNew.Y = pt.Y;
                            rcNew.Width = pt.X - rcNew.Left;
                            rcNew.Height = CropRect.Bottom - pt.Y;

                            if (rcNew.X > rcNew.Right)
                            {
                                rcNew.Offset(-nWd, 0);
                                if (rcNew.X < 0)
                                    rcNew.X = 0;
                            }
                            if (rcNew.Y > rcNew.Bottom)
                            {
                                rcNew.Offset(0, nHt);
                                if (rcNew.Bottom > ClientRectangle.Height)
                                    rcNew.Y = ClientRectangle.Height - rcNew.Height;
                            }

                            DrawDragRect(e);
                            rcOld = CropRect = rcNew;
                            Cursor = Cursors.SizeNESW;
                        }
                        else
                            if (nResizeLT == 1)
                            {
                                rcNew.X = pt.X;
                                rcNew.Y = pt.Y;
                                rcNew.Width = CropRect.Right - pt.X;
                                rcNew.Height = CropRect.Bottom - pt.Y;

                                if (rcNew.X > rcNew.Right)
                                {
                                    rcNew.Offset(nWd, 0);
                                    if (rcNew.Right > ClientRectangle.Width)
                                        rcNew.Width = ClientRectangle.Width - rcNew.X;
                                }
                                if (rcNew.Y > rcNew.Bottom)
                                {
                                    rcNew.Offset(0, nHt);
                                    if (rcNew.Bottom > ClientRectangle.Height)
                                        rcNew.Height = ClientRectangle.Height - rcNew.Y;
                                }

                                DrawDragRect(e);
                                rcOld = CropRect = rcNew;
                                Cursor = Cursors.SizeNWSE;
                            }
                            else
                                if (nCropRect == 1) //Moving the rectangle
                                {
                                    ptNew = pt;
                                    int dx = ptNew.X - ptOld.X;
                                    int dy = ptNew.Y - ptOld.Y;
                                    CropRect.Offset(dx, dy);
                                    rcNew = CropRect;
                                    DrawDragRect(e);
                                    ptOld = ptNew;
                                }

                AdjustResizeRects();
                DisplayLocation();
                pbEdit.Update();
            }

            base.OnMouseMove(e);
            #endregion

        }

        private void DrawDragRect(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                AdjustResizeRects();
                pbEdit.Invalidate();
            }
        }
        public void pictureboxmouseup(MouseEventArgs e)
        {
            if (nThatsIt == 0)
                return;

            nCropRect = 0;
            nResizeRB = 0;
            nResizeBL = 0;
            nResizeRT = 0;
            nResizeLT = 0;

            if (CropRect.Width <= 0 || CropRect.Height <= 0)
                CropRect = rcOriginal;

            if (CropRect.Right > ClientRectangle.Width)
                CropRect.Width = ClientRectangle.Width - CropRect.X;

            if (CropRect.Bottom > ClientRectangle.Height)
                CropRect.Height = ClientRectangle.Height - CropRect.Y;

            if (CropRect.X < 0)
                CropRect.X = 0;

            if (CropRect.Y < 0)
                CropRect.Y = 0;

            // need to add logic for portrait mode of crop box in this
            // area

            // now that the crop box position is established
            // force it to the proper aspect ratio
            // and scale it
            /*
                        if (CropRect.Width > CropRect.Height)
                        {
                            CropRect.Height = (int)(CropRect.Width / CropAspectRatio);
                        }
                        else
                        {
                            CropRect.Width = (int)(CropRect.Height * CropAspectRatio);
                        }
                        */
            AdjustResizeRects();
            pbEdit.Refresh();

            base.OnMouseUp(e);

            nWd = rcNew.Width;
            nHt = rcNew.Height;
            rcBegin = rcNew;

            DisplayLocation();
        }
        public void pictureboxmousedown(MouseEventArgs e)
        {
            Point pt = new Point(e.X, e.Y);
            rcOriginal = CropRect;
            rcBegin = CropRect;

            if (rcRB.Contains(pt))
            {
                rcOld = new Rectangle(CropRect.X, CropRect.Y, CropRect.Width, CropRect.Height);
                rcNew = rcOld;
                nResizeRB = 1;
            }
            else
                if (rcLB.Contains(pt))
                {
                    rcOld = new Rectangle(CropRect.X, CropRect.Y, CropRect.Width, CropRect.Height);
                    rcNew = rcOld;
                    nResizeBL = 1;
                }
                else
                    if (rcRT.Contains(pt))
                    {
                        rcOld = new Rectangle(CropRect.X, CropRect.Y, CropRect.Width, CropRect.Height);
                        rcNew = rcOld;
                        nResizeRT = 1;
                    }
                    else
                        if (rcLT.Contains(pt))
                        {
                            rcOld = new Rectangle(CropRect.X, CropRect.Y, CropRect.Width, CropRect.Height);
                            rcNew = rcOld;
                            nResizeLT = 1;
                        }
                        else
                            if (CropRect.Contains(pt))
                            {
                                nResizeBL = nResizeLT = nResizeRB = nResizeRT = 0;
                                nCropRect = 1;
                                ptNew = ptOld = pt;
                            }
            nThatsIt = 1;
            base.OnMouseDown(e);
        }


        private void pictureBox1_MouseLeave(object sender, EventArgs e)
        {
            Cursor = Cursors.Default;
        }

        private void cmbSelectedAspectRatio_SelectionChangeCommitted(object sender, EventArgs e)
        {
            UpdateAspectRatio();
        }

        private void cmbSelectedCropBoxSize_SelectionChangeCommitted(object sender, EventArgs e)
        {
            CropWidth = Convert.ToInt16(cmbSelectedCropBoxSize.Text);

            CropRect.X = 0;
            CropRect.Y = 0;

            UpdateAspectRatio();
        }

        private void Form1_ResizeEnd(object sender, EventArgs e)
        {
            if (ImageAspectRatio == 0)
                return;

            this.Height = (int)((this.Width / ImageAspectRatio)) + HeightOffset;

            // logic for portrait mode goes here
            // for form resize

            UpdateAspectRatio();
            this.Refresh();
        }


        private void btnInvertColors_Click(object sender, EventArgs e)
        {
            inverting();
        }


        private void btnGrayScaleImage_Click(object sender, EventArgs e)
        {
            GrayScaling();
        }



        private void btnBrightnessImage_Click(object sender, EventArgs e)
        {
            brightnesss();
        }


        private void btnContrastImage_Click(object sender, EventArgs e)
        {
            contrasts();
        }



        private void btnReset_Click(object sender, EventArgs e)
        {
            LoadImage(filename);
        }

        private void numUpDnBrightnessImage_ValueChanged(object sender, EventArgs e)
        {
            //“brightness” ranges from -1 to +1
            // numeric up down is 0 to 100
            // brightness = (float)(Convert.ToDouble(numUpDnBrightnessImage.Value) - 50.0) / 50.0f;
        }

        private void numUpDnContrastImage_ValueChanged(object sender, EventArgs e)
        {
            //  contrast = (float)(Convert.ToDouble(numUpDnContrastImage.Value));
        }


        private void PreparePicture()
        {
            // If there's a picture
            if (pbEdit.Image != null)
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


        public void inverting()
        {
            Cursor = Cursors.AppStarting;

            PreparePicture();
            cmPicture = new System.Drawing.Imaging.ColorMatrix(new float[][]
            {
                new float[] {-1, 0, 0, 0, 0},
                new float[] {0, -1, 0, 0, 0},
                new float[] {0, 0, -1, 0, 0},
                new float[] {0, 0, 0, 1, 0},
                new float[] {1, 1, 1, 0, 1}
            });
            FinalizePicture();

            Cursor = Cursors.Default;
        }
        public void GrayScaling()
        {
            Cursor = Cursors.AppStarting;

            PreparePicture();
            cmPicture = new System.Drawing.Imaging.ColorMatrix(new float[][]
            {
                new float[] {0.30f, 0.30f, 0.30f, 0, 0},
                new float[] {0.59f, 0.59f, 0.59f, 0, 0},
                new float[] {0.11f, 0.11f, 0.11f, 0, 0},
                new float[] {0, 0, 0, 1, 0},
                new float[] {0, 0, 0, 0, 1}
            });
            FinalizePicture();

            Cursor = Cursors.Default;
        }
        public void brightnesss()
        {
            Cursor = Cursors.AppStarting;
            float bf = brightness;

            PreparePicture();
            cmPicture = new System.Drawing.Imaging.ColorMatrix(new float[][]
            {
                new float[]{1f,    0f,    0f,    0f,    0f},
                new float[]{0f,    1f,    0f,    0f,    0f},
                new float[]{0f,    0f,    1f,    0f,    0f},
                new float[]{0f,    0f,    0f,    1f,    0f},
                new float[]{bf,    bf,    bf,    1f,    1f}
            });
            FinalizePicture();

            Cursor = Cursors.Default;
        }

        public void contrasts()
        {
            Cursor = Cursors.AppStarting;
            float cf = 0.04f * contrast;

            PreparePicture();

            cmPicture = new System.Drawing.Imaging.ColorMatrix(new float[][]
            {
                new float[]{cf,    0f,    0f,    0f,   0f},
                new float[]{0f,    cf,    0f,    0f,   0f},
                new float[]{0f,    0f,    cf,    0f,   0f},
                new float[]{0f,    0f,    0f,    1f,   0f},
                new float[]{0.001f,    0.001f,    0.001f,    0f,   1f}
            });
            FinalizePicture();

            Cursor = Cursors.Default;
        }

        private void DisplayLocation()
        {
            // assume not yet initialized
            if (pbEdit.Image == null)
                return;

            //tsLabelCropboxLocation.Text = String.Format("{0} |  Scale {1:0.00}% | Crop Area {2} x {3} | Crop X, Y {4}, {5}",
            //                    imageStats,
            //                    ZoomedRatio * 100.0,
            //                    (int)((double)CropRect.Width / ZoomedRatio),
            //                    (int)((double)CropRect.Height / ZoomedRatio),
            //                    (int)((double)CropRect.X / ZoomedRatio),
            //                    (int)((double)CropRect.Y / ZoomedRatio)
            //                );
        }

        private void UpdateAspectRatio()
        {
            int ratioIndex = sbSelectedAspectRatio.SelectedIndex;

            CropAspectRatio = ratios[ratioIndex];
            int CropHeight = (int)((CropWidth / CropAspectRatio));

            try
            {
                ZoomedRatio = pbEdit.ClientRectangle.Width / (double)imageWidth;
            }
            catch
            {
                // imageWidth is not yet established (division by zero)
                // force a value
                ZoomedRatio = 1.0;
            }

            // scale crop rect to image scale
            CropRect.Width = (int)((double)CropWidth * ZoomedRatio);
            CropRect.Height = (int)((double)CropHeight * ZoomedRatio);

            // update crop box and refresh everything
            nThatsIt = 1;
            pictureEdit1_MouseUp(null, null);

        }

        private static Image CropImage(Image img, Rectangle cropArea)
        {
            try
            {
                Bitmap bmpImage = new Bitmap(img);
                Bitmap bmpCrop = bmpImage.Clone(cropArea, bmpImage.PixelFormat);
                return (Image)(bmpCrop);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "CropImage()");
            }
            return null;
        }

        private void saveJpeg(string path, Bitmap img, long quality)
        {
            // Encoder parameter for image quality
            EncoderParameter qualityParam = new EncoderParameter(
                    System.Drawing.Imaging.Encoder.Quality, (long)quality);

            // Jpeg image codec
            ImageCodecInfo jpegCodec = getEncoderInfo("image/jpeg");

            if (jpegCodec == null)
            {
                MessageBox.Show("Can't find JPEG encoder?", "saveJpeg()");
                return;
            }
            EncoderParameters encoderParams = new EncoderParameters(1);
            encoderParams.Param[0] = qualityParam;

            img.Save(path, jpegCodec, encoderParams);
        }

        private ImageCodecInfo getEncoderInfo(string mimeType)
        {
            // Get image codecs for all image formats
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();

            // Find the correct image codec
            for (int i = 0; i < codecs.Length; i++)
                if (codecs[i].MimeType == mimeType)
                    return codecs[i];

            return null;
        }
        #endregion
       


        public event ImageEditerEventHandler SaveClicked;

        public delegate void ImageEditerEventHandler(Bitmap images,FileInfo fileInfo);

        #region Private Methods
        private void sbSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            Bitmap bmp = null;

            Rectangle ScaledCropRect = new Rectangle();
            ScaledCropRect.X = (int)(CropRect.X / ZoomedRatio);
            ScaledCropRect.Y = (int)(CropRect.Y / ZoomedRatio);
            ScaledCropRect.Width = (int)((double)(CropRect.Width) / ZoomedRatio);
            ScaledCropRect.Height = (int)((double)(CropRect.Height) / ZoomedRatio);

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    bmp = (Bitmap)CropImage(pbEdit.Image, ScaledCropRect);

                    if (SaveClicked != null)
                        SaveClicked.Invoke(bmp, new FileInfo("Edited-(" + DateTime.Now.Ticks.ToString() + ").jpg"));

                    saveJpeg(saveFileDialog1.FileName, bmp, 85);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Path Error");
                }
            }

            if (bmp != null)
                bmp.Dispose();
        }


        private void cmbSelectedCropBoxSize_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void sbCenter_Click(object sender, EventArgs e)
        {
            cropcenter();
        }

        private void sbRotateImage_Click(object sender, EventArgs e)
        {
            rotate();
        }

        private void sbInverting_Click(object sender, EventArgs e)
        {
            inverting();
        }

        private void sbGrayScaling_Click(object sender, EventArgs e)
        {
            GrayScaling();
        }

        private void pictureEdit1_Paint(object sender, PaintEventArgs e)
        {
            pictureboxpaint(e);
        }

        private void pictureEdit1_MouseMove(object sender, MouseEventArgs e)
        {
            pictureboxmousemove(e);
        }

        private void pictureEdit1_MouseDown(object sender, MouseEventArgs e)
        {
            pictureboxmousedown(e);
        }

        private void pictureEdit1_MouseUp(object sender, MouseEventArgs e)
        {
            pictureboxmouseup(e);
        }

        #endregion
        //public void SelectedAspectRatio_SelectionChangeCommitted()
        //{
        //    CropWidth = Convert.ToInt16(cmbSelectedCropBoxSize.Text);

        //    CropRect.X = 0;
        //    CropRect.Y = 0;

        //    UpdateAspectRatio();
        //}


    }
}
