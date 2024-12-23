using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CNET.ERP.Client.Common.UI;
using DevExpress.Utils.Menu;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid;
using DevExpress.Data;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraReports.UI;
using System.Threading.Tasks;
using CNET.Progress.Reporter;
using CNET.FrontOffice_V._7;
using CNET.FrontOffice_V._7.APICommunication;
using CNET.POS.DocumentBrowser.Models;
using DevExpress.Utils;
using DevExpress.XtraEditors.Repository;
using CNET.POS.Common.Models;
using System.Reflection.Metadata;
using DevExpress.Printing.Core.PdfExport.Metafile;
using DevExpress.XtraVerticalGrid;
using DevExpress.XtraRichEdit.Model;
using DevExpress.CodeParser;

namespace DocumentBrowser
{
    public partial class ConsigneeDocument : UserControl
    {
        #region PersonFilter
        private string personCode;
        private string title;
        private string firstName;
        private string secondName;
        private string thirdName;
        private string fullname;
        private string gslType;
        private string gender;
        private string genderName;
        private string nationality;
        private string nationalityName;
        private string dob;
        private bool isActive = true;
        private string personTitle;
        private string identificationDescription;
        private string idNumber; 
        private string address;
        private string tax;
        private string taxName;
        private string preference;
        private string preferenceName;
        private string prefParent;
        private string addressPreference;
        private string identificationType;
        private string index;
        private int? age;
        private string operatorV;
        private string dateSearch;
        private string startDate;
        private string endDate;
        private string objectStateDefnition;
        private string objectStateDefnitionName;
        #endregion


        public string GslTypeName = "";
        public int GslTypeId = 0;
        public bool IsPerson = false;
        private string businessType;
        private string businessTypeName;


        private List<FieldFormat> field_Format_List;

