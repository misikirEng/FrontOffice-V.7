using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using CNET.ERP.Client.Common.UI;
using CNET.ERP.Client.UI_Logic.PMS.Forms.Setting_and_Miscellaneous.DTO;
using DevExpress.XtraBars;
using DevExpress.XtraEditors.Controls;
using System.Linq;
using DevExpress.XtraEditors;
using System.Text;
using CNET.FrontOffice_V._7;
using CNET_V7_Domain.Domain.PmsSchema;
using ProcessManager;
using CNET.FrontOffice_V._7.PMS.Common_Classes;
using CNET_V7_Domain.Domain.SettingSchema;
using CNET.FrontOffice_V._7.Validation;
using CNET_V7_Domain.Domain.CommonSchema;
using CNET.FrontOffice_V._7.PMS.DTO;

namespace CNET.ERP.Client.UI_Logic.PMS.Forms
{
    public partial class frmRoomDetailForProperty : XtraForm// UILogicBase 
    {
        public frmProperty prop = new frmProperty();
        private List<RoomTypeDTO> rTypeList = null;

        int editedRoomDetail;
        private List<HkvalueDTO> _hkValues = null;

        private RoomDetailDTO _roomDetail = null;
        private doorLockVM doorLock = null;


        /** Properties **/

        public Room RoomToEdit { get; set; }
        public frmProperty OwnerForm { get; set; }

