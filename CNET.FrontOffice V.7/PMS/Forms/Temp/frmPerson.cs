using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using CNET.ERP.Client.Common.UI;
using CNET.FrontOffice_V._7.PMS.Contracts;
using DevExpress.XtraEditors;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using System.Drawing;
using CNET.ERP.Client.Common.UI.Library;
using DevExpress.XtraTreeList;
using ImplementationDefault;
using System.Windows.Forms;
using FormValidation;
using CNET.FrontOffice_V._7.Exceptions;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using CNET.FrontOffice_V._7.Forms.Non_Navigatable_Modals;
using CNET.FrontOffice_V._7.PMS.Contracts;

namespace CNET.FrontOffice_V._7.Forms.Temp
{
    public partial class frmPerson : UILogicBase, ILogicHelper, ICanCreate, ICanSave, ICanDelete

        //public partial class frmPerson :XtraForm
    {
        private DateTime _currenDateTime;
        List<Lookup> pertitleList = UIProcessManager.GetLookup(CNETConstantes.PERSONALTITLE);
        List<Lookup> genderList = UIProcessManager.GetLookup(CNETConstantes.GENDER);
        List<Currency> currencyList = UIProcessManager.SelectAllCurrency();
        List<Preference> addressAttribute = UIProcessManager.GetPreferenceByComponent(CNETConstantes.Address);
        public frmPerson()
        {
            InitializeComponent();
            InitializeUI();
            InitializeData();
            FormSize = new Size(840, 530);

            ApplyIcons();
        }

        private void ApplyIcons()
        {
            ImageProvider.AssignIcon(bbiNew, CNETStandardIcons.NEW);
            ImageProvider.AssignIcon(bbiSave, CNETStandardIcons.SAVE);
            ImageProvider.AssignIcon(bbiDelete, CNETStandardIcons.DELETE);
            ImageProvider.AssignIcon(bsiOptions, CNETStandardIcons.OPTIONS);
            ImageProvider.AssignIcon(bbiSearch, CNETStandardIcons.SEARCH);
            ImageProvider.AssignIcon(bbiAddGuest, CNETStandardIcons.ADD);
        }
        public void InitializeUI()
        {
            CNETFooterRibbon.ribbonControl = ribbonControl1;
            cdeAddress.Logic.InitializeData(typeof(Preference));
            cdeAddress.SetTreeRelationshipFields("code", "parent");
            cdeAddress.SetColumns(new String[] { "description", "remark" });
            cdeAddress.Properties.ParentFieldName = "parent";
            cdeAddress.Properties.KeyFieldName = "code";
            cdeAddress.Logic.InitializeData(typeof(Preference));
            cdeAddress.GetTreeList().Columns[0].BestFit();
            cdeAddress.GetTreeList().Columns[0].OptionsColumn.AllowEdit = false;
            cdeAddress.GetTreeList().Columns[1].BestFit();
            cdeAddress.GetTreeList().Columns[1].Caption = @"Value";
            cdeAddress.GetTreeList().ShowingEditor += Form3_ShowingEditor;
            cdeAddress.GetTreeList().NodeCellStyle += frmPerson_NodeCellStyle;

            cdeAddress.GetTreeList().CustomNodeCellEdit += frmPerson_CustomNodeCellEdit;
            try
            {
                cdeAddress.GetTreeList().ValidateNode += OnValidateNode;
                cdeAddress.GetTreeList().InvalidNodeException += OnInvalidNodeException;
            }
            catch (Exception)
            {

            }

            var sb = new SimpleButton
            {
                Text = @"Add new",
                ButtonStyle = BorderStyles.UltraFlat,
                Location = new Point(84, 58),
                ShowFocusRectangle = DefaultBoolean.False,
                Size = new Size(75, 23),
                TabIndex = 4
            };
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

        private void gridView1_InvalidRowException(object sender, DevExpress.XtraGrid.Views.Base.InvalidRowExceptionEventArgs e)
        {
            //Suppress displaying the error message box
            e.ExceptionMode = ExceptionMode.NoAction;
        }

        private void CdeorgUnitDefListIdSetting_FocusedNodeChanged(object sender, FocusedNodeChangedEventArgs e)
        {

        }

        private void SystemLogic_ValidateRow(object sender, DevExpress.XtraGrid.Views.Base.ValidateRowEventArgs e)
        {
            GridView view = sender as GridView;
            GridColumn addressValue = view.Columns["value"];
            GridColumn addressType = view.Columns["description"];

            //Get the value of the first column
            Int16 inSt = (Int16)view.GetRowCellValue(e.RowHandle, addressValue);
            //Get the value of the second column
            //Validity criterion
            //if (inSt < onOrd)
            //{
            e.Valid = false;
            //    //Set errors with specific descriptions for the columns
            view.SetColumnError(addressValue, "The value must be greater than Units On Order");
            //}

            //if (addressType == "")
            //{

            //}
        }

        private void frmPerson_NodeCellStyle(object sender, GetCustomNodeCellStyleEventArgs e)
        {
            if (e.Node.HasChildren)
            {
                e.Appearance.BackColor = Color.FromArgb(224, 224, 224);
            }
        }

        private void Form3_ShowingEditor(object sender, CancelEventArgs e)
        {
            if (cdeAddress.GetTreeList().FocusedColumn != null && cdeAddress.GetTreeList().FocusedNode.HasChildren)
            {
                e.Cancel = true;
            }
        }


        private void frmPerson_CustomNodeCellEdit(object sender, GetCustomNodeCellEditEventArgs e)
        {
        }
        public void OnCreate()
        {
        }
        public void InitializeData()
        {
            Utility.AdjustRibbon(lciRibbonHolder);

            _currenDateTime = UIProcessManager.GetServerDateTime("Gregorian", "Server");
            var languageList = UIProcessManager.SelectAllLanguage();

            cacLanguage.Properties.Columns.Add(new LookUpColumnInfo("description", "Description"));
            cacLanguage.Properties.DisplayMember = "description";
            cacLanguage.Properties.ValueMember = "code";
            cacLanguage.Properties.DataSource = (languageList);

            cacTitle.Properties.Columns.Add(new LookUpColumnInfo("description", "Description"));
            cacTitle.Properties.DisplayMember = "description";
            cacTitle.Properties.ValueMember = "code";
            cacTitle.Properties.DataSource = (pertitleList.OrderByDescending(c => c.isDefault).ToList());

            var defaultTit = pertitleList.FirstOrDefault(c => c.isDefault);

            if (defaultTit != null)
            {
                cacTitle.EditValue = (defaultTit.code);
            }

            cacCurrency.Properties.Columns.Add(new LookUpColumnInfo("description", "Description"));
            cacCurrency.Properties.DisplayMember = "description";
            cacCurrency.Properties.ValueMember = "code";
            cacCurrency.Properties.DataSource = (currencyList.OrderByDescending(c => c.isDefault).ToList());

            var currency = currencyList.FirstOrDefault(c => c.isDefault);
            if (currency != null)
            {
                cacCurrency.EditValue = (currency.code);
            }

            var businessSegmentList = UIProcessManager.GetPreferenceByComponent(CNETConstantes.PERSON);
            cacBusinessSegment.Properties.Columns.Add(new LookUpColumnInfo("description", "Bus. Segment"));
            cacBusinessSegment.Properties.DisplayMember = "description";
            cacBusinessSegment.Properties.ValueMember = "code";
            cacBusinessSegment.Properties.DataSource = businessSegmentList;

            var mailingActionList = UIProcessManager.GetLookup(CNETConstantes.PRIVACYRULE);
            cacMailingAction.Properties.Columns.Add(new LookUpColumnInfo("description", "Mailing Methods"));
            cacMailingAction.Properties.DisplayMember = "description";
            cacMailingAction.Properties.ValueMember = "code";
            cacMailingAction.Properties.DataSource = (mailingActionList.OrderByDescending(c => c.isDefault).ToList());



            cacGender.Properties.Columns.Add(new LookUpColumnInfo("description", "Sex"));
            cacGender.Properties.DisplayMember = "description";
            cacGender.Properties.ValueMember = "code";
            cacGender.Properties.DataSource = (genderList.OrderByDescending(c => c.isDefault).ToList());
            var defaultSex = genderList.FirstOrDefault(c => c.isDefault);
            if (defaultSex != null)
            {
                cacGender.EditValue = (defaultSex.code);
            }
            var countryList = UIProcessManager.SelectAllCountry();

            cacNationality.Properties.Columns.Add(new LookUpColumnInfo("name", "Country"));
            cacNationality.Properties.Columns.Add(new LookUpColumnInfo("nationality", "Nationality"));
            cacNationality.Properties.DisplayMember = "nationality";
            cacNationality.Properties.ValueMember = "code";
            cacNationality.Properties.DataSource = countryList;

            var rateCodeList = PMSUIProcessManager.GetRateCodeCategoryCurrency();
            cacRateCode.Properties.Columns.Add(new LookUpColumnInfo("code", "Rate Code"));
            cacRateCode.Properties.Columns.Add(new LookUpColumnInfo("Category", "Category"));
            cacRateCode.Properties.Columns.Add(new LookUpColumnInfo("Currency", "Currency"));
            cacRateCode.Properties.DisplayMember = "code";
            cacRateCode.Properties.ValueMember = "code";
            cacRateCode.Properties.DataSource = (rateCodeList);

            var objectStateDefinitions = UIProcessManager.SelectAllObjectStateDefinition().Where(s => s.type == CNETConstantes.OBJECTSTATETYPEGUEST).ToList();
            cacVIP.Properties.Columns.Add(new LookUpColumnInfo("description", "Status"));
            cacVIP.Properties.DisplayMember = "description";
            cacVIP.Properties.ValueMember = "code";
            cacVIP.Properties.DataSource = (objectStateDefinitions);

            var accountList = UIProcessManager.SelectAllAccount();
            cacAccount.Properties.Columns.Add(new LookUpColumnInfo("description", "Account"));
            cacAccount.Properties.DisplayMember = "description";
            cacAccount.Properties.ValueMember = "code";
            cacAccount.Properties.DataSource = (accountList);
            deDateOfBirth.DateTime = DateTime.Now;
            ceIsActive.Checked = true;



            cdeAddress.SetData(addressAttribute);
        }
        public void LoadData(UILogicBase requesterForm, object args)
        {
            this.requesterForm = requesterForm;

            if (requesterForm.SubForm.GetType().Equals(typeof(frmReservation)))
            {
                rpgNew.Visible = true;
                rpgOptions.Visible = true;
                rpgSaveAndDelete.Visible = true;
                rpgSearch.Visible = true;
                rpgAddGuest.Visible = false;
            }
            if (requesterForm.SubForm.GetType().Equals(typeof(frmAccompanyingGuest)))
            {
                isFromAccGuest = true;
                // RegistrationVoucher = args.ToString();
                accForm = (frmAccompanyingGuest)args;
            }
        }

        private UILogicBase requesterForm;

        public override void Reset()
        {
            base.Reset();
            teLastName.Text = "";
            List<Preference> currentList = cdeAddress.Logic.bindinglList.Cast<Preference>().ToList();
            foreach (Preference pr in currentList)
            {

                pr.remark = "";
            }
            cdeAddress.SetData(currentList);
            teFirstName.Text = "";
            teMiddle.Text = "";
            tePassportNumber.Text = "";
            ceIsActive.Checked = true;
            deDateOfBirth.DateTime = DateTime.Now;
            cacNationality.EditValue = "";
            cacBusinessSegment.EditValue = "";
            cacLanguage.EditValue = "";
            cacAccount.EditValue = "";
            cacVIP.EditValue = "";
            cacRateCode.EditValue = "";
            cacMailingAction.EditValue = "";
            var defaultTit = pertitleList.FirstOrDefault(c => c.isDefault);
            if (defaultTit != null)
            {
                cacTitle.EditValue = (defaultTit.code);
            }

            var defaultSex = genderList.FirstOrDefault(c => c.isDefault);
            if (defaultSex != null)
            {
                cacGender.EditValue = (defaultSex.code);
            }

            var currency = currencyList.FirstOrDefault(c => c.isDefault);
            if (currency != null)
            {
                cacCurrency.EditValue = (currency.code);
            }

        }
        public SaveClickedResult OnSave()
        {
            List<Control> controls = new List<Control>
            {
                 cacNationality,
                 cacGender, 
                cacBusinessSegment,  
                teFirstName
            };

            IList<Control> invalidControls = CustomValidationRule.Validate(controls);

            if (invalidControls.Count > 0)
                return new SaveClickedResult { SaveResult = Enum.SaveResult.SAVE_THENSHOWNOTHING, MessageType = Enum.MessageType.ALLERT };
            Person person = new Person();
            CurrencyPreference cuPreference = new CurrencyPreference();
            Device device = new Device();
            string deviceName = System.Windows.Forms.SystemInformation.ComputerName;
            var posMachineid = UIProcessManager.getArticleCode(deviceName);
            posMachineid = UIProcessManager.getDeviceByArticle(posMachineid);
            device.code = posMachineid; if (Text == @"GUEST")
            {
                person.code = UIProcessManager.GenerateId(CNETConstantes.GSL_SYSTEM_PARAMETER, device, CNETConstantes.GUEST.ToString(), CNETConstantes.PERSON);
                //use Id Setting Table 
                person.type = CNETConstantes.GUEST;
            }
            else
            {

                person.code = UIProcessManager.GenerateId(CNETConstantes.GSL_SYSTEM_PARAMETER, device, CNETConstantes.CONTACT.ToString(), CNETConstantes.PERSON);//use Id Setting Table 
                person.type = CNETConstantes.CONTACT;
            }

            person.firstName = teFirstName.Text;
            person.middleName = teMiddle.Text;
            person.lastName = teLastName.Text;
            person.isActive = ceIsActive.Checked;

            if (cacNationality.EditValue != "")
            {
                person.nationality = cacNationality.EditValue.ToString();
            }
            if (deDateOfBirth.EditValue != null)
            {
                person.DOB = (DateTime)deDateOfBirth.EditValue;
            }
            if (cacTitle.EditValue != "")
            {
                person.title = cacTitle.EditValue.ToString();
            }
            if (cacGender.EditValue != "")
            {
                person.gender = cacGender.EditValue.ToString();
            }
            if (cacBusinessSegment.EditValue != "")
            {
                person.preference = cacBusinessSegment.EditValue.ToString();
            }
            else
            {
                person.preference = UIProcessManager.GetPreferenceByComponent(CNETConstantes.PERSON).FirstOrDefault().code;
            }

            bool isCreated = UIProcessManager.CreatePerson(person);
            if (isCreated)
            {
                if (Text == @"GUEST")
                {
                    //  UIProcessManager.GenerateId(CNETConstantes.GSL_SYSTEM_PARAMETER, device, CNETConstantes.GUEST.ToString(), CNETConstantes.PERSON);
                }
                else
                {
                    //  UIProcessManager.GenerateId(CNETConstantes.GSL_SYSTEM_PARAMETER, device, CNETConstantes.CONTACT.ToString(), CNETConstantes.PERSON);
                }
                //Identification
                if (!string.IsNullOrEmpty(tePassportNumber.Text))
                {
                    var aviltin = UIProcessManager.GetIdentificationByReference(person.code);
                    SavePassport(person.code, aviltin.Count > 0 ? aviltin : null);
                }

                // Currency
                if (cacCurrency.EditValue != "")
                {
                    cuPreference.Currency = cacCurrency.EditValue.ToString();
                    cuPreference.Code = "";// UIProcessManager.GenerateId("Currency", device, "Currency");
                    cuPreference.Reference = person.code;

                    UIProcessManager.CreateCurrencyPreference(cuPreference);
                }

                //Language
                if (cacLanguage.EditValue != "")
                {
                    LanguageOfInterest languageOfInterest = new LanguageOfInterest
                    {
                        code = String.Empty,
                        person = person.code,
                        language = cacLanguage.EditValue.ToString(),
                        proficiency = CNETConstantes.PROFICIENCYLEVELEXCLLENT
                    };

                    UIProcessManager.CreateLanguageOfInterest(languageOfInterest);
                }

                //Privancy for mailing Action
                if (cacMailingAction.EditValue != "")
                {
                    CNETPrivacy privacy = new CNETPrivacy
                    {
                        code = String.Empty,
                        PrivacyRule = cacMailingAction.EditValue.ToString(),
                        reference = person.code
                    };

                    UIProcessManager.CreateCNEtPrivacy(privacy);
                }

                //Negotiated Rate
                if (cacRateCode.EditValue != "")
                {
                    NegotiatedRate negorate = new NegotiatedRate
                    {
                        code = String.Empty,
                        consignee = person.code,
                        rateCode = cacRateCode.EditValue.ToString()
                    };

                    PMSUIProcessManager.CreateNegotiatedRate(negorate);
                }

                //Object State
                if (cacVIP.EditValue != "")
                {
                    ObjectState objstaObjectState = new ObjectState
                    {
                        code = String.Empty,
                        reference = person.code,
                        objectStateDefinition = cacVIP.EditValue.ToString()
                    };

                    List<ObjectState> objs = new List<ObjectState>();

                    objs.Add(objstaObjectState);

                    UIProcessManager.CreateObjectState(objs);
                }

                //Account
                if (cacAccount.EditValue != "")
                {
                    AccountMap acc = new AccountMap();
                    List<AccountMap> accList = new List<AccountMap>();

                    acc.code = String.Empty;
                    acc.account =
                        UIProcessManager.SelectAllAccount()
                            .Where(a => a.code == cacAccount.EditValue.ToString())
                            .Select(r => r.controlAccount)
                            .FirstOrDefault();
                    acc.description = cacAccount.EditValue.ToString();
                    acc.reference = person.code;
                    accList.Add(acc);

                    UIProcessManager.CreateAccountMap(accList);
                }

                string note = this.NoteContent;

                if (!string.IsNullOrEmpty(note))
                {
                    Voucher vo = new Voucher
                    {
                        code = UIProcessManager.GenerateId(CNETConstantes.VOUCHER_SYSTEM_PARAMETER, device, CNETConstantes.CASH_SALES.ToString(), CNETConstantes.VOUCHER_COMPONENET),
                        type = CNETConstantes.TRANSACTIONTYPENORMALTXN,
                        voucherDefinition = CNETConstantes.CASH_SALES,
                        component = CNETConstantes.PERSON,
                        consignee = person.code,
                        IssuedDate = _currenDateTime,
                        year = _currenDateTime.Year,
                        month = _currenDateTime.Month,
                        date = _currenDateTime.Day,
                        IsIssued = true,
                        IsVoid = false,
                        grandTotal = 0,
                        LastObjectState = CNETConstantes.AD_PREPARED,// "AD-004",
                        period = UIProcessManager.getPeriodCode(_currenDateTime)
                    };
                    // CNETConstantes.act
                    if (UIProcessManager.CreateVoucher(vo))
                    {
                        VoucherNote vNote = new VoucherNote();

                        vNote.code = string.Empty;
                        vNote.voucher = vo.code;
                        vNote.note = note;
                        UIProcessManager.CreateVoucherNote(vNote);


                    }
                }
                // temp address container
                List<Address> addresses = new List<Address>();

                var prefernce = UIProcessManager.GetPrefWithKeyword("Person").FirstOrDefault();
                if (prefernce != null)
                {
                    //   List<Lookup> addressAttribute = UIProcessManager.GetLookup("Address Attribute");
                    List<Preference> addresAddedRows = cdeAddress.Logic.GetAddedAndEditedRows().Cast<Preference>().ToList();

                    foreach (Preference pref in addresAddedRows)
                    {
                        Address address = new Address();
                        address.code = "";
                        address.value = pref.remark;
                        address.reference = person.code;
                        address.preference = pref.code;
                        UIProcessManager.CreateAddress(address);
                    }
                }
                if (isFromAccGuest)
                {
                    accForm.RegVoucher = accForm.RegistrationExt.Registration;
                    accForm.Person = person;
                    isFromAccGuest = false;
                    SystemMessage.ShowModalInfoMessage("SAVED SUCESSESFULLY", "MESSAGE");
                    this.Hide();
                    //  accForm.ShowDialog();

                }
                else
                {
                    SystemMessage.ShowModalInfoMessage("SAVED SUCESSESFULLY", "MESSAGE");
                }
                Reset();
                return new SaveClickedResult { SaveResult = Enum.SaveResult.SAVE_THENSHOWNOTHING, MessageType = Enum.MessageType.MESSAGEBOX };
            }
            else
            {
                //throw new SaveException("Saving Not Successful", true);
                SystemMessage.ShowModalInfoMessage("SAVE NOT SUCESSESFUL", "ERROR");
                return new SaveClickedResult { SaveResult = Enum.SaveResult.SAVE_THENSHOWNOTHING, MessageType = Enum.MessageType.MESSAGEBOX };

            }

        }
        public DeleteClickedResult OnDelete()
        {
            return new DeleteClickedResult { DeleteResult = Enum.DeleteResult.DELETE_SUCESSESFULLY, MessageType = Enum.MessageType.MESSAGEBOX };
        }
        private void bbiAddOn_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
        }
        private void SavePassport(string perCode, List<Identification> aviltin)
        {
            var tinList = new List<Identification>();
            var tin = new Identification
            {
                code = String.Empty,
                reference = perCode,
                description = "Passport",
                idNumber = tePassportNumber.Text,
                type = CNETConstantes.PERSONAL_IDENTIFICATIONTYPE_PASPORT
            };

            tinList.Add(tin);

            try
            {
                UIProcessManager.CreateIdentification(tinList);
            }
            catch (Exception)
            {
            }
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
            ShowNote();
        }

        private void bbiNew_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Reset();
        }

        frmAccompanyingGuest accForm = new frmAccompanyingGuest();
        public bool isFromAccGuest { get; set; }
        public string RegistrationVoucher { get; set; }

        private void frmPerson_Load(object sender, EventArgs e)
        {
            Reset();
        }

        private void bbiSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            OnSave();
        }
    }
}
