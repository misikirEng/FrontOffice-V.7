using CNET.ERP.Client.Common.UI;
using CNET.FrontOffice_V._7.PMS.Contracts;
using CNET.FrontOffice_V._7;
using DevExpress.XtraEditors.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using CNET_V7_Domain.Domain.SettingSchema;
using ProcessManager;
using CNET_V7_Domain.Domain.PmsSchema;
using CNET_V7_Domain.Domain.ConsigneeSchema;
using CNET.FrontOffice_V._7.Validation;
using CNET.FrontOffice_V._7.PMS.Common_Classes;
using CNET_V7_Domain.Domain.CommonSchema;

namespace CNET.FrontOffice_V._7.Forms
{


    // public partial class frmRoomTypeEditor : Form
    public partial class frmRoomTypeEditor : UILogicBase
    {
        List<LookupDTO> roomclass = null;
        private List<HkvalueDTO> _hkValues = null;
        public DateTime? CurrentTime { get; set; }

        public frmRoomTypeEditor()
        {
            InitializeComponent();

            CurrentTime = UIProcessManager.GetServiceTime();
            if (CurrentTime == null)
            {
                XtraMessageBox.Show("Server DateTime Error !!", "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            FormSize = new Size(500, 570);
            Utility.AdjustForm(this);
            Utility.AdjustRibbon(lciRibbonContainer);
            this.Size = new Size(650, 574);
            this.Location = new Point(450, 150);

            //this.Location = new Point(350, 200);
            ceActive.Checked = true;
            roomclass = LocalBuffer.LocalBuffer.LookUpBufferList.Where(l => l.Type == CNETConstantes.ROOM_CLASS).ToList();
            deActivationDate.Properties.MinValue = CurrentTime.Value;
            cacRoomClass.Properties.Columns.Add(new LookUpColumnInfo("Description", "Description"));
            cacRoomClass.Properties.DisplayMember = "Description";
            cacRoomClass.Properties.ValueMember = "Id";
            // cacRoomClass.Properties.PopulateColumns();
            cacRoomClass.Properties.DataSource = roomclass.OrderByDescending(c => c.IsDefault).ToList();
            var Rclass = roomclass.FirstOrDefault(c => c.IsDefault);
            if (Rclass != null)
            {
                cacRoomClass.EditValue = Rclass.Id;
            }

            List<RoomTypeDTO> rTypeList = UIProcessManager.SelectAllRoomType();
            cacComponentRoom.Properties.Columns.Add(new LookUpColumnInfo("Description", "Description"));
            cacComponentRoom.Properties.DisplayMember = "Description";
            cacComponentRoom.Properties.ValueMember = "Id";
            //cacComponentRoom.Properties.PopulateColumns();
            cacComponentRoom.Properties.DataSource = rTypeList;
            deActivationDate.DateTime = CurrentTime.Value;

            iOrgUnit = LocalBuffer.LocalBuffer.HotelBranchBufferList;
            leHotel.Properties.DisplayMember = "Name";
            leHotel.Properties.ValueMember = "Id";
            leHotel.Properties.DataSource = (iOrgUnit.Select(x => new { x.Id, x.Name })).ToList();

        }


        private List<ConsigneeUnitDTO> iOrgUnit;
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
        private void bbiCancel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //var prop = ((frmProperty)this.Tag);
            //if (prop != null)
            //    prop.RefreshRoomTypeGrid();

            this.Close();
        }


        RoomTypeDTO editedRoomType;
        public Boolean editMode = false;

        internal RoomTypeDTO EditedRoomType
        {
            get { return editedRoomType; }
            set
            {
                editedRoomType = value;
                teRoomTypeCode.EditValue = value.Id;
                teDescription.Text = value.Description;
                cacComponentRoom.EditValue = value.ComponentRoom;
                deActivationDate.DateTime = (DateTime)value.ActivationDate;
                cacRoomClass.EditValue = value.RoomClass;
                teDeafultOccupancy.Text = value.DefaultOccupancy.ToString();
                teNoOfRooms.Text = value.NumberOfRooms.ToString();
                teMaximumOccupancy.Text = value.MaxOccupancy.ToString();
                teMaximumAdult.Text = value.MaxAdults.ToString();
                teMaximumChilds.Text = value.MaxChildren.ToString();
                txtRoominfo.Text = value.Remark;
                ceActive.Checked = value.IsActive;
                if (Convert.ToBoolean(value.IspseudoRoomType))
                    clbcRoomType.Items[0].CheckState = CheckState.Checked;
                else
                    clbcRoomType.Items[0].CheckState = CheckState.Unchecked;
                if (Convert.ToBoolean(value.CanBeMeetingRoom))
                    clbcRoomType.Items[1].CheckState = CheckState.Checked;
                else
                    clbcRoomType.Items[1].CheckState = CheckState.Unchecked;
                if (Convert.ToBoolean(value.IsHouseKeeping))
                    clbcRoomType.Items[2].CheckState = CheckState.Checked;
                else
                    clbcRoomType.Items[2].CheckState = CheckState.Unchecked;
                if (Convert.ToBoolean(value.AutoRoomAssign))
                    clbcRoomType.Items[3].CheckState = CheckState.Checked;
                else
                    clbcRoomType.Items[3].CheckState = CheckState.Unchecked;

                editMode = true;
                //cacDefRateCode.EditValue = value.code;

                // Get HK Values by reference
                _hkValues = UIProcessManager.GetHKValueByreference(value.Id);
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

        public override void Reset()
        {
            base.Reset();


        }

        private void bbiOk_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (tcg_formTab.SelectedTabPage == lct_basicInfoTab)
            {
                if (!Convert.ToBoolean(clbcRoomType.Items[0].CheckState) &&
                    !Convert.ToBoolean(clbcRoomType.Items[1].CheckState) &&
                    !Convert.ToBoolean(clbcRoomType.Items[2].CheckState) &&
                    !Convert.ToBoolean(clbcRoomType.Items[3].CheckState))
                {

                    SystemMessage.ShowModalInfoMessage("At list one Room Type must be selected ||", "ERROR");
                    return;
                }
                //disable new button
                bbiReset.Enabled = true;

                RoomTypeDTO rType = new RoomTypeDTO();
                rType.Id = Convert.ToInt32(teRoomTypeCode.EditValue);
                rType.Description = teDescription.Text;
                if (!string.IsNullOrEmpty(teNoOfRooms.Text))
                {
                    rType.NumberOfRooms = Convert.ToInt32(teNoOfRooms.Text);
                }
                rType.RoomClass = Convert.ToInt32(cacRoomClass.EditValue);
                rType.Index = 1;

                if (!string.IsNullOrEmpty(teDeafultOccupancy.Text))
                {
                    rType.DefaultOccupancy = Convert.ToInt32(teDeafultOccupancy.Text);
                }

                if (!string.IsNullOrEmpty(teMaximumOccupancy.Text))
                {
                    rType.MaxOccupancy = Convert.ToInt32(teMaximumOccupancy.Text);
                }

                if (!string.IsNullOrEmpty(teMaximumAdult.Text))
                {
                    rType.MaxAdults = Convert.ToInt32(teMaximumAdult.Text);
                }
                if (!string.IsNullOrEmpty(teMaximumChilds.Text))
                {
                    rType.MaxChildren = Convert.ToInt32(teMaximumChilds.Text);
                }
                rType.Consigneeunit = Convert.ToInt32(leHotel.EditValue);
                rType.ComponentRoom = (cacComponentRoom.EditValue != null && !string.IsNullOrEmpty(cacComponentRoom.EditValue.ToString())) ? Convert.ToInt32(cacComponentRoom.EditValue.ToString()) : null;
                rType.ActivationDate = deActivationDate.DateTime;
                rType.IspseudoRoomType = Convert.ToBoolean(clbcRoomType.Items[0].CheckState);
                rType.CanBeMeetingRoom = Convert.ToBoolean(clbcRoomType.Items[1].CheckState);
                rType.IsHouseKeeping = Convert.ToBoolean(clbcRoomType.Items[2].CheckState);
                rType.AutoRoomAssign = Convert.ToBoolean(clbcRoomType.Items[3].CheckState);
                rType.IsActive = ceActive.Checked;
                rType.Remark = txtRoominfo.Text;
                List<Control> controls = new List<Control>();

                controls.Add(teDescription);
                controls.Add(teNoOfRooms);
                controls.Add(leHotel);
                IList<Control> invalidControls = CustomValidationRule.Validate(controls);

                if (invalidControls.Count > 0)
                    return;
                List<RoomTypeDTO> rtList = UIProcessManager.GetRoomTypeByConsigneeUnit(SelectedHotelcode);
                RoomTypeDTO exist = rtList.FirstOrDefault(r => r.Description.ToLower() == teDescription.Text.ToLower());
                if (exist != null)
                {
                    if (editMode && rType.Id != exist.Id)
                    {
                        SystemMessage.ShowModalInfoMessage("There exists a room type with the same name!!!", "ERROR");
                        return;
                    }
                    else if (!editMode)
                    {
                        SystemMessage.ShowModalInfoMessage("There exists a room type with the same name!!!", "ERROR");
                        return;
                    }
                }
                if (editMode)
                {

                    UIProcessManager.UpdateRoomType(rType);
                }
                else
                {
                    RoomTypeDTO getRoomType = UIProcessManager.GetRoomTypeById(Convert.ToInt32(teRoomTypeCode.EditValue));

                    if (getRoomType == null)
                    {
                        UIProcessManager.CreateRoomType(rType);
                    }

                }
            }
            // HK-Credit
            else
            {
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
                    RoomTypeDTO roomType = UIProcessManager.GetRoomTypeById(Convert.ToInt32(teRoomTypeCode.EditValue));
                    if (roomType == null)
                    {
                        XtraMessageBox.Show("Unable to get room type with " + teRoomTypeCode.Text + " Code", "CNET_V2016", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    foreach (var hkValue in hkValues)
                    {
                        hkValue.Remark = "";
                        hkValue.Reference = roomType.Id;
                        hkValue.Pointer = CNETConstantes.TABLE_ROOM_TYPE;
                        HkvalueDTO isSaved = UIProcessManager.CreateHKValue(hkValue);
                        if (isSaved != null)
                        {
                            saveFaliedAttributes.Add(hkValue.Value);
                        }
                    }

                    if (saveFaliedAttributes.Count == 0)
                    {
                        XtraMessageBox.Show("HK Values Saved!", "CNET_V2016", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                        return;
                    }

                }
                else
                {
                    XtraMessageBox.Show("Unable to save HK Values", "CNET_V2016", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }//end of else

            DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void bbiReset_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //if (tcg_formTab.SelectedTabPage == lct_hkAttachment)
            //{
            //    if (!string.IsNullOrEmpty(teRoomTypeCode.Text))
            //    {
            //        attachementData.OpenNewAttachment(teRoomTypeCode.Text, CNETConstantes.COMPANY);

            //    }
            //}
            //else
            //{
            teDescription.Text = "";
            teNoOfRooms.Text = "";
            var Rclass = roomclass.FirstOrDefault(c => c.IsDefault);
            if (Rclass != null)
            {
                cacRoomClass.EditValue = Rclass;
            }
            teMaximumOccupancy.Text = "";
            teDeafultOccupancy.Text = "";
            teMaximumAdult.Text = "";
            teMaximumAdult.Text = "";
            teMaximumChilds.Text = "";
            txtRoominfo.Text = "";
            cacComponentRoom.EditValue = "";
            deActivationDate.DateTime = CurrentTime.Value;
            ceActive.Checked = true;
            clbcRoomType.Items[0].CheckState = CheckState.Unchecked;
            clbcRoomType.Items[1].CheckState = CheckState.Unchecked;
            clbcRoomType.Items[2].CheckState = CheckState.Unchecked;
            clbcRoomType.Items[3].CheckState = CheckState.Unchecked;

            //ResetRateHeader HK-Value fields
            te_daySectionCredit.EditValue = "0.0";
            te_departureCredit.EditValue = "0.0";
            te_eveningSectionCredit.EditValue = "0.0";
            te_pickupCredit.EditValue = "0.0";
            te_stayoverCredit.EditValue = "0.0";
            te_turndownCredit.EditValue = "0.0";
            // }
        }

        private void cacComponentRoom_KeyDown(object sender, KeyEventArgs e)
        {
            LookUpEdit edit = sender as LookUpEdit;
            if (e.KeyData == Keys.Back || e.KeyData == Keys.Delete)
            {
                edit.ClosePopup();
                edit.EditValue = null;

            }
            e.Handled = true;
        }

        private void cacRoomClass_KeyDown(object sender, KeyEventArgs e)
        {
            LookUpEdit edit = sender as LookUpEdit;
            if (e.KeyData == Keys.Back || e.KeyData == Keys.Delete)
            {
                edit.ClosePopup();
                edit.EditValue = null;

            }
            e.Handled = true;
        }

        private void clbcRoomType_ItemCheck(object sender, DevExpress.XtraEditors.Controls.ItemCheckEventArgs e)
        {
            //if (e.State == CheckState.Checked)
            //{
            //    foreach (CheckedListBoxItem item in clbcRoomType.Items)
            //    {
            //        if (item.Description != clbcRoomType.Items[e.Index].Description && item.CheckState == CheckState.Checked)
            //        {
            //            item.CheckState = CheckState.Unchecked;
            //        }
            //    }
            //}
        }

        private void xtraTabControl1_Click(object sender, EventArgs e)
        {

        }


        public int SelectedHotelcode { get; set; }




        private void frmRoomTypeEditor_Load(object sender, EventArgs e)
        {
            deActivationDate.DateTime = CurrentTime.Value;
            tcg_formTab.SelectedTabPageIndex = 0;
            leHotel.EditValue = SelectedHotelcode;

        }

        private void tcg_formTab_SelectedPageChanged(object sender, DevExpress.XtraLayout.LayoutTabPageChangedEventArgs e)
        {
            if (tcg_formTab.SelectedTabPage == lct_hkAttachment)
            {
                ribbonPageGroup1.Visible = false;
                ribbonPageGroup2.Visible = false;
            }
            else
            {
                ribbonPageGroup1.Visible = true;
                ribbonPageGroup2.Visible = true;
            }
        }
    }
}
