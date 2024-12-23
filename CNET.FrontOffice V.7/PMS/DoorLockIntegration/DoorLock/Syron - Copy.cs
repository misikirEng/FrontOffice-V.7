using CNET.ERP.Client.Common.UI;
using DevExpress.XtraEditors;
using SLock.Reader;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CNET_V7_Domain.Domain.SettingSchema;
using CNET.FrontOffice_V._7.PMS.Common_Classes;

namespace CNET.FrontOffice_V._7.PMS.DoorLockIntegration.DoorLock
{
    public class Syron : IDoorLock
    {



        SerialPortReader se = new SerialPortReader();

        AuxiliaryFunc axdata = new AuxiliaryFunc
        {
            Channel = false,
            Limit = false,
            OpenLock = true,
            SixCall = false,
            Sound = true,
            Weak = false,
        };

        public bool InitializeLock(List<ConfigurationDTO> deviceConfigs, DeviceDTO device)
        {
            se.SerialPortval();
            se.MacInitialization();
            return true;
        }

        public string GetDoorLockName()
        {
            return "Syron Door Lock";
        }


        public void ShowStatusMessage(int status)
        {
            string messag = SerialPortReader.GetReaderReturnResualt(status);
            if (string.IsNullOrEmpty(messag))
            {
                SystemMessage.ShowModalInfoMessage("Successful", "MESSAGE");
            }
            else
            {
                SystemMessage.ShowModalInfoMessage(messag, "ERROR");
            }

        }

