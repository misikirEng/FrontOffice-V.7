
using CNET.ERP.Client.Common.UI;
using ProcessManager;
using CNET.FrontOffice_V._7.PMS.Common_Classes;
using CNET.FrontOffice_V._7.Validation;
using CNET_V7_Domain.Domain.SettingSchema;
using CNET_V7_Domain.Domain.TransactionSchema;
using CNET_V7_Domain.Domain.ViewSchema;
using DevExpress.DataProcessing;
using DevExpress.XtraEditors;
using DevExpress.XtraGantt.Scheduling;
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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using CNET_V7_Domain.Misc;
using DevExpress.ClipboardSource.SpreadsheetML;

namespace CNET.FrontOffice_V._7.Forms.Non_Navigatable_Modals
{
    public partial class frmLostAndFound : Form
    {

        private int? _adPrepared = null;

        public DateTime CurrentDate { get; set; }

        public frmLostAndFound()
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

            // Found By
            GridColumn colAssignPer = sluFoundBy.Properties.View.Columns.AddField("Id");
            colAssignPer.Visible = false;
            colAssignPer = sluFoundBy.Properties.View.Columns.AddField("Code");
            colAssignPer.Visible = true; 
            colAssignPer = sluFoundBy.Properties.View.Columns.AddField("FirstName");
            colAssignPer.Visible = true;
            colAssignPer.Caption = "First Name";
            colAssignPer = sluFoundBy.Properties.View.Columns.AddField("SecondName");
            colAssignPer.Visible = true;
            colAssignPer.Caption = "Middle Name";
            //colAssignPer = sluFoundBy.Properties.View.Columns.AddField("department");
            //colAssignPer.Visible = true;
            //colAssignPer.Caption = "Department";
            sluFoundBy.Properties.DisplayMember = "FirstName";
            sluFoundBy.Properties.ValueMember = "Id";
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
                    CurrentDate = CurrentTime.Value;
                    deDate.Properties.MaxValue = CurrentTime.Value;
                    deDate.EditValue = CurrentTime.Value;
                }



                string currentVoCode = UIProcessManager.IdGenerater("Voucher", CNETConstantes.LOST_AND_FOUND_VOUCHER, 0, LocalBuffer.LocalBuffer.CurrentDeviceConsigneeUnit.Value, false, LocalBuffer.LocalBuffer.CurrentDevice.Id);

                if (!string.IsNullOrEmpty(currentVoCode))
                {
                    teNo.Text = currentVoCode;
                }
                else
                {
                    SystemMessage.ShowModalInfoMessage("There is a problem on id setting! ", "ERROR");
                    ////CNETInfoReporter.Hide();
                    return false;
                }


                //workflow Prepared
                ActivityDefinitionDTO workFlow = UIProcessManager.GetActivityDefinitionBydescriptionandreference(CNETConstantes.LU_ACTIVITY_DEFINATION_PREPARED, CNETConstantes.LOST_AND_FOUND_VOUCHER).FirstOrDefault();

