using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EventManagement
{
    public class EventVoucherSetting
    {
        public static int TotalAmountRoundDigit { get; internal set; }
        public static bool ValueIsTaxInclusive { get; internal set; }
        public static int UnitPriceRoundDigit { get; internal set; }
        public static int QuantityRoundDigit { get; internal set; }


        public static string ApplicableDiscount { get; internal set; }
        public static string ApplicableAddtiCharge { get; internal set; }

        public static void GetCurrentVoucherSetting(int Voucher_Definition)
        {
            string lastSettingInfoWithError = "";

            var configurationList = LocalBuffer.LocalBuffer.ConfigurationBufferList.Where(c => c.Reference == Voucher_Definition.ToString()).ToList();
            if (configurationList != null && configurationList.Count <= 0)
            {
                XtraMessageBox.Show("There is no setting values for the current voucher type", "CNET ERP",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try
            {
                foreach (var config in configurationList)
                {
                    lastSettingInfoWithError = config.Attribute;
                    switch (config.Attribute.ToLower())
                    {

                        case "value is tax inclusive":
                            ValueIsTaxInclusive = Convert.ToBoolean(config.CurrentValue);
                            break;
                        case "round digit quantity":
                            QuantityRoundDigit = Convert.ToInt32(config.CurrentValue);
                            break;
                        case "round digit unit price":
                            UnitPriceRoundDigit = Convert.ToInt32(config.CurrentValue);
                            break;
                        case "round digit total":
                            TotalAmountRoundDigit = Convert.ToInt32(config.CurrentValue);
                            break;
                        case "applicable additional charge":
                            ApplicableAddtiCharge = config.CurrentValue;
                            break;
                        case "applicable discount":
                            ApplicableDiscount = config.CurrentValue;
                            break;



                    }
                }
            }
            catch (Exception ex)
            {

                XtraMessageBox.Show("Since  voucher settings for this voucher are not properly settled" + "\n" + "some functionalities may not work properly!" + "\n" + ex.Message + ":" + "\n" + lastSettingInfoWithError, "CNET_V2016", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
        }

    }
}
