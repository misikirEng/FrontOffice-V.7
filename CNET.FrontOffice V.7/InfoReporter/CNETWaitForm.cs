using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraWaitForm;

namespace CNET.FrontOffice_V._7
{
    public partial class CNETWaitForm : WaitForm
    {
        public CNETWaitForm()
        {
            InitializeComponent();
            this.ppMin.AutoHeight = true;
        }

        #region Overrides

        public override void SetCaption(string caption)
        {
            base.SetCaption(caption);
            this.ppMin.Caption = caption;
        }
        public override void SetDescription(string description)
        {
            base.SetDescription(description);
            this.ppMin.Description = description;
        }
        public override void ProcessCommand(Enum cmd, object arg)
        {
            WaitFormCommand comand = (WaitFormCommand)cmd;

            if (comand == WaitFormCommand.ProgressValue)
            {
                if (pbcProgress.Visible == false)
                    pbcProgress.Visible = true;

                pbcProgress.EditValue = (int)arg;

                if (pbcChild.Visible == true)
                    pbcChild.Visible = false;
            }

            if (comand == WaitFormCommand.Color)
            {
                tableLayoutPanel1.BackColor = (Color)(arg);
            }

            if (comand == WaitFormCommand.ProgressValueChild)
            {

                //   if (pbcChild == null) pbcChild = new DevExpress.XtraEditors.ProgressBarControl();


                pbcChild.EditValue = (int)arg;
                //pbcChild.Text = arg.ToString()+"xxx";

            //    pbcChild.Refresh();

                if (pbcChild.Visible == false)
                    pbcChild.Visible = true;

                return;

            }

            if (comand == WaitFormCommand.ProgressValueChildDescription)
            {
                String value = pbcChild.EditValue.ToString();
                //   pbcChild.EditValue = pbcChild.EditValue.ToString() + "%  Loading value...";
            //    pbcChild.Text = arg.ToString();
                progressBarText = value + "% - " + arg.ToString();

                // ppMin.Description = arg.ToString();

                if (Convert.ToInt16(value) > 99)
                    pbcChild.Visible = false;
                



            }

            if (comand == WaitFormCommand.HideChildProgressPanel)
            {
                pbcChild.Visible = false;
                //    pbcChild.Dispose();
                //    pbcChild = null;

            }

            base.ProcessCommand(cmd, arg);
        }

        #endregion

        public enum WaitFormCommand
        {
            ProgressValue,
            Color,
            ProgressMax,
            ProgressValueChild,
            ProgressValueChildDescription,
            HideChildProgressPanel
        }
        String progressBarText;

        private void pbcChild_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            e.DisplayText = progressBarText;
        }

        private void CNETWaitForm_Load(object sender, EventArgs e)
        {

        }

    }
}