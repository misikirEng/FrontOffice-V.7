using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes; 
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using DevExpress.Data.Linq;
using WMPLib;
using DevExpress.XtraGrid.Columns; 
using DevExpress.XtraEditors;
using DevExpress.XtraRichEdit;
using CNET_ImageEditor;
using DevExpress.XtraEditors.Controls;
using DevExpress.Utils.Menu;
using ERP.Attachement;
using CNET_V7_Domain.Domain.CommonSchema;
using ProcessManager;

namespace CNETAttachment
{
    public partial class CNET_Attachments: UserControl
    {
        Dictionary<string, string> dictionary = new Dictionary<string, string>();
        GridColumn gcolParentName = new GridColumn();
        GridColumn gcolRightKey = new GridColumn();
        string typeForLookup = null;
        List<AttachmentDTO> itemByRefd = null;
        int Type { get; set; }
        private GridColumn gcolRightValue = new GridColumn();
        public static int reference;
        private DXMenuItem menuItems = new DXMenuItem("Open");
        modalNewAttachment modalNewAtta = null;


        private bool voucherFlag = false;
        private string tempAttaReference = "";
        public List<AttachmentDTO> tempAtta = null;
        private List<LookupDTO> attachmentCatagories = null;
        private int? selectedAttachmentCode = null;
        private string selectedAttachmentCatgory = null;
        public bool AttachmentIsFTP { get; set; }
        public string AttachmentIsFTPTINNo { get; set; }
        public string AttachmentIsFTPOrganizationUnitDef { get; set; }
        public bool AttachmentIsFTPIsRoom { get; set; }
        public string AttachmentFTPRoomType { get; set; }
        /// <summary>
        /// use this for voucher atttachment
        /// </summary>
        /// <param name="attachments"></param>
        /// <param name="isVoucher"></param>
        /// <param name="code"></param>
        public CNET_Attachments(List<AttachmentDTO> attachments,int type, bool isVoucher, string code, List<LookupDTO> displayedAttachmentCatagories = null)
        {
            InitializeComponent(); 
            Type = type;
            ribbonControl1.Visible = false;
            voucherFlag = isVoucher;
            tempAttaReference = code;
            tempAtta = new List<AttachmentDTO>();
            if (displayedAttachmentCatagories == null)
            {
                if (LocalBuffer.LocalBuffer.LookUpBufferList != null)
                    attachmentCatagories = LocalBuffer.LocalBuffer.LookUpBufferList.Where(x => x.Type.ToLower() == "attachment catagory").ToList();
            }
            else
            {
                attachmentCatagories = displayedAttachmentCatagories;
            }
            if (attachments != null)
                tempAtta = attachments;
            showTempAttachments(tempAtta);
        }

        /// <summary>
        /// Use this constractor for General Attachment use
        /// </summary>
        /// <param name="code"></param>
        /// <param name="ribbonVisible"></param>
        public CNET_Attachments(string code, int type, bool ribbonVisible, List<LookupDTO> displayedAttachmentCatagories = null)
        {
                InitializeComponent();
                reference = code;
                Type = type;

                ribbonControl1.Visible = ribbonVisible;
                if (displayedAttachmentCatagories == null)
                {
                    if (LocalBuffer.LocalBuffer.LookUpBufferList != null)
                        attachmentCatagories = LocalBuffer.LocalBuffer.LookUpBufferList.Where(x => x.Type.ToLower() == "attachment catagory").ToList();
                }
                else
                {
                    attachmentCatagories = displayedAttachmentCatagories;
                }
            if(reference != null)
                showAttachement(reference.Trim());

        }

        public CNET_Attachments(int type)
        {
            InitializeComponent();
            Type = type;
        }

        public CNET_Attachments(string code ,int type)
        {
            InitializeComponent();
            reference = code;
            Type = type;
        }

        public CNET_Attachments(bool ribbonVisible,int type, List<LookupDTO> displayedAttachmentCatagories = null)
        {
            InitializeComponent();
            Type = type;
            ribbonControl1.Visible = ribbonVisible;
            if (displayedAttachmentCatagories == null)
            {
                if (LocalBuffer.LocalBuffer.LookUpBufferList != null)
                    attachmentCatagories = LocalBuffer.LocalBuffer.LookUpBufferList.Where(x => x.Type.ToLower() == "attachment catagory").ToList();
            }
            else
            {
                attachmentCatagories = displayedAttachmentCatagories;
            }
        }

        private void bbiNew_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
           OpenNewAttachment(reference,Type);
        }

        /// <summary>
        /// use this method to call new attachment form for general purpose Attachment
        /// use this method with general purpose attachment constractor
        /// </summary>
        /// <param name="codenew"></param>
        public void OpenNewAttachment(string codenew,int Type)
        {
            if (!string.IsNullOrEmpty(selectedAttachmentCatgory))
            {
                reference = codenew;
                modalNewAttachment newform1 = new modalNewAttachment(selectedAttachmentCatgory, Type, AttachmentIsFTP);
                newform1.AttachmentIsFTP = AttachmentIsFTP;
                newform1.AttachmentFTPRoomType = AttachmentFTPRoomType;
                newform1.AttachmentIsFTPRoom = AttachmentIsFTPIsRoom;
                newform1.AttachmentIsFTPTINNo = AttachmentIsFTPTINNo;
                newform1.AttachmentIsFTPOrganizationUnitDef = AttachmentIsFTPOrganizationUnitDef;
                newform1.bbiSave.ItemClick += SaveClicked;
                newform1.ShowDialog();
            }
            else
            {
                XtraMessageBox.Show("Please Select Attachment Catagory!", "CNET Attachment");
            }
        }

