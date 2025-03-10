using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows.Forms; 
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraRichEdit;  
using DevExpress.XtraPrinting.Native;
using CNETCamera;
using CNETDocumentScanner;
using WMPLib;
using AxWMPLib;
using System.Drawing.Drawing2D;
using DevExpress.XtraEditors.Controls;
using CNET_ImageEditor;
using CNET_V7_Domain.Domain.CommonSchema;
using CNET_V7_Domain.Domain.SettingSchema;
using CNET.Progress.Reporter;
using ProcessManager;
using DevExpress.Mvvm.Native;

namespace ERP.Attachement
{
    public partial class modalNewAttachment : XtraForm
    {
        private const int CP_NOCLOSE_BUTTON = 0x200;
        private AttachmentDTO attachmentObj = new AttachmentDTO();
        private byte[] byteData;
        private string extension;
        private string fileName;
        public static Image acquiredImage = null;
        public static Image imm { get; set; }
        ScannerMain scaner = null;
        ConfigurationDTO attchmentSaveValue = null;
        private bool voucherFlag = false;
        private int? attReference = null;
        public AttachmentDTO savedAttachment = null;
        private int? catagory = null;
        private List<AttachmentDTO> temporaryAttachments = new List<AttachmentDTO>();
        CameraMain cameraMain = new CameraMain();
        string TypeName { get; set; }
        int TypeId { get; set; }
        //string DefaultAttachmentPath { get; set; }

        //public bool AttachmentIsFTP { get; set; }
        public string AttachmentIsFTPTINNo { get; set; }
        public string AttachmentIsFTPOrganizationUnitDef { get; set; }
        //public bool AttachmentIsFTPRoom { get; set; }
        //public string AttachmentFTPRoomType { get; set; }

