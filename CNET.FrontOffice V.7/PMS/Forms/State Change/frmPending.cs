using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using CNET.FrontOffice_V._7;
using CNET.ERP.Client.Common.UI;
using DevExpress.XtraEditors.Controls;
using CNET.FrontOffice_V._7.Forms.Non_Navigatable_Modals;
using CNET.FrontOffice_V._7.PMS.Common_Classes;
using CNET_V7_Domain.Domain.SettingSchema;
using ProcessManager;
using CNET_V7_Domain.Domain.ViewSchema;
using CNET_V7_Domain.Domain.TransactionSchema;
using CNET_V7_Domain.Domain.PmsSchema;
using CNET_V7_Domain.Domain.CommonSchema;
using CNET.FrontOffice_V._7.Validation;
using CNET_V7_Domain.Misc.PmsDTO;
using CNET.FrontOffice_V._7.PMS.DTO;
using CNET_V7_Domain.Domain.ConsigneeSchema;

namespace CNET.FrontOffice_V._7.Forms.State_Change
{
    public partial class frmPending : UILogicBase
    {

        private int? adCode;
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

        private RegistrationListVMDTO regExtension;
        internal RegistrationListVMDTO RegExtension
        {
            get { return regExtension; }
            set
            {
                regExtension = value;
            }
        }

        /************************* CONSTRUCTOR ******************/
        public frmPending()
        {
            InitializeComponent();
            InitializeUI();
        }


        #region Helper Methods

        private void InitializeUI()
        {
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            lcStatus.Text = @"You are moving the guest to pending room!";

            //pseudo room list
            cacRoom.Properties.Columns.Add(new LookUpColumnInfo("Description", "Room"));
            cacRoom.Properties.Columns.Add(new LookUpColumnInfo("RoomType", "Room Type"));
            cacRoom.Properties.Columns.Add(new LookUpColumnInfo("Space", "Floor"));
            cacRoom.Properties.DisplayMember = "Description";
            cacRoom.Properties.ValueMember = "Id";

            //reason list
            leReason.Properties.Columns.Add(new LookUpColumnInfo("Description", "Reason"));
            leReason.Properties.DisplayMember = "Description";
            leReason.Properties.ValueMember = "Id";
        }