        private void SaveClicked(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (voucherFlag)
            {
                if (modalNewAtta.savedAttachment != null)
                    tempAtta.Add(modalNewAtta.savedAttachment);
                showTempAttachments(tempAtta);
            }
            else
            {
                showAttachement(reference.Trim());
            }
        }

        private void spreadsheetControl1_MouseHover(object sender, EventArgs e)
        {
            spreadsheetControl1.Focus();
        }

        private void pictureBox1_MouseHover(object sender, EventArgs e)
        {
            pictureEdit1.Focus();
        }

        private void pdfViewer1_MouseHover(object sender, EventArgs e)
        {
            pdfViewer1.Focus();
        }

        private void richEditControl1_MouseHover(object sender, EventArgs e)
        {
            richEditControl1.Focus();
        }

       
        public void DeleteAttachment(string reff = null) 
        {
            if (voucherFlag)
            {
                if (selectedAttachmentCode != null)
                {
                    try
                    {
                        tempAtta.Remove(tempAtta.Single(x => x.Id == selectedAttachmentCode));
                        showTempAttachments(tempAtta);
                    }
                    catch { };
                }
                else
                {
                    XtraMessageBox.Show("Please Select Attachment to Delete!", "CNET Attachment");
                }
            }
            else
            {
                if (selectedAttachmentCode != null)
                {
                    try
                    {
                        UIProcessManager.DeleteAttachmentById(selectedAttachmentCode.Value);
                        showAttachement(reff.Trim());
                    }
                    catch { }
                }
                else
                {
                    XtraMessageBox.Show("Please Select Attachment to Delete!", "CNET Attachment");
                }
            }
        }

        #region METHODS
        public void disposeAttachmentControls()
        {
            try
            {
                pdfViewer1.Dispose();
                pdfViewer1.Visible = false;
                richEditControl1.Dispose();
                richEditControl1.Visible = false;
                pictureEdit1.Dispose();
                pictureEdit1.Visible = false;
                spreadsheetControl1.Dispose();
                spreadsheetControl1.Visible = false;
                axWindowsMediaPlayer1.close();
                axWindowsMediaPlayer1.Visible = false;
                treeList1.ClearNodes();
            }
            catch
            { }
        }
        /// <summary>
        /// For General Purpose
        /// </summary>
        /// <param name="selectedReference"></param>
        public void showAttachement(int selectedReference , bool documentBrowser = false)
        {
            if (documentBrowser)
                reference = selectedReference;
            pdfViewer1.Visible = false;
            richEditControl1.Visible = false;
            pictureEdit1.Visible = false;
            spreadsheetControl1.Visible = false;
            axWindowsMediaPlayer1.close();
            axWindowsMediaPlayer1.Visible = false;
            gcolRightKey.Visible = true;
            treeList1.ClearNodes();

            if (attachmentCatagories != null)
            {
                itemByRefd = UIProcessManager.GetAttachmentByReference(selectedReference).ToList();

             
                //Populate Parent Node
                for (int index = 0; index < attachmentCatagories.Count; index++)
                {
                    int INDEX = getTreelistImageIndex(attachmentCatagories[index].Id.ToString());
                    if(INDEX == -1)
                        treeList1.AppendNode(new Object[] { attachmentCatagories[index].Description, attachmentCatagories[index].Id }, index);
                    else
                        treeList1.AppendNode(new Object[] { attachmentCatagories[index].Description, attachmentCatagories[index].Id }, index, INDEX, INDEX, INDEX);
                    
                }

                //Populate Child Node
                for (int index = 0; index < attachmentCatagories.Count; index++)
                {                  
                    var thisLookupAtt = itemByRefd.Where(x => x.Category == attachmentCatagories[index].Id).OrderBy(x => x.Description).ToList();

                    foreach (AttachmentDTO att in thisLookupAtt)
                    {
                        int INDEX = getAttachmentIconIndex(att.Type.ToString());
                        if(INDEX == -1)
                            treeList1.AppendNode(new object[] { att.Description, att.Id }, index);
                        else
                            treeList1.AppendNode(new object[] { att.Description, att.Id }, index, INDEX, INDEX, INDEX);
                    }
                }

                treeList1.EndUnboundLoad();
                treeList1.ExpandAll();
            }
            
        }


        public void showAttachement(int selectedReference)
        {

            pdfViewer1.Visible = false;
            richEditControl1.Visible = false;
            pictureEdit1.Visible = false;
            spreadsheetControl1.Visible = false;
            axWindowsMediaPlayer1.close();
            axWindowsMediaPlayer1.Visible = false;
            gcolRightKey.Visible = true;
            treeList1.ClearNodes();

            if (attachmentCatagories != null)
            {
                itemByRefd = UIProcessManager.GetAttachmentByReference(selectedReference).ToList();


                //Populate Parent Node
                for (int index = 0; index < attachmentCatagories.Count; index++)
                {
                    int INDEX = getTreelistImageIndex(attachmentCatagories[index].Id.ToString());
                    if (INDEX == -1)
                        treeList1.AppendNode(new Object[] { attachmentCatagories[index].Description, attachmentCatagories[index].Id }, index);
                    else
                        treeList1.AppendNode(new Object[] { attachmentCatagories[index].Description, attachmentCatagories[index].Id }, index, INDEX, INDEX, INDEX);

                }

                //Populate Child Node
                for (int index = 0; index < attachmentCatagories.Count; index++)
                {
                    var thisLookupAtt = itemByRefd.Where(x => x.Category == attachmentCatagories[index].Id).OrderBy(x => x.Description).ToList();

                    foreach (AttachmentDTO att in thisLookupAtt)
                    {
                        int INDEX = getAttachmentIconIndex(att.Type.ToString());
                        if (INDEX == -1)
                            treeList1.AppendNode(new object[] { att.Description, att.Id }, index);
                        else
                            treeList1.AppendNode(new object[] { att.Description, att.Id }, index, INDEX, INDEX, INDEX);
                    }
                }

                treeList1.EndUnboundLoad();
                treeList1.ExpandAll();
            }

        }


