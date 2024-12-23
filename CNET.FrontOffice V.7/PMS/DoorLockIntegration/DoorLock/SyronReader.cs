 
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace CNET.FrontOffice_V._7.PMS.DoorLockIntegration.DoorLock
{

    namespace SyronReader
    {
        public static class ListToSkipTake
        {
            // Token: 0x0600012F RID: 303 RVA: 0x00006F50 File Offset: 0x00005150
            public static List<List<T>> SkipTake<T>(this List<T> lst, int takeCount)
            {
                int num = lst.Count<T>() / takeCount;
                List<List<T>> list = new List<List<T>>();
                int i;
                for (i = 0; i < num; i++)
                {
                    List<T> item = lst.Skip(i * takeCount).Take(takeCount).ToList<T>();
                    list.Add(item);
                }
                int num2 = lst.Count<T>() % takeCount;
                if (num2 > 0)
                {
                    list.Add(lst.Skip(i * takeCount).Take(num2).ToList<T>());
                }
                return list;
            }
        }

        public class RoomNoCBD : BaseInfo
        {
            // Token: 0x060000C3 RID: 195 RVA: 0x00004AD5 File Offset: 0x00002CD5
            public RoomNoCBD(string roomNo)
            {
                this.RoomNo = roomNo;
            }

            // Token: 0x17000051 RID: 81
            // (get) Token: 0x060000C4 RID: 196 RVA: 0x00004AE4 File Offset: 0x00002CE4
            // (set) Token: 0x060000C5 RID: 197 RVA: 0x00004AEC File Offset: 0x00002CEC
            public string RoomNo { get; set; }

            // Token: 0x060000C6 RID: 198 RVA: 0x00004AF8 File Offset: 0x00002CF8
            public override List<byte> TransToBytes()
            {
                base.Bytes = new List<byte>();
                List<char> list = this.RoomNo.ToCharArray(0, this.RoomNo.Length).ToList<char>();
                if (list.Count < 6)
                {
                    int num = 6 - list.Count<char>();
                    for (int i = 0; i < num; i++)
                    {
                        list.Insert(0, '0');
                    }
                }
                else
                {
                    list = list.Skip(list.Count - 6).Take(6).ToList<char>();
                }
                List<List<char>> list2 = list.SkipTake(2);
                list2.Reverse();
                for (int j = 0; j < 3; j++)
                {
                    base.Bytes.Add(new string(list2[j].ToArray()).ToByte_16());
                }
                return base.Bytes;
            }

            // Token: 0x060000C7 RID: 199 RVA: 0x00004BB8 File Offset: 0x00002DB8
            public override void TransToData(List<byte> bys)
            {
                if (bys.Count != 3)
                {
                    base.Info = string.Empty;
                }
                string text = string.Empty;
                string text2 = BitConverter.ToString(bys.ToArray(), 0);
                text2 = text2.TrimEnd(new char[]
                {
                '0'
                });
                string[] source = text2.Split(new char[]
                {
                '-'
                }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string str in source.Reverse<string>())
                {
                    text += str;
                }
                base.Info = text;
            }
        }
        public static class TransformatHelper
        {
            public static List<byte> ToBytes(this DateTime dateTime)
            {
                List<byte> list = new List<byte>();
                _ = dateTime.Year;
                string[] array = dateTime.ToString("yy:MM:dd:HH:mm").Split(new char[1] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                if (array.Length != 5)
                {
                    return new List<byte>();
                }

                list.Add(Convert.ToByte(Convert.ToInt32(array[0])));
                list.Add(Convert.ToByte(Convert.ToInt32(array[1])));
                list.Add(Convert.ToByte(Convert.ToInt32(array[2])));
                list.Add(Convert.ToByte(Convert.ToInt32(array[3])));
                list.Add(Convert.ToByte(Convert.ToInt32(array[4])));
                return list;
            }

            public static List<byte> ToBytes2(this DateTime dateTime)
            {
                List<byte> list = new List<byte>();
                _ = dateTime.Year;
                string[] array = dateTime.ToString("yy:MM:dd:HH:mm:ss").Split(new char[1] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                if (array.Length != 6)
                {
                    return new List<byte>();
                }

                list.Add(Convert.ToByte(Convert.ToInt32(array[0])));
                list.Add(Convert.ToByte(Convert.ToInt32(array[1])));
                list.Add(Convert.ToByte(Convert.ToInt32(array[2])));
                list.Add(Convert.ToByte(Convert.ToInt32(array[3])));
                list.Add(Convert.ToByte(Convert.ToInt32(array[4])));
                list.Add(Convert.ToByte(Convert.ToInt32(array[5])));
                return list;
            }

            public static DateTime ToDateTime(this List<byte> bys)
            {
                DateTime dateTime = new DateTime(2000, 1, 1);
                try
                {
                    if (bys.Count != 5)
                    {
                        return DateTime.MinValue;
                    }

                    int value = Convert.ToInt16(bys[0]);
                    int num = Convert.ToInt16(bys[1]);
                    int num2 = Convert.ToInt16(bys[2]);
                    int num3 = Convert.ToInt16(bys[3]);
                    int num4 = Convert.ToInt16(bys[4]);
                    return dateTime.AddYears(value).AddMonths(num - 1).AddDays(num2 - 1)
                        .AddHours(num3)
                        .AddMinutes(num4);
                }
                catch
                {
                    return DateTime.MinValue;
                }
            }

            public static byte ToByte_2(this string binaryStr)
            {
                int value = Convert.ToInt32(binaryStr, 2);
                return Convert.ToByte(value);
            }

            public static byte ToByte_16(this string OxStr)
            {
                return Convert.ToByte(OxStr, 16);
            }

            public static string ToBinary(this byte bt)
            {
                return Convert.ToString(bt, 2);
            }

            public static string To8Binary(this byte bt)
            {
                string text = bt.ToBinary();
                int length = text.Length;
                if (length < 8)
                {
                    for (int i = 0; i < 8 - length; i++)
                    {
                        text = "0" + text;
                    }
                }

                return text;
            }

            public static byte GetCHKByte(this List<byte> bys)
            {
                if (bys.Count() < 2)
                {
                    return 0;
                }

                int num = 0;
                for (int i = 0; i < bys.Count(); i++)
                {
                    num = (num + Convert.ToInt32(bys[i])) % 256;
                }

                num = (256 - num) % 256;
                return Convert.ToByte(num);
            }

            public static bool CheckBytes(this List<byte> bys)
            {
                if (bys.Count() < 2)
                {
                    return false;
                }

                List<byte> list = new List<byte>();
                list.AddRange(bys.ToArray());
                byte b = list.Last();
                byte b2 = list.First();
                if (b2 == Commands.PDAReturn_FirstByte)
                {
                    list.Remove(b2);
                }

                list.Remove(b);
                return list.GetCHKByte() == b;
            }
        }
        public static class ReturnIdentification
        {
            public static byte ReadFromPDA => 85;

            public static byte NoCard => 80;

            public static byte EncryptionWrong => 81;

            public static byte ReadCardIDSuccess => 82;

            public static byte ReadSuccess => 99;

            public static byte WriteSuccess => 94;

            public static byte Wrong => 95;
        }

        public static class T_Valuse
        {
            // Token: 0x040000A4 RID: 164
            public const string Mg_Readtype = "MGxx-1";

            // Token: 0x040000A5 RID: 165
            public const string RF57_Readrtye = "RF57-3";

            // Token: 0x040000A6 RID: 166
            public const string V_KeyReadertype = "M157-1";

            // Token: 0x040000A7 RID: 167
            public static string V_Readertype = "";

            // Token: 0x040000A8 RID: 168
            public static int V_cardtypeval = 0;

            // Token: 0x040000A9 RID: 169
            public static int V_checkCard = 0;

            // Token: 0x040000AA RID: 170
            public static string V_Sector = "";

            // Token: 0x040000AB RID: 171
            public static string V_Key = "";

            // Token: 0x040000AC RID: 172
            public static string V_Lange = "";
        }

        public class RoomCardInfo : BaseInfo
        {
            public CardSerialID SerialID { get; set; }

            public OpenRoomCode OpenCode { get; set; }

            public DateTime Start { get; set; }

            public DateTime End { get; set; }

            public DatePeriod Period { get; set; }

            public AuxiliaryFunc Auxiliary { get; set; }

            public VipFeatures VipFunc { get; set; }

            public CardIndex Index { get; set; }

            public string CardID { get; set; }

            public ElevatorInfo ElevatorData { get; set; }

            public int CardType { get; set; }

            public bool IsVerification { get; set; }

            public RoomCardInfo()
            {
                SerialID = new CardSerialID(0);
                OpenCode = new OpenRoomCode();
                Period = new DatePeriod();
                Index = new CardIndex();
                ElevatorData = new ElevatorInfo();
                Auxiliary = new AuxiliaryFunc();
                VipFunc = new VipFeatures();
                Start = DateTime.MinValue;
                End = DateTime.MinValue;
                IsVerification = true;
            }

            public override List<byte> TransToBytes()
            {
                List<byte> list = new List<byte>();
                list.AddRange(SerialID.TransToBytes());
                list.AddRange(OpenCode.TransToBytes());
                list.AddRange(Start.ToBytes().ToArray());
                list.AddRange(End.ToBytes().ToArray());
                list.AddRange(Period.TransToBytes().ToArray());
                list.Add(Auxiliary.TransToBytes().First());
                list.Add(VipFunc.TransToBytes().First());
                list.AddRange(Index.TransToBytes().ToArray());
                byte[] collection = new byte[3];
                list.AddRange(collection);
                if (IsVerification)
                {
                    list.Add(170);
                }
                else
                {
                    list.Add(0);
                }

                list.AddRange(ElevatorData.TransToBytes());
                return list;
            }

            private int JudgeCardType(List<byte> bys)
            {
                if (bys.Count > 12 && bys[3] == byte.MaxValue && bys[4] == 15 && bys[9] == 90)
                {
                    return -101;
                }

                if (bys.Count > 12 && bys[3] == byte.MaxValue && bys[4] == 15 && bys[9] == 0)
                {
                    return -100;
                }

                return 1;
            }

            public override void TransToData(List<byte> bys)
            {
                if (bys.Count() >= 30)
                {
                    base.Bytes = bys;
                    CardType = JudgeCardType(bys);
                    T_Valuse.V_cardtypeval = JudgeCardType(bys);
                    SerialID.TransToData(new List<byte>
                {
                    bys[3],
                    bys[4]
                });
                    OpenCode.TransToData(new List<byte>
                {
                    bys[5],
                    bys[6],
                    bys[7],
                    bys[8]
                });
                    Start = new List<byte>
                {
                    bys[9],
                    bys[10],
                    bys[11],
                    bys[12],
                    bys[13]
                }.ToDateTime();
                    End = new List<byte>
                {
                    bys[14],
                    bys[15],
                    bys[16],
                    bys[17],
                    bys[18]
                }.ToDateTime();
                    Period.TransToData(new List<byte> { bys[19] });
                    Auxiliary.TransToData(new List<byte> { bys[20] });
                    VipFunc.TransToData(new List<byte> { bys[21] });
                    Index.TransToData(new List<byte>
                {
                    bys[22],
                    bys[23],
                    bys[24]
                });
                    if (T_Valuse.V_Readertype != "MGxx-1" && T_Valuse.V_Readertype != "RF57-3")
                    {
                        CardID = BitConverter.ToString(new List<byte>
                    {
                        bys[30],
                        bys[31],
                        bys[32],
                        bys[33]
                    }.ToArray()).Replace("-", "");
                        ElevatorData.TransToData(new List<byte>
                    {
                        bys[35],
                        bys[36],
                        bys[37],
                        bys[38],
                        bys[39],
                        bys[40],
                        bys[41],
                        bys[42]
                    });
                    }
                }
            }
        }


        public class PortSetting
        {
            private static int is_ToStartLog = -1;

            private static string SettingFilePath = "Setting.config";

            public static bool Is_ToStartLog
            {
                get
                {
                    if (is_ToStartLog < 0)
                    {
                        string configuration = GetConfiguration("Is_ToStartLog");
                        if (string.IsNullOrEmpty(configuration))
                        {
                            SetConfiguration("Is_ToStartLog", "0");
                            return false;
                        }

                        is_ToStartLog = Convert.ToInt32(GetConfiguration("Is_ToStartLog"));
                    }

                    return is_ToStartLog == 1;
                }
            }

            public static string GetConfiguration(string key, string defaultValue = "")
            {
                try
                {
                    Configuration configuration = null;
                    AppSettingsSection appSettingsSection = null;
                    configuration = ConfigurationManager.OpenExeConfiguration(SettingFilePath);
                    appSettingsSection = configuration.AppSettings;
                    string text = null;
                    if (appSettingsSection.Settings.AllKeys.Contains(key))
                    {
                        text = appSettingsSection.Settings[key].Value;
                    }

                    if (string.IsNullOrEmpty(text))
                    {
                        SetConfiguration(key, defaultValue);
                        text = defaultValue;
                    }

                    return text;
                }
                catch
                {
                }

                return null;
            }

            public static void SetConfiguration(string key, string value)
            {
                try
                {
                    Configuration configuration = null;
                    AppSettingsSection appSettingsSection = null;
                    if (!File.Exists(SettingFilePath))
                    {
                        File.Create(SettingFilePath);
                    }

                    configuration = ConfigurationManager.OpenExeConfiguration(SettingFilePath);
                    appSettingsSection = configuration.AppSettings;
                    if (appSettingsSection.Settings.AllKeys.Contains(key))
                    {
                        appSettingsSection.Settings[key].Value = value;
                    }
                    else
                    {
                        appSettingsSection.Settings.Add(key, value);
                    }

                    configuration.Save();
                }
                catch (Exception)
                {
                }
            }
        }

        public class CTxtReadtor
        {
            public string Read(string fileName)
            {
                if (!File.Exists(fileName))
                {
                    FileStream fileStream = File.Create(fileName);
                    fileStream.Close();
                }

                StreamReader streamReader = new StreamReader(fileName, detectEncodingFromByteOrderMarks: false);
                string result = streamReader.ReadToEnd();
                streamReader.Close();
                return result;
            }

            public void AppendText(string filename, string toStr)
            {
                if (!File.Exists(filename))
                {
                    FileStream fileStream = File.Create(filename);
                    fileStream.Close();
                }

                StreamWriter streamWriter = File.AppendText(filename);
                streamWriter.Write(toStr + "\r\n");
                streamWriter.Close();
            }

            public void AppendText(string filename, byte[] btys)
            {
                string toStr = BitConverter.ToString(btys).Replace("-", "");
                AppendText(filename, toStr);
            }
        }
        public class ElevatorAccessControl : BaseInfo
        {
            public DateTime Interval1_From { get; set; }

            public DateTime Interval1_To { get; set; }

            public DateTime Interval2_From { get; set; }

            public DateTime Interval2_To { get; set; }

            public bool EnableInterval1 { get; set; }

            public bool EnableInterval2 { get; set; }

            public int Rad1 { get; set; }

            public int Rad2 { get; set; }

            public ElevatorAccessControl()
            {
                base.Bytes = new List<byte>();
            }

            public override List<byte> TransToBytes()
            {
                base.Bytes = new List<byte>();
                if (EnableInterval1)
                {
                    base.Bytes.Add(Convert.ToByte(Interval1_From.Hour));
                    base.Bytes.Add(Convert.ToByte(Interval1_From.Minute));
                    base.Bytes.Add(Convert.ToByte(Interval1_To.Hour));
                    base.Bytes.Add(Convert.ToByte(Interval1_To.Minute));
                }
                else
                {
                    base.Bytes.AddRange(new byte[4] { 255, 255, 255, 255 });
                }

                base.Bytes.Add(208);
                if (Rad2 == 1 && Rad1 == 1)
                {
                    base.Bytes.Add(0);
                }
                else if (Rad2 == 1 && Rad1 == 0)
                {
                    base.Bytes.Add(160);
                }
                else if (Rad2 == 0 && Rad1 == 1)
                {
                    base.Bytes.Add(11);
                }
                else if (Rad2 == 0 && Rad1 == 0)
                {
                    base.Bytes.Add(171);
                }

                if (EnableInterval2)
                {
                    base.Bytes.Add(Convert.ToByte(Interval2_From.Hour));
                    base.Bytes.Add(Convert.ToByte(Interval2_From.Minute));
                    base.Bytes.Add(Convert.ToByte(Interval2_To.Hour));
                    base.Bytes.Add(Convert.ToByte(Interval2_To.Minute));
                }
                else
                {
                    base.Bytes.AddRange(new byte[4] { 255, 255, 255, 255 });
                }

                return base.Bytes;
            }

            public override void TransToData(List<byte> bys)
            {
                if (bys.Count != 0)
                {
                    base.Bytes = bys;
                    Interval1_From = DateTime.Now.Date;
                    if (bys[2] != byte.MaxValue)
                    {
                        EnableInterval1 = true;
                        Interval1_From = Interval1_From.AddHours(Convert.ToInt32(bys[2]));
                        Interval1_From = Interval1_From.AddMinutes(Convert.ToInt32(bys[3]));
                        Interval1_To = Interval1_To.AddHours(Convert.ToInt32(bys[4]));
                        Interval1_To = Interval1_To.AddMinutes(Convert.ToInt32(bys[5]));
                    }
                    else
                    {
                        EnableInterval1 = false;
                    }

                    if (bys[7] == 171)
                    {
                        Rad1 = 0;
                        Rad2 = 0;
                    }
                    else if (bys[7] == 160)
                    {
                        Rad1 = 0;
                        Rad2 = 1;
                    }
                    else if (bys[7] == 11)
                    {
                        Rad1 = 1;
                        Rad2 = 0;
                    }
                    else if (bys[7] == 0)
                    {
                        Rad1 = 1;
                        Rad2 = 1;
                    }

                    if (bys[8] != byte.MaxValue)
                    {
                        EnableInterval2 = true;
                        Interval2_From = Interval2_From.AddHours(Convert.ToInt32(bys[8]));
                        Interval2_From = Interval2_From.AddMinutes(Convert.ToInt32(bys[9]));
                        Interval2_To = Interval2_To.AddHours(Convert.ToInt32(bys[10]));
                        Interval2_To = Interval2_To.AddMinutes(Convert.ToInt32(bys[11]));
                    }
                    else
                    {
                        EnableInterval2 = false;
                    }

                    base.Info = BitConverter.ToString(bys.ToArray(), 0);
                }
            }
        }



        public class PDAMachineInfo : BaseInfo
        {
            public string DateFormat { get; set; }

            public bool IsShowLogo { get; set; }

            public int Lan { get; set; }

            public override List<byte> TransToBytes()
            {
                return base.TransToBytes();
            }

            public override void TransToData(List<byte> bys)
            {
                if (bys.Count == 20)
                {
                    char[] source = bys[14].To8Binary().ToCharArray();
                    source = source.Reverse().ToArray();
                    if (source[0].ToString() + source[1] == "00")
                    {
                        DateFormat = "YYYY-MM-DD";
                    }
                    else if (source[0].ToString() + source[1] == "01")
                    {
                        DateFormat = "MM-DD-YYYY";
                    }
                    else if (source[0].ToString() + source[1] == "10")
                    {
                        DateFormat = "DD-MM-YYYY";
                    }

                    IsShowLogo = source[2].ToString() == "1";
                    if (source[6].ToString() + source[7] == "01")
                    {
                        Lan = 0;
                    }
                    else
                    {
                        Lan = 1;
                    }

                    base.Info = bys[14].To8Binary();
                }
            }
        }

        public class T_TimePeriod
        {
            // Token: 0x06000139 RID: 313 RVA: 0x00003AEC File Offset: 0x00001CEC
            public T_TimePeriod()
            {
                this.TimeStart = new DateTime(2000, 1, 1);
                this.TimeEnd = new DateTime(2000, 1, 1);
            }

            
            public string PeriodName { get; set; }
 
            public DateTime TimeStart { get; set; }

             
            public DateTime TimeEnd { get; set; }
        }

        public class PDACmmData : BaseInfo
        {
            public int AlarmDelay { get; set; }

            public List<T_TimePeriod> LstTimePeriod { get; set; }

            public int SectorNumber { get; set; }

            public int WorkingWay { get; set; }

            public PDACmmData()
            {
            }

            public PDACmmData(T_SysConfig2 config, List<T_TimePeriod> lstTimePeriod, int sectorNumber)
            {
                AlarmDelay = config.AlarmTime;
                LstTimePeriod = lstTimePeriod;
                SectorNumber = sectorNumber;
                if (config.MotorWorkMode == 0)
                {
                    if (config.WorkMode1== 0)
                    {
                        WorkingWay = config.WorkMode1Time  / 20;
                    }
                    else
                    {
                        WorkingWay = 192 + config.WorkMode1Time;
                    }
                }
            }

            public override List<byte> TransToBytes()
            {
                List<byte> list = new List<byte>();
                list.Add(Convert.ToByte(AlarmDelay));
                foreach (T_TimePeriod item in LstTimePeriod)
                {
                    list.AddRange(CreatePeriodBys(item).ToArray());
                }

                list.Add(Convert.ToByte(SectorNumber));
                list.Add(Convert.ToByte(WorkingWay));
                byte[] collection = new byte[7];
                list.AddRange(collection);
                base.Bytes = list;
                return base.Bytes;
            }

            private List<byte> CreatePeriodBys(T_TimePeriod tp)
            {
                List<byte> list = new List<byte>();
                string text = Convert.ToByte(tp.TimeStart.Hour).To8Binary();
                string text2 = Convert.ToByte(tp.TimeStart.Minute).To8Binary();
                string text3 = Convert.ToByte(tp.TimeEnd.Hour).To8Binary();
                string text4 = Convert.ToByte(tp.TimeEnd.Minute).To8Binary();
                string binaryStr = text2.Substring(2, 3) + text.Substring(3, 5);
                list.Add(binaryStr.ToByte_2());
                string binaryStr2 = text2.Substring(5, 3) + text3.Substring(3, 5);
                list.Add(binaryStr2.ToByte_2());
                string binaryStr3 = "00" + text4.Substring(2, 6);
                list.Add(binaryStr3.ToByte_2());
                return list;
            }

            public override void TransToData(List<byte> bys)
            {
                base.TransToData(base.Bytes);
            }
        }


        public class T_SysConfig2 
        {
           
            public bool Check1 { get; set; }

            
            public bool Check2 { get; set; }
 
            public bool Check3 { get; set; }

            
            public bool Check4 { get; set; }
             
            public bool Check5 { get; set; }

            
            public bool Check6 { get; set; }

       
            public bool Check7 { get; set; }

           
            public int AlarmTime { get; set; }
             
            public int MotorWorkMode { get; set; }
 
            public int WorkMode1 { get; set; }

         
            public int WorkMode1Time { get; set; }

        
            public string WorkMode2Type { get; set; }
        }

        public class T_SysConfig1  
        {
            // Token: 0x06000102 RID: 258 RVA: 0x0000387C File Offset: 0x00001A7C
            public T_SysConfig1()
            {
                this.LeaveTime = new DateTime(2000, 1, 1);
            }

             
            public string PortID { get; set; }

           
            public DateTime LeaveTime { get; set; }

            
            public int AuthorityWhen { get; set; }

          
            public bool Check1 { get; set; }
 
            public bool Check2 { get; set; }
 
            public bool Check3 { get; set; }
 
            public bool Check4 { get; set; }
 
            public bool Check5 { get; set; }

           
            public bool Check6 { get; set; }

            
            public string Mifare1 { get; set; }

             
            public int MC { get; set; }

           
            public int SC { get; set; }

            
            public string SystemClosingDate { get; set; }

             
            public string CustomerCodeBys { get; set; }
 
        }

        public class ReaderBase
        {
            protected int LogType { get; set; }

            protected byte CurrentCommand { get; set; }

            public ReaderBase()
            {
                CurrentCommand = 0;
            }

            protected virtual void WriteLog(IEnumerable<byte> lst, string logFlag = "", int logType = 0)
            {
                LogType = logType;
                if (PortSetting.Is_ToStartLog)
                {
                    string text = Application.StartupPath + "\\Log";
                    if (!Directory.Exists(text))
                    {
                        Directory.CreateDirectory(text);
                    }

                    string text2 = DateTime.Now.ToString("yyyyMMdd");
                    string filename = text + "\\" + ((logType == 0) ? "pda" : "serialport") + text2 + ".txt";
                    string text3 = string.Empty;
                    if (!string.IsNullOrEmpty(logFlag))
                    {
                        text3 = text3 + "***" + logFlag + "***\r\n";
                    }

                    text3 = ((lst == null || lst.Count() <= 0) ? (text3 + "null") : (text3 + BitConverter.ToString(lst.ToArray()).Replace("-", "")));
                    new CTxtReadtor().AppendText(filename, text3);
                }
            }

            public virtual int MachineInitialization(ref MachineInfo mi, T_SysConfig1 config)
            {
                return 0;
            }

            public virtual int ReadMachineInfo(ref MachineInfo mi)
            {
                return 0;
            }

            public virtual int WriteReaderSector(int sector)
            {
                return -1;
            }

            public virtual int WriteReaderPW(string s_PW)
            {
                return -1;
            }

            public virtual int ReadReaderSector()
            {
                return -1;
            }

            public virtual int SalesCodeSertting(int mc, int sc)
            {
                return -1;
            }

            public virtual int ReadCardID(ref string cardId)
            {
                return 0;
            }

            public virtual int ReadRoomCardInfo(ref RoomCardInfo cardInfo, ref List<byte> bys, bool toLog = true)
            {
                return 0;
            }

            public virtual int WriteRoomCardInfo(RoomCardInfo cardInfo)
            {
                return 0;
            }

            public virtual bool TryConnect()
            {
                return false;
            }

            public virtual int WriteElevatorNumber(List<int> lstElevatorSID)
            {
                return 0;
            }

            public virtual int ReadElevatorNumber(ref List<int> lstElevatorSID, ref string cardID)
            {
                return 0;
            }

            public virtual int WriteElevatorControl(List<int> lstElevatorSID)
            {
                return 0;
            }

            public virtual int ReadElevatorControl(ref List<int> lstElevatorSID)
            {
                return 0;
            }

            public virtual int WriteElevatorAccessControl(ElevatorAccessControl accessObj)
            {
                return 0;
            }

            public virtual int ReadElevatorAccessControl(ref ElevatorAccessControl accessObj)
            {
                return 0;
            }

            public virtual int WriteLimitCard(int readerType)
            {
                return -1;
            }

            public virtual int WriteEmergencyCard(int readerType)
            {
                return -1;
            }

            public virtual void PDA_WakeUp(int flag = 0)
            {
            }

            public virtual int PDA_ReadMachineInfo(ref PDAMachineInfo info)
            {
                return -3;
            }

            public virtual int PDA_DownLoadComData(PDACmmData data)
            {
                return -3;
            }

            public virtual int PDA_DownLoadDateTime(DateTime date)
            {
                return -3;
            }

            public virtual int PDA_AddressSetting(List<byte> bys)
            {
                return -3;
            }

            public virtual int PDA_DownLoadLanguge(List<byte> bysData, int frameIndex)
            {
                return -3;
            }

            public virtual int PDA_DownLoadOpenCode(List<byte> bysData, int frameIndex)
            {
                return -3;
            }

            public virtual int PDA_DownLoadOpenAccess(List<byte> bysData, SysRoomOpenAccess openObject, ref int frameIndex)
            {
                return -3;
            }

            public virtual int PDA_GetLockerState(int index, ref LockerState lockState)
            {
                return -3;
            }

            public virtual int PDA_GetLockerOpenRecord(int lockIndex, ref List<List<byte>> bysReocrds)
            {
                return -3;
            }
        }

        public class SysRoomOpenAccess
        {
            // Token: 0x170000D3 RID: 211
            // (get) Token: 0x060001CD RID: 461 RVA: 0x00005D9C File Offset: 0x00003F9C
            // (set) Token: 0x060001CE RID: 462 RVA: 0x00005DA4 File Offset: 0x00003FA4
            public string RoomNo { get; set; }

            // Token: 0x170000D4 RID: 212
            // (get) Token: 0x060001CF RID: 463 RVA: 0x00005DAD File Offset: 0x00003FAD
            // (set) Token: 0x060001D0 RID: 464 RVA: 0x00005DB5 File Offset: 0x00003FB5
            public string GID { get; set; }

            // Token: 0x170000D5 RID: 213
            // (get) Token: 0x060001D1 RID: 465 RVA: 0x00005DBE File Offset: 0x00003FBE
            // (set) Token: 0x060001D2 RID: 466 RVA: 0x00005DC6 File Offset: 0x00003FC6
            public int RoomType { get; set; }

            // Token: 0x170000D6 RID: 214
            // (get) Token: 0x060001D3 RID: 467 RVA: 0x00005DCF File Offset: 0x00003FCF
            // (set) Token: 0x060001D4 RID: 468 RVA: 0x00005DD7 File Offset: 0x00003FD7
            public int VIPKindID { get; set; }

            // Token: 0x170000D7 RID: 215
            // (get) Token: 0x060001D5 RID: 469 RVA: 0x00005DE0 File Offset: 0x00003FE0
            // (set) Token: 0x060001D6 RID: 470 RVA: 0x00005DE8 File Offset: 0x00003FE8
            public int SerialID { get; set; }

            // Token: 0x170000D8 RID: 216
            // (get) Token: 0x060001D7 RID: 471 RVA: 0x00005DF1 File Offset: 0x00003FF1
            // (set) Token: 0x060001D8 RID: 472 RVA: 0x00005DF9 File Offset: 0x00003FF9
            public int OpenCode { get; set; }

            // Token: 0x170000D9 RID: 217
            // (get) Token: 0x060001D9 RID: 473 RVA: 0x00005E02 File Offset: 0x00004002
            // (set) Token: 0x060001DA RID: 474 RVA: 0x00005E0A File Offset: 0x0000400A
            public List<int> LstAccessSerialID { get; set; }
        }

        public class LockerState
        {
            public int Channel { get; set; }

            public int Voltage { get; set; }

            public int DeadBolt { get; set; }

            public int LatchBolt { get; set; }

            public int NormalPosition { get; set; }

            public double VoltageValue { get; set; }

            public string RoomNO { get; set; }

            public LockerState()
            {
            }

            public LockerState(List<byte> bys)
            {
                if (bys.Count == 20)
                {
                    char[] array = bys[3].To8Binary().ToCharArray();
                    NormalPosition = ((array[0] == '0') ? 1 : 0);
                    LatchBolt = ((array[1] == '0') ? 1 : 0);
                    DeadBolt = ((array[2] == '0') ? 1 : 0);
                    Voltage = ((array[3] == '0') ? 1 : 0);
                    Channel = ((array[4] == '0') ? 1 : 0);
                    VoltageValue = Convert.ToDouble(bys[4]) * 7.0 / 256.0;
                    RoomNoCBD roomNoCBD = new RoomNoCBD(null);
                    roomNoCBD.TransToData(new List<byte>
                {
                    bys[6],
                    bys[7],
                    bys[8]
                });
                    RoomNO = roomNoCBD.Info;
                }
            }
        }

        public static class Commands
        {
            private static byte[] customerCodeBys = new byte[2] { 1, 1 };

            public static byte PDAReturn_FirstByte = 85;

            public static byte COMM_MC { get; set; }

            public static byte COMM_SC { get; set; }

            public static byte[] CustomerCodeBys_ForElevator
            {
                get
                {
                    if (customerCodeBys == null || customerCodeBys.Count() == 0)
                    {
                        return null;
                    }

                    List<byte> list = new List<byte>();
                    list.Add(customerCodeBys.First());
                    List<byte> list2 = list;
                    string text = BitConverter.ToString(new byte[1] { customerCodeBys.Last() });
                    list2.Insert(0, Convert.ToByte(text.ToCharArray()[1].ToString(), 16));
                    return list2.ToArray();
                }
            }

            public static byte[] CustomerCodeBys
            {
                get
                {
                    return customerCodeBys;
                }
                set
                {
                    customerCodeBys = value;
                }
            }

            public static List<byte> COMM_CheckCardReader
            {
                get
                {
                    List<byte> list = new List<byte>();
                    list.Add(235);
                    list.Add(86);
                    list.Add(0);
                    list.Add(191);
                    return list;
                }
            }

            public static List<byte> COMM_ReadCardID
            {
                get
                {
                    List<byte> list = new List<byte>();
                    list.Add(235);
                    list.Add(82);
                    list.Add(0);
                    list.Add(195);
                    return list;
                }
            }

            public static List<byte> COMM_ReadCardInfo
            {
                get
                {
                    List<byte> list = new List<byte>();
                    list.Add(235);
                    list.Add(99);
                    list.Add(0);
                    list.Add(178);
                    return list;
                }
            }

            public static List<byte> COMM_WriteRoomCard
            {
                get
                {
                    List<byte> list = new List<byte>();
                    list.Add(235);
                    list.Add(101);
                    list.Add(34);
                    return list;
                }
            }

            public static List<byte> COMM_ElevatorNumber
            {
                get
                {
                    List<byte> list = new List<byte>();
                    list.Add(235);
                    list.Add(131);
                    list.Add(34);
                    return list;
                }
            }

            public static List<byte> COMM_ElevatorNumber_Read
            {
                get
                {
                    List<byte> list = new List<byte>();
                    list.Add(235);
                    list.Add(132);
                    list.Add(0);
                    list.Add(145);
                    return list;
                }
            }

            public static List<byte> COMM_ElevatorControl
            {
                get
                {
                    List<byte> list = new List<byte>();
                    list.Add(235);
                    list.Add(128);
                    list.Add(10);
                    return list;
                }
            }

            public static List<byte> COMM_ElevatorControl_Read
            {
                get
                {
                    List<byte> list = new List<byte>();
                    list.Add(235);
                    list.Add(129);
                    list.Add(0);
                    list.Add(148);
                    return list;
                }
            }

            public static List<byte> COMM_ElevatorAccessControl
            {
                get
                {
                    List<byte> list = new List<byte>();
                    list.Add(235);
                    list.Add(87);
                    list.Add(13);
                    return list;
                }
            }

            public static List<byte> COMM_ReadElevatorAccessControl
            {
                get
                {
                    List<byte> list = new List<byte>();
                    list.Add(235);
                    list.Add(84);
                    list.Add(0);
                    list.Add(193);
                    return list;
                }
            }

            public static List<byte> COMM_WriteLimitCard
            {
                get
                {
                    List<byte> list = new List<byte>();
                    list.Add(235);
                    list.Add(101);
                    list.Add(35);
                    list.Add(byte.MaxValue);
                    list.Add(15);
                    list.Add(COMM_MC);
                    list.Add(COMM_SC);
                    list.Add(CustomerCodeBys[0]);
                    list.Add(CustomerCodeBys[1]);
                    list.Add(90);
                    list.Add(0);
                    list.Add(0);
                    list.Add(0);
                    list.Add(0);
                    list.Add(0);
                    list.Add(0);
                    list.Add(0);
                    list.Add(0);
                    list.Add(0);
                    list.Add(0);
                    list.Add(0);
                    list.Add(0);
                    list.Add(0);
                    list.Add(0);
                    list.Add(0);
                    list.Add(0);
                    list.Add(0);
                    list.Add(0);
                    list.Add(0);
                    list.Add(0);
                    list.Add(0);
                    list.Add(0);
                    list.Add(0);
                    list.Add(0);
                    list.Add(0);
                    list.Add(0);
                    list.Add(0);
                    list.Add(0);
                    return list;
                }
            }

            public static List<byte> COMM_WriteLimitCard_ForMg57
            {
                get
                {
                    List<byte> list = new List<byte>();
                    list.Add(235);
                    list.Add(101);
                    list.Add(34);
                    list.Add(byte.MaxValue);
                    list.Add(15);
                    list.Add(COMM_MC);
                    list.Add(COMM_SC);
                    list.Add(CustomerCodeBys[0]);
                    list.Add(CustomerCodeBys[1]);
                    list.Add(90);
                    list.Add(0);
                    list.Add(0);
                    list.Add(0);
                    list.Add(0);
                    list.Add(0);
                    list.Add(0);
                    list.Add(0);
                    list.Add(0);
                    list.Add(0);
                    list.Add(0);
                    list.Add(0);
                    list.Add(0);
                    list.Add(0);
                    list.Add(0);
                    list.Add(0);
                    list.Add(0);
                    list.Add(0);
                    list.Add(0);
                    list.Add(0);
                    list.Add(0);
                    list.Add(0);
                    list.Add(0);
                    list.Add(0);
                    list.Add(0);
                    list.Add(0);
                    list.Add(0);
                    list.Add(0);
                    return list;
                }
            }

            public static List<byte> COMM_WriteEmergencyCard
            {
                get
                {
                    List<byte> list = new List<byte>();
                    list.Add(235);
                    list.Add(101);
                    list.Add(35);
                    list.Add(byte.MaxValue);
                    list.Add(15);
                    list.Add(COMM_MC);
                    list.Add(COMM_SC);
                    list.Add(CustomerCodeBys[0]);
                    list.Add(CustomerCodeBys[1]);
                    list.Add(0);
                    list.Add(0);
                    list.Add(0);
                    list.Add(0);
                    list.Add(0);
                    list.Add(0);
                    list.Add(0);
                    list.Add(0);
                    list.Add(0);
                    list.Add(0);
                    list.Add(0);
                    list.Add(0);
                    list.Add(0);
                    list.Add(0);
                    list.Add(0);
                    list.Add(0);
                    list.Add(0);
                    list.Add(0);
                    list.Add(0);
                    list.Add(170);
                    list.Add(0);
                    list.Add(0);
                    list.Add(0);
                    list.Add(0);
                    list.Add(0);
                    list.Add(0);
                    list.Add(0);
                    list.Add(0);
                    list.Add(0);
                    return list;
                }
            }

            public static List<byte> COMM_WriteEmergencyCard_ForMg57
            {
                get
                {
                    List<byte> list = new List<byte>();
                    list.Add(235);
                    list.Add(101);
                    list.Add(34);
                    list.Add(byte.MaxValue);
                    list.Add(15);
                    list.Add(COMM_MC);
                    list.Add(COMM_SC);
                    list.Add(CustomerCodeBys[0]);
                    list.Add(CustomerCodeBys[1]);
                    list.Add(0);
                    list.Add(0);
                    list.Add(0);
                    list.Add(0);
                    list.Add(0);
                    list.Add(0);
                    list.Add(0);
                    list.Add(0);
                    list.Add(0);
                    list.Add(0);
                    list.Add(0);
                    list.Add(0);
                    list.Add(0);
                    list.Add(0);
                    list.Add(0);
                    list.Add(0);
                    list.Add(0);
                    list.Add(0);
                    list.Add(0);
                    list.Add(170);
                    list.Add(0);
                    list.Add(0);
                    list.Add(0);
                    list.Add(0);
                    list.Add(0);
                    list.Add(0);
                    list.Add(0);
                    list.Add(0);
                    return list;
                }
            }

            public static List<byte> PDACOMM_WakeUp
            {
                get
                {
                    List<byte> list = new List<byte>();
                    list.Add(144);
                    list.Add(1);
                    list.Add(0);
                    list.Add(111);
                    return list;
                }
            }

            public static List<byte> PDACOMM_WakeUp2
            {
                get
                {
                    List<byte> list = new List<byte>();
                    list.Add(144);
                    list.Add(0);
                    list.Add(0);
                    list.Add(112);
                    return list;
                }
            }

            public static List<byte> PDACOMM_ComDate
            {
                get
                {
                    List<byte> list = new List<byte>();
                    list.Add(144);
                    list.Add(1);
                    list.Add(38);
                    return list;
                }
            }

            public static List<byte> PDA_COMM_DownloadDateTime
            {
                get
                {
                    List<byte> list = new List<byte>();
                    list.Add(144);
                    list.Add(2);
                    list.Add(10);
                    return list;
                }
            }

            public static List<byte> PDA_COMM_ReadMachineInfo
            {
                get
                {
                    List<byte> list = new List<byte>();
                    list.Add(144);
                    list.Add(6);
                    list.Add(0);
                    list.Add(106);
                    return list;
                }
            }

            public static List<byte> PDA_COMM_DownLoadLanInfo
            {
                get
                {
                    List<byte> list = new List<byte>();
                    list.Add(144);
                    list.Add(42);
                    list.Add(131);
                    return list;
                }
            }

            public static List<byte> PDA_COMM_DownLoadLanInfo_USB
            {
                get
                {
                    List<byte> list = new List<byte>();
                    list.Add(144);
                    list.Add(42);
                    list.Add(131);
                    return list;
                }
            }

            public static List<byte> PDA_COMM_AddressSetting
            {
                get
                {
                    List<byte> list = new List<byte>();
                    list.Add(144);
                    list.Add(42);
                    list.Add(4);
                    list.Add(0);
                    list.Add(40);
                    list.Add(1);
                    list.Add(225);
                    list.Add(56);
                    return list;
                }
            }

            public static List<byte> PDA_COMM_OpenCode
            {
                get
                {
                    List<byte> list = new List<byte>();
                    list.Add(144);
                    list.Add(12);
                    list.Add(66);
                    return list;
                }
            }

            public static List<byte> PDA_COMM_OpenCode_USB
            {
                get
                {
                    List<byte> list = new List<byte>();
                    list.Add(144);
                    list.Add(12);
                    list.Add(34);
                    return list;
                }
            }

            public static List<byte> PDA_COMM_OpenAccess
            {
                get
                {
                    List<byte> list = new List<byte>();
                    list.Add(144);
                    list.Add(15);
                    list.Add(139);
                    return list;
                }
            }

            public static List<byte> PDA_COMM_OpenAccess_USB
            {
                get
                {
                    List<byte> list = new List<byte>();
                    list.Add(144);
                    list.Add(15);
                    list.Add(49);
                    return list;
                }
            }

            public static List<byte> PDA_COMM_GetLockerState
            {
                get
                {
                    List<byte> list = new List<byte>();
                    list.Add(144);
                    list.Add(9);
                    list.Add(5);
                    return list;
                }
            }
        }
        public class MachineInfo : BaseInfo
        {
            public string ReaderType { get; set; }

            public string PortName { get; set; }

            public string Version { get; set; }

            public string Agreement { get; set; }

            public int MC { get; set; }

            public int SC { get; set; }

            public int Sector { get; set; }

            public string Key { get; set; }

            public override List<byte> TransToBytes()
            {
                return base.TransToBytes();
            }

            public override void TransToData(List<byte> bys)
            {
                if (bys.Count != 0)
                {
                    if (bys.Count == 21)
                    {
                        bys.RemoveRange(0, 1);
                    }

                    base.Bytes = bys;
                    ReaderType = Encoding.UTF8.GetString(bys.ToArray(), 3, 6);
                    Version = BitConverter.ToString(bys.ToArray(), 8, 2).Replace("-", "");
                    Agreement = BitConverter.ToString(bys.ToArray(), 10, 1).Replace("-", "");
                    MC = Convert.ToInt32(bys[11]);
                    SC = Convert.ToInt32(bys[12]);
                    Sector = Convert.ToInt32(bys[13]);
                    new ASCIIEncoding();
                    Key = Encoding.UTF8.GetString(bys.ToArray(), 15, 1);
                    T_Valuse.V_Key = Key;
                }
            }

            public override string ToString()
            {
                //IL_0291: Unknown result type (might be due to invalid IL or missing references)
                string text = MC.ToString();
                string text2 = SC.ToString();
                if (MC < 10)
                {
                    text = "00" + MC;
                }
                else if (MC < 100)
                {
                    text = "0" + MC;
                }

                if (SC < 10)
                {
                    text2 = "00" + SC;
                }
                else if (SC < 100)
                {
                    text2 = "0" + SC;
                }

                string text3 = ((PortName != null) ? (ReaderType + "(" + PortName + ")   MC/SC(" + text + "/" + text2 + ")    Sector(" + Sector + ")") : ((T_Valuse.V_Readertype == "MGxx-1") ? (ReaderType + "   MC/SC(" + text + "/" + text2 + ")") : ((!(T_Valuse.V_Readertype == "M157-1")) ? (ReaderType + "   MC/SC(" + text + "/" + text2 + ")    Sector(" + Sector + ")") : (ReaderType + "   MC/SC(" + text + "/" + text2 + ")    Sector(" + Sector + ")    Key(" + Key + ")"))));
                string text4 = "";
                if (T_Valuse.V_Lange == "zh-CN")
                {
                    text4 = "   感谢使用本系统,欢迎提供宝贵意见和建议;有任何疑问请留言QQ(86589457)或微信/WeChat(13710007629)";
                }
                else if (T_Valuse.V_Lange != "")
                {
                    text4 = "   Thank you for using this system;Please leave a message;QQ(86589457)/WeChat(13710007629)";
                }

                int num = 1;// new Class1().t_ReadRecsys();
                if (num == 1)
                {
                    text3 += text4;
                }

                return text3;
            }
        }
        public class CardIndex : BaseInfo
        {
            public int IndexValue { get; set; }

            public CardIndex()
            {
                base.Bytes = new List<byte> { 0, 0, 0 };
            }

            public CardIndex(int index)
            {
                IndexValue = index;
                byte[] array = BitConverter.GetBytes(index);
                if (array.Length > 3)
                {
                    array = new byte[3]
                    {
                    array[0],
                    array[1],
                    array[2]
                    };
                }

                base.Bytes = array.ToList();
            }

            public override List<byte> TransToBytes()
            {
                return base.Bytes;
            }

            public override void TransToData(List<byte> bys)
            {
                if (bys.Count != 0)
                {
                    base.Bytes = new List<byte>(bys);
                    string text = "(" + BitConverter.ToString(bys.ToArray()) + ")";
                    bys.Add(0);
                    base.Info = (IndexValue = BitConverter.ToInt32(bys.ToArray(), 0)) + text;
                }
            }
        }
        public class VipFeatures : BaseInfo
        {
            public VipFeatures()
            {
                base.Bytes = new List<byte> { 8 };
            }

            public VipFeatures(List<int> lstSerialID)
            {
                string text = string.Empty;
                lstSerialID = lstSerialID.OrderBy((int x) => x).ToList();
                for (int i = 1; i <= 8; i++)
                {
                    text += (lstSerialID.Contains(i) ? "1" : "0");
                }

                base.Bytes = new List<byte> { new string(text.Reverse().ToArray()).ToByte_2() };
            }

            public override List<byte> TransToBytes()
            {
                return base.Bytes;
            }

            public override void TransToData(List<byte> bys)
            {
                if (bys.Count != 0)
                {
                    base.Bytes = new List<byte>(bys);
                    string text = "(" + BitConverter.ToString(bys.ToArray()) + ")";
                    string source = bys[0].To8Binary();
                    base.Info = new string(source.Reverse().ToArray()) + text;
                }
            }
        }
        public class AuxiliaryFunc : BaseInfo
        {
            public bool SixCall { get; set; }

            public bool Weak { get; set; }

            public bool Sound { get; set; }

            public bool Limit { get; set; }

            public bool Channel { get; set; }

            public bool OpenLock { get; set; }

            public string GetBinaryStr => "00" + (SixCall ? "1" : "0") + (Weak ? "1" : "0") + (Sound ? "1" : "0") + (Limit ? "1" : "0") + (Channel ? "1" : "0") + (OpenLock ? "1" : "0");

            public AuxiliaryFunc()
            {
                base.Bytes = new List<byte> { 8 };
            }

            public AuxiliaryFunc(string binaryStr)
            {
                if (string.IsNullOrEmpty(binaryStr))
                {
                    binaryStr = "00000000";
                }

                TransToData(new List<byte> { binaryStr.ToByte_2() });
            }

            public override List<byte> TransToBytes()
            {
                string getBinaryStr = GetBinaryStr;
                base.Bytes = new List<byte> { getBinaryStr.ToByte_2() };
                return base.Bytes;
            }

            public override void TransToData(List<byte> bys)
            {
                if (bys.Count != 0)
                {
                    base.Bytes = bys;
                    string text = "(" + BitConverter.ToString(bys.ToArray()) + ")";
                    string text2 = bys[0].To8Binary();
                    SixCall = text2.Substring(2, 1) == "1";
                    Weak = text2.Substring(3, 1) == "1";
                    Sound = text2.Substring(4, 1) == "1";
                    Limit = text2.Substring(5, 1) == "1";
                    Channel = text2.Substring(6, 1) == "1";
                    OpenLock = text2.Substring(7, 1) == "1";
                    base.Info = text2 + text;
                }
            }
        }
        public class DatePeriod : BaseInfo
        {
            public DatePeriod()
            {
                base.Bytes = new List<byte> { 0 };
            }

            public DatePeriod(List<int> lstSerialID)
            {
                string text = string.Empty;
                lstSerialID = lstSerialID.OrderBy((int x) => x).ToList();
                for (int i = 1; i <= 8; i++)
                {
                    text += (lstSerialID.Contains(i) ? "1" : "0");
                }

                base.Bytes = new List<byte> { new string(text.Reverse().ToArray()).ToByte_2() };
            }

            public override List<byte> TransToBytes()
            {
                return base.Bytes;
            }

            public override void TransToData(List<byte> bys)
            {
                if (bys.Count != 0)
                {
                    base.Bytes = bys;
                    string text = "(" + BitConverter.ToString(bys.ToArray()) + ")";
                    string source = bys[0].To8Binary();
                    base.Info = new string(source.Reverse().ToArray()) + text;
                }
            }
        }
        public class OpenRoomCode : BaseInfo
        {
            public OpenRoomCode()
            {
                base.Bytes = new List<byte> { 0, 0, 0, 0 };
            }

            public OpenRoomCode(int openCode)
            {
                byte[] bytes = BitConverter.GetBytes(openCode);
                base.Bytes = bytes.ToList();
            }

            public override List<byte> TransToBytes()
            {
                return base.Bytes;
            }

            public override void TransToData(List<byte> bys)
            {
                base.Bytes = new List<byte>(bys.ToArray());
                string text = "(" + BitConverter.ToString(bys.ToArray()) + ")";
                bys.Reverse();
                base.Info = BitConverter.ToInt32(bys.ToArray(), 0) + text;
            }
        }
        public class CardSerialID : BaseInfo
        {
            public CardSerialID()
            {
                base.Bytes = new List<byte> { 0, 0 };
            }

            public CardSerialID(int serialID)
            {
                byte[] bytes = BitConverter.GetBytes(serialID);
                base.Bytes = new List<byte>
            {
                bytes[0],
                bytes[1]
            };
            }

            public override List<byte> TransToBytes()
            {
                return base.Bytes;
            }

            public override void TransToData(List<byte> bys)
            {
                if (bys.Count != 0)
                {
                    base.Bytes = new List<byte>(bys);
                    string text = "(" + BitConverter.ToString(bys.ToArray()) + ")";
                    bys.Add(0);
                    bys.Add(0);
                    base.Info = BitConverter.ToInt32(bys.ToArray(), 0) + text;
                }
            }
        }
        public class BaseInfo
        {
            protected List<byte> Bytes { get; set; }

            public string Info { get; protected set; }

            public virtual List<byte> TransToBytes()
            {
                return null;
            }

            public virtual void TransToData(List<byte> bys)
            {
                Info = BitConverter.ToString(bys.ToArray());
            }
        }

        public class ElevatorInfo : BaseInfo
        {
            public ElevatorInfo()
            {
                base.Bytes = new List<byte> { 0, 0, 0, 0, 0, 0, 0, 0 };
            }

            public ElevatorInfo(List<int> lstSerialID)
            {
                base.Bytes = new List<byte>();
                lstSerialID = lstSerialID.OrderBy((int x) => x).ToList();
                string text = string.Empty;
                for (int i = 1; i <= 64; i++)
                {
                    text += (lstSerialID.Contains(i - 1) ? "1" : "0");
                    if (i % 8 == 0)
                    {
                        byte item = new string(text.Reverse().ToArray()).ToByte_2();
                        base.Bytes.Add(item);
                        text = string.Empty;
                    }
                }
            }

            public override List<byte> TransToBytes()
            {
                return base.Bytes;
            }

            public override void TransToData(List<byte> bys)
            {
                if (bys.Count == 0)
                {
                    return;
                }

                base.Bytes = new List<byte>(bys);
                string text = "(" + BitConverter.ToString(bys.ToArray()) + ")";
                base.Info = string.Empty;
                foreach (byte by in bys)
                {
                    base.Info += new string(by.To8Binary().Reverse().ToArray());
                }

                base.Info += text;
            }
        }
    }
}
