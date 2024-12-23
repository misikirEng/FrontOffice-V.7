
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
using DevExpress.DataProcessing.InMemoryDataProcessor;
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
using CNET_V7_Domain.Misc;

namespace CNET.FrontOffice_V._7.Forms.Non_Navigatable_Modals
{
    public partial class frmServiceRequest : XtraForm
    {

        public RegistrationListVMDTO RegistrationEX { get; set; }

        private int? _adPrepared = null;
        private int? _adSerRequested = null;


        /**************** CONSTRUCTOR ****************/
        public frmServiceRequest()
        {
            InitializeComponent();
            InitializeUI();
        }



        #region Helper Methods

        private void InitializeUI()
        {
            // State
            GridColumn colState = sluState.Properties.View.Columns.AddField("Id");
            colState.Visible = true;
            colState = sluState.Properties.View.Columns.AddField("Description");
            colState.Visible = true;
            colState.Caption = "State";
            sluState.Properties.DisplayMember = "Description";
            sluState.Properties.ValueMember = "Id";

            // Assigned Person
            GridColumn colAssignPer = sluAssignedPer.Properties.View.Columns.AddField("Code");
            colAssignPer.Visible = true;
            colAssignPer.Caption = "Code";
            colAssignPer = sluAssignedPer.Properties.View.Columns.AddField("FirstName");
            colAssignPer.Visible = true;
            colAssignPer.Caption = "First Name";
            colAssignPer = sluAssignedPer.Properties.View.Columns.AddField("FirstName");
            colAssignPer.Visible = true;
            colAssignPer.Caption = "Middle Name";
            //colAssignPer = sluAssignedPer.Properties.View.Columns.AddField("branchDescription");
            //colAssignPer.Visible = false;
            //colAssignPer.Caption = "Branch";
            //colAssignPer = sluAssignedPer.Properties.View.Columns.AddField("department");
            //colAssignPer.Visible = false;
            //colAssignPer.Caption = "Department";
            sluAssignedPer.Properties.DisplayMember = "FirstName";
            sluAssignedPer.Properties.ValueMember = "Id";
        }

        private bool InitializeData()
        {
            try
            {
                // Progress_Reporter.Show_Progress("Loading Data", "Please Wait...");

                //Get Date and Time
                DateTime? CurrentTime = UIProcessManager.GetServiceTime();
                if (CurrentTime == null)
                {
                    ////CNETInfoReporter.Hide();
                    return false;

                }
                else
                {
                    teDate.Value = CurrentTime.Value;
                }

                //Populate Reg Fields
                if (RegistrationEX != null)
                {
                    teRegistration.EditValue = RegistrationEX.Registration;
                    teRoom.EditValue = RegistrationEX.Room;
                    teGuest.EditValue = RegistrationEX.Guest;
                }


                //Generate ID

                string currentVoCode = UIProcessManager.IdGenerater("Voucher", CNETConstantes.SERVICE_REQUEST_VOUCHER, 0, LocalBuffer.LocalBuffer.CurrentDeviceConsigneeUnit.Value, false, LocalBuffer.LocalBuffer.CurrentDevice.Id);

                if (!string.IsNullOrEmpty(currentVoCode))
                {
                    teNumber.Text = currentVoCode;
                }
                else
                {
                    SystemMessage.ShowModalInfoMessage("There is a problem on id setting! ", "ERROR");
                    ////CNETInfoReporter.Hide();
                    return false;
                }

                //workflow service Requested
                ActivityDefinitionDTO workServRequested = UIProcessManager.GetActivityDefinitionBydescriptionandreference(CNETConstantes.LU_ACTIVITY_SERVICE_REQUESTED, CNETConstantes.REGISTRATION_VOUCHER).FirstOrDefault();

                if (workServRequested != null)
                {

                    _adSerRequested = workServRequested.Id;
                }
                else
                {
                    SystemMessage.ShowModalInfoMessage("Please define workflow of Service Requested for REGISTRATION Voucher ", "ERROR");
                    ////CNETInfoReporter.Hide();
                    return false;
                }

                //workflow Prepared
                ActivityDefinitionDTO workFlow = UIProcessManager.GetActivityDefinitionBydescriptionandreference(CNETConstantes.LU_ACTIVITY_DEFINATION_PREPARED, CNETConstantes.SERVICE_REQUEST_VOUCHER).FirstOrDefault();

                if (workFlow != null)
                    _adPrepared = workFlow.Id;
                else
                {
                    SystemMessage.ShowModalInfoMessage("Please define workflow of PREPARED for SERVICE REQUEST Voucher ", "ERROR");
                    ////CNETInfoReporter.Hide();
                    return false;
                }

                //Check Activity Previlage
                var userRoleMapper = LocalBuffer.LocalBuffer.UserRoleMapperBufferList.FirstOrDefault(r => r.User == LocalBuffer.LocalBuffer.CurrentLoggedInUser.Id);
                if (userRoleMapper != null)
                {
                    var roleActivity = UIProcessManager.GetRoleActivityByactivityDefinition(_adPrepared.Value).FirstOrDefault(r => r.Role == userRoleMapper.Role && r.NeedsPassCode);
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


                //Populate OSD
                List<SystemConstantDTO> osdList = LocalBuffer.LocalBuffer.ObjectStateDefinitionBufferList;
                sluState.Properties.DataSource = osdList;

                //Populate Service Lookups
                List<LookupDTO> services = LocalBuffer.LocalBuffer.LookUpBufferList.Where(l => l.Type == CNETConstantes.SERVICE_REQUEST).ToList();
                if (services != null)
                {
                    List<ServReqVM> dtoList = new List<ServReqVM>();
                    foreach (var ser in services)
                    {
                        ServReqVM dto = new ServReqVM()
                        {
                            Id = ser.Id,
                            Description = ser.Description,
                            IsSelected = false,
                            TargetTime = ser.Value
                        };

                        dtoList.Add(dto);
                    }

                    gcServices.DataSource = dtoList;
                    gvServices.RefreshData();
                }


                //Populate Employee
                List<VwConsigneeViewDTO> allEmployees = UIProcessManager.GetConsigneeViewByGslType(CNETConstantes.Employee);
                sluAssignedPer.Properties.DataSource = allEmployees;

                ////CNETInfoReporter.Hide();
                return true;
            }
            catch (Exception ex)
            {
                ////CNETInfoReporter.Hide();
                SystemMessage.ShowModalInfoMessage("Error in initializing data. DETAIL:: " + ex.Message, "ERROR");
                return false;
            }
        }

        #endregion

        #region Event Handlers


        private void bbiClose_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Close();
        }

