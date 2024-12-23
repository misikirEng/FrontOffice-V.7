using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using CNET.FrontOffice_V._7.PMS.Contracts;
using CNET.FrontOffice_V._7;
using CNET.ERP.Client.Common.UI.Library;
using CNET.ERP.Client.Common.UI;
using DevExpress.XtraTreeList;
using CNET.FrontOffice_V._7.Forms.Non_Navigatable_Modals;
using CNET.ERP.ResourceProvider;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using CNET.FrontOffice_V._7.Forms.State_Change;
using DevExpress.XtraTreeList.Nodes;
using System.Drawing;
using CNET_V7_Domain.Domain.AccountingSchema;
using CNET_V7_Domain.Domain.SettingSchema;
using CNET_V7_Domain.Domain.ViewSchema;
using CNET_V7_Domain.Domain.CommonSchema;
using ProcessManager;
using CNET.FrontOffice_V._7.PMS.Common_Classes;
using CNET_V7_Domain.Domain.PmsSchema;
using CNET.FrontOffice_V._7.Validation;
using CNET_V7_Domain.Domain.ConsigneeSchema;
using DevExpress.Mvvm.POCO;
using CNET.Progress.Reporter;
using CNET.FrontOffice_V._7.Group_Registration;

namespace CNET.FrontOffice_V._7.Forms
{
    public partial class frmOrganization : UILogicBase
    {
        private DateTime currenDateTime;
        private AccountMapDTO accountMap = new AccountMapDTO();
        private PreferenceDTO prefernce = new PreferenceDTO();
        private List<ConsigneeDTO> organizationList = new List<ConsigneeDTO>();
        private string[] orgNameList;

        #region Repository Edit Items 
        private RepositoryItemTextEdit noneMask = new RepositoryItemTextEdit();
        private RepositoryItemSearchLookUpEdit countryLookUpEdit = new RepositoryItemSearchLookUpEdit();
        #endregion

        private string orgCode = "";

        private string note = "";


        private ActivityDTO _activity = null;
        private int? activityDefCode = null;
        private int? defaultCurrency = null;
        private int? defaultMailAction = null;
        private int? defaultBusType = null;
        private int? defaultObjState = null;


        #region Properties

        public int GslType { get; set; }
        public bool IsFromDocumentBrowser { get; set; }
        public bool isFromRegisteration { get; set; }

