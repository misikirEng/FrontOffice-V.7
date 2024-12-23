using CNET.ERP.Client.Common.UI;
using CNET.FrontOffice_V._7;

using DevExpress.XtraEditors;
using System.Collections.Generic;
using System.Windows.Documents;
using System.Linq;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using System.Windows.Forms;
using DevExpress.XtraGrid.Columns;
using System;
using System.Text;
using DevExpress.XtraGrid.Views.Grid;
using CNET_V7_Domain.Domain.ConsigneeSchema;
using CNET_V7_Domain.Domain.ViewSchema;
using ProcessManager;
using CNET.FrontOffice_V._7.PMS.Common_Classes;
using CNET_V7_Domain.Domain.PmsSchema;
using CNET.FrontOffice_V._7.Forms;
using CNET_V7_Domain.Domain.SettingSchema;
using CNET_V7_Domain.Misc.PmsDTO;
using CNET_V7_Domain.Misc.PmsView;
using CNET_V7_Domain.Domain.TransactionSchema;
using CNET_V7_Domain.Domain.CommonSchema;
using CNET.FrontOffice_V._7.Forms.Non_Navigatable_Modals;

namespace CNET.FrontOffice_V._7.Non_Navigatable_Modals
{
    public partial class frmAccompanyingGuest : UILogicBase
    {
        private List<VoucherConsigneeListDTO> _accList = null;
        private List<AccompanyGuestVM> _accGuestVM = new List<AccompanyGuestVM>();
        public List<NegotiatedViewVM> accList = new List<NegotiatedViewVM>();
        private List<NegotiatedViewVM> negoList = new List<NegotiatedViewVM>();
        private RepositoryItemSearchLookUpEdit myLookup = null;


        ////////////////////////////////////// CONSTRUCTOR /////////////////////////////////////////
        public frmAccompanyingGuest()
        {
            InitializeComponent();
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;

            InitializeUI();

        }


        #region Properties
        private string regVoucher;
        internal string RegVoucher
        {
            get { return regVoucher; }
            set
            {
                regVoucher = value;
            }

        }

        private ConsigneeDTO person;
        internal ConsigneeDTO Person
        {
            get { return person; }
            set
            { person = value; }
        }



        //internal VwConsigneeViewDTO Person
        //{
        //    get { return person; }
        //    set
        //    { person = value;

        //        string addString = string.Empty;
        //        if (person != null)
        //        {
        //            AccompanyGuestVM accVM = new AccompanyGuestVM();
        //            //accVM.IdentficationDescription = person.Identifications;
        //            accVM.gslType = person.GslType;
        //            accVM.nationalityName = person.Nationality;
        //            accVM.PersonGender = person.GenderDescription;
        //            accVM.fullName = person.FirstName + " " + person.SecondName + " " + person.ThirdName;
        //            accVM.consignee = person.Code;
        //            accVM.Id = person.Id;
        //            accVM.Id = person.Id;
        //            accVM.address = string.Format("Phone Number {0}", person.Phone1);
        //            _accGuestVM.Add(accVM);
        //            gcAccGuest.DataSource = _accGuestVM;
        //            gcAccGuest.RefreshDataSource();
        //        }

        //    }
        //}

        private RegistrationListVMDTO registrationExt;
        internal RegistrationListVMDTO RegistrationExt
        {
            get { return registrationExt; }
            set
            {
                registrationExt = value;
                regVoucher = value.Registration;


            }
        }

        private const int CP_NOCLOSE_BUTTON = 0x200;
        private int adCode;
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


