using CNET_V7_Domain.Domain.TransactionSchema;
using CNET_V7_Domain.Misc.PmsDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNET.FrontOffice_V._7.PMS.Common_Classes
{
    public class VoucherFinalCalculator
    {

        #region Voucher Calculation
        public VoucherFinalDTO voucheFinal;
        public decimal taxTotal = 0;
        public decimal taxableTotal = 0;
        public string ApplicableDiscountValueFactor { get; set; }
        public string ApplicableAddtiChargValueFactor { get; set; }

        //This algorithm is responsible for the calculation of the voucher totals
        public VoucherFinalDTO VoucherCalculation(VoucherDTO voucher,List<LineItemDetails> lineitems,string applicableDiscountVF = "", 
                                                  string applicableAddChargeVF = "", VoucherTermDTO term = null)
        {
            try
            {
                List<TaxTransactionDTO> taxTransactions = new List<TaxTransactionDTO>();
                taxTransactions = new List<TaxTransactionDTO>();
                voucheFinal = new VoucherFinalDTO();
                voucheFinal.lineItemDetails = new List<LineItemDetails>();
                voucheFinal.taxTransactions = new List<TaxTransactionDTO>(); 
                voucheFinal.voucher = new VoucherDTO();
                //tax transactions are computed form line items grouped by tax types
                foreach (var ll in lineitems)
                {
                    TaxTransactionDTO taxtrans = new TaxTransactionDTO();
                    taxtrans = new TaxTransactionDTO { Tax = (int)ll.lineItems.Tax, TaxAmount = (decimal)ll.lineItems.TaxAmount, TaxableAmount = (decimal)ll.lineItems.TaxableAmount };//l.taxableAmountvoucheFinal.taxTransactions
                    taxTransactions.Add(taxtrans);
                }


                taxTransactions = taxTransactions.Select(s => new TaxTransactionDTO { Tax = s.Tax,  TaxAmount = s.TaxAmount, TaxableAmount = s.TaxableAmount }).ToList();

                var taxTrans = taxTransactions.GroupBy(sl => new { sl.Tax }).Select(o => new
                {

                    taxTypee = o.Key.Tax,
                    taxableAmount = o.Sum(s => s.TaxableAmount),
                    taxAmount = o.Sum(t => t.TaxAmount),
                    remark = ""
                }).ToList();

                taxTotal = Convert.ToDecimal(taxTrans.Sum(s => s.taxAmount));
                taxTotal = RoundValue(taxTotal, PMSVoucherSetting.TotalAmountRoundDigit);
                taxableTotal = Convert.ToDecimal(taxTrans.Sum(s => s.taxableAmount));
                taxableTotal = RoundValue(taxableTotal, PMSVoucherSetting.TotalAmountRoundDigit);

                foreach (var tt in taxTrans)
                {
                    TaxTransactionDTO taxTransaction = new TaxTransactionDTO(); 
                    taxTransaction.Tax = tt.taxTypee;
                    taxTransaction.TaxAmount = RoundValue(tt.taxAmount.Value, PMSVoucherSetting.TotalAmountRoundDigit);
                    taxTransaction.TaxableAmount = RoundValue(tt.taxableAmount.Value, PMSVoucherSetting.TotalAmountRoundDigit);
                    voucheFinal.taxTransactions.Add(taxTransaction);
                }
                decimal subtotal = 0;
                foreach (var li in lineitems)
                {
                    subtotal += Convert.ToDecimal(li.lineItems.TotalAmount);
                }
                voucheFinal.voucher.SubTotal = RoundValue(subtotal, PMSVoucherSetting.TotalAmountRoundDigit);
                voucher.SubTotal = voucheFinal.voucher.SubTotal;
                decimal? discount = 0;
                decimal? addtionalCharge = 0;
                foreach (var val in lineitems)
                {
                    if (val.lineItemValueFactor != null && val.lineItemValueFactor.Count > 0)
                    {
                        discount +=
                            val.lineItemValueFactor.Where(ll => ll.IsDiscount == true).Sum(lv => lv.Amount);
                        addtionalCharge +=
                            val.lineItemValueFactor.Where(ll => ll.IsDiscount == false).Sum(lv => lv.Amount);
                    }
                    else
                    {

                        discount += 0;

                        addtionalCharge += 0;

                    }
                }
                voucheFinal.voucher.AddCharge = RoundValue(addtionalCharge.Value, PMSVoucherSetting.TotalAmountRoundDigit);
                voucher.AddCharge = voucheFinal.voucher.AddCharge;
                voucheFinal.voucher.Discount = RoundValue(discount.Value, PMSVoucherSetting.TotalAmountRoundDigit);
                voucher.Discount = voucheFinal.voucher.Discount;

                //  taxableTotal += (decimal)(addtionalCharge - discount);
                voucheFinal.voucher.GrandTotal = RoundValue((taxableTotal + taxTotal), PMSVoucherSetting.TotalAmountRoundDigit);
                voucher.GrandTotal = voucheFinal.voucher.GrandTotal;
                voucheFinal.lineItemDetails.AddRange(lineitems);
                return voucheFinal;
            }
            catch (Exception)
            {
                return new VoucherFinalDTO();
                //  throw;
            }
        }





        #endregion

        #region Value Formatting
        public decimal RoundValue(decimal number, int digit)
        {
            decimal roundValue = 0;
            switch (digit)
            {
                case 2:
                    roundValue = Convert.ToDecimal(string.Format("{0:f2}", number));
                    break;
                case 3:
                    roundValue = Convert.ToDecimal(string.Format("{0:f3}", number));
                    break;
                case 4:
                    roundValue = Convert.ToDecimal(string.Format("{0:f4}", number));
                    break;

            }
            return roundValue;
        }
        #endregion



    }
}