                if (workFlow != null)
                {

                    _adPrepared = workFlow.Id;
                }
                else
                {
                    SystemMessage.ShowModalInfoMessage("Please define workflow of PREPARED for LOST and FOUND Voucher ", "ERROR");
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
                List<SystemConstantDTO> osdList = LocalBuffer.LocalBuffer.ObjectStateDefinitionBufferList.Where(o => o.Category == CNETConstantes.OSD_Category_Transaction).ToList();
                sluState.Properties.DataSource = osdList;

                //Populate Employee
                List<VwConsigneeViewDTO> allEmployees = UIProcessManager.GetConsigneeViewByGslType(CNETConstantes.Employee);
                sluFoundBy.Properties.DataSource = allEmployees;

                ////CNETInfoReporter.Hide();
                return true;

            }
            catch (Exception ex)
            {
                ////CNETInfoReporter.Hide();
                SystemMessage.ShowModalInfoMessage("Error in initializing Lost and Found. Detail:: " + ex.Message, "ERROR");
                return false;
            }
        }

        private void Reset()
        {
            sluFoundBy.EditValue = null;
            deDate.EditValue = CurrentDate;
            teFoundLocation.EditValue = null;
            memoInfo.EditValue = null;
            sluState.EditValue = null;
            memoRemark.EditValue = null;


            string currentVoCode = UIProcessManager.IdGenerater("Voucher", CNETConstantes.LOST_AND_FOUND_VOUCHER, 0, LocalBuffer.LocalBuffer.CurrentDeviceConsigneeUnit.Value, false, LocalBuffer.LocalBuffer.CurrentDevice.Id);

            if (!string.IsNullOrEmpty(currentVoCode))
            {
                teNo.Text = currentVoCode;
            }

        }

        #endregion

        #region Event Handlers

        private void bbiClose_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Close();
        }


        #endregion

        private void frmLostAndFound_Load(object sender, EventArgs e)
        {
            if (!InitializeData())
            {
                this.Close();
            }
        }

        private void bbiSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                List<Control> controls = new List<Control>
                {
                    sluState,
                    sluFoundBy,
                    teFoundLocation,
                    memoInfo,
                    deDate,
                    teTime
                };

                IList<Control> invalidControls = CustomValidationRule.Validate(controls);

                if (invalidControls.Count > 0)
                {
                    ////CNETInfoReporter.Hide();
                    return;

                }



                // Progress_Reporter.Show_Progress("Saving Lost and Found", "Please Wait...");

                DateTime? CurrentTime = UIProcessManager.GetServiceTime();
                if (CurrentTime == null)
                {
                    ////CNETInfoReporter.Hide();
                    return;
                }

                string localcode = "";
                VoucherBuffer voucherbuffer = new VoucherBuffer();


                string currentVoCode = UIProcessManager.IdGenerater("Voucher", CNETConstantes.LOST_AND_FOUND_VOUCHER, 1, LocalBuffer.LocalBuffer.CurrentDeviceConsigneeUnit.Value, false, LocalBuffer.LocalBuffer.CurrentDevice.Id);

                if (!string.IsNullOrEmpty(currentVoCode))
                {
                    localcode = currentVoCode;
                }
                else
                {
                    SystemMessage.ShowModalInfoMessage("There is a problem on id setting!!!", "ERROR");
                    ////CNETInfoReporter.Hide();
                    return;
                }

                DateTime foundDate = new DateTime(deDate.DateTime.Year, deDate.DateTime.Month, deDate.DateTime.Day,
                    teTime.Time.Hour, teTime.Time.Minute, teTime.Time.Second);
                voucherbuffer.Voucher = new VoucherDTO()
                {
                    Code = localcode,
                    Definition = CNETConstantes.LOST_AND_FOUND_VOUCHER,
                    Consignee1 = Convert.ToInt32(sluFoundBy.EditValue),
                    Type = CNETConstantes.TRANSACTIONTYPENORMALTXN,
                    IssuedDate = foundDate,
                    IsIssued = true,
                    Year = foundDate.Year,
                    Month = foundDate.Month,
                    Day = foundDate.Day,
                    IsVoid = false,
                    Period = LocalBuffer.LocalBuffer.GetPeriodCode(CurrentTime.Value),
                    Remark = teFoundLocation.EditValue.ToString(),
                    LastState = Convert.ToInt32(sluState.EditValue),
                    Note = memoInfo.Text,
                    OriginConsigneeUnit = LocalBuffer.LocalBuffer.CurrentDeviceConsigneeUnit, 
                    LastUser= LocalBuffer.LocalBuffer.CurrentLoggedInUser.Id,
                    LastDevice = LocalBuffer.LocalBuffer.CurrentDevice.Id

                };
                voucherbuffer.Activity = ActivityLogManager.SetupActivity(CurrentTime.Value, _adPrepared.Value, CNETConstantes.PMS_Pointer);


                voucherbuffer.TransactionCurrencyBuffer = null;

               ResponseModel<VoucherBuffer> isSaved = UIProcessManager.CreateVoucherBuffer(voucherbuffer);
                if (isSaved != null && isSaved.Success)
                {
                    ////CNETInfoReporter.Hide();
                    SystemMessage.ShowModalInfoMessage("Lost and Found is Saved!", "MESSAGE");
                    Reset();
                }
                else
                    SystemMessage.ShowModalInfoMessage("Saving Failed !! " + Environment.NewLine + isSaved.Message, "Error");

            }
            catch (Exception ex)
            {
                ////CNETInfoReporter.Hide();
                SystemMessage.ShowModalInfoMessage("Error in Saving Lost and Found. Detail:: " + ex.Message, "ERROR");

            }
        }

        private void bbiNew_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Reset();
        }

        public string _adSyncout { get; set; }
    }
}