        public int RoomDetailCode
        {
            get { return editedRoomDetail; }
            set
            {
                editedRoomDetail = value;

                if (value != null)
                {

                    // Get HK Values by reference
                    _hkValues = UIProcessManager.GetHKValueByreference(value).ToList();
                    if (_hkValues != null)
                    {
                        foreach (var hkValue in _hkValues)
                        {
                            if (hkValue.Attribute == CNETConstantes.STAYOVER_CREDIT.ToString())
                                te_stayoverCredit.EditValue = hkValue.Value;
                            else if (hkValue.Attribute == CNETConstantes.DEPARTURE_CREDIT.ToString())
                                te_departureCredit.EditValue = hkValue.Value;
                            else if (hkValue.Attribute == CNETConstantes.TURNDOWN_CREDIT.ToString())
                                te_turndownCredit.EditValue = hkValue.Value;
                            else if (hkValue.Attribute == CNETConstantes.PICKUP_CREDIT.ToString())
                                te_pickupCredit.EditValue = hkValue.Value;
                            else if (hkValue.Attribute == CNETConstantes.DAYSECTION_CREDIT.ToString())
                                te_daySectionCredit.EditValue = hkValue.Value;
                            else if (hkValue.Attribute == CNETConstantes.EVENING_CREDIT.ToString())
                                te_eveningSectionCredit.EditValue = hkValue.Value;

                        }
                    }
                }

            }


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



        /// //////////////////// CONSTRUCTOR /////////////////////////////////
        public frmRoomDetailForProperty(frmProperty propertyForm)
        {
            InitializeComponent();

            //  FormSize = new Size(350, 450);
            OwnerForm = propertyForm;
            InitializeUI();
        }


        #region Helper Methods

        public void InitializeUI()
        {
            Tag = this;
            Size = new Size(450, 500);
            Location = new Point(450, 150);
            this.StartPosition = FormStartPosition.CenterScreen;

            //space arrangment
            leSpaceArrangement.Properties.Columns.Add(new LookUpColumnInfo("Description", "Arrangement"));
            leSpaceArrangement.Properties.DisplayMember = "Description";
            leSpaceArrangement.Properties.ValueMember = "Id";

            //room type list
            cacRoomType1.Properties.Columns.Add(new LookUpColumnInfo("Description", "Room Types"));//,new LookUpColumnInfo("abbreviation", "Abb."))
            cacRoomType1.Properties.DisplayMember = "Description";
            cacRoomType1.Properties.ValueMember = "Id";

            cacRoomType1.EditValueChanged += cacRoomType1_EditValueChanged;
        }

        public bool InitializeData()
        {
            try
            {
                if (RoomToEdit == null)
                {
                    SystemMessage.ShowModalInfoMessage("Please Select Room!", "ERROR");
                    return false;
                }

                _roomDetail = RoomToEdit.RoomDetail;
                if (_roomDetail == null)
                {
                    SystemMessage.ShowModalInfoMessage("Unable to find room detail.", "ERROR");
                    return false;
                }

                teSpace.Text = _roomDetail.Space.ToString();
                teRoomCode.Text = _roomDetail.Id.ToString();
                memoRemark.Text = _roomDetail.Remark;
                teRoom.Text = RoomToEdit.RoomNo;
                tePhoneNo.Text = _roomDetail.PhoneNumber;
                teMeasurement.Text = RoomToEdit.Measurement.ToString();
                teMaxOccupancy.Text = _roomDetail.MaxOccupnancy == null ? "0" : _roomDetail.MaxOccupnancy.Value.ToString();
                ceIsActive.Checked = _roomDetail.IsActive == null ? false : _roomDetail.IsActive;

                // Progress_Reporter.Show_Progress("Initializing Data", "Please Wait...");

                //get lock number
                CreateDoorLockVM();

                //    doorLock = UIProcessManager.GetDoorLockViewBySpaceAndRoom(_roomDetail.Space, _roomDetail.Code).FirstOrDefault();
                doorLock = doorLockVMlist.FirstOrDefault(x => x.space == _roomDetail.Space && x.roomDetail == _roomDetail.Id);

                if (doorLock != null)
                {
                    teLockNum.Text = doorLock.KeyCode;
                }

                List<LookupDTO> spaceArrangList = LocalBuffer.LocalBuffer.LookUpBufferList.Where(l => l.Type == CNETConstantes.SPACE_ARRANGEMENT && l.IsActive).ToList();
                leSpaceArrangement.Properties.DataSource = spaceArrangList;
                if (spaceArrangList != null)
                {
                    var lukDef = spaceArrangList.FirstOrDefault(l => l.IsDefault);
                    if (lukDef != null)
                    {
                        leSpaceArrangement.EditValue = lukDef.Id;
                    }
                }



                rTypeList = UIProcessManager.GetRoomTypeByConsigneeUnit(SelectedHotelcode);
                cacRoomType1.Properties.DataSource = rTypeList;
                if (rTypeList == null || rTypeList.Count == 0)
                {
                    ////CNETInfoReporter.Hide();
                    SystemMessage.ShowModalInfoMessage("Empty Room Type!", "ERROR");
                    return false;
                }
                if (RoomToEdit.RoomType != null)
                {
                    cacRoomType1.EditValue = RoomToEdit.RoomType.Id;
                }



                ////CNETInfoReporter.Hide();
                return true;
            }
            catch (Exception ex)
            {
                ////CNETInfoReporter.Hide();
                SystemMessage.ShowModalInfoMessage("Error in initiailizing form. Detail:: " + ex.Message, "ERROR");
                return false;
            }

        }

        public List<doorLockVM> doorLockVMlist { get; set; }
        public int SelectedHotelcode { get; set; }

        private void CreateDoorLockVM()
        {
            doorLockVMlist = new List<doorLockVM>();



        }

        #endregion

        #region Event Handlers

        private void bbiOK_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                // Progress_Reporter.Show_Progress("Saving Room Detail", "Please Wait...");
                if (tcRoomDetailSpace.SelectedTabPage == tpRoomDetail)
                {
                    #region Save Room Detail


                    List<Control> controls = new List<Control>();
                    controls.Add(cacRoomType1);
                    IList<Control> invalidControls = CustomValidationRule.Validate(controls);
                    if (invalidControls.Count > 0)
                    {
                        ////CNETInfoReporter.Hide();
                        return;

                    }

                    RoomTypeDTO rType = null;
                    if (cacRoomType1.EditValue != "")
                    {
                        rType = rTypeList.FirstOrDefault(r => r.Id == Convert.ToInt32(cacRoomType1.EditValue));
                    }
                    RoomDetailDTO rd = new RoomDetailDTO();
                    rd.Id = _roomDetail.Id;
                    rd.Space = _roomDetail.Space;
                    rd.Description = teRoom.Text;
                    if (!string.IsNullOrEmpty(teMaxOccupancy.Text))
                    {
                        rd.MaxOccupnancy = Convert.ToInt32(teMaxOccupancy.Text);
                    }
                    rd.RoomType = Convert.ToInt32(cacRoomType1.EditValue);
                    rd.PhoneNumber = tePhoneNo.Text;
                    rd.Area = null;// teMeasurement.Text;
                    rd.Remark = memoRemark.Text;
                    rd.IsActive = ceIsActive.Checked;
                    rd.LastState = _roomDetail.LastState;

                    int currentCount = 0;
                    if (RoomToEdit.RoomType != null && RoomToEdit.RoomType.Id == Convert.ToInt32(cacRoomType1.EditValue))
                    {
                        currentCount = prop.GetRoomTypeCount(rType.Description);
                    }
                    else
                    {
                        currentCount = prop.GetRoomTypeCount(rType.Description) + 1;
                    }
                    if (currentCount > rType.NumberOfRooms)
                    {
                        ////CNETInfoReporter.Hide();
                        SystemMessage.ShowModalInfoMessage(
                            "You can not assign more than " + rType.NumberOfRooms + " Rooms to a Room Type: " +
                            rType.Description, "ERROR", "Selected Rooms Count Exceed");
                        return;
                    }
                    else
                    {
                        if (UIProcessManager.UpdateRoomDetail(rd) != null)
                        {


                            //update or create key
                            if (doorLock == null)
                            {
                                if (!string.IsNullOrWhiteSpace(teLockNum.Text))
                                {
                                    KeyDefinitionDTO kd = new KeyDefinitionDTO()
                                    {
                                        Space = rd.Space,
                                        KeyCode = teLockNum.Text
                                    };
                                    KeyDefinitionDTO flag = UIProcessManager.CreateKeyDefinition(kd);
                                    if (flag != null)
                                    {
                                        KeyDefinitionDTO savedKeyDef = UIProcessManager.GetKeyDefinitionByspace(rd.Space).FirstOrDefault(k => k.KeyCode == teLockNum.Text);
                                        if (savedKeyDef != null)
                                        {
                                            KeyOptionDTO ko = new KeyOptionDTO()
                                            {
                                                RoomDetail = rd.Id,
                                                KeyDefinition = savedKeyDef.Id
                                            };
                                            UIProcessManager.CreateKeyOption(ko);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                //updating key definition;
                                KeyDefinitionDTO kd = UIProcessManager.GetKeyDefinitionById(doorLock.KeyDefCode);
                                if (kd != null)
                                {
                                    kd.KeyCode = teLockNum.Text;
                                    KeyDefinitionDTO flag = UIProcessManager.UpdateKeyDefinition(kd);
                                }
                            }

                            SpaceDTO sps = UIProcessManager.GetSpaceById(Convert.ToInt32(teSpace.Text));
                            if (sps != null)
                            {
                                sps.Description = teRoom.Text;
                                UIProcessManager.UpdateSpace(sps);
                            }

                            SystemMessage.ShowModalInfoMessage("Room Detail is saved successfully!", "MESSAGE");
                            RoomToEdit.RoomType = rType;
                            RoomToEdit.RoomNo = teRoom.Text;
                            RoomToEdit.RoomDetail = rd;

                            DialogResult = DialogResult.OK;
                        }
                        else
                        {
                            SystemMessage.ShowModalInfoMessage("Room Detail is not saved!", "ERROR");
                        }




                    }



                    #endregion

                }
                else if (tcRoomDetailSpace.SelectedTabPage == tpSpaceCapacity)
                {
                    #region Save Space Capacity

                    if (teSpace.Text != "")
                    {
                        SpaceCapacityDTO spCap = new SpaceCapacityDTO();
                        spCap.Id = 0;
                        spCap.Space = Convert.ToInt32(teSpace.Text);
                        spCap.SpaceArrangemet = Convert.ToInt32(leSpaceArrangement.EditValue);
                        if (!string.IsNullOrEmpty(teMaxCapacity.Text))
                        {
                            spCap.Capacity = Convert.ToInt32(teMaxCapacity.Text);
                        }
                        spCap.Remark = meRemarkSpace.Text;
                        if (true)//UIProcessManager.CreateSpaceCapacity(spCap))
                        {
                            SystemMessage.ShowModalInfoMessage("Space Capacity saved successfully!!", "MESSAGE");
                            DialogResult = DialogResult.OK;
                        }
                        else
                        {
                            SystemMessage.ShowModalInfoMessage("Space Cpapacity is not saved", "ERROR");
                            DialogResult = DialogResult.No;
                        }
                    }
                    #endregion

                }

                else if (tcRoomDetailSpace.SelectedTabPage == tp_hkStatus)
                {

                    #region Save HK-Status
                    List<HkvalueDTO> hkValues = new List<HkvalueDTO>();

                    HkvalueDTO stayoverValue = new HkvalueDTO()
                    {
                        Attribute = CNETConstantes.STAYOVER_CREDIT.ToString(),
                        Value = Convert.ToDecimal(te_stayoverCredit.EditValue)
                    };
                    HkvalueDTO departureValue = new HkvalueDTO()
                    {
                        Attribute = CNETConstantes.DEPARTURE_CREDIT.ToString(),
                        Value = Convert.ToDecimal(te_departureCredit.EditValue)
                    };
                    HkvalueDTO turndownValue = new HkvalueDTO()
                    {
                        Attribute = CNETConstantes.TURNDOWN_CREDIT.ToString(),
                        Value = Convert.ToDecimal(te_turndownCredit.EditValue)
                    };
                    HkvalueDTO pickupValue = new HkvalueDTO()
                    {
                        Attribute = CNETConstantes.PICKUP_CREDIT.ToString(),
                        Value = Convert.ToDecimal(te_pickupCredit.EditValue),
                    };
                    HkvalueDTO daySectionValue = new HkvalueDTO()
                    {
                        Attribute = CNETConstantes.DAYSECTION_CREDIT.ToString(),
                        Value = Convert.ToDecimal(te_daySectionCredit.EditValue)

                    };
                    HkvalueDTO eveningSectionValue = new HkvalueDTO()
                    {
                        Attribute = CNETConstantes.EVENING_CREDIT.ToString(),
                        Value = Convert.ToDecimal(te_eveningSectionCredit.EditValue)
                    };

                    // Add to the list
                    hkValues.Add(stayoverValue);
                    hkValues.Add(departureValue);
                    hkValues.Add(turndownValue);
                    hkValues.Add(pickupValue);
                    hkValues.Add(daySectionValue);
                    hkValues.Add(eveningSectionValue);


                    // Delete Existing Values
                    bool isHkValuesDeleted = false;
                    if (_hkValues != null && _hkValues.Count > 0)
                    {
                        foreach (var hkValue in _hkValues)
                        {
                            try
                            {
                                isHkValuesDeleted = UIProcessManager.DeleteHKValueById(hkValue.Id);

                            }
                            catch (Exception ex)
                            {
                                isHkValuesDeleted = false;
                            }
                        }
                    }
                    else
                    {
                        isHkValuesDeleted = true;
                    }

                    if (isHkValuesDeleted)
                    {
                        List<decimal?> saveFaliedAttributes = new List<decimal?>();

                        RoomDetailDTO roomDetail = UIProcessManager.GetRoomDetailById(Convert.ToInt32(teRoomCode.Text));
                        if (roomDetail == null)
                        {
                            XtraMessageBox.Show("Unable to get room Detail with " + teRoomCode.Text + " Code", "CNET_V2016", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        foreach (var hkValue in hkValues)
                        {
                            hkValue.Id = 0;
                            hkValue.Remark = "";
                            hkValue.Reference = Convert.ToInt32(teRoomCode.Text);
                            hkValue.Pointer = CNETConstantes.TABLE_ROOM_DETAIL;
                            HkvalueDTO isSaved = UIProcessManager.CreateHKValue(hkValue);
                            if (isSaved != null)
                            {
                                saveFaliedAttributes.Add(hkValue.Value);
                            }
                        }

                        if (saveFaliedAttributes.Count == 0)
                        {
                            XtraMessageBox.Show("HK Values Saved!", "CNET_V2016", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            DialogResult = DialogResult.OK;
                        }
                        else
                        {
                            StringBuilder strBuilder = new StringBuilder();
                            foreach (var fail in saveFaliedAttributes)
                            {
                                strBuilder.Append(fail);
                                strBuilder.Append(",");
                            }

                            XtraMessageBox.Show("Unable to save " + strBuilder.ToString() + " values", "CNET_V2016", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            DialogResult = DialogResult.No;
                        }
                    }

                    else
                    {
                        XtraMessageBox.Show("Unable to save HK Values", "CNET_V2016", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        DialogResult = DialogResult.No;
                    }

                    #endregion

                }

                ////CNETInfoReporter.Hide();
            }
            catch (Exception ex)
            {
                ////CNETInfoReporter.Hide();
                SystemMessage.ShowModalInfoMessage("Error in saving. Detail:: " + ex.Message, "ERROR");

            }
        }

        private void teMaxOccupancy_EditValueChanged(object sender, EventArgs e)
        {
            if (cacRoomType1.EditValue != null && cacRoomType1.EditValue != "")
            {
                RoomTypeDTO roomType = rTypeList.FirstOrDefault(rt => rt.Id == Convert.ToInt32(cacRoomType1.EditValue));
                if (roomType != null)
                {
                    if (!string.IsNullOrEmpty(teMaxOccupancy.Text))
                    {
                        if (roomType.MaxOccupancy < Convert.ToDecimal(teMaxOccupancy.Text))
                        {
                            SystemMessage.ShowModalInfoMessage("You can not assign more than " + roomType.MaxOccupancy + " to " + roomType.Description, "ERROR");
                            bbiOK.Enabled = false;
                        }
                        else
                        {
                            bbiOK.Enabled = true;
                        }
                    }
                    else
                    {
                        bbiOK.Enabled = true;
                    }
                }
            }
        }

        private void cacRoomType1_KeyDown(object sender, KeyEventArgs e)
        {
            LookUpEdit edit = sender as LookUpEdit;
            if (e.KeyData == Keys.Back || e.KeyData == Keys.Delete)
            {
                edit.ClosePopup();
                edit.EditValue = null;

            }
            e.Handled = true;
        }

        private void teMaxOccupancy_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (cacRoomType1.EditValue != null && cacRoomType1.EditValue != "")
            {
                RoomTypeDTO roomType = rTypeList.FirstOrDefault(rt => rt.Id == Convert.ToInt32(cacRoomType1.EditValue));
                if (roomType != null)
                {
                    if (!string.IsNullOrEmpty(teMaxOccupancy.Text))
                    {
                        if (roomType.MaxOccupancy < Convert.ToDecimal(teMaxOccupancy.Text))
                        {
                            bbiOK.Enabled = false;
                            SystemMessage.ShowModalInfoMessage("You can not assign more than " + roomType.MaxOccupancy + " to " + roomType.Description, "ERROR");
                            e.Cancel = true;
                        }
                        else
                        {
                            bbiOK.Enabled = true;
                        }
                    }
                    else
                    {
                        bbiOK.Enabled = true;
                    }
                }
            }
        }

        private void teMaxOccupancy_InvalidValue(object sender, InvalidValueExceptionEventArgs e)
        {
            e.ErrorText = "You can not assign more than maximum occumpancy of the specified room type!";
        }

        void cacRoomType1_EditValueChanged(object sender, EventArgs e)
        {
            if (cacRoomType1.EditValue != null && cacRoomType1.EditValue != "")
            {
                RoomTypeDTO roomType = rTypeList.FirstOrDefault(rt => rt.Id == Convert.ToInt32(cacRoomType1.EditValue));
                if (roomType != null)
                {
                    if (!string.IsNullOrEmpty(teMaxOccupancy.Text))
                    {
                        if (roomType.MaxOccupancy < Convert.ToDecimal(teMaxOccupancy.Text))
                        {
                            bbiOK.Enabled = false;
                            SystemMessage.ShowModalInfoMessage("You can not assign more than " + roomType.MaxOccupancy + " to " + roomType.Description, "ERROR");
                        }
                        else
                        {
                            bbiOK.Enabled = true;
                        }
                    }
                    else
                    {
                        bbiOK.Enabled = true;
                    }
                }
            }
        }

        private void bbiCancel_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.Close();
        }

        private void frmRoomDetailForProperty_Load(object sender, EventArgs e)
        {
            if (!InitializeData())
            {
                this.Close();
            }
        }
        #endregion



    }
}
