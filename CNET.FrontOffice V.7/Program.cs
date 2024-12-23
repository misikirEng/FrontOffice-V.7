using DevExpress.LookAndFeel;
using DevExpress.XtraEditors;
using System.Configuration;
using DevExpress.UserSkins;
using log4net;
using log4net.Config;
using log4net.Repository.Hierarchy; 
using System.Diagnostics;
using System.IO;
using CNET_V7_Domain.Domain.ConsigneeSchema;
using System.Globalization;

namespace CNET.FrontOffice_V._7
{ 
    public static class Program
    {

        private static Configuration config = null;
        private static ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();

        [STAThread]
        static void Main()
        {
             
            if (!ReadLauncherConfigurations())
                return;
            else
                UpdateLauncherConfigurations();
             
            PMSDataLogger.InitalizePMSDataLogger();
            PMSDataLogger.LogMessage("Program", "==================================================================");
            PMSDataLogger.LogMessage("Program", "Starting FrontOffice V.7");

            string BaseAddressValue = ReadConfigurationBaseAddress();

            if (string.IsNullOrEmpty(BaseAddressValue))
            {
                XtraMessageBox.Show("Error Reading Configuration !!");
                return;
            }

            API.Manager.HttpSinglton.BaseAddressValue = BaseAddressValue;
            Application.Run(new MainLogin());
        }

        public static bool ReadLauncherConfigurations()
        {
            try
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(Application.StartupPath + "//FrontOffice.PMS.exe");
                return Convert.ToBoolean(config.AppSettings.Settings["POSLauncherLoad"].Value);
            }
            catch (Exception io)
            {
                return false;
            }
        }

        public static bool UpdateLauncherConfigurations()
        {
            try
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(Application.StartupPath + "//FrontOffice.PMS.exe");
                config.AppSettings.Settings["POSLauncherLoad"].Value = "false";
                config.Save();
                return true;
            }
            catch (Exception io)
            {
                return false;
            }
        }

        public static string ReadConfigurationBaseAddress()
        {
            try
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(Application.StartupPath + "//FrontOffice.PMS.dll");
                return config.AppSettings.Settings["BaseAPIAddress"].Value;  
            }
            catch (Exception io)
            {
                return null;
            }
        }
    }
}