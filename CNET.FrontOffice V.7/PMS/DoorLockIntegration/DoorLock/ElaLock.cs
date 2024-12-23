using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using CNET_V7_Domain.Domain.SettingSchema;
using CNET.FrontOffice_V._7.PMS.Common_Classes;

namespace CNET.FrontOffice_V._7.PMS.DoorLockIntegration.DoorLock
{
    public class ElaLock : IDoorLock
    {



        public bool InitializeLock(List<ConfigurationDTO> deviceConfigs, DeviceDTO device)
        {
            try
            {
                //Adel Software
                var adelSystem = deviceConfigs.FirstOrDefault(c => c.Attribute.ToLower() == "ela system url");
                if (adelSystem == null || string.IsNullOrEmpty(adelSystem.CurrentValue))
                {
                    SystemMessage.ShowModalInfoMessage("ERROR. Please Set Ela Software Location.", "ERROR");
                    return false;
                }
                string server = adelSystem.CurrentValue;


                //string server = @"D:\New Folder";

                string user = "DllUser";

                int LockSoftware = 1;

                uint lStatus;


                lStatus = StartSession(LockSoftware, server, user, 0);
                if (lStatus != 0)
                {
                    ShowStatusMessage(lStatus.ToString("X"));
                }


                return true;

            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Exception has occured in initializing door lock. Detail:: " + ex.Message, "Door Lock", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public string GetDoorLockName()
        {
            return "Ela Door Lock";
        }


        int Port = 0;
        public string GetCardSN(bool showStatusMessage = true)
        {
            try
            {
                uint lStatus, CardNo;
                CardNo = 0;
                lStatus = ReadCardID(Port, ref CardNo);

                if (lStatus == 0)
                {
                    return CardNo.ToString("X");
                }
                else
                {

                    if (showStatusMessage)
                        ShowStatusMessage(lStatus.ToString("X"));
                    return null;
                }

            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Exception has occured in reading card serial number. Detail:: " + ex.Message, "Door Lock", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null; ;
            }
        }


        public CardInfo ReadCardData(bool showStatusMessage = true)
        {
            CardInfo cInfo = new CardInfo();
            try
            {


                uint lStatus;
                int CardNo, Breakfast, CardStatus;

                StringBuilder RoomNo = new StringBuilder("", 64);
                StringBuilder Holder = new StringBuilder("", 64);
                StringBuilder IDNo = new StringBuilder("", 64);
                StringBuilder TimeStr = new StringBuilder("", 64);
                StringBuilder Door = new StringBuilder("", 128);
                StringBuilder Lift = new StringBuilder("", 128);


                CardNo = CardStatus = Breakfast = 0;


                lStatus = ReadKeyCard(Port, RoomNo, Door, Lift, TimeStr, Holder, IDNo, ref CardNo, ref CardStatus, ref Breakfast);


                if (lStatus == 0)
                {
                    cInfo.CardNumber = CardNo.ToString();
                    cInfo.LockNumber = RoomNo.ToString();

                    string startStr = TimeStr.ToString().Substring(0, 12);
                    string endStr = TimeStr.ToString().Substring(12, 12);
                    string format = GetStringFormatOfDate();

                    cInfo.StartDate = DateTime.ParseExact(startStr, format, CultureInfo.InvariantCulture).ToString("dd-MM-yyyy HH:mm");
                    cInfo.EndDate = DateTime.ParseExact(endStr, format, CultureInfo.InvariantCulture).ToString("dd-MM-yyyy HH:mm");

                }
                else
                {
                    if (showStatusMessage)
                    {
                        ShowStatusMessage(lStatus.ToString("X"));
                    }

                }

                return cInfo;

            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Exception has occured in reading card data. Detail:: " + ex.Message, "Door Lock", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return cInfo;
            }
        }


        public bool IssueGuestCard(string lockNumber, DateTime startDate, DateTime endDate, bool isDuplicate = false)
        {
            try
            {
                if (string.IsNullOrEmpty(lockNumber))
                {
                    XtraMessageBox.Show("Invalid lock number!", "Door Lock", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                uint lStatus;
                int CardNo, OverFlag, Breakfast;
                string RoomNo, Holder, IDNo, TimeStr;

                OverFlag = 1;

                Breakfast = 0;

                RoomNo = lockNumber;

                Holder = "Holder";

                IDNo = "";
                string intime = startDate.ToString(GetStringFormatOfDate());
                string outtime = endDate.ToString(GetStringFormatOfDate());


                StringBuilder sbTime = new StringBuilder(24);
                sbTime.Append(intime);
                sbTime.Append(outtime);
                TimeStr = sbTime.ToString();

                CardNo = 0;


                lStatus = NewKey(Port, RoomNo, "", "", TimeStr, Holder, IDNo, Breakfast, OverFlag, ref CardNo);

                if (lStatus != 0)
                {
                    ShowStatusMessage(lStatus.ToString("X"));
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Exception has occured in issuing guest card. Detail:: " + ex.Message, "Door Lock", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }



        public bool ClearCard(string lockNumber = null)
        {
            try
            {
                uint lStatus;
                int CardNo;
                CardNo = 0;

                lStatus = EraseKeyCard(Port, CardNo);

                if (lStatus != 0)
                {
                    ShowStatusMessage(lStatus.ToString("X"));
                    return false;
                }
                return true;

            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Exception has occured in clearing card. Detail:: " + ex.Message, "Door Lock", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }


        public string GetStringFormatOfDate()
        {
            return "yyyyMMddhhmm";
        }


        public void ShowStatusMessage(int status)
        {
            switch (status.ToString("X"))
            {
                case "0":
                    //SystemMessage.ShowModalInfoMessage("SUCCESS!!", "MESSAGE");
                    break;
                case "8010000C":
                    SystemMessage.ShowModalInfoMessage("ERROR. Password Error.", "ERROR");
                    break;
                case "81100001":
                    SystemMessage.ShowModalInfoMessage("ERROR. Communication Error .", "ERROR");
                    break;
                case "81300001":
                    SystemMessage.ShowModalInfoMessage("ERROR. SERIAL PORT ERROR.", "ERROR");
                    break;
                case "80100069":
                    SystemMessage.ShowModalInfoMessage("ERROR. Card Removed.", "ERROR");
                    break;
                case "81100002":
                    SystemMessage.ShowModalInfoMessage("ERROR. Card damaged.", "ERROR");
                    break;
                case "FFFF0001":
                    SystemMessage.ShowModalInfoMessage("ERROR. Card type Error.", "ERROR");
                    break;
                case "FFFF0002":
                    SystemMessage.ShowModalInfoMessage("ERROR. Card Replaced.", "ERROR");
                    break;
                case "FFFF0003":
                    SystemMessage.ShowModalInfoMessage("ERROR. Blank Card.", "ERROR");
                    break;
                case "FFFF0004":
                    SystemMessage.ShowModalInfoMessage("ERROR. Illegal Card.", "ERROR");
                    break;
                case "FFFF0005":
                    SystemMessage.ShowModalInfoMessage("ERROR. Group Card.", "ERROR");
                    break;
                case "FFFF0006":
                    SystemMessage.ShowModalInfoMessage("ERROR. Blanck group Card.", "ERROR");
                    break;
                case "FFFF0007":
                    SystemMessage.ShowModalInfoMessage("ERROR. Not Blank Card.", "ERROR");
                    break;
                case "FFFF0008":
                    SystemMessage.ShowModalInfoMessage("ERROR. COM Port open Error.", "ERROR");
                    break;
                case "FFFF0009":
                    SystemMessage.ShowModalInfoMessage("ERROR. COM port communication Error.", "ERROR");
                    break;
                case "FFFF1001":
                    SystemMessage.ShowModalInfoMessage("ERROR. Initialization function was not invoked.", "ERROR");
                    break;
                case "FFFF1002":
                    SystemMessage.ShowModalInfoMessage("ERROR. Defined guest not exist.", "ERROR");
                    break;
                case "FFFF1003":
                    SystemMessage.ShowModalInfoMessage("ERROR. Card info not exist.", "ERROR");
                    break;
                case "FFFF1004":
                    SystemMessage.ShowModalInfoMessage("ERROR. Not guest Card.", "ERROR");
                    break;
                case "FFFF1005":
                    SystemMessage.ShowModalInfoMessage("ERROR. Wrong room no.", "ERROR");
                    break;
                case "FFFF1006":
                    SystemMessage.ShowModalInfoMessage("ERROR. Wrong common door.", "ERROR");
                    break;
                case "FFFF3000":
                    SystemMessage.ShowModalInfoMessage("ERROR. SQL execution Error.", "ERROR");
                    break;
                case "FFFF3001":
                    SystemMessage.ShowModalInfoMessage("ERROR. SQL Connection Error.", "ERROR");
                    break;
                case "FFFF3002":
                    SystemMessage.ShowModalInfoMessage("ERROR. System Parameter not exist.", "ERROR");
                    break;
                case "FFFF3003":
                    SystemMessage.ShowModalInfoMessage("ERROR. Wrong Serial Number.", "ERROR");
                    break;
                case "FFFF4000":
                    SystemMessage.ShowModalInfoMessage("ERROR. Interface authentication code not exist.", "ERROR");
                    break;
                default:
                    SystemMessage.ShowModalInfoMessage("ERROR. OPERATION IS NOT SUCCESSFULL.", "ERROR");
                    break;
            }
        }


        public void ShowStatusMessage(string status)
        {
            switch (status)
            {
                case "0":
                    //SystemMessage.ShowModalInfoMessage("SUCCESS!!", "MESSAGE");
                    break;
                case "8010000C":
                    SystemMessage.ShowModalInfoMessage("ERROR. IC Card Not Found.", "ERROR");
                    break;
                case "81100001":
                    SystemMessage.ShowModalInfoMessage("ERROR. Password Error .", "ERROR");
                    break;
                case "81300001":
                    SystemMessage.ShowModalInfoMessage("ERROR. Communication Error.", "ERROR");
                    break;
                case "80100069":
                    SystemMessage.ShowModalInfoMessage("ERROR. Card Removed.", "ERROR");
                    break;
                case "81100002":
                    SystemMessage.ShowModalInfoMessage("ERROR. Card damaged.", "ERROR");
                    break;
                case "FFFF0001":
                    SystemMessage.ShowModalInfoMessage("ERROR. Card type Error.", "ERROR");
                    break;
                case "FFFF0002":
                    SystemMessage.ShowModalInfoMessage("ERROR. Card Replaced.", "ERROR");
                    break;
                case "FFFF0003":
                    SystemMessage.ShowModalInfoMessage("ERROR. Blank Card.", "ERROR");
                    break;
                case "FFFF0004":
                    SystemMessage.ShowModalInfoMessage("ERROR. Illegal Card.", "ERROR");
                    break;
                case "FFFF0005":
                    SystemMessage.ShowModalInfoMessage("ERROR. Group Card.", "ERROR");
                    break;
                case "FFFF0006":
                    SystemMessage.ShowModalInfoMessage("ERROR. Blanck group Card.", "ERROR");
                    break;
                case "FFFF0007":
                    SystemMessage.ShowModalInfoMessage("ERROR. Not Blank Card.", "ERROR");
                    break;
                case "FFFF0008":
                    SystemMessage.ShowModalInfoMessage("ERROR. COM Port open Error.", "ERROR");
                    break;
                case "FFFF0009":
                    SystemMessage.ShowModalInfoMessage("ERROR. COM port communication Error.", "ERROR");
                    break;
                case "FFFF1001":
                    SystemMessage.ShowModalInfoMessage("ERROR. Initialization function was not invoked.", "ERROR");
                    break;
                case "FFFF1002":
                    SystemMessage.ShowModalInfoMessage("ERROR. Defined guest not exist.", "ERROR");
                    break;
                case "FFFF1003":
                    SystemMessage.ShowModalInfoMessage("ERROR. Card info not exist.", "ERROR");
                    break;
                case "FFFF1004":
                    SystemMessage.ShowModalInfoMessage("ERROR. Not guest Card.", "ERROR");
                    break;
                case "FFFF1005":
                    SystemMessage.ShowModalInfoMessage("ERROR. Wrong room no.", "ERROR");
                    break;
                case "FFFF1006":
                    SystemMessage.ShowModalInfoMessage("ERROR. Wrong common door.", "ERROR");
                    break;
                case "FFFF3000":
                    SystemMessage.ShowModalInfoMessage("ERROR. SQL execution Error.", "ERROR");
                    break;
                case "FFFF3001":
                    SystemMessage.ShowModalInfoMessage("ERROR. SQL Connection Error.", "ERROR");
                    break;
                case "FFFF3002":
                    SystemMessage.ShowModalInfoMessage("ERROR. System Parameter not exist.", "ERROR");
                    break;
                case "FFFF3003":
                    SystemMessage.ShowModalInfoMessage("ERROR. Wrong Serial Number.", "ERROR");
                    break;
                case "FFFF4000":
                    SystemMessage.ShowModalInfoMessage("ERROR. Interface authentication code not exist.", "ERROR");
                    break;
                default:
                    SystemMessage.ShowModalInfoMessage("ERROR. OPERATION IS NOT SUCCESSFULL.", "ERROR");
                    break;
            }
        }
        [DllImport(@"lib\Ela\LockDll.dll", CharSet = CharSet.Ansi)]
        public static extern uint StartSession(int Software, string DBServer, string LogUser, int DBFlag);

        [DllImport(@"lib\Ela\LockDll.dll", CharSet = CharSet.Ansi)]
        public static extern uint EndSession();

        [DllImport(@"lib\Ela\LockDll.dll", CharSet = CharSet.Ansi)]
        public static extern uint ChangeLogUser(string LogUser);

        [DllImport(@"lib\Ela\LockDll.dll", CharSet = CharSet.Ansi)]
        public static extern uint NewKey(int Port, string RoomNo, string CommonDoor, string LiftFloor, string TimeStr, string Holder,
            string IDNo, int Breakfast, int overflag, ref int CardNo);

        [DllImport(@"lib\Ela\LockDll.dll", CharSet = CharSet.Ansi)]
        public static extern uint AddKey(int Port, string RoomNo, string CommonDoor, string LiftFloor, string TimeStr, string Holder,
            string IDNo, int Breakfast, int overflag, ref int CardNo);

        [DllImport(@"lib\Ela\LockDll.dll", CharSet = CharSet.Ansi)]
        public static extern uint DupKey(int Port, string RoomNo, string CommonDoor, string LiftFloor, string TimeStr, string Holder,
            string IDNo, int Breakfast, int overflag, ref int CardNo);

        [DllImport(@"lib\Ela\LockDll.dll", CharSet = CharSet.Ansi)]
        public static extern uint ReadKeyCard(int Port, StringBuilder RoomNo, StringBuilder CommonDoor, StringBuilder LiftFloor, StringBuilder TimeStr,
            StringBuilder Holder, StringBuilder IDNo, ref int CardNo, ref int CardStatus, ref int Breakfast);

        [DllImport(@"lib\Ela\LockDll.dll", CharSet = CharSet.Ansi)]
        public static extern uint EraseKeyCard(long Port, int CardNo);

        [DllImport(@"lib\Ela\LockDll.dll", CharSet = CharSet.Ansi)]
        public static extern uint CheckOut(string RoomNO, int CardNo);

        [DllImport(@"lib\Ela\LockDll.dll", CharSet = CharSet.Ansi)]
        public static extern uint ReadCardID(int Port, ref uint CardNo);
    }
}
