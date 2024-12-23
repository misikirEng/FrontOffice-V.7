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

namespace CNET.FrontOffice_V._7.PMS.Common_Classes
{
    public class NewLineItemCaculator
    {
        #region New Voucher

        private ArticleDTO article = new ArticleDTO(); 
        public LineItemDetailPMS LineItemDetailCalculatorVoucher(VoucherBuffer newbufferVoucher, LineItemDTO lineitem, int regCode,
            int? discountVFD = null, int? additionalVFD = null, double? additionalChargeParam = null, double? discountParam = null, bool priceExtracted = false,
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
            lineItemDetail.lineItem.Article = article.Id;
            lineItemDetail.lineItem.Voucher = newbufferVoucher.Voucher.Id;
            lineItemDetail.lineItem.ObjectState =null;
            List<ValueFactorDefinitionDTO> valueFactorDefinitions = new List<ValueFactorDefinitionDTO>();
            tax = CommonLogics.GetApplicableTax(regCode, newbufferVoucher.Voucher.Definition, newbufferVoucher.Voucher.Consignee1, lineitem.Article);
            if (!string.IsNullOrEmpty(tax.Remark))
            {
                XtraMessageBox.Show(tax.Remark, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
            //if (tax.code != 0)
            //{
            if (tax.Remark == null || tax.Remark == "")
            {
                if (PMSVoucherSetting.ValueIsTaxInclusive == true && priceExtracted == false)
                {
                    lineItemDetail.lineItem.UnitAmount = lineitem.UnitAmount / (1 + Convert.ToDecimal(tax.Amount) / 100);//Extract tax
                    lineItemDetail.lineItem.UnitAmount = RoundValue(lineItemDetail.lineItem.UnitAmount, PMSVoucherSetting.UnitPriceRoundDigit);
                }
                else
                {
                    lineItemDetail.lineItem.UnitAmount = lineitem.UnitAmount;
                    lineItemDetail.lineItem.UnitAmount = RoundValue(lineItemDetail.lineItem.UnitAmount, PMSVoucherSetting.UnitPriceRoundDigit);
                }
                lineItemDetail.lineItem.Quantity = lineitem.Quantity;
                lineItemDetail.lineItem.Quantity = RoundValue(Convert.ToDecimal(lineItemDetail.lineItem.Quantity), PMSVoucherSetting.QuantityRoundDigit);

                lineItemDetail.lineItem.TotalAmount = Convert.ToDecimal( lineItemDetail.lineItem.Quantity) * lineItemDetail.lineItem.UnitAmount;
                lineItemDetail.lineItem.TotalAmount = RoundValue(lineItemDetail.lineItem.TotalAmount, PMSVoucherSetting.TotalAmountRoundDigit);


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
                lineItemDetail.lineItem.TaxAmount = RoundValue(taxableAmount * (Convert.ToDecimal(tax.Amount) / 100), PMSVoucherSetting.TotalAmountRoundDigit);
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




        public List<ValueFactorDefinitionDTO> GetValueFactorDefnsFromSetting(VoucherDTO currentVoucherBuffer, LineItemDTO currentLineItem, string type)
        {

            List<ValueFactorDefinitionDTO> valueFactorDefinitions = new List<ValueFactorDefinitionDTO>();
            VoucherDTO valueFactorvoucher = new VoucherDTO();
            string valueFactorType = null;

            if (type == "discount")
            {
                if (PMSVoucherSetting.ApplicableDiscount != null)
                {
                    valueFactorType = PMSVoucherSetting.ApplicableDiscount;
                }
                else
                {
                    XtraMessageBox.Show("Applicable Discount Setting is Not Defined!", "Applicable Discount Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                if (PMSVoucherSetting.ApplicableAddtiCharge != null)
                {
                    valueFactorType = PMSVoucherSetting.ApplicableAddtiCharge;
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

        private List<ValueFactorDefinitionDTO> GetValueFactorDefnFromParam(double discountParam, int? discountValueFactDefn, string type)
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




        #endregion
    }
}