        public string GetCardSN(bool showStatusMessage = true)
        {
            string CardNo = "";
            CardInfo cInfo = new CardInfo();
            try
            {

                RoomCardInfo roomCardInfo = new RoomCardInfo();
                List<byte> bys = new List<byte>();
                bool tolog = true;
                int val = se.ReadRoomCardInfo(ref roomCardInfo, ref bys, tolog);

                if (val == 1)
                {
                    CardNo = roomCardInfo.CardID.ToString();
                }
                else
                {
                    if (showStatusMessage)
                    {
                        ShowStatusMessage(val);
                    }

                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Exception has occured in reading card data. Detail:: " + ex.Message, "Door Lock", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return "";
            }
            return CardNo;

        }

        public CardInfo ReadCardData(bool showStatusMessage = true)
        {
            CardInfo cInfo = new CardInfo();
            try
            {

                RoomCardInfo roomCardInfo = new RoomCardInfo();
                List<byte> bys = new List<byte>();
                bool tolog = true;
                int val = se.ReadRoomCardInfo(ref roomCardInfo, ref bys, tolog);

                if (val == 1)
                {
                    cInfo.CardNumber = roomCardInfo.CardID.ToString();
                    byte[] serialID = roomCardInfo.SerialID.TransToBytes().ToArray();
                    Array.Resize(ref serialID, 4);
                    string LockNumber = BitConverter.ToInt32(roomCardInfo.OpenCode.TransToBytes().ToArray(), 0).ToString() + ":" + BitConverter.ToInt32(serialID, 0).ToString();
                    cInfo.LockNumber = LockNumber;
                    cInfo.StartDate = roomCardInfo.Start.ToString();
                    cInfo.EndDate = roomCardInfo.End.ToString();

                }
                else
                {
                    if (showStatusMessage)
                    {
                        ShowStatusMessage(val);
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

                RoomCardInfo roomCardInfo = new RoomCardInfo();
                List<byte> bys = new List<byte>();
                bool tolog = true;
                int Opencodeval = 0;
                int serialval = 0;

                SerialPortReader se = new SerialPortReader();
                se.SerialPortval();

                string[] lockj = lockNumber.Split(':');

                try
                {
                    Opencodeval = Convert.ToInt32(lockj[0]);
                    serialval = Convert.ToInt32(lockj[1]);
                }
                catch
                {
                    XtraMessageBox.Show("Exception has occured in issuing guest card. Detail:: The Lock Number has some Error !!!", "Door Lock", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                roomCardInfo = new RoomCardInfo
                {
                    Start = startDate.Date,
                    End = endDate,
                    Index = new CardIndex(17716),
                    OpenCode = new OpenRoomCode(Opencodeval),
                    SerialID = new CardSerialID(serialval),
                    Auxiliary = axdata,
                    CardType = 1,
                    ElevatorData = new ElevatorInfo(),
                    IsVerification = true,
                    Period = new DatePeriod(),
                    VipFunc = new VipFeatures()
                };
                ClearCard();
                int val = se.WriteRoomCardInfo(roomCardInfo);

                // roomCardInfo.OpenCode.TransToData(roomCardInfo.OpenCode.TransToBytes());


                if (val != 1)
                {
                    ShowStatusMessage(val);
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
            RoomCardInfo roomCardInfo = new RoomCardInfo();
            roomCardInfo = new RoomCardInfo
            {
                Index = new CardIndex(-999),
                IsVerification = false
            };

            int val = se.WriteRoomCardInfo(roomCardInfo);
            return true;
        }

        public string GetStringFormatOfDate()
        {
            return "yyyyMMddhhmm";
        }




        public int _port { get; set; }
    }


    internal class SerialPortReader : ReaderBase
    {
        protected double WaitSecon
        {
            get;
            set;
        }

        protected static List<byte> CurrentResultBys
        {
            get;
            set;
        }

        private static ReaderBase _readerBase
        {
            get;
            set;
        }

        private static SerialPort SerialPort
        {
            get;
            set;
        }

        public void SerialPortval()
        {
            if (SerialPort == null)
            {
                SerialPort = new SerialPort();
                // SerialPort.PortName = "COM6";
                SerialPort.BaudRate = 9600;
                SerialPort.StopBits = StopBits.One;
                SerialPort.Parity = Parity.None;
                SerialPort.ReceivedBytesThreshold = 1;
                SerialPort.DataReceived += SerialPort_DataReceived;
                SerialPort.Open();
            }
            CurrentResultBys = new List<byte>();
        }

        public static string GetReaderReturnResualt(int re)
        {
            string empty = string.Empty;
            switch (re)
            {
                case -6:
                    return "Card is Canceled";
                case -5:
                    return "No Card Information";
                case -4:
                    return "No Response";
                case -3:
                    return "Not Open";
                case -2:
                    return "Encryption Error";
                case -1:
                    return "Writeing Error";
                case 0:
                    return "No Card";
                case 1:
                    return "";
                default:
                    return "Unknown Error";
            }
        }

        public static ReaderBase CreateReader()
        {
            if (_readerBase == null)
            {
                SerialPortReader serialPortReader = (SerialPortReader)(_readerBase = new SerialPortReader());
            }
            return _readerBase;
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            double num = 0.0;
            double num2 = WaitSecon > 0.0 ? WaitSecon : 5.0;
            lock (CurrentResultBys)
            {
                while (true)
                {
                    try
                    {
                        if (num >= num2 * 1000.0)
                        {
                            return;
                        }
                        byte[] array = new byte[SerialPort.BytesToRead];
                        SerialPort.Read(array, 0, array.Length);
                        CurrentResultBys.AddRange(array.ToArray());
                        if (CurrentResultBys.CheckBytes())
                        {
                            return;
                        }
                        num += 10.0;
                        Thread.Sleep(10);
                    }
                    catch
                    {
                        return;
                    }
                }
            }
        }

        public override bool TryConnect()
        {
            return SerialPort.IsOpen;
        }

        protected virtual int WaitForDo(double waitSecon = 5.0, bool isToLog = true)
        {
            WaitSecon = waitSecon;
            double num = waitSecon * 100.0;
            while (num > 0.0)
            {
                if (CurrentResultBys.CheckBytes())
                {
                    if (isToLog)
                    {
                        WriteLog(CurrentResultBys, "收到的数据", base.LogType);
                    }
                    if (CurrentResultBys[0] == Commands.PDAReturn_FirstByte)
                    {
                        CurrentResultBys.RemoveAt(0);
                    }
                    return 1;
                }
                num -= 1.0;
                Thread.Sleep(20);
            }
            if (isToLog)
            {
                WriteLog(CurrentResultBys, "收到的数据", base.LogType);
            }
            return -4;
        }

        private void WriteByts(List<byte> lst, string logFlag = "", int logType = 0, bool toLog = true)
        {
            if (toLog)
            {
                base.WriteLog(lst, logFlag, logType);
            }
            SerialPort.Write(lst.ToArray(), 0, lst.Count);
        }

        public int MacInitialization()
        {


            //  SerialPort = new System.IO.Ports.SerialPort();
            CurrentResultBys = new List<byte>();
            MachineInfo mi = new MachineInfo();
            string[] portNames = SerialPort.GetPortNames();
            foreach (string text in portNames)
            {
                try
                {
                    if (SerialPort.IsOpen)
                    {
                        SerialPort.Close();
                    }
                    SerialPort.PortName = text;
                    SerialPort.Open();
                    ReadMachineInfo(ref mi);
                    if (mi != null)
                    {
                        PortSetting.SetConfiguration("PortName", text);
                        return 1;
                    }
                    SerialPort.Close();
                }
                catch (Exception)
                {
                }
            }
            return 0;
        }

        public override int ReadMachineInfo(ref MachineInfo mi)
        {
            mi = null;
            if (!SerialPort.IsOpen)
            {
                return -3;
            }
            lock (SerialPort)
            {
                base.CurrentCommand = Commands.COMM_CheckCardReader[1];
                SerialPort.DiscardOutBuffer();
                CurrentResultBys.Clear();
                WriteByts(Commands.COMM_CheckCardReader, "读取机器版本信息", 1);
                int num = WaitForDo(0.5);
                if (num == 1 && (CurrentResultBys.Count == 20 || CurrentResultBys.Count == 21))
                {
                    byte b = CurrentResultBys[1];
                    if (CurrentResultBys.Count == 21)
                    {
                        b = CurrentResultBys[2];
                    }
                    string empty = string.Empty;
                    if (b == Commands.COMM_CheckCardReader[1])
                    {
                        mi = new MachineInfo();
                        mi.TransToData(CurrentResultBys);
                        mi.PortName = SerialPort.PortName;
                        return 1;
                    }
                }
                return 0;
            }
        }

        public override int ReadCardID(ref string cardId)
        {
            if (!SerialPort.IsOpen)
            {
                return -3;
            }
            try
            {
                lock (SerialPort)
                {
                    SerialPort.DiscardOutBuffer();
                    CurrentResultBys.Clear();
                    WriteByts(Commands.COMM_ReadCardID, "读取卡ID", 1);
                    int num = WaitForDo(0.2);
                    if (num != 1 || !CurrentResultBys.CheckBytes())
                    {
                        return num;
                    }
                    byte b = CurrentResultBys[1];
                    string empty = string.Empty;
                    if (b == ReturnIdentification.EncryptionWrong)
                    {
                        return -2;
                    }
                    if (b == ReturnIdentification.Wrong)
                    {
                        return -1;
                    }
                    if (b == ReturnIdentification.NoCard)
                    {
                        return 0;
                    }
                    if (b == ReturnIdentification.ReadCardIDSuccess)
                    {
                        cardId = BitConverter.ToString(new List<byte>
                    {
                        CurrentResultBys[7],
                        CurrentResultBys[8],
                        CurrentResultBys[9],
                        CurrentResultBys[10]
                    }.ToArray()).Replace("-", "");
                        return 1;
                    }
                }
            }
            catch
            {
            }
            return 0;
        }

        public override int ReadRoomCardInfo(ref RoomCardInfo rci, ref List<byte> bys, bool toLog = true)
        {
            if (!SerialPort.IsOpen)
            {
                return -3;
            }
            try
            {
                lock (SerialPort)
                {
                    SerialPort.DiscardOutBuffer();
                    CurrentResultBys.Clear();
                    WriteByts(Commands.COMM_ReadCardInfo, "读客卡信息", 1, toLog);
                    int num = WaitForDo(0.5, toLog);
                    if (num != 1 || !CurrentResultBys.CheckBytes())
                    {
                        return num;
                    }
                    bys = CurrentResultBys.ToList();
                    byte b = CurrentResultBys[1];
                    string empty = string.Empty;
                    if (b == ReturnIdentification.EncryptionWrong)
                    {
                        return -2;
                    }
                    if (b == ReturnIdentification.Wrong)
                    {
                        return -1;
                    }
                    if (b == ReturnIdentification.NoCard)
                    {
                        return 0;
                    }
                    if (b == ReturnIdentification.ReadSuccess)
                    {
                        rci.TransToData(CurrentResultBys);
                        return 1;
                    }
                }
            }
            catch (Exception)
            {
            }
            return 0;
        }

        public override int WriteRoomCardInfo(RoomCardInfo cardInfo)
        {
            if (!SerialPort.IsOpen)
            {
                return -3;
            }
            lock (SerialPort)
            {
                SerialPort.DiscardOutBuffer();
                CurrentResultBys.Clear();
                List<byte> list = cardInfo.TransToBytes();
                List<byte> list2 = new List<byte>();
                list2.AddRange(Commands.COMM_WriteRoomCard.ToArray());
                list2.AddRange(list.ToArray());
                list2.Add(list2.GetCHKByte());
                WriteByts(list2, "写客卡信息", 1);
                int num = WaitForDo(2.0);
                if (num == 1 && CurrentResultBys.CheckBytes())
                {
                    byte b = CurrentResultBys[1];
                    string empty = string.Empty;
                    if (b == ReturnIdentification.EncryptionWrong)
                    {
                        return -2;
                    }
                    if (b == ReturnIdentification.Wrong)
                    {
                        return -1;
                    }
                    if (b == ReturnIdentification.NoCard)
                    {
                        return 0;
                    }
                    if (b == ReturnIdentification.WriteSuccess)
                    {
                        return 1;
                    }
                }
            }
            return 0;
        }

        public override int WriteLimitCard(int readerType)
        {
            if (!SerialPort.IsOpen)
            {
                return -3;
            }
            lock (SerialPort)
            {
                SerialPort.DiscardOutBuffer();
                CurrentResultBys.Clear();
                List<byte> list = new List<byte>();
                if (readerType == 0)
                {
                    list.AddRange(Commands.COMM_WriteLimitCard_ForMg57);
                }
                else
                {
                    list.AddRange(Commands.COMM_WriteLimitCard);
                }
                list.Add(list.GetCHKByte());
                WriteByts(list, "制限制卡", 1);
                int num = WaitForDo(2.0);
                if (num == 1 && CurrentResultBys.CheckBytes())
                {
                    byte b = CurrentResultBys[1];
                    string empty = string.Empty;
                    if (b == ReturnIdentification.EncryptionWrong)
                    {
                        return -2;
                    }
                    if (b == ReturnIdentification.Wrong)
                    {
                        return -1;
                    }
                    if (b == ReturnIdentification.NoCard)
                    {
                        return 0;
                    }
                    if (b == ReturnIdentification.WriteSuccess || b == 111)
                    {
                        return 1;
                    }
                }
            }
            return 0;
        }

        public override int WriteEmergencyCard(int readerType)
        {
            if (!SerialPort.IsOpen)
            {
                return -3;
            }
            lock (SerialPort)
            {
                SerialPort.DiscardOutBuffer();
                CurrentResultBys.Clear();
                List<byte> list = new List<byte>();
                if (readerType == 0)
                {
                    list.AddRange(Commands.COMM_WriteEmergencyCard_ForMg57);
                }
                else
                {
                    list.AddRange(Commands.COMM_WriteEmergencyCard);
                }
                list.Add(list.GetCHKByte());
                WriteByts(list, "应急管理卡", 1);
                int num = WaitForDo(2.0);
                if (num == 1 && CurrentResultBys.CheckBytes())
                {
                    byte b = CurrentResultBys[1];
                    string empty = string.Empty;
                    if (b == ReturnIdentification.EncryptionWrong)
                    {
                        return -2;
                    }
                    if (b == ReturnIdentification.Wrong)
                    {
                        return -1;
                    }
                    if (b == ReturnIdentification.NoCard)
                    {
                        return 0;
                    }
                    if (b == ReturnIdentification.WriteSuccess || b == 111)
                    {
                        return 1;
                    }
                }
            }
            return 0;
        }

    }


}
