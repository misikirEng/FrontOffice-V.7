using CNET.FrontOffice_V._7.Forms.Non_Navigatable_Modals;
using CNET_V7_Domain;
using CNET_V7_Domain.Domain.SettingSchema;
using CNET_V7_Domain.Domain.TransactionSchema;
using CNET_V7_Domain.Domain.ViewSchema;
using CNET_V7_Domain.Misc.CommonTypes;
using CNET_V7_Domain.Misc.PmsDTO;
using DevExpress.DataAccess.Native.Sql;
using DevExpress.XtraEditors;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports;
using DevExpress.XtraSpreadsheet.PrintLayoutEngine;
using DocumentPrint.DTO;
using DocumentPrint.Grid;
using DocumentPrint.Template;
using ProcessManager;
using System;
using System.Collections.Generic; 
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms; 
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace DocumentPrint
{
    public class ReportGenerator
    {
        PrintDialog pd { get; set; }
        DocumentPrint.Forms.PDFViewer viewer { get; set; }

        #region Voucher Attachmanet

        public Image GetAttachamentImage(int voucherDefinition, int PrintCount, string PrintWaterMark)
        {
            if (PrintWaterMark == "Standard")
            {
                if (PrintCount == 0)
                    return DocumentPrint.Properties.Resources.Original;
                else if (PrintCount == 1)
                    return DocumentPrint.Properties.Resources._1st_COPY;
                else if (PrintCount == 2)
                    return DocumentPrint.Properties.Resources._2nd_COPY;
                else if (PrintCount == 3)
                    return DocumentPrint.Properties.Resources._3rd_COPY;
                else if (PrintCount == 4)
                    return DocumentPrint.Properties.Resources._4th_COPY;
                else if (PrintCount == 5)
                    return DocumentPrint.Properties.Resources._5th_COPY;
                else if (PrintCount == 6)
                    return DocumentPrint.Properties.Resources._6th_COPY;
                else if (PrintCount == 7)
                    return DocumentPrint.Properties.Resources._7th_COPY;
                else if (PrintCount == 8)
                    return DocumentPrint.Properties.Resources._8th_COPY;
                else if (PrintCount == 9)
                    return DocumentPrint.Properties.Resources._9th_COPY;
                else if (PrintCount == 10)
                    return DocumentPrint.Properties.Resources._10th_COPY;
                else if (PrintCount == 11)
                    return DocumentPrint.Properties.Resources._11th_COPY;
                else if (PrintCount == 12)
                    return DocumentPrint.Properties.Resources._12th_COPY;
                else
                    return DocumentPrint.Properties.Resources.Attachment;
            }
            else
                return DocumentPrint.Properties.Resources.Attachment;
        }
        public async void GetAttachementReport(string VoucherId)
        {
            VoucherDTO voucher = UIProcessManager.GetVoucherByCode(VoucherId);
            if (voucher != null)
            {
                GetAttachementReport(voucher.Id);
            }
        }

        public async void GetAttachementReport(int VoucherId)
        {
            DocumentPrintSetting.CheckAndGetDefaultData();

            pd = new PrintDialog();

            VoucherDetailDTO voucherDetail = UIProcessManager.get_voucher_detail(VoucherId);
            if (voucherDetail == null)
                return;

            SystemConstantDTO voucherdef = DocumentPrintSetting.SystemConstantList.FirstOrDefault(x => x.Id == voucherDetail.VoucherHeader.DefinitionId);


            PrintDocumentVoucher PrintDocument = new PrintDocumentVoucher();

            // Line Item Printout
            VoucherPrintModel VoucherPrint = null;
            if (string.IsNullOrEmpty(voucherdef.Category) || voucherdef.Category.ToLower().Trim() == "lineitem")
            {
                VoucherPrint = await PrintDocument.PrintLineItemVoucher(voucherDetail);
                if (VoucherPrint != null)
                {
                    if (VoucherPrint.PaperSize == "A4")
                    {
                        A4LineItemPrintout A4LineItem = new A4LineItemPrintout(VoucherPrint);
                        A4LineItem.RequestParameters = false;

                        A4LineItem.Watermark.Image = GetAttachamentImage(VoucherPrint.voucherDefinition, VoucherPrint.PrintCount, VoucherPrint.PrintWaterMark);

                        A4LineItem.Watermark.TextTransparency = 113;
                        A4LineItem.Watermark.TextDirection = DevExpress.XtraPrinting.Drawing.DirectionMode.ForwardDiagonal;
                        A4LineItem.Watermark.Font = new System.Drawing.Font("Verdana", 105, FontStyle.Bold);
                        A4LineItem.Watermark.ForeColor = Color.LightGray;
                        A4LineItem.Watermark.PageRange = "all";
                        A4LineItem.Watermark.ShowBehind = true;

                        pd.PrintReport(A4LineItem, VoucherPrint);
                        pd.ShowDialog();
                    }
                    else
                    {
                        A5LineItemPrintOut A5LineItem = new A5LineItemPrintOut(VoucherPrint);
                        A5LineItem.RequestParameters = false;
                        A5LineItem.ParameterPanelLayoutItems.Clear();

                        A5LineItem.Watermark.Image = GetAttachamentImage(VoucherPrint.voucherDefinition, VoucherPrint.PrintCount, VoucherPrint.PrintWaterMark);
                        A5LineItem.Watermark.TextTransparency = 113;
                        A5LineItem.Watermark.TextDirection = DevExpress.XtraPrinting.Drawing.DirectionMode.ForwardDiagonal;
                        A5LineItem.Watermark.Font = new System.Drawing.Font("Verdana", 105, FontStyle.Bold);
                        A5LineItem.Watermark.ImageViewMode = DevExpress.XtraPrinting.Drawing.ImageViewMode.Zoom;
                        A5LineItem.Watermark.ForeColor = Color.LightGray;
                        A5LineItem.Watermark.PageRange = "all";
                        A5LineItem.Watermark.ShowBehind = true;


                        pd.PrintReport(A5LineItem, VoucherPrint);
                        pd.ShowDialog();
                    }

                }
                else
                {
                    XtraMessageBox.Show("Fail to get Voucher Data for Printout !!!");

                }



            }
            // Non Line Item Printout
            HeaderDTO VoucherPrintNonLineItem = null;
            if (string.IsNullOrEmpty(voucherdef.Category) || voucherdef.Category.ToLower().Trim() == "non-lineitem")
            {
                VoucherPrintNonLineItem = await PrintDocument.PrintNoneLineItemVoucher(voucherDetail);

                if (VoucherPrintNonLineItem != null)
                {

                    if (voucherdef.Id == 197)
                    {
                        bool watermark = true;
                        if ((string.IsNullOrEmpty(VoucherPrintNonLineItem.waterMark) && VoucherPrintNonLineItem.waterMark == "Custom"))
                            watermark = false;

                        RecieptReport reciept = new RecieptReport(VoucherPrintNonLineItem, watermark);

                        if (watermark)
                        {
                            reciept.Watermark.Image = DocumentPrint.Properties.Resources.CashRecipt;
                            reciept.Watermark.ImageTransparency = 30;
                            reciept.Watermark.TextDirection = DevExpress.XtraPrinting.Drawing.DirectionMode.ForwardDiagonal;
                            reciept.Watermark.Font = new System.Drawing.Font("Verdana", 105, FontStyle.Bold);
                            reciept.Watermark.ForeColor = Color.LightGray;
                            reciept.Watermark.PageRange = "all";
                            reciept.Watermark.ShowBehind = true;
                        }
                        pd.PrintReport(reciept, null, false, null, VoucherPrintNonLineItem);
                        pd.ShowDialog();
                        return;
                    }

                    if (VoucherPrintNonLineItem.PaperSize == "A4")
                    {
                        A4NonLineItemPrintout A4NonLineItem = new A4NonLineItemPrintout(VoucherPrintNonLineItem);


                        A4NonLineItem.Watermark.Image = GetAttachamentImage(VoucherPrintNonLineItem.voucherDefinition, VoucherPrintNonLineItem.PrintCount, VoucherPrintNonLineItem.waterMark);
                        A4NonLineItem.Watermark.TextTransparency = 113;
                        A4NonLineItem.Watermark.TextDirection = DevExpress.XtraPrinting.Drawing.DirectionMode.ForwardDiagonal;
                        A4NonLineItem.Watermark.Font = new System.Drawing.Font("Verdana", 105, FontStyle.Bold);
                        A4NonLineItem.Watermark.ForeColor = Color.LightGray;
                        A4NonLineItem.Watermark.PageRange = "all";
                        A4NonLineItem.Watermark.ShowBehind = true;


                        pd.PrintReport(A4NonLineItem, null, false, null, VoucherPrintNonLineItem);
                        pd.ShowDialog();
                    }
                    else
                    {
                        if (VoucherPrintNonLineItem.VoucherOrientation == "Portrait")
                        {
                            A5NonLineItemPrintout A5NonLineItem = new A5NonLineItemPrintout(VoucherPrintNonLineItem);


                            A5NonLineItem.Watermark.Image = GetAttachamentImage(VoucherPrintNonLineItem.voucherDefinition, VoucherPrintNonLineItem.PrintCount, VoucherPrintNonLineItem.waterMark);
                            A5NonLineItem.Watermark.Image = DocumentPrint.Properties.Resources.Attachment;
                            A5NonLineItem.Watermark.TextTransparency = 113;
                            A5NonLineItem.Watermark.TextDirection = DevExpress.XtraPrinting.Drawing.DirectionMode.ForwardDiagonal;
                            A5NonLineItem.Watermark.Font = new System.Drawing.Font("Verdana", 105, FontStyle.Bold);
                            A5NonLineItem.Watermark.ForeColor = Color.LightGray;
                            A5NonLineItem.Watermark.PageRange = "all";
                            A5NonLineItem.Watermark.ShowBehind = true;


                            pd.PrintReport(A5NonLineItem, null, false, null, VoucherPrintNonLineItem);
                            pd.ShowDialog();
                        }
                        else
                        {
                            A5NonLineItemPrintoutLandScape A5NonLineItemLandScape = new A5NonLineItemPrintoutLandScape(VoucherPrintNonLineItem);


                            A5NonLineItemLandScape.Watermark.Image = GetAttachamentImage(VoucherPrintNonLineItem.voucherDefinition, VoucherPrintNonLineItem.PrintCount, VoucherPrintNonLineItem.waterMark);
                            A5NonLineItemLandScape.Watermark.TextTransparency = 113;
                            A5NonLineItemLandScape.Watermark.TextDirection = DevExpress.XtraPrinting.Drawing.DirectionMode.ForwardDiagonal;
                            A5NonLineItemLandScape.Watermark.Font = new System.Drawing.Font("Verdana", 105, FontStyle.Bold);
                            A5NonLineItemLandScape.Watermark.ForeColor = Color.LightGray;
                            A5NonLineItemLandScape.Watermark.ImageViewMode = DevExpress.XtraPrinting.Drawing.ImageViewMode.Zoom;
                            A5NonLineItemLandScape.Watermark.PageRange = "all";
                            A5NonLineItemLandScape.Watermark.ShowBehind = true;


                            pd.PrintReport(A5NonLineItemLandScape, null, false, null, VoucherPrintNonLineItem);
                            pd.ShowDialog();
                        }
                    }

                }
                else
                {
                    XtraMessageBox.Show("Fail to get Voucher Data for Printout !!!");
                }
            }
        }
        #endregion

        #region Grid
        public void GenerateGridReport(IPrintable control1, string ReportName, string Datetime, string Peroid = "", string Account = "")
        {
            DocumentPrintSetting.CheckAndGetDefaultData();

            pd = new PrintDialog();

            GridPrint report = new GridPrint(control1, ReportName, Datetime, Peroid, Account);

            pd.PrintReport(report);
            pd.ShowDialog();

        }

        public void GenerateGridAndChartReport(IPrintable control1, IPrintable control2, string ReportName, DateTime Date)
        {
            DocumentPrintSetting.CheckAndGetDefaultData();
            pd = new PrintDialog();
            GridandChart report = new GridandChart(control1, control2, ReportName, Date);
            pd.PrintReport(report);
            pd.ShowDialog();
        }
        #endregion

        #region PMS
        public void GenerateGuestLedgerReport(IPrintable control1, IPrintable control2, IPrintable control3, LedgerObjects objects)
        {
            DocumentPrintSetting.CheckAndGetDefaultData();

            pd = new PrintDialog();
            GuestLedgerReport report = new GuestLedgerReport(control1, control2, control3);
            report.LedgerParameters(objects.CustomerName, objects.CompanyName, objects.CompanyTin, objects.RegistrationNumber, objects.Plan, objects.FsNo, objects.TINNo, objects.ArrivalDate, objects.DepartureDate, objects.ConsigneeUnit, objects.User);
            report.RoomChargeParameters(objects.SubTotal, objects.Discount, objects.ServiceCharge, objects.Vat, objects.GrandTotal);
            report.ExtraBillsHistoryParameters(objects.TotalOtherBill);
            pd.ConsigneeId = objects.ConsigneeId;
            pd.DocumentName = "Guest Ledger";
            report.PaymentHistoryParameters(objects.TotalPaid, objects.TotalCredit, objects.Refund, objects.RemainingBalance);
            report.Watermark.Image = DocumentPrint.Properties.Resources.NonFisical;
            report.Watermark.ImageAlign = ContentAlignment.MiddleCenter;
            report.Watermark.ImageTiling = false;
            report.Watermark.ImageTransparency = 70;
            report.Watermark.PageRange = "all";
            report.Watermark.ShowBehind = true;
            pd.PrintReport(report);
            pd.ShowDialog();
        }

        public void GeneratePackagePrintout(IPrintable control1, string Name, string Datetime, RegistrationListVMDTO CurrentRegistration)
        {
            pd = new PrintDialog();
            PackagePrintout report = new PackagePrintout(control1, Name, Datetime, CurrentRegistration);
            pd.PrintReport(report);
            pd.ShowDialog();
        }

        public void GenerateGuestLedgerRoomCharge(IPrintable control1, LedgerObjects objects)
        {
            DocumentPrintSetting.CheckAndGetDefaultData();

            pd = new PrintDialog();
            GuestLedgerRoomCharge report = new GuestLedgerRoomCharge(control1);
            report.RoomChargeParameters(objects.SubTotal, objects.Discount, objects.ServiceCharge, objects.Vat, objects.GrandTotal);
            report.LedgerParameters(objects.HeaderText, objects.CustomerName, objects.CompanyName, objects.CompanyTin, objects.RegistrationNumber, objects.Plan, objects.FsNo, objects.TINNo, objects.ArrivalDate, objects.DepartureDate, objects.ConsigneeUnit, objects.User);


            pd.DocumentName = "Guest Ledger Room Charge";
            report.Watermark.Image = DocumentPrint.Properties.Resources.Attachment;
            report.Watermark.ImageAlign = ContentAlignment.MiddleCenter;
            report.Watermark.ImageTiling = false;
            report.Watermark.ImageTransparency = 70;
            report.Watermark.PageRange = "all";
            report.Watermark.ShowBehind = true;

            pd.ConsigneeId = objects.ConsigneeId;
            pd.DocumentName = objects.HeaderText;
            pd.PrintReport(report);
            pd.ShowDialog();

        }

        public void GetPMSConformationAttachement(int voucherid, String UserName)
        {
            DocumentPrintSetting.CheckAndGetDefaultData();

            RegistrationPrintOutDTO registrationPrintOut = UIProcessManager.GetRegistrationConformation(voucherid);

            VoucherDTO vo = UIProcessManager.GetVoucherById(voucherid);

            pd = new PrintDialog();
            PMSConformation ConformationDetail = new PMSConformation(registrationPrintOut, UserName, vo.LastState);
            pd.PrintReport(ConformationDetail);
            pd.ShowDialog();
        }

        public void GetPMSRegistrationCard(int voucherid, bool ontab = false)
        {
            VoucherDTO Registrationvoucher = UIProcessManager.GetVoucherById(voucherid);
            MemoryStream filestream = null;
            bool Exist = FTPInterface.FTPAttachment.InitalizePMSFTPAttachment(LocalBuffer.LocalBuffer.CompanyConsigneeData.Tin);
            if (Exist)
            {
                FTPInterface.FTPAttachment.ORGUnitDefcode = Registrationvoucher.OriginConsigneeUnit.ToString();
                filestream = FTPInterface.FTPAttachment.GetFileStreamFromFTP(Registrationvoucher.Code, CNETConstantes.REGISTRATION_VOUCHER);
            }

            if (filestream != null)
            {
                CheckPdfFile(filestream, ontab);
            }
            else if (Registrationvoucher != null && (string.IsNullOrEmpty(Registrationvoucher.DefaultImageUrl) || !File.Exists(Registrationvoucher.DefaultImageUrl)))
            {
                DocumentPrintSetting.CheckAndGetDefaultData();

                RegistrationPrintOutDTO registrationPrintOut = UIProcessManager.GetRegistrationConformation(voucherid);

                pd = new PrintDialog();




              ConfigurationDTO regOrientation =  DocumentPrintSetting.ConfigurationDTOList.FirstOrDefault(x=> x.Reference == CNETConstantes.REGISTRATION_VOUCHER.ToString() && x.Attribute== "Voucher Orientation");
                if (regOrientation == null || regOrientation.CurrentValue == "Portrait")
                {
                    PMSRegistration ConformationDetail = new PMSRegistration(registrationPrintOut, null);
                    pd.BranchId = Registrationvoucher.OriginConsigneeUnit;
                    pd.PrintReport(ConformationDetail, null, ontab, registrationPrintOut);
                }
                else
                {
                    PMSRegistrationLandscape ConformationDetaillandscape = new PMSRegistrationLandscape(registrationPrintOut, null);
                    pd.BranchId = Registrationvoucher.OriginConsigneeUnit;
                    pd.PrintReport(ConformationDetaillandscape, null, ontab, registrationPrintOut, null, false);
                }

                if (ontab)
                    SetFormLocation(pd);
             
                pd.ShowDialog();

            }
            //else
            //{
            //    if (File.Exists(Registrationvoucher.DefaultImageUrl))
            //    {
            //        CheckPdfFile(Registrationvoucher.DefaultImageUrl, ontab);
            //    }
            //}
        }

        public void CloseTabViewer()
        {
            if (viewer != null)
                viewer.Close();
            else if (pd != null)
                pd.Close();
        }

        private void SetFormLocation(Form form)
        {
            Screen screen = Screen.PrimaryScreen;

            if (ScribbleWinTab.TabInfo.IsTabAvailable() && !string.IsNullOrEmpty(ScribbleWinTab.TabInfo.GetDeviceInfo()))
            {
                form.StartPosition = FormStartPosition.Manual;
                // form.FormBorderStyle = FormBorderStyle.None;
                form.WindowState = FormWindowState.Maximized;

                Screen[] screens = Screen.AllScreens;

                screen = screens.FirstOrDefault(x=> !x.Primary);
                if (screen == null) return;
                //int count = screens.Length;
                //if (screens.Length > 1)
                //{
                //    screen = screens[count - 1];
                //}
                Rectangle bound = screen.Bounds;
                form.SetBounds(bound.X, bound.Y, bound.Width, bound.Height);
                form.TopMost = true;
            }
            else
            {
                screen = Screen.PrimaryScreen;
                form.WindowState = FormWindowState.Normal;
                form.StartPosition = FormStartPosition.CenterParent;
                form.AutoScroll = true;
                Rectangle r = screen.Bounds;
                r.Inflate(-120, -50);
                form.Bounds = form.RectangleToClient(r);
                // form.AutoSizeMode = (AutoSizeMode.GrowAndShrink);
            }

        }
        #endregion



        #region PDF
        private bool CheckPdfFile(MemoryStream filestream, bool displayOnTablet = false)
        {
            bool fileExists = false;
            viewer = new Forms.PDFViewer();
            viewer.filestream = filestream;
            Screen screen = Screen.PrimaryScreen;
            if (viewer.OpenFile())
            {
                if (ScribbleWinTab.TabInfo.IsTabAvailable() && !string.IsNullOrEmpty(ScribbleWinTab.TabInfo.GetDeviceInfo()) && displayOnTablet)
                {
                    viewer.StartPosition = FormStartPosition.Manual;
                    // form.FormBorderStyle = FormBorderStyle.None;
                    viewer.WindowState = FormWindowState.Maximized;

                    Screen[] screens = Screen.AllScreens;


                    screen = screens.FirstOrDefault(x => !x.Primary);
                    if (screen == null) return false;
                    //int count = screens.Length;
                    //if (screens.Length > 1)
                    //{
                    //    screen = screens[count - 1];
                    //}
                    Rectangle bound = screen.Bounds;
                    viewer.SetBounds(bound.X, bound.Y, bound.Width, bound.Height);
                    viewer.TopMost = true;
                    viewer.ShowDialog();
                }
                else
                {
                    viewer.StartPosition = FormStartPosition.CenterParent;
                    viewer.ShowDialog();
                }
            }
            fileExists = true;

            return fileExists;
        }
        private bool CheckPdfFile(string attachmentpath, bool displayOnTablet = false)
        {
            bool fileExists = false;
            if (File.Exists(attachmentpath))
            {
                viewer = new Forms.PDFViewer();
                viewer.filePath = attachmentpath;
                Screen screen = Screen.PrimaryScreen;
                if (viewer.OpenFile())
                {
                    if (ScribbleWinTab.TabInfo.IsTabAvailable() && !string.IsNullOrEmpty(ScribbleWinTab.TabInfo.GetDeviceInfo()) && displayOnTablet)
                    {
                        viewer.StartPosition = FormStartPosition.Manual;
                        // form.FormBorderStyle = FormBorderStyle.None;
                        viewer.WindowState = FormWindowState.Maximized;

                        Screen[] screens = Screen.AllScreens;

                        screen = screens.FirstOrDefault(x => !x.Primary);
                        if (screen == null) return false;
                        //int count = screens.Length;
                        //if (screens.Length > 1)
                        //{
                        //    screen = screens[count - 1];
                        //}
                        Rectangle bound = screen.Bounds;
                        viewer.SetBounds(bound.X, bound.Y, bound.Width, bound.Height);
                        viewer.TopMost = true;
                        viewer.ShowDialog();
                    }
                    else
                    {
                        viewer.StartPosition = FormStartPosition.CenterParent;
                        viewer.ShowDialog();
                    }
                }
                fileExists = true;
            }
            return fileExists;
        }
        #endregion



    }
}