        private bool InitializeData()
        {
            try
            {
                if (RegistrationExt == null) return false;

                // Progress_Reporter.Show_Progress("Loading data. Please Wait...");

                //check workflow

                ActivityDefinitionDTO workFlow = UIProcessManager.GetActivityDefinitionBydescriptionandreference(CNETConstantes.LU_ACTIVITY_DEFINATION_ACCOMPYINGGUESTADDED, CNETConstantes.REGISTRATION_VOUCHER).FirstOrDefault();

                if (workFlow != null)
                {

                    adCode = workFlow.Id;
                }
                else
                {
                    // CNETInfoReporter.Hide();
                    SystemMessage.ShowModalInfoMessage("Please define workflow of Accompying Guest Added for Registration Voucher ", "ERROR");
                    return false;
                }

                //Check Activity Previlage
                var userRoleMapper = LocalBuffer.LocalBuffer.UserRoleMapperBufferList.FirstOrDefault(r => r.User == LocalBuffer.LocalBuffer.CurrentLoggedInUser.Id);
                if (userRoleMapper != null)
                {
                    var roleActivity = UIProcessManager.GetRoleActivityByactivityDefinition(workFlow.Id).FirstOrDefault(r => r.Role == userRoleMapper.Role && r.NeedsPassCode);
                    if (roleActivity != null)
                    {
                        frmNeedPassword frmNeedPass = new frmNeedPassword(true);

                        frmNeedPass.ShowDialog();
                        if (!frmNeedPass.IsAutenticated)
                        {
                            // CNETInfoReporter.Hide();
                            return false;
                        }

                    }

                }

                PopulateLookEdit();

                LoadAccompanyGrid();

                return true;

            }
            catch (Exception ex)
            {
                // CNETInfoReporter.Hide();
                XtraMessageBox.Show("Error in initializing data. DETAIL: " + ex.Message, "CNET_v2016", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void InitializeUI()
        {

            myLookup = new DevExpress.XtraEditors.Repository.RepositoryItemSearchLookUpEdit();
            GridColumn column = risle_accGuest.View.Columns.AddField("consignee");
            column.Caption = "consignee";
            column.Width = 50;
            column.Visible = true;

            column = risle_accGuest.View.Columns.AddField("fullName");
            column.Caption = "Full Name";
            column.Width = 100;
            column.Visible = true;


            column = risle_accGuest.View.Columns.AddField("Passport");
            column.Visible = true;

            risle_accGuest.ShowFooter = true;
            //myLookup.DataSource = negoList;
            //myLookup.NullText = "";
            risle_accGuest.EditValueChanged += risle_accGuest_EditValueChanged;
            risle_accGuest.AddNewValue += risle_accGuest_AddNewValue;
        }




        #region Methods

        private void PopulateLookEdit()
        {
            List<ConsigneeDTO> _personList = LocalBuffer.LocalBuffer.AllGuestConsigneeViewlist.Where(p => p.Id != RegistrationExt.GuestId).ToList();
            if (_personList == null) return;
            List<AccompanyGuestVM> personVM = new List<AccompanyGuestVM>();
            foreach (var person in _personList)
            {
                AccompanyGuestVM accGuestVM = new AccompanyGuestVM();
                accGuestVM.fullName = person.FirstName + " " + person.SecondName + " " + person.ThirdName;
                accGuestVM.consignee = person.Code;
                accGuestVM.Id = person.Id;
                accGuestVM.Passport = person.BioId;
                //IdentificationDTO passport = LocalBuffer.LocalBuffer.IdentificationBufferList.FirstOrDefault(i => i.Type == CNETConstantes.PERSONAL_IDENTIFICATIONTYPE_PASPORT && i.Consignee == person.Id);
                //if (passport != null)
                //{
                //}

                personVM.Add(accGuestVM);
            }
            risle_accGuest.DataSource = personVM;
        }

        private void LoadAccompanyGrid()
        {
            _accGuestVM.Clear();

            gcAccGuest.DataSource = null;

            if (_accList == null)
            {
                if (registrationExt == null || string.IsNullOrEmpty(registrationExt.Registration)) return;
                _accList = UIProcessManager.GetVoucherConsigneeListByVoucher(registrationExt.Id);
            }


            if (_accList == null) return;
            int count = 0;
            foreach (var guest in _accList)
            {
                ConsigneeDTO Guests = LocalBuffer.LocalBuffer.ConsigneeViewBufferList.FirstOrDefault(x => x.Id == guest.Consignee);
                if (Guests != null)
                {
                    VwConsigneeViewDTO vwConsignee = UIProcessManager.GetConsigneeViewById(guest.Id);
                    AccompanyGuestVM accVM = new AccompanyGuestVM();
                    accVM.IdentficationDescription = "Identfication";
                    accVM.gslType = Guests.GslType;
                    accVM.nationalityName = vwConsignee.Nationality;
                    accVM.PersonGender = vwConsignee.GenderDescription;
                    accVM.fullName = vwConsignee.FullName;
                    accVM.Title = vwConsignee.TitleDescription;
                    accVM.Id = Guests.Id;
                    accVM.consignee = Guests.Code;
                    accVM.address = "";
                    count++;
                    accVM.SN = count;
                    accVM.IsSaved = true;
                    _accGuestVM.Add(accVM);
                }
            }
            gcAccGuest.DataSource = _accGuestVM;
            gcAccGuest.RefreshDataSource();
            AddLastRow();
        }


        private void AddLastRow(bool isDelete = false)
        {
            List<AccompanyGuestVM> dtoList = gcAccGuest.DataSource as List<AccompanyGuestVM>;
            if (dtoList == null || dtoList.Count == 0)
            {
                dtoList = new List<AccompanyGuestVM>();
                AccompanyGuestVM dto = new AccompanyGuestVM()
                {
                    SN = 1,
                    consignee = null,
                    fullName = "",
                    address = ""

                };
                dtoList.Add(dto);
                gcAccGuest.DataSource = dtoList;
                gv_accGuest.RefreshData();

                _accGuestVM = dtoList;
                return;
            }

            AccompanyGuestVM lastEmptyDto = dtoList.OrderByDescending(l => l.SN).FirstOrDefault(f => string.IsNullOrWhiteSpace(f.consignee));

            if (isDelete)
            {
                if (lastEmptyDto != null)
                {
                    lastEmptyDto.SN = lastEmptyDto.SN - 1;
                    gcAccGuest.DataSource = dtoList;
                    gv_accGuest.RefreshData();
                    _accGuestVM = dtoList;

                    gv_accGuest.FocusedColumn = gv_accGuest.Columns[0];
                    return;
                }
            }
            else
            {
                if (lastEmptyDto != null)
                    return;
            }

            AccompanyGuestVM lastDto = dtoList.OrderByDescending(l => l.SN).FirstOrDefault(f => !string.IsNullOrWhiteSpace(f.consignee));
            if (lastDto != null)
            {
                AccompanyGuestVM dto = new AccompanyGuestVM()
                {
                    SN = lastDto.SN + 1,
                    consignee = null,
                    fullName = "",
                    address = ""
                };
                dtoList.Add(dto);
                gcAccGuest.DataSource = dtoList;
                gv_accGuest.RefreshData();
            }

            _accGuestVM = dtoList;
        }

        #endregion




        #region Event Handlers

        void risle_accGuest_EditValueChanged(object sender, System.EventArgs e)
        {
            var searchLookUpEdit = sender as SearchLookUpEdit;
            if (searchLookUpEdit != null && searchLookUpEdit.EditValue != null)
            {
                string value = searchLookUpEdit.EditValue.ToString();
                if (value != "")
                {
                    List<AccompanyGuestVM> lookupPersons = risle_accGuest.DataSource as List<AccompanyGuestVM>;

                    if (lookupPersons == null) return;

                    AccompanyGuestVM selectedPerson = lookupPersons.Where(p => p.consignee == value).FirstOrDefault();

                    if (selectedPerson == null) return;

                    //check guest existance in the list
                    AccompanyGuestVM guest = _accGuestVM.Where(g => g.consignee == selectedPerson.consignee).FirstOrDefault();
                    if (guest == null)
                    {
                        AccompanyGuestVM focusedGuest = gv_accGuest.GetFocusedRow() as AccompanyGuestVM;
                        int index = _accGuestVM.FindIndex(0, g => g.consignee == focusedGuest.consignee);

                        try
                        {
                            selectedPerson.address = "";

                        }
                        catch (Exception ex) { }

                        _accGuestVM.Remove(focusedGuest);
                        selectedPerson.SN = index + 1;
                        _accGuestVM.Insert(index, selectedPerson);

                        gcAccGuest.DataSource = _accGuestVM;
                        gcAccGuest.RefreshDataSource();
                        gv_accGuest.RefreshData();

                        AddLastRow();
                    }
                    else
                    {
                        AccompanyGuestVM focusedGuest = gv_accGuest.GetFocusedRow() as AccompanyGuestVM;
                        int index = _accGuestVM.FindIndex(0, g => g.consignee == focusedGuest.consignee);
                        gv_accGuest.DeleteRow(index);
                        gv_accGuest.RefreshData();
                        AddLastRow(true);
                    }





                }
            }
            //cdeAccompanyGuest.GetGridView().FocusedColumn = cdeAccompanyGuest.GetGridView().Columns["address"];
        }

        private void bbiNew_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            _accGuestVM.Add(new AccompanyGuestVM() { consignee = null, fullName = "", address = "" });
            gcAccGuest.BeginUpdate();

            gcAccGuest.DataSource = _accGuestVM;
            gcAccGuest.RefreshDataSource();

            gcAccGuest.EndUpdate();

        }

        private void frmAccompanyingGuest_Load(object sender, System.EventArgs e)
        {
            if (!InitializeData())
            {
                this.Close();
            }
        }

        private void bbiSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            bool flag = true;
            try
            {
                if (RegistrationExt == null || string.IsNullOrEmpty(RegistrationExt.Registration))
                {
                    SystemMessage.ShowModalInfoMessage("Unable to save accompanying guest. Please try again!", "ERROR");
                    return;
                }
                List<VoucherConsigneeListDTO> accConsinees = UIProcessManager.GetVoucherConsigneeListByVoucher(RegistrationExt.Id);
                if (accConsinees != null)
                {
                    if (accConsinees.Count > 0)
                    {
                        foreach (var con in accConsinees)
                        {
                            flag = UIProcessManager.DeleteVoucherConsigneeListById(con.Id);
                        }
                        if (!flag)
                        {
                            SystemMessage.ShowModalInfoMessage("Unable to Delete previous accompanying guest. Please try again!", "ERROR");
                            return;
                        }

                    }

                    List<AccompanyGuestVM> filterdVM = _accGuestVM.Where(ag => !string.IsNullOrEmpty(ag.consignee)).ToList();
                    if (filterdVM == null || filterdVM.Count == 0)
                    {
                        SystemMessage.ShowModalInfoMessage("No accompanying guest is selected!", "ERROR");
                        return;
                    }

                    DateTime? currentTime = UIProcessManager.GetServiceTime();
                    //update number of adults in registration detail
                    List<RegistrationDetailDTO> regDetailList = new List<RegistrationDetailDTO>();
                    List<RegistrationDetailDTO> ALLregDetailList = UIProcessManager.GetRegistrationDetailByvoucher(RegistrationExt.Id).ToList();
                    if (ALLregDetailList == null)
                    {
                        SystemMessage.ShowModalInfoMessage("Unable to get registration detail for " + RegistrationExt.Registration, "ERROR");
                        return;
                    }

                    if (currentTime != null)
                    {
                        regDetailList = ALLregDetailList.Where(x => x.Date >= currentTime.Value.Date).ToList();
                    }
                    if (regDetailList == null || regDetailList.Count == 0)
                    {
                        SystemMessage.ShowModalInfoMessage("There is no registration detail for " + RegistrationExt.Registration + " for today and after please Amend the registration Date !", "ERROR");
                        return;
                    }


                    int adultNumber = filterdVM.Count + 1;
                    foreach (var regDetail in regDetailList)
                    {
                        regDetail.Adult = adultNumber;
                        if (UIProcessManager.UpdateRegistrationDetail(regDetail) == null)
                        {
                            flag = false;
                        }
                    }

                    if (!flag)
                    {
                        SystemMessage.ShowModalInfoMessage("Unable to update registration detail for " + RegistrationExt.Registration, "ERROR");
                        return;
                    }

                    int counter = 0;
                    StringBuilder addedGuests = new StringBuilder();
                    foreach (var person in filterdVM)
                    {
                        person.IsSaved = true;
                        VoucherConsigneeListDTO newConsignee = new VoucherConsigneeListDTO();
                        newConsignee.Type = null;
                        newConsignee.Consignee = person.Id;
                        newConsignee.Voucher = registrationExt.Id;
                        if (UIProcessManager.CreateVoucherConsigneeList(newConsignee) == null)
                        {
                            flag = false;
                            break;
                        }
                        else
                        {
                            addedGuests.Append(person.fullName);
                            addedGuests.Append(" | ");
                            counter = counter + 1;
                        }


                    }
                    if (flag)
                    {
                        SystemMessage.ShowModalInfoMessage("New Accompanying Guest(s) Added Successfully!", "MESSAGE");
                        if (currentTime != null)
                        {

                            ActivityDTO activity = ActivityLogManager.SetupActivity(currentTime.Value, adCode, CNETConstantes.PMS_Pointer, addedGuests.ToString());
                            activity.Reference = RegistrationExt.Id;
                            UIProcessManager.CreateActivity(activity);

                        }

                        //Load buffer
                        //MasterPageForm.LoadAccompanyGuestBuffer();
                    }
                    else
                    {
                        SystemMessage.ShowModalInfoMessage("Unable to save Accompanying guest", "ERROR");
                        foreach (var regDetail in regDetailList)
                        {
                            regDetail.Adult = counter + 1;
                            if (UIProcessManager.UpdateRegistrationDetail(regDetail) == null)
                            {
                                SystemMessage.ShowModalInfoMessage("Unable to update registration detail for " + RegistrationExt.Registration, "ERROR");
                                return;
                            }
                        }

                    }
                }
                else
                {
                    SystemMessage.ShowModalInfoMessage("Unable to save Accompanying guest", "ERROR");
                }
            }
            catch (Exception ex)
            {
                SystemMessage.ShowModalInfoMessage("Unable to save Accompanying guest. ERROR DETAIL::" + ex.Message, "ERROR");
            }
        }

        private void bbiClose_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Close();
        }