        /// <summary>
        /// For Voucher Attachment
        /// </summary>
        /// <param name="temporaryAttachments"></param>
        public void showTempAttachments(List<AttachmentDTO> temporaryAttachments)
        {
            pdfViewer1.Visible = false;
            richEditControl1.Visible = false;
            pictureEdit1.Visible = false;
            spreadsheetControl1.Visible = false;
            axWindowsMediaPlayer1.close();
            axWindowsMediaPlayer1.Visible = false;
            gcolRightKey.Visible = true;
            treeList1.ClearNodes();

            if (attachmentCatagories != null)
            {
                itemByRefd = temporaryAttachments;

                //Populate Parent Node
                for (int index = 0; index < attachmentCatagories.Count; index++)
                {
                    int INDEX = getTreelistImageIndex(attachmentCatagories[index].Id.ToString());
                    if (INDEX == -1)
                        treeList1.AppendNode(new Object[] { attachmentCatagories[index].Description, attachmentCatagories[index].Id }, index);
                    else
                        treeList1.AppendNode(new Object[] { attachmentCatagories[index].Description, attachmentCatagories[index].Id }, index, INDEX, INDEX, INDEX);
                }

                //Populate Child Node
                for (int index = 0; index < attachmentCatagories.Count; index++)
                {
                    var thisLookupAtt = itemByRefd.Where(x => x.Category == attachmentCatagories[index].Id).OrderBy(x => x.Description).ToList();

                    foreach (AttachmentDTO att in thisLookupAtt)
                    {
                        int INDEX = getAttachmentIconIndex(att.Type.ToString());
                        if (INDEX == -1)
                            treeList1.AppendNode(new object[] { att.Description, att.Id }, index);
                        else
                            treeList1.AppendNode(new object[] { att.Description, att.Id }, index, INDEX, INDEX, INDEX);
                    }
                }

                treeList1.EndUnboundLoad();
                treeList1.ExpandAll();
            }
            

        }

