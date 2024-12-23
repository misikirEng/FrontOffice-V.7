using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using CNET.ERP.Client.Common.UI;
using CNET.FrontOffice_V._7.PMS.Contracts;
using CNET.FrontOffice_V._7;
using DevExpress.XtraEditors;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using System.Drawing;
using CNET.ERP.Client.Common.UI.Library;
using DevExpress.XtraTreeList;
using System.Windows.Forms;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using CNET.FrontOffice_V._7.Forms.Non_Navigatable_Modals;
using System.Runtime.InteropServices;
using System.IO;
using CNET.ERP.ResourceProvider;
using DevExpress.XtraEditors.Repository;
using System.Globalization;
using System.Drawing.Imaging;
using CNET.FrontOffice_V._7.Forms.State_Change;
using DevExpress.XtraTreeList.Nodes;
using V7PassportOCR;
using System.Threading.Tasks;
using CNET_V7_Domain.Domain.ConsigneeSchema;
using CNET_V7_Domain.Domain.SettingSchema;
using CNET_V7_Domain.Domain.CommonSchema;
using CNET_V7_Domain.Domain.ViewSchema;
using ProcessManager;
using CNET.FrontOffice_V._7.PMS.Common_Classes;
using CNET_V7_Domain.Domain.AccountingSchema;
using CNET_V7_Domain.Domain.PmsSchema;
using CNET.FrontOffice_V._7.Validation;
using V7PassportOCR.PassportCharactes;
using CNET_V7_Domain.Domain.ArticleSchema;
using CNET.FrontOffice_V._7.Group_Registration;
using CNET.FrontOffice_V._7.Non_Navigatable_Modals;
using V7PassportOCR;
using static DevExpress.Xpo.Helpers.AssociatedCollectionCriteriaHelper;
using Pr22.Processing;
using Pr22.Task;
using Pr22;

