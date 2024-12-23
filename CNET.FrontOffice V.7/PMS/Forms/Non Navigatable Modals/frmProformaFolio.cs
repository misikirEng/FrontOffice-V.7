
using CNET.ERP.Client.Common.UI;
using CNET.FrontOffice_V._7;
using ProcessManager;
using CNET.FrontOffice_V._7.PMS.Common_Classes;
using CNET_V7_Domain.Domain.CommonSchema;
using CNET_V7_Domain.Domain.PmsSchema;
using CNET_V7_Domain.Domain.SettingSchema;
using CNET_V7_Domain.Domain.ViewSchema;
using CNET_V7_Domain.Misc.PmsDTO;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DocumentPrint;
using DocumentPrint.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CNET_V7_Domain.Domain.ConsigneeSchema;

namespace CNET.FrontOffice_V._7.Forms.Non_Navigatable_Modals
{
    public partial class frmProformaFolio : UILogicBase
    {
        public RegistrationListVMDTO RegExt { get; set; }

        /************ CONSTRUCTOR ****************/
        public frmProformaFolio()
        {
            InitializeComponent();

            InitializeUI();
        }

        #region Helper Methods

        private void InitializeUI()
        {

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

                teCompany.Text = RegExt.Company;
                teCustomer.Text = RegExt.Guest;
                teRegistration.Text = RegExt.Registration;
                teArrivalDate.Text = RegExt.Arrival.ToString("D");
                teDepartureDate.Text = RegExt.Departure.ToString("D");

                // Progress_Reporter.Show_Progress("Populating Proforma Folio", "Please Wait...");

                // getting other consignee's tin number
                if (RegExt.CompanyId != null)
                {
                    ConsigneeDTO Company = LocalBuffer.LocalBuffer.ConsigneeViewBufferList.FirstOrDefault(x => x.Id == RegExt.CompanyId);
                    if (Company != null)
                    {
                        teTIN.Text = "  " + Company.Tin;
                    }
                }


                PopulateProformaFolio();

                ////CNETInfoReporter.Hide();


                return true;
            }
            catch (Exception ex)
            {
                ////CNETInfoReporter.Hide();
                SystemMessage.ShowModalInfoMessage("Error in populating proforma folio. Detail:: " + ex.Message, "ERROR");
                return false;
            }
        }

        private void PopulateProformaFolio()
        {

            List<ProformaVM> proformaDtoList = new List<ProformaVM>();

            List<RegistrationDetailDTO> _registrationDetails = UIProcessManager.GetRegistrationDetailByvoucher(RegExt.Id);
            if (_registrationDetails == null || _registrationDetails.Count == 0)
            {
                SystemMessage.ShowModalInfoMessage("Unable to get registration detail.", "ERROR");
                return;
            }

            List<RateCodeDetailDTO> rateCodeLIst = UIProcessManager.SelectAllRateCodeDetail();

            List<RateSummaryVM> summaryList = new List<RateSummaryVM>();
            TaxDTO tax = UIProcessManager.GetTaxById(CNETConstantes.VAT);
            decimal taxRate = tax == null ? 15 : (decimal)tax.Amount;
            decimal serviceCharge = 0;
            int rowCount = 0;
            //Service Charge
            var vfdList = UIProcessManager.SelectAllValueFactorDefinition();
            List<int> vfdCodeList = vfdList.Where(v => v.Type == CNETConstantes.ADDTIONAL_CHARGE).Select(v => v.Id).ToList();
            foreach (RegistrationDetailDTO reg in _registrationDetails)
            {
                ProformaVM dto = new ProformaVM();
                dto.Date = reg.Date.Value;
                dto.NoOfPerson = reg.Adult.Value + reg.Child.Value;
                RateCodeDetailDTO rateCodeDetail = rateCodeLIst.FirstOrDefault(r => r.Id == reg.RateCode);
                if (rateCodeDetail != null)
                {
                    RateCodeHeaderDTO rateCode = UIProcessManager.GetRateCodeHeaderById(rateCodeDetail.RateCodeHeader);
                    if (rateCode != null)
                    {
                        //Getting Service Charge
                        var vfList = UIProcessManager.GetValueFactorByreference(rateCode.Article);
                        if (vfList != null && vfdCodeList != null)
                        {
                            ValueFactorDTO vf = vfList.FirstOrDefault(v => vfdCodeList.Contains(v.ValueFactorDefinition.Value));
                            if (vf != null)
                            {
                                ValueFactorDefinitionDTO vfd = UIProcessManager.GetValueFactorDefinitionById(vf.ValueFactorDefinition.Value);
                                if (vfd != null) serviceCharge = vfd.Value;
                            }
                        }
                        dto.RateDesc = rateCode.Description;
                        dto.RateCode = rateCode.Id;
                    }
                }
                if (reg.RateAmount != null)
                {

                    decimal factor = (((1 + taxRate / 100)) * ((decimal)serviceCharge / 100)) + (1 + taxRate / 100);
                    decimal rateAmt = Math.Round(reg.RateAmount.Value / factor, 2);
                    List<PackagesToPostDTO> pckList = UIProcessManager.GetPackagesToPostByRegistrationDetail(reg.Id).ToList();
                    decimal pckAmount = 0;
                    decimal quantity = 0;
                    string allPackages = "";
                    if (pckList != null)
                    {
                        foreach (PackagesToPostDTO pk in pckList)
                        {
                            PackageHeaderDTO pkHeader = UIProcessManager.GetPackageHeaderById(pk.PackageHeader);
                            if (pkHeader != null)
                            {
                                allPackages = allPackages + " " + pkHeader.Description + ",";
                                switch (pkHeader.CalculationRule)
                                {
                                    case CNETConstantes.Per_Person:
                                        quantity = Convert.ToDecimal(reg.Adult + reg.Child);
                                        break;
                                    case CNETConstantes.Flat_Rate:
                                        quantity = 1;
                                        break;
                                    case CNETConstantes.Per_Adult:
                                        quantity = Convert.ToDecimal(reg.Adult);
                                        break;
                                    case CNETConstantes.Per_Child:
                                        quantity = Convert.ToDecimal(reg.Child);
                                        break;
                                    case CNETConstantes.Per_Room:
                                        //To be implemented
                                        quantity = 1;
                                        break;
                                    default:
                                        quantity = 1;
                                        break;
                                }
                            }

                            pckAmount += pk.Amount * quantity;
                        }
                    }
                    dto.PackageDesc = allPackages;
                    pckAmount = Math.Round(pckAmount / factor, 2);
                    dto.RoomCharge = Math.Round(rateAmt - pckAmount, 2);
                    dto.Package = pckAmount;
                    dto.Subtotal = Math.Round(rateAmt, 2);
                }
                dto.ServiceCharge = (dto.Subtotal * (decimal)serviceCharge) / 100;
                dto.VAT = Math.Round((taxRate * (dto.Subtotal + dto.ServiceCharge)) / 100, 2);
                dto.Amount = Math.Round(dto.Subtotal + dto.VAT + dto.ServiceCharge, 2);
                rowCount++;
                dto.SN = rowCount;
                proformaDtoList.Add(dto);
            }
            gcProforma.DataSource = proformaDtoList;
            gvProforma.RefreshData();
            gvProforma.BestFitColumns();

            //Adjust Grid For Room Charges Grid
            gvProforma.BeginUpdate();
            GridViewInfo info = gvProforma.GetViewInfo() as GridViewInfo;
            int height = 28;
            GridRowInfo rInfo = info.RowsInfo.FindRow(0);

            if (rInfo != null)
            {
                //height = rInfo.Bounds.Height;
                height = (height * gvProforma.RowCount) + (2 * height);
                if (height < 65) height = 65;
                lc_proforma.ControlMinSize = new Size(gcProforma.Width, height);

                gvProforma.LayoutChanged();

            }

            gvProforma.EndUpdate();

            List<ProformaVM> dtoList = gcProforma.DataSource as List<ProformaVM>;
            if (dtoList == null || dtoList.Count == 0)
            {
                return;
            }

            decimal grandTotal = Math.Round(dtoList.Sum(d => d.Amount), 2);
            decimal serCharge = Math.Round(dtoList.Sum(d => d.ServiceCharge), 2);
            decimal subtotal = Math.Round(dtoList.Sum(d => d.Subtotal), 2);
            decimal vat = Math.Round(dtoList.Sum(d => d.VAT), 2);

            teGrandTotal.Text = grandTotal.ToString();
            teServiceCharge.Text = serCharge.ToString();
            teSubtotal.Text = subtotal.ToString();
            teVat.Text = vat.ToString();

        }