        bool CompanyFTPExist { get; set; }
        public modalNewAttachment()
        {
            InitializeComponent();
            CompanyFTPExist = FTPInterface.FTPAttachment.InitalizeFTPAttachment(LocalBuffer.LocalBuffer.CompanyConsigneeData.Tin);
            if (!CompanyFTPExist)
            {
                XtraMessageBox.Show("Company FTP Path not available !!", "FTP Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }


        public modalNewAttachment(int id, bool isVoucher, int attachmentCatagory, int type)
        {
            InitializeComponent();

            CompanyFTPExist = FTPInterface.FTPAttachment.InitalizeFTPAttachment(LocalBuffer.LocalBuffer.CompanyConsigneeData.Tin);
            if (!CompanyFTPExist)
            {
                XtraMessageBox.Show("Company FTP Path not available !!", "FTP Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            voucherFlag = isVoucher;
            attReference = id;
            catagory = attachmentCatagory;
            TypeId = type;
            SystemConstantDTO vou = LocalBuffer.LocalBuffer.SystemConstantDTOBufferList.FirstOrDefault(x => x.Id == type);
            if (vou != null)
            {
                TypeName = vou.Description;

            }
            else
            {
                //SystemConstantDTO gsltype = LocalBuffer.LocalBuffer.SystemConstantDTOBufferList.FirstOrDefault(x => x.Id == type);
                //if (gsltype != null)
                //{
                //    TypeName = gsltype.Description;
                //}
                //else
                //{
                XtraMessageBox.Show("Improper Type !", "ERP Attachment");
                TypeName = "Others\\";
                return;
                //}
            }
            //if (!IsFTP)
            //{
            //    bool checkpath = GetAttachmentPath();

            //    if (!checkpath)
            //    {
            //        return;
            //    }

            //}

        }

        //private bool GetAttachmentPath()
        //{
        //    bool good = false;
             
        //    DefaultAttachmentPath = Environment.ProcessPath + "\\" + TypeName;
        //    try
        //    {
        //        if (!Directory.Exists(DefaultAttachmentPath))
        //        {
        //            Directory.CreateDirectory(DefaultAttachmentPath);
        //        }
        //        if (!Directory.Exists(DefaultAttachmentPath))
        //        {
        //            XtraMessageBox.Show("Please Select Default Attachment Path for File Storage Device!", "ERP Attachment");
        //            return false;
        //        }
        //    }
        //    catch
        //    {
        //        XtraMessageBox.Show("Please Select Proper Default Attachment Path for File Storage Device!", "ERP Attachment");
        //        return false;
        //    }
        //    good = true;









        //    //DeviceDTO FileStorageDevice = LoginPage.Authentication.DeviceBufferList.FirstOrDefault(x => x.preference == CNETConstantes.FileServer);
        //    //if (FileStorageDevice != null)
        //    //{
        //    //    if (LoginPage.Authentication.ConfigurationBufferList == null)
        //    //    {
        //    //       Progress_Reporter.Close_Progress();
        //    //        XtraMessageBox.Show("Please Select Default Attachment Path for File Storage Device!", "ERP Attachment");
        //    //        return false;
        //    //    }
        //    //    Configuration value = LoginPage.Authentication.ConfigurationBufferList.FirstOrDefault(x => x.reference == FileStorageDevice.code && x.attribute == "Default Attachment Path");
        //    //    if (value == null)
        //    //    {
        //    //       Progress_Reporter.Close_Progress();
        //    //        XtraMessageBox.Show("Please Select Default Attachment Path for File Storage Device!", "ERP Attachment");
        //    //        return false;
        //    //    }

        //    //    DefaultAttachmentPath = value.currentValue+"\\"+TypeName;
        //    //    try
        //    //    {
        //    //        if (!Directory.Exists(DefaultAttachmentPath))
        //    //        {
        //    //            Directory.CreateDirectory(DefaultAttachmentPath);
        //    //        }
        //    //        if (!Directory.Exists(DefaultAttachmentPath))
        //    //        {
        //    //            XtraMessageBox.Show("Please Select Default Attachment Path for File Storage Device!", "ERP Attachment");
        //    //            return false;
        //    //        }
        //    //    }
        //    //    catch
        //    //    {
        //    //        XtraMessageBox.Show("Please Select Proper Default Attachment Path for File Storage Device!", "ERP Attachment");
        //    //        return false;
        //    //    }
        //    //    good = true;
        //    //}
        //    //else
        //    //{
        //    //    DefaultAttachmentPath = "";
        //    //   Progress_Reporter.Close_Progress();
        //    //    XtraMessageBox.Show("Please Maintain File Storage Device and Set Default Attachment Path !", "ERP Attachment");
        //    //    return false;
        //    //}
        //    return good;
        //}

        /// <summary>
        /// FOR VOUCHER ATTACHMENT
        /// </summary>
        /// <param name="code"></param>
        /// <param name="isVoucher"></param>
        public modalNewAttachment(int? id, bool isVoucher, int attachmentCatagory, List<AttachmentDTO> previousAttachment, int type, bool IsFTP = true)
        {
            InitializeComponent(); 
            
            CompanyFTPExist = FTPInterface.FTPAttachment.InitalizeFTPAttachment(LocalBuffer.LocalBuffer.CompanyConsigneeData.Tin);
            if (!CompanyFTPExist)
            {
                XtraMessageBox.Show("Company FTP Path not available !!", "FTP Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            voucherFlag = isVoucher;
            attReference = id;
            catagory = attachmentCatagory;
            temporaryAttachments = previousAttachment;

            SystemConstantDTO vou = LocalBuffer.LocalBuffer.SystemConstantDTOBufferList.FirstOrDefault(x => x.Id == type);
            if (vou != null)
            {
                TypeName = vou.Description;

            }
            else
            {
                //GSLTypeList gsltype = LoginPage.Authentication.GSLTypeListBufferList.FirstOrDefault(x => x.code == type);
                //if (gsltype != null)
                //{
                //    TypeName = gsltype.description;
                //}
                //else
                //{
                   // XtraMessageBox.Show("Improper Voucher Definition or GSL Type !", "ERP Attachment");
                    TypeName = "Others\\";
                    return;
                //}
            }
            //if (!IsFTP)
            //{
            //    bool checkpath = GetAttachmentPath();

            //    if (!checkpath)
            //    {
            //        return;
            //    }

            //}
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

        private void modalNewAttachment_Load(object sender, EventArgs e)
        {
            pictureEdit1.Visible = false;
            richEditControl1.Visible = false;
            pdfViewer1.Visible = false;
            spreadsheetControl1.Visible = false;
            axWindowsMediaPlayer1.Visible = false;
            if (LocalBuffer.LocalBuffer.ConfigurationBufferList == null)
                return;
           /*List<Configuration> value = LoginPage.Authentication.ConfigurationBufferList.Where(x => x.reference == CNETConstantes.CNET_ERP2016).ToList();
            if (value == null || value.Count == 0)
            {
                return;
            }
            attchmentSaveValue = value.Where(x => x.attribute == "Save Attachment").FirstOrDefault();
            if (attchmentSaveValue != null)
            {
                if (attchmentSaveValue.currentValue == "URLOnly")
                {
                    ckUrlOnly.Checked = true;
                }
                else if (attchmentSaveValue.currentValue == "DB")
                {
                    ckUrlOnly.Checked = false;
                }
            }*/

        }

        private void bbiClose_ItemClick(object sender, ItemClickEventArgs e)
        {
            cameraMain.closeOpenCamera();
            Hide();
        }

        private void bbiBrowse_ItemClick(object sender, ItemClickEventArgs e)
        {
            pcPreview.Controls.Clear();
            bbiCapture.Visible = false;
            bbiConfig.Visible = false;
            bbiScanDocumnet.Visible = false;
            cbDevices.Visible = false;
            cbScannerDevices.Visible = false;
            txtUrl.Visible =true; 
            pcPreview.Controls.Add(pictureEdit1);
            pcPreview.Controls.Add(richEditControl1);
            pcPreview.Controls.Add(pdfViewer1);
            pcPreview.Controls.Add(spreadsheetControl1);
            pcPreview.Controls.Add(axWindowsMediaPlayer1);
            pictureEdit1.Visible = false;
            richEditControl1.Visible = false;
            pdfViewer1.Visible = false;
            spreadsheetControl1.Visible = false;
            axWindowsMediaPlayer1.Visible = false;

            axWindowsMediaPlayer1.close();
            groupControl1.Text = "Preview";
            var dialoge = new OpenFileDialog();
            dialoge.Title = "Select Attachment";
            dialoge.ShowDialog();
            fileName = dialoge.FileName;
            txtUrl.Text = fileName;
            extension = Path.GetExtension(fileName).ToLower();

            try
            {
                if (extension == ".jpg" || extension == ".png" || extension == ".jpeg" || extension == ".bmp" || extension == ".gif" ||
                    extension == ".wmf" || extension == ".tif" || extension == ".tiff")
                {

                    pictureEdit1.Visible = true;
                    richEditControl1.Visible = false;
                    pdfViewer1.Visible = false;
                    spreadsheetControl1.Visible = false;
                    axWindowsMediaPlayer1.Visible = false;
                    pictureEdit1.Image = Image.FromFile(fileName);
                    if (pictureEdit1.Image.Height <= pictureEdit1.Height &&
                         pictureEdit1.Image.Width <= pictureEdit1.Width)
                        pictureEdit1.Properties.SizeMode = PictureSizeMode.Clip;
                    else
                        pictureEdit1.Properties.SizeMode = PictureSizeMode.Zoom;


                    imageEdit.pbEdit.Image = pictureEdit1.Image;
                    if (imageEdit.pbEdit.Image.Height <= imageEdit.pbEdit.Height &&
                        imageEdit.pbEdit.Image.Width <= imageEdit.pbEdit.Width)
                    {
                        imageEdit.pbEdit.SizeMode = PictureBoxSizeMode.CenterImage;
                    }

                    else
                    {
                        imageEdit.pbEdit.SizeMode = PictureBoxSizeMode.Zoom;
                    }

                    bbiEditor.Enabled = true;
                }
                else if (extension == ".txt" || extension == ".docx" ||
                         extension == ".doc")
                {
                    pictureEdit1.Visible = false;
                    richEditControl1.Visible = true;
                    pdfViewer1.Visible = false;
                    spreadsheetControl1.Visible = false;
                    axWindowsMediaPlayer1.Visible = false;
                    if (extension == ".txt")
                        richEditControl1.LoadDocument(fileName, DocumentFormat.PlainText);

                    else if (extension == ".docx" || extension == ".ppt")
                        richEditControl1.LoadDocument(fileName, DocumentFormat.OpenXml);

                    else if (extension == ".doc")
                        richEditControl1.LoadDocument(fileName, DocumentFormat.Doc);

                }
                else if (extension == ".pdf")
                {
                    pictureEdit1.Visible = false;
                    richEditControl1.Visible = false;
                    pdfViewer1.Visible = true;
                    spreadsheetControl1.Visible = false;
                    axWindowsMediaPlayer1.Visible = false;
                    pdfViewer1.LoadDocument(fileName);


                }
                else if (extension == ".xls" || extension == ".xlsx" || extension == ".csv")
                {
                    pictureEdit1.Visible = false;
                    richEditControl1.Visible = false;
                    pdfViewer1.Visible = false;
                    spreadsheetControl1.Visible = true;
                    axWindowsMediaPlayer1.Visible = false;
                    if (extension == ".xls")
                        spreadsheetControl1.LoadDocument(fileName, DevExpress.Spreadsheet.DocumentFormat.Xls);

                    else if (extension == ".xlsx")
                        spreadsheetControl1.LoadDocument(fileName, DevExpress.Spreadsheet.DocumentFormat.OpenXml);

                    else if (extension == ".csv")
                        spreadsheetControl1.LoadDocument(fileName, DevExpress.Spreadsheet.DocumentFormat.Csv);

                }
                else if (extension == ".mp4" || extension == ".avi" || extension == ".mpg" ||
                         extension == ".mpeg" || extension == ".mp3" || extension == ".3gp" || extension == ".wav" ||
                         extension == ".mov" || extension == ".wmv" || extension == ".m4a" || extension == ".wma" ||
                         extension == ".cda" || extension == ".mp2")
                {
                    pictureEdit1.Visible = false;
                    richEditControl1.Visible = false;
                    pdfViewer1.Visible = false;
                    spreadsheetControl1.Visible = false;
                    axWindowsMediaPlayer1.Visible = true;
                    axWindowsMediaPlayer1.stretchToFit = true;
                    axWindowsMediaPlayer1.URL = fileName;
                }
                else
                {
                    pictureEdit1.Visible = true;
                    richEditControl1.Visible = false;
                    pdfViewer1.Visible = false;
                    spreadsheetControl1.Visible = false;
                    axWindowsMediaPlayer1.Visible = false;
                    //pictureEdit1.Image = ERP.Attachement.Properties.Resources.nopreview;
                }

            }
            catch (Exception ex)
            {

                XtraMessageBox.Show(ex.Message, "ERP Attachment");
            }
        }

        public byte[] imageToByteArray(Image imageIn)
        {
            using (var ms = new MemoryStream())
            {
                imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
                return ms.ToArray();
            }
        }
        public void bbiSave_ItemClick(object sender, ItemClickEventArgs e)
        {
            //if (voucherFlag)
            //{
            //    if (string.IsNullOrEmpty(txtDescription.Text.ToString()))
            //    {
            //        Progress_Reporter.Close_Progress();
            //        XtraMessageBox.Show("Please Enter Description", "ERP Attachment");
            //        return;
            //    }
            //    savedAttachment = saveAndReturnAttachment(attReference.Value);
            //    if (savedAttachment != null)
            //        this.Close();
            //}
            //else
            //{
            if (ERP_Attachments.reference == null)
            {
                Progress_Reporter.Close_Progress();
                XtraMessageBox.Show("Select Reference!", "ERP Attachment");
                return;
            }
            Progress_Reporter.Show_Progress("Saving Attachment", "Please Wait..");
            axWindowsMediaPlayer1.close();
            if (string.IsNullOrEmpty(fileName))
            {
                if (LocalBuffer.LocalBuffer.ConfigurationBufferList == null)
                {
                    Progress_Reporter.Close_Progress();
                    XtraMessageBox.Show("Please Select Default Attachment Path!", "ERP Attachment");
                    return;
                }


                if (acquiredImage != null)
                {
                    string ftpurl = FTPInterface.FTPAttachment.SendConsigneeImageAttachement(TypeId, attReference.ToString(), acquiredImage);
                    if (acquiredImage.Tag == "Camera")
                    {
                        // string savePath = pathToSave + @"\Snapshoot " + DateTime.Now.ToString("yyyyMMddHHmmss") +".png";
                        // acquiredImage.Save(savePath, ImageFormat.Png);
                        //string ftpurl =   FTPInterface.FTPAttachment.SendConsigneeImageAttachement(TypeId, attReference.ToString(), acquiredImage);


                        attachmentObj.Url = ftpurl;
                        attachmentObj.Remark = ".png";
                        attachmentObj.Type = CNETConstantes.ATTACHMENT_TYPE_PICTURE;
                    }
                    if (acquiredImage.Tag == "Scanner")
                    {
                        //string savePath = pathToSave + @"\Scanned Image " + DateTime.Now.ToString("yyyyMMddHHmmss") +".png";
                        //acquiredImage.Save(savePath, ImageFormat.Png);
                        attachmentObj.Url = ftpurl;
                        attachmentObj.Remark = ".png";
                        attachmentObj.Type = CNETConstantes.ATTACHMENT_TYPE_PICTURE;
                    }
                    if (acquiredImage.Tag == "Edited")
                    {
                        //string savePath = pathToSave + @"\Edited Image " + DateTime.Now.ToString("yyyyMMddHHmmss") +".png";
                        //acquiredImage.Save(savePath, ImageFormat.Png);
                        attachmentObj.Url = ftpurl;
                        attachmentObj.Remark = ".png";
                        attachmentObj.Type = CNETConstantes.ATTACHMENT_TYPE_PICTURE;
                    }
                }
                else
                {
                    Progress_Reporter.Close_Progress();
                    XtraMessageBox.Show("No File To Save", "ERP Attachment");
                    return;
                }

                if (string.IsNullOrEmpty(txtDescription.Text.ToString()))
                {
                    Progress_Reporter.Close_Progress();
                    XtraMessageBox.Show("Please Enter Description", "ERP Attachment");
                    return;
                }
                if (txtDescription.Text == "logo" && acquiredImage != null)
                {

                    string ftpurl = "";
                    if (!voucherFlag)
                        ftpurl = FTPInterface.FTPAttachment.SendGSlFileAttachement(TypeId, fileName, attReference.Value);
                    else
                        ftpurl = FTPInterface.FTPAttachment.SendTransactionAttachement(TypeId, fileName, attReference.Value);


                    //byteData = ImagetoByte(acquiredImage);
                    //attachmentObj.file = byteData;
                    // string ftpurl = FTPInterface.FTPAttachment.SendConsigneeImageAttachement(TypeId, attReference.ToString(), acquiredImage);
                    //string pathToSave = DefaultAttachmentPath;
                    //string savePath = pathToSave + @"\Edited Image " + DateTime.Now.ToString("yyyyMMddHHmmss") +".png";
                    //acquiredImage.Save(savePath, ImageFormat.Png);
                    attachmentObj.Url = ftpurl;
                    attachmentObj.Remark = ".png";
                    attachmentObj.Type = CNETConstantes.ATTACHMENT_TYPE_PICTURE;
                }
            }
            else
            {
                //if (AttachmentIsFTP)
                //{
                //    string ImageType = "Image";
                //    LookupDTO ImageTypeLookup = LocalBuffer.LocalBuffer.LookUpBufferList.FirstOrDefault(x => x.Id == catagory);
                //    if (ImageTypeLookup != null)
                //        ImageType = ImageTypeLookup.Description;
                //    bool initalized = FTPAttachment.InitalizeFTPAttachment(AttachmentIsFTPTINNo, AttachmentIsFTPOrganizationUnitDef, AttachmentIsFTPRoom, AttachmentFTPRoomType);
                //    if (!initalized)
                //        return;

                //    string ftppath = FTPAttachment.SendFTPImage(ImageType, fileName);

                //    attachmentObj.Url = ftppath;
                //    attachmentObj.Type = getLookUp(extension);
                //}
                //else
                //{
                //if (DefaultAttachmentPath != null && !string.IsNullOrEmpty(DefaultAttachmentPath))
                //{
                //    if (!Directory.Exists(DefaultAttachmentPath))
                //    {
                //        try
                //        {
                //            Directory.CreateDirectory(DefaultAttachmentPath);
                //        }
                //        catch
                //        {
                //            Progress_Reporter.Close_Progress();
                //            XtraMessageBox.Show("Default Attachment Path Not Found!", "ERP Attachment");
                //            return;
                //        }
                //    }

                //    string pathToSave = DefaultAttachmentPath;
                /*  string Name = Path.GetFileName(fileName);
                  string destfile = Path.Combine(pathToSave, Name);
                  if (File.Exists(destfile))
                  {
                      int namecount = 1;
                      while (File.Exists(destfile))
                      {
                          string namewithoutex = Path.GetFileNameWithoutExtension(fileName);
                          string fileextension = Name.Replace(namewithoutex, "");
                          Name = namewithoutex + "(" + namecount.ToString() + ")" + fileextension;
                          destfile = Path.Combine(pathToSave, Name);
                          namecount++;
                      }
                  }
                  File.Copy(fileName, destfile, true);
                */
                string ftpurl = "";
                if (!voucherFlag)
                    ftpurl = FTPInterface.FTPAttachment.SendGSlFileAttachement(TypeId, fileName, attReference.Value);
                else
                    ftpurl = FTPInterface.FTPAttachment.SendTransactionAttachement(TypeId, fileName, attReference.Value);
                // string ftpurl = FTPInterface.FTPAttachment.SendGSlFileAttachement(TypeId, fileName, attReference.Value);
                attachmentObj.Url = ftpurl;
                attachmentObj.Type = getLookUp(extension);
                attachmentObj.Remark = Path.GetExtension(fileName);


                if (string.IsNullOrEmpty(txtDescription.Text.ToString()))
                {
                    Progress_Reporter.Close_Progress();
                    XtraMessageBox.Show("Please Enter Description", "ERP Attachment");
                    return;
                }
                if (txtDescription.Text == "logo" && !string.IsNullOrEmpty(fileName))
                {
                    /* string pathToSave = DefaultAttachmentPath;
                     string destfile = Path.Combine(pathToSave, Name);
                     if (File.Exists(destfile))
                     {
                         int namecount = 1;
                         while (File.Exists(destfile))
                         {
                             string namewithoutex = Path.GetFileNameWithoutExtension(fileName);
                             string fileextension = Name.Replace(namewithoutex, "");
                             Name = namewithoutex + "(" + namecount.ToString() + ")" + fileextension;
                             destfile = Path.Combine(pathToSave, Name);
                             namecount++;
                         }
                     }*/

                    if (!voucherFlag)
                        ftpurl = FTPInterface.FTPAttachment.SendGSlFileAttachement(TypeId, fileName, attReference.Value);
                    else
                        ftpurl = FTPInterface.FTPAttachment.SendTransactionAttachement(TypeId, fileName, attReference.Value);
                    // ftpurl = FTPInterface.FTPAttachment.SendGSlFileAttachement(TypeId, fileName, attReference.Value);
                    attachmentObj.Url = ftpurl;


                    //File.Copy(fileName, destfile, true);
                    //    attachmentObj.Url = destfile;
                    attachmentObj.Remark = Path.GetExtension(fileName).ToLower();
                    attachmentObj.Type = getLookUp(extension);
                }


            }


            if (attachmentObj.Url == "")
            {
                Progress_Reporter.Close_Progress();
                XtraMessageBox.Show("No File To Save", "ERP Attachment");
                return;
            }
            else
            {
                if (txtDescription.Text == null || string.IsNullOrEmpty(txtDescription.Text.ToString()))
                {
                    Progress_Reporter.Close_Progress();
                    XtraMessageBox.Show("Please Enter Description", "ERP Attachment");
                    return;
                }
                else
                {
                    attachmentObj.Description = txtDescription.Text.ToString();



                    attachmentObj.Reference = ERP_Attachments.reference;
                    var referenceItem = UIProcessManager.GetAttachmentByReference(attachmentObj.Reference);
                    attachmentObj.Index = (byte)referenceItem.Count;
                    attachmentObj.Category = catagory;
                    UIProcessManager.CreateAttachment(attachmentObj);
                    txtDescription.Text = "";
                    txtUrl.Text = "";
                    pictureEdit1.Visible = false;
                    richEditControl1.Visible = false;
                    pdfViewer1.Visible = false;
                    spreadsheetControl1.Visible = false;
                    axWindowsMediaPlayer1.Visible = false;

                }

                Progress_Reporter.Show_Progress("Saving Attachment", "Please Wait..");
                Progress_Reporter.Close_Progress();
                XtraMessageBox.Show("Attachment Saved!", "ERP Attachment");
            }

            fileName = null;
            acquiredImage = null;
            Hide();
            // }
            cameraMain.closeOpenCamera();
        }

        
        public static byte[] fileToByte(string fileName)
        {
            byte[] dataByte = null;

            using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                using (var reader = new BinaryReader(stream))
                {
                    try
                    {
                        dataByte = reader.ReadBytes((int)stream.Length);
                    }
                    catch
                    {
                        XtraMessageBox.Show("File too Large! \n Only URL", "ERP Attachment");
                    }
                }
            }
            return dataByte;
        }
       
        public static byte[] ImagetoByte(Image image)
        {
            if (image == null)
            {
                return null;
            }
            MemoryStream ms = new MemoryStream();
            try
            {
                using (ms)
                {
                    image.Save(ms, ImageFormat.Png);
                    return ms.ToArray();
                }
            }
            catch (Exception ex)
            {
                ms = null;
            }
            return ms.ToArray();
        }

        public static int getLookUp(string ext)
        {
            int lookUP = 0;
            if (ext == ".jpg")
                lookUP = CNETConstantes.ATTACHMENT_TYPE_PICTURE;
            else if (ext == ".png")
                lookUP = CNETConstantes.ATTACHMENT_TYPE_PICTURE;
            else if (ext == ".jpeg")
                lookUP = CNETConstantes.ATTACHMENT_TYPE_PICTURE;
            else if (ext == ".bmp")
                lookUP = CNETConstantes.ATTACHMENT_TYPE_PICTURE;
            else if (ext == ".gif")
                lookUP = CNETConstantes.ATTACHMENT_TYPE_PICTURE;
            else if (ext == ".wmf")
                lookUP = CNETConstantes.ATTACHMENT_TYPE_PICTURE;
            else if (ext == ".tif")
                lookUP = CNETConstantes.ATTACHMENT_TYPE_PICTURE;
            else if (ext == ".tiff")
                lookUP = CNETConstantes.ATTACHMENT_TYPE_PICTURE;

            else if (ext == ".xls")
                lookUP = CNETConstantes.ATTACHMENT_TYPE_XLS;
            else if (ext == ".txt")
                lookUP = CNETConstantes.ATTACHMENT_TYPE_TEXT;
            else if (ext == ".ej")
                lookUP = CNETConstantes.ATTACHMENT_TYPE_EJ;
            else if (ext == ".docx")
                lookUP = CNETConstantes.ATTACHMENT_TYPE_DOCX;
            else if (ext == ".doc")
                lookUP = CNETConstantes.ATTACHMENT_TYPE_DOC;
            else if (ext == ".xlsx")
                lookUP = CNETConstantes.ATTACHMENT_TYPE_XLSX;
            else if (ext == ".pdf")
                lookUP = CNETConstantes.ATTACHMENT_TYPE_PDF;
            else if (ext == ".csv")
                lookUP = CNETConstantes.ATTACHMENT_TYPE_CSV;
            else if (ext == ".pptx")
                lookUP = CNETConstantes.ATTACHMENT_TYPE_PPTX;
            else if (ext == ".ppt")
                lookUP = CNETConstantes.ATTACHMENT_TYPE_PPT;

            else if (ext == ".wav")
                lookUP = CNETConstantes.ATTACHMENT_TYPE_AUDIO;
            else if (ext == ".mp3")
                lookUP = CNETConstantes.ATTACHMENT_TYPE_AUDIO;
            else if (ext == ".m4a")
                lookUP = CNETConstantes.ATTACHMENT_TYPE_AUDIO;
            else if (ext == ".wma")
                lookUP = CNETConstantes.ATTACHMENT_TYPE_AUDIO;
            else if (ext == ".cda")
                lookUP = CNETConstantes.ATTACHMENT_TYPE_AUDIO;
            else if (ext == ".mp2")
                lookUP = CNETConstantes.ATTACHMENT_TYPE_AUDIO;

            else if (ext == ".mpg")
                lookUP = CNETConstantes.ATTACHMENT_TYPE_VIDEO;
            else if (ext == ".mp4")
                lookUP = CNETConstantes.ATTACHMENT_TYPE_VIDEO;
            else if (ext == ".avi")
                lookUP = CNETConstantes.ATTACHMENT_TYPE_VIDEO;
            else if (ext == ".flv")
                lookUP = CNETConstantes.ATTACHMENT_TYPE_VIDEO;
            else if (ext == ".3gp")
                lookUP = CNETConstantes.ATTACHMENT_TYPE_VIDEO;
            else if (ext == ".mov")
                lookUP = CNETConstantes.ATTACHMENT_TYPE_VIDEO;
            else if (ext == ".wmv")
                lookUP = CNETConstantes.ATTACHMENT_TYPE_VIDEO;
            else
            {
                lookUP = 0;
            }

            return lookUP;
        }

        private void bbiCamera_ItemClick(object sender, ItemClickEventArgs e)
        {
            fileName = null;
            cbDevices.Properties.Items.Clear();
            AForge.Video.DirectShow.FilterInfoCollection avialableCameras = cameraMain.getCameraDevices();
            if (avialableCameras == null || avialableCameras.Count <= 0)
            {
                XtraMessageBox.Show("No Camera is connected to this PC", "ERP Attachment", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            else
            {
                foreach (AForge.Video.DirectShow.FilterInfo device in avialableCameras)
                {
                    cbDevices.Properties.Items.Add(device.Name);
                }
            }
            if (cbDevices.Properties.Items.Count > 0)
                cbDevices.SelectedIndex = 0;
            pcPreview.Controls.Clear();
            groupControl1.Text = "Camera";


            pcPreview.Controls.Add(cameraMain);
            cameraMain.Dock = DockStyle.Fill;
            bbiCapture.Visible = true;
            bbiConfig.Visible = true;
            bbiScanDocumnet.Visible = false;
            txtUrl.Visible = false;
            cbDevices.Visible = true;
            cbScannerDevices.Visible = false; 
        }


        ScannerMain imageScanner;
        private void barButtonItem1_ItemClick(object sender, ItemClickEventArgs e)
        {
            fileName = null;
            scaner = new ScannerMain();

            try
            {
                //get list of devices available 
                List<string> devices = scaner.GetDevices();
                cbScannerDevices.Properties.Items.Clear(); foreach (string device in devices)
                {
                    cbScannerDevices.Properties.Items.Add(device);
                }
                //check if device is not available
                if (cbScannerDevices.Properties.Items.Count == 0)
                {
                    XtraMessageBox.Show("No Scanner is connected to this PC", "ERP Attachment", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                else
                {
                    bbiCapture.Visible = false;
                    bbiConfig.Visible = false;
                    bbiScanDocumnet.Visible = true;
                    txtUrl.Visible = false;
                    cbDevices.Visible = false;
                    cbScannerDevices.Visible = true; 
                    cbScannerDevices.Visible = true;

                    if (cbScannerDevices.Properties.Items.Count > 0)
                        cbScannerDevices.SelectedIndex = 0;
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
            groupControl1.Text = "Scan";
            UseImageScanner();
        }

        private void UseImageScanner()
        {
            imageScanner = new ScannerMain();

            pcPreview.Controls.Clear();
            pcPreview.Controls.Add(imageScanner);
            imageScanner.Dock = DockStyle.Fill;


        }

        private void bbiCapture_Click(object sender, EventArgs e)
        {
            if (bbiCapture.Text == "Capture")
            {
                Bitmap capturedImage = cameraMain.capture();
                imageEdit.pbEdit.Image = capturedImage;
                acquiredImage = capturedImage;
                acquiredImage.Tag = "Camera";
                bbiCapture.Text = "Clear";

                if (imageEdit.pbEdit.Image.Height <= imageEdit.pbEdit.Height &&
                    imageEdit.pbEdit.Image.Width <= imageEdit.pbEdit.Width)
                {
                    imageEdit.pbEdit.SizeMode = PictureBoxSizeMode.CenterImage;
                }

                else
                {
                    imageEdit.pbEdit.SizeMode = PictureBoxSizeMode.Zoom;
                }

                bbiEditor.Enabled = true;
            }
            else if (bbiCapture.Text == "Clear")
            {
                cameraMain.clear();
                bbiCapture.Text = "Capture";
                bbiEditor.Enabled = false;
            }
        }

        private void bbiConfig_Click(object sender, EventArgs e)
        {
            try
            {
                cameraMain.showConfiguration(this.Handle);
            }
            catch
            { }
        }


        public void cbDevices_SelectedIndexChanged(object sender, EventArgs e)
        {
            cameraMain.connectToCamera(cbDevices.SelectedIndex);
        }

        private void cbScannerDevices_SelectedIndexChanged(object sender, EventArgs e)
        {
            ScannerMain.ComboValue(cbScannerDevices.SelectedItem.ToString());
        }

        private void bbiScanDocumnet_Click(object sender, EventArgs e)
        {
            if (cbScannerDevices.SelectedItem == null)
            {
                XtraMessageBox.Show("Please Select Scanner!", "ERP Attachment");
                return;
            }
            acquiredImage = scaner.Scan(cbScannerDevices.SelectedItem.ToString()).FirstOrDefault();
            acquiredImage.Tag = "Scanner";
            imageScanner.pictureEdit1.Image = acquiredImage;
            if (imageScanner.pictureEdit1.Image.Height <= imageScanner.pictureEdit1.Height && pictureEdit1.Image.Width <= pictureEdit1.Width)
                imageScanner.pictureEdit1.Properties.SizeMode = PictureSizeMode.Clip;
            else
                imageScanner.pictureEdit1.Properties.SizeMode = PictureSizeMode.Zoom;
            imageEdit.pbEdit.Image = imageScanner.pictureEdit1.Image;
            if (imageEdit.pbEdit.Image.Height <= imageEdit.pbEdit.Height &&
                imageEdit.pbEdit.Image.Width <= imageEdit.pbEdit.Width)
            {
                imageEdit.pbEdit.SizeMode = PictureBoxSizeMode.CenterImage;
            }

            else
            {
                imageEdit.pbEdit.SizeMode = PictureBoxSizeMode.Zoom;
            }

            bbiEditor.Enabled = true;
        }


        imageEditor imageEdit = new imageEditor();
        private void bbiEditor_ItemClick_1(object sender, ItemClickEventArgs e)
        {
            fileName = null;
            imageEdit.bbiSave.ItemClick += bbiSaveEditor_ItemClick;
            imageEdit.ShowDialog();
        }

        // This Event saves the edited image from the image editor into pictureEdit1
        // This is called when the save button on the image editor is clicked
        private void bbiSaveEditor_ItemClick(object sender, ItemClickEventArgs e)
        {
            pictureEdit1.Visible = true;
            pictureEdit1.Image = imageEdit.pbEdit.Image;
            acquiredImage = pictureEdit1.Image;
            acquiredImage.Tag = "Edited";
            txtUrl.Text = "";
            pcPreview.Controls.Clear();
            pcPreview.Controls.Add(pictureEdit1);
            pictureEdit1.Properties.SizeMode = PictureSizeMode.Clip;

        }

        #region NEW METHOD
        private AttachmentDTO saveAndReturnAttachment(int id)
        {
            axWindowsMediaPlayer1.close();
            if (string.IsNullOrEmpty(fileName))
            {


                //acquiredImage = Image.FromFile(fileName, true);
                if (acquiredImage != null)
                {
                    if (acquiredImage.Tag == "Camera")
                    {
                        //string savePath = pathToSave + @"\Snapshoot " + DateTime.Now.ToString("yyyyMMddHHmmss") +
                        //              ".png";
                        //acquiredImage.Save(savePath, ImageFormat.Png);
                        string ftpurl = FTPInterface.FTPAttachment.SendConsigneeImageAttachement(TypeId, attReference.ToString(), acquiredImage);
                        attachmentObj.Url = ftpurl;
                        attachmentObj.Remark = ".png";
                        attachmentObj.Type = CNETConstantes.ATTACHMENT_TYPE_PICTURE;
                    }
                    if (acquiredImage.Tag == "Scanner")
                    {
                        //string savePath = pathToSave + @"\ScannedImage" + DateTime.Now.ToString("yyyyMMddHHmmss") +
                        //                  ".png";
                        try
                        {

                            Bitmap bmp = new Bitmap(acquiredImage.Width, acquiredImage.Height);
                            Graphics graphics = Graphics.FromImage(bmp);
                            graphics.DrawImage(acquiredImage, new Rectangle(0, 0, bmp.Width, bmp.Height), 0, 0, bmp.Width, bmp.Height, GraphicsUnit.Pixel);
                            acquiredImage.Dispose();
                            acquiredImage = bmp;
                            graphics.Dispose();

                            //acquiredImage.Save(savePath, ImageFormat.Png);

                            string ftpurl = FTPInterface.FTPAttachment.SendConsigneeImageAttachement(TypeId, attReference.ToString(), acquiredImage);
                            attachmentObj.Url = ftpurl;
                            attachmentObj.Remark = ".png";
                            attachmentObj.Type = CNETConstantes.ATTACHMENT_TYPE_PICTURE;
                        }
                        catch (Exception ex)
                        {
                            XtraMessageBox.Show("Saving  Image Error" + Environment.NewLine + ex.Message);
                        }
                    }
                    if (acquiredImage.Tag == "Edited")
                    {
                        //string savePath = pathToSave + @"\Edited Image " + DateTime.Now.ToString("yyyyMMddHHmmss") +
                        //              ".png";
                        //acquiredImage.Save(savePath, ImageFormat.Png);
                        string ftpurl = FTPInterface.FTPAttachment.SendConsigneeImageAttachement(TypeId, attReference.ToString(), acquiredImage);
                        attachmentObj.Url = ftpurl;
                        attachmentObj.Remark = ".png";
                        attachmentObj.Type = CNETConstantes.ATTACHMENT_TYPE_PICTURE;
                    }
                }
                else
                {
                    Progress_Reporter.Close_Progress();
                    XtraMessageBox.Show("No File To Save", "ERP Attachment");
                    return null;
                }



            }
            else
            {
                string ftpurl = "";
                if (!voucherFlag)
                    ftpurl = FTPInterface.FTPAttachment.SendGSlFileAttachement(TypeId, fileName, attReference.Value);
                else
                    ftpurl = FTPInterface.FTPAttachment.SendTransactionAttachement(TypeId, fileName, attReference.Value);
                attachmentObj.Url = ftpurl;
                attachmentObj.Remark = Path.GetExtension(fileName);

                attachmentObj.Type = getLookUp(extension);
            }


            if (string.IsNullOrEmpty(attachmentObj.Url))
            {
                Progress_Reporter.Close_Progress();
                XtraMessageBox.Show("No File To Save", "ERP Attachment");
                return null;
            }
            else
            {
                if (txtDescription.Text == null || string.IsNullOrEmpty(txtDescription.Text.ToString()))
                {
                    Progress_Reporter.Close_Progress();
                    XtraMessageBox.Show("Please Enter Description", "ERP Attachment");
                    return null;
                }
                else
                {

                    //    if (temporaryAttachments == null || temporaryAttachments.Count == 0)
                    //        attachmentObj.Id=0;
                    //    else
                    //        if (temporaryAttachments.Last().code.ToLower().Contains("att"))
                    //        {
                    //            int codex = Int32.Parse(new String(temporaryAttachments.Last().code.Where(Char.IsDigit).ToArray())) + 1;
                    //            attachmentObj.code = String.Format("Att{0}", codex);
                    //        }
                    //        else 
                    //            attachmentObj.code = (Convert.ToInt16(temporaryAttachments.Last().code) + 1).ToString();


                    attachmentObj.Id = 0;
                    attachmentObj.Description = txtDescription.Text.ToString();
                    attachmentObj.Reference = 0;
                    attachmentObj.Index = 0;
                    attachmentObj.Category = catagory;
                    txtDescription.Text = "";
                    txtUrl.Text = "";
                    pictureEdit1.Visible = false;
                    richEditControl1.Visible = false;
                    pdfViewer1.Visible = false;
                    spreadsheetControl1.Visible = false;
                    axWindowsMediaPlayer1.Visible = false;

                }

                Progress_Reporter.Close_Progress();
                return attachmentObj;
            }

            fileName = null;
            acquiredImage = null;

        }
        #endregion


    }
    public class DataType
    {
        public int code { get; set; }
        public string Description { get; set; }
    }
}