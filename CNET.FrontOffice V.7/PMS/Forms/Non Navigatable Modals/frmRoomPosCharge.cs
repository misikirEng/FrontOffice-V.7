
using CNET.ERP.Client.Common.UI;
using CNET.FrontOffice_V._7;
using ProcessManager;
using CNET.FrontOffice_V._7.PMS.Common_Classes;
using CNET_V7_Domain.Domain.CommonSchema;
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
    public partial class frmRoomPosCharge : UILogicBase
    {
        public frmRoomPosCharge()
        {
            InitializeComponent();
            InitializeUI();
        }

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
         
        private List<RegistrationListVMDTO> regListVM = new List<RegistrationListVMDTO>();

        private List<VwRoomPoschargeViewDTO> allRoomPosCharges;
         
        private void InitializeUI()
        {
            //inhouse-guests
            GridColumn columnGuest = slukGuest.Properties.View.Columns.AddField("Registration");
            columnGuest.Visible = true;
            columnGuest = slukGuest.Properties.View.Columns.AddField("RoomTypeDescription");
            columnGuest.Caption = "Room Type";
            columnGuest.Visible = true;
            columnGuest = slukGuest.Properties.View.Columns.AddField("Room");
            columnGuest.Visible = true;
            columnGuest = slukGuest.Properties.View.Columns.AddField("Customer");
            columnGuest.Caption = "Guest";
            columnGuest.Visible = true;
        }

        private VoucherValuesData _voucher;
        private int _activityDef;
        private int? _lastState;
        public VoucherValuesData Voucher
        {
            get
            {
                return _voucher;
            }
            set
            {
                _voucher = value;
                if (value == null) return;

                teVoucherCode.Text = value.voucherCode.ToString();
                teSubTotal.Text = String.Format("{0:F2}", Math.Round(value.subTotal == null ? 0 : value.subTotal.Value, 2));
                teTax.Text = String.Format("{0:F2}", Math.Round(value.VATtaxAmount == null ? 0 : value.VATtaxAmount.Value, 2));
                teAdditionalCharge.Text = String.Format("{0:F2}", Math.Round(value.additionalCharge == null ? 0 : value.additionalCharge.Value, 2));
                teDiscount.Text = String.Format("{0:F2}", Math.Round(value.discount == null ? 0 : value.discount.Value, 2));
                teGrandTotal.Text = String.Format("{0:F2}", Math.Round(value.grandTotal == null ? 0 : value.grandTotal.Value, 2)); 
            }
        }

        private bool InitializeData()
        {
            try
            {
                if (Voucher == null) return false;
               // Progress_Reporter.Show_Progress("Initializing Messaging Form", "Please Wait...");

               
                DateTime? CurrentTime = UIProcessManager.GetServiceTime();
                if (CurrentTime == null)
                {
                   ////CNETInfoReporter.Hide();
                    return false;
                }

                

                //workflow
                ActivityDefinitionDTO workFlow = UIProcessManager.GetActivityDefinitionBydescriptionandreference(CNETConstantes.LU_ACTIVITY_DEFINATION_ROOM_POS_CHARGE_MADE,Voucher.voucherDefinition).FirstOrDefault();

                if (workFlow != null)
                {

                    _activityDef = workFlow.Id;
                    _lastState = workFlow.State;
                }
                else
                {
                    SystemMessage.ShowModalInfoMessage("Please define workflow of ROOM POS CHARGE for credit sales and cash sales Voucher ", "ERROR");
                   ////CNETInfoReporter.Hide();
                    return false;
                }

               

                //to check already it is charged
                allRoomPosCharges = UIProcessManager.GetAllRoomPosCharges(CurrentTime.Value, CurrentTime.Value, null);
                if (allRoomPosCharges != null)
                {
                    allRoomPosCharges = allRoomPosCharges.Where(p => p.Referringid == Voucher.voucherCode).ToList();
                }
          
                if (regListVM != null)
                    regListVM.Clear();
                regListVM = UIProcessManager.GetRegistrationViewModelData( null, null,CNETConstantes.CHECKED_IN_STATE, SelectedHotelcode);
                if (regListVM != null)
                {
                    if (allRoomPosCharges != null && allRoomPosCharges.Count > 0)
                    {
                        List<int> chargedRegList = allRoomPosCharges.Select(r => r.Referenced.Value).ToList();
                        regListVM = regListVM.Where(r => r.Departure.Date >= CurrentTime.Value.Date && !chargedRegList.Contains(r.Id)).ToList();
                    }
                    else
                    {
                        regListVM = regListVM.Where(r => r.Departure.Date >= CurrentTime.Value.Date).ToList();
                    }
                    
                }

                slukGuest.Properties.DataSource = regListVM;

                

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

  
        private void slukGuest_EditValueChanged(object sender, EventArgs e)
        {
            SearchLookUpEdit view = sender as SearchLookUpEdit;
            if (regListVM == null || view.EditValue == null) return;
            RegistrationListVMDTO model = regListVM.FirstOrDefault(m => m.Registration == view.EditValue.ToString());
            if (model == null) return;

            teRegistration.Text = model.Registration;
            teRoomType.Text = model.RoomTypeDescription;
            teRoom.Text = model.Room;
        }

        private void bbiClose_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Close();
        }

        private void frmRoomPosCharge_Load(object sender, EventArgs e)
        {
            if (!InitializeData())
            {
                try
                {
                    this.Close();
                }
                catch (Exception ex)
                {

                }
            }
        }

        private void bbiOk_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (slukGuest.EditValue == null)
                {
                    SystemMessage.ShowModalInfoMessage("Please, select guest first!", "ERROR");
                    return;
                }

                if ( Voucher.voucherCode == 0) return;

               // Progress_Reporter.Show_Progress("Saving Room POS Charge...");
                TransactionReferenceDTO tRef = new TransactionReferenceDTO()
                {
                    ReferencedVoucherDefn= CNETConstantes.REGISTRATION_VOUCHER,
                    Referenced = Convert.ToInt32( slukGuest.EditValue),
                    ReferencingVoucherDefn = Voucher.voucherDefinition,
                    Referring = Voucher.voucherCode,
                    Value = Voucher.grandTotal
                };

                if (UIProcessManager.CreateTransactionReference(tRef) != null)
                {
                    //Save Activity
                    DateTime? CurrentTime = UIProcessManager.GetServiceTime();
                    if (CurrentTime != null)
                    {
                        var response = UIProcessManager.GetVoucherBufferById(tRef.Referring.Value); 
                        VoucherBuffer voucherBuffer = response.Data;

                        voucherBuffer.Activity = ActivityLogManager.SetupActivity( CurrentTime.Value, _activityDef , CNETConstantes.PMS_Pointer, "Room POS Charge Made from Document Browser");
                        // activity.Reference  = tRef.Referring.Value;
                        if (voucherBuffer.TransactionReferencesBuffer != null && voucherBuffer.TransactionReferencesBuffer.Count > 0)
                            voucherBuffer.TransactionReferencesBuffer.ToList().ForEach(x => x.ReferencedActivity = null);



                        voucherBuffer.TransactionCurrencyBuffer = null;


                        UIProcessManager.UpdateVoucherBuffer(voucherBuffer);
                      
                        
                        //string activityCode =  ActivityLogManager.CommitActivity(activity, _activityDef, CNETConstantes.REGISTRATION_VOUCHER, "Room POS Charge Made from Document Browser", CNETConstantes.PMS);
                        //if (!string.IsNullOrWhiteSpace(activityCode))
                        //{
                        //    Voucher vo = UIProcessManager.SelectVoucher(tRef.referening);
                        //    if (vo != null)
                        //    {
                        //        vo.lastActivity = activityCode;
                        //        vo.LastObjectState = _lastState;
                        //        UIProcessManager.UpdateVoucher(vo);
                        //    }
                        //}
                    }

                    XtraMessageBox.Show("Room POS Charge is Saved!", "CNET_v2016", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }


               ////CNETInfoReporter.Hide();
            }
            catch (Exception ex)
            {
                SystemMessage.ShowModalInfoMessage("Error in saving room pos charge. DETAIL::" + ex.Message, "ERROR");
               ////CNETInfoReporter.Hide();
            }
        }

        public int SelectedHotelcode { get; set; }
    }
}