        #endregion

        #region Event Handlers

        private void bbiPrint_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {


                ReportGenerator reportGenerator = new ReportGenerator();
                LedgerObjects lObject = new LedgerObjects()
                {
                    ArrivalDate = teArrivalDate.Text,
                    CompanyName = teCompany.Text,
                    CustomerName = teCustomer.Text,
                    DepartureDate = teDepartureDate.Text,
                    TINNo = teTIN.Text,
                    Discount = "",
                    FsNo = "",
                    GrandTotal = teGrandTotal.Text,
                    Plan = "",
                    Refund = "",
                    RegistrationNumber = teRegistration.Text,
                    RemainingBalance = "",
                    ServiceCharge = teServiceCharge.Text,
                    SubTotal = teSubtotal.Text,
                    Vat = teVat.Text,
                    ConsigneeId = RegExt.GuestId,
                    SetWaterMark = false,
                    HeaderText = "Proforma Folio",
                    User = LocalBuffer.LocalBuffer.CurrentLoggedInUser.UserName
                };
                if (RegExt.CompanyId != null)
                {
                    ConsigneeDTO consigneeDTO = LocalBuffer.LocalBuffer.AllCustomerConsigneeViewlist.FirstOrDefault(x => x.Id == RegExt.CompanyId.Value);
                    if (consigneeDTO != null)
                        lObject.CompanyTin = consigneeDTO.Tin;
                }
                reportGenerator.GenerateGuestLedgerRoomCharge(gcProforma, lObject);
            }
            catch (Exception ex)
            {
                SystemMessage.ShowModalInfoMessage("Error in printing proforma folio. Detail:: " + ex.Message, "ERROR");
            }
        }

        private void bbiRefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                // Progress_Reporter.Show_Progress("Populating Proforma Folio", "Please Wait...");
                PopulateProformaFolio();

                ////CNETInfoReporter.Hide();
            }
            catch (Exception ex)
            {
                ////CNETInfoReporter.Hide();
                SystemMessage.ShowModalInfoMessage("Error in populating proforma folio. Detail:: " + ex.Message, "ERROR");
            }
        }

        private void bbiClose_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Close();
        }

        private void frmProformaFolio_Load(object sender, EventArgs e)
        {
            if (!InitializeData())
            {
                this.Close();
            }
        }


        #endregion

        private void gvProforma_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            //if (e.Column.Caption == "SN")
            //{
            //    var dto = gvProforma.GetRow(e.RowHandle) as ProformaDTO;
            //    if (dto != null) dto.SN = e.RowHandle + 1;
            //    e.DisplayText = (e.RowHandle + 1).ToString();
            //}
        }





    }
}
