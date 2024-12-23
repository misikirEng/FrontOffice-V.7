
using CNET.ERP.Client.Common.UI;
using CNET.FrontOffice_V._7;
using ProcessManager;
using CNET.FrontOffice_V._7.PMS.Common_Classes;
using CNET_V7_Domain.Domain.CommonSchema;
using CNET_V7_Domain.Domain.SecuritySchema;
using CNET_V7_Domain.Domain.SettingSchema;
using CNET_V7_Domain.Domain.TransactionSchema;
using CNET_V7_Domain.Misc.PmsDTO;
using CNET_V7_Domain.Misc.PmsView;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Columns;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CNET.FrontOffice_V._7.Forms.Non_Navigatable_Modals
{
    public partial class frmTransferPosCharge : UILogicBase
    {


        private List<RegistrationDocumentDTO> _regDocList = null;
        private List<RegistrationListVMDTO> regListVM = new List<RegistrationListVMDTO>();

        private int activityDefCode = 0;
        private int? lastState = 0;

        /********* Constructor ***********/
        public frmTransferPosCharge()
        {
            InitializeComponent();
            InitializeUI();
        }

        #region Properties

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

        public UserDTO CurrentUser { get; set; }
        public DeviceDTO CurrentDevice { get; set; }

        private VwRoomPoschargeViewDTO _roomPosCharge;
        public VwRoomPoschargeViewDTO RoomPosCharge
        {
            get
            {
                return _roomPosCharge;
            }
            set
            {
                _roomPosCharge = value;
                if (value == null) return;

                teGuest.Text = value.Name;
                teRegNumFrom.Text = value.ReferringCode;
                teChargeNum.Text = value.ReferencedCode;
                teRoomTypeFrom.Text = value.RoomType;
                teAmount.Text = value.Value.ToString();
                teRoomFrom.Text = value.Room;
            }
        }

        #endregion



        #region Helper Methods

        //Initialize UI
        public void InitializeUI()
        {
            //inhouse-guests
            GridColumn columnGuest = slukGuest.Properties.View.Columns.AddField("Registration");
            columnGuest.Visible = true;
            columnGuest = slukGuest.Properties.View.Columns.AddField("RoomTypeDescription");
            columnGuest.Caption = "Room Type";
            columnGuest.Visible = true;
            columnGuest = slukGuest.Properties.View.Columns.AddField("Room");
            columnGuest.Visible = true;
            columnGuest = slukGuest.Properties.View.Columns.AddField("Guest");
            columnGuest.Caption = "Guest";
            columnGuest.Visible = true;

        }

        //Initialize Data
        private bool InitializeData()
        {
            try
            {
                if (RoomPosCharge == null) return false;

                // Progress_Reporter.Show_Progress("Initializing room pos charge...");

                CurrentUser = LocalBuffer.LocalBuffer.CurrentLoggedInUser;
                CurrentDevice = LocalBuffer.LocalBuffer.CurrentDevice;

                DateTime? CurrentTime = UIProcessManager.GetServiceTime();
                if (CurrentTime == null)
                {
                    ////CNETInfoReporter.Hide();
                    return false;
                }

                //load in-house guests
                //if (_regDocList != null)
                //    _regDocList.Clear();
                //_regDocList = MasterPageForm.RegistrationDataChange(UIProcessManager.GetRegistrationDocList(CNETConstantes.CHECKED_IN_STATE, null, null, SelectedHotelcode));
                //if (_regDocList != null)
                //{
                //    var filterd = _regDocList.Where(r => r.departureDate.Date >= CurrentTime.Value.Date && r.code != RoomPosCharge.referenced).ToList();
                //    PopulateRegListVM(filterd);


                //}
                regListVM.Clear();
                regListVM = UIProcessManager.GetRegistrationViewModelData(null, null, CNETConstantes.CHECKED_IN_STATE, SelectedHotelcode);
                slukGuest.Properties.DataSource = regListVM;

                //check workflow

                ActivityDefinitionDTO workFlow = UIProcessManager.GetActivityDefinitionBydescriptionandreference(CNETConstantes.LU_ACTIVITY_DEFINATION_ROOM_POS_CHARGE_TRANSFERED, RoomPosCharge.Definition.Value).FirstOrDefault();

                if (workFlow != null)
                {

                    activityDefCode = workFlow.Id;
                    lastState = workFlow.State;
                }
                else
                {
                    ////CNETInfoReporter.Hide();
                    SystemMessage.ShowModalInfoMessage("Please define workflow of ROOM POS CHARGE TRANSFERED for Credit Sales and Cash Sales Voucher ", "ERROR");
                    return false;
                }


                //Check Activity Previlage
                var userRoleMapper = LocalBuffer.LocalBuffer.UserRoleMapperBufferList.FirstOrDefault(r => r.User == LocalBuffer.LocalBuffer.CurrentLoggedInUser.Id);
                if (userRoleMapper != null)
                {
                    var roleActivity = UIProcessManager.GetRoleActivityByactivityDefinition(activityDefCode).FirstOrDefault(r => r.Role == userRoleMapper.Role && r.NeedsPassCode);
                    if (roleActivity != null)
                    {
                        frmNeedPassword frmNeedPass = new frmNeedPassword(true);

                        frmNeedPass.ShowDialog();
                        if (!frmNeedPass.IsAutenticated)
                        {
                            ////CNETInfoReporter.Hide();
                            return false;
                        }

                    }

                }


                ////CNETInfoReporter.Hide();
                return true;
            }
            catch (Exception ex)
            {
                ////CNETInfoReporter.Hide();
                XtraMessageBox.Show("Error in initializing room pos charge transfer. DETAIL::" + ex.Message, "CNET_v2016", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
        /*
                private void PopulateRegListVM(List<RegistrationDocumentDTO> registrationDocumentBrowser)
                {
                    regListVM.Clear();

                    foreach (RegistrationDocumentDTO regDocBro in registrationDocumentBrowser)
                    {
                        //string customer = "";
                        var rd = new RegistrationListVM();
                        rd.Registration = regDocBro.code;
                        rd.Consignee = regDocBro.consignee;
                        rd.RoomType = regDocBro.roomType;
                        rd.RoomTypeDescription = regDocBro.RoomTypeDescription;
                        rd.Market = regDocBro.Market;
                        rd.Color = regDocBro.color;
                        // rd.RegExtCode = regDocBro.RegistrationExtension;
                        if (regDocBro.arrivalDate != null)
                        {
                            rd.RegistrationDate = regDocBro.arrivalDate;
                            if (regDocBro.roomCount != null)
                            {
                                rd.NoOfRoom = regDocBro.roomCount;
                            }
                            //if (!string.IsNullOrEmpty(regDocBro.resType))
                            //{
                            //    rd.ResType = UIProcessManager.SelectLookup(regDocBro.resType).description;
                            //}
                            if (regDocBro.roomCount > 1)
                            {
                                rd.Room = "#" + regDocBro.roomCount;
                            }
                            else
                            {
                                rd.Room = regDocBro.RoomNumber;
                            }

                            rd.Customer = regDocBro.name;
                            if (regDocBro.tradeName != null)
                            {
                                rd.Company = regDocBro.requiredGSL == CNETConstantes.REQ_GSL_COMPANY ? regDocBro.tradeName : "";
                            }
                            else
                            {
                                rd. = regDocBro.tradeName;
                            }
                            if (regDocBro.arrivalDate != null)
                            {
                                rd.Arrival = Convert.ToDateTime(regDocBro.arrivalDate);
                            }
                        }
                        if (regDocBro.departureDate != null)
                        {
                            rd.Departure = Convert.ToDateTime(regDocBro.departureDate);
                        }
                        rd.Payment = regDocBro.PaymentMethod;
                        rd.lastState = regDocBro.foStatus;
                        if (!regListVM.Contains(rd))
                            regListVM.Add(rd);
                    }
                }
        */

        #endregion

        #region Event Handlers

        private void bbiTransfer_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (slukGuest.EditValue == null)
            {
                XtraMessageBox.Show("You must select a guest first!", "CNET_v2016", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                VwRoomPoschargeViewDTO model = RoomPosCharge;
                if (model == null) return;

                // Progress_Reporter.Show_Progress("Transfering room pos charge..");
                List<TransactionReferenceDTO> traRefList = UIProcessManager.GetTransactionReferenceByreferenced(model.Referenced.Value);
                if (traRefList != null)
                {
                    TransactionReferenceDTO tranRef = traRefList.FirstOrDefault(m => m.Referring == model.Referring.Value);
                    if (tranRef != null)
                    {
                        tranRef.Referenced = Convert.ToInt32(slukGuest.EditValue);
                        if (UIProcessManager.UpdateTransactionReference(tranRef) != null)
                        {
                            //saving activity;
                            DateTime? CurrentTime = UIProcessManager.GetServiceTime();
                            if (CurrentTime != null)
                            {

                                ActivityDTO activity = ActivityLogManager.SetupActivity(CurrentTime.Value, activityDefCode, CNETConstantes.PMS_Pointer);
                                activity.Reference = tranRef.Referring.Value;
                                ActivityDTO savedacti = UIProcessManager.CreateActivity(activity);
                                if (savedacti != null)
                                {
                                    VoucherDTO voo = UIProcessManager.GetVoucherById(tranRef.Referring.Value);

                                    voo.LastActivity = savedacti.Id;
                                    voo.LastState = lastState.Value;
                                    if (voo != null)
                                    {
                                        UIProcessManager.UpdateVoucher(voo);
                                    }
                                }
                            }

                            ////CNETInfoReporter.Hide();
                            XtraMessageBox.Show("Room POS Charge is Transfered!", "CNET_v2016", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            DialogResult = System.Windows.Forms.DialogResult.OK;
                            this.Close();

                        }
                        else
                        {
                            ////CNETInfoReporter.Hide();
                            XtraMessageBox.Show("Room POS Charge is not Transfered!", "CNET_v2016", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        ////CNETInfoReporter.Hide();
                        XtraMessageBox.Show("Unable to get transaction reference record!", "CNET_v2016", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }


                }
                else
                {
                    ////CNETInfoReporter.Hide();
                    XtraMessageBox.Show("Unable to get transaction reference record!", "CNET_v2016", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }


            }
            catch (Exception ex)
            {
                ////CNETInfoReporter.Hide();
                XtraMessageBox.Show("Error in transfering room pos charge. DETAIL::" + ex.Message, "CNET_v2016", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }

        }

        private void bbiClose_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Close();
        }

        private void frmTransferPosCharge_Load(object sender, EventArgs e)
        {
            if (!InitializeData())
            {
                this.Close();
            }
        }

        private void slukGuest_EditValueChanged(object sender, EventArgs e)
        {
            SearchLookUpEdit view = sender as SearchLookUpEdit;
            if (regListVM == null || view.EditValue == null) return;
            RegistrationListVMDTO model = regListVM.FirstOrDefault(m => m.Registration == view.EditValue.ToString());
            if (model == null) return;

            teRegNumTo.Text = model.Registration;
            teRoomTypeTo.Text = model.RoomTypeDescription;
            teRoomTo.Text = model.Room;
        }

        #endregion



        public int SelectedHotelcode { get; set; }
    }
}
