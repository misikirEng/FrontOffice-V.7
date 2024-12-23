using CNET.ERP.Client.Common.UI;

using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using CNET_V7_Domain.Domain.SettingSchema;
using CNET.FrontOffice_V._7.PMS.Common_Classes;

namespace CNET.FrontOffice_V._7.PMS.DoorLockIntegration.DoorLock
{
    public class BeTechNew : IDoorLock
    {


        #region DLL Import

        [DllImport(@"lib\betech\New\btlock57.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int Write_Guest_Card(int Port, int ReaderType, int SectorNo, StringBuilder HotelPwd,
            int CardNo, int GuestSN, int GuestIdx, StringBuilder DoorID, StringBuilder SuitDoor,
            StringBuilder PubDoor, StringBuilder BeginTime, StringBuilder EndTime);

        [DllImport(@"lib\betech\New\btlock57.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int Read_Guest_Card(int Port, int ReaderType, int SectorNo, StringBuilder HotelPwd,
            ref int CardNo, ref int GuestSN, ref int GuestIdx, StringBuilder DoorID, StringBuilder SuitDoor,
            StringBuilder PubDoor, StringBuilder BeginTime, StringBuilder EndTime);


        [DllImport(@"lib\betech\New\btlock57.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int Read_Guest_Lift(int Port, int ReaderType, int SectorNo, string HotelPwd,
            int BeginAddr, int EndAddr, ref int CardNo, StringBuilder BeginTime, StringBuilder EndTime,
            StringBuilder LiftData);

        [DllImport(@"lib\betech\New\btlock57.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int Write_Guest_Lift(int Port, int ReaderType, int SectorNo, string HotelPwd,
            int CardNo, int BeginAddr, int EndAddr, int MaxAddr, StringBuilder BeginTime, StringBuilder EndTime,
            StringBuilder LiftData);


        [DllImport(@"lib\betech\New\btlock57.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int Write_Guest_PowerSwitch(int Port, int ReaderType, int SectorNo, string PowerSwitchPwd,
            int CardNo, int GuestSex, StringBuilder DoorID, StringBuilder GuestName, StringBuilder BeginTime,
            StringBuilder EndTime, int PowerSwitchType);

        [DllImport(@"lib\betech\New\btlock57.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int Read_Guest_PowerSwitch(int Port, int ReaderType, int SectorNo, string PowerSwitchPwd,
            ref int CardNo, ref int GuestSex, StringBuilder DoorID, StringBuilder GuestName,
            StringBuilder BeginTime, StringBuilder EndTime, int PowerSwitchType);


        [DllImport(@"lib\betech\New\btlock57.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int Reader_Alarm(int Port, int ReaderType, int AlarmCount);

        [DllImport(@"lib\betech\New\btlock57.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int SerialNo_FromNow();

        #endregion



        private short _type = 1; //the only serial encoder type
        private short _comPort = 1;

        private short _sectorNo = 0;
        private string _hotelPassword = "123456"; // should be 6-digit ASCII chars.

        private string _suitDoor = "0000"; //it always has length-4
        private string _pubDoor = "00000000"; //it always has length-8

        private int _cardNo = 1; //scope from 1 to 4294967296
        private int _guestSN = 1; //scope from 1 to 4294967296 ===> the one with larger SN replase a guest card with smaller SN
        private int _guestIdx = 1; //scope from 1 to 255

        private short _successBuzzer = 1;
        private short _errorBuzzer = 2;
        private short _initializeBuzzer = 3;

        public bool InitializeLock(List<ConfigurationDTO> deviceConfigs, DeviceDTO device)
        {
            try
            {
                //Encoder type
                string readerType = "";
                var encoderTypeConfig = deviceConfigs.FirstOrDefault(c => c.Attribute.ToLower() == "betech encoder type");
                if (encoderTypeConfig != null)
                {
                    readerType = encoderTypeConfig.CurrentValue;
                }

                //System Password
                var passConfig = deviceConfigs.FirstOrDefault(c => c.Attribute.ToLower() == "system password");
                if (encoderTypeConfig != null)
                {
                    _hotelPassword = passConfig.CurrentValue;
                }

                //get Serial parameter 
                if (device.SerialPort == null || device.SerialPort.Value == 0)
                {
                    SystemMessage.ShowModalInfoMessage("ERROR. EMPTY SERIAL PORT.", "ERROR");
                    return false;
                }
                _comPort = Convert.ToInt16(device.SerialPort);

                if (readerType.ToLower() == "rw_21")
                    _type = 1;
                else if (readerType.ToLower() == "rw_33")
                    _type = 2;
                else if (readerType.ToLower() == "rw_26b")
                    _type = 3;
                else if (readerType.ToLower() == "rw_41")
                    _type = 4;
                else if (readerType.ToLower() == "rw_49a")
                    _type = 5;
                else
                {
                    SystemMessage.ShowModalInfoMessage("ERROR. UNKNOWN READER/WRITER TYPE.", "ERROR");
                    return false;
                }

                if (_type == 1)
                {
                    var portConfig = deviceConfigs.FirstOrDefault(c => c.Attribute.ToLower() == "port");
                    if (portConfig != null && !string.IsNullOrEmpty(portConfig.CurrentValue))
                    {
                        _comPort = Convert.ToInt16(encoderTypeConfig.CurrentValue);
                    }
                }

                int st = Reader_Alarm(_comPort, _type, _initializeBuzzer);
                if (st != 0)
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
            return "BE-TECH Door Lock";
        }

        public void ShowStatusMessage(int status)
        {
            switch (status)
            {
                case 0:
                    //SystemMessage.ShowModalInfoMessage("SUCCESS!!", "MESSAGE");
                    break;
                case 1:
                    SystemMessage.ShowModalInfoMessage("ERROR. UNABLE TO OPEN SERIAL COM PORT.", "ERROR");
                    break;
                case 2:
                    SystemMessage.ShowModalInfoMessage("ERROR. NO CARD.", "ERROR");
                    break;
                case 3:
                    SystemMessage.ShowModalInfoMessage("ERROR. INVALID CARD TYPE", "ERROR");
                    break;
                case 4:
                    SystemMessage.ShowModalInfoMessage("ERROR. UNABLE TO READ CARD.", "ERROR");
                    break;
                case 5:
                    SystemMessage.ShowModalInfoMessage("ERROR. HOTEL PASSWORD ERROR.", "ERROR");
                    break;
                case 6:
                    SystemMessage.ShowModalInfoMessage("ERROR. WRITE CARD ERROR", "ERROR");
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
                return ReadCardData(showStatusMessage).CardNumber;
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
                int st = 255; //failer
                StringBuilder tmpDoorID = new StringBuilder();
                StringBuilder tmpSuitDoor = new StringBuilder();
                StringBuilder tmpPubDoor = new StringBuilder();
                StringBuilder intime = new StringBuilder();
                StringBuilder outtime = new StringBuilder();
                StringBuilder psw = new StringBuilder(_hotelPassword);

                st = Read_Guest_Card(_comPort, _type, _sectorNo, psw,
                 ref _cardNo, ref _guestSN, ref _guestIdx, tmpDoorID, tmpSuitDoor, tmpPubDoor, intime, outtime);

                if (st == 0)
                {
                    cInfo.CardNumber = _guestSN.ToString();
                    cInfo.LockNumber = tmpDoorID.ToString();
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

                string[] splited = lockNumber.Split(',');
                if (splited == null || splited.Length != 3)
                {
                    XtraMessageBox.Show("Invalid lock number format!", "Door Lock", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                _guestIdx = Convert.ToInt32(splited[1]);
                string intime = startDate.ToString(GetStringFormatOfDate());
                string outtime = endDate.ToString(GetStringFormatOfDate());

                StringBuilder tmpDoorID = new StringBuilder(splited[0]);
                StringBuilder tmpSuitDoor = new StringBuilder(_suitDoor);
                StringBuilder tmpPubDoor = new StringBuilder(_pubDoor);
                StringBuilder tmpBndTime = new StringBuilder(intime);
                StringBuilder tmpEndTime = new StringBuilder(outtime);

                //guest SN
                //Convert the time of computer to 4-bits integer serial number
                string lastGuestSN = splited[2];
                if (lastGuestSN == "FFFF")
                    _guestSN = SerialNo_FromNow();
                else
                    _guestSN = Convert.ToInt32(lastGuestSN);

                StringBuilder psw = new StringBuilder(_hotelPassword);
                int st = Write_Guest_Card(_comPort, _type, _sectorNo, psw,
                 _cardNo, _guestSN, _guestIdx, tmpDoorID, tmpSuitDoor, tmpPubDoor, tmpBndTime, tmpEndTime);
                if (st != 0)
                {
                    Reader_Alarm(_comPort, _type, _successBuzzer);
                    ShowStatusMessage(st);
                    return false;
                }

                Reader_Alarm(_comPort, _type, _errorBuzzer);
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
            //no implementation of clearining card for be-tech
            return true;
        }

        public string GetStringFormatOfDate()
        {
            return "yyMMddHHmm"; ;
        }



    }
}