        /// <summary>
        /// For General purpose
        /// </summary>
        /// <param name="attachmentCode"></param>
        private void showSelectedAttachment(int attachmentCode)
        {
            CNETInfoReporter.WaitForm("Opening Attachment", "Please Wait..", 1, 2);
            axWindowsMediaPlayer1.close();

            var attachmentItem = UIProcessManager.GetAttachmentById(attachmentCode);

            if (attachmentItem != null)
            {
                try
                {
                    if (attachmentItem.Type == CNETConstantes.ATTACHMENT_TYPE_PICTURE)
                    {
                        pictureEdit1.Visible = true;
                        richEditControl1.Visible = false;
                        pdfViewer1.Visible = false;
                        spreadsheetControl1.Visible = false;
                        axWindowsMediaPlayer1.Visible = false;
                        pictureEdit1.Image = null;
                    //    if (attachmentItem.file == null)
                    //    {
                    //        goto here;
                    //    }

                    //    MemoryStream memory = new MemoryStream(attachmentItem.file);
                    //    if (memory.Length == 1)
                    //    {
                    //        goto here;
                    //    }

                    //    Image img = Image.FromStream(memory);
                    //    if (img != null)
                    //    {
                    //        pictureEdit1.Image = img;
                    //        if (pictureEdit1.Image.Height <= pictureEdit1.Height && pictureEdit1.Image.Width <= pictureEdit1.Width)
                    //            pictureEdit1.Properties.SizeMode = PictureSizeMode.Clip;
                    //        else
                    //            pictureEdit1.Properties.SizeMode = PictureSizeMode.Zoom;
                    //        goto here1;
                    //    }
                    //here:
                        if (AttachmentIsFTP && attachmentItem.Url.StartsWith("ftp://"))
                        {
                         pictureEdit1.Image =   FTPAttachment.GetImageFromFTP(attachmentItem.Url);
                        }
                        else
                        {

                        pictureEdit1.LoadAsync(attachmentItem.Url);
                        }
                        if (pictureEdit1.Image.Height <= pictureEdit1.Height && pictureEdit1.Image.Width <= pictureEdit1.Width)
                            pictureEdit1.Properties.SizeMode = PictureSizeMode.Clip;
                        else
                            pictureEdit1.Properties.SizeMode = PictureSizeMode.Zoom;

                    //here1: ;

                    }

                    else if (attachmentItem.Type == CNETConstantes.ATTACHMENT_TYPE_TEXT ||
                             attachmentItem.Type == CNETConstantes.ATTACHMENT_TYPE_EJ ||
                             attachmentItem.Type == CNETConstantes.ATTACHMENT_TYPE_DOCX ||
                             attachmentItem.Type == CNETConstantes.ATTACHMENT_TYPE_DOC)
                    {
                        pictureEdit1.Visible = false;
                        richEditControl1.Visible = true;
                        pdfViewer1.Visible = false;
                        spreadsheetControl1.Visible = false;
                        axWindowsMediaPlayer1.Visible = false;
                        richEditControl1.ResetText();
                        if (!string.IsNullOrEmpty(attachmentItem.Url))
                        {
                            if (attachmentItem.Type == CNETConstantes.ATTACHMENT_TYPE_TEXT ||
                                attachmentItem.Type == CNETConstantes.ATTACHMENT_TYPE_EJ)
                            {
                                richEditControl1.LoadDocument(attachmentItem.Url, DevExpress.XtraRichEdit.DocumentFormat.PlainText);
                            }


                            else if (attachmentItem.Type == CNETConstantes.ATTACHMENT_TYPE_DOCX)
                            {
                                richEditControl1.LoadDocument(attachmentItem.Url, DevExpress.XtraRichEdit.DocumentFormat.OpenXml);
                            }
                            else if (attachmentItem.Type == CNETConstantes.ATTACHMENT_TYPE_DOC)
                            {
                                richEditControl1.LoadDocument(attachmentItem.Url, DevExpress.XtraRichEdit.DocumentFormat.Doc);
                            }
                        //    goto here;
                        }

                        //MemoryStream memory = new MemoryStream(attachmentItem.file);

                        //if (memory.Length == 0)
                        //{

                        //    if (attachmentItem.type == CNETConstantes.ATTACHMENT_TYPE_TEXT ||
                        //        attachmentItem.type == CNETConstantes.ATTACHMENT_TYPE_EJ)
                        //    {
                        //        richEditControl1.LoadDocument(attachmentItem.url, DevExpress.XtraRichEdit.DocumentFormat.PlainText);
                        //    }
                        //    else if (attachmentItem.type == CNETConstantes.ATTACHMENT_TYPE_DOCX ||
                        //             attachmentItem.type == CNETConstantes.ATTACHMENT_TYPE_PPTX)
                        //    {
                        //        richEditControl1.LoadDocument(attachmentItem.url, DevExpress.XtraRichEdit.DocumentFormat.OpenXml);
                        //    }
                        //    else if (attachmentItem.type == CNETConstantes.ATTACHMENT_TYPE_DOC)
                        //    {
                        //        richEditControl1.LoadDocument(attachmentItem.url, DevExpress.XtraRichEdit.DocumentFormat.Doc);
                        //    }
                        //    goto here;
                        //}
                    //    if (attachmentItem.type == CNETConstantes.ATTACHMENT_TYPE_TEXT ||
                    //        attachmentItem.type == CNETConstantes.ATTACHMENT_TYPE_EJ)
                    //        richEditControl1.LoadDocument(memory, DocumentFormat.PlainText);

                    //    else if (attachmentItem.type == CNETConstantes.ATTACHMENT_TYPE_DOCX)
                    //        richEditControl1.LoadDocument(memory, DocumentFormat.OpenXml);

                    //    else if (attachmentItem.type == CNETConstantes.ATTACHMENT_TYPE_DOC)
                    //        richEditControl1.LoadDocument(memory, DocumentFormat.Doc);

                    //here:
                    //    ;
                    }


                    else if (attachmentItem.Type == CNETConstantes.ATTACHMENT_TYPE_XLS ||
                             attachmentItem.Type == CNETConstantes.ATTACHMENT_TYPE_XLSX ||
                             attachmentItem.Type == CNETConstantes.ATTACHMENT_TYPE_CSV)
                    {
                        pictureEdit1.Visible = false;
                        richEditControl1.Visible = false;
                        pdfViewer1.Visible = false;
                        spreadsheetControl1.Visible = true;
                        axWindowsMediaPlayer1.Visible = false;
                        spreadsheetControl1.ResetText();
                        if (!string.IsNullOrEmpty(attachmentItem.Url))
                        {
                            if (attachmentItem.Type == CNETConstantes.ATTACHMENT_TYPE_XLSX)
                                spreadsheetControl1.LoadDocument(attachmentItem.Url, DevExpress.Spreadsheet.DocumentFormat.OpenXml);

                            else if (attachmentItem.Type == CNETConstantes.ATTACHMENT_TYPE_XLS)
                                spreadsheetControl1.LoadDocument(attachmentItem.Url, DevExpress.Spreadsheet.DocumentFormat.Xls);

                            else if (attachmentItem.Type == CNETConstantes.ATTACHMENT_TYPE_CSV)
                                spreadsheetControl1.LoadDocument(attachmentItem.Url, DevExpress.Spreadsheet.DocumentFormat.Csv);
                            //goto here;
                        }
                    //    MemoryStream ms1 = new MemoryStream(attachmentItem.file);
                    //    if (ms1.Length == 0)
                    //    {
                    //        if (attachmentItem.type == CNETConstantes.ATTACHMENT_TYPE_XLSX)
                    //            spreadsheetControl1.LoadDocument(attachmentItem.url, DevExpress.Spreadsheet.DocumentFormat.OpenXml);

                    //        else if (attachmentItem.type == CNETConstantes.ATTACHMENT_TYPE_XLS)
                    //            spreadsheetControl1.LoadDocument(attachmentItem.url, DevExpress.Spreadsheet.DocumentFormat.Xls);

                    //        else if (attachmentItem.type == CNETConstantes.ATTACHMENT_TYPE_CSV)
                    //            spreadsheetControl1.LoadDocument(attachmentItem.url, DevExpress.Spreadsheet.DocumentFormat.Csv);

                    //        goto here;
                    //    }

                    //    if (attachmentItem.type == CNETConstantes.ATTACHMENT_TYPE_XLSX)
                    //        spreadsheetControl1.LoadDocument(ms1, DevExpress.Spreadsheet.DocumentFormat.OpenXml);

                    //    else if (attachmentItem.type == CNETConstantes.ATTACHMENT_TYPE_XLS)
                    //        spreadsheetControl1.LoadDocument(ms1, DevExpress.Spreadsheet.DocumentFormat.Xls);

                    //    else if (attachmentItem.type == CNETConstantes.ATTACHMENT_TYPE_CSV)
                    //        spreadsheetControl1.LoadDocument(ms1, DevExpress.Spreadsheet.DocumentFormat.Csv);

                    //here:
                    //    ;
                    }
                    else if (attachmentItem.Type == CNETConstantes.ATTACHMENT_TYPE_PDF)
                    {
                        pictureEdit1.Visible = false;
                        richEditControl1.Visible = false;
                        pdfViewer1.Visible = true;
                        spreadsheetControl1.Visible = false;
                        axWindowsMediaPlayer1.Visible = false;
                        if (!string.IsNullOrEmpty(attachmentItem.Url))
                        {
                            pdfViewer1.LoadDocument(attachmentItem.Url);
                            //goto here;
                        }
                    //    MemoryStream ms1 = new MemoryStream(attachmentItem.file);
                    //    if (ms1.Length == 0)
                    //    {
                    //        pdfViewer1.LoadDocument(attachmentItem.url);
                    //        goto here;
                    //    }
                    //    pdfViewer1.LoadDocument(ms1);

                    //here: ;
                    }
                    else if (attachmentItem.Type == CNETConstantes.ATTACHMENT_TYPE_VIDEO ||
                             attachmentItem.Type == CNETConstantes.ATTACHMENT_TYPE_AUDIO)
                    {
                        pictureEdit1.Visible = false;
                        richEditControl1.Visible = false;
                        pdfViewer1.Visible = false;
                        spreadsheetControl1.Visible = false;
                        axWindowsMediaPlayer1.Visible = true;
                        axWindowsMediaPlayer1.stretchToFit = true;
                        axWindowsMediaPlayer1.Dock = DockStyle.Fill;

                        axWindowsMediaPlayer1.URL = attachmentItem.Url;
                    }
                    else
                    {
                        pictureEdit1.Visible = true;
                        richEditControl1.Visible = false;
                        pdfViewer1.Visible = false;
                        spreadsheetControl1.Visible = false;
                        axWindowsMediaPlayer1.Visible = false;
                        pictureEdit1.Image = CNETAttachment.Properties.Resources.nopreview;
                    }
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show(ex.Message);
                }
            }
            CNETInfoReporter.WaitForm("Opening Attachment", "Please Wait..", 2, 2);
            CNETInfoReporter.Hide();
        }

