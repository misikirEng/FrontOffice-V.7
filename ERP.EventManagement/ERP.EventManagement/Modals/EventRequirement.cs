using CNET_V7_Domain;
using CNET_V7_Domain.Domain.ArticleSchema;
using CNET_V7_Domain.Domain.CommonSchema;
using CNET_V7_Domain.Domain.ConsigneeSchema;
using CNET_V7_Domain.Domain.SettingSchema;
using CNET_V7_Domain.Domain.Transaction;
using CNET_V7_Domain.Domain.TransactionSchema;
using CNET_V7_Domain.Domain.ViewSchema;
using CNET_V7_Domain.Misc;
using CNET_V7_Domain.Misc.CommonTypes;
using CNET_V7_Domain.Misc.PmsDTO;
using DevExpress.CodeParser;
using DevExpress.Diagram.Core.Shapes;
using DevExpress.Printing.Core.PdfExport.Metafile;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using ERP.EventManagement.DTO;
using ProcessManager;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ERP.EventManagement.Modals
{
    public partial class EventRequirement : Form
    {
        #region Declaration 

        DateTime CurrentDate = UIProcessManager.GetServiceTime().Value;
        string voucherID { get; set; }

        bool IsUpdate = false;
        public bool MadeChange { get; set; }
        ConfigurationDTO PriceFlexible { get; set; }
        public List<TaxTransactionDTO> TaxTransaction { get; set; }

        VoucherBuffer EventRequestVoucher { get; set; }

        VoucherFinalDTO VoucherFinal = new VoucherFinalDTO();
        List<LineItemBuffer> LineItemList { get; set; }
        public PriceMethod SelectedPricevalue { get; set; }

        List<LineItemDetails> lineItemDetails = new List<LineItemDetails>();


        public ActivityDefinitionDTO EditActDef { get; set; }

        public ActivityDefinitionDTO PreparedActDef { get; set; }

        public int? Customerid { get; set; }
        List<LineItemDisplayDTO> LineItemDisplaydata { get; set; }
        public class LineItemDisplayDTO
        {
            public int ArticleId { get; set; }
            public string ArticleCode { get; set; }
            public string ArticleName { get; set; }
            public decimal Quantity { get; set; }
            public decimal UnitPrice { get; set; }
            public decimal? AddCharge { get; set; }
            public decimal? TaxAmount { get; set; }
            public decimal TotalPrice { get; set; }
        }
        int EventId { get; set; }
        int EventDetailId { get; set; }
        int EventRequirementId { get; set; }
        #endregion

        #region Constractor
        public EventRequirement(int Eventid, int eventdetailid)
        {

            InitializeComponent();
            EventVoucherSetting.GetCurrentVoucherSetting(CNETConstantes.EVENT_REQUIREMENT_VOUCHER);
            EventId = Eventid;
            EventDetailId = eventdetailid;
            GetEventRequirementActivity();
            // GetArticleByGSLRequirement();
            sleServices.Properties.DataSource = EventMgtForm.AllArticleList;
            deIssuedDate.EditValue = CurrentDate;
            GetAndPopulateData();
            GenerateId();
            IsUpdate = false;
            MadeChange = false;
            this.Text = "Create Event Requirement";
            txtRemark.Text = "Event Requirement";

            EventRequestVoucher = new VoucherBuffer();
            EventRequestVoucher.Voucher = new VoucherDTO
            {
                Code = txtVoucherNo.Text,
                Type = CNETConstantes.VOUCHER_COMPONENET,
                Definition = CNETConstantes.EVENT_REQUIREMENT_VOUCHER,
                IssuedDate = CurrentDate,
                IsIssued = true,
                Year = CurrentDate.Year,
                Month = CurrentDate.Month,
                Day = CurrentDate.Day,
                GrandTotal = 0,
                Period = null,
                LastActivity = null,
                Remark = null
            };
            GetRoundSetting();
        }
        public int QuantityRoundDigit = 2;
        public int UnitPriceRoundDigit = 2;
        public int TotalAmountRoundDigit = 2;
        public void GetRoundSetting()
        {
            var configurationList = LocalBuffer.LocalBuffer.ConfigurationBufferList.Where(x=> x.Reference == CNETConstantes.EVENT_REQUIREMENT_VOUCHER.ToString());
            foreach (ConfigurationDTO config in configurationList)
            {
                switch (config.Attribute.ToLower())
                {
                    case "round digit quantity":
                        QuantityRoundDigit = Convert.ToInt32(config.CurrentValue);
                        break;
                    case "round digit unit price":
                        UnitPriceRoundDigit = Convert.ToInt32(config.CurrentValue);
                        break;
                    case "round digit total":
                        TotalAmountRoundDigit = Convert.ToInt32(config.CurrentValue);
                        break;
                }
            }
        }
        public EventRequirement(int Eventid, int eventdetailid, int eventrequirementid)
        {
            InitializeComponent();
            EventVoucherSetting.GetCurrentVoucherSetting(CNETConstantes.EVENT_REQUIREMENT_VOUCHER);
            GetEventRequirementActivity();
            //GetArticleByGSLRequirement();
            EventId = Eventid;
            EventDetailId = eventdetailid;
            EventRequirementId = eventrequirementid;
            sleServices.Properties.DataSource = EventMgtForm.AllArticleList;
            var data = UIProcessManager.GetVoucherBufferById(eventrequirementid);

            if (!data.Success)
            {
                XtraMessageBox.Show("Fail to get Event Requirement voucher data !!");
                return;
            }
            EventRequestVoucher = data.Data;
            PopulateUpdateData(EventRequestVoucher);
            GetAndPopulateData();
            IsUpdate = true;
            this.Text = "Update Event Requirement";
            MadeChange = false;

        }

        public void GetEventRequirementActivity()
        {
            var data = UIProcessManager.GetActivityDefinitionByreference(CNETConstantes.EVENT_REQUIREMENT_VOUCHER);

            if (data != null && data.Count > 0)
            {
                PreparedActDef = data.FirstOrDefault(x => x.Description == CNETConstantes.LU_ACTIVITY_DEFINATION_PREPARED);

                EditActDef = data.FirstOrDefault(x => x.Description == CNETConstantes.LU_ACTIVITY_DEFINATION_Edit);
            }
        }

        #endregion

        #region Public Methods 
        public void PopulateUpdateData(VoucherBuffer SelectedReservation)
        {
            EventRequestVoucher = SelectedReservation;
            sleCustomer.EditValue = SelectedReservation.Voucher.Consignee1;
            deIssuedDate.EditValue = SelectedReservation.Voucher.IssuedDate;
            CurrentDate = SelectedReservation.Voucher.IssuedDate;
            txtVoucherNo.Text = SelectedReservation.Voucher.Code;
            txtRemark.Text = SelectedReservation.Voucher.Note;
            LineItemList = SelectedReservation.LineItemsBuffer.ToList();
            AddFullItemItem(LineItemList);
        }
        public void GetAndPopulateData()
        {

            PriceFlexible = LocalBuffer.LocalBuffer.ConfigurationBufferList.FirstOrDefault(x => x.Reference == CNETConstantes.EVENT_REQUIREMENT_VOUCHER.ToString() && x.Attribute == "Value Rule");
            if (PriceFlexible != null)
            {
                if (PriceFlexible.CurrentValue.ToLower() == CNETConstantes.VALUE_RULE_ALTERNATES.ToLower())
                {
                    cmbPrice.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
                    cmbPrice.Properties.Enabled = true;
                    cmbPrice.Properties.ReadOnly = false;
                }
                else if (PriceFlexible.CurrentValue.ToLower() == CNETConstantes.VALUE_RULE_FIXED.ToLower())
                {
                    cmbPrice.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
                    cmbPrice.Properties.Enabled = false;
                    cmbPrice.Properties.ReadOnly = true;
                }
                else if (PriceFlexible.CurrentValue.ToLower() == CNETConstantes.VALUE_RULE_FLEXIBLE.ToLower())
                {
                    cmbPrice.Properties.TextEditStyle = TextEditStyles.Standard;
                    cmbPrice.Properties.ReadOnly = false;
                    cmbPrice.Properties.Enabled = true;
                }
            }
            else
            {
                cmbPrice.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
                cmbPrice.Properties.Enabled = true;
                cmbPrice.Properties.ReadOnly = false;
            }

            cmbPrice.Text = String.Format("{0:F2}", "0.00");
            cmbPrice.Properties.DataSource = null;


            List<EventConsgineeDTO> consigneeDTOList = LocalBuffer.LocalBuffer.ConsigneeViewBufferList.Select(c => new EventConsgineeDTO()
            {
                id = c.Id,
                code = c.Code,
                gslType = c.GslType,
                IdentficationType = "TIN Number",
                idNumber = c.Tin,
                name = c.FirstName + " " + c.SecondName + " " + c.ThirdName
            }).ToList();

            sleCustomer.Properties.DataSource = consigneeDTOList;
            sleCustomer.Properties.DisplayMember = "name";
            sleCustomer.Properties.ValueMember = "id";

            sleServices.Properties.DataSource = EventMgtForm.AllArticleList;
            sleServices.Properties.DisplayMember = "Name";
            sleServices.Properties.ValueMember = "Id";

        }
        //List<VwArticleViewDTO> ArticleList { get; set; }
        //public void GetArticleByGSLRequirement()
        //{
        //    List<VwRequiredGslDTO> requiredGslDetail = UIProcessManager.Get_VwRequiredGslDTO_By_VoucherDefn_Type(CNETConstantes.EVENT_REQUIREMENT_VOUCHER, CNETConstantes.LK_Required_GSL_Article);
        //    if (requiredGslDetail != null && requiredGslDetail.Count > 0)
        //    {
        //        ArticleList = new List<VwArticleViewDTO>();
        //        foreach (VwRequiredGslDTO req in requiredGslDetail)
        //        {
        //            var ar = UIProcessManager.GetArticleViewByGslType(req.GslType);
        //            if (ar != null && ar.Count > 0)
        //                ArticleList.AddRange(ar);
        //        }
        //    }
        //    else
        //        ArticleList = new List<VwArticleViewDTO>();
        //}

        public void GenerateId(int type = 0)
        {

            var voucherID = UIProcessManager.IdGenerater("Voucher", CNETConstantes.EVENT_REQUIREMENT_VOUCHER, type, LocalBuffer.LocalBuffer.CurrentDeviceConsigneeUnit.Value, false, LocalBuffer.LocalBuffer.CurrentDevice.Id);



            if (!string.IsNullOrEmpty(voucherID))
            {
                txtVoucherNo.Text = voucherID;
            }
            else
            {
                XtraMessageBox.Show("There is a problem on id setting Event Requiremet! ", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        public void ClearControl()
        {
            gcServices.DataSource = null;
            sleCustomer.EditValue = null;
            txtRemark.Text = "";
            lineItemDetails = new List<LineItemDetails>();
            VoucherFinal = new VoucherFinalDTO();
            LineItemList = new List<LineItemBuffer>();


            EventRequestVoucher = new VoucherBuffer();
            EventRequestVoucher.Voucher = new VoucherDTO
            {
                Code = txtVoucherNo.Text,
                Type = CNETConstantes.VOUCHER_COMPONENET,
                Definition = CNETConstantes.EVENT_REQUIREMENT_VOUCHER,
                IssuedDate = CurrentDate,
                IsIssued = true,
                Year = CurrentDate.Year,
                Month = CurrentDate.Month,
                Day = CurrentDate.Day,
                GrandTotal = 0,
                Period = null,
                LastActivity = null,
                Remark = null
            };
        }
        public bool ValidateControl()
        {
            if (lineItemDetails == null || lineItemDetails.Count == 0)
            {
                XtraMessageBox.Show("Please Select Lineitems !!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }
        List<PriceMethod> valueList { get; set; }
        private void SetArticlePrice(int articleId)
        {
            List<PriceMethod> valueList = new List<PriceMethod>();
            valueList = UIProcessManager.Get_Article_Values(articleId, CNETConstants.GOODS_RESERVATION_VOUCHER, null, LocalBuffer.LocalBuffer.CurrentDeviceConsigneeUnit.Value, 0, CurrentDate, LocalBuffer.LocalBuffer.CurrentDevice.Id, true);

            int count = 1;
            if (valueList.Count() > 0)
            {


                if (PriceFlexible.CurrentValue.ToLower() == CNETConstantes.VALUE_RULE_ALTERNATES.ToLower())
                {
                    cmbPrice.Properties.DataSource = valueList;
                    cmbPrice.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
                    cmbPrice.Properties.DisplayMember = "Price";
                    cmbPrice.Properties.ValueMember = "Price";
                    var valu = valueList.First();
                    cmbPrice.EditValue = valu.Price;
                    cmbPrice.Properties.ReadOnly = false;
                }
                else if (PriceFlexible.CurrentValue.ToLower() == CNETConstantes.VALUE_RULE_FIXED.ToLower())
                {
                    cmbPrice.Properties.DataSource = valueList;
                    cmbPrice.Properties.DisplayMember = "Price";
                    cmbPrice.Properties.ValueMember = "Price";
                    var valu = valueList.First();
                    cmbPrice.EditValue = valu.Price;
                    cmbPrice.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
                    cmbPrice.Properties.ReadOnly = true;
                }
                else if (PriceFlexible.CurrentValue.ToLower() == CNETConstantes.VALUE_RULE_FLEXIBLE.ToLower())
                {

                    cmbPrice.Properties.TextEditStyle = TextEditStyles.Standard;
                    var valu = valueList.First();
                    if (valu == null)
                    {
                        valu = valueList.FirstOrDefault();
                    }
                    if (SelectedPricevalue != null && SelectedPricevalue.Price != 0)
                    {
                        if (SelectedPricevalue.Price != valu.Price)
                        {
                            if (DialogResult.OK == XtraMessageBox.Show("Do you want to change the price to default", "CNET_ERP2016", MessageBoxButtons.OKCancel,
                                      MessageBoxIcon.Information))
                            {
                                cmbPrice.Properties.DataSource = valueList;
                                cmbPrice.Properties.DisplayMember = "Price";
                                cmbPrice.Properties.ValueMember = "Price";
                                cmbPrice.EditValue = valu.Price;
                            }
                            else
                            {
                                cmbPrice.Properties.DataSource = valueList;
                                cmbPrice.Properties.DisplayMember = "Price";
                                cmbPrice.Properties.ValueMember = "Price";
                                cmbPrice.EditValue = SelectedPricevalue.Price;
                            }

                        }
                        else
                        {
                            cmbPrice.Properties.DataSource = valueList;
                            cmbPrice.Properties.DisplayMember = "Price";
                            cmbPrice.Properties.ValueMember = "Price";
                            cmbPrice.EditValue = valu.Price;
                        }
                    }
                    else
                    {
                        cmbPrice.Properties.DataSource = valueList;
                        cmbPrice.Properties.DisplayMember = "Price";
                        cmbPrice.Properties.ValueMember = "Price";
                        cmbPrice.EditValue = valu.Price;
                    }
                    cmbPrice.Properties.ReadOnly = false;
                }
            }
            else
            {
                AddValue("0");
            }
            SelectedPricevalue = new PriceMethod();
        }
        public void AddItem()
        {
            string addtionalChargePram = null;
            string discountParam = null;

            if (sleCustomer.EditValue != null && !string.IsNullOrEmpty(sleCustomer.EditValue.ToString()))
            {
                EventRequestVoucher.Voucher.Consignee1 = Convert.ToInt32(sleCustomer.EditValue.ToString());
            }

            decimal UnitPrice = Math.Round(Convert.ToDecimal(cmbPrice.EditValue.ToString()), UnitPriceRoundDigit);
            decimal OrigionalUnitPrice = Math.Round(Convert.ToDecimal(cmbPrice.EditValue.ToString()), UnitPriceRoundDigit);
            decimal Quantity = Math.Round(Convert.ToDecimal(nudQuantity.Text), QuantityRoundDigit);

            VwArticleViewDTO Article = EventMgtForm.AllArticleList.FirstOrDefault(x => x.Id.ToString() == sleServices.EditValue.ToString());

            LineItemDTO line = new LineItemDTO()
            {
                Article = Convert.ToInt32(sleServices.EditValue.ToString()),
                UnitAmount = UnitPrice,
                Uom = Article.UomId,
                Quantity = Quantity
            };
            LineItemCalculatorDTO ll = new LineItemCalculatorDTO()
            {
                voucherDefintion = CNETConstants.GOODS_RESERVATION_VOUCHER,
                consignee = null,
                prferentialValueFactorDefn = null,
                currentLineItem = line,
                discountValueFactorCode = null,
                additionalChargeValueFactorCode = null,
                additionalChargeParam = null,
                discountParam = null,
                priceExtracted = true,
                isPercentDiscount = false,
                isPercentAdditionalCharge = false,
                scIncluded = false,
                discountChecked = true,
                additionalChargeChecked = true,
                flexibleTaxChecked = true,
                consigneeUnit = null

            };

            LineItemDetails lineItemObjCheck = new LineItemDetails();

            if (lineItemDetails != null && lineItemDetails.Count > 0)
            {
                lineItemObjCheck = lineItemDetails.FirstOrDefault(c => c.lineItems.Article == Article.Id);
            }
            else
            {
                lineItemDetails = new List<LineItemDetails>();
            }


            if (lineItemObjCheck != null && lineItemObjCheck.lineItems != null)
            {
                ll.currentLineItem.Quantity = nudQuantity.Value + lineItemObjCheck.lineItems.Quantity;
                lineItemDetails.Remove(lineItemObjCheck);
            }
            else
            {
                ll.currentLineItem.Quantity = nudQuantity.Value;
            }

            VwArticleViewDTO Art = EventMgtForm.AllArticleList.FirstOrDefault(x => x.Id.ToString() == sleServices.EditValue.ToString());

            LineItemDTO lineItem = new LineItemDTO();
            lineItem.Quantity = nudQuantity.Value;
            lineItem.Article = Convert.ToInt32(sleServices.EditValue);
            lineItem.UnitAmount = UnitPrice;
            lineItem.Tax = Article.DefaultTax;
            lineItem.ObjectState = null;
            lineItem.Note = txtLineItemNote.Text;

            LineItemDetailPMS liDetail = new NewLineItemCaculator().LineItemDetailCalculatorVoucher(EventRequestVoucher, lineItem, null, null, null, null, null, true, false, false, false);
            LineItemDetails liDetails = new LineItemDetails()
            {
                articleName = Art == null ? "Unknown Article" : Art.Name,
                lineItems = liDetail.lineItem,
                lineItemValueFactor = liDetail.lineItemValueFactor
            };

            lineItemDetails.Add(liDetails);

            VoucherFinal = new VoucherFinalCalculator().VoucherCalculation(EventRequestVoucher.Voucher, lineItemDetails);


            // List<LineItemBuffer> buff = lineItemDetails.Select(x => new LineItemBuffer { LineItem = x.lineItems, LineItemValueFactors = x.lineItemValueFactor }).ToList();




            LineItemDisplaydata = new List<LineItemDisplayDTO>();

            LineItemDisplaydata = lineItemDetails.Select(x => new LineItemDisplayDTO()
            {
                ArticleId = x.lineItems.Article,
                ArticleName = x.articleName,
                Quantity = x.lineItems.Quantity,
                UnitPrice = x.lineItems.UnitAmount,
                AddCharge = x.lineItems.AddCharge,
                TaxAmount = x.lineItems.TaxAmount,
                TotalPrice = (x.lineItems.TaxableAmount.HasValue ? x.lineItems.TaxableAmount.Value : 0) + (x.lineItems.TaxAmount.HasValue ? x.lineItems.TaxAmount.Value : 0)

            }).ToList();

            gcServices.DataSource = LineItemDisplaydata;
            gcServices.RefreshDataSource();
        }
        public void AddFullItemItem(List<LineItemBuffer> LineItemList)
        {

            foreach (LineItemBuffer line in LineItemList)
            {
                VwArticleViewDTO Art = EventMgtForm.AllArticleList.FirstOrDefault(x => x.Id == line.LineItem.Article);
                //LineItemDTO lineItem =line.LineItem;
                //lineItem.Quantity = line.LineItem.Quantity;
                //lineItem.Article = line.LineItem.Article;
                //lineItem.UnitAmount = line.LineItem.UnitAmount;
                //lineItem.Tax = line.LineItem.Tax;
                //lineItem.Note = line.LineItem.Note;
                //lineItem.ObjectState = null;

                LineItemDetailPMS liDetail = new NewLineItemCaculator().LineItemDetailCalculatorVoucher(EventRequestVoucher, line.LineItem, null, null, null, null, null, true, false, false, false);
                LineItemDetails liDetails = new LineItemDetails()
                {
                    articleName = Art == null ? "Unknown Article" : Art.Name,
                    lineItems = liDetail.lineItem,
                    lineItemValueFactor = liDetail.lineItemValueFactor
                };

                lineItemDetails.Add(liDetails);


            }

            VoucherFinal = new VoucherFinalCalculator().VoucherCalculation(EventRequestVoucher.Voucher, lineItemDetails);

            //List<LineItemBuffer> buff = lineItemDetails.Select(x => new LineItemBuffer { LineItem = x.lineItem, LineItemValueFactors = x.lineItemValueFactor }).ToList();


            //VoucherFinal = CNET.POS.LIbrary.Wrapper.Common.Voucher_Calculator(buff);
            //TaxTransaction = VoucherFinal.tax_Transaction;
            //EventRequestVoucher.Voucher.GrandTotal = VoucherFinal.voucher.GrandTotal;

            LineItemDisplaydata = new List<LineItemDisplayDTO>();

            LineItemDisplaydata = lineItemDetails.Select(x => new LineItemDisplayDTO()
            {
                ArticleId = x.lineItems.Article,
                ArticleName = x.articleName,
                Quantity = x.lineItems.Quantity,
                UnitPrice = x.lineItems.UnitAmount,
                AddCharge = x.lineItems.AddCharge,
                TaxAmount = x.lineItems.TaxAmount,
                TotalPrice = (x.lineItems.TaxableAmount.HasValue ? x.lineItems.TaxableAmount.Value : 0) + (x.lineItems.TaxAmount.HasValue ? x.lineItems.TaxAmount.Value : 0)
            }).ToList();


            gcServices.DataSource = LineItemDisplaydata;
            gcServices.RefreshDataSource();
        }
        private void AddValue(String newValue)
        {
            if (newValue != null && newValue.Trim() != "")
            {
                PriceMethod valuee = new PriceMethod();
                if (valueList == null)
                {
                    valueList = new List<PriceMethod>();
                }

                valuee.Price = Convert.ToDecimal(newValue);
                var valueViews = cmbPrice.Properties.DataSource as List<PriceMethod>;
                if (valueViews == null)
                    valueViews = new List<PriceMethod>();
                if (!(valueViews.Any(vv => vv.Price == valuee.Price)))
                {
                    valuee.ValueDescription = "User Defined";
                    valueList.Add(valuee);
                    cmbPrice.Properties.DataSource = valueList;
                    cmbPrice.Properties.DisplayMember = "Price";
                    cmbPrice.Properties.ValueMember = "Price";
                    cmbPrice.EditValue = valueList.LastOrDefault().Price;
                }

            }


        }
        #endregion

        #region Event Handler Methods
        private void btnNew_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ClearControl();
            GenerateId();
        }
        public bool SaveEvent()
        {
            if (ValidateControl())
            {

                CurrentDate = UIProcessManager.GetServiceTime().Value;

                ResponseModel<VoucherBuffer> Saved = null;
                EventRequestVoucher.Voucher.Code = txtVoucherNo.Text;
                EventRequestVoucher.Voucher.Note = txtRemark.Text;
                EventRequestVoucher.Voucher.OriginConsigneeUnit = LocalBuffer.LocalBuffer.CurrentDeviceConsigneeUnit;
                EventRequestVoucher.Voucher.LastUser = LocalBuffer.LocalBuffer.CurrentLoggedInUser.Id;
                if(sleCustomer.EditValue != null)
                EventRequestVoucher.Voucher.Consignee1 = Convert.ToInt32(sleCustomer.EditValue.ToString());


                EventRequestVoucher.LineItemsBuffer = lineItemDetails.Select(x => new LineItemBuffer { LineItem = x.lineItems, LineItemValueFactors = x.lineItemValueFactor }).ToList();
                EventRequestVoucher.TaxTransactions = VoucherFinal.taxTransactions;
                EventRequestVoucher.TransactionReferencesBuffer = new List<TransactionReferenceBuffer>()
                {   new TransactionReferenceBuffer()
                    {
                        TransactionReference = new TransactionReferenceDTO()
                        {
                        Referenced = EventId,
                        ReferencedVoucherDefn = CNETConstantes.EVENT_VOUCHER,
                        ReferencingVoucherDefn = CNETConstantes.EVENT_REQUIREMENT_VOUCHER,
                        Value = 0,

                        },
                    ClosedRelation = null,
                    ReferencedActivity = null,
                    ReferencedVoucherLastState= null
                    }
                };

                ActivityDTO Act = new ActivityDTO();
                Act.TimeStamp = CurrentDate;
                Act.ConsigneeUnit = LocalBuffer.LocalBuffer.CurrentDeviceConsigneeUnit;



                if (IsUpdate)
                    Act.ActivityDefinition = EditActDef.Id;
                else
                    Act.ActivityDefinition = PreparedActDef.Id;





                Act.Device = LocalBuffer.LocalBuffer.CurrentDevice.Id;
                Act.User = LocalBuffer.LocalBuffer.CurrentLoggedInUser.Id;
                Act.Year = CurrentDate.Year;
                Act.Month = CurrentDate.Month;
                Act.Day = CurrentDate.Day;
                Act.Platform = "1";
                Act.Pointer = CNETConstants.VOUCHER_COMPONENT;

                EventRequestVoucher.Activity = Act;

                EventRequestVoucher.TaxTransactions = VoucherFinal.taxTransactions;

                if (IsUpdate)
                {
                    EventRequestVoucher.Voucher.LastState = EditActDef.State.Value;
                    Saved = UIProcessManager.UpdateVoucherBuffer(EventRequestVoucher);

                }
                else
                {
                    EventRequestVoucher.Voucher.Code = txtVoucherNo.Text;
                    EventRequestVoucher.Voucher.LastState = PreparedActDef.State.Value;
                    Saved = UIProcessManager.CreateVoucherBuffer(EventRequestVoucher);
                    if (Saved == null)
                    {
                        XtraMessageBox.Show("Saving Voucher Failed !!" + Saved.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                    RelationDTO relation = new RelationDTO()
                    {
                        RelationType = CNETConstantes.VOUCHER_RELATION_TYPE_NES_REF,
                        ReferencedPointer = 0,
                        ReferencedObject = EventDetailId,
                        ReferringObject = Saved.Data.Voucher.Id,
                        ReferringPointer = Saved.Data.Voucher.Definition
                    };
                    UIProcessManager.CreateRelation(relation);

                }


                if (Saved != null)
                {
                    XtraMessageBox.Show("Voucher is Saved successfully", "CNETERP_V6", MessageBoxButtons.OK, MessageBoxIcon.Information);


                }
                else
                {
                    XtraMessageBox.Show("Saving Voucher Failed !!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            else
            {
                return false;
            }
            return true;
        }
        private void btnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (SaveEvent())
            {
                GenerateId(1);
                ClearControl();
                MadeChange = true;
            }
        }
        private void btnClose_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Close();
        }
        private void leResourceType_EditValueChanged(object sender, EventArgs e)
        {
            //leResource.EditValue = null;
            //if (leResourceType.EditValue != null && !string.IsNullOrEmpty(leResourceType.EditValue.ToString()))
            //{
            //    List<Space> SpaceList = LoginPage.Authentication.spaceBufferList.Where(x => x.Type == leResourceType.EditValue.ToString()).ToList();
            //    leResource.Properties.DataSource = SpaceList;
            //}
            //else
            //{
            //    leResource.Properties.DataSource = null;
            //}
        }
        private void sleCustomer_AddNewValue(object sender, DevExpress.XtraEditors.Controls.AddNewValueEventArgs e)
        {
            UIMaintainCustomer maintainAgent = new UIMaintainCustomer(CNETConstantes.CUSTOMER);
            maintainAgent.ShowDialog();
            if (UIMaintainCustomer.CreatedCustomer != null && UIMaintainCustomer.CreatedCustomer != null)
            {
                LocalBuffer.LocalBuffer.ConsigneeViewBufferList.Add(UIMaintainCustomer.CreatedCustomer);

                sleCustomer.Properties.DataSource = LocalBuffer.LocalBuffer.ConsigneeViewBufferList;
                e.NewValue = UIMaintainCustomer.CreatedCustomer.Id;
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (sleServices.EditValue == null || string.IsNullOrEmpty(sleServices.EditValue.ToString()))
            {
                XtraMessageBox.Show("Please select a Service First! ", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (cmbPrice.EditValue == null || string.IsNullOrEmpty(cmbPrice.EditValue.ToString()) || cmbPrice.EditValue.ToString() == "0")
            {
                XtraMessageBox.Show("Please Insert a correct Price! ", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (nudQuantity.Value < 0)
            {
                XtraMessageBox.Show("Please Insert a correct Quantity! ", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            AddItem();
            sleServices.EditValue = null;
            cmbPrice.Properties.DataSource = null;
            cmbPrice.EditValue = null;
            nudQuantity.Value = 1;
            txtLineItemNote.Text = "";
        }
        private void btnRemove_Click(object sender, EventArgs e)
        {

            LineItemDisplayDTO Selected = (LineItemDisplayDTO)gvServices.GetFocusedRow();
            if (Selected != null)
            {
                var data = lineItemDetails.FirstOrDefault(x => x.lineItems.Article == Selected.ArticleId);
                lineItemDetails.Remove(data);
                VoucherFinal = new VoucherFinalCalculator().VoucherCalculation(EventRequestVoucher.Voucher, lineItemDetails);
                LineItemDisplaydata = new List<LineItemDisplayDTO>();

                LineItemDisplaydata = lineItemDetails.Select(x => new LineItemDisplayDTO()
                {
                    ArticleId = x.lineItems.Article,
                    ArticleName = x.articleName,
                    Quantity = x.lineItems.Quantity,
                    UnitPrice = x.lineItems.UnitAmount,
                    AddCharge = x.lineItems.AddCharge,
                    TaxAmount = x.lineItems.TaxAmount,
                    TotalPrice = (x.lineItems.TaxableAmount.HasValue ? x.lineItems.TaxableAmount.Value : 0) + (x.lineItems.TaxAmount.HasValue ? x.lineItems.TaxAmount.Value : 0)
                }).ToList();
                gcServices.DataSource = LineItemDisplaydata;
                gcServices.RefreshDataSource();
            }
        }
        private void sleServices_EditValueChanged(object sender, EventArgs e)
        {
            if (sleServices.EditValue != null && !string.IsNullOrEmpty(sleServices.EditValue.ToString()))
            {
                int id = Convert.ToInt32(sleServices.EditValue.ToString());
                //List<MiniArticleViewDTO> ArticlePrice = MainSchedulePage.reservationPOSInit.ArticleList.FirstOrDefault(x => x.ID == id).DefaultValue;

                if (id > 0)
                {
                    SetArticlePrice(id);
                }
                else
                {
                    AddValue("0");
                }
            }



        }
        private void cmbPrice_ProcessNewValue(object sender, ProcessNewValueEventArgs e)
        {
            if (cmbPrice.Text != null)
            {
                string valuew = cmbPrice.Text.ToString();
                AddValue(valuew);
            }
        }
        #endregion
        private void frmNewSchedule_Load(object sender, EventArgs e)
        {
            if (Customerid != null)
                sleCustomer.EditValue = Customerid;
        }

    }
}
