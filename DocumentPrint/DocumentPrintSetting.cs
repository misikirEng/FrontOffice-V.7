using CNET_V7_Domain;
using CNET_V7_Domain.Domain.CommonSchema;
using CNET_V7_Domain.Domain.ConsigneeSchema;
using CNET_V7_Domain.Domain.SecuritySchema;
using CNET_V7_Domain.Domain.SettingSchema;
using DevExpress.Pdf.Native;
using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using ProcessManager;
using CNET_V7_Domain.Domain.ViewSchema;
using System.Drawing;
using DevExpress.XtraBars.Ribbon;

namespace DocumentPrint
{
    public class DocumentPrintSetting
    {
        public static int FileServer = 520;
        public static string CompanyName { get; set; }
        public static string CompanyBranchName { get; set; }

        public static Image? CompanyAttachmentLogo { get; set; }

        public static ConsigneeDTO CompanyConsigneeDTO { get; set; }

        public static ActivityDefinitionDTO PrintActivityDefinitionDTO { get; set; }

        public static List<ConfigurationDTO> ConfigurationDTOList { get; set; } 

        public static ConsigneeUnitDTO CompanyConsigneeUnitDTO { get; set; }
        public static List<ConsigneeUnitDTO> CompanyBranchList { get; set; }

        public static List<SystemConstantDTO> SystemConstantList { get; set; }
        public static List<DistributionDTO> DistributionList { get; set; }
        public static List<TaxDTO> TaxList { get; set; }
        public static List<CurrencyDTO> CurrencyList { get; set; }

        private static bool IsfirstTime = true;

        public static string CompanyVATNumber { get; set; }
        public static string CompanyTIN { get; set; }
        public static string CompanyTel { get; set; }
        public static string CompanyContact { get; set; }
        public static string CompanyWeb { get; set; }
        public static string CompanyFax { get; set; }
        public static string CompanyPOBox { get; set; }
        public static string CompanyEmail { get; set; }
         
        public static string VoucherOrientation = "portrait";

        public static void CheckAndGetDefaultData()
        {
            if (IsfirstTime || LocalBuffer.LocalBuffer.BufferRefreshed)
            {
                LocalBuffer.LocalBuffer.BufferRefreshed = false;

                IsfirstTime = false;

                var ActivityDefinitionDTO = UIProcessManager.GetActivityDefinitionByDescription(CNETConstantes.Activity_Printed);

                if (ActivityDefinitionDTO != null && ActivityDefinitionDTO.Count > 0)
                    PrintActivityDefinitionDTO = ActivityDefinitionDTO.FirstOrDefault();

                //Company Data
                CompanyConsigneeDTO = GetCompanyDTO();

                CompanyConsigneeUnitDTO = UIProcessManager.GetConsigneeUnitById(LocalBuffer.LocalBuffer.CurrentDeviceConsigneeUnit.Value);

                List<IdentificationDTO> IdentificationList = UIProcessManager.SelectAllIdentification();

                IdentificationDTO identificationDTO = IdentificationList.FirstOrDefault(x => x.Consignee == CompanyConsigneeDTO.Id && x.Type == 535);
                if (identificationDTO != null)
                    CompanyVATNumber = identificationDTO.IdNumber;

                ConfigurationDTOList = LocalBuffer.LocalBuffer.ConfigurationBufferList;

                CompanyBranchList = UIProcessManager.GetConsigneeUnitByconsignee(CompanyConsigneeDTO.Id);

                SystemConstantList = LocalBuffer.LocalBuffer.SystemConstantDTOBufferList;

                DistributionList = UIProcessManager.SelectAllDistribution();

                TaxList = LocalBuffer.LocalBuffer.TaxBufferList;

                CurrencyList = LocalBuffer.LocalBuffer.CurrencyBufferList;

                //Company Branch Data
                CompanyName = CompanyConsigneeDTO.FirstName;


                CompanyTIN = CompanyConsigneeDTO.Tin;
                if (CompanyConsigneeUnitDTO != null)
                {
                    CompanyBranchName = CompanyConsigneeUnitDTO.Name;
                    CompanyTel = CompanyConsigneeUnitDTO.Phone1;
                    CompanyContact = CompanyConsigneeUnitDTO.Contact;
                    CompanyWeb = CompanyConsigneeUnitDTO.Website; 
                    CompanyPOBox = CompanyConsigneeUnitDTO.PoBox;
                    CompanyEmail = CompanyConsigneeUnitDTO.Email;
                    CompanyFax = CompanyConsigneeUnitDTO.Phone2;
                }
            
                //Company Attachment Logo
                //if (!string.IsNullOrEmpty(CompanyConsigneeDTO.DefaultImageUrl))
                //    CompanyAttachmentLogo = FTPInterface.FTPAttachment.GetImageFromFTP(CompanyConsigneeDTO.DefaultImageUrl);

                CompanyAttachmentLogo = LocalBuffer.LocalBuffer.CompanyDefaultLogo;

            }
        }

        public static ConsigneeDTO GetCompanyDTO()
        {
            List<ConsigneeDTO> Companylist = UIProcessManager.GetConsigneeBygslType(1);
            if (Companylist != null)
                return Companylist.FirstOrDefault();
            else return null;
        }

        public static string GetFileServerAttachment()
        {
            string DefaultAttachmentPath = "";

            DeviceDTO FileStorageDevice = UIProcessManager.GetDeviceByPreference(FileServer);
            if (FileStorageDevice != null)
            {
                if (ConfigurationDTOList == null)
                { 
                    XtraMessageBox.Show("Please Select Default Attachment Path for File Storage Device!", "CNET Attachment");
                    DefaultAttachmentPath = "";
                }

                ConfigurationDTO value = ConfigurationDTOList.FirstOrDefault(x => x.Reference == FileStorageDevice.Id.ToString() && x.Attribute == "Default Attachment Path");
                if (value == null)
                { 
                    XtraMessageBox.Show("Please Select Default Attachment Path for File Storage Device!", "CNET Attachment");
                    DefaultAttachmentPath = "";
                }

                DefaultAttachmentPath = value.CurrentValue + "\\PMS";
                try
                {
                    if (!Directory.Exists(DefaultAttachmentPath))
                    {
                        Directory.CreateDirectory(DefaultAttachmentPath);
                    }
                    if (!Directory.Exists(DefaultAttachmentPath))
                    {
                        XtraMessageBox.Show("Please Select Default Attachment Path for File Storage Device!", "CNET Attachment");
                        DefaultAttachmentPath = "";
                    }
                }
                catch
                {
                    XtraMessageBox.Show("Please Select Proper Default Attachment Path for File Storage Device!", "CNET Attachment");
                    DefaultAttachmentPath = "";
                }
            }
            else
            {
                DefaultAttachmentPath = ""; 
                XtraMessageBox.Show("Please Maintain File Storage Device and Set Default Attachment Path !", "CNET Attachment");
            }
            return DefaultAttachmentPath;
        }

    }
}