        private void frmServiceRequest_Load(object sender, EventArgs e)
        {
            if (!InitializeData())
            {
                this.Close();
            }
        }

        private void bbiNew_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            sluState.EditValue = null;
            sluAssignedPer.EditValue = null;
            List<ServReqVM> dtoList = gvServices.DataSource as List<ServReqVM>;
            if (dtoList != null && dtoList.Count > 0)
            {
                foreach (var dto in dtoList)
                {
                    dto.IsSelected = false;
                }

                gcServices.DataSource = dtoList;
                gvServices.RefreshData();
            }
        }

        private void bbiSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {

                List<Control> controls = new List<Control>
                {
                    sluState,
                    teNumber
                };

                IList<Control> invalidControls = CustomValidationRule.Validate(controls);

                if (invalidControls.Count > 0)
                {
                    ////CNETInfoReporter.Hide();
                    return;

                }

                //check atleast one service is selected
                List<ServReqVM> dtoList = gvServices.DataSource as List<ServReqVM>;
                if (dtoList == null || dtoList.Count == 0)
                {
                    SystemMessage.ShowModalInfoMessage("No Service is selected!", "ERROR");
                    return;
                }

                List<ServReqVM> selectedServices = dtoList.Where(s => s.IsSelected).ToList();
                if (selectedServices == null || selectedServices.Count == 0)
                {
                    SystemMessage.ShowModalInfoMessage("No Service is selected!", "ERROR");
                    return;
                }


                // Progress_Reporter.Show_Progress("Saving Service Request", "Please Wait...");

                DateTime? CurrentTime = UIProcessManager.GetServiceTime();
                if (CurrentTime == null)
                {
                    //   SystemMessage.ShowModalInfoMessage("Select date time!", "ERROR");
                    ////CNETInfoReporter.Hide();
                    return;
                }
                DateTime? TargetTime = teDate.Value;
                if (TargetTime == null)
                {
                    SystemMessage.ShowModalInfoMessage("Select date time!", "ERROR");
                    ////CNETInfoReporter.Hide();
                    return;
                }

                string generatedLocalcode = "";
                VoucherBuffer voucherBuffer = new VoucherBuffer();

                string currentVoCode = UIProcessManager.IdGenerater("Voucher", CNETConstantes.SERVICE_REQUEST_VOUCHER, 1, LocalBuffer.LocalBuffer.CurrentDeviceConsigneeUnit.Value, false, LocalBuffer.LocalBuffer.CurrentDevice.Id);

                if (!string.IsNullOrEmpty(currentVoCode))
                {
                    generatedLocalcode = currentVoCode;
                }
                else
                {
                    SystemMessage.ShowModalInfoMessage("There is a problem on id setting!!!", "ERROR");
                    ////CNETInfoReporter.Hide();
                    return;
                }
                voucherBuffer.Voucher = new VoucherDTO()
                {
                    Code = generatedLocalcode,
                    Definition = CNETConstantes.SERVICE_REQUEST_VOUCHER,
                    Consignee1 = sluAssignedPer.EditValue == null ? null : Convert.ToInt32(sluAssignedPer.EditValue),
                    Type = CNETConstantes.TRANSACTIONTYPENORMALTXN,
                    IssuedDate = TargetTime.Value,
                    IsIssued = true,
                    Year = TargetTime.Value.Year,
                    Month = TargetTime.Value.Month,
                    Day = TargetTime.Value.Day,
                    IsVoid = false,
                    Period = LocalBuffer.LocalBuffer.GetPeriodCode(CurrentTime.Value),
                    Remark = teRemark.EditValue == null ? "" : teRemark.EditValue.ToString(),
                    Note = memoNote.EditValue == null ? "" : memoNote.EditValue.ToString(),
                    LastState = Convert.ToInt32(sluState.EditValue),
                    LastUser = LocalBuffer.LocalBuffer.CurrentLoggedInUser.Id,
                    LastDevice = LocalBuffer.LocalBuffer.CurrentDevice.Id


                };
                voucherBuffer.TransactionReferencesBuffer = new List<TransactionReferenceBuffer>();
                TransactionReferenceBuffer TRBuffer = new TransactionReferenceBuffer();

                TRBuffer.TransactionReference = new TransactionReferenceDTO()
                {
                    Referenced = RegistrationEX.Id,
                    ReferencedVoucherDefn = CNETConstantes.REGISTRATION_VOUCHER,
                    ReferencingVoucherDefn = CNETConstantes.SERVICE_REQUEST_VOUCHER,
                    RelationType = CNETConstantes.VOUCHER_RELATION_TYPE_NES_REF
                };
                TRBuffer.ReferencedActivity = null;
                voucherBuffer.TransactionReferencesBuffer.Add(TRBuffer);

                voucherBuffer.VoucherLookupLists = selectedServices == null ? new List<VoucherLookupListDTO>() :
                    selectedServices.Select(x => new VoucherLookupListDTO() { SelectedLookup = x.Id }).ToList();
                voucherBuffer.Activity = ActivityLogManager.SetupActivity(CurrentTime.Value, _adPrepared.Value, CNETConstantes.PMS_Pointer);
                voucherBuffer.TransactionCurrencyBuffer = null;

                ResponseModel<VoucherBuffer> isSaved = UIProcessManager.CreateVoucherBuffer(voucherBuffer);
                if (isSaved != null && isSaved.Success)
                {
                    //Save Transaction Reference
                    //Save Voucher Note
                    //Save Voucher Lookup 

                    //Save Activity for the Registration
                    if (!ceNotGuest.Checked)
                    {
                        //Save ActivityRegistrationEX.Registration,
                        ActivityDTO actReg = ActivityLogManager.SetupActivity(CurrentTime.Value, _adSerRequested.Value, CNETConstantes.PMS_Pointer);
                        actReg.Reference = RegistrationEX.Id;
                        UIProcessManager.CreateActivity(actReg);
                        //ActivityLogManager.CommitActivity(actReg, _adSerRequested, CNETConstantes.REGISTRATION_VOUCHER, voucher.code, voucher.component);

                    }

                    ////CNETInfoReporter.Hide();
                    SystemMessage.ShowModalInfoMessage("Service Request is Saved!", "MESSAGE");
                    this.Close();
                }
                else
                {
                    ////CNETInfoReporter.Hide();
                    SystemMessage.ShowModalInfoMessage("Service is not saved!" + Environment.NewLine + isSaved.Message, "ERROR");
                    return;
                }


                ////CNETInfoReporter.Hide();

                // MasterPageForm.LoadServiceRequestBuffer();

            }
            catch (Exception ex)
            {
                ////CNETInfoReporter.Hide();
                SystemMessage.ShowModalInfoMessage("Error in Saving Service Request. DETAIL:: " + ex.Message, "ERROR");
            }
        }

        private void ceNotGuest_CheckedChanged(object sender, EventArgs e)
        {
            CheckEdit view = sender as CheckEdit;
            if (view.Checked)
            {
                teRegistration.EditValue = null;
                teRoom.EditValue = null;
                teGuest.EditValue = null;
            }
            else
            {
                //Populate Reg Fields
                if (RegistrationEX != null)
                {
                    teRegistration.EditValue = RegistrationEX.Registration;
                    teRoom.EditValue = RegistrationEX.Room;
                    teGuest.EditValue = RegistrationEX.Guest;
                }
            }

        }

        #endregion
    }
}
