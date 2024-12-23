namespace CNET.FrontOffice_V._7
{
    partial class CNETWaitForm
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
            this.ppMin = new DevExpress.XtraWaitForm.ProgressPanel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.pbcProgress = new DevExpress.XtraEditors.ProgressBarControl();
            this.pbcChild = new DevExpress.XtraEditors.ProgressBarControl();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbcProgress.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbcChild.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // ppMin
            // 
            this.ppMin.Appearance.BackColor = System.Drawing.Color.Transparent;
            this.ppMin.Appearance.Options.UseBackColor = true;
            this.ppMin.AppearanceCaption.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.ppMin.AppearanceCaption.Options.UseFont = true;
            this.ppMin.AppearanceDescription.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.ppMin.AppearanceDescription.Options.UseFont = true;
            this.ppMin.Dock = System.Windows.Forms.DockStyle.Top;
            this.ppMin.ImageHorzOffset = 20;
            this.ppMin.Location = new System.Drawing.Point(18, 18);
            this.ppMin.LookAndFeel.SkinName = "Office 2013 Dark Gray";
            this.ppMin.LookAndFeel.UseDefaultLookAndFeel = false;
            this.ppMin.Margin = new System.Windows.Forms.Padding(18);
            this.ppMin.Name = "ppMin";
            this.ppMin.Size = new System.Drawing.Size(317, 77);
            this.ppMin.TabIndex = 0;
            this.ppMin.Text = "progressPanel1";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.BackColor = System.Drawing.Color.Transparent;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.ppMin, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.pbcProgress, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.pbcChild, 0, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(353, 239);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // pbcProgress
            // 
            this.pbcProgress.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pbcProgress.Location = new System.Drawing.Point(3, 116);
            this.pbcProgress.Name = "pbcProgress";
            this.pbcProgress.Properties.AllowFocused = false;
            this.pbcProgress.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Office2003;
            this.pbcProgress.Properties.LookAndFeel.SkinName = "Metropolis";
            this.pbcProgress.Properties.LookAndFeel.UseDefaultLookAndFeel = false;
            this.pbcProgress.Properties.ProgressViewStyle = DevExpress.XtraEditors.Controls.ProgressViewStyle.Solid;
            this.pbcProgress.Properties.ShowTitle = true;
            this.pbcProgress.ShowProgressInTaskBar = true;
            this.pbcProgress.Size = new System.Drawing.Size(347, 17);
            this.pbcProgress.TabIndex = 2;
            this.pbcProgress.UseWaitCursor = true;
            this.pbcProgress.Visible = false;
            // 
            // pbcChild
            // 
            this.pbcChild.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pbcChild.Location = new System.Drawing.Point(3, 216);
            this.pbcChild.Name = "pbcChild";
            this.pbcChild.Properties.AllowFocused = false;
            this.pbcChild.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            this.pbcChild.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Flat;
            this.pbcChild.Properties.LookAndFeel.SkinName = "Metropolis";
            this.pbcChild.Properties.LookAndFeel.UseDefaultLookAndFeel = false;
            this.pbcChild.Properties.NullText = "wds";
            this.pbcChild.Properties.PercentView = false;
            this.pbcChild.Properties.ProgressViewStyle = DevExpress.XtraEditors.Controls.ProgressViewStyle.Solid;
            this.pbcChild.Properties.ShowTitle = true;
            this.pbcChild.ShowProgressInTaskBar = true;
            this.pbcChild.Size = new System.Drawing.Size(347, 20);
            this.pbcChild.TabIndex = 3;
            this.pbcChild.UseWaitCursor = true;
            this.pbcChild.Visible = false;
            this.pbcChild.CustomDisplayText += new DevExpress.XtraEditors.Controls.CustomDisplayTextEventHandler(this.pbcChild_CustomDisplayText);
            // 
            // CNETWaitForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(353, 239);
            this.Controls.Add(this.tableLayoutPanel1);
            this.DoubleBuffered = true;
            this.Name = "CNETWaitForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.CNETWaitForm_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbcProgress.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbcChild.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraWaitForm.ProgressPanel ppMin;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private DevExpress.XtraEditors.ProgressBarControl pbcProgress;
        private DevExpress.XtraEditors.ProgressBarControl pbcChild;
    }
}