        private void bbiDelete_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            bool isDeleted = true;

            AccompanyGuestVM accGuestVM = (AccompanyGuestVM)gv_accGuest.GetFocusedRow();
            if (accGuestVM == null) return;

            if (!accGuestVM.IsSaved)
            {
                int index = _accGuestVM.FindIndex(0, g => g.consignee == accGuestVM.consignee);
                gv_accGuest.DeleteRow(index);
                gv_accGuest.RefreshData();
                AddLastRow(true);
                return;
            }

            bool flag = true;
            //update number of adults in registration detail
            List<RegistrationDetailDTO> regDetailList = UIProcessManager.GetRegistrationDetailByvoucher(RegistrationExt.Id).ToList();
            if (regDetailList == null)
            {
                SystemMessage.ShowModalInfoMessage("Unable to get registration detail for " + RegistrationExt.Registration, "ERROR");
                return;
            }

            foreach (var regDetail in regDetailList)
            {
                if (regDetail.Adult > 1)
                    regDetail.Adult = regDetail.Adult - 1;

                if (UIProcessManager.UpdateRegistrationDetail(regDetail) == null)
                {
                    flag = false;
                }
            }

            if (!flag)
            {
                SystemMessage.ShowModalInfoMessage("Unable to update registration detail for " + RegistrationExt.Registration, "ERROR");
                return;
            }

            // DELETE accompany guest
            List<VoucherConsigneeListDTO> accConsinees = UIProcessManager.GetVoucherConsigneeListByconsigneeandVoucher(accGuestVM.Id, registrationExt.Id).ToList();
            if (accConsinees != null && accConsinees.Count > 0)
            {
                foreach (VoucherConsigneeListDTO con in accConsinees)
                {
                    if (!UIProcessManager.DeleteVoucherConsigneeListById(con.Id))
                    {
                        isDeleted = false;
                    }
                }
            }
            else
            {
                isDeleted = false;
            }
            if (isDeleted)
            {
                SystemMessage.ShowModalInfoMessage("Accompanying Guest Removed Successfully!", "MESSAGE");

                _accGuestVM.Remove(accGuestVM);
                gcAccGuest.BeginUpdate();

                gcAccGuest.DataSource = _accGuestVM;
                gcAccGuest.RefreshDataSource();

                gcAccGuest.EndUpdate();

                AddLastRow(true);

                //Load buffer
                // MasterPageForm.LoadAccompanyGuestBuffer();


            }
            else
            {
                SystemMessage.ShowModalInfoMessage("Unable to delete the selected accompanying guest", "ERROR");

                //role back registration detail update
                foreach (var regDetail in regDetailList)
                {
                    regDetail.Adult = regDetail.Adult + 1;

                    if (UIProcessManager.UpdateRegistrationDetail(regDetail) != null)
                    {
                        //ERROR
                    }
                }
            }

        }