        #region Constructor
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reference"></param>
        public ConsigneeDocument(int GslType)
        {
            Progress_Reporter.Show_Progress("Initializing Components...", "Please Wait...");
            InitializeComponent();

            GslTypeId = GslType;

            var VoucherDefinitionN = LocalBuffer.SystemConstantDTOBufferList.FirstOrDefault(x => x.Id == GslType);
            if (VoucherDefinitionN != null)
            {
                GslTypeName = VoucherDefinitionN.Description;
                docPersonDetail.Text = GslTypeName + " Detail";
            }
            else
            {
                Progress_Reporter.Close_Progress();
                XtraMessageBox.Show(String.Format("Sorry! GSLType Is Not Defined For GslType ={0}.", GslType), "Gsl Type Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                ribbonControl1.Enabled = false;
                return;
            }


            Progress_Reporter.Show_Progress("Loading Field Formats...", "Please Wait...");
            field_Format_List = Get_GslType_Field_Format();


            if (field_Format_List == null || field_Format_List.Count == 0)
            {
                Progress_Reporter.Close_Progress();
                XtraMessageBox.Show(String.Format("Field Format Not Available For {0}.", GslTypeName), "Field Format Not Found", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                ribbonControl1.Enabled = false;
            }


            Progress_Reporter.Show_Progress("Loading Default Document Configuration...", "Please Wait...");
            var value = LocalBuffer.ConfigurationBufferList.FirstOrDefault(uip => uip.Reference == GslTypeId.ToString() && uip.Preference == "Gsl" && uip.Attribute == "Default Document");
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
            value = LocalBuffer.ConfigurationBufferList.FirstOrDefault(uip => uip.Reference == GslTypeId.ToString() && uip.Preference == "Voucher" && uip.Attribute == "Show Find Panel");
            if (value != null && value.CurrentValue.ToLower() == "true")
            {
                gvConsignee.ShowFindPanel();
               // gvConsignee.OptionsFind.FindMode = FindMode.Always;
                gvConsignee.OptionsFind.ShowCloseButton = false;
            }

            Progress_Reporter.Show_Progress("Loading Show Group Panel Configuration...", "Please Wait...");
            value = LocalBuffer.ConfigurationBufferList.FirstOrDefault(uip => uip.Reference == GslTypeId.ToString() && uip.Preference == "Voucher" && uip.Attribute == "Show Group Panel");
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

            Initialize_Search_By_Date_Criteria();



            Progress_Reporter.Show_Progress("Getting Consignees...", "Please Wait...");
            cmbConsignee.DataSource = Get_All_Consignees();


            Progress_Reporter.Show_Progress("Initalizing Object States...", "Please Wait...");
            chkCmbObjectState.DataSource = LocalBuffer.SystemConstantDTOBufferList.Where(x => x.Type == "ObjectState Definition" && x.Category == "Transaction" && x.IsActive).ToList();


            Progress_Reporter.Show_Progress("Initalizing Preference...", "Please Wait...");
            tlPreference.DataSource = LocalBuffer.PreferenceBufferList.Where(x=> x.Reference == GslTypeId);



            Progress_Reporter.Close_Progress();
             
            gcConsignee.DataSource = null;


            gcConsignee.EmbeddedNavigator.Appearance.BackColor = Color.LightYellow;
            gcConsignee.EmbeddedNavigator.Appearance.ForeColor = Color.Red;
            gcConsignee.EmbeddedNavigator.Buttons.Append.Visible = false;
            gcConsignee.EmbeddedNavigator.Buttons.CancelEdit.Visible = false;
            gcConsignee.EmbeddedNavigator.Buttons.EndEdit.Visible = false;
            gcConsignee.EmbeddedNavigator.Buttons.NextPage.Visible = false;
            gcConsignee.EmbeddedNavigator.Buttons.PrevPage.Visible = false;
            gcConsignee.EmbeddedNavigator.Buttons.Remove.Visible = false;
            gcConsignee.EmbeddedNavigator.Buttons.Edit.Visible = false;
            gcConsignee.EmbeddedNavigator.Click += EmbeddedNavigator_Click;

        }


        #endregion


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

        private List<FieldFormat> Get_GslType_Field_Format()
        {
            try
            {
                var field_Formats = UIProcessManager.Get_Field_Format_By_Reference(GslTypeId);
                if (field_Formats != null && field_Formats.Count > 0)
                {
                    return field_Formats.Select(f => new FieldFormat
                    {
                        columnName = f.Caption,
                        columnBindName = f.FieldComponent,
                        columnWidth = f.Width,
                        index = f.Index,
                        font = f.Font,
                        color = f.Color,
                        columnWrap = f.IsRequired
                    }).ToList();
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
                        if (column.columnBindName == "IssuedDate")
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
                string[] SearChBy = { "Show Filter Panel", "Nationality", "Name", "Gender", "Person Title",   "ID", "Age","Phone1", "Email","First Name", "Is Active" };
                bbSearchByMenu.DataSource = SearChBy;
            }
            else
            {
                string[] SearChBy = { "Show Filter Panel", "Business Type", "Tin",  "Phone1", "Email", "Is Active" };
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
                var all_Req_Consignees = UIProcessManager.GetConsigneeBygslType(GslTypeId);
                if (all_Req_Consignees != null && all_Req_Consignees.Count > 0)
                {
                    return all_Req_Consignees.Select(x => new Consignee
                    {
                        ID = x.Id,
                        Code = x.Code,
                        IsPerson = x.IsPerson,
                        Name = x.FirstName + (!string.IsNullOrEmpty(x.SecondName) ? " " + x.SecondName : string.Empty) + (!string.IsNullOrEmpty(x.ThirdName) ? " " + x.ThirdName : string.Empty),
                        TIN = x.BioId,
                    }).ToList();
                }
            }
            catch { }
            return null;
        }


        private string Get_Filter_Url()
        {
            var queryString = System.Web.HttpUtility.ParseQueryString(string.Empty);
            queryString.Add("gslType", GslTypeId.ToString());
            queryString.Add("fieldFormatType", "1");

            if (bbFromDate.EditValue != null)
                queryString.Add("startDate", bbFromDate.EditValue.ToString());
            if (bbToDate.EditValue != null)
                queryString.Add("endDate", bbToDate.EditValue.ToString());

            if (lkuPreference.EditValue != null && !string.IsNullOrEmpty(lkuPreference.EditValue.ToString()))
                queryString.Add("childpreferenceID", lkuPreference.EditValue.ToString());


            if (!string.IsNullOrEmpty(personCode))
                queryString.Add("code", personCode);

            if (!string.IsNullOrEmpty(title))
                queryString.Add("title", title);
/*
            if (!string.IsNullOrEmpty(firistName))
                queryString.Add("firstName", firistName);

            if (!string.IsNullOrEmpty(middleName))
                queryString.Add("secondName", middleName);

            if (!string.IsNullOrEmpty(lastName))
                queryString.Add("thirdName", lastName);

            if (!string.IsNullOrEmpty(firistName))
                queryString.Add("title", firistName);
            if (!string.IsNullOrEmpty(firistName))
                queryString.Add("title", firistName);
            if (!string.IsNullOrEmpty(firistName))
                queryString.Add("title", firistName);
            if (!string.IsNullOrEmpty(firistName))
                queryString.Add("title", firistName);
            if (!string.IsNullOrEmpty(firistName))
                queryString.Add("title", firistName);


            middleName

                lastName

                name
                objectStateDefnitionName


                genderName
                nationalityName

                dob
                isActive
                personTitle
                idNumber


                issueDate

                taxName

                preferenceName

                */





            return queryString.ToString();
        }

        private void Browse_Consignee_Document()
        {
            try
            {
                gvConsignee.ShowLoadingPanel();

                var result = UIProcessManager.Get_Consignee_Browser_Data(Get_Filter_Url());

                gcConsignee.DataSource = result.ToList();
                gcConsignee.RefreshDataSource();

                gvConsignee.HideLoadingPanel();
            }
            catch { }
        }
        #endregion
        #region Events

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bbnRefresh_ItemClick(object sender, ItemClickEventArgs e)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BarButtonItem_clicked(object sender, ItemClickEventArgs e)
        {
        }


    

        /// <summary>
        /// This event handles the show button click.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnShow_ItemClick(object sender, ItemClickEventArgs e)
        {
            Browse_Consignee_Document();
        }

        /// <summary>
        /// This event handles the context menu clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ContextMenuClicked(object sender, EventArgs e)
        {
            try
            {
                DXMenuItem clickedItem = sender as DXMenuItem;
                string name = clickedItem.Caption;
                OperationList(name);
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message, "Context Menu Error");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        private void OperationList(string name)
        {
        }

        /// <summary>
        /// This method takes takes two arguments voucher code and activity Definition.
        /// </summary>
        /// <param name="voucherRef"></param>
        /// <param name="activityDef"></param>
        /// <returns></returns>




        /// <summary>
        /// This event handles the export to excel from the grid view.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bbtnExcelExport_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Title = "Save";
                sfd.DefaultExt = "xls";
                sfd.Filter = "*.xls|*.xls|*.xlsx|*.xlsx";
                sfd.FileName = "Exported Person Document File";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    gcConsignee.ExportToXls(sfd.FileName);
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message, "Document Browser");
            }
        }
        /// <summary>
        /// This event handles the .csv file to excel from the grid view.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bbtnCsvExport_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Title = "Save";
                sfd.DefaultExt = "xls";
                sfd.Filter = "|*.csv";
                sfd.FileName = "Exported Person Document File";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    gcConsignee.ExportToCsv(sfd.FileName);
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message, "Document Browser");
            }
        }

        /// <summary>
        /// This event handles the export to pdf file from the grid view.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bbtnPdfExport_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Title = "Save";
                sfd.DefaultExt = "xls";
                sfd.Filter = "*.pdf|*.pdf";
                sfd.FileName = "Exported Person Document File";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    gcConsignee.ExportToPdf(sfd.FileName);
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message, "Document Browser");
            }
        }

        /// <summary>
        /// This event handles clearing all search criteria and give them default value.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bbtnShowAll_ItemClick(object sender, ItemClickEventArgs e)
        {
        }

        private void gcDocumentViewer_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void gcDocumentViewer_DoubleClick(object sender, EventArgs e)
        {
        }

        private void gvPerson_RowStyle(object sender, RowStyleEventArgs e)
        {
        }

        private void BBDateSearch_EditValueChanged(object sender, EventArgs e)
        {
        }

        private void searchByAtTheDayOf(object sender, EventArgs e)
        {
        }

        private void FromDateSearch(object sender, EventArgs e)
        {
        }

        private void SearchByRange(object sender, EventArgs e)
        {

        }

        private void SearchByPersonCode(object sender, EventArgs e)
        {

        }

        private void tlPreference_EditValueChanged(object sender, EventArgs e)
        {
        }


        private void lkuPreference_EditValueChanged(object sender, EventArgs e)
        {
        }


        /// <summary>
        /// This event handles the selection search by combo. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bbSearchBy_EditValueChanged(object sender, EventArgs e)
        {
            if (bbSearchBy.EditValue != null)
            {
                bbComboParam.Visibility = BarItemVisibility.Always;
                bbTextParam.Visibility = BarItemVisibility.Never;
                bbComboParam.EditValue = null;
                bbTextParam.EditValue = null;
                cmbSearchBy.View.Columns.Clear();

                var name = bbSearchBy.EditValue.ToString();
                bbComboParam.Caption = name;
                if (IsPerson)
                {
                    switch (name)
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
                             cmbSearchBy.DataSource = LocalBuffer.CountryBufferList;

                            cmbSearchBy.View.Columns.Add();
                            cmbSearchBy.View.Columns[0].FieldName = "nationalityName";
                            cmbSearchBy.View.Columns[0].Caption = "Nationality";
                            cmbSearchBy.View.Columns[0].Visible = true;
                            cmbSearchBy.ValueMember = "Id";
                            cmbSearchBy.DisplayMember = "Nationality";
                            break;
                        case "Name":
                            bbTextParam.Visibility = BarItemVisibility.Always;
                            string[] NameList = { "First Name", "Middle Name", "Last Name", "Full Name" };
                            cmbSearchBy.DataSource = NameList;
                            bbComboParam.EditValue = NameList[3];
                            bbTextParam.Caption = NameList[3];
                            cmbSearchBy.View.Columns.Add();
                            cmbSearchBy.View.Columns[0].FieldName = "Column";
                            cmbSearchBy.View.Columns[0].Caption = "Select Name";
                            cmbSearchBy.View.Columns[0].Visible = true;
                            cmbSearchBy.DisplayMember = "Column";
                            cmbSearchBy.ValueMember = "Column";
                            break;
                        case "Gender":
                            string[] gender = { "Male", "Female" };
                            cmbSearchBy.DataSource = gender;
                            cmbSearchBy.View.Columns.Add();
                            cmbSearchBy.View.Columns[0].FieldName = "Column";
                            cmbSearchBy.View.Columns[0].Caption = "Gender";
                            cmbSearchBy.View.Columns[0].Visible = true;
                            cmbSearchBy.DisplayMember = "Column";
                            cmbSearchBy.ValueMember = "Column";
                            break;
                        case "Person Title":
                            var persTitle = LocalBuffer.LookUpBufferList.Where(uip => uip.Type == "Personal Title").ToList();
                            cmbSearchBy.DataSource = persTitle;
                            cmbSearchBy.View.Columns.Add();
                            cmbSearchBy.View.Columns[0].FieldName = "description";
                            cmbSearchBy.View.Columns[0].Caption = "Person Title";
                            cmbSearchBy.View.Columns[0].Visible = true;
                            cmbSearchBy.DisplayMember = "description";
                            cmbSearchBy.ValueMember = "description";
                            break;

                        case "Tax":
                            var taxc = LocalBuffer.TaxBufferList.ToList();
                            cmbSearchBy.DataSource = taxc;
                            cmbSearchBy.View.Columns.Add();
                            cmbSearchBy.View.Columns[0].FieldName = "code";
                            cmbSearchBy.View.Columns[0].Caption = "Tax Code";
                            cmbSearchBy.View.Columns[0].Visible = true;
                            cmbSearchBy.View.Columns.Add();
                            cmbSearchBy.View.Columns[1].FieldName = "description";
                            cmbSearchBy.View.Columns[1].Caption = "Tax Name";
                            cmbSearchBy.View.Columns[1].Visible = true;
                            cmbSearchBy.View.Columns.Add();
                            cmbSearchBy.View.Columns[2].FieldName = "amount";
                            cmbSearchBy.View.Columns[2].Caption = "Tax Amount";
                            cmbSearchBy.View.Columns[2].Visible = true;
                            cmbSearchBy.ValueMember = "code";
                            cmbSearchBy.DisplayMember = "description";
                            cmbSearchBy.EditValueChanged += SearchByTax;
                            break;


                        case "Address":
                            cmbSearchBy.DataSource = null;

                            cmbSearchBy.View.Columns.Add();
                            cmbSearchBy.View.Columns[0].FieldName = "Address";
                            cmbSearchBy.View.Columns[0].Caption = "Address";
                            cmbSearchBy.View.Columns[0].Visible = true;

                            cmbSearchBy.ValueMember = "Address";
                            cmbSearchBy.DisplayMember = "Address";

                            cmbSearchBy.EditValueChanged += SearchByAddress;
                            break;
                        case "ID":
                            bbTextParam.Visibility = BarItemVisibility.Always;
                            var IDdesc = LocalBuffer.LookUpBufferList.Where(uip => uip.Type == "Personal Identification Type").ToList();

                            cmbSearchBy.DataSource = IDdesc;
                            bbTextParam.Caption = "ID Number";
                            bbComboParam.Caption = "ID Description";

                            cmbSearchBy.View.Columns.Add();
                            cmbSearchBy.View.Columns[0].FieldName = "description";
                            cmbSearchBy.View.Columns[0].Caption = "ID Description";
                            cmbSearchBy.View.Columns[0].Visible = true;

                            cmbSearchBy.ValueMember = "description";
                            cmbSearchBy.DisplayMember = "description";

                            cmbSearchBy.EditValueChanged += SearchByIDDescription;
                            txtSearchBy.EditValueChanged += SearchByIDNumber;
                            break;
                        case "Age":
                            bbTextParam.Visibility = BarItemVisibility.Always;

                            string[] Operators = { ">", ">=", "=", "<=", "<" };
                            cmbSearchBy.DataSource = Operators;
                            bbComboParam.EditValue = Operators[2];
                           // operatorV = bbComboParam.EditValue.ToString();
                            bbTextParam.Caption = "Value";


                            cmbSearchBy.View.Columns.Add();
                            cmbSearchBy.View.Columns[0].FieldName = "Column";
                            cmbSearchBy.View.Columns[0].Caption = "Select Operator";
                            cmbSearchBy.View.Columns[0].Visible = true;

                            cmbSearchBy.DisplayMember = "Column";
                            cmbSearchBy.ValueMember = "Column";

                            cmbSearchBy.EditValueChanged += ChangeOfOperator;
                            cmbSearchBy.EditValueChanged += SearchByAge2;
                            txtSearchBy.EditValueChanged += SearchByAge;
                            break;
                    }
                }
                else
                {
                    switch (name)
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

                            cmbSearchBy.EditValueChanged += ShowFilterPanel;
                            break;
                        case "Is Active":


                            string[] IsActive = { "True", "False" };
                            cmbSearchBy.DataSource = IsActive;
                            cmbSearchBy.View.Columns.Add();
                            cmbSearchBy.View.Columns[0].FieldName = "Column";
                            cmbSearchBy.View.Columns[0].Caption = "Is Active";
                            cmbSearchBy.View.Columns[0].Visible = true;
                            cmbSearchBy.DisplayMember = "Column";
                            cmbSearchBy.ValueMember = "Column";
                            break;
                        case "Business Type":
                            var businessTyp = LocalBuffer.SystemConstantDTOBufferList.Where(uip => uip.Category == CNETConstantes.BUSINESSTYPE).ToList();
                            cmbSearchBy.DataSource = businessTyp;

                            cmbSearchBy.View.Columns.Add();
                            cmbSearchBy.View.Columns[0].FieldName = "Description";
                            cmbSearchBy.View.Columns[0].Caption = name;
                            cmbSearchBy.View.Columns[0].Visible = true;

                            cmbSearchBy.ValueMember = "Id";
                            cmbSearchBy.DisplayMember = "Description";

                            cmbSearchBy.EditValueChanged += SearchByBusinessType;
                            break; 
                        case "TIN": 
                            txtSearchBy.EditValueChanged += SearchByIDNumber;
                            break;
                    }
                }
            }

        }



        /// <summary>
        /// This event handles the edit value changed of the text box.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bbTextParam_EditValueChanged(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// This event handles the focused row changed of the grid view.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gvPerson_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
        {

        }

        /// <summary>
        /// This event handles the pop menu for the rignt click in the gridview.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gvPerson_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {

        }



        /// <summary>
        /// This event handles the object state selection.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkObjectState_EditValueChanged(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// The event handles the selection of embedded navigator in the gridview.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EmbeddedNavigator_Click(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// This event handles the mouse down on the gridview.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gcDocumentViewer_MouseDown(object sender, MouseEventArgs e)
        {

        }

        /// <summary>
        /// This event handles the row click of gvPerson.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gvPerson_RowClick(object sender, RowClickEventArgs e)
        {

        }

        /// <summary>
        /// When the mouse is hovered on the ribbon control for the first time after loading it intializes 
        /// the datasources of the components in the ribbon control.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ribbonControl1_MouseHover(object sender, EventArgs e)
        {

        }


        private void bbnPrint_ItemClick(object sender, ItemClickEventArgs e)
        {
        }

        private void bsbActivity_ItemClick(object sender, ItemClickEventArgs e)
        {

        }
    }
    #endregion


}



