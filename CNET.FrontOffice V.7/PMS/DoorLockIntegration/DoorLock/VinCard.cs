using CNET.ERP.Client.Common.UI;

using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CNET_V7_Domain.Domain.SettingSchema;
using CNET_V7_Domain.Domain.ArticleSchema;

namespace CNET.FrontOffice_V._7.PMS.DoorLockIntegration.DoorLock
{
    public class VINCard : IDoorLock
    {
        string VisiolineServiceIP { get; set; }
        int VisiolineServicePort { get; set; }
        int EncoderAddress { get; set; } 

        public bool InitializeLock(List<ConfigurationDTO> deviceConfigs, DeviceDTO device)
        {
            try
            {
                if(device.IpAddress != null )
                    VisiolineServiceIP = device.IpAddress;
                else
                    XtraMessageBox.Show("No Visio line Service Ip Address !!", "Door Lock", MessageBoxButtons.OK, MessageBoxIcon.Error);


                if (device.IpAddress != null)
                    VisiolineServicePort = device.IpPort.Value;
                else
                    XtraMessageBox.Show("No Visio line Service Ip Port !!", "Door Lock", MessageBoxButtons.OK, MessageBoxIcon.Error);


                /*  var VisiolineServiceIPconfig = deviceConfigs.FirstOrDefault(c => c.attribute.ToLower() == "visioline service ip");
                  if (VisiolineServiceIPconfig != null)
                  {
                      VisiolineServiceIP = VisiolineServiceIPconfig.currentValue;
                  }

                  var VisiolineServicePortconfig = deviceConfigs.FirstOrDefault(c => c.attribute.ToLower() == "visioline service port");
                  if (VisiolineServicePortconfig != null)
                  {
                      VisiolineServicePort = Convert.ToInt32(VisiolineServicePortconfig.currentValue);
                  }*/

                var EncoderAddressconfig = deviceConfigs.FirstOrDefault(c => c.Attribute.ToLower() == "encoder address");
                if (EncoderAddressconfig != null)
                {
                    EncoderAddress = Convert.ToInt32(EncoderAddressconfig.CurrentValue);
                }

                return TestConnectiontoserver();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Exception has occured in initializing door lock. Detail:: " + ex.Message, "Door Lock", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

        }

        public string GetDoorLockName()
        {
            return "VingCard Door Lock";
        }

        public void ShowStatusMessage(int status)
        {

        }



        public string GetCardSN(bool showStatusMessage = true)
        {
            string RoomNumber = "";
            string CardData = ReadCard();
            if (!string.IsNullOrEmpty(CardData))
            {
                List<string> part1 = CardData.Replace(";", ",").Split(',').ToList();
                RoomNumber = part1.FirstOrDefault(x => x.StartsWith("GR")).Replace("GR", "");
            }
            return RoomNumber;
        }

        public CardInfo ReadCardData(bool showStatusMessage = true)
        {

            CardInfo RoomCardInfo = null;

            string CardData = ReadCard();


            if (CardData.Contains("The card is valid"))
            {
                List<string> part1 = CardData.Replace(";", ",").Split(',').ToList();
                string RoomNumber = part1.FirstOrDefault(x => x.StartsWith("GR")).Replace("GR", "");
                string CheckInDatetime = part1.FirstOrDefault(x => x.StartsWith("CI")).Replace("CI", "");
                string CheckOutDatetime = part1.FirstOrDefault(x => x.StartsWith("CO")).Replace("CO", "");
                DateTime checkinTime = DateTime.ParseExact(CheckInDatetime, "yyyyMMddHHmm", null);
                DateTime checkOUTTime = DateTime.ParseExact(CheckOutDatetime, "yyyyMMddHHmm", null);

                RoomCardInfo = new CardInfo
                {
                    CardNumber = RoomNumber,
                    LockNumber = RoomNumber,
                    StartDate = checkinTime.ToString("yyyyMMdd"),
                    EndDate = checkOUTTime.ToString("yyyyMMdd")

                };

            }
            return RoomCardInfo;
        }

        public bool IssueGuestCard(string lockNumber, DateTime startDate, DateTime endDate, bool isDuplicate = false)
        {
            return IssueCard(lockNumber, endDate);
        }

        public bool ClearCard(string lockNumber = null)
        {
            return CheckOut(lockNumber);
        }

        public string GetStringFormatOfDate()
        {
            return "yyyyMMddhhmmss";
        }

        #region Methods For Interface

        string ReturnSuccsses = "RC0,".Replace(",", ";");
        string ReturnFail = "RC1,".Replace(",", ";");
        string Thecardisvalid = "The card is valid";

        private string StartClient(string Data)
        {
            string Result = "";
            IPAddress ipAddress = IPAddress.Parse(VisiolineServiceIP);
            // IPEndPoint remoteEP = new IPEndPoint(ipAddress, VisiolineServicePort);
            Socket client = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                client.ReceiveTimeout = 5000;
                client.Connect(ipAddress, VisiolineServicePort);
                //txtInfo.Text += "> " + Data + Environment.NewLine;
                byte[] byteData = Encoding.ASCII.GetBytes(Data + "\r\n");
                try
                {
                    client.Send(byteData);
                    byte[] reciveddd = new byte[200];
                    client.Receive(reciveddd);
                    Result = Encoding.ASCII.GetString(reciveddd);
                    //  txtInfo.Text += "< " + Result.ToString() + Environment.NewLine;
                    client.Shutdown(SocketShutdown.Both);
                    client.Close();
                }
                catch (Exception io)
                {
                    // MessageBox.Show(io.Message, "Error");
                    client.Shutdown(SocketShutdown.Both);
                    client.Close();
                    return Result;
                }
            }
            catch (Exception e)
            {
                // Console.WriteLine(e.ToString());
                client.Shutdown(SocketShutdown.Both);
                client.Close();
                return Result;
            }
            return Result;
        }

