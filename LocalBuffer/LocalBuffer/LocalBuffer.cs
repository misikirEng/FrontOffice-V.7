 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CNET_V7_Domain.Domain.ArticleSchema;
using CNET_V7_Domain.Domain.CommonSchema;
using CNET_V7_Domain.Domain.ConsigneeSchema;
using CNET_V7_Domain.Domain.PmsSchema;
using CNET_V7_Domain.Domain.SecuritySchema;
using CNET_V7_Domain.Domain.SettingSchema;
using CNET_V7_Domain.Domain.ViewSchema;
using ProcessManager; 
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Runtime.InteropServices;
using CNET_V7_Domain.Domain.AccountingSchema;
using CNET.Progress.Reporter; 
using System.Security.Permissions;
using System.Drawing;
using System.Windows.Forms;

namespace LocalBuffer
{
    public static class LocalBuffer
    {
        public static List<UserDTO> UserBufferList { get; internal set; }
        public static List<DeviceDTO> DeviceBufferList { get; internal set; }
        public static List<ActivityDefinitionDTO> ActivityDefinitionBufferList { get; internal set; }
        public static VwConsigneeViewDTO CompanyConsigneeData { get; internal set; }
        public static List<ConfigurationDTO> ConfigurationBufferList { get; internal set; }
        public static List<ConsigneeDTO> ConsigneeViewBufferList { get; internal set; }
        public static List<ConsigneeDTO> AllGuestConsigneeViewlist { get; internal set; }
        public static List<ConsigneeDTO> AllCustomerConsigneeViewlist { get; internal set; }
        public static UserDTO CurrentLoggedInUser { get; set; }
        public static bool SystemProviderCNET { get; set; }
        public static string CurrentLoggedInUserEmployeeName { get; set; }
        public static DeviceDTO CurrentDevice { get; internal set; }
        public static List<UserRoleMapperDTO> UserRoleMapperBufferList { get; internal set; }
        public static List<CurrencyDTO> CurrencyBufferList { get; internal set; }
        public static List<ConsigneeUnitDTO> HotelBranchBufferList { get; internal set; }
        public static List<ConsigneeUnitDTO> AllHotelBranchBufferList { get; internal set; }
        public static List<SystemConstantDTO> SystemConstantDTOBufferList { get; internal set; }
        public static List<LookupDTO> LookUpBufferList { get; internal set; }
        public static List<PreferenceDTO> PreferenceBufferList { get; internal set; }
        public static List<CountryDTO> CountryBufferList { get; internal set; }
        public static int? CurrentDeviceConsigneeUnit { get; internal set; }
        public static ConsigneeUnitDTO CurrentDeviceConsigneeUnitData { get; internal set; }
        public static bool UserHasHotelBranchAccess { get; set; } 
        public static List<SystemConstantDTO> VoucherDefinitionBufferlist { get; internal set; }
        public static List<ViewFunctWithAccessMDTO> AllAccessMatrixFunctionList { get; set; }
        public static List<PeriodDTO> PeriodBufferList { get; internal set; }
        public static bool BufferRefreshed { get; set; }

        public static void LoadVoucherConsigneeBuffer()
        {
            GetCustomerAndGuestBuffer();
        }

        public static void RefreshLocalBuffer()
        {
            BufferRefreshed = true;
            LoadLocalBuffer();
            GetUserRoleAccess();
        }

