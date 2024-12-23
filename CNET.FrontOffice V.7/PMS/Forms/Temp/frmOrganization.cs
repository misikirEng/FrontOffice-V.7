using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using CNET.FrontOffice_V._7.PMS.Contracts;
using CNET.FrontOffice_V._7.Enum;

using CNET.ERP.Client.Common.UI.Library;
using CNET.ERP.Client.Common.UI;
using DevExpress.XtraTreeList;
using ImplementationDefault;
using CNET.FrontOffice_V._7.Exceptions;
using FormValidation;
using DevExpress.XtraEditors.Controls;
using CNET.FrontOffice_V._7.PMS.Contracts;

namespace CNET.FrontOffice_V._7.Forms.Temp
{
    public partial class frmOrganization : UILogicBase, ILogicHelper, ICanCreate, ICanSave, ICanDelete
    {
        private DateTime currenDateTime;
        private Organization organization = new Organization();
        private Address address = new Address();
        private List<Identification> aviltin = new List<Identification>();
        private AccountMap accountMap = new AccountMap();
        private List<AccountMap> accMapList = new List<AccountMap>();
        private Preference prefernce = new Preference();
        private Device device = new Device();
        List<Currency> currencyList = UIProcessManager.SelectAllCurrency();

        public frmOrganization()
        {
            InitializeComponent();
            ApplyIcons();
            InitializeUI();

            InitializeData();
            FormSize = new System.Drawing.Size(840, 600);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            var addressAttribute = new List<Preference>();
            addressAttribute = UIProcessManager.GetPreferenceByComponent(CNETConstantes.Address).ToList();

            cdeAddress.SetData(addressAttribute);
           
        }
        private void ApplyIcons()
        {
            ImageProvider.AssignIcon(bbiNew, CNETStandardIcons.NEW);
            ImageProvider.AssignIcon(bbiSave, CNETStandardIcons.SAVE);
            ImageProvider.AssignIcon(bbiDelete, CNETStandardIcons.DELETE);
            ImageProvider.AssignIcon(bsiOptions, CNETStandardIcons.OPTIONS);
        }
        public void OnCreate()
        {
        }

