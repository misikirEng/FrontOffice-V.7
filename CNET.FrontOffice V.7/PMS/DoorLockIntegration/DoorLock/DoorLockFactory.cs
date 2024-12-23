using CNET.ERP.Client.Common.UI;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CNET_V7_Domain.Domain.SettingSchema;
using ProcessManager;
using CNET.FrontOffice_V._7.PMS.Common_Classes;
using CNET_V7_Domain.Domain.ArticleSchema;

namespace CNET.FrontOffice_V._7.PMS.DoorLockIntegration.DoorLock
{
    public enum DoorLockType
    {
        MOLLY = 1,
        DELUNS = 2,
        ADEL = 3,
        BETECH = 4,
        DIGI = 5,
        Syron = 6,
        VINCARD = 7,
        ADELNEW = 8,
        BETECHNEW = 9,
        ELA = 10,
        LOCKSTAR = 11,
        DELUNSV10 = 12,

    }

    public class DoorLockFactory
    {

        public static int EXCEPTION_MESSAGE = 100;
        public static int ABORT_MESSAGE = 101;
        public static DoorLockType CURRENT_DOOR_LOCK;

        public IDoorLock GetDoorLock(bool showStatusMessage = true)
        {
            IDoorLock dLock = null;

            //read device setting
            DeviceDTO doorLockDevice = UIProcessManager.GetDeviceByhostandpreference(LocalBuffer.LocalBuffer.CurrentDevice.Id, CNETConstantes.DEVICE_DOOR_LOCK);
            if (doorLockDevice == null)
            {
                if (showStatusMessage)
                    SystemMessage.ShowModalInfoMessage("Please register your door lock device!", "ERROR");
                return null;
            }

            ArticleDTO ArticleExt = UIProcessManager.GetArticleById(doorLockDevice.Article);
            if (ArticleExt == null || string.IsNullOrEmpty(ArticleExt.Model))
            {
                SystemMessage.ShowModalInfoMessage("Unable to get door lock device model!", "ERROR");
                return null;
            }

            if (ArticleExt.Model.Trim().ToLower() == "molly lock")
            {
                dLock = new MollyLock();
                CURRENT_DOOR_LOCK = DoorLockType.MOLLY;
            }
            else if (ArticleExt.Model.Trim().ToLower() == "adel")
            {
                dLock = new AdelLock();
                CURRENT_DOOR_LOCK = DoorLockType.ADEL;
            }
            else if (ArticleExt.Model.Trim().ToLower() == "deluns")
            {
                dLock = new DelunsLock();
                CURRENT_DOOR_LOCK = DoorLockType.DELUNS;
            }
            else if (ArticleExt.Model.Trim().ToLower() == "deluns v10")
            {
                dLock = new DelunsLockV10();
                CURRENT_DOOR_LOCK = DoorLockType.DELUNSV10;
            }
            else if (ArticleExt.Model.Trim().ToLower() == "be-tech old")
            {
                dLock = new BeTech();
                CURRENT_DOOR_LOCK = DoorLockType.BETECH;
            }
            else if (ArticleExt.Model.Trim().ToLower() == "be-tech new")
            {
                dLock = new BeTechNew();
                CURRENT_DOOR_LOCK = DoorLockType.BETECHNEW;
            }
            else if (ArticleExt.Model.Trim().ToLower() == "digi")
            {
                 dLock = new DIGI();
                CURRENT_DOOR_LOCK = DoorLockType.DIGI;
            }
            else if (ArticleExt.Model.Trim().ToLower() == "syron")
            {
                dLock = new Syron();
                CURRENT_DOOR_LOCK = DoorLockType.Syron;
            }
            else if (ArticleExt.Model.Trim().ToLower() == "vincard")
            {
                dLock = new VINCard();
                CURRENT_DOOR_LOCK = DoorLockType.VINCARD;
            }
            else if (ArticleExt.Model.Trim().ToLower() == "adel new")
            {
                dLock = new AdelLockNew();
                CURRENT_DOOR_LOCK = DoorLockType.ADELNEW;
            }
            else if (ArticleExt.Model.Trim().ToLower() == "ela")
            {
                dLock = new ElaLock();
                CURRENT_DOOR_LOCK = DoorLockType.ELA;
            }
            else if (ArticleExt.Model.Trim().ToLower() == "lock star")
            {
                dLock = new LockStar();
                CURRENT_DOOR_LOCK = DoorLockType.LOCKSTAR;
            }
            else
            {
                SystemMessage.ShowModalInfoMessage("Unknown Door lock Model.", "ERROR");
                return null;
            }

            //read device configuration
            var deviceConfigs = LocalBuffer.LocalBuffer.ConfigurationBufferList.Where(c => c.Reference == doorLockDevice.Id.ToString()).ToList();
            if (deviceConfigs != null && deviceConfigs.Count > 0)
            {
                bool status = false;
                try
                {
                    status = dLock.InitializeLock(deviceConfigs, doorLockDevice);
                }
                catch
                {
                    SystemMessage.ShowModalInfoMessage("Fail to Initialize Door lock.", "ERROR");
                    status = false;
                }
                if (!status)
                {
                    return null;
                }
            }
            else
            {
                SystemMessage.ShowModalInfoMessage("Please save configuration settings for the current device.", "ERROR");
                return null;
            }
            return dLock;
        }
    }
}
