using CNET.POS.Common;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Docking;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Linq;
using CNET.Progress.Reporter;
using CNET.POS.DocumentBrowser.Models;
using CNET_V7_Domain.Domain.SettingSchema;
using DevExpress.Utils;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid;
using DevExpress.Data;
using CNET_V7_Domain;
using CNET_V7_Domain.Misc.CommonTypes;
using CNET.POS.Common.Models;
using DevExpress.Pdf.ContentGeneration;
using DevExpress.DataProcessing;
using System.Windows.Controls;
using DevExpress.DataAccess.DataFederation;
using DevExpress.Office;
using DevExpress.Printing.Core.PdfExport.Metafile;
using DevExpress.XtraCharts;
using DevExpress.XtraRichEdit.Import.Rtf;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;
using System.Xml.Linq;
using DevExpress.Pdf.Native.BouncyCastle.Ocsp;
using CNET_V7_Domain.Domain.ViewSchema;
using DevExpress.Charts.Native;
using CNET.FrontOffice_V._7;
using ProcessManager;
using System.IO;
using DevExpress.CodeParser;
using System.Windows.Shapes;
using Microsoft.Office.Interop.Excel;
using DocumentPrint;
//using Application = Microsoft.Office.Interop.Excel.Application;

namespace CNET.POS.DocumentBrowser
{
    public partial class UCVoucherDocument : System.Windows.Forms.UserControl
    {
        #region Declarations
        private int voucherDefnition;
        private DateTime? currentDate;
        private string dateSearch;
        public string issuedDate;
        public string issueDateEnd;
        private List<FieldFormat> field_Format_List;
        private string VoucherDefinitionName;
        private string documentBrowserType = "heavydocumentbrowser";
        private List<Right_view> listOfRightView;
        private Right_view rv;
        #endregion

        public event EventHandler<VoucherRoomPOSChargeClicked> RoomPOSChargeButtonClicked;

        #region Search Parameters
        int? From_Store, To_Store;
        bool? Is_Issued, Is_Isvoid;
        string Grand_Total, Add_Charge, Discount, Sub_Total, VAT, FS_No, MRC_No, Ext1, Ext2, Ext3, Ext4, Ext5, Ext6;
        #endregion