        public bool TestConnectiontoserver()
        {
            bool connected = false;
            try
            {
                string Result = StartClient("CCC,EAHEARTBEAT,AM1".Replace(",", ";"));
                if (Result.Contains(ReturnSuccsses))
                {
                    connected = true;
                }
                else
                {
                    connected = false;
                }
            }
            catch (Exception io)
            {
                return connected;
            }
            return connected;
        }

        public bool IssueCard(string RoomNumber, DateTime Checkout)
        {
            bool Issued = false;
            try
            {
                string IssueCardString = ("CCA,EA" + EncoderAddress + ",GR" + RoomNumber + ",CO" + Checkout.ToString("yyyyMMddhhmm") + ",AM1").Replace(",", ";");
                string Result = StartClient(IssueCardString);
                if (Result.Contains(ReturnSuccsses))
                {
                    Issued = true;
                }
                else
                {
                    Issued = false;
                }
            }
            catch (Exception io)
            {
                return Issued;
            }
            return Issued;
        }

        public string ReadCard()
        {
            string carddata = "";
            try
            {
                string ReadCardstring = ("CCB,EA" + EncoderAddress + ",TAFDV03,AM1").Replace(",", ";");
                string Result = StartClient(ReadCardstring);
                if (Result.Contains(Thecardisvalid))
                {
                    carddata = Result;
                }
                else
                {
                    carddata = "";
                }
            }
            catch (Exception io)
            {
                return carddata;
            }
            return carddata;
        }

        public bool CheckOut(string RoomNumber)
        {
            bool Issued = false;
            try
            {
                string CheckOutstring = ("CCG,EA" + EncoderAddress + ",GR" + RoomNumber + ",AM1").Replace(",", ";");
                string Result = StartClient(CheckOutstring);
                if (Result.Contains(ReturnSuccsses))
                {
                    Issued = true;
                }
                else
                {
                    Issued = false;
                }
            }
            catch (Exception io)
            {
                return Issued;
            }
            return Issued;
        }

        public bool ExtendCard(string RoomNumber, DateTime Checkout)
        {
            bool Issued = false;
            try
            {
                string ExtendCardstring = ("CCF,EA" + EncoderAddress + ",GR" + RoomNumber + ",CO" + Checkout.ToString("yyyyMMddhhmm") + ",AM1").Replace(",", ";");
                string Result = StartClient(ExtendCardstring);
                if (Result.Contains(ReturnSuccsses))
                {
                    Issued = true;
                }
                else
                {
                    Issued = false;
                }
            }
            catch (Exception io)
            {
                return Issued;
            }
            return Issued;
        }

        public bool MoveRoom(string OldRoomNumber, string NewRoomNumber)
        {
            bool Issued = false;
            try
            {
                string MoveRoomstring = ("CCF,EA" + EncoderAddress + ",GR" + OldRoomNumber + ",NR" + NewRoomNumber + ",AM1").Replace(",", ";");
                string Result = StartClient(MoveRoomstring);
                if (Result.Contains(ReturnSuccsses))
                {
                    Issued = true;
                }
                else
                {
                    Issued = false;
                }
            }
            catch (Exception io)
            {
                return Issued;
            }
            return Issued;
        }


        #endregion



    }
}
