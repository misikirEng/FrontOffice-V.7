using CNET.API.Manager.Common;
using CNET.API.Manager.Consignee;
using CNET.API.Manager.Filter;
using CNET.API.Manager.PMS;
using CNET.API.Manager.Setting;
using CNET.API.Manager.Transaction;
using CNET_V7_Domain.Domain.CommonSchema;
using CNET_V7_Domain.Domain.ConsigneeSchema;
using CNET_V7_Domain.Domain.SettingSchema;
using CNET_V7_Domain.Domain.TransactionSchema;
using CNET_V7_Domain.Domain.ViewSchema;
using CNET_V7_Domain.Misc.CommonTypes;
using CNET_V7_Domain.Misc.PmsDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentPrint.APICall
{
    public class APICall
    {
        
        public static VoucherDTO GetVoucherById(int Id)
        {
            return Task.Run(async () => await transactionRequest.GetVoucherById(Id)).Result;
        }
        public static VoucherDTO UpdateVoucher(VoucherDTO voucher)
        {
            return Task.Run(async () => await transactionRequest.UpdateVoucher(voucher)).Result;
        }
        public static VoucherDetailDTO get_voucher_detail(int Id)
        {
            return Task.Run(async () => await transactionRequest.get_voucher_detail(Id)).Result;
        }
        public static RegistrationPrintOutDTO GetRegistrationConformation(int Id)
        {
            return Task.Run(async () => await pmsLibraryRequest.GetRegistrationConformation(Id)).Result;
        }
        public static ConsigneeDTO GetCompanyDTO()
        {
            List<ConsigneeDTO> Companylist = GetConsigneeBygslType(1);
            if (Companylist != null)
                return Companylist.FirstOrDefault();
            else return null;
        }
        public static List<TaxTransactionDTO> GetTaxTransactionByvoucher(int voucher)
        {
            Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>();
            Dictionaryvalue.Add("voucher", voucher.ToString());
            return Task.Run(async () => await filterRequest.GetFilterData<List<TaxTransactionDTO>>("TaxTransaction", Dictionaryvalue)).Result;
        }
        public static List<TransactionReferenceDTO> GetTransactionReferenceByReferring(int Referring)
        {
            Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>();
            Dictionaryvalue.Add("Referring", Referring.ToString());
            return Task.Run(async () => await filterRequest.GetFilterData<List<TransactionReferenceDTO>>("TransactionReference", Dictionaryvalue)).Result;
        }
        public static List<ConsigneeDTO> GetConsigneeBygslType(int gslType)
        {
            Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>();
            Dictionaryvalue.Add("gslType", gslType.ToString());
            return Task.Run(async () => await filterRequest.GetFilterData<List<ConsigneeDTO>>("Consignee", Dictionaryvalue)).Result;
        }
        public static List<ConsigneeUnitDTO> GetConsigneeByConsignee(int Consignee)
        {
            Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>();
            Dictionaryvalue.Add("Consignee", Consignee.ToString());
            return Task.Run(async () => await filterRequest.GetFilterData<List<ConsigneeUnitDTO>>("ConsigneeUnit", Dictionaryvalue)).Result;
        }
        public static List<ConsigneeUnitDTO> GetCompanyBranch(int consignee)
        { 
          return GetConsigneeByConsignee(consignee);
        }
        public static ConsigneeUnitDTO GetCurrentBranch()
        {
            string deviceName = System.Windows.Forms.SystemInformation.ComputerName;
            DeviceDTO CurrentDevice = GetDeviceByname(deviceName);

            if (CurrentDevice.ConsigneeUnit != null)

                return Task.Run(async () => await consigneeRequest.GetConsigneeUnitById(CurrentDevice.ConsigneeUnit.Value)).Result;
            else
                return null;
        }
        public static List<ConfigurationDTO> GetAllConfiguration()
        {
            return Task.Run(async () => await settingRequest.SelectAllConfiguration()).Result;
        }
        public static List<SystemConstantDTO> GetAllSystemConstant()
        {
            return Task.Run(async () => await settingRequest.SelectAllSystemConstant()).Result;
        }
        public static List<DistributionDTO> GetAllDistribution()
        {
            return Task.Run(async () => await settingRequest.SelectAllDistribution()).Result;
        }
        public static List<TaxDTO> GetAllTax()
        {
            return Task.Run(async () => await settingRequest.SelectAllTax()).Result;
        }      
        public static List<CurrencyDTO> GetAllCurrency()
        {
            return Task.Run(async () => await commonRequest.SelectAllCurrency()).Result;
        }
        public static DeviceDTO GetDeviceByname(string name)
        {
            try
            {
                Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>();
                Dictionaryvalue.Add("machineName", name.ToString());
                List<DeviceDTO> returndevicelist = Task.Run(async () => await filterRequest.GetFilterData<List<DeviceDTO>>("Device", Dictionaryvalue)).Result;
                if (returndevicelist != null && returndevicelist.Count > 0)
                    return returndevicelist.FirstOrDefault();
                else
                    return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static DeviceDTO GetDeviceByPreference(int preference)
        {
            try
            {
                Dictionary<string, string> Dictionaryvalue = new Dictionary<string, string>();
                Dictionaryvalue.Add("Preference", preference.ToString());
                List<DeviceDTO> returndevicelist = Task.Run(async () => await filterRequest.GetFilterData<List<DeviceDTO>>("Device", Dictionaryvalue)).Result;
                if (returndevicelist != null && returndevicelist.Count > 0)
                    return returndevicelist.FirstOrDefault();
                else
                    return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