        public UI_Logic.PMS.DTO.SaveClickedResult OnSave()
        {
            List<Control> controls = new List<Control>
            {
                 cacBusinessType,
                 cacBusSegment,             
                teName
            };

            IList<Control> invalidControls = CustomValidationRule.Validate(controls);

            if (invalidControls.Count > 0)
                return new SaveClickedResult { SaveResult = Enum.SaveResult.SAVE_THENSHOWNOTHING, MessageType = Enum.MessageType.ALLERT };

            var deviceName = System.Windows.Forms.SystemInformation.ComputerName;
            var posMachineid = string.Empty;
            posMachineid = UIProcessManager.getArticleCode(deviceName);
            posMachineid = UIProcessManager.getDeviceByArticle(posMachineid);
            device.code = posMachineid;

            if (string.IsNullOrEmpty(teCorpId.Text))
            {
                if (Text == "TRAVEL AGENT")
                {
                    organization.code = UIProcessManager.GenerateId(CNETConstantes.GSL_SYSTEM_PARAMETER, device, CNETConstantes.AGENTPERSON.ToString(), CNETConstantes.ORGANIZATION);
                    organization.type = CNETConstantes.AGENT;
                }

                else
                {
                    if (Text == "GROUP")
                    {
                        organization.code = UIProcessManager.GenerateId(CNETConstantes.GSL_SYSTEM_PARAMETER, device, CNETConstantes.GROUP.ToString(), CNETConstantes.ORGANIZATION);
                        organization.type = CNETConstantes.GROUP;
                    }
                    else
                    {
                        if (Text == "SOURCE")
                        {
                            organization.code = UIProcessManager.GenerateId(CNETConstantes.GSL_SYSTEM_PARAMETER, device, CNETConstantes.BUSINESSsOURCE.ToString(), CNETConstantes.ORGANIZATION);
                            organization.type = CNETConstantes.BUSINESSsOURCE;
                        }
                        else
                        {
                            organization.code = UIProcessManager.GenerateId(CNETConstantes.GSL_SYSTEM_PARAMETER, device, CNETConstantes.COMPANY.ToString(), CNETConstantes.ORGANIZATION);
                            organization.type = CNETConstantes.CUSTOMERORG;

                        }
                    }
                }
            }
            else
            {
                organization.code = teCorpId.Text;
                if (Text == "TRAVEL AGENT")
                {
                    organization.type = CNETConstantes.AGENT;
                }
                else
                {
                    if (Text == "GROUP")
                    {
                        organization.type = CNETConstantes.GROUP;
                    }
                    else
                    {
                        if (Text == "SOURCE")
                        {
                            organization.type = CNETConstantes.BUSINESSsOURCE;
                        }
                        else
                        {
                            organization.type = CNETConstantes.CUSTOMERORG;
                        }
                    }
                }
            }

            organization.tradeName = teName.Text;
            organization.brandName = teName.Text;
            organization.businessType = cacBusinessType.EditValue.ToString();
            organization.preference = cacBusSegment.EditValue.ToString();
            organization.isActive = ceIsActive.Checked;
            prefernce = UIProcessManager.GetPrefWithKeyword(CNETConstantes.ORGANIZATION_PREF).FirstOrDefault();


            if (UIProcessManager.CreateOrganization(organization))
            {
                var lstAddress = new List<Address>();
                lstAddress = UIProcessManager.GetAddress(organization.code);
                if (lstAddress.Count > 0)
                {
                    UpdateAddress(organization.code, lstAddress);
                }
                else
                {
                    var editedAddedRows = cdeAddress.Logic.GetAddedAndEditedRows().Cast<Preference>().ToList();

                    foreach (Preference pref in editedAddedRows)
                    {
                        address = new Address();
                        address.code = string.Empty;
                        address.value = pref.remark;
                        address.reference = organization.code;
                        address.preference = pref.code;
                        UIProcessManager.CreateAddress(address);
                    }
                }

                if (!string.IsNullOrEmpty(teTINNo.Text))
                {
                    aviltin = new List<Identification>();
                    aviltin = UIProcessManager.GetIdentificationByReference(organization.code);
                    updateTIN(organization.code, aviltin.Count > 0 ? aviltin : null);
                }
                if (cacAccountNo.EditValue != "")
                {
                    accMapList = new List<AccountMap>();
                    accountMap.code = string.Empty;
                    accountMap.reference = organization.code;
                    accountMap.description = UIProcessManager.SelectAllAccount().Where(a => a.code == cacAccountNo.EditValue).Select(r => r.controlAccount).FirstOrDefault();
                    accountMap.account = cacAccountNo.EditValue.ToString();
                    accMapList.Add(accountMap);
                    UIProcessManager.CreateAccountMap(accMapList);
                }

                if (cacVIP.EditValue != "")
                {
                    var objList = new List<ObjectState>();
                    var objstaObjectState = new ObjectState();
                    objstaObjectState.code = string.Empty;
                    objstaObjectState.reference = organization.code;
                    objstaObjectState.objectStateDefinition = cacVIP.EditValue.ToString();
                    objList.Add(objstaObjectState);
                    UIProcessManager.CreateObjectState(objList);
                }
                if (cacMailingAction.EditValue != "")
                {
                    var privacy = new CNETPrivacy();
                    privacy.code = string.Empty;
                    privacy.PrivacyRule = cacMailingAction.EditValue.ToString();
                    privacy.reference = organization.code;
                    UIProcessManager.CreateCNEtPrivacy(privacy);
                }
                if (cacCurrency.EditValue != "")
                {
                    var cuPreference = new CurrencyPreference();
                    cuPreference.Code = string.Empty;
                    cuPreference.Currency = cacCurrency.EditValue.ToString();
                    cuPreference.Reference = organization.code;
                    UIProcessManager.CreateCurrencyPreference(cuPreference);
                }

                if (cacRateCode.EditValue != "")
                {
                    var negorate = new NegotiatedRate();
                    negorate.code = string.Empty;
                    negorate.consignee = organization.code;
                    negorate.rateCode = cacRateCode.EditValue.ToString();
                    PMSUIProcessManager.CreateNegotiatedRate(negorate);
                }

                if (cacOwner.EditValue != "")
                {
                    var gUser = new GSLUser();
                    gUser.Code = string.Empty;
                    gUser.Reference = CNETConstantes.CUSTOMERORG.ToString();
                    gUser.Role = cacOwner.EditValue.ToString();
                    var handle = UIProcessManager.GetLookupByKeyword(CNETConstantes.HANDLE_CASE).FirstOrDefault();
                    if (handle != null)
                    {
                        gUser.Type = handle.code;
                    }
                    UIProcessManager.CreateGSLUser(gUser);
                }

                if (!string.IsNullOrEmpty(tePotentialRoomnights.Text))
                {
                    var plan = new Plan();
                    plan.code = string.Empty;
                    var Potential = UIProcessManager.GetLookupByKeyword(CNETConstantes.POTENTIAL_ROOM).FirstOrDefault();
                    if (Potential != null)
                    {
                        plan.type = Potential.code;
                    }
                    plan.description = "Potential Room Night";
                    plan.period = UIProcessManager.getPeriodCode(currenDateTime);
                    if (tePotentialRoomnights.Text != string.Empty)
                    {
                        plan.value = Convert.ToDecimal(tePotentialRoomnights.Text);
                    }
                    var rating = UIProcessManager.GetLookupByKeyword(CNETConstantes.RATING).FirstOrDefault();
                    if (rating != null)
                    {
                        plan.rating = rating.code;
                    }
                    var planCode = UIProcessManager.CreatePlan(plan);


                    if (!string.IsNullOrEmpty(planCode))
                    {
                        var pDetail = new PlanDetail();

                        pDetail.code = string.Empty;
                        pDetail.plan = planCode;
                        pDetail.Type = CNETConstantes.TAGTYPESCHEDULE;
                        pDetail.component = CNETConstantes.ORGANIZATION;
                        pDetail.reference = organization.code;
                        pDetail.resourceRole = "Customer";
                        UIProcessManager.CreatePlanDetail(pDetail);
                    }
                }
                Reset();
                return new SaveClickedResult() { SaveResult = SaveResult.SAVE_SUCESSESFULLY, MessageType = MessageType.MESSAGEBOX };
            }
            else
            {
                throw new SaveException("Saving Not Successful", true);

            }

        }