        private bool InitializeData()
        {
            try
            {
                if (RegExtension == null)
                {
                    SystemMessage.ShowModalInfoMessage("Please select a registration!", "ERROR");
                    return false;
                }


                // Progress_Reporter.Show_Progress("Initializing Post Master Form...");

                //check workflow
                ActivityDefinitionDTO workFlow = UIProcessManager.GetActivityDefinitionBydescriptionandreference(CNETConstantes.LU_ACTIVITY_DEFINATION_Pending, CNETConstantes.REGISTRATION_VOUCHER).FirstOrDefault();

                if (workFlow != null)
                {

                    adCode = workFlow.Id;
                }
                else
                {
                    ////CNETInfoReporter.Hide();
                    SystemMessage.ShowModalInfoMessage("Please define workflow of Pending for Registration Voucher ", "ERROR");
                    return false;
                }

                //Check Activity Previlage
                var userRoleMapper = LocalBuffer.LocalBuffer.UserRoleMapperBufferList.FirstOrDefault(r => r.User == LocalBuffer.LocalBuffer.CurrentLoggedInUser.Id);
                if (userRoleMapper != null)
                {
                    var roleActivity = UIProcessManager.GetRoleActivityByactivityDefinition(adCode.Value).FirstOrDefault(r => r.Role == userRoleMapper.Role && r.NeedsPassCode);
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


                teRegNo.Text = RegExtension.Registration;
                int index = RegExtension.Guest.IndexOf("(");
                if (index > 0)
                {
                    teGuest.Text = RegExtension.Guest.Substring(0, index);
                }
                else
                {
                    teGuest.Text = RegExtension.Guest;
                }
                teRoom.Text = RegExtension.Room;
                tePaymentType.Text = RegExtension.PaymentDesc;
                teDeparture.Text = RegExtension.Departure.Date.ToString();


                //get Guest Ledger
                GuestLedgerDTO gLedger = UIProcessManager.GetGuestLedger(RegExtension.Id, RegExtension.Arrival.Date, RegExtension.Departure.Date, RegExtension.Room, null);

                if (gLedger != null)
                {
                    teTotalBill.Text = gLedger.TotalCredit.ToString();
                    tePaid.Text = gLedger.TotalPaid.ToString();
                    teRemaing.Text = gLedger.RemainingBalanceFormated;

                }
                VoucherDTO Voucher = UIProcessManager.GetVoucherById(RegExtension.Id);

                if (Voucher.Consignee3 != null)
                {
                    ConsigneeDTO Agent = LocalBuffer.LocalBuffer.AllCustomerConsigneeViewlist.FirstOrDefault(x => x.Id == Voucher.Consignee3);
                    teAgent.Text = Agent.FirstName;
                }

                if (Voucher.Consignee4 != null)
                {
                    ConsigneeDTO Source = LocalBuffer.LocalBuffer.AllCustomerConsigneeViewlist.FirstOrDefault(x => x.Id == Voucher.Consignee3);
                    teSource.Text = Source.FirstName;
                }

                if (Voucher.Consignee2 != null)
                {
                    ConsigneeDTO Company = LocalBuffer.LocalBuffer.AllCustomerConsigneeViewlist.FirstOrDefault(x => x.Id == Voucher.Consignee2);

                    if (Company != null)
                        teCompany.Text = Company.FirstName;
                }


                List<RoomTypeDTO> allRoomTypes =
                    UIProcessManager.GetRoomTypeByConsigneeUnit(SelectedHotelcode).Where(t => t.IspseudoRoomType.Value).ToList();
                List<RoomDetailDTO> rooms = UIProcessManager.GetUnassignedRoomsByState(RegExtension.Arrival, RegExtension.Departure, CNETConstantes.CHECKED_OUT_STATE).Where(r => allRoomTypes.Select(t => t.Id).Contains(r.RoomType)).ToList(); //PMSUIProcessManager.SelectAllRoomDetail();
                List<RoomDetailVM> pseudorooms = new List<RoomDetailVM>();
                List<SpaceDTO> spacelist = UIProcessManager.SelectAllSpace();

                if (rooms != null)
                {
                    pseudorooms = rooms.Select(x => new RoomDetailVM()
                    {
                        Id = x.Id,
                        Description = x.Description,
                        RoomType = allRoomTypes.FirstOrDefault(s => s.Id == x.RoomType) != null ? allRoomTypes.FirstOrDefault(s => s.Id == x.RoomType).Description : x.RoomType.ToString(),
                        Space = GetRoomFloor(spacelist, x.Id)
                    }).ToList();

                    cacRoom.Properties.DataSource = pseudorooms;

                }

                List<LookupDTO> reasonsToMove = LocalBuffer.LocalBuffer.LookUpBufferList.Where(l => l.Type == CNETConstantes.POST_MASTER_REASON).ToList();
                leReason.Properties.DataSource = (reasonsToMove);
                LookupDTO canLookup = reasonsToMove.FirstOrDefault(c => c.IsDefault);
                if (canLookup != null)
                    leReason.EditValue = (canLookup.Id);

                ////CNETInfoReporter.Hide();

                return true;
            }
            catch (Exception ex)
            {
                ////CNETInfoReporter.Hide();
                SystemMessage.ShowModalInfoMessage("Error in initializing pending form. Detail:: " + ex.Message, "ERROR");
                return false;
            }
        }
        public string GetRoomFloor(List<SpaceDTO> spaceList, int roomspacecode)
        {
            SpaceDTO spc = spaceList.FirstOrDefault(s => s.Id == roomspacecode);
            if (spc != null)
            {
                return spaceList.FirstOrDefault(s => s.Id == spc.ParentId) != null ? spaceList.FirstOrDefault(s => s.Id == spc.ParentId).Description : string.Empty;
            }
            else return string.Empty;
        }
        #endregion

        #region Event Handlers

        private void bbiClose_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Close();
        }

