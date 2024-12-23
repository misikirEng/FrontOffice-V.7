
using CNET.ERP.Client.Common.UI; 
using CNET.FrontOffice_V._7;
using ProcessManager;
using CNET.FrontOffice_V._7.PMS.Common_Classes;
using CNET.FrontOffice_V._7.PMS.DoorLockIntegration.DoorLock;
using CNET.FrontOffice_V._7.PMS.DTO;
using CNET.Progress.Reporter;
using CNET_V7_Domain.Domain.ArticleSchema;
using CNET_V7_Domain.Domain.PmsSchema;
using CNET_V7_Domain.Domain.SettingSchema;
using CNET_V7_Domain.Domain.TransactionSchema;
using CNET_V7_Domain.Misc.PmsDTO;
using CNET_V7_Domain.Misc.PmsView;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DocumentPrint;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CNET_V7_Domain.Misc;

namespace CNET.FrontOffice_V._7.Forms.Pinned_Tabs
{
    public partial class frmPackageAudit : UILogicBase
    {

        private List<PackageHeaderDTO> _pkgHeaderList = null;

        private IDoorLock _doorLock = null;

        private List<VwPackageViewDTO> _pkgAuditViewList = null;

        //private List<RegistrationDocumentDTO> _regDocList = null;
        private List<RegistrationListVMDTO> regListVM = new List<RegistrationListVMDTO>();
    
        public DateTime CurrentDate { get; set; }
        public frmPackageAudit()
        {
            InitializeComponent();

            InitializeUI();

        }

        #region Helper Methods

        private void InitializeUI()
        {
            //Packages
            GridColumn col = gvPackagesList.Columns.AddVisible("Id", "Id");
            col.Visible = false;
            col.Width = 10;
            col = gvPackagesList.Columns.AddVisible("Description", "Name");
            col.Visible = true;
            ripoSlukPackages.ValueMember = "Id";
            ripoSlukPackages.DisplayMember = "Description";

            //inhouse-guests
            GridColumn columnGuest = gvGuestList.Columns.AddField("Registration");
            columnGuest.Visible = true;
            columnGuest = gvGuestList.Columns.AddField("Id"); 
            columnGuest.Visible = false;
            columnGuest = gvGuestList.Columns.AddField("Guest");
            columnGuest.Caption = "Guest";
            columnGuest.Visible = true;
            columnGuest = gvGuestList.Columns.AddField("RoomTypeDescription");
            columnGuest.Caption = "Room Type";
            columnGuest.Visible = true;
            columnGuest = gvGuestList.Columns.AddField("Room");
            columnGuest.Visible = true;

            repoSlukGuest.DisplayMember = "Guest";
            repoSlukGuest.ValueMember = "Id";

            repoSlukGuest.EditValueChanged += repoSlukGuest_EditValueChanged;
            ripoSlukPackages.EditValueChanged += ripoSlukPackages_EditValueChanged;
        }

        private bool InitializeData()
        {
            try
            {
                //Progress_Reporter.Show_Progress("Initializing Door Lock", "Please Wait...");
                beiGuests.EditValue = null;
                beiPackage.EditValue = null;
                //get Door lock device 
                DoorLockFactory dLockFactory = new DoorLockFactory();
                _doorLock = dLockFactory.GetDoorLock(false);

                if (_doorLock == null)
                {
                    //if there is no device, show guest list and ok button
                    beiGuests.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    rpgOk.Visible = true;
                    bbiReadCard.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;


                    DateTime? currentDate = UIProcessManager.GetServiceTime();
                    if (currentDate == null) return false;
                    CurrentDate = currentDate.Value;

                    beiDate.EditValue = CurrentDate;



                    if (regListVM != null)
                        regListVM.Clear();

                    regListVM =  UIProcessManager.GetRegistrationViewModelData(null, null,CNETConstantes.CHECKED_IN_STATE,  SelectedHotelcode.Value);
                     

                    if (regListVM != null)
                    {
                        regListVM = regListVM.Where(x => x.Departure.Date >= CurrentDate.Date && x.Arrival.Date <= CurrentDate.Date).ToList();
                        
                    }

                    repoSlukGuest.DataSource = regListVM;

                }
                


                

                //Populate packages
              //  _pkgHeaderList = UIProcessManager.SelectAllPackageByBranch();
                if (SelectedHotelcode != null)
                    _pkgHeaderList = UIProcessManager.GetAllPackageHeaderByConsigneeUnit(SelectedHotelcode.Value);

                ripoSlukPackages.DataSource = _pkgHeaderList;

                return true;
            }
            catch (Exception ex)
            {
                SystemMessage.ShowModalInfoMessage("Error in initializing package audit. Detail: " + ex.Message, "ERROR");
                return false;
            }
        } 

