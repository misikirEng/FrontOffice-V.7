using CNET.ERP.Client.Common.UI;
using CNET.FrontOffice_V._7;
using ProcessManager;
using CNET.FrontOffice_V._7.PMS.Common_Classes;
using CNET_V7_Domain.Domain.ArticleSchema;
using CNET_V7_Domain.Domain.SettingSchema;
using CNET_V7_Domain.Domain.TransactionSchema;
using CNET_V7_Domain.Misc.PmsDTO;
using DevExpress.Mvvm.POCO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNET.ERP.Client.UI_Logic.PMS.Forms.CommonClass
{
    public class SplitVoucher
    {

        public List<VoucherBuffer> SplitVouchers(VoucherBuffer sourceVoucherBuffer, int noOfSplits, string splitBy, bool? splitTypeIsPercentage, decimal? splitValue, DateTime currentTime, int regVoucher)
        {
            List<VoucherBuffer> splitVoucherBuffers = new List<VoucherBuffer>();
            int count = noOfSplits;
            float quantity = 0, amount = 0, remainder = 0;

            List<LineItemDetailPMS> lineItemDetailsnew = new List<LineItemDetailPMS>();
            List<LineItemDetails> lineItemDetails = new List<LineItemDetails>();
            List<LineItemBuffer> lineItemObjs = sourceVoucherBuffer.LineItemsBuffer.ToList();
             
            string currentVoCode = UIProcessManager.IdGenerater("Voucher", sourceVoucherBuffer.Voucher.Definition,  0,LocalBuffer.LocalBuffer.CurrentDeviceConsigneeUnit.Value, false, LocalBuffer.LocalBuffer.CurrentDevice.Id);


            if (string.IsNullOrEmpty(currentVoCode))
            {
                SystemMessage.ShowModalInfoMessage("Unable to generate voucher Id!", "ERROR");
                return null;
            }

            while (count > 0)
            {
                VoucherBuffer voucherBuffer = new VoucherBuffer();
                VoucherDTO voucher = new VoucherDTO();
                lineItemDetails = new List<LineItemDetails>();
                //voucher.Id = sourceVoucherBuffer.Voucher.Id;
                voucher.Code = currentVoCode; 
                voucher.Consignee1 = sourceVoucherBuffer.Voucher.Consignee1;
                voucher.IssuedDate = sourceVoucherBuffer.Voucher.IssuedDate;
                voucher.Year = sourceVoucherBuffer.Voucher.Year;
                voucher.Day = sourceVoucherBuffer.Voucher.Day;
                voucher.Month = sourceVoucherBuffer.Voucher.Month;
                voucher.Type = sourceVoucherBuffer.Voucher.Type;
                voucher.Definition = sourceVoucherBuffer.Voucher.Definition;
                voucher.LastState = sourceVoucherBuffer.Voucher.LastState;
                voucher.LastActivity = sourceVoucherBuffer.Voucher.LastActivity;
                voucher.IsVoid = sourceVoucherBuffer.Voucher.IsVoid;
                voucher.Note = sourceVoucherBuffer.Voucher.Note;
                voucher.IsIssued = sourceVoucherBuffer.Voucher.IsIssued;
                voucher.Period = sourceVoucherBuffer.Voucher.Period;
                voucher.Type = sourceVoucherBuffer.Voucher.Type;
                voucher.Remark = sourceVoucherBuffer.Voucher.Remark;
                voucherBuffer.Voucher = voucher;
                voucherBuffer.LineItemsBuffer = new List<LineItemBuffer>();


                int? articleCode = null;
                if ((lineItemObjs != null && lineItemObjs.Count > 0))
                {
                    foreach (var li in lineItemObjs)
                    {
                        ArticleDTO a = UIProcessManager.GetArticleById(li.LineItem.Article);
                        if (voucher.Definition == CNETConstantes.DAILY_ROOM_CHARGE_VOUCHER && a.Preference == LocalBuffer.LocalBuffer.ACCOMODATION_PREFERENCE_CODE)
                        {
                            articleCode = a.Id;
                            break;
                        }
                        else
                        {
                            articleCode = a.Id;
                            break;
                        }
                    }
                }
                TaxDTO tax = CommonLogics.GetApplicableTax(regVoucher,voucher.Definition, voucher.Consignee1, articleCode);
                if (!string.IsNullOrEmpty(tax.Remark))
                {
                    SystemMessage.ShowModalInfoMessage(tax.Remark, "ERROR");
                    return null ;
                }


                if (lineItemObjs != null && lineItemObjs.Count > 0)
                {

                    

                    foreach (var lineItemObj in lineItemObjs)
                    {
                        LineItemBuffer newLineItemObj = new LineItemBuffer();
                        LineItemDTO lineItem = new LineItemDTO();
                        lineItem.Id = lineItemObj.LineItem.Id;
                        lineItem.Article = lineItemObj.LineItem.Article;
                        lineItem.Tax = tax.Id;
                        lineItem.ObjectState = null;
                        //The user doesn't set any splitting instruction and in this case the splitting will be done in to equal portio
                        if (splitValue == null)
                        {
                            if (splitBy.ToLower() == "quantity")
                            {
                                lineItem.Quantity = lineItemObj.LineItem.Quantity / noOfSplits;
                                lineItem.UnitAmount = lineItemObj.LineItem.UnitAmount;
                            }
                            else if (splitBy.ToLower() == "value")
                            {
                                lineItem.UnitAmount = lineItemObj.LineItem.UnitAmount / noOfSplits;
                                lineItem.Quantity = lineItemObj.LineItem.Quantity;
                            }

                        }
                        else if (splitValue != null)
                        {
                            if (splitTypeIsPercentage.Value == true)
                            {
                                if (count == 2)
                                {
                                    if (splitBy.ToLower() == "quantity")
                                    {
                                        lineItem.Quantity = lineItemObj.LineItem.Quantity * (100 - splitValue.Value) / 100;
                                        lineItem.UnitAmount = lineItemObj.LineItem.UnitAmount;
                                    }
                                    else if (splitBy.ToLower() == "value")
                                    {
                                        lineItem.UnitAmount = lineItemObj.LineItem.UnitAmount * (100 - splitValue.Value) / 100;
                                        lineItem.Quantity = lineItemObj.LineItem.Quantity;
                                    }
                                }
                                else if (count == 1)
                                {
                                    //splitValue = 100 - splitValue.Value;
                                    if (splitBy.ToLower() == "quantity")
                                    {
                                        lineItem.Quantity = lineItemObj.LineItem.Quantity * (splitValue.Value) / 100;
                                        lineItem.UnitAmount = lineItemObj.LineItem.UnitAmount;
                                    }
                                    else if (splitBy.ToLower() == "value")
                                    {
                                        lineItem.UnitAmount = lineItemObj.LineItem.UnitAmount * (splitValue.Value) / 100;
                                        lineItem.Quantity = lineItemObj.LineItem.Quantity;
                                    }
                                }
                            }
                            else if (splitTypeIsPercentage.Value == false)
                            {
                                if (count == 2)
                                {
                                    if (splitBy.ToLower() == "quantity")
                                    {
                                        lineItem.Quantity = lineItemObj.LineItem.Quantity - splitValue.Value;
                                        lineItem.UnitAmount = lineItemObj.LineItem.UnitAmount;
                                    }
                                    else if (splitBy.ToLower() == "value")
                                    {
                                        lineItem.UnitAmount = lineItemObj.LineItem.UnitAmount - splitValue.Value;
                                        lineItem.Quantity = lineItemObj.LineItem.Quantity;
                                    }
                                }
                                else if (count == 1)
                                {
                                    if (splitBy.ToLower() == "quantity")
                                    {
                                        lineItem.Quantity = splitValue.Value;
                                        lineItem.UnitAmount = lineItemObj.LineItem.UnitAmount;
                                    }
                                    else if (splitBy.ToLower() == "value")
                                    {
                                        lineItem.UnitAmount = splitValue.Value;
                                        lineItem.Quantity = lineItemObj.LineItem.Quantity;
                                    }
                                }
                            }
                        }
                        NewLineItemCaculator lineItemCalculator = new NewLineItemCaculator();
                        LineItemDetailPMS lineItemDetail = lineItemCalculator.LineItemDetailCalculatorVoucher(voucherBuffer, lineItem, regVoucher, null, null, null, null, true);

                        LineItemDetails newLineitemDetail = new LineItemDetails()
                        {
                            lineItems = lineItemDetail.lineItem,
                            lineItemValueFactor = lineItemDetail.lineItemValueFactor
                        };

                        lineItemDetails.Add(newLineitemDetail);

                        newLineItemObj.LineItem = new LineItemDTO();
                        newLineItemObj.LineItem = lineItemDetail.lineItem;
                        newLineItemObj.LineItem.Id = lineItemObj.LineItem.Id;
                        newLineItemObj.LineItem.ObjectState =null;
                        //    lineItemDetail.lineItem.article;

                        //newLineItemObj.ArticleName = lineItemDetail.articleName;
                        //newLineItemObj.Quantity = lineItemDetail.lineItem.quantity;
                        //newLineItemObj.UnitPrice = lineItemDetail.lineItem.unitAmt.Value;
                        //newLineItemObj.TotalAmount = lineItemDetail.lineItem.totalAmount.Value;

                        //newLineItemObj.lineitem = lineItemDetail.lineItem;
                        //if (count == 2)
                        //    newLineItemObj.LineItem.Voucher = sourceVoucherBuffer.Voucher.Code;
                        //else
                        //    newLineItemObj.LineItem.Voucher = voucher.Code;

                        //newLineItemObj.lineitemValueFactors = lineItemDetail.lineItemValueFactor;
                        voucherBuffer.LineItemsBuffer.Add(newLineItemObj);
                    }

                    VoucherFinalCalculator voucherfinalCalc = new VoucherFinalCalculator();
                    VoucherFinalDTO voucherFinalResult = voucherfinalCalc.VoucherCalculation(voucherBuffer.Voucher, lineItemDetails);

                    voucherBuffer.Voucher.AddCharge = voucherFinalResult.voucher.AddCharge;
                    voucherBuffer.Voucher.Discount = voucherFinalResult.voucher.Discount;
                    voucherBuffer.Voucher.SubTotal = voucherFinalResult.voucher.SubTotal;
                    voucherBuffer.Voucher.GrandTotal = voucherFinalResult.voucher.GrandTotal;
                   // voucherBuffer.voucherValue = voucherFinalResult.voucherValues;
                    voucherBuffer.TaxTransactions = voucherFinalResult.taxTransactions; 
                    if (count == 2)
                    {
                        //voucherBuffer.voucherValue.voucher = sourceVoucherBuffer.voucher.code;
                        //if (voucherBuffer.TaxTransactions != null && voucherBuffer.taxTransactions.Count > 0)
                        //{
                        //    voucherBuffer.taxTransactions.FirstOrDefault().voucher = sourceVoucherBuffer.voucher.code;
                        //}
                    }
                    else
                    {
                        //voucherBuffer.voucherValue.voucher = voucher.code;

                        //if (voucherBuffer.taxTransactions != null && voucherBuffer.taxTransactions.Count > 0)
                        //{
                        //    voucherBuffer.taxTransactions.FirstOrDefault().voucher = voucher.code;
                        //}

                    }

                }
                else
                {
                    /**The Voucher is non-lineItem**/
                    if (splitValue == null)
                    {
                        /**The user doesn't set any splitting instruction and
                         * in this case the splitting is will be done into equal portions.**/
                        voucherBuffer.Voucher.GrandTotal = sourceVoucherBuffer.Voucher.GrandTotal / noOfSplits;
                    }
                    else if (splitValue != null)
                    {
                        if (splitTypeIsPercentage.Value == true)
                        {
                            voucherBuffer.Voucher.GrandTotal = sourceVoucherBuffer.Voucher.GrandTotal * (100 - splitValue.Value) / 100;
                            splitValue = 100 - splitValue.Value;
                        }
                        else if (splitTypeIsPercentage.Value == false)
                        {
                            if (count == 2)
                            {
                                voucherBuffer.Voucher.GrandTotal = sourceVoucherBuffer.Voucher.GrandTotal - splitValue.Value;
                            }
                            else if (count == 1)
                            {
                                voucherBuffer.Voucher.GrandTotal = splitValue.Value;
                            }
                        }




                    }

                    //** For Rebate, save Voucher Values and Tax Transactions **//
                    if (sourceVoucherBuffer.Voucher.Definition == CNETConstantes.CREDIT_NOTE_VOUCHER)
                    {

                        bool isNetoff = true;
                        //var voValue = UIProcessManager.GetVoucherValue(sourceVoucherBuffer.Voucher);
                        //if (voValue != null)
                        //{
                            if (sourceVoucherBuffer.Voucher.Remark == "netoff_off")
                            {
                                isNetoff = false;
                            }
                        //}

                        //Save Tax Transction
                        decimal taxAmt = 0;
                        decimal taxRate = tax.Amount;
                        if (isNetoff)
                        {
                            decimal subtotal = Math.Round(voucherBuffer.Voucher.GrandTotal / (((decimal)taxRate / 100) + 1), 2);
                            voucherBuffer.Voucher.GrandTotal = subtotal;

                            //netoff
                            taxAmt = Math.Round(voucherBuffer.Voucher.GrandTotal - subtotal, 2);
                        }
                        else
                        {
                            taxAmt = Math.Round(voucherBuffer.Voucher.GrandTotal * (((decimal)taxRate / 100)), 2);
                        }

                        TaxTransactionDTO taxTrans = new TaxTransactionDTO()
                        { 
                            TaxableAmount = voucherBuffer.Voucher.GrandTotal,
                            Tax = tax.Id,
                            TaxAmount = taxAmt,
                            Remark = "Rebate"
                        };

                        voucherBuffer.TaxTransactions = new List<TaxTransactionDTO>() { taxTrans };
                         
                        voucherBuffer.Voucher.SubTotal  = voucherBuffer.Voucher.GrandTotal;
                        voucherBuffer.Voucher.Remark = isNetoff ? "netoff_on" : "netoff_off";
                        voucherBuffer.Voucher.AddCharge = 0;
                        voucherBuffer.Voucher.Discount = 0;

                    }
                }

               //add transaction currency 
                //if (voucherBuffer.TransactionCurrencies != null)
                //{
                //    TransactionCurrencyDTO tranCurrency = new TransactionCurrencyDTO(); 
                //    tranCurrency.Voucher = voucherBuffer.voucher.code;
                //    tranCurrency.Currency = tranCur.currency;
                //    tranCurrency.Amount = voucherBuffer.voucher.grandTotal;
                //    tranCurrency.Rate = tranCur.rate;
                //    tranCurrency.Total = voucherBuffer.voucher.grandTotal * tranCur.rate;
                //    voucherBuffer.TransactionCurrencies = = new List< tranCurrency;

                //}


                splitVoucherBuffers.Add(voucherBuffer);
                count--;
            }
            /*The first splitted voucher will inherit the nature of the original voucher 
             * except its splitted parts*/

            splitVoucherBuffers[0].Voucher.Id = sourceVoucherBuffer.Voucher.Id;
            splitVoucherBuffers[0].Voucher.Code = sourceVoucherBuffer.Voucher.Code;
            splitVoucherBuffers[0].Voucher.IssuedDate = sourceVoucherBuffer.Voucher.IssuedDate;
            splitVoucherBuffers[0].TransactionReferencesBuffer = sourceVoucherBuffer.TransactionReferencesBuffer;

            return splitVoucherBuffers;
        }
    }
}