namespace CNET.FrontOffice_V._7.Forms
{
    public partial class frmPerson : UILogicBase
    // public partial class frmPerson : XtraForm
    {
        #region Passport scanner field

        Pr22.DocumentReaderDevice pr = null;
        Pr22.Processing.Document AnalyzeResult;
        Pr22.Processing.Document MrzDoc;
        bool m_bIsIDCardLoaded = false;

        private System.Windows.Forms.Timer _wintoneTimer = null;

        List<ConfigurationDTO> passportConfigList = null;

        #endregion


        #region Lookup Fields


        List<LookupDTO> personTitleList = null;
        List<SystemConstantDTO> genderList = null;
        List<LookupDTO> idTypeList = null;
        List<LookupDTO> mailingActionList = null;
        List<CurrencyDTO> currencyList = null;
        List<CountryDTO> countryList = null;
        List<LanguageDTO> languageList = null;
        List<LookupDTO> regligionList = null;
        List<ConsigneeDTO> _personList = new List<ConsigneeDTO>();

        #endregion

        private string wintonePassportImageFullPath;
        private List<string> lastNameList, firstNameList, middleNameList;

        private List<PersonSearchVM> _pSearchVMList = new List<PersonSearchVM>();

        private ConsigneeDTO savedPerson;

        private frmAccompanyingGuest accForm = new frmAccompanyingGuest();

        private DeviceDTO device = null;
        private bool isARH = false;
        private bool isFlatBedScanner = false;
        private DeviceDTO scanDevice = null;

        private string imageName = "";
        private string path = "";

        private int activityDefCode;
        private bool doCapitialize = false;

        private string note = "";
        private UILogicBase requesterForm;

        private int _defTitle;
        string TINNumber { get; set; }

        private void GetTINNumber()
        {
            ConsigneeDTO CompanyOrg = LocalBuffer.LocalBuffer.ConsigneeViewBufferList.FirstOrDefault(x => x.GslType == CNETConstantes.COMPANY);
            if (CompanyOrg != null)
            {
                TINNumber = CompanyOrg.Tin;
            }
        }

        private ActivityDTO _activity = null;

        ///////////////////////////// CONSTRUCTOR ///////////////////////////////////
        public frmPerson(string perType)
        {
            this.Text = perType;
            InitializeComponent();
            InitializeUI();
            ApplyIcons();
            GetTINNumber();

            //   GetData();
        }

        #region Properties

        //public override sealed string Text
        //{
        //    get { return base.Text; }
        //    set { base.Text = value; }
        //}

        public ConsigneeDTO SavedPerson
        {
            get
            {
                return savedPerson;
            }
            set
            {
                savedPerson = value;
            }
        }

        private ConsigneeDTO _personToEdit;
        public ConsigneeDTO PersonToEdit
        {
            get
            {
                return _personToEdit;
            }
            set
            {
                _personToEdit = value;
            }
        }

        public bool isFromAccGuest { get; set; }
        public bool isFromRegisteration { get; set; }
        public bool isFromDocumentBrowser { get; set; }

        public string RegistrationVoucher { get; set; }

        public int GSLType { get; set; }

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

        #endregion

        #region Methods

        // Initialize UI
        public void InitializeUI()
        {
            FormSize = new Size(840, 530);
            beiCode.EditValue = "";
            this.StartPosition = FormStartPosition.CenterParent;
            CNETFooterRibbon.ribbonControl = ribbonControl1;


            //Language 
            cacLanguage.Properties.Columns.Add(new LookUpColumnInfo("Description", "Description"));
            cacLanguage.Properties.DisplayMember = "Description";
            cacLanguage.Properties.ValueMember = "Id";

            //Title
            cacTitle.Properties.Columns.Add(new LookUpColumnInfo("Description", "Description"));
            cacTitle.Properties.DisplayMember = "Description";
            cacTitle.Properties.ValueMember = "Id";

            //Id Type
            leIDType.Properties.Columns.Add(new LookUpColumnInfo("Description", "ID Type"));
            leIDType.Properties.DisplayMember = "Description";
            leIDType.Properties.ValueMember = "Id";

            //Currency
            cacCurrency.Properties.Columns.Add(new LookUpColumnInfo("Description", "Description"));
            cacCurrency.Properties.DisplayMember = "Description";
            cacCurrency.Properties.ValueMember = "Id";

            //Currency
            lukTaxType.Properties.Columns.Add(new LookUpColumnInfo("Description", "Description"));
            lukTaxType.Properties.DisplayMember = "Description";
            lukTaxType.Properties.ValueMember = "Id";


            //Business Segment
            cacBusinessSegment.Properties.Columns.Add(new LookUpColumnInfo("Description", "Bus. Segment"));
            cacBusinessSegment.Properties.DisplayMember = "Description";
            cacBusinessSegment.Properties.ValueMember = "Id";

            //Mailing Action
            cacMailingAction.Properties.Columns.Add(new LookUpColumnInfo("Description", "Mailing Methods"));
            cacMailingAction.Properties.DisplayMember = "Description"; cacMailingAction.Properties.ValueMember = "Id";

            //Gender List
            cacGender.Properties.Columns.Add(new LookUpColumnInfo("Description", "Sex"));
            cacGender.Properties.DisplayMember = "Description";
            cacGender.Properties.ValueMember = "Id";

            //Country List
            //GridColumn column = countryLookUpEdit.View.Columns.AddField("name");
            //column.Visible = true;
            //column = countryLookUpEdit.View.Columns.AddField("ICAOCountryCode");
            //column.Visible = true;
            //column = countryLookUpEdit.View.Columns.AddField("nationality");
            //column.Visible = true;
            //countryLookUpEdit.DisplayMember = "name";
            //countryLookUpEdit.ValueMember = "name";
            //countryLookUpEdit.NullText = "";

            //Nationlity
            // CountryDTO
            cacNationality.Properties.Columns.Add(new LookUpColumnInfo("Name", "Country"));
            cacNationality.Properties.Columns.Add(new LookUpColumnInfo("Nationality", "Nationality"));
            cacNationality.Properties.DisplayMember = "Nationality";
            cacNationality.Properties.ValueMember = "Id";

            //Rate Code List
            cacRateCode.Properties.Columns.Add(new LookUpColumnInfo("Id", "Rate Code"));
            cacRateCode.Properties.Columns.Add(new LookUpColumnInfo("Category", "Category"));
            cacRateCode.Properties.Columns.Add(new LookUpColumnInfo("Currency", "Currency"));
            cacRateCode.Properties.DisplayMember = "Id";
            cacRateCode.Properties.ValueMember = "Id";

            //VIP list
            cacObjectState.Properties.Columns.Add(new LookUpColumnInfo("Description", "Status"));
            cacObjectState.Properties.DisplayMember = "Description";
            cacObjectState.Properties.ValueMember = "Id";

            //Account List
            cacAccount.Properties.Columns.Add(new LookUpColumnInfo("contAcct", "Account"));
            cacAccount.Properties.DisplayMember = "contAcct";
            cacAccount.Properties.ValueMember = "Id";

            //Religion List
            luk_religion.Properties.Columns.Add(new LookUpColumnInfo("Description", "Status"));
            luk_religion.Properties.DisplayMember = "Description";
            luk_religion.Properties.ValueMember = "Id";
        }

        // Initialize Data
        public bool InitializeData()
        {
            try
            {
                // Progress_Reporter.Show_Progress("Loading Data...");

                device = LocalBuffer.LocalBuffer.CurrentDevice;
                string currentVoCode = null;


                if (GSLType == CNETConstantes.CONTACT)
                {
                    currentVoCode = UIProcessManager.IdGenerater("Consignee", CNETConstantes.CONTACT, 0, LocalBuffer.LocalBuffer.CurrentDeviceConsigneeUnit.Value, false, LocalBuffer.LocalBuffer.CurrentDevice.Id);

                    if (!string.IsNullOrEmpty(currentVoCode))
                    {
                        if (beiCode.EditValue == null || string.IsNullOrEmpty(beiCode.EditValue.ToString()))
                            beiCode.EditValue = currentVoCode;

                    }
                    else
                    {
                        SystemMessage.ShowModalInfoMessage("There is a problem on id setting!!!", "ERROR");
                        ////CNETInfoReporter.Hide();
                        return false;
                    }

                }
                else
                {

                    currentVoCode = UIProcessManager.IdGenerater("Consignee", CNETConstantes.GUEST, 0, LocalBuffer.LocalBuffer.CurrentDeviceConsigneeUnit.Value, false, LocalBuffer.LocalBuffer.CurrentDevice.Id);

                    if (!string.IsNullOrEmpty(currentVoCode))
                    {
                        if (beiCode.EditValue == null || string.IsNullOrEmpty(beiCode.EditValue.ToString()))
                            beiCode.EditValue = currentVoCode;
                    }
                    else
                    {
                        SystemMessage.ShowModalInfoMessage("There is a problem on id setting!!!", "ERROR");
                        ////CNETInfoReporter.Hide();
                        return false;
                    }


                }


                if (PersonToEdit != null)
                {
                    ActivityDefinitionDTO workFlow = UIProcessManager.GetActivityDefinitionBydescriptionandreference(CNETConstantes.Activity_Updated, GSLType).FirstOrDefault();

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
                        var roleActivity = UIProcessManager.GetRoleActivityByactivityDefinition(activityDefCode).FirstOrDefault(r => r.Role == userRoleMapper.Role && r.NeedsPassCode);
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
                    ActivityDefinitionDTO workFlow = UIProcessManager.GetActivityDefinitionBydescriptionandreference(CNETConstantes.MAINTAINED, GSLType).FirstOrDefault();

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
                        var roleActivity = UIProcessManager.GetRoleActivityByactivityDefinition(activityDefCode).FirstOrDefault(r => r.Role == userRoleMapper.Role && r.NeedsPassCode);
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

                //create default required gsl list
                //if (!CreateDefaultRequiredGsl())
                //{
                //   ////CNETInfoReporter.Hide();
                //    return false;
                //}

                // Load Person data
                if (GSLType == CNETConstantes.CONTACT)
                    _personList = LocalBuffer.LocalBuffer.AllGuestConsigneeViewlist.Where(p => p.GslType == CNETConstantes.CONTACT).ToList();
                else
                    _personList = LocalBuffer.LocalBuffer.AllGuestConsigneeViewlist.Where(p => p.GslType == CNETConstantes.GUEST).ToList();

                // setup autocomplete names
                AutoCompleteNames();

                DateTime? CurrentTime = UIProcessManager.GetServiceTime();
                if (CurrentTime == null)
                {
                    ////CNETInfoReporter.Hide();
                    return false;
                }

                deDateOfBirth.Properties.MaxValue = CurrentTime.Value;
                deDateOfBirth.DateTime = CurrentTime.Value.Date;
                ceIsActive.Checked = true;



                //Load Lookup Data
                personTitleList = LocalBuffer.LocalBuffer.LookUpBufferList.Where(l => l.Type == CNETConstantes.PERSONALTITLE && l.IsActive).ToList();
                genderList = LocalBuffer.LocalBuffer.SystemConstantDTOBufferList.Where(l => l.Category == CNETConstantes.GENDER && l.IsActive).ToList();
                idTypeList = LocalBuffer.LocalBuffer.LookUpBufferList.Where(l => l.Type == CNETConstantes.PersonalIdentificationType && l.IsActive).ToList();
                currencyList = LocalBuffer.LocalBuffer.CurrencyBufferList;
                countryList = LocalBuffer.LocalBuffer.CountryBufferList;
                mailingActionList = LocalBuffer.LocalBuffer.LookUpBufferList.Where(l => l.Type == CNETConstantes.PRIVACYRULE && l.IsActive).ToList();
                regligionList = LocalBuffer.LocalBuffer.LookUpBufferList.Where(l => l.Type == CNETConstantes.RELIGION && l.IsActive).ToList();

                //Tax Type
                List<TaxDTO> taxList = LocalBuffer.LocalBuffer.TaxBufferList;
                lukTaxType.Properties.DataSource = taxList;
                lukTaxType.EditValue = CNETConstantes.VAT;

                //mailing action
                if (mailingActionList != null)
                    cacMailingAction.Properties.DataSource = (mailingActionList.OrderByDescending(c => c.IsDefault).ToList());


                //Assign Lookup's DataSource
                if (personTitleList != null)
                {
                    cacTitle.Properties.DataSource = (personTitleList.OrderByDescending(c => c.IsDefault).ToList());

                    //Default Title
                    var defTitle = personTitleList.FirstOrDefault(p => p.IsDefault);
                    if (defTitle != null)
                    {
                        _defTitle = defTitle.Id;
                        cacTitle.EditValue = _defTitle;
                    }
                }
                cacLanguage.Properties.DataSource = UIProcessManager.SelectAllLanguage();
                //countryLookUpEdit.DataSource = countryList;
                cacNationality.Properties.DataSource = countryList;

                //ID type list
                if (idTypeList != null)
                {
                    leIDType.Properties.DataSource = (idTypeList.OrderByDescending(c => c.IsDefault).ToList());
                    var defualtID = idTypeList.FirstOrDefault(c => c.IsDefault);
                    if (defualtID != null)
                    {
                        leIDType.EditValue = (defualtID.Id);
                    }
                }

                //Currency
                if (currencyList != null)
                {
                    cacCurrency.Properties.DataSource = (currencyList.OrderByDescending(c => c.IsDefault).ToList());
                    var currency = currencyList.FirstOrDefault(c => c.IsDefault);
                    if (currency != null)
                    {
                        cacCurrency.EditValue = (currency.Id);
                    }

                }

                //religion
                if (regligionList != null)
                {
                    luk_religion.Properties.DataSource = (regligionList.OrderByDescending(c => c.IsDefault).ToList());
                    var religion = regligionList.FirstOrDefault(c => c.IsDefault);
                    if (religion != null)
                    {
                        luk_religion.EditValue = (religion.Id);
                    }

                }

                //business segment list
                List<PreferenceDTO> businessSegmentList = businessSegmentList = LocalBuffer.LocalBuffer.PreferenceBufferList.Where(p => p.SystemConstant == GSLType).ToList();
                cacBusinessSegment.Properties.DataSource = businessSegmentList;

                if (businessSegmentList != null && businessSegmentList.Count > 0)
                    cacBusinessSegment.EditValue = businessSegmentList.FirstOrDefault().Id;

                //gender list
                if (genderList != null)
                {
                    cacGender.Properties.DataSource = genderList;
                    var defaultSex = genderList.FirstOrDefault(c => c.IsDefault);
                    if (defaultSex != null)
                    {
                        cacGender.EditValue = (defaultSex.Id);
                    }
                }

                //rate code list
                // var rateCodeList = UIProcessManager.GetRateCodeCategoryCurrency();
                // cacRateCode.Properties.DataSource = (rateCodeList);

                //for Object state list
                var objectStateDefinitions = LocalBuffer.LocalBuffer.ObjectStateDefinitionBufferList.Where(s => s.Category == "Consignee").ToList();
                cacObjectState.Properties.DataSource = (objectStateDefinitions);
                if (objectStateDefinitions != null && objectStateDefinitions.Count > 0)
                {
                    cacObjectState.EditValue = objectStateDefinitions.FirstOrDefault().Id;
                }

                //account List
                var accountList = new List<GslacctRequirementDTO>();
                accountList = UIProcessManager.GetGSLAcctRequirementBygslTypeList(GSLType);
                if (accountList != null)
                {
                    foreach (var acc in accountList)
                    {
                        ControlAccountDTO ACC = UIProcessManager.GetControlAccountByid(acc.ContAcct);
                        // acc.ContAcct = ACC != null ? ACC.Description : String.Empty;
                    }

                }

                cacAccount.Properties.DataSource = (accountList);

                //read configuration setting for capitialization
                var Caseconfig = LocalBuffer.LocalBuffer.ConfigurationBufferList.FirstOrDefault(c => c.Reference == GSLType.ToString() && c.Attribute == "Maintain Case");
                if (Caseconfig != null)
                {
                    if (Caseconfig.CurrentValue == "UpperCase")
                        doCapitialize = true;

                }

                //Address Map
                //List<GSLAddressMap> addresmMaps = new List<GSLAddressMap>();
                //addresmMaps = UIProcessManager.GetGSLAddressMap(GSLType);

                //if (addresmMaps.Count > 0)
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

                //        addressDTOList.Add(aDto);
                //    }
                //}




                //Populate If It is Editing
                if (PersonToEdit != null)
                {
                    VwConsigneeViewDTO selected = UIProcessManager.GetConsigneeViewById(PersonToEdit.Id);
                    PopulateAllPersonFields(selected);
                }



                #region Setup Passport Scanner
                try
                {
                    bool isConnected = false;

                    if (checkPassportScanner())
                    {
                        //Connect to passport scanner
                        //Progress_Reporter.Show_Progress("Connecting to passport scanner...");
                        pr = new Pr22.DocumentReaderDevice();
                        isConnected = ConnectPassportScanner();
                    }


                    if (!isConnected)
                    {
                        toolStripStatusLabel1.Text = "passport scanner device is not detected!";
                        //XtraMessageBox.Show("No passport scanner device is connected!", "CNET_v2016", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        bbiScanFingerPrint.Enabled = false;
                        scanDevice = null;
                    }
                    else
                    {
                        //if (!isARH)
                        //bbiScanFingerPrint.Enabled = false;
                    }

                    if (scanDevice != null)
                    {
                        passportConfigList = LocalBuffer.LocalBuffer.ConfigurationBufferList.Where(c => c.Reference == scanDevice.Id.ToString()).ToList();
                        if (passportConfigList != null)
                        {
                            ConfigurationDTO config = passportConfigList.Where(pc => pc.Attribute == "Image URL").FirstOrDefault();
                            path = config != null ? config.CurrentValue.ToString() : "";
                        }
                    }

                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show("ERROR! " + ex.Message, "CNET_v2016", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    bbiScanFingerPrint.Enabled = false;
                }

                #endregion

                ////CNETInfoReporter.Hide();
                return true;
            }
            catch (Exception ex)
            {
                ////CNETInfoReporter.Hide();
                XtraMessageBox.Show("Error in initializing data. DETAIL: " + ex.Message, "CNET_v2016", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private List<LanguageDTO> GetLanguage()
        {
            return UIProcessManager.SelectAllLanguage();
        }


        // create default required gsl for other consignees
        //private bool CreateDefaultRequiredGsl()
        //{
        //    try
        //    {
        //        bool flag = true;
        //        var reqGslList = LocalBuffer.LocalBuffer.RequiredGSLBufferList.Where(rg => rg.voucherDefn == CNETConstantes.REGISTRATION_VOUCHER).ToList();
        //        if (reqGslList == null || reqGslList.Count == 0)
        //        {
        //            //create new ones
        //            int count = 6;
        //            for (int i = 0; i < count; i++)
        //            {

        //                if (i == 0)
        //                {
        //                    flag = CreateReqGsl(i, CNETConstantes.REQ_GSL_AGENT, "Agent");

        //                }
        //                else if (i == 1)
        //                {
        //                    flag = CreateReqGsl(i, CNETConstantes.REQ_GSL_SOURCE, "Business Source");
        //                }

        //                else if (i == 2)
        //                {
        //                    flag = CreateReqGsl(i, CNETConstantes.REQ_GSL_GROUP, "Group");
        //                }

        //                else if (i == 3)
        //                {
        //                    flag = CreateReqGsl(i, CNETConstantes.REQ_GSL_COMPANY, "Company");
        //                }

        //                else if (i == 4)
        //                {
        //                    flag = CreateReqGsl(i, CNETConstantes.REQ_GSL_CONTACT, "Contact");
        //                }
        //                else if (i == 5)
        //                {
        //                    flag = CreateReqGsl(i, CNETConstantes.ACCOMPANYING_GUEST_REQUIRED_GSL_CODE, "Accompany Guest");
        //                }

        //                if (!flag)
        //                    break;

        //            }//end of for loop
        //            if (!flag)
        //            {
        //                SystemMessage.ShowModalInfoMessage("Unable to create default required gsl!", "ERROR");
        //            }
        //            else
        //            {
        //                LocalBuffer.LocalBuffer.loadRequiredGSL();
        //            }
        //            return flag;
        //        }
        //        else
        //        {
        //            //check agent
        //            var agent = reqGslList.FirstOrDefault(r => r.code == CNETConstantes.REQ_GSL_AGENT);
        //            if (agent == null)
        //            {
        //                flag = CreateReqGsl(0, CNETConstantes.REQ_GSL_AGENT, "Agent");
        //            }

        //            if (flag)
        //            {
        //                //check source
        //                var source = reqGslList.FirstOrDefault(r => r.code == CNETConstantes.REQ_GSL_SOURCE);
        //                if (source == null)
        //                {
        //                    flag = CreateReqGsl(1, CNETConstantes.REQ_GSL_SOURCE, "Business Source");
        //                }
        //            }

        //            if (flag)
        //            {
        //                //check group
        //                var group = reqGslList.FirstOrDefault(r => r.code == CNETConstantes.REQ_GSL_GROUP);
        //                if (group == null)
        //                {
        //                    flag = CreateReqGsl(2, CNETConstantes.REQ_GSL_GROUP, "Group");
        //                }
        //            }

        //            if (flag)
        //            {

        //                //check company
        //                var company = reqGslList.FirstOrDefault(r => r.code == CNETConstantes.REQ_GSL_COMPANY);
        //                if (company == null)
        //                {
        //                    flag = CreateReqGsl(3, CNETConstantes.REQ_GSL_COMPANY, "Company");
        //                }
        //            }

        //            if (flag)
        //            {

        //                //check contact
        //                var contact = reqGslList.FirstOrDefault(r => r.code == CNETConstantes.REQ_GSL_CONTACT);
        //                if (contact == null)
        //                {
        //                    flag = CreateReqGsl(4, CNETConstantes.REQ_GSL_CONTACT, "Contact");
        //                }

        //            }

        //            if (flag)
        //            {

        //                //check contact
        //                var accGuest = reqGslList.FirstOrDefault(r => r.code == CNETConstantes.ACCOMPANYING_GUEST_REQUIRED_GSL_CODE);
        //                if (accGuest == null)
        //                {
        //                    flag = CreateReqGsl(5, CNETConstantes.ACCOMPANYING_GUEST_REQUIRED_GSL_CODE, "Accompany Guest");
        //                }

        //            }

        //            if (!flag)
        //            {
        //                SystemMessage.ShowModalInfoMessage("Unable to create default required gsl!", "ERROR");
        //            }
        //            else
        //            {
        //                LocalBuffer.LocalBuffer.loadRequiredGSL();
        //            }
        //            return flag;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        SystemMessage.ShowModalInfoMessage("Error to create default required gsl codes! Detail:: " + ex.Message, "ERROR");
        //        return false;
        //    }

        //}

        //private bool CreateReqGsl(int index, string code, string description)
        //{
        //    RequiredGslDTO rGsl = new RequiredGslDTO()
        //    { 
        //        VoucherDefn = CNETConstantes.REGISTRATION_VOUCHER,
        //        Type = CNETConstantes.VOUCHER_CONSIGNEE,
        //        Index = (byte)index,
        //        Description = description
        //    };

        //    return UIProcessManager.CreateRequiredGSL(rGsl);
        //}

        //filter person
        private void FilterPerson()
        {
            if (_personList == null) return;

            //List<vw_PersonView> filterd = new List<vw_PersonView>();
            List<ConsigneeDTO> filterd = new List<ConsigneeDTO>();
            string nameFilter = string.Empty;
            gc_searchPerson.DataSource = null;

            if (!string.IsNullOrEmpty(teLastName.Text) && !string.IsNullOrEmpty(teFirstName.Text) &&
                !string.IsNullOrEmpty(teMiddle.Text))
            {
                filterd = _personList.Where(p => (p.ThirdName != null && p.ThirdName.ToLower().Contains(teLastName.Text.ToLower())) &&
                   (p.FirstName != null && p.FirstName.ToLower().Contains(teFirstName.Text.ToLower())) &&
                   (p.SecondName != null && p.SecondName.ToLower().Contains(teMiddle.Text.ToLower()))).ToList();
            }
            else if (!string.IsNullOrEmpty(teLastName.Text) && string.IsNullOrEmpty(teFirstName.Text) &&
                     string.IsNullOrEmpty(teMiddle.Text))
            {
                filterd = _personList.Where(p => (p.ThirdName != null && p.ThirdName.ToLower().Contains(teLastName.Text.ToLower()))
                    ).ToList();
            }
            else if (!string.IsNullOrEmpty(teLastName.Text) && !string.IsNullOrEmpty(teFirstName.Text) &&
                     string.IsNullOrEmpty(teMiddle.Text))
            {
                filterd = _personList.Where(p => (p.ThirdName != null && p.ThirdName.ToLower().Contains(teLastName.Text.ToLower())) &&
                  (p.FirstName != null && p.FirstName.ToLower().Contains(teFirstName.Text.ToLower()))
                    ).ToList();
            }
            else if (!string.IsNullOrEmpty(teLastName.Text) && string.IsNullOrEmpty(teFirstName.Text) &&
                     !string.IsNullOrEmpty(teMiddle.Text))
            {
                filterd = _personList.Where(p => (p.ThirdName != null && p.ThirdName.ToLower().Contains(teLastName.Text.ToLower())) &&
                   (p.SecondName != null && p.SecondName.ToLower().Contains(teMiddle.Text.ToLower()))
                    ).ToList();
            }
            else if (string.IsNullOrEmpty(teLastName.Text) && !string.IsNullOrEmpty(teFirstName.Text) &&
                     !string.IsNullOrEmpty(teMiddle.Text))
            {
                filterd = _personList.Where(p =>
                  (p.FirstName != null && p.FirstName.ToLower().Contains(teFirstName.Text.ToLower())) &&
                    (p.SecondName != null && p.SecondName.ToLower().Contains(teMiddle.Text.ToLower()))
                     ).ToList();
            }
            else if (string.IsNullOrEmpty(teLastName.Text) && string.IsNullOrEmpty(teFirstName.Text) &&
                     !string.IsNullOrEmpty(teMiddle.Text))
            {
                filterd = _personList.Where(p =>
                    (p.SecondName != null && p.SecondName.ToLower().Contains(teMiddle.Text.ToLower()))
                    ).ToList();
            }
            else if (string.IsNullOrEmpty(teLastName.Text) && !string.IsNullOrEmpty(teFirstName.Text) &&
                     string.IsNullOrEmpty(teMiddle.Text))
            {
                filterd = _personList.Where(p =>
                   (p.FirstName != null && p.FirstName.ToLower().Contains(teFirstName.Text.ToLower()))
                    ).ToList();
            }
            else
            {
                nameFilter = string.Empty;
            }
            // nameFilter = nameFilter.ToLower().Trim();

            //filterd = _personList.Where(p => p.name.ToLower().Contains(nameFilter)).ToList();
            if (filterd == null) return;

            _pSearchVMList.Clear();
            foreach (var p in filterd)
            {
                PersonSearchVM pVM = new PersonSearchVM()
                {
                    Id = p.Id,
                    Code = p.Code,
                    FirstName = p.FirstName,
                    LastName = p.ThirdName,
                    MiddleName = p.SecondName
                };

                _pSearchVMList.Add(pVM);
            }

            //bind to grid
            gc_searchPerson.BeginUpdate();
            gc_searchPerson.DataSource = _pSearchVMList;
            gc_searchPerson.Refresh();
            gc_searchPerson.EndUpdate();
        }

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


            Image = Provider.GetImage("Search", ProviderType.APPLICATIONICON, PictureSize.Dimension_32X32);

            bbiSearch.Glyph = Image;
            bbiSearch.LargeGlyph = Image;

            Image = Provider.GetImage("Add", ProviderType.APPLICATIONICON, PictureSize.Dimension_32X32);

            bbiAddGuest.Glyph = Image;
            bbiAddGuest.LargeGlyph = Image;

        }

        private async void PopulateAllPersonFields(VwConsigneeViewDTO focusedPerson)
        {
            if (focusedPerson != null)
            {
                beiCode.EditValue = focusedPerson.Code;
                teLastName.Text = focusedPerson.ThirdName;
                teFirstName.Text = focusedPerson.FirstName;
                teMiddle.Text = focusedPerson.SecondName;
                deDateOfBirth.EditValue = focusedPerson.StartDate;
                cacNationality.EditValue = focusedPerson.NationalityId;
                cacTitle.EditValue = focusedPerson.Title;
                cacGender.EditValue = focusedPerson.Gender;
                ceIsActive.Checked = focusedPerson.ConsigneeIsActive;
                cacBusinessSegment.EditValue = focusedPerson.ChildpreferenceId;
                // luk_religion.EditValue = focusedPerson.;

                //load load attachment 
                try
                {
                    bool Exist = FTPInterface.FTPAttachment.InitalizePMSFTPAttachment(LocalBuffer.LocalBuffer.CompanyConsigneeData.Tin);
                    if (Exist && !string.IsNullOrEmpty(focusedPerson.ConsigneeImageUrl))
                    {
                        pePassPortImage.Image = FTPInterface.FTPAttachment.GetImageFromFTP(focusedPerson.ConsigneeImageUrl);
                        pePassPortImage.Refresh();
                    }
                    
                }
                catch (Exception ex) { }


                //load gsl tax list
                GsltaxDTO gTax = UIProcessManager.GetGSLTaxByReference(focusedPerson.Id);
                if (gTax != null)
                {
                    lukTaxType.EditValue = gTax.Tax;
                }

                //load note 
                if (focusedPerson.Note != null)
                    note = focusedPerson.Note;

                //load identification
                if (!string.IsNullOrEmpty(focusedPerson.PassportId))
                {
                    leIDType.EditValue = CNETConstantes.PERSONAL_IDENTIFICATIONTYPE_PASPORT;
                    tePassportNumber.Text = focusedPerson.PassportId;
                }

                if (focusedPerson.DefaultLanguage != null)
                    cacLanguage.EditValue = focusedPerson.DefaultLanguage;


                cacCurrency.EditValue = focusedPerson.DefaultCurrency;

                //CNETPrivacy cnetPrivacy = UIProcessManager.SelectAllCNETPrivacy().FirstOrDefault(c => c.reference == focusedPerson.code);
                //if (cnetPrivacy != null)
                //{
                //    cacMailingAction.EditValue = cnetPrivacy.PrivacyRule;
                //}

                NegotiationRateDTO nerateNegotiatedRate = UIProcessManager.SelectAllNegotiationRate().FirstOrDefault(n => n.Consignee == focusedPerson.Id);
                if (nerateNegotiatedRate != null)
                {
                    cacRateCode.EditValue = nerateNegotiatedRate.RateCode;
                }
                ObjectStateDTO objectState = UIProcessManager.GetObjectStateByReference(focusedPerson.Id);

                if (objectState != null)
                {
                    cacObjectState.EditValue = objectState.ObjectStateDefinition;
                }
                AccountMapDTO accMap = UIProcessManager.GetAccountMapByreferencefirstordefault(focusedPerson.Id);
                if (accMap != null)
                {
                    cacAccount.EditValue = accMap.Remark;
                }
                //List<Address> addresses = LocalBuffer.LocalBuffer.AddressBufferList.Where(r => r.reference == focusedPerson.code).ToList();
                //List<AddressDTO> currentList = tList_Address.DataSource as List<AddressDTO>;
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
                //tList_Address.DataSource = currentList;
                //tList_Address.RefreshDataSource();
                //tList_Address.ExpandAll();


                txtphone1.EditValue = focusedPerson.Phone1;
                txtphone2.EditValue = focusedPerson.Phone2;
                txtemail.EditValue = focusedPerson.Email;
                txtwebsite.EditValue = focusedPerson.Website;
                txtpobox.EditValue = focusedPerson.PoBox;
                sleRegion.EditValue = focusedPerson.Region;
                sleCity.EditValue = focusedPerson.City;
                sleSubCity.EditValue = focusedPerson.Subcity;
                txtkebele.EditValue = focusedPerson.Kebele;
                txtstreet.EditValue = focusedPerson.Street;
                txtaddress1.EditValue = focusedPerson.AddressLine1;


            }
        }

        private void AutoCompleteNames()
        {
            if (_personList == null) return;
            lastNameList = _personList.Where(x => x.ThirdName != null).Select(p => doCapitialize ? p.ThirdName.ToUpper() : p.ThirdName).ToList();
            firstNameList = _personList.Where(x => x.FirstName != null).Select(p => doCapitialize ? p.FirstName.ToUpper() : p.FirstName).ToList();
            middleNameList = _personList.Where(x => x.SecondName != null).Select(p => doCapitialize ? p.SecondName.ToUpper() : p.SecondName).ToList();
            List<string> allNames = new List<string>();
            allNames.AddRange(lastNameList);
            allNames.AddRange(firstNameList);
            allNames.AddRange(middleNameList);

            AutoCompleteStringCollection collection = new AutoCompleteStringCollection();
            collection.AddRange(allNames.ToArray());
            teLastName.MaskBox.AutoCompleteSource = AutoCompleteSource.CustomSource;
            teLastName.MaskBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            teLastName.MaskBox.AutoCompleteCustomSource = collection;

            AutoCompleteStringCollection fNamecollection = new AutoCompleteStringCollection();
            fNamecollection.AddRange(allNames.ToArray());
            teFirstName.MaskBox.AutoCompleteSource = AutoCompleteSource.CustomSource;
            teFirstName.MaskBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            teFirstName.MaskBox.AutoCompleteCustomSource = fNamecollection;

            AutoCompleteStringCollection mNamecollection = new AutoCompleteStringCollection();
            mNamecollection.AddRange(allNames.ToArray());
            teMiddle.MaskBox.AutoCompleteSource = AutoCompleteSource.CustomSource;
            teMiddle.MaskBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            teMiddle.MaskBox.AutoCompleteCustomSource = mNamecollection;
        }

        public void OnCreate()
        {
        }

        private void ResetFields()
        {
            DateTime? CurrentTime = UIProcessManager.GetServiceTime();
            teFirstName.Text = "";
            teMiddle.Text = "";
            teLastName.Text = "";
            tePassportNumber.Text = "";
            ceIsActive.Checked = true;
            deDateOfBirth.DateTime = CurrentTime == null ? DateTime.Now : CurrentTime.Value;
            cacNationality.EditValue = "";
            cacBusinessSegment.EditValue = "";
            cacLanguage.EditValue = "";
            cacAccount.EditValue = "";
            cacObjectState.EditValue = "";
            cacRateCode.EditValue = "";
            cacMailingAction.EditValue = "";
            cacTitle.EditValue = string.Empty;

            pePassPortImage.Image = null;
            //pePassPortImage.
            // pePassPortImage.
            pePassPortImage.Refresh();
            ////var defaultTit = personTitleList.FirstOrDefault(c => c.isDefault);
            ////if (defaultTit != null)
            ////{
            ////    cacTitle.EditValue = (defaultTit.code);
            ////}

            var defaultSex = genderList.FirstOrDefault(c => c.IsDefault);
            if (defaultSex != null)
            {
                cacGender.EditValue = (defaultSex.Id);
            }

            var currency = currencyList.FirstOrDefault(c => c.IsDefault);
            if (currency != null)
            {
                cacCurrency.EditValue = (currency.Id);
            }
            var defualtID = idTypeList.FirstOrDefault(c => c.IsDefault);

            if (defualtID != null)
            {
                leIDType.EditValue = (defualtID.Id);
            }

            //this.Refresh();
        }

        public override void Reset()
        {
            AutoCompleteNames();
            base.Reset();

            rpgAttachment.Visible = false;
            cacTitle.EditValue = _defTitle;

            string currentVoCode = null;
            teLastName.Text = "";
            if (GSLType == CNETConstantes.CONTACT)
            {

                currentVoCode = UIProcessManager.IdGenerater("Consignee", CNETConstantes.CONTACT, 0, LocalBuffer.LocalBuffer.CurrentDeviceConsigneeUnit.Value, false, LocalBuffer.LocalBuffer.CurrentDevice.Id);

                if (!string.IsNullOrEmpty(currentVoCode))
                {
                    beiCode.EditValue = currentVoCode;
                }
                else
                {
                    SystemMessage.ShowModalInfoMessage("There is a problem on id setting!!!", "ERROR");
                    return;
                }

            }
            else
            {
                currentVoCode = UIProcessManager.IdGenerater("Consignee", CNETConstantes.GUEST, 0, LocalBuffer.LocalBuffer.CurrentDeviceConsigneeUnit.Value, false, LocalBuffer.LocalBuffer.CurrentDevice.Id);

                if (!string.IsNullOrEmpty(currentVoCode))
                {
                    beiCode.EditValue = currentVoCode;
                }
                else
                {
                    SystemMessage.ShowModalInfoMessage("There is a problem on id setting!!!", "ERROR");
                    return;
                }
                // perCode = UIProcessManager.GetCurrentIdByDevice(CNETConstantes.GSL_SYSTEM_PARAMETER, device, CNETConstantes.GUEST.ToString(), CNETConstantes.PERSON);
            }


            DateTime? CurrentTime = UIProcessManager.GetServiceTime();
            _personToEdit = null;
            teFirstName.Text = "";
            teMiddle.Text = "";
            tePassportNumber.Text = "";
            ceIsActive.Checked = true;
            deDateOfBirth.DateTime = CurrentTime == null ? DateTime.Now : CurrentTime.Value;
            cacNationality.EditValue = "";
            cacBusinessSegment.EditValue = "";
            cacLanguage.EditValue = "";
            cacAccount.EditValue = "";
            cacObjectState.EditValue = "";
            cacRateCode.EditValue = "";
            cacMailingAction.EditValue = "";
            cacTitle.EditValue = "";
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
            //var defaultTit = personTitleList.FirstOrDefault(c => c.isDefault);
            //if (defaultTit != null)
            //{
            //    cacTitle.EditValue = (defaultTit.code);
            //}

            var defaultSex = genderList.FirstOrDefault(c => c.IsDefault);
            if (defaultSex != null)
            {
                cacGender.EditValue = (defaultSex.Id);
            }

            var currency = currencyList.FirstOrDefault(c => c.IsDefault);
            if (currency != null)
            {
                cacCurrency.EditValue = (currency.Id);
            }
            var defualtID = idTypeList.FirstOrDefault(c => c.IsDefault);

            if (defualtID != null)
            {
                leIDType.EditValue = (defualtID.Id);
            }
        }

        public async void OnSave()
        {
            List<Control> controls = new List<Control>
            {
                cacGender,
                teFirstName,
                teLastName,
                leIDType,
                cacTitle,
                cacBusinessSegment
            };
            if (GSLType == CNETConstantes.GUEST)
            {
                controls.Add(tePassportNumber);
                controls.Add(cacNationality);
            }

            IList<Control> invalidControls = CustomValidationRule.Validate(controls);

            if (invalidControls.Count > 0)
            {
                SystemMessage.ShowModalInfoMessage("PLEASE FILL ALL REQUIRED FIELDS", "ERROR");
                return;
            }

            DateTime? CurrentTime = UIProcessManager.GetServiceTime();
            if (CurrentTime == null) return;

            //  VwConsigneeViewDTO CheckPerson = LocalBuffer.LocalBuffer.ConsigneeViewBufferList.FirstOrDefault(p => p.FirstName == teFirstName.EditValue && p.SecondName == teMiddle.EditValue && p.ThirdName == teLastName.EditValue);
            if (_personToEdit != null)
            {
                ConsigneeBuffer consigneeBuffer = UIProcessManager.GetConsigneeBufferById(_personToEdit.Id);

                #region Updating Person
                // Progress_Reporter.Show_Progress("Updating Person");

                consigneeBuffer.consignee.FirstName = teFirstName.Text;
                consigneeBuffer.consignee.SecondName = teMiddle.Text;
                consigneeBuffer.consignee.ThirdName = teLastName.Text;
                consigneeBuffer.consignee.IsActive = ceIsActive.Checked;
                consigneeBuffer.consignee.IsPerson = true;

                if (cacNationality.EditValue != null && cacNationality.EditValue.ToString() != "")
                {
                    consigneeBuffer.consignee.Nationality = Convert.ToInt32(cacNationality.EditValue);
                }

                //if (luk_religion.EditValue != null && luk_religion.EditValue.ToString() != "")
                //{
                //    consigneeBuffer.consignee.religion = luk_religion.EditValue.ToString();
                //}
                if (deDateOfBirth.EditValue != null)
                {
                    consigneeBuffer.consignee.StartDate = Convert.ToDateTime(deDateOfBirth.EditValue);
                }
                if (cacTitle.EditValue != null && cacTitle.EditValue.ToString() != "")
                {
                    consigneeBuffer.consignee.Title = Convert.ToInt32(cacTitle.EditValue);
                }
                if (cacGender.EditValue != null && cacGender.EditValue.ToString() != "")
                {
                    consigneeBuffer.consignee.Gender = Convert.ToInt32(cacGender.EditValue);
                }

                if (cacBusinessSegment.EditValue != null && cacBusinessSegment.EditValue.ToString() != "")
                {
                    consigneeBuffer.consignee.Preference = Convert.ToInt32(cacBusinessSegment.EditValue);
                }
                else
                {
                    var firstOrDefault = LocalBuffer.LocalBuffer.PreferenceBufferList.FirstOrDefault(p => p.SystemConstant == GSLType);
                    if (firstOrDefault != null)
                        consigneeBuffer.consignee.Preference = firstOrDefault.Id;

                }
                consigneeBuffer.Activity = ActivityLogManager.SetupActivity(CurrentTime.Value, activityDefCode, CNETConstantes.CONSIGNEE);
                if (GSLType == CNETConstantes.GUEST)
                {
                    if (lukTaxType.EditValue != null && !string.IsNullOrEmpty(lukTaxType.EditValue.ToString()))
                    {
                        if (consigneeBuffer.Gsltaxs.ToList().Count > 0)
                        {
                            consigneeBuffer.Gsltaxs.FirstOrDefault().Tax = Convert.ToInt32(lukTaxType.EditValue.ToString());
                        }
                    }
                    else
                    {
                        if (consigneeBuffer.Gsltaxs.ToList().Count > 0)
                            consigneeBuffer.Gsltaxs = new List<GsltaxDTO>();
                    }
                }

                if (leIDType.EditValue != null && !string.IsNullOrEmpty(leIDType.EditValue.ToString()) && Convert.ToInt32(leIDType.EditValue) == CNETConstantes.PERSONAL_IDENTIFICATIONTYPE_PASPORT)
                {
                    if (!string.IsNullOrEmpty(tePassportNumber.Text))
                        consigneeBuffer.consignee.PassportId = tePassportNumber.Text;
                    else
                        consigneeBuffer.consignee.PassportId = null;
                }
                else if (leIDType.EditValue != null && !string.IsNullOrEmpty(leIDType.EditValue.ToString()) && Convert.ToInt32(leIDType.EditValue) == CNETConstantes.PERSONAL_IDENTIFICATIONTYPE_NationalID)
                {
                    if (!string.IsNullOrEmpty(tePassportNumber.Text))
                        consigneeBuffer.consignee.NationalId = tePassportNumber.Text;
                    else
                        consigneeBuffer.consignee.NationalId = null;
                }


                if (cacLanguage.EditValue != null && !string.IsNullOrEmpty(cacLanguage.EditValue.ToString()))
                    consigneeBuffer.consignee.DefaultLanguage = Convert.ToInt32(cacLanguage.EditValue);
                else
                    consigneeBuffer.consignee.DefaultLanguage = null;

                if (!string.IsNullOrEmpty(note))
                    consigneeBuffer.consignee.Note = note;
                else
                    consigneeBuffer.consignee.Note = null;

                consigneeBuffer.consigneeUnits = CreateConssigneeUnit(_personToEdit.MainConsigneeUnit);


                if (passportConfigList != null)
                {
                    ConfigurationDTO imageDestination = passportConfigList.Where(pc => pc.Attribute == "Image Destination").FirstOrDefault();
                    // Attachment
                    if (scanDevice != null)
                    {
                        string strImgPath;
                        string nameAndYear = imageName + DateTime.Now.ToString("yyyyMMdd_hhmmss");
                        if (pePassPortImage.Image != null)
                        {
                            // Progress_Reporter.Show_Progress("Updating Attachment");
                            if (isARH || isFlatBedScanner)
                            {

                                string localdirectory = Environment.CurrentDirectory + "\\Passport Scanner Images";
                                if (!Directory.Exists(localdirectory))
                                {
                                    Directory.CreateDirectory(localdirectory);
                                }
                                path = localdirectory;

                                strImgPath = @path + "/" + nameAndYear + ".jpg";
                                pePassPortImage.Image.Save(@strImgPath);
                                consigneeBuffer.consignee.DefaultImageUrl = strImgPath;


                                bool Exist = FTPInterface.FTPAttachment.InitalizePMSFTPAttachment(TINNumber);
                                if (Exist)
                                {
                                    string FTPLocation = FTPInterface.FTPAttachment.SendPMSGuestImageAttachement(consigneeBuffer.consignee.Code, pePassPortImage.Image);
                                    consigneeBuffer.consignee.DefaultImageUrl = FTPLocation;
                                }
                                else
                                {
                                    consigneeBuffer.consignee.DefaultImageUrl = strImgPath;
                                }
                            }
                            else
                            {
                                consigneeBuffer.consignee.DefaultImageUrl = wintonePassportImageFullPath;
                            }
                            //CNETInfoReporter.Hide();
                        }

                    }

                }

                ConsigneeBuffer isUpdate = UIProcessManager.UpdateConsigneeBuffer(consigneeBuffer);
                if (isUpdate != null)
                {
                    //Load Person Buffer List
                    // LocalBuffer.LocalBuffer.loadvw_AllPersonView();

                    //Gsl Tax


                    // Attachment



                    //Negotiated Rate

                    List<NegotiationRateDTO> nerateNegotiatedRatelist = UIProcessManager.GetNegotiationRateByConsignee(_personToEdit.Id);
                    if (nerateNegotiatedRatelist != null && nerateNegotiatedRatelist.Count > 0)
                    {
                        NegotiationRateDTO nerateNegotiatedRate = nerateNegotiatedRatelist.FirstOrDefault();
                        // Progress_Reporter.Show_Progress("Updating Negotiated Rate");
                        if (cacRateCode.EditValue != "")
                        {
                            nerateNegotiatedRate.RateCode = Convert.ToInt32(cacRateCode.EditValue);
                            UIProcessManager.UpdateNegotiatedRate(nerateNegotiatedRate);
                        }
                        else
                        {
                            UIProcessManager.DeleteNegotiationRateById(nerateNegotiatedRate.Id);
                        }
                        ////CNETInfoReporter.Hide();
                    }
                    else if (cacRateCode.EditValue != "")
                    {
                        // Progress_Reporter.Show_Progress("Saving Negotiated Rate");
                        NegotiationRateDTO negorate = new NegotiationRateDTO
                        {
                            Consignee = _personToEdit.Id,
                            RateCode = Convert.ToInt32(cacRateCode.EditValue)
                        };

                        UIProcessManager.CreateNegotiationRate(negorate);
                        ////CNETInfoReporter.Hide();
                    }

                    //Object State
                    ObjectStateDTO objectState = UIProcessManager.GetObjectStateByReference(_personToEdit.Id);

                    if (objectState != null)
                    {
                        // Progress_Reporter.Show_Progress("Updating Object State");
                        if (cacObjectState.EditValue != null && !string.IsNullOrEmpty(cacObjectState.EditValue.ToString()))
                        {
                            objectState.ObjectStateDefinition = Convert.ToInt32(cacObjectState.EditValue);
                            //List<ObjectStateDTO> objs = new List<ObjectStateDTO>();
                            //objs.Add(objectState);
                            UIProcessManager.UpdateObjectState(objectState);
                        }
                        else
                        {
                            //List<ObjectState> objs = new List<ObjectState>();
                            //objs.Add(objectState);
                            UIProcessManager.DeleteObjectStateById(objectState.Id);
                        }


                        ////CNETInfoReporter.Hide();
                    }
                    else if (cacObjectState.EditValue != null && !string.IsNullOrEmpty(cacObjectState.EditValue.ToString()))
                    {
                        // Progress_Reporter.Show_Progress("Saving Object State");
                        ObjectStateDTO objstaObjectState = new ObjectStateDTO
                        {
                            Reference = _personToEdit.Id,
                            ObjectStateDefinition = Convert.ToInt32(cacObjectState.EditValue)
                        };

                        UIProcessManager.CreateObjectState(objstaObjectState);

                        ////CNETInfoReporter.Hide();
                    }


                    //Account Map
                    AccountMapDTO accMap = UIProcessManager.GetAccountMapByreferencefirstordefault(_personToEdit.Id);
                    if (accMap != null)
                    {
                        // Progress_Reporter.Show_Progress("updating Account Requirement");
                        if (cacAccount.EditValue != null && !string.IsNullOrEmpty(cacAccount.EditValue.ToString()))
                        {
                            accMap.Description = cacAccount.Text; // 
                            accMap.Remark = cacAccount.EditValue.ToString();

                            UIProcessManager.UpdateAccountMap(accMap);
                        }
                        else
                        {
                            UIProcessManager.DeleteAccountMapById(accMap.Id);
                        }
                        ////CNETInfoReporter.Hide();
                    }
                    else if (cacAccount.EditValue != null && cacAccount.EditValue.ToString() != "")
                    {
                        GslacctRequirementDTO gslAcc = UIProcessManager.GetGSLAcctRequirementById((int)cacAccount.EditValue);
                        AccountDTO acctAccount = new AccountDTO();
                        if (gslAcc != null)
                        {
                            acctAccount = UIProcessManager.GetAccountById(gslAcc.ContAcct);
                        }
                        if (acctAccount != null)
                        {
                            // Progress_Reporter.Show_Progress("Saving Account Requirement");
                            //List<AccountMap> accMapList = new List<AccountMap>();
                            AccountMapDTO accountMap = new AccountMapDTO();
                            accountMap.Reference = _personToEdit.Id.ToString();
                            accountMap.Description = cacAccount.Text; // 
                            accountMap.Account = acctAccount.Id;
                            accountMap.Remark = cacAccount.EditValue.ToString();
                            //accMapList.Add(accountMap);
                            UIProcessManager.CreateAccountMap(accountMap);
                            ////CNETInfoReporter.Hide();
                        }
                    }

                    ////Address
                    ////CNETInfoReporter.Hide();
                    SystemMessage.ShowModalInfoMessage("Updated SUCESSESFULLY", "MESSAGE");
                    DialogResult = DialogResult.OK;
                    ////Reset();
                    ////DialogResult = System.Windows.Forms.DialogResult.OK;
                }
                else
                {
                    SystemMessage.ShowModalInfoMessage(teFirstName.Text + " " + teMiddle.Text + " " + teLastName.Text + " " + "not upadeted!", "ERROR");
                }
                #endregion
            }
            else
            {
                #region Saving New Person


                bool isSaved = false;
                List<ConsigneeDTO> personsList = LocalBuffer.LocalBuffer.ConsigneeViewBufferList.Where(p => p.GslType == CNETConstantes.GUEST || p.GslType == CNETConstantes.CONTACT).ToList();
                ConsigneeDTO personSaved = personsList.FirstOrDefault(p => p.FirstName == teFirstName.Text && p.ThirdName == teLastName.Text &&
                            p.SecondName == teMiddle.Text);

                DialogResult dr = DialogResult.Yes;
                if (personSaved != null)
                {
                    if (personSaved.BioId == tePassportNumber.Text)
                    {
                        SystemMessage.ShowModalInfoMessage(
                          teFirstName.Text + " " + teMiddle.Text + " " + teLastName.Text + " already exists", "ERROR");
                        return;
                    }
                    else
                    {
                        if (XtraMessageBox.Show(teFirstName.Text + " " + teMiddle.Text + " " + teLastName.Text + " already exists but with a different Id Number " + Environment.NewLine + "Do u like to update the previous id Number with the New ??", "Information", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK)
                        {
                            ConsigneeDTO consignee = UIProcessManager.GetConsigneeById(personSaved.Id);

                            if (leIDType.EditValue != null && !string.IsNullOrEmpty(leIDType.EditValue.ToString()) && Convert.ToInt32(leIDType.EditValue) == CNETConstantes.PERSONAL_IDENTIFICATIONTYPE_PASPORT)
                            {
                                if (!string.IsNullOrEmpty(tePassportNumber.Text))
                                    consignee.PassportId = tePassportNumber.Text;
                                else
                                    consignee.PassportId = null;
                            }
                            else if (leIDType.EditValue != null && !string.IsNullOrEmpty(leIDType.EditValue.ToString()) && Convert.ToInt32(leIDType.EditValue) == CNETConstantes.PERSONAL_IDENTIFICATIONTYPE_NationalID)
                            {
                                if (!string.IsNullOrEmpty(tePassportNumber.Text))
                                    consignee.NationalId = tePassportNumber.Text;
                                else
                                    consignee.NationalId = null;
                            }
                            UIProcessManager.UpdateConsignee(consignee);
                            return;
                        }
                    }
                }
                if (isSaved)
                {
                    // dr = MessageBox.Show(teFirstName.Text+" "+teMiddle.Text+" "+teLastName.Text+" already exists. Do you want continue saving?", this.Text , MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    SystemMessage.ShowModalInfoMessage(
                        teFirstName.Text + " " + teMiddle.Text + " " + teLastName.Text + " already exists", "ERROR");

                }
                else
                {
                    string currentVoCode = null;
                    // Progress_Reporter.Show_Progress("Saving Person");
                    ConsigneeBuffer consigneeBuffer = new ConsigneeBuffer();
                    consigneeBuffer.consignee = new ConsigneeDTO();
                    if (GSLType == CNETConstantes.CONTACT)
                    {
                        currentVoCode = UIProcessManager.IdGenerater("Consignee", CNETConstantes.CONTACT, 1, LocalBuffer.LocalBuffer.CurrentDeviceConsigneeUnit.Value, false, LocalBuffer.LocalBuffer.CurrentDevice.Id);

                        if (!string.IsNullOrEmpty(currentVoCode))
                        {
                            consigneeBuffer.consignee.Code = currentVoCode;
                        }
                        consigneeBuffer.consignee.GslType = CNETConstantes.CONTACT;

                    }
                    else
                    {
                        currentVoCode = UIProcessManager.IdGenerater("Consignee", CNETConstantes.GUEST, 1, LocalBuffer.LocalBuffer.CurrentDeviceConsigneeUnit.Value, false, LocalBuffer.LocalBuffer.CurrentDevice.Id);

                        if (!string.IsNullOrEmpty(currentVoCode))
                        {
                            consigneeBuffer.consignee.Code = currentVoCode;
                        }
                        consigneeBuffer.consignee.GslType = CNETConstantes.GUEST;
                    }
                    consigneeBuffer.consignee.FirstName = teFirstName.Text;
                    consigneeBuffer.consignee.SecondName = teMiddle.Text;
                    consigneeBuffer.consignee.ThirdName = teLastName.Text;
                    consigneeBuffer.consignee.IsActive = ceIsActive.Checked;
                    consigneeBuffer.consignee.IsPerson = true;

                    if (cacNationality.EditValue != null && cacNationality.EditValue.ToString() != "")
                    {
                        consigneeBuffer.consignee.Nationality = Convert.ToInt32(cacNationality.EditValue);
                    }
                    if (deDateOfBirth.EditValue != null)
                    {
                        consigneeBuffer.consignee.StartDate = Convert.ToDateTime(deDateOfBirth.EditValue);
                    }
                    if (cacTitle.EditValue != null && cacTitle.EditValue.ToString() != "")
                    {
                        consigneeBuffer.consignee.Title = Convert.ToInt32(cacTitle.EditValue);
                    }
                    if (cacGender.EditValue != null && cacGender.EditValue.ToString() != "")
                    {
                        consigneeBuffer.consignee.Gender = Convert.ToInt32(cacGender.EditValue);
                    }

                    if (cacBusinessSegment.EditValue != null && cacBusinessSegment.EditValue.ToString() != "")
                    {
                        consigneeBuffer.consignee.Preference = Convert.ToInt32(cacBusinessSegment.EditValue);
                    }
                    else
                    {

                        var firstOrDefault = LocalBuffer.LocalBuffer.PreferenceBufferList.FirstOrDefault(p => p.SystemConstant == GSLType);
                        if (firstOrDefault != null)
                            consigneeBuffer.consignee.Preference = Convert.ToInt32(firstOrDefault.Id);
                    }

                    if (lukTaxType.EditValue != null && !string.IsNullOrEmpty(lukTaxType.EditValue.ToString()))
                    {
                        consigneeBuffer.Gsltaxs = new List<GsltaxDTO>();
                        consigneeBuffer.Gsltaxs.ToList().Add(new GsltaxDTO() { Tax = Convert.ToInt32(lukTaxType.EditValue) });
                    }

                    consigneeBuffer.Activity = ActivityLogManager.SetupActivity(CurrentTime.Value, activityDefCode, CNETConstantes.CONSIGNEE);

                    //if (!string.IsNullOrEmpty(tePassportNumber.Text))
                    //    consigneeBuffer.consignee.IdNumber = tePassportNumber.Text;

                    if (leIDType.EditValue != null && !string.IsNullOrEmpty(leIDType.EditValue.ToString()) && Convert.ToInt32(leIDType.EditValue) == CNETConstantes.PERSONAL_IDENTIFICATIONTYPE_PASPORT)
                    {
                        if (!string.IsNullOrEmpty(tePassportNumber.Text))
                            consigneeBuffer.consignee.PassportId = tePassportNumber.Text;
                        else
                            consigneeBuffer.consignee.PassportId = null;
                    }
                    else if (leIDType.EditValue != null && !string.IsNullOrEmpty(leIDType.EditValue.ToString()) && Convert.ToInt32(leIDType.EditValue) == CNETConstantes.PERSONAL_IDENTIFICATIONTYPE_NationalID)
                    {
                        if (!string.IsNullOrEmpty(tePassportNumber.Text))
                            consigneeBuffer.consignee.NationalId = tePassportNumber.Text;
                        else
                            consigneeBuffer.consignee.NationalId = null;
                    }


                    if (cacCurrency.EditValue != null && !string.IsNullOrEmpty(cacCurrency.EditValue.ToString()))
                        consigneeBuffer.consignee.DefaultCurrency = Convert.ToInt32(cacCurrency.EditValue);

                    if (cacLanguage.EditValue != null && !string.IsNullOrEmpty(cacLanguage.EditValue.ToString()))
                        consigneeBuffer.consignee.DefaultLanguage = Convert.ToInt32(cacLanguage.EditValue);

                    if (!string.IsNullOrEmpty(note))
                        consigneeBuffer.consignee.Note = note;
                    consigneeBuffer.consigneeUnits = CreateConssigneeUnit(null);

                    consigneeBuffer.consignee.LastModified = DateTime.Now;
                    consigneeBuffer.consignee.CreatedOn = DateTime.Now;


                    if (passportConfigList != null && passportConfigList.Count > 0)
                    {
                        ConfigurationDTO imageDestination = passportConfigList.Where(pc => pc.Attribute == "Image Destination").FirstOrDefault();

                        // Attachment
                        if (scanDevice != null)
                        {
                            string strImgPath;
                            string nameAndYear = imageName + DateTime.Now.ToString("yyyyMMdd_hhmmss");
                            if (pePassPortImage.Image != null)
                            {
                                //  Attachment atta = new Attachment();
                                if (isARH || isFlatBedScanner)
                                {
                                    if (path == "")
                                    {
                                        if (!Directory.Exists(@"C:\Passport Scanner Images"))
                                        {
                                            Directory.CreateDirectory(@"C:\Passport Scanner Images");
                                        }
                                        path = @"C:\Passport Scanner Images";
                                    }
                                    if (path != "" && !Directory.Exists(path))
                                    {
                                        try
                                        {
                                            Directory.CreateDirectory(path);
                                        }
                                        catch
                                        {
                                            if (!Directory.Exists(@"C:\Passport Scanner Images"))
                                            {
                                                Directory.CreateDirectory(@"C:\Passport Scanner Images");
                                            }
                                            path = @"C:\Passport Scanner Images";
                                        }
                                    }

                                    strImgPath = @path + "/" + nameAndYear + ".jpg";
                                    pePassPortImage.Image.Save(@strImgPath);
                                    //if (imageDestination != null)
                                    //{
                                    //    consigneeBuffer.consignee.DefaultImageUrl = strImgPath;
                                    //}

                                    bool Exist = FTPInterface.FTPAttachment.InitalizePMSFTPAttachment(TINNumber);
                                    if (Exist)
                                    {
                                        string FTPLocation = FTPInterface.FTPAttachment.SendPMSGuestImageAttachement(consigneeBuffer.consignee.Code, pePassPortImage.Image);
                                        consigneeBuffer.consignee.DefaultImageUrl = FTPLocation;
                                    }
                                    else
                                    {
                                        consigneeBuffer.consignee.DefaultImageUrl = strImgPath;
                                    }
                                }
                                else
                                {
                                    if (imageDestination != null)
                                    {
                                        consigneeBuffer.consignee.DefaultImageUrl = wintonePassportImageFullPath;
                                    }
                                }
                                 
                            }

                        }

                    }



                    ConsigneeBuffer isCreated = UIProcessManager.CreateConsigneeBuffer(consigneeBuffer);
                    if (isCreated != null && isCreated.consignee != null)
                    {
                        //Negotiated Rate
                        if (cacRateCode.EditValue != "")
                        {
                            // Progress_Reporter.Show_Progress("Saving Negotiated Rate");
                            NegotiationRateDTO negorate = new NegotiationRateDTO
                            {
                                Consignee = isCreated.consignee.Id,
                                RateCode = Convert.ToInt32(cacRateCode.EditValue)
                            };

                            UIProcessManager.CreateNegotiationRate(negorate);
                            ////CNETInfoReporter.Hide();
                        }






                        ////Privancy for mailing Action
                        //if (cacMailingAction.EditValue != "")
                        //{
                        //   // Progress_Reporter.Show_Progress("Saving mailing Action");
                        //    CNETPrivacy privacy = new CNETPrivacy
                        //    {
                        //        code = String.Empty,
                        //        PrivacyRule = cacMailingAction.EditValue.ToString(),
                        //        reference = person.code
                        //    };

                        //    UIProcessManager.CreateCNEtPrivacy(privacy);
                        //   ////CNETInfoReporter.Hide();
                        //}



                        //Object State
                        if (cacObjectState != null && !string.IsNullOrEmpty(cacObjectState.EditValue.ToString()))
                        {
                            // Progress_Reporter.Show_Progress("Saving Object State");
                            ObjectStateDTO objstaObjectState = new ObjectStateDTO
                            {
                                Reference = isCreated.consignee.Id,
                                ObjectStateDefinition = Convert.ToInt32(cacObjectState.EditValue)
                            };

                            UIProcessManager.CreateObjectState(objstaObjectState);


                            ////CNETInfoReporter.Hide();
                        }

                        //Account
                        if (cacAccount.EditValue != null && !string.IsNullOrEmpty(cacAccount.EditValue.ToString()))
                        {


                            GslacctRequirementDTO gslAcc = UIProcessManager.GetGSLAcctRequirementById(Convert.ToInt32(cacAccount.EditValue));
                            AccountDTO acctAccount = new AccountDTO();
                            if (gslAcc != null)
                            {
                                acctAccount = UIProcessManager.GetAccountById(gslAcc.ContAcct);
                                // UIProcessManager.SelectAllAccount().FirstOrDefault(a => a.controlAccount == gslAcc.contAcct);
                            }
                            if (acctAccount != null)
                            {
                                // Progress_Reporter.Show_Progress("Saving Account Requirement");
                                //List<AccountMapDTO> accMapList = new List<AccountMapDTO>();
                                AccountMapDTO accountMap = new AccountMapDTO();
                                accountMap.Reference = isCreated.consignee.Id.ToString();
                                accountMap.Description = cacAccount.Text; // 
                                accountMap.Account = acctAccount.Id;
                                accountMap.Remark = cacAccount.EditValue.ToString();
                                //accMapList.Add(accountMap);
                                AccountMapDTO result = UIProcessManager.CreateAccountMap(accountMap);
                            }
                        }

                        ////CNETInfoReporter.Hide();

                        if (isFromAccGuest)
                        {
                            accForm.RegVoucher = accForm.RegistrationExt.Registration;
                            accForm.Person = UIProcessManager.GetConsigneeById(isCreated.consignee.Id);
                            isFromAccGuest = false;
                            DialogResult = System.Windows.Forms.DialogResult.OK;
                            ReleasePassportScanners();
                            // SystemMessage.ShowModalInfoMessage("SAVED SUCESSESFULLY", "MESSAGE");
                            this.Close();
                            //  accForm.ShowDialog();

                        }
                        else if (isFromRegisteration)
                        {
                            SavedPerson = UIProcessManager.GetConsigneeById(isCreated.consignee.Id);
                            LocalBuffer.LocalBuffer.ConsigneeViewBufferList.Add(SavedPerson);
                            // ((frmMultipleRoomCheckIn)requesterForm.SubForm).SelectedPersonHandler.Invoke(person);
                            isFromRegisteration = false;
                            DialogResult = DialogResult.OK;
                            ReleasePassportScanners();


                        }
                        else
                        {

                            SavedPerson = UIProcessManager.GetConsigneeById(isCreated.consignee.Id);
                            LocalBuffer.LocalBuffer.ConsigneeViewBufferList.Add(SavedPerson);
                            // SystemMessage.ShowModalInfoMessage("SAVED SUCESSESFULLY", "MESSAGE");
                        }

                        this.BeginInvoke(
                            (System.Action)(() => SystemMessage.ShowModalInfoMessage("SAVED SUCESSESFULLY", "MESSAGE")));
                        Reset();
                        //  DialogResult = System.Windows.Forms.DialogResult.OK;
                    }
                    else
                    {
                        SystemMessage.ShowModalInfoMessage(
                            teFirstName.Text + " " + teMiddle.Text + " " + teLastName.Text + " " + "not saved", "ERROR");
                    }

                }

                #endregion
            }


            if (GSLType == CNETConstantes.CONTACT)
                _personList = LocalBuffer.LocalBuffer.ConsigneeViewBufferList.Where(p => p.GslType == CNETConstantes.CONTACT).ToList();
            else
                _personList = LocalBuffer.LocalBuffer.ConsigneeViewBufferList.Where(p => p.GslType == CNETConstantes.GUEST).ToList();

            AutoCompleteNames();
        }

        private List<ConsigneeUnitDTO> CreateConssigneeUnit(int? id)
        {
            ConsigneeUnitDTO consignee = new ConsigneeUnitDTO()
            {
                Id = id == null ? 0 : id.Value,
                Name = "Main Consignee",
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
                LastModified = DateTime.Now,
                IsOnline = false,
                Type = CNETConstantes.ORG_UNIT_TYPE_BRUNCH
                //  AddressLine2 = txtaddress2.EditValue == null ? null : txtaddress2.EditValue.ToString(),
            };

            return new List<ConsigneeUnitDTO>() { consignee };
        }

        public DeleteClickedResult OnDelete()
        {
            return new DeleteClickedResult { DeleteResult = ERP.Client.UI_Logic.PMS.Enum.DeleteResult.DELETE_SUCESSESFULLY, MessageType = ERP.Client.UI_Logic.PMS.Enum.MessageType.MESSAGEBOX };
        }



        #region PassPort Scanner

        private void ReleasePassportScanners()
        {
           try
            {
                if (!isARH)
                    MyDll.FreeIDCard();
            }
            catch (Exception ex)
            {
                //handle
            }

            try
            {
                if (pr != null)
                {
                    pr.Close();
                }
            }
            catch (Exception ex) { }

            if (_wintoneTimer != null)
            {
                try
                {
                    _wintoneTimer.Enabled = false;
                    _wintoneTimer.Stop();
                }
                catch (Exception ex) { }
            }
        }


        public bool checkPassportScanner()
        {
            scanDevice = UIProcessManager.GetDeviceByhostandpreference(device.Id, CNETConstantes.PASSPORT_SCANNER);
            if (scanDevice != null)
            {
                return true;
            }
            return false;
        }

        private bool ConnectPassportScanner()
        {
            bool flag = false;
           if (device == null) return flag;

            scanDevice = UIProcessManager.GetDeviceByhostandpreference(device.Id, CNETConstantes.PASSPORT_SCANNER);
            if (scanDevice != null)
            {
                ArticleDTO article = UIProcessManager.GetArticleById(scanDevice.Article);
                if (article == null)
                {
                    XtraMessageBox.Show("Unable to get product extension detail for the passport scanner", "CNET_v2016", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                if (string.IsNullOrEmpty(article.Model))
                {
                    XtraMessageBox.Show("Unable to get Model " + article.Name, "CNET_v2016", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                if (string.IsNullOrEmpty(scanDevice.DeviceValue))
                {
                    XtraMessageBox.Show("Unable to get device value for " + article.Model, "CNET_v2016", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                try
                {
                    if (article.Model == CNETConstantes.ARH_PASSPORT_SCANNER)
                    {
                        pr = new Pr22.DocumentReaderDevice();
                        pr.Close();
                        pr.UseDevice(scanDevice.DeviceValue);
                        pr.SetProperty("freerun_mode", "4");
                        flag = true;
                        isARH = true;
                        pr.PresenceStateChanged += new System.EventHandler<Pr22.Events.DetectionEventArgs>(DocumentStateChanged);
                        toolStripStatusLabel1.Text = "" + article.Model + " is connected!";
                    }
                    else if (article.Model == CNETConstantes.WINTONE_PASSPORT_SCANNER)
                    {
                        _wintoneTimer = new System.Windows.Forms.Timer(this.components);
                        _wintoneTimer.Tick += new System.EventHandler(this.AutoClassAndRecognize);
                        _wintoneTimer.Interval = 200;
                        _wintoneTimer.Enabled = true;
                        if (m_bIsIDCardLoaded)
                        {
                            return false;
                        }
                        int nRet = 0;
                        nRet = MyDll.LoadLibrary("IDCard");

                        if (nRet == 0)
                        {
                            //XtraMessageBox.Show("Failed to load IDCard.dll!", "CNET ERP");
                            return false;
                        }
                        string id = scanDevice.DeviceValue;
                        char[] arr = id.ToCharArray();
                        nRet = MyDll.InitIDCard(arr, 0, null);
                        if (nRet != 0)
                        {
                            // XtraMessageBox.Show("Failed to initialize the recognition engine!", "CNET ERP");
                            return false;
                        }
                        MyDll.SetSpecialAttribute(1, 1);
                        m_bIsIDCardLoaded = true;
                        if (!m_bIsIDCardLoaded)
                        {
                            //XtraMessageBox.Show("Failed to Load the recognition engine!", "CNET ERP");
                            return false;
                        }
                        isARH = false;
                        flag = true;
                        toolStripStatusLabel1.Text = "" + article.Model + " is connected!";
                    }
                    else if (article.Model == CNETConstantes.CNET_FLATBED_SCANNER)
                    {
                        toolStripStatusLabel1.Text = "" + article.Model + " is connected!";
                        isFlatBedScanner = true;
                        flag = true;

                    }

                }
                catch (Pr22.Exceptions.NoSuchDevice ex)
                {
                    return false;
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show("ERROR in connecting to passport scanner. DETAIL::" + ex.Message, "CNET_v2016", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }


            }
            else
            {
                //XtraMessageBox.Show("Unable to get passport scanner device registration", "CNET_v2016", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        


            return flag;
        }

        private void CaptureArhPassport()
        {
            try
            {
               Invoke(new System.Action(() =>
                {
                    ResetFields();
                }));


                string givenName = "";
                string countryCode = "";

                DocScannerTask CaptureTask = new DocScannerTask();
                CaptureTask.Add((Pr22.Imaging.Light.White));
                EngineTask OcrTask = new EngineTask();
                OcrTask.Add(Pr22.Processing.FieldSource.Mrz, Pr22.Processing.FieldId.All);
                OcrTask.Add(Pr22.Processing.FieldSource.Viz, Pr22.Processing.FieldId.All);
               

                AnalyzeResult = pr.Scanner.Scan(CaptureTask, Pr22.Imaging.PagePosition.First).Analyze(OcrTask);



                foreach (Pr22.Processing.FieldReference field in AnalyzeResult.GetFields())
                {
                    try
                    {
                        givenName = AnalyzeResult.GetField(FieldSource.Mrz, FieldId.Givenname).GetFormattedStringValue();

                        string lastName = AnalyzeResult.GetField(FieldSource.Mrz, FieldId.Surname).GetFormattedStringValue();
                        Invoke(new System.Action(() =>
                        {
                            if (givenName.Contains(' '))
                            {
                                string[] names = givenName.Split(' ');
                                teFirstName.Text = names[0];
                                teMiddle.Text = names[1];
                            }

                            else
                            {
                                teFirstName.Text = givenName;
                                teMiddle.Text = string.Empty;
                            }

                            teLastName.Text = lastName;
                        }));

                        AnalyzeResult.GetField(FieldSource.Mrz, FieldId.BirthDate).GetStandardizedStringValue();
                        countryCode = AnalyzeResult.GetField(FieldSource.Mrz, FieldId.Nationality).GetFormattedStringValue();
                        CountryDTO country = UIProcessManager.GetCountryByIcaocountryCode(countryCode);
                        if (country != null)
                        {
                            Invoke(new System.Action(() =>
                            {
                                cacNationality.EditValue = country.Id;
                            }));
                        }

                        if (AnalyzeResult.GetField(FieldSource.Mrz, FieldId.Sex).GetFormattedStringValue() == "F")
                        {

                            Invoke(new System.Action(() =>
                            {
                                cacGender.EditValue = CNETConstantes.FEMALE;
                            }));
                        }
                        else
                        {
                            Invoke(new System.Action(() =>
                            {
                                cacGender.EditValue = CNETConstantes.MALE;
                            }));

                        }

                        string mIdType = AnalyzeResult.GetField(FieldSource.Mrz, FieldId.DocType).GetFormattedStringValue();
                        try
                        {
                            if (mIdType == "PS" || mIdType == "P" || mIdType == "PF" || mIdType.StartsWith("P"))
                            {

                                Invoke(new System.Action(() =>
                                {
                                    leIDType.EditValue = CNETConstantes.PERSONAL_IDENTIFICATIONTYPE_PASPORT;
                                }));
                            }
                            else if (mIdType == "VC" || mIdType.StartsWith("V"))
                            {


                            }
                        }
                        catch (Pr22.Exceptions.EntryNotFound)
                        {
                        }

                        string passportNum = AnalyzeResult.GetField(FieldSource.Mrz, FieldId.DocumentNumber).GetFormattedStringValue();
                        Invoke(new System.Action(() =>
                        {
                            tePassportNumber.Text = passportNum;
                        }));

                        string CountryCode = AnalyzeResult.GetField(FieldSource.Mrz, FieldId.IssueCountry).GetStandardizedStringValue();
                        try
                        {


                            RegionInfo info = new RegionInfo(CountryCode);
                            string converted = "";
                            converted = info.TwoLetterISORegionName.ToString().ToUpper();
                            RegionInfo newinfo = new RegionInfo(converted);

                        }
                        catch (ArgumentException argEx)
                        {
                            AnalyzeResult.GetField(FieldSource.Mrz, FieldId.IssueCountry).GetFormattedStringValue();
                        }


                        string mYY = null;
                        string mMM = null;
                        string mDD = null;



                        string stanbirth = AnalyzeResult.GetField(FieldSource.Mrz, FieldId.BirthDate).GetFormattedStringValue();
                        if (!string.IsNullOrEmpty(stanbirth))
                        {
                            if (stanbirth.Length == 6)
                            {
                                mYY = stanbirth.Substring(0, 2);
                                mMM = stanbirth.Substring(2, 2);
                                mDD = stanbirth.Substring(4, 2);

                                Invoke(new System.Action(() =>
                                {
                                    deDateOfBirth.EditValue = mMM + "/" + mDD + "/" + "19" + mYY;
                                }));
                            }
                            else if (stanbirth.Length == 8)
                            {
                                mYY = stanbirth.Substring(0, 4);
                                mMM = stanbirth.Substring(4, 2);
                                mDD = stanbirth.Substring(6, 2); 
                                Invoke(new System.Action(() =>
                                {
                                    deDateOfBirth.EditValue = mMM + "/" + mDD + "/" + "19" + mYY;
                                }));
                               // deDateOfBirth.EditValue = mMM + "/" + mDD + "/" + mYY;
                            }
                        }

                    }
                    catch (Pr22.Exceptions.EntryNotFound) { }
                }

                Invoke(new System.Action(() =>
                {
                    pePassPortImage.Image = pr.Scanner.GetPage(0).Select(Pr22.Imaging.Light.White).ToBitmap();
                }));
            }
            catch (Exception ex)
            {
                //
            }
        }



        private void bbiScanFingerPrint_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (isFlatBedScanner)
                {
                    Analyze analyze = new Analyze();
                    var person = analyze.Person;
                    if (person == null) return;
                    teFirstName.Text = person.firstName;
                    teMiddle.Text = person.middleName;
                    teLastName.Text = person.lastName;
                    // cacNationality.EditValue = person.nationality;
                    cacGender.EditValue = person.gender;
                    deDateOfBirth.EditValue = person.DOB;


                    //  var identification = analyze.Identification;
                    if (!string.IsNullOrEmpty(person.idNumber))
                    {
                        tePassportNumber.Text = person.idNumber;
                        // leIDType.EditValue = identification.type;
                    }

                    var passImage = analyze.PassportImage;
                    if (passImage != null)
                    {
                        pePassPortImage.Image = passImage;
                    }
                }
                else
                {
                    if (!isARH)
                    {
                        CaptureRecognizeWintone(false);
                    }
                    else
                    {
                        #region Capture ARH
                       
                        ResetFields();

                        string givenName = "";
                        string countryCode = "";


                        DocScannerTask CaptureTask = new DocScannerTask();
                        CaptureTask.Add((Pr22.Imaging.Light.White));
                        EngineTask OcrTask = new EngineTask();
                        OcrTask.Add(Pr22.Processing.FieldSource.Mrz, Pr22.Processing.FieldId.All);
                        OcrTask.Add(Pr22.Processing.FieldSource.Viz, Pr22.Processing.FieldId.All);
                        AnalyzeResult = pr.Scanner.Scan(CaptureTask, Pr22.Imaging.PagePosition.First).Analyze(OcrTask);

                        foreach (Pr22.Processing.FieldReference field in AnalyzeResult.GetFields())
                        {
                            try
                            {
                                givenName = AnalyzeResult.GetField(FieldSource.Mrz, FieldId.Givenname).GetFormattedStringValue();
                                if (givenName.Contains(' '))
                                {
                                    string[] names = givenName.Split(' ');
                                    teFirstName.Text = names[0];
                                    teMiddle.Text = names[1];
                                }

                                else
                                {
                                    teFirstName.Text = givenName;
                                    teMiddle.Text = string.Empty;
                                }
                                teLastName.Text = AnalyzeResult.GetField(FieldSource.Mrz, FieldId.Surname).GetFormattedStringValue();
                                AnalyzeResult.GetField(FieldSource.Mrz, FieldId.BirthDate).GetStandardizedStringValue();
                                countryCode = AnalyzeResult.GetField(FieldSource.Mrz, FieldId.Nationality).GetFormattedStringValue();
                                CountryDTO country = UIProcessManager.GetCountryByIcaocountryCode(countryCode);
                                if (country != null)
                                    cacNationality.EditValue = country.Id;

                                if (AnalyzeResult.GetField(FieldSource.Mrz, FieldId.Sex).GetFormattedStringValue() == "F")
                                {
                                    cacGender.EditValue = CNETConstantes.FEMALE;
                                }
                                else
                                {
                                    cacGender.EditValue = CNETConstantes.MALE;

                                }

                                string mIdType = AnalyzeResult.GetField(FieldSource.Mrz, FieldId.DocType).GetFormattedStringValue();
                                try
                                {
                                    if (mIdType == "PS" || mIdType == "P" || mIdType == "PF" || mIdType.StartsWith("P"))
                                    {
                                        leIDType.EditValue = CNETConstantes.PERSONAL_IDENTIFICATIONTYPE_PASPORT;
                                    }
                                    else if (mIdType == "VC" || mIdType.StartsWith("V"))
                                    {
                                        leIDType.EditValue = CNETConstantes.PERSONAL_IDENTIFICATIONTYPE_VISSA;
                                    }
                                }
                                catch (Pr22.Exceptions.EntryNotFound)
                                {
                                }


                                tePassportNumber.Text = AnalyzeResult.GetField(FieldSource.Mrz, FieldId.DocumentNumber).GetFormattedStringValue();
                                string CountryCode = AnalyzeResult.GetField(FieldSource.Mrz, FieldId.IssueCountry).GetStandardizedStringValue();
                                try
                                {


                                    RegionInfo info = new RegionInfo(CountryCode);
                                    string converted = "";
                                    converted = info.TwoLetterISORegionName.ToString().ToUpper();
                                    RegionInfo newinfo = new RegionInfo(converted);

                                }
                                catch (ArgumentException argEx)
                                {
                                    AnalyzeResult.GetField(FieldSource.Mrz, FieldId.IssueCountry).GetFormattedStringValue();
                                }


                                string mYY = null;
                                string mMM = null;
                                string mDD = null;



                                string stanbirth = AnalyzeResult.GetField(FieldSource.Mrz, FieldId.BirthDate).GetFormattedStringValue();
                                if (!string.IsNullOrEmpty(stanbirth))
                                {
                                    if (stanbirth.Length == 6)
                                    {
                                        mYY = stanbirth.Substring(0, 2);
                                        mMM = stanbirth.Substring(2, 2);
                                        mDD = stanbirth.Substring(4, 2);
                                        deDateOfBirth.EditValue = mMM + "/" + mDD + "/" + "19" + mYY;
                                    }
                                    else if (stanbirth.Length ==8)
                                    {
                                        mYY = stanbirth.Substring(0, 4);
                                        mMM = stanbirth.Substring(4, 2);
                                        mDD = stanbirth.Substring(6, 2);
                                        deDateOfBirth.EditValue = mMM + "/" + mDD + "/"+ mYY;
                                    }
                                }

                            }
                            catch (Pr22.Exceptions.EntryNotFound) { }
                        }
                        pePassPortImage.Image = pr.Scanner.GetPage(0).Select(Pr22.Imaging.Light.White).ToBitmap();
                        
                        #endregion
                    }
                }
                
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Error capturing from passport scanner. Detail:: " + ex.Message, "Passport scanner", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void DocumentStateChanged(object a, Pr22.Events.DetectionEventArgs e)
        {
            if (e.State == Pr22.Util.PresenceState.Present)
            {
                CaptureArhPassport();
            }
        }

        private void AutoClassAndRecognize(object sender, EventArgs e)
        {
            CaptureRecognizeWintone(true);
        }

        private void CaptureRecognizeWintone(bool isAuto)
        {
            try
            {
                if (!m_bIsIDCardLoaded)
                {
                    return;
                }
                int a = MyDll.GetGrabSignalType();
                if (isAuto && a != 1) return;

                ResetFields();
                bool bRet = MyDll.SetAcquireImageType(1, 1);
                int nRet = MyDll.AcquireImage(21);
                if (nRet != 0)
                {
                    XtraMessageBox.Show("Image Acquisition Failure!", "CNET ERP");
                    return;
                }

                int[] nSubID = new int[1];
                nSubID[0] = 0;
                MyDll.AddIDCardID(1, nSubID, 1);
                MyDll.AddIDCardID(2, nSubID, 1);
                MyDll.AddIDCardID(3, nSubID, 1);
                MyDll.AddIDCardID(4, nSubID, 1);
                MyDll.AddIDCardID(5, nSubID, 1);
                MyDll.AddIDCardID(6, nSubID, 1);
                MyDll.AddIDCardID(7, nSubID, 1);
                MyDll.AddIDCardID(9, nSubID, 1);
                MyDll.AddIDCardID(10, nSubID, 1);
                MyDll.AddIDCardID(11, nSubID, 1);
                MyDll.AddIDCardID(12, nSubID, 1);
                MyDll.AddIDCardID(13, nSubID, 1);
                MyDll.AddIDCardID(14, nSubID, 1);
                MyDll.AddIDCardID(15, nSubID, 1);
                MyDll.AddIDCardID(16, nSubID, 1);
                MyDll.AddIDCardID(1000, nSubID, 1);
                MyDll.AddIDCardID(1001, nSubID, 1);
                MyDll.AddIDCardID(1003, nSubID, 1);
                MyDll.AddIDCardID(1004, nSubID, 1);
                MyDll.AddIDCardID(1005, nSubID, 1);
                MyDll.AddIDCardID(1107, nSubID, 1);
                MyDll.AddIDCardID(1008, nSubID, 1);
                MyDll.AddIDCardID(1009, nSubID, 1);
                MyDll.AddIDCardID(1010, nSubID, 1);

                bool recogFailureFlag = false;
                nRet = MyDll.RecogIDCard();
                if (nRet <= 0)
                {
                    XtraMessageBox.Show("Recognition Failure!", "CNET ERP");
                    goto HERE;
                }

                int MAX_CH_NUM = 128;
                char[] cArrFieldValue = new char[MAX_CH_NUM];
                char[] cArrFieldName = new char[MAX_CH_NUM];
                string[] name = new string[20];
                string[] value = new string[20];
                for (int i = 0; ; i++)
                {

                    nRet = MyDll.GetRecogResult(i, cArrFieldValue, ref MAX_CH_NUM);
                    if (nRet == 3)
                    {
                        break;
                    }
                    MyDll.GetFieldName(i, cArrFieldName, ref MAX_CH_NUM);
                    name[i] = new string(cArrFieldName);
                    value[i] = new String(cArrFieldValue);
                    string[] values = new string[2];
                    values[0] = name[i];
                    values[1] = value[i];
                    string fieldName = values[0];
                    string fieldValue = values[1];
                    string[] X = fieldName.Split(('\0'));
                    string[] Y = fieldValue.Split(('\0'));


                    if (i == 8)
                    {
                        teLastName.Text = Y[0];
                    }
                    else if (i == 9)
                    {
                        String fname = Y[0];
                        if (!string.IsNullOrEmpty(fname))
                        {
                            String[] fmname = fname.Split(' ');
                            if (fmname.Count() > 0)
                                teFirstName.Text = fmname[0];
                            if (fmname.Count() > 1)
                                teMiddle.Text = fmname[1];
                        }
                    }
                    else if (i == 5)
                    {
                        deDateOfBirth.EditValue = Y[0];

                    }
                    else if (i == 12)
                    {
                        CountryDTO country = UIProcessManager.GetCountryByIcaocountryCode(Y[0]);
                        if (country != null)
                            cacNationality.EditValue = country.Id;
                    }
                    else if (i == 13)
                    {
                        tePassportNumber.Text = Y[0];
                    }
                    else if (i == 4)
                    {
                        if (Y[0] == "F")
                        {
                            cacGender.EditValue = CNETConstantes.FEMALE;
                        }
                        else
                        {
                            cacGender.EditValue = CNETConstantes.MALE;
                        }
                    }
                    else if (i == 0)
                    {
                        if (Y[0] == "P" || Y[0] == "PS" || Y[0] == "PF" || Y[0].StartsWith("P"))
                        {
                            leIDType.EditValue = CNETConstantes.PERSONAL_IDENTIFICATIONTYPE_PASPORT;
                        }
                        else if (Y[0] == "V" || Y[0] == "VC")
                        {
                            leIDType.EditValue = CNETConstantes.PERSONAL_IDENTIFICATIONTYPE_VISSA;
                        }
                    }

                }


                HERE:
                if (path == "")
                {
                    if (!Directory.Exists(@"C:\Passport Scanner Images"))
                    {
                        Directory.CreateDirectory(@"C:\Passport Scanner Images");
                    }
                    path = @"C:\Passport Scanner Images";
                }
                if (path != "" && !Directory.Exists(path))
                {
                    try
                    {
                        Directory.CreateDirectory(path);
                    }
                    catch
                    {
                        if (!Directory.Exists(@"C:\Passport Scanner Images"))
                        {
                            try
                            {
                                Directory.CreateDirectory(@"C:\Passport Scanner Images");
                            }
                            catch
                            {
                                XtraMessageBox.Show("Can not save Image!", "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Error);

                                return;
                            }
                        }
                        path = @"C:\Passport Scanner Images";
                    }
                }
                //string date = DateTime.Now.ToString();
                //string saveName = imageName + date;
                string nameAndYear = imageName + DateTime.Now.ToString("yyyyMMdd_hhmmss");
                string strImgPath = @path + "/" + nameAndYear + ".jpg";

                char[] carrImgPath = strImgPath.ToCharArray();
                nRet = MyDll.SaveImage(carrImgPath);
                if (nRet != 0)
                {
                    XtraMessageBox.Show("Failed to save image!", "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                wintonePassportImageFullPath = strImgPath;
                Image img = Image.FromFile(strImgPath);
                pePassPortImage.Refresh();
                pePassPortImage.Image = img;
                Array.Clear(carrImgPath, 0, carrImgPath.Length);
                imageName = "";
                strImgPath = "";
            }
            catch (Exception ex) { }

        }

        public static byte[] ImageToByte(Image img)
        {
            byte[] byteArray = new byte[0];
            using (MemoryStream stream = new MemoryStream())
            {
                img.Save(stream, System.Drawing.Imaging.ImageFormat.Bmp);
                stream.Close();

                byteArray = stream.ToArray();
            }
            return byteArray;
        }


        #endregion


        #endregion

        #region Event Handlers


        public void LoadData(UILogicBase requesterForm, object args)
        {
            this.requesterForm = requesterForm;

            if (requesterForm.SubForm.GetType() == typeof(frmGroupRegistration) || requesterForm.SubForm.GetType() == typeof(frmReservation) || requesterForm.SubForm.GetType() == typeof(frmProfileAmendment) || requesterForm.SubForm.GetType() == typeof(frmMultipleRoomCheckIn) || requesterForm.SubForm.GetType() == typeof(frmCheckIn))
            {
                rpgNew.Visible = true;
                // rpgOptions.Visible = true;
                isFromRegisteration = true;
                rpgSaveAndDelete.Visible = true;
                rpgSearch.Visible = true;
                rpgAddGuest.Visible = false;
            }
            if (requesterForm.SubForm.GetType() == typeof(frmAccompanyingGuest))
            {
                isFromAccGuest = true;
                // RegistrationVoucher = args.ToString();
                accForm = (frmAccompanyingGuest)args;
            }
        }

        private void bbiAddOn_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
        }

        private void bbiAddGuest_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (requesterForm.SubForm.GetType().Equals(typeof(frmReservation)))
            {
                OnSave();
            }

            Close();
        }

        private void bbiNote_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmNote frmNote = new frmNote();
            frmNote.NoteContent = note;
            if (frmNote.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                note = frmNote.NoteContent;
            }
        }

        private void bbiNew_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Reset();
        }

        private void frmPerson_Load(object sender, EventArgs e)
        {
            bool result = InitializeData();
            if (!result)
            {
                try
                {
                    if (!isARH)
                        MyDll.FreeIDCard();
                }
                catch (Exception ex)
                {
                    //handle
                }
                this.Close();
            }
        }

        private void bbiSave_ItemClick_1(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            OnSave();
        }

        private void bbiClose_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (!isARH)
                    MyDll.FreeIDCard();
            }
            catch (Exception ex)
            {
                //handle
            }

            try
            {
                if (pr != null)
                {
                    pr.Close();
                }
            }
            catch (Exception ex) { }

            if (_wintoneTimer != null)
            {
                try
                {


                    _wintoneTimer.Tick -= new System.EventHandler(this.AutoClassAndRecognize);
                    _wintoneTimer.Stop();
                    _wintoneTimer.Enabled = false;
                    _wintoneTimer.Dispose();
                    _wintoneTimer = null;

                    this.components.Dispose();
                }
                catch (Exception ex)
                {

                }
            }
            personTitleList = null;
            genderList = null;
            idTypeList = null;
            mailingActionList = null;
            currencyList = null;
            countryList = null;
            languageList = null;
            _personList = null;
            requesterForm = null;
            device = null;
            scanDevice = null;
            _pSearchVMList = null;
            savedPerson = null;
            accForm = null;
            PersonToEdit = null;
            this.Close();
        }

        private void teLastName_ParseEditValue(object sender, ConvertEditValueEventArgs e)
        {
            if (e.Value != null)
            {
                e.Value = doCapitialize ? e.Value.ToString().ToUpper() : e.Value.ToString();
                e.Handled = true;
            }
        }

        private void teFirstName_ParseEditValue(object sender, ConvertEditValueEventArgs e)
        {
            if (e.Value != null)
            {
                e.Value = doCapitialize ? e.Value.ToString().ToUpper() : e.Value.ToString();
                e.Handled = true;
            }
        }

        private void teMiddle_ParseEditValue(object sender, ConvertEditValueEventArgs e)
        {
            if (e.Value != null)
            {
                e.Value = doCapitialize ? e.Value.ToString().ToUpper() : e.Value.ToString();
                e.Handled = true;
            }
        }

        private void cacTitle_KeyDown(object sender, KeyEventArgs e)
        {
            LookUpEdit edit = sender as LookUpEdit;
            if (e.KeyData == Keys.Back || e.KeyData == Keys.Delete)
            {
                edit.ClosePopup();
                edit.EditValue = "";

            }
            e.Handled = true;
        }

        private void cacLanguage_KeyDown(object sender, KeyEventArgs e)
        {
            LookUpEdit edit = sender as LookUpEdit;
            if (e.KeyData == Keys.Back || e.KeyData == Keys.Delete)
            {
                edit.ClosePopup();
                edit.EditValue = "";

            }
            e.Handled = true;
        }

        private void cacGender_KeyDown(object sender, KeyEventArgs e)
        {
            LookUpEdit edit = sender as LookUpEdit;
            if (e.KeyData == Keys.Back || e.KeyData == Keys.Delete)
            {
                edit.ClosePopup();
                edit.EditValue = "";

            }
            e.Handled = true;
        }

        private void cacNationality_KeyDown(object sender, KeyEventArgs e)
        {
            LookUpEdit edit = sender as LookUpEdit;
            if (e.KeyData == Keys.Back || e.KeyData == Keys.Delete)
            {
                edit.ClosePopup();
                edit.EditValue = "";

            }
            e.Handled = true;
        }

        private void leIDType_KeyDown(object sender, KeyEventArgs e)
        {
            LookUpEdit edit = sender as LookUpEdit;
            if (e.KeyData == Keys.Back || e.KeyData == Keys.Delete)
            {
                edit.ClosePopup();
                edit.EditValue = "";

            }
            e.Handled = true;
        }

        private void cacBusinessSegment_KeyDown(object sender, KeyEventArgs e)
        {
            LookUpEdit edit = sender as LookUpEdit;
            if (e.KeyData == Keys.Back || e.KeyData == Keys.Delete)
            {
                edit.ClosePopup();
                edit.EditValue = "";

            }
            e.Handled = true;
        }

        private void cacAccount_KeyDown(object sender, KeyEventArgs e)
        {
            LookUpEdit edit = sender as LookUpEdit;
            if (e.KeyData == Keys.Back || e.KeyData == Keys.Delete)
            {
                edit.ClosePopup();
                edit.EditValue = "";

            }
            e.Handled = true;
        }

        private void cacVIP_KeyDown(object sender, KeyEventArgs e)
        {
            LookUpEdit edit = sender as LookUpEdit;
            if (e.KeyData == Keys.Back || e.KeyData == Keys.Delete)
            {
                edit.ClosePopup();
                edit.EditValue = "";

            }
            e.Handled = true;
        }

        private void cacCurrency_KeyDown(object sender, KeyEventArgs e)
        {
            LookUpEdit edit = sender as LookUpEdit;
            if (e.KeyData == Keys.Back || e.KeyData == Keys.Delete)
            {
                edit.ClosePopup();
                edit.EditValue = "";

            }
            e.Handled = true;
        }

        private void cacMailingAction_KeyDown(object sender, KeyEventArgs e)
        {
            LookUpEdit edit = sender as LookUpEdit;
            if (e.KeyData == Keys.Back || e.KeyData == Keys.Delete)
            {
                edit.ClosePopup();
                edit.EditValue = "";

            }
            e.Handled = true;
        }

        private void cacRateCode_KeyDown(object sender, KeyEventArgs e)
        {
            LookUpEdit edit = sender as LookUpEdit;
            if (e.KeyData == Keys.Back || e.KeyData == Keys.Delete)
            {
                edit.ClosePopup();
                edit.EditValue = "";

            }
            e.Handled = true;
        }

        private void teLastName_Leave(object sender, EventArgs e)
        {
            FilterPerson();
        }

        private void teFirstName_Leave(object sender, EventArgs e)
        {
            FilterPerson();
        }

        private void teMiddle_Leave(object sender, EventArgs e)
        {
            FilterPerson();
        }

        private void teLastName_InvalidValue(object sender, InvalidValueExceptionEventArgs e)
        {
            e.ErrorText = "Please enter last name.";
            AutoValidate = AutoValidate.EnableAllowFocusChange;
        }

        private void teFirstName_InvalidValue(object sender, InvalidValueExceptionEventArgs e)
        {
            e.ErrorText = "Please enter first name.";
            AutoValidate = AutoValidate.EnableAllowFocusChange;
        }

        private void tePassportNumber_InvalidValue(object sender, InvalidValueExceptionEventArgs e)
        {
            e.ErrorText = "Please enter a  valid id number";
            AutoValidate = AutoValidate.EnableAllowFocusChange;
        }

        private void teLastName_KeyDown(object sender, KeyEventArgs e)
        {
            DevExpress.XtraEditors.TextEdit edit = sender as DevExpress.XtraEditors.TextEdit;
            if (e.KeyData == Keys.Enter)
            {
                FilterPerson();
            }
            e.Handled = true;
        }

        private void teFirstName_KeyDown(object sender, KeyEventArgs e)
        {
            DevExpress.XtraEditors.TextEdit edit = sender as DevExpress.XtraEditors.TextEdit;
            if (e.KeyData == Keys.Enter)
            {
                FilterPerson();

            }
            e.Handled = true;
        }

        private void teMiddle_KeyDown(object sender, KeyEventArgs e)
        {
            DevExpress.XtraEditors.TextEdit edit = sender as DevExpress.XtraEditors.TextEdit;
            if (e.KeyData == Keys.Enter)
            {
                FilterPerson();

            }
            e.Handled = true;
        }

        private void gv_searchPerson_DoubleClick(object sender, EventArgs e)
        {
            PersonSearchVM focusedVM = gv_searchPerson.GetFocusedRow() as PersonSearchVM;
            if (focusedVM != null)
            {
                VwConsigneeViewDTO person = UIProcessManager.GetConsigneeViewById(focusedVM.Id);
                if (person != null)
                {
                    PopulateAllPersonFields(person);
                    rpgAttachment.Visible = true;
                }
                else
                {
                    XtraMessageBox.Show("Unable to get person detail with code = " + focusedVM.Code, "CNET_v2016", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }


        private void pePassPortImage_Click(object sender, EventArgs e)
        {

        }

        private void leIDType_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void bbiAttachment_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            if (beiCode.EditValue != null && !string.IsNullOrEmpty(beiCode.EditValue.ToString()))
            {
                //var person = LocalBuffer.LocalBuffer.AllPersonViewBufferList.FirstOrDefault(p => p.code == beiCode.EditValue.ToString());
                //if (person == null)
                //{
                //    SystemMessage.ShowModalInfoMessage("Please Select or Save a guest first!", "ERROR");
                //    return;
                //}


                //frmAttachment _frmAttachment = new frmAttachment();
                //_frmAttachment.IsFromProfile = true;
                //_frmAttachment.ConsigneeId = person.code;
                //_frmAttachment.IntType = CNETConstantes.CUSTOMERPERSO;
                //_frmAttachment.ShowDialog();
            }
            else
            {
                SystemMessage.ShowModalInfoMessage("Please Select or Save a guest first!", "ERROR");
                return;
            }

        }

        #endregion



        /// Clean up any resources being used.
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
                personTitleList = null;
                genderList = null;
                idTypeList = null;
                mailingActionList = null;
                currencyList = null;
                countryList = null;
                languageList = null;
                _personList = null;
                requesterForm = null;
                device = null;
                scanDevice = null;
                _pSearchVMList = null;
                savedPerson = null;
                accForm = null;
                PersonToEdit = null;
            }
            // MyDll.FreeIDCard();

            base.Dispose(disposing);
        }








    }//END OF frmPerson CLASS



    /// ///////////////////////////////// INNER CLASSESS ////////////////////////////////////////

    public static class MyDll
    {

        [DllImport("kernel32")]
        public static extern int LoadLibrary(string strDllName);

        [DllImport("IDCard", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Winapi)]
        public static extern int InitIDCard(char[] cArrUserID, int nType, char[] cArrDirectory);

        [DllImport("IDCard", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Winapi)]
        public static extern int GetRecogResult(int nIndex, char[] cArrBuffer, ref int nBufferLen);

        [DllImport("IDCard", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Winapi)]
        public static extern int RecogIDCard();

        [DllImport("IDCard", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Winapi)]
        public static extern int GetFieldName(int nIndex, char[] cArrBuffer, ref int nBufferLen);

        [DllImport("IDCard", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Winapi)]
        public static extern int AcquireImage(int nCardType);
        [DllImport("IDCard", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Winapi)]
        public static extern int SaveImage(char[] cArrFileName);
        [DllImport("IDCard", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Winapi)]
        public static extern int SaveHeadImage(char[] cArrFileName);


        [DllImport("IDCard", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Winapi)]
        public static extern int GetCurrentDevice(char[] cArrDeviceName, int nLength);

        [DllImport("IDCard", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Winapi)]
        public static extern void GetVersionInfo(char[] cArrVersion, int nLength);

        [DllImport("IDCard", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Winapi)]
        public static extern bool CheckDeviceOnline();

        [DllImport("IDCard", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Winapi)]
        public static extern bool SetAcquireImageType(int nLightType, int nImageType);

        [DllImport("IDCard", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Winapi)]
        public static extern bool SetUserDefinedImageSize(int nWidth, int nHeight);

        [DllImport("IDCard", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Winapi)]
        public static extern bool SetAcquireImageResolution(int nResolutionX, int nResolutionY);

        [DllImport("IDCard", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Winapi)]
        public static extern int SetIDCardID(int nMainID, int[] nSubID, int nSubIdCount);

        [DllImport("IDCard", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Winapi)]
        public static extern int AddIDCardID(int nMainID, int[] nSubID, int nSubIdCount);

        [DllImport("IDCard", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Winapi)]
        public static extern int RecogIDCardEX(int nMainID, int nSubID);

        [DllImport("IDCard", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Winapi)]
        public static extern int GetButtonDownType();

        [DllImport("IDCard", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Winapi)]
        public static extern int GetGrabSignalType();

        [DllImport("IDCard", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Winapi)]
        public static extern int SetSpecialAttribute(int nType, int nSet);

        [DllImport("IDCard", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Winapi)]
        public static extern void FreeIDCard();
    }


    class PersonSearchVM
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }


    }
}