        /// <summary>
        /// For Voucher
        /// </summary>
        /// <param name="attachment"></param>
        private void showSelectedAttachment(AttachmentDTO attachment)
        {
            CNETInfoReporter.WaitForm("Opening Attachment", "Please Wait..", 1, 2);
            axWindowsMediaPlayer1.close();

            var attachmentItem = attachment;

            if (attachmentItem != null)
            {
                try
                {
                    if (attachmentItem.Type == CNETConstantes.ATTACHMENT_TYPE_PICTURE)
                    {
                        pictureEdit1.Visible = true;
                        richEditControl1.Visible = false;
                        pdfViewer1.Visible = false;
                        spreadsheetControl1.Visible = false;
                        axWindowsMediaPlayer1.Visible = false;
                        pictureEdit1.Image = null;
                    //    if (attachmentItem.file == null)
                    //    {
                    //        goto here;
                    //    }
                    //    MemoryStream memory = new MemoryStream(attachmentItem.file);
                    //    if (memory.Length == 1)
                    //    {
                    //        goto here;
                    //    }

                    //    Image img = Image.FromStream(memory);
                    //    if (img != null)
                    //    {
                    //        pictureEdit1.Image = img;
                    //        if (pictureEdit1.Image.Height <= pictureEdit1.Height && pictureEdit1.Image.Width <= pictureEdit1.Width)
                    //            pictureEdit1.Properties.SizeMode = PictureSizeMode.Clip;
                    //        else
                    //            pictureEdit1.Properties.SizeMode = PictureSizeMode.Zoom;
                    //        goto here1;
                    //    }
                    //here:
                        pictureEdit1.LoadAsync(attachmentItem.Url);
                        if (pictureEdit1.Image.Height <= pictureEdit1.Height && pictureEdit1.Image.Width <= pictureEdit1.Width)
                            pictureEdit1.Properties.SizeMode = PictureSizeMode.Clip;
                        else
                            pictureEdit1.Properties.SizeMode = PictureSizeMode.Zoom;

                    //here1: ;

                    }

                    else if (attachmentItem.Type == CNETConstantes.ATTACHMENT_TYPE_TEXT ||
                             attachmentItem.Type == CNETConstantes.ATTACHMENT_TYPE_EJ ||
                             attachmentItem.Type == CNETConstantes.ATTACHMENT_TYPE_DOCX ||
                             attachmentItem.Type == CNETConstantes.ATTACHMENT_TYPE_DOC)
                    {
                        pictureEdit1.Visible = false;
                        richEditControl1.Visible = true;
                        pdfViewer1.Visible = false;
                        spreadsheetControl1.Visible = false;
                        axWindowsMediaPlayer1.Visible = false;
                        richEditControl1.ResetText();
                        if (!string.IsNullOrEmpty(attachmentItem.Url))
                        {
                            if (attachmentItem.Type == CNETConstantes.ATTACHMENT_TYPE_TEXT ||
                                attachmentItem.Type == CNETConstantes.ATTACHMENT_TYPE_EJ)
                            {
                                richEditControl1.LoadDocument(attachmentItem.Url, DevExpress.XtraRichEdit.DocumentFormat.PlainText);
                            }


                            else if (attachmentItem.Type == CNETConstantes.ATTACHMENT_TYPE_DOCX)
                            {
                                richEditControl1.LoadDocument(attachmentItem.Url, DevExpress.XtraRichEdit.DocumentFormat.OpenXml);
                            }
                            else if (attachmentItem.Type == CNETConstantes.ATTACHMENT_TYPE_DOC)
                            {
                                richEditControl1.LoadDocument(attachmentItem.Url, DevExpress.XtraRichEdit.DocumentFormat.Doc);
                            }
                            //goto here;
                        }

                    //    MemoryStream memory = new MemoryStream(attachmentItem.file);

                    //    if (memory.Length == 0)
                    //    {

                    //        if (attachmentItem.type == CNETConstantes.ATTACHMENT_TYPE_TEXT ||
                    //            attachmentItem.type == CNETConstantes.ATTACHMENT_TYPE_EJ)
                    //        {
                    //            richEditControl1.LoadDocument(attachmentItem.url, DevExpress.XtraRichEdit.DocumentFormat.PlainText);
                    //        }
                    //        else if (attachmentItem.type == CNETConstantes.ATTACHMENT_TYPE_DOCX ||
                    //                 attachmentItem.type == CNETConstantes.ATTACHMENT_TYPE_PPTX)
                    //        {
                    //            richEditControl1.LoadDocument(attachmentItem.url, DevExpress.XtraRichEdit.DocumentFormat.OpenXml);
                    //        }
                    //        else if (attachmentItem.type == CNETConstantes.ATTACHMENT_TYPE_DOC)
                    //        {
                    //            richEditControl1.LoadDocument(attachmentItem.url, DevExpress.XtraRichEdit.DocumentFormat.Doc);
                    //        }
                    //        goto here;
                    //    }
                    //    if (attachmentItem.type == CNETConstantes.ATTACHMENT_TYPE_TEXT ||
                    //        attachmentItem.type == CNETConstantes.ATTACHMENT_TYPE_EJ)
                    //        richEditControl1.LoadDocument(memory, DocumentFormat.PlainText);

                    //    else if (attachmentItem.type == CNETConstantes.ATTACHMENT_TYPE_DOCX)
                    //        richEditControl1.LoadDocument(memory, DocumentFormat.OpenXml);

                    //    else if (attachmentItem.type == CNETConstantes.ATTACHMENT_TYPE_DOC)
                    //        richEditControl1.LoadDocument(memory, DocumentFormat.Doc);

                    //here:
                        ;
                    }


                    else if (attachmentItem.Type == CNETConstantes.ATTACHMENT_TYPE_XLS ||
                             attachmentItem.Type == CNETConstantes.ATTACHMENT_TYPE_XLSX ||
                             attachmentItem.Type == CNETConstantes.ATTACHMENT_TYPE_CSV)
                    {
                        pictureEdit1.Visible = false;
                        richEditControl1.Visible = false;
                        pdfViewer1.Visible = false;
                        spreadsheetControl1.Visible = true;
                        axWindowsMediaPlayer1.Visible = false;
                        spreadsheetControl1.ResetText();
                        if (!string.IsNullOrEmpty(attachmentItem.Url))
                        {
                            if (attachmentItem.Type == CNETConstantes.ATTACHMENT_TYPE_XLSX)
                                spreadsheetControl1.LoadDocument(attachmentItem.url, DevExpress.Spreadsheet.DocumentFormat.OpenXml);

                            else if (attachmentItem.Type == CNETConstantes.ATTACHMENT_TYPE_XLS)
                                spreadsheetControl1.LoadDocument(attachmentItem.url, DevExpress.Spreadsheet.DocumentFormat.Xls);

                            else if (attachmentItem.Type == CNETConstantes.ATTACHMENT_TYPE_CSV)
                                spreadsheetControl1.LoadDocument(attachmentItem.url, DevExpress.Spreadsheet.DocumentFormat.Csv);
                            //goto here;
                        }
                    //    MemoryStream ms1 = new MemoryStream(attachmentItem.file);
                    //    if (ms1.Length == 0)
                    //    {
                    //        if (attachmentItem.type == CNETConstantes.ATTACHMENT_TYPE_XLSX)
                    //            spreadsheetControl1.LoadDocument(attachmentItem.url, DevExpress.Spreadsheet.DocumentFormat.OpenXml);

                    //        else if (attachmentItem.type == CNETConstantes.ATTACHMENT_TYPE_XLS)
                    //            spreadsheetControl1.LoadDocument(attachmentItem.url, DevExpress.Spreadsheet.DocumentFormat.Xls);

                    //        else if (attachmentItem.type == CNETConstantes.ATTACHMENT_TYPE_CSV)
                    //            spreadsheetControl1.LoadDocument(attachmentItem.url, DevExpress.Spreadsheet.DocumentFormat.Csv);

                    //        goto here;
                    //    }

                    //    if (attachmentItem.type == CNETConstantes.ATTACHMENT_TYPE_XLSX)
                    //        spreadsheetControl1.LoadDocument(ms1, DevExpress.Spreadsheet.DocumentFormat.OpenXml);

                    //    else if (attachmentItem.type == CNETConstantes.ATTACHMENT_TYPE_XLS)
                    //        spreadsheetControl1.LoadDocument(ms1, DevExpress.Spreadsheet.DocumentFormat.Xls);

                    //    else if (attachmentItem.type == CNETConstantes.ATTACHMENT_TYPE_CSV)
                    //        spreadsheetControl1.LoadDocument(ms1, DevExpress.Spreadsheet.DocumentFormat.Csv);

                    //here:
                    //    ;
                    }
                    else if (attachmentItem.Type == CNETConstantes.ATTACHMENT_TYPE_PDF)
                    {
                        pictureEdit1.Visible = false;
                        richEditControl1.Visible = false;
                        pdfViewer1.Visible = true;
                        spreadsheetControl1.Visible = false;
                        axWindowsMediaPlayer1.Visible = false;
                        if (!string.IsNullOrEmpty(attachmentItem.Url))
                        {
                            pdfViewer1.LoadDocument(attachmentItem.Url);
                            //goto here;
                        }
                    //    MemoryStream ms1 = new MemoryStream(attachmentItem.file);
                    //    if (ms1.Length == 0)
                    //    {
                    //        pdfViewer1.LoadDocument(attachmentItem.url);
                    //        goto here;
                    //    }
                    //    pdfViewer1.LoadDocument(ms1);

                    //here: ;
                    }
                    else if (attachmentItem.Type == CNETConstantes.ATTACHMENT_TYPE_VIDEO ||
                             attachmentItem.Type == CNETConstantes.ATTACHMENT_TYPE_AUDIO)
                    {
                        pictureEdit1.Visible = false;
                        richEditControl1.Visible = false;
                        pdfViewer1.Visible = false;
                        spreadsheetControl1.Visible = false;
                        axWindowsMediaPlayer1.Visible = true;
                        axWindowsMediaPlayer1.stretchToFit = true;
                        axWindowsMediaPlayer1.Dock = DockStyle.Fill;

                        axWindowsMediaPlayer1.URL = attachmentItem.Url;
                    }
                    else
                    {
                        pictureEdit1.Visible = true;
                        richEditControl1.Visible = false;
                        pdfViewer1.Visible = false;
                        spreadsheetControl1.Visible = false;
                        axWindowsMediaPlayer1.Visible = false;
                        pictureEdit1.Image = CNETAttachment.Properties.Resources.nopreview;
                    }
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show(ex.Message);
                }

            }


            CNETInfoReporter.WaitForm("Opening Attachment", "Please Wait..", 2, 2);
            CNETInfoReporter.Hide();
        }



