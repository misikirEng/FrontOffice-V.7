//***************************************************************************************************
// Assembly	: CNET POS V6
// File		: Data\DataManager.cs
// Company	: CNET Software Technologies P.L.C
//
// Developers	:  Mekanehiowt Fisseha
// Created		: 12/2/2015 - 3:00 PM
//***************************************************************************************************
// Copyright (c) 2015 CNET Software Technologies P.L.C. All rights reserved.
// Description:	Maintain Customer Form
//***************************************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using CNET_V7_Domain.Domain.CommonSchema;
using CNET_V7_Domain.Domain.ConsigneeSchema;
using CNET_V7_Domain.Domain.SettingSchema;
using CNET_V7_Domain;
using CNET.POS.Common.Models;
using CNET_V7_Domain.Domain.ViewSchema;
using CNET_V7_Domain.Misc.CommonTypes;
using ProcessManager;

namespace ERP.EventManagement
{
    public partial class UIMaintainCustomer : XtraForm
    {

        #region Constractors 
        ObjectStateDTO objec;
        List<ObjectStateDTO> Objecss;
        ConsigneeDTO person;
        LookupDTO lookup;
        PreferenceDTO pref;
        DeviceDTO device;
        ActivityDTO activity;
        List<TaxDTO> alltax { get; set; }
        string Tin { get; set; }
        public string SavedTIN { get; set; }
        public bool isSaved = false;
        public string customercode { get; set; }
        public string CustomerTin { get; set; }
        public string CustomerTel { get; set; }
        private string identificationType { get; set; }
        public bool CanOpen = false;
        public int ActivityDefn { get; set; }

        private const int CP_NOCLOSE_BUTTON = 0x200;
        public static ConsigneeDTO CreatedCustomer { get; set; }
        List<SystemConstantDTO> Objectstatelist { get; set; }
        int gsltype { get; set; }
        #endregion

