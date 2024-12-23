
namespace CNET.FrontOffice_V._7.Forms.Non_Navigatable_Modals
{
    partial class frmReconciliation
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmReconciliation));
            checkBox1 = new CheckBox();
            statusStrip1 = new StatusStrip();
            gc_recon = new DevExpress.XtraGrid.GridControl();
            gv_recon = new DevExpress.XtraGrid.Views.Grid.GridView();
            gCol_voucher = new DevExpress.XtraGrid.Columns.GridColumn();
            gcol_description = new DevExpress.XtraGrid.Columns.GridColumn();
            gCol_date = new DevExpress.XtraGrid.Columns.GridColumn();
            gCol_weekDay = new DevExpress.XtraGrid.Columns.GridColumn();
            gCol_debit = new DevExpress.XtraGrid.Columns.GridColumn();
            gcol_credit = new DevExpress.XtraGrid.Columns.GridColumn();
            gCol_balance = new DevExpress.XtraGrid.Columns.GridColumn();
            ((System.ComponentModel.ISupportInitialize)gc_recon).BeginInit();
            ((System.ComponentModel.ISupportInitialize)gv_recon).BeginInit();
            SuspendLayout();
            // 
            // checkBox1
            // 
            checkBox1.AutoSize = true;
            checkBox1.Location = new Point(573, 429);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new Size(77, 17);
            checkBox1.TabIndex = 0;
            checkBox1.Text = "checkBox1";
            checkBox1.UseVisualStyleBackColor = true;
            // 
            // statusStrip1
            // 
            statusStrip1.Location = new Point(3, 425);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(891, 22);
            statusStrip1.SizingGrip = false;
            statusStrip1.TabIndex = 2;
            statusStrip1.Text = "statusStrip1";
            // 
            // gc_recon
            // 
            gc_recon.Dock = DockStyle.Fill;
            gc_recon.Location = new Point(3, 3);
            gc_recon.MainView = gv_recon;
            gc_recon.Name = "gc_recon";
            gc_recon.Size = new Size(891, 422);
            gc_recon.TabIndex = 3;
            gc_recon.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { gv_recon });
            // 
            // gv_recon
            // 
            gv_recon.Appearance.FocusedCell.BackColor = SystemColors.Highlight;
            gv_recon.Appearance.FocusedCell.ForeColor = Color.White;
            gv_recon.Appearance.FocusedCell.Options.UseBackColor = true;
            gv_recon.Appearance.FocusedCell.Options.UseForeColor = true;
            gv_recon.Appearance.FocusedRow.BackColor = SystemColors.Highlight;
            gv_recon.Appearance.FocusedRow.ForeColor = Color.White;
            gv_recon.Appearance.FocusedRow.Options.UseBackColor = true;
            gv_recon.Appearance.FocusedRow.Options.UseForeColor = true;
            gv_recon.Appearance.OddRow.BackColor = SystemColors.ControlLight;
            gv_recon.Appearance.OddRow.Options.UseBackColor = true;
            gv_recon.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] { gCol_voucher, gcol_description, gCol_date, gCol_weekDay, gCol_debit, gcol_credit, gCol_balance });
            gv_recon.GridControl = gc_recon;
            gv_recon.Name = "gv_recon";
            gv_recon.OptionsBehavior.ReadOnly = true;
            gv_recon.OptionsSelection.EnableAppearanceFocusedCell = false;
            gv_recon.OptionsSelection.EnableAppearanceHideSelection = false;
            gv_recon.OptionsView.EnableAppearanceOddRow = true;
            gv_recon.OptionsView.ShowGroupPanel = false;
            gv_recon.OptionsView.ShowIndicator = false;
            // 
            // gCol_voucher
            // 
            gCol_voucher.AppearanceHeader.Font = new Font("Tahoma", 8.25F, FontStyle.Bold, GraphicsUnit.Point);
            gCol_voucher.AppearanceHeader.Options.UseFont = true;
            gCol_voucher.Caption = "Voucher";
            gCol_voucher.FieldName = "voucherCode";
            gCol_voucher.Name = "gCol_voucher";
            gCol_voucher.Visible = true;
            gCol_voucher.VisibleIndex = 0;
            // 
            // gcol_description
            // 
            gcol_description.AppearanceHeader.Font = new Font("Tahoma", 8.25F, FontStyle.Bold, GraphicsUnit.Point);
            gcol_description.AppearanceHeader.Options.UseFont = true;
            gcol_description.Caption = "Description";
            gcol_description.FieldName = "Description";
            gcol_description.Name = "gcol_description";
            gcol_description.Visible = true;
            gcol_description.VisibleIndex = 1;
            // 
            // gCol_date
            // 
            gCol_date.AppearanceHeader.Font = new Font("Tahoma", 8.25F, FontStyle.Bold, GraphicsUnit.Point);
            gCol_date.AppearanceHeader.Options.UseFont = true;
            gCol_date.Caption = "Date";
            gCol_date.FieldName = "date";
            gCol_date.Name = "gCol_date";
            gCol_date.Visible = true;
            gCol_date.VisibleIndex = 2;
            // 
            // gCol_weekDay
            // 
            gCol_weekDay.AppearanceHeader.Font = new Font("Tahoma", 8.25F, FontStyle.Bold, GraphicsUnit.Point);
            gCol_weekDay.AppearanceHeader.Options.UseFont = true;
            gCol_weekDay.Caption = "Week Day";
            gCol_weekDay.FieldName = "weekDay";
            gCol_weekDay.Name = "gCol_weekDay";
            gCol_weekDay.Visible = true;
            gCol_weekDay.VisibleIndex = 3;
            // 
            // gCol_debit
            // 
            gCol_debit.AppearanceCell.Options.UseTextOptions = true;
            gCol_debit.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            gCol_debit.AppearanceHeader.Font = new Font("Tahoma", 8.25F, FontStyle.Bold, GraphicsUnit.Point);
            gCol_debit.AppearanceHeader.Options.UseFont = true;
            gCol_debit.Caption = "Debit";
            gCol_debit.FieldName = "debit";
            gCol_debit.Name = "gCol_debit";
            gCol_debit.Visible = true;
            gCol_debit.VisibleIndex = 4;
            // 
            // gcol_credit
            // 
            gcol_credit.AppearanceCell.Options.UseTextOptions = true;
            gcol_credit.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            gcol_credit.AppearanceHeader.Font = new Font("Tahoma", 8.25F, FontStyle.Bold, GraphicsUnit.Point);
            gcol_credit.AppearanceHeader.Options.UseFont = true;
            gcol_credit.Caption = "Credit";
            gcol_credit.FieldName = "credit";
            gcol_credit.Name = "gcol_credit";
            gcol_credit.Visible = true;
            gcol_credit.VisibleIndex = 5;
            // 
            // gCol_balance
            // 
            gCol_balance.AppearanceCell.Options.UseTextOptions = true;
            gCol_balance.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            gCol_balance.AppearanceHeader.Font = new Font("Tahoma", 8.25F, FontStyle.Bold, GraphicsUnit.Point);
            gCol_balance.AppearanceHeader.Options.UseFont = true;
            gCol_balance.Caption = "Balance";
            gCol_balance.FieldName = "balance";
            gCol_balance.Name = "gCol_balance";
            gCol_balance.Visible = true;
            gCol_balance.VisibleIndex = 6;
            // 
            // frmReconciliation
            // 
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(897, 450);
            Controls.Add(gc_recon);
            Controls.Add(statusStrip1);
            Controls.Add(checkBox1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            IconOptions.Icon = (Icon)resources.GetObject("frmReconciliation.IconOptions.Icon");
            IconOptions.ShowIcon = false;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "frmReconciliation";
            Padding = new Padding(3);
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Reconciliation";
            ((System.ComponentModel.ISupportInitialize)gc_recon).EndInit();
            ((System.ComponentModel.ISupportInitialize)gv_recon).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private CheckBox checkBox1;
        private StatusStrip statusStrip1;
        private DevExpress.XtraGrid.GridControl gc_recon;
        private DevExpress.XtraGrid.Views.Grid.GridView gv_recon;
        private DevExpress.XtraGrid.Columns.GridColumn gCol_voucher;
        private DevExpress.XtraGrid.Columns.GridColumn gcol_description;
        private DevExpress.XtraGrid.Columns.GridColumn gCol_date;
        private DevExpress.XtraGrid.Columns.GridColumn gCol_weekDay;
        private DevExpress.XtraGrid.Columns.GridColumn gCol_debit;
        private DevExpress.XtraGrid.Columns.GridColumn gcol_credit;
        private DevExpress.XtraGrid.Columns.GridColumn gCol_balance;
    }
}