        private void PopulatePackageAuditList(int pkgHeader)
        {
            List<PackageAuditVM> _dtoList = new List<PackageAuditVM>();
            _pkgAuditViewList = UIProcessManager.GetPackageView(CurrentDate,SelectedHotelcode).Where(p => p.pkgHeaderId == pkgHeader).ToList();

            if (_pkgAuditViewList != null)
            {
                _pkgAuditViewList = _pkgAuditViewList.GroupBy(x=> x.RegId).Select(s=> s.FirstOrDefault()).ToList();

                foreach (var pkgAudit in _pkgAuditViewList)
                {
                    PackageAuditVM dto = new PackageAuditVM();
                    dto.Guest = pkgAudit.Guest;
                    dto.RegNum = pkgAudit.RegNum;
                    dto.Room = pkgAudit.Room;
                    dto.RoomType = pkgAudit.RoomType;
                    dto.VoucherCode = pkgAudit.VoucherCode;
                    dto.AdultCount = pkgAudit.AdultCount.Value;
                    dto.ChildCount = pkgAudit.ChildCount.Value;
                    _dtoList.Add(dto);
                }
            }
            gcConsumedPackages.DataSource = _dtoList;
            gvConsumedPackages.RefreshData();
        }

     
         

        private void SavePackageConsumptionVoucher(int regVoucherCode, int pkgHeader, string pkgDescription)
        {

            //check the package is alredy auditd
            if (_pkgAuditViewList != null)
            {
                var pkgAuditView = _pkgAuditViewList.FirstOrDefault(p => p.RegId == regVoucherCode && p.pkgHeaderId == pkgHeader);
                if (pkgAuditView != null)
                {
                    SystemMessage.ShowModalInfoMessage("Package is already audited!", "ERROR");
                    return;

                }

            }

            //generate id
            string generatedId = UIProcessManager.IdGenerater("Voucher",
                               CNETConstantes.PACKAGE_CONSUMPTION_VOUCHER,1,LocalBuffer.LocalBuffer.CurrentDeviceConsigneeUnit.Value,false,LocalBuffer.LocalBuffer.CurrentDevice.Id);
            if (string.IsNullOrEmpty(generatedId))
            {
                SystemMessage.ShowModalInfoMessage("There is a problem on ID setting!", "ERROR");
                return;
            }


            //check workflow
            int? adCode = null;
            int? lastState =null;
            ActivityDefinitionDTO workFlow = UIProcessManager.GetActivityDefinitionBydescriptionandreference(CNETConstantes.LU_ACTIVITY_DEFINATION_PREPARED ,CNETConstantes.PACKAGE_CONSUMPTION_VOUCHER).FirstOrDefault();

            if (workFlow != null)
            {

                adCode = workFlow.Id;
                lastState = workFlow.State;
            }
            else
            {
                SystemMessage.ShowModalInfoMessage("Please define workflow of PREPARED for Package Consumption Voucher ", "ERROR");
                return;
            }

            //get current time
            DateTime? currentTime = UIProcessManager.GetServiceTime();
            if (currentTime == null) return;

            //get registration voucher
            VoucherDTO regVoucher = UIProcessManager.GetVoucherById(regVoucherCode);
            if (regVoucher == null)
            {
                SystemMessage.ShowModalInfoMessage("Unable to get registration voucher!", "ERROR");
                return;
            }

            //get today's registration detail
            RegistrationDetailDTO regDetail = UIProcessManager.GetRegistrationDetailByvoucher(regVoucher.Id).FirstOrDefault(r => r.Date.Value.Date == currentTime.Value.Date);

            if (regDetail == null)
            {
                //get the previous day registrtion detail
                regDetail = UIProcessManager.GetRegistrationDetailByvoucher(regVoucher.Id).FirstOrDefault(r => r.Date.Value.Date == currentTime.Value.Date.Subtract(TimeSpan.FromDays(1)));
            }
            
            if (regDetail == null)
            {
                SystemMessage.ShowModalInfoMessage("Unable to get registration detail for the current date!", "ERROR");
                return;
            }
            var pkgToPosdfgdt = UIProcessManager.GetPostingPackageToPostViewByRegistrationDetail(regDetail.Id);
            //get package to post 
            VwPackageToPostViewDTO pkgToPost = UIProcessManager.GetPostingPackageToPostViewByRegistrationDetail(regDetail.Id).FirstOrDefault( p => p.packageHeader == pkgHeader);
            if (pkgToPost == null)
            {
                SystemMessage.ShowModalInfoMessage("There is no " + pkgDescription + " for the current registration!" , "ERROR");
                return;
            }


            VoucherBuffer voucherbuffer = new VoucherBuffer();
            voucherbuffer.Voucher = new VoucherDTO();
            voucherbuffer.Voucher.Definition = CNETConstantes.PACKAGE_CONSUMPTION_VOUCHER;
            voucherbuffer.Voucher.Code = generatedId;
            voucherbuffer.Voucher.Consignee1 = regVoucher.Consignee1;
            voucherbuffer.Voucher.IssuedDate = currentTime.Value; 
            voucherbuffer.Voucher.Type = CNETConstantes.TRANSACTIONTYPENORMALTXN;
            voucherbuffer.Voucher.IsIssued = true;
            voucherbuffer.Voucher.IsVoid = false;
            voucherbuffer.Voucher.Year = currentTime.Value.Date.Year;
            voucherbuffer.Voucher.Month = currentTime.Value.Date.Month;
            voucherbuffer.Voucher.Day = currentTime.Value.Date.Day;
            voucherbuffer.Voucher.GrandTotal = pkgToPost.amount.Value;
            voucherbuffer.Voucher.SubTotal = pkgToPost.amount.Value;
            voucherbuffer.Voucher.AddCharge = 0;
            voucherbuffer.Voucher.Discount = 0;
            voucherbuffer.Voucher.LastUser = LocalBuffer.LocalBuffer.CurrentLoggedInUser.Id;
            voucherbuffer.Voucher.LastDevice = LocalBuffer.LocalBuffer.CurrentDevice.Id;
            voucherbuffer.Voucher.OriginConsigneeUnit = LocalBuffer.LocalBuffer.CurrentDeviceConsigneeUnit;

            voucherbuffer.LineItemsBuffer = new List<LineItemBuffer>();

            LineItemBuffer lineItemBuffer = new LineItemBuffer();
            ArticleDTO article = UIProcessManager.GetArticleById(pkgToPost.article.Value);
            lineItemBuffer.LineItem = new LineItemDTO()
            {
                Article = article.Id,
                UnitAmount = pkgToPost.amount.Value,
                Quantity = 1,
                Uom = article != null ? article.Uom : null,
                TotalAmount = pkgToPost.amount.Value,
                TaxableAmount = 0,
                Tax = CNETConstantes.VAT,
                TaxAmount = 0,
                CalculatedCost = 0,
                ObjectState= null
            };
            voucherbuffer.LineItemsBuffer.Add(lineItemBuffer);


            voucherbuffer.TransactionReferencesBuffer = new List<TransactionReferenceBuffer>();
            TransactionReferenceBuffer TRBuffer = new TransactionReferenceBuffer();
            TRBuffer.TransactionReference = new TransactionReferenceDTO()
            {
                ReferencedVoucherDefn = CNETConstantes.REGISTRATION_VOUCHER,
                ReferencingVoucherDefn = CNETConstantes.PACKAGE_CONSUMPTION_VOUCHER,
                Referenced = regVoucher.Id,
                Remark = pkgHeader.ToString()
            };
            TRBuffer.ReferencedActivity = null;
            voucherbuffer.TransactionReferencesBuffer.Add(TRBuffer);
            voucherbuffer.Activity = ActivityLogManager.SetupActivity(currentTime.Value, adCode.Value, CNETConstantes.PMS_Pointer, pkgDescription);

            voucherbuffer.Voucher.LastUser = LocalBuffer.LocalBuffer.CurrentLoggedInUser.Id;
            voucherbuffer.Voucher.LastDevice = LocalBuffer.LocalBuffer.CurrentDevice.Id;
            voucherbuffer.TransactionCurrencyBuffer = null;

            ResponseModel<VoucherBuffer> isSaved = UIProcessManager.CreateVoucherBuffer(voucherbuffer);
            if (isSaved == null || !isSaved.Success)
            {
                SystemMessage.ShowModalInfoMessage("Unable to save package consumption voucher!" + Environment.NewLine+ isSaved.Message, "ERROR");
                return;
            }
            
              

            SystemMessage.ShowModalInfoMessage("Package Audit is Saved!!", "MESSAGE");


            //refresh grid
            PopulatePackageAuditList(pkgHeader);

            



        }

