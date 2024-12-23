
using CNET.ERP.Client.Common.UI;
using CNET.FrontOffice_V._7;
using ProcessManager;
using CNET.FrontOffice_V._7.PMS.Common_Classes;
using CNET_V7_Domain.Domain.CommonSchema;
using CNET_V7_Domain.Domain.PmsSchema;
using CNET_V7_Domain.Domain.SettingSchema;
using CNET_V7_Domain.Domain.TransactionSchema;
using CNET_V7_Domain.Misc.PmsDTO;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace CNET.FrontOffice_V._7.Forms.Non_Navigatable_Modals
{
    public partial class frmOtherFields : XtraForm // UILogicBase
    {
        VoucherDTO voucher;
        private List<RegistrationDetailDTO> _regDetailList;

        //PROPERTIES    
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

        public DateTime CurrentTime { get; set; }

        public RegistrationListVMDTO RegExt { get; set; }

        /*>>>>>>>>>>> CONSTRUCTOR <<<<<<<<<<<<<<<<<*/
        public frmOtherFields()
        {
            InitializeComponent();
            InitializeUI();
        }

        #region Helper Methods

        private void InitializeUI()
        {
            //Member Type
            cacMemberType.Properties.Columns.Add(new LookUpColumnInfo("Description", "Member Type"));
            cacMemberType.Properties.DisplayMember = "Description";
            cacMemberType.Properties.ValueMember = "Id";

            //Origion
            cacOrigion.Properties.Columns.Add(new LookUpColumnInfo("Description", "Origin"));
            cacOrigion.Properties.DisplayMember = "Description";
            cacOrigion.Properties.ValueMember = "Id";

            //Business Source
            cacSource.Properties.Columns.Add(new LookUpColumnInfo("Description", "Business Source"));
            cacSource.Properties.DisplayMember = "Description";
            cacSource.Properties.ValueMember = "Id";

            //Reservation Type
            cacResType.Properties.Columns.Add(new LookUpColumnInfo("Description", "Reservation Type"));
            cacResType.Properties.DisplayMember = "Description";
            cacResType.Properties.ValueMember = "Id";

            //Market
            cacMarket.Properties.Columns.Add(new LookUpColumnInfo("Description", "Market"));
            cacMarket.Properties.DisplayMember = "Description";
            cacMarket.Properties.ValueMember = "Id";

            //Specials
            cacSpecials.Properties.Columns.Add(new LookUpColumnInfo("Description", "Specials"));
            cacSpecials.Properties.DisplayMember = "Description";
            cacSpecials.Properties.ValueMember = "Id";

        }

        private bool InitializeData()
        {
            try
            {

                if (RegExt == null)
                {
                    SystemMessage.ShowModalInfoMessage("Please select a registration!", "ERROR");
                    return false;
                }

                // Progress_Reporter.Show_Progress("Initializing data", "Please Wait...");

                DateTime? currentTime = UIProcessManager.GetServiceTime();
                if (currentTime == null)
                {
                    ////CNETInfoReporter.Hide();
                    return false;
                }

                CurrentTime = currentTime.Value;


                //TO DO: Member Type
                List<LookupDTO> memberType = LocalBuffer.LocalBuffer.LookUpBufferList.Where(l => l.Type == CNETConstantes.MEMBER_TYPE && l.IsActive).ToList();
                cacMemberType.Properties.DataSource = memberType;

                //Origion
                List<LookupDTO> origionList = LocalBuffer.LocalBuffer.LookUpBufferList.Where(l => l.Type == CNETConstantes.LIST_OF_ORGIN && l.IsActive).ToList();
                cacOrigion.Properties.DataSource = origionList;

                //Business Source
                List<LookupDTO> busSoureceList = LocalBuffer.LocalBuffer.LookUpBufferList.Where(l => l.Type == CNETConstantes.BUSSINESS_SOURCE && l.IsActive).ToList();
                cacSource.Properties.DataSource = busSoureceList;

                //Reservation Type List
                List<SystemConstantDTO> resTypeList = LocalBuffer.LocalBuffer.SystemConstantDTOBufferList.Where(l => l.Category == CNETConstantes.RESERVATION_TYPE && l.IsActive).ToList();
                cacResType.Properties.DataSource = resTypeList;

                //Market
                List<LookupDTO> marketList = LocalBuffer.LocalBuffer.LookUpBufferList.Where(l => l.Type == CNETConstantes.MARKET && l.IsActive).ToList();
                cacMarket.Properties.DataSource = marketList;

                //specials
                List<LookupDTO> specialsList = LocalBuffer.LocalBuffer.LookUpBufferList.Where(l => l.Type == CNETConstantes.SPECIALS && l.IsActive).ToList();
                cacSpecials.Properties.DataSource = specialsList;

                VoucherDTO voucher = UIProcessManager.GetVoucherById(RegExt.Id);
                if (voucher != null)
                {
                    cacResType.EditValue = voucher.Type;
                    cacSpecials.EditValue = voucher.Extension3;
                    cacOrigion.EditValue = voucher.Extension2;
                    memoPurposeTravel.EditValue = voucher.Purpose;
                }

                _regDetailList = UIProcessManager.GetRegistrationDetailByvoucher(RegExt.Id);
                if (_regDetailList != null && _regDetailList.Count > 0)
                {
                    var regDetail = _regDetailList.LastOrDefault();
                    if (regDetail != null)
                    {
                        cacMarket.EditValue = regDetail.Market;
                        cacSource.EditValue = regDetail.Source;
                    }
                }

                ////CNETInfoReporter.Hide();
                return true;
            }
            catch (Exception ex)
            {
                ////CNETInfoReporter.Hide();
                XtraMessageBox.Show("Error in initializing form. Detail: " + ex.Message, "Other Fields", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        #endregion

        #region Event Handlers

        private void bbiSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                // Progress_Reporter.Show_Progress("Saving Other Fields...", "Please Wait...");

                VoucherDTO isRegExtSaved = null;
                RegistrationDetailDTO isRegDetailSaved = null;

                //save on registration extension
                if (voucher != null)
                {
                    voucher.Type = (cacResType.EditValue == null || string.IsNullOrEmpty(cacResType.EditValue.ToString())) ? CNETConstantes.TRANSACTIONTYPENORMALTXN : Convert.ToInt32(cacResType.EditValue);
                    voucher.Extension3 = (cacSpecials.EditValue == null || string.IsNullOrEmpty(cacSpecials.EditValue.ToString())) ? "" : cacSpecials.EditValue.ToString();
                    voucher.Extension2 = (cacOrigion.EditValue == null || string.IsNullOrEmpty(cacOrigion.EditValue.ToString())) ? "" : cacOrigion.EditValue.ToString();
                    voucher.Purpose = (memoPurposeTravel.EditValue == null || string.IsNullOrEmpty(memoPurposeTravel.EditValue.ToString())) ? null : Convert.ToInt32(memoPurposeTravel.EditValue);

                    isRegExtSaved = UIProcessManager.UpdateVoucher(voucher);
                }



                //save registration detail
                if (_regDetailList != null && _regDetailList.Count > 0)
                {
                    foreach (var regDetail in _regDetailList)
                    {
                        regDetail.Market = cacMarket.EditValue == null ? null : Convert.ToInt32(cacMarket.EditValue);
                        regDetail.Source = cacSource.EditValue == null ? null : Convert.ToInt32(cacSource.EditValue);
                        isRegDetailSaved = UIProcessManager.UpdateRegistrationDetail(regDetail);
                        if (isRegDetailSaved == null)
                            break;
                    }
                }

                if (isRegExtSaved == null || isRegDetailSaved == null)
                {
                    XtraMessageBox.Show("Some other fields are not saved!", "Other Fields", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                }
                else
                {
                    XtraMessageBox.Show("Other Fileds are successfully saved!", "Other Fields", MessageBoxButtons.OK, MessageBoxIcon.Information);

                }

                ////CNETInfoReporter.Hide();

            }
            catch (Exception ex)
            {
                ////CNETInfoReporter.Hide();
                XtraMessageBox.Show("Error in Saving Other Fields. Detail: " + ex.Message, "Other Fields", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        private void bbiClose_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Close();
        }

        private void frmOtherFields_Load(object sender, EventArgs e)
        {
            if (!InitializeData())
            {
                this.Close();
            }
        }

        #endregion


    }
}
