using DevExpress.DataAccess.ObjectBinding;
using DevExpress.XtraReports.UI;
using DocumentPrint.DTO;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;

namespace DocumentPrint.Template
{
    public partial class A4NonLineItemPrintout : DevExpress.XtraReports.UI.XtraReport
    {
        public A4NonLineItemPrintout(HeaderDTO header)
        {
            InitializeComponent();

            #region logo 
            if (DocumentPrintSetting.CompanyAttachmentLogo != null)
            {
                try
                {
                    pictureBox1.Image = DocumentPrintSetting.CompanyAttachmentLogo;
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
            #endregion
            //#region consignee
            //if (dataSource.voucherDefinition == 137 | dataSource.voucherDefinition == 143)
            //{
            //    consCell.Text = "Received From: ";
            //}
            //else if (dataSource.voucherDefinition == 135
            //   || dataSource.voucherDefinition == 136
            //    || dataSource.voucherDefinition == 141 ||
            //    dataSource.voucherDefinition == 144)
            //{
            //    consCell.Text = "Pay To: ";
            //}
            //else if (dataSource.voucherDefinition == 139)
            //{
            //    consCell.Text = "Debit To: ";
            //}
            //else if (dataSource.voucherDefinition == 159)
            //{
            //    consCell.Text = "Credit To: ";
            //}
            //else if (dataSource.voucherDefinition == 109)
            //{
            //    consCell.Text = "Refund To: ";
            //}
            //else
            //{
            //    consCell.Text = LoginPage.Authentication.RequiredGSLBufferList.Where(re => re.voucherDefn == dataSource.voucherDefinition && re.type == "LKUP000000900").FirstOrDefault().description;
            //}
            //#endregion
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
                        xcelAcctDebit.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
                        xcelAcctDebit.Borders = DevExpress.XtraPrinting.BorderSide.Right | ((c == d?.Count) ? DevExpress.XtraPrinting.BorderSide.Bottom : 0);
                        xcelAcctDebit.BorderColor = Color.Black;
                        xcelAcctDebit.BorderDashStyle = DevExpress.XtraPrinting.BorderDashStyle.Solid;
                        xcelAcctDebit.BorderWidth = 1;
                        xrow.Cells.Add(xcelAcctDebit);

                        XRTableCell xcelAcctCredit = new XRTableCell();
                        xcelAcctCredit.Text = String.Format("{0:N}", jd.Credit);
                        xcelAcctCredit.WidthF = 83.19f;
                        xcelAcctCredit.Font = new System.Drawing.Font("Times New Roman", 8F, System.Drawing.FontStyle.Regular);
                        xcelAcctCredit.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
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
            #region payment Method
            if (header.payment_method == "" || header.payment_method == null) { payementMethodTbl.Rows.RemoveAt(0); }
            #endregion

            objectDataSource1.DataSource = header;

        }
    }
}