        public static void LoadLocalBuffer()
        {
            //"desk-top"
            string deviceName = System.Windows.Forms.SystemInformation.ComputerName;
            CurrentDevice = UIProcessManager.GetDeviceByname(deviceName);
            //CurrentLoggedInUser = MainLogin.LoggedInUser;// UIProcessManager.GetUserById(440);

            Progress_Reporter.Show_Progress("Getting User Data", "Please Wait ........");
            UserBufferList = UIProcessManager.SelectAllUser();
            Progress_Reporter.Show_Progress("Getting ActivityDefinition Data", "Please Wait ........");
            ActivityDefinitionBufferList = UIProcessManager.SelectAllActivityDefinition();
            Progress_Reporter.Show_Progress("Getting Device Data", "Please Wait ........");
            DeviceBufferList = UIProcessManager.SelectAllDevice();
            Progress_Reporter.Show_Progress("Getting Configuration Data", "Please Wait ........");
            ConfigurationBufferList = UIProcessManager.SelectAllConfiguration();
            Progress_Reporter.Show_Progress("Getting Lookup Data", "Please Wait ........");
            LookUpBufferList = UIProcessManager.SelectAllLookup();
            Progress_Reporter.Show_Progress("Getting Tax Data", "Please Wait ........");
            TaxBufferList = UIProcessManager.SelectAllTax();
            Progress_Reporter.Show_Progress("Getting UserRoleMapper Data", "Please Wait ........");
            UserRoleMapperBufferList = UIProcessManager.SelectAllUserRoleMapper();
            Progress_Reporter.Show_Progress("Getting Currency Data", "Please Wait ........");
            CurrencyBufferList = UIProcessManager.SelectAllCurrency();
            Progress_Reporter.Show_Progress("Getting SystemConstant Data", "Please Wait ........");
            SystemConstantDTOBufferList = UIProcessManager.SelectAllSystemConstant();
            Progress_Reporter.Show_Progress("Getting Preference Data", "Please Wait ........");
            PreferenceBufferList = UIProcessManager.SelectAllPreference();
            Progress_Reporter.Show_Progress("Getting Country Data", "Please Wait ........");
            CountryBufferList = UIProcessManager.SelectAllCountry();
            ObjectStateDefinitionBufferList = SystemConstantDTOBufferList.Where(x => x.Type == "ObjectState Definition").ToList();
            VoucherDefinitionBufferlist = SystemConstantDTOBufferList.Where(x => x.Type == "Transaction").ToList();
            Progress_Reporter.Show_Progress("Getting VoucherExtension Data", "Please Wait ........");
            VoucherExtensionBufferList = UIProcessManager.SelectAllVoucherExtension();
            Progress_Reporter.Show_Progress("Getting Period Data", "Please Wait ........");
            PeriodBufferList = UIProcessManager.SelectAllPeriod();
            //Progress_Reporter.Show_Progress("Getting Access Data", "Please Wait ........");
            //GetUserRoleAccess();
            Progress_Reporter.Show_Progress("Getting Customer And Guest Data", "Please Wait ........");
            GetCustomerAndGuestBuffer();
            Progress_Reporter.Show_Progress("Getting Hotel Branches Data", "Please Wait ........");
            GetHotelBranches();
            Progress_Reporter.Show_Progress("Getting PMS Preference Data", "Please Wait ........");
            CheckandGetPMSPreference();
            Progress_Reporter.Show_Progress("Getting PMS CheckOut Type Data", "Please Wait ........");
            CheckPMSCheckoutType();
            
            Progress_Reporter.Close_Progress();
        }
        public static bool PMSRecieptIsCheckout { get; set; }

        public static void CheckPMSCheckoutType()
        {
            ConfigurationDTO Checkoutconfig = ConfigurationBufferList.FirstOrDefault(c => c.Reference == CNETConstantes.PMS_Pointer.ToString() && c.Attribute == CNETConstantes.PMS_SETTING_CheckOutType);
           
            if (Checkoutconfig != null && !string.IsNullOrEmpty(Checkoutconfig.CurrentValue) && Checkoutconfig.CurrentValue == "CRV")
                PMSRecieptIsCheckout = false;
            else 
                PMSRecieptIsCheckout = true;
        }
        public static void GetUserRoleAccess()
        {
            UserRoleMapperDTO userrole = UIProcessManager.GetUserRoleByUserId(CurrentLoggedInUser.Id);
            if(userrole != null)
            {
                AllAccessMatrixFunctionList = UIProcessManager.GetFuncwithAccessMatView(userrole.Role, CNETConstantes.PMS_Pointer);
                if (AllAccessMatrixFunctionList != null)
                {
                    ViewFunctWithAccessMDTO HotelAccess = AllAccessMatrixFunctionList.FirstOrDefault(x => x.Description == "Branch Hotel Access");
                    if (HotelAccess != null)
                        UserHasHotelBranchAccess = true;
                }
                else
                    AllAccessMatrixFunctionList = new List<ViewFunctWithAccessMDTO>();
            }
            else
            {
                AllAccessMatrixFunctionList = new List<ViewFunctWithAccessMDTO>();
                MessageBox.Show( "User has No Role !!","Error");
            }
        }