        #endregion


        #region Event Handlers

        private void frmPackageAudit_Load(object sender, EventArgs e)
        { 
            reiHotel.DisplayMember = "Name";
            reiHotel.ValueMember = "Id";
            reiHotel.DataSource = (LocalBuffer.LocalBuffer.HotelBranchBufferList.Select(x => new { x.Id, x.Name })).ToList();

            beiHotel.EditValue = LocalBuffer.LocalBuffer.CurrentDeviceConsigneeUnit;
            reiHotel.ReadOnly = !LocalBuffer.LocalBuffer.UserHasHotelBranchAccess;
            //if (!InitializeData())
            //{
            //    rcPackageAudit.Enabled = false;

            //}
        }

        private void bbiReadCard_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (beiPackage.EditValue == null || string.IsNullOrWhiteSpace(beiPackage.EditValue.ToString()))
                {
                    SystemMessage.ShowModalInfoMessage("Please select package first!", "ERROR");
                    return;
                }

               

                Progress_Reporter.Show_Progress("Reading Guest Card", "Please Wait.......");

                string cardSn = _doorLock.GetCardSN();
                if (!string.IsNullOrEmpty(cardSn))
                {
                  List<VoucherDTO> tranList =  UIProcessManager.GetVoucherByExtension1(cardSn); 
                    if (tranList == null || tranList.Count == 0)
                    {
                        Progress_Reporter.Close_Progress();
                        SystemMessage.ShowModalInfoMessage("No card issued with the current guest card!", "ERROR");
                        return;
                    }

                    var tran = tranList.FirstOrDefault();

                    Progress_Reporter.Show_Progress("Saving Package Consumption Voucher", "Please Wait.......");
                    var pkg = _pkgHeaderList.FirstOrDefault(p => p.Id == Convert.ToInt32( beiPackage.EditValue));
                    SavePackageConsumptionVoucher(tran.Id,Convert.ToInt32( beiPackage.EditValue), pkg == null? "": pkg.Description);
                }

