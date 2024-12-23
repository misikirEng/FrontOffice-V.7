using ProcessManager;
using CNET_V7_Domain.Domain.CommonSchema;
using CNET_V7_Domain.Domain.SecuritySchema;
using CNET_V7_Domain.Domain.SettingSchema;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNET.FrontOffice_V._7.PMS.Common_Classes
{
    public class ActivityLogManager
    {
      

        //setup Activity
        public static ActivityDTO SetupActivity(DateTime serverTimeStamp, int activityDefCode, int compCode,string remark="" , int? userCode = null)
        {
            //bool isExist = IsExistActDefinitionCode(activityDefCode);

            //if (!isExist) return null;

            UserDTO currentUser = LocalBuffer.LocalBuffer.CurrentLoggedInUser;
            DeviceDTO device = LocalBuffer.LocalBuffer.CurrentDevice;

            ActivityDTO activity = new ActivityDTO()
            {
                ConsigneeUnit = LocalBuffer.LocalBuffer.CurrentDeviceConsigneeUnit,
                TimeStamp = serverTimeStamp,
                Year = serverTimeStamp.Year,
                ActivityDefinition = activityDefCode,
                Month = serverTimeStamp.Month,
                Day = serverTimeStamp.Day,
                Device = device.Id,
                Pointer = compCode,
                Platform = "1",
                User = userCode == null ? currentUser.Id : userCode.Value,
                Remark = remark
            };

            return activity;
        }


        public static ActivityDTO GetActivity(int reference, int activityDefCode)
        {
            ActivityDTO activity = null;

            if (activityDefCode > 0)
            {
                // check the exitance Activity Defination Code
                if (!IsExistActDefinitionCode(activityDefCode)) return null;

                // get the activity of "REINSTATE" defination for this Regstration+
                activity = UIProcessManager.GetActivityByreferenceandactivityDefinition(reference, activityDefCode)
                        .OrderByDescending(a => a.TimeStamp)
                        .FirstOrDefault();
            }

            return activity;
        }
        
        //check the existance of activity definition code
        private static bool IsExistActDefinitionCode(int actDefinitionCode)
        {
            var actDef = UIProcessManager.GetActivityDefinitionById(actDefinitionCode);
            return actDef != null ? true : false;
        }

        // get Activity Defination Code
        public static int? GetActivityDefinationCode(int lookupCode, int type)
        {

            var activityDef = UIProcessManager.SelectAllActivityDefinition().Where(ad => ad.Description == lookupCode && ad.Reference == type).FirstOrDefault();

            return activityDef == null ? null : activityDef.Id;

        }

        
    }
}
