using CNET_V7_Domain.Domain.SettingSchema;
using CNET_V7_Domain.Domain.TransactionSchema;
using DevExpress.DataAccess.ObjectBinding;
using DevExpress.Utils.About;
using DevExpress.XtraReports.UI;
using DocumentPrint.DTO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Drawing;
using ProcessManager;

namespace DocumentPrint.Grid
{
    public partial class A4LineItemPrintout : DevExpress.XtraReports.UI.XtraReport
    {
        public A4LineItemPrintout(VoucherPrintModel voucherModel)
        {  // Data Source=VoucherPrintModel 
            // Data Mermber= ListLineItemObj
            InitializeComponent();
            #region parameters
            companyName.Value = voucherModel?.CompanyName;
            tinNo.Value = voucherModel?.TINNo;
            vatNo.Value = voucherModel?.VATNo;
            tel.Value = voucherModel?.CompanyTel;
            web.Value = voucherModel?.CompanyWeb;
            fax.Value = voucherModel?.CompanyFax;
            pobox.Value = voucherModel?.CompanyPOBox;
            email.Value = voucherModel?.CompanyEmail;

            lblFSNo.Text = voucherModel?.voucherExtensionString; 
            #endregion
            try
            {
                #region logo 
                if (DocumentPrintSetting.CompanyAttachmentLogo != null)
                {
                    try
                    {
                        logoPictureBox.Image = DocumentPrintSetting.CompanyAttachmentLogo;
                        this.xrLblCompanyName.Visible = false;

                    }
                    catch (Exception ex)
                    {
                        if (DocumentPrintSetting.CompanyName != null)
                        {
                            xrLblCompanyName.Text = DocumentPrintSetting.CompanyName;
                            xrLblCompanyName.Visible = true;
                        }
                    }
                }
                else
                {
                    if (DocumentPrintSetting.CompanyName != null)
                    {
                        xrLblCompanyName.Text = DocumentPrintSetting.CompanyName;
                        xrLblCompanyName.Visible = true;
                    }
                }
                #endregion

                #region lineItem
                var sn = 1;
                List<ArticleObjsPrint> articleObjsList = voucherModel.ListLineItemObj;
                voucherModel.ListLineItemObj = articleObjsList;
                #endregion

                #region voucher Operator
                if (voucherModel?.VoucherUserOrientation == "Horizontal")
                {
                    operatorCellFirst.Text = voucherModel.VoucheroperatorsString;
                }
                else if (voucherModel.VoucherUserOrientation == "Vertical")
                {
                    if (voucherModel.ActivityDefDesc?.Count > 0)
                    {
                        var opereratorString = new List<string>();
                        var operarators = voucherModel.Voucheroperators;
                        var activityDesc = voucherModel.ActivityDefDesc;
                        var activityDate = voucherModel.ActivityDate;
                        for (int z = 0; z < voucherModel.ActivityDefDesc.Count; z++)
                        {
                            string oppText = "";
                            oppText += voucherModel.ActivityDefDesc[z];
                            oppText += " by ";
                            oppText += voucherModel.Voucheroperators[z];
                            oppText += " on " + activityDate[z];
                            oppText += "__________";
                            opereratorString.Add(oppText);
                        }

                        opereratorString = opereratorString.Where(x => x != "").ToList();
                        if (opereratorString.Count > 0)
                        {
                            for (var x = 0; x < opereratorString.Count; x++)
                            {

                                XRTableRow xrow = new XRTableRow();
                                XRTableCell op = new XRTableCell();
                                op.Font = new System.Drawing.Font("Tahoma", 9);
                                op.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
                                if (x == 0)
                                {
                                    operatorCellFirst.Text = opereratorString[x];
                                }
                                else
                                {
                                    op.Text = opereratorString[x];
                                    xrow.Cells.Add(op);
                                    operatorTable.Rows.Add(xrow);
                                }

                            }
                        }
                    }
                }
                #endregion operator 

                #region tax region

                var voucherValue = voucherModel?.voucherValues;
                if (voucherModel.PrintValues == null)
                {
                    voucherModel.PrintValues = "All";
                }
                if (voucherValue != null && (voucherModel.PrintValues.ToLower() == "all" || voucherModel.PrintValues.ToLower() == "subtotalonly"))
                {
                    subtotalCell.Text = voucherValue.SubTotal.ToString("N2");
                    if (voucherModel.PrintValues.ToLower() == "all")
                    {
                        if (voucherValue?.AdditionalCharge != 0)
                        {
                            var additionalChargeLablel = "Additional Charge";
                            if (voucherValue?.SubTotal != 0)
                            {
                                try
                                {
                                    double discount = (double)(voucherValue?.AdditionalCharge / voucherValue?.SubTotal) * 100.00;
                                    additionalChargeLablel += string.Format(" ({0}%)", discount.ToString("N2"));
                                }
                                catch { }
                            }
                            additionalChargeLbl.Text = additionalChargeLablel;
                            additionalChargeCell.Text = voucherValue.AdditionalCharge.ToString("N2");
                        }
                        else
                        {
                            additionalChargeTbl.Rows.RemoveAt(0);
                        }

                        if (string.IsNullOrEmpty(voucherValue?.Remark))
                        {
                            if (voucherValue?.Discount != 0)
                            {
                                var disCountLabel = "Discount";
                                if (voucherValue?.SubTotal != 0)
                                {
                                    double discount = (double)(voucherValue?.Discount / voucherValue?.SubTotal) * 100.00;
                                    disCountLabel += string.Format(" ({0}%)", discount.ToString("N2"));
                                }
                                discountLbl.Text = disCountLabel;
                                discountCell.Text = voucherValue.Discount.ToString("N2");
                            }
                            else
                            {
                                discountTbl.Rows.RemoveAt(0);
                            }
                        }
                        else
                        {
                            if (voucherValue?.Discount != 0)
                            {
                                var disCountLabel = "Discount";
                                //Lookup lookup = LoginPage.Authentication.LookupBufferList.Where(l => l.code == voucherValue.Remark).FirstOrDefault();
                                //if (lookup != null)
                                //{
                                //    disCountLabel = lookup.description;
                                //}
                                discountLbl.Text = disCountLabel;
                                discountLbl.Text = voucherValue.Discount.ToString("N2");
                            }
                            else
                            {
                                discountTbl.Rows.RemoveAt(0);
                            }

                        }

                        List<TaxTransactionDTO> taxTransactionList = voucherValue?.VoucherTax;
                        var taxBuffer = voucherModel.TaxBuffer;
                        TaxDTO TypeName = new TaxDTO();

                        if (taxTransactionList?.Count > 0)
                        {
                            for (int m = 0; m <= taxTransactionList.Count() - 1; m++)
                            {
                                string taxString = "";
                                decimal? taxableAmount = 0;
                                decimal? taxAmount = 0;
                                if (taxTransactionList[m].Tax == 6)
                                {
                                    // WithHoldingTax = Math.Round(taxTransactionList[m].taxAmount, 2);

                                }
                                else
                                {
                                    TypeName = taxBuffer.Where(x => x.Id == voucherValue.VoucherTax[m].Tax).FirstOrDefault();
                                    if (TypeName != null)
                                    {
                                        taxString = TypeName.Description;
                                        if (TypeName.Amount != 0)
                                        {
                                            taxString += " (" + TypeName.Amount != null ? Math.Round((decimal)TypeName.Amount, 2) : 0 + "%)";
                                        }
                                    }
                                    taxAmount = taxTransactionList[m].TaxAmount;
                                    taxableAmount = taxTransactionList[m].TaxableAmount;

                                    if (m == 0)
                                    {
                                        taxStringCell.Text = taxString;
                                        taxableAmountCell.Text = String.Format("{0:N}", taxableAmount);
                                        taxAmountCell.Text = String.Format("{0:N}", taxAmount);
                                    }
                                    else
                                    {

                                        XRTableRow xrow = new XRTableRow();

                                        XRTableCell taxNameCell = new XRTableCell();
                                        taxNameCell.Text = taxString;
                                        taxNameCell.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
                                        taxNameCell.Font = new System.Drawing.Font("Arial", 9, System.Drawing.FontStyle.Bold);
                                        taxNameCell.Borders = DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Bottom;
                                        taxNameCell.BorderColor = Color.Silver;
                                        taxNameCell.BorderDashStyle = DevExpress.XtraPrinting.BorderDashStyle.Solid;
                                        taxNameCell.BorderWidth = 1;
                                        taxNameCell.WidthF = 116.65F;
                                        taxNameCell.CanGrow = true;
                                        xrow.Cells.Add(taxNameCell);

                                        XRTableCell taxableAmountCell = new XRTableCell();
                                        taxableAmountCell.Text = String.Format("{0:N}", taxableAmount);
                                        taxableAmountCell.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
                                        taxableAmountCell.Font = new System.Drawing.Font("Arial", 9, System.Drawing.FontStyle.Bold);
                                        taxableAmountCell.Borders = DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Bottom;
                                        taxableAmountCell.BorderColor = Color.Silver;
                                        taxableAmountCell.BorderDashStyle = DevExpress.XtraPrinting.BorderDashStyle.Solid;
                                        taxableAmountCell.BorderWidth = 1;
                                        taxableAmountCell.WidthF = 91.55F;
                                        taxableAmountCell.CanGrow = true;
                                        xrow.Cells.Add(taxableAmountCell);

                                        XRTableCell taxAmountCell = new XRTableCell();
                                        taxAmountCell.Text = String.Format("{0:N}", taxAmount);
                                        taxAmountCell.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
                                        taxAmountCell.Font = new System.Drawing.Font("Arial", 9, System.Drawing.FontStyle.Bold);
                                        taxAmountCell.Borders = DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Bottom | DevExpress.XtraPrinting.BorderSide.Right;
                                        taxAmountCell.BorderColor = Color.Silver;
                                        taxAmountCell.BorderDashStyle = DevExpress.XtraPrinting.BorderDashStyle.Solid;
                                        taxAmountCell.BorderWidth = 1;
                                        taxAmountCell.WidthF = 124.95F;
                                        taxAmountCell.CanGrow = true;
                                        xrow.Cells.Add(taxAmountCell);

                                        taxTable2.Rows.Add(xrow);
                                    }

                                }

                            }
                        }
                        else
                        {
                            taxTable2.Rows.RemoveAt(0);
                        }
                        grandTotalValue.Text = voucherValue.GrandTotal.ToString("N2");
                        //mTextToPrint = "Grand Total";
                        //e.Graphics.DrawString(PrintDocumentVoucher.VoucherValues.GrandTotal.ToString(PrintDocumentVoucher.RoundDigitTotal), mFont, Brushes.Black, mHorizontalPosition + 782 - PrintDocumentVoucher.Right, mVerticalPosition + 5, mDrawFormat);

                    }
                    else
                    {
                        additionalChargeTbl.Rows.RemoveAt(0);
                        discountTbl.Rows.RemoveAt(0);
                        taxTable2.Rows.RemoveAt(0);
                        groundTotalTbl.Rows.RemoveAt(0);
                    }

                }
                else if (voucherModel?.PrintValues?.ToLower() == "none")
                {
                    tableCell17.Dispose();
                    tableCell13.Dispose();
                    tableCell23.Dispose();
                    tableCell24.Dispose();

                    subtotalTbl.Rows.RemoveAt(0);
                    additionalChargeTbl.Rows.RemoveAt(0);
                    discountTbl.Rows.RemoveAt(0);
                    taxTable2.Rows.RemoveAt(0);
                    groundTotalTbl.Rows.RemoveAt(0);
                }
                else
                {
                    subtotalTbl.Rows.RemoveAt(0);
                    additionalChargeTbl.Rows.RemoveAt(0);
                    discountTbl.Rows.RemoveAt(0);
                    taxTable2.Rows.RemoveAt(0);
                    groundTotalTbl.Rows.RemoveAt(0);
                }

                #endregion

                #region otherConsignee 

                if (voucherModel.OtherConsigneeDetail != null && voucherModel.OtherConsigneeDetail.Count > 0)
                {
                    // var otherConsDetailList = voucherModel.ConsigneeDetail;
                    var consigneeUnitBuffer = voucherModel.ConsigneeUnitBuffer;
                    var i = 0;
                    foreach (var _consignee in voucherModel.OtherConsigneeDetail)
                    {
                        var OtherConsigneeTaxId = "";
                        var OtherConsigneeAdressExtended = "";
                        var otherConsDesc = _consignee.requiredGSlDesc;
                        if (_consignee != null)
                        {
                            if (i == 0)
                            {
                                otherConsDesc += ":   " + _consignee.consigneeFullName;
                            }
                            else
                            {
                                otherConsDesc = _consignee.consigneeFullName;
                            }
                            OtherConsigneeTaxId = _consignee.consigneTin;
                            OtherConsigneeTaxId = "TIN : " + OtherConsigneeTaxId;
                            var consigneeUnit = consigneeUnitBuffer.Where(x => x.Consignee == _consignee.consignee && x.Abrivation == "HO")?.FirstOrDefault();
                            if (consigneeUnit?.PoBox != null)
                            {
                                OtherConsigneeAdressExtended += "POBox " + consigneeUnit?.PoBox;
                            }
                            if (consigneeUnit?.AddressLine1 != null)
                            {
                                OtherConsigneeAdressExtended += "Tel. " + consigneeUnit?.AddressLine1;
                            }
                            if (consigneeUnit?.HouseNumber != null)
                            {
                                OtherConsigneeAdressExtended += "H No. " + consigneeUnit?.HouseNumber;
                            }
                            if (consigneeUnit?.AddressLine2 != null)
                            {
                                OtherConsigneeAdressExtended += "Mob. " + consigneeUnit?.AddressLine2;
                            }

                        }
                        OtherConsigneeAdressExtended = "Address: " + OtherConsigneeAdressExtended;
                        if (i == 0)
                        {
                            lblCons1.Text = otherConsDesc;
                            cons1Tin.Text = OtherConsigneeTaxId.Replace("TIN : ", "");
                            cons1Address.Text = OtherConsigneeAdressExtended.Replace("Address: ", "");
                            otherConsTable.Visible = false;
                        }
                        else if (i == 1)
                        {
                            otherConsCell1.Text = otherConsDesc;
                            otherConsCell2.Text = OtherConsigneeTaxId;
                            otherConsCell3.Text = OtherConsigneeAdressExtended;
                            otherConsTable.Visible = true;
                        }
                        else
                        {
                            otherConsTable.Visible = true;
                            XRTableRow xrow = new XRTableRow();

                            XRTableCell otherConsNameCell = new XRTableCell();
                            otherConsNameCell.Text = otherConsDesc;
                            otherConsNameCell.TextAlignment = DevExpress.XtraPrinting.TextAlignment.BottomLeft;
                            otherConsNameCell.Font = new System.Drawing.Font("Tahoma", 9, System.Drawing.FontStyle.Regular);
                            otherConsNameCell.Borders = DevExpress.XtraPrinting.BorderSide.None;
                            otherConsNameCell.CanGrow = false;
                            otherConsNameCell.WordWrap = false;
                            otherConsNameCell.TextTrimming = StringTrimming.Character;
                            otherConsNameCell.CanShrink = false;
                            otherConsNameCell.WidthF = 318.35F;
                            otherConsNameCell.Multiline = true;
                            xrow.Cells.Add(otherConsNameCell);

                            XRTableCell otherConsTinCell = new XRTableCell();
                            otherConsTinCell.Text = OtherConsigneeTaxId;
                            otherConsTinCell.TextAlignment = DevExpress.XtraPrinting.TextAlignment.BottomLeft;
                            otherConsTinCell.Font = new System.Drawing.Font("Tahoma", 9, System.Drawing.FontStyle.Regular);
                            otherConsTinCell.Borders = DevExpress.XtraPrinting.BorderSide.None;
                            otherConsTinCell.CanGrow = true;
                            otherConsTinCell.WidthF = 120.83F;
                            xrow.Cells.Add(otherConsTinCell);

                            XRTableCell otherConsAddressCell = new XRTableCell();
                            otherConsAddressCell.Text = OtherConsigneeAdressExtended;
                            otherConsAddressCell.TextAlignment = DevExpress.XtraPrinting.TextAlignment.BottomLeft;
                            otherConsAddressCell.Font = new System.Drawing.Font("Tahoma", 9, System.Drawing.FontStyle.Regular);
                            otherConsAddressCell.Borders = DevExpress.XtraPrinting.BorderSide.None;
                            otherConsAddressCell.CanGrow = false;
                            otherConsAddressCell.WordWrap = false;
                            otherConsAddressCell.TextTrimming = StringTrimming.Character;
                            otherConsAddressCell.CanShrink = false;
                            otherConsAddressCell.WidthF = 300.92F;
                            otherConsAddressCell.Multiline = true;
                            xrow.Cells.Add(otherConsAddressCell);
                            otherConsTable.Rows.Add(xrow);


                        }
                        i++;
                    }
                }
                else
                {
                    otherConsTable.Rows.RemoveAt(0);
                    panel3.Visible = false;
                    panel3.HeightF = 0f;
                    PageHeader.HeightF = PageHeader.HeightF - 40f;
                }
                #endregion

                #region quantity sum
                if (voucherModel.PrintQuantitySum)
                {
                    decimal quantitySum = 0;
                    if (voucherModel.ListLineItemObj != null && voucherModel.ListLineItemObj.Count > 0)
                    {
                        voucherModel.ListLineItemObj.ForEach(x => quantitySum += x.Quantity);
                    }
                    quanSum.Text = quantitySum.ToString("0.00");
                    quanSum.Visible = true;
                }
                else
                {
                    quanSum.Visible = false;
                }
                #endregion

                #region grandtotalInwords
                if (string.IsNullOrWhiteSpace(voucherModel?.GrandTotalInWords))
                {
                    label4.Visible = false;
                }
                #endregion

                #region witholding
                if (voucherModel.withHoldingAmount != 0)
                {
                    netPaymentVal.Text = string.Format("{0:N}", (voucherValue.GrandTotal - voucherModel.withHoldingAmount));
                }
                else
                {
                    withholdinglbl.Visible = false;
                    netPaymentLbl.Visible = false;
                    withholdingVal.Visible = false;
                    netPaymentVal.Visible = false;
                    withholdinglbl.Dispose();
                    netPaymentLbl.Dispose();
                    withholdingVal.Dispose();
                    netPaymentVal.Dispose();
                }
                #endregion

            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("");

            }
            objectDataSource1.DataSource = voucherModel;
            objectDataSource2.DataSource = voucherModel.ListLineItemObj;


        }
    }
}
