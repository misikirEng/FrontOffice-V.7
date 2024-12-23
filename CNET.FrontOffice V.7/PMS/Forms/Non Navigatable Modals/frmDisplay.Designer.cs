namespace CNET.FrontOffice_V._7.Forms.Non_Navigatable_Modals
{
    partial class frmDisplay
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmDisplay));
            this.winPlayerDisplay = new frmDisplay.MyPlayer();
            ((System.ComponentModel.ISupportInitialize)(this.winPlayerDisplay)).BeginInit();
            this.SuspendLayout();
            // 
            // winPlayerDisplay
            // 
            this.winPlayerDisplay.Dock = System.Windows.Forms.DockStyle.Fill;
            this.winPlayerDisplay.Enabled = true;
            this.winPlayerDisplay.Location = new System.Drawing.Point(0, 0);
            this.winPlayerDisplay.Name = "winPlayerDisplay";
            this.winPlayerDisplay.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("winPlayerDisplay.OcxState")));
            this.winPlayerDisplay.Size = new System.Drawing.Size(617, 476);
            this.winPlayerDisplay.TabIndex = 4;
            // 
            // frmDisplay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(617, 476);
            this.ControlBox = false;
            this.Controls.Add(this.winPlayerDisplay);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimizeBox = false;
            this.Name = "frmDisplay";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "frmDisplay";
            this.Load += new System.EventHandler(this.frmDisplay_Load);
            ((System.ComponentModel.ISupportInitialize)(this.winPlayerDisplay)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private frmDisplay.MyPlayer winPlayerDisplay;



    }
}