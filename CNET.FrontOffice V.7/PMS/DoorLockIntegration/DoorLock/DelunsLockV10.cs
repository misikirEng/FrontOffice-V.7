
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
    public class DelunsLockV10 : IDoorLock
    {
        private short _fusb = 1;
        private int _dai = 0;
        private short _deadbolt = 1;
        private short _pDoors = 1;
        private int _coId = 0;
        private int _cardNo = 1;

        private short _successBuzzer = 100;
        private short _errorBuzzer = 50;

        public bool InitializeLock(List<ConfigurationDTO> deviceConfigs, DeviceDTO device)
        {
            try
            {
                //encoder type
                var encoderTypeConfig = deviceConfigs.FirstOrDefault(c => c.Attribute.ToLower() == "deluns encoder type");
                if (encoderTypeConfig != null)
                {
                    if (encoderTypeConfig.CurrentValue.ToLower() == "usb")
                        _fusb = 0;
                    else if (encoderTypeConfig.CurrentValue.ToLower() == "prousb")
                        _fusb = 1;
                }

                //deadbolt
                var deadboltConfig = deviceConfigs.FirstOrDefault(c => c.Attribute.ToLower() == "unlock deadbolt");
                if (deadboltConfig != null)
                {
                    if (deadboltConfig.CurrentValue.ToLower() == "true")
                        _deadbolt = 1;
                    else if (deadboltConfig.CurrentValue.ToLower() == "false")
                        _deadbolt = 0;
                }

                //public doors
                var publicDoorsConfig = deviceConfigs.FirstOrDefault(c => c.Attribute.ToLower() == "unlock public doors");
                if (publicDoorsConfig != null)
                {
                    if (publicDoorsConfig.CurrentValue.ToLower() == "true")
                        _pDoors = 1;
                    else if (publicDoorsConfig.CurrentValue.ToLower() == "false")
                        _pDoors = 0;
                }

                int st = initializeUSB(_fusb);
                if (st != 0)
                {
                    Buzzer(_fusb, _errorBuzzer);
                    SystemMessage.ShowModalInfoMessage("ERROR. UNABLE TO INITIALIZE DOOR LOCK", "ERROR");
                    return false;
                }
                Buzzer(_fusb, _successBuzzer);
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
            return "Deluns V10 Door Lock";
        }

        public void ShowStatusMessage(int status)
        {
            switch (status)
            {
                case 0:
                    //SystemMessage.ShowModalInfoMessage("SUCCESS!!", "MESSAGE");
                    break;
                case 1:
                    SystemMessage.ShowModalInfoMessage("ERROR. INVALID CARD DATA.", "ERROR");
                    break;
                case 2:
                    SystemMessage.ShowModalInfoMessage("ERROR. NOT A CARD IN THE HOTEL.", "ERROR");
                    break;
                case 3:
                    SystemMessage.ShowModalInfoMessage("ERROR. NOT A GUEST CARD.", "ERROR");
                    break;

                default:
                    SystemMessage.ShowModalInfoMessage("ERROR. OPERATION IS FAILED.", "ERROR");
                    break;
            }
        }

        public string GetCardSN(bool showStatusMessage = true)
        {
            try
            {
                StringBuilder buff = new StringBuilder(200);
                int st = ReadCard(_fusb, buff);
                if (st == 0)
                {
                    string cardNumber = buff.ToString().Substring(24, 8);
                    return cardNumber;
                }
                else
                {
                    SystemMessage.ShowModalInfoMessage("ERROR. UNABLE TO READ CARD NUMBER", "ERROR");
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

                StringBuilder buff = new StringBuilder(500);
                int st = ReadCard(_fusb, buff);

                if (st == 0)
                {
                    //if(showStatusMessage)
                    //Buzzer(_fusb, _successBuzzer);

                    //card number
                    cInfo.CardNumber = buff.ToString().Substring(24, 8);


                    cInfo.LockNumber = "UNKOWN";
                    cInfo.StartDate = "UNKOWN";
                    cInfo.EndDate = "UNKNOWN";

                    //Read hotel Id
                    ReadHotelId();

                    //Read Lock Number 
                    StringBuilder lockNumber = new StringBuilder(6);
                    int lockNumStatus = GetGuestLockNoByCardDataStr(_coId, buff, lockNumber);
                    if (lockNumStatus == 0)
                    {
                        cInfo.LockNumber = lockNumber.ToString();
                    }

                    //Read end date
                    StringBuilder expiryDate = new StringBuilder(10);
                    int expiryDateStatus = GetGuestETimeByCardDataStr(_coId, buff, expiryDate);
                    if (expiryDateStatus == 0)
                    {
                        try
                        {
                            string edate = expiryDate.ToString();
                            string format = GetStringFormatOfDate();
                            cInfo.EndDate = DateTime.ParseExact(expiryDate.ToString(), format, CultureInfo.InvariantCulture).ToString("dd/MM/yyy HH:mm");


                        }
                        catch (Exception ex)
                        {

                        }
                    }


                }
                else
                {
                    if (showStatusMessage)
                    {
                        //Buzzer(_fusb, _errorBuzzer);
                        SystemMessage.ShowModalInfoMessage("ERROR. UNABLE TO READ CARD DATA", "ERROR");
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

                if (lockNumber.Length != 6)
                {
                    XtraMessageBox.Show("Length of lock number must be 6.", "Door Lock", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                string intime = startDate.ToString(GetStringFormatOfDate());
                string outtime = endDate.ToString(GetStringFormatOfDate());


                StringBuilder sbStart = new StringBuilder(10);
                sbStart.Append(intime);

                StringBuilder sbEndDate = new StringBuilder(10);
                sbEndDate.Append(outtime);

                StringBuilder sbLock = new StringBuilder(6);
                sbLock.Append(lockNumber); //Lock number ===> 01020399 --> Building 01, Floor 02, Room ID 03

                //Read Hotel Id
                ReadHotelId();

                StringBuilder cardBuff = new StringBuilder(500);
                int st = GuestCard(_fusb, _coId, _cardNo, _dai, _deadbolt, 0, sbStart, sbEndDate, sbLock, cardBuff);
                //int st = GuestCard(_fusb, _coId, _cardNo, 6, _deadbolt, _pDoors, sbStart, sbEndDate, sbLock, cardBuff);
                if (st != 0)
                {
                    Buzzer(_fusb, _errorBuzzer);
                    SystemMessage.ShowModalInfoMessage("ERROR. UNABLE TO ISSUE GUEST CARD", "ERROR");
                    return false;
                }

                Buzzer(_fusb, _successBuzzer);
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
                StringBuilder buff = new StringBuilder();
                int st = CardErase(_fusb, _coId, buff);
                if (st != 0)
                {
                    Buzzer(_fusb, _errorBuzzer);
                    SystemMessage.ShowModalInfoMessage("ERROR. UNABLE TO ERASE CARD", "ERROR");
                    return false;
                }
                Buzzer(_fusb, _successBuzzer);
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
            return "yyMMddHHmm";
        }


        //private method
        private void ReadHotelId()
        {
            try
            {
                StringBuilder buff = new StringBuilder(500);
                int st = ReadCard(_fusb, buff);
                if (st == 0)
                {
                    string s = buff.ToString().Substring(10, 4);
                    int i = Convert.ToInt16(s, 16) % 16384; //Convert Hex to int and take it's modulo
                    s = buff.ToString().Substring(8, 2);
                    _coId = i + Convert.ToInt16(s, 16) * 65536;
                    //_coId = 182383;
                }
                else
                {
                    XtraMessageBox.Show("Hotel Id is not identified!", "Door Lock", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                }
            }
            catch (Exception ex)
            {

                XtraMessageBox.Show("Hotel Id is not identified!", "Door Lock", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
        }


        #region DLL Import

        [DllImport(@"lib\deluns\Delus V10\proRFL.dll", EntryPoint = "GetDLLVersion")]
        public static extern int GetDLLVersion(StringBuilder buffer);

        [DllImport(@"lib\deluns\Delus V10\proRFL.dll", EntryPoint = "initializeUSB")]
        public static extern int initializeUSB(short fUSB);

        [DllImport(@"lib\deluns\Delus V10\proRFL.dll", EntryPoint = "Buzzer")]
        public static extern int Buzzer(short fUSB, int t);

        [DllImport(@"lib\deluns\Delus V10\proRFL.dll", EntryPoint = "ReadCard")]
        public static extern int ReadCard(short fUSB, StringBuilder buffData);

        [DllImport(@"lib\deluns\Delus V10\proRFL.dll", EntryPoint = "GuestCard")]
        public static extern int GuestCard(short fUSB, int dlsCoID, int cardNumber, int dai, short deadbolt, short publicDoors,
            StringBuilder BDate, StringBuilder EDate, StringBuilder lockNumber, StringBuilder cardHexStr);

        [DllImport(@"lib\deluns\Delus V10\proRFL.dll", EntryPoint = "CardErase")]
        public static extern int CardErase(short fUSB, int dlsCoID, StringBuilder cardHexStr);

        [DllImport(@"lib\deluns\Delus V10\proRFL.dll", EntryPoint = "GetCardTypeByCardDataStr")]
        public static extern int GetCardTypeByCardDataStr(StringBuilder cardDataStr, StringBuilder cardType);

        [DllImport(@"lib\deluns\Delus V10\proRFL.dll", EntryPoint = "GetGuestLockNoByCardDataStr")]
        public static extern int GetGuestLockNoByCardDataStr(int dlsColID, StringBuilder cardDataStr, StringBuilder lockNumber);

        [DllImport(@"lib\deluns\Delus V10\proRFL.dll", EntryPoint = "GetGuestETimeByCardDataStr")]
        public static extern int GetGuestETimeByCardDataStr(int dlsColID, StringBuilder cardDataStr, StringBuilder lockNumber);

        #endregion
    }
}