        /// <summary>
        /// use this method for new attachment form for voucher attachment
        /// use this method only with voucher attachment constractor
        /// </summary>
        public void OpenNewAttachment(int type)
        {
            if (!string.IsNullOrEmpty(selectedAttachmentCatgory))
            {
                modalNewAtta = new modalNewAttachment(tempAttaReference, voucherFlag, selectedAttachmentCatgory, tempAtta, type, AttachmentIsFTP);
                modalNewAtta.AttachmentIsFTP = AttachmentIsFTP;
                modalNewAtta.AttachmentIsFTPTINNo = AttachmentIsFTPTINNo;
                modalNewAtta.AttachmentIsFTPRoom = AttachmentIsFTPIsRoom;
                modalNewAtta.AttachmentFTPRoomType = AttachmentFTPRoomType;
                modalNewAtta.AttachmentIsFTPOrganizationUnitDef = AttachmentIsFTPOrganizationUnitDef;
                modalNewAtta.bbiSave.ItemClick += SaveClicked;
                modalNewAtta.ShowDialog();
            }
            else
            {
                XtraMessageBox.Show("Please Select Attachment Catagory!", "CNET Attachment");
            }
        }

        private int getTreelistImageIndex(string attachmentLookUp)
        {
            int imageIndex = -1;
            if (!string.IsNullOrEmpty(attachmentLookUp))
            {
                switch (attachmentLookUp)
                {
                    case CNETConstantes.ATTACHMENT_CATAGORY_PERSONALPHOTO:
                        imageIndex = 4;
                        break;
                    case CNETConstantes.ATTACHMENT_CATAGORY_PASSPORT:
                        imageIndex = 3;
                        break;
                    case CNETConstantes.ATTACHMENT_CATAGORY_COMPANYLOGO:
                        imageIndex = 12;
                        break;
                    case CNETConstantes.ATTACHMENT_CATAGORY_REFERENCEDOCUMENTS:
                        imageIndex = 5;
                        break;
                    case CNETConstantes.ATTACHMENT_CATAGORY_CERTIFICATE:
                        imageIndex = 11;
                        break;
                    case CNETConstantes.ATTACHMENT_CATAGORY_LICENSE:
                        imageIndex = 10;
                        break;
                    case CNETConstantes.ATTACHMENT_CATAGORY_PICTURE:
                        imageIndex = 2;
                        break;
                    case CNETConstantes.ATTACHMENT_CATAGORY_CATALOGUE:
                        imageIndex = 0;
                        break;
                    case CNETConstantes.ATTACHMENT_CATAGORY_MANUAL:
                        imageIndex = 1;
                        break;

                }
            }
            return imageIndex;
        }


