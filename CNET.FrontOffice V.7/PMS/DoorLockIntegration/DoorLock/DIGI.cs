using CNET.ERP.Client.Common.UI;
using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CNET_V7_Domain.Domain.SettingSchema;
using CNET.FrontOffice_V._7.PMS.Common_Classes;
using IssueTCrd;

namespace CNET.FrontOffice_V._7.PMS.DoorLockIntegration.DoorLock
{
    public class DIGI : IDoorLock
    {



        public string GetDoorLockName()
        {
            return "DIGI Door Lock";
        }


        public void ShowStatusMessage(int status)
        {
            switch (status)
            {
                case 0:
                    //SystemMessage.ShowModalInfoMessage("SUCCESS!!", "MESSAGE");
                    break;
                case -1:
                    SystemMessage.ShowModalInfoMessage("ERROR. Register Error.", "ERROR");
                    break;
                case -99:
                    SystemMessage.ShowModalInfoMessage("ERROR. Com Port Error. (Check Uncoder Com Port).", "ERROR");
                    break;
                case -128:
                    SystemMessage.ShowModalInfoMessage("ERROR. Read Error. (Check The Card).", "ERROR");
                    break;
                case -129:
                    SystemMessage.ShowModalInfoMessage("ERROR. Write Error. (Check The Card).", "ERROR");
                    break;
                case -201:
                    SystemMessage.ShowModalInfoMessage("ERROR. Digilock system is not Activate.", "ERROR");
                    break;
                case -202:
                    SystemMessage.ShowModalInfoMessage("ERROR. Register ID Error(Check The Register ID).", "ERROR");
                    break;
                case -203:
                    SystemMessage.ShowModalInfoMessage("ERROR. Register ID Error(Contact Supplier).", "ERROR");
                    break;
                case -300:
                    SystemMessage.ShowModalInfoMessage("ERROR. Card Error (Check The Card,Try other card).", "ERROR");
                    break;
                case -310:
                    SystemMessage.ShowModalInfoMessage("ERROR. Card Error (Check The Card,Try other card).", "ERROR");
                    break;
                case -500:
                    SystemMessage.ShowModalInfoMessage("ERROR. The Card is Reset Card (Change the card).", "ERROR");
                    break;
                case -600:
                    SystemMessage.ShowModalInfoMessage("ERROR. The Card is Special Card(Change the card).", "ERROR");
                    break;
                case -700:
                    SystemMessage.ShowModalInfoMessage("ERROR. Empty Card.", "ERROR");
                    break;
                case -800:
                    SystemMessage.ShowModalInfoMessage("ERROR. Register Error.", "ERROR");
                    break;
                case -810:
                    SystemMessage.ShowModalInfoMessage("ERROR. Lost The Setting.INI.", "ERROR");
                    break;
                case -854:
                    SystemMessage.ShowModalInfoMessage("ERROR. Building No. Error(Check Building No., Format :1-99).", "ERROR");
                    break;
                case -855:
                    SystemMessage.ShowModalInfoMessage("ERROR. Floor No. Error(Check FloorNo. , Format :1-99).", "ERROR");
                    break;
                case -856:
                    SystemMessage.ShowModalInfoMessage("ERROR. Room No. Error (Check Room No.).", "ERROR");
                    break;
                default:
                    SystemMessage.ShowModalInfoMessage("ERROR. OPERATION IS FAILED.", "ERROR");
                    break;
            }
        }

        public string GetCardSN(bool showStatusMessage = true)
        {
            string CardNo = "";
            string lockNumber = "";
            string EndDate = "";
            try
            {

                DigiCard pp = new DigiCard();
                int st = pp.ReadCard(_port, ref CardNo, ref lockNumber, ref EndDate);

                if (st == 0)
                {
                    return CardNo;
                }
                else
                {
                    return "999999**";
                    //  if (showStatusMessage)
                    //  {
                    //   ShowStatusMessage(st);
                    //  }
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

                string CardNo = "";
                string lockNumber = "";
                string EndDate = "";

                DigiCard pp = new DigiCard();
                int st = pp.ReadCard(_port, ref CardNo, ref lockNumber, ref EndDate);

                if (st == 0)
                {
                    cInfo.CardNumber = CardNo.ToString();
                    cInfo.LockNumber = lockNumber.ToString();
                    // cInfo.StartDate = intime.ToString();
                    cInfo.EndDate = EndDate.ToString();

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

                string CardNo = "00000000";
                string RoomNo = lockNumber.Substring(4, lockNumber.Length - 4);
                string Holder = "holder01";
                short BuildNo = Convert.ToInt16(lockNumber.Substring(0, 2));
                short FloorNo = Convert.ToInt16(lockNumber.Substring(2, 2));
                string WriteEndDate = endDate.ToString("yyyyMMddhhmm");

                DigiCard pp = new DigiCard();
                short st = pp.WriteCard(_port, WriteEndDate, CardNo, BuildNo, FloorNo, RoomNo, Holder);
                if (st != 0)
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
            return IssueGuestCard("9999", DateTime.Now, DateTime.Now.Date);
        }

        public string GetStringFormatOfDate()
        {
            return "yyyyMMddhhmm";
        }

        public bool InitializeLock(List<ConfigurationDTO> deviceConfigs, DeviceDTO device)
        { 
            if (device.SerialPort == null || device.SerialPort.Value == 0)
            {
                SystemMessage.ShowModalInfoMessage("ERROR. EMPTY SERIAL Port.", "ERROR");
                return false;
            }
            _port = Convert.ToInt32(device.SerialPort.Value);

            return true;
        }

        public int _port { get; set; }
    }
}
