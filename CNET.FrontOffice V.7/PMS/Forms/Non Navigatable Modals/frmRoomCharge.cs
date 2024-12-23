using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using CNET.ERP.Client.Common.UI;
using CNET.FrontOffice_V._7;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraBars;
using CNET_V7_Domain.Misc.PmsDTO;
using ProcessManager;
using CNET.FrontOffice_V._7.PMS.Common_Classes;
using CNET_V7_Domain.Domain.SettingSchema;
using CNET_V7_Domain.Domain.TransactionSchema;
using CNET_V7_Domain.Domain.PmsSchema;

namespace CNET.FrontOffice_V._7.Forms.Non_Navigatable_Modals
{
    public partial class frmRoomCharge : UILogicBase
    {
        private string postType = "";
        private List<DailyRoomChargeDTO> allDailyRoomCharges = new List<DailyRoomChargeDTO>();

        private IList<Control> _invalidControls;

        //  private List<viewFunctWithAccessM> allSecuredfunctions;

        DateTime? CurrentTime = UIProcessManager.GetServiceTime();


        //*************** CONSTRUCTOR ***************************//
        public frmRoomCharge()
        {
            InitializeComponent();



            riteNights.EditValueChanged += riteNights_EditValueChanged;
            //riteNights 
            gvRoomTaxPostingMaster.OptionsBehavior.Editable = false;
            gvRoomTaxPostingDetail.OptionsBehavior.Editable = false;
            gvRoomTaxPostingMaster.RowStyle += GridRowShadeBasedOnState;


        }

        //***************** PROPERTIES *************************/
        #region Properties

