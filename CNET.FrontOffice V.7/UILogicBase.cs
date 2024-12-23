using DevExpress.XtraBars.Ribbon;
using CNET.FrontOffice_V._7.CustomEventArgs;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraBars.Alerter;

namespace CNET.FrontOffice_V._7
{
    public class UILogicBase : XtraForm
    {
        public UILogicBase()
        {

        }



        //public void Close()
        //{
        //    subForm.Close();
        //}


        public void SetIcon(Image image)
        {
            if (subForm != null)
            {
                subForm.Icon = ConvertImageToIcon(image);
                subForm.ShowIcon = true;
            }
        }
        public LoadEventArgs LoadEventArg = new LoadEventArgs();

        private Icon ConvertImageToIcon(Image image)
        {
            return null;
        }

        /// <summary> Determines if we can continue closing. </summary>
        /// <returns> true if it succeeds, false if it fails. </returns>
        protected virtual bool ContinueClosing()
        {
            return true;
        }

        public virtual void Reset()
        {
        }

        public Size FormSize { get; set; }

        public string FormFullName { get; set; }

        public string FormName { get; set; }


        private XtraForm subForm;

        public XtraForm SubForm
        {
            get
            {
                return subForm;
            }
            set
            {
                value.FormClosing += subForm_FormClosing;
                value.Resize += value_Resize;
                subForm = value;
            }
        }

        private void value_Resize(object sender, EventArgs e)
        {
            currentFormSize = subForm.Size;
        }

        private void subForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!ContinueClosing())
            {
                e.Cancel = false;
            }
        }

        private CNETRibbon cNETRibbon = new CNETRibbon();

        public CNETRibbon CNETFooterRibbon
        {
            get
            {
                return cNETRibbon;
            }
            set
            {
                cNETRibbon = value;
            }
        }

        // public static Home Home { get; set; }

        public event EventHandler BeforeInitializationEHandler;
        public event EventHandler AfterInitializationEHandler;
        public event EventHandler BeforeCloseEHandler;
        public event EventHandler BeforeResetEHandler;
        public event EventHandler AfterResetEHandler;

        public SavingEventHandler BeforeSaving { get; set; }
        public SavingEventHandler AfterSaving { get; set; }
        public CreatingEventHandler BeforeCreating { get; set; }
        public CreatingEventHandler AfterCreating { get; set; }
        public DeletingEventHandler BeforeDeleting { get; set; }
        public DeletingEventHandler AfterDeleting { get; set; }

        //private frmNote _frmNote;

        //public frmNote NoteForm
        //{
        //    get
        //    {
        //        if (_frmNote == null)
        //        {
        //            _frmNote = new frmNote();
        //        }
        //        return _frmNote;
        //    }
        //    set
        //    {
        //        _frmNote = value;
        //    }
        //}
        public string NoteContent = string.Empty;
        public string CommentContent = string.Empty;
        // public Person person = new Person();
        public void ShowNote()
        {
            //NoteForm.StartPosition = FormStartPosition.CenterScreen;

            //if (NoteForm.ShowDialog(subForm) == DialogResult.OK)
            //{
            //    NoteContent = NoteForm.NoteContent;
            //}
            //else
            //{
            //    NoteContent = string.Empty;
            //}
        }

        public void ShowComment()
        {
            //NoteForm.StartPosition = FormStartPosition.CenterScreen;

            //NoteForm.Text = "Comments";

            //if (NoteForm.ShowDialog(subForm) == DialogResult.OK)
            //{
            //    CommentContent = NoteForm.NoteContent;
            //}
            //else
            //{
            //    CommentContent = string.Empty;
            //}
        }

        private AlertControl alerter = null;
        private void CreateAlertControl()
        {
            if (alerter != null)
            {
                return;
            }
            alerter = new AlertControl();
            alerter.AutoFormDelay = 3000;
            alerter.FormShowingEffect = AlertFormShowingEffect.MoveHorizontal;
            alerter.FormLocation = AlertFormLocation.TopRight;
            alerter.BeforeFormShow += alerter_BeforeFormShow;
        }

        private void alerter_BeforeFormShow(object sender, AlertFormEventArgs e)
        {
            e.Location = new Point(subForm.Location.X + subForm.ActiveControl.Size.Width - 185, subForm.Location.Y);
        }

        private Size currentFormSize = new Size(0, 0);




        public void ShowAlertInformation(string message, string messageType = "")
        {
            CreateAlertControl();

            if (subForm == null)
            {
                return;
            }
            if (messageType == "ERROR")
            {
                alerter.Show(subForm, "Error Message", message);
            }
            else
            {
                if (messageType == "MESSAGE")
                {
                    // alerter.Show(subForm, "Information", message, ERP.Client.UI_Logic.PMS.Properties.Resources.information);
                    alerter.Show(subForm, "Message", message);
                }
                else
                {
                    alerter.Show(subForm, "Message", message);
                }
            }
        }

    }

    public delegate void CreatingEventHandler(CNETFormEventArgs e);
    public delegate void SavingEventHandler(CNETFormEventArgs e);
    public delegate void DeletingEventHandler(CNETFormEventArgs e);

    public class CNETFormEventArgs : EventArgs
    {
        public CNETFormEventArgs(UILogicBase logicBase)
        {
            LogicBase = logicBase;
        }
        public UILogicBase LogicBase { get; set; }
    }

    public class CNETRibbon
    {
        public RibbonControl ribbonControl { get; set; }
    }
}
