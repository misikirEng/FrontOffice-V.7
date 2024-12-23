using CNET.Client.Common.UserControls.Data.Data_Cache;
using CNET.Client.Common.UserControls.Data.Data_Manager;
using CNET.ERP.Client.Common.UI;
using CNET.ERP.Client.UI_Logic.PMS.Infra;
using CNET.ERP2016.ServiceInterfaces.Types;
using CNET.ERP2016.SQLDataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNET.FrontOffice_V._7.Logics
{
    public class SplitVoucher
    {

        public List<NewVoucherBuffer> SplitVouchers(VoucherSetting voucherSetting, NewVoucherBuffer sourceVoucherBuffer, int noOfSplits, string splitBy, bool? splitTypeIsPercentage, decimal? splitValue, DateTime currentTime, string regVoucher)
        {
            List<NewVoucherBuffer> splitVoucherBuffers = new List<NewVoucherBuffer>();
            int count = noOfSplits;
            float quantity = 0, amount = 0, remainder = 0;

            List<LineItemDetailNew> lineItemDetailsnew = new List<LineItemDetailNew>();
            List<LineItemDetails> lineItemDetails = new List<LineItemDetails>();
            List<NewLineitemObj> lineItemObjs = sourceVoucherBuffer.newLineitemObjs;

            Generated_ID generatedId = UIProcessManager.IdGenerater("Voucher", LoginPage.Authentication.DeviceObject,
                               sourceVoucherBuffer.voucher.voucherDefinition.ToString(),
                               CNETConstantes.VOUCHER_COMPONENET, 0);
            if (generatedId == null)
            {
                Home.ShowModalInfoMessage("Unable to generate voucher Id!", "ERROR");
                return null;
            }

            while (count > 0)
            {
                NewVoucherBuffer voucherBuffer = new NewVoucherBuffer();
                Voucher voucher = new Voucher();
                lineItemDetails = new List<LineItemDetails>();
                voucher.code = generatedId.GeneratedNewId;
                voucher.component = sourceVoucherBuffer.voucher.component;
                voucher.consignee = sourceVoucherBuffer.voucher.consignee;
                voucher.IssuedDate = sourceVoucherBuffer.voucher.IssuedDate;
                voucher.year = sourceVoucherBuffer.voucher.year;
                voucher.date = sourceVoucherBuffer.voucher.date;
                voucher.month = sourceVoucherBuffer.voucher.month;
                voucher.type = sourceVoucherBuffer.voucher.type;
                voucher.voucherDefinition = sourceVoucherBuffer.voucher.voucherDefinition;
                voucher.LastObjectState = sourceVoucherBuffer.voucher.LastObjectState;
                voucher.lastActivity = sourceVoucherBuffer.voucher.lastActivity;

                voucher.IsIssued = sourceVoucherBuffer.voucher.IsIssued;
                voucher.period = sourceVoucherBuffer.voucher.period;
                voucher.type = sourceVoucherBuffer.voucher.type;
                voucher.remark = sourceVoucherBuffer.voucher.remark;
                voucherBuffer.voucher = voucher;
                voucherBuffer.newLineitemObjs = new List<NewLineitemObj>();


                string articleCode = "";
                if (lineItemObjs != null && lineItemObjs.Count > 0)
                {
                    foreach (var li in lineItemObjs)
                    {
                        Article a = UIProcessManager.SelectArticle(li.lineitem.article);
                        if (voucher.voucherDefinition == CNETConstantes.DAILY_ROOM_CHARGE_VOUCHER && a.preference == CNETConstantes.ACCOMODATION_PREFERENCE_CODE)
                        {
                            articleCode = a.code;
                            break;
                        }
                        else
                        {
                            articleCode = a.code;
                            break;
                        }
                    }
                }
                Tax tax = CommonLogics.GetApplicableTax(regVoucher, voucher.voucherDefinition, voucher.consignee, articleCode);
                if (!string.IsNullOrEmpty(tax.remark))
                {
                    Home.ShowModalInfoMessage(tax.remark, "ERROR");
                    return null;
                }


                if (lineItemObjs != null && lineItemObjs.Count > 0)
                {



                    foreach (var lineItemObj in lineItemObjs)
                    {
                        NewLineitemObj newLineItemObj = new NewLineitemObj();
                        LineItem lineItem = new LineItem();
                        lineItem.article = lineItemObj.lineitem.article;
                        lineItem.tax = tax.code;

                        //The user doesn't set any splitting instruction and in this case the splitting will be done in to equal portio
                        if (splitValue == null)
                        {
                            if (splitBy.ToLower() == "quantity")
                            {
                                lineItem.quantity = lineItemObj.lineitem.quantity / noOfSplits;
                                lineItem.unitAmt = lineItemObj.lineitem.unitAmt;
                            }
                            else if (splitBy.ToLower() == "value")
                            {
                                lineItem.unitAmt = lineItemObj.lineitem.unitAmt / noOfSplits;
                                lineItem.quantity = lineItemObj.lineitem.quantity;
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
                                        lineItem.quantity = lineItemObj.lineitem.quantity * (100 - splitValue.Value) / 100;
                                        lineItem.unitAmt = lineItemObj.lineitem.unitAmt;
                                    }
                                    else if (splitBy.ToLower() == "value")
                                    {
                                        lineItem.unitAmt = lineItemObj.lineitem.unitAmt * (100 - splitValue.Value) / 100;
                                        lineItem.quantity = lineItemObj.lineitem.quantity;
                                    }
                                }
                                else if (count == 1)
                                {
                                    //splitValue = 100 - splitValue.Value;
                                    if (splitBy.ToLower() == "quantity")
                                    {
                                        lineItem.quantity = lineItemObj.lineitem.quantity * splitValue.Value / 100;
                                        lineItem.unitAmt = lineItemObj.lineitem.unitAmt;
                                    }
                                    else if (splitBy.ToLower() == "value")
                                    {
                                        lineItem.unitAmt = lineItemObj.lineitem.unitAmt * splitValue.Value / 100;
                                        lineItem.quantity = lineItemObj.lineitem.quantity;
                                    }
                                }
                            }
                            else if (splitTypeIsPercentage.Value == false)
                            {
                                if (count == 2)
                                {
                                    if (splitBy.ToLower() == "quantity")
                                    {
                                        lineItem.quantity = lineItemObj.lineitem.quantity - splitValue.Value;
                                        lineItem.unitAmt = lineItemObj.lineitem.unitAmt;
                                    }
                                    else if (splitBy.ToLower() == "value")
                                    {
                                        lineItem.unitAmt = lineItemObj.lineitem.unitAmt - splitValue.Value;
                                        lineItem.quantity = lineItemObj.lineitem.quantity;
                                    }
                                }
                                else if (count == 1)
                                {
                                    if (splitBy.ToLower() == "quantity")
                                    {
                                        lineItem.quantity = splitValue.Value;
                                        lineItem.unitAmt = lineItemObj.lineitem.unitAmt;
                                    }
                                    else if (splitBy.ToLower() == "value")
                                    {
                                        lineItem.unitAmt = splitValue.Value;
                                        lineItem.quantity = lineItemObj.lineitem.quantity;
                                    }
                                }
                            }
                        }
                        NewLineItemCaculator lineItemCalculator = new NewLineItemCaculator();
                        LineItemDetailNew lineItemDetail = lineItemCalculator.LineItemDetailCalculatorVoucher(voucherSetting, voucherBuffer, lineItem, regVoucher, null, null, null, null, true);

                        LineItemDetails newLineitemDetail = new LineItemDetails()
                        {
                            lineItems = lineItemDetail.lineItem,
                            lineItemValueFactor = lineItemDetail.lineItemValueFactor
                        };
                        lineItemDetails.Add(newLineitemDetail);

                        newLineItemObj.ArticleCode = lineItemDetail.lineItem.article;
                        newLineItemObj.ArticleName = lineItemDetail.articleName;
                        newLineItemObj.Quantity = lineItemDetail.lineItem.quantity;
                        newLineItemObj.UnitPrice = lineItemDetail.lineItem.unitAmt.Value;
                        newLineItemObj.TotalAmount = lineItemDetail.lineItem.totalAmount.Value;

                        newLineItemObj.lineitem = lineItemDetail.lineItem;
                        if (count == 2)
                            newLineItemObj.lineitem.voucher = sourceVoucherBuffer.voucher.code;

                        else
                            newLineItemObj.lineitem.voucher = voucher.code;

                        newLineItemObj.lineitemValueFactors = lineItemDetail.lineItemValueFactor;
                        voucherBuffer.newLineitemObjs.Add(newLineItemObj);
                    }

                    VoucherFinalCalculator voucherfinalCalc = new VoucherFinalCalculator();
                    VoucherFinal voucherFinalResult = voucherfinalCalc.VoucherCalculation(voucherSetting, voucherBuffer.voucher, lineItemDetails);
                    voucherBuffer.voucher.grandTotal = voucherFinalResult.voucher.grandTotal;
                    voucherBuffer.voucherValue = voucherFinalResult.voucherValues;
                    voucherBuffer.taxTransactions = voucherFinalResult.taxTransactions;

                    if (count == 2)
                    {
                        voucherBuffer.voucherValue.voucher = sourceVoucherBuffer.voucher.code;
                        if (voucherBuffer.taxTransactions != null && voucherBuffer.taxTransactions.Count > 0)
                        {
                            voucherBuffer.taxTransactions.FirstOrDefault().voucher = sourceVoucherBuffer.voucher.code;
                        }
                    }
                    else
                    {
                        voucherBuffer.voucherValue.voucher = voucher.code;

                        if (voucherBuffer.taxTransactions != null && voucherBuffer.taxTransactions.Count > 0)
                        {
                            voucherBuffer.taxTransactions.FirstOrDefault().voucher = voucher.code;
                        }

                    }

                }
                else
                {
                    /**The Voucher is non-lineItem**/
                    if (splitValue == null)
                    {
                        /**The user doesn't set any splitting instruction and
                         * in this case the splitting is will be done into equal portions.**/
                        voucherBuffer.voucher.grandTotal = sourceVoucherBuffer.voucher.grandTotal / noOfSplits;
                    }
                    else if (splitValue != null)
                    {
                        if (splitTypeIsPercentage.Value == true)
                        {
                            voucherBuffer.voucher.grandTotal = sourceVoucherBuffer.voucher.grandTotal * (100 - splitValue.Value) / 100;
                            splitValue = 100 - splitValue.Value;
                        }
                        else if (splitTypeIsPercentage.Value == false)
                        {
                            if (count == 2)
                            {
                                voucherBuffer.voucher.grandTotal = sourceVoucherBuffer.voucher.grandTotal - splitValue.Value;
                            }
                            else if (count == 1)
                            {
                                voucherBuffer.voucher.grandTotal = splitValue.Value;
                            }
                        }




                    }

                    //** For Rebate, save Voucher Values and Tax Transactions **//
                    if (sourceVoucherBuffer.voucher.voucherDefinition == CNETConstantes.CREDIT_NOTE_VOUCHER)
                    {

                        bool isNetoff = true;
                        var voValue = UIProcessManager.GetVoucherValue(sourceVoucherBuffer.voucher);
                        if (voValue != null)
                        {
                            if (voValue.remark == "netoff_off")
                            {
                                isNetoff = false;
                            }
                        }

                        //Save Tax Transction
                        decimal taxAmt = 0;
                        decimal taxRate = tax.amount;
                        if (isNetoff)
                        {
                            decimal subtotal = Math.Round(voucherBuffer.voucher.grandTotal / (taxRate / 100 + 1), 2);
                            voucherBuffer.voucher.grandTotal = subtotal;

                            //netoff
                            taxAmt = Math.Round(voucherBuffer.voucher.grandTotal - subtotal, 2);
                        }
                        else
                        {
                            taxAmt = Math.Round(voucherBuffer.voucher.grandTotal * (taxRate / 100), 2);
                        }

                        TaxTransaction taxTrans = new TaxTransaction()
                        {
                            code = string.Empty,
                            voucher = count == 1 ? voucher.code : sourceVoucherBuffer.voucher.code,
                            taxableAmount = voucherBuffer.voucher.grandTotal,
                            taxType = tax.code,
                            taxAmount = taxAmt,
                            remark = "Rebate"
                        };

                        voucherBuffer.taxTransactions = new List<TaxTransaction>() { taxTrans };


                        //voucher value
                        VoucherValue voucValue = new VoucherValue()
                        {
                            code = string.Empty,
                            voucher = count == 1 ? voucher.code : sourceVoucherBuffer.voucher.code,
                            discount = 0,
                            subTotal = voucherBuffer.voucher.grandTotal,
                            remark = isNetoff ? "netoff_on" : "netoff_off",
                            additionalCharge = 0
                        };
                        voucherBuffer.voucherValue = voucValue;

                    }
                }

                //add transaction currency
                var tranCur = UIProcessManager.GetTransCurrencyByVoucher(voucher.code).FirstOrDefault();
                if (tranCur != null)
                {
                    TransactionCurrency tranCurrency = new TransactionCurrency();
                    tranCurrency.code = "";
                    tranCurrency.voucher = voucherBuffer.voucher.code;
                    tranCurrency.currency = tranCur.currency;
                    tranCurrency.amount = voucherBuffer.voucher.grandTotal;
                    tranCurrency.rate = tranCur.rate;
                    tranCurrency.total = voucherBuffer.voucher.grandTotal * tranCur.rate;
                    voucherBuffer.transactionCurrency = tranCurrency;

                }


                splitVoucherBuffers.Add(voucherBuffer);
                count--;
            }
            /*The first splitted voucher will inherit the nature of the original voucher 
             * except its splitted parts*/
            splitVoucherBuffers[0].voucher.code = sourceVoucherBuffer.voucher.code;
            splitVoucherBuffers[0].voucher.IssuedDate = sourceVoucherBuffer.voucher.IssuedDate;

            return splitVoucherBuffers;
        }
    }
}