        private int getAttachmentIconIndex(string attachmentType)
        {
            int imageIndex = -1;
            if (!string.IsNullOrEmpty(attachmentType))
            {
                switch (attachmentType)
                {
                    case CNETConstantes.ATTACHMENT_TYPE_PICTURE:
                        imageIndex = 8;
                        break;
                    case CNETConstantes.ATTACHMENT_TYPE_VIDEO:
                        imageIndex = 9;
                        break;
                    case CNETConstantes.ATTACHMENT_TYPE_AUDIO:
                        imageIndex = 6;
                        break;
                    case CNETConstantes.ATTACHMENT_TYPE_TEXT:
                        imageIndex = 7;
                        break;
                    case CNETConstantes.ATTACHMENT_TYPE_EJ:
                        imageIndex = 7;
                        break;
                    case CNETConstantes.ATTACHMENT_TYPE_DOCX:
                        imageIndex = 7;
                        break;
                    case CNETConstantes.ATTACHMENT_TYPE_DOC:
                        imageIndex = 7;
                        break;
                    case CNETConstantes.ATTACHMENT_TYPE_XLS:
                        imageIndex = 7;
                        break;
                    case CNETConstantes.ATTACHMENT_TYPE_XLSX:
                        imageIndex = 7;
                        break;
                    case CNETConstantes.ATTACHMENT_TYPE_CSV:
                        imageIndex = 7;
                        break;
                    case CNETConstantes.ATTACHMENT_TYPE_PDF:
                        imageIndex = 7;
                        break;
                    case CNETConstantes.ATTACHMENT_TYPE_PPT:
                        imageIndex = 7;
                        break;
                    case CNETConstantes.ATTACHMENT_TYPE_PPTX:
                        imageIndex = 7;
                        break;

                }
            }
            return imageIndex;
        }

