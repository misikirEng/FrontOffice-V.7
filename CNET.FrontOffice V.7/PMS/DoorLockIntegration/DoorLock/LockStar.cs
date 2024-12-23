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
    public class LockStar : IDoorLock
    {



        public bool InitializeLock(List<ConfigurationDTO> deviceConfigs, DeviceDTO device)
        {
            try
            {
                int lStatus;

                lStatus = OpenPort();
                if (lStatus != 0)
                {
                    ShowStatusMessage(lStatus.ToString());
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
            return "Lock Star";
        }


        public string GetCardSN(bool showStatusMessage = true)
        {
            try
            {
                int lStatus;
                StringBuilder cardNo = new StringBuilder(36);
                lStatus = GetCardNo(cardNo, 1);
                if (lStatus == 0)
                {
                    return cardNo.ToString();
                }
                else
                {
                    if (showStatusMessage)
                        ShowStatusMessage(lStatus.ToString());
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


                int lStatus;


                StringBuilder CardData = new StringBuilder(36);
                StringBuilder CNum = new StringBuilder(36);

                lStatus = ReadData(CardData, CNum, 1);


                if (lStatus == 0)
                {
                    cInfo.CardNumber = CNum.ToString();

                    int LockNumberstartindex = CardData.ToString().IndexOf('R') + 1;
                    int LockNumberEndindex = CardData.ToString().IndexOf('S') - 1;

                    cInfo.LockNumber = CardData.ToString().Substring(LockNumberstartindex, LockNumberEndindex - LockNumberstartindex);


                    int StartTimestartindex = CardData.ToString().IndexOf('D') + 1;
                    int StartTimeEndindex = CardData.ToString().IndexOf('O') - 1;

                    string startStr = CardData.ToString().Substring(StartTimestartindex, StartTimeEndindex - StartTimestartindex);


                    int EndTimestartindex = CardData.ToString().IndexOf('O') + 1;
                    int EndTimeEndindex = CardData.ToString().IndexOf('L') - 1;


                    string endStr = CardData.ToString().Substring(EndTimestartindex, EndTimeEndindex - EndTimestartindex);
                    string format = GetStringFormatOfDate();

                    cInfo.StartDate = DateTime.ParseExact(startStr, format, CultureInfo.InvariantCulture).ToString("dd-MM-yyyy HH:mm");
                    cInfo.EndDate = DateTime.ParseExact(endStr, format, CultureInfo.InvariantCulture).ToString("dd-MM-yyyy HH:mm");

                }
                else
                {
                    if (showStatusMessage)
                    {
                        ShowStatusMessage(lStatus.ToString());
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

                int lStatus;
                string intime = startDate.ToString(GetStringFormatOfDate());
                string outtime = endDate.ToString(GetStringFormatOfDate());

                StringBuilder CData = new StringBuilder("T0|" + "R" + lockNumber + "|S1|D" + intime + "|O" + outtime + "|L0");

                StringBuilder CNum = new StringBuilder(36);
                lStatus = IssueData(CData, CNum, 1);


                if (lStatus != 0)
                {
                    ShowStatusMessage(lStatus.ToString());
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
                int lStatus;
                StringBuilder CNum = new StringBuilder(36);

                lStatus = CancelCard(CNum, 1);

                if (lStatus != 0)
                {
                    ShowStatusMessage(lStatus.ToString());
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
            return "yyMMddhhmm";
        }


        public void ShowStatusMessage(int status)
        {
            switch (status.ToString())
            {
                case "0":
                    //SystemMessage.ShowModalInfoMessage("SUCCESS!!", "MESSAGE");
                    break;
                case "1":
                    SystemMessage.ShowModalInfoMessage("ERROR. No Card.", "ERROR");
                    break;
                case "2":
                    SystemMessage.ShowModalInfoMessage("ERROR. Card Error.", "ERROR");
                    break;
                case "3":
                    SystemMessage.ShowModalInfoMessage("ERROR. Password Error.", "ERROR");
                    break;
                case "4":
                    SystemMessage.ShowModalInfoMessage("ERROR. Serial Port Communication Error.", "ERROR");
                    break;
                case "5":
                    SystemMessage.ShowModalInfoMessage("ERROR. Authorization Error.", "ERROR");
                    break;
                case "7":
                    SystemMessage.ShowModalInfoMessage("ERROR. New Card.", "ERROR");
                    break;
                case "10":
                    SystemMessage.ShowModalInfoMessage("ERROR. Data Error.", "ERROR");
                    break;
                case "11":
                    SystemMessage.ShowModalInfoMessage("ERROR. Configuration file Error.", "ERROR");
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
                case "1":
                    SystemMessage.ShowModalInfoMessage("ERROR. No Card.", "ERROR");
                    break;
                case "2":
                    SystemMessage.ShowModalInfoMessage("ERROR. Card Error.", "ERROR");
                    break;
                case "3":
                    SystemMessage.ShowModalInfoMessage("ERROR. Password Error.", "ERROR");
                    break;
                case "4":
                    SystemMessage.ShowModalInfoMessage("ERROR. Serial Port Communication Error.", "ERROR");
                    break;
                case "5":
                    SystemMessage.ShowModalInfoMessage("ERROR. Authorization Error.", "ERROR");
                    break;
                case "7":
                    SystemMessage.ShowModalInfoMessage("ERROR. New Card.", "ERROR");
                    break;
                case "10":
                    SystemMessage.ShowModalInfoMessage("ERROR. Data Error.", "ERROR");
                    break;
                case "11":
                    SystemMessage.ShowModalInfoMessage("ERROR. Configuration file Error.", "ERROR");
                    break;
                default:
                    SystemMessage.ShowModalInfoMessage("ERROR. OPERATION IS NOT SUCCESSFULL.", "ERROR");
                    break;
            }
        }


















        [DllImport(@"lib\LockStar\NewICdll.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int OpenPort();

        [DllImport(@"lib\LockStar\NewICdll.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int ClosePort();

        [DllImport(@"lib\LockStar\NewICdll.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int BeepNow();

        [DllImport(@"lib\LockStar\NewICdll.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int GetCardNo(StringBuilder cardNo, int nBeepFlag);

        [DllImport(@"lib\LockStar\NewICdll.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int ReadData(StringBuilder CardData, StringBuilder CNum, int nBeepFlag);

        [DllImport(@"lib\LockStar\NewICdll.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int IssueData(StringBuilder CData, StringBuilder CNum, int nBeepFlag);

        [DllImport(@"lib\LockStar\NewICdll.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int CancelCard(StringBuilder CNum, int nBeepFlag);
    }
}