        public UCVoucherDocument()
        {
            InitializeComponent();
        }
        public UCVoucherDocument(int voucher_Defn)
        {
            Progress_Reporter.Show_Progress("Initializing Components...", "Please Wait...");
            InitializeComponent();
            this.voucherDefnition = voucher_Defn;
            Initialize_Search_By_Date_Criteria();
            //      dpVoucherDetail.Visibility = DockVisibility.AutoHide;
            //dpLineItemDetail.Visibility = DockVisibility.AutoHide;
            var VoucherDefinitionN = LocalBuffer.LocalBuffer.SystemConstantDTOBufferList.FirstOrDefault(x => x.Id == voucherDefnition);
            if (VoucherDefinitionN != null)
            {
                VoucherDefinitionName = VoucherDefinitionN.Description;
                if (VoucherDefinitionName.ToLower().Contains("voucher"))
                {
                    dpVoucherDetail.Text = VoucherDefinitionName + " Detail";
                }
                else
                {
                    VoucherDefinitionName = VoucherDefinitionName + " Voucher";
                    dpVoucherDetail.Text = VoucherDefinitionName + " Detail";
                }
            }
            else
            {
                Progress_Reporter.Close_Progress();
                XtraMessageBox.Show(String.Format("Sorry! Voucher Is Not Defined For Voucher Definition ={0}.", voucherDefnition), "Voucher Definition Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                ribbonControl1.Enabled = false;
                return;
            }

            Progress_Reporter.Show_Progress("Loading Field Formats...", "Please Wait...");
            field_Format_List = Get_Voucher_Defn_Field_Format();
            if (field_Format_List == null || field_Format_List.Count == 0)
            {
                Progress_Reporter.Close_Progress();
                XtraMessageBox.Show(String.Format("Field Format Not Available For {0}.", VoucherDefinitionName), "Field Format Not Found", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                ribbonControl1.Enabled = false;
            }

            Progress_Reporter.Show_Progress("Loading Default Document Configuration...", "Please Wait...");
            var value = LocalBuffer.LocalBuffer.ConfigurationBufferList.FirstOrDefault(uip => uip.Reference == voucherDefnition.ToString() && uip.Attribute == "Default Document");
            if (value != null)
            {
                if (value.CurrentValue.ToLower() == "monthly")
                {
                    bbDateSearchBy.EditValue = "Monthly";
                }
                else if (value.CurrentValue.ToLower() == "weekly")
                {
                    bbDateSearchBy.EditValue = "Weekly";
                }
                else if (value.CurrentValue.ToLower() == "daily")
                {
                    bbDateSearchBy.EditValue = "Daily";
                }
                else if (value.CurrentValue.ToLower() == "none")
                {
                    bbDateSearchBy.EditValue = "None";
                }
                else if (value.CurrentValue.ToLower() == "annually")
                {
                    bbDateSearchBy.EditValue = "Annually";
                }
                else if (value.CurrentValue.ToLower() == "showall")
                {
                    bbDateSearchBy.EditValue = "Show All";
                }
            }
            else
            {
                Progress_Reporter.Close_Progress();
                XtraMessageBox.Show("No Date Criteria Available To Browse Document.\nPlease Save Default Document Property Setting First.", "Setting Not Set", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ribbonControl1.Enabled = false;
                field_Format_List.Clear();
                return;
            }

            Progress_Reporter.Show_Progress("Loading Show Find Panel Configuration...", "Please Wait...");
            value = LocalBuffer.LocalBuffer.ConfigurationBufferList.FirstOrDefault(uip => uip.Reference == voucherDefnition.ToString() && uip.Attribute == "Show Find Panel");
            if (value != null && value.CurrentValue.ToLower() == "true")
            {
                gvVoucher.ShowFindPanel();
                gvVoucher.OptionsFind.FindMode = FindMode.Always;
                gvVoucher.OptionsFind.ShowCloseButton = false;
            }

            Progress_Reporter.Show_Progress("Loading Show Group Panel Configuration...", "Please Wait...");
            value = LocalBuffer.LocalBuffer.ConfigurationBufferList.FirstOrDefault(uip => uip.Reference == voucherDefnition.ToString() && uip.Attribute == "Show Group Panel");
            if (value != null)
            {
                if (value.CurrentValue.ToLower() == "true")
                    gvVoucher.OptionsView.ShowGroupPanel = true;
                else if (value.CurrentValue.ToLower() == "false")
                    gvVoucher.OptionsView.ShowGroupPanel = false;
            }

            value = LocalBuffer.LocalBuffer.ConfigurationBufferList.FirstOrDefault(p => p.Attribute == "Document Browser Type" && p.Reference == voucherDefnition.ToString());
            if (value != null && !string.IsNullOrEmpty(value.CurrentValue))
            {
                documentBrowserType = value.CurrentValue;
            }
            else
            {
                documentBrowserType = "lightdocumentbrowser";
            }
            Progress_Reporter.Show_Progress("Applying Field Formats...", "Please Wait...");
            if (!Create_Grid_Columns())
            {
                Progress_Reporter.Close_Progress();
                XtraMessageBox.Show("Failed To Apply Field Format.\nPlease Check Your Field Format Settings.", "Field Format Error ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ribbonControl1.Enabled = false;
                field_Format_List.Clear();
                return;
            }

            Progress_Reporter.Show_Progress("Initalize Search By...", "Please Wait...");
            Initialize_SearchBy();

            Progress_Reporter.Show_Progress("Getting Consignees...", "Please Wait...");
            cmbConsignee.DataSource = Get_All_Consignees();

            Progress_Reporter.Show_Progress("Initalizing Object States...", "Please Wait...");
            chkCmbObjectState.DataSource = LocalBuffer.LocalBuffer.SystemConstantDTOBufferList.Where(x => x.Type == "ObjectState Definition" && x.Category == "Transaction" && x.IsActive).ToList();

            Progress_Reporter.Show_Progress("Initalizing Period...", "Please Wait...");
            Initalize_Period();

            Progress_Reporter.Show_Progress("Initalizing Consignee Units...", "Please Wait...");
            cmbConsigneeUnit.DataSource = LocalBuffer.LocalBuffer.HotelBranchBufferList;

            Progress_Reporter.Show_Progress("Initalizing Devices...", "Please Wait...");
            cmbDevice.DataSource = UIProcessManager.SelectAllDevice();// Buffers.Device_Buffer;

            Progress_Reporter.Show_Progress("Initalizing Users...", "Please Wait...");
            cmbUser.DataSource = UIProcessManager.SelectAllUser();// Buffers.User_Buffer;

            Progress_Reporter.Close_Progress();


        }

        #region Methods

        private void Initialize_Search_By_Date_Criteria()
        {
            string[] SearChByDate = { "Daily", "Weekly", "Monthly", "At the day of", "Annually", "Date Range", "Period Code", "Period Range", "None", "Show All" };
            cmbDateSearchBy.DataSource = SearChByDate;

            cmbDateSearchBy.View.Columns.Add();
            cmbDateSearchBy.View.Columns[0].FieldName = "Column";
            cmbDateSearchBy.View.Columns[0].Caption = "Search By Date criteria";
            cmbDateSearchBy.View.Columns[0].Visible = true;

            cmbDateSearchBy.DisplayMember = "Column";
            cmbDateSearchBy.ValueMember = "Column";
        }
        private List<FieldFormat> Get_Voucher_Defn_Field_Format()
        {
            try
            {
                var field_Formats = UIProcessManager.Get_Field_Format_By_Reference(voucherDefnition);
                if (field_Formats != null && field_Formats.Count > 0)
                {
                    field_Formats = field_Formats.Where(x => x.Type == 1617).ToList();
                    if (field_Formats != null && field_Formats.Count > 0)
                    {
                        return field_Formats.Select(f => new FieldFormat
                        {
                            columnName = f.Caption,
                            columnBindName = f.ObjectComponent,
                            columnWidth = f.Width,
                            //index = f.Index,
                            //font = f.Font,
                            color = f.Color,
                            columnWrap = f.IsRequired
                        }).ToList();
                    }
                    else
                        return null;
                }
                else
                    return null;
            }
            catch { return null; }
        }
        private bool Create_Grid_Columns()
        {
            try
            {
                if (field_Format_List != null)
                {
                    foreach (var column in field_Format_List)
                    {
                        var grid = new GridColumn
                        {
                            Name = column.columnName,
                            FieldName = column.columnBindName,
                            Caption = column.columnName,


                        };
                        if (column.columnWidth != null)
                            grid.Width = column.columnWidth.Value;
                        if (column.index != null)
                            grid.VisibleIndex = column.index.Value;

                        if (column.columnWrap != null && column.columnWrap.Value)
                        {
                            grid.ColumnEdit = new RepositoryItemMemoEdit();
                            gvVoucher.OptionsView.RowAutoHeight = true;
                        }

                        grid.Visible = true;
                        grid.OptionsColumn.AllowSize = true;
                        grid.OptionsColumn.AllowSort = DefaultBoolean.True;
                        if (column.columnBindName == "IssuedDate")
                        {
                            grid.DisplayFormat.FormatType = FormatType.DateTime;
                        }
                        else if (column.columnBindName == "grandTotal")
                        {
                            var item = new GridColumnSummaryItem(SummaryItemType.Sum,
                                "grandTotal", "Sum={0:N}");
                            grid.Summary.Add(item);

                            grid.DisplayFormat.FormatType = FormatType.Custom;
                            grid.DisplayFormat.FormatString = "{0:N}";
                        }
                        else if (column.columnBindName == "subTotal" || column.columnBindName == "discount" || column.columnBindName == "additionalCharge"
                            || column.columnBindName == "VATtaxableAmount" || column.columnBindName == "VATtaxAmount"
                            || column.columnBindName == "TOT1taxableAmount" || column.columnBindName == "TOT1taxAmount"
                            || column.columnBindName == "TOT2taxableAmount" || column.columnBindName == "TOT2taxAmount"
                            || column.columnBindName == "NonTaxableAmount"
                            || column.columnBindName == "WithholdingTaxAmount" || column.columnBindName == "WithholdingTaxableAmount"
                            || column.columnBindName == "IncomeTaxtaxableAmount" || column.columnBindName == "IncomeTaxAmount"
                            || column.columnBindName == "EmployeePensionTaxableAmount" || column.columnBindName == "EmployeePensionTaxAmount"
                            || column.columnBindName == "CompanyPensionTaxableAmount" || column.columnBindName == "CompanyPensionTaxAmount")
                        {
                            grid.DisplayFormat.FormatType = FormatType.Custom;
                            grid.DisplayFormat.FormatString = "{0:N}";
                        }
                        try
                        {
                            if (column.color != null)
                            {
                                if (column.color.Contains(","))
                                {
                                    string[] col = column.color.Split(',');
                                    int red = Convert.ToInt16(col[0]);
                                    int green = Convert.ToInt16(col[1]);
                                    int blue = Convert.ToInt16(col[2]);
                                    grid.AppearanceHeader.BackColor = Color.FromArgb(red, green, blue);
                                    grid.AppearanceCell.BackColor = Color.FromArgb(red, green, blue);

                                }
                                else if (!column.color.Contains("."))
                                {
                                    grid.AppearanceHeader.BackColor = Color.FromName(column.color);
                                    grid.AppearanceCell.BackColor = Color.FromName(column.color);
                                }
                            }
                            //if(column.font != null)
                            //{
                            //    string[] fontStrings = column.font.Split(',');
                            //    if (fontStrings.Count() == 3)
                            //    {
                            //        fontStrings[1] = fontStrings[1].Replace("pt", "");
                            //        fontStrings[2] = fontStrings[2].Replace("style=", "");
                            //       // grid.AppearanceCell.Font = new System.Drawing.Font(fontStrings[0], float.Parse(fontStrings[1]), ((FontStyle)Enum.Parse(typeof(FontStyle), fontStrings[2])));
                            //    }
                            //    else if (fontStrings.Count() == 2)
                            //    {
                            //        fontStrings[1] = fontStrings[1].Replace("pt", "");
                            //        grid.AppearanceCell.Font = new System.Drawing.Font(fontStrings[0], float.Parse(fontStrings[1]));
                            //    }
                            //    else if(fontStrings.Count() == 1)
                            //    {
                            //        //grid.AppearanceCell.Font = new System.Drawing.Font(fontStrings[0], 12);
                            //    }
                            //}
                        }
                        catch { }
                        gvVoucher.Columns.Add(grid);
                    }
                }
                return true;
            }
            catch { return false; }
        }

        private void Initialize_SearchBy()
        {
            if (documentBrowserType.ToLower() == "lightdocumentbrowser")
            {
                string[] SearChBy = { "Show Filter Panel", "From Store","To Store", "Is Issued", "Is Void",
                                     "Grand Total", "Additional Charge", "Discount", "Sub Total", "VAT",
                                    "FS Number", "MRC Number", "Extension 1","Extension 2","Extension 3","Extension 4","Extension 5","Extension 6"};
                bbSearchByMenu.DataSource = SearChBy;
            }
            else
            {
                string[] SearChBy = { "Show Filter Panel", "From Store","To Store", "Is Issued", "Is Void",
                                    "Consignee 2","Consignee 3","Consignee 4","Consignee 5","Consignee 6",
                                    "Extension 1","Extension 2","Extension 3","Extension 4","Extension 5","Extension 6",
                                    "Grand Total", "Additional Charge", "Discount", "Sub Total", "VAT",
                                    "Forward Reference", "Backward Reference", "FS Number", "MRC Number"};
                bbSearchByMenu.DataSource = SearChBy;
            }
            bbSearchByMenu.View.Columns.Add();
            bbSearchByMenu.View.Columns[0].FieldName = "Column";
            bbSearchByMenu.View.Columns[0].Caption = "Search By";
            bbSearchByMenu.View.Columns[0].Visible = true;

            bbSearchByMenu.DisplayMember = "Column";
            bbSearchByMenu.ValueMember = "Column";

        }
        private List<Consignee> Get_All_Consignees()
        {
            try
            {
                var all_Req_Consignees = UIProcessManager.Get_Required_Consignees(voucherDefnition);
                if (all_Req_Consignees != null && all_Req_Consignees.Count > 0)
                {
                    return all_Req_Consignees.Select(x => new Consignee
                    {
                        ID = x.ID,
                        Code = x.Code,
                        IsPerson = x.IsPerson,
                        Name = x.FirstName + (!string.IsNullOrEmpty(x.SecondName) ? " " + x.SecondName : string.Empty) + (!string.IsNullOrEmpty(x.ThirdName) ? " " + x.ThirdName : string.Empty),
                        TIN = x.TIN,
                        //MainConsigneeUnit = x.mainconsigneeeUnit,
                        PhoneNo = x.PhoneNo
                    }).ToList();
                }
            }
            catch { }
            return null;
        }

        private void Initalize_Period()
        {
            try
            {
                var config = LocalBuffer.LocalBuffer.ConfigurationBufferList.FirstOrDefault(x => x.Reference == voucherDefnition.ToString() && x.Attribute == "Use Period");
                if (config != null && !string.IsNullOrEmpty(config.CurrentValue))
                {
                    var lkp = LocalBuffer.LocalBuffer.SystemConstantDTOBufferList.FirstOrDefault(x => x.Type == "Lookup" && x.Category == "Period Type" && x.Description == config.CurrentValue);
                    if (lkp != null)
                        treeListLookUpPeriod.DataSource = UIProcessManager.GetPeriodByType(lkp.Id);
                }
            }
            catch { }
        }
        private void Browse_Voucher_Document()
        {
            try
            {
                gvVoucher.ShowLoadingPanel();

                var result = UIProcessManager.Get_Document_Browser_Data(Get_Filter_Url());

                gcDocumentViewer.DataSource = result.ToList();
                gcDocumentViewer.RefreshDataSource();

                gvVoucher.HideLoadingPanel();
            }
            catch (Exception io)
            {
                gvVoucher.HideLoadingPanel();
            }
        }


        private string Get_Filter_Url()
        {
            var queryString = System.Web.HttpUtility.ParseQueryString(string.Empty);
            queryString.Add("definitionId", voucherDefnition.ToString());
            queryString.Add("fieldFormatType", "1617");

            if (bbFromDate.EditValue != null && bbToDate.EditValue != null)
            {
                queryString.Add(":issuedDate", bbFromDate.EditValue.ToString());
                queryString.Add("issuedDate:", bbToDate.EditValue.ToString());
            }
            else if (bbFromDate.EditValue != null && bbToDate.EditValue == null)
            {
                queryString.Add("issuedDate", bbFromDate.EditValue.ToString());
                //queryString.Add("issuedDate:", bbToDate.EditValue.ToString());
            }
            if (bbVoucherCode.EditValue != null && !string.IsNullOrEmpty(bbVoucherCode.EditValue.ToString()))
                queryString.Add("code", bbVoucherCode.EditValue.ToString());
            if (bbConsignee.EditValue != null && !string.IsNullOrEmpty(bbConsignee.EditValue.ToString()))
                queryString.Add("consignee1Id", bbConsignee.EditValue.ToString());
            if (bbConsigneeUnit.EditValue != null && !string.IsNullOrEmpty(bbConsigneeUnit.EditValue.ToString()))
                queryString.Add("originConsigneeId", bbConsigneeUnit.EditValue.ToString());
            if (bbDevice.EditValue != null && !string.IsNullOrEmpty(bbDevice.EditValue.ToString()))
                queryString.Add("lastDeviceId", bbDevice.EditValue.ToString());
            if (bbUser.EditValue != null && !string.IsNullOrEmpty(bbUser.EditValue.ToString()))
                queryString.Add("lastUserId", bbUser.EditValue.ToString());
            if (bbPeriod.EditValue != null && !string.IsNullOrEmpty(bbPeriod.EditValue.ToString()))
                queryString.Add("period", bbPeriod.EditValue.ToString());

            if (From_Store != null)
                queryString.Add("sourceStoreId", From_Store.ToString());
            if (To_Store != null)
                queryString.Add("destinationStoreId", To_Store.ToString());

            if (Is_Issued != null)
                queryString.Add("isIssued", Is_Issued.ToString());
            if (Is_Isvoid != null)
                queryString.Add("isVoid", Is_Isvoid.ToString());

            if (!string.IsNullOrEmpty(Grand_Total))
                queryString.Add("grandTotal", Grand_Total);
            if (!string.IsNullOrEmpty(Add_Charge))
                queryString.Add("additionalCharge", Add_Charge);
            if (!string.IsNullOrEmpty(Discount))
                queryString.Add("discount", Discount);
            if (!string.IsNullOrEmpty(Sub_Total))
                queryString.Add("subTotal", Sub_Total);
            if (!string.IsNullOrEmpty(VAT))
                queryString.Add("VAT", VAT);
            if (!string.IsNullOrEmpty(FS_No))
                queryString.Add("fsNumber", FS_No);
            if (!string.IsNullOrEmpty(MRC_No))
                queryString.Add("mrc", MRC_No);
            if (!string.IsNullOrEmpty(Ext1))
                queryString.Add("extension1", Ext1);
            if (!string.IsNullOrEmpty(Ext2))
                queryString.Add("extension2", Ext2);
            if (!string.IsNullOrEmpty(Ext3))
                queryString.Add("extension3", Ext3);
            if (!string.IsNullOrEmpty(Ext4))
                queryString.Add("extension4", Ext4);
            if (!string.IsNullOrEmpty(Ext5))
                queryString.Add("extension5", Ext5);
            if (!string.IsNullOrEmpty(Ext6))
                queryString.Add("extension6", Ext6);

            return queryString.ToString();
        }
        private void Default_Values()
        {
            From_Store = To_Store = null;
            Is_Issued = Is_Isvoid = null;
            Grand_Total = Add_Charge = Discount = Sub_Total = VAT = FS_No = MRC_No = Ext1 = Ext2 = Ext3 = Ext4 = Ext5 = Ext6 = null;


            dateSearch = "Show All";
            issuedDate = null;



            bbComboParam.Visibility = BarItemVisibility.Never;
            bbTextParam.Visibility = BarItemVisibility.Never;
            bbFromDate.Enabled = false;
            bbFromDate.Visibility = BarItemVisibility.Never;
            bbToDate.Enabled = false;
            bbToDate.Visibility = BarItemVisibility.Never;

            bbPeriod.Visibility = BarItemVisibility.Never;
            bbTextParam.Visibility = BarItemVisibility.Never;
            bbComboParam.Visibility = BarItemVisibility.Never;


            bbVoucherCode.EditValue = null;
            if (bbConsignee.Edit.ReadOnly == false)
            {
                bbConsignee.EditValue = null;
            }
            bbPeriod.EditValue = null;


            bbFromDate.Reset();
            bbFromDate.Enabled = true;
            bbFromDate.Caption = "From";
            bbFromDate.EditValue = null;
            bbFromDate.Enabled = false;

            bbToDate.Reset();
            bbToDate.Enabled = true;
            bbToDate.Caption = "To";
            bbToDate.EditValue = null;
            bbToDate.Enabled = false;


            bbTextParam.EditValue = null;
            bbComboParam.EditValue = null;
            bbSearchBy.EditValue = null;
            bbObjectState.EditValue = null;

            Initialize_SearchBy();
            Initialize_Search_By_Date_Criteria();
        }
        private void Show_Voucher_Detail()
        {
            try
            {
                gcVoucherDetail.DataSource = null;

                var row = gvVoucher.GetFocusedRow() as VwVoucherHeaderDTO;
                if (row != null && row.Id != null)
                {
                    var voucher = UIProcessManager.GetVoucherById(row.Id);
                    if (voucher != null)
                    {
                        #region Voucher Detail
                        if (dpVoucherDetail.Visibility == DockVisibility.Visible)
                        {
                            Progress_Reporter.Show_Progress("Loading Voucher Details...", "Please Wait...");
                            listOfRightView = new List<Right_view>();
                            var activities = UIProcessManager.GetActivityByreference(voucher.Id);
                            if (activities != null && activities.Count > 0)
                            {
                                foreach (var act in activities)
                                {
                                    rv = new Right_view();
                                    rv.Detail = "Activity Definition";
                                    rv.RightKey = string.Format("{0} by {1}", Get_Activity_Defn_Desc(act.ActivityDefinition), Get_User_Name(act.User));
                                    rv.RightValue = string.Format("[{0}][{1}] {2}", act.TimeStamp, Get_Device_Name(act.Device), Get_Branch_Name(act.ConsigneeUnit));
                                    listOfRightView.Add(rv);
                                }
                            }

                            //Non Cash Transaction

                            #region Store Transaction
                            if (voucher.SourceStore != null)
                            {
                                var source = LocalBuffer.LocalBuffer.AllHotelBranchBufferList.FirstOrDefault(x => x.Id == voucher.SourceStore);
                                if (source != null)
                                {
                                    rv = new Right_view();
                                    rv.Detail = "Store Transaction";
                                    rv.RightKey = "Source";
                                    rv.RightValue = source.Description;
                                    listOfRightView.Add(rv);
                                }
                            }
                            if (voucher.DestinationStore != null)
                            {
                                var destination = LocalBuffer.LocalBuffer.AllHotelBranchBufferList.FirstOrDefault(x => x.Id == voucher.DestinationStore);
                                if (destination != null)
                                {
                                    rv = new Right_view();
                                    rv.Detail = "Store Transaction";
                                    rv.RightKey = "Destination";
                                    rv.RightValue = destination.Description;
                                    listOfRightView.Add(rv);
                                }
                            }
                            #endregion


                            if (voucher.SubTotal != null && Math.Round((decimal)voucher.SubTotal, 2) > 0)
                            {
                                rv = new Right_view();
                                rv.Detail = "Voucher Value";
                                rv.RightKey = "Sub Total";
                                rv.RightValue = string.Format("{0:N}", voucher.SubTotal);
                                listOfRightView.Add(rv);
                            }

                            if (voucher.AddCharge != null && Math.Round((decimal)voucher.AddCharge, 2) > 0)
                            {
                                rv = new Right_view();
                                rv.Detail = "Voucher Value";
                                rv.RightKey = "Additional Charge";
                                rv.RightValue = string.Format("{0:N}", voucher.AddCharge);
                                listOfRightView.Add(rv);
                            }

                            if (voucher.Discount != null && Math.Round((decimal)voucher.Discount, 2) > 0)
                            {
                                rv = new Right_view();
                                rv.Detail = "Voucher Value";
                                rv.RightKey = "Discount";
                                rv.RightValue = string.Format("{0:N}", voucher.Discount);
                                listOfRightView.Add(rv);
                            }

                            #region Voucher Extension
                            var voucherExtensions = UIProcessManager.GetVoucherByExtensionDefinitionByVoucher(voucher.Definition);
                            foreach (var vex in voucherExtensions.OrderBy(x => x.Index))
                            {
                                if (vex.Index == 1 && !string.IsNullOrEmpty(voucher.Extension1))
                                {
                                    rv = new Right_view();
                                    rv.Detail = "Voucher Extension";
                                    rv.RightKey = vex.Descritpion;
                                    rv.RightValue = voucher.Extension1;
                                    listOfRightView.Add(rv);
                                }
                                if (vex.Index == 2 && !string.IsNullOrEmpty(voucher.Extension2))
                                {
                                    rv = new Right_view();
                                    rv.Detail = "Voucher Extension";
                                    rv.RightKey = vex.Descritpion;
                                    rv.RightValue = voucher.Extension2;
                                    listOfRightView.Add(rv);
                                }
                                if (vex.Index == 3 && !string.IsNullOrEmpty(voucher.Extension3))
                                {
                                    rv = new Right_view();
                                    rv.Detail = "Voucher Extension";
                                    rv.RightKey = vex.Descritpion;
                                    rv.RightValue = voucher.Extension3;
                                    listOfRightView.Add(rv);
                                }
                                if (vex.Index == 4 && !string.IsNullOrEmpty(voucher.Extension4))
                                {
                                    rv = new Right_view();
                                    rv.Detail = "Voucher Extension";
                                    rv.RightKey = vex.Descritpion;
                                    rv.RightValue = voucher.Extension4;
                                    listOfRightView.Add(rv);
                                }
                                if (vex.Index == 5 && voucher.Extension5 != null)
                                {
                                    rv = new Right_view();
                                    rv.Detail = "Voucher Extension";
                                    rv.RightKey = vex.Descritpion;
                                    rv.RightValue = voucher.Extension5.ToString();
                                    listOfRightView.Add(rv);
                                }
                                if (vex.Index == 2 && voucher.Extension6 != null)
                                {
                                    rv = new Right_view();
                                    rv.Detail = "Voucher Extension";
                                    rv.RightKey = vex.Descritpion;
                                    rv.RightValue = voucher.Extension6.ToString();
                                    listOfRightView.Add(rv);
                                }
                            }
                            #endregion

                            #region Tax Transaction
                            var taxTransaction = UIProcessManager.GetTaxTransactionByVoucher(voucher.Id);
                            foreach (var taxTrans in taxTransaction)
                            {
                                var tax = LocalBuffer.LocalBuffer.TaxBufferList.FirstOrDefault(x => x.Id == taxTrans.Tax);
                                if (tax != null)
                                {
                                    rv = new Right_view();
                                    rv.Detail = "Tax Transaction";
                                    rv.RightKey = string.Format("{0} [{1}%] of {2}", tax.Description, Math.Round(tax.Amount, 0), string.Format("{0:N}", taxTrans.TaxableAmount));
                                    rv.RightValue = string.Format("{0:N}", taxTrans.TaxAmount);
                                    listOfRightView.Add(rv);
                                }
                            }
                            #endregion

                            #region Transaction Reference
                            var transRef = UIProcessManager.GetTransactionReferenceByreferenced(voucher.Id);
                            if (transRef != null && transRef.Count > 0)
                            {
                                foreach (var view in transRef)
                                {
                                    var refering_Voucher = UIProcessManager.GetVoucherById(view.Referring.Value);
                                    if (refering_Voucher != null)
                                    {
                                        rv = new Right_view();
                                        rv.Detail = "Transaction Reference Forward";
                                        rv.RightKey = refering_Voucher.Code;
                                        rv.RightValue = string.Format("{0:N}", view.Value.Value);
                                        rv.isVoucherCode = true;
                                        listOfRightView.Add(rv);
                                    }
                                }
                            }

                            transRef = UIProcessManager.GetTransactionReferenceByreferring(voucher.Id);
                            if (transRef != null && transRef.Count > 0)
                            {
                                foreach (var view in transRef)
                                {
                                    var referenced_Voucher = UIProcessManager.GetVoucherById(view.Referenced.Value);
                                    if (referenced_Voucher != null)
                                    {
                                        rv = new Right_view();
                                        rv.Detail = "Transaction Reference Backward";
                                        rv.RightKey = referenced_Voucher.Code;
                                        rv.RightValue = string.Format("{0:N}", view.Value.Value);
                                        rv.isVoucherCode = true;
                                        listOfRightView.Add(rv);
                                    }
                                }
                            }
                            #endregion

                            #region Shift
                            if (voucher.Shift != null)
                            {
                                var shiftt = UIProcessManager.GetPeriodById(voucher.Shift.Value);
                                if (shiftt != null)
                                {
                                    rv = new Right_view();
                                    rv.Detail = "Period";
                                    rv.RightKey = shiftt.PeriodName;
                                    rv.RightValue = String.Format("({0} to {1})", shiftt.Start.ToShortDateString(), shiftt.End.ToShortDateString());
                                    listOfRightView.Add(rv);
                                }
                            }
                            #endregion

                            gcVoucherDetail.DataSource = listOfRightView;
                            gcVoucherDetail.RefreshDataSource();
                            gvVoucherDetail.ExpandAllGroups();

                            Progress_Reporter.Close_Progress();
                        }
                        #endregion

                        #region Line Item Detail
                        if (dpLineItemDetail.Visibility == DockVisibility.Visible)
                        {
                            gcLineItemDetail.DataSource = UIProcessManager.Get_voucherDetailReport(voucher.Id);
                            gvLineItemDetail.BestFitColumns();
                            gcLineItemDetail.RefreshDataSource();
                            gvLineItemDetail.ExpandAllGroups();
                        }
                        #endregion
                    }



                }
            }
            catch { Progress_Reporter.Close_Progress(); }
        }
        private string Get_User_Name(int? id)
        {
            if (id == null) return "";
            var user = LocalBuffer.LocalBuffer.UserBufferList.FirstOrDefault(x => x.Id == id);
            if (user != null)
                return user.UserName;
            else
                return "";
        }
        private string Get_Activity_Defn_Desc(int? actDefn)
        {
            if (actDefn == null) return "";
            var act_Defn = LocalBuffer.LocalBuffer.ActivityDefinitionBufferList.FirstOrDefault(x => x.Id == actDefn);
            if (act_Defn != null)
            {
                var sysConst = LocalBuffer.LocalBuffer.SystemConstantDTOBufferList.FirstOrDefault(x => x.Id == act_Defn.Description);
                if (sysConst != null)
                    return sysConst.Description;
            }
            return "";
        }
        private string Get_Device_Name(int? id)
        {
            if (id == null) return "";
            var device = LocalBuffer.LocalBuffer.DeviceBufferList.FirstOrDefault(x => x.Id == id);
            if (device != null)
                return device.MachineName;
            else
                return "";
        }
        private string Get_Branch_Name(int? id)
        {
            if (id == null) return "";
            var branch = LocalBuffer.LocalBuffer.HotelBranchBufferList.FirstOrDefault(x => x.Id == id);
            if (branch != null)
                return branch.Name;
            else
                return "";
        }

        #endregion

        #region Event Handler
        private void bbDateSearchBy_EditValueChanged(object sender, EventArgs e)
        {
            if (bbDateSearchBy.EditValue != null)
            {
                currentDate = UIProcessManager.GetServiceTime();
                if (currentDate == null)
                {
                    XtraMessageBox.Show("Server DateTime Error !!", "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                bbToDate.EditValue = null;
                bbFromDate.EditValue = null;
                bbPeriod.EditValue = null;
                bbToDate.Visibility = BarItemVisibility.Always;
                bbFromDate.Visibility = BarItemVisibility.Always;
                bbPeriod.Visibility = BarItemVisibility.Never;
                deFromDate.ShowDropDown = ShowDropDown.Never;
                deToDate.ShowDropDown = ShowDropDown.Never;
                deFromDate.ReadOnly = true;
                deToDate.ReadOnly = true;
                if (bbDateSearchBy.EditValue.ToString() == "Daily")
                {
                    bbFromDate.Caption = "Current Date";
                    dateSearch = "Current Date";
                    bbFromDate.EditValue = currentDate;
                    DateTime date = Convert.ToDateTime(bbFromDate.EditValue.ToString());
                    issuedDate = date.ToString("MM-dd-yyyy");
                    issueDateEnd = issuedDate;
                    bbToDate.Visibility = BarItemVisibility.Never;
                }
                else if (bbDateSearchBy.EditValue.ToString() == "None")
                {
                    dateSearch = "None";
                    issuedDate = null;
                    issueDateEnd = null;
                    bbFromDate.Caption = "From";
                    bbToDate.Visibility = BarItemVisibility.Never;
                    bbFromDate.Visibility = BarItemVisibility.Never;
                }
                else if (bbDateSearchBy.EditValue.ToString() == "Weekly")
                {
                    DateTime now = currentDate.Value;
                    var weekStart = DayOfWeek.Monday;
                    DateTime firstdayOfTheWeek = now.AddDays(weekStart - now.DayOfWeek);

                    dateSearch = "Date Range";
                    bbFromDate.Caption = "From";
                    bbFromDate.EditValue = firstdayOfTheWeek.ToString("MM-dd-yyyy");
                    issuedDate = bbFromDate.EditValue.ToString();
                    string WeekDate = currentDate.Value.ToString("MM-dd-yyyy");
                    bbToDate.EditValue = WeekDate;
                    bbToDate.Reset();
                    issueDateEnd = currentDate.Value.ToString("MM-dd-yyyy");
                }
                else if (bbDateSearchBy.EditValue.ToString() == "Monthly")
                {
                    dateSearch = "Date Range";
                    bbFromDate.Caption = "From";
                    DateTime now = currentDate.Value;
                    var startDate = new DateTime(now.Year, now.Month, 1);
                    bbFromDate.EditValue = startDate.ToString("MM-dd-yyyy");
                    issuedDate = bbFromDate.EditValue.ToString();
                    string Month = currentDate.Value.ToString("MM-dd-yyyy");
                    bbToDate.EditValue = Month;
                    bbToDate.Reset();
                    issueDateEnd = currentDate.Value.ToString("MM-dd-yyyy");
                }
                else if (bbDateSearchBy.EditValue.ToString() == "Annually")
                {
                    dateSearch = "Date Range";
                    bbFromDate.Caption = "From";
                    DateTime now = currentDate.Value;
                    var startDate = new DateTime(now.Year, 1, 1);
                    bbFromDate.EditValue = startDate.ToString("MM-dd-yyyy");
                    issuedDate = bbFromDate.EditValue.ToString();
                    string Month = currentDate.Value.ToString("MM-dd-yyyy");
                    bbToDate.EditValue = Month;
                    bbToDate.Reset();
                    issueDateEnd = currentDate.Value.ToString("MM-dd-yyyy");
                }
                else if (bbDateSearchBy.EditValue.ToString() == "At the day of")
                {
                    bbFromDate.Reset();
                    bbToDate.Visibility = BarItemVisibility.Never;
                    bbFromDate.EditValue = null;
                    bbFromDate.Caption = "At the day of";
                    if (!bbFromDate.Enabled)
                    {
                        bbFromDate.Enabled = true;
                    }
                    dateSearch = "Current Date";
                    deFromDate.ShowDropDown = ShowDropDown.SingleClick;
                    deFromDate.ReadOnly = false;
                }
                else if (bbDateSearchBy.EditValue.ToString() == "Date Range")
                {
                    dateSearch = "Date Range";
                    bbFromDate.Caption = "From";
                    deFromDate.ShowDropDown = ShowDropDown.SingleClick;
                    deToDate.ShowDropDown = ShowDropDown.SingleClick;
                    deFromDate.ReadOnly = false;
                    deToDate.ReadOnly = false;

                    if (!bbFromDate.Enabled)
                    {
                        bbFromDate.Enabled = true;
                    }

                    if (!bbToDate.Enabled)
                    {
                        bbToDate.Enabled = true;
                    }
                }
                else if (bbDateSearchBy.EditValue.ToString() == "Show All")
                {
                    dateSearch = "Show All";
                    issuedDate = null;
                    issueDateEnd = null;
                    bbFromDate.Caption = "From";
                    bbToDate.Visibility = BarItemVisibility.Never;
                    bbFromDate.Visibility = BarItemVisibility.Never;
                }
                else if (bbDateSearchBy.EditValue.ToString() == "Period Code")
                {
                    dateSearch = "Show All";
                    issuedDate = null;
                    issueDateEnd = null;
                    bbToDate.Visibility = BarItemVisibility.Never;
                    bbFromDate.Visibility = BarItemVisibility.Never;
                    bbPeriod.Visibility = BarItemVisibility.Always;
                }
                else if (bbDateSearchBy.EditValue.ToString() == "Period Range")
                {
                    dateSearch = "Show All";
                    issuedDate = null;
                    issueDateEnd = null;
                    bbToDate.Visibility = BarItemVisibility.Never;
                    bbFromDate.Visibility = BarItemVisibility.Never;
                    bbPeriod.Visibility = BarItemVisibility.Always;
                }
            }
        }
        private void bbSearchBy_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                bbComboParam.EditValue = null;
                if (bbSearchBy.EditValue != null)
                {
                    switch (bbSearchBy.EditValue.ToString())
                    {
                        case "Show Filter Panel":
                            string[] filterPanel = { "True", "False" };
                            cmbSearchBy.DataSource = filterPanel;
                            cmbSearchBy.View.Columns.Add();
                            cmbSearchBy.View.Columns[0].FieldName = "Column";
                            cmbSearchBy.View.Columns[0].Caption = "Show";
                            cmbSearchBy.View.Columns[0].Visible = true;
                            cmbSearchBy.DisplayMember = "Column";
                            cmbSearchBy.ValueMember = "Column";
                            break;
                        case "From Store":
                            cmbSearchBy.DataSource = LocalBuffer.LocalBuffer.AllHotelBranchBufferList.Where(uip => uip.Type == CNETConstantes.ORG_UNIT_TYPE_STORE).ToList();
                            bbComboParam.Caption = bbSearchBy.EditValue.ToString();
                            cmbSearchBy.View.Columns.Add();
                            cmbSearchBy.View.Columns[0].FieldName = "Name";
                            cmbSearchBy.View.Columns[0].Caption = "From Store";
                            cmbSearchBy.View.Columns[0].Visible = true;
                            cmbSearchBy.ValueMember = "Id";
                            cmbSearchBy.DisplayMember = "Name";
                            break;
                        case "To Store":
                            cmbSearchBy.DataSource = LocalBuffer.LocalBuffer.AllHotelBranchBufferList.Where(uip => uip.Type == CNETConstantes.ORG_UNIT_TYPE_STORE).ToList();
                            bbComboParam.Caption = bbSearchBy.EditValue.ToString();
                            cmbSearchBy.View.Columns.Add();
                            cmbSearchBy.View.Columns[0].FieldName = "Name";
                            cmbSearchBy.View.Columns[0].Caption = "Store";
                            cmbSearchBy.View.Columns[0].Visible = true;
                            cmbSearchBy.ValueMember = "Id";
                            cmbSearchBy.DisplayMember = "Name";
                            break;
                        case "Is Issued":
                            string[] IsIssu = { "True", "False" };
                            cmbSearchBy.DataSource = IsIssu;
                            bbComboParam.Caption = bbSearchBy.EditValue.ToString();
                            cmbSearchBy.View.Columns.Add();
                            cmbSearchBy.View.Columns[0].FieldName = "Column";
                            cmbSearchBy.View.Columns[0].Caption = "Is Issued";
                            cmbSearchBy.View.Columns[0].Visible = true;
                            cmbSearchBy.DisplayMember = "Column";
                            cmbSearchBy.ValueMember = "Column";
                            break;
                        case "Is Void":
                            string[] IsVo = { "True", "False" };
                            cmbSearchBy.DataSource = IsVo;
                            bbComboParam.Caption = bbSearchBy.EditValue.ToString();
                            cmbSearchBy.View.Columns.Add();
                            cmbSearchBy.View.Columns[0].FieldName = "Column";
                            cmbSearchBy.View.Columns[0].Caption = "Is Void";
                            cmbSearchBy.View.Columns[0].Visible = true;
                            cmbSearchBy.DisplayMember = "Column";
                            cmbSearchBy.ValueMember = "Column";
                            break;
                        case "Grand Total":
                        case "Additional Charge":
                        case "Discount":
                        case "Sub Total":
                        case "VAT":
                        case "FS Number":
                        case "MRC Number":
                        case "Extension 1":
                        case "Extension 2":
                        case "Extension 3":
                        case "Extension 4":
                        case "Extension 5":
                        case "Extension 6":
                            bbTextParam.Visibility = BarItemVisibility.Always;
                            bbTextParam.Caption = bbSearchBy.EditValue.ToString();
                            break;
                        case "Consignee 2":
                            break;
                        case "Consignee 3":
                            break;
                        case "Consignee 4":
                            break;
                        case "Consignee 5":
                            break;
                        case "Consignee 6":
                            break;
                        case "Forward Reference":
                            break;
                        case "Backward Reference":
                            break;
                    }
                }
            }
            catch { }
        }
        private void cmbSearchBy_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (bbSearchBy.EditValue != null && bbComboParam.EditValue != null)
                {
                    switch (bbSearchBy.EditValue.ToString())
                    {
                        case "Show Filter Panel":
                            if (bbSearchBy.EditValue.ToString() == "True")
                                gvVoucher.ShowFilterEditor(gvVoucher.Columns[0]);
                            break;
                        case "From Store":
                            From_Store = Convert.ToInt16(bbSearchBy.EditValue);
                            break;
                        case "To Store":
                            To_Store = Convert.ToInt16(bbSearchBy.EditValue);
                            break;
                        case "Is Issued":
                            Is_Issued = Convert.ToBoolean(bbSearchBy.EditValue);
                            break;
                        case "Is Void":
                            Is_Isvoid = Convert.ToBoolean(bbSearchBy.EditValue);
                            break;
                    }
                }
            }
            catch { }
        }
        private void txtSearchBy_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (bbSearchBy.EditValue != null && bbTextParam.EditValue != null)
                {
                    switch (bbSearchBy.EditValue.ToString())
                    {
                        case "Grand Total":
                            Grand_Total = bbTextParam.EditValue.ToString();
                            break;
                        case "Additional Charge":
                            Add_Charge = bbTextParam.EditValue.ToString();
                            break;
                        case "Discount":
                            Discount = bbTextParam.EditValue.ToString();
                            break;
                        case "Sub Total":
                            Sub_Total = bbTextParam.EditValue.ToString();
                            break;
                        case "VAT":
                            VAT = bbTextParam.EditValue.ToString();
                            break;
                        case "FS Number":
                            FS_No = bbTextParam.EditValue.ToString();
                            break;
                        case "MRC Number":
                            MRC_No = bbTextParam.EditValue.ToString();
                            break;
                        case "Extension 1":
                            Ext1 = bbTextParam.EditValue.ToString();
                            break;
                        case "Extension 2":
                            Ext2 = bbTextParam.EditValue.ToString();
                            break;
                        case "Extension 3":
                            Ext3 = bbTextParam.EditValue.ToString();
                            break;
                        case "Extension 4":
                            Ext4 = bbTextParam.EditValue.ToString();
                            break;
                        case "Extension 5":
                            Ext5 = bbTextParam.EditValue.ToString();
                            break;
                        case "Extension 6":
                            Ext6 = bbTextParam.EditValue.ToString();
                            break;
                    }
                }
            }
            catch { }
        }
        private void btnClearSearchHistory_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                Progress_Reporter.Show_Progress("Clearing search history...", "Please Wait...");
                Default_Values();
                if (gvVoucher.IsFindPanelVisible == true)
                {
                    gvVoucher.HideFindPanel();
                    gvVoucher.OptionsFind.ClearFindOnClose = true;
                    gvVoucher.ShowFindPanel();
                    gvVoucher.OptionsFind.ShowCloseButton = false;
                    gvVoucher.OptionsFind.FindMode = FindMode.Always;
                }
                bbDateSearchBy.EditValue = "Show All";
                cmbSearchBy.DataSource = null;
                gcDocumentViewer.DataSource = null;
                gvVoucher.ClearColumnsFilter();
                Progress_Reporter.Close_Progress();
            }
            catch
            {
                Progress_Reporter.Close_Progress();
            }
        }
        private void btnShow_ItemClick(object sender, ItemClickEventArgs e)
        {
            Browse_Voucher_Document();
        }
        private void gvVoucher_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
            try
            {
                if (gvVoucher.FocusedRowHandle >= 0)
                {
                    Show_Voucher_Detail();
                }
            }
            catch { }
        }

        private void barSubExport_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Title = "Save";
                sfd.DefaultExt = "xls";
                sfd.Filter = "*.xls|*.xls|*.xlsx|*.xlsx";
                sfd.FileName = VoucherDefinitionName + " Exported Voucher Document File";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    gcDocumentViewer.ExportToXls(sfd.FileName);
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message, "CNET Document Browser", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnPrint_ItemClick(object sender, ItemClickEventArgs e)
        {
            VwVoucherHeaderDTO selecteddata = (VwVoucherHeaderDTO)gvVoucher.GetFocusedRow();
            if (selecteddata != null)
            {
                DocumentPrint.ReportGenerator reportGenerator = new DocumentPrint.ReportGenerator();
                reportGenerator.GetAttachementReport(selecteddata.Id);
            }
        }

        private void gvVoucher_RowStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs e)
        {
            //try
            //{

            //    if (e.RowHandle >= 0)
            //    {
            //        GridView View = sender as GridView;
            //        bool strikeout = false;

            //        string colorHTML = "Black";

            //        // LastStateColor

            //        VwVoucherHeaderDTO row = (VwVoucherHeaderDTO)gvVoucher.GetRow(e.RowHandle);
            //        if (row != null)
            //        {
            //            colorHTML = row.LastStateColor;
            //            strikeout = row.IsVoid;
            //        }


            //        e.Appearance.ForeColor = ColorTranslator.FromHtml(colorHTML);
            //        if (strikeout)
            //        {
            //            e.Appearance.Font = new System.Drawing.Font(e.Appearance.Font, FontStyle.Strikeout);
            //        }
            //        else
            //        {
            //            e.Appearance.Font = new System.Drawing.Font(e.Appearance.Font, FontStyle.Regular);
            //        }

            //    }
            //}
            //catch (Exception ex)
            //{


            //}


        }

        private void gvVoucher_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            try
            {

                if (e.RowHandle >= 0)
                {
                    GridView View = sender as GridView;
                    bool strikeout = false;

                    string colorHTML = "Black";

                    // LastStateColor

                    VwVoucherHeaderDTO row = (VwVoucherHeaderDTO)gvVoucher.GetRow(e.RowHandle);
                    if (row != null)
                    {
                        if (!string.IsNullOrEmpty(row.LastStateColor))
                            colorHTML = row.LastStateColor;

                        strikeout = row.IsVoid;
                    }


                    e.Appearance.ForeColor = ColorTranslator.FromHtml(colorHTML);
                    if (strikeout)
                    {
                        e.Appearance.Font = new System.Drawing.Font(e.Appearance.Font, FontStyle.Strikeout);
                    }
                    else
                    {
                        e.Appearance.Font = new System.Drawing.Font(e.Appearance.Font, FontStyle.Regular);
                    }

                }
            }
            catch (Exception ex)
            {


            }
        }
    }
    #endregion


    public class VoucherRoomPOSChargeClicked : EventArgs
    {
        public class RoomPOSChargeButtonClicked
        {

        }
    }
}