        private void exportFileToDefaultApp(int attachmentCode)
        {
            var attachmentItem = UIProcessManager.GetAttachmentById(attachmentCode);

            if (attachmentItem != null)
            {
                if (!string.IsNullOrEmpty(attachmentItem.Url))
                {
                    try
                    {
                        if (File.Exists(attachmentItem.Url))
                        {
                            System.Diagnostics.Process.Start(attachmentItem.Url);
                        }
                        else
                        {
                            XtraMessageBox.Show("Could not find file '" + attachmentItem.Url + "'!", "CNET Attachment", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    catch
                    {
                    }
                }
            }
        }

        #endregion

        private void treeList1_NodeCellStyle(object sender, GetCustomNodeCellStyleEventArgs e)
        {
            treeList1.OptionsBehavior.AutoNodeHeight = true;
            if (e.Node.ParentNode == null)
            {
                e.Appearance.Font = new System.Drawing.Font("Tahoma", 11);
            }
        }

        private void treeList1_FocusedNodeChanged(object sender, FocusedNodeChangedEventArgs e)
        {
            TreeListNode focusedNode = e.Node;
            if (focusedNode != null && focusedNode.ParentNode != null)
            {
                selectedAttachmentCatgory = null;
                selectedAttachmentCode = Convert.ToInt32( focusedNode.GetValue(treeList1.Columns[1].ToString()).ToString());
                if ( selectedAttachmentCode != null)
                {
                    if (voucherFlag)
                    {
                        if (tempAtta != null)
                        {
                            AttachmentDTO tempAtt = tempAtta.FirstOrDefault(x => x.Id == selectedAttachmentCode.Value);
                            showSelectedAttachment(tempAtt);
                        }
                    }
                    else
                        showSelectedAttachment(selectedAttachmentCode.Value);
                }
            }
            else if (focusedNode != null)
            {
                selectedAttachmentCatgory = focusedNode.GetValue(treeList1.Columns[1]).ToString();
                selectedAttachmentCode = null;
            }
        }

        private void treeList1_PopupMenuShowing(object sender, DevExpress.XtraTreeList.PopupMenuShowingEventArgs e)
        {
            TreeList tree = sender as TreeList;
            TreeListHitInfo info = tree.CalcHitInfo(e.Point);
            if (info.HitInfoType == HitInfoType.Cell)
            {
                if (info.Node.Level == 1)
                {
                    tree.FocusedNode = info.Node;
                    menuItems.Click -= ContextMenuClicked;
                    e.Menu.Items.Add(menuItems);
                    menuItems.Click += ContextMenuClicked;
                }
            }
        }

        private void ContextMenuClicked(object sender, EventArgs e)
        {
            try
            {
                var clickedItem = sender as DXMenuItem;
                var name = clickedItem.Caption;
                switch (name)
                {
                    case "Open":
                        exportFileToDefaultApp(selectedAttachmentCode.Value);
                        break;
                }
            }
            catch
            {

            }
        }

        private void btnDeleteAtt_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if ( selectedAttachmentCode != null)
                {
                    var attachmentItem = UIProcessManager.GetAttachmentById(selectedAttachmentCode.Value);
                    if (UIProcessManager.DeleteAttachmentById(selectedAttachmentCode.Value))
                    { 
                        if (AttachmentIsFTP && attachmentItem != null && !string.IsNullOrEmpty( attachmentItem.Url))
                        {
                            bool deleted = FTPAttachment.DeleteImageFromFTP(attachmentItem.Url);
                            showAttachement(reference);
                        }
                        else
                        {


                            if (voucherFlag)
                            {
                                if (tempAtta != null && tempAtta.Count > 0)
                                {
                                    tempAtta.Remove(tempAtta.FirstOrDefault(x => x.Id == selectedAttachmentCode.Value));
                                    showTempAttachments(tempAtta);
                                }
                            }
                            else
                            {
                                showAttachement(reference);
                            }
                        }
                    }
                }
                else
                {
                    XtraMessageBox.Show("Please Select Attachment To Delete!", "CNET_ERP2016", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }          
            }
            catch { }
        }

        

    }
  }

