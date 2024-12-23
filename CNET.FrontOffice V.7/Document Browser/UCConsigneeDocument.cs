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
using DevExpress.XtraPrinting.Preview;

namespace CNET.POS.DocumentBrowser
{
    public partial class UCConsigneeDocument : System.Windows.Forms.UserControl
    {
        #region Declarations
        private int GslTypeId;
        private DateTime? currentDate;
        private string dateSearch;
        public string issuedDate;
        public string issueDateEnd;
        private List<FieldFormat> field_Format_List;
        private string GslTypeName;
        private bool IsPerson = false;
        private List<Right_view> listOfRightView;
        private Right_view rv;
        #endregion


        #region Search Parameters
        int? From_Store, To_Store;
        bool? Is_Issued, Is_Isvoid;
        string Grand_Total, Add_Charge, Discount, Sub_Total, VAT, FS_No, MRC_No, Ext1, Ext2, Ext3, Ext4, Ext5, Ext6;
        #endregion

        public UCConsigneeDocument()
        {
            InitializeComponent();
        }
        public UCConsigneeDocument(int GslTypeId)
        {
            Progress_Reporter.Show_Progress("Initializing Components...", "Please Wait...");
            InitializeComponent();
            this.GslTypeId = GslTypeId;
            Initialize_Search_By_Date_Criteria();
            //      dpVoucherDetail.Visibility = DockVisibility.AutoHide;
            //dpLineItemDetail.Visibility = DockVisibility.AutoHide;
            var GSLTYpeN = LocalBuffer.LocalBuffer.SystemConstantDTOBufferList.FirstOrDefault(x => x.Id == GslTypeId);
            if (GSLTYpeN != null)
            {
                GslTypeName = GSLTYpeN.Description;
                dpConsigneeDetail.Text = GslTypeName + " Detail";

            }
            else
            {
                Progress_Reporter.Close_Progress();
                XtraMessageBox.Show(String.Format("Sorry! Voucher Is Not Defined For GSL Type ={0}.", GslTypeName), "VGSL Type Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                ribbonControl1.Enabled = false;
                return;
            }

            Progress_Reporter.Show_Progress("Loading Field Formats...", "Please Wait...");
            field_Format_List = Get_GSLType_Field_Format();
            if (field_Format_List == null || field_Format_List.Count == 0)
            {
                Progress_Reporter.Close_Progress();
                XtraMessageBox.Show(String.Format("Field Format Not Available For {0}.", GslTypeName), "Field Format Not Found", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                ribbonControl1.Enabled = false;
            }

            Progress_Reporter.Show_Progress("Loading Default Document Configuration...", "Please Wait...");
            var value = LocalBuffer.LocalBuffer.ConfigurationBufferList.FirstOrDefault(uip => uip.Reference == GslTypeId.ToString() && uip.Attribute == "Default Document");
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
            value = LocalBuffer.LocalBuffer.ConfigurationBufferList.FirstOrDefault(uip => uip.Reference == GslTypeId.ToString() && uip.Attribute == "Show Find Panel");
            if (value != null && value.CurrentValue.ToLower() == "true")
            {
                gvConsignee.ShowFindPanel();
                gvConsignee.OptionsFind.FindMode = FindMode.Always;
                gvConsignee.OptionsFind.ShowCloseButton = false;
            }

            Progress_Reporter.Show_Progress("Loading Show Group Panel Configuration...", "Please Wait...");
            value = LocalBuffer.LocalBuffer.ConfigurationBufferList.FirstOrDefault(uip => uip.Reference == GslTypeId.ToString() && uip.Attribute == "Show Group Panel");
            if (value != null)
            {
                if (value.CurrentValue.ToLower() == "true")
                    gvConsignee.OptionsView.ShowGroupPanel = true;
                else if (value.CurrentValue.ToLower() == "false")
                    gvConsignee.OptionsView.ShowGroupPanel = false;
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



            Progress_Reporter.Show_Progress("Getting Preference...", "Please Wait...");
            cmbPreference.DataSource = LocalBuffer.LocalBuffer.PreferenceBufferList.Where(x => x.SystemConstant == GslTypeId);

            Progress_Reporter.Show_Progress("Initalizing Object States...", "Please Wait...");
            chkCmbObjectState.DataSource = LocalBuffer.LocalBuffer.SystemConstantDTOBufferList.Where(x => x.Type == "ObjectState Definition" && x.Category == "Consignee" && x.IsActive).ToList();

            //Progress_Reporter.Show_Progress("Initalizing Period...", "Please Wait...");
            //Initalize_Period();

            //Progress_Reporter.Show_Progress("Initalizing Consignee Units...", "Please Wait...");
            //cmbConsigneeUnit.DataSource = LocalBuffer.LocalBuffer.HotelBranchBufferList;

            Progress_Reporter.Show_Progress("Initalizing Devices...", "Please Wait...");
            cmbDevice.DataSource = UIProcessManager.SelectAllDevice();// Buffers.Device_Buffer;

            Progress_Reporter.Show_Progress("Initalizing Users...", "Please Wait...");
            cmbUser.DataSource = UIProcessManager.SelectAllUser();// Buffers.User_Buffer;

            Progress_Reporter.Close_Progress();


        }

        #region Methods

        private void Initialize_Search_By_Date_Criteria()
        {
            string[] SearChByDate = { "Daily", "Weekly", "Monthly", "At the day of", "Annually", "Date Range", "None", "Show All" };
            cmbDateSearchBy.DataSource = SearChByDate;

            cmbDateSearchBy.View.Columns.Add();
            cmbDateSearchBy.View.Columns[0].FieldName = "Column";
            cmbDateSearchBy.View.Columns[0].Caption = "Search By Date criteria";
            cmbDateSearchBy.View.Columns[0].Visible = true;

            cmbDateSearchBy.DisplayMember = "Column";
            cmbDateSearchBy.ValueMember = "Column";
        }
        private List<FieldFormat> Get_GSLType_Field_Format()
        {
            try
            {
                var field_Formats = UIProcessManager.Get_Field_Format_By_Reference(GslTypeId);
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
                            gvConsignee.OptionsView.RowAutoHeight = true;
                        }

                        grid.Visible = true;
                        grid.OptionsColumn.AllowSize = true;
                        grid.OptionsColumn.AllowSort = DefaultBoolean.True;
                        if (column.columnBindName == "startDate")
                        {
                            grid.DisplayFormat.FormatType = FormatType.DateTime;
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
                            else
                            {
                                grid.AppearanceHeader.BackColor = Color.White;
                                grid.AppearanceCell.BackColor = Color.White;
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
                        gvConsignee.Columns.Add(grid);
                    }
                }
                return true;
            }
            catch { return false; }
        }

        private void Initialize_SearchBy()
        {
            if (IsPerson)
            {

                string[] SearChBy = { "Show Filter Panel", "Nationality", "Gender", "Person Title", "ID", "Age", "Address", "Name", "Is Active" };
                bbSearchByMenu.DataSource = SearChBy;
            }
            else
            {

                string[] SearChBy = { "Show Filter Panel", "Business Type", "Tin", "Address", "Is Active" };
                bbSearchByMenu.DataSource = SearChBy;
            }
            bbSearchByMenu.View.Columns.Add();
            bbSearchByMenu.View.Columns[0].FieldName = "Column";
            bbSearchByMenu.View.Columns[0].Caption = "Search By";
            bbSearchByMenu.View.Columns[0].Visible = true;

            bbSearchByMenu.DisplayMember = "Column";
            bbSearchByMenu.ValueMember = "Column";

        }



        private void Browse_Consignee_Document()
        {
            try
            {
                gvConsignee.ShowLoadingPanel();

                var result = UIProcessManager.Get_Consignee_Browser_Data(Get_Filter_Url());


                gcConsigneeViewer.DataSource = result.ToList();
                gcConsigneeViewer.RefreshDataSource();

                gvConsignee.HideLoadingPanel();
            }
            catch { }
        }


        private string Get_Filter_Url()
        {
            var queryString = System.Web.HttpUtility.ParseQueryString(string.Empty);
            queryString.Add("gslType", GslTypeId.ToString());
            queryString.Add("fieldFormatType", "1617");

            if (bbFromDate.EditValue != null)
                queryString.Add(":startDate", bbFromDate.EditValue.ToString());
            if (bbToDate.EditValue != null)
                queryString.Add("startDate:", bbToDate.EditValue.ToString());

            if (bbConsigneeCode.EditValue != null && !string.IsNullOrEmpty(bbConsigneeCode.EditValue.ToString()))
                queryString.Add("code", bbConsigneeCode.EditValue.ToString());

            if (bbPreference.EditValue != null && !string.IsNullOrEmpty(bbPreference.EditValue.ToString()))
                queryString.Add("childpreferenceID", bbPreference.EditValue.ToString());



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
            bbTextParam.Visibility = BarItemVisibility.Never;
            bbComboParam.Visibility = BarItemVisibility.Never;


            bbConsigneeCode.EditValue = null;
            if (bbPreference.Edit.ReadOnly == false)
            {
                bbPreference.EditValue = null;
            }


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


        #endregion

        #region Event Handler
        private void bbDateSearchBy_EditValueChanged(object sender, EventArgs e)
        {
            if (bbDateSearchBy.EditValue != null)
            {
                currentDate = UIProcessManager.GetServiceTime();
                if (currentDate != null)
                {
                    currentDate = currentDate;
                }
                else
                {
                    XtraMessageBox.Show("Server DateTime Error !!", "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                bbToDate.EditValue = null;
                bbFromDate.EditValue = null;
                bbToDate.Visibility = BarItemVisibility.Always;
                bbFromDate.Visibility = BarItemVisibility.Always;
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
            }
        }
        private void bbSearchBy_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                bbTextParam.Visibility = BarItemVisibility.Never;
                bbComboParam.Visibility = BarItemVisibility.Never;
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
                        case "Nationality":
                            cmbSearchBy.DataSource = LocalBuffer.LocalBuffer.CountryBufferList;
                            bbComboParam.Caption = bbSearchBy.EditValue.ToString();
                            cmbSearchBy.View.Columns.Add();
                            cmbSearchBy.View.Columns[0].FieldName = "nationalityName";
                            cmbSearchBy.View.Columns[0].Caption = "Nationality";
                            cmbSearchBy.View.Columns[0].Visible = true;
                            cmbSearchBy.ValueMember = "Id";
                            cmbSearchBy.DisplayMember = "nationalityName";
                            bbComboParam.Visibility = BarItemVisibility.Always;
                            break;
                        case "Business Type":
                            cmbSearchBy.DataSource = LocalBuffer.LocalBuffer.SystemConstantDTOBufferList.Where(uip => uip.Category == CNETConstantes.BUSINESSTYPE).ToList();
                            bbComboParam.Caption = bbSearchBy.EditValue.ToString();
                            cmbSearchBy.View.Columns.Add();
                            cmbSearchBy.View.Columns[0].FieldName = "Description";
                            cmbSearchBy.View.Columns[0].Caption = "Business Type";
                            cmbSearchBy.View.Columns[0].Visible = true;
                            cmbSearchBy.ValueMember = "Id";
                            cmbSearchBy.DisplayMember = "Description";
                            bbComboParam.Visibility = BarItemVisibility.Always;
                            break;
                        case "Gender":
                            string[] gender = { "Male", "Female" };
                            cmbSearchBy.DataSource = gender;
                            bbComboParam.Caption = bbSearchBy.EditValue.ToString();
                            cmbSearchBy.View.Columns.Add();
                            cmbSearchBy.View.Columns[0].FieldName = "Column";
                            cmbSearchBy.View.Columns[0].Caption = "Gender";
                            cmbSearchBy.View.Columns[0].Visible = true;
                            cmbSearchBy.DisplayMember = "Column";
                            cmbSearchBy.ValueMember = "Column";
                            bbComboParam.Visibility = BarItemVisibility.Always;
                            break;
                        case "Name":
                            bbTextParam.Visibility = BarItemVisibility.Always;
                            string[] NameList = { "First Name", "Middle Name", "Last Name", "Full Name" };
                            cmbSearchBy.DataSource = NameList;
                            bbComboParam.Caption = bbSearchBy.EditValue.ToString();
                            cmbSearchBy.View.Columns.Add();
                            cmbSearchBy.View.Columns[0].FieldName = "Column";
                            cmbSearchBy.View.Columns[0].Caption = "Name";
                            cmbSearchBy.View.Columns[0].Visible = true;
                            cmbSearchBy.DisplayMember = "Column";
                            cmbSearchBy.ValueMember = "Column";
                            bbComboParam.Visibility = BarItemVisibility.Always;
                            break;
                        case "Address":
                            bbTextParam.Visibility = BarItemVisibility.Always;
                            string[] AddressList = { "Phone", "Email" };
                            cmbSearchBy.DataSource = AddressList;
                            bbComboParam.Caption = bbSearchBy.EditValue.ToString();
                            cmbSearchBy.View.Columns.Add();
                            cmbSearchBy.View.Columns[0].FieldName = "Column";
                            cmbSearchBy.View.Columns[0].Caption = "Address";
                            cmbSearchBy.View.Columns[0].Visible = true;
                            cmbSearchBy.DisplayMember = "Column";
                            cmbSearchBy.ValueMember = "Column";
                            bbComboParam.Visibility = BarItemVisibility.Always;
                            break;
                        case "TIN":
                        case "ID":
                        case "Age":
                            bbTextParam.Visibility = BarItemVisibility.Always;
                            bbTextParam.Caption = bbSearchBy.EditValue.ToString();
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
                                gvConsignee.ShowFilterEditor(gvConsignee.Columns[0]);
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
                if (gvConsignee.IsFindPanelVisible == true)
                {
                    gvConsignee.HideFindPanel();
                    gvConsignee.OptionsFind.ClearFindOnClose = true;
                    gvConsignee.ShowFindPanel();
                    gvConsignee.OptionsFind.ShowCloseButton = false;
                    gvConsignee.OptionsFind.FindMode = FindMode.Always;
                }
                bbDateSearchBy.EditValue = "Show All";
                cmbSearchBy.DataSource = null;
                gcConsigneeViewer.DataSource = null;
                gvConsignee.ClearColumnsFilter();
                Progress_Reporter.Close_Progress();
            }
            catch
            {
                Progress_Reporter.Close_Progress();
            }
        }
        private void btnShow_ItemClick(object sender, ItemClickEventArgs e)
        {
            Browse_Consignee_Document();
        }
        private void gvVoucher_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
        }

        private void barSubExport_ItemClick(object sender, ItemClickEventArgs e)
        {

            try
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Title = "Save";
                sfd.DefaultExt = "xls";
                sfd.Filter = "*.xls|*.xls|*.xlsx|*.xlsx";
                sfd.FileName = GslTypeName + " Exported Voucher Document File";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    gcConsigneeViewer.ExportToXls(sfd.FileName);
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message, "CNET Document Browser", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void gvConsignee_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    GridView View = sender as GridView;
                    bool strikeout = false;
                    string colorHTML = "Black";

                    VwConsigneeViewDTO row = (VwConsigneeViewDTO)gvConsignee.GetRow(e.RowHandle);
                    if (row != null)
                    {
                        if (!string.IsNullOrEmpty(row.LastStateColor))
                            colorHTML = row.LastStateColor; 
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


}