        public UI_Logic.PMS.DTO.DeleteClickedResult OnDelete()
        {
            return new DeleteClickedResult { DeleteResult = Enum.DeleteResult.DELETE_SUCESSESFULLY, MessageType = Enum.MessageType.ALLERT };

        }

        public void InitializeUI()
        {
            if (!DesignMode)
            {
                Utility.AdjustRibbon(lciRibbonHolder);
            }
            CNETFooterRibbon.ribbonControl = rcOrganization;

            cdeAddress.SetTreeRelationshipFields("code", "parent");
            cdeAddress.SetColumns(new String[] { "description", "remark" });
            cdeAddress.Properties.ParentFieldName = "parent";
            cdeAddress.Properties.KeyFieldName = "code";
            cdeAddress.GetTreeList().Columns[0].BestFit();
            cdeAddress.GetTreeList().Columns[0].OptionsColumn.AllowEdit = false;
            cdeAddress.GetTreeList().Columns[1].BestFit();
            cdeAddress.GetTreeList().Columns[1].Caption = "Value";
            cdeAddress.Logic.InitializeData(typeof(Preference));
            cdeAddress.GetTreeList().ShowingEditor += Form3_ShowingEditor;
            cdeAddress.GetTreeList().NodeCellStyle += frmPerson_NodeCellStyle;

            try
            {
                cdeAddress.GetTreeList().ValidateNode += OnValidateNode;
                cdeAddress.GetTreeList().InvalidNodeException += OnInvalidNodeException;
            }
            catch (Exception)
            {

            }
         
        }

        private void OnInvalidNodeException(object sender, InvalidNodeExceptionEventArgs invalidNodeExceptionEventArgs)
        {
            string value = ((Preference)cdeAddress.GetFocused()).remark;
            string description =
              Convert.ToString(cdeAddress.GetTreeList().FocusedNode.GetValue("description"));

            if (!string.IsNullOrEmpty(value))
            {
                switch (description)
                {
                    case "Email":
                        if (!CustomValidationRule.Validate(new Control(), value, "Email"))
                        {
                            invalidNodeExceptionEventArgs.ExceptionMode = ExceptionMode.DisplayError;
                            invalidNodeExceptionEventArgs.ErrorText = "Please enter a valid email address.";
                        }
                        break;
                    case "Mobile phone":
                        if (!CustomValidationRule.Validate(new Control(), value, "Mobile phone"))
                        {
                            invalidNodeExceptionEventArgs.ExceptionMode = ExceptionMode.DisplayError;
                            invalidNodeExceptionEventArgs.ErrorText = "Please enter a valid email address.";
                        }
                        break;
                    case "POBox":
                        break;
                    case "Facebook":
                        break;
                    case "City":
                        break;
                    case "Telephone":
                        if (!CustomValidationRule.Validate(new Control(), value, "Telephone"))
                        {
                            invalidNodeExceptionEventArgs.ExceptionMode = ExceptionMode.DisplayError;
                            invalidNodeExceptionEventArgs.ErrorText = "Please enter a valid email address.";
                        }
                        break;
                    case "Website":
                        if (!CustomValidationRule.Validate(new Control(), value, "Website"))
                        {
                            invalidNodeExceptionEventArgs.ExceptionMode = ExceptionMode.DisplayError;
                            invalidNodeExceptionEventArgs.ErrorText = "Please enter a valid email address.";
                        }
                        break;
                }
            }
        }