        #region Constractors
        public UIMaintainCustomer( int GslType)
        {

            InitializeComponent();
            gsltype = GslType;
            var response = UIProcessManager.IdGenerater("Consignee", CNETConstantes.Consignee_Customer, 0, LocalBuffer.LocalBuffer.CurrentDeviceConsigneeUnit.Value, false, LocalBuffer.LocalBuffer.CurrentDevice.Id);

            if (string.IsNullOrEmpty(response))
            {
                XtraMessageBox.Show("There is a problem on id setting! ", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;

            }

            txtCode.Text = response;



            lkCategory.Properties.DataSource = LocalBuffer.LocalBuffer.PreferenceBufferList.Where(x => x.SystemConstant == GslType);
            lkCategory.Properties.DisplayMember = "Description";
            lkCategory.Properties.ValueMember = "Id";

        }
        #endregion

        #region Public Methods

        private bool Validation()
        {

            errorProvider1.Clear();
            int length = 0;
            string tin = "";

            tin = txtTIN.Text.Replace("_", "").Trim();

            length = tin.Length;

            if (txtFirstName.Text == "")
            {
                errorProvider1.SetError(txtFirstName, "Insert Name");
                return false;
            }

            if (lkCategory.EditValue == null)
            {
                errorProvider1.SetError(lkCategory, "Select Category");
                return false;
            }
            if (string.IsNullOrEmpty(txtCode.Text))
            {
                XtraMessageBox.Show(this, "Please Check id Setting!", "Maintain Customer Organization Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            ConfigurationDTO TinMandatory = LocalBuffer.LocalBuffer.ConfigurationBufferList.FirstOrDefault(x => x.Reference == gsltype.ToString() && x.Attribute == "TIN is Mandatory");

            if (TinMandatory != null && TinMandatory.CurrentValue == "True")
            {
                if (txtTIN.Text == "")
                {
                    errorProvider1.SetError(txtTIN, "Insert TIN");
                    return false;
                }

                if (txtTIN.Text != "")
                {
                    if (length == 10 || length == 12 || length == 13)
                    {
                        errorProvider1.Clear();
                    }
                    else
                    {
                        errorProvider1.SetError(txtTIN, "Insert TIN");
                        return false;
                    }
                }
            }



            var activiDefOrg = LocalBuffer.LocalBuffer.ActivityDefinitionBufferList.Where(p => p.Reference == gsltype).ToList();
            if (activiDefOrg != null && activiDefOrg.Count > 0)
            {
                if (activiDefOrg.OrderBy(p => p.Index).FirstOrDefault() != null)
                {
                    ActivityDefn = activiDefOrg.OrderBy(p => p.Index).FirstOrDefault().Id;
                }
                else
                {
                    XtraMessageBox.Show("Please Define Work Flow to Maintain Organization.", "Work Flow not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;


                }
            }
            else
            {
                XtraMessageBox.Show("Please Define Work Flow to Maintain Organization.", "Work Flow not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return false;

            }
            return true;
        }
        bool IsNumber(string text)
        {
            Regex regex = new Regex(@"^[-+]?[0-9]*\.?[0-9]+$");
            return regex.IsMatch(text);
        }
        private void btnSave()
        {
            try
            {
                if (!Validation())
                {
                    return;
                }

                ConsigneeBuffer Customerbuffer = new ConsigneeBuffer();

                Customerbuffer.consignee = new ConsigneeDTO()
                {
                    Code = txtCode.Text,
                    FirstName = txtFirstName.Text, 
                    Preference = Convert.ToInt32(lkCategory.EditValue),
                    Tin = txtTIN.Text,
                    IsPerson = true,
                    GslType = gsltype,
                    IsActive = true,

                };

                Customerbuffer.consigneeUnits = new List<ConsigneeUnitDTO>()
                { new ConsigneeUnitDTO()
                    {
                     Id = 0,
                    Type = 2006,
                     Name = "Residence Address",
                     Phone1 = txtTelNo.Text,
                     Phone2 = txtPhone2.Text,
                     Email = txtEmail.Text,
                     AddressLine1 = txtAddress.Text,
                    IsActive = true,
                    Locked = false,
                    IsOnline = false,
                    CreatedOn = DateTime.Now,
                    LastModified = DateTime.Now
                }
                };

                ActivityDTO Act = new ActivityDTO();
                Act.TimeStamp = DateTime.Now;
                Act.ConsigneeUnit = LocalBuffer.LocalBuffer.CurrentDeviceConsigneeUnit;
                Act.ActivityDefinition = ActivityDefn;
                Act.Device = LocalBuffer.LocalBuffer.CurrentDevice.Id;
                Act.User = LocalBuffer.LocalBuffer.CurrentLoggedInUser.Id;
                Act.Year = DateTime.Now.Year;
                Act.Month = DateTime.Now.Month;
                Act.Day = DateTime.Now.Day;
                Act.Platform = "1";
                Act.Pointer = CNETConstants.VOUCHER_COMPONENT;
                Customerbuffer.Activity = Act;


                ConsigneeBuffer con = UIProcessManager.CreateConsigneeBuffer(Customerbuffer);

                if (con == null || con.consignee.Id == 0)
                {
                    XtraMessageBox.Show(this, "The customer is not saved!", "Maintain Customer Organization Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                else
                {

                    CreatedCustomer = new ConsigneeDTO()
                    {
                        Id = con.consignee.Id,

                        Code = Customerbuffer.consignee.Code,

                        GslType = gsltype,

                        IsPerson = true,

                        FirstName = Customerbuffer.consignee.FirstName,

                        SecondName = Customerbuffer.consignee.SecondName,

                        ThirdName = Customerbuffer.consignee.ThirdName,

                        Tin = txtTIN.Text,

                        Preference = Convert.ToInt32(lkCategory.EditValue.ToString()),

                    };
                    XtraMessageBox.Show(this, "The customer is saved!", "Maintain Customer Organization", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    clearcontrol();
                    this.Close();
                }
            }
            catch (Exception)
            {

            }


        }

        public void clearcontrol()
        {
            txtFirstName.EditValue = null;
            txtPhone2.EditValue = null;
            txtEmail.EditValue = null;
            txtAddress.EditValue = null;
            txtTelNo.EditValue = null;
            txtTIN.EditValue = null;
            lkCategory.EditValue = null;
            txtFirstName.EditValue = null;

            var response = UIProcessManager.IdGenerater("Consignee", gsltype , 1, LocalBuffer.LocalBuffer.CurrentDeviceConsigneeUnit.Value, false, LocalBuffer.LocalBuffer.CurrentDevice.Id);

            if (string.IsNullOrEmpty(response))
            {
                XtraMessageBox.Show("There is a problem on id setting! ", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;

            }

            txtCode.Text = response;
        }

        #endregion

        #region Private Methods
        private void btnOk_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnSave();
        }
        private void btnClose_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Close();
        }
        private void txtTINO_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.Control)
            {
                e.Handled = true;
                txtTIN.SelectionLength = 0;
            }

            if (e.KeyCode == Keys.Enter)
            {
                txtTelNo.Focus();
                txtTelNo.SelectAll();
            }
        }
        private void txtTelNoO_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnOk.PerformClick();

            }
        }
        private void txtNameO_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txtTIN.Focus();
                txtTIN.SelectAll();
            }
        }
        private void txtTelNoO_TextChanged(object sender, EventArgs e)
        {
            if (!IsNumber(txtTelNo.Text))
            {
                errorProvider1.SetError(txtTelNo, "invalid");
            }
        }
        private void txtTINO_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar))
            {
                if (!char.IsNumber(e.KeyChar)) // &&
                {
                    if (e.KeyChar != '-')
                    {
                        e.Handled = true;
                    }
                    else if (txtTIN.Text.Contains("-"))
                    {
                        e.Handled = true;
                    }
                }
            }
            if (!char.IsControl(e.KeyChar))
            {
                if (txtTIN.Text.Length == 10)
                {
                    if (!txtTIN.Text.Contains("-"))
                    {
                        txtTIN.Text = txtTIN.Text + '-';
                        txtTIN.SelectionStart = txtTIN.Text.Length;
                    }
                }
            }
        }

        #endregion
    }
}
