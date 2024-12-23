using CNET.FrontOffice_V._7.Night_Audit;

namespace CNET.FrontOffice_V._7
{
    partial class LicenseAlert
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
            lblMessage = new DevExpress.XtraEditors.LabelControl();
            pictureBox1 = new PictureBox();
            panelControl1 = new DevExpress.XtraEditors.PanelControl();
            cbtMore = new DevExpress.XtraEditors.CheckButton();
            gCtrlLicenseAlert = new DevExpress.XtraGrid.GridControl();
            gvwLicenseAlert = new DevExpress.XtraGrid.Views.Grid.GridView();
            Subsystem = new DevExpress.XtraGrid.Columns.GridColumn();
            DayesLeft = new DevExpress.XtraGrid.Columns.GridColumn();
            gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)panelControl1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)gCtrlLicenseAlert).BeginInit();
            ((System.ComponentModel.ISupportInitialize)gvwLicenseAlert).BeginInit();
            SuspendLayout();
            // 
            // lblMessage
            // 
            lblMessage.Appearance.Font = new Font("Tahoma", 10F, FontStyle.Regular, GraphicsUnit.Point);
            lblMessage.Appearance.Options.UseFont = true;
            lblMessage.Appearance.Options.UseTextOptions = true;
            lblMessage.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            lblMessage.Location = new Point(76, 14);
            lblMessage.Name = "lblMessage";
            lblMessage.Size = new Size(275, 32);
            lblMessage.TabIndex = 0;
            lblMessage.Text = "Some of your licenses are expired/invalid or will\r\nexpire in a few days!";
            // 
            // pictureBox1
            // 
            pictureBox1.Image = Properties.Resources.warning_attention;
            pictureBox1.Location = new Point(5, 4);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(65, 52);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 1;
            pictureBox1.TabStop = false;
            // 
            // panelControl1
            // 
            panelControl1.Appearance.BackColor = Color.Transparent;
            panelControl1.Appearance.BorderColor = Color.Silver;
            panelControl1.Appearance.Options.UseBackColor = true;
            panelControl1.Appearance.Options.UseBorderColor = true;
            panelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            panelControl1.Location = new Point(3, 62);
            panelControl1.Name = "panelControl1";
            panelControl1.Size = new Size(412, 2);
            panelControl1.TabIndex = 3;
            // 
            // cbtMore
            // 
            cbtMore.Appearance.BorderColor = Color.Silver;
            cbtMore.Appearance.Options.UseBorderColor = true;
            cbtMore.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            cbtMore.Checked = true;
            cbtMore.Location = new Point(324, 52);
            cbtMore.Name = "cbtMore";
            cbtMore.Size = new Size(165, 18);
            cbtMore.TabIndex = 4;
            cbtMore.Text = "Less";
            cbtMore.CheckedChanged += cbtMore_CheckedChanged;
            // 
            // gCtrlLicenseAlert
            // 
            gCtrlLicenseAlert.Location = new Point(6, 79);
            gCtrlLicenseAlert.MainView = gvwLicenseAlert;
            gCtrlLicenseAlert.Name = "gCtrlLicenseAlert";
            gCtrlLicenseAlert.Size = new Size(483, 114);
            gCtrlLicenseAlert.TabIndex = 5;
            gCtrlLicenseAlert.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { gvwLicenseAlert });
            // 
            // gvwLicenseAlert
            // 
            gvwLicenseAlert.Appearance.HeaderPanel.Font = new Font("Tahoma", 8.25F, FontStyle.Bold, GraphicsUnit.Point);
            gvwLicenseAlert.Appearance.HeaderPanel.Options.UseFont = true;
            gvwLicenseAlert.Appearance.HeaderPanel.Options.UseTextOptions = true;
            gvwLicenseAlert.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            gvwLicenseAlert.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] { Subsystem, DayesLeft, gridColumn1 });
            gvwLicenseAlert.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            gvwLicenseAlert.GridControl = gCtrlLicenseAlert;
            gvwLicenseAlert.Name = "gvwLicenseAlert";
            gvwLicenseAlert.OptionsBehavior.Editable = false;
            gvwLicenseAlert.OptionsBehavior.ReadOnly = true;
            gvwLicenseAlert.OptionsCustomization.AllowColumnMoving = false;
            gvwLicenseAlert.OptionsCustomization.AllowColumnResizing = false;
            gvwLicenseAlert.OptionsCustomization.AllowFilter = false;
            gvwLicenseAlert.OptionsCustomization.AllowGroup = false;
            gvwLicenseAlert.OptionsCustomization.AllowQuickHideColumns = false;
            gvwLicenseAlert.OptionsSelection.EnableAppearanceFocusedCell = false;
            gvwLicenseAlert.OptionsSelection.EnableAppearanceFocusedRow = false;
            gvwLicenseAlert.OptionsView.ShowGroupPanel = false;
            gvwLicenseAlert.OptionsView.ShowIndicator = false;
            gvwLicenseAlert.RowStyle += gvwLicenseAlert_RowStyle;
            // 
            // Subsystem
            // 
            Subsystem.AppearanceHeader.Options.UseTextOptions = true;
            Subsystem.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            Subsystem.Caption = "Subsystem";
            Subsystem.FieldName = "subsystem";
            Subsystem.Name = "Subsystem";
            Subsystem.Visible = true;
            Subsystem.VisibleIndex = 0;
            Subsystem.Width = 200;
            // 
            // DayesLeft
            // 
            DayesLeft.Caption = "Days Left";
            DayesLeft.FieldName = "days";
            DayesLeft.Name = "DayesLeft";
            DayesLeft.OptionsColumn.FixedWidth = true;
            DayesLeft.Visible = true;
            DayesLeft.VisibleIndex = 1;
            DayesLeft.Width = 80;
            // 
            // gridColumn1
            // 
            gridColumn1.Caption = "Message";
            gridColumn1.FieldName = "Message";
            gridColumn1.Name = "gridColumn1";
            gridColumn1.Visible = true;
            gridColumn1.VisibleIndex = 2;
            gridColumn1.Width = 150;
            // 
            // LicenseAlert
            // 
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            ClientSize = new Size(497, 210);
            Controls.Add(gCtrlLicenseAlert);
            Controls.Add(cbtMore);
            Controls.Add(panelControl1);
            Controls.Add(pictureBox1);
            Controls.Add(lblMessage);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimumSize = new Size(336, 0);
            Name = "LicenseAlert";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "CNET ERP";
            Load += LicenseAlert_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)panelControl1).EndInit();
            ((System.ComponentModel.ISupportInitialize)gCtrlLicenseAlert).EndInit();
            ((System.ComponentModel.ISupportInitialize)gvwLicenseAlert).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DevExpress.XtraEditors.LabelControl lblMessage;
        private PictureBox pictureBox1;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.CheckButton cbtMore;
        private DevExpress.XtraGrid.GridControl gCtrlLicenseAlert;
        private DevExpress.XtraGrid.Views.Grid.GridView gvwLicenseAlert;
        private DevExpress.XtraGrid.Columns.GridColumn Subsystem;
        private DevExpress.XtraGrid.Columns.GridColumn DayesLeft;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
    }
}