        private void OnValidateNode(object sender, ValidateNodeEventArgs validateNodeEventArgs)
        {
            string value = ((Preference)cdeAddress.GetFocused()).remark;
            string description =
              Convert.ToString(cdeAddress.GetTreeList().FocusedNode.GetValue("description"));

            if (!string.IsNullOrEmpty(value))
            {
                switch (description)
                {
                    case "Email":
                        if (!CustomValidationRule.Validate(new Control(), value, "Email"))
                        {
                            validateNodeEventArgs.Valid = false;
                        }
                        break;
                    case "Mobile phone":
                        break;
                    case "POBox":
                        break;
                    case "Facebook":
                        break;
                    case "City":
                        break;
                    case "Telephone":
                        break;
                    case "Website":
                        break;
                }
            }
        }

        private void Form3_ShowingEditor(object sender, CancelEventArgs e)
        {
            if (cdeAddress.GetTreeList().FocusedColumn != null && cdeAddress.GetTreeList().FocusedNode.HasChildren)
            {
                e.Cancel = true;
            }
        }
        private void frmPerson_NodeCellStyle(object sender, GetCustomNodeCellStyleEventArgs e)
        {
            if (e.Node.HasChildren)
            {
                e.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))),
                    ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            }
        }
        public void InitializeData()
        {
            currenDateTime = UIProcessManager.GetServerDateTime("Gregorian", "Server");

            var countryList = new List<Country>();


            cacCurrency.Properties.Columns.Add(new LookUpColumnInfo("description", "Description"));
            cacCurrency.Properties.DisplayMember = "description";
            cacCurrency.Properties.ValueMember = "code";
            cacCurrency.Properties.DataSource = (currencyList.OrderByDescending(c => c.isDefault).ToList());
            var currency = currencyList.FirstOrDefault(c => c.isDefault);
            if (currency != null)
            {
                cacCurrency.EditValue = (currency.code);
            }
            var bussinessSeg = new List<Preference>();
            bussinessSeg = UIProcessManager.GetPreferenceByComponent(CNETConstantes.ORGANIZATION);
            cacBusSegment.Properties.Columns.Add(new LookUpColumnInfo("description", "Bus. Segment"));
            cacBusSegment.Properties.DisplayMember = "description";
            cacBusSegment.Properties.ValueMember = "code";
            cacBusSegment.Properties.DataSource = bussinessSeg;


            var mailingAction = new List<Lookup>();
            mailingAction = UIProcessManager.GetLookup(CNETConstantes.PRIVACYRULE);
            cacMailingAction.Properties.Columns.Add(new LookUpColumnInfo("description", "Mailing Methods"));
            cacMailingAction.Properties.DisplayMember = "description";
            cacMailingAction.Properties.ValueMember = "code";
            cacMailingAction.Properties.DataSource = (mailingAction.OrderByDescending(c => c.isDefault).ToList());

            var busTypeList = new List<Lookup>();
            busTypeList = UIProcessManager.GetLookup(CNETConstantes.BUSINESSTYPE);
            cacBusinessType.Properties.Columns.Add(new LookUpColumnInfo("description", "Types"));
            cacBusinessType.Properties.DisplayMember = "description";
            cacBusinessType.Properties.ValueMember = "code";
            cacBusinessType.Properties.DataSource = (busTypeList.OrderByDescending(c => c.isDefault).ToList());

            var objectStateDefinitions = new List<ObjectStateDefinition>();
            objectStateDefinitions =
                UIProcessManager.SelectAllObjectStateDefinition().Where(s => s.type == CNETConstantes.TAGTYPEORGANIZATION).ToList();
            cacVIP.Properties.Columns.Add(new LookUpColumnInfo("description", "Status"));
            cacVIP.Properties.DisplayMember = "description";
            cacVIP.Properties.ValueMember = "code";
            cacVIP.Properties.DataSource = (objectStateDefinitions);


            var rateCodeList = new List<RateCodeCategoryCurrency_View>();
            rateCodeList = PMSUIProcessManager.GetRateCodeCategoryCurrency();
            cacRateCode.Properties.Columns.Add(new LookUpColumnInfo("code", "Rate Code"));
            cacRateCode.Properties.Columns.Add(new LookUpColumnInfo("Category", "Category"));
            cacRateCode.Properties.Columns.Add(new LookUpColumnInfo("Currency", "Currency"));
            cacRateCode.Properties.DisplayMember = "code";
            cacRateCode.Properties.ValueMember = "code";
            cacRateCode.Properties.DataSource = (rateCodeList);

            var accountList = new List<Account>();
            accountList = UIProcessManager.SelectAllAccount();
            cacAccountNo.Properties.Columns.Add(new LookUpColumnInfo("description", "Account"));
            cacAccountNo.Properties.DisplayMember = "description";
            cacAccountNo.Properties.ValueMember = "code";
            cacAccountNo.Properties.DataSource = (accountList);

            var ownerList = new List<OrganizationUnitDefinition>();
            ownerList = UIProcessManager.SelectAllOrganizationUnitDefinition().Where(o => o.type == CNETConstantes.ROLE).ToList();
            cacOwner.Properties.Columns.Add(new LookUpColumnInfo("description", "Description"));
            cacOwner.Properties.DisplayMember = "description";
            cacOwner.Properties.ValueMember = "code";
            cacOwner.Properties.DataSource = (ownerList);
            ceIsActive.Checked = true;

            var addressAttribute = new List<Preference>();
            addressAttribute = UIProcessManager.GetPreferenceByComponent(CNETConstantes.Address).ToList();

            cdeAddress.SetData(addressAttribute);
            Reset();
        }

        private bool UpdateAddress(string orgCode, List<Address> lstAddress)
        {
            try
            {
                if (lstAddress.Count > 0)
                {
                    foreach (Address ad in lstAddress)
                    {
                        var address = new Address();
                        address.code = ad.code;
                        address.reference = orgCode;
                        address.preference = CNETConstantes.ORGANIZATION;
                        UIProcessManager.UpdateAddress(address);
                        address = new Address();
                        address.reference = orgCode;
                        address.code = ad.code;
                        address.preference = CNETConstantes.ORGANIZATION;
                        UIProcessManager.UpdateAddress(address);
                        address = new Address();
                        address.reference = orgCode;
                        address.code = ad.code;
                        address.preference = CNETConstantes.ORGANIZATION;
                        UIProcessManager.UpdateAddress(address);
                        address = new Address();
                        address.reference = orgCode;
                        address.code = ad.code;
                        address.preference = CNETConstantes.ORGANIZATION;
                        ;
                        UIProcessManager.UpdateAddress(address);
                        address = new Address();
                        address.reference = orgCode;
                        address.code = ad.code;
                        address.preference = CNETConstantes.ORGANIZATION;
                        UIProcessManager.UpdateAddress(address);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
            return false;
        }
        private bool updateTIN(string orgCode, List<Identification> aviltin)
        {
            var tin = new Identification();
            var tinList = new List<Identification>();
            tin = new Identification();

            tin.code = "";
            tin.reference = orgCode;
            tin.description = "TIN";
            tin.idNumber = teTINNo.Text;
            tin.type = CNETConstantes.PERSONAL_IDENTIFICATION_TYPETIN;
            tinList.Add(tin);
            try
            {
                if (aviltin != null)
                {
                    if (aviltin.Count > 0)
                    {
                        tinList.Add(tin);
                        UIProcessManager.UpdateIdentification(tinList);
                    }
                }
                else
                {
                    UIProcessManager.CreateIdentification(tinList);
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private void bbiNew_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Reset();
        }
        public override void Reset()
        {
            teName.Text = string.Empty;
            cacAccountNo.EditValue = "";
            teTINNo.Text = string.Empty;
            teCorpId.Text = string.Empty;
            cacBusSegment.EditValue = "";
            cacBusinessType.EditValue = "";
            var currency = currencyList.FirstOrDefault(c => c.isDefault);
            if (currency != null)
            {
                cacCurrency.EditValue = (currency.code);
            }
            cacMailingAction.EditValue = "";
            cacVIP.EditValue = "";
            cacOwner.EditValue = "";
            cacRateCode.EditValue = "";
            tePotentialRoomnights.Text = string.Empty;
            tePotentialRevenue.Text = string.Empty;
            List<Preference> currentList = cdeAddress.Logic.bindinglList.Cast<Preference>().ToList();
            foreach (Preference pr in currentList)
            {

                pr.remark = "";
            }
            cdeAddress.SetData(currentList);
            ceIsActive.Checked = true;
        }

        private void bbiSave_ItemClick_1(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
        }


        public void LoadData(UILogicBase requesterForm, object args)
        {
        }

        private void bbiNote_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ShowNote();
        }

        private void bbiNew_ItemClick_1(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Reset();
        }

        private void teTINNo_Leave(object sender, EventArgs e)
        {

        }

        private void bbiSave_ItemClick_2(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            OnSave();
        }

        private void frmOrganization_Load(object sender, EventArgs e)
        {
            Reset();
        }
    }
}