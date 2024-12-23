
using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CNET_V7_Domain.Domain.SettingSchema;
using CNET.FrontOffice_V._7.PMS.Common_Classes;

namespace CNET.FrontOffice_V._7.PMS.DoorLockIntegration.DoorLock
{
    public class MollyLock : IDoorLock
    {

        public bool InitializeLock(List<ConfigurationDTO> deviceConfigs, DeviceDTO device)
        {
            try
            {
                string readerType = "";
                var encoderTypeConfig = deviceConfigs.FirstOrDefault(c => c.Attribute.ToLower() == "molly encoder type");
                if (encoderTypeConfig != null)
                {
                    readerType = encoderTypeConfig.CurrentValue;
                }

                short type = 0;
                if (readerType.ToLower() == "rf_57")
                    type = 4;
                else if (readerType.ToLower() == "rf_50")
                    type = 5;
                else
                {
                    SystemMessage.ShowModalInfoMessage("ERROR. UNKNOWN READER/WRITER TYPE.", "ERROR");
                    return false;
                }

                int st = TP_Configuration(type);
                if (st != 1)
                {
                    ShowStatusMessage(st);
                    return false;
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
            return "Molly Door Lock";
        }

        public void ShowStatusMessage(int status)
        {
            switch (status)
            {
                case 1:
                    //SystemMessage.ShowModalInfoMessage("SUCCESS!!", "MESSAGE");
                    break;
                case -1:
                    SystemMessage.ShowModalInfoMessage("ERROR. NO CARD.", "ERROR");
                    break;
                case -2:
                    SystemMessage.ShowModalInfoMessage("ERROR. NO READER/WRITER.", "ERROR");
                    break;
                case -3:
                    SystemMessage.ShowModalInfoMessage("ERROR. INVALID CARD.", "ERROR");
                    break;
                case -4:
                    SystemMessage.ShowModalInfoMessage("ERROR. INVALID CARD TYPE.", "ERROR");
                    break;
                case -5:
                    SystemMessage.ShowModalInfoMessage("ERROR. UNABLE TO READ CARD.", "ERROR");
                    break;
                case -6:
                    SystemMessage.ShowModalInfoMessage("ERROR. PORT NOT OPEN.", "ERROR");
                    break;
                case -7:
                    SystemMessage.ShowModalInfoMessage("ERROR. END OF DATA CARD.", "ERROR");
                    break;
                case -8:
                    SystemMessage.ShowModalInfoMessage("ERROR. ERROR INPUT.", "ERROR");
                    break;
                case -9:
                    SystemMessage.ShowModalInfoMessage("ERROR. INVALID OPERATION.", "ERROR");
                    break;
                case -10:
                    SystemMessage.ShowModalInfoMessage("ERROR. UNKNOWN ERROR.", "ERROR");
                    break;
                case -11:
                    SystemMessage.ShowModalInfoMessage("ERROR. PORT IN USED.", "ERROR");
                    break;
                case -12:
                    SystemMessage.ShowModalInfoMessage("ERROR. COMMUNICATION ERROR.", "ERROR");
                    break;
                case -20:
                    SystemMessage.ShowModalInfoMessage("ERROR. CLIENT CODE ERROR.", "ERROR");
                    break;
                case -29:
                    SystemMessage.ShowModalInfoMessage("ERROR. NOT REGISTERED.", "ERROR");
                    break;
                case -30:
                    SystemMessage.ShowModalInfoMessage("ERROR. NO AUTHORIZATION CARD INFORMATION.", "ERROR");
                    break;
                case -31:
                    SystemMessage.ShowModalInfoMessage("ERROR. NUMBER OF ROOMS EXCEEDES THE AVAILABLE SECTORS.", "ERROR");
                    break;
                case 100:
                case 101:
                    //do not show any message
                    break;
                default:
                    SystemMessage.ShowModalInfoMessage("ERROR!!", "OPERATION IS FAILED.");
                    break;
            }

        }

        public string GetCardSN(bool showStatusMessage = true)
        {
            try
            {
                StringBuilder cardNumber = new StringBuilder();
                int readStatus = TP_GetCardSnr(cardNumber);
                if (readStatus == 1)
                {
                    return cardNumber.ToString();
                }
                else
                {
                    if (showStatusMessage)
                        ShowStatusMessage(readStatus);
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
                StringBuilder card_snr = new StringBuilder();
                StringBuilder lockNumber = new StringBuilder();
                StringBuilder intime = new StringBuilder();
                StringBuilder outtime = new StringBuilder();
                int st = TP_ReadGuestCard(card_snr, lockNumber, intime, outtime);
                if (st == 1)
                {
                    cInfo.CardNumber = card_snr.ToString();
                    cInfo.LockNumber = lockNumber.ToString();
                    cInfo.StartDate = intime.ToString();
                    cInfo.EndDate = outtime.ToString();
                }
                else
                {
                    if (showStatusMessage)
                    {
                        ShowStatusMessage(st);
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
                string intime = startDate.ToString(GetStringFormatOfDate());
                string outtime = endDate.ToString(GetStringFormatOfDate());
                StringBuilder card_snr = new StringBuilder();
                int st = TP_MakeGuestCard(card_snr, lockNumber, intime, outtime, 0);
                if (st != 1)
                {
                    ShowStatusMessage(st);
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
                StringBuilder card_snr = new StringBuilder();
                int st = TP_CancelCard(card_snr);
                if (st != 1)
                {
                    ShowStatusMessage(st);
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
            return "yyyy-MM-dd HH:mm:ss";
        }

        #region Dll Import 

        [DllImport(@"lib\molly\LockSDK.dll", EntryPoint = "TP_Configuration")]
        public static extern int TP_Configuration(short LockType);

        [DllImport(@"lib\molly\LockSDK.dll", EntryPoint = "TP_MakeGuestCard")]
        public static extern int TP_MakeGuestCard(StringBuilder card_snr, string room_no, string checkin_time, string checkout_time, short iflags);

        [DllImport(@"lib\molly\LockSDK.dll", EntryPoint = "TP_ReadGuestCard")]
        public static extern int TP_ReadGuestCard(StringBuilder card_snr, StringBuilder room_no, StringBuilder checkin_time, StringBuilder checkout_time);

        [DllImport(@"lib\molly\LockSDK.dll", EntryPoint = "TP_CancelCard")]
        public static extern int TP_CancelCard(StringBuilder card_snr);

        [DllImport(@"lib\molly\LockSDK.dll", EntryPoint = "TP_GetCardSnr")]
        public static extern int TP_GetCardSnr(StringBuilder card_snr);

        #endregion

    }
}