        public static List<VoucherExtensionDefinitionDTO> VoucherExtensionBufferList { get; internal set; }  
        public static List<SystemConstantDTO> ObjectStateDefinitionBufferList { get; internal set; }
        public static List<TaxDTO> TaxBufferList { get; internal set; }
        public static bool EarlyCheckIn { get; internal set; }
        public static int EarlyCheckInArticle { get; internal set; }
        public static DateTime EarlyCheckInUntilTime { get; internal set; }
        public static bool EarlyCheckInChargeMandatory { get; internal set; } 
        public static string CompanyName { get; internal set; }
        public static int? GetPeriodCode(DateTime currentTime)
        {

            PeriodDTO filter = UIProcessManager.GetPeriodByDateAndType(currentTime, CNETConstantes.PERIOD_TYPE_Accounting) ;
           
            if (filter != null)
               return filter.Id;
            else
                return null;
        }
        public static void GetCustomerAndGuestBuffer()
        {
            ConsigneeViewBufferList = new List<ConsigneeDTO>();
            ConsigneeViewBufferList = UIProcessManager.SelectAllConsigneeWithRequiredfield();
            AllGuestConsigneeViewlist = new List<ConsigneeDTO>(); ;
            AllCustomerConsigneeViewlist = new List<ConsigneeDTO>(); ;

            if (ConsigneeViewBufferList != null && ConsigneeViewBufferList.Count > 0)
                AllGuestConsigneeViewlist = ConsigneeViewBufferList.Where(x => x.GslType == CNETConstantes.GUEST).ToList();

            if (ConsigneeViewBufferList != null && ConsigneeViewBufferList.Count > 0)
                AllCustomerConsigneeViewlist = ConsigneeViewBufferList.Where(x => x.GslType == CNETConstantes.CUSTOMER).ToList();

            var CompanyConsigneeDatalist = ConsigneeViewBufferList.Where(x => x.GslType == CNETConstantes.COMPANY).ToList();

            if (CompanyConsigneeDatalist != null && ConsigneeViewBufferList.Count > 0)
                CompanyConsigneeData = UIProcessManager.GetConsigneeViewById(CompanyConsigneeDatalist.FirstOrDefault().Id);


            if (CompanyConsigneeData != null)
                CompanyName = CompanyConsigneeData.FirstName;


            ConsigneeDTO Company = UIProcessManager.GetConsigneeById(CompanyConsigneeDatalist.FirstOrDefault().Id);
            string ImageURL = Company.DefaultImageUrl;

            if (!string.IsNullOrEmpty(ImageURL) && ImageURL.Contains("ftp://192.168.1.8"))
                ImageURL= ImageURL.Replace("192.168.1.8", "196.191.244.132");

            if (!string.IsNullOrEmpty(ImageURL))
                CompanyDefaultLogo = FTPInterface.FTPAttachment.GetImageFromFTP(ImageURL);



        }

