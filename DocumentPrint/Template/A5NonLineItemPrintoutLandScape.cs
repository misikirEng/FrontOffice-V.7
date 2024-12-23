using DevExpress.DataAccess.ObjectBinding;
using DevExpress.XtraReports;
using DevExpress.XtraReports.UI;
using DevExpress.XtraSpreadsheet.Layout.Engine;
using DocumentPrint.DTO;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;

namespace DocumentPrint.Template
{
    public partial class A5NonLineItemPrintoutLandScape : DevExpress.XtraReports.UI.XtraReport
    {
        public A5NonLineItemPrintoutLandScape(HeaderDTO header, Image img = null)
        {
            InitializeComponent();

            #region logo 
            if (string.IsNullOrEmpty(header.paperType) == null || header.paperType != "Continuous")
            {

                if (DocumentPrintSetting.CompanyAttachmentLogo != null)
                {
                    try
                    {
                        xrPictureBox1.Image = DocumentPrintSetting.CompanyAttachmentLogo;
                        this.xrLabel8.Visible = false;
                    }
                    catch (Exception ex)
                    {
                        if (DocumentPrintSetting.CompanyName != null)
                        {
                            xrLabel8.Text = DocumentPrintSetting.CompanyName;
                            xrLabel8.Visible = true;
                        }
                    }
                }
                else
                {
                    if (DocumentPrintSetting.CompanyName != null)
                    {
                        xrLabel8.Text = DocumentPrintSetting.CompanyName;
                        xrLabel8.Visible = true;

                    }
                }

                if (img != null)
                {
                    //xrPictureBox2.back = img;
                    //xrPictureBox2.bac = 0;
                    //this.Watermark.Image = img;
                    //this.Watermark.TextTransparency = 113;
                    //this.Watermark.TextDirection = DevExpress.XtraPrinting.Drawing.DirectionMode.ForwardDiagonal; 
                    //this.Watermark.Font = new System.Drawing.Font("Verdana", 105, FontStyle.Bold);
                    //this.Watermark.ForeColor = Color.LightGray; 
                    //this.Watermark.ShowBehind = true;
                }
            }
            else
            {

                header.companyName = "";
                header.voucherType = "";
                xrLabel8.Text = "";
                //label8.Visible = false;
                //label9.Visible = false;
                //xrLine5.Visible = false;
                headerTbl.Visible = false;
                xrPictureBox1.Visible = false;
                xrLabel17.Visible = false;
                lblVoucherDef.Text = "";
                xrLine1.Visible = false;
                //xrLabel1.Visible = false;
                xrRefTable.Visible  = true; 
            }
            #endregion
            #region journals
            var JournalDetailView = header.JournalDetail;
            bool showJournals = header.PrintJournal;
            if (showJournals)
            {
                var d = JournalDetailView;
                var c = 0;
                if (d != null && d?.Count > 0)
                {
                    foreach (var jd in d)
                    {
                        c += 1;
                        XRTableRow xrow = new XRTableRow();

                        XRTableCell xcelAcctCode = new XRTableCell();
                        xcelAcctCode.Text = jd.Account;
                        xcelAcctCode.WidthF = 89.54f;
                        xcelAcctCode.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
                        xcelAcctCode.Font = new System.Drawing.Font("Times New Roman", 8F, System.Drawing.FontStyle.Regular);
                        xrow.Cells.Add(xcelAcctCode);

                        XRTableCell xcelAcctDesc = new XRTableCell();
                        xcelAcctDesc.Text = jd.Description;
                        xcelAcctDesc.WidthF = 175.05f;
                        xcelAcctDesc.CanGrow = false;
                        xcelAcctDesc.WordWrap = false;
                        xcelAcctDesc.Font = new System.Drawing.Font("Times New Roman", 8F, System.Drawing.FontStyle.Regular);
                        xcelAcctDesc.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
                        xrow.Cells.Add(xcelAcctDesc);

                        XRTableCell xcelAcctDebit = new XRTableCell();
                        xcelAcctDebit.Text = String.Format("{0:N}", jd.Debit);
                        xcelAcctDebit.WidthF = 83.19f;
                        xcelAcctDebit.Font = new System.Drawing.Font("Times New Roman", 8F, System.Drawing.FontStyle.Regular);
                        xcelAcctDebit.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
                        xcelAcctDebit.Borders = DevExpress.XtraPrinting.BorderSide.Right | ((c == d?.Count) ? DevExpress.XtraPrinting.BorderSide.Bottom : 0);
                        xcelAcctDebit.BorderColor = Color.Black;
                        xcelAcctDebit.BorderDashStyle = DevExpress.XtraPrinting.BorderDashStyle.Solid;
                        xcelAcctDebit.BorderWidth = 1;
                        xrow.Cells.Add(xcelAcctDebit);

                        XRTableCell xcelAcctCredit = new XRTableCell();
                        xcelAcctCredit.Text = String.Format("{0:N}", jd.Credit);
                        xcelAcctCredit.WidthF = 83.19f;
                        xcelAcctCredit.Font = new System.Drawing.Font("Times New Roman", 8F, System.Drawing.FontStyle.Regular);
                        xcelAcctCredit.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
                        xrow.Cells.Add(xcelAcctCredit);
                        if (c == d?.Count)
                        {
                            xrow.Borders = DevExpress.XtraPrinting.BorderSide.Bottom;
                            xrow.BorderColor = Color.Black;
                            xrow.BorderDashStyle = DevExpress.XtraPrinting.BorderDashStyle.Solid;
                            xrow.BorderWidth = 1;
                        }
                        journalTbl.Rows.Add(xrow);
                    }
                }
                else
                {
                    journalTbl.Rows.RemoveAt(0);
                }
            }
            else
            {
                journalTbl.Rows.RemoveAt(0);
            }
            #endregion
            #region payment Method
            if (header.payment_method == "" || header.payment_method == null) { payementMethodTbl.Rows.RemoveAt(0); }
            #endregion
            #region reference
            if (header.RefNo != null && header.RefNo != "")
            {
                var count = header.RefNo.Split(',');
                if (count.Length > 3)
                {
                    headerTbl.Rows.RemoveAt(2);
                }
                else
                {
                    refTbl.Rows.RemoveAt(0);
                }
            }
            else
            {
                refTbl.Rows.RemoveAt(0);
            }
            #endregion
            #region operators
            if (header.VoucheroperatorsString == "" || header.VoucheroperatorsString == null)
            {
                label8.Dispose();
                label9.Dispose();
            }
            #endregion
            #region withholding and income Tax
            if (header.withHoldingAmount != 0 || header.incomeAmount != 0)
            {
                if (header.withHoldingAmount == 0)
                {
                    withholdingTbl.Rows.RemoveAt(0);
                }
                if (header.incomeAmount == 0)
                {
                    IncomeTaxTbl.Rows.RemoveAt(0);
                }
                if (header.voucherDefinition == 136 || header.voucherDefinition == 137)
                {
                    totalPaymentLabelCell.Text = "Total Payment : ";
                    totalPaymentCell.Text = String.Format("{0:N}", header.grandTotal);
                }
                else
                {
                    totalPaymentLabelCell.Text = "Net Payment : ";
                    totalPaymentCell.Text = (header.grandTotal - header.withHoldingAmount - header.incomeAmount).ToString();
                }
            }
            else
            {
                withholdingTbl.Rows.RemoveAt(0);
                IncomeTaxTbl.Rows.RemoveAt(0);
                netPaymentTbl.Rows.RemoveAt(0);
            }
            #endregion

            objectDataSource1.DataSource = header;
        }
    }
}
