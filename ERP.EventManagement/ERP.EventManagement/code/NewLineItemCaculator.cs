using ProcessManager;
using CNET_V7_Domain.Domain.ArticleSchema;
using CNET_V7_Domain.Domain.CommonSchema;
using CNET_V7_Domain.Domain.SettingSchema;
using CNET_V7_Domain.Domain.TransactionSchema;
using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EventManagement
{
    public class NewLineItemCaculator
    {
        #region New Voucher

        private ArticleDTO article = new ArticleDTO(); 
        public LineItemDetailPMS LineItemDetailCalculatorVoucher(VoucherBuffer newbufferVoucher, LineItemDTO lineitem, int? regCode,
            int? discountVFD = null, int? additionalVFD = null, decimal? additionalChargeParam = null, decimal? discountParam = null, bool priceExtracted = false,
            bool scIncluded = false, bool isPercentAddCharge = false, bool isPercentDiscount = false)
        { 

            List<PreferentialValueFactorDTO> preferentialValueFactor = new List<PreferentialValueFactorDTO>();
            LineItemDetailPMS lineItemDetail = new LineItemDetailPMS();
            lineItemDetail.lineItemValueFactor = new List<LineItemValueFactorDTO>();
            lineItemDetail.lineItem = new LineItemDTO();
            TaxDTO tax = new TaxDTO();
            preferentialValueFactor = newbufferVoucher.PreferentialValueFactors.ToList();

            article = UIProcessManager.GetArticleById(lineitem.Article);
            lineItemDetail.articleName = article.Name;
            SystemConstantDTO ArticleUOM = LocalBuffer.LocalBuffer.SystemConstantDTOBufferList.FirstOrDefault(b => b.Id == article.Uom);
            if (ArticleUOM != null)
            {
                lineItemDetail.uom = ArticleUOM.Description;
            }
            lineItemDetail.lineItem.Note = lineitem.Note;
            lineItemDetail.lineItem.Article = article.Id;
            lineItemDetail.lineItem.Voucher = newbufferVoucher.Voucher.Id;
            lineItemDetail.lineItem.ObjectState =null;
            List<ValueFactorDefinitionDTO> valueFactorDefinitions = new List<ValueFactorDefinitionDTO>();
            tax = GetApplicableTax(regCode, newbufferVoucher.Voucher.Definition, newbufferVoucher.Voucher.Consignee1, lineitem.Article);
            if (!string.IsNullOrEmpty(tax.Remark))
            {
                XtraMessageBox.Show(tax.Remark, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
            //if (tax.code != 0)
            //{
            if (tax.Remark == null || tax.Remark == "")
            {
                if (EventVoucherSetting.ValueIsTaxInclusive == true && priceExtracted == false)
                {
                    lineItemDetail.lineItem.UnitAmount = lineitem.UnitAmount / (1 + Convert.ToDecimal(tax.Amount) / 100);//Extract tax
                    lineItemDetail.lineItem.UnitAmount = RoundValue(lineItemDetail.lineItem.UnitAmount, EventVoucherSetting.UnitPriceRoundDigit);
                }
                else
                {
                    lineItemDetail.lineItem.UnitAmount = lineitem.UnitAmount;
                    lineItemDetail.lineItem.UnitAmount = RoundValue(lineItemDetail.lineItem.UnitAmount, EventVoucherSetting.UnitPriceRoundDigit);
                }
                lineItemDetail.lineItem.Quantity = lineitem.Quantity;
                lineItemDetail.lineItem.Quantity = RoundValue(Convert.ToDecimal(lineItemDetail.lineItem.Quantity), EventVoucherSetting.QuantityRoundDigit);

                lineItemDetail.lineItem.TotalAmount = Convert.ToDecimal( lineItemDetail.lineItem.Quantity) * lineItemDetail.lineItem.UnitAmount;
                lineItemDetail.lineItem.TotalAmount = RoundValue(lineItemDetail.lineItem.TotalAmount, EventVoucherSetting.TotalAmountRoundDigit);


                //Discount

                if (discountParam == null)
                {
                    if (preferentialValueFactor == null || preferentialValueFactor.Count == 0)
                    {
                        //value factor from value factor setting
                        valueFactorDefinitions.AddRange(GetValueFactorDefnsFromSetting(newbufferVoucher.Voucher, lineitem, "discount"));
                    }
                    else
                    {
                        valueFactorDefinitions.AddRange(GetValueFactorDefnFromCategory(preferentialValueFactor, CNETConstantes.DISCOUNT));
                    }
                }
                else
                {
                    if (discountParam.Value > 0)
                    {
                        //use the passed discount parameter
                        valueFactorDefinitions.AddRange(GetValueFactorDefnFromParam(discountParam.Value, discountVFD, "discount"));
                    }
                }


                //Additional Charge
                if (additionalChargeParam == null)
                {
                    if (preferentialValueFactor == null || preferentialValueFactor.Count == 0)
                    {
                        //value factor from valuefacto setting
                        valueFactorDefinitions.AddRange(GetValueFactorDefnsFromSetting(newbufferVoucher.Voucher, lineitem, "service"));
                    }
                    else
                    {
                        valueFactorDefinitions.AddRange(GetValueFactorDefnFromCategory(preferentialValueFactor, CNETConstantes.ADDTIONAL_CHARGE));
                    }
                }
                else
                {
                    if (additionalChargeParam.Value > 0)
                    {
                        valueFactorDefinitions.AddRange(GetValueFactorDefnFromParam(additionalChargeParam.Value, additionalVFD, "additionalCharge"));
                    }
                }
                decimal valueFactorNetEffect = 0;
                decimal ADDTIONAL_CHARGE = 0;
                decimal DISCOUNT = 0;
                if (valueFactorDefinitions != null && valueFactorDefinitions.Count > 0)
                {
                    foreach (var valueFactorDefn in valueFactorDefinitions)
                    {
                        if (valueFactorDefn.Type == CNETConstantes.ADDTIONAL_CHARGE && scIncluded) //This will extract the additional charge from the unit value from the lineItem.
                        {
                            lineItemDetail.lineItem.UnitAmount = lineItemDetail.lineItem.UnitAmount / Convert.ToDecimal((1 + (valueFactorDefn.Value / 100)));
                            lineItemDetail.lineItem.TotalAmount = Convert.ToDecimal(lineitem.Quantity) * lineItemDetail.lineItem.UnitAmount;
                        }
                        LineItemValueFactorDTO lineItemValueFactor = new LineItemValueFactorDTO();
                        lineItemValueFactor.LineItem = lineitem.Id;
                        lineItemValueFactor.ValueFactor = valueFactorDefn.Id;
                        if (valueFactorDefn.Type == CNETConstantes.DISCOUNT)
                        {
                            if (valueFactorDefn.IsPercent)
                            {
                                lineItemValueFactor.Amount = lineItemDetail.lineItem.TotalAmount * Convert.ToDecimal((valueFactorDefn.Value / 100));
                            }
                            else
                            {
                                lineItemValueFactor.Amount = (decimal)valueFactorDefn.Value;
                            }
                            lineItemValueFactor.IsDiscount = true;
                            valueFactorNetEffect -= lineItemValueFactor.Amount.Value;
                            if (lineItemValueFactor.Amount > 0)
                            {
                                DISCOUNT += lineItemValueFactor.Amount.Value;
                                lineItemDetail.lineItemValueFactor.Add(lineItemValueFactor);
                            }
                        }
                        if (valueFactorDefn.Type == CNETConstantes.ADDTIONAL_CHARGE)
                        {
                            if (valueFactorDefn.IsPercent)
                            {
                                lineItemValueFactor.Amount = (decimal)lineItemDetail.lineItem.TotalAmount * Convert.ToDecimal((valueFactorDefn.Value / 100));

                            }
                            else
                            {
                                lineItemValueFactor.Amount = (decimal)valueFactorDefn.Value;
                            }
                            lineItemValueFactor.IsDiscount = false;
                            valueFactorNetEffect += lineItemValueFactor.Amount.Value;
                            if (lineItemValueFactor.Amount > 0)
                            {
                                ADDTIONAL_CHARGE += lineItemValueFactor.Amount.Value;
                                lineItemDetail.lineItemValueFactor.Add(lineItemValueFactor);
                            }
                        }
                    }


                }
                decimal taxableAmount = (decimal)lineItemDetail.lineItem.TotalAmount + valueFactorNetEffect;
                lineItemDetail.lineItem.Tax = tax.Id;
                lineItemDetail.lineItem.TaxableAmount = taxableAmount;
                lineItemDetail.lineItem.Discount = DISCOUNT;
                lineItemDetail.lineItem.AddCharge = ADDTIONAL_CHARGE;
                lineItemDetail.lineItem.TaxAmount = RoundValue(taxableAmount * (Convert.ToDecimal(tax.Amount) / 100), EventVoucherSetting.TotalAmountRoundDigit);
            }
            else
            {
                XtraMessageBox.Show(tax.Remark, "Lineitem Calculator Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            //}
            //else
            //{
            //    XtraMessageBox.Show("Tax Not Defined", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}
            return lineItemDetail;
        }


        public decimal RoundValue(decimal number, int digit)
        {
            decimal roundValue = 0;
            string format = String.Format("{{0:f{0}}}", digit);
            roundValue = Convert.ToDecimal(string.Format(format, number));

            return roundValue;
        }
        private List<ValueFactorDefinitionDTO> GetValueFactorDefnsFromSetting(VoucherDTO currentVoucherBuffer, LineItemDTO currentLineItem, string type)
        {

            List<ValueFactorDefinitionDTO> valueFactorDefinitions = new List<ValueFactorDefinitionDTO>();
            VoucherDTO valueFactorvoucher = new VoucherDTO();
            string valueFactorType = null;

            if (type == "discount")
            {
                if (EventVoucherSetting.ApplicableDiscount != null)
                {
                    valueFactorType = EventVoucherSetting.ApplicableDiscount;
                }
                else
                {
                    XtraMessageBox.Show("Applicable Discount Setting is Not Defined!", "Applicable Discount Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                if (EventVoucherSetting.ApplicableAddtiCharge != null)
                {
                    valueFactorType = EventVoucherSetting.ApplicableAddtiCharge;
                }
                else
                {
                    XtraMessageBox.Show("Applicable Additional Charge Setting is Not Defined!", "Applicable Additional Charge Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            switch (valueFactorType.ToLower())
            {
                case "notapplicable":
                    break;

                case "article":
                    ValueFactorDefinitionDTO articleValueFactorDefns = new ValueFactorDefinitionDTO();
                    List<ValueFactorDTO> articleValueFactor = UIProcessManager.GetValueFactorByreference(currentLineItem.Article).ToList();

                    if (articleValueFactor != null && articleValueFactor.Count > 0)
                    {
                        foreach (var vf in articleValueFactor)
                        {
                            if (vf.ValueFactorDefinition != null)
                            {

                                ValueFactorDefinitionDTO currVdf = UIProcessManager.GetValueFactorDefinitionById(vf.ValueFactorDefinition.Value);

                                if (type == "discount")
                                {
                                    if (currVdf.Type == CNETConstantes.DISCOUNT)
                                    {
                                        articleValueFactorDefns = currVdf;
                                        valueFactorDefinitions.Add(articleValueFactorDefns);
                                    }
                                }
                                else
                                {
                                    if (currVdf.Type == CNETConstantes.ADDTIONAL_CHARGE)
                                    {
                                        articleValueFactorDefns = currVdf;
                                        valueFactorDefinitions.Add(articleValueFactorDefns);
                                    }
                                }
                            }
                        }
                    }
                    break;

                case "consignee":
                    ValueFactorDefinitionDTO consigneeValueFactorDefns = new ValueFactorDefinitionDTO();
                    if (currentVoucherBuffer.Consignee1 != null)
                    {
                        List<ValueFactorDTO> consigneeValueFactor = UIProcessManager.GetValueFactorByreference(currentVoucherBuffer.Consignee1.Value).ToList();

                        if (consigneeValueFactor != null && consigneeValueFactor.Count > 0)
                        {
                            foreach (var vf in consigneeValueFactor)
                            {
                                if (vf.ValueFactorDefinition != null)
                                {
                                    ValueFactorDefinitionDTO currVdf = UIProcessManager.GetValueFactorDefinitionById(vf.ValueFactorDefinition.Value);
                                    if (type == "discount")
                                    {
                                        if (currVdf != null)
                                        {
                                            if (currVdf.Type == CNETConstantes.DISCOUNT)
                                            {
                                                consigneeValueFactorDefns = currVdf;
                                                valueFactorDefinitions.Add(consigneeValueFactorDefns);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (currVdf != null)
                                        {
                                            if (currVdf.Type == CNETConstantes.ADDTIONAL_CHARGE)
                                            {
                                                consigneeValueFactorDefns = currVdf;
                                                valueFactorDefinitions.Add(consigneeValueFactorDefns);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    break;

                case "term":
                    break;
            }

            return valueFactorDefinitions;
        }

        private List<ValueFactorDefinitionDTO> GetValueFactorDefnFromCategory(List<PreferentialValueFactorDTO> preferentialValueFactors, int type)
        {
            List<ValueFactorDefinitionDTO> valueFactorDefns = new List<ValueFactorDefinitionDTO>();
            PreferenceDTO parentPreference = new PreferenceDTO();

            parentPreference = LocalBuffer.LocalBuffer.PreferenceBufferList.FirstOrDefault(c => c.Id == article.Preference);

            PreferentialValueFactorDTO preferenceValueF = preferentialValueFactors.FirstOrDefault(c => c.Preference == article.Preference || c.Preference == parentPreference.ParentId);

            if (preferenceValueF != null)
            {
                ValueFactorDefinitionDTO prefValueFactorDfn = new ValueFactorDefinitionDTO();

                prefValueFactorDfn = UIProcessManager.GetValueFactorDefinitionById(preferenceValueF.ValueFactorDefn.Value);

                if (prefValueFactorDfn != null)
                {
                    prefValueFactorDfn.Value = preferenceValueF.Amount.Value;

                    valueFactorDefns.Add(prefValueFactorDfn);
                }
            }
            return valueFactorDefns;

        }

        private List<ValueFactorDefinitionDTO> GetValueFactorDefnFromParam(decimal discountParam, int? discountValueFactDefn, string type)
        {
            List<ValueFactorDefinitionDTO> valueFactorDefns = new List<ValueFactorDefinitionDTO>();
            ValueFactorDefinitionDTO valueFactorDefinition = new ValueFactorDefinitionDTO();
            int? typeFactor = null;
            if (type == "discount")
            {
                typeFactor = CNETConstantes.DISCOUNT;
            }
            else
            {
                typeFactor = CNETConstantes.ADDTIONAL_CHARGE;
            }

            valueFactorDefinition = UIProcessManager.GetValueFactorDefinitionById(discountValueFactDefn.Value);
            if (valueFactorDefinition != null)
            {
                valueFactorDefinition.IsPercent = false;
                valueFactorDefinition.Value =(decimal) discountParam;
                valueFactorDefns.Add(valueFactorDefinition);
            }
            return valueFactorDefns;
        }


        public static TaxDTO GetApplicableTax(int? regVoucher, int voucherDef, int? guestCode, int? articleCode)
        {
            VoucherDTO oc = null; int? companyCode = null;
            if (regVoucher != null)
           oc = UIProcessManager.GetVoucherById(regVoucher.Value);

            if (oc != null)
               companyCode = oc.Consignee2 == null ? null : oc.Consignee2;


            TaxDTO tax = null;
            TaxDTO taxToReturn = new TaxDTO()
            {
                Id = -1,
                Category = 0,
                Description = "",
                Amount = 0,
                Remark = ""
            };

            ConfigurationDTO config = LocalBuffer.LocalBuffer.ConfigurationBufferList.FirstOrDefault(c =>
                c.Reference == voucherDef.ToString() && c.Attribute.ToLower() == "tax priority");
            if (config == null)
            {
                taxToReturn.Remark = "Unable to get tax priority setting! voucher Def: " + voucherDef;
                return taxToReturn;

            }
            if (config.CurrentValue.ToLower() == "voucher")
            {
                ConfigurationDTO voucherTaxType = LocalBuffer.LocalBuffer.ConfigurationBufferList.FirstOrDefault(c =>
                c.Reference == voucherDef.ToString() && c.Attribute.ToLower() == "voucher tax type");
                if (voucherTaxType != null)
                {
                    tax = LocalBuffer.LocalBuffer.TaxBufferList.FirstOrDefault(t => t.Description == voucherTaxType.CurrentValue);
                }

                if (tax != null)
                {
                    taxToReturn.Id = tax.Id;
                    taxToReturn.Category = tax.Category;
                    taxToReturn.Description = tax.Description;
                    taxToReturn.Amount = tax.Amount;
                    taxToReturn.Remark = null;
                }
                else
                {
                    taxToReturn.Remark = "Voucher's Tax Type is not defined! Voucher Def: " + voucherDef;
                }

                return taxToReturn;
            }
            else if (config.CurrentValue.ToLower() == "consignee")
            {
                taxToReturn.Remark = "Voucher Tax Priority should be set to Voucher not Consignee Please Fix Your Setting";
                /*
                if (!string.IsNullOrEmpty(companyCode))
                {
                    GSLTax gslTax = LocalBuffer.LocalBuffer.GSLTaxBufferList.FirstOrDefault(g => g.reference == companyCode);
                    if (gslTax != null)
                    {
                        tax = LocalBuffer.LocalBuffer.TaxBufferList.FirstOrDefault(t => t.code == gslTax.tax);

                    }
                }
                else if (!string.IsNullOrEmpty(guestCode))
                {
                    GSLTax gslTax = LocalBuffer.LocalBuffer.GSLTaxBufferList.FirstOrDefault(g => g.reference == guestCode);
                    if (gslTax != null)
                    {
                        tax = LocalBuffer.LocalBuffer.TaxBufferList.FirstOrDefault(t => t.code == gslTax.tax);

                    }
                }
                

                //if tax for both consignee is not defined, take the vouchers tax type
                if (tax == null)
                {
                    Configuration voucherTaxType = LocalBuffer.LocalBuffer.ConfigurationBufferList.FirstOrDefault(c =>
                    c.reference == voucherDef.ToString() && c.attribute.ToLower() == "voucher tax type");
                    if (voucherTaxType != null)
                    {
                        tax = LocalBuffer.LocalBuffer.TaxBufferList.FirstOrDefault(t => t.description == voucherTaxType.currentValue);
                    }
                }

                if (tax != null)
                {
                    taxToReturn.code = tax.code;
                    taxToReturn.category = tax.category;
                    taxToReturn.description = tax.description;
                    taxToReturn.amount = tax.amount;
                    taxToReturn.remark = null;
                }
                else
                {
                    taxToReturn.remark = "Consignee tax type is not defined! Voucher Def: " + voucherDef;
                }
                */
                return taxToReturn;
            }
            else if (config.CurrentValue.ToLower() == "article")
            {
                if (articleCode != null && articleCode > 0)
                {
                    ArticleDTO articleddt = UIProcessManager.GetArticleById(articleCode.Value);
                    if (articleddt != null)
                    {
                        if (articleddt.DefaultTax != null)
                        {

                            tax = LocalBuffer.LocalBuffer.TaxBufferList.FirstOrDefault(t => t.Id == articleddt.DefaultTax);
                        }
                        else
                        {
                            GsltaxDTO gslTax = UIProcessManager.GetGSLTaxByReference(articleCode.Value);
                            if (gslTax != null)
                            {
                                tax = LocalBuffer.LocalBuffer.TaxBufferList.FirstOrDefault(t => t.Id == gslTax.Tax);

                            }

                        }

                    }
                    else
                    {
                        taxToReturn.Remark = "Article Can't be found " + articleCode.Value;

                    }

                }

                if (tax != null)
                {
                    taxToReturn.Id = tax.Id;
                    taxToReturn.Category = tax.Category;
                    taxToReturn.Description = tax.Description;
                    taxToReturn.Amount = tax.Amount;
                    taxToReturn.Remark = null;
                }
                else
                {
                    taxToReturn.Remark = "Article Tax Type is not defined! Voucher Def: " + voucherDef;
                }

                return taxToReturn;
            }
            else
            {
                //take the voucher's tax type
                ConfigurationDTO voucherTaxType = LocalBuffer.LocalBuffer.ConfigurationBufferList.FirstOrDefault(c =>
                c.Reference == voucherDef.ToString() && c.Attribute.ToLower() == "voucher tax type");
                if (voucherTaxType != null)
                {
                    tax = LocalBuffer.LocalBuffer.TaxBufferList.FirstOrDefault(t => t.Description == voucherTaxType.CurrentValue);
                }


                if (tax != null)
                {
                    taxToReturn.Id = tax.Id;
                    taxToReturn.Category = tax.Category;
                    taxToReturn.Description = tax.Description;
                    taxToReturn.Amount = tax.Amount;
                    taxToReturn.Remark = null;
                }
                else
                {
                    taxToReturn.Remark = "Voucher's Tax Type is not defined! Voucher Def: " + voucherDef;
                }

                return taxToReturn;

            }



        }


        #endregion
    }
}