        private VwConsigneeViewDTO _orgToEdit = null;
        public VwConsigneeViewDTO OrgToEdit
        {
            get
            {
                return _orgToEdit;
            }
            set
            {
                _orgToEdit = value;
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

        private ConsigneeDTO savedOrgan;
        private bool doCapitialize;

        public ConsigneeDTO SavedOrg
        {
            get
            {
                return savedOrgan;
            }
            set
            {
                savedOrgan = value;
            }
        }

        #endregion


        /// ///////////////////////////////////////////// CONSTRUCTOR ////////////////////////////////////////////////////
        public frmOrganization()
        {


            InitializeComponent();

            // FormSize = new System.Drawing.Size(840, 520);
            this.StartPosition = FormStartPosition.CenterParent;
            InitializeUI();

        }


        #region Helper Methods


        public void InitializeUI()
        {

            ApplyIcons();

            //  CNETFooterRibbon.ribbonControl = rcOrganization;

            //Country List
            GridColumn column = countryLookUpEdit.View.Columns.AddField("name");
            column.Visible = true;
            column = countryLookUpEdit.View.Columns.AddField("ICAOCountryCode");
            column.Visible = true;
            column = countryLookUpEdit.View.Columns.AddField("nationality");
            column.Visible = true;
            countryLookUpEdit.DisplayMember = "name";
            countryLookUpEdit.ValueMember = "name";
            countryLookUpEdit.NullText = "";

            //Currency List
            cacCurrency.Properties.Columns.Add(new LookUpColumnInfo("Description", "Description"));
            cacCurrency.Properties.DisplayMember = "Description";
            cacCurrency.Properties.ValueMember = "Id";


            //Business Seg
            cacBusSegment.Properties.Columns.Add(new LookUpColumnInfo("Description", "Bus. Segment"));
            cacBusSegment.Properties.DisplayMember = "Description";
            cacBusSegment.Properties.ValueMember = "Id";

            //Mailing Action
            cacMailingAction.Properties.Columns.Add(new LookUpColumnInfo("Description", "Mailing Methods"));
            cacMailingAction.Properties.DisplayMember = "Description";
            cacMailingAction.Properties.ValueMember = "Id";

            //Gsl Tax
            lukTaxType.Properties.Columns.Add(new LookUpColumnInfo("Description", "Mailing Methods"));
            lukTaxType.Properties.DisplayMember = "Description";
            lukTaxType.Properties.ValueMember = "Id";

            //Business Type
            cacBusinessType.Properties.Columns.Add(new LookUpColumnInfo("Description", "Types"));
            cacBusinessType.Properties.DisplayMember = "Description";
            cacBusinessType.Properties.ValueMember = "Id";

            //Object State
            cacObjectState.Properties.Columns.Add(new LookUpColumnInfo("Description", "Status"));
            cacObjectState.Properties.DisplayMember = "Description";
            cacObjectState.Properties.ValueMember = "Id";

            //Rate Code
            cacRateCode.Properties.Columns.Add(new LookUpColumnInfo("Id", "Rate Code"));
            cacRateCode.Properties.Columns.Add(new LookUpColumnInfo("Category", "Category"));
            cacRateCode.Properties.Columns.Add(new LookUpColumnInfo("Currency", "Currency"));
            cacRateCode.Properties.DisplayMember = "Id";
            cacRateCode.Properties.ValueMember = "Id";

            //Account Number
            cacAccountNo.Properties.Columns.Add(new LookUpColumnInfo("contAcct", "Account"));
            cacAccountNo.Properties.DisplayMember = "contAcct";
            cacAccountNo.Properties.ValueMember = "Id";

            //Owner
            cacOwner.Properties.Columns.Add(new LookUpColumnInfo("Description", "Description"));
            cacOwner.Properties.DisplayMember = "Description";
            cacOwner.Properties.ValueMember = "Id";

        }

        private bool InitializeData()
        {
            try
            {

                if (GslType == CNETConstantes.CUSTOMER)
                {
                    this.Text = "Company";
                }
                else if (GslType == CNETConstantes.AGENT)
                {
                    this.Text = "Travel Agent";

                }
                else if (GslType == CNETConstantes.GROUP)
                {
                    this.Text = "Group";
                }

                else if (GslType == CNETConstantes.BUSINESSsOURCE)
                {
                    this.Text = "Business Source";
                }

                // Progress_Reporter.Show_Progress("Generting Id", "Please Wait...", 1, 13);

                //Generate Id  
                string currentVoCode = UIProcessManager.IdGenerater("Consignee", CNETConstantes.CUSTOMER, 0, LocalBuffer.LocalBuffer.CurrentDeviceConsigneeUnit.Value, false, LocalBuffer.LocalBuffer.CurrentDevice.Id);

                if (!string.IsNullOrEmpty(currentVoCode))
                {
                    orgCode = currentVoCode;
                    teCorpId.Text = orgCode;
                }
                else
                {
                    ////CNETInfoReporter.Hide();
                    SystemMessage.ShowModalInfoMessage("There is a problem on id setting!!!", "ERROR");
                    return false;
                }



                if (OrgToEdit != null)
                {
                    ActivityDefinitionDTO workFlow = UIProcessManager.GetActivityDefinitionBydescriptionandreference(CNETConstantes.Activity_Updated, GslType).FirstOrDefault();

                    if (workFlow != null)
                    {
                        activityDefCode = workFlow.Id;
                    }
                    else
                    {
                        ////CNETInfoReporter.Hide();
                        SystemMessage.ShowModalInfoMessage("Please define workflow of UPDATED!", "ERROR");
                        return false;
                    }



                    //show attachment menu
                    rpgAttachment.Visible = true;

                    //Check Activity Previlage
                    var userRoleMapper = LocalBuffer.LocalBuffer.UserRoleMapperBufferList.FirstOrDefault(r => r.User == LocalBuffer.LocalBuffer.CurrentLoggedInUser.Id);
                    if (userRoleMapper != null)
                    {
                        var roleActivity = UIProcessManager.GetRoleActivityByactivityDefinition(activityDefCode.Value).FirstOrDefault(r => r.Role == userRoleMapper.Role && r.NeedsPassCode);
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
                }
                else
                {
                    ActivityDefinitionDTO workFlow = UIProcessManager.GetActivityDefinitionBydescriptionandreference(CNETConstantes.MAINTAINED, GslType).FirstOrDefault();

                    if (workFlow != null)
                    {
                        activityDefCode = workFlow.Id;
                    }
                    else
                    {
                        ////CNETInfoReporter.Hide();
                        SystemMessage.ShowModalInfoMessage("Please define workflow of MAINTAINED!", "ERROR");
                        return false;
                    }

                    //Check Activity Previlage
                    var userRoleMapper = LocalBuffer.LocalBuffer.UserRoleMapperBufferList.FirstOrDefault(r => r.User == LocalBuffer.LocalBuffer.CurrentLoggedInUser.Id);
                    if (userRoleMapper != null)
                    {
                        var roleActivity = UIProcessManager.GetRoleActivityByactivityDefinition(activityDefCode.Value).FirstOrDefault(r => r.Role == userRoleMapper.Role && r.NeedsPassCode);
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
                }

                AutoCompleteName();

                #region other initialization

                //Tax Type List
                List<TaxDTO> taxType = LocalBuffer.LocalBuffer.TaxBufferList;
                lukTaxType.Properties.DataSource = taxType;
                lukTaxType.EditValue = CNETConstantes.VAT;

                //Country List
                // Progress_Reporter.Show_Progress("Populating Country List", "Please Wait...", 3, 13);
                List<CountryDTO> countryList = LocalBuffer.LocalBuffer.CountryBufferList;
                countryLookUpEdit.DataSource = countryList;

                //Currency List
                // Progress_Reporter.Show_Progress("Populating Currerncy List", "Please Wait...", 4, 13);
                List<CurrencyDTO> currencyList = LocalBuffer.LocalBuffer.CurrencyBufferList;
                if (currencyList != null)
                {
                    cacCurrency.Properties.DataSource = (currencyList.OrderByDescending(c => c.IsDefault).ToList());
                    var currency = currencyList.FirstOrDefault(c => c.IsDefault);
                    if (currency != null)
                    {
                        defaultCurrency = currency.Id;
                        cacCurrency.EditValue = (currency.Id);
                    }

                }

                //Business Segment List
                // Progress_Reporter.Show_Progress("Populating Business Segment List", "Please Wait...", 5, 13);
                List<PreferenceDTO> bussinessSeg = LocalBuffer.LocalBuffer.PreferenceBufferList.Where(p => p.SystemConstant == GslType).ToList();
                cacBusSegment.Properties.DataSource = bussinessSeg;

                if (bussinessSeg != null && bussinessSeg.Count > 0)
                    cacBusSegment.EditValue = bussinessSeg.FirstOrDefault().Id;
                //Mailing Action List
                // Progress_Reporter.Show_Progress("Populating Mailing List", "Please Wait...", 6, 13);
                List<LookupDTO> mailingActionList = LocalBuffer.LocalBuffer.LookUpBufferList.Where(l => l.Type == CNETConstantes.PRIVACYRULE && l.IsActive).ToList();
                if (mailingActionList != null)
                {
                    cacMailingAction.Properties.DataSource = (mailingActionList.OrderByDescending(c => c.IsDefault).ToList());
                    var mailAction = mailingActionList.FirstOrDefault(c => c.IsDefault);
                    if (mailAction != null)
                    {
                        defaultMailAction = mailAction.Id;
                        cacMailingAction.EditValue = (mailAction.Id);
                    }

                }

                //Business Type List
                // Progress_Reporter.Show_Progress("Populating Business Type List", "Please Wait...", 7, 13);
                List<SystemConstantDTO> busTypeList = LocalBuffer.LocalBuffer.SystemConstantDTOBufferList.Where(l => l.Category == CNETConstantes.BUSINESSTYPE && l.IsActive).ToList();
                if (busTypeList != null)
                {
                    cacBusinessType.Properties.DataSource = (busTypeList.OrderByDescending(c => c.IsDefault).ToList());
                    var bustype = busTypeList.FirstOrDefault(c => c.IsDefault);
                    if (bustype != null)
                    {
                        defaultBusType = bustype.Id;
                        cacBusinessType.EditValue = (bustype.Id);
                    }

                }

                //for Object state list
                // Progress_Reporter.Show_Progress("Populating Object State List", "Please Wait...", 8, 13);
                var objectStateDefinitions = LocalBuffer.LocalBuffer.ObjectStateDefinitionBufferList;
                cacObjectState.Properties.DataSource = (objectStateDefinitions);
                if (objectStateDefinitions != null)
                {
                    cacObjectState.EditValue = objectStateDefinitions.FirstOrDefault().Id;
                    defaultObjState = objectStateDefinitions.FirstOrDefault().Id;
                }

                //rate code list
                // Progress_Reporter.Show_Progress("Populating Rate List", "Please Wait...", 9, 13);
                var rateCodeList = UIProcessManager.SelectAllRateCodeHeader();
                cacRateCode.Properties.DataSource = (rateCodeList);

                //account List
                // Progress_Reporter.Show_Progress("Populating Account List", "Please Wait...", 10, 13);
                var accountList = new List<GslacctRequirementDTO>();
                accountList = UIProcessManager.GetGSLAcctRequirementBygslTypeList(GslType);
                if (accountList != null)
                {
                    foreach (var acc in accountList)
                    {
                        //  acc.ContAcct = UIProcessManager.SelectAllControlAccount(acc.ContAcct) != null ? UIProcessManager.SelectAllControlAccount(acc.ContAcct).description : String.Empty;
                    }

                }
                cacAccountNo.Properties.DataSource = (accountList);

                //Owner List
                // Progress_Reporter.Show_Progress("Populating Owner List", "Please Wait...", 11, 13);
                //   var ownerList = LocalBuffer.LocalBuffer.OrganizationUnitDefinitionBufferList.Where(o => o.type == CNETConstantes.ROLE).ToList();
                cacOwner.Properties.DataSource = null;// (ownerList);

                //read configuration setting for capitialization
                var config = LocalBuffer.LocalBuffer.ConfigurationBufferList.FirstOrDefault(c => c.Reference == GslType.ToString() && c.Attribute == "Maintain Case");
                if (config != null)
                {
                    if (config.CurrentValue == "UpperCase")
                        doCapitialize = true;

                }

                //Organization List
                // Progress_Reporter.Show_Progress("Populating Organization List", "Please Wait...", 12, 13);
                List<ConsigneeDTO> _orgList = LocalBuffer.LocalBuffer.AllCustomerConsigneeViewlist.Where(o => o.GslType == GslType).ToList();
                if (_orgList != null)
                {
                    teName.Properties.Items.AddRange(_orgList.Select(o => o.FirstName).ToList());
                }


                //Address Map List
                Progress_Reporter.Show_Progress("Populating Address Map List", "Please Wait.......");
                //List<GSLAddressMap> addresmMaps = UIProcessManager.GetGSLAddressMap(GslType);
                //if (addresmMaps != null)
                //{
                //    foreach (var am in addresmMaps)
                //    {
                //        Preference pref = LocalBuffer.LocalBuffer.PreferenceBufferList.FirstOrDefault(p => p.code == am.prefrrence);
                //        if (pref == null) continue;
                //        AddressDTO aDto = new AddressDTO()
                //        {
                //            code = pref.code,
                //            description = pref.description,
                //            parent = pref.parent,
                //            reference = pref.reference,
                //            value = pref.value
                //        };

                //        addressDtoList.Add(aDto);
                //    }
                //}



                ceIsActive.Checked = true;

                #endregion

                //Populate Org if it is editing
                if (OrgToEdit != null)
                {
                    ConsigneeDTO vwConsignee = UIProcessManager.GetConsigneeById(OrgToEdit.Id);
                    PopulateOrganizationFields(vwConsignee);
                }

                ////CNETInfoReporter.Hide();
                return true;

            }
            catch (Exception ex)
            {
                ////CNETInfoReporter.Hide();
                SystemMessage.ShowModalInfoMessage("Error in initializing form. Detail: " + ex.Message, "ERROR");
                return false;
            }
        }

        private void PopulateOrganizationFields(ConsigneeDTO orgSaved)
        {
            if (orgSaved != null)
            {
                teCorpId.Text = orgSaved.Code;
                teName.Text = orgSaved.FirstName;
                teTINNo.Text = orgSaved.Tin;
                ceIsActive.Checked = orgSaved.IsActive;
                cacBusinessType.EditValue = orgSaved.BusinessType;
                cacBusSegment.EditValue = orgSaved.Preference;
                note = orgSaved.Note;

                //load gsl tax
                GsltaxDTO gTax = UIProcessManager.GetGSLTaxByReference(orgSaved.Id);
                if (gTax != null)
                {
                    lukTaxType.EditValue = gTax.Tax;
                }
                cacCurrency.EditValue = orgSaved.DefaultCurrency;

                //CNETPrivacy cnetPrivacy = UIProcessManager.SelectAllCNETPrivacy().FirstOrDefault(c => c.reference == orgSaved.code);
                //if (cnetPrivacy != null)
                //{
                //    cacMailingAction.EditValue = cnetPrivacy.PrivacyRule;
                //}

                //List<NegotiatedRateDTO> nerateNegotiatedRate = UIProcessManager.GetNegotiationRateByrateCode(orgSaved.Id);
                //if (nerateNegotiatedRate != null && nerateNegotiatedRate.Count >0)
                //{
                //    cacRateCode.EditValue = nerateNegotiatedRate.FirstOrDefault().RateCodeHeader;
                //}
                ObjectStateDTO objectState = UIProcessManager.GetObjectStateByReference(orgSaved.Id);

                if (objectState != null)
                {
                    cacObjectState.EditValue = objectState.ObjectStateDefinition;
                }
                List<AccountMapDTO> accMap = UIProcessManager.GetAccountMapByreference(orgSaved.Id);
                if (accMap != null && accMap.Count > 0)
                {
                    cacAccountNo.EditValue = accMap.FirstOrDefault().Remark;
                }

                //List<PlanDetail> pDetails = UIProcessManager.SelectAllPlanDetail().Where(p => p.reference == orgSaved.code).ToList();
                //if (pDetails != null)
                //{
                //    foreach (var pd in pDetails)
                //    {
                //        Plan plan = UIProcessManager.SelectPlan(pd.plan);
                //        if (plan != null)
                //        {
                //            if (plan.description == "Potential Room Night")
                //            {
                //                tePotentialRoomnights.Text = plan.value.ToString();
                //            }
                //            else if (plan.description == "Potential Revenue")
                //            {
                //                tePotentialRevenue.Text = plan.value.ToString();
                //            }
                //        }
                //    }

                //}

                //address
                //List<Address> addresses = LocalBuffer.LocalBuffer.AddressBufferList.Where(r => r.reference == orgSaved.code).ToList();
                //List<AddressDTO> currentList = treeList_orgAddress.DataSource as List<AddressDTO>;
                //if (currentList == null)
                //{
                //    currentList = new List<AddressDTO>();
                //}
                //foreach (var add in addresses)
                //{
                //    if (add == null) continue;
                //    AddressDTO pref = currentList.FirstOrDefault(p => p != null && p.code == add.preference);
                //    if (pref != null)
                //    {
                //        pref.remark = add.value;
                //    }
                //}
                //treeList_orgAddress.DataSource = currentList;
                //treeList_orgAddress.RefreshDataSource();
                //treeList_orgAddress.ExpandAll();
            }
        }

        private void AutoCompleteName()
        {
            if (LocalBuffer.LocalBuffer.AllCustomerConsigneeViewlist == null) return;
            orgNameList = LocalBuffer.LocalBuffer.AllCustomerConsigneeViewlist.Where(x => x.FirstName != null).Select(p => doCapitialize ? p.FirstName.ToUpper() : p.FirstName).ToArray();

            AutoCompleteStringCollection collection = new AutoCompleteStringCollection();
            collection.AddRange(orgNameList);
            teName.MaskBox.AutoCompleteSource = AutoCompleteSource.CustomSource;
            teName.MaskBox.AutoCompleteMode = AutoCompleteMode.Suggest;
            teName.MaskBox.AutoCompleteCustomSource = collection;
        }



        public override void Reset()
        {
            rpgAttachment.Visible = false;
            string currentVoCode = UIProcessManager.IdGenerater("Consignee", GslType, 0, LocalBuffer.LocalBuffer.CurrentDeviceConsigneeUnit.Value, false, LocalBuffer.LocalBuffer.CurrentDevice.Id);

            if (!string.IsNullOrEmpty(currentVoCode))
            {
                orgCode = currentVoCode;
                teCorpId.Text = orgCode;
            }
            else
            {
                SystemMessage.ShowModalInfoMessage("There is a problem on id setting!!!", "ERROR");
                this.Close();
            }
            //AutoCompleteName();
            teName.Text = string.Empty;
            cacAccountNo.EditValue = "";
            teTINNo.Text = string.Empty;
            teCorpId.Text = orgCode;
            cacBusSegment.EditValue = "";
            cacBusinessType.EditValue = defaultBusType;
            cacCurrency.EditValue = defaultCurrency;
            cacMailingAction.EditValue = defaultMailAction;
            cacObjectState.EditValue = defaultObjState;
            cacOwner.EditValue = "";
            cacRateCode.EditValue = "";
            txtphone1.EditValue = null;
            txtphone2.EditValue = null;
            txtemail.EditValue = null;
            txtwebsite.EditValue = null;
            txtpobox.EditValue = null;
            sleRegion.EditValue = null;
            sleCity.EditValue = null;
            sleSubCity.EditValue = null;
            txtwereda.EditValue = null;
            txtkebele.EditValue = null;
            txtstreet.EditValue = null;
            txtaddress1.EditValue = null;
            //tePotentialRoomnights.Text = string.Empty;
            //tePotentialRevenue.Text = string.Empty;

            ceIsActive.Checked = true;
        }


        public void LoadData(UILogicBase requesterForm, object args)
        {
            if (requesterForm.SubForm.GetType() == typeof(frmGroupRegistration) || requesterForm.SubForm.GetType() == typeof(frmReservation) || requesterForm.SubForm.GetType() == typeof(frmProfileAmendment) || requesterForm.SubForm.GetType() == typeof(frmCheckIn))
            {
                isFromRegisteration = true;
            }
            //if (requesterForm.SubForm.GetType() == typeof(frmDocumentBrowser))
            //{
            //    IsFromDocumentBrowser = true;
            //}
        }


        public delegate void ApplyDelegate();
        private void ApplyIcons()
        {
            Image Image = Provider.GetImage("New", ProviderType.APPLICATIONICON, PictureSize.Dimension_32X32);

            bbiNew.Glyph = Image;
            bbiNew.LargeGlyph = Image;

            Image = Provider.GetImage("Save", ProviderType.APPLICATIONICON, PictureSize.Dimension_32X32);

            bbiSave.Glyph = Image;
            bbiSave.LargeGlyph = Image;


            Image = Provider.GetImage("Options", ProviderType.APPLICATIONICON, PictureSize.Dimension_32X32);

            bsiOptions.Glyph = Image;
            bsiOptions.LargeGlyph = Image;

        }


        public void OnSave()
        {
            List<Control> controls = new List<Control>
            {
                cacBusinessType,
                teCorpId,
                teName,
                teTINNo
            };

            IList<Control> invalidControls = CustomValidationRule.Validate(controls);

            if (invalidControls.Count > 0)
            {
                SystemMessage.ShowModalInfoMessage("PLEASE FILL ALL REQUIRED FIELDS", "ERROR");
                return;
            }

            //   VwConsigneeViewDTO CheckOrganization =LocalBuffer.LocalBuffer.ConsigneeViewBufferList.FirstOrDefault(x=> x.Code == teCorpId.Text);
            if (_orgToEdit != null)
            {
                ConsigneeBuffer existOrganization = UIProcessManager.GetConsigneeBufferById(_orgToEdit.Id);
                DateTime? CurrentTime = UIProcessManager.GetServiceTime();
                /**************** UPDATING ******************************/
                #region Update Organization

                // Progress_Reporter.Show_Progress("Updating " + this.Text);
                existOrganization.consignee.IsPerson = false;
                existOrganization.consignee.FirstName = teName.Text;
                existOrganization.consignee.SecondName = teName.Text;
                existOrganization.consignee.BusinessType = Convert.ToInt32(cacBusinessType.EditValue);
                existOrganization.consignee.IsActive = ceIsActive.Checked;
                existOrganization.consignee.Tin = teTINNo.Text;

                //preference
                if (cacBusSegment.EditValue != null && !string.IsNullOrEmpty(cacBusSegment.EditValue.ToString()))
                {
                    existOrganization.consignee.Preference = Convert.ToInt32(cacBusSegment.EditValue);
                }
                else
                {
                    var firstOrDefault = LocalBuffer.LocalBuffer.PreferenceBufferList.FirstOrDefault(p => p.SystemConstant == GslType);
                    if (firstOrDefault != null)
                        existOrganization.consignee.Preference = firstOrDefault.Id;

                }

                existOrganization.Activity = ActivityLogManager.SetupActivity(CurrentTime.Value, activityDefCode.Value, CNETConstantes.CONSIGNEE, "");
                existOrganization.consignee.Note = note;

                if (GslType == CNETConstantes.CUSTOMER)
                {
                    existOrganization.Gsltaxs = new List<GsltaxDTO>();
                    GsltaxDTO tax = new GsltaxDTO()
                    {
                        Tax = Convert.ToInt32(lukTaxType.EditValue.ToString())
                    };
                    existOrganization.Gsltaxs.ToList().Add(tax);
                }
                if (cacCurrency.EditValue != null && !string.IsNullOrEmpty(cacCurrency.EditValue.ToString()))
                {
                    existOrganization.consignee.DefaultCurrency = Convert.ToInt32(cacCurrency.EditValue);
                }

                existOrganization.consigneeUnits = CreateConssigneeUnit();

                ConsigneeBuffer updatedconsignee = UIProcessManager.UpdateConsigneeBuffer(existOrganization);


                if (updatedconsignee != null)
                {

                    ////CNETInfoReporter.Hide();

                    //Account Map
                    AccountMapDTO accMap = UIProcessManager.GetAccountMapByreferencefirstordefault(updatedconsignee.consignee.Id);
                    if (accMap != null)
                    {
                        // Progress_Reporter.Show_Progress("updating Account Requirement");
                        if (cacAccountNo.EditValue != null && !string.IsNullOrEmpty(cacAccountNo.EditValue.ToString()))
                        {
                            accMap.Description = cacAccountNo.Text; // 
                            accMap.Remark = cacAccountNo.EditValue.ToString();
                            UIProcessManager.UpdateAccountMap(accMap);
                        }
                        else
                        {
                            UIProcessManager.DeleteAccountMapById(accMap.Id);
                        }
                        ////CNETInfoReporter.Hide();
                    }
                    else if (cacAccountNo.EditValue != null && !string.IsNullOrEmpty(cacAccountNo.EditValue.ToString()))
                    {
                        ControlAccountDTO acctAccount = UIProcessManager.GetControlAccountById(Convert.ToInt32(cacAccountNo.EditValue));

                        if (acctAccount != null)
                        {
                            // Progress_Reporter.Show_Progress("Saving Account Requirement"); 
                            accountMap.Reference = existOrganization.consignee.Id.ToString();
                            accountMap.Description = cacAccountNo.Text; // 
                            accountMap.Account = acctAccount.Id;
                            accountMap.Remark = cacAccountNo.EditValue.ToString();
                            UIProcessManager.CreateAccountMap(accountMap);
                            ////CNETInfoReporter.Hide();
                        }
                    }

                    //Object State
                    ObjectStateDTO objectState = UIProcessManager.GetObjectStateByReference(existOrganization.consignee.Id);


                    if (objectState != null)
                    {
                        // Progress_Reporter.Show_Progress("Updating Object State");
                        if (cacObjectState.EditValue != null && !string.IsNullOrEmpty(cacObjectState.EditValue.ToString()))
                        {
                            objectState.ObjectStateDefinition = Convert.ToInt32(cacObjectState.EditValue);
                            UIProcessManager.UpdateObjectState(objectState);
                        }
                        else
                        {
                            UIProcessManager.DeleteObjectStateById(objectState.Id);
                        }
                        ////CNETInfoReporter.Hide();
                    }
                    else if (cacObjectState.EditValue != null && !string.IsNullOrEmpty(cacObjectState.EditValue.ToString()))
                    {
                        // Progress_Reporter.Show_Progress("Saving status"); 
                        ObjectStateDTO objstaObjectState = new ObjectStateDTO();
                        objstaObjectState.Reference = existOrganization.consignee.Id;
                        objstaObjectState.ObjectStateDefinition = Convert.ToInt32(cacObjectState.EditValue);
                        UIProcessManager.CreateObjectState(objstaObjectState);
                        ////CNETInfoReporter.Hide();
                    }

                    //Privancy for mailing Action
                    //CNETPrivacy cnetPrivacy =
                    //    UIProcessManager.SelectAllCNETPrivacy().FirstOrDefault(c => c.reference == existOrganization.code);
                    //if (cnetPrivacy != null)
                    //{
                    //   // Progress_Reporter.Show_Progress("Updating mailing Action");
                    //    if (cacMailingAction.EditValue != null && !string.IsNullOrEmpty(cacMailingAction.EditValue.ToString()))
                    //    {
                    //        cnetPrivacy.PrivacyRule = cacMailingAction.EditValue.ToString();
                    //        UIProcessManager.UpdateCNETPrivacy(cnetPrivacy);
                    //    }
                    //    else
                    //    {
                    //        UIProcessManager.DeleteCNEtPrivacy(cnetPrivacy.code);
                    //    }
                    //   ////CNETInfoReporter.Hide();
                    //}
                    //else if (cacMailingAction.EditValue != null && !string.IsNullOrEmpty(cacMailingAction.EditValue.ToString()))
                    //{
                    //   // Progress_Reporter.Show_Progress("Saving Mailing Action");
                    //    var privacy = new CNETPrivacy();
                    //    privacy.code = string.Empty;
                    //    privacy.PrivacyRule = cacMailingAction.EditValue.ToString();
                    //    privacy.reference = existOrganization.code;
                    //    UIProcessManager.CreateCNEtPrivacy(privacy);
                    //   ////CNETInfoReporter.Hide();
                    //}

                    //Negotiated Rate

                    //NegotiatedRate nerateNegotiatedRate =
                    //    UIProcessManager.SelectAllNegotiatedRate().FirstOrDefault(n => n.consignee == existOrganization.code);
                    //if (nerateNegotiatedRate != null)
                    //{
                    //   // Progress_Reporter.Show_Progress("Updating Negotiated Rate");
                    //    if (cacRateCode.EditValue != null && !string.IsNullOrEmpty(cacRateCode.EditValue.ToString()))
                    //    {
                    //        nerateNegotiatedRate.rateCode = cacRateCode.EditValue.ToString();
                    //        UIProcessManager.UpdateNegotiatedRate(nerateNegotiatedRate);
                    //    }
                    //    else
                    //    {
                    //        UIProcessManager.DeleteNegotiatedRate(nerateNegotiatedRate.code);
                    //    }
                    //   ////CNETInfoReporter.Hide();
                    //}
                    //else if (cacRateCode.EditValue != null && !string.IsNullOrEmpty(cacRateCode.EditValue.ToString()))
                    //{
                    //   // Progress_Reporter.Show_Progress("Saving Rate Code");
                    //    var negorate = new NegotiatedRate();
                    //    negorate.code = string.Empty;
                    //    negorate.consignee = existOrganization.code;
                    //    negorate.rateCode = cacRateCode.EditValue.ToString();
                    //    UIProcessManager.CreateNegotiatedRate(negorate);
                    //   ////CNETInfoReporter.Hide();
                    //}

                    //Plan Detail
                    //List<PlanDetail> pDetails =
                    //    UIProcessManager.SelectAllPlanDetail().Where(p => p.reference == existOrganization.code).ToList();
                    //if (pDetails.Count > 0 && pDetails != null)
                    //{
                    //    foreach (var pd in pDetails)
                    //    {
                    //        Plan plan = UIProcessManager.SelectPlan(pd.plan);
                    //        if (plan != null)
                    //        {
                    //            if (plan.description == "Potential Room Night")
                    //            {
                    //                if (!string.IsNullOrEmpty(tePotentialRoomnights.Text))
                    //                {
                    //                    plan.value = Convert.ToDecimal(tePotentialRoomnights.Text);
                    //                    UIProcessManager.UpdatePlan(plan);
                    //                }
                    //                else
                    //                {
                    //                    UIProcessManager.DeletePlanDetail(pd.code);
                    //                    UIProcessManager.DeletePlan(plan.code);
                    //                }

                    //            }
                    //            else if (plan.description == "Potential Revenue")
                    //            {
                    //                if (!string.IsNullOrEmpty(tePotentialRevenue.Text))
                    //                {
                    //                    plan.value = Convert.ToDecimal(tePotentialRevenue.Text);
                    //                    UIProcessManager.UpdatePlan(plan);
                    //                }
                    //                else
                    //                {
                    //                    UIProcessManager.DeletePlanDetail(pd.code);
                    //                    UIProcessManager.DeletePlan(plan.code);
                    //                }
                    //            }
                    //        }
                    //    }
                    //}
                    //else
                    //{
                    //    if (!string.IsNullOrEmpty(tePotentialRoomnights.Text))
                    //    {
                    //       // Progress_Reporter.Show_Progress("Saving Potential Room Nights");
                    //        var plan = new Plan();
                    //        plan.code = string.Empty;
                    //        var Potential = UIProcessManager.GetLookupByKeyword(CNETConstantes.POTENTIAL_ROOM).FirstOrDefault();
                    //        if (Potential != null)
                    //        {
                    //            plan.type = Potential.code;
                    //        }
                    //        plan.description = "Potential Room Night";
                    //        plan.period = LocalBuffer.LocalBuffer.GetPeriodCode(currenDateTime);
                    //        if (tePotentialRoomnights.Text != string.Empty)
                    //        {
                    //            plan.value = Convert.ToDecimal(tePotentialRoomnights.Text);
                    //        }
                    //        var rating = UIProcessManager.GetLookupByKeyword(CNETConstantes.RATING).FirstOrDefault();
                    //        if (rating != null)
                    //        {
                    //            plan.rating = rating.code;
                    //        }
                    //        var planCode = UIProcessManager.CreatePlan(plan);


                    //        if (!string.IsNullOrEmpty(planCode))
                    //        {
                    //            var pDetail = new PlanDetail();

                    //            pDetail.code = string.Empty;
                    //            pDetail.plan = planCode;
                    //            pDetail.Type = CNETConstantes.TAGTYPESCHEDULE;
                    //            pDetail.component = CNETConstantes.ORGANIZATION;
                    //            pDetail.reference = existOrganization.code;
                    //            pDetail.resourceRole = "Customer";
                    //            UIProcessManager.CreatePlanDetail(pDetail);
                    //        }
                    //       ////CNETInfoReporter.Hide();
                    //    }
                    //    if (!string.IsNullOrEmpty(tePotentialRevenue.Text))
                    //    {

                    //        var plan = new Plan();
                    //        plan.code = string.Empty;
                    //        var Potential = UIProcessManager.GetLookupByKeyword(CNETConstantes.POTENTIAL_ROOM).FirstOrDefault();
                    //        if (Potential != null)
                    //        {
                    //            plan.type = Potential.code;
                    //        }
                    //        plan.description = "Potential Revenue";
                    //        plan.period = LocalBuffer.LocalBuffer.GetPeriodCode(currenDateTime);
                    //        if (tePotentialRevenue.Text != string.Empty)
                    //        {
                    //            plan.value = Convert.ToDecimal(tePotentialRevenue.Text);
                    //        }
                    //        var rating = UIProcessManager.GetLookupByKeyword(CNETConstantes.RATING).FirstOrDefault();
                    //        if (rating != null)
                    //        {
                    //            plan.rating = rating.code;
                    //        }
                    //        var planCode = UIProcessManager.CreatePlan(plan);


                    //        if (!string.IsNullOrEmpty(planCode))
                    //        {
                    //            var pDetail = new PlanDetail();

                    //            pDetail.code = string.Empty;
                    //            pDetail.plan = planCode;
                    //            pDetail.Type = CNETConstantes.TAGTYPESCHEDULE;
                    //            pDetail.component = CNETConstantes.ORGANIZATION;
                    //            pDetail.reference = existOrganization.code;
                    //            pDetail.resourceRole = "Customer";
                    //            UIProcessManager.CreatePlanDetail(pDetail);
                    //        }

                    //    }
                    //}
                    SystemMessage.ShowModalInfoMessage(teName.Text + " updated successfully.", "MESSAGE");

                    DialogResult = DialogResult.OK;

                }
                else
                {
                    SystemMessage.ShowModalInfoMessage(teName.Text + " " + "not updated!!!", "ERROR");
                }

                #endregion
            }
            else
            {

                /*************************** CREATEING NEW ORAGANIZATION ****************************/
                #region CREATE NEW
                ConsigneeBuffer organization = new ConsigneeBuffer();
                organization.consignee = new ConsigneeDTO();
                organization.consignee.GslType = GslType;
                organizationList = LocalBuffer.LocalBuffer.AllCustomerConsigneeViewlist.Where(o => o.GslType == GslType).ToList();

                bool isOrgExist = false;

                //preference
                if (cacBusSegment.EditValue != null && !string.IsNullOrEmpty(cacBusSegment.EditValue.ToString()))
                {
                    organization.consignee.Preference = Convert.ToInt32(cacBusSegment.EditValue);
                }
                else
                {
                    var prefernce = LocalBuffer.LocalBuffer.PreferenceBufferList.FirstOrDefault(p => p.SystemConstant == GslType);
                    if (prefernce != null)
                    {

                        organization.consignee.Preference = prefernce.Id;
                    }
                }

                //Check Org Existance
                ConsigneeDTO orgSaved = organizationList.FirstOrDefault(p => p.FirstName == teName.Text && p.Tin == teTINNo.Text);

                DialogResult dr = DialogResult.Yes;
                if (orgSaved != null)
                    isOrgExist = true;

                if (isOrgExist)
                {
                    //  dr = MessageBox.Show(teName.Text +  @" already exists. Do you want continue saving?", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    SystemMessage.ShowModalInfoMessage(teName.Text + " already exists", "ERROR");
                    return;
                }
                else
                {
                    string currentVoCode = UIProcessManager.IdGenerater("Consignee", GslType, 1, LocalBuffer.LocalBuffer.CurrentDeviceConsigneeUnit.Value, false, LocalBuffer.LocalBuffer.CurrentDevice.Id);

                    if (!string.IsNullOrEmpty(currentVoCode))
                    {
                        orgCode = currentVoCode;
                    }
                    else
                    {
                        SystemMessage.ShowModalInfoMessage("There is a problem on id setting!!!", "ERROR");
                        return;
                    }

                    DateTime? currentTime = UIProcessManager.GetServiceTime();

                    //// Progress_Reporter.Show_Progress("Saving Organization");
                    organization.consignee.Code = orgCode;
                    organization.consignee.FirstName = teName.Text;
                    organization.consignee.SecondName = teName.Text;
                    organization.consignee.BusinessType = Convert.ToInt32(cacBusinessType.EditValue);
                    organization.consignee.IsActive = ceIsActive.Checked;
                    organization.consignee.Tin = teTINNo.Text;
                    organization.consignee.Note = note;
                    organization.consignee.DefaultCurrency = Convert.ToInt32(cacCurrency.EditValue);
                    organization.consignee.LastModified = currentTime.Value;
                    organization.consignee.CreatedOn = currentTime.Value;

                    organization.Activity = ActivityLogManager.SetupActivity(currentTime.Value, activityDefCode.Value, CNETConstantes.CONSIGNEE, "");


                    organization.consigneeUnits = new List<ConsigneeUnitDTO>();
                    organization.consigneeUnits = CreateConssigneeUnit();


                    organization.Gsltaxs = new List<GsltaxDTO>();
                    if (lukTaxType.EditValue != null && !string.IsNullOrEmpty(lukTaxType.EditValue.ToString()))
                    {
                        GsltaxDTO gTax = new GsltaxDTO();
                        gTax.Tax = Convert.ToInt32(lukTaxType.EditValue.ToString());
                        organization.Gsltaxs.ToList().Add(gTax);
                    }


                    ConsigneeBuffer savedConsigneeBuffer = UIProcessManager.CreateConsigneeBuffer(organization);

                    if (savedConsigneeBuffer != null)
                    {
                        // LocalBuffer.LocalBuffer.loadOrganization();

                        //saving Address



                        /*
                        //Save Account Map
                        if (cacAccountNo.EditValue != null && !string.IsNullOrEmpty(cacAccountNo.EditValue.ToString()))
                        {
                            GSLAcctRequirement gslAcc =
                                UIProcessManager.SelectGSLAcctRequirement(cacAccountNo.EditValue.ToString());
                            Account acctAccount = new Account();
                            if (gslAcc != null)
                            {
                                acctAccount = UIProcessManager.SelectAllAccount().FirstOrDefault();
                                // UIProcessManager.SelectAllAccount().FirstOrDefault(a => a.controlAccount == gslAcc.contAcct);
                            }
                            if (acctAccount != null)
                            {
                               // Progress_Reporter.Show_Progress("Saving Account Requirement");
                                accMapList = new List<AccountMap>();
                                accountMap.code = string.Empty;
                                accountMap.reference = organization.code;
                                accountMap.description = cacAccountNo.Text; // 
                                accountMap.account = acctAccount.code;
                                accountMap.remark = cacAccountNo.EditValue.ToString();
                                accMapList.Add(accountMap);
                                bool result = UIProcessManager.CreateAccountMap(accMapList);
                                if (result)
                                {
                                    LocalBuffer.LocalBuffer.LoadAccountMap();
                                }
                               ////CNETInfoReporter.Hide();
                            }
                        }

                        //Save ObjectState 
                        if (cacObjectState.EditValue != null && !string.IsNullOrEmpty(cacObjectState.EditValue.ToString()))
                        {
                           // Progress_Reporter.Show_Progress("Saving status");
                            var objList = new List<ObjectState>();
                            var objstaObjectState = new ObjectState();
                            objstaObjectState.code = string.Empty;
                            objstaObjectState.reference = organization.code;
                            objstaObjectState.objectStateDefinition = cacObjectState.EditValue.ToString();
                            objList.Add(objstaObjectState);
                            UIProcessManager.CreateObjectState(objList);
                            LocalBuffer.LocalBuffer.LoadObjectStateTable();
                           ////CNETInfoReporter.Hide();
                        }
                        if (cacMailingAction.EditValue != null && !string.IsNullOrEmpty(cacMailingAction.EditValue.ToString()))
                        {
                           // Progress_Reporter.Show_Progress("Saving Mailing Action");
                            var privacy = new CNETPrivacy();
                            privacy.code = string.Empty;
                            privacy.PrivacyRule = cacMailingAction.EditValue.ToString();
                            privacy.reference = organization.code;
                            UIProcessManager.CreateCNEtPrivacy(privacy);
                           ////CNETInfoReporter.Hide();
                        }
                      
                        if (cacRateCode.EditValue != null && !string.IsNullOrEmpty(cacRateCode.EditValue.ToString()))
                        {
                           // Progress_Reporter.Show_Progress("Saving Rate Code");
                            var negorate = new NegotiatedRate();
                            negorate.code = string.Empty;
                            negorate.consignee = organization.code;
                            negorate.rateCode = cacRateCode.EditValue.ToString();
                            UIProcessManager.CreateNegotiatedRate(negorate);
                           ////CNETInfoReporter.Hide();
                        }

                        if (cacOwner.EditValue != null && !string.IsNullOrEmpty(cacOwner.EditValue.ToString()))
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
                           // Progress_Reporter.Show_Progress("Saving Potential Room Nights");
                            var plan = new Plan();
                            plan.code = string.Empty;
                            var Potential = UIProcessManager.GetLookupByKeyword(CNETConstantes.POTENTIAL_ROOM).FirstOrDefault();
                            if (Potential != null)
                            {
                                plan.type = Potential.code;
                            }
                            plan.description = "Potential Room Night";
                            plan.period = LocalBuffer.LocalBuffer.GetPeriodCode(currenDateTime);
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
                           ////CNETInfoReporter.Hide();
                        }
                        if (!string.IsNullOrEmpty(tePotentialRevenue.Text))
                        {
                            var plan = new Plan();
                            plan.code = string.Empty;
                            var Potential = UIProcessManager.GetLookupByKeyword(CNETConstantes.POTENTIAL_ROOM).FirstOrDefault();
                            if (Potential != null)
                            {
                                plan.type = Potential.code;
                            }
                            plan.description = "Potential Revenue";
                            plan.period = LocalBuffer.LocalBuffer.GetPeriodCode(currenDateTime);
                            if (tePotentialRevenue.Text != string.Empty)
                            {
                                plan.value = Convert.ToDecimal(tePotentialRevenue.Text);
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
                   */
                        ConsigneeDTO savedconsigneeview = UIProcessManager.GetConsigneeById(savedConsigneeBuffer.consignee.Id);
                        LocalBuffer.LocalBuffer.ConsigneeViewBufferList.Add(savedconsigneeview);

                        if (GslType == CNETConstantes.CUSTOMER)
                            LocalBuffer.LocalBuffer.AllCustomerConsigneeViewlist.Add(savedconsigneeview);
                        if (isFromRegisteration)
                        {

                            SavedOrg = savedconsigneeview;
                            isFromRegisteration = false;
                            DialogResult = DialogResult.OK;
                        }

                        this.BeginInvoke((System.Action)(() => SystemMessage.ShowModalInfoMessage("SAVED SUCESSESFULLY", "MESSAGE")));

                        Reset();
                    }
                    else
                    {
                        SystemMessage.ShowModalInfoMessage(teName.Text + " " + "NOT SAVED", "ERROR");


                    }
                }

                #endregion
            }

            AutoCompleteName();
        }// end of method OnSave()


        #endregion

        private List<ConsigneeUnitDTO> CreateConssigneeUnit()
        {
            ConsigneeUnitDTO consignee = new ConsigneeUnitDTO()
            {
                Name = "Head Office",
                CreatedOn = DateTime.Now,
                Phone1 = txtphone1.EditValue == null ? null : txtphone1.EditValue.ToString(),
                Phone2 = txtphone2.EditValue == null ? null : txtphone2.EditValue.ToString(),
                Email = txtemail.EditValue == null ? null : txtemail.EditValue.ToString(),
                Website = txtwebsite.EditValue == null ? null : txtwebsite.EditValue.ToString(),
                PoBox = txtpobox.EditValue == null ? null : txtpobox.EditValue.ToString(),
                Region = sleRegion.EditValue == null ? null : Convert.ToInt32(sleRegion.EditValue.ToString()),
                City = sleCity.EditValue == null ? null : Convert.ToInt32(sleCity.EditValue.ToString()),
                Subcity = sleSubCity.EditValue == null ? null : Convert.ToInt32(sleSubCity.EditValue.ToString()),
                Wereda = txtwereda.EditValue == null ? null : txtwereda.EditValue.ToString(),
                Kebele = txtkebele.EditValue == null ? null : txtkebele.EditValue.ToString(),
                Street = txtstreet.EditValue == null ? null : txtstreet.EditValue.ToString(),
                AddressLine1 = txtaddress1.EditValue == null ? null : txtaddress1.EditValue.ToString(),
                //  AddressLine2 = txtaddress2.EditValue == null ? null : txtaddress2.EditValue.ToString(),
                LastModified = DateTime.Now,
                IsOnline = false,
                Type = CNETConstantes.ORG_UNIT_TYPE_BRUNCH
            };

            return new List<ConsigneeUnitDTO>() { consignee };
        }

        //Event Handlers
        #region Event Handlers 


        private void bbiNote_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmNote frmNote = new frmNote();
            frmNote.NoteContent = note;
            if (frmNote.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                note = frmNote.NoteContent;
            }
        }

        private void bbiNew_ItemClick_1(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Reset();
        }

        private void teTINNo_Leave(object sender, EventArgs e)
        {

        }

        private void frmOrganization_Load(object sender, EventArgs e)
        {
            if (!InitializeData())
            {
                this.Close();
            }
        }

        private void bbiSave_ItemClick_2(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            OnSave();
        }

        private void bbiClose_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Close();
        }

        private void teName_ParseEditValue(object sender, ConvertEditValueEventArgs e)
        {
            if (e.Value != null)
            {
                e.Value = doCapitialize ? e.Value.ToString().ToUpper() : e.Value.ToString();
                e.Handled = true;
            }
        }

        private void teName_TextChanged(object sender, EventArgs e)
        {
            //if (!System.Text.RegularExpressions.Regex.IsMatch(teName.Text, "^[a-zA-Z]"))
            //{
            //    MessageBox.Show(@"This textbox accepts only alphabetical characters");
            // //   teName.Text.Remove(teName.Text.Length - 1);
            //}
        }

        private void cacBusinessType_KeyDown(object sender, KeyEventArgs e)
        {
            LookUpEdit edit = sender as LookUpEdit;
            if (e.KeyData == Keys.Back || e.KeyData == Keys.Delete)
            {
                edit.ClosePopup();
                edit.EditValue = null;

            }
            e.Handled = true;
        }

        private void cacBusSegment_KeyDown(object sender, KeyEventArgs e)
        {
            LookUpEdit edit = sender as LookUpEdit;
            if (e.KeyData == Keys.Back || e.KeyData == Keys.Delete)
            {
                edit.ClosePopup();
                edit.EditValue = null;

            }
            e.Handled = true;
        }

        private void cacAccountNo_KeyDown(object sender, KeyEventArgs e)
        {
            LookUpEdit edit = sender as LookUpEdit;
            if (e.KeyData == Keys.Back || e.KeyData == Keys.Delete)
            {
                edit.ClosePopup();
                edit.EditValue = null;

            }
            e.Handled = true;
        }

        private void cacRateCode_KeyDown(object sender, KeyEventArgs e)
        {
            LookUpEdit edit = sender as LookUpEdit;
            if (e.KeyData == Keys.Back || e.KeyData == Keys.Delete)
            {
                edit.ClosePopup();
                edit.EditValue = null;

            }
            e.Handled = true;
        }

        private void cacObjectState_KeyDown(object sender, KeyEventArgs e)
        {
            LookUpEdit edit = sender as LookUpEdit;
            if (e.KeyData == Keys.Back || e.KeyData == Keys.Delete)
            {
                edit.ClosePopup();
                edit.EditValue = null;

            }
            e.Handled = true;
        }

        private void cacMailingAction_KeyDown(object sender, KeyEventArgs e)
        {
            LookUpEdit edit = sender as LookUpEdit;
            if (e.KeyData == Keys.Back || e.KeyData == Keys.Delete)
            {
                edit.ClosePopup();
                edit.EditValue = null;

            }
            e.Handled = true;
        }

        private void cacCurrency_KeyDown(object sender, KeyEventArgs e)
        {
            LookUpEdit edit = sender as LookUpEdit;
            if (e.KeyData == Keys.Back || e.KeyData == Keys.Delete)
            {
                edit.ClosePopup();
                edit.EditValue = null;

            }
            e.Handled = true;
        }

        private void cacOwner_KeyDown(object sender, KeyEventArgs e)
        {
            LookUpEdit edit = sender as LookUpEdit;
            if (e.KeyData == Keys.Back || e.KeyData == Keys.Delete)
            {
                edit.ClosePopup();
                edit.EditValue = null;

            }
            e.Handled = true;
        }
        private void teName_EditValueChanged_1(object sender, EventArgs e)
        {

        }

        private void teName_Leave(object sender, EventArgs e)
        {
            if (!IsFromDocumentBrowser)
            {
                teTINNo.Text = "";
                ceIsActive.Checked = true;
                cacBusSegment.EditValue = "";
                cacBusinessType.EditValue = "";
                ConsigneeDTO orgSaved = LocalBuffer.LocalBuffer.AllCustomerConsigneeViewlist.FirstOrDefault(p => p.FirstName == teName.Text);
                if (orgSaved != null)
                {
                    PopulateOrganizationFields(orgSaved);
                    rpgAttachment.Visible = true;
                }
            }

        }

        private void teName_InvalidValue(object sender, InvalidValueExceptionEventArgs e)
        {
            e.ErrorText = "Please enter valid name.";
            AutoValidate = AutoValidate.EnableAllowFocusChange;
        }






        private void bbiNew_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Reset();
        }
        private void teTINNo_InvalidValue(object sender, InvalidValueExceptionEventArgs e)
        {
            e.ErrorText = "Please enter valid TIN No.";
            AutoValidate = AutoValidate.EnableAllowFocusChange;
        }


        private void bbiAttachment_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (!string.IsNullOrEmpty(teCorpId.Text))
            {
                //var org = LocalBuffer.LocalBuffer.OrganizationBufferList.FirstOrDefault(o => o.code == teCorpId.Text);
                //if (org == null)
                //{
                //    SystemMessage.ShowModalInfoMessage("Please Select or Save an organization first!", "ERROR");
                //    return;
                //}

                //frmAttachment _frmAttachment = new frmAttachment();
                //_frmAttachment.IsFromProfile = true;
                //_frmAttachment.ConsigneeId = org.code;
                //_frmAttachment.IntType = CNETConstantes.CUSTOMERORG;
                //_frmAttachment.ShowDialog();
            }
            else
            {
                SystemMessage.ShowModalInfoMessage("Please Select organization first!", "ERROR");
                return;
            }
        }


        #endregion






    }
}