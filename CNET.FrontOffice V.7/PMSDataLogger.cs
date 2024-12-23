
using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CNET.FrontOffice_V._7
{
    public class PMSDataLogger
    {
        private static ILog logger;

        private class InitializeLogging
        {
            public static void ConfigureLogging()
            {
                XmlConfigurator.Configure();
            }
        }


        public static void InitalizePMSDataLogger()
        {
            InitializeLogging.ConfigureLogging();
            logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        }


        public static void LogMessage(string Location, string Message, bool Error = false, Exception ex = null)
        {
            if (Error)
            {
                logger.Error("("+ Location+") "+ Message, ex);
            }
            else
            {
                logger.Info("(" + Location + ") " + Message);
            }
        }
    }
}
