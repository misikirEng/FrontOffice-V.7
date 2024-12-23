using CNET.ERP.Client.Common.UI;
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
    public class AdelLock : IDoorLock
    {
        private string _server = "";
        private string _userName = "";
        private short _software = -1;
        private short _port = 1;
        private short _encoder = 0; //manual
        private short _TMEncoder = 5; //DS9097U

        private string _gate = "99";

        /* sound
         * 11- green light, 1 second, a long beep
         * 12- red light, 1 second, a long beep
         * 15- red light, 1 second, short beeps twice
         * 16- one short beep
         */
        private const short GREEN_LONG_BEEP = 11;
        private const short RED_LONG_BEEP = 12;
        private const short RED_SHORT_BEEP_TWICE = 15;
        private const short ONE_SHORT_BEEP = 16;

        private long cardNumber = 0;

        public bool InitializeLock(List<ConfigurationDTO> deviceConfigs, DeviceDTO device)
        {
            try
            {
                //Adel Software
                var adelSystem = deviceConfigs.FirstOrDefault(c => c.Attribute.ToLower() == "adel system");
                if (adelSystem != null)
                {
                    _software = GetAdelSystem(adelSystem.CurrentValue);

                }

                if (_software == -1)
                {
                    SystemMessage.ShowModalInfoMessage("ERROR. INVALID ADEL SOFTWARE TYPE.", "ERROR");
                    return false;
                }

                //Host
                var hostConfig = deviceConfigs.FirstOrDefault(c => c.Attribute.ToLower() == "host");
                if (hostConfig != null)
                {
                    _server = hostConfig.CurrentValue;
                }

                //User
                var userConfig = deviceConfigs.FirstOrDefault(c => c.Attribute.ToLower() == "user");
                if (userConfig != null)
                {
                    _userName = userConfig.CurrentValue;
                }

                //TM Encoder
                var tmEncoderConfig = deviceConfigs.FirstOrDefault(c => c.Attribute.ToLower() == "tm encoder");
                if (tmEncoderConfig != null)
                {
                    if (tmEncoderConfig.CurrentValue == "DS9097U")
                        _TMEncoder = 5;
                    else if (tmEncoderConfig.CurrentValue == "DS9097E")
                        _TMEncoder = 1;
                }

                //Adel Encoder Type
                var encoderTypeConfig = deviceConfigs.FirstOrDefault(c => c.Attribute.ToLower() == "adel encoder type");
                if (encoderTypeConfig != null)
                {
                    if (encoderTypeConfig.CurrentValue.ToLower() == "manual")
                        _encoder = 0;
                    else if (encoderTypeConfig.CurrentValue.ToLower() == "automatic")
                        _encoder = 1;

                }

                //get Serial parameter 
                if (device.SerialPort == null || device.SerialPort == 0)
                {
                    SystemMessage.ShowModalInfoMessage("ERROR. EMPTY SERIAL PORT.", "ERROR");
                    return false;
                }
                _port = Convert.ToInt16(device.SerialPort);


                int st = Init(_software, new StringBuilder(_server), new StringBuilder(_userName), _port, _encoder, _TMEncoder);
                if (st != 0)
                {
                    Reader_Beep(RED_SHORT_BEEP_TWICE);
                    ShowStatusMessage(st);
                    return false;
                }
                Reader_Beep(GREEN_LONG_BEEP);
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
            return "Adel Door Lock";
        }

        public void ShowStatusMessage(int status)
        {
            switch (status)
            {
                case 0:
                    //SystemMessage.ShowModalInfoMessage("SUCCESS!!", "MESSAGE");
                    break;
                case 1:
                    SystemMessage.ShowModalInfoMessage("ERROR. READ/WRITE ERROR.", "ERROR");
                    break;
                case 2:
                    SystemMessage.ShowModalInfoMessage("ERROR. BROKEN CARD.", "ERROR");
                    break;
                case 3:
                    SystemMessage.ShowModalInfoMessage("ERROR. NO CARD DETECTED.", "ERROR");
                    break;
                case 4:
                    SystemMessage.ShowModalInfoMessage("ERROR. SERIAL PORT ERROR.", "ERROR");
                    break;
                case 5:
                    SystemMessage.ShowModalInfoMessage("ERROR. CARD BEING CHANGED.", "ERROR");
                    break;
                case 6:
                    SystemMessage.ShowModalInfoMessage("ERROR. NOT NEW CARD.", "ERROR");
                    break;
                case 7:
                    SystemMessage.ShowModalInfoMessage("ERROR. NEW CARD.", "ERROR");
                    break;
                case 8:
                    SystemMessage.ShowModalInfoMessage("ERROR. INVALID CARD.", "ERROR");
                    break;
                case 9:
                    SystemMessage.ShowModalInfoMessage("ERROR. NOT GUEST CARD.", "ERROR");
                    break;
                case 10:
                    SystemMessage.ShowModalInfoMessage("ERROR. NOT MEMBER CARD.", "ERROR");
                    break;
                case 11:
                    SystemMessage.ShowModalInfoMessage("ERROR. WRONG PIN.", "ERROR");
                    break;
                case 12:
                    SystemMessage.ShowModalInfoMessage("ERROR. NO ACCESS RECORD.", "ERROR");
                    break;
                case 13:
                    SystemMessage.ShowModalInfoMessage("ERROR. WRONG CARD TYPE.", "ERROR");
                    break;
                case 14:
                    SystemMessage.ShowModalInfoMessage("ERROR. WRONG PARAMETER.", "ERROR");
                    break;
                case 15:
                    SystemMessage.ShowModalInfoMessage("ERROR. CANCELED BY OPERATOR.", "ERROR");
                    break;
                case 16:
                    SystemMessage.ShowModalInfoMessage("ERROR. WAIT TIMEOUT.", "ERROR");
                    break;
                case 17:
                    SystemMessage.ShowModalInfoMessage("ERROR. WRONG INSERT.", "ERROR");
                    break;
                case 18:
                    SystemMessage.ShowModalInfoMessage("ERROR. EMPTY CARD.", "ERROR");
                    break;
                case 19:
                    SystemMessage.ShowModalInfoMessage("ERROR. RESERVED.", "ERROR");
                    break;
                case 20:
                    SystemMessage.ShowModalInfoMessage("ERROR. NOT INITIALIZED.", "ERROR");
                    break;
                case 21:
                    SystemMessage.ShowModalInfoMessage("ERROR. UNSUPPORTED VERSION.", "ERROR");
                    break;
                case 22:
                    SystemMessage.ShowModalInfoMessage("ERROR. DATABASE CONNECTION ERROR.", "ERROR");
                    break;
                case 23:
                    SystemMessage.ShowModalInfoMessage("ERROR. DOOR LOCK SYSTEM PARAMETER DOESN'T EXIST.", "ERROR");
                    break;
                case 24:
                    SystemMessage.ShowModalInfoMessage("ERROR. INITIALIZATION FAILURE.", "ERROR");
                    break;
                case 25:
                    SystemMessage.ShowModalInfoMessage("ERROR. NO GUEST/THE GUEST DOESN'T EXIST.", "ERROR");
                    break;
                case 26:
                    SystemMessage.ShowModalInfoMessage("ERROR. GUEST ROOM FULL.", "ERROR");
                    break;
                case 27:
                    SystemMessage.ShowModalInfoMessage("ERROR. NO RECORD FOR THE CARD.", "ERROR");
                    break;
                case 28:
                    SystemMessage.ShowModalInfoMessage("ERROR. PORT IS NOT SET.", "ERROR");
                    break;
                case 29:
                    SystemMessage.ShowModalInfoMessage("ERROR. INVALID GUEST ROOM NUMBER.", "ERROR");
                    break;
                case 30:
                    SystemMessage.ShowModalInfoMessage("ERROR. INVALID TIME FRAME.", "ERROR");
                    break;
                case 31:
                    SystemMessage.ShowModalInfoMessage("ERROR. CARD NUMBER EXISTS.", "ERROR");
                    break;
                case 32:
                    SystemMessage.ShowModalInfoMessage("ERROR. DOES NOT SUPPORT.", "ERROR");
                    break;
                case 33:
                    SystemMessage.ShowModalInfoMessage("ERROR. INVALID AUTHORIZATION KEY (OR EXPIRED AUTH. KEY).", "ERROR");
                    break;
                default:
                    SystemMessage.ShowModalInfoMessage("ERROR. OPERATION IS NOT SUCCESSFULL.", "ERROR");
                    break;
            }
        }

        public string GetCardSN(bool showStatusMessage = true)
        {
            try
            {
                ulong cardId;
                int readStatus = ReadCardId(out cardId);
                if (readStatus == 0)
                {
                    return cardId.ToString();
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

                short status = -1;


                StringBuilder lockNumber = new StringBuilder(10);
                StringBuilder gate = new StringBuilder(10);
                StringBuilder sTime = new StringBuilder(24);

                int st = ReadCard(lockNumber, gate, sTime, null, null, null, null, ref cardNumber, ref status);
                if (st == 0)
                {

                    string cardSN = GetCardSN(false);
                    Reader_Beep(ONE_SHORT_BEEP);

                    cInfo.CardNumber = cardSN;
                    cInfo.LockNumber = lockNumber.ToString();

                    string startStr = sTime.ToString().Substring(0, 12);
                    string endStr = sTime.ToString().Substring(12, 12);
                    string format = GetStringFormatOfDate();

                    cInfo.StartDate = DateTime.ParseExact(startStr, format, CultureInfo.InvariantCulture).ToString("dd-MM-yyyy HH:mm");
                    cInfo.EndDate = DateTime.ParseExact(endStr, GetStringFormatOfDate(), CultureInfo.InvariantCulture).ToString("dd-MM-yyyy HH:mm");


                }
                else
                {
                    if (showStatusMessage && st != 7)
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

                if (lockNumber.Length != 6)
                {
                    XtraMessageBox.Show("Length of lock number must be 6.", "Door Lock", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                string intime = startDate.ToString(GetStringFormatOfDate());
                string outtime = endDate.ToString(GetStringFormatOfDate());


                StringBuilder sbTime = new StringBuilder(24);
                sbTime.Append(intime);
                sbTime.Append(outtime);

                StringBuilder sbLock = new StringBuilder(6);
                sbLock.Append(lockNumber);

                short overrideFlag = 1;
                StringBuilder gate = new StringBuilder(2);
                gate.Append("00");

                long cardSN = 0;
                int st = 0;
                if (isDuplicate)
                {
                    st = DupKey(sbLock, gate, sbTime, new StringBuilder("guest"), new StringBuilder("11"), overrideFlag, ref cardSN, null, null);

                }
                else
                {
                    st = NewKey(sbLock, gate, sbTime, new StringBuilder("guest"), new StringBuilder("11"), overrideFlag, ref cardSN, null, null);

                }

                if (st != 0)
                {
                    Reader_Beep(RED_SHORT_BEEP_TWICE);
                    ShowStatusMessage(st);
                    return false;
                }

                Reader_Beep(ONE_SHORT_BEEP);
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

                int st = EraseCard(0, null, null);
                if (st != 0)
                {
                    ShowStatusMessage(st);
                    return false;
                }
                Reader_Beep(ONE_SHORT_BEEP);
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
            return "yyyyMMddHHmm";
        }

        private short GetAdelSystem(string type)
        {
            switch (type)
            {
                case "Lock3200":
                    return 1;
                case "Lock3200K":
                    return 2;
                case "Lock4200":
                    return 3;
                case "Lock4200D":
                    return 4;
                case "Lock5200":
                    return 5;
                case "Lock6200":
                    return 6;
                case "Lock7200":
                    return 7;
                case "Lock7200D":
                    return 8;
                case "Lock9200":
                    return 9;
                case "Lock9200T":
                    return 10;
                case "A30":
                    return 11;
                case "A50":
                    return 14;
                case "A90":
                    return 18;
                case "A92":
                    return 22;
                default:
                    return -1;
            }
        }

        #region Dll Import

        [DllImport(@"lib\adel\MainDll.dll", EntryPoint = "Init")]
        public static extern int Init(short software, StringBuilder server, StringBuilder userName, short port, short encoder, short TMEncoder);

        [DllImport(@"lib\adel\MainDll.dll", EntryPoint = "EndSession")]
        public static extern int EndSession();

        /// <summary>
        /// Set port，not necessary after call Init function
        /// </summary>
        /// <returns></returns>
        [DllImport(@"lib\adel\MainDll.dll", EntryPoint = "SetPort")]
        public static extern int SetPort(short software, short port, short encoder, short TMEncoder);

        /// <summary>
        /// Issue guest Card
        /// </summary>
        /// <param name="lockNumber">6 byte  </param>
        /// <param name="gate">"00" =  Default authorized gate, "99" = all common gate, "010203" => gate 01,02,03   </param>
        /// <param name="stime">24 byte. format => yyyymmddhhnnyyyymmddhhnn ---> start and end date on the same except for Lock9200 software</param>
        /// <param name="guestName">30 byte => can be NULL </param>
        /// <param name="guestId">30 byte => can be NULL </param>
        /// <param name="overwriteflag">  1 = replace, 0 = don't replaced. ---> for magnetic card (manual encoder) must avoid this parameter, replace directly </param>
        /// <param name="cardNumber"> returned long card number. can be NULL </param>
        /// <param name="track1"> magnetic first track data. can be NULL </param>
        /// <param name="track2">magnetic second track data. can be NULL </param>
        /// <returns> integer status </returns>
        [DllImport(@"lib\adel\MainDll.dll", EntryPoint = "NewKey")]
        public static extern int NewKey(StringBuilder lockNumber, StringBuilder gate, StringBuilder stime, StringBuilder guestName, StringBuilder guestId, short overwriteflag, ref long cardNumber, StringBuilder track1, StringBuilder track2);

        /// <summary>
        /// duplicate”guest card，new issued guest card can work with the current guest card and both cards can work together，
        /// both cards has the same start and end date
        /// </summary>
        [DllImport(@"lib\adel\MainDll.dll", EntryPoint = "DupKey")]
        public static extern int DupKey(StringBuilder lockNumber, StringBuilder gate, StringBuilder stime, StringBuilder guestName, StringBuilder guestId, short overwriteflag, ref long cardNumber, StringBuilder track1, StringBuilder track2);

        /// <summary>
        /// Read card
        /// </summary>
        /// <param name="lockNumber"> 10 byte </param>
        /// <param name="gate"> authorized common gate. can be NULL </param>
        /// <param name="guestName"> can be NULL. </param>
        /// <param name="guestId"> can be NULL</param>
        /// <param name="track1"> can be NULL </param>
        /// <param name="track2"> can be NULL </param>
        /// <param name="cardNo"> return card number </param>
        /// <param name="status"> receive returned card status. 1 - normal use, 3- normal erase, 4 - lost erase, 5 - damage erase, 6 - auto erase.  Can be NULL. </param>
        /// <returns></returns>
        [DllImport(@"lib\adel\MainDll.dll", EntryPoint = "ReadCard")]
        public static extern int ReadCard(StringBuilder lockNumber, StringBuilder gate, StringBuilder sTime, StringBuilder guestName, StringBuilder guestId, StringBuilder track1, StringBuilder track2, ref long cardNo, ref short status);

        /// <summary>
        /// reads the informations from the card
        /// </summary>
        /// <param name="cardNumber"></param>
        /// <param name="cardType"> 9 -> for guest card, 4-master card</param>
        /// <param name="cardStatus"> 1-normal，3-erase，4-lost，5-damage，6-past due； can be NULL </param>
        /// <param name="roomNumber">receive guest card/spare card room number. Not less than 20 byte</param>
        /// <param name="userName"> receive guest name，no less than 20 byte，can be NULL.</param>
        /// <param name="stime"> receive guest card/maid card expiry date，no less than 28 byte，can be NULL.</param>
        /// <returns></returns>
        [DllImport(@"lib\adel\MainDll.dll", EntryPoint = "GetCardInfo")]
        public static extern int GetCardInfo(long cardNumber, short cardType, out short cardStatus, out StringBuilder roomNumber, out StringBuilder userName, out StringBuilder stime);

        /// <summary>
        /// Erase card，at the same time updating database
        /// </summary>
        /// <param name="cardNumber">can be 0. When the parameter is 0：IC card, RF card, TM card, Magnetic card（automatic encoder）automatically read the card number and erase，at the same time update the database；
        /// magnetic card（manual encoder）when erase the card，does not update database，suggest use CheckOut function update the database，or get the card number first by read card then use EraseCard.</param>
        /// <returns> status flag</returns>
        [DllImport(@"lib\adel\MainDll.dll", EntryPoint = "EraseCard")]
        public static extern int EraseCard(int cardNumber, StringBuilder track1, StringBuilder track2);

        /// <summary>
        /// Check out，only update database，it does not erase card
        /// </summary>
        /// <param name="lockNumber"> room number </param>
        /// <param name="cardNumber"> card number，can be 0.When parameter is 0，guest card labels normal erase </param>
        /// <returns></returns>
        [DllImport(@"lib\adel\MainDll.dll", EntryPoint = "CheckOut")]
        public static extern int CheckOut(StringBuilder lockNumber, long cardNumber);

        /// <summary>
        /// Lost erase，only update database，do not erase card
        /// </summary>
        /// <returns></returns>
        [DllImport(@"lib\adel\MainDll.dll", EntryPoint = "Lostcard")]
        public static extern int Lostcard(StringBuilder lockNumber, long cardNumber);

        [DllImport(@"lib\adel\MainDll.dll", EntryPoint = "ReadCardId")]
        public static extern int ReadCardId(out ulong cardId);

        [DllImport(@"lib\adel\MainDll.dll", EntryPoint = "PopCard")]
        public static extern int PopCard();

        [DllImport(@"lib\adel\MainDll.dll", EntryPoint = "Reader_Beep")]
        public static extern int Reader_Beep(short sound);
        #endregion
    }
}