        private void bbPending_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                List<Control> controls = new List<Control>
                {
                   cacRoom
                };
                IList<Control> invalidControls = CustomValidationRule.Validate(controls);

                if (invalidControls.Count > 0)
                    return;

                VoucherDTO voucher = UIProcessManager.GetVoucherById(RegExtension.Id);
                if (voucher == null)
                {
                    SystemMessage.ShowModalInfoMessage("Unable to get voucher!", "ERROR");
                    return;
                }

                //var voExtDoorLock = LocalBuffer.LocalBuffer.VoucherExtensionBufferList.FirstOrDefault(v => v.Type == CNETConstantes.VOUCHER_EXTENTION_DEFINITION_DOOR_LOCK && v.VoucherDefinition == CNETConstantes.REGISTRATION_VOUCHER);
                //if (voExtDoorLock != null)
                //{
                // Progress_Reporter.Show_Progress("Checking door lock card return");
                if (!string.IsNullOrEmpty(voucher.Extension1))
                {
                    ////CNETInfoReporter.Hide();
                    SystemMessage.ShowModalInfoMessage("Please return guest's door lock card first!", "ERROR");
                    return;
                }
                ////CNETInfoReporter.Hide();

                // }

                // Progress_Reporter.Show_Progress("Saving Post Master", "Please Wait...");

                DateTime? currentTime = UIProcessManager.GetServiceTime();
                if (currentTime != null)
                {

                    ActivityDTO activity = ActivityLogManager.SetupActivity(currentTime.Value, adCode.Value, CNETConstantes.PMS_Pointer, "Pending made from another state");
                    activity.Reference = voucher.Id;
                    ActivityDTO SavedActivity = UIProcessManager.CreateActivity(activity);

                    if (SavedActivity == null)
                    {
                        XtraMessageBox.Show("WARNING: activity log for this state change is not saved", "CNET_v2016", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }

                if (cacRoom.EditValue != null)
                {
                    List<RegistrationDetailDTO> regDeatilList = UIProcessManager.GetRegistrationDetailByvoucher(voucher.Id);
                    if (regDeatilList == null)
                    {
                        ////CNETInfoReporter.Hide();
                        SystemMessage.ShowModalInfoMessage("Unable to get registration detail.", "ERROR");
                        return;
                    }

                    regDeatilList = regDeatilList.Where(x => x.Date.Value.Date >= currentTime.Value.Date).ToList();
                    if (regDeatilList == null)
                    {
                        ////CNETInfoReporter.Hide();
                        SystemMessage.ShowModalInfoMessage("Unable to get registration detail.", "ERROR");
                        return;
                    }
                    RoomDetailDTO rt = UIProcessManager.GetRoomDetailById(Convert.ToInt32(cacRoom.EditValue));
                    foreach (RegistrationDetailDTO reg in regDeatilList)
                    {
                        reg.Room = rt.Id;
                        reg.RoomType = rt.RoomType;
                        UIProcessManager.UpdateRegistrationDetail(reg);
                    }
                }

                DialogResult = System.Windows.Forms.DialogResult.OK;
                SystemMessage.ShowModalInfoMessage("Registration is moved to Psuedo Room!", "MESSAGE");


                ////CNETInfoReporter.Hide();
            }
            catch (Exception ex)
            {
                ////CNETInfoReporter.Hide();
                SystemMessage.ShowModalInfoMessage("Error in saving post master. Detail:: " + ex.Message, "ERROR");

            }
        }

        private void frmPending_Load(object sender, EventArgs e)
        {
            if (!InitializeData())
            {
                this.Close();
            }
        }

        private void bbiReturnCard_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmDoorLock frmDoorLock = new frmDoorLock();
            frmDoorLock.RegExt = RegExtension;
            frmDoorLock.ShowDialog();
        }








        #endregion

        private void bbiFolio_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmFolio _frmFolio = new frmFolio();
            _frmFolio.RegistrationExt = RegExtension;

            _frmFolio.ShowDialog(this);
        }

        public static int SelectedHotelcode { get; set; }
    }
}