                Progress_Reporter.Close_Progress();

            }
            catch (Exception ex)
            {
                Progress_Reporter.Close_Progress();
                SystemMessage.ShowModalInfoMessage("Error in saving package audit! Detail:: " + ex.Message, "ERROR");
            }

        }

        private void bbiOk_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (beiPackage.EditValue == null || string.IsNullOrWhiteSpace(beiPackage.EditValue.ToString()))
                {
                    SystemMessage.ShowModalInfoMessage("Please select package first!", "ERROR");
                    return;
                }

                if (beiGuests.EditValue == null || string.IsNullOrWhiteSpace(beiGuests.EditValue.ToString()))
                {
                    SystemMessage.ShowModalInfoMessage("Please select guest first!", "ERROR");
                    return;
                }
                List<PackageAuditVM> SavedPackageConsumptionList = (List<PackageAuditVM>)gcConsumedPackages.DataSource;

                if (SavedPackageConsumptionList != null && SavedPackageConsumptionList.Count>0)
                {

                    PackageAuditVM RegistrationConsuption = SavedPackageConsumptionList.FirstOrDefault(x => x.RegNum == beiGuests.EditValue.ToString());
                    if (RegistrationConsuption != null)
                    {
                        SystemMessage.ShowModalInfoMessage("The guest Already used Package !", "ERROR");
                        return;
                    }
                }



                Progress_Reporter.Show_Progress("Saving Package Consumption Voucher", "Please Wait.......");
                var pkg = _pkgHeaderList.FirstOrDefault(p => p.Id == Convert.ToUInt32(beiPackage.EditValue));
                SavePackageConsumptionVoucher(Convert.ToInt32(beiGuests.EditValue), Convert.ToInt32( beiPackage.EditValue), pkg == null ? "" : pkg.Description);

                Progress_Reporter.Close_Progress();

            }
            catch (Exception ex)
            {
                Progress_Reporter.Close_Progress();
                SystemMessage.ShowModalInfoMessage("Error in saving package audit! Detail:: " + ex.Message, "ERROR");
            }
        }

        private void bbiPrint_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                ReportGenerator rg = new ReportGenerator();
                rg.GenerateGridReport(gcConsumedPackages, "Package Audit", CurrentDate.ToShortDateString());
            }
            catch (Exception ex)
            {
                SystemMessage.ShowModalInfoMessage("Error in printing package audit! Detail: " + ex.Message, "ERROR");
            }
        }

        private void gvConsumedPackages_RowStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs e)
        {


        }

        private void gvConsumedPackages_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            GridView view = sender as GridView;
            if (e.Column.Caption == "SN")
            {
                PackageAuditVM dto = view.GetRow(e.RowHandle) as PackageAuditVM;
                if (dto != null)
                    dto.SN = e.RowHandle + 1;
                e.DisplayText = (e.RowHandle + 1).ToString();
            }

        }



        void ripoSlukPackages_EditValueChanged(object sender, EventArgs e)
        {
            DevExpress.XtraEditors.SearchLookUpEdit view = sender as DevExpress.XtraEditors.SearchLookUpEdit;
            if (view.EditValue != null)
            {
                PopulatePackageAuditList(Convert.ToInt32(view.EditValue));
            }
        }

        void repoSlukGuest_EditValueChanged(object sender, EventArgs e)
        {
            
        }

        #endregion

        public int? SelectedHotelcode { get; set; }
        private void beiHotel_EditValueChanged(object sender, EventArgs e)
        {
            SelectedHotelcode = beiHotel.EditValue == null ? null :Convert.ToInt32(beiHotel.EditValue);
            if (!InitializeData())
            {
                rcPackageAudit.Enabled = false;
            }
        }


        


    }
}