        void risle_accGuest_AddNewValue(object sender, AddNewValueEventArgs e)
        {
            this.SubForm = this;
            // Home.OpenForm(this, "MENU//PROFILE//GUEST", this);
            frmPerson person = new frmPerson("Guest");
            person.Text = "Guest";
            person.GSLType = CNETConstantes.GUEST;
            person.rpgScanFingerPrint.Visible = true;
            person.LoadEventArg.Args = "Guest";
            person.LoadData(this, this);
            person.LoadEventArg.Sender = null;
            if (person.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (this.person != null)
                    LocalBuffer.LocalBuffer.AllGuestConsigneeViewlist.Add(this.person);
                //cdeAccompanyGuest.Logic.SetData(accList);
                // Progress_Reporter.Show_Progress("Loading persons...");
                PopulateLookEdit();

                List<AccompanyGuestVM> lookupPersons = risle_accGuest.DataSource as List<AccompanyGuestVM>;
                if (lookupPersons == null) return;
                AccompanyGuestVM selectedPerson = lookupPersons.FirstOrDefault(p => p.Id == this.person.Id);
                //gcAccGuest.DataSource = null;
                //gcAccGuest.RefreshDataSource();
                if (selectedPerson != null)
                {
                    AccompanyGuestVM focusedGuest = gv_accGuest.GetFocusedRow() as AccompanyGuestVM;
                    int index = _accGuestVM.FindIndex(0, g => g.consignee == focusedGuest.consignee);


                    _accGuestVM.Remove(focusedGuest);
                    selectedPerson.SN = index + 1;
                    _accGuestVM.Insert(index, selectedPerson);

                    gcAccGuest.DataSource = _accGuestVM;
                    gcAccGuest.RefreshDataSource();
                    gv_accGuest.RefreshData();

                    AddLastRow();


                }



                //e.Cancel = false;
                //e.NewValue = this.person.Id;
                // CNETInfoReporter.Hide();
            }
            //this.Hide();
        }

        private void bbiIssueCard_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmDoorLock frmDoorLock = new frmDoorLock();
            frmDoorLock.RegExt = RegistrationExt;
            frmDoorLock.ShowDialog();
        }

        private void gv_accGuest_RowStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs e)
        {
            GridView view = sender as GridView;
            if (view == null) return;
            AccompanyGuestVM dto = view.GetRow(e.RowHandle) as AccompanyGuestVM;
            if (dto == null) return;
            dto.SN = (e.RowHandle + 1);
        }


        #endregion


        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();

                if (_accGuestVM != null)
                {
                    _accGuestVM.Clear();
                    _accGuestVM = null;
                }

                if (_accList != null)
                {
                    _accList.Clear();
                    _accList = null;
                }

                RegistrationExt = null;
                Person = null;

            }
            base.Dispose(disposing);
        }















    }
}