        //Create Params
        private const int CP_NOCLOSE_BUTTON = 0x200;
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams myCp = base.CreateParams;
                myCp.ClassStyle = myCp.ClassStyle | CP_NOCLOSE_BUTTON;
                return myCp;
            }
        }

        //Reg.Extension
        private RegistrationListVMDTO regExtension;
        internal RegistrationListVMDTO RegExtension
        {
            get { return regExtension; }
            set
            {
                regExtension = value;
                //rideFrom.MinValue = regExtension.Arrival;
                //ri
                //.MinValue = regExtension.Arrival;

                DateTime? CurrentTime = UIProcessManager.GetServiceTime();
                if (CurrentTime == null)
                {
                    SystemMessage.ShowModalInfoMessage("Unable to get server date and time !", "ERROR");
                    return;
                }

                if (regExtension.preStayCharging && CurrentTime.Value.Date <= value.Arrival.Date && (regExtension.lastState == CNETConstantes.SIX_PM_STATE || regExtension.lastState == CNETConstantes.GAURANTED_STATE))
                {
                    rideFrom.MaxValue = value.Arrival;
                    rideTo.MaxValue = value.Arrival;
                }
                else
                {
                    if (regExtension.postStayCharging && regExtension.lastState == CNETConstantes.CHECKED_OUT_STATE && CurrentTime.Value.Date >= value.Departure.Date)
                    {
                        rideFrom.MaxValue = CurrentTime.Value.Date;
                        rideTo.MaxValue = CurrentTime.Value.Date.AddDays(1);
                    }
                    else
                    {
                        rideFrom.MaxValue = value.Departure;
                        rideTo.MaxValue = value.Departure;
                    }
                }




                rideFrom.MinValue = CurrentTime.Value.Date;
                rideTo.MinValue = CurrentTime.Value.Date;

            }
        }

        #endregion

        //******************* METHODS ****************************/
        #region Methods

        private bool InitializeData()
        {
            try
            {
                // Progress_Reporter.Show_Progress("Loding Data...");

                // allSecuredfunctions = GetAllSecuredFunctions();

                EnableDisableBarItems("Options", bsiOptions);

                if ((!regExtension.preStayCharging && !regExtension.postStayCharging) && (regExtension.lastState == CNETConstantes.SIX_PM_STATE ||
                    regExtension.lastState == CNETConstantes.GAURANTED_STATE ||
                    regExtension.lastState == CNETConstantes.CHECKED_OUT_STATE))
                {
                    SystemMessage.ShowModalInfoMessage("Room Charge Can't Be Done in this state !! ", "ERROR");
                    return false;
                }
                else if (regExtension.preStayCharging && !regExtension.postStayCharging && regExtension.lastState == CNETConstantes.CHECKED_OUT_STATE && CurrentTime.Value.Date > regExtension.Departure)
                {
                    SystemMessage.ShowModalInfoMessage("This Registration Doesn't have a post stay Charge Privllege  !! ", "ERROR");
                    return false;
                }
                else if (regExtension.postStayCharging && !regExtension.preStayCharging && (regExtension.lastState == CNETConstantes.SIX_PM_STATE ||
                    regExtension.lastState == CNETConstantes.GAURANTED_STATE) && CurrentTime.Value.Date <= regExtension.Arrival)
                {
                    SystemMessage.ShowModalInfoMessage("This Registration Doesn't have a pre stay Charge Privllege  !! ", "ERROR");
                    return false;
                }



                if ((regExtension.preStayCharging || regExtension.postStayCharging) &&
                    (regExtension.lastState == CNETConstantes.SIX_PM_STATE ||
                    regExtension.lastState == CNETConstantes.GAURANTED_STATE ||
                    regExtension.lastState == CNETConstantes.CHECKED_OUT_STATE) && regExtension.lastState != CNETConstantes.CHECKED_IN_STATE)
                {
                    bbiEntireStay.Visibility = BarItemVisibility.Never;
                    bbiNights.Visibility = BarItemVisibility.Never;

                }

                ////CNETInfoReporter.Hide();
                return true;

            }
            catch (Exception ex)
            {
                ////CNETInfoReporter.Hide();
                SystemMessage.ShowModalInfoMessage("Error in initializing data. Detail: " + ex.Message, "ERROR");
                return false;
            }
        }

        private void GridRowShadeBasedOnState(object sender, DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs e)
        {
            GridView View = sender as GridView;
            if (e.RowHandle >= 0)
            {
                DailyRoomVoucherVM dto = View.GetRow(e.RowHandle) as DailyRoomVoucherVM;
                if (dto == null) return;

                if (!dto.isCharged)
                {

                    View.Appearance.FocusedRow.BackColor = ColorTranslator.FromHtml("SkyBlue");
                    View.Appearance.FocusedRow.ForeColor = Color.Blue;


                    View.Appearance.FocusedCell.BackColor = ColorTranslator.FromHtml("SkyBlue");
                    View.Appearance.FocusedCell.ForeColor = Color.Blue;

                    View.Appearance.SelectedRow.BackColor = ColorTranslator.FromHtml("SkyBlue");
                    View.Appearance.SelectedRow.ForeColor = Color.Blue;

                    e.Appearance.ForeColor = Color.Blue;



                }
                else
                {
                    View.Appearance.FocusedRow.BackColor = ColorTranslator.FromHtml("SkyBlue");
                    View.Appearance.FocusedRow.ForeColor = Color.Red;


                    View.Appearance.FocusedCell.BackColor = ColorTranslator.FromHtml("SkyBlue");
                    View.Appearance.FocusedCell.ForeColor = Color.Red;

                    View.Appearance.SelectedRow.BackColor = ColorTranslator.FromHtml("SkyBlue");
                    View.Appearance.SelectedRow.ForeColor = Color.Red;

                    e.Appearance.ForeColor = Color.Red;
                }
            }
        }


        private void PopulateRoomAndTax()
        {
            // Progress_Reporter.Show_Progress("Loading room and tax. Please wait...");
            try
            {
                allDailyRoomCharges.Clear();

                BindingList<DailyRoomVoucherVM> data = new BindingList<DailyRoomVoucherVM>();
                DateTime counter = (DateTime)beiFrom.EditValue;
                DateTime toDateTime = (DateTime)beiTo.EditValue;
                counter = counter.Date;
                toDateTime = toDateTime.Date;

                // for day-use
                if (counter == toDateTime)
                {
                    toDateTime = toDateTime.AddDays(1);
                }

                //get applicable tax

                int? accArticle = null;
                var rateCodeHeader = UIProcessManager.GetRateCodeHeaderById(RegExtension.RateCodeHeader.Value);
                if (rateCodeHeader != null)
                {
                    accArticle = rateCodeHeader.Article;
                }
                else
                {
                    SystemMessage.ShowModalInfoMessage("Unable to find registration's rate", "ERROR");
                    return;
                }

                if (accArticle == null)
                {
                    SystemMessage.ShowModalInfoMessage("Unable to find rate's article", "ERROR");
                    return;
                }

                TaxDTO tax = CommonLogics.GetApplicableTax(RegExtension.Id, CNETConstantes.DAILY_ROOM_CHARGE_VOUCHER, RegExtension.GuestId, accArticle.Value);
                if (!string.IsNullOrEmpty(tax.Remark))
                {
                    SystemMessage.ShowModalInfoMessage(tax.Remark, "ERROR");
                    return;
                }

                while (counter < toDateTime)
                {
                    //counter = Convert.ToDateTime("2024-04-05");
                    DailyRoomChargeDTO dailyRoomCharge = UIProcessManager.GetDailyRoomChargePostingByRegistration(RegExtension.Id, counter.Date, CNETConstantes.REGISTRATION_VOUCHER, RegExtension.lastState.Value, tax, null, null);
                    if (dailyRoomCharge == null)
                        break;

                    counter = counter.AddDays(1);
                    allDailyRoomCharges.Add(dailyRoomCharge);
                }

                foreach (DailyRoomChargeDTO dr in allDailyRoomCharges)
                {


                    VoucherFinalDTO voFinal = dr.VoFinal;

                    DailyRoomVoucherVM dto = new DailyRoomVoucherVM();
                    dto.isCharged = dr.IsPosted;
                    dto.registrationId = dr.registrationId;
                    dto.registrationNo = dr.registrationVoucher;
                    dto.roomNo = dr.room;
                    dto.date = dr.dailyRoomChargeVoucher.IssuedDate;
                    dto.consignee = RegExtension.Guest;

                    DateTime? currentDate = UIProcessManager.GetServiceTime();

                    //If the DRC is charged but if there is a change in rate, update the voucher
                    if (dto.isCharged && currentDate != null)
                    {
                        //var todayCharges = UIProcessManager.GetDailyRoomChargePostingByRegistration(dr.registrationVoucher, CNETConstantes.DAILY_ROOM_CHARGE_VOUCHER.ToString(), null);
                        //var todayCharge = UIProcessManager.GetAllRoomCharges(currentDate.Value, currentDate.Value).FirstOrDefault(x => x.regNum == dr.registrationVoucher);
                        var todayCharge = UIProcessManager.GetDailyRoomVoucherByReg(dr.registrationId, currentDate.Value.Date);

                        if (todayCharge != null && todayCharge.Count > 0)
                        {
                            VoucherDTO vo = UIProcessManager.GetVoucherById(todayCharge.FirstOrDefault());
                            if (vo != null)
                            {
                                if (Math.Round(vo.GrandTotal, 2) != voFinal.voucher.GrandTotal)
                                {
                                    vo.IsVoid = true;
                                    vo.LastState = CNETConstantes.OS_VOID;
                                    vo.LastUser = LocalBuffer.LocalBuffer.CurrentLoggedInUser.Id;
                                    vo.LastDevice = LocalBuffer.LocalBuffer.CurrentDevice.Id;

                                    var tranRef = UIProcessManager.GetTransactionReferenceByreferring(vo.Id);
                                    if (tranRef != null && tranRef.Count > 0)
                                    {
                                        bool flag = UIProcessManager.DeleteTransactionReferenceById(tranRef.FirstOrDefault().Id);
                                        if (flag)
                                        {
                                            
                                            UIProcessManager.UpdateVoucher(vo);
                                            dto.isCharged = false;
                                        }
                                    }
                                }
                            }
                        }

                        //if (todayCharge != null && todayCharge.Count >0)
                        //{
                        //    var response = UIProcessManager.GetVoucherBufferById(todayCharge.FirstOrDefault());

                        //    VoucherBuffer vo = response.Data;
                        //    if (vo != null)
                        //    {
                        //        if (Math.Round(vo.Voucher.GrandTotal, 2) != voFinal.voucher.GrandTotal)
                        //        {
                        //            vo.Voucher.IsVoid = true;
                        //            var tranRef = vo.TransactionReferencesBuffer;
                        //            if (tranRef != null && tranRef.Count > 0)
                        //            {
                        //                bool flag = UIProcessManager.DeleteTransactionReferenceById(tranRef.FirstOrDefault().);

                        //                vo.TransactionReferencesBuffer.Remove(tranRef.FirstOrDefault());
                        //                if (vo.TransactionReferencesBuffer != null && vo.TransactionReferencesBuffer.Count > 0)
                        //                    vo.TransactionReferencesBuffer.ToList().ForEach(x => x.ReferencedActivity = null);


                        //                vo.TransactionCurrencyBuffer = null;

                        //                UIProcessManager.UpdateVoucherBuffer(vo);
                        //                dto.isCharged = false;
                        //            }
                        //        }
                        //    }
                        //}


                    }

                    if (voFinal != null)
                    {

                        if (dr.dailyRoomChargeVoucher != null) dto.amount = voFinal.voucher == null ? 0 : Math.Round(voFinal.voucher.GrandTotal, 2);
                    }
                    data.Add(dto);

                    foreach (LineItemDetails itemD in dr.lineItemList)
                    {
                        if (itemD != null)
                        {
                            dto.lineItems.Add(new DailyLineDTO()
                            {
                                articleCode = itemD.lineItems.Article,
                                quantity = itemD.lineItems.Quantity,
                                totalAmunt = itemD.lineItems.TotalAmount != null ? Math.Round(itemD.lineItems.TotalAmount, 2) : 0,

                                Name = itemD.articleName,
                                unitAmount = itemD.lineItems.UnitAmount != null ? Math.Round(itemD.lineItems.UnitAmount, 2) : 0
                            });

                            if (itemD.lineItems.UnitAmount < 0)
                            {
                                SystemMessage.ShowModalInfoMessage("Couldn't Get Room Charge b/c " + itemD.articleName + " Price Can't be less than Zero !!" + Environment.NewLine + "Please Check your rate and package !!", "ERROR");
                                ////CNETInfoReporter.Hide();
                                return;
                            }

                        }

                    }

                }
                gcRoomTaxPosting.DataSource = data;
                gcRoomTaxPosting.RefreshDataSource();

            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Error in room and tax retrival. DETAIL:: " + ex.Message, "CNET_v2016", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            ////CNETInfoReporter.Hide();
        }

        private void EnableDisableBarItems(String catagory, BarSubItem barItem)
        {
            List<String> approvedFunctionalities = LocalBuffer.LocalBuffer.AllAccessMatrixFunctionList.Select(x => x.Description).ToList();

            bool oneTrue = false;
            foreach (BarItemLink itemLink in barItem.ItemLinks)
            {
                if (!IsFunctionExists(approvedFunctionalities, itemLink.DisplayCaption))
                {
                    itemLink.Item.Enabled = false;
                }
                else
                {
                    oneTrue = true;
                }
            }
            if (!oneTrue)
            {
                barItem.Enabled = false;

            }
        }

        private static bool IsFunctionExists(List<string> approvedFunctionalities, string selectedName)
        {
            foreach (String str in approvedFunctionalities)
            {
                if (str.ToLower().Trim().Equals(selectedName.ToLower().Trim()))
                {
                    return true;
                }
            }
            return false;
        }

        //private List<viewFunctWithAccessM> GetAllSecuredFunctions()
        //{
        //    List<viewFunctWithAccessM> retVal = new List<viewFunctWithAccessM>();

        //    try
        //    {
        //        String SubSystemComponent = CNETConstantes.SECURITYRoomCharge;
        //        string currentRole = "";
        //        var role = LocalBuffer.LocalBuffer.UserRoleMapperBufferList.Where(x => x.user == LocalBuffer.LocalBuffer.CurrentLoggedInUser.Id).FirstOrDefault();
        //        if (role != null)
        //            currentRole = role.role;

        //        retVal.AddRange(UIProcessManager.GetFuncwithAccessMatView(currentRole, "Options", SubSystemComponent).Where(x => x.access == true).ToList());

        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //    return retVal;
        //}

        #endregion

        //******************** EVENT HANDLRES **********************/
        #region Event Handlers

        private void riteNights_EditValueChanged(object sender, EventArgs e)
        {
            int noNights = 0;
            if (RegExtension.Registration != null)
            {
                DateTime start = RegExtension.Arrival;
                DateTime end = RegExtension.Departure;
                noNights = (end.Date - start.Date).Days;
            }
            TextEdit edit = sender as TextEdit;
            if (edit != null)
            {
                string value = edit.Text;

                if (!string.IsNullOrEmpty(value))
                {
                    int intValue = Convert.ToInt16(value);
                    if (intValue > noNights)
                    {
                        SystemMessage.ShowModalInfoMessage("You can not charge more than the stay duration of this guest. Please enter not more than " + noNights, "ERROR");
                    }
                    else
                    {
                        DateTime? CurrentTime = UIProcessManager.GetServiceTime();
                        if (CurrentTime == null)
                        {
                            SystemMessage.ShowModalInfoMessage("Unable to get server date and time !", "ERROR");
                            return;
                        }
                        beiTo.EditValue = CurrentTime.Value.AddDays(intValue);
                    }

                }
            }
        }

        private void bbiPost_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DateTime? CurrentTime = UIProcessManager.GetServiceTime();
            if (CurrentTime == null)
            {
                SystemMessage.ShowModalInfoMessage("Unable to get server date and time !", "ERROR");
                return;
            }
            CommonLogics.PostRoomCharge(allDailyRoomCharges, CurrentTime.Value, LocalBuffer.LocalBuffer.CurrentDevice, regExtension.ConsigneeUnit.Value);
            bbiShow.PerformClick();
        }

        private void bbiClose_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Close();
        }

        private void bbiToday_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DateTime? CurrentTime = UIProcessManager.GetServiceTime();
            if (CurrentTime == null)
            {
                SystemMessage.ShowModalInfoMessage("Unable to get server date and time !", "ERROR");
                return;
            }
            gcRoomTaxPosting.DataSource = null;
            gcRoomTaxPosting.RefreshDataSource();
            rpgNoNights.Visible = false;
            postType = "Today";
            beiFrom.EditValue = CurrentTime.Value;
            beiTo.EditValue = CurrentTime.Value.AddDays(1);

        }

        private void bbiEntireStay_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DateTime? CurrentTime = UIProcessManager.GetServiceTime();
            if (CurrentTime == null)
            {
                SystemMessage.ShowModalInfoMessage("Unable to get server date and time !", "ERROR");
                return;
            }
            gcRoomTaxPosting.DataSource = null;
            gcRoomTaxPosting.RefreshDataSource();
            rpgNoNights.Visible = false;
            postType = "Entire";
            beiFrom.EditValue = CurrentTime.Value.Date > RegExtension.Arrival.Date ? CurrentTime.Value : RegExtension.Arrival;
            beiTo.EditValue = RegExtension.Departure.Date;

        }

        private void bbiNights_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DateTime? CurrentTime = UIProcessManager.GetServiceTime();
            if (CurrentTime == null)
            {
                SystemMessage.ShowModalInfoMessage("Unable to get server date and time !", "ERROR");
                return;
            }
            gcRoomTaxPosting.DataSource = null;
            gcRoomTaxPosting.RefreshDataSource();
            rpgNoNights.Visible = true;
            postType = "Nights";
            beiFrom.EditValue = CurrentTime.Value.Date > RegExtension.Arrival.Date ? CurrentTime.Value : RegExtension.Arrival;
            beiTo.EditValue = null;


        }

        private void bbiShow_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (beiFrom.EditValue != null && beiFrom.EditValue != "" && beiTo.EditValue != null && beiTo.EditValue != "")
            {

                if ((regExtension.preStayCharging || regExtension.postStayCharging) && (regExtension.lastState == CNETConstantes.SIX_PM_STATE || regExtension.lastState == CNETConstantes.GAURANTED_STATE || regExtension.lastState == CNETConstantes.CHECKED_OUT_STATE))
                {
                    DateTime Fromdate = (DateTime)beiFrom.EditValue;
                    DateTime toDateTime = (DateTime)beiTo.EditValue;
                    List<RegistrationDetailDTO> RegistrationDetailLIST = UIProcessManager.GetRegistrationDetailByvoucher(regExtension.Id);
                    bool beforeArrival = true;

                    if (CurrentTime.Value.Date < regExtension.Arrival.Date)
                    {
                        beforeArrival = true;
                    }
                    else if (CurrentTime.Value.Date >= regExtension.Departure.Date)
                    {
                        beforeArrival = false;
                    }

                    if (RegistrationDetailLIST != null && RegistrationDetailLIST.Count > 0)
                    {
                        RegistrationDetailDTO FirstDETAILLIST = RegistrationDetailLIST.FirstOrDefault();
                        RegistrationDetailDTO LastDETAILLIST = RegistrationDetailLIST.LastOrDefault();
                        for (var day = Fromdate.Date; day.Date < toDateTime.Date; day = day.AddDays(1))
                        {
                            RegistrationDetailDTO Registrationbydate = RegistrationDetailLIST.FirstOrDefault(x => x.Date == day);

                            if (Registrationbydate == null)
                            {
                                if (beforeArrival)
                                {
                                    if (regExtension.preStayCharging)
                                    {
                                        FirstDETAILLIST.Date = day;
                                        UIProcessManager.CreateRegistrationDetail(FirstDETAILLIST);
                                    }
                                }
                                else
                                {
                                    if (regExtension.postStayCharging)
                                    {
                                        LastDETAILLIST.Date = day;
                                        UIProcessManager.CreateRegistrationDetail(LastDETAILLIST);
                                    }
                                }
                            }
                        }
                    }
                }
                PopulateRoomAndTax();

            }
            else
            {
                SystemMessage.ShowModalInfoMessage("PLease Select both From and To dates", "ERROR");
            }

        }


        #endregion

        private void frmRoomCharge_Load(object sender, EventArgs e)
        {
            if (!InitializeData())
            {
                this.Close();
            }
        }
    }
}