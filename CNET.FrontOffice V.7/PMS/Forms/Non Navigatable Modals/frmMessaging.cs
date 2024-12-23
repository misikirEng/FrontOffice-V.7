
using CNET.ERP.Client.Common.UI;
using CNET.FrontOffice_V._7;
using ProcessManager;
using CNET.FrontOffice_V._7.PMS.Common_Classes;
using CNET.FrontOffice_V._7.Validation;
using CNET_V7_Domain.Domain.CommonSchema;
using CNET_V7_Domain.Domain.SettingSchema;
using CNET_V7_Domain.Domain.TransactionSchema;
using CNET_V7_Domain.Domain.ViewSchema;
using CNET_V7_Domain.Misc.PmsDTO;
using CNET_V7_Domain.Misc.PmsView;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraLayout.Utils;
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
    public partial class frmMessaging : UILogicBase
    {
        private int _activityDef;
        private int? _defMessageType = null;
        private List<RegistrationDocumentDTO> _regDocList = null;
        private List<RegistrationListVMDTO> regListVM = new List<RegistrationListVMDTO>();

        public frmMessaging()
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




        private RegistrationListVMDTO _regExt;
        public RegistrationListVMDTO RegExt
        {
            get
            {
                return _regExt;
            }
            set
            {
                if (value == null) return;
                _regExt = value;


                teRegNum.Text = value.Registration;
                teRoom.Text = value.Room;
                teConsignee.Text = value.Guest;

            }
        }


        public bool IsFromMsgBrowser { get; set; }
        private VwMessageViewDTO _forwardedMessage;
        private int? _adSeen;
        public VwMessageViewDTO ForwardedMessage
        {
            get
            {
                return _forwardedMessage;
            }
            set
            {
                _forwardedMessage = value;
                if (value != null)
                    meMessage.Text = value.Message;

            }
        }

        #endregion

        #region Helper Methods 

        private void InitializeUI()
        {
            //message types
            luMessageTypes.Properties.Columns.Add(new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Description", "Message Type"));
            luMessageTypes.Properties.ValueMember = "Id";
            luMessageTypes.Properties.DisplayMember = "Description";


        }

        private bool InitializeData()
        {
            try
            {
                if (RegExt == null && !IsFromMsgBrowser) return false;
                // Progress_Reporter.Show_Progress("Initializing Messaging Form", "Please Wait...");



                string currentVoCode = UIProcessManager.IdGenerater("Voucher", CNETConstantes.MESSAGE, 0, LocalBuffer.LocalBuffer.CurrentDeviceConsigneeUnit.Value, false, LocalBuffer.LocalBuffer.CurrentDevice.Id);


                if (!string.IsNullOrEmpty(currentVoCode))
                {
                    teMsgNumber.Text = currentVoCode;
                }
                else
                {
                    SystemMessage.ShowModalInfoMessage("There is a problem on id setting! ", "ERROR");
                    ////CNETInfoReporter.Hide();
                    return false;
                }

                DateTime? CurrentTime = UIProcessManager.GetServiceTime();
                if (CurrentTime == null)
                {
                    ////CNETInfoReporter.Hide();
                    return false;
                }

                teDate.Text = CurrentTime.Value.ToShortDateString();
                ceSendFlag.Checked = true;

                //workflow
                ActivityDefinitionDTO workFlow = UIProcessManager.GetActivityDefinitionBydescriptionandreference(CNETConstantes.LU_ACTIVITY_DEFINATION_MESSAGEMADE, CNETConstantes.MESSAGE).FirstOrDefault();

                if (workFlow != null)
                {

                    _activityDef = workFlow.Id;
                }
                else
                {
                    SystemMessage.ShowModalInfoMessage("Please define workflow of MESSAGE MADE for MESSAGE Voucher ", "ERROR");
                    ////CNETInfoReporter.Hide();
                    return false;
                }


                //Check Activity Previlage
                var userRoleMapper = LocalBuffer.LocalBuffer.UserRoleMapperBufferList.FirstOrDefault(r => r.User == LocalBuffer.LocalBuffer.CurrentLoggedInUser.Id);
                if (userRoleMapper != null)
                {
                    var roleActivity = UIProcessManager.GetRoleActivityByactivityDefinition(_activityDef).FirstOrDefault(r => r.Role == userRoleMapper.Role && r.NeedsPassCode);
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

                //check seen and deleted workflow
                ActivityDefinitionDTO workFlowSeen = UIProcessManager.GetActivityDefinitionBydescriptionandreference(CNETConstantes.LU_ACTIVITY_SEEN, CNETConstantes.MESSAGE).FirstOrDefault();

                if (workFlowSeen != null)
                {
                    _adSeen = workFlowSeen.Id;
                }
                else
                {
                    ////CNETInfoReporter.Hide();
                    XtraMessageBox.Show("Please define workflow of SEEN for Message Voucher", "CNET ERPv6", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    return false;
                }

                List<SystemConstantDTO> messageType = LocalBuffer.LocalBuffer.SystemConstantDTOBufferList.Where(l => l.Category == CNETConstantes.MESSAGE_TYPE && l.IsActive).ToList();
                if (messageType != null)
                {
                    luMessageTypes.Properties.DataSource = (messageType.OrderByDescending(c => c.IsDefault).ToList());
                    SystemConstantDTO defMesg = messageType.FirstOrDefault(c => c.IsDefault);
                    if (defMesg != null)
                    {
                        _defMessageType = defMesg.Id;
                    }
                    if (ForwardedMessage != null)
                    {
                        luMessageTypes.EditValue = ForwardedMessage.TypeCode;
                    }
                    else
                    {
                        luMessageTypes.EditValue = _defMessageType;
                    }


                }
                else
                {
                    ////CNETInfoReporter.Hide();
                    SystemMessage.ShowModalInfoMessage("Unable to load message types", "ERROR");
                    return false;
                }

                if (_defMessageType == null)
                    _defMessageType = CNETConstantes.INTERNAL_MESSAGE;

                luMessageTypes.EditValue = _defMessageType;

                //if (_defMessageType != null)
                //    PopulateDestinations(CNETConstantes.INTERNAL_MESSAGE);
                //else
                //     PopulateDestinations(_defMessageType);


                ////CNETInfoReporter.Hide();
                return true;
            }
            catch (Exception ex)
            {
                SystemMessage.ShowModalInfoMessage("Error in initializing messaging form. DETAIL::" + ex.Message, "ERROR");
                ////CNETInfoReporter.Hide();
                return false;
            }
        }

        private void PopulateDestinations(int? messageType)
        {
            try
            {
                if (messageType != null)
                {
                    sLookupEdit_destinations.Properties.DataSource = null;
                    if (messageType == CNETConstantes.GUEST_MESSAGE || messageType == CNETConstantes.WELCOME_MESSAGE)
                    {
                        sLookupEdit_destinations.Properties.View.Columns.Clear();
                        //inhouse-guests
                        GridColumn columnGuest = sLookupEdit_destinations.Properties.View.Columns.AddField("Registration");
                        columnGuest.Visible = true;
                        columnGuest = sLookupEdit_destinations.Properties.View.Columns.AddField("GuestId");
                        columnGuest.Visible = false;
                        columnGuest = sLookupEdit_destinations.Properties.View.Columns.AddField("Guest");
                        columnGuest.Caption = "Guest";
                        columnGuest.Visible = true;
                        columnGuest = sLookupEdit_destinations.Properties.View.Columns.AddField("RoomTypeDescription");
                        columnGuest.Caption = "Room Type";
                        columnGuest.Visible = true;
                        columnGuest = sLookupEdit_destinations.Properties.View.Columns.AddField("Room");
                        columnGuest.Visible = true;

                        sLookupEdit_destinations.Properties.DisplayMember = "Registration";
                        sLookupEdit_destinations.Properties.ValueMember = "GuestId";

                        if (regListVM != null)
                            regListVM.Clear();
                        regListVM = UIProcessManager.GetRegistrationViewModelData(null, null, CNETConstantes.CHECKED_IN_STATE, SelectedHotelcode);


                        sLookupEdit_destinations.Properties.DataSource = regListVM;
                        if (!IsFromMsgBrowser)
                        {
                            sLookupEdit_destinations.EditValue = RegExt.Registration;
                        }
                        else
                        {
                            if (regListVM != null && regListVM.Count > 0)
                                sLookupEdit_destinations.EditValue = regListVM.FirstOrDefault().Registration;
                        }
                    }
                    else if (messageType == CNETConstantes.INTERNAL_MESSAGE)
                    {
                        sLookupEdit_destinations.Properties.View.Columns.Clear();
                        //inhouse-guests
                        GridColumn columnGuest = sLookupEdit_destinations.Properties.View.Columns.AddField("person");
                        columnGuest.Visible = true;
                        columnGuest.Caption = "Id";
                        columnGuest = sLookupEdit_destinations.Properties.View.Columns.AddField("userName");
                        columnGuest.Caption = "User Name";
                        columnGuest.Visible = true;

                        sLookupEdit_destinations.Properties.DisplayMember = "userName";
                        sLookupEdit_destinations.Properties.ValueMember = "person";

                        sLookupEdit_destinations.Properties.DataSource = UIProcessManager.SelectAllUser();
                    }

                }
            }
            catch (Exception ex)
            {
                SystemMessage.ShowModalInfoMessage("Error in populating Destinations Users or Guests. Detail:: " + ex.Message, "ERROR");

            }
        }


        private void Reset()
        {
            meMessage.EditValue = "";


            string currentVoCode = UIProcessManager.IdGenerater("Voucher", CNETConstantes.MESSAGE, 0, LocalBuffer.LocalBuffer.CurrentDeviceConsigneeUnit.Value, false, LocalBuffer.LocalBuffer.CurrentDevice.Id);

            if (!string.IsNullOrEmpty(currentVoCode))
            {
                teMsgNumber.Text = currentVoCode;
            }
            else
            {
                SystemMessage.ShowModalInfoMessage("There is a problem on id setting! ", "ERROR");
                ////CNETInfoReporter.Hide();
            }

            if (_defMessageType == null)
                _defMessageType = CNETConstantes.INTERNAL_MESSAGE;

            luMessageTypes.EditValue = _defMessageType;

            //if (_defMessageType == null)
            //    PopulateDestinations(CNETConstantes.INTERNAL_MESSAGE);
            //else
            //    PopulateDestinations(_defMessageType);

        }

        #endregion

        private void bbiClose_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            teMsgNumber.Text = "Message";
            meMessage.Text = "Message";
            this.Close();
        }

        private void frmMessaging_Load(object sender, EventArgs e)
        {
            if (!InitializeData())
            {
                Close();
            }
        }

        private void bbiSend_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                List<Control> controls = new List<Control>
                {
                    luMessageTypes,
                    teMsgNumber,
                    teRegNum,
                    meMessage
                };

                if (IsFromMsgBrowser)
                {
                    controls.Remove(teRegNum);
                }

                IList<Control> invalidControls = CustomValidationRule.Validate(controls);

                if (invalidControls.Count > 0)
                {
                    ////CNETInfoReporter.Hide();
                    return;

                }

                // Progress_Reporter.Show_Progress("Saving", "Please Wait...");
                VoucherBuffer SavedVoucher = null;
                VoucherBuffer voucherBuffer = new VoucherBuffer();

                string vCode = "";
                vCode = UIProcessManager.IdGenerater("Voucher", CNETConstantes.MESSAGE, 1, LocalBuffer.LocalBuffer.CurrentDeviceConsigneeUnit.Value, false, LocalBuffer.LocalBuffer.CurrentDevice.Id);

                if (string.IsNullOrEmpty(vCode))
                {
                    SystemMessage.ShowModalInfoMessage("There is a problem on id setting!!!", "ERROR");
                    ////CNETInfoReporter.Hide();
                    return;
                }

                DateTime? CurrentTime = UIProcessManager.GetServiceTime();
                if (CurrentTime == null)
                {
                    return;
                }

                if (Convert.ToInt32(luMessageTypes.EditValue) == CNETConstantes.GUEST_MESSAGE || Convert.ToInt32(luMessageTypes.EditValue) == CNETConstantes.WELCOME_MESSAGE)
                {
                    if (sLookupEdit_destinations.EditValue == null || string.IsNullOrEmpty(sLookupEdit_destinations.EditValue.ToString()))
                    {
                        SystemMessage.ShowModalInfoMessage("Please Select a Guest!", "ERROR");
                        ////CNETInfoReporter.Hide();
                        return;
                    }

                    voucherBuffer.Voucher.Consignee1 = RegExt.GuestId;

                }
                else
                {

                    if (!ceSendFlag.Checked && (sLookupEdit_destinations.EditValue == null || string.IsNullOrEmpty(sLookupEdit_destinations.EditValue.ToString())))
                    {
                        SystemMessage.ShowModalInfoMessage("Please Select a User!", "ERROR");
                        ////CNETInfoReporter.Hide();
                        return;
                    }
                    voucherBuffer.Voucher.Consignee1 = ceSendFlag.Checked ? null : Convert.ToInt32(sLookupEdit_destinations.EditValue);
                }

                voucherBuffer.Voucher.Code = vCode;// UIProcessManager.GetCurrentIdByDevice("Voucher", device, CNETConstantes.MESSAGE.ToString(), CNETConstantes.VOUCHER_COMPONENET);
                voucherBuffer.Voucher.Definition = CNETConstantes.MESSAGE;
                voucherBuffer.Voucher.Type = Convert.ToInt32(luMessageTypes.EditValue);
                voucherBuffer.Voucher.IssuedDate = CurrentTime.Value;
                voucherBuffer.Voucher.IsIssued = true;
                voucherBuffer.Voucher.Year = CurrentTime.Value.Year;
                voucherBuffer.Voucher.Month = CurrentTime.Value.Month;
                voucherBuffer.Voucher.Day = CurrentTime.Value.Day;
                voucherBuffer.Voucher.IsVoid = false;
                voucherBuffer.Voucher.Period = LocalBuffer.LocalBuffer.GetPeriodCode(CurrentTime.Value);
                voucherBuffer.Voucher.Remark = "Message Voucher";
                voucherBuffer.Voucher.LastState = CNETConstantes.OSD_UNREAD_STATE;
                voucherBuffer.Voucher.Note = meMessage.EditValue != null ? meMessage.Text : "";

                voucherBuffer.Activity = ActivityLogManager.SetupActivity(CurrentTime.Value, _activityDef, CNETConstantes.PMS_Pointer);

                voucherBuffer.TransactionReferencesBuffer = new List<TransactionReferenceBuffer>();
                if (!IsFromMsgBrowser || RegExt != null)
                {
                    TransactionReferenceBuffer TrBuffer = new TransactionReferenceBuffer();
                    TrBuffer.TransactionReference = new TransactionReferenceDTO()
                    {
                        ReferencedVoucherDefn = CNETConstantes.REGISTRATION_VOUCHER,
                        Referenced = RegExt.Id,
                        ReferencingVoucherDefn = voucherBuffer.Voucher.Definition,
                        Value = 0,
                    };
                    TrBuffer.ReferencedActivity = null;

                    voucherBuffer.TransactionReferencesBuffer.Add(TrBuffer);
                }





                voucherBuffer.Voucher.LastUser = LocalBuffer.LocalBuffer.CurrentLoggedInUser.Id;
                voucherBuffer.Voucher.LastDevice = LocalBuffer.LocalBuffer.CurrentDevice.Id;
                voucherBuffer.TransactionCurrencyBuffer = null;


                var Voucherdata = UIProcessManager.CreateVoucherBuffer(voucherBuffer);



                if (Voucherdata != null && Voucherdata.Success)
                {
                    SavedVoucher = Voucherdata.Data;
                    //save activity Seen
                    ActivityDTO activitySeen = ActivityLogManager.SetupActivity(CurrentTime.Value, _adSeen.Value, CNETConstantes.PMS_Pointer);
                    activitySeen.Reference = SavedVoucher.Voucher.Id;

                    UIProcessManager.CreateActivity(activitySeen);
                    SystemMessage.ShowModalInfoMessage("Message is successfully saved!", "MESSAGE");
                    Reset();

                }
                else
                {
                    SystemMessage.ShowModalInfoMessage("Message is not saved!" + Environment.NewLine + Voucherdata.Message, "ERROR");
                }


                ////CNETInfoReporter.Hide();
            }
            catch (Exception ex)
            {
                SystemMessage.ShowModalInfoMessage("Error in saving message. DETAIL::" + ex.Message, "ERROR");
                ////CNETInfoReporter.Hide();
            }
        }

        private void sLookupEdit_inhouses_EditValueChanged(object sender, EventArgs e)
        {
            SearchLookUpEdit view = sender as SearchLookUpEdit;
            RegistrationListVMDTO selected = regListVM.FirstOrDefault(r => r.Registration == view.EditValue.ToString());
            RegExt = selected;
        }

        private void bbiNew_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Reset();
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
                _regDocList = null;
                regListVM = null;
                ForwardedMessage = null;
                RegExt = null;
            }
            base.Dispose(disposing);
        }

        private void luMessageTypes_EditValueChanged(object sender, EventArgs e)
        {
            LookUpEdit view = sender as LookUpEdit;
            if (view == null || view.EditValue == null || string.IsNullOrEmpty(view.EditValue.ToString())) return;
            if (Convert.ToInt32(view.EditValue) == CNETConstantes.INTERNAL_MESSAGE)
            {
                if (ceSendFlag.Checked)
                {
                    sLookupEdit_destinations.Enabled = false;
                }
                else
                {
                    sLookupEdit_destinations.Enabled = true;
                }
                lc_checkBox.Visibility = LayoutVisibility.Always;
                lci_destination.Text = "Users";
            }
            else if (Convert.ToInt32(view.EditValue) == CNETConstantes.GUEST_MESSAGE || Convert.ToInt32(view.EditValue) == CNETConstantes.WELCOME_MESSAGE)
            {
                sLookupEdit_destinations.Enabled = true;
                lc_checkBox.Visibility = LayoutVisibility.Never;
                lci_destination.Text = "In-House Guests";
            }

            PopulateDestinations(Convert.ToInt32(view.EditValue));
        }

        private void ceSendFlag_CheckedChanged(object sender, EventArgs e)
        {
            if (ceSendFlag.Checked)
            {
                sLookupEdit_destinations.Enabled = false;
            }
            else
            {
                sLookupEdit_destinations.Enabled = true;
            }
        }
        public int SelectedHotelcode { get; set; }
    }
}
