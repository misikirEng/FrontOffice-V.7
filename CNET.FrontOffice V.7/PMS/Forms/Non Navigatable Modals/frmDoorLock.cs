
using CNET.ERP.Client.Common.UI;
using CNET.FrontOffice_V._7;
using ProcessManager;
using CNET.FrontOffice_V._7.PMS.Common_Classes;
using CNET.FrontOffice_V._7.PMS.DoorLockIntegration.DoorLock;
using CNET_V7_Domain.Domain.CommonSchema;
using CNET_V7_Domain.Domain.PmsSchema;
using CNET_V7_Domain.Domain.SettingSchema;
using CNET_V7_Domain.Domain.TransactionSchema;
using CNET_V7_Domain.Misc.PmsDTO;
using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CNET.FrontOffice_V._7.Forms.Non_Navigatable_Modals
{
    public partial class frmDoorLock : UILogicBase
    {

        private IDoorLock _doorLock = null;
        // private string _currentCardNo = "";

        private List<string> _addedTranList = null;

        private DateTime _endTime;

        public string SavedCardNumber { get; set; }
        private RegistrationListVMDTO _regExt;
        private int adCode;
        private int lastState;
        public RegistrationListVMDTO RegExt
        {
            get
            {
                return _regExt;
            }
            set
            {
                _regExt = value;
            }
        }

        /***************************** CONSTRUCTOR ************************/
        public frmDoorLock()
        {
            InitializeComponent();
        }

        #region Helper Method

        private void InitializeUI()
        {

        }

        private bool InitializeData()
        {
            try
            {
                if (RegExt == null)
                {
                    SystemMessage.ShowModalInfoMessage("Please Select Registration!", "ERROR");
                    return false;
                }

                //note: date format really matters here!
                teArrivalDate.Text = RegExt.Arrival.ToString("yyyy-MM-dd HH:mm:ss");
                teDepartureDate.Text = RegExt.Departure.ToString("yyyy-MM-dd HH:mm:ss");
                teGuest.Text = RegExt.Guest;
                teRegNum.Text = RegExt.Registration;
                teRoomType.Text = RegExt.RoomTypeDescription;
                teRoom.Text = RegExt.Room;

                //check workflow
                ActivityDefinitionDTO workFlow = UIProcessManager.GetActivityDefinitionBydescriptionandreference(CNETConstantes.LU_ACTIVITY_ISSUED, CNETConstantes.REGISTRATION_VOUCHER).FirstOrDefault();

                if (workFlow != null)
                {

                    adCode = workFlow.Id;
                    lastState = workFlow.State.Value;
                }
                else
                {
                    SystemMessage.ShowModalInfoMessage("Please define workflow of ISSUED for Registration Voucher ", "ERROR");
                    return false;
                }

                var configCheckoutTime = LocalBuffer.LocalBuffer.ConfigurationBufferList.FirstOrDefault(c => c.Reference == CNETConstantes.PMS_Pointer.ToString() && c.Attribute == CNETConstantes.PMS_SETTING_CheckOutTime);
                if (configCheckoutTime != null)
                {
                    DateTime checkoutTime = Convert.ToDateTime(configCheckoutTime.CurrentValue);
                    //DateTime checkoutTime = DateTime.ParseExact(configCheckoutTime.CurrentValue, "HH:mm:ss", CultureInfo.InvariantCulture);

                    _endTime = new DateTime(RegExt.Departure.Year, RegExt.Departure.Month, RegExt.Departure.Day,
                        checkoutTime.Hour, checkoutTime.Minute, checkoutTime.Second);
                    teDepartureDate.Text = _endTime.ToString("yyyy-MM-dd HH:mm:ss");
                }

                // Progress_Reporter.Show_Progress("Initializing Door Lock", "Please Wait...");


                DoorLockFactory dLockFactory = new DoorLockFactory();
                _doorLock = dLockFactory.GetDoorLock();
                if (_doorLock == null)
                {
                    SystemMessage.ShowModalInfoMessage("Unable to initialize door lock!", "ERROR");
                    ////CNETInfoReporter.Hide();
                    return false;

                }

                statLabel.Text = _doorLock.GetDoorLockName();

                //get last registration Detail
                RegistrationDetailDTO regDetail = UIProcessManager.GetRegistrationDetailByvoucher(RegExt.Id).OrderByDescending(r => r.Date).FirstOrDefault();
                if (regDetail != null)
                {
                    teAdultCount.Text = regDetail.Adult.ToString();
                }
                else
                {
                    teAdultCount.Text = "";
                }

                RoomDetailDTO rd = UIProcessManager.GetRoomDetailById(RegExt.RoomCode.Value);
                if (rd != null)
                {
                    List<KeyDefinitionDTO> KeyDefList = UIProcessManager.GetKeyDefinitionBySpace(rd.Space);
                    List<KeyOptionDTO> KeyOpList = UIProcessManager.GetKeyOptionByRoom(rd.Id);


                    if (KeyDefList != null && KeyOpList != null)
                    {
                        KeyDefinitionDTO Keydef = KeyDefList.FirstOrDefault();
                        KeyOptionDTO keyop = KeyOpList.FirstOrDefault();
                        if (Keydef != null)
                        {
                            teLockNumber.Text = Keydef.KeyCode;
                        }

                    }

                }
                //read no of issue
                GetCardTransaction();


                //********** read card data ***************/
                ReadCardData(false);

                ////CNETInfoReporter.Hide();

                return true;
            }
            catch (Exception ex)
            {
                ////CNETInfoReporter.Hide();
                SystemMessage.ShowModalInfoMessage("Error in initializing door lock. Detail:: " + ex.Message, "ERROR");
                return false;
            }
        }

        private List<string> GetCardTransaction()
        {

            //var voExtDoorLock = LocalBuffer.LocalBuffer.VoucherExtensionBufferList.FirstOrDefault(v => v.Type == CNETConstantes.VOUCHER_EXTENTION_DEFINITION_DOOR_LOCK && v.VoucherDefinition == CNETConstantes.REGISTRATION_VOUCHER);
            //if (voExtDoorLock != null)
            //{

            VoucherDTO checkdoor = UIProcessManager.GetVoucherById(RegExt.Id);

            if (!string.IsNullOrEmpty(checkdoor.Extension1))
                _addedTranList = checkdoor.Extension1.Split(",").ToList();// UIProcessManager.GetVoucherByExtension1(_currentCardNo);

            if (_addedTranList != null && _addedTranList.Count > 0)
            {
                teNoOfIssue.Text = _addedTranList.Count.ToString();

                int noAdult = Convert.ToInt32(teAdultCount.Text);
                int noIssue = Convert.ToInt32(teNoOfIssue.Text);

                if (noIssue == noAdult)
                {
                    bbiIssue.Caption = "Re-Issue";
                }
                else
                {
                    bbiIssue.Caption = "Issue";
                }

            }
            else
            {
                teNoOfIssue.Text = "0";
            }

            return _addedTranList;
            //}
            //else
            //{
            //    return null;
            //}
        }

        private List<VoucherDTO> GetCardTransactions(string number)
        {
            //var voExtDoorLock = LocalBuffer.LocalBuffer.VoucherExtensionBufferList.FirstOrDefault(v => v.Type == CNETConstantes.VOUCHER_EXTENTION_DEFINITION_DOOR_LOCK && v.VoucherDefinition == CNETConstantes.REGISTRATION_VOUCHER);
            //if (voExtDoorLock != null)
            //{
            var trans = UIProcessManager.GetVoucherByExtension1(number);
            return trans;
            //}
            //else
            //{
            //    return null;
            //}
        }

        private void ReadCardData(bool isFirstTime = true)
        {
            CardInfo cInfo = _doorLock.ReadCardData(isFirstTime);
            if (cInfo != null)
            {
                lblCardNo.Text = "Card No. : " + cInfo.CardNumber;
                lblLockNo.Text = "Lock No. : " + cInfo.LockNumber;
                lblStartDate.Text = "Start Date : " + cInfo.StartDate;
                lblEndDate.Text = "End Date : " + cInfo.EndDate;
            }

        }




        #endregion

        #region Event Handlers

        private void bbiIssue_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            try
            {

                // Progress_Reporter.Show_Progress("Issuing Guest Card", "Please Wait...");
               // MessageBox.Show("Getting time");
                DateTime? currentTime = UIProcessManager.GetServiceTime();
                if (currentTime == null)
                {
                    ////CNETInfoReporter.Hide();
                    return;
                }

                int noAdult = string.IsNullOrEmpty(teAdultCount.Text) ? 0 : Convert.ToInt32(teAdultCount.Text);
                int noIssue = string.IsNullOrEmpty(teNoOfIssue.Text) ? 0 : Convert.ToInt32(teNoOfIssue.Text);

                //MessageBox.Show("Getting GetCardSN");
                string cardSN = _doorLock.GetCardSN();
                if (string.IsNullOrEmpty(cardSN))
                {
                    ////CNETInfoReporter.Hide();
                    SystemMessage.ShowModalInfoMessage("Unable to get card's Serial Number!", "ERROR");
                    return;
                }

               // MessageBox.Show("Getting GetCardTransactions");
                var trans = GetCardTransactions(cardSN);
                if (trans != null && trans.Count > 0)
                {
                    var cardIssued = trans.FirstOrDefault(v => v.Id != RegExt.Id);
                    if (cardIssued != null && cardIssued.Count > 0 )
                    {
                        ////CNETInfoReporter.Hide();
                        SystemMessage.ShowModalInfoMessage("There is another issue with the current card!", "ERROR");
                        return;
                    }
                }

                //MessageBox.Show("Getting tran");
                string tran = null;
                if (noIssue > 0)
                {

                    if (_addedTranList != null)
                    {
                        tran = _addedTranList.FirstOrDefault(t => t == cardSN);
                        if (noIssue == noAdult && tran == null)
                        {
                            ////CNETInfoReporter.Hide();
                            SystemMessage.ShowModalInfoMessage("You can't Re-Issue with different guest card!", "ERROR");
                            return;
                        }

                    }

                }



               // MessageBox.Show("Getting intime");
                string lockNumber = teLockNumber.Text;
                DateTime intime =  currentTime.Value; //RegExt.ArrivalDate

                //MessageBox.Show("Getting isDuplicate");
                //for BE-TECH we need the first guest's serial number
                string guestSN = "FFFF";
                bool isDuplicate = false;
                if (DoorLockFactory.CURRENT_DOOR_LOCK != DoorLockType.DELUNS && DoorLockFactory.CURRENT_DOOR_LOCK != DoorLockType.DELUNSV10 && _addedTranList != null && _addedTranList.Count > 0)
                {
                    isDuplicate = true;
                    VoucherDTO voucher = UIProcessManager.GetVoucherById(RegExt.Id);

                    if (DoorLockFactory.CURRENT_DOOR_LOCK == DoorLockType.BETECH || DoorLockFactory.CURRENT_DOOR_LOCK == DoorLockType.BETECHNEW)
                        guestSN = voucher.Remark;
                    else
                       // intime = DateTime.ParseExact(voucher.Remark, _doorLock.GetStringFormatOfDate(), CultureInfo.InvariantCulture);
                    intime = currentTime.Value;
                }



                if (string.IsNullOrWhiteSpace(lockNumber))
                {
                    ////CNETInfoReporter.Hide();
                    SystemMessage.ShowModalInfoMessage("Invalid Lock Number", "ERROR");
                    return;
                }

                //for BE-TECH
                string guestId = "10";
                if (DoorLockFactory.CURRENT_DOOR_LOCK == DoorLockType.BETECH || DoorLockFactory.CURRENT_DOOR_LOCK == DoorLockType.BETECHNEW)
                {
                    Random rand = new Random();
                    guestId = rand.Next(1, 255).ToString();
                    lockNumber = lockNumber + "," + guestId + "," + guestSN;

                }
                // MessageBox.Show(_endTime.ToLongDateString());

                bool st = _doorLock.IssueGuestCard(lockNumber, intime, _endTime, isDuplicate);

                if (st)
                {
                    cardSN = _doorLock.GetCardSN();

                    if (cardSN == "FFFFFFFF" || cardSN == "CCCCCCCC")
                    {
                        ////CNETInfoReporter.Hide();
                        SystemMessage.ShowModalInfoMessage("ERROR. NO CARD!", "ERROR");
                        return;
                    }
                    else
                        SystemMessage.ShowModalInfoMessage("Card Issued !!");

                    //update voucher extension transaction
                    if (DoorLockFactory.CURRENT_DOOR_LOCK == DoorLockType.DELUNS
                        || DoorLockFactory.CURRENT_DOOR_LOCK == DoorLockType.DELUNSV10
                        || DoorLockFactory.CURRENT_DOOR_LOCK == DoorLockType.DIGI ||
                        DoorLockFactory.CURRENT_DOOR_LOCK == DoorLockType.MOLLY)
                    {
                        //SystemMessage.ShowModalInfoMessage("Update vo !!");
                        if (tran != null)
                        {
                            //SystemMessage.ShowModalInfoMessage("tran not null !!");
                            ////update voucher extenstion transaction
                            //SystemMessage.ShowModalInfoMessage("Card No. "+ cardSN);
                            //SystemMessage.ShowModalInfoMessage("Reg id No. " + RegExt.Id);
                            VoucherDTO voucher = UIProcessManager.GetVoucherById(RegExt.Id);


                            if (voucher.Extension1 == null || string.IsNullOrEmpty(voucher.Extension1.Trim()))
                                voucher.Extension1 = cardSN;
                            else
                                voucher.Extension1 += "," + cardSN;


                            //SystemMessage.ShowModalInfoMessage("Updating !!");
                            VoucherDTO flag = UIProcessManager.UpdateVoucher(voucher);

                            //UIProcessManager.DeleteVoucherExtensionTransaction(tran.code);

                            //bool flag = CommonLogics.SaveVoucherExtensionWithTrans(CNETConstantes.REGISTRATION_VOUCHER,
                            //     CNETConstantes.VOUCHER_EXTENTION_DEFINITION_DOOR_LOCK, RegExt.Registration, cardSN, intime.ToString(_doorLock.GetStringFormatOfDate()));
                            if (flag != null)
                            {
                                SystemMessage.ShowModalInfoMessage("Card Re-Issued Successfully!", "MESSAGE");
                                GetCardTransaction();
                            }

                        }
                        else
                        {
                            //    SystemMessage.ShowModalInfoMessage("tran null !!");
                            //    SystemMessage.ShowModalInfoMessage("Card No. " + cardSN);
                            //    SystemMessage.ShowModalInfoMessage("Reg id No. " + RegExt.Id);
                            VoucherDTO voucher = UIProcessManager.GetVoucherById(RegExt.Id);

                            if (voucher.Extension1 == null || string.IsNullOrEmpty(voucher.Extension1.Trim()))
                                voucher.Extension1 = cardSN;
                            else
                                voucher.Extension1 += "," + cardSN;

                            //SystemMessage.ShowModalInfoMessage("Updating !!");
                            VoucherDTO flag = UIProcessManager.UpdateVoucher(voucher);

                            //bool flag = CommonLogics.SaveVoucherExtensionWithTrans(CNETConstantes.REGISTRATION_VOUCHER,
                            //     CNETConstantes.VOUCHER_EXTENTION_DEFINITION_DOOR_LOCK, RegExt.Registration, cardSN, intime.ToString(_doorLock.GetStringFormatOfDate()));
                            if (flag == null)
                            {
                                SystemMessage.ShowModalInfoMessage("Card Issued Successfully!", "MESSAGE");
                                GetCardTransaction();
                            }

                        }
                    }
                    else
                    {

                        // SystemMessage.ShowModalInfoMessage("diff model");

                        if (noAdult == noIssue)
                        {
                            SystemMessage.ShowModalInfoMessage("Card Re-Issued Successfully!", "MESSAGE");
                        }
                        else
                        {
                            string startTime = "";
                            if (DoorLockFactory.CURRENT_DOOR_LOCK == DoorLockType.BETECH || DoorLockFactory.CURRENT_DOOR_LOCK == DoorLockType.BETECHNEW)
                            {
                                startTime = cardSN;
                            }
                            else
                            {
                                startTime = intime.ToString(_doorLock.GetStringFormatOfDate());
                            }
                            if (noAdult > 1 && _addedTranList != null && _addedTranList.Count > 0)
                            {
                                var filtered = _addedTranList.FirstOrDefault(t => t == cardSN.ToString());
                                if (filtered != null)
                                {
                                    SystemMessage.ShowModalInfoMessage("Card Re-Issued Successfully!", "MESSAGE");
                                }
                                else
                                {
                                    VoucherDTO voucher = UIProcessManager.GetVoucherById(RegExt.Id);

                                    if (voucher.Extension1 == null || string.IsNullOrEmpty(voucher.Extension1.Trim()))
                                        voucher.Extension1 = cardSN;
                                    else
                                        voucher.Extension1 += "," + cardSN;

                                    VoucherDTO flag = UIProcessManager.UpdateVoucher(voucher);
                                    //bool flag = CommonLogics.SaveVoucherExtensionWithTrans(CNETConstantes.REGISTRATION_VOUCHER,
                                    // CNETConstantes.VOUCHER_EXTENTION_DEFINITION_DOOR_LOCK, RegExt.Registration, cardSN, startTime);
                                    if (flag == null)
                                    {
                                        SystemMessage.ShowModalInfoMessage("Card Issued Successfully!", "MESSAGE");
                                        GetCardTransaction();
                                    }
                                }
                            }
                            else
                            {
                                VoucherDTO voucher = UIProcessManager.GetVoucherById(RegExt.Id);

                                if (voucher.Extension1 == null || string.IsNullOrEmpty(voucher.Extension1.Trim()))
                                    voucher.Extension1 = cardSN;
                                else
                                    voucher.Extension1 += "," + cardSN;

                                VoucherDTO flag = UIProcessManager.UpdateVoucher(voucher);
                                //bool flag = CommonLogics.SaveVoucherExtensionWithTrans(CNETConstantes.REGISTRATION_VOUCHER,
                                //     CNETConstantes.VOUCHER_EXTENTION_DEFINITION_DOOR_LOCK, RegExt.Registration, cardSN, startTime);
                                if (flag == null)
                                {
                                    SystemMessage.ShowModalInfoMessage("Card Issued Successfully!", "MESSAGE");
                                    GetCardTransaction();
                                }
                            }
                        }
                    }

                    //save Activity
                    ActivityDTO activity = ActivityLogManager.SetupActivity(currentTime.Value, adCode, CNETConstantes.PMS_Pointer, string.Format("Card No: {0}   Lock No: {1}", cardSN, lockNumber));
                    activity.Reference = RegExt.Id;
                    UIProcessManager.CreateActivity(activity);
                }

                ReadCardData();

                ////CNETInfoReporter.Hide();

            }
            catch (Exception ex)
            {
                ////CNETInfoReporter.Hide();
                SystemMessage.ShowModalInfoMessage("Error in issuing door lock. Detail: " + ex.Message, "ERROR");
            }
        }

        private void bbiClear_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                //forbid for active guest card earse
                string _currentCardNo = _doorLock.GetCardSN();
                if (string.IsNullOrEmpty(_currentCardNo))
                {
                    ////CNETInfoReporter.Hide();
                    return;
                }

                var tranList = GetCardTransaction();
                if (tranList != null)
                {
                    var tranToDelete = tranList.FirstOrDefault(t => t == _currentCardNo);
                    if (tranToDelete != null)
                    {
                        XtraMessageBox.Show("There is active transaction with the current card.", "Door Lock", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                        return;

                    }

                }


                DialogResult dr = XtraMessageBox.Show("Do you want to clear card data?", "Door Lock", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dr == System.Windows.Forms.DialogResult.No) return;
                bool status = _doorLock.ClearCard();
                if (status)
                {
                    SystemMessage.ShowModalInfoMessage("Card is cleared successfully!", "MESSAGE");
                }

                ReadCardData();
            }
            catch (Exception ex)
            {
                SystemMessage.ShowModalInfoMessage("Error in clearing card. Detail: " + ex.Message, "ERROR");
            }
        }
        public bool CardReturned { get; set; }
        private void bbiReturnCard_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                PMSDataLogger.LogMessage("DoorLock", "Return card Click.");
                DialogResult dr = XtraMessageBox.Show("Do you want to return card?", "Door Lock", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dr == System.Windows.Forms.DialogResult.No) return;
                // Progress_Reporter.Show_Progress("Returning Card", "Please Wait...");

                //read card data
                ReadCardData();
                string _currentCardNo = _doorLock.GetCardSN();
                PMSDataLogger.LogMessage("DoorLock", "Card No from reader :-"+ _currentCardNo);
                if (string.IsNullOrEmpty(_currentCardNo))
                {
                    ////CNETInfoReporter.Hide();
                    return;
                }
                var tranList = GetCardTransaction();
                if (tranList == null || tranList.Count == 0)
                {
                    ////CNETInfoReporter.Hide();
                    SystemMessage.ShowModalInfoMessage("No Card Issue for this registration!", "ERROR");
                    return;
                }

                PMSDataLogger.LogMessage("DoorLock", "Card No from DB :-" + string.Join(',', tranList));
                string tranToDelete = tranList.FirstOrDefault(t => t == _currentCardNo);
                if (tranToDelete == null)
                {
                    ////CNETInfoReporter.Hide();
                    SystemMessage.ShowModalInfoMessage("The current card number is not found in the transaction!", "ERROR");
                    return;
                }

                //bool flag = UIProcessManager.DeleteVoucherExtensionTransaction(tranToDelete.code);

                tranList.Remove(tranToDelete);
                string? Doorlockvalue = null;
                if (tranList != null && tranList.Count > 0)
                {
                    Doorlockvalue = string.Join(',', tranList);
                }

                PMSDataLogger.LogMessage("DoorLock", "Extension 1 :-" + Doorlockvalue);

                PMSDataLogger.LogMessage("DoorLock", "Voucher Update");
                VoucherDTO updatevoucher = UIProcessManager.GetVoucherById(RegExt.Id);
                updatevoucher.Extension1 = Doorlockvalue;
                VoucherDTO flag = UIProcessManager.UpdateVoucher(updatevoucher);

                if (flag != null)
                {
                    PMSDataLogger.LogMessage("DoorLock", "Voucher Update Success.");
                    bool status = _doorLock.ClearCard();
                    CardReturned = true;
                    ReadCardData();
                    if (status)
                    {
                        SystemMessage.ShowModalInfoMessage("Card is returned successfully!!", "MESSAGE");
                    }
                    else
                    {
                        XtraMessageBox.Show("Card is returned but card data is not cleared!", "Door Lock", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    GetCardTransaction();
                }
                else
                {
                    PMSDataLogger.LogMessage("DoorLock", "Voucher Update Fail.");
                    SystemMessage.ShowModalInfoMessage("Card is not returned!", "ERROR");
                }

                ////CNETInfoReporter.Hide();

            }
            catch (Exception ex)
            {
                ////CNETInfoReporter.Hide();
                SystemMessage.ShowModalInfoMessage("Error in returning card. Detail: " + ex.Message, "ERROR");

            }
        }

        private void bbiReadCard_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ReadCardData();
        }

        private void bbiLost_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                int noIssue = string.IsNullOrEmpty(teNoOfIssue.Text) ? 0 : Convert.ToInt32(teNoOfIssue.Text);
                if (noIssue <= 0)
                {
                    SystemMessage.ShowModalInfoMessage("There is no issued card for this guest!", "ERROR");
                    return;
                }
                DialogResult dr = XtraMessageBox.Show("Do you want to charge a guest for lost card?", "Door Lock", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dr == System.Windows.Forms.DialogResult.No) return;

                //read configuration
                var config = LocalBuffer.LocalBuffer.ConfigurationBufferList.FirstOrDefault(c => c.Reference == CNETConstantes.PMS_Pointer.ToString() && c.Attribute == CNETConstantes.PMS_SETTING_LostCardFeeArticle);
                if (config == null)
                {
                    SystemMessage.ShowModalInfoMessage("Please set Lost-Card article configuration!", "ERROR");
                    return;
                }
                frmManualCharge manualChargeForm = new frmManualCharge();
                manualChargeForm.RegExt = RegExt;
                manualChargeForm.IsLostCardFee = true;
                manualChargeForm.LostCardArticleCode = config.CurrentValue;

                if (manualChargeForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    // Progress_Reporter.Show_Progress("Returning Card", "Please Wait...");
                    VoucherDTO tranList = UIProcessManager.GetVoucherById(RegExt.Id);

                    if (tranList == null)
                    {
                        ////CNETInfoReporter.Hide();
                        SystemMessage.ShowModalInfoMessage("The current card number is not found in the transaction!", "ERROR");
                        return;
                    }
                    tranList.Extension1 = null;
                    VoucherDTO flag = UIProcessManager.UpdateVoucher(tranList);
                    if (flag == null)
                    {

                        SystemMessage.ShowModalInfoMessage("Card is returned successfully!!", "MESSAGE");

                        //GetCardTransaction();
                    }
                    else
                    {
                        SystemMessage.ShowModalInfoMessage("Card is not returned!", "ERROR");
                    }
                }
            }
            catch (Exception ex)
            {
                ////CNETInfoReporter.Hide();
                SystemMessage.ShowModalInfoMessage("Error in handling lost card. Detail:: " + ex.Message, "ERROR");
            }
        }

        private void frmDoorLock_Load(object sender, EventArgs e)
        {
            if (!InitializeData())
            {
                this.Close();
            }
        }

        private void bbiClose_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Close();
        }

        private void bbiCardHolderLabel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                //LabelPrinter.CardHolderLabelPrint chPrint = new LabelPrinter.CardHolderLabelPrint(new LabelPrinter.CardHolderLabelPrint.CardHolderLabelObj()
                //{
                //    GuestName = RegExt.Customer,
                //    RoomNumber = RegExt.Room,
                //    ValidThru = teDepartureDate.Text
                //});

            }
            catch (Exception ex)
            {
                SystemMessage.ShowModalInfoMessage("Error in printing card-holder label. DETAIL: " + ex.Message, "ERROR");
            }
        }


        #endregion






    }
}