        public static Image? CompanyDefaultLogo { get; set; }
        public static void GetHotelBranches()
        {
            if (CompanyConsigneeData != null)
            {
                AllHotelBranchBufferList = UIProcessManager.GetConsigneeUnitByconsignee(CompanyConsigneeData.Id);

                if (AllHotelBranchBufferList != null && AllHotelBranchBufferList.Count > 0)
                    HotelBranchBufferList = AllHotelBranchBufferList.Where(x => x.ParentId != null && x.Type == CNETConstantes.ORG_UNIT_TYPE_Business_Unit && (x.Specialization == CNETConstantes.ORG_UNIT_TYPE_HOTEL_And_Accommodation /*||x.Specialization == CNETConstantes.ORG_UNIT_TYPE_HOTEL ||  x.Specialization == CNETConstantes.ORG_UNIT_TYPE_HOTEL_And_Events*/)).ToList();


                if (HotelBranchBufferList != null && HotelBranchBufferList.Count > 0)
                {
                    HotelBranchBufferList.ForEach(x => x.Id = x.ParentId.Value);
                }
                if (CurrentDevice != null)
                {
                    CurrentDeviceConsigneeUnit = CurrentDevice.ConsigneeUnit;

                    if (CurrentDevice.ConsigneeUnit != null)
                        CurrentDeviceConsigneeUnitData = UIProcessManager.GetConsigneeUnitById(CurrentDevice.ConsigneeUnit.Value);
                }

            }
        }

        public static void LoadPreference()
        {
            PreferenceBufferList = UIProcessManager.SelectAllPreference();
        }

        public static void CheckandGetPMSPreference()
        {
            PreferenceDTO PackagePreference = PreferenceBufferList.FirstOrDefault(x=> x.Description== "Package");
            if(PackagePreference == null)
            {
                PackagePreference = CreatePMSPreference("Package",CNETConstantes.PRODUCT);
            }

            PACKAGE_PEREFERENCE = PackagePreference.Id;
            PreferenceDTO AccomodationPreference = PreferenceBufferList.FirstOrDefault(x => x.Description == "Accomodation");

            if (AccomodationPreference == null)
            {
                AccomodationPreference = CreatePMSPreference("Accomodation",CNETConstantes.SERVICES);
            }
            ACCOMODATION_PREFERENCE_CODE = AccomodationPreference.Id;

            ArticleDTO RoomChargeArticle = UIProcessManager.GetArticleByname("ROOM CHARGE");
            if(RoomChargeArticle == null)
            {
                CreateRoomChargeArticle("ROOM CHARGE", ACCOMODATION_PREFERENCE_CODE);

            }
            else if(RoomChargeArticle.Preference != ACCOMODATION_PREFERENCE_CODE)
            {
                RoomChargeArticle.Preference = ACCOMODATION_PREFERENCE_CODE;
                UIProcessManager.UpdateArticle(RoomChargeArticle);
            }

        }

        private static void CreateRoomChargeArticle(string Name, int aCCOMODATION_PREFERENCE_CODE)
        {
            ArticleDTO Roomchargearticle = new ArticleDTO()
            {
                LocalCode = UIProcessManager.IdGenerater("Article",CNETConstantes.SERVICES, 1,CurrentDeviceConsigneeUnit.Value,false,CurrentDevice.Id),
                Name = Name,
                Preference = aCCOMODATION_PREFERENCE_CODE,
                GslType = CNETConstantes.SERVICES,
                Uom = CNETConstantes.UNITOFMEASURMENTPCS
            };
            ArticleDTO savedarticle = UIProcessManager.CreateArticle(Roomchargearticle);

            if (Roomchargearticle == null)
                MessageBox.Show("Unable to save Room Charge Article.");
        
        }

        private static PreferenceDTO CreatePMSPreference(string preferencedesc, int reference)
        {
            PreferenceDTO preference = new PreferenceDTO()
            {
                SystemConstant = reference,
               //SystemConstant = reference,
               Description = preferencedesc,
               Index =0,
               Value = 0,
               IsActive= true

            }; 
            PreferenceDTO savedpreference = UIProcessManager.CreatePreference(preference);

            if (savedpreference != null)
                return savedpreference;
            else
                return null;
        }

        public static int PACKAGE_PEREFERENCE { get; set; }
        public static int ACCOMODATION_PREFERENCE_CODE { get; set; }
    }
}
