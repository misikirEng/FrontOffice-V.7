
using CNET.ERP.Client.Common.UI;


using CNET.FrontOffice_V._7.Logics;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraVerticalGrid; 
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CNET.FrontOffice_V._7.Forms.Setting_and_Miscellaneous
{
    public partial class frmSetting : UILogicBase
    {
        private PMSSetting pmsSetting;

        private List<Article> articles = null;
        private GridView gv_articleViewGrid;
        private XtraForm mForm;
        private Button mButtonCancel = new Button();
        private SearchLookUpEdit sLuk_article;

        private XtraForm lPrinterForm;
        private Button mbutton;
        private ListBox mList;
        private TextBox tbox;
        private Button lPrinterCancelBtn;

        private string selectedAttribute = "";

        private RepositoryItemTimeEdit _repTimeEdit;
        private RepositoryItemLookUpEdit _repoLookupEdit;
        private RepositoryItemLookUpEdit _repoBillingPrintType;

        /***************************   CONSTRUCTOR ******************/
        public frmSetting()
        {
            InitializeComponent();
            InitializeUI();
            InitializeData();
        }

        #region Helper Methods

        private void InitializeUI()
        {
            mForm = new XtraForm();
            gv_articleViewGrid = new GridView();
            sLuk_article = new SearchLookUpEdit();

            mForm.Size = new System.Drawing.Size(0, 0);
            mForm.Controls.Add(sLuk_article);
            mForm.LostFocus += mForm_LostFocus;
            // mForm.Controls.Add(this.mButtonCancel);
            mButtonCancel.Click += new System.EventHandler(this.mButtonCancel_Click);
            mForm.ControlBox = false;
            mForm.ShowIcon = false;
            mForm.ControlBox = false;
            mForm.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            mForm.StartPosition = FormStartPosition.CenterScreen;
            mForm.TopMost = true;


            //sLukEdit
            this.sLuk_article.Location = new System.Drawing.Point(52, 12);
            this.sLuk_article.Name = "sLuk_article";
            this.sLuk_article.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.sLuk_article.Properties.NullText = "";
            this.sLuk_article.Properties.View = this.gv_articleViewGrid;
            this.sLuk_article.Size = new System.Drawing.Size(0, 0);
            this.sLuk_article.TabIndex = 4;

            //Article
            GridColumn col = gv_articleViewGrid.Columns.AddVisible("Id", "Id");
            col.Visible = true;
            col = gv_articleViewGrid.Columns.AddVisible("name", "Name");
            col.Visible = true;
            sLuk_article.Properties.ValueMember = "Id";
            sLuk_article.Properties.DisplayMember = "name";

            sLuk_article.EditValueChanged += sLuk_article_EditValueChanged;

            //gv_articleViewGrid
            this.gv_articleViewGrid.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gv_articleViewGrid.Name = "gv_articleViewGrid";
            this.gv_articleViewGrid.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gv_articleViewGrid.OptionsSelection.EnableAppearanceHideSelection = false;
            this.gv_articleViewGrid.OptionsView.ShowGroupPanel = false;

            gcProperty.FocusedRowChanged += gcProperty_FocusedRowChanged;
            gcProperty.MouseDown += gcProperty_MouseDown;

        }

        

        private void InitializeData()
        {
            try
            {
                pmsSetting = LoadPMSSettings();
                if (pmsSetting == null)
                    pmsSetting = new PMSSetting();


                articles = UIProcessManager.GetArticleByGSL(CNETConstantes.SERVICES);

                gcProperty.SelectedObject = pmsSetting;


                //populate lookup edit
                _repoLookupEdit = new RepositoryItemLookUpEdit();
                _repoLookupEdit.Columns.Add(new LookUpColumnInfo("Description", "Posting Routine Header"));
                _repoLookupEdit.DisplayMember = "Description";
                _repoLookupEdit.ValueMember = "Id";
                List<PostingRoutineHeader> prHeaderList = ACCUIProcessManager.GetPostingRoutineHeadersByComponent(CNETConstantes.PMS);
                _repoLookupEdit.DataSource = prHeaderList;


                //Populate Billing Printing Type Lookup
                //Fiscal Bill Type
                _repoBillingPrintType = new RepositoryItemLookUpEdit();
                _repoBillingPrintType.Columns.Add(new LookUpColumnInfo("Column", "Fiscal Bill Type"));
                _repoBillingPrintType.DisplayMember = "Column";
                _repoBillingPrintType.ValueMember = "Column";
                string[] fiscalBillTypes = { "Summary", "Long Detail", "Summary Edit" };
                _repoBillingPrintType.DataSource = fiscalBillTypes;



                _repTimeEdit = new RepositoryItemTimeEdit();
                _repTimeEdit.Mask.EditMask = "HH:mm";
                _repTimeEdit.Mask.UseMaskAsDisplayFormat = true;
                gcProperty.RepositoryItems.Add(_repTimeEdit);
                gcProperty.Rows["rowCheckInTime"].Properties.RowEdit = _repTimeEdit;
                gcProperty.Rows["rowCheckOutTime"].Properties.RowEdit = _repTimeEdit;
                gcProperty.Rows["rowLateCheckoutTime"].Properties.RowEdit = _repTimeEdit;
                gcProperty.Rows["rowNightAuditTime"].Properties.RowEdit = _repTimeEdit;
                gcProperty.Rows["rowPostineRoutine"].Properties.RowEdit = _repoLookupEdit;
                gcProperty.Rows["rowDefaultFiscalBillType"].Properties.RowEdit = _repoBillingPrintType;

                gcProperty.Refresh();
            }
            catch (Exception ex)
            {
                SystemMessage.ShowModalInfoMessage("Error has occured in initializng form. Detail:: " + ex.Message, "ERROR");
             
            }
        }

       

        private PMSSetting LoadPMSSettings()
        {
            var configList = LocalBuffer.ConfigurationBufferList.Where(c => c.reference == CNETConstantes.PMS).ToList();

            if (configList == null || configList.Count == 0) return null;
            var checkinTimeConfig = configList.FirstOrDefault(c => c.attribute == CNETConstantes.PMS_SETTING_CheckInTime);
            var checkoutTimeConfig = configList.FirstOrDefault(c => c.attribute == CNETConstantes.PMS_SETTING_CheckOutTime);
            var lateCheckoutTimeConfig = configList.FirstOrDefault(c => c.attribute == CNETConstantes.PMS_SETTING_LateCheckoutTime);
            var nightAuditTimeConfig = configList.FirstOrDefault(c => c.attribute == CNETConstantes.PMS_SETTING_NightAuditTime);
            var undoCheckinMinConfig = configList.FirstOrDefault(c => c.attribute == CNETConstantes.PMS_SETTING_UndoCheckinMin);
            var minRateAdjConfig = configList.FirstOrDefault(c => c.attribute == CNETConstantes.PMS_SETTING_MinRateAdjustment);
            var archivePathConfig = configList.FirstOrDefault(c => c.attribute == CNETConstantes.PMS_SETTING_ArchivePath);
            var archivePrintConfig = configList.FirstOrDefault(c => c.attribute == CNETConstantes.PMS_SETTING_ArchivePrint);
            var lateCheckoutReqPaymentConfig = configList.FirstOrDefault(c => c.attribute == CNETConstantes.PMS_SETTING_LateCheckoutRequiredPayment);
            var lateCheckoutAdditionPaymentConfig = configList.FirstOrDefault(c => c.attribute == CNETConstantes.PMS_SETTING_LateCheckoutAdditionPayment);
            var useLateCheckout = configList.FirstOrDefault(c => c.attribute == CNETConstantes.PMS_SETTING_UseLateCheckout);
            var enforceCheckoutCardReturn = configList.FirstOrDefault(c => c.attribute == CNETConstantes.PMS_SETTING_EnforceCheckoutCardReturn);
            var lostCardArticleCode = configList.FirstOrDefault(c => c.attribute == CNETConstantes.PMS_SETTING_LostCardFeeArticle);
            var labelDesignFile = configList.FirstOrDefault(c => c.attribute == CNETConstantes.PMS_SETTING_LabelDesignFile);
            var labelPrinter = configList.FirstOrDefault(c => c.attribute == CNETConstantes.PMS_SETTING_LabelPrinter);
            var postinRoutine = configList.FirstOrDefault(c => c.attribute == CNETConstantes.PMS_SETTING_PostineRoutine);
            var fiscalBillType = configList.FirstOrDefault(c => c.attribute == CNETConstantes.PMS_SETTING_DefaultFiscalBillType);
            var enableJournalization = configList.FirstOrDefault(c => c.attribute == CNETConstantes.PMS_SETTING_EnableJournalization);
            var mattressAmountConfig = configList.FirstOrDefault(c => c.attribute == CNETConstantes.PMS_SETTING_MattressAmount);
            var custHighBalanceConfig = configList.FirstOrDefault(c => c.attribute == CNETConstantes.PMS_SETTING_CustHighBalance);
            var validateReference = configList.FirstOrDefault(c => c.attribute == CNETConstantes.PMS_SETTING_ValidateReference);
            var chargeAtCheckin = configList.FirstOrDefault(c => c.attribute == CNETConstantes.PMS_SETTING_ChargeAtCheckIn);



            PMSSetting pmsSetting = new PMSSetting();
            pmsSetting.CheckInTime = checkinTimeConfig == null ? new DateTime() : DateTime.ParseExact(checkinTimeConfig.currentValue, "HH:mm", CultureInfo.InvariantCulture);
            pmsSetting.CheckOutTime = checkoutTimeConfig == null ? new DateTime() : DateTime.ParseExact(checkoutTimeConfig.currentValue, "HH:mm", CultureInfo.InvariantCulture);
            pmsSetting.LateCheckoutTime = lateCheckoutTimeConfig == null ? new DateTime() : DateTime.ParseExact(lateCheckoutTimeConfig.currentValue, "HH:mm", CultureInfo.InvariantCulture);
            pmsSetting.NightAuditTime = nightAuditTimeConfig == null ? new DateTime() : DateTime.ParseExact(nightAuditTimeConfig.currentValue, "HH:mm", CultureInfo.InvariantCulture);
            pmsSetting.UndoCheckinTime = undoCheckinMinConfig == null ? 0 : Convert.ToInt32(undoCheckinMinConfig.currentValue);
            pmsSetting.MinimumRateAdujstment = minRateAdjConfig == null ? 0 : Convert.ToDecimal(minRateAdjConfig.currentValue);
            pmsSetting.MattressAmount = mattressAmountConfig == null ? 0 : Convert.ToDecimal(mattressAmountConfig.currentValue);
            pmsSetting.ArchivePath = archivePathConfig == null ? "" : archivePathConfig.currentValue;
            pmsSetting.ArchivePrint = archivePrintConfig == null ? false: Convert.ToBoolean(archivePrintConfig.currentValue);
            pmsSetting.UseLateCheckout = useLateCheckout == null ? false : Convert.ToBoolean(useLateCheckout.currentValue);
            pmsSetting.EnforceCardReturn = enforceCheckoutCardReturn == null ? false : Convert.ToBoolean(enforceCheckoutCardReturn.currentValue);
            pmsSetting.LostFeeArticle = lostCardArticleCode == null ? "" : lostCardArticleCode.currentValue;
            pmsSetting.LateCheckoutRequiredPayment = lateCheckoutReqPaymentConfig == null ? 0 : Convert.ToDouble(lateCheckoutReqPaymentConfig.currentValue);
            pmsSetting.LateCheckoutAdditionalPayment = lateCheckoutAdditionPaymentConfig == null ? 0 : Convert.ToDouble(lateCheckoutAdditionPaymentConfig.currentValue);
            pmsSetting.LabelDesignFile = labelDesignFile == null ? "" : labelDesignFile.currentValue;
            pmsSetting.LabelPrinter = labelPrinter == null ? "" : labelPrinter.currentValue;
            pmsSetting.EnableJournalize = enableJournalization == null ? false : Convert.ToBoolean(enableJournalization.currentValue);
            pmsSetting.PostineRoutine = postinRoutine == null ? "" : postinRoutine.currentValue;
            pmsSetting.DefaultFiscalBillType = fiscalBillType == null ? "" : fiscalBillType.currentValue;
            pmsSetting.CustomerHighBalance = custHighBalanceConfig == null ? 0 : Convert.ToDecimal(custHighBalanceConfig.currentValue);
            pmsSetting.ValidateExternalReference = validateReference == null ? false : Convert.ToBoolean(validateReference.currentValue);
            pmsSetting.ChargeAtCheckin = chargeAtCheckin == null ? false : Convert.ToBoolean(chargeAtCheckin.currentValue);

            return pmsSetting;
        }

        public void SaveData(string attribute, string currentValue)
        {
            if (string.IsNullOrEmpty(attribute)) return;
            Configuration configuration = new Configuration();
            Configuration Prev = LocalBuffer.ConfigurationBufferList.Where(x => x.attribute.Trim() == attribute.Trim()).FirstOrDefault();

            configuration.code = "";
            configuration.preference = "PMS";
            configuration.reference = CNETConstantes.PMS;
            configuration.attribute = attribute;
            configuration.currentValue = currentValue;
            if (Prev != null)
            {
                configuration.previousValue = Prev.currentValue;
                configuration.code = Prev.code;
                UIProcessManager.UpdateConfiguration(new List<Configuration>(){configuration});
            }
            else
            {
                configuration.previousValue = currentValue;
                UIProcessManager.CreateConfiguration(new List<Configuration>() { configuration });
            }
            
            
        }


        private void DisplayPopupForm(string type)
        {

            try
            {
                if (lPrinterForm == null)
                {
                    lPrinterForm = new XtraForm();
                    mbutton = new Button();
                    mList = new ListBox();
                    this.lPrinterCancelBtn = new Button();
                    this.tbox = new TextBox();
                }
                if (!lPrinterForm.IsDisposed)
                {

                    lPrinterForm.Close();
                    lPrinterForm = new XtraForm();
                    mbutton = new Button();
                    mList = new ListBox();
                    this.lPrinterCancelBtn = new Button();
                    this.tbox = new TextBox();
                }
                mbutton.Text = "Ok";
                mbutton.Location = new Point(40, 230);
                this.lPrinterCancelBtn.Text = "Cancel";
                this.lPrinterCancelBtn.Location = new Point(160, 230);
                mList.Items.Clear();
                switch (type)
                {
                    case "rowLabelPrinter":

                        foreach (string printer in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
                        {
                            mList.Items.Add(printer);
                        }
                        lPrinterForm.Text = "Lable  Printer";
                        break;
                }
                mList.Size = (Size)(new Point(250, 190));
                mList.Location = new Point(20, 15);

                lPrinterForm.Controls.Add(mList);

                lPrinterForm.Controls.Add(this.mbutton);
                lPrinterForm.Controls.Add(this.lPrinterCancelBtn);
                mbutton.Click += labelPrinterOk_Click;
                lPrinterCancelBtn.Click += lPrinterCancelBtn_Click;
                //mForm.Visible = true;
                lPrinterForm.ControlBox = false;
                // mForm.BringToFront();
                //mForm.MdiParent = this;
                //mForm.LayoutMdi = this;
                lPrinterForm.StartPosition = FormStartPosition.CenterScreen;
                lPrinterForm.TopMost = true;
                lPrinterForm.Show();
                lPrinterForm.Activate();

            }
            catch (Exception ex)
            {
            }
        }

        


        private void DisplayForm(string stype)
        {

            try
            {
                
                this.mButtonCancel.Text = "Cancel";
                this.mButtonCancel.Location = new Point(160, 30);
                sLuk_article.Properties.DataSource = null;
                switch (stype)
                {

                    case "rowLostFeeArticle":
                        sLuk_article.Properties.DataSource = articles;
                        //mForm.Text = "Lost Card Fee Article";
                        break;

                }
                if (mForm.IsDisposed)
                {
                    mForm = new XtraForm();
                    mForm.Size = new System.Drawing.Size(0, 0);
                    mForm.Controls.Add(sLuk_article);
                    mForm.LostFocus += mForm_LostFocus;
                    // mForm.Controls.Add(this.mButtonCancel);
                    mButtonCancel.Click += new System.EventHandler(this.mButtonCancel_Click);
                    mForm.ControlBox = false;
                    mForm.ShowIcon = false;
                    mForm.ControlBox = false;
                    mForm.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                    mForm.StartPosition = FormStartPosition.WindowsDefaultLocation;
                    mForm.TopMost = true;
                }

                
                mForm.Show();
                mForm.Activate();
                sLuk_article.ShowPopup();

            }
            catch (Exception ex)
            {
            }
        }

        void mForm_LostFocus(object sender, EventArgs e)
        {
           
            //if (!sLuk_article.Focused)
            //{
            //    sLuk_article.Hide();
            //}
            //sLuk_article.Hide();
            //mForm.Hide();
        }

        #endregion


        #region Event Handlers 
        private void bbiSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                pmsSetting = (PMSSetting)gcProperty.SelectedObject;
                if (pmsSetting == null)
                {
                    SystemMessage.ShowModalInfoMessage("Setting is not saved!", "ERROR");
                    return;
                }

                CNETInfoReporter.WaitForm("Saving PMS Settings", "Please Wait...");

                //save attiributes
                SaveData(CNETConstantes.PMS_SETTING_CheckInTime, pmsSetting.CheckInTime.ToString("HH:mm"));
                SaveData(CNETConstantes.PMS_SETTING_CheckOutTime, pmsSetting.CheckOutTime.ToString("HH:mm"));
                SaveData(CNETConstantes.PMS_SETTING_LateCheckoutTime, pmsSetting.LateCheckoutTime.ToString("HH:mm"));
                SaveData(CNETConstantes.PMS_SETTING_NightAuditTime, pmsSetting.NightAuditTime.ToString("HH:mm"));
                SaveData(CNETConstantes.PMS_SETTING_UndoCheckinMin, pmsSetting.UndoCheckinTime.ToString());
                SaveData(CNETConstantes.PMS_SETTING_MinRateAdjustment, pmsSetting.MinimumRateAdujstment.ToString());
                SaveData(CNETConstantes.PMS_SETTING_MattressAmount, pmsSetting.MattressAmount.ToString());
                SaveData(CNETConstantes.PMS_SETTING_ArchivePath, pmsSetting.ArchivePath);
                SaveData(CNETConstantes.PMS_SETTING_UseLateCheckout, pmsSetting.UseLateCheckout.ToString());
                SaveData(CNETConstantes.PMS_SETTING_ArchivePrint, pmsSetting.ArchivePrint.ToString());
                SaveData(CNETConstantes.PMS_SETTING_LateCheckoutRequiredPayment, pmsSetting.LateCheckoutRequiredPayment.ToString());
                SaveData(CNETConstantes.PMS_SETTING_LateCheckoutAdditionPayment, pmsSetting.LateCheckoutAdditionalPayment.ToString());
                SaveData(CNETConstantes.PMS_SETTING_EnforceCheckoutCardReturn, pmsSetting.EnforceCardReturn.ToString());
                SaveData(CNETConstantes.PMS_SETTING_LostCardFeeArticle, pmsSetting.LostFeeArticle);
                SaveData(CNETConstantes.PMS_SETTING_LabelDesignFile, pmsSetting.LabelDesignFile);
                SaveData(CNETConstantes.PMS_SETTING_LabelPrinter, pmsSetting.LabelPrinter);
                SaveData(CNETConstantes.PMS_SETTING_EnableJournalization, pmsSetting.EnableJournalize.ToString());
                SaveData(CNETConstantes.PMS_SETTING_PostineRoutine, pmsSetting.PostineRoutine);
                SaveData(CNETConstantes.PMS_SETTING_DefaultFiscalBillType, pmsSetting.DefaultFiscalBillType);
                SaveData(CNETConstantes.PMS_SETTING_CustHighBalance, pmsSetting.CustomerHighBalance.ToString());
                SaveData(CNETConstantes.PMS_SETTING_ValidateReference, pmsSetting.ValidateExternalReference.ToString());
                SaveData(CNETConstantes.PMS_SETTING_ChargeAtCheckIn, pmsSetting.ChargeAtCheckin.ToString());


                //update login buffer
                LocalBuffer.LoadConfigurations();

                CNETInfoReporter.Hide();
                SystemMessage.ShowModalInfoMessage("Settings are Saved!", "MESSAGE");

                

            }
            catch (Exception ex)
            {
                CNETInfoReporter.Hide();
                SystemMessage.ShowModalInfoMessage("Error in saving pms setting. Detail:: " + ex.Message, "ERROR");

            }
        }

        private void sLuk_article_EditValueChanged(object sender, EventArgs e)
        {

            if (sLuk_article.EditValue != null && !string.IsNullOrWhiteSpace(sLuk_article.EditValue.ToString()))
            {
                string articleCode = sLuk_article.EditValue.ToString();
                if (selectedAttribute == "rowLostFeeArticle")
                {
                    pmsSetting.LostFeeArticle = articleCode;
                    gcProperty.Refresh();
                    gcProperty.ShowEditor();
                    gcProperty.CloseEditor();
                    gcProperty.SelectedObject = pmsSetting;
                }

                mForm.Hide();
                
            }
        }

        private void mButtonCancel_Click(object sender, EventArgs e)
        {
            if (mForm != null)
            {
                mForm.Close();
            }
        }

        private void lPrinterCancelBtn_Click(object sender, EventArgs e)
        {
            if (lPrinterForm != null)
            {
                lPrinterForm.Hide();
                // lPrinterForm.Dispose();
            }
        }

        private void labelPrinterOk_Click(object sender, EventArgs e)
        {
            if (mList.Items != null && mList.Items.Count > 0)
            {
                pmsSetting.LabelPrinter = mList.SelectedItem as string;
                gcProperty.Refresh();
                gcProperty.ShowEditor();
                gcProperty.CloseEditor();
                gcProperty.SelectedObject = pmsSetting;

                lPrinterForm.Hide();
                
            }
        }
      
        private void gcProperty_CellValueChanging(object sender, DevExpress.XtraVerticalGrid.Events.CellValueChangedEventArgs e)
       {
            PropertyGridControl view = sender as PropertyGridControl;
            view.SetCellValue(e.Row, e.RecordIndex, e.Value);
        }

        private void gcProperty_FocusedRowChanged(object sender, DevExpress.XtraVerticalGrid.Events.FocusedRowChangedEventArgs e)
        {
            try
            {
                if (e.Row != null)
                {
                    selectedAttribute = e.Row.Name;
                    if (e.Row.Name == "rowLostFeeArticle")
                    {
                        DisplayForm(e.Row.Name);
                    }
                    else if (e.Row.Name == "rowLabelPrinter")
                    {
                        DisplayPopupForm(e.Row.Name);
                    }
                    else
                    {

                        mForm.Hide();
                        sLuk_article.Hide();
                    }
                }

            }
            catch (Exception ex) { }


        }

        private void bbiPrint_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ReportGenerator rg = new ReportGenerator();
            DateTime? currentTime = CommonLogics.GetServiceTime();
            if (currentTime != null)
            {
                rg.GenerateGridReportPotrait(gcProperty, "PMS Setting", currentTime.Value.ToShortDateString());
            }
        }

        #endregion

        private void rcPMSSetting_Click(object sender, EventArgs e)
        {
            mForm.Hide();
            sLuk_article.Hide();
        }

        private void gcProperty_Click(object sender, EventArgs e)
        {
            //try
            //{
            //    PropertyGridControl view = sender as PropertyGridControl;
            //    if (view == null) return;
            //    var row = view.FocusedRow;
            //    if (row == null)
            //    {
            //        mForm.Hide();
            //        sLuk_article.Hide();
            //        return;
            //    }
            //    selectedAttribute = row.Name;
            //    if (row.Name != "rowLostFeeArticle")
            //    {
            //        mForm.Hide();
            //        sLuk_article.Hide();
            //    }

            //}
            //catch (Exception ex) { }
        }

        private void propertyDescriptionControl1_Click(object sender, EventArgs e)
        {
            mForm.Hide();
            sLuk_article.Hide();
        }


        private void gcProperty_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                PropertyGridControl view = sender as PropertyGridControl;
                if (view == null) return;
                VGridHitInfo hitInfo =  view.CalcHitInfo(e.Location);
                if (hitInfo != null)
                {
                    if (hitInfo.HitInfoType != HitInfoTypeEnum.ValueCell)
                    {
                        mForm.Hide();
                        sLuk_article.Hide();
                    }
                    else
                    {
                        var row = view.FocusedRow;
                        selectedAttribute = row.Name;
                        if (row == null)
                        {
                            return;
                        }
                        if (row.Name != "rowLostFeeArticle")
                        {
                            mForm.Hide();
                            sLuk_article.Hide();
                        }
                        else
                        {
                            DisplayForm(row.Name);
                        }
                    }
                }
            }
            catch (Exception ex) { }
        }
        

        

        


    }